using Ritrama2025.Models;

namespace Ritrama2025.Helpers
{
    public static class SesionActual
    {
        public static Usuario? Usuario { get; set; }
        public static List<string> Permisos { get; set; } = [];
        public static bool IsAuthenticated => Usuario != null;

        public static void Clear()
        {
            Usuario = null;
            Permisos.Clear();
        }
    }
}
