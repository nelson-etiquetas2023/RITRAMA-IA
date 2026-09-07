
namespace Ritrama2025.Models
{
    public class OrdenDetailsMP
    {
        public string Numero { get; set; } = null!;
        public string Product_Id { get; set; } = string.Empty;
        public string Product_Name { get; set; } = string.Empty;
        public string Product_Type { get; set; } = string.Empty;
        public double Width { get; set; }
        public double Length { get; set; }
        public double Msi { get; set; }
        public string RollId { get; set; } = string.Empty;
        public int Splice { get; set; }
        public double Core { get; set; }
        public string Ubicacion { get; set; } = string.Empty;
        public double Cantidad_Pedido { get; set; }
        public double Cantidad_Real { get; set; }
        public string Estado { get; set; } = "";
        public int Num_empalme { get; set; }
        public string Num_Paleta { get; set; } = "";
        public DateTime Fecha_produccion { get; set; }
        public DateTime Fecha_Ingreso { get; set; }
        public string Factura { get; set; } = string.Empty;
    }
}
