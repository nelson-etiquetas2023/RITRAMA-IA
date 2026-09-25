
namespace Ritrama2025.Models
{
    public class OrdenRecepcion
    {
        public string Numero { get; set; } = null!;
        public DateTime Fecha { get; set; }
        public string ProveedorId { get; set; } = null!;
        public string ProveedorName { get; set; } = null!;
        public string OrdenCompra { get; set; } = null!;
        public string Recepcionista { get; set; } = null!;
        public bool Status { get; set; }
        public string Notas { get; set; } = null!;
        public string Transporte { get; set; } = null!;
        public string GuiaImport { get; set; } = null!;
        public string NumLote { get; set; } = null!;
        public List<DetailOrdenRecepcion> Detalles { get; set; } = new();
    }
}
