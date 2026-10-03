using System.Data;
using Ritrama2025.Core;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ClienteService
{
    /// <summary>
    /// Contrato del servicio de clientes: listado para el módulo de Clientes,
    /// y operaciones de alta/edición con validación de dominio.
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Carga todos los clientes (activos y desactivados): las tres columnas del
        /// grid (customer_id, customer_name, status) y el resto de campos de la tabla
        /// customer que pinta la página de detalle.
        /// </summary>
        Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default);

        // ── Operaciones de escritura con Result<T> ──

        /// <summary>
        /// Inserta un cliente con validación de dominio y Result para no usar excepciones como control de flujo.
        /// </summary>
        Task<Result<bool>> AddValidatedAsync(Cliente cliente, CancellationToken cancellationToken = default);

        /// <summary>
        /// Actualiza un cliente existente, incluido su estado (activar/desactivar).
        /// </summary>
        Task<Result<bool>> UpdateValidatedAsync(Cliente cliente, CancellationToken cancellationToken = default);

        /// <summary>
        /// Verifica si un código de cliente ya existe en BD.
        /// </summary>
        Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default);

        /// <summary>
        /// Valida solo reglas de dominio (sin I/O) para un cliente. Útil para UI antes de llamar a Add/Update.
        /// </summary>
        Result ValidateCliente(Cliente cliente);
    }
}