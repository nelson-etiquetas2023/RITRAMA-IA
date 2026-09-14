using Ritrama2025.Models;

namespace Ritrama2025.Services.SeguridadService
{
    public interface ISeguridadService
    {
        Task<Usuario?> LoginAsync(string username, string password);
        // userId nulo = intento fallido sin usuario valido (la columna
        // auditoria_login.user_id es NULLABLE y tiene FK a usuarios: insertar
        // un 0 rompia la FK). NULL deja el registro sin violar la FK.
        Task LogLoginAsync(int? userId, bool exitoso);
        Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword);
        Task<bool> ResetPasswordAsync(int userId, string newPassword);
        Task<bool> CambiarContrasenaPrimerLoginAsync(int userId, string newPassword);
        Task<List<Usuario>> GetUsuariosAsync();
        Task<Usuario?> GetUsuarioByIdAsync(int userId);
        Task<int> CreateUsuarioAsync(Usuario usuario, string password, List<int> roleIds);
        Task<bool> UpdateUsuarioAsync(Usuario usuario);
        Task<bool> DeleteUsuarioAsync(int userId);
        Task<List<Role>> GetRolesAsync();
        Task<int> CreateRoleAsync(Role role);
        Task<bool> UpdateRoleAsync(Role role);
        Task<bool> DeleteRoleAsync(int roleId);
        Task<List<int>> GetRolePermisoIdsAsync(int roleId);
        Task<List<int>> GetUsuarioRoleIdsAsync(int userId);
        Task<bool> UpdateRolePermisosAsync(int roleId, List<int> permisoIds);
        Task<List<Permiso>> GetPermisosAsync();
        Task<List<string>> GetUserPermisosAsync(int userId);
        Task<bool> HasPermissionAsync(int userId, string modulo, string accion);
    }
}
