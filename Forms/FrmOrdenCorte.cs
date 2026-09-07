using System.Configuration;
using System.Data;
using System.Globalization;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;
using System.Net;
using System.Net.Sockets;
using System.Reflection;
using Newtonsoft.Json;
using Ritrama2025.Core;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Forms.Seleccion;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ReportsService.ReportsService;

using Sunny.UI;
namespace Ritrama2025.Forms;

public partial class FrmOrdenCorte : UIForm, IAsyncFormLoad, IFormTemaClaro
{
    private readonly IProduccionService Service;
    private readonly IExportDataService ExportDataService;
    private readonly IReportsService ReportService;
    private readonly ICommonService CommonService;
    DataSet Ds = new();
    readonly BindingSource BsMaster = [];
    readonly BindingSource BsDetails = [];
    readonly BindingSource BsCortes = [];
    DataRowView ParentRow = null!;
    DataRowView ChildRowCortes = null!;
    DataRowView RollosCortados = null!;
    readonly string operadorId = "ff8fe855-0f8b-4062-8aa5-860d94f804d5";
    readonly string operadorName = "NO-ASIGNADO";
    private string TipoMovimiento = "";
    int EditMode = 0;
    readonly Dictionary<Control, Color> coloresOriginalesTextbox = [];
    readonly Dictionary<Control, Color> coloresOriginalesDisable = [];
    readonly Dictionary<Control, bool> styleCustomModeOriginales = [];
    readonly Dictionary<Control, Color> coloresOriginalesRect = [];
    readonly Dictionary<Control, Color> coloresOriginalesRectReadonly = [];
    string Rollid_master = string.Empty;
    Orden Orden { get; set; } = null!;
    List<Corte> Cortes { get; set; } = [];
    List<RolloCortado> Detalle { get; set; } = [];

    public FrmOrdenCorte(IProduccionService service, IExportDataService exportService, IReportsService reportService, ICommonService commonService)
    {
        InitializeComponent();
        Service = service;
        ExportDataService = exportService;
        ReportService = reportService;
        CommonService = commonService;
        this.Text = "Producción";

        // Este modulo usa el estilo VERDE de SunnyUI (el resto de la app usa naranja).
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

        FormClosed += (_, _) => _loadingTimer?.Dispose();
    }

    // IFormTemaClaro: reaplica el verde para pisar el UIStyleManager global del Main
    // (naranja) que puede re-estilizar este form al embeberse o al repintar.
    public void ReaplicarTema() => AplicarTemaVerde();

    private void AplicarTemaVerde()
    {
        Color verde = Color.FromArgb(110, 190, 40);
        Color verdeOsc = Color.FromArgb(70, 140, 25);
        this.BackColor = Color.White;
        this.Style = UIStyle.Green;
        this.TitleColor = verde;
        this.TitleForeColor = Color.White;
        if (BARRA_TITULO != null) BARRA_TITULO.BackColor = verde;
        if (tabControl1 == null) return;
        tabControl1.Style = UIStyle.Green;
        tabControl1.BackColor = verde;
        if (tabPage1 != null) tabPage1.BackColor = Color.White;
        tabControl1.TabBackColor = verdeOsc;
        tabControl1.TabSelectedColor = verde;
        tabControl1.TabSelectedForeColor = Color.White;
        tabControl1.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
        tabControl1.Invalidate();
        AjustarFuentesDatePicker();
    }

    // Los DateTimePicker (UIDatetimePicker) deben usar el mismo tamano de letra que
    // los TextBox; y las letras del calendario desplegable deben tener un tamano que
    // no se monte en los controles.
    private void AjustarFuentesDatePicker()
    {
        Font fuenteBase = txt_rollid_1?.Font ?? Font;
        Font fuenteCalendario = new Font(fuenteBase.FontFamily, 9f, FontStyle.Regular);
        var pickers = new[] { txt_fecha_emision, txt_fecha_produccion };
        foreach (var dp in pickers)
        {
            if (dp == null) continue;
            dp.Font = new Font(fuenteBase.FontFamily, fuenteBase.Size, fuenteBase.Style);
            AjustarFontCalendario(dp, fuenteCalendario);
        }
    }

    private void AjustarFontCalendario(UIDatetimePicker dp, Font fuente)
    {
        try
        {
            var fi = typeof(UIDatetimePicker).GetField("item",
                BindingFlags.NonPublic | BindingFlags.Instance);
            if (fi?.GetValue(dp) is Control cal)
            {
                SetFontRecursive(cal, fuente);
            }
        }
        catch
        {
        }
    }

    private void SetFontRecursive(Control parent, Font fuente)
    {
        foreach (Control c in parent.Controls)
        {
            SetFontRecursive(c, fuente);
        }
        parent.Font = fuente;
    }

    // IAsyncFormLoad: el FormManager llama a InitializeAsync ANTES de mostrar el form,
    // mostrando un FrmLoading mientras tanto. La carga de datos y el pintado de los
    // controles ocurren aqui, evitando la "pantalla en blanco" y el efecto de
    // minimizado a maximizado durante la carga.
    public async Task InitializeAsync()
    {
        try
        {
            Toggleloading(true);
            Ds = await Service.LoadDataOC();
        }
        finally
        {
            Toggleloading(false);
        }
        BsMaster.DataSource = Ds;
        BsMaster.DataMember = "DtMaster";
        BsDetails.DataSource = BsMaster;
        BsDetails.DataMember = R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS;
        //Enlace a datos Encabezado de la Orden Corte.
        HeaderBinding();
        BindingRollos();
        BindingCortes();

        Ds.Tables["DtMaster"]!.AcceptChanges();
        Ds.Tables["DtCortes"]!.AcceptChanges();
        Ds.Tables["DtRollos"]!.AcceptChanges();
        Ds.Tables["DtOperator"]!.AcceptChanges();
        Ds.Tables["DtCustomer"]!.AcceptChanges();

        BsMaster.MoveLast();
        UpdateStepIndicator();
        ContadorRegistros();

        // Por la forma correcta de suscribirse al evento PositionChanged:
        BsMaster.PositionChanged += BsMaster_PositionChanged;

        // Pestaña interna de "Rollos Cortados": bordes redondeados y tema VERDE de
        // SunnyUI (barra de titulo, pestanas del grid y tabs internos del detalle).
        AplicarTemaVerde();
        RedondearTabControlOrden();
        tabControl1.Resize += (s, e) => RedondearTabControlOrden();

        // Bordes redondeados en las grillas.
        grid_items.BorderStyle = BorderStyle.None;
        grid_cortes.BorderStyle = BorderStyle.None;
        RedondearControl(grid_items, 10);
        RedondearControl(grid_cortes, 10);
        grid_items.Resize += (s, e) => RedondearControl(grid_items, 10);
        grid_cortes.Resize += (s, e) => RedondearControl(grid_cortes, 10);

        // Al volver a esta pestana (cambiar y regresar), se re-aplican los redondeos y
        // el layout para que el formulario no quede "montado" / superpuesto.
        this.VisibleChanged += (s, e) =>
        {
            if (this.Visible)
            {
                AplicarTemaVerde();
                RedondearTabControlOrden();
                RedondearControl(grid_items, 10);
                RedondearControl(grid_cortes, 10);
                this.PerformLayout();
                this.Refresh();
            }
        };
    }

    private void FrmOrdenCorte_Load(object sender, EventArgs e)
    {
        if (this.TopLevel)
        {
            this.StartPosition = FormStartPosition.Manual;
            this.Location = new Point(155, 45);
        }

        // Estandariza el tamaño de fuente de los campos de fecha con el del resto de
        // campos (aqui el UIStyleManager ya aplicó su GlobalFont).
        Font fuenteCampos = txt_operador_id.Font;
        txt_fecha_emision.Font = fuenteCampos;
        txt_fecha_produccion.Font = fuenteCampos;
    }
    private void RedondearTabControlOrden()
    {
        if (tabControl1 == null) return;
        Rectangle rect = tabControl1.ClientRectangle;
        if (rect.Width <= 0 || rect.Height <= 0) return;
        int radio = 12;
        var gp = new GraphicsPath();
        gp.StartFigure();
        gp.AddArc(0, 0, radio, radio, 180, 90);
        gp.AddArc(rect.Width - radio, 0, radio, radio, 270, 90);
        gp.AddArc(rect.Width - radio, rect.Height - radio, radio, radio, 0, 90);
        gp.AddArc(0, rect.Height - radio, radio, radio, 90, 90);
        gp.CloseFigure();
        tabControl1.Region = new Region(gp);
    }
    private void RedondearControl(Control ctrl, int radio)
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
    #region ENLACE A DATOS
    private void HeaderBinding()
    {
        txt_numeroOC.DataBindings.Add("Text", BsMaster, "numero");
        // Fechas: DateTimePicker se enlaza por "Value" (no por "Text") y con formato corto.
        txt_fecha_emision.DateFormat = "dd/MM/yyyy";
        txt_fecha_produccion.DateFormat = "dd/MM/yyyy";
        txt_fecha_emision.DateCultureInfo = new CultureInfo("es-ES");
        txt_fecha_produccion.DateCultureInfo = new CultureInfo("es-ES");
        foreach (Control c in txt_fecha_emision.Controls) c.Font = new Font(c.Font.FontFamily, 7F);
        foreach (Control c in txt_fecha_produccion.Controls) c.Font = new Font(c.Font.FontFamily, 7F);
        txt_fecha_emision.DataBindings.Add("Value", BsMaster, "fecha", true, DataSourceUpdateMode.OnValidation);
        txt_fecha_produccion.DataBindings.Add("Value", BsMaster, "fecha_produccion", true, DataSourceUpdateMode.OnValidation);
        txt_fecha_emision.DataBindings["Value"]!.Format += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = DateTime.Today;
        };
        txt_fecha_produccion.DataBindings["Value"]!.Format += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = DateTime.Today;
        };
        txt_rollid_1.DataBindings.Add("Text", BsMaster, "rollid_1");

        txt_width1.DataBindings.Add("Text", BsMaster, "width_1");
        txt_length1.DataBindings.Add("Text", BsMaster, "lenght_1");

        txt_real1_width.DataBindings.Add("Text", BsMaster, "util1_real_width");
        txt_real1_length.DataBindings.Add("Text", BsMaster, "util1_real_lenght");
        txt_tipo_master.DataBindings.Add("Text", BsMaster, "master_tipo");

        txt_real2_width.DataBindings.Add("Text", BsMaster, "util2_real_width");
        txt_real2_length.DataBindings.Add("Text", BsMaster, "util2_real_lenght");
        txt_rollid_2.DataBindings.Add("Text", BsMaster, "rollid_2");
        txt_width2.DataBindings.Add("Text", BsMaster, "width_2");
        txt_length2.DataBindings.Add("Text", BsMaster, "lenght_2");
        txt_matrest1_width.DataBindings.Add("Text", BsMaster, "rest1_width");
        txt_matrest1_lenght.DataBindings.Add("Text", BsMaster, "rest1_lenght");
        txt_matrest2_width.DataBindings.Add("Text", BsMaster, "rest2_width");
        txt_matrest2_lenght.DataBindings.Add("Text", BsMaster, "rest2_lenght");
        txt_product_id.DataBindings.Add("Text", BsMaster, "product_id");
        txt_product_name.DataBindings.Add("Text", BsMaster, "product_Name");
        txt_operador_id.DataBindings.Add("Text", BsMaster, "operador_id", true, DataSourceUpdateMode.OnPropertyChanged);
        txt_operador_name.DataBindings.Add("Text", BsMaster, "nombre");
        txt_cust_id.DataBindings.Add("Text", BsMaster, "customer_id", true, DataSourceUpdateMode.OnPropertyChanged);
        txt_cust_name.DataBindings.Add("Text", BsMaster, "customer_name");
        txt_resta_corte.DataBindings.Add("Text", BsMaster, "resta_entrada");
        txt_largo_corte.DataBindings.Add("Text", BsMaster, "lenght_entrada");
        txt_plus1.DataBindings.Add("Text", BsMaster, "plus1_pies");
        txt_plus2.DataBindings.Add("Text", BsMaster, "plus2_pies");
        txt_long_cortar.DataBindings.Add("Text", BsMaster, "longitud_cortar");
        txt_cortes_ancho.DataBindings.Add("Text", BsMaster, "cortes_ancho");
        txt_vueltas1.DataBindings.Add("Text", BsMaster, "cortes_largo");
        txt_rollos_cortar1.DataBindings.Add("Text", BsMaster, "cant_rollos");
        txt_ancho_corte.DataBindings.Add("Text", BsMaster, "total_salida");
        txt_step.DataBindings.Add("Text", BsMaster, "step");
        txt_sellOrder.DataBindings.Add("Text", BsMaster, "sellOrder");
        txt_ubic.DataBindings.Add("Text", BsMaster, "Ubicacion");
        chk_desperdicio1.DataBindings.Add("Checked", BsMaster, "desperdicio", true);
        chk_desperdicio2.DataBindings.Add("Checked", BsMaster, "desperdicio2", true);


        chk_two_master.DataBindings.Add("Checked", BsMaster, "TwoMasters");

        txt_long_cortar2.DataBindings.Add("Text", BsMaster, "longitud_cortar2");
        txt_vueltas2.DataBindings.Add("Text", BsMaster, "vueltas2");
        txt_cantidad_rollos2.DataBindings.Add("Text", BsMaster, "cantidad_rollos2");
        chk_document_anul.DataBindings.Add("Checked", BsMaster, "anulada");

        //check desperdicios.
        chk_desperdicio1.DataBindings["Checked"]!.Format += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        chk_desperdicio1.DataBindings["Checked"]!.Parse += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        //check desperdicios2.
        chk_desperdicio2.DataBindings["Checked"]!.Format += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        chk_desperdicio2.DataBindings["Checked"]!.Parse += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        //check two-master.
        chk_two_master.DataBindings["Checked"]!.Format += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        chk_two_master.DataBindings["Checked"]!.Parse += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        chk_document_anul.DataBindings["Checked"]!.Format += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        chk_document_anul.DataBindings["Checked"]!.Parse += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        chk_two_master.DataBindings["Checked"]!.Parse += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };

        chk_ConfigVueltas.DataBindings.Add("Checked", BsMaster, "configvueltas");
        chk_ConfigVueltas.DataBindings["Checked"]!.Format += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };
        chk_ConfigVueltas.DataBindings["Checked"]!.Parse += (s, e) =>
        {
            if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
        };

    }
    private void BindingRollos()
    {
        //Enlace a datos de Grid-Rollos Cortados.
        grid_items.AutoGenerateColumns = false;
        grid_items.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        Font fGrid = new(this.Font.FontFamily, 9f);
        grid_items.DefaultCellStyle.Font = fGrid;
        grid_items.ColumnHeadersDefaultCellStyle.Font = new Font(this.Font.FontFamily, 9f, FontStyle.Bold);
        grid_items.RowTemplate.Height = 22;
        ADD_COLUMN_GRID("roll_number", 23, "#", "roll_number", grid_items);
        ADD_COLUMN_GRID("product_id", 50, "Product Id", "product_id", grid_items);
        ADD_COLUMN_GRID("product_name", 210, "Product Name", "product_name", grid_items);
        ADD_COLUMN_GRID("unique_code", 65, "Unique Code", "unique_code", grid_items);
        ADD_COLUMN_GRID("width", 65, "Width [Inch]", "width", grid_items);
        ADD_COLUMN_GRID("large", 75, "Length [Pies]", "large", grid_items);
        ADD_COLUMN_GRID("msi", 60, "MSI", "msi", grid_items);
        ADD_COLUMN_GRID("splice", 40, "Splice", "splice", grid_items);
        ADD_COLUMN_GRID("roll_id", 70, "Roll Id.", "roll_id", grid_items);
        ADD_COLUMN_GRID("code_person", 60, "Code Person.", "code_person", grid_items);
        ADD_COLUMN_GRID("vuelta", 60, "Vuelta", "vuelta", grid_items);

        DataGridViewComboBoxColumn estado = new()
        {
            HeaderText = "Status",
            DropDownWidth = 200,
            Width = 110,
            FlatStyle = FlatStyle.Flat,
            Name = "status",
            DisplayMember = "status",
            ValueMember = "status",
            DataPropertyName = "status"
        };

        //Agregar las opciones.
        estado.Items.AddRange("Ok.", "Mal Estado", "Reservado", "Observacion");
        grid_items.Columns.Add(estado);
        BsDetails.Sort = "roll_number";
        grid_items.Columns[4].DefaultCellStyle.Format = "N3";
        grid_items.Columns[5].DefaultCellStyle.Format = "N3";
        grid_items.Columns[6].DefaultCellStyle.Format = "N3";
        grid_items.DataSource = BsDetails;
    }
    private void BindingCortes()
    {
        // Enlace a datos de Grid-Cortes.
        BsCortes.DataSource = BsMaster;
        BsCortes.DataMember = "FK_ENCABEZADO_CORTES";
        grid_cortes.AutoGenerateColumns = false;
        Font fGridC = new(this.Font.FontFamily, 9f);
        grid_cortes.DefaultCellStyle.Font = fGridC;
        grid_cortes.ColumnHeadersDefaultCellStyle.Font = new Font(this.Font.FontFamily, 9f, FontStyle.Bold);
        grid_cortes.RowTemplate.Height = 22;
        ADD_COLUMN_GRID("it", 30, "It.", "num", grid_cortes);
        ADD_COLUMN_GRID("width", 80, "Width [INCH]", "width", grid_cortes);
        ADD_COLUMN_GRID("lenght", 80, "Lenght [PIES]", "lenght", grid_cortes);
        ADD_COLUMN_GRID("msi", 80, "Msi", "msi", grid_cortes);
        grid_cortes.Columns[1].DefaultCellStyle.Format = "N3";
        grid_cortes.Columns[2].DefaultCellStyle.Format = "N3";
        grid_cortes.Columns[3].DefaultCellStyle.Format = "N3";
        grid_cortes.DataSource = BsCortes;
    }
    private static void ADD_COLUMN_GRID(string name, int size, string title, string field_bd, DataGridView grid)
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
    #endregion

    #region CALCULAR ORDEN-CORTE



    private async void Btn_buscar_rollid1_Click(object sender, EventArgs e)
    {
        using Frm_RollId frmrollid = new(Service);

        frmrollid.ShowDialog();
        if (frmrollid.MasterRoll != null)
        {
            string rollidAnterior = txt_rollid_1.Text.Trim();
            string rollidNuevo = frmrollid.MasterRoll.Roll_Id;

            if (rollidNuevo != rollidAnterior && !string.IsNullOrWhiteSpace(txt_numeroOC.Text)
                && double.TryParse(txt_real1_length.Text, out double consumoActual) && consumoActual > 0)
            {
                bool esDesperdicio = chk_desperdicio1.Checked;
                double consumoDesperdicio = esDesperdicio && double.TryParse(txt_matrest1_lenght.Text, out double desp)
                    ? desp : 0;
                string tipoAnterior = txt_tipo_master.Text.Trim();
                string tipoNuevo = frmrollid.MasterRoll.Tipo_mov;

                DialogResult confirmar = MessageBox.Show(
                    $"El master {rollidAnterior} tiene {consumoActual:N2} pies consumidos por la orden {txt_numeroOC.Text}.\n\n" +
                    "Al cambiar el master, el material se devolverá al inventario del master anterior y se restará " +
                    $"del consumo del nuevo master {rollidNuevo}.\n\n¿Desea continuar?",
                    "Reasignación de master", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmar != DialogResult.Yes)
                {
                    frmrollid.Dispose();
                    return;
                }

                bool ok = await Service.ReasignarConsumoMasterAsync(
                    txt_numeroOC.Text, rollidAnterior, rollidNuevo,
                    consumoActual, consumoDesperdicio, esDesperdicio, tipoAnterior, tipoNuevo);

                if (!ok)
                {
                    frmrollid.Dispose();
                    return;
                }
            }

            Rollid_master = rollidNuevo;
            txt_rollid_1.Text = rollidNuevo;
            txt_width1.Text = frmrollid.MasterRoll.Width.ToString("N2");
            txt_length1.Text = frmrollid.MasterRoll.Length.ToString("N2");
            txt_real1.Text = frmrollid.MasterRoll.Length.ToString("N2");
            txt_product_id.Text = frmrollid.MasterRoll.Product_Id;
            txt_product_name.Text = frmrollid.MasterRoll.Product_Name;
            TipoMovimiento = frmrollid.MasterRoll.Tipo_mov;
            txt_tipo_master.Text = frmrollid.MasterRoll.Tipo_mov;

            CALCULATE_TOTAL_WIDTH_CORTES();
            CALCULATE_MATERIAL_RESTANTE();

            this.Validate();

            txt_rollid_1.Focus();
            txt_rollid_1.Select();

            BsMaster.EndEdit();
            Ds.Tables["DtMaster"]!.AcceptChanges();
            BsMaster.ResetBindings(false);

            frmrollid.Dispose();
        }
    }
    private void CALCULATE_MATERIAL_RESTANTE()
    {
        double material_rest_width = CalculosOrdenCorte.RestanteMaterial(Convert.ToDouble(txt_width1.Text), Convert.ToDouble(txt_real1_width.Text));

        double material_rest_len = CalculosOrdenCorte.RestanteMaterial(Convert.ToDouble(txt_length1.Text), Convert.ToDouble(txt_real1_length.Text));


        txt_matrest1_width.Text = material_rest_width.ToString("N2");

        txt_matrest1_lenght.Text = material_rest_len.ToString("N2");


    }
    private void CALCULATE_DATA_CORTES()
    {
        if (EditMode == 0) return;

        double long_cortar = txt_long_cortar.Text == string.Empty ? 0 : Convert.ToDouble(txt_long_cortar.Text);

        if (long_cortar <= 0) return;

        for (int i = 0; i <= grid_cortes.Rows.Count - 1; i++)
        {
            //llenar la columna length.
            grid_cortes.Rows[i].Cells["lenght"].Value = long_cortar;
            //calcular el msi.
            grid_cortes.Rows[i].Cells["msi"].Value = CalculosOrdenCorte.CalcularMsi(Convert.ToDouble(grid_cortes.Rows[i].Cells["width"].Value), Convert.ToDouble(grid_cortes.Rows[i].Cells["lenght"].Value));

            //ASIGNAR CORTES A LO ANCHO.
            txt_cortes_ancho.Text = grid_cortes.Rows.Count.ToString();

            if (chk_two_master.Checked)
            {
                double MatRes2 = CalculosOrdenCorte.RestanteMaterial(Convert.ToDouble(txt_length2.Text), Convert.ToDouble(txt_real2_length.Text));
                txt_matrest2_lenght.Text = MatRes2.ToString("N2");
            }
        }

        ACTUALIZAR_ROLLID_1();
    }
    private void ACTUALIZAR_ROLLID_1()
    {

        txt_matrest1_width.Text = txt_width1.Text;
        if (txt_real1_length.Text == "")
        {
            txt_real1_length.Text = "0";

        }
        if (txt_length1.Text == "")
        {
            txt_length1.Text = "0";

        }

        //Actualiza el material restante del RollId 1
        double num2 = CalculosOrdenCorte.RestanteMaterial(Convert.ToDouble(txt_length1.Text), Convert.ToDouble(txt_real1_length.Text));
        txt_matrest1_lenght.Text = num2.ToString("N2");




    }
    private void Txt_vueltas1_KeyUp(object sender, KeyEventArgs e)
    {
        ProgramarRecalculo(() =>
        {
            if (!string.IsNullOrEmpty(txt_vueltas1.Text) && !string.IsNullOrEmpty(txt_cortes_ancho.Text))
            {
                double num = CalculosOrdenCorte.LongitudTotal(Convert.ToDouble(txt_cortes_ancho.Text), Convert.ToDouble(txt_vueltas1.Text));
                txt_rollos_cortar1.Text = num.ToString();
            }
            CalcularLONGITUDACORTAR();
        });
    }
    private void GENERAR_ROLLOS_CORTADOS()
    {
        //VERIFICA SI EXISTEN ROLLOS ANTERIORES PARA BORRARLOS Y VOLVER A GENERARLOS.
        if (grid_items.Rows.Count > 0)
        {
            BorrarRollosCortadosHijos();
        }
        //CALCULO DE ROLLOS CORTADOS.
        if (EditMode == 2)
        {
            ParentRow = (DataRowView)BsMaster.Current!;
        }

        //ROLLOS DE MASTER1
        int vueltas = Convert.ToInt32(txt_vueltas1.Text);
        int numcortes = (grid_cortes.Rows.Count);
        int renglon = 1;
        for (int i = 1; i <= vueltas; i++)
        {
            int numvuelta = i;
            for (int j = 0; j <= numcortes - 1; j++)
            {
                RollosCortados = (DataRowView)BsDetails.AddNew()!;
                RollosCortados.BeginEdit();
                RollosCortados["roll_number"] = renglon;
                RollosCortados["product_id"] = txt_product_id.Text;
                RollosCortados["product_name"] = txt_product_name.Text;
                RollosCortados["unique_code"] = "0";
                RollosCortados["Width"] = grid_cortes.Rows[j].Cells["width"].Value;
                RollosCortados["large"] = grid_cortes.Rows[j].Cells["Lenght"].Value;
                RollosCortados["msi"] = grid_cortes.Rows[j].Cells["msi"].Value;
                RollosCortados["splice"] = 0;
                RollosCortados["roll_id"] = txt_rollid_1.Text;
                RollosCortados["code_person"] = "N/A";
                RollosCortados["vuelta"] = numvuelta;
                RollosCortados["status"] = "";
                RollosCortados.Row.SetParentRow(ParentRow.Row);
                InicializarColumnasObligatorias(RollosCortados);
                RollosCortados.EndEdit();
                renglon += 1;
            }
        }
        //CALCULO DE LOS ROLLOS SI TIENE USO DE SEGUNDO MASTER
        if (chk_two_master.Checked)
        {
            int vueltas2 = Convert.ToInt32(txt_vueltas2.Text);
            int numcortes2 = (grid_cortes.Rows.Count);
            int renglon2 = renglon;
            for (int i = 1; i <= vueltas2; i++)
            {
                int numvuelta2 = i;
                for (int j = 0; j <= numcortes2 - 1; j++)
                {
                    RollosCortados = (DataRowView)BsDetails.AddNew()!;
                    RollosCortados.BeginEdit();
                    RollosCortados["roll_number"] = renglon2;
                    RollosCortados["product_id"] = txt_product_id.Text;
                    RollosCortados["product_name"] = txt_product_name.Text;
                    RollosCortados["unique_code"] = "0";
                    RollosCortados["Width"] = grid_cortes.Rows[j].Cells["width"].Value;
                    RollosCortados["large"] = txt_long_cortar2.Text;
                    RollosCortados["msi"] = CalculosOrdenCorte.CalcularMsi(Convert.ToDouble(grid_cortes.Rows[j].Cells["width"].Value), Convert.ToDouble(txt_long_cortar2.Text));
                    RollosCortados["splice"] = 0;
                    RollosCortados["roll_id"] = txt_rollid_2.Text;
                    RollosCortados["code_person"] = "N/A";
                    RollosCortados["vuelta"] = numvuelta2;
                    RollosCortados["status"] = "";
                    RollosCortados.Row.SetParentRow(ParentRow.Row);
                    InicializarColumnasObligatorias(RollosCortados);
                    RollosCortados.EndEdit();
                    renglon2 += 1;
                }
            }
        }






        //BsDetails.Sort = "roll_number";
        if (grid_items.Rows.Count > 0)
        {
            grid_items.Focus();
            grid_items.Rows[0].Selected = true;
            grid_items.CurrentCell = grid_items.Rows[0].Cells[0];
        }
    }
    private void Btn_generar_rollos_Click(object sender, EventArgs e)
    {
        if (txt_rollid_1.Text == "0")
        {
            MessageBox.Show("Tiene que seleccionar un roll-id primero... ");
            return;
        }
        if (txt_product_id.Text == "0")
        {
            MessageBox.Show("Tiene que seleccionar un producto primero... ");
            return;
        }

        if (!ValidDefintionsCortes())
        {
            MessageBox.Show("debe definir los cortes primero...");
            return;
        }

        //en modo two-master se genera un segundo grupo de rollos a partir del
        //segundo master: exige su roll-id y una longitud/vueltas validas.
        if (chk_two_master.Checked)
        {
            if (txt_rollid_2.Text == string.Empty || txt_rollid_2.Text == "0")
            {
                MessageBox.Show("Tiene que seleccionar el roll-id del segundo master primero...");
                return;
            }
            if (txt_vueltas2.Value <= 0)
            {
                MessageBox.Show("Debe indicar el numero de vueltas del segundo master...");
                return;
            }
            if (!double.TryParse(txt_long_cortar2.Text, out double long2) || long2 <= 0)
            {
                MessageBox.Show("Debe indicar una longitud a cortar valida para el segundo master...");
                return;
            }
        }

        if (txt_vueltas1.Value <= 0)
        {
            MessageBox.Show("Debe indicar el numero de vueltas...");
            return;
        }
        if (!double.TryParse(txt_long_cortar.Text, out double long1) || long1 <= 0)
        {
            MessageBox.Show("Debe indicar una longitud a cortar valida...");
            return;
        }

        GENERAR_ROLLOS_CORTADOS();

        grid_items.ReadOnly = false;

        foreach (DataGridViewRow row in grid_items.Rows)
        {
            if (row.Cells["status"] is DataGridViewComboBoxCell comboCell)
            {
                comboCell.Value = comboCell.Items[0]; // Asignar la primera opci�n  
            }
        }
    }
    // Al modificar la OC (vueltas, longitud a cortar o cortes) se regeneran
    // automaticamente los datos y la cantidad de rollos cortados a producir.
    private void RegenerarRollosEnEdicion()
    {
        if (EditMode != 2) return;
        if (txt_rollid_1.Text == "0") return;
        if (txt_product_id.Text == "0") return;
        if (!ValidDefintionsCortes()) return;
        if (!int.TryParse(txt_vueltas1.Text, out int v1) || v1 <= 0) return;
        if (!double.TryParse(txt_long_cortar.Text, out double l1) || l1 <= 0) return;
        if (chk_two_master.Checked)
        {
            if (txt_rollid_2.Text == string.Empty || txt_rollid_2.Text == "0") return;
            if (!int.TryParse(txt_vueltas2.Text, out int v2) || v2 <= 0) return;
            if (!double.TryParse(txt_long_cortar2.Text, out double l2) || l2 <= 0) return;
        }

        GENERAR_ROLLOS_CORTADOS();
    }
    private void CALCULATE_TOTAL_WIDTH_CORTES()
    {
        double num = 0;
        for (int i = 0; i <= grid_cortes.Rows.Count - 1; i++)
        {
            object? w = grid_cortes.Rows[i].Cells["width"].Value;
            num += (w != null && w != DBNull.Value && double.TryParse(w.ToString(), out double anchoCorte)) ? anchoCorte : 0;
            txt_ancho_corte.Text = num.ToString();
            txt_real1_width.Text = num.ToString();
            // si estoy en modo two-master
            if (chk_two_master.Checked)
            {
                txt_real2_width.Text = num.ToString();
            }
        }
    }
    private void Grid_cortes_CellEndEdit(object sender, DataGridViewCellEventArgs e)
    {

        txt_cortes_ancho.Text = grid_cortes.Rows.Count.ToString();

        //calcular el msi por corte.
        object? wCell = grid_cortes.Rows[e.RowIndex].Cells["width"].Value;
        object? lCell = grid_cortes.Rows[e.RowIndex].Cells["lenght"].Value;
        double wCorte = 0;
        double lCorte = 0;
        bool wOk = wCell != null && wCell != DBNull.Value && double.TryParse(wCell.ToString(), out wCorte);
        bool lOk = lCell != null && lCell != DBNull.Value && double.TryParse(lCell.ToString(), out lCorte);
        if (wOk && lOk)
        {
            grid_cortes.Rows[e.RowIndex].Cells["msi"].Value = CalculosOrdenCorte.CalcularMsi(wCorte, lCorte);
        }

        CALCULATE_TOTAL_WIDTH_CORTES();
        ACTUALIZAR_ROLLID_1();
        CALCULATE_MATERIAL_RESTANTE();
        CALCULAR_TOTAL_ROLLOS_CORTAR();
        RegenerarRollosEnEdicion();
    }

    // P1-4: debounce de recalculados por teclado. Se cancela la recarga previa para
    // ejecutar el recalculo una sola vez tras 300 ms sin pulsaciones.
    private CancellationTokenSource? _debounceCts;
    private void ProgramarRecalculo(Action action)
    {
        _debounceCts?.Cancel();
        var cts = new CancellationTokenSource();
        _debounceCts = cts;
        var token = cts.Token;
        _ = EjecutarTrasPausaSiNoCancelado(action, token);
    }
    private static async Task EjecutarTrasPausaSiNoCancelado(Action action, CancellationToken token)
    {
        try
        {
            await Task.Delay(300, token);
            if (!token.IsCancellationRequested) action();
        }
        catch (OperationCanceledException)
        {
        }
    }

    private void CancelarRecalculosPendientes()
    {
        _debounceCts?.Cancel();
        _debounceCts?.Dispose();
        _debounceCts = null;
    }

    private void CalcularLONGITUDACORTAR()
    {
        if (txt_long_cortar.Text == string.Empty) return;

        double num = CalculosOrdenCorte.LongitudTotal(Convert.ToDouble(txt_long_cortar.Text),
            Convert.ToDouble(txt_vueltas1.Value));

        txt_largo_corte.Text = num.ToString();

        txt_real1_length.Text = num.ToString();

        //si esta en modo two-master.
        if (chk_two_master.Checked)
        {
            if (txt_long_cortar2.Text == string.Empty) txt_long_cortar2.Text = "0";
            if (txt_vueltas2.Text == string.Empty) txt_vueltas2.Value = 0;

            double ConsumoRealM2 = CalculosOrdenCorte.LongitudTotal(Convert.ToDouble(txt_long_cortar2.Text),
            Convert.ToDouble(txt_vueltas2.Value));

            txt_real2_length.Text = ConsumoRealM2.ToString();

        }
    }
    private void Txt_vueltas1_ValueChanged_1(object sender, EventArgs e)
    {

        CalcularConsumosMaster1();
    }

    private void CalcularConsumosMaster1()
    {
        if (EditMode == 0) return;

        CalcularLONGITUDACORTAR();
        CALCULAR_TOTAL_ROLLOS_CORTAR();

        //El grid de rollos se regenera automaticamente con los nuevos datos.
        RegenerarRollosEnEdicion();

        ACTUALIZAR_ROLLID_1();
    }
    private void CALCULAR_TOTAL_ROLLOS_CORTAR2()
    {
        if (txt_cortes_ancho.Text == "")
        {
            txt_cortes_ancho.Text = "0";
        }
        //Multiplicacion de las vueltas x los cortes son los rollos totales a producir.
        int num = CalculosOrdenCorte.TotalRollos(Convert.ToInt32(txt_vueltas2.Value), Convert.ToInt32(txt_cortes_ancho.Text));
        txt_cantidad_rollos2.Text = num.ToString();

    }

    private void CALCULAR_TOTAL_ROLLOS_CORTAR()
    {
        if (txt_cortes_ancho.Text == "")
        {
            txt_cortes_ancho.Text = "0";
        }
        //Multiplicacion de las vueltas x los cortes son los rollos totales a producir.
        int num = CalculosOrdenCorte.TotalRollos(Convert.ToInt32(txt_vueltas1.Value), Convert.ToInt32(txt_cortes_ancho.Text));
        txt_rollos_cortar1.Text = num.ToString();
    }
    private void BorrarRollosCortadosHijos()
    {

        if (EditMode == 0) return;
        if (BsMaster.Current == null) return;


        Ds.EnforceConstraints = false;
        BsMaster.EndEdit();
        BsCortes.EndEdit();
        BsDetails.EndEdit();

        // Obtener las filas hijas actuales (excluye eliminadas)
        var filasHijas = BuscarItemsDetailsOrden();

        foreach (var filaHija in filasHijas)
        {
            if (filaHija.RowState != DataRowState.Deleted && filaHija.RowState != DataRowState.Detached)
            {
                filaHija.Delete(); // Marca como Deleted
            }
        }

        Ds.Tables["DtMaster"]!.AcceptChanges();
        Ds.Tables["DtRollos"]!.AcceptChanges();
        Ds.Tables["DtCortes"]!.AcceptChanges();
        BsMaster.ResetBindings(false);
        BsDetails.ResetBindings(false);
        BsCortes.ResetBindings(false);
        Ds.EnforceConstraints = false;
    }
    private void Txt_long_cortar_KeyUp(object sender, KeyEventArgs e)
    {
        ProgramarRecalculo(() =>
        {
            CalcularConsumosMaster1();
            CALCULATE_DATA_CORTES();
        });

    }


    #endregion

    #region FORMS UI
    private void CloseUIFormAddNewMode()
    {
        //4.- Actualizar la UI.
        RefrescarUI();

        //Crear el txt de rollos cortados
        ExportDataService.ExportTxtFormatRollosCortados(BuscarItemsDetailsOrden(), chk_generartxt_rc.Checked, Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), false);

        //restaurar el color de los textbox al salir del modo edicion.
        ResaltarControlesEditables(false);

        EditMode = 0;
    }
    private void CloseUIFormUpdateMode()
    {
        //configurar la barra de herramientas
        bot_guardar.Enabled = false;
        bot_primero.Enabled = true;
        bot_siguiente.Enabled = true;
        bot_anterior.Enabled = true;
        bot_ultimo.Enabled = true;
        bot_accion.Enabled = true;
        bot_exportar.Enabled = true;
        btn_buscar_orden.Enabled = true;
        bot_buscarOrders.Enabled = true;
        bot_cancelar.Enabled = false;

        txt_fecha_emision.Enabled = false;
        txt_fecha_produccion.Enabled = false;


        btn_buscar_rollid1.Enabled = false;
        chk_desperdicio1.Enabled = false;
        btn_buscar_operador.Enabled = false;
        txt_sellOrder.ReadOnly = true;
        btn_generar_rollos.Enabled = false;
        txt_long_cortar.ReadOnly = true;
        txt_vueltas1.Enabled = false;
        txt_ubic.ReadOnly = true;
        grid_items.ReadOnly = true;

        btn_add_row_corte.Enabled = false;
        btn_delete_row_corte.Enabled = false;
        btn_buscar_customer.Enabled = false;

        label_ModoEdition.Visible = false;
        ICON_EDITMODE.Visible = false;

        foreach (DataGridViewRow row in grid_items.Rows)
        {
            row.DefaultCellStyle.BackColor = Color.White;
        }

        //actualizo la ui 
        BsDetails.EndEdit();
        BsMaster.EndEdit();
        grid_items.EndEdit();

        //generar el txt para 
        ExportDataService.ExportTxtFormatRollosCortados(BuscarItemsDetailsOrden(), false, Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), Convert.ToDateTime(txt_fecha_emision.Text).ToShortDateString(), false);

        //restaurar el color de los textbox al salir del modo edicion.
        ResaltarControlesEditables(false);

        //Modo Solo-Lectura.
        EditMode = 0;

    }
    private void ContadorRegistros()
    {
        registros.Text = "Registros: " + (BsMaster.Position + 1) + "/" + BsMaster.Count.ToString();
    }
    private void UpdateOptionMenuAction(bool b1, bool b2, bool b3, bool b4, bool b5)
    {
        opt_send_production.Enabled = b1;
        opt_etiquetar_orden.Enabled = b2;
        opt_aprobar_orden.Enabled = b3;
        opt_cerrar_orden.Enabled = b4;
        opt_modif_orden.Enabled = b5;
    }
    private void Txt_vueltas1_Enter(object sender, EventArgs e)
    {
        var numeric = (NumericUpDown)sender;

        if (string.IsNullOrWhiteSpace(numeric.Text))
            numeric.Text = "0";

        numeric.BeginInvoke(new Action(() =>
        {
            numeric.Select(0, numeric.Text.Length);
        }));

    }
    private void Txt_long_cortar_Enter(object sender, EventArgs e)
    {
        var txt = (UITextBox)sender;

        if (string.IsNullOrWhiteSpace(txt.Text))
            txt.Text = "0";

        txt.BeginInvoke(new Action(() => txt.SelectAll()));
    }
    private void Grid_cortes_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
        var grid = (DataGridView)sender;

        // Mostrar contexto del error
        MessageBox.Show($"Error en celda [{e.RowIndex}, {e.ColumnIndex}]: {e.Context}",
                        "Error de datos", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        // Si es una excepci�n de restricci�n (por ejemplo, clave duplicada o nulo no permitido)
        if (e.Exception is ConstraintException)
        {
            grid.Rows[e.RowIndex].ErrorText = "Error de restricci�n en los datos.";
            grid.Rows[e.RowIndex].Cells[e.ColumnIndex].ErrorText = "Dato inv�lido.";
            e.ThrowException = false; // Evita que se propague la excepci�n
        }

        // Puedes cancelar el error si no quieres que se propague
        e.Cancel = true;

    }
    private void SoloNumeros_KeyPress(object sender, KeyPressEventArgs e)
    {
        // Permitir solo n�meros y control (Backspace, Supr, etc.)
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
        {
            e.Handled = true;
        }
    }
    private void Txt_long_cortar_KeyPress(object sender, KeyPressEventArgs e)
    {
        if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
        {
            e.Handled = true;
        }
    }
    private void Bot_primero_Click(object sender, EventArgs e)
    {
        BsMaster.MoveFirst();
        UpdateStepIndicator();
        ContadorRegistros();
    }

    private void Bot_anterior_Click(object sender, EventArgs e)
    {
        BsMaster.MovePrevious();
        UpdateStepIndicator();
        ContadorRegistros();
    }

    private void Bot_siguiente_Click(object sender, EventArgs e)
    {
        BsMaster.MoveNext();
        UpdateStepIndicator();
        ContadorRegistros();
    }

    private void Bot_ultimo_Click(object sender, EventArgs e)
    {
        BsMaster.Position = BsMaster.Count + 1;
        UpdateStepIndicator();
        ContadorRegistros();
    }
    private void Txt_plus1_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txt_plus1.Text))
        {
            txt_plus1.Text = "0";
        }
    }

    private void Txt_menos1_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txt_menos1.Text))
        {
            txt_menos1.Text = "0";
        }
    }

    private void Txt_plus2_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txt_plus2.Text))
        {
            txt_plus2.Text = "0";
        }

    }
    private void RefrescarUI()
    {
        BsMaster.MoveLast();
        BsMaster.EndEdit();
        BsMaster.ResetBindings(false);
        BsDetails.EndEdit();
        BsDetails.ResetBindings(false);
        Ds.Tables["DtMaster"]!.AcceptChanges();
        Ds.Tables["DtRollos"]!.AcceptChanges();
        grid_items.Refresh();
        CerrarForms();
        ContadorRegistros();
    }
    private void Txt_menos2_TextChanged(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txt_menos2.Text))
        {
            txt_menos2.Text = "0";
        }
    }
    #endregion

    #region ACTIONS ADD-UPDATE-CANCEL
    private void CREATE_CORTES()
    {
        Cortes = [];
        for (int i = 0; i <= grid_cortes.Rows.Count - 1; i++)
        {
            //var codePersonValue = grid_cortes.Rows[i].Cells["code_person"].Value;
            //string codePerson = codePersonValue?.ToString() ?? string.Empty; // Ensure no null reference

            Corte corte = new()
            {
                Numero = i + 1,
                Width = Convert.ToDouble(grid_cortes.Rows[i].Cells["width"].Value),
                Length = Convert.ToDouble(grid_cortes.Rows[i].Cells["lenght"].Value),
                Msi = Convert.ToDouble(grid_cortes.Rows[i].Cells["msi"].Value),
                Orden = Convert.ToInt32(txt_numeroOC.Text)
            };
            Cortes.Add(corte);
        }

    }
    private void CREATE_DETALLE_ORDEN()
    {
        Detalle = [];
        for (int i = 0; i <= grid_items.Rows.Count - 1; i++)
        {
            var rollNumberValue = grid_items.Rows[i].Cells["roll_number"].Value;
            var uniqueCodeValue = grid_items.Rows[i].Cells["unique_code"].Value;
            var productIdValue = grid_items.Rows[i].Cells["product_id"].Value;
            var productNameValue = grid_items.Rows[i].Cells["product_name"].Value;
            var widthValue = grid_items.Rows[i].Cells["width"].Value;
            var lengthValue = grid_items.Rows[i].Cells["large"].Value;
            var msiValue = grid_items.Rows[i].Cells["msi"].Value;
            var spliceValue = grid_items.Rows[i].Cells["splice"].Value;
            var rollIdValue = grid_items.Rows[i].Cells["roll_id"].Value;
            var codePersonValue = grid_items.Rows[i].Cells["code_person"].Value;
            var statusRollo = grid_items.Rows[i].Cells["status"].Value;
            var vuelta = grid_items.Rows[i].Cells["vuelta"].Value;


            RolloCortado rollo = new()
            {
                Numero = txt_numeroOC.Text?.ToString() ?? string.Empty,
                UniqueCode = uniqueCodeValue?.ToString() ?? string.Empty,
                Product_Id = productIdValue?.ToString() ?? string.Empty,
                Product_Name = productNameValue?.ToString() ?? string.Empty,
                RollNumber = rollNumberValue is null or DBNull ? 0 : Convert.ToInt32(rollNumberValue),
                Width = widthValue is null or DBNull ? 0 : Convert.ToDouble(widthValue),
                Length = lengthValue is null or DBNull ? 0 : Convert.ToDouble(lengthValue),
                Msi = msiValue is null or DBNull ? 0 : Convert.ToDouble(msiValue),
                Splice = spliceValue is null or DBNull ? 0 : Convert.ToInt32(spliceValue),
                Roll_Id = rollIdValue?.ToString() ?? string.Empty,
                Cantidad_despacho = 0,
                Cantidad = 0,
                Tipo = "CORTADO",
                Paleta = string.Empty,
                Code_Person = codePersonValue?.ToString() ?? string.Empty,
                Ubicacion = ".",
                Status = statusRollo?.ToString() ?? string.Empty,
                Disponible = false,
                Vuelta = vuelta is null or DBNull ? 0 : Convert.ToInt32(vuelta)
            };
            Detalle.Add(rollo);
        }
    }
    private void CREATE_HEADER_ORDEN()
    {
        string hostName = Dns.GetHostName();

        // Filtramos solo IPv4
        string ip = Dns.GetHostAddresses(hostName)
                       .FirstOrDefault(a => a.AddressFamily == AddressFamily.InterNetwork)?
                       .ToString() ?? "N/A";



        Orden = new()
        {
            Numero = Convert.ToInt32(txt_numeroOC.Text),
            Fecha = Convert.ToDateTime(txt_fecha_emision.Text),
            Fecha_produccion = Convert.ToDateTime(txt_fecha_produccion.Text),
            Rollid_1 = txt_rollid_1.Text,
            Width_1 = Convert.ToDecimal(txt_width1.Text),
            Lenght_1 = Convert.ToDecimal(txt_length1.Text),
            Util1_Real_Width = Convert.ToDouble(txt_real1_width.Text),
            Util1_real_Lenght = Convert.ToDouble(txt_real1_length.Text),
            Rest1_width = Convert.ToDouble(txt_matrest1_width.Text),
            Rest1_lenght = Convert.ToDouble(txt_matrest1_lenght.Text),
            Rollid_2 = txt_rollid_2.Text,
            Width_2 = Convert.ToDecimal(txt_width2.Text),
            Lenght_2 = Convert.ToDecimal(txt_length2.Text),
            Util2_Real_Width = Convert.ToDouble(txt_real2_width.Text),
            Util2_real_Lenght = Convert.ToDouble(txt_real2_length.Text),
            Rest2_width = Convert.ToDouble(txt_matrest2_width.Text),
            Rest2_lenght = Convert.ToDouble(txt_matrest2_lenght.Text),
            Product_id = txt_product_id.Text,
            Product_name = txt_product_name.Text,
            Operador_id = Guid.Parse(txt_operador_id.Text),
            Nombre_operador = txt_operador_name.Text,
            Customer_Id = Guid.Parse(txt_cust_id.Text),
            Customer_Name = txt_cust_name.Text ?? string.Empty,
            Longitud_Cortar = Convert.ToDouble(txt_long_cortar.Text),
            Cortes_Largo = Convert.ToInt32(txt_vueltas1.Value),
            Cortes_Largo2 = Convert.ToInt32(txt_vueltas2.Value),
            Cortes_Ancho = Convert.ToInt32(txt_cortes_ancho.Text),
            Cantidad_Rollos = Convert.ToInt32(txt_rollos_cortar1.Text),
            //Cantidad_Rollos2 = Convert.ToInt32(txt_rollos_cortar2.Text == "" ? 0 : txt_rollos_cortar2.Text),
            Anulada = false,
            Procesado = false,
            CloseDocument = false,
            Descartable1_pies = 0,
            Descartable2_pies = 0,
            Total_Inch_Ancho = Convert.ToDouble(txt_ancho_corte.Text == "" ? 0 : txt_ancho_corte.Text),
            Lenght_Master_Real = Convert.ToDouble(txt_real1.Text == "" ? 0 : txt_real1.Text),
            Master_lenght2_Real = Convert.ToDouble(txt_real2.Text == "" ? 0 : txt_real2.Text),
            LastUpdate = DateTime.Now,
            FechaAutorize = DateTime.Now,
            Step = 2,
            ToAutorize = "",
            Note = "",
            Plus1_pies = Convert.ToDecimal(txt_plus1.Text),
            Plus2_pies = Convert.ToDecimal(txt_plus2.Text),
            Tipo_Mov1 = "",
            Tipo_Mov2 = "",
            Rollo_unificado = chk_two_master.Checked,
            Lenght_entrada = 0,
            Real_usado_r1 = 0,
            Real_usado_r2 = 0,
            Restante_rollid1 = txt_matrest1_lenght.Text,
            Restante_rollid2 = txt_matrest2_lenght.Text,
            SellOrder = txt_sellOrder.Text == string.Empty ? "0" : txt_sellOrder.Text,
            Desperdicio = chk_desperdicio1.Checked,
            Master_Tipo = TipoMovimiento,
            Ubicacion = txt_ubic.Text == string.Empty ? "n/t" : txt_ubic.Text,
            TwoMasters = chk_two_master.Checked,
            Longitud_Cortar2 = Convert.ToDouble(txt_long_cortar2.Text == "" ? 0 : txt_long_cortar2.Text),
            Vueltas2 = Convert.ToInt32(txt_vueltas2.Value),
            Cantidad_rollos2 = Convert.ToInt32(txt_cantidad_rollos2.Text == "" ? 0 : txt_cantidad_rollos2.Text),
            Desperdicio2 = chk_desperdicio2.Checked,
            MachineName = Environment.MachineName,
            DireccionIp = ip
        };
    }
    private bool GuardarOrdeAddMode()
    {
        // Consecutivo atomico: se avanza y se obtiene el numero definitivo en una sola operacion,
        // eliminando la colision entre usuarios que crean ordenes al mismo tiempo.
        int numero = Service.GetAndIncrementConsecOC();
        Orden.Numero = numero;
        this.Invoke(() => txt_numeroOC.Text = numero.ToString());

        // Guardado del documento completo en UNA sola transaccion (encabezado + cortes + rollos).
        return Service.GuardarOrdenCompleta(Orden, Cortes, Detalle);
    }
    private void GuardarOrderUpdateMode()
    {
        bool actualizado = Service.UpdateOrdenCorte(Orden);
        if (actualizado)
        {
            MessageBox.Show("Se actualizo la orden de corte correctamente.");
        }
    }


    private async void Bot_guardar_Click(object sender, EventArgs e)
    {
        try
        {
            if (EditMode == 1)
            {
                //1.- Validar los datos del formulario.
                if (!Validar()) return;

                var sw = System.Diagnostics.Stopwatch.StartNew();
                Toggleloading(true);
                await Task.Run(() => GuardarOrderNew());
                await EsperarMinimoLoading(sw);
                Toggleloading(false);
                // Se dejan los datos en pantalla (no se borra el form) pero los
                // controles pasan a solo lectura.
                ActivarModoSoloLectura();
            }
            if (EditMode == 2)
            {
                if (grid_items.Rows.Count <= 0)
                {
                    MessageBox.Show("No hay rollos cortados para guardar...");
                    return;
                }

                var sw = System.Diagnostics.Stopwatch.StartNew();
                Toggleloading(true);
                await Task.Run(() => GuardarOrderUpdate());
                await EsperarMinimoLoading(sw);
                Toggleloading(false);
                // Se dejan los datos en pantalla (no se borra el form) pero los
                // controles pasan a solo lectura.
                ActivarModoSoloLectura();
            }
        }
        catch (Exception ex)
        {
            Toggleloading(false);
            MessageBox.Show("Ocurrió un error al guardar la orden: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    // Mantiene el loading visible al menos 1 segundo para que alcance a verse, aunque
    // el guardado termine antes.
    private async Task EsperarMinimoLoading(System.Diagnostics.Stopwatch sw)
    {
        const int minimoMs = 1000;
        int transcurrido = (int)sw.ElapsedMilliseconds;
        int restante = minimoMs - transcurrido;
        if (restante > 0) await Task.Delay(restante);
    }

    // Pone los controles en modo solo lectura despues de guardar, sin borrar los
    // datos que quedaron en pantalla.
    private void ActivarModoSoloLectura()
    {
        // Barra de herramientas.
        bot_guardar.Enabled = false;
        bot_primero.Enabled = true;
        bot_siguiente.Enabled = true;
        bot_anterior.Enabled = true;
        bot_ultimo.Enabled = true;
        bot_accion.Enabled = true;
        bot_exportar.Enabled = true;
        btn_buscar_orden.Enabled = true;
        bot_buscarOrders.Enabled = true;
        bot_cancelar.Enabled = false;

        // Campos principales.
        txt_fecha_emision.Enabled = false;
        txt_fecha_produccion.Enabled = false;
        btn_buscar_rollid1.Enabled = false;
        chk_desperdicio1.Enabled = false;
        btn_buscar_operador.Enabled = false;
        txt_sellOrder.ReadOnly = true;
        btn_generar_rollos.Enabled = false;
        txt_long_cortar.ReadOnly = true;
        txt_vueltas1.Enabled = false;
        txt_ubic.ReadOnly = true;
        grid_items.ReadOnly = true;

        btn_add_row_corte.Enabled = false;
        btn_delete_row_corte.Enabled = false;
        btn_buscar_customer.Enabled = false;

        label_ModoEdition.Visible = false;
        ICON_EDITMODE.Visible = false;

        foreach (DataGridViewRow row in grid_items.Rows)
        {
            row.DefaultCellStyle.BackColor = Color.White;
        }

        EditMode = 0;
    }
    private async Task GuardarOrderUpdate()
    {

        if (!ValidarDocumento()) return;

        if (txt_operador_id.Text == "" && txt_cust_id.Text == "")
        {
            MessageBox.Show("debe introducir los datos del operador y cliente...");
            return;
        }
        //Modificaciones en el header de la OC.

        Orden orden = new()
        {
            Numero = Convert.ToInt32(txt_numeroOC.Text),
            Fecha = Convert.ToDateTime(txt_fecha_emision.Text),
            Fecha_produccion = Convert.ToDateTime(txt_fecha_produccion.Text),
            Rollid_1 = txt_rollid_1.Text,
            Width_1 = Convert.ToDecimal(txt_width1.Text),
            Lenght_1 = Convert.ToDecimal(txt_length1.Text),
            Util1_Real_Width = Convert.ToDouble(txt_real1_width.Text),
            Util1_real_Lenght = Convert.ToDouble(txt_real1_length.Text),
            Rest1_width = Convert.ToDouble(txt_matrest1_width.Text),
            Rest2_lenght = Convert.ToDouble(txt_matrest2_lenght.Text),
            Product_id = txt_product_id.Text.ToString(),
            Product_name = txt_product_name.Text.ToString(),
            Desperdicio = chk_desperdicio1.Checked,
            Operador_id = new Guid(txt_operador_id.Text),
            Customer_Id = new Guid(txt_cust_id.Text),
            Longitud_Cortar = Convert.ToDouble(txt_long_cortar.Text),
            Cortes_Largo = Convert.ToInt32(txt_vueltas1.Value),
            Cortes_Ancho = Convert.ToInt32(txt_cortes_ancho.Text),
            Cantidad_Rollos = Convert.ToInt32(txt_rollos_cortar1.Text),
            SellOrder = txt_sellOrder.Text == string.Empty ? "0" : txt_sellOrder.Text,
            Ubicacion = txt_ubic.Text == string.Empty ? "n/t" : txt_ubic.Text,
            ConfigVueltas = chk_ConfigVueltas.Checked
        };
        CREATE_CORTES();
        orden.Cortes = Cortes;
        CREATE_DETALLE_ORDEN();
        orden.rollos = Detalle;

        //actualiza el encabezado de la OC.
        await Task.Run(() => Service.Update_Header_Documnet_OC(orden));

        //actualiza la base de datos los items rollos.
        //Service.Update_Items_Orden_Corte(Lista);

    }
    private void SaveDocument()
    {
        if (EditMode == 1)
        {
            if (GuardarOrdeAddMode())
            {
                // El consecutivo ya fue avanzado atomicamente dentro de GuardarOrdeAddMode,
                // por lo que ya no se actualiza por separado aqui.
            }
            else
            {
                MessageBox.Show("No se pudo guardar la orden. Verifique los datos e intente nuevamente.", "Error al guardar", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
    }
    private void GuardarOrderNew()
    {
        //2.- Crear los objetos (clases) de la Orden de Corte.
        CrearObjetoOrden();

        // 3.- Guardar el documento en la Base de Datos.
        SaveDocument();
    }

    // Asigna un valor por defecto (segun el tipo) a toda columna NOT NULL del esquema
    // que quede sin valor en una fila nueva. Evita System.Data.NoNullAllowedException
    // al hacer EndEdit (como ocurria con 'fecha'/'fecha_produccion' y ahora 'ubicacion').
    private static void InicializarColumnasObligatorias(DataRowView fila)
    {
        if (fila == null) return;
        foreach (DataColumn col in fila.Row.Table.Columns)
        {
            if (!col.AllowDBNull && !col.AutoIncrement &&
                (fila[col.ColumnName] == null || Convert.IsDBNull(fila[col.ColumnName])))
            {
                fila[col.ColumnName] = ValorPorDefectoColumna(col);
            }
        }
    }

    private static object ValorPorDefectoColumna(DataColumn col)
    {
        if (col.DataType == typeof(DateTime)) return DateTime.Now;
        if (col.DataType == typeof(bool)) return false;
        if (col.DataType == typeof(byte)) return (byte)0;
        if (col.DataType == typeof(short)) return (short)0;
        if (col.DataType == typeof(int)) return 0;
        if (col.DataType == typeof(long)) return 0L;
        if (col.DataType == typeof(decimal)) return 0m;
        if (col.DataType == typeof(double)) return 0d;
        if (col.DataType == typeof(float)) return 0f;
        if (col.DataType == typeof(Guid)) return Guid.Empty;
        if (col.DataType == typeof(byte[])) return Array.Empty<byte>();
        return string.Empty;
    }

    // Resalta los textbox de parametros con un rojo clarito mientras la orden esta en
    // modo edicion (PRODUCCION) y restaura su color original al salir del modo.
    // Los controles son de SunnyUI: pintan sobre FillColor (no BackColor), por eso se
    // aplica el color a traves de la propiedad FillColor.
    private static readonly Color ColorEdicionCampos = Color.FromArgb(255, 214, 216);
    private static readonly Color ColorEdicionGrid = Color.FromArgb(255, 192, 203);
    private static readonly Color ColorTituloVerde = Color.FromArgb(110, 190, 40);
    private static readonly Color ColorTituloEdicion = Color.FromArgb(230, 80, 60);

    private void ResaltarControlesEditables(bool activo)
    {
        Control[] parametros =
        [
            txt_fecha_emision, txt_fecha_produccion,
            txt_operador_id, txt_operador_name, txt_cust_id, txt_cust_name,
            txt_plus1, txt_menos1, txt_plus2, txt_menos2,
            txt_sellOrder, txt_long_cortar, txt_ubic,
            txt_vueltas1, txt_vueltas2,
            txt_rollid_1, txt_width1, txt_length1,
            btn_buscar_rollid1, btn_buscar_operador, btn_buscar_customer
        ];

        foreach (Control c in parametros)
        {
            if (c == null) continue;
            if (activo)
            {
                AplicarFillColor(c, ColorEdicionCampos);
            }
            else
            {
                RestaurarFillColor(c);
            }
        }

        // El grid de cortes y el de rollos cortados se pintan de rosado cuando la
        // orden entra en edicion y vuelven a blanco al salir.
        foreach (DataGridView grid in new[] { grid_cortes, grid_items })
        {
            if (grid == null) continue;
            Color colorGrid = activo ? ColorEdicionGrid : Color.White;
            foreach (DataGridViewRow row in grid.Rows)
            {
                row.DefaultCellStyle.BackColor = colorGrid;
            }
            // Las columnas nuevas que se agreguen en edicion deben heredar el color.
            grid.DefaultCellStyle.BackColor = colorGrid;
            grid.RowsDefaultCellStyle.BackColor = colorGrid;
        }

        // La capa de titulo de la orden de corte es el Panel BARRA_TITULO (su BackColor
        // verde es lo que se ve): pasa a rojo mientras se edita y vuelve a verde al salir.
        if (BARRA_TITULO != null)
        {
            BARRA_TITULO.BackColor = activo ? ColorTituloEdicion : ColorTituloVerde;
        }
        // Tambien se actualiza el TitleColor del formulario por si el titulo se mostrara.
        TitleColor = activo ? ColorTituloEdicion : ColorTituloVerde;
    }

    private static Color ObtenerFillColor(Control c)
    {
        Type tipo = c.GetType();
        if (!c.Enabled)
        {
            System.Reflection.PropertyInfo? dis = tipo.GetProperty("FillDisableColor");
            if (dis?.GetValue(c) is Color colorDisable)
            {
                return colorDisable;
            }
        }
        if (tipo.GetProperty("ReadOnly")?.GetValue(c) is true)
        {
            System.Reflection.PropertyInfo? ro = tipo.GetProperty("FillReadOnlyColor");
            if (ro?.GetValue(c) is Color colorRo)
            {
                return colorRo;
            }
        }
        System.Reflection.PropertyInfo? prop = c.GetType().GetProperty("FillColor");
        return prop?.GetValue(c) is Color color ? color : c.BackColor;
    }

    private void AplicarFillColor(Control c, Color color)
    {
        Type tipo = c.GetType();

        // Guarda los valores originales la primera vez que se resalta.
        if (!coloresOriginalesTextbox.ContainsKey(c))
        {
            coloresOriginalesTextbox[c] = ObtenerFillColor(c);
            coloresOriginalesDisable[c] = ObtenerColorActivo(c, "FillDisableColor", c.BackColor);
            styleCustomModeOriginales[c] = ObtenerBoolActivo(c, "StyleCustomMode");
            coloresOriginalesRect[c] = ObtenerColorActivo(c, "RectColor", c.BackColor);
            coloresOriginalesRectReadonly[c] = ObtenerColorActivo(c, "RectReadOnlyColor", c.BackColor);
        }

        SetColor(tipo, c, "FillColor", color);
        if (tipo.GetProperty("ReadOnly")?.GetValue(c) is true)
        {
            // Los textboxes en ReadOnly de SunnyUI se pintan con FillReadOnlyColor.
            SetColor(tipo, c, "FillReadOnlyColor", color);
            SetColor(tipo, c, "RectReadOnlyColor", color);
        }
        if (!c.Enabled)
        {
            // Los controles deshabilitados se pintan con FillDisableColor.
            SetColor(tipo, c, "FillDisableColor", color);
        }

        // El borde del campo tambien se pinta para que el resaltado sea muy visible.
        SetColor(tipo, c, "RectColor", color);

        // StyleCustomMode=true hace que el control use FillColor directamente en vez del estilo.
        SetBool(tipo, c, "StyleCustomMode", true);

        // Para controles sin FillColor (p.ej. Button) se pinta por BackColor.
        if (tipo.GetProperty("FillColor") == null)
        {
            c.BackColor = color;
        }
    }

    private void RestaurarFillColor(Control c)
    {
        if (!coloresOriginalesTextbox.TryGetValue(c, out Color original))
        {
            return;
        }

        Type tipo = c.GetType();
        SetColor(tipo, c, "FillColor", original);
        SetColor(tipo, c, "FillReadOnlyColor", original);
        SetColor(tipo, c, "FillDisableColor", coloresOriginalesDisable.TryGetValue(c, out Color disOriginal)
            ? disOriginal
            : original);
        SetColor(tipo, c, "RectColor", coloresOriginalesRect.TryGetValue(c, out Color rectOriginal)
            ? rectOriginal
            : original);
        SetColor(tipo, c, "RectReadOnlyColor", coloresOriginalesRectReadonly.TryGetValue(c, out Color rectRoOriginal)
            ? rectRoOriginal
            : original);
        SetBool(tipo, c, "StyleCustomMode", styleCustomModeOriginales.TryGetValue(c, out bool styleOriginal)
            && styleOriginal);

        if (tipo.GetProperty("FillColor") == null)
        {
            c.BackColor = original;
        }
    }

    private static Color ObtenerColorActivo(Control c, string nombre, Color fallback)
    {
        System.Reflection.PropertyInfo? p = c.GetType().GetProperty(nombre);
        return p?.GetValue(c) is Color color ? color : fallback;
    }

    private static bool ObtenerBoolActivo(Control c, string nombre)
    {
        System.Reflection.PropertyInfo? p = c.GetType().GetProperty(nombre);
        return p?.GetValue(c) is bool b && b;
    }

    private static void SetColor(Type tipo, Control c, string nombre, Color color)
    {
        System.Reflection.PropertyInfo? p = tipo.GetProperty(nombre);
        if (p != null && p.CanWrite)
        {
            p.SetValue(c, color);
        }
    }

    private static void SetBool(Type tipo, Control c, string nombre, bool valor)
    {
        System.Reflection.PropertyInfo? p = tipo.GetProperty(nombre);
        if (p != null && p.CanWrite)
        {
            p.SetValue(c, valor);
        }
    }

    private void Opt_create_document_Click(object sender, EventArgs e)
    {
        //1.- Inicialiozar el Documento de Orden de Corte.
        ParentRow = (DataRowView)BsMaster.AddNew()!;
        ParentRow.BeginEdit();
        ParentRow["numero"] = Service.BuscarConsecOC();
        ParentRow["rollid_1"] = "0";
        ParentRow["rollid_2"] = "0";
        ParentRow["width_1"] = "0";
        ParentRow["lenght_1"] = "0";
        ParentRow["width_2"] = "0";
        ParentRow["lenght_2"] = "0";
        ParentRow["util1_real_width"] = "0";
        ParentRow["util1_real_lenght"] = "0";
        ParentRow["rest1_width"] = "0";
        ParentRow["rest1_lenght"] = "0";
        ParentRow["rest2_width"] = "0";
        ParentRow["rest2_lenght"] = "0";
        ParentRow["util2_real_width"] = "0";
        ParentRow["util2_real_lenght"] = "0";
        ParentRow["plus1_pies"] = "0";
        ParentRow["plus2_pies"] = "0";
        ParentRow["longitud_cortar"] = "0";
        ParentRow["cortes_ancho"] = "0";
        ParentRow["cortes_largo"] = "0";
        ParentRow["cant_rollos"] = "0";
        ParentRow["cant_rollos2"] = "0";

        // La columna fecha es NOT NULL en el esquema inferido del DataTable: asignar
        // valores por defecto antes de EndEdit evita NoNullAllowedException al crear.
        ParentRow["fecha"] = DateTime.Now;
        ParentRow["fecha_produccion"] = DateTime.Now;

        ParentRow["TwoMasters"] = false;


        txt_menos1.Text = "0";
        txt_real1.Text = "0";
        txt_real2.Text = "0";
        txt_plus1.Text = "0";
        txt_menos2.Text = "0";

        //txt_rollos_cortar2.Text = "0";
        txt_ancho_corte.Text = "0";

        // Garantiza que ninguna columna NOT NULL quede sin valor al insertar la fila.
        InicializarColumnasObligatorias(ParentRow);

        ParentRow.EndEdit();
        //Crear la Dimension de los Cortes.
        for (int i = 0; i < 5; i++)
        {
            ChildRowCortes = (DataRowView)BsCortes.AddNew()!;
            ChildRowCortes.BeginEdit();
            ChildRowCortes["num"] = i + 1;
            ChildRowCortes["width"] = "0";
            ChildRowCortes["lenght"] = "0";
            ChildRowCortes["msi"] = "0";
            ChildRowCortes["code_person"] = "S/N";
            InicializarColumnasObligatorias(ChildRowCortes);
            ChildRowCortes.EndEdit();
        }
        if (grid_cortes.Rows.Count > 0)
        {
            grid_cortes.ClearSelection();
            grid_cortes.CurrentCell = grid_cortes.Rows[0].Cells[0];
            grid_cortes.Rows[0].Selected = true;
        }

        //OPERADOR POR DEFECTO.
        Service.CheckOperatorDefault(operadorId, operadorName);
        txt_operador_id.Text = operadorId;
        txt_operador_name.Text = operadorName;
        grid_cortes.ReadOnly = false;
        btn_add_row_corte.Enabled = true;
        btn_delete_row_corte.Enabled = true;
        txt_long_cortar.ReadOnly = false;
        txt_vueltas1.ReadOnly = false;
        btn_buscar_operador.Enabled = true;
        btn_buscar_customer.Enabled = true;
        //3.- Abrir los Textbox para editar los datos de la Orden de Corte.
        txt_fecha_emision.Enabled = true;
        txt_fecha_produccion.Enabled = true;
        txt_plus1.ReadOnly = false;
        txt_menos1.ReadOnly = false;
        txt_plus2.ReadOnly = false;
        txt_menos2.ReadOnly = false;
        txt_sellOrder.ReadOnly = false;
        btn_buscar_rollid1.Enabled = true;
        txt_ubic.ReadOnly = false;
        CloseToolsBar();
        //Controles del Formulario.
        btn_generar_rollos.Enabled = true;
        btn_add_row_corte.Enabled = true;
        btn_delete_row_corte.Enabled = true;
        txt_vueltas1.Enabled = true;
        txt_step.Text = "2";
        chk_desperdicio1.Enabled = true;
        btn_buscar_orden.Enabled = false;
        btn_generar_txt.Enabled = false;
        chk_two_master.Enabled = true;
        UpdateStepIndicator();
        ResaltarControlesEditables(true);
        EditMode = 1;
    }
    private void Opt_modif_orden_Click(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;
        int step = txt_step.Text == string.Empty ? 0 : Convert.ToInt32(txt_step.Text);
        if (step != 2)
        {
            MessageBox.Show("Solo se puede modificar la orden en estado de PRODUCCION...", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        //configurar la barra de herramientas
        bot_guardar.Enabled = true;
        bot_primero.Enabled = false;
        bot_siguiente.Enabled = false;
        bot_anterior.Enabled = false;
        bot_ultimo.Enabled = false;
        bot_accion.Enabled = false;
        bot_exportar.Enabled = false;
        btn_buscar_orden.Enabled = false;
        bot_buscarOrders.Enabled = false;
        bot_cancelar.Enabled = true;
        btn_buscar_rollid1.Enabled = true;
        btn_vueltas.Enabled = true;


        //configurar el grid
        grid_items.ReadOnly = false;
        grid_items.Columns[0].ReadOnly = true;
        grid_items.Columns[1].ReadOnly = true;
        grid_items.Columns[2].ReadOnly = true;
        grid_items.Columns[3].ReadOnly = true;
        grid_items.Columns[4].ReadOnly = true;
        grid_items.Columns[5].ReadOnly = true;
        grid_items.Columns[6].ReadOnly = true;
        grid_items.Columns[8].ReadOnly = true;

        //cambiar fecha.
        txt_fecha_emision.Enabled = true;
        txt_fecha_produccion.Enabled = true;
        //desperdicio
        chk_desperdicio1.Enabled = true;
        //grid de cortes

        grid_cortes.ReadOnly = false;




        //ICONO DE EDICION
        label_ModoEdition.Visible = true;
        ICON_EDITMODE.Visible = true;

        //operador
        btn_buscar_operador.Enabled = true;

        //cliente
        btn_buscar_customer.Enabled = true;
        txt_sellOrder.ReadOnly = false;

        //botones de corte.
        btn_add_row_corte.Enabled = true;
        btn_delete_row_corte.Enabled = true;

        //longitud a cortar  
        txt_long_cortar.ReadOnly = false;
        txt_vueltas1.Enabled = true;
        txt_vueltas2.Enabled = true;
        txt_largo_corte.Enabled = true;
        txt_ubic.ReadOnly = false;

        //parametros de excelencia (merma/plus) tambien editables en PRODUCCION.
        txt_plus1.ReadOnly = false;
        txt_menos1.ReadOnly = false;
        txt_plus2.ReadOnly = false;
        txt_menos2.ReadOnly = false;
        if (chk_two_master.Checked)
        {
            btn_buscar_rollid2.Enabled = true;
            txt_real2_width.Enabled = true;
            txt_real2_length.Enabled = true;
            txt_matrest2_width.Enabled = true;
            txt_matrest2_lenght.Enabled = true;
            chk_desperdicio2.Enabled = true;
        }

        btn_generar_rollos.Enabled = true;

        //resaltar los textbox de parametros editables en modo edicion.
        ResaltarControlesEditables(true);

        Ds.Tables["DtMaster"]!.AcceptChanges();
        Ds.Tables["DtCortes"]!.AcceptChanges();
        Ds.Tables["DtRollos"]!.AcceptChanges();
        Ds.Tables["DtCustomer"]!.AcceptChanges();
        Ds.Tables["DtOperator"]!.AcceptChanges();
        BsMaster.ResetBindings(false);
        BsDetails.ResetBindings(false);
        BsCortes.ResetBindings(false);

        EditMode = 2;

    }
    private void Bot_cancelar_Click(object sender, EventArgs e)
    {

        //para borrar el documento se eliminan las filas primero y luego la fila master.
        if (BsMaster.Current is DataRowView drvMaster)
        {

            DataRow RowMaster = drvMaster.Row;
            DataRow[] items = RowMaster.GetChildRows(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS);

            //borrar los rollos cortados.
            foreach (var item in items)
            {
                item.Delete();
            }

            //borrar los cortes
            DataRow[] cortes_del = RowMaster.GetChildRows("FK_ENCABEZADO_CORTES");
            foreach (var cor_item in cortes_del)
            {
                cor_item.Delete();
            }

            //se borra la orden de corte en master
            RowMaster.Delete();

        }

        BsMaster.EndEdit();
        BsMaster.Position = BsMaster.Count;
        bot_primero.Enabled = true;
        bot_siguiente.Enabled = true;
        bot_ultimo.Enabled = true;
        bot_anterior.Enabled = true;
        bot_guardar.Enabled = false;
        bot_cancelar.Enabled = false;
        bot_exportar.Enabled = true;
        bot_accion.Enabled = true;
        //cerrar el formulario
        txt_long_cortar.ReadOnly = true;
        txt_vueltas1.ReadOnly = true;
        txt_vueltas2.ReadOnly = true;

        btn_add_row_corte.Enabled = false;
        btn_buscar_customer.Enabled = false;
        btn_buscar_operador.Enabled = false;
        btn_buscar_rollid1.Enabled = false;
        btn_buscar_rollid2.Enabled = false;
        btn_generar_rollos.Enabled = false;
        btn_buscar_orden.Enabled = true;

        //restaurar el color de los textbox al salir del modo edicion.
        ResaltarControlesEditables(false);
        EditMode = 0;
    }
    private List<RolloCortado> CREATE_ROLLOS_CORTADOS()
    {
        List<RolloCortado> Lista_Rollos = [];
        //picking-list;
        for (int i = 0; i <= grid_items.Rows.Count - 1; i++)
        {
            RolloCortado Rollo = new()
            {
                RollNumber = ConversorCelda.ToInt(grid_items.Rows[i].Cells["Roll_Number"].Value),
                Product_Id = ConversorCelda.ToString(grid_items.Rows[i].Cells["product_id"].Value),
                Product_Name = ConversorCelda.ToString(grid_items.Rows[i].Cells["product_name"].Value),
                UniqueCode = ConversorCelda.ToString(grid_items.Rows[i].Cells["unique_code"].Value),
                Width = ConversorCelda.ToDouble(grid_items.Rows[i].Cells["width"].Value),
                Length = ConversorCelda.ToDouble(grid_items.Rows[i].Cells["large"].Value),
                Msi = ConversorCelda.ToDouble(grid_items.Rows[i].Cells["msi"].Value),
                Splice = ConversorCelda.ToInt(grid_items.Rows[i].Cells["splice"].Value),
                Roll_Id = ConversorCelda.ToString(grid_items.Rows[i].Cells["roll_id"].Value),
                Code_Person = ConversorCelda.ToString(grid_items.Rows[i].Cells["code_person"].Value),
            };
            Lista_Rollos.Add(Rollo);
        }
        return Lista_Rollos;
    }
    private void CrearObjetoOrden()
    {
        //Actualizar la base de datos
        CREATE_HEADER_ORDEN();
        CREATE_CORTES();
        CREATE_DETALLE_ORDEN();
    }
    #endregion

    #region LIFECYCLE-DOCUMENT
    private void UpdateUIStepIndicator(int step)
    {
        DataRowView FilaActual;
        FilaActual = (DataRowView)BsMaster.Current!;
        FilaActual["step"] = step;
        BsMaster.EndEdit();

        ////crear el txt de rollos cortados
        //ExportDataService.ExportTxtFormatRollosCortados(BuscarItemsDetailsOrden(), chk_generartxt_rc.Checked, Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), true);
    }

    private void Btn_datosDocAprob_Click(object sender, EventArgs e)
    {
        int opt = Convert.ToInt32(txt_step.Text);
        if (opt < 4)
        {
            MessageBox.Show("el documento no esta aprobado...");
            return;
        }
        Frm_AprobarOC form = new(CommonService)
        {
            NumeroOC = txt_numeroOC.Text,
            TypeAction = "READ"
        };
        form.ShowDialog();
    }
    private async void Opt_cerrar_orden_Click(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;

        // No se puede cerrar una OC que aun no tiene rollos etiquetados: el cierre
        // libera los rollos cortados como producto terminado (disponible=1).
        int stepActual = txt_step.Text == string.Empty ? 0 : Convert.ToInt32(txt_step.Text);
        if (stepActual < 3)
        {
            MessageBox.Show("La orden debe estar etiquetada (ETIQUETADA) antes de poder cerrarse.", "Validación Cierre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DialogResult resultado = MessageBox.Show("¿Realmente desea Cerrar la Orden de Corte", "Advertencia...", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (resultado == DialogResult.Yes)
        {
            Toggleloading(true);
            try
            {
                //se actualiza en la Base de Datos.
                Service.UpdateStatusDocumentOC(5, txt_numeroOC.Text);
                // Al cerrar la orden se habilita el boton de generar txt y se
                // desactiva la edicion de la orden.
                CerrarForms();
                //actualiza la ui del textbox de step-indicator.
                UpdateUIStepIndicator(5);
                //actualizar el control de Step Indicator.
                UpdateStepIndicator();
                //actualizar los inventarios del master.
                bool invActualizado = await ACTUALIZAR_INVENTARIOS_MASTER();
                if (!invActualizado)
                {
                    MessageBox.Show("La orden se cerro, pero hubo un error al actualizar el inventario de masters. Revise el detalle de consumos del master.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
                else
                {
                    MessageBox.Show("Se ha cerrado la Orden de Corte Correctamente...", "Advertencia", MessageBoxButtons.OK);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cerrar la orden de corte: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Toggleloading(false);
            }
        }
    }
    private void Opt_send_production_Click(object sender, EventArgs e)
    {
        StateProductionOC();
    }


    private bool CheckDocAnulado()
    {
        if (chk_document_anul.Checked)
        {
            MessageBox.Show("El documento esta anulado, no se pueden realizar acciones sobre el...", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void Opt_etiquetar_orden_Click(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;
        EtiquetarOrdenCorte();
    }
    private void Opt_aprobar_orden_Click(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;

        // El flujo correcto es: CREAR(2) -> ETIQUETAR(3) -> APROBAR(4). No se puede
        // saltar el etiquetado (sin ROLLID los rollos no se pueden liberar a venta).
        int stepActual = txt_step.Text == string.Empty ? 0 : Convert.ToInt32(txt_step.Text);
        if (stepActual != 3)
        {
            MessageBox.Show("La orden debe estar etiquetada (ETIQUETADA) antes de poder aprobarse.",
                "Validación Aprobación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        //cargar el formulario de aprobacion
        Frm_AprobarOC form = new(CommonService)
        {
            NumeroOC = txt_numeroOC.Text,
            TypeAction = "WRITE"
        };
        form.ShowDialog();
        //se actualiza en la Base de Datos
        Service.UpdateStatusDocumentOC(4, txt_numeroOC.Text);
        //actualiza la ui del textbox de step-indicator
        DataRowView FilaActual;
        FilaActual = (DataRowView)BsMaster.Current!;
        FilaActual["step"] = 4;
        BsMaster.EndEdit();
        UpdateStepIndicator();
    }
    private void StateProductionOC()
    {
        //se actualiza en la Base de Datos
        Service.UpdateStatusDocumentOC(2, txt_numeroOC.Text);
        //se actualiza en la UI del Sistema.
        DataRowView FilaActual;
        FilaActual = (DataRowView)BsMaster.Current!;
        FilaActual["step"] = 2;
        BsMaster.EndEdit();
        UpdateStepIndicator();
        MessageBox.Show("Se ha cambiado el estatus del documento a PRODUCCION...");
    }
    private void EtiquetarOrdenCorte()
    {
        // El etiquetado (asignacion del ROLLID) se hace una sola vez: re-etiquetar una
        // orden ya etiquetada/aprobada/cerrada sobrescribiria el unique_code y degradaria
        // el step (de 5 a 3), dejando datos inconsistentes para despacho e inventario.
        int stepActual = txt_step.Text == string.Empty ? 0 : Convert.ToInt32(txt_step.Text);
        if (stepActual >= 3)
        {
            MessageBox.Show("Esta orden ya fue etiquetada o esta en una etapa posterior (aprobada/cerrada). No es posible etiquetarla nuevamente.",
                "Validación Etiquetado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        //se actualiza el unique code
        if (BsMaster.Current == null) return;

        // Obtener la fila maestra actual como DataRowView
        DataRowView rowMaestro = (DataRowView)BsMaster.Current;
        // Obtener todas las filas hijas relacionadas y me aseguro que este ordenado por roll
        DataRow[] filasHijas = rowMaestro.Row.GetChildRows(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS).OrderBy(x => x["roll_number"]).ToArray();

        int numero_unico = Service.BuscarUniqueCodeConsec();

        // VALIDACION de consecutivo unico continuo (sin saltos):
        // la cantidad real de rollos debe coincidir con cant_rollos de la orden
        // y el rango [RC(numero_unico+1) .. RC(numero_unico+n)] no debe colisionar
        // con codigos ya asignados a otras ordenes.
        int cantRollosEsperada = 0;
        if (rowMaestro["cant_rollos"] != DBNull.Value)
            cantRollosEsperada = Convert.ToInt32(rowMaestro["cant_rollos"]);

        if (filasHijas.Length == 0)
        {
            MessageBox.Show("La orden no tiene rollos cortados para etiquetar. Verifica la cantidad de rollos.",
                "Validación Etiquetado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        if (cantRollosEsperada > 0 && filasHijas.Length != cantRollosEsperada)
        {
            MessageBox.Show($"No se permite etiquetar: la orden tiene {filasHijas.Length} rollos, pero cant_rollos indica {cantRollosEsperada}. Corrige la cantidad antes de etiquetar.",
                "Validación Etiquetado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        int primero = numero_unico + 1;
        int ultimo = numero_unico + filasHijas.Length;
        var colisiones = Service.ValidarRangoUniqueCodeGlobal(primero, ultimo, txt_numeroOC.Text);
        if (colisiones.Count > 0)
        {
            string ocupados = string.Join(", ", colisiones.Select(c => "RC" + c).OrderBy(x => x));
            MessageBox.Show($"No se permite etiquetar: los códigos RC{primero} a RC{ultimo} deben ser consecutivos sin saltos, " +
                $"pero ya existen asignados a otros rollos: {ocupados}. Verifica el consecutivo único antes de etiquetar.",
                "Validación Etiquetado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        MessageBox.Show($"Se etiquetarán {filasHijas.Length} rollos con códigos RC{primero} a RC{ultimo} (último = consecutivo {numero_unico} + {filasHijas.Length}).",
                "Etiquetado", MessageBoxButtons.OK, MessageBoxIcon.Information);

        //actualiza la ui del datagrid items rollos cortados
        List<RolloCortado> rolls = [];

        foreach (DataRow item in filasHijas)
        {
            RolloCortado rollo = new();
            item.BeginEdit();
            numero_unico += 1;
            item["unique_code"] = "RC" + Convert.ToString(numero_unico);
            item.EndEdit();
            rollo.Numero = txt_numeroOC.Text;
            rollo.RollNumber = Convert.ToInt32(item["roll_number"]);
            rollo.UniqueCode = item["unique_code"].ToString()!;
            rolls.Add(rollo);
        }

        //se actualiza en la Base de Datos el step del documento
        Service.UpdateStatusDocumentOC(3, txt_numeroOC.Text);

        //actualiza la ui del textbox de step-indicator
        DataRowView FilaActual;
        FilaActual = (DataRowView)BsMaster.Current!;
        FilaActual["step"] = 3;
        BsMaster.EndEdit();

        //se actualizan los rollos cortados en la BD con los unique code nuevos
        Service.UpdateUniqueCodeRollosCortados(rolls);

        //actualiza el consecutivo de codigo unico
        Service.UpdateUniqueCodeBD(numero_unico.ToString());
        //actualiza la ui del indicator
        UpdateStepIndicator();

        //se crea el txt de los rollos cortados.
        ExportDataService.ExportTxtFormatRollosCortados(BuscarItemsDetailsOrden(), chk_generartxt_rc.Checked, Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), Convert.ToDateTime(txt_fecha_emision.Text).ToShortDateString(), false);

        MessageBox.Show("Se ha Etiquetado la Orden de Corte...");
    }
    private void UpdateStepIndicator()
    {
        if (txt_step.Text == string.Empty) return;
        int opt = Convert.ToInt32(txt_step.Text);

        if (opt == 1)
        {
            InitStepIndicator();
            labelstep1.Visible = true;
            pictureBox1.Image = Properties.Resources.step1;
            UpdateOptionMenuAction(true, true, true, true, true);
        }
        if (opt == 2)
        {
            InitStepIndicator();
            labelstep2.Visible = true;
            pictureBox2.Image = Properties.Resources.step2_active;
            UpdateOptionMenuAction(false, true, true, true, true);
        }
        if (opt == 3)
        {
            InitStepIndicator();
            labelstep3.Visible = true;
            pictureBox3.Image = Properties.Resources.step3_active;
            UpdateOptionMenuAction(false, false, true, true, true);
        }
        if (opt == 4)
        {
            //Aprobado.
            InitStepIndicator();

            labelstep4.Visible = true;
            pictureBox4.Image = Properties.Resources.step4_active;
            UpdateOptionMenuAction(false, false, false, true, true);
        }
        if (opt == 5)
        {
            //Aprobado.
            InitStepIndicator();
            labelstep5.Visible = true;
            pictureBox5.Image = Properties.Resources.step5_active;
            UpdateOptionMenuAction(false, false, false, false, false);
        }


        if (chk_document_anul.Checked)
        {
            Icon_Anulado.Visible = true;
            anularOrden.Enabled = false;
        }
        else
        {
            Icon_Anulado.Visible = false;
            anularOrden.Enabled = true;
        }

    }
    private void InitStepIndicator()
    {
        labelstep1.Visible = false;
        pictureBox1.Image = Properties.Resources.step1_deactivate;
        labelstep2.Visible = false;
        pictureBox2.Image = Properties.Resources.step2_deactivate;
        labelstep3.Visible = false;
        pictureBox3.Image = Properties.Resources.step3_deactive;
        labelstep4.Visible = false;
        pictureBox4.Image = Properties.Resources.step4_deactive;
        labelstep5.Visible = false;
        pictureBox5.Image = Properties.Resources.step5_deactive;
    }
    #endregion

    #region OTHERS FUNCTIONS
    private void UpdateValueRealLenghtMaster1()
    {
        if (txt_real1.Text != "" &&
            double.TryParse(txt_length1.Text, out double length) &&
            double.TryParse(txt_menos1.Text, out double menos) &&
            double.TryParse(txt_plus1.Text, out double plus))
        {
            double num = CalculosOrdenCorte.LongitudReal(length, menos, plus);
            txt_real1.Text = num.ToString();
        }

    }

    private void Txt_plus1_KeyUp(object sender, KeyEventArgs e)
    {
        ProgramarRecalculo(() => UpdateValueRealLenghtMaster1());
    }

    private void Txt_menos1_KeyUp(object sender, KeyEventArgs e)
    {
        ProgramarRecalculo(() => UpdateValueRealLenghtMaster1());
    }
    private void Btn_delete_row_corte_Click(object sender, EventArgs e)
    {
        if (grid_cortes.SelectedRows.Count > 0)
        {
            foreach (DataGridViewRow row in grid_cortes.SelectedRows)
            {
                grid_cortes.Rows.Remove(row);
            }
            txt_cortes_ancho.Text = grid_cortes.Rows.Count.ToString();

        }
        else
        {
            MessageBox.Show("Por favor, seleccione una fila para eliminar.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
        }
        CALCULATE_TOTAL_WIDTH_CORTES();
        CALCULATE_MATERIAL_RESTANTE();
        CALCULATE_DATA_CORTES();
        CALCULAR_TOTAL_ROLLOS_CORTAR();
        grid_cortes.Focus();
        grid_cortes.CurrentCell = grid_cortes.Rows[^1].Cells[1];
    }
    private Panel? _loadingOverlay;
    private Label? _dot1, _dot2, _dot3;
    private System.Windows.Forms.Timer? _loadingTimer;
    private int _loadingStep;

    private void EnsureLoadingOverlay()
    {
        if (_loadingOverlay != null) return;

        // Splash flotante: solo el cuadro con los puntos animados, superpuesto en la
        // parte superior del form. No hay fondo de pantalla completa, por lo que el
        // formulario NO se borra ni se repinta al mostrarlo/ocultarlo.
        _loadingOverlay = new Panel
        {
            Size = new Size(260, 112),
            BackColor = Color.White,
            BorderStyle = BorderStyle.FixedSingle,
            Visible = false
        };

        var msg = new Label
        {
            AutoSize = false,
            Size = new Size(240, 24),
            Location = new Point(10, 16),
            Text = "Cargando datos...",
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11F, FontStyle.Bold),
            ForeColor = Color.FromArgb(60, 60, 60)
        };
        _loadingOverlay.Controls.Add(msg);

        var dotFont = new Font("Segoe UI", 18F, FontStyle.Bold);
        _dot1 = new Label { AutoSize = false, Size = new Size(28, 28), Location = new Point(58, 62), Text = "●", TextAlign = ContentAlignment.MiddleCenter, Font = dotFont };
        _dot2 = new Label { AutoSize = false, Size = new Size(28, 28), Location = new Point(116, 62), Text = "●", TextAlign = ContentAlignment.MiddleCenter, Font = dotFont };
        _dot3 = new Label { AutoSize = false, Size = new Size(28, 28), Location = new Point(174, 62), Text = "●", TextAlign = ContentAlignment.MiddleCenter, Font = dotFont };

        _loadingOverlay.Controls.Add(_dot1);
        _loadingOverlay.Controls.Add(_dot2);
        _loadingOverlay.Controls.Add(_dot3);
        this.Controls.Add(_loadingOverlay);
        _loadingOverlay.BringToFront();

        _loadingTimer = new System.Windows.Forms.Timer { Interval = 180 };
        _loadingTimer.Tick += (_, _) =>
        {
            _loadingStep = (_loadingStep + 1) % 3;
            UpdateLoadingDots();
        };
    }

    private void CenterLoadingOverlay()
    {
        if (_loadingOverlay == null) return;
        // Se muestra centrado en el form, encima de el y sin borrarlo.
        _loadingOverlay.Location = new Point(
            Math.Max(0, (this.ClientSize.Width - _loadingOverlay.Width) / 2),
            Math.Max(0, (this.ClientSize.Height - _loadingOverlay.Height) / 2));
    }

    private void UpdateLoadingDots()
    {
        if (_dot1 == null || _dot2 == null || _dot3 == null) return;
        var active = Color.FromArgb(235, 109, 14);
        var idle = Color.FromArgb(200, 200, 200);
        _dot1.ForeColor = _loadingStep == 0 ? active : idle;
        _dot2.ForeColor = _loadingStep == 1 ? active : idle;
        _dot3.ForeColor = _loadingStep == 2 ? active : idle;
    }

    private void Toggleloading(bool isLoading)
    {
        EnsureLoadingOverlay();
        if (isLoading)
        {
            CenterLoadingOverlay();
            _loadingStep = 0;
            UpdateLoadingDots();
            _loadingOverlay!.Visible = true;
            _loadingOverlay.BringToFront();
            _loadingTimer!.Start();
            this.Cursor = Cursors.WaitCursor;
        }
        else
        {
            _loadingTimer!.Stop();
            _loadingOverlay!.Visible = false;
            this.Cursor = Cursors.Default;
        }
    }
    private bool Validar()
    {
        if (!ValidarDocumento()) return false;
        return true;

    }
    private bool ValidarDocumento()
    {
        bool validateDoc;

        //validar el rollid
        if (txt_rollid_1.Text == "0" && txt_width1.Text == "0" && txt_length1.Text == "0")
        {
            MessageBox.Show("debe seleccionar el roll-id del master a montar...");
            validateDoc = false;
            return validateDoc;
        }

        //validar los cortes
        if (!ValidDefintionsCortes())
            return false;

        //validar el operador
        if (txt_operador_id.Text == "")
        {
            MessageBox.Show("debe introducir el nombre del operador...");
            validateDoc = false;
            return validateDoc;
        }
        if (txt_cust_id.Text == "" && txt_cust_name.Text == "")
        {
            MessageBox.Show("debe introducir los datos del cliente...");
            validateDoc = false;
            return validateDoc;
        }
        if (!double.TryParse(txt_long_cortar.Text, out double longitudCortar) || longitudCortar <= 0)
        {
            MessageBox.Show("debe introducir una longitud a cortar valida (mayor que cero)...");
            validateDoc = false;
            return validateDoc;
        }
        if (txt_vueltas1.Value <= 0)
        {
            MessageBox.Show("debe agregar el numero de vueltas...");
            validateDoc = false;
            return validateDoc;
        }
        if (grid_items.Rows.Count == 0)
        {
            MessageBox.Show("no tiene renglones de rollos cortados, debe generar los rollos ...");
            validateDoc = false;
            return validateDoc;
        }
        if (grid_cortes.Rows.Count == 0)
        {
            MessageBox.Show("no tiene la definicion de los cortres, definirla por favor...");
            validateDoc = false;
            return validateDoc;
        }

        //si es two-master debe elegirse el roll-id del segundo master.
        if (chk_two_master.Checked && (txt_rollid_2.Text == string.Empty || txt_rollid_2.Text == "0"))
        {
            MessageBox.Show("debe seleccionar el roll-id del segundo master (two-master)...");
            validateDoc = false;
            return validateDoc;
        }

        //ACC-03: cuadre a lo ancho — la suma de los anchos de los cortes debe cubrir
        //el ancho completo del master (con tolerancia de redondeo).
        if (double.TryParse(txt_ancho_corte.Text, out double anchoCortes) &&
            double.TryParse(txt_width1.Text, out double anchoMaster) &&
            !CalculosOrdenCorte.CuadreAnchoValido(anchoCortes, anchoMaster))
        {
            MessageBox.Show($"El cuadre a lo ancho no es correcto: la suma de los cortes es {anchoCortes:N2} plg y el ancho del master es {anchoMaster:N2} plg. Debe cubrir el ancho completo del master.",
                "Validación Cuadre", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            validateDoc = false;
            return validateDoc;
        }

        //ACC-04: el largo del master debe alcanzar para producir los rollos. Se compara el
        //consumo requerido (longitud_cortar x vueltas) contra el largo del master
        //(txt_length1): el "restante" (txt_matrest1_lenght) ya descuenta este mismo
        //consumo y no representa el material disponible para montar la OC.
        double disponibleM1 = txt_length1.Text == "" ? 0 : Convert.ToDouble(txt_length1.Text);
        double consumoM1 = CalculosOrdenCorte.LongitudTotal(longitudCortar, Convert.ToDouble(txt_vueltas1.Value));
        if (!CalculosOrdenCorte.MasterTieneMaterialSuficiente(consumoM1, disponibleM1))
        {
            MessageBox.Show($"Master 1 ({txt_rollid_1.Text}) insuficiente: se requieren {consumoM1:N2} pies y el master dispone de {disponibleM1:N2} pies. Seleccione un master con mas material.",
                "Validación Material", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            validateDoc = false;
            return validateDoc;
        }

        if (chk_two_master.Checked && double.TryParse(txt_long_cortar2.Text, out double longCortar2) && txt_vueltas2.Value > 0)
        {
            double disponibleM2 = txt_length2.Text == "" ? 0 : Convert.ToDouble(txt_length2.Text);
            double consumoM2 = CalculosOrdenCorte.LongitudTotal(longCortar2, Convert.ToDouble(txt_vueltas2.Value));
            if (!CalculosOrdenCorte.MasterTieneMaterialSuficiente(consumoM2, disponibleM2))
            {
                MessageBox.Show($"Master 2 ({txt_rollid_2.Text}) insuficiente: se requieren {consumoM2:N2} pies y el master dispone de {disponibleM2:N2} pies. Seleccione un master con mas material.",
                    "Validación Material", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                validateDoc = false;
                return validateDoc;
            }
        }

        if (txt_ubic.Text == "")
        {
            MessageBox.Show("establezca un valor para la ubicaci�n...");
            validateDoc = false;
            return validateDoc;
        }
        return true;
    }
    public DataRow[] BuscarItemsDetailsOrden()
    {
        DataRowView rowMaestro = (DataRowView)BsMaster.Current!;
        return rowMaestro.Row.GetChildRows(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS);
    }

    private void Btn_add_row_corte_Click(object sender, EventArgs e)
    {
        RollosCortados = (DataRowView)BsCortes.AddNew()!;
        RollosCortados[0] = grid_cortes.Rows.Count.ToString();
        RollosCortados["width"] = 0;
        RollosCortados["lenght"] = 0;
        RollosCortados["msi"] = 0;
        RollosCortados["code_person"] = "S/N";
        RollosCortados.BeginEdit();
        InicializarColumnasObligatorias(RollosCortados);

        grid_cortes.Focus();
        grid_cortes.CurrentCell = grid_cortes.Rows[^1].Cells[1];


    }
    private bool ValidDefintionsCortes()
    {
        for (int i = 0; i < grid_cortes.Rows.Count; i++)
        {
            object? w = grid_cortes.Rows[i].Cells["width"].Value;
            object? l = grid_cortes.Rows[i].Cells["lenght"].Value;
            object? m = grid_cortes.Rows[i].Cells["msi"].Value;
            if (!EsDefinicionCorteValida(w) || !EsDefinicionCorteValida(l) || !EsDefinicionCorteValida(m))
            {
                MessageBox.Show("Debe completar todas la definicion de los cortes antes de continuar.", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }
        }
        return true;

        // null/DBNull, no numerico o menor/igual a cero se consideran definicion invalida.
        static bool EsDefinicionCorteValida(object? valor)
            => valor != null && valor != DBNull.Value
               && double.TryParse(valor.ToString(), out double d) && d > 0;
    }
    private void Btn_LabelCodeBar_Click(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;
        CREATE_DETALLE_ORDEN();

        string fecha_produccion = Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString();

        PrintLabelsRolls PrintLabels = new()
        {
            Rollos = Detalle,
            Orden_Corte = txt_numeroOC.Text,
            Fechapro = fecha_produccion
        };
        PrintLabels.ShowDialog();
    }
    private void Bot_buscarOrders_Click(object sender, EventArgs e)
    {
        FrmBuscadorOC fbuscador = new()
        {
            DtItems = Ds.Tables["DtMaster"]!
        };
        fbuscador.ShowDialog();
        if (fbuscador.Orden != null && fbuscador.Orden != "")
        {
            int busqueda = BsMaster.Find("numero", fbuscador.Orden);
            if (busqueda > 0)
            {
                BsMaster.Position = busqueda;
            }
        }
    }
    private void Btn_code_person_Click(object sender, EventArgs e)
    {
        if (txt_code_person.Text == "")
        {
            MessageBox.Show("Debe ingresar el codigo personalizado...", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }
        foreach (DataRowView row in BsDetails)
        {
            row["code_person"] = txt_code_person.Text.ToString();
        }
        Service.OrdenUpdateCodePerson(txt_numeroOC.Text, txt_code_person.Text);
        //Generar el txt de los rollos cortados.
        ExportDataService.ExportTxtFormatRollosCortados(BuscarItemsDetailsOrden(), chk_generartxt_rc.Checked, Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), false);
    }
    private void Bot_imprimir_Click(object sender, EventArgs e)
    {

    }
    private void Btn_buscar_orden_Click(object sender, EventArgs e)
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
            int busqueda = BsMaster.Find("numero", frmBuscar.Parameter.Trim());
            if (busqueda > 0)
            {
                BsMaster.Position = busqueda;
                UpdateStepIndicator();
                ContadorRegistros();
            }
            else
            {
                MessageBox.Show("No se encontro la orden de corte...", "Advertencia", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
        }
    }
    private void Bot_buscar_Click(object sender, EventArgs e)
    {
        txt_fecha_emision.Enabled = true;
        txt_fecha_produccion.Enabled = true;
        btn_buscar_operador.Enabled = true;
        btn_buscar_customer.Enabled = true;
        txt_sellOrder.ReadOnly = false;
        chk_desperdicio1.Enabled = true;
        CloseToolsBar();
        ResaltarControlesEditables(true);
        EditMode = 2;
    }
    private void Bot_exportar_Click(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;
        List<RolloCortado> rollosCortados = CREATE_ROLLOS_CORTADOS();
        ExportDataService.ExportToExcel<RolloCortado>(rollosCortados, "RollosCortados.xlsx");
    }
    private void Btn_generar_txt_Click(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;
        ExportDataService.ExportTxtFormatRollosCortados(BuscarItemsDetailsOrden(), chk_generartxt_rc.Checked, Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), Convert.ToDateTime(txt_fecha_produccion.Text).ToShortDateString(), true);
    }
    private void CerrarForms()
    {
        //Menu opciones
        bot_primero.Enabled = true;
        bot_siguiente.Enabled = true;
        bot_ultimo.Enabled = true;
        bot_anterior.Enabled = true;
        bot_accion.Enabled = true;
        bot_exportar.Enabled = true;
        bot_buscarOrders.Enabled = true;
        bot_guardar.Enabled = false;
        bot_cancelar.Enabled = false;
        //botones formulario
        btn_buscar_customer.Enabled = false;
        btn_buscar_rollid1.Enabled = false;
        btn_buscar_rollid2.Enabled = false;
        btn_buscar_operador.Enabled = false;
        btn_generar_rollos.Enabled = false;
        btn_add_row_corte.Enabled = false;
        btn_delete_row_corte.Enabled = false;
        btn_buscar_orden.Enabled = true;
        btn_generar_txt.Enabled = true;

        //controles del formulario.
        txt_fecha_emision.Enabled = false;
        txt_fecha_produccion.Enabled = false;
        txt_plus1.ReadOnly = true;
        txt_plus2.ReadOnly = true;
        txt_menos1.ReadOnly = true;
        txt_menos2.ReadOnly = true;
        txt_sellOrder.ReadOnly = true;
        txt_vueltas1.Enabled = false;
        txt_largo_corte.Enabled = false;
        txt_long_cortar.ReadOnly = true;
        txt_ubic.ReadOnly = true;
        chk_desperdicio1.Enabled = false;
        grid_cortes.ReadOnly = true;

        //restaurar el color de los textbox al salir del modo edicion.
        ResaltarControlesEditables(false);
    }
    private void CloseToolsBar()
    {
        //Menu opciones
        bot_primero.Enabled = false;
        bot_siguiente.Enabled = false;
        bot_ultimo.Enabled = false;
        bot_anterior.Enabled = false;
        bot_accion.Enabled = false;
        bot_exportar.Enabled = false;
        bot_guardar.Enabled = true;
        bot_cancelar.Enabled = true;
        bot_buscarOrders.Enabled = false;
    }
    private static void UpdateAppSettingJson<T>(string key, T value)
    {
        try
        {
            // Ruta del archivo appsettings.json en tiempo de desarrollo
            string appSettingsPath = AppDomain.CurrentDomain.BaseDirectory + "appsettings.json";
            string json = File.ReadAllText(appSettingsPath);
            dynamic jsonObj = JsonConvert.DeserializeObject(json)!;
            var sectionPath = key.Split(":")[0];
            if (!string.IsNullOrEmpty(sectionPath))
            {
                var keyPath = key.Split(":")[1];
                jsonObj[sectionPath][keyPath] = value;
            }
            else
            {
                jsonObj[sectionPath] = value; // if no sectionpath just set the value
            }
            string output = JsonConvert.SerializeObject(jsonObj, Formatting.Indented);
            File.WriteAllText(appSettingsPath, output);
        }
        catch (ConfigurationErrorsException)
        {
            Console.WriteLine("Error writing app settings");
        }

    }
    private void Btn_buscar_operador_Click(object sender, EventArgs e)
    {
        FrmSeleccion SelOperator = new(CommonService)
        {
            DtItems = Ds.Tables["DtOperator"]!,
            Titulo = "operadores",
        };
        SelOperator.ShowDialog();
        txt_operador_id.Text = SelOperator.Id;
        txt_operador_name.Text = SelOperator.Description;
    }

    private void Btn_buscar_customer_Click(object sender, EventArgs e)
    {
        FrmSeleccion SelCust = new(CommonService)
        {
            DtItems = Ds.Tables["DtCustomer"]!,
            Titulo = "clientes",
        };
        SelCust.ShowDialog();
        txt_cust_id.Text = SelCust.Id;
        txt_cust_name.Text = SelCust.Description;
    }
    #endregion

    #region INVENTARIOS
    private async Task<bool> ACTUALIZAR_INVENTARIOS_MASTER()
    {
        // Los controles UI se leen en el hilo de UI (sin Task.Run): si se leen desde un
        // hilo de pool se viola el token de hilo del control (acceso cross-thread).
        double consumoReal = txt_real1_length.Text == "" ? 0 : Convert.ToDouble(txt_real1_length.Text);
        double consumoDesper = chk_desperdicio1.Checked
            ? (txt_matrest1_lenght.Text == "" ? 0 : Convert.ToDouble(txt_matrest1_lenght.Text))
            : 0;
        string rollid = txt_rollid_1.Text;
        string tipo_m = txt_tipo_master.Text.ToString().ToUpper().Trim();

        bool ok = await Service.ActualizarInventariosMasterAsync(
            rollid,
            txt_numeroOC.Text,
            consumoReal,
            consumoDesper,
            chk_desperdicio1.Checked,
            tipo_m,
            txt_numeroOC.Text);

        //ACC-01: en modo two-master el consumo del segundo master tambien se descuenta
        //del inventario (igual que el master 1: consumo real + desperdicio).
        if (chk_two_master.Checked && txt_rollid_2.Text != string.Empty && txt_rollid_2.Text != "0")
        {
            double consumoReal2 = txt_real2_length.Text == "" ? 0 : Convert.ToDouble(txt_real2_length.Text);
            double consumoDesper2 = chk_desperdicio2.Checked
                ? (txt_matrest2_lenght.Text == "" ? 0 : Convert.ToDouble(txt_matrest2_lenght.Text))
                : 0;

            string tipoM2 = await ResolverTipoMaster(txt_rollid_2.Text);
            if (string.IsNullOrWhiteSpace(tipoM2))
            {
                ServiceErrors.Report($"No se pudo determinar el tipo de inventario del master 2 ({txt_rollid_2.Text}); no se actualizo su consumo.");
                return false;
            }

            bool okM2 = await Service.ActualizarInventariosMasterAsync(
                txt_rollid_2.Text,
                txt_numeroOC.Text,
                consumoReal2,
                consumoDesper2,
                chk_desperdicio2.Checked,
                tipoM2,
                txt_numeroOC.Text);

            return ok && okM2;
        }

        return ok;
    }

    private async Task<string> ResolverTipoMaster(string rollid)
    {
        try
        {
            DataTable dt = await Service.BuscarRollId("Roll_Id", rollid);
            if (dt != null && dt.Rows.Count > 0)
            {
                return dt.Rows[0]["tipo_mov"]?.ToString()?.Trim() ?? string.Empty;
            }
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al resolver el tipo de inventario del master " + rollid + ": " + ex.Message);
        }
        return string.Empty;
    }
    #endregion
    private void Btn_vueltas_Click(object sender, EventArgs e)
    {

        Frm_ConfigVueltas frmConfigVueltas = new(Service)
        {
            Numero_Vueltas = Convert.ToInt32(txt_vueltas1.Value),
            Longitud_a_Cortar = Convert.ToDouble(txt_long_cortar.Text),
            OC = txt_numeroOC.Text,
            StatusConfigVueltas = chk_ConfigVueltas.Checked,
            EditMode = this.EditMode,
            NumeroCortes = Convert.ToInt32(txt_cortes_ancho.Text.Trim())
        };

        frmConfigVueltas.ShowDialog();

        //guardar en la base de datos la configuracion de vueltas.
        if (frmConfigVueltas.SaveChenged)
        {
            //actualizar la ui del grid de vueltas.
            ActualizarUIConfigVueltas(frmConfigVueltas.Vueltas, frmConfigVueltas.VueltasModificadas);

            double total_length = frmConfigVueltas.Total_Length_utilizado;

            //actualiza el valor del length ral utilizado despues de configurar las vueltas.
            txt_real1_length.Text = total_length.ToString("F2");

            //Actualizar el estatus de Configuracion de Vueltas.
            chk_ConfigVueltas.Checked = true;

            CALCULATE_TOTAL_WIDTH_CORTES();
            CALCULATE_MATERIAL_RESTANTE();

            //Verifica si el valor del length real utilizado es negativo se coloca en cero.
            double real_restante = Convert.ToDouble(txt_matrest1_lenght.Text);

            if (real_restante < 0)
            {
                txt_matrest1_lenght.Text = "0";
            }
        }
    }





    private void ActualizarUIConfigVueltas(List<ConfigVueltas> ConfigVueltas, List<int> VueltasModif)
    {

        foreach (DataRowView row in BsDetails)
        {
            int vueltamod = Convert.ToInt32(row["vuelta"]);

            if (VueltasModif.Contains(vueltamod))
            {
                row["large"] = ConfigVueltas.FirstOrDefault(v => v.Vuelta_numero == vueltamod)!.Longitud_Cortar;
                row["splice"] = ConfigVueltas.FirstOrDefault(v => v.Vuelta_numero == vueltamod)!.Splice;
            }

        }
        BsDetails.ResetBindings(false);

    }


    // Modifica la firma del m�todo para que el par�metro sender sea nullable, coincidiendo con el delegado EventHandler.
    private void BsMaster_PositionChanged(object? sender, EventArgs args)
    {
        if (BsMaster.Current == null) return;

        DataRowView FilaActual = (DataRowView)BsMaster.Current!;

        if (FilaActual == null) return;

        bool DocumentConfigVueltas = false;
        if (FilaActual["ConfigVueltas"] != DBNull.Value && FilaActual["ConfigVueltas"] != null)
            DocumentConfigVueltas = Convert.ToBoolean(FilaActual["ConfigVueltas"]);

        if (DocumentConfigVueltas)
        {
            btn_vueltas.Enabled = true;
        }
        else
        {
            btn_vueltas.Enabled = false;
        }
    }

    private void Grid_items_DataError(object sender, DataGridViewDataErrorEventArgs e)
    {
        if (e.Exception != null && e.Context == DataGridViewDataErrorContexts.Commit)
        {
            e.ThrowException = false;
        }
    }

    private void OrdenCorteToolStripMenuItem_Click(object sender, EventArgs e)
    {

    }

    private void Chk_two_rollid_CheckedChanged(object sender, EventArgs e)
    {
        if (EditMode == 0) return;

        if (chk_two_master.Checked)
        {
            WorkflowMastersTwoRollid();
        }
    }
    public void WorkflowMastersTwoRollid()
    {

        btn_buscar_rollid2.Enabled = true;
        txt_long_cortar2.ReadOnly = false;
        txt_vueltas2.ReadOnly = false;
        txt_real2_width.Enabled = true;
        txt_real2_length.Enabled = true;
        txt_matrest2_width.Enabled = true;
        txt_matrest2_lenght.Enabled = true;
        chk_desperdicio2.Enabled = true;

    }
    private async void Btn_buscar_rollid2_Click(object sender, EventArgs e)
    {
        using Frm_RollId frmrollid = new(Service);
        frmrollid.ShowDialog();

        if (frmrollid.MasterRoll != null)
        {
            string rollidAnterior = txt_rollid_2.Text.Trim();
            string rollidNuevo = frmrollid.MasterRoll.Roll_Id;

            if (rollidNuevo != rollidAnterior && !string.IsNullOrWhiteSpace(txt_numeroOC.Text)
                && double.TryParse(txt_real2_length.Text, out double consumoActual) && consumoActual > 0)
            {
                bool esDesperdicio = chk_desperdicio2.Checked;
                double consumoDesperdicio = esDesperdicio && double.TryParse(txt_matrest2_lenght.Text, out double desp)
                    ? desp : 0;
                string tipoAnterior = await ResolverTipoMaster(rollidAnterior);
                string tipoNuevo = frmrollid.MasterRoll.Tipo_mov;

                if (string.IsNullOrWhiteSpace(tipoAnterior))
                {
                    ServiceErrors.Report($"No se pudo determinar el tipo de inventario del master 2 ({rollidAnterior}); no se reasigno su consumo.");
                    frmrollid.Dispose();
                    return;
                }

                DialogResult confirmar = MessageBox.Show(
                    $"El master {rollidAnterior} tiene {consumoActual:N2} pies consumidos por la orden {txt_numeroOC.Text}.\n\n" +
                    "Al cambiar el master, el material se devolverá al inventario del master anterior y se restará " +
                    $"del consumo del nuevo master {rollidNuevo}.\n\n¿Desea continuar?",
                    "Reasignación de master", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

                if (confirmar != DialogResult.Yes)
                {
                    frmrollid.Dispose();
                    return;
                }

                bool ok = await Service.ReasignarConsumoMasterAsync(
                    txt_numeroOC.Text, rollidAnterior, rollidNuevo,
                    consumoActual, consumoDesperdicio, esDesperdicio, tipoAnterior, tipoNuevo);

                if (!ok)
                {
                    frmrollid.Dispose();
                    return;
                }
            }

            //Rollid_master = frmrollid.MasterRoll.Roll_Id;
            txt_rollid_2.Text = rollidNuevo;
            txt_width2.Text = frmrollid.MasterRoll.Width.ToString("N2");
            txt_length2.Text = frmrollid.MasterRoll.Length.ToString("N2");
            //txt_real1.Text = frmrollid.MasterRoll.Length.ToString("N2");
            //txt_product_id.Text = frmrollid.MasterRoll.Product_Id;
            //txt_product_name.Text = frmrollid.MasterRoll.Product_Name;
            //TipoMovimiento = frmrollid.MasterRoll.tipo_mov;
            //txt_tipo_master.Text = frmrollid.MasterRoll.tipo_mov;

            CALCULATE_TOTAL_WIDTH_CORTES();
            CALCULATE_MATERIAL_RESTANTE();

            this.Validate();

            //txt_rollid_1.Focus();
            //txt_rollid_1.Select();

            BsMaster.EndEdit();
            Ds.Tables["DtMaster"]!.AcceptChanges();
            BsMaster.ResetBindings(false);
            frmrollid.Dispose();
        }
    }

    private void Txt_vueltas2_ValueChanged(object sender, EventArgs e)
    {
        if (EditMode == 0) return;
        CalcularMateriaRestanteMaster2();
        RegenerarRollosEnEdicion();
    }
    private void CalcularMateriaRestanteMaster2()
    {
        if (chk_two_master.Checked)
        {
            CalcularLONGITUDACORTAR();
            double MatRes2 = CalculosOrdenCorte.RestanteMaterial(Convert.ToDouble(txt_length2.Text), Convert.ToDouble(txt_real2_length.Text));
            txt_matrest2_lenght.Text = MatRes2.ToString("N2");
            txt_matrest2_width.Text = txt_ancho_corte.Text;
            CALCULAR_TOTAL_ROLLOS_CORTAR2();
        }
    }

    private void Txt_long_cortar2_KeyUp(object sender, KeyEventArgs e)
    {
        if (EditMode == 0) return;
        ProgramarRecalculo(() =>
        {
            CalcularMateriaRestanteMaster2();
            RegenerarRollosEnEdicion();
        });
    }

    private void FrmOrdenCorte_FormClosed(object sender, FormClosedEventArgs e)
    {
        CancelarRecalculosPendientes();
        this.Dispose();
        Ds.Dispose();
    }

    private void FrmOrdenCorte_SizeChanged(object sender, EventArgs e)
    {
        if (this.WindowState == FormWindowState.Maximized)
        {
            // Recorremos todos los formularios abiertos en la aplicaci�n
            foreach (Form frm in Application.OpenForms)
            {
                // Ignoramos el formulario principal
                if (frm != this)
                {
                    frm.BringToFront();
                    frm.Activate();
                }
            }
        }

    }

    private void AnularOrden_Click(object sender, EventArgs e)
    {
        DialogResult result = MessageBox.Show("Desea realmente anular este documento...(S/N)", "Confirmación", MessageBoxButtons.YesNo, MessageBoxIcon.Question);

        if (result == DialogResult.Yes)
        {
            // Se anula primero en la Base de Datos: el servicio reporta por si mismo el
            // motivo del rechazo (orden ya anulada, cerrada, aprobada, etc.). La UI solo
            // se marca como anulada cuando el servicio confirma la anulacion.
            bool anulada = Service.AnularOrdenCorte(txt_numeroOC.Text.ToString());
            if (anulada)
            {
                var current = (DataRowView)BsMaster.Current!;
                if (current != null)
                {
                    current["anulada"] = true;
                    BsMaster.ResetCurrentItem();
                }
                Icon_Anulado.Visible = true;
                MessageBox.Show("se anulo la orden correctamente.");
            }
        }
    }

    private void reporteDeDesperdiciosToolStripMenuItem_Click(object sender, EventArgs e)
    {
        if (string.IsNullOrWhiteSpace(txt_numeroOC.Text))
        {
            MessageBox.Show("Seleccione una orden de corte.");
            return;
        }
        if (CheckDocAnulado()) return;
        ReportService.Reporte_Desperdicios(txt_numeroOC.Text, this, "ReporteDesperdicios.rdlc", "Reporte de Desperdicios.");
    }

    private void bot_imprimir_Click_1(object sender, EventArgs e)
    {
        if (CheckDocAnulado()) return;
        ReportService.Reporte_Orden_Corte(txt_numeroOC.Text, this, "RptOC.rdlc", "Reporte de Orden de Corte.");
    }
}

