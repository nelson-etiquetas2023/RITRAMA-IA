using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Sunny.UI;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Text;
using System.Globalization;
using System.Runtime.InteropServices;

namespace Ritrama2025
{
    public partial class Main : UIForm
    {
        private IConfiguration Config { get; set; } = null!;
        private readonly FormManager _formManager;
        private string MODE = "";
        private PrivateFontCollection _pfc = new();

        // Colores del tema del sidebar.
        private static readonly Color colorFondoSidebar = Color.FromArgb(38, 38, 44);

        public Main(FormManager formManager, IConfiguration config)
        {
            InitializeComponent();
            CargarFuenteBebasNeue();
            AplicarTemaSidebarOscuro();
            _formManager = formManager;
            Config = config;
            _formManager.HostTabControl = tabContent;
            AplicarEsquinasRedondeadas();
            tabContent.Resize += (s, e) => AplicarEsquinasRedondeadas();
            AplicarTemaOscuroTabContent();
            ConfigurarCierrePestanas();

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
            var menu = new ContextMenuStrip();
            var itemCerrar = new ToolStripMenuItem("Cerrar pestaña");
            itemCerrar.Click += (s, e) =>
            {
                if (tabContent.SelectedTab != null)
                {
                    var form = tabContent.SelectedTab.Controls.OfType<Form>().FirstOrDefault();
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
                    if (index >= 0) tabContent.SelectedIndex = index;
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
                return new Font(_pfc.Families[0], size, FontStyle.Bold);
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
                    btn.Font = ObtenerFuenteSidebar(16f);
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
            if (tabContent == null) return;
            Rectangle rect = tabContent.ClientRectangle;
            if (rect.Width <= 0 || rect.Height <= 0) return;
            int radio = 16;
            var gp = new GraphicsPath();
            gp.StartFigure();
            gp.AddArc(0, 0, radio, radio, 180, 90);
            gp.AddArc(rect.Width - radio, 0, radio, radio, 270, 90);
            gp.AddArc(rect.Width - radio, rect.Height - radio, radio, radio, 0, 90);
            gp.AddArc(0, rect.Height - radio, radio, radio, 90, 90);
            gp.CloseFigure();
            tabContent.Region = new Region(gp);
        }

        // Aplica fondo oscuro al area de pestanas central y a las pestanas que se creen.
        private void AplicarTemaOscuroTabContent()
        {
            if (tabContent == null) return;
            tabContent.BackColor = TemaOscuroHelper.Fondo;
            tabContent.TabUnSelectedForeColor = TemaOscuroHelper.Texto;
            tabContent.GotFocus += (s, e) => tabContent.BackColor = TemaOscuroHelper.Fondo;
        }

        private void Main_Load(object sender, EventArgs e)
        {
            if (Config != null)
            {
                MODE = Config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            }
            panel2.BackColor = colorFondoSidebar;
            LAB_MODE_RUN.Text = MODE;

        }
        private void Bot_despacho_Click(object sender, EventArgs e)
        {
            _formManager.ShowForm<FrmDespacho>();
        }

        private void Bot_ordencorte_Click(object sender, EventArgs e)
        {
            _formManager.ShowForm<FrmOrdenCorte>();
        }

        private void Bot_recepciones_Click(object sender, EventArgs e)
        {
            _formManager.ShowForm<FrmMateriaPrima>();
        }

        private void OPC_MENU_LABELS_Click(object sender, EventArgs e)
        {
            _formManager.ShowForm<FrmCodeBarLabel>();
        }

        private void Bot_products_Click(object sender, EventArgs e)
        {
            _formManager.ShowForm<FrmProductos>();
        }

        private void Bot_inventario_Click(object sender, EventArgs e)
        {
            _formManager.ShowForm<Frm_Inventarios>();
        }

        // Repintar el sidebar para que nunca quede en blanco durante la carga de
        // modulos embebidos en la pestana central.
        private void RefrescarSidebar()
        {
            if (panel1 == null) return;
            panel1.Invalidate();
            panel1.Update();
            foreach (Control ctrl in panel1.Controls)
            {
                if (ctrl is Button btn)
                {
                    btn.Invalidate();
                    btn.Update();
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
            if (tabContent.SelectedTab == null) return;
            foreach (Control ctrl in tabContent.SelectedTab.Controls)
            {
                    if (ctrl is Form f && !f.IsDisposed)
                    {
                        if (f is IFormTemaClaro temaClaro) temaClaro.ReaplicarTema();
                        else TemaOscuroHelper.Aplicar(f);
                        f.PerformLayout();
                        f.Refresh();
                        f.BringToFront();
                    }
            }
        }
    }
}