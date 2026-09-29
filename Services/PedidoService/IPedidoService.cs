using System.Data;

namespace Ritrama2025.Services.PedidoService
{
    public interface IPedidoService
    {
        Task<DataTable> LoadDataPedidos(CancellationToken cancellationToken = default);
        Task<DataTable> LoadDataCustomers(CancellationToken cancellationToken = default);

        /// <summary>
        /// Consulta en la base el consecutivo y las dos direcciones de un cliente. Se llama al
        /// elegirlo en el combo, para que la pantalla muestre lo que el maestro tiene en ese
        /// momento y no lo que se cargo al abrir el formulario. Devuelve null si el cliente no
        /// existe o esta anulado.
        /// </summary>
        Task<Models.ClienteDatos?> BuscarClienteAsync(Guid customerId, CancellationToken cancellationToken = default);
        Task<DataTable> LoadDataVendors(CancellationToken cancellationToken = default);
        Task<DataTable> LoadDataPedidoDetalle(string numero, CancellationToken cancellationToken = default);

        /// <summary>
        /// Numero que tendra el proximo pedido. No reserva nada: el numero real se asigna en
        /// <see cref="SavePedidoCompleto"/> y puede diferir de este si otro usuario guarda antes.
        /// </summary>
        Task<string> GetProximoNumeroPedido(CancellationToken cancellationToken = default);

        /// <summary>
        /// Guarda el pedido completo y le asigna el numero. Al terminar, el numero real quedo en
        /// <see cref="Models.Pedido.Numero"/>.
        /// </summary>
        bool SavePedidoCompleto(Models.Pedido pedido);

        /// <summary>
        /// Actualiza el encabezado y las lineas de un pedido existente sin cambiarle el numero:
        /// el encabezado se reemplaza con un UPDATE y el detalle se borra y se vuelve a insertar.
        /// Devuelve false si los datos no son validos o si el pedido ya no existe (fue modificado
        /// por otro usuario); el motivo queda en <see cref="ErrorMsg"/>.
        /// </summary>
        bool ActualizarPedidoCompleto(Models.Pedido pedido);

        /// <summary>
        /// Motivo del ultimo fallo de una operacion de escritura, para que la pantalla pueda
        /// mostrarlo sin conocer la clase concreta.
        /// </summary>
        string? ErrorMsg { get; }
        bool AnularPedido(string numero);

        /// <summary>
        /// Devuelve un pedido anulado a estado activo (anulado = 0). Espejo inverso de
        /// <see cref="AnularPedido"/>, usado por el switch de la pantalla de Pedidos.
        /// </summary>
        bool RestaurarPedido(string numero);
        bool ActualizarEstadoPedido(string numero, string estado);
    }
}
