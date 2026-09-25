using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

namespace Ritrama2025.Helpers
{
    public class GrayToolStripRenderer : ToolStripProfessionalRenderer
    {
        private readonly Color top = Color.FromArgb(245, 245, 245);
        private readonly Color bottom = Color.FromArgb(220, 220, 220);
        private readonly Color buttonHover = Color.FromArgb(235, 235, 235);
        private readonly Color buttonPressed = Color.FromArgb(200, 200, 200);

        protected override void OnRenderToolStripBackground(ToolStripRenderEventArgs e)
        {
            base.OnRenderToolStripBackground(e);
            using LinearGradientBrush b = new System.Drawing.Drawing2D.LinearGradientBrush(e.AffectedBounds, top, bottom, System.Drawing.Drawing2D.LinearGradientMode.Vertical);
            e.Graphics.FillRectangle(b, e.AffectedBounds);
            // draw separator bottom line
            using Pen pen = new Pen(Color.FromArgb(200, 200, 200));
            e.Graphics.DrawLine(pen, e.AffectedBounds.Left, e.AffectedBounds.Bottom - 1, e.AffectedBounds.Right, e.AffectedBounds.Bottom - 1);
        }

        protected override void OnRenderButtonBackground(ToolStripItemRenderEventArgs e)
        {
            base.OnRenderButtonBackground(e);
            if (e.Item is ToolStripButton btn)
            {
                Rectangle bounds = new Rectangle(Point.Empty, btn.ContentRectangle.Size);
                if (btn.Checked || btn.Pressed)
                {
                    using SolidBrush b = new SolidBrush(buttonPressed);
                    e.Graphics.FillRectangle(b, bounds);
                }
                else if (btn.Selected)
                {
                    using SolidBrush b = new SolidBrush(buttonHover);
                    e.Graphics.FillRectangle(b, bounds);
                }
            }
        }
    }
}
