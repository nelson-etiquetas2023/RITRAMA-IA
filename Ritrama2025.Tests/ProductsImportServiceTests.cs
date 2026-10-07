using ClosedXML.Excel;
using FluentAssertions;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProductsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas del importador de productos: la plantilla descargable y la lectura de hojas
/// escritas con las cabeceras de esa plantilla (product_id / product_name / categoria).
/// La lectura consulta los códigos Ritrama existentes contra el SQL Server de dev,
/// de modo que se ejecuta como prueba de integración.
/// </summary>
[Trait("Categoria", "Integracion")]
public class ProductsImportServiceTests : IDisposable
{
    private readonly ProductsImportService _service;
    private readonly string _tempDir;

    public ProductsImportServiceTests()
    {
        // LeerYValidarExcel solo usa la cadena de conexión; las dependencias de inserción
        // no se tocan en estas pruebas (nada se inserta), así que basta un stub.
        _service = new ProductsImportService(
            new ProductsServiceStub(),
            new ConsecutivosServiceStub(),
            TestConfiguration.BuildServiceConfiguration());

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
            // El temporizador del sistema puede retener el fichero: no es fallo de la prueba.
        }
    }

    [Fact]
    public void CrearPlantilla_EscribeLasTresCabecerasDeLaPlantilla()
    {
        string ruta = Path.Combine(_tempDir, "plantilla.xlsx");

        Result resultado = _service.CrearPlantilla(ruta);

        resultado.IsSuccess.Should().BeTrue(resultado.Error);
        File.Exists(ruta).Should().BeTrue("la plantilla debe crearse en la ruta indicada");

        using XLWorkbook workbook = new(ruta);
        IXLWorksheet hoja = workbook.Worksheet(1);
        hoja.Cell(1, 1).GetString().Should().Be("product_id");
        hoja.Cell(1, 2).GetString().Should().Be("product_name");
        hoja.Cell(1, 3).GetString().Should().Be("categoria");
        hoja.LastRowUsed().RowNumber().Should().Be(1, "la plantilla se descarga sin filas de datos");
    }

    [Fact]
    public void CrearPlantilla_RutaVacia_FallaSinExcepcion()
    {
        Result resultado = _service.CrearPlantilla("   ");

        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().NotBeNullOrWhiteSpace();
    }

    [Fact]
    public void LeerYValidarExcel_AceptaLasCabecerasDeLaPlantilla()
    {
        string ruta = RellenarPlantillaEjemplo("Master");

        Result<List<ProductoImportFila>> lectura = _service.LeerYValidarExcel(ruta);

        lectura.IsSuccess.Should().BeTrue(lectura.Error);
        ProductoImportFila fila = lectura.Value.Should().ContainSingle().Subject;
        fila.CodigoRitrama.Should().Be("TST-PLANTILLA-001");
        fila.Nombre.Should().Be("Producto de prueba");
        fila.Tipo.Should().Be("Master");
        fila.EsValida.Should().BeTrue(fila.Error);
    }

    [Fact]
    public void LeerYValidarExcel_RechazaLaCategoriaDesconocida()
    {
        string ruta = RellenarPlantillaEjemplo("Volante");

        Result<List<ProductoImportFila>> lectura = _service.LeerYValidarExcel(ruta);

        lectura.IsSuccess.Should().BeTrue(lectura.Error);
        ProductoImportFila fila = lectura.Value.Should().ContainSingle().Subject;
        fila.EsValida.Should().BeFalse("la categoria 'Volante' no es un tipo de producto valido");
        fila.Error.Should().Contain("Master, Rollo Cortado, Resma o Graphics");
    }

    [Fact]
    public void LeerYValidarExcel_FallaSiFaltaUnaColumnaDeLaPlantilla()
    {
        string ruta = Path.Combine(_tempDir, "sin_categoria.xlsx");
        using (XLWorkbook workbook = new())
        {
            IXLWorksheet hoja = workbook.AddWorksheet("Productos");
            hoja.Cell(1, 1).Value = "product_id";
            hoja.Cell(1, 2).Value = "product_name";
            hoja.Cell(2, 1).Value = "OTRO-001";
            hoja.Cell(2, 2).Value = "Sin categoria";
            workbook.SaveAs(ruta);
        }

        Result<List<ProductoImportFila>> lectura = _service.LeerYValidarExcel(ruta);

        lectura.IsSuccess.Should().BeFalse();
        lectura.Error.Should().Contain("categoria");
    }

    [Fact]
    public async Task ImportarFilasAsync_ElCodigoDeLaHojaEsElProduct_IDyElConsecutivoVaEnIdConsec()
    {
        // Convención vigente: el código que trae la hoja ES la clave del producto
        // (product_id), y el contador PROD solo rellena el consecutivo referencial
        // (IdConsec). Si se invirtieran, el inventario dejaría de cruzar por el código.
        GrabadorProductos grabador = new();
        ProductsImportService servicio = new(
            grabador,
            new ConsecutivosDePrueba([41, 42]),
            TestConfiguration.BuildServiceConfiguration());

        List<ProductoImportFila> filas =
        [
            new ProductoImportFila { FilaExcel = 2, CodigoRitrama = " 00753 ", Nombre = "00753-500", Tipo = "Master" },
            new ProductoImportFila { FilaExcel = 3, CodigoRitrama = "01874", Nombre = "01874 Gris", Tipo = "Rollo Cortado" }
        ];

        Result<ProductImportResumen> resumen = await servicio.ImportarFilasAsync(filas);

        resumen.IsSuccess.Should().BeTrue(resumen.Error);
        resumen.Value!.Insertadas.Should().Be(2);
        grabador.Agregados.Select(p => p.Product_id).Should().Equal(
            ["00753", "01874"],
            "el código tecleado manda como clave, recortado de espacios");
        grabador.Agregados.Select(p => p.IdConsec).Should().Equal(
            [41, 42],
            "el consecutivo lo pone el contador PROD y vive en su propia columna");
    }

    /// <summary>Crea una plantilla con una fila y el tipo indicado; devuelve la ruta.</summary>
    private string RellenarPlantillaEjemplo(string categoria)
    {
        string ruta = Path.Combine(_tempDir, $"plantilla_{Guid.NewGuid():N}.xlsx");
        _service.CrearPlantilla(ruta).IsSuccess.Should().BeTrue("la plantilla base debe poder crearse");

        using XLWorkbook workbook = new(ruta);
        IXLWorksheet hoja = workbook.Worksheet(1);
        hoja.Cell(2, 1).Value = "TST-PLANTILLA-001";
        hoja.Cell(2, 2).Value = "Producto de prueba";
        hoja.Cell(2, 3).Value = categoria;
        workbook.Save();

        return ruta;
    }

    /// <summary>IProductsService no usado por la lectura; solo hace falta no nulo.</summary>
    private sealed class ProductsServiceStub : IProductsService
    {
        public Task<System.Data.DataSet> Load(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<bool> Add(Product producto) => throw new NotImplementedException();

        public Task<bool> Update(Product producto) => throw new NotImplementedException();

        public bool Anular(string IdProduct) => throw new NotImplementedException();

        public bool ValidProductid(string id) => throw new NotImplementedException();

        public Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Result ValidateProduct(Product producto) => throw new NotImplementedException();
    }

    /// <summary>IConsecutivosService no usado por la lectura; solo hace falta no nulo.</summary>
    private sealed class ConsecutivosServiceStub : IConsecutivosService
    {
        public int GetAndIncrementConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecOCTransactional(
            Microsoft.Data.SqlClient.SqlConnection conn,
            Microsoft.Data.SqlClient.SqlTransaction transaction) => throw new NotImplementedException();

        public int BuscarUniqueCodeConsec() => throw new NotImplementedException();

        public int BuscarConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecProducto() => throw new NotImplementedException();

        public int GetAndIncrementConsecCliente() => throw new NotImplementedException();

        public int GetAndIncrementConsecProveedor() => throw new NotImplementedException();

        public int GetAndIncrementConsecVendedor() => throw new NotImplementedException();

        public bool UpdateConsecOC(string consec) => throw new NotImplementedException();

        public bool UpdateUniqueCodeBD(string uniquecode) => throw new NotImplementedException();
    }

    /// <summary>Captura los productos que el importador da de alta, sin tocar la base.</summary>
    private sealed class GrabadorProductos : IProductsService
    {
        public List<Product> Agregados { get; } = [];

        public Task<System.Data.DataSet> Load(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<bool> Add(Product producto) => throw new NotImplementedException();

        public Task<bool> Update(Product producto) => throw new NotImplementedException();

        public bool Anular(string IdProduct) => throw new NotImplementedException();

        public bool ValidProductid(string id) => throw new NotImplementedException();

        public Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default)
        {
            Agregados.Add(producto);
            return Task.FromResult(Result<bool>.Success(true));
        }

        public Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default)
            => throw new NotImplementedException();

        public Result ValidateProduct(Product producto) => throw new NotImplementedException();
    }

    /// <summary>Contador que entrega la cola de números indicada (41, 42, ...).</summary>
    private sealed class ConsecutivosDePrueba(int[] valores) : IConsecutivosService
    {
        private readonly Queue<int> _valores = new(valores);

        public int GetAndIncrementConsecProducto() => _valores.Dequeue();

        public int GetAndIncrementConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecOCTransactional(
            Microsoft.Data.SqlClient.SqlConnection conn,
            Microsoft.Data.SqlClient.SqlTransaction transaction) => throw new NotImplementedException();

        public int BuscarUniqueCodeConsec() => throw new NotImplementedException();

        public int BuscarConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecCliente() => throw new NotImplementedException();

        public int GetAndIncrementConsecProveedor() => throw new NotImplementedException();

        public int GetAndIncrementConsecVendedor() => throw new NotImplementedException();

        public bool UpdateConsecOC(string consec) => throw new NotImplementedException();

        public bool UpdateUniqueCodeBD(string uniquecode) => throw new NotImplementedException();
    }
}
