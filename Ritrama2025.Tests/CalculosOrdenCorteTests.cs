using FluentAssertions;
using Ritrama2025.Core;
using Xunit;

namespace Ritrama2025.Tests;

public class CalculosOrdenCorteTests
{
    [Theory]
    [InlineData(10, 100, 12.0)]
    [InlineData(0, 100, 0.0)]
    [InlineData(1, 100, 1.2)]
    public void CalcularMsi_MultiplicaAnchoLargoYFactor(double ancho, double largo, double esperado)
    {
        double resultado = CalculosOrdenCorte.CalcularMsi(ancho, largo);
        resultado.Should().Be(esperado);
    }

    [Fact]
    public void RestanteMaterial_RestaYRedondeaADosDecimales()
    {
        double resultado = CalculosOrdenCorte.RestanteMaterial(10.055, 2.5);
        resultado.Should().Be(7.56);
    }

    [Theory]
    [InlineData(20115.00, 20000.00, false, 115.0)]
    [InlineData(100.0, 0.0, false, 100.0)]
    [InlineData(100.0, 100.0, false, 0.0)]
    [InlineData(100.0, 150.0, false, -50.0)]
    public void RestanteMaster_EsLargoMenosConsumo(double largo, double consumo, bool desperdicio, double esperado)
    {
        // REGLA RN-RESTANTE-OC: caso real OC 4625 (largo 20115.00 - consumo 20000.00 = 115.00).
        CalculosOrdenCorte.RestanteMaster(largo, consumo, desperdicio).Should().Be(esperado);
    }

    [Theory]
    [InlineData(20115.00, 20000.00)]
    [InlineData(100.0, 150.0)]
    public void RestanteMaster_ConDesperdicio_EsCero(double largo, double consumo)
    {
        CalculosOrdenCorte.RestanteMaster(largo, consumo, desperdicio: true).Should().Be(0.0,
            "una OC de desperdicio consume todo el master, el restante archivado es 0");
    }

    [Fact]
    public void LongitudTotal_MultiplicaLongitudPorVueltas()
    {
        double resultado = CalculosOrdenCorte.LongitudTotal(25, 20);
        resultado.Should().Be(500.0);
    }

    [Fact]
    public void TotalRollos_MultiplicaVueltasPorCortesAncho()
    {
        int resultado = CalculosOrdenCorte.TotalRollos(8, 5);
        resultado.Should().Be(40);
    }

    [Fact]
    public void LongitudReal_CalcularLargoMenosMas()
    {
        double resultado = CalculosOrdenCorte.LongitudReal(100, 10, 5);
        resultado.Should().Be(95.0);
    }

    [Theory]
    [InlineData(399.99, 0.01)]
    [InlineData(400, 0.01)]
    [InlineData(390, 0.01)]
    [InlineData(400.005, 0.01)]
    public void SumaCortesNoExcedeMaster_DevuelveVerdaderoCuandoCabeEnAnchoDelMaster(double suma, double tolerancia)
    {
        CalculosOrdenCorte.SumaCortesNoExcedeMaster(suma, 400, tolerancia).Should().BeTrue();
    }

    [Theory]
    [InlineData(400.02, 0.01)]
    [InlineData(400.02, 0.0)]
    [InlineData(410)]
    [InlineData(450)]
    public void SumaCortesNoExcedeMaster_DevuelveFalsoCuandoExcedeAnchoDelMaster(double suma, double tolerancia = 0.01)
    {
        CalculosOrdenCorte.SumaCortesNoExcedeMaster(suma, 400, tolerancia).Should().BeFalse();
    }

    [Theory]
    [InlineData(500, 500)]
    [InlineData(450, 500)]
    [InlineData(500.005, 500)]
    [InlineData(19500, 20617)]
    public void MasterTieneMaterialSuficiente_DevuelveVerdaderoCuandoConsumoNoExcedeRestante(double consumo, double restante)
    {
        CalculosOrdenCorte.MasterTieneMaterialSuficiente(consumo, restante).Should().BeTrue();
    }

    [Theory]
    [InlineData(500.02, 500)]
    [InlineData(600, 500)]
    [InlineData(20700, 20617)]
    public void MasterTieneMaterialSuficiente_DevuelveFalsoCuandoConsumoExcedeRestante(double consumo, double restante)
    {
        CalculosOrdenCorte.MasterTieneMaterialSuficiente(consumo, restante).Should().BeFalse();
    }

    [Theory]
    [InlineData(400.02, 400)]
    [InlineData(410, 400)]
    [InlineData(450, 400)]
    public void CortesExcedenAnchoMaster_DevuelveVerdaderoCuandoSumaSuperaAncho(double suma, double ancho)
    {
        CalculosOrdenCorte.CortesExcedenAnchoMaster(suma, ancho).Should().BeTrue();
    }

    [Theory]
    [InlineData(399.99, 400)]
    [InlineData(400, 400)]
    [InlineData(390, 400)]
    public void CortesExcedenAnchoMaster_DevuelveFalsoCuandoSumaCabeEnAncho(double suma, double ancho)
    {
        CalculosOrdenCorte.CortesExcedenAnchoMaster(suma, ancho).Should().BeFalse();
    }

    [Theory]
    [InlineData(500.02, 10, 500)]
    [InlineData(600, 10, 500)]
    [InlineData(20700, 1, 20617)]
    public void ConsumoExcedeLargoDisponible_DevuelveVerdaderoCuandoConsumoSuperaLargo(double longitud, double vueltas, double largo)
    {
        CalculosOrdenCorte.ConsumoExcedeLargoDisponible(longitud, vueltas, largo).Should().BeTrue();
    }

    [Theory]
    [InlineData(500, 1, 500)]
    [InlineData(450, 1, 500)]
    [InlineData(500.005, 1, 500)]
    public void ConsumoExcedeLargoDisponible_DevuelveFalsoCuandoConsumoCabeEnLargo(double longitud, double vueltas, double largo)
    {
        CalculosOrdenCorte.ConsumoExcedeLargoDisponible(longitud, vueltas, largo).Should().BeFalse();
    }
}

public class ConversorCeldaTests
{
    [Theory]
    [InlineData("25", 25)]
    [InlineData("abc", 0)]
    [InlineData(null, 0)]
    public void ToInt_DevuelveEnteroConvertidoOCero(string? valor, int esperado)
    {
        ConversorCelda.ToInt(valor).Should().Be(esperado);
    }

    [Fact]
    public void ToInt_DBNull_DevuelveCero()
    {
        ConversorCelda.ToInt(System.DBNull.Value).Should().Be(0);
    }

    [Theory]
    [InlineData("12.5", 12.5)]
    [InlineData("--", 0.0)]
    public void ToDouble_DevuelveDecimalConvertidoOCero(string valor, double esperado)
    {
        ConversorCelda.ToDouble(valor).Should().Be(esperado);
    }

    [Theory]
    [InlineData(null, "")]
    [InlineData("abc", "abc")]
    public void ToString_DevuelveTextoOCadenaVacia(string? valor, string esperado)
    {
        ConversorCelda.ToString(valor).Should().Be(esperado);
    }

    [Fact]
    public void ToString_DBNull_DevuelveVacio()
    {
        ConversorCelda.ToString(System.DBNull.Value).Should().Be("");
    }
}