using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.SeguridadService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    public partial class FrmRoles : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly ISeguridadService _seguridadService;
        private List<Role> _roles = [];
        private List<Permiso> _permisos = [];
        private int _roleEnEdicion = -1;

        public FrmRoles(ISeguridadService seguridadService)
        {
            _seguridadService = seguridadService;
            InitializeComponent();
            this.Text = "Gestión de Roles";

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
            this.BackColor = Color.White;
            this.Style = UIStyle.Green;
            this.TitleColor = verde;
            this.TitleForeColor = Color.White;
        }

        public async Task InitializeAsync()
        {
            await CargarRolesAsync();
            _permisos = await _seguridadService.GetPermisosAsync();
            FinalizarConfiguracionUI();
            DeshabilitarCaptura();
        }

        private void FinalizarConfiguracionUI()
        {
            gridRoles.Columns.Clear();
            gridRoles.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("Nombre", 200, "Rol", "Nombre", gridRoles);
            CommonService.ADD_COLUMN_GRID("Descripcion", 350, "Descripción", "Descripcion", gridRoles);
            CommonService.ADD_COLUMN_GRID("Activo", 80, "Activo", "Activo", gridRoles);

            CargarPermisosEnChecklist();
        }

        private void CargarPermisosEnChecklist()
        {
            clbPermisos.Items.Clear();
            foreach (var permiso in _permisos)
            {
                clbPermisos.Items.Add($"{permiso.Modulo} - {permiso.Accion}", false);
            }
        }

        private async Task CargarRolesAsync()
        {
            _roles = await _seguridadService.GetRolesAsync();
            gridRoles.DataSource = null;
            gridRoles.DataSource = _roles;
        }

        private void DeshabilitarCaptura()
        {
            _roleEnEdicion = -1;
            LimpiarCampos();
            panelCaptura.Enabled = false;
            tsbGuardar.Enabled = false;
            tsbEliminar.Enabled = false;
        }

        private void HabilitarCaptura()
        {
            panelCaptura.Enabled = true;
            tsbGuardar.Enabled = true;
            tsbEliminar.Enabled = _roleEnEdicion > 0;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtDescripcion.Clear();
            chkActivo.Checked = true;
            for (int i = 0; i < clbPermisos.Items.Count; i++)
                clbPermisos.SetItemChecked(i, false);
        }

        private void TsbNuevo_Click(object? sender, EventArgs e)
        {
            _roleEnEdicion = -1;
            LimpiarCampos();
            gridRoles.ClearSelection();
            HabilitarCaptura();
            txtNombre.Focus();
        }

        private async void GridRoles_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridRoles.CurrentRow == null || gridRoles.CurrentRow.Index < 0)
            {
                tsbEliminar.Enabled = false;
                return;
            }

            if (_roleEnEdicion == -1 && panelCaptura.Enabled) return;

            var role = gridRoles.CurrentRow.DataBoundItem as Role;
            if (role == null) return;

            _roleEnEdicion = role.RoleId;
            txtNombre.Text = role.Nombre;
            txtDescripcion.Text = role.Descripcion ?? string.Empty;
            chkActivo.Checked = role.Activo;

            var permisosIds = await _seguridadService.GetRolePermisoIdsAsync(role.RoleId);
            for (int i = 0; i < clbPermisos.Items.Count; i++)
            {
                var permiso = _permisos[i];
                clbPermisos.SetItemChecked(i, permisosIds.Contains(permiso.PermisoId));
            }

            panelCaptura.Enabled = true;
            tsbGuardar.Enabled = true;
            tsbEliminar.Enabled = true;
        }

        private async void TsbGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar el nombre del rol.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var permisosSeleccionados = new List<int>();
            for (int i = 0; i < clbPermisos.Items.Count; i++)
            {
                if (clbPermisos.GetItemChecked(i))
                    permisosSeleccionados.Add(_permisos[i].PermisoId);
            }

            tsbGuardar.Enabled = false;
            using var loading = new FrmLoading("Guardando rol...");
            loading.Show(this);
            loading.BringToFront();

            try
            {
                if (_roleEnEdicion == -1)
                {
                    var nuevoRole = new Role
                    {
                        Nombre = txtNombre.Text.Trim(),
                        Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                        Activo = chkActivo.Checked
                    };

                    int newId = await _seguridadService.CreateRoleAsync(nuevoRole);
                    if (newId > 0)
                    {
                        await _seguridadService.UpdateRolePermisosAsync(newId, permisosSeleccionados);
                        MessageBox.Show("Rol creado.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await CargarRolesAsync();
                        DeshabilitarCaptura();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo crear el rol.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
                else
                {
                    var role = new Role
                    {
                        RoleId = _roleEnEdicion,
                        Nombre = txtNombre.Text.Trim(),
                        Descripcion = string.IsNullOrWhiteSpace(txtDescripcion.Text) ? null : txtDescripcion.Text.Trim(),
                        Activo = chkActivo.Checked
                    };

                    bool guardado = await _seguridadService.UpdateRoleAsync(role);
                    if (guardado)
                    {
                        await _seguridadService.UpdateRolePermisosAsync(_roleEnEdicion, permisosSeleccionados);
                        MessageBox.Show("Rol actualizado.", "Éxito",
                            MessageBoxButtons.OK, MessageBoxIcon.Information);
                        await CargarRolesAsync();
                        DeshabilitarCaptura();
                    }
                    else
                    {
                        MessageBox.Show("No se pudo actualizar el rol.", "Error",
                            MessageBoxButtons.OK, MessageBoxIcon.Error);
                    }
                }
            }
            finally
            {
                if (!loading.IsDisposed) loading.Close();
            }
        }

        private async void TsbEliminar_Click(object? sender, EventArgs e)
        {
            if (_roleEnEdicion < 0) return;

            var confirm = MessageBox.Show($"¿Eliminar el rol {txtNombre.Text}?", "Confirmar",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            tsbEliminar.Enabled = false;
            using var loading = new FrmLoading("Eliminando rol...");
            loading.Show(this);
            loading.BringToFront();

            try
            {
                bool eliminado = await _seguridadService.DeleteRoleAsync(_roleEnEdicion);
                if (eliminado)
                {
                    MessageBox.Show("Rol eliminado.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarRolesAsync();
                    DeshabilitarCaptura();
                }
                else
                {
                    MessageBox.Show("No se pudo eliminar el rol.", "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
            finally
            {
                if (!loading.IsDisposed) loading.Close();
            }
        }
    }
}
