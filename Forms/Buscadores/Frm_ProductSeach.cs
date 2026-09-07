using System.ComponentModel;
using System.Data;
using Ritrama2025.Services.CommonService;

using Sunny.UI;
namespace Ritrama2025.Forms.Buscadores
{
    public partial class Frm_ProductSeach : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable DtItems { get; set; } = new();
        DataView Dv = new();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Selected_ProductID { get; set; } = "";
        public Frm_ProductSeach()
        {
            InitializeComponent();
        }

        private void Frm_ProductSeach_Load(object sender, EventArgs e)
        {
            Dv = DtItems.DefaultView;
            Dv.RowFilter = "";
            Grid_Products.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("product_id", 70, "Product Id.", "product_id", Grid_Products);
            CommonService.ADD_COLUMN_GRID("product_name", 420, "Descripcion del Producto", "product_name", Grid_Products);
            CommonService.ADD_COLUMN_GRID("tipo", 100, "Tipo Producto", "tipo", Grid_Products);
            Grid_Products.DataSource = Dv;
            RefreshForm();
        }
        private void RefreshForm()
        {
            COUNTER_ROWS.Text = "REGISTROS: " + Dv.Count.ToString();
        }
        private void btn_search_Click(object sender, EventArgs e)
        {
            if (rad_productid.Checked)
            {
                Dv.RowFilter = "product_id like '%" + txt_buscar.Text + "%'";
            }
            if (rad_productName.Checked)
            {
                Dv.RowFilter = "product_name like '%" + txt_buscar.Text + "%'";
            }
            if (rad_master.Checked || rad_graphics.Checked || rad_hojas.Checked || rad_rolloCortado.Checked)
            {
                Dv.RowFilter = "tipo like '%" + txt_buscar.Text + "%'";
            }
            RefreshForm();
        }

        private void rad_master_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_master.Checked)
            {
                txt_buscar.Text = "MASTER";
            }
            else
            {
                txt_buscar.Text = "";
            }

        }

        private void rad_graphics_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_graphics.Checked)
            {
                txt_buscar.Text = "GRAPHICS";
            }
            else
            {
                txt_buscar.Text = "";
            }
        }

        private void rad_hojas_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_hojas.Checked)
            {
                txt_buscar.Text = "RESMA";
            }
            else
            {
                txt_buscar.Text = "";
            }
        }

        private void rad_rolloCortado_CheckedChanged(object sender, EventArgs e)
        {
            if (rad_rolloCortado.Checked)
            {
                txt_buscar.Text = "ROLLO";
            }
            else
            {
                txt_buscar.Text = "";
            }
        }

        private void Grid_Products_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex == -1) return;
            Selected_ProductID = Grid_Products.Rows[e.RowIndex].Cells["product_id"].Value!.ToString()!;
            this.Close();
        }
    }
}


