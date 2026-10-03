using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ProveedorService
{
    /// <summary>
    /// Servicio de proveedores: listado del módulo de Proveedores y operaciones de alta/edición.
    /// La conexión se resuelve del ambiente activo (nunca hardcodeada) con el mismo
    /// resolver centralizado que ClienteService/ProductsService.
    /// </summary>
    public sealed class ProveedorService : IProveedorService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Crea el servicio de proveedores.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación para resolver la cadena de conexión.</param>
        /// <exception cref="ArgumentNullException">Si la configuración es nula.</exception>
        /// <exception cref="InvalidOperationException">Si no se puede resolver la cadena de conexión.</exception>
        public ProveedorService(IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(configuration);
            _connectionString = ResolveConnectionString(configuration);
        }

        private static string ResolveConnectionString(IConfiguration config)
        {
            string cs = ConexionResolver.Resolver(config);
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException("No se pudo resolver la cadena de conexión para el ambiente configurado. Verifique appsettings.json / User Secrets.");
            }

            return cs;
        }

        /// <summary>
        /// Trae todos los proveedores (anulado = 0 y 1) con el estado traducido a texto.
        /// Devuelve las tres columnas del grid (Proveedor_Id, Proveedor_Name, status)
        /// y el resto de campos de la tabla provider que pinta la página de detalle.
        /// </summary>
        public async Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DataTable dt = new DataTable();
            dt.Columns.Add("Proveedor_Id", typeof(string));
            dt.Columns.Add("Proveedor_Name", typeof(string));
            dt.Columns.Add("phone", typeof(string));
            dt.Columns.Add("persona_contacto", typeof(string));
            dt.Columns.Add("direccion", typeof(string));
            dt.Columns.Add("email", typeof(string));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("unidad_master_1", typeof(bool));
            dt.Columns.Add("unidad_master_2", typeof(bool));
            dt.Columns.Add("codigo_interno", typeof(int));
            dt.Columns.Add("categoria", typeof(string));
            dt.Columns.Add("direccion_entrega", typeof(string));

            // Solo columnas de la tabla provider (sin joins ni valores externos, así que
            // no hay nada que parametrizar); status traduce el bit anulado a texto.
            // codigo_interno es 0 y categoria/direccion_entrega son "" en filas legadas
            // anteriores a la migración.
            const string sql = @"
                SELECT Proveedor_Id,
                       Proveedor_Name,
                       phone,
                       persona_contacto,
                       direccion,
                       email,
                       CASE WHEN anulado = 0 THEN 'activo' ELSE 'desactivado' END AS status,
                       unidad_master_1,
                       unidad_master_2,
                       COALESCE(codigo_interno, 0) AS codigo_interno,
                       COALESCE(categoria, '') AS categoria,
                       COALESCE(direccion_entrega, '') AS direccion_entrega
                FROM provider
                ORDER BY Proveedor_Name";

            using SqlConnection conn = new SqlConnection(_connectionString);
            using SqlCommand cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync(cancellationToken);
            using SqlDataReader reader = await cmd.ExecuteReaderAsync(cancellationToken);
            while (await reader.ReadAsync(cancellationToken))
            {
                dt.Rows.Add(
                    Texto(reader, 0),
                    Texto(reader, 1),
                    Texto(reader, 2),
                    Texto(reader, 3),
                    Texto(reader, 4),
                    Texto(reader, 5),
                    Texto(reader, 6),
                    reader.IsDBNull(7) ? DBNull.Value : reader.GetValue(7),
                    reader.IsDBNull(8) ? DBNull.Value : reader.GetValue(8),
                    reader.IsDBNull(9) ? 0 : Convert.ToInt32(reader.GetValue(9)),
                    Texto(reader, 10),
                    Texto(reader, 11));
            }

            return dt;
        }

        /// <summary>
        /// Inserta un proveedor con validación de dominio.
        /// </summary>
        public async Task<Result<bool>> AddValidatedAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Result validacion = ValidateProveedor(proveedor);
            if (!validacion.IsSuccess)
            {
                return Result<bool>.Failure(validacion.Error ?? "Datos de proveedor inválidos");
            }

            // La categoría se elige en combo y es obligatoria solo al crear: al editar se
            // preserva la guardada aunque sea un valor legado fuera del combo.
            if (!EsCategoriaValida(proveedor.Categoria))
            {
                return Result<bool>.Failure("Seleccione la categoría del proveedor (Nacional o Internacional).");
            }

            // Verificar duplicado
            Result<bool> existe = await ExistsAsync(proveedor.Proveedor_Id, cancellationToken);
            if (!existe.IsSuccess)
            {
                return Result<bool>.Failure(existe.Error ?? "Error al verificar existencia");
            }
            if (existe.Value)
            {
                return Result<bool>.Failure("El código de proveedor ya existe.");
            }

            const string sql = @"
                INSERT INTO provider (
                    Proveedor_Id, Proveedor_Name, phone, persona_contacto, direccion, email,
                    unidad_master_1, unidad_master_2, anulado, codigo_interno, categoria,
                    direccion_entrega
                ) VALUES (
                    @Proveedor_Id, @Proveedor_Name, @phone, @persona_contacto, @direccion, @email,
                    @unidad_master_1, @unidad_master_2, @anulado, @codigo_interno, @categoria,
                    @direccion_entrega
                )";

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(sql, conn);
                // Los null se coalescen a DBNull: AddWithValue(null) hace que SqlClient omita
                // el parámetro y el servidor responde "se esperaba @x, no proporcionado".
                cmd.Parameters.AddWithValue("@Proveedor_Id", proveedor.Proveedor_Id);
                cmd.Parameters.AddWithValue("@Proveedor_Name", proveedor.Proveedor_Name);
                cmd.Parameters.AddWithValue("@phone", (object?)proveedor.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@persona_contacto", (object?)proveedor.PersonaContacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@direccion", (object?)proveedor.Direccion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object?)proveedor.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@unidad_master_1", proveedor.Unidad_master_1);
                cmd.Parameters.AddWithValue("@unidad_master_2", proveedor.Unidad_master_2);
                cmd.Parameters.AddWithValue("@anulado", proveedor.Anulado ? 1 : 0);
                cmd.Parameters.AddWithValue("@codigo_interno", proveedor.CodigoInterno);
                cmd.Parameters.AddWithValue("@categoria", (object?)proveedor.Categoria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@direccion_entrega", (object?)proveedor.DireccionEntrega ?? DBNull.Value);

                await conn.OpenAsync(cancellationToken);
                int filas = await cmd.ExecuteNonQueryAsync(cancellationToken);
                return Result<bool>.Success(filas > 0);
            }
            catch (SqlException ex)
            {
                return Result<bool>.Failure($"Error de base de datos al insertar proveedor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un proveedor existente, incluido su estado (activar/desactivar).
        /// </summary>
        public async Task<Result<bool>> UpdateValidatedAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Result validacion = ValidateProveedor(proveedor);
            if (!validacion.IsSuccess)
            {
                return Result<bool>.Failure(validacion.Error ?? "Datos de proveedor inválidos");
            }

            // Verificar que existe
            Result<bool> existe = await ExistsAsync(proveedor.Proveedor_Id, cancellationToken);
            if (!existe.IsSuccess)
            {
                return Result<bool>.Failure(existe.Error ?? "Error al verificar existencia");
            }
            if (!existe.Value)
            {
                return Result<bool>.Failure($"El proveedor '{proveedor.Proveedor_Id}' no existe.");
            }

            const string sql = @"
                UPDATE provider SET
                    Proveedor_Name = @Proveedor_Name,
                    phone = @phone,
                    persona_contacto = @persona_contacto,
                    direccion = @direccion,
                    email = @email,
                    unidad_master_1 = @unidad_master_1,
                    unidad_master_2 = @unidad_master_2,
                    anulado = @anulado,
                    categoria = @categoria,
                    direccion_entrega = @direccion_entrega
                WHERE Proveedor_Id = @Proveedor_Id";

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(sql, conn);
                // Mismo blindaje que en AddValidatedAsync: los null se coalescen a DBNull.
                cmd.Parameters.AddWithValue("@Proveedor_Id", proveedor.Proveedor_Id);
                cmd.Parameters.AddWithValue("@Proveedor_Name", proveedor.Proveedor_Name);
                cmd.Parameters.AddWithValue("@phone", (object?)proveedor.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@persona_contacto", (object?)proveedor.PersonaContacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@direccion", (object?)proveedor.Direccion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@email", (object?)proveedor.Email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@unidad_master_1", proveedor.Unidad_master_1);
                cmd.Parameters.AddWithValue("@unidad_master_2", proveedor.Unidad_master_2);
                cmd.Parameters.AddWithValue("@anulado", proveedor.Anulado ? 1 : 0);
                cmd.Parameters.AddWithValue("@categoria", (object?)proveedor.Categoria ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@direccion_entrega", (object?)proveedor.DireccionEntrega ?? DBNull.Value);

                await conn.OpenAsync(cancellationToken);
                int filas = await cmd.ExecuteNonQueryAsync(cancellationToken);
                return Result<bool>.Success(filas > 0);
            }
            catch (SqlException ex)
            {
                return Result<bool>.Failure($"Error de base de datos al actualizar proveedor: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si un código de proveedor ya existe en BD.
        /// </summary>
        public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<bool>.Failure("El código de proveedor no puede estar vacío.");
            }

            const string sql = "SELECT 1 FROM provider WHERE Proveedor_Id = @id";

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@id", id.Trim());

                await conn.OpenAsync(cancellationToken);
                object? result = await cmd.ExecuteScalarAsync(cancellationToken);
                return Result<bool>.Success(result is not null);
            }
            catch (SqlException ex)
            {
                return Result<bool>.Failure($"Error de base de datos al verificar proveedor: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida solo reglas de dominio (sin I/O) para un proveedor.
        /// </summary>
        public Result ValidateProveedor(Proveedor proveedor)
        {
            if (proveedor is null)
            {
                return Result.Failure("El proveedor no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(proveedor.Proveedor_Id))
            {
                return Result.Failure("El código de proveedor es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(proveedor.Proveedor_Name))
            {
                return Result.Failure("El nombre del proveedor es obligatorio.");
            }

            // El sistema genera GUID (36 caracteres) al dar de alta, así que el tope
            // es 50, igual que product_id en Productos.
            if (proveedor.Proveedor_Id.Length > 50)
            {
                return Result.Failure("El código de proveedor no puede exceder 50 caracteres.");
            }

            if (proveedor.Proveedor_Name.Length > 100)
            {
                return Result.Failure("El nombre del proveedor no puede exceder 100 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(proveedor.Email) && proveedor.Email.Length > 100)
            {
                return Result.Failure("El email no puede exceder 100 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(proveedor.Phone) && proveedor.Phone.Length > 20)
            {
                return Result.Failure("El teléfono no puede exceder 20 caracteres.");
            }

            return Result.Success();
        }

        /// <summary>Categorías válidas del combo (comparación insensible a mayúsculas).</summary>
        private static bool EsCategoriaValida(string? categoria)
            => string.Equals(categoria, "Nacional", StringComparison.OrdinalIgnoreCase)
                || string.Equals(categoria, "Internacional", StringComparison.OrdinalIgnoreCase);

        /// <summary>Lee una columna como texto (NULL → cadena vacía).</summary>
        private static object Texto(SqlDataReader reader, int indice)
            => reader.IsDBNull(indice)
                ? string.Empty
                : Convert.ToString(reader.GetValue(indice)) ?? string.Empty;
    }
}