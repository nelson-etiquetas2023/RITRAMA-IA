using System.ComponentModel;
using System.Data;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Services.CommonService;


using Sunny.UI;
namespace Ritrama2025.Forms.Seleccion
{
    public partial class FrmSeleccion : UIForm
    {
        private static readonly Dictionary<string, (String IdCol, string DesCol, string TypeCol)> Columnas = new()
        {
            ["clientes"] = ("customer_id", "customer_name", ""),
            ["Vendedores"] = ("vendor_id", "vendor_name", ""),
            ["Transporte"] = ("transport_id", "transport_name", ""),
            ["chofer"] = ("chofer_id", "chofer_name", ""),
            ["camion"] = ("placas_id", "camion_name", ""),
            ["operadores"] = ("operador_id", "nombre", ""),
            ["Proveedor"] = ("proveedor_id", "proveedor_name", ""),
            ["Persona"] = ("person_id", "person_name", ""),
            ["Producto"] = ("product_id", "product_name", "tipo")
        };
        readonly ICommonService service;

        public FrmSeleccion(ICommonService service)
        {
            InitializeComponent();
            this.service = service;
        }
        //readonly CommonService service = new();

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public DataTable DtItems { get; set; } = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Id { get; set; } = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Description { get; set; } = null!;

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Titulo { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string Tipo { get; set; } = null!;

        DataView Dv = new();
        string colname1 = null!;
        string colname2 = null!;
        string colname3 = null!;

        private void Seleccion_Load(object sender, EventArgs e)
        {
            Dv = DtItems.DefaultView;
            Grid_Items.AutoGenerateColumns = false;
            Dv.RowFilter = "";
            Grid_Items.DataSource = Dv;
            Numero_reg.Text = Convert.ToString(Dv.Count) + " Registro Encontrados";
            titleform.Text = Titulo;
            bot_buscar.Focus();
            if (Columnas.TryGetValue(Titulo, out var cols))
            {
                colname1 = cols.IdCol;
                colname2 = cols.DesCol;
                if (Titulo == "Producto")
                {
                    colname3 = cols.TypeCol;
                }
            }
            EstilosGrid();
        }
        private void BuscarItems()
        {
            if (ra_id.Checked)
            {
                Dv.RowFilter = colname1 + " like '%" + txt_buscar.Text + "%'";
            }
            if (ra_description.Checked)
            {
                Dv.RowFilter = colname2 + " like '%" + txt_buscar.Text + "%'";
            }

        }
        private void EstilosGrid()
        {
            DataGridViewTextBoxColumn col1 = new()
            {
                Name = colname1,
                Width = 70,
                HeaderText = "C�digo",
                DataPropertyName = colname1
            };
            Grid_Items.Columns.Add(col1);
            DataGridViewTextBoxColumn col2 = new()
            {
                Name = colname2,
                Width = 280,
                HeaderText = "Descripci�n",
                DataPropertyName = colname2
            };
            Grid_Items.Columns.Add(col2);
            if (Titulo == "Producto")
            {
                DataGridViewTextBoxColumn col3 = new()
                {
                    Name = colname3,
                    Width = 70,
                    HeaderText = "Tipo",
                    DataPropertyName = colname3,
                };
                Grid_Items.Columns.Add(col3);
            }
        }

        private void Bot_buscar_Click(object sender, EventArgs e)
        {
            BuscarItems();
        }

        private void Grid_Items_CellMouseDoubleClick(object sender, DataGridViewCellMouseEventArgs e)
        {
            if (e.RowIndex < 0)
            {
                return;
            }
            Id = Grid_Items.Rows[e.RowIndex].Cells[0].Value!.ToString()!;
            Description = Grid_Items.Rows[e.RowIndex].Cells[1].Value!.ToString()!;
            if (Titulo == "Producto") Tipo = Grid_Items.Rows[e.RowIndex].Cells[2].Value!.ToString()!;

            this.Close();
        }

        private void Btn_add_new_Click(object sender, EventArgs e)
        {
            if (Titulo is "Transporte" or "chofer" or "camion" or "Proveedor" or "Persona" or "Producto" or "operadores" or "clientes" or "Vendedores")
            {
                var fromNew = new Frm_AddNew(service)
                {
                    TitleForm = $"Agregar {Titulo}",
                    Dt = DtItems,
                    NombreEntidad = Titulo
                };
                fromNew.ShowDialog();
                DtItems = fromNew.Dt;
            }
        }

        private void Btn_delete_row_Click(object sender, EventArgs e)
        {
            //procedimiento para eliminar un registro seleccionado.
            if (MessageBox.Show("Desea Eliminar este registro (S/N)", "Confirmar", MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                //se actualiza la ui de la aplicaci�n.
                DataRowView row = Dv[Grid_Items.CurrentRow!.Index];

                if (Titulo == "Transporte")
                {
                    string id = row["transport_id"].ToString()!;
                    service.DeleteTransportEntity(id);
                }
                if (Titulo == "Chofer")
                {
                    string id = row["chofer_id"].ToString()!;
                    service.DeleteChoferEntity(id);
                }
                if (Titulo == "Camion")
                {
                    string id = row["placas_id"].ToString()!;
                    service.DeleteCamionEntity(id);
                }
                if (Titulo == "Proveedor")
                {
                    string id = row["proveedor_id"].ToString()!;
                    service.DeleteProvaiderEntity(id);
                }
                if (Titulo == "operadores")
                {
                    string id = row["operador_id"].ToString()!;
                    service.DeleteOperatorEntity(id);
                }
                //borro solo al final.
                DtItems.Rows.Remove(row.Row);
                DtItems.AcceptChanges();
            }
        }
    }
}


