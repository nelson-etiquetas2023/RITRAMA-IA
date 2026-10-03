namespace Ritrama2025.Models
{
    /// <summary>
    /// Entidad de cliente. Coincide con la tabla customer de la base de datos.
    /// </summary>
    public class Cliente
    {
        /// <summary>Código primario del cliente (customer_id). Lo genera el sistema (GUID) al dar de alta.</summary>
        public string Customer_id { get; set; } = null!;

        /// <summary>Código interno consecutivo (1, 2, 3...; se muestra a 5 dígitos). Lo genera el sistema al dar de alta y no se edita.</summary>
        public int CodigoInterno { get; set; }

        /// <summary>Nombre del cliente.</summary>
        public string Customer_name { get; set; } = null!;

        /// <summary>Identificación fiscal (NIT, DNI, etc.).</summary>
        public string Identificacion { get; set; } = null!;

        /// <summary>Empresa/razón social.</summary>
        public string Empresa { get; set; } = null!;

        /// <summary>Categoría del cliente.</summary>
        public string Customer_category { get; set; } = null!;

        /// <summary>Teléfono de contacto.</summary>
        public string Phone { get; set; } = null!;

        /// <summary>Persona de contacto.</summary>
        public string Contacto { get; set; } = null!;

        /// <summary>Persona de contacto adicional.</summary>
        public string PersonaContacto { get; set; } = null!;

        /// <summary>Email del cliente.</summary>
        public string Customer_email { get; set; } = null!;

        /// <summary>Condición de pago.</summary>
        public string Condicion_pago { get; set; } = null!;

        /// <summary>Impuesto aplicable.</summary>
        public short Impuesto { get; set; }

        /// <summary>Dirección de facturación.</summary>
        public string Direccion_facturacion { get; set; } = null!;

        /// <summary>Dirección de entrega.</summary>
        public string Direccion_entrega { get; set; } = null!;

        /// <summary>Unidad master 1.</summary>
        public bool Unity1 { get; set; }

        /// <summary>Unidad master 2.</summary>
        public bool Unity2 { get; set; }

        /// <summary>Indica si el cliente está anulado/desactivado.</summary>
        public bool Anulado { get; set; }
    }
}