using System.Xml.Linq;
using FluentAssertions;
using Microsoft.Reporting.WinForms;
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
/// Boton Reporte de FrmUsuarios: abre el padron completo de usuarios en el visor de
/// reportes. Copia el contrato del boton de FrmProductos — permiso Usuarios:Ver antes
/// de tocar el servicio, y si el visor falla se avisa por ServiceErrors y en pantalla —
/// con el stub del visor en memoria, porque abrir una ventana modal dentro de un test
/// lo dejaria colgado. Vive en un fichero propio para no mezclarlo con la maquetacion.
/// </summary>
[Trait("Categoria", "Unit")]
[Collection("Usuarios")]
public class FrmUsuariosReporteTests : IDisposable
{
    private readonly Usuario? _usuarioAnterior = SesionActual.Usuario;
    private readonly List<string> _permisosAnteriores = [.. SesionActual.Permisos];

    /// <summary>
    /// Sesion con permiso de ver, que es el que pide el boton. Sin esto el clic no
    /// llega al servicio. Se restaura al terminar, igual que en las pruebas de
    /// Exportar y de Alta/Edicion.
    /// </summary>
    public FrmUsuariosReporteTests()
    {
        SesionActual.Usuario = new Usuario { UserId = 1, Username = "prueba" };
        SesionActual.Permisos.AddRange(["Usuarios:Ver", "Usuarios:Crear", "Usuarios:Editar"]);
    }

    public void Dispose()
    {
        SesionActual.Usuario = _usuarioAnterior;
        SesionActual.Permisos.Clear();
        SesionActual.Permisos.AddRange(_permisosAnteriores);
    }

    /// <summary>
    /// Formulario con los dialogos sustituidos por capturas. Cancelar ya no pregunta
    /// antes de descartar, asi que no hace falta sustituir ninguna confirmacion.
    /// </summary>
    private sealed class FrmUsuariosSinDialogos(
        ISeguridadService servicio, IExportDataService exporta, IReportsService reportes)
        : FrmUsuarios(servicio, exporta, reportes)
    {
        public List<string> Avisos { get; } = [];

        protected override void MostrarAviso(string mensaje) => Avisos.Add(mensaje);
    }

    private static FrmUsuariosSinDialogos CrearFormulario(out ReportsServiceStub reportes)
    {
        reportes = new ReportsServiceStub();
        return new FrmUsuariosSinDialogos(
            new SeguridadServiceStub(), new ExportDataServiceStub(), reportes);
    }

    private static ToolStripButton BotonDe(FrmUsuarios form, string nombre)
        => (ToolStripButton)((ToolStrip)form.Controls.Find("barraHerramientas", true).Single())
            .Items[nombre]!;

    /// <summary>
    /// Muestra el formulario sin abrir una ventana. <see cref="Control.Visible"/> de un
    /// ToolStripItem depende de que la barra este visible, asi que sin Show() "esta
    /// visible" y "esta oculto" darian el mismo false y no se probaria nada.
    /// </summary>
    private static void Mostrar(FrmUsuarios form)
    {
        form.TopLevel = false;
        form.Show();
        System.Windows.Forms.Application.DoEvents();
    }

    private static void Clic(FrmUsuarios form, string nombre)
    {
        BotonDe(form, nombre).PerformClick();
        System.Windows.Forms.Application.DoEvents();
    }

    // ─────────────────────────────────────────────────────────────────
    // El boton en la barra
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void ElBotonReporteEstaEnLaBarraConSuIconoYSuAyuda()
    {
        using FrmUsuariosSinDialogos form = CrearFormulario(out ReportsServiceStub reportes);

        ToolStripButton boton = BotonDe(form, "btnReporteUsuario");

        boton.Name.Should().Be("btnReporteUsuario",
            "el nombre es el contrato con el diseniador y con las pruebas de la barra");
        boton.Text.Should().Be("Reporte", "es el rotulo que se lee junto a Exportar");
        boton.ToolTipText.Should().Be("Ver el catalogo de usuarios en el visor de reportes");
        boton.Image.Should().NotBeNull("sin icono el boton se lee como una accion mas de texto");
        reportes.LlamadasReporteUsuarios.Should().Be(0, "nada mas crear el formulario no se abre nada");
    }

    [Fact]
    public void ElBotonReporteSeOcultaAlEscribirElDetalleYVuelveAlCancelar()
    {
        using FrmUsuariosSinDialogos form = CrearFormulario(out _);
        Mostrar(form);

        BotonDe(form, "btnReporteUsuario").Visible.Should().BeTrue(
            "en consulta el padron de la base es el de pantalla, asi que se puede ver");

        Clic(form, "btnNuevoUsuario");

        BotonDe(form, "btnReporteUsuario").Visible.Should().BeFalse(
            "mientras se captura, el listado todavia no refleja lo que se esta escribiendo");

        Clic(form, "btnCancelarUsuario");

        BotonDe(form, "btnReporteUsuario").Visible.Should().BeTrue(
            "al volver a consulta vuelve a estar disponible, junto a Exportar");
    }

    // ─────────────────────────────────────────────────────────────────
    // Permisos y llamada al servicio
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void ConPermisoDeVerElClicPideElCatalogoConSuTituloYSuFichero()
    {
        using FrmUsuariosSinDialogos form = CrearFormulario(out ReportsServiceStub reportes);
        Mostrar(form);

        Clic(form, "btnReporteUsuario");

        reportes.LlamadasReporteUsuarios.Should().Be(1,
            "con Usuarios:Ver el visor se abre una sola vez");
        reportes.Formulario.Should().BeSameAs(form, "el reporte se embebe en este formulario");
        reportes.Titulo.Should().Be("Catalogo de Usuarios");
        reportes.NombreReporte.Should().Be("Report_Usuarios.rdlc");
        form.Avisos.Should().BeEmpty("no hay nada que avisar cuando el visor se abre bien");
    }

    [Fact]
    public void SinPermisoDeVerElClicNoTocaElServicioYElAvisoExplicaQueFaltaVer()
    {
        FrmUsuariosSinDialogos form = CrearFormulario(out ReportsServiceStub reportes);
        Mostrar(form);
        try
        {
            // Sin Usuarios:Ver. El usuario sigue en sesion, para que el fallo sea el
            // permiso que falta y no la falta de sesion.
            SesionActual.Permisos.Clear();

            Clic(form, "btnReporteUsuario");

            reportes.LlamadasReporteUsuarios.Should().Be(0,
                "sin permiso de ver no se llega a tocar el servicio de reportes");
            form.Avisos.Should().ContainSingle();
            form.Avisos[0].Should().Be("No tiene permiso para ver los usuarios.");
        }
        finally
        {
            form.Dispose();

            // La coleccion entera comparte la sesion estatica: se repone YA aqui y no
            // solo en Dispose, para que el resto no encuentre el hueco.
            SesionActual.Usuario = _usuarioAnterior;
            SesionActual.Permisos.Clear();
            SesionActual.Permisos.AddRange(_permisosAnteriores);
        }
    }

    [Fact]
    public void SiElVisorFallaElAvisoDiceQueNoSePudoAbrirElReporte()
    {
        using FrmUsuariosSinDialogos form = CrearFormulario(out ReportsServiceStub reportes);
        Mostrar(form);
        reportes.ErrorAlAbrir = new IOException("el visor no arranca");

        Clic(form, "btnReporteUsuario");

        reportes.LlamadasReporteUsuarios.Should().Be(1, "el intento se hizo, fallo al abrir");
        form.Avisos.Should().ContainSingle("el fallo tiene que llegar al usuario");
        form.Avisos[0].Should().Contain("No se pudo abrir el reporte");
        form.Avisos[0].Should().Contain("el visor no arranca", "el motivo del fallo se arrastra al aviso");
    }

    // ─────────────────────────────────────────────────────────────────
    // El .rdlc
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void ElRdlcDeUsuariosLoAceptaElVisorYTraeLosOchoCampos()
    {
        // Esta es la comprobacion que de verdad importa: la carga la hace el ReportViewer,
        // no la aplicacion. Si el .rdlc no cuadra, LoadReportDefinition lanza y el reporte
        // no abriria nunca, aunque el XML estuviese bien formado.
        string ruta = Path.Combine(AppContext.BaseDirectory, "Reports", "Usuarios", "Report_Usuarios.rdlc");
        File.Exists(ruta).Should().BeTrue($"el .rdlc tiene que copiarse a la salida: {ruta}");

        using LocalReport informe = new();
        using FileStream flujo = File.OpenRead(ruta);
        informe.LoadReportDefinition(flujo);

        // El nombre del DataSource es el contrato con ReportsService.Reporte_Usuarios: si
        // uno de los dos cambia, el visor abre el reporte vacio sin dar ningun error.
        informe.GetDataSourceNames().Should().Contain("DsUsuarios");

        // Los campos se sacan del XML: la API del visor no los expone. Ocho, los que
        // pinta la tabla del padron de usuarios.
        XDocument xml = XDocument.Load(ruta);
        XNamespace r = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

        xml.Descendants(r + "Field").Should().HaveCount(8,
            "Codigo, Usuario, Nombre, Email, Cargo, Departamento, Rol y Estado");
    }
}
