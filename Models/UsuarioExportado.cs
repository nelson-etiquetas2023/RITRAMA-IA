
namespace Ritrama2025.Models
{
    /// <summary>
    /// Fila del Excel de usuarios. No se exporta la entidad <see cref="Usuario"/> tal cual:
    /// su PasswordHash saldria en claro en la hoja, que es un informe de personas y no un
    /// volcado de la tabla. Aqui la clave NO VIAJA, el estado va ya resuelto en texto y el
    /// rol como nombres unidos. Los nombres de las propiedades SON los titulos de las
    /// columnas, porque ExportDataService.ExportToExcel&lt;T&gt; escribe el nombre de
    /// propiedad como encabezado.
    /// </summary>
    public class UsuarioExportado
    {
        public int UserId { get; set; }

        public string Usuario { get; set; } = string.Empty;

        public string Nombre { get; set; } = string.Empty;

        public string Email { get; set; } = string.Empty;

        public string Cargo { get; set; } = string.Empty;

        public string Departamento { get; set; } = string.Empty;

        /// <summary>Roles del usuario, unidos con ", " tal como se ven en el listado.</summary>
        public string Roles { get; set; } = string.Empty;

        /// <summary>Activo o Inactivo, los mismos dos textos del grid.</summary>
        public string Estado { get; set; } = string.Empty;

        public DateTime FechaCreacion { get; set; }

        /// <summary>
        /// Ultimo acceso. Nulo (celda vacia) en un usuario que nunca entro: no es un
        /// error, es un dato que todavia no existe.
        /// </summary>
        public DateTime? UltimoLogin { get; set; }
    }
}
