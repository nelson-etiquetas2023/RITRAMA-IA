namespace Ritrama2025.Models
{
    public class OrdenCompraDetalle
    {
        public string? Product_id { get; set; }
        public string? Product_name { get; set; }
        public decimal Cant { get; set; }
        public string? Unidad { get; set; }
        public decimal Width { get; set; }
        public decimal Lenght { get; set; }
        public decimal Msi { get; set; }
        public decimal? Precio { get; set; }
        public decimal? Total_Renglon { get; set; }
        public string? Notas { get; set; }
    }
}
