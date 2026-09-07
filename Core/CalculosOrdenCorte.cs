namespace Ritrama2025.Core;

public static class CalculosOrdenCorte
{
    public const double FACTOR_MSI = 0.012;

    public static double CalcularMsi(double ancho, double largo)
        => ancho * largo * FACTOR_MSI;

    public static double RestanteMaterial(double total, double utilizado)
        => Math.Round(total - utilizado, 2);

    public static double LongitudTotal(double longitudCortar, double vueltas)
        => longitudCortar * vueltas;

    public static int TotalRollos(int vueltas, int cortesAncho)
        => vueltas * cortesAncho;

    public static double LongitudReal(double largo, double menos, double plus)
        => largo - menos + plus;

    public static bool CuadreAnchoValido(double sumaAnchosCortes, double anchoMaster, double tolerancia = 0.01)
        => Math.Abs(sumaAnchosCortes - anchoMaster) <= tolerancia;

    public static bool MasterTieneMaterialSuficiente(double consumoRequerido, double restante, double tolerancia = 0.01)
        => consumoRequerido <= restante + tolerancia;
}