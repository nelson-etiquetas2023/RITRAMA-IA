using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.DespachoService.DespachoService
{
    public interface IDespachoService
    {
        Task<DataSet> LoadDataDespachos(CancellationToken cancellationToken = default);
        string GetNumberConsec();
        Task<string> GetNumberConsecAsync(CancellationToken ct = default);
        decimal GetRatioProductById(string product_id);
        void AddDocumentDespacho(Despacho document);
        void AddPickingListDespacho(List<RolloCortado> PickingList);
        void AddPaletDetailsDespacho(List<Paleta> paleta);
        public void AddItemsDespacho(List<ItemsDespacho> items);
        public void UpDateInventoryRC(List<RolloCortado> items, string orden, DateTime fecha);
        bool SaveDespachoCompleto(Despacho document, DateTime fecha);
    }
}
