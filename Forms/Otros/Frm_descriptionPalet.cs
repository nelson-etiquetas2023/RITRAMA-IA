using System.ComponentModel;


using Sunny.UI;
namespace Ritrama2025.Forms.Otros
{
    public partial class Frm_descriptionPalet : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string ContentTextDescription { get; set; } = string.Empty;
        public Frm_descriptionPalet()
        {
            InitializeComponent();
        }

        private void Btn_aceptar_Click(object sender, EventArgs e)
        {
            if (!string.IsNullOrEmpty(txt_ContentText.Text))
            {
                ContentTextDescription = txt_ContentText.Text;
            }

            Close();
        }

        private void Frm_descriptionPalet_Load(object sender, EventArgs e)
        {
            txt_ContentText.Text = ContentTextDescription;
        }
    }
}


