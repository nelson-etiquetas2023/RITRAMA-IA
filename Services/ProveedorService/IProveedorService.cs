using System.Data;

namespace Ritrama2025.Services.ProveedorService
{
    /// <summary>
    /// Contrato del servicio de proveedores: listado para el módulo de Proveedores.
    /// </summary>
    public interface IProveedorService
    {
        /// <summary>
        /// Carga todos los proveedores (activos y anulados): las tres columnas del
        /// grid (Proveedor_Id, Proveedor_Name, status) y el resto de campos de la
        /// tabla provider que pinta la página de detalle.
        /// </summary>
        Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default);
    }
}
