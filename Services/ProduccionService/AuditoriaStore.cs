using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService
{
    /// <summary>
    /// Store en memoria (singleton) con los hallazgos pendientes de auditoria.
    /// Centraliza lo que antes eran MessageBox modales: reconciliacion de BD y
    /// validaciones del documento. La pantalla FrmAuditoriaInconsistencias lo
    /// muestra bajo demanda; el badge de FrmOrdenCorte solo cuenta pendientes.
    /// </summary>
    public sealed class AuditoriaStore : IAuditoriaStore
    {
        private readonly Lock _lock = new();
        private readonly List<HallazgoAuditoria> _hallazgos = [];
        private readonly List<InconsistenciaOC> _reconciliacion = [];

        public event EventHandler? Changed;

        public IReadOnlyList<HallazgoAuditoria> Hallazgos
        {
            get { lock (_lock) { return _hallazgos.ToList(); } }
        }

        public IReadOnlyList<InconsistenciaOC> ReconciliacionOriginal
        {
            get { lock (_lock) { return _reconciliacion.ToList(); } }
        }

        public int PendientesCount
        {
            get
            {
                lock (_lock)
                {
                    return _hallazgos.Count(h =>
                        h.Estado == EstadoHallazgo.Pendiente &&
                        h.Severidad != SeveridadHallazgo.Info);
                }
            }
        }

        public void Agregar(HallazgoAuditoria hallazgo)
        {
            if (hallazgo == null) return;
            lock (_lock)
            {
                // Evita duplicados pendientes del mismo origen/codigo/OC/descripcion
                // (p.ej. pulsar Guardar varias veces seguidas).
                bool duplicado = _hallazgos.Any(h =>
                    h.Estado == EstadoHallazgo.Pendiente &&
                    h.Origen == hallazgo.Origen &&
                    h.Codigo == hallazgo.Codigo &&
                    h.OC == hallazgo.OC &&
                    h.Descripcion == hallazgo.Descripcion);
                if (duplicado) return;
                _hallazgos.Add(hallazgo);
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void SincronizarReconciliacion(List<InconsistenciaOC> inconsistencias)
        {
            lock (_lock)
            {
                _hallazgos.RemoveAll(h => h.Origen == OrigenHallazgo.Reconciliacion);
                _reconciliacion.Clear();
                foreach (var inc in inconsistencias ?? [])
                {
                    _reconciliacion.Add(inc);
                    _hallazgos.Add(new HallazgoAuditoria
                    {
                        Fecha = inc.FechaDeteccion,
                        Origen = OrigenHallazgo.Reconciliacion,
                        Codigo = inc.TipoInconsistencia,
                        OC = inc.NumeroOC.ToString(),
                        NumeroOC = inc.NumeroOC,
                        Descripcion = inc.Descripcion,
                        AccionSugerida = inc.RequiereAccionManual
                            ? "Requiere revision manual del inventario de masters."
                            : "Se puede corregir desde esta pantalla (Corregir automaticas).",
                        Severidad = inc.RequiereAccionManual
                            ? SeveridadHallazgo.Bloqueante
                            : SeveridadHallazgo.Advertencia,
                        RequiereAccionManual = inc.RequiereAccionManual
                    });
                }
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void LimpiarOrigen(OrigenHallazgo origen)
        {
            bool cambio;
            lock (_lock)
            {
                cambio = _hallazgos.RemoveAll(h => h.Origen == origen) > 0;
            }
            if (cambio) Changed?.Invoke(this, EventArgs.Empty);
        }

        public void MarcarRevisado(Guid id, string usuario)
        {
            lock (_lock)
            {
                var h = _hallazgos.FirstOrDefault(x => x.Id == id);
                if (h == null || h.Estado != EstadoHallazgo.Pendiente) return;
                h.Estado = EstadoHallazgo.Revisado;
                h.RevisadoPor = usuario;
                h.FechaRevision = DateTime.Now;
            }
            Changed?.Invoke(this, EventArgs.Empty);
        }

        public void MarcarRevisados(IEnumerable<Guid> ids, string usuario)
        {
            bool cambio = false;
            lock (_lock)
            {
                foreach (var id in ids ?? [])
                {
                    var h = _hallazgos.FirstOrDefault(x => x.Id == id);
                    if (h == null || h.Estado != EstadoHallazgo.Pendiente) continue;
                    h.Estado = EstadoHallazgo.Revisado;
                    h.RevisadoPor = usuario;
                    h.FechaRevision = DateTime.Now;
                    cambio = true;
                }
            }
            if (cambio) Changed?.Invoke(this, EventArgs.Empty);
        }

        /// <summary>
        /// Devuelve las inconsistencias automaticas que siguen pendientes (para
        /// corregirlas desde la pantalla) y las marca como corregidas en el store.
        /// </summary>
        public List<InconsistenciaOC> TomarAutomaticasPendientes()
        {
            List<InconsistenciaOC> pendientes;
            lock (_lock)
            {
                var clavesPendientes = _hallazgos
                    .Where(h => h.Origen == OrigenHallazgo.Reconciliacion &&
                                h.Estado == EstadoHallazgo.Pendiente &&
                                !h.RequiereAccionManual)
                    .Select(h => (h.NumeroOC, h.Codigo))
                    .ToHashSet();
                pendientes = _reconciliacion
                    .Where(r => !r.RequiereAccionManual &&
                                clavesPendientes.Contains((r.NumeroOC, r.TipoInconsistencia)))
                    .ToList();
                foreach (var h in _hallazgos.Where(h =>
                    h.Origen == OrigenHallazgo.Reconciliacion &&
                    h.Estado == EstadoHallazgo.Pendiente &&
                    !h.RequiereAccionManual))
                {
                    h.Estado = EstadoHallazgo.Corregido;
                    h.FechaRevision = DateTime.Now;
                }
            }
            Changed?.Invoke(this, EventArgs.Empty);
            return pendientes;
        }
    }
}
