using System;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Windows.Forms;

using Sunny.UI;
namespace Ritrama2025.Forms.Otros;

public class FrmLoading : UIForm
    {
        private readonly Label _lblMensaje;
        private readonly ProgressBar _progressBar;

        public FrmLoading(string mensaje = "Cargando datos...", string titulo = "Form UI")
        {
            Text = titulo;
            FormBorderStyle = FormBorderStyle.None;
            StartPosition = FormStartPosition.CenterScreen;
            TopMost = true;
            Size = new Size(340, 130);
            BackColor = Color.White;
            ControlBox = false;
            MaximizeBox = false;
            MinimizeBox = false;

            Region = GetRoundedRegion();

            _lblMensaje = new Label
            {
                Text = mensaje,
                Dock = DockStyle.Top,
                Height = 60,
                TextAlign = ContentAlignment.MiddleCenter,
                Font = new Font("Segoe UI", 11f, FontStyle.Regular),
                ForeColor = Color.FromArgb(110, 190, 40)
            };

            _progressBar = new ProgressBar
            {
                Style = ProgressBarStyle.Marquee,
                MarqueeAnimationSpeed = 30,
                Dock = DockStyle.Bottom,
                Height = 22,
                ForeColor = Color.FromArgb(110, 190, 40)
            };

            Controls.Add(_progressBar);
            Controls.Add(_lblMensaje);
        }

        private Region GetRoundedRegion()
        {
            var bounds = ClientRectangle;
            var path = new GraphicsPath();
            path.StartFigure();
            int radius = 20;
            path.AddArc(bounds.Left, bounds.Top, radius * 2, radius * 2, 180, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Top, radius * 2, radius * 2, 270, 90);
            path.AddArc(bounds.Right - radius * 2, bounds.Bottom - radius * 2, radius * 2, radius * 2, 0, 90);
            path.AddArc(bounds.Left, bounds.Bottom - radius * 2, radius * 2, radius * 2, 90, 90);
            path.CloseFigure();
            return new Region(path);
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            Region = GetRoundedRegion();
        }

        public void SetMensaje(string mensaje) => _lblMensaje.Text = mensaje;
    }


