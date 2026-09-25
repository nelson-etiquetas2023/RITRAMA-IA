using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas unitarias del calculo de importes. No abren conexiones a la base de datos.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoCalculosTests
{
    [Fact]
    public void Calcular_DosLineas_SumaCantPorPrecio()
    {
        List<PedidoDetalle> lineas = new()
        {
            new PedidoDetalle { Cant = 2m, Precio = 50m },
            new PedidoDetalle { Cant = 3m, Precio = 25.50m }
        };

        (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(lineas, 0m);

        subTotal.Should().Be(176.50m);
        montoItbis.Should().Be(0m);
        total.Should().Be(176.50m);
    }

    [Fact]
    public void Calcular_SinLineas_DevuelveCeros()
    {
        (decimal subTotal, decimal montoItbis, decimal total) =
            PedidoCalculos.Calcular(new List<PedidoDetalle>(), 18m);

        subTotal.Should().Be(0m);
        montoItbis.Should().Be(0m);
        total.Should().Be(0m);
    }

    [Fact]
    public void Calcular_AplicaItbisDe18PorCiento()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 1m, Precio = 100m } };

        (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(lineas, 18m);

        subTotal.Should().Be(100m);
        montoItbis.Should().Be(18m);
        total.Should().Be(118m);
    }

    [Fact]
    public void Calcular_RespetaPorcentajeItbisEditado()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 1m, Precio = 200m } };

        (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(lineas, 10m);

        montoItbis.Should().Be(20m);
        total.Should().Be(220m);
    }

    [Fact]
    public void Calcular_RedondeaASoloDosDecimales()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 1m, Precio = 0.125m } };

        var (subTotal, _, _) = PedidoCalculos.Calcular(lineas, 0m);

        subTotal.Should().Be(0.13m);
    }

    [Fact]
    public void Calcular_LineaSinPrecio_CuentaComoCero()
    {
        List<PedidoDetalle> lineas = new() { new PedidoDetalle { Cant = 4m, Precio = null } };

        var (subTotal, _, _) = PedidoCalculos.Calcular(lineas, 18m);

        subTotal.Should().Be(0m);
    }

    [Fact]
    public void TotalRenglon_MultiplicaCantidadPorPrecio()
    {
        PedidoCalculos.TotalRenglon(3m, 12.50m).Should().Be(37.50m);
    }

    [Fact]
    public void TotalRenglon_PrecioNuloDevuelveCero()
    {
        PedidoCalculos.TotalRenglon(3m, null).Should().Be(0m);
    }
}
