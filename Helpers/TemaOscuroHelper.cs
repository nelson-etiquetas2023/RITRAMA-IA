using System;
using System.Drawing;
using System.Windows.Forms;

namespace Ritrama2025.Helpers
{
    /// <summary>
    /// Aplica un tema oscuro coherente a un formulario y a todos sus controles,
    /// recorriendo la jerarquia de forma recursiva.
    /// </summary>
    public static class TemaOscuroHelper
    {
        public static readonly Color Fondo = Color.FromArgb(37, 37, 42);
        public static readonly Color FondoSecundario = Color.FromArgb(45, 45, 51);
        public static readonly Color FondoControles = Color.FromArgb(30, 30, 35);
        public static readonly Color Texto = Color.FromArgb(235, 235, 235);
        public static readonly Color Acento = Color.FromArgb(235, 109, 14);
        public static readonly Color GridEncabezado = Color.FromArgb(52, 52, 59);
        public static readonly Color GridSeleccion = Color.FromArgb(90, 90, 99);

        public static void Aplicar(Control root)
        {
            if (root == null)
            {
                return;
            }

            root.SuspendLayout();
            try
            {
                PintarControl(root);
                foreach (Control hijo in root.Controls)
                {
                    Aplicar(hijo);
                }
            }
            finally
            {
                root.ResumeLayout(true);
                root.PerformLayout();
            }
        }

        private static void PintarControl(Control ctrl)
        {
            if (ctrl is Form f)
            {
                f.BackColor = Fondo;
                f.ForeColor = Texto;
            }
            else if (ctrl is DataGridView g)
            {
                AplicarGrid(g);
            }
            else if (ctrl is Panel || ctrl is UserControl || ctrl is TabPage)
            {
                ctrl.BackColor = Fondo;
                ctrl.ForeColor = Texto;
            }
            else if (ctrl is Label || ctrl is GroupBox || ctrl is CheckBox || ctrl is RadioButton)
            {
                ctrl.BackColor = Fondo;
                ctrl.ForeColor = Texto;
            }
            else if (ctrl is TextBox tb)
            {
                tb.BackColor = FondoControles;
                tb.ForeColor = Texto;
                tb.BorderStyle = BorderStyle.FixedSingle;
            }
            else if (ctrl is RichTextBox rtb)
            {
                rtb.BackColor = FondoControles;
                rtb.ForeColor = Texto;
            }
            else if (ctrl is NumericUpDown nud)
            {
                nud.BackColor = FondoControles;
                nud.ForeColor = Texto;
            }
            else if (ctrl is ComboBox cb)
            {
                cb.BackColor = FondoControles;
                cb.ForeColor = Texto;
            }
            else if (ctrl is DateTimePicker dtp)
            {
                dtp.CalendarMonthBackground = FondoControles;
                dtp.BackColor = FondoControles;
                dtp.ForeColor = Texto;
            }
            else if (ctrl is ListBox lb)
            {
                lb.BackColor = FondoControles;
                lb.ForeColor = Texto;
            }
            else if (ctrl is Button b)
            {
                b.BackColor = FondoSecundario;
                b.ForeColor = Texto;
                b.FlatStyle = FlatStyle.Flat;
                b.FlatAppearance.BorderSize = 0;
            }
            else if (ctrl is TabControl tc)
            {
                tc.BackColor = Fondo;
                tc.ForeColor = Texto;
            }
            else if (ctrl is ToolStrip ts)
            {
                ts.BackColor = FondoSecundario;
                ts.ForeColor = Texto;
                foreach (ToolStripItem item in ts.Items)
                {
                    item.ForeColor = Texto;
                    if (item is ToolStripButton)
                    {
                        item.BackColor = FondoSecundario;
                    }
                }
            }
            else if (ctrl is StatusStrip ss)
            {
                ss.BackColor = FondoSecundario;
                ss.ForeColor = Texto;
            }
        }

        private static void AplicarGrid(DataGridView g)
        {
            g.BackgroundColor = Fondo;
            g.GridColor = GridEncabezado;
            g.BorderStyle = BorderStyle.None;
            g.ForeColor = Texto;
            g.DefaultCellStyle.BackColor = FondoControles;
            g.DefaultCellStyle.ForeColor = Texto;
            g.DefaultCellStyle.SelectionBackColor = GridSeleccion;
            g.DefaultCellStyle.SelectionForeColor = Color.White;
            g.EnableHeadersVisualStyles = false;
            g.ColumnHeadersDefaultCellStyle.BackColor = GridEncabezado;
            g.ColumnHeadersDefaultCellStyle.ForeColor = Texto;
            g.ColumnHeadersDefaultCellStyle.SelectionBackColor = GridEncabezado;
            g.ColumnHeadersDefaultCellStyle.SelectionForeColor = Texto;
            g.RowHeadersDefaultCellStyle.BackColor = FondoSecundario;
            g.RowHeadersDefaultCellStyle.ForeColor = Texto;
            g.RowHeadersDefaultCellStyle.SelectionBackColor = GridSeleccion;
            g.RowHeadersDefaultCellStyle.SelectionForeColor = Color.White;
        }
    }
}
