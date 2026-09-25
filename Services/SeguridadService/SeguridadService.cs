using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.SeguridadService
{
    public class SeguridadService : ISeguridadService
    {
        public string StringConnex { get; set; } = null!;
        private readonly IConfiguration _config;

        public SeguridadService(IConfiguration config)
        {
            _config = config ?? throw new ArgumentNullException(nameof(config));
            string ambiente = _config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            StringConnex = _config.GetSection("ConnectionStringsEnvironment")[ambiente]!;
        }

        public async Task<Usuario?> LoginAsync(string username, string password)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT user_id, username, password_hash, nombre_completo, email, 
                                           activo, primer_login, fecha_creacion, ultimo_login
                                    FROM usuarios 
                                    WHERE username = @username AND activo = 1"
                };
                cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.NVarChar, 50) { Value = username });

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    string hash = reader.GetString(2);
                    if (BCrypt.Net.BCrypt.Verify(password, hash))
                    {
                        Usuario usuario = new Usuario
                        {
                            UserId = reader.GetInt32(0),
                            Username = reader.GetString(1),
                            PasswordHash = hash,
                            NombreCompleto = reader.GetString(3),
                            Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                            Activo = reader.GetBoolean(5),
                            PrimerLogin = reader.GetBoolean(6),
                            FechaCreacion = reader.GetDateTime(7),
                            UltimoLogin = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                        };
                        return usuario;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error en Login: " + ex.Message);
                return null;
            }
        }

        public async Task LogLoginAsync(int? userId, bool exitoso)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"INSERT INTO auditoria_login (user_id, fecha, exitoso)
                                    VALUES (@userId, GETDATE(), @exitoso)"
                };
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = (object?)userId ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@exitoso", SqlDbType.Bit) { Value = exitoso });
                await cmd.ExecuteNonQueryAsync();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al registrar auditoria: " + ex.Message);
            }
        }

        public async Task<bool> ChangePasswordAsync(int userId, string oldPassword, string newPassword)
        {
            try
            {
                Usuario? usuario = await GetUsuarioByIdAsync(userId);
                if (usuario == null)
                {
                    return false;
                }

                if (!BCrypt.Net.BCrypt.Verify(oldPassword, usuario.PasswordHash))
                {
                    return false;
                }

                string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"UPDATE usuarios SET password_hash = @hash, primer_login = 0 
                                    WHERE user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar, 255) { Value = newHash });
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cambiar contraseña: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> ResetPasswordAsync(int userId, string newPassword)
        {
            try
            {
                string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"UPDATE usuarios SET password_hash = @hash, primer_login = 1 
                                    WHERE user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar, 255) { Value = newHash });
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al resetear contraseña: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> CambiarContrasenaPrimerLoginAsync(int userId, string newPassword)
        {
            try
            {
                string newHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"UPDATE usuarios SET password_hash = @hash, primer_login = 0 
                                    WHERE user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar, 255) { Value = newHash });
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cambiar contraseña (primer login): " + ex.Message);
                return false;
            }
        }

        public async Task<List<Usuario>> GetUsuariosAsync()
        {
            List<Usuario> lista = new List<Usuario>();
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT user_id, username, nombre_completo, email, activo, 
                                           primer_login, fecha_creacion, ultimo_login
                                    FROM usuarios ORDER BY nombre_completo"
                };
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new Usuario
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        NombreCompleto = reader.GetString(2),
                        Email = reader.IsDBNull(3) ? null : reader.GetString(3),
                        Activo = reader.GetBoolean(4),
                        PrimerLogin = reader.GetBoolean(5),
                        FechaCreacion = reader.GetDateTime(6),
                        UltimoLogin = reader.IsDBNull(7) ? null : reader.GetDateTime(7)
                    });
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar usuarios: " + ex.Message);
            }
            return lista;
        }

        public async Task<Usuario?> GetUsuarioByIdAsync(int userId)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT user_id, username, password_hash, nombre_completo, email, 
                                           activo, primer_login, fecha_creacion, ultimo_login
                                    FROM usuarios WHERE user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });

                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                if (await reader.ReadAsync())
                {
                    return new Usuario
                    {
                        UserId = reader.GetInt32(0),
                        Username = reader.GetString(1),
                        PasswordHash = reader.GetString(2),
                        NombreCompleto = reader.GetString(3),
                        Email = reader.IsDBNull(4) ? null : reader.GetString(4),
                        Activo = reader.GetBoolean(5),
                        PrimerLogin = reader.GetBoolean(6),
                        FechaCreacion = reader.GetDateTime(7),
                        UltimoLogin = reader.IsDBNull(8) ? null : reader.GetDateTime(8)
                    };
                }
                return null;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al obtener usuario: " + ex.Message);
                return null;
            }
        }

        public async Task<int> CreateUsuarioAsync(Usuario usuario, string password, List<int> roleIds)
        {
            try
            {
                string hash = BCrypt.Net.BCrypt.HashPassword(password);
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();

                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"INSERT INTO usuarios (username, password_hash, nombre_completo, email, activo, primer_login)
                                    VALUES (@username, @hash, @nombre, @email, 1, 1);
                                    SELECT SCOPE_IDENTITY();"
                };
                cmd.Parameters.Add(new SqlParameter("@username", SqlDbType.NVarChar, 50) { Value = usuario.Username });
                cmd.Parameters.Add(new SqlParameter("@hash", SqlDbType.NVarChar, 255) { Value = hash });
                cmd.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar, 150) { Value = usuario.NombreCompleto });
                cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 100) { Value = (object?)usuario.Email ?? DBNull.Value });

                int newId = Convert.ToInt32(await cmd.ExecuteScalarAsync());

                foreach (int roleId in roleIds)
                {
                    using SqlCommand cmdRole = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO usuario_roles (user_id, role_id) VALUES (@userId, @roleId)"
                    };
                    cmdRole.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = newId });
                    cmdRole.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int) { Value = roleId });
                    await cmdRole.ExecuteNonQueryAsync();
                }

                return newId;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al crear usuario: " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> UpdateUsuarioAsync(Usuario usuario)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"UPDATE usuarios SET nombre_completo = @nombre, email = @email, 
                                           activo = @activo
                                    WHERE user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar, 150) { Value = usuario.NombreCompleto });
                cmd.Parameters.Add(new SqlParameter("@email", SqlDbType.NVarChar, 100) { Value = (object?)usuario.Email ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@activo", SqlDbType.Bit) { Value = usuario.Activo });
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = usuario.UserId });
                await cmd.ExecuteNonQueryAsync();

                // Actualizar roles
                using SqlCommand cmdDel = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "DELETE FROM usuario_roles WHERE user_id = @userId"
                };
                cmdDel.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = usuario.UserId });
                await cmdDel.ExecuteNonQueryAsync();

                foreach (Role role in usuario.Roles)
                {
                    using SqlCommand cmdRole = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO usuario_roles (user_id, role_id) VALUES (@userId, @roleId)"
                    };
                    cmdRole.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = usuario.UserId });
                    cmdRole.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int) { Value = role.RoleId });
                    await cmdRole.ExecuteNonQueryAsync();
                }

                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al actualizar usuario: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteUsuarioAsync(int userId)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "UPDATE usuarios SET activo = 0 WHERE user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al eliminar usuario: " + ex.Message);
                return false;
            }
        }

        public async Task<List<Role>> GetRolesAsync()
        {
            List<Role> lista = new List<Role>();
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT role_id, nombre, descripcion, activo FROM roles ORDER BY nombre"
                };
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new Role
                    {
                        RoleId = reader.GetInt32(0),
                        Nombre = reader.GetString(1),
                        Descripcion = reader.IsDBNull(2) ? null : reader.GetString(2),
                        Activo = reader.GetBoolean(3)
                    });
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar roles: " + ex.Message);
            }
            return lista;
        }

        public async Task<int> CreateRoleAsync(Role role)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"INSERT INTO roles (nombre, descripcion, activo)
                                    VALUES (@nombre, @descripcion, 1);
                                    SELECT SCOPE_IDENTITY();"
                };
                cmd.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar, 50) { Value = role.Nombre });
                cmd.Parameters.Add(new SqlParameter("@descripcion", SqlDbType.NVarChar, 200) { Value = (object?)role.Descripcion ?? DBNull.Value });
                return Convert.ToInt32(await cmd.ExecuteScalarAsync());
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al crear rol: " + ex.Message);
                return -1;
            }
        }

        public async Task<bool> UpdateRoleAsync(Role role)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"UPDATE roles SET nombre = @nombre, descripcion = @descripcion, 
                                           activo = @activo
                                    WHERE role_id = @roleId"
                };
                cmd.Parameters.Add(new SqlParameter("@nombre", SqlDbType.NVarChar, 50) { Value = role.Nombre });
                cmd.Parameters.Add(new SqlParameter("@descripcion", SqlDbType.NVarChar, 200) { Value = (object?)role.Descripcion ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@activo", SqlDbType.Bit) { Value = role.Activo });
                cmd.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int) { Value = role.RoleId });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al actualizar rol: " + ex.Message);
                return false;
            }
        }

        public async Task<bool> DeleteRoleAsync(int roleId)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "UPDATE roles SET activo = 0 WHERE role_id = @roleId"
                };
                cmd.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int) { Value = roleId });
                await cmd.ExecuteNonQueryAsync();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al eliminar rol: " + ex.Message);
                return false;
            }
        }

        public async Task<List<int>> GetRolePermisoIdsAsync(int roleId)
        {
            List<int> ids = new List<int>();
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT permiso_id FROM role_permisos WHERE role_id = @roleId"
                };
                cmd.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int) { Value = roleId });
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ids.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar permisos del rol: " + ex.Message);
            }
            return ids;
        }

        public async Task<List<int>> GetUsuarioRoleIdsAsync(int userId)
        {
            List<int> ids = new List<int>();
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT role_id FROM usuario_roles WHERE user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    ids.Add(reader.GetInt32(0));
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar roles del usuario: " + ex.Message);
            }
            return ids;
        }

        public async Task<bool> UpdateRolePermisosAsync(int roleId, List<int> permisoIds)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();

                using SqlCommand cmdDel = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "DELETE FROM role_permisos WHERE role_id = @roleId"
                };
                cmdDel.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int) { Value = roleId });
                await cmdDel.ExecuteNonQueryAsync();

                foreach (int permisoId in permisoIds)
                {
                    using SqlCommand cmdIns = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO role_permisos (role_id, permiso_id) VALUES (@roleId, @permisoId)"
                    };
                    cmdIns.Parameters.Add(new SqlParameter("@roleId", SqlDbType.Int) { Value = roleId });
                    cmdIns.Parameters.Add(new SqlParameter("@permisoId", SqlDbType.Int) { Value = permisoId });
                    await cmdIns.ExecuteNonQueryAsync();
                }
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al actualizar permisos del rol: " + ex.Message);
                return false;
            }
        }

        public async Task<List<Permiso>> GetPermisosAsync()
        {
            List<Permiso> lista = new List<Permiso>();
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT permiso_id, modulo, accion, descripcion FROM permisos ORDER BY modulo, accion"
                };
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    lista.Add(new Permiso
                    {
                        PermisoId = reader.GetInt32(0),
                        Modulo = reader.GetString(1),
                        Accion = reader.GetString(2),
                        Descripcion = reader.IsDBNull(3) ? null : reader.GetString(3)
                    });
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar permisos: " + ex.Message);
            }
            return lista;
        }

        public async Task<List<string>> GetUserPermisosAsync(int userId)
        {
            List<string> permisos = new List<string>();
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT DISTINCT p.modulo, p.accion
                                    FROM permisos p
                                    INNER JOIN role_permisos rp ON p.permiso_id = rp.permiso_id
                                    INNER JOIN usuario_roles ur ON rp.role_id = ur.role_id
                                    WHERE ur.user_id = @userId"
                };
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                using SqlDataReader reader = await cmd.ExecuteReaderAsync();
                while (await reader.ReadAsync())
                {
                    permisos.Add($"{reader.GetString(0)}:{reader.GetString(1)}");
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar permisos del usuario: " + ex.Message);
            }
            return permisos;
        }

        public async Task<bool> HasPermissionAsync(int userId, string modulo, string accion)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                await conn.OpenAsync();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = @"SELECT CASE WHEN EXISTS (
                        SELECT 1 FROM permisos p
                        INNER JOIN role_permisos rp ON p.permiso_id = rp.permiso_id
                        INNER JOIN usuario_roles ur ON rp.role_id = ur.role_id
                        WHERE ur.user_id = @userId AND p.modulo = @modulo AND p.accion = @accion
                    ) THEN 1 ELSE 0 END"
                };
                cmd.Parameters.Add(new SqlParameter("@userId", SqlDbType.Int) { Value = userId });
                cmd.Parameters.Add(new SqlParameter("@modulo", SqlDbType.NVarChar, 50) { Value = modulo });
                cmd.Parameters.Add(new SqlParameter("@accion", SqlDbType.NVarChar, 50) { Value = accion });
                object? result = await cmd.ExecuteScalarAsync();
                return Convert.ToInt32(result) == 1;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al verificar permiso: " + ex.Message);
                return false;
            }
        }
    }
}
