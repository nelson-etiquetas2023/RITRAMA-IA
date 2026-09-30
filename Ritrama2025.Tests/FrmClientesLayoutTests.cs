using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño 30/70 de FrmClientes.
/// No toca base de datos: construye el formulario con configuración vacía para
/// comprobar que el layout separa listado (30%) y detalle (70%) con sus controles.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmClientesLayoutTests
{
    /// <summary>Construye el formulario con configuración vacía: si resolviera
    /// cadena de conexión, la prueba fallaría.</summary>
    private static FrmClientes CrearFormulario()
        => new(new ConfigurationBuilder().Build());

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
    public void SeConstruye_ConConfiguracionVacia_SinResolverBaseDeDatos()
    {
        // Si el constructor resolviera la cadena de conexión, esto lanzaría.
        using FrmClientes form = CrearFormulario();

        form.Should().NotBeNull();
        form.Controls.Find("tlpRoot", true).Should().ContainSingle();
    }
}
