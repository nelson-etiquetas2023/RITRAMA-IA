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

    [Fact]
    public void TotalCantidad_SumaLasCantidadesDeLasLineas()
    {
        PedidoDetalle[] lineas = { Linea(2m), Linea(3.5m), Linea(1.25m) };

        PedidoCalculos.TotalCantidad(lineas).Should().Be(6.75m);
    }

    [Fact]
    public void TotalCantidad_SinLineas_DevuelveCero()
    {
        PedidoCalculos.TotalCantidad(Array.Empty<PedidoDetalle>()).Should().Be(0m);
    }

    [Fact]
    public void TotalCantidad_SerieNula_DevuelveCeroYNoLanza()
    {
        PedidoCalculos.TotalCantidad(null!).Should().Be(0m);
    }

    /// <summary>
    /// La suma de cantidades es exacta y no se redondea. La columna pedido_detalle.cant es
    /// decimal(18,2) NOT NULL, asi que cada linea tiene a lo sumo dos decimales y el
    /// acumulado nunca se desvía. Esta prueba usa el caso que mas se acerca al limite: tres
    /// lineas de 0.33 que en coma flotante darían 0.989999... y con un redondeo mal puesto se
    /// verian como 1.00 en vez de 0.99.
    /// </summary>
    [Fact]
    public void TotalCantidad_NoRedondeaNiPierdePrecision()
    {
        PedidoDetalle[] lineas = { Linea(0.33m), Linea(0.33m), Linea(0.33m) };

        PedidoCalculos.TotalCantidad(lineas).Should().Be(0.99m);
    }

    /// <summary>
    /// El caso real de la base: cantidades enteras. SO-00041 tiene dos lineas de 20 y 30, y
    /// el total tiene que dar 50.00 y no 50.
    /// </summary>
    [Fact]
    public void TotalCantidad_CantidadesEnteras_DaElTotalExacto()
    {
        PedidoDetalle[] lineas = { Linea(20m), Linea(30m) };

        PedidoCalculos.TotalCantidad(lineas).Should().Be(50m);
    }

    /// <summary>
    /// El total tiene que verse igual que la columna Qty del grid, que muestra cada cantidad con
    /// N2. Se compara el valor, no el texto, porque el separador que pone N2 depende de la
    /// cultura de la maquina y una prueba que lo fije seria fragil.
    /// </summary>
    [Fact]
    public void TotalCantidad_DaElValorExactoQueSeMostraraConN2()
    {
        PedidoDetalle[] lineas = { Linea(2.5m), Linea(3.25m) };

        decimal total = PedidoCalculos.TotalCantidad(lineas);

        total.Should().Be(5.75m);
        total.ToString("N2").Should().Be((2.5m + 3.25m).ToString("N2"));
    }

    /// <summary>
    /// El total de cantidad es independiente del precio: dos lineas con la misma cantidad dan el
    /// mismo total, valga lo que valgan. Es lo que lo distingue del subtotal.
    /// </summary>
    [Fact]
    public void TotalCantidad_NoDependeDelPrecio()
    {
        PedidoDetalle[] lineas =
        {
            new PedidoDetalle { Cant = 4m, Precio = 100m },
            new PedidoDetalle { Cant = 4m, Precio = null },
        };

        PedidoCalculos.TotalCantidad(lineas).Should().Be(8m);
    }

    private static PedidoDetalle Linea(decimal cant) => new() { Cant = cant };
}
