using System.Data;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;

namespace Ritrama2025.Tests.Stubs;

/// <summary>
/// IExportDataService en memoria para las pruebas de FrmUsuarios: guarda lo que se le
/// pasa, para comprobar el contenido de la hoja sin escribir un fichero de verdad ni
/// abrir Excel. Devuelve false por defecto, que es el caso mas proximo a la realidad;
/// da igual para los avisos, porque el formulario no muestra mensaje de exito en
/// ninguno de los dos casos: solo avisa si el servicio lanza.
/// </summary>
internal sealed class ExportDataServiceStub : IExportDataService
{
    /// <summary>Filas de la ultima exportacion (vacias si no se exporto nada).</summary>
    public List<UsuarioExportado> Exportado { get; } = [];

    /// <summary>Numero de veces que se pidio exportar.</summary>
    public int Llamadas { get; private set; }

    /// <summary>Nombre de fichero de la ultima exportacion.</summary>
    public string? Fichero { get; private set; }

    /// <summary>Lo que devuelve la exportacion.</summary>
    public bool Resultado { get; set; }

    /// <summary>Si se pone, el servicio lanza al exportar, para probar el camino de error.</summary>
    public Exception? ErrorAlExportar { get; set; }

    public bool ExportToExcel<T>(List<T> data, string FileName)
    {
        Llamadas++;
        Fichero = FileName;
        Exportado.Clear();
        Exportado.AddRange(data.OfType<UsuarioExportado>());

        if (ErrorAlExportar is not null)
        {
            throw ErrorAlExportar;
        }

        return Resultado;
    }

    public bool ExportToExcelProducts<T>(List<T> data, string FileName)
        => ExportToExcel(data, FileName);

    public bool ExportTxtFormatRollosCortados(DataRow[] rollos, bool solo_rc, string? fecha_produccion, string? fecha_registro, bool openNotePad)
        => true;

    public bool ExportTxtFormatMasterRePrintLabel(ProductMAP master, bool openNotePad)
        => true;
}
