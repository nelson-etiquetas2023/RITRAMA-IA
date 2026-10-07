using ClosedXML.Excel;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Services.InventarioService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de la plantilla de Excel descargable de la pestaña "Cargar Inventario" (Master).
/// </summary>
public class InventarioServicePlantillaTests : IDisposable
{
    private readonly InventarioService _service;
    private readonly string _tempDir;

    public InventarioServicePlantillaTests()
    {
        // CrearPlantillaMaster no toca la base de datos: basta la configuración.
        IConfiguration options = TestConfiguration.BuildServiceOptions();
        _service = new InventarioService(options);

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
    public void CrearPlantillaMaster_EscribeLasCabecerasSinFilasDeDatos()
    {
        string ruta = Path.Combine(_tempDir, "plantilla_master.xlsx");

        Result resultado = _service.CrearPlantillaMaster(ruta);

        resultado.IsSuccess.Should().BeTrue(resultado.Error);
        File.Exists(ruta).Should().BeTrue("la plantilla debe crearse en la ruta indicada");

        using XLWorkbook workbook = new(ruta);
        IXLWorksheet hoja = workbook.Worksheet(1);
        for (int i = 0; i < MasterExcelReader.CabecerasPlantilla.Length; i++)
        {
            hoja.Cell(1, i + 1).GetString().Should().Be(MasterExcelReader.CabecerasPlantilla[i]);
        }
        hoja.LastRowUsed()!.RowNumber().Should().Be(1, "la plantilla se descarga sin filas de datos");
        hoja.Cell(1, 1).Style.Font.Bold.Should().BeTrue("los encabezados van en negrita");
        hoja.SheetView.SplitRow.Should().Be(1, "la fila de encabezados queda congelada");
    }

    [Fact]
    public void CrearPlantillaMaster_RutaInvalida_FallaSinExcepcion()
    {
        Result resultado = _service.CrearPlantillaMaster("   ");

        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNullOrWhiteSpace();
    }
}
