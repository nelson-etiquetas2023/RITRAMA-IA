using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ClienteService
{
    /// <summary>
    /// Servicio de clientes: listado del módulo de Clientes y operaciones de alta/edición.
    /// La conexión se resuelve del ambiente activo (nunca hardcodeada) con el mismo
    /// resolver centralizado que ProductsService/OrdenCorteService.
    /// </summary>
    public sealed class ClienteService : IClienteService
    {
        private readonly string _connectionString;

        /// <summary>
        /// Crea el servicio de clientes.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación para resolver la cadena de conexión.</param>
        /// <exception cref="ArgumentNullException">Si la configuración es nula.</exception>
        /// <exception cref="InvalidOperationException">Si no se puede resolver la cadena de conexión.</exception>
        public ClienteService(IConfiguration configuration)
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
        /// Trae todos los clientes (anulado = 0 y 1) con el estado traducido a texto.
        /// Devuelve las tres columnas del grid (customer_id, customer_name, status)
        /// y el resto de campos de la tabla customer que pinta la página de detalle.
        /// </summary>
        public async Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DataTable dt = new DataTable();
            dt.Columns.Add("customer_id", typeof(string));
            dt.Columns.Add("customer_name", typeof(string));
            dt.Columns.Add("identificacion", typeof(string));
            dt.Columns.Add("empresa", typeof(string));
            dt.Columns.Add("customer_category", typeof(string));
            dt.Columns.Add("phone", typeof(string));
            dt.Columns.Add("contacto", typeof(string));
            dt.Columns.Add("persona_contacto", typeof(string));
            dt.Columns.Add("customer_email", typeof(string));
            dt.Columns.Add("condicion_pago", typeof(string));
            dt.Columns.Add("impuesto", typeof(short));
            dt.Columns.Add("direccion_facturacion", typeof(string));
            dt.Columns.Add("direccion_entrega", typeof(string));
            dt.Columns.Add("unity1", typeof(bool));
            dt.Columns.Add("unity2", typeof(bool));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("codigo_interno", typeof(int));

            // Solo columnas de la tabla customer (sin joins ni valores externos, así que
            // no hay nada que parametrizar); status traduce el bit anulado a texto.
            // codigo_interno es 0 en filas legadas anteriores a la migración.
            const string sql = @"
                SELECT customer_id,
                       customer_name,
                       identificacion,
                       empresa,
                       customer_category,
                       phone,
                       contacto,
                       persona_contacto,
                       customer_email,
                       condicion_pago,
                       impuesto,
                       direccion_facturacion,
                       direccion_entrega,
                       unity1,
                       unity2,
                       CASE WHEN anulado = 0 THEN 'activo' ELSE 'desactivado' END AS status,
                       COALESCE(codigo_interno, 0) AS codigo_interno
                FROM customer
                ORDER BY customer_name";

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
                    Texto(reader, 7),
                    Texto(reader, 8),
                    Texto(reader, 9),
                    reader.IsDBNull(10) ? DBNull.Value : reader.GetValue(10),
                    Texto(reader, 11),
                    Texto(reader, 12),
                    reader.IsDBNull(13) ? DBNull.Value : reader.GetValue(13),
                    reader.IsDBNull(14) ? DBNull.Value : reader.GetValue(14),
                    reader.IsDBNull(15) ? string.Empty : reader.GetString(15),
                    reader.IsDBNull(16) ? 0 : Convert.ToInt32(reader.GetValue(16)));
            }

            return dt;
        }

        /// <summary>
        /// Inserta un cliente con validación de dominio.
        /// </summary>
        public async Task<Result<bool>> AddValidatedAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Result validacion = ValidateCliente(cliente);
            if (!validacion.IsSuccess)
            {
                return Result<bool>.Failure(validacion.Error ?? "Datos de cliente inválidos");
            }

            // La categoría se elige en combo y es obligatoria solo al crear: al editar se
            // preserva la guardada aunque sea un valor legado fuera del combo.
            if (!EsCategoriaValida(cliente.Customer_category))
            {
                return Result<bool>.Failure("Seleccione la categoría del cliente (Nacional o Internacional).");
            }

            // Verificar duplicado
            Result<bool> existe = await ExistsAsync(cliente.Customer_id, cancellationToken);
            if (!existe.IsSuccess)
            {
                return Result<bool>.Failure(existe.Error ?? "Error al verificar existencia");
            }
            if (existe.Value)
            {
                return Result<bool>.Failure("El código de cliente ya existe.");
            }

            const string sql = @"
                INSERT INTO customer (
                    customer_id, customer_name, identificacion, empresa, customer_category,
                    phone, contacto, persona_contacto, customer_email, condicion_pago, impuesto,
                    direccion_facturacion, direccion_entrega, unity1, unity2, anulado,
                    codigo_interno
                ) VALUES (
                    @customer_id, @customer_name, @identificacion, @empresa, @customer_category,
                    @phone, @contacto, @persona_contacto, @customer_email, @condicion_pago, @impuesto,
                    @direccion_facturacion, @direccion_entrega, @unity1, @unity2, @anulado,
                    @codigo_interno
                )";

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(sql, conn);
                // Los campos sin caja en el detalle (identificación, empresa, condición...)
                // llegan null: AddWithValue(null) hace que SqlClient OMITA el parámetro y el
                // servidor responde "se esperaba @x, no proporcionado". Se coalesce a DBNull.
                cmd.Parameters.AddWithValue("@customer_id", cliente.Customer_id);
                cmd.Parameters.AddWithValue("@customer_name", cliente.Customer_name);
                cmd.Parameters.AddWithValue("@identificacion", (object?)cliente.Identificacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@empresa", (object?)cliente.Empresa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@customer_category", (object?)cliente.Customer_category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", (object?)cliente.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@contacto", (object?)cliente.Contacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@persona_contacto", (object?)cliente.PersonaContacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@customer_email", (object?)cliente.Customer_email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@condicion_pago", (object?)cliente.Condicion_pago ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@impuesto", cliente.Impuesto);
                cmd.Parameters.AddWithValue("@direccion_facturacion", (object?)cliente.Direccion_facturacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@direccion_entrega", (object?)cliente.Direccion_entrega ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@unity1", cliente.Unity1);
                cmd.Parameters.AddWithValue("@unity2", cliente.Unity2);
                cmd.Parameters.AddWithValue("@anulado", cliente.Anulado ? 1 : 0);
                cmd.Parameters.AddWithValue("@codigo_interno", cliente.CodigoInterno);

                await conn.OpenAsync(cancellationToken);
                int filas = await cmd.ExecuteNonQueryAsync(cancellationToken);
                return Result<bool>.Success(filas > 0);
            }
            catch (SqlException ex)
            {
                return Result<bool>.Failure($"Error de base de datos al insertar cliente: {ex.Message}");
            }
        }

        /// <summary>
        /// Actualiza un cliente existente, incluido su estado (activar/desactivar).
        /// </summary>
        public async Task<Result<bool>> UpdateValidatedAsync(Cliente cliente, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            Result validacion = ValidateCliente(cliente);
            if (!validacion.IsSuccess)
            {
                return Result<bool>.Failure(validacion.Error ?? "Datos de cliente inválidos");
            }

            // Verificar que existe
            Result<bool> existe = await ExistsAsync(cliente.Customer_id, cancellationToken);
            if (!existe.IsSuccess)
            {
                return Result<bool>.Failure(existe.Error ?? "Error al verificar existencia");
            }
            if (!existe.Value)
            {
                return Result<bool>.Failure($"El cliente '{cliente.Customer_id}' no existe.");
            }

            const string sql = @"
                UPDATE customer SET
                    customer_name = @customer_name,
                    identificacion = @identificacion,
                    empresa = @empresa,
                    customer_category = @customer_category,
                    phone = @phone,
                    contacto = @contacto,
                    persona_contacto = @persona_contacto,
                    customer_email = @customer_email,
                    condicion_pago = @condicion_pago,
                    impuesto = @impuesto,
                    direccion_facturacion = @direccion_facturacion,
                    direccion_entrega = @direccion_entrega,
                    unity1 = @unity1,
                    unity2 = @unity2,
                    anulado = @anulado
                WHERE customer_id = @customer_id";

            try
            {
                using SqlConnection conn = new SqlConnection(_connectionString);
                using SqlCommand cmd = new SqlCommand(sql, conn);
                // Ver comentario en AddValidatedAsync: los null se coalescen a DBNull para
                // que SqlClient no omita el parámetro ("se esperaba @x, no proporcionado").
                cmd.Parameters.AddWithValue("@customer_id", cliente.Customer_id);
                cmd.Parameters.AddWithValue("@customer_name", cliente.Customer_name);
                cmd.Parameters.AddWithValue("@identificacion", (object?)cliente.Identificacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@empresa", (object?)cliente.Empresa ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@customer_category", (object?)cliente.Customer_category ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@phone", (object?)cliente.Phone ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@contacto", (object?)cliente.Contacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@persona_contacto", (object?)cliente.PersonaContacto ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@customer_email", (object?)cliente.Customer_email ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@condicion_pago", (object?)cliente.Condicion_pago ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@impuesto", cliente.Impuesto);
                cmd.Parameters.AddWithValue("@direccion_facturacion", (object?)cliente.Direccion_facturacion ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@direccion_entrega", (object?)cliente.Direccion_entrega ?? DBNull.Value);
                cmd.Parameters.AddWithValue("@unity1", cliente.Unity1);
                cmd.Parameters.AddWithValue("@unity2", cliente.Unity2);
                cmd.Parameters.AddWithValue("@anulado", cliente.Anulado ? 1 : 0);

                await conn.OpenAsync(cancellationToken);
                int filas = await cmd.ExecuteNonQueryAsync(cancellationToken);
                return Result<bool>.Success(filas > 0);
            }
            catch (SqlException ex)
            {
                return Result<bool>.Failure($"Error de base de datos al actualizar cliente: {ex.Message}");
            }
        }

        /// <summary>
        /// Verifica si un código de cliente ya existe en BD.
        /// </summary>
        public async Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            if (string.IsNullOrWhiteSpace(id))
            {
                return Result<bool>.Failure("El código de cliente no puede estar vacío.");
            }

            const string sql = "SELECT 1 FROM customer WHERE customer_id = @id";

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
                return Result<bool>.Failure($"Error de base de datos al verificar cliente: {ex.Message}");
            }
        }

        /// <summary>
        /// Valida solo reglas de dominio (sin I/O) para un cliente.
        /// </summary>
        public Result ValidateCliente(Cliente cliente)
        {
            if (cliente is null)
            {
                return Result.Failure("El cliente no puede ser nulo.");
            }

            if (string.IsNullOrWhiteSpace(cliente.Customer_id))
            {
                return Result.Failure("El código de cliente es obligatorio.");
            }

            if (string.IsNullOrWhiteSpace(cliente.Customer_name))
            {
                return Result.Failure("El nombre del cliente es obligatorio.");
            }

            // El sistema genera GUID (36 caracteres) al dar de alta, así que el tope
            // es 50, igual que product_id en Productos.
            if (cliente.Customer_id.Length > 50)
            {
                return Result.Failure("El código de cliente no puede exceder 50 caracteres.");
            }

            if (cliente.Customer_name.Length > 100)
            {
                return Result.Failure("El nombre del cliente no puede exceder 100 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Customer_email) && cliente.Customer_email.Length > 100)
            {
                return Result.Failure("El email no puede exceder 100 caracteres.");
            }

            if (!string.IsNullOrWhiteSpace(cliente.Phone) && cliente.Phone.Length > 20)
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