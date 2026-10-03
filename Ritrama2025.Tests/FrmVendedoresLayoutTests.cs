using System.Data;
using System.Threading.Tasks;
using FluentAssertions;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.VendedorService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño 30/70 de FrmVendedores: buscador, cuadro de
/// resumen (total/filtrado), grid de tres columnas y página de detalle refrescada
/// por selección, con los valores como textbox editables (solo id y estado de
/// solo lectura). No toca base de datos: el servicio va con un stub.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmVendedoresLayoutTests
{
    /// <summary>Servicio mínimo para instanciar el formulario sin dependencias reales.</summary>
    private sealed class VendedorServiceStub : IVendedorService
    {
        private readonly DataTable _datos;

        public VendedorServiceStub(DataTable? datos = null)
            => _datos = datos ?? new DataTable();

        public Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(_datos);

        public Task<Result<bool>> AddValidatedAsync(Vendedor vendedor, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> UpdateValidatedAsync(Vendedor vendedor, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(false));

        public Result ValidateVendedor(Vendedor vendedor)
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

        public int GetAndIncrementConsecProveedor() => throw new NotImplementedException();

        public int GetAndIncrementConsecVendedor() => ++_interno;

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

    /// <summary>
    /// Formulario con los avisos capturados: un MessageBox de verdad dentro de una prueba lo
    /// dejaría colgado a la espera de que alguien pulse un botón.
    /// </summary>
    private sealed class FrmVendedoresSinDialogos(
        IVendedorService vendedores, IConsecutivosService consecutivos,
        IExportDataService exporta, IReportsService reportes)
        : FrmVendedores(vendedores, consecutivos, exporta, reportes)
    {
        public List<string> Avisos { get; } = [];

        protected override void MostrarAviso(string mensaje) => Avisos.Add(mensaje);
    }

    private static FrmVendedoresSinDialogos CrearFormulario(DataTable? datos = null)
        => new(new VendedorServiceStub(datos), new ConsecutivosServiceStub(), new ExportDataServiceStub(),
               new ReportsServiceStub());

    /// <summary>
    /// Posición de un control dentro de otro cualquiera, sumando la cadena de contenedores:
    /// Top/Left son relativos al padre, y no todos los controles cuelgan del mismo.
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
    public void PanelBuscador_ElContadorEsUnaBarraVerdeAlPieComoEnProductos()
    {
        using FrmVendedores form = CrearFormulario();
        form.PerformLayout();

        int titulo = PosicionRelativaA(form.Controls.Find("lblTitulo", true).Single(), form).Y;
        int buscador = PosicionRelativaA(form.Controls.Find("txtBuscar", true).Single(), form).Y;
        int grid = PosicionRelativaA(form.Controls.Find("gridVendedores", true).Single(), form).Y;
        int contador = PosicionRelativaA(form.Controls.Find("lblResumen", true).Single(), form).Y;

        titulo.Should().BeLessThan(buscador, because: "el título va arriba del buscador");
        buscador.Should().BeLessThan(grid, because: "el grid va debajo del buscador");
        contador.Should().BeGreaterThan(grid, because: "el contador va al pie, debajo del grid");

        Panel panelContador = (Panel)form.Controls.Find("pnlResumen", true).Single();
        panelContador.Dock.Should().Be(DockStyle.Bottom);
        panelContador.BackColor.Should().Be(Color.FromArgb(110, 190, 40));

        Label etiqueta = (Label)form.Controls.Find("lblResumen", true).Single();
        etiqueta.ForeColor.Should().Be(Color.White);
        etiqueta.Font.Bold.Should().BeTrue(because: "el total se lee de un vistazo");
    }

    [Fact]
    public void FiltroCategoria_NoHayRadiosDeCategoriaEnVendedores()
    {
        using FrmVendedores form = CrearFormulario();

        // Vendedores no tiene columna de categoría: no se le pone filtro de categoría.
        form.Controls.Find("rbCatTodos", true).Should().BeEmpty();
        form.Controls.Find("rbCatNacional", true).Should().BeEmpty();
        form.Controls.Find("rbCatInternacional", true).Should().BeEmpty();
    }

    /// <summary>Dos vendedores de prueba: uno activo y otro anulado.</summary>
    private static DataTable VendedoresDePrueba()
    {
        DataTable datos = new DataTable();
        datos.Columns.Add("vendor_id", typeof(string));
        datos.Columns.Add("vendor_name", typeof(string));
        datos.Columns.Add("correo", typeof(string));
        datos.Columns.Add("phone", typeof(string));
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("V-001", "Vendedor Activo", "activo@ritrama.com", "809-555-0001", "activo");
        datos.Rows.Add("V-002", "Vendedor Anulado", "anulado@ritrama.com", "809-555-0002", "desactivado");
        return datos;
    }

    [Fact]
    public void Raiz_DivideElAnchoEnTreintaYSetentaPorCiento()
    {
        using FrmVendedores form = CrearFormulario();

        TableLayoutPanel root = (TableLayoutPanel)form.Controls.Find("tlpRoot", true).Single();
        root.ColumnCount.Should().Be(2);
        root.ColumnStyles[0].SizeType.Should().Be(SizeType.Percent);
        root.ColumnStyles[0].Width.Should().Be(30f);
        root.ColumnStyles[1].Width.Should().Be(70f);
    }

    [Fact]
    public void Titulos_DelFormularioYDelPanelSonVendedores()
    {
        using FrmVendedores form = CrearFormulario();

        form.Text.Should().Be("Vendedores");
        form.Controls.Find("lblTitulo", true).Single().Text.Should().Be("CATALOGO DE VENDEDORES");
    }

    [Fact]
    public void PanelIzquierdo_TieneBuscadorYGridDeTresColumnas()
    {
        using FrmVendedores form = CrearFormulario();

        form.Controls.Find("txtBuscar", true).Should().ContainSingle();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();
        grid.Columns.Count.Should().Be(3);
        grid.Columns[0].DataPropertyName.Should().Be("vendor_id");
        grid.Columns[1].DataPropertyName.Should().Be("vendor_name");
        grid.Columns[2].DataPropertyName.Should().Be("status");
        grid.ReadOnly.Should().BeTrue();
        grid.AllowUserToAddRows.Should().BeFalse();
        grid.AutoGenerateColumns.Should().BeFalse();
    }

    [Fact]
    public void PanelIzquierdo_TieneCuadroDeResumenAlPie()
    {
        using FrmVendedores form = CrearFormulario();

        Panel resumen = (Panel)form.Controls.Find("pnlResumen", true).Single();
        resumen.Dock.Should().Be(DockStyle.Bottom);
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 0 vendedores");
    }

    [Fact]
    public void PanelDerecho_TieneLosCamposDelVendedorEnLaPaginaDeDetalle()
    {
        using FrmVendedores form = CrearFormulario();

        form.Controls.Find("panelDer", true).Should().ContainSingle();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleVendedor", true).Single();
        detalle.Text.Should().Be("Detalle");
        form.Controls.Find("lblDetalleTitulo", true).Single().Text.Should().Be("DETALLE DEL VENDEDOR");
        form.Controls.Find("txtValorId", true).Should().ContainSingle();
        form.Controls.Find("txtValorNombre", true).Should().ContainSingle();
        form.Controls.Find("txtValorCorreo", true).Should().ContainSingle();
        form.Controls.Find("txtValorTelefono", true).Should().ContainSingle();
        form.Controls.Find("lblPlaceDetalle", true).Should().BeEmpty();
    }

    [Fact]
    public void Detalle_EnConsultaTodoEsSoloLectura()
    {
        using FrmVendedores form = CrearFormulario();

        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorId", true).Single()).ReadOnly.Should().BeTrue();
        Sunny.UI.UITextBox interno = (Sunny.UI.UITextBox)form.Controls.Find("txtCodigoInterno", true).Single();
        interno.ReadOnly.Should().BeTrue();
        interno.Text.Should().Be("—");
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorNombre", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorCorreo", true).Single()).ReadOnly.Should().BeTrue();
        // Nota: El UISwitch (swEstado) reemplaza al txtValorEstado y se gobierna
        // por ReadOnly (igual que Productos), no por Enabled.
        Sunny.UI.UISwitch estado = (Sunny.UI.UISwitch)form.Controls.Find("swEstado", true).Single();
        estado.Enabled.Should().BeTrue();
        estado.ReadOnly.Should().BeTrue();
        form.Controls.Find("txtValorTelefono", true).Single().Should().NotBeNull();
    }

    // Nota: el Nuevo con GUID + consecutivo 1, 2, 3... no se cubre aquí a
    // propósito: requiere clic con sesión global (SesionActual) y las clases de
    // test corren en paralelo — si otra clase limpia la sesión a mitad del clic,
    // salta un MessageBox real y cuelga la corrida. Se verifica manual.

    [Fact]
    public void GridSeleccion_PintaLaFilaDeNegroConLetrasBlancas()
    {
        using FrmVendedores form = CrearFormulario();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();
        grid.SelectionMode.Should().Be(DataGridViewSelectionMode.FullRowSelect);
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
        // todo lo que toca la base vive detrás del IVendedorService.
        using FrmVendedores form = CrearFormulario();

        form.Should().NotBeNull();
        form.Controls.Find("tlpRoot", true).Should().ContainSingle();
    }

    [Fact]
    public async Task InitializeAsync_LlenaElGridConLoQueDevuelveElServicio()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());

        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();
        grid.Columns.Count.Should().Be(3);
        grid.Rows.Count.Should().Be(2);
        grid.Rows[0].Cells[1].Value.Should().Be("Vendedor Activo");
        grid.Rows[0].Cells[2].Value.Should().Be("activo");
        grid.Rows[1].Cells[2].Value.Should().Be("desactivado");
    }

    [Fact]
    public async Task InitializeAsync_MuestraElTotalDeVendedoresEnElResumen()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());

        await form.InitializeAsync();

        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 2 vendedores");
    }

    [Fact]
    public async Task Buscar_FiltraElListadoPorNombre()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        form.Controls.Find("txtBuscar", true).Single().Text = "Anulado";

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();
        grid.Rows.Count.Should().Be(1);
        grid.Rows[0].Cells[1].Value.Should().Be("Vendedor Anulado");
    }

    [Fact]
    public async Task Buscar_ActualizaElResumenConLoQueSeFiltra()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        form.Controls.Find("txtBuscar", true).Single().Text = "Anulado";

        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Mostrando 1 de 2 vendedores");
    }

    [Fact]
    public async Task SeleccionarFila_RefreshLosCamposDelDetalle()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();
        grid.CurrentCell = grid.Rows[0].Cells[0];

        form.Controls.Find("txtValorId", true).Single().Text.Should().Be("V-001");
        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Vendedor Activo");
        form.Controls.Find("txtValorCorreo", true).Single().Text.Should().Be("activo@ritrama.com");
        form.Controls.Find("txtValorTelefono", true).Single().Text.Should().Be("809-555-0001");
        // El estado ahora se muestra en un UISwitch (swEstado)
        form.Controls.Find("swEstado", true).Should().ContainSingle();

        // Cambiar de fila debe refrescar el detalle con el nuevo vendedor.
        grid.CurrentCell = grid.Rows[1].Cells[0];

        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Vendedor Anulado");
        // El estado en el switch
        Sunny.UI.UISwitch sw = (Sunny.UI.UISwitch)form.Controls.Find("swEstado", true).Single();
        sw.Active.Should().BeFalse();
    }

    [Fact]
    public void Barra_TieneElBotonDeReporteConSuIcono()
    {
        using FrmVendedores form = CrearFormulario();
        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        ToolStripButton boton = (ToolStripButton)barra.Items["btnReporteVendedor"]!;

        boton.Should().NotBeNull("el boton de reporte es la entrada al catalogo en el visor");
        boton.Text.Should().Be("Reporte");
        boton.Image.Should().NotBeNull("el boton de reporte necesita un icono");

        // Tercera posicion, detras de Importar y delante de Guardar (ver FrmProductos).
        barra.Items.IndexOf(boton).Should().Be(3);
        barra.Items.IndexOf(barra.Items["btnGuardarVendedor"]!).Should().BeGreaterThan(barra.Items.IndexOf(boton));
    }

    [Fact]
    public void Barra_ElBotonNuevoDelAltaEstaHabilitado()
    {
        using FrmVendedores form = CrearFormulario();
        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        ToolStripButton nuevo = (ToolStripButton)barra.Items["btnNuevoVendedor"]!;

        // El alta se rige por CreacionHabilitada; si alguien vuelve a ponerla en false el
        // boton desaparece de la barra y hay que actualizar esta prueba a proposito.
        nuevo.Should().NotBeNull("el alta de vendedores esta habilitada");
        nuevo.Enabled.Should().BeTrue();
    }

    [Fact]
    public async Task Acciones_NoExigenPermiso_PorqueElModuloNoTienePermisos()
    {
        // Precondicion: sin sesión autenticada todo permiso da false, y el modulo Vendedores
        // ademas no existe en la tabla permisos. Antes cada accion pedia PuedeCrear/PuedeEditar/
        // PuedeVer("Vendedores"), asi que solo el admin podia hacer algo aqui.
        PermisoHelper.PuedeVer("Vendedores").Should().BeFalse("el modulo no esta en permisos");

        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        ToolStripButton nuevo = (ToolStripButton)barra.Items["btnNuevoVendedor"]!;

        nuevo.PerformClick();

        // Si siguiera exigiendo permiso se quedaria en Consulta con el detalle bloqueado y
        // saldria el aviso "No tiene permiso para crear vendedores" en vez de entrar en alta.
        Sunny.UI.UITextBox nombre = (Sunny.UI.UITextBox)form.Controls.Find("txtValorNombre", true).Single();
        nombre.ReadOnly.Should().BeFalse("Nuevo tiene que dejar el detalle escribible");
        nombre.Text.Should().BeEmpty("en el alta las cajas van en blanco");
        form.Controls.Find("txtValorId", true).Single().Text.Should().NotBe("—",
            "el alta pone el GUID del nuevo vendedor");
    }

    [Fact]
    public async Task Editar_NoExigePermiso_YSeActivaConFilaSeleccionada()
    {
        PermisoHelper.PuedeEditar("Vendedores").Should().BeFalse("el modulo no esta en permisos");

        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();
        grid.CurrentCell = grid.Rows[0].Cells[0];

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        ToolStripButton editar = (ToolStripButton)barra.Items["btnEditarVendedor"]!;

        // Antes exigia PuedeEditar("Vendedores"), que nunca existe, y el boton quedaba
        // muerto para todo el que no fuera admin.
        editar.Enabled.Should().BeTrue("con fila seleccionada Editar se habilita");

        editar.PerformClick();

        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorNombre", true).Single()).ReadOnly
            .Should().BeFalse("Editar tiene que dejar el detalle escribible");
    }

    // ─────────────────────────────────────────────────────────────────
    // Filtro de estado (Todos / Activos / Desactivados)
    // ─────────────────────────────────────────────────────────────────

    private static RadioButton Radio(FrmVendedores form, string nombre)
        => (RadioButton)form.Controls.Find(nombre, true).Single();

    [Fact]
    public void FiltroEstado_TieneLosTresRadiosYArrancaEnTodos()
    {
        using FrmVendedores form = CrearFormulario();

        Radio(form, "rbEstadoTodos").Text.Should().Be("Todos");
        Radio(form, "rbEstadoActivos").Text.Should().Be("Activos");
        Radio(form, "rbEstadoDesactivados").Text.Should().Be("Desactivados");

        Radio(form, "rbEstadoTodos").Checked.Should().BeTrue("sin filtro de estado la lista va completa");
        Radio(form, "rbEstadoActivos").Checked.Should().BeFalse();
        Radio(form, "rbEstadoDesactivados").Checked.Should().BeFalse();

        // Vendedores no tiene categoría: esos radios siguen sin existir aquí.
        form.Controls.Find("rbCatNacional", true).Should().BeEmpty();

        Panel panel = (Panel)form.Controls.Find("pnlFiltroEstado", true).Single();
        panel.Dock.Should().Be(DockStyle.Top);
    }

    [Fact]
    public void FiltroEstado_LosRadiosVanDebajoDelBuscadorYEncimaDelGrid()
    {
        using FrmVendedores form = CrearFormulario();
        form.PerformLayout();

        int buscador = PosicionRelativaA(form.Controls.Find("txtBuscar", true).Single(), form).Y;
        int radios = PosicionRelativaA(form.Controls.Find("rbEstadoTodos", true).Single(), form).Y;
        int grid = PosicionRelativaA(form.Controls.Find("gridVendedores", true).Single(), form).Y;

        buscador.Should().BeLessThan(radios, because: "los radios van debajo del buscador");
        radios.Should().BeLessThan(grid, because: "el grid va debajo de los radios");
    }

    [Fact]
    public async Task FiltroEstado_FiltraActivosYDesactivados()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();

        Radio(form, "rbEstadoActivos").Checked = true;
        grid.Rows.Count.Should().Be(1);
        grid.Rows[0].Cells[1].Value.Should().Be("Vendedor Activo");
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Mostrando 1 de 2 vendedores");

        Radio(form, "rbEstadoDesactivados").Checked = true;
        grid.Rows.Count.Should().Be(1);
        grid.Rows[0].Cells[1].Value.Should().Be("Vendedor Anulado");

        Radio(form, "rbEstadoTodos").Checked = true;
        grid.Rows.Count.Should().Be(2, "'Todos' devuelve la lista completa");
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 2 vendedores");
    }

    [Fact]
    public async Task FiltroEstado_SeCombinaConLaBusquedaPorNombre()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();

        form.Controls.Find("txtBuscar", true).Single().Text = "Anulado";
        Radio(form, "rbEstadoActivos").Checked = true;

        grid.Rows.Count.Should().Be(0, "el texto pide 'Anulado' y el radio pide 'Activos'");
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Mostrando 0 de 2 vendedores");

        Radio(form, "rbEstadoTodos").Checked = true;
        grid.Rows.Count.Should().Be(1, "sin el radio solo queda el filtro de texto");
        grid.Rows[0].Cells[1].Value.Should().Be("Vendedor Anulado");
    }

    // ─────────────────────────────────────────────────────────────────
    // Filas desactivadas en rojo
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Grid_ElVendedorDesactivadoSePintaEnRojoYElActivoNo()
    {
        using FrmVendedores form = CrearFormulario(VendedoresDePrueba());
        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();

        grid.Rows[1].Cells[0].Style.BackColor.Should().Be(FrmVendedores.ColorFondoAnulado);
        grid.Rows[1].Cells[0].Style.ForeColor.Should().Be(FrmVendedores.ColorTextoAnulado);

        grid.Rows[0].Cells[0].Style.BackColor.Should().NotBe(FrmVendedores.ColorFondoAnulado);
        grid.Rows[0].Cells[0].Style.ForeColor.Should().NotBe(FrmVendedores.ColorTextoAnulado);
    }

    [Fact]
    public void Grid_ElRojoYLaLetraClaraSonLosMismosQueEnLosDemasCatalogos()
    {
        // El mismo criterio en los cuatro módulos: fondo rojo de verdad y letra más clara
        // que el fondo, para que se lea encima del rojo.
        FrmVendedores.ColorFondoAnulado.Should().Be(Color.Firebrick);
        FrmVendedores.ColorFondoAnulado.Should().Be(FrmClientes.ColorFondoAnulado);
        FrmVendedores.ColorTextoAnulado.Should().Be(FrmClientes.ColorTextoAnulado);
        FrmVendedores.ColorTextoAnulado.Should().Be(FrmProveedores.ColorTextoAnulado);

        int sumaFondo = FrmVendedores.ColorFondoAnulado.R + FrmVendedores.ColorFondoAnulado.G + FrmVendedores.ColorFondoAnulado.B;
        int sumaTexto = FrmVendedores.ColorTextoAnulado.R + FrmVendedores.ColorTextoAnulado.G + FrmVendedores.ColorTextoAnulado.B;
        sumaTexto.Should().BeGreaterThan(sumaFondo, "la letra va más clara que el fondo");
    }

    // ─────────────────────────────────────────────────────────────────
    // Exportar a Excel sin avisos
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Exportar_NoMuestraNingunMensaje()
    {
        FrmVendedoresSinDialogos form = CrearFormulario(VendedoresDePrueba());
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            ((ToolStripButton)barra.Items["btnImportarVendedor"]!).PerformClick();

            form.Avisos.Should().BeEmpty("al exportar no salen mensajes: el Excel se abre solo");
        }
        finally
        {
            form.Dispose();
        }
    }
}