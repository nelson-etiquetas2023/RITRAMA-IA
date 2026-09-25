using Ritrama2025.Core;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProductsService;

/// <summary>
/// Validaciones de dominio para <see cref="Product"/> centralizadas para reuso y testabilidad sin I/O.
/// Reglas:
/// - Product_id y Product_Name son obligatorios.
/// - Exactamente una categoría entre Master, RolloCortado, Hoja (Resmas) y Graphics debe estar en true (bits exclusivos).
/// Referencia: R.cs SELECT_QUERY_PRODUCTS resuelve el tipo con CASE WHEN MasterRolls=1 then 'Master' ...
/// Ubicado en Services para no crear dependencia circular Core → Models; la lógica pura podría migrarse a Core
/// si Product se mueve a Core en el futuro.
/// </summary>
internal static class ProductValidator
{
    public const string CODE_CATEGORY = "VALIDATION_CATEGORY";
    public const string CODE_REQUIRED = "VALIDATION_REQUIRED";
    public const string CODE_ANULADO = "VALIDATION_ANULADO";

    /// <summary>Cuenta cuántas categorías están activas. Delega a <see cref="ProductCategoryRules"/> (Core).</summary>
    public static int CountActiveCategories(Product p)
    {
        ArgumentNullException.ThrowIfNull(p);
        return ProductCategoryRules.Count(p.Master, p.RolloCortado, p.Hoja, p.Graphics);
    }

    /// <summary>Valida que exactamente una categoría esté activa.</summary>
    public static bool IsCategoryValid(Product p) => ProductCategoryRules.IsValid(p.Master, p.RolloCortado, p.Hoja, p.Graphics);

    /// <summary>
    /// Valida un producto para Add/Update. Retorna Result con códigos para UI.
    /// No toca BD; valida solo invariants de dominio.
    /// </summary>
    public static Result ValidateForPersistence(Product producto)
    {
        ArgumentNullException.ThrowIfNull(producto);

        if (string.IsNullOrWhiteSpace(producto.Product_id))
        {
            return Result.Failure("El código del producto (Product_id) es obligatorio.", CODE_REQUIRED);
        }

        if (string.IsNullOrWhiteSpace(producto.Product_Name))
        {
            return Result.Failure("El nombre del producto (Product_Name) es obligatorio.", CODE_REQUIRED);
        }

        Result catResult = ProductCategoryRules.Validate(producto.Master, producto.RolloCortado, producto.Hoja, producto.Graphics);
        if (!catResult.IsSuccess)
        {
            return Result.Failure(catResult.Error!, CODE_CATEGORY);
        }

        if (producto.Precio < 0)
        {
            return Result.Failure("El precio no puede ser negativo.", CODE_REQUIRED);
        }

        if (producto.Ratio < 0)
        {
            return Result.Failure("El ratio no puede ser negativo.", CODE_REQUIRED);
        }

        return Result.Success();
    }

    /// <summary>
    /// Valida que un producto anulado no se edite. Para Update: si el registro en BD está anulado, rechaza.
    /// </summary>
    public static Result ValidateNotAnulado(bool isAnuladoInDb, string productId)
    {
        if (isAnuladoInDb)
        {
            return Result.Failure($"El producto '{productId}' está anulado y no se puede editar.", CODE_ANULADO);
        }

        return Result.Success();
    }

    /// <summary>Obtiene el nombre de categoría resuelto (misma lógica que el CASE de R.cs).</summary>
    public static string GetCategoriaNombre(Product p)
    {
        ArgumentNullException.ThrowIfNull(p);
        return ProductCategoryRules.GetNombre(p.Master, p.RolloCortado, p.Hoja, p.Graphics);
    }
}
