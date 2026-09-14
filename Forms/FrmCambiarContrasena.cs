using Ritrama2025.Services.SeguridadService;
using Sunny.UI;
using System.Windows.Forms;

namespace Ritrama2025.Forms
{
    public partial class FrmCambiarContrasena : UIForm
    {
        private readonly ISeguridadService _seguridadService;

        public FrmCambiarContrasena(ISeguridadService seguridadService)
        {
            _seguridadService = seguridadService;
            InitializeComponent();
        }

        private async void btn_cambiar_Click(object sender, EventArgs e)
        {
            string nuevaContrasena = txt_nueva.Text;
            string confirmar = txt_confirmar.Text;

            if (string.IsNullOrEmpty(nuevaContrasena))
            {
                MessageBox.Show("Ingrese la nueva contraseña.", "Campo requerido",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nuevaContrasena.Length < 6)
            {
                MessageBox.Show("La contraseña debe tener al menos 6 caracteres.", "Contraseña débil",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (nuevaContrasena != confirmar)
            {
                MessageBox.Show("Las contraseñas no coinciden.", "Error de confirmación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int userId = Helpers.SesionActual.Usuario?.UserId ?? 0;
            bool success = await _seguridadService.CambiarContrasenaPrimerLoginAsync(userId, nuevaContrasena);
            if (success)
            {
                MessageBox.Show(
                    "Contraseña cambiada correctamente. Ahora puede iniciar sesión.",
                    "Éxito",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                MessageBox.Show("Error al cambiar la contraseña. Intente de nuevo.", "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        private void btn_cancelar_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
