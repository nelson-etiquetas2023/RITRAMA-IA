namespace Ritrama2025.Models
{
    /// <summary>
    /// Vocabularios cerrados de tipo de venta y condiciones de pago. Los valores estan en minuscula
    /// porque asi es como ya viven en dbo.pedido: SQL Server distingue "contado" de "Contado" y un
    /// cambio de capitalizacion partiria el dato en dos grupos incomparables al reportear.
    ///
    /// No confundir con customer.condicion_pago, que viene de una importacion heredada con 14
    /// variantes inconsistentes ("Net 30", "NET 45", "Due on receipt", "-"). Ahi el vocabulario
    /// canonico es el de esta clase.
    /// </summary>
    public static class PedidoCatalogos
    {
        /// <summary>Tipo de venta del pedido. Medido sobre los pedidos existentes: solo aparecen estos dos.</summary>
        public static readonly IReadOnlyList<string> TipoVenta = new[] { "contado", "credito" };

        /// <summary>Condiciones de pago del pedido, en el orden en que se ofrecen al usuario.</summary>
        public static readonly IReadOnlyList<string> CondicionesPago =
            new[] { "contado", "credito", "neto 30", "neto 45", "neto 60", "anticipo" };

        /// <summary>
        /// Prioridad del pedido. "normal" es el valor de facto: los pedidos guardados antes de
        /// que existiera la columna tienen NULL, y la aplicacion los trata como normales.
        /// </summary>
        public static readonly IReadOnlyList<string> Prioridad = new[] { "normal", "urgente" };
    }
}
