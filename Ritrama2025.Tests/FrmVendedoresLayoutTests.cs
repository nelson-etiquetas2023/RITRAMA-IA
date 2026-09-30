using System.Data;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Services.VendedorService;
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
    }

    private static FrmVendedores CrearFormulario(DataTable? datos = null)
        => new(new VendedorServiceStub(datos));

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
    public void PanelIzquierdo_TieneCuadroDeResumenDebajoDelBuscador()
    {
        using FrmVendedores form = CrearFormulario();

        Panel resumen = (Panel)form.Controls.Find("pnlResumen", true).Single();
        resumen.Dock.Should().Be(DockStyle.Top);
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
        form.Controls.Find("txtValorEstado", true).Should().ContainSingle();
        form.Controls.Find("lblPlaceDetalle", true).Should().BeEmpty();
    }

    [Fact]
    public void Detalle_TxtValorSonEditablesExceptoIdYEstado()
    {
        using FrmVendedores form = CrearFormulario();

        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorId", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorEstado", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorNombre", true).Single()).ReadOnly.Should().BeFalse();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorCorreo", true).Single()).ReadOnly.Should().BeFalse();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorTelefono", true).Single()).ReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GridSeleccion_PintaLaFilaDeVerde()
    {
        using FrmVendedores form = CrearFormulario();

        DataGridView grid = (DataGridView)form.Controls.Find("gridVendedores", true).Single();
        grid.SelectionMode.Should().Be(DataGridViewSelectionMode.FullRowSelect);
        grid.DefaultCellStyle.SelectionBackColor.Should().Be(Color.FromArgb(110, 190, 40));
        grid.DefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
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
        form.Controls.Find("txtValorEstado", true).Single().Text.Should().Be("activo");

        // Cambiar de fila debe refrescar el detalle con el nuevo vendedor.
        grid.CurrentCell = grid.Rows[1].Cells[0];

        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Vendedor Anulado");
        form.Controls.Find("txtValorEstado", true).Single().Text.Should().Be("desactivado");
    }
}
