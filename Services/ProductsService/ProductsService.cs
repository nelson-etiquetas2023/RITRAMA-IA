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

            // Evitar duplicado: si el id ya existe, fallar con mensaje claro (sin excepción como control de flujo).
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

                // Regla de estado: un vigente se edita siempre (y se puede desactivar o reactivar
                // desde el formulario); un anulado solo admite el cambio si la operacion lo
                // reactiva. Verificar el estado actual en BD, no el que trae el objeto.
                bool? anuladoActual = await GetAnuladoFlagAsync(conn, producto.Product_id, cancellationToken).ConfigureAwait(false);
                if (anuladoActual == null)
                {
                    return Result<bool>.Failure($"No existe un producto con el código '{producto.Product_id}'.", ProductValidator.CODE_REQUIRED);
                }

                Result estado = ProductValidator.ValidateEditableState(
                    anuladoActual.Value,
                    !producto.Anulado,
                    producto.Product_id);
                if (!estado.IsSuccess)
                {
                    return Result<bool>.Failure(estado.Error!, estado.ErrorCode);
                }

                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.SQL_STRING_QUERY.UPDATE_PRODUCT
                };
                AddUpdateParameters(cmd, producto);
                int rows = await cmd.ExecuteNonQueryAsync(cancellationToken).ConfigureAwait(false);
                if (rows == 0)
                {
                    return Result<bool>.Failure($"No se actualizó ningún producto con el código '{producto.Product_id}'.", ProductValidator.CODE_REQUIRED);
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

        private static void AddUpdateParameters(SqlCommand cmd, Product p)
        {
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.NVarChar, 50) { Value = (object)p.Product_id ?? DBNull.Value });
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
