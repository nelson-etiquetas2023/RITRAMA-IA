using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ritrama2025.Forms;

/// <summary>
/// Helper estático para dibujar una barra de progreso en celdas de un DataGridView.
/// Se usa desde el evento CellPainting del grid. No depende de la versión de .NET.
/// Colores dinámicos según porcentaje: Rojo (0-25%), Naranja (25-50%), Amarillo (50-75%), Verde (75-100%).
/// </summary>
public static class ProgressBarRenderer
{
    private static readonly Color ColorBarraBg = Color.FromArgb(235, 235, 235);
    private static readonly Color ColorRojo = Color.FromArgb(220, 53, 69);
    private static readonly Color ColorNaranja = Color.FromArgb(255, 152, 0);
    private static readonly Color ColorAmarillo = Color.FromArgb(255, 211, 42);
    private static readonly Color ColorVerde = Color.FromArgb(40, 167, 69);
    private static readonly Color ColorTexto = Color.FromArgb(48, 48, 48);

    public static void PaintCell(Graphics? graphics, Rectangle cellBounds,
        DataGridViewElementStates cellState, object? value,
        DataGridViewCellStyle? cellStyle, bool selected,
        string columnLengthName = "length",
        string columnRestanteName = "length_restante")
    {
        if (graphics == null || cellStyle == null) return;

        // Calcular porcentaje.
        double pct = CalcularPorcentaje(value, graphics, cellBounds, columnLengthName, columnRestanteName);

        // Fondo de la celda: usa el color del estilo (blanco/alterno del grid).
        Color colorFondo = selected ? cellStyle.SelectionBackColor : cellStyle.BackColor;
        using (Brush bgBrush = new SolidBrush(colorFondo))
            graphics.FillRectangle(bgBrush, cellBounds);

        // Borde de la celda: gris del grid.
        ControlPaint.DrawBorder(graphics, cellBounds, Color.FromArgb(225, 225, 225), ButtonBorderStyle.Solid);

        // Margen interno.
        Rectangle barBounds = new(cellBounds.X + 2, cellBounds.Y + 3, cellBounds.Width - 5, cellBounds.Height - 7);
        if (barBounds.Width <= 0 || barBounds.Height <= 0) return;

        int radius = Math.Min(6, barBounds.Height / 2);

        // Fondo de la barra (gris claro — se ve sobre cualquier color de fila).
        using (GraphicsPath bgPath = CreateRoundedRect(barBounds, radius))
        using (Brush barBgBrush = new SolidBrush(ColorBarraBg))
            graphics.FillPath(barBgBrush, bgPath);

        // Barra de progreso con gradiente.
        if (pct > 0)
        {
            Color colorBarra = pct switch
            {
                < 25 => ColorRojo,
                < 50 => ColorNaranja,
                < 75 => ColorAmarillo,
                _ => ColorVerde
            };

            int fillWidth = (int)(barBounds.Width * (pct / 100.0));
            fillWidth = Math.Max(fillWidth, radius * 2);
            fillWidth = Math.Min(fillWidth, barBounds.Width);

            Rectangle fillBounds = new(barBounds.X, barBounds.Y, fillWidth, barBounds.Height);
            using GraphicsPath fillPath = CreateRoundedRect(fillBounds, radius);
            using (LinearGradientBrush fillBrush = new(fillBounds, ControlPaint.Light(colorBarra), colorBarra, LinearGradientMode.Vertical))
            {
                graphics.SetClip(fillPath);
                graphics.FillRectangle(fillBrush, barBounds);
                graphics.ResetClip();
            }

            // Borde de la barra.
            using GraphicsPath borderPath = CreateRoundedRect(barBounds, radius);
            using Pen borderPen = new(ControlPaint.Dark(colorBarra, 0.1f), 1.2f);
            graphics.DrawPath(borderPen, borderPath);
        }

        // Texto del porcentaje centrado.
        string texto = $"{pct:N0} %";
        using Font font = new("Segoe UI", 9f, FontStyle.Bold);
        SizeF textSize = graphics.MeasureString(texto, font);
        PointF textPos = new(
            cellBounds.X + (cellBounds.Width - textSize.Width) / 2f,
            cellBounds.Y + (cellBounds.Height - textSize.Height) / 2f);
        using Brush textBrush = new SolidBrush(pct < 25 ? Color.White : ColorTexto);
        graphics.DrawString(texto, font, textBrush, textPos);
    }

    private static double CalcularPorcentaje(object? value, Graphics g, Rectangle bounds,
        string colLength, string colRestante)
    {
        if (value is double d) return Math.Clamp(d, 0, 100);
        if (value is string s && double.TryParse(s.Replace("%", "").Trim(), out double parsed))
            return Math.Clamp(parsed, 0, 100);
        return 0;
    }

    public static double CalcularDesdeFila(DataGridViewRow fila, string colLength = "length", string colRestante = "length_restante")
    {
        try
        {
            object? vLenght = fila.Cells[colLength].Value;
            object? vRestante = fila.Cells[colRestante].Value;
            if (vLenght == null || vLenght == DBNull.Value || vRestante == null || vRestante == DBNull.Value) return 0;

            if (double.TryParse(vLenght.ToString(), out double total) &&
                double.TryParse(vRestante.ToString(), out double restante) &&
                total > 0)
            {
                return Math.Clamp((restante / total) * 100, 0, 100);
            }
        }
        catch { }
        return 0;
    }

    private static GraphicsPath CreateRoundedRect(Rectangle rect, int radius)
    {
        GraphicsPath path = new();
        int diameter = radius * 2;
        path.AddArc(rect.X, rect.Y, diameter, diameter, 180, 90);
        path.AddArc(rect.Right - diameter, rect.Y, diameter, diameter, 270, 90);
        path.AddArc(rect.Right - diameter, rect.Bottom - diameter, diameter, diameter, 0, 90);
        path.AddArc(rect.X, rect.Bottom - diameter, diameter, diameter, 90, 90);
        path.CloseFigure();
        return path;
    }
}
