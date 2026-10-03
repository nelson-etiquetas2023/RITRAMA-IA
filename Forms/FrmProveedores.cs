using System.Data;
using System.Globalization;
using System.Threading.Tasks;
using Ritrama2025.Core;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ClienteService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProveedorService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Proveedores (rediseño 30/70): panel izquierdo con buscador, cuadro
    /// de resumen (total/filtrado) y grid de proveedores; panel derecho con la página de
    /// detalle, que se refresca al seleccionar una fila del grid.
    /// </summary>
    public partial class FrmProveedores : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IProveedorService _proveedoresService;
        private readonly IConsecutivosService _consecutivosService;
        private readonly IExportDataService _exportDataService;
        private readonly IReportsService _reportsService;
        private DataTable _dtProveedores = new();

        /// <summary>Etiqueta y caja del código interno (creadas por código, ver CrearCampoCodigoInterno).</summary>
        private UILabel lblCapCodigoInterno = null!;
        private UITextBox txtCodigoInterno = null!;

        /// <summary>Etiqueta y combo de categoría (creados por código, ver CrearCampoCategoria).</summary>
        private UILabel lblCapCategoria = null!;
        private UIComboBox cboCategoria = null!;

        /// <summary>Etiqueta y caja de dirección de entrega (creadas por código, ver CrearCampoDireccionEntrega).</summary>
        private UILabel lblCapDireccionEntrega = null!;
        private UITextBox txtValorDireccionEntrega = null!;

        /// <summary>Etiqueta y caja de persona de contacto (creadas por código, ver CrearCampoPersonaContacto).</summary>
        private UILabel lblCapPersonaContacto = null!;
        private UITextBox txtValorPersonaContacto = null!;

        /// <summary>Radios del filtro de categoría bajo el buscador (ver CrearFiltroCategoria).</summary>
        private Panel pnlFiltroCategoria = null!;
        private RadioButton rbCatTodos = null!;
        private RadioButton rbCatNacional = null!;
        private RadioButton rbCatInternacional = null!;

        /// <summary>Valores del combo de categoría (los únicos que acepta la validación).</summary>
        private static readonly string[] Categorias = ["Nacional", "Internacional"];

        /// <summary>
        /// Equivalencias para filtrar por categoría sobre los datos ya guardados. El combo
        /// del detalle solo acepta Nacional/Internacional, pero la base arrastra textos viejos
        /// ("Local", "Venta local", "Exterior", "Zona franca") y muchos proveedores sin
        /// categoría. Para que el filtro sirva de algo, esos sinónimos se agrupan. Quien no
        /// tiene categoría no entra en ninguno de los dos radios: solo aparece en "Todos".
        /// </summary>
        private static readonly string[] CategoriasNacionales = ["Nacional", "Local", "Venta local"];

        private static readonly string[] CategoriasInternacionales =
            ["Internacional", "Exterior", "Zona franca", "Zonal Franca"];

        /// <summary>
        /// Estado del formulario. Consulta es el estado por defecto: el detalle se ve pero no se
        /// escribe. Solo Nuevo y Editar habilitan la escritura, cada uno con sus permisos.
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

        /// <summary>Categoría guardada de la fila abierta (null si no tiene). Si al editar el
        /// combo queda intacto, se preserva esta en lugar de exigir elegir.</summary>
        private string? _categoriaOriginal;
        private bool _guardando;

        /// <summary>
        /// Unidad master 1 y 2 tal como venían de la base. Ya no se editan en pantalla, así que
        /// se reenvían tal cual al guardar para no pisar lo que ya estaba.
        /// </summary>
        private bool _unidad1Original;
        private bool _unidad2Original;

        /// <summary>
        /// True cuando el formulario admite escritura: Nuevo crea un proveedor y Editar modifica
        /// el seleccionado. Consulta es el único estado de solo lectura.
        /// </summary>
        private bool EsEditable => _modo is ModoFormulario.Nuevo or ModoFormulario.Editar;

        /// <summary>
        /// El interruptor de estado solo se mueve (y solo se ve) al EDITAR un proveedor que ya
        /// existe. En el alta queda OCULTO porque un proveedor se crea siempre vigente: nace
        /// Activo y se desactiva después, desde Editar, si hace falta. En consulta se ve, pero
        /// bloqueado, para mostrar el estado del proveedor seleccionado.
        /// </summary>
        private bool EstadoEsEditable => _modo is ModoFormulario.Editar;

        /// <summary>
        /// Crea el formulario de proveedores con su servicio de listado.
        /// </summary>
        /// <param name="proveedoresService">Servicio que trae el listado de la base.</param>
        /// <param name="consecutivosService">Servicio que genera el código interno consecutivo.</param>
        public FrmProveedores(IProveedorService proveedoresService, IConsecutivosService consecutivosService, IExportDataService exportDataService, IReportsService reportsService)
        {
            InitializeComponent();
            ArgumentNullException.ThrowIfNull(proveedoresService);
            ArgumentNullException.ThrowIfNull(consecutivosService);
            ArgumentNullException.ThrowIfNull(exportDataService);
            ArgumentNullException.ThrowIfNull(reportsService);
            _proveedoresService = proveedoresService;
            _consecutivosService = consecutivosService;
            _exportDataService = exportDataService;
            _reportsService = reportsService;

            Text = "Proveedores";

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
            CrearCampoCategoria();
            CrearCampoPersonaContacto();
            CrearCampoDireccionEntrega();
            EstilarCamposDetalle();
            EstilarSwitchEstado();
            EstilarSeleccionGrid();
            EstilarBarraHerramientas();
            CrearFiltroCategoria();

            // Filtrado en vivo por nombre sobre el listado ya cargado: el RowFilter del
            // DataTable se refleja solo en el grid enlazado, sin volver a la base.
            txtBuscar.TextChanged += (_, _) => AplicarFiltros();

            // RefrescarDetalle con los dos eventos: el SelectionChanged se difiere al
            // siguiente ciclo de mensajes (y dispara con la fila anterior) mientras que
            // el CurrentCellChanged dispara síncrono — mismo criterio que FrmClientes.
            gridProveedores.SelectionChanged += (_, _) => RefrescarDetalleDeLaFilaActiva();
            gridProveedores.CurrentCellChanged += (_, _) => RefrescarDetalleDeLaFilaActiva();

            // Toolbar del detalle
            btnNuevoProveedor.Click += BtnNuevoProveedor_Click;
            btnEditarProveedor.Click += BtnEditarProveedor_Click;
            btnImportarProveedor.Click += BtnImportarProveedor_Click;
            btnReporteProveedor.Click += BtnReporteProveedor_Click;
            btnGuardarProveedor.Click += BtnGuardarProveedor_Click;
            btnCancelarProveedor.Click += BtnCancelarProveedor_Click;

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

            gridProveedores.ColumnHeadersDefaultCellStyle.BackColor = verde;
            gridProveedores.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            gridProveedores.GridColor = Color.FromArgb(180, 180, 180);
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
        /// Crea la fila "Categoría" justo debajo de "Nombre" (fila 4) 100% por código
        /// para no tocar el Designer: desplaza las filas 4..N una posición e inserta el estilo.
        /// Se llama después de CrearCampoCodigoInterno, así que las filas ya incluyen el interno.
        /// </summary>
        private void CrearCampoCategoria()
        {
            tlpDetalle.SuspendLayout();

            lblCapCategoria = new UILabel
            {
                Name = "lblCapCategoria",
                Text = "Categoría",
                Dock = DockStyle.Fill,
                Font = new Font("JetBrains Mono", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0)
            };
            cboCategoria = new UIComboBox
            {
                Name = "cboCategoria",
                Dock = DockStyle.Fill,
                DropDownStyle = UIDropDownStyle.DropDownList,
                FillColor = Color.White,
                RectColor = Color.FromArgb(180, 180, 180),
                Font = new Font("JetBrains Mono", 9F)
            };
            cboCategoria.Items.AddRange(Categorias);

            const int filaNueva = 4;
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
            tlpDetalle.Controls.Add(lblCapCategoria, 0, filaNueva);
            tlpDetalle.Controls.Add(cboCategoria, 1, filaNueva);

            tlpDetalle.ResumeLayout(true);
        }

        /// <summary>
        /// Selecciona en el combo el valor guardado (insensible a mayúsculas, por filas
        /// legadas). Si el dato no es Nacional ni Internacional, deja el combo vacío en
        /// lugar de inventar una selección.
        /// </summary>
        private static void AsignarCategoria(UIComboBox combo, string? valor)
        {
            combo.SelectedIndex = -1;

            if (string.IsNullOrWhiteSpace(valor) || valor == "—")
            {
                return;
            }

            for (int i = 0; i < Categorias.Length; i++)
            {
                if (string.Equals(Categorias[i], valor.Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    combo.SelectedIndex = i;
                    return;
                }
            }
        }

        /// <summary>Normaliza lo elegido a "Nacional" / "Internacional" o "" si no hay selección.</summary>
        private static string NormalizarCategoria(string? texto)
        {
            foreach (string categoria in Categorias)
            {
                if (string.Equals(categoria, (texto ?? string.Empty).Trim(), StringComparison.OrdinalIgnoreCase))
                {
                    return categoria;
                }
            }

            return string.Empty;
        }

        /// <summary>
        /// Crea la fila "Persona contacto" justo debajo del teléfono (fila 3) 100% por código
        /// para no tocar el Designer: desplaza las filas 3..N una posición e inserta el estilo.
        /// Se llama después de CrearCampoCategoria, así que las filas ya incluyen interno y categoría.
        /// </summary>
        private void CrearCampoPersonaContacto()
        {
            tlpDetalle.SuspendLayout();

            lblCapPersonaContacto = new UILabel
            {
                Name = "lblCapPersonaContacto",
                Text = "Persona contacto",
                Dock = DockStyle.Fill,
                Font = new Font("JetBrains Mono", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0)
            };
            txtValorPersonaContacto = new UITextBox
            {
                Name = "txtValorPersonaContacto",
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                RectColor = Color.FromArgb(180, 180, 180),
                Font = new Font("JetBrains Mono", 9F),
                TextAlignment = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 6, 0),
                Text = "—"
            };

            const int filaNueva = 3;
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
            tlpDetalle.Controls.Add(lblCapPersonaContacto, 0, filaNueva);
            tlpDetalle.Controls.Add(txtValorPersonaContacto, 1, filaNueva);

            tlpDetalle.ResumeLayout(true);
        }

        /// <summary>
        /// Crea la fila "Dir. entrega" justo debajo de la dirección (fila 7) 100% por código
        /// para no tocar el Designer, y renombra la existente a "Dir. facturación": el proveedor
        /// maneja las dos y pueden diferir. Desplaza las filas 7..N una posición.
        /// Se llama después de CrearCampoCategoria, así que las filas ya incluyen interno y categoría.
        /// </summary>
        private void CrearCampoDireccionEntrega()
        {
            tlpDetalle.SuspendLayout();

            lblCapDireccion.Text = "Dir. facturación";
            lblCapDireccionEntrega = new UILabel
            {
                Name = "lblCapDireccionEntrega",
                Text = "Dir. entrega",
                Dock = DockStyle.Fill,
                Font = new Font("JetBrains Mono", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(2, 0, 0, 0)
            };
            txtValorDireccionEntrega = new UITextBox
            {
                Name = "txtValorDireccionEntrega",
                Dock = DockStyle.Fill,
                FillColor = Color.White,
                RectColor = Color.FromArgb(180, 180, 180),
                Font = new Font("JetBrains Mono", 9F),
                TextAlignment = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 6, 0),
                Text = "—"
            };

            const int filaNueva = 7;
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
            tlpDetalle.Controls.Add(lblCapDireccionEntrega, 0, filaNueva);
            tlpDetalle.Controls.Add(txtValorDireccionEntrega, 1, filaNueva);

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
                lblCapId, lblCapNombre, lblCapTelefono, lblCapDireccion,
                lblCapEmail, lblCapEstado
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
                txtValorId, txtValorNombre, txtValorTelefono,
                txtValorPersonaContacto, txtValorDireccion, txtValorEmail
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
        /// El estado del proveedor es un UISwitch con los dos estados del dominio: Activo y
        /// Desactivado (que en la base es status = 'activo'/'desactivado'). Solo se ve y se mueve en Editar: en el
        /// alta está oculto porque un proveedor nuevo nace siempre Activo (ver AplicarModo), y en
        /// consulta se ve bloqueado para mostrar el estado del proveedor seleccionado.
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
            gridProveedores.DefaultCellStyle.SelectionBackColor = Color.Black;
            gridProveedores.DefaultCellStyle.SelectionForeColor = Color.White;
            gridProveedores.RowsDefaultCellStyle.SelectionBackColor = Color.Black;
            gridProveedores.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            // Las impares se pintan con el estilo alterno (StripeOddColor): sin esto la
            // fila impar seleccionada conservaba el verde del diseñador.
            gridProveedores.AlternatingRowsDefaultCellStyle.SelectionBackColor = Color.Black;
            gridProveedores.AlternatingRowsDefaultCellStyle.SelectionForeColor = Color.White;
        }

        /// <summary>
        /// Fondo de las filas de proveedores anulados: rojo de verdad (firebrick), el mismo
        /// que ya usan las filas anuladas de Pedidos, para que la fila se note de un vistazo
        /// sin tener que leer la columna de estado.
        /// </summary>
        internal static readonly Color ColorFondoAnulado = Color.Firebrick;

        /// <summary>
        /// Letra de las filas de proveedores anulados: rosa claro, más clara que el texto
        /// normal, para que se lea sobre <see cref="ColorFondoAnulado"/> y no se funda con el
        /// rojo del fondo.
        /// </summary>
        internal static readonly Color ColorTextoAnulado = Color.FromArgb(255, 214, 214);

        /// <summary>
        /// Pinta en rojo con letra clara las filas de los proveedores anulados, para que el
        /// usuario los distinga de un vistazo sin abrirlos. Pinta fondo y texto, pero no la
        /// selección: si la fila está seleccionada gana el negro con blanco de la barra, porque
        /// el color de selección del grid tiene prioridad sobre el de la celda.
        /// Se repinta al cargar el listado y al cambiar cualquier filtro, porque en ambos
        /// casos el grid cambia de filas.
        /// </summary>
        private void PintarFilasAnuladas()
        {
            foreach (DataGridViewRow fila in gridProveedores.Rows)
            {
                bool anulado = fila.DataBoundItem is DataRowView datos
                    && Texto(datos, "status") == "desactivado";

                foreach (DataGridViewCell celda in fila.Cells)
                {
                    celda.Style.ForeColor = anulado ? ColorTextoAnulado : Color.Empty;
                    celda.Style.BackColor = anulado ? ColorFondoAnulado : Color.Empty;
                }
            }
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
                btnNuevoProveedor, btnEditarProveedor, btnGuardarProveedor, btnCancelarProveedor
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
            btnGuardarProveedor.Image = null;
            btnCancelarProveedor.Image = null;
        }

        /// <summary>
        /// Carga el listado de proveedores antes de mostrar la pestaña. Si la base falla,
        /// se reporta el error y el formulario queda visible con el grid vacío.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _dtProveedores = await _proveedoresService.LoadListadoAsync();
                gridProveedores.DataSource = _dtProveedores;
                PintarFilasAnuladas();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Proveedores: " + ex.Message);
            }
            finally
            {
                ActualizaResumen();
            }
        }

        /// <summary>
        /// Los tres radios de categoría (Todos / Nacional / Internacional) debajo del buscador.
        /// Se crean por código, no en el Designer, para no pelear con el generador visual
        /// (mismo criterio que CrearCampoCategoria). "Todos" es el estado inicial: es el
        /// reset que devuelve la lista completa cuando el usuario ya filtró por otra cosa.
        /// </summary>
        private void CrearFiltroCategoria()
        {
            pnlFiltroCategoria = new Panel
            {
                Name = "pnlFiltroCategoria",
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(8, 4, 8, 4),
                Size = new Size(325, 32)
            };

            FlowLayoutPanel filaRadios = new()
            {
                Name = "flwRadiosCategoria",
                Dock = DockStyle.Fill,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = false,
                AutoSize = false,
                Size = new Size(325, 24)
            };

            rbCatTodos = CrearRadioCategoria("rbCatTodos", "Todos", filaRadios);
            rbCatNacional = CrearRadioCategoria("rbCatNacional", "Nacional", filaRadios);
            rbCatInternacional = CrearRadioCategoria("rbCatInternacional", "Internacional", filaRadios);

            pnlFiltroCategoria.Controls.Add(filaRadios);

            // pnlFiltroCategoria cuelga de panelIzq entre el buscador y el grid. El contador
            // (pnlResumen) ya viene dockeado abajo desde el diseñador, como en Productos.
            //
            // Ojo con el orden: verificado midiendo la Y real de cada panel. WinForms procesa
            // el dockeo en orden INVERSO al de la colección, así que el último control
            // agregado es el que queda más ARRIBA. El diseñador dejó
            // [gridProveedores(Fill), pnlResumen(Bottom), pnlBuscador(Top)]; para meter los
            // radios hay que quitarlos de la cola y volver a agregarlos al revés.
            panelIzq.Controls.Remove(pnlBuscador);
            panelIzq.Controls.Remove(pnlResumen);
            panelIzq.Controls.Add(pnlFiltroCategoria);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Controls.Add(pnlResumen);

            // "Todos" arranca marcado: sin filtro de categoría la lista va completa.
            rbCatTodos.Checked = true;
        }

        /// <summary>Un radio del filtro de categoría, con el estilo del formulario y enganchado
        /// al filtrado. Los tres comparten el mismo panel, que los mantiene excluyentes.</summary>
        private RadioButton CrearRadioCategoria(string nombre, string texto, Control contenedor)
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
        /// Filtra el listado por nombre (comodín SQL, escapando las comillas) y por categoría,
        /// combinando ambas condiciones con AND. "Todos" no agrega condición de categoría, así
        /// que devuelve la lista completa. Actualiza también el cuadro de resumen.
        /// </summary>
        private void AplicarFiltros()
        {
            List<string> condiciones = [];

            string texto = txtBuscar.Text?.Trim() ?? string.Empty;
            if (texto.Length > 0)
            {
                condiciones.Add($"Proveedor_Name LIKE '%{texto.Replace("'", "''")}%'");
            }

            if (rbCatNacional.Checked)
            {
                condiciones.Add(CondicionCategoria(CategoriasNacionales));
            }
            else if (rbCatInternacional.Checked)
            {
                condiciones.Add(CondicionCategoria(CategoriasInternacionales));
            }

            _dtProveedores.DefaultView.RowFilter = string.Join(" AND ", condiciones);
            PintarFilasAnuladas();
            ActualizaResumen();
        }

        /// <summary>
        /// Condición de categoría con equivalencias: la base tiene textos viejos ("Local",
        /// "Exterior", "Zona franca"...) que el combo del detalle ya no maneja, así que se
        /// agrupan bajo Nacional / Internacional. Los valores van entre comillas simples para
        /// que el RowFilter los compare como texto.
        /// </summary>
        private static string CondicionCategoria(IEnumerable<string> valores)
        {
            string lista = string.Join(", ", valores.Select(v => $"'{v}'"));
            return $"categoria IN ({lista})";
        }

        /// <summary>
        /// Cuadro de resumen bajo el buscador: total de proveedores sin filtro, o
        /// cuántos se muestran del total cuando el filtro está activo. Cuenta como
        /// activo tanto el texto buscado como el radio de categoría.
        /// </summary>
        private void ActualizaResumen()
        {
            int total = _dtProveedores.Rows.Count;
            int visibles = _dtProveedores.DefaultView.Count;
            bool filtrando = (txtBuscar.Text?.Trim().Length ?? 0) > 0 || HayFiltroCategoria();

            lblResumen.Text = !filtrando
                ? $"Total: {total} {(total == 1 ? "proveedor" : "proveedores")}"
                : $"Mostrando {visibles} de {total} {(total == 1 ? "proveedor" : "proveedores")}";
        }

        /// <summary>True si el usuario eligió una categoría concreta (no "Todos").</summary>
        private bool HayFiltroCategoria() => rbCatNacional.Checked || rbCatInternacional.Checked;

        /// <summary>
        /// Único camino que vuelca la fila activa en la pestaña de detalle y en la barra de acciones.
        /// Si no hay fila, el detalle se limpia. Mientras se está escribiendo (Nuevo o Editar)
        /// no se toca el detalle: el grid sigue siendo navegable, pero recargar sus campos
        /// dejaría al usuario corrigiendo los datos de un proveedor sobre la pantalla de otro,
        /// y Guardar los guardaría como el equivocado.
        /// </summary>
        private void RefrescarDetalleDeLaFilaActiva()
        {
            if (EsEditable)
            {
                return;
            }

            DataRowView? fila = gridProveedores.CurrentRow?.DataBoundItem as DataRowView;
            _idSeleccionado = fila is not null ? Texto(fila, "Proveedor_Id") : null;

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
            txtValorId.Text = Texto(fila, "Proveedor_Id");
            txtCodigoInterno.Text = TextoCodigoInterno(EnteroCodigoInterno(fila));
            txtValorNombre.Text = Texto(fila, "Proveedor_Name");
            string categoriaGuardada = Texto(fila, "categoria");
            _categoriaOriginal = categoriaGuardada == "—" ? null : categoriaGuardada;
            AsignarCategoria(cboCategoria, categoriaGuardada);
            txtValorTelefono.Text = Texto(fila, "phone");
            txtValorPersonaContacto.Text = Texto(fila, "persona_contacto");
            txtValorDireccion.Text = Texto(fila, "direccion");
            txtValorDireccionEntrega.Text = Texto(fila, "direccion_entrega");
            txtValorEmail.Text = Texto(fila, "email");
            _unidad1Original = Bit(fila, "unidad_master_1");
            _unidad2Original = Bit(fila, "unidad_master_2");
            PintarEstado(Texto(fila, "status") == "activo");
        }

        /// <summary>
        /// Pinta el estado del proveedor en el switch (true = Activo, false = Desactivado).
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
            cboCategoria.SelectedIndex = -1;
            _categoriaOriginal = null;
            _unidad1Original = false;
            _unidad2Original = false;
            txtValorTelefono.Text = "—";
            txtValorPersonaContacto.Text = "—";
            txtValorDireccion.Text = "—";
            txtValorDireccionEntrega.Text = "—";
            txtValorEmail.Text = "—";
            PintarEstado(true);
        }

        /// <summary>
        /// Habilita/deshabilita los botones de la barra según el modo y la selección.
        /// </summary>
        private void ActualizarBotonesBarra(DataRowView? fila = null)
        {
            btnNuevoProveedor.Visible = !EsEditable;
            btnEditarProveedor.Visible = !EsEditable;
            btnGuardarProveedor.Visible = EsEditable;
            btnCancelarProveedor.Visible = EsEditable;

            btnNuevoProveedor.Enabled = !EsEditable;
            btnEditarProveedor.Enabled = !EsEditable && fila is not null && PermisoHelper.PuedeEditar("Proveedores");
            btnGuardarProveedor.Enabled = EsEditable;
            btnCancelarProveedor.Enabled = EsEditable;
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
            // calculados y en Editar porque son la identidad del proveedor.
            // ReadOnly y no Enabled=false: permite pintar valores por código.
            swEstado.ReadOnly = !EstadoEsEditable;
            txtValorId.ReadOnly = true;
            txtCodigoInterno.ReadOnly = true;

            txtValorNombre.ReadOnly = !editable;
            txtValorTelefono.ReadOnly = !editable;
            txtValorPersonaContacto.ReadOnly = !editable;
            txtValorDireccion.ReadOnly = !editable;
            txtValorDireccionEntrega.ReadOnly = !editable;
            txtValorEmail.ReadOnly = !editable;

            // Combos: ReadOnly solo impide escribir (verificado en FrmPedidos: el desplegable
            // se sigue abriendo con el ratón), así que en solo lectura también se deshabilitan.
            cboCategoria.ReadOnly = !editable;
            cboCategoria.Enabled = editable;

            // El interruptor NO SE MUESTRA en alta. Un proveedor nuevo nace siempre vigente.
            swEstado.Visible = modo is not ModoFormulario.Nuevo;
            lblCapEstado.Visible = modo is not ModoFormulario.Nuevo;

            // El filtro se USA en consulta, así que va habilitado mientras no se está
            // escribiendo. Solo se bloquea en Nuevo y Editar.
            txtBuscar.Enabled = !editable;

            // En escritura se ocultan Nuevo y Editar, y aparecen Guardar y Cancelar.
            ActualizarBotonesBarra(gridProveedores.CurrentRow?.DataBoundItem as DataRowView);
        }

        // ─────────────────────────────────────────────────────────────────
        // Nuevo
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Entra en alta: vacía el detalle, lo deja escribible y pone los valores de partida.
        /// El permiso se comprueba aquí.
        /// </summary>
        private void BtnNuevoProveedor_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeCrear("Proveedores"))
            {
                MostrarAviso("No tiene permiso para crear proveedores.");
                return;
            }

            // Los dos códigos los calcula el sistema: GUID para el primario e
            // interno 1, 2, 3... Sin interno no se entra a Nuevo (no se guarda en 0).
            int interno;
            try
            {
                interno = _consecutivosService.GetAndIncrementConsecProveedor();
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

        /// <summary>Entra en edición con el proveedor de la fila activa ya volcado en el detalle.</summary>
        private void BtnEditarProveedor_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeEditar("Proveedores"))
            {
                MostrarAviso("No tiene permiso para editar proveedores.");
                return;
            }

            DataRowView? fila = gridProveedores.CurrentRow?.DataBoundItem as DataRowView;
            if (fila is null)
            {
                MostrarAviso("Seleccione primero el proveedor que quiere editar.");
                return;
            }

            _idEnAlta = null;
            _idEnEdicion = Texto(fila, "Proveedor_Id");
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
        private void BtnCancelarProveedor_Click(object? sender, EventArgs e)
        {
            string pregunta = _modo == ModoFormulario.Nuevo
                ? "¿Descartar el proveedor nuevo? Se perderá lo que haya escrito."
                : "¿Descartar los cambios de este proveedor?";

            if (!ConfirmarDescarte(pregunta))
            {
                return;
            }

            VolverAConsulta();
        }

        /// <summary>
        /// Vuelve al estado de consulta dejando el detalle como estaba antes de empezar a
        /// escribir: el proveedor editado, o vacío si era un alta que se ha descartado.
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
                    _idSeleccionado = Texto(fila, "Proveedor_Id");
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
        /// Busca un proveedor por ID en el DataTable cargado.
        /// </summary>
        private DataRowView? BuscarEnCatalogo(string id)
        {
            DataRow[] filas = _dtProveedores.Select($"Proveedor_Id = '{id.Replace("'", "''")}'");
            return filas.Length > 0 ? new DataView(filas.CopyToDataTable())[0] : null;
        }

        /// <summary>
        /// Selecciona la fila del grid que corresponde al ID dado.
        /// </summary>
        private void SeleccionarFilaEnGrid(string id)
        {
            foreach (DataGridViewRow row in gridProveedores.Rows)
            {
                if (row.DataBoundItem is DataRowView fila && Texto(fila, "Proveedor_Id") == id)
                {
                    gridProveedores.ClearSelection();
                    row.Selected = true;
                    gridProveedores.CurrentCell = row.Cells[0];
                    break;
                }
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Guardar
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Guarda el proveedor del formulario, sea alta nueva o edición, y refresca el listado.
        /// Ante un fallo no se sale del modo de escritura: lo escrito se queda en pantalla para
        /// que el usuario lo corrija en vez de volver a teclearlo.
        /// </summary>
        private async void BtnGuardarProveedor_Click(object? sender, EventArgs e)
        {
            if (_guardando)
            {
                return;
            }

            bool esNuevo = _modo == ModoFormulario.Nuevo;
            if (esNuevo ? !PermisoHelper.PuedeCrear("Proveedores") : !PermisoHelper.PuedeEditar("Proveedores"))
            {
                MostrarAviso("No tiene permiso para guardar este proveedor.");
                return;
            }

            Result<Proveedor> construccion = ConstruirProveedorDesdeFormulario();
            if (!construccion.IsSuccess)
            {
                MostrarAviso(construccion.Error ?? "Revise los datos del proveedor.");
                return;
            }

            Proveedor proveedor = construccion.Value!;

            // Validación de dominio antes de ir a la base
            Result validacion = _proveedoresService.ValidateProveedor(proveedor);
            if (!validacion.IsSuccess)
            {
                MostrarAviso(validacion.Error ?? "Revise los datos del proveedor.");
                return;
            }

            _guardando = true;
            btnGuardarProveedor.Enabled = false;

            Result<bool> resultado = esNuevo
                ? await _proveedoresService.AddValidatedAsync(proveedor)
                : await _proveedoresService.UpdateValidatedAsync(proveedor);

            _guardando = false;
            btnGuardarProveedor.Enabled = true;

            if (!resultado.IsSuccess)
            {
                MostrarAviso(resultado.Error ?? "No se pudo guardar el proveedor.");
                return;
            }

            if (!resultado.Value)
            {
                MostrarAviso(esNuevo
                    ? "No se insertó ninguna fila. Revise que el código no esté repetido."
                    : $"No se actualizó ninguna fila del proveedor '{proveedor.Proveedor_Id}'.");
                return;
            }

            // En un alta, el id guardado es el recién creado: lo necesita VolverAConsulta para
            // volver a mostrarlo y dejar seleccionada su fila.
            if (esNuevo)
            {
                _idEnAlta = proveedor.Proveedor_Id;
            }

            await CargarCatalogoAsync(proveedor.Proveedor_Id);
            VolverAConsulta();
        }

        /// <summary>
        /// Recarga el catálogo desde la base y selecciona opcionalmente un ID.
        /// </summary>
        private async Task CargarCatalogoAsync(string? seleccionarId = null)
        {
            try
            {
                _dtProveedores = await _proveedoresService.LoadListadoAsync();
                gridProveedores.DataSource = _dtProveedores;
                ActualizaResumen();

                if (seleccionarId is not null)
                {
                    SeleccionarFilaEnGrid(seleccionarId);
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al recargar Proveedores: " + ex.Message);
            }
        }

        // ─────────────────────────────────────────────────────────────────
        // Lectura del formulario (pantalla → Proveedor)
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Monta el proveedor con lo que hay en el formulario. Devuelve un Result porque
        /// los campos pueden traer texto inválido, y eso hay que poder reportarlo antes de tocar la base.
        /// </summary>
        private Result<Proveedor> ConstruirProveedorDesdeFormulario()
        {
            int codigoInterno = 0;
            string textoInterno = (txtCodigoInterno.Text ?? string.Empty).Trim();
            if (textoInterno.Length > 0 && textoInterno != "—")
            {
                int.TryParse(textoInterno, out codigoInterno);
            }

            Proveedor proveedor = new()
            {
                // En Editar el código está bloqueado, así que se toma el de la fila abierta y no
                // el de la pantalla: si el usuario movió la selección del grid a media edición,
                // el código que manda es el del proveedor que se está editando.
                Proveedor_Id = (_modo == ModoFormulario.Nuevo ? txtValorId.Text : _idEnEdicion ?? string.Empty).Trim(),
                // El interno nunca se escribe a mano: en Nuevo viene calculado y en Editar
                // es el de la fila abierta (la caja es de solo lectura).
                CodigoInterno = codigoInterno,
                // Si el combo quedó intacto se preserva la guardada (aunque sea un valor
                // legado fuera del combo); en Nuevo no hay original y el alta exige elegir.
                Categoria = cboCategoria.SelectedIndex >= 0
                    ? NormalizarCategoria(cboCategoria.SelectedItem?.ToString())
                    : (_categoriaOriginal ?? string.Empty),
                Proveedor_Name = SinGuion(txtValorNombre.Text),
                Phone = SinGuion(txtValorTelefono.Text),
                PersonaContacto = SinGuion(txtValorPersonaContacto.Text),
                Direccion = SinGuion(txtValorDireccion.Text),
                DireccionEntrega = SinGuion(txtValorDireccionEntrega.Text),
                Email = SinGuion(txtValorEmail.Text),
                // Unidad master 1 y 2 ya no son editables (se quitaron del detalle), así que
                // se reenvía lo que ya tenía la fila: si se dejaran en false, guardar cualquier
                // otro campo de un proveedor con unidad master activa la perdería.
                Unidad_master_1 = _unidad1Original,
                Unidad_master_2 = _unidad2Original,
                Anulado = !swEstado.Active
            };

            return Result<Proveedor>.Success(proveedor);
        }

        /// <summary>La "—" es solo visual de "sin valor": al guardar equivale a vacío para
        /// no grabar la raya literal cuando el campo no se tocó (p. ej. teléfono legado NULL).</summary>
        private static string SinGuion(string? texto)
        {
            string limpio = (texto ?? string.Empty).Trim();
            return limpio == "—" ? string.Empty : limpio;
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

        /// <summary>Lee un bit de la fila; false si la columna no existe o no es booleano.</summary>
        private static bool Bit(DataRowView fila, string columna)
        {
            if (!fila.Row.Table.Columns.Contains(columna) || fila[columna] is not bool valor)
            {
                return false;
            }

            return valor;
        }

        // ─────────────────────────────────────────────────────────────────
        // Hoja de Excel con todos los proveedores
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crea un Excel con TODOS los proveedores del catálogo. Se exporta el catálogo entero,
        /// no lo que haya salido en pantalla: un filtro de categoría o una búsqueda no pueden
        /// cambiar lo que el fichero contiene, que es el inventario completo.
        /// </summary>
        private void BtnImportarProveedor_Click(object? sender, EventArgs e)
        {
            // Exportar no crea ni modifica datos, así que basta el permiso de ver. Se valida
            // en el clic y no deshabilitando el botón, igual que el resto de la barra.
            if (!PermisoHelper.PuedeVer("Proveedores"))
            {
                MostrarAviso("No tiene permiso para ver los proveedores.");
                return;
            }

            // ExportToExcel lanza si la colección va vacía: se comprueba antes para salir
            // en silencio. No haber nada que exportar no es un error y en el listado ya se
            // ve: el único aviso que queda en exportar es el del fallo.
            if (_dtProveedores.Rows.Count == 0)
            {
                return;
            }

            List<ProveedorExportado> filas = _dtProveedores.AsEnumerable().Select(ParaExcel).ToList();

            try
            {
                // Sin aviso de éxito al exportar: el propio ExportToExcel abre el fichero,
                // que ya es la confirmación. Los fallos siguen avisando.
                _exportDataService.ExportToExcel(filas, "Proveedores.xlsx");
            }
            catch (Exception ex)
            {
                // El servicio ya avisa por ServiceErrors de sus fallos propios; aquí se cubre
                // lo que se le escape (permisos de carpeta, disco lleno, Excel abierto...).
                ServiceErrors.Report("Error al crear la hoja de proveedores: " + ex.Message);
                MostrarAviso("No se pudo crear la hoja de Excel: " + ex.Message);
            }
        }

        /// <summary>
        /// Abre el reporte del catálogo de proveedores en el visor (ReportsViewer). Va contra la
        /// base con la consulta de R.QUERY.PROVIDERS y trae el catálogo entero, no lo que
        /// esté filtrado en pantalla, igual que en Productos.
        /// </summary>
        private void BtnReporteProveedor_Click(object? sender, EventArgs e)
        {
            // Ver el reporte no crea ni modifica datos, así que basta el permiso de ver.
            if (!PermisoHelper.PuedeVer("Proveedores"))
            {
                MostrarAviso("No tiene permiso para ver los proveedores.");
                return;
            }

            try
            {
                _reportsService.Reporte_Proveedores(this, "Catalogo de Proveedores", "Report_Proveedores.rdlc");
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("No se pudo abrir el reporte de proveedores: " + ex.Message);
                MostrarAviso("No se pudo abrir el reporte: " + ex.Message);
            }
        }

        /// <summary>
        /// Proyecta un proveedor a la fila del Excel: el estado en texto, en vez del booleano crudo.
        /// </summary>
        private static ProveedorExportado ParaExcel(DataRow row)
        {
            return new ProveedorExportado
            {
                Codigo = row.Table.Columns.Contains("Proveedor_Id") ? (row["Proveedor_Id"]?.ToString() ?? string.Empty) : string.Empty,
                Nombre = row.Table.Columns.Contains("Proveedor_Name") ? (row["Proveedor_Name"]?.ToString() ?? string.Empty) : string.Empty,
                Telefono = row.Table.Columns.Contains("phone") ? (row["phone"]?.ToString() ?? string.Empty) : string.Empty,
                PersonaContacto = row.Table.Columns.Contains("persona_contacto") ? (row["persona_contacto"]?.ToString() ?? string.Empty) : string.Empty,
                Direccion = row.Table.Columns.Contains("direccion") ? (row["direccion"]?.ToString() ?? string.Empty) : string.Empty,
                DireccionEntrega = row.Table.Columns.Contains("direccion_entrega") ? (row["direccion_entrega"]?.ToString() ?? string.Empty) : string.Empty,
                Email = row.Table.Columns.Contains("email") ? (row["email"]?.ToString() ?? string.Empty) : string.Empty,
                Categoria = row.Table.Columns.Contains("categoria") ? (row["categoria"]?.ToString() ?? string.Empty) : string.Empty,
                Estado = row.Table.Columns.Contains("status") ? (row["status"]?.ToString() ?? string.Empty) : string.Empty
            };
        }

        /// <summary>
        /// Aviso de validación o de fallo del servicio. Vive en un método aparte, y no en un
        /// MessageBox en línea, para que las pruebas puedan sustituir el diálogo modal por una
        /// llamada recorded: un MessageBox de verdad dentro de un test lo dejaría colgado.
        /// </summary>
        protected virtual void MostrarAviso(string mensaje)
            => MessageBox.Show(mensaje, "Proveedores", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// Confirmación de Cancelar. Mismo motivo que <see cref="MostrarAviso"/>.
        /// </summary>
        protected virtual bool ConfirmarDescarte(string pregunta)
            => MessageBox.Show(this, pregunta, "Proveedores", MessageBoxButtons.YesNo, MessageBoxIcon.Question)
                == DialogResult.Yes;
    }
}