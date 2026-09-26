namespace Ritrama2025.Models
{
    /// <summary>
    /// Datos del cliente que se muestran al elegirlo en un pedido: el consecutivo que va en el
    /// campo de solo lectura y las dos direcciones del maestro.
    ///
    /// Se consultan en el momento de elegir y no se leen de la tabla que el combo ya tiene
    /// cargada, para que lo que se pantalla sea lo que el maestro tiene ahora y no lo que tenia
    /// cuando se abrio el formulario.
    /// </summary>
    public sealed class ClienteDatos
    {
        /// <summary>Consecutivo del cliente, ya formateado a 4 digitos.</summary>
        public string Consecutivo { get; init; } = string.Empty;

        /// <summary>Direccion de facturacion, la que va en Bill To.</summary>
        public string DireccionFacturacion { get; init; } = string.Empty;

        /// <summary>Direccion de entrega, la que va en Ship To.</summary>
        public string DireccionEntrega { get; init; } = string.Empty;
    }
}
