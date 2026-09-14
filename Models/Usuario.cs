namespace Ritrama2025.Models
{
    public class Usuario
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? Email { get; set; }
        public bool Activo { get; set; } = true;
        public bool PrimerLogin { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public List<Role> Roles { get; set; } = [];
    }
}
