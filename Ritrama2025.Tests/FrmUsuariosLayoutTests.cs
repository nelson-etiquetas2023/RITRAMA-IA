using System.Data;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Tests.Stubs;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño 30/70 de FrmUsuarios: barra de acciones a nivel
/// de pagina (Nuevo / Editar), buscador, grid de cuatro columnas y cuadro de resumen.
/// No toca base de datos: el servicio va con un stub.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmUsuariosLayoutTests
{
    private static FrmUsuarios CrearFormulario()
        => new(new SeguridadServiceStub());

    [Fact]
    public void Raiz_DivideElAnchoEnTreintaYSetentaPorCiento()
    {
        using FrmUsuarios form = CrearFormulario();

        TableLayoutPanel root = (TableLayoutPanel)form.Controls.Find("tlpRoot", true).Single();
        root.ColumnCount.Should().Be(2);
        root.ColumnStyles[0].SizeType.Should().Be(SizeType.Percent);
        root.ColumnStyles[0].Width.Should().Be(30f);
        root.ColumnStyles[1].Width.Should().Be(70f);
    }

    [Fact]
    public void Titulo_DelFormularioEsUsuarios()
    {
        using FrmUsuarios form = CrearFormulario();

        form.Text.Should().Be("Usuarios");
        form.Controls.Find("lblTitulo", true).Single().Text.Should().Be("USUARIOS");
    }

    [Fact]
    public void BarraDeAcciones_EstaANivelDeLaPaginaDeDetalle()
    {
        using FrmUsuarios form = CrearFormulario();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        Control panelDer = form.Controls.Find("panelDer", true).Single();

        // A nivel de la pagina de detalle: cuelga de panelDer, no del form entero.
        barra.Dock.Should().Be(DockStyle.Top);
        barra.Parent.Should().BeSameAs(panelDer);
    }

    /// <summary>
    /// Los ToolStripButton no son Control, asi que Controls.Find no los ve: se buscan
    /// por nombre dentro de los items de la barra.
    /// </summary>
    private static ToolStripButton BotonDe(ToolStrip barra, string nombre)
        => barra.Items.OfType<ToolStripButton>().Single(b => b.Name == nombre);

    [Fact]
    public void BarraDeAcciones_TieneNuevoYEditarEnEseOrden()
    {
        using FrmUsuarios form = CrearFormulario();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

        barra.Items.OfType<ToolStripButton>().Select(b => b.Text).Should().Equal("Nuevo", "Editar");
        BotonDe(barra, "btnNuevoUsuario").ToolTipText.Should().Be("Nuevo usuario");
        BotonDe(barra, "btnEditarUsuario").ToolTipText.Should().Be("Editar el usuario seleccionado");
    }

    [Fact]
    public void BotonEditar_ArrancaDeshabilitadoPorqueNoHaySeleccion()
    {
        using FrmUsuarios form = CrearFormulario();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        BotonDe(barra, "btnNuevoUsuario").Enabled.Should().BeTrue();
        BotonDe(barra, "btnEditarUsuario").Enabled.Should().BeFalse();
    }

    [Fact]
    public void ElBannerQuedaArribaYElContentoAbajo()
    {
        using FrmUsuarios form = CrearFormulario();

        // El docking se resuelve en orden inverso de Controls.Add, asi que el indice
        // mas bajo es la banda mas alta. Esta es la asercion que falla si alguien
        // reordena el Controls.Add del Designer.
        form.Controls.Find("panelTitulo", true).Single().Should().BeSameAs(form.Controls[1]);
        form.Controls.Find("tlpRoot", true).Single().Should().BeSameAs(form.Controls[0]);
    }

    [Fact]
    public void LaBarraQuedaArribaDelTabDeDetalleYNoDelForm()
    {
        using FrmUsuarios form = CrearFormulario();

        Control panelDer = form.Controls.Find("panelDer", true).Single();

        // Indice 1 = Agregada despues del tab = docked arriba. Y la barra no debe
        // aparecer entre los controles del form, que es donde estaba antes.
        panelDer.Controls[1].Should().BeSameAs(form.Controls.Find("barraHerramientas", true).Single());
        panelDer.Controls[0].Should().BeSameAs(form.Controls.Find("tabDetalle", true).Single());
        form.Controls.Find("barraHerramientas", false).Should().BeEmpty();
    }

    [Fact]
    public void PanelIzquierdo_TieneBuscadorGridDeCuatroColumnasYResumen()
    {
        using FrmUsuarios form = CrearFormulario();

        form.Controls.Find("txtBuscar", true).Should().ContainSingle();

        DataGridView grid = (DataGridView)form.Controls.Find("gridUsuarios", true).Single();
        grid.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText)
            .Should().Equal("ID", "Usuario", "Rol", "Estado");

        form.Controls.Find("lblResumen", true).Should().ContainSingle();
        form.Controls.Find("lblResumenDetalle", true).Should().ContainSingle();
    }

    [Fact]
    public void PanelDerecho_TieneLaPaginaDeDetalle()
    {
        using FrmUsuarios form = CrearFormulario();

        Sunny.UI.UITabControl tab = (Sunny.UI.UITabControl)form.Controls.Find("tabDetalle", true).Single();
        tab.TabPages.Cast<TabPage>().Select(p => p.Name).Should().Equal("tabDetalleUsuario");
    }
}
