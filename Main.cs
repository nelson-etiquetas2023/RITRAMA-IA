using System.Data;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ritrama2025.Forms;
using Ritrama2025.Forms.Seleccion;
using Ritrama2025.Helpers;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.DespachoService.DespachoService;
using Ritrama2025.Services.SeguridadService;
using Sunny.UI;

namespace Ritrama2025
{
    public partial class Main : UIForm
    {
        private IConfiguration Config { get; set; } = null!;
        private readonly FormManager _formManager;
        private string MODE = "";
        private PrivateFontCollection _pfc = new();

        private static readonly Color colorFondoSidebar = Color.FromArgb(38, 38, 44);

        private Button Btn_toggleSidebar = null!;
        private Button btn_ventas = null!;
        private Panel pnlVentas = null!;
        private Button btn_vendedores = null!;
        private bool _ventasExpandido = false;
        private const int VENTAS_SUBBUTTON_HEIGHT = 50;
        private readonly Dictionary<Button, string> _menuButtonTexts = new();
        private readonly Dictionary<Button, string> _menuButtonToolTips = new();
        private readonly System.Windows.Forms.ToolTip _sidebarToolTip = new();
        private System.Windows.Forms.Timer _toggleTimer = null!;
        private bool _isSidebarExpanded = true;
        private bool _isAnimating;
        private int _targetWidth;
        private const int SIDEBAR_WIDTH_EXPANDED = 210;
        private const int SIDEBAR_WIDTH_COLLAPSED = 55;

        private Panel? panel_barraUsuario;

        private readonly SessionManager _sessionManager;
        private System.Windows.Forms.Timer _sessionTimer = null!;
        private IServiceProvider _serviceProvider = null!;

        public Main(FormManager formManager, IConfiguration config, IServiceProvider serviceProvider)
        {
            _serviceProvider = serviceProvider;
            InitializeComponent();
            CargarFuenteBebasNeue();
            AplicarTemaSidebarOscuro();
            InicializarSidebarColapsable();
            CrearGrupoVentas();
            ActualizarVentasHeader();
            _formManager = formManager;
            Config = config;
            _formManager.HostTabControl = tabContent;
            // El boton "Reportes" abre la auditoria de inconsistencias y
            // validaciones (pantalla bajo demanda, sin modales).
            button4.Text = "Auditoria";
            button4.Click += Bot_auditoria_Click;
            // InicializarSidebarColapsable ya capturo el texto anterior ("Reportes"):
            // se actualiza para que colapsar/expandir conserve "Auditoria".
            _menuButtonTexts[button4] = "Auditoria";
            _menuButtonToolTips[button4] = "10. AUDITORIA";
            AplicarEsquinasRedondeadas();
            tabContent.Resize += (s, e) => AplicarEsquinasRedondeadas();
            AplicarTemaOscuroTabContent();
            ConfigurarCierrePestanas();

            // Quita el bloque de foto de usuario generico y labels: ocupa espacio y
            // se dibuja encima de las opciones del menu, tapandolas. Al removerlo,
            // entran todas las opciones del sidebar (Orden de Corte, Inventario, etc.).
            panel1.Controls.Remove(panel_DATA);

            // Inicializar session manager (15 min timeout)
            _sessionManager = new SessionManager();
            _sessionManager.SessionExpired += OnSessionExpired;
            _sessionManager.Start();

            _sessionTimer = new System.Windows.Forms.Timer { Interval = 60000 };
            _sessionTimer.Tick += SessionTimer_Tick;
            _sessionTimer.Start();

            // Crear barra de usuario con avatar, nombre, tiempo y opciones
            CrearBarraUsuario();

            // Detectar actividad del usuario para resetear timeout
            MouseMove += (s, e) => _sessionManager.ResetActivity();
            MouseClick += (s, e) => _sessionManager.ResetActivity();
            KeyDown += (s, e) => _sessionManager.ResetActivity();
            KeyPress += (s, e) => _sessionManager.ResetActivity();
            tabContent.MouseMove += (s, e) => _sessionManager.ResetActivity();
            tabContent.MouseClick += (s, e) => _sessionManager.ResetActivity();
            panel1.MouseMove += (s, e) => _sessionManager.ResetActivity();
            panel1.MouseClick += (s, e) => _sessionManager.ResetActivity();

            // Mantiene el sidebar siempre visible y oscuro: cuando cambia o se repinta
            // la pestana central (por ejemplo mientras un modulo carga datos), lo trae
            // al frente y fuerza su repintado para que nunca quede en blanco.
            tabContent.SelectedIndexChanged += (s, e) =>
            {
                RefrescarSidebar();
                RefrescarFormSeleccionado();
            };
            tabContent.Resize += (s, e) => RefrescarSidebar();

            // Tema SunnyUI para el shell (recorre recursivamente los controles hijos,
            // por lo que tambien tematiza los modulos embebidos en pestañas).
            // Se pasa el contenedor al constructor para que el componente quede
            // correctamente "sitiado" y pueda aplicar el estilo a los controles
            // WinForms estandar (Button, TextBox, DataGridView, TabControl, etc.).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            // SunnyUI solo trae recursos zh-CN (chino) y en-US (inglés). Registramos
            // nuestra propia clase de recursos en español y la activamos para que el
            // datepicker (y el resto de controles) muestren español, no chino.
            UIStyles.BuiltInResources.TryAdd(new CultureInfo("es-ES").LCID, new es_ES_Resources());
            UIStyles.CultureInfo = new CultureInfo("es-ES");
        }

        private void ConfigurarCierrePestanas()
        {
            ContextMenuStrip menu = new ContextMenuStrip();
            ToolStripMenuItem itemCerrar = new ToolStripMenuItem("Cerrar pestaña");
            itemCerrar.Click += (s, e) =>
            {
                if (tabContent.SelectedTab != null)
                {
                    Form? form = tabContent.SelectedTab.Controls.OfType<Form>().FirstOrDefault();
                    tabContent.TabPages.Remove(tabContent.SelectedTab);
                    form?.Close();
                }
            };
            menu.Items.Add(itemCerrar);
            tabContent.ContextMenuStrip = menu;

            tabContent.MouseDown += (s, e) =>
            {
                if (e.Button == MouseButtons.Right)
                {
                    int index = -1;
                    for (int i = 0; i < tabContent.TabPages.Count; i++)
                    {
                        if (tabContent.GetTabRect(i).Contains(e.Location))
                        {
                            index = i;
                            break;
                        }
                    }
                    if (index >= 0)
                    {
                        tabContent.SelectedIndex = index;
                    }
                }
            };
        }

        private void CargarFuenteBebasNeue()
        {
            string fontPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Fonts", "BebasNeue-Regular.ttf");
            if (File.Exists(fontPath))
            {
                byte[] fontBytes = File.ReadAllBytes(fontPath);
                IntPtr ptr = Marshal.AllocCoTaskMem(fontBytes.Length);
                Marshal.Copy(fontBytes, 0, ptr, fontBytes.Length);
                _pfc.AddMemoryFont(ptr, fontBytes.Length);
                Marshal.FreeCoTaskMem(ptr);
            }
        }

        private Font ObtenerFuenteSidebar(float size)
        {
            if (_pfc.Families.Length > 0)
            {
                return new Font(_pfc.Families[0], size, FontStyle.Bold);
            }

            return new Font("Impact", size, FontStyle.Bold);
        }

        private void AplicarTemaSidebarOscuro()
        {
            Color fondoOscuro = colorFondoSidebar;
            Color textoClaro = Color.White;
            Color verde = Color.FromArgb(110, 190, 40);
            Color hover = Color.FromArgb(70, 140, 25); // verde oscuro del tema

            panel1.BackColor = fondoOscuro;
            panel_DATA.BackColor = fondoOscuro;
            panel_version_software.BackColor = fondoOscuro;

            // Botones del menú y labels dentro del sidebar.
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.BackColor = fondoOscuro;
                    btn.ForeColor = Color.FromArgb(205, 205, 215);
                    btn.Font = ObtenerFuenteSidebar(12f);
                    btn.FlatStyle = FlatStyle.Flat;
                    btn.FlatAppearance.BorderSize = 0;
                    btn.FlatAppearance.MouseOverBackColor = hover;
                    btn.FlatAppearance.MouseDownBackColor = hover;
                    btn.TextImageRelation = TextImageRelation.ImageBeforeText;
                    btn.ImageAlign = ContentAlignment.MiddleLeft;
                    btn.TextAlign = ContentAlignment.MiddleLeft;
                    btn.Padding = new Padding(12, 0, 0, 0);
                    btn.MouseEnter += (s, e) => btn.ForeColor = Color.White;
                    btn.MouseLeave += (s, e) => btn.ForeColor = Color.FromArgb(205, 205, 215);
                }
                else if (ctrl is Label lbl)
                {
                    lbl.ForeColor = textoClaro;
                }
            }

            // Controles dentro de panel_DATA (versión, usuario, modo).
            foreach (Control ctrl in panel_DATA.Controls)
            {
                if (ctrl is Label lbl)
                {
                    lbl.ForeColor = textoClaro;
                }
                else if (ctrl is Panel p && p != panel2)
                {
                    p.BackColor = fondoOscuro;
                }
            }
            lbl_user_name.ForeColor = textoClaro;
        }

        private void AplicarEsquinasRedondeadas()
        {
            if (tabContent == null)
            {
                return;
            }

            Rectangle rect = tabContent.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0)
            {
                return;
            }

            int radio = 16;
            GraphicsPath gp = new GraphicsPath();
            gp.StartFigure();
            gp.AddArc(0, 0, radio, radio, 180, 90);
            gp.AddArc(rect.Width - radio, 0, radio, radio, 270, 90);
            gp.AddArc(rect.Width - radio, rect.Height - radio, radio, radio, 0, 90);
            gp.AddArc(0, rect.Height - radio, radio, radio, 90, 90);
            gp.CloseFigure();
            tabContent.Region = new Region(gp);
        }

        // Aplica tema claro al area de pestanas central: fondo blanco y pestana
        // seleccionada en verde, igual que el tema de los modulos embebidos.
        private void AplicarTemaOscuroTabContent()
        {
            if (tabContent == null)
            {
                return;
            }

            Color verde = Color.FromArgb(110, 190, 40);
            Color verdeOsc = Color.FromArgb(70, 140, 25);
            tabContent.FillColor = Color.White;
            tabContent.BackColor = Color.White;
            tabContent.TabBackColor = Color.White;
            tabContent.TabSelectedColor = verde;
            tabContent.TabSelectedForeColor = Color.White;
            tabContent.TabSelectedHighColor = verdeOsc;
            tabContent.TabSelectedHighColorSize = 3;
            tabContent.TabUnSelectedColor = Color.White;
            tabContent.TabUnSelectedForeColor = Color.FromArgb(48, 48, 48);
            tabContent.Style = UIStyle.Custom;
            tabContent.GotFocus += (s, e) => tabContent.BackColor = Color.White;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (Config != null)
            {
                MODE = Config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            }
            panel2.BackColor = colorFondoSidebar;
            LAB_MODE_RUN.Text = MODE;
            lbl_user_name.Text = $"Usuario : {SesionActual.Usuario?.NombreCompleto ?? "Sin sesión"}";
        }
        private void Bot_despacho_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Despacho"))
            {
                return;
            }

            _formManager.ShowForm<FrmDespacho>();
        }

        private void Bot_ordencorte_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Produccion"))
            {
                return;
            }

            _formManager.ShowForm<FrmOrdenCorte>();
        }

        private void Bot_recepciones_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Recepciones"))
            {
                return;
            }

            _formManager.ShowForm<FrmMateriaPrima>();
        }

        private void OPC_MENU_LABELS_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Etiquetas"))
            {
                return;
            }

            _formManager.ShowForm<FrmCodeBarLabel>();
        }

        private void Bot_products_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Productos"))
            {
                return;
            }

            _formManager.ShowForm<FrmProductos>();
        }

        private void Bot_pedidos_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Pedidos"))
            {
                return;
            }

            _formManager.ShowForm<FrmPedidos>();
        }

        private void Bot_clientes_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Clientes"))
            {
                return;
            }

            _formManager.ShowForm<FrmClientes>();
        }

        private void Bot_inventario_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Inventario"))
            {
                return;
            }

            _formManager.ShowForm<Frm_Inventarios>();
        }

        private void Bot_verlogs_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Reportes"))
            {
                return;
            }

            _formManager.ShowForm<FrmLogViewer>();
        }

        private void Bot_auditoria_Click(object? sender, EventArgs e)
        {
            if (!VerificarPermiso("Reportes"))
            {
                return;
            }

            _formManager.ShowForm<FrmAuditoriaInconsistencias>();
        }

        private void Bot_usuarios_Click(object sender, EventArgs e)
        {
            if (!VerificarPermiso("Usuarios"))
            {
                return;
            }

            _formManager.ShowForm<FrmUsuarios>();
        }

        private bool VerificarPermiso(string modulo)
        {
            if (SesionActual.Usuario?.Username == "admin")
            {
                return true;
            }

            if (PermisoHelper.PuedeVer(modulo))
            {
                return true;
            }

            MessageBox.Show($"No tiene permiso para acceder al módulo {modulo}.",
                "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return false;
        }

        private void InicializarSidebarColapsable()
        {
            Btn_toggleSidebar = new Button
            {
                Dock = DockStyle.Top,
                Height = 70,
                Text = "\u2630",
                FlatStyle = FlatStyle.Flat,
                BackColor = colorFondoSidebar,
                ForeColor = Color.FromArgb(205, 205, 215),
                Font = new Font("Segoe UI Symbol", 14f),
                ImageAlign = ContentAlignment.MiddleCenter,
                TextImageRelation = TextImageRelation.ImageBeforeText,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            Btn_toggleSidebar.FlatAppearance.BorderSize = 0;
            Btn_toggleSidebar.FlatAppearance.MouseOverBackColor = Color.FromArgb(70, 140, 25);
            Btn_toggleSidebar.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 140, 25);
            Btn_toggleSidebar.MouseEnter += (s, e) => Btn_toggleSidebar.ForeColor = Color.White;
            Btn_toggleSidebar.MouseLeave += (s, e) => Btn_toggleSidebar.ForeColor = Color.FromArgb(205, 205, 215);
            Btn_toggleSidebar.Click += Btn_toggleSidebar_Click;

            panel1.Controls.Add(Btn_toggleSidebar);

            Button[] menuButtons = { bot_ordencorte, bot_inventario, bot_despacho, bot_recepciones, bot_products, button1, button2, button3, button4, OPC_MENU_LABELS, bot_pedidos };
            string[] titulosModulo =
            {
                "1. PRODUCCIÓN", "2. INVENTARIO", "3. DESPACHO", "4. RECEPCIONES", "5. PRODUCTOS",
                "6. CLIENTES", "7. USUARIOS", "8. PROVEEDORES", "9. REPORTES", "10. ETIQUETAS", "11. PEDIDOS"
            };
            for (int i = 0; i < menuButtons.Length; i++)
            {
                _menuButtonTexts[menuButtons[i]] = menuButtons[i].Text;
                _menuButtonToolTips[menuButtons[i]] = titulosModulo[i];
            }

            // Fija el orden visual del sidebar: con Dock=Top el indice MAS ALTO
            // queda arriba, asi que se asigna en orden descendente (hamburguesa al tope).
            Control[] ordenVisual =
            {
                Btn_toggleSidebar, bot_ordencorte, bot_inventario, bot_despacho, bot_recepciones, bot_products,
                button1, button2, button3, button4, OPC_MENU_LABELS, bot_pedidos
            };
            int idxOrden = ordenVisual.Length - 1;
            foreach (Control c in ordenVisual)
            {
                panel1.Controls.SetChildIndex(c, idxOrden--);
            }

            _toggleTimer = new System.Windows.Forms.Timer { Interval = 12 };
            _toggleTimer.Tick += ToggleTimer_Tick;

            _sidebarToolTip.InitialDelay = 300;
            _sidebarToolTip.ReshowDelay = 100;
            _sidebarToolTip.AutoPopDelay = 5000;
            _sidebarToolTip.ShowAlways = true;
        }

        // Grupo Ventas (acordeón): Ventas > Pedido de Ventas, Clientes, Vendedores.
        // Se construye 100% en código para respetar el límite del diseñador
        // (no se toca Main.Designer.cs). Reparenta button1 (Clientes) y
        // bot_pedidos (Pedidos) dentro del sub-panel y crea Vendedores con
        // el selector existente (FrmSeleccion + Frm_AddNew).
        private void CrearGrupoVentas()
        {
            Color fondoSubmenu = Color.FromArgb(30, 30, 36);
            Color textoSubmenu = Color.FromArgb(205, 205, 215);
            Color hover = Color.FromArgb(70, 140, 25);

            panel1.SuspendLayout();

            btn_ventas = new Button
            {
                Dock = DockStyle.Top,
                Height = 70,
                Text = _ventasExpandido ? "▼ Ventas" : "▶ Ventas",
                FlatStyle = FlatStyle.Flat,
                BackColor = colorFondoSidebar,
                ForeColor = textoSubmenu,
                Font = ObtenerFuenteSidebar(12f),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            btn_ventas.FlatAppearance.BorderSize = 0;
            btn_ventas.FlatAppearance.MouseOverBackColor = hover;
            btn_ventas.FlatAppearance.MouseDownBackColor = hover;
            btn_ventas.MouseEnter += (s, e) => btn_ventas.ForeColor = Color.White;
            btn_ventas.MouseLeave += (s, e) => btn_ventas.ForeColor = textoSubmenu;
            btn_ventas.Click += Btn_ventas_Click;

            pnlVentas = new Panel
            {
                Dock = DockStyle.Top,
                Height = _ventasExpandido ? 3 * VENTAS_SUBBUTTON_HEIGHT : 0,
                BackColor = fondoSubmenu,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Visible = _ventasExpandido
            };

            // Saca Clientes y Pedidos del nivel raíz para meterlos al sub-panel.
            panel1.Controls.Remove(button1);
            panel1.Controls.Remove(bot_pedidos);

            ConfigurarSubBotonVentas(bot_pedidos, "Pedido de Ventas", fondoSubmenu, textoSubmenu, hover);
            ConfigurarSubBotonVentas(button1, "Clientes", fondoSubmenu, textoSubmenu, hover);

            btn_vendedores = new Button
            {
                Dock = DockStyle.Top,
                Height = VENTAS_SUBBUTTON_HEIGHT,
                Text = "Vendedores",
                FlatStyle = FlatStyle.Flat,
                BackColor = fondoSubmenu,
                ForeColor = textoSubmenu,
                Font = ObtenerFuenteSidebar(11f),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(28, 0, 0, 0)
            };
            btn_vendedores.FlatAppearance.BorderSize = 0;
            btn_vendedores.FlatAppearance.MouseOverBackColor = hover;
            btn_vendedores.FlatAppearance.MouseDownBackColor = hover;
            btn_vendedores.MouseEnter += (s, e) => btn_vendedores.ForeColor = Color.White;
            btn_vendedores.MouseLeave += (s, e) => btn_vendedores.ForeColor = textoSubmenu;
            if (button2.Image != null)
            {
                btn_vendedores.Image = button2.Image;
            }
            btn_vendedores.Click += Bot_vendedores_Click;

            // Orden dentro del sub-panel (arriba -> abajo): Pedido, Clientes, Vendedores.
            // Con Dock=Top el índice MÁS ALTO queda arriba.
            pnlVentas.Controls.Add(btn_vendedores);
            pnlVentas.Controls.Add(button1);
            pnlVentas.Controls.Add(bot_pedidos);
            pnlVentas.Controls.SetChildIndex(bot_pedidos, 2);
            pnlVentas.Controls.SetChildIndex(button1, 1);
            pnlVentas.Controls.SetChildIndex(btn_vendedores, 0);

            panel1.Controls.Add(btn_ventas);
            panel1.Controls.Add(pnlVentas);

            // Actualiza textos/tooltips: Pedidos pasa a "Pedido de Ventas".
            _menuButtonTexts[bot_pedidos] = "Pedido de Ventas";
            _menuButtonToolTips[bot_pedidos] = "VENTAS - PEDIDO DE VENTAS";
            _menuButtonTexts[button1] = "Clientes";
            _menuButtonToolTips[button1] = "VENTAS - CLIENTES";
            _menuButtonTexts[btn_ventas] = "▼ Ventas";
            _menuButtonToolTips[btn_ventas] = "VENTAS";
            _menuButtonTexts[btn_vendedores] = "Vendedores";
            _menuButtonToolTips[btn_vendedores] = "VENTAS - VENDEDORES";

            if (!_isSidebarExpanded)
            {
                bot_pedidos.Text = string.Empty;
                button1.Text = string.Empty;
                btn_vendedores.Text = string.Empty;
                btn_ventas.Text = string.Empty;
                _sidebarToolTip.SetToolTip(bot_pedidos, "VENTAS - PEDIDO DE VENTAS");
                _sidebarToolTip.SetToolTip(button1, "VENTAS - CLIENTES");
                _sidebarToolTip.SetToolTip(btn_vendedores, "VENTAS - VENDEDORES");
                _sidebarToolTip.SetToolTip(btn_ventas, "Expandir Ventas");
            }
            else
            {
                bot_pedidos.Text = "Pedido de Ventas";
                button1.Text = "Clientes";
                btn_vendedores.Text = "Vendedores";
                btn_ventas.Text = "▼ Ventas";
            }

            // Reordena el sidebar: el grupo Ventas queda tras Productos.
            Control[] nuevoOrden =
            {
                Btn_toggleSidebar, bot_ordencorte, bot_inventario, bot_despacho,
                bot_recepciones, bot_products, btn_ventas, pnlVentas,
                button2, button3, button4, OPC_MENU_LABELS
            };

            // Reordena con SetChildIndex(c, 0): el control va al indice mas bajo y empuja al
            // resto hacia arriba. Con Dock=Top dibuja el indice mas alto arriba, asi que
            // recorriendo el array en orden, el primero queda en lo mas alto.
            //
            // No se deriva el indice de Controls.Count porque eso ata el resultado a cuantos
            // controles hay: hoy Count es 13 y el array tiene 12, asi que un indice derivado de
            // Count acierta por coincidencia. Con un control menos, el Math.Max(0, ...) que
            // hacia falta para no desbordar apilaba las ultimas entradas en el indice 0 y las
            // invertia en silencio. El indice 0 siempre es valido, asi que esta forma no puede
            // desbordar ni al crecer el array.
            foreach (Control c in nuevoOrden)
            {
                if (panel1.Controls.Contains(c))
                {
                    panel1.Controls.SetChildIndex(c, 0);
                }
            }

            // panel_DATA no va en el array, asi que el recorrido lo deja con el indice mas alto
            // y se dibujaria encima del menu. Es Dock=None y se posiciona absoluto abajo
            // (y=706), asi que el indice no cambia donde se ve, pero si el orden de pintado.
            // Bajarlo al fondo deja el resultado igual al que daba el recorrido por indice.
            if (panel1.Controls.Contains(panel_DATA))
            {
                panel1.Controls.SetChildIndex(panel_DATA, 0);
            }

            panel1.ResumeLayout(true);
            panel1.PerformLayout();
            RefrescarSidebar();
        }

        private void ConfigurarSubBotonVentas(Button btn, string texto, Color fondo, Color fore, Color hover)
        {
            btn.Dock = DockStyle.Top;
            btn.Height = VENTAS_SUBBUTTON_HEIGHT;
            btn.Text = texto;
            btn.BackColor = fondo;
            btn.ForeColor = fore;
            btn.Font = ObtenerFuenteSidebar(11f);
            btn.FlatStyle = FlatStyle.Flat;
            btn.FlatAppearance.BorderSize = 0;
            btn.FlatAppearance.MouseOverBackColor = hover;
            btn.FlatAppearance.MouseDownBackColor = hover;
            btn.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn.ImageAlign = ContentAlignment.MiddleLeft;
            btn.TextAlign = ContentAlignment.MiddleLeft;
            btn.Padding = new Padding(28, 0, 0, 0);
        }

        private void Btn_ventas_Click(object? sender, EventArgs e)
        {
            _ventasExpandido = !_ventasExpandido;
            pnlVentas.Visible = _ventasExpandido;
            pnlVentas.Height = _ventasExpandido ? 3 * VENTAS_SUBBUTTON_HEIGHT : 0;
            ActualizarVentasHeader();
            panel1.PerformLayout();
            RefrescarSidebar();
            _sessionManager.ResetActivity();
        }

        private void ActualizarVentasHeader()
        {
            if (_isSidebarExpanded)
            {
                string texto = _ventasExpandido ? "▼ Ventas" : "▶ Ventas";
                btn_ventas.Text = texto;
                _menuButtonTexts[btn_ventas] = texto;
                _sidebarToolTip.SetToolTip(btn_ventas, _ventasExpandido ? "Colapsar Ventas" : "Expandir Ventas");
            }
            else
            {
                btn_ventas.Text = string.Empty;
                _sidebarToolTip.SetToolTip(btn_ventas, _ventasExpandido ? "Colapsar Ventas" : "Expandir Ventas");
            }
        }

        /// <summary>
        /// Regla de acceso al modulo Vendedores. Vive en un metodo propio y no en el handler
        /// porque es una decision de negocio, no una guarda: define que permisos de Ventas
        /// habilitan este modulo.
        ///
        /// Hoy cualquiera de los tres da acceso, asi que un usuario con permiso de Clientes o de
        /// Pedidos tambien ve la lista completa de vendedores. Si eso no es lo que se quiere,
        /// el cambio es borrar los otros dos terminos de esta sola linea; antes estaba
        /// escrito en el medio del handler, donde era facil pasarlo por alto al revisar.
        /// admin entra siempre.
        ///
        /// El boton se muestra en la barra lateral sin filtrar, igual que los demas modulos: la
        /// autorizacion se aplica al hacer clic, en el mismo patron de VerificarPermiso.
        /// </summary>
        private static bool PuedeVerVendedores()
        {
            return SesionActual.Usuario?.Username == "admin"
                || PermisoHelper.PuedeVer("Vendedores")
                || PermisoHelper.PuedeVer("Clientes")
                || PermisoHelper.PuedeVer("Pedidos");
        }

        private async void Bot_vendedores_Click(object? sender, EventArgs e)
        {
            // Vendedores hereda permiso de Ventas: vale Vendedores, Clientes o Pedidos (admin pasa siempre).
            if (!PuedeVerVendedores())
            {
                MessageBox.Show("No tiene permiso para acceder al módulo Vendedores.",
                    "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            try
            {
                Cursor = Cursors.WaitCursor;
                IDespachoService despachoService = _serviceProvider.GetRequiredService<IDespachoService>();
                ICommonService commonService = _serviceProvider.GetRequiredService<ICommonService>();
                DataSet ds = await despachoService.LoadDataDespachos().ConfigureAwait(true);
                DataTable? dtVendors = ds.Tables.Contains("DtVendors") ? ds.Tables["DtVendors"] : null;
                if (dtVendors == null)
                {
                    MessageBox.Show("No se pudieron cargar los vendedores.", "Vendedores",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                using FrmSeleccion sel = new(commonService)
                {
                    DtItems = dtVendors,
                    Titulo = "Vendedores"
                };
                sel.ShowDialog(this);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al abrir Vendedores: " + ex.Message, "Error",
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                Cursor = Cursors.Default;
            }
        }

        private void CrearBarraUsuario()
        {
            // Panel contenedor de la barra de usuario
            panel_barraUsuario = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = 60,
                BackColor = colorFondoSidebar,
                ForeColor = Color.White,
                Padding = new Padding(8)
            };

            // Avatar / Foto
            PictureBox picAvatar = new PictureBox
            {
                // Imagen por defecto: círculo con iniciales
                BackColor = Color.FromArgb(60, 60, 68),
                SizeMode = PictureBoxSizeMode.Zoom,
                Width = 40,
                Height = 40,
                Location = new Point(12, 8)
            };
            // Dibujar círculo con iniciales
            using (Graphics g = Graphics.FromImage(new Bitmap(40, 40)))
            {
                g.Clear(Color.FromArgb(60, 60, 68));
                g.SmoothingMode = SmoothingMode.AntiAlias;
                // Fondo circular
                using (Brush brush = new SolidBrush(Color.FromArgb(100, 150, 200)))
                {
                    g.FillEllipse(brush, 2, 2, 36, 36);
                }
                // Iniciales
                if (SesionActual.Usuario != null)
                {
                    string iniciales = SesionActual.Usuario.Username.Substring(0, 1).ToUpper();
                    using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
                    using (Brush brush = new SolidBrush(Color.White))
                    {
                        g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                        SizeF size = g.MeasureString(iniciales, font);
                        g.DrawString(iniciales, font, brush, (40 - size.Width) / 2, (40 - size.Height) / 2);
                    }
                }
            }
            picAvatar.SizeMode = PictureBoxSizeMode.Zoom;
            Bitmap avatarBitmap = new Bitmap(40, 40);
            using (Graphics g = Graphics.FromImage(avatarBitmap))
            {
                g.DrawImage(new Bitmap(40, 40), new Rectangle(0, 0, 40, 40));
            }
            picAvatar.Image = avatarBitmap;
            picAvatar.BackColor = Color.Transparent;

            // Contenedor de información del usuario
            Panel panelInfo = new Panel
            {
                Location = new Point(60, 12),
                Size = new Size(200, 36)
            };

            Label lblNombre = new Label
            {
                Text = $"Bienvenido, {SesionActual.Usuario?.NombreCompleto ?? "Usuario"}",
                Font = new Font("Segoe UI", 11f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoSize = false,
                Size = new Size(180, 20),
                Location = new Point(0, 0)
            };

            Label lblTiempo = new Label
            {
                Text = "",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(180, 180, 200),
                AutoSize = true,
                Location = new Point(0, 18)
            };

            panelInfo.Controls.Add(lblNombre);
            panelInfo.Controls.Add(lblTiempo);

            // Botón Cerrar Sesión
            Button btnSalir = new Button
            {
                Text = "Salir",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f),
                FlatAppearance = { BorderSize = 0 },
                Size = new Size(70, 32),
                Location = new Point(280, 16)
            };
            btnSalir.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 40, 50);
            btnSalir.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 30, 40);
            btnSalir.Click += (s, e) => OnSessionExpired();

            // Enlace Cambiar de usuario
            LinkLabel lnkCambiar = new LinkLabel
            {
                Text = "Cambiar de usuario",
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(180, 180, 200),
                Location = new Point(360, 20),
                AutoSize = true
            };
            lnkCambiar.LinkClicked += (s, e) =>
            {
                // Cerrar sesión actual y volver al login
                SesionActual.Clear();
                _sessionManager.Start();
                // Aquí se reabriría el login - por ahora solo mensaje
                MessageBox.Show("Función: Cambiar de usuario (reabrirá login)", "Información",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
            };

            // Agregar controles al panel de barra
            panel_barraUsuario.Controls.Add(picAvatar);
            panel_barraUsuario.Controls.Add(panelInfo);
            panel_barraUsuario.Controls.Add(btnSalir);
            panel_barraUsuario.Controls.Add(lnkCambiar);

            // Actualizar tiempo logueado
            ActualizarTiempoLogueado(lblTiempo);

            Controls.Add(panel_barraUsuario);
        }

        private void ActualizarTiempoLogueado(Label lblTiempo)
        {
            // Actualizar cada tick del timer de sesión
            _sessionTimer.Tick += (s, e) =>
            {
                TimeSpan tiempo = DateTime.Now - _sessionManager.LastActivity;
                lblTiempo.Text = $"Hace {tiempo.Minutes} min";
            };
        }

        private void SessionTimer_Tick(object? sender, EventArgs e)
        {
            if (_sessionManager.IsExpired)
            {
                _sessionTimer.Stop();
                OnSessionExpired();
            }
        }

        private void OnSessionExpired()
        {
            tabContent.TabPages.Clear();
            SesionActual.Clear();

            ISeguridadService seguridadService = _serviceProvider.GetRequiredService<ISeguridadService>();
            using FrmLogin loginForm = new FrmLogin(seguridadService);

            if (loginForm.ShowDialog() == DialogResult.OK)
            {
                SesionActual.Usuario = loginForm.UsuarioAutenticado;
                SesionActual.Permisos = loginForm.Permisos;
                lbl_user_name.Text = $"Usuario : {SesionActual.Usuario?.NombreCompleto}";
                _sessionManager.Start();
                _sessionTimer.Start();
            }
            else
            {
                Application.Exit();
            }
        }

        private void Btn_toggleSidebar_Click(object? sender, EventArgs e)
        {
            if (_isAnimating)
            {
                return;
            }

            _isSidebarExpanded = !_isSidebarExpanded;
            _targetWidth = _isSidebarExpanded ? SIDEBAR_WIDTH_EXPANDED : SIDEBAR_WIDTH_COLLAPSED;
            // Al colapsar se pasa a solo iconos de inmediato y luego se anima el ancho:
            // da la sensacion de respuesta instantanea y evita que el texto se aplaste.
            if (!_isSidebarExpanded)
            {
                AplicarEstadoColapsado();
            }

            _toggleTimer.Start();
        }

        private void ToggleTimer_Tick(object? sender, EventArgs e)
        {
            int step = 20;
            if (panel1.Width < _targetWidth)
            {
                panel1.Width = Math.Min(panel1.Width + step, _targetWidth);
            }
            else if (panel1.Width > _targetWidth)
            {
                panel1.Width = Math.Max(panel1.Width - step, _targetWidth);
            }

            if (panel1.Width == _targetWidth)
            {
                _toggleTimer.Stop();
                _isAnimating = false;
                if (_isSidebarExpanded)
                {
                    AplicarEstadoExpandido();
                }
                else
                {
                    AplicarEstadoColapsado();
                }

                panel1.Invalidate();
                panel1.Update();
                RefrescarSidebar();
            }
            else
            {
                _isAnimating = true;
            }
        }

        private void AplicarEstadoColapsado()
        {
            foreach (Button btn in _menuButtonTexts.Keys)
            {
                btn.Text = "";
                btn.ImageAlign = ContentAlignment.MiddleCenter;
                btn.Padding = new Padding(0);
                _sidebarToolTip.SetToolTip(btn, _menuButtonToolTips[btn]);
            }
            Btn_toggleSidebar.Text = "\u2630";
            Btn_toggleSidebar.ImageAlign = ContentAlignment.MiddleCenter;
            Btn_toggleSidebar.TextAlign = ContentAlignment.MiddleCenter;
            Btn_toggleSidebar.Padding = new Padding(0);
            _sidebarToolTip.SetToolTip(Btn_toggleSidebar, "Expandir menú");
            _sidebarToolTip.SetToolTip(btn_ventas, _ventasExpandido ? "Colapsar Ventas" : "Expandir Ventas");
            panel_DATA.Visible = false;
            if (pnlVentas != null)
            {
                pnlVentas.Visible = _ventasExpandido;
            }
            panel1.Invalidate();
        }

        private void AplicarEstadoExpandido()
        {
            foreach (KeyValuePair<Button, string> kvp in _menuButtonTexts)
            {
                kvp.Key.Text = kvp.Value;
                kvp.Key.ImageAlign = ContentAlignment.MiddleLeft;
                bool esSub = pnlVentas != null && (kvp.Key == bot_pedidos || kvp.Key == button1 || kvp.Key == btn_vendedores);
                kvp.Key.Padding = esSub ? new Padding(28, 0, 0, 0) : new Padding(12, 0, 0, 0);
                _sidebarToolTip.SetToolTip(kvp.Key, null);
            }
            Btn_toggleSidebar.Text = "\u2630";
            Btn_toggleSidebar.ImageAlign = ContentAlignment.MiddleCenter;
            Btn_toggleSidebar.TextAlign = ContentAlignment.MiddleLeft;
            Btn_toggleSidebar.Padding = new Padding(12, 0, 0, 0);
            _sidebarToolTip.SetToolTip(Btn_toggleSidebar, "Colapsar menú");
            _sidebarToolTip.SetToolTip(btn_ventas, _ventasExpandido ? "Colapsar Ventas" : "Expandir Ventas");
            panel_DATA.Visible = true;
            panel_DATA.Left = 0;
            if (pnlVentas != null)
            {
                pnlVentas.Visible = _ventasExpandido;
                pnlVentas.Height = _ventasExpandido ? 3 * VENTAS_SUBBUTTON_HEIGHT : 0;
            }
            panel1.Invalidate();
        }

        private void RefrescarSidebar()
        {
            if (panel1 == null)
            {
                return;
            }

            panel1.Invalidate();
            panel1.Update();
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Invalidate();
                    btn.Update();
                }
                else if (ctrl == pnlVentas && pnlVentas != null)
                {
                    pnlVentas.Invalidate();
                    pnlVentas.Update();
                    foreach (Control sub in pnlVentas.Controls)
                    {
                        if (sub is Button subBtn)
                        {
                            subBtn.Invalidate();
                            subBtn.Update();
                        }
                    }
                }
            }
            panel_DATA.Invalidate();
            panel_DATA.Update();
        }

        // Al volver a una pestana, fuerza el layout del formulario embebido para que
        // no quede "montado" / superpuesto ni mal redimensionado, y reaplica el tema
        // oscuro por si el UIStyleManager de SunnyUI lo re-estilizo al repintar.
        private void RefrescarFormSeleccionado()
        {
            if (tabContent.SelectedTab == null)
            {
                return;
            }

            foreach (Control ctrl in tabContent.SelectedTab.Controls)
            {
                if (ctrl is Form f && !f.IsDisposed)
                {
                    if (f is IFormTemaClaro temaClaro)
                    {
                        temaClaro.ReaplicarTema();
                    }
                    else
                    {
                        TemaOscuroHelper.Aplicar(f);
                    }

                    f.PerformLayout();
                    f.Refresh();
                    f.BringToFront();
                }
            }
        }
    }
}
