using System.Data;
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de integracion del modulo Orden de Corte contra el SQL Server de pruebas/dev.
/// Cubren los cambios de rendimiento y transaccionalidad realizados.
/// </summary>
[Trait("Categoria", "Integracion")]
public class OrdenCorteServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public OrdenCorteServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    [Fact]
    public async Task LoadDataOC_NoIncluyeOrdenesAnuladasNiCerradas()
    {
        DataSet ds = await _fixture.Service.LoadDataOC();
        DataTable? dt = ds.Tables["DtMaster"];
        dt.Should().NotBeNull();

        foreach (DataRow row in dt!.Rows)
        {
            Convert.ToInt32(row["anulada"]).Should().Be(0, "el filtro debe excluir las ordenes anuladas");
        }

        // El numero de filas debe coincidir con las ordenes activas (anulada=0 Y CloseDocument=0) en la BD.
        int activas = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM orden_corte WHERE anulada = 0 AND CloseDocument = 0")!);
        dt.Rows.Count.Should().Be(activas, "LoadDataOC debe traer solo ordenes activas (filtro anulada/CloseDocument)");
    }

    [SkippableFact]
    public async Task BuscarRollId_ExcluyeRollosConsumidos()
    {
        DataTable dt = await _fixture.Service.BuscarRollId("Roll_Id", "");

        Skip.If(dt.Rows.Count == 0, "no hay master rolls en la BD; validado en prueba manual");

        foreach (DataRow row in dt.Rows)
        {
            Convert.ToDecimal(row["largo_restante"]).Should().BeGreaterThan(
                100, "el browser de master debe excluir los rollos con restante <= 100 pies (se consideran desperdicio)");
        }
    }

    [Fact]
    public void Consecutivo_RoundTrip_IncrementaEnUno()
    {
        int before = _fixture.Service.BuscarConsecOC();
        _fixture.Service.UpdateConsecOC((before + 1).ToString());
        int after = _fixture.Service.BuscarConsecOC();
        after.Should().Be(before + 1);

        // restaurar el valor original del contador
        _fixture.Service.UpdateConsecOC(before.ToString());
        _fixture.Service.BuscarConsecOC().Should().Be(before);
    }

    [Fact]
    public void GuardarEncabezadoOrdenCorte_DatosInvalidosDevuelveFalse()
    {
        int consecBefore = _fixture.Service.BuscarConsecOC();
        Orden orden = new Orden
        {
            // fechas validas para no disparar SqlTypeException por DateTime.MinValue
            Fecha = DateTime.Now,
            Fecha_produccion = DateTime.Now,
            LastUpdate = DateTime.Now,
            FechaAutorize = DateTime.Now
        };
        bool result = _fixture.Service.GuardarEncabezadoOrdenCorte(orden);
        result.Should().BeFalse();

        // a nivel de servicio un guardado fallido no avanza el consecutivo
        _fixture.Service.BuscarConsecOC().Should().Be(consecBefore);
    }

    [SkippableFact]
    public async Task ActualizarInventariosMasterAsync_Commit_IncrementaYMarcaDisponible()
    {
        // Selecciona un master con inventario real suficiente (Lenght − consumos ya
        // registrados ≥ 2 pies) para no chocar con la REGLA RN-CONSUMO-RESTANTE.
        object? rollidObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 m.Roll_Id FROM MasterInic m " +
            "LEFT JOIN MasterDetailsInic d ON d.rollid = m.Roll_Id " +
            "WHERE m.Roll_Id IS NOT NULL AND m.anulado = 0 " +
            "GROUP BY m.Roll_Id, m.Lenght " +
            "HAVING m.Lenght - ISNULL(SUM(d.consumo), 0) >= 2.0");
        Skip.If(rollidObj == null, "no hay master rolls con inventario suficiente; validado en prueba manual");

        object? numeroObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 a.numero FROM orden_corte a JOIN rolls_details r ON r.numero = a.numero " +
            "WHERE a.anulada = 0 AND a.CloseDocument = 0");
        Skip.If(numeroObj == null, "no hay orden con rollos; validado en prueba manual");

        string rollid = rollidObj.ToString()!;
        string numero = numeroObj.ToString()!;

        decimal before = Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(largo_consumido, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!);
        int dispBefore = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT ISNULL(disponible, 0) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!);

        // Idempotencia: si el detalle (rollid, orden) ya existe (etiquetado o corrida previa)
        // el service NO vuelve a descontar. Para probar el incremento limpiamos el detalle
        // previo de esa orden/master y asi el consumo SI se aplica.
        _fixture.ExecuteNonQuery("DELETE FROM MasterDetailsInic WHERE rollid = @p1 AND orden = @p2", ("@p1", rollid), ("@p2", numero));

        _fixture.ExecuteNonQuery("UPDATE rolls_details SET disponible = 0 WHERE numero = @p1", ("@p1", numero));

        bool ok = await _fixture.Service.ActualizarInventariosMasterAsync(
            rollid, numero, 1.0, 0, false, "INIC.", numero);
        ok.Should().BeTrue();

        decimal after = Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(largo_consumido, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!);
        after.Should().BeApproximately(before + 1.0m, 0.001m);

        int dispAfter = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT ISNULL(disponible, 0) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!);
        dispAfter.Should().Be(1);

        // restaurar el estado original para no dejar rastro en la BD de pruebas
        _fixture.ExecuteNonQuery(
            "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", before), ("@p2", rollid));
        _fixture.ExecuteNonQuery(
            "UPDATE rolls_details SET disponible = @p1 WHERE numero = @p2", ("@p1", dispBefore), ("@p2", numero));
        _fixture.ExecuteNonQuery(
            "DELETE FROM MasterDetailsInic WHERE rollid = @p1 AND orden = @p2", ("@p1", rollid), ("@p2", numero));
    }

    [SkippableFact]
    public async Task ActualizarInventariosMasterAsync_Idempotente_NoVuelveADescontarSiYaRegistrado()
    {
        object? rollidObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 Roll_Id FROM MasterInic WHERE Roll_Id IS NOT NULL");
        Skip.If(rollidObj == null, "no hay master rolls; validado en prueba manual");

        object? numeroObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 a.numero FROM orden_corte a JOIN rolls_details r ON r.numero = a.numero " +
            "WHERE a.anulada = 0 AND a.CloseDocument = 0");
        Skip.If(numeroObj == null, "no hay orden con rollos; validado en prueba manual");

        string rollid = rollidObj.ToString()!;
        string numero = numeroObj.ToString()!;

        decimal antes = Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(largo_consumido, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!);

        // Forzar el escenario de doble cierre: registramos el detalle (rollid, orden)
        // manualmente y luego llamamos al metodo: NO debe volver a descontar stock.
        _fixture.ExecuteNonQuery(
            "INSERT INTO MasterDetailsInic (rollid, orden, consumo, fecha_reg, desperdicio) VALUES (@p1, @p2, 1.0, GETDATE(), 0)",
            ("@p1", rollid), ("@p2", numero));

        bool ok = await _fixture.Service.ActualizarInventariosMasterAsync(
            rollid, numero, 1.0, 0, false, "INIC.", numero);
        ok.Should().BeTrue();

        decimal despues = Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(largo_consumido, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!);
        despues.Should().BeApproximately(antes, 0.001m, "con el detalle ya registrado el cierre no vuelve a descontar stock");

        _fixture.ExecuteNonQuery(
            "DELETE FROM MasterDetailsInic WHERE rollid = @p1 AND orden = @p2", ("@p1", rollid), ("@p2", numero));
    }

    [SkippableFact]
    public void Update_Header_Documnet_OC_Rollback_RevierteDeleteAnteError()
    {
        // orden activa existente que tenga cortes
        object? numeroObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 a.numero FROM orden_corte a JOIN cortes c ON c.orden = a.numero " +
            "WHERE a.anulada = 0 AND a.CloseDocument = 0");
        Skip.If(numeroObj == null, "no hay orden activa con cortes; validado en prueba manual");
        int numero = Convert.ToInt32(numeroObj);

        int cortesAntes = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM cortes WHERE orden = @p1", ("@p1", numero))!);
        int rollosAntes = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!);
        Skip.If(cortesAntes == 0, "no hay cortes asociados a la orden; validado en prueba manual");

        string ubicAntes = (_fixture.ExecuteScalar(
            "SELECT ISNULL(ubicacion, '') FROM orden_corte WHERE numero = @p1", ("@p1", numero)) ?? "").ToString()!;

        // Forzamos un SqlException real: cortes.width es decimal(18,5) y double.MaxValue desborda.
        // El catch(Exception) debe capturarlo, hacer Rollback y reportar el error (sin MessageBox).
        bool reportado = false;
        Action<string> reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            Orden orden = new Orden
            {
                Numero = numero,
                Fecha = DateTime.Now,
                Fecha_produccion = DateTime.Now,
                LastUpdate = DateTime.Now,
                FechaAutorize = DateTime.Now,
                Ubicacion = "ZZZ_NO_DEBE_PERSISTIR",
                Rollid_1 = "X",
                Product_id = "X",
                SellOrder = "X",
                Width_1 = 1,
                Lenght_1 = 1,
                Util1_Real_Width = 1,
                Util1_real_Lenght = 1,
                Rest1_width = 1,
                Rest1_lenght = 1,
                Desperdicio = false,
                Operador_id = Guid.NewGuid(),
                Customer_Id = Guid.NewGuid(),
                Cortes_Largo = 1,
                Longitud_Cortar = 1,
                Cortes_Ancho = 1,
                Cantidad_Rollos = 1,
                ConfigVueltas = false,
                Cortes = new List<Corte> { new Corte { Numero = 999999, Orden = numero, Width = double.MaxValue, Length = 1, Msi = 1 } },
                rollos = new List<RolloCortado>()
            };

            _fixture.Service.Update_Header_Documnet_OC(orden);
        }
        finally
        {
            ServiceErrors.Report = reporterOriginal;
        }

        reportado.Should().BeTrue("el error debe reportarse al llamador");

        int cortesDespues = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM cortes WHERE orden = @p1", ("@p1", numero))!);
        int rollosDespues = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!);
        string ubicDespues = (_fixture.ExecuteScalar(
            "SELECT ISNULL(ubicacion, '') FROM orden_corte WHERE numero = @p1", ("@p1", numero)) ?? "").ToString()!;

        cortesDespues.Should().Be(cortesAntes, "el DELETE de cortes debe revertirse dentro de la transaccion");
        rollosDespues.Should().Be(rollosAntes);
        ubicDespues.Should().Be(ubicAntes, "el UPDATE del encabezado no debe persistir (rollback)");
    }

    [Fact]
    public void AplicarReglaRestanteOC_CasoOC4625_RecalculaRestanteCeroA115()
    {
        // REGLA RN-RESTANTE-OC: caso real OC 4625 (master 243058320006, largo 20115.00,
        // consumo 20000.00). Al editar se archivaba rest1_lenght=0 y restante_rollid1='335,00'
        // en vez de 115.00 / '115,00'. La regla fuerza el restante = largo - consumo.
        Orden orden = CrearOrdenValida(0);
        orden.Lenght_1 = 20115;
        orden.Util1_real_Lenght = 20000;
        orden.Desperdicio = false;
        orden.Rest1_lenght = 0;
        orden.Restante_rollid1 = "335,00";

        _fixture.Service.AplicarReglaRestanteOC(orden);

        orden.Rest1_lenght.Should().Be(115.0, "restante = largo del master - consumo de la OC");
        orden.Restante_rollid1.Should().Be("115,00", "el texto archivado debe reflejar el restante correcto");
    }

    [Fact]
    public void AplicarReglaRestanteOC_NoDosMasters_DejaSegundoMasterEnCero()
    {
        Orden orden = CrearOrdenValida(0);
        orden.Lenght_1 = 500;
        orden.Util1_real_Lenght = 400;
        orden.Desperdicio = false;

        _fixture.Service.AplicarReglaRestanteOC(orden);

        orden.Rest1_lenght.Should().Be(100.0);
        orden.Rest2_lenght.Should().Be(0.0, "sin TwoMasters no hay restante para el master 2");
        orden.Restante_rollid2.Should().Be("0,00");
    }

    [Fact]
    public void AplicarReglaRestanteOC_ConDesperdicio_DejaRestanteCero()
    {
        Orden orden = CrearOrdenValida(0);
        orden.Lenght_1 = 20115;
        orden.Util1_real_Lenght = 20000;
        orden.Desperdicio = true;

        _fixture.Service.AplicarReglaRestanteOC(orden);

        orden.Rest1_lenght.Should().Be(0.0, "una OC de desperdicio consume todo el master");
        orden.Restante_rollid1.Should().Be("0,00");
    }

    [Fact]
    public void AplicarReglaRestanteOC_DosMasters_RecalculaAmbos()
    {
        Orden orden = CrearOrdenValida(0);
        orden.Lenght_1 = 20115;
        orden.Util1_real_Lenght = 20000;
        orden.Desperdicio = false;
        orden.TwoMasters = true;
        orden.Rollid_2 = "Y";
        orden.Lenght_2 = 1000;
        orden.Util2_real_Lenght = 900;
        orden.Desperdicio2 = false;

        _fixture.Service.AplicarReglaRestanteOC(orden);

        orden.Rest1_lenght.Should().Be(115.0);
        orden.Restante_rollid1.Should().Be("115,00");
        orden.Rest2_lenght.Should().Be(100.0, "master 2: largo 1000 - consumo 900");
        orden.Restante_rollid2.Should().Be("100,00");
    }

    [Fact]
    public void GetAndIncrementConsecOC_EsAtomicoYAvanza()
    {
        // P0-2: el consecutivo se obtiene y avanza en una sola operacion atómica (UPDATE + OUTPUT),
        // sin ventana de carrera entre leer y escribir el contador.
        int antes = _fixture.Service.BuscarConsecOC();

        int n1 = _fixture.Service.GetAndIncrementConsecOC();
        int n2 = _fixture.Service.GetAndIncrementConsecOC();

        n1.Should().Be(antes, "devuelve el valor previo al incremento");
        n2.Should().Be(antes + 1, "avanza de a uno");
        n1.Should().NotBe(n2);

        _fixture.Service.BuscarConsecOC().Should().Be(antes + 2, "el contador avanzo en 2");

        // restaurar el contador original para no dejar rastro
        _fixture.Service.UpdateConsecOC(antes.ToString());
        _fixture.Service.BuscarConsecOC().Should().Be(antes);
    }

    private static Orden CrearOrdenValida(int numero)
    {
        return new Orden
        {
            Numero = numero,
            Fecha = DateTime.Now,
            Fecha_produccion = DateTime.Now,
            LastUpdate = DateTime.Now,
            FechaAutorize = DateTime.Now,
            Lenght_entrada = 0,
            Resta_entrada = 0,
            Salida_pies = 0,
            Rollid_1 = "X",
            Width_1 = 1,
            Lenght_1 = 1,
            Descartable1_pies = 0,
            Plus1_pies = 0,
            Master_lenght1_Real = 0,
            Util1_Real_Width = 1,
            Util1_real_Lenght = 1,
            Rest1_width = 1,
            Rest1_lenght = 1,
            Tipo_Mov1 = "X",
            Rollid_2 = "X",
            Width_2 = 1,
            Lenght_2 = 1,
            Descartable2_pies = 0,
            Plus2_pies = 0,
            Master_lenght2_Real = 0,
            Util2_Real_Width = 1,
            Util2_real_Lenght = 1,
            Rest2_width = 1,
            Rest2_lenght = 1,
            Tipo_Mov2 = "X",
            Product_id = "X",
            Product_name = "X",
            Anulada = false,
            Procesado = false,
            CloseDocument = false,
            Total_Inch_Ancho = 1,
            Longitud_Cortar = 1,
            Longitud_Cortar2 = 0,
            Cortes_Ancho = 1,
            Cortes_Largo = 1,
            Cortes_Largo2 = 0,
            Cantidad_Rollos = 1,
            Cantidad_Rollos2 = 0,
            STATE = 0,
            Rebobinado = false,
            Real_usado_r1 = 0,
            Real_usado_r2 = 0,
            Rollo_unificado = false,
            Operador_id = Guid.NewGuid(),
            Nombre_operador = "X",
            Rollid_oculto = "X",
            Restante_rollid1 = "X",
            Restante_rollid2 = "X",
            Customer_Id = Guid.NewGuid(),
            Customer_Name = "X",
            Lenght_Master_Real = 0,
            Step = 0,
            ToAutorize = "0",
            Note = "",
            SellOrder = "X",
            Desperdicio = false,
            Desperdicio2 = false,
            Master_Tipo = "X",
            Ubicacion = "X",
            ConfigVueltas = false,
            TwoMasters = false,
            Vueltas2 = 0,
            Cantidad_rollos2 = 0,
            MachineName = "X",
            DireccionIp = "X"
        };
    }

    [SkippableFact]
    public void GuardarOrdenCompleta_MarcaDocumentoDeLaOCEnElMasterUsado()
    {
        // Cruce master <-> OC: al guardar la OC el master usado queda con documento = numero de la OC.
        object? rollidObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 Roll_Id FROM MasterInic WHERE Roll_Id IS NOT NULL");
        Skip.If(rollidObj == null, "no hay master rolls; validado en prueba manual");
        string rollid = rollidObj.ToString()!;

        int numero = _fixture.Service.GetAndIncrementConsecOC();
        int documentoBefore = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT ISNULL(documento, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!);
        try
        {
            Orden orden = CrearOrdenValida(numero);
            orden.Rollid_1 = rollid;
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = rollid, Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };

            _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos).Should().BeTrue();

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT ISNULL(documento, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!)
                .Should().Be(numero, "el master usado debe quedar marcado con el numero de la OC");

            object? itemsRollidObj = _fixture.ExecuteScalar(
                "SELECT TOP 1 rollid FROM ItemsMateria WHERE rollid IS NOT NULL");
            if (itemsRollidObj != null)
            {
                string itemsRollid = itemsRollidObj.ToString()!;
                int numero2 = _fixture.Service.GetAndIncrementConsecOC();
                int itemsDocBefore = Convert.ToInt32(_fixture.ExecuteScalar(
                    "SELECT ISNULL(documento, 0) FROM ItemsMateria WHERE rollid = @p1", ("@p1", itemsRollid))!);
                try
                {
                    Orden orden2 = CrearOrdenValida(numero2);
                    orden2.Rollid_1 = itemsRollid;
                    _fixture.Service.GuardarEncabezadoOrdenCorte(orden2).Should().BeTrue();

                    Convert.ToInt32(_fixture.ExecuteScalar(
                        "SELECT ISNULL(documento, 0) FROM ItemsMateria WHERE rollid = @p1", ("@p1", itemsRollid))!)
                        .Should().Be(numero2, "el master por compras usado debe quedar marcado con el numero de la OC");
                }
                finally
                {
                    _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero2));
                    _fixture.Service.UpdateConsecOC(numero2.ToString());
                    _fixture.ExecuteNonQuery(
                        "UPDATE ItemsMateria SET documento = @p1 WHERE rollid = @p2", ("@p1", itemsDocBefore), ("@p2", itemsRollid));
                }
            }
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM cortes WHERE orden = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET documento = @p1 WHERE Roll_Id = @p2", ("@p1", documentoBefore), ("@p2", rollid));
            _fixture.Service.UpdateConsecOC(numero.ToString());
        }
    }

    [Fact]
    public void GuardarOrdenCompleta_GuardaTodoEnUnaTransaccion()
    {
        // P0-1 (happy path): encabezado + cortes + rollos se persisten juntos.
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        try
        {
            Orden orden = CrearOrdenValida(numero);
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = "X", Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };

            bool ok = _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos);
            ok.Should().BeTrue();

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM orden_corte WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM cortes WHERE orden = @p1", ("@p1", numero))!).Should().Be(1);
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM cortes WHERE orden = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero));
            _fixture.Service.UpdateConsecOC(numero.ToString());
        }
    }

    [Fact]
    public void GuardarOrdenCompleta_PersisteTotalSalidaConSumaDeAnchos()
    {
        // ACC-03 (4606): total_salida se guardaba en 0 por desalineacion de parametros
        // en el INSERT; debe persistir la suma de anchos de los cortes.
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        try
        {
            Orden orden = CrearOrdenValida(numero);
            orden.Total_Inch_Ancho = 60;
            List<Corte> cortes = new List<Corte>
            {
                new Corte { Numero = 1, Orden = numero, Width = 20, Length = 1, Msi = 1 },
                new Corte { Numero = 2, Orden = numero, Width = 40, Length = 1, Msi = 1 }
            };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 20, Length = 1, Msi = 1, Roll_Id = "X", Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };

            bool ok = _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos);
            ok.Should().BeTrue();

            double persistido = Convert.ToDouble(_fixture.ExecuteScalar(
                "SELECT total_salida FROM orden_corte WHERE numero = @p1", ("@p1", numero))!);
            persistido.Should().Be(60.0, "total_salida debe quedar con la suma de anchos de los cortes");
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM cortes WHERE orden = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero));
            _fixture.Service.UpdateConsecOC(numero.ToString());
        }
    }

    #region Reasignacion de master (sustitucion de materia prima)

    private string? ObtenerMasterInicExcluyendo(string? excluir)
    {
        object? obj = _fixture.ExecuteScalar(
            "SELECT TOP 1 Roll_Id FROM MasterInic WHERE Roll_Id IS NOT NULL AND (@excluir IS NULL OR Roll_Id <> @excluir)",
            ("@excluir", (object?)excluir ?? DBNull.Value));
        return obj?.ToString();
    }

    private string? ObtenerMasterItemsMateria()
    {
        object? obj = _fixture.ExecuteScalar(
            "SELECT TOP 1 rollid FROM ItemsMateria WHERE rollid IS NOT NULL");
        return obj?.ToString();
    }

    private decimal LargoConsumidoMasterInic(string rollid)
        => Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(largo_consumido, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!);

    private decimal LargoConsumidoItemsMateria(string rollid)
        => Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(largo_consumido, 0) FROM ItemsMateria WHERE rollid = @p1", ("@p1", rollid))!);

    private decimal SumaDetalleReasignacion(string orden, string rollid)
        => Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(SUM(consumo), 0) FROM MasterDetailsInic WHERE orden = @p1 AND rollid = @p2",
            ("@p1", orden), ("@p2", rollid))!);

    private void BorrarOrdenTest(int numero)
    {
        _fixture.ExecuteNonQuery("DELETE FROM MasterDetailsInic WHERE orden = @p1", ("@p1", numero));
        _fixture.ExecuteNonQuery("DELETE FROM cortes WHERE orden = @p1", ("@p1", numero));
        _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
        _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero));
        _fixture.Service.UpdateConsecOC(numero.ToString());
    }

    [SkippableFact]
    public async Task ReasignarConsumoMaster_DevuelveAlAnteriorYDescuentaAlNuevo()
    {
        string? anterior = ObtenerMasterInicExcluyendo(null);
        string? nuevo = ObtenerMasterInicExcluyendo(anterior);
        Skip.If(anterior == null || nuevo == null, "se requieren al menos 2 masters iniciales; validado en prueba manual");

        int numero = _fixture.Service.GetAndIncrementConsecOC();
        decimal antesAnterior = 0, antesNuevo = 0;
        try
        {
            Orden orden = CrearOrdenValida(numero);
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = anterior, Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };
            _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos).Should().BeTrue();

            antesAnterior = LargoConsumidoMasterInic(anterior);
            antesNuevo = LargoConsumidoMasterInic(nuevo);

            bool ok = await _fixture.Service.ReasignarConsumoMasterAsync(
                numero.ToString(), anterior, nuevo, 10, 0, false, "INIC.", "INIC.");
            ok.Should().BeTrue();

            LargoConsumidoMasterInic(anterior).Should().BeApproximately(antesAnterior - 10m, 0.001m,
                "el consumo debe devolverse al master anterior");
            LargoConsumidoMasterInic(nuevo).Should().BeApproximately(antesNuevo + 10m, 0.001m,
                "el consumo debe descontarse del master nuevo");
            SumaDetalleReasignacion(numero.ToString(), anterior).Should().BeApproximately(-10m, 0.001m,
                "el detalle debe registrar la devolucion negativa del master anterior");
            SumaDetalleReasignacion(numero.ToString(), nuevo).Should().BeApproximately(10m, 0.001m,
                "el detalle debe registrar el consumo del master nuevo");
        }
        finally
        {
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", antesAnterior), ("@p2", anterior));
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", antesNuevo), ("@p2", nuevo));
            BorrarOrdenTest(numero);
        }
    }

    [SkippableFact]
    public async Task ReasignarConsumoMaster_ConDesperdicio_TrasladaConsumoYDesperdicio()
    {
        string? anterior = ObtenerMasterInicExcluyendo(null);
        string? nuevo = ObtenerMasterInicExcluyendo(anterior);
        Skip.If(anterior == null || nuevo == null, "se requieren al menos 2 masters iniciales; validado en prueba manual");

        int numero = _fixture.Service.GetAndIncrementConsecOC();
        decimal antesAnterior = 0, antesNuevo = 0;
        try
        {
            Orden orden = CrearOrdenValida(numero);
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = anterior, Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };
            _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos).Should().BeTrue();

            antesAnterior = LargoConsumidoMasterInic(anterior);
            antesNuevo = LargoConsumidoMasterInic(nuevo);

            bool ok = await _fixture.Service.ReasignarConsumoMasterAsync(
                numero.ToString(), anterior, nuevo, 10, 5, true, "INIC.", "INIC.");
            ok.Should().BeTrue();

            LargoConsumidoMasterInic(anterior).Should().BeApproximately(antesAnterior - 15m, 0.001m,
                "se debe devolver consumo + desperdicio al master anterior");
            LargoConsumidoMasterInic(nuevo).Should().BeApproximately(antesNuevo + 15m, 0.001m,
                "se debe descontar consumo + desperdicio del master nuevo");
            SumaDetalleReasignacion(numero.ToString(), anterior).Should().BeApproximately(-15m, 0.001m);
            SumaDetalleReasignacion(numero.ToString(), nuevo).Should().BeApproximately(15m, 0.001m);
        }
        finally
        {
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", antesAnterior), ("@p2", anterior));
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", antesNuevo), ("@p2", nuevo));
            BorrarOrdenTest(numero);
        }
    }

    [SkippableFact]
    public async Task ReasignarConsumoMaster_InicAPorCompras_ActualizaInventariosDistintos()
    {
        string? anterior = ObtenerMasterInicExcluyendo(null);
        Skip.If(anterior == null, "se requiere un master inicial; validado en prueba manual");

        int numero = _fixture.Service.GetAndIncrementConsecOC();

        // ItemsMateria puede estar vacia en la BD de pruebas; si no hay un master por compras
        // se siembra una fila temporal (rollid "TC<numero>") que se elimina en el finally.
        string? nuevo = ObtenerMasterItemsMateria();
        bool materiaSembrada = false;
        if (nuevo == null)
        {
            nuevo = "TC" + numero;
            _fixture.ExecuteNonQuery(
                "INSERT INTO ItemsMateria (numero, product_id, cant_pedido, cant_real, width, length, msi, rollid, " +
                "splice, ubicacion, core, largo_consumido, largo_restante) " +
                "SELECT @num, 'X', 1, 1, 1, 1, 1, @roll, 0, 'X', 0, 0, 0 " +
                "WHERE NOT EXISTS (SELECT 1 FROM ItemsMateria WHERE rollid = @roll)",
                ("@num", "TEST-" + numero), ("@roll", nuevo));
            materiaSembrada = true;
        }

        decimal antesAnterior = 0, antesNuevo = 0;
        try
        {
            Orden orden = CrearOrdenValida(numero);
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = anterior, Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };
            _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos).Should().BeTrue();

            antesAnterior = LargoConsumidoMasterInic(anterior);
            antesNuevo = LargoConsumidoItemsMateria(nuevo);

            bool ok = await _fixture.Service.ReasignarConsumoMasterAsync(
                numero.ToString(), anterior, nuevo, 10, 0, false, "INIC.", "Por Compras");
            ok.Should().BeTrue();

            LargoConsumidoMasterInic(anterior).Should().BeApproximately(antesAnterior - 10m, 0.001m,
                "el master inicial debe devolver su consumo");
            LargoConsumidoItemsMateria(nuevo).Should().BeApproximately(antesNuevo + 10m, 0.001m,
                "el master por compras debe descontar el consumo");
            SumaDetalleReasignacion(numero.ToString(), anterior).Should().BeApproximately(-10m, 0.001m);
            SumaDetalleReasignacion(numero.ToString(), nuevo).Should().BeApproximately(10m, 0.001m);
        }
        finally
        {
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", antesAnterior), ("@p2", anterior));
            if (materiaSembrada)
            {
                _fixture.ExecuteNonQuery("DELETE FROM ItemsMateria WHERE rollid = @p1", ("@p1", nuevo));
            }
            else
            {
                _fixture.ExecuteNonQuery(
                    "UPDATE ItemsMateria SET largo_consumido = @p1 WHERE rollid = @p2", ("@p1", antesNuevo), ("@p2", nuevo));
            }
            BorrarOrdenTest(numero);
        }
    }

    [SkippableFact]
    public async Task ReasignarConsumoMaster_FallaEnDesperdicio_RevierteTodo()
    {
        // Solo aplica si alguna columna de inventario es decimal/numeric (permite forzar
        // overflow real con double.MaxValue y verificar que la transaccion hace rollback).
        int colNumeric = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM INFORMATION_SCHEMA.COLUMNS " +
            "WHERE (TABLE_NAME='MasterInic' AND COLUMN_NAME='largo_consumido' " +
            "       OR TABLE_NAME='ItemsMateria' AND COLUMN_NAME='largo_consumido' " +
            "       OR TABLE_NAME='MasterDetailsInic' AND COLUMN_NAME='consumo') " +
            "AND DATA_TYPE IN ('decimal','numeric')")!);
        Skip.If(colNumeric == 0, "no hay columnas decimales para forzar el overflow; validado en prueba manual");

        string? anterior = ObtenerMasterInicExcluyendo(null);
        string? nuevo = ObtenerMasterInicExcluyendo(anterior);
        Skip.If(anterior == null || nuevo == null, "se requieren al menos 2 masters iniciales; validado en prueba manual");

        int numero = _fixture.Service.GetAndIncrementConsecOC();
        decimal antesAnterior = 0, antesNuevo = 0;
        bool reportado = false;
        Action<string> reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            Orden orden = CrearOrdenValida(numero);
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = anterior, Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };
            _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos).Should().BeTrue();

            antesAnterior = LargoConsumidoMasterInic(anterior);
            antesNuevo = LargoConsumidoMasterInic(nuevo);

            // el consumo real (10) se aplica y despues el desperdicio (double.MaxValue) desborda
            // la columna decimal -> SqlException -> el Rollback debe revertir tambien los 10.
            bool ok = await _fixture.Service.ReasignarConsumoMasterAsync(
                numero.ToString(), anterior, nuevo, 10, double.MaxValue, true, "INIC.", "INIC.");
            ok.Should().BeFalse("un fallo intermedio debe abortar la reasignacion");
            reportado.Should().BeTrue();

            LargoConsumidoMasterInic(anterior).Should().BeApproximately(antesAnterior, 0.001m,
                "la devolucion al master anterior debe revertirse (rollback)");
            LargoConsumidoMasterInic(nuevo).Should().BeApproximately(antesNuevo, 0.001m,
                "el descuento del master nuevo debe revertirse (rollback)");
            SumaDetalleReasignacion(numero.ToString(), anterior).Should().Be(0m);
            SumaDetalleReasignacion(numero.ToString(), nuevo).Should().Be(0m);
        }
        finally
        {
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", antesAnterior), ("@p2", anterior));
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p1 WHERE Roll_Id = @p2", ("@p1", antesNuevo), ("@p2", nuevo));
            BorrarOrdenTest(numero);
            ServiceErrors.Report = reporterOriginal;
        }
    }

    #endregion

    [Fact]
    public void GuardarOrdenCompleta_FalloEnCortes_NoDejaOrdenHuerfana()
    {
        // P0-1 (atomicidad): si falla un corte, toda la transaccion se revierte y NO queda
        // el encabezado de la orden ni los rollos insertados parcialmente.
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        bool reportado = false;
        Action<string> reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            Orden orden = CrearOrdenValida(numero);
            // cortes.width es decimal(18,5): double.MaxValue desborda y fuerza SqlException real.
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = double.MaxValue, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = "X", Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };

            bool ok = _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos);
            ok.Should().BeFalse("un corte invalido debe abortar el guardado");
            reportado.Should().BeTrue("el error debe reportarse");

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM orden_corte WHERE numero = @p1", ("@p1", numero))!).Should().Be(0,
                "el encabezado no debe persistir (rollback total)");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM cortes WHERE orden = @p1", ("@p1", numero))!).Should().Be(0);
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!).Should().Be(0);
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM cortes WHERE orden = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero));
            _fixture.Service.UpdateConsecOC(numero.ToString());
            ServiceErrors.Report = reporterOriginal;
        }
    }

    [Fact]
    public void GuardarOrdenCompleta_NumeroPorAsignar_FalloNoAvanzaConsecutivo()
    {
        // Regla de produccion: el consecutivo de Ordenes de Corte no debe tener saltos.
        // El incremento ocurre DENTRO de la transaccion, por lo que un guardado fallido
        // (orden con Numero=0 => el servicio lo asigna internamente) deja el contador intacto.
        int consecBefore = _fixture.Service.BuscarConsecOC();
        bool reportado = false;
        Action<string> reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            Orden orden = CrearOrdenValida(0);
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = 0, Width = double.MaxValue, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = "X", Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = "0", Vuelta = 1
                }
            };

            bool ok = _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos);
            ok.Should().BeFalse("un corte invalido debe abortar el guardado");
            reportado.Should().BeTrue("el error debe reportarse");

            // el consecutivo NO avanzo: si el guardado falla, el numero queda disponible
            _fixture.Service.BuscarConsecOC().Should().Be(consecBefore,
                "un guardado fallido no debe quemar el consecutivo (sin saltos)");
        }
        finally
        {
            ServiceErrors.Report = reporterOriginal;
        }
    }

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void GuardarOrdenCompleta_RechazaOrdenSinCortesOSinRollos(bool vaciarCortes, bool vaciarRollos)
    {
        // P0-1 (integridad): no se puede guardar una orden incompleta (sin cortes o sin
        // detalle de rollos) aun pasando el header; se rechaza y NO queda encabezado huerfano.
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        bool reportado = false;
        Action<string> reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            Orden orden = CrearOrdenValida(numero);
            List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            List<RolloCortado> rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = "X", Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };

            List<Corte> cortesArg = vaciarCortes ? new List<Corte>() : cortes;
            List<RolloCortado> rollosArg = vaciarRollos ? new List<RolloCortado>() : rollos;

            bool ok = _fixture.Service.GuardarOrdenCompleta(orden, cortesArg, rollosArg);
            ok.Should().BeFalse("una orden sin cortes o sin detalle de rollos no debe guardarse");
            reportado.Should().BeTrue("debe notificarse al usuario el motivo del rechazo");

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM orden_corte WHERE numero = @p1", ("@p1", numero))!).Should().Be(0,
                "el encabezado no debe persistir (orden incompleta rechazada)");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM cortes WHERE orden = @p1", ("@p1", numero))!).Should().Be(0);
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!).Should().Be(0);
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM cortes WHERE orden = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
            _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero));
            _fixture.Service.UpdateConsecOC(numero.ToString());
            ServiceErrors.Report = reporterOriginal;
        }
    }

    private int CrearOrdenTemporal(out Orden orden)
    {
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        orden = CrearOrdenValida(numero);
        List<Corte> cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
        List<RolloCortado> rollos = new List<RolloCortado>
        {
            new RolloCortado
            {
                Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "UC" + numero, Splice = 0,
                Width = 1, Length = 1, Msi = 1, Roll_Id = "X", Code_Person = "X", Status = "X",
                Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
            }
        };
        _fixture.Service.GuardarOrdenCompleta(orden, cortes, rollos).Should().BeTrue();
        return numero;
    }

    private void EliminarOrdenTemporal(int numero)
    {
        _fixture.ExecuteNonQuery("DELETE FROM cortes WHERE orden = @p1", ("@p1", numero));
        _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
        _fixture.ExecuteNonQuery("DELETE FROM orden_corte WHERE numero = @p1", ("@p1", numero));
        _fixture.Service.UpdateConsecOC(numero.ToString());
    }

    [Fact]
    public void Update_Items_Orden_Corte_ActualizaLosRenglones()
    {
        // P1: la actualizacion de items debe reflejarse en la BD y ser atomica (una transaccion).
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        string uc = "UCT" + numero;
        try
        {
            _fixture.ExecuteNonQuery(
                "INSERT INTO rolls_details (product_id,product_name,roll_number,unique_code,splice,width,large,msi,roll_id,code_person,status,ubic,numero,vuelta) " +
                "VALUES('X','X',1,@uc,0,1,1,1,'X','X','X','X',@num,1)",
                ("@uc", uc), ("@num", numero));

            _fixture.Service.Update_Items_Orden_Corte(new List<RolloCortado>
            {
                new RolloCortado { Numero = numero.ToString(), UniqueCode = uc, Splice = 7, Status = "Y", Code_Person = "Z" }
            });

            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT splice FROM rolls_details WHERE numero = @p1 AND unique_code = @p2",
                ("@p1", numero), ("@p2", uc))!).Should().Be(7);
            _fixture.ExecuteScalar(
                "SELECT status FROM rolls_details WHERE numero = @p1 AND unique_code = @p2",
                ("@p1", numero), ("@p2", uc))!.ToString().Should().Be("Y");
            _fixture.ExecuteScalar(
                "SELECT code_person FROM rolls_details WHERE numero = @p1 AND unique_code = @p2",
                ("@p1", numero), ("@p2", uc))!.ToString().Should().Be("Z");

            // lista vacia: no debe lanzar ni modificar nada.
            _fixture.Service.Update_Items_Orden_Corte(new List<RolloCortado>());
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
            _fixture.Service.UpdateConsecOC(numero.ToString());
        }
    }

    [Fact]
    public void UpdateUniqueCodeRollosCortados_ActualizaUniqueCode()
    {
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        try
        {
            _fixture.ExecuteNonQuery(
                "INSERT INTO rolls_details (product_id,product_name,roll_number,unique_code,splice,width,large,msi,roll_id,code_person,status,ubic,numero,vuelta) " +
                "VALUES('X','X',5,'UC_VIEJO',0,1,1,1,'X','X','X','X',@num,1)",
                ("@num", numero));

            _fixture.Service.UpdateUniqueCodeRollosCortados(new List<RolloCortado>
            {
                new RolloCortado { RollNumber = 5, Numero = numero.ToString(), UniqueCode = "UC_NUEVO" }
            });

            _fixture.ExecuteScalar(
                "SELECT unique_code FROM rolls_details WHERE roll_number = 5 AND numero = @p1",
                ("@p1", numero))!.ToString().Should().Be("UC_NUEVO");
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM rolls_details WHERE numero = @p1", ("@p1", numero));
            _fixture.Service.UpdateConsecOC(numero.ToString());
        }
    }

    [Fact]
    public void AnularOrdenCorte_MarcaComoAnulada()
    {
        int numero = CrearOrdenTemporal(out _);
        try
        {
            bool ok = _fixture.Service.AnularOrdenCorte(numero.ToString());
            ok.Should().BeTrue();
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT anulada FROM orden_corte WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void AnularOrdenCorte_OrdenCerrada_NoAnula()
    {
        int numero = CrearOrdenTemporal(out _);
        try
        {
            _fixture.ExecuteNonQuery("UPDATE orden_corte SET CloseDocument = 1 WHERE numero = @p1", ("@p1", numero));

            bool ok = _fixture.Service.AnularOrdenCorte(numero.ToString());
            ok.Should().BeFalse("una orden cerrada no debe poder anularse");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT anulada FROM orden_corte WHERE numero = @p1", ("@p1", numero))!).Should().Be(0,
                "el estatus de anulado no debe cambiar");
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void AnularOrdenCorte_YaAnulada_NoAnula()
    {
        int numero = CrearOrdenTemporal(out _);
        try
        {
            _fixture.Service.AnularOrdenCorte(numero.ToString()).Should().BeTrue();

            bool segundo = _fixture.Service.AnularOrdenCorte(numero.ToString());
            segundo.Should().BeFalse("anular dos veces no debe reportar exito");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT anulada FROM orden_corte WHERE numero = @p1", ("@p1", numero))!).Should().Be(1);
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void AnularOrdenCorte_Inexistente_RetornaFalse()
    {
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        try
        {
            // numero recien obtenido aun no existe como orden.
            bool ok = _fixture.Service.AnularOrdenCorte(numero.ToString());
            ok.Should().BeFalse();
        }
        finally
        {
            _fixture.Service.UpdateConsecOC(numero.ToString());
        }
    }

    [Fact]
    public void UpdateStatusDocumentOC_ActualizaStep()
    {
        int numero = CrearOrdenTemporal(out _);
        try
        {
            bool ok = _fixture.Service.UpdateStatusDocumentOC(7, numero.ToString());
            ok.Should().BeTrue();
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT step FROM orden_corte WHERE numero = @p1", ("@p1", numero))!).Should().Be(7);
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void UpdateStatusDocumentOC_Anulada_NoCambiaStep()
    {
        int numero = CrearOrdenTemporal(out _);
        try
        {
            _fixture.ExecuteNonQuery("UPDATE orden_corte SET anulada = 1 WHERE numero = @p1", ("@p1", numero));
            int stepInicial = Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT step FROM orden_corte WHERE numero = @p1", ("@p1", numero))!);

            bool ok = _fixture.Service.UpdateStatusDocumentOC(3, numero.ToString());
            ok.Should().BeFalse("una OC anulada no debe admitir cambios de estado");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT step FROM orden_corte WHERE numero = @p1", ("@p1", numero))!)
                .Should().Be(stepInicial, "el step no debe cambiar cuando la OC esta anulada");
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void UpdateStatusDocumentOC_Cerrada_NoCambiaStep()
    {
        int numero = CrearOrdenTemporal(out _);
        try
        {
            _fixture.ExecuteNonQuery("UPDATE orden_corte SET CloseDocument = 1 WHERE numero = @p1", ("@p1", numero));
            int stepInicial = Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT step FROM orden_corte WHERE numero = @p1", ("@p1", numero))!);

            bool ok = _fixture.Service.UpdateStatusDocumentOC(7, numero.ToString());
            ok.Should().BeFalse("una OC cerrada no debe admitir cambios de estado");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT step FROM orden_corte WHERE numero = @p1", ("@p1", numero))!)
                .Should().Be(stepInicial, "el step no debe cambiar cuando la OC esta cerrada");
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void AnularOrdenCorte_AnulaRollosHijos()
    {
        int numero = CrearOrdenTemporal(out _);
        try
        {
            int dispAntes = Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1 AND disponible = 1", ("@p1", numero))!);
            dispAntes.Should().Be(1, "la OC temporal debe tener 1 rollo disponible");

            bool ok = _fixture.Service.AnularOrdenCorte(numero.ToString());
            ok.Should().BeTrue();
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1 AND disponible = 1", ("@p1", numero))!)
                .Should().Be(0, "la anulacion debe marcar los rollos hijos como no disponibles");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT anulada FROM orden_corte WHERE numero = @p1", ("@p1", numero))!)
                .Should().Be(1);
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void AnularOrdenCorte_ReverteConsumoFisicoDelMaster()
    {
        int numero = CrearOrdenTemporal(out _);
        string rollid = "ZT" + numero;
        decimal largoOriginal = 200;
        decimal consumoRegistrado = 50;
        try
        {
            _fixture.ExecuteNonQuery("UPDATE orden_corte SET rollid_1 = @p1 WHERE numero = @p2",
                ("@p1", rollid), ("@p2", numero));

            _fixture.ExecuteNonQuery(
                "INSERT INTO MasterInic (part_number,disponible,OrderPurchase,width,lenght,roll_id,splice,ubicacion,core,anulado,master,resma,graphics,embarque,fecha_pro,fecha_reg,width_c,lenght_c,palet_num) " +
                "VALUES('X',1,1,@p2,@p3,@p1,0,'X',0,0,1,0,0,'',GETDATE(),GETDATE(),0,0,'')",
                ("@p1", rollid), ("@p2", largoOriginal), ("@p3", consumoRegistrado));
            _fixture.ExecuteNonQuery(
                "UPDATE MasterInic SET largo_consumido = @p3 WHERE Roll_Id = @p1",
                ("@p1", rollid), ("@p3", consumoRegistrado));

            _fixture.ExecuteNonQuery(
                "INSERT INTO MasterDetailsInic (rollid, orden, consumo, fecha_reg, desperdicio) " +
                "VALUES (@p1, @p2, @p3, GETDATE(), 0)",
                ("@p1", rollid), ("@p2", numero), ("@p3", consumoRegistrado));

            _fixture.Service.AnularOrdenCorte(numero.ToString()).Should().BeTrue();

            Convert.ToDecimal(_fixture.ExecuteScalar(
                "SELECT largo_consumido FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!)
                .Should().Be(0m, "el consumo fisico del master debe revertirse completamente (50 - 50 = 0)");
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM MasterDetailsInic WHERE rollid = @p1 AND orden = @p2",
                ("@p1", rollid), ("@p2", numero))!)
                .Should().Be(0, "el detalle de consumo debe eliminarse al anular la OC");
        }
        finally
        {
            _fixture.ExecuteNonQuery("DELETE FROM MasterDetailsInic WHERE rollid = @p1", ("@p1", rollid));
            _fixture.ExecuteNonQuery("DELETE FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid));
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void UpdateOrdenCorte_ActualizaCampos()
    {
        int numero = CrearOrdenTemporal(out Orden? orden);
        try
        {
            orden.SellOrder = "SO_EDITADA";
            orden.Fecha = new DateTime(2020, 1, 1);
            bool ok = _fixture.Service.UpdateOrdenCorte(orden);
            ok.Should().BeTrue();
            _fixture.ExecuteScalar(
                "SELECT sellOrder FROM orden_corte WHERE numero = @p1", ("@p1", numero))!.ToString().Should().Be("SO_EDITADA");
        }
        finally
        {
            EliminarOrdenTemporal(numero);
        }
    }

    [Fact]
    public void ServiceLogger_RegistraMensajeEnSink()
    {
        // logging centralizado: el mensaje debe enrutarse por el Sink configurado.
        string? capturado = null;
        Action<string>? original = ServiceLogger.Sink;
        ServiceLogger.Sink = m => capturado = m;
        try
        {
            ServiceLogger.Log("mensaje de prueba");
            capturado.Should().Be("mensaje de prueba");
        }
        finally
        {
            ServiceLogger.Sink = original;
        }
    }

    [Fact]
    public void ServiceErrors_Report_Defecto_RegistraYNotifica()
    {
        // El Report por defecto debe registrar en ServiceLogger ademas de notificar al usuario.
        // En el test redirigimos el Sink y desactivamos el MessageBox para no bloquear el runner.
        string? registrado = null;
        Action<string>? sinkOriginal = ServiceLogger.Sink;
        Action<string> notifyOriginal = ServiceErrors.Notify;
        Action<string> reportOriginal = ServiceErrors.Report;
        ServiceLogger.Sink = m => registrado = m;
        ServiceErrors.Notify = _ => { };
        try
        {
            ServiceErrors.Report = ServiceErrors.DefaultReport;
            ServiceErrors.Report("error centralizado");
            registrado.Should().Be("error centralizado");
        }
        finally
        {
            ServiceLogger.Sink = sinkOriginal;
            ServiceErrors.Notify = notifyOriginal;
            ServiceErrors.Report = reportOriginal;
        }
    }
}
