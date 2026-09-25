namespace Ritrama2025.Core;

public static class CalculosOrdenCorte
{
    public const double FACTOR_MSI = 0.012;

    public static double CalcularMsi(double ancho, double largo)
        => ancho * largo * FACTOR_MSI;

    public static double RestanteMaterial(double total, double utilizado)
        => Math.Round(total - utilizado, 2);

    // REGLA DE PRODUCCION RN-RESTANTE-OC: el restante archivado de un master en la OC siempre
    // es largo del master − consumo de la OC. Si la OC es desperdicio, todo el master se
    // consume como desperdicio (igual que el libro contable de consumos), por lo que el
    // restante queda en 0.
    public static double RestanteMaster(double largo, double consumo, bool desperdicio)
        => Math.Round(desperdicio ? 0.0 : largo - consumo, 2);

    public static double LongitudTotal(double longitudCortar, double vueltas)
        => longitudCortar * vueltas;

    public static int TotalRollos(int vueltas, int cortesAncho)
        => vueltas * cortesAncho;

    public static double LongitudReal(double largo, double menos, double plus)
        => largo - menos + plus;

    public static bool SumaCortesNoExcedeMaster(double sumaAnchosCortes, double anchoMaster, double tolerancia = 0.01)
        => sumaAnchosCortes <= anchoMaster + tolerancia;

    public static bool CuadreAnchoValido(double sumaAnchosCortes, double anchoMaster, double tolerancia = 0.01)
        => sumaAnchosCortes >= anchoMaster - tolerancia;

    public static bool MasterTieneMaterialSuficiente(double consumoRequerido, double restante, double tolerancia = 0.01)
        => consumoRequerido <= restante + tolerancia;

    // REGLA VISUAL DE LA PARRILLA: el grid de cortes se marca en rojo cuando la suma de
    // anchos de los cortes excede el ancho del master, o cuando el consumo total
    // (longitud a cortar x vueltas) excede el largo del master.
    public static bool CortesExcedenAnchoMaster(double sumaAnchosCortes, double anchoMaster, double tolerancia = 0.01)
        => !SumaCortesNoExcedeMaster(sumaAnchosCortes, anchoMaster, tolerancia);

    public static bool ConsumoExcedeLargoDisponible(double longitudCortar, double vueltas, double largoMaster, double tolerancia = 0.01)
        => !MasterTieneMaterialSuficiente(LongitudTotal(longitudCortar, vueltas), largoMaster, tolerancia);
}
