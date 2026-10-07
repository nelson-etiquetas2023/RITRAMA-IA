
namespace Ritrama2025.Models
{
    /// <summary>
    /// Fila de la hoja de Excel de importación de productos, ya validada.
    /// La plantilla descargable tiene tres columnas: product_id (el código Ritrama único
    /// que teclea el usuario, que ya es el Product_ID del producto; el consecutivo del
    /// sistema lo guarda el importador en IdConsec),
    /// product_name y categoria (Master / Rollo Cortado / Resma / Graphics). El importador
    /// acepta también las cabeceras CodigoRitrama, Nombre y Tipo. El resto de datos nacen
    /// con valores por defecto.
    /// </summary>
    public class ProductoImportFila
    {
        /// <summary>Número de fila en la hoja Excel (para mensajes de error).</summary>
        public int FilaExcel { get; set; }

        public string CodigoRitrama { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        /// <summary>Tipo tecleado en la hoja (texto libre, se normaliza al importar).</summary>
        public string Tipo { get; set; } = string.Empty;

        /// <summary>Motivo del rechazo. Vacío = fila válida.</summary>
        public string Error { get; set; } = string.Empty;

        /// <summary>True si la fila pasó todas las validaciones.</summary>
        public bool EsValida => string.IsNullOrEmpty(Error);
    }
}
