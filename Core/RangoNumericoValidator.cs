namespace Ritrama2025.Core;

public static class RangoNumericoValidator
{
    public const decimal MaximoNumeric = 9999999999999999.99m;

    public static string? Verificar(string campo, decimal valor)
    {
        if (valor > MaximoNumeric || valor < -MaximoNumeric)
        {
            return $"el campo '{campo}' tiene un valor fuera de rango: {valor} (maximo permitido {MaximoNumeric}). Revise los datos del despacho.";
        }

        return null;
    }

    public static string? PrimeraFueraDeRango(IEnumerable<(string Campo, decimal Valor)> valores)
    {
        foreach ((string Campo, decimal Valor) valor in valores)
        {
            string? resultado = Verificar(valor.Campo, valor.Valor);
            if (resultado != null)
            {
                return resultado;
            }
        }

        return null;
    }
}
