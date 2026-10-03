using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Services.SeguridadService;
using Ritrama2025.Tests.Stubs;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Hoja de Excel de FrmUsuarios: el boton Exportar de la barra. Es el mismo
/// comportamiento que Importar en Productos — padron completo sin filtrar, el fallo
/// avisa y el exito no — con una regla propia: la contraseña NUNCA viaja al fichero.
/// No toca base de datos: el servicio de seguridad va con un stub y el de exportacion
/// con un stub en memoria, para no escribir un fichero ni abrir Excel dentro de un test.
/// </summary>
[Trait("Categoria", "Unit")]
[Collection("Usuarios")]
public class FrmUsuariosExportarTests : IDisposable
{
    private readonly Usuario? _usuarioAnterior = SesionActual.Usuario;
    private readonly List<string> _permisosAnteriores = [.. SesionActual.Permisos];

    /// <summary>
    /// Sesion con permiso de ver (y de crear, que es el que pide abrir el alta). Sin
    /// esto PermisoHelper devuelve false y el clic no llega al servicio, igual que en
    /// las pruebas de Productos. Se restaura al terminar.
    /// </summary>
    public FrmUsuariosExportarTests()
    {
        SesionActual.Usuario = new Usuario { UserId = 1, Username = "prueba" };
        SesionActual.Permisos.AddRange(["Usuarios:Ver", "Usuarios:Crear"]);
    }

    public void Dispose()
    {
        SesionActual.Usuario = _usuarioAnterior;
        SesionActual.Permisos.Clear();
        SesionActual.Permisos.AddRange(_permisosAnteriores);
    }

    /// <summary>
    /// Formulario con los dialogos sustituidos por capturas: un MessageBox real dentro
    /// de una prueba la dejaria colgada a la espera de que alguien pulse un boton.
    /// </summary>
    private sealed class FrmUsuariosSinDialogos(ISeguridadService servicio, IExportDataService exporta, IReportsService reportes)
        : FrmUsuarios(servicio, exporta, reportes)
    {
        public List<string> Avisos { get; } = [];

        protected override void MostrarAviso(string mensaje) => Avisos.Add(mensaje);
    }

    /// <summary>Un solo rol en el catalogo, el que se asigna a todos los usuarios.</summary>
    private static readonly Role[] Catalogo = [new() { RoleId = 1, Nombre = "Admin" }];

    private static FrmUsuariosSinDialogos CrearFormulario(
        out ExportDataServiceStub exporta,
        params Usuario[] usuarios)
    {
        exporta = new ExportDataServiceStub();
        Dictionary<int, List<int>> roles = usuarios.ToDictionary(u => u.UserId, _ => new List<int> { 1 });

        return new FrmUsuariosSinDialogos(
            new SeguridadServiceStub([.. usuarios], [.. Catalogo], roles), exporta, new ReportsServiceStub());
    }

    /// <summary>Usuario con todos los datos puestos, para comprobar el mapeo al Excel.</summary>
    private static Usuario UsuarioParaExcel(int id = 1, bool activo = true) => new()
    {
        UserId = id,
        Username = "user" + id,
        NombreCompleto = "Nombre " + id,
        Email = "user" + id + "@ritrama.com",
        TituloCargo = "Coordinador de ventas",
        Departamento = "Ventas",
        Activo = activo,
        FechaCreacion = new DateTime(2026, 3, 14, 9, 30, 0),
        UltimoLogin = new DateTime(2026, 9, 1, 18, 5, 0),
        PasswordHash = "$2a$11$NO_DEBE_APARECER_EN_LA_HOJA"
    };

    /// <summary>Pulsa el boton Exportar de la barra.</summary>
    private static void ClicEnExportar(FrmUsuarios form)
        => BotonDe(form, "btnExportarUsuario").PerformClick();

    private static ToolStripButton BotonDe(FrmUsuarios form, string nombre)
        => (ToolStripButton)((ToolStrip)form.Controls.Find("barraHerramientas", true).Single())
            .Items[nombre]!;

    // ─────────────────────────────────────────────────────────────────
    // Hoja de Excel con todos los usuarios
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public async Task Exportar_CreaLaHojaConTodosLosUsuariosAunCuandoElListadoEstaFiltrado()
    {
        FrmUsuariosSinDialogos form = CrearFormulario(
            out ExportDataServiceStub exporta,
            UsuarioParaExcel(1), UsuarioParaExcel(2), UsuarioParaExcel(3));
        try
        {
            await form.InitializeAsync();
            exporta.Resultado = true;

            // El buscador deja en pantalla un solo usuario.
            ((Control)form.Controls.Find("txtBuscar", true).Single()).Text = "user1";
            ((DataGridView)form.Controls.Find("gridUsuarios", true).Single()).Rows.Count
                .Should().Be(1, "en pantalla solo sale el filtrado");

            ClicEnExportar(form);

            exporta.Llamadas.Should().Be(1);
            exporta.Fichero.Should().Be("Usuarios.xlsx");
            exporta.Exportado.Should().HaveCount(3,
                "el fichero es el padron completo: filtrar en pantalla no lo recorta");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_LaClaveNoViajaYElEstadoElRolYLasFechasVanResueltos()
    {
        FrmUsuariosSinDialogos form = CrearFormulario(
            out ExportDataServiceStub exporta,
            UsuarioParaExcel(1, activo: false));
        try
        {
            await form.InitializeAsync();

            ClicEnExportar(form);

            UsuarioExportado fila = exporta.Exportado.Single();

            // La contraseña no tiene por donde salir: ni la fila la trae ni el tipo
            // expone una propiedad con ese valor. Esta es la comprobacion que importa.
            typeof(UsuarioExportado).GetProperty("PasswordHash").Should().BeNull(
                "la hoja es un listado de personas, no un volcado de la tabla");
            string.Concat(fila.GetType().GetProperties().Select(p => p.GetValue(fila)))
                .Should().NotContain("$2a$11$");

            fila.Estado.Should().Be("Inactivo", "el mismo texto del grid, no un false");
            fila.Roles.Should().Be("Admin", "el rol ya resuelto a nombre");
            fila.Cargo.Should().Be("Coordinador de ventas");
            fila.Departamento.Should().Be("Ventas");
            fila.FechaCreacion.Should().Be(new DateTime(2026, 3, 14, 9, 30, 0));
            fila.UltimoLogin.Should().Be(new DateTime(2026, 9, 1, 18, 5, 0));
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_AUnUsuarioQueNuncaEntroDejaLaFechDeUltimoAccesoVacia()
    {
        Usuario nuncaEntro = UsuarioParaExcel(1);
        nuncaEntro.UltimoLogin = null;

        FrmUsuariosSinDialogos form = CrearFormulario(out ExportDataServiceStub exporta, nuncaEntro);
        try
        {
            await form.InitializeAsync();

            ClicEnExportar(form);

            // Sin valor no se inventa una fecha: la celda sale vacia, que es como se
            // lee en el detalle.
            exporta.Exportado.Single().UltimoLogin.Should().BeNull();
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_SinUsuariosNoIntentaExportarNiDiceNada()
    {
        FrmUsuariosSinDialogos form = CrearFormulario(out ExportDataServiceStub exporta);
        try
        {
            await form.InitializeAsync();

            ClicEnExportar(form);

            exporta.Llamadas.Should().Be(0, "ExportToExcel lanza si la lista va vacia, asi que no se llama");
            form.Avisos.Should().BeEmpty(
                "no hay filas que exportar: no es un error y en el listado ya se ve, asi que no se avisa");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_NoDependeDePermisos()
    {
        // Sin sesion abierta. El módulo no tiene permisos propios, asi que exportar no
        // pide Usuarios:Ver: antes el padrón entero se quedaba sin exportar y con el
        // aviso "No tiene permiso para ver los usuarios" en pantalla.
        SesionActual.Usuario = null;
        SesionActual.Permisos.Clear();

        FrmUsuariosSinDialogos form = CrearFormulario(out ExportDataServiceStub exporta, UsuarioParaExcel());
        try
        {
            await form.InitializeAsync();

            ClicEnExportar(form);

            exporta.Llamadas.Should().Be(1, "no hay permiso que comprobar antes de exportar");
            form.Avisos.Should().BeEmpty();
        }
        finally
        {
            form.Dispose();

            // La clase entera comparte la sesion estatica: se repone YA aqui y no solo
            // en Dispose, para que el resto de la coleccion no encuentre el hueco.
            SesionActual.Usuario = _usuarioAnterior;
            SesionActual.Permisos.Clear();
            SesionActual.Permisos.AddRange(_permisosAnteriores);
        }
    }

    [Fact]
    public async Task Exportar_SiElServicioFallaSeAvisaYNoSeDaPorHecho()
    {
        FrmUsuariosSinDialogos form = CrearFormulario(out ExportDataServiceStub exporta, UsuarioParaExcel());
        try
        {
            await form.InitializeAsync();
            exporta.ErrorAlExportar = new IOException("el disco esta lleno");

            ClicEnExportar(form);

            form.Avisos.Should().ContainSingle();
            form.Avisos[0].Should().Contain("No se pudo crear la hoja de Excel");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public void Exportar_SeOcultaMientrasSeEscribeElDetalle()
    {
        // Sin Show() "esta visible" y "esta oculto" darian el mismo false y la
        // asercion no probaria nada. TopLevel=false evita abrir una ventana real.
        FrmUsuariosSinDialogos form = CrearFormulario(out _, UsuarioParaExcel());
        try
        {
            form.TopLevel = false;
            form.Show();
            System.Windows.Forms.Application.DoEvents();

            BotonDe(form, "btnExportarUsuario").Visible.Should().BeTrue("en consulta se exporta");

            BotonDe(form, "btnNuevoUsuario").PerformClick();
            System.Windows.Forms.Application.DoEvents();

            BotonDe(form, "btnExportarUsuario").Visible.Should().BeFalse(
                "un clic mientras se captura tiraria el borrador a un fichero que nadie pidio");
        }
        finally
        {
            form.Dispose();
        }
    }
}
