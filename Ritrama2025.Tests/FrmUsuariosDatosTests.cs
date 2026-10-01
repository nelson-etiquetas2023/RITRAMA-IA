using System.Data;
using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Tests.Stubs;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Carga de datos del listado de FrmUsuarios: proyeccion a las cuatro columnas del grid,
/// resolucion de nombres de rol, filtro en vivo del buscador y cuadro de resumen.
/// No toca base de datos: el servicio va con un stub en memoria.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmUsuariosDatosTests
{
    private static Usuario Usuario(int id, string username, string? email = null, bool activo = true)
        => new() { UserId = id, Username = username, Email = email, Activo = activo };

    private static Role Rol(int id, string nombre) => new() { RoleId = id, Nombre = nombre };

    private static FrmUsuarios CrearFormulario(SeguridadServiceStub servicio)
        => new(servicio);

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
    public async Task InitializeAsync_PueblaIdUsuarioRolYEstadoDeCadaFila()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Usuario(1, "admin", "admin@ritrama.com")],
            roles: [Rol(7, "Administrador")],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [7] });

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        // Las cuatro columnas del grid, en orden. Si una columna pierde su
        // DataPropertyName aparece una celda extra y esta asercion lo delata.
        Valores(form, 0).Should().Equal("1", "admin", "Administrador", "Activo");
    }

    [Fact]
    public async Task InitializeAsync_JuntaLosNombresDeRolDelUsuarioConComa()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Usuario(1, "admin"), Usuario(2, "mostrador")],
            roles: [Rol(1, "Administrador"), Rol(2, "Supervisor")],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1, 2], [2] = [2] });

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Grid(form).Rows[0].Cells["colRol"].Value.Should().Be("Administrador, Supervisor");
    }

    [Fact]
    public async Task InitializeAsync_MarcaConGuionElUsuarioQueNoTieneRoles()
    {
        SeguridadServiceStub servicio = new(usuarios: [Usuario(2, "mostrador")]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        // Un guion y no una celda en blanco: el listado se lee de un vistazo.
        Grid(form).Rows[0].Cells["colRol"].Value.Should().Be("—");
    }

    [Fact]
    public async Task InitializeAsync_TraduceElBoolActivoAActivoOInactivo()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Usuario(1, "admin", activo: true), Usuario(2, "baja", activo: false)]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Grid(form).Rows[0].Cells["colEstado"].Value.Should().Be("Activo");
        Grid(form).Rows[1].Cells["colEstado"].Value.Should().Be("Inactivo");
    }

    [Fact]
    public async Task InitializeAsync_ConsultaElCatalogoDeRolesUnaSolaVez()
    {
        // El catalogo es el mismo para todos los usuarios: pedirlo por usuario seria
        // el N+1 completo. Esta es la asercion que protege el cache.
        SeguridadServiceStub servicio = new(
            usuarios: [Usuario(1, "admin"), Usuario(2, "mostrador"), Usuario(3, "vendedor")],
            roles: [Rol(1, "Administrador")],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1], [2] = [1], [3] = [1] });

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        servicio.LlamadasGetRoles.Should().Be(1);
        servicio.LlamadasGetUsuarios.Should().Be(1);
    }

    [Fact]
    public async Task ElResumenSuperior_MuestraElTotalDeUsuariosRegistrados()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Usuario(1, "admin"), Usuario(2, "mostrador"), Usuario(3, "vendedor")]);

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
                Usuario(2, "mostrador", activo: true),
                Usuario(3, "baja", activo: false)
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        ResumenDetalle(form).Should().Be("Mostrando 3 - Activos 2 - Inactivos 1");
    }

    [Fact]
    public async Task ElBuscadorFiltraPorNombreDeUsuario()
    {
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                Usuario(1, "admin"),
                Usuario(2, "mostrador"),
                Usuario(3, "vendedor")
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "vend");

        Grid(form).Rows.Count.Should().Be(1);
        Grid(form).Rows[0].Cells["colUsuario"].Value.Should().Be("vendedor");
    }

    [Fact]
    public async Task ElBuscadorTambienFiltraPorCorreo()
    {
        // El watermark dice "usuario o correo": un solo campo, y con OR entre los dos.
        SeguridadServiceStub servicio = new(
            usuarios:
            [
                Usuario(1, "admin", "jefe@ritrama.com"),
                Usuario(2, "mostrador", "piso@ritrama.com"),
                Usuario(3, "vendedor", "calle@ritrama.com")
            ]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "piso@");

        Grid(form).Rows.Count.Should().Be(1);
        Grid(form).Rows[0].Cells["colUsuario"].Value.Should().Be("mostrador");
    }

    [Fact]
    public async Task ElBuscadorIgnoraMayusculasYMinusculas()
    {
        SeguridadServiceStub servicio = new(usuarios: [Usuario(1, "Admin"), Usuario(2, "vendedor")]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "ADMIN");

        Grid(form).Rows.Count.Should().Be(1);
    }

    [Fact]
    public async Task UnBusquedaSoloConEspaciosTraeTodasLasFilas()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Usuario(1, "admin"), Usuario(2, "vendedor")]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "   ");

        Grid(form).Rows.Count.Should().Be(2);
    }

    [Fact]
    public async Task BuscarSinCoincidenciasDejaElListadoVacioYDeshabilitaEditar()
    {
        SeguridadServiceStub servicio = new(usuarios: [Usuario(1, "admin"), Usuario(2, "vendedor")]);

        using FrmUsuarios form = CrearFormulario(servicio);
        await form.InitializeAsync();

        Buscar(form, "zzzz");

        Grid(form).Rows.Count.Should().Be(0);

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        barra.Items.OfType<ToolStripButton>().Single(b => b.Name == "btnEditarUsuario")
            .Enabled.Should().BeFalse();
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

        string? reportado = null;
        Action<string> notifyOriginal = ServiceErrors.Notify;
        ServiceErrors.Notify = msg => reportado = msg;

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
            ServiceErrors.Notify = notifyOriginal;
        }
    }
}