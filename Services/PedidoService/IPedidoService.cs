using System.Data;

namespace Ritrama2025.Services.PedidoService
{
    public interface IPedidoService
    {
        Task<DataTable> LoadDataPedidos(CancellationToken cancellationToken = default);
        Task<DataTable> LoadDataCustomers(CancellationToken cancellationToken = default);
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
        /// Motivo del ultimo fallo de una operacion de escritura, para que la pantalla pueda
        /// mostrarlo sin conocer la clase concreta.
        /// </summary>
        string? ErrorMsg { get; }
        bool AnularPedido(string numero);
        bool ActualizarEstadoPedido(string numero, string estado);
    }
}
