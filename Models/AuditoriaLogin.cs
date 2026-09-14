namespace Ritrama2025.Models
{
    public class AuditoriaLogin
    {
        public int LogId { get; set; }
        public int? UserId { get; set; }
        public DateTime Fecha { get; set; }
        public bool Exitoso { get; set; }
        public string? IpAddress { get; set; }
    }
}
