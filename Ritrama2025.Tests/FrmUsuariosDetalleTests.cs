using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Tests.Stubs;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pagina de detalle de FrmUsuarios: un textbox de solo lectura por propiedad del
/// modelo, que se refresca al elegir una fila del listado.
/// </summary>
[Trait("Categoria", "Unit")]
[Collection("Usuarios")]
public class FrmUsuariosDetalleTests
{
    /// <summary>Los ocho campos de texto del detalle, en el orden en que los arma el Designer.</summary>
    private static readonly string[] Campos =
    [
        "txtDetId", "txtDetUsuario", "txtDetNombre", "txtDetEmail",
        "txtDetTituloCargo", "txtDetDepartamento", "txtDetRoles", "txtDetFechaCreacion"
    ];

    private static Usuario Usuario(
        int id = 1,
        string username = "admin",
        string? email = "admin@ritrama.com",
        string nombre = "Ana Perez",
        bool activo = true,
        string? tituloCargo = "Coordinador de ventas",
        string? departamento = "Ventas")
        => new()
        {
            UserId = id,
            Username = username,
            NombreCompleto = nombre,
            Email = email,
            TituloCargo = tituloCargo,
            Departamento = departamento,
            Activo = activo,
            FechaCreacion = new DateTime(2026, 3, 14, 9, 30, 0),
            UltimoLogin = new DateTime(2026, 9, 1, 18, 5, 0),
            PasswordHash = "$2a$11$NO_DEBE_APARECER_EN_LA_UI"
        };

    private static FrmUsuarios FormCon(SeguridadServiceStub servicio)
        => new(servicio, new ExportDataServiceStub(), new ReportsServiceStub());

    private static FrmUsuarios FormConUsuarios(params Usuario[] usuarios)
        => new(new SeguridadServiceStub([.. usuarios]), new ExportDataServiceStub(), new ReportsServiceStub());

    private static DataGridView Grid(FrmUsuarios form)
        => (DataGridView)form.Controls.Find("gridUsuarios", true).Single();

    /// <summary>Elige la fila del listado: es lo que dispara el refresco del detalle.</summary>
    private static void ElegirFila(FrmUsuarios form, int indice)
    {
        DataGridView grid = Grid(form);
        grid.Rows[indice].Selected = true;
        grid.CurrentCell = grid.Rows[indice].Cells[0];
    }

    /// <summary>Saca la seleccion del grid, que es como queda sin fila elegida.</summary>
    private static void QuitarSeleccion(FrmUsuarios form)
    {
        DataGridView grid = Grid(form);
        grid.ClearSelection();
        grid.CurrentCell = null;
    }

    private static string Valor(FrmUsuarios form, string control)
        => ((Control)form.Controls.Find(control, true).Single()).Text;

    [Fact]
    public async Task SinFilaElegidaElDetalleArrancaTodoEnGuiones()
    {
        // El grid arranca con SelectedIndex = -1, o sea que nunca hay seleccion al cargar.
        using FrmUsuarios form = FormConUsuarios(Usuario());
        await form.InitializeAsync();

        foreach (string campo in Campos)
        {
            Valor(form, campo).Should().Be("—", campo);
        }
    }

    [Fact]
    public async Task ElegirUnaFilaMuestraLasPropiedadesDelUsuario()
    {
        using FrmUsuarios form = FormConUsuarios(Usuario());
        await form.InitializeAsync();

        ElegirFila(form, 0);

        Valor(form, "txtDetId").Should().Be("1");
        Valor(form, "txtDetUsuario").Should().Be("admin");
        Valor(form, "txtDetNombre").Should().Be("Ana Perez");
        Valor(form, "txtDetEmail").Should().Be("admin@ritrama.com");
        Valor(form, "txtDetTituloCargo").Should().Be("Coordinador de ventas");
        Valor(form, "txtDetDepartamento").Should().Be("Ventas");
    }

    [Fact]
    public async Task SinPuestoNiAreaElDetalleMuestraGuiones()
    {
        // Las dos columnas son NULL en las filas anteriores a la migración.
        using FrmUsuarios form = FormConUsuarios(Usuario(tituloCargo: null, departamento: null));
        await form.InitializeAsync();

        ElegirFila(form, 0);

        Valor(form, "txtDetTituloCargo").Should().Be("—");
        Valor(form, "txtDetDepartamento").Should().Be("—");
    }

    [Fact]
    public async Task ElDetallePintaElEstadoEnElSwitchYNoEnUnTextbox()
    {
        using FrmUsuarios form = FormConUsuarios(Usuario(activo: true));
        await form.InitializeAsync();

        ElegirFila(form, 0);

        // El estado es siempre el switch de SunnyUI, tambien en consulta. Antes habia un
        // textbox con "Sí"/"No" que se sustituia por el switch al escribir.
        Sunny.UI.UISwitch interruptor =
            (Sunny.UI.UISwitch)form.Controls.Find("swDetActivo", true).Single();

        // Visible no se comprueba: este archivo no muestra el formulario, y Visible de
        // cualquier control cuyo padre no lo esta da false tanto si se ve como si no.
        // La visibilidad se prueba en FrmUsuariosAltaEdicionTests, que sí hace Show().
        interruptor.Active.Should().BeTrue();
        interruptor.ReadOnly.Should().BeTrue("en consulta no se cambia el estado");
        form.Controls.Find("txtDetActivo", true).Should().BeEmpty();
    }

    [Fact]
    public async Task ElDetalleNoTieneCamposDePrimerNiUltimoLogin()
    {
        using FrmUsuarios form = FormConUsuarios(Usuario());
        await form.InitializeAsync();

        // Se quitaron del formulario. El modelo y la columna siguen porque el primer
        // login es lo que obliga a cambiar la clave al entrar (ver Program.cs).
        form.Controls.Find("txtDetPrimerLogin", true).Should().BeEmpty();
        form.Controls.Find("txtDetUltimoLogin", true).Should().BeEmpty();
        form.Controls.Find("lblCapPrimerLogin", true).Should().BeEmpty();
        form.Controls.Find("lblCapUltimoLogin", true).Should().BeEmpty();
    }

    [Fact]
    public async Task ElDetalleMuestraLosRolesDelUsuarioElegido()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Usuario(1, "admin"), Usuario(2, "mostrador", nombre: "Luis Gomez")],
            roles: [new Role { RoleId = 1, Nombre = "Admin" }, new Role { RoleId = 2, Nombre = "Super-Admin" }],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1, 2], [2] = [2] });

        using FrmUsuarios form = FormCon(servicio);
        await form.InitializeAsync();

        ElegirFila(form, 1);

        // Solo los roles del segundo usuario, no los del primero.
        Valor(form, "txtDetRoles").Should().Be("Super-Admin");
    }

    [Fact]
    public async Task ElDetalleMuestraLaFechaDeCreacionEnFormatoDeLectura()
    {
        using FrmUsuarios form = FormConUsuarios(Usuario());
        await form.InitializeAsync();

        ElegirFila(form, 0);

        Valor(form, "txtDetFechaCreacion").Should().Be("14/03/2026 09:30");
    }

    [Fact]
    public async Task UnUsuarioNuncaMostradoDaGuionesEnLosTextboxDeTexto()
    {
        using FrmUsuarios form = FormConUsuarios(Usuario(id: 7, email: null, nombre: "Sin Correo"));
        await form.InitializeAsync();

        ElegirFila(form, 0);

        // Email nullable: vacio es guion, no "" ni "null".
        Valor(form, "txtDetEmail").Should().Be("—");
        Valor(form, "txtDetId").Should().Be("7");
    }

    [Fact]
    public async Task SacarLaSeleccionVuelveElDetalleAGuiones()
    {
        using FrmUsuarios form = FormConUsuarios(Usuario(), Usuario(2, "mostrador"));
        await form.InitializeAsync();

        ElegirFila(form, 0);
        Valor(form, "txtDetUsuario").Should().Be("admin");

        QuitarSeleccion(form);

        Valor(form, "txtDetUsuario").Should().Be("—");
        Valor(form, "txtDetId").Should().Be("—");
        Valor(form, "txtDetRoles").Should().Be("—");
    }

    [Fact]
    public async Task ElDetalleNuncaMuestraElHashDeLaContrasena()
    {
        using FrmUsuarios form = FormConUsuarios(Usuario());
        await form.InitializeAsync();

        ElegirFila(form, 0);

        // PasswordHash viaja a la UI por el modelo. Al menos que no se muestre en pantalla.
        foreach (string campo in Campos)
        {
            Valor(form, campo).Should().NotContain("2a$11", campo);
        }
    }

    [Fact]
    public void LosTextboxDelDetalleSonDeSoloLectura()
    {
        // El detalle es de consulta: escribir ahi no guarda nada y engaña al usuario.
        using FrmUsuarios form = FormConUsuarios();

        foreach (string campo in Campos)
        {
            ((Sunny.UI.UITextBox)form.Controls.Find(campo, true).Single()).ReadOnly.Should().BeTrue(campo);
        }
    }
}
