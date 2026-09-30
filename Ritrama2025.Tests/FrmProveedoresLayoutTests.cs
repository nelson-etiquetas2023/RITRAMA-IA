using System.Data;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Services.ProveedorService;
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
    }

    private static FrmProveedores CrearFormulario(DataTable? datos = null)
        => new(new ProveedorServiceStub(datos));

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
    public void PanelIzquierdo_TieneCuadroDeResumenDebajoDelBuscador()
    {
        using FrmProveedores form = CrearFormulario();

        Panel resumen = (Panel)form.Controls.Find("pnlResumen", true).Single();
        resumen.Dock.Should().Be(DockStyle.Top);
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
        form.Controls.Find("txtValorEstado", true).Should().ContainSingle();
        form.Controls.Find("lblPlaceDetalle", true).Should().BeEmpty();
    }

    [Fact]
    public void Detalle_TxtValorSonEditablesExceptoIdYEstado()
    {
        using FrmProveedores form = CrearFormulario();

        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorId", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorEstado", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorNombre", true).Single()).ReadOnly.Should().BeFalse();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorTelefono", true).Single()).ReadOnly.Should().BeFalse();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorDireccion", true).Single()).ReadOnly.Should().BeFalse();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorEmail", true).Single()).ReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GridSeleccion_PintaLaFilaDeVerde()
    {
        using FrmProveedores form = CrearFormulario();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProveedores", true).Single();
        grid.SelectionMode.Should().Be(DataGridViewSelectionMode.FullRowSelect);
        grid.DefaultCellStyle.SelectionBackColor.Should().Be(Color.FromArgb(110, 190, 40));
        grid.DefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
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
        form.Controls.Find("txtValorEstado", true).Single().Text.Should().Be("activo");

        // Cambiar de fila debe refrescar el detalle con el nuevo proveedor.
        grid.CurrentCell = grid.Rows[1].Cells[0];

        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Proveedor Anulado");
        form.Controls.Find("txtValorEstado", true).Single().Text.Should().Be("desactivado");
    }
}
