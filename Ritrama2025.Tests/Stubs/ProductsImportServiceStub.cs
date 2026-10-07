using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProductsService;

namespace Ritrama2025.Tests.Stubs;

/// <summary>
/// IProductsImportService en memoria para las pruebas de FrmProductos: no lee ficheros
/// ni toca la base. Registrar las lecturas permite comprobar que el boton Importar abre
/// el importador con el servicio que toca, sin crear hojas de Excel de verdad.
/// </summary>
internal sealed class ProductsImportServiceStub : IProductsImportService
{
    /// <summary>Veces que se pidio leer una hoja.</summary>
    public int Lecturas { get; private set; }

    /// <summary>Fichero que se pidio leer en la ultima llamada.</summary>
    public string? UltimoFichero { get; private set; }

    /// <summary>Resultado que devolvera la proxima lectura. Vacio = hoja sin filas validas.</summary>
    public Result<List<ProductoImportFila>> ResultadoLectura { get; set; }
        = Result<List<ProductoImportFila>>.Success([]);

    /// <summary>Resumen que devolvera la importacion.</summary>
    public Result<ProductImportResumen> ResultadoImport { get; set; }
        = Result<ProductImportResumen>.Success(new ProductImportResumen());

    /// <summary>Veces que se pidio crear la plantilla.</summary>
    public int Plantillas { get; private set; }

    /// <summary>Resultado que devolvera la creacion de la plantilla.</summary>
    public Result ResultadoPlantilla { get; set; } = Result.Success();

    public Result<List<ProductoImportFila>> LeerYValidarExcel(string pathFileName)
    {
        Lecturas++;
        UltimoFichero = pathFileName;
        return ResultadoLectura;
    }

    public Result CrearPlantilla(string pathFileName)
    {
        Plantillas++;
        return ResultadoPlantilla;
    }

    public Task<Result<ProductImportResumen>> ImportarFilasAsync(
        IReadOnlyList<ProductoImportFila> filasValidas,
        CancellationToken cancellationToken = default)
        => Task.FromResult(ResultadoImport);
}
