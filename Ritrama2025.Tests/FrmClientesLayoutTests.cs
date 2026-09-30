using System.Data;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Services.ClienteService;
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
    }

    private static FrmClientes CrearFormulario(DataTable? datos = null)
        => new(new ClienteServiceStub(datos));

    /// <summary>Dos clientes de prueba: uno activo y otro desactivado.</summary>
    private static DataTable ClientesDePrueba()
    {
        DataTable datos = new DataTable();
        datos.Columns.Add("customer_id", typeof(string));
        datos.Columns.Add("customer_name", typeof(string));
        datos.Columns.Add("customer_category", typeof(string));
        datos.Columns.Add("customer_email", typeof(string));
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("C-001", "Cliente Activo SA", "Distribuidor", "activo@cliente.com", "activo");
        datos.Rows.Add("C-002", "Cliente Borrado", "Mayorista", "borrado@cliente.com", "desactivado");
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
    public void PanelIzquierdo_TieneCuadroDeResumenDebajoDelBuscador()
    {
        using FrmClientes form = CrearFormulario();

        Panel resumen = (Panel)form.Controls.Find("pnlResumen", true).Single();
        resumen.Dock.Should().Be(DockStyle.Top);
        form.Controls.Find("lblResumen", true).Single().Text.Should().Be("Total: 0 clientes");
    }

    [Fact]
    public void PanelDerecho_TieneLosCamposDelClienteEnLaPaginaDeDetalle()
    {
        using FrmClientes form = CrearFormulario();

        form.Controls.Find("panelDer", true).Should().ContainSingle();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleCliente", true).Single();
        detalle.Text.Should().Be("Detalle");
        form.Controls.Find("lblDetalleTitulo", true).Single().Text.Should().Be("DETALLE DEL CLIENTE");
        form.Controls.Find("txtValorId", true).Should().ContainSingle();
        form.Controls.Find("txtValorNombre", true).Should().ContainSingle();
        form.Controls.Find("txtValorCategoria", true).Should().ContainSingle();
        form.Controls.Find("txtValorEmail", true).Should().ContainSingle();
        form.Controls.Find("txtValorTelefono", true).Should().ContainSingle();
        form.Controls.Find("txtValorEstado", true).Should().ContainSingle();
    }

    [Fact]
    public void Detalle_TxtValorSonEditablesExceptoIdYEstado()
    {
        using FrmClientes form = CrearFormulario();

        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorId", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorEstado", true).Single()).ReadOnly.Should().BeTrue();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorNombre", true).Single()).ReadOnly.Should().BeFalse();
        ((Sunny.UI.UITextBox)form.Controls.Find("txtValorEmail", true).Single()).ReadOnly.Should().BeFalse();
    }

    [Fact]
    public void GridSeleccion_PintaLaFilaDeVerde()
    {
        using FrmClientes form = CrearFormulario();

        DataGridView grid = (DataGridView)form.Controls.Find("gridClientes", true).Single();
        grid.SelectionMode.Should().Be(DataGridViewSelectionMode.FullRowSelect);
        grid.DefaultCellStyle.SelectionBackColor.Should().Be(Color.FromArgb(110, 190, 40));
        grid.DefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
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

        form.Controls.Find("txtValorId", true).Single().Text.Should().Be("C-001");
        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Cliente Activo SA");
        form.Controls.Find("txtValorEstado", true).Single().Text.Should().Be("activo");

        // Cambiar de fila debe refrescar el detalle con el nuevo cliente.
        grid.CurrentCell = grid.Rows[1].Cells[0];

        form.Controls.Find("txtValorNombre", true).Single().Text.Should().Be("Cliente Borrado");
        form.Controls.Find("txtValorEstado", true).Single().Text.Should().Be("desactivado");
    }
}
