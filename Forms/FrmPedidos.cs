using System.Data;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Helpers;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.PedidoService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProductsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Pedidos de Cliente: listado, búsqueda en vivo por numero/cliente/estado
    /// y contador de pedidos visibles.
    /// </summary>
    public partial class FrmPedidos : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IPedidoService _pedidoService;
        private readonly IProductsService _productsService;
        private DataTable _dtPedidos;
        private DataTable? _dtProductos;
        private CancellationTokenSource? _ctsDetalle;
        private int _ultimoPedidoDetalleConsulta;
        private readonly Dictionary<UIComboBox, object?> _valoresSel = new();

        /// <summary>
        /// Crea el formulario de pedidos con sus servicios por DI.
        /// </summary>
        /// <param name="pedidoService">Servicio de acceso a datos de pedidos.</param>
        /// <param name="productsService">Servicio del catálogo de productos.</param>
        /// <param name="configuration">Configuración de la aplicación.</param>
        public FrmPedidos(IPedidoService pedidoService, IProductsService productsService, IConfiguration configuration)
        {
            InitializeComponent();

            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            ArgumentNullException.ThrowIfNull(configuration);

            Text = "Pedidos de Cliente";

            // Este módulo usa el estilo VERDE de SunnyUI (igual que Clientes/Producción/Despacho).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            ConfigurarGridPedidos();
            AplicarTemaVerde();
            uiDataGridView1.AllowUserToAddRows = false;
            uiDataGridView1.ReadOnly = true;

            // Combos con búsqueda incremental mientras se escribe.
            ConfigurarComboFiltroIncremental(cbo_customers);
            ConfigurarComboFiltroIncremental(uiComboBox1);
            ConfigurarComboFiltroIncremental(uiComboBox2);
            ConfigurarSincronizacionValores();

            // Búsqueda en vivo sin recargar la BD: filtra el DataTable ya cargado.
            txtBuscarPedido.TextChanged += TxtBuscarPedido_TextChanged;
            btnLimpiarBusqueda.Click += BtnLimpiarBusqueda_Click;

            // Acciones sobre las líneas de producto del detalle.
            btnAddProducto.Click += BtnAddProducto_Click;
            btnBuscarProducto.Click += BtnBuscarProducto_Click;
        }

        /// <summary>
        /// Reaplica el tema verde para pisar el UIStyleManager global del Main.
        /// </summary>
        public void ReaplicarTema() => AplicarTemaVerde();

        private void AplicarTemaVerde()
        {
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = LightGreenTheme.PrimaryDark;
            TitleForeColor = Color.White;

            EstilizarGridVerde();
        }

        private void EstilizarGridVerde()
        {
            gridPedidos.BackgroundColor = LightGreenTheme.AlternateRow;
            gridPedidos.GridColor = Color.FromArgb(200, 220, 180);
            gridPedidos.EnableHeadersVisualStyles = false;

            gridPedidos.ColumnHeadersDefaultCellStyle.BackColor = LightGreenTheme.PrimaryDark;
            gridPedidos.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridPedidos.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            gridPedidos.RowsDefaultCellStyle.BackColor = Color.White;
            gridPedidos.RowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
            gridPedidos.RowsDefaultCellStyle.SelectionBackColor = LightGreenTheme.Primary;
            gridPedidos.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            gridPedidos.AlternatingRowsDefaultCellStyle.BackColor = LightGreenTheme.AlternateRow;
            gridPedidos.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
        }

        /// <summary>
        /// Carga asíncrona del listado de pedidos antes de mostrar el form.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _dtPedidos = await _pedidoService.LoadDataPedidos();
                gridPedidos.DataSource = _dtPedidos;
                gridPedidos.ClearSelection();
                gridPedidos.CurrentCell = null;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Pedidos: " + ex.Message);
            }
            ActualizarContador();
            await CargarCombosAsync();
        }

        private void ConfigurarGridPedidos()
        {
            gridPedidos.AutoGenerateColumns = false;
            gridPedidos.MultiSelect = false;
            gridPedidos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridPedidos.Columns.Clear();
            gridPedidos.SelectionChanged += GridPedidos_SelectionChanged;
            CommonService.ADD_COLUMN_GRID("numero", 90, "Numero", "numero", gridPedidos);
            CommonService.ADD_COLUMN_GRID("customer_name", 160, "Cliente", "customer_name", gridPedidos);
            CommonService.ADD_COLUMN_GRID("estado", 110, "Status", "estado", gridPedidos);

            gridPedidos.Columns["numero"].FillWeight = 20;
            gridPedidos.Columns["customer_name"].FillWeight = 60;
            gridPedidos.Columns["estado"].FillWeight = 20;
        }

        // BUSCAR: filtra el DataTable cargado por numero, cliente o estado (sin volver a la BD).
        private void TxtBuscarPedido_TextChanged(object? sender, EventArgs e)
        {
            if (_dtPedidos == null)
            {
                return;
            }

            string filtro = EscapeLike(txtBuscarPedido.Text?.Trim() ?? string.Empty);
            _dtPedidos.DefaultView.RowFilter = filtro.Length == 0
                ? string.Empty
                : "CONVERT(numero, 'System.String') LIKE '%" + filtro + "%' OR customer_name LIKE '%" + filtro + "%' OR estado LIKE '%" + filtro + "%'";
            gridPedidos.DataSource = _dtPedidos;
            gridPedidos.ClearSelection();
            gridPedidos.CurrentCell = null;
            ActualizarContador();
        }

        private void GridPedidos_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridPedidos.SelectedRows.Count == 0)
            {
                return;
            }

            CargarPedidoEnGeneral(gridPedidos.SelectedRows[0].Index);
        }

        /// <summary>
        /// Muestra los datos del pedido seleccionado en la pestaña General.
        /// </summary>
        private void CargarPedidoEnGeneral(int rowIndex)
        {
            if (_dtPedidos == null || rowIndex < 0 || rowIndex >= _dtPedidos.DefaultView.Count)
            {
                return;
            }

            DataRowView drv = _dtPedidos.DefaultView[rowIndex];
            AsignarCombo(cbo_customers, Safe(drv, "customer_id"), _valoresSel);
            AsignarCombo(uiComboBox1, Safe(drv, "vendor_id"), _valoresSel);
            uiTextBox1.Text = Safe(drv, "numero")?.ToString() ?? string.Empty;
            if (DateTime.TryParse(Safe(drv, "fecha")?.ToString(), out DateTime fecha))
            {
                uiDatetimePicker1.Value = fecha;
            }

            if (DateTime.TryParse(Safe(drv, "fecha_entrega")?.ToString(), out DateTime fechaEntrega))
            {
                uiDatetimePicker2.Value = fechaEntrega;
            }

            uiTextBox2.Text = Safe(drv, "estado")?.ToString() ?? string.Empty;
            uiRichTextBox1.Text = Safe(drv, "direccion_entrega")?.ToString() ?? string.Empty;

            string shipTo = Safe(drv, "customer_name")?.ToString() ?? string.Empty;
            string contacto = Safe(drv, "persona_contacto")?.ToString() ?? string.Empty;
            uiRichTextBox2.Text = string.IsNullOrEmpty(contacto)
                ? shipTo
                : string.IsNullOrEmpty(shipTo) ? contacto : shipTo + Environment.NewLine + "Contacto: " + contacto;

            uiRichTextBox3.Text = Safe(drv, "notas")?.ToString() ?? string.Empty;
            uiTextBox4.Text = FormatoDinero(Safe(drv, "subtotal"));
            uiTextBox5.Text = FormatoDinero(Safe(drv, "itbis"));
            uiTextBox6.Text = FormatoDinero(Safe(drv, "total$"));

            string numero = Safe(drv, "numero")?.ToString() ?? string.Empty;
            _ = CargarDetallePedidoAsync(numero);
        }

        /// <summary>
        /// Carga las líneas del pedido seleccionado en el grid de detalle.
        /// </summary>
        private async Task CargarDetallePedidoAsync(string numero)
        {
            if (!int.TryParse(numero, out int numeroPedido))
            {
                uiDataGridView1.Rows.Clear();
                return;
            }

            _ctsDetalle?.Cancel();
            _ctsDetalle?.Dispose();
            CancellationTokenSource cts = new();
            _ctsDetalle = cts;
            _ultimoPedidoDetalleConsulta = numeroPedido;

            try
            {
                DataTable detalle = await _pedidoService.LoadDataPedidoDetalle(numeroPedido, cts.Token).ConfigureAwait(false);
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                if (_ultimoPedidoDetalleConsulta != numeroPedido)
                {
                    return;
                }

                if (uiDataGridView1.InvokeRequired)
                {
                    uiDataGridView1.BeginInvoke((Action)(() => LlenarGridDetalle(detalle)));
                }
                else
                {
                    LlenarGridDetalle(detalle);
                }
            }
            catch (OperationCanceledException)
            {
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el detalle del pedido: " + ex.Message);
            }
            finally
            {
                if (ReferenceEquals(_ctsDetalle, cts))
                {
                    _ctsDetalle = null;
                }
            }
        }

        private void LlenarGridDetalle(DataTable detalle)
        {
            uiDataGridView1.Rows.Clear();
            int renglon = 0;
            foreach (DataRow row in detalle.Rows)
            {
                renglon++;
                uiDataGridView1.Rows.Add(
                    renglon,
                    row["product_name"]?.ToString() ?? string.Empty,
                    row["unidad"]?.ToString() ?? string.Empty,
                    row["cant"]?.ToString() ?? string.Empty,
                    row["notas"]?.ToString() ?? string.Empty,
                    FormatoDinero(row["precio"]),
                    FormatoDinero(row["total_renglon"]));
            }
        }

        /// <summary>
        /// Formatea un monto como decimal con 2 dígitos; devuelve string vacío si no es monto.
        /// </summary>
        private static string FormatoDinero(object? value)
        {
            return value != null
                && decimal.TryParse(value.ToString(), out decimal monto)
                ? monto.ToString("N2")
                : string.Empty;
        }

        /// <summary>
        /// Lee una columna del pedido devolviendo null si no existe en el esquema.
        /// </summary>
        private static object? Safe(DataRowView drv, string column)
        {
            return drv.DataView.Table.Columns.Contains(column) ? drv[column] : null;
        }

        private void ActualizarContador()
        {
            lblContador.Text = _dtPedidos == null ? "0 pedidos" : $"{_dtPedidos.DefaultView.Count} pedidos";
        }

        private static string EscapeLike(string value)
        {
            return value.Replace("'", "''");
        }

        private void BtnLimpiarBusqueda_Click(object? sender, EventArgs e)
        {
            txtBuscarPedido.Clear();
            txtBuscarPedido.Focus();
        }

        /// <summary>
        /// Activa la búsqueda incremental nativa de SunnyUI en el combo: editable y filtra
        /// la lista mientras el usuario escribe.
        /// </summary>
        private static void ConfigurarComboFiltroIncremental(UIComboBox combo)
        {
            combo.DropDownStyle = UIDropDownStyle.DropDown;
            combo.ShowFilter = true;
            combo.FilterIgnoreCase = true;
            combo.TrimFilter = true;
            combo.FilterMaxCount = 5000;
        }

        /// <summary>
        /// SunnyUI vuelve no-op el setter de <see cref="UIComboBox.SelectedValue"/> cuando el
        /// combo usa filtro incremental (ShowFilter=true); por eso se registra cada selección
        /// (por código o del usuario) en <see cref="_valoresSel"/> para poder leerla después.
        /// </summary>
        private void ConfigurarSincronizacionValores()
        {
            foreach (UIComboBox combo in new[] { cbo_customers, uiComboBox1, uiComboBox2 })
            {
                combo.SelectedValueChanged += (_, _) => _valoresSel[combo] = combo.SelectedValue;
            }
        }

        /// <summary>
        /// Asigna el valor de un combo con filtro incremental: con ShowFilter=true el setter
        /// de SelectedValue no hace nada, por lo que se muestra el texto del elemento y se
        /// guarda el valor en el registro para poder consultarlo con <see cref="ValorCombo"/>.
        /// </summary>
        private static void AsignarCombo(UIComboBox combo, object? valor, Dictionary<UIComboBox, object?> registro)
        {
            registro[combo] = valor == null || valor == DBNull.Value ? null : valor;

            if (valor == null || valor == DBNull.Value)
            {
                combo.Text = string.Empty;
                return;
            }

            if (combo.DataSource is DataTable tabla)
            {
                string? vm = combo.ValueMember;
                string? dm = combo.DisplayMember;
                if (!string.IsNullOrEmpty(vm) && !string.IsNullOrEmpty(dm))
                {
                    DataRow[] filas = tabla.Select($"{vm} = '{EscapeLike(valor.ToString() ?? string.Empty)}'");
                    if (filas.Length > 0)
                    {
                        combo.Text = filas[0][dm]?.ToString() ?? string.Empty;
                        return;
                    }
                }
            }

            combo.Text = string.Empty;
        }

        /// <summary>
        /// Devuelve el valor seleccionado de un combo con filtro incremental, priorizando el
        /// valor registrado (por código o por selección del usuario).
        /// </summary>
        private object? ValorCombo(UIComboBox combo)
        {
            return _valoresSel.TryGetValue(combo, out object? valor) ? valor : combo.SelectedValue;
        }

        /// <summary>
        /// Llena los combos de cliente y producto (solo registros activos) para el detalle del pedido.
        /// </summary>
        private async Task CargarCombosAsync()
        {
            try
            {
                DataTable clientes = await _pedidoService.LoadDataCustomers();
                cbo_customers.DataSource = clientes;
                cbo_customers.DisplayMember = "customer_name";
                cbo_customers.ValueMember = "customer_id";

                DataTable vendedores = await _pedidoService.LoadDataVendors();
                uiComboBox1.DataSource = vendedores;
                uiComboBox1.DisplayMember = "vendor_name";
                uiComboBox1.ValueMember = "vendor_id";

                DataSet dsProductos = await _productsService.Load();
                _dtProductos = dsProductos.Tables["DtProducts"];
                if (_dtProductos != null)
                {
                    DataView dvProductos = new(_dtProductos, "anulado = 0", "product_name", DataViewRowState.CurrentRows);
                    uiComboBox2.DataSource = dvProductos;
                    uiComboBox2.DisplayMember = "product_name";
                    uiComboBox2.ValueMember = "product_id";
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar los combos de Pedidos: " + ex.Message);
            }
        }

        /// <summary>
        /// Agrega la línea del producto seleccionado (y su cantidad) al grid de detalle.
        /// </summary>
        private void BtnAddProducto_Click(object? sender, EventArgs e)
        {
            if (_dtProductos == null)
            {
                return;
            }

            object? productIdRaw = ValorCombo(uiComboBox2);
            if (productIdRaw == null)
            {
                return;
            }

            string productId = productIdRaw.ToString() ?? string.Empty;
            DataRow[] filas = _dtProductos.Select($"product_id = '{EscapeLike(productId)}'");
            if (filas.Length == 0)
            {
                return;
            }

            DataRow prod = filas[0];
            string descripcion = prod["product_name"]?.ToString() ?? string.Empty;
            decimal precio = decimal.TryParse(prod["precio"]?.ToString(), out decimal p) ? p : 0m;
            string qty = uiTextBox3.Text?.Trim() ?? string.Empty;

            int renglon = uiDataGridView1.Rows.Count + 1;
            uiDataGridView1.Rows.Add(renglon, "ROLLO", descripcion, qty, string.Empty, precio.ToString("N2"), string.Empty);
            uiDataGridView1.ClearSelection();
        }

        /// <summary>
        /// Abre el buscador de productos; al elegir uno lo selecciona en el combo de producto.
        /// </summary>
        private void BtnBuscarProducto_Click(object? sender, EventArgs e)
        {
            if (_dtProductos == null)
            {
                return;
            }

            Frm_ProductSeach buscador = new()
            {
                DtItems = _dtProductos.Copy()
            };
            buscador.ShowDialog();

            if (string.IsNullOrEmpty(buscador.Selected_ProductID))
            {
                return;
            }

            AsignarCombo(uiComboBox2, buscador.Selected_ProductID, _valoresSel);
        }

        private void btnBuscarProducto_Click_1(object sender, EventArgs e)
        {

        }
    }
}
