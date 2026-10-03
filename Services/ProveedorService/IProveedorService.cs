using System.Data;
using Ritrama2025.Core;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProveedorService
{
    /// <summary>
    /// Contrato del servicio de proveedores: listado para el módulo de Proveedores,
    /// y operaciones de alta/edición con validación de dominio.
    /// </summary>
    public interface IProveedorService
    {
        /// <summary>
        /// Carga todos los proveedores (activos y anulados): las tres columnas del
        /// grid (Proveedor_Id, Proveedor_Name, status) y el resto de campos de la tabla
        /// provider que pinta la página de detalle.
        /// </summary>
        Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default);

        // ── Operaciones de escritura con Result<T> ──

        /// <summary>
        /// Inserta un proveedor con validación de dominio y Result para no usar excepciones como control de flujo.
        /// </summary>
        Task<Result<bool>> AddValidatedAsync(Proveedor proveedor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza un proveedor existente, incluido su estado (activar/desactivar).
        /// </summary>
        Task<Result<bool>> UpdateValidatedAsync(Proveedor proveedor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un código de proveedor ya existe en BD.
        /// </summary>
        Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida solo reglas de dominio (sin I/O) para un proveedor. Útil para UI antes de llamar a Add/Update.
        /// </summary>
        Result ValidateProveedor(Proveedor proveedor);
    }
}
