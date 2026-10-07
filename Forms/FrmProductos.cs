using System.ComponentModel;
using System.Data;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.InventarioService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProductsService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ReportsService.ReportsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Productos (rediseño 30/70).
    /// Panel izquierdo (30%): buscador, grid con product_id / product_name / tipo y contador de productos.
    /// Panel derecho (70%): barra de acciones (Nuevo | Editar) y pestañas de detalle del producto seleccionado.
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

        /// <summary>
        /// Fondo de las filas de productos anulados (desactivados): rojo vivo #D02020, más
        /// saturado y brillante que el Firebrick del resto de módulos, para que el estado se
        /// resalte de un vistazo en el listado de productos sin tener que leer el detalle.
        /// </summary>
        private static readonly Color FondoAnulado = Color.FromArgb(208, 32, 32);

        /// <summary>
        /// Letra de una fila desactivada: rosa claro, más clara que el texto normal, para que
        /// se lea sobre <see cref="FondoAnulado"/> y no se funda con el rojo del fondo.
        /// </summary>
        private static readonly Color TextoAnulado = Color.FromArgb(255, 214, 214);

        /// <summary>
        /// Fondo de la fila desactivada CUANDO ESTA SELECCIONADA. La seleccion del modulo es
        /// negra con letras blancas, y ese negro tapaba justo el rojo del producto anulado:
        /// como para ver un desactivado hay que seleccionarlo, el estado quedaba invisible
        /// justo cuando interesaba. Aqui la seleccion se pinta en rojo oscuro con la misma
        /// letra clara, de modo que el fondo rojo de la fila sigue notandose.
        /// </summary>
        private static readonly Color SeleccionAnulado = Color.DarkRed;

        /// <summary>Color del texto normal del grid.</summary>
        private static readonly Color GrisFuerte = Color.FromArgb(48, 48, 48);

        /// <summary>Borde suave de las cajas de solo lectura del detalle.</summary>
        private static readonly Color ColorBordeCampo = Color.FromArgb(180, 210, 180);

        private readonly IProductsService _productsService;
        private readonly IExportDataService _exportDataService;
        private readonly IReportsService _reportsService;
        private readonly IConfiguration _configuration;
        private readonly IConsecutivosService _consecutivosService;
        private readonly IProductsImportService _productsImportService;
        private readonly IInventarioService _inventarioService;

        /// <summary>Catalogo completo en memoria (fuente de verdad del buscador y del contador).</summary>
        private List<Product> _productos = new();

        /// <summary>Codigo del producto seleccionado, para conservarlo tras filtrar.</summary>
        private string? _idSeleccionado;

        /// <summary>
        /// Codigo cuyos masters ya estan cargados en el grid de la pestana Inventario. Sirve
        /// de cache para no volver a consultar la base cada vez que se cambia de pestana con
        /// el mismo producto a la vista; se invalida al cambiar de fila activa.
        /// </summary>
        private string? _inventarioCargadoPara;

        /// <summary>
        /// Crea el formulario de productos.
        /// </summary>
        /// <param name="productsService">Servicio del catalogo de productos.</param>
        /// <param name="exportDataService">Servicio de exportacion a Excel, para la hoja de productos.</param>
        /// <param name="reportsService">Servicio de reportes.</param>
        /// <param name="configuration">Configuracion de la aplicacion.</param>
        /// <param name="consecutivosService">Servicio para generar IDs consecutivos de productos.</param>
        /// <param name="productsImportService">Servicio de importacion de productos desde Excel.</param>
        /// <param name="inventarioService">Servicio de inventario, para la pestana Inventario.</param>
        public FrmProductos(IProductsService productsService, IExportDataService exportDataService, IReportsService reportsService, IConfiguration configuration, IConsecutivosService consecutivosService, IProductsImportService productsImportService, IInventarioService inventarioService)
        {
            ArgumentNullException.ThrowIfNull(productsService);
            ArgumentNullException.ThrowIfNull(exportDataService);
            ArgumentNullException.ThrowIfNull(reportsService);
            ArgumentNullException.ThrowIfNull(configuration);
            ArgumentNullException.ThrowIfNull(consecutivosService);
            ArgumentNullException.ThrowIfNull(productsImportService);
            ArgumentNullException.ThrowIfNull(inventarioService);

            InitializeComponent();

            _productsService = productsService;
            _exportDataService = exportDataService;
            _reportsService = reportsService;
            _configuration = configuration;
            _consecutivosService = consecutivosService;
            _productsImportService = productsImportService;
            _inventarioService = inventarioService;

            components ??= new Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            EstilarCamposDetalle();
            EstilarFiltroCategoria();
            EstilarSeleccionGrid();
            EstilarPestanaInventario();
            ConfigurarGridMasters();
            ConfigurarEventos();
            ActualizarContador();
            LimpiarDetalle();

            // El formulario nace en consulta: el detalle se ve bloqueado y Guardar/Cancelar
            // quedan ocultos. Sin esta llamada, los controles arrancarían editables.
            AplicarModo(ModoFormulario.Consulta);
            ActualizarBotonesBarra();
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
        /// <param name="seleccionarDespues">
        /// Codigo que debe quedar seleccionado al terminar. Se fija antes de filtrar porque
        /// <see cref="AplicarFiltro"/> termina en <see cref="RestaurarSeleccion"/>, que si no
        /// encuentra el codigo se queda con la primera fila: tras guardar, sin esto, el usuario
        /// no veria el producto que acaba de crear. null deja el comportamiento de siempre.
        /// </param>
        private async Task CargarCatalogoAsync(string? seleccionarDespues = null)
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

            _idSeleccionado = seleccionarDespues;
            AplicarFiltro();
        }

        /// <summary>
        /// Enlaza los eventos de la UI y evita que el grid genere columnas automaticas.
        /// </summary>
        private void ConfigurarEventos()
        {
            gridProductos.AutoGenerateColumns = false;
            gridProductos.SelectionChanged += GridProductos_SelectionChanged;
            gridProductos.CurrentCellChanged += GridProductos_CurrentCellChanged;
            gridProductos.CellFormatting += GridProductos_CellFormatting;
            txtSearch.TextChanged += TxtSearch_TextChanged;

            // Filtro de categoria: los cuatro radios, con el mismo criterio de "segundo clic
            // quita el filtro" que explico en FiltroCategoria_Click.
            foreach (UIRadioButton radio in RadiosFiltroCategoria())
            {
                radio.Click += FiltroCategoria_Click;
            }

            // La barra de acciones se cablea aqui y no en el diseniador: los handlers son
            // privados del formulario y el Visual Studio no puede engancharlos.
            btnNuevoProducto.Click += BtnNuevoProducto_Click;
            btnEditarProducto.Click += BtnEditarProducto_Click;
            btnExportarProducto.Click += BtnExportarProducto_Click;
            btnImportarCatalogo.Click += BtnImportarCatalogo_Click;
            btnReporteProducto.Click += BtnReporteProducto_Click;
            btnGuardarProducto.Click += BtnGuardarProducto_Click;
            btnCancelarProducto.Click += BtnCancelarProducto_Click;

            // Pestaña Inventario: el cambio de pestaña dispara la carga, y los dos pintados
            // del grid de masters se cablean aqui y no en el diseñador, por el mismo motivo
            // que la barra de acciones (los handlers son privados del formulario).
            tabDetalle.SelectedIndexChanged += TabDetalle_SelectedIndexChanged;
            gridMasters.CellFormatting += GridMasters_CellFormatting;
            gridMasters.CellPainting += GridMasters_CellPainting;
        }

        /// <summary>
        /// Estado del formulario. Consulta es el estado por defecto: el detalle se ve pero no se
        /// escribe. Solo Nuevo y Editar habilitan la escritura, cada uno con sus permisos.
        /// Es el mismo patron que usa FrmPedidos para el modulo de pedidos.
        /// </summary>
        private enum ModoFormulario
        {
            Consulta,
            Nuevo,
            Editar
        }

        private ModoFormulario _modo = ModoFormulario.Consulta;

        /// <summary>
        /// True cuando el formulario admite escritura: Nuevo crea un producto y Editar modifica
        /// el seleccionado. Consulta es el unico estado de solo lectura.
        /// </summary>
        private bool EsEditable => _modo is ModoFormulario.Nuevo or ModoFormulario.Editar;

        /// <summary>
        /// El interruptor de estado solo se mueve (y solo se ve) al EDITAR un producto que ya
        /// existe. En el alta queda OCULTO porque un producto se crea siempre vigente: nace
        /// Activo y se desactiva despues, desde Editar, si hace falta. En consulta se ve, pero
        /// bloqueado, para mostrar el estado del producto seleccionado.
        /// Vive en su propia propiedad (y no como "!editable" en AplicarModo) porque lo
        /// consultan DOS sitios: AplicarModo y PintarEstado. Con la regla duplicada, PintarEstado
        /// se ejecutaba despues de AplicarModo en el alta y volvia a dejar el switch editable.
        /// </summary>
        private bool EstadoEsEditable => _modo is ModoFormulario.Editar;

        /// <summary>
        /// Codigo del producto que se esta editando. Vive aparte de <see cref="_idSeleccionado"/>
        /// porque mientras se edita el usuario puede seguir moviendose por el grid: el detalle no
        /// debe cambiar bajo sus pies, y al guardar hay que saber sobre que producto se guardo
        /// aunque la fila activa sea otra.
        /// </summary>
        private string? _idEnEdicion;

        /// <summary>Codigo del producto que se esta dando de alta (solo en modo Nuevo).</summary>
        private string? _idEnAlta;

        /// <summary>
        /// Evita que un doble clic en Guardar dispare dos escrituras: el segundo clic llega
        /// mientras el primero sigue esperando al servicio.
        /// </summary>
        private bool _guardando;

        /// <summary>
        /// Unico metodo que decide que se puede escribir. AplicarModo es el unico sitio que toca
        /// ReadOnly, Enabled y Visible, para que no queden dos reglas de editabilidad que se
        /// pisen entre si. En Consulta deja el detalle entero bloqueado, igual que antes de
        /// existir el modo de edicion.
        /// </summary>
        private void AplicarModo(ModoFormulario modo)
        {
            _modo = modo;
            bool editable = EsEditable;

            // El consecutivo (IdConsec) lo genera el sistema (contador PROD) y es solo
            // referencial: siempre queda fijo. El codigo Ritrama ES el product_id, la
            // identidad del producto; lo teclea el usuario y es editable en Nuevo y en
            // Editar, y si cambia en edicion el servicio propaga el nuevo codigo a las
            // tablas que lo referencian (inventario, ordenes de corte, ...).
            txtDetId.ReadOnly = true;

            txtDetCodigoRitrama.ReadOnly = !editable;
            txtDetNombre.ReadOnly = !editable;
            txtDetReferencia.ReadOnly = !editable;
            txtDetCodebar.ReadOnly = !editable;
            txtDetPrecio.ReadOnly = !editable;
            txtDetCosto.ReadOnly = !editable;
            txtDetRatio.ReadOnly = !editable;
            txtDetDescripcion.ReadOnly = !editable;

            // ReadOnly y no Enabled=false (mismo criterio que los radios ya existentes):
            // permite pintar Checked / Active por codigo al mostrar un producto, y solo corta
            // el clic del usuario.
            foreach (UIRadioButton radio in RadiosTipo())
            {
                radio.ReadOnly = !editable;
            }

            swDetEstado.ReadOnly = !EstadoEsEditable;

            // En el alta el interruptor NO SE MUESTRA. No es que este bloqueado: un producto
            // nuevo nace siempre vigente, asi que el control no tiene nada que decidir y
            // enseñarlo invites a pensar que se puede dar de alta desactivado. Se oculta tambien
            // su etiqueta, para no dejar un rotulo sin nada al lado. Al entrar a Editar vuelve a
            // aparecer, que es donde si tiene sentido activar o desactivar.
            swDetEstado.Visible = modo is not ModoFormulario.Nuevo;
            lblDetEstado.Visible = modo is not ModoFormulario.Nuevo;

            // El filtro se USA en consulta, asi que va habilitado mientras no se esta
            // escribiendo. Solo se bloquea en Nuevo y Editar, para que el listado no cambie
            // debajo del formulario mientras se guarda (la fila abierta podria desaparecer).
            foreach (UIRadioButton filtro in RadiosFiltroCategoria())
            {
                filtro.Enabled = !editable;
            }

            // En escritura se ocultan Nuevo, Editar y Exportar para que un clic perdido no tire
            // el borrador, y aparecen Guardar y Cancelar, que solo existen en estos dos modos.
            btnNuevoProducto.Visible = !editable;
            btnEditarProducto.Visible = !editable;
            btnImportarCatalogo.Visible = !editable;
            btnExportarProducto.Visible = !editable;
            btnReporteProducto.Visible = !editable;
            btnGuardarProducto.Visible = editable;
            btnCancelarProducto.Visible = editable;
            btnGuardarProducto.Enabled = editable;
            btnCancelarProducto.Enabled = editable;
        }

        /// <summary>
        /// Muestra en la pestana de detalle el producto de la fila activa.
        /// Se engancha a los dos eventos del grid porque el SelectionChanged se difiere al siguiente
        /// ciclo de mensajes y el CurrentCellChanged no: con una sola via, el detalle (y "Editar")
        /// quedarian un paso atras de la fila que el usuario acaba de tocar.
        /// </summary>
        private void GridProductos_CurrentCellChanged(object? sender, EventArgs e) => RefrescarDetalleDeLaFilaActiva();

        /// <summary>Ver <see cref="GridProductos_CurrentCellChanged"/>: mismo trabajo, otra via de aviso.</summary>
        private void GridProductos_SelectionChanged(object? sender, EventArgs e) => RefrescarDetalleDeLaFilaActiva();

        /// <summary>
        /// Unico camino que vuelca la fila activa en la pestaña de detalle y en la barra de acciones.
        /// Si el producto de la fila no esta en el catalogo en memoria, el detalle se limpia en vez
        /// de dejar a la vista los datos del producto anterior.
        /// Mientras se esta escribiendo (Nuevo o Editar) no se toca el detalle: el grid sigue
        /// siendo navegable, pero recargar sus campos dejaria al usuario corrigiendo los datos
        /// de un producto sobre la pantalla de otro, y Guardar los guardaria como el equivocado.
        /// </summary>
        private void RefrescarDetalleDeLaFilaActiva()
        {
            if (EsEditable)
            {
                return;
            }

            Product? producto = ProductoSeleccionado();
            _idSeleccionado = producto?.Product_id;

            MostrarDetalle(producto);
            ActualizarBotonesBarra(producto);

            // Si la pestana Inventario esta abierta, lo que hay que ver en ella es el
            // producto de la fila activa: se invalida la cache para que vuelva a consultar.
            // _idSeleccionado ya esta actualizado mas arriba; el caso sin producto tambien
            // llega aqui, porque MostrarDetalle(null) pasa por LimpiarDetalle.
            if (tabDetalle.SelectedTab == tabInventarioProducto)
            {
                _inventarioCargadoPara = null;
                _ = RefrescarInventarioAsync();
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Pestaña Inventario (masters del producto seleccionado)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Cambio de pestaña: al entrar en Inventario se cargan los masters del producto que
        /// está activo en el grid. Con el servicio real la consulta va a la base; con los
        /// stubs de las pruebas la tarea viene ya resuelta, así que el cuerpo termina de forma
        /// síncrona y se puede afirmar justo después de cambiar de pestaña.
        /// </summary>
        private async void TabDetalle_SelectedIndexChanged(object? sender, EventArgs e)
        {
            if (tabDetalle.SelectedTab == tabInventarioProducto)
            {
                await RefrescarInventarioAsync();
            }
        }

        /// <summary>
        /// Carga en <see cref="gridMasters"/> los masters del producto activo, o el mensaje
        /// de vacío si no hay producto o no hay filas. El resultado se cachea en
        /// <see cref="_inventarioCargadoPara"/> para no martillar la base al reentrar en la
        /// pestaña con el mismo producto.
        /// </summary>
        private async Task RefrescarInventarioAsync()
        {
            if (tabDetalle.SelectedTab != tabInventarioProducto)
            {
                return;
            }

            string? codigo = _idSeleccionado;

            // El try envuelve el cuerpo entero: una excepción sin capturar en un async void
            // (TabDetalle_SelectedIndexChanged) mataría la aplicación.
            try
            {
                if (string.IsNullOrWhiteSpace(codigo))
                {
                    // Sin producto seleccionado no se llama al servicio.
                    MostrarInventarioVacio();
                    _inventarioCargadoPara = null;
                    return;
                }

                if (_inventarioCargadoPara == codigo)
                {
                    return;
                }

                DataTable? masters = await _inventarioService.BuscarMastersDeProducto(codigo);

                if (masters is null || masters.Rows.Count == 0)
                {
                    MostrarInventarioVacio();
                    return;
                }

                lblInventarioVacio.Visible = false;
                gridMasters.DataSource = masters.DefaultView;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar el inventario del producto: " + ex.Message);
                MostrarInventarioVacio();
            }
            finally
            {
                // Se marca como cargado también cuando falló: si no, el siguiente cambio de
                // pestaña reintentaría en bucle sobre el mismo error.
                if (!string.IsNullOrWhiteSpace(codigo))
                {
                    _inventarioCargadoPara = codigo;
                }
            }
        }

        /// <summary>
        /// Único mensaje de estado vacío de la pestaña Inventario (diseño aprobado: sin
        /// estados diferenciados): se vacía el grid y se muestra la etiqueta con el texto
        /// fijo de "sin masters".
        /// </summary>
        private void MostrarInventarioVacio()
        {
            gridMasters.DataSource = null;
            lblInventarioVacio.Text = "Este producto no tiene masters en inventario";
            lblInventarioVacio.Visible = true;
        }

        /// <summary>
        /// Nueve columnas del grid de masters, en el mismo orden que GridMaster de
        /// Frm_Inventarios. La columna "% Disponible" no existe en el resultado del SQL: es
        /// una columna visual que se pinta en <see cref="GridMasters_CellPainting"/>.
        /// </summary>
        private void ConfigurarGridMasters()
        {
            gridMasters.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("roll_id", 100, "Rollid", "roll_id", gridMasters);
            CommonService.ADD_COLUMN_GRID("width", 80, "Width", "width", gridMasters);
            CommonService.ADD_COLUMN_GRID("length", 80, "Length", "lenght", gridMasters);
            CommonService.ADD_COLUMN_GRID("length_consumido", 90, "Consumido", "largo_consumido", gridMasters);
            CommonService.ADD_COLUMN_GRID("length_restante", 90, "Restante", "largo_restante", gridMasters);
            CommonService.ADD_COLUMN_GRID("pct_disponible", 120, "% Disponible", "pct_disponible", gridMasters);
            CommonService.ADD_COLUMN_GRID("tipo_mov", 90, "Origen", "tipo_mov", gridMasters);
            CommonService.ADD_COLUMN_GRID("estado", 150, "Estado", "estado", gridMasters);
            CommonService.ADD_COLUMN_GRID("documento_oc", 140, "Documento OC", "documento_oc", gridMasters);
        }

        /// <summary>
        /// Pinta la columna "estado" del grid de masters con el mismo criterio que
        /// GridMaster_CellFormatting de Frm_Inventarios: Agotado y Desperdicio en rojo,
        /// Completo en verde y Parcialmente Consumido en naranja, siempre con letra blanca.
        /// Aquí no se relanza la excepción del catch: un fallo al leer el valor se queda en
        /// el color normal de la celda.
        /// </summary>
        private void GridMasters_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0
                || e.ColumnIndex < 0
                || gridMasters.Columns[e.ColumnIndex].Name != "estado")
            {
                return;
            }

            string estado = Convert.ToString(e.Value) ?? string.Empty;

            if (estado == "Desperdicio" || estado == "Agotado")
            {
                e.CellStyle.BackColor = Color.Red;
                e.CellStyle.ForeColor = Color.White;
            }

            if (estado == "Completo")
            {
                e.CellStyle.BackColor = Color.Green;
                e.CellStyle.ForeColor = Color.White;
            }

            if (estado == "Parcialmente Consumido")
            {
                e.CellStyle.BackColor = Color.Orange;
                e.CellStyle.ForeColor = Color.White;
            }
        }

        /// <summary>
        /// Pinta la columna "% Disponible" como barra de progreso con color según el
        /// porcentaje (GridMaster_CellPainting de Frm_Inventarios). El porcentaje sale de
        /// "length" y "length_restante" de la propia fila, no de una columna del SQL.
        /// </summary>
        private void GridMasters_CellPainting(object? sender, DataGridViewCellPaintingEventArgs e)
        {
            if (e.RowIndex < 0 || e.ColumnIndex < 0)
            {
                return;
            }

            if (gridMasters.Columns[e.ColumnIndex].Name != "pct_disponible")
            {
                return;
            }

            DataGridViewRow fila = gridMasters.Rows[e.RowIndex];
            if (fila.IsNewRow)
            {
                return;
            }

            double pct = ProgressBarRenderer.CalcularDesdeFila(fila, "length", "length_restante");
            ProgressBarRenderer.PaintCell(e.Graphics, e.CellBounds, e.State, pct, e.CellStyle,
                (e.State & DataGridViewElementStates.Selected) != 0);
            e.Handled = true;
        }

        /// <summary>
        /// Aplica fuente, color y solo lectura a los controles de la pestaña de detalle.
        /// Se hace aqui (y no en el diseniador) porque el UIStyleManager del constructor re-estiliza
        /// el form despues de InitializeComponent y pisaria los colores puestos en el diseniador.
        /// </summary>
        private void EstilarCamposDetalle()
        {
            UILabel[] etiquetas =
            [
                lblDetId, lblDetCodigoRitrama, lblDetNombre, lblDetTipo, lblDetReferencia,
                lblDetCodebar, lblDetPrecio, lblDetCosto, lblDetRatio, lblDetEstado, lblDetDescripcion
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
                txtDetId, txtDetCodigoRitrama, txtDetNombre, txtDetReferencia,
                txtDetCodebar, txtDetPrecio, txtDetCosto, txtDetRatio, txtDetDescripcion
            ];

            foreach (UITextBox campo in campos)
            {
                // ReadOnly no se fija aqui: lo gobierna AplicarModo, que es el unico sitio que
                // decide si el formulario admite escritura. Este metodo solo deja el aspecto.
                campo.FillColor = Color.White;
                campo.RectColor = ColorBordeCampo;
                campo.Font = new Font("JetBrains Mono", 9F);
                campo.TextAlignment = ContentAlignment.MiddleLeft;
            }

            foreach (UIRadioButton radio in RadiosTipo())
            {
                radio.Font = new Font("JetBrains Mono", 9F);
                radio.ForeColor = GrisTexto;
                radio.RadioButtonColor = Verde;

                // ReadOnly (no Enabled=false) porque el grupo solo muestra la categoria del producto
                // mientras el formulario este en consulta: ReadOnly corta el clic del usuario pero
                // deja asignar Checked por codigo, que es como se pinta el producto seleccionado.
                // En Nuevo y Editar se habilitan (ver AplicarModo).
            }

            EstilarSwitchEstado();
        }

        /// <summary>
        /// Estilo de los cuatro radios del filtro de categoria. Se hace aqui (y no en el
        /// disenador) por el mismo motivo que EstilarCamposDetalle: el UIStyleManager del
        /// constructor re-estiliza el form despues de InitializeComponent y pisaria los colores
        /// puestos en el disenador. No se fija ReadOnly porque el filtro se usa siempre, tambien
        /// en consulta: filtrar la lista no es escribir sobre ningun producto.
        /// </summary>
        private void EstilarFiltroCategoria()
        {
            foreach (UIRadioButton radio in RadiosFiltroCategoria())
            {
                radio.Font = new Font("JetBrains Mono", 8.5F);
                radio.ForeColor = GrisTexto;
                radio.RadioButtonColor = Verde;
            }
        }

        /// <summary>
        /// El estado del producto es un UISwitch con los dos estados del dominio: Activo y
        /// Desactivado (que en la base es anulado = 1). Solo se ve y se mueve en Editar: en el
        /// alta esta oculto porque un producto nuevo nace siempre Activo (ver AplicarModo), y en
        /// consulta se ve bloqueado para mostrar el estado del producto seleccionado.
        /// ReadOnly y no Enabled=false por lo mismo que los radios: permite pintar Active por codigo.
        /// </summary>
        private void EstilarSwitchEstado()
        {
            swDetEstado.Font = new Font("JetBrains Mono", 9F);
            swDetEstado.ForeColor = GrisTexto;
            swDetEstado.ActiveColor = Verde;
            swDetEstado.ActiveText = "Activo";
            swDetEstado.InActiveText = "Desactivado";

            // ReadOnly y Visible no se fijan aqui: los gobierna AplicarModo y PintarEstado, que
            // comparten las reglas de EstadoEsEditable y del modo actual.
        }

        /// <summary>
        /// Pinta el estado del producto en el switch (true = Activo, false = Desactivado).
        /// SunnyUI hace que <see cref="UISwitch.Active"/> ignore el setter cuando el control
        /// es ReadOnly, asi que aqui se levanta ReadOnly solo mientras se asigna. Al terminar se
        /// restituye la regla del interruptor (<see cref="EstadoEsEditable"/>), que es la misma
        /// que aplica AplicarModo: de otro modo, este metodo se ejecutaria despues y dejaria
        /// el switch editable en el alta, que es justo lo que no debe pasar.
        /// </summary>
        /// <param name="activo">true si el producto esta vigente, false si esta anulado.</param>
        private void PintarEstado(bool activo)
        {
            swDetEstado.ReadOnly = false;
            swDetEstado.Active = activo;
            swDetEstado.ReadOnly = !EstadoEsEditable;
        }

        /// <summary>
        /// La fila seleccionada del grid se pinta negro con letras blancas.
        /// Se hace aqui (y no en el diseniador) porque el UIStyleManager del constructor re-estiliza
        /// el form despues de InitializeComponent y pisaria los colores puestos en el diseniador.
        /// </summary>
        private void EstilarSeleccionGrid()
        {
            gridProductos.DefaultCellStyle.SelectionBackColor = Color.Black;
            gridProductos.DefaultCellStyle.SelectionForeColor = Color.White;
            gridProductos.RowsDefaultCellStyle.SelectionBackColor = Color.Black;
            gridProductos.RowsDefaultCellStyle.SelectionForeColor = Color.White;
        }

        /// <summary>
        /// Aspecto de la pestaña Inventario. Se hace aqui (y no en el diseniador) por el mismo
        /// motivo que <see cref="EstilarCamposDetalle"/>: el UIStyleManager del constructor
        /// re-estiliza el formulario despues de InitializeComponent y pisaria los colores
        /// puestos en el diseniador.
        /// </summary>
        private void EstilarPestanaInventario()
        {
            // La etiqueta de vacio va con la letra del resto del detalle.
            lblInventarioVacio.Font = new Font("JetBrains Mono", 9F);
            lblInventarioVacio.ForeColor = GrisTexto;

            // El grid, fondo blanco y cabecera verde con letra blanca, como gridProductos:
            // las dos pestanas tienen que leerse como la misma aplicacion.
            gridMasters.Font = new Font("Microsoft Sans Serif", 9F);
            gridMasters.BackgroundColor = Color.White;
            gridMasters.GridColor = ColorBordeCampo;
            gridMasters.EnableHeadersVisualStyles = false;
            gridMasters.ColumnHeadersDefaultCellStyle.BackColor = Verde;
            gridMasters.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridMasters.ColumnHeadersDefaultCellStyle.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            gridMasters.DefaultCellStyle.SelectionBackColor = Color.Black;
            gridMasters.DefaultCellStyle.SelectionForeColor = Color.White;
            gridMasters.RowsDefaultCellStyle.SelectionBackColor = Color.Black;
            gridMasters.RowsDefaultCellStyle.SelectionForeColor = Color.White;
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

            foreach (UIRadioButton radio in RadiosTipo())
            {
                radio.RadioButtonColor = Verde;
            }

            foreach (UIRadioButton radio in RadiosFiltroCategoria())
            {
                radio.RadioButtonColor = Verde;
            }

            Invalidate();

            EstilarSeleccionGrid();

            // La cabecera del grid de la pestana Inventario se repite aqui: el estilo global
            // de Main la re-tenie igual que la del listado de productos.
            EstilarPestanaInventario();
        }

        /// <summary>
        /// Filtra el catalogo cada vez que cambia el texto del buscador.
        /// </summary>
        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            AplicarFiltro();
        }

        /// <summary>
        /// Pinta en rojo con letra clara las filas de productos anulados (desactivados), con
        /// el mismo criterio que Clientes, Proveedores y Vendedores: el estado se ve de un
        /// vistazo en el listado sin tener que abrir el detalle.
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
            e.CellStyle.BackColor = FondoAnulado;
            e.CellStyle.ForeColor = TextoAnulado;
            e.CellStyle.SelectionBackColor = SeleccionAnulado;
            e.CellStyle.SelectionForeColor = TextoAnulado;
        }

        /// <summary>
        /// Reconstruye las filas visibles del grid. Hay dos filtros y se aplican ENCADENADOS:
        /// primero la categoria (los cuatro radios) y despues el texto del buscador sobre lo que
        /// queda. Asigna el filtro de texto a la categoria antes de filtrar, para que buscar
        /// "papel" dentro de "Hojas" no devuelva los Masters que tambien se llamen papel.
        /// Conserva la seleccion previa cuando el producto sigue visible.
        /// El nombre de la categoria sale de ProductCategoryRules (misma logica que el CASE de R.cs).
        /// </summary>
        private void AplicarFiltro()
        {
            string texto = (txtSearch.Text ?? string.Empty).Trim();

            IEnumerable<Product> consulta = _productos.Where(CoincideCategoria);
            if (texto.Length > 0)
            {
                consulta = consulta.Where(p => CoincideTexto(p, texto));
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
            ActualizarBotonesBarra();
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

            txtDetId.Text = producto.IdConsec.ToString();
            txtDetCodigoRitrama.Text = producto.Product_id ?? string.Empty;
            txtDetNombre.Text = producto.Product_Name ?? string.Empty;
            MarcarTipo(producto);
            txtDetReferencia.Text = producto.Referencia ?? string.Empty;
            txtDetCodebar.Text = producto.Codigo_Barra ?? string.Empty;
            txtDetPrecio.Text = producto.Precio.ToString("N2", CultureInfo.InvariantCulture);
            txtDetCosto.Text = producto.Costo.ToString("N2", CultureInfo.InvariantCulture);
            txtDetRatio.Text = producto.Ratio.ToString("N4", CultureInfo.InvariantCulture);
            // El switch va al reves del bit: Activo = vigente, Desactivado = anulado.
            PintarEstado(!producto.Anulado);
            txtDetDescripcion.Text = producto.Product_Description ?? string.Empty;
        }

        /// <summary>
        /// Deja vacios todos los campos de la pestaña de detalle.
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

            // El estado no es una caja de texto: se reinicia a Desactivado para no dejar
            // encendera la fila anterior cuando ya no hay producto seleccionado.
            PintarEstado(false);

            DesmarcarTipo();
        }

        /// <summary>
        /// Los cuatro radio de categoria, en el orden en que se pintan en la pestaña de detalle.
        /// </summary>
        private UIRadioButton[] RadiosTipo() =>
        [
            rbTipoMaster, rbTipoRolloCortado, rbTipoHojas, rbTipoGraphics
        ];

        /// <summary>
        /// Marca el radio de la categoria del producto. Los cuatro bits son excluyentes
        /// (los valida ProductCategoryRules), asi que como mucho queda uno marcado; un producto
        /// sin categoria deja los cuatro sin marcar en vez de inventar una.
        /// </summary>
        /// <param name="producto">Producto mostrado en el detalle.</param>
        private void MarcarTipo(Product producto)
        {
            ArgumentNullException.ThrowIfNull(producto);

            rbTipoMaster.Checked = producto.Master;
            rbTipoRolloCortado.Checked = producto.RolloCortado;
            rbTipoHojas.Checked = producto.Hoja;
            rbTipoGraphics.Checked = producto.Graphics;
        }

        /// <summary>Deja el grupo de tipo sin ninguna categoria marcada.</summary>
        private void DesmarcarTipo()
        {
            foreach (UIRadioButton radio in RadiosTipo())
            {
                radio.Checked = false;
            }
        }

        /// <summary>
        /// Actualiza la franja verde. "Productos existentes" y "Total" son del catalogo entero
        /// (no cambian al filtrar, para no perder la referencia de cuanto hay), pero "Mostrando" y
        /// "Anulados" SI cuentan lo que se ve ahora mismo, que es lo que el usuario quiere saber
        /// al filtrar. Cuando hay filtro de categoria se indica cual es, para que quede claro por
        /// que la lista es mas corta.
        /// </summary>
        private void ActualizarContador()
        {
            int total = _productos.Count;
            int anulados = _productos.Count(p => p.Anulado);
            int existentes = total - anulados;
            int mostrando = gridProductos.Rows.Count;

            // Anulados de lo que se ve, no del catalogo: con un filtro activo son otros numeros.
            int anuladosVisibles = gridProductos.Rows
                .Cast<DataGridViewRow>()
                .Count(r => r.DataBoundItem is FilaProducto fila && fila.Anulado);

            string categoria = CategoriaFiltrada();

            lblContador.Text = $"Productos existentes: {existentes}";
            lblContadorDetalle.Text = categoria.Length > 0
                ? $"{categoria}: Mostrando {mostrando}  -  Anulados {anuladosVisibles}  -  Total {total}"
                : $"Mostrando {mostrando}  -  Anulados {anuladosVisibles}  -  Total {total}";
        }

        /// <summary>
        /// Producto de la fila activa del catalogo, o null si no hay ninguna fila seleccionada.
        /// </summary>
        private Product? ProductoSeleccionado()
        {
            if (gridProductos.CurrentRow?.DataBoundItem is not FilaProducto fila)
            {
                return null;
            }

            return _productos.FirstOrDefault(p =>
                string.Equals(p.Product_id, fila.ProductId, StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Refleja en la barra de acciones lo que permite la seleccion actual. "Editar" se enciende
        /// con cualquier producto seleccionado, tambien anulado: reactivarlo es precisamente una
        /// de las operaciones validas (ver ProductValidator.ValidateEditableState), asi que el
        /// filtro por anulado no lo bloquea. Mientras se esta escribiendo no se enciende nada,
        /// porque la barra muestra Guardar y Cancelar. "Nuevo" no depende de la seleccion, por eso
        /// nace habilitado.
        /// </summary>
        private void ActualizarBotonesBarra() => ActualizarBotonesBarra(ProductoSeleccionado());

        /// <summary>Ver <see cref="ActualizarBotonesBarra()"/>; esta sobrecarga evita releer la seleccion.</summary>
        /// <param name="producto">Producto de la fila activa, o null si no hay ninguna.</param>
        private void ActualizarBotonesBarra(Product? producto)
            => btnEditarProducto.Enabled = producto is not null && !EsEditable;

        // ─────────────────────────────────────────────────────────────────
        // Nuevo / Editar / Cancelar
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Entra en alta: vacia el detalle, lo deja escribible y pone los valores de partida.
        /// El permiso se comprueba aqui y no deshabilitando el boton, igual que hace FrmPedidos:
        /// si la tabla de permisos no tiene filas de Productos, deshabilitar dejaria el modulo
        /// entero sin explicacion de por que no se puede escribir.
        /// </summary>
        private void BtnNuevoProducto_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeCrear("Productos"))
            {
                MostrarAviso("No tiene permiso para crear productos.");
                return;
            }

            _idEnEdicion = null;
            _idEnAlta = null;

            LimpiarDetalle();
            AplicarModo(ModoFormulario.Nuevo);

            // Generar el consecutivo del sistema (IdConsec, arranca en 1); el usuario no lo
            // toca: lo que teclea es el Codigo Ritrama, que es el product_id del producto.
            try
            {
                int nuevoId = _consecutivosService.GetAndIncrementConsecProducto();
                txtDetId.Text = nuevoId.ToString();
                txtDetId.ReadOnly = true; // El ID generado no se debe editar
            }
            catch (Exception ex)
            {
                MostrarAviso("No se pudo generar el ID automático: " + ex.Message);
                txtDetId.ReadOnly = false; // Permitir entrada manual si falla
            }

            // Los numeros arrancan a 0 para no obligar a escribir lo que no aplique, y el alta
            // nace Activa: un producto se desactiva a proposito, no por omitir el interruptor.
            txtDetPrecio.Text = "0";
            txtDetCosto.Text = "0";
            txtDetRatio.Text = "0";
            PintarEstado(true);

            ActualizarBotonesBarra();
            txtDetNombre.Focus();
        }

        /// <summary>Entra en edicion con el producto de la fila activa ya volcado en el detalle.</summary>
        private void BtnEditarProducto_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeEditar("Productos"))
            {
                MostrarAviso("No tiene permiso para editar productos.");
                return;
            }

            Product? producto = ProductoSeleccionado();
            if (producto is null)
            {
                MostrarAviso("Seleccione primero el producto que quiere editar.");
                return;
            }

            _idEnAlta = null;
            _idEnEdicion = producto.Product_id;
            _idSeleccionado = producto.Product_id;

            // El modo se aplica antes de volcar el detalle: MostrarDetalle pinta el switch a
            // traves de PintarEstado, que restituye el ReadOnly que manda el modo actual.
            AplicarModo(ModoFormulario.Editar);
            MostrarDetalle(producto);
            ActualizarBotonesBarra(producto);

            txtDetNombre.Focus();
        }

        /// <summary>Pregunta antes de tirar lo escrito y vuelve a consulta.</summary>
        private void BtnCancelarProducto_Click(object? sender, EventArgs e)
        {
            string pregunta = _modo == ModoFormulario.Nuevo
                ? "¿Descartar el producto nuevo? Se perderá lo que haya escrito."
                : "¿Descartar los cambios de este producto?";

            if (!ConfirmarDescarte(pregunta))
            {
                return;
            }

            VolverAConsulta();
        }

        /// <summary>
        /// Vuelve al estado de consulta dejando el detalle como estaba antes de empezar a
        /// escribir: el producto editado, o vacio si era un alta que se ha descartado.
        /// </summary>
        private void VolverAConsulta()
        {
            string? mostrarId = _idEnEdicion ?? _idEnAlta;
            _idEnEdicion = null;
            _idEnAlta = null;
            _guardando = false;

            AplicarModo(ModoFormulario.Consulta);

            Product? producto = mostrarId is null ? null : BuscarEnCatalogo(mostrarId);
            if (producto is not null)
            {
                _idSeleccionado = producto.Product_id;
                SeleccionarFilaEnGrid(producto.Product_id);
            }
            else
            {
                _idSeleccionado = null;
            }

            RefrescarDetalleDeLaFilaActiva();
        }

        // ─────────────────────────────────────────────────────────────────
        // Guardar
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Guarda el producto del formulario, sea alta nueva o edicion, y refresca el listado.
        /// Ante un fallo no se sale del modo de escritura: lo escrito se queda en pantalla para
        /// que el usuario lo corrija en vez de volver a teclearlo.
        /// </summary>
        private async void BtnGuardarProducto_Click(object? sender, EventArgs e)
        {
            if (_guardando)
            {
                return;
            }

            bool esNuevo = _modo == ModoFormulario.Nuevo;
            if (esNuevo ? !PermisoHelper.PuedeCrear("Productos") : !PermisoHelper.PuedeEditar("Productos"))
            {
                MostrarAviso("No tiene permiso para guardar este producto.");
                return;
            }

            Result<Product> construccion = ConstruirProductoDesdeFormulario();
            if (!construccion.IsSuccess)
            {
                MostrarAviso(construccion.Error ?? "Revise los datos del producto.");
                return;
            }

            Product producto = construccion.Value!;

            // Validacion de dominio antes de ir a la base: es la misma que ejecuta el servicio
            // por dentro (ValidateProduct), pero sin el viaje de ida y vuelta.
            Result validacion = _productsService.ValidateProduct(producto);
            if (!validacion.IsSuccess)
            {
                MostrarAviso(validacion.Error ?? "Revise los datos del producto.");
                return;
            }

            _guardando = true;
            btnGuardarProducto.Enabled = false;

            Result<bool> resultado = esNuevo
                ? await _productsService.AddValidatedAsync(producto)
                : await _productsService.UpdateValidatedAsync(producto);

            _guardando = false;
            btnGuardarProducto.Enabled = true;

            if (!resultado.IsSuccess)
            {
                MostrarAviso(resultado.Error ?? "No se pudo guardar el producto.");
                return;
            }

            if (!resultado.Value)
            {
                MostrarAviso(esNuevo
                    ? "No se insertó ninguna fila. Revise que el código no esté repetido."
                    : $"No se actualizó ninguna fila del producto '{producto.Product_id}'.");
                return;
            }

            // El filtro del buscador puede esconder lo recien guardado: si no casa con el, la
            // fila no sale en el listado y el usuario ve el formulario como si nada se hubiera
            // guardado. Se limpia en ese caso.
            if (!CumpleFiltroBuscador(producto))
            {
                txtSearch.Clear();
            }

            // En un alta, el id guardado es el recien creado: lo necesita VolverAConsulta para
            // volver a mostrarlo y dejar seleccionada su fila. En una edicion pasa lo mismo,
            // y ademas el codigo puede haber cambiado: hay que apuntar al nuevo.
            if (esNuevo)
            {
                _idEnAlta = producto.Product_id;
            }
            else
            {
                _idEnEdicion = producto.Product_id;
            }

            await CargarCatalogoAsync(producto.Product_id);
            VolverAConsulta();
        }

        // ─────────────────────────────────────────────────────────────────
        // Reporte del catalogo
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Abre el reporte del catalogo en el visor (ReportsViewer). Va contra la base con la
        /// consulta de R.QUERY.PRODUCTS y trae el catalogo entero, no lo que este filtrado.
        /// </summary>
        private void BtnReporteProducto_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeVer("Productos"))
            {
                MostrarAviso("No tiene permiso para ver los productos.");
                return;
            }

            try
            {
                _reportsService.Reporte_Productos(this, "Catalogo de Productos", "Report_Productos.rdlc");
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("No se pudo abrir el reporte de productos: " + ex.Message);
                MostrarAviso("No se pudo abrir el reporte: " + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Lectura del formulario (pantalla → Product)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Monta el producto con lo que hay en el formulario. Devuelve un <see cref="Result{T}"/>
        /// porque los tres campos numericos pueden traer texto que no es un numero, y eso hay que
        /// poder reportarlo antes de tocar la base.
        /// </summary>
        private Result<Product> ConstruirProductoDesdeFormulario()
        {
            Product producto = new()
            {
                // El codigo Ritrama ES el product_id: se teclea en su campo, tanto en Nuevo
                // como en Editar (el servicio, si cambia, propaga el nuevo codigo a las tablas
                // referenciadas). El detalle no se recarga al mover la seleccion del grid, asi
                // que a media edicion sigue mandando el codigo del producto abierto.
                Product_id = txtDetCodigoRitrama.Text.Trim(),

                // El consecutivo del sistema es solo referencial: sale de txtDetId, que en
                // Nuevo lo genera el contador PROD y que esta bloqueado en los dos modos.
                IdConsec = int.TryParse(txtDetId.Text.Trim(), out int consecutivo) ? consecutivo : 0,
                Product_Name = txtDetNombre.Text.Trim(),
                Product_Description = txtDetDescripcion.Text.Trim(),
                Referencia = txtDetReferencia.Text.Trim(),
                Codigo_Barra = txtDetCodebar.Text.Trim(),
                Master = rbTipoMaster.Checked,
                RolloCortado = rbTipoRolloCortado.Checked,
                Hoja = rbTipoHojas.Checked,
                Graphics = rbTipoGraphics.Checked,

                // El switch va al reves del bit: Activo = vigente, Desactivado = anulado. En un
                // alta el switch esta bloqueado y en Activo (ver EstadoEsEditable), asi que
                // aqui un producto nuevo SIEMPRE sale con Anulado = false: no se puede dar de
                // alta un producto desactivado. Desactivar es una operacion posterior, desde Editar.
                Anulado = !swDetEstado.Active
            };

            if (!TryParseDecimal(txtDetPrecio.Text, out decimal precio))
            {
                return Result<Product>.Failure("El precio no es un número válido.");
            }

            if (!TryParseDecimal(txtDetCosto.Text, out decimal costo))
            {
                return Result<Product>.Failure("El costo no es un número válido.");
            }

            if (!TryParseDecimal(txtDetRatio.Text, out decimal ratio))
            {
                return Result<Product>.Failure("El ratio no es un número válido.");
            }

            // Se ajusta a la precision de las columnas (precio y costo decimal(18,2),
            // ratio decimal(18,4)) y no a la tecleada: es lo que guarda la base.
            producto.Precio = decimal.Round(precio, 2);
            producto.Costo = decimal.Round(costo, 2);
            producto.Ratio = decimal.Round(ratio, 4);

            return Result<Product>.Success(producto);
        }

        /// <summary>
        /// Lee un decimal de un campo del detalle. El formato se decide por el ultimo separador que
        /// aparece, no por la cultura del sistema ni probando culturas en orden fijo: el detalle se
        /// pinta con InvariantCulture ("1,250.50") y el usuario escribe con es-ES ("1.250,50"), y
        /// probar en orden fijo no los distingue. Con NumberStyles.Any bajo es-ES, "98.25" se lee
        /// como 9825 (punto = millares) y el precio se guardaria mil veces mayor.
        /// Mirando el ultimo separador, cada formato cae en su cultura sin ambiguedad.
        /// Un campo vacio vale cero, no es un error.
        /// </summary>
        private static bool TryParseDecimal(string? texto, out decimal valor)
        {
            string limpio = (texto ?? string.Empty).Trim();
            if (limpio.Length == 0)
            {
                valor = 0m;
                return true;
            }

            int ultimoPunto = limpio.LastIndexOf('.');
            int ultimaComa = limpio.LastIndexOf(',');
            int ultimoSeparador = Math.Max(ultimoPunto, ultimaComa);

            CultureInfo cultura = ultimoSeparador < 0 || ultimoSeparador == ultimoPunto
                ? CultureInfo.InvariantCulture
                : CultureInfo.GetCultureInfo("es-ES");

            return decimal.TryParse(limpio, NumberStyles.Any, cultura, out valor);
        }

        /// <summary>
        /// True si el producto encaja con el texto del buscador. Se miran el codigo y el nombre,
        /// y ademas la CATEGORIA, que es una columna del grid pero no del producto: sin esto,
        /// escribir "resma" no encontraba nada aunque las 25 hojas estuvieran a la vista.
        /// Todas las comparaciones ignoran mayusculas (<see cref="Contiene"/>), asi que da igual
        /// escribir "Resma", "resma" o "RESMA".
        /// </summary>
        private static bool CoincideTexto(Product producto, string texto)
        {
            if (Contiene(producto.Product_id, texto) || Contiene(producto.Product_Name, texto))
            {
                return true;
            }

            string tipo = ProductCategoryRules.GetNombre(
                producto.Master, producto.RolloCortado, producto.Hoja, producto.Graphics);

            return Contiene(tipo, texto) || AliasesCategoria(tipo).Any(alias => Contiene(alias, texto));
        }

        /// <summary>
        /// Otras formas de escribir una categoria, para el buscador. La misma categoria se llama
        /// "Resma" en la columna Tipo del grid y "Hojas" en el filtro de radios, y ademas se
        /// escribe en singular y en plural, asi que tiene que encontrar la por todas. El nombre
        /// canonico lo pone ProductCategoryRules y se busca por separado, aqui solo las variantes.
        /// </summary>
        private static string[] AliasesCategoria(string tipo) => tipo switch
        {
            "Rollo Cortado" => ["rollos cortados", "rollocortado", "rc"],
            "Resma" => ["resmas", "hoja", "hojas"],
            "Graphics" => ["grafica", "graficas"],
            _ => []
        };

        /// <summary>True si el producto sobrevive al filtro activo del buscador.</summary>
        private bool CumpleFiltroBuscador(Product producto)
        {
            string filtro = (txtSearch.Text ?? string.Empty).Trim();
            return filtro.Length == 0 || CoincideTexto(producto, filtro);
        }

        /// <summary>Producto del catalogo en memoria con el codigo indicado, o null si no esta.</summary>
        private Product? BuscarEnCatalogo(string productId)
            => _productos.FirstOrDefault(p => string.Equals(p.Product_id, productId, StringComparison.OrdinalIgnoreCase));

        /// <summary>
        /// Selecciona en el grid la fila del codigo indicado. Se usa al volver de Editar y tras
        /// guardar, porque manda el codigo guardado y no la fila que el usuario tuviera de fondo.
        /// </summary>
        private void SeleccionarFilaEnGrid(string productId)
        {
            foreach (DataGridViewRow fila in gridProductos.Rows)
            {
                if (fila.DataBoundItem is FilaProducto dato
                    && string.Equals(dato.ProductId, productId, StringComparison.OrdinalIgnoreCase))
                {
                    gridProductos.ClearSelection();
                    fila.Selected = true;
                    gridProductos.CurrentCell = fila.Cells[0];
                    return;
                }
            }
        }

        /// <summary>
        /// Aviso de validacion o de fallo del servicio. Vive en un metodo aparte, y no en un
        /// MessageBox en linea, para que las pruebas puedan sustituir el dialogo modal por una
        /// llamada recorded: un MessageBox de verdad dentro de un test lo dejaria colgado.
        /// </summary>
        /// <param name="mensaje">Texto a mostrar al usuario.</param>
        protected virtual void MostrarAviso(string mensaje)
            => MessageBox.Show(mensaje, "Productos", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// Confirmacion de Cancelar. Mismo motivo que <see cref="MostrarAviso"/>.
        /// </summary>
        /// <param name="pregunta">Pregunta a confirmar.</param>
        /// <returns>True si el usuario acepta descartar.</returns>
        protected virtual bool ConfirmarDescarte(string pregunta)
            => MessageBox.Show(this, pregunta, "Productos", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes;

        /// <summary>
        /// Los radios del filtro de lista. Son OTROS controles que los del detalle
        /// (<see cref="RadiosTipo"/>), que eligen el tipo del producto que se esta guardando:
        /// si se confundieran, marcar el tipo de un producto moveria el filtro de la lista.
        /// El primero es "Todos", que es el estado de partida y el que devuelve la vista completa.
        /// </summary>
        private UIRadioButton[] RadiosFiltroCategoria() =>
        [
            rbFiltroTodos, rbFiltroMaster, rbFiltroRolloCortado, rbFiltroHojas, rbFiltroGraphics
        ];

        // ─────────────────────────────────────────────────────────────────
        // Hoja de Excel con todos los productos
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Abre el importador de productos desde Excel (CodigoRitrama + Nombre + Tipo).
        /// Requiere permiso de creacion: la importacion da de alta productos nuevos.
        /// Si algo se inserto, se recarga el catalogo para mostrarlo.
        /// </summary>
        private async void BtnImportarCatalogo_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeCrear("Productos"))
            {
                MostrarAviso("No tiene permiso para crear productos.");
                return;
            }

            using Otros.Frm_ImportProductos frm = new(_productsImportService);
            if (frm.ShowDialog(this) == DialogResult.OK)
            {
                await CargarCatalogoAsync();
            }
        }

        /// <summary>
        /// Crea un Excel con TODOS los productos del catalogo. Se exporta el catalogo entero,
        /// no lo que haya salido en pantalla: un filtro de categoria o una busqueda no pueden
        /// cambiar lo que el fichero contiene, que es el inventario completo.
        /// </summary>
        private void BtnExportarProducto_Click(object? sender, EventArgs e)
        {
            // Exportar no crea ni modifica datos, asi que basta el permiso de ver. Se valida
            // en el clic y no deshabilitando el boton, igual que el resto de la barra.
            if (!PermisoHelper.PuedeVer("Productos"))
            {
                MostrarAviso("No tiene permiso para ver los productos.");
                return;
            }

            // ExportToExcel lanza si la coleccion va vacia: se comprueba antes para salir
            // en silencio. No haber nada que exportar no es un error y en el listado ya se
            // ve: el unico aviso que queda en exportar es el del fallo.
            if (_productos.Count == 0)
            {
                return;
            }

            List<ProductoExportado> filas = _productos.Select(ParaExcel).ToList();

            try
            {
                // OJO con el nombre: "Products.xlsx" es un caso especial de ExportToExcel que
                // reescribe a mano las tres primeras cabeceras, y se comeria la de Descripcion.
                // Por eso el fichero se llama Productos.xlsx.
                // Sin aviso de éxito al exportar: el propio ExportToExcel abre el fichero,
                // que ya es la confirmación. Los fallos siguen avisando.
                _exportDataService.ExportToExcel(filas, "Productos.xlsx");
            }
            catch (Exception ex)
            {
                // El servicio ya avisa por ServiceErrors de sus fallos propios; aqui se cubre
                // lo que se le escape (permisos de carpeta, disco lleno, Excel abierto...).
                ServiceErrors.Report("Error al crear la hoja de productos: " + ex.Message);
                MostrarAviso("No se pudo crear la hoja de Excel: " + ex.Message);
            }
        }

        /// <summary>
        /// Proyecta un producto a la fila del Excel: el tipo ya resuelto en una columna y el
        /// estado en texto, en vez de los cuatro bits sueltos y el booleano de anulado.
        /// </summary>
        private static ProductoExportado ParaExcel(Product producto)
        {
            ArgumentNullException.ThrowIfNull(producto);

            return new ProductoExportado
            {
                Codigo = producto.Product_id ?? string.Empty,
                Consecutivo = producto.IdConsec.ToString(),
                Nombre = producto.Product_Name ?? string.Empty,
                Descripcion = producto.Product_Description ?? string.Empty,
                Referencia = producto.Referencia ?? string.Empty,
                CodigoBarra = producto.Codigo_Barra ?? string.Empty,
                Tipo = ProductCategoryRules.GetNombre(
                    producto.Master, producto.RolloCortado, producto.Hoja, producto.Graphics),
                Precio = producto.Precio,
                Costo = producto.Costo,
                Ratio = producto.Ratio,
                Estado = producto.Anulado ? "Desactivado" : "Activo"
            };
        }

        /// <summary>
        /// Aplica el filtro que marquen los radios; con "Todos" no se filtra nada. Los radios son
        /// excluyentes entre si (viven en el mismo contenedor), asi que marcar un tipo desmarca
        /// "Todos" solo: no hace falta guardar ningun estado aparte.
        /// </summary>
        private void FiltroCategoria_Click(object? sender, EventArgs e) => AplicarFiltro();

        /// <summary>Categoria que hay marcada en el filtro, o vacio si se ve el catalogo entero.</summary>
        private string CategoriaFiltrada() => ProductCategoryRules.GetNombre(
            rbFiltroMaster.Checked,
            rbFiltroRolloCortado.Checked,
            rbFiltroHojas.Checked,
            rbFiltroGraphics.Checked);

        /// <summary>True si el producto pasa el filtro de categoria activo.</summary>
        private bool CoincideCategoria(Product producto)
        {
            if (rbFiltroTodos.Checked)
            {
                return true;
            }

            if (rbFiltroMaster.Checked)
            {
                return producto.Master;
            }

            if (rbFiltroRolloCortado.Checked)
            {
                return producto.RolloCortado;
            }

            if (rbFiltroHojas.Checked)
            {
                return producto.Hoja;
            }

            if (rbFiltroGraphics.Checked)
            {
                return producto.Graphics;
            }

            // Sin filtro: pasan todos.
            return true;
        }

        /// <summary>Contenido insensible a mayusculas para el buscador.</summary>
        private static bool Contiene(string? valor, string filtro)
            => (valor ?? string.Empty).Contains(filtro, StringComparison.OrdinalIgnoreCase);

        /// <summary>Fila enlazada al grid: los tres campos visibles mas el estado para poder pintarlos.</summary>
        private sealed record FilaProducto(string ProductId, string ProductName, string Tipo, bool Anulado);
    }
}
