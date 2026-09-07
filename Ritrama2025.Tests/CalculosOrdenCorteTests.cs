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
    [InlineData(400.005, 0.01)]
    [InlineData(400.01, 0.01)]
    public void CuadreAnchoValido_DevuelveVerdaderoCuandoCubreAnchoDelMaster(double suma, double tolerancia)
    {
        CalculosOrdenCorte.CuadreAnchoValido(suma, 400, tolerancia).Should().BeTrue();
    }

    [Theory]
    [InlineData(399.98)]
    [InlineData(390)]
    [InlineData(400.02)]
    [InlineData(410)]
    public void CuadreAnchoValido_DevuelveFalsoCuandoNoCubreONoSeIgualaAnchoDelMaster(double suma)
    {
        CalculosOrdenCorte.CuadreAnchoValido(suma, 400).Should().BeFalse();
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