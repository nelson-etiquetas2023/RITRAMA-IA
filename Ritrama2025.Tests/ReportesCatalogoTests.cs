using System.Xml.Linq;
using FluentAssertions;
using Microsoft.Reporting.WinForms;
using Ritrama2025;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Valida los tres .rdlc del catalogo (clientes, proveedores, vendedores), que se copian del
/// mismo patron que Report_Productos.rdlc. Lo importante no es que el XML este bien formado,
/// sino que el ReportViewer los acepte y que los nombres de campo cuadren con los alias de
/// R.QUERY: si no cuadran, el visor abre el reporte vacio sin dar ningun error.
/// </summary>
[Trait("Categoria", "Unit")]
public class ReportesCatalogoTests
{
    /// <summary>Espacio de nombres de las definiciones 2016/01 que usan los .rdlc del proyecto.</summary>
    private static readonly XNamespace Ns = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

    [Fact]
    public void ReporteClientes_ElRdlcLoAceptaElVisorYTraeLasSieteColumnas()
        => VerificarRdlc("Customers", "Report_Clientes.rdlc", "DsClientes",
            ["Codigo", "Nombre", "Identificacion", "Telefono", "Email", "Direccion", "Estado"],
            R.QUERY.CUSTOMERS.SQL_QUERY_REPORTE_CLIENTES);

    [Fact]
    public void ReporteProveedores_ElRdlcLoAceptaElVisorYTraeLasSieteColumnas()
        => VerificarRdlc("Providers", "Report_Proveedores.rdlc", "DsProveedores",
            ["Codigo", "Nombre", "Contacto", "Telefono", "Direccion", "Categoria", "Estado"],
            R.QUERY.PROVIDERS.SQL_QUERY_REPORTE_PROVEEDORES);

    [Fact]
    public void ReporteVendedores_ElRdlcLoAceptaElVisorYTraeLasSeisColumnas()
        => VerificarRdlc("Vendedores", "Report_Vendedores.rdlc", "DsVendedores",
            ["Codigo", "Nombre", "Email", "Telefono", "Zona", "Estado"],
            R.QUERY.VENDERS.SQL_QUERY_REPORTE_VENDEDORES);

    /// <summary>
    /// Comprobacion comun del contrato del reporte: existe en la salida, el visor lo carga,
    /// el DataSource se llama como espera ReportsService y las columnas del Tablix cuadran
    /// con los campos, que a su vez son los alias de la consulta.
    /// </summary>
    private static void VerificarRdlc(string carpeta, string fichero, string dataSource,
        string[] campos, string sql)
    {
        // La carga la hace el ReportViewer, no la aplicacion: si el .rdlc no cuadra,
        // LoadReportDefinition lanza y el reporte no abriria nunca.
        string ruta = Path.Combine(AppContext.BaseDirectory, "Reports", carpeta, fichero);
        File.Exists(ruta).Should().BeTrue($"el .rdlc tiene que copiarse a la salida: {ruta}");

        using LocalReport informe = new();
        using FileStream flujo = File.OpenRead(ruta);
        informe.LoadReportDefinition(flujo);

        // El nombre del DataSource es el contrato con ReportsService: si uno de los dos cambia,
        // el visor se queda sin datos sin dar ningun error.
        informe.GetDataSourceNames().Should().Contain(dataSource);

        // Campos y columnas se sacan del XML: la API del visor no los expone.
        XDocument xml = XDocument.Load(ruta);

        xml.Descendants(Ns + "Field").Select(f => f.Attribute("Name")!.Value)
            .Should().Equal(campos);

        // Una celda de cabecera y una de detalle por campo: si no cuadran, el Tablix se rompe
        // al procesar y el error sale en pantalla, no en el codigo.
        xml.Descendants(Ns + "TablixColumn").Count().Should().Be(campos.Length);
        xml.Descendants(Ns + "TablixCell").Count().Should().Be(campos.Length * 2,
            "{0} celdas de cabecera + {0} de detalle", campos.Length);

        // Y cada campo tiene que salir de la consulta con ese alias, o la columna llegaria
        // vacia al visor aunque el .rdlc este bien.
        foreach (string campo in campos)
        {
            sql.Should().Contain($"AS {campo}",
                "la consulta del reporte tiene que exponer {0} con ese alias", campo);
        }
    }
}
