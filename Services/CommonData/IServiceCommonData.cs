using System.Data;
using Microsoft.Data.SqlClient;
using Ritrama2025.Models;

namespace Ritrama2025.Services.CommonData
{
    public interface IServiceCommonData
    {
        ObjectQuery CreateObjectQuery(ObjectQuery objectquery, DataSet dataset);
        ObjectQuery CreateObjectProduct(SqlDataAdapter da);
        Task LoadTable(ObjectQuery objectQuery);
        int GetConsecutive(string filtro);
        bool VerificarRollIdNoRepeat(string rollid);
    }
}
