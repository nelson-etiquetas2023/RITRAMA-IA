using ClosedXML.Excel;

namespace Ritrama2025.Helpers;

/// <summary>
/// Abre un archivo Excel copiandolo a memoria con FileShare.ReadWrite, de modo que
/// pueda leerse aunque el archivo este abierto en Excel u otro programa. Si el handle
/// no se puede obtener, se lanza IOException (el llamador decide el mensaje al usuario).
/// </summary>
public static class ExcelWorkbook
{
    public static XLWorkbook Abrir(string rutaArchivo)
    {
        using FileStream fs = new FileStream(rutaArchivo, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
        using MemoryStream ms = new MemoryStream();
        fs.CopyTo(ms);
        ms.Position = 0;
        return new XLWorkbook(ms);
    }
}
