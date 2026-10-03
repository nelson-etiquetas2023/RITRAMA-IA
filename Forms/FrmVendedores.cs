using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Ritrama2025.Core;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.VendedorService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Vendedores (rediseño 30/70): panel izquierdo con buscador, cuadro
    /// de resumen (total/filtrado) y grid de vendedores; panel derecho con la página de
    /// detalle, que se refresca al seleccionar una fila del grid.
    /// </summary>
    public partial class FrmVendedores : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IVendedorService _vendedoresService;
        private readonly IConsecutivosService _consecutivosService;
        private readonly IExportDataService _exportDataService;
        private readonly IReportsService _reportsService;
        private DataTable _dtVendedores = new();

        /// <summary>
        /// Filtro de estado: los tres radios (Todos / Activos / Desactivados) que se crean por
        /// código bajo el buscador, ver CrearFiltroEstado. Vendedores no tiene categoría, así
        /// que su criterio de filtro es el estado.
        /// </summary>
        private Panel pnlFiltroEstado = null!;
        private RadioButton rbEstadoTodos = null!;
        private RadioButton rbEstadoActivos = null!;
        private RadioButton rbEstadoDesactivados = null!;

        /// <summary>Valores de la columna status, tal y como los escribe el servicio.</summary>
        private const string EstadoActivo = "activo";
        private const string EstadoDesactivado = "desactivado";

        /// <summary>Etiqueta y caja del código interno (creadas por código, ver CrearCampoCodigoInterno).</summary>
        private UILabel lblCapCodigoInterno = null!;
        private UITextBox txtCodigoInterno = null!;

        /// <summary>Etiqueta y caja de zona (creadas por código, ver CrearCampoZona).</summary>
        private UILabel lblCapZona = null!;
        private UITextBox txtValorZona = null!;

        /// <summary>
        /// Estado del formulario. Consulta es el estado por defecto: el detalle se ve pero no se
        /// escribe. Solo Nuevo y Editar habilitan la escritura; el módulo no pide permisos.
        /// </summary>
        private enum ModoFormulario
        {
            Consulta,
            Nuevo,
            Editar
        }

        private ModoFormulario _modo = ModoFormulario.Consulta;
        private string? _idEnEdicion;
        private string? _idEnAlta;
        private string? _idSeleccionado;
        private bool _guardando;

        /// <summary>
        /// El alta de vendedores está activa: el botón Nuevo aparece en la barra (oculto al
        /// entrar en escritura, igual que el resto). Sirve de interruptor: para volver a
        /// esconderlo basta con poner este valor en false.
        /// </summary>
        private const bool CreacionHabilitada = true;

        /// <summary>
        /// True cuando el formulario admite escritura: Nuevo crea un vendedor y Editar modifica
        /// el seleccionado. Consulta es el único estado de solo lectura.
        /// </summary>
        private bool EsEditable => _modo is ModoFormulario.Nuevo or ModoFormulario.Editar;

        /// <summary>
        /// El interruptor de estado solo se mueve (y solo se ve) al EDITAR un vendedor que ya
        /// existe. En el alta queda OCULTO porque un vendedor se crea siempre vigente: nace
        /// Activo y se desactiva después, desde Editar, si hace falta. En consulta se ve, pero
        /// bloqueado, para mostrar el estado del vendedor seleccionado.
        /// </summary>
        private bool EstadoEsEditable => _modo is ModoFormulario.Editar;

        /// <summary>
        /// Crea el formulario de vendedores con su servicio de listado.
        /// </summary>
        /// <param name="vendedoresService">Servicio que trae el listado de la base.</param>
        /// <param name="consecutivosService">Servicio que genera el código interno consecutivo.</param>
        public FrmVendedores(IVendedorService vendedoresService, IConsecutivosService consecutivosService, IExportDataService exportDataService, IReportsService reportsService)
        {
            InitializeComponent();
            ArgumentNullException.ThrowIfNull(vendedoresService);
            ArgumentNullException.ThrowIfNull(consecutivosService);
            ArgumentNullException.ThrowIfNull(exportDataService);
            ArgumentNullException.ThrowIfNull(reportsService);
            _vendedoresService = vendedoresService;
            _consecutivosService = consecutivosService;
            _exportDataService = exportDataService;
            _reportsService = reportsService;

            Text = "Vendedores";

            // Este módulo usa el estilo VERDE de SunnyUI (igual que Pedidos/Producción/Despacho).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();
            CrearCampoCodigoInterno();
            CrearCampoZona();
            EstilarCamposDetalle();
            EstilarSwitchEstado();
            EstilarSeleccionGrid();
            EstilarBarraHerramientas();
            CrearFiltroEstado();

            // Filtrado en vivo sobre el listado ya cargado: por nombre y por el radio de
            // estado, combinados con AND. El RowFilter del DataTable se refleja solo en el
            // grid enlazado, sin volver a la base.
            txtBuscar.TextChanged += (_, _) => AplicarFiltros();

            // RefrescarDetalle con los dos eventos: el SelectionChanged se difiere al
            // siguiente ciclo de mensajes (y dispara con la fila anterior) mientras que
            // el CurrentCellChanged dispara síncrono — mismo criterio que FrmClientes.
            gridVendedores.SelectionChanged += (_, _) => RefrescarDetalleDeLaFilaActiva();
            gridVendedores.CurrentCellChanged += (_, _) => RefrescarDetalleDeLaFilaActiva();

            // Toolbar del detalle
            btnNuevoVendedor.Click += BtnNuevoVendedor_Click;
            btnEditarVendedor.Click += BtnEditarVendedor_Click;
            btnImportarVendedor.Click += BtnImportarVendedor_Click;
            btnReporteVendedor.Click += BtnReporteVendedor_Click;
            btnGuardarVendedor.Click += BtnGuardarVendedor_Click;
            btnCancelarVendedor.Click += BtnCancelarVendedor_Click;

            // El formulario nace en consulta: el detalle se ve bloqueado y Guardar/Cancelar
            // quedan ocultos. Sin esta llamada, los controles arrancarían con el ReadOnly
            // del diseñador (editable).
            AplicarModo(ModoFormulario.Consulta);
        }

        /// <summary>
        /// Reaplica el tema verde para pisar el UIStyleManager global del Main.
        /// </summary>
        public void ReaplicarTema()
        {
            AplicarTemaVerde();
            EstilarBarraHerramientas();
            EstilarSeleccionGrid();
        }

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = verde;
            TitleForeColor = Color.White;

            gridVendedores.ColumnHeadersDefaultCellStyle.BackColor = verde;
            gridVendedores.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridVendedores.GridColor = Color.FromArgb(180, 180, 180);
        }

        /// <summary>
        /// Crea la fila "Código interno" justo debajo de "Código" (fila 2) 100% por código
        /// para no tocar el Designer: desplaza las filas 2..N una posición e inserta el estilo.
        /// La caja nace de solo lectura y así queda siempre: el interno lo asigna el sistema.
        /// </summary>
        private void CrearCampoCodigoInterno()
        {
            tlpDetalle.SuspendLayout();

            lblCapCodigoInterno = new UILabel
            {
                Name = "lblCapCodigoInterno",
                Text = "Código interno",
                Dock = DockStyle.Fill,
                Font = new Font("JetBrains Mono", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0)
            };
            txtCodigoInterno = new UITextBox
            {
                Name = "txtCodigoInterno",
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                RectColor = Color.FromArgb(180, 180, 180),
                Font = new Font("JetBrains Mono", 9F),
                TextAlignment = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 6, 0),
                ReadOnly = true,
                Text = "—"
            };

            const int filaNueva = 2;
            Control[] controles = new Control[tlpDetalle.Controls.Count];
            tlpDetalle.Controls.CopyTo(controles, 0);
            foreach (Control c in controles)
            {
                int fila = tlpDetalle.GetRow(c);
                if (fila >= filaNueva)
                {
                    tlpDetalle.SetRow(c, fila + 1);
                }
            }
            tlpDetalle.RowCount++;
            tlpDetalle.RowStyles.Insert(filaNueva, new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.Controls.Add(lblCapCodigoInterno, 0, filaNueva);
            tlpDetalle.Controls.Add(txtCodigoInterno, 1, filaNueva);

            tlpDetalle.ResumeLayout(true);
        }

        /// <summary>Lee el código interno de la fila ("—"/NULL → 0, filas legadas).</summary>
        private static int EnteroCodigoInterno(DataRowView fila)
        {
            if (!fila.Row.Table.Columns.Contains("codigo_interno"))
            {
                return 0;
            }

            object valor = fila["codigo_interno"];
            if (valor is null || valor == DBNull.Value)
            {
                return 0;
            }

            try
            {
                return Convert.ToInt32(valor);
            }
            catch (FormatException)
            {
                return 0;
            }
            catch (InvalidCastException)
            {
                return 0;
            }
        }

        /// <summary>Formato de pantalla del interno: 5 dígitos (00001) o "—" si no tiene.</summary>
        private static string TextoCodigoInterno(int interno)
            => interno > 0 ? interno.ToString("D5") : "—";

        /// <summary>
        /// Crea la fila "Zona" justo debajo del teléfono (fila 6) 100% por código
        /// para no tocar el Designer: desplaza las filas 6..N una posición e inserta el estilo.
        /// Se llama después de CrearCampoCodigoInterno, así que las filas ya incluyen el interno.
        /// </summary>
        private void CrearCampoZona()
        {
            tlpDetalle.SuspendLayout();

            lblCapZona = new UILabel
            {
                Name = "lblCapZona",
                Text = "Zona",
                Dock = DockStyle.Fill,
                Font = new Font("JetBrains Mono", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0)
            };
            txtValorZona = new UITextBox
            {
                Name = "txtValorZona",
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                RectColor = Color.FromArgb(180, 180, 180),
                Font = new Font("JetBrains Mono", 9F),
                TextAlignment = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 6, 0),
                Text = "—"
            };

            const int filaNueva = 6;
            Control[] controles = new Control[tlpDetalle.Controls.Count];
            tlpDetalle.Controls.CopyTo(controles, 0);
            foreach (Control c in controles)
            {
                int fila = tlpDetalle.GetRow(c);
                if (fila >= filaNueva)
                {
                    tlpDetalle.SetRow(c, fila + 1);
                }
            }
            tlpDetalle.RowCount++;
            tlpDetalle.RowStyles.Insert(filaNueva, new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.Controls.Add(lblCapZona, 0, filaNueva);
            tlpDetalle.Controls.Add(txtValorZona, 1, filaNueva);

            tlpDetalle.ResumeLayout(true);
        }

        /// <summary>
        /// Aplica fuente, color y solo lectura a los controles de la pestaña de detalle.
        /// Se hace aquí (y no en el diseñador) porque el UIStyleManager del constructor re-estiliza
        /// el form después de InitializeComponent y pisaría los colores puestos en el diseñador.
        /// </summary>
        private void EstilarCamposDetalle()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            Color grisTexto = Color.FromArgb(80, 80, 80);
            Color colorBorde = Color.FromArgb(180, 180, 180);

            UILabel[] etiquetas =
            [
                lblCapId, lblCapNombre, lblCapCorreo, lblCapTelefono,
                lblCapZona, lblCapEstado
            ];

            foreach (UILabel etiqueta in etiquetas)
            {
                etiqueta.Font = new Font("JetBrains Mono", 9F);
                etiqueta.ForeColor = grisTexto;
            }

            lblDetalleTitulo.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            lblDetalleTitulo.ForeColor = verde;

            UITextBox[] campos =
            [
                txtValorId, txtValorNombre, txtValorCorreo, txtValorTelefono, txtValorZona
            ];

            foreach (UITextBox campo in campos)
            {
                campo.FillColor = Color.White;
                campo.RectColor = colorBorde;
                campo.Font = new Font("JetBrains Mono", 9F);
                campo.TextAlignment = ContentAlignment.MiddleLeft;
            }
        }

        /// <summary>
        /// El estado del vendedor es un UISwitch con los dos estados del dominio: Activo y
        /// Desactivado (que en la base es status = 'activo'/'desactivado'). Solo se ve y se mueve en Editar: en el
        /// alta está oculto porque un vendedor nuevo nace siempre Activo (ver AplicarModo), y en
        /// consulta se ve bloqueado para mostrar el estado del vendedor seleccionado.
        /// </summary>
        private void EstilarSwitchEstado()
        {
            // Tamaño copiado de Productos (swDetEstado): Dock.Left y 120x28 para que
            // ocupe solo el botón + título, no todo el ancho de la celda. En el
            // diseñador quedó Dock.Fill y Enabled=false: el Fill lo estiraba a todo
            // el ancho y el Enabled lo dejaba gris apagado. Se gobierna por ReadOnly
            // (ver AplicarModo/PintarEstado), igual que en Productos.
            swEstado.Dock = DockStyle.Left;
            swEstado.Size = new Size(120, 28);
            swEstado.MinimumSize = new Size(1, 16);
            swEstado.Margin = new Padding(4, 5, 4, 5);
            swEstado.Enabled = true;
            swEstado.ReadOnly = true;
            swEstado.Font = new Font("JetBrains Mono", 9F);
            swEstado.ForeColor = Color.FromArgb(64, 64, 64);
            swEstado.ActiveColor = Color.FromArgb(110, 190, 40);
            swEstado.ActiveText = "Activo";
            swEstado.InActiveText = "Desactivado";
        }

        /// <summary>
        /// La fila seleccionada del grid se pinta negro con letras blancas.
        /// Se hace aquí (y no en el diseñador) porque el UIStyleManager del constructor re-estiliza
        /// el form después de InitializeComponent y pisaría los colores puestos en el diseñador.
        /// </summary>
        private void EstilarSeleccionGrid()
        {
            gridVendedores.DefaultCellStyle.SelectionBackColor = Color.Black;
            gridVendedores.DefaultCellStyle.SelectionForeColor = Color.White;
            gridVendedores.RowsDefaultCellStyle.SelectionBackColor = Color.Black;
            gridVendedores.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            // Las impares se pintan con el estilo alterno (StripeOddColor): sin esto la
            // fila impar seleccionada conservaba el verde del diseñador.
            gridVendedores.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.Black;
            gridVendedores.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
        }

        /// <summary>
        /// Replica el estilo de la barra de Productos (FrmProductos.barraHerramientas):
        /// fondo gris claro, fuente Microsoft Sans Serif 10 Bold, botones 92x36 sin
        /// AutoSize, Guardar/Cancelar solo texto (sin icono rojo/verde).
        /// Se hace aquí y no en el diseñador porque el UIStyleManager re-estiliza
        /// el form tras InitializeComponent y pisaría los valores del diseñador.
        /// </summary>
        private void EstilarBarraHerramientas()
        {
            Color grisBoton = Color.FromArgb(80, 80, 80);
            Font fuenteBarra = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);

            barraHerramientas.BackColor = Color.FromArgb(225, 225, 225);
            barraHerramientas.ForeColor = grisBoton;
            barraHerramientas.Font = fuenteBarra;
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            barraHerramientas.Padding = new Padding(4, 2, 0, 2);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;

            ToolStripButton[] botones =
            [
                btnNuevoVendedor, btnEditarVendedor, btnGuardarVendedor, btnCancelarVendedor
            ];

            foreach (ToolStripButton boton in botones)
            {
                boton.AutoSize = false;
                boton.BackColor = Color.Transparent;
                boton.Font = fuenteBarra;
                boton.ForeColor = grisBoton;
                boton.ImageAlign = ContentAlignment.MiddleLeft;
                boton.ImageScaling = ToolStripItemImageScaling.None;
                boton.Margin = new Padding(4, 1, 0, 2);
                boton.Size = new Size(92, 36);
            }

            // Sin icono a propósito (igual que Productos): el check verde y la X roja
            // se leen como éxito/error, no como guardar/descartar.
            btnGuardarVendedor.Image = null;
            btnCancelarVendedor.Image = null;
        }

        /// <summary>
        /// Carga el listado de vendedores antes de mostrar la pestaña. Si la base falla,
        /// se reporta el error y el formulario queda visible con el grid vacío.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _dtVendedores = await _vendedoresService.LoadListadoAsync();
                gridVendedores.DataSource = _dtVendedores;
                PintarFilasAnuladas();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Vendedores: " + ex.Message);
            }
            finally
            {
                ActualizaResumen();
            }
        }

        /// <summary>
        /// Fondo de las filas de vendedores desactivados: rojo de verdad (firebrick), el mismo
        /// que ya usan Clientes, Proveedores y Pedidos, para que la fila se note de un vistazo
        /// sin tener que leer la columna de estado.
        /// </summary>
        internal static readonly Color ColorFondoAnulado = Color.Firebrick;

        /// <summary>
        /// Letra de las filas de vendedores desactivados: rosa claro, más clara que el texto
        /// normal, para que se lea sobre <see cref="ColorFondoAnulado"/> y no se funda con el
        /// rojo del fondo.
        /// </summary>
        internal static readonly Color ColorTextoAnulado = Color.FromArgb(255, 214, 214);

        /// <summary>
        /// Pinta en rojo con letra clara las filas de los vendedores desactivados, para que el
        /// usuario los distinga de un vistazo sin abrirlos. Pinta fondo y texto, pero no la
        /// selección: si la fila está seleccionada gana el verde del módulo, porque el color
        /// de selección del grid tiene prioridad sobre el de la celda.
        /// Se repinta al cargar el listado y cada vez que se filtra, porque en ambos casos el
        /// grid cambia de filas.
        /// </summary>
        private void PintarFilasAnuladas()
        {
            foreach (DataGridViewRow fila in gridVendedores.Rows)
            {
                bool anulado = fila.DataBoundItem is DataRowView datos
                    && Texto(datos, "status") == EstadoDesactivado;

                foreach (DataGridViewCell celda in fila.Cells)
                {
                    celda.Style.ForeColor = anulado ? ColorTextoAnulado : Color.Empty;
                    celda.Style.BackColor = anulado ? ColorFondoAnulado : Color.Empty;
                }
            }
        }

        /// <summary>
        /// Los tres radios de estado (Todos / Activos / Desactivados) debajo del buscador.
        /// En Vendedores el criterio de filtro es el estado, no la categoría (la tabla
        /// vendedor no tiene categoría), así que estos radios ocupan el hueco que en
        /// Clientes y Proveedores tienen los de Nacional/Internacional. Se crean por código,
        /// no en el Designer, para no pelear con el generador visual.
        /// "Todos" es el estado inicial: el reset que devuelve la lista completa.
        /// </summary>
        private void CrearFiltroEstado()
        {
            pnlFiltroEstado = new Panel
            {
                Name = "pnlFiltroEstado",
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(8, 4, 8, 4),
                Size = new Size(325, 32)
            };

            FlowLayoutPanel filaRadios = new()
            {
                Name = "flwRadiosEstado",
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Size = new Size(325, 24)
            };

            rbEstadoTodos = CrearRadioEstado("rbEstadoTodos", "Todos", filaRadios);
            rbEstadoActivos = CrearRadioEstado("rbEstadoActivos", "Activos", filaRadios);
            rbEstadoDesactivados = CrearRadioEstado("rbEstadoDesactivados", "Desactivados", filaRadios);

            pnlFiltroEstado.Controls.Add(filaRadios);

            // pnlFiltroEstado cuelga de panelIzq entre el buscador y el grid. El contador
            // (pnlResumen) ya viene dockeado abajo desde el diseñador.
            //
            // Ojo con el orden: WinForms procesa el dockeo en orden INVERSO al de la
            // colección, así que el último control agregado es el que queda más ARRIBA. El
            // diseñador dejó [gridVendedores(Fill), pnlResumen(Bottom), pnlBuscador(Top)];
            // para meter los radios hay que quitarlos de la cola y volver a agregarlos al revés.
            panelIzq.Controls.Remove(pnlBuscador);
            panelIzq.Controls.Remove(pnlResumen);
            panelIzq.Controls.Add(pnlFiltroEstado);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Controls.Add(pnlResumen);

            // "Todos" arranca marcado: sin filtro de estado la lista va completa.
            rbEstadoTodos.Checked = true;
        }

        /// <summary>Un radio del filtro de estado, con el estilo del formulario y enganchado
        /// al filtrado. Los tres comparten el mismo panel, que los mantiene excluyentes.</summary>
        private RadioButton CrearRadioEstado(string nombre, string texto, Control contenedor)
        {
            RadioButton radio = new()
            {
                Name = nombre,
                Text = texto,
                AutoSize = true,
                Appearance = Appearance.Normal,
                Font = new Font("JetBrains Mono", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Margin = new Padding(0, 0, 12, 0),
                Tag = texto
            };
            radio.CheckedChanged += (_, _) =>
            {
                if (radio.Checked)
                {
                    AplicarFiltros();
                }
            };

            contenedor.Controls.Add(radio);
            return radio;
        }

        /// <summary>
        /// Filtra el listado por nombre (comodín SQL, escapando las comillas) y por estado con
        /// los radios, combinando ambas condiciones con AND: "Todos" no agrega condición de
        /// estado, así que devuelve la lista completa. Repinta las filas desactivadas y
        /// actualiza el cuadro de resumen.
        /// </summary>
        private void AplicarFiltros()
        {
            List<string> condiciones = [];

            string filtro = txtBuscar.Text?.Trim() ?? string.Empty;
            if (filtro.Length > 0)
            {
                condiciones.Add($"vendor_name LIKE '%{filtro.Replace("'", "''")}%'");
            }

            if (rbEstadoActivos.Checked)
            {
                condiciones.Add($"status = '{EstadoActivo}'");
            }
            else if (rbEstadoDesactivados.Checked)
            {
                condiciones.Add($"status = '{EstadoDesactivado}'");
            }

            _dtVendedores.DefaultView.RowFilter = string.Join(" AND ", condiciones);
            PintarFilasAnuladas();
            ActualizaResumen();
        }

        /// <summary>True si el usuario eligió un estado concreto (no "Todos").</summary>
        private bool HayFiltroEstado() => rbEstadoActivos.Checked || rbEstadoDesactivados.Checked;

        /// <summary>
        /// Cuadro de resumen bajo el buscador: total de vendedores sin filtro, o
        /// cuántos se muestran del total cuando el filtro está activo. Cuenta como
        /// activo tanto el texto buscado como el radio de estado.
        /// </summary>
        private void ActualizaResumen()
        {
            int total = _dtVendedores.Rows.Count;
            int visibles = _dtVendedores.DefaultView.Count;
            bool filtrando = (txtBuscar.Text?.Trim().Length ?? 0) > 0 || HayFiltroEstado();

            lblResumen.Text = !filtrando
                ? $"Total: {total} {(total == 1 ? "vendedor" : "vendedores")}"
                : $"Mostrando {visibles} de {total} {(total == 1 ? "vendedor" : "vendedores")}";
        }

        /// <summary>
        /// Único camino que vuelca la fila activa en la pestaña de detalle y en la barra de acciones.
        /// Si no hay fila, el detalle se limpia. Mientras se está escribiendo (Nuevo o Editar)
        /// no se toca el detalle: el grid sigue siendo navegable, pero recargar sus campos
        /// dejaría al usuario corrigiendo los datos de un vendedor sobre la pantalla de otro,
        /// y Guardar los guardaría como el equivocado.
        /// </summary>
        private void RefrescarDetalleDeLaFilaActiva()
        {
            if (EsEditable)
            {
                return;
            }

            DataRowView? fila = gridVendedores.CurrentRow?.DataBoundItem as DataRowView;
            _idSeleccionado = fila is not null ? Texto(fila, "vendor_id") : null;

            if (fila is not null)
            {
                MostrarDetalle(fila);
            }
            else
            {
                LimpiarDetalle();
            }
            ActualizarBotonesBarra(fila);
        }

        /// <summary>
        /// Vuelca la fila seleccionada en la pestaña de detalle.
        /// </summary>
        private void MostrarDetalle(DataRowView fila)
        {
            txtValorId.Text = Texto(fila, "vendor_id");
            txtCodigoInterno.Text = TextoCodigoInterno(EnteroCodigoInterno(fila));
            txtValorNombre.Text = Texto(fila, "vendor_name");
            txtValorCorreo.Text = Texto(fila, "correo");
            txtValorTelefono.Text = Texto(fila, "phone");
            txtValorZona.Text = Texto(fila, "zona");
            PintarEstado(Texto(fila, "status") == "activo");
        }

        /// <summary>
        /// Pinta el estado del vendedor en el switch (true = Activo, false = Desactivado).
        /// SunnyUI hace que UISwitch.Active ignore el setter cuando el control
        /// es ReadOnly, así que aquí se levanta ReadOnly solo mientras se asigna.
        /// </summary>
        private void PintarEstado(bool activo)
        {
            swEstado.ReadOnly = false;
            swEstado.Active = activo;
            swEstado.ReadOnly = !EstadoEsEditable;
            swEstado.ForeColor = activo
                ? Color.FromArgb(60, 110, 20)
                : Color.FromArgb(180, 60, 60);
        }

        /// <summary>
        /// Deja el detalle vacío ("—") cuando no hay fila seleccionada.
        /// </summary>
        private void LimpiarDetalle()
        {
            txtValorId.Text = "—";
            txtCodigoInterno.Text = "—";
            txtValorNombre.Text = "—";
            txtValorCorreo.Text = "—";
            txtValorTelefono.Text = "—";
            txtValorZona.Text = "—";
            PintarEstado(true);
        }

        /// <summary>
        /// Habilita/deshabilita los botones de la barra según el modo y la selección.
        /// </summary>
        private void ActualizarBotonesBarra(DataRowView? fila = null)
        {
            // CreacionHabilitada es el interruptor del alta y está activo: el botón Nuevo se
            // ve en consulta. Ni aquí ni en el resto del módulo se pide permiso: la tabla
            // permisos no incluye el módulo Vendedores, así que pedirlo lo bloqueaba siempre.
            btnNuevoVendedor.Visible = !EsEditable && CreacionHabilitada;
            btnEditarVendedor.Visible = !EsEditable;
            btnGuardarVendedor.Visible = EsEditable;
            btnCancelarVendedor.Visible = EsEditable;

            btnNuevoVendedor.Enabled = !EsEditable && CreacionHabilitada;
            btnEditarVendedor.Enabled = !EsEditable && fila is not null;
            btnGuardarVendedor.Enabled = EsEditable;
            btnCancelarVendedor.Enabled = EsEditable;
        }

        /// <summary>
        /// Único método que decide qué se puede escribir. AplicarModo es el único sitio que toca
        /// ReadOnly, Enabled y Visible, para que no queden dos reglas de editabilidad que se
        /// pisen entre sí. En Consulta deja el detalle entero bloqueado.
        /// </summary>
        private void AplicarModo(ModoFormulario modo)
        {
            _modo = modo;
            bool editable = EsEditable;

            // Los dos códigos los genera el sistema al dar de alta (GUID + interno
            // consecutivo), así que quedan fijos siempre: en Nuevo porque vienen
            // calculados y en Editar porque son la identidad del vendedor.
            // ReadOnly y no Enabled=false: permite pintar valores por código.
            swEstado.ReadOnly = !EstadoEsEditable;
            txtValorId.ReadOnly = true;
            txtCodigoInterno.ReadOnly = true;

            txtValorNombre.ReadOnly = !editable;
            txtValorCorreo.ReadOnly = !editable;
            txtValorTelefono.ReadOnly = !editable;
            txtValorZona.ReadOnly = !editable;

            // El interruptor NO SE MUESTRA en alta. Un vendedor nuevo nace siempre vigente.
            swEstado.Visible = modo is not ModoFormulario.Nuevo;
            lblCapEstado.Visible = modo is not ModoFormulario.Nuevo;

            // El filtro se USA en consulta, así que va habilitado mientras no se está
            // escribiendo. Solo se bloquea en Nuevo y Editar.
            txtBuscar.Enabled = !editable;

            // En escritura se ocultan Nuevo y Editar, y aparecen Guardar y Cancelar.
            ActualizarBotonesBarra(gridVendedores.CurrentRow?.DataBoundItem as DataRowView);
        }

        // ─────────────────────────────────────────────────────────────────
        // Nuevo
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Entra en alta: vacía el detalle, lo deja escribible y pone los valores de partida.
        /// Este módulo no pide permisos: la tabla permisos no tiene el módulo Vendedores.
        /// </summary>
        private void BtnNuevoVendedor_Click(object? sender, EventArgs e)
        {
            // Los dos códigos los calcula el sistema: GUID para el primario e
            // interno 1, 2, 3... Sin interno no se entra a Nuevo (no se guarda en 0).
            int interno;
            try
            {
                interno = _consecutivosService.GetAndIncrementConsecVendedor();
            }
            catch (Exception ex)
            {
                MostrarAviso("No se pudo generar el código interno: " + ex.Message);
                return;
            }

            _idEnEdicion = null;
            _idEnAlta = null;

            LimpiarDetalle();
            AplicarModo(ModoFormulario.Nuevo);

            // En el alta las cajas van en blanco: la "—" es solo para "sin selección".
            foreach (Control control in tlpDetalle.Controls)
            {
                if (control is UITextBox caja)
                {
                    caja.Text = string.Empty;
                }
            }

            txtValorId.Text = Guid.NewGuid().ToString();
            txtCodigoInterno.Text = interno.ToString("D5");

            txtValorNombre.Focus();
        }

        // ─────────────────────────────────────────────────────────────────
        // Editar
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Entra en edición con el vendedor de la fila activa ya volcado en el detalle.
        /// Sin comprobación de permisos: el módulo Vendedores no tiene permisos propios.
        /// </summary>
        private void BtnEditarVendedor_Click(object? sender, EventArgs e)
        {
            DataRowView? fila = gridVendedores.CurrentRow?.DataBoundItem as DataRowView;
            if (fila is null)
            {
                MostrarAviso("Seleccione primero el vendedor que quiere editar.");
                return;
            }

            _idEnAlta = null;
            _idEnEdicion = Texto(fila, "vendor_id");
            _idSeleccionado = _idEnEdicion;

            // El modo se aplica antes de volcar el detalle: MostrarDetalle pinta el switch a
            // través de PintarEstado, que restituye el ReadOnly que manda el modo actual.
            AplicarModo(ModoFormulario.Editar);
            MostrarDetalle(fila);
            ActualizarBotonesBarra(fila);

            txtValorNombre.Focus();
        }

        // ─────────────────────────────────────────────────────────────────
        // Cancelar
        // ─────────────────────────────────────────────────────────────────

        /// <summary>Pregunta antes de tirar lo escrito y vuelve a consulta.</summary>
        private void BtnCancelarVendedor_Click(object? sender, EventArgs e)
        {
            string pregunta = _modo == ModoFormulario.Nuevo
                ? "¿Descartar el vendedor nuevo? Se perderá lo que haya escrito."
                : "¿Descartar los cambios de este vendedor?";

            if (!ConfirmarDescarte(pregunta))
            {
                return;
            }

            VolverAConsulta();
        }

        /// <summary>
        /// Vuelve al estado de consulta dejando el detalle como estaba antes de empezar a
        /// escribir: el vendedor editado, o vacío si era un alta que se ha descartado.
        /// </summary>
        private void VolverAConsulta()
        {
            string? mostrarId = _idEnEdicion ?? _idEnAlta;
            _idEnEdicion = null;
            _idEnAlta = null;
            _guardando = false;

            AplicarModo(ModoFormulario.Consulta);

            if (mostrarId is not null)
            {
                DataRowView? fila = BuscarEnCatalogo(mostrarId);
                if (fila is not null)
                {
                    _idSeleccionado = Texto(fila, "vendor_id");
                    SeleccionarFilaEnGrid(_idSeleccionado);
                }
            }
            else
            {
                _idSeleccionado = null;
            }

            RefrescarDetalleDeLaFilaActiva();
        }

        /// <summary>
        /// Busca un vendedor por ID en el DataTable cargado.
        /// </summary>
        private DataRowView? BuscarEnCatalogo(string id)
        {
            DataRow[] filas = _dtVendedores.Select($"vendor_id = '{id.Replace("'", "''")}'");
            return filas.Length > 0 ? new DataView(filas.CopyToDataTable())[0] : null;
        }

        /// <summary>
        /// Selecciona la fila del grid que corresponde al ID dado.
        /// </summary>
        private void SeleccionarFilaEnGrid(string id)
        {
            foreach (DataGridViewRow row in gridVendedores.Rows)
            {
                if (row.DataBoundItem is DataRowView fila && Texto(fila, "vendor_id") == id)
                {
                    gridVendedores.ClearSelection();
                    row.Selected = true;
                    gridVendedores.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Guardar
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Guarda el vendedor del formulario, sea alta nueva o edición, y refresca el listado.
        /// Ante un fallo no se sale del modo de escritura: lo escrito se queda en pantalla para
        /// que el usuario lo corrija en vez de volver a teclearlo.
        /// </summary>
        private async void BtnGuardarVendedor_Click(object? sender, EventArgs e)
        {
            if (_guardando)
            {
                return;
            }

            bool esNuevo = _modo == ModoFormulario.Nuevo;

            Result<Vendedor> construccion = ConstruirVendedorDesdeFormulario();
            if (!construccion.IsSuccess)
            {
                MostrarAviso(construccion.Error ?? "Revise los datos del vendedor.");
                return;
            }

            Vendedor vendedor = construccion.Value!;

            // Validación de dominio antes de ir a la base
            Result validacion = _vendedoresService.ValidateVendedor(vendedor);
            if (!validacion.IsSuccess)
            {
                MostrarAviso(validacion.Error ?? "Revise los datos del vendedor.");
                return;
            }

            _guardando = true;
            btnGuardarVendedor.Enabled = false;

            Result<bool> resultado = esNuevo
                ? await _vendedoresService.AddValidatedAsync(vendedor)
                : await _vendedoresService.UpdateValidatedAsync(vendedor);

            _guardando = false;
            btnGuardarVendedor.Enabled = true;

            if (!resultado.IsSuccess)
            {
                MostrarAviso(resultado.Error ?? "No se pudo guardar el vendedor.");
                return;
            }

            if (!resultado.Value)
            {
                MostrarAviso(esNuevo
                    ? "No se insertó ninguna fila. Revise que el código no esté repetido."
                    : $"No se actualizó ninguna fila del vendedor '{vendedor.Vendor_id}'.");
                return;
            }

            // En un alta, el id guardado es el recién creado: lo necesita VolverAConsulta para
            // volver a mostrarlo y dejar seleccionada su fila.
            if (esNuevo)
            {
                _idEnAlta = vendedor.Vendor_id;
            }

            await CargarCatalogoAsync(vendedor.Vendor_id);
            VolverAConsulta();
        }

        /// <summary>
        /// Recarga el catálogo desde la base y selecciona opcionalmente un ID.
        /// </summary>
        private async Task CargarCatalogoAsync(string? seleccionarId = null)
        {
            try
            {
                _dtVendedores = await _vendedoresService.LoadListadoAsync();
                gridVendedores.DataSource = _dtVendedores;

                // Reaplica texto + radios (el DataTable nuevo llega sin RowFilter) y repinta
                // las filas desactivadas, que se pierden al cambiarse el DataSource.
                AplicarFiltros();

                if (seleccionarId is not null)
                {
                    SeleccionarFilaEnGrid(seleccionarId);
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al recargar Vendedores: " + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Lectura del formulario (pantalla → Vendedor)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Monta el vendedor con lo que hay en el formulario. Devuelve un Result porque
        /// los campos pueden traer texto inválido, y eso hay que poder reportarlo antes de tocar la base.
        /// </summary>
        private Result<Vendedor> ConstruirVendedorDesdeFormulario()
        {
            int codigoInterno = 0;
            string textoInterno = (txtCodigoInterno.Text ?? string.Empty).Trim();
            if (textoInterno.Length > 0 && textoInterno != "—")
            {
                int.TryParse(textoInterno, out codigoInterno);
            }

            Vendedor vendedor = new()
            {
                // En Editar el código está bloqueado, así que se toma el de la fila abierta y no
                // el de la pantalla: si el usuario movió la selección del grid a media edición,
                // el código que manda es el del vendedor que se está editando.
                Vendor_id = (_modo == ModoFormulario.Nuevo ? txtValorId.Text : _idEnEdicion ?? string.Empty).Trim(),
                // El interno nunca se escribe a mano: en Nuevo viene calculado y en Editar
                // es el de la fila abierta (la caja es de solo lectura).
                CodigoInterno = codigoInterno,
                Vendor_name = txtValorNombre.Text.Trim(),
                Correo = txtValorCorreo.Text.Trim(),
                Phone = txtValorTelefono.Text.Trim(),
                Zona = txtValorZona.Text.Trim(),
                Anulado = !swEstado.Active
            };

            return Result<Vendedor>.Success(vendedor);
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

        // ─────────────────────────────────────────────────────────────────
        // Hoja de Excel con todos los vendedores
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crea un Excel con TODOS los vendedores del catálogo. Se exporta el catálogo entero,
        /// no lo que haya salido en pantalla: un filtro de búsqueda no puede cambiar lo que
        /// el fichero contiene, que es el inventario completo.
        /// </summary>
        private void BtnImportarVendedor_Click(object? sender, EventArgs e)
        {
            // Sin comprobación de permisos: el módulo Vendedores no tiene permisos propios
            // (la tabla permisos no lo incluye), así que pedir uno aquí lo bloqueaba siempre.

            // ExportToExcel lanza si la colección va vacía: se comprueba antes para salir
            // en silencio. No haber nada que exportar no es un error y en el listado ya se
            // ve: el único aviso que queda en exportar es el del fallo.
            if (_dtVendedores.Rows.Count == 0)
            {
                return;
            }

            List<VendedorExportado> filas = _dtVendedores.AsEnumerable().Select(ParaExcel).ToList();

            try
            {
                // Sin aviso de éxito al exportar: el propio ExportToExcel abre el fichero,
                // que ya es la confirmación. Los fallos siguen avisando.
                _exportDataService.ExportToExcel(filas, "Vendedores.xlsx");
            }
            catch (Exception ex)
            {
                // El servicio ya avisa por ServiceErrors de sus fallos propios; aquí se cubre
                // lo que se le escape (permisos de carpeta, disco lleno, Excel abierto...).
                ServiceErrors.Report("Error al crear la hoja de vendedores: " + ex.Message);
                MostrarAviso("No se pudo crear la hoja de Excel: " + ex.Message);
            }
        }

        /// <summary>
        /// Abre el reporte del catálogo de vendedores en el visor (ReportsViewer). Va contra la
        /// base con la consulta de R.QUERY.VENDERS y trae el catálogo entero, no lo que
        /// esté filtrado en pantalla, igual que en Productos.
        /// </summary>
        private void BtnReporteVendedor_Click(object? sender, EventArgs e)
        {
            // Sin comprobación de permisos, igual que el resto de las acciones del módulo:
            // Vendedores no tiene permisos propios en la tabla permisos.
            try
            {
                _reportsService.Reporte_Vendedores(this, "Catalogo de Vendedores", "Report_Vendedores.rdlc");
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("No se pudo abrir el reporte de vendedores: " + ex.Message);
                MostrarAviso("No se pudo abrir el reporte: " + ex.Message);
            }
        }

        /// <summary>
        /// Proyecta un vendedor a la fila del Excel: el estado en texto, en vez del booleano crudo.
        /// </summary>
        private static VendedorExportado ParaExcel(DataRow row)
        {
            return new VendedorExportado
            {
                Codigo = row.Table.Columns.Contains("vendor_id") ? (row["vendor_id"]?.ToString() ?? string.Empty) : string.Empty,
                Nombre = row.Table.Columns.Contains("vendor_name") ? (row["vendor_name"]?.ToString() ?? string.Empty) : string.Empty,
                Correo = row.Table.Columns.Contains("correo") ? (row["correo"]?.ToString() ?? string.Empty) : string.Empty,
                Telefono = row.Table.Columns.Contains("phone") ? (row["phone"]?.ToString() ?? string.Empty) : string.Empty,
                Zona = row.Table.Columns.Contains("zona") ? (row["zona"]?.ToString() ?? string.Empty) : string.Empty,
                Estado = row.Table.Columns.Contains("status") ? (row["status"]?.ToString() ?? string.Empty) : string.Empty
            };
        }

        /// <summary>
        /// Aviso de validación o de fallo del servicio. Vive en un método aparte, y no en un
        /// MessageBox en línea, para que las pruebas puedan sustituir el diálogo modal por una
        /// llamada recorded: un MessageBox de verdad dentro de un test lo dejaría colgado.
        /// </summary>
        protected virtual void MostrarAviso(string mensaje)
            => MessageBox.Show(mensaje, "Vendedores", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// Confirmación de Cancelar. Mismo motivo que <see cref="MostrarAviso"/>.
        /// </summary>
        protected virtual bool ConfirmarDescarte(string pregunta)
            => MessageBox.Show(this, pregunta, "Vendedores", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes;
    }
}