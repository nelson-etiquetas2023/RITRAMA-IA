using System.ComponentModel;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProductsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Productos (rediseño 30/70).
    /// Panel izquierdo (30%): buscador, grid con product_id / product_name / tipo y contador de productos.
    /// Panel derecho (70%): pestaas de detalle del producto seleccionado.
    /// Los datos vienen de <see cref="IProductsService.LoadTypedAsync"/> (Product + ProductCategoryRules),
    /// sin DataSet crudo ni SQL en la capa de presentacion.
    /// </summary>
    public partial class FrmProductos : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        /// <summary>Verde corporativo del modulo (mismo que ProductCard y el tema SunnyUI Green).</summary>
        private static readonly Color Verde = Color.FromArgb(110, 190, 40);

        /// <summary>Verde oscuro usado en los titulos de seccion.</summary>
        private static readonly Color VerdeTitulo = Color.FromArgb(60, 110, 20);

        /// <summary>Color de las etiquetas del detalle.</summary>
        private static readonly Color GrisTexto = Color.FromArgb(64, 64, 64);

        /// <summary>Color de las filas de productos anulados.</summary>
        private static readonly Color GrisAnulado = Color.FromArgb(150, 150, 150);

        /// <summary>Color del texto normal del grid.</summary>
        private static readonly Color GrisFuerte = Color.FromArgb(48, 48, 48);

        /// <summary>Borde suave de las cajas de solo lectura del detalle.</summary>
        private static readonly Color ColorBordeCampo = Color.FromArgb(180, 210, 180);

        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;

        /// <summary>Catalogo completo en memoria (fuente de verdad del buscador y del contador).</summary>
        private List<Product> _productos = new();

        /// <summary>Codigo del producto seleccionado, para conservarlo tras filtrar.</summary>
        private string? _idSeleccionado;

        /// <summary>
        /// Crea el formulario de productos.
        /// </summary>
        /// <param name="productsService">Servicio del catalogo de productos.</param>
        /// <param name="configuration">Configuracion de la aplicacion (reservada para usos futuros del modulo).</param>
        public FrmProductos(IProductsService productsService, IConfiguration configuration)
        {
            ArgumentNullException.ThrowIfNull(productsService);
            ArgumentNullException.ThrowIfNull(configuration);

            InitializeComponent();

            _productsService = productsService;
            _configuration = configuration;

            components ??= new Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            EstilarCamposDetalle();
            ConfigurarEventos();
            ActualizarContador();
            LimpiarDetalle();
        }

        /// <summary>
        /// Carga el catalogo. FormManager la llama antes de mostrar el formulario (FrmLoading incluido).
        /// </summary>
        public async Task InitializeAsync()
        {
            await CargarCatalogoAsync();
        }

        /// <summary>
        /// Lee el catalogo desde el servicio y refresca grid + contador.
        /// Un fallo de datos no lanza: se notifica via ServiceErrors y se deja el grid vacio.
        /// </summary>
        private async Task CargarCatalogoAsync()
        {
            Result<IReadOnlyList<Product>> resultado = await _productsService.LoadTypedAsync();

            if (resultado.IsSuccess && resultado.Value is not null)
            {
                _productos = resultado.Value.ToList();
            }
            else
            {
                _productos = new List<Product>();
                ServiceErrors.Report(resultado.Error ?? "No se pudo cargar el catalogo de productos.");
            }

            _idSeleccionado = null;
            AplicarFiltro();
            LimpiarDetalle();
        }

        /// <summary>
        /// Enlaza los eventos de la UI y evita que el grid genere columnas automaticas.
        /// </summary>
        private void ConfigurarEventos()
        {
            gridProductos.AutoGenerateColumns = false;
            gridProductos.SelectionChanged += GridProductos_SelectionChanged;
            gridProductos.CellFormatting += GridProductos_CellFormatting;
            txtSearch.TextChanged += TxtSearch_TextChanged;
        }

        /// <summary>
        /// Aplica fuente, color y solo lectura a los campos de la pestaña de detalle.
        /// Se hace aqui (y no en el diseniador) para no repetir el mismo bloque nueve veces.
        /// </summary>
        private void EstilarCamposDetalle()
        {
            UILabel[] etiquetas =
            [
                lblDetId, lblDetNombre, lblDetTipo, lblDetReferencia,
                lblDetCodebar, lblDetPrecio, lblDetRatio, lblDetEstado, lblDetDescripcion
            ];

            foreach (UILabel etiqueta in etiquetas)
            {
                etiqueta.Font = new Font("JetBrains Mono", 9F);
                etiqueta.ForeColor = GrisTexto;
            }

            lblDetalleTitulo.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            lblDetalleTitulo.ForeColor = VerdeTitulo;

            UITextBox[] campos =
            [
                txtDetId, txtDetNombre, txtDetTipo, txtDetReferencia,
                txtDetCodebar, txtDetPrecio, txtDetRatio, txtDetEstado, txtDetDescripcion
            ];

            foreach (UITextBox campo in campos)
            {
                campo.ReadOnly = true;
                campo.FillColor = Color.White;
                campo.RectColor = ColorBordeCampo;
                campo.Font = new Font("JetBrains Mono", 9F);
                campo.TextAlignment = ContentAlignment.MiddleLeft;
            }
        }

        /// <summary>
        /// Reaplica el tema verde del modulo para pisar el UIStyleManager global de Main al embeberse.
        /// </summary>
        public void ReaplicarTema()
        {
            Style = UIStyle.Green;
            TitleColor = Verde;
            TitleForeColor = Color.White;
            BackColor = Color.White;

            gridProductos.ColumnHeadersDefaultCellStyle.BackColor = Verde;
            gridProductos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridProductos.GridColor = ColorBordeCampo;
            pnlContador.BackColor = Verde;
            lblContador.ForeColor = Color.White;
            lblContadorDetalle.ForeColor = Color.White;
            lblTitulo.ForeColor = VerdeTitulo;
            Invalidate();
        }

        /// <summary>
        /// Filtra el catalogo cada vez que cambia el texto del buscador.
        /// </summary>
        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            AplicarFiltro();
        }

        /// <summary>
        /// Muestra en la pestana de detalle el producto de la fila activa.
        /// </summary>
        private void GridProductos_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridProductos.CurrentRow?.DataBoundItem is not FilaProducto filaProducto)
            {
                return;
            }

            _idSeleccionado = filaProducto.ProductId;

            Product? producto = _productos.FirstOrDefault(p =>
                string.Equals(p.Product_id, filaProducto.ProductId, StringComparison.OrdinalIgnoreCase));

            MostrarDetalle(producto);
        }

        /// <summary>
        /// Pinta en gris las filas de productos anulados (el grid es de solo lectura).
        /// </summary>
        private void GridProductos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0
                || gridProductos.Rows[e.RowIndex].DataBoundItem is not FilaProducto filaProducto
                || !filaProducto.Anulado)
            {
                return;
            }

            e.CellStyle ??= new DataGridViewCellStyle();
            e.CellStyle.ForeColor = GrisAnulado;
            e.CellStyle.SelectionForeColor = Color.White;
        }

        /// <summary>
        /// Reconstruye las filas visibles del grid aplicando el filtro del buscador
        /// y conserva la seleccion previa cuando el producto sigue visible.
        /// El nombre de la categoria sale de ProductCategoryRules (misma logica que el CASE de R.cs).
        /// </summary>
        private void AplicarFiltro()
        {
            string filtro = (txtSearch.Text ?? string.Empty).Trim();

            IEnumerable<Product> consulta = _productos;
            if (filtro.Length > 0)
            {
                consulta = _productos.Where(p =>
                    Contiene(p.Product_id, filtro) || Contiene(p.Product_Name, filtro));
            }

            List<FilaProducto> filas = consulta
                .Select(p => new FilaProducto(
                    p.Product_id ?? string.Empty,
                    p.Product_Name ?? string.Empty,
                    ProductCategoryRules.GetNombre(p.Master, p.RolloCortado, p.Hoja, p.Graphics),
                    p.Anulado))
                .ToList();

            gridProductos.DataSource = filas;
            ActualizarContador();
            RestaurarSeleccion();
        }

        /// <summary>
        /// Vuelve a seleccionar el producto previo tras rellenar el grid; si ya no esta,
        /// selecciona el primero para que el detalle no quede vacio sin motivo.
        /// </summary>
        private void RestaurarSeleccion()
        {
            if (gridProductos.Rows.Count == 0)
            {
                _idSeleccionado = null;
                LimpiarDetalle();
                return;
            }

            int indice = 0;
            if (_idSeleccionado is not null)
            {
                for (int i = 0; i < gridProductos.Rows.Count; i++)
                {
                    if (gridProductos.Rows[i].DataBoundItem is FilaProducto fila
                        && string.Equals(fila.ProductId, _idSeleccionado, StringComparison.OrdinalIgnoreCase))
                    {
                        indice = i;
                        break;
                    }
                }
            }

            gridProductos.ClearSelection();
            gridProductos.Rows[indice].Selected = true;
            gridProductos.CurrentCell = gridProductos.Rows[indice].Cells[0];
        }

        /// <summary>
        /// Rellena la pestana de detalle con el producto indicado.
        /// </summary>
        private void MostrarDetalle(Product? producto)
        {
            if (producto is null)
            {
                LimpiarDetalle();
                return;
            }

            txtDetId.Text = producto.Product_id ?? string.Empty;
            txtDetNombre.Text = producto.Product_Name ?? string.Empty;
            txtDetTipo.Text = ProductCategoryRules.GetNombre(
                producto.Master, producto.RolloCortado, producto.Hoja, producto.Graphics);
            txtDetReferencia.Text = producto.Referencia ?? string.Empty;
            txtDetCodebar.Text = producto.Codigo_Barra ?? string.Empty;
            txtDetPrecio.Text = producto.Precio.ToString("N2", CultureInfo.InvariantCulture);
            txtDetRatio.Text = producto.Ratio.ToString("N4", CultureInfo.InvariantCulture);
            txtDetEstado.Text = producto.Anulado ? "ANULADO" : "Vigente";
            txtDetDescripcion.Text = producto.Product_Description ?? string.Empty;
        }

        /// <summary>
        /// Deja vacios todos los campos de la pestana de detalle.
        /// </summary>
        private void LimpiarDetalle()
        {
            foreach (Control control in tlpDetalle.Controls)
            {
                if (control is UITextBox caja)
                {
                    caja.Text = string.Empty;
                }
            }
        }

        /// <summary>
        /// Actualiza la franja verde: "existentes" son los productos no anulados.
        /// </summary>
        private void ActualizarContador()
        {
            int total = _productos.Count;
            int anulados = _productos.Count(p => p.Anulado);
            int existentes = total - anulados;
            int mostrando = gridProductos.Rows.Count;

            lblContador.Text = $"Productos existentes: {existentes}";
            lblContadorDetalle.Text = $"Mostrando {mostrando}  -  Anulados {anulados}  -  Total {total}";
        }

        /// <summary>Contenido insensible a mayusculas para el buscador.</summary>
        private static bool Contiene(string? valor, string filtro)
            => (valor ?? string.Empty).Contains(filtro, StringComparison.OrdinalIgnoreCase);

        /// <summary>Fila enlazada al grid: los tres campos visibles mas el estado para poder pintarlos.</summary>
        private sealed record FilaProducto(string ProductId, string ProductName, string Tipo, bool Anulado);
    }
}
