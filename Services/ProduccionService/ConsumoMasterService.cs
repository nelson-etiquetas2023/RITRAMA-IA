using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Services.ProduccionService;

public class ConsumoMasterService : IConsumoMasterService
{
    private readonly string _conn;
    public string ErrorMsg { get; set; } = null!;

    public ConsumoMasterService(IConfiguration config)
    {
        _conn = ConexionResolver.Resolver(config);
    }

    public async Task<bool> UpdateInventaryMasterInitial(object objeto)
    {
        try
        {
            Type tipo = objeto.GetType();
            object? rollIdProperty = tipo.GetProperty("roll_id")!.GetValue(objeto);
            object? consumoParcialProperty = tipo.GetProperty("consumo")!.GetValue(objeto);
            object? nameTableProperty = tipo.GetProperty("nametable")!.GetValue(objeto);
            string? sqlProperty = tipo.GetProperty("sql")!.GetValue(objeto)!.ToString();

            SqlParameter[] parametros =
            [
                new SqlParameter("@consumo", consumoParcialProperty),
                new SqlParameter("@rollid", rollIdProperty)
            ];

            await ProduccionDataAccess.CargarTablaAsync(_conn, sqlProperty!, false, null, parametros, nameTableProperty!.ToString(), false);

            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al actualizar los inventarios de master Inic [error code: ] " + ex.Message);
            return false;
        }
    }

    public async Task<DataTable?> LoadTableMasterInic()
    {
        try
        {
            var tabla = new { nameTabla = "MasterInic", sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID };

            DataTable? dt = await ProduccionDataAccess.CargarTablaAsync(_conn, tabla.sql, false, null, null, tabla.nameTabla, true);

            if (dt == null)
            {
                throw new InvalidOperationException("La tabla MasterInic no se pudo cargar correctamente.");
            }
            else
            {
                return dt;
            }
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al cargar la tabla de los iniciales [error code: ] " + ex.Message);
            return null;
        }
    }

    public async Task<bool> UpdateDetailsConsumosMasterIniciales(string rollid, string orden, double length_consumo, DateTime fecha_reg, bool desperdicio)
    {
        try
        {
            var tabla = new { nameTabla = "MasterDetailsInic", sql = R.QUERY.PRODUCTION.UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES };

            SqlParameter[] parameros =
            [
                new SqlParameter("@rollid", rollid),
                new SqlParameter("@orden", orden),
                new SqlParameter("@consumo", length_consumo),
                new SqlParameter("@fecha", fecha_reg),
                new SqlParameter("@desperdicio", desperdicio)
            ];

            await ProduccionDataAccess.CargarTablaAsync(_conn, tabla.sql, false, null, parameros, tabla.nameTabla, false);
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al actualizar el detalle de los master [error code: ] " + ex.Message);
            return false;
        }
    }

    public async Task<DataTable?> LoadDataDetailsConsumosMasterInic(string rollid)
    {
        try
        {
            var tabla = new { nameTabla = "orden_corte", sql = R.QUERY.PRODUCTION.SQL_SELECT_QUERY_LOAD_DETAILS_MASTER_INICIALES_START_TRANSACTIONS };

            SqlParameter[] parametros =
            [
                new SqlParameter("@rollid", rollid)
            ];

            DataTable? dt = await ProduccionDataAccess.CargarTablaAsync(_conn, tabla.sql, false, null, parametros, tabla.nameTabla, true);

            if (dt == null)
            {
                throw new InvalidOperationException("La tabla de detalle de consumos de master no se pudo cargar correctamente.");
            }
            else
            {
                return dt;
            }
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al cargar los detalles de los master iniciales [error code: ] " + ex.Message);
            return null;
        }
    }

    public async Task<bool> ActualizarInventariosMasterAsync(string rollid, string orden, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMaster, string ocNumero)
    {
        string sqlInv = tipoMaster.ToUpper().Trim() == "INIC."
            ? R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_INICIALES
            : R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_MATERIA;

        try
        {
            using SqlConnection conn = new(_conn);
            await conn.OpenAsync();
            using SqlTransaction tran = conn.BeginTransaction();

            // IDEMPOTENCIA: el consumo de inventario ya se registra al ETIQUETAR la OC
            // (GuardarEtiquetado). Este metodo se mantiene como red de seguridad para el cierre
            // y OC historicas; si el detalle (rollid, orden, desperdicio) ya existe (etiquetado
            // previo, cierre repetido o corrida anterior) NO se vuelve a descontar stock ni se
            // duplica el detalle. Solo se libera disponible=1 de los rollos cortados.
            bool consumoRegistrado = await ConsumoDetalleExisteAsync(conn, tran, rollid, orden, false);
            bool despRegistrado = false;
            if (desperdicio && consumoDesperdicio > 0)
            {
                despRegistrado = await ConsumoDetalleExisteAsync(conn, tran, rollid, orden, true);
            }

            // REGLA RN-CONSUMO-RESTANTE: el restante del inventario NO puede quedar en negativo.
            // Antes de descontar stock se calcula lo que se va a registrar por primera vez
            // (evita el doble descuento idempotente) y se exige que quepa en el restante real
            // del master (largo - total registrado en el libro de consumo MasterDetailsInic).
            double aEscribir = (consumoRegistrado ? 0 : consumoReal)
                             + (despRegistrado ? 0 : (desperdicio ? consumoDesperdicio : 0));
            if (aEscribir > 0.01)
            {
                double restante = await RestanteMasterLedgerAsync(conn, tran, rollid);
                if (aEscribir > restante + 0.01)
                {
                    string tipo = tipoMaster.ToUpper().Trim() == "INIC." ? "master" : "material";
                    ErrorMsg = $"REGLA RN-CONSUMO-RESTANTE: el {tipo} {rollid} no tiene material suficiente: " +
                        $"quedan {restante:N2} pies y se intentan registrar {aEscribir:N2} pies de consumo. " +
                        "El restante del inventario no puede ser negativo.";
                    return false;
                }
            }

            if (!consumoRegistrado)
            {
                await EjecutarAsync(conn, tran, R.QUERY.PRODUCTION.UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES,
                    new SqlParameter[] {
                        new("@rollid", rollid), new("@orden", orden), new("@consumo", consumoReal),
                        new("@fecha", DateTime.Now), new("@desperdicio", false)
                    });

                await EjecutarAsync(conn, tran, sqlInv,
                    new SqlParameter[] { new("@consumo", consumoReal), new("@rollid", rollid) });
            }

            if (despRegistrado == false && desperdicio && consumoDesperdicio > 0)
            {
                await EjecutarAsync(conn, tran, R.QUERY.PRODUCTION.UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES,
                    new SqlParameter[] {
                        new("@rollid", rollid), new("@orden", orden), new("@consumo", consumoDesperdicio),
                        new("@fecha", DateTime.Now), new("@desperdicio", true)
                    });

                await EjecutarAsync(conn, tran, sqlInv,
                    new SqlParameter[] { new("@consumo", consumoDesperdicio), new("@rollid", rollid) });
            }

            await EjecutarAsync(conn, tran, "UPDATE rolls_details SET disponible=1 WHERE numero=@p1",
                new SqlParameter[] { new("@p1", ocNumero) });

            tran.Commit();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al actualizar los inventarios de master... error code: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Restante real del inventario del rollid segun el libro contable de consumos
    /// (MasterDetailsInic): largo del master − total registrado. Si el rollid no existe
    /// devuelve infinito (no bloquea). REGLA RN-CONSUMO-RESTANTE: este restante nunca debe
    /// quedar en negativo al registrar un consumo nuevo.
    /// </summary>
    private async Task<double> RestanteMasterLedgerAsync(SqlConnection conn, SqlTransaction tran, string rollid)
    {
        const string sql = @"
SELECT COALESCE((SELECT TOP 1 Lenght FROM MasterInic WHERE Roll_Id = @rollid),
                (SELECT TOP 1 length FROM ItemsMateria WHERE rollid = @rollid), 0)
     - COALESCE((SELECT SUM(consumo) FROM MasterDetailsInic WHERE rollid = @rollid), 0)";
        using SqlCommand cmd = new(sql, conn, tran);
        cmd.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
        object? r = await cmd.ExecuteScalarAsync();
        return r != null && r != DBNull.Value ? Convert.ToDouble(r) : double.PositiveInfinity;
    }

    /// <summary>
    /// Indica si el detalle de consumo (rollid, orden, desperdicio) ya existe en MasterDetailsInic,
    /// para no registrar dos veces el mismo consumo (idempotencia cierre/etiquetado).
    /// </summary>
    private async Task<bool> ConsumoDetalleExisteAsync(SqlConnection conn, SqlTransaction tran, string rollid, string orden, bool esDesperdicio)
    {
        using SqlCommand comando = new(R.QUERY.PRODUCTION.SQL_QUERY_CONSUMO_OC_DETALLE_EXISTE, conn, tran);
        comando.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
        comando.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = orden });
        comando.Parameters.Add(new SqlParameter("@desperdicio", SqlDbType.Bit) { Value = esDesperdicio });
        object? resultado = await comando.ExecuteScalarAsync();
        return resultado != null;
    }

    public async Task<bool> ReasignarConsumoMasterAsync(string orden, string rollidAnterior, string rollidNuevo, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMasterAnterior, string tipoMasterNuevo)
    {
        try
        {
            string sqlInvAnterior = tipoMasterAnterior.ToUpper().Trim() == "INIC."
                ? R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_INICIALES
                : R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_MATERIA;
            string sqlInvNuevo = tipoMasterNuevo.ToUpper().Trim() == "INIC."
                ? R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_INICIALES
                : R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_MATERIA;

            using SqlConnection conn = new(_conn);
            await conn.OpenAsync();
            using SqlTransaction tran = conn.BeginTransaction();

            await ReasignarAsync(conn, tran, sqlInvAnterior, sqlInvNuevo,
                rollidAnterior, rollidNuevo, orden, consumoReal, false);

            if (desperdicio && consumoDesperdicio > 0)
            {
                await ReasignarAsync(conn, tran, sqlInvAnterior, sqlInvNuevo,
                    rollidAnterior, rollidNuevo, orden, consumoDesperdicio, true);
            }

            tran.Commit();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al reasignar el consumo entre masters... error code: " + ex.Message);
            return false;
        }
    }

    /// <summary>
    /// Devuelve el material consumido al inventario del master anterior (consumo negativo sobre el
    /// largo consumido y detalle) y consume el mismo material del nuevo master asignado al documento.
    /// </summary>
    private async Task ReasignarAsync(SqlConnection conn, SqlTransaction tran, string sqlInvAnterior, string sqlInvNuevo,
        string rollidAnterior, string rollidNuevo, string orden, double consumo, bool esDesperdicio)
    {
        await EjecutarAsync(conn, tran, sqlInvAnterior,
            new SqlParameter[] { new("@consumo", -consumo), new("@rollid", rollidAnterior) });
        await EjecutarAsync(conn, tran, R.QUERY.PRODUCTION.UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES,
            new SqlParameter[] {
                new("@rollid", rollidAnterior), new("@orden", orden), new("@consumo", -consumo),
                new("@fecha", DateTime.Now), new("@desperdicio", esDesperdicio)
            });

        await EjecutarAsync(conn, tran, sqlInvNuevo,
            new SqlParameter[] { new("@consumo", consumo), new("@rollid", rollidNuevo) });
        await EjecutarAsync(conn, tran, R.QUERY.PRODUCTION.UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES,
            new SqlParameter[] {
                new("@rollid", rollidNuevo), new("@orden", orden), new("@consumo", consumo),
                new("@fecha", DateTime.Now), new("@desperdicio", esDesperdicio)
            });
    }

    private async Task EjecutarAsync(SqlConnection conn, SqlTransaction tran, string sql, SqlParameter[] parametros)
    {
        using SqlCommand comando = new()
        {
            Connection = conn,
            Transaction = tran,
            CommandText = sql,
            CommandType = CommandType.Text
        };
        comando.Parameters.AddRange(parametros);
        await comando.ExecuteNonQueryAsync();
    }
}
