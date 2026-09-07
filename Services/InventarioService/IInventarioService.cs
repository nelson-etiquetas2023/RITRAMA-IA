using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.InventarioService
{
    public interface IInventarioService
    {
        bool SaveMasterInitialDB(ProductMAP producto);
        bool ValidProductid(string id);
        bool InsertProduct(Product producto);
        Task<DataTable?> LoadMasterInventario();
        Task<DataTable?> LoadRolloCortadoInventaerio();
        bool BorrarMasterDB(string rollid);
        bool DropTableInit(int indexTable);
        bool ValidRollId(string rollid);
        bool SaveRolloCortado(RolloCortado rollo);
    }
}
