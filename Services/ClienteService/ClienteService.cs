using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ClienteService
{
    /// <summary>
    /// Servicio de clientes: por ahora solo el listado del módulo de Clientes.
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
            dt.Columns.Add("customer_email", typeof(string));
            dt.Columns.Add("condicion_pago", typeof(string));
            dt.Columns.Add("impuesto", typeof(short));
            dt.Columns.Add("customer_dir", typeof(string));
            dt.Columns.Add("unity1", typeof(bool));
            dt.Columns.Add("unity2", typeof(bool));
            dt.Columns.Add("status", typeof(string));

            // Solo columnas de la tabla customer (sin joins ni valores externos, así que
            // no hay nada que parametrizar); status traduce el bit anulado a texto.
            const string sql = @"
                SELECT customer_id,
                       customer_name,
                       identificacion,
                       empresa,
                       customer_category,
                       phone,
                       contacto,
                       customer_email,
                       condicion_pago,
                       impuesto,
                       customer_dir,
                       unity1,
                       unity2,
                       CASE WHEN anulado = 0 THEN 'activo' ELSE 'desactivado' END AS status
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
                    reader.IsDBNull(9) ? DBNull.Value : reader.GetValue(9),
                    Texto(reader, 10),
                    reader.IsDBNull(11) ? DBNull.Value : reader.GetValue(11),
                    reader.IsDBNull(12) ? DBNull.Value : reader.GetValue(12),
                    reader.IsDBNull(13) ? string.Empty : reader.GetString(13));
            }

            return dt;
        }

        /// <summary>Lee una columna como texto (NULL → cadena vacía).</summary>
        private static object Texto(SqlDataReader reader, int indice)
            => reader.IsDBNull(indice)
                ? string.Empty
                : Convert.ToString(reader.GetValue(indice)) ?? string.Empty;
    }
}
