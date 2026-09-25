using System.ComponentModel;
using System.Data;
using Ritrama2025.Services.CommonService;

using Sunny.UI;
namespace Ritrama2025.Forms.Buscadores
{
    public partial class FrmBuscador_OrdenesMP : UIForm

    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable DtItems { get; set; } = null!;
        DataView Dv = new();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Orden { get; set; } = "";
        public FrmBuscador_OrdenesMP()
        {
            InitializeComponent();
        }

        private void FrmBuscador_OrdenesMP_Load(object sender, EventArgs e)
        {
            Dv = DtItems.DefaultView;
            Dv.RowFilter = "";
            Grid_Items.AutoGenerateColumns = false;
            SetStyleGrid();
            Grid_Items.DataSource = Dv;
            RowsCounter();


        }
        private void RowsCounter()
        {
            lbl_registros_encontrados.Text = Convert.ToString(Dv.Count) + " Registro Encontrados";
        }

        private void SetStyleGrid()
        {
            CommonService.ADD_COLUMN_GRID("numero", 70, "Orden", "numero", Grid_Items);
            CommonService.ADD_COLUMN_GRID("fecha_recepcion", 70, "Ingreso", "fecha_recepcion", Grid_Items);
            CommonService.ADD_COLUMN_GRID("fecha_pro", 70, "Producci�n", "fecha_pro", Grid_Items);
            CommonService.ADD_COLUMN_GRID("orden_compra", 70, "O.Compra", "orden_compra", Grid_Items);
            CommonService.ADD_COLUMN_GRID("proveedor_name", 70, "Proveedor", "proveedor_name", Grid_Items);
            CommonService.ADD_COLUMN_GRID("transport_name", 70, "Transporte", "transport_name", Grid_Items);
            CommonService.ADD_COLUMN_GRID("person_name", 70, "Recepcionista", "person_name", Grid_Items);
            CommonService.ADD_COLUMN_GRID("guia_import", 70, "Guia", "guia_import", Grid_Items);
            CommonService.ADD_COLUMN_GRID("doc_embarque", 70, "Embarque", "doc_embarque", Grid_Items);
            CommonService.ADD_COLUMN_GRID("lote", 70, "Lote", "lote", Grid_Items);
            CommonService.ADD_COLUMN_GRID("closedocument", 70, "Doc. Cerrado", "closedocument", Grid_Items);
            CommonService.ADD_COLUMN_GRID("anulado", 70, "Doc. Anulado", "anulado", Grid_Items);
        }


        private void btn_buscar_Click(object sender, EventArgs e)
        {
            BuscarItems();
        }
        private void BuscarItems()
        {
            if (rad_numeroOrden.Checked)
            {
                Dv.RowFilter = "numero like '%" + txt_buscar.Text + "%'";
            }
            if (rad_proveedor.Checked)
            {
                Dv.RowFilter = "proveedor_name like '%" + txt_buscar.Text + "%'";
            }
            if (rad_transporte.Checked)
            {
                Dv.RowFilter = "transport_name like '%" + txt_buscar.Text + "%'";
            }
            if (rad_recepcionista.Checked)
            {
                Dv.RowFilter = "person_name like '%" + txt_buscar.Text + "%'";
            }
            if (rad_embarque.Checked)
            {
                Dv.RowFilter = "doc_embarque like '%" + txt_buscar.Text + "%'";
            }
            if (rad_Orden_Compra.Checked)
            {
                Dv.RowFilter = "orden_compra like '%" + txt_buscar.Text + "%'";
            }
            if (rad_guia.Checked)
            {
                Dv.RowFilter = "guia_import like '%" + txt_buscar.Text + "%'";
            }
            if (rad_fecha_emision.Checked)
            {
                string fechadesde = txt_fecha_desde.Value.ToString("MM/dd/yyyy");
                string fechahasta = txt_fecha_hasta.Value.ToString("MM/dd/yyyy");

                Dv.RowFilter = $"fecha_recepcion >= #{fechadesde}# AND fecha_recepcion <= #{fechahasta}#";
            }
            if (rad_fecha_produccion.Checked)
            {
                string fechadesde = txt_fecha_desde.Value.ToString("MM/dd/yyyy");
                string fechahasta = txt_fecha_hasta.Value.ToString("MM/dd/yyyy");

                Dv.RowFilter = $"fecha_pro >= #{fechadesde}# AND fecha_pro <= #{fechahasta}#";
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
            Orden = Grid_Items.Rows[e.RowIndex].Cells[0].Value!.ToString()!;
            Close();
        }
    }
}


