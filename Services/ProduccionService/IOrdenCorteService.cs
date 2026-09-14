using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService
{
    public interface IOrdenCorteService
    {
        Task<DataTable> LoadDataRollID(string ocExcluir = "");
        Task<DataTable> BuscarRollId(string columna, string texto, string ocExcluir = "");
        Task<DataSet> LoadDataOC();
        bool GuardarEncabezadoOrdenCorte(Orden OrdenCorte);
        bool GuardarCortes(List<Corte> cortes);
        bool GuardarRollos(List<RolloCortado> rollos);
        bool GuardarOrdenCompleta(Orden orden, List<Corte>? cortes, List<RolloCortado>? rollos);
        bool UpdateStatusDocumentOC(int stepchange, string oc);
        void UpdateUniqueCodeRollosCortados(List<RolloCortado> rollos);
        (int Primero, int Ultimo) GuardarEtiquetado(List<RolloCortado> rollos, string numeroOc, string rollidMaster1, double consumoMaster1, double desperdicio1, string tipoMaster1, string rollidMaster2, double consumoMaster2, double desperdicio2, string tipoMaster2, bool twoMasters);
        List<int> ValidarRangoUniqueCodeGlobal(int inicio, int fin, string numeroOc);
        bool CheckOperatorDefault(string id, string name);
        void AddOperatorDefault(string id, string name);
        bool OrdenUpdateCodePerson(string orden, string code_person);
        bool UpdateOrdenCorte(Orden orden);
        void AplicarReglaRestanteOC(Orden orden);
        double ConsumoComprometidoOtrosOC(string rollid, int? ocExcluir);
        double ObtenerLargoOriginalMaster(string rollid);
        void Update_Items_Orden_Corte(List<RolloCortado> rollos);
        void Update_Header_Documnet_OC(Orden orden);
        void RollosCortadosDispobnibles(string oc);
        void GuardarConfigVueltas(List<ConfigVueltas> lista);
        void UpdateConfigVueltas(List<ConfigVueltas> lista);
        List<ConfigVueltas> GetConfigVueltas(string oc);
        bool AnularOrdenCorte(string numero_oc);
    }
}