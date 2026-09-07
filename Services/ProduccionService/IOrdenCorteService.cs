using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService
{
    public interface IOrdenCorteService
    {
        Task<DataTable> LoadDataRollID();
        Task<DataTable> BuscarRollId(string columna, string texto);
        Task<DataSet> LoadDataOC();
        bool GuardarEncabezadoOrdenCorte(Orden OrdenCorte);
        bool GuardarCortes(List<Corte> cortes);
        bool GuardarRollos(List<RolloCortado> rollos);
        bool GuardarOrdenCompleta(Orden orden, List<Corte>? cortes, List<RolloCortado>? rollos);
        bool UpdateStatusDocumentOC(int stepchange, string oc);
        void UpdateUniqueCodeRollosCortados(List<RolloCortado> rollos);
        List<int> ValidarRangoUniqueCodeGlobal(int inicio, int fin, string numeroOc);
        bool CheckOperatorDefault(string id, string name);
        void AddOperatorDefault(string id, string name);
        bool OrdenUpdateCodePerson(string orden, string code_person);
        bool UpdateOrdenCorte(Orden orden);
        void Update_Items_Orden_Corte(List<RolloCortado> rollos);
        void Update_Header_Documnet_OC(Orden orden);
        void RollosCortadosDispobnibles(string oc);
        void GuardarConfigVueltas(List<ConfigVueltas> lista);
        void UpdateConfigVueltas(List<ConfigVueltas> lista);
        List<ConfigVueltas> GetConfigVueltas(string oc);
        bool AnularOrdenCorte(string numero_oc);
    }
}