using System.Data;
using System.Drawing.Drawing2D;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Forms.Seleccion;

using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.DespachoService.DespachoService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Core;
using Ritrama2025.Helpers;


using Sunny.UI;
namespace Ritrama2025.Forms
{
    public partial class FrmDespacho : UIForm, IFormTemaClaro
    {
        private readonly IConfiguration Config;
        private readonly IDespachoService Service;
        private readonly IReportsService ReportService;
        private readonly IExportDataService ExportDataService;
        private readonly ICommonService CommonService;

        DataSet Ds = new();
        readonly BindingSource Bs = [];
        readonly BindingSource BsDetalleRC = [];
        readonly BindingSource BsItems = [];
        readonly BindingSource BsPalet = [];
        DataRowView ParentRow = null!;
        DataRowView ParentRowPalet = null!;
        public readonly decimal porc_itbis = 18.00m;

        public FrmDespacho(
            IConfiguration Config,
            IDespachoService Service,
            IReportsService reportService,
            ICommonService commonService,
            IExportDataService exportDataService)
        {
            InitializeComponent();
            this.Config = Config ?? throw new ArgumentNullException(nameof(Config));
            this.Service = Service ?? throw new ArgumentNullException(nameof(Service));
            this.ReportService = reportService ?? throw new ArgumentNullException(nameof(reportService));
            this.CommonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            this.ExportDataService = exportDataService ?? throw new ArgumentNullException(nameof(exportDataService));

            // Este modulo usa el estilo VERDE de SunnyUI (igual que Produccion).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            // Aplica el tema verde en el constructor para que el primer pintado ya sea
            // verde y no se vea el flash naranja del UIStyleManager global del Main.
            AplicarTemaVerde();
        }

        // IFormTemaClaro: reaplica el verde para pisar el UIStyleManager global del Main
        // (naranja) que puede re-estilizar este form al embeberse o al repintar.
        public void ReaplicarTema() => AplicarTemaVerde();

        private void RedondearControl(Control? ctrl, int radio)
        {
            if (ctrl == null) return;
            Rectangle rect = ctrl.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            var gp = new GraphicsPath();
            gp.StartFigure();
            gp.AddArc(0, 0, radio, radio, 180, 90);
            gp.AddArc(rect.Width - radio, 0, radio, radio, 270, 90);
            gp.AddArc(rect.Width - radio, rect.Height - radio, radio, radio, 0, 90);
            gp.AddArc(0, rect.Height - radio, radio, radio, 90, 90);
            gp.CloseFigure();
            ctrl.Region = new Region(gp);
        }

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            this.BackColor = Color.White;
            this.Style = UIStyle.Green;
            this.TitleColor = verde;
            this.TitleForeColor = Color.White;
            if (panel1 != null)
            {
                panel1.BackColor = verde;
                label26.ForeColor = Color.White;
                registros.ForeColor = Color.White;
            }
            if (tabControl1 != null)
            {
                tabControl1.BackColor = verde;
                if (tabPage1 != null) tabPage1.BackColor = Color.White;
                if (tabPage2 != null) tabPage2.BackColor = Color.White;
                tabControl1.Invalidate();
            }
            RedondearControl(tabControl1, 12);
            RedondearControl(grid_items, 12);
            RedondearControl(grid_rc, 12);
            RedondearControl(grid_detalle_paletas, 12);
        }

        private async void Despacho_Load(object sender, EventArgs e)
        {
            if (this.TopLevel)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(155, 45);
            }

            // Fase 1: async/await correcto con CancellationToken - no bloquear UI con .Result
            FrmLoading loading = null!;
            try
            {
                loading = new FrmLoading("Cargando despachos...");
                loading.Show();
                loading.Refresh();

                UseWaitCursor = true;
                Enabled = false;

                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                Ds = await Service.LoadDataDespachos(cts.Token).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Tiempo de espera agotado al cargar despachos.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar despachos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            finally
            {
                loading?.Close();
                UseWaitCursor = false;
                Enabled = true;
            }

            if (Ds.Tables.Count > 0)
            {
                //Enlace a datos Encabezado.
                Bs.DataSource = Ds;
                Bs.DataMember = "DtMasterDespachos";
                //Enlace Datos DetalleRC.
                BsDetalleRC.DataSource = Bs;
                BsDetalleRC.DataMember = "FK_DESPACHOS_DETALLERC";
                //Enlace Datos Items.
                BsItems.DataSource = Bs;
                BsItems.DataMember = "FK_DESPACHOS_ITEMS";
                //Enlace Datos Grid Palet.
                BsPalet.DataSource = Bs;
                BsPalet.DataMember = "FK_DESPACHOS_PALET";

                //Definicion de las columnas del grid rollos cortados.
                DefColumnsGridRC();
                //Definicion de las columnas del grid de Items.
                DefColumnsGridItems();
                //Definicion de las columnas del grid de Detalle de paleta.
                grid_detalle_paletas.AutoGenerateColumns = false;
                AGREGAR_COLUMN_GRID("number_palet", 70, "# Palet.", "number_palet", grid_detalle_paletas);
                AGREGAR_COLUMN_GRID("medida", 70, "Medida", "medida", grid_detalle_paletas);
                AGREGAR_COLUMN_GRID("contenido", 200, "Contenido", "contenido", grid_detalle_paletas);
                AGREGAR_COLUMN_GRID("kilo_neto", 70, "Kilo Neto", "kilo_neto", grid_detalle_paletas);
                AGREGAR_COLUMN_GRID("kilo_bruto", 70, "Kilo Bruto", "kilo_bruto", grid_detalle_paletas);
                grid_detalle_paletas.DataSource = BsPalet;
                //Binding Forms
                txt_numero.DataBindings.Add("Text", Bs, "numero");
                // Fecha: DateTimePicker (UIDatetimePicker) se enlaza por "Value" (no por "Text") y con formato corto.
                txt_fecha_despacho.DateFormat = "dd/MM/yyyy";
                txt_fecha_despacho.DateCultureInfo = new CultureInfo("es-ES");
                foreach (Control c in txt_fecha_despacho.Controls) c.Font = new Font(c.Font.FontFamily, 7F);
                txt_fecha_despacho.DataBindings.Add("Value", Bs, "fecha", true, DataSourceUpdateMode.OnValidation);
                txt_fecha_despacho.DataBindings["Value"]!.Format += (s, e) =>
                {
                    if (e.Value == DBNull.Value || e.Value == null) e.Value = DateTime.Today;
                };
                txt_persondelivery.DataBindings.Add("Text", Bs, "person_contact");
                txt_custid.DataBindings.Add("Text", Bs, "customer_id");
                txt_transport_id.DataBindings.Add("Text", Bs, "transport_id");
                txt_transport_name.DataBindings.Add("Text", Bs, "transporte");
                txt_chofer_id.DataBindings.Add("Text", Bs, "chofer_id");
                txt_chofer_name.DataBindings.Add("Text", Bs, "chofer");
                txt_camion_id.DataBindings.Add("Text", Bs, "placas_id");
                txt_camion_name.DataBindings.Add("Text", Bs, "camion");
                txt_vend_id.DataBindings.Add("Text", Bs, "vendor_id");
                cbo_embalaje.DataBindings.Add("Text", Bs, "packing");
                txt_orden_trabajo.DataBindings.Add("Text", Bs, "orden_trabajo");
                txt_orden_compra.DataBindings.Add("Text", Bs, "orden_compra");
                cbo_tipoVenta.DataBindings.Add("Text", Bs, "tipo_venta");
                txt_subtotal.DataBindings.Add("Text", Bs, "subtotal");
                txt_porc_itbis.DataBindings.Add("Text", Bs, "porc_itbis");
                txt_itbis.DataBindings.Add("Text", Bs, "itbis");
                txt_totalmonto.DataBindings.Add("Text", Bs, "total$rd");
                txt_custname.DataBindings.Add("Text", Bs, "customer_name");
                txt_vendorname.DataBindings.Add("Text", Bs, "vendor_name");
                txt_cant_total.DataBindings.Add("Text", Bs, "total_cantidad");
                txt_msi_total.DataBindings.Add("Text", Bs, "total_msi");
                txt_pie_total.DataBindings.Add("Text", Bs, "total_pie");
                txt_kilos_total.DataBindings.Add("Text", Bs, "total_kilos");
                txt_palet_kilo_neto.DataBindings.Add("Text", Bs, "total_kilos_netos_palet");
                txt_palet_kilo_bruto.DataBindings.Add("Text", Bs, "total_kilos_brutos_palet");
                //agregar la columna.
                DataGridViewButtonColumn ColumnButton = new()
                {
                    Name = "btn_description",
                    HeaderText = "Accion",
                    Text = "...",
                    UseColumnTextForButtonValue = true,
                    Width = 60,
                };
                grid_detalle_paletas.Columns.Add(ColumnButton);
                grid_detalle_paletas.Columns["btn_description"]!.DisplayIndex = 3;
                //IR AL FINAL DE LA BASE DE DATOS LA ULTIMA ORDEN DE DESPACHO.
                Bs.Position = Bs.Count - 1;
            }

            RedondearControl(grid_items, 12);
            RedondearControl(grid_rc, 12);
            RedondearControl(grid_detalle_paletas, 12);






        }

        private void Bot_primero_Click(object sender, EventArgs e)
        {
            Bs.Position = 0;
        }

        private void Bot_siguiente_Click(object sender, EventArgs e)
        {
            Bs.Position += 1;
        }

        private void Bot_anterior_Click(object sender, EventArgs e)
        {
            Bs.Position -= 1;
        }

        private void Bot_ultimo_Click(object sender, EventArgs e)
        {
            Bs.Position = Bs.Count - 1;
        }
        private static void AGREGAR_COLUMN_GRID(string name, int size, string title, string field_bd, DataGridView grid)
        {
            DataGridViewTextBoxColumn col = new()
            {
                Name = name,
                Width = size,
                HeaderText = title,
                DataPropertyName = field_bd,
            };
            grid.Columns.Add(col);
        }

        private async void Bot_nuevo_Click(object sender, EventArgs e)
        {
            ParentRow = (DataRowView)Bs.AddNew()!;
            ParentRow.BeginEdit();
            ParentRow["numero"] = await Service.GetNumberConsecAsync().ConfigureAwait(true);
            ParentRow.EndEdit();
            grid_rc.DataSource = "";
            if (grid_rc.Rows.Count > 0)
            {
                grid_rc.Rows.Clear();
            }
            grid_items.DataSource = "";
            if (grid_items.Rows.Count > 0)
            {
                grid_items.Rows.Clear();
            }
            txt_fecha_despacho.Enabled = true;
            txt_persondelivery.ReadOnly = false;
            cbo_embalaje.Enabled = true;
            cbo_tipoVenta.Enabled = true;
            txt_orden_trabajo.ReadOnly = false;
            txt_orden_compra.ReadOnly = false;
            txt_subtotal.Text = "0";
            txt_itbis.Text = "0";
            txt_totalmonto.Text = "0";
            txt_palet_kilo_bruto.Text = "0";
            txt_palet_kilo_neto.Text = "0";
            txt_cant_total.Text = "0";
            txt_msi_total.Text = "0";
            txt_pie_total.Text = "0";
            txt_kilos_total.Text = "0";

            bot_add_palet.Enabled = true;
            bot_delete_palet.Enabled = true;
            bot_picking.Enabled = true;
            btn_buscar_customer.Enabled = true;
            bot_buscar_vendor.Enabled = true;
            bot_camion.Enabled = true;
            bot_transporte.Enabled = true;
            bot_chofer.Enabled = true;
            bot_grabar.Enabled = true;
            bot_cancelar.Enabled = true;

            bot_primero.Enabled = false;
            bot_siguiente.Enabled = false;
            bot_anterior.Enabled = false;
            bot_ultimo.Enabled = false;
            bot_nuevo.Enabled = false;
            bot_buscar.Enabled = false;
            btn_reports.Enabled = false;
            btn_exports.Enabled = false;
            btn_close_document.Enabled = false;
            btn_label_print.Enabled = false;
            bot_anular.Enabled = false;

            grid_detalle_paletas.ReadOnly = false;

        }

        private void Bot_picking_Click(object sender, EventArgs e)
        {
            FrmPickingDespacho frm_picking = new(CommonService);
            frm_picking.ShowDialog();

            if (frm_picking.Lista_Rollos.Count <= 0)
            {
                return;
            }

            //descarga de los rollos caortados.
            foreach (var item in frm_picking.Lista_Rollos)
            {
                DataRowView row = (DataRowView)BsDetalleRC.AddNew()!;
                row.BeginEdit();
                row["conduce"] = txt_numero.Text;
                row["unique_code"] = item.UniqueCode;
                row["product_id"] = item.Product_Id;
                row["product_name"] = item.Product_Name;
                row["roll_number"] = item.RollNumber;
                row["width"] = item.Width;
                row["lenght"] = item.Length;
                row["msi"] = item.Msi;
                row["splice"] = item.Splice;
                row["roll_id"] = item.Roll_Id;
                row["cant_despacho"] = item.Cantidad_despacho;
                row["tipo"] = item.Tipo;
                row["no_paleta"] = item.Paleta;
                row.Row.SetParentRow(((DataRowView)Bs.Current!).Row, Ds.Relations["FK_DESPACHOS_DETALLERC"]);
                row.EndEdit();
            }

            grid_rc.DataSource = BsDetalleRC;
            grid_rc.Refresh();

            foreach (var item in frm_picking.Lista_Items)
            {
                DataRowView row = (DataRowView)BsItems.AddNew()!;
                row.BeginEdit();
                row["product_id"] = item.Product_id;
                row["product_name"] = item.Product_name;
                row["unidad"] = "ROLLOS";
                row["cant"] = item.Cantidad;
                row["width"] = item.Width;
                row["lenght"] = item.Lenght;
                row["msi"] = item.Msi;
                row["total_pie_lin"] = item.Total_PieLineal;
                row["m2"] = item.M2;
            }

            grid_items.DataSource = BsItems;
            grid_items.Refresh();

            //Calculo de los pies lineales.
            for (int i = 0; i <= grid_items.Rows.Count - 1; i++)
            {
                if (grid_items.Rows[i].IsNewRow) continue;
                decimal ancho = ToDecimalSafe(grid_items.Rows[i].Cells["width"].Value);
                decimal largo = ToDecimalSafe(grid_items.Rows[i].Cells["lenght"].Value);
                int cantidad = ToIntSafe(grid_items.Rows[i].Cells["cant"].Value);
                //CALCULO DEL MSI RENGLON
                grid_items.Rows[i].Cells["msi"].Value = CalculosDespacho.CalcularMsiRenglon(ancho, largo, cantidad);
                grid_items.Rows[i].Cells["total_pie_lin"].Value = "0";
                //Busqueda del ratio por producto.
                grid_items.Rows[i].Cells["ratio"].Value = "0";
                //Calculo de la Columna Kilo-Rollo.
                grid_items.Rows[i].Cells["kilo_rollo"].Value = "0";
                grid_items.Rows[i].Cells["kilo_total"].Value = "0";

                grid_items.Rows[i].Cells["precio"].Value = "0";
                grid_items.Rows[i].Cells["total_renglon"].Value = "0";
            }


            CalcularTotalesColumns();
            txt_porc_itbis.Text = $"{porc_itbis:0.##}";


            //abrir la columna de precio para hacer el calculo de total renglon.
            grid_items.ReadOnly = false;

        }
        private void Btn_buscar_customer_Click(object sender, EventArgs e)
        {
            FrmSeleccion SelClientes = new(CommonService)
            {
                DtItems = Ds.Tables["DtClientes"]!,
                Titulo = "clientes",
            };
            SelClientes.ShowDialog();
            txt_custid.Text = SelClientes.Id;
            txt_custname.Text = SelClientes.Description;
        }

        private void Bot_buscar_vendor_Click(object sender, EventArgs e)
        {
            FrmSeleccion SelVendors = new(CommonService)
            {
                DtItems = Ds.Tables["DtVendors"]!,
                Titulo = "Vendedores",
            };
            SelVendors.ShowDialog();
            txt_vend_id.Text = SelVendors.Id;
            txt_vendorname.Text = SelVendors.Description;
        }

        private void Bot_transporte_Click(object sender, EventArgs e)
        {
            FrmSeleccion SelTransport = new(CommonService)
            {
                DtItems = Ds.Tables["DtTransport"]!,
                Titulo = "Transporte",
            };
            SelTransport.ShowDialog();
            txt_transport_id.Text = SelTransport.Id;
            txt_transport_name.Text = SelTransport.Description;
        }

        private void Bot_chofer_Click(object sender, EventArgs e)
        {
            FrmSeleccion SelChofer = new(CommonService)
            {
                DtItems = Ds.Tables["DtChofer"]!,
                Titulo = "chofer",
            };
            SelChofer.ShowDialog();
            txt_chofer_id.Text = SelChofer.Id;
            txt_chofer_name.Text = SelChofer.Description;
        }

        private void Bot_camion_Click(object sender, EventArgs e)
        {
            FrmSeleccion SelCamion = new(CommonService)
            {
                DtItems = Ds.Tables["DtCamion"]!,
                Titulo = "camion",
            };
            SelCamion.ShowDialog();
            txt_camion_id.Text = SelCamion.Id;
            txt_camion_name.Text = SelCamion.Description;
        }
        private void CalcularTotalesColumns()
        {
            int TotalCantitdad = 0;
            decimal TotalMsi = 0;
            decimal TotalPieLin = 0;
            decimal TotalKilosRollo = 0;
            decimal TotalKilosTotal = 0;
            for (int i = 0; i <= grid_items.Rows.Count - 1; i++)
            {
                if (grid_items.Rows[i].IsNewRow) continue;
                TotalCantitdad += ToIntSafe(grid_items.Rows[i].Cells["cant"].Value);
                //TotalMsi += ToDecimalSafe(grid_items.Rows[i].Cells["m2"].Value);
                if (!string.IsNullOrEmpty(grid_items.Rows[i].Cells["total_pie_lin"].Value!.ToString()))
                {
                    TotalPieLin += ToDecimalSafe(grid_items.Rows[i].Cells["total_pie_lin"].Value);
                }
                if (!string.IsNullOrEmpty(grid_items.Rows[i].Cells["kilo_rollo"].Value!.ToString()))
                {
                    TotalKilosRollo += ToDecimalSafe(grid_items.Rows[i].Cells["kilo_rollo"].Value);
                }
                if (!string.IsNullOrEmpty(grid_items.Rows[i].Cells["kilo_total"].Value!.ToString()))
                {
                    TotalKilosTotal += ToDecimalSafe(grid_items.Rows[i].Cells["kilo_total"].Value);
                }
                if (!string.IsNullOrEmpty(grid_items.Rows[i].Cells["msi"].Value!.ToString()))
                {
                    TotalMsi += ToDecimalSafe(grid_items.Rows[i].Cells["msi"].Value);
                }
            }
            txt_cant_total.Text = TotalCantitdad.ToString();
            txt_msi_total.Text = $"{TotalMsi:0.##}";
            txt_pie_total.Text = $"{TotalPieLin:###.###.##}";
            txt_kilos_total.Text = $"{TotalKilosTotal:###.###.##}";
            txt_cant_total.Refresh();
            txt_msi_total.Refresh();
            txt_pie_total.Refresh();
            txt_kilos_total.Refresh();
        }
        private static string CalcularImpuestoRenglon(decimal subtotal, decimal monto_itbis)
        {
            decimal renglonImpuesto = subtotal + monto_itbis;
            return $"{renglonImpuesto,12:N2}";
        }
        private static string CalcularImpuestoDocument(decimal subtotal)
        {
            return $"{CalculosDespacho.CalcularItbis(subtotal),12:N2}";
        }
        private decimal CalcularSubtotalDocument()
        {
            decimal SubTotalDoc = 0;
            for (int i = 0; i <= grid_items.Rows.Count - 1; i++)
            {
                if (grid_items.Rows[i].IsNewRow) continue;
                SubTotalDoc += ToDecimalSafe(grid_items.Rows[i].Cells["total_renglon"].Value);
            }
            txt_subtotal.Text = $"{SubTotalDoc,12:N2}";
            return SubTotalDoc;
        }
        private static string CalcularTotalesDocument(decimal subtotal, decimal impuesto)
        {
            return $"{CalculosDespacho.CalcularTotal(subtotal, impuesto),12:N2}";
        }
        private void CalcularTotalesDocument()
        {
            decimal subtotal = CalcularSubtotalDocument();
            decimal itbis = CalculosDespacho.CalcularItbis(subtotal);
            txt_itbis.Text = $"{itbis,12:N2}";
            txt_totalmonto.Text = CalcularTotalesDocument(subtotal, itbis);
        }

        private void Grid_items_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            //Calculo del total renglon.
            if (e.RowIndex >= 0 && e.RowIndex < grid_items.Rows.Count && !grid_items.Rows[e.RowIndex].IsNewRow)
            {
                decimal precio = ToDecimalSafe(grid_items.Rows[e.RowIndex].Cells["precio"].Value);
                decimal kiloTotal = ToDecimalSafe(grid_items.Rows[e.RowIndex].Cells["kilo_total"].Value);
                grid_items.Rows[e.RowIndex].Cells["total_renglon"].Value = precio * kiloTotal;
            }
            //Calculo de los totales del documento.
            CalcularTotalesDocument();

        }
        private void Bot_add_palet_Click(object sender, EventArgs e)
        {
            ParentRowPalet = (DataRowView)BsPalet.AddNew()!;
            ParentRowPalet.BeginEdit();
            ParentRowPalet["contenido"] = "";
            ParentRowPalet[4] = "0";
            ParentRowPalet[5] = "0";
            grid_detalle_paletas.Focus();
            grid_detalle_paletas.CurrentCell = grid_detalle_paletas.Rows[0].Cells[0];
            grid_detalle_paletas.BeginEdit(true); // Opcional: inicia edici�n en la celda

        }

        private void CalcularTotalPalet()
        {
            decimal PaletPesoNeto = 0;
            decimal PaletPesoBruto = 0;

            for (int i = 0; i <= grid_detalle_paletas.Rows.Count - 1; i++)
            {
                PaletPesoNeto += Convert.ToDecimal(grid_detalle_paletas.Rows[i].Cells["kilo_neto"].Value);
                PaletPesoBruto += Convert.ToDecimal(grid_detalle_paletas.Rows[i].Cells["kilo_bruto"].Value);
            }

            txt_palet_kilo_neto.Text = Convert.ToString(PaletPesoNeto);
            txt_palet_kilo_bruto.Text = Convert.ToString(PaletPesoBruto);
        }
        private void Grid_detalle_paletas_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            if (grid_detalle_paletas.Columns["kilo_neto"]!.Index == e.ColumnIndex || grid_detalle_paletas.Columns["kilo_bruto"]!.Index == e.ColumnIndex)
            {
                CalcularTotalPalet();
            }
        }

        private void Grid_detalle_paletas_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (grid_detalle_paletas.Columns["btn_description"]!.Index == e.ColumnIndex)
            {
                Frm_descriptionPalet DescriptionPalet = new()
                {
                    ContentTextDescription = Convert.ToString(grid_detalle_paletas.Rows[e.RowIndex].Cells["contenido"].Value) ?? string.Empty
                };

                DescriptionPalet.ShowDialog();

                grid_detalle_paletas.Rows[e.RowIndex].Cells["contenido"].Value = DescriptionPalet.ContentTextDescription;
            }
        }

        private static decimal ToDecimalSafe(object? v, decimal fallback = 0)
            => decimal.TryParse(Convert.ToString(v), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : fallback;
        private static int ToIntSafe(object? v, int fallback = 0)
            => int.TryParse(Convert.ToString(v), out var n) ? n : fallback;
        private static double ToDoubleSafe(object? v, double fallback = 0)
            => double.TryParse(Convert.ToString(v), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var d) ? d : fallback;
        private static short ToInt16Safe(object? v, short fallback = 0)
            => short.TryParse(Convert.ToString(v), out var n) ? n : fallback;

        private void Bot_grabar_Click(object sender, EventArgs e)
        {
            //validaciones.
            if (txt_custid.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir los datos del cliente...");
                return;
            }
            if (txt_camion_id.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir los datos del camion...");
                return;
            }
            if (txt_transport_id.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir los datos del transporte...");
                return;
            }
            if (txt_chofer_id.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir los datos del chofer...");
                return;
            }
            if (txt_vend_id.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir los datos del vendedor...");
                return;
            }
            if (cbo_embalaje.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir el tipo de embalaje...");
                return;
            }
            if (txt_orden_trabajo.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir el numero de la orden de trabajo...");
                return;
            }
            if (txt_orden_compra.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir el numero de la orden de compra...");
                return;
            }
            if (cbo_tipoVenta.Text == string.Empty)
            {
                MessageBox.Show("Debe introducir el tipo de venta");
                return;
            }
            if (string.IsNullOrWhiteSpace(txt_fecha_despacho.Text) || !DateTime.TryParse(txt_fecha_despacho.Text, out DateTime fechaDespacho))
            {
                MessageBox.Show("Debe introducir una fecha de despacho valida...");
                return;
            }
            if (grid_rc.Rows.Count <= 0)
            {
                MessageBox.Show("Debe agregar los renglones de los rollos costados...");
                return;
            }





            if (string.IsNullOrWhiteSpace(txt_numero.Text))
            {
                MessageBox.Show("No se pudo generar el n�mero de despacho. Cree el documento de nuevo.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            foreach (DataGridViewRow Fila in grid_detalle_paletas.Rows)
            {
                if (Fila.IsNewRow) continue;
                foreach (DataGridViewCell Celda in Fila.Cells)
                {
                    if (Celda.ColumnIndex == 3)
                    {
                        if (ToDecimalSafe(Celda.Value) <= 0)
                        {
                            MessageBox.Show("debe introducir los kilos netos de la paleta.");
                            return;
                        }
                    }
                    if (Celda.ColumnIndex == 4)
                    {
                        if (ToDecimalSafe(Celda.Value) <= 0)
                        {
                            MessageBox.Show("debe introducir los kilos brutos de la paleta.");
                            return;
                        }
                    }
                }
            }


            //Encabezado de despacho.
            Despacho DocumentDespacho = new()
            {

                Numero = txt_numero.Text,
                Fecha_despacho = fechaDespacho,
                Customer_Id = txt_custid.Text,
                Customer_Name = txt_custname.Text,
                Persona_Contact = txt_persondelivery.Text,
                Vendor_Id = txt_vend_id.Text,
                Vendor_Name = txt_vendorname.Text,
                Transport_Id = txt_transport_id.Text,
                Transport_Name = txt_transport_name.Text,
                Chofer_Id = txt_chofer_id.Text,
                Chofer_Name = txt_chofer_name.Text,
                Camion_Id = txt_camion_id.Text,
                Camion_Name = txt_camion_name.Text,
                Tipo_Embalaje = cbo_embalaje.Text,
                Orden_Trabajo = txt_orden_trabajo.Text,
                Orden_Compra = txt_orden_compra.Text,
                Tipo_venta = cbo_tipoVenta.Text,
                Total_Cantidad = Convert.ToInt32(txt_cant_total.Text),
                Total_Msi = Convert.ToDecimal(txt_msi_total.Text),
                Total_Pie = 0,
                Total_Kilos = 0,
                SubTotal = 0,
                Porc_Itbis = 0,
                Monto_Itbis = 0,
                Total_Despacho = 0,
                Total_kilos_netos_palet = 0,
                Total_kilos_brutos_palet = 0,
                //crear picking-list.
                Detalle_RC = [],
                //Items de despacho.
                Items_Despacho = [],
                //Detalle de paleta.
                Detalle_Paleta = [],
            };
            //picking-list;
            for (int i = 0; i <= grid_rc.Rows.Count - 1; i++)
            {
                if (grid_rc.Rows[i].IsNewRow) continue;
                RolloCortado Rollo = new()
                {
                    Numero = DocumentDespacho.Numero,
                    UniqueCode = Convert.ToString(grid_rc.Rows[i].Cells["unique_code"].Value) ?? string.Empty,
                    Product_Id = Convert.ToString(grid_rc.Rows[i].Cells["product_id"].Value) ?? string.Empty,
                    Product_Name = Convert.ToString(grid_rc.Rows[i].Cells["product_name"].Value) ?? string.Empty,
                    RollNumber = ToIntSafe(grid_rc.Rows[i].Cells["roll_number"].Value),
                    Width = ToDoubleSafe(grid_rc.Rows[i].Cells["width"].Value),
                    Length = ToDoubleSafe(grid_rc.Rows[i].Cells["length"].Value),
                    Msi = ToDoubleSafe(grid_rc.Rows[i].Cells["msi"].Value),
                    Splice = ToInt16Safe(grid_rc.Rows[i].Cells["splice"].Value),
                    Roll_Id = Convert.ToString(grid_rc.Rows[i].Cells["roll_id"].Value) ?? string.Empty,
                    Cantidad_despacho = 0,
                    Tipo = "n/a",
                    Paleta = "0"
                };
                DocumentDespacho.Detalle_RC.Add(Rollo);
            }
            //item a despachar.
            for (int i = 0; i <= grid_items.Rows.Count - 1; i++)
            {
                if (grid_items.Rows[i].IsNewRow) continue;
                ItemsDespacho itemsDespacho = new()
                {
                    Numero = DocumentDespacho.Numero,
                    Product_id = Convert.ToString(grid_items.Rows[i].Cells["product_id"].Value) ?? string.Empty,
                    Product_name = Convert.ToString(grid_items.Rows[i].Cells["product_name"].Value) ?? string.Empty,
                    Cantidad = ToDecimalSafe(grid_items.Rows[i].Cells["cant"].Value),
                    Unid_id = "1",
                    Unidad = Convert.ToString(grid_items.Rows[i].Cells["unidad"].Value) ?? string.Empty,
                    Width = ToDecimalSafe(grid_items.Rows[i].Cells["width"].Value),
                    Lenght = ToDecimalSafe(grid_items.Rows[i].Cells["lenght"].Value),
                    Msi = ToDecimalSafe(grid_items.Rows[i].Cells["msi"].Value),
                    Total_PieLineal = ToDecimalSafe(grid_items.Rows[i].Cells["total_pie_lin"].Value),
                    Ratio = ToDecimalSafe(grid_items.Rows[i].Cells["ratio"].Value),
                    Kilo_Rollo = ToDecimalSafe(grid_items.Rows[i].Cells["kilo_rollo"].Value),
                    Kilo_Total = ToDecimalSafe(grid_items.Rows[i].Cells["kilo_total"].Value),
                    Precio = ToDecimalSafe(grid_items.Rows[i].Cells["precio"].Value),
                    Total_Renglon = ToDecimalSafe(grid_items.Rows[i].Cells["total_renglon"].Value),
                    Code_Person = "N/A",
                    M2 = grid_items.Columns.Contains("m2") ? ToDecimalSafe(grid_items.Rows[i].Cells["m2"].Value) : 0
                };
                DocumentDespacho.Items_Despacho.Add(itemsDespacho);
            }
            //detalle paleta.
            for (int i = 0; i <= grid_detalle_paletas.Rows.Count - 1; i++)
            {
                if (grid_detalle_paletas.Rows[i].IsNewRow) continue;
                Paleta palet = new()
                {
                    Numero = DocumentDespacho.Numero,
                    Number_Palet = Convert.ToString(grid_detalle_paletas.Rows[i].Cells["number_palet"].Value) ?? string.Empty,
                    Medida = Convert.ToString(grid_detalle_paletas.Rows[i].Cells["medida"].Value) ?? string.Empty,
                    Contenido = Convert.ToString(grid_detalle_paletas.Rows[i].Cells["contenido"].Value) ?? string.Empty,
                    Kilo_Neto = ToDecimalSafe(grid_detalle_paletas.Rows[i].Cells["kilo_neto"].Value),
                    Kilo_Bruto = ToDecimalSafe(grid_detalle_paletas.Rows[i].Cells["kilo_bruto"].Value)
                };
                DocumentDespacho.Detalle_Paleta.Add(palet);
            }


            // Calcular totales reales del despacho a partir de los items (evita guardar totales en 0).
            var totales = CalculosDespacho.CalcularTotales(
                DocumentDespacho.Items_Despacho.Select(i => i.Total_Renglon),
                DocumentDespacho.Items_Despacho.Select(i => i.Total_PieLineal),
                DocumentDespacho.Items_Despacho.Select(i => i.Kilo_Total));
            DocumentDespacho.SubTotal = totales.SubTotal;
            DocumentDespacho.Porc_Itbis = CalculosDespacho.PORC_ITBIS;
            DocumentDespacho.Monto_Itbis = totales.Itbis;
            DocumentDespacho.Total_Despacho = totales.Total;
            DocumentDespacho.Total_Pie = totales.TotalPieLineales;
            DocumentDespacho.Total_Kilos = totales.TotalKilos;
            // El "msi" del item ya incluye la cantidad (CalcularMsiRenglon = ancho*largo*cant/CONST_MSI),
            // por lo que el total debe ser la suma directa; multiplicarlo de nuevo por Cantidad
            // duplica el factor y desborda numeric(18,2) con cantidades grandes.
            DocumentDespacho.Total_Msi = DocumentDespacho.Items_Despacho.Sum(i => i.Msi);
            DocumentDespacho.Total_Cantidad = DocumentDespacho.Items_Despacho.Count;

            // Guardar todo el despacho en una sola transaccion (atomico).
            if (!Service.SaveDespachoCompleto(DocumentDespacho, fechaDespacho))
            {
                return;
            }
            MessageBox.Show("La Orden de despacho se guardo correctamente...", "Exito", MessageBoxButtons.OK, MessageBoxIcon.Information);




            //cerrar el formulario a solo lectura.
            txt_persondelivery.ReadOnly = true;
            txt_fecha_despacho.Enabled = false;
            cbo_tipoVenta.Enabled = false;
            cbo_embalaje.Enabled = false;
            txt_orden_compra.ReadOnly = true;
            txt_orden_trabajo.ReadOnly = true;
            bot_camion.Enabled = false;
            bot_chofer.Enabled = false;
            bot_transporte.Enabled = false;
            bot_add_palet.Enabled = false;
            bot_delete_palet.Enabled = false;
            bot_buscar_vendor.Enabled = false;
            btn_buscar_customer.Enabled = false;
            bot_picking.Enabled = false;
            btn_exports.Enabled = true;
            btn_label_print.Enabled = true;
            bot_anular.Enabled = true;
            grid_items.ReadOnly = true;
            grid_rc.ReadOnly = true;
            grid_detalle_paletas.ReadOnly = true;

            bot_primero.Enabled = true;
            bot_siguiente.Enabled = true;
            bot_anterior.Enabled = true;
            bot_ultimo.Enabled = true;
            bot_nuevo.Enabled = true;
            bot_grabar.Enabled = false;
            bot_cancelar.Enabled = false;
            bot_buscar.Enabled = true;
            btn_reports.Enabled = true;
            export_excel.Enabled = true;
            //Defino las columnas de nuevo y establezco el datasource de lo9s grid.
            DefColumnsGridRC();
            DefColumnsGridItems();
        }
        private void Reporte_conduce_conprecio_Click(object sender, EventArgs e)
        {
            var TitleReport = "REPORTE DE CONDUCE CON PRECIO.";
            ReportService.ReporteConduce_conPrecio(txt_numero.Text, this, "RptConduceConPrecio.rdlc", TitleReport);

        }

        private void Reporte_conduce_sinprecio_Click(object sender, EventArgs e)
        {
            var TitleReport = "REPORTE DE CONDUCE SIN PRECIO.";
            ReportService.ReporteCondece_sinPrecio(txt_numero.Text, this, "RptConduceSinPrecio.rdlc", TitleReport);
        }

        private void Reporte_picking_list_Click(object sender, EventArgs e)
        {
            ReportService.Reporte_PackingList(txt_numero.Text, this);
        }

        private void Reporte_detalle_paleta_Click(object sender, EventArgs e)
        {
            ReportService.Reporte_DetallePaleta(txt_numero.Text, this);
        }

        private void Export_excel_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exportar a Excel");
        }

        private void Export_pdf_Click(object sender, EventArgs e)
        {
            MessageBox.Show("Exportar a PDF.");
        }

        private void RollosCortadosToolStripMenuItem_Click(object sender, EventArgs e)
        {
            List<RolloCortado> rollosCortados = CREATE_ROLLOS_CORTADOS();
            ExportDataService.ExportToExcel<RolloCortado>(rollosCortados, "RollosCortados.xlsx");
        }

        private List<RolloCortado> CREATE_ROLLOS_CORTADOS()
        {
            List<RolloCortado> Lista_Rollos = [];
            //picking-list;
            for (int i = 0; i <= grid_rc.Rows.Count - 1; i++)
            {
                if (grid_rc.Rows[i].IsNewRow) continue;
                RolloCortado Rollo = new()
                {
                    Numero = txt_numero.Text,
                    UniqueCode = Convert.ToString(grid_rc.Rows[i].Cells["unique_code"].Value) ?? string.Empty,
                    Product_Id = Convert.ToString(grid_rc.Rows[i].Cells["product_id"].Value) ?? string.Empty,
                    Product_Name = Convert.ToString(grid_rc.Rows[i].Cells["product_name"].Value) ?? string.Empty,
                    RollNumber = ToIntSafe(grid_rc.Rows[i].Cells["Roll_Number"].Value),
                    Width = ToDoubleSafe(grid_rc.Rows[i].Cells["width"].Value),
                    Length = ToDoubleSafe(grid_rc.Rows[i].Cells["length"].Value),
                    Msi = ToDoubleSafe(grid_rc.Rows[i].Cells["msi"].Value),
                    Splice = ToInt16Safe(grid_rc.Rows[i].Cells["splice"].Value),
                    Roll_Id = Convert.ToString(grid_rc.Rows[i].Cells["roll_id"].Value) ?? string.Empty,
                    //Code_Person = Convert.ToString(grid_rc.Rows[i].Cells["code_person"].Value) ?? string.Empty,
                    Cantidad_despacho = 0,
                    Tipo = "n/a",
                    Paleta = "0"
                };
                Lista_Rollos.Add(Rollo);
            }
            return Lista_Rollos;
        }

        private List<Paleta> CREATE_DETALLE_PALETA_LIST()
        {
            List<Paleta> Lista_Paletas = [];
            //detalle paleta.
            for (int i = 0; i <= grid_detalle_paletas.Rows.Count - 1; i++)
            {
                if (grid_detalle_paletas.Rows[i].IsNewRow) continue;
                Paleta palet = new()
                {
                    Numero = txt_numero.Text,
                    Number_Palet = Convert.ToString(grid_detalle_paletas.Rows[i].Cells["number_palet"].Value) ?? string.Empty,
                    Medida = Convert.ToString(grid_detalle_paletas.Rows[i].Cells["medida"].Value) ?? string.Empty,
                    Contenido = Convert.ToString(grid_detalle_paletas.Rows[i].Cells["contenido"].Value) ?? string.Empty,
                    Kilo_Neto = ToDecimalSafe(grid_detalle_paletas.Rows[i].Cells["kilo_neto"].Value),
                    Kilo_Bruto = ToDecimalSafe(grid_detalle_paletas.Rows[i].Cells["kilo_bruto"].Value)
                };
                Lista_Paletas.Add(palet);
            }
            return Lista_Paletas;
        }

        private void Opc_exportdata_excel_detallepaleta_Click(object sender, EventArgs e)
        {
            List<Paleta> detalle_paleta = CREATE_DETALLE_PALETA_LIST();
            ExportDataService.ExportToExcel<Paleta>(detalle_paleta, "detalle_paleta.xlsx");
        }

        private void Btn_exports_ButtonClick(object sender, EventArgs e)
        {

        }

        private void Bot_cancelar_Click(object sender, EventArgs e)
        {
            DataRowView FilaActual;
            FilaActual = (DataRowView)Bs.Current!;
            FilaActual.Row.Delete();
            Bs.EndEdit();
            Bs.Position = Bs.Count;
            //Cerrar formulario.
            bot_primero.Enabled = true;
            bot_siguiente.Enabled = true;
            bot_anterior.Enabled = true;
            bot_ultimo.Enabled = true;
            bot_nuevo.Enabled = true;
            bot_grabar.Enabled = false;
            bot_cancelar.Enabled = false;
            bot_buscar.Enabled = true;
            btn_reports.Enabled = true;
            btn_exports.Enabled = true;
            btn_close_document.Enabled = true;
            btn_label_print.Enabled = true;
            bot_anular.Enabled = true;
            txt_fecha_despacho.Enabled = false;
            txt_persondelivery.ReadOnly = true;
            //txt_tipo_embalaje.ReadOnly = true;
            txt_orden_trabajo.ReadOnly = true;
            txt_orden_compra.ReadOnly = true;
            //txt_tipoventa.ReadOnly = true;
            bot_picking.Enabled = false;
            bot_camion.Enabled = false;
            bot_chofer.Enabled = false;
            bot_transporte.Enabled = false;
            bot_add_palet.Enabled = false;
            bot_delete_palet.Enabled = false;
            btn_buscar_customer.Enabled = false;
            bot_buscar_vendor.Enabled = false;
            DefColumnsGridRC();
            DefColumnsGridItems();
        }
        private void DefColumnsGridRC()
        {
            //Definicion de las columnas del grid de DetalleRC
            grid_rc.Columns.Clear();
            grid_rc.AutoGenerateColumns = false;
            AGREGAR_COLUMN_GRID("unique_code", 70, "Unique Code", "unique_code", grid_rc);
            AGREGAR_COLUMN_GRID("product_id", 70, "Prod. Id.", "product_id", grid_rc);
            AGREGAR_COLUMN_GRID("product_name", 250, "Product Name", "product_name", grid_rc);
            AGREGAR_COLUMN_GRID("roll_number", 70, "Roll Number", "roll_number", grid_rc);
            AGREGAR_COLUMN_GRID("width", 70, "Width", "width", grid_rc);
            AGREGAR_COLUMN_GRID("length", 70, "Lenght", "lenght", grid_rc);
            AGREGAR_COLUMN_GRID("msi", 70, "Msi", "msi", grid_rc);
            AGREGAR_COLUMN_GRID("splice", 70, "Splice", "splice", grid_rc);
            AGREGAR_COLUMN_GRID("cant_despacho", 80, "Cantidad Despacho", "cant_despacho", grid_rc);
            AGREGAR_COLUMN_GRID("roll_id", 70, "Roll Id.", "roll_id", grid_rc);
            AGREGAR_COLUMN_GRID("tipo", 70, "Tipo", "tipo", grid_rc);
            AGREGAR_COLUMN_GRID("paleta", 70, "Paleta", "no_paleta", grid_rc);
            grid_rc.DataSource = BsDetalleRC;
        }
        private void DefColumnsGridItems()
        {
            grid_items.Columns.Clear();
            grid_items.AutoGenerateColumns = false;
            AGREGAR_COLUMN_GRID("product_id", 70, "Product Id.", "product_id", grid_items);
            AGREGAR_COLUMN_GRID("product_name", 200, "Product Name", "product_name", grid_items);
            AGREGAR_COLUMN_GRID("unidad", 70, "Unidad", "unidad", grid_items);
            AGREGAR_COLUMN_GRID("cant", 60, "Cant.", "cant", grid_items);
            AGREGAR_COLUMN_GRID("width", 65, "Width [Pulg]", "width", grid_items);
            AGREGAR_COLUMN_GRID("lenght", 65, "Lenght [Pies]", "lenght", grid_items);
            AGREGAR_COLUMN_GRID("msi", 70, "MSI", "msi", grid_items);
            AGREGAR_COLUMN_GRID("total_pie_lin", 70, "Pie Lineales", "total_pie_lin", grid_items);
            AGREGAR_COLUMN_GRID("ratio", 60, "Ratio", "ratio", grid_items);
            AGREGAR_COLUMN_GRID("kilo_rollo", 70, "Kilo Rollo", "kilo_rollo", grid_items);
            AGREGAR_COLUMN_GRID("kilo_total", 70, "Kilo Total", "kilo_total", grid_items);
            AGREGAR_COLUMN_GRID("precio", 60, "Precio", "precio", grid_items);
            AGREGAR_COLUMN_GRID("total_renglon", 70, "Total Renglon", "total_renglon", grid_items);
            AGREGAR_COLUMN_GRID("code_person", 70, "Code Person", "code_person", grid_items);
            AGREGAR_COLUMN_GRID("m2", 70, "Total M2", "m2", grid_items);
            grid_items.DataSource = BsItems;
        }

        private void Grid_detalle_paletas_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            // Lista de columnas que deseas validar (puede ser por �ndice o por nombre)
            var columnasAValidar = new[] { "kilo_neto", "kilo_bruto" }; // nombres de columna

            // Obtener el nombre de la columna actual
            string columnName = grid_detalle_paletas.Columns[e.ColumnIndex].Name;

            // Solo validar si la columna est� en la lista
            if (columnasAValidar.Contains(columnName))
            {
                if (!decimal.TryParse(e.FormattedValue!.ToString(), out decimal valor) || valor <= 0)
                {
                    MessageBox.Show($"El valor de la columna '{columnName}' debe ser mayor que cero.", "Valor inv�lido", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Cancel = true; // Cancela el cambio hasta que sea v�lido
                }
            }
        }

        private void bot_buscar_Click(object sender, EventArgs e)
        {

        }

        private void toolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {

        }

        private void btn_buscar_orden_Click(object sender, EventArgs e)
        {
            Frm_oneparameter frmBuscar = new()
            {
                //MdiParent = (Form)this.Parent!,
                StartPosition = StartPosition = FormStartPosition.Manual,
                Location = new Point { X = Location.X + 300, Y = Location.Y + 150 }
            };
            frmBuscar.ShowDialog();
            if (frmBuscar.Parameter != null)
            {
                if (!int.TryParse(frmBuscar.Parameter.Trim(), out int parametro))
                {
                    MessageBox.Show("El numero de despacho debe ser un valor numerico.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                Bs.Sort = "numero";
                int index = Bs.Find("numero", parametro);

                if (index >= 0)
                {
                    Bs.Position = index;
                    //    UpdateStepIndicator();
                    //    ContadorRegistros();
                }
                else
                {
                    MessageBox.Show("No se encontro el numero de despacho...", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void label13_Click(object sender, EventArgs e)
        {

        }

        private void txt_custid_TextChanged(object sender, EventArgs e)
        {

        }
    }
}


