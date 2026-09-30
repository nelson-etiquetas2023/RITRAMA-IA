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
        /// Trae todos los clientes (anulado = 0 y 1) con el estado traducido a texto,
        /// para que el grid pueda mostrar activo/desactivado en la columna status.
        /// </summary>
        public async Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            DataTable dt = new DataTable();
            dt.Columns.Add("customer_id", typeof(string));
            dt.Columns.Add("customer_name", typeof(string));
            dt.Columns.Add("status", typeof(string));

            // Consulta sin valores externos (no hay nada que parametrizar) que proyecta
            // solo las tres columnas que pinta el grid.
            const string sql = @"
                SELECT customer_id,
                       customer_name,
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
                    reader.IsDBNull(0) ? string.Empty : reader.GetValue(0).ToString(),
                    reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    reader.IsDBNull(2) ? string.Empty : reader.GetString(2));
            }

            return dt;
        }
    }
}
