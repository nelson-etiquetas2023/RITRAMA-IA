using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.InventarioService
{
    public interface IInventarioService
    {
        bool SaveMasterInitialDB(ProductMAP producto);
        bool ValidProductid(string id);
        bool InsertProduct(Product producto);
        // Busqueda directa a SQL Server con filtros (sin carga masiva local).
        // Todos los parametros son opcionales; los nulos o vacios no filtran.
        Task<DataTable?> BuscarMasterInventario(string? rollid, string? productId, string? productName, string? ubicacion, string? estado);
        Task<DataTable?> BuscarRollosCortadosInventario(string? rollid, string? productId, string? productName, string? ubicacion, string? uniqueCode, string? codePerson, string? numeroOC);
        bool BorrarMasterDB(string rollid);
        bool DropTableInit(int indexTable);
        bool ValidRollId(string rollid);
        bool SaveRolloCortado(RolloCortado rollo);
    }
}
