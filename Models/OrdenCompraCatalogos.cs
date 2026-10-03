namespace Ritrama2025.Models
{
    public static class OrdenCompraCatalogos
    {
        public static readonly string[] CondicionesPago =
        {
            "contado", "7 dias", "15 dias", "30 dias", "45 dias", "60 dias", "credito"
        };

        public static readonly string[] Prioridad = { "normal", "urgente" };

        public static bool EsContado(string? condicion)
        {
            return string.Equals(condicion, "contado", StringComparison.OrdinalIgnoreCase);
        }
    }
}
