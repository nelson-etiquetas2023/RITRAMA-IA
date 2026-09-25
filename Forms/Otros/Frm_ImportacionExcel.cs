using System.ComponentModel;
using ClosedXML.Excel;
using Ritrama2025.Models;

using Sunny.UI;
namespace Ritrama2025.Forms.Otros
{
    public partial class Frm_ImportacionExcel : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PathFileName { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public List<TemplateMasterExcel> lista = [];

        public Frm_ImportacionExcel()
        {
            InitializeComponent();
        }

        private void Bot_buscar_Click(object sender, EventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                Title = "Select an Excel File"
            };

            dialog.ShowDialog();

            PathFileName = dialog.FileName;
            FileName = Path.GetFileName(PathFileName);

            txt_name_file.Text = FileName;
            txt_path_file.Text = PathFileName;

        }

        private void Frm_ImportacionExcel_Load(object sender, EventArgs e)
        {

        }

        private void bot_cargar_Click(object sender, EventArgs e)
        {
            try
            {
                using XLWorkbook workbook = new XLWorkbook(PathFileName);
                IXLWorksheet worksheet = workbook.Worksheet(1);
                //Empiezo en la fila 2 por los encabezados.
                IEnumerable<IXLRow> filas = worksheet.Rows().Skip(1);
                // recorro filas donde esta la data de la hoja.


                foreach (IXLRow? item in filas)
                {
                    TemplateMasterExcel master = new()
                    {
                        product_id = item.Cell(1).Value.ToString(),
                        product_name = item.Cell(2).Value.ToString(),
                        rollid = item.Cell(3).Value.ToString(),
                        width = item.Cell(4).GetDouble(),
                        length = item.Cell(5).GetDouble(),
                        num_empalme = item.Cell(6).Value.ToString(),
                        fecha_produccion = item.Cell(7).GetDateTime(),
                        factura = item.Cell(8).Value.ToString(),
                        ubicacion = item.Cell(9).Value.ToString(),
                        palet_num = item.Cell(10).Value.ToString(),
                        fecha_llegada = item.Cell(11).GetDateTime(),
                    };
                    lista.Add(master);
                }
                grid_items.DataSource = lista;
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Error al tratar de abrir la hoja de excel, si esta abierta por favor cierrela y vuelva a intentarlo...[error code:] " + ex.Message);
                throw;
            }
            counts_rows.Text = grid_items.Rows.Count.ToString();
        }

        private void bot_guardar_Click(object sender, EventArgs e)
        {
            Close();
        }
    }
}


