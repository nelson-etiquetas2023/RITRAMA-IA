namespace Ritrama2025.Models
{
    public enum SeveridadHallazgo
    {
        Bloqueante = 0,
        Advertencia = 1,
        Info = 2
    }

    public enum OrigenHallazgo
    {
        Reconciliacion = 0,
        ValidacionDocumento = 1,
        GenerarRollos = 2,
        MontarMaster = 3,
        Etiquetado = 4,
        Cierre = 5,
        Aprobacion = 6,
        Guardado = 7
    }

    public enum EstadoHallazgo
    {
        Pendiente = 0,
        Revisado = 1,
        Corregido = 2
    }

    /// <summary>
    /// Hallazgo unificado para la pantalla de auditoria: inconsistencias de BD
    /// (reconciliacion) y validaciones del documento en curso. Los bloqueantes
    /// impidieron guardar/etiquetar/cerrar; se revisan bajo demanda en
    /// FrmAuditoriaInconsistencias en lugar de un MessageBox modal.
    /// </summary>
    public sealed class HallazgoAuditoria
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public DateTime Fecha { get; set; } = DateTime.Now;
        public OrigenHallazgo Origen { get; set; }
        public string Codigo { get; set; } = string.Empty;
        public string OC { get; set; } = string.Empty;
        public int NumeroOC { get; set; }
        public string Rollid { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string AccionSugerida { get; set; } = string.Empty;
        public SeveridadHallazgo Severidad { get; set; } = SeveridadHallazgo.Bloqueante;
        public bool RequiereAccionManual { get; set; }
        public EstadoHallazgo Estado { get; set; } = EstadoHallazgo.Pendiente;
        public string? RevisadoPor { get; set; }
        public DateTime? FechaRevision { get; set; }
    }
}
