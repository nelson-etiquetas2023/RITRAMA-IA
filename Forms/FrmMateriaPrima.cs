using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using ClosedXML.Excel;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Forms.Seleccion;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.InventarioService;
using Ritrama2025.Services.MateriaPrima;
using Ritrama2025.Services.ReportsService.ReportsService;

using Sunny.UI;
namespace Ritrama2025.Forms
{
    public partial class FrmMateriaPrima : UIForm
    {
        public readonly IServiceMateriaPrima Services;
        public readonly IExportDataService ExportDataService;
        public readonly IReportsService ReportService;
        public readonly IServiceCommonData ServiceCommonData;
        public readonly ICommonService CommonService;
        IInventarioService InventarioService { get; set; }

        public DataSet Ds = new();
        readonly BindingSource Bs = [];
        readonly BindingSource BsDetalle = [];
        private DataRowView ParentRow = null!;
        private DataRowView ChildsRows = null!;
        string EditMode = "READ";
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string PathFileName { get; set; } = null!;
        [DesignerSerializationVisibility(DesignerSerializationVisibility.Hidden)]
        public string FileName { get; set; } = null!;

        public List<TemplateMasterExcel> ListExcel = [];

        public FrmMateriaPrima(IInventarioService inventarioService, IServiceMateriaPrima Services, IExportDataService exportDataService, IReportsService reportService, IServiceCommonData serviceCommonData, ICommonService commonService)
        {
            InitializeComponent();
            this.Services = Services;
            this.ExportDataService = exportDataService;
            this.ReportService = reportService;
            this.ServiceCommonData = serviceCommonData;
            this.InventarioService = inventarioService;
            this.CommonService = commonService;
        }

        private async void FrmMateriaPrima_Load(object sender, EventArgs e)
        {
            if (this.TopLevel)
            {
                this.StartPosition = FormStartPosition.Manual;
                this.Location = new Point(155, 45);
            }
            await LoadDataAsync();
            BindDataSource();
            BindingControls();
            RefreshDocument();
        }
        private void RefreshDocument()
        {
            label_counter_rows.Text = $"Registros: {Bs.Count}";
            Bs.Sort = "numero DESC";
            string basePath = AppContext.BaseDirectory;
            string ruta = Path.Combine(basePath, "Images");

            string estado = !chk_DocumentClose.Checked ? "abierto" : "cerrado";
            if (chk_anulado.Checked) estado = "anulado";

            switch (estado)
            {
                case "abierto":
                    //Pic_Document.Image = Image.FromFile(ruta + @"\open_document.png");
                    break;
                case "cerrado":
                    //Pic_Document.Image = Image.FromFile(ruta + @"\close_document.png");
                    break;
                case "anulado":
                    //Pic_Document.Image = Image.FromFile(ruta + @"\anulado_documento.png");
                    break;
            }
        }
        private void BindDataSource()
        {
            //Configuracion del BindingSource.
            Bs.DataSource = Ds;
            Bs.DataMember = "DtMateria";
            //Bs.Sort = "numero DESC";
            //Bindingsource para el detalle de los productos.
            BsDetalle.DataSource = Bs;
            BsDetalle.DataMember = "FK_MASTER_DETAILS";
            GridItems.AutoGenerateColumns = false;
            ADD_COLUMN_GRID("product_id", 70, "Product Id.", "product_id", GridItems);
            ADD_COLUMN_GRID("product_name", 200, "Product Name.", "product_name", GridItems);
            ADD_COLUMN_GRID("rollid", 70, "Roll-Id.", "rollid", GridItems);
            ADD_COLUMN_GRID("width", 75, "Width [Inch.]", "width", GridItems);
            ADD_COLUMN_GRID("length", 75, "Length [Pies]", "length", GridItems);
            ADD_COLUMN_GRID("num_empalme", 75, "# Empalme", "empalme", GridItems);
            ADD_COLUMN_GRID("fecha_produccion", 85, "Fecha Produccion", "fecha_produccion", GridItems);
            ADD_COLUMN_GRID("factura", 85, "Factura", "factura", GridItems);
            ADD_COLUMN_GRID("ubicacion", 70, "Ubica.", "ubicacion", GridItems);
            ADD_COLUMN_GRID("num_paleta", 70, "Palet #", "num_paleta", GridItems);
            ADD_COLUMN_GRID("fecha_llegada", 70, "Fecha LLegada", "fecha_llegada", GridItems);

            GridItems.DataSource = BsDetalle;

        }
        private async Task LoadDataAsync()
        {
            try
            {
                UseWaitCursor = true;
                Enabled = false;
                Ds = await Services.LoadData().ConfigureAwait(true);
            }
            catch (OperationCanceledException)
            {
                MessageBox.Show("Tiempo de espera agotado al cargar materia prima.", "Timeout", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error al cargar materia prima: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            finally
            {
                UseWaitCursor = false;
                Enabled = true;
            }
        }

        private void BindingControls()
        {
            //trabajar con los enlaces a datos.
            txt_numeroOrden.DataBindings.Add("Text", Bs, "numero");
            txt_OrdenCompra.DataBindings.Add("Text", Bs, "orden_compra");
            txt_prov_Id.DataBindings.Add("Text", Bs, "proveedor_id");
            txt_nombre_prov.DataBindings.Add("Text", Bs, "proveedor_name");
            txt_fecha_produccion.DataBindings.Add("Text", Bs, "fecha_pro");
            txt_fecha_recepcion.DataBindings.Add("Text", Bs, "fecha_recepcion");
            txt_person_name.DataBindings.Add("Text", Bs, "persona_respons");
            txt_transport_id.DataBindings.Add("Text", Bs, "transport_id");
            txt_transport_name.DataBindings.Add("Text", Bs, "transport_name");
            txt_guia.DataBindings.Add("Text", Bs, "guia_import");
            txt_lote.DataBindings.Add("Text", Bs, "lote");
            txt_embarque.DataBindings.Add("Text", Bs, "doc_embarque");
            txt_total_cantidad.DataBindings.Add("Text", Bs, "total_cantidad");
            txt_notas.DataBindings.Add("Text", Bs, "notas");

            txt_person_id.DataBindings.Add("Text", Bs, "person_id");
            chk_DocumentClose.DataBindings.Add("Checked", Bs, "CloseDocument");
            chk_anulado.DataBindings.Add("Checked", Bs, "anulado");

        }
        private void StyleGridColumns()
        {
            GridItems.AutoGenerateColumns = false;
            //Configurar las columnas del detalle de los productos.
            ADD_COLUMN_GRID("product_id", 70, "Product Id.", "product_id", GridItems);
            ADD_COLUMN_GRID("product_name", 200, "Product Name.", "product_name", GridItems);
            GridItems.DataSource = BsDetalle;
        }

        private static void ADD_COLUMN_GRID(string name, int size, string title, string field_bd, DataGridView grid)
        {
            DataGridViewTextBoxColumn col = new()
            {
                Name = name,
                Width = size,
                HeaderText = title,
                DataPropertyName = field_bd,
            };
            grid.Columns.Add(col);
        }

        private void Btn_siguiente_Click(object sender, EventArgs e)
        {
            Bs.Position--;
            RefreshDocument();
        }

        private void Btn_anterior_Click(object sender, EventArgs e)
        {
            Bs.Position++;
            RefreshDocument();
        }

        private void Btn_ultimo_Click(object sender, EventArgs e)
        {
            Bs.Position = 0;
            RefreshDocument();
        }

        private void Btn_primero_Click(object sender, EventArgs e)
        {
            Bs.Position = Bs.Count - 1;
            RefreshDocument();
        }

        private void Btn_create_Click(object sender, EventArgs e)
        {
            EditMode = "ADDNEW";
            Bs.Sort = "";
            chk_anulado.DataBindings.Clear();
            chk_DocumentClose.DataBindings.Clear();
            chk_DocumentClose.Checked = false;
            chk_anulado.Checked = false;
            ParentRow = (DataRowView)Bs.AddNew()!;
            ParentRow.BeginEdit();
            ParentRow["numero"] = Services.LoadConsecOrden("CMP");
            ParentRow["total_cantidad"] = 0;
            ParentRow["CloseDocument"] = false;
            ParentRow["Anulado"] = false;
            ParentRow.EndEdit();
            AbrirFormulario();
            string basePath = AppContext.BaseDirectory;
            string ruta = Path.Combine(basePath, "Images");
            Pic_Document.Image?.Dispose();
            Pic_Document.Image = Image.FromFile(ruta + @"\add_document.png");
        }
        private void AbrirFormulario()
        {
            txt_OrdenCompra.ReadOnly = false;
            btn_ProvBuscar.Enabled = true;
            btn_TransportBuscar.Enabled = true;
            btn_RecepBuscar.Enabled = true;
            txt_fecha_produccion.Enabled = true;
            txt_fecha_recepcion.Enabled = true;
            txt_guia.ReadOnly = false;
            txt_lote.ReadOnly = false;
            txt_embarque.ReadOnly = false;
            txt_notas.ReadOnly = false;
            btn_addRows.Enabled = true;
            btn_deleteRows.Enabled = true;
            btn_save.Enabled = true;
            btn_cancel.Enabled = true;
            btn_siguiente.Enabled = false;
            btn_anterior.Enabled = false;
            btn_primero.Enabled = false;
            btn_ultimo.Enabled = false;
            btn_create.Enabled = false;
            txt_notas.BackColor = System.Drawing.Color.White;
            txt_notas.ReadOnly = false;
            btn_LoadRows.Enabled = true;
            btn_template.Enabled = true;
            btn_CloseDoc.Enabled = false;
            btn_AnularDoc.Enabled = false;
            btn_OrdenBuscar.Enabled = false;
            btn_printDoc.Enabled = false;
            btn_ExportDoc.Enabled = false;
            btn_SearchDoc.Enabled = false;
        }

        private void Btn_ProvBuscar_Click(object sender, EventArgs e)
        {
            FrmSeleccion SelVendor = new(CommonService)
            {
                DtItems = Ds.Tables["DtProvider"]!,
                Titulo = "Proveedor"
            };
            SelVendor.ShowDialog();
            txt_prov_Id.Text = SelVendor.Id;
            txt_nombre_prov.Text = SelVendor.Description;
        }

        private void Btn_TransportBuscar_Click(object sender, EventArgs e)
        {

            FrmSeleccion SelTransport = new(CommonService)
            {
                DtItems = Ds.Tables["DtTransport"]!,
                Titulo = "Transporte"
            };
            SelTransport.ShowDialog();
            txt_transport_id.Text = SelTransport.Id;
            txt_transport_name.Text = SelTransport.Description;
        }

        private void Btn_RecepBuscar_Click(object sender, EventArgs e)
        {
            FrmSeleccion SelPerson = new(CommonService)
            {
                DtItems = Ds.Tables["DtPerson"]!,
                Titulo = "Persona"
            };
            SelPerson.ShowDialog();
            txt_person_id.Text = SelPerson.Id;
            txt_person_name.Text = SelPerson.Description;
        }

        private void Btn_addRows_Click(object sender, EventArgs e)
        {

            FrmProductsInsert frmInsertRows = new(ServiceCommonData, CommonService)
            {
                DtItems = Ds.Tables["DtProducts"]!,
                Titulo = "Producto"
            };
            frmInsertRows.ShowDialog();

            string rollid_form = frmInsertRows.Producto.Rollid;
            bool IsNotcreate = false;

            foreach (DataGridViewRow row in GridItems.Rows)
            {
                var rollid_grid = row.Cells["rollid"].Value?.ToString();

                if (rollid_grid == rollid_form)
                {
                    IsNotcreate = true;
                    break;
                }
            }

            if (IsNotcreate)
            {
                MessageBox.Show("El roll-id ya esta en la lista, no se va ha crear...");
            }
            else
            {
                //Insertar el row en el GridItems.
                if (frmInsertRows.Producto != null)
                {
                    ChildsRows = (DataRowView)BsDetalle.AddNew()!;
                    ChildsRows.BeginEdit();
                    ChildsRows["numero"] = txt_numeroOrden.Text;
                    ChildsRows["product_id"] = frmInsertRows.Producto.Product_Id;
                    ChildsRows["product_name"] = frmInsertRows.Producto.Product_Name;
                    ChildsRows["type"] = frmInsertRows.Producto.Product_Type;
                    ChildsRows["width"] = frmInsertRows.Producto.Width;
                    ChildsRows["length"] = frmInsertRows.Producto.Length;
                    ChildsRows["msi"] = frmInsertRows.Producto.Msi;
                    ChildsRows["rollid"] = frmInsertRows.Producto.Rollid;
                    ChildsRows["splice"] = frmInsertRows.Producto.Splice;
                    ChildsRows["core"] = frmInsertRows.Producto.Core;
                    ChildsRows["ubicacion"] = frmInsertRows.Producto.Ubic;
                    ChildsRows["cant_pedido"] = frmInsertRows.Producto.Cant;
                    ChildsRows["cant_real"] = 0;
                    ChildsRows["empalme"] = 0;
                    ChildsRows["num_paleta"] = 0;
                    ChildsRows["fecha_produccion"] = DateTime.Now;
                    ChildsRows["fecha_llegada"] = DateTime.Now;
                    ChildsRows["factura"] = "0";
                    ChildsRows.Row.SetParentRow(((DataRowView)Bs.Current!).Row, Ds.Relations["FK_MASTER_DETAILS"]);
                    ChildsRows.EndEdit();
                    ContarFilas();
                }
            }
        }

        private void ContarFilas()
        {
            int filas = GridItems.Rows.Count;
            txt_total_cantidad.Text = filas.ToString();
        }

        private void Btn_save_Click(object sender, EventArgs e)
        {
            //validar los roll-id
            foreach (DataGridViewRow row in GridItems.Rows)
            {
                var rollid_grid = row.Cells["rollid"].Value?.ToString();
                if (!ServiceCommonData.VerificarRollIdNoRepeat(rollid_grid!)) return;
            }

            if (txt_OrdenCompra.Text == "")
            {
                MessageBox.Show("Introduzca un valor para la orden de compra...");
                return;
            }
            if (txt_prov_Id.Text == "")
            {
                MessageBox.Show("Introduzca un valor para el proveedor...");
                return;
            }
            if (txt_transport_id.Text == "")
            {
                MessageBox.Show("Introduzca un valor para el transportista...");
                return;
            }
            if (txt_person_id.Text == "")
            {
                MessageBox.Show("Introduzca un valor para el recepcionista...");
                return;
            }
            if (txt_guia.Text == "")
            {
                MessageBox.Show("Introduzca un valor para la guia de importaion...");
                return;
            }
            if (txt_embarque.Text == "")
            {
                MessageBox.Show("Introduzca un valor para el numero de embarque...");
                return;
            }
            if (txt_lote.Text == "")
            {
                MessageBox.Show("Introduzca un valor para el numero de lote...");
                return;
            }
            if (GridItems.Rows.Count == 0)
            {
                MessageBox.Show("Debe registras algunos productos para poder grabar la orden...");
                return;
            }



            if (EditMode == "ADDNEW")
            {
                SAVE_NEW();
            }
            else if (EditMode == "UPDATE")
            {
                //SAVE_UPDATE();
            }
        }

        private void SAVE_NEW()
        {
            EditMode = "EDIT";
            bool ok = Services.GuardarOrden(CREATE_ORDEN_OBJECT());
            if (ok) MessageBox.Show("se guardo correctamente...");
            int ProxConsec = Convert.ToInt16(txt_numeroOrden.Text) + 1;
            Services.UpdateConsecOrden(ProxConsec.ToString());
            btn_primero.Enabled = true;
            btn_ultimo.Enabled = true;
            btn_siguiente.Enabled = true;
            btn_anterior.Enabled = true;
            btn_save.Enabled = false;
            btn_cancel.Enabled = false;
            btn_create.Enabled = true;
            btn_addRows.Enabled = false;
            btn_deleteRows.Enabled = false;
            btn_template.Enabled = false;
            btn_LoadRows.Enabled = false;
            btn_CloseDoc.Enabled = true;
            btn_AnularDoc.Enabled = true;
            btn_SearchDoc.Enabled = true;
            btn_printDoc.Enabled = true;
            btn_ExportDoc.Enabled = true;
            RefreshDocument();
        }

        private OrdenMP CREATE_ORDEN_OBJECT()
        {
            //Header.
            OrdenMP Orden = new()
            {
                Numero = txt_numeroOrden.Text,
                Fecha_Recepcion = Convert.ToDateTime(txt_fecha_recepcion.Text),
                Fecha_Produccion = Convert.ToDateTime(txt_fecha_produccion.Text),
                Orden_Compra = txt_OrdenCompra.Text,
                Proveedor_id = Guid.Parse(txt_prov_Id.Text),
                Proveedor_name = txt_nombre_prov.Text,
                Transport_id = Guid.Parse(txt_transport_id.Text),
                Transport_name = txt_transport_name.Text,
                Guia = txt_guia.Text,
                Lote = txt_lote.Text,
                Numero_Embarque = txt_embarque.Text,
                Person_Id = Guid.Parse(txt_person_id.Text),
                Person_Name = txt_person_name.Text,
                CloseDocument = false,
                Notas = txt_notas.Text + Environment.NewLine + "Documento de Materia Prima Creado: " + Environment.NewLine + DateTime.Now,
                Renglones = Convert.ToInt32(txt_total_cantidad.Text),


            };
            //Items.
            foreach (DataGridViewRow Item in GridItems.Rows)
            {

                var ProductId = Item.Cells["product_id"].Value;
                var ProductName = Item.Cells["product_name"].Value;
                //var Product_Type = Item.Cells["product_type"].Value;
                var WidthMaster = Convert.ToDouble(Item.Cells["width"].Value);
                var LengthMaster = Convert.ToDouble(Item.Cells["length"].Value);
                //var MsiMaster = Convert.ToDouble(Item.Cells["msi"].Value);
                var RollId = Item.Cells["rollid"].Value!.ToString()!;
                //var Splice = Convert.ToInt16(Item.Cells["splice"].Value);
                //var Core = Convert.ToDouble(Item.Cells["core"].Value);
                var Ubicacion = Item.Cells["ubicacion"].Value!.ToString()!;
                //var Cantidad_Pedido = Convert.ToInt32(Item.Cells["cant_pedido"].Value);
                //var Cantidad_Real = Convert.ToInt32(Item.Cells["cant_real"].Value);

                var num_empalme = Convert.ToInt32(Item.Cells["num_empalme"].Value);
                var num_paleta = Convert.ToString(Item.Cells["num_paleta"].Value);

                var factura = Convert.ToString(Item.Cells["factura"].Value);

                var fecha_produccion = Convert.ToDateTime(Item.Cells["fecha_produccion"].Value);
                var fecha_llegada = Convert.ToDateTime(Item.Cells["fecha_llegada"].Value);



                Orden.Items.Add(new OrdenDetailsMP
                {
                    Numero = txt_numeroOrden.Text,
                    Product_Id = ProductId!.ToString()!,
                    Product_Name = ProductName!.ToString()!,
                    Width = WidthMaster,
                    Length = LengthMaster,
                    RollId = RollId,
                    //Splice = Splice,
                    //Core = Core,
                    Ubicacion = Ubicacion,
                    //Cantidad_Pedido = Cantidad_Pedido,
                    //Cantidad_Real = Cantidad_Real,

                    Num_empalme = num_empalme!,
                    Num_Paleta = num_paleta!,
                    Factura = factura!,
                    Fecha_produccion = fecha_produccion!,
                    Fecha_Ingreso = fecha_llegada!,



                    Estado = "Completo"
                });
            }

            return Orden;
        }

        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            if (Bs.Current is DataRowView drvMaster)
            {
                DataRow rowMaster = drvMaster.Row;
                DataRow[] items = rowMaster.GetChildRows("FK_MASTER_DETAILS");

                //borrar el detalle del documento.
                foreach (var item in items)
                {
                    item.Delete();
                }

                rowMaster.Delete();
                Bs.EndEdit();
                Bs.ResetBindings(false);
                Bs.Position = Bs.Count;

            }









            Bs.Position = Bs.Count;
            // Cerrar el formulario.
            btn_primero.Enabled = true;
            btn_siguiente.Enabled = true;
            btn_anterior.Enabled = true;
            btn_ultimo.Enabled = true;
            btn_create.Enabled = true;
            btn_save.Enabled = false;
            btn_cancel.Enabled = false;
            txt_fecha_produccion.Enabled = false;
            txt_fecha_recepcion.Enabled = false;
            txt_OrdenCompra.ReadOnly = true;
            txt_guia.ReadOnly = true;
            txt_lote.ReadOnly = true;
            txt_embarque.ReadOnly = true;
            btn_ProvBuscar.Enabled = false;
            btn_TransportBuscar.Enabled = false;
            btn_RecepBuscar.Enabled = false;
            GridItems.ReadOnly = true;
            RefreshDocument();
        }

        private void Btn_template_Click(object sender, EventArgs e)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("TemplateMateriaPrima");
            string filePath = Path.Combine(Environment.CurrentDirectory, "Template");


            worksheet.Cell(1, 1).Value = "Product. Id.";
            worksheet.Cell(1, 2).Value = "Nombre del Producto";
            worksheet.Cell(1, 3).Value = "Roll-Id";
            worksheet.Column(3).Width = 20;
            worksheet.Cell(1, 4).Value = "Width";
            worksheet.Cell(1, 5).Value = "Length";
            worksheet.Cell(1, 6).Value = "# Empalme";
            worksheet.Cell(1, 7).Value = "Fecha Produccion";
            worksheet.Cell(1, 8).Value = "Factura";
            worksheet.Cell(1, 9).Value = "Ubicacion";
            worksheet.Cell(1, 10).Value = "Palet #";
            worksheet.Cell(1, 11).Value = "Fecha de Llegada";
            var col1 = worksheet.Column(1);
            col1.Style.NumberFormat.Format = "@";
            col1.Width = 15; // Ajustar el ancho de la columna

            var col2 = worksheet.Column(2);
            col2.Width = 50; // Ajustar el ancho de la columna


            var col3 = worksheet.Column(3);
            col3.Width = 15; // Ajustar el ancho de la columna
            worksheet.Column("C").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("D").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("E").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("F").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("G").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("H").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("I").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("J").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            worksheet.Column("K").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            var col4 = worksheet.Column(4);
            col4.Width = 15; // Ajustar el ancho de la columna

            var col5 = worksheet.Column(5);
            col5.Width = 15; // Ajustar el ancho de la columna

            var col6 = worksheet.Column(6);
            col6.Width = 15; // Ajustar el ancho de la columna

            var col7 = worksheet.Column(7);
            col7.Width = 25; // Ajustar el ancho de la columna

            var col8 = worksheet.Column(8);
            col8.Width = 12; // Ajustar el ancho de la columna

            var col9 = worksheet.Column(9);
            col9.Width = 12; // Ajustar el ancho de la columna

            var col10 = worksheet.Column(10);
            col10.Width = 12; // Ajustar el ancho de la columna

            var col11 = worksheet.Column(11);
            col11.Width = 25; // Ajustar el ancho de la columna

            var headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            try
            {
                workbook.SaveAs(filePath + ".xlsx");
                var psi = new ProcessStartInfo
                {
                    FileName = filePath + ".xlsx",      // Abre con la app por defecto (.xlsx ? Excel)
                    UseShellExecute = true     // Necesario en .NET Core/5+ para usar la asociaci�n de ficheros
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el archivo autom�ticamente: {ex.Message}");
            }
        }

        private void Btn_LoadRows_Click(object sender, EventArgs e)
        {

            Frm_ImportacionExcel frmImport = new();
            frmImport.ShowDialog();

            ListExcel = frmImport.lista;

            foreach (var item in ListExcel)
            {
                ChildsRows = (DataRowView)BsDetalle.AddNew()!;
                ChildsRows.BeginEdit();
                ChildsRows["numero"] = txt_numeroOrden.Text;
                ChildsRows["product_id"] = item.product_id;
                ChildsRows["product_name"] = item.product_name;
                ChildsRows["rollid"] = item.rollid;
                ChildsRows["width"] = item.width;
                ChildsRows["length"] = item.length;
                ChildsRows["empalme"] = item.num_empalme;
                ChildsRows["fecha_produccion"] = item.fecha_produccion;
                ChildsRows["factura"] = item.factura;
                ChildsRows["ubicacion"] = item.ubicacion;
                ChildsRows["num_paleta"] = item.palet_num;
                ChildsRows["fecha_llegada"] = item.fecha_llegada;
                ChildsRows.Row.SetParentRow(((DataRowView)Bs.Current!).Row, Ds.Relations["FK_MASTER_DETAILS"]);
                ChildsRows.EndEdit();
            }

            if (GridItems.Rows.Count > 0)
            {
                GridItems.ClearSelection(); // Limpia selecci�n previa
                GridItems.Rows[0].Selected = true; // Selecciona la primera fila
                GridItems.CurrentCell = GridItems.Rows[0].Cells[0]; // Mueve el foco
                ContarFilas();
            }


        }

        private void Btn_OrdenBuscar_Click(object sender, EventArgs e)
        {
            Frm_oneparameter frmbuscar = new()
            {
                StartPosition = StartPosition = FormStartPosition.Manual,
                Location = new Point(this.Location.X + 300, this.Location.Y + 150)
            };
            frmbuscar.ShowDialog();
            if (frmbuscar.Parameter != null)
            {
                int busqueda = Bs.Find("numero", frmbuscar.Parameter);
                if (busqueda > 0)
                {
                    Bs.Position = busqueda;
                    RefreshDocument();
                }
                else
                {
                    MessageBox.Show("No se encontro el numero del documento...", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }

        }

        private void Btn_CloseDoc_Click(object sender, EventArgs e)
        {
            CerrarDocumentoOrder();
        }

        private void CerrarDocumentoOrder()
        {

            if (chk_DocumentClose.Checked)
            {
                MessageBox.Show("El Documento ya se encuentra cerrado.", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Deseas Cerrar este Documento (S/N)???", "Confrmar Cierre",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bool ok = Services.CloseOrder(txt_numeroOrden.Text);
                if (ok) MessageBox.Show("Orden cerrada correctamente.");
                var doc = (DataRowView)Bs.Current!;
                doc.BeginEdit();
                doc["CloseDocument"] = true;
                doc.EndEdit();
                doc.Row.AcceptChanges();
                RefreshDocument();
                //Actualizo los logs del documento en el campo notas
                string user = "-> Npino - Departamento de Sistema";
                string dataClose = $"Documento cerrado por: " + user + Environment.NewLine +
                                  "Fecha de Cierre: " + DateTime.Now + Environment.NewLine;

                Services.UpDateLogsNotes(txt_numeroOrden.Text, dataClose);

            }
            else if (result == DialogResult.No)
            {
                //enviar mensaje al log de windows
            }
        }

        private void ToolStripButton3_Click(object sender, EventArgs e)
        {

        }

        private void Btn_AnularDoc_Click(object sender, EventArgs e)
        {
            if (chk_anulado.Checked)
            {
                MessageBox.Show("El Documento ya esta anulado...", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show("Deseas Anular este Documento (S/N)???", "Confrmar Cierre",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (result == DialogResult.Yes)
            {
                bool ok = Services.AnularOrden(txt_numeroOrden.Text);
                if (ok) MessageBox.Show("Orden Anulada correctamente.");
                var doc = (DataRowView)Bs.Current!;
                doc.BeginEdit();
                doc["Anulado"] = true;
                doc.EndEdit();
                doc.Row.AcceptChanges();
                RefreshDocument();
                //Actualizo los logs del documento en el campo notas
                string user = "-> Npino - Departamento de Sistema";
                string dataClose = $"Documento Anulado por: " + user + Environment.NewLine +
                                  "Fecha de en que el documento se anulo: " + DateTime.Now + Environment.NewLine;

                Services.UpDateLogsNotes(txt_numeroOrden.Text, dataClose);

            }

        }
        private void Chk_anulado_Click(object sender, EventArgs e)
        {
            // Cancelar cambio manualmente
            chk_anulado.Checked = !chk_anulado.Checked;
        }

        private void Chk_anulado_KeyDown(object sender, KeyEventArgs e)
        {
            // Prevenir cambio con teclado
            e.Handled = true;
        }

        private void Chk_DocumentClose_Click(object sender, EventArgs e)
        {
            // Cancelar cambio manualmente
            chk_DocumentClose.Checked = !chk_DocumentClose.Checked;
        }

        private void Chk_DocumentClose_KeyDown(object sender, KeyEventArgs e)
        {
            // Prevenir cambio con teclado
            e.Handled = true;
        }

        private void Btn_SearchDoc_Click(object sender, EventArgs e)
        {
            FrmBuscador_OrdenesMP frm_busqueda = new()
            {
                DtItems = Ds.Tables["Dtmateria"]!
            };
            frm_busqueda.ShowDialog();
            if (frm_busqueda.Orden != null)
            {
                int busqueda = Bs.Find("numero", frm_busqueda.Orden);
                if (busqueda > 0)
                {
                    Bs.Position = busqueda;
                    RefreshDocument();
                }
                else
                {
                    MessageBox.Show("No se encontro el numero del documento...", "Advertencia",
                        MessageBoxButtons.OK, MessageBoxIcon.Warning);
                }
            }
        }

        private void Btn_deleteRows_Click(object sender, EventArgs e)
        {
            if (GridItems.CurrentRow == null)
            {
                MessageBox.Show("No hay ninguna fila seleccionada.", "aviso", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }
            var row = (DataRowView)GridItems.CurrentRow.DataBoundItem!;

            if (MessageBox.Show($"Eliminar el producto con Id = {row["product_id"]} - Y roll-id ={row["rollid"]}", "Confirmar Borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes) return;

            row.Delete();
            Bs.EndEdit();


        }

        private void Btn_ExportDoc_Click(object sender, EventArgs e)
        {
            List<OrdenDetailsMP> Ordenes = CREATE_LIST_PRODUCTS();
            ExportDataService.ExportToExcel<OrdenDetailsMP>(Ordenes, "ordenes_mp.xlsx");
        }
        private List<OrdenDetailsMP> CREATE_LIST_PRODUCTS()
        {
            List<OrdenDetailsMP> Ordenes = [];
            for (int i = 0; i <= GridItems.Rows.Count - 1; i++)
            {
                OrdenDetailsMP orden = new()
                {
                    Numero = txt_numeroOrden.Text.ToString(),
                    Product_Id = Convert.ToString(GridItems.Rows[i].Cells["product_id"].Value)!,
                    Product_Name = Convert.ToString(GridItems.Rows[i].Cells["product_name"].Value)!,
                    Product_Type = Convert.ToString(GridItems.Rows[i].Cells["product_type"].Value)!,
                    Width = Convert.ToDouble(GridItems.Rows[i].Cells["width"].Value)!,
                    Length = Convert.ToDouble(GridItems.Rows[i].Cells["length"].Value)!,
                    Msi = Convert.ToDouble(GridItems.Rows[i].Cells["msi"].Value)!,
                    RollId = Convert.ToString(GridItems.Rows[i].Cells["rollid"].Value)!,
                    Splice = Convert.ToInt16(GridItems.Rows[i].Cells["splice"].Value)!,
                    Core = Convert.ToInt16(GridItems.Rows[i].Cells["core"].Value)!,
                    Ubicacion = Convert.ToString(GridItems.Rows[i].Cells["ubicacion"].Value)!,
                    Cantidad_Pedido = Convert.ToInt16(GridItems.Rows[i].Cells["cant_pedido"].Value)!,
                    Cantidad_Real = Convert.ToInt16(GridItems.Rows[i].Cells["cant_real"].Value)!
                };
                Ordenes.Add(orden);
            }
            return Ordenes;
        }

        private void Btn_printDoc_Click(object sender, EventArgs e)
        {
            ReportService.Reporte_Orden_MatPrima(txt_numeroOrden.Text, this, "RptImportFormat.rdlc", "ORDEN DE RECEPCION DE MATERIA PRIMA");
        }
    }
}


