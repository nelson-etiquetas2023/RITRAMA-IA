
using Ritrama2025.Models;

namespace Ritrama2025.Services.CommonService
{
    public interface ICommonService
    {
        Task<List<RolloCortado>> GetDataRolloCortado(List<RolloCortado> lista);
        void SaveTransportEntity(string Id, string Name);
        void DeleteTransportEntity(string Id);
        void SaveChoferEntity(string Id, string Name);
        void DeleteChoferEntity(string Id);
        void SaveCamionEntity(string Id, string Name);
        void DeleteCamionEntity(string Id);
        void SaveProvaiderEntity(string Id, string Name);
        void DeleteProvaiderEntity(string Id);
        void SavePersonEntity(string Id, string Name);
        void DeletePersonEntity(string Id);
        void SaveOperatorEntity(string Id, string Name);
        void DeleteOperatorEntity(string Id);
        void SaveCustomerEntity(string Id, string Name);
        void SaveVendedorEntity(string Id, string Name);
        bool DocumentCheckWriteOC(DocumentCheckOC doc);
        DocumentCheckOC DocumentCheckReadOC(string oc);
        RolloCortado SearchCodigoUnico(string id);

    }
}
