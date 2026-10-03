using Ritrama2025.Services.ReportsService.ReportsService;

namespace Ritrama2025.Tests.Stubs;

/// <summary>
/// IReportsService en memoria para las pruebas de FrmUsuarios. Solo
/// <see cref="Reporte_Usuarios"/> deja rastro: guarda lo que se le pasa para comprobar
/// que el boton Reporte pide el titulo y el .rdlc correctos, sin abrir el visor ni
/// pegarle a la base. El resto de reportes estan vacios porque esta pantalla no los usa.
/// </summary>
public sealed class ReportsServiceStub : IReportsService
{
    /// <summary>Numero de veces que se pidio el reporte de usuarios.</summary>
    public int LlamadasReporteUsuarios { get; private set; }

    /// <summary>Formulario con el que se abrio el reporte (null si nunca se abrio).</summary>
    public Form? Formulario { get; private set; }

    /// <summary>Titulo del ultimo reporte pedido.</summary>
    public string? Titulo { get; private set; }

    /// <summary>Nombre del .rdlc del ultimo reporte pedido.</summary>
    public string? NombreReporte { get; private set; }

    /// <summary>Si se pone, Reporte_Usuarios lanza, para probar el camino de error.</summary>
    public Exception? ErrorAlAbrir { get; set; }

    public void Reporte_Usuarios(Form form, string Report_Title, string Report_Name)
    {
        LlamadasReporteUsuarios++;
        Formulario = form;
        Titulo = Report_Title;
        NombreReporte = Report_Name;

        if (ErrorAlAbrir is not null)
        {
            throw ErrorAlAbrir;
        }
    }

    public void Reporte_Productos(Form form, string Report_Title, string Report_Name) { }

    public void Reporte_Clientes(Form form, string Report_Title, string Report_Name) { }

    public void Reporte_Proveedores(Form form, string Report_Title, string Report_Name) { }

    public void Reporte_Vendedores(Form form, string Report_Title, string Report_Name) { }

    public void Reporte_Orden_Corte(string orden, Form form, string ReportName, string TitleReport) { }

    public void Reporte_Desperdicios(string orden, Form form, string ReportName, string TitleReport) { }

    public void Reporte_Orden_MatPrima(string orden, Form form, string ReportName, string TitleReport) { }

    public void ReporteConduce_conPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

    public void ReporteCondece_sinPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

    public void Reporte_PackingList(string conduce, Form form) { }

    public void Reporte_DetallePaleta(string conduce, Form form) { }

    public void Reporte_InventarioRollosCortados(Form form, string Report_Title, string Report_Name) { }

    public void Reporte_InventarioMaster(Form form, string Report_Title, string Report_Name) { }
}
