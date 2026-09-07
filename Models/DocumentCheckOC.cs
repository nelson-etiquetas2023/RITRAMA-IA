
namespace Ritrama2025.Models
{
    public class DocumentCheckOC
    {
        public string PersonCheck { get; set; } = null!;
        public string Orden_Servicio { get; set; } = null!;
        public string Orden_Trabajo { get; set; } = null!;
        public string Observaciones { get; set; } = null!;
        public DateTime FechaCheck { get; set; }
        public string OrdenCorte { get; set; } = null!;
    }
}
