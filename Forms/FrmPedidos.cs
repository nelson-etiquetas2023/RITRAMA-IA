using System.Data;
using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms.Buscadores;
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
    /// Formulario de Pedidos de Cliente: listado, búsqueda en vivo por numero/cliente/estado
    /// y contador de pedidos visibles.
    /// </summary>
    public partial class FrmPedidos : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IPedidoService _pedidoService;
        private readonly IProductsService _productsService;
        private DataTable? _dtPedidos;
        private DataTable? _dtProductos;
        private CancellationTokenSource? _ctsDetalle;
        private string? _ultimoPedidoDetalleConsulta;
        private readonly Dictionary<UIComboBox, object?> _valoresSel = new();

        /// <summary>
        /// Fuente de verdad del detalle: las lineas del pedido que se esta editando. El grid es
        /// una proyeccion de esta lista, nunca su almacen.
        /// </summary>
        private readonly List<PedidoDetalle> _lineas = new();

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

            // Vocabularios cerrados de tipo de venta y condiciones de pago. Se cargan aqui y no
            // en CargarCombosAsync porque no vienen de la base: son catálogos fijos del negocio.
            // SelectedIndex = -1 deja el combo sin seleccion, que es el estado "no informado".
            cboTipoVenta.DataSource = PedidoCatalogos.TipoVenta;
            cboCondicionesPago.DataSource = PedidoCatalogos.CondicionesPago;
            cbo_prioridad.DataSource = PedidoCatalogos.Prioridad;
            cboTipoVenta.SelectedIndex = -1;
            cboCondicionesPago.SelectedIndex = -1;
            cbo_prioridad.SelectedIndex = -1;

            // Eventos de selección para campos informativos.
            cbo_customers.SelectedValueChanged += CboCustomers_ValueChanged;
            uiComboBox1.SelectedValueChanged += UiComboBox1_Vendedor_ValueChanged;

            // El tipo de venta gobierna si la condicion de pago se puede elegir: al contado no
            // hay plazo que elegir. Se engancha al combo y no solo a la entrada en modo Nuevo,
            // para que cambiar el tipo de venta en la pantalla/aprete y para que un pedido
            // guardado que venga al contado se muestre igual.
            cboTipoVenta.SelectedIndexChanged += CboTipoVenta_SelectedIndexChanged;

            // Búsqueda en vivo sin recargar la BD: filtra el DataTable ya cargado.
            txtBuscarPedido.TextChanged += TxtBuscarPedido_TextChanged;
            btnLimpiarBusqueda.Click += BtnLimpiarBusqueda_Click;

            // Acciones sobre las lineas de producto del detalle.
            btnAddProducto.Click += (_, _) => AgregarLinea();
            btnEditarProducto.Click += (_, _) => EditarLinea();
            btnEliminarProducto.Click += (_, _) => EliminarLinea();
            btnBuscarProducto.Click += BtnBuscarProducto_Click;
            uiTextBox7.TextChanged += (_, _) => ActualizarTotalesEnPantalla();

            // Barra: Nuevo abre el borrador; Guardar y Cancelar solo existen en ese modo.
            btnNuevo.Click += BtnNuevo_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            uiDataGridView1.SelectionChanged += (_, _) => CargarLineaSeleccionadaEnEditor();

            // La pantalla arranca y termina en solo lectura: Consulta es el estado por defecto.
            AplicarModo(ModoFormulario.Consulta);
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

        /// <summary>
        /// Estado del formulario. Consulta es el estado por defecto: la pestaña General es de
        /// solo lectura y solo se habilita al entrar explicitamente a Nuevo.
        /// </summary>
        private enum ModoFormulario
        {
            Consulta,
            Nuevo
        }

        private ModoFormulario _modo = ModoFormulario.Consulta;

        /// <summary>
        /// Unico metodo que cambia ReadOnly o Enabled. Al volver a Consulta deja toda la
        /// pestaña General en solo lectura.
        /// </summary>
        private void AplicarModo(ModoFormulario modo)
        {
            _modo = modo;
            bool esNuevo = modo == ModoFormulario.Nuevo;

            // Encabezado siempre de solo lectura.
            uiTextBox1.ReadOnly = true;
            uiTextBox2.ReadOnly = true;
            uiRichTextBox2.ReadOnly = true;
            uiTextBox4.ReadOnly = true;
            uiTextBox5.ReadOnly = true;
            uiTextBox6.ReadOnly = true;

            // Editables solo en modo Nuevo (textos y areas de texto: ReadOnly ya impide escribir).
            // Fecha Reg es editable: hay pedidos que se registran con fecha anterior a la del
            // sistema, y el dato se guarda tal como lo pone el usuario.
            uiDatetimePicker1.ReadOnly = !esNuevo;
            uiDatetimePicker2.ReadOnly = !esNuevo;
            uiRichTextBox1.ReadOnly = !esNuevo;
            uiRichTextBox3.ReadOnly = !esNuevo;
            uiTextBox7.ReadOnly = !esNuevo;
            uiTextBox3.ReadOnly = !esNuevo;
            uiTextBox8.ReadOnly = !esNuevo;
            uiTextBox9.ReadOnly = !esNuevo;

            // Combos y fechas: ReadOnly solo impide escribir. Verificado en el IL de SunnyUI
            // 3.9.8 que UIDropControl.ReadOnly solo escribe en el TextBoxBase interno
            // (edit.ReadOnly), mientras UIComboBox.ListBox_Click y Edit_KeyDown siguen
            // cambiando el valor, y el desplegable se sigue abriendo con el raton. Por eso en
            // solo lectura se deshabilitan: Enabled = false no entrega ni teclado ni raton.
            uiDatetimePicker1.Enabled = esNuevo;
            uiDatetimePicker2.Enabled = esNuevo;
            cbo_customers.ReadOnly = !esNuevo;
            cbo_customers.Enabled = esNuevo;
            uiComboBox1.ReadOnly = !esNuevo;
            uiComboBox1.Enabled = esNuevo;
            uiComboBox2.ReadOnly = !esNuevo;
            uiComboBox2.Enabled = esNuevo;

            // Informativos: se muestran pero el usuario no los edita en ningun modo.
            txt_id_cust.ReadOnly = true;
            txt_id_vendor.ReadOnly = true;

            // Terminos del pedido. Los combos se ven en los dos modos para poder consultar un
            // pedido ya guardado, pero solo en Nuevo se editan. El contacto tambien se ve al
            // consultar: antes vivia concatenado dentro de Ship To y, al separar las direcciones,
            // se hubiera quedado invisible sin su propio campo.
            cboTipoVenta.ReadOnly = !esNuevo;
            cboTipoVenta.Enabled = esNuevo;
            AplicarEditabilidadCondicionesPago();
            cbo_prioridad.ReadOnly = !esNuevo;
            cbo_prioridad.Enabled = esNuevo;
            txtPersonaContacto.ReadOnly = !esNuevo;
            txtPersonaContacto.Visible = true;

            // Las dos direcciones son editables solo al crear. Ship To antes era de solo lectura
            // siempre porque era un bloque informativo; ahora es un dato mas del pedido.
            uiRichTextBox1.ReadOnly = !esNuevo;
            uiRichTextBox2.ReadOnly = !esNuevo;

            btnAddProducto.Visible = esNuevo;
            btnEditarProducto.Visible = esNuevo;
            btnEliminarProducto.Visible = esNuevo;
            btnBuscarProducto.Visible = esNuevo;
            btnAddProducto.Enabled = esNuevo;
            btnEditarProducto.Enabled = esNuevo;
            btnEliminarProducto.Enabled = esNuevo;
            btnBuscarProducto.Enabled = esNuevo;

            // Barra de herramientas. btnEditar sigue deshabilitado: el modo Editar llega en
            // el plan siguiente.
            btnNuevo.Visible = !esNuevo;
            btnGuardar.Visible = esNuevo;
            btnCancelar.Visible = esNuevo;

            gridPedidos.Enabled = !esNuevo;
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

            foreach (DataGridViewColumn columna in gridPedidos.Columns)
            {
                columna.FillWeight = columna.Name switch
                {
                    "customer_name" => 60,
                    _ => 20
                };
            }
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
            // Mientras se edita un borrador, elegir otro pedido del listado no debe pisar la
            // pantalla: el borrador se descarta primero.
            if (_modo != ModoFormulario.Consulta)
            {
                return;
            }

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

            // El consecutivo se busca por el id del propio pedido, no por lo que quedo elegido
            // en pantalla. AsignarCombo dispara SelectedValueChanged, asi que el handler ya
            // corrio para cuando se llega aca: si el campo se llenara desde el, el orden de las
            // lineas decidiria que queda escrito.
            MostrarConsecutivo(cbo_customers, txt_id_cust, Safe(drv, "customer_id"));
            MostrarConsecutivo(uiComboBox1, txt_id_vendor, Safe(drv, "vendor_id"));

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
            // Bill To = facturacion, Ship To = entrega. Cada una congelada en el pedido.
            uiRichTextBox1.Text = Safe(drv, "direccion_facturacion")?.ToString() ?? string.Empty;
            uiRichTextBox2.Text = Safe(drv, "direccion_entrega")?.ToString() ?? string.Empty;

            uiRichTextBox3.Text = Safe(drv, "notas")?.ToString() ?? string.Empty;

            // Los terminos no tenian donde mostrarse: quedaban invisibles al revisar un pedido.
            AsignarComboOpcional(cboTipoVenta, PedidoCatalogos.TipoVenta, Safe(drv, "tipo_venta"));
            AsignarComboOpcional(cboCondicionesPago, PedidoCatalogos.CondicionesPago, Safe(drv, "condiciones_pago"));
            // La columna se agrego despues que los pedidos existentes, asi que arrives con
            // null en pedidos viejos: se deja el combo vacio y se trata como "normal".
            AsignarComboOpcional(cbo_prioridad, PedidoCatalogos.Prioridad, Safe(drv, "prioridad"));
            txtPersonaContacto.Text = Safe(drv, "persona_contacto")?.ToString() ?? string.Empty;

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
            if (string.IsNullOrWhiteSpace(numero))
            {
                _lineas.Clear();
                uiDataGridView1.Rows.Clear();
                return;
            }

            // El formato no se exige para cargar. El numero viene de la fila que se esta
            // consultando, o sea que la base ya lo entrego como clave de ese pedido, y la
            // consulta lo manda como parametro. Exigir SO-#### solo servia para que un numero
            // guardado con otra cantidad de digitos dejara el grid vacio en silencio: sin error,
            // sin aviso, y con las lineas del pedido invisible para el usuario. Se reporta para
            // que quede registrado, pero se carga igual.
            if (!PedidoNumero.EsValido(numero))
            {
                ServiceErrors.Report(
                    "El pedido " + numero + " no tiene el formato SO-####. Se consulta igual: "
                        + "el valor viene de la base.");
            }

            _ctsDetalle?.Cancel();
            _ctsDetalle?.Dispose();
            CancellationTokenSource cts = new();
            _ctsDetalle = cts;
            _ultimoPedidoDetalleConsulta = numero;

            try
            {
                DataTable detalle = await _pedidoService.LoadDataPedidoDetalle(numero, cts.Token).ConfigureAwait(false);
                if (cts.IsCancellationRequested)
                {
                    return;
                }

                if (_ultimoPedidoDetalleConsulta != numero)
                {
                    return;
                }

                // _lineas se modifica y se proyecta en el hilo de la interfaz: la consulta
                // vuelve de un hilo de fondo y la lista es la fuente de verdad compartida.
                void AplicarDetalle()
                {
                    _lineas.Clear();
                    _lineas.AddRange(PedidoDetalleMapper.Mapear(detalle));
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

        /// <summary>
        /// Proyecta las lineas del pedido sobre el grid. Cada fila guarda su linea en Tag, de
        /// modo que agregar, editar y eliminar no dependan del indice visible de la fila.
        /// </summary>
        private void ProyectarLineasEnGrid()
        {
            uiDataGridView1.Rows.Clear();
            int renglon = 0;
            foreach (PedidoDetalle linea in _lineas)
            {
                renglon++;
                int indice = uiDataGridView1.Rows.Add(
                    renglon,
                    linea.Product_name ?? string.Empty,
                    linea.Unidad ?? string.Empty,
                    linea.Cant.ToString("N2"),
                    linea.Notas ?? string.Empty,
                    linea.Precio.HasValue ? linea.Precio.Value.ToString("N2") : string.Empty,
                    linea.Total_Renglon.HasValue ? linea.Total_Renglon.Value.ToString("N2") : string.Empty);
                uiDataGridView1.Rows[indice].Tag = linea;
            }

            uiDataGridView1.ClearSelection();
        }

        /// <summary>
        /// Devuelve la linea seleccionada en el grid de detalle, o null si no hay seleccion.
        /// </summary>
        private PedidoDetalle? LineaSeleccionada()
        {
            return uiDataGridView1.CurrentRow?.Tag as PedidoDetalle;
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
            DataTable? tabla = drv.DataView?.Table;
            return tabla != null && tabla.Columns.Contains(column) ? drv[column] : null;
        }

        /// <summary>
        /// Lee una columna de un DataRow devolviendo null si no existe en el esquema.
        /// </summary>
        private static object? Safe(DataRow dr, string column)
        {
            DataTable? tabla = dr.Table;
            return tabla != null && tabla.Columns.Contains(column) ? dr[column] : null;
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

        /// <summary>
        /// Tabla que alimenta al combo. El combo de producto se enlaza a un DataView filtrado
        /// por anulado = 0, no a la tabla, asi que se resuelve en los dos casos. Se busca sobre
        /// la tabla completa: al editar la linea de un pedido cuyo producto fue anulado, el
        /// nombre debe seguir apareciendo.
        /// </summary>
        private static DataTable? TablaDelCombo(UIComboBox combo)
        {
            return combo.DataSource switch
            {
                DataTable tabla => tabla,
                DataView vista => vista.Table,
                _ => null
            };
        }

        /// <summary>
        /// Devuelve la fila del combo que esta seleccionada, o null si no hay ninguna.
        ///
        /// Busca por el texto visible y no por la posicion. Con el filtro incremental la lista
        /// se acota mientras se escribe, asi que el SelectedIndex cuenta sobre lo filtrado y ya
        /// no es el indice de la fila en la tabla de origen: por ahi devolvia null al elegir un
        /// cliente y no se llenaban ni el id ni las direcciones.
        /// </summary>
        private static DataRow? FilaDelCombo(UIComboBox combo)
        {
            return ConsecutivoCliente.BuscarFilaPorTexto(TablaDelCombo(combo), combo.DisplayMember, combo.Text);
        }

        /// <summary>
        /// Busca la fila del combo cuyo valor de la columna ValueMember sea el indicado, sin
        /// depender de cual este seleccionado. Es lo que se usa al abrir un pedido: ahi la fila
        /// correcta se deduce del customer_id o vendor_id del propio pedido, y no de lo que
        /// haya quedado elegido en pantalla.
        /// </summary>
        private static DataRow? FilaPorValor(UIComboBox combo, object? valor)
        {
            return ConsecutivoCliente.BuscarFila(TablaDelCombo(combo), combo.ValueMember, valor);
        }

        /// <summary>
        /// Escribe en el textbox el consecutivo de la fila, con el relleno de cuatro digitos, y
        /// lo deja vacio si la fila no existe o no trae consecutivo. Es la misma operacion para
        /// cliente y para vendedor, y se usa igual al elegir en pantalla y al abrir un pedido,
        /// para que el campo no muestre un numero al elegir y un GUID al consultar.
        /// </summary>
        private static void MostrarConsecutivo(UIComboBox combo, UITextBox destino, object? idBuscado = null)
        {
            destino.Text = idBuscado is null
                ? ConsecutivoCliente.Formatear(FilaDelCombo(combo))
                : ConsecutivoCliente.FormatearPorLlave(TablaDelCombo(combo), combo.ValueMember, idBuscado);
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
        /// Al elegir cliente, muestra su consecutivo de 4 digitos y precarga la direccion de
        /// entrega con la del cliente. Solo precarga en modo nuevo: en consulta la direccion de
        /// entrega viene del propio pedido y no debe tocarse.
        /// </summary>
        private async void CboCustomers_ValueChanged(object? sender, EventArgs e)
        {
            if (ValorCombo(cbo_customers) is not object valorCliente
                || !Guid.TryParse(valorCliente.ToString(), out Guid clienteId)
                || clienteId == Guid.Empty)
            {
                return;
            }

            ClienteDatos? datos = await _pedidoService.BuscarClienteAsync(clienteId).ConfigureAwait(true);
            if (datos is null)
            {
                return;
            }

            txt_id_cust.Text = datos.Consecutivo;

            if (_modo != ModoFormulario.Nuevo)
            {
                return;
            }

            // Solo en modo Nuevo: al consultar, las direcciones vienen del pedido guardado, no
            // del maestro. Si se pisaran aqui, reabrir un pedido viejo mostraria la direccion
            // que el cliente tiene hoy en vez de la que se acordo ese dia.
            uiRichTextBox1.Text = datos.DireccionFacturacion;
            uiRichTextBox2.Text = datos.DireccionEntrega;
        }


        /// <summary>
        /// Al elegir vendedor, muestra su consecutivo de 4 digitos en el campo de solo lectura.
        /// Misma operacion que en el cliente.
        /// </summary>
        private void UiComboBox1_Vendedor_ValueChanged(object? sender, EventArgs e)
        {
            MostrarConsecutivo(uiComboBox1, txt_id_vendor);
        }

        /// <summary>
        /// Al elegir el tipo de venta, recalcula si la condicion de pago se puede editar.
        /// </summary>
        private void CboTipoVenta_SelectedIndexChanged(object? sender, EventArgs e)
        {
            AplicarEditabilidadCondicionesPago();
        }

        /// <summary>
        /// Deja la condicion de pago editable o no segun la regla del catalogo, que es la unica
        /// que sabe cuando no se puede elegir. Se separa del resto de AplicarModo porque tiene
        /// dos entradas: el cambio de modo y el cambio de tipo de venta, y las dos tienen que
        /// terminar en el mismo lugar. Si se writeara solo en AplicarModo, cambiar el tipo de
        /// venta con el formulario abierto no tendria ningun efecto.
        /// </summary>
        private void AplicarEditabilidadCondicionesPago()
        {
            bool editable = PedidoCatalogos.CondicionesPagoEditable(
                ComboOpcional(cboTipoVenta), _modo == ModoFormulario.Nuevo);

            cboCondicionesPago.ReadOnly = !editable;
            cboCondicionesPago.Enabled = editable;
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
        /// Agrega al borrador la linea del producto, cantidad, precio y notas del editor.
        /// </summary>
        private void AgregarLinea()
        {
            if (!LeerValoresEditor(out DataRow? producto, out decimal cant, out decimal? precio))
            {
                return;
            }

            PedidoDetalle linea = new();
            CopiarValoresEditor(linea, producto, cant, precio);
            _lineas.Add(linea);

            ProyectarLineasEnGrid();
            ActualizarTotalesEnPantalla();
            LimpiarEditorLinea();
        }

        /// <summary>
        /// Valida los controles del editor de linea. Devuelve false, avisando al usuario, cuando
        /// falta el producto o los numeros no son utilizables.
        /// </summary>
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
                MessageBox.Show("La cantidad debe ser un número mayor que cero.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                cant = 0m;
                return false;
            }

            precio = decimal.TryParse(uiTextBox8.Text?.Trim(), out decimal p) ? p : null;
            if (precio.HasValue && precio.Value < 0m)
            {
                MessageBox.Show("El precio no puede ser negativo.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            return true;
        }

        /// <summary>
        /// Vuelca el editor sobre la linea, incluido el total del renglon. Agregar y Editar
        /// comparten esta copia para que no se separen los campos que se guardan.
        /// </summary>
        private void CopiarValoresEditor(PedidoDetalle linea, DataRow producto, decimal cant, decimal? precio)
        {
            linea.Product_id = producto["product_id"]?.ToString();
            linea.Product_name = producto["product_name"]?.ToString();
            linea.Unidad = producto["tipo"]?.ToString();
            linea.Cant = cant;
            linea.Precio = precio;
            linea.Total_Renglon = PedidoCalculos.TotalRenglon(cant, precio);
            linea.Notas = uiTextBox9.Text?.Trim();
        }

        /// <summary>
        /// Carga la linea seleccionada en los controles del editor para poder modificarla.
        /// </summary>
        private void CargarLineaEnEditor(PedidoDetalle linea)
        {
            AsignarCombo(uiComboBox2, linea.Product_id, _valoresSel);
            uiTextBox3.Text = linea.Cant.ToString("N2");
            uiTextBox8.Text = linea.Precio.HasValue ? linea.Precio.Value.ToString("N2") : string.Empty;
            uiTextBox9.Text = linea.Notas ?? string.Empty;
        }

        /// <summary>
        /// Carga en el editor la fila que el usuario acaba de seleccionar, para que Editar
        /// trabaje sobre ella. Fuera del modo Nuevo el editor esta deshabilitado y no se toca.
        /// </summary>
        private void CargarLineaSeleccionadaEnEditor()
        {
            if (_modo != ModoFormulario.Nuevo)
            {
                return;
            }

            PedidoDetalle? linea = LineaSeleccionada();
            if (linea != null)
            {
                CargarLineaEnEditor(linea);
            }
        }

        /// <summary>
        /// Reemplaza la linea seleccionada por los valores del editor.
        /// </summary>
        private void EditarLinea()
        {
            PedidoDetalle? linea = LineaSeleccionada();
            if (linea == null)
            {
                MessageBox.Show("Seleccione la linea que desea editar.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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

        /// <summary>
        /// Quita del borrador la linea seleccionada, previa confirmacion.
        /// </summary>
        private void EliminarLinea()
        {
            PedidoDetalle? linea = LineaSeleccionada();
            if (linea == null)
            {
                MessageBox.Show("Seleccione la linea que desea eliminar.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Eliminar la linea " + (linea.Product_name ?? string.Empty) + " del borrador?",
                "Pedido nuevo",
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

        /// <summary>
        /// Vacia los controles del editor de linea.
        /// </summary>
        private void LimpiarEditorLinea()
        {
            AsignarCombo(uiComboBox2, null, _valoresSel);
            uiTextBox3.Clear();
            uiTextBox8.Clear();
            uiTextBox9.Clear();
        }

        /// <summary>
        /// Devuelve el producto elegido en el combo del editor, o false si no hay ninguno.
        /// </summary>
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
                MessageBox.Show("Seleccione un producto.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            DataRow[] filas = _dtProductos.Select($"product_id = '{EscapeLike(productIdRaw.ToString() ?? string.Empty)}'");
            if (filas.Length == 0)
            {
                MessageBox.Show("El producto seleccionado no existe en el catálogo.", "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return false;
            }

            producto = filas[0];
            return true;
        }

        /// <summary>
        /// Porcentaje de ITBIS indicado en pantalla; 18 por defecto si no es un numero valido.
        /// </summary>
        private decimal PorcItbisActual()
        {
            return decimal.TryParse(uiTextBox7.Text?.Trim(), out decimal porcentaje) && porcentaje >= 0m
                ? porcentaje
                : 18m;
        }

        /// <summary>
        /// Recalcula los importes del borrador y los muestra en solo lectura.
        /// </summary>
        private void ActualizarTotalesEnPantalla()
        {
            decimal porcItbis = PorcItbisActual();
            (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(_lineas, porcItbis);
            uiTextBox4.Text = subTotal.ToString("N2");
            uiTextBox5.Text = montoItbis.ToString("N2");
            uiTextBox6.Text = total.ToString("N2");
        }

        /// <summary>
        /// Entra al modo Nuevo: consume el consecutivo, limpia la pantalla y habilita la
        /// escritura de la cabecera y las lineas.
        /// </summary>
        private async void BtnNuevo_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeCrear("Pedidos"))
            {
                MessageBox.Show("No tiene permiso para crear pedidos.", "Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                // Solo previsualiza: el contador no se toca, asi que cancelar o fallar no gastan
                // numero. El numero real se reserva en SavePedidoCompleto y puede diferir.
                string numeroPrevisto = await _pedidoService.GetProximoNumeroPedido();
                LimpiarGeneral();
                _lineas.Clear();
                ProyectarLineasEnGrid();

                uiTextBox1.Text = numeroPrevisto;
                uiDatetimePicker1.Value = DateTime.Now;
                uiDatetimePicker2.Value = DateTime.Now;
                uiTextBox2.Text = PedidoEstado.Creado;
                uiTextBox7.Text = "18";
                AplicarModo(ModoFormulario.Nuevo);
                ActualizarTotalesEnPantalla();
                cbo_customers.Focus();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al iniciar el pedido nuevo: " + ex.Message);
            }
        }

        /// <summary>
        /// Valida el borrador y lo guarda. Al terminar vuelve a solo lectura.
        /// </summary>
        private void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (_modo != ModoFormulario.Nuevo)
            {
                return;
            }

            Pedido pedido = ConstruirPedidoDesdeFormulario(uiDatetimePicker1.Value);
            if (!PedidoValidador.EsValido(pedido, out string error))
            {
                MessageBox.Show(error, "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // El numero lo reserva el servicio al guardar. El que se previsualiza al entrar a
            // Nuevo es solo una estimacion: si otra persona guardo en el meantime, el numero
            // real sera otro y el aviso de abajo lo dice.
            string numeroPrevisualizado = uiTextBox1.Text?.Trim() ?? string.Empty;

            if (!_pedidoService.SavePedidoCompleto(pedido))
            {
                MessageBox.Show("No se pudo guardar el pedido: " + _pedidoService.ErrorMsg, "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(ConfirmacionGuardado(pedido.Numero, numeroPrevisualizado), "Pedido nuevo", MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarGeneral();
            _lineas.Clear();
            ProyectarLineasEnGrid();
            AplicarModo(ModoFormulario.Consulta);
            _ = RecargarListadoAsync();
        }

        /// <summary>
        /// Descarta el borrador y vuelve a solo lectura.
        /// </summary>
        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            DialogResult confirmacion = MessageBox.Show(
                "¿Descartar el pedido en borrador?",
                "Pedido nuevo",
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

        /// <summary>
        /// Vuelve a cargar el listado de pedidos y recalcula el contador.
        /// </summary>
        private async Task RecargarListadoAsync()
        {
            try
            {
                _dtPedidos = await _pedidoService.LoadDataPedidos();
                gridPedidos.DataSource = _dtPedidos;
                gridPedidos.ClearSelection();
                gridPedidos.CurrentCell = null;
                ActualizarContador();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al recargar los pedidos: " + ex.Message);
            }
        }

        /// <summary>
        /// Lee un combo de vocabulario cerrado. Sin seleccion devuelve null, no cadena vacia:
        /// la columna acepta NULL y un "" seria un valor distinto que rompe los agrupamientos.
        /// </summary>
        private static string? ComboOpcional(UIComboBox combo)
        {
            return combo.SelectedIndex < 0 ? null : combo.SelectedItem?.ToString();
        }

        /// <summary>
        /// Normaliza un campo de texto opcional: sin contenido devuelve null en vez de "".
        /// Aplica a las directions tambien, para no guardar cadenas vacias en la base.
        /// </summary>
        private static string? TextoOpcional(UITextBox texto) => TextoOpcional(texto.Text);

        /// <inheritdoc cref="TextoOpcional(UITextBox)"/>
        private static string? TextoOpcional(UIRichTextBox texto) => TextoOpcional(texto.Text);

        private static string? TextoOpcional(string? contenido)
        {
            string? valor = contenido?.Trim();
            return string.IsNullOrEmpty(valor) ? null : valor;
        }

        /// <summary>
        /// Selecciona en un combo de vocabulario el valor guardado. Si el dato no esta en el
        /// catalogo (por ejemplo una variante heredada con otra capitalizacion) deja el combo
        /// vacio en lugar de inventar una seleccion que no corresponde a lo que hay en la base.
        /// </summary>
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

        /// <summary>
        /// Mensaje de confirmacion. Si el numero asignado difiere del que se previsualizo, se
        /// explica en vez de mostrar solo el guardado: el usuario vio el otro numero en pantalla
        /// y merece saber cual quedo realmente.
        /// </summary>
        private static string ConfirmacionGuardado(string numeroAsignado, string numeroPrevisualizado)
        {
            string baseTexto = "Pedido " + numeroAsignado + " guardado.";

            return !PedidoNumero.EsValido(numeroPrevisualizado)
                || string.Equals(numeroAsignado, numeroPrevisualizado, StringComparison.Ordinal)
                ? baseTexto
                : baseTexto + Environment.NewLine + Environment.NewLine
                    + "Se previsualizo como " + numeroPrevisualizado
                    + " porque alguien mas registro un pedido mientras usted llenaba este. Quedo guardado como "
                    + numeroAsignado + ".";
        }

        /// <summary>
        /// Arma el pedido con los valores de la pantalla y las lineas del borrador. El numero no se
        /// pone aca: lo reserva el servicio al guardar.
        /// </summary>
        private Pedido ConstruirPedidoDesdeFormulario(DateTime fecha)
        {
            Pedido pedido = new()
            {
                // Numero lo reserva el servicio al guardar, con UPDLOCK y dentro de la
                // transaccion. Dejarlo vacio aca es lo que permite que un fallo no gaste numero.
                Numero = string.Empty,
                Fecha = fecha,
                Fecha_entrega = uiDatetimePicker2.Value.Date,
                Estado = string.IsNullOrWhiteSpace(uiTextBox2.Text) ? PedidoEstado.Creado : uiTextBox2.Text.Trim(),
                Direccion_facturacion = TextoOpcional(uiRichTextBox1),
                Direccion_entrega = TextoOpcional(uiRichTextBox2),
                Notas = uiRichTextBox3.Text?.Trim(),
                Anulado = false,
                Porc_Itbis = PorcItbisActual(),
                // Terminos comerciales. Los combos son opcionales: sin seleccion se guardan como
                // NULL y no como cadena vacia, para no crear un tercer estado invisible al lado
                // de los NULL y de los valores reales.
                Tipo_venta = ComboOpcional(cboTipoVenta),
                Condiciones_pago = ComboOpcional(cboCondicionesPago),
                Prioridad = ComboOpcional(cbo_prioridad),
                Persona_Contacto = TextoOpcional(txtPersonaContacto),
                Detalle = new List<PedidoDetalle>(_lineas)
            };

            if (ValorCombo(cbo_customers) is object valorCliente
                && Guid.TryParse(valorCliente.ToString(), out Guid clienteId))
            {
                pedido.Customer_Id = clienteId;
                pedido.Customer_Name = cbo_customers.Text;
            }

            if (ValorCombo(uiComboBox1) is object valorVendedor
                && Guid.TryParse(valorVendedor.ToString(), out Guid vendedorId))
            {
                pedido.Vendor_Id = vendedorId;
            }

            (decimal subTotal, decimal montoItbis, decimal total) = PedidoCalculos.Calcular(pedido.Detalle, pedido.Porc_Itbis);
            pedido.SubTotal = subTotal;
            pedido.Monto_Itbis = montoItbis;
            pedido.Total = total;

            return pedido;
        }

        /// <summary>
        /// Deja la pestaña General sin datos, lista para ver un pedido o empezar uno nuevo.
        /// </summary>
        private void LimpiarGeneral()
        {
            AsignarCombo(cbo_customers, null, _valoresSel);
            AsignarCombo(uiComboBox1, null, _valoresSel);
            uiTextBox1.Clear();
            uiDatetimePicker1.Value = DateTime.Now;
            uiDatetimePicker2.Value = DateTime.Now;
            uiTextBox2.Clear();
            uiRichTextBox1.Clear();
            uiRichTextBox2.Clear();
            uiRichTextBox3.Clear();
            cboTipoVenta.SelectedIndex = -1;
            cboCondicionesPago.SelectedIndex = -1;
            cbo_prioridad.SelectedIndex = -1;
            txtPersonaContacto.Clear();
            uiTextBox4.Clear();
            uiTextBox5.Clear();
            uiTextBox6.Clear();
            uiTextBox7.Clear();
            txt_id_cust.Clear();

            txt_id_vendor.Clear();

            LimpiarEditorLinea();
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

        private void uiLabel15_Click(object sender, EventArgs e)
        {

        }

        private void btnEditarProducto_Click(object sender, EventArgs e)
        {

        }
    }
}
