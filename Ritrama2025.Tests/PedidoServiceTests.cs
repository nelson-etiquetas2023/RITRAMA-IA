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

    private int LeerPar1() =>
        Convert.ToInt32(_fixture.ExecuteScalar("SELECT par1 FROM control WHERE filter='PED'")!);

    /// <summary>
    /// Devuelve el contador a su valor exacto. Restar 1 seria fragil: si el guardado falla, el
    /// ROLLBACK ya devolvio el numero al contador y restarlo de nuevo lo deja por debajo de lo
    /// que corresponde, el proximo guardado reintenta un numero en uso y revienta con
    /// UQ_pedido_numero. Restaurar el valor capturado lo hace exacto sin importar quantas
    /// reservas hubo.
    /// </summary>
    private void RestaurarPar1(int valorOriginal) =>
        _fixture.ExecuteNonQuery("UPDATE control SET par1 = @p1 WHERE filter='PED'", ("@p1", valorOriginal));

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
    public async Task GetProximoNumeroPedido_NoGastaNumeroYSaveAsignaElSiguiente()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;

        int antes = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT par1 FROM control WHERE filter='PED'")!);

        // Previsualizar es solo una lectura: tiene que devolver el mismo valor las veces que se
        // llame, porque es lo que la pantalla muestra como estimado.
        string previo1 = await service.GetProximoNumeroPedido();
        string previo2 = await service.GetProximoNumeroPedido();
        previo1.Should().Be(previo2, "previsualizar no debe avanzar el contador");
        PedidoNumero.EsValido(previo1).Should().BeTrue();

        Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT par1 FROM control WHERE filter='PED'")!).Should().Be(antes,
            "la previsualizacion no debe escribir nada");

        Pedido pedido = new Pedido
        {
            Fecha = DateTime.Now,
            Customer_Id = customerId,
            Customer_Name = "Cliente Test",
            Estado = PedidoEstado.Creado,
            Detalle =
            {
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "Producto Test",
                    Cant = 1m,
                    Unidad = "un",
                    Precio = 10m,
                    Total_Renglon = 10m
                }
            }
        };

        service.SavePedidoCompleto(pedido).Should().BeTrue(because: service.ErrorMsg ?? "sin mensaje del servicio");
        string asignado = pedido.Numero;
        PedidoNumero.EsValido(asignado).Should().BeTrue();

        try
        {
            // El numero guardado es el que se previsualizo, y el contador avanzo en uno.
            asignado.Should().Be(previo1);
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT par1 FROM control WHERE filter='PED'")!).Should().Be(antes + 1);
        }
        finally
        {
            LimpiarPedido(asignado);
            RestaurarPar1(antes);
        }
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

        // El numero no se pide: lo reserva el servicio al guardar y queda en pedido.Numero.
        int par1Antes = LeerPar1();
        Pedido pedido = new Pedido
        {
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

        try
        {
            service.SavePedidoCompleto(pedido).Should().BeTrue(because: service.ErrorMsg ?? "sin mensaje del servicio");

            string numero = pedido.Numero;
            PedidoNumero.EsValido(numero).Should().BeTrue();

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido_detalle WHERE numero = @p1", ("@p1", numero))!).Should().Be(2);
        }
        finally
        {
            LimpiarPedido(pedido.Numero);
            RestaurarPar1(par1Antes);
        }
    }

    /// <summary>
    /// La garantia fuerte de que la numeracion no tiene saltos: un guardado que falla tiene que
    /// devolver el numero al contador, y el siguiente guardado exitoso tiene que recibir
    /// EXACTAMENTE el mismo numero que se habia previsualizado. Si el rollback no devolviera la
    /// reserva, el segundo guardado recibiria el siguiente y quedaria un hueco.
    /// </summary>
    [SkippableFact]
    public async Task SavePedidoCompleto_FallidoYReintento_NoDejaSaltoEnLaNumeracion()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;

        int par1Antes = LeerPar1();
        string previsto = await service.GetProximoNumeroPedido();

        // Pedido con un detalle que desborda la columna: el INSERT del detalle falla y se
        // revierte la transaccion completa, incluida la reserva del numero.
        Pedido conDetalleInvalido = new Pedido
        {
            Fecha = DateTime.Now,
            Customer_Id = customerId,
            Customer_Name = "Cliente Test",
            Estado = PedidoEstado.Creado,
            Detalle =
            {
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

        Pedido pedidoValido = new Pedido
        {
            Fecha = DateTime.Now,
            Customer_Id = customerId,
            Customer_Name = "Cliente Test",
            Estado = PedidoEstado.Creado,
            Detalle =
            {
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "Producto Test",
                    Cant = 1m,
                    Unidad = "un",
                    Precio = 25m,
                    Total_Renglon = 25m
                }
            }
        };

        try
        {
            service.SavePedidoCompleto(conDetalleInvalido).Should().BeFalse();
            LeerPar1().Should().Be(par1Antes, "el fallo no debe avanzar el contador");

            service.SavePedidoCompleto(pedidoValido).Should().BeTrue();

            // Este es el punto: el reintento recibe el mismo numero, no el siguiente.
            pedidoValido.Numero.Should().Be(previsto,
                "el numero del intento fallido tiene que volver al contador");
            LeerPar1().Should().Be(par1Antes + 1, "solo el guardado exitoso avanza el contador");

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido WHERE numero = @p1", ("@p1", previsto))!).Should().Be(1);
        }
        finally
        {
            LimpiarPedido(conDetalleInvalido.Numero);
            LimpiarPedido(pedidoValido.Numero);
            RestaurarPar1(par1Antes);
        }
    }

    [SkippableFact]
    public async Task SavePedidoCompleto_EsAtomico_NoDejaRastroSiFalla()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);

        int par1Antes = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT par1 FROM control WHERE filter='PED'")!);

        Pedido pedido = new Pedido
        {
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

        try
        {
            service.SavePedidoCompleto(pedido).Should().BeFalse(because: "un detalle invalido debe impedir el guardado");

            // El numero si se reservo, pero la transaccion se revirtio entera.
            string numero = pedido.Numero;
            PedidoNumero.EsValido(numero).Should().BeTrue();

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido WHERE numero = @p1", ("@p1", numero))!).Should().Be(0,
                "el rollback debe eliminar el encabezado aunque el error sea en el detalle");

            // Esta es la garantia nueva: el ROLLBACK devuelve el numero al contador, asi que un
            // pedido que no se guardo no deja un hueco en la numeracion.
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT par1 FROM control WHERE filter='PED'")!).Should().Be(par1Antes,
                "un guardado fallido no debe gastar el consecutivo");
        }
        finally
        {
            LimpiarPedido(pedido.Numero);
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
        // base de desarrollo y el DELETE del finally borraria un pedido real. La previsualizacion
        // no consume, asi que el finally no devuelve nada al contador.
        string numero = await service.GetProximoNumeroPedido();
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

        // El numero no se pide: lo reserva el servicio al guardar.
        int par1Antes = LeerPar1();
        Pedido pedido = new Pedido
        {
            Fecha = new DateTime(2026, 3, 4),
            Customer_Id = customerId,
            Customer_Name = "MAPA_CLIENTE",
            Persona_Contacto = "MAPA_CONTACTO",
            Tipo_venta = "credito",
            Fecha_entrega = new DateTime(2026, 5, 6),
            Condiciones_pago = "30 dias",
            Prioridad = "urgente",
            Direccion_facturacion = "MAPA_FACTURACION",
            Direccion_entrega = "MAPA_ENTREGA",
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

        try
        {
            service.SavePedidoCompleto(pedido).Should().BeTrue(because: service.ErrorMsg ?? "sin mensaje del servicio");
            string numero = pedido.Numero;

            // Cada lectura usa un valor que solo puede venir de su columna. Los decimales se
            // comparan con tolerancia porque decimal(18,2) redondea al guardar.
            DataRow fila = (await service.LoadDataPedidos()).AsEnumerable()
                .Cast<DataRow>()
                .Single(r => r["numero"].ToString() == numero);

            fila["customer_name"].ToString().Should().Be("MAPA_CLIENTE");
            fila["persona_contacto"].ToString().Should().Be("MAPA_CONTACTO");
            fila["tipo_venta"].ToString().Should().Be("credito");
            fila["condiciones_pago"].ToString().Should().Be("30 dias");
            fila["prioridad"].ToString().Should().Be("urgente");
            fila["direccion_facturacion"].ToString().Should().Be("MAPA_FACTURACION");
            fila["direccion_entrega"].ToString().Should().Be("MAPA_ENTREGA");
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
            LimpiarPedido(pedido.Numero);
            RestaurarPar1(par1Antes);
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
        string numero = await service.GetProximoNumeroPedido();
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

    /// <summary>
    /// Editar un pedido reemplaza el encabezado y regenera el detalle completo, y NO consume el
    /// consecutivo: el numero lo tiene el pedido y actualizar nunca reserva uno nuevo. La
    /// re-lectura va por la base, no por el objeto en memoria, para detectar un UPDATE que no se
    /// aplicara o un detalle que no se reemplazara entero.
    /// </summary>
    [SkippableFact]
    public async Task ActualizarPedidoCompleto_ActualizaEncabezadoYReemplazaDetalle()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;

        int par1Antes = LeerPar1();

        // Dos lineas originales: al editar se conserva una (con la cantidad cambiada), se quita
        // la otra y se agrega una nueva. Las notas identifican cada linea al releer.
        Pedido pedido = new Pedido
        {
            Fecha = new DateTime(2026, 3, 4),
            Customer_Id = customerId,
            Customer_Name = "Cliente Original",
            Estado = PedidoEstado.Creado,
            Notas = "NOTA_ORIGINAL",
            SubTotal = 150m,
            Porc_Itbis = 18m,
            Monto_Itbis = 27m,
            Total = 177m,
            Detalle =
            {
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "ORIGINAL_A",
                    Cant = 1m,
                    Unidad = "un",
                    Width = 10m,
                    Lenght = 20m,
                    Msi = 1m,
                    Precio = 100m,
                    Total_Renglon = 100m,
                    Notas = "LINEA_A"
                },
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "ORIGINAL_B",
                    Cant = 2m,
                    Unidad = "un",
                    Width = 30m,
                    Lenght = 40m,
                    Msi = 1m,
                    Precio = 50m,
                    Total_Renglon = 100m,
                    Notas = "LINEA_B"
                }
            }
        };

        try
        {
            service.SavePedidoCompleto(pedido).Should().BeTrue(because: service.ErrorMsg ?? "sin mensaje del servicio");
            string numero = pedido.Numero;
            PedidoNumero.EsValido(numero).Should().BeTrue();

            int par1TrasGuardar = LeerPar1();
            string proximoAntes = await service.GetProximoNumeroPedido();

            // Encabezado con valores nuevos y detalle reescrito: fuera LINEA_B, LINEA_A con otra
            // cantidad y una linea mas. Es lo que la pantalla manda al guardar en modo Editar.
            pedido.Fecha = new DateTime(2026, 4, 10);
            pedido.Customer_Name = "Cliente Editado";
            pedido.Notas = "NOTA_EDITADA";
            pedido.Prioridad = "urgente";
            pedido.Tipo_venta = "credito";
            pedido.Condiciones_pago = "30 dias";
            pedido.SubTotal = 250m;
            pedido.Monto_Itbis = 45m;
            pedido.Total = 295m;
            pedido.Detalle = new List<PedidoDetalle>
            {
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "EDITADA_A",
                    Cant = 5m,
                    Unidad = "un",
                    Width = 11m,
                    Lenght = 21m,
                    Msi = 1m,
                    Precio = 40m,
                    Total_Renglon = 200m,
                    Notas = "LINEA_A_EDITADA"
                },
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "NUEVA_C",
                    Cant = 1m,
                    Unidad = "un",
                    Width = 12m,
                    Lenght = 22m,
                    Msi = 1m,
                    Precio = 50m,
                    Total_Renglon = 50m,
                    Notas = "LINEA_NUEVA"
                }
            };

            service.ActualizarPedidoCompleto(pedido).Should().BeTrue(because: service.ErrorMsg ?? "sin mensaje del servicio");

            // Encabezado releido de la base. Los decimales se comparan exactos: son enteros, no
            // sufren el redondeo de decimal(18,2).
            DataRow fila = (await service.LoadDataPedidos()).AsEnumerable()
                .Cast<DataRow>()
                .Single(r => r["numero"].ToString() == numero);

            fila["customer_name"].ToString().Should().Be("Cliente Editado");
            fila["notas"].ToString().Should().Be("NOTA_EDITADA");
            fila["prioridad"].ToString().Should().Be("urgente");
            fila["tipo_venta"].ToString().Should().Be("credito");
            fila["condiciones_pago"].ToString().Should().Be("30 dias");
            Convert.ToDecimal(fila["subtotal"]).Should().Be(250m);
            Convert.ToDecimal(fila["itbis"]).Should().Be(45m);
            Convert.ToDecimal(fila["total$"]).Should().Be(295m);
            Convert.ToDateTime(fila["fecha"]).Should().Be(new DateTime(2026, 4, 10));

            // Detalle reemplazado entero: dos lineas, la quitada no sobrevive y la conservada
            // quedo con la cantidad nueva.
            List<PedidoDetalle> lineas = PedidoDetalleMapper.Mapear(await service.LoadDataPedidoDetalle(numero));
            lineas.Should().HaveCount(2, "el detalle se borra y se vuelve a insertar completo");
            lineas.Should().NotContain(l => l.Notas == "LINEA_B", "la linea quitada no debe quedar en la base");
            lineas.Single(l => l.Notas == "LINEA_A_EDITADA").Cant.Should().Be(5m);
            lineas.Single(l => l.Notas == "LINEA_A_EDITADA").Product_name.Should().Be("EDITADA_A");
            lineas.Single(l => l.Notas == "LINEA_NUEVA").Cant.Should().Be(1m);

            // Actualizar no reserva numero: ni el contador ni la previsualizacion se mueven.
            LeerPar1().Should().Be(par1TrasGuardar, "actualizar un pedido no debe gastar el consecutivo");
            (await service.GetProximoNumeroPedido()).Should().Be(proximoAntes,
                "la previsualizacion no debe avanzar por editar");
        }
        finally
        {
            LimpiarPedido(pedido.Numero);
            RestaurarPar1(par1Antes);
        }
    }

    /// <summary>
    /// Un pedido que no existe no se puede editar: el UPDATE no toca ninguna fila y el servicio
    /// tiene que devolver false con el motivo en ErrorMsg, para que la pantalla no muestre un
    /// guardado exitoso que nunca ocurrio.
    /// </summary>
    [SkippableFact]
    public async Task ActualizarPedidoCompleto_PedidoInexistenteDevuelveFalse()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;

        int par1Antes = LeerPar1();

        // Numero generado por corrida, nunca fijo (950000/950001): la previsualizacion no
        // consume, asi que el numero sigue sin existir en la base y el DELETE del finally no
        // puede borrar un pedido real.
        string numeroInexistente = await service.GetProximoNumeroPedido();
        PedidoNumero.EsValido(numeroInexistente).Should().BeTrue();
        Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM pedido WHERE numero = @p1", ("@p1", numeroInexistente))!).Should().Be(0,
            "el numero previsualizado todavia no debe estar usado");

        Pedido pedido = new Pedido
        {
            Numero = numeroInexistente,
            Fecha = DateTime.Now,
            Customer_Id = customerId,
            Customer_Name = "Cliente Test",
            Estado = PedidoEstado.Creado,
            Detalle =
            {
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "Producto Test",
                    Cant = 1m,
                    Unidad = "un",
                    Precio = 10m,
                    Total_Renglon = 10m
                }
            }
        };

        try
        {
            service.ActualizarPedidoCompleto(pedido).Should().BeFalse(
                because: "no existe ningun pedido con ese numero");
            service.ErrorMsg.Should().NotBeNull(
                "el motivo del fallo tiene que quedar en ErrorMsg para que la pantalla lo muestre");

            // Ni se crea el pedido ni se mueve el consecutivo.
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM pedido WHERE numero = @p1", ("@p1", numeroInexistente))!).Should().Be(0);
            LeerPar1().Should().Be(par1Antes, "actualizar un pedido inexistente no debe gastar el consecutivo");
        }
        finally
        {
            RestaurarPar1(par1Antes);
        }
    }

    /// <summary>
    /// Una linea sin producto hace fallar la actualizacion entera: ni el encabezado que venia en
    /// el mismo objeto ni el detalle original pueden cambiar. Si el UPDATE se aplicara antes de
    /// validar, la operacion devolveria false y el encabezado quedaria editado a medias.
    /// </summary>
    [SkippableFact]
    public async Task ActualizarPedidoCompleto_LineaInvalida_HaceRollback()
    {
        IPedidoService service = CrearServicio();

        object? customerIdObj = _fixture.ExecuteScalar("SELECT TOP 1 customer_id FROM customer");
        Skip.If(customerIdObj == null, "no hay clientes; validado en prueba manual");
        object? productIdObj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(productIdObj == null, "no hay productos; validado en prueba manual");

        Guid customerId = Guid.Parse(customerIdObj.ToString()!);
        string productId = productIdObj.ToString()!;

        int par1Antes = LeerPar1();

        Pedido pedido = new Pedido
        {
            Fecha = new DateTime(2026, 3, 4),
            Customer_Id = customerId,
            Customer_Name = "Cliente Original",
            Estado = PedidoEstado.Creado,
            Notas = "NOTA_ORIGINAL",
            SubTotal = 150m,
            Porc_Itbis = 18m,
            Monto_Itbis = 27m,
            Total = 177m,
            Detalle =
            {
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "ORIGINAL_A",
                    Cant = 1m,
                    Unidad = "un",
                    Width = 10m,
                    Lenght = 20m,
                    Msi = 1m,
                    Precio = 100m,
                    Total_Renglon = 100m,
                    Notas = "LINEA_A"
                },
                new PedidoDetalle
                {
                    Product_id = productId,
                    Product_name = "ORIGINAL_B",
                    Cant = 2m,
                    Unidad = "un",
                    Width = 30m,
                    Lenght = 40m,
                    Msi = 1m,
                    Precio = 50m,
                    Total_Renglon = 100m,
                    Notas = "LINEA_B"
                }
            }
        };

        try
        {
            service.SavePedidoCompleto(pedido).Should().BeTrue(because: service.ErrorMsg ?? "sin mensaje del servicio");
            string numero = pedido.Numero;

            // La edicion invalida tambien trae cambios de encabezado: tienen que quedarse en
            // borrador, no aplicarse a medias.
            Pedido editado = new Pedido
            {
                Numero = numero,
                Fecha = new DateTime(2026, 7, 15),
                Customer_Id = customerId,
                Customer_Name = "Cliente Rollback",
                Estado = PedidoEstado.Creado,
                Notas = "NO_DEBE_GUARDARSE",
                Prioridad = "alta",
                Tipo_venta = "contado",
                SubTotal = 999m,
                Porc_Itbis = 18m,
                Monto_Itbis = 179.82m,
                Total = 1178.82m,
                Detalle =
                {
                    new PedidoDetalle
                    {
                        // Sin producto asignado: el validador rechaza la linea y toda la
                        // transaccion se revierte.
                        Product_id = null,
                        Product_name = "Linea Invalida",
                        Cant = 1m,
                        Unidad = "un",
                        Width = 10m,
                        Lenght = 20m,
                        Msi = 1m
                    }
                }
            };

            service.ActualizarPedidoCompleto(editado).Should().BeFalse(
                because: service.ErrorMsg ?? "una linea sin producto debe impedir la actualizacion");

            // Encabezado sin cambios: sigue siendo el del guardado original.
            DataRow fila = (await service.LoadDataPedidos()).AsEnumerable()
                .Cast<DataRow>()
                .Single(r => r["numero"].ToString() == numero);

            fila["customer_name"].ToString().Should().Be("Cliente Original",
                "el encabezado no debe cambiar si la operacion fallo");
            fila["notas"].ToString().Should().Be("NOTA_ORIGINAL");
            fila.IsNull("prioridad").Should().BeTrue(
                "las columnas que el pedido original no traia siguen vacias: la edicion fallida no las puebla");
            fila.IsNull("tipo_venta").Should().BeTrue();
            Convert.ToDateTime(fila["fecha"]).Should().Be(new DateTime(2026, 3, 4));
            Convert.ToDecimal(fila["subtotal"]).Should().Be(150m);
            Convert.ToDecimal(fila["total$"]).Should().Be(177m);

            // Detalle original intacto, sin la linea invalida entre medias.
            List<PedidoDetalle> lineas = PedidoDetalleMapper.Mapear(await service.LoadDataPedidoDetalle(numero));
            lineas.Should().HaveCount(2, "el detalle original tiene que seguir tal cual");
            lineas.Select(l => l.Notas).Should().BeEquivalentTo(new[] { "LINEA_A", "LINEA_B" });
            lineas.Should().NotContain(l => string.IsNullOrWhiteSpace(l.Product_id),
                "no debe haberse insertado ninguna linea sin producto");
        }
        finally
        {
            LimpiarPedido(pedido.Numero);
            RestaurarPar1(par1Antes);
        }
    }

    /// <summary>
    /// ActualizarPedidoCompleto tiene que rechazar un pedido invalido ANTES de abrir la
    /// conexion, igual que SavePedidoCompleto. El servicio se apunta a un servidor con
    /// credenciales invalidas: si ErrorMsg trae el motivo de la validacion (y no un error
    /// de red o de login), la validacion corrio antes de Open(). No escribe nada en la base,
    /// por eso no usa SkippableFact, Skip.If ni limpieza.
    /// </summary>
    [Fact]
    public void ActualizarPedidoCompleto_PedidoInvalido_SeRechazaSinAbrirConexion()
    {
        IConfiguration config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ambiente"] = "Desarrollo",
                ["ConnectionStringsEnvironment:Desarrollo"] =
                    "Server=localhost;Database=Ritrama_NoExiste_Pruebas;User Id=sa;Password=NoAbre;TrustServerCertificate=True;Connect Timeout=2"
            })
            .Build();
        IPedidoService service = new PedidoService(config);

        Pedido pedido = new Pedido
        {
            Numero = "SO-07770",
            Fecha = new DateTime(2026, 9, 25),
            Customer_Id = Guid.Empty,
            Estado = PedidoEstado.Creado,
            Detalle =
            {
                new PedidoDetalle { Product_id = "P001", Cant = 1m, Precio = 10m }
            }
        };

        service.ActualizarPedidoCompleto(pedido).Should().BeFalse(
            "un pedido sin cliente no debe llegar ni a tocar la base");
        service.ErrorMsg.Should().Contain("cliente",
            "el motivo tiene que ser el de la validacion y no un error de conexion: solo asi "
            + "se demuestra que la validacion corrio antes de Open()");
    }
}
