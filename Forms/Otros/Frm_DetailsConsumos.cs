using System.ComponentModel;
using System.Data;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ProduccionService;

using Sunny.UI;
namespace Ritrama2025.Forms.Otros
{
    public partial class Frm_DetailsConsumos : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Rollid { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Productid { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Product_Name { get; set; } = null!;
        private DataTable DtItems { get; set; } = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Width_t { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Length { get; set; } = null!;
        private IProduccionService ProduccionService { get; set; }
        public Frm_DetailsConsumos(IProduccionService produccionService)
        {
            InitializeComponent();
            ProduccionService = produccionService;
        }

        private async void Frm_DetailsConsumos_Load(object sender, EventArgs e)
        {
            LoadParameters();
            ColumnDefinitionsGrid();
            try
            {
                await LoadData();
                CalculateTotalConsumo();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar detalle de consumos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void CalculateTotalConsumo()
        {
            if (Grid_Items.Rows.Count == 0)
            {
                return;
            }

            double total_consumo = 0;
            int rollos = 0;
            for (int i = 1; i < Grid_Items.Rows.Count + 1; i++)
            {
                Grid_Items.Rows[i - 1].Cells["item"].Value = i.ToString();
                total_consumo += Convert.ToDouble(Grid_Items.Rows[i - 1].Cells["consumo"].Value) +
                    Convert.ToDouble(Grid_Items.Rows[i - 1].Cells["monto_des"].Value);
                rollos += Convert.ToInt32(Grid_Items.Rows[i - 1].Cells["cant_rollos"].Value);

            }
            txt_total.Text = total_consumo.ToString("N2");
            txt_rollos_Producc.Text = rollos.ToString();
            RowCounter.Text = "Documentos Relacionados : " + Grid_Items.Rows.Count.ToString();
        }

        private async Task LoadData()
        {
            DataTable? dt = await ProduccionService.LoadDataDetailsConsumosMasterInic(Rollid);
            DtItems = dt ?? new DataTable();
            Grid_Items.DataSource = DtItems;
        }

        private void ColumnDefinitionsGrid()
        {
            Grid_Items.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("item", 30, "It.", "", Grid_Items);
            CommonService.ADD_COLUMN_GRID("orden", 60, "Orden Corte", "orden", Grid_Items);
            CommonService.ADD_COLUMN_GRID("consumo", 60, "Consumo Length [Pies]", "consumo", Grid_Items);
            CommonService.ADD_COLUMN_GRID("cant_rollos", 60, "Cant. Rollos", "cant_rollos", Grid_Items);
            CommonService.ADD_COLUMN_GRID("customer_name", 130, "Cliente", "customer_name", Grid_Items);
            CommonService.ADD_COLUMN_GRID("fecha", 100, "Fecha Registro", "fecha_reg", Grid_Items);
            CommonService.ADD_COLUMN_GRID("monto_des", 60, "Monto desperdicio", "monto_desperdicio", Grid_Items);
            DataGridViewCheckBoxColumn colCheck = new DataGridViewCheckBoxColumn
            {
                Name = "chk_desper",
                Width = 80,
                HeaderText = "Desperdicio",
                DataPropertyName = "desperdicio",
            };
            colCheck.DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            Grid_Items.Columns.Add(colCheck);
        }
        private void LoadParameters()
        {
            txt_rollid.Text = Rollid;
            txt_productid.Text = Productid;
            txt_productName.Text = Product_Name;
            txt_width.Text = Width_t;
            txt_lenght.Text = Length;
        }
    }
}


