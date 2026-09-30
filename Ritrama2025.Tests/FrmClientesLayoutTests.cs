using System.Data;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Services.ClienteService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño 30/70 de FrmClientes y de su enlace con el
/// servicio de listado. No toca base de datos: el servicio va con un stub.
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
        datos.Columns.Add("status", typeof(string));
        datos.Rows.Add("C-001", "Cliente Activo SA", "activo");
        datos.Rows.Add("C-002", "Cliente Borrado", "desactivado");
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
    public void PanelDerecho_TienePaginaDeDetalleConMarcador()
    {
        using FrmClientes form = CrearFormulario();

        form.Controls.Find("panelDer", true).Should().ContainSingle();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleCliente", true).Single();
        detalle.Text.Should().Be("Detalle");
        form.Controls.Find("lblPlaceDetalle", true).Should().ContainSingle();
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
        grid.Rows.Count.Should().Be(2);
        grid.Rows[0].Cells[1].Value.Should().Be("Cliente Activo SA");
        grid.Rows[0].Cells[2].Value.Should().Be("activo");
        grid.Rows[1].Cells[2].Value.Should().Be("desactivado");
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
}
