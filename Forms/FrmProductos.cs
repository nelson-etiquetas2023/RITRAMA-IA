using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Helpers;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ProductsService;
using Ritrama2025.Services.ProduccionService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Productos: captura de productos con franja verde de datos,
    /// zona de detalle de items (grid), guardado en lote (INSERT/UPDATE) y anulación.
    /// </summary>
    public partial class FrmProductos : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private readonly string _conexion;

        private DataTable _dtProductos = new();
        private DataTable _dtDetalle = new();
        private bool _capturando;
        private bool _editandoNuevo;
        private bool _esperandoPrimerAdd;

        private const string SQL_INSERT_PRODUCTO =
            "INSERT INTO producto (Product_ID,Product_Name,Category_ID,unidad,width,lenght,cantidad,msi,precio,MasterRolls,rollo_cortado,Resmas,Graphics,anulado) " +
            "VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,0)";

        private const string SQL_UPDATE_PRODUCTO =
            "UPDATE producto SET Product_Name=@p2,Category_ID=@p3,unidad=@p4,width=@p5,lenght=@p6,cantidad=@p7,msi=@p8,precio=@p9,MasterRolls=@p10,rollo_cortado=@p11,Resmas=@p12,Graphics=@p13 WHERE Product_ID=@p1";

        private const string SQL_ANULAR_PRODUCTO =
            "UPDATE producto SET anulado = 1 WHERE Product_ID = @p1";

        /// <summary>
        /// Crea el formulario de productos con sus dependencias.
        /// </summary>
        /// <param name="productsService">Servicio de productos (catálogo).</param>
        /// <param name="configuration">Configuración de la aplicación (cadena de conexión).</param>
        public FrmProductos(IProductsService productsService, IConfiguration configuration)
        {
            InitializeComponent();
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            string ambiente = _configuration["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            _conexion = _configuration.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]
                        ?? throw new InvalidOperationException("No se pudo resolver la cadena de conexión.");

            this.Text = "Productos";

            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();
        }

        /// <summary>
        /// Reaplica el tema verde para pisar el UIStyleManager global del Main.
        /// </summary>
        public void ReaplicarTema() => AplicarTemaVerde();

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            this.BackColor = Color.White;
            this.Style = UIStyle.Green;
            this.TitleColor = verde;
            this.TitleForeColor = Color.White;
        }

        /// <summary>
        /// Carga asíncrona del catálogo de productos antes de mostrar el form.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                await CargarCatalogoAsync();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Productos: " + ex.Message);
            }
            FinalizarConfiguracionUI();
            DeshabilitarCaptura();
        }

        private async Task CargarCatalogoAsync()
        {
            try
            {
                var ds = await _productsService.Load();
                _dtProductos = ds.Tables["Dtproducts"] ?? new DataTable();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar productos: " + ex.Message);
                _dtProductos = new DataTable();
            }
        }

        private void FinalizarConfiguracionUI()
        {
            gridDetalle.Columns.Clear();
            gridDetalle.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("product_id", 120, "Código", "product_id", gridDetalle);
            CommonService.ADD_COLUMN_GRID("product_name", 220, "Producto", "product_name", gridDetalle);
            CommonService.ADD_COLUMN_GRID("categoria", 130, "Categoría", "categoria", gridDetalle);
            CommonService.ADD_COLUMN_GRID("unidad", 70, "Unidad", "unidad", gridDetalle);
            CommonService.ADD_COLUMN_GRID("cantidad", 80, "Cantidad", "cantidad", gridDetalle);
            CommonService.ADD_COLUMN_GRID("width", 80, "Ancho", "width", gridDetalle);
            CommonService.ADD_COLUMN_GRID("lenght", 80, "Largo", "lenght", gridDetalle);
            CommonService.ADD_COLUMN_GRID("msi", 100, "MSI", "msi", gridDetalle);
            CommonService.ADD_COLUMN_GRID("precio", 90, "Precio", "precio", gridDetalle);
            CommonService.ADD_COLUMN_GRID("total_renglon", 110, "Total", "total_renglon", gridDetalle);

            _dtDetalle = CrearTablaDetalle();
            RepoblarDetalleDesdeCatalogo();
            gridDetalle.DataSource = _dtDetalle;

            gridDetalle.Columns["cantidad"]!.DefaultCellStyle.Format = "N0";
            gridDetalle.Columns["width"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["lenght"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["msi"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["precio"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["total_renglon"]!.DefaultCellStyle.Format = "N2";
        }

        private static DataTable CrearTablaDetalle()
        {
            var dt = new DataTable();
            dt.Columns.Add("product_id", typeof(string));
            dt.Columns.Add("product_name", typeof(string));
            dt.Columns.Add("categoria", typeof(string));
            dt.Columns.Add("unidad", typeof(string));
            dt.Columns.Add("cantidad", typeof(decimal));
            dt.Columns.Add("width", typeof(decimal));
            dt.Columns.Add("lenght", typeof(decimal));
            dt.Columns.Add("msi", typeof(decimal));
            dt.Columns.Add("precio", typeof(decimal));
            dt.Columns.Add("total_renglon", typeof(decimal));
            return dt;
        }

        private void RepoblarDetalleDesdeCatalogo()
        {
            _dtDetalle.Rows.Clear();
            if (_dtProductos == null) return;

            foreach (DataRow prod in _dtProductos.Rows)
            {
                var row = _dtDetalle.NewRow();
                row["product_id"] = prod["product_id"]?.ToString() ?? string.Empty;
                row["product_name"] = prod["product_name"]?.ToString() ?? string.Empty;
                row["categoria"] = ObtenerCategoria(prod);
                row["unidad"] = "ROLLO";
                row["cantidad"] = 1m;
                row["width"] = 0m;
                row["lenght"] = 0m;
                row["msi"] = 0m;
                row["precio"] = ObtenerDecimal(prod, "precio");
                row["total_renglon"] = Convert.ToDecimal(row["precio"]);
                _dtDetalle.Rows.Add(row);
            }
        }

        private static string ObtenerCategoria(DataRow row)
        {
            if (row.Table.Columns.Contains("tipo"))
            {
                string tipo = row["tipo"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(tipo)) return tipo.Trim();
            }
            if (row.Table.Columns.Contains("category_id"))
                return row["category_id"]?.ToString() ?? string.Empty;
            return string.Empty;
        }

        private static decimal ObtenerDecimal(DataRow row, string column)
        {
            if (!row.Table.Columns.Contains(column)) return 0m;
            object valor = row[column];
            if (valor == null || valor == DBNull.Value) return 0m;
            if (decimal.TryParse(valor.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0m;
        }

        private void DeshabilitarCaptura()
        {
            _capturando = false;
            _editandoNuevo = false;
            _esperandoPrimerAdd = false;
            panelFranja.Enabled = false;
            btnAdd.Enabled = false;
            tsbGuardar.Enabled = false;
            tsbCancelar.Enabled = false;
            tsbAnular.Enabled = false;
        }

        private void HabilitarCapturaNuevo()
        {
            _capturando = true;
            _editandoNuevo = true;
            panelFranja.Enabled = true;
            btnAdd.Enabled = true;
            tsbGuardar.Enabled = true;
            tsbCancelar.Enabled = true;
            tsbAnular.Enabled = false;
        }

        private void LimpiarBand()
        {
            txtBuscar.Clear();
            txtProductId.Clear();
            txtNombre.Clear();
            txtUnidad.Text = "ROLLO";
            txtCantidad.Clear();
            txtWidth.Clear();
            txtLenght.Clear();
            txtMsi.Clear();
            txtPrecio.Clear();
            txtTotal.Clear();
            cboCategoria.SelectedIndex = -1;
        }

        private bool BandTieneContenido()
        {
            return !string.IsNullOrWhiteSpace(txtProductId.Text) || !string.IsNullOrWhiteSpace(txtNombre.Text);
        }

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            if (_editandoNuevo && BandTieneContenido())
            {
                var respuesta = MessageBox.Show(
                    "Existe una captura sin guardar. ¿Desea descartarla y comenzar una nueva?",
                    "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
                if (respuesta != DialogResult.Yes) return;
            }

            LimpiarBand();
            gridDetalle.ClearSelection();
            _esperandoPrimerAdd = true;
            HabilitarCapturaNuevo();
            txtProductId.Focus();
        }

        private void BtnBuscar_Click(object? sender, EventArgs e)
        {
            var buscador = new Frm_ProductSeach
            {
                DtItems = _dtProductos.Copy()
            };
            buscador.ShowDialog();

            if (string.IsNullOrEmpty(buscador.Selected_ProductID)) return;

            var filas = _dtProductos.Select($"product_id = '{EscapeLike(buscador.Selected_ProductID)}'");
            if (filas.Length == 0) return;

            var prod = filas[0];
            txtBuscar.Text = prod["product_name"]?.ToString() ?? string.Empty;
            txtProductId.Text = prod["product_id"]?.ToString() ?? string.Empty;
            txtNombre.Text = prod["product_name"]?.ToString() ?? string.Empty;
            SetCategoriaCombo(ObtenerCategoria(prod));
            txtPrecio.Text = ObtenerDecimal(prod, "precio").ToString("N2", CultureInfo.InvariantCulture);
            txtUnidad.Text = "ROLLO";
        }

        private void SetCategoriaCombo(string categoria)
        {
            int idx = cboCategoria.Items.IndexOf(categoria);
            cboCategoria.SelectedIndex = idx >= 0 ? idx : -1;
        }

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            if (!_capturando) return;

            if (string.IsNullOrWhiteSpace(txtProductId.Text))
            {
                MessageBox.Show("Debe ingresar el Product ID.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar el nombre del producto.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }
            decimal cantidad = ParseDecimal(txtCantidad.Text);
            if (cantidad <= 0)
            {
                MessageBox.Show("La cantidad debe ser mayor que cero.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string productoId = txtProductId.Text.Trim();
            decimal width = ParseDecimal(txtWidth.Text);
            decimal lenght = ParseDecimal(txtLenght.Text);
            decimal precio = ParseDecimal(txtPrecio.Text);
            decimal msi = width * lenght * cantidad;
            decimal total = cantidad * precio;
            string categoria = cboCategoria.Text;
            string unidad = string.IsNullOrWhiteSpace(txtUnidad.Text) ? "ROLLO" : txtUnidad.Text.Trim();

            var fila = BuscarFilaPorProductId(productoId);
            if (fila != null)
            {
                fila["product_name"] = txtNombre.Text.Trim();
                fila["categoria"] = categoria;
                fila["unidad"] = unidad;
                fila["cantidad"] = cantidad;
                fila["width"] = width;
                fila["lenght"] = lenght;
                fila["msi"] = msi;
                fila["precio"] = precio;
                fila["total_renglon"] = total;
            }
            else
            {
                var row = _dtDetalle.NewRow();
                row["product_id"] = productoId;
                row["product_name"] = txtNombre.Text.Trim();
                row["categoria"] = categoria;
                row["unidad"] = unidad;
                row["cantidad"] = cantidad;
                row["width"] = width;
                row["lenght"] = lenght;
                row["msi"] = msi;
                row["precio"] = precio;
                row["total_renglon"] = total;
                _dtDetalle.Rows.Add(row);
            }

            _esperandoPrimerAdd = false;
            txtCantidad.Clear();
            txtWidth.Clear();
            txtLenght.Clear();
            txtPrecio.Clear();
            txtMsi.Clear();
            txtTotal.Clear();
            txtCantidad.Focus();
        }

        private DataRow? BuscarFilaPorProductId(string productId)
        {
            foreach (DataRow row in _dtDetalle.Rows)
            {
                if (string.Equals(row["product_id"]?.ToString(), productId, StringComparison.OrdinalIgnoreCase))
                    return row;
            }
            return null;
        }

        private void GridDetalle_SelectionChanged(object? sender, EventArgs e)
        {
            var current = gridDetalle.CurrentRow;
            if (current == null || current.Index < 0)
            {
                tsbAnular.Enabled = false;
                return;
            }

            object? valor = current.Cells["product_id"].Value;
            if (valor == null || valor == DBNull.Value || string.IsNullOrWhiteSpace(valor.ToString()))
            {
                tsbAnular.Enabled = false;
                return;
            }

            string productId = valor.ToString()!;
            bool existeEnBD = ProductoExisteEnCatalogo(productId);

            if (!(_editandoNuevo && _esperandoPrimerAdd))
            {
                CargarRenglonABand(current);
            }

            txtProductId.ReadOnly = existeEnBD;
            tsbAnular.Enabled = existeEnBD;

            if (existeEnBD)
            {
                _capturando = true;
                panelFranja.Enabled = true;
                btnAdd.Enabled = true;
                tsbGuardar.Enabled = true;
                tsbCancelar.Enabled = true;
            }
        }

        private bool ProductoExisteEnCatalogo(string productId)
        {
            if (_dtProductos == null || !_dtProductos.Columns.Contains("product_id")) return false;
            return _dtProductos.Select($"product_id = '{EscapeLike(productId)}'").Length > 0;
        }

        private static string EscapeLike(string value)
        {
            return value.Replace("'", "''");
        }

        private void CargarRenglonABand(DataGridViewRow row)
        {
            string categoria = row.Cells["categoria"].Value?.ToString() ?? string.Empty;
            string nombre = row.Cells["product_name"].Value?.ToString() ?? string.Empty;
            txtBuscar.Text = nombre;
            txtProductId.Text = row.Cells["product_id"].Value?.ToString() ?? string.Empty;
            txtNombre.Text = nombre;
            txtUnidad.Text = string.IsNullOrWhiteSpace(row.Cells["unidad"].Value?.ToString())
                ? "ROLLO"
                : row.Cells["unidad"].Value!.ToString();
            txtCantidad.Text = FormatearDecimal(row.Cells["cantidad"].Value);
            txtWidth.Text = FormatearDecimal(row.Cells["width"].Value);
            txtLenght.Text = FormatearDecimal(row.Cells["lenght"].Value);
            txtMsi.Text = FormatearDecimal(row.Cells["msi"].Value);
            txtPrecio.Text = FormatearDecimal(row.Cells["precio"].Value);
            txtTotal.Text = FormatearDecimal(row.Cells["total_renglon"].Value);
            SetCategoriaCombo(categoria);
        }

        private static string FormatearDecimal(object? value)
        {
            if (value == null || value == DBNull.Value) return string.Empty;
            return Convert.ToDecimal(value).ToString("N2", CultureInfo.InvariantCulture);
        }

        private void TxtMedida_TextChanged(object? sender, EventArgs e)
        {
            decimal cantidad = ParseDecimal(txtCantidad.Text);
            decimal width = ParseDecimal(txtWidth.Text);
            decimal lenght = ParseDecimal(txtLenght.Text);
            decimal precio = ParseDecimal(txtPrecio.Text);

            txtMsi.Text = (width * lenght * cantidad).ToString("N2", CultureInfo.InvariantCulture);
            txtTotal.Text = (cantidad * precio).ToString("N2", CultureInfo.InvariantCulture);
        }

        private static decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0m;
        }

        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!_capturando)
            {
                MessageBox.Show("Presione 'Nuevo' o seleccione un producto para capturar.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_dtDetalle.Rows.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto en el detalle.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            var items = new List<ProductoFila>();
            foreach (DataRow row in _dtDetalle.Rows)
            {
                string productId = row["product_id"]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(productId)) continue;
                items.Add(new ProductoFila
                {
                    ProductId = productId,
                    Nombre = row["product_name"]?.ToString() ?? string.Empty,
                    Categoria = row["categoria"]?.ToString() ?? string.Empty,
                    Unidad = row["unidad"]?.ToString() ?? string.Empty,
                    Cantidad = Convert.ToDecimal(row["cantidad"]),
                    Width = Convert.ToDecimal(row["width"]),
                    Lenght = Convert.ToDecimal(row["lenght"]),
                    Msi = Convert.ToDecimal(row["msi"]),
                    Precio = Convert.ToDecimal(row["precio"])
                });
            }

            tsbGuardar.Enabled = false;
            using var loading = new FrmLoading("Guardando productos...");
            loading.Show(this);
            loading.BringToFront();
            try
            {
                string error = await Task.Run(() => GuardarProductos(items));
                if (string.IsNullOrEmpty(error))
                {
                    MessageBox.Show("Productos guardados correctamente.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await RecargarCatalogoAsync();
                    RepoblarDetalleDesdeCatalogo();
                    LimpiarBand();
                    DeshabilitarCaptura();
                }
                else
                {
                    MessageBox.Show("No se pudieron guardar los productos: " + error, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (_capturando) tsbGuardar.Enabled = true;
                }
            }
            finally
            {
                if (!loading.IsDisposed) loading.Close();
            }
        }

        private string GuardarProductos(List<ProductoFila> items)
        {
            try
            {
                using var conn = new SqlConnection(_conexion);
                conn.Open();
                foreach (var item in items)
                {
                    bool existe = ProductoExisteEnBD(item.ProductId, conn);
                    using var cmd = new SqlCommand(existe ? SQL_UPDATE_PRODUCTO : SQL_INSERT_PRODUCTO, conn);
                    AgregarParametrosProducto(cmd, item);
                    cmd.ExecuteNonQuery();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static bool ProductoExisteEnBD(string productId, SqlConnection conn)
        {
            using var cmd = new SqlCommand("SELECT COUNT(*) FROM producto WHERE Product_ID = @p1", conn);
            cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = productId });
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        private static void AgregarParametrosProducto(SqlCommand cmd, ProductoFila item)
        {
            cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = item.ProductId });
            cmd.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = NuloSiVacio(item.Nombre) });
            cmd.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = NuloSiVacio(item.Categoria) });
            cmd.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = NuloSiVacio(item.Unidad) });
            cmd.Parameters.Add(new SqlParameter("@p5", SqlDbType.Decimal) { Value = item.Width });
            cmd.Parameters.Add(new SqlParameter("@p6", SqlDbType.Decimal) { Value = item.Lenght });
            cmd.Parameters.Add(new SqlParameter("@p7", SqlDbType.Decimal) { Value = item.Cantidad });
            cmd.Parameters.Add(new SqlParameter("@p8", SqlDbType.Decimal) { Value = item.Msi });
            cmd.Parameters.Add(new SqlParameter("@p9", SqlDbType.Decimal) { Value = item.Precio });
            cmd.Parameters.Add(new SqlParameter("@p10", SqlDbType.Bit) { Value = item.Categoria == "Master" });
            cmd.Parameters.Add(new SqlParameter("@p11", SqlDbType.Bit) { Value = item.Categoria == "Rollo Cortado" });
            cmd.Parameters.Add(new SqlParameter("@p12", SqlDbType.Bit) { Value = item.Categoria == "Resma" });
            cmd.Parameters.Add(new SqlParameter("@p13", SqlDbType.Bit) { Value = item.Categoria == "Graphics" });
        }

        private static object NuloSiVacio(string value)
        {
            string trim = value?.Trim() ?? string.Empty;
            return trim.Length == 0 ? (object)DBNull.Value : trim;
        }

        private async Task RecargarCatalogoAsync()
        {
            try
            {
                var ds = await _productsService.Load();
                _dtProductos = ds.Tables["Dtproducts"] ?? new DataTable();
            }
            catch
            {
                _dtProductos = await CargarCatalogoDirecto();
            }
        }

        private async Task<DataTable> CargarCatalogoDirecto()
        {
            var dt = new DataTable();
            try
            {
                using var conn = new SqlConnection(_conexion);
                using var cmd = new SqlCommand(R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS, conn);
                await conn.OpenAsync();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al recargar catálogo de productos: " + ex.Message);
            }
            return dt;
        }

        private async void BtnAnular_Click(object? sender, EventArgs e)
        {
            var current = gridDetalle.CurrentRow;
            if (current == null || current.Index < 0)
            {
                MessageBox.Show("Seleccione un producto para anular.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            object? valor = current.Cells["product_id"].Value;
            if (valor == null || valor == DBNull.Value) return;
            string productId = valor.ToString()!;
            string nombre = current.Cells["product_name"].Value?.ToString() ?? productId;

            var confirm = MessageBox.Show($"¿Anular el producto {nombre} ({productId})?", "Confirmar anulación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes) return;

            tsbAnular.Enabled = false;
            using var loading = new FrmLoading("Anulando producto...");
            loading.Show(this);
            loading.BringToFront();
            try
            {
                string error = await Task.Run(() => AnularProducto(productId));
                if (string.IsNullOrEmpty(error))
                {
                    MessageBox.Show("Producto anulado.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await RecargarCatalogoAsync();
                    RepoblarDetalleDesdeCatalogo();
                    LimpiarBand();
                    DeshabilitarCaptura();
                }
                else
                {
                    MessageBox.Show("No se pudo anular el producto: " + error, "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tsbAnular.Enabled = true;
                }
            }
            finally
            {
                if (!loading.IsDisposed) loading.Close();
            }
        }

        private string AnularProducto(string productId)
        {
            try
            {
                using var conn = new SqlConnection(_conexion);
                using var cmd = new SqlCommand(SQL_ANULAR_PRODUCTO, conn);
                cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = productId });
                conn.Open();
                cmd.ExecuteNonQuery();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            RepoblarDetalleDesdeCatalogo();
            LimpiarBand();
            DeshabilitarCaptura();
        }

        private sealed class ProductoFila
        {
            public string ProductId = string.Empty;
            public string Nombre = string.Empty;
            public string Categoria = string.Empty;
            public string Unidad = string.Empty;
            public decimal Cantidad;
            public decimal Width;
            public decimal Lenght;
            public decimal Msi;
            public decimal Precio;
        }
    }
}