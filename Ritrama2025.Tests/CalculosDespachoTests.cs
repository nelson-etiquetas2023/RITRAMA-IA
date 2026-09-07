using FluentAssertions;
using Ritrama2025.Core;
using Xunit;

namespace Ritrama2025.Tests;

public class CalculosDespachoTests
{
    [Theory]
    [InlineData(1000.0, 180.00)]
    [InlineData(0.0, 0.0)]
    [InlineData(250.0, 45.0)]
    [InlineData(33.33, 6.00)]
    public void CalcularItbis_DevuelveEl18PorCientoRedondeado(double subtotal, double esperado)
    {
        CalculosDespacho.CalcularItbis((decimal)subtotal).Should().Be((decimal)esperado);
    }

    [Fact]
    public void CalcularTotal_SumaSubtotalEItbis()
    {
        CalculosDespacho.CalcularTotal(1000.00m, 180.00m).Should().Be(1180.00m);
    }

    [Theory]
    [InlineData(2, 100, 3, 7.2)]
    [InlineData(0, 100, 3, 0)]
    [InlineData(2, 0, 3, 0)]
    public void CalcularMsiRenglon_AplicaFormulaDePicking(double ancho, double largo, double cantidad, double esperado)
    {
        CalculosDespacho
            .CalcularMsiRenglon((decimal)ancho, (decimal)largo, (decimal)cantidad)
            .Should().Be((decimal)esperado);
    }

    [Fact]
    public void CalcularPieLineales_AplicaConstante()
    {
        // ancho 2 * largo 100 * cantidad 3 * 0.012 = 7.2
        CalculosDespacho.CalcularPieLineales(2m, 100m, 3).Should().Be(7.2m);
    }

    [Fact]
    public void Constantes_Incambiadas()
    {
        CalculosDespacho.PORC_ITBIS.Should().Be(18.00m);
        CalculosDespacho.CONST_MSI.Should().Be(83.33333333333333m);
    }

    [Fact]
    public void CalcularTotales_SumaRenglonesYAplicaItbis()
    {
        // Subtotal = 1000 + 500 = 1500 -> ITBIS 18% = 270 -> Total 1770
        var t = CalculosDespacho.CalcularTotales(
            renglones: new[] { 1000m, 500m },
            piesLineales: new[] { 10m, 5m },
            kilos: new[] { 100m, 50m });

        t.SubTotal.Should().Be(1500m);
        t.Itbis.Should().Be(270m);
        t.Total.Should().Be(1770m);
        t.TotalPieLineales.Should().Be(15m);
        t.TotalKilos.Should().Be(150m);
    }

    [Fact]
    public void CalcularTotales_SinRenglones_DevuelveCero()
    {
        var t = CalculosDespacho.CalcularTotales(
            renglones: new decimal[0],
            piesLineales: new decimal[0],
            kilos: new decimal[0]);

        t.SubTotal.Should().Be(0m);
        t.Itbis.Should().Be(0m);
        t.Total.Should().Be(0m);
    }

    [Fact]
    public void CalcularTotales_UsaPorcentajeItbisPersonalizado()
    {
        var t = CalculosDespacho.CalcularTotales(
            renglones: new[] { 1000m },
            piesLineales: new[] { 0m },
            kilos: new[] { 0m },
            porcItbis: 10m);

        t.Itbis.Should().Be(100m);
        t.Total.Should().Be(1100m);
    }
}