using Ritrama2025.Forms.Otros;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.SeguridadService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    public partial class FrmUsuarios : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly ISeguridadService _seguridadService;
        private List<Usuario> _usuarios = [];
        private List<Role> _roles = [];
        private int _usuarioEnEdicion = -1;

        public FrmUsuarios(ISeguridadService seguridadService)
        {
            _seguridadService = seguridadService;
            InitializeComponent();
            Text = "Gestión de Usuarios";

            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };
            AplicarTemaVerde();
        }

        public void ReaplicarTema() => AplicarTemaVerde();

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = verde;
            TitleForeColor = Color.White;
        }

        public async Task InitializeAsync()
        {
            await CargarUsuariosAsync();
            await CargarRolesAsync();
            FinalizarConfiguracionUI();
            DeshabilitarCaptura();
        }

        private void FinalizarConfiguracionUI()
        {
            gridUsuarios.Columns.Clear();
            gridUsuarios.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("Username", 120, "Usuario", "Username", gridUsuarios);
            CommonService.ADD_COLUMN_GRID("NombreCompleto", 250, "Nombre Completo", "NombreCompleto", gridUsuarios);
            CommonService.ADD_COLUMN_GRID("Email", 200, "Email", "Email", gridUsuarios);
            CommonService.ADD_COLUMN_GRID("Activo", 80, "Activo", "Activo", gridUsuarios);
            CommonService.ADD_COLUMN_GRID("PrimerLogin", 100, "Primer Login", "PrimerLogin", gridUsuarios);
            CommonService.ADD_COLUMN_GRID("UltimoLogin", 150, "Último Login", "UltimoLogin", gridUsuarios);
        }

        private async Task CargarUsuariosAsync()
        {
            _usuarios = await _seguridadService.GetUsuariosAsync();
            gridUsuarios.DataSource = null;
            gridUsuarios.DataSource = _usuarios;
        }

        private async Task CargarRolesAsync()
        {
            _roles = await _seguridadService.GetRolesAsync();
            clbRoles.Items.Clear();
            foreach (Role role in _roles)
            {
                clbRoles.Items.Add(role.Nombre, false);
            }
        }

        private void DeshabilitarCaptura()
        {
            _usuarioEnEdicion = -1;
            LimpiarCampos();
            panelCaptura.Enabled = false;
            tsbGuardar.Enabled = false;
            tsbEliminar.Enabled = false;
            tsbResetPassword.Enabled = false;
        }

        private void HabilitarCaptura()
        {
            panelCaptura.Enabled = true;
            tsbGuardar.Enabled = true;
            tsbEliminar.Enabled = _usuarioEnEdicion > 0;
            tsbResetPassword.Enabled = _usuarioEnEdicion > 0;
        }

        private void LimpiarCampos()
        {
            txtUsername.Clear();
            txtNombre.Clear();
            txtEmail.Clear();
            chkActivo.Checked = true;
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                clbRoles.SetItemChecked(i, false);
            }
        }

        private void TsbNuevo_Click(object? sender, EventArgs e)
        {
            _usuarioEnEdicion = -1;
            LimpiarCampos();
            gridUsuarios.ClearSelection();
            HabilitarCaptura();
            txtUsername.Enabled = true;
            txtUsername.Focus();
        }

        private async void GridUsuarios_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridUsuarios.CurrentRow == null || gridUsuarios.CurrentRow.Index < 0)
            {
                tsbEliminar.Enabled = false;
                tsbResetPassword.Enabled = false;
                return;
            }

            if (_usuarioEnEdicion == -1 && panelCaptura.Enabled)
            {
                return;
            }

            Usuario? usuario = gridUsuarios.CurrentRow.DataBoundItem as Usuario;
            if (usuario == null)
            {
                return;
            }

            _usuarioEnEdicion = usuario.UserId;
            txtUsername.Text = usuario.Username;
            txtUsername.Enabled = false;
            txtNombre.Text = usuario.NombreCompleto;
            txtEmail.Text = usuario.Email ?? string.Empty;
            chkActivo.Checked = usuario.Activo;

            List<int> rolesUsuario = await _seguridadService.GetUsuarioRoleIdsAsync(usuario.UserId);
            List<Role> roles = await _seguridadService.GetRolesAsync();

            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                Role role = roles[i];
                clbRoles.SetItemChecked(i, rolesUsuario.Contains(role.RoleId));
            }

            panelCaptura.Enabled = true;
            tsbGuardar.Enabled = true;
            tsbEliminar.Enabled = true;
            tsbResetPassword.Enabled = true;
        }

        private async void TsbGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtUsername.Text))
            {
                MessageBox.Show("Debe ingresar el nombre de usuario.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar el nombre completo.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<int> rolesSeleccionados = new List<int>();
            List<Role> roles = await _seguridadService.GetRolesAsync();
            for (int i = 0; i < clbRoles.Items.Count; i++)
            {
                if (clbRoles.GetItemChecked(i))
                {
                    rolesSeleccionados.Add(roles[i].RoleId);
                }
            }

            tsbGuardar.Enabled = false;
            using FrmLoading loading = new FrmLoading("Guardando usuario...");
            loading.Show(this);
            loading.BringToFront();

            try
            {
                if (_usuarioEnEdicion == -1)
                {
                    // Nuevo usuario - contraseña temporal
                    string passwordTemporal = "cambiar123";
                    Usuario nuevoUsuario = new Usuario
                    {
                        Username = txtUsername.Text.Trim(),
                        NombreCompleto = txtNombre.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        Activo = chkActivo.Checked
                    };

                    int newId = await _seguridadService.CreateUsuarioAsync(nuevoUsuario, passwordTemporal, rolesSeleccionados);
                    if (newId > 0)
                    {
                        MessageBox.Show($"Usuario creado. Contraseña temporal: {passwordTemporal}", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await CargarUsuariosAsync();
                        DeshabilitarCaptura();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo crear el usuario.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    Usuario usuario = new Usuario
                    {
                        UserId = _usuarioEnEdicion,
                        Username = txtUsername.Text.Trim(),
                        NombreCompleto = txtNombre.Text.Trim(),
                        Email = string.IsNullOrWhiteSpace(txtEmail.Text) ? null : txtEmail.Text.Trim(),
                        Activo = chkActivo.Checked,
                        Roles = rolesSeleccionados.Select(r => new Role { RoleId = r }).ToList()
                    };

                    bool guardado = await _seguridadService.UpdateUsuarioAsync(usuario);
                    if (guardado)
                    {
                        MessageBox.Show("Usuario actualizado.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await CargarUsuariosAsync();
                        DeshabilitarCaptura();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el usuario.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                if (!loading.IsDisposed)
                {
                    loading.Close();
                }
            }
        }

        private async void TsbEliminar_Click(object? sender, EventArgs e)
        {
            if (_usuarioEnEdicion < 0)
            {
                MessageBox.Show("Seleccione un usuario para eliminar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirm = MessageBox.Show($"¿Eliminar el usuario {txtUsername.Text}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            tsbEliminar.Enabled = false;
            using FrmLoading loading = new FrmLoading("Eliminando usuario...");
            loading.Show(this);
            loading.BringToFront();

            try
            {
                bool eliminado = await _seguridadService.DeleteUsuarioAsync(_usuarioEnEdicion);
                if (eliminado)
                {
                    MessageBox.Show("Usuario eliminado.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarUsuariosAsync();
                    DeshabilitarCaptura();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el usuario.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (!loading.IsDisposed)
                {
                    loading.Close();
                }
            }
        }

        private async void TsbResetPassword_Click(object? sender, EventArgs e)
        {
            if (_usuarioEnEdicion < 0)
            {
                return;
            }

            DialogResult confirm = MessageBox.Show(
                $"¿Resetear la contraseña del usuario {txtUsername.Text}?\nSe asignará la contraseña: cambiar123",
                "Confirmar reseteo", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            bool reseteado = await _seguridadService.ResetPasswordAsync(_usuarioEnEdicion, "cambiar123");
            if (reseteado)
            {
                MessageBox.Show("Contraseña reseteada a: cambiar123", "Éxito",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
            else
            {
                MessageBox.Show("No se pudo resetear la contraseña.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void TsbBuscar_Click(object? sender, EventArgs e)
        {
            AplicarFiltroBusqueda();
        }

        private void TxtBuscar_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AplicarFiltroBusqueda();
            }
        }

        private void AplicarFiltroBusqueda()
        {
            if (_usuarios == null)
            {
                return;
            }

            string filtro = txtBuscar.Text?.Trim() ?? string.Empty;
            List<Usuario> filtrados = filtro.Length == 0
                ? _usuarios
                : _usuarios.Where(u => u.NombreCompleto.Contains(filtro, StringComparison.OrdinalIgnoreCase)
                                    || u.Username.Contains(filtro, StringComparison.OrdinalIgnoreCase)).ToList();
            gridUsuarios.DataSource = null;
            gridUsuarios.DataSource = filtrados;
        }
    }
}
