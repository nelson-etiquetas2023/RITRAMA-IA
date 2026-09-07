namespace Ritrama2025.Models
{
    public class Despacho
    {
        public string Numero { get; set; } = null!;
        public DateTime Fecha_despacho { get; set; }
        public string Customer_Id { get; set; } = null!;
        public string Customer_Name { get; set; } = null!;

        public string Persona_Contact { get; set; } = null!;
        public string Vendor_Id { get; set; } = null!;
        public string Vendor_Name { get; set; } = null!;
        public string Transport_Id { get; set; } = null!;
        public string Transport_Name { get; set; } = null!;
        public string Chofer_Id { get; set; } = null!;
        public string Chofer_Name { get; set; } = null!;
        public string Camion_Id { get; set; } = null!;
        public string Camion_Name { get; set; } = null!;
        public string Tipo_Embalaje { get; set; } = null!;
        public string Orden_Trabajo { get; set; } = null!;
        public string Orden_Compra { get; set; } = null!;
        public decimal SubTotal { get; set; }
        public decimal Porc_Itbis { get; set; }
        public decimal Monto_Itbis { get; set; }
        public decimal Total_Despacho { get; set; }
        public string Tipo_venta { get; set; } = null!;
        public List<ItemsDespacho> Items_Despacho { get; set; } = null!;
        public List<RolloCortado> Detalle_RC { get; set; } = null!;
        public List<Paleta> Detalle_Paleta { get; set; } = null!;
        public decimal Total_Cantidad { get; set; }
        public decimal Total_Msi { get; set; }
        public decimal Total_Pie { get; set; }
        public decimal Total_Kilos { get; set; }
        public decimal Total_kilos_netos_palet { get; set; }
        public decimal Total_kilos_brutos_palet { get; set; }

    }
}
