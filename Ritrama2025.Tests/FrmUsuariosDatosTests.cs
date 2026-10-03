using System.Data;
using System.Reflection;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Services.SeguridadService;
using Ritrama2025.Tests.Stubs;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Carga de datos del listado de FrmUsuarios: proyeccion a las cinco columnas del grid,
/// resolucion de nombres de rol, filtro en vivo del buscador y cuadro de resumen.
/// No toca base de datos: el servicio va con un stub en memoria.
/// </summary>
[Trait("Categoria", "Unit")]
[Collection("Usuarios")]
public class FrmUsuariosDatosTests
{
    /// <summary>
    /// Formulario con los avisos capturados: MostrarAviso avisa con un MessageBox de
    /// verdad y eso bloquearia el test que lo dispare.
    /// </summary>
    private sealed class FrmUsuariosSinDialogos(ISeguridadService servicio, IExportDataService exporta, IReportsService reportes)
        : FrmUsuarios(servicio, exporta, reportes)
    {
        public List<string> Avisos { get; } = [];

        protected override void MostrarAviso(string mensaje) => Avisos.Add(mensaje);
    }

    private static Usuario Usuario(int id, string username, string? email = null, bool activo = true)
        => new() { UserId = id, Username = username, Email = email, Activo = activo };

    private static Role Rol(int id, string nombre) => new() { RoleId = id, Nombre = nombre };

    private static FrmUsuariosSinDialogos CrearFormulario(SeguridadServiceStub servicio)
        => new(servicio, new ExportDataServiceStub(), new ReportsServiceStub());

    private static DataGridView Grid(FrmUsuarios form)
        => (DataGridView)form.Controls.Find("gridUsuarios", true).Single();

    /// <summary>Escribe en el buscador; dispara TextChanged, que es lo que filtra.</summary>
    private static void Buscar(FrmUsuarios form, string texto)
        => ((Control)form.Controls.Find("txtBuscar", true).Single()).Text = texto;

    /// <summary>Las cuatro celdas de una fila, en orden.</summary>
    private static string[] Valores(FrmUsuarios form, int indiceFila)
        => Grid(form).Rows[indiceFila].Cells.Cast<DataGridViewCell>()
            .Select(c => c.Value?.ToString() ?? string.Empty)
            .ToArray();

    private static string ResumenTotal(FrmUsuarios form)
        => ((Control)form.Controls.Find("lblResumen", true).Single()).Text;

    private static string ResumenDetalle(FrmUsuarios form)
        => ((Control)form.Controls.Find("lblResumenDetalle", true).Single()).Text;

    [Fact]
    public async Task InitializeAsync_PueblaIdUsuarioNombreRolYEstadoDeCadaFila()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [new Usuario
            {
                UserId = 1,
                Username = "admin",
                NombreCompleto = "Ana Perez",
                Email = "admin@ritrama.com"
            }],
            roles: [Rol(7, "Administrador")],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [7] });

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        // Las cinco columnas del grid, en orden.
        Valores(form, 0).Should().Equal("1", "admin", "Ana Perez", "Administrador", "Activo");
    }

    [Fact]
    public async Task ElGridMuestraElNombreCompletoDeCadaUsuario()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                new Usuario { UserId = 1, Username = "admin", NombreCompleto = "Ana Perez" },
                new Usuario { UserId = 2, Username = "mostrador", NombreCompleto = "Luis Gomez" }
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Grid(form).Rows[0].Cells["colNombre"].Value.Should().Be("Ana Perez");
        Grid(form).Rows[1].Cells["colNombre"].Value.Should().Be("Luis Gomez");
    }

    [Fact]
    public async Task ElUsuarioSinNombreCompletoApareceConGuionEnElGrid()
    {
        // Una celda en blanco se confunde con que falte el dato. El guion dice
        // "no tiene", que es otra cosa.
        SeguridadServiceStub servicio = new(
            usuarios: [new Usuario { UserId = 1, Username = "mostrador", NombreCompleto = null }]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Grid(form).Rows[0].Cells["colNombre"].Value.Should().Be("—");
    }

    [Fact]
    public async Task InitializeAsync_JuntaLosNombresDeRolDelUsuarioConComa()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [new Usuario { UserId = 1, Username = "admin" }],
            roles: [new Role { RoleId = 1, Nombre = "Admin" }, new Role { RoleId = 2, Nombre = "Super-Admin" }],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1, 2] });

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Grid(form).Rows[0].Cells["colRol"].Value.Should().Be("Admin, Super-Admin");
    }

    [Fact]
    public async Task ElGridNoMuestraRolesInactivos()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [new Usuario { UserId = 1, Username = "admin" }],
            roles:
            [
                new Role { RoleId = 1, Nombre = "Activo", Activo = true },
                new Role { RoleId = 2, Nombre = "Retirado", Activo = false }
            ],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1, 2] });

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Grid(form).Rows[0].Cells["colRol"].Value.Should().Be("Activo");
    }

    [Fact]
    public async Task ElBuscadorFiltraPorUsuarioNombreYCorreo()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                new Usuario { UserId = 1, Username = "ana", NombreCompleto = "Ana Perez", Email = "ana@ritrama.com" },
                new Usuario { UserId = 2, Username = "luis", NombreCompleto = "Luis Gomez", Email = "luis@ritrama.com" },
                new Usuario { UserId = 3, Username = "carlos", NombreCompleto = "Carlos Ruiz", Email = "carlos@otro.com" }
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "ana");

        Grid(form).Rows.Count.Should().Be(1);
        Grid(form).Rows[0].Cells["colUsuario"].Value.Should().Be("ana");
    }

    [Fact]
    public async Task ElBuscadorNoEsCaseSensitive()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [new Usuario { UserId = 1, Username = "Ana", NombreCompleto = "Ana Perez", Email = "ana@ritrama.com" }]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "ANA");
        Grid(form).Rows.Count.Should().Be(1);

        Buscar(form, "ana");
        Grid(form).Rows.Count.Should().Be(1);
    }

    [Fact]
    public async Task ElFiltroDeRolCuelgaDebajoDelBuscadorYEncimaDelGrid()
    {
        using FrmUsuarios form = CrearFormularioVisible(ServicioConRoles());
        await form.InitializeAsync();

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
    public async Task ArrancaConTodosMarcadoYUnRadioPorRolActivo()
    {
        using FrmUsuarios form = CrearFormularioVisible(ServicioConRoles());
        await form.InitializeAsync();

        Control panel = form.Controls.Find("pnlFiltroRol", true).Single();
        Control fila = panel.Controls.Find("flwRadiosRol", true).Single();
        RadioButton[] radios = fila.Controls.OfType<RadioButton>().ToArray();

        // El radio "Inactivo" no aparece: un rol dado de baja no tiene usuarios que
        // mostrar, y ofrecerlo seria un filtro que nunca devuelve nada.
        radios.Select(r => r.Text)
            .Should().Equal("Todos", "Admin", "Invitado");

        radios[0].Checked.Should().BeTrue();
        radios.Where(r => r.Text != "Todos")
            .Select(r => r.Tag)
            .Should().Equal("1", "3");
    }

    [Fact]
    public async Task MarcarUnRadioDeRolDejaSoloLosUsuariosDeEseRol()
    {
        using FrmUsuarios form = CrearFormularioVisible(ServicioConRoles());
        await form.InitializeAsync();

        ClickRadio(form, "Admin");

        Valores(form, 0)[1].Should().Be("admin");
        Grid(form).Rows.Count.Should().Be(2);
        Grid(form).Rows.Cast<DataGridViewRow>()
            .Select(r => r.Cells["colUsuario"].Value?.ToString())
            .Should().Equal("admin", "admin2");
    }

    [Fact]
    public async Task VolverATodosDevuelveLaListaCompleta()
    {
        using FrmUsuarios form = CrearFormularioVisible(ServicioConRoles());
        await form.InitializeAsync();

        ClickRadio(form, "Invitado");
        Grid(form).Rows.Count.Should().Be(1);

        ClickRadio(form, "Todos");

        Grid(form).Rows.Count.Should().Be(3);
    }

    [Fact]
    public async Task ElFiltroDeRolYElBuscadorSeCombinan()
    {
        using FrmUsuarios form = CrearFormularioVisible(ServicioConRoles());
        await form.InitializeAsync();

        ClickRadio(form, "Admin");

        // "ana" esta en el nombre y el correo del primer admin, no en su usuario. Con
        // los dos filtros puestos solo ese queda.
        Buscar(form, "ana");

        Grid(form).Rows.Count.Should().Be(1);
        Valores(form, 0)[1].Should().Be("admin");

        // Y al reves: al cambiar de rol el texto del buscador sigue puesto, y los dos
        // filtros siguen sumandose. "ana" no esta en el invitado, asi que no queda nadie.
        ClickRadio(form, "Invitado");
        Grid(form).Rows.Count.Should().Be(0);

        // Quitado el texto, el filtro de rol solo deja al invitado.
        Buscar(form, string.Empty);
        Grid(form).Rows.Count.Should().Be(1);
        Valores(form, 0)[1].Should().Be("invitado");
    }

    [Fact]
    public async Task ElFiltroDeRolSobreviveAUnaRecargaDelListado()
    {
        using FrmUsuariosSinDialogos form = CrearFormularioVisible(ServicioConRoles());
        await form.InitializeAsync();

        ClickRadio(form, "Admin");

        // Editar vuelve a pedir los roles y CargarCatalogoDeRoles rehace los radios del
        // filtro. Si no se cuida la seleccion, el filtro se pierde solo y el usuario ve
        // la lista completa sin haber pedido nada.
        Click(BotonDeBarra(form, "btnEditarUsuario"));

        // Sin esto la prueba pasaria vacia: si Editar se corta por el camino (sin fila
        // elegida, rol no resuelto...) los radios jamas se vuelven a armar.
        form.Avisos.Should().BeEmpty("el clic en Editar tiene que abrir la edicion");

        Click(BotonDeBarra(form, "btnCancelarUsuario"));

        RadiosDelFiltro(form).Single(r => r.Checked).Text.Should().Be("Admin");
        Grid(form).Rows.Count.Should().Be(2);
    }

    [Fact]
    public async Task Grid_ElUsuarioDesactivadoSePintaEnRojoYElActivoNo()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                Usuario(1, "admin", activo: true),
                Usuario(2, "baja", activo: false)
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Color vigente = ColorPintada(form, 0);
        Color desactivado = ColorPintada(form, 1);

        vigente.Should().NotBe(TextoDesactivadoEsperado,
            "un usuario vigente se ve con su color normal, no apagado");
        desactivado.Should().Be(TextoDesactivadoEsperado,
            "un usuario de baja tiene que verse con letra clara sobre fondo rojo");

        DataGridView grid = Grid(form);
        grid.Rows[1].Cells[0].Style.BackColor.Should().Be(FondoDesactivadoEsperado);
        grid.Rows[0].Cells[0].Style.BackColor.Should().NotBe(FondoDesactivadoEsperado);
    }

    [Fact]
    public async Task Grid_UnaFilaDeBajaSeleccionadaSiguePintandoseEnRojo()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                Usuario(1, "admin", activo: true),
                Usuario(2, "baja", activo: false)
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        // Seleccionar la fila es lo que hace el usuario para mirarla en el detalle, y la
        // seleccion negra del modulo se comia el rojo justo ahi.
        DataGridView grid = Grid(form);
        grid.CurrentCell = grid.Rows[1].Cells[0];

        ColorPintada(form, 1);

        DataGridViewCell celda = grid.Rows[1].Cells[0];
        celda.Style.SelectionBackColor.Should().Be(Color.DarkRed,
            "la seleccion no debe tapar el rojo del usuario de baja");
        celda.Style.SelectionForeColor.Should().Be(TextoDesactivadoEsperado);
    }

    [Fact]
    public async Task ElResumenTotal_CuentaTodosLosUsuarios()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                Usuario(1, "admin", activo: true),
                Usuario(2, "vendedor", activo: false),
                Usuario(3, "vendedor2", activo: false)
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        ResumenTotal(form).Should().Be("Usuarios registrados: 3");
    }

    [Fact]
    public async Task ElResumenDetalle_CuentaActivosEInactivosDelListado()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                Usuario(1, "admin", activo: true),
                Usuario(2, "vendedor", activo: false),
                Usuario(3, "vendedor2", activo: false)
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        // Un activo y dos de baja: el resumen cuenta el listado tal cual esta.
        ResumenDetalle(form).Should().Be("Mostrando 3 - Activos 1 - Inactivos 2");
    }

    [Fact]
    public async Task ElResumenDetalle_SigueAlFiltroYElTotalNoCambia()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                Usuario(1, "admin", activo: true),
                Usuario(2, "vendedor", activo: false),
                Usuario(3, "vendedor2", activo: false)
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "vendedor");

        // El total de arriba es global: el filtro no debe alterar el padron real.
        ResumenTotal(form).Should().Be("Usuarios registrados: 3");
        ResumenDetalle(form).Should().Be("Mostrando 2 - Activos 0 - Inactivos 2");
    }

    [Fact]
    public async Task InitializeAsync_SinUsuariosDejaElFormVacioYElResumenEnCero()
    {
        using FrmUsuarios form = CrearFormulario(new SeguridadServiceStub());

        await form.InitializeAsync();

        Grid(form).Rows.Count.Should().Be(0);
        ResumenTotal(form).Should().Be("Usuarios registrados: 0");
        ResumenDetalle(form).Should().Be("Mostrando 0 - Activos 0 - Inactivos 0");
    }

    [Fact]
    public async Task InitializeAsync_SiLaBaseFallaElFormSigueUtilizable()
    {
        SeguridadServiceStub servicio = new() { ErrorAlListar = new InvalidOperationException("sin conexion") };

        // El fallo no se traga: el formulario lo vuelca por ServiceErrors.Report, que es el
        // canal que usa. Se cambia EL REPORT y no el Notify, que era el error: en una
        // corrida completa la DatabaseFixture deja Report puesto en un no-op y el enlace
        // Report -> Notify nunca se llega a recorrer, con lo que reportado se quedaba nulo.
        string? reportado = null;
        Action<string> reportOriginal = ServiceErrors.Report;
        ServiceErrors.Report = msg => reportado = msg;

        try
        {
            using FrmUsuarios form = CrearFormulario(servicio);
            await form.InitializeAsync();

            Grid(form).Rows.Count.Should().Be(0);
            ResumenTotal(form).Should().Be("Usuarios registrados: 0");
            reportado.Should().Contain("Usuarios").And.Contain("sin conexion");
        }
        finally
        {
            ServiceErrors.Report = reportOriginal;
        }
    }

    /// <summary>
    /// Dispara el CellFormatting del grid sobre una fila —que es lo que hace el pintado—
    /// y devuelve el ForeColor que queda. Se invoca a mano porque el evento solo salta al
    /// pintar de verdad, y un formulario de prueba nunca se dibuja.
    /// </summary>
    private static Color ColorPintada(FrmUsuarios form, int indiceFila)
    {
        DataGridView grid = Grid(form);
        DataGridViewRow fila = grid.Rows[indiceFila];

        MethodInfo formatear = typeof(FrmUsuarios).GetMethod(
            "GridUsuarios_CellFormatting", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // El cellStyle que se le pasa ES el de la celda, que es lo que hace WinForms al
        // lanzar el evento: asi el handler escribe sobre la celda y el cambio se ve.
        object?[] argumentos =
        [
            grid,
            new DataGridViewCellFormattingEventArgs(0, indiceFila, null, typeof(string), fila.Cells[0].Style)
        ];

        formatear.Invoke(form, argumentos);

        return fila.Cells[0].Style.ForeColor;
    }

    /// <summary>El rojo vivo del fondo de las filas de usuarios desactivados (#D02020).</summary>
    private static Color FondoDesactivadoEsperado => Color.FromArgb(208, 32, 32);

    /// <summary>La letra clara con la que se marcan las filas de usuarios desactivados.</summary>
    private static Color TextoDesactivadoEsperado => Color.FromArgb(255, 214, 214);

    /// <summary>
    /// Dos admins y un invitado, con roles que ya existen en la base. El 2 es el rol
    /// "Inactivo" a proposito: tiene usuarios pero no debe aparecer en el filtro.
    /// </summary>
    private static SeguridadServiceStub ServicioConRoles() => new(
        usuarios:
        [
            new Usuario { UserId = 1, Username = "admin", NombreCompleto = "Ana", Email = "ana@ritrama.com" },
            new Usuario { UserId = 2, Username = "admin2", NombreCompleto = "Beto", Email = "beto@ritrama.com" },
            new Usuario { UserId = 3, Username = "invitado", NombreCompleto = "Caro", Email = "caro@ritrama.com" }
        ],
        roles:
        [
            Rol(1, "Admin"),
            new Role { RoleId = 2, Nombre = "Inactivo", Activo = false },
            Rol(3, "Invitado")
        ],
        rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1], [2] = [1], [3] = [3] });

    private static RadioButton[] RadiosDelFiltro(FrmUsuarios form)
        => ((Control)form.Controls.Find("flwRadiosRol", true).Single())
            .Controls.OfType<RadioButton>().ToArray();

    private static void ClickRadio(FrmUsuarios form, string texto)
        => RadiosDelFiltro(form).Single(r => r.Text == texto).PerformClick();

    private static void Click(ToolStripButton boton) => boton.PerformClick();

    private static ToolStripButton BotonDeBarra(FrmUsuarios form, string nombre)
        => (ToolStripButton)((ToolStrip)form.Controls.Find("barraHerramientas", true).Single()).Items[nombre]!;

    /// <summary>
    /// Formulario ya mostrado. Necesario para las pruebas del filtro de rol: un
    /// RadioButton con PerformClick solo responde si esta visible, y sin Show() el
    /// clic se pierde en silencio y la prueba pasa probando nada.
    /// </summary>
    private static FrmUsuariosSinDialogos CrearFormularioVisible(SeguridadServiceStub servicio)
    {
        FrmUsuariosSinDialogos form = CrearFormulario(servicio);
        form.TopLevel = false;
        form.Show();
        Application.DoEvents();
        return form;
    }

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
}