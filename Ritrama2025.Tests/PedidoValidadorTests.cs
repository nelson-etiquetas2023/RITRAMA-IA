using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas unitarias de las reglas de negocio del pedido. No abren conexiones.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoValidadorTests
{
    private static Pedido PedidoValido()
    {
        return new Pedido
        {
            Numero = "SO-01001",
            Fecha = new DateTime(2026, 9, 25),
            Customer_Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Estado = PedidoEstado.Creado,
            SubTotal = 100m,
            Porc_Itbis = 18m,
            Monto_Itbis = 18m,
            Total = 118m,
            Detalle =
            {
                new PedidoDetalle
                {
                    Product_id = "P001",
                    Product_name = "Producto Test",
                    Cant = 1m,
                    Precio = 100m,
                    Total_Renglon = 100m
                }
            }
        };
    }

    [Fact]
    public void EsValido_PedidoCompleto_EsValido()
    {
        PedidoValidador.EsValido(PedidoValido(), out string error).Should().BeTrue();
        error.Should().BeEmpty();
    }

    /// <summary>
    /// El numero ya no es responsabilidad del validador. Lo reserva el servicio dentro de la
    /// transaccion, cuando el pedido viene de la pantalla con el numero vacio. Si el validador lo
    /// exigiera aca, rechazaria todo pedido nuevo antes de llegar al servicio.
    /// </summary>
    [Theory]
    [InlineData("")]
    [InlineData("1001")]
    [InlineData("SO-00027")]
    [InlineData("SO-99999")]
    public void EsValido_NumeroNoLoRevisa_ElServicioLoAsigna(string numero)
    {
        Pedido pedido = PedidoValido();
        pedido.Numero = numero;

        bool ok = PedidoValidador.EsValido(pedido, out string error);

        ok.Should().BeTrue();
        error.Should().BeEmpty();
    }

    [Fact]
    public void EsValido_SinCliente_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Customer_Id = Guid.Empty;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("cliente");
    }

    [Fact]
    public void EsValido_SinLineas_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle.Clear();

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("linea");
    }

    [Fact]
    public void EsValido_LineaSinProducto_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle[0].Product_id = "  ";

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("producto");
    }

    [Fact]
    public void EsValido_CantidadCero_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle[0].Cant = 0m;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("cantidad");
    }

    [Fact]
    public void EsValido_PrecioNegativo_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Detalle[0].Precio = -1m;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("precio");
    }

    [Fact]
    public void EsValido_PorcItbisNegativo_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Porc_Itbis = -1m;

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("ITBIS");
    }

    [Fact]
    public void EsValido_EstadoDesconocido_Rechazado()
    {
        Pedido pedido = PedidoValido();
        pedido.Estado = "inventado";

        PedidoValidador.EsValido(pedido, out string error).Should().BeFalse();
        error.Should().Contain("estado");
    }

    [Fact]
    public void EsValido_EstadoVacio_AceptadoYSeCompletaAlGuardar()
    {
        Pedido pedido = PedidoValido();
        pedido.Estado = null;

        PedidoValidador.EsValido(pedido, out string error).Should().BeTrue();
        error.Should().BeEmpty();
    }

    [Theory]
    [InlineData("creado")]
    [InlineData("en producción")]
    [InlineData("pickeado")]
    [InlineData("despachado")]
    [InlineData("devuelto")]
    public void EsEstadoValido_EstadosConocidos_Aceptados(string estado)
    {
        PedidoValidador.EsEstadoValido(estado).Should().BeTrue();
    }

    [Fact]
    public void EsEstadoValido_Desconocido_Rechazado()
    {
        PedidoValidador.EsEstadoValido("inventado").Should().BeFalse();
        PedidoValidador.EsEstadoValido("").Should().BeFalse();
    }
}
