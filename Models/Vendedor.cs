namespace Ritrama2025.Models
{
    /// <summary>
    /// Entidad de vendedor. Coincide con la tabla vendedor de la base de datos.
    /// </summary>
    public class Vendedor
    {
        /// <summary>Código primario del vendedor (vendor_id). Lo genera el sistema (GUID) al dar de alta.</summary>
        public string Vendor_id { get; set; } = null!;

        /// <summary>Código interno consecutivo (1, 2, 3...; se muestra a 5 dígitos). Lo genera el sistema al dar de alta y no se edita.</summary>
        public int CodigoInterno { get; set; }

        /// <summary>Nombre del vendedor.</summary>
        public string Vendor_name { get; set; } = null!;

        /// <summary>Correo del vendedor.</summary>
        public string Correo { get; set; } = null!;

        /// <summary>Teléfono del vendedor.</summary>
        public string Phone { get; set; } = null!;

        /// <summary>Zona del vendedor.</summary>
        public string Zona { get; set; } = null!;

        /// <summary>Indica si el vendedor está anulado/desactivado.</summary>
        public bool Anulado { get; set; }
    }
}