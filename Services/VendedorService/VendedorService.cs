using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.VendedorService
{
    /// <summary>
    /// Servicio de vendedores: por ahora solo el listado del módulo de Vendedores.
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
            dt.Columns.Add("status", typeof(string));

            // Solo columnas de la tabla vendedor (sin joins ni valores externos, así que
            // no hay nada que parametrizar); status traduce el bit anulado a texto.
            const string sql = @"
                SELECT vendor_id,
                       vendor_name,
                       correo,
                       phone,
                       CASE WHEN anulado = 0 THEN 'activo' ELSE 'desactivado' END AS status
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
                    Texto(reader, 4));
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
