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
        /// <summary>
        /// Pago inmediato. Es el unico valor compartido por los dos vocabularios: aparece como
        /// tipo de venta y como condicion de pago, porque al contado el plazo es el dia cero.
        /// Vive aqui y no como literal en la pantalla para que el dato y la regla que lo usa no
        /// puedan separarse.
        /// </summary>
        public const string Contado = "contado";

        /// <summary>Tipo de venta del pedido. Medido sobre los pedidos existentes: solo aparecen estos dos.</summary>
        public static readonly IReadOnlyList<string> TipoVenta = new[] { Contado, "credito" };

        /// <summary>
        /// Condiciones de pago del pedido, ordenadas por plazo: "contado" es el dia cero, los
        /// plazos van ascendentes y "credito" queda al final por ser el caso abierto, sin fecha
        /// fija. Sin tilde a proposito: en pantalla y en la base se escribe "dias", igual que
        /// los valores historicos.
        ///
        /// Reemplaza el vocabulario anterior (neto 30, neto 45, neto 60, anticipo), que se
        /// remapeo con Scripts/Migrar_Pedido_CondicionesPago.sql.
        /// </summary>
        public static readonly IReadOnlyList<string> CondicionesPago =
            new[] { Contado, "7 dias", "15 dias", "30 dias", "45 dias", "60 dias", "credito" };

        /// <summary>
        /// Prioridad del pedido. "normal" es el valor de facto: los pedidos guardados antes de
        /// que existiera la columna tienen NULL, y la aplicacion los trata como normales.
        /// </summary>
        public static readonly IReadOnlyList<string> Prioridad = new[] { "normal", "urgente" };

        /// <summary>
        /// Indica si el tipo de venta es al contado. La comparacion no distingue mayusculas
        /// porque el dato viene de una columna varchar y no todas las filas se escribieron con la
        /// misma capitalizacion.
        /// </summary>
        public static bool EsContado(string? tipoVenta)
        {
            return string.Equals(tipoVenta, Contado, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Un pedido al contado no tiene plazo de credito, asi que la condicion de pago no se
        /// elige: queda fija. El modo tambien manda, porque al consultar un pedido guardado
        /// ninguno de los dos combos se edita, este o no el tipo de venta.
        ///
        /// Vive acá y no en el formulario para que se pueda probar sin levantar la pantalla, que
        /// es lo que impidió detectar la regla cuando se escribio directamente sobre el combo.
        /// </summary>
        public static bool CondicionesPagoEditable(string? tipoVenta, bool esNuevo)
        {
            return esNuevo && !EsContado(tipoVenta);
        }
    }
}
