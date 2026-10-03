using Ritrama2025.Models;
using Ritrama2025.Services.SeguridadService;

namespace Ritrama2025.Tests.Stubs;

/// <summary>
/// ISeguridadService en memoria para las pruebas de FrmUsuarios. Solo los metodos que
/// la UI invoca devuelven datos; el resto lanza NotSupportedException porque llamarlos
/// desde el formulario seria un defecto, no una prueba.
/// </summary>
internal sealed class SeguridadServiceStub : ISeguridadService
{
    private readonly List<Usuario> _usuarios;
    private readonly List<Role> _roles;

    public SeguridadServiceStub(
        List<Usuario>? usuarios = null,
        List<Role>? roles = null,
        Dictionary<int, List<int>>? rolesPorUsuario = null)
    {
        _usuarios = usuarios ?? [];
        _roles = roles ?? [];
        RolesPorUsuario = rolesPorUsuario ?? [];
        ErrorAlListar = null;
    }

    /// <summary>Asignacion de roles por usuario: userId -> roleIds.</summary>
    public Dictionary<int, List<int>> RolesPorUsuario { get; }

    /// <summary>
    /// Cuando esta seteado, GetUsuariosAsync lo propaga como fallo. Sirve para probar
    /// que el formulario sigue utilizable si la base no responde.
    /// </summary>
    public Exception? ErrorAlListar { get; init; }

    /// <summary>Cuentas de llamadas: el formulario no deberia re-consultar al escribir.</summary>
    public int LlamadasGetUsuarios { get; private set; }

    public int LlamadasGetRoles { get; private set; }

    /// <summary>
    /// Lo que se leyo del catalogo, para poder fechar las casillas de Editar sin
    /// volver a pegarle al servicio.
    /// </summary>
    public List<Role> Roles() => _roles;

    /// <summary>Ultimo usuario que se intento crear, con su password y sus roleIds.</summary>
    public Usuario? UsuarioCreado { get; private set; }

    public string? PasswordCreada { get; private set; }

    public List<int> RoleIdsCreados { get; private set; } = [];

    /// <summary>Ultimo usuario que se intento actualizar.</summary>
    public Usuario? UsuarioActualizado { get; private set; }

    /// <summary>
    /// Cuando FallarAlCrear / FallarAlActualizar estan, el stub devuelve el mismo
    /// fallo que devuelve SeguridadService: -1 y false, no una excepcion. Asi el
    /// formulario se prueba contra el contrato real.
    /// </summary>
    public bool FallarAlCrear { get; init; }

    public bool FallarAlActualizar { get; init; }

    /// <summary>Id que devuelve CreateUsuarioAsync cuando tiene exito.</summary>
    public int IdCreado { get; init; } = 99;

    public Task<List<Usuario>> GetUsuariosAsync()
    {
        LlamadasGetUsuarios++;
        return ErrorAlListar is null
            ? Task.FromResult(_usuarios)
            : Task.FromException<List<Usuario>>(ErrorAlListar);
    }

    public Task<List<Role>> GetRolesAsync()
    {
        LlamadasGetRoles++;
        return Task.FromResult(_roles);
    }

    public Task<List<int>> GetUsuarioRoleIdsAsync(int userId)
        => Task.FromResult(RolesPorUsuario.TryGetValue(userId, out List<int>? ids) ? ids : []);

    public Task<Usuario?> GetUsuarioByIdAsync(int userId)
        => Task.FromResult(_usuarios.FirstOrDefault(u => u.UserId == userId));

    public Task<bool> HasPermissionAsync(int userId, string modulo, string accion)
        => Task.FromResult(true);

    public Task<Usuario?> LoginAsync(string username, string password) => throw new NotSupportedException();

    public Task LogLoginAsync(int? userId, bool exitoso) => throw new NotSupportedException();

    public Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword) => throw new NotSupportedException();

    public Task<bool> ResetPasswordAsync(int userId, string newPassword) => throw new NotSupportedException();

    public Task<bool> CambiarContrasenaPrimerLoginAsync(int userId, string newPassword) => throw new NotSupportedException();

    public Task<int> CreateUsuarioAsync(Usuario usuario, string password, List<int> roleIds)
    {
        UsuarioCreado = usuario;
        PasswordCreada = password;
        RoleIdsCreados = [.. roleIds];

        if (FallarAlCrear)
        {
            return Task.FromResult(-1);
        }

        // Alta real: el usuario pasa a existir y hay que recargarlo del "almacen"
        // para que el listado lo muestre sin volver a pegarle al servicio.
        Usuario copia = new()
        {
            UserId = IdCreado,
            Username = usuario.Username,
            NombreCompleto = usuario.NombreCompleto,
            Email = usuario.Email,
            TituloCargo = usuario.TituloCargo,
            Departamento = usuario.Departamento,
            Activo = usuario.Activo
        };

        _usuarios.Add(copia);
        RolesPorUsuario[IdCreado] = [.. roleIds];

        return Task.FromResult(IdCreado);
    }

    public Task<bool> UpdateUsuarioAsync(Usuario usuario)
    {
        UsuarioActualizado = usuario;

        if (FallarAlActualizar)
        {
            return Task.FromResult(false);
        }

        Usuario? guardado = _usuarios.FirstOrDefault(u => u.UserId == usuario.UserId);
        if (guardado is not null)
        {
            guardado.Username = usuario.Username;
            guardado.NombreCompleto = usuario.NombreCompleto;
            guardado.Email = usuario.Email;
            guardado.TituloCargo = usuario.TituloCargo;
            guardado.Departamento = usuario.Departamento;
            guardado.Activo = usuario.Activo;
            RolesPorUsuario[usuario.UserId] = [.. usuario.Roles.Select(r => r.RoleId)];
        }

        return Task.FromResult(true);
    }

    public Task<bool> DeleteUsuarioAsync(int userId) => throw new NotSupportedException();

    public Task<int> CreateRoleAsync(Role role) => throw new NotSupportedException();

    public Task<bool> UpdateRoleAsync(Role role) => throw new NotSupportedException();

    public Task<bool> DeleteRoleAsync(int roleId) => throw new NotSupportedException();

    public Task<List<int>> GetRolePermisoIdsAsync(int roleId) => throw new NotSupportedException();

    public Task<bool> UpdateRolePermisosAsync(int roleId, List<int> permisoIds) => throw new NotSupportedException();

    public Task<List<Permiso>> GetPermisosAsync() => throw new NotSupportedException();

    public Task<List<string>> GetUserPermisosAsync(int userId) => throw new NotSupportedException();
}