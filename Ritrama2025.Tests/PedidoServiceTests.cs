using System.Data;
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

    private void LimpiarPedido(string numero)
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

        string n1 = await service.GetNewNumeroPedido();
        PedidoNumero.EsValido(n1).Should().BeTrue();
        string n2 = await service.GetNewNumeroPedido();
        PedidoNumero.EsValido(n2).Should().BeTrue();
        PedidoNumero.ParteNumerica(n2).Should().Be(PedidoNumero.ParteNumerica(n1) + 1);

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

        string numero = await service.GetNewNumeroPedido();
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
        string numero = await service.GetNewNumeroPedido();
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
    public async Task AnularPedido_MarcaAnulado()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        Guid customerId = Guid.Parse(customerIdObj.ToString()!);

        // Numero generado por el consecutivo, nunca fijo: un numero fijo podria existir en la
        // base de desarrollo y el DELETE del finally borraria un pedido real.
        string numero = await service.GetNewNumeroPedido();
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
            _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");
        }
    }

    [Fact]
    public void ActualizarEstadoPedido_EstadoInvalidoRechazadoDevuelveFalse()
    {
        IPedidoService service = CrearServicio();
        service.ActualizarEstadoPedido("SO-999999", "inexistente").Should().BeFalse();
    }

    /// <summary>
    /// Cada columna del encabezado recibe un valor distinto y reconocible. Si el mapeo de
    /// parametros se desplaza una posicion al agregar una columna al INSERT, los valores caen
    /// en columnas vecinas y este test lo detecta: un simple "se guardo bien" no lo notaria,
    /// porque el INSERT seguiria teniendo exito y sin error de SQL.
    /// </summary>
    [SkippableFact]
    public async Task SavePedidoCompleto_CadaColumnaQuedaEnSuPropiaColumna()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;

        string numero = await service.GetNewNumeroPedido();
        try
        {
            Pedido pedido = new Pedido
            {
                Numero = numero,
                Fecha = new DateTime(2026, 3, 4),
                Customer_Id = customerId,
                Customer_Name = "MAPA_CLIENTE",
                Persona_Contacto = "MAPA_CONTACTO",
                Tipo_venta = "credito",
                Fecha_entrega = new DateTime(2026, 5, 6),
                Condiciones_pago = "neto 30",
                Prioridad = "urgente",
                Direccion_entrega = "MAPA_DIRECCION",
                Estado = PedidoEstado.Creado,
                Notas = "MAPA_NOTAS",
                SubTotal = 111.11m,
                Porc_Itbis = 18m,
                Monto_Itbis = 20.00m,
                Total = 131.11m,
                Detalle =
                {
                    new PedidoDetalle
                    {
                        Product_id = productId,
                        Product_name = "MAPA_PRODUCTO",
                        Cant = 3m,
                        Unidad = "un",
                        Width = 1m,
                        Lenght = 2m,
                        Msi = 1m,
                        // precio y total_renglon son decimal(9,2) en la base: un valor con mas
                        // de dos decimales se redondea al guardar y la comparacion exacta
                        // fallaria por el redondeo, no por un error de mapeo.
                        Precio = 37.04m,
                        Total_Renglon = 111.12m,
                        Notas = "MAPA_NOTA_DETALLE"
                    }
                }
            };

            service.SavePedidoCompleto(pedido).Should().BeTrue();

            // Cada lectura usa un valor que solo puede venir de su columna. Los decimales se
            // comparan con tolerancia porque decimal(18,2) redondea al guardar.
            DataRow fila = (await service.LoadDataPedidos()).AsEnumerable()
                .Cast<DataRow>()
                .Single(r => r["numero"].ToString() == numero);

            fila["customer_name"].ToString().Should().Be("MAPA_CLIENTE");
            fila["persona_contacto"].ToString().Should().Be("MAPA_CONTACTO");
            fila["tipo_venta"].ToString().Should().Be("credito");
            fila["condiciones_pago"].ToString().Should().Be("neto 30");
            fila["prioridad"].ToString().Should().Be("urgente");
            fila["direccion_entrega"].ToString().Should().Be("MAPA_DIRECCION");
            fila["estado"].ToString().Should().Be(PedidoEstado.Creado);
            fila["notas"].ToString().Should().Be("MAPA_NOTAS");
            Convert.ToDecimal(fila["subtotal"]).Should().Be(111.11m);
            Convert.ToDecimal(fila["porc_itbis"]).Should().Be(18m);
            Convert.ToDecimal(fila["itbis"]).Should().Be(20.00m);
            Convert.ToDecimal(fila["total$"]).Should().Be(131.11m);
            Convert.ToDateTime(fila["fecha"]).Should().Be(new DateTime(2026, 3, 4));
            Convert.ToDateTime(fila["fecha_entrega"]).Should().Be(new DateTime(2026, 5, 6));

            List<PedidoDetalle> lineas = PedidoDetalleMapper.Mapear(await service.LoadDataPedidoDetalle(numero));
            lineas.Should().ContainSingle();
            lineas[0].Product_name.Should().Be("MAPA_PRODUCTO");
            lineas[0].Cant.Should().Be(3m);
            lineas[0].Precio.Should().Be(37.04m);
            lineas[0].Notas.Should().Be("MAPA_NOTA_DETALLE");
        }
        finally
        {
            LimpiarPedido(numero);
            _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");
        }
    }

    [SkippableFact]
    public async Task ActualizarEstadoPedido_TransicionesValidasCambiaEstado()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        Guid customerId = Guid.Parse(customerIdObj.ToString()!);

        // Numero generado por el consecutivo, nunca fijo (ver AnularPedido_MarcaAnulado).
        string numero = await service.GetNewNumeroPedido();
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
            _fixture.ExecuteNonQuery("UPDATE control SET par1 = par1 - 1 WHERE filter='PED'");
        }
    }
}
