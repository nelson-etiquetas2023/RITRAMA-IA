using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas del camino de rechazo de SavePedidoCompleto. El servicio se apunta a un servidor
/// inexistente: si el metodo devuelve false es porque valido antes de abrir la conexion,
/// asi que estas pruebas nunca tocan una base de datos real.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoServiceValidacionTests
{
    private const string ConexionInexistente =
        "Server=localhost;Database=Ritrama_NoExiste_Pruebas;User Id=sa;Password=NoAbre;TrustServerCertificate=True;Connect Timeout=2";

    private static PedidoService CrearServicio()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ambiente"] = "Desarrollo",
                ["ConnectionStringsEnvironment:Desarrollo"] = ConexionInexistente
            })
            .Build();

        return new PedidoService(config);
    }

    private static Pedido PedidoCon(PedidoDetalle? linea)
    {
        Pedido pedido = new()
        {
            Numero = "SO-7770",
            Fecha = new DateTime(2026, 9, 25),
            Customer_Id = Guid.Parse("11111111-1111-1111-1111-111111111111"),
            Estado = PedidoEstado.Creado
        };

        if (linea != null)
        {
            pedido.Detalle.Add(linea);
        }

        return pedido;
    }

    private static PedidoDetalle LineaValida()
    {
        return new PedidoDetalle { Product_id = "P001", Cant = 1m, Precio = 10m };
    }

    [Fact]
    public void SavePedidoCompleto_SinCliente_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();
        Pedido pedido = PedidoCon(LineaValida());
        pedido.Customer_Id = Guid.Empty;

        bool resultado = service.SavePedidoCompleto(pedido);

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("cliente");
    }

    [Fact]
    public void SavePedidoCompleto_SinLineas_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();

        bool resultado = service.SavePedidoCompleto(PedidoCon(null));

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("linea");
    }

    [Fact]
    public void SavePedidoCompleto_LineaSinProducto_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();
        PedidoDetalle linea = LineaValida();
        linea.Product_id = null;

        bool resultado = service.SavePedidoCompleto(PedidoCon(linea));

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("producto");
    }

    [Fact]
    public void SavePedidoCompleto_CantidadCero_DevuelveFalseConMotivo()
    {
        PedidoService service = CrearServicio();
        PedidoDetalle linea = LineaValida();
        linea.Cant = 0m;

        bool resultado = service.SavePedidoCompleto(PedidoCon(linea));

        resultado.Should().BeFalse();
        service.ErrorMsg.Should().Contain("cantidad");
    }
}
