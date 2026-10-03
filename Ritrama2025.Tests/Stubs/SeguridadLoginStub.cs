using Ritrama2025.Models;
using Ritrama2025.Services.SeguridadService;

namespace Ritrama2025.Tests.Stubs;

/// <summary>
/// ISeguridadService en memoria SOLO para el login de FrmLogin. Replica el contrato
/// del servicio real: busca al usuario sin distinguir mayusculas, no devuelve a los
/// desactivados y compara la clave contra el hash con BCrypt. El resto de los metodos
/// lanza NotSupportedException: FrmLogin no deberia tocarlos.
///
/// Va aparte de <see cref="SeguridadServiceStub"/> (el de FrmUsuarios) para no
/// relajar ni su exigencia de fallo ni sus contadores de llamadas.
/// </summary>
internal sealed class SeguridadLoginStub : ISeguridadService
{
    private readonly List<Usuario> _usuarios;

    public SeguridadLoginStub(params Usuario[] usuarios) => _usuarios = [.. usuarios];

    /// <summary>Permisos que devuelve GetUserPermisosAsync tras un login bueno.</summary>
    public List<string> Permisos { get; init; } = [];

    /// <summary>Veces que se pregunto por el padron de usuarios.</summary>
    public int LlamadasLogin { get; private set; }

    /// <summary>Veces que se pidieron los permisos del usuario.</summary>
    public int LlamadasPermisos { get; private set; }

    /// <summary>Username tal cual lo recibio el formulario, para ver si llega recortado.</summary>
    public string? UsernameConsultado { get; private set; }

    /// <summary>user_id de cada intento auditado; null en los fallos (la FK lo exige).</summary>
    public List<int?> UsuariosAuditados { get; } = [];

    /// <summary>Si cada intento auditado salio bien o mal, en el mismo orden.</summary>
    public List<bool> ResultadosAuditados { get; } = [];

    public Task<Usuario?> LoginAsync(string username, string password)
    {
        LlamadasLogin++;
        UsernameConsultado = username;

        Usuario? usuario = _usuarios.FirstOrDefault(u =>
            u.Activo &&
            string.Equals(u.Username?.Trim(), username?.Trim(), StringComparison.OrdinalIgnoreCase));

        if (usuario is null || string.IsNullOrWhiteSpace(usuario.PasswordHash))
        {
            return Task.FromResult<Usuario?>(null);
        }

        try
        {
            bool ok = BCrypt.Net.BCrypt.Verify(password, usuario.PasswordHash);
            return Task.FromResult(ok ? usuario : null);
        }
        catch (Exception)
        {
            // Hash corrupto: credencial invalida, igual que en el servicio real.
            return Task.FromResult<Usuario?>(null);
        }
    }

    public Task LogLoginAsync(int? userId, bool exitoso)
    {
        UsuariosAuditados.Add(userId);
        ResultadosAuditados.Add(exitoso);
        return Task.CompletedTask;
    }

    public Task<List<string>> GetUserPermisosAsync(int userId)
    {
        LlamadasPermisos++;
        return Task.FromResult(Permisos);
    }

    public Task<List<Usuario>> GetUsuariosAsync() => Task.FromResult(_usuarios);

    public Task<Usuario?> GetUsuarioByIdAsync(int userId)
        => Task.FromResult(_usuarios.FirstOrDefault(u => u.UserId == userId));

    public Task<bool> HasPermissionAsync(int userId, string modulo, string accion)
        => Task.FromResult(true);

    public Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword) => throw new NotSupportedException();
    public Task<bool> ResetPasswordAsync(int userId, string newPassword) => throw new NotSupportedException();
    public Task<bool> CambiarContrasenaPrimerLoginAsync(int userId, string newPassword) => throw new NotSupportedException();
    public Task<int> CreateUsuarioAsync(Usuario usuario, string password, List<int> roleIds) => throw new NotSupportedException();
    public Task<bool> UpdateUsuarioAsync(Usuario usuario) => throw new NotSupportedException();
    public Task<bool> DeleteUsuarioAsync(int userId) => throw new NotSupportedException();
    public Task<List<Role>> GetRolesAsync() => throw new NotSupportedException();
    public Task<int> CreateRoleAsync(Role role) => throw new NotSupportedException();
    public Task<bool> UpdateRoleAsync(Role role) => throw new NotSupportedException();
    public Task<bool> DeleteRoleAsync(int roleId) => throw new NotSupportedException();
    public Task<List<int>> GetRolePermisoIdsAsync(int roleId) => throw new NotSupportedException();
    public Task<List<int>> GetUsuarioRoleIdsAsync(int userId) => throw new NotSupportedException();
    public Task<bool> UpdateRolePermisosAsync(int roleId, List<int> permisoIds) => throw new NotSupportedException();
    public Task<List<Permiso>> GetPermisosAsync() => throw new NotSupportedException();
}
