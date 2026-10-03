using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.OrdenesCompras
{
    /// <summary>
    /// Contrato del servicio de órdenes de compra: listado, combos, detalle, guardado,
    /// anulación y actualización de estado.
    /// </summary>
    public interface IOrdenesComprasService
    {
        /// <summary>Error del último método ejecutado, si lo hubo.</summary>
        string? ErrorMsg { get; }

        Task<DataTable> LoadDataOrdenesCompra(CancellationToken ct = default);
        Task<DataTable> LoadDataProveedores(CancellationToken ct = default);
        Task<ProveedorDatos?> BuscarProveedorAsync(string? proveedorId, CancellationToken ct = default);
        Task<DataTable> LoadDataOrdenCompraDetalle(string numero, CancellationToken ct = default);
        Task<string> GetProximoNumeroOrdenCompra(CancellationToken ct = default);
        bool SaveOrdenCompraCompleto(OrdenCompra orden);
        bool ActualizarOrdenCompraCompleto(OrdenCompra orden);
        bool AnularOrdenCompra(string numero);
        bool RestaurarOrdenCompra(string numero);
        bool ActualizarEstadoOrdenCompra(string numero, string estado);
    }
}
