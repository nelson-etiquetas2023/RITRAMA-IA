namespace Ritrama2025.Services.ReportsService.ReportsService
{
    public interface IReportsService
    {
        public void Reporte_Orden_Corte(string orden, Form form, string ReportName, string TitleReport);
        public void Reporte_Desperdicios(string orden, Form form, string ReportName, string TitleReport);
        public void Reporte_Orden_MatPrima(string orden, Form form, string ReportName, string TitleReport);
        public void ReporteConduce_conPrecio(string conduce, Form form, string ReportName, string TitleReport);
        public void ReporteCondece_sinPrecio(string conduce, Form form, string ReportName, string TitleReport);
        public void Reporte_PackingList(string conduce, Form form);
        public void Reporte_DetallePaleta(string conduce, Form form);
        public void Reporte_InventarioRollosCortados(Form form, string Report_Title, string Report_Name);
        public void Reporte_InventarioMaster(Form form, string Report_Title, string Report_Name);
    }
}
