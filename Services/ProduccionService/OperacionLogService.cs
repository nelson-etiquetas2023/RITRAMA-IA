using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Services.ProduccionService;

public class OperacionLogService : IOperacionLogService
{
    private readonly string _conn;

    public OperacionLogService(IConfiguration config)
    {
        _conn = ConexionResolver.Resolver(config);
    }

    public async Task<long> RegistrarInicioAsync(string tipoOperacion, string descripcion, string usuario = "", string maquina = "", string ipAddress = "")
    {
        const string sql = @"
            INSERT INTO operaciones_log (fecha_inicio, tipo_operacion, descripcion, usuario, maquina, ip_address, exitoso)
            OUTPUT INSERTED.id
            VALUES (@fecha_inicio, @tipo_operacion, @descripcion, @usuario, @maquina, @ip_address, 0)";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@fecha_inicio", SqlDbType.DateTime2) { Value = DateTime.Now });
        cmd.Parameters.Add(new SqlParameter("@tipo_operacion", SqlDbType.NVarChar, 50) { Value = tipoOperacion });
        cmd.Parameters.Add(new SqlParameter("@descripcion", SqlDbType.NVarChar, 500) { Value = descripcion });
        cmd.Parameters.Add(new SqlParameter("@usuario", SqlDbType.NVarChar, 100) { Value = usuario });
        cmd.Parameters.Add(new SqlParameter("@maquina", SqlDbType.NVarChar, 100) { Value = maquina });
        cmd.Parameters.Add(new SqlParameter("@ip_address", SqlDbType.NVarChar, 50) { Value = ipAddress });

        object? result = await cmd.ExecuteScalarAsync();
        return Convert.ToInt64(result);
    }

    public async Task RegistrarFinAsync(long operacionId, bool exitoso, string resultado = "", string detalleError = "")
    {
        const string sql = @"
            UPDATE operaciones_log 
            SET fecha_fin = @fecha_fin, 
                exitoso = @exitoso, 
                resultado = @resultado,
                detalle_error = @detalle_error
            WHERE id = @id";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@id", SqlDbType.BigInt) { Value = operacionId });
        cmd.Parameters.Add(new SqlParameter("@fecha_fin", SqlDbType.DateTime2) { Value = DateTime.Now });
        cmd.Parameters.Add(new SqlParameter("@exitoso", SqlDbType.Bit) { Value = exitoso });
        cmd.Parameters.Add(new SqlParameter("@resultado", SqlDbType.NVarChar, 1000) { Value = resultado });
        cmd.Parameters.Add(new SqlParameter("@detalle_error", SqlDbType.NVarChar, 2000) { Value = detalleError });

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task RegistrarDetalleAsync(long operacionId, string accion, string entidad, string? valorAnterior = null, string? valorNuevo = null, string notas = "")
    {
        const string sql = @"
            INSERT INTO operaciones_log_detalle (operacion_id, fecha, accion, entidad, valor_anterior, valor_nuevo, notas)
            VALUES (@operacion_id, @fecha, @accion, @entidad, @valor_anterior, @valor_nuevo, @notas)";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@operacion_id", SqlDbType.BigInt) { Value = operacionId });
        cmd.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime2) { Value = DateTime.Now });
        cmd.Parameters.Add(new SqlParameter("@accion", SqlDbType.NVarChar, 100) { Value = accion });
        cmd.Parameters.Add(new SqlParameter("@entidad", SqlDbType.NVarChar, 100) { Value = entidad });
        cmd.Parameters.Add(new SqlParameter("@valor_anterior", SqlDbType.NVarChar, 500) { Value = (object?)valorAnterior ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@valor_nuevo", SqlDbType.NVarChar, 500) { Value = (object?)valorNuevo ?? DBNull.Value });
        cmd.Parameters.Add(new SqlParameter("@notas", SqlDbType.NVarChar, 1000) { Value = notas });

        await cmd.ExecuteNonQueryAsync();
    }

    public async Task<List<OperacionLog>> ConsultarLogsAsync(DateTime fechaInicio, DateTime fechaFin, string? tipoOperacion = null, int maxRegistros = 500)
    {
        List<OperacionLog> resultado = [];

        string sql = @"
            SELECT TOP (@maxregistros) id, fecha_inicio, fecha_fin, tipo_operacion, descripcion, 
                   usuario, maquina, ip_address, exitoso, resultado, detalle_error
            FROM operaciones_log
            WHERE fecha_inicio >= @fecha_inicio 
              AND fecha_inicio <= @fecha_fin";

        if (!string.IsNullOrWhiteSpace(tipoOperacion))
            sql += " AND tipo_operacion = @tipo_operacion";

        sql += " ORDER BY fecha_inicio DESC";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@fecha_inicio", SqlDbType.DateTime2) { Value = fechaInicio });
        cmd.Parameters.Add(new SqlParameter("@fecha_fin", SqlDbType.DateTime2) { Value = fechaFin });
        cmd.Parameters.Add(new SqlParameter("@maxregistros", SqlDbType.Int) { Value = maxRegistros });

        if (!string.IsNullOrWhiteSpace(tipoOperacion))
            cmd.Parameters.Add(new SqlParameter("@tipo_operacion", SqlDbType.NVarChar, 50) { Value = tipoOperacion });

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var log = new OperacionLog
            {
                Id = reader.GetInt64(reader.GetOrdinal("id")),
                FechaInicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                FechaFin = reader.IsDBNull(reader.GetOrdinal("fecha_fin")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha_fin")),
                TipoOperacion = reader.GetString(reader.GetOrdinal("tipo_operacion")),
                Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                Usuario = reader.GetString(reader.GetOrdinal("usuario")),
                Maquina = reader.GetString(reader.GetOrdinal("maquina")),
                IpAddress = reader.GetString(reader.GetOrdinal("ip_address")),
                Exitoso = reader.GetBoolean(reader.GetOrdinal("exitoso")),
                Resultado = reader.GetString(reader.GetOrdinal("resultado")),
                DetalleError = reader.GetString(reader.GetOrdinal("detalle_error"))
            };

            // Cargar detalles
            log.Detalles = await CargarDetallesAsync(log.Id);

            resultado.Add(log);
        }

        return resultado;
    }

    public async Task<List<OperacionLog>> ConsultarLogsPorOCAsync(string numeroOC)
    {
        List<OperacionLog> resultado = [];

        const string sql = @"
            SELECT id, fecha_inicio, fecha_fin, tipo_operacion, descripcion, 
                   usuario, maquina, ip_address, exitoso, resultado, detalle_error
            FROM operaciones_log
            WHERE descripcion LIKE @oc_pattern
            ORDER BY fecha_inicio DESC";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@oc_pattern", SqlDbType.NVarChar) { Value = $"%OC {numeroOC}%" });

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            var log = new OperacionLog
            {
                Id = reader.GetInt64(reader.GetOrdinal("id")),
                FechaInicio = reader.GetDateTime(reader.GetOrdinal("fecha_inicio")),
                FechaFin = reader.IsDBNull(reader.GetOrdinal("fecha_fin")) ? null : reader.GetDateTime(reader.GetOrdinal("fecha_fin")),
                TipoOperacion = reader.GetString(reader.GetOrdinal("tipo_operacion")),
                Descripcion = reader.GetString(reader.GetOrdinal("descripcion")),
                Usuario = reader.GetString(reader.GetOrdinal("usuario")),
                Maquina = reader.GetString(reader.GetOrdinal("maquina")),
                IpAddress = reader.GetString(reader.GetOrdinal("ip_address")),
                Exitoso = reader.GetBoolean(reader.GetOrdinal("exitoso")),
                Resultado = reader.GetString(reader.GetOrdinal("resultado")),
                DetalleError = reader.GetString(reader.GetOrdinal("detalle_error"))
            };

            log.Detalles = await CargarDetallesAsync(log.Id);
            resultado.Add(log);
        }

        return resultado;
    }

    public async Task<string> GenerarReporteAsync(long operacionId)
    {
        var logs = await ConsultarLogsAsync(DateTime.Now.AddDays(-30), DateTime.Now);
        var log = logs.FirstOrDefault(l => l.Id == operacionId);

        if (log == null)
            return "Operación no encontrada.";

        var sb = new System.Text.StringBuilder();
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine($"REPORTE DE OPERACIÓN #{log.Id}");
        sb.AppendLine("═══════════════════════════════════════════════════════════════");
        sb.AppendLine($"Fecha Inicio:    {log.FechaInicio:dd/MM/yyyy HH:mm:ss.fff}");
        sb.AppendLine($"Fecha Fin:       {(log.FechaFin?.ToString("dd/MM/yyyy HH:mm:ss.fff") ?? "En proceso...")}");
        sb.AppendLine($"Tipo:            {log.TipoOperacion}");
        sb.AppendLine($"Descripción:     {log.Descripcion}");
        sb.AppendLine($"Usuario:         {(string.IsNullOrWhiteSpace(log.Usuario) ? "Sistema" : log.Usuario)}");
        sb.AppendLine($"Máquina:         {log.Maquina}");
        sb.AppendLine($"IP:              {log.IpAddress}");
        sb.AppendLine($"Estado:          {(log.Exitoso ? "✓ EXITOSO" : "✗ FALLIDO")}");
        sb.AppendLine($"Resultado:       {log.Resultado}");

        if (!string.IsNullOrWhiteSpace(log.DetalleError))
        {
            sb.AppendLine($"Error:           {log.DetalleError}");
        }

        if (log.Detalles.Count > 0)
        {
            sb.AppendLine("");
            sb.AppendLine("DETALLE DE ACCIONES:");
            sb.AppendLine("───────────────────────────────────────────────────────────────");

            foreach (var detalle in log.Detalles)
            {
                sb.AppendLine($"  [{detalle.Fecha:HH:mm:ss.fff}] {detalle.Accion}");
                sb.AppendLine($"    Entidad: {detalle.Entidad}");

                if (!string.IsNullOrWhiteSpace(detalle.ValorAnterior))
                    sb.AppendLine($"    Anterior: {detalle.ValorAnterior}");

                if (!string.IsNullOrWhiteSpace(detalle.ValorNuevo))
                    sb.AppendLine($"    Nuevo: {detalle.ValorNuevo}");

                if (!string.IsNullOrWhiteSpace(detalle.Notas))
                    sb.AppendLine($"    Notas: {detalle.Notas}");

                sb.AppendLine("");
            }
        }

        sb.AppendLine("═══════════════════════════════════════════════════════════════");

        return sb.ToString();
    }

    private async Task<List<OperacionLogDetalle>> CargarDetallesAsync(long operacionId)
    {
        List<OperacionLogDetalle> detalles = [];

        const string sql = @"
            SELECT id, operacion_id, fecha, accion, entidad, valor_anterior, valor_nuevo, notas
            FROM operaciones_log_detalle
            WHERE operacion_id = @operacion_id
            ORDER BY fecha";

        using SqlConnection conn = new(_conn);
        await conn.OpenAsync();
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@operacion_id", SqlDbType.BigInt) { Value = operacionId });

        using SqlDataReader reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            detalles.Add(new OperacionLogDetalle
            {
                Id = reader.GetInt64(reader.GetOrdinal("id")),
                OperacionId = reader.GetInt64(reader.GetOrdinal("operacion_id")),
                Fecha = reader.GetDateTime(reader.GetOrdinal("fecha")),
                Accion = reader.GetString(reader.GetOrdinal("accion")),
                Entidad = reader.GetString(reader.GetOrdinal("entidad")),
                ValorAnterior = reader.IsDBNull(reader.GetOrdinal("valor_anterior")) ? null : reader.GetString(reader.GetOrdinal("valor_anterior")),
                ValorNuevo = reader.IsDBNull(reader.GetOrdinal("valor_nuevo")) ? null : reader.GetString(reader.GetOrdinal("valor_nuevo")),
                Notas = reader.GetString(reader.GetOrdinal("notas"))
            });
        }

        return detalles;
    }
}
