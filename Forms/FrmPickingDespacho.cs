using System.ComponentModel;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;

using Sunny.UI;
namespace Ritrama2025.Forms
{
    public partial class FrmPickingDespacho : UIForm
    {

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<RolloCortado> Lista_Rollos { get; set; } = [];

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Visible)]
        public List<ItemsDespacho> Lista_Items { get; set; } = [];

        readonly List<Recepcion> Lista_Hojas = [];
        readonly List<Recepcion> Lista_Graphics = [];
        readonly List<Recepcion> Lista_Master = [];
        public ICommonService Servicio = null!;


        public FrmPickingDespacho(ICommonService servicio)
        {
            InitializeComponent();
            Servicio = servicio;
        }


        private void FrmPickingDespacho_Load(object sender, EventArgs e)
        {
            EstilosGrid();
            DefColumnsGroupItems();
            grid_detallerc.DataSource = Lista_Rollos.ToList();
        }

        private async void Button2_Click(object sender, EventArgs e)
        {
            // dotnet-winforms-basics: async handler con await - no .Result (gotcha #5)
            Lista_Graphics.RemoveAll(r => r.Palet_cant >= 0m);
            Lista_Hojas.RemoveAll(r => r.Palet_cant >= 0m);
            Lista_Master.RemoveAll(r => r.Palet_cant >= 0m);

            OpenFileDialog openFileDialog = new()
            {
                InitialDirectory = @"\Users\siste\OneDrive\Documentos\APPRITRAMA\data",
                Title = "Selecciona el archivo TXT para cargalo al despacho",
                CheckFileExists = true,
                CheckPathExists = true,
                DefaultExt = "txt",
                Filter = "Archivos TXT (*.txt)|*.txt",
                FilterIndex = 2,
                RestoreDirectory = true,
                ReadOnlyChecked = true,
                ShowReadOnly = true,
            };
            if (openFileDialog.ShowDialog() != DialogResult.OK)
            {
                return;
            }
            if (RA_CORTADO.Checked)
            {
                BOT_LEER_TXT.Enabled = false;
                UseWaitCursor = true;
                try
                {
                    List<RolloCortado> items = await ExtraerDataAppMovil(openFileDialog.FileName).ConfigureAwait(true);

                    foreach (RolloCortado item in items)
                    {
                        if (!Lista_Rollos.Any(r => r.UniqueCode == item.UniqueCode))
                        {
                            Lista_Rollos.Add(item);
                        }
                    }

                    grid_detallerc.DataSource = null;
                    grid_detallerc.DataSource = Lista_Rollos.ToList();

                    CountRowsGrid();

                    grid_renglones.DataSource = QueryItemsGrouping();
                    Lista_Items = [.. QueryItemsGrouping()];
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Error al cargar picking: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                }
                finally
                {
                    UseWaitCursor = false;
                    BOT_LEER_TXT.Enabled = true;
                }
            }
        }

        private void DefColumnsGroupItems()
        {
            grid_renglones.AutoGenerateColumns = false;
            AGREGAR_COLUMN_GRID("product_id", 60, "Product Id.", "product_id", grid_renglones);
            AGREGAR_COLUMN_GRID("product_name", 180, "Nombre del Producto", "product_name", grid_renglones);
            AGREGAR_COLUMN_GRID("unidad", 70, "Unidad", "unidad", grid_renglones);
            AGREGAR_COLUMN_GRID("cantidad", 80, "Cantidad", "cantidad", grid_renglones);
            AGREGAR_COLUMN_GRID("width", 80, "Width", "width", grid_renglones);
            AGREGAR_COLUMN_GRID("Lenght", 80, "Largo", "Lenght", grid_renglones);
            AGREGAR_COLUMN_GRID("msi", 80, "Msi", "msi", grid_renglones);
            AGREGAR_COLUMN_GRID("Code_Person", 80, "Msi", "Code_Person", grid_renglones);

        }

        private IEnumerable<ItemsDespacho> QueryItemsGrouping()
        {

            IEnumerable<ItemsDespacho> Query = from p in Lista_Rollos
                                               orderby p.Product_Id descending
                                               orderby p.Width
                                               group p by new { p.Product_Id, p.Width, p.Length } into g
                                               select new ItemsDespacho
                                               {
                                                   Product_id = g.First().Product_Id,
                                                   Product_name = g.First().Product_Name,
                                                   Cantidad = g.Count(),
                                                   Width = Convert.ToDecimal(g.First().Width),
                                                   Unidad = "ROLLO",
                                                   Lenght = Convert.ToDecimal(g.First().Length),
                                                   Msi = Convert.ToDecimal(g.First().Msi),
                                                   Code_Person = g.First().Code_Person
                                               };
            return Query.ToList();
        }

        private void EstilosGrid()
        {
            grid_detallerc.AutoGenerateColumns = false;
            //grid de rollo detalle rc
            AGREGAR_COLUMN_GRID("it", 30, "it.", "", grid_detallerc);
            AGREGAR_COLUMN_GRID("UniqueCode", 65, "Codigo Unico", "UniqueCode", grid_detallerc);
            AGREGAR_COLUMN_GRID("product_id", 60, "Product Id.", "product_id", grid_detallerc);
            AGREGAR_COLUMN_GRID("product_name", 180, "Nombre del Producto", "product_name", grid_detallerc);
            AGREGAR_COLUMN_GRID("RollNumber", 60, "Roll Number", "RollNumber", grid_detallerc);
            AGREGAR_COLUMN_GRID("width", 50, "Width", "width", grid_detallerc);
            AGREGAR_COLUMN_GRID("Length", 58, "Largo", "Length", grid_detallerc);
            AGREGAR_COLUMN_GRID("msi", 54, "Msi", "msi", grid_detallerc);
            AGREGAR_COLUMN_GRID("splice", 50, "Splice", "splice", grid_detallerc);
            AGREGAR_COLUMN_GRID("roll_id", 72, "Roll Id.", "roll_id", grid_detallerc);
            AGREGAR_COLUMN_GRID("code_person", 100, "Codigo Perso.", "code_person", grid_detallerc);
            AGREGAR_COLUMN_GRID("status", 100, "Estatus", "status", grid_detallerc);


            grid_detallerc.Columns[3].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid_detallerc.Columns[4].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid_detallerc.Columns[4].DefaultCellStyle.Format = "###,##0.00";
            grid_detallerc.Columns[5].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid_detallerc.Columns[5].DefaultCellStyle.Format = "###,##0.00";
            grid_detallerc.Columns[6].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleRight;
            grid_detallerc.Columns[6].DefaultCellStyle.Format = "###,##0.00";
            grid_detallerc.Columns[7].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            grid_detallerc.Columns[7].DefaultCellStyle.Format = "##";
            grid_detallerc.Columns[8].DefaultCellStyle.Format = "##";

            grid_detallerc.DataSource = Lista_Rollos;
        }

        public async Task<List<RolloCortado>> ExtraerDataAppMovil(string file)
        {
            //leer txt de picking
            List<RolloCortado> rollos = [];
            if (File.Exists(file))
            {
                try
                {
                    using StreamReader sr = new(file);
                    string? row;
                    while ((row = sr.ReadLine()) != null)
                    {
                        string code = row.Trim();
                        if (string.IsNullOrWhiteSpace(code))
                        {
                            continue;
                        }
                        // soporta codigos separados por coma y toma el primero
                        string uniqueCode = code.Split(',')[0].Trim();
                        if (string.IsNullOrWhiteSpace(uniqueCode))
                        {
                            continue;
                        }
                        RolloCortado rollo = new()
                        {
                            UniqueCode = uniqueCode,
                        };
                        rollos.Add(rollo);
                    }
                }
                catch (Exception ex2)
                {
                    MessageBox.Show("error al tratar de crear el txt de despacho" + ex2);
                }
            }
            //llenar la lista de rollo cortado.
            await Servicio.GetDataRolloCortado(rollos);

            rollos.RemoveAll(p => string.IsNullOrWhiteSpace(p.Product_Id));

            return rollos;
        }
        private static void AGREGAR_COLUMN_GRID(string name, int size, string title, string field_bd, DataGridView grid)
        {
            DataGridViewTextBoxColumn dataGridViewColumn = new()
            {
                Name = name,
                Width = size,
                HeaderText = title,
                DataPropertyName = field_bd
            };
            grid.Columns.Add(dataGridViewColumn);
        }

        private void BOT_DESPACHAR_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void Btn_buscar_Click(object sender, EventArgs e)
        {
            SearchCodeUnique();
        }

        private void SearchCodeUnique()
        {
            //SI ESTA VACIO NO ENTRA
            if (txt_codigo.Text == "")
            {
                return;
            }

            //HACER LA BUSQUEDA DE RC
            RolloCortado rollo = Servicio.SearchCodigoUnico(txt_codigo.Text);

            //NO ENCONTRADO
            if (rollo!.UniqueCode == null)
            {
                MessageBox.Show("el codigo no fue encontrado...");
                return;
            }

            //SI EL ROLLO YA FUE DESPACHADO
            if (rollo.Disponible == false)
            {
                MessageBox.Show("ROLLO FUE DESPACHADO...!");
                return;
            }

            //SI YA EXISTE EN LA LISTA
            if (Lista_Rollos.Any(r => r.UniqueCode == txt_codigo.Text))
            {
                MessageBox.Show("rollo ya esta en la lista...");
                return;
            }
            //ACTUALIZAR LA UI.
            Lista_Rollos.Add(rollo);
            grid_detallerc.DataSource = null;
            grid_detallerc.DataSource = Lista_Rollos.ToList();
            CountRowsGrid();

            //BALNQUEAR TEXTBOX DE BUSQUEDA.
            txt_codigo.Text = "";

            //Actualizar el grid de group
            grid_renglones.DataSource = QueryItemsGrouping();
            Lista_Items = [.. QueryItemsGrouping()];
        }




        private void CountRowsGrid()
        {
            //CALCULAR LOS NUMEROS DE FILAS
            for (int i = 0; i < grid_detallerc.Rows.Count; i++)
            {
                grid_detallerc.Rows[i].Cells[0].Value = i + 1;
            }

        }

        private void txt_codigo_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                SearchCodeUnique();
            }
        }
    }
}


