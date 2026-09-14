namespace Ritrama2025.Services.ProduccionService
{
    public interface IOperacionLogService
    {
        Task<long> RegistrarInicioAsync(string tipoOperacion, string descripcion, string usuario = "", string maquina = "", string ipAddress = "");
        Task RegistrarFinAsync(long operacionId, bool exitoso, string resultado = "", string detalleError = "");
        Task RegistrarDetalleAsync(long operacionId, string accion, string entidad, string? valorAnterior = null, string? valorNuevo = null, string notas = "");
        Task<List<OperacionLog>> ConsultarLogsAsync(DateTime fechaInicio, DateTime fechaFin, string? tipoOperacion = null, int maxRegistros = 500);
        Task<List<OperacionLog>> ConsultarLogsPorOCAsync(string numeroOC);
        Task<string> GenerarReporteAsync(long operacionId);
    }

    public class OperacionLog
    {
        public long Id { get; set; }
        public DateTime FechaInicio { get; set; }
        public DateTime? FechaFin { get; set; }
        public string TipoOperacion { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Usuario { get; set; } = string.Empty;
        public string Maquina { get; set; } = string.Empty;
        public string IpAddress { get; set; } = string.Empty;
        public bool Exitoso { get; set; }
        public string Resultado { get; set; } = string.Empty;
        public string DetalleError { get; set; } = string.Empty;
        public List<OperacionLogDetalle> Detalles { get; set; } = [];
    }

    public class OperacionLogDetalle
    {
        public long Id { get; set; }
        public long OperacionId { get; set; }
        public DateTime Fecha { get; set; }
        public string Accion { get; set; } = string.Empty;
        public string Entidad { get; set; } = string.Empty;
        public string? ValorAnterior { get; set; }
        public string? ValorNuevo { get; set; }
        public string Notas { get; set; } = string.Empty;
    }
}
