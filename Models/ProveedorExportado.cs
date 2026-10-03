namespace Ritrama2025.Models
{
    /// <summary>
    /// Fila del Excel de proveedores. No se exporta la entidad <see cref="Proveedor"/> tal cual:
    /// el estado saldría como un booleano crudo, que es una hoja de cálculo y no un informe.
    /// Aquí el estado va ya resuelto en texto. Los nombres de las propiedades SON los títulos
    /// de las columnas, porque ExportDataService.ExportToExcel&lt;T&gt; escribe el nombre
    /// de propiedad como encabezado.
    /// </summary>
    public class ProveedorExportado
    {
        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Telefono { get; set; } = string.Empty;

        public string PersonaContacto { get; set; } = string.Empty;

        public string Direccion { get; set; } = string.Empty;

        public string DireccionEntrega { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Categoria { get; set; } = string.Empty;

        /// <summary>Activo o Desactivado, para no tener que traducir un anulado = 1.</summary>
        public string Estado { get; set; } = string.Empty;
    }
}
