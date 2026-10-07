using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
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

        private Button btn_compras = null!;
        private Panel pnlCompras = null!;
        private Button btn_ordenesCompra = null!;
        private bool _comprasExpandido = false;
        private const int COMPRAS_SUBBUTTON_HEIGHT = 50;
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
        private Panel? _userFilaSuperior;
        private Panel? _userInfoPanel;
        private PictureBox? _picAvatarUsuario;
        private Label? _lblNombreUsuario;
        private Label? _lblLoginUsuario;
        private Label? _lblCorreoUsuario;
        private Label? _lblTiempoUsuario;
        private Button? _btnSalirUsuario;
        private LinkLabel? _lnkCambiarUsuario;
        private const int USERBAR_HEIGHT_EXPANDED = 138;
        private const int USERBAR_HEIGHT_COLLAPSED = 62;

        /// <summary>
        /// Alto de la fila de arriba de la barra: nombre + login + correo + tiempo,
        /// que son 64 px, mas 2 px del padding de _userInfoPanel. Si se anyade un
        /// renglon hay que subir este numero y USERBAR_HEIGHT_EXPANDED.
        /// </summary>
        private const int USERBAR_FILA_HEIGHT = 66;

        /// <summary>Lado del avatar. El bitmap de CrearAvatarConIniciales usa el mismo.</summary>
        private const int USERBAR_AVATAR_SIZE = 40;

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
            CrearGrupoCompras();
            ActualizarComprasHeader();
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
            lbl_user_name.Text = $"Usuario : {UsuarioHelper.NombreMostrar(SesionActual.Usuario)}";
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

        private void Bot_proveedores_Click(object? sender, EventArgs e)
        {
            _formManager.ShowForm<FrmProveedores>();
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

        // Módulo Usuarios: sin guarda de permisos, mismo criterio que Vendedores. La
        // tabla permisos no da Usuarios:Ver a nadie que no sea admin, asi que la
        // comprobacion cerraba el módulo con el aviso "Acceso denegado" al resto. El
        // acceso lo da el inicio de sesión.
        private void Bot_usuarios_Click(object sender, EventArgs e)
        {
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
        // bot_pedidos (Pedidos) dentro del sub-panel y crea Vendedores, que ahora abre
        // el módulo FrmVendedores (antes usaba el selector FrmSeleccion + Frm_AddNew).
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
            btn_ventas.Image = Properties.Resources.paid_bill_48px;

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

            // Reordena el sidebar: el grupo Ventas queda en 3ra posicion
            // (tras hamburguesa y Orden Corte). CrearGrupoCompras reordena de
            // nuevo al final y coloca Compras en 4ta.
            Control[] nuevoOrden =
            {
                Btn_toggleSidebar, bot_ordencorte,
                btn_ventas, pnlVentas,
                bot_inventario, bot_despacho, bot_recepciones, bot_products,
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

        // Módulo Vendedores: se reemplazó el selector (LoadDataDespachos + FrmSeleccion,
        // que solo mostraba el listado en un diálogo) por el formulario propio FrmVendedores,
        // con buscador, resumen y detalle, siguiendo el patrón de Clientes/Proveedores.
        //
        // Sin guarda de permisos: Vendedores no tiene permisos propios (la tabla permisos no
        // incluye el módulo), así que la comprobación anterior pedía Clientes o Pedidos y
        // cerraba el módulo a todo el que no fuera admin. El acceso lo da el inicio de sesión.
        private void Bot_vendedores_Click(object? sender, EventArgs e)
        {
            _formManager.ShowForm<FrmVendedores>();
        }

        /// <summary>
        /// Grupo Compras (acordeón): Compras > Órdenes de Compra, Proveedores.
        /// Se construye 100% en código para respetar el límite del diseñador
        /// (no se toca Main.Designer.cs). Reparenta button3 (Proveedores) y
        /// crea Órdenes de Compra.
        /// </summary>
        private void CrearGrupoCompras()
        {
            Color fondoSubmenu = Color.FromArgb(30, 30, 36);
            Color textoSubmenu = Color.FromArgb(205, 205, 215);
            Color hover = Color.FromArgb(70, 140, 25);

            panel1.SuspendLayout();

            btn_compras = new Button
            {
                Dock = DockStyle.Top,
                Height = 70,
                Text = _comprasExpandido ? "▼ Compras" : "▶ Compras",
                FlatStyle = FlatStyle.Flat,
                BackColor = colorFondoSidebar,
                ForeColor = textoSubmenu,
                Font = ObtenerFuenteSidebar(12f),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(12, 0, 0, 0)
            };
            btn_compras.FlatAppearance.BorderSize = 0;
            btn_compras.FlatAppearance.MouseOverBackColor = hover;
            btn_compras.FlatAppearance.MouseDownBackColor = hover;
            btn_compras.MouseEnter += (s, e) => btn_compras.ForeColor = Color.White;
            btn_compras.MouseLeave += (s, e) => btn_compras.ForeColor = textoSubmenu;
            btn_compras.Click += Btn_compras_Click;
            btn_compras.Image = Properties.Resources.procurement_48px;

            pnlCompras = new Panel
            {
                Dock = DockStyle.Top,
                Height = _comprasExpandido ? 2 * COMPRAS_SUBBUTTON_HEIGHT : 0,
                BackColor = fondoSubmenu,
                Padding = new Padding(0),
                Margin = new Padding(0),
                Visible = _comprasExpandido
            };

            // Saca Proveedores del nivel raíz para meterlo al sub-panel.
            panel1.Controls.Remove(button3);

            ConfigurarSubBotonCompras(button3, "Proveedores", fondoSubmenu, textoSubmenu, hover);
            button3.Click -= Bot_proveedores_Click;
            button3.Click += Bot_proveedores_Click;

            btn_ordenesCompra = new Button
            {
                Dock = DockStyle.Top,
                Height = COMPRAS_SUBBUTTON_HEIGHT,
                Text = "Órdenes de Compra",
                FlatStyle = FlatStyle.Flat,
                BackColor = fondoSubmenu,
                ForeColor = textoSubmenu,
                Font = ObtenerFuenteSidebar(11f),
                TextImageRelation = TextImageRelation.ImageBeforeText,
                ImageAlign = ContentAlignment.MiddleLeft,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(28, 0, 0, 0)
            };
            btn_ordenesCompra.FlatAppearance.BorderSize = 0;
            btn_ordenesCompra.FlatAppearance.MouseOverBackColor = hover;
            btn_ordenesCompra.FlatAppearance.MouseDownBackColor = hover;
            btn_ordenesCompra.MouseEnter += (s, e) => btn_ordenesCompra.ForeColor = Color.White;
            btn_ordenesCompra.MouseLeave += (s, e) => btn_ordenesCompra.ForeColor = textoSubmenu;
            btn_ordenesCompra.Click += Bot_ordenesCompra_Click;
            btn_ordenesCompra.Image = Properties.Resources.add_file_32px;

            // Orden dentro del sub-panel (arriba -> abajo): Órdenes de Compra, Proveedores.
            // Con Dock=Top el índice MÁS ALTO queda arriba.
            pnlCompras.Controls.Add(btn_ordenesCompra);
            pnlCompras.Controls.Add(button3);
            pnlCompras.Controls.SetChildIndex(button3, 1);
            pnlCompras.Controls.SetChildIndex(btn_ordenesCompra, 0);

            panel1.Controls.Add(btn_compras);
            panel1.Controls.Add(pnlCompras);

            // Actualiza textos/tooltips
            _menuButtonTexts[button3] = "Proveedores";
            _menuButtonToolTips[button3] = "COMPRAS - PROVEEDORES";
            _menuButtonTexts[btn_compras] = "▼ Compras";
            _menuButtonToolTips[btn_compras] = "COMPRAS";
            _menuButtonTexts[btn_ordenesCompra] = "Órdenes de Compra";
            _menuButtonToolTips[btn_ordenesCompra] = "COMPRAS - ÓRDENES DE COMPRA";

            if (!_isSidebarExpanded)
            {
                button3.Text = string.Empty;
                btn_ordenesCompra.Text = string.Empty;
                btn_compras.Text = string.Empty;
                _sidebarToolTip.SetToolTip(button3, "COMPRAS - PROVEEDORES");
                _sidebarToolTip.SetToolTip(btn_ordenesCompra, "COMPRAS - ÓRDENES DE COMPRA");
                _sidebarToolTip.SetToolTip(btn_compras, "Expandir Compras");
            }
            else
            {
                button3.Text = "Proveedores";
                btn_ordenesCompra.Text = "Órdenes de Compra";
                btn_compras.Text = "▼ Compras";
            }

            // Reordena el sidebar: Ventas completo en 3ra posicion y Compras
            // completo en 4ta (tras hamburguesa y Orden Corte), con el resto debajo.
            Control[] nuevoOrden =
            {
                Btn_toggleSidebar, bot_ordencorte,
                btn_ventas, pnlVentas,
                btn_compras, pnlCompras,
                bot_inventario, bot_despacho, bot_recepciones, bot_products,
                button2, button4, OPC_MENU_LABELS
            };

            foreach (Control c in nuevoOrden)
            {
                if (panel1.Controls.Contains(c))
                {
                    panel1.Controls.SetChildIndex(c, 0);
                }
            }

            if (panel1.Controls.Contains(panel_DATA))
            {
                panel1.Controls.SetChildIndex(panel_DATA, 0);
            }

            panel1.ResumeLayout(true);
            panel1.PerformLayout();
            RefrescarSidebar();
        }

        private void ConfigurarSubBotonCompras(Button btn, string texto, Color fondo, Color fore, Color hover)
        {
            btn.Dock = DockStyle.Top;
            btn.Height = COMPRAS_SUBBUTTON_HEIGHT;
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

        private void Btn_compras_Click(object? sender, EventArgs e)
        {
            _comprasExpandido = !_comprasExpandido;
            pnlCompras.Visible = _comprasExpandido;
            pnlCompras.Height = _comprasExpandido ? 2 * COMPRAS_SUBBUTTON_HEIGHT : 0;
            ActualizarComprasHeader();
            panel1.PerformLayout();
            RefrescarSidebar();
            _sessionManager.ResetActivity();
        }

        private void ActualizarComprasHeader()
        {
            if (_isSidebarExpanded)
            {
                string texto = _comprasExpandido ? "▼ Compras" : "▶ Compras";
                btn_compras.Text = texto;
                _menuButtonTexts[btn_compras] = texto;
                _sidebarToolTip.SetToolTip(btn_compras, _comprasExpandido ? "Colapsar Compras" : "Expandir Compras");
            }
            else
            {
                btn_compras.Text = string.Empty;
                _sidebarToolTip.SetToolTip(btn_compras, _comprasExpandido ? "Colapsar Compras" : "Expandir Compras");
            }
        }

        private static bool PuedeVerOrdenesCompra()
        {
            return SesionActual.Usuario?.Username == "admin"
                || PermisoHelper.PuedeVer("OrdenesCompra")
                || PermisoHelper.PuedeVer("Proveedores");
        }

        private void Bot_ordenesCompra_Click(object? sender, EventArgs e)
        {
            if (!PuedeVerOrdenesCompra())
            {
                MessageBox.Show("No tiene permiso para acceder al módulo Órdenes de Compra.",
                    "Acceso denegado", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            _formManager.ShowForm<FrmOrdenesCompra>();
        }

        private void CrearBarraUsuario()
        {
            // Vive DENTRO del sidebar (panel1), anclada abajo. Antes se agregaba a
            // Controls (el Form) con Dock=Bottom y ocupaba todo el ancho inferior.
            panel_barraUsuario = new Panel
            {
                Dock = DockStyle.Bottom,
                Height = USERBAR_HEIGHT_EXPANDED,
                BackColor = colorFondoSidebar,
                ForeColor = Color.White,
                Padding = new Padding(8, 8, 8, 8)
            };

            // Fila superior: avatar + info (nombre / login / correo / tiempo).
            // Altura justa para los cuatro renglones de _userInfoPanel.
            _userFilaSuperior = new Panel
            {
                Dock = DockStyle.Top,
                Height = USERBAR_FILA_HEIGHT,
                BackColor = Color.Transparent,
                Padding = new Padding(0),
                Margin = new Padding(0)
            };

            _picAvatarUsuario = new PictureBox
            {
                Dock = DockStyle.Left,
                Width = USERBAR_AVATAR_SIZE,
                BackColor = Color.Transparent,
                SizeMode = PictureBoxSizeMode.Zoom,
                Margin = new Padding(0)
            };
            _picAvatarUsuario.Image = CrearAvatarConIniciales();
            _userFilaSuperior.Controls.Add(_picAvatarUsuario);

            _userInfoPanel = new Panel
            {
                Dock = DockStyle.Fill,
                BackColor = Color.Transparent,
                Padding = new Padding(6, 2, 0, 0),
                Margin = new Padding(0)
            };

            _lblNombreUsuario = new Label
            {
                Dock = DockStyle.Top,
                Height = 20,
                Font = new Font("Segoe UI", 9f, FontStyle.Bold),
                ForeColor = Color.White,
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Login (@usuario). Verde claro para que se distinga del nombre.
            // MiddleCenter, igual que el nombre: con MiddleLeft el renglon quedaba
            // pegado al borde izquierdo del panel y se veia montado sobre el avatar.
            _lblLoginUsuario = new Label
            {
                Dock = DockStyle.Top,
                Height = 14,
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(170, 215, 130),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // Correo. Se oculta en el refresco si el usuario no tiene, para que no
            // quede un renglon en blanco metido en el medio.
            _lblCorreoUsuario = new Label
            {
                Dock = DockStyle.Top,
                Height = 14,
                Font = new Font("Segoe UI", 7.5f),
                ForeColor = Color.FromArgb(180, 180, 200),
                AutoEllipsis = true,
                TextAlign = ContentAlignment.MiddleCenter
            };

            _lblTiempoUsuario = new Label
            {
                Dock = DockStyle.Top,
                Height = 16,
                Font = new Font("Segoe UI", 8f),
                ForeColor = Color.FromArgb(180, 180, 200),
                AutoSize = false,
                TextAlign = ContentAlignment.MiddleCenter
            };

            // El orden de Add es el INVERSO al visual: WinForms ancla arriba a lo
            // ultimo que se anade, asi que va tiempo, correo, login y nombre.
            _userInfoPanel.Controls.Add(_lblTiempoUsuario);
            _userInfoPanel.Controls.Add(_lblCorreoUsuario);
            _userInfoPanel.Controls.Add(_lblLoginUsuario);
            _userInfoPanel.Controls.Add(_lblNombreUsuario);
            _userFilaSuperior.Controls.Add(_userInfoPanel);

            // Fila inferior: acciones. Dock Bottom para que queden al pie del sidebar.
            _btnSalirUsuario = new Button
            {
                Dock = DockStyle.Bottom,
                Height = 32,
                Text = "Salir",
                FlatStyle = FlatStyle.Flat,
                BackColor = Color.FromArgb(220, 53, 69),
                ForeColor = Color.White,
                Font = new Font("Segoe UI", 9f)
            };
            _btnSalirUsuario.FlatAppearance.BorderSize = 0;
            _btnSalirUsuario.FlatAppearance.MouseOverBackColor = Color.FromArgb(200, 40, 50);
            _btnSalirUsuario.FlatAppearance.MouseDownBackColor = Color.FromArgb(180, 30, 40);
            _btnSalirUsuario.Click += (s, e) => OnSessionExpired();

            _lnkCambiarUsuario = new LinkLabel
            {
                Dock = DockStyle.Bottom,
                Height = 20,
                Text = "Cambiar de usuario",
                Font = new Font("Segoe UI", 8f),
                TextAlign = ContentAlignment.MiddleCenter,
                LinkColor = Color.FromArgb(180, 180, 200),
                ActiveLinkColor = Color.White,
                BackColor = Color.Transparent
            };
            _lnkCambiarUsuario.LinkClicked += (s, e) => CambiarDeUsuario();

            panel_barraUsuario.Controls.Add(_lnkCambiarUsuario);
            panel_barraUsuario.Controls.Add(_btnSalirUsuario);
            panel_barraUsuario.Controls.Add(_userFilaSuperior);

            RefrescarDatosUsuario();

            if (_lblTiempoUsuario != null)
            {
                ActualizarTiempoLogueado(_lblTiempoUsuario);
            }

            // Padre = sidebar, no el Form. Con Dock=Bottom queda al final abajo
            // dentro del contenedor del sidebar y respeta su ancho (210 / 55).
            panel1.Controls.Add(panel_barraUsuario);
            panel_barraUsuario.BringToFront();

            AplicarEstadoBarraUsuario();
        }

        private Bitmap CrearAvatarConIniciales()
        {
            Bitmap bmp = new Bitmap(USERBAR_AVATAR_SIZE, USERBAR_AVATAR_SIZE);
            using (Graphics g = Graphics.FromImage(bmp))
            {
                g.Clear(colorFondoSidebar);
                g.SmoothingMode = SmoothingMode.AntiAlias;
                using (Brush fondo = new SolidBrush(Color.FromArgb(100, 150, 200)))
                {
                    g.FillEllipse(fondo, 2, 2, USERBAR_AVATAR_SIZE - 4, USERBAR_AVATAR_SIZE - 4);
                }

                // Nombre completo primero (es lo que muestra el label de al lado);
                // si esta vacio cae al usuario, y si no hay sesion, en "?".
                string inicial = UsuarioHelper.Inicial(SesionActual.Usuario);

                using (Font font = new Font("Segoe UI", 14, FontStyle.Bold))
                using (Brush brush = new SolidBrush(Color.White))
                {
                    g.TextRenderingHint = System.Drawing.Text.TextRenderingHint.AntiAlias;
                    SizeF size = g.MeasureString(inicial, font);
                    g.DrawString(
                        inicial, font, brush,
                        (USERBAR_AVATAR_SIZE - size.Width) / 2,
                        (USERBAR_AVATAR_SIZE - size.Height) / 2 - 1);
                }
            }

            return bmp;
        }

        /// <summary>
        /// Vuelve a pintar la barra de usuario del sidebar con los datos de la sesión
        /// actual. Se llama en cada arranque y en cada login (al reabrir el login por
        /// timeout, por el boton Salir o por "Cambiar de usuario"), que es cuando
        /// pueden cambiar los tres datos que se muestran: nombre, login y correo.
        /// </summary>
        private void RefrescarDatosUsuario()
        {
            Usuario? usuario = SesionActual.Usuario;

            // Nombre completo si tiene; si no, el usuario; nunca cadena vacia.
            string nombre = UsuarioHelper.NombreMostrar(usuario);
            if (_lblNombreUsuario != null)
            {
                _lblNombreUsuario.Text = nombre;
            }

            // Login. Se oculta cuando se repetiria: si el nombre completo viene vacio,
            // el renglon de arriba ya muestra el usuario y no pinta decirlo dos veces.
            string login = UsuarioHelper.Login(usuario);
            if (_lblLoginUsuario != null)
            {
                bool repetido = string.Equals(login, nombre, StringComparison.OrdinalIgnoreCase);
                _lblLoginUsuario.Text = login.Length > 0 ? "@" + login : string.Empty;
                _lblLoginUsuario.Visible = login.Length > 0 && !repetido;
            }

            // Correo. Sin dato no se pinta el renglon: con Dock no se reserva el sitio
            // de un control oculto, asi que no queda un hueco en blanco.
            string correo = UsuarioHelper.Correo(usuario);
            if (_lblCorreoUsuario != null)
            {
                _lblCorreoUsuario.Text = correo;
                _lblCorreoUsuario.Visible = correo.Length > 0;
            }

            if (_picAvatarUsuario != null)
            {
                _picAvatarUsuario.Image?.Dispose();
                _picAvatarUsuario.Image = CrearAvatarConIniciales();

                // Con el sidebar colapsado solo se ve el avatar: el tooltip lleva el
                // login y el correo ademas del nombre para identificar la cuenta.
                _sidebarToolTip.SetToolTip(_picAvatarUsuario, UsuarioHelper.Tooltip(usuario));
            }
        }

        private void AplicarEstadoBarraUsuario()
        {
            if (panel_barraUsuario == null)
            {
                return;
            }

            bool expandido = _isSidebarExpanded;
            panel_barraUsuario.Height = expandido ? USERBAR_HEIGHT_EXPANDED : USERBAR_HEIGHT_COLLAPSED;

            if (_userInfoPanel != null)
            {
                _userInfoPanel.Visible = expandido;
            }

            if (_btnSalirUsuario != null)
            {
                _btnSalirUsuario.Visible = expandido;
            }

            if (_lnkCambiarUsuario != null)
            {
                _lnkCambiarUsuario.Visible = expandido;
            }

            if (_userFilaSuperior != null)
            {
                _userFilaSuperior.Dock = expandido ? DockStyle.Top : DockStyle.Fill;

                // Dock=Fill (lo que pasa al colapsar) reescribe el Bounds del control y
                // le deja los 46 px que hay en el sidebar cerrado. Al volver a Top el
                // layout usa ese alto en vez del original, y los cuatro renglones salian
                // cortados y montados entre si. El alto se devuelve a mano.
                if (expandido)
                {
                    _userFilaSuperior.Height = USERBAR_FILA_HEIGHT;
                }
            }

            if (_picAvatarUsuario != null)
            {
                _picAvatarUsuario.Dock = expandido ? DockStyle.Left : DockStyle.Fill;
                _picAvatarUsuario.SizeMode = PictureBoxSizeMode.CenterImage;

                // El ancho se pierde por el mismo motivo que el alto de la fila.
                if (expandido)
                {
                    _picAvatarUsuario.Width = USERBAR_AVATAR_SIZE;
                }
            }

            panel_barraUsuario.Invalidate();
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

            // Mismo filtro que en el arranque: sin un usuario activo no se monta la
            // sesion nueva, y aqui se sale de la aplicacion igual que si se cancela.
            if (loginForm.ShowDialog() == DialogResult.OK &&
                loginForm.UsuarioAutenticado is { Activo: true })
            {
                SesionActual.Usuario = loginForm.UsuarioAutenticado;
                SesionActual.Permisos = loginForm.Permisos;
                lbl_user_name.Text = $"Usuario : {UsuarioHelper.NombreMostrar(SesionActual.Usuario)}";
                RefrescarDatosUsuario();
                _sessionManager.Start();
                _sessionTimer.Start();
            }
            else
            {
                Application.Exit();
            }
        }

        /// <summary>
        /// Enlace "Cambiar de usuario" de la barra: reabre el login y, al aceptar,
        /// vuelve a cargar usuario y permisos y repinta la barra lateral. Es el otro
        /// punto de la app donde se loguea, y el unico que puede cambiar de cuenta
        /// sin reiniciar. Si se cancela se conserva la sesion que habia.
        /// </summary>
        private void CambiarDeUsuario()
        {
            // Se para el reloj mientras el login esta en medio: si el dialogo se tiene
            // abierto mas de lo que dura la sesion, SessionTimer_Tick dispararia
            // OnSessionExpired() a media altura, que limpia la sesion entera. Se vuelve
            // a arrancar en TODA salida, tambien al cancelar: sin el, la sesion dejaria
            // de caducar.
            _sessionTimer.Stop();

            ISeguridadService seguridadService = _serviceProvider.GetRequiredService<ISeguridadService>();
            using FrmLogin loginForm = new FrmLogin(seguridadService);

            if (loginForm.ShowDialog() != DialogResult.OK ||
                loginForm.UsuarioAutenticado is not { Activo: true })
            {
                _sessionManager.Start();
                _sessionTimer.Start();
                return;
            }

            Usuario? usuarioAnterior = SesionActual.Usuario;
            List<string> permisosAnteriores = [.. SesionActual.Permisos];

            SesionActual.Usuario = loginForm.UsuarioAutenticado;
            SesionActual.Permisos = loginForm.Permisos;

            // Primer login: misma regla que en el arranque, no se cambia de cuenta
            // sin pasar por el cambio de la clave temporal. El dialogo lee el usuario
            // de SesionActual, por eso se pone antes de abrirlo, y si se cancela se
            // devuelve la sesion anterior.
            if (SesionActual.Usuario.PrimerLogin)
            {
                using FrmCambiarContrasena frmCambiar = _serviceProvider.GetRequiredService<FrmCambiarContrasena>();
                if (frmCambiar.ShowDialog() != DialogResult.OK)
                {
                    SesionActual.Usuario = usuarioAnterior;
                    SesionActual.Permisos = permisosAnteriores;
                    _sessionManager.Start();
                    _sessionTimer.Start();
                    return;
                }
            }

            // Las pestañas abiertas son del usuario anterior, y los formularios ya
            // abiertos no vuelven a comprobar permisos al re-entrar en ellos: un
            // usuario sin Pedidos podria seguir trabajando en la pestaña que dejo
            // abierta el anterior. Igual que al expirar la sesión.
            tabContent.TabPages.Clear();

            RefrescarDatosUsuario();
            _sessionManager.Start();
            _sessionTimer.Start();
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
            if (panel_DATA != null && !panel_DATA.IsDisposed)
            {
                panel_DATA.Visible = false;
            }
            if (pnlVentas != null)
            {
                pnlVentas.Visible = _ventasExpandido;
            }
            AplicarEstadoBarraUsuario();
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
            if (panel_DATA != null && !panel_DATA.IsDisposed)
            {
                panel_DATA.Visible = true;
                panel_DATA.Left = 0;
            }
            if (pnlVentas != null)
            {
                pnlVentas.Visible = _ventasExpandido;
                pnlVentas.Height = _ventasExpandido ? 3 * VENTAS_SUBBUTTON_HEIGHT : 0;
            }
            AplicarEstadoBarraUsuario();
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
                else if (ctrl == pnlCompras && pnlCompras != null)
                {
                    pnlCompras.Invalidate();
                    pnlCompras.Update();
                }
                else if (ctrl == panel_barraUsuario && panel_barraUsuario != null)
                {
                    panel_barraUsuario.Invalidate();
                    panel_barraUsuario.Update();
                }
            }
            if (panel_DATA != null && !panel_DATA.IsDisposed && panel_DATA.Parent != null)
            {
                panel_DATA.Invalidate();
                panel_DATA.Update();
            }
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
