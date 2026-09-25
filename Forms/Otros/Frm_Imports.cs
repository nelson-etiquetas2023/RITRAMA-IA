using System.ComponentModel;
using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using Microsoft.Data.SqlClient;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.InventarioService;

using Sunny.UI;
namespace Ritrama2025.Forms.Otros
{
    public partial class Frm_Imports : UIForm
    {
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PathFileName { get; set; } = null!;

        private IInventarioService InventarioService { get; set; }
        readonly List<ProductMAP> lista = [];

        readonly List<Product> ListaProductsNotFound = [];

        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public StringBuilder ErrorsImporExcel { get; set; } = new();

        public Frm_Imports(IInventarioService inventarioService)
        {
            InventarioService = inventarioService;
            InitializeComponent();
        }

        private void Frm_Imports_Load(object sender, EventArgs e)
        {
            DefineColumnsGrid();
            txt_fileName.Text = FileName;
            txt_filePath.Text = PathFileName;
            txt_number_rows.Text = "0";
            txt_warning.Text = "0";
        }

        private void Btn_load_data_Click(object sender, EventArgs e)
        {

            Grid_Items.DataSource = "";
            LoadData();
            chk_saveproductsnotfound.Enabled = true;
            btn_accion.Enabled = true;
        }

        private void LoadData()
        {
            lista.Clear();
            string filePath = PathFileName;
            //string fileName = FileName;
            //leer la hoja de excel.
            try
            {
                using XLWorkbook workbook = new XLWorkbook(filePath);
                IXLWorksheet worksheet = workbook.Worksheet(1);
                //Empiezo en la fila 2 por los encabezados.
                IEnumerable<IXLRow> filas = worksheet.Rows().Skip(1);
                // recorro filas donde esta la data de la hoja.
                //crear el validador de excel.
                //var validator = new ExcelValidator();
                //1.- validaciones de las columnas
                foreach (IXLRow? item in filas)
                {
                    //ProductMAP producto = new()
                    //{
                    //    ItemNo = itemno++,
                    //    Product_Id = item.Cell(1).Value.ToString(),
                    //    Product_Name = item.Cell(2).Value.ToString(),
                    //    Rollid = item.Cell(3).Value.ToString(),
                    //    Width = validator.TryGetDouble(item.Cell(4), worksheet),
                    //    Length = validator.TryGetDouble(item.Cell(5), worksheet),
                    //    Splice = validator.TryGetInt(item.Cell(6), worksheet),
                    //    Fecha_Produccion = validator.TryGetDateTime(item.Cell(7), worksheet),
                    //    Factura = item.Cell(8).Value.ToString(),
                    //    Ubic = item.Cell(9).Value.ToString(),
                    //    Fecha_Llegada = validator.TryGetDateTime(item.Cell(10), worksheet),
                    //    Paleta = item.Cell(11).Value.ToString(),
                    //};
                    //lista.Add(producto);
                    //txt_log_notifications.Text = validator.Errores.ToString();
                }
                Grid_Items.DataSource = lista;
                //2.- validar productos que no existen en la base de datos
                if (chk_valid_products.Checked)
                {
                    ProductsNotFoundDB();
                }
                //3.- Validar que rollid no se repitan. 

                string filasduplex = "";

                List<IGrouping<string, ProductMAP>> rollid_duplex = lista.GroupBy(r => r.Rollid)
                    .Where(g => g.Count() > 1).ToList();

                if (rollid_duplex.Count != 0)
                {
                    foreach (IGrouping<string, ProductMAP> grupo in rollid_duplex)
                    {

                        filasduplex = string.Join(",", grupo.Select(r => r.ItemNo.ToString()));


                        txt_log_notifications.Text += $"El rollid {grupo.Key} " +
                            $"se repite {grupo.Count()} veces en las filas {filasduplex}.\n";
                    }
                }


                //NUMERO DE ERRORES    
                //int numeroDeLineas = (validator.Errores.ToString().Split(Environment.NewLine).Length) - 1;
                //txt_errors.Text = numeroDeLineas.ToString();
                MessageBox.Show("Se Cargo los datos con Exito...!");
            }
            catch (System.IO.IOException ex)
            {
                MessageBox.Show("Error al tratar de abrir la hoja de excel, " +
                    "si esta abierta por favor cierrela y vuelva a intentarlo...[error code:] " + ex.Message);
            }
            txt_number_rows.Text = Grid_Items.Rows.Count.ToString();
        }

        private void SaveProductsNotDFoundDB()
        {
            //recorrer la lista de productos no encotrados.
            foreach (Product item in ListaProductsNotFound)
            {
                Product producto = new Product
                {
                    Product_id = item.Product_id,
                    Product_Name = item.Product_Name,
                    Product_Description = item.Product_Name,
                    Master = true,
                    Anulado = false,
                    Hoja = false,
                    Graphics = false,
                    RolloCortado = false,
                };
                try
                {
                    InventarioService.InsertProduct(producto);
                }
                catch (SqlException ex)
                {
                    MessageBox.Show("Error al guardar el producto en la base de datos, [error code:] " + ex.Message);
                }

                //notificar que se creo el producto.
                txt_log_notifications.Text = $"Se creo el producto {producto.Product_id} " +
                    $"- {producto.Product_Name} en la base de datos." + Environment.NewLine;

                MessageBox.Show("Se actualizaron los productos del sistema...");
            }
        }

        private void Btn_saveDatabase_Click(object sender, EventArgs e)
        {
            int errors = int.Parse(txt_errors.Text);

            if (errors > 0)
            {
                MessageBox.Show("No se pueden Guardar los datos mientra la hoja de excel tenga errores en los datos...");
                return;

            }
            //Guardar en Base de Datos.
            //InventarioService.SaveMasterInitialDB(lista);
        }
        private void ProductsNotFoundDB()
        {
            string messageProduct = "";

            foreach (ProductMAP item in lista)
            {
                //verifico si existe en la base de datos.
                if (!InventarioService.ValidProductid(item.Product_Id))
                {
                    messageProduct = "-> PRODUCTO NO EXISTE: " + " [ " + item.Product_Id + " - "
                    + item.Product_Name + " ] " + Environment.NewLine;

                    //crear la notificacion.
                    txt_log_notifications.Text += messageProduct;

                    //Agrego a una lista de productos no encontrados.
                    ListaProductsNotFound.Add(new Product
                    {
                        Product_id = item.Product_Id,
                        Product_Name = item.Product_Name,
                    });
                }
            }
        }
        private void DefineColumnsGrid()
        {
            Grid_Items.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("item", 30, "It.", "itemNo", Grid_Items);
            CommonService.ADD_COLUMN_GRID("product_id", 50, "Product Id.", "product_id", Grid_Items);
            CommonService.ADD_COLUMN_GRID("product_name", 300, "Product Name", "product_name", Grid_Items);
            CommonService.ADD_COLUMN_GRID("rollid", 70, "Roll-Id", "rollid", Grid_Items);
            CommonService.ADD_COLUMN_GRID("width", 70, "Width [Inch.]", "Width", Grid_Items);
            CommonService.ADD_COLUMN_GRID("lenght", 70, "Length [Pies.]", "length", Grid_Items);
            CommonService.ADD_COLUMN_GRID("splice", 70, "Splice", "splice", Grid_Items);
            CommonService.ADD_COLUMN_GRID("fecha_fabricacion", 70, "Fecha Produccion", "fecha_produccion", Grid_Items);
            CommonService.ADD_COLUMN_GRID("recep", 70, "Recepcion", "factura", Grid_Items);
            CommonService.ADD_COLUMN_GRID("ubic", 70, "Ubicacion", "ubic", Grid_Items);
            CommonService.ADD_COLUMN_GRID("fecha_llegada", 70, "Fecha Llegada", "fecha_llegada", Grid_Items);
            CommonService.ADD_COLUMN_GRID("paleta", 70, "Paleta", "paleta", Grid_Items);
        }
        private void Btn_accion_Click(object sender, EventArgs e)
        {
            //Guardar los productos no encontrados en la base de datos.
            if (chk_saveproductsnotfound.Checked)
            {
                SaveProductsNotDFoundDB();
            }
        }

        private void Grid_Items_CellContentClick(object sender, DataGridViewCellEventArgs e)
        {

        }
    }
}


