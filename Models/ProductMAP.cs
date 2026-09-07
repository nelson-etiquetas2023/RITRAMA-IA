
namespace Ritrama2025.Models
{
    public class ProductMAP
    {
        public int ItemNo { get; set; } = 0;
        public string Product_Id { get; set; } = null!;
        public string Product_Name { get; set; } = null!;
        public string Product_Type { get; set; } = null!;
        public double Width { get; set; }
        public double Length { get; set; }
        public double Length_Consumido { get; set; }
        public double Length_Restante { get; set; }
        public string Estado { get; set; } = null!;
        public double Msi { get; set; }
        public int Core { get; set; }
        public int Splice { get; set; }
        public string Rollid { get; set; } = null!;
        public string Ubic { get; set; } = null!;
        public int Cant { get; set; }
        public string Paleta { get; set; } = null!;
        public string Recepcion { get; set; } = null!;
        public DateTime Fecha_Fabricacion { get; set; }
        public DateTime Fecha_Llegada { get; set; }
        public DateTime Fecha_Produccion { get; set; }

        public string Factura { get; set; } = null!;

        public string Fecha_Impresion { get; set; } = null!;


    }
}
