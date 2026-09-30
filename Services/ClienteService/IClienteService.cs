using System.Data;

namespace Ritrama2025.Services.ClienteService
{
    /// <summary>
    /// Contrato del servicio de clientes: listado para el módulo de Clientes.
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Carga el listado de clientes (activos y desactivados) con el estado en
        /// texto: una fila por cliente con customer_id, customer_name y status.
        /// </summary>
        Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default);
    }
}
