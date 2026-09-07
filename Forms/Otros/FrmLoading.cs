using System.Drawing;
using System.Windows.Forms;

using Sunny.UI;
namespace Ritrama2025.Forms.Otros;

public class FrmLoading : UIForm
{
    private readonly Label _lblMensaje;
    private readonly ProgressBar _progressBar;

    public FrmLoading(string mensaje = "Cargando datos...")
    {
        FormBorderStyle = FormBorderStyle.None;
        StartPosition = FormStartPosition.CenterScreen;
        TopMost = true;
        Size = new Size(340, 130);
        BackColor = Color.White;
        ControlBox = false;
        MaximizeBox = false;
        MinimizeBox = false;

        _lblMensaje = new Label
        {
            Text = mensaje,
            Dock = DockStyle.Top,
            Height = 60,
            TextAlign = ContentAlignment.MiddleCenter,
            Font = new Font("Segoe UI", 11f, FontStyle.Regular)
        };

        _progressBar = new ProgressBar
        {
            Style = ProgressBarStyle.Marquee,
            MarqueeAnimationSpeed = 30,
            Dock = DockStyle.Bottom,
            Height = 22
        };

        Controls.Add(_progressBar);
        Controls.Add(_lblMensaje);
    }

    public void SetMensaje(string mensaje) => _lblMensaje.Text = mensaje;
}


