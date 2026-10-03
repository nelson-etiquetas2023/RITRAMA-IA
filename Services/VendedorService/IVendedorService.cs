using System.Data;
using Ritrama2025.Core;
using Ritrama2025.Models;

namespace Ritrama2025.Services.VendedorService
{
    /// <summary>
    /// Contrato del servicio de vendedores: listado para el módulo de Vendedores,
    /// y operaciones de alta/edición con validación de dominio.
    /// </summary>
    public interface IVendedorService
    {
        /// <summary>
        /// Carga todos los vendedores (activos y anulados): las tres columnas del
        /// grid (vendor_id, vendor_name, status) y el resto de campos de la tabla
        /// vendedor que pinta la página de detalle.
        /// </summary>
        Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default);

        // ── Operaciones de escritura con Result<T> ──

        /// <summary>
        /// Inserta un vendedor con validación de dominio y Result para no usar excepciones como control de flujo.
        /// </summary>
        Task<Result<bool>> AddValidatedAsync(Vendedor vendedor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza un vendedor existente, incluido su estado (activar/desactivar).
        /// </summary>
        Task<Result<bool>> UpdateValidatedAsync(Vendedor vendedor, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un código de vendedor ya existe en BD.
        /// </summary>
        Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida solo reglas de dominio (sin I/O) para un vendedor. Útil para UI antes de llamar a Add/Update.
        /// </summary>
        Result ValidateVendedor(Vendedor vendedor);
    }
}