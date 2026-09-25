using System.Data;
using System.Net;
using System.Net.Sockets;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Services.ProduccionService;

public class ReconciliacionService : IReconciliacionService
{
    private readonly string _conn;
    private readonly IOperacionLogService _logService;
    public string ErrorMsg { get; set; } = null!;

    public ReconciliacionService(IConfiguration config, IOperacionLogService logService)
    {
        _conn = ConexionResolver.Resolver(config);
        _logService = logService;
    }

    public async Task<List<InconsistenciaOC>> DetectarInconsistenciasAsync()
    {
        List<InconsistenciaOC> inconsistencias = [];
        long? operacionId = null;

        try
        {
            // Registrar inicio de detección
            string maquina = Environment.MachineName;
            string ip = ObtenerDireccionIP();
            operacionId = await _logService.RegistrarInicioAsync(
                "RECONCILIACION",
                "Detección de inconsistencias en inventario de masters",
                Environment.UserName,
                maquina,
                ip);

            using SqlConnection conn = new(_conn);
            await conn.OpenAsync();

            // Detectar OCs etiquetadas (step>=3) sin consumo registrado en MasterDetailsInic
            List<InconsistenciaOC> sinConsumo = await DetectarOCSinConsumoAsync(conn);
            inconsistencias.AddRange(sinConsumo);

            // Detectar OCs cerradas (step=5) con rollos no disponibles
            List<InconsistenciaOC> sinDisponibles = await DetectarOCSinDisponiblesAsync(conn);
            inconsistencias.AddRange(sinDisponibles);

            // Detectar OCs con consumo en MasterDetailsInic pero step<3
            List<InconsistenciaOC> consumoSinEtiquetar = await DetectarConsumoSinEtiquetarAsync(conn);
            inconsistencias.AddRange(consumoSinEtiquetar);

            // Registrar resultado
            string resultado = $"Detectadas {inconsistencias.Count} inconsistencia(s)";
            await _logService.RegistrarFinAsync(operacionId.Value, true, resultado);

            // Registrar detalle de cada inconsistencia
            foreach (InconsistenciaOC inc in inconsistencias)
            {
                await _logService.RegistrarDetalleAsync(
                    operacionId.Value,
                    "INCONSISTENCIA_DETECTADA",
                    $"OC {inc.NumeroOC}",
                    null,
                    inc.TipoInconsistencia,
                    inc.Descripcion);
            }
        }
        catch (Exception ex)
        {
            ErrorMsg = $"Error al detectar inconsistencias: {ex.Message}";
            ServiceErrors.Report(ErrorMsg);

            if (operacionId.HasValue)
            {
                await _logService.RegistrarFinAsync(operacionId.Value, false, "", ex.Message);
            }
        }

        return inconsistencias;
    }

    private async Task<List<InconsistenciaOC>> DetectarOCSinConsumoAsync(SqlConnection conn)
    {
        List<InconsistenciaOC> resultado = [];
        List<(int numero, int step, string rollid2, double consumoTotal2)> pendientesMaster2 = [];

        const string sql = @"
            SELECT oc.numero, oc.step, oc.rollid_1, oc.rollid_2,
                   oc.util1_real_lenght, oc.desperdicio, oc.lenght_1,
                   oc.util2_real_lenght, oc.desperdicio2, oc.lenght_2,
                   oc.TwoMasters
            FROM orden_corte oc
            WHERE oc.anulada = 0 
              AND oc.CloseDocument = 0
              AND oc.step >= 3
              AND NOT EXISTS (
                  SELECT 1 FROM MasterDetailsInic md 
                  WHERE md.rollid = oc.rollid_1 AND md.orden = oc.numero
              )
              AND oc.util1_real_lenght > 0";

        using (SqlCommand cmd = new(sql, conn))
        using (SqlDataReader reader = await cmd.ExecuteReaderAsync())
        {
            while (await reader.ReadAsync())
            {
                int numero = reader.GetInt32(reader.GetOrdinal("numero"));
                int step = reader.GetInt32(reader.GetOrdinal("step"));
                string rollid1 = reader.GetString(reader.GetOrdinal("rollid_1"));
                double consumo1 = (double)reader.GetDecimal(reader.GetOrdinal("util1_real_lenght"));
                bool desperdicio1 = reader.GetBoolean(reader.GetOrdinal("desperdicio"));
                double lenght1 = (double)reader.GetDecimal(reader.GetOrdinal("lenght_1"));

                double consumoTotal1 = consumo1 + (desperdicio1 ? (lenght1 - consumo1) : 0);

                resultado.Add(new InconsistenciaOC
                {
                    NumeroOC = numero,
                    StepActual = step,
                    TipoInconsistencia = "SIN_CONSUMO_MASTER1",
                    Descripcion = $"OC {numero} (step={step}) tiene consumo registrado en UI ({consumoTotal1:N2} pies) pero NO existe detalle en MasterDetailsInic para master {rollid1}.",
                    RequiereAccionManual = false
                });

                bool twoMasters = reader.GetBoolean(reader.GetOrdinal("TwoMasters"));
                if (twoMasters)
                {
                    string rollid2 = reader.GetString(reader.GetOrdinal("rollid_2"));
                    double consumo2 = (double)reader.GetDecimal(reader.GetOrdinal("util2_real_lenght"));
                    bool desperdicio2 = reader.GetBoolean(reader.GetOrdinal("desperdicio2"));
                    double lenght2 = (double)reader.GetDecimal(reader.GetOrdinal("lenght_2"));

                    if (!string.IsNullOrWhiteSpace(rollid2) && rollid2 != "0" && consumo2 > 0)
                    {
                        double consumoTotal2 = consumo2 + (desperdicio2 ? (lenght2 - consumo2) : 0);
                        pendientesMaster2.Add((numero, step, rollid2, consumoTotal2));
                    }
                }
            }
        }

        foreach ((int numero, int step, string rollid2, double consumoTotal2) pendiente in pendientesMaster2)
        {
            bool existeM2 = await ExisteConsumoMasterAsync(conn, pendiente.rollid2, pendiente.numero.ToString());
            if (!existeM2)
            {
                resultado.Add(new InconsistenciaOC
                {
                    NumeroOC = pendiente.numero,
                    StepActual = pendiente.step,
                    TipoInconsistencia = "SIN_CONSUMO_MASTER2",
                    Descripcion = $"OC {pendiente.numero} (step={pendiente.step}) tiene consumo registrado en UI para master 2 ({pendiente.consumoTotal2:N2} pies) pero NO existe detalle en MasterDetailsInic para master {pendiente.rollid2}.",
                    RequiereAccionManual = false
                });
            }
        }

        return resultado;
    }

    private async Task<List<InconsistenciaOC>> DetectarOCSinDisponiblesAsync(SqlConnection conn)
    {
        List<InconsistenciaOC> resultado = [];

        const string sql = @"
            SELECT oc.numero, oc.step,
                   COUNT(rd.roll_number) AS total_rollos,
                   SUM(CASE WHEN rd.disponible = 1 THEN 1 ELSE 0 END) AS disponibles
            FROM orden_corte oc
            INNER JOIN rolls_details rd ON rd.numero = oc.numero
            WHERE oc.anulada = 0 
              AND oc.CloseDocument = 1
              AND oc.step = 5
            GROUP BY oc.numero, oc.step
            HAVING SUM(CASE WHEN rd.disponible = 1 THEN 1 ELSE 0 END) < COUNT(rd.roll_number)";

        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int numero = reader.GetInt32(reader.GetOrdinal("numero"));
            int step = reader.GetInt32(reader.GetOrdinal("step"));
            int total = reader.GetInt32(reader.GetOrdinal("total_rollos"));
            int disponibles = reader.GetInt32(reader.GetOrdinal("disponibles"));
            int faltantes = total - disponibles;

            resultado.Add(new InconsistenciaOC
            {
                NumeroOC = numero,
                StepActual = step,
                TipoInconsistencia = "ROLLOS_NO_DISPONIBLES",
                Descripcion = $"OC {numero} (step={step}) esta cerrada pero {faltantes} de {total} rollos no estan marcados como disponibles.",
                RequiereAccionManual = false
            });
        }

        return resultado;
    }

    private async Task<List<InconsistenciaOC>> DetectarConsumoSinEtiquetarAsync(SqlConnection conn)
    {
        List<InconsistenciaOC> resultado = [];

        const string sql = @"
            SELECT md.orden, md.rollid, SUM(md.consumo) AS total_consumo
            FROM MasterDetailsInic md
            INNER JOIN orden_corte oc ON oc.numero = md.orden
            WHERE oc.anulada = 0
              AND oc.step < 3
            GROUP BY md.orden, md.rollid";

        using SqlCommand cmd = new(sql, conn);
        using SqlDataReader reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            int orden = reader.GetInt32(reader.GetOrdinal("orden"));
            string rollid = reader.GetString(reader.GetOrdinal("rollid"));
            double totalConsumo = (double)reader.GetDecimal(reader.GetOrdinal("total_consumo"));

            resultado.Add(new InconsistenciaOC
            {
                NumeroOC = orden,
                StepActual = 2, // step<3
                TipoInconsistencia = "CONSUMO_SIN_ETIQUETAR",
                Descripcion = $"OC {orden} tiene consumo registrado en MasterDetailsInic ({totalConsumo:N2} pies) para master {rollid} pero su step es < 3 (no etiquetada).",
                RequiereAccionManual = true
            });
        }

        return resultado;
    }

    private async Task<bool> ExisteConsumoMasterAsync(SqlConnection conn, string rollid, string orden)
    {
        const string sql = "SELECT 1 FROM MasterDetailsInic WHERE rollid=@rollid AND orden=@orden";
        using SqlCommand cmd = new(sql, conn);
        cmd.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
        cmd.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = orden });
        object? result = await cmd.ExecuteScalarAsync();
        return result != null;
    }

    public async Task<int> CorregirInconsistenciasAsync(List<InconsistenciaOC> inconsistencias)
    {
        int corregidas = 0;
        long? operacionId = null;

        try
        {
            string maquina = Environment.MachineName;
            string ip = ObtenerDireccionIP();
            operacionId = await _logService.RegistrarInicioAsync(
                "RECONCILIACION",
                $"Corrección de {inconsistencias.Count} inconsistencia(s) detectada(s)",
                Environment.UserName,
                maquina,
                ip);

            foreach (InconsistenciaOC inconsistencia in inconsistencias)
            {
                if (inconsistencia.RequiereAccionManual)
                {
                    await _logService.RegistrarDetalleAsync(
                        operacionId.Value,
                        "REQUIERE_ACCION_MANUAL",
                        $"OC {inconsistencia.NumeroOC}",
                        null,
                        inconsistencia.TipoInconsistencia,
                        inconsistencia.Descripcion);
                    continue;
                }

                try
                {
                    using SqlConnection conn = new(_conn);
                    await conn.OpenAsync();
                    using SqlTransaction tran = conn.BeginTransaction();

                    // Registrar estado anterior
                    await _logService.RegistrarDetalleAsync(
                        operacionId.Value,
                        "ANTES_CORRECCION",
                        $"OC {inconsistencia.NumeroOC}",
                        null,
                        inconsistencia.TipoInconsistencia,
                        inconsistencia.Descripcion);

                    switch (inconsistencia.TipoInconsistencia)
                    {
                        case "SIN_CONSUMO_MASTER1":
                        case "SIN_CONSUMO_MASTER2":
                            await CorregirSinConsumoAsync(conn, tran, inconsistencia);
                            corregidas++;
                            break;

                        case "ROLLOS_NO_DISPONIBLES":
                            await CorregirRollosNoDisponiblesAsync(conn, tran, inconsistencia);
                            corregidas++;
                            break;
                    }

                    tran.Commit();

                    // Registrar estado después de corrección
                    await _logService.RegistrarDetalleAsync(
                        operacionId.Value,
                        "DESPUES_CORRECCION",
                        $"OC {inconsistencia.NumeroOC}",
                        inconsistencia.TipoInconsistencia,
                        "CORREGIDO",
                        $"Corrección exitosa de {inconsistencia.TipoInconsistencia}");
                }
                catch (Exception ex)
                {
                    ErrorMsg = $"Error al corregir inconsistencia {inconsistencia.TipoInconsistencia} en OC {inconsistencia.NumeroOC}: {ex.Message}";
                    ServiceErrors.Report(ErrorMsg);

                    await _logService.RegistrarDetalleAsync(
                        operacionId.Value,
                        "ERROR_CORRECCION",
                        $"OC {inconsistencia.NumeroOC}",
                        inconsistencia.TipoInconsistencia,
                        "ERROR",
                        ex.Message);
                }
            }

            string resultadoFinal = $"Corregidas {corregidas} de {inconsistencias.Count} inconsistencia(s)";
            await _logService.RegistrarFinAsync(operacionId.Value, true, resultadoFinal);
        }
        catch (Exception ex)
        {
            ErrorMsg = $"Error general en corrección de inconsistencias: {ex.Message}";
            ServiceErrors.Report(ErrorMsg);

            if (operacionId.HasValue)
            {
                await _logService.RegistrarFinAsync(operacionId.Value, false, "", ex.Message);
            }
        }

        return corregidas;
    }

    private async Task CorregirSinConsumoAsync(SqlConnection conn, SqlTransaction tran, InconsistenciaOC inconsistencia)
    {
        // Obtener datos de la OC
        const string sqlOC = @"
            SELECT rollid_1, util1_real_lenght, desperdicio, lenght_1,
                   rollid_2, util2_real_lenght, desperdicio2, lenght_2, TwoMasters
            FROM orden_corte WHERE numero=@numero";

        string rollid1 = "", rollid2 = "";
        double consumo1 = 0, consumo2 = 0;
        bool desperdicio1 = false, desperdicio2 = false;
        double lenght1 = 0, lenght2 = 0;
        bool twoMasters = false;

        using (SqlCommand cmd = new(sqlOC, conn, tran))
        {
            cmd.Parameters.Add(new SqlParameter("@numero", SqlDbType.Int) { Value = inconsistencia.NumeroOC });
            using SqlDataReader reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                rollid1 = reader.GetString(reader.GetOrdinal("rollid_1"));
                consumo1 = (double)reader.GetDecimal(reader.GetOrdinal("util1_real_lenght"));
                desperdicio1 = reader.GetBoolean(reader.GetOrdinal("desperdicio"));
                lenght1 = (double)reader.GetDecimal(reader.GetOrdinal("lenght_1"));
                twoMasters = reader.GetBoolean(reader.GetOrdinal("TwoMasters"));

                if (twoMasters)
                {
                    rollid2 = reader.GetString(reader.GetOrdinal("rollid_2"));
                    consumo2 = (double)reader.GetDecimal(reader.GetOrdinal("util2_real_lenght"));
                    desperdicio2 = reader.GetBoolean(reader.GetOrdinal("desperdicio2"));
                    lenght2 = (double)reader.GetDecimal(reader.GetOrdinal("lenght_2"));
                }
            }
        }

        // Registrar consumo faltante para master 1
        if (consumo1 > 0)
        {
            double desperdicioMonto1 = desperdicio1 ? (lenght1 - consumo1) : 0;
            await RegistrarConsumoFaltanteAsync(conn, tran, inconsistencia.NumeroOC.ToString(), rollid1, consumo1, desperdicioMonto1);
        }

        // Registrar consumo faltante para master 2
        if (twoMasters && !string.IsNullOrWhiteSpace(rollid2) && consumo2 > 0)
        {
            double desperdicioMonto2 = desperdicio2 ? (lenght2 - consumo2) : 0;
            await RegistrarConsumoFaltanteAsync(conn, tran, inconsistencia.NumeroOC.ToString(), rollid2, consumo2, desperdicioMonto2);
        }
    }

    private async Task RegistrarConsumoFaltanteAsync(SqlConnection conn, SqlTransaction tran, string orden, string rollid, double consumo, double desperdicio)
    {
        // Verificar si ya existe
        const string sqlExiste = "SELECT 1 FROM MasterDetailsInic WHERE rollid=@rollid AND orden=@orden AND desperdicio=0";
        using (SqlCommand cmdExiste = new(sqlExiste, conn, tran))
        {
            cmdExiste.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            cmdExiste.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = orden });
            if (await cmdExiste.ExecuteScalarAsync() != null)
            {
                return; // Ya existe, no duplicar
            }
        }

        // Determinar tipo de master
        string sqlInv;
        const string sqlTipoInic = "SELECT 1 FROM MasterInic WHERE Roll_Id=@rollid";
        using (SqlCommand cmdTipo = new(sqlTipoInic, conn, tran))
        {
            cmdTipo.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            bool esInic = await cmdTipo.ExecuteScalarAsync() != null;
            sqlInv = esInic
                ? R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_INICIALES
                : R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_MATERIA;
        }

        // Insertar detalle de consumo
        const string sqlInsert = "INSERT INTO MasterDetailsInic (rollid, orden, consumo, fecha_reg, desperdicio) VALUES(@rollid, @orden, @consumo, @fecha, @desperdicio)";
        using (SqlCommand cmdInsert = new(sqlInsert, conn, tran))
        {
            cmdInsert.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            cmdInsert.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = orden });
            cmdInsert.Parameters.Add(new SqlParameter("@consumo", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = consumo });
            cmdInsert.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime) { Value = DateTime.Now });
            cmdInsert.Parameters.Add(new SqlParameter("@desperdicio", SqlDbType.Bit) { Value = false });
            await cmdInsert.ExecuteNonQueryAsync();
        }

        // Actualizar inventario
        using (SqlCommand cmdInv = new(sqlInv, conn, tran))
        {
            cmdInv.Parameters.Add(new SqlParameter("@consumo", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = consumo });
            cmdInv.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            await cmdInv.ExecuteNonQueryAsync();
        }

        // Registrar desperdicio si aplica
        if (desperdicio > 0)
        {
            const string sqlExisteDesp = "SELECT 1 FROM MasterDetailsInic WHERE rollid=@rollid AND orden=@orden AND desperdicio=1";
            using (SqlCommand cmdExisteDesp = new(sqlExisteDesp, conn, tran))
            {
                cmdExisteDesp.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
                cmdExisteDesp.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = orden });
                if (await cmdExisteDesp.ExecuteScalarAsync() == null)
                {
                    const string sqlInsertDesp = "INSERT INTO MasterDetailsInic (rollid, orden, consumo, fecha_reg, desperdicio) VALUES(@rollid, @orden, @consumo, @fecha, @desperdicio)";
                    using (SqlCommand cmdInsertDesp = new(sqlInsertDesp, conn, tran))
                    {
                        cmdInsertDesp.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
                        cmdInsertDesp.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = orden });
                        cmdInsertDesp.Parameters.Add(new SqlParameter("@consumo", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = desperdicio });
                        cmdInsertDesp.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime) { Value = DateTime.Now });
                        cmdInsertDesp.Parameters.Add(new SqlParameter("@desperdicio", SqlDbType.Bit) { Value = true });
                        await cmdInsertDesp.ExecuteNonQueryAsync();
                    }

                    using (SqlCommand cmdInvDesp = new(sqlInv, conn, tran))
                    {
                        cmdInvDesp.Parameters.Add(new SqlParameter("@consumo", SqlDbType.Decimal) { Precision = 18, Scale = 2, Value = desperdicio });
                        cmdInvDesp.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
                        await cmdInvDesp.ExecuteNonQueryAsync();
                    }
                }
            }
        }
    }

    private async Task CorregirRollosNoDisponiblesAsync(SqlConnection conn, SqlTransaction tran, InconsistenciaOC inconsistencia)
    {
        const string sql = "UPDATE rolls_details SET disponible=1 WHERE numero=@numero AND (disponible IS NULL OR disponible=0)";
        using SqlCommand cmd = new(sql, conn, tran);
        cmd.Parameters.Add(new SqlParameter("@numero", SqlDbType.NVarChar) { Value = inconsistencia.NumeroOC.ToString() });
        await cmd.ExecuteNonQueryAsync();
    }

    private static string ObtenerDireccionIP()
    {
        try
        {
            IPHostEntry host = Dns.GetHostEntry(Dns.GetHostName());
            foreach (IPAddress ip in host.AddressList)
            {
                if (ip.AddressFamily == AddressFamily.InterNetwork)
                {
                    return ip.ToString();
                }
            }
        }
        catch
        {
            // Ignorar errores al obtener IP
        }
        return "127.0.0.1";
    }
}
