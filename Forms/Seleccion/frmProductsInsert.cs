using System.ComponentModel;
using System.Data;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.CommonService;

using Sunny.UI;
namespace Ritrama2025.Forms.Seleccion
{

    public partial class FrmProductsInsert : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable DtItems { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Titulo { get; set; } = string.Empty;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public ProductMAP Producto { get; set; } = new ProductMAP();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string TypeProduct { get; set; } = "";

        readonly IServiceCommonData ServiceData;
        readonly ICommonService CommonService;

        public FrmProductsInsert(IServiceCommonData serviceData, ICommonService commonService)
        {
            InitializeComponent();

            this.ServiceData = serviceData;
            this.CommonService = commonService;
        }

        private void Btn_buscar_Click(object sender, EventArgs e)
        {
            FrmSeleccion frmSelectProducts = new(CommonService)
            {
                DtItems = DtItems,
                Titulo = "Producto"
            };
            frmSelectProducts.ShowDialog();
            txt_productid.Text = frmSelectProducts.Id;
            txt_productname.Text = frmSelectProducts.Description;

            switch (frmSelectProducts.Tipo)
            {
                case "Master":
                    rad_master.Checked = true;
                    rad_hojas.Checked = false;
                    rad_graphics.Checked = false;
                    rad_rolloCortado.Checked = false;
                    TypeProduct = "Master";
                    break;
                case "Resma":
                    rad_master.Checked = false;
                    rad_hojas.Checked = true;
                    rad_graphics.Checked = false;
                    rad_rolloCortado.Checked = false;
                    TypeProduct = "Resma";
                    break;
                case "Graphics":
                    rad_master.Checked = false;
                    rad_hojas.Checked = false;
                    rad_graphics.Checked = true;
                    rad_rolloCortado.Checked = false;
                    TypeProduct = "Graphics";
                    break;
                case "Rollo Cortado":
                    rad_master.Checked = false;
                    rad_hojas.Checked = false;
                    rad_graphics.Checked = false;
                    rad_rolloCortado.Checked = true;
                    TypeProduct = "Rollo Cortado";
                    break;
                default:
                    break;
            }
        }

        private void FrmProductsInsert_Load(object sender, EventArgs e)
        {
            txt_cant.Text = "1";
            txt_width.Text = "0";
            txt_msi.Text = "0";
            txt_lenght.Text = "0";
            txt_splice.Text = "0";
            txt_core.Text = "0";
            txt_ubic.Text = "SU";
            txt_rollid.Text = "0";
        }
        private void CALCULAR_MSI()
        {
            if (txt_width.Text == "" || txt_lenght.Text == "") return;

            double width = Convert.ToDouble(txt_width.Text);
            double lenght = Convert.ToDouble(txt_lenght.Text);
            double msi = CalculosOrdenCorte.CalcularMsi(width, lenght);
            txt_msi.Text = msi.ToString();
        }

        private void Txt_width_KeyUp(object sender, KeyEventArgs e)
        {
            CALCULAR_MSI();
        }

        private void Txt_lenght_KeyUp(object sender, KeyEventArgs e)
        {

            CALCULAR_MSI();
        }

        private void Btn_guardar_Click(object sender, EventArgs e)
        {
            if (txt_productid.Text == "")
            {
                MessageBox.Show("Introduzca el codigo de producto...");
                return;
            }
            if (txt_width.Text == "0" && txt_width.Text == "")
            {
                MessageBox.Show("Introduzca el ancho del producto");
                return;
            }
            if (txt_lenght.Text == "0")
            {
                MessageBox.Show("Introduzca el largo del producto");
                return;
            }
            if (txt_rollid.Text == "0")
            {
                MessageBox.Show("Introduzca el rollid del producto");
                return;
            }

            //verificar que l roll-id no exista.
            if (!ServiceData.VerificarRollIdNoRepeat(txt_rollid.Text)) return;




            //genero el producto.
            Producto = new ProductMAP
            {
                Product_Id = txt_productid.Text,
                Product_Name = txt_productname.Text,
                Product_Type = TypeProduct,
                Width = Convert.ToDouble(txt_width.Text),
                Length = Convert.ToDouble(txt_lenght.Text),
                Msi = Convert.ToDouble(txt_msi.Text),
                Core = Convert.ToInt32(txt_core.Text),
                Rollid = txt_rollid.Text,
                Splice = Convert.ToInt16(txt_splice.Text),
                Cant = Convert.ToInt32(txt_cant.Text),
                Ubic = txt_ubic.Text,
            };
            this.Close();
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void Txt_width_KeyPress(object sender, KeyPressEventArgs e)
        { // Permitir tecla de retroceso (Backspace)
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void Txt_lenght_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar) && e.KeyChar != '.')
            {
                e.Handled = true;
            }
        }

        private void Txt_core_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Txt_splice_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Txt_cant_KeyPress(object sender, KeyPressEventArgs e)
        {
            if (!char.IsControl(e.KeyChar) && !char.IsDigit(e.KeyChar))
            {
                e.Handled = true;
            }
        }

        private void Txt_width_Enter(object sender, EventArgs e)
        {
            var txt = (UITextBox)sender;

            if (string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "0";

            txt.BeginInvoke(new Action(() => txt.SelectAll()));
        }

        private void Txt_lenght_Enter(object sender, EventArgs e)
        {
            var txt = (UITextBox)sender;

            if (string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "0";

            txt.BeginInvoke(new Action(() => txt.SelectAll()));
        }

        private void Txt_core_Enter(object sender, EventArgs e)
        {
            var txt = (UITextBox)sender;

            if (string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "0";

            txt.BeginInvoke(new Action(() => txt.SelectAll()));
        }

        private void Txt_splice_Enter(object sender, EventArgs e)
        {
            var txt = (UITextBox)sender;

            if (string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "0";

            txt.BeginInvoke(new Action(() => txt.SelectAll()));
        }

        private void Txt_rollid_Enter(object sender, EventArgs e)
        {
            var txt = (UITextBox)sender;

            if (string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "0";

            txt.BeginInvoke(new Action(() => txt.SelectAll()));
        }

        private void Txt_ubic_Enter(object sender, EventArgs e)
        {
            var txt = (UITextBox)sender;

            if (string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "0";

            txt.BeginInvoke(new Action(() => txt.SelectAll()));
        }

        private void Txt_cant_Enter(object sender, EventArgs e)
        {
            var txt = (UITextBox)sender;

            if (string.IsNullOrWhiteSpace(txt.Text))
                txt.Text = "0";

            txt.BeginInvoke(new Action(() => txt.SelectAll()));
        }
    }
}


