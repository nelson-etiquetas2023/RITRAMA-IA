using System.Linq;

namespace Ritrama2025.Core;

public static class CalculosDespacho
{
    public const decimal PORC_ITBIS = 18.00m;
    public const decimal CONST_MSI = 83.33333333333333m;
    public const decimal CONST_PIE_LINEALES = 0.012m;

    public static decimal CalcularItbis(decimal subtotal, decimal porcItbis = PORC_ITBIS)
        => Math.Round(subtotal * porcItbis / 100m, 2, MidpointRounding.AwayFromZero);

    public static decimal CalcularTotal(decimal subtotal, decimal itbis)
        => subtotal + itbis;

    public static decimal CalcularMsiRenglon(decimal ancho, decimal largo, decimal cantidad)
        => Math.Round((ancho * largo * cantidad) / CONST_MSI, 2, MidpointRounding.AwayFromZero);

    public static decimal CalcularPieLineales(decimal ancho, decimal largo, decimal cantidad)
        => Math.Round(ancho * largo * cantidad * CONST_PIE_LINEALES, 4, MidpointRounding.AwayFromZero);

    public static (decimal SubTotal, decimal Itbis, decimal Total, decimal TotalPieLineales, decimal TotalKilos)
        CalcularTotales(
            IEnumerable<decimal> renglones,
            IEnumerable<decimal> piesLineales,
            IEnumerable<decimal> kilos,
            decimal porcItbis = PORC_ITBIS)
    {
        decimal subTotal = renglones.Sum();
        decimal itbis = CalcularItbis(subTotal, porcItbis);
        decimal total = CalcularTotal(subTotal, itbis);
        return (subTotal, itbis, total, piesLineales.Sum(), kilos.Sum());
    }
}
