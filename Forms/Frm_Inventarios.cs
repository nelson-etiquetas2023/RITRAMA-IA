using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Drawing2D;
using System.Drawing.Printing;
using System.Globalization;
using System.Security.Cryptography;
using System.Text;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Helpers;
using Ritrama2025.LabelSdk;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.InventarioService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ReportsService.ReportsService;

using Sunny.UI;
namespace Ritrama2025.Forms;

public partial class Frm_Inventarios : UIForm, IFormTemaClaro
{
    IInventarioService InventarioService { get; set; }
    IProduccionService ProduccionService { get; set; }
    IExportDataService ExportDataService { get; set; }
    IReportsService ReportService { get; set; }
    // Busqueda directa a SQL Server (sin carga masiva local): Dv/DvRollos solo
    // contienen el resultado de la ultima busqueda con el boton Buscar.
    private DataView Dv { get; set; } = new DataTable().DefaultView;
    private DataView DvRollos { get; set; } = new DataTable().DefaultView;
    List<int> IndexSelects { get; set; } = [];
    [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public string PathFileName { get; set; } = null!;
    readonly List<ProductMAP> lista = [];
    readonly List<RolloCortado> listaRollos = [];
    readonly List<Product> ListaProductsNotFound = [];

    public Frm_Inventarios(IInventarioService inventarioService, IProduccionService produccionService, IExportDataService exportDataService, IReportsService reportService)
    {
        InventarioService = inventarioService;
        ProduccionService = produccionService;
        ExportDataService = exportDataService;
        ReportService = reportService;
        InitializeComponent();
        Text = "Inventario";

        // NOTE: runtime UI overrides removed so designer settings prevail.
        // The original code configured Sunny.UI style manager, set runtime colors and
        // attached custom draw/renderer logic here. Those runtime modifications
        // have been intentionally disabled so the Designer configuration is used
        // without being overwritten at runtime.

        // Si necesitas reaplicar programáticamente el tema, llama a ReaplicarTema() manualmente.
    }

    // IFormTemaClaro: reaplica el verde para pisar el UIStyleManager global del Main.
    public void ReaplicarTema() => AplicarTemaVerde();

    private void AplicarTemaVerde()
    {
        // No-op: quitar estilos aplicados en tiempo de ejecución para respetar el Designer
        return;
    }

    // Se eliminó el paint con degradado. La barra de título usa ahora un color sólido.

    private void TabPages_Inventario_DrawItem(object? sender, DrawItemEventArgs e)
    {
        if (sender is not UITabControl tc)
        {
            return;
        }

        Graphics g = e.Graphics;

        TabPage tab = tc.TabPages[e.Index];
        Rectangle tabRect = e.Bounds;

        // Dibujar pestaña con esquinas redondeadas
        int radius = 12; // radio de redondeo
        using GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        Rectangle rect = new Rectangle(tabRect.Left + 2, tabRect.Top + 4, tabRect.Width - 4, tabRect.Height - 8);
        path.AddArc(rect.X, rect.Y, radius * 2, radius * 2, 180, 90);
        path.AddArc(rect.Right - radius * 2, rect.Y, radius * 2, radius * 2, 270, 90);
        path.AddArc(rect.Right - radius * 2, rect.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
        path.AddArc(rect.X, rect.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
        path.CloseFigure();
        // determinar si está seleccionada
        bool selected = (e.State & DrawItemState.Selected) == DrawItemState.Selected;
        // fondo de la pestaña
        Color bg = selected ? (PANEL_TITULO?.BackColor ?? Color.FromArgb(110, 190, 40)) : Color.Transparent;
        using (SolidBrush brushBg = new SolidBrush(bg))
        {
            g.FillPath(brushBg, path);
        }

        // Dibujar borde sutil
        using (Pen pen = new Pen(Color.FromArgb(200, 200, 200), 1))
        {
            g.DrawPath(pen, path);
        }

        // No rellenar todo el fondo: dejar transparente y pintar sólo indicador para el seleccionado

        // dibujar icono si existe
        int imgIndex = tab.ImageIndex;
        int iconSize = imageList1?.ImageSize.Width ?? 0;
        int padding = 14; // más espacio alrededor del icono
        int iconX = tabRect.Left + padding;
        int iconY = tabRect.Top + (tabRect.Height - iconSize) / 2;
        if (imageList1 != null && imgIndex >= 0 && imgIndex < imageList1.Images.Count)
        {
            // dibujar icono un poco más grande centrado verticalmente
            imageList1.Draw(g, iconX, iconY, imgIndex);
        }

        // texto centrado verticalmente y alineado a la derecha del icono
        int textX = iconX + (iconSize > 0 ? iconSize + 16 : padding);
        Rectangle textRect = new Rectangle(textX, tabRect.Top, tabRect.Width - (textX - tabRect.Left) - padding, tabRect.Height);
        using (StringFormat sf = new StringFormat { LineAlignment = StringAlignment.Center })
        using (Font headerFont = new Font(tc.Font.FontFamily, tc.Font.Size, FontStyle.Bold))
        using (SolidBrush fore = new SolidBrush(selected ? Color.White : Color.FromArgb(48, 48, 48)))
        {
            g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.ClearTypeGridFit;
            g.DrawString(tab.Text, headerFont, fore, textRect, sf);
        }

        // indicador inferior verde cuando está seleccionado (en lugar de fondo completo)
        if (selected)
        {
            int barHeight = 6;
            Rectangle barRect = new Rectangle(tabRect.Left + 8, tabRect.Bottom - barHeight - 6, tabRect.Width - 16, barHeight);
            // Usar color del diseñador para el indicador en lugar del tema aplicado en tiempo de ejecución
            Color indicador = (PANEL_TITULO != null) ? PANEL_TITULO.BackColor : Color.FromArgb(110, 190, 40);
#pragma warning disable IDE0008 // Usar un tipo explícito
            using var barBrush = new SolidBrush(indicador);
#pragma warning restore IDE0008 // Usar un tipo explícito
            g.FillRectangle(barBrush, barRect);
        }
    }

    // Aplica una Region redondeada a un control para darle borde redondeado.
    private static void RedondearControl(Control c, int radio)
    {
        if (c == null || c.Width <= 0 || c.Height <= 0)
        {
            return;
        }

        using GraphicsPath path = new System.Drawing.Drawing2D.GraphicsPath();
        int d = radio * 2;
        path.AddArc(c.ClientRectangle.X, c.ClientRectangle.Y, d, d, 180, 90);
        path.AddArc(c.ClientRectangle.Right - d, c.ClientRectangle.Y, d, d, 270, 90);
        path.AddArc(c.ClientRectangle.Right - d, c.ClientRectangle.Bottom - d, d, d, 0, 90);
        path.AddArc(c.ClientRectangle.X, c.ClientRectangle.Bottom - d, d, d, 90, 90);
        path.CloseFigure();
        c.Region = new Region(path);
    }

    // Igual que el grid "detalle de rollos cortados" de la OC (grid_items):
    // fuente Microsoft Sans Serif 12F, fondo blanco, filas alternas verde claro,
    // encabezados y seleccion en verde.
    private void AplicarEstilosGrid(DataGridView? g)
    {
        // No-op: evitar aplicar estilos en tiempo de ejecución. Dejar que el Designer controle apariencia.
        return;
    }

    private void Frm_Inventarios_Load(object sender, EventArgs e)
    {
        if (TopLevel)
        {
            StartPosition = FormStartPosition.Manual;
            Location = new System.Drawing.Point(155, 45);
        }
        DefColumnsSheetExcel();
        BindingMasterGrid();
        // Painting de la columna "% Disponible" como barra de progreso visual.
        GridMaster.CellPainting += GridMaster_CellPainting;
        DefColumnsGridRollosCortados();

        ComboPrinters.Items.Clear();
        foreach (string impresora in PrinterSettings.InstalledPrinters)
        {
            ComboPrinters.Items.Add(impresora);
        }

        // AplicarTemaVerde() convertido a no-op; evitar cambios en tiempo de ejecución.
        // Se eliminó el ajuste de docking en tiempo de ejecución para respetar el Designer.

        // Sin carga inicial: los grids se llenan solo con el boton Buscar
        // (consulta directa a SQL Server con los filtros).
        GridMaster.DataSource = Dv;
        GridRollosCortados.DataSource = DvRollos;
        ContarRegistros();
        ContarRegistrosRollos();
        // Ajustar ancho de las pestañas para que queden menos anchas y alineadas
        try
        {
            int gridWidth = (GridMaster?.Width > 0) ? GridMaster.Width : 800;
            // queremos pestañas más estrechas; limitar entre 120 y 220 px y basarlo en el grid
            int desiredTabWidth = Math.Max(120, Math.Min(220, gridWidth - 800 > 0 ? 220 : 200));
            TabPages_Inventario.ItemSize = new Size(desiredTabWidth, TabPages_Inventario.ItemSize.Height);
            TabPages_Inventario.Padding = new Point(8, TabPages_Inventario.Padding.Y);
        }
        catch
        {
            // ignore en diseño
        }
        // No forzamos Z-order adicional en runtime; el diseñador controla el layout.
    }
    private void Toggleloading(bool isLoading)
    {
        panel_loading.Visible = isLoading;
        panel_loading.BringToFront();
        bot_buscar_cor.Enabled = !isLoading;
    }

    private List<Roll_Details> CreateListaRollosCortados()
    {
        List<Roll_Details> lista = [];
        for (int i = 0; i <= GridRollosCortados.Rows.Count - 1; i++)
        {
            Roll_Details rollo = new()
            {
                ItemNo = i + 1,
                Product_id = GridRollosCortados.Rows[i].Cells["product_id"].Value?.ToString() ?? string.Empty,
                Product_name = GridRollosCortados.Rows[i].Cells["product_name"].Value?.ToString() ?? string.Empty,
                Unique_code = GridRollosCortados.Rows[i].Cells["unique_code"].Value?.ToString() ?? string.Empty,
                Width = Convert.ToDecimal(GridRollosCortados.Rows[i].Cells["width"].Value),
                Large = Convert.ToDecimal(GridRollosCortados.Rows[i].Cells["lenght"].Value),
                Msi = Convert.ToDecimal(GridRollosCortados.Rows[i].Cells["msi"].Value),
                Roll_id = GridRollosCortados.Rows[i].Cells["roll_id"].Value?.ToString() ?? string.Empty,
                Numero_Orden = GridRollosCortados.Rows[i].Cells["numero"].Value?.ToString() ?? string.Empty,
                Splice = Convert.ToInt16(GridRollosCortados.Rows[i].Cells["splice"].Value),
                Status = GridRollosCortados.Rows[i].Cells["status"].Value?.ToString() ?? string.Empty,
                Ubic = GridRollosCortados.Rows[i].Cells["ubic"].Value?.ToString() ?? string.Empty,
                Code_Person = GridRollosCortados.Rows[i].Cells["code_person"].Value?.ToString() ?? string.Empty,
            };
            lista.Add(rollo);
        }
        return lista;
    }

    private List<ProductMAP> CreateListaMasterRolls()
    {
        List<ProductMAP> lista = [];
        for (int i = 0; i <= GridMaster.Rows.Count - 1; i++)
        {
            ProductMAP master = new()
            {
                ItemNo = i + 1,
                Product_Id = GridMaster.Rows[i].Cells["product_id"].Value?.ToString() ?? string.Empty,
                Product_Name = GridMaster.Rows[i].Cells["product_name"].Value?.ToString() ?? string.Empty,
                Rollid = GridMaster.Rows[i].Cells["roll_id"].Value?.ToString() ?? string.Empty,
                Width = Convert.ToDouble(GridMaster.Rows[i].Cells["width"].Value),
                Length = Convert.ToDouble(GridMaster.Rows[i].Cells["length"].Value),
                Length_Consumido = Convert.ToDouble(GridMaster.Rows[i].Cells["length_consumido"].Value),
                Length_Restante = Convert.ToDouble(GridMaster.Rows[i].Cells["length_restante"].Value),
                Estado = GridMaster.Rows[i].Cells["estado"].Value?.ToString() ?? string.Empty,
                //Msi = Convert.ToDouble(GridMaster.Rows[i].Cells["msi"].Value),
                //Ubic = GridMaster.Rows[i].Cells["ubic"].Value?.ToString() ?? string.Empty,
                //Cant = 1, // Assuming each row represents one roll
                //Recepcion = GridMaster.Rows[i].Cells["fecha"].Value?.ToString() ?? string.Empty,
                //Fecha_Fabricacion = Convert.ToDateTime(GridMaster.Rows[i].Cells["fecha_pro"].Value),
                //Fecha_Llegada = DateTime.Now, // Assuming current date for arrival
            };
            lista.Add(master);
        }
        return lista;
    }
    private void DefColumnsGridRollosCortados()
    {
        GridRollosCortados.AutoGenerateColumns = false;
        CommonService.ADD_COLUMN_GRID("product_id", 60, "Prod. Id", "product_id", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("product_name", 250, "Product Name", "product_name", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("unique_code", 60, "Unique Code", "unique_code", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("width", 60, "Width [Inch.]", "width", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("lenght", 60, "Lenght [Pies]", "large", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("msi", 60, "Msi", "msi", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("splice", 60, "Splice", "splice", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("roll_id", 80, "Roll-Id", "roll_id", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("code_person", 60, "Code Person.", "code_person", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("numero", 60, "Orden Corte", "numero", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("status", 60, "Status", "status", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("ubic", 80, "Ubicacion", "ubic", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("fecha", 80, "Creacion", "fecha", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("despacho", 80, "Doc. Despacho", "despacho", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("fecha_despacho", 80, "Fecha Despacho", "fecha_desPACHO", GridRollosCortados);
        CommonService.ADD_COLUMN_GRID("disponible", 80, "Disponible", "disponible", GridRollosCortados);
        //agregar la columna de images para el disponible del producto.
        DataGridViewImageColumn colEstado = new()
        {
            Name = "colEstado",
            HeaderText = "...",
            ImageLayout = DataGridViewImageCellLayout.Zoom,
            DisplayIndex = 0,
            Width = 16
        };
        GridRollosCortados.Columns.Add(colEstado);
    }
    private void BindingMasterGrid()
    {
        GridMaster.AutoGenerateColumns = false;
        CommonService.ADD_COLUMN_GRID("product_id", 80, "Prod. Id", "part_number", GridMaster);
        CommonService.ADD_COLUMN_GRID("product_name", 250, "Product Name", "product_name", GridMaster);
        CommonService.ADD_COLUMN_GRID("roll_id", 100, "Rollid", "roll_id", GridMaster);
        CommonService.ADD_COLUMN_GRID("width", 80, "Width", "width", GridMaster);
        CommonService.ADD_COLUMN_GRID("length", 80, "Length", "lenght", GridMaster);
        CommonService.ADD_COLUMN_GRID("length_consumido", 80, "Consumido", "largo_consumido", GridMaster);
        CommonService.ADD_COLUMN_GRID("length_restante", 80, "Restante", "largo_restante", GridMaster);
        // Columna visual "% Disponible": barra de progreso con color dinámico según porcentaje.
        // El rendering se hace en GridMaster_CellPainting con ProgressBarRenderer.
        CommonService.ADD_COLUMN_GRID("pct_disponible", 120, "% Disponible", "pct_disponible", GridMaster);
        CommonService.ADD_COLUMN_GRID("estado", 80, "Estado", "estado", GridMaster);
        CommonService.ADD_COLUMN_GRID("documento_oc", 120, "Documento OC", "documento_oc", GridMaster);
        CommonService.ADD_COLUMN_GRID("msi", 80, "Msi", "msi", GridMaster);
        CommonService.ADD_COLUMN_GRID("core", 80, "Core", "core", GridMaster);
        CommonService.ADD_COLUMN_GRID("fecha_pro", 100, "Produccion", "fecha_pro", GridMaster);
        CommonService.ADD_COLUMN_GRID("fecha_reg", 100, "Llegada", "fecha_reg", GridMaster);
        CommonService.ADD_COLUMN_GRID("splice", 80, "Splice", "splice", GridMaster);
        CommonService.ADD_COLUMN_GRID("ubic", 80, "Ubic. ", "ubicacion", GridMaster);
        CommonService.ADD_COLUMN_GRID("tipo_mov", 80, "Tipo", "tipo_mov", GridMaster);
        //Columna Check para seleccionar las filas a imprimnir.

        DataGridViewCheckBoxColumn colSelPrint = new()
        {
            HeaderText = "Sel.",
            ReadOnly = false,
            DisplayIndex = 0,
            Width = 30,
            Name = "colSelPrint"
        };
        GridMaster.Columns.Add(colSelPrint);


    }

    private void ContarRegistros()
    {
        COUNT_ROWS.Text = Dv.Count.ToString() + " Registros Encontrados." ?? "0 Registros Encontrados";
    }
    private void ContarRegistrosRollos()
    {
        COUNTER_ROLLOS.Text = DvRollos.Count.ToString() + " Registros Encontrados." ?? "0 Registros Encontrados";
    }
    private static void DefColumnsSheetExcel()
    {
        //llenar la lista de las columnas.
        //var columnas = new List<ColumnaType>()
        //{
        //    new() { Description = "Product Id   ", Index = 1, TipoValor = "string  " },
        //    new() { Description = "Product Name ", Index = 2, TipoValor = "string  " },
        //    new() { Description = "Width        ", Index = 3, TipoValor = "decimal " },
        //    new() { Description = "Length       ", Index = 4, TipoValor = "decimal " },
        //    new() { Description = "Msi          ", Index = 5, TipoValor = "decimal " }
        //};
    }
    private void Btn_load_sheet_Click(object sender, EventArgs e)
    {
        //validacion del tipo de producto
        if (!rad_master.Checked && !rad_graphics.Checked && !rad_rollos.Checked)
        {
            MessageBox.Show("Debe escoger el tipo de producto primero...");
            return;
        }
        //open dialog para seleccionar el archivo de excel
        OpenFileDialog dialog = new()
        {
            Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
            Title = "Select an Excel File"
        };
        dialog.ShowDialog();

        string filePath = dialog.FileName;
        string fileName = System.IO.Path.GetFileName(filePath);

        txt_file_name.Text = fileName;
        txt_file_path.Text = filePath;
    }
    private void Btn_import_excel_Click(object sender, EventArgs e)
    {
        Frm_Imports importData = new(InventarioService)
        {
            FileName = txt_file_name.Text,
            PathFileName = txt_file_path.Text
        };
        importData.ShowDialog();
    }

    private async void Btn_buscar_Click(object sender, EventArgs e)
    {
        // Preparar parámetros según radio seleccionado para pasar al servicio
        // (el botón Buscar en Inventario siempre busca en Master)
        string? rollidParam = null;
        string? productIdParam = null;
        string? productNameParam = null;
        string? ubicacionParam = null;
        string? estadoParam = null;

        // Aplicar filtro segun el radiobutton seleccionado (solo si hay texto de búsqueda)
        if (!string.IsNullOrWhiteSpace(txt_buscar.Text))
        {
            if (rad_rollid.Checked)
            {
                rollidParam = txt_buscar.Text?.Trim();
            }
            else if (rad_productid.Checked)
            {
                productIdParam = txt_buscar.Text?.Trim();
            }
            else if (rad_product_name.Checked)
            {
                productNameParam = txt_buscar.Text?.Trim();
            }
            else if (rad_ubication.Checked)
            {
                ubicacionParam = txt_buscar.Text?.Trim();
            }
        }
        // si el usuario tiene un radio seleccionado pero no escribió texto,
        // los parámetros permanecen en null y el servicio regresa todos los registros
        // si hay más radios para estado, añadir aquí

        DataTable? dt = await InventarioService.BuscarMasterInventario(
            rollid: rollidParam,
            productId: productIdParam,
            productName: productNameParam,
            ubicacion: ubicacionParam,
            estado: estadoParam
        );

        Dv = dt!.DefaultView;
        ContarRegistros();
        GridMaster.DataSource = Dv;
    }
    private void Btn_limpiar_filtros_Click(object sender, EventArgs e)
    {
        txt_buscar.Text = string.Empty;
        Dv.RowFilter = string.Empty;
        ContarRegistros();
    }

    private void Btn_DetailsConsumos_Click(object sender, EventArgs e)
    {
        Frm_DetailsConsumos frmDetails = new(ProduccionService)
        {
            Rollid = GridMaster.CurrentRow?.Cells["roll_id"].Value?.ToString() ?? string.Empty,
            Productid = GridMaster.CurrentRow?.Cells["product_id"].Value?.ToString() ?? string.Empty,
            Product_Name = GridMaster.CurrentRow?.Cells["product_name"].Value?.ToString() ?? string.Empty,
            Width_t = GridMaster.CurrentRow?.Cells["width"].Value!.ToString() ?? string.Empty,
            Length = GridMaster.CurrentRow?.Cells["length"].Value!.ToString() ?? string.Empty,
        };
        frmDetails.ShowDialog();
    }

    private async void Bot_Excel_Click(object sender, EventArgs e)
    {

        string activeTabtext = TabPages_Inventario.SelectedTab!.Text;

        if (activeTabtext == "Master")
        {
            if (GridMaster.Rows.Count == 0)
            {
                MessageBox.Show("Cargue los datos primero...");
                return;
            }
        }

        if (activeTabtext == "Rollos Cortados")
        {
            if (GridRollosCortados.Rows.Count == 0)
            {
                MessageBox.Show("Cargue los datos primero...");
                return;
            }
        }

        try
        {
            if (activeTabtext == "Master")
            {
                Toggleloading(true);
                List<ProductMAP> listaMasterRolls = CreateListaMasterRolls();
                await Task.Run(() =>
                {
                    ExportDataService.ExportToExcel<ProductMAP>(listaMasterRolls, "InventarioMaster.xlsx");
                });
            }

            if (activeTabtext == "Rollos Cortados")
            {
                Toggleloading(true);
                List<Roll_Details> listaRollosCortados = CreateListaRollosCortados();
                await Task.Run(() =>
                {
                    ExportDataService.ExportToExcel<Roll_Details>(listaRollosCortados, "Inventario_RollosCortados.xlsx");
                });
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al exportar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
        finally
        {
            Toggleloading(false);
        }
    }

    private void Bot_Txt_Click(object sender, EventArgs e)
    {

        string activeTabtext = TabPages_Inventario.SelectedTab!.Text;

        if (activeTabtext == "Master")
        {
            if (GridMaster.Rows.Count == 0)
            {
                MessageBox.Show("Cargue los datos primero...");
                return;
            }
            Toggleloading(true);
            try
            {
                DataRow[] listaMasters = [.. Dv.ToTable().AsEnumerable().Where(r => r.RowState != DataRowState.Deleted)];

                ExportFileTextFormat(listaMasters);
            }
            finally
            {
                Toggleloading(false);
            }
        }

        if (activeTabtext == "Rollos Cortados")
        {
            if (GridRollosCortados.Rows.Count == 0)
            {
                MessageBox.Show("Cargue los datos primero...");
                return;
            }
            Toggleloading(true);
            try
            {
                DataRow[] ListaRollos = [.. DvRollos.ToTable().AsEnumerable().Where(r => r.RowState != DataRowState.Deleted)];

                ExportFileTextFormat_Rollo_Cortado(ListaRollos);
            }
            finally
            {
                Toggleloading(false);
            }
        }



    }

    private static bool ExportFileTextFormat_Rollo_Cortado(DataRow[] listaCortados)
    {
        try
        {
            string folderPath = System.IO.Path.Combine(Application.StartupPath, "Archivos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = System.IO.Path.Combine(folderPath, "IRolloCortado.txt");
            using (StreamWriter sr = new(filePath))
            {
                foreach (DataRow item in listaCortados)
                {
                    string product_id = item["product_id"].ToString()!.Trim();
                    string product_name = item["product_name"].ToString()!.Trim();
                    string unique_code = item["unique_code"].ToString()!.Trim();
                    string width = item["width"].ToString()!.Trim();
                    string length = item["large"].ToString()!.Trim();
                    string splice = item["splice"].ToString()!.Trim();
                    string rollid = item["roll_id"].ToString()!.Trim();
                    string code_per = item["code_person"].ToString()!.Trim();
                    string orden = item["numero"].ToString()!.Trim();
                    string status = item["status"].ToString()!.Trim();
                    string ubic = item["ubic"].ToString()!.Trim();
                    string fecha_crea = item["fecha"].ToString()!.Trim();
                    string despacho = item["despacho"].ToString()!.Trim();
                    string fecha_des = item["fecha_despacho"].ToString()!.Trim();

                    string linea = $"{product_id},{product_name},{unique_code},{width},{length},{splice},{rollid},{code_per},{orden},{status},{ubic},{fecha_crea},{despacho},{fecha_des}";

                    sr.WriteLine(linea);
                }
            }
            //abri el archivo con el programa predeterminado.
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            };
            Process.Start(psi);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al crear el inventario de rollo cortados...: " + ex.Message);
            return false;
        }
    }

    private static bool ExportFileTextFormat(DataRow[] listaMasters)
    {
        try
        {
            string folderPath = System.IO.Path.Combine(Application.StartupPath, "Archivos");
            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }
            string filePath = System.IO.Path.Combine(folderPath, "Imaster.txt");
            using (StreamWriter sr = new(filePath))
            {
                foreach (DataRow item in listaMasters)
                {
                    string product_id = item["part_number"].ToString()!.Trim();
                    string product_name = item["product_name"].ToString()!.Trim();
                    string rollid = item["roll_id"].ToString()!.Trim();
                    string width = item["width"].ToString()!.Trim();
                    string lenght = item["lenght"].ToString()!.Trim();
                    string length_Consumido = item["largo_consumido"].ToString()!.Trim();
                    string length_Restante = item["largo_restante"].ToString()!.Trim();
                    string estado = item["estado"].ToString()!.Trim();
                    //string fec_produc = item["fecha_pro"].ToString()!.Trim();
                    //string fec_ingreso = item["fecha_recep"].ToString()!.Trim();
                    //string splice = item["splice"].ToString()!.Trim();
                    //string ubic = item["ubicacion"].ToString()!.Trim();
                    //string tipo_mov = item["tipo_mov"].ToString()!.Trim();

                    string linea = $"{product_id},{product_name},{rollid},{width},{lenght},{length_Consumido}," +
                        $"{length_Restante},{estado}";

                    sr.WriteLine(linea);
                }
            }
            //abri el archivo con el programa predeterminado.
            ProcessStartInfo psi = new ProcessStartInfo
            {
                FileName = filePath,
                UseShellExecute = true
            };
            Process.Start(psi);
            return true;
        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al crear el inventario de masters...: " + ex.Message);
            return false;
        }
    }

    private void GridMaster_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        if (GridMaster.Columns[e.ColumnIndex].Name == "estado")
        {
            try
            {
                string estado = Convert.ToString(e.Value)!;
                if (estado == "Desperdicio" || estado == "Agotado")
                {
                    e.CellStyle.BackColor = System.Drawing.Color.Red;
                    e.CellStyle.ForeColor = System.Drawing.Color.White;
                }
                if (estado == "Completo")
                {
                    e.CellStyle.BackColor = System.Drawing.Color.Green;
                    e.CellStyle.ForeColor = System.Drawing.Color.White;
                }
                if (estado == "Parcialmente Consumido")
                {
                    e.CellStyle.BackColor = System.Drawing.Color.Orange;
                    e.CellStyle.ForeColor = System.Drawing.Color.White;
                }
            }
            catch (Exception)
            {
                e.CellStyle.BackColor = System.Drawing.Color.White;
                throw;
            }
        }
    }

    /// <summary>
    /// Pinta la columna "% Disponible" como barra de progreso visual con color
    /// dinámico según el porcentaje (verde/amarillo/naranja/rojo).
    /// </summary>
    private void GridMaster_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
    {
        if (e.RowIndex < 0 || e.ColumnIndex < 0)
        {
            return;
        }

        if (GridMaster.Columns[e.ColumnIndex].Name != "pct_disponible")
        {
            return;
        }

        DataGridViewRow fila = GridMaster.Rows[e.RowIndex];
        if (fila.IsNewRow)
        {
            return;
        }

        double pct = ProgressBarRenderer.CalcularDesdeFila(fila, "length", "length_restante");
        ProgressBarRenderer.PaintCell(e.Graphics, e.CellBounds, e.State, pct, e.CellStyle,
            (e.State & DataGridViewElementStates.Selected) != 0);
        e.Handled = true;
    }

    private void Bot_buscar_cor_Click(object sender, EventArgs e)
    {
        if (rad_rollid_cor.Checked)
        {
            DvRollos.RowFilter = "roll_id like '%" + txt_buscar_cor.Text + "%'";
        }
        if (rad_productid_cor.Checked)
        {
            DvRollos.RowFilter = "product_id like '%" + txt_buscar_cor.Text + "%'";
        }
        if (rad_productname_cor.Checked)
        {
            DvRollos.RowFilter = "product_name like '%" + txt_buscar_cor.Text + "%'";
        }
        if (rad_ubic_cor.Checked)
        {
            DvRollos.RowFilter = "ubic like '%" + txt_buscar_cor.Text + "%'";
        }
        if (rad_codeunique_cor.Checked)
        {
            DvRollos.RowFilter = "unique_code like '%" + txt_buscar_cor.Text + "%'";
        }
        if (rad_codeperson_cor.Checked)
        {
            DvRollos.RowFilter = "code_person like '%" + txt_buscar_cor.Text + "%'";
        }
        if (rad_ordencorte_cor.Checked)
        {
            DvRollos.RowFilter = "CONVERT(numero, 'System.String') LIKE '%" + txt_buscar_cor.Text + "%'";
        }

        ContarRegistrosRollos();

    }

    private void Bto_limpiar_cor_Click(object sender, EventArgs e)
    {
        txt_buscar_cor.Text = string.Empty;
        GridRollosCortados.DataSource = "";
        ContarRegistrosRollos();
    }

    private void PictureBox3_Click(object sender, EventArgs e)
    {

    }

    private void ToolStrip1_ItemClicked(object sender, ToolStripItemClickedEventArgs e)
    {

    }

    private void Bot_Reports_Click(object sender, EventArgs e)
    {
        string activeTabtext = TabPages_Inventario.SelectedTab!.Text;

        if (activeTabtext == "Master")
        {
            if (GridMaster.Rows.Count == 0)
            {
                MessageBox.Show("Cargue los datos primero...");
                return;
            }

            try
            {
                ReportService.Reporte_InventarioMaster(this, "Inventario de Master", "Report_Inventario_Master.rdlc");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }
        }

        if (activeTabtext == "Rollos Cortados")
        {
            if (GridRollosCortados.Rows.Count == 0)
            {
                MessageBox.Show("Cargue los datos primero...");
                return;
            }
            ReportService.Reporte_InventarioRollosCortados(this, "Inventario de Rollos Cortados", "Report_Inventarios_RollosCortados.rdlc");
        }

    }

    private void GridRollosCortados_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
    {
        //obtener el valor de la columna disponible.

        if (GridRollosCortados.Columns[e.ColumnIndex].Name == "colEstado")
        {
            //obtener el valor de la columna disponible.
            bool dispo = Convert.ToBoolean(GridRollosCortados.Rows[e.RowIndex].Cells["disponible"].Value);

            if (dispo)
            {
                //var row = GridRollosCortados.Rows[e.RowIndex];
                //row.DefaultCellStyle.BackColor = System.Drawing.Color.White;
                //row.DefaultCellStyle.ForeColor = System.Drawing.Color.Black;
                e.Value = Properties.Resources.products_dispo;

            }
            else
            {
                //var row = GridRollosCortados.Rows[e.RowIndex];
                //row.DefaultCellStyle.BackColor = System.Drawing.Color.LightCoral;
                //row.DefaultCellStyle.ForeColor = System.Drawing.Color.White;
                e.Value = Properties.Resources.products_nodispo;
            }
        }
    }


    private const string EtiquetaMasterZpl =
            "^FO20,20^A0N,40,40^FD{product_name}^FS\r\n" +
            "^FO20,100^A0N,30,30^FDROLL ID: {rollid}^FS\r\n" +
            "^BY2,3,80\r\n" +
            "^FO20,160^BCN,80,Y,N,N^FD{rollid}^FS\r\n" +
            "^FO20,280^A0N,30,30^FDPRODUCT ID: {product_id}^FS\r\n" +
            "^FO20,330^A0N,30,30^FDWIDTH: {width} in.  LENGTH: {lenght} ft.^FS\r\n" +
            "^FO20,380^A0N,30,30^FDMSI: {msi}  SPLICE: {splice}^FS\r\n" +
            "^FO20,430^A0N,30,30^FDFECHA: {fecha}  ESTADO: {estado}^FS\r\n" +
            "^FO20,480^A0N,30,30^FDUBICACION: {ubicacion}^FS";

    private static string CellValue(DataGridViewRow fila, string columnName)
    {
        if (fila.DataGridView == null || !fila.DataGridView.Columns.Contains(columnName))
        {
            return "";
        }

        object? valor = fila.Cells[columnName].Value;
        return valor?.ToString() ?? "";
    }

    private static string FormatearFecha(string fecha)
    {
        if (DateTime.TryParse(fecha, out DateTime fechaOk))
        {
            return fechaOk.ToString("dd/MM/yyyy");
        }

        return fecha;
    }

    private void Bot_printLabel_Click(object sender, EventArgs e)
    {
        if (ComboPrinters.SelectedItem == null)
        {
            MessageBox.Show("seleccione una impresora primero...");
            return;
        }

        string impresora = ComboPrinters.SelectedItem.ToString()!;
        int impresas = 0;

        foreach (DataGridViewRow fila in GridMaster.Rows)
        {
            bool RowSelect = fila.Cells["colSelPrint"].Value != null && Convert.ToBoolean(fila.Cells["colSelPrint"].Value);

            if (RowSelect)
            {
                Dictionary<string, string> valores = new()
                {
                    { "product_id", CellValue(fila, "product_id") },
                    { "product_name", CellValue(fila, "product_name") },
                    { "rollid", CellValue(fila, "roll_id") },
                    { "fecha", FormatearFecha(CellValue(fila, "fecha_pro")) },
                    { "width", CellValue(fila, "width") },
                    { "lenght", CellValue(fila, "length") },
                    { "msi", CellValue(fila, "msi") },
                    { "splice", CellValue(fila, "splice") },
                    { "estado", CellValue(fila, "estado") },
                    { "ubicacion", CellValue(fila, "ubic") }
                };

                bool ok = ZebraTemplateEngine.Print(impresora, EtiquetaMasterZpl, valores, StandardLabelSizes.Size_4x6_203dpi);
                if (ok)
                {
                    impresas++;
                }
                else
                {
                    MessageBox.Show("Error al imprimir la etiqueta del rollo: " + CellValue(fila, "roll_id"), "Error en impresion", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        if (impresas > 0)
        {
            MessageBox.Show(impresas + " etiqueta(s) impresa(s) correctamente.");
        }
        else
        {
            MessageBox.Show("No se selecciono ninguna fila para imprimir.", "Aviso");
        }
    }

    private void GridRollosCortados_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

    }

    private void GridMaster_CellContentClick(object sender, DataGridViewCellEventArgs e)
    {

    }

    private void Btn_delete_master_Click(object sender, EventArgs e)
    {
        foreach (DataGridViewRow fila in GridMaster.Rows)
        {
            bool RowSelect = Convert.ToBoolean(fila.Cells["colSelPrint"].Value);
            bool estadoCompleto = fila.Cells["estado"].Value?.ToString()!.ToUpper() == "COMPLETO";

            if (RowSelect)
            {
                if (estadoCompleto)
                {
                    string rollid = fila.Cells["roll_id"].Value?.ToString()!;
                    InventarioService.BorrarMasterDB(rollid);
                }
                else
                {
                    MessageBox.Show("Solo se pueden eliminar los master que esten en estado COMPLETO...");
                }

            }
        }

        MessageBox.Show("Proceso terminado...");
    }

    private void Label13_Click(object sender, EventArgs e)
    {

    }

    private void Rad_MasterCompleto_CheckedChanged(object sender, EventArgs e)
    {
        rad_rollid.Checked = false;
    }

    private void Rad_MasterParcial_CheckedChanged(object sender, EventArgs e)
    {
        rad_rollid.Checked = false;
    }

    private void Rad_MasterConsumido_CheckedChanged(object sender, EventArgs e)
    {
        rad_rollid.Checked = false;
    }

    private void GroupBox1_Enter(object sender, EventArgs e)
    {

    }

    private void Btn_dropmaster_Click(object sender, EventArgs e)
    {
        //Mostrar mensaje de confirmacion antes de eliminar todos los master.
        DialogResult resultado = MessageBox.Show("� Esta seguro de eliminar todos los datos de la tabla de masters.?",
            "Advertencia", MessageBoxButtons.YesNo, MessageBoxIcon.Warning);

        if (resultado == DialogResult.Yes)
        {
            InventarioService.DropTableInit(cbo_tabla.SelectedIndex);
            MessageBox.Show("Se eliminaron los datos correctamente.", "Aviso");
        }
        else
        {
            MessageBox.Show("Acci�n cancelada", "Aviso");
        }
    }

    private void Btn_load_data_Click(object sender, EventArgs e)
    {
        Toggleloading(true);
        Grid_Items.DataSource = "";
        try
        {
            if (rad_master.Checked)
            {
                LoadDataMaster();
            }
            if (rad_rollos.Checked)
            {
                LoadDataRolloCortados();
            }
        }
        finally
        {
            Toggleloading(false);
        }

        chk_saveproductsnotfound.Enabled = true;
        btn_accion.Enabled = true;
    }

    private void LoadDataRolloCortados()
    {
        lista.Clear();
        string filePath = txt_file_path.Text.Trim();
        if (string.IsNullOrWhiteSpace(filePath))
        {
            MessageBox.Show("seleccione la hoja de excel primero...");
            return;
        }
        try
        {
            using XLWorkbook workbook = new XLWorkbook(filePath);
            IXLWorksheet worksheet = workbook.Worksheet(1);
            //Empiezo en la fila 2 por los encabezados.
            IEnumerable<IXLRow> filas = worksheet.Rows().Skip(1);
            // recorro filas donde esta la data de la hoja.
            //crear el validador de excel.
            ExcelValidator validator = new ExcelValidator();
            int itemno = 1;
            foreach (IXLRow? item in filas)
            {
                RolloCortado producto = new()
                {
                    item = itemno++,
                    Product_Id = item.Cell(1).Value.ToString(),
                    Product_Name = item.Cell(2).Value.ToString(),
                    Width = validator.TryGetDouble(item.Cell(3), worksheet),
                    Length = validator.TryGetDouble(item.Cell(4), worksheet),
                    Msi = validator.TryGetDouble(item.Cell(5), worksheet),
                    UniqueCode = item.Cell(6).Value.ToString(),
                    Splice = (int)(double)item.Cell(7).Value,
                    Code_Person = item.Cell(8).Value.ToString(),
                    Ubicacion = item.Cell(9).Value.ToString()
                };
                listaRollos.Add(producto);
                txt_log_notifications.Text = validator.Errores.ToString();
            }
            Grid_Items.DataSource = listaRollos;
            AplicarEstilosGrid(Grid_Items);






        }
        catch (System.IO.IOException ex)
        {
            MessageBox.Show("Error al tratar de abrir la hoja de excel, " +
                "si esta abierta por favor cierrela y vuelva a intentarlo...[error code:] " + ex.Message);
        }
        txt_number_rows.Text = Grid_Items.Rows.Count.ToString();



    }


    private void LoadDataMaster()
    {
        lista.Clear();
        string filePath = txt_file_path.Text.Trim();

        if (string.IsNullOrWhiteSpace(filePath))
        {
            MessageBox.Show("seleccione la hoja de excel primero...");
            return;
        }
        //string fileName = FileName;
        //leer la hoja de excel.
        try
        {
            using XLWorkbook workbook = new XLWorkbook(filePath);
            IXLWorksheet worksheet = workbook.Worksheet(1);
            //Empiezo en la fila 2 por los encabezados.
            IEnumerable<IXLRow> filas = worksheet.Rows().Skip(1);
            // recorro filas donde esta la data de la hoja.
            //crear el validador de excel.
            ExcelValidator validator = new ExcelValidator();
            int itemno = 1;
            //1.- validaciones de las columnas



            foreach (IXLRow? item in filas)
            {
                ProductMAP producto = new()
                {
                    ItemNo = itemno++,
                    Product_Id = item.Cell(1).Value.ToString(),
                    Product_Name = item.Cell(2).Value.ToString(),
                    Rollid = item.Cell(3).Value.ToString(),
                    Width = validator.TryGetDouble(item.Cell(4), worksheet),
                    Length = validator.TryGetDouble(item.Cell(5), worksheet),
                    Splice = validator.TryGetInt(item.Cell(6), worksheet),
                    Fecha_Produccion = validator.TryGetDateTime(item.Cell(7), worksheet),
                    Factura = item.Cell(8).Value.ToString(),
                    Ubic = item.Cell(9).Value.ToString(),
                    Fecha_Llegada = validator.TryGetDateTime(item.Cell(10), worksheet),
                    Paleta = item.Cell(11).Value.ToString(),
                };
                lista.Add(producto);
                txt_log_notifications.Text = validator.Errores.ToString();
            }
            Grid_Items.DataSource = lista;
            AplicarEstilosGrid(Grid_Items);
            //2.- validar productos que no existen en la base de datos
            if (chk_valid_products.Checked && rad_master.Checked)
            {
                //validacion solo para master
                ValidProductsMasterNotFoundDB();
            }
            //3.- Validar que rollid no se repitan. 
            if (chk_repeat_rollid.Checked && rad_master.Checked)
            {
                ValidFieldsMasterExcelSheet(validator);

            }
        }
        catch (System.IO.IOException ex)
        {
            MessageBox.Show("Error al tratar de abrir la hoja de excel, " +
                "si esta abierta por favor cierrela y vuelva a intentarlo...[error code:] " + ex.Message);
        }
        txt_number_rows.Text = Grid_Items.Rows.Count.ToString();
    }




    private void ValidFieldsMasterExcelSheet(ExcelValidator ev)
    {
        string filasduplex = "";

        List<IGrouping<string, ProductMAP>> rollid_duplex = lista.GroupBy(r => r.Rollid)
            .Where(g => g.Count() > 1).ToList();

        if (rollid_duplex.Count != 0)
        {
            foreach (IGrouping<string, ProductMAP> grupo in rollid_duplex)
            {

                filasduplex = string.Join(",", grupo.Select(r => r.ItemNo.ToString()));


                txt_log_notifications.Text += $"El rollid {grupo.Key} " +
                    $"se repite {grupo.Count()} veces en las filas {filasduplex}.\n";
            }
        }
        //NUMERO DE ERRORES    
        int numeroDeLineas = (ev.Errores.ToString().Split(Environment.NewLine).Length) - 1;
        txt_errors.Text = numeroDeLineas.ToString();
        txt_log_notifications.Text = ev.Errores.ToString();
    }

    private void ValidProductsMasterNotFoundDB()
    {
        string messageProduct = "";

        foreach (ProductMAP item in lista)
        {
            //verifico si existe en la base de datos.
            if (!InventarioService.ValidProductid(item.Product_Id))
            {
                messageProduct = "-> PRODUCTO NO EXISTE: " + " [ " + item.Product_Id + " - "
                + item.Product_Name + " ] " + Environment.NewLine;

                //crear la notificacion.
                txt_log_notifications.Text += messageProduct;

                //Agrego a una lista de productos no encontrados.
                ListaProductsNotFound.Add(new Product
                {
                    Product_id = item.Product_Id,
                    Product_Name = item.Product_Name,
                });
            }
        }
    }
    private void DefineColumnsGridMaster()
    {
        if (Grid_Items.Columns.Count > 1)
        {
            Grid_Items.Columns.Clear();
        }
        Grid_Items.AutoGenerateColumns = false;
        CommonService.ADD_COLUMN_GRID("item", 30, "It.", "itemNo", Grid_Items);
        CommonService.ADD_COLUMN_GRID("product_id", 50, "Product Id.", "product_id", Grid_Items);
        CommonService.ADD_COLUMN_GRID("product_name", 300, "Product Name", "product_name", Grid_Items);
        CommonService.ADD_COLUMN_GRID("rollid", 70, "Roll-Id", "rollid", Grid_Items);
        CommonService.ADD_COLUMN_GRID("width", 70, "Width [Inch.]", "Width", Grid_Items);
        CommonService.ADD_COLUMN_GRID("lenght", 70, "Length [Pies.]", "length", Grid_Items);
        CommonService.ADD_COLUMN_GRID("splice", 70, "Splice", "splice", Grid_Items);
        CommonService.ADD_COLUMN_GRID("fecha_fabricacion", 70, "Fecha Produccion", "fecha_produccion", Grid_Items);
        CommonService.ADD_COLUMN_GRID("recep", 70, "Recepcion", "factura", Grid_Items);
        CommonService.ADD_COLUMN_GRID("ubic", 70, "Ubicacion", "ubic", Grid_Items);
        CommonService.ADD_COLUMN_GRID("fecha_llegada", 70, "Fecha Llegada", "fecha_llegada", Grid_Items);
        CommonService.ADD_COLUMN_GRID("paleta", 70, "Paleta", "paleta", Grid_Items);
    }
    private void Rad_master_CheckedChanged(object sender, EventArgs e)
    {
        if (rad_master.Checked)
        {
            int option = 1;
            Grid_Items.DataSource = "";
            DefineColumnGridType(option);
        }

    }

    private void DefineColumnGridType(int option)
    {
        if (option == 1)
        {
            DefineColumnsGridMaster();
        }
        if (option == 4)
        {
            DefineColumnsGridRollosCortados();
        }
    }

    private void DefineColumnsGridRollosCortados()
    {
        if (Grid_Items.Columns.Count > 1)
        {
            Grid_Items.Columns.Clear();
        }
        Grid_Items.AutoGenerateColumns = false;
        CommonService.ADD_COLUMN_GRID("item", 30, "It.", "item", Grid_Items);
        CommonService.ADD_COLUMN_GRID("product_id", 50, "Product Id.", "product_id", Grid_Items);
        CommonService.ADD_COLUMN_GRID("product_name", 300, "Product Name", "product_name", Grid_Items);
        CommonService.ADD_COLUMN_GRID("width", 70, "Width [Inch.]", "Width", Grid_Items);
        CommonService.ADD_COLUMN_GRID("lenght", 70, "Length [Pies.]", "length", Grid_Items);
        CommonService.ADD_COLUMN_GRID("msi", 70, "Msi.", "msi", Grid_Items);
        CommonService.ADD_COLUMN_GRID("uniquecode", 100, "Codigo Unico", "uniquecode", Grid_Items);
        CommonService.ADD_COLUMN_GRID("splice", 70, "Splice", "splice", Grid_Items);
        CommonService.ADD_COLUMN_GRID("code_person", 70, "Codigo Personalizado", "code_person", Grid_Items);
        CommonService.ADD_COLUMN_GRID("ubic", 70, "Ubicacion", "ubicacion", Grid_Items);

    }

    private void Btn_saveDatabase_Click(object sender, EventArgs e)
    {
        try
        {
            Toggleloading(true);
            if (!int.TryParse(txt_errors.Text, out int errors))
            {
                MessageBox.Show("No se pudo leer el contador de errores de la hoja...");
                return;
            }

            if (errors > 0)
            {
                MessageBox.Show("No se pueden Guardar los datos mientra la hoja de excel tenga errores en los datos...");
                return;
            }

            // validar ssi se cargo la hoja de excel.
            if (txt_file_name.Text.Length == 0)
            {
                MessageBox.Show("seleccione la hoja de excel primero...");
                return;
            }

            //validar tipo de producto
            if (!rad_master.Checked && !rad_rollos.Checked)
            {
                MessageBox.Show("debe seleccionar el tipo de producto...");
                return;
            }

            //validar que esten cargados los datos.
            if (Grid_Items.Rows.Count == 0)
            {
                MessageBox.Show("cargue los datos, desde la hoja");
                return;
            }

            if (rad_master.Checked)
            {
                GuardarMasterBD();
            }

            if (rad_rollos.Checked)
            {
                GuardarRolloCortados();
            }


        }
        catch (Exception ex)
        {
            MessageBox.Show("Error al guardar los datos del inventario, error code => " + ex);

        }
        finally
        {
            Toggleloading(false);
        }


    }

    private void GuardarMasterBD()
    {
        //validar la lista para que no se repita en la base de datos
        foreach (ProductMAP item in lista)
        {
            if (InventarioService.ValidRollId(item.Rollid))
            {
                txt_log_notifications.Text += "RollId ya existe en la base datos => " + item.Rollid + Environment.NewLine;
            }
            else
            {
                //Guardar en Base de Datos.
                InventarioService.SaveMasterInitialDB(item);
            }
        }
    }

    private void GuardarRolloCortados()
    {
        foreach (RolloCortado item in listaRollos)
        {
            InventarioService.SaveRolloCortado(item);
        }
    }




    private void Btn_accion_Click(object sender, EventArgs e)
    {
        //Guardar los productos no encontrados en la base de datos.
        if (chk_saveproductsnotfound.Checked)
        {
            SaveProductsNotDFoundDB();
        }
    }
    private void SaveProductsNotDFoundDB()
    {
        //recorrer la lista de productos no encotrados.
        foreach (Product item in ListaProductsNotFound)
        {
            Product producto = new Product
            {
                Product_id = item.Product_id,
                Product_Name = item.Product_Name,
                Product_Description = item.Product_Name,
                Master = true,
                Anulado = false,
                Hoja = false,
                Graphics = false,
                RolloCortado = false,
            };
            try
            {
                InventarioService.InsertProduct(producto);
            }
            catch (SqlException ex)
            {
                MessageBox.Show("Error al guardar el producto en la base de datos, [error code:] " + ex.Message);
            }

            //notificar que se creo el producto.
            txt_log_notifications.Text = $"Se creo el producto {producto.Product_id} " +
                $"- {producto.Product_Name} en la base de datos." + Environment.NewLine;

            MessageBox.Show("Se actualizaron los productos del sistema...");
        }
    }

    private void Btn_clearGrid_Click(object sender, EventArgs e)
    {
        Grid_Items.DataSource = "";
        lista.Clear();
        listaRollos.Clear();
    }

    private void Rad_rollos_CheckedChanged_1(object sender, EventArgs e)
    {
        if (rad_rollos.Checked)
        {
            int option = 4;
            Grid_Items.DataSource = "";
            DefineColumnGridType(option);
        }
    }
}
public class ColumnaType
{
    public string Description { get; set; } = null!;
    public int Index { get; set; }
    public string TipoValor { get; set; } = null!;

    public string InfoParaDisplay
    {
        get
        {
            // PadRight alinea el texto agregando espacios a la derecha.
            // Ajusta el n�mero (25) seg�n el ancho que necesites para la primera columna.
            return $"{Description}{TipoValor}{Index}";
        }
    }
}




