using System.ComponentModel;

using Sunny.UI;
namespace Ritrama2025.Forms.Otros
{
    public partial class Frm_oneparameter : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Parameter { get; set; } = null!;
        public Frm_oneparameter()
        {
            InitializeComponent();
        }

        private void Frm_oneparameter_Load(object sender, EventArgs e)
        {

        }

        private void Btn_aceptar_Click(object sender, EventArgs e)
        {
            GuardarDatos();
        }
        private void GuardarDatos()
        {
            Parameter = txt_buscar.Text;
            Close();

        }

        private void txt_buscar_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (e.KeyChar == 13)
            {
                GuardarDatos();
            }
        }
    }
}


