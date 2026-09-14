using System.Data;
using System.Globalization;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Forms.Seleccion;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.PedidoService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProductsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Pedidos de Cliente: listado, creación de nuevos pedidos con su
    /// detalle (rollos a cortar), anulación y cambio de estado.
    /// </summary>
    public partial class FrmPedidos : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IPedidoService _pedidoService;
        private readonly IProductsService _productsService;
        private readonly ICommonService _commonService;
        private readonly IConfiguration _configuration;
        private readonly string _conexion;

        private DataTable _dtCustomer = new();
        private DataTable _dtProductos = new();
        private DataTable _dtVendedor = new();
        private DataTable _dtDetalle = new();
        private Guid? _customerSeleccionado;
        private Guid? _vendedorSeleccionado;
        private string _productoSeleccionado = string.Empty;
        private string _productoNombreSeleccionado = string.Empty;
        private bool _editando;
        private int _consecutivo;

        private const decimal PORC_ITBIS_DEFAULT = 18m;

        /// <summary>
        /// Crea el formulario de pedidos de cliente con sus dependencias.
        /// </summary>
        /// <param name="pedidoService">Servicio de pedidos.</param>
        /// <param name="productsService">Servicio de productos (para el buscador).</param>
        /// <param name="commonService">Servicio común (para el selector de clientes).</param>
        /// <param name="configuration">Configuración de la aplicación (cadena de conexión).</param>
        public FrmPedidos(IPedidoService pedidoService, IProductsService productsService, ICommonService commonService, IConfiguration configuration)
        {
            InitializeComponent();
            _pedidoService = pedidoService ?? throw new ArgumentNullException(nameof(pedidoService));
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _commonService = commonService ?? throw new ArgumentNullException(nameof(commonService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            string ambiente = _configuration["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            _conexion = _configuration.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]
                        ?? throw new InvalidOperationException("No se pudo resolver la cadena de conexión.");

            this.Text = "Pedidos de Cliente";

            // Este módulo usa el estilo VERDE de SunnyUI (igual que Producción/Despacho).
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
        /// Carga asíncrona de catálogos (clientes y productos) antes de mostrar el form.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                await CargarCatalogosAsync();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Pedidos: " + ex.Message);
            }
            FinalizarConfiguracionUI();
            DeshabilitarCaptura();
        }

        private async Task CargarCatalogosAsync()
        {
            // Clientes para el picker.
            _dtCustomer = await CargarTablaAsync("SELECT customer_id, customer_name FROM customer");

            // Vendedores para el picker.
            _dtVendedor = await CargarTablaAsync("SELECT vendor_id, vendor_name FROM vendedor");

            // Productos para el buscador de la línea de detalle.
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

        private async Task<DataTable> CargarTablaAsync(string sql)
        {
            var dt = new DataTable();
            try
            {
                using var conn = new SqlConnection(_conexion);
                using var cmd = new SqlCommand(sql, conn);
                await conn.OpenAsync();
                using var da = new SqlDataAdapter(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar catálogo: " + ex.Message);
            }
            return dt;
        }

        private void FinalizarConfiguracionUI()
        {
            // Grid de detalle de captura.
            gridDetalle.Columns.Clear();
            gridDetalle.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("cant", 80, "Cant. Rollos", "cant", gridDetalle);
            CommonService.ADD_COLUMN_GRID("width", 80, "Ancho", "width", gridDetalle);
            CommonService.ADD_COLUMN_GRID("lenght", 80, "Largo", "lenght", gridDetalle);
            CommonService.ADD_COLUMN_GRID("msi", 90, "MSI", "msi", gridDetalle);
            CommonService.ADD_COLUMN_GRID("precio", 90, "Precio", "precio", gridDetalle);
            CommonService.ADD_COLUMN_GRID("total_renglon", 100, "Total", "total_renglon", gridDetalle);

            _dtDetalle = new DataTable();
            _dtDetalle.Columns.Add("cant", typeof(decimal));
            _dtDetalle.Columns.Add("width", typeof(decimal));
            _dtDetalle.Columns.Add("lenght", typeof(decimal));
            _dtDetalle.Columns.Add("msi", typeof(decimal));
            _dtDetalle.Columns.Add("precio", typeof(decimal));
            _dtDetalle.Columns.Add("total_renglon", typeof(decimal));
            gridDetalle.ReadOnly = false;
            gridDetalle.DataSource = _dtDetalle;

            gridDetalle.Columns["cant"]!.DefaultCellStyle.Format = "N0";
            gridDetalle.Columns["width"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["lenght"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["msi"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["precio"]!.DefaultCellStyle.Format = "N2";
            gridDetalle.Columns["total_renglon"]!.DefaultCellStyle.Format = "N2";

            dtpFecha.Value = DateTime.Today;
            dtpFechaEntrega.Value = DateTime.Today.AddDays(15);
            dtpFecha.ValueChanged += DtpFecha_ValueChanged;
            txtPorcItbis.Text = PORC_ITBIS_DEFAULT.ToString(CultureInfo.InvariantCulture);
        }

        private void DeshabilitarCaptura()
        {
            _editando = false;
            panelHeader.Enabled = false;
            gridDetalle.Enabled = false;
            btnAgregarLinea.Enabled = false;
            btnQuitarLinea.Enabled = false;
            tsbGuardar.Enabled = false;
            tsbBuscarCliente.Enabled = false;
            tsbBuscarVendedor.Enabled = false;
        }

        private void HabilitarCaptura()
        {
            _editando = true;
            panelHeader.Enabled = true;
            gridDetalle.Enabled = true;
            btnAgregarLinea.Enabled = true;
            btnQuitarLinea.Enabled = true;
            tsbGuardar.Enabled = true;
            tsbBuscarCliente.Enabled = true;
            tsbBuscarVendedor.Enabled = true;
        }

        private void LimpiarCaptura()
        {
            txtNumero.Clear();
            txtClienteID.Clear();
            txtClienteNombre.Clear();
            cboCondPago.SelectedIndex = 0;
            txtDireccion.Clear();
            txtContacto.Clear();
            txtNotas.Clear();
            txtSubtotal.Clear();
            txtItbis.Clear();
            txtTotal.Clear();
            cboTipoVenta.SelectedIndex = 0;
            dtpFecha.Value = DateTime.Today;
            dtpFechaEntrega.Value = DateTime.Today.AddDays(15);
            txtPorcItbis.Text = PORC_ITBIS_DEFAULT.ToString(CultureInfo.InvariantCulture);
            _customerSeleccionado = null;
            _vendedorSeleccionado = null;
            _productoSeleccionado = string.Empty;
            _productoNombreSeleccionado = string.Empty;
            txtProductoID.Clear();
            txtProductoNombre.Clear();
            txtVendedorID.Clear();
            txtVendedorNombre.Clear();
            _dtDetalle.Rows.Clear();
            gridDetalle.DataSource = null;
            gridDetalle.DataSource = _dtDetalle;
            RecargarTotales();
        }

        // NUEVO: obtiene consecutivo y habilita captura.
        private async void BtnNuevo_Click(object? sender, EventArgs e)
        {
            try
            {
                _consecutivo = await _pedidoService.GetNewNumeroPedido();
                LimpiarCaptura();
                txtNumero.Text = _consecutivo.ToString();
                HabilitarCaptura();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al obtener el número de pedido: " + ex.Message);
                MessageBox.Show("Error al obtener el número de pedido: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }

        // GUARDAR: valida, construye el pedido y guarda mostrando un loading.
        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!_editando || _consecutivo <= 0)
            {
                MessageBox.Show("Presione 'Nuevo' para iniciar un pedido.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (_customerSeleccionado == null)
            {
                MessageBox.Show("Debe seleccionar un cliente.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (string.IsNullOrEmpty(_productoSeleccionado))
            {
                MessageBox.Show("Seleccione el producto del pedido.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_dtDetalle.Rows.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos una línea de detalle.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            RecargarTotales();

            var pedido = new Pedido
            {
                Numero = _consecutivo,
                Fecha = dtpFecha.Value,
                Customer_Id = _customerSeleccionado.Value,
                Customer_Name = txtClienteNombre.Text,
                Vendor_Id = _vendedorSeleccionado,
                Persona_Contacto = txtContacto.Text.Trim(),
                Tipo_venta = cboTipoVenta.Text.Trim(),
                Fecha_entrega = dtpFechaEntrega.Value,
                Condiciones_pago = cboCondPago.Text.Trim(),
                Direccion_entrega = txtDireccion.Text.Trim(),
                Estado = PedidoEstado.Creado,
                Notas = txtNotas.Text.Trim(),
                Anulado = false,
                Porc_Itbis = ObtenerPorcItbis(),
                SubTotal = ParseDecimal(txtSubtotal.Text),
                Monto_Itbis = ParseDecimal(txtItbis.Text),
                Total = ParseDecimal(txtTotal.Text)
            };

            foreach (DataRow row in _dtDetalle.Rows)
            {
                decimal cant = Convert.ToDecimal(row["cant"]);
                decimal width = Convert.ToDecimal(row["width"]);
                decimal lenght = Convert.ToDecimal(row["lenght"]);
                decimal msi = width * lenght * cant;
                decimal precio = Convert.ToDecimal(row["precio"]);
                pedido.Detalle.Add(new PedidoDetalle
                {
                    Product_id = _productoSeleccionado,
                    Product_name = _productoNombreSeleccionado,
                    Cant = cant,
                    Unidad = "ROLLO",
                    Width = width,
                    Lenght = lenght,
                    Msi = msi,
                    Precio = precio,
                    Total_Renglon = cant * precio,
                    Notas = null
                });
            }

            tsbGuardar.Enabled = false;
            using var loading = new FrmLoading("Guardando pedido...");
            loading.Show(this);
            loading.BringToFront();
            try
            {
                bool guardado = await Task.Run(() => _pedidoService.SavePedidoCompleto(pedido));
                if (guardado)
                {
                    MessageBox.Show($"Pedido {_consecutivo} guardado.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    DeshabilitarCaptura();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el pedido: " + ObtenerErrorServicio(), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (_editando) tsbGuardar.Enabled = true;
                }
            }
            finally
            {
                if (!loading.IsDisposed) loading.Close();
            }
        }

        // Fecha de entrega: siempre 15 días después de la fecha de registro.
        private void DtpFecha_ValueChanged(object? sender, EventArgs e)
        {
            dtpFechaEntrega.Value = dtpFecha.Value.Date.AddDays(15);
        }

        // Cliente: abrir FrmSeleccion con catálogo de clientes.
        private void BtnCliente_Click(object? sender, EventArgs e)
        {
            if (!_editando) return;

            var seleccion = new FrmSeleccion(_commonService)
            {
                DtItems = _dtCustomer.Copy(),
                Titulo = "clientes"
            };
            seleccion.ShowDialog();

            if (!string.IsNullOrEmpty(seleccion.Id))
            {
                if (Guid.TryParse(seleccion.Id, out var guid))
                {
                    _customerSeleccionado = guid;
                    txtClienteID.Text = seleccion.Id;
                    txtClienteNombre.Text = seleccion.Description;
                }
                else
                {
                    MessageBox.Show("El identificador del cliente no es un GUID válido. No se permitirá guardar.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _customerSeleccionado = null;
                    txtClienteID.Clear();
                    txtClienteNombre.Clear();
                }
            }
        }

        // Vendedor: abrir FrmSeleccion con catálogo de vendedores.
        private void BtnBuscarVendedor_Click(object? sender, EventArgs e)
        {
            if (!_editando) return;

            var seleccion = new FrmSeleccion(_commonService)
            {
                DtItems = _dtVendedor.Copy(),
                Titulo = "Vendedores"
            };
            seleccion.ShowDialog();

            if (!string.IsNullOrEmpty(seleccion.Id))
            {
                if (Guid.TryParse(seleccion.Id, out var guid))
                {
                    _vendedorSeleccionado = guid;
                    txtVendedorID.Text = seleccion.Id;
                    txtVendedorNombre.Text = seleccion.Description;
                }
                else
                {
                    MessageBox.Show("El identificador del vendedor no es un GUID válido.",
                        "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    _vendedorSeleccionado = null;
                    txtVendedorID.Clear();
                    txtVendedorNombre.Clear();
                }
            }
        }

        // Producto (cabecera): abrir el buscador de rollos cortados.
        private void BtnBuscarProducto_Click(object? sender, EventArgs e)
        {
            if (!_editando) return;

            var buscador = new Frm_ProductSeach
            {
                DtItems = DtProductosFiltrados()
            };
            buscador.ShowDialog();

            if (string.IsNullOrEmpty(buscador.Selected_ProductID)) return;

            _productoSeleccionado = buscador.Selected_ProductID;
            _productoNombreSeleccionado = ObtenerNombreProducto(buscador.Selected_ProductID);
            txtProductoID.Text = _productoSeleccionado;
            txtProductoNombre.Text = _productoNombreSeleccionado;
        }

        // Catálogo de productos filtrando solo los de tipo rollo cortado.
        private DataTable DtProductosFiltrados()
        {
            if (_dtProductos.Columns.Contains("rollo_cortado"))
            {
                var dt = _dtProductos.Clone();
                foreach (DataRow row in _dtProductos.Rows)
                {
                    if (Convert.ToInt32(row["rollo_cortado"]) == 1)
                        dt.ImportRow(row);
                }
                return dt;
            }
            return _dtProductos.Copy();
        }

        private string ObtenerNombreProducto(string productId)
        {
            if (_dtProductos.Columns.Contains("product_id"))
            {
                var rows = _dtProductos.Select($"product_id = '{productId.Replace("'", "''")}'");
                if (rows.Length > 0)
                    return rows[0]["product_name"]?.ToString() ?? productId;
            }
            return productId;
        }

        // Agregar línea de detalle (una por cada medida/rollo del producto de cabecera).
        private void BtnAgregarLinea_Click(object? sender, EventArgs e)
        {
            if (!_editando) return;

            if (string.IsNullOrEmpty(_productoSeleccionado))
            {
                MessageBox.Show("Seleccione el producto del pedido en el encabezado.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            var newRow = _dtDetalle.NewRow();
            newRow["cant"] = 1m;
            newRow["width"] = 0m;
            newRow["lenght"] = 0m;
            newRow["msi"] = 0m;
            newRow["precio"] = 0m;
            newRow["total_renglon"] = 0m;
            _dtDetalle.Rows.Add(newRow);
            RecargarTotales();
        }

        // Quitar línea seleccionada del detalle.
        private void BtnQuitarLinea_Click(object? sender, EventArgs e)
        {
            if (!_editando) return;
            if (gridDetalle.CurrentRow == null) return;
            gridDetalle.Rows.Remove(gridDetalle.CurrentRow);
            RecargarTotales();
        }

        // Al terminar de editar una celda, recalcula msi y total del renglón.
        private void GridDetalle_CellEndEdit(object? sender, DataGridViewCellEventArgs e)
        {
            RecalcularRenglon(e.RowIndex);
            RecargarTotales();
        }

        private void RecalcularRenglon(int rowIndex)
        {
            if (rowIndex < 0 || rowIndex >= _dtDetalle.Rows.Count) return;
            var row = _dtDetalle.Rows[rowIndex];

            decimal cant = Convert.ToDecimal(row["cant"]);
            decimal width = Convert.ToDecimal(row["width"]);
            decimal lenght = Convert.ToDecimal(row["lenght"]);
            decimal precio = Convert.ToDecimal(row["precio"]);

            row["msi"] = width * lenght * cant;
            row["total_renglon"] = cant * precio;
        }

        private void RecargarTotales()
        {
            decimal subtotal = 0m;
            foreach (DataRow row in _dtDetalle.Rows)
            {
                if (row["total_renglon"] == DBNull.Value) continue;
                subtotal += Convert.ToDecimal(row["total_renglon"]);
            }

            decimal porc = ObtenerPorcItbis();
            decimal itbis = subtotal * porc;
            decimal total = subtotal + itbis;

            txtSubtotal.Text = subtotal.ToString("N2", CultureInfo.InvariantCulture);
            txtItbis.Text = itbis.ToString("N2", CultureInfo.InvariantCulture);
            txtTotal.Text = total.ToString("N2", CultureInfo.InvariantCulture);
        }

        private decimal ObtenerPorcItbis()
        {
            if (decimal.TryParse(txtPorcItbis.Text.Trim(), NumberStyles.Number, CultureInfo.InvariantCulture, out var porc))
                return porc / 100m;
            return PORC_ITBIS_DEFAULT / 100m;
        }

        private static decimal ParseDecimal(string value)
        {
            if (decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
                return result;
            return 0m;
        }

        /// <summary>
        /// Obtiene el mensaje de error del último fallo del servicio de pedidos.
        /// </summary>
        private string ObtenerErrorServicio()
        {
            if (_pedidoService is PedidoService ps && !string.IsNullOrEmpty(ps.ErrorMsg))
                return ps.ErrorMsg;
            return "Error desconocido.";
        }
    }
}
