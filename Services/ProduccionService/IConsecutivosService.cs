namespace Ritrama2025.Services.ProduccionService
{
    public interface IConsecutivosService
    {
        int GetAndIncrementConsecOC();
        int BuscarUniqueCodeConsec();
        int BuscarConsecOC();
        bool UpdateConsecOC(string consec);
        bool UpdateUniqueCodeBD(string consec);
    }
}