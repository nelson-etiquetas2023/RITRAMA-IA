using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.VendedorService
{
    /// <summary>
    /// Servicio de vendedores: listado del módulo de Vendedores y operaciones de alta/edición.
    /// La conexión se resuelve del ambiente activo (nunca hardcodeada) con el mismo
    /// resolver centralizado que ClienteService/ProveedorService.
    /// </summary>
    public sealed class VendedorService : IVendedorService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Crea el servicio de vendedores.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación para resolver la cadena de conexión.</param>
        /// <exception cref="ArgumentNullException">Si la configuración es nula.</exception>
        /// <exception cref="InvalidOperationException">Si no se puede resolver la cadena de conexión.</exception>
        public VendedorService(IConfiguration configuration)
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
        /// Trae todos los vendedores (anulado = 0 y 1) con el estado traducido a texto.
        /// Devuelve las tres columnas del grid (vendor_id, vendor_name, status)
        /// y el resto de campos de la tabla vendedor que pinta la página de detalle.
        /// </summary>
        public async Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DataTable dt = new DataTable();
            dt.Columns.Add("vendor_id", typeof(string));
            dt.Columns.Add("vendor_name", typeof(string));
            dt.Columns.Add("correo", typeof(string));
            dt.Columns.Add("phone", typeof(string));
            dt.Columns.Add("zona", typeof(string));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("codigo_interno", typeof(int));

            // Solo columnas de la tabla vendedor (sin joins ni valores externos, así que
            // no hay nada que parametrizar); status traduce el bit anulado a texto.
            // codigo_interno es 0 en filas legadas anteriores a la migración.
            const string sql = @"
                SELECT vendor_id,
                       vendor_name,
                       correo,
                       phone,
                       zona,
                       CASE WHEN anulado = 0 THEN 'activo' ELSE 'desactivado' END AS status,
                       COALESCE(codigo_interno, 0) AS codigo_interno
                FROM vendedor
                ORDER BY vendor_name";

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
                    reader.IsDBNull(6) ? 0 : Convert.ToInt32(reader.GetValue(6)));
            }

            return dt;
        }

        /// <summary>
        /// Inserta un vendedor con validación de dominio.
        /// </summary>
        public async Task<Result<bool>> AddValidatedAsync(Vendedor vendedor, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Result validacion = ValidateVendedor(vendedor);
            if (!validacion.IsSuccess)
            {
                return Result<bool>.Failure(validacion.Error ?? "Datos de vendedor inválidos");
            }

            // Verificar duplicado
            Result<bool> existe = await ExistsAsync(vendedor.Vendor_id, cancellationToken);
            if (!existe.IsSuccess)
            {
                return Result<bool>.Failure(existe.Error ?? "Error al verificar existencia");
            }
            if (existe.Value)
            {
                return Result<bool>.Failure("El código de vendedor ya existe.");
            }

            const string sql = @"
                INSERT INTO vendedor (
                    vendor_id, vendor_name, correo, phone, zona, anulado, codigo_interno
                ) VALUES (
                    @vendor_id, @vendor_name, @correo, @phone, @zona, @anulado, @codigo_interno
                )";

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(sql, conn);
                // Los null se coalescen a DBNull: AddWithValue(null) hace que SqlClient omita
                // el parámetro y el servidor responde "se esperaba @x, no proporcionado".
                cmd.Parameters.AddWithValue("@vendor_id", vendedor.Vendor_id);
                cmd.Parameters.AddWithValue("@vendor_name", vendedor.Vendor_name);
                cmd.Parameters.AddWithValue("@correo", (object?)vendedor.Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", (object?)vendedor.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@zona", (object?)vendedor.Zona ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@anulado", vendedor.Anulado ? 1 : 0);
                cmd.Parameters.AddWithValue("@codigo_interno", vendedor.CodigoInterno);

                await conn.OpenAsync(cancellationToken);
                int filas = await cmd.ExecuteNonQueryAsync(cancellationToken);
                return Result<bool>.Success(filas > 0);
            }
            catch (SqlException ex)
            {
                return Result<bool>.Failure($"Error de base de datos al insertar vendedor: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un vendedor existente, incluido su estado (activar/desactivar).
        /// </summary>
        public async Task<Result<bool>> UpdateValidatedAsync(Vendedor vendedor, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Result validacion = ValidateVendedor(vendedor);
            if (!validacion.IsSuccess)
            {
                return Result<bool>.Failure(validacion.Error ?? "Datos de vendedor inválidos");
            }

            // Verificar que existe
            Result<bool> existe = await ExistsAsync(vendedor.Vendor_id, cancellationToken);
            if (!existe.IsSuccess)
            {
                return Result<bool>.Failure(existe.Error ?? "Error al verificar existencia");
            }
            if (!existe.Value)
            {
                return Result<bool>.Failure($"El vendedor '{vendedor.Vendor_id}' no existe.");
            }

            const string sql = @"
                UPDATE vendedor SET
                    vendor_name = @vendor_name,
                    correo = @correo,
                    phone = @phone,
                    zona = @zona,
                    anulado = @anulado
                WHERE vendor_id = @vendor_id";

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(sql, conn);
                // Mismo blindaje que en AddValidatedAsync: los null se coalescen a DBNull.
                cmd.Parameters.AddWithValue("@vendor_id", vendedor.Vendor_id);
                cmd.Parameters.AddWithValue("@vendor_name", vendedor.Vendor_name);
                cmd.Parameters.AddWithValue("@correo", (object?)vendedor.Correo ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", (object?)vendedor.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@zona", (object?)vendedor.Zona ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@anulado", vendedor.Anulado ? 1 : 0);

                await conn.OpenAsync(cancellationToken);
                int filas = await cmd.ExecuteNonQueryAsync(cancellationToken);
                return Result<bool>.Success(filas > 0);
            }
            catch (SqlException ex)
            {
                return Result<bool>.Failure($"Error de base de datos al actualizar vendedor: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si un código de vendedor ya existe en BD.
        /// </summary>
        public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<bool>.Failure("El código de vendedor no puede estar vacío.");
            }

            const string sql = "SELECT 1 FROM vendedor WHERE vendor_id = @id";

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
                return Result<bool>.Failure($"Error de base de datos al verificar vendedor: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida solo reglas de dominio (sin I/O) para un vendedor.
        /// </summary>
        public Result ValidateVendedor(Vendedor vendedor)
        {
            if (vendedor is null)
            {
                return Result.Failure("El vendedor no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(vendedor.Vendor_id))
            {
                return Result.Failure("El código de vendedor es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(vendedor.Vendor_name))
            {
                return Result.Failure("El nombre del vendedor es obligatorio.");
            }

            // El sistema genera GUID (36 caracteres) al dar de alta, así que el tope
            // es 50, igual que product_id en Productos.
            if (vendedor.Vendor_id.Length > 50)
            {
                return Result.Failure("El código de vendedor no puede exceder 50 caracteres.");
            }

            if (vendedor.Vendor_name.Length > 100)
            {
                return Result.Failure("El nombre del vendedor no puede exceder 100 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(vendedor.Correo) && vendedor.Correo.Length > 100)
            {
                return Result.Failure("El correo no puede exceder 100 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(vendedor.Phone) && vendedor.Phone.Length > 20)
            {
                return Result.Failure("El teléfono no puede exceder 20 caracteres.");
            }

            return Result.Success();
        }

        /// <summary>Lee una columna como texto (NULL → cadena vacía).</summary>
        private static object Texto(SqlDataReader reader, int indice)
            => reader.IsDBNull(indice)
                ? string.Empty
                : Convert.ToString(reader.GetValue(indice)) ?? string.Empty;
    }
}