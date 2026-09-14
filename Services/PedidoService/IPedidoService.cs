using System.Data;

namespace Ritrama2025.Services.PedidoService
{
    public interface IPedidoService
    {
        Task<DataTable> LoadDataPedidos(CancellationToken cancellationToken = default);
        Task<int> GetNewNumeroPedido(CancellationToken cancellationToken = default);
        bool SavePedidoCompleto(Models.Pedido pedido);
        bool AnularPedido(int numero);
        bool ActualizarEstadoPedido(int numero, string estado);
    }
}
