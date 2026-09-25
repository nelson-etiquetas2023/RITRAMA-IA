namespace Ritrama2025.Core;

/// <summary>
/// Reglas puras de categoría de producto (sin dependencia de Models ni UI) para reuso en Core y tests.
/// Los 4 bits son mutuamente excluyentes: exactamente uno debe estar en true.
/// </summary>
public static class ProductCategoryRules
{
    /// <summary>Cuenta cuántas categorías están activas.</summary>
    public static int Count(bool master, bool rolloCortado, bool hoja, bool graphics)
    {
        int c = 0;
        if (master) { c++; }
        if (rolloCortado) { c++; }
        if (hoja) { c++; }
        if (graphics) { c++; }
        return c;
    }

    /// <summary>True si exactamente una categoría está activa.</summary>
    public static bool IsValid(bool master, bool rolloCortado, bool hoja, bool graphics)
        => Count(master, rolloCortado, hoja, graphics) == 1;

    /// <summary>Valida y retorna Result con código para capa de servicio.</summary>
    public static Result Validate(bool master, bool rolloCortado, bool hoja, bool graphics)
    {
        int active = Count(master, rolloCortado, hoja, graphics);
        if (active == 0)
        {
            return Result.Failure("Debe seleccionar exactamente una categoría: Master, Rollo Cortado, Resma o Graphics.", "VALIDATION_CATEGORY");
        }

        if (active > 1)
        {
            return Result.Failure($"Solo una categoría puede estar activa ({active} marcadas). Master, Rollo Cortado, Resma y Graphics son mutuamente excluyentes.", "VALIDATION_CATEGORY");
        }

        return Result.Success();
    }

    /// <summary>Resuelve el nombre de categoría con la misma lógica del CASE de R.cs.</summary>
    public static string GetNombre(bool master, bool rolloCortado, bool hoja, bool graphics)
    {
        if (master) { return "Master"; }
        if (rolloCortado) { return "Rollo Cortado"; }
        if (hoja) { return "Resma"; }
        if (graphics) { return "Graphics"; }
        return string.Empty;
    }
}
