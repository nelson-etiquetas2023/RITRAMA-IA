namespace Ritrama2025.Models
{
    public class Role
    {
        public int RoleId { get; set; }
        public string Nombre { get; set; } = null!;
        public string? Descripcion { get; set; }
        public bool Activo { get; set; } = true;
    }
}
