namespace Ritrama2025.Services.ProduccionService
{
    public interface IReconciliacionService
    {
        string ErrorMsg { get; set; }
        Task<List<InconsistenciaOC>> DetectarInconsistenciasAsync();
        Task<int> CorregirInconsistenciasAsync(List<InconsistenciaOC> inconsistencias);
    }

    public class InconsistenciaOC
    {
        public int NumeroOC { get; set; }
        public int StepActual { get; set; }
        public string TipoInconsistencia { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public bool RequiereAccionManual { get; set; }
        public DateTime FechaDeteccion { get; set; } = DateTime.Now;
    }
}
