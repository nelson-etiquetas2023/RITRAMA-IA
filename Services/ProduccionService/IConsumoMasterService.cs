using System.Data;

namespace Ritrama2025.Services.ProduccionService
{
    public interface IConsumoMasterService
    {
        Task<bool> UpdateInventaryMasterInitial(object objeto);
        Task<DataTable?> LoadTableMasterInic();
        Task<bool> UpdateDetailsConsumosMasterIniciales(string rollid, string orden, double length_consumo, DateTime fecha_reg, bool desperdicio);
        Task<DataTable?> LoadDataDetailsConsumosMasterInic(string rollid);
        Task<bool> ActualizarInventariosMasterAsync(string rollid, string orden, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMaster, string ocNumero);
        Task<bool> ReasignarConsumoMasterAsync(string orden, string rollidAnterior, string rollidNuevo, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMasterAnterior, string tipoMasterNuevo);
    }
}