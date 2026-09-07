using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService;

public class OrdenCorteService : IOrdenCorteService
{
    private readonly string _conn;
    private readonly DataSet _ds = new();
    public string ErrorMsg { get; set; } = null!;

    public OrdenCorteService(IConfiguration config)
    {
        _conn = ConexionResolver.Resolver(config);
    }

    public List<ConfigVueltas> GetConfigVueltas(string oc)
    {
        List<ConfigVueltas> lista = [];

        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();

            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "SELECT orden,num_vuelta,longitud_cortar,rollos,splice FROM vueltas WHERE orden=@p1",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = oc });


            using SqlDataReader reader = comando.ExecuteReader();

            while (reader.Read())
            {
                ConfigVueltas item = new()
                {
                    OrdenCorte = reader.GetString(reader.GetOrdinal("orden")),
                    Vuelta_numero = reader.GetInt32(reader.GetOrdinal("num_vuelta")),
                    Longitud_Cortar = Convert.ToDouble(reader["longitud_cortar"]),
                    Rollos = reader.GetString(reader.GetOrdinal("rollos")),
                    Splice = reader.GetInt32(reader.GetOrdinal("splice")),
                };
                lista.Add(item);
            }
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al cargar las configuracion de las vueltas..." + ex.Message);
        }
        return lista;
    }

    public void UpdateConfigVueltas(List<ConfigVueltas> lista)
    {
        if (lista == null || lista.Count == 0) return;
        GuardarConfigVueltas(lista);
    }

    public void GuardarConfigVueltas(List<ConfigVueltas> lista)
    {
        if (lista == null || lista.Count == 0) return;
        using SqlConnection conn = new(_conn);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            using SqlCommand comandoClear = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "DELETE FROM vueltas WHERE orden=@p1",
                CommandType = CommandType.Text
            };
            comandoClear.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = lista.FirstOrDefault()!.OrdenCorte! });
            comandoClear.ExecuteNonQuery();

            foreach (var item in lista)
            {
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "INSERT INTO vueltas (orden,num_vuelta,longitud_cortar,rollos,splice) VALUES(@p1,@p2,@p3,@p4,@p5)",
                    CommandType = CommandType.Text
                };
                comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = item.OrdenCorte });
                comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.Int) { Value = item.Vuelta_numero });
                comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.Float) { Value = item.Longitud_Cortar });
                comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = item.Rollos });
                comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.Int) { Value = item.Splice });
                comando.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch (Exception ex)
        {
            try { transaction.Rollback(); } catch { }
            ServiceErrors.Report("error al guardar las configuracion de las vueltas..." + ex.Message);
        }
    }

    private string BuildSqlRollIdDisponibles(string columna, string texto)
    {
        string sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID;
        string busq1, busq2;
        switch (columna)
        {
            case "Part_Number": busq1 = "a.Part_Number"; busq2 = "a.product_id"; break;
            case "Product_Name": busq1 = "b.Product_Name"; busq2 = "b.Product_Name"; break;
            default: busq1 = "a.Roll_Id"; busq2 = "a.rollid"; break;
        }
        string textoCond1 = string.IsNullOrWhiteSpace(texto) ? "" : $" AND (@text = '%' OR {busq1} LIKE @text)";
        string textoCond2 = string.IsNullOrWhiteSpace(texto) ? "" : $" AND (@text = '%' OR {busq2} LIKE @text)";
        string cond1 = $"b.MasterRolls = 1 AND (a.Lenght - ISNULL(ct.largo_consumido,0)) > 100{textoCond1}";
        string cond2 = $"b.MasterRolls = 1 AND (a.length - ISNULL(ct.largo_consumido,0)) > 100{textoCond2}";
        sql = sql.Replace("b.MasterRolls = 1 UNION ALL", cond1 + " UNION ALL");
        sql = sql.Replace("b.MasterRolls = 1 ORDER BY Roll_Id", cond2 + " ORDER BY Roll_Id");
        return sql;
    }

    public async Task<DataTable> LoadDataRollID()
    {
        string sql = BuildSqlRollIdDisponibles("Roll_Id", "");
        var dt = await ProduccionDataAccess.CargarTablaAsync(_conn, sql, false, null, null, "DtRollid", true);
        return dt ?? new DataTable("DtRollid");
    }

    public async Task<DataTable> BuscarRollId(string columna, string texto)
    {
        string sql = BuildSqlRollIdDisponibles(columna, texto);
        SqlParameter[]? parametros = string.IsNullOrWhiteSpace(texto)
            ? null
            : new[] { new SqlParameter("@text", "%" + texto.Trim() + "%") };
        return (await ProduccionDataAccess.CargarTablaAsync(_conn, sql, false, null, parametros, null, true))!;
    }

    public bool AnularOrdenCorte(string numeroc)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();

            using SqlCommand check = new()
            {
                Connection = conn,
                CommandText = "SELECT TOP 1 CASE WHEN anulada = 1 THEN 2 WHEN CloseDocument = 1 THEN 3 ELSE 1 END FROM orden_corte WHERE numero = @oc",
                CommandType = CommandType.Text
            };
            check.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = numeroc });
            var estado = check.ExecuteScalar();

            if (estado == null)
            {
                ServiceErrors.Report("La orden de corte " + numeroc + " no existe.");
                return false;
            }

            int codigo = Convert.ToInt32(estado);
            if (codigo == 2)
            {
                ServiceErrors.Report("La orden de corte " + numeroc + " ya se encuentra anulada.");
                return false;
            }
            if (codigo == 3)
            {
                ServiceErrors.Report("No se puede anular la orden de corte " + numeroc + " porque esta cerrada.");
                return false;
            }

            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update orden_corte set anulada=1 where numero=@oc",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = numeroc });
            comando.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al anular la orden de corte " + ex.Message);
            return false;
        }
    }

    public async Task<DataSet> LoadDataOC()
    {
        try
        {
            _ds.RejectChanges();
            _ds.Relations.Clear();
            foreach (DataTable table in _ds.Tables)
            {
                table.Clear();
            }
            _ds.Tables.Clear();
            _ds.AcceptChanges();
            var tablas = new[]
            {
                new { Nombre = "DtMaster", Sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_OC_HEADER },
                new { Nombre = "DtRollos", Sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_OC_ROLLO_CORTADO },
                new { Nombre = "DtOperator", Sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_OPERATOR },
                new { Nombre = "DtCustomer", Sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_CUSTOMER },
                new { Nombre = "DtCortes", Sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_OC_CORTES },
                //new { Nombre = "DtRollid", Sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID },

            };

            // P0: cargar las 5 tablas en paralelo (cada una con su propia conexion) y
            // fusionarlas en el DataSet de forma secuencial (DataSet no es thread-safe);
            // anteriormente eran 5 round-trips secuenciales.
            var resultados = await Task.WhenAll(
                tablas.Select(t => ProduccionDataAccess.CargarTablaAsync(_conn, t.Sql, false, null, null, t.Nombre, true)));

            foreach (var (tabla, dt) in tablas.Zip(resultados))
            {
                dt!.TableName = tabla.Nombre;
                _ds.Tables.Add(dt);
            }

            SetRelaionsTables();

        }
        catch (Exception ex)
        {
            ErrorMsg = ex.Message;
            throw;
        }

        return _ds;
    }

    private bool SetRelaionsTables()
    {
        try
        {

            var relacion = new DataRelation(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS,
                            _ds.Tables["DtMaster"]!.Columns["numero"]!,
                            _ds.Tables["DtRollos"]!.Columns["numero"]!, false);

            if (!_ds.Relations.Contains(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS))
                _ds.Relations.Add(relacion);

            DataColumn? ParentCol0 = _ds.Tables["DtMaster"]?.Columns["numero"];
            DataColumn? ChildCol0 = _ds.Tables["DtCortes"]?.Columns["orden"];

            DataRelation Despacho_Cortes = new("FK_ENCABEZADO_CORTES", ParentCol0!, ChildCol0!, false);
            _ds.Relations.Add(Despacho_Cortes);
            return true;
        }
        catch (ConstraintException ex)
        {
            ErrorMsg = ex.Message;
            return false;
        }
        catch (InvalidOperationException ex)
        {
            ErrorMsg = ex.Message;
            return false;
        }
    }

    public bool GuardarEncabezadoOrdenCorte(Orden OrdenCorte)
    {
        using SqlConnection conn = new(_conn);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            GuardarEncabezadoOrdenCorteCore(OrdenCorte, conn, transaction);
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            try { transaction.Rollback(); } catch { }
            ServiceErrors.Report("error al guardar el encabezado de la orden " + ex.Message);
            return false;
        }
    }

    private void GuardarEncabezadoOrdenCorteCore(Orden OrdenCorte, SqlConnection conn, SqlTransaction transaction)
    {
        using SqlCommand comando = new()
        {
            Connection = conn,
            Transaction = transaction,
            CommandText = "INSERT INTO orden_corte (numero,fecha,fecha_produccion,product_id,rollid_1,width_1,lenght_1,rollid_2,width_2,lenght_2,anulada,procesado,CloseDocument,tot_inch_ancho,longitud_cortar,cortes_ancho,cortes_largo,cant_rollos,decartable1_pies,lenght_master_real,util1_real_width,util1_real_lenght,descartable2_pies" + ",util2_real_width,util2_real_lenght,lenght_master_real2,rest1_width,rest1_lenght,rest2_width,rest2_lenght,cant_rollos2,cortes_largo2,step,lastupdate,fecha_autorize,toautorize,notes,tipo_mov1,tipo_mov2,plus1_pies,plus2_pies,rollo_unificado,length_entrada,real_usado_r1,real_usado_r2,restante_rollid1,restante_rollid2,resta_entrada,total_salida,lenght_entrada,customer_id,operador_id,SellOrder,desperdicio,master_tipo,ubicacion,TwoMasters,longitud_cortar2,vueltas2,cantidad_rollos2,desperdicio2,MachineName,DireccionIp) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19,@p20,@p21,@p22,@p23,@p24,@p25,@p26,@p27,@p28,@p29,@p30,@p31,@p32,@p33,@p34,@p35,@p36,@p37,@p38,@p39,@p40,@p41,@p44,@p45,@p46,@p47,@p48,@p49,@p50,@p51,@p52,@customer_id,@operador_id,@SellOrder,@desper,@MasterTipo,@ubic,@twomasters,@longcortar2,@vueltas2,@cantrollos2,@desper2,@Machine,@Ip)",
            CommandType = CommandType.Text
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = OrdenCorte.Numero });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.DateTime) { Value = OrdenCorte.Fecha });
        comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.DateTime) { Value = OrdenCorte.Fecha_produccion });
        comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = OrdenCorte.Product_id });
        comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.NVarChar) { Value = OrdenCorte.Rollid_1 });
        comando.Parameters.Add(new SqlParameter("@p6", SqlDbType.Decimal) { Value = OrdenCorte.Width_1 });
        comando.Parameters.Add(new SqlParameter("@p7", SqlDbType.Decimal) { Value = OrdenCorte.Lenght_1 });
        comando.Parameters.Add(new SqlParameter("@p8", SqlDbType.NVarChar) { Value = OrdenCorte.Rollid_2 });
        comando.Parameters.Add(new SqlParameter("@p9", SqlDbType.Decimal) { Value = OrdenCorte.Width_2 });
        comando.Parameters.Add(new SqlParameter("@p10", SqlDbType.Decimal) { Value = OrdenCorte.Lenght_2 });
        comando.Parameters.Add(new SqlParameter("@p11", SqlDbType.Bit) { Value = OrdenCorte.Anulada });
        comando.Parameters.Add(new SqlParameter("@p12", SqlDbType.Bit) { Value = OrdenCorte.Procesado });
        comando.Parameters.Add(new SqlParameter("@p13", SqlDbType.Bit) { Value = OrdenCorte.CloseDocument });
        comando.Parameters.Add(new SqlParameter("@p14", SqlDbType.Float) { Value = OrdenCorte.Total_Inch_Ancho });
        comando.Parameters.Add(new SqlParameter("@p15", SqlDbType.Float) { Value = OrdenCorte.Longitud_Cortar });
        comando.Parameters.Add(new SqlParameter("@p16", SqlDbType.Int) { Value = OrdenCorte.Cortes_Ancho });
        comando.Parameters.Add(new SqlParameter("@p17", SqlDbType.Int) { Value = OrdenCorte.Cortes_Largo });
        comando.Parameters.Add(new SqlParameter("@p18", SqlDbType.Int) { Value = OrdenCorte.Cantidad_Rollos });
        comando.Parameters.Add(new SqlParameter("@p19", SqlDbType.Float) { Value = OrdenCorte.Descartable1_pies });
        comando.Parameters.Add(new SqlParameter("@p20", SqlDbType.Float) { Value = OrdenCorte.Lenght_Master_Real });
        comando.Parameters.Add(new SqlParameter("@p21", SqlDbType.Float) { Value = OrdenCorte.Util1_Real_Width });
        comando.Parameters.Add(new SqlParameter("@p22", SqlDbType.Float) { Value = OrdenCorte.Util1_real_Lenght });
        comando.Parameters.Add(new SqlParameter("@p23", SqlDbType.Float) { Value = OrdenCorte.Descartable2_pies });
        comando.Parameters.Add(new SqlParameter("@p24", SqlDbType.Float) { Value = OrdenCorte.Util2_Real_Width });
        comando.Parameters.Add(new SqlParameter("@p25", SqlDbType.Float) { Value = OrdenCorte.Util2_real_Lenght });
        comando.Parameters.Add(new SqlParameter("@p26", SqlDbType.Float) { Value = OrdenCorte.Master_lenght2_Real });
        comando.Parameters.Add(new SqlParameter("@p27", SqlDbType.Float) { Value = OrdenCorte.Rest1_width });
        comando.Parameters.Add(new SqlParameter("@p28", SqlDbType.Float) { Value = OrdenCorte.Rest1_lenght });
        comando.Parameters.Add(new SqlParameter("@p29", SqlDbType.Float) { Value = OrdenCorte.Rest2_width });
        comando.Parameters.Add(new SqlParameter("@p30", SqlDbType.Float) { Value = OrdenCorte.Rest2_lenght });
        comando.Parameters.Add(new SqlParameter("@p31", SqlDbType.Int) { Value = OrdenCorte.Cantidad_Rollos2 });
        comando.Parameters.Add(new SqlParameter("@p32", SqlDbType.Int) { Value = OrdenCorte.Cortes_Largo2 });
        comando.Parameters.Add(new SqlParameter("@p33", SqlDbType.Int) { Value = OrdenCorte.Step });
        comando.Parameters.Add(new SqlParameter("@p34", SqlDbType.DateTime) { Value = OrdenCorte.LastUpdate });
        comando.Parameters.Add(new SqlParameter("@p35", SqlDbType.DateTime) { Value = OrdenCorte.FechaAutorize });
        comando.Parameters.Add(new SqlParameter("@p36", SqlDbType.NVarChar) { Value = OrdenCorte.ToAutorize });
        comando.Parameters.Add(new SqlParameter("@p37", SqlDbType.NVarChar) { Value = OrdenCorte.Note });
        comando.Parameters.Add(new SqlParameter("@p38", SqlDbType.NVarChar) { Value = OrdenCorte.Tipo_Mov1 });
        comando.Parameters.Add(new SqlParameter("@p39", SqlDbType.NVarChar) { Value = OrdenCorte.Tipo_Mov2 });
        comando.Parameters.Add(new SqlParameter("@p40", SqlDbType.Decimal) { Value = OrdenCorte.Plus1_pies });
        comando.Parameters.Add(new SqlParameter("@p41", SqlDbType.Decimal) { Value = OrdenCorte.Plus2_pies });
        comando.Parameters.Add(new SqlParameter("@p44", SqlDbType.Bit) { Value = OrdenCorte.Rollo_unificado });
        comando.Parameters.Add(new SqlParameter("@p45", SqlDbType.Decimal) { Value = OrdenCorte.Lenght_entrada });
        comando.Parameters.Add(new SqlParameter("@p46", SqlDbType.Float) { Value = OrdenCorte.Real_usado_r1 });
        comando.Parameters.Add(new SqlParameter("@p47", SqlDbType.Float) { Value = OrdenCorte.Real_usado_r2 });
        comando.Parameters.Add(new SqlParameter("@p48", SqlDbType.NVarChar) { Value = OrdenCorte.Restante_rollid1 });
        comando.Parameters.Add(new SqlParameter("@p49", SqlDbType.NVarChar) { Value = OrdenCorte.Restante_rollid2 });
        comando.Parameters.Add(new SqlParameter("@p50", SqlDbType.Int) { Value = 0 });
        comando.Parameters.Add(new SqlParameter("@p51", SqlDbType.Int) { Value = 0 });
        comando.Parameters.Add(new SqlParameter("@p52", SqlDbType.Int) { Value = 0 });
        comando.Parameters.Add(new SqlParameter("@SellOrder", SqlDbType.NVarChar) { Value = OrdenCorte.SellOrder });
        comando.Parameters.Add(new SqlParameter("@desper", SqlDbType.Bit) { Value = OrdenCorte.Desperdicio });
        comando.Parameters.Add(new SqlParameter("@desper2", SqlDbType.Bit) { Value = OrdenCorte.Desperdicio2 });
        comando.Parameters.Add(new SqlParameter("@MasterTipo", SqlDbType.NVarChar) { Value = OrdenCorte.Master_Tipo });
        comando.Parameters.Add(new SqlParameter("@ubic", SqlDbType.NVarChar) { Value = OrdenCorte.Ubicacion });
        comando.Parameters.Add(new SqlParameter("@twomasters", SqlDbType.Bit) { Value = OrdenCorte.TwoMasters });
        comando.Parameters.Add(new SqlParameter("@longcortar2", SqlDbType.Float) { Value = OrdenCorte.Longitud_Cortar2 });
        comando.Parameters.Add(new SqlParameter("@vueltas2", SqlDbType.Int) { Value = OrdenCorte.Vueltas2 });
        comando.Parameters.Add(new SqlParameter("@cantrollos2", SqlDbType.Int) { Value = OrdenCorte.Cantidad_rollos2 });
        comando.Parameters.Add(new SqlParameter("@Machine", SqlDbType.NVarChar) { Value = OrdenCorte.MachineName });
        comando.Parameters.Add(new SqlParameter("@Ip", SqlDbType.NVarChar) { Value = OrdenCorte.DireccionIp });
        comando.Parameters.Add(new SqlParameter("@customer_id", SqlDbType.UniqueIdentifier)
        {
            Value = OrdenCorte.Customer_Id
        });
        comando.Parameters.Add(new SqlParameter("@operador_id", SqlDbType.UniqueIdentifier)
        {
            Value = OrdenCorte.Operador_id
        });
        comando.ExecuteNonQuery();
    }

    public bool GuardarCortes(List<Corte> cortes)
    {
        if (cortes == null || cortes.Count == 0) return true;
        using SqlConnection conn = new(_conn);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            foreach (var corte in cortes)
                GuardarCorteCore(corte, conn, transaction);
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            try { transaction.Rollback(); } catch { }
            ServiceErrors.Report("error al guardar los cortes " + ex.Message);
            return false;
        }
    }

    private void GuardarCorteCore(Corte corte, SqlConnection conn, SqlTransaction transaction)
    {
        using SqlCommand comando = new()
        {
            Connection = conn,
            Transaction = transaction,
            CommandText = "INSERT INTO cortes (num,width,lenght,msi,orden) VALUES(@p1,@p2,@p3,@p4,@p5)",
            CommandType = CommandType.Text
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = corte.Numero });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.Float) { Value = corte.Width });
        comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.Float) { Value = corte.Length });
        comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.Float) { Value = corte.Msi });
        comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.Float) { Value = corte.Orden });
        comando.ExecuteNonQuery();
    }

    public bool GuardarRollos(List<RolloCortado> rollos)
    {
        if (rollos == null || rollos.Count == 0) return true;
        using SqlConnection conn = new(_conn);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            foreach (var roll in rollos)
                GuardarRolloCore(roll, conn, transaction);
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            try { transaction.Rollback(); } catch { }
            ServiceErrors.Report("error al guardar los rollos " + ex.Message);
            return false;
        }
    }

    private void GuardarRolloCore(RolloCortado roll, SqlConnection conn, SqlTransaction transaction)
    {
        using SqlCommand comando = new()
        {
            Connection = conn,
            Transaction = transaction,
            CommandText = "INSERT INTO rolls_details (product_id,product_name,roll_number,unique_code,splice,width,large,msi,roll_id,code_person,status,disponible,ubic,numero,vuelta) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,1,@p13,@p14,@p15)",
            CommandType = CommandType.Text
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = roll.Product_Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = roll.Product_Name });
        comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.Int) { Value = roll.RollNumber });
        comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = roll.UniqueCode });
        comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.Int) { Value = roll.Splice });
        comando.Parameters.Add(new SqlParameter("@p6", SqlDbType.Float) { Value = roll.Width });
        comando.Parameters.Add(new SqlParameter("@p7", SqlDbType.Float) { Value = roll.Length });
        comando.Parameters.Add(new SqlParameter("@p8", SqlDbType.Float) { Value = roll.Msi });
        comando.Parameters.Add(new SqlParameter("@p9", SqlDbType.NVarChar) { Value = roll.Roll_Id });
        comando.Parameters.Add(new SqlParameter("@p10", SqlDbType.NVarChar) { Value = roll.Code_Person });
        comando.Parameters.Add(new SqlParameter("@p11", SqlDbType.NVarChar) { Value = roll.Status });
        comando.Parameters.Add(new SqlParameter("@p13", SqlDbType.NVarChar) { Value = roll.Ubicacion });
        comando.Parameters.Add(new SqlParameter("@p14", SqlDbType.NVarChar) { Value = roll.Numero });
        comando.Parameters.Add(new SqlParameter("@p15", SqlDbType.Int) { Value = roll.Vuelta });
        comando.ExecuteNonQuery();
    }

    /// <summary>
    /// Guarda el documento de Orden de Corte completo (encabezado + cortes + rollos) dentro de
    /// UNA sola transaccion, de modo que un fallo en cualquier parte revierta todo el documento.
    /// </summary>
    public bool GuardarOrdenCompleta(Orden orden, List<Corte>? cortes, List<RolloCortado>? rollos)
    {
        if (cortes == null || cortes.Count == 0)
        {
            ServiceErrors.Report("No se puede guardar la orden de corte: la orden no tiene renglones de cortes.");
            return false;
        }
        if (rollos == null || rollos.Count == 0)
        {
            ServiceErrors.Report("No se puede guardar la orden de corte: la orden no tiene detalle de rollos cortados.");
            return false;
        }

        using SqlConnection conn = new(_conn);
        conn.Open();
        using var transaction = conn.BeginTransaction();
        try
        {
            GuardarEncabezadoOrdenCorteCore(orden, conn, transaction);
            foreach (var corte in cortes)
                GuardarCorteCore(corte, conn, transaction);
            foreach (var roll in rollos)
                GuardarRolloCore(roll, conn, transaction);
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            try { transaction.Rollback(); } catch { }
            ServiceErrors.Report("error al guardar la orden de corte completa " + ex.Message);
            return false;
        }
    }

    public bool UpdateStatusDocumentOC(int stepchange, string oc)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update orden_corte set step=@p2 where numero=@p1",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@p1", oc);
            SqlParameter p2 = new("@p2", stepchange);
            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al actualizar el estatus del documento. codigo error: " + ex.Message);
            return false;
        }

    }

    public void UpdateUniqueCodeRollosCortados(List<RolloCortado> rollos)
    {
        if (rollos == null || rollos.Count == 0) return;
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            foreach (var roll in rollos)
            {
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "update rolls_details set unique_code=@p2 where roll_number=@p1 and numero=@p3",
                    CommandType = CommandType.Text
                };
                comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = roll.RollNumber });
                comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = roll.UniqueCode });
                comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = roll.Numero });
                comando.ExecuteNonQuery();
            }
            transaction.Commit();
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al guardar los codigo unicos de los rollos" + ex.Message);
        }
    }

    /// <summary>
    /// Valida que el rango de codigos unicos [RC(inicio)..RC(fin)] no colisione con codigos
    /// ya asignados a rollos de otras ordenes (evita saltos/duplicados en el consecutivo global UC).
    /// Devuelve la lista de numeros ocupados (vacía si el rango está libre).
    /// </summary>
    public List<int> ValidarRangoUniqueCodeGlobal(int inicio, int fin, string numeroOc)
    {
        List<int> ocupados = [];
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = @"
                    SELECT TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code))) AS uc
                    FROM rolls_details
                    WHERE numero <> @numero
                      AND unique_code LIKE 'RC%'
                      AND TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code))) BETWEEN @inicio AND @fin",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@numero", SqlDbType.NVarChar) { Value = numeroOc });
            comando.Parameters.Add(new SqlParameter("@inicio", SqlDbType.Int) { Value = inicio });
            comando.Parameters.Add(new SqlParameter("@fin", SqlDbType.Int) { Value = fin });
            using var reader = comando.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0)) ocupados.Add(reader.GetInt32(0));
            }
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al validar el rango de codigos unicos. Codigo de Error : " + ex.Message);
        }
        return ocupados;
    }

    public bool CheckOperatorDefault(string id, string name)
    {
        try
        {
            using SqlConnection Conn = new(_conn);
            Conn.Open();

            using SqlCommand comando = new()
            {
                Connection = Conn,
                CommandType = CommandType.Text,
                CommandText = "select COUNT(*) from operadores where operador_id = @id"
            };

            SqlParameter p1 = new("@id", id);
            comando.Parameters.Add(p1);

            var result = (int)comando.ExecuteScalar();
            if (result > 0)
            {
                return true;
            }
            else
            {
                AddOperatorDefault(id, name);
                return false;
            }
        }
        catch (Exception)
        {
            return false;
        }
    }

    public void AddOperatorDefault(string id, string name)
    {
        try
        {
            using SqlConnection Conn = new(_conn);
            Conn.Open();
            using SqlCommand comando = new()
            {
                Connection = Conn,
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO operadores (operador_id,nombre,status) VALUES (@id,@name,1)"
            };
            SqlParameter p1 = new("@id", id);
            SqlParameter p2 = new("@name", name);
            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2);
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al agregar el operador por defecto: " + ex.Message);
        }

    }

    public bool OrdenUpdateCodePerson(string orden, string code_person)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update rolls_details set code_person=@code_per where numero=@orden",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@code_per", code_person);
            SqlParameter p2 = new("@orden", orden);

            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2);

            comando.ExecuteNonQuery();

            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al actualizar el codigo personalizado. Codigo de Error : " + ex.Message);
            return false;
        }

    }

    public bool UpdateOrdenCorte(Orden orden)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update orden_corte set fecha=@fecha,fecha_produccion=@fecha_pro,operador_id=@oper,sellOrder=@sellOrder,desperdicio=@desper,customer_id=@CustId where numero=@orden",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@orden", orden.Numero);
            SqlParameter p2 = new("@fecha", orden.Fecha);
            SqlParameter p3 = new("@fecha_pro", orden.Fecha_produccion);
            SqlParameter p4 = new("@oper", orden.Operador_id);
            SqlParameter p5 = new("@sellOrder", orden.SellOrder);
            SqlParameter p6 = new("@desper", orden.Desperdicio);
            SqlParameter p7 = new("@CustId", orden.Customer_Id);
            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2);
            comando.Parameters.Add(p3);
            comando.Parameters.Add(p4);
            comando.Parameters.Add(p5);
            comando.Parameters.Add(p6);
            comando.Parameters.Add(p7);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al actualizar la orden de corte: error[code] :" + ex.Message);
            return false;
        }
    }

    public void Update_Items_Orden_Corte(List<RolloCortado> rollos)
    {
        if (rollos == null || rollos.Count == 0) return;
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using var transaction = conn.BeginTransaction();
            foreach (var item in rollos)
            {
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "UPDATE rolls_details SET splice = @p3,status = @p4,code_person = @p5 WHERE numero = @p1 and unique_code = @p2",
                    CommandType = CommandType.Text
                };
                comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = item.Numero });
                comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = item.UniqueCode });
                comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.Int) { Value = item.Splice });
                comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = item.Status });
                comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.NVarChar) { Value = item.Code_Person });

                comando.ExecuteNonQuery();

            }
            transaction.Commit();
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al mopdificar los renglones de la orden de corte...error code: " + ex);
        }
    }

    public void Update_Header_Documnet_OC(Orden orden)
    {
        SqlTransaction? transaction = null;
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            transaction = conn.BeginTransaction();
            using SqlCommand comando = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "UPDATE orden_corte SET fecha=@p2,fecha_produccion=@p3,width_1=@p4,lenght_1=@p5,util1_real_width=@p6,util1_real_lenght=@p7,rest1_width=@p8,rest1_lenght=@p9,product_id=@p10,desperdicio=@p11,operador_id=@p12,customer_id=@p13,cortes_largo=@p14,longitud_cortar=@p15,cortes_ancho=@p16,cant_rollos=@p17,sellOrder=@p18,rollid_1=@p19,ubicacion=@p20,ConfigVueltas=@p21 WHERE numero=@p1",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = orden.Numero });
            comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.DateTime) { Value = orden.Fecha });
            comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.DateTime) { Value = orden.Fecha_produccion });
            comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.Decimal) { Value = orden.Width_1 });
            comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.Decimal) { Value = orden.Lenght_1 });
            comando.Parameters.Add(new SqlParameter("@p6", SqlDbType.Float) { Value = orden.Util1_Real_Width });
            comando.Parameters.Add(new SqlParameter("@p7", SqlDbType.Float) { Value = orden.Util1_real_Lenght });
            comando.Parameters.Add(new SqlParameter("@p8", SqlDbType.Float) { Value = orden.Rest1_width });
            comando.Parameters.Add(new SqlParameter("@p9", SqlDbType.Float) { Value = orden.Rest1_lenght });
            comando.Parameters.Add(new SqlParameter("@p10", SqlDbType.NVarChar) { Value = orden.Product_id });
            comando.Parameters.Add(new SqlParameter("@p11", SqlDbType.Bit) { Value = orden.Desperdicio });
            comando.Parameters.Add(new SqlParameter("@p12", SqlDbType.UniqueIdentifier) { Value = orden.Operador_id });
            comando.Parameters.Add(new SqlParameter("@p13", SqlDbType.UniqueIdentifier) { Value = orden.Customer_Id });
            comando.Parameters.Add(new SqlParameter("@p14", SqlDbType.Int) { Value = orden.Cortes_Largo });
            comando.Parameters.Add(new SqlParameter("@p15", SqlDbType.Float) { Value = orden.Longitud_Cortar });
            comando.Parameters.Add(new SqlParameter("@p16", SqlDbType.Int) { Value = orden.Cortes_Ancho });
            comando.Parameters.Add(new SqlParameter("@p17", SqlDbType.Int) { Value = orden.Cantidad_Rollos });
            comando.Parameters.Add(new SqlParameter("@p18", SqlDbType.NVarChar) { Value = orden.SellOrder });
            comando.Parameters.Add(new SqlParameter("@p19", SqlDbType.NVarChar) { Value = orden.Rollid_1 });
            comando.Parameters.Add(new SqlParameter("@p20", SqlDbType.NVarChar) { Value = orden.Ubicacion });
            comando.Parameters.Add(new SqlParameter("@p21", SqlDbType.Bit) { Value = orden.ConfigVueltas });
            comando.ExecuteNonQuery();
            using SqlCommand comando_borrar_cortes = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "DELETE FROM cortes WHERE orden=@p1",
                CommandType = CommandType.Text
            };
            comando_borrar_cortes.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = orden.Numero });
            comando_borrar_cortes.ExecuteNonQuery();

            foreach (var corte in orden.Cortes!)
            {
                using SqlCommand comando_insert_cortes = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "INSERT INTO cortes (num,width,lenght,msi,orden) VALUES(@p1,@p2,@p3,@p4,@p5)",
                    CommandType = CommandType.Text
                };
                comando_insert_cortes.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = corte.Numero });
                comando_insert_cortes.Parameters.Add(new SqlParameter("@p2", SqlDbType.Float) { Value = corte.Width });
                comando_insert_cortes.Parameters.Add(new SqlParameter("@p3", SqlDbType.Float) { Value = corte.Length });
                comando_insert_cortes.Parameters.Add(new SqlParameter("@p4", SqlDbType.Float) { Value = corte.Msi });
                comando_insert_cortes.Parameters.Add(new SqlParameter("@p5", SqlDbType.Float) { Value = corte.Orden });
                comando_insert_cortes.ExecuteNonQuery();
            }
            using SqlCommand comando_borrar_rollos = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "DELETE FROM rolls_details WHERE numero=@p1",
                CommandType = CommandType.Text
            };
            comando_borrar_rollos.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = orden.Numero });
            comando_borrar_rollos.ExecuteNonQuery();

            foreach (var item in orden.rollos!)
            {
                using SqlCommand comando_rolls = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "INSERT INTO rolls_details (numero,roll_number,product_id,product_name,roll_id,width,large,msi,unique_code,splice,code_person,status,ubic,ratio,rollid_oculto,despacho,fecha,fecha_despacho,vuelta) VALUES(@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,'nt',0,'','',GETDATE(),GETDATE(),@p13)",
                    CommandType = CommandType.Text
                };
                comando_rolls.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = item.Numero });
                comando_rolls.Parameters.Add(new SqlParameter("@p2", SqlDbType.Int) { Value = item.RollNumber });
                comando_rolls.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = item.Product_Id });
                comando_rolls.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = item.Product_Name });
                comando_rolls.Parameters.Add(new SqlParameter("@p5", SqlDbType.NVarChar) { Value = item.Roll_Id });
                comando_rolls.Parameters.Add(new SqlParameter("@p6", SqlDbType.Float) { Value = item.Width });
                comando_rolls.Parameters.Add(new SqlParameter("@p7", SqlDbType.Float) { Value = item.Length });
                comando_rolls.Parameters.Add(new SqlParameter("@p8", SqlDbType.Float) { Value = item.Msi });
                comando_rolls.Parameters.Add(new SqlParameter("@p9", SqlDbType.NVarChar) { Value = item.UniqueCode });
                comando_rolls.Parameters.Add(new SqlParameter("@p10", SqlDbType.Int) { Value = item.Splice });
                comando_rolls.Parameters.Add(new SqlParameter("@p11", SqlDbType.NVarChar) { Value = item.Code_Person });
                comando_rolls.Parameters.Add(new SqlParameter("@p12", SqlDbType.NVarChar) { Value = item.Status });
                comando_rolls.Parameters.Add(new SqlParameter("@p13", SqlDbType.Int) { Value = item.Vuelta });
                comando_rolls.ExecuteNonQuery();

            }

            transaction.Commit();
            conn.Close();
        }
        catch (Exception ex)
        {
            try { transaction?.Rollback(); } catch { }
            ServiceErrors.Report("Error al modificar la orden de corte...error code: " + ex);
        }
    }

    public void RollosCortadosDispobnibles(string oc)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "UPDATE rolls_details SET disponible=1 WHERE numero=@p1",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = oc });
            comando.ExecuteNonQuery();
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al modificar la orden de corte...error code: " + ex);
        }
    }
}