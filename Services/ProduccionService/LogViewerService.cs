using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Services.ProduccionService;

public interface ILogViewerService
{
    Task<DataTable> ObtenerLogsAsync(DateTime fechaInicio, DateTime fechaFin, string? tipoOperacion = null);
    Task<DataTable> ObtenerDetallesLogAsync(long operacionId);
    Task<DataTable> ObtenerLogsPorOCAsync(string numeroOC);
    Task<DataTable> ObtenerResumenDiarioAsync();
    Task<string> GenerarReporteTextoAsync(long operacionId);
}

public class LogViewerService : ILogViewerService
{
    private readonly string _conn;

    public LogViewerService(IConfiguration config)
    {
        _conn = ConexionResolver.Resolver(config);
    }

    public async Task<DataTable> ObtenerLogsAsync(DateTime fechaInicio, DateTime fechaFin, string? tipoOperacion = null)
    {
        string sql = @"
            SELECT 
                id,
                fecha_inicio AS [Fecha Inicio],
                fecha_fin AS [Fecha Fin],
                DATEDIFF(SECOND, fecha_inicio, ISNULL(fecha_fin, GETDATE())) AS [Duración Seg],
                tipo_operacion AS [Tipo],
                descripcion AS [Descripción],
                usuario AS [Usuario],
                maquina AS [Máquina],
                CASE WHEN exitoso = 1 THEN '✓' ELSE '✗' END AS [Estado],
                resultado AS [Resultado],
                detalle_error AS [Error]
            FROM operaciones_log
            WHERE fecha_inicio >= @fecha_inicio 
              AND fecha_inicio <= @fecha_fin";

        if (!string.IsNullOrWhiteSpace(tipoOperacion))
            sql += " AND tipo_operacion = @tipo_operacion";

        sql += " ORDER BY fecha_inicio DESC";

        return await CargarTablaAsync(sql, fechaInicio, fechaFin, tipoOperacion);
    }

    public async Task<DataTable> ObtenerDetallesLogAsync(long operacionId)
    {
        const string sql = @"
            SELECT 
                fecha AS [Fecha],
                accion AS [Acción],
                entidad AS [Entidad],
                valor_anterior AS [Valor Anterior],
                valor_nuevo AS [Valor Nuevo],
                notas AS [Notas]
            FROM operaciones_log_detalle
            WHERE operacion_id = @operacion_id
            ORDER BY fecha";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@operacion_id", SqlDbType.BigInt) { Value = operacionId });

        DataTable dt = new("Detalles");
        using SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);
        return dt;
    }

    public async Task<DataTable> ObtenerLogsPorOCAsync(string numeroOC)
    {
        const string sql = @"
            SELECT 
                id,
                fecha_inicio AS [Fecha],
                tipo_operacion AS [Operación],
                CASE WHEN exitoso = 1 THEN '✓' ELSE '✗' END AS [Estado],
                usuario AS [Usuario],
                descripcion AS [Descripción],
                resultado AS [Resultado]
            FROM operaciones_log
            WHERE descripcion LIKE @oc_pattern
            ORDER BY fecha_inicio DESC";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@oc_pattern", SqlDbType.NVarChar) { Value = $"%OC {numeroOC}%" });

        DataTable dt = new("LogsOC");
        using SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);
        return dt;
    }

    public async Task<DataTable> ObtenerResumenDiarioAsync()
    {
        const string sql = @"
            SELECT 
                tipo_operacion AS [Tipo Operación],
                COUNT(*) AS [Total],
                SUM(CASE WHEN exitoso = 1 THEN 1 ELSE 0 END) AS [Exitosas],
                SUM(CASE WHEN exitoso = 0 THEN 1 ELSE 0 END) AS [Fallidas],
                AVG(DATEDIFF(SECOND, fecha_inicio, ISNULL(fecha_fin, GETDATE()))) AS [Promedio Seg]
            FROM operaciones_log
            WHERE fecha_inicio > CAST(GETDATE() AS DATE)
            GROUP BY tipo_operacion
            ORDER BY [Total] DESC";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);

        DataTable dt = new("Resumen");
        using SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);
        return dt;
    }

    public async Task<string> GenerarReporteTextoAsync(long operacionId)
    {
        const string sqlCabecera = @"
            SELECT id, fecha_inicio, fecha_fin, tipo_operacion, descripcion, 
                   usuario, maquina, ip_address, exitoso, resultado, detalle_error
            FROM operaciones_log WHERE id = @id";

        const string sqlDetalle = @"
            SELECT fecha, accion, entidad, valor_anterior, valor_nuevo, notas
            FROM operaciones_log_detalle WHERE operacion_id = @id ORDER BY fecha";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();

        var sb = new System.Text.StringBuilder();

        // Cabecera
        using (SqlCommand cmd = new(sqlCabecera, conn))
        {
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt) { Value = operacionId });
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                sb.AppendLine("═══════════════════════════════════════════════════════════════");
                sb.AppendLine($"REPORTE DE OPERACIÓN #{reader.GetInt64(reader.GetOrdinal("id"))}");
                sb.AppendLine("═══════════════════════════════════════════════════════════════");
                sb.AppendLine($"Fecha Inicio:    {reader.GetDateTime(reader.GetOrdinal("fecha_inicio")):dd/MM/yyyy HH:mm:ss.fff}");
                sb.AppendLine($"Fecha Fin:       {(reader.IsDBNull(reader.GetOrdinal("fecha_fin")) ? "En proceso..." : reader.GetDateTime(reader.GetOrdinal("fecha_fin")).ToString("dd/MM/yyyy HH:mm:ss.fff"))}");
                sb.AppendLine($"Tipo:            {reader.GetString(reader.GetOrdinal("tipo_operacion"))}");
                sb.AppendLine($"Descripción:     {reader.GetString(reader.GetOrdinal("descripcion"))}");
                sb.AppendLine($"Usuario:         {reader.GetString(reader.GetOrdinal("usuario"))}");
                sb.AppendLine($"Máquina:         {reader.GetString(reader.GetOrdinal("maquina"))}");
                sb.AppendLine($"IP:              {reader.GetString(reader.GetOrdinal("ip_address"))}");
                sb.AppendLine($"Estado:          {(reader.GetBoolean(reader.GetOrdinal("exitoso")) ? "✓ EXITOSO" : "✗ FALLIDO")}");
                sb.AppendLine($"Resultado:       {reader.GetString(reader.GetOrdinal("resultado"))}");

                string error = reader.GetString(reader.GetOrdinal("detalle_error"));
                if (!string.IsNullOrWhiteSpace(error))
                    sb.AppendLine($"Error:           {error}");
            }
        }

        // Detalles
        sb.AppendLine("");
        sb.AppendLine("DETALLE DE ACCIONES:");
        sb.AppendLine("───────────────────────────────────────────────────────────────");

        using (SqlCommand cmd = new(sqlDetalle, conn))
        {
            cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt) { Value = operacionId });
            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                sb.AppendLine($"  [{reader.GetDateTime(reader.GetOrdinal("fecha")):HH:mm:ss.fff}] {reader.GetString(reader.GetOrdinal("accion"))}");
                sb.AppendLine($"    Entidad: {reader.GetString(reader.GetOrdinal("entidad"))}");

                string anterior = reader.IsDBNull(reader.GetOrdinal("valor_anterior")) ? "" : reader.GetString(reader.GetOrdinal("valor_anterior"));
                if (!string.IsNullOrWhiteSpace(anterior))
                    sb.AppendLine($"    Anterior: {anterior}");

                string nuevo = reader.IsDBNull(reader.GetOrdinal("valor_nuevo")) ? "" : reader.GetString(reader.GetOrdinal("valor_nuevo"));
                if (!string.IsNullOrWhiteSpace(nuevo))
                    sb.AppendLine($"    Nuevo: {nuevo}");

                string notas = reader.GetString(reader.GetOrdinal("notas"));
                if (!string.IsNullOrWhiteSpace(notas))
                    sb.AppendLine($"    Notas: {notas}");

                sb.AppendLine("");
            }
        }

        sb.AppendLine("═══════════════════════════════════════════════════════════════");

        return sb.ToString();
    }

    private async Task<DataTable> CargarTablaAsync(string sql, DateTime fechaInicio, DateTime fechaFin, string? tipoOperacion)
    {
        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@fecha_inicio", SqlDbType.DateTime2) { Value = fechaInicio });
        cmd.Parameters.Add(new SqlParameter("@fecha_fin", SqlDbType.DateTime2) { Value = fechaFin });

        if (!string.IsNullOrWhiteSpace(tipoOperacion))
            cmd.Parameters.Add(new SqlParameter("@tipo_operacion", SqlDbType.NVarChar, 50) { Value = tipoOperacion });

        DataTable dt = new("Logs");
        using SqlDataAdapter adapter = new(cmd);
        adapter.Fill(dt);
        return dt;
    }
}
