using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService;

public class OrdenCorteService : IOrdenCorteService
{
    private readonly string _conn;
    private readonly IConsecutivosService _consecutivos;
    private readonly DataSet _ds = new();
    public string ErrorMsg { get; set; } = null!;

    public OrdenCorteService(IConfiguration config, IConsecutivosService consecutivos)
    {
        _conn = ConexionResolver.Resolver(config);
        _consecutivos = consecutivos;
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
        if (lista == null || lista.Count == 0)
        {
            return;
        }

        GuardarConfigVueltas(lista);
    }

    public void GuardarConfigVueltas(List<ConfigVueltas> lista)
    {
        if (lista == null || lista.Count == 0)
        {
            return;
        }

        using SqlConnection conn = new(_conn);
        conn.Open();
        using SqlTransaction transaction = conn.BeginTransaction();
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

            foreach (ConfigVueltas item in lista)
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

    private string BuildSqlRollIdDisponibles(string columna, string texto, string ocExcluir)
    {
        string sql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID;
        string busq1, busq2;
        switch (columna)
        {
            case "Part_Number": busq1 = "a.Part_Number"; busq2 = "a.product_id"; break;
            case "Product_Name": busq1 = "b.Product_Name"; busq2 = "b.Product_Name"; break;
            default: busq1 = "a.Roll_Id"; busq2 = "a.rollid"; break;
        }
        string textoCond1 = string.IsNullOrWhiteSpace(texto) ? "" : $" AND {busq1} LIKE @text";
        string textoCond2 = string.IsNullOrWhiteSpace(texto) ? "" : $" AND {busq2} LIKE @text";
        string cond1 = $"b.MasterRolls = 1 AND (a.Lenght - ISNULL(ct.largo_consumido,0)) > 100{textoCond1}";
        string cond2 = $"b.MasterRolls = 1 AND (a.length - ISNULL(ct.largo_consumido,0)) > 100{textoCond2}";

        // REGLA: un master con material disponible (restante > 100) SI se puede reusar en otra OC,
        // aunque otra OC activa (no anulada y no cerrada) lo tenga asignado; el restante se calcula
        // restando todo lo ya consumido. Solo se excluye del picker el master que OTRA OC activa
        // tenga asignado SIN consumo previo (aun sin empezar): ese sigue siendo exclusivo de esa OC
        // y evitaria que dos OC compitan por el mismo material completo.
        string noUsadoEnOtraOc1 = " AND (ISNULL(ct.largo_consumido, 0) > 0 OR NOT EXISTS (SELECT 1 FROM orden_corte ocx WHERE ocx.anulada = 0 AND ocx.CloseDocument = 0"
            + (string.IsNullOrWhiteSpace(ocExcluir) ? "" : " AND ocx.numero <> @ocExcluir")
            + " AND (ocx.rollid_1 = a.Roll_Id OR ocx.rollid_2 = a.Roll_Id)))";
        string noUsadoEnOtraOc2 = " AND (ISNULL(ct.largo_consumido, 0) > 0 OR NOT EXISTS (SELECT 1 FROM orden_corte ocx WHERE ocx.anulada = 0 AND ocx.CloseDocument = 0"
            + (string.IsNullOrWhiteSpace(ocExcluir) ? "" : " AND ocx.numero <> @ocExcluir")
            + " AND (ocx.rollid_1 = a.rollid OR ocx.rollid_2 = a.rollid)))";

        sql = sql.Replace("b.MasterRolls = 1 UNION ALL", cond1 + noUsadoEnOtraOc1 + " UNION ALL");
        sql = sql.Replace("b.MasterRolls = 1 ORDER BY Roll_Id", cond2 + noUsadoEnOtraOc2 + " ORDER BY Roll_Id");
        return sql;
    }

    public async Task<DataTable> LoadDataRollID(string ocExcluir = "")
    {
        string sql = BuildSqlRollIdDisponibles("Roll_Id", "", ocExcluir);
        SqlParameter[]? parametros = string.IsNullOrWhiteSpace(ocExcluir)
            ? null
            : new[] { new SqlParameter("@ocExcluir", SqlDbType.NVarChar) { Value = ocExcluir } };
        DataTable? dt = await ProduccionDataAccess.CargarTablaAsync(_conn, sql, false, null, parametros, "DtRollid", true);
        return dt ?? new DataTable("DtRollid");
    }

    public async Task<DataTable> BuscarRollId(string columna, string texto, string ocExcluir = "")
    {
        string sql = BuildSqlRollIdDisponibles(columna, texto, ocExcluir);
        List<SqlParameter> ps = new();
        if (!string.IsNullOrWhiteSpace(texto))
        {
            ps.Add(new SqlParameter("@text", "%" + texto.Trim() + "%"));
        }

        if (!string.IsNullOrWhiteSpace(ocExcluir))
        {
            ps.Add(new SqlParameter("@ocExcluir", SqlDbType.NVarChar) { Value = ocExcluir });
        }

        SqlParameter[]? parametros = ps.Count == 0 ? null : ps.ToArray();
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
            object estado = check.ExecuteScalar();

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

            // REGLA A + REGLA B + REGLA C en UNA sola transaccion:
            // Anular la OC (anulada=1), anular sus rollos cortados hijos (disponible=0) y
            // revertir el consumo fisico registrado contra sus masters para liberarlos.
            using SqlTransaction transaction = conn.BeginTransaction();
            try
            {
                using (SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "UPDATE orden_corte SET anulada=1 WHERE numero=@oc",
                    CommandType = CommandType.Text
                })
                {
                    comando.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = numeroc });
                    comando.ExecuteNonQuery();
                }

                // REGLA C: los rollos cortados de la OC (hijos del master) dejan de estar disponibles.
                using (SqlCommand anularRollos = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "UPDATE rolls_details SET disponible=0 WHERE numero=@oc",
                    CommandType = CommandType.Text
                })
                {
                    anularRollos.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = numeroc });
                    anularRollos.ExecuteNonQuery();
                }

                // REGLA B: revertir el consumo fisico de cada master de la OC para liberarlo.
                string rollid1 = string.Empty, rollid2 = string.Empty;
                using (SqlCommand leerMasters = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "SELECT rollid_1, rollid_2 FROM orden_corte WHERE numero=@oc",
                    CommandType = CommandType.Text
                })
                {
                    leerMasters.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = numeroc });
                    using SqlDataReader reader = leerMasters.ExecuteReader();
                    if (reader.Read())
                    {
                        rollid1 = reader.IsDBNull(0) ? string.Empty : reader.GetString(0);
                        rollid2 = reader.IsDBNull(1) ? string.Empty : reader.GetString(1);
                    }
                }

                if (!string.IsNullOrWhiteSpace(rollid1) && rollid1 != "0")
                {
                    RevertirConsumoMaster(conn, transaction, numeroc, rollid1);
                }

                if (!string.IsNullOrWhiteSpace(rollid2) && rollid2 != "0")
                {
                    RevertirConsumoMaster(conn, transaction, numeroc, rollid2);
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try { transaction.Rollback(); } catch { }
                ServiceErrors.Report("error al anular la orden de corte " + ex.Message);
                return false;
            }
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("error al anular la orden de corte " + ex.Message);
            return false;
        }
    }

    // REGLA B: libera el master devolviendole el largo consumido por esta OC y borrando el
    // detalle de consumo. Se ejecuta el UPDATE en AMBAS tablas fisicas (MasterInic / ItemsMateria);
    // solo la tabla correcta hara match (0 filas en la otra, sin error).
    private static void RevertirConsumoMaster(SqlConnection conn, SqlTransaction transaction, string oc, string rollid)
    {
        double total = 0;
        using (SqlCommand sumar = new()
        {
            Connection = conn,
            Transaction = transaction,
            CommandText = "SELECT ISNULL(SUM(consumo),0) FROM MasterDetailsInic WHERE rollid=@rollid AND orden=@oc",
            CommandType = CommandType.Text
        })
        {
            sumar.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            sumar.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = oc });
            object resultado = sumar.ExecuteScalar();
            if (resultado != null && resultado != DBNull.Value)
            {
                total = Convert.ToDouble(resultado);
            }
        }

        if (total > 0)
        {
            using (SqlCommand revertir = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "UPDATE MasterInic SET largo_consumido = largo_consumido - @total WHERE Roll_Id=@rollid",
                CommandType = CommandType.Text
            })
            {
                revertir.Parameters.Add(new SqlParameter("@total", SqlDbType.Float) { Value = total });
                revertir.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
                revertir.ExecuteNonQuery();
            }

            using (SqlCommand revertir = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "UPDATE ItemsMateria SET largo_consumido = largo_consumido - @total WHERE rollid=@rollid",
                CommandType = CommandType.Text
            })
            {
                revertir.Parameters.Add(new SqlParameter("@total", SqlDbType.Float) { Value = total });
                revertir.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
                revertir.ExecuteNonQuery();
            }
        }

        using (SqlCommand limpiar = new()
        {
            Connection = conn,
            Transaction = transaction,
            CommandText = "DELETE FROM MasterDetailsInic WHERE rollid=@rollid AND orden=@oc",
            CommandType = CommandType.Text
        })
        {
            limpiar.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            limpiar.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = oc });
            limpiar.ExecuteNonQuery();
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
            DataTable?[] resultados = await Task.WhenAll(
                tablas.Select(t => ProduccionDataAccess.CargarTablaAsync(_conn, t.Sql, false, null, null, t.Nombre, true)));

            for (int i = 0; i < tablas.Length; i++)
            {
                DataTable? dt = resultados[i];
                dt!.TableName = tablas[i].Nombre;
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

            DataRelation relacion = new DataRelation(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS,
                            _ds.Tables["DtMaster"]!.Columns["numero"]!,
                            _ds.Tables["DtRollos"]!.Columns["numero"]!, false);

            if (!_ds.Relations.Contains(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS))
            {
                _ds.Relations.Add(relacion);
            }

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
        using SqlTransaction transaction = conn.BeginTransaction();
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
        comando.Parameters.Add(new SqlParameter("@p50", SqlDbType.Float) { Value = OrdenCorte.Lenght_entrada });
        comando.Parameters.Add(new SqlParameter("@p51", SqlDbType.Float) { Value = OrdenCorte.Total_Inch_Ancho });
        comando.Parameters.Add(new SqlParameter("@p52", SqlDbType.Float) { Value = OrdenCorte.Lenght_entrada });
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

        // Cruce master <-> OC: cuando se guarda una OC se deja en el master el numero de
        // esa OC (cubre MasterInic y ItemsMateria) para cruzar inventario de masters con la OC.
        ActualizarDocumentoMaster(OrdenCorte.Numero, OrdenCorte.Rollid_1, conn, transaction);
        ActualizarDocumentoMaster(OrdenCorte.Numero, OrdenCorte.Rollid_2, conn, transaction);
    }

    // Cruce master <-> OC: actualiza el numero de la OC en el master usado (MasterInic e ItemsMateria).
    private static void ActualizarDocumentoMaster(int numeroOc, string? rollid, SqlConnection conn, SqlTransaction transaction)
    {
        if (string.IsNullOrWhiteSpace(rollid))
        {
            return;
        }

        using SqlCommand cmd = new(
            "UPDATE MasterInic SET documento = @num WHERE Roll_Id = @rollid", conn, transaction);
        cmd.Parameters.Add(new SqlParameter("@num", SqlDbType.Int) { Value = numeroOc });
        cmd.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
        cmd.ExecuteNonQuery();

        cmd.CommandText = "UPDATE ItemsMateria SET documento = @num WHERE rollid = @rollid";
        cmd.ExecuteNonQuery();
    }

    public bool GuardarCortes(List<Corte> cortes)
    {
        if (cortes == null || cortes.Count == 0)
        {
            return true;
        }

        using SqlConnection conn = new(_conn);
        conn.Open();
        using SqlTransaction transaction = conn.BeginTransaction();
        try
        {
            foreach (Corte corte in cortes)
            {
                GuardarCorteCore(corte, conn, transaction);
            }

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
        if (rollos == null || rollos.Count == 0)
        {
            return true;
        }

        using SqlConnection conn = new(_conn);
        conn.Open();
        using SqlTransaction transaction = conn.BeginTransaction();
        try
        {
            foreach (RolloCortado roll in rollos)
            {
                GuardarRolloCore(roll, conn, transaction);
            }

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
        if (!ValidarDatosObligatoriosOC(orden, out string motivo))
        {
            ErrorMsg = motivo;
            ServiceErrors.Report(motivo);
            return false;
        }
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
        // REGLA DE PRODUCCION RN-ROLLOS-COUNT: el total de rollos a cortar (Cantidad_Rollos + Cantidad_Rollos2)
        // debe coincidir con el numero de filas del detalle de rollos cortados antes de guardar.
        int rollosEsperados = orden.Cantidad_Rollos + orden.Cantidad_Rollos2;
        if (rollosEsperados > 0 && rollos.Count != rollosEsperados)
        {
            ServiceErrors.Report($"No se puede guardar la orden de corte: se indicaron {rollosEsperados} rollos a cortar pero el detalle tiene {rollos.Count} rollos cortados.");
            return false;
        }

        // RN-DEF-WIDTH-LENGTH-MSI: validar que la definicion de cortes (width, leng, msi)
        // sea consistente con los rollos cortados detalle. Los anchos, longitudes y MSI
        // definidos en los cortes no deben diferir de los registrados en los rollos cortados.
        if (!ValidarConsistenciaCorteLYM(orden, cortes!, rollos!))
        {
            return false;
        }

        // REGLA DE PRODUCCION RN-RESTANTE-OC: recalcullar y forzar restante = largo master - consumo.
        AplicarReglaRestanteOC(orden);

        using SqlConnection conn = new(_conn);
        conn.Open();
        using SqlTransaction transaction = conn.BeginTransaction();
        try
        {
            if (orden.Numero <= 0)
            {
                int numero = _consecutivos.GetAndIncrementConsecOCTransactional(conn, transaction);
                orden.Numero = numero;
                foreach (Corte corte in cortes)
                {
                    corte.Orden = numero;
                }

                foreach (RolloCortado roll in rollos)
                {
                    roll.Numero = numero.ToString();
                }
            }
            GuardarEncabezadoOrdenCorteCore(orden, conn, transaction);
            foreach (Corte corte in cortes)
            {
                GuardarCorteCore(corte, conn, transaction);
            }

            foreach (RolloCortado roll in rollos)
            {
                GuardarRolloCore(roll, conn, transaction);
            }

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

    // REGLA DE PRODUCCION RN-OC-REQUERIDOS: la OC no se puede grabar sin
    // customer, operador, ubicacion y sell order asignados (backstop del servicio;
    // la UI valida los mismos campos en ValidarDocumento antes de guardar).
    private static bool ValidarDatosObligatoriosOC(Orden orden, out string motivo)
    {
        if (orden.Customer_Id == Guid.Empty)
        {
            motivo = "REGLA DE PRODUCCION: la orden de corte debe tener un cliente asignado.";
            return false;
        }
        if (orden.Operador_id == Guid.Empty)
        {
            motivo = "REGLA DE PRODUCCION: la orden de corte debe tener un operador asignado.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(orden.SellOrder) || orden.SellOrder == "0")
        {
            motivo = "REGLA DE PRODUCCION: la orden de corte debe tener un sell order asignado.";
            return false;
        }
        if (string.IsNullOrWhiteSpace(orden.Ubicacion))
        {
            motivo = "REGLA DE PRODUCCION: la orden de corte debe tener una ubicacion asignada.";
            return false;
        }
        // REGLA DE PRODUCCION: la fecha de produccion no puede ser menor que la
        // fecha de la orden de corte (registro/emision).
        if (orden.Fecha_produccion.Date < orden.Fecha.Date)
        {
            motivo = "REGLA DE PRODUCCION: la fecha de produccion no puede ser menor que la fecha de la orden de corte.";
            return false;
        }
        motivo = "";
        return true;
    }

    // REGLA DE PRODUCCION RN-RESTANTE-OC: al guardar (crear o editar) el restante archivado de
    // cada master debe ser length del master - consumo de la OC, jamas 0 (ni un valor ajeno)
    // si el master tiene material. Como el restante es un campo derivado, la regla lo recalcula
    // y fuerza el valor correcto en el objeto antes de persistir. Caso real: OC 4625, master
    // 243058320006, length 20115.00 y consumo 20000.00 -> al editar se archivaba rest1_lenght=0
    // y restante_rollid1='335,00' (valor del master anterior) en vez de 115.00 / '115,00'.
    // RN-RESTA-WIDTH: tambien calcula el restante de ancho (width) del master, almacenado
    // en Rest1_width/rest2_width fields (utilizados por la UI y reportes), mientras que
    // Restante_rollid1/2 mantienen el formato de length restante para compatibilidad.
    public void AplicarReglaRestanteOC(Orden orden)
    {
        // Calculate remaining length (original behavior, preserved)
        orden.Rest1_lenght = CalculosOrdenCorte.RestanteMaster(Convert.ToDouble(orden.Lenght_1), orden.Util1_real_Lenght, orden.Desperdicio);
        orden.Restante_rollid1 = FormatRestante(orden.Rest1_lenght);

        // Calculate remaining width (new): master width - sum of corte widths
        double anchoCortes1 = 0;
        if (orden.Cortes != null && orden.Cortes.Count > 0)
        {
            if (orden.TwoMasters)
            {
                // First half of cortess go to master 1, second half to master 2
                int mitad = orden.Cortes.Count / 2;
                anchoCortes1 = orden.Cortes.Take(mitad).Sum(c => c.Width);
            }
            else
            {
                anchoCortes1 = orden.Cortes.Sum(c => c.Width);
            }
        }
        orden.Rest1_width = Math.Round(Convert.ToDouble(orden.Width_1) - anchoCortes1, 4);

        if (orden.TwoMasters || !string.IsNullOrWhiteSpace(orden.Rollid_2))
        {
            // Calculate remaining length for master 2 (original behavior)
            orden.Rest2_lenght = CalculosOrdenCorte.RestanteMaster(Convert.ToDouble(orden.Lenght_2), orden.Util2_real_Lenght, orden.Desperdicio2);
            orden.Restante_rollid2 = FormatRestante(orden.Rest2_lenght);

            // Calculate remaining width for master 2 (new)
            double anchoCortes2 = 0;
            if (orden.Cortes != null && orden.Cortes.Count > 0 && orden.TwoMasters)
            {
                int mitad = orden.Cortes.Count / 2;
                anchoCortes2 = orden.Cortes.Skip(mitad).Take(orden.Cortes.Count - mitad).Sum(c => c.Width);
            }
            orden.Rest2_width = Math.Round(Convert.ToDouble(orden.Width_2) - anchoCortes2, 4);
        }
        else
        {
            orden.Rest2_width = 0;
        }
    }

    /// <summary>
    /// VALIDACION RN-DEF-WIDTH-LENGTH-MSI: la definicion de los cortes (width, leng, msi)
    /// no debe ser distinta a la registrada en los rollos cortados detalle de la orden.
    /// Cada rollo cortado debe tener dimensiones compatibles con algun corte definido,
    /// y la suma de anchos de los cortes no debe exceder el ancho del master.
    /// </summary>
    private bool ValidarConsistenciaCorteLYM(Orden orden, List<Corte> cortes, List<RolloCortado> rollos)
    {
        // 1. Determinar si el ancho del master tiene un valor significativo (> 10 plg)
        //    Si es un valor por defecto (0-10), saltar validacion estricta de suma de anchos
        //    para no bloquear guardado con valores de prueba, pero aun validar compatibilidad basica
        bool widthMasterSignificativo = Convert.ToDouble(orden.Width_1) > 10.0;
        bool widthMasterSignificativo2 = orden.TwoMasters || Convert.ToDouble(orden.Width_2) > 10.0;

        // Si ambos masters tienen width por defecto, solo validar compatibilidad basica
        if (!widthMasterSignificativo && !widthMasterSignificativo2)
        {
            return ValidarCompatibilidadRolloCorteBasica(cortes, rollos);
        }

        // 2. Validar que la suma de anchos de los cortes no exceda el ancho del master
        double anchoMaster = Convert.ToDouble(orden.Width_1);
        if (orden.TwoMasters || widthMasterSignificativo2)
        {
            if (Convert.ToDouble(orden.Width_2) > 10.0)
            {
                anchoMaster = Math.Max(anchoMaster, Convert.ToDouble(orden.Width_2));
            }
        }

        double sumaAnchosCortes = cortes.Sum(c => c.Width);
        if (!CalculosOrdenCorte.SumaCortesNoExcedeMaster(sumaAnchosCortes, anchoMaster))
        {
            ServiceErrors.Report($"REGLA DE PRODUCCION: la suma de anchos de los cortes ({sumaAnchosCortes:N2} plg) excede el ancho del master ({anchoMaster:N2} plg).");
            return false;
        }

        // 3. Validar que cada rollo cortado tenga dimensiones consistentes con los cortes
        //    (cada rollo debe poder ser asignado a un corte con width/length/msi compatibles)
        return ValidarRollosCompatiblesConCortes(cortes, rollos);
    }

    /// <summary>
    /// Validacion basica de compatibilidad rollo-corte cuando el width del master es por defecto.
    /// No bloquea el guardado, solo reporta informacion.
    /// </summary>
    private bool ValidarCompatibilidadRolloCorteBasica(List<Corte> cortes, List<RolloCortado> rollos)
    {
        if (cortes == null || cortes.Count == 0)
        {
            if (rollos != null && rollos.Count > 0)
            {
                return true; // advertencia pero no bloquear
            }

            return true;
        }

        if (rollos == null || rollos.Count == 0)
        {
            return true;
        }

        // Validar que cada rollo tenga dimensiones consistentes con algun corte
        foreach (RolloCortado rollo in rollos)
        {
            bool rolloCompatible = false;
            foreach (Corte corte in cortes)
            {
                if (Math.Abs(rollo.Width - corte.Width) <= 0.01 &&
                    Math.Abs(rollo.Length - corte.Length) <= 0.01 &&
                    Math.Abs(rollo.Msi - corte.Msi) <= 0.01)
                {
                    rolloCompatible = true;
                    break;
                }
            }
            if (!rolloCompatible)
            {
                // No bloquear en modo basico, solo reportar
                ServiceErrors.Report($"INFO: el rollo corte '{rollo.UniqueCode}' no tiene corte definido exacto, pero se guarda igual.");
            }
        }
        return true;
    }

    /// <summary>
    /// Validar que cada rollo cortado tenga dimensiones consistentes con los cortes
    /// cuando el master width es significativo.
    /// </summary>
    private bool ValidarRollosCompatiblesConCortes(List<Corte> cortes, List<RolloCortado> rollos)
    {
        if (cortes == null || cortes.Count == 0)
        {
            if (rollos != null && rollos.Count > 0)
            {
                ServiceErrors.Report("REGLA DE PRODUCCION: la orden tiene rollos cortados pero no hay definicion de cortes.");
                return false;
            }
            return true;
        }

        if (rollos == null || rollos.Count == 0)
        {
            ServiceErrors.Report("REGLA DE PRODUCCION: la orden tiene definicion de cortes pero no hay rollos cortados registrados.");
            return false;
        }

        // Validar que cada rollo tenga dimensiones consistentes con algun corte
        foreach (RolloCortado rollo in rollos)
        {
            bool rolloCompatible = false;
            foreach (Corte corte in cortes)
            {
                if (Math.Abs(rollo.Width - corte.Width) <= 0.01 &&
                    Math.Abs(rollo.Length - corte.Length) <= 0.01 &&
                    Math.Abs(rollo.Msi - corte.Msi) <= 0.01)
                {
                    rolloCompatible = true;
                    break;
                }
            }
            if (!rolloCompatible)
            {
                ServiceErrors.Report($"REGLA DE PRODUCCION: el rollo corte '{rollo.UniqueCode}' (width={rollo.Width}, leng={rollo.Length}, msi={rollo.Msi}) no es compatible con ningun corte definido.");
                return false;
            }
        }

        return true;
    }

    private static string FormatRestante(double restante)
        => restante.ToString("0.00", System.Globalization.CultureInfo.InvariantCulture).Replace('.', ',');

    public bool UpdateStatusDocumentOC(int stepchange, string oc)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();

            // REGLA A (backstop): una OC anulada o cerrada no puede recibir mas cambios de estado.
            using (SqlCommand validar = new()
            {
                Connection = conn,
                CommandText = "SELECT TOP 1 anulada, CloseDocument FROM orden_corte WHERE numero=@oc",
                CommandType = CommandType.Text
            })
            {
                validar.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = oc });
                using SqlDataReader reader = validar.ExecuteReader();
                if (!reader.Read())
                {
                    ServiceErrors.Report("La orden de corte " + oc + " no existe.");
                    return false;
                }

                if (!reader.IsDBNull(0) && reader.GetBoolean(0))
                {
                    ServiceErrors.Report("La orden de corte " + oc + " se encuentra anulada, no se pueden realizar más acciones.");
                    return false;
                }

                if (!reader.IsDBNull(1) && reader.GetBoolean(1))
                {
                    ServiceErrors.Report("La orden de corte " + oc + " está cerrada, no se pueden realizar más acciones.");
                    return false;
                }
            }

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
        if (rollos == null || rollos.Count == 0)
        {
            return;
        }

        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using SqlTransaction transaction = conn.BeginTransaction();
            foreach (RolloCortado roll in rollos)
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
                      AND TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code))) BETWEEN @inicio AND @fin
                    UNION ALL
                    SELECT TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code)))
                    FROM RollsInic
                    WHERE unique_code LIKE 'RC%'
                      AND TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code))) BETWEEN @inicio AND @fin",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@numero", SqlDbType.NVarChar) { Value = numeroOc });
            comando.Parameters.Add(new SqlParameter("@inicio", SqlDbType.Int) { Value = inicio });
            comando.Parameters.Add(new SqlParameter("@fin", SqlDbType.Int) { Value = fin });
            using SqlDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                if (!reader.IsDBNull(0))
                {
                    ocupados.Add(reader.GetInt32(0));
                }
            }
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al validar el rango de codigos unicos. Codigo de Error : " + ex.Message);
        }
        return ocupados;
    }

    /// <summary>
    /// Etiqueta los rollos de una OC EN UNA SOLA TRANSACCION y SIN SALTOS en el consecutivo RC.
    /// Ademas de asignar los ROLLID, avanza el step y actualiza el contador, REGISTRA de forma
    /// atomica el CONSUMO de inventario de los master(s) usados. De este modo una OC SIEMPRE
    /// deja registrado su consumo y descuenta su stock al etiquetar (nunca puede quedar
    /// etiquetada sin haber consumido inventario ni reutilizarse el master en otra OC sin
    /// descontar). Si el consumo no se puede registrar, la transaccion completa revierte y la
    /// OC NO queda etiquetada. La escritura de consumo es idempotente por (rollid,orden,desperdicio).
    /// </summary>
    public (int Primero, int Ultimo) GuardarEtiquetado(
        List<RolloCortado> rollos, string numeroOc,
        string rollidMaster1, double consumoMaster1, double desperdicio1, string tipoMaster1,
        string rollidMaster2, double consumoMaster2, double desperdicio2, string tipoMaster2, bool twoMasters)
    {
        if (rollos == null || rollos.Count == 0)
        {
            throw new ArgumentException("No hay rollos para etiquetar.", nameof(rollos));
        }

        using SqlConnection conn = new(_conn);
        conn.Open();
        using SqlTransaction transaction = conn.BeginTransaction();
        try
        {
            // REGLA DE PRODUCCION: un master se puede consumir en 1 o VARIAS OC segun la
            // necesidad, siempre que su restante alcance. Por eso aqui (etiquetar = momento en
            // que se registra el consumo) NO se prohíbe que el master este asignado a otra OC:
            // solo se valida que el consumo a registrar de ESTA OC quepa en el material
            // disponible del master (largo del master − consumo ya registrado por otras OC).
            ValidarConsumoDisponibleMaster(conn, transaction, numeroOc, rollidMaster1, tipoMaster1, consumoMaster1, desperdicio1, "master 1");
            if (twoMasters && !string.IsNullOrWhiteSpace(rollidMaster2) && rollidMaster2 != "0")
            {
                ValidarConsumoDisponibleMaster(conn, transaction, numeroOc, rollidMaster2, tipoMaster2, consumoMaster2, desperdicio2, "master 2");
            }

            // Reserva atomica: avanza el contador 'UC' en una sola UPDATE con OUTPUT y lo
            // alinea primero contra el maximo RC real en BD (rolls_details + RollsInic) para
            // sanear contadores atrasados o adelantados que produciran saltos o colisiones.
            (int primero, int ultimo) = ReservarRangoUniqueCodeConsec(conn, transaction, rollos.Count);

            int numero = primero - 1;
            foreach (RolloCortado roll in rollos)
            {
                numero++;
                roll.UniqueCode = "RC" + numero;
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "UPDATE rolls_details SET unique_code=@p2 WHERE roll_number=@p1 AND numero=@p3",
                    CommandType = CommandType.Text
                };
                comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = roll.RollNumber });
                comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = roll.UniqueCode });
                comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = roll.Numero });
                comando.ExecuteNonQuery();
            }

            // CONSUMO atómico de inventario del/los master al etiquetar. Si falla, TODO revierte
            // y la OC no queda etiquetada (protege que nunca haya OC etiquetada sin consumo).
            if (consumoMaster1 > 0 || desperdicio1 > 0)
            {
                RegistrarConsumoMaster(conn, transaction, numeroOc, rollidMaster1, tipoMaster1, consumoMaster1, desperdicio1);
            }

            if (twoMasters && !string.IsNullOrWhiteSpace(rollidMaster2) && (consumoMaster2 > 0 || desperdicio2 > 0))
            {
                RegistrarConsumoMaster(conn, transaction, numeroOc, rollidMaster2, tipoMaster2, consumoMaster2, desperdicio2);
            }

            using (SqlCommand comando = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "UPDATE orden_corte SET step=3 WHERE numero=@p1 AND step < 3",
                CommandType = CommandType.Text
            })
            {
                comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = numeroOc });
                comando.ExecuteNonQuery();
            }

            transaction.Commit();
            return (primero, ultimo);
        }
        catch (Exception ex)
        {
            try { transaction.Rollback(); } catch { }
            ServiceErrors.Report("Error al etiquetar la orden de corte. Codigo de Error : " + ex.Message);
            throw;
        }
    }

    // REGLA DE PRODUCCION (negocio): un master se puede consumir entre 1 o VARIAS OC segun
    // la necesidad. Al etiquetar (cuando se registra el consumo) se valida que el consumo de
    // ESTA OC no exceda el material disponible real del master: largo del master − consumo ya
    // registrado por otras OC (no anuladas). Si sobra, se permite; si no alcanza, se aborta.
    private static void ValidarConsumoDisponibleMaster(SqlConnection conn, SqlTransaction tran, string numeroOc, string rollid, string tipoMaster, double consumoNuevo, double desperdicioNuevo, string nombreMaster)
    {
        if (string.IsNullOrWhiteSpace(rollid) || rollid == "0")
        {
            return;
        }

        double nuevoConsumo = consumoNuevo + desperdicioNuevo;
        if (nuevoConsumo <= 0)
        {
            return;
        }

        bool esPorCompras = tipoMaster?.Trim() == "Por Compras";
        string tabla = esPorCompras ? "ItemsMateria" : "MasterInic";
        string colLargo = esPorCompras ? "length" : "lenght";
        string colRollid = esPorCompras ? "rollid" : "Roll_Id";

        double largoMaster = 0;
        using (SqlCommand cmdLargo = new($"SELECT {colLargo} FROM {tabla} WHERE {colRollid} = @rollid", conn, tran))
        {
            cmdLargo.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            object largo = cmdLargo.ExecuteScalar();
            if (largo == null || largo == DBNull.Value)
            {
                return;
            }

            largoMaster = Convert.ToDouble(largo);
        }

        double consumidoOtrasOC = 0;
        string sqlConsumido = @"
SELECT ISNULL(SUM(x.ConsumoTotal),0)
FROM orden_corte oc
CROSS APPLY (VALUES
    (oc.util1_real_lenght + CASE WHEN oc.desperdicio = 1 THEN (oc.lenght_1 - oc.util1_real_lenght) ELSE 0 END, oc.rollid_1, oc.anulada, oc.numero),
    (oc.util2_real_lenght + CASE WHEN oc.desperdicio2 = 1 THEN (oc.lenght_2 - oc.util2_real_lenght) ELSE 0 END, oc.rollid_2, oc.anulada, oc.numero)
) AS x(ConsumoTotal, rollidConsumido, anulada, numero)
WHERE x.rollidConsumido = @rollid AND x.anulada = 0 AND x.numero <> @oc";
        using (SqlCommand cmdConsumido = new(sqlConsumido, conn, tran))
        {
            cmdConsumido.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            cmdConsumido.Parameters.Add(new SqlParameter("@oc", SqlDbType.NVarChar) { Value = numeroOc });
            object suma = cmdConsumido.ExecuteScalar();
            if (suma != null && suma != DBNull.Value)
            {
                consumidoOtrasOC = Convert.ToDouble(suma);
            }
        }

        double disponible = largoMaster - consumidoOtrasOC;
        if (nuevoConsumo > disponible + 0.01)
        {
            throw new InvalidOperationException(
                $"El master {rollid} ({nombreMaster}) tiene {disponible:N2} pies disponibles, pero esta OC necesita {nuevoConsumo:N2} pies para etiquetar. " +
                "Un master puede consumirse en varias OC, pero su consumo total no puede superar su largo.");
        }
    }

    // Compromiso de material de un master (RN-CONSUMO-SUM): por cada OC no anulada que lo tiene
    // montado (master 1 o 2) cuenta el MAYOR entre su consumo PLANIFICADO (longitud_cortar x
    // cortes_largo / longitud_cortar2 x vueltas2) y su consumo REAL registrado (util*_real_lenght
    // + desperdicio), para nunca subestimar un compromiso. Devuelve el total comprometido por las
    // OC "de otras ordenes" (excluyendo @oc cuando no es NULL). Usada por la validacion de edicion
    // (dentro de la transaccion) y por el formulario (al crear/editar) via ConsumoComprometidoOtrosOC.
    private static double ConsumoComprometidoOC(SqlConnection conn, SqlTransaction? tran, string rollid, int? ocExcluir)
    {
        const string sql = @"
SELECT ISNULL(SUM(x.Comprometido), 0)
FROM orden_corte oc
CROSS APPLY (VALUES
    (CASE WHEN oc.rollid_1 = @rollid THEN
        (CASE WHEN ISNULL(oc.longitud_cortar, 0) * oc.cortes_largo >= (oc.util1_real_lenght + CASE WHEN oc.desperdicio = 1 THEN (oc.lenght_1 - oc.util1_real_lenght) ELSE 0 END)
             THEN ISNULL(oc.longitud_cortar, 0) * oc.cortes_largo
             ELSE (oc.util1_real_lenght + CASE WHEN oc.desperdicio = 1 THEN (oc.lenght_1 - oc.util1_real_lenght) ELSE 0 END) END)
     ELSE 0 END),
    (CASE WHEN oc.rollid_2 = @rollid THEN
        (CASE WHEN ISNULL(oc.longitud_cortar2, 0) * oc.vueltas2 >= (oc.util2_real_lenght + CASE WHEN oc.desperdicio2 = 1 THEN (oc.lenght_2 - oc.util2_real_lenght) ELSE 0 END)
             THEN ISNULL(oc.longitud_cortar2, 0) * oc.vueltas2
             ELSE (oc.util2_real_lenght + CASE WHEN oc.desperdicio2 = 1 THEN (oc.lenght_2 - oc.util2_real_lenght) ELSE 0 END) END)
     ELSE 0 END)
) AS x(Comprometido)
WHERE oc.anulada = 0 AND (oc.rollid_1 = @rollid OR oc.rollid_2 = @rollid)
  AND (@oc IS NULL OR oc.numero <> @oc)";
        using SqlCommand cmd = new(sql, conn, tran)
        {
            Parameters =
            {
                new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid },
                new SqlParameter("@oc", SqlDbType.Int) { Value = (object?)ocExcluir ?? DBNull.Value }
            }
        };
        object suma = cmd.ExecuteScalar();
        return suma != null && suma != DBNull.Value ? Convert.ToDouble(suma) : 0;
    }

    /// <summary>
    /// Total de pies del rollid ya comprometido por otras ordenes de corte no anuladas
    /// (cada OC aporta el mayor entre su consumo planificado y el real). El formulario lo usa
    /// para validar al montar el master (crear) y al editar la OC; si al OC nueva/actualizada le falta
    /// material disponible se bloquea el guardado (RN-CONSUMO-SUM).
    /// </summary>
    public double ConsumoComprometidoOtrosOC(string rollid, int? ocExcluir)
    {
        if (string.IsNullOrWhiteSpace(rollid) || rollid == "0")
        {
            return 0;
        }

        using SqlConnection conn = new(_conn);
        conn.Open();
        return ConsumoComprometidoOC(conn, null, rollid, ocExcluir);
    }

    /// <summary>
    /// Largo ORIGINAL del master (MasterInic.lenght o ItemsMateria.length, el que
    /// exista). El formulario lo usa para mostrarlo en el mensaje de bloqueo
    /// RN-CONSUMO-SUM, junto al restante y al compromiso de otras OC, para que el
    /// usuario recalcule longitud/vueltas. Devuelve 0 si no lo encuentra.
    /// </summary>
    public double ObtenerLargoOriginalMaster(string rollid)
    {
        if (string.IsNullOrWhiteSpace(rollid) || rollid == "0")
        {
            return 0;
        }

        using SqlConnection conn = new(_conn);
        conn.Open();
        using SqlCommand cmd = new(
            "SELECT COALESCE((SELECT TOP 1 lenght FROM MasterInic WHERE Roll_Id = @rollid), " +
            "(SELECT TOP 1 length FROM ItemsMateria WHERE rollid = @rollid), 0)", conn);
        cmd.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
        object? largo = cmd.ExecuteScalar();
        return largo != null && largo != DBNull.Value ? Convert.ToDouble(largo) : 0;
    }

    // REGLA DE PRODUCCION (negocio) RN-CONSUMO-SUM: montar DOS O MÁS ordenes de corte sobre el
    // MISMO master (rollid) y que la SUMA de sus consumos supere el lenght del master es un ERROR
    // que deja el inventario del master en NEGATIVO y esta PROHIBIDO.
    // Se valida al EDITAR: la OC en edicion compromete el mayor entre su NUEVO consumo
    // (longitud_cortar x cortes_largo) y lo que ya consumio (util1_real_lenght); se suma al
    // compromiso de las otras OC (planificado o real, el mayor). Si el total excede el lenght
    // del master se lanza InvalidOperationException: la transaccion revierte, el formulario
    // muestra el mensaje y NO guarda hasta reducir longitud a cortar/vueltas o cambiar de master.
    private static void ValidarConsumoTotalizadoMaster(SqlConnection conn, SqlTransaction tran, int numeroOc, string rollid, double consumoNuevo, double consumoRealPropio)
    {
        if (string.IsNullOrWhiteSpace(rollid) || rollid == "0")
        {
            return;
        }

        double autoCompromiso = Math.Max(consumoNuevo, consumoRealPropio);
        if (autoCompromiso <= 0)
        {
            return;
        }

        double largoMaster = 0;
        using (SqlCommand cmdLargo = new(
            "SELECT COALESCE((SELECT TOP 1 lenght FROM MasterInic WHERE Roll_Id = @rollid), " +
            "(SELECT TOP 1 length FROM ItemsMateria WHERE rollid = @rollid), 0)", conn, tran))
        {
            cmdLargo.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            object largo = cmdLargo.ExecuteScalar();
            if (largo == null || largo == DBNull.Value)
            {
                return;
            }

            largoMaster = Convert.ToDouble(largo);
        }

        if (largoMaster <= 0)
        {
            return;
        }

        double comprometidoOtrasOC = ConsumoComprometidoOC(conn, tran, rollid, numeroOc);

        double disponible = largoMaster - comprometidoOtrasOC;
        if (autoCompromiso > disponible + 0.01)
        {
            throw new InvalidOperationException(
                $"El master {rollid} tiene un largo de {largoMaster:N2} pies y {comprometidoOtrasOC:N2} pies ya comprometidos por otras ordenes de corte, " +
                $"por lo que quedan {disponible:N2} pies disponibles.\n\n" +
                $"Esta orden compromete {autoCompromiso:N2} pies y EXCEDE el material disponible del master.\n\n" +
                "Dos o mas ordenes montadas sobre el mismo master no pueden superar el largo del master: ajuste la longitud a cortar y/o las vueltas, " +
                "o seleccione otro master.");
        }
    }

    private static void RegistrarConsumoMaster(SqlConnection conn, SqlTransaction tran, string numeroOc,
        string rollid, string tipoMaster, double consumoReal, double desperdicio)
    {
        if (string.IsNullOrWhiteSpace(rollid))
        {
            return;
        }

        string sqlInv = tipoMaster.Trim().ToUpperInvariant() == "INIC."
            ? R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_INICIALES
            : R.QUERY.PRODUCTION.SQL_QUERY_ACTUALIZAR_INVENTARIO_MATERIA;

        if (consumoReal > 0)
        {
            RegistrarConsumo(conn, tran, numeroOc, rollid, sqlInv, consumoReal, false);
        }

        if (desperdicio > 0)
        {
            RegistrarConsumo(conn, tran, numeroOc, rollid, sqlInv, desperdicio, true);
        }
    }

    /// <summary>
    /// Registra un consumo del master en el detalle (MasterDetailsInic) y en el inventario
    /// (largo_consumido). Idempotente por (rollid,orden,desperdicio): si el detalle ya existe
    /// (corrida previa o doble intento) no vuelve a descontar stock ni duplica el detalle.
    /// </summary>
    private static void RegistrarConsumo(SqlConnection conn, SqlTransaction tran, string numeroOc,
        string rollid, string sqlInv, double consumo, bool esDesperdicio)
    {
        // No descontar dos veces el mismo (rollid, orden, desperdicio) si una corrida previa
        // ya dejo el detalle en MasterDetailsInic.
        using (SqlCommand comando = new SqlCommand(R.QUERY.PRODUCTION.SQL_QUERY_CONSUMO_OC_DETALLE_EXISTE, conn, tran))
        {
            comando.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            comando.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = numeroOc });
            comando.Parameters.Add(new SqlParameter("@desperdicio", SqlDbType.Bit) { Value = esDesperdicio });
            if (comando.ExecuteScalar() != null)
            {
                return;
            }
        }

        using (SqlCommand comando = new SqlCommand(R.QUERY.PRODUCTION.UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES, conn, tran))
        {
            comando.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            comando.Parameters.Add(new SqlParameter("@orden", SqlDbType.NVarChar) { Value = numeroOc });
            comando.Parameters.Add(new SqlParameter("@consumo", SqlDbType.Float) { Value = consumo });
            comando.Parameters.Add(new SqlParameter("@fecha", SqlDbType.DateTime) { Value = DateTime.Now });
            comando.Parameters.Add(new SqlParameter("@desperdicio", SqlDbType.Bit) { Value = esDesperdicio });
            comando.ExecuteNonQuery();
        }

        using (SqlCommand comando = new SqlCommand(sqlInv, conn, tran))
        {
            comando.Parameters.Add(new SqlParameter("@consumo", SqlDbType.Float) { Value = consumo });
            comando.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar) { Value = rollid });
            comando.ExecuteNonQuery();
        }
    }

    private static (int Primero, int Ultimo) ReservarRangoUniqueCodeConsec(SqlConnection conn, SqlTransaction transaction, int cantidad)
    {
        const string sql = @"
DECLARE @n int = @cantidad;
DECLARE @vmax int = ISNULL((SELECT MAX(uc) FROM (
    SELECT TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code))) AS uc
    FROM rolls_details
    WHERE unique_code LIKE 'RC%'
    UNION ALL
    SELECT TRY_CONVERT(int, SUBSTRING(unique_code, 3, LEN(unique_code)))
    FROM RollsInic
    WHERE unique_code LIKE 'RC%'
) t WHERE uc IS NOT NULL), 0);
UPDATE control SET par1 = CASE WHEN ISNULL(par1, 0) < @vmax THEN @vmax ELSE ISNULL(par1, 0) END WHERE filter='UC';
UPDATE control SET par1 = par1 + @n OUTPUT DELETED.par1 + 1 AS Primero, DELETED.par1 + @n AS Ultimo WHERE filter='UC';";

        using SqlCommand comando = new(sql, conn, transaction);
        comando.Parameters.Add(new SqlParameter("@cantidad", SqlDbType.Int) { Value = cantidad });
        using SqlDataReader reader = comando.ExecuteReader();
        if (!reader.Read() || reader.IsDBNull(0))
        {
            throw new InvalidOperationException("No existe la fila 'UC' en la tabla control: no se pudo reservar el consecutivo de codigos unicos.");
        }

        int primero = reader.GetInt32(0);
        int ultimo = reader.GetInt32(1);
        return (primero, ultimo);
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

            int result = (int)comando.ExecuteScalar();
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
        if (!ValidarDatosObligatoriosOC(orden, out string motivoRequeridos))
        {
            ErrorMsg = motivoRequeridos;
            ServiceErrors.Report(motivoRequeridos);
            return false;
        }
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
        if (rollos == null || rollos.Count == 0)
        {
            return;
        }

        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using SqlTransaction transaction = conn.BeginTransaction();
            foreach (RolloCortado item in rollos)
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
        // REGLA DE PRODUCCION RN-OC-REQUERIDOS: no se puede grabar la OC sin
        // customer/operador/ubicacion/sell order. Se lanza antes de abrir la
        // transaccion para no tocar nada en BD.
        if (!ValidarDatosObligatoriosOC(orden, out string motivoRequeridos))
        {
            throw new InvalidOperationException(motivoRequeridos);
        }
        SqlTransaction? transaction = null;
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            transaction = conn.BeginTransaction();

            // RN-CONSUMO-SUM: el nuevo consumo (longitud a cortar x vueltas) de esta OC no deberia
            // agravar el compromiso del master: solo se bloquea si supera lo que esta OC ya consumo
            // (util1_real_lenght) Y no cabe en el disponible real (largo master − consumo real de
            // otras OC anulada=0). Con esto no se rompe el flujo de produccion existente.
            double consumoNuevoMaster1 = CalculosOrdenCorte.LongitudTotal(orden.Longitud_Cortar, orden.Cortes_Largo);
            ValidarConsumoTotalizadoMaster(conn, transaction, orden.Numero, orden.Rollid_1, consumoNuevoMaster1, orden.Util1_real_Lenght);

            // REGLA DE PRODUCCION RN-RESTANTE-OC: recalcular y forzar restante = largo master - consumo,
            // y persistirlo en la OC (rest1_lenght + restante_rollid1/2). Antes el edit no asignaba
            // Rest1_lenght (quedaba 0) ni persistia restante_rollid1 (quedaba el valor del master anterior).
            AplicarReglaRestanteOC(orden);

            using SqlCommand comando = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "UPDATE orden_corte SET fecha=@p2,fecha_produccion=@p3,width_1=@p4,lenght_1=@p5,util1_real_width=@p6,util1_real_lenght=@p7,rest1_width=@p8,rest1_lenght=@p9,product_id=@p10,desperdicio=@p11,operador_id=@p12,customer_id=@p13,cortes_largo=@p14,longitud_cortar=@p15,cortes_ancho=@p16,cant_rollos=@p17,sellOrder=@p18,rollid_1=@p19,ubicacion=@p20,ConfigVueltas=@p21,tot_inch_ancho=@p22,total_salida=@p23,restante_rollid1=@p24,restante_rollid2=@p25 WHERE numero=@p1",
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
            comando.Parameters.Add(new SqlParameter("@p22", SqlDbType.Float) { Value = orden.Total_Inch_Ancho });
            comando.Parameters.Add(new SqlParameter("@p23", SqlDbType.Float) { Value = orden.Total_Inch_Ancho });
            comando.Parameters.Add(new SqlParameter("@p24", SqlDbType.NVarChar) { Value = orden.Restante_rollid1 ?? "0.00" });
            comando.Parameters.Add(new SqlParameter("@p25", SqlDbType.NVarChar) { Value = orden.Restante_rollid2 ?? "0.00" });
            comando.ExecuteNonQuery();
            ActualizarDocumentoMaster(orden.Numero, orden.Rollid_1, conn, transaction);
            using SqlCommand comando_borrar_cortes = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandText = "DELETE FROM cortes WHERE orden=@p1",
                CommandType = CommandType.Text
            };
            comando_borrar_cortes.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = orden.Numero });
            comando_borrar_cortes.ExecuteNonQuery();

            foreach (Corte corte in orden.Cortes!)
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

            foreach (RolloCortado item in orden.rollos!)
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
            if (ex is InvalidOperationException)
            {
                throw;
            }

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
