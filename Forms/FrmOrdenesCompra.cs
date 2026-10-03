using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.OrdenesCompras;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.PedidoService;
using Ritrama2025.Services.ProductsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    public partial class FrmOrdenesCompra : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IOrdenesComprasService _ocService;
        private readonly IProductsService _productsService;
        private DataTable? _dtOrdenes;
        private DataTable? _dtProductos;
        private DataTable? _dtProveedores;
        private CancellationTokenSource? _ctsDetalle;
        private string? _ultimaOcDetalleConsulta;

        private readonly Dictionary<UIComboBox, object?> _valoresSel = new();

        private bool _actualizandoSwitchAnulado;
        private DataRow? _filaOcActual;

        private bool _fechaEntregaCargadaNula;
        private DateTime _fechaEntregaMostrada;

        private readonly List<OrdenCompraDetalle> _lineas = new();
        private string? _productoLineaNoVisible;

        public FrmOrdenesCompra(IOrdenesComprasService ocService, IProductsService productsService, IConfiguration configuration)
        {
            InitializeComponent();
            gridOrdenes.ReadOnly = false;

            _ocService = ocService ?? throw new ArgumentNullException(nameof(ocService));
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            ArgumentNullException.ThrowIfNull(configuration);

            Text = "Órdenes de Compra";

            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();

            gridOrdenes.ReadOnly = true;
            gridOrdenes.AllowUserToAddRows = false;

            sw_anular_oc.Style = UIStyle.Custom;

            AplicarTemaVerde();

            ConfigurarFechas();

            ConfigurarComboFiltroIncremental(cbo_proveedores);
            ConfigurarComboFiltroIncremental(uiComboBox2);
            ConfigurarSincronizacionValores();

            cboCondicionesPago.DataSource = OrdenCompraCatalogos.CondicionesPago;
            cbo_prioridad.DataSource = OrdenCompraCatalogos.Prioridad;
            cboCondicionesPago.SelectedIndex = -1;
            cbo_prioridad.SelectedIndex = -1;

            cbo_proveedores.SelectedValueChanged += CboProveedores_ValueChanged;

            txtBuscar.TextChanged += TxtBuscar_TextChanged;

            sw_anular_oc.Enabled = false;
            sw_anular_oc.ActiveChanged += SwAnularOc_ActiveChanged;

            btnAddProducto.Click += (_, _) => AgregarLinea();
            btnEditarProducto.Click += (_, _) => EditarLinea();
            btnEliminarProducto.Click += (_, _) => EliminarLinea();
            btnBuscarProducto.Click += BtnBuscarProducto_Click;
            uiTextBox7.TextChanged += (_, _) => ActualizarTotalesEnPantalla();

            uiComboBox2.SelectedValueChanged += (_, _) => AplicarEditabilidadMedidas();

            btnNuevo.Click += BtnNuevo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            uiDataGridView1.SelectionChanged += (_, _) => CargarLineaSeleccionadaEnEditor();

            AplicarModo(ModoFormulario.Consulta);

            // La barra venia del disenador con botones de 92x24 e ImageScaling=None: con
            // el icono de 32px de "Nuevo" el texto quedaba cortado. Se estila en codigo
            // para que el ajuste sobreviva a regenerar el disenador.
            ToolStripTheme.Ajustar(barraHerramientas);
        }

        public void ReaplicarTema() => AplicarTemaVerde();

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = verde;
            TitleForeColor = Color.White;

            sw_anular_oc.ActiveColor = verde;
            sw_anular_oc.InActiveColor = Color.Firebrick;

            EstilizarGridVerde();
        }

        private enum ModoFormulario
        {
            Consulta,
            Nuevo,
            Editar
        }

        private ModoFormulario _modo = ModoFormulario.Consulta;

        private bool EsEditable => _modo is ModoFormulario.Nuevo or ModoFormulario.Editar;

        private void AplicarModo(ModoFormulario modo)
        {
            _modo = modo;

            uiTextBox1.ReadOnly = true;
            txt_id_proveedor.ReadOnly = true;
            uiTextBox2.ReadOnly = true;
            uiTextBox4.ReadOnly = true;
            uiTextBox5.ReadOnly = true;
            uiTextBox6.ReadOnly = true;
            txt_total_cantidad.ReadOnly = true;

            uiDatetimePicker1.ReadOnly = !EsEditable;
            uiDatetimePicker2.ReadOnly = !EsEditable;
            uiRichTextBox3.ReadOnly = !EsEditable;
            uiTextBox7.ReadOnly = !EsEditable;
            txtPersonaContacto.ReadOnly = !EsEditable;

            uiDatetimePicker1.Enabled = EsEditable;
            uiDatetimePicker2.Enabled = EsEditable;
            cbo_proveedores.ReadOnly = !EsEditable;
            cbo_proveedores.Enabled = EsEditable;
            cboCondicionesPago.ReadOnly = !EsEditable;
            cboCondicionesPago.Enabled = EsEditable;
            cbo_prioridad.ReadOnly = !EsEditable;
            cbo_prioridad.Enabled = EsEditable;

            uiRichTextBox1.ReadOnly = true;
            uiRichTextBox2.ReadOnly = true;

            btnAddProducto.Visible = EsEditable;
            btnEditarProducto.Visible = EsEditable;
            btnEliminarProducto.Visible = EsEditable;
            btnBuscarProducto.Visible = EsEditable;
            btnAddProducto.Enabled = EsEditable;
            btnEditarProducto.Enabled = EsEditable;
            btnEliminarProducto.Enabled = EsEditable;
            btnBuscarProducto.Enabled = EsEditable;

            AplicarEditabilidadMedidas();

            btnNuevo.Visible = _modo == ModoFormulario.Consulta;
            btnEditar.Visible = _modo == ModoFormulario.Consulta;
            btnGuardar.Visible = EsEditable;
            btnCancelar.Visible = EsEditable;

            // Cambian los botones visibles de la barra: hay que recalcular su alto.
            ToolStripTheme.AjustarAlto(barraHerramientas);

            gridOrdenes.Enabled = !EsEditable;

            _actualizandoSwitchAnulado = true;
            sw_anular_oc.Active = true;
            _actualizandoSwitchAnulado = false;
            sw_anular_oc.Enabled = _modo != ModoFormulario.Nuevo && _filaOcActual != null;
        }

        private void EstilizarGridVerde()
        {
            gridOrdenes.BackgroundColor = LightGreenTheme.AlternateRow;
            gridOrdenes.GridColor = Color.FromArgb(180, 210, 180);
            gridOrdenes.EnableHeadersVisualStyles = false;

            gridOrdenes.ColumnHeadersDefaultCellStyle.BackColor = LightGreenTheme.PrimaryDark;
            gridOrdenes.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridOrdenes.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

            gridOrdenes.RowsDefaultCellStyle.BackColor = Color.White;
            gridOrdenes.RowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
            gridOrdenes.RowsDefaultCellStyle.SelectionBackColor = LightGreenTheme.Primary;
            gridOrdenes.RowsDefaultCellStyle.SelectionForeColor = Color.White;

            gridOrdenes.AlternatingRowsDefaultCellStyle.BackColor = LightGreenTheme.AlternateRow;
            gridOrdenes.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
        }

        public async Task InitializeAsync()
        {
            try
            {
                _dtOrdenes = await _ocService.LoadDataOrdenesCompra();
                gridOrdenes.DataSource = _dtOrdenes;
                gridOrdenes.ClearSelection();
                gridOrdenes.CurrentCell = null;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar las órdenes de compra: " + ex.Message);
            }

            btnEditar.Enabled = false;
            ActualizarResumen();
            await CargarCombosAsync();
        }

        private void ConfigurarGridOrdenes()
        {
            gridOrdenes.AutoGenerateColumns = false;
            gridOrdenes.MultiSelect = false;
            gridOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridOrdenes.Columns.Clear();

            gridOrdenes.Columns.Add(colNumero);
            gridOrdenes.Columns.Add(colProveedor);
            gridOrdenes.Columns.Add(colEstado);

            foreach (DataGridViewColumn columna in gridOrdenes.Columns)
            {
                columna.FillWeight = columna.Name switch
                {
                    "colNumero" => 30,
                    "colProveedor" => 50,
                    "colEstado" => 20,
                    _ => 20
                };
            }

            gridOrdenes.SelectionChanged += GridOrdenes_SelectionChanged;
            gridOrdenes.CurrentCellChanged += GridOrdenes_SelectionChanged;
        }

        private void GridOrdenes_SelectionChanged(object? sender, EventArgs e)
        {
            if (_modo != ModoFormulario.Consulta)
            {
                return;
            }

            if (gridOrdenes.SelectedRows.Count == 0)
            {
                return;
            }

            CargarOrdenEnGeneral(gridOrdenes.SelectedRows[0].Index);
        }

        private void TxtBuscar_TextChanged(object? sender, EventArgs e)
        {
            if (_dtOrdenes == null)
            {
                return;
            }

            string filtro = EscapeLike(txtBuscar.Text?.Trim() ?? string.Empty);
            _dtOrdenes.DefaultView.RowFilter = filtro.Length == 0
                ? string.Empty
                : "CONVERT(numero, 'System.String') LIKE '%" + filtro + "%' OR proveedor_name LIKE '%" + filtro + "%' OR estado LIKE '%" + filtro + "%'";
            gridOrdenes.DataSource = _dtOrdenes;
            gridOrdenes.ClearSelection();
            gridOrdenes.CurrentCell = null;
            ActualizarResumen();
        }

        private void ActualizarResumen()
        {
            int total = _dtOrdenes?.Rows.Count ?? 0;
            int visibles = _dtOrdenes?.DefaultView.Count ?? 0;
            bool filtrando = (txtBuscar.Text?.Trim().Length ?? 0) > 0;

            lblResumen.Text = !filtrando
                ? $"Total: {total} {(total == 1 ? "orden" : "órdenes")}"
                : $"Mostrando {visibles} de {total} {(total == 1 ? "orden" : "órdenes")}";
        }

        private void CargarOrdenEnGeneral(int rowIndex)
        {
            if (_dtOrdenes == null || rowIndex < 0 || rowIndex >= _dtOrdenes.DefaultView.Count)
            {
                return;
            }

            DataRowView drv = _dtOrdenes.DefaultView[rowIndex];
            AsignarCombo(cbo_proveedores, Safe(drv, "proveedor_id"), _valoresSel);

            MostrarConsecutivo(cbo_proveedores, txt_id_proveedor, Safe(drv, "proveedor_id"));

            uiTextBox1.Text = Safe(drv, "numero")?.ToString() ?? string.Empty;
            if (DateTime.TryParse(Safe(drv, "fecha")?.ToString(), out DateTime fecha))
            {
                uiDatetimePicker1.Value = fecha;
            }

            object? entregaCruda = Safe(drv, "fecha_entrega");
            if (entregaCruda != null
                && entregaCruda != DBNull.Value
                && DateTime.TryParse(entregaCruda.ToString(), out DateTime fechaEntrega))
            {
                uiDatetimePicker2.Value = fechaEntrega;
                _fechaEntregaCargadaNula = false;
            }
            else
            {
                _fechaEntregaCargadaNula = true;
                _fechaEntregaMostrada = uiDatetimePicker2.Value;
            }

            uiTextBox2.Text = Safe(drv, "estado")?.ToString() ?? string.Empty;
            uiRichTextBox1.Text = Safe(drv, "direccion_facturacion")?.ToString() ?? string.Empty;
            uiRichTextBox2.Text = Safe(drv, "direccion_entrega")?.ToString() ?? string.Empty;
            uiRichTextBox3.Text = Safe(drv, "notas")?.ToString() ?? string.Empty;

            AsignarComboOpcional(cboCondicionesPago, OrdenCompraCatalogos.CondicionesPago, Safe(drv, "condiciones_pago"));
            AsignarComboOpcional(cbo_prioridad, OrdenCompraCatalogos.Prioridad, Safe(drv, "prioridad"));
            txtPersonaContacto.Text = Safe(drv, "persona_contacto")?.ToString() ?? string.Empty;

            uiTextBox7.Text = decimal.TryParse(Safe(drv, "porc_itbis")?.ToString(), out decimal porcItbis)
                ? porcItbis.ToString("0.##")
                : "18";

            uiTextBox4.Text = FormatoDinero(Safe(drv, "subtotal"));
            uiTextBox5.Text = FormatoDinero(Safe(drv, "itbis"));
            uiTextBox6.Text = FormatoDinero(Safe(drv, "total$"));

            string numero = Safe(drv, "numero")?.ToString() ?? string.Empty;
            _ = CargarDetalleOcAsync(numero);

            _filaOcActual = drv.Row;
            bool anulado = EsAnulado(drv.Row);
            _actualizandoSwitchAnulado = true;
            sw_anular_oc.Active = !anulado;
            _actualizandoSwitchAnulado = false;
            sw_anular_oc.Enabled = _modo == ModoFormulario.Consulta;

            btnEditar.Enabled = !anulado;
        }

        private static bool EsAnulado(DataRow fila)
        {
            try
            {
                object? valor = fila["anulado"];
                if (valor is null || valor is DBNull)
                {
                    return false;
                }

                return Convert.ToInt32(valor) == 1;
            }
            catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
            {
                return false;
            }
        }

        private void SwAnularOc_ActiveChanged(object? sender, EventArgs e)
        {
            if (_actualizandoSwitchAnulado)
            {
                return;
            }

            if ((_modo != ModoFormulario.Consulta && _modo != ModoFormulario.Editar)
                || _filaOcActual is null)
            {
                return;
            }

            string numero = _filaOcActual["numero"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(numero))
            {
                return;
            }

            bool activo = sw_anular_oc.Active;
            string pregunta = activo
                ? $"¿Restaurar la orden {numero}?\nVolverá a quedar activa."
                : $"¿Anular la orden {numero}?\nSeguirá visible en la lista con la fila en rojo.";
            DialogResult confirmacion = MessageBox.Show(
                pregunta,
                "Anulación de orden",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                _actualizandoSwitchAnulado = true;
                sw_anular_oc.Active = !activo;
                _actualizandoSwitchAnulado = false;
                return;
            }

            bool exitoso = activo
                ? _ocService.RestaurarOrdenCompra(numero)
                : _ocService.AnularOrdenCompra(numero);

            if (!exitoso)
            {
                MessageBox.Show(
                    "No se pudo actualizar la anulación: " + _ocService.ErrorMsg,
                    "Anulación de orden",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _actualizandoSwitchAnulado = true;
                sw_anular_oc.Active = !activo;
                _actualizandoSwitchAnulado = false;
                return;
            }

            bool anuladoAhora = !activo;
            Type tipoAnulado = _filaOcActual.Table.Columns["anulado"]?.DataType ?? typeof(int);
            _filaOcActual["anulado"] = tipoAnulado == typeof(bool)
                ? (object)anuladoAhora
                : (anuladoAhora ? 1 : 0);
            gridOrdenes.Refresh();

            btnEditar.Enabled = !anuladoAhora;
        }

        private async Task<bool> CargarDetalleOcAsync(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                _lineas.Clear();
                ProyectarLineasEnGrid();
                return false;
            }

            if (!OrdenesCompraNumero.EsValido(numero))
            {
                ServiceErrors.Report(
                    "La orden " + numero + " no tiene el formato OC-#####. Se consulta igual: "
                        + "el valor viene de la base.");
            }

            _ctsDetalle?.Cancel();
            _ctsDetalle?.Dispose();
            CancellationTokenSource cts = new();
            _ctsDetalle = cts;
            _ultimaOcDetalleConsulta = numero;

            try
            {
                DataTable detalle = await _ocService.LoadDataOrdenCompraDetalle(numero, cts.Token).ConfigureAwait(false);
                if (cts.IsCancellationRequested)
                {
                    return false;
                }

                if (_ultimaOcDetalleConsulta != numero)
                {
                    return false;
                }

                void AplicarDetalle()
                {
                    _lineas.Clear();
                    _lineas.AddRange(OrdenesCompraDetalleMapper.Mapear(detalle));
                    ProyectarLineasEnGrid();
                }

                if (uiDataGridView1.InvokeRequired)
                {
                    uiDataGridView1.BeginInvoke((Action)AplicarDetalle);
                }
                else
                {
                    AplicarDetalle();
                }

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el detalle de la orden de compra: " + ex.Message);
                return false;
            }
            finally
            {
                if (ReferenceEquals(_ctsDetalle, cts))
                {
                    _ctsDetalle = null;
                }
            }
        }

        private void ProyectarLineasEnGrid()
        {
            uiDataGridView1.Rows.Clear();
            foreach (OrdenCompraDetalle linea in _lineas)
            {
                int indice = uiDataGridView1.Rows.Add(
                    linea.Product_id ?? string.Empty,
                    linea.Product_name ?? string.Empty,
                    linea.Unidad ?? string.Empty,
                    linea.Cant.ToString("N2"),
                    linea.Notas ?? string.Empty,
                    linea.Precio.HasValue ? linea.Precio.Value.ToString("N2") : string.Empty,
                    linea.Width.ToString("N2"),
                    linea.Lenght.ToString("N2"),
                    linea.Total_Renglon.HasValue ? linea.Total_Renglon.Value.ToString("N2") : string.Empty);
                uiDataGridView1.Rows[indice].Tag = linea;
            }

            uiDataGridView1.ClearSelection();
            ActualizarTotalCantidad();
        }

        private OrdenCompraDetalle? LineaSeleccionada()
        {
            return uiDataGridView1.CurrentRow?.Tag as OrdenCompraDetalle;
        }

        private static string FormatoDinero(object? value)
        {
            return value != null
                && decimal.TryParse(value.ToString(), out decimal monto)
                ? monto.ToString("N2")
                : string.Empty;
        }

        private static object? Safe(DataRowView drv, string column)
        {
            DataTable? tabla = drv.DataView?.Table;
            return tabla != null && tabla.Columns.Contains(column) ? drv[column] : null;
        }

        private static object? Safe(DataRow dr, string column)
        {
            DataTable? tabla = dr.Table;
            return tabla != null && tabla.Columns.Contains(column) ? dr[column] : null;
        }

        private void ActualizarTotalCantidad()
        {
            txt_total_cantidad.Text = OrdenesCompraCalculos.TotalCantidad(_lineas).ToString("N2");
        }

        private void ConfigurarFechas()
        {
            const string formato = "dd/MM/yyyy HH:mm";

            uiDatetimePicker1.DateFormat = formato;
            uiDatetimePicker2.DateFormat = formato;
        }

        private static void ConfigurarComboFiltroIncremental(UIComboBox combo)
        {
            combo.DropDownStyle = UIDropDownStyle.DropDown;
            combo.ShowFilter = true;
            combo.FilterIgnoreCase = true;
            combo.TrimFilter = true;
            combo.FilterMaxCount = 5000;
        }

        private void ConfigurarSincronizacionValores()
        {
            foreach (UIComboBox combo in new[] { cbo_proveedores, uiComboBox2 })
            {
                combo.SelectedValueChanged += (_, _) =>
                {
                    _valoresSel[combo] = combo.SelectedValue;

                    if (ReferenceEquals(combo, uiComboBox2)
                        && combo.SelectedValue is not null
                        && combo.SelectedValue != DBNull.Value)
                    {
                        _productoLineaNoVisible = null;
                    }
                };
            }
        }

        private async void CboProveedores_ValueChanged(object? sender, EventArgs e)
        {
            MostrarConsecutivo(cbo_proveedores, txt_id_proveedor);

            if (ValorCombo(cbo_proveedores) is not object valorProveedor
                || string.IsNullOrWhiteSpace(valorProveedor?.ToString()))
            {
                return;
            }

            string proveedorId = valorProveedor?.ToString() ?? string.Empty;

            if (!EsEditable)
            {
                return;
            }

            ProveedorDatos? datos = await _ocService.BuscarProveedorAsync(proveedorId).ConfigureAwait(true);
            if (datos is null)
            {
                return;
            }

            uiRichTextBox1.Text = datos.DireccionFacturacion;
            uiRichTextBox2.Text = datos.DireccionEntrega;
        }

        private void AplicarEditabilidadMedidas()
        {
            if (!EsEditable)
            {
                txt_width.ReadOnly = true;
                txt_length.ReadOnly = true;
                return;
            }

            txt_width.ReadOnly = false;
            txt_length.ReadOnly = false;
        }

        private static void AsignarCombo(UIComboBox combo, object? valor, Dictionary<UIComboBox, object?> registro)
        {
            registro[combo] = valor == null || valor == DBNull.Value ? null : valor;

            if (valor == null || valor == DBNull.Value)
            {
                combo.Text = string.Empty;
                return;
            }

            DataTable? tabla = TablaDelCombo(combo);
            if (tabla != null)
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

        private static DataTable? TablaDelCombo(UIComboBox combo)
        {
            return combo.DataSource switch
            {
                DataTable tabla => tabla,
                DataView vista => vista.Table,
                _ => null
            };
        }

        private static DataRow? FilaDelCombo(UIComboBox combo)
        {
            return ConsecutivoProveedor.BuscarFila(TablaDelCombo(combo), combo.DisplayMember, combo.Text);
        }

        private static DataRow? FilaPorValor(UIComboBox combo, object? valor)
        {
            return ConsecutivoProveedor.BuscarFila(TablaDelCombo(combo), combo.ValueMember, valor);
        }

        private static void MostrarConsecutivo(UIComboBox combo, UITextBox destino, object? idBuscado = null)
        {
            destino.Text = idBuscado is null
                ? ConsecutivoProveedor.Formatear(FilaDelCombo(combo))
                : ConsecutivoProveedor.FormatearPorLlave(TablaDelCombo(combo), combo.ValueMember, idBuscado);
        }

        private object? ValorCombo(UIComboBox combo)
        {
            return _valoresSel.TryGetValue(combo, out object? valor) ? valor : combo.SelectedValue;
        }

        private async Task CargarCombosAsync()
        {
            try
            {
                DataTable proveedores = await _ocService.LoadDataProveedores();
                _dtProveedores = proveedores;
                cbo_proveedores.DataSource = proveedores;
                cbo_proveedores.DisplayMember = "Proveedor_Name";
                cbo_proveedores.ValueMember = "Proveedor_Id";

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
                ServiceErrors.Report("Error al cargar los combos de Órdenes de Compra: " + ex.Message);
            }
        }

        private void AgregarLinea()
        {
            if (!LeerValoresEditor(out DataRow? producto, out decimal cant, out decimal? precio))
            {
                return;
            }

            OrdenCompraDetalle linea = new();
            CopiarValoresEditor(linea, producto, cant, precio);
            _lineas.Add(linea);

            ProyectarLineasEnGrid();
            ActualizarTotalesEnPantalla();
            LimpiarEditorLinea();
        }

        private bool LeerValoresEditor([NotNullWhen(true)] out DataRow? producto, out decimal cant, out decimal? precio)
        {
            cant = 0m;
            precio = null;
            producto = null;
            if (!TryGetProductoEditor(out DataRow? seleccionado))
            {
                return false;
            }

            producto = seleccionado;

            if (!decimal.TryParse(uiTextBox3.Text?.Trim(), out cant) || cant <= 0m)
            {
                MessageBox.Show("La cantidad debe ser un número mayor que cero.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cant = 0m;
                return false;
            }

            precio = decimal.TryParse(uiTextBox8.Text?.Trim(), out decimal p) ? p : null;
            if (precio.HasValue && precio.Value < 0m)
            {
                MessageBox.Show("El precio no puede ser negativo.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        private DataRow? FilaProductoEditor()
        {
            if (_dtProductos == null) return null;

            object? productIdRaw = ValorCombo(uiComboBox2);
            if (productIdRaw == null || string.IsNullOrEmpty(productIdRaw.ToString()))
            {
                return null;
            }

            DataRow[] filas = _dtProductos.Select($"product_id = '{EscapeLike(productIdRaw.ToString() ?? string.Empty)}'");
            return filas.Length == 0 ? null : filas[0];
        }

        private void CopiarValoresEditor(OrdenCompraDetalle linea, DataRow producto, decimal cant, decimal? precio)
        {
            linea.Product_id = producto["product_id"]?.ToString();
            linea.Product_name = producto["product_name"]?.ToString();
            linea.Unidad = ObtenerUnidad(producto["tipo"]?.ToString());
            linea.Cant = cant;
            linea.Precio = precio;
            PedidoMedidas.LeerMedida(txt_width.Text, out decimal ancho);
            PedidoMedidas.LeerMedida(txt_length.Text, out decimal largo);
            linea.Width = ancho;
            linea.Lenght = largo;
            linea.Total_Renglon = OrdenesCompraCalculos.TotalRenglon(cant, precio);
            linea.Notas = uiTextBox9.Text?.Trim();
        }

        private static string ObtenerUnidad(string? tipoProducto)
        {
            if (string.IsNullOrWhiteSpace(tipoProducto))
            {
                return string.Empty;
            }

            string tipo = tipoProducto.Trim();
            if (tipo.Equals("Master", StringComparison.OrdinalIgnoreCase)
                || tipo.Equals("Rollo Cortado", StringComparison.OrdinalIgnoreCase))
            {
                return "rollo";
            }

            if (tipo.Equals("Hoja", StringComparison.OrdinalIgnoreCase))
            {
                return "resmas";
            }

            if (tipo.Equals("Graphics", StringComparison.OrdinalIgnoreCase))
            {
                return "x unidad";
            }

            return tipo;
        }

        private void CargarLineaEnEditor(OrdenCompraDetalle linea)
        {
            AsignarCombo(uiComboBox2, linea.Product_id, _valoresSel);
            _productoLineaNoVisible = !string.IsNullOrWhiteSpace(linea.Product_id)
                && string.IsNullOrWhiteSpace(uiComboBox2.Text)
                ? linea.Product_id
                : null;
            uiTextBox3.Text = linea.Cant.ToString("N2");
            uiTextBox8.Text = linea.Precio.HasValue ? linea.Precio.Value.ToString("N2") : string.Empty;
            uiTextBox9.Text = linea.Notas ?? string.Empty;
            txt_width.Text = linea.Width > 0m ? linea.Width.ToString("N2") : string.Empty;
            txt_length.Text = linea.Lenght > 0m ? linea.Lenght.ToString("N2") : string.Empty;
        }

        private void CargarLineaSeleccionadaEnEditor()
        {
            if (!EsEditable)
            {
                return;
            }

            OrdenCompraDetalle? linea = LineaSeleccionada();
            if (linea != null)
            {
                CargarLineaEnEditor(linea);
            }
        }

        private void EditarLinea()
        {
            OrdenCompraDetalle? linea = LineaSeleccionada();
            if (linea == null)
            {
                MessageBox.Show("Seleccione la línea que desea editar.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (!LeerValoresEditor(out DataRow? producto, out decimal cant, out decimal? precio))
            {
                return;
            }

            CopiarValoresEditor(linea, producto, cant, precio);

            ProyectarLineasEnGrid();
            ActualizarTotalesEnPantalla();
            LimpiarEditorLinea();
        }

        private void EliminarLinea()
        {
            OrdenCompraDetalle? linea = LineaSeleccionada();
            if (linea == null)
            {
                MessageBox.Show("Seleccione la línea que desea eliminar.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Eliminar la línea " + (linea.Product_name ?? string.Empty) + " del borrador?",
                TituloModo(),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            _lineas.Remove(linea);
            ProyectarLineasEnGrid();
            ActualizarTotalesEnPantalla();
            LimpiarEditorLinea();
        }

        private void LimpiarEditorLinea()
        {
            AsignarCombo(uiComboBox2, null, _valoresSel);
            _productoLineaNoVisible = null;
            uiTextBox3.Clear();
            uiTextBox8.Clear();
            uiTextBox9.Clear();
            txt_width.Clear();
            txt_length.Clear();
        }

        private bool TryGetProductoEditor([NotNullWhen(true)] out DataRow? producto)
        {
            producto = null;
            if (_dtProductos == null)
            {
                return false;
            }

            object? productIdRaw = ValorCombo(uiComboBox2);
            if (productIdRaw == null || string.IsNullOrEmpty(productIdRaw.ToString()))
            {
                if (!string.IsNullOrWhiteSpace(_productoLineaNoVisible))
                {
                    DataRow[] porLinea = _dtProductos.Select($"product_id = '{EscapeLike(_productoLineaNoVisible)}'");
                    if (porLinea.Length > 0)
                    {
                        producto = porLinea[0];
                        return true;
                    }

                    MessageBox.Show("El producto de esta línea no existe en el catálogo.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return false;
                }

                MessageBox.Show("Seleccione un producto.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataRow[] filas = _dtProductos.Select($"product_id = '{EscapeLike(productIdRaw.ToString() ?? string.Empty)}'");
            if (filas.Length == 0)
            {
                MessageBox.Show("El producto seleccionado no existe en el catálogo.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            producto = filas[0];
            return true;
        }

        private decimal PorcItbisActual()
        {
            return decimal.TryParse(uiTextBox7.Text?.Trim(), out decimal porcentaje) && porcentaje >= 0m
                ? porcentaje
                : 18m;
        }

        private void ActualizarTotalesEnPantalla()
        {
            decimal porcItbis = PorcItbisActual();
            (decimal subTotal, decimal montoItbis, decimal total) = OrdenesCompraCalculos.Calcular(_lineas, porcItbis);
            uiTextBox4.Text = subTotal.ToString("N2");
            uiTextBox5.Text = montoItbis.ToString("N2");
            uiTextBox6.Text = total.ToString("N2");
        }

        private string TituloModo() => _modo == ModoFormulario.Editar ? "Edición de orden" : "Orden nueva";

        private async void BtnNuevo_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeCrear("OrdenesCompra"))
            {
                MessageBox.Show("No tiene permiso para crear órdenes de compra.", "Órdenes de Compra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                string numeroPrevisto = await _ocService.GetProximoNumeroOrdenCompra();
                LimpiarGeneral();
                _lineas.Clear();
                ProyectarLineasEnGrid();

                uiTextBox1.Text = numeroPrevisto;
                uiDatetimePicker1.Value = DateTime.Now;
                uiDatetimePicker2.Value = DateTime.Now;
                uiTextBox2.Text = OrdenCompraEstado.Creado;
                uiTextBox7.Text = "18";
                AplicarModo(ModoFormulario.Nuevo);
                ActualizarTotalesEnPantalla();
                cbo_proveedores.Focus();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al iniciar la orden nueva: " + ex.Message);
            }
        }

        private async void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeEditar("OrdenesCompra"))
            {
                MessageBox.Show("No tiene permiso para editar órdenes de compra.", "Órdenes de Compra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_filaOcActual is null)
            {
                MessageBox.Show("Seleccione la orden que desea editar.", "Órdenes de Compra", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (EsAnulado(_filaOcActual))
            {
                MessageBox.Show("No se puede editar una orden anulada.", "Órdenes de Compra", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string numero = _filaOcActual["numero"]?.ToString() ?? string.Empty;

            bool detalleCargado = await CargarDetalleOcAsync(numero);

            if (_filaOcActual is null
                || !string.Equals(
                    _filaOcActual["numero"]?.ToString(),
                    numero,
                    StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "La selección de la lista cambió mientras se cargaba el detalle de "
                        + numero + ". No se entró en edición: pulse Editar de nuevo sobre "
                        + "la orden que desea editar.",
                    "Órdenes de Compra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!detalleCargado)
            {
                MessageBox.Show(
                    "No se pudo cargar el detalle de la orden " + numero
                        + ". No se entró en edición; intente de nuevo.",
                    "Órdenes de Compra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            AplicarModo(ModoFormulario.Editar);
            ActualizarTotalesEnPantalla();
            cbo_proveedores.Focus();
        }

        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!EsEditable)
            {
                return;
            }

            bool esNuevo = _modo == ModoFormulario.Nuevo;
            bool permitido = esNuevo
                ? PermisoHelper.PuedeCrear("OrdenesCompra")
                : PermisoHelper.PuedeEditar("OrdenesCompra");
            if (!permitido)
            {
                MessageBox.Show(
                    esNuevo ? "No tiene permiso para crear órdenes de compra." : "No tiene permiso para editar órdenes de compra.",
                    "Órdenes de Compra",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            OrdenCompra orden = ConstruirOrdenDesdeFormulario(uiDatetimePicker1.Value);
            if (!OrdenesCompraValidador.EsValido(orden, out string error))
            {
                MessageBox.Show(error, TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string numeroPrevisualizado = uiTextBox1.Text?.Trim() ?? string.Empty;

            bool ok = esNuevo
                ? _ocService.SaveOrdenCompraCompleto(orden)
                : _ocService.ActualizarOrdenCompraCompleto(orden);

            if (!ok)
            {
                MessageBox.Show("No se pudo guardar la orden: " + _ocService.ErrorMsg, TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(ConfirmacionGuardado(orden.Numero, numeroPrevisualizado), TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarGeneral();
            _lineas.Clear();
            ProyectarLineasEnGrid();
            AplicarModo(ModoFormulario.Consulta);

            await RecargarListadoAsync();
            SeleccionarFilaPorNumero(orden.Numero);
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            string mensaje = _modo == ModoFormulario.Editar
                ? "¿Descartar los cambios hechos a la orden " + uiTextBox1.Text + "?"
                : "¿Descartar la orden en borrador?";
            DialogResult confirmacion = MessageBox.Show(
                mensaje,
                TituloModo(),
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);
            if (confirmacion != DialogResult.Yes)
            {
                return;
            }

            LimpiarGeneral();
            _lineas.Clear();
            ProyectarLineasEnGrid();
            AplicarModo(ModoFormulario.Consulta);
            _ = RecargarListadoAsync();
        }

        private async Task RecargarListadoAsync()
        {
            try
            {
                _dtOrdenes = await _ocService.LoadDataOrdenesCompra();
                gridOrdenes.DataSource = _dtOrdenes;
                gridOrdenes.ClearSelection();
                gridOrdenes.CurrentCell = null;
                ActualizarResumen();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al recargar las órdenes de compra: " + ex.Message);
            }
        }

        private void SeleccionarFilaPorNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return;
            }

            foreach (DataGridViewRow fila in gridOrdenes.Rows)
            {
                if (!string.Equals(
                        fila.Cells["numero"]?.Value?.ToString(),
                        numero,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                gridOrdenes.ClearSelection();
                fila.Selected = true;
                if (fila.Cells["numero"] is DataGridViewCell celda && celda.Visible)
                {
                    gridOrdenes.CurrentCell = celda;
                }
                return;
            }
        }

        private static string? ComboOpcional(UIComboBox combo)
        {
            return combo.SelectedIndex < 0 ? null : combo.SelectedItem?.ToString();
        }

        private static string? ValorFila(DataRow? fila, string columna)
        {
            object? valor = fila == null ? null : Safe(fila, columna);
            return valor == null || valor == DBNull.Value ? null : valor.ToString();
        }

        private static string? NombreFilaPorValor(UIComboBox combo, object? valor)
        {
            DataRow? fila = FilaPorValor(combo, valor);
            return fila != null && fila.Table.Columns.Contains(combo.DisplayMember)
                ? ValorFila(fila, combo.DisplayMember)
                : null;
        }

        private static string? TextoOpcional(UITextBox texto) => TextoOpcional(texto.Text);

        private static string? TextoOpcional(UIRichTextBox texto) => TextoOpcional(texto.Text);

        private static string? TextoOpcional(string? contenido)
        {
            string? valor = contenido?.Trim();
            return string.IsNullOrEmpty(valor) ? null : valor;
        }

        private static void AsignarComboOpcional(UIComboBox combo, IReadOnlyList<string> valores, object? valor)
        {
            combo.SelectedIndex = -1;

            string? texto = valor?.ToString();
            if (string.IsNullOrEmpty(texto))
            {
                return;
            }

            for (int i = 0; i < valores.Count; i++)
            {
                if (string.Equals(valores[i], texto, StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        private static string ConfirmacionGuardado(string numeroAsignado, string numeroPrevisualizado)
        {
            string baseTexto = "Orden " + numeroAsignado + " guardada.";

            return !OrdenesCompraNumero.EsValido(numeroPrevisualizado)
                || string.Equals(numeroAsignado, numeroPrevisualizado, StringComparison.Ordinal)
                ? baseTexto
                : baseTexto + Environment.NewLine + Environment.NewLine
                    + "Se previsualizó como " + numeroPrevisualizado
                    + " porque alguien más registró una orden mientras usted llenaba esta. Quedó guardada como "
                    + numeroAsignado + ".";
        }

        private OrdenCompra ConstruirOrdenDesdeFormulario(DateTime fecha)
        {
            bool editando = _modo == ModoFormulario.Editar;

            OrdenCompra orden = new()
            {
                Numero = editando ? uiTextBox1.Text?.Trim() ?? string.Empty : string.Empty,
                Fecha = fecha,
                Fecha_entrega = editando
                        && _fechaEntregaCargadaNula
                        && uiDatetimePicker2.Value == _fechaEntregaMostrada
                    ? null
                    : uiDatetimePicker2.Value.Date,
                Estado = string.IsNullOrWhiteSpace(uiTextBox2.Text) ? OrdenCompraEstado.Creado : uiTextBox2.Text.Trim(),
                Direccion_entrega = TextoOpcional(uiRichTextBox2),
                Direccion_facturacion = TextoOpcional(uiRichTextBox1),
                Notas = uiRichTextBox3.Text?.Trim(),
                Anulado = editando && _filaOcActual != null && EsAnulado(_filaOcActual),
                Porc_Itbis = PorcItbisActual(),
                Condiciones_pago = ComboOpcional(cboCondicionesPago)
                    ?? (editando ? ValorFila(_filaOcActual, "condiciones_pago") : null),
                Prioridad = ComboOpcional(cbo_prioridad)
                    ?? (editando ? ValorFila(_filaOcActual, "prioridad") : null),
                Persona_Contacto = TextoOpcional(txtPersonaContacto),
                Detalle = new List<OrdenCompraDetalle>(_lineas)
            };

            object? valorProveedor = ValorCombo(cbo_proveedores);
            if (valorProveedor is not null && !string.IsNullOrEmpty(valorProveedor.ToString()))
            {
                string proveedorId = valorProveedor.ToString() ?? string.Empty;
                orden.Proveedor_Id = proveedorId;
                orden.Proveedor_Name = NombreFilaPorValor(cbo_proveedores, proveedorId)
                    ?? (editando ? ValorFila(_filaOcActual, "proveedor_name") : null)
                    ?? TextoOpcional(cbo_proveedores.Text);
            }
            else if (editando && ValorFila(_filaOcActual, "proveedor_id") is string proveedorFila && !string.IsNullOrEmpty(proveedorFila))
            {
                orden.Proveedor_Id = proveedorFila;
                orden.Proveedor_Name = ValorFila(_filaOcActual, "proveedor_name")
                    ?? TextoOpcional(cbo_proveedores.Text);
            }

            (decimal subTotal, decimal montoItbis, decimal total) = OrdenesCompraCalculos.Calcular(orden.Detalle, orden.Porc_Itbis);
            orden.SubTotal = subTotal;
            orden.Monto_Itbis = montoItbis;
            orden.Total = total;

            return orden;
        }

        private void LimpiarGeneral()
        {
            AsignarCombo(cbo_proveedores, null, _valoresSel);
            uiTextBox1.Clear();
            uiDatetimePicker1.Value = DateTime.Now;
            uiDatetimePicker2.Value = DateTime.Now;
            uiTextBox2.Clear();
            uiRichTextBox1.Clear();
            uiRichTextBox2.Clear();
            uiRichTextBox3.Clear();
            cboCondicionesPago.SelectedIndex = -1;
            cbo_prioridad.SelectedIndex = -1;
            txtPersonaContacto.Clear();
            uiTextBox4.Clear();
            uiTextBox5.Clear();
            uiTextBox6.Clear();
            uiTextBox7.Clear();
            txt_id_proveedor.Clear();

            _filaOcActual = null;
            btnEditar.Enabled = false;
            _actualizandoSwitchAnulado = true;
            sw_anular_oc.Active = true;
            _actualizandoSwitchAnulado = false;
            sw_anular_oc.Enabled = false;

            LimpiarEditorLinea();
        }

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

        private static string EscapeLike(string value)
        {
            return value.Replace("'", "''");
        }
    }
}
