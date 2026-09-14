using System.Data;
using System.Threading.Tasks;
using System.Drawing.Printing;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Microsoft.Reporting.WinForms;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ReportsService.ReportsService
{
    public class ReportsService : IReportsService
    {
        public IConfiguration Config { get; }
        public string StringConnex { get; set; } = "";
        public string MessageError { get; set; } = "";

        public ReportsService(IConfiguration Config)
        {
            this.Config = Config;
            if (Config != null)
            {
                var ambiente = Config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
                StringConnex = Config.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]!;
            }
        }

        public void Reporte_InventarioMaster(Form form, string Report_Title, string Report_Name)
        {
            DataTable dt = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID
                    };
                    conn.Open();
                    SqlDataAdapter da = new(comando);
                    da.Fill(dt);
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al cargar el reporte. codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    try
                    {
                        ReportsViewer report = new()
                        {
                            Text = Report_Title,
                            Width = 1000,
                            Height = 800,
                            MdiParent = form.MdiParent,
                            StartPosition = FormStartPosition.CenterScreen

                        };
                        report.reportViewer1.ProcessingMode = ProcessingMode.Local;
                        report.reportViewer1.LocalReport.ReportPath = GetPathApplication(Report_Name, R.PATH_REPORTS.REPORTS_INVENTARIOS);
                        PageSettings pageSettings = new()
                        {
                            PaperSize = new PaperSize("Carta", 850, 1100), //A4-carta.
                            Landscape = false,
                            Margins = new Margins(0, 0, 0, 0),
                        };
                        report.reportViewer1.SetPageSettings(pageSettings);
                        ReportDataSource rds = new("DsInventarios", dt);
                        report.reportViewer1.LocalReport.DataSources.Clear();
                        report.reportViewer1.LocalReport.DataSources.Add(rds);
                        report.reportViewer1.RefreshReport();
                        report.Show();
                    }
                    catch (ReportViewerException ex)
                    {
                        ServiceErrors.Report("Error al crear el report view del reporte. codigo error: " + ex.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el reporte. codigo error: " + ex.Message);
            }
        }

        public void Reporte_InventarioRollosCortados(Form form, string Report_Title, string Report_Name)
        {
            DataTable dt = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = R.QUERY.PRODUCTION.SQL_QUERY_LOAD_INVENTARIO_ROLLO_CORTADO
                    };
                    conn.Open();
                    SqlDataAdapter da = new(comando);
                    da.Fill(dt);
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al cargar el reporte. codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    try
                    {
                        ReportsViewer report = new()
                        {
                            Text = Report_Title,
                            Width = 900,
                            Height = 800,
                            MdiParent = form.MdiParent,
                            StartPosition = FormStartPosition.CenterScreen

                        };
                        report.reportViewer1.ProcessingMode = ProcessingMode.Local;
                        report.reportViewer1.LocalReport.ReportPath = GetPathApplication(Report_Name, R.PATH_REPORTS.REPORTS_INVENTARIOS);
                        PageSettings pageSettings = new()
                        {
                            PaperSize = new PaperSize("Carta", 850, 1100), //A4-carta.
                            Landscape = false,
                            Margins = new Margins(0, 0, 0, 0),
                        };
                        report.reportViewer1.SetPageSettings(pageSettings);
                        ReportDataSource rds = new("DsInventarios", dt);
                        report.reportViewer1.LocalReport.DataSources.Clear();
                        report.reportViewer1.LocalReport.DataSources.Add(rds);
                        report.reportViewer1.RefreshReport();
                        report.Show();
                    }
                    catch (ReportViewerException ex)
                    {
                        ServiceErrors.Report("Error al crear el report view del reporte. codigo error: " + ex.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el reporte. codigo error: " + ex.Message);
            }
        }

        public void Reporte_Orden_MatPrima(string Orden, Form Form, string ReportName, string TitleReport)
        {
            DataTable dt = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "select a.numero,fecha_recepcion,fecha_pro,orden_compra,persona_respons,guia_import,doc_embarque,closeDocument," +
                        "lote,c.proveedor_name,d.transport_name,total_cantidad,b.product_id,e.product_name,b.width,b.length,b.msi,b.rollid,b.splice,b.ubicacion,b.type,b.core,B.cant_pedido,b.cant_real,a.notas from OrdenMateria a left join itemsMateria b on a.numero=b.numero " +
                            "left join provider c on a.proveedor_id = c.proveedor_id LEFT JOIN transporte d ON a.transport_id = d.transport_id LEFT JOIN producto e ON b.product_id = e.product_id where a.numero=@p1"
                    };
                    conn.Open();
                    comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Orden });
                    SqlDataAdapter da = new(comando);
                    da.Fill(dt);
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al cargar el reporte de orden recepcion materia prima. codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    try
                    {
                        ReportsViewer report = new()
                        {
                            Text = TitleReport,
                            Width = 1130,
                            Height = 780,
                            MdiParent = Form.MdiParent,
                            StartPosition = FormStartPosition.CenterScreen

                        };
                        report.reportViewer1.ProcessingMode = ProcessingMode.Local;
                        report.reportViewer1.LocalReport.ReportPath = GetPathApplication(ReportName, R.PATH_REPORTS.REPORTS_DESPACHO);
                        ReportParameter param = new("Orden_MatPrima", Orden);
                        ReportDataSource rds = new("DsImportacion", dt);
                        report.reportViewer1.LocalReport.DataSources.Clear();
                        report.reportViewer1.LocalReport.DataSources.Add(rds);
                        report.reportViewer1.LocalReport.SetParameters(param);
                        report.reportViewer1.RefreshReport();
                        report.Show();
                    }
                    catch (ReportViewerException ex)
                    {
                        ServiceErrors.Report("Error al cargar el reporte de orden recepcion materia prima. codigo error: " + ex.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al generar el reporte de la orden de corte...codigo error: " + ex.Message);
            }
        }
        public void Reporte_Orden_Corte(string numeroOC, Form form, string ReportName, string TitleReport)
        {
            DataTable dt = new();
            DataTable dtcortes = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT a.numero, a.fecha, a.fecha_produccion, a.product_id, c.Product_Name AS producto,b.unique_code, b.roll_number, b.splice, b.width, b.large, b.msi, b.roll_id, b.code_person, b.status, d.nombre AS operador,e.Customer_Name, a.width_1 AS master_width, a.lenght_1 AS master_length, a.longitud_cortar AS master_corte_largo, a.cortes_ancho AS master_cortes_ancho, a.cortes_largo AS master_vueltas,a.cant_rollos AS master_cant_rollos,util1_real_width as master_width_real, util1_real_lenght as master_length_real,(ISNULL(a.width_1,0) - ISNULL(a.util1_real_width,0)) as master_width_restante,(ISNULL(a.lenght_1,0) - ISNULL(a.util1_real_lenght,0)) as master_length_restabnte,step,a.rollid_2, a.width_2, a.lenght_2, a.sellOrder, a.Ubicacion, a.TwoMasters, a.vueltas2, a.longitud_cortar2, a.cantidad_rollos2,util2_real_width,util2_real_lenght,(ISNULL(a.width_2,0) - ISNULL(a.util2_real_width,0)) as rest2_width,(ISNULL(a.lenght_2,0) - ISNULL(a.util2_real_lenght,0)) as rest2_lenght FROM orden_corte AS a LEFT OUTER JOIN rolls_details AS b ON a.numero = b.numero LEFT OUTER JOIN producto AS c ON a.product_id = c.Product_ID LEFT OUTER JOIN operadores AS d ON a.operador_id = d.operador_id LEFT OUTER JOIN Customer AS e ON a.customer_id = e.customer_id WHERE (a.numero = @numero_oc) ORDER BY b.roll_number"
                    };
                    conn.Open();
                    comando.Parameters.Add(new SqlParameter("@numero_oc", SqlDbType.NVarChar) { Value = numeroOC });
                    SqlDataAdapter da = new(comando);
                    da.Fill(dt);
                    SqlCommand comado_cortes = new(StringConnex)
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT num,width,lenght,msi,orden,code_person FROM cortes WHERE orden = @numero_oc"
                    };
                    comado_cortes.Parameters.Add(new SqlParameter("@numero_oc", SqlDbType.NVarChar) { Value = numeroOC });
                    SqlDataAdapter dacortes = new(comado_cortes);
                    dacortes.Fill(dtcortes);
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al generar el reporte de la orden de corte...codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    try
                    {
                        ReportsViewer report = new()
                        {
                            Text = TitleReport,
                            Width = 900,
                            Height = 800,
                            MdiParent = form.MdiParent,
                            StartPosition = FormStartPosition.CenterScreen

                        };
                        report.reportViewer1.ProcessingMode = ProcessingMode.Local;
                        report.reportViewer1.LocalReport.ReportPath = GetPathApplication(ReportName, R.PATH_REPORTS.REPORTS_DESPACHO);
                        PageSettings pageSettings = new()
                        {
                            PaperSize = new PaperSize("Carta", 850, 1100), //A4-carta.
                            Landscape = false,
                            Margins = new Margins(0, 0, 0, 0),
                        };
                        report.reportViewer1.SetPageSettings(pageSettings);
                        ReportParameter param = new("numero_oc", numeroOC);
                        ReportDataSource rds = new("DsOC", dt);
                        ReportDataSource rdsCortes = new("DsCortes", dtcortes);
                        report.reportViewer1.LocalReport.DataSources.Clear();
                        report.reportViewer1.LocalReport.DataSources.Add(rds);
                        report.reportViewer1.LocalReport.DataSources.Add(rdsCortes);
                        report.reportViewer1.LocalReport.SetParameters(param);
                        report.reportViewer1.RefreshReport();
                        report.Show();
                    }
                    catch (ReportViewerException ex)
                    {
                        ServiceErrors.Report("Error al cargar el reporte. codigo error: " + ex.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al generar el reporte de la orden de corte...codigo error: " + ex.Message);
            }
        }
        public void Reporte_Desperdicios(string numeroOC, Form form, string ReportName, string TitleReport)
        {
            DataTable dt = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "SELECT a.numero, a.fecha, a.product_id, c.Product_Name AS product_name, e.Customer_Name AS customer_name, 'Master 1' AS master, a.rollid_1 AS rollid, CAST(a.lenght_1 AS decimal(18,2)) AS largo_original, CAST(a.util1_real_lenght AS decimal(18,2)) AS consumido, CAST(ISNULL(a.lenght_1,0) - ISNULL(a.util1_real_lenght,0) AS decimal(18,2)) AS pies_desperdicio FROM orden_corte a LEFT OUTER JOIN producto c ON a.product_id = c.product_id LEFT OUTER JOIN Customer e ON a.customer_id = e.customer_id WHERE (a.numero = @numero_oc) AND (a.desperdicio = 1)" +
                        " UNION ALL SELECT a.numero, a.fecha, a.product_id, c.Product_Name AS product_name, e.Customer_Name AS customer_name, 'Master 2' AS master, a.rollid_2 AS rollid, CAST(a.lenght_2 AS decimal(18,2)) AS largo_original, CAST(a.util2_real_lenght AS decimal(18,2)) AS consumido, CAST(ISNULL(a.lenght_2,0) - ISNULL(a.util2_real_lenght,0) AS decimal(18,2)) AS pies_desperdicio FROM orden_corte a LEFT OUTER JOIN producto c ON a.product_id = c.product_id LEFT OUTER JOIN Customer e ON a.customer_id = e.customer_id WHERE (a.numero = @numero_oc) AND (a.desperdicio2 = 1)"
                    };
                    conn.Open();
                    comando.Parameters.Add(new SqlParameter("@numero_oc", SqlDbType.NVarChar) { Value = numeroOC });
                    SqlDataAdapter da = new(comando);
                    da.Fill(dt);
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al generar el reporte de desperdicios...codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    try
                    {
                        ReportsViewer report = new()
                        {
                            Text = TitleReport,
                            Width = 900,
                            Height = 800,
                            MdiParent = form.MdiParent,
                            StartPosition = FormStartPosition.CenterScreen

                        };
                        report.reportViewer1.ProcessingMode = ProcessingMode.Local;
                        report.reportViewer1.LocalReport.ReportPath = GetPathApplication(ReportName, R.PATH_REPORTS.REPORTS_DESPACHO);
                        PageSettings pageSettings = new()
                        {
                            PaperSize = new PaperSize("Letter", 850, 1100), //A4-carta.
                            Landscape = false,
                            Margins = new Margins(0, 0, 0, 0),
                        };
                        report.reportViewer1.SetPageSettings(pageSettings);
                        ReportDataSource rds = new("DsDesperdicio", dt);
                        report.reportViewer1.LocalReport.DataSources.Clear();
                        report.reportViewer1.LocalReport.DataSources.Add(rds);
                        report.reportViewer1.RefreshReport();
                        report.Show();
                    }
                    catch (ReportViewerException ex)
                    {
                        ServiceErrors.Report("Error al crear el report view del reporte de desperdicios. codigo error: " + ex.Message);
                    }
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al generar el reporte de desperdicios...codigo error: " + ex.Message);
            }
        }
        public void ReporteConduce_conPrecio(string conduce, Form form, string ReportName, string TitleReport)
        {
            DataSet ds = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandText = "select a.numero,a.fecha,a.customer_id,b.Customer_Name,a.person_contact,a.vendor_id,c.vendor_name,a.packing," +
                          "a.orden_trabajo,a.orden_compra,a.subtotal,a.porc_itbis,a.itbis,a.total$rd as " + "TotalMontoDoc,a.transport_id,a.transporte,a.chofer_id,a.chofer,a.placas_id,a.camion,a.tipo_venta," + "a.reserva,a.impuesto,a.status,a.total_cantidad,a.total_msi,a.total_pie,a.total_kilos,a.total_kilos_netos_palet,a.total_kilos_brutos_palet,d.product_id,e.Product_Name,e.Product_Descrip,e.ratio,e.precio," + "d.cant,d.unid_id,f.UNID_NAME,d.width,d.lenght,d.msi,d.total_pie_lin,d.ratio,d.kilo_rollo,d.kilo_total," +
                          "d.precio,d.total_renglon,d.code_person,d.m2 from despacho a left join Customer b on " + "a.customer_id=b.Customer_ID left join vendedor c on a.vendor_id=c.vendor_id left join item_despacho d on a.numero = d.numero left join producto e on d.product_id = e.Product_ID left join unidad f on f.UNID_ID = d.unid_id where a.numero=@p1",
                        CommandType = CommandType.Text
                    };
                    SqlParameter p1 = new("@p1", SqlDbType.VarChar)
                    {
                        Value = conduce
                    };
                    comando.Parameters.Add(p1);
                    conn.Open();
                    SqlDataAdapter da = new(comando);
                    da.Fill(ds, "dt");
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al cargar el reporte de conduce. codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    ReportsViewer reports = new()
                    {
                        Text = TitleReport,
                        Width = 1050,
                        Height = 780,
                        MdiParent = form.MdiParent,

                    };
                    reports.reportViewer1.ProcessingMode = ProcessingMode.Local;
                    reports.reportViewer1.LocalReport.ReportPath = GetPathApplication(ReportName, R.PATH_REPORTS.REPORTS_DESPACHO);
                    reports.StartPosition = FormStartPosition.CenterParent;
                    PageSettings pageSettings = new()
                    {
                        PaperSize = new PaperSize("Carta", 850, 1100),
                        Landscape = false,
                        Margins = new Margins(100, 100, 100, 100)
                    };
                    reports.reportViewer1.SetPageSettings(pageSettings);
                    ReportParameter[] param = [new ReportParameter("numero_conduce", conduce)];
                    reports.reportViewer1.LocalReport.DataSources.Clear();
                    reports.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DataSet1", ds.Tables["dt"]));
                    reports.reportViewer1.LocalReport.SetParameters(param);
                    reports.reportViewer1.RefreshReport();
                    reports.Show();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el reporte de conduce. codigo error: " + ex.Message);
            }
        }
        public void ReporteCondece_sinPrecio(string conduce, Form form, string ReportName, string TitleReport)
        {
            ReporteConduce_conPrecio(conduce, form, ReportName, TitleReport);
        }
        public void Reporte_PackingList(string conduce, Form form)
        {
            DataSet ds = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandText = "SELECT a.conduce,a.product_id,b.product_name,a.unique_code,a.roll_number,a.width,a.lenght,a.msi,a.splice,a.roll_id,a.cant_despacho,a.tipo,no_paleta,c.fecha,c.customer_id,d.customer_name FROM rcdespacho a LEFT JOIN producto b ON a.product_id = b.product_id LEFT JOIN despacho c ON a.conduce=c.numero LEFT JOIN customer d ON c.customer_id=d.customer_id WHERE conduce=@p1",
                        CommandType = CommandType.Text
                    };
                    SqlParameter p1 = new("@p1", SqlDbType.VarChar)
                    {
                        Value = conduce
                    };
                    comando.Parameters.Add(p1);
                    conn.Open();
                    SqlDataAdapter da = new(comando);
                    da.Fill(ds, "dt");
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al cargar el reporte de packing list. codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    ReportsViewer reports = new()
                    {
                        Text = "REPORTE DE PACKING-LIST.",
                        Width = 1130,
                        Height = 780,
                        MdiParent = form.MdiParent,
                    };
                    string ReportName = "picking-list.rdlc";
                    reports.reportViewer1.ProcessingMode = ProcessingMode.Local;
                    reports.reportViewer1.LocalReport.ReportPath = GetPathApplication(ReportName, R.PATH_REPORTS.REPORTS_DESPACHO);
                    PageSettings pageSettings = new()
                    {
                        PaperSize = new PaperSize("Carta", 850, 1100),
                        Landscape = false,
                        Margins = new Margins(0, 0, 0, 0)
                    };
                    reports.reportViewer1.SetPageSettings(pageSettings);
                    ReportParameter[] param = [new ReportParameter("numero_conduce", conduce)];
                    reports.reportViewer1.LocalReport.DataSources.Clear();
                    reports.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DsRollos", ds.Tables["dt"]));
                    reports.reportViewer1.LocalReport.SetParameters(param);
                    reports.reportViewer1.RefreshReport();
                    reports.Show();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el reporte de packing list. codigo error: " + ex.Message);
            }
        }
        public void Reporte_DetallePaleta(string conduce, Form form)
        {
            DataSet ds = new();
            try
            {
                Task.Run(() =>
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandText = "SELECT a.numero,a.number_palet,a.medida,a.contenido,a.kilo_neto,a.kilo_bruto," + "b.customer_id,c.Customer_Name,b.fecha FROM Paleta AS a LEFT OUTER JOIN despacho AS b " +
                          "ON a.numero = b.numero LEFT OUTER JOIN Customer AS c ON b.customer_id = c.Customer_ID " +
                          "WHERE(a.numero = @p1)",
                        CommandType = CommandType.Text
                    };
                    SqlParameter p1 = new("@p1", SqlDbType.VarChar)
                    {
                        Value = conduce
                    };
                    comando.Parameters.Add(p1);
                    conn.Open();
                    SqlDataAdapter da = new(comando);
                    da.Fill(ds, "dt");
                })
                .ContinueWith(t =>
                {
                    if (t.IsFaulted)
                    {
                        ServiceErrors.Report("Error al cargar el reporte de detalle de paleta. codigo error: " + t.Exception?.GetBaseException()?.Message);
                        return;
                    }
                    ReportsViewer reports = new()
                    {
                        Text = "REPORTE DETALLE DE PALETA",
                        Width = 1130,
                        Height = 780,
                        MdiParent = form.MdiParent,
                    };
                    string ReportName = "RptDetalle-Paleta.rdlc";
                    reports.reportViewer1.ProcessingMode = ProcessingMode.Local;
                    reports.reportViewer1.LocalReport.ReportPath = GetPathApplication(ReportName, R.PATH_REPORTS.REPORTS_DESPACHO);
                    PageSettings pageSettings = new()
                    {
                        PaperSize = new PaperSize("Letter", 1100, 850),
                        Landscape = true,
                        Margins = new Margins(0, 0, 0, 0)
                    };
                    reports.reportViewer1.SetPageSettings(pageSettings);
                    ReportParameter[] param = [new ReportParameter("numero_conduce", conduce)];
                    reports.reportViewer1.LocalReport.DataSources.Clear();
                    reports.reportViewer1.LocalReport.DataSources.Add(new ReportDataSource("DsPaleta", ds.Tables["dt"]));
                    reports.reportViewer1.LocalReport.SetParameters(param);
                    reports.reportViewer1.RefreshReport();
                    reports.Show();
                }, TaskScheduler.FromCurrentSynchronizationContext());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el reporte de detalle de paleta. codigo error: " + ex.Message);
            }
        }
        private static string GetPathApplication(string ReportName, string folderNameReport)
        {
            string AppDirectory = AppDomain.CurrentDomain.BaseDirectory;
            string ReportsFolder = Path.Combine(AppDirectory, folderNameReport);
            if (!Directory.Exists(ReportsFolder))
            {
                Directory.CreateDirectory(ReportsFolder);
            }
            string ReportPath = Path.Combine(ReportsFolder, ReportName);
            return ReportPath;
        }
    }
}
