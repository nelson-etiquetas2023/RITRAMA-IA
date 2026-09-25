using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de integracion del modulo Pedido contra el SQL Server de pruebas/dev.
/// El fixture expone ProduccionService, por lo que cada test crea su propio PedidoService.
/// </summary>
[Trait("Categoria", "Integracion")]
public class PedidoServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public PedidoServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private IPedidoService CrearServicio()
    {
        IConfiguration config = TestConfiguration.BuildServiceConfiguration();
        return new PedidoService(config);
    }

    private void LimpiarPedido(int numero)
    {
        _fixture.ExecuteNonQuery("DELETE FROM pedido_detalle WHERE numero = @p1", ("@p1", numero));
        _fixture.ExecuteNonQuery("DELETE FROM pedido WHERE numero = @p1", ("@p1", numero));
    }

    private Guid ObtenerCustomerReal()
    {
        object? obj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        if (obj == null)
        {
            throw new Xunit.Sdk.XunitException("no hay clientes; validado en prueba manual");
        }
        return Guid.Parse(obj.ToString()!);
    }

    [Fact]
    public async Task GetNewNumeroPedido_IncrementaConsecutivoPED()
    {
        IPedidoService service = CrearServicio();

        int n1 = await service.GetNewNumeroPedido();
        n1.Should().BeGreaterThan(0);
        int n2 = await service.GetNewNumeroPedido();
        n2.Should().Be(n1 + 1);

        // restaurar el contador para no dejar rastro (comportamiento del servicio es incrementar).
        _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 2 WHERE filter='PED'");
    }

    [SkippableFact]
    public async Task SavePedidoCompleto_ConDetalleValido_InsertaHeaderYDetalle()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;

        int numero = await service.GetNewNumeroPedido();
        try
        {
            Pedido pedido = new Pedido
            {
                Numero = numero,
                Fecha = DateTime.Now,
                Customer_Id = customerId,
                Customer_Name = "Cliente Test",
                Estado = PedidoEstado.Creado,
                SubTotal = 100m,
                Porc_Itbis = 18m,
                Monto_Itbis = 18m,
                Total = 118m,
                Detalle =
                {
                    new PedidoDetalle
                    {
                        Product_id = productId,
                        Product_name = "Producto Test",
                        Cant = 1m,
                        Unidad = "un",
                        Width = 10m,
                        Lenght = 20m,
                        Msi = 1m,
                        Precio = 100m,
                        Total_Renglon = 100m
                    },
                    new PedidoDetalle
                    {
                        Product_id = productId,
                        Product_name = "Producto Test",
                        Cant = 2m,
                        Unidad = "un",
                        Width = 30m,
                        Lenght = 40m,
                        Msi = 1m,
                        Precio = 50m,
                        Total_Renglon = 100m
                    }
                }
            };

            bool ok = service.SavePedidoCompleto(pedido);
            ok.Should().BeTrue();

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido_detalle WHERE numero = @p1", ("@p1", numero))!).Should().Be(2);
        }
        finally
        {
            LimpiarPedido(numero);
            _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");
        }
    }

    [SkippableFact]
    public async Task SavePedidoCompleto_EsAtomico_NoDejaRastroSiFalla()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        int numero = await service.GetNewNumeroPedido();
        try
        {
            Pedido pedido = new Pedido
            {
                Numero = numero,
                Fecha = DateTime.Now,
                Customer_Id = customerId,
                Customer_Name = "Cliente Test",
                Estado = PedidoEstado.Creado,
                Detalle =
                {
                    // product_id es NVARCHAR(25): un valor de 30 chars desborda la columna
                    // en el INSERT del detalle y fuerza una SqlException real.
                    new PedidoDetalle
                    {
                        Product_id = new string('x', 30),
                        Cant = 1m,
                        Width = 10m,
                        Lenght = 20m,
                        Msi = 1m
                    }
                }
            };

            bool ok = service.SavePedidoCompleto(pedido);
            ok.Should().BeFalse();

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(0,
                "el rollback debe eliminar el encabezado aunque el error sea en el detalle");
        }
        finally
        {
            LimpiarPedido(numero);
            _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");
        }
    }

    [SkippableFact]
    public void AnularPedido_MarcaAnulado()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        Guid customerId = Guid.Parse(customerIdObj.ToString()!);

        int numero = 950000;
        try
        {
            _fixture.ExecuteNonQuery(
                "INSERT INTO pedido (numero,fecha,customer_id,customer_name,estado) VALUES (@p1,@p2,@p3,@p4,@p5)",
                ("@p1", numero),
                ("@p2", DateTime.Now),
                ("@p3", customerId),
                ("@p4", "Cliente Test"),
                ("@p5", "creado"));

            service.AnularPedido(numero).Should().BeTrue();
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT anulado FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
        }
        finally
        {
            LimpiarPedido(numero);
        }
    }

    [Fact]
    public void ActualizarEstadoPedido_EstadoInvalidoRechazadoDevuelveFalse()
    {
        IPedidoService service = CrearServicio();
        service.ActualizarEstadoPedido(999999, "inexistente").Should().BeFalse();
    }

    [SkippableFact]
    public void ActualizarEstadoPedido_TransicionesValidasCambiaEstado()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        Guid customerId = Guid.Parse(customerIdObj.ToString()!);

        int numero = 950001;
        try
        {
            _fixture.ExecuteNonQuery(
                "INSERT INTO pedido (numero,fecha,customer_id,customer_name,estado) VALUES (@p1,@p2,@p3,@p4,@p5)",
                ("@p1", numero),
                ("@p2", DateTime.Now),
                ("@p3", customerId),
                ("@p4", "Cliente Test"),
                ("@p5", "creado"));

            service.ActualizarEstadoPedido(numero, PedidoEstado.EnProduccion).Should().BeTrue();
            _fixture.ExecuteScalar(
                "SELECT estado FROM pedido WHERE numero = @p1", ("@p1", numero))!.ToString().Should().Be("en producción");
        }
        finally
        {
            LimpiarPedido(numero);
        }
    }
}
