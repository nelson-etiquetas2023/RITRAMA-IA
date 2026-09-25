using System.Data;

namespace Ritrama2025.Services.PedidoService
{
    public interface IPedidoService
    {
        Task<DataTable> LoadDataPedidos(CancellationToken cancellationToken = default);
        Task<DataTable> LoadDataCustomers(CancellationToken cancellationToken = default);
        Task<DataTable> LoadDataVendors(CancellationToken cancellationToken = default);
        Task<DataTable> LoadDataPedidoDetalle(string numero, CancellationToken cancellationToken = default);
        Task<string> GetNewNumeroPedido(CancellationToken cancellationToken = default);
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
