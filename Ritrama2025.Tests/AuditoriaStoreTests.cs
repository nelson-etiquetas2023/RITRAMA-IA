using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using Xunit;

namespace Ritrama2025.Tests;

public class AuditoriaStoreTests
{
    private static HallazgoAuditoria Hallazgo(OrigenHallazgo origen, string codigo, string oc,
        SeveridadHallazgo severidad = SeveridadHallazgo.Bloqueante) => new()
        {
            Origen = origen,
            Codigo = codigo,
            OC = oc,
            Descripcion = codigo + " en " + oc,
            Severidad = severidad
        };

    private static InconsistenciaOC Inconsistencia(int oc, string tipo, bool manual) => new()
    {
        NumeroOC = oc,
        StepActual = 3,
        TipoInconsistencia = tipo,
        Descripcion = tipo + " OC " + oc,
        RequiereAccionManual = manual
    };

    [Fact]
    public void Agregar_CuentaSoloPendientesNoInfo()
    {
        var store = new AuditoriaStore();
        store.Agregar(Hallazgo(OrigenHallazgo.ValidacionDocumento, "A", "100"));
        store.Agregar(Hallazgo(OrigenHallazgo.Guardado, "B", "100", SeveridadHallazgo.Info));

        store.PendientesCount.Should().Be(1);
        store.Hallazgos.Should().HaveCount(2);
    }

    [Fact]
    public void Agregar_EvitaDuplicadosPendientes()
    {
        var store = new AuditoriaStore();
        store.Agregar(Hallazgo(OrigenHallazgo.Cierre, "X", "200"));
        store.Agregar(Hallazgo(OrigenHallazgo.Cierre, "X", "200"));

        store.Hallazgos.Should().HaveCount(1);
    }

    [Fact]
    public void LimpiarOrigen_RemueveSoloEseOrigen()
    {
        var store = new AuditoriaStore();
        store.Agregar(Hallazgo(OrigenHallazgo.ValidacionDocumento, "A", "1"));
        store.Agregar(Hallazgo(OrigenHallazgo.Etiquetado, "B", "1"));

        store.LimpiarOrigen(OrigenHallazgo.ValidacionDocumento);

        store.Hallazgos.Should().ContainSingle(h => h.Origen == OrigenHallazgo.Etiquetado);
    }

    [Fact]
    public void SincronizarReconciliacion_ReemplazaAnteriores()
    {
        var store = new AuditoriaStore();
        store.SincronizarReconciliacion([Inconsistencia(10, "SIN_CONSUMO_MASTER1", false)]);
        store.SincronizarReconciliacion([Inconsistencia(11, "CONSUMO_SIN_ETIQUETAR", true)]);

        var hallazgos = store.Hallazgos.Where(h => h.Origen == OrigenHallazgo.Reconciliacion).ToList();
        hallazgos.Should().ContainSingle();
        hallazgos[0].OC.Should().Be("11");
        hallazgos[0].RequiereAccionManual.Should().BeTrue();
        hallazgos[0].Severidad.Should().Be(SeveridadHallazgo.Bloqueante);
    }

    [Fact]
    public void MarcarRevisados_BajaElContador()
    {
        var store = new AuditoriaStore();
        var h = Hallazgo(OrigenHallazgo.Cierre, "A", "300");
        store.Agregar(h);

        store.MarcarRevisados([h.Id], "tester");

        store.PendientesCount.Should().Be(0);
        store.Hallazgos[0].Estado.Should().Be(EstadoHallazgo.Revisado);
        store.Hallazgos[0].RevisadoPor.Should().Be("tester");
    }

    [Fact]
    public void TomarAutomaticasPendientes_DevuelveSoloNoManuales()
    {
        var store = new AuditoriaStore();
        store.SincronizarReconciliacion([
            Inconsistencia(20, "SIN_CONSUMO_MASTER1", false),
            Inconsistencia(21, "CONSUMO_SIN_ETIQUETAR", true)
        ]);

        var auto = store.TomarAutomaticasPendientes();

        auto.Should().ContainSingle(a => a.NumeroOC == 20);
        store.PendientesCount.Should().Be(1);
    }
}
