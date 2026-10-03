using System.Data;
using FluentAssertions;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.ClienteService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño 30/70 de FrmClientes: buscador, cuadro de
/// resumen (total/filtrado), grid y página de detalle refrescada por selección,
/// con los valores como textbox editables (solo id y estado de solo lectura).
/// No toca base de datos: el servicio va con un stub.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmClientesLayoutTests
{
    /// <summary>Servicio mínimo para instanciar el formulario sin dependencias reales.</summary>
    private sealed class ClienteServiceStub : IClienteService
    {
        private readonly DataTable _datos;

        public ClienteServiceStub(DataTable? datos = null)
            => _datos = datos ?? new DataTable();

        public Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_datos);

        public Task<Result<bool>> AddValidatedAsync(Cliente cliente, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> UpdateValidatedAsync(Cliente cliente, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(false));

        public Result ValidateCliente(Cliente cliente)
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

        public int GetAndIncrementConsecCliente() => ++_interno;

        public int GetAndIncrementConsecProveedor() => throw new NotImplementedException();

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

    private static FrmClientes CrearFormulario(DataTable? datos = null)
        => new(new ClienteServiceStub(datos), new ConsecutivosServiceStub(), new ExportDataServiceStub(),
               new ReportsServiceStub());

    /// <summary>Dos clientes de prueba: uno activo y otro desactivado.</summary>
    private static DataTable ClientesDePrueba()
    {
        DataTable datos = new DataTable();
        datos.Columns.Add("customer_id", typeof(string));
        datos.Columns.Add("customer_name", typeof(string));
        datos.Columns.Add("phone", typeof(string));
        datos.Columns.Add("direccion_facturacion", typeof(string));
        datos.Columns.Add("customer_email", typeof(string));
        datos.Columns.Add("unity1", typeof(bool));
        datos.Columns.Add("unity2", typeof(bool));
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("C-001", "Cliente Activo SA", "809-555-0001", "Calle Fact 1", "activo@cliente.com", true, false, "activo");
        datos.Rows.Add("C-002", "Cliente Borrado", "809-555-0002", "Calle Fact 2", "borrado@cliente.com", false, true, "desactivado");
        return datos;
    }

    [Fact]
    public void Raiz_DivideElAnchoEnTreintaYSetentaPorCiento()
    {
        using FrmClientes form = CrearFormulario();

        TableLayoutPanel root = (TableLayoutPanel)form.Controls.Find("tlpRoot", true).Single();
        root.ColumnCount.Should().Be(2);
        root.ColumnStyles[0].SizeType.Should().Be(SizeType.Percent);
        root.ColumnStyles[0].Width.Should().Be(30f);
        root.ColumnStyles[1].Width.Should().Be(70f);
    }

    [Fact]
    public void Titulos_DelFormularioYDelPanelSonClientes()
    {
        using FrmClientes form = CrearFormulario();

        form.Text.Should().Be("Clientes");
        form.Controls.Find("lblTitulo", true).Single().Text.Should().Be("CATALOGO DE CLIENTES");
    }

    [Fact]
    public void PanelIzquierdo_TieneBuscadorYGridDeTresColumnas()
    {
        using FrmClientes form = CrearFormulario();

        form.Controls.Find("txtBuscar", true).Should().ContainSingle();

        DataGridView grid = (DataGridView)form.Controls.Find("gridClientes", true).Single();
        grid.Columns.Count.Should().Be(3);
        grid.Columns[0].DataPropertyName.Should().Be("customer_id");
        grid.Columns[1].DataPropertyName.Should().Be("customer_name");
        grid.Columns[2].DataPropertyName.Should().Be("status");
        grid.ReadOnly.Should().BeTrue();
        grid.AllowUserToAddRows.Should().BeFalse();
    }

    [Fact]
    public void PanelIzquierdo_TieneCuadroDeResumenAlPie()
    {
        using FrmClientes form = CrearFormulario();

        Panel resumen = (Panel)form.Controls.Find("pnlResumen", true).Single();
        resumen.Dock.Should().Be(DockStyle.Bottom);
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 0 clientes");
    }

    [Fact]
    public void PanelDerecho_TieneLosCamposDelClienteEnLaPaginaDeDetalle()
    {
        using FrmClientes form = CrearFormulario();

        form.Controls.Find("panelDer", true).Should().ContainSingle();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleCliente", true).Single();
        detalle.Text.Should().Be("Detalle");
        detalle.Controls.Find("lblDetalleTitulo", true).Single().Text.Should().Be("DETALLE DEL CLIENTE");
        detalle.Controls.Find("txtValorId", true).Should().ContainSingle();
        detalle.Controls.Find("txtValorNombre", true).Should().ContainSingle();
        detalle.Controls.Find("txtValorTelefono", true).Should().ContainSingle();
        detalle.Controls.Find("txtValorDireccion", true).Should().ContainSingle();
        detalle.Controls.Find("txtValorEmail", true).Should().ContainSingle();
        detalle.Controls.Find("swEstado", true).Should().ContainSingle();

        // La unidad master se quitó del detalle: ni las cajas ni sus etiquetas deben existir.
        detalle.Controls.Find("txtValorUnity1", true).Should().BeEmpty();
        detalle.Controls.Find("txtValorUnity2", true).Should().BeEmpty();
        detalle.Controls.Find("lblCapUnity1", true).Should().BeEmpty();
        detalle.Controls.Find("lblCapUnity2", true).Should().BeEmpty();
    }

    [Fact]
    public void Detalle_EnConsultaTodoEsSoloLectura()
    {
        using FrmClientes form = CrearFormulario();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleCliente", true).Single();
        ((Sunny.UI.UITextBox)detalle.Controls.Find("txtValorId", true).Single()).ReadOnly.Should().BeTrue();
        Sunny.UI.UITextBox interno = (Sunny.UI.UITextBox)detalle.Controls.Find("txtCodigoInterno", true).Single();
        interno.ReadOnly.Should().BeTrue();
        interno.Text.Should().Be("—");
        ((Sunny.UI.UITextBox)detalle.Controls.Find("txtValorNombre", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)detalle.Controls.Find("txtValorEmail", true).Single()).ReadOnly.Should().BeTrue();
        Sunny.UI.UITextBox entrega = (Sunny.UI.UITextBox)detalle.Controls.Find("txtValorDireccionEntrega", true).Single();
        entrega.ReadOnly.Should().BeTrue();
        entrega.Text.Should().Be("—");
        // El estado se gobierna por ReadOnly (igual que Productos), no por Enabled.
        Sunny.UI.UISwitch estado = (Sunny.UI.UISwitch)detalle.Controls.Find("swEstado", true).Single();
        estado.Enabled.Should().BeTrue();
        estado.ReadOnly.Should().BeTrue();
    }

    [Fact]
    public void Detalle_TieneComboCategoriaConNacionalEInternacional()
    {
        using FrmClientes form = CrearFormulario();

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
        using FrmClientes form = CrearFormulario();

        DataGridView grid = (DataGridView)form.Controls.Find("gridClientes", true).Single();
        grid.SelectionMode.Should().Be(DataGridViewSelectionMode.FullRowSelect);
        // EstilarSeleccionGrid hace la selección negra con letras blancas
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
        // El ctor ya no pide configuración ni resuelve cadena de conexión:
        // todo lo que toca la base vive detrás del IClienteService.
        using FrmClientes form = CrearFormulario();

        form.Should().NotBeNull();
        form.Controls.Find("tlpRoot", true).Should().ContainSingle();
    }

    [Fact]
    public async Task InitializeAsync_LlenaElGridConLoQueDevuelveElServicio()
    {
        using FrmClientes form = CrearFormulario(ClientesDePrueba());

        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridClientes", true).Single();
        // Post-bind: con AutoGenerateColumns activo el esquema del DataTable reordena
        // las columnas y Cells[2] deja de ser status — por eso se fija aquí también.
        grid.Columns.Count.Should().Be(3);
        grid.Rows.Count.Should().Be(2);
        grid.Rows[0].Cells[1].Value.Should().Be("Cliente Activo SA");
        grid.Rows[0].Cells[2].Value.Should().Be("activo");
        grid.Rows[1].Cells[2].Value.Should().Be("desactivado");
    }

    [Fact]
    public async Task InitializeAsync_MuestraElTotalDeClientesEnElResumen()
    {
        using FrmClientes form = CrearFormulario(ClientesDePrueba());

        await form.InitializeAsync();

        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 2 clientes");
    }

    [Fact]
    public async Task Buscar_FiltraElListadoPorNombre()
    {
        using FrmClientes form = CrearFormulario(ClientesDePrueba());
        await form.InitializeAsync();

        form.Controls.Find("txtBuscar", true).Single().Text = "Borrado";

        DataGridView grid = (DataGridView)form.Controls.Find("gridClientes", true).Single();
        grid.Rows.Count.Should().Be(1);
        grid.Rows[0].Cells[1].Value.Should().Be("Cliente Borrado");
    }

    [Fact]
    public async Task Buscar_ActualizaElResumenConLoQueSeFiltra()
    {
        using FrmClientes form = CrearFormulario(ClientesDePrueba());
        await form.InitializeAsync();

        form.Controls.Find("txtBuscar", true).Single().Text = "Borrado";

        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Mostrando 1 de 2 clientes");
    }

    [Fact]
    public async Task SeleccionarFila_RefreshLosCamposDelDetalle()
    {
        using FrmClientes form = CrearFormulario(ClientesDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridClientes", true).Single();
        grid.CurrentCell = grid.Rows[0].Cells[0];

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleCliente", true).Single();
        detalle.Controls.Find("txtValorId", true).Single().Text.Should().Be("C-001");
        detalle.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Cliente Activo SA");
        detalle.Controls.Find("txtValorTelefono", true).Single().Text.Should().Be("809-555-0001");
        detalle.Controls.Find("txtValorDireccion", true).Single().Text.Should().Be("Calle Fact 1");
        detalle.Controls.Find("txtValorEmail", true).Single().Text.Should().Be("activo@cliente.com");
        ((Sunny.UI.UISwitch)detalle.Controls.Find("swEstado", true).Single()).Active.Should().BeTrue();

        // Cambiar de fila debe refrescar el detalle con el nuevo cliente.
        grid.CurrentCell = grid.Rows[1].Cells[0];

        detalle.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Cliente Borrado");
        detalle.Controls.Find("txtValorTelefono", true).Single().Text.Should().Be("809-555-0002");
        ((Sunny.UI.UISwitch)detalle.Controls.Find("swEstado", true).Single()).Active.Should().BeFalse();
    }

    /// <summary>
    /// Datos con la mezcla real de categorías que hay en la base: textos exactos,
    /// sinónimos viejos y filas sin categoría. "Local"/"Venta local" son
    /// equivalentes a Nacional; "Exterior"/"Zona franca"/"Zonal Franca" son
    /// equivalentes a Internacional.
    /// </summary>
    private static DataTable ClientesConCategorias()
    {
        DataTable datos = new DataTable();
        datos.Columns.Add("customer_id", typeof(string));
        datos.Columns.Add("customer_name", typeof(string));
        datos.Columns.Add("phone", typeof(string));
        datos.Columns.Add("direccion_facturacion", typeof(string));
        datos.Columns.Add("customer_email", typeof(string));
        datos.Columns.Add("customer_category", typeof(string));
        datos.Columns.Add("unity1", typeof(bool));
        datos.Columns.Add("unity2", typeof(bool));
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("C-001", "Nacional Exacto", "1", "", "", "Nacional", false, false, "activo");
        datos.Rows.Add("C-002", "Local Viejo", "2", "", "", "Local", false, false, "activo");
        datos.Rows.Add("C-003", "Venta Local", "3", "", "", "Venta local", false, false, "activo");
        datos.Rows.Add("C-004", "Int Exacto", "4", "", "", "Internacional", false, false, "activo");
        datos.Rows.Add("C-005", "Exterior Viejo", "5", "", "", "Exterior", false, false, "activo");
        datos.Rows.Add("C-006", "Zona Franca", "6", "", "", "Zona franca", false, false, "activo");
        datos.Rows.Add("C-007", "Zonal Franca", "7", "", "", "Zonal Franca", false, false, "activo");
        datos.Rows.Add("C-008", "Sin Categoria", "8", "", "", DBNull.Value, false, false, "activo");
        return datos;
    }

    private static DataGridView GridClientes(FrmClientes form)
        => (DataGridView)form.Controls.Find("gridClientes", true).Single();

    private static RadioButton Radio(FrmClientes form, string nombre)
        => (RadioButton)form.Controls.Find(nombre, true).Single();

    /// <summary>Nombres de los clientes que el grid está mostrando ahora mismo.</summary>
    private static List<string?> NombresVisibles(FrmClientes form)
        => GridClientes(form).Rows
            .Cast<DataGridViewRow>()
            .Select(r => r.Cells[1].Value as string)
            .ToList();

    [Fact]
    public void FiltroCategoria_TieneTresRadiosTodosNacionalEInternacional()
    {
        using FrmClientes form = CrearFormulario();

        form.Controls.Find("rbCatTodos", true).Should().ContainSingle();
        form.Controls.Find("rbCatNacional", true).Should().ContainSingle();
        form.Controls.Find("rbCatInternacional", true).Should().ContainSingle();

        RadioButton todos = Radio(form, "rbCatTodos");
        RadioButton nacional = Radio(form, "rbCatNacional");
        RadioButton internacional = Radio(form, "rbCatInternacional");
        todos.Text.Should().Be("Todos");
        nacional.Text.Should().Be("Nacional");
        internacional.Text.Should().Be("Internacional");

        // "Todos" arranca marcado: la lista completa es el estado inicial.
        todos.Checked.Should().BeTrue();
        nacional.Checked.Should().BeFalse();
        internacional.Checked.Should().BeFalse();
    }

    [Fact]
    public async Task FiltroCategoria_TodosMuestraLaListaCompleta()
    {
        using FrmClientes form = CrearFormulario(ClientesConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatTodos").Checked = true;

        GridClientes(form).Rows.Count.Should().Be(8);
    }

    [Fact]
    public async Task FiltroCategoria_NacionalIncluyeLosSinonimosLocales()
    {
        using FrmClientes form = CrearFormulario(ClientesConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;

        // Nacional exacto + Local + Venta local. No entra "Sin Categoria".
        GridClientes(form).Rows.Count.Should().Be(3);
        NombresVisibles(form).Should().BeEquivalentTo(["Nacional Exacto", "Local Viejo", "Venta Local"]);
    }

    [Fact]
    public async Task FiltroCategoria_InternacionalIncluyeExteriorYZonaFranca()
    {
        using FrmClientes form = CrearFormulario(ClientesConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatInternacional").Checked = true;

        // Internacional exacto + Exterior + Zona franca + Zonal Franca.
        GridClientes(form).Rows.Count.Should().Be(4);
    }

    [Fact]
    public async Task FiltroCategoria_QuienNoTieneCategoriaNoSaleEnNingunRadio()
    {
        using FrmClientes form = CrearFormulario(ClientesConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;
        NombresVisibles(form).Should().NotContain("Sin Categoria");

        Radio(form, "rbCatInternacional").Checked = true;
        NombresVisibles(form).Should().NotContain("Sin Categoria");

        // Solo en Todos.
        Radio(form, "rbCatTodos").Checked = true;
        NombresVisibles(form).Should().Contain("Sin Categoria");
    }

    [Fact]
    public async Task FiltroCategoria_VuelveATodosParaQuitarElFiltro()
    {
        using FrmClientes form = CrearFormulario(ClientesConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;
        GridClientes(form).Rows.Count.Should().Be(3);

        Radio(form, "rbCatTodos").Checked = true;
        GridClientes(form).Rows.Count.Should().Be(8);
    }

    [Fact]
    public async Task FiltroCategoria_SeCombinaConElBuscadorPorNombre()
    {
        using FrmClientes form = CrearFormulario(ClientesConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatInternacional").Checked = true;
        form.Controls.Find("txtBuscar", true).Single().Text = "Franca";

        // De los 4 internacionales solo 2 llevan "Franca" en el nombre.
        GridClientes(form).Rows.Count.Should().Be(2);
    }

    [Fact]
    public async Task FiltroCategoria_ActualizaElResumenAunqueNoHayTextoBuscado()
    {
        using FrmClientes form = CrearFormulario(ClientesConCategorias());
        await form.InitializeAsync();

        Radio(form, "rbCatNacional").Checked = true;

        // Sin escribir nada en el buscador, el resumen ya refleja el filtro.
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Mostrando 3 de 8 clientes");
    }

    [Fact]
    public void PanelBuscador_ElContadorTieneSuPropioPanelAbajoYNoSeRecorta()
    {
        using FrmClientes form = CrearFormulario();
        form.PerformLayout();

        Panel panelContador = (Panel)form.Controls.Find("pnlResumen", true).Single();
        Panel panelRadios = (Panel)form.Controls.Find("pnlFiltroCategoria", true).Single();
        Label contador = (Label)form.Controls.Find("lblResumen", true).Single();

        // El contador vive solo en su panel: los radios ya no lo comparten, así que
        // no hay dos cosas peleando por la misma franja vertical.
        contador.Parent.Should().BeSameAs(panelContador);
        panelContador.Controls.Find("pnlFiltroCategoria", true).Should().BeEmpty();
        panelRadios.Controls.Find("lblResumen", true).Should().BeEmpty();

        // Sigue siendo visible: con tamaño y sin salirse del panel.
        contador.Width.Should().BeGreaterThan(0);
        contador.Height.Should().BeGreaterThan(0);
        (contador.Top + contador.Height).Should().BeLessThanOrEqualTo(panelContador.ClientSize.Height);
    }

    [Fact]
    public void PanelBuscador_ElOrdenVerticalEsTituloBuscadorRadiosGridYContadorAlPie()
    {
        using FrmClientes form = CrearFormulario();
        form.PerformLayout();

        int titulo = PosicionRelativaA(form.Controls.Find("lblTitulo", true).Single(), form).Y;
        int buscador = PosicionRelativaA(form.Controls.Find("txtBuscar", true).Single(), form).Y;
        int radios = PosicionRelativaA(form.Controls.Find("rbCatTodos", true).Single(), form).Y;
        int grid = PosicionRelativaA(form.Controls.Find("gridClientes", true).Single(), form).Y;
        int contador = PosicionRelativaA(form.Controls.Find("lblResumen", true).Single(), form).Y;

        titulo.Should().BeLessThan(buscador, because: "el título va arriba del buscador");
        buscador.Should().BeLessThan(radios, because: "los radios van debajo del buscador");
        radios.Should().BeLessThan(grid, because: "el grid va debajo de los radios");

        // El contador es una barra al pie, como la de Productos: por debajo del grid.
        contador.Should().BeGreaterThan(grid, because: "el contador va al pie, debajo del grid");

        Panel panelContador = (Panel)form.Controls.Find("pnlResumen", true).Single();
        panelContador.Dock.Should().Be(DockStyle.Bottom);
    }

    [Fact]
    public void PanelBuscador_ElContadorSePintaComoLaBarraVerdeDeProductos()
    {
        using FrmClientes form = CrearFormulario();

        Panel panelContador = (Panel)form.Controls.Find("pnlResumen", true).Single();
        Label contador = (Label)form.Controls.Find("lblResumen", true).Single();

        // Mismo contraste que Productos: fondo verde y texto blanco.
        panelContador.BackColor.Should().Be(Color.FromArgb(110, 190, 40));
        contador.ForeColor.Should().Be(Color.White);
        contador.Font.Bold.Should().BeTrue(because: "el total se lee de un vistazo");
        (contador.Top + contador.Height).Should().BeLessThanOrEqualTo(panelContador.ClientSize.Height);
    }

    /// <summary>
    /// Posición de un control dentro de otro cualquiera, sumando la cadena de contenedores.
    /// Necesaria porque Top/Left de cada control son relativos a su padre, y aquí los radios
    /// viven colgados de pnlResumen mientras que el buscador cuelga de pnlBuscador: comparar
    /// sus Top en crudo compararía dos sistemas de coordenadas distintos.
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
    public void FiltroCategoria_LosRadiosQuedanDebajoDelTextboxDeBuscar()
    {
        using FrmClientes form = CrearFormulario();
        form.PerformLayout();

        Control buscador = form.Controls.Find("txtBuscar", true).Single();
        Point inicioBuscador = PosicionRelativaA(buscador, form);

        // Los tres radios deben quedar por DEBAJO del buscador: su Y es mayor que la del buscador.
        foreach (string nombre in new[] { "rbCatTodos", "rbCatNacional", "rbCatInternacional" })
        {
            Point inicioRadio = PosicionRelativaA(form.Controls.Find(nombre, true).Single(), form);
            inicioRadio.Y.Should().BeGreaterThan(
                inicioBuscador.Y,
                because: $"el radio {nombre} debe ir debajo del buscador");
        }

        // Los tres caben en una sola fila horizontal, de izquierda a derecha.
        Point todos = PosicionRelativaA(form.Controls.Find("rbCatTodos", true).Single(), form);
        Point nacional = PosicionRelativaA(form.Controls.Find("rbCatNacional", true).Single(), form);
        Point internacional = PosicionRelativaA(form.Controls.Find("rbCatInternacional", true).Single(), form);

        nacional.Y.Should().Be(todos.Y);
        internacional.Y.Should().Be(todos.Y);
        nacional.X.Should().BeGreaterThan(todos.X);
        internacional.X.Should().BeGreaterThan(nacional.X);

        // Y cuelgan del panel que ya estaba bajo el buscador, sin tapar el texto de resumen.
        Panel panelRadios = (Panel)form.Controls.Find("pnlFiltroCategoria", true).Single();
        Panel panelContador = (Panel)form.Controls.Find("pnlResumen", true).Single();

        // Los tres cuelgan de panelIzq como hermanos, cada uno en su franja.
        panelRadios.Parent.Should().BeSameAs(panelContador.Parent);
        panelContador.Controls.Find("lblResumen", true).Should().ContainSingle();
    }

    /// <summary>Un cliente activo y otro desactivado, para el pintado por estado.</summary>
    private static DataTable ClientesParaColorear()
    {
        DataTable datos = new DataTable();
        datos.Columns.Add("customer_id", typeof(string));
        datos.Columns.Add("customer_name", typeof(string));
        datos.Columns.Add("phone", typeof(string));
        datos.Columns.Add("direccion_facturacion", typeof(string));
        datos.Columns.Add("customer_email", typeof(string));
        datos.Columns.Add("customer_category", typeof(string));
        datos.Columns.Add("unity1", typeof(bool));
        datos.Columns.Add("unity2", typeof(bool));
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("C-001", "Cliente Activo SA", "1", "", "", "Nacional", false, false, "activo");
        datos.Rows.Add("C-002", "Cliente Anulado", "2", "", "", "Nacional", false, false, "desactivado");
        return datos;
    }

    [Fact]
    public async Task Grid_ElClienteDesactivadoSePintaEnRojo()
    {
        using FrmClientes form = CrearFormulario(ClientesParaColorear());
        await form.InitializeAsync();

        DataGridViewRow anulado = GridClientes(form).Rows[1];
        anulado.Cells[0].Style.ForeColor.Should().Be(FrmClientes.ColorTextoAnulado);
        anulado.Cells[0].Style.BackColor.Should().Be(FrmClientes.ColorFondoAnulado);
    }

    [Fact]
    public async Task Grid_ElFondoAnuladoEsRojoYElTextoMasClaro()
    {
        using FrmClientes form = CrearFormulario(ClientesParaColorear());
        await form.InitializeAsync();

        Color fondo = FrmClientes.ColorFondoAnulado;
        Color texto = FrmClientes.ColorTextoAnulado;

        // Fondo rojo de verdad, no un rosa lavado: domina el rojo y el verde/azul quedan bajos.
        fondo.Should().Be(Color.Firebrick);
        fondo.G.Should().BeLessThan(fondo.R);
        fondo.B.Should().BeLessThan(fondo.R);

        // La letra va MÁS CLARA que el fondo para que se vea encima del rojo.
        ((int)texto.R + texto.G + texto.B).Should().BeGreaterThan(fondo.R + fondo.G + fondo.B);
        texto.Should().Be(Color.FromArgb(255, 214, 214));
        texto.Should().NotBe(Color.White, "se distingue del texto normal de una fila vigente");
    }

    [Fact]
    public async Task Grid_ElClienteActivoNoSePintaEnRojo()
    {
        using FrmClientes form = CrearFormulario(ClientesParaColorear());
        await form.InitializeAsync();

        DataGridViewRow activo = GridClientes(form).Rows[0];
        activo.Cells[0].Style.ForeColor.Should().NotBe(FrmClientes.ColorTextoAnulado);
        activo.Cells[0].Style.BackColor.Should().NotBe(FrmClientes.ColorFondoAnulado);
    }

    [Fact]
    public async Task Grid_LaFilaSeleccionadaSigueNegraConLetrasBlancas()
    {
        using FrmClientes form = CrearFormulario(ClientesParaColorear());
        await form.InitializeAsync();

        DataGridView grid = GridClientes(form);
        grid.CurrentCell = grid.Rows[1].Cells[0];

        // Aunque esté anulada, seleccionada gana: negro con blanco, no rojo.
        grid.Rows[1].Selected.Should().BeTrue();
        grid.DefaultCellStyle.SelectionBackColor.Should().Be(Color.Black);
        grid.DefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
    }

    [Fact]
    public async Task Grid_AlDeseleccionarElAnuladoVuelveARojo()
    {
        using FrmClientes form = CrearFormulario(ClientesParaColorear());
        await form.InitializeAsync();

        DataGridView grid = GridClientes(form);
        grid.CurrentCell = grid.Rows[0].Cells[0];
        grid.Rows[1].Cells[0].Style.ForeColor.Should().Be(FrmClientes.ColorTextoAnulado);
        grid.Rows[1].Cells[0].Style.BackColor.Should().Be(FrmClientes.ColorFondoAnulado);
    }

    [Fact]
    public void Barra_TieneElBotonDeReporteConSuIcono()
    {
        using FrmClientes form = CrearFormulario();
        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        ToolStripButton boton = (ToolStripButton)barra.Items["btnReporteCliente"]!;

        boton.Should().NotBeNull("el boton de reporte es la entrada al catalogo en el visor");
        boton.Text.Should().Be("Reporte");
        boton.Image.Should().NotBeNull("el boton de reporte necesita un icono");

        // Va en tercera posicion, justo detras de Importar: si se coloca despues de Guardar,
        // en escritura desaparece con el resto y no se distingue el orden de la barra.
        barra.Items.IndexOf(boton).Should().Be(3);
        barra.Items.IndexOf(barra.Items["btnGuardarCliente"]!).Should().BeGreaterThan(barra.Items.IndexOf(boton));
    }
}
