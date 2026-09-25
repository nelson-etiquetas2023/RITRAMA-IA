using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService
{
    public interface IProduccionService
    {
        string ErrorMsg { get; }

        Task<DataTable> LoadDataRollID(string ocExcluir = "");
        Task<DataTable> BuscarRollId(string columna, string texto, string ocExcluir = "");
        Task<DataSet> LoadDataOC();
        bool GuardarEncabezadoOrdenCorte(Orden OrdenCorte);
        bool GuardarCortes(List<Corte> cortes);
        bool GuardarRollos(List<RolloCortado> rollos);
        bool GuardarOrdenCompleta(Orden orden, List<Corte>? cortes, List<RolloCortado>? rollos);
        int GetAndIncrementConsecOC();
        bool UpdateStatusDocumentOC(int stepchange, string oc);
        int BuscarConsecOC();
        bool UpdateConsecOC(string consec);
        int BuscarUniqueCodeConsec();
        void UpdateUniqueCodeRollosCortados(List<RolloCortado> lista);
        bool UpdateUniqueCodeBD(string consec);
        List<int> ValidarRangoUniqueCodeGlobal(int inicio, int fin, string numeroOc);
        (int Primero, int Ultimo) GuardarEtiquetado(List<RolloCortado> rollos, string numeroOc, string rollidMaster1, double consumoMaster1, double desperdicio1, string tipoMaster1, string rollidMaster2, double consumoMaster2, double desperdicio2, string tipoMaster2, bool twoMasters);
        public bool CheckOperatorDefault(string id, string name);
        public bool OrdenUpdateCodePerson(string orden, string code_person);
        public bool UpdateOrdenCorte(Orden orden);
        public void AplicarReglaRestanteOC(Orden orden);
        public double ConsumoComprometidoOtrosOC(string rollid, int? ocExcluir);
        public double ObtenerLargoOriginalMaster(string rollid);
        public Task<bool> UpdateInventaryMasterInitial(object objeto);
        public Task<DataTable?> LoadTableMasterInic();
        public Task<bool> UpdateDetailsConsumosMasterIniciales(string rollid, string orden, double length_consumo, DateTime fecha_reg, bool desperdicio);
        public Task<DataTable?> LoadDataDetailsConsumosMasterInic(string rollid);
        public void Update_Items_Orden_Corte(List<RolloCortado> rollos);
        public void Update_Header_Documnet_OC(Orden orden);
        public void RollosCortadosDispobnibles(string oc);
        public Task<bool> ActualizarInventariosMasterAsync(string rollid, string orden, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMaster, string ocNumero);
        Task<bool> ReasignarConsumoMasterAsync(string orden, string rollidAnterior, string rollidNuevo, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMasterAnterior, string tipoMasterNuevo);
        public void GuardarConfigVueltas(List<ConfigVueltas> lista);
        public void UpdateConfigVueltas(List<ConfigVueltas> lista);
        public List<ConfigVueltas> GetConfigVueltas(string oc);
        bool AnularOrdenCorte(string numero_oc);

    }
}
