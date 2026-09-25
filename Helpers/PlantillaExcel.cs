using ClosedXML.Excel;

namespace Ritrama2025.Helpers;

/// <summary>Genera una plantilla de Excel vacia (solo encabezados) para que el usuario la llene.</summary>
public static class PlantillaExcel
{
    public static readonly string[] EncabezadosMaster = ["Product Id.", "Product Name", "Roll-Id", "Width [Inch.]", "Length [Pies.]", "Splice", "Fecha Produccion", "Recepcion", "Ubicacion", "Fecha Llegada", "Paleta"];

    public static readonly string[] EncabezadosRollos = ["Product Id.", "Product Name", "Width [Inch.]", "Length [Pies.]", "Msi.", "Codigo Unico", "Splice", "Codigo Personalizado", "Ubicacion"];

    public static void Crear(string rutaArchivo, bool esMaster)
    {
        using XLWorkbook workbook = new XLWorkbook();
        IXLWorksheet hoja = workbook.AddWorksheet("Inventario");
        string[] encabezados = esMaster ? EncabezadosMaster : EncabezadosRollos;
        for (int i = 0; i < encabezados.Length; i++)
        {
            hoja.Cell(1, i + 1).Value = encabezados[i];
        }

        hoja.Row(1).Style.Font.Bold = true;
        hoja.SheetView.FreezeRows(1);
        workbook.SaveAs(rutaArchivo);
    }
}
