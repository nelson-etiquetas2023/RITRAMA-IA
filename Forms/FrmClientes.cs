using System.Data;
using Ritrama2025.Helpers;
using Ritrama2025.Services.ClienteService;
using Ritrama2025.Services.ProduccionService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Clientes (rediseño 30/70): panel izquierdo con buscador y grid
    /// de clientes, panel derecho con la página de detalle. El listado viene del
    /// servicio (activos y desactivados); la captura volverá en un paso posterior.
    /// </summary>
    public partial class FrmClientes : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IClienteService _clientesService;
        private DataTable _dtClientes = new();

        /// <summary>
        /// Crea el formulario de clientes con su servicio de listado.
        /// </summary>
        /// <param name="clientesService">Servicio que trae el listado de la base.</param>
        public FrmClientes(IClienteService clientesService)
        {
            InitializeComponent();
            ArgumentNullException.ThrowIfNull(clientesService);
            _clientesService = clientesService;

            Text = "Clientes";

            // Este módulo usa el estilo VERDE de SunnyUI (igual que Pedidos/Producción/Despacho).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();

            // Filtrado en vivo por nombre sobre el listado ya cargado: el RowFilter del
            // DataTable se refleja solo en el grid enlazado, sin volver a la base.
            txtBuscar.TextChanged += (_, _) => AplicarFiltroBusqueda();
        }

        /// <summary>
        /// Reaplica el tema verde para pisar el UIStyleManager global del Main.
        /// </summary>
        public void ReaplicarTema() => AplicarTemaVerde();

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = verde;
            TitleForeColor = Color.White;
        }

        /// <summary>
        /// Carga el listado de clientes antes de mostrar la pestaña. Si la base falla,
        /// se reporta el error y el formulario queda visible con el grid vacío.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _dtClientes = await _clientesService.LoadListadoAsync();
                gridClientes.DataSource = _dtClientes;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Clientes: " + ex.Message);
            }
        }

        /// <summary>
        /// Filtra el listado por nombre (comodín SQL, escapando las comillas).
        /// </summary>
        private void AplicarFiltroBusqueda()
        {
            string filtro = txtBuscar.Text?.Trim() ?? string.Empty;
            _dtClientes.DefaultView.RowFilter = filtro.Length == 0
                ? string.Empty
                : $"customer_name LIKE '%{filtro.Replace("'", "''")}%'";
        }
    }
}
