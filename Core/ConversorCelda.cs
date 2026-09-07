namespace Ritrama2025.Core;

public static class ConversorCelda
{
    public static int ToInt(object? value)
        => int.TryParse(value?.ToString(), out int i) ? i : 0;

    public static double ToDouble(object? value)
        => double.TryParse(value?.ToString(), out double d) ? d : 0;

    public static string ToString(object? value)
        => value?.ToString() ?? string.Empty;
}