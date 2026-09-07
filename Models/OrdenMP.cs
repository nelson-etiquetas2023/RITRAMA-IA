
namespace Ritrama2025.Models
{
    public class OrdenMP
    {
        public string Numero { get; set; } = null!;
        public string Orden_Compra { get; set; } = null!;
        public Guid Proveedor_id { get; set; }
        public string Proveedor_name { get; set; } = null!;
        public DateTime Fecha_Recepcion { get; set; }
        public DateTime Fecha_Produccion { get; set; }
        public Guid Transport_id { get; set; }
        public string Transport_name { get; set; } = null!;
        public string Guia { get; set; } = null!;
        public string Lote { get; set; } = null!;
        public string Numero_Embarque { get; set; } = null!;
        public Guid Person_Id { get; set; }
        public string Person_Name { get; set; } = null!;
        public string Notas { get; set; } = null!;
        public int Renglones { get; set; }
        public bool CloseDocument { get; set; }
        public List<OrdenDetailsMP> Items { get; set; } = [];
    }
}
