using System.Data;

namespace Ritrama2025.Services.ClienteService
{
    /// <summary>
    /// Contrato del servicio de clientes: listado para el módulo de Clientes.
    /// </summary>
    public interface IClienteService
    {
        /// <summary>
        /// Carga todos los clientes (activos y desactivados): las tres columnas del
        /// grid (customer_id, customer_name, status) y el resto de campos de la tabla
        /// customer que pinta la página de detalle.
        /// </summary>
        Task<DataTable> LoadListadoAsync(CancellationToken cancellationToken = default);
    }
}
