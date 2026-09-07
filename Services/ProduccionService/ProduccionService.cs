using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProduccionService;

public class ProduccionService : IProduccionService
{
    private readonly IOrdenCorteService _ordenCorte;
    private readonly IConsecutivosService _consecutivos;
    private readonly IConsumoMasterService _consumoMaster;

    public ProduccionService(IOrdenCorteService ordenCorte, IConsecutivosService consecutivos, IConsumoMasterService consumoMaster)
    {
        _ordenCorte = ordenCorte;
        _consecutivos = consecutivos;
        _consumoMaster = consumoMaster;
    }

    public Task<DataTable> LoadDataRollID() => _ordenCorte.LoadDataRollID();

    public Task<DataTable> BuscarRollId(string columna, string texto) => _ordenCorte.BuscarRollId(columna, texto);

    public Task<DataSet> LoadDataOC() => _ordenCorte.LoadDataOC();

    public bool GuardarEncabezadoOrdenCorte(Orden OrdenCorte) => _ordenCorte.GuardarEncabezadoOrdenCorte(OrdenCorte);

    public bool GuardarCortes(List<Corte> cortes) => _ordenCorte.GuardarCortes(cortes);

    public bool GuardarRollos(List<RolloCortado> rollos) => _ordenCorte.GuardarRollos(rollos);

    public bool GuardarOrdenCompleta(Orden orden, List<Corte>? cortes, List<RolloCortado>? rollos) => _ordenCorte.GuardarOrdenCompleta(orden, cortes, rollos);

    public int GetAndIncrementConsecOC() => _consecutivos.GetAndIncrementConsecOC();

    public bool UpdateStatusDocumentOC(int stepchange, string oc) => _ordenCorte.UpdateStatusDocumentOC(stepchange, oc);

    public int BuscarConsecOC() => _consecutivos.BuscarConsecOC();

    public bool UpdateConsecOC(string consec) => _consecutivos.UpdateConsecOC(consec);

    public int BuscarUniqueCodeConsec() => _consecutivos.BuscarUniqueCodeConsec();

    public void UpdateUniqueCodeRollosCortados(List<RolloCortado> lista) => _ordenCorte.UpdateUniqueCodeRollosCortados(lista);

    public bool UpdateUniqueCodeBD(string consec) => _consecutivos.UpdateUniqueCodeBD(consec);

    public List<int> ValidarRangoUniqueCodeGlobal(int inicio, int fin, string numeroOc) => _ordenCorte.ValidarRangoUniqueCodeGlobal(inicio, fin, numeroOc);

    public bool CheckOperatorDefault(string id, string name) => _ordenCorte.CheckOperatorDefault(id, name);

    public bool OrdenUpdateCodePerson(string orden, string code_person) => _ordenCorte.OrdenUpdateCodePerson(orden, code_person);

    public bool UpdateOrdenCorte(Orden orden) => _ordenCorte.UpdateOrdenCorte(orden);

    public Task<bool> UpdateInventaryMasterInitial(object objeto) => _consumoMaster.UpdateInventaryMasterInitial(objeto);

    public Task<DataTable?> LoadTableMasterInic() => _consumoMaster.LoadTableMasterInic();

    public Task<bool> UpdateDetailsConsumosMasterIniciales(string rollid, string orden, double length_consumo, DateTime fecha_reg, bool desperdicio) => _consumoMaster.UpdateDetailsConsumosMasterIniciales(rollid, orden, length_consumo, fecha_reg, desperdicio);

    public Task<DataTable?> LoadDataDetailsConsumosMasterInic(string rollid) => _consumoMaster.LoadDataDetailsConsumosMasterInic(rollid);

    public void Update_Items_Orden_Corte(List<RolloCortado> rollos) => _ordenCorte.Update_Items_Orden_Corte(rollos);

    public void Update_Header_Documnet_OC(Orden orden) => _ordenCorte.Update_Header_Documnet_OC(orden);

    public void RollosCortadosDispobnibles(string oc) => _ordenCorte.RollosCortadosDispobnibles(oc);

    public Task<bool> ActualizarInventariosMasterAsync(string rollid, string orden, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMaster, string ocNumero) => _consumoMaster.ActualizarInventariosMasterAsync(rollid, orden, consumoReal, consumoDesperdicio, desperdicio, tipoMaster, ocNumero);

    public Task<bool> ReasignarConsumoMasterAsync(string orden, string rollidAnterior, string rollidNuevo, double consumoReal, double consumoDesperdicio, bool desperdicio, string tipoMasterAnterior, string tipoMasterNuevo) => _consumoMaster.ReasignarConsumoMasterAsync(orden, rollidAnterior, rollidNuevo, consumoReal, consumoDesperdicio, desperdicio, tipoMasterAnterior, tipoMasterNuevo);

    public void GuardarConfigVueltas(List<ConfigVueltas> lista) => _ordenCorte.GuardarConfigVueltas(lista);

    public void UpdateConfigVueltas(List<ConfigVueltas> lista) => _ordenCorte.UpdateConfigVueltas(lista);

    public List<ConfigVueltas> GetConfigVueltas(string oc) => _ordenCorte.GetConfigVueltas(oc);

    public bool AnularOrdenCorte(string numero_oc) => _ordenCorte.AnularOrdenCorte(numero_oc);
}