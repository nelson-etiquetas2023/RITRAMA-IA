using System.Data;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Text;
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
        private const string ColumnaSeleccion = "seleccionado";
        private Sunny.UI.UIButton? btnEnviarDispositivo;
        private readonly Dictionary<UIComboBox, object?> _valoresSel = new();

        // Anulacion de pedidos: mientras el switch se sincroniza desde los datos no se debe
        // disparar su handler (guardaria un UPDATE que el usuario no pidio).
        private bool _actualizandoSwitchAnulado;

        // Fila del DataTable que sostiene el pedido mostrado en General. Sirve para poner
        // anulado = 0/1 en sitio y que el grid se repinte sin recargar la lista.
        private DataRow? _filaPedidoActual;

        // Fecha de entrega de la fila cargada. Cuando la fila trae NULL el date picker no
        // se toca y se queda con el valor heredado (el del pedido anterior o la hora
        // actual), asi que hay que recordar que el dato no existia y que valor se mostro:
        // de ahi sale la regla de ConstruirPedidoDesdeFormulario para no inventar una
        // fecha de entrega que el usuario nunca puso.
        private bool _fechaEntregaCargadaNula;
        private DateTime _fechaEntregaMostrada;

        /// <summary>
        /// Fuente de verdad del detalle: las lineas del pedido que se esta editando. El grid es
        /// una proyeccion de esta lista, nunca su almacen.
        /// </summary>
        private readonly List<PedidoDetalle> _lineas = new();

        // Id del producto de la linea cargada en el editor de linea cuando el combo no
        // puede mostrarlo (el producto fue anulado y el combo solo trae los activos). El
        // editor se resuelve entonces contra la tabla completa. Nulo cuando el combo si
        // mostro el producto o cuando el editor no vino de una linea.
        private string? _productoLineaNoVisible;

        /// <summary>
        /// Crea el formulario de pedidos con sus servicios por DI.
        /// </summary>
        /// <param name="pedidoService">Servicio de acceso a datos de pedidos.</param>
        /// <param name="productsService">Servicio del catálogo de productos.</param>
        /// <param name="configuration">Configuración de la aplicación.</param>
        public FrmPedidos(IPedidoService pedidoService, IProductsService productsService, IConfiguration configuration)
        {
            InitializeComponent();
            gridPedidos.ReadOnly = false;

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

            // UISwitch nace con Style = Inherited, y UIBaseForm.OnShown llama Render(),
            // que via UIStyleHelper.SetChildUIStyle() re-aplica SetStyleColor() del tema
            // global a todo control Inherited. Eso pasa DESPUES de que FormManager/Main
            // llamen ReaplicarTema(), asi que borraba InActiveColor (el rojo del pedido
            // anulado) y ActiveColor. SetChildUIStyle solo toca controles Inherited:
            // marcar el switch como Custom lo excluye de la herencia y sus colores
            // propios quedan fijos.
            sw_anular_pedido.Style = UIStyle.Custom;

            AplicarTemaVerde();
            uiDataGridView1.AllowUserToAddRows = false;
            uiDataGridView1.ReadOnly = true;

            // Fechas con dia, mes, anio y hora. Sin esto los dos date pickers caian en el
            // formato por defecto de SunnyUI, que no muestra la hora. Se fija en codigo y no en
            // el disenador para no depender de que el disenador lo mantenga.
            ConfigurarFechas();



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
            //btnLimpiarBusqueda.Click += BtnLimpiarBusqueda_Click;

            // Anulacion: el switch solo actua sobre un pedido ya cargado en Consulta. Arranca
            // apagado y deshabilitado hasta que CargarPedidoEnGeneral (o el guardia de Nuevo)
            // lo ponga segun la columna anulado. Los colores se fijan en AplicarTemaVerde.
            sw_anular_pedido.Enabled = false;
            sw_anular_pedido.ActiveChanged += SwAnularPedido_ActiveChanged;

            // Acciones sobre las lineas de producto del detalle.
            btnAddProducto.Click += (_, _) => AgregarLinea();
            btnEditarProducto.Click += (_, _) => EditarLinea();
            btnEliminarProducto.Click += (_, _) => EliminarLinea();
            btnBuscarProducto.Click += BtnBuscarProducto_Click;
            uiTextBox7.TextChanged += (_, _) => ActualizarTotalesEnPantalla();

            // Width y Length: solo editables para productos tipo Rollo Cortado.
            uiComboBox2.SelectedValueChanged += (_, _) => AplicarEditabilidadMedidas();

            // Barra: Nuevo abre el borrador; Editar entra en la edicion del pedido cargado;
            // Guardar y Cancelar solo existen en los modos de escritura.
            btnNuevo.Click += BtnNuevo_Click;
            btnEditar.Click += BtnEditar_Click;
            btnGuardar.Click += BtnGuardar_Click;
            btnCancelar.Click += BtnCancelar_Click;
            uiDataGridView1.SelectionChanged += (_, _) => CargarLineaSeleccionadaEnEditor();

            

            // Botón Enviar Dispositivo: envía los pedidos seleccionados al proceso de picking.
            // Se coloca en el panel de búsqueda, debajo del textbox y encima del grid.
            btnEnviarDispositivo = new Sunny.UI.UIButton
            {
                Text = "Enviar Dispositivo",
                Name = "btnEnviarDispositivo",
                Size = new Size(200, 30),
                Location = new Point(72, 56)
            };
            btnEnviarDispositivo.Click += BtnEnviarDispositivo_Click;
            panelBuscar.Controls.Add(btnEnviarDispositivo);

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

            // UISwitch.SetStyleColor() se ejecuta al poner Style y REEMPLAZA ActiveColor e
            // InActiveColor por los del tema, borrando el rojo que el Designer dejo puesto.
            // Por eso van aqui, despues de Style =: el switch se pinta con ActiveColor
            // (verde) cuando esta activo y con InActiveColor (rojo) cuando esta anulado.
            sw_anular_pedido.ActiveColor = LightGreenTheme.PrimaryDark;
            sw_anular_pedido.InActiveColor = Color.Firebrick;

            EstilizarGridVerde();
        }

        /// <summary>
        /// Estado del formulario. Consulta es el estado por defecto: la pestaña General es de
        /// solo lectura y solo se habilita al entrar explicitamente a Nuevo o a Editar.
        /// </summary>
        private enum ModoFormulario
        {
            Consulta,
            Nuevo,
            Editar
        }

        private ModoFormulario _modo = ModoFormulario.Consulta;

        /// <summary>
        /// True cuando el formulario admite escritura: Nuevo crea un pedido y Editar modifica
        /// el cargado. Consulta es el unico estado de solo lectura total.
        /// </summary>
        private bool EsEditable => _modo is ModoFormulario.Nuevo or ModoFormulario.Editar;

        /// <summary>
        /// Unico metodo que cambia ReadOnly o Enabled. Al volver a Consulta deja toda la
        /// pestaña General en solo lectura.
        /// </summary>
        private void AplicarModo(ModoFormulario modo)
        {
            _modo = modo;

            // Encabezado siempre de solo lectura.
            uiTextBox1.ReadOnly = true;
            uiTextBox2.ReadOnly = true;
            uiTextBox4.ReadOnly = true;
            uiTextBox5.ReadOnly = true;
            uiTextBox6.ReadOnly = true;

            // Editables solo en los modos de escritura (textos y areas de texto: ReadOnly ya
            // impide escribir). Fecha Reg es editable: hay pedidos que se registran con fecha
            // anterior a la del sistema, y el dato se guarda tal como lo pone el usuario.
            //
            // Las dos direcciones NO van aqui: son datos del maestro de clientes y no se editan
            // nunca. Se dejan fijas mas abajo, junto con su comentario.
            uiDatetimePicker1.ReadOnly = !EsEditable;
            uiDatetimePicker2.ReadOnly = !EsEditable;
            uiRichTextBox3.ReadOnly = !EsEditable;
            uiTextBox7.ReadOnly = !EsEditable;
            uiTextBox3.ReadOnly = !EsEditable;
            uiTextBox8.ReadOnly = !EsEditable;
            uiTextBox9.ReadOnly = !EsEditable;

            // Combos y fechas: ReadOnly solo impide escribir. Verificado en el IL de SunnyUI
            // 3.9.8 que UIDropControl.ReadOnly solo escribe en el TextBoxBase interno
            // (edit.ReadOnly), mientras UIComboBox.ListBox_Click y Edit_KeyDown siguen
            // cambiando el valor, y el desplegable se sigue abriendo con el raton. Por eso en
            // solo lectura se deshabilitan: Enabled = false no entrega ni teclado ni raton.
            uiDatetimePicker1.Enabled = EsEditable;
            uiDatetimePicker2.Enabled = EsEditable;
            cbo_customers.ReadOnly = !EsEditable;
            cbo_customers.Enabled = EsEditable;
            uiComboBox1.ReadOnly = !EsEditable;
            uiComboBox1.Enabled = EsEditable;
            uiComboBox2.ReadOnly = !EsEditable;
            uiComboBox2.Enabled = EsEditable;

            // Informativos: se muestran pero el usuario no los edita en ningun modo.
            txt_id_cust.ReadOnly = true;
            txt_id_vendor.ReadOnly = true;

            // Terminos del pedido. Los combos se ven en los dos modos para poder consultar un
            // pedido ya guardado, pero solo se editan escribiendo (Nuevo o Editar). El contacto
            // tambien se ve al consultar: antes vivia concatenado dentro de Ship To y, al
            // separar las direcciones, se hubiera quedado invisible sin su propio campo.
            cboTipoVenta.ReadOnly = !EsEditable;
            cboTipoVenta.Enabled = EsEditable;
            AplicarEditabilidadCondicionesPago();
            cbo_prioridad.ReadOnly = !EsEditable;
            cbo_prioridad.Enabled = EsEditable;
            txtPersonaContacto.ReadOnly = !EsEditable;
            txtPersonaContacto.Visible = true;

            // Las dos direcciones no se editan nunca: son datos del maestro de clientes, no del
            // pedido. Se muestran de solo lectura y se llenan solas al elegir el cliente, que es
            // donde van a buscar a la base. Editables solo al crear las ponia una version
            // anterior, y permitia guardar un pedido con una direccion que no era la del cliente:
            // un pedido con una direccion inventada a mano no se puede ni rastrear.
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

            // Width y Length del editor siguen el mismo criterio que los botones de linea.
            AplicarEditabilidadMedidas();

            // Barra de herramientas. En Consulta se ven las acciones de la lista (Nuevo,
            // Editar y el switch de anulacion); en los modos de escritura, Guardar y
            // Cancelar. El estado habilitado de btnEditar lo gobierna la seleccion de la
            // lista: lo enciende CargarPedidoEnGeneral y lo apaga LimpiarGeneral.
            btnNuevo.Visible = _modo == ModoFormulario.Consulta;
            btnEditar.Visible = _modo == ModoFormulario.Consulta;
            btnGuardar.Visible = EsEditable;
            btnCancelar.Visible = EsEditable;

            gridPedidos.Enabled = !EsEditable;

            // El switch de anulacion solo tiene sentido sobre un pedido guardado y en Consulta:
            // en modo Nuevo no hay numero aun (y aca se borra General, que lo deja activo), y en
            // Editar anular no es parte de la edicion. Se fuerza "Pedido Activo" sin disparar el
            // handler de persistencia.
            _actualizandoSwitchAnulado = true;
            sw_anular_pedido.Active = true;
            _actualizandoSwitchAnulado = false;
            sw_anular_pedido.Enabled = _modo == ModoFormulario.Consulta && _filaPedidoActual != null;
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

                // La selección del listado vive como una columna más del DataTable, y no en un
                // HashSet del formulario, por una razón útil: el filtro de búsqueda solo cambia
                // DefaultView.RowFilter sobre los mismos DataRow, así que un pedido marcado
                // sigue marcado cuando el usuario escribe en el buscador. Un HashSet en cambio
                // tendría que reconciliarse con cada cambio de filtro.
                // Por omisión todos vienen en false, que es "no marcado". La columna se agrega
                // DESPUÉS de que la tabla ya tiene filas, así que las celdas nuevas nacen en
                // DBNull: sin este bucle la columna checkbox pinta DBNull y revienta con
                // FormatException en el primer pintado de la pantalla.
                _dtPedidos.Columns.Add(ColumnaSeleccion, typeof(bool));
                foreach (DataRow fila in _dtPedidos.Rows)
                {
                    fila[ColumnaSeleccion] = false;
                }

                gridPedidos.DataSource = _dtPedidos;
                gridPedidos.ClearSelection();
                gridPedidos.CurrentCell = null;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Pedidos: " + ex.Message);
            }

            // btnEditar nace habilitado en el disenador y su estado lo gobierna la seleccion
            // de la lista. Al terminar la carga inicial no hay fila elegida (se acaba de anular
            // la seleccion), asi que se apaga hasta que CargarPedidoEnGeneral lo vuelva a
            // encender sobre un pedido cargado y no anulado.
            btnEditar.Enabled = false;

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

// Primera columna: selección con checkbox (30 px)
            DataGridViewCheckBoxColumn colSeleccion = new()
            {
                Name = "seleccionado",
                HeaderText = string.Empty,
                DataPropertyName = ColumnaSeleccion,
                Width = 30,
                AutoSizeMode = DataGridViewAutoSizeColumnMode.None,
                ReadOnly = false,
                SortMode = DataGridViewColumnSortMode.NotSortable,
                ThreeState = false,
                DefaultCellStyle = new DataGridViewCellStyle { Alignment = DataGridViewContentAlignment.MiddleCenter }
            };
            colSeleccion.HeaderCell.Style.Alignment = DataGridViewContentAlignment.MiddleCenter;
            // Valores explícitos: sin esto el checkbox compara contra null y en la conversión
            // puede tirar FormatException.
            colSeleccion.ThreeState = false;
            colSeleccion.FalseValue = false;
            colSeleccion.TrueValue = true;
            colSeleccion.IndeterminateValue = false;
            gridPedidos.Columns.Add(colSeleccion);

            CommonService.ADD_COLUMN_GRID("numero", 120, "Numero", "numero", gridPedidos);
            CommonService.ADD_COLUMN_GRID("customer_name", 160, "Cliente", "customer_name", gridPedidos);
            CommonService.ADD_COLUMN_GRID("estado", 110, "Status", "estado", gridPedidos);

            // El numero del pedido es SO-#####, ocho caracteres, y a 90 px el dato se cortaba.
            // El ancho solo no alcanza: con FillWeight el grid reparte el espacio disponible al
            // redimensionar y manda el peso, no el tamano fijo. Por eso numero sube de 20 a 30 y
            // el cliente baja de 60 a 50, que sigue sobrando para un nombre largo. La columna de
            // seleccion entra con 5 para no competir con el reparto.
            foreach (DataGridViewColumn columna in gridPedidos.Columns)
            {
                columna.FillWeight = columna.Name switch
                {
                    "numero" => 30,
                    "customer_name" => 50,
                    "seleccionado" => 5,
                    _ => 20
                };
            }

            // El clic del usuario es el que dispara el marcado. Sin EditOnEnter y sin confirmar
            // la celda a mano, el tilde se dibujaria pero el DataRow no se enteraria hasta que
            // el foco se moviera a otro lado, y marcar todo quedaria a medio camino.
            gridPedidos.CellContentClick += GridPedidos_CellContentClick;
            gridPedidos.CurrentCellDirtyStateChanged += GridPedidos_CurrentCellDirtyStateChanged;

            // Registrados UNA sola vez aquí, en el constructor, antes de que InitializeAsync
            // asigne el DataSource. Antes vivían dentro de RecargarListadoAsync con +=, así
            // que no existían durante la primera carga (donde salía el diálogo de Format-
            // Exception) y encima se acumulaban uno por cada recarga.
            gridPedidos.DataError += GridPedidos_DataError;
            gridPedidos.CellFormatting += GridPedidos_CellFormatting;
        }

        /// <summary>
        /// Suprime el FormatException que el DataGridView reporta al pintar la columna
        /// checkbox con un valor que no puede convertir. Sin esto el control muestra su
        /// diálogo de error al usuario.
        /// </summary>
        private void GridPedidos_DataError(object? sender, DataGridViewDataErrorEventArgs e)
        {
            if (e.Exception is FormatException)
            {
                e.ThrowException = false;
                System.Diagnostics.Debug.WriteLine(
                    $"FormatException suprimido: Col={e.ColumnIndex}, Row={e.RowIndex}, Err={e.Exception.Message}");
            }
        }

        /// <summary>
        /// Fuerza el formato de la celda checkbox a bool para que la conversión no dependa
        /// del valor crudo del DataRow. Las filas de pedidos anulados se pintan de rojo
        /// para que sigan visibles en el listado (requisito del usuario).
        /// </summary>
        private void GridPedidos_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }

            if (e.ColumnIndex == gridPedidos.Columns[ColumnaSeleccion]?.Index && e.Value is bool b)
            {
                e.Value = b;
                e.FormattingApplied = true;
            }

            // Fila completa en rojo para anulados. Se evalua en cada celda porque
            // CellFormatting es por celda; el costo es un solo acceso a DataRow.
            if (gridPedidos.Rows[e.RowIndex].DataBoundItem is DataRowView drv && EsAnulado(drv.Row))
            {
                e.CellStyle.BackColor = Color.Firebrick;
                e.CellStyle.ForeColor = Color.White;
                e.CellStyle.SelectionBackColor = Color.DarkRed;
                e.CellStyle.SelectionForeColor = Color.White;
            }
        }

        /// <summary>
        /// Marca o desmarca un pedido, o todos los visibles cuando se hace clic en el encabezado.
        /// El encabezado tiene tres estados: vacío cuando no hay ninguno marcado, tilde cuando
        /// estan todos, y guion cuando hay algunos. Un clic en cualquiera de los tres pasa a
        /// "todos marcados"; volver a marcarlo cuando ya estaban todos los desmarca.
        /// </summary>
        private void GridPedidos_CellContentClick(object? sender, DataGridViewCellEventArgs e)
        {
            if (e.ColumnIndex != gridPedidos.Columns["seleccionado"]?.Index)
            {
                return;
            }

            if (e.RowIndex == -1)
            {
                MarcarTodosSeleccionados(HeaderEstaTodoMarcado() ? false : true);
                return;
            }

            if (e.RowIndex >= 0)
            {
                gridPedidos.EndEdit();
                ActualizarSeleccion();
            }
        }

        /// <summary>
        /// Confirma el valor de la celda justo después de marcarla, sin esperar a que el foco
        /// se mueva a otro lado. Sin esto el DataRow conserva el valor anterior y el tilde
        /// dibujado no es el guardado.
        /// </summary>
        private void GridPedidos_CurrentCellDirtyStateChanged(object? sender, EventArgs e)
        {
            if (gridPedidos.IsCurrentCellDirty
                && gridPedidos.CurrentCell is { ColumnIndex: var columna }
                && columna == gridPedidos.Columns["seleccionado"]?.Index)
            {
                gridPedidos.CommitEdit(DataGridViewDataErrorContexts.Commit);
            }
        }

        /// <summary>
        /// Marca o desmarca todas las filas visibles. Si el checkbox del encabezado está
        /// vacío (ninguno marcado), marca todos; si todos ya estaban marcados, los desmarca.
        /// </summary>
        private void MarcarTodosSeleccionados(bool marcado)
        {
            if (_dtPedidos == null) return;
            if (!_dtPedidos.Columns.Contains(ColumnaSeleccion)) return;

            foreach (DataRowView fila in _dtPedidos.DefaultView)
            {
                fila[ColumnaSeleccion] = marcado;
            }

            gridPedidos.Refresh();
            ActualizarSeleccion();
        }

        /// <summary>True cuando todas las filas visibles están marcadas.</summary>
        private bool HeaderEstaTodoMarcado()
        {
            int visibles = 0;
            int marcados = 0;
            ContarSeleccionados(out visibles, out marcados);

            return visibles > 0 && visibles == marcados;
        }

        /// <summary>
        /// Actualiza el estado visual del encabezado de selección y el contador.
        /// Método consolidado para evitar llamadas múltiples que causan FormatException.
        /// </summary>
        private void ActualizarSeleccion()
        {
            if (_dtPedidos == null) return;

            DataGridViewColumn? columna = gridPedidos.Columns[ColumnaSeleccion];
            if (columna == null) return;

            ContarSeleccionados(out int visibles, out int marcados);

            // No asignar HeaderCell.Value para evitar FormatException en la columna checkbox
            // Solo actualizamos el contador

            lblContador.Text = marcados == 0
                ? $"{_dtPedidos.DefaultView.Count} pedidos"
                : $"{_dtPedidos.DefaultView.Count} pedidos ({marcados} seleccionados)";
        }

        /// <summary>
        /// Cuenta las filas que se están viendo y cuántas de ellas están marcadas. Cuenta sobre
        /// el DefaultView, así que con el filtro puesto mira solo lo que el usuario tiene a la vista.
        /// </summary>
        private void ContarSeleccionados(out int visibles, out int marcados)
        {
            visibles = 0;
            marcados = 0;
            if (_dtPedidos == null) return;
            if (!_dtPedidos.Columns.Contains(ColumnaSeleccion)) return;

            foreach (DataRowView fila in _dtPedidos.DefaultView)
            {
                visibles++;
                if (fila[ColumnaSeleccion] is true)
                {
                    marcados++;
                }
            }
        }

        /// <summary>
        /// Obtiene la lista de números de los pedidos marcados en el orden del listado. Es el
        /// punto de entrada para la acción "enviar al picking".
        /// </summary>
        private List<string> NumerosPedidosSeleccionados()
        {
            List<string> numeros = new List<string>();
            if (_dtPedidos == null) return numeros;
            if (!_dtPedidos.Columns.Contains(ColumnaSeleccion)) return numeros;

            foreach (DataRowView fila in _dtPedidos.DefaultView)
            {
                if (fila[ColumnaSeleccion] is true && fila["numero"] is not null)
                {
                    numeros.Add(fila["numero"].ToString() ?? string.Empty);
                }
            }

            return numeros;
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
            ActualizarSeleccion();
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
                // Fila sin fecha de entrega: el picker se queda con lo que ya mostraba. Se
                // anota el valor visible para que, al guardar, una fecha heredada no se
                // persista como si la hubiera elegido el usuario.
                _fechaEntregaCargadaNula = true;
                _fechaEntregaMostrada = uiDatetimePicker2.Value;
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

            // El % de ITBIS se carga antes que los totales: si TextChanged dispara
            // ActualizarTotalesEnPantalla (el control avisa cuando cambia el texto), lo que
            // pasa abajo lo sobrescribe con los montos guardados. Al reves, el recalculo
            // quedaria el ultimo escritor y pisaria los totales con lineas que todavia no
            // se cargaron. Pedidos viejos sin columna o con valor raro: se asume 18.
            uiTextBox7.Text = decimal.TryParse(Safe(drv, "porc_itbis")?.ToString(), out decimal porcItbis)
                ? porcItbis.ToString("0.##")
                : "18";

            uiTextBox4.Text = FormatoDinero(Safe(drv, "subtotal"));
            uiTextBox5.Text = FormatoDinero(Safe(drv, "itbis"));
            uiTextBox6.Text = FormatoDinero(Safe(drv, "total$"));

            string numero = Safe(drv, "numero")?.ToString() ?? string.Empty;
            _ = CargarDetallePedidoAsync(numero);

            // Estado de anulacion: el switch refleja la columna anulado (1 = anulado, el
            // switch apagado). La guardia evita que ActiveChanged dispare un UPDATE al
            // sincronizar. Si el valor viene raro (DBNull en filas viejas) se asume activo.
            _filaPedidoActual = drv.Row;
            bool anulado = EsAnulado(drv.Row);
            _actualizandoSwitchAnulado = true;
            sw_anular_pedido.Active = !anulado;
            _actualizandoSwitchAnulado = false;
            sw_anular_pedido.Enabled = _modo == ModoFormulario.Consulta;

            // btnEditar refleja la seleccion de la lista: solo se puede editar un pedido
            // cargado y, ademas, uno que no este anulado.
            btnEditar.Enabled = !anulado;
        }

        /// <summary>
        /// Lee la columna anulado de una fila del listado tolerando DBNull y textos.
        /// </summary>
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

        /// <summary>
        /// Cambia el estado de anulación del pedido mostrado. El cambio se pide con
        /// confirmación y se escribe en la base; si falla o se cancela, el switch
        /// regresa al estado anterior sin tocar la fila.
        /// </summary>
        private void SwAnularPedido_ActiveChanged(object? sender, EventArgs e)
        {
            // Durante la sincronización desde datos (CargarPedidoEnGeneral / AplicarModo /
            // LimpiarGeneral) no hay decisión del usuario que persistir.
            if (_actualizandoSwitchAnulado)
            {
                return;
            }

            if (_modo != ModoFormulario.Consulta || _filaPedidoActual is null)
            {
                return;
            }

            string numero = _filaPedidoActual["numero"]?.ToString() ?? string.Empty;
            if (string.IsNullOrWhiteSpace(numero))
            {
                return;
            }

            bool activo = sw_anular_pedido.Active;
            string pregunta = activo
                ? $"¿Restaurar el pedido {numero}?\nVolverá a quedar activo."
                : $"¿Anular el pedido {numero}?\nSeguirá visible en la lista con la fila en rojo.";
            DialogResult confirmacion = MessageBox.Show(
                pregunta,
                "Anulación de pedido",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question);

            if (confirmacion != DialogResult.Yes)
            {
                // Revierte el switch al estado que tenia en la base.
                _actualizandoSwitchAnulado = true;
                sw_anular_pedido.Active = !activo;
                _actualizandoSwitchAnulado = false;
                return;
            }

            bool exitoso = activo
                ? _pedidoService.RestaurarPedido(numero)
                : _pedidoService.AnularPedido(numero);

            if (!exitoso)
            {
                MessageBox.Show(
                    "No se pudo actualizar la anulación: " + _pedidoService.ErrorMsg,
                    "Anulación de pedido",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
                _actualizandoSwitchAnulado = true;
                sw_anular_pedido.Active = !activo;
                _actualizandoSwitchAnulado = false;
                return;
            }

            // Éxito: se marca la fila en el DataTable para que el grid la pinte de roja
            // (o le quite el rojo) sin recargar el listado ni perder la selección. Se
            // refresca todo el grid porque el índice del DataTable no coincide con el de la
            // grilla cuando hay filtro u orden activos.
            // La columna puede venir como bool (BIT de SQL Server) o como numérica: se
            // asigna el tipo que declare la tabla para no lanzar ArgumentException.
            bool anuladoAhora = !activo;
            Type tipoAnulado = _filaPedidoActual.Table.Columns["anulado"]?.DataType ?? typeof(int);
            _filaPedidoActual["anulado"] = tipoAnulado == typeof(bool)
                ? (object)anuladoAhora
                : (anuladoAhora ? 1 : 0);
            gridPedidos.Refresh();

            // Excepcion documentada al contrato "AplicarModo es el unico escritor de
            // ReadOnly/Enabled": btnEditar no gobierna la pestaña General sino la seleccion
            // de la lista (mismo criterio que CargarPedidoEnGeneral y LimpiarGeneral), y
            // aqui la fila acaba de cambiar de estado sin que se vuelva a disparar
            // SelectionChanged. Sin esto, restaurar dejaba el boton apagado hasta que el
            // usuario cambiara de fila y anular lo dejaba encendido.
            btnEditar.Enabled = !anuladoAhora;
        }

        /// <summary>
        /// Carga las líneas del pedido seleccionado en el grid de detalle.
        ///
        /// Devuelve true solo cuando las lineas quedaron aplicadas. BtnEditar_Click exige
        /// esa senal para no entrar en edicion con un detalle que no se cargo: los demas
        /// llamadores lo disparan y lo ignoran (fire-and-forget), asi que el cambio no los
        /// afecta.
        /// </summary>
        private async Task<bool> CargarDetallePedidoAsync(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                // Se pasa por la proyeccion y no por un Rows.Clear() a mano, para que el total de
                // cantidad se ponga en cero tambien. Limpiando solo el grid, el total se quedaba
                // con el del pedido anterior.
                _lineas.Clear();
                ProyectarLineasEnGrid();
                // Sin numero no hay detalle que editar: se reporta como fallo para que la
                // entrada a Editar no se ciegue con un pedido que no puede cargar.
                return false;
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
                    return false;
                }

                if (_ultimoPedidoDetalleConsulta != numero)
                {
                    return false;
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

                return true;
            }
            catch (OperationCanceledException)
            {
                return false;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el detalle del pedido: " + ex.Message);
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

        /// <summary>
        /// Proyecta las lineas del pedido sobre el grid. Cada fila guarda su linea en Tag, de
        /// modo que agregar, editar y eliminar no dependan del indice visible de la fila.
        /// </summary>
        private void ProyectarLineasEnGrid()
        {
            uiDataGridView1.Rows.Clear();
            foreach (PedidoDetalle linea in _lineas)
            {
                // La primera columna se titula "Id. Pro." y por ahi va el codigo del producto.
                // Antes receives el numero de renglon, que no es un identificador de nada y
                // ademas repetia el nombre que ya muestra la columna de al lado.
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

        private void ActualizarContador() => ActualizarSeleccion();

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
        /// Refresca el campo "Total Cantidad". Se llama desde la proyeccion al grid porque esa es
        /// la unica operacion que pasa por todos los caminos en que las lineas cambian: agregar,
        /// editar, eliminar, cargar un pedido y limpiar la pantalla. Poner el refresco en un
        /// punto por caso seria seis lugares que hay que acordarse.
        /// </summary>
        private void ActualizarTotalCantidad()
        {
            txt_total_cantidad.Text = PedidoCalculos.TotalCantidad(_lineas).ToString("N2");
        }

        /// <summary>
        /// Formato de las dos fechas del pedido: dia, mes, anio y hora en 24 horas.
        ///
        /// dd/MM/yyyy HH:mm es la sintaxis de .NET, donde MM es el mes y mm los minutos; con
        /// minúsculas se confundirian y la pantalla mostraria el mes donde va la hora.
        ///
        /// Ojo: el UIDatePicker de SunnyUI no tiene editor de hora, ShowType solo ofrece
        /// YearMonthDay, YearMonth y Year. Con esto la hora se MUESTRA, tomada del Value, pero
        /// el usuario no la puede escribir: la del Value es la de DateTime.Now del momento en
        /// que se creo el pedido.
        /// </summary>
        private void ConfigurarFechas()
        {
            const string formato = "dd/MM/yyyy HH:mm";

            uiDatetimePicker1.DateFormat = formato;
            uiDatetimePicker2.DateFormat = formato;
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
                combo.SelectedValueChanged += (_, _) =>
                {
                    _valoresSel[combo] = combo.SelectedValue;

                    // Elegir un producto en el editor anula el rescate por la linea original:
                    // _productoLineaNoVisible solo existe para que una linea con producto
                    // anulado siga siendo editable mientras el combo no pueda mostrarlo.
                    // En cuanto el usuario elige otro producto, ese pacto termina y si luego
                    // vacia el combo tiene que pedir uno, no volver al producto con el que
                    // cargo la linea.
                    if (ReferenceEquals(combo, uiComboBox2)
                        && combo.SelectedValue is not null
                        && combo.SelectedValue != DBNull.Value)
                    {
                        _productoLineaNoVisible = null;
                    }
                };
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

            if (!EsEditable)
            {
                return;
            }

            // Solo escribiendo (Nuevo o Editar): al consultar, las direcciones vienen del pedido
            // guardado, no del maestro. Si se pisaran aqui, reabrir un pedido viejo mostraria la
            // direccion que el cliente tiene hoy en vez de la que se acordo ese dia. Al editar si
            // se refrescan: si el usuario cambia de cliente, mostrar la direccion del cliente
            // viejo estaria mal.
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
                ComboOpcional(cboTipoVenta), EsEditable);

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

        /// <summary>
        /// Habilita width/length cuando se está agregando un producto.
        /// En los modos de escritura (Nuevo o Editar) siempre están editables.
        /// </summary>
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

        /// Vuelca el editor sobre la linea, incluido el total del renglon. Agregar y Editar
        /// comparten esta copia para que no se separen los campos que se guardan.
        /// </summary>
        private void CopiarValoresEditor(PedidoDetalle linea, DataRow producto, decimal cant, decimal? precio)
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
            linea.Total_Renglon = PedidoCalculos.TotalRenglon(cant, precio);
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

        /// <summary>
        /// Carga la linea seleccionada en los controles del editor para poder modificarla.
        /// </summary>
        private void CargarLineaEnEditor(PedidoDetalle linea)
        {
            AsignarCombo(uiComboBox2, linea.Product_id, _valoresSel);
            // Si el combo no puede mostrar el producto de la linea (fue anulado y el
            // catalogo del combo solo trae activos) se recuerda su id: TryGetProductoEditor
            // lo resuelve contra la tabla completa para que la linea siga siendo editable.
            // Con el combo mostrando el producto no se guarda nada, para que vaciarlo a mano
            // siga pidiendo elegir uno.
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

        /// <summary>
        /// Carga en el editor la fila que el usuario acaba de seleccionar, para que Editar
        /// trabaje sobre ella. Fuera de los modos de escritura el editor esta deshabilitado y
        /// no se toca.
        /// </summary>
        private void CargarLineaSeleccionadaEnEditor()
        {
            if (!EsEditable)
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
                MessageBox.Show("Seleccione la linea que desea editar.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                MessageBox.Show("Seleccione la linea que desea eliminar.", TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            DialogResult confirmacion = MessageBox.Show(
                "¿Eliminar la linea " + (linea.Product_name ?? string.Empty) + " del borrador?",
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

        /// <summary>
        /// Vacia los controles del editor de linea.
        /// </summary>
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
                // El combo quedo sin valor: si el editor vino de una linea cuyo producto no
                // puede mostrarse (anulado), se rescata por el id de la propia linea contra
                // la tabla completa, no contra la vista filtrada del combo. Si el editor no
                // vino de una linea o el producto ya no existe, se pide uno.
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
        /// Titulo de los dialogos del formulario. Los mismos mensajes sirven para el borrador
        /// nuevo y para la edicion de un pedido guardado, y el titulo es el que distingue en
        /// cual de los dos se esta trabajando.
        /// </summary>
        private string TituloModo() => _modo == ModoFormulario.Editar ? "Edición de pedido" : "Pedido nuevo";

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
        /// Entra al modo Editar sobre el pedido cargado en General. El modo es explicito: no
        /// se edita nada hasta que el usuario lo pide, y para eso antes tiene que haber un
        /// pedido seleccionado y no anulado.
        /// </summary>
        private async void BtnEditar_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeEditar("Pedidos"))
            {
                MessageBox.Show("No tiene permiso para editar pedidos.", "Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            if (_filaPedidoActual is null)
            {
                MessageBox.Show("Seleccione el pedido que desea editar.", "Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            if (EsAnulado(_filaPedidoActual))
            {
                MessageBox.Show("No se puede editar un pedido anulado.", "Pedidos", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            string numero = _filaPedidoActual["numero"]?.ToString() ?? string.Empty;

            // Se espera la carga del detalle antes de entrar: _lineas es la fuente de verdad
            // que se reemplaza al guardar, y la carga que dispara la seleccion del listado es
            // asincrona. Sin esperar, podria llegar despues y pisar las lineas que el usuario
            // empiece a editar.
            bool detalleCargado = await CargarDetallePedidoAsync(numero);

            // Re-validacion DESPUES de la espera. Mientras duro el await el listado sigue
            // vivo en Consulta: elegir otra fila cambia _filaPedidoActual y la cabecera, y la
            // carga de esa nueva seleccion cancela la que estaba en curso (o una carga que
            // fallo dejo las lineas viejas). Sin este corte, Editar podia abrirse con la
            // cabecera de un pedido y las lineas de otro, y Guardar las escribia.
            //
            // El corte de seleccion va primero: si la seleccion cambio, ahi termina la
            // historia (y el mensaje habla de la seleccion, que es lo que paso). El caso de
            // "no se pudo cargar" queda para cuando la seleccion sigue siendo la misma.
            if (_filaPedidoActual is null
                || !string.Equals(
                    _filaPedidoActual["numero"]?.ToString(),
                    numero,
                    StringComparison.OrdinalIgnoreCase))
            {
                MessageBox.Show(
                    "La selección del listado cambió mientras se cargaba el detalle de "
                        + numero + ". No se entró en edición: pulse Editar de nuevo sobre "
                        + "el pedido que desea editar.",
                    "Pedidos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (!detalleCargado)
            {
                MessageBox.Show(
                    "No se pudo cargar el detalle del pedido " + numero
                        + ". No se entró en edición; intente de nuevo.",
                    "Pedidos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            AplicarModo(ModoFormulario.Editar);
            ActualizarTotalesEnPantalla();
            cbo_customers.Focus();
        }

        /// <summary>
        /// Valida el borrador y lo guarda, como pedido nuevo o como edicion del cargado. Al
        /// terminar vuelve a solo lectura.
        /// </summary>
        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            if (!EsEditable)
            {
                return;
            }

            bool esNuevo = _modo == ModoFormulario.Nuevo;
            bool permitido = esNuevo
                ? PermisoHelper.PuedeCrear("Pedidos")
                : PermisoHelper.PuedeEditar("Pedidos");
            if (!permitido)
            {
                MessageBox.Show(
                    esNuevo ? "No tiene permiso para crear pedidos." : "No tiene permiso para editar pedidos.",
                    "Pedidos",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Warning);
                return;
            }

            Pedido pedido = ConstruirPedidoDesdeFormulario(uiDatetimePicker1.Value);
            if (!PedidoValidador.EsValido(pedido, out string error))
            {
                MessageBox.Show(error, TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            // En Nuevo el numero lo reserva el servicio al guardar y el de pantalla es solo una
            // estimacion: si otra persona guardo en el meantime, el numero real sera otro y el
            // aviso de abajo lo dice. En Editar los dos son el mismo.
            string numeroPrevisualizado = uiTextBox1.Text?.Trim() ?? string.Empty;

            bool ok = esNuevo
                ? _pedidoService.SavePedidoCompleto(pedido)
                : _pedidoService.ActualizarPedidoCompleto(pedido);

            if (!ok)
            {
                MessageBox.Show("No se pudo guardar el pedido: " + _pedidoService.ErrorMsg, TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(ConfirmacionGuardado(pedido.Numero, numeroPrevisualizado), TituloModo(), MessageBoxButtons.OK, MessageBoxIcon.Information);
            LimpiarGeneral();
            _lineas.Clear();
            ProyectarLineasEnGrid();
            AplicarModo(ModoFormulario.Consulta);

            // Se espera la recarga y despues se re-selecciona el pedido guardado, para que el
            // usuario vea el resultado de su edicion en la lista y en la pestaña General.
            await RecargarListadoAsync();
            SeleccionarFilaPorNumero(pedido.Numero);
        }

        /// <summary>
        /// Descarta el borrador (Nuevo) o los cambios sin grabar (Editar) y vuelve a solo
        /// lectura.
        /// </summary>
        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            // En Editar ya no hay borrador: el pedido existe y lo que se descartan son los
            // cambios de pantalla, por eso el mensaje nombra el pedido y no un borrador.
            string mensaje = _modo == ModoFormulario.Editar
                ? "¿Descartar los cambios hechos al pedido " + uiTextBox1.Text + "?"
                : "¿Descartar el pedido en borrador?";
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

        /// <summary>
        /// Vuelve a cargar el listado de pedidos y recalcula el contador.
        /// </summary>
        private async Task RecargarListadoAsync()
        {
            try
            {
                _dtPedidos = await _pedidoService.LoadDataPedidos();

                // La columna se agrega después de llenar la tabla, así que hay que inicializar
                // las filas: DBNull en la columna checkbox es lo que disparaba FormatException.
                _dtPedidos.Columns.Add(ColumnaSeleccion, typeof(bool));
                foreach (DataRow fila in _dtPedidos.Rows)
                {
                    fila[ColumnaSeleccion] = false;
                }

                // Sin esto el grid seguía mostrando la tabla anterior: la recarga creaba
                // _dtPedidos pero nunca se volvía a enlazar. Los manejadores DataError y
                // CellFormatting (y TrueValue/FalseValue) ya viven en ConfigurarGridPedidos.
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
        /// Selecciona en el listado la fila del numero indicado. Al fijar la celda actual el
        /// grid selecciona la fila y dispara GridPedidos_SelectionChanged, que es el que
        /// recarga la pestaña General con ese pedido. Si el numero no esta visible (filtro de
        /// busqueda activo, por ejemplo) no se hace nada: el formulario queda sin seleccion,
        /// que es el estado normal de Consulta.
        /// </summary>
        private void SeleccionarFilaPorNumero(string numero)
        {
            if (string.IsNullOrWhiteSpace(numero))
            {
                return;
            }

            foreach (DataGridViewRow fila in gridPedidos.Rows)
            {
                if (!string.Equals(
                        fila.Cells["numero"]?.Value?.ToString(),
                        numero,
                        StringComparison.OrdinalIgnoreCase))
                {
                    continue;
                }

                gridPedidos.ClearSelection();
                fila.Selected = true;
                if (fila.Cells["numero"] is DataGridViewCell celda && celda.Visible)
                {
                    gridPedidos.CurrentCell = celda;
                }
                return;
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
        /// Lee una columna de texto de una fila devolviendo null si la columna no existe o
        /// si el valor es NULL. DBNull no debe viajar como cadena vacia: "" y NULL son dos
        /// estados distintos para una columna que acepta NULL.
        /// </summary>
        private static string? ValorFila(DataRow? fila, string columna)
        {
            object? valor = fila == null ? null : Safe(fila, columna);
            return valor == null || valor == DBNull.Value ? null : valor.ToString();
        }

        /// <summary>
        /// Nombre visible de la fila del catalogo que corresponde al valor elegido en un
        /// combo con filtro incremental. Se busca por el id y no por el texto: el texto
        /// tecleado a medias no corresponde a ninguna fila, y persistirlo contra el id que
        /// quedo registrado guardaria un fragmento suelto.
        /// </summary>
        private static string? NombreFilaPorValor(UIComboBox combo, object valor)
        {
            DataRow? fila = FilaPorValor(combo, valor);
            return fila != null && fila.Table.Columns.Contains(combo.DisplayMember)
                ? ValorFila(fila, combo.DisplayMember)
                : null;
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
        /// Arma el pedido con los valores de la pantalla y las lineas del borrador. En Nuevo el
        /// numero no se pone: lo reserva el servicio al guardar. En Editar si se toma de la
        /// pantalla, porque es la clave del UPDATE.
        /// </summary>
        private Pedido ConstruirPedidoDesdeFormulario(DateTime fecha)
        {
            bool editando = _modo == ModoFormulario.Editar;

            Pedido pedido = new()
            {
                // En Nuevo el numero lo reserva el servicio al guardar, con UPDLOCK y dentro de
                // la transaccion: dejarlo vacio aca es lo que permite que un fallo no gaste
                // numero. En Editar es el de la pantalla, que el validador salta pero el
                // servicio necesita para el WHERE del UPDATE.
                Numero = editando ? uiTextBox1.Text?.Trim() ?? string.Empty : string.Empty,
                Fecha = fecha,
                // La fila cargada sin fecha de entrega dejo el picker con un valor heredado
                // (el del pedido anterior o la hora actual): si el usuario no lo toco, se
                // guarda NULL y no una fecha que nadie pidio. Con valor no nulo se sigue
                // escribiendo la fecha sin hora (.Date), como hasta ahora.
                Fecha_entrega = editando
                        && _fechaEntregaCargadaNula
                        && uiDatetimePicker2.Value == _fechaEntregaMostrada
                    ? null
                    : uiDatetimePicker2.Value.Date,
                Estado = string.IsNullOrWhiteSpace(uiTextBox2.Text) ? PedidoEstado.Creado : uiTextBox2.Text.Trim(),
                Direccion_facturacion = TextoOpcional(uiRichTextBox1),
                Direccion_entrega = TextoOpcional(uiRichTextBox2),
                Notas = uiRichTextBox3.Text?.Trim(),
                // En Editar se preserva el flag de la fila cargada: el UPDATE escribe anulado
                // y un false por defecto desharia una anulacion hecha en otro momento.
                Anulado = editando && _filaPedidoActual != null && EsAnulado(_filaPedidoActual),
                Porc_Itbis = PorcItbisActual(),
                // Terminos comerciales. Los combos son opcionales: sin seleccion se guardan como
                // NULL y no como cadena vacia, para no crear un tercer estado invisible al lado
                // de los NULL y de los valores reales.
                //
                // Al editar, un valor guardado que ya no esta en el catalogo deja el combo
                // vacio: en ese caso se persiste el valor que trae la fila, nunca un NULL que
                // borraria un dato que el usuario no toco (spec 4.3: los terminos del pedido
                // tienen que hacer round-trip).
                Tipo_venta = ComboOpcional(cboTipoVenta)
                    ?? (editando ? ValorFila(_filaPedidoActual, "tipo_venta") : null),
                Condiciones_pago = ComboOpcional(cboCondicionesPago)
                    ?? (editando ? ValorFila(_filaPedidoActual, "condiciones_pago") : null),
                Prioridad = ComboOpcional(cbo_prioridad)
                    ?? (editando ? ValorFila(_filaPedidoActual, "prioridad") : null),
                Persona_Contacto = TextoOpcional(txtPersonaContacto),
                Detalle = new List<PedidoDetalle>(_lineas)
            };

            object? valorCliente = ValorCombo(cbo_customers);
            if (valorCliente is not null && Guid.TryParse(valorCliente.ToString(), out Guid clienteId))
            {
                pedido.Customer_Id = clienteId;
                // El nombre no se toma del texto del combo: escribiendo un fragmento sin
                // elegir de la lista, _valoresSel conserva el id anterior y se guardaria
                // ese fragmento contra ese id. Se usa la fila del catalogo que corresponde
                // al id elegido; si el id ya no figura (cliente anulado despues de guardado),
                // el nombre de la fila cargada.
                pedido.Customer_Name = NombreFilaPorValor(cbo_customers, clienteId)
                    ?? (editando ? ValorFila(_filaPedidoActual, "customer_name") : null)
                    ?? TextoOpcional(cbo_customers.Text);
            }
            else if (editando && Guid.TryParse(ValorFila(_filaPedidoActual, "customer_id"), out Guid clienteFila))
            {
                // El combo quedo sin valor (por ejemplo un cliente anulado despues de
                // guardar el pedido, que ya no figura en el catalogo): se conserva el
                // cliente de la fila cargada en vez de guardar el pedido sin cliente ni
                // nombre, que el validador rechazaria sin que el usuario pueda resolverlo.
                pedido.Customer_Id = clienteFila;
                pedido.Customer_Name = ValorFila(_filaPedidoActual, "customer_name")
                    ?? TextoOpcional(cbo_customers.Text);
            }

            object? valorVendedor = ValorCombo(uiComboBox1);
            if (valorVendedor is not null && Guid.TryParse(valorVendedor.ToString(), out Guid vendedorId))
            {
                pedido.Vendor_Id = vendedorId;
            }
            else if (editando && Guid.TryParse(ValorFila(_filaPedidoActual, "vendor_id"), out Guid vendedorFila))
            {
                // Mismo criterio que el cliente: un vendedor anulado despues de guardar
                // desaparece del catalogo y el combo queda vacio. Sin este respaldo el
                // UPDATE escribiria un NULL silencioso en un campo que el usuario nunca toco.
                pedido.Vendor_Id = vendedorFila;
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

            // No hay pedido en pantalla: se suelta la fila, btnEditar se apaga (su estado
            // refleja la seleccion de la lista) y el switch vuelve a "Pedido Activo" y
            // deshabilitado, con la guardia para no escribir en la base al hacerlo.
            _filaPedidoActual = null;
            btnEditar.Enabled = false;
            _actualizandoSwitchAnulado = true;
            sw_anular_pedido.Active = true;
            _actualizandoSwitchAnulado = false;
            sw_anular_pedido.Enabled = false;

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

        /// <summary>
        /// Maneja el clic del botón Enviar Dispositivo.
        /// Muestra los pedidos seleccionados y limpia la selección.
        /// </summary>
        private void BtnEnviarDispositivo_Click(object? sender, EventArgs e)
        {
            List<string> numeros = NumerosPedidosSeleccionados();
            if (numeros.Count == 0)
            {
                MessageBox.Show("No hay pedidos seleccionados.", "Enviar Dispositivo",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            StringBuilder sb = new StringBuilder();
            sb.AppendLine("Pedidos enviados al dispositivo:");
            foreach (string num in numeros)
            {
                sb.AppendLine($"  - {num}");
            }

            MessageBox.Show(sb.ToString(), "Enviar Dispositivo",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            MarcarTodosSeleccionados(false);
        }
    }
}
