namespace Ritrama2025.Models
{
    public class Permiso
    {
        public int PermisoId { get; set; }
        public string Modulo { get; set; } = null!;
        public string Accion { get; set; } = null!;
        public string? Descripcion { get; set; }
    }
}
