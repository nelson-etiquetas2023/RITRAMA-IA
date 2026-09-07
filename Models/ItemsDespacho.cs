
namespace Ritrama2025.Models
{
    public class ItemsDespacho
    {
        public string Numero { get; set; } = null!;
        public string Product_id { get; set; } = null!;
        public string Product_name { get; set; } = null!;
        public decimal Cantidad { get; set; }
        public string Unid_id { get; set; } = null!;
        public string Unidad { get; set; } = null!;
        public decimal Width { get; set; }
        public decimal Lenght { get; set; }
        public decimal Msi { get; set; }
        public decimal Total_PieLineal { get; set; }
        public decimal Ratio { get; set; }
        public decimal Kilo_Rollo { get; set; }
        public decimal Kilo_Total { get; set; }
        public decimal Precio { get; set; }
        public decimal M2 { get; set; }
        public decimal Total_Renglon { get; set; }
        public string Code_Person { get; set; } = null!;
    }
}
