using System.ComponentModel;
using System.Data;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;


using Sunny.UI;
namespace Ritrama2025.Forms.Seleccion
{
    public partial class Frm_RollId : UIForm
    {
        DataView Dv = new();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable DtRollid { get; set; } = new();
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public RolloCortado MasterRoll { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Tipo_mov { get; set; } = "";

        private IProduccionService ProduccionService { get; set; }
        public Frm_RollId(IProduccionService produccionService)
        {
            InitializeComponent();
            ProduccionService = produccionService;
        }

        private async void Frm_RollId_Load(object sender, EventArgs e)
        {
            try
            {
                DtRollid = await ProduccionService.LoadDataRollID();

                Dv = DtRollid.DefaultView;
                GridItems.AutoGenerateColumns = false;
                StyleGridColumns();
                GridItems.DataSource = DtRollid;
                RefreshForms();
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error al cargar rollos: " + ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void RefreshForms()
        {
            CONTADOR_REGISTROS.Text = Convert.ToString(Dv.Count) + " registros encontrados.";
        }
        private void StyleGridColumns()
        {
            GridItems.DataSource = null;
            GridItems.Columns.Clear();
            AGREGAR_COLUMN_GRID("rollid", 70, "Roll Id", "Roll_Id");
            AGREGAR_COLUMN_GRID("product_id", 60, "Product Id", "part_number");
            AGREGAR_COLUMN_GRID("product_name", 240, "Nombre del Producto", "product_name");
            AGREGAR_COLUMN_GRID("Ancho", 65, "Width", "width");
            AGREGAR_COLUMN_GRID("lenght", 80, "Largo Original", "lenght");
            AGREGAR_COLUMN_GRID("lenght_consumido", 80, "Consumido", "largo_consumido");
            AGREGAR_COLUMN_GRID("largo_restante", 80, "Restante", "largo_restante");
            AGREGAR_COLUMN_GRID("estado", 70, "Estado", "estado");
            AGREGAR_COLUMN_GRID("fecha_pro", 67, "Fecha produccion", "fecha_pro");
            AGREGAR_COLUMN_GRID("fecha_reg", 67, "Llegada", "fecha_reg");
            AGREGAR_COLUMN_GRID("splice", 50, "Splice", "splice");
            AGREGAR_COLUMN_GRID("ubicacion", 60, "Ubicacion", "ubicacion");
            AGREGAR_COLUMN_GRID("tipo_mov", 40, "Tipo", "tipo_mov");
        }
        private void AGREGAR_COLUMN_GRID(string name, int size, string title, string field_bd)
        {
            DataGridViewTextBoxColumn dataGridViewColumn = new()
            {
                Name = name,
                Width = size,
                HeaderText = title,
                DataPropertyName = field_bd
            };
            GridItems.Columns.Add(dataGridViewColumn);
        }
        private async Task BuscarMasterIdData()
        {
            string columna = rad_rollid.Checked ? "Roll_Id" : rad_productid.Checked ? "Part_Number" : "Product_Name";
            DtRollid = await ProduccionService.BuscarRollId(columna, txt_buscar.Text);
            Dv = DtRollid.DefaultView;
            GridItems.DataSource = DtRollid;
            RefreshForms();
        }

        private async void Btn_buscar_Click(object sender, EventArgs e)
        {
            await BuscarMasterIdData();
        }

        private void GridItems_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex >= 0)
            {
                object? restoValue = GridItems.Rows[e.RowIndex].Cells["largo_restante"].Value;
                double resto = restoValue != null && restoValue != DBNull.Value && double.TryParse(restoValue.ToString(), out double r)
                    ? r
                    : 0;

                if (resto <= 0)
                {
                    MessageBox.Show("este rollo ya ha sido consumido...");
                    return;
                }
                // Ensure that the cell values are not null before accessing them
                var rollIdValue = GridItems.Rows[e.RowIndex].Cells[0].Value?.ToString();
                var productIdValue = GridItems.Rows[e.RowIndex].Cells[1].Value?.ToString();
                var productNameValue = GridItems.Rows[e.RowIndex].Cells[2].Value?.ToString();
                var widthValue = GridItems.Rows[e.RowIndex].Cells[3].Value;
                var lengthValue = GridItems.Rows[e.RowIndex].Cells["largo_restante"].Value;
                var tipo_movi = GridItems.Rows[e.RowIndex].Cells["tipo_mov"].Value?.ToString();



                if (rollIdValue != null && productIdValue != null && productNameValue != null &&
                    widthValue != null && lengthValue != null && tipo_movi != null)
                {
                    MasterRoll = new()
                    {
                        Roll_Id = rollIdValue,
                        Product_Id = productIdValue,
                        Product_Name = productNameValue,
                        Width = Convert.ToDouble(widthValue),
                        Length = Convert.ToDouble(lengthValue),
                        Tipo_mov = tipo_movi,
                    };
                    this.Close();
                }
                else
                {
                    MessageBox.Show("Algunos valores de las celdas son nulos. Por favor, verifique los datos.", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
            }
        }

        private async void Btn_reload_Click(object sender, EventArgs e)
        {
            DtRollid = await ProduccionService.LoadDataRollID();
            Dv = DtRollid.DefaultView;
            GridItems.DataSource = DtRollid;
            RefreshForms();
            txt_buscar.Text = string.Empty; // Clear the search text    

        }

        private void Btn_DetailsConsumos_Click(object sender, EventArgs e)
        {
            Frm_DetailsConsumos frmDetails = new(ProduccionService)
            {
                Rollid = GridItems.CurrentRow?.Cells["rollid"].Value?.ToString() ?? string.Empty,
                Productid = GridItems.CurrentRow?.Cells["product_id"].Value?.ToString() ?? string.Empty,
                Product_Name = GridItems.CurrentRow?.Cells["product_name"].Value?.ToString() ?? string.Empty,
                Width_t = GridItems.CurrentRow?.Cells["Ancho"].Value?.ToString() ?? string.Empty,
                Length = GridItems.CurrentRow?.Cells["lenght"].Value?.ToString() ?? string.Empty

            };
            frmDetails.ShowDialog();
        }

        private void GridItems_CellFormatting(object sender, DataGridViewCellFormattingEventArgs e)
        {
            if (this.GridItems.Columns[e.ColumnIndex].Name == "estado")
            {
                try
                {
                    string estado = Convert.ToString(e.Value)!;
                    if (estado == "Agotado")
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.Red;
                        e.CellStyle.ForeColor = System.Drawing.Color.White;
                    }
                    if (estado == "Completo")
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.Green;
                        e.CellStyle.ForeColor = System.Drawing.Color.White;
                    }
                    if (estado == "Parcialmente Consumido")
                    {
                        e.CellStyle.BackColor = System.Drawing.Color.Orange;
                        e.CellStyle.ForeColor = System.Drawing.Color.White;
                    }
                }
                catch (Exception)
                {
                    e.CellStyle.BackColor = System.Drawing.Color.White;
                    e.CellStyle.ForeColor = System.Drawing.Color.Black;
                }
            }
        }

        private void Frm_RollId_FormClosed(object sender, FormClosedEventArgs e)
        {
            this.Dispose();
            DtRollid.Dispose();
            Dv.Dispose();
        }
    }
}


