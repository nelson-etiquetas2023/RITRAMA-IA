using System.Data;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Tests.Stubs;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño 30/70 de FrmUsuarios: barra de acciones a nivel
/// de pagina (Nuevo / Editar), buscador, grid de cinco columnas y cuadro de resumen.
/// No toca base de datos: el servicio va con un stub.
/// </summary>
[Trait("Categoria", "Unit")]
[Collection("Usuarios")]
public class FrmUsuariosLayoutTests
{
    private static FrmUsuarios CrearFormulario()
        => new(new SeguridadServiceStub(), new ExportDataServiceStub(), new ReportsServiceStub());

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
    public void Titulo_USUARIOSEstaCentradoEnLaBandaDelTitulo()
    {
        using FrmUsuarios form = CrearFormulario();

        Sunny.UI.UILabel titulo = (Sunny.UI.UILabel)form.Controls.Find("lblTitulo", true).Single();
        Panel banda = (Panel)form.Controls.Find("panelTitulo", true).Single();

        titulo.TextAlign.Should().Be(ContentAlignment.MiddleCenter);

        // El rotulo va con Dock=Fill, asi que solo alcanza el area que el padding deja
        // libre: con un padding a la izquierda el centro del texto caia desplazado.
        banda.Padding.Should().Be(Padding.Empty);
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
    public void BarraDeAcciones_TieneNuevoEditarExportarReporteYDespuesGuardarYCancelar()
    {
        using FrmUsuarios form = CrearFormulario();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

        // Guardar y Cancelar van al final. Si estan visibles se comprueba con el
        // formulario mostrado, porque Visible de un ToolStripItem tambien depende de
        // que la barra este visible: aca el form todavia no se mostro.
        barra.Items.OfType<ToolStripButton>().Select(b => b.Text)
            .Should().Equal("Nuevo", "Editar", "Exportar", "Reporte", "Guardar", "Cancelar");

        BotonDe(barra, "btnNuevoUsuario").ToolTipText.Should().Be("Nuevo usuario");
        BotonDe(barra, "btnEditarUsuario").ToolTipText.Should().Be("Editar el usuario seleccionado");
        BotonDe(barra, "btnExportarUsuario").ToolTipText.Should()
            .Be("Crear una hoja de Excel con todos los usuarios");
        BotonDe(barra, "btnReporteUsuario").ToolTipText.Should()
            .Be("Ver el catalogo de usuarios en el visor de reportes");
    }

    [Fact]
    public void BotonExportar_TieneIconoDeExcel()
    {
        using FrmUsuarios form = CrearFormulario();

        ToolStripButton boton = BotonDe(
            (ToolStrip)form.Controls.Find("barraHerramientas", true).Single(), "btnExportarUsuario");

        boton.Image.Should().NotBeNull("sin icono el boton se lee como una accion mas de texto");
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
    public void PanelIzquierdo_TieneBuscadorGridDeCincoColumnasYResumen()
    {
        using FrmUsuarios form = CrearFormulario();

        form.Controls.Find("txtBuscar", true).Should().ContainSingle();

        DataGridView grid = (DataGridView)form.Controls.Find("gridUsuarios", true).Single();

        // Nombre va despues de Usuario: es el mismo dato que la persona, y al lado se
        // lee. Si una columna pierde su DataPropertyName aparece una celda extra y
        // esta asercion lo delata.
        grid.Columns.Cast<DataGridViewColumn>().Select(c => c.HeaderText)
            .Should().Equal("ID", "Usuario", "Nombre", "Rol", "Estado");

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

    // ─────────────────────────────────────────────────────────────
    // Detalle: filas, orden y el hueco de la contraseña
    // ─────────────────────────────────────────────────────────────

    private static TableLayoutPanel Detalle(FrmUsuarios form)
        => (TableLayoutPanel)form.Controls.Find("tlpDetalle", true).Single();

    /// <summary>
    /// Posicion de un control dentro de un ancestro, sumando la cadena de Left/Top. Los
    /// controles del filtro cuelgan de panelIzq y el buscador de pnlBuscador: comparar sus
    /// Top en crudo compararia dos sistemas de coordenadas distintos.
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

    /// <summary>Controles del detalle en el orden en que los dibuja el TableLayoutPanel.</summary>
    private static string[] ControlesEnOrdenDeFilas(FrmUsuarios form)
        => [.. Detalle(form).Controls.Cast<Control>()
            .OrderBy(c => Detalle(form).GetRow(c))
            .Select(c => c.Name)];

    [Fact]
    public void ElDetalleNoTieneFilasVacias()
    {
        using FrmUsuarios form = CrearFormulario();

        TableLayoutPanel detalle = Detalle(form);

        // Antes sobraba una fila de 90 px sin nada dentro, que empujaba el conjunto hacia
        // abajo sin aportar nada. Toda fila declarada tiene que tener controles.
        detalle.RowStyles.Count.Should().Be(detalle.RowCount);
        detalle.RowCount.Should().Be(10);

        for (int fila = 0; fila < detalle.RowCount; fila++)
        {
            detalle.Controls.Cast<Control>().Should().Contain(c => detalle.GetRow(c) == fila,
                $"la fila {fila} no tiene ningun control");
        }
    }

    [Fact]
    public void ElDetalleVaDeIdentidadAEstadoConLaContrasenaDebajoDelUsuario()
    {
        using FrmUsuarios form = CrearFormulario();

        // El estado es lo ultimo: lo decide el administrador, no se escribe al completar
        // los datos. Y puesto + area van pegados al correo, que es donde se leen. Primer
        // login y ultimo login ya no estan: se leen mejor en la auditoria que en el alta.
        // La contraseña va pegada al usuario porque es su segunda parte —se escribe justo
        // cuando se da de alta—; antes quedaba entre el area y el rol, lejos de donde
        // se escribe.
        ControlesEnOrdenDeFilas(form).Should().Equal(
            "lblCapId", "txtDetId",
            "lblCapUsuario", "txtDetUsuario",
            "lblCapPassword", "pnlPassword",
            "lblCapNombre", "txtDetNombre",
            "lblCapEmail", "txtDetEmail",
            "lblCapTituloCargo", "txtDetTituloCargo",
            "lblCapDepartamento", "txtDetDepartamento",
            "lblCapRoles", "txtDetRoles", "cboRoles",
            "lblCapFechaCreacion", "txtDetFechaCreacion",
            "lblCapActivo", "swDetActivo");
    }

    [Fact]
    public void CadaRotuloDelDetalleTieneSuTitulo()
    {
        using FrmUsuarios form = CrearFormulario();

        // El rotulo es el titulo del campo: sin Text la pagina de detalle se queda con
        // los campos en blanco, sin decir que contiene cada uno de ellos.
        (string nombre, string titulo)[] rotulos =
        [
            ("lblCapId", "Código"),
            ("lblCapUsuario", "Usuario"),
            ("lblCapPassword", "Contraseña"),
            ("lblCapNombre", "Nombre"),
            ("lblCapEmail", "Email"),
            ("lblCapTituloCargo", "Cargo"),
            ("lblCapDepartamento", "Departamento"),
            ("lblCapRoles", "Roles"),
            ("lblCapFechaCreacion", "Fecha de creación"),
            ("lblCapActivo", "Estado")
        ];

        foreach ((string nombre, string titulo) in rotulos)
        {
            Control rotulo = Detalle(form).Controls.Find(nombre, true).Single();
            rotulo.Text.Should().Be(titulo, $"el rotulo {nombre} es el titulo del campo");
        }
    }

    [Fact]
    public void LaContrasenaSeEscribeOcultaConPuntitos()
    {
        using FrmUsuarios form = CrearFormulario();

        // SunnyUI nace con PasswordChar = ' ', o sea "no enmascares": sin esto la
        // contraseña se leia en claro mientras se escribia. Un espacio es el unico valor
        // que deja de enmascarar, asi que cualquier caracter visible sirve de masque.
        Sunny.UI.UITextBox contrasena =
            (Sunny.UI.UITextBox)form.Controls.Find("txtPassword", true).Single();

        contrasena.PasswordChar.Should().NotBe(' ');
        contrasena.PasswordChar.Should().Be('•');
    }

    [Fact]
    public void FueraDelAltaLaFilaDeLaContrasenaNoOcupaAlto()
    {
        using FrmUsuarios form = CrearFormulario();

        // En un TableLayoutPanel el alto lo manda el RowStyle, no la visibilidad: con
        // la fila en 38 px se quedaba un hueco entre el usuario y el nombre.
        Detalle(form).RowStyles[2].Height.Should().Be(0, "la contraseña no se ve en consulta");
    }

    [Fact]
    public void ElInterruptorDeActivoEsUnUISwitchConElTamanoPorDefectoDeSunny()
    {
        using FrmUsuarios form = CrearFormulario();

        Sunny.UI.UISwitch interruptor =
            (Sunny.UI.UISwitch)form.Controls.Find("swDetActivo", true).Single();

        // 75x29 es lo que trae Sunny.UI.UISwitch recien construido: no se estira a la
        // celda ni se estira a la fila.
        interruptor.Size.Should().Be(new Size(75, 29));
        interruptor.Dock.Should().Be(DockStyle.None);

        // Los textos por defecto de SunnyUI son "开" y "关": hay que fijarlos.
        interruptor.ActiveText.Should().Be("Activo");
        interruptor.InActiveText.Should().Be("Inactivo");
    }

    [Fact]
    public void ElOjoDeLaContrasenaViveDentroDelCampoYNoAlLado()
    {
        using FrmUsuarios form = CrearFormulario();

        // Con dos controles dockeados en la misma celda del TableLayoutPanel el reparto
        // depende del z-order. El ojo va dentro de un panel propio, pegado a la derecha.
        Control panel = form.Controls.Find("pnlPassword", true).Single();
        Control ojo = form.Controls.Find("btnOjoPassword", true).Single();
        Control campo = form.Controls.Find("txtPassword", true).Single();

        ojo.Parent.Should().BeSameAs(panel);
        campo.Parent.Should().BeSameAs(panel);
        ojo.Dock.Should().Be(DockStyle.Right);
        panel.Dock.Should().Be(DockStyle.Fill);

        // Y el panel es el que ocupa la celda del detalle, no el textbox pelado.
        Detalle(form).GetRow(panel).Should().Be(2);
    }

    [Theory]
    [InlineData("btnNuevoUsuario")]
    [InlineData("btnEditarUsuario")]
    [InlineData("btnGuardarUsuario")]
    [InlineData("btnCancelarUsuario")]
    public void Barra_ElIconoYElTextoDeCadaBotonEntranCompletos(string nombre)
    {
        using FrmUsuarios form = CrearFormulario();
        form.CreateControl();
        form.PerformLayout();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

        // Guardar y Cancelar nacen ocultos: solo existen mientras se escribe. Un item
        // invisible no pasa por el layout, asi que hay que mostrarlos para medirlos
        // como el usuario los ve en el alta.
        foreach (ToolStripButton item in barra.Items.OfType<ToolStripButton>())
        {
            item.Visible = true;
        }

        barra.PerformLayout();

        ToolStripButton boton = (ToolStripButton)barra.Items[nombre]!;

        // El disenador deja los botones en 92x36 con ImageScaling = None, y los iconos
        // de Resources vienen en tamanos dispares: el de Nuevo es de 32 px. Con eso el
        // icono se sale del boton y el texto queda recortado. ToolStripTheme.Ajustar
        // normaliza a 24x24 con SizeToFit y AutoSize.
        //
        // La referencia es el GetPreferredSize DEL PROPIO ToolStrip, no un
        // TextRenderer.MeasureText aparte: el layout del ToolStrip excluye el leading
        // interno que MeasureText si cuenta, y con "Cancelar" —la etiqueta mas larga de
        // la barra— esa diferencia de unos pixeles hacia fallar un boton que esta bien.
        Size preferido = boton.GetPreferredSize(Size.Empty);

        boton.AutoSize.Should().BeTrue();
        boton.ImageScaling.Should().Be(ToolStripItemImageScaling.SizeToFit);
        boton.Width.Should().BeGreaterThanOrEqualTo(preferido.Width,
            "el icono y el texto de " + nombre + " no deben quedar cortados");
        boton.Height.Should().BeGreaterThanOrEqualTo(preferido.Height);
    }

    [Fact]
    public void LaBarraMideLoQueMidenSusBotones()
    {
        using FrmUsuarios form = CrearFormulario();
        form.CreateControl();
        form.PerformLayout();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        int anchoBotones = barra.Items.OfType<ToolStripButton>()
            .Sum(b => b.Width + b.Margin.Horizontal);

        barra.Width.Should().BeGreaterThanOrEqualTo(anchoBotones,
            "con un boton por lado se sale de la barra y el ultimo queda cortado");
        barra.Height.Should().BeGreaterThanOrEqualTo(barra.Items.OfType<ToolStripButton>()
            .Where(b => b.Available).Max(b => b.Height));
    }

    [Fact]
    public void ElRotuloDeActivoYElSwitchCuentanConLaMismaFuente()
    {
        using FrmUsuarios form = CrearFormulario();

        Sunny.UI.UILabel rotulo =
            (Sunny.UI.UILabel)form.Controls.Find("lblCapActivo", true).Single();
        Sunny.UI.UISwitch interruptor =
            (Sunny.UI.UISwitch)form.Controls.Find("swDetActivo", true).Single();

        // UIStyleManager con GlobalFont le ponia 12pt al switch y 9pt al rotulo: el mismo
        // dato escrito con dos tamanos. Con la letra mas grande, la palabra del switch se
        // iba de la linea del rotulo y no se leian como el mismo dato.
        interruptor.Font.Size.Should().Be(rotulo.Font.Size);
        interruptor.Font.Name.Should().Be(rotulo.Font.Name);
    }

[Fact]
    public void ElFiltroDeRolCuelgaDebajoDelBuscadorYEncimaDelGrid()
    {
        using FrmUsuarios form = CrearFormulario();
        form.CreateControl();
        form.PerformLayout();

        Control panel = form.Controls.Find("pnlFiltroRol", true).Single();
        Control buscador = form.Controls.Find("pnlBuscador", true).Single();
        Control grid = form.Controls.Find("gridUsuarios", true).Single();

        panel.Dock.Should().Be(DockStyle.Top);
        panel.Parent.Should().BeSameAs(buscador.Parent, "el filtro va en la columna del listado");

        int inicio = PosicionRelativaA(panel, form).Y;
        int fin = inicio + panel.Height;
        int inicioBuscador = PosicionRelativaA(buscador, form).Y;
        int finBuscador = inicioBuscador + buscador.Height;
        int inicioGrid = PosicionRelativaA(grid, form).Y;

        finBuscador.Should().BeLessThanOrEqualTo(inicio, "el filtro va DEBAJO del buscador");
        fin.Should().BeLessThanOrEqualTo(inicioGrid, "el filtro va ENCIMA del grid");
    }

    [Fact]
    public void ArrancaConTodosMarcadoYUnRadioPorRolActivo()
    {
        using FrmUsuarios form = CrearFormulario();

        Control panel = form.Controls.Find("pnlFiltroRol", true).Single();
        Control fila = panel.Controls.Find("flwRadiosRol", true).Single();
        RadioButton[] radios = fila.Controls.OfType<RadioButton>().ToArray();

        // El constructor todavia no tiene catalogo: solo "Todos". Los radios de rol los
        // arma CargarCatalogoDeRoles cuando llega de la base.
        radios.Should().ContainSingle();
        radios[0].Name.Should().Be("rbRolTodos");
        radios[0].Checked.Should().BeTrue();
        radios[0].Tag.Should().BeNull("Todos no filtra por ningun rol");
    }

    [Fact]
    public void ElRotuloDeActivoQuedaAlineadoConElSwitch()
    {
        using FrmUsuarios form = CrearFormulario();

        Sunny.UI.UISwitch interruptor =
            (Sunny.UI.UISwitch)form.Controls.Find("swDetActivo", true).Single();
        Sunny.UI.UILabel rotulo =
            (Sunny.UI.UILabel)form.Controls.Find("lblCapActivo", true).Single();

        // El texto del rotulo arranca en Margin.Left + Padding.Left = 3 + 2 = 5. El
        // switch tiene que arrancar en el mismo x, o el boton queda a la izquierda del
        // texto y las dos lineas no leen como una.
        int inicioRotulo = rotulo.Margin.Left + rotulo.Padding.Left;
        interruptor.Margin.Left.Should().Be(inicioRotulo);

        // Y los 29 px del switch tienen que quedar centrados en la fila de 38, con el
        // mismo centro que el texto MiddleLeft del rotulo. Con alto impar de celda el
        // centro cae en medio pixel, asi que se tolera menos de 1 px.
        float altoFila = Detalle(form).RowStyles[Detalle(form).GetRow(interruptor)].Height;
        altoFila.Should().Be(38);

        float desvio = (altoFila - interruptor.Height) / 2 - interruptor.Margin.Top;
        Math.Abs(desvio).Should().BeLessThan(1, "el switch queda centrado en la fila");
    }

    [Fact]
    public void ElRolEsUnComboDeUnSoloValorYNoUnaListaDeCasillas()
    {
        using FrmUsuarios form = CrearFormulario();

        Sunny.UI.UIComboBox combo = (Sunny.UI.UIComboBox)form.Controls.Find("cboRoles", true).Single();

        combo.DropDownStyle.Should().Be(Sunny.UI.UIDropDownStyle.DropDownList);

        // Items es un ObjectCollection, no un IEnumerable<T>: solo tiene Count.
        combo.Items.Count.Should().Be(0, "los roles los llena el catalogo, no el diseñador");
        form.Controls.Find("clbRoles", true).Should().BeEmpty();
    }
}

