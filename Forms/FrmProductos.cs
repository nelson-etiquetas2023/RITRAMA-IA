using System.Data;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProductsService;

using Sunny.UI;
namespace Ritrama2025.Forms
{
    public partial class FrmProductos : UIForm
    {
        IProductsService ProductsService;
        IExportDataService ExportDataService;
        public DataSet Ds = new();
        BindingSource Bs = new();
        DataRowView Row = null!;
        int EditMode = 0;
        Product producto { get; set; } = null!;

        public FrmProductos(IProductsService productsService, IExportDataService exportDataService)
        {
            InitializeComponent();
            ProductsService = productsService;
            ExportDataService = exportDataService;
        }

        private async void FrmProductos_Load(object sender, EventArgs e)
        {
            if (this.TopLevel)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(155, 45);
            }
            await LoadProductsAsync();
            DataBindindControls();
            Refreshform();
        }

        private void DataBindindControls()
        {
            Bs.DataSource = Ds;
            Bs.DataMember = "Dtproducts";
            txt_productname.DataBindings.Add("text", Bs, "product_name");
            txt_productdescription.DataBindings.Add("text", Bs, "product_descrip");
            txt_referencia.DataBindings.Add("text", Bs, "Product_Ref");
            txt_codebar.DataBindings.Add("text", Bs, "codebar");
            txt_precio.DataBindings.Add("text", Bs, "precio");
            txt_ratio.DataBindings.Add("text", Bs, "ratio");
            chk_product_anulado.DataBindings.Add("Checked", Bs, "anulado");
            rad_master.DataBindings.Add("Checked", Bs, "MasterRolls", true, DataSourceUpdateMode.OnPropertyChanged);
            rad_hoja.DataBindings.Add("Checked", Bs, "Resmas", true, DataSourceUpdateMode.OnPropertyChanged);
            rad_graphics.DataBindings.Add("Checked", Bs, "Graphics", true, DataSourceUpdateMode.OnPropertyChanged);
            rad_rollocortado.DataBindings.Add("Checked", Bs, "rollo_cortado", true, DataSourceUpdateMode.OnPropertyChanged);
            //check anulado.
            chk_product_anulado.DataBindings["Checked"]!.Format += (s, e) =>
            {
                if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
            };
            chk_product_anulado.DataBindings["Checked"]!.Parse += (s, e) =>
            {
                if (e.Value == null) e.Value = false;
            };
            //radio master.
            rad_master.DataBindings["Checked"]!.Format += (s, e) =>
            {
                if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
            };
            rad_master.DataBindings["Checked"]!.Parse += (s, e) =>
            {
                if (e.Value == null) e.Value = false;
            };
            //radio graphics.
            rad_graphics.DataBindings["Checked"]!.Format += (s, e) =>
            {
                if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
            };
            rad_graphics.DataBindings["Checked"]!.Parse += (s, e) =>
            {
                if (e.Value == null) e.Value = false;
            };
            //radio hojas.
            rad_hoja.DataBindings["Checked"]!.Format += (s, e) =>
            {
                if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
            };
            rad_hoja.DataBindings["Checked"]!.Parse += (s, e) =>
            {
                if (e.Value == null) e.Value = false;
            };
            //rollo cortado.
            rad_rollocortado.DataBindings["Checked"]!.Format += (s, e) =>
            {
                if (e.Value == DBNull.Value || e.Value == null) e.Value = false;
            };
            rad_rollocortado.DataBindings["Checked"]!.Parse += (s, e) =>
            {
                if (e.Value == null) e.Value = false;
            };
            //product_id validar
            AddBindingWithValidationTextBox(txt_partid, "Text", Bs, "product_id");
        }

        private void AddBindingWithValidationTextBox(TextBox textbox, string propertyName, BindingSource bs, string colname)
        {
            var binding = textbox.DataBindings.Add(propertyName, bs, colname, true, DataSourceUpdateMode.OnPropertyChanged);

            binding.Format += (s, e) =>
            {
                if (e.Value == DBNull.Value || e.Value == null)
                    e.Value = "";
            };

            binding.Parse += (s, e) =>
            {
                string? input = e.Value?.ToString()?.Trim();
                if (string.IsNullOrWhiteSpace(input))
                {
                    MessageBox.Show($"El campo {colname} no puede estar vac�o.", "Validaci�n", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                    e.Value = DBNull.Value;
                }
                else
                {
                    e.Value = input;
                }
            };
        }

        private void Refreshform()
        {
            lbl_contador.Text = "Registros: " + (Bs.Position + 1).ToString() + "-" + Bs.Count.ToString();
        }

        private async Task LoadProductsAsync()
        {
            try
            {
                UseWaitCursor = true;
                Enabled = false;
                using var cts = new CancellationTokenSource(TimeSpan.FromSeconds(30));
                Ds = await ProductsService.Load(cts.Token).ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Tiempo de espera agotado al cargar productos.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar productos: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                Enabled = true;
            }
        }

        [Obsolete("Usar LoadProductsAsync - mantiene compatibilidad")]
        private void GetProductsAsync() => _ = LoadProductsAsync();

        private void bot_siguiente_Click(object sender, EventArgs e)
        {
            Bs.Position = Bs.Position + 1;
            Refreshform();
        }

        private void bot_anterior_Click(object sender, EventArgs e)
        {
            Bs.Position = Bs.Position - 1;
            Refreshform();
        }

        private void bot_ultimo_Click(object sender, EventArgs e)
        {
            Bs.Position = Bs.Count - 1;
            Refreshform();
        }

        private void bot_primero_Click(object sender, EventArgs e)
        {
            Bs.Position = 0;
            Refreshform();
        }

        private void FrmProductos_FormClosing(object sender, FormClosingEventArgs e)
        {
            Ds.Tables.Clear();
            Ds.Dispose();
        }

        private void bot_nuevo_Click(object sender, EventArgs e)
        {
            OpenEditForm();
            Row = (DataRowView)Bs.AddNew()!;
            Row.BeginEdit();
            Row["product_id"] = "";
            EditMode = 1; //Modo Add
        }
        private void OpenEditForm()
        {
            txt_partid.ReadOnly = false;
            txt_productname.ReadOnly = false;
            txt_productdescription.ReadOnly = false;
            txt_referencia.ReadOnly = false;
            txt_codebar.ReadOnly = false;
            txt_precio.ReadOnly = false;
            txt_ratio.ReadOnly = false;
            rad_master.Enabled = true;
            rad_hoja.Enabled = true;
            rad_rollocortado.Enabled = true;
            rad_graphics.Enabled = true;
            //solo en add
            if (EditMode == 1)
            {
                rad_graphics.Checked = false;
                rad_hoja.Checked = false;
                rad_master.Checked = false;
                rad_rollocortado.Checked = false;
            }
            //toolsbar changes.
            bot_siguiente.Enabled = false;
            bot_anterior.Enabled = false;
            bot_primero.Enabled = false;
            bot_ultimo.Enabled = false;
            bot_guardar.Enabled = true;
            bot_buscar.Enabled = false;
            bot_print.Enabled = false;
            bot_nuevo.Enabled = false;
            bot_excel.Enabled = false;
            bot_cancelar.Enabled = true;
            btn_update.Enabled = false;
        }

        private void bot_guardar_Click(object sender, EventArgs e)
        {
            //validar
            if (!ValidModelProduct()) return;

            //crear producto
            producto = CREATE_OBJECT_PRODUCT();

            if (EditMode == 1)
            {
                _ = SaveAdd();
            }
            if (EditMode == 2)
            {
                _ = SaveUpdate();
            }
            //cerrar formulario
            CloseFormUI();
            Bs.EndEdit();
            Refreshform();
            btn_update.Enabled = true;
            EditMode = 0;
        }
        private async Task SaveUpdate()
        {
            //llamar al servicio que modifica el producto
            bool ok = await ProductsService.Update(producto);
            if (ok) MessageBox.Show("Producto modificado satisfactoriamente...");
        }

        private async Task SaveAdd()
        {
            //verificar que no se repita el codigo
            if (ProductsService.ValidProductid(txt_partid.Text))
            {
                MessageBox.Show("el codigo del producto que acaba de introducir ya lo esta utilizando otro producto existente...");
                return;
            }
            //guardar base de datos.
            bool ok = await ProductsService.Add(producto);
            if (ok) MessageBox.Show("producto nuevo guardado satisfactoriamente...");
        }

        private Product CREATE_OBJECT_PRODUCT()
        {
            Product producto = new()
            {
                Product_id = txt_partid.Text,
                Product_Name = txt_productname.Text,
                Product_Description = txt_productdescription.Text,
                Referencia = txt_referencia.Text,
                Codigo_Barra = txt_codebar.Text,
                Precio = Convert.ToDecimal(txt_precio.Text == "" || txt_precio.Text is null ? 0 : txt_precio.Text),
                Ratio = Convert.ToDecimal(txt_ratio.Text == "" || txt_ratio.Text is null ? 0 : txt_ratio.Text),
                Master = rad_master.Checked,
                Graphics = rad_graphics.Checked,
                Hoja = rad_hoja.Checked,
                RolloCortado = rad_rollocortado.Checked,
                Anulado = false
            };
            return producto;
        }

        private void CloseFormUI()
        {
            //toolsbar
            bot_anterior.Enabled = true;
            bot_primero.Enabled = true;
            bot_siguiente.Enabled = true;
            bot_ultimo.Enabled = true;
            bot_nuevo.Enabled = true;
            bot_guardar.Enabled = false;
            bot_cancelar.Enabled = false;
            bot_excel.Enabled = true;
            bot_print.Enabled = true;
            bot_buscar.Enabled = true;
            //textboxs
            txt_partid.ReadOnly = true;
            txt_productname.ReadOnly = true;
            txt_productdescription.ReadOnly = true;
            txt_precio.ReadOnly = true;
            txt_ratio.ReadOnly = true;
            txt_codebar.ReadOnly = true;
            txt_referencia.ReadOnly = true;
            //radios
            rad_graphics.Enabled = false;
            rad_hoja.Enabled = false;
            rad_master.Enabled = false;
            rad_rollocortado.Enabled = false;
        }

        private bool ValidModelProduct()
        {
            bool valid = true;
            if (txt_partid.Text == "")
            {
                MessageBox.Show("debe introducir el codigo del producto!");
                valid = false;
                return valid;
            }
            if (txt_productname.Text == "")
            {
                MessageBox.Show("debe introducir el nombre del producto!");
                valid = false;
                return valid;
            }
            if (txt_productdescription.Text == "")
            {
                MessageBox.Show("debe introducir la descripcuion del producto!");
                valid = false;
                return valid;
            }
            if (!rad_master.Checked && !rad_graphics.Checked && !rad_hoja.Checked && !rad_rollocortado.Checked)
            {
                MessageBox.Show("debe seleccionar el tipo de producto!");
                valid = false;
                return valid;
            }

            return valid;
        }

        private void txt_precio_TextChanged(object sender, EventArgs e)
        {

        }

        private void bot_cancelar_Click(object sender, EventArgs e)
        {
            DataRowView RowCurrent;
            RowCurrent = (DataRowView)Bs.Current!;
            RowCurrent.Row.Delete();
            Bs.EndEdit();
            Bs.Position = 0;
            //Close Form.
            CloseFormUI();
            btn_update.Enabled = true;
            Refreshform();
        }

        private void bot_buscar_Click(object sender, EventArgs e)
        {
            Frm_ProductSeach buscador = new()
            {
                DtItems = Ds.Tables["Dtproducts"]!
            };
            buscador.ShowDialog();
            if (buscador.Selected_ProductID != "" && buscador.Selected_ProductID != null)
            {
                int busqueda = Bs.Find("product_id", buscador.Selected_ProductID);
                if (busqueda > 0)
                {
                    Bs.Position = busqueda;
                }
                else
                {
                    MessageBox.Show("No se encontro el numero del documento...", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void btn_update_Click(object sender, EventArgs e)
        {
            OpenEditForm();
            btn_update.Enabled = false;
            txt_partid.ReadOnly = true;
            EditMode = 2;
        }

        private void bot_excel_Click(object sender, EventArgs e)
        {
            var lista = new List<Product>();
            foreach (DataRow row in Ds.Tables["Dtproducts"]!.Rows)
            {
                var producto = new Product
                {
                    Product_id = row["product_id"].ToString()!,
                    Product_Name = row["product_name"].ToString()!
                };
                lista.Add(producto);
            }

            ExportDataService.ExportToExcelProducts(lista, "products.xlsx");





        }
    }
}


