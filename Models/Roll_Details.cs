
namespace Ritrama2025.Models
{
    public class Roll_Details
    {
        public int ItemNo { get; set; }
        public DateTime Fecha { get; set; }

        public string Numero_Orden { get; set; } = null!;

        public string Roll_number { get; set; } = null!;

        public string Product_id { get; set; } = null!;

        public string Product_name { get; set; } = null!;

        public string Roll_id { get; set; } = null!;

        public decimal Width { get; set; }

        public decimal Large { get; set; }

        public decimal Msi { get; set; }

        public int Splice { get; set; }

        public string Code_Person { get; set; } = null!;

        public string Unique_code { get; set; } = null!;

        public string Status { get; set; } = null!;

        public bool Disponible { get; set; }

        public string Tipo_mov { get; set; } = null!;

        public int Tipo_pro { get; set; }

        public int Cantidad { get; set; }

        public int Cant_despacho { get; set; }

        public string Ubic { get; set; } = null!;

        public string? Rollid_oculto { get; set; } = null!;

        public string? No_paleta { get; set; } = null!;
        public string? despacho { get; set; } = null!;
        public DateTime? fecha_despacho { get; set; } = DateTime.MinValue;


    }
}
