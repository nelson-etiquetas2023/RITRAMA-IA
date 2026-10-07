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

    /// <summary>
    /// Longitud máxima del código Ritrama. Vive en product_id (y en MasterInic.part_number),
    /// nvarchar(25) en las dos bases: pasarlo de ahi el INSERT truena contra la columna.
    /// </summary>
    public const int MaxCodigoRitrama = 25;

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

        // El codigo Ritrama ES el product_id: lo teclea el usuario y es unico (clave
        // primaria de producto). Sin valor no hay alta, y con mas de MaxCodigoRitrama
        // caracteres el INSERT truena contra la columna nvarchar(25).
        if (string.IsNullOrWhiteSpace(producto.Product_id))
        {
            return Result.Failure("El código Ritrama es obligatorio.", CODE_REQUIRED);
        }

        if (producto.Product_id.Trim().Length > MaxCodigoRitrama)
        {
            return Result.Failure($"El código Ritrama no puede exceder {MaxCodigoRitrama} caracteres.", CODE_REQUIRED);
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

        if (producto.Costo < 0)
        {
            return Result.Failure("El costo no puede ser negativo.", CODE_REQUIRED);
        }

        if (producto.Ratio < 0)
        {
            return Result.Failure("El ratio no puede ser negativo.", CODE_REQUIRED);
        }

        return Result.Success();
    }

    /// <summary>
    /// Regla de estado para Update. Un producto vigente se edita siempre, y se puede desactivar
    /// o reactivar desde el propio formulario. Un producto anulado en BD solo admite el cambio
    /// si la operacion lo reactiva (Anulado = false): editar los datos de un producto dado de
    /// baja sin devolverlo a la vida no tiene sentido, y reactivarlo si, porque es la unica
    /// forma de volver a tocar sus datos. Mismo criterio que SQL_ANULAR_PEDIDO y
    /// SQL_RESTAURAR_PEDIDO para el modulo de pedidos.
    /// </summary>
    /// <param name="isAnuladoInDb">Estado de la columna anulado leido de la base.</param>
    /// <param name="seReactivara">True si la operacion deja el producto vigente (Anulado = false).</param>
    /// <param name="productId">Codigo del producto, solo para el mensaje de error.</param>
    public static Result ValidateEditableState(bool isAnuladoInDb, bool seReactivara, string productId)
    {
        if (isAnuladoInDb && !seReactivara)
        {
            return Result.Failure(
                $"El producto '{productId}' está anulado. Actívelo para poder guardar los cambios.",
                CODE_ANULADO);
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
