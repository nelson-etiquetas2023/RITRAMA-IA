using System.Data;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.SeguridadService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Gestion de Usuarios. Rediseño 30/70 sobre el esquema de FrmProductos: barra
    /// de acciones a nivel de pagina (Nuevo / Editar), panel izquierdo con buscador,
    /// grid de cuatro columnas y cuadro de resumen, panel derecho con la pagina de
    /// detalle. El detalle y la carga de datos se definen en los pasos siguientes.
    ///
    /// La version previa (listado en grid, captura con usuario/nombre/email/activo
    /// y check list de roles) se recupera con:
    ///   git checkout -- Forms/FrmUsuarios.cs Forms/FrmUsuarios.Designer.cs
    /// </summary>
    public partial class FrmUsuarios : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly ISeguridadService _seguridadService;

        /// <summary>Listado completo, sin filtrar. Es la fuente del buscador.</summary>
        private List<Usuario> _usuarios = [];

        /// <summary>
        /// Nombres de rol ya resueltos por UserId. Sin este cache, cada tecla del
        /// buscador volveria a preguntar los roles de cada usuario.
        /// </summary>
        private Dictionary<int, string> _rolesPorUsuario = [];

        public FrmUsuarios(ISeguridadService seguridadService)
        {
            ArgumentNullException.ThrowIfNull(seguridadService);
            _seguridadService = seguridadService;

            InitializeComponent();
            Text = "Usuarios";

            // Estilo VERDE de SunnyUI, igual que Clientes / Proveedores / Pedidos.
            // Debe ir antes de que la UI exista para que UIStyleManager tome el mando.
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();

            // Editar se habilita solo con una fila seleccionada del listado.
            gridUsuarios.SelectionChanged += (_, _) => ActualizarBotonesBarra();
            gridUsuarios.CurrentCellChanged += (_, _) => ActualizarBotonesBarra();

            // Filtrado en vivo sobre el listado ya cargado: no se vuelve a la base.
            txtBuscar.TextChanged += (_, _) => AplicarFiltroBusqueda();

            // La barra se cablea aqui y no en el disenador: los handlers son privados
            // del formulario y el Visual Studio no puede engancharlos. Mismo criterio
            // que FrmProductos.
            btnNuevoUsuario.Click += BtnNuevoUsuario_Click;
            btnEditarUsuario.Click += BtnEditarUsuario_Click;
        }

        /// <summary>
        /// Reaplica el tema verde para pisar el UIStyleManager global de Main.
        /// </summary>
        public void ReaplicarTema() => AplicarTemaVerde();

        /// <summary>
        /// Editar solo tiene sentido con una fila elegida en el listado; Nuevo siempre
        /// esta disponible. Mismo criterio que ActualizarBotonesBarra de FrmProductos.
        /// </summary>
        private void ActualizarBotonesBarra()
            => btnEditarUsuario.Enabled = gridUsuarios.CurrentRow is not null;

        /// <summary>Nuevo usuario: la pagina de detalle se define en el proximo paso.</summary>
        private void BtnNuevoUsuario_Click(object? sender, EventArgs e)
        {
        }

        /// <summary>Editar usuario: la pagina de detalle se define en el proximo paso.</summary>
        private void BtnEditarUsuario_Click(object? sender, EventArgs e)
        {
        }

        private void AplicarTemaVerde()
        {
            // Fondo verde pastel y barra de titulo en el verde saturado de la app.
            BackColor = Color.FromArgb(240, 250, 235);
            Style = UIStyle.Green;
            TitleColor = Color.FromArgb(110, 190, 40);
            TitleForeColor = Color.White;
            ForeColor = Color.FromArgb(48, 48, 48);
        }

        /// <summary>
        /// Carga el listado antes de mostrar el formulario (la invoca FormManager).
        /// Si la base falla se reporta el error y el formulario queda utilizable con
        /// el listado vacio, en vez de reventar al abrir.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _usuarios = await _seguridadService.GetUsuariosAsync();
                _rolesPorUsuario = await ResolverRolesPorUsuarioAsync(_usuarios);
            }
            catch (Exception ex)
            {
                _usuarios = [];
                _rolesPorUsuario = [];
                ServiceErrors.Report("Error al cargar Usuarios: " + ex.Message);
            }

            AplicarFiltroBusqueda();
            ActualizarBotonesBarra();
        }

        /// <summary>
        /// GetUsuariosAsync no hace JOIN de roles (Usuario.Roles viene vacio), asi que
        /// hay que preguntar los roles de cada usuario. El catalogo se trae una sola vez
        /// y los ids en paralelo para no encadenar N viajes de ida y vuelta.
        /// </summary>
        private async Task<Dictionary<int, string>> ResolverRolesPorUsuarioAsync(List<Usuario> usuarios)
        {
            Dictionary<int, string> nombresPorUsuario = [];

            if (usuarios.Count == 0)
            {
                return nombresPorUsuario;
            }

            List<Role> catalogo = await _seguridadService.GetRolesAsync();
            Dictionary<int, string> nombrePorId = catalogo.ToDictionary(r => r.RoleId, r => r.Nombre);

            // List<int>[] y no List<List<int>>: WhenAll devuelve el array que le pide el tipo.
            List<int>[] idsPorUsuario = await Task.WhenAll(
                usuarios.Select(u => _seguridadService.GetUsuarioRoleIdsAsync(u.UserId)));

            for (int i = 0; i < usuarios.Count; i++)
            {
                IEnumerable<string> nombres = idsPorUsuario[i]
                    .Where(nombrePorId.ContainsKey)
                    .Select(id => nombrePorId[id]);

                nombresPorUsuario[usuarios[i].UserId] = string.Join(", ", nombres);
            }

            return nombresPorUsuario;
        }

        /// <summary>
        /// Proyecta los usuarios a un DataTable con las cuatro columnas del grid.
        /// Se proyecta a proposito y no se enlaza la lista: <see cref="Usuario.Roles"/> es
        /// una lista de Role y <c>Activo</c> es un bool, y el grid no sabe pintar ni uno
        /// ni otro. El guion en el rol vacio mantiene el ancho de columna estable.
        /// </summary>
        private DataTable ConstruirDataTable(IEnumerable<Usuario> usuarios)
        {
            DataTable tabla = new();
            tabla.Columns.Add("UserId", typeof(int));
            tabla.Columns.Add("Username", typeof(string));
            tabla.Columns.Add("Rol", typeof(string));
            tabla.Columns.Add("Estado", typeof(string));

            foreach (Usuario usuario in usuarios)
            {
                _rolesPorUsuario.TryGetValue(usuario.UserId, out string? rol);

                tabla.Rows.Add(
                    usuario.UserId,
                    usuario.Username,
                    string.IsNullOrWhiteSpace(rol) ? "—" : rol,
                    usuario.Activo ? "Activo" : "Inactivo");
            }

            return tabla;
        }

        /// <summary>
        /// Filtra el listado por nombre de usuario o correo — lo que dice el watermark
        /// del buscador. Filtra en memoria contra la lista ya cargada: el RowFilter del
        /// DataTable exigiria escapar comillas y repetir el criterio en cada tecla.
        /// </summary>
        private void AplicarFiltroBusqueda()
        {
            string texto = txtBuscar.Text?.Trim() ?? string.Empty;

            IEnumerable<Usuario> visibles = texto.Length == 0
                ? _usuarios
                : _usuarios.Where(u =>
                    u.Username.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || (u.Email?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false));

            gridUsuarios.DataSource = ConstruirDataTable(visibles);

            ActualizarResumen();
            ActualizarBotonesBarra();
        }

        /// <summary>
        /// Cuadro de resumen: arriba el total de usuarios registrados, abajo lo que hay
        /// en pantalla con el corte de activos. Los dos numeros cuenta cosas distintas
        /// —el primero no cambia con el filtro— asi que van en lineas separadas.
        /// </summary>
        private void ActualizarResumen()
        {
            DataTable? tabla = gridUsuarios.DataSource as DataTable;
            int visibles = tabla?.Rows.Count ?? 0;
            int activos = 0;

            if (tabla is not null)
            {
                foreach (DataRow fila in tabla.Rows)
                {
                    if (Equals(fila["Estado"], "Activo"))
                    {
                        activos++;
                    }
                }
            }

            lblResumen.Text = $"Usuarios registrados: {_usuarios.Count}";
            lblResumenDetalle.Text = $"Mostrando {visibles} - Activos {activos} - Inactivos {visibles - activos}";
        }
    }
}
