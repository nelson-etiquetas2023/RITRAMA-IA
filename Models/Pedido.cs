namespace Ritrama2025.Models
{
    public class Pedido
    {
        public int Numero { get; set; }
        public DateTime Fecha { get; set; }
        public Guid Customer_Id { get; set; }
        public string? Customer_Name { get; set; }
        public Guid? Vendor_Id { get; set; }
        public string? Persona_Contacto { get; set; }
        public string? Tipo_venta { get; set; }
        public DateTime? Fecha_entrega { get; set; }
        public string? Condiciones_pago { get; set; }
        public string? Direccion_entrega { get; set; }
        public string? Estado { get; set; }
        public string? Notas { get; set; }
        public bool Anulado { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Porc_Itbis { get; set; }
        public decimal Monto_Itbis { get; set; }
        public decimal Total { get; set; }
        public List<PedidoDetalle> Detalle { get; set; } = new();
    }
}
