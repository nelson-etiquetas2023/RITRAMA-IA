using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ProveedorService
{
    /// <summary>
    /// Servicio de proveedores: por ahora solo el listado del módulo de Proveedores.
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
            dt.Columns.Add("direccion", typeof(string));
            dt.Columns.Add("email", typeof(string));
            dt.Columns.Add("status", typeof(string));
            dt.Columns.Add("unidad_master_1", typeof(bool));
            dt.Columns.Add("unidad_master_2", typeof(bool));

            // Solo columnas de la tabla provider (sin joins ni valores externos, así que
            // no hay nada que parametrizar); status traduce el bit anulado a texto.
            const string sql = @"
                SELECT Proveedor_Id,
                       Proveedor_Name,
                       phone,
                       direccion,
                       email,
                       CASE WHEN anulado = 0 THEN 'activo' ELSE 'desactivado' END AS status,
                       unidad_master_1,
                       unidad_master_2
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
                    reader.IsDBNull(6) ? DBNull.Value : reader.GetValue(6),
                    reader.IsDBNull(7) ? DBNull.Value : reader.GetValue(7));
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
