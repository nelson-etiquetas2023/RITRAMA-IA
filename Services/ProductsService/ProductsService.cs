using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ProductsService
{
    /// <summary>
    /// Servicio de productos: CRUD del catálogo producto con validaciones de dominio,
    /// acceso parametrizado y compatibilidad con el contrato legacy (bool/DataSet).
    /// Reglas: 4 bits exclusivos (Master/RolloCortado/Resmas/Graphics) y estado del producto:
    /// un anulado solo se puede reactivar, no editar en el mismo estado.
    /// </summary>
    public sealed class ProductsService : IProductsService
    {
        private readonly string _connectionString;
        private readonly IServiceCommonData _commonData;
        private readonly IConfiguration _configuration;

        /// <summary>
        /// Cadena de conexión resuelta del ambiente activo. Nunca hardcodeada; proviene de IConfiguration.
        /// Se expone para compatibilidad con código legacy que la leía directamente.
        /// </summary>
        public string StringConnex => _connectionString;

        /// <summary>
        /// Configuración inyectada (solo lectura, validada).
        /// </summary>
        public IConfiguration Configuration => _configuration;

        /// <summary>
        /// Crea el servicio de productos.
        /// </summary>
        /// <param name="commonData">Servicio común de datos (validado no nulo, aunque actualmente no se usa directamente en Productos).</param>
        /// <param name="configuration">Configuración de la aplicación para resolver la cadena de conexión.</param>
        /// <exception cref="ArgumentNullException">Si alguna dependencia es nula.</exception>
        /// <exception cref="InvalidOperationException">Si no se puede resolver la cadena de conexión.</exception>
        public ProductsService(IServiceCommonData commonData, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(commonData);
            ArgumentNullException.ThrowIfNull(configuration);

            _commonData = commonData;
            _configuration = configuration;
            _connectionString = ResolveConnectionString(configuration);
        }

        private static string ResolveConnectionString(IConfiguration config)
        {
            // Reusa el resolver centralizado que usan OrdenCorteService/ConsecutivosService.
            // Evita duplicar la lógica de Ambiente → ConnectionStringsEnvironment[ambiente].
            string cs = ConexionResolver.Resolver(config);
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException("No se pudo resolver la cadena de conexión para el ambiente configurado. Verifique appsettings.json / User Secrets.");
            }

            return cs;
        }

        // ─────────────────────────────────────────────────────────────────
        // Load (compatibilidad DataSet) + LoadTyped (nuevo)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Carga el catálogo de productos como DataSet (compatibilidad con FrmProductos).
        /// Usa SQL parametrizado centralizado en R.cs y dispose correcto de conexión.
        /// </summary>
        /// <param name="cancellationToken">Token de cancelación.</param>
        /// <returns>DataSet con tabla DtProducts.</returns>
        public async Task<DataSet> Load(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DataSet ds = new();
            DataTable dt = await LoadDataTableAsync(R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS, null, cancellationToken).ConfigureAwait(false);
            dt.TableName = "DtProducts";
            ds.Tables.Add(dt);
            return ds;
        }

        /// <summary>
        /// Carga tipada como lista de <see cref="Product"/> usando <see cref="ProductMapper"/> (Core).
        /// Evita exponer DataSet en código nuevo.
        /// </summary>
        public async Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();
                DataTable dt = await LoadDataTableAsync(R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS, null, cancellationToken).ConfigureAwait(false);
                IReadOnlyList<Product> lista = ProductMapper.FromDataTable(dt);
                return Result<IReadOnlyList<Product>>.Success(lista);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.LoadTypedAsync", ex);
                return Result<IReadOnlyList<Product>>.Failure("Error al cargar los productos: " + ex.Message);
            }
        }

        private async Task<DataTable> LoadDataTableAsync(string sql, SqlParameter[]? parameters, CancellationToken ct)
        {
            using SqlConnection conn = new(_connectionString);
            await conn.OpenAsync(ct).ConfigureAwait(false);

            using SqlCommand cmd = new()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = sql
            };
            if (parameters != null && parameters.Length > 0)
            {
                cmd.Parameters.AddRange(parameters);
            }

            using SqlDataReader reader = await cmd.ExecuteReaderAsync(CommandBehavior.CloseConnection, ct).ConfigureAwait(false);
            DataTable table = new();
            table.Load(reader);
            return table;
        }

        // ─────────────────────────────────────────────────────────────────
        // Add
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Inserta un producto (firma legacy bool). Internamente delega a <see cref="AddValidatedAsync"/> y
        /// traduce Result a bool + notificación vía <see cref="ServiceErrors"/>.
        /// </summary>
        public async Task<bool> Add(Product producto)
        {
            Result<bool> result = await AddValidatedAsync(producto).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                ServiceErrors.Report(result.Error!);
                return false;
            }
            return result.Value;
        }

        /// <summary>
        /// Inserta con validación de dominio (Result). Valida categoría exclusiva y parámetros obligatorios
        /// antes de tocar la BD, luego ejecuta INSERT parametrizado con using/dispose correctos.
        /// </summary>
        public async Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(producto);
            cancellationToken.ThrowIfCancellationRequested();

            Result validation = ValidateProduct(producto);
            if (!validation.IsSuccess)
            {
                return Result<bool>.Failure(validation.Error!, validation.ErrorCode);
            }

            // Evitar duplicado: el codigo Ritrama ES el product_id, asi que basta con la
            // comprobacion de la clave para cubrir tambien el unico del indice primario.
            Result<bool> exists = await ExistsAsync(producto.Product_id, cancellationToken).ConfigureAwait(false);
            if (exists.IsSuccess && exists.Value)
            {
                return Result<bool>.Failure($"Ya existe un producto con el código '{producto.Product_id}'.", ProductValidator.CODE_REQUIRED);
            }

            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.SQL_STRING_QUERY.INSERT_PRODUCT
                };
                AddProductParameters(cmd, producto);
                int rows = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                return Result<bool>.Success(rows > 0);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.AddValidatedAsync", ex);
                return Result<bool>.Failure("Error al agregar el producto: " + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Update
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Actualiza un producto (firma legacy bool).
        /// </summary>
        public async Task<bool> Update(Product producto)
        {
            Result<bool> result = await UpdateValidatedAsync(producto).ConfigureAwait(false);
            if (!result.IsSuccess)
            {
                ServiceErrors.Report(result.Error!);
                return false;
            }
            return result.Value;
        }

        /// <summary>
        /// Actualiza con validación de dominio y verificación de categoría exclusiva.
        /// Persiste también el estado: un producto vigente se puede desactivar y uno anulado se
        /// puede reactivar, pero un anulado que sigue anulado no se guarda.
        /// La fila se localiza por el consecutivo (identidad estable): el codigo (product_id)
        /// es editable, asi que si cambia se propaga en la misma transaccion a todas las
        /// tablas que lo referencian (MasterInic.part_number, orden_corte, rolls_details...)
        /// para no dejar inventario ni cortes huerfanos.
        /// </summary>
        public async Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default)
        {
            ArgumentNullException.ThrowIfNull(producto);
            cancellationToken.ThrowIfCancellationRequested();

            Result validation = ValidateProduct(producto);
            if (!validation.IsSuccess)
            {
                return Result<bool>.Failure(validation.Error!, validation.ErrorCode);
            }

            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                ProductoActual? actual = await GetByConsecutivoAsync(conn, producto.IdConsec, cancellationToken).ConfigureAwait(false);
                if (actual is null)
                {
                    // Sin consecutivo utilizable (objeto legacy) se localiza por el codigo y,
                    // en ese caso, no hay codigo anterior que propagar.
                    if (producto.IdConsec > 0)
                    {
                        return Result<bool>.Failure($"No existe un producto con el consecutivo {producto.IdConsec}.", ProductValidator.CODE_REQUIRED);
                    }

                    bool? anuladoPorCodigo = await GetAnuladoFlagAsync(conn, producto.Product_id, cancellationToken).ConfigureAwait(false);
                    if (anuladoPorCodigo == null)
                    {
                        return Result<bool>.Failure($"No existe un producto con el código '{producto.Product_id}'.", ProductValidator.CODE_REQUIRED);
                    }

                    actual = new ProductoActual(producto.Product_id, anuladoPorCodigo.Value);
                }

                // Regla de estado: un vigente se edita siempre (y se puede desactivar o reactivar
                // desde el formulario); un anulado solo admite el cambio si la operacion lo
                // reactiva. Verificar el estado actual en BD, no el que trae el objeto.
                Result estado = ProductValidator.ValidateEditableState(
                    actual.Anulado,
                    !producto.Anulado,
                    actual.ProductId);
                if (!estado.IsSuccess)
                {
                    return Result<bool>.Failure(estado.Error!, estado.ErrorCode);
                }

                bool codigoCambiado = !string.Equals(actual.ProductId, producto.Product_id, StringComparison.OrdinalIgnoreCase);
                if (codigoCambiado)
                {
                    // El codigo nuevo pasa a ser la clave primaria: comprobar que no la ocupe
                    // otro producto (el indice primario lanzaria SqlException sin mensaje claro).
                    Result<bool> yaExiste = await ExistsAsync(producto.Product_id, cancellationToken).ConfigureAwait(false);
                    if (yaExiste.IsSuccess && yaExiste.Value)
                    {
                        return Result<bool>.Failure($"Ya existe un producto con el código '{producto.Product_id}'.", ProductValidator.CODE_REQUIRED);
                    }
                }

                using SqlTransaction tx = conn.BeginTransaction();
                try
                {
                    using SqlCommand cmd = new()
                    {
                        Connection = conn,
                        Transaction = tx,
                        CommandType = CommandType.Text,
                        CommandText = R.SQL_STRING_QUERY.UPDATE_PRODUCT
                    };
                    AddUpdateParameters(cmd, producto, actual.ProductId);
                    int rows = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                    if (rows == 0)
                    {
                        tx.Rollback();
                        return Result<bool>.Failure($"No se actualizó ningún producto con el código '{actual.ProductId}'.", ProductValidator.CODE_REQUIRED);
                    }

                    if (codigoCambiado)
                    {
                        await PropagarCambioCodigoAsync(conn, tx, actual.ProductId, producto.Product_id, cancellationToken).ConfigureAwait(false);
                    }

                    tx.Commit();
                }
                catch
                {
                    try
                    {
                        tx.Rollback();
                    }
                    catch (Exception rbEx)
                    {
                        // La transaccion ya puede estar cerrada por el propio fallo: el error
                        // que importa es el original, este solo no debe taparlo.
                        ServiceLogger.LogError("ProductsService.UpdateValidatedAsync.Rollback", rbEx);
                    }

                    throw;
                }

                return Result<bool>.Success(true);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.UpdateValidatedAsync", ex);
                return Result<bool>.Failure("Error al modificar los datos del producto: " + ex.Message);
            }
        }

        /// <summary>Par de valores que devuelve <see cref="GetByConsecutivoAsync"/>.</summary>
        private sealed record ProductoActual(string ProductId, bool Anulado);

        /// <summary>
        /// Lee el codigo actual y el estado de un producto por su consecutivo (clave estable
        /// del catalogo). Devuelve null si el consecutivo no existe.
        /// </summary>
        private static async Task<ProductoActual?> GetByConsecutivoAsync(SqlConnection conn, int idConsec, CancellationToken ct)
        {
            if (idConsec <= 0)
            {
                return null;
            }

            using SqlCommand cmd = new()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = R.SQL_STRING_QUERY.SELECT_PRODUCT_BY_IDCONSEC
            };
            cmd.Parameters.Add(new SqlParameter("@idconsec", SqlDbType.Int) { Value = idConsec });

            using SqlDataReader reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
            if (!await reader.ReadAsync(ct).ConfigureAwait(false))
            {
                return null;
            }

            string productId = reader.GetString(0);
            bool anulado = reader.IsDBNull(1) ? false : Convert.ToBoolean(reader.GetValue(1));
            return new ProductoActual(productId, anulado);
        }

        /// <summary>
        /// Propaga el cambio de codigo a todas las tablas que referencian el producto.
        /// La lista se descubre en runtime contra INFORMATION_SCHEMA: asi no hay que mantener
        /// a mano un listado que se rompe en cuanto aparece una tabla nueva (item_despacho,
        /// ItemsMateria, orden_corte, pedido_detalle, rolls_details, RollsInic, ...).
        /// Si el codigo nuevo no cabe en alguna columna, se avisa y no se aplica nada.
        /// </summary>
        private static async Task PropagarCambioCodigoAsync(SqlConnection conn, SqlTransaction tx, string codigoViejo, string codigoNuevo, CancellationToken ct)
        {
            List<(string Tabla, string Columna, int Largo)> referencias = [];

            const string descubrir =
                "SELECT TABLE_NAME, COLUMN_NAME, CHARACTER_MAXIMUM_LENGTH " +
                "FROM INFORMATION_SCHEMA.COLUMNS " +
                "WHERE UPPER(COLUMN_NAME) IN ('PRODUCT_ID','PART_NUMBER') " +
                "  AND UPPER(TABLE_NAME) <> 'PRODUCTO'";

            using (SqlCommand cmd = new(descubrir, conn, tx))
            using (SqlDataReader reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false))
            {
                while (await reader.ReadAsync(ct).ConfigureAwait(false))
                {
                    referencias.Add((reader.GetString(0), reader.GetString(1), reader.IsDBNull(2) ? -1 : reader.GetInt32(2)));
                }
            }

            string nuevo = codigoNuevo.Trim();
            foreach ((string tabla, string columna, int largo) in referencias)
            {
                // largo -1 = columna no de texto (no aplica). Si no cabe, mejor fallar aqui que
                // truncar en silencio: un part_number recortado deja el master invisible.
                if (largo > 0 && nuevo.Length > largo)
                {
                    throw new InvalidOperationException(
                        $"El código '{nuevo}' no cabe en {tabla}.{columna} (máximo {largo} caracteres).");
                }

                using SqlCommand upd = new($"UPDATE [{tabla}] SET [{columna}] = @nuevo WHERE [{columna}] = @viejo", conn, tx);
                upd.Parameters.Add(new SqlParameter("@nuevo", SqlDbType.NVarChar, largo > 0 ? largo : 100) { Value = nuevo });
                upd.Parameters.Add(new SqlParameter("@viejo", SqlDbType.NVarChar, 100) { Value = codigoViejo.Trim() });
                await upd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
            }
        }

        private static async Task<bool?> GetAnuladoFlagAsync(SqlConnection conn, string productId, CancellationToken ct)
        {
            using SqlCommand cmd = new()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = R.SQL_STRING_QUERY.SELECT_PRODUCT_ANULADO
            };
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = productId });
            object? val = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
            if (val == null || val == DBNull.Value)
            {
                return null;
            }

            if (val is bool b)
            {
                return b;
            }

            if (val is byte by)
            {
                return by != 0;
            }

            if (val is int i)
            {
                return i != 0;
            }

            if (bool.TryParse(val.ToString(), out bool parsed))
            {
                return parsed;
            }

            return Convert.ToInt32(val) != 0;
        }

        // ─────────────────────────────────────────────────────────────────
        // ValidProductid / ExistsAsync
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Verifica si un código de producto ya existe (sincrónico, compatibilidad).
        /// Usa SQL parametrizado y dispose correcto. Retorna false en caso de error de infraestructura.
        /// </summary>
        /// <param name="id">Código a verificar.</param>
        /// <returns>True si existe.</returns>
        public bool ValidProductid(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return false;
            }

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.SQL_STRING_QUERY.SELECT_PRODUCT_EXISTS
                };
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = id.Trim() });
                int count = Convert.ToInt32(cmd.ExecuteScalar());
                return count > 0;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.ValidProductid", ex);
                return false;
            }
        }

        /// <summary>
        /// Verifica existencia de forma asíncrona con Result y CancellationToken.
        /// </summary>
        public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<bool>.Failure("El código de producto no puede estar vacío.", ProductValidator.CODE_REQUIRED);
            }

            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.SQL_STRING_QUERY.SELECT_PRODUCT_EXISTS
                };
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = id.Trim() });
                object? scalar = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                int count = Convert.ToInt32(scalar);
                return Result<bool>.Success(count > 0);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.ExistsAsync", ex);
                return Result<bool>.Failure("Error al verificar el código de producto: " + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Anular
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Anula un producto (soft-delete). Firma legacy sincrónica.
        /// </summary>
        public bool Anular(string IdProduct)
        {
            if (string.IsNullOrWhiteSpace(IdProduct))
            {
                return false;
            }

            try
            {
                using SqlConnection conn = new(_connectionString);
                conn.Open();

                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.SQL_STRING_QUERY.UPDATE_PRODUCT_ANULAR
                };
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = IdProduct.Trim() });
                int rows = cmd.ExecuteNonQuery();
                return rows > 0;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.Anular", ex);
                ServiceErrors.Report("Error al anular el producto: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Anula con Result y CancellationToken. Verifica que el producto exista y no esté ya anulado.
        /// </summary>
        public async Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(idProduct))
            {
                return Result<bool>.Failure("El código de producto no puede estar vacío.", ProductValidator.CODE_REQUIRED);
            }

            try
            {
                using SqlConnection conn = new(_connectionString);
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.SQL_STRING_QUERY.UPDATE_PRODUCT_ANULAR
                };
                cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = idProduct.Trim() });
                int rows = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                if (rows == 0)
                {
                    // Distinguir: no existe vs ya anulado
                    Result<bool> exists = await ExistsAsync(idProduct, cancellationToken).ConfigureAwait(false);
                    if (exists.IsSuccess && !exists.Value)
                    {
                        return Result<bool>.Failure($"No existe un producto con el código '{idProduct}'.", ProductValidator.CODE_REQUIRED);
                    }

                    return Result<bool>.Failure($"El producto '{idProduct}' ya está anulado o no se pudo anular.", ProductValidator.CODE_ANULADO);
                }
                return Result<bool>.Success(true);
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.AnularAsync", ex);
                return Result<bool>.Failure("Error al anular el producto: " + ex.Message);
            }
        }

        /// <summary>
        /// Valida solo invariants de dominio sin I/O (útil para validar en UI antes de llamar a Add/Update).
        /// </summary>
        public Result ValidateProduct(Product producto)
        {
            ArgumentNullException.ThrowIfNull(producto);
            return ProductValidator.ValidateForPersistence(producto);
        }

        // ─────────────────────────────────────────────────────────────────
        // Helpers de parámetros (SQL parametrizado, nunca concatenar)
        // ─────────────────────────────────────────────────────────────────

        private static void AddProductParameters(SqlCommand cmd, Product p)
        {
            // Todos los valores se pasan como parámetro; si algún string es null se envía DBNull.Value
            // para no romper la inferencia de tipos y evitar inyección/cultura (decimales con coma).
            cmd.Parameters.Add(new SqlParameter("@product_id", SqlDbType.NVarChar, 50) { Value = (object)p.Product_id ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@idconsec", SqlDbType.Int) { Value = p.IdConsec });
            cmd.Parameters.Add(new SqlParameter("@product_name", SqlDbType.NVarChar, 200) { Value = (object)p.Product_Name ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@product_description", SqlDbType.NVarChar, 500) { Value = (object)p.Product_Description ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@reference", SqlDbType.NVarChar, 50) { Value = (object)p.Referencia ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@codebar", SqlDbType.NVarChar, 50) { Value = (object)p.Codigo_Barra ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@master", SqlDbType.Bit) { Value = p.Master });
            cmd.Parameters.Add(new SqlParameter("@rollo", SqlDbType.Bit) { Value = p.RolloCortado });
            cmd.Parameters.Add(new SqlParameter("@resma", SqlDbType.Bit) { Value = p.Hoja });
            cmd.Parameters.Add(new SqlParameter("@graphics", SqlDbType.Bit) { Value = p.Graphics });
            cmd.Parameters.Add(new SqlParameter("@anulado", SqlDbType.Bit) { Value = p.Anulado });
            cmd.Parameters.Add(new SqlParameter("@precio", SqlDbType.Decimal) { Value = p.Precio, Precision = 18, Scale = 2 });
            cmd.Parameters.Add(new SqlParameter("@costo", SqlDbType.Decimal) { Value = p.Costo, Precision = 18, Scale = 2 });
            cmd.Parameters.Add(new SqlParameter("@ratio", SqlDbType.Decimal) { Value = p.Ratio, Precision = 18, Scale = 4 });
        }

        /// <summary>
        /// Parámetros del UPDATE. <paramref name="idViejo"/> es el codigo actual en BD y
        /// <c>p.Product_id</c> el nuevo: product_id es editable, por eso el WHERE filtra por
        /// el viejo y la escritura usa el nuevo.
        /// </summary>
        private static void AddUpdateParameters(SqlCommand cmd, Product p, string idViejo)
        {
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = (object)p.Product_id ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@id_viejo", SqlDbType.NVarChar, 50) { Value = (object)idViejo ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 200) { Value = (object)p.Product_Name ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@descrip", SqlDbType.NVarChar, 500) { Value = (object)p.Product_Description ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@reference", SqlDbType.NVarChar, 50) { Value = (object)p.Referencia ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@barra", SqlDbType.NVarChar, 50) { Value = (object)p.Codigo_Barra ?? DBNull.Value });
            cmd.Parameters.Add(new SqlParameter("@precio", SqlDbType.Decimal) { Value = p.Precio, Precision = 18, Scale = 2 });
            cmd.Parameters.Add(new SqlParameter("@costo", SqlDbType.Decimal) { Value = p.Costo, Precision = 18, Scale = 2 });
            cmd.Parameters.Add(new SqlParameter("@ratio", SqlDbType.Decimal) { Value = p.Ratio, Precision = 18, Scale = 4 });
            cmd.Parameters.Add(new SqlParameter("@master", SqlDbType.Bit) { Value = p.Master });
            cmd.Parameters.Add(new SqlParameter("@graphics", SqlDbType.Bit) { Value = p.Graphics });
            cmd.Parameters.Add(new SqlParameter("@hoja", SqlDbType.Bit) { Value = p.Hoja });
            cmd.Parameters.Add(new SqlParameter("@rollo", SqlDbType.Bit) { Value = p.RolloCortado });

            // El estado (anulado) se persiste tambien en el Update: es lo que permite activar o
            // desactivar un producto desde el formulario. Lo mantiene en el mismo bit que usa
            // AddProductParameters y que escribe el mapper al leer.
            cmd.Parameters.Add(new SqlParameter("@anulado", SqlDbType.Bit) { Value = p.Anulado });
        }
    }
}
