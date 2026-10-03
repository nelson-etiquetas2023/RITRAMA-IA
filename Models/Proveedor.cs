namespace Ritrama2025.Models
{
    /// <summary>
    /// Entidad de proveedor. Coincide con la tabla provider de la base de datos.
    /// </summary>
    public class Proveedor
    {
        /// <summary>Código primario del proveedor (Proveedor_Id). Lo genera el sistema (GUID) al dar de alta.</summary>
        public string Proveedor_Id { get; set; } = null!;

        /// <summary>Código interno consecutivo (1, 2, 3...; se muestra a 5 dígitos). Lo genera el sistema al dar de alta y no se edita.</summary>
        public int CodigoInterno { get; set; }

        /// <summary>Nombre del proveedor.</summary>
        public string Proveedor_Name { get; set; } = null!;

        /// <summary>Teléfono de contacto.</summary>
        public string Phone { get; set; } = null!;

        /// <summary>Persona de contacto.</summary>
        public string PersonaContacto { get; set; } = null!;

        /// <summary>Dirección.</summary>
        public string Direccion { get; set; } = null!;

        /// <summary>Dirección de entrega (puede diferir de la dirección).</summary>
        public string DireccionEntrega { get; set; } = string.Empty;

        /// <summary>Email del proveedor.</summary>
        public string Email { get; set; } = null!;

        /// <summary>Unidad master 1.</summary>
        public bool Unidad_master_1 { get; set; }

        /// <summary>Unidad master 2.</summary>
        public bool Unidad_master_2 { get; set; }

        /// <summary>Categoría del proveedor (Nacional o Internacional). Se elige en combo.</summary>
        public string Categoria { get; set; } = string.Empty;

        /// <summary>Indica si el proveedor está anulado/desactivado.</summary>
        public bool Anulado { get; set; }
    }
}