namespace Ritrama2025.Models
{
    public class Usuario
    {
        public int UserId { get; set; }
        public string Username { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string NombreCompleto { get; set; } = null!;
        public string? Email { get; set; }

        /// <summary>
        /// Puesto que ocupa la persona ("Coordinador de ventas"). Texto libre: no hay
        /// catalogo, se escribe. NULL en las filas anteriores a la migracion.
        /// </summary>
        public string? TituloCargo { get; set; }

        /// <summary>
        /// Area de la empresa a la que pertenece ("Ventas", "Produccion"). Texto libre,
        /// igual que <see cref="TituloCargo"/>. NULL en las filas anteriores a la migracion.
        /// </summary>
        public string? Departamento { get; set; }
        public bool Activo { get; set; } = true;
        public bool PrimerLogin { get; set; } = true;
        public DateTime FechaCreacion { get; set; }
        public DateTime? UltimoLogin { get; set; }
        public List<Role> Roles { get; set; } = [];
    }
}
