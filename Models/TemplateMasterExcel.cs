
namespace Ritrama2025.Models
{
    public class TemplateMasterExcel
    {
        public string product_id { get; set; } = null!;
        public string product_name { get; set; } = null!;
        public string rollid { get; set; } = null!;
        public double width { get; set; }
        public double length { get; set; }
        public string num_empalme { get; set; } = null!;
        public DateTime fecha_produccion { get; set; }
        public string factura { get; set; } = null!;
        public string ubicacion { get; set; } = null!;
        public string palet_num { get; set; } = null!;
        public DateTime fecha_llegada { get; set; }
    }
}
