using System.Data;

namespace Ritrama2025.Services.VendedorService
{
    /// <summary>
    /// Contrato del servicio de vendedores: listado para el módulo de Vendedores.
    /// </summary>
    public interface IVendedorService
    {
        /// <summary>
        /// Carga todos los vendedores (activos y anulados): las tres columnas del
        /// grid (vendor_id, vendor_name, status) y el resto de campos de la tabla
        /// vendedor que pinta la página de detalle.
        /// </summary>
        Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default);
    }
}
