using System.ComponentModel;
using System.Data;
using Ritrama2025.Services.CommonService;


using Sunny.UI;
namespace Ritrama2025.Forms.Buscadores
{
    public partial class FrmBuscadorOC : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable DtItems { get; set; } = null!;

        DataView Dv { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Orden { get; set; } = "";
        public FrmBuscadorOC()
        {
            InitializeComponent();
        }

        private void FrmBuscadorOC_Load(object sender, EventArgs e)
        {
            Dv = DtItems.DefaultView;
            Dv.RowFilter = "";
            DefinitionColumnsGrid();
            RowsCounter();
        }
        private void DefinitionColumnsGrid()
        {
            Grid_Items.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("numero", 50, "Orden", "numero", Grid_Items);
            CommonService.ADD_COLUMN_GRID("fecha_reg", 70, "Ingreso", "fecha", Grid_Items);
            CommonService.ADD_COLUMN_GRID("fecha_pro", 70, "Produccion", "fecha_produccion", Grid_Items);
            CommonService.ADD_COLUMN_GRID("rollid", 70, "Roll-Id", "rollid_1", Grid_Items);
            CommonService.ADD_COLUMN_GRID("width", 70, "Width[Inch]", "width_1", Grid_Items);
            CommonService.ADD_COLUMN_GRID("lenght", 70, "Length[Pies]", "lenght_1", Grid_Items);
            CommonService.ADD_COLUMN_GRID("product_id", 50, "Prod Id", "product_id", Grid_Items);
            CommonService.ADD_COLUMN_GRID("product_name", 150, "Product Name", "product_name", Grid_Items);
            CommonService.ADD_COLUMN_GRID("cust_name", 150, "Customer Name", "customer_name", Grid_Items);
            CommonService.ADD_COLUMN_GRID("sellOrder", 70, "Sell Order", "sellOrder", Grid_Items);
            CommonService.ADD_COLUMN_GRID("operator_name", 70, "Operador", "nombre", Grid_Items);

            Grid_Items.DataSource = Dv;
        }
        private void RowsCounter()
        {
            lbl_registros_encontrados.Text = Convert.ToString(Dv.Count) + " Registro Encontrados";
        }

        private void btn_buscar_Click(object sender, EventArgs e)
        {
            BuscarItems();
        }
        private void BuscarItems()
        {
            if (rad_numeroOrden.Checked)
            {
                Dv.RowFilter = "Convert(numero, 'System.String') LIKE '%" + txt_buscar.Text.Trim() + "%'";
            }
            if (rad_Customer.Checked)
            {
                Dv.RowFilter = "customer_name LIKE '%" + txt_buscar.Text.Trim() + "%'";
            }
            if (rad_Opetator.Checked)
            {
                Dv.RowFilter = "nombre LIKE '%" + txt_buscar.Text.Trim() + "%'";
            }
            if (rad_Product_id.Checked)
            {
                Dv.RowFilter = "product_id LIKE '%" + txt_buscar.Text.Trim() + "%'";
            }
            if (rad_ProductName.Checked)
            {
                Dv.RowFilter = "product_name LIKE '%" + txt_buscar.Text.Trim() + "%'";
            }
            if (rad_Sell_Order.Checked)
            {
                Dv.RowFilter = "sellOrder LIKE '%" + txt_buscar.Text.Trim() + "%'";
            }
            if (rad_RollId.Checked)
            {
                Dv.RowFilter = "rollid_1 LIKE '%" + txt_buscar.Text.Trim() + "%'";
            }
            if (rad_fecha_emision.Checked)
            {
                string fechadesde = txt_fecha_desde.Value.ToString("MM/dd/yyyy");
                string fechahasta = txt_fecha_hasta.Value.ToString("MM/dd/yyyy");
                Dv.RowFilter = $"fecha >= #{fechadesde}# AND fecha <= #{fechahasta}#";
            }
            if (rad_fecha_produccion.Checked)
            {
                string fechadesde = txt_fecha_desde.Value.ToString("MM/dd/yyyy");
                string fechahasta = txt_fecha_hasta.Value.ToString("MM/dd/yyyy");
                Dv.RowFilter = $"fecha_produccion >= #{fechadesde}# AND fecha_produccion <= #{fechahasta}#";
            }
            RowsCounter();
        }

        private void btn_reload_Click(object sender, EventArgs e)
        {
            Dv.RowFilter = "";
        }

        private void Grid_Items_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            Orden = Grid_Items.Rows[e.RowIndex].Cells["numero"].Value!.ToString()!;
            this.Close();
        }
    }
}


