
namespace Ritrama2025.Models
{
    public class DetailOrdenRecepcion
    {
        public string Product_id { get; set; } = null!;
        public string Product_Name { get; set; } = null!;
        public string Product_Type { get; set; } = null!;
        public double Cantidad { get; set; }
        public decimal Width { get; set; }
        public decimal Length { get; set; }
        public decimal Msi { get; set; }
        public int Splice { get; set; }
        public string ubicacion { get; set; } = null!;
    }
}
