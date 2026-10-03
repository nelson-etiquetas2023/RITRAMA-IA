using System.Windows.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.SeguridadService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    public partial class FrmLogin : UIForm, IFormTemaClaro
    {
        private readonly ISeguridadService _seguridadService;
        private int _intentosRestantes = 5;
        private const int MAX_INTENTOS = 5;
        private bool _mostrarClave;

        public Usuario? UsuarioAutenticado { get; private set; }
        public List<string> Permisos { get; private set; } = [];

        public FrmLogin(ISeguridadService seguridadService)
        {
            _seguridadService = seguridadService;
            InitializeComponent();

            // Estilo VERDE de SunnyUI (igual que Produccion/Despacho/Pedidos).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };
            AplicarTemaVerde();

            ActualizarContador();
        }

        public void ReaplicarTema() => AplicarTemaVerde();

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            Color verdeOsc = Color.FromArgb(70, 140, 25);
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = verde;
            TitleForeColor = Color.White;

            lbl_empresa.ForeColor = verdeOsc;
            lbl_title.ForeColor = verdeOsc;

            txt_username.Style = UIStyle.Green;
            txt_username.StyleCustomMode = false;
            txt_password.Style = UIStyle.Green;
            txt_password.StyleCustomMode = false;

            btn_login.Style = UIStyle.Green;
            btn_login.StyleCustomMode = false;
            btn_salir.Style = UIStyle.Gray;
            btn_salir.StyleCustomMode = false;
            AplicarEstiloOjo();
        }

        private void AplicarEstiloOjo()
        {
            // Verde cuando la clave esta visible, gris cuando esta oculta.
            btn_ojo.Style = _mostrarClave ? UIStyle.Green : UIStyle.Gray;
            btn_ojo.StyleCustomMode = false;
        }

        private void ActualizarContador()
        {
            lbl_intentos.Text = $"Intentos restantes: {_intentosRestantes}";

            lbl_intentos.ForeColor = _intentosRestantes switch
            {
                >= 4 => Color.FromArgb(110, 190, 40),
                3 => Color.FromArgb(255, 193, 7),
                2 => Color.FromArgb(255, 152, 0),
                1 => Color.FromArgb(220, 53, 69),
                _ => Color.FromArgb(220, 53, 69)
            };

            if (_intentosRestantes == 1)
            {
                lbl_intentos.Text = "Intento restante: 1";
            }
        }

        private async void btn_login_Click(object sender, EventArgs e)
        {
            await RealizarLogin();
        }

        /// <summary>
        /// Autentica contra el servicio y cierra el formulario. Es interna (y no
        /// privada) para que las pruebas puedan llamarla y esperar el Task: dispararla
        /// desde el evento Click produce un async void que no se puede aguardar.
        /// </summary>
        internal async Task RealizarLogin()
        {
            string username = txt_username.Text.Trim();
            string password = txt_password.Text;

            if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
            {
                MostrarAviso("Ingrese usuario y contraseña.", MessageBoxIcon.Warning);
                return;
            }

            btn_login.Enabled = false;
            btn_login.Text = "Verificando...";

            Usuario? usuario = await _seguridadService.LoginAsync(username, password);

            // Defensa en profundidad: el servicio ya excluye a los desactivados en el
            // WHERE, pero aqui se vuelve a comprobar para no abrir sesion con un
            // usuario inactivo aunque el servicio o una integracion lo devuelvan.
            if (usuario is { Activo: true })
            {
                UsuarioAutenticado = usuario;
                Permisos = await _seguridadService.GetUserPermisosAsync(usuario.UserId);
                await _seguridadService.LogLoginAsync(usuario.UserId, true);
                DialogResult = DialogResult.OK;
                Close();
            }
            else
            {
                _intentosRestantes--;
                ActualizarContador();
                // NULL (no 0): user_id tiene FK a usuarios y 0 la violaba.
                await _seguridadService.LogLoginAsync(null, false);

                if (_intentosRestantes == 0)
                {
                    MostrarAviso(
                        "Agotó todos los intentos. La aplicación se cerrará.",
                        MessageBoxIcon.Error);
                    DialogResult = DialogResult.Cancel;
                    Close();
                }
                else
                {
                    // Un unico texto para usuario inexistente, desactivado o clave
                    // mala: no se revela cual de los tres fallo.
                    MostrarAviso(
                        $"Usuario o contraseña erróneos. Intentos restantes: {_intentosRestantes}.",
                        MessageBoxIcon.Warning);
                    txt_password.Clear();
                    txt_password.Focus();
                }

                btn_login.Enabled = true;
                btn_login.Text = "Iniciar Sesión";
            }
        }

        /// <summary>
        /// Aviso del login. Vive en un metodo aparte y no en un MessageBox en linea
        /// para que las pruebas puedan sustituir el dialogo modal por una llamada
        /// grabada: un MessageBox de verdad dentro de un test lo dejaria colgado.
        /// </summary>
        /// <param name="mensaje">Texto a mostrar al usuario.</param>
        /// <param name="icono">Icono del dialogo; aviso por defecto.</param>
        protected virtual void MostrarAviso(string mensaje, MessageBoxIcon icono = MessageBoxIcon.Warning)
            => MessageBox.Show(mensaje, "Login", MessageBoxButtons.OK, icono);

        private void btn_salir_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }

        private void btn_ojo_Click(object sender, EventArgs e)
        {
            _mostrarClave = !_mostrarClave;
            txt_password.PasswordChar = _mostrarClave ? '\0' : '*';
            AplicarEstiloOjo();
        }

        private void txt_password_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                _ = RealizarLogin();
            }
        }

        private void txt_username_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == (char)Keys.Enter)
            {
                e.Handled = true;
                txt_password.Focus();
            }
        }
    }
}
