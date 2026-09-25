using System.Drawing;
using System.Windows.Forms;
using Sunny.UI;

namespace Ritrama2025.Helpers
{
    public static class LightGreenTheme
    {
        // Paleta light-green
        public static Color Primary = Color.FromArgb(150, 210, 120);
        public static Color PrimaryDark = Color.FromArgb(100, 170, 80);
        public static Color Background = Color.White;
        public static Color AlternateRow = Color.FromArgb(240, 250, 230);
        public static Font GlobalFont = new Font("Microsoft Sans Serif", 10F);

        public static void Apply(Form f)
        {
            if (f == null)
            {
                return;
            }

            // Configura UIStyleManager para integrar Sunny.UI
            try
            {
                UIStyleManager mgr = new UIStyleManager();
                mgr.Style = UIStyle.Green;
                mgr.GlobalFont = true;
                mgr.GlobalFontName = GlobalFont.Name;
            }
            catch
            {
                // ignore si no está disponible en tiempo de diseño
            }

            f.BackColor = Background;
            f.ForeColor = Color.Black;
            f.Font = GlobalFont;

            // Aplica a controles de forma recursiva
            ApplyToControl(f);
        }

        public static void ApplyToControl(Control parent)
        {
            if (parent == null)
            {
                return;
            }

            foreach (Control c in parent.Controls)
            {
                // Ajustes generales: respetar valores expresamente establecidos en el diseñador.
                // Sólo aplicar el fondo si el control aún usa el color por defecto de sistema.
                if (c.BackColor == SystemColors.Control || c.BackColor == Color.Empty)
                {
                    c.BackColor = Background;
                }
                // Sólo aplicar ForeColor cuando usa el color por defecto del sistema
                if (c.ForeColor == SystemColors.ControlText || c.ForeColor == Color.Empty)
                {
                    c.ForeColor = Color.Black;
                }
                // No forzar la fuente de controles: el formulario ya recibe GlobalFont en Apply(Form)

                // Controles Sunny.UI específicos
                if (c is Sunny.UI.UIButton btn)
                {
                    btn.FillColor = Primary;
                    btn.FillColor2 = Primary;
                    btn.Style = UIStyle.Green;
                    btn.ForeColor = Color.White;
                }
                else if (c is Sunny.UI.UILabel lbl)
                {
                    lbl.ForeColor = PrimaryDark;
                }
                else if (c is Sunny.UI.UITextBox tb)
                {
                    tb.Style = UIStyle.Green;
                    tb.FillColor = Color.White;
                    tb.ForeColor = Color.Black;
                }
                else if (c is Sunny.UI.UIComboBox cb)
                {
                    cb.Style = UIStyle.Green;
                }
                else if (c is Sunny.UI.UITabControl tt)
                {
                    // Pestañas con cabecera verde al estilo Sunny.UI y fondo de página claro
                    tt.Style = UIStyle.Green;
                    tt.TabBackColor = PrimaryDark;
                    tt.TabSelectedColor = Primary;
                    tt.TabSelectedForeColor = Color.White;
                    tt.TabUnSelectedColor = Color.White;
                    tt.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
                    tt.FillColor = Primary;
                    // contenido de cada TabPage en tonos verdes claros para el tema
                    foreach (TabPage pg in tt.TabPages)
                    {
                        pg.BackColor = AlternateRow;
                    }
                }
                else if (c is TabPage pg)
                {
                    // Asegurar fondo del TabPage y permitir que contenedores sean transparentes
                    pg.BackColor = AlternateRow;
                    foreach (Control child in pg.Controls)
                    {
                        // No forzar DataGridView ni controles de entrada a transparente
                        if (child is Panel || child is GroupBox || child.GetType().Name.Contains("UIGroupBox"))
                        {
                            child.BackColor = Color.Transparent;
                        }
                        // Aplicar recursión para ajustar controles anidados
                        if (child.HasChildren)
                        {
                            ApplyToControl(child);
                        }
                    }
                }
                else if (c is DataGridView g)
                {
                    // Fondo general del grid acorde al tema (tonos verdes claros)
                    g.BackgroundColor = AlternateRow;
                    g.EnableHeadersVisualStyles = false;

                    // Cabecera de columnas: verde oscuro/primario con texto blanco
                    g.ColumnHeadersDefaultCellStyle.BackColor = PrimaryDark;
                    g.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
                    g.ColumnHeadersDefaultCellStyle.Font = GlobalFont;
                    g.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;

                    // Filas: alternado blanco/verde claro y selección en color primario
                    g.RowsDefaultCellStyle.BackColor = Background;
                    g.RowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
                    g.RowsDefaultCellStyle.Font = GlobalFont;
                    g.RowsDefaultCellStyle.SelectionBackColor = Primary;
                    g.RowsDefaultCellStyle.SelectionForeColor = Color.White;

                    g.AlternatingRowsDefaultCellStyle.BackColor = AlternateRow;
                    g.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);

                    // Cabecera de filas (indicador)
                    g.RowHeadersDefaultCellStyle.BackColor = AlternateRow;
                    g.RowHeadersDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
                    g.RowHeadersDefaultCellStyle.SelectionBackColor = Primary;
                    g.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;

                    g.GridColor = Color.FromArgb(200, 220, 180);
                }
                else if (c is ToolStrip ts)
                {
                    // ToolStrip styling acorde al tema
                    ts.BackColor = Primary;
                    ts.ForeColor = Color.White;
                    ts.RenderMode = ToolStripRenderMode.Professional;
                    ts.GripStyle = ToolStripGripStyle.Hidden;
                    foreach (ToolStripItem it in ts.Items)
                    {
                        if (it is ToolStripButton tsBtn)
                        {
                            tsBtn.BackColor = Color.Transparent;
                            tsBtn.ForeColor = Color.White;
                            tsBtn.DisplayStyle = ToolStripItemDisplayStyle.ImageAndText;
                            tsBtn.TextImageRelation = TextImageRelation.ImageAboveText;
                            tsBtn.ImageScaling = ToolStripItemImageScaling.None;
                            tsBtn.AutoSize = false;
                            tsBtn.Padding = new Padding(6);
                            try { tsBtn.Size = new Size(96, 64); } catch { }
                        }
                        else if (it is ToolStripDropDownButton dd)
                        {
                            dd.BackColor = Color.Transparent;
                            dd.ForeColor = Color.White;
                        }
                        else if (it is ToolStripLabel tl)
                        {
                            tl.ForeColor = Color.White;
                        }
                    }
                }

                // Recursión
                if (c.HasChildren)
                {
                    ApplyToControl(c);
                }
            }
        }
    }
}
