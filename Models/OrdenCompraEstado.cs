namespace Ritrama2025.Models
{
    public static class OrdenCompraEstado
    {
        public const string Creado = "creado";
        public const string Ordenado = "ordenado";
        public const string Parcial = "parcial";
        public const string Recibido = "recibido";
        public const string Cancelado = "cancelado";

        public static readonly IReadOnlyList<string> EstadosValidos = new[]
        {
            Creado, Ordenado, Parcial, Recibido, Cancelado
        };
    }
}
