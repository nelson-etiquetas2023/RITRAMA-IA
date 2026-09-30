using System.Data;
using Ritrama2025.Helpers;
using Ritrama2025.Services.ClienteService;
using Ritrama2025.Services.ProduccionService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Clientes (rediseño 30/70): panel izquierdo con buscador, cuadro
    /// de resumen (total/filtrado) y grid de clientes; panel derecho con la página de
    /// detalle, que se refresca al seleccionar una fila del grid.
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

            // RefrescarDetalle con los dos eventos: el SelectionChanged se difiere al
            // siguiente ciclo de mensajes (y dispara con la fila anterior) mientras que
            // el CurrentCellChanged dispara síncrono — mismo criterio que FrmProductos.
            gridClientes.SelectionChanged += (_, _) => RefrescarDetalle();
            gridClientes.CurrentCellChanged += (_, _) => RefrescarDetalle();
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
            finally
            {
                ActualizaResumen();
            }
        }

        /// <summary>
        /// Filtra el listado por nombre (comodín SQL, escapando las comillas)
        /// y actualiza el cuadro de resumen.
        /// </summary>
        private void AplicarFiltroBusqueda()
        {
            string filtro = txtBuscar.Text?.Trim() ?? string.Empty;
            _dtClientes.DefaultView.RowFilter = filtro.Length == 0
                ? string.Empty
                : $"customer_name LIKE '%{filtro.Replace("'", "''")}%'";
            ActualizaResumen();
        }

        /// <summary>
        /// Cuadro de resumen bajo el buscador: total de clientes sin filtro, o
        /// cuántos se muestran del total cuando el filtro está activo.
        /// </summary>
        private void ActualizaResumen()
        {
            int total = _dtClientes.Rows.Count;
            int visibles = _dtClientes.DefaultView.Count;
            bool filtrando = (txtBuscar.Text?.Trim().Length ?? 0) > 0;

            lblResumen.Text = !filtrando
                ? $"Total: {total} {(total == 1 ? "cliente" : "clientes")}"
                : $"Mostrando {visibles} de {total} {(total == 1 ? "cliente" : "clientes")}";
        }

        /// <summary>
        /// Refresca los campos de la página de detalle con la fila seleccionada
        /// del grid; sin selección deja los valores en "—".
        /// </summary>
        private void RefrescarDetalle()
        {
            if (gridClientes.CurrentRow?.DataBoundItem is not DataRowView fila)
            {
                LimpiarDetalle();
                return;
            }

            lblValorId.Text = Texto(fila, "customer_id");
            lblValorNombre.Text = Texto(fila, "customer_name");
            lblValorIdentificacion.Text = Texto(fila, "identificacion");
            lblValorEmpresa.Text = Texto(fila, "empresa");
            lblValorCategoria.Text = Texto(fila, "customer_category");
            lblValorTelefono.Text = Texto(fila, "phone");
            lblValorContacto.Text = Texto(fila, "contacto");
            lblValorEmail.Text = Texto(fila, "customer_email");
            lblValorCondicion.Text = Texto(fila, "condicion_pago");
            lblValorImpuesto.Text = Texto(fila, "impuesto");
            lblValorDireccion.Text = Texto(fila, "customer_dir");
            lblValorUnity1.Text = SiNo(fila, "unity1");
            lblValorUnity2.Text = SiNo(fila, "unity2");
            lblValorEstado.Text = Texto(fila, "status");
            lblValorEstado.ForeColor = lblValorEstado.Text == "activo"
                ? Color.FromArgb(60, 110, 20)
                : Color.FromArgb(180, 60, 60);
        }

        /// <summary>
        /// Deja el detalle vacío ("—") cuando no hay fila seleccionada.
        /// </summary>
        private void LimpiarDetalle()
        {
            lblValorId.Text = "—";
            lblValorNombre.Text = "—";
            lblValorIdentificacion.Text = "—";
            lblValorEmpresa.Text = "—";
            lblValorCategoria.Text = "—";
            lblValorTelefono.Text = "—";
            lblValorContacto.Text = "—";
            lblValorEmail.Text = "—";
            lblValorCondicion.Text = "—";
            lblValorImpuesto.Text = "—";
            lblValorDireccion.Text = "—";
            lblValorUnity1.Text = "—";
            lblValorUnity2.Text = "—";
            lblValorEstado.Text = "—";
            lblValorEstado.ForeColor = Color.FromArgb(48, 48, 48);
        }

        /// <summary>Lee un campo del detalle; "—" si la columna no existe o está vacía.</summary>
        private static string Texto(DataRowView fila, string columna)
        {
            if (!fila.Row.Table.Columns.Contains(columna))
            {
                return "—";
            }

            object valor = fila[columna];
            if (valor is null or DBNull)
            {
                return "—";
            }

            string texto = valor.ToString() ?? string.Empty;
            return texto.Length == 0 ? "—" : texto;
        }

        /// <summary>Traduce un bit a Sí/No ("—" si no hay valor).</summary>
        private static string SiNo(DataRowView fila, string columna)
        {
            if (!fila.Row.Table.Columns.Contains(columna) || fila[columna] is not bool valor)
            {
                return "—";
            }

            return valor ? "Sí" : "No";
        }
    }
}
