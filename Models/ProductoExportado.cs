
namespace Ritrama2025.Models
{
    /// <summary>
    /// Fila del Excel de productos. No se exporta la entidad <see cref="Product"/> tal cual:
    /// sus cuatro bits de categoria (Master / RolloCortado / Hoja / Graphics) saldrian como
    /// cuatro columnas de True/False y el estado como un booleano crudo, que es una hoja de
    /// calculo y no un informe. Aqui el tipo va ya resuelto en una columna y el estado en texto.
    /// Los nombres de las propiedades SON los titulos de las columnas, porque
    /// ExportDataService.ExportToExcel&lt;T&gt; escribe el nombre de propiedad como encabezado.
    /// </summary>
    public class ProductoExportado
    {
        public string Codigo { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Descripcion { get; set; } = string.Empty;

        public string Referencia { get; set; } = string.Empty;

        public string CodigoBarra { get; set; } = string.Empty;

        /// <summary>Categoria ya resuelta: Master, Rollo Cortado, Resma o Graphics.</summary>
        public string Tipo { get; set; } = string.Empty;

        public decimal Precio { get; set; }

        public decimal Costo { get; set; }

        public decimal Ratio { get; set; }

        /// <summary>Activo o Desactivado, para no tener que traducir un anulado = 1.</summary>
        public string Estado { get; set; } = string.Empty;
    }
}
