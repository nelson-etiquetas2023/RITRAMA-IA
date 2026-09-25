
using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.MateriaPrima
{
    public interface IServiceMateriaPrima
    {
        Task<DataSet> LoadData();
        Task LoadProducts();
        Task LoadTableHeaderMateriaPrima();
        Task LoadTableDetailsMateriaPrima();
        Task LoadTableProveedores();
        Task LoadTableTransportista();
        Task SetRelationsTables();
        bool GuardarOrden(OrdenMP orden);
        int LoadConsecOrden(string filtro);
        bool UpdateConsecOrden(string NumConsec);
        bool CloseOrder(string orden);
        bool UpDateLogsNotes(string orden, string logText);
        bool AnularOrden(string orden);
    }
}
