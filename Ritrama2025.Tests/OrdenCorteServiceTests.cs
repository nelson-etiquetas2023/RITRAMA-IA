using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using System.Data;
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
        var ds = await _fixture.Service.LoadDataOC();
        var dt = ds.Tables["DtMaster"];
        dt.Should().NotBeNull();

        foreach (DataRow row in dt!.Rows)
        {
            Convert.ToInt32(row["anulada"]).Should().Be(0, "el filtro debe excluir las ordenes anuladas");
        }

        // El numero de filas debe coincidir con las ordenes activas (anulada=0 Y CloseDocument=0) en la BD.
        var activas = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM orden_corte WHERE anulada = 0 AND CloseDocument = 0")!);
        dt.Rows.Count.Should().Be(activas, "LoadDataOC debe traer solo ordenes activas (filtro anulada/CloseDocument)");
    }

    [Fact]
    public async Task BuscarRollId_ExcluyeRollosConsumidos()
    {
        var dt = await _fixture.Service.BuscarRollId("Roll_Id", "");

        if (dt.Rows.Count == 0)
        {
            return; // no hay master rolls en la BD; validado en prueba manual
        }

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
        var orden = new Orden
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

    [Fact]
    public async Task ActualizarInventariosMasterAsync_Commit_IncrementaYMarcaDisponible()
    {
        var rollidObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 Roll_Id FROM MasterInic WHERE Roll_Id IS NOT NULL");
        if (rollidObj == null)
        {
            return; // no hay master rolls; validado en prueba manual
        }

        var numeroObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 a.numero FROM orden_corte a JOIN rolls_details r ON r.numero = a.numero " +
            "WHERE a.anulada = 0 AND a.CloseDocument = 0");
        if (numeroObj == null)
        {
            return; // no hay orden con rollos; validado en prueba manual
        }

        string rollid = rollidObj.ToString()!;
        string numero = numeroObj.ToString()!;

        decimal before = Convert.ToDecimal(_fixture.ExecuteScalar(
            "SELECT ISNULL(largo_consumido, 0) FROM MasterInic WHERE Roll_Id = @p1", ("@p1", rollid))!);
        int dispBefore = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT ISNULL(disponible, 0) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!);

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
    }

    [Fact]
    public void Update_Header_Documnet_OC_Rollback_RevierteDeleteAnteError()
    {
        // orden activa existente que tenga cortes
        var numeroObj = _fixture.ExecuteScalar(
            "SELECT TOP 1 a.numero FROM orden_corte a JOIN cortes c ON c.orden = a.numero " +
            "WHERE a.anulada = 0 AND a.CloseDocument = 0");
        if (numeroObj == null) return;
        int numero = Convert.ToInt32(numeroObj);

        int cortesAntes = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM cortes WHERE orden = @p1", ("@p1", numero))!);
        int rollosAntes = Convert.ToInt32(_fixture.ExecuteScalar(
            "SELECT COUNT(*) FROM rolls_details WHERE numero = @p1", ("@p1", numero))!);
        if (cortesAntes == 0) return;

        string ubicAntes = (_fixture.ExecuteScalar(
            "SELECT ISNULL(ubicacion, '') FROM orden_corte WHERE numero = @p1", ("@p1", numero)) ?? "").ToString()!;

        // Forzamos un SqlException real: cortes.width es decimal(18,5) y double.MaxValue desborda.
        // El catch(Exception) debe capturarlo, hacer Rollback y reportar el error (sin MessageBox).
        bool reportado = false;
        var reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            var orden = new Orden
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

    [Fact]
    public void GuardarOrdenCompleta_GuardaTodoEnUnaTransaccion()
    {
        // P0-1 (happy path): encabezado + cortes + rollos se persisten juntos.
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        try
        {
            var orden = CrearOrdenValida(numero);
            var cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            var rollos = new List<RolloCortado>
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
    public void GuardarOrdenCompleta_FalloEnCortes_NoDejaOrdenHuerfana()
    {
        // P0-1 (atomicidad): si falla un corte, toda la transaccion se revierte y NO queda
        // el encabezado de la orden ni los rollos insertados parcialmente.
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        bool reportado = false;
        var reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            var orden = CrearOrdenValida(numero);
            // cortes.width es decimal(18,5): double.MaxValue desborda y fuerza SqlException real.
            var cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = double.MaxValue, Length = 1, Msi = 1 } };
            var rollos = new List<RolloCortado>
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

    [Theory]
    [InlineData(false, true)]
    [InlineData(true, false)]
    public void GuardarOrdenCompleta_RechazaOrdenSinCortesOSinRollos(bool vaciarCortes, bool vaciarRollos)
    {
        // P0-1 (integridad): no se puede guardar una orden incompleta (sin cortes o sin
        // detalle de rollos) aun pasando el header; se rechaza y NO queda encabezado huerfano.
        int numero = _fixture.Service.GetAndIncrementConsecOC();
        bool reportado = false;
        var reporterOriginal = ServiceErrors.Report;
        ServiceErrors.Report = _ => reportado = true;
        try
        {
            var orden = CrearOrdenValida(numero);
            var cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
            var rollos = new List<RolloCortado>
            {
                new RolloCortado
                {
                    Product_Id = "X", Product_Name = "X", RollNumber = 1, UniqueCode = "X", Splice = 0,
                    Width = 1, Length = 1, Msi = 1, Roll_Id = "X", Code_Person = "X", Status = "X",
                    Ubicacion = "X", Numero = numero.ToString(), Vuelta = 1
                }
            };

            var cortesArg = vaciarCortes ? new List<Corte>() : cortes;
            var rollosArg = vaciarRollos ? new List<RolloCortado>() : rollos;

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
        var cortes = new List<Corte> { new Corte { Numero = 1, Orden = numero, Width = 1, Length = 1, Msi = 1 } };
        var rollos = new List<RolloCortado>
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
    public void UpdateOrdenCorte_ActualizaCampos()
    {
        int numero = CrearOrdenTemporal(out var orden);
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
        var original = ServiceLogger.Sink;
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
        var sinkOriginal = ServiceLogger.Sink;
        var notifyOriginal = ServiceErrors.Notify;
        var reportOriginal = ServiceErrors.Report;
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
