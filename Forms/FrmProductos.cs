using System.Data;
using System.Globalization;
using System.Linq;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Forms.Controls;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProductsService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Productos: captura de productos con franja verde de datos,
    /// zona de detalle de items (grid), guardado en lote (INSERT/UPDATE) y anulación.
    /// Panel izquierdo: catálogo con buscador, filtro categoría, FlowLayout de ProductCard y summary.
    /// </summary>
    public partial class FrmProductos : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IProductsService _productsService;
        private readonly IConfiguration _configuration;
        private readonly string _conexion;

        private DataTable _dtProductos = new();
        private readonly DataTable _dtDetalle = new();
        private ProductCard? _cardSeleccionada;
        private bool _catalogEventsWired;
        private bool _toolbarEventsWired;
        private bool _nuevoProductoEventsWired;
        private System.Windows.Forms.ErrorProvider? _errorProviderNuevo;
        private static readonly string[] _categoriasFiltro = ["Todos", "Master", "Rollo Cortado", "Resma", "Graphics"];
        private bool _modoEdicion;
        private string? _idEnEdicion;
        //private bool _capturando;
        //private bool _editandoNuevo;
        //private readonly bool _esperandoPrimerAdd;

        private const string SQL_INSERT_PRODUCTO =
            "INSERT INTO producto (Product_ID,Product_Name,Category_ID,unidad,width,lenght,cantidad,msi,precio,MasterRolls,rollo_cortado,Resmas,Graphics,anulado) " +
            "VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,0)";

        private const string SQL_UPDATE_PRODUCTO =
            "UPDATE producto SET Product_Name=@p2,Category_ID=@p3,unidad=@p4,width=@p5,lenght=@p6,cantidad=@p7,msi=@p8,precio=@p9,MasterRolls=@p10,rollo_cortado=@p11,Resmas=@p12,Graphics=@p13 WHERE Product_ID=@p1";

        private const string SQL_ANULAR_PRODUCTO =
            "UPDATE producto SET anulado = 1 WHERE Product_ID = @p1";

        /// <summary>
        /// Crea el formulario de productos con sus dependencias.
        /// </summary>
        /// <param name="productsService">Servicio de productos (catálogo).</param>
        /// <param name="configuration">Configuración de la aplicación (cadena de conexión).</param>
        public FrmProductos(IProductsService productsService, IConfiguration configuration)
        {
            InitializeComponent();
            _productsService = productsService ?? throw new ArgumentNullException(nameof(productsService));
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            string ambiente = _configuration["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            _conexion = _configuration.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]
                        ?? throw new InvalidOperationException("No se pudo resolver la cadena de conexión.");

            //this.Text = "Productos";

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
        public void ReaplicarTema()
        {
            AplicarTemaVerde();
            AplicarTemaCatalogo();
        }

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            BackColor = Color.White;
            try { Style = UIStyle.Green; } catch { }
            TitleColor = verde;
            try { TitleForeColor = Color.White; } catch { }
        }

        private void AplicarTemaCatalogo()
        {
            try
            {
                // Si existe panelIzq (Designer moderno), asegura colores coherentes verde/blanco.
                // Fallback defensivo: si el Designer aún no lo tiene, no hace nada.
                Panel? panelIzqCtrl = Controls.Find("panelIzq", true).FirstOrDefault() as Panel;
                // También soporta campo directo si existe (Designer genera campo panelIzq)
                Panel? panel = null;
                try { panel = panelIzq; } catch { panel = panelIzqCtrl; }
                panel ??= panelIzqCtrl;
                if (panel != null)
                {
                    panel.BackColor = Color.White;
                    Panel? buscador = panel.Controls.Find("pnlBuscador", false).FirstOrDefault() as Panel;
                    if (buscador != null)
                    {
                        buscador.BackColor = Color.White;
                    }

                    Panel? summary = panel.Controls.Find("pnlSummary", false).FirstOrDefault() as Panel;
                    if (summary != null)
                    {
                        summary.BackColor = Color.FromArgb(110, 190, 40);
                    }

                    FlowLayoutPanel? flp = panel.Controls.Find("flpProductos", false).FirstOrDefault() as FlowLayoutPanel;
                    if (flp != null)
                    {
                        flp.BackColor = Color.FromArgb(245, 245, 245);
                    }
                }
            }
            catch { /* no crítico */ }

            // Tematiza la ToolStrip superior (barraHerramientas) con verde corporativo.
            // Debe reaplicarse después de LightGreenTheme para no perder Font 12 Bold.
            try
            {
                ToolStrip? barra = null;
                try { barra = barraHerramientas; } catch { barra = null; }
                barra ??= Controls.Find("barraHerramientas", true).FirstOrDefault() as ToolStrip;
                if (barra != null)
                {
                    barra.BackColor = Color.FromArgb(110, 190, 40);
                    barra.ForeColor = Color.White;
                    barra.Font = new Font("JetBrains Mono", 12F, FontStyle.Bold, GraphicsUnit.Point);
                    barra.GripStyle = ToolStripGripStyle.Hidden;
                    barra.RenderMode = ToolStripRenderMode.Professional;
                    foreach (ToolStripItem item in barra.Items)
                    {
                        item.ForeColor = Color.White;
                        if (item is ToolStripButton btn)
                        {
                            btn.ForeColor = Color.White;
                            btn.Font = new Font("JetBrains Mono", 12F, FontStyle.Bold, GraphicsUnit.Point);
                            btn.BackColor = Color.Transparent;
                        }
                    }
                }
            }
            catch { /* no crítico */ }

            // Reaplica tema de btnGuardarNuevo para pisar cambios de LightGreenTheme (debe quedar verde 110,190,40)
            try
            {
                Sunny.UI.UIButton? btnG = null;
                try { btnG = btnGuardarNuevo; } catch { btnG = null; }
                btnG ??= Controls.Find("btnGuardarNuevo", true).FirstOrDefault() as Sunny.UI.UIButton;
                if (btnG != null)
                {
                    btnG.FillColor = Color.FromArgb(110, 190, 40);
                    btnG.FillColor2 = Color.FromArgb(110, 190, 40);
                    btnG.RectColor = Color.FromArgb(110, 190, 40);
                    btnG.FillHoverColor = Color.FromArgb(90, 160, 30);
                    btnG.FillPressColor = Color.FromArgb(80, 140, 25);
                    btnG.ForeColor = Color.White;
                    btnG.Style = UIStyle.Green;
                    btnG.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold, GraphicsUnit.Point);
                }
            }
            catch { /* no crítico */ }
        }


        public async Task InitializeAsync()
        {
            try
            {
                await CargarCatalogoAsync();
                // Los controles deben configurarse en el hilo de UI
                if (InvokeRequired)
                {
                    Invoke(() =>
                    {
                        ConfigurarControlesCatalogo();
                        WireCatalogEvents();
                        WireToolbarEvents();
                        WireNuevoProductoEvents();
                        EnsureNuevoProductoTabOculta();
                        FiltrarYRender();
                        AplicarTemaCatalogo();
                        ActualizarEstadoToolbar();
                    });
                }
                else
                {
                    ConfigurarControlesCatalogo();
                    WireCatalogEvents();
                    WireToolbarEvents();
                    WireNuevoProductoEvents();
                    EnsureNuevoProductoTabOculta();
                    FiltrarYRender();
                    AplicarTemaCatalogo();
                    ActualizarEstadoToolbar();
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Productos: " + ex.Message);
            }
            //FinalizarConfiguracionUI();
            //DeshabilitarCaptura();
        }

        private async Task CargarCatalogoAsync()
        {
            try
            {
                DataSet ds = await _productsService.Load();
                DataTable? dt = ds.Tables["Dtproducts"]
                    ?? ds.Tables["DtProducts"]
                    ?? ds.Tables["DtProductos"]
                    ?? (ds.Tables.Count > 0 ? ds.Tables[0] : null);
                _dtProductos = dt ?? new DataTable();
                // Asegura que tabla tenga al menos las columnas esperadas para no romper filtros
                // (no crea columnas, solo evita null)
                if (_dtProductos == null)
                {
                    _dtProductos = new DataTable();
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar productos: " + ex.Message);
                _dtProductos = new DataTable();
            }
        }

        // ---------------------------------------------------------------------
        // Catálogo panel izquierdo: buscador + filtro + cards + summary
        // ---------------------------------------------------------------------

        /// <summary>
        /// Configura controles del catálogo (cbo categorías, flp layout).
        /// Soporta fallback si el Designer aún no los tiene (crea via Find o inicializa).
        /// </summary>
        private void ConfigurarControlesCatalogo()
        {
            // Fallback: si los campos del Designer aún no existen (otro agente no ha mergeado),
            // intenta resolverlos via Controls.Find y si no existen crea el panel izquierdo mínimo.
            // En el flujo normal (Designer ya tiene panelIzq) este fallback no hace nada.
            EnsureCatalogControlsFallback();

            try
            {
                // Activar buscador por ID/Nombre y combo de categoría.
                MostrarFiltrosCatalogo();

                if (flpProductos != null)
                {
                    flpProductos.AutoScroll = true;
                    flpProductos.FlowDirection = FlowDirection.TopDown;
                    flpProductos.WrapContents = false;
                }
            }
            catch { /* defensivo */ }
        }

        private void OcultarFiltrosCatalogo()
        {
            try
            {
                Sunny.UI.UITextBox? txt = null;
                try { txt = txtSearch; } catch { txt = null; }
                txt ??= Controls.Find("txtSearch", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (txt != null)
                {
                    txt.Visible = false;
                    txt.Enabled = false;
                }

                Sunny.UI.UIComboBox? cbo = null;
                try { cbo = cboFiltroCategoria; } catch { cbo = null; }
                cbo ??= Controls.Find("cboFiltroCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;
                if (cbo != null)
                {
                    cbo.Visible = false;
                    cbo.Enabled = false;
                }

                Panel? buscador = null;
                try
                {
                    Panel? panelIzqCtrl = Controls.Find("panelIzq", true).FirstOrDefault() as Panel;
                    Panel? panel = null;
                    try { panel = panelIzq; } catch { panel = panelIzqCtrl; }
                    panel ??= panelIzqCtrl;
                    if (panel != null)
                    {
                        buscador = panel.Controls.Find("pnlBuscador", false).FirstOrDefault() as Panel;
                    }
                }
                catch { buscador = null; }

                buscador ??= Controls.Find("pnlBuscador", true).FirstOrDefault() as Panel;
                if (buscador != null)
                {
                    buscador.Height = 42;
                }
            }
            catch { /* no crítico */ }
        }

        /// <summary>
        /// Muestra y habilita el txtSearch y el cboFiltroCategoria, ajustando la altura del panel buscador
        /// para que ambos controles sean visibles.
        /// </summary>
        private void MostrarFiltrosCatalogo()
        {
            try
            {
                Sunny.UI.UITextBox? txt = null;
                try { txt = txtSearch; } catch { txt = null; }
                txt ??= Controls.Find("txtSearch", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (txt != null)
                {
                    txt.Visible = true;
                    txt.Enabled = true;
                }

                Sunny.UI.UIComboBox? cbo = null;
                try { cbo = cboFiltroCategoria; } catch { cbo = null; }
                cbo ??= Controls.Find("cboFiltroCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;
                if (cbo != null)
                {
                    cbo.Visible = true;
                    cbo.Enabled = true;
                    // Asegurar que "Todos" esté seleccionado por defecto
                    if (cbo.SelectedIndex < 0 && cbo.Items.Count > 0)
                    {
                        cbo.SelectedIndex = 0;
                    }
                }

                // Ampliar panel para que quepan: lblTitulo (20px) + txtSearch (29px) + cboFiltroCategoria (29px) + paddings (10+4+4)
                Panel? buscador = null;
                try
                {
                    Panel? panelIzqCtrl = Controls.Find("panelIzq", true).FirstOrDefault() as Panel;
                    Panel? panel = null;
                    try { panel = panelIzq; } catch { panel = panelIzqCtrl; }
                    panel ??= panelIzqCtrl;
                    if (panel != null)
                    {
                        buscador = panel.Controls.Find("pnlBuscador", false).FirstOrDefault() as Panel;
                    }
                }
                catch { buscador = null; }

                buscador ??= Controls.Find("pnlBuscador", true).FirstOrDefault() as Panel;
                if (buscador != null)
                {
                    buscador.Height = 102;
                }
            }
            catch { /* no crítico */ }
        }

        /// <summary>
        /// Fallback que crea el panel izquierdo mínimo si el Designer aún no lo tiene.
        /// No usa SplitContainer. Solo se ejecuta si panelIzq no se encuentra.
        /// </summary>
        private void EnsureCatalogControlsFallback()
        {
            try
            {
                // Intenta resolver campos directos; si existen, no crea nada.
                bool hasPanel = false;
                try { hasPanel = panelIzq != null; } catch { hasPanel = false; }
                if (hasPanel)
                {
                    return;
                }

                Control? foundPanel = Controls.Find("panelIzq", true).FirstOrDefault();
                if (foundPanel != null)
                {
                    return;
                }

                // Crear panel izquierdo mínimo programático (solo si Designer no lo trajo)
                // Usa controles SunnyUI para coherencia visual (Watermark, RectColor verde, etc.).
                Panel panelIzqNew = new Panel
                {
                    Name = "panelIzq",
                    Dock = DockStyle.Left,
                    Width = 340,
                    BackColor = Color.White
                };
                Panel pnlBuscadorNew = new Panel
                {
                    Name = "pnlBuscador",
                    Dock = DockStyle.Top,
                    Height = 78,
                    BackColor = Color.White,
                    Padding = new Padding(10)
                };
                Sunny.UI.UITextBox txtSearchNew = new Sunny.UI.UITextBox
                {
                    Name = "txtSearch",
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    Font = new Font("JetBrains Mono", 9F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(12, 30),
                    Size = new Size(316, 29),
                    Watermark = "Buscar por ID / Nombre / Categoría…",
                    ShowText = false,
                    FillColor = Color.White,
                    RectColor = Color.FromArgb(110, 190, 40),
                    TextAlignment = ContentAlignment.MiddleLeft
                };
                Sunny.UI.UIComboBox cboNew = new Sunny.UI.UIComboBox
                {
                    Name = "cboFiltroCategoria",
                    Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right,
                    DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList,
                    Font = new Font("JetBrains Mono", 9F, FontStyle.Regular, GraphicsUnit.Point),
                    Location = new Point(12, 61),
                    Size = new Size(316, 29),
                    FillColor = Color.White,
                    RectColor = Color.FromArgb(110, 190, 40),
                    TextAlignment = ContentAlignment.MiddleLeft,
                    Watermark = ""
                };
                cboNew.Items.AddRange(_categoriasFiltro);
                cboNew.SelectedIndex = 0;
                pnlBuscadorNew.Controls.Add(txtSearchNew);
                pnlBuscadorNew.Controls.Add(cboNew);

                Panel pnlSummaryNew = new Panel
                {
                    Name = "pnlSummary",
                    Dock = DockStyle.Bottom,
                    Height = 44,
                    BackColor = Color.FromArgb(110, 190, 40),
                    Padding = new Padding(10, 4, 10, 4)
                };
                Sunny.UI.UILabel lblTotalNew = new Sunny.UI.UILabel
                {
                    Name = "lblTotal",
                    Dock = DockStyle.Fill,
                    Font = new Font("JetBrains Mono", 9F, FontStyle.Bold, GraphicsUnit.Point),
                    ForeColor = Color.White,
                    Text = "Total: 0 productos",
                    TextAlign = ContentAlignment.MiddleLeft
                };
                pnlSummaryNew.Controls.Add(lblTotalNew);

                FlowLayoutPanel flpNew = new FlowLayoutPanel
                {
                    Name = "flpProductos",
                    Dock = DockStyle.Fill,
                    AutoScroll = true,
                    FlowDirection = FlowDirection.TopDown,
                    WrapContents = false,
                    BackColor = Color.FromArgb(245, 245, 245),
                    Padding = new Padding(6)
                };

                panelIzqNew.Controls.Add(flpNew);
                panelIzqNew.Controls.Add(pnlSummaryNew);
                panelIzqNew.Controls.Add(pnlBuscadorNew);

                Controls.Add(panelIzqNew);
                panelIzqNew.BringToFront();

                // Asigna a los campos del Designer si son accesibles via reflexión fallback
                // (no necesario para Find, pero útil para referencias directas posteriores)
                // Se deja que futuros accesos usen Find si los campos siguen null.
            }
            catch { /* fallback no crítico */ }
        }

        private void WireCatalogEvents()
        {
            if (_catalogEventsWired)
            {
                return;
            }

            try
            {
                // txtSearch (Sunny.UI.UITextBox)
                Sunny.UI.UITextBox? txt = null;
                try { txt = txtSearch; } catch { txt = null; }
                txt ??= Controls.Find("txtSearch", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (txt != null)
                {
                    txt.TextChanged -= TxtSearch_TextChanged;
                    txt.TextChanged += TxtSearch_TextChanged;
                }

                Sunny.UI.UIComboBox? cbo = null;
                try { cbo = cboFiltroCategoria; } catch { cbo = null; }
                cbo ??= Controls.Find("cboFiltroCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;
                if (cbo != null)
                {
                    cbo.SelectedIndexChanged -= CboFiltroCategoria_SelectedIndexChanged;
                    cbo.SelectedIndexChanged += CboFiltroCategoria_SelectedIndexChanged;
                }

                FlowLayoutPanel? flp = null;
                try { flp = flpProductos; } catch { flp = null; }
                flp ??= Controls.Find("flpProductos", true).FirstOrDefault() as FlowLayoutPanel;
                if (flp != null)
                {
                    flp.SizeChanged -= FlpProductos_SizeChanged;
                    flp.SizeChanged += FlpProductos_SizeChanged;
                }

                _catalogEventsWired = true;
            }
            catch { /* defensivo */ }
        }

        /// <summary>
        /// Conecta los eventos de la ToolStrip superior (barraHerramientas).
        /// Usa patrón defensivo con campo directo + Controls.Find fallback para compatibilidad con Designer antiguo.
        /// </summary>
        private void WireToolbarEvents()
        {
            if (_toolbarEventsWired)
            {
                try { WireNuevoProductoEvents(); } catch { }
                return;
            }

            try
            {
                ToolStrip? barra = null;
                try { barra = barraHerramientas; } catch { barra = null; }
                barra ??= Controls.Find("barraHerramientas", true).FirstOrDefault() as ToolStrip;
                if (barra == null)
                {
                    return;
                }

                ToolStripButton? bNuevo = null;
                try { bNuevo = btnNuevo; } catch { bNuevo = null; }
                bNuevo ??= barra.Items["btnNuevo"] as ToolStripButton;
                bNuevo ??= barra.Items.OfType<ToolStripButton>().FirstOrDefault(b => b.Name == "btnNuevo");

                ToolStripButton? bEditar = null;
                try { bEditar = btnEditar; } catch { bEditar = null; }
                bEditar ??= barra.Items["btnEditar"] as ToolStripButton;
                bEditar ??= barra.Items.OfType<ToolStripButton>().FirstOrDefault(b => b.Name == "btnEditar");

                ToolStripButton? bImportar = null;
                try { bImportar = btnImportar; } catch { bImportar = null; }
                bImportar ??= barra.Items["btnImportar"] as ToolStripButton;
                bImportar ??= barra.Items.OfType<ToolStripButton>().FirstOrDefault(b => b.Name == "btnImportar");

                if (bNuevo != null)
                {
                    bNuevo.Click -= BtnToolbarNuevo_Click;
                    bNuevo.Click += BtnToolbarNuevo_Click;
                }

                if (bEditar != null)
                {
                    bEditar.Click -= BtnToolbarEditar_Click;
                    bEditar.Click += BtnToolbarEditar_Click;
                }

                if (bImportar != null)
                {
                    bImportar.Click -= BtnToolbarImportar_Click;
                    bImportar.Click += BtnToolbarImportar_Click;
                }

                _toolbarEventsWired = true;
                ActualizarEstadoToolbar();
                WireNuevoProductoEvents();
            }
            catch { /* defensivo: si Toolbar no existe en Designer antiguo, no rompe */ }

            // Asegura que el nuevo también se wire aunque la toolbar ya estuviera cableada
            try { WireNuevoProductoEvents(); } catch { }
        }

        /// <summary>
        /// Cablea los eventos de la pestaña Producto Nuevo (btnGuardarNuevo y limpieza de errores).
        /// Debe ser idempotente y no romper si los controles aún no existen en Designer antiguo.
        /// </summary>
        private void WireNuevoProductoEvents()
        {
            if (_nuevoProductoEventsWired)
            {
                return;
            }

            try
            {
                EnsureErrorProviderNuevo();

                Sunny.UI.UIButton? btnGuardar = null;
                try { btnGuardar = btnGuardarNuevo; } catch { btnGuardar = null; }
                btnGuardar ??= Controls.Find("btnGuardarNuevo", true).FirstOrDefault() as Sunny.UI.UIButton;
                if (btnGuardar != null)
                {
                    btnGuardar.Click -= BtnGuardarNuevo_Click;
                    btnGuardar.Click += BtnGuardarNuevo_Click;
                }

                // Limpia ErrorProvider al editar
                Sunny.UI.UITextBox? tId = null;
                try { tId = txtNuevoId; } catch { tId = null; }
                tId ??= Controls.Find("txtNuevoId", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tId != null)
                {
                    tId.TextChanged -= NuevoCampo_TextChanged;
                    tId.TextChanged += NuevoCampo_TextChanged;
                }

                Sunny.UI.UITextBox? tNombre = null;
                try { tNombre = txtNuevoNombre; } catch { tNombre = null; }
                tNombre ??= Controls.Find("txtNuevoNombre", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tNombre != null)
                {
                    tNombre.TextChanged -= NuevoCampo_TextChanged;
                    tNombre.TextChanged += NuevoCampo_TextChanged;
                }

                Sunny.UI.UITextBox? tPrecio = null;
                try { tPrecio = txtNuevoPrecio; } catch { tPrecio = null; }
                tPrecio ??= Controls.Find("txtNuevoPrecio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tPrecio != null)
                {
                    tPrecio.TextChanged -= NuevoCampo_TextChanged;
                    tPrecio.TextChanged += NuevoCampo_TextChanged;
                }

                Sunny.UI.UITextBox? tRatio = null;
                try { tRatio = txtNuevoRatio; } catch { tRatio = null; }
                tRatio ??= Controls.Find("txtNuevoRatio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tRatio != null)
                {
                    tRatio.TextChanged -= NuevoCampo_TextChanged;
                    tRatio.TextChanged += NuevoCampo_TextChanged;
                }

                Sunny.UI.UITextBox? tDescrip = null;
                try { tDescrip = txtNuevoDescrip; } catch { tDescrip = null; }
                tDescrip ??= Controls.Find("txtNuevoDescrip", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tDescrip != null)
                {
                    tDescrip.TextChanged -= NuevoCampo_TextChanged;
                    tDescrip.TextChanged += NuevoCampo_TextChanged;
                }

                Sunny.UI.UITextBox? tRef = null;
                try { tRef = txtNuevoRef; } catch { tRef = null; }
                tRef ??= Controls.Find("txtNuevoRef", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tRef != null)
                {
                    tRef.TextChanged -= NuevoCampo_TextChanged;
                    tRef.TextChanged += NuevoCampo_TextChanged;
                }

                Sunny.UI.UITextBox? tBar = null;
                try { tBar = txtNuevoCodebar; } catch { tBar = null; }
                tBar ??= Controls.Find("txtNuevoCodebar", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tBar != null)
                {
                    tBar.TextChanged -= NuevoCampo_TextChanged;
                    tBar.TextChanged += NuevoCampo_TextChanged;
                }

                Sunny.UI.UIComboBox? cboCat = null;
                try { cboCat = cboNuevoCategoria; } catch { cboCat = null; }
                cboCat ??= Controls.Find("cboNuevoCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;
                if (cboCat != null)
                {
                    cboCat.SelectedIndexChanged -= NuevoCategoria_SelectedIndexChanged;
                    cboCat.SelectedIndexChanged += NuevoCategoria_SelectedIndexChanged;
                }

                _nuevoProductoEventsWired = true;
            }
            catch { /* defensivo */ }
        }

        private void NuevoCampo_TextChanged(object? sender, EventArgs e)
        {
            try
            {
                if (sender is Control c)
                {
                    _errorProviderNuevo?.SetError(c, string.Empty);
                }
            }
            catch { }
        }

        private void NuevoCategoria_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                Sunny.UI.UIComboBox? cbo = null;
                try { cbo = cboNuevoCategoria; } catch { cbo = null; }
                cbo ??= Controls.Find("cboNuevoCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;
                if (cbo != null)
                {
                    _errorProviderNuevo?.SetError(cbo, string.Empty);
                }
            }
            catch { }
        }

        private void EnsureErrorProviderNuevo()
        {
            if (_errorProviderNuevo != null)
            {
                return;
            }

            try
            {
                components ??= new System.ComponentModel.Container();
                _errorProviderNuevo = new System.Windows.Forms.ErrorProvider(components)
                {
                    BlinkStyle = System.Windows.Forms.ErrorBlinkStyle.NeverBlink
                };
            }
            catch { /* no crítico */ }
        }

        private void LimpiarNuevoProducto()
        {
            try
            {
                Sunny.UI.UITextBox? tId = null;
                try { tId = txtNuevoId; } catch { tId = null; }
                tId ??= Controls.Find("txtNuevoId", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tNombre = null;
                try { tNombre = txtNuevoNombre; } catch { tNombre = null; }
                tNombre ??= Controls.Find("txtNuevoNombre", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tDescrip = null;
                try { tDescrip = txtNuevoDescrip; } catch { tDescrip = null; }
                tDescrip ??= Controls.Find("txtNuevoDescrip", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRef = null;
                try { tRef = txtNuevoRef; } catch { tRef = null; }
                tRef ??= Controls.Find("txtNuevoRef", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tBar = null;
                try { tBar = txtNuevoCodebar; } catch { tBar = null; }
                tBar ??= Controls.Find("txtNuevoCodebar", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tPrecio = null;
                try { tPrecio = txtNuevoPrecio; } catch { tPrecio = null; }
                tPrecio ??= Controls.Find("txtNuevoPrecio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRatio = null;
                try { tRatio = txtNuevoRatio; } catch { tRatio = null; }
                tRatio ??= Controls.Find("txtNuevoRatio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UIComboBox? cboCat = null;
                try { cboCat = cboNuevoCategoria; } catch { cboCat = null; }
                cboCat ??= Controls.Find("cboNuevoCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;

                if (tId != null) { tId.Text = string.Empty; }
                if (tNombre != null) { tNombre.Text = string.Empty; }
                if (tDescrip != null) { tDescrip.Text = string.Empty; }
                if (tRef != null) { tRef.Text = string.Empty; }
                if (tBar != null) { tBar.Text = string.Empty; }
                if (tPrecio != null) { tPrecio.Text = string.Empty; }
                if (tRatio != null) { tRatio.Text = string.Empty; }
                if (cboCat != null) { cboCat.SelectedIndex = -1; }

                _errorProviderNuevo?.Clear();
            }
            catch { /* no crítico */ }
        }

        private void ResetModoEdicion()
        {
            _modoEdicion = false;
            _idEnEdicion = null;
            try
            {
                Sunny.UI.UITextBox? tId = null;
                try { tId = txtNuevoId; } catch { tId = null; }
                tId ??= Controls.Find("txtNuevoId", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (tId != null)
                {
                    tId.ReadOnly = false;
                    tId.FillColor = Color.White;
                }

                Sunny.UI.UIButton? btnG = null;
                try { btnG = btnGuardarNuevo; } catch { btnG = null; }
                btnG ??= Controls.Find("btnGuardarNuevo", true).FirstOrDefault() as Sunny.UI.UIButton;
                if (btnG != null)
                {
                    btnG.Text = "Guardar";
                }

                Sunny.UI.UITabControl? tab = null;
                try { tab = tabDetalle; } catch { tab = null; }
                tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;
                TabPage? pg = null;
                try { pg = tabNuevoProducto; } catch { pg = null; }
                pg ??= Controls.Find("tabNuevoProducto", true).FirstOrDefault() as TabPage;
                if (pg != null)
                {
                    pg.Text = "Producto Nuevo";
                }
            }
            catch { /* no crítico */ }
        }

        private void ActivarModoEdicion(DataRow row)
        {
            if (row == null) { return; }

            _modoEdicion = true;
            _idEnEdicion = GetStringSafe(row, "product_id");
            try
            {
                Sunny.UI.UITextBox? tId = null;
                try { tId = txtNuevoId; } catch { tId = null; }
                tId ??= Controls.Find("txtNuevoId", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tNombre = null;
                try { tNombre = txtNuevoNombre; } catch { tNombre = null; }
                tNombre ??= Controls.Find("txtNuevoNombre", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tDescrip = null;
                try { tDescrip = txtNuevoDescrip; } catch { tDescrip = null; }
                tDescrip ??= Controls.Find("txtNuevoDescrip", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRef = null;
                try { tRef = txtNuevoRef; } catch { tRef = null; }
                tRef ??= Controls.Find("txtNuevoRef", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tBar = null;
                try { tBar = txtNuevoCodebar; } catch { tBar = null; }
                tBar ??= Controls.Find("txtNuevoCodebar", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tPrecio = null;
                try { tPrecio = txtNuevoPrecio; } catch { tPrecio = null; }
                tPrecio ??= Controls.Find("txtNuevoPrecio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRatio = null;
                try { tRatio = txtNuevoRatio; } catch { tRatio = null; }
                tRatio ??= Controls.Find("txtNuevoRatio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UIComboBox? cboCat = null;
                try { cboCat = cboNuevoCategoria; } catch { cboCat = null; }
                cboCat ??= Controls.Find("cboNuevoCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;

                string id = GetStringSafe(row, "product_id");
                string nombre = GetStringSafe(row, "product_name");
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    nombre = GetStringSafe(row, "product_descrip");
                }

                string descrip = GetStringSafe(row, "product_descrip");
                string referencia = GetStringSafe(row, "product_ref");
                if (string.IsNullOrWhiteSpace(referencia))
                {
                    referencia = GetStringSafe(row, "code_rc");
                }

                string codebar = GetStringSafe(row, "codebar");
                string categoria = ObtenerCategoria(row);
                string precio = FormatearDecimalParaDetalle(GetDecimalSafe(row, "precio"));
                string ratio = FormatearDecimalParaDetalle(GetDecimalSafe(row, "ratio"));

                if (tId != null)
                {
                    tId.Text = id;
                    tId.ReadOnly = true;
                    tId.FillColor = Color.WhiteSmoke;
                }

                if (tNombre != null) { tNombre.Text = nombre; }
                if (tDescrip != null) { tDescrip.Text = descrip; }
                if (tRef != null) { tRef.Text = referencia; }
                if (tBar != null) { tBar.Text = codebar; }
                if (tPrecio != null) { tPrecio.Text = precio; }
                if (tRatio != null) { tRatio.Text = ratio; }
                if (cboCat != null)
                {
                    int idx = -1;
                    for (int i = 0; i < cboCat.Items.Count; i++)
                    {
                        if (string.Equals(cboCat.Items[i]?.ToString(), categoria, StringComparison.OrdinalIgnoreCase))
                        {
                            idx = i;
                            break;
                        }
                    }
                    cboCat.SelectedIndex = idx;
                }

                Sunny.UI.UIButton? btnG = null;
                try { btnG = btnGuardarNuevo; } catch { btnG = null; }
                btnG ??= Controls.Find("btnGuardarNuevo", true).FirstOrDefault() as Sunny.UI.UIButton;
                if (btnG != null)
                {
                    btnG.Text = "Actualizar";
                }

                Sunny.UI.UITabControl? tab = null;
                try { tab = tabDetalle; } catch { tab = null; }
                tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;
                TabPage? pg = null;
                try { pg = tabNuevoProducto; } catch { pg = null; }
                pg ??= Controls.Find("tabNuevoProducto", true).FirstOrDefault() as TabPage;
                if (pg != null)
                {
                    pg.Text = "Editar Producto";
                }

                _errorProviderNuevo?.Clear();
            }
            catch { /* no crítico */ }
        }

        private static bool TryParseDecimalSafe(string input, out decimal value)
        {
            value = 0m;
            if (string.IsNullOrWhiteSpace(input))
            {
                return false;
            }

            string trimmed = input.Trim();

            if (decimal.TryParse(trimmed, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal r1))
            {
                value = r1;
                return true;
            }

            if (decimal.TryParse(trimmed, NumberStyles.Any, new CultureInfo("es-ES"), out decimal r2))
            {
                value = r2;
                return true;
            }

            // Fallback con coma/punto normalizado
            string normalized = trimmed.Replace(',', '.');
            if (decimal.TryParse(normalized, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal r3))
            {
                value = r3;
                return true;
            }

            return false;
        }

        private static decimal ParseDecimalSafe(string input)
        {
            if (string.IsNullOrWhiteSpace(input))
            {
                return 0m;
            }

            if (TryParseDecimalSafe(input, out decimal v))
            {
                return v;
            }

            return 0m;
        }

        private void EnsureNuevoProductoTabOculta()
        {
            try
            {
                Sunny.UI.UITabControl? tab = null;
                try { tab = tabDetalle; } catch { tab = null; }
                tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;
                TabPage? pg = null;
                try { pg = tabNuevoProducto; } catch { pg = null; }
                pg ??= Controls.Find("tabNuevoProducto", true).FirstOrDefault() as TabPage;
                if (tab != null && pg != null && tab.TabPages.Contains(pg))
                {
                    tab.TabPages.Remove(pg);
                }
            }
            catch { /* no crítico */ }
        }

        /// <summary>
        /// Actualiza el estado habilitado/deshabilitado de btnEditar según haya selección.
        /// Busca el botón vía campo directo o fallback Find para compatibilidad con Designer antiguo.
        /// </summary>
        private void ActualizarEstadoToolbar()
        {
            try
            {
                ToolStripButton? bEditar = null;
                try { bEditar = btnEditar; } catch { bEditar = null; }
                if (bEditar == null)
                {
                    ToolStrip? barra = null;
                    try { barra = barraHerramientas; } catch { barra = null; }
                    barra ??= Controls.Find("barraHerramientas", true).FirstOrDefault() as ToolStrip;
                    if (barra != null)
                    {
                        bEditar = barra.Items["btnEditar"] as ToolStripButton;
                        bEditar ??= barra.Items.OfType<ToolStripButton>().FirstOrDefault(b => b.Name == "btnEditar");
                    }
                }

                if (bEditar != null)
                {
                    bool habilitado = _cardSeleccionada != null && _cardSeleccionada.IsSelected;
                    bEditar.Enabled = habilitado;
                }
            }
            catch { /* no crítico */ }
        }

        /// <summary>
        /// Handler de la ToolStrip: Nuevo producto.
        /// Crea o selecciona la pestaña Producto Nuevo, limpia campos y enfoca el ID.
        /// La pestaña NO está agregada en Designer; se agrega dinámicamente aquí.
        /// </summary>
        private void BtnToolbarNuevo_Click(object? sender, EventArgs e)
        {
            try
            {
                // Deselecciona cards existentes
                try
                {
                    FlowLayoutPanel? flp = null;
                    try { flp = flpProductos; } catch { flp = null; }
                    flp ??= Controls.Find("flpProductos", true).FirstOrDefault() as FlowLayoutPanel;
                    if (flp != null)
                    {
                        foreach (ProductCard c in flp.Controls.OfType<ProductCard>())
                        {
                            c.IsSelected = false;
                        }
                    }
                }
                catch { }

                _cardSeleccionada = null;
                LimpiarDetalleProducto();
                ActualizarEstadoToolbar();

                // Asegura wiring de la pestaña nueva
                WireNuevoProductoEvents();
                EnsureErrorProviderNuevo();

                Sunny.UI.UITabControl? tab = null;
                try { tab = tabDetalle; } catch { tab = null; }
                tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;

                TabPage? pgNuevo = null;
                try { pgNuevo = tabNuevoProducto; } catch { pgNuevo = null; }
                pgNuevo ??= Controls.Find("tabNuevoProducto", true).FirstOrDefault() as TabPage;

                if (tab == null || pgNuevo == null)
                {
                    ServiceErrors.Report("No se pudo abrir la pestaña Producto Nuevo (controles no encontrados).");
                    MessageBox.Show(this, "No se pudo abrir la pestaña Producto Nuevo.", "Nuevo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                // Reset modo edición y prepara UI para alta
                ResetModoEdicion();
                LimpiarNuevoProducto();

                // Si ya existe, solo seleccionar; si no, agregar dinámicamente
                if (!tab.TabPages.Contains(pgNuevo))
                {
                    tab.TabPages.Add(pgNuevo);
                }

                tab.SelectedTab = pgNuevo;

                AplicarTemaCatalogo();

                // Enfoca txtNuevoId (ahora editable)
                try
                {
                    Sunny.UI.UITextBox? tId = null;
                    try { tId = txtNuevoId; } catch { tId = null; }
                    tId ??= Controls.Find("txtNuevoId", true).FirstOrDefault() as Sunny.UI.UITextBox;
                    tId?.Focus();
                }
                catch { }
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("BtnToolbarNuevo_Click", ex);
                ServiceErrors.Report("Error en Nuevo: " + ex.Message);
            }
        }

        /// <summary>
        /// Handler del botón Guardar en pestaña Producto Nuevo. Valida antes de guardar y usa async/await sin bloquear UI.
        /// </summary>
        private async void BtnGuardarNuevo_Click(object? sender, EventArgs e)
        {
            Sunny.UI.UIButton? btnGuardar = null;
            try { btnGuardar = btnGuardarNuevo; } catch { btnGuardar = null; }
            btnGuardar ??= Controls.Find("btnGuardarNuevo", true).FirstOrDefault() as Sunny.UI.UIButton;

            try
            {
                EnsureErrorProviderNuevo();
                _errorProviderNuevo?.Clear();

                if (btnGuardar != null)
                {
                    btnGuardar.Enabled = false;
                }

                Cursor = Cursors.WaitCursor;

                // Resuelve controles con fallback
                Sunny.UI.UITextBox? tId = null;
                try { tId = txtNuevoId; } catch { tId = null; }
                tId ??= Controls.Find("txtNuevoId", true).FirstOrDefault() as Sunny.UI.UITextBox;

                Sunny.UI.UITextBox? tNombre = null;
                try { tNombre = txtNuevoNombre; } catch { tNombre = null; }
                tNombre ??= Controls.Find("txtNuevoNombre", true).FirstOrDefault() as Sunny.UI.UITextBox;

                Sunny.UI.UITextBox? tDescrip = null;
                try { tDescrip = txtNuevoDescrip; } catch { tDescrip = null; }
                tDescrip ??= Controls.Find("txtNuevoDescrip", true).FirstOrDefault() as Sunny.UI.UITextBox;

                Sunny.UI.UITextBox? tRef = null;
                try { tRef = txtNuevoRef; } catch { tRef = null; }
                tRef ??= Controls.Find("txtNuevoRef", true).FirstOrDefault() as Sunny.UI.UITextBox;

                Sunny.UI.UITextBox? tBar = null;
                try { tBar = txtNuevoCodebar; } catch { tBar = null; }
                tBar ??= Controls.Find("txtNuevoCodebar", true).FirstOrDefault() as Sunny.UI.UITextBox;

                Sunny.UI.UIComboBox? cboCat = null;
                try { cboCat = cboNuevoCategoria; } catch { cboCat = null; }
                cboCat ??= Controls.Find("cboNuevoCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;

                Sunny.UI.UITextBox? tPrecio = null;
                try { tPrecio = txtNuevoPrecio; } catch { tPrecio = null; }
                tPrecio ??= Controls.Find("txtNuevoPrecio", true).FirstOrDefault() as Sunny.UI.UITextBox;

                Sunny.UI.UITextBox? tRatio = null;
                try { tRatio = txtNuevoRatio; } catch { tRatio = null; }
                tRatio ??= Controls.Find("txtNuevoRatio", true).FirstOrDefault() as Sunny.UI.UITextBox;

                string id = (tId?.Text ?? string.Empty).Trim();
                string nombre = (tNombre?.Text ?? string.Empty).Trim();
                string descrip = (tDescrip?.Text ?? string.Empty).Trim();
                string referencia = (tRef?.Text ?? string.Empty).Trim();
                string codebar = (tBar?.Text ?? string.Empty).Trim();
                string precioText = (tPrecio?.Text ?? string.Empty).Trim();
                string ratioText = (tRatio?.Text ?? string.Empty).Trim();
                string categoriaSel = (cboCat?.SelectedItem?.ToString() ?? cboCat?.Text ?? string.Empty).Trim();

                // ── Validación Product ID ──
                if (string.IsNullOrWhiteSpace(id))
                {
                    string msg = "El código es requerido.";
                    if (tId != null) { _errorProviderNuevo?.SetError(tId, msg); }
                    ServiceErrors.Report(msg);
                    MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tId?.Focus();
                    return;
                }

                if (id.Length > 50)
                {
                    string msg = "El código no puede superar 50 caracteres.";
                    if (tId != null) { _errorProviderNuevo?.SetError(tId, msg); }
                    MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tId?.Focus();
                    return;
                }

                // ── Validación Nombre ──
                if (string.IsNullOrWhiteSpace(nombre))
                {
                    string msg = "El nombre es requerido.";
                    if (tNombre != null) { _errorProviderNuevo?.SetError(tNombre, msg); }
                    ServiceErrors.Report(msg);
                    MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    tNombre?.Focus();
                    return;
                }

                // ── Validación Categoría ──
                if (string.IsNullOrWhiteSpace(categoriaSel))
                {
                    string msg = "Seleccione una categoría.";
                    if (cboCat != null) { _errorProviderNuevo?.SetError(cboCat, msg); }
                    ServiceErrors.Report(msg);
                    MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboCat?.Focus();
                    return;
                }

                string[] categoriasValidas = ["Master", "Rollo Cortado", "Resma", "Graphics"];
                bool catValida = categoriasValidas.Any(c => c.Equals(categoriaSel, StringComparison.OrdinalIgnoreCase));
                if (!catValida)
                {
                    string msg = "Seleccione una categoría válida: Master, Rollo Cortado, Resma o Graphics.";
                    if (cboCat != null) { _errorProviderNuevo?.SetError(cboCat, msg); }
                    MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    cboCat?.Focus();
                    return;
                }

                // ── Validación Precio ──
                decimal precio = 0m;
                if (!string.IsNullOrWhiteSpace(precioText))
                {
                    if (!TryParseDecimalSafe(precioText, out precio))
                    {
                        string msg = "El precio debe ser un número válido mayor o igual a 0.";
                        if (tPrecio != null) { _errorProviderNuevo?.SetError(tPrecio, msg); }
                        MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tPrecio?.Focus();
                        return;
                    }

                    if (precio < 0)
                    {
                        string msg = "El precio no puede ser negativo.";
                        if (tPrecio != null) { _errorProviderNuevo?.SetError(tPrecio, msg); }
                        MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tPrecio?.Focus();
                        return;
                    }
                }

                // ── Validación Ratio ──
                decimal ratio = 0m;
                if (!string.IsNullOrWhiteSpace(ratioText))
                {
                    if (!TryParseDecimalSafe(ratioText, out ratio))
                    {
                        string msg = "El ratio debe ser un número válido mayor o igual a 0.";
                        if (tRatio != null) { _errorProviderNuevo?.SetError(tRatio, msg); }
                        MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tRatio?.Focus();
                        return;
                    }

                    if (ratio < 0)
                    {
                        string msg = "El ratio no puede ser negativo.";
                        if (tRatio != null) { _errorProviderNuevo?.SetError(tRatio, msg); }
                        MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        tRatio?.Focus();
                        return;
                    }
                }

                // ── Modo edición vs alta: id efectivo y validación de duplicado solo en alta ──
                bool esEdicion = _modoEdicion && !string.IsNullOrWhiteSpace(_idEnEdicion);
                string idEfectivo = esEdicion ? _idEnEdicion!.Trim() : id;

                if (esEdicion)
                {
                    // El ID no se edita; asegura que el campo visible coincida con el original
                    id = idEfectivo;
                    if (tId != null && !string.Equals((tId.Text ?? string.Empty).Trim(), idEfectivo, StringComparison.Ordinal))
                    {
                        // Corrige UI si por algún motivo se desincronizó (defensivo)
                        Action fixId = () => tId.Text = idEfectivo;
                        if (IsHandleCreated && InvokeRequired) { try { Invoke(fixId); } catch { } } else { try { fixId(); } catch { } }
                    }
                }
                else
                {
                    // ── Verificar existencia previa solo en alta (evita mensaje genérico de BD) ──
                    Result<bool> existsResult = await _productsService.ExistsAsync(id).ConfigureAwait(false);
                    if (existsResult.IsSuccess && existsResult.Value)
                    {
                        string msg = $"Ya existe un producto con el código '{id}'.";
                        Action showExists = () =>
                        {
                            if (tId != null) { _errorProviderNuevo?.SetError(tId, msg); }
                            ServiceErrors.Report(msg);
                            MessageBox.Show(this, msg, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                            tId?.Focus();
                        };

                        if (IsHandleCreated && InvokeRequired)
                        {
                            Invoke(showExists);
                        }
                        else
                        {
                            showExists();
                        }

                        return;
                    }
                }

                // ── Construir entidad ──
                Product producto = new()
                {
                    Product_id = idEfectivo,
                    Product_Name = nombre,
                    Product_Description = descrip,
                    Referencia = referencia,
                    Codigo_Barra = codebar,
                    Precio = precio,
                    Ratio = ratio,
                    Anulado = false,
                    Master = categoriaSel.Equals("Master", StringComparison.OrdinalIgnoreCase),
                    RolloCortado = categoriaSel.Equals("Rollo Cortado", StringComparison.OrdinalIgnoreCase),
                    Hoja = categoriaSel.Equals("Resma", StringComparison.OrdinalIgnoreCase),
                    Graphics = categoriaSel.Equals("Graphics", StringComparison.OrdinalIgnoreCase)
                };

                // ── Validación de dominio vía servicio (categoría exclusiva, negativos, etc.) ──
                Result validacion = _productsService.ValidateProduct(producto);
                if (!validacion.IsSuccess)
                {
                    string err = validacion.Error ?? "Error de validación.";
                    ServiceErrors.Report(err);
                    Action showVal = () => MessageBox.Show(this, err, "Validación", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    if (IsHandleCreated && InvokeRequired)
                    {
                        Invoke(showVal);
                    }
                    else
                    {
                        showVal();
                    }

                    // Mapea error de categoría al combo
                    if (validacion.ErrorCode == ProductValidator.CODE_CATEGORY && cboCat != null)
                    {
                        _errorProviderNuevo?.SetError(cboCat, err);
                        cboCat.Focus();
                    }
                    else if (validacion.ErrorCode == ProductValidator.CODE_REQUIRED)
                    {
                        // Si es por código/nombre, ya se validó arriba; enfoca genérico
                        tId?.Focus();
                    }

                    return;
                }

                // ── Guardado async (branch Add vs Update) ──
                Result<bool> result;
                if (esEdicion)
                {
                    result = await _productsService.UpdateValidatedAsync(producto).ConfigureAwait(false);
                }
                else
                {
                    result = await _productsService.AddValidatedAsync(producto).ConfigureAwait(false);
                }
                if (!result.IsSuccess)
                {
                    string err = result.Error ?? "Error al guardar el producto.";
                    ServiceErrors.Report(err);
                    // Si es anulado, enfoca info
                    if (result.ErrorCode == ProductValidator.CODE_ANULADO)
                    {
                        err += "\nEl producto anulado no se puede editar.";
                    }

                    Action showErr = () => MessageBox.Show(this, err, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (IsHandleCreated && InvokeRequired)
                    {
                        Invoke(showErr);
                    }
                    else
                    {
                        showErr();
                    }

                    // Si el error fue por anulado, resetea modo edición para evitar reintentos inconsistentes
                    if (result.ErrorCode == ProductValidator.CODE_ANULADO && esEdicion)
                    {
                        Action resetOnErr = () => ResetModoEdicion();
                        if (IsHandleCreated && InvokeRequired) { try { Invoke(resetOnErr); } catch { } } else { try { resetOnErr(); } catch { } }
                    }

                    return;
                }

                // ── Éxito: mensaje, remover pestaña, limpiar y recargar catálogo ──
                void OnSuccessUI()
                {
                    string msgOk = esEdicion ? "Producto actualizado correctamente." : "Producto guardado correctamente.";
                    MessageBox.Show(this, msgOk, "Éxito", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    try
                    {
                        Sunny.UI.UITabControl? tab = null;
                        try { tab = tabDetalle; } catch { tab = null; }
                        tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;
                        TabPage? pg = null;
                        try { pg = tabNuevoProducto; } catch { pg = null; }
                        pg ??= Controls.Find("tabNuevoProducto", true).FirstOrDefault() as TabPage;
                        if (tab != null && pg != null && tab.TabPages.Contains(pg))
                        {
                            tab.TabPages.Remove(pg);
                        }
                    }
                    catch { }

                    LimpiarNuevoProducto();
                    ResetModoEdicion();
                    AplicarTemaCatalogo();
                }

                if (IsHandleCreated && InvokeRequired)
                {
                    Invoke(OnSuccessUI);
                }
                else if (IsHandleCreated)
                {
                    OnSuccessUI();
                }

                // Recargar catálogo sin bloquear UI: carga en background y render en UI thread
                // Asegura reset del modo por si OnSuccessUI no se ejecutó (handle no creado)
                // Captura id a reseleccionar antes del reset (tanto en alta como en edición)
                string? seleccionarIdPost = idEfectivo;
                ResetModoEdicion();
                try
                {
                    DataSet ds = await _productsService.Load().ConfigureAwait(false);
                    DataTable? dt = ds.Tables["Dtproducts"]
                        ?? ds.Tables["DtProducts"]
                        ?? ds.Tables["DtProductos"]
                        ?? (ds.Tables.Count > 0 ? ds.Tables[0] : null);
                    DataTable dtNuevo = dt ?? new DataTable();

                    void UpdateCatalogUI()
                    {
                        _dtProductos = dtNuevo;
                        FiltrarYRender(seleccionarIdPost);
                        // Asegura que el detalle del producto seleccionado sea visible en tabGeneral
                        try
                        {
                            Sunny.UI.UITabControl? tab = null;
                            try { tab = tabDetalle; } catch { tab = null; }
                            tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;
                            TabPage? pg = null;
                            try { pg = tabGeneral; } catch { pg = null; }
                            pg ??= Controls.Find("tabGeneral", true).FirstOrDefault() as TabPage;
                            if (tab != null && pg != null && tab.TabPages.Contains(pg))
                            {
                                tab.SelectedTab = pg;
                            }
                        }
                        catch { /* no crítico */ }

                        ActualizarEstadoToolbar();
                        AplicarTemaCatalogo();
                    }

                    if (IsHandleCreated && InvokeRequired)
                    {
                        Invoke(UpdateCatalogUI);
                    }
                    else if (IsHandleCreated)
                    {
                        UpdateCatalogUI();
                    }
                    else
                    {
                        _dtProductos = dtNuevo;
                    }
                }
                catch (Exception exLoad)
                {
                    ServiceLogger.LogError("BtnGuardarNuevo_Click:Recargar", exLoad);
                    // Fallback a RecargarCatalogoAsync en UI thread
                    try
                    {
                        if (IsHandleCreated && InvokeRequired)
                        {
                            Invoke(new Action(async () => await RecargarCatalogoAsync().ConfigureAwait(false)));
                        }
                        else
                        {
                            await RecargarCatalogoAsync().ConfigureAwait(false);
                        }
                    }
                    catch { }
                }
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("BtnGuardarNuevo_Click", ex);
                ServiceErrors.Report("Error al guardar: " + ex.Message);
                try
                {
                    Action showEx = () => MessageBox.Show(this, "Error al guardar: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    if (IsHandleCreated && InvokeRequired)
                    {
                        Invoke(showEx);
                    }
                    else
                    {
                        showEx();
                    }
                }
                catch { }
            }
            finally
            {
                try
                {
                    Action restore = () =>
                    {
                        if (btnGuardar != null)
                        {
                            btnGuardar.Enabled = true;
                        }

                        Cursor = Cursors.Default;
                    };

                    if (IsHandleCreated && InvokeRequired)
                    {
                        Invoke(restore);
                    }
                    else
                    {
                        restore();
                    }
                }
                catch
                {
                    if (btnGuardar != null)
                    {
                        try { btnGuardar.Enabled = true; } catch { }
                    }

                    try { Cursor = Cursors.Default; } catch { }
                }
            }
        }

        /// <summary>
        /// Handler de la ToolStrip: Editar producto seleccionado.
        /// Reusa tabNuevoProducto en modo edición: llena campos desde la card, valida anulado, cambia botón a Actualizar.
        /// </summary>
        private void BtnToolbarEditar_Click(object? sender, EventArgs e)
        {
            try
            {
                if (_cardSeleccionada == null || !_cardSeleccionada.IsSelected)
                {
                    MessageBox.Show(this, "Seleccione un producto para editar.", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                DataRow? row = _cardSeleccionada.Tag as DataRow;
                if (row == null)
                {
                    try
                    {
                        string searchId = _cardSeleccionada.ProductId ?? string.Empty;
                        if (!string.IsNullOrWhiteSpace(searchId) && _dtProductos != null && _dtProductos.Rows.Count > 0)
                        {
                            row = _dtProductos.Rows.Cast<DataRow>().FirstOrDefault(r => GetStringSafe(r, "product_id") == searchId);
                        }
                    }
                    catch { row = null; }
                }

                if (row == null)
                {
                    MessageBox.Show(this, "No se pudo obtener el detalle del producto seleccionado.", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Regla de negocio: producto anulado no se puede editar
                if (EsAnulado(row))
                {
                    string idAnulado = GetStringSafe(row, "product_id");
                    string msg = string.IsNullOrWhiteSpace(idAnulado)
                        ? "El producto está anulado y no se puede editar."
                        : $"El producto '{idAnulado}' está anulado y no se puede editar.";
                    ServiceErrors.Report(msg);
                    MessageBox.Show(this, msg, "Editar", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    return;
                }

                // Asegura wiring y resuelve pestaña de edición
                WireNuevoProductoEvents();
                EnsureErrorProviderNuevo();

                Sunny.UI.UITabControl? tab = null;
                try { tab = tabDetalle; } catch { tab = null; }
                tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;

                TabPage? pgNuevo = null;
                try { pgNuevo = tabNuevoProducto; } catch { pgNuevo = null; }
                pgNuevo ??= Controls.Find("tabNuevoProducto", true).FirstOrDefault() as TabPage;

                if (tab == null || pgNuevo == null)
                {
                    ServiceErrors.Report("No se pudo abrir la pestaña de edición (controles no encontrados).");
                    MessageBox.Show(this, "No se pudo abrir la pestaña de edición.", "Editar", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }

                if (!tab.TabPages.Contains(pgNuevo))
                {
                    tab.TabPages.Add(pgNuevo);
                }

                // Carga datos en modo edición y enfoca Nombre
                ActivarModoEdicion(row);
                tab.SelectedTab = pgNuevo;
                AplicarTemaCatalogo();

                try
                {
                    Sunny.UI.UITextBox? tNombre = null;
                    try { tNombre = txtNuevoNombre; } catch { tNombre = null; }
                    tNombre ??= Controls.Find("txtNuevoNombre", true).FirstOrDefault() as Sunny.UI.UITextBox;
                    tNombre?.Focus();
                }
                catch { }
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("BtnToolbarEditar_Click", ex);
                ServiceErrors.Report("Error en Editar: " + ex.Message);
            }
        }

        /// <summary>
        /// Handler de la ToolStrip: Importar productos desde Excel.
        /// Intenta abrir Frm_ImportacionExcel (sin DI) o Frm_Imports (con DI). Fallback: OpenFileDialog + placeholder.
        /// Patrón basado en Frm_Inventarios (grupo import excel).
        /// </summary>
        private void BtnToolbarImportar_Click(object? sender, EventArgs e)
        {
            try
            {
                // Intento 1: Frm_ImportacionExcel (constructor sin parámetros)
                try
                {
                    Frm_ImportacionExcel frmExcel = new Frm_ImportacionExcel();
                    frmExcel.ShowDialog(this);
                    return;
                }
                catch (Exception ex)
                {
                    // Si falla por falta de recursos o error interno, loggea y sigue a fallback
                    ServiceErrors.Report("Importar (Frm_ImportacionExcel) no disponible: " + ex.Message);
                }

                // Intento 2: Frm_Imports requiere IInventarioService – no disponible en este form sin ServiceProvider.
                // Se deja placeholder para integración futura vía DI. Si se inyecta IInventarioService en FrmProductos,
                // aquí se instanciaría: new Frm_Imports(inventarioService) { FileName, PathFileName }.ShowDialog().
                // Por ahora fallback a diálogo de archivo.

                using (OpenFileDialog dlg = new OpenFileDialog())
                {
                    dlg.Filter = "Excel Files|*.xls;*.xlsx;*.xlsm";
                    dlg.Title = "Seleccionar archivo Excel para importar productos";
                    if (dlg.ShowDialog(this) == DialogResult.OK)
                    {
                        string selected = dlg.FileName;
                        MessageBox.Show($"Archivo seleccionado: {selected}\nImportar - funcionalidad por implementar. Integración con Frm_Imports pendiente de DI.", "Importar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                    else
                    {
                        MessageBox.Show("Importar - funcionalidad por implementar", "Importar", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    }
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error en Importar: " + ex.Message);
            }
        }

        private void TxtSearch_TextChanged(object? sender, EventArgs e)
        {
            // Filtros quitados: no re-renderizar por búsqueda para no perder selección actual
            try
            {
                Sunny.UI.UITextBox? txt = sender as Sunny.UI.UITextBox ?? Controls.Find("txtSearch", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (txt != null && (!txt.Visible || !txt.Enabled))
                {
                    return;
                }
            }
            catch { }

            FiltrarYRender();
        }

        private void CboFiltroCategoria_SelectedIndexChanged(object? sender, EventArgs e)
        {
            try
            {
                Sunny.UI.UIComboBox? cbo = sender as Sunny.UI.UIComboBox ?? Controls.Find("cboFiltroCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;
                if (cbo != null && (!cbo.Visible || !cbo.Enabled))
                {
                    return;
                }
            }
            catch { }

            FiltrarYRender();
        }

        private void FlpProductos_SizeChanged(object? sender, EventArgs e)
        {
            FlowLayoutPanel? flp = sender as FlowLayoutPanel ?? flpProductos ?? Controls.Find("flpProductos", true).FirstOrDefault() as FlowLayoutPanel;
            if (flp == null || flp.Controls.Count == 0)
            {
                return;
            }

            flp.SuspendLayout();
            foreach (ProductCard card in flp.Controls.OfType<ProductCard>())
            {
                int ancho = flp.ClientSize.Width - flp.Padding.Horizontal - card.Margin.Horizontal - 6;
                if (ancho < 300)
                {
                    ancho = 300;
                }

                if (ancho > 360)
                {
                    ancho = 360;
                }

                card.Width = ancho;
            }
            flp.ResumeLayout(true);
        }

        /// <summary>
        /// Filtra el catálogo por el texto del buscador (ID o nombre) y la categoría seleccionada,
        /// y renderiza las cards resultantes.
        /// </summary>
        private void FiltrarYRender(string? seleccionarId = null)
        {
            try
            {
                if (_dtProductos == null || _dtProductos.Rows.Count == 0)
                {
                    RenderCards(new DataTable());
                    ActualizarSummary(0, 0);
                    return;
                }

                // Leer texto del buscador
                string texto = string.Empty;
                Sunny.UI.UITextBox? txt = null;
                try { txt = txtSearch; } catch { txt = null; }
                txt ??= Controls.Find("txtSearch", true).FirstOrDefault() as Sunny.UI.UITextBox;
                if (txt != null && txt.Visible && txt.Enabled)
                {
                    texto = txt.Text?.Trim() ?? string.Empty;
                }

                // Leer categoría del combo
                string categoria = "Todos";
                Sunny.UI.UIComboBox? cbo = null;
                try { cbo = cboFiltroCategoria; } catch { cbo = null; }
                cbo ??= Controls.Find("cboFiltroCategoria", true).FirstOrDefault() as Sunny.UI.UIComboBox;
                if (cbo != null && cbo.Visible && cbo.Enabled && cbo.SelectedItem is string sel)
                {
                    categoria = sel;
                }

                // Aplicar filtros
                DataTable dtMostrar = (string.IsNullOrEmpty(texto) && categoria.Equals("Todos", StringComparison.OrdinalIgnoreCase))
                    ? _dtProductos
                    : FiltrarDataTable(texto, categoria);

                RenderCards(dtMostrar, seleccionarId);
                ActualizarSummary(dtMostrar.Rows.Count, _dtProductos.Rows.Count);
                ActualizarEstadoToolbar();

                // Si se seleccionó una card, asegura tabGeneral visible
                if (dtMostrar.Rows.Count > 0 && _cardSeleccionada != null)
                {
                    try
                    {
                        Sunny.UI.UITabControl? tab = null;
                        try { tab = tabDetalle; } catch { tab = null; }
                        tab ??= Controls.Find("tabDetalle", true).FirstOrDefault() as Sunny.UI.UITabControl;
                        TabPage? pg = null;
                        try { pg = tabGeneral; } catch { pg = null; }
                        pg ??= Controls.Find("tabGeneral", true).FirstOrDefault() as TabPage;
                        if (tab != null && pg != null && tab.TabPages.Contains(pg) && tab.SelectedTab != pg)
                        {
                            bool enEdicion = _modoEdicion && tab.TabPages.Contains(Controls.Find("tabNuevoProducto", true).FirstOrDefault() as TabPage ?? tabNuevoProducto);
                            if (!enEdicion)
                            {
                                tab.SelectedTab = pg;
                            }
                        }
                    }
                    catch { /* no crítico */ }
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al renderizar catálogo: " + ex.Message);
            }
        }

        private DataTable FiltrarDataTable(string texto, string categoria)
        {
            DataTable filtrada = _dtProductos.Clone();
            if (_dtProductos.Rows.Count == 0)
            {
                return filtrada;
            }

            string catFiltro = (categoria ?? "Todos").Trim();
            bool filtrarPorCategoria = !catFiltro.Equals("Todos", StringComparison.OrdinalIgnoreCase);

            foreach (DataRow row in _dtProductos.Rows)
            {
                if (RowMatchesFilter(row, texto, catFiltro, filtrarPorCategoria))
                {
                    filtrada.ImportRow(row);
                }
            }
            return filtrada;
        }

        private bool RowMatchesFilter(DataRow row, string texto, string catFiltro, bool filtrarPorCategoria)
        {
            // 1) Filtro por texto (LIKE %texto% case-insensitive en product_id/product_name/tipo/category_id)
            bool textoMatch = true;
            if (!string.IsNullOrWhiteSpace(texto))
            {
                string textoLower = texto.ToLowerInvariant();
                string id = GetStringSafe(row, "product_id").ToLowerInvariant();
                string nombre = GetStringSafe(row, "product_name").ToLowerInvariant();
                string tipo = GetStringSafe(row, "tipo").ToLowerInvariant();
                string catId = GetStringSafe(row, "category_id").ToLowerInvariant();

                textoMatch = id.Contains(textoLower, StringComparison.Ordinal)
                    || nombre.Contains(textoLower, StringComparison.Ordinal)
                    || tipo.Contains(textoLower, StringComparison.Ordinal)
                    || catId.Contains(textoLower, StringComparison.Ordinal);

                // Fallback: si tipo/category_id están vacíos, también buscar en categoría resuelta (Master etc)
                if (!textoMatch)
                {
                    string catResuelta = ObtenerCategoria(row).ToLowerInvariant();
                    textoMatch = catResuelta.Contains(textoLower, StringComparison.Ordinal);
                }
            }

            if (!textoMatch)
            {
                return false;
            }

            // 2) Filtro por categoría seleccionada
            if (!filtrarPorCategoria)
            {
                return true;
            }

            // Soporte "Anulados" si algún día se añade al combo
            if (catFiltro.Equals("Anulados", StringComparison.OrdinalIgnoreCase) || catFiltro.Equals("Anulado", StringComparison.OrdinalIgnoreCase))
            {
                return EsAnulado(row);
            }

            string categoriaRow = ObtenerCategoria(row);
            // Si la fila no tiene categoría y se filtra por una específica, no coincide
            if (string.IsNullOrWhiteSpace(categoriaRow))
            {
                return false;
            }

            return categoriaRow.Equals(catFiltro, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Renderiza las cards en flpProductos a partir de la DataTable filtrada.
        /// Usa SuspendLayout/ResumeLayout para performance.
        /// Si hay filas, selecciona por defecto el primero o el id indicado (para post-edición).
        /// </summary>
        /// <param name="dtFiltrada">Tabla a renderizar.</param>
        /// <param name="seleccionarId">Id a seleccionar tras render; si null selecciona el primero.</param>
        private void RenderCards(DataTable dtFiltrada, string? seleccionarId = null)
        {
            FlowLayoutPanel? flp = null;
            try { flp = flpProductos; } catch { flp = null; }
            flp ??= Controls.Find("flpProductos", true).FirstOrDefault() as FlowLayoutPanel;
            if (flp == null)
            {
                return;
            }

            ProductCard? cardASeleccionar = null;
            try
            {
                flp.SuspendLayout();
                flp.Controls.Clear();
                _cardSeleccionada = null;
                LimpiarDetalleProducto();
                ActualizarEstadoToolbar();

                if (dtFiltrada == null || dtFiltrada.Rows.Count == 0)
                {
                    flp.ResumeLayout(true);
                    ActualizarEstadoToolbar();
                    return;
                }

                foreach (DataRow row in dtFiltrada.Rows)
                {
                    string id = GetStringSafe(row, "product_id");
                    string nombre = GetStringSafe(row, "product_name");
                    // Si product_name vacío, fallback a product_descrip o similar
                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        nombre = GetStringSafe(row, "product_descrip");
                    }

                    if (string.IsNullOrWhiteSpace(nombre))
                    {
                        nombre = GetStringSafe(row, "Product_Name");
                    }

                    string categoria = ObtenerCategoria(row);
                    bool anulado = EsAnulado(row);
                    string existencia = "Exist: --"; // placeholder, luego vendrá de inventario

                    ProductCard card = new ProductCard();
                    card.SetData(id, nombre, categoria, existencia, anulado);
                    card.Margin = new Padding(4, 4, 4, 4);
                    int anchoCard = flp.ClientSize.Width - flp.Padding.Horizontal - card.Margin.Horizontal - 6;
                    if (anchoCard < 300)
                    {
                        anchoCard = 300;
                    }

                    if (anchoCard > 360)
                    {
                        anchoCard = 360;
                    }

                    card.Width = anchoCard;
                    card.Tag = row;
                    // Suscribir Click para selección (deseleccionar previas, IsSelected=true)
                    ProductCard localCard = card;
                    localCard.Click += (s, e) => OnCardSelected(localCard);
                    flp.Controls.Add(localCard);

                    // Determina candidato a selección: prioriza seleccionarId, sino primer card
                    if (cardASeleccionar == null)
                    {
                        // primer card como fallback
                        cardASeleccionar = localCard;
                    }

                    if (!string.IsNullOrWhiteSpace(seleccionarId) && string.Equals(id, seleccionarId.Trim(), StringComparison.OrdinalIgnoreCase))
                    {
                        cardASeleccionar = localCard;
                    }
                }
                flp.ResumeLayout(true);
                flp.PerformLayout();

                // Selección por defecto: primer card o el id indicado (post-edición)
                if (cardASeleccionar != null)
                {
                    try
                    {
                        OnCardSelected(cardASeleccionar);
                        // Asegura visibilidad del seleccionado en el scroll
                        flp.ScrollControlIntoView(cardASeleccionar);
                    }
                    catch { /* no crítico */ }
                }

                ActualizarEstadoToolbar();
            }
            catch (Exception ex)
            {
                try { flp.ResumeLayout(true); } catch { }
                ServiceErrors.Report("Error al renderizar catálogo: " + ex.Message);
            }
            finally
            {
                ActualizarEstadoToolbar();
            }
        }

        private void OnCardSelected(ProductCard selected)
        {
            if (selected == null)
            {
                return;
            }

            FlowLayoutPanel? flp = null;
            try { flp = flpProductos; } catch { flp = null; }
            flp ??= Controls.Find("flpProductos", true).FirstOrDefault() as FlowLayoutPanel;
            if (flp != null)
            {
                foreach (ProductCard c in flp.Controls.OfType<ProductCard>())
                {
                    c.IsSelected = false;
                }
            }
            selected.IsSelected = true;
            _cardSeleccionada = selected;

            // Mostrar detalle en pestaña General del panel derecho
            if (selected.Tag is DataRow row)
            {
                MostrarDetalleProducto(row);
            }
            else
            {
                // Fallback por si Tag no es DataRow (ej. datos sintéticos)
                DataRow? fallback = _dtProductos.Rows.Cast<DataRow>().FirstOrDefault(r => GetStringSafe(r, "product_id") == selected.ProductId);
                if (fallback != null)
                {
                    MostrarDetalleProducto(fallback);
                }
            }

            ActualizarEstadoToolbar();
        }

        /// <summary>
        /// Muestra los detalles del producto en la pestaña General del panel derecho.
        /// </summary>
        /// <param name="row">Fila del producto.</param>
        private void MostrarDetalleProducto(DataRow row)
        {
            if (row == null)
            {
                return;
            }

            string id = GetStringSafe(row, "product_id");
            string nombre = GetStringSafe(row, "product_name");
            if (string.IsNullOrWhiteSpace(nombre))
            {
                nombre = GetStringSafe(row, "product_descrip");
            }

            string descrip = GetStringSafe(row, "product_descrip");
            string refer = GetStringSafe(row, "product_ref");
            // Algunas instalaciones usan 'product_ref' o 'code_rc' para referencia
            if (string.IsNullOrWhiteSpace(refer))
            {
                refer = GetStringSafe(row, "code_rc");
            }

            string codebar = GetStringSafe(row, "codebar");
            string categoria = ObtenerCategoria(row);
            bool anulado = EsAnulado(row);

            string precio = FormatearDecimalParaDetalle(GetDecimalSafe(row, "precio"));
            string ratio = FormatearDecimalParaDetalle(GetDecimalSafe(row, "ratio"));

            // Asignar a controles SunnyUI del panel derecho (Find defensivo por si Designer aún no los tiene)
            try
            {
                Sunny.UI.UITextBox? tId = txtDetId ?? Controls.Find("txtDetId", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tNombre = txtDetNombre ?? Controls.Find("txtDetNombre", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tDescrip = txtDetDescrip ?? Controls.Find("txtDetDescrip", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRef = txtDetRef ?? Controls.Find("txtDetRef", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tBar = txtDetCodebar ?? Controls.Find("txtDetCodebar", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tCat = txtDetCategoria ?? Controls.Find("txtDetCategoria", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tPrecio = txtDetPrecio ?? Controls.Find("txtDetPrecio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRatio = txtDetRatio ?? Controls.Find("txtDetRatio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UICheckBox? chk = chkDetAnulado ?? Controls.Find("chkDetAnulado", true).FirstOrDefault() as Sunny.UI.UICheckBox;

                if (tId != null)
                {
                    tId.Text = id;
                }

                if (tNombre != null)
                {
                    tNombre.Text = nombre;
                }

                if (tDescrip != null)
                {
                    tDescrip.Text = descrip;
                }

                if (tRef != null)
                {
                    tRef.Text = refer;
                }

                if (tBar != null)
                {
                    tBar.Text = codebar;
                }

                if (tCat != null)
                {
                    tCat.Text = categoria;
                }

                if (tPrecio != null)
                {
                    tPrecio.Text = precio;
                }

                if (tRatio != null)
                {
                    tRatio.Text = ratio;
                }

                if (chk != null)
                {
                    chk.Checked = anulado;
                }

                // Marcar RadioButton de categoría
                bool isMaster = GetBoolSafe(row, "masterRolls") || GetBoolSafe(row, "MasterRolls");
                bool isRollo = GetBoolSafe(row, "rollo_cortado") || GetBoolSafe(row, "rolloCortado");
                bool isGraphics = GetBoolSafe(row, "graphics") || GetBoolSafe(row, "Graphics");
                bool isHojas = GetBoolSafe(row, "resmas") || GetBoolSafe(row, "Resmas");

                RadioButton? rMaster = null;
                RadioButton? rRollo = null;
                RadioButton? rGraphics = null;
                RadioButton? rHojas = null;
                try { rMaster = rdoDetMaster; } catch { rMaster = null; }
                try { rRollo = rdoDetRollo; } catch { rRollo = null; }
                try { rGraphics = rdoDetGraphics; } catch { rGraphics = null; }
                try { rHojas = rdoDetHojas; } catch { rHojas = null; }
                rMaster ??= Controls.Find("rdoDetMaster", true).FirstOrDefault() as RadioButton;
                rRollo ??= Controls.Find("rdoDetRollo", true).FirstOrDefault() as RadioButton;
                rGraphics ??= Controls.Find("rdoDetGraphics", true).FirstOrDefault() as RadioButton;
                rHojas ??= Controls.Find("rdoDetHojas", true).FirstOrDefault() as RadioButton;

                if (rMaster != null) { rMaster.Checked = isMaster; }
                if (rRollo != null) { rRollo.Checked = isRollo; }
                if (rGraphics != null) { rGraphics.Checked = isGraphics; }
                if (rHojas != null) { rHojas.Checked = isHojas; }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al mostrar detalle: " + ex.Message);
            }
        }

        private static decimal GetDecimalSafe(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column))
            {
                return 0m;
            }

            object? v = row[column];
            if (v == null || v == DBNull.Value)
            {
                return 0m;
            }

            if (v is decimal d)
            {
                return d;
            }

            if (v is double db)
            {
                return (decimal)db;
            }

            if (v is float f)
            {
                return (decimal)f;
            }

            if (v is int i)
            {
                return i;
            }

            if (v is long l)
            {
                return l;
            }

            string s = v.ToString() ?? string.Empty;
            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal r1))
            {
                return r1;
            }

            if (decimal.TryParse(s, NumberStyles.Any, new CultureInfo("es-ES"), out decimal r2))
            {
                return r2;
            }

            return 0m;
        }

        private static bool GetBoolSafe(DataRow row, string column)
        {
            if (row == null || row.Table == null || !row.Table.Columns.Contains(column))
            {
                return false;
            }

            object? v = row[column];
            if (v == null || v == DBNull.Value)
            {
                return false;
            }

            if (v is bool b)
            {
                return b;
            }

            string s = v.ToString() ?? string.Empty;
            if (bool.TryParse(s, out bool r1))
            {
                return r1;
            }

            return s is "1" or "true" or "TRUE" or "True";
        }

        private static string FormatearDecimalParaDetalle(decimal value)
        {
            return value.ToString("N2", CultureInfo.InvariantCulture);
        }

        private void LimpiarDetalleProducto()
        {
            try
            {
                Sunny.UI.UITextBox? tId = null;
                try { tId = txtDetId; } catch { tId = null; }
                tId ??= Controls.Find("txtDetId", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tNombre = null;
                try { tNombre = txtDetNombre; } catch { tNombre = null; }
                tNombre ??= Controls.Find("txtDetNombre", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tDescrip = null;
                try { tDescrip = txtDetDescrip; } catch { tDescrip = null; }
                tDescrip ??= Controls.Find("txtDetDescrip", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRef = null;
                try { tRef = txtDetRef; } catch { tRef = null; }
                tRef ??= Controls.Find("txtDetRef", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tBar = null;
                try { tBar = txtDetCodebar; } catch { tBar = null; }
                tBar ??= Controls.Find("txtDetCodebar", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tCat = null;
                try { tCat = txtDetCategoria; } catch { tCat = null; }
                tCat ??= Controls.Find("txtDetCategoria", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tPrecio = null;
                try { tPrecio = txtDetPrecio; } catch { tPrecio = null; }
                tPrecio ??= Controls.Find("txtDetPrecio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UITextBox? tRatio = null;
                try { tRatio = txtDetRatio; } catch { tRatio = null; }
                tRatio ??= Controls.Find("txtDetRatio", true).FirstOrDefault() as Sunny.UI.UITextBox;
                Sunny.UI.UICheckBox? chk = null;
                try { chk = chkDetAnulado; } catch { chk = null; }
                chk ??= Controls.Find("chkDetAnulado", true).FirstOrDefault() as Sunny.UI.UICheckBox;

                if (tId != null)
                {
                    tId.Text = string.Empty;
                }

                if (tNombre != null)
                {
                    tNombre.Text = string.Empty;
                }

                if (tDescrip != null)
                {
                    tDescrip.Text = string.Empty;
                }

                if (tRef != null)
                {
                    tRef.Text = string.Empty;
                }

                if (tBar != null)
                {
                    tBar.Text = string.Empty;
                }

                if (tCat != null)
                {
                    tCat.Text = string.Empty;
                }

                if (tPrecio != null)
                {
                    tPrecio.Text = string.Empty;
                }

                if (tRatio != null)
                {
                    tRatio.Text = string.Empty;
                }

                if (chk != null)
                {
                    chk.Checked = false;
                }
            }
            catch
            {
                // No crítico
            }
            finally
            {
                ActualizarEstadoToolbar();
            }
        }

        /// <summary>
        /// Actualiza lblTotal (<see cref="Sunny.UI.UILabel"/>) con "Total: filtrados / total productos" o "Total: N productos" o "Sin productos".
        /// </summary>
        /// <param name="filtrados">Cantidad de productos filtrados.</param>
        /// <param name="total">Cantidad total de productos.</param>
        private void ActualizarSummary(int filtrados, int total)
        {
            Sunny.UI.UILabel? lbl = null;
            try { lbl = lblTotal; } catch { lbl = null; }
            lbl ??= Controls.Find("lblTotal", true).FirstOrDefault() as Sunny.UI.UILabel;
            if (lbl == null)
            {
                return;
            }

            if (total == 0)
            {
                lbl.Text = "Sin productos";
            }
            else if (filtrados == total)
            {
                lbl.Text = $"Total: {total} productos";
            }
            else
            {
                lbl.Text = $"Total: {filtrados} / {total} productos";
            }
        }

        private static string GetStringSafe(DataRow row, string column)
        {
            if (row == null || row.Table == null)
            {
                return string.Empty;
            }

            if (!row.Table.Columns.Contains(column))
            {
                return string.Empty;
            }

            object? v = row[column];
            if (v == null || v == DBNull.Value)
            {
                return string.Empty;
            }

            return v.ToString() ?? string.Empty;
        }

        private static bool EsAnulado(DataRow row)
        {
            if (row == null || row.Table == null)
            {
                return false;
            }

            if (!row.Table.Columns.Contains("anulado"))
            {
                return false;
            }

            object? v = row["anulado"];
            if (v == null || v == DBNull.Value)
            {
                return false;
            }

            if (v is bool b)
            {
                return b;
            }

            if (v is byte by)
            {
                return by != 0;
            }

            if (v is short sh)
            {
                return sh != 0;
            }

            if (v is int i)
            {
                return i != 0;
            }

            if (v is long l)
            {
                return l != 0;
            }

            string s = v.ToString() ?? string.Empty;
            if (bool.TryParse(s, out bool bb))
            {
                return bb;
            }

            if (int.TryParse(s, out int ii))
            {
                return ii != 0;
            }

            return false;
        }

        private static bool IsTruthy(object? v)
        {
            if (v == null || v == DBNull.Value)
            {
                return false;
            }

            if (v is bool b)
            {
                return b;
            }

            if (v is byte by)
            {
                return by != 0;
            }

            if (v is short sh)
            {
                return sh != 0;
            }

            if (v is int i)
            {
                return i != 0;
            }

            if (v is long l)
            {
                return l != 0;
            }

            if (v is decimal d)
            {
                return d != 0;
            }

            string s = v.ToString() ?? string.Empty;
            if (bool.TryParse(s, out bool bb))
            {
                return bb;
            }

            if (int.TryParse(s, out int ii))
            {
                return ii != 0;
            }

            if (decimal.TryParse(s, NumberStyles.Any, CultureInfo.InvariantCulture, out decimal dd))
            {
                return dd != 0;
            }

            return false;
        }

        //private void FinalizarConfiguracionUI()
        //{
        //    //gridDetalle.Columns.Clear();
        //    //gridDetalle.AutoGenerateColumns = false;
        //    CommonService.ADD_COLUMN_GRID("product_id", 120, "Código", "product_id", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("product_name", 220, "Producto", "product_name", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("categoria", 130, "Categoría", "categoria", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("unidad", 70, "Unidad", "unidad", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("cantidad", 80, "Cantidad", "cantidad", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("width", 80, "Ancho", "width", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("lenght", 80, "Largo", "lenght", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("msi", 100, "MSI", "msi", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("precio", 90, "Precio", "precio", gridDetalle);
        //    CommonService.ADD_COLUMN_GRID("total_renglon", 110, "Total", "total_renglon", gridDetalle);

        //    _dtDetalle = CrearTablaDetalle();
        //    RepoblarDetalleDesdeCatalogo();
        //    gridDetalle.DataSource = _dtDetalle;

        //    gridDetalle.Columns["cantidad"]!.DefaultCellStyle.Format = "N0";
        //    gridDetalle.Columns["width"]!.DefaultCellStyle.Format = "N2";
        //    gridDetalle.Columns["lenght"]!.DefaultCellStyle.Format = "N2";
        //    gridDetalle.Columns["msi"]!.DefaultCellStyle.Format = "N2";
        //    gridDetalle.Columns["precio"]!.DefaultCellStyle.Format = "N2";
        //    gridDetalle.Columns["total_renglon"]!.DefaultCellStyle.Format = "N2";
        //}

        private static DataTable CrearTablaDetalle()
        {
            DataTable dt = new();
            dt.Columns.Add("product_id", typeof(string));
            dt.Columns.Add("product_name", typeof(string));
            dt.Columns.Add("categoria", typeof(string));
            dt.Columns.Add("unidad", typeof(string));
            dt.Columns.Add("cantidad", typeof(decimal));
            dt.Columns.Add("width", typeof(decimal));
            dt.Columns.Add("lenght", typeof(decimal));
            dt.Columns.Add("msi", typeof(decimal));
            dt.Columns.Add("precio", typeof(decimal));
            dt.Columns.Add("total_renglon", typeof(decimal));
            return dt;
        }

        private void RepoblarDetalleDesdeCatalogo()
        {
            _dtDetalle.Rows.Clear();
            //if (_dtProductos == null) return;

            //foreach (DataRow prod in _dtProductos.Rows)
            //{
            //    //var row = _dtDetalle.NewRow();
            //    //row["product_id"] = prod["product_id"]?.ToString() ?? string.Empty;
            //    //row["product_name"] = prod["product_name"]?.ToString() ?? string.Empty;
            //    //row["categoria"] = ObtenerCategoria(prod);
            //    //row["unidad"] = "ROLLO";
            //    //row["cantidad"] = 1m;
            //    //row["width"] = 0m;
            //    //row["lenght"] = 0m;
            //    //row["msi"] = 0m;
            //    //row["precio"] = ObtenerDecimal(prod, "precio");
            //    //row["total_renglon"] = Convert.ToDecimal(row["precio"]);
            //    //_dtDetalle.Rows.Add(row);
            //}
        }

        private static string ObtenerCategoria(DataRow row)
        {
            if (row == null || row.Table == null)
            {
                return string.Empty;
            }
            // 1) columna tipo (CASE WHEN MasterRolls=1 then 'Master' ...)
            if (row.Table.Columns.Contains("tipo"))
            {
                string tipo = row["tipo"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(tipo))
                {
                    return tipo.Trim();
                }
            }
            // 2) category_id si trae texto
            if (row.Table.Columns.Contains("category_id"))
            {
                string cat = row["category_id"]?.ToString() ?? string.Empty;
                if (!string.IsNullOrWhiteSpace(cat))
                {
                    // Si es numérico puro, no asumir categoría textual
                    string trimmed = cat.Trim();
                    // Si contiene letras, devolverlo; si es solo dígito, seguir a bits
                    bool hasLetter = trimmed.Any(char.IsLetter);
                    if (hasLetter)
                    {
                        return trimmed;
                    }
                    // Si category_id codifica categoría (p.ej. "Master"), ya se devolvió
                }
            }
            // 3) bits MasterRolls / rollo_cortado / Resmas / Graphics (robusto a mayúsculas)
            // Master
            if (row.Table.Columns.Contains("MasterRolls") && IsTruthy(row["MasterRolls"]))
            {
                return "Master";
            }

            if (row.Table.Columns.Contains("masterRolls") && IsTruthy(row["masterRolls"]))
            {
                return "Master";
            }

            if (row.Table.Columns.Contains("Master") && IsTruthy(row["Master"]))
            {
                return "Master";
            }

            if (row.Table.Columns.Contains("master") && IsTruthy(row["master"]))
            {
                return "Master";
            }
            // Rollo Cortado
            if (row.Table.Columns.Contains("rollo_cortado") && IsTruthy(row["rollo_cortado"]))
            {
                return "Rollo Cortado";
            }

            if (row.Table.Columns.Contains("Rollo_Cortado") && IsTruthy(row["Rollo_Cortado"]))
            {
                return "Rollo Cortado";
            }

            if (row.Table.Columns.Contains("rolloCortado") && IsTruthy(row["rolloCortado"]))
            {
                return "Rollo Cortado";
            }
            // Resma
            if (row.Table.Columns.Contains("Resmas") && IsTruthy(row["Resmas"]))
            {
                return "Resma";
            }

            if (row.Table.Columns.Contains("resmas") && IsTruthy(row["resmas"]))
            {
                return "Resma";
            }

            if (row.Table.Columns.Contains("Resma") && IsTruthy(row["Resma"]))
            {
                return "Resma";
            }
            // Graphics
            if (row.Table.Columns.Contains("Graphics") && IsTruthy(row["Graphics"]))
            {
                return "Graphics";
            }

            if (row.Table.Columns.Contains("graphics") && IsTruthy(row["graphics"]))
            {
                return "Graphics";
            }

            return string.Empty;
        }

        //private static decimal ObtenerDecimal(DataRow row, string column)
        //{
        //    //if (!row.Table.Columns.Contains(column)) return 0m;
        //    //object valor = row[column];
        //    //if (valor == null || valor == DBNull.Value) return 0m;
        //    //if (decimal.TryParse(valor.ToString(), NumberStyles.Number, CultureInfo.InvariantCulture, out var result))
        //    //    return result;
        //    //return 0m;
        //}

        //private void DeshabilitarCaptura()
        //{
        //    _capturando = false;
        //    _editandoNuevo = false;
        //    _esperandoPrimerAdd = false;
        //    //panelFranja.Enabled = false;
        //    btnAdd.Enabled = false;
        //    tsbGuardar.Enabled = false;
        //    tsbCancelar.Enabled = false;
        //    tsbAnular.Enabled = false;
        //}

        //private void HabilitarCapturaNuevo()
        //{
        //    _capturando = true;
        //    _editandoNuevo = true;
        //    //panelFranja.Enabled = true;
        //    btnAdd.Enabled = true;
        //    tsbGuardar.Enabled = true;
        //    tsbCancelar.Enabled = true;
        //    tsbAnular.Enabled = false;
        //}

        //private void LimpiarBand()
        //{
        //    //txtBuscar.Clear();
        //    //txtProductId.Clear();
        //    //txtNombre.Clear();
        //    //txtUnidad.Text = "ROLLO";
        //    //txtCantidad.Clear();
        //    //txtWidth.Clear();
        //    //txtLenght.Clear();
        //    // txtMsi.Clear();
        //    //txtPrecio.Clear();
        //    //txtTotal.Clear();
        //    //cboCategoria.SelectedIndex = -1;
        //}

        //private void BandTieneContenido()
        //{
        //    //return !string.IsNullOrWhiteSpace(txtProductId.Text) || !string.IsNullOrWhiteSpace(txtNombre.Text);
        //}

        private void BtnNuevo_Click(object? sender, EventArgs e)
        {
            //if (_editandoNuevo && BandTieneContenido())
            //{
            //    var respuesta = MessageBox.Show(
            //        "Existe una captura sin guardar. ¿Desea descartarla y comenzar una nueva?",
            //        "Aviso", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //    if (respuesta != DialogResult.Yes) return;
            //}

            //LimpiarBand();
            //gridDetalle.ClearSelection();
            //_esperandoPrimerAdd = true;
            //HabilitarCapturaNuevo();
            //txtProductId.Focus();
        }

        private void BtnBuscar_Click(object? sender, EventArgs e)
        {
            Frm_ProductSeach buscador = new()
            {
                DtItems = _dtProductos.Copy()
            };
            buscador.ShowDialog();

            if (string.IsNullOrEmpty(buscador.Selected_ProductID))
            {
                return;
            }

            DataRow[] filas = _dtProductos.Select($"product_id = '{EscapeLike(buscador.Selected_ProductID)}'");
            if (filas.Length == 0)
            {
                return;
            }

            //DataRow prod = filas[0];
            //txtBuscar.Text = prod["product_name"]?.ToString() ?? string.Empty;
            //txtProductId.Text = prod["product_id"]?.ToString() ?? string.Empty;
            //txtNombre.Text = prod["product_name"]?.ToString() ?? string.Empty;
            //SetCategoriaCombo(ObtenerCategoria(prod));
            //txtPrecio.Text = ObtenerDecimal(prod, "precio").ToString("N2", CultureInfo.InvariantCulture);
            //txtUnidad.Text = "ROLLO";
        }

        //private void SetCategoriaCombo(string categoria)
        //{
        //    int idx = cboCategoria.Items.IndexOf(categoria);
        //    cboCategoria.SelectedIndex = idx >= 0 ? idx : -1;
        //}

        private void BtnAdd_Click(object? sender, EventArgs e)
        {
            //if (!_capturando)
            //{
            //    return;
            //}

            //if (string.IsNullOrWhiteSpace(txtProductId.Text))
            //{
            //    MessageBox.Show("Debe ingresar el Product ID.", "Validación",
            //        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //if (string.IsNullOrWhiteSpace(txtNombre.Text))
            //{
            //    MessageBox.Show("Debe ingresar el nombre del producto.", "Validación",
            //        MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //    return;
            //}
            //decimal cantidad = ParseDecimal(txtCantidad.Text);
            //if (cantidad <= 0)
            //{
            //MessageBox.Show("La cantidad debe ser mayor que cero.", "Validación",
            //  MessageBoxButtons.OK, MessageBoxIcon.Warning);
            //return;
            //}

            //string productoId = txtProductId.Text.Trim();
            //decimal width = ParseDecimal(txtWidth.Text);
            //decimal lenght = ParseDecimal(txtLenght.Text);
            //decimal precio = ParseDecimal(txtPrecio.Text);
            //decimal msi = width * lenght * cantidad;
            //decimal total = cantidad * precio;
            //string categoria = cboCategoria.Text;
            //string unidad = string.IsNullOrWhiteSpace(txtUnidad.Text) ? "ROLLO" : txtUnidad.Text.Trim();

            //var fila = BuscarFilaPorProductId(productoId);
            //if (fila != null)
            //{
            //    //fila["product_name"] = txtNombre.Text.Trim();
            //    //fila["categoria"] = categoria;
            //    //fila["unidad"] = unidad;
            //    //fila["cantidad"] = cantidad;
            //    //fila["width"] = width;
            //    //fila["lenght"] = lenght;
            //    //fila["msi"] = msi;
            //    //fila["precio"] = precio;
            //    //fila["total_renglon"] = total;
            //}
            //else
            //{
            //    //var row = _dtDetalle.NewRow();
            //    //row["product_id"] = productoId;
            //    //row["product_name"] = txtNombre.Text.Trim();
            //    //row["categoria"] = categoria;
            //    //row["unidad"] = unidad;
            //    //row["cantidad"] = cantidad;
            //    //row["width"] = width;
            //    //row["lenght"] = lenght;
            //    //row["msi"] = msi;
            //    //row["precio"] = precio;
            //    //row["total_renglon"] = total;
            //    //_dtDetalle.Rows.Add(row);
            //}

            //_esperandoPrimerAdd = false;
            //txtCantidad.Clear();
            //txtWidth.Clear();
            //txtLenght.Clear();
            //txtPrecio.Clear();
            //txtMsi.Clear();
            //txtTotal.Clear();
            //txtCantidad.Focus();
        }

        private DataRow? BuscarFilaPorProductId(string productId)
        {
            foreach (DataRow row in _dtDetalle.Rows)
            {
                if (string.Equals(row["product_id"]?.ToString(), productId, StringComparison.OrdinalIgnoreCase))
                {
                    return row;
                }
            }
            return null;
        }

        //private void GridDetalle_SelectionChanged(object? sender, EventArgs e)
        //{
        //    var current = gridDetalle.CurrentRow;
        //    if (current == null || current.Index < 0)
        //    {
        //        tsbAnular.Enabled = false;
        //        return;
        //    }

        //    object? valor = current.Cells["product_id"].Value;
        //    if (valor == null || valor == DBNull.Value || string.IsNullOrWhiteSpace(valor.ToString()))
        //    {
        //        tsbAnular.Enabled = false;
        //        return;
        //    }

        //    string productId = valor.ToString()!;
        //    bool existeEnBD = ProductoExisteEnCatalogo(productId);

        //    if (!(_editandoNuevo && _esperandoPrimerAdd))
        //    {
        //        CargarRenglonABand(current);
        //    }

        //    //txtProductId.ReadOnly = existeEnBD;
        //    tsbAnular.Enabled = existeEnBD;

        //    if (existeEnBD)
        //    {
        //        _capturando = true;
        //        //panelFranja.Enabled = true;
        //        btnAdd.Enabled = true;
        //        tsbGuardar.Enabled = true;
        //        tsbCancelar.Enabled = true;
        //    }
        //}

        private bool ProductoExisteEnCatalogo(string productId)
        {
            return _dtProductos != null && _dtProductos.Columns.Contains("product_id") && _dtProductos.Select($"product_id = '{EscapeLike(productId)}'").Length > 0;
        }

        private static string EscapeLike(string value)
        {
            return value.Replace("'", "''");
        }

        //private void CargarRenglonABand(DataGridViewRow row)
        //{
        //    //string categoria = row.Cells["categoria"].Value?.ToString() ?? string.Empty;
        //    //string nombre = row.Cells["product_name"].Value?.ToString() ?? string.Empty;
        //    //txtBuscar.Text = nombre;
        //    //txtProductId.Text = row.Cells["product_id"].Value?.ToString() ?? string.Empty;
        //    //txtNombre.Text = nombre;
        //    //txtUnidad.Text = string.IsNullOrWhiteSpace(row.Cells["unidad"].Value?.ToString())
        //    //    ? "ROLLO"
        //    //    : row.Cells["unidad"].Value!.ToString();
        //    //txtCantidad.Text = FormatearDecimal(row.Cells["cantidad"].Value);
        //    //txtWidth.Text = FormatearDecimal(row.Cells["width"].Value);
        //    //txtLenght.Text = FormatearDecimal(row.Cells["lenght"].Value);
        //    //txtMsi.Text = FormatearDecimal(row.Cells["msi"].Value);
        //    //txtPrecio.Text = FormatearDecimal(row.Cells["precio"].Value);
        //    //txtTotal.Text = FormatearDecimal(row.Cells["total_renglon"].Value);
        //    //SetCategoriaCombo(categoria);
        //}

        private static string FormatearDecimal(object? value)
        {
            if (value == null || value == DBNull.Value)
            {
                return string.Empty;
            }

            return Convert.ToDecimal(value).ToString("N2", CultureInfo.InvariantCulture);
        }

        private void TxtMedida_TextChanged(object? sender, EventArgs e)
        {
            //decimal cantidad = ParseDecimal(txtCantidad.Text);
            //decimal width = ParseDecimal(txtWidth.Text);
            //decimal lenght = ParseDecimal(txtLenght.Text);
            //decimal precio = ParseDecimal(txtPrecio.Text);

            //txtMsi.Text = (width * lenght * cantidad).ToString("N2", CultureInfo.InvariantCulture);
            //txtTotal.Text = (cantidad * precio).ToString("N2", CultureInfo.InvariantCulture);
        }

        private static decimal ParseDecimal(string value)
        {
            return decimal.TryParse(value, NumberStyles.Number, CultureInfo.InvariantCulture, out decimal result) ? result : 0m;
        }

        private async void BtnGuardar_Click(object? sender, EventArgs e)
        {
            //if (!_capturando)
            //{
            //    MessageBox.Show("Presione 'Nuevo' o seleccione un producto para capturar.",
            //        "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            if (_dtDetalle.Rows.Count == 0)
            {
                MessageBox.Show("Debe agregar al menos un producto en el detalle.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            List<ProductoFila> items = [];
            foreach (DataRow row in _dtDetalle.Rows)
            {
                string productId = row["product_id"]?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(productId))
                {
                    continue;
                }

                items.Add(new ProductoFila
                {
                    ProductId = productId,
                    Nombre = row["product_name"]?.ToString() ?? string.Empty,
                    Categoria = row["categoria"]?.ToString() ?? string.Empty,
                    Unidad = row["unidad"]?.ToString() ?? string.Empty,
                    Cantidad = Convert.ToDecimal(row["cantidad"]),
                    Width = Convert.ToDecimal(row["width"]),
                    Lenght = Convert.ToDecimal(row["lenght"]),
                    Msi = Convert.ToDecimal(row["msi"]),
                    Precio = Convert.ToDecimal(row["precio"])
                });
            }

            //tsbGuardar.Enabled = false;
            using FrmLoading loading = new("Guardando productos...");
            loading.Show(this);
            loading.BringToFront();
            //try
            //{
            //    string error = await Task.Run(() => GuardarProductos(items));
            //    if (string.IsNullOrEmpty(error))
            //    {
            //        MessageBox.Show("Productos guardados correctamente.", "Éxito",
            //            MessageBoxButtons.OK, MessageBoxIcon.Information);
            //        await RecargarCatalogoAsync();
            //        RepoblarDetalleDesdeCatalogo();
            //        LimpiarBand();
            //        DeshabilitarCaptura();
            //    }
            //    else
            //    {
            //        MessageBox.Show("No se pudieron guardar los productos: " + error, "Error",
            //            MessageBoxButtons.OK, MessageBoxIcon.Error);
            //        if (_capturando) tsbGuardar.Enabled = true;
            //    }
            //}
            //finally
            //{
            //    if (!loading.IsDisposed) loading.Close();
            //}
        }

        private string GuardarProductos(List<ProductoFila> items)
        {
            try
            {
                using SqlConnection conn = new(_conexion);
                conn.Open();
                foreach (ProductoFila item in items)
                {
                    bool existe = ProductoExisteEnBD(item.ProductId, conn);
                    using SqlCommand cmd = new(existe ? SQL_UPDATE_PRODUCTO : SQL_INSERT_PRODUCTO, conn);
                    //AgregarParametrosProducto(cmd, item);
                    cmd.ExecuteNonQuery();
                }
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private static bool ProductoExisteEnBD(string productId, SqlConnection conn)
        {
            using SqlCommand cmd = new("SELECT COUNT(*) FROM producto WHERE Product_ID = @p1", conn);
            cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = productId });
            return Convert.ToInt32(cmd.ExecuteScalar()) > 0;
        }

        //private static void AgregarParametrosProducto(SqlCommand cmd, ProductoFila item)
        //{
        //    cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = item.ProductId });
        //    cmd.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = NuloSiVacio(item.Nombre) });
        //    cmd.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = NuloSiVacio(item.Categoria) });
        //    cmd.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = NuloSiVacio(item.Unidad) });
        //    cmd.Parameters.Add(new SqlParameter("@p5", SqlDbType.Decimal) { Value = item.Width });
        //    cmd.Parameters.Add(new SqlParameter("@p6", SqlDbType.Decimal) { Value = item.Lenght });
        //    cmd.Parameters.Add(new SqlParameter("@p7", SqlDbType.Decimal) { Value = item.Cantidad });
        //    cmd.Parameters.Add(new SqlParameter("@p8", SqlDbType.Decimal) { Value = item.Msi });
        //    cmd.Parameters.Add(new SqlParameter("@p9", SqlDbType.Decimal) { Value = item.Precio });
        //    cmd.Parameters.Add(new SqlParameter("@p10", SqlDbType.Bit) { Value = item.Categoria == "Master" });
        //    cmd.Parameters.Add(new SqlParameter("@p11", SqlDbType.Bit) { Value = item.Categoria == "Rollo Cortado" });
        //    cmd.Parameters.Add(new SqlParameter("@p12", SqlDbType.Bit) { Value = item.Categoria == "Resma" });
        //    cmd.Parameters.Add(new SqlParameter("@p13", SqlDbType.Bit) { Value = item.Categoria == "Graphics" });
        //}

        private static object NuloSiVacio(string value)
        {
            string trim = value?.Trim() ?? string.Empty;
            return trim.Length == 0 ? (object)DBNull.Value : trim;
        }

        private async Task RecargarCatalogoAsync()
        {
            try
            {
                DataSet ds = await _productsService.Load();
                DataTable? dt = ds.Tables["Dtproducts"] ?? ds.Tables["DtProducts"] ?? ds.Tables["DtProductos"] ?? (ds.Tables.Count > 0 ? ds.Tables[0] : null);
                _dtProductos = dt ?? new DataTable();
                // Tras recargar, refrescar catálogo visual
                FiltrarYRender();
            }
            catch
            {
                //_dtProductos = await CargarCatalogoDirecto();
            }
        }

        //private async Task<DataTable> CargarCatalogoDirecto()
        //{
        //    //var dt = new DataTable();
        //    //try
        //    //{
        //    //    using var conn = new SqlConnection(_conexion);
        //    //    using var cmd = new SqlCommand(R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS, conn);
        //    //    await conn.OpenAsync();
        //    //    using var da = new SqlDataAdapter(cmd);
        //    //    da.Fill(dt);
        //    //}
        //    //catch (Exception ex)
        //    //{
        //    //    ServiceErrors.Report("Error al recargar catálogo de productos: " + ex.Message);
        //    //}
        //    //return dt;
        //}

        private async void BtnAnular_Click(object? sender, EventArgs e)
        {
            //var current = gridDetalle.CurrentRow;
            //if (current == null || current.Index < 0)
            //{
            //    MessageBox.Show("Seleccione un producto para anular.", "Aviso",
            //        MessageBoxButtons.OK, MessageBoxIcon.Information);
            //    return;
            //}

            //object? valor = current.Cells["product_id"].Value;
            //if (valor == null || valor == DBNull.Value) return;
            //string productId = valor.ToString()!;
            //string nombre = current.Cells["product_name"].Value?.ToString() ?? productId;

            //var confirm = MessageBox.Show($"¿Anular el producto {nombre} ({productId})?", "Confirmar anulación",
            //    MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            //if (confirm != DialogResult.Yes) return;

            //tsbAnular.Enabled = false;
            using FrmLoading loading = new("Anulando producto...");
            loading.Show(this);
            loading.BringToFront();
            try
            {
                //string error = await Task.Run(() => AnularProducto(productId));
                //if (string.IsNullOrEmpty(error))
                //{
                //    MessageBox.Show("Producto anulado.", "Éxito",
                //        MessageBoxButtons.OK, MessageBoxIcon.Information);
                //    await RecargarCatalogoAsync();
                //    RepoblarDetalleDesdeCatalogo();
                //    LimpiarBand();
                //    //DeshabilitarCaptura();
                //}
                //else
                //{
                //    MessageBox.Show("No se pudo anular el producto: " + error, "Error",
                //        MessageBoxButtons.OK, MessageBoxIcon.Error);
                //    tsbAnular.Enabled = true;
                //}
            }
            finally
            {
                if (!loading.IsDisposed)
                {
                    loading.Close();
                }
            }
        }

        private string AnularProducto(string productId)
        {
            try
            {
                using SqlConnection conn = new(_conexion);
                using SqlCommand cmd = new(SQL_ANULAR_PRODUCTO, conn);
                cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = productId });
                conn.Open();
                cmd.ExecuteNonQuery();
                return string.Empty;
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            RepoblarDetalleDesdeCatalogo();
            //LimpiarBand();
            //DeshabilitarCaptura();
        }

        private sealed class ProductoFila
        {
            public string ProductId = string.Empty;
            public string Nombre = string.Empty;
            public string Categoria = string.Empty;
            public string Unidad = string.Empty;
            public decimal Cantidad;
            public decimal Width;
            public decimal Lenght;
            public decimal Msi;
            public decimal Precio;
        }

        private void FrmProductos_Load(object sender, EventArgs e)
        {
            // Fallback por si InitializeAsync no se llamó (p.ej. apertura directa sin FormManager)
            // o si el Designer fue actualizado después de la carga inicial.
            try
            {
                ConfigurarControlesCatalogo();
                WireCatalogEvents();
                WireToolbarEvents();
                WireNuevoProductoEvents();
                EnsureNuevoProductoTabOculta();
                // Si ya hay datos pero aún no se renderizó (flp vacío), renderiza
                FlowLayoutPanel? flp = null;
                try { flp = flpProductos; } catch { flp = null; }
                flp ??= Controls.Find("flpProductos", true).FirstOrDefault() as FlowLayoutPanel;
                if (flp != null && flp.Controls.Count == 0 && _dtProductos != null && _dtProductos.Rows.Count > 0)
                {
                    FiltrarYRender();
                }
                else if (_dtProductos != null && _dtProductos.Rows.Count == 0)
                {
                    ActualizarSummary(0, 0);
                }

                AplicarTemaCatalogo();
                ActualizarEstadoToolbar();
            }
            catch { /* defensivo */ }
        }

    }
}
