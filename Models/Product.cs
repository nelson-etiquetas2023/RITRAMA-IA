
namespace Ritrama2025.Models
{
    /// <summary>
    /// Entidad de producto. Los 4 flags de categoría son mutuamente excluyentes:
    /// solo uno entre Master, RolloCortado, Hoja (Resmas) y Graphics puede estar en true.
    /// Resuelto en base de datos via CASE WHEN en <see cref="R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS"/>.
    /// Un producto anulado no se puede editar (regla de negocio).
    /// </summary>
    public class Product
    {
        /// <summary>Código primario del producto (product_id).</summary>
        public string Product_id { get; set; } = null!;

        /// <summary>Nombre del producto.</summary>
        public string Product_Name { get; set; } = null!;

        /// <summary>Descripción del producto.</summary>
        public string Product_Description { get; set; } = null!;

        /// <summary>Referencia interna.</summary>
        public string Referencia { get; set; } = null!;

        /// <summary>Código de barra.</summary>
        public string Codigo_Barra { get; set; } = null!;

        /// <summary>Precio del producto.</summary>
        public decimal Precio { get; set; }

        /// <summary>Ratio del producto.</summary>
        public decimal Ratio { get; set; }

        /// <summary>Indica si el producto está anulado. Un producto anulado no se puede editar.</summary>
        public bool Anulado { get; set; }

        /// <summary>Categoría Master (MasterRolls = 1). Exclusiva con las otras tres.</summary>
        public bool Master { get; set; }

        /// <summary>Categoría Resma (Resmas = 1). Exclusiva.</summary>
        public bool Hoja { get; set; }

        /// <summary>Categoría Graphics (Graphics = 1). Exclusiva.</summary>
        public bool Graphics { get; set; }

        /// <summary>Categoría Rollo Cortado (rollo_cortado = 1). Exclusiva.</summary>
        public bool RolloCortado { get; set; }
    }

}


