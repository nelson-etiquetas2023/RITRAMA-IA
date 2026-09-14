using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService
{
    public interface IAuditoriaStore
    {
        event EventHandler? Changed;

        IReadOnlyList<HallazgoAuditoria> Hallazgos { get; }

        IReadOnlyList<InconsistenciaOC> ReconciliacionOriginal { get; }

        int PendientesCount { get; }

        void Agregar(HallazgoAuditoria hallazgo);

        void SincronizarReconciliacion(List<InconsistenciaOC> inconsistencias);

        void LimpiarOrigen(OrigenHallazgo origen);

        void MarcarRevisado(Guid id, string usuario);

        void MarcarRevisados(IEnumerable<Guid> ids, string usuario);

        List<InconsistenciaOC> TomarAutomaticasPendientes();
    }
}
