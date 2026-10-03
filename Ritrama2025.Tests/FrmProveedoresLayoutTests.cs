using System.Data;
using FluentAssertions;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProveedorService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño 30/70 de FrmProveedores: buscador, cuadro de
/// resumen (total/filtrado), grid y página de detalle refrescada por selección,
/// con los valores como textbox editables (solo id y estado de solo lectura).
/// No toca base de datos: el servicio va con un stub.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmProveedoresLayoutTests
{
    /// <summary>Servicio mínimo para instanciar el formulario sin dependencias reales.</summary>
    private sealed class ProveedorServiceStub : IProveedorService
    {
        private readonly DataTable _datos;

        public ProveedorServiceStub(DataTable? datos = null)
            => _datos = datos ?? new DataTable();

        public Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_datos);

        public Task<Result<bool>> AddValidatedAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> UpdateValidatedAsync(Proveedor proveedor, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(false));

        public Result ValidateProveedor(Proveedor proveedor)
            => Result.Success();
    }

    /// <summary>Consecutivos sin base de datos: interno 1, 2, 3...</summary>
    private sealed class ConsecutivosServiceStub : IConsecutivosService
    {
        private int _interno;

        public int GetAndIncrementConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecOCTransactional(Microsoft.Data.SqlClient.SqlConnection conn, Microsoft.Data.SqlClient.SqlTransaction transaction)
            => throw new NotImplementedException();

        public int BuscarUniqueCodeConsec() => throw new NotImplementedException();

        public int BuscarConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecProducto() => throw new NotImplementedException();

        public int GetAndIncrementConsecCliente() => throw new NotImplementedException();

        public int GetAndIncrementConsecProveedor() => ++_interno;

        public int GetAndIncrementConsecVendedor() => throw new NotImplementedException();

        public bool UpdateConsecOC(string consec) => throw new NotImplementedException();

        public bool UpdateUniqueCodeBD(string consec) => throw new NotImplementedException();
    }

    /// <summary>Servicio de exportación sin efectos: no crea archivos ni abre Excel.</summary>
    private sealed class ExportDataServiceStub : IExportDataService
    {
        public bool ExportToExcel<T>(List<T> data, string FileName) => true;

        public bool ExportToExcelProducts<T>(List<T> data, string FileName) => true;

        public bool ExportTxtFormatRollosCortados(DataRow[] rollos, bool solo_rc, string? fecha_produccion, string? fecha_registro, bool openNotePad) => true;

        public bool ExportTxtFormatMasterRePrintLabel(ProductMAP master, bool openNotePad) => true;
    }

    /// <summary>Visor de reportes sin efectos: cumple el contrato sin abrir ninguna ventana.</summary>
    private sealed class ReportsServiceStub : IReportsService
    {
        public void Reporte_Orden_Corte(string orden, Form form, string ReportName, string TitleReport) { }

        public void Reporte_Desperdicios(string orden, Form form, string ReportName, string TitleReport) { }

        public void Reporte_Orden_MatPrima(string orden, Form form, string ReportName, string TitleReport) { }

        public void ReporteConduce_conPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

        public void ReporteCondece_sinPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

        public void Reporte_PackingList(string conduce, Form form) { }

        public void Reporte_DetallePaleta(string conduce, Form form) { }

        public void Reporte_InventarioRollosCortados(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_InventarioMaster(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Productos(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Clientes(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Proveedores(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Vendedores(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Usuarios(Form form, string Report_Title, string Report_Name) { }
    }

    private static FrmProveedores CrearFormulario(DataTable? datos = null)
        => new(new ProveedorServiceStub(datos), new ConsecutivosServiceStub(), new ExportDataServiceStub(),
               new ReportsServiceStub());

    /// <summary>Dos proveedores de prueba: uno activo y otro anulado.</summary>
    private static DataTable ProveedoresDePrueba()
    {
        DataTable datos = new DataTable();
        datos.Columns.Add("Proveedor_Id", typeof(string));
        datos.Columns.Add("Proveedor_Name", typeof(string));
        datos.Columns.Add("phone", typeof(string));
        datos.Columns.Add("direccion", typeof(string));
        datos.Columns.Add("email", typeof(string));
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("P-001", "Proveedor Activo SA", "809-555-0001", "Calle Uno #1", "activo@prov.com", "activo");
        datos.Rows.Add("P-002", "Proveedor Anulado", "809-555-0002", "Calle Dos #2", "anulado@prov.com", "desactivado");
        return datos;
    }

    [Fact]
    public void Raiz_DivideElAnchoEnTreintaYSetentaPorCiento()
    {
        using FrmProveedores form = CrearFormulario();

        TableLayoutPanel root = (TableLayoutPanel)form.Controls.Find("tlpRoot", true).Single();
        root.ColumnCount.Should().Be(2);
        root.ColumnStyles[0].SizeType.Should().Be(SizeType.Percent);
        root.ColumnStyles[0].Width.Should().Be(30f);
        root.ColumnStyles[1].Width.Should().Be(70f);
    }

    [Fact]
    public void Titulos_DelFormularioYDelPanelSonProveedores()
    {
        using FrmProveedores form = CrearFormulario();

        form.Text.Should().Be("Proveedores");
        form.Controls.Find("lblTitulo", true).Single().Text.Should().Be("CATALOGO DE PROVEEDORES");
    }

    [Fact]
    public void PanelIzquierdo_TieneBuscadorYGridDeTresColumnas()
    {
        using FrmProveedores form = CrearFormulario();

        form.Controls.Find("txtBuscar", true).Should().ContainSingle();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProveedores", true).Single();
        grid.Columns.Count.Should().Be(3);
        grid.Columns[0].DataPropertyName.Should().Be("Proveedor_Id");
        grid.Columns[1].DataPropertyName.Should().Be("Proveedor_Name");
        grid.Columns[2].DataPropertyName.Should().Be("status");
        grid.ReadOnly.Should().BeTrue();
        grid.AllowUserToAddRows.Should().BeFalse();
        grid.AutoGenerateColumns.Should().BeFalse();
    }

    [Fact]
    public void PanelIzquierdo_TieneCuadroDeResumenAlPie()
    {
        using FrmProveedores form = CrearFormulario();

        Panel resumen = (Panel)form.Controls.Find("pnlResumen", true).Single();
        resumen.Dock.Should().Be(DockStyle.Bottom);
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 0 proveedores");
    }

    [Fact]
    public void PanelDerecho_TieneLosCamposDelProveedorEnLaPaginaDeDetalle()
    {
        using FrmProveedores form = CrearFormulario();

        form.Controls.Find("panelDer", true).Should().ContainSingle();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleProveedor", true).Single();
        detalle.Text.Should().Be("Detalle");
        form.Controls.Find("lblDetalleTitulo", true).Single().Text.Should().Be("DETALLE DEL PROVEEDOR");
        form.Controls.Find("txtValorId", true).Should().ContainSingle();
        form.Controls.Find("txtValorNombre", true).Should().ContainSingle();
        form.Controls.Find("txtValorTelefono", true).Should().ContainSingle();
        form.Controls.Find("txtValorDireccion", true).Should().ContainSingle();
        form.Controls.Find("txtValorEmail", true).Should().ContainSingle();
        form.Controls.Find("swEstado", true).Should().ContainSingle();
        form.Controls.Find("lblPlaceDetalle", true).Should().BeEmpty();
    }

    [Fact]
    public void Detalle_EnConsultaTodoEsSoloLectura()
    {
        using FrmProveedores form = CrearFormulario();

        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorId", true).Single()).ReadOnly.Should().BeTrue();
        Sunny.UI.UITextBox interno = (Sunny.UI.UITextBox)form.Controls.Find("txtCodigoInterno", true).Single();
        interno.ReadOnly.Should().BeTrue();
        interno.Text.Should().Be("—");
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorNombre", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorEmail", true).Single()).ReadOnly.Should().BeTrue();
        Sunny.UI.UITextBox entrega = (Sunny.UI.UITextBox)form.Controls.Find("txtValorDireccionEntrega", true).Single();
        entrega.ReadOnly.Should().BeTrue();
        entrega.Text.Should().Be("—");
        // El estado se gobierna por ReadOnly (igual que Productos), no por Enabled.
        Sunny.UI.UISwitch estado = (Sunny.UI.UISwitch)form.Controls.Find("swEstado", true).Single();
        estado.Enabled.Should().BeTrue();
        estado.ReadOnly.Should().BeTrue();
    }

    [Fact]
    public void Detalle_TieneComboCategoriaConNacionalEInternacional()
    {
        using FrmProveedores form = CrearFormulario();

        Sunny.UI.UIComboBox combo = (Sunny.UI.UIComboBox)form.Controls.Find("cboCategoria", true).Single();
        combo.Items.Count.Should().Be(2);
        combo.Items[0].ToString().Should().Be("Nacional");
        combo.Items[1].ToString().Should().Be("Internacional");
        combo.SelectedIndex.Should().Be(-1);
        combo.ReadOnly.Should().BeTrue();
        combo.Enabled.Should().BeFalse();
    }

    // Nota: el Nuevo con GUID + consecutivo 1, 2, 3... no se cubre aquí a
    // propósito: requiere clic con sesión global (SesionActual) y las clases de
    // test corren en paralelo — si otra clase limpia la sesión a mitad del clic,
    // salta un MessageBox real y cuelga la corrida. Se verifica manual.

    [Fact]
    public void GridSeleccion_PintaLaFilaDeVerde()
    {
        using FrmProveedores form = CrearFormulario();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProveedores", true).Single();
        grid.SelectionMode.Should().Be(DataGridViewSelectionMode.FullRowSelect);
        // EstilarSeleccionGrid hace la selección negra con letras blancas (igual que FrmClientes)
        grid.DefaultCellStyle.SelectionBackColor.Should().Be(Color.Black);
        grid.DefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
        grid.RowsDefaultCellStyle.SelectionBackColor.Should().Be(Color.Black);
        grid.RowsDefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
        grid.AlternatingRowsDefaultCellStyle.SelectionBackColor.Should().Be(Color.Black);
        grid.AlternatingRowsDefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
    }

    [Fact]
    public void SeConstruye_SoloConElServicio_SinDependenciasOcultas()
    {
        // El ctor no pide configuración ni resuelve cadena de conexión:
        // todo lo que toca la base vive detrás del IProveedorService.
        using FrmProveedores form = CrearFormulario();

        form.Should().NotBeNull();
        form.Controls.Find("tlpRoot", true).Should().ContainSingle();
    }

    [Fact]
    public async Task InitializeAsync_LlenaElGridConLoQueDevuelveElServicio()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresDePrueba());

        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProveedores", true).Single();
        grid.Columns.Count.Should().Be(3);
        grid.Rows.Count.Should().Be(2);
        grid.Rows[0].Cells[1].Value.Should().Be("Proveedor Activo SA");
        grid.Rows[0].Cells[2].Value.Should().Be("activo");
        grid.Rows[1].Cells[2].Value.Should().Be("desactivado");
    }

    [Fact]
    public async Task InitializeAsync_MuestraElTotalDeProveedoresEnElResumen()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresDePrueba());

        await form.InitializeAsync();

        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 2 proveedores");
    }

    [Fact]
    public async Task Buscar_FiltraElListadoPorNombre()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresDePrueba());
        await form.InitializeAsync();

        form.Controls.Find("txtBuscar", true).Single().Text = "Anulado";

        DataGridView grid = (DataGridView)form.Controls.Find("gridProveedores", true).Single();
        grid.Rows.Count.Should().Be(1);
        grid.Rows[0].Cells[1].Value.Should().Be("Proveedor Anulado");
    }

    [Fact]
    public async Task Buscar_ActualizaElResumenConLoQueSeFiltra()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresDePrueba());
        await form.InitializeAsync();

        form.Controls.Find("txtBuscar", true).Single().Text = "Anulado";

        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Mostrando 1 de 2 proveedores");
    }

    [Fact]
    public async Task SeleccionarFila_RefreshLosCamposDelDetalle()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProveedores", true).Single();
        grid.CurrentCell = grid.Rows[0].Cells[0];

        form.Controls.Find("txtValorId", true).Single().Text.Should().Be("P-001");
        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Proveedor Activo SA");
        form.Controls.Find("txtValorTelefono", true).Single().Text.Should().Be("809-555-0001");
        form.Controls.Find("txtValorEmail", true).Single().Text.Should().Be("activo@prov.com");
        ((Sunny.UI.UISwitch)form.Controls.Find("swEstado", true).Single()).Active.Should().BeTrue();

        // Cambiar de fila debe refrescar el detalle con el nuevo proveedor.
        grid.CurrentCell = grid.Rows[1].Cells[0];

        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Proveedor Anulado");
        ((Sunny.UI.UISwitch)form.Controls.Find("swEstado", true).Single()).Active.Should().BeFalse();
    }

    /// <summary>
    /// Datos con la mezcla real de categorías de la base: texto exacto, sinónimos viejos
    /// y filas sin categoría (en Provider casi todas están en NULL).
    /// </summary>
    private static DataTable ProveedoresConCategorias()
    {
        DataTable datos = new DataTable();
        datos.Columns.Add("Proveedor_Id", typeof(string));
        datos.Columns.Add("Proveedor_Name", typeof(string));
        datos.Columns.Add("phone", typeof(string));
        datos.Columns.Add("direccion", typeof(string));
        datos.Columns.Add("email", typeof(string));
        datos.Columns.Add("categoria", typeof(string));
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("P-001", "Nacional Exacto", "1", "", "", "Nacional", "activo");
        datos.Rows.Add("P-002", "Local Viejo", "2", "", "", "Local", "activo");
        datos.Rows.Add("P-003", "Venta Local", "3", "", "", "Venta local", "activo");
        datos.Rows.Add("P-004", "Int Exacto", "4", "", "", "Internacional", "activo");
        datos.Rows.Add("P-005", "Exterior Viejo", "5", "", "", "Exterior", "activo");
        datos.Rows.Add("P-006", "Zona Franca", "6", "", "", "Zona franca", "activo");
        datos.Rows.Add("P-007", "Sin Categoria", "7", "", "", DBNull.Value, "activo");
        return datos;
    }

    private static DataGridView GridProveedores(FrmProveedores form)
        => (DataGridView)form.Controls.Find("gridProveedores", true).Single();

    private static RadioButton Radio(FrmProveedores form, string nombre)
        => (RadioButton)form.Controls.Find(nombre, true).Single();

    private static List<string?> NombresVisibles(FrmProveedores form)
        => GridProveedores(form).Rows
            .Cast<DataGridViewRow>()
            .Select(r => r.Cells[1].Value as string)
            .ToList();

    [Fact]
    public void FiltroCategoria_TieneTresRadiosTodosNacionalEInternacional()
    {
        using FrmProveedores form = CrearFormulario();

        RadioButton todos = Radio(form, "rbCatTodos");
        todos.Text.Should().Be("Todos");
        Radio(form, "rbCatNacional").Text.Should().Be("Nacional");
        Radio(form, "rbCatInternacional").Text.Should().Be("Internacional");

        todos.Checked.Should().BeTrue();
        Radio(form, "rbCatNacional").Checked.Should().BeFalse();
        Radio(form, "rbCatInternacional").Checked.Should().BeFalse();
    }

    [Fact]
    public async Task FiltroCategoria_TodosMuestraLaListaCompleta()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatTodos").Checked = true;

        GridProveedores(form).Rows.Count.Should().Be(7);
    }

    [Fact]
    public async Task FiltroCategoria_NacionalIncluyeLosSinonimosLocales()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;

        GridProveedores(form).Rows.Count.Should().Be(3);
        NombresVisibles(form).Should().BeEquivalentTo(["Nacional Exacto", "Local Viejo", "Venta Local"]);
    }

    [Fact]
    public async Task FiltroCategoria_InternacionalIncluyeExteriorYZonaFranca()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatInternacional").Checked = true;

        GridProveedores(form).Rows.Count.Should().Be(3);
        NombresVisibles(form).Should().BeEquivalentTo(["Int Exacto", "Exterior Viejo", "Zona Franca"]);
    }

    [Fact]
    public async Task FiltroCategoria_QuienNoTieneCategoriaNoSaleEnNingunRadio()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;
        NombresVisibles(form).Should().NotContain("Sin Categoria");

        Radio(form, "rbCatInternacional").Checked = true;
        NombresVisibles(form).Should().NotContain("Sin Categoria");

        Radio(form, "rbCatTodos").Checked = true;
        NombresVisibles(form).Should().Contain("Sin Categoria");
    }

    [Fact]
    public async Task FiltroCategoria_VuelveATodosParaQuitarElFiltro()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;
        GridProveedores(form).Rows.Count.Should().Be(3);

        Radio(form, "rbCatTodos").Checked = true;
        GridProveedores(form).Rows.Count.Should().Be(7);
    }

    [Fact]
    public async Task FiltroCategoria_SeCombinaConElBuscadorPorNombre()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatInternacional").Checked = true;
        form.Controls.Find("txtBuscar", true).Single().Text = "Viejo";

        GridProveedores(form).Rows.Count.Should().Be(1);
        NombresVisibles(form).Should().BeEquivalentTo(["Exterior Viejo"]);
    }

    [Fact]
    public void FiltroCategoria_LosRadiosQuedanDebajoDelTextboxDeBuscar()
    {
        using FrmProveedores form = CrearFormulario();
        form.PerformLayout();

        Point inicioBuscador = PosicionRelativaA(form.Controls.Find("txtBuscar", true).Single(), form);

        foreach (string nombre in new[] { "rbCatTodos", "rbCatNacional", "rbCatInternacional" })
        {
            Point inicioRadio = PosicionRelativaA(form.Controls.Find(nombre, true).Single(), form);
            inicioRadio.Y.Should().BeGreaterThan(
                inicioBuscador.Y,
                because: $"el radio {nombre} debe ir debajo del buscador");
        }

        Point todos = PosicionRelativaA(form.Controls.Find("rbCatTodos", true).Single(), form);
        Point nacional = PosicionRelativaA(form.Controls.Find("rbCatNacional", true).Single(), form);
        Point internacional = PosicionRelativaA(form.Controls.Find("rbCatInternacional", true).Single(), form);

        nacional.Y.Should().Be(todos.Y);
        internacional.Y.Should().Be(todos.Y);
        nacional.X.Should().BeGreaterThan(todos.X);
        internacional.X.Should().BeGreaterThan(nacional.X);

        Panel panelRadios = (Panel)form.Controls.Find("pnlFiltroCategoria", true).Single();
        Panel panelContador = (Panel)form.Controls.Find("pnlResumen", true).Single();

        // Los tres cuelgan de panelIzq como hermanos, cada uno en su franja, y el contador
        // no comparte panel con los radios.
        panelRadios.Parent.Should().BeSameAs(panelContador.Parent);
        panelContador.Controls.Find("lblResumen", true).Should().ContainSingle();
        panelContador.Controls.Find("pnlFiltroCategoria", true).Should().BeEmpty();
    }

    [Fact]
    public void PanelBuscador_ElOrdenVerticalEsTituloBuscadorRadiosGridYContadorAlPie()
    {
        using FrmProveedores form = CrearFormulario();
        form.PerformLayout();

        int titulo = PosicionRelativaA(form.Controls.Find("lblTitulo", true).Single(), form).Y;
        int buscador = PosicionRelativaA(form.Controls.Find("txtBuscar", true).Single(), form).Y;
        int radios = PosicionRelativaA(form.Controls.Find("rbCatTodos", true).Single(), form).Y;
        int grid = PosicionRelativaA(form.Controls.Find("gridProveedores", true).Single(), form).Y;
        int contador = PosicionRelativaA(form.Controls.Find("lblResumen", true).Single(), form).Y;

        titulo.Should().BeLessThan(buscador, because: "el título va arriba del buscador");
        buscador.Should().BeLessThan(radios, because: "los radios van debajo del buscador");
        radios.Should().BeLessThan(grid, because: "el grid va debajo de los radios");

        // El contador es una barra al pie, como la de Productos: por debajo del grid.
        contador.Should().BeGreaterThan(grid, because: "el contador va al pie, debajo del grid");

        Panel panelContador = (Panel)form.Controls.Find("pnlResumen", true).Single();
        panelContador.Dock.Should().Be(DockStyle.Bottom);
        panelContador.BackColor.Should().Be(Color.FromArgb(110, 190, 40));

        Label etiqueta = (Label)form.Controls.Find("lblResumen", true).Single();
        etiqueta.ForeColor.Should().Be(Color.White);
        etiqueta.Font.Bold.Should().BeTrue(because: "el total se lee de un vistazo");

        Panel panelRadios = (Panel)form.Controls.Find("pnlFiltroCategoria", true).Single();
        panelRadios.Parent.Should().BeSameAs(panelContador.Parent);
        panelContador.Controls.Find("pnlFiltroCategoria", true).Should().BeEmpty();
    }

    /// <summary>
    /// Posición de un control dentro de otro cualquiera, sumando la cadena de contenedores:
    /// Top/Left son relativos al padre, y los radios cuelgan de panelIzq mientras que el
    /// buscador cuelga de pnlBuscador.
    /// </summary>
    private static Point PosicionRelativaA(Control control, Control ancla)
    {
        int x = 0;
        int y = 0;
        for (Control? actual = control; actual is not null && actual != ancla; actual = actual.Parent)
        {
            x += actual.Left;
            y += actual.Top;
        }

        return new Point(x, y);
    }

    [Fact]
    public async Task FiltroCategoria_ActualizaElResumenAunqueNoHayTextoBuscado()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;

        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Mostrando 3 de 7 proveedores");
    }

    [Fact]
    public async Task Grid_ElProveedorAnuladoSePintaEnRojo()
    {
        using FrmProveedores form = CrearFormulario(ProveedoresDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProveedores", true).Single();
        grid.Rows[1].Cells[0].Style.ForeColor.Should().Be(FrmProveedores.ColorTextoAnulado);
        grid.Rows[1].Cells[0].Style.BackColor.Should().Be(FrmProveedores.ColorFondoAnulado);
        grid.Rows[0].Cells[0].Style.ForeColor.Should().NotBe(FrmProveedores.ColorTextoAnulado);
        grid.Rows[0].Cells[0].Style.BackColor.Should().NotBe(FrmProveedores.ColorFondoAnulado);
    }

    [Fact]
    public void Barra_TieneElBotonDeReporteConSuIcono()
    {
        using FrmProveedores form = CrearFormulario();
        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        ToolStripButton boton = (ToolStripButton)barra.Items["btnReporteProveedor"]!;

        boton.Should().NotBeNull("el boton de reporte es la entrada al catalogo en el visor");
        boton.Text.Should().Be("Reporte");
        boton.Image.Should().NotBeNull("el boton de reporte necesita un icono");

        // Tercera posicion, detras de Importar y delante de Guardar (ver FrmProductos).
        barra.Items.IndexOf(boton).Should().Be(3);
        barra.Items.IndexOf(barra.Items["btnGuardarProveedor"]!).Should().BeGreaterThan(barra.Items.IndexOf(boton));
    }
}
