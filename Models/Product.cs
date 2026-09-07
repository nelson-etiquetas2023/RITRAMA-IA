
namespace Ritrama2025.Models
{
    public class Product
    {
        public string Product_id { get; set; } = null!;
        public string Product_Name { get; set; } = null!;
        public string Product_Description { get; set; } = null!;
        public string Referencia { get; set; } = null!;
        public string Codigo_Barra { get; set; } = null!;
        public decimal Precio { get; set; }
        public decimal Ratio { get; set; }
        public bool Anulado { get; set; }
        public bool Master { get; set; }
        public bool Hoja { get; set; }
        public bool Graphics { get; set; }
        public bool RolloCortado { get; set; }
    }

}


