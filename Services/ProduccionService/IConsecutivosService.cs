using Microsoft.Data.SqlClient;

namespace Ritrama2025.Services.ProduccionService
{
    public interface IConsecutivosService
    {
        int GetAndIncrementConsecOC();
        int GetAndIncrementConsecOCTransactional(SqlConnection conn, SqlTransaction transaction);
        int BuscarUniqueCodeConsec();
        int BuscarConsecOC();
        bool UpdateConsecOC(string consec);
        bool UpdateUniqueCodeBD(string consec);
    }
}
