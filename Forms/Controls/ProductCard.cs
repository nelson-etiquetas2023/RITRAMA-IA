using System.ComponentModel;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ritrama2025.Forms.Controls;

/// <summary>
/// Card compacta para catálogo de productos (300x78).
/// Replica tema verde SunnyUI (110,190,40) + JetBrains Mono + borde 10px vía GraphicsPath
/// como FrmOrdenCorte:RedondearControl. Muestra product_id (chip gris), product_name (bold),
/// categoría (chip color) y existencia. Soporta estado Selected con borde/Back y estado Anulado.
/// </summary>
/// <remarks>
/// <para>
/// Se mantienen controles estándar <see cref="Panel"/> y <see cref="Label"/> en lugar de
/// <c>Sunny.UI.UIPanel</c>/<c>Sunny.UI.UILabel</c> para preservar el custom painting
/// con <see cref="System.Drawing.Drawing2D.GraphicsPath"/> y <see cref="Control.Region"/>.
/// </para>
/// <para>
/// <c>UIPanel</c> y <c>UILabel</c> aplican su propio estilo, relleno y manejo de bordes que
/// interfiere con el redondeo manual (Region + OnPaint) y el cambio dinámico de BackColor
/// por categoría/selección. Migrar a SunnyUI rompería el borde de 12px y los chips redondeados
/// sin aportar beneficio visual, ya que los colores se gestionan manualmente (VerdeMaster,
/// AzulRollo, etc.). Los contenedores de layout (Panel/FlowLayoutPanel) están permitidos por
/// la regla del proyecto; solo los controles visibles editables deben ser SunnyUI.
/// </para>
/// </remarks>
public partial class ProductCard : UserControl
{
    // Colores de categoría: R.cs:99 / Models/Product.cs + anulado
    private static readonly Color VerdeMaster = Color.FromArgb(110, 190, 40);
    private static readonly Color AzulRollo = Color.FromArgb(0, 120, 215);
    private static readonly Color NaranjaResma = Color.FromArgb(255, 140, 0);
    private static readonly Color VioletaGraphics = Color.FromArgb(128, 0, 128);
    private static readonly Color RojoAnulado = Color.FromArgb(220, 80, 60);
    private static readonly Color GrisChipBack = Color.FromArgb(240, 240, 240);
    private static readonly Color GrisChipFore = Color.FromArgb(80, 80, 80);
    private static readonly Color VerdeCardNormal = Color.FromArgb(214, 245, 214);
    private static readonly Color VerdeCardSelected = Color.FromArgb(110, 190, 40);
    private static readonly Color VerdeBordeNormal = Color.FromArgb(180, 210, 180);
    private static readonly Color VerdeBordeSelected = Color.FromArgb(70, 140, 25);

    private bool _isSelected;
    private bool _anulado;
    private string _productId = string.Empty;
    private string _nombre = string.Empty;
    private string _categoriaRaw = string.Empty;
    private string _existenciaText = "Exist: --";

    /// <summary>Product_ID del producto.</summary>
    [Category("ProductCard"), DefaultValue("")]
    public string ProductId
    {
        get => _productId;
        set
        {
            _productId = value ?? string.Empty;
            if (lblIdChip != null)
            {
                lblIdChip.Text = _productId;
            }
        }
    }

    /// <summary>Nombre del producto (product_name).</summary>
    [Category("ProductCard"), DefaultValue("")]
    public string Nombre
    {
        get => _nombre;
        set
        {
            _nombre = value ?? string.Empty;
            if (lblNombre != null)
            {
                lblNombre.Text = _nombre;
            }
        }
    }

    /// <summary>Categoría cruda (Master / Rollo Cortado / Resma / Graphics). Visual se resuelve vía Anulado.</summary>
    [Category("ProductCard"), DefaultValue("")]
    public string Categoria
    {
        get => _categoriaRaw;
        set
        {
            _categoriaRaw = value ?? string.Empty;
            ActualizarVisualCategoria();
        }
    }

    /// <summary>Texto de existencia (placeholder, ej "Exist: --").</summary>
    [Category("ProductCard"), DefaultValue("Exist: --")]
    public string ExistenciaText
    {
        get => _existenciaText;
        set
        {
            _existenciaText = string.IsNullOrWhiteSpace(value) ? "Exist: --" : value!;
            if (lblExistencia != null)
            {
                lblExistencia.Text = _existenciaText;
            }
        }
    }

    /// <summary>Indica si el producto está anulado (anulado=1). Fuerza chip "Anulado" rojo.</summary>
    [Category("ProductCard"), DefaultValue(false)]
    public bool Anulado
    {
        get => _anulado;
        set
        {
            _anulado = value;
            ActualizarVisualCategoria();
            ActualizarEstadoSeleccion();
        }
    }

    /// <summary>Estado de selección: cambia BackColor y borde (verde cuando está seleccionado).</summary>
    [Category("ProductCard"), DefaultValue(false)]
    public bool IsSelected
    {
        get => _isSelected;
        set
        {
            if (_isSelected == value)
            {
                return;
            }

            _isSelected = value;
            ActualizarEstadoSeleccion();
        }
    }

    // Alias para compatibilidad con consumidores que esperan "Selected"
    [Browsable(false), DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
    public bool Selected
    {
        get => IsSelected;
        set => IsSelected = value;
    }

    public ProductCard()
    {
        InitializeComponent();

        // Tema base: blanco + JetBrains Mono + tamaño 300x78 como pide la spec
        // Si la fuente no está instalada, WinForms cae a fuente por defecto sin romper build.
        try
        {
            Font = new Font("JetBrains Mono", 8.25F, FontStyle.Regular, GraphicsUnit.Point);
        }
        catch
        {
            // fallback silencioso
        }

        // Double buffer + UserPaint para borde redondeado sin flicker
        SetStyle(ControlStyles.AllPaintingInWmPaint
                 | ControlStyles.OptimizedDoubleBuffer
                 | ControlStyles.UserPaint
                 | ControlStyles.ResizeRedraw
                 | ControlStyles.SupportsTransparentBackColor, true);
        DoubleBuffered = true;

        Margin = new Padding(4, 4, 4, 4);
        Size = new Size(352, 84);
        BackColor = VerdeCardNormal;
        Cursor = Cursors.Hand;

        // Propagar clicks de todos los hijos hacia el UserControl (la card completa es clickeable)
        // Solo hijos: no adjuntar a 'this' para evitar recursión Click -> OnClick -> Click StackOverflow.
        foreach (Control c in GetAllControls(this))
        {
            AttachClickHandlers(c);
        }

        // Valores por defecto visuales
        if (lblExistencia != null && string.IsNullOrWhiteSpace(lblExistencia.Text))
        {
            lblExistencia.Text = "Exist: --";
        }

        ActualizarVisualCategoria();
        ActualizarEstadoSeleccion();

        // Redondeo inicial y al redimensionar
        HandleCreated += (_, _) => AplicarRedondeo();
        Resize += (_, _) => AplicarRedondeo();
        // Re-aplicar cuando cambia el tamaño del panel interno o chips
        if (pnlContent != null)
        {
            pnlContent.Resize += (_, _) => AplicarRedondeo();
        }
    }

    private static IEnumerable<Control> GetAllControls(Control parent)
    {
        foreach (Control c in parent.Controls)
        {
            yield return c;
            foreach (Control sub in GetAllControls(c))
            {
                yield return sub;
            }
        }
    }

    private void AttachClickHandlers(Control ctrl)
    {
        ctrl.Click += (_, e) => OnClick(e);
        // Para que el click en labels no sea "tragado" sin propagar MouseDown también
        ctrl.MouseDown += (_, e) =>
        {
            if (e.Button == MouseButtons.Left)
            {
                OnClick(EventArgs.Empty);
            }
        };
    }

    /// <summary>
    /// Asigna todos los datos de la card de una vez. Normaliza categoría y existencia.
    /// </summary>
    public void SetData(string id, string nombre, string categoria, string existencia, bool anulado)
    {
        _productId = id ?? string.Empty;
        _nombre = nombre ?? string.Empty;
        _categoriaRaw = categoria ?? string.Empty;
        _anulado = anulado;
        _existenciaText = string.IsNullOrWhiteSpace(existencia) ? "Exist: --" : existencia!;

        if (lblIdChip != null)
        {
            lblIdChip.Text = _productId;
        }

        if (lblNombre != null)
        {
            lblNombre.Text = _nombre;
        }

        if (lblExistencia != null)
        {
            lblExistencia.Text = _existenciaText;
        }

        ActualizarVisualCategoria();
        ActualizarEstadoSeleccion();
        AplicarRedondeo();
        Invalidate();
    }

    private void ActualizarVisualCategoria()
    {
        if (lblCategoriaChip == null)
        {
            return;
        }

        string display;
        Color back;
        Color fore = Color.White;

        if (_anulado)
        {
            display = "Anulado";
            back = RojoAnulado;
        }
        else
        {
            string norm = (_categoriaRaw ?? string.Empty).Trim().ToLowerInvariant();
            // Normaliza variantes de BD: MasterRolls / rollo_cortado / resmas / Graphics
            // y las ya resueltas por R.cs SELECT: Master / Rollo Cortado / Resma / Graphics
            if (norm == "master" || norm == "masterrolls" || norm == "master_rolls" || norm == "masters")
            {
                display = "Master";
                back = VerdeMaster;
            }
            else if (norm == "rollo cortado" || norm == "rollo_cortado" || norm == "rollocortado" || norm == "rollocortado " || norm == "rollo_cortado\r" || norm == "rollo cortado\r")
            {
                display = "Rollo Cortado";
                back = AzulRollo;
            }
            else if (norm == "resma" || norm == "resmas")
            {
                display = "Resma";
                back = NaranjaResma;
            }
            else if (norm == "graphics")
            {
                display = "Graphics";
                back = VioletaGraphics;
            }
            else if (norm.Length == 0)
            {
                display = "—";
                back = Color.FromArgb(160, 160, 160);
            }
            else
            {
                // Categoría desconocida: muestra tal cual pero con gris
                display = _categoriaRaw!.Trim();
                back = Color.FromArgb(140, 140, 140);
            }
        }

        lblCategoriaChip.Text = display;
        lblCategoriaChip.BackColor = back;
        lblCategoriaChip.ForeColor = fore;
        // Chip redondeado 6px
        RedondearControl(lblCategoriaChip, 6);
    }

    private void ActualizarEstadoSeleccion()
    {
        // Verdecito normal (214,245,214), verde fuerte seleccionado (110,190,40) con texto blanco.
        Color back = _isSelected ? VerdeCardSelected : VerdeCardNormal;
        BackColor = back;
        if (pnlContent != null)
        {
            pnlContent.BackColor = back;
        }

        // Texto: oscuro sobre verdecito, blanco sobre verde fuerte.
        if (lblNombre != null)
        {
            lblNombre.ForeColor = _isSelected ? Color.White : Color.FromArgb(40, 40, 40);
        }

        if (lblExistencia != null)
        {
            lblExistencia.ForeColor = _isSelected ? Color.FromArgb(240, 255, 240) : Color.FromArgb(90, 120, 90);
        }
        // Chips mantienen su color pero el ID se invierte ligeramente en seleccionado para contraste.
        if (lblIdChip != null)
        {
            lblIdChip.BackColor = _isSelected ? Color.White : GrisChipBack;
            lblIdChip.ForeColor = _isSelected ? Color.FromArgb(60, 60, 60) : GrisChipFore;
        }

        Invalidate();
    }

    private void AplicarRedondeo()
    {
        // Radio 12px para que el redondeo sea claramente visible (antes 10px era sutil).
        RedondearControl(this, 12);
        if (pnlContent != null)
        {
            RedondearControl(pnlContent, 10);
        }

        if (lblIdChip != null)
        {
            RedondearControl(lblIdChip, 6);
        }

        if (lblCategoriaChip != null)
        {
            RedondearControl(lblCategoriaChip, 6);
        }

        // Fuerza repintado del borde
        Invalidate();
    }

    /// <summary>
    /// Replica exacta de FrmOrdenCorte.cs:286 RedondearControl (GraphicsPath con 4 arcos).
    /// </summary>
    private static void RedondearControl(Control ctrl, int radio)
    {
        if (ctrl == null)
        {
            return;
        }
        // Usar ClientRectangle pero guardando contra tamaños 0 que dejan Region vacía
        Rectangle rect = ctrl.ClientRectangle;
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }
        // Chips pequeños requieren clamp de radio
        int r = Math.Min(radio, Math.Min(rect.Width, rect.Height) / 2);
        if (r <= 0)
        {
            return;
        }

        GraphicsPath gp = new GraphicsPath();
        gp.StartFigure();
        gp.AddArc(0, 0, r, r, 180, 90);
        gp.AddArc(rect.Width - r, 0, r, r, 270, 90);
        gp.AddArc(rect.Width - r, rect.Height - r, r, r, 0, 90);
        gp.AddArc(0, rect.Height - r, r, r, 90, 90);
        gp.CloseFigure();

        // Evitar leak de Region anterior
        try { ctrl.Region?.Dispose(); } catch { /* ignore */ }
        ctrl.Region = new Region(gp);
        gp.Dispose();
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        // Borde redondeado: verde sólido cuando seleccionado, gris claro cuando no
        // Se pinta por encima del Region para que siempre sea visible
        e.Graphics.SmoothingMode = SmoothingMode.AntiAlias;

        Color borderColor = _isSelected ? VerdeBordeSelected : VerdeBordeNormal;
        int borderWidth = _isSelected ? 2 : 1;

        Rectangle rect = new Rectangle(0, 0, Width - 1, Height - 1);
        if (rect.Width <= 0 || rect.Height <= 0)
        {
            return;
        }

        int radius = 12;
        int r = Math.Min(radius, Math.Min(rect.Width, rect.Height) / 2);

        using GraphicsPath path = new GraphicsPath();
        path.StartFigure();
        path.AddArc(rect.X, rect.Y, r, r, 180, 90);
        path.AddArc(rect.Right - r, rect.Y, r, r, 270, 90);
        path.AddArc(rect.Right - r, rect.Bottom - r, r, r, 0, 90);
        path.AddArc(rect.X, rect.Bottom - r, r, r, 90, 90);
        path.CloseFigure();

        using Pen pen = new Pen(borderColor, borderWidth);
        // Ajuste para que el trazo quede dentro del rect (evita clip exterior)
        pen.Alignment = PenAlignment.Inset;
        e.Graphics.DrawPath(pen, path);

        // Si está anulado, opcional línea diagonal sutil o desaturación ya la da el chip rojo;
        // no se altera el Back para mantener legibilidad.
    }

    protected override void OnHandleCreated(EventArgs e)
    {
        base.OnHandleCreated(e);
        AplicarRedondeo();
    }

    // Expone helpers estáticos por si FrmProductos quiere mapear colores sin instanciar la card
    public static Color ColorPorCategoria(string? categoria, bool anulado)
    {
        if (anulado)
        {
            return RojoAnulado;
        }

        string norm = (categoria ?? string.Empty).Trim().ToLowerInvariant();
        return norm switch
        {
            "master" or "masterrolls" or "master_rolls" => VerdeMaster,
            "rollo cortado" or "rollo_cortado" or "rollocortado" => AzulRollo,
            "resma" or "resmas" => NaranjaResma,
            "graphics" => VioletaGraphics,
            _ => Color.FromArgb(160, 160, 160)
        };
    }

    public static string TextoPorCategoria(string? categoria, bool anulado)
    {
        if (anulado)
        {
            return "Anulado";
        }

        string norm = (categoria ?? string.Empty).Trim().ToLowerInvariant();
        return norm switch
        {
            "master" or "masterrolls" or "master_rolls" => "Master",
            "rollo cortado" or "rollo_cortado" or "rollocortado" => "Rollo Cortado",
            "resma" or "resmas" => "Resma",
            "graphics" => "Graphics",
            _ when string.IsNullOrWhiteSpace(categoria) => "—",
            _ => categoria!.Trim()
        };
    }
}
