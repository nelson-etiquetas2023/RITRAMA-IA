using Ritrama2025.Core;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProductsService;

/// <summary>
/// Resultado de una importación de productos desde Excel: cuántas filas entraron y
/// el motivo de las que no entraron.
/// </summary>
public sealed class ProductImportResumen
{
    public int Insertadas { get; set; }

    public List<string> Fallos { get; set; } = [];
}

/// <summary>
/// Importación masiva del catálogo de productos desde una hoja de Excel.
/// Plantilla mínima: product_id (el código Ritrama único que teclea el usuario), Nombre y
/// Tipo; el resto por defecto y el consecutivo PROD lo guarda el sistema en IdConsec.
/// </summary>
public interface IProductsImportService
{
    /// <summary>
    /// Lee la primera hoja del fichero y valida cada fila (obligatorios, tipo válido,
    /// código Ritrama único en la hoja y en la base). No inserta nada.
    /// </summary>
    Result<List<ProductoImportFila>> LeerYValidarExcel(string pathFileName);

    /// <summary>
    /// Escribe la plantilla de importación en la ruta indicada: hoja "Productos" con las
    /// cabeceras product_id, product_name y categoria. El usuario la rellena y la vuelve a
    /// cargar con <see cref="LeerYValidarExcel"/>.
    /// </summary>
    Result CrearPlantilla(string pathFileName);

    /// <summary>
    /// Inserta las filas válidas una a una con la misma validación de dominio del alta
    /// manual (AddValidatedAsync) y el consecutivo del sistema. Las filas inválidas se
    /// omiten y se reportan en el resumen.
    /// </summary>
    Task<Result<ProductImportResumen>> ImportarFilasAsync(
        IReadOnlyList<ProductoImportFila> filasValidas,
        CancellationToken cancellationToken = default);
}
