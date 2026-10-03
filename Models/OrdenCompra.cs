namespace Ritrama2025.Models
{
    public class OrdenCompra
    {
        public string Numero { get; set; } = string.Empty;
        public DateTime Fecha { get; set; }
        public string? Proveedor_Id { get; set; }
        public string? Proveedor_Name { get; set; }
        public string? Persona_Contacto { get; set; }
        public DateTime? Fecha_entrega { get; set; }
        public string? Direccion_entrega { get; set; }
        public string? Direccion_facturacion { get; set; }
        public string? Condiciones_pago { get; set; }
        public string? Prioridad { get; set; }
        public string? Estado { get; set; }
        public string? Notas { get; set; }
        public bool Anulado { get; set; }
        public decimal SubTotal { get; set; }
        public decimal Porc_Itbis { get; set; }
        public decimal Monto_Itbis { get; set; }
        public decimal Total { get; set; }
        public List<OrdenCompraDetalle> Detalle { get; set; } = new();
    }
}
