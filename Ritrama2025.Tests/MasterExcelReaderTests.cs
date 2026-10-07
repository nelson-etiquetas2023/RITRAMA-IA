using ClosedXML.Excel;
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.InventarioService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas del lector de la pestaña "Cargar Inventario" (Master): lectura por encabezado
/// con la plantilla descargable, retroceso al orden antiguo por posición y valores por
/// defecto de las celdas en blanco.
/// </summary>
public class MasterExcelReaderTests : IDisposable
{
    private readonly string _tempDir;

    public MasterExcelReaderTests()
    {
        _tempDir = Path.Combine(Path.GetTempPath(), "Ritrama2025_Tests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_tempDir);
    }

    public void Dispose()
    {
        try
        {
            if (Directory.Exists(_tempDir))
            {
                Directory.Delete(_tempDir, recursive: true);
            }
        }
        catch (IOException)
        {
            // El sistema puede retener el fichero: no es fallo de la prueba.
        }
    }

    [Fact]
    public void Leer_ConCabecerasDeLaPlantilla_MapeaLasDoceColumnas()
    {
        string ruta = CrearHoja("plantilla.xlsx", hoja =>
        {
            EscribirFila(hoja, 1, MasterExcelReader.CabecerasPlantilla);
            EscribirFila(hoja, 2,
                "P-001", "Producto Uno", "ROLL-100", 54.5, 1500, 250000, 2,
                new DateTime(2024, 1, 5), "FACT-9", "BOD-3", new DateTime(2024, 2, 10), "PAL-7");
        });

        LecturaMasterResultado lectura = MasterExcelReader.Leer(ruta);

        lectura.Errores.ToString().Should().BeEmpty();
        ProductMAP fila = lectura.Filas.Should().ContainSingle().Subject;
        fila.Product_Id.Should().Be("P-001");
        fila.Product_Name.Should().Be("Producto Uno");
        fila.Rollid.Should().Be("ROLL-100");
        fila.Width.Should().Be(54.5);
        fila.Length.Should().Be(1500);
        fila.Msi.Should().Be(250000);
        fila.Splice.Should().Be(2);
        fila.Fecha_Produccion.Should().Be(new DateTime(2024, 1, 5));
        fila.Factura.Should().Be("FACT-9");
        fila.Ubic.Should().Be("BOD-3");
        fila.Fecha_Llegada.Should().Be(new DateTime(2024, 2, 10));
        fila.Paleta.Should().Be("PAL-7");
    }

    [Fact]
    public void Leer_AceptaLosAliasDeLosEncabezados()
    {
        string ruta = CrearHoja("alias.xlsx", hoja =>
        {
            EscribirFila(hoja, 1,
                "Product ID", "Nombre", "ROLLID", "Width", "Lenght", "MSI",
                "Splice", "Fecha Producción", "Factura", "Ubicación", "Fecha Llegada", "Paleta");
            EscribirFila(hoja, 2, "P-002", "Producto Dos", "ROLL-200", 44, 2000, 100000, 1);
        });

        LecturaMasterResultado lectura = MasterExcelReader.Leer(ruta);

        lectura.Errores.ToString().Should().BeEmpty();
        ProductMAP fila = lectura.Filas.Should().ContainSingle().Subject;
        fila.Product_Id.Should().Be("P-002");
        fila.Rollid.Should().Be("ROLL-200");
        fila.Width.Should().Be(44);
        fila.Length.Should().Be(2000);
        fila.Msi.Should().Be(100000);
        fila.Splice.Should().Be(1);
    }

    [Fact]
    public void Leer_HojaSinEncabezadosReconocibles_UsaElOrdenAntiguo()
    {
        string ruta = CrearHoja("antigua.xlsx", hoja =>
        {
            EscribirFila(hoja, 1,
                "col1", "col2", "col3", "col4", "col5", "col6",
                "col7", "col8", "col9", "col10", "col11");
            EscribirFila(hoja, 2,
                "P-100", "Producto Viejo", "ROLL-9", 44, 2000, 3,
                new DateTime(2024, 3, 1), "FAC-1", "UBI-1", new DateTime(2024, 3, 15), "PAL-1");
        });

        LecturaMasterResultado lectura = MasterExcelReader.Leer(ruta);

        lectura.Errores.ToString().Should().BeEmpty();
        ProductMAP fila = lectura.Filas.Should().ContainSingle().Subject;
        fila.Product_Id.Should().Be("P-100");
        fila.Rollid.Should().Be("ROLL-9");
        fila.Width.Should().Be(44);
        fila.Length.Should().Be(2000);
        fila.Splice.Should().Be(3, "splice sigue en la columna 6 del formato antiguo");
        fila.Fecha_Produccion.Should().Be(new DateTime(2024, 3, 1));
        fila.Factura.Should().Be("FAC-1");
        fila.Ubic.Should().Be("UBI-1");
        fila.Fecha_Llegada.Should().Be(new DateTime(2024, 3, 15));
        fila.Paleta.Should().Be("PAL-1");
        fila.Msi.Should().Be(0, "el formato antiguo no traía columna msi");
    }

    [Fact]
    public void Leer_FilaSinProductId_NoDevuelveFila()
    {
        string ruta = CrearHoja("fila_vacia.xlsx", hoja =>
        {
            EscribirFila(hoja, 1, MasterExcelReader.CabecerasPlantilla);
            EscribirFila(hoja, 2, "P-003", "Producto Tres", "ROLL-300");
            hoja.Cell(3, 2).Value = "Sin codigo";
        });

        LecturaMasterResultado lectura = MasterExcelReader.Leer(ruta);

        lectura.Filas.Should().ContainSingle()
            .Which.Product_Id.Should().Be("P-003", "la fila sin product_id se ignora");
    }

    [Fact]
    public void Leer_CeldasEnBlanco_UsanLosValoresPorDefecto()
    {
        string ruta = CrearHoja("defaults.xlsx", hoja =>
        {
            EscribirFila(hoja, 1, MasterExcelReader.CabecerasPlantilla);
            EscribirFila(hoja, 2, "P-004", "Producto Cuatro", "ROLL-400");
        });

        LecturaMasterResultado lectura = MasterExcelReader.Leer(ruta);

        lectura.Errores.ToString().Should().BeEmpty("las celdas en blanco no son un error");
        ProductMAP fila = lectura.Filas.Should().ContainSingle().Subject;
        fila.Width.Should().Be(0);
        fila.Length.Should().Be(0);
        fila.Msi.Should().Be(0);
        fila.Splice.Should().Be(0);
        fila.Fecha_Produccion.Should().Be(DateTime.Today, "las fechas en blanco usan la fecha actual");
        fila.Fecha_Llegada.Should().Be(DateTime.Today);
        fila.Factura.Should().BeEmpty();
        fila.Ubic.Should().BeEmpty();
        fila.Paleta.Should().BeEmpty();
    }

    [Fact]
    public void Leer_ValorNoNumerico_ReportaElErrorYUsaCero()
    {
        string ruta = CrearHoja("no_numerico.xlsx", hoja =>
        {
            EscribirFila(hoja, 1, MasterExcelReader.CabecerasPlantilla);
            EscribirFila(hoja, 2, "P-005", "Producto Cinco", "ROLL-500", "ANCHO?");
        });

        LecturaMasterResultado lectura = MasterExcelReader.Leer(ruta);

        ProductMAP fila = lectura.Filas.Should().ContainSingle().Subject;
        fila.Width.Should().Be(0);
        lectura.Errores.ToString().Should().Contain("wid").And.Contain("ANCHO?");
    }

    [Fact]
    public void Leer_FaltaLaColumnaProductId_ReportaElErrorYNoLeeFilas()
    {
        string ruta = CrearHoja("sin_product_id.xlsx", hoja =>
        {
            EscribirFila(hoja, 1, "product_name", "rollid", "wid", "length");
            EscribirFila(hoja, 2, "Producto Seis", "ROLL-600", 44, 2000);
        });

        LecturaMasterResultado lectura = MasterExcelReader.Leer(ruta);

        lectura.Filas.Should().BeEmpty("sin product_id no hay forma de identificar la fila");
        lectura.Errores.ToString().Should().Contain("product_id");
    }

    private string CrearHoja(string nombreArchivo, Action<IXLWorksheet> escribir)
    {
        string ruta = Path.Combine(_tempDir, nombreArchivo);
        using XLWorkbook workbook = new();
        IXLWorksheet hoja = workbook.AddWorksheet("Master");
        escribir(hoja);
        workbook.SaveAs(ruta);
        return ruta;
    }

    private static void EscribirFila(IXLWorksheet hoja, int fila, params object[] valores)
    {
        for (int i = 0; i < valores.Length; i++)
        {
            if (valores[i] is null)
            {
                continue;
            }

            XLCellValue valor = valores[i] switch
            {
                string texto => texto,
                DateTime fecha => fecha,
                double numero => numero,
                int entero => (double)entero,
                _ => valores[i].ToString() ?? string.Empty,
            };
            hoja.Cell(fila, i + 1).Value = valor;
        }
    }
}
