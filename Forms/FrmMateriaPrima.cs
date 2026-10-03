using System.Data;
using System.Diagnostics;
using ClosedXML.Excel;
using Ritrama2025.Forms.Buscadores;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Forms.Seleccion;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.MateriaPrima;
using Ritrama2025.Services.ReportsService.ReportsService;

using Sunny.UI;
namespace Ritrama2025.Forms
{
    /// <summary>
    /// Orden de compra: cabecera + renglones de materia prima recibida. El formulario es
    /// IAsyncFormLoad para que FormManager lo cargue y lo pinte antes de meterlo en la
    /// pestana (evita el frame en blanco) e IFormTemaClaro para que Main no le imponga
    /// el tema oscuro sobre el estilo verde que usa el resto de los modulos.
    /// </summary>
    public partial class FrmMateriaPrima : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        public readonly IServiceMateriaPrima Services;
        public readonly IExportDataService ExportDataService;
        public readonly IReportsService ReportService;
        public readonly IServiceCommonData ServiceCommonData;
        public readonly ICommonService CommonService;

        public DataSet Ds = new();
        readonly BindingSource Bs = [];
        readonly BindingSource BsDetalle = [];
        private DataRowView ParentRow = null!;
        private DataRowView ChildsRows = null!;
        string EditMode = "READ";

        // Columna calculada en memoria para ordenar las ordenes numericamente.
        private const string ColumnaOrdenNumerico = "numero_orden";

        public List<TemplateMasterExcel> ListExcel = [];

        public FrmMateriaPrima(IServiceMateriaPrima Services, IExportDataService exportDataService, IReportsService reportService, IServiceCommonData serviceCommonData, ICommonService commonService)
        {
            InitializeComponent();
            this.Services = Services;
            ExportDataService = exportDataService;
            ReportService = reportService;
            ServiceCommonData = serviceCommonData;
            CommonService = commonService;
            Text = "RECEPCION MATERIA PRIMA";

            // Estilo verde de SunnyUI, el mismo que usan Clientes/Pedidos/Orden de Corte.
            // Sin este UIStyleManager el form queda con la paleta naranja de SunnyUI y el
            // tema oscuro de Main, que es justo lo que rompe la estetica del modulo.
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            ReaplicarTema();
            ToolStripTheme.Ajustar(toolStrip1);
            ToolStripTheme.AjustarAnchoMinimo(this, toolStrip1);
        }

        /// <summary>
        /// Reaplica el tema verde para pisar el UIStyleManager global del Main.
        /// </summary>
        public void ReaplicarTema()
        {
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = LightGreenTheme.PrimaryDark;
            TitleForeColor = Color.White;
            EstilizarGrid();
        }

        /// <summary>
        /// Carga datos y deja el formulario pintado. Lo invoca FormManager antes de
        /// mostrarlo en la pestana; por eso aqui ya se pueden crear bindings y columnas.
        /// </summary>
        public async Task InitializeAsync()
        {
            await LoadDataAsync();

            // Si la carga fallo (sin conexion, tabla inexistente) no hay cabecera que
            // enlazar: seguir adelante dejaba el formulario en blanco y el error se
            // repetia en cada binding.
            if (Ds.Tables["DtMateria"] == null)
            {
                label_counter_rows.Text = "Registros: sin datos";
                return;
            }

            AsegurarColumnaOrdenNumerico();
            BindDataSource();
            BindingControls();
            Bs.Sort = $"{ColumnaOrdenNumerico} DESC";
            RefreshDocument();
        }

        private void FrmMateriaPrima_Load(object sender, EventArgs e)
        {
            // Solo cuando el form se usa como ventana independiente (fallback sin pestanas).
            // Embebido en la pestana central ya lo cargo FormManager con InitializeAsync y
            // este no vuelve a cargar nada.
            if (TopLevel)
            {
                StartPosition = FormStartPosition.Manual;
                Location = new Point(155, 45);
                InitializeAsyncSafe();
            }
        }

        /// <summary>
        /// Carga inicial cuando el form corre como ventana independiente, donde FormManager
        /// no llama a InitializeAsync. Es async void porque el evento Load no admite await;
        /// las excepciones se atrapan dentro de LoadDataAsync.
        /// </summary>
        private async void InitializeAsyncSafe()
        {
            if (Bs.DataSource != null)
            {
                return;
            }

            await InitializeAsync();
        }

        /// <summary>
        /// Pinta el grid del detalle con la paleta verde del modulo y deja las barras de
        /// desplazamiento y la altura de fila uniformes.
        /// </summary>
        private void EstilizarGrid()
        {
            GridItems.BackgroundColor = LightGreenTheme.AlternateRow;
            GridItems.EnableHeadersVisualStyles = false;
            GridItems.BorderStyle = BorderStyle.None;
            GridItems.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            GridItems.ColumnHeadersDefaultCellStyle.BackColor = LightGreenTheme.PrimaryDark;
            GridItems.ColumnHeadersDefaultCellStyle.ForeColor = Color.White;
            GridItems.ColumnHeadersDefaultCellStyle.SelectionBackColor = LightGreenTheme.PrimaryDark;
            GridItems.ColumnHeadersDefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            GridItems.RowsDefaultCellStyle.BackColor = LightGreenTheme.Background;
            GridItems.RowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
            GridItems.RowsDefaultCellStyle.SelectionBackColor = LightGreenTheme.Primary;
            GridItems.RowsDefaultCellStyle.SelectionForeColor = Color.White;
            GridItems.AlternatingRowsDefaultCellStyle.BackColor = LightGreenTheme.AlternateRow;
            GridItems.AlternatingRowsDefaultCellStyle.ForeColor = Color.FromArgb(48, 48, 48);
            GridItems.RowHeadersDefaultCellStyle.BackColor = LightGreenTheme.AlternateRow;
            GridItems.GridColor = Color.FromArgb(200, 220, 180);

            // Fill reparte el ancho disponible entre las columnas: el grid es lo unico que
            // ocupa el panel central y de lo contrario quedaban huecos al ensanchar la
            // ventana (o columnas cortadas al angostarla).
            GridItems.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
        }

        /// <summary>
        /// Refresca el encabezado del documento actual: contador, icono de estado y total.
        /// </summary>
        private void RefreshDocument()
        {
            label_counter_rows.Text = $"Registros: {Bs.Count}";

            // numero es NVarChar, asi que ordenar por el como texto daria "9" > "10". Se
            // ordena una sola vez y numericamente al cargar; en cada refresco solo se
            // recalcula lo que depende del registro actual, para no cambiar el registro
            // que el usuario esta viendo.
            ActualizarEstadoDocumento();
            ContarFilas();
        }

        /// <summary>
        /// Calcula el estado (abierto/cerrado/anulado) del documento visible y refleja el
        /// icono y el texto del estado.
        /// </summary>
        private void ActualizarEstadoDocumento()
        {
            bool anulado = chk_anulado.Checked;
            bool cerrado = chk_DocumentClose.Checked;

            string estado = anulado ? "anulado" : cerrado ? "cerrado" : "abierto";
            string recurso = anulado
                ? "Ritrama2025.Images.ANULADO_DOCUMENTO.png"
                : cerrado
                    ? "Ritrama2025.Images.CLOSE_DOCUMENT.png"
                    : "Ritrama2025.Images.OPEN_DOCUMENT.png";

            label16.Text = $"Status Orden : {estado}";
            label16.ForeColor = anulado
                ? Color.Firebrick
                : cerrado
                    ? LightGreenTheme.PrimaryDark
                    : Color.FromArgb(48, 48, 48);

            // Los iconos van como EmbeddedResource, no como archivo en disco: leerlos con
            // Image.FromFile lanzaba FileNotFoundException y rompia el formulario al abrir.
            Image? icono = CargarIcono(recurso);
            if (icono != null)
            {
                Image? anterior = Pic_Document.Image;
                Pic_Document.Image = icono;
                anterior?.Dispose();
            }
        }

        /// <summary>
        /// Carga un icono desde los recursos incrustados del ensamblado. Devuelve null si el
        /// recurso no existe, para no dejar el formulario a medias por una imagen.
        /// </summary>
        private static Image? CargarIcono(string nombreRecurso)
        {
            try
            {
                using Stream? stream = typeof(FrmMateriaPrima).Assembly.GetManifestResourceStream(nombreRecurso);
                if (stream == null)
                {
                    return null;
                }
                using MemoryStream copia = new();
                stream.CopyTo(copia);
                copia.Position = 0;
                return Image.FromStream(copia);
            }
            catch
            {
                return null;
            }
        }
        private void BindDataSource()
        {
            //Configuracion del BindingSource.
            Bs.DataSource = Ds;
            Bs.DataMember = "DtMateria";
            //Bindingsource para el detalle de los productos.
            BsDetalle.DataSource = Bs;
            BsDetalle.DataMember = "FK_MASTER_DETAILS";
            ConfigurarColumnasDetalle();
        }

        /// <summary>
        /// Crea las columnas del grid del detalle. El Name coincide con el DataPropertyName:
        /// el codigo despues lee las celdas por nombre (Cells["empalme"], Cells["num_paleta"],
        /// etc.) y con nombres distintos esas lecturas devolvian null.
        /// </summary>
        private void ConfigurarColumnasDetalle()
        {
            GridItems.AutoGenerateColumns = false;
            GridItems.Columns.Clear();
            ADD_COLUMN_GRID("product_id", 70, "Product Id.", "product_id", GridItems);
            ADD_COLUMN_GRID("product_name", 200, "Product Name.", "product_name", GridItems);
            ADD_COLUMN_GRID("rollid", 70, "Roll-Id.", "rollid", GridItems);
            ADD_COLUMN_GRID("width", 75, "Width [Inch.]", "width", GridItems);
            ADD_COLUMN_GRID("length", 75, "Length [Pies]", "length", GridItems);
            ADD_COLUMN_GRID("empalme", 75, "# Empalme", "empalme", GridItems);
            ADD_COLUMN_GRID("fecha_produccion", 85, "Fecha Produccion", "fecha_produccion", GridItems);
            ADD_COLUMN_GRID("factura", 85, "Factura", "factura", GridItems);
            ADD_COLUMN_GRID("ubicacion", 70, "Ubica.", "ubicacion", GridItems);
            ADD_COLUMN_GRID("num_paleta", 70, "Palet #", "num_paleta", GridItems);
            ADD_COLUMN_GRID("fecha_llegada", 85, "Fecha Llegada", "fecha_llegada", GridItems);

            GridItems.DataSource = BsDetalle;
        }

        /// <summary>
        /// Agrega a la cabecera una columna entera con el numero de orden. "numero" es
        /// NVarChar, asi que ordenar por el como texto daria "9" mayor que "10"; la columna
        /// numerica permite ordenar de verdad sin tocar el SELECT ni la tabla de la BD.
        /// </summary>
        private void AsegurarColumnaOrdenNumerico()
        {
            DataTable? tabla = Ds.Tables["DtMateria"];
            if (tabla == null || tabla.Columns.Contains(ColumnaOrdenNumerico))
            {
                return;
            }

            tabla.Columns.Add(ColumnaOrdenNumerico, typeof(int));
            foreach (DataRow row in tabla.Rows)
            {
                row[ColumnaOrdenNumerico] = int.TryParse(Convert.ToString(row["numero"]), out int numero) ? numero : 0;
            }
        }

        private async Task LoadDataAsync()
        {
            try
            {
                UseWaitCursor = true;
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
            }
        }

        private void BindingControls()
        {
            //trabajar con los enlaces a datos.
            txt_numeroOrden.DataBindings.Add("Text", Bs, "numero");
            txt_OrdenCompra.DataBindings.Add("Text", Bs, "Orden_Compra");
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
            chk_anulado.DataBindings.Add("Checked", Bs, "Anulado");

        }

        /// <summary>
        /// Reengancha los checks de estado con la cabecera. Al entrar en Nuevo se desconectan
        /// para poder inicializarlos sin disparar un UPDATE; al cancelar quedaban
        /// desconectados para el resto de la sesion y el estado ya no se refrescaba.
        /// </summary>
        private void RestaurarBindingsEstado()
        {
            if (chk_anulado.DataBindings.Count == 0)
            {
                chk_anulado.DataBindings.Add("Checked", Bs, "Anulado");
            }

            if (chk_DocumentClose.DataBindings.Count == 0)
            {
                chk_DocumentClose.DataBindings.Add("Checked", Bs, "CloseDocument");
            }
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

        // La navegacion sigue el orden de la lista: "Siguiente" avanza hacia el final de la
        // lista y "Ultimo" se para en el ultimo registro. Antes los botones estaban
        // invertidos (siguiente retrocedia, primero iba al final).

        private void Btn_siguiente_Click(object sender, EventArgs e)
        {
            IrAPosicion(Bs.Position + 1);
        }

        private void Btn_anterior_Click(object sender, EventArgs e)
        {
            IrAPosicion(Bs.Position - 1);
        }

        private void Btn_ultimo_Click(object sender, EventArgs e)
        {
            IrAPosicion(Bs.Count - 1);
        }

        private void Btn_primero_Click(object sender, EventArgs e)
        {
            IrAPosicion(0);
        }

        /// <summary>
        /// Mueve el BindingSource a una posicion valida. Position fuera del rango deja el
        /// formulario sin documento, por eso se recorta al primer o al ultimo registro.
        /// </summary>
        private void IrAPosicion(int posicion)
        {
            if (Bs.Count == 0)
            {
                return;
            }

            Bs.Position = Math.Clamp(posicion, 0, Bs.Count - 1);
            RefreshDocument();
        }

        private void Btn_create_Click(object sender, EventArgs e)
        {
            if (Bs.Count > 0)
            {
                MessageBox.Show("Hay una orden en edicion. Use Cancelar antes de crear otra.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            EditMode = "ADDNEW";
            chk_anulado.DataBindings.Clear();
            chk_DocumentClose.DataBindings.Clear();
            chk_DocumentClose.Checked = false;
            chk_anulado.Checked = false;
            ParentRow = (DataRowView)Bs.AddNew()!;
            ParentRow.BeginEdit();
            int consecutivo = Services.LoadConsecOrden("CMP");
            ParentRow["numero"] = consecutivo;
            ParentRow[ColumnaOrdenNumerico] = consecutivo;
            ParentRow["total_cantidad"] = 0;
            ParentRow["CloseDocument"] = false;
            ParentRow["Anulado"] = false;
            ParentRow.EndEdit();
            Bs.EndEdit();
            AbrirFormulario();
            ActualizarEstadoDocumento();
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
            if (Bs.Current is not DataRowView cabecera)
            {
                MessageBox.Show("Primero debe crear la orden (Nuevo) antes de agregar productos.",
                    "Aviso", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            FrmProductsInsert frmInsertRows = new(ServiceCommonData, CommonService)
            {
                DtItems = Ds.Tables["DtProducts"]!,
                Titulo = "Producto"
            };
            frmInsertRows.ShowDialog();

            // El dialogo se puede cerrar sin elegir producto: antes se leia Producto.Rollid
            // aqui y eso reventaba con NullReferenceException.
            if (frmInsertRows.Producto == null)
            {
                return;
            }

            string rollid_form = frmInsertRows.Producto.Rollid;
            bool IsNotcreate = false;

            foreach (DataGridViewRow row in GridItems.Rows)
            {
                string? rollid_grid = row.Cells["rollid"].Value?.ToString();

                if (rollid_grid == rollid_form)
                {
                    IsNotcreate = true;
                    break;
                }
            }

            if (IsNotcreate)
            {
                MessageBox.Show("El roll-id ya esta en la lista, no se va ha crear...");
                return;
            }

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
            ChildsRows.Row.SetParentRow(cabecera.Row, Ds.Relations["FK_MASTER_DETAILS"]);
            ChildsRows.EndEdit();
            BsDetalle.EndEdit();
            ContarFilas();
        }

        /// <summary>
        /// Escribe el numero de renglones del documento en la cabecera. Se escribe sobre la
        /// fila y no sobre el textbox porque txt_total_cantidad esta enlazado a
        /// total_cantidad: escribir en el textbox lo perdia al siguiente refresco.
        /// </summary>
        private void ContarFilas()
        {
            if (Bs.Current is not DataRowView cabecera)
            {
                return;
            }

            int filas = GridItems.Rows.Count;
            cabecera.BeginEdit();
            cabecera["total_cantidad"] = filas;
            cabecera.EndEdit();
        }

        private void Btn_save_Click(object sender, EventArgs e)
        {
            //validar los roll-id
            foreach (DataGridViewRow row in GridItems.Rows)
            {
                string? rollid_grid = row.Cells["rollid"].Value?.ToString();
                if (!ServiceCommonData.VerificarRollIdNoRepeat(rollid_grid!))
                {
                    return;
                }
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

            if (EditMode != "ADDNEW")
            {
                MessageBox.Show("Solo se puede guardar una orden en modo Nuevo.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            SAVE_NEW();
        }

        private void SAVE_NEW()
        {
            bool ok = Services.GuardarOrden(CREATE_ORDEN_OBJECT());
            if (!ok)
            {
                // El servicio ya reporto el error. No se avanza el consecutivo ni se cambia
                // el estado de los botones: la orden sigue en edicion y se puede reintentar.
                return;
            }

            MessageBox.Show("se guardo correctamente...");
            EditMode = "EDIT";

            // Int32 y no Int16: el consecutivo superaba 32767 y Convert.ToInt16 lanzaba
            // OverflowException al pedir una orden nueva pasado ese numero.
            int ProxConsec = int.TryParse(txt_numeroOrden.Text, out int numeroActual) ? numeroActual + 1 : 1;
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
                // Value del DateTimePicker en vez del Text: el Text depende de la cultura
                // y Convert.ToDateTime fallaba con formatos dd/MM/yyyy en otras regiones.
                Fecha_Recepcion = txt_fecha_recepcion.Value,
                Fecha_Produccion = txt_fecha_produccion.Value,
                Orden_Compra = txt_OrdenCompra.Text,
                Proveedor_id = LeerGuid(txt_prov_Id.Text),
                Proveedor_name = txt_nombre_prov.Text,
                Transport_id = LeerGuid(txt_transport_id.Text),
                Transport_name = txt_transport_name.Text,
                Guia = txt_guia.Text,
                Lote = txt_lote.Text,
                Numero_Embarque = txt_embarque.Text,
                Person_Id = LeerGuid(txt_person_id.Text),
                Person_Name = txt_person_name.Text,
                CloseDocument = false,
                Notas = txt_notas.Text + Environment.NewLine + "Documento de Materia Prima Creado: " + Environment.NewLine + DateTime.Now,
                Renglones = GridItems.Rows.Count,
            };

            //Items: se leen de las filas del detalle, que traen todas las columnas de
            // ItemsMateria. Antes se leian del grid y se guardaban en cero / vacio lo que no
            // estaba como columna visible (producto, msi, splice, core, cantidades).
            foreach (DataGridViewRow Item in GridItems.Rows)
            {
                if (Item.DataBoundItem is not DataRowView detalle)
                {
                    continue;
                }

                Orden.Items.Add(ConstruirRenglon(detalle, txt_numeroOrden.Text));
            }

            return Orden;
        }

        /// <summary>
        /// Construye un renglon de la orden a partir de la fila del detalle. Concentra la
        /// conversion de tipos, que antes estaba duplicada y con Convert.To* sin TryParse
        /// (un NULL en la base lanzaba FormatException al guardar).
        /// </summary>
        private static OrdenDetailsMP ConstruirRenglon(DataRowView detalle, string numero)
        {
            return new OrdenDetailsMP
            {
                Numero = numero,
                Product_Id = LeerTexto(detalle, "product_id"),
                Product_Name = LeerTexto(detalle, "product_name"),
                Product_Type = LeerTexto(detalle, "type"),
                Width = LeerDouble(detalle, "width"),
                Length = LeerDouble(detalle, "length"),
                Msi = LeerDouble(detalle, "msi"),
                RollId = LeerTexto(detalle, "rollid"),
                Splice = (int)LeerDouble(detalle, "splice"),
                Core = LeerDouble(detalle, "core"),
                Ubicacion = LeerTexto(detalle, "ubicacion"),
                Cantidad_Pedido = LeerDouble(detalle, "cant_pedido"),
                Cantidad_Real = LeerDouble(detalle, "cant_real"),
                Num_empalme = (int)LeerDouble(detalle, "empalme"),
                Num_Paleta = LeerTexto(detalle, "num_paleta"),
                Factura = LeerTexto(detalle, "factura"),
                Fecha_produccion = LeerFecha(detalle, "fecha_produccion"),
                Fecha_Ingreso = LeerFecha(detalle, "fecha_llegada"),
                Estado = LeerTexto(detalle, "estado") is { Length: > 0 } estado ? estado : "Completo",
            };
        }

        private static string LeerTexto(DataRowView fila, string columna)
        {
            return fila.Row.Table.Columns.Contains(columna)
                ? Convert.ToString(fila[columna]) ?? string.Empty
                : string.Empty;
        }

        private static double LeerDouble(DataRowView fila, string columna)
        {
            if (!fila.Row.Table.Columns.Contains(columna) || fila[columna] == DBNull.Value)
            {
                return 0;
            }

            return double.TryParse(Convert.ToString(fila[columna]), System.Globalization.CultureInfo.CurrentCulture,
                out double valor)
                ? valor
                : 0;
        }

        private static DateTime LeerFecha(DataRowView fila, string columna)
        {
            if (!fila.Row.Table.Columns.Contains(columna) || fila[columna] == DBNull.Value)
            {
                return DateTime.Now;
            }

            return fila[columna] is DateTime fecha ? fecha : DateTime.Now;
        }

        private static Guid LeerGuid(string texto)
        {
            return Guid.TryParse(texto, out Guid id) ? id : Guid.Empty;
        }


        private void Btn_cancel_Click(object sender, EventArgs e)
        {
            if (Bs.Current is DataRowView drvMaster)
            {
                DataRow rowMaster = drvMaster.Row;
                DataRow[] items = rowMaster.GetChildRows("FK_MASTER_DETAILS");

                //borrar el detalle del documento.
                foreach (DataRow item in items)
                {
                    item.Delete();
                }

                rowMaster.Delete();
                BsDetalle.EndEdit();
                Bs.EndEdit();
                Bs.ResetBindings(false);

                // Position fuera del rango (Bs.Count) dejaba el formulario sin documento y
                // los textboxes en blanco; se vuelve al primer registro valido.
                if (Bs.Count > 0)
                {
                    Bs.Position = 0;
                }
            }

            EditMode = "READ";
            RestaurarBindingsEstado();
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
            using XLWorkbook workbook = new XLWorkbook();
            IXLWorksheet worksheet = workbook.Worksheets.Add("TemplateMateriaPrima");
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
            IXLColumn col1 = worksheet.Column(1);
            col1.Style.NumberFormat.Format = "@";
            col1.Width = 15; // Ajustar el ancho de la columna

            IXLColumn col2 = worksheet.Column(2);
            col2.Width = 50; // Ajustar el ancho de la columna


            IXLColumn col3 = worksheet.Column(3);
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

            IXLColumn col4 = worksheet.Column(4);
            col4.Width = 15; // Ajustar el ancho de la columna

            IXLColumn col5 = worksheet.Column(5);
            col5.Width = 15; // Ajustar el ancho de la columna

            IXLColumn col6 = worksheet.Column(6);
            col6.Width = 15; // Ajustar el ancho de la columna

            IXLColumn col7 = worksheet.Column(7);
            col7.Width = 25; // Ajustar el ancho de la columna

            IXLColumn col8 = worksheet.Column(8);
            col8.Width = 12; // Ajustar el ancho de la columna

            IXLColumn col9 = worksheet.Column(9);
            col9.Width = 12; // Ajustar el ancho de la columna

            IXLColumn col10 = worksheet.Column(10);
            col10.Width = 12; // Ajustar el ancho de la columna

            IXLColumn col11 = worksheet.Column(11);
            col11.Width = 25; // Ajustar el ancho de la columna

            IXLRow headerRow = worksheet.Row(1);
            headerRow.Style.Font.Bold = true;
            headerRow.Style.Fill.BackgroundColor = XLColor.LightGray;

            try
            {
                workbook.SaveAs(filePath + ".xlsx");
                ProcessStartInfo psi = new ProcessStartInfo
                {
                    FileName = filePath + ".xlsx",      // Abre con la app por defecto (.xlsx -> Excel)
                    UseShellExecute = true     // Necesario en .NET Core/5+ para usar la asociaci\u00f3n de ficheros
                };
                Process.Start(psi);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"No se pudo abrir el archivo autom\u00e1ticamente: {ex.Message}");
            }
        }

        private void Btn_LoadRows_Click(object sender, EventArgs e)
        {

            Frm_ImportacionExcel frmImport = new();
            frmImport.ShowDialog();

            ListExcel = frmImport.lista;

            foreach (TemplateMasterExcel item in ListExcel)
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
                GridItems.ClearSelection(); // Limpia selecci\u00f3n previa
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
                Location = new Point(Location.X + 300, Location.Y + 150)
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
                if (ok)
                {
                    MessageBox.Show("Orden cerrada correctamente.");
                }

                DataRowView doc = (DataRowView)Bs.Current!;
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
                if (ok)
                {
                    MessageBox.Show("Orden Anulada correctamente.");
                }

                DataRowView doc = (DataRowView)Bs.Current!;
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
                DtItems = Ds.Tables["DtMateria"]!
            };
            frm_busqueda.ShowDialog();
            if (frm_busqueda.Orden != null)
            {
                IrAPosicion(BuscarPosicionDocumento(frm_busqueda.Orden));
            }
        }

        /// <summary>
        /// Devuelve la posicion del documento buscado, o -1 si no existe. La busqueda con
        /// Bs.Find devuelve el indice y antes se comparaba con &gt; 0, con lo cual el primer
        /// registro de la lista jamas se encontraba.
        /// </summary>
        private int BuscarPosicionDocumento(string numero)
        {
            int posicion = Bs.Find("numero", numero);
            if (posicion < 0)
            {
                MessageBox.Show("No se encontro el numero del documento...", "Advertencia",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }

            return posicion;
        }

        private void Btn_deleteRows_Click(object sender, EventArgs e)
        {
            if (GridItems.CurrentRow == null)
            {
                MessageBox.Show("No hay ninguna fila seleccionada.", "aviso", MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                return;
            }

            if (GridItems.CurrentRow.DataBoundItem is not DataRowView row)
            {
                return;
            }

            if (MessageBox.Show($"Eliminar el producto con Id = {row["product_id"]} - Y roll-id ={row["rollid"]}", "Confirmar Borrado", MessageBoxButtons.YesNo, MessageBoxIcon.Warning) != DialogResult.Yes)
            {
                return;
            }

            row.Delete();
            // El origen del detalle es BsDetalle, no Bs: con Bs.EndEdit el borrado se
            // quedaba sin confirmar y el total no se recalculaba.
            BsDetalle.EndEdit();
            Bs.EndEdit();
            ContarFilas();
        }

        private void Btn_ExportDoc_Click(object sender, EventArgs e)
        {
            List<OrdenDetailsMP> Ordenes = CREATE_LIST_PRODUCTS();
            if (Ordenes.Count == 0)
            {
                MessageBox.Show("El documento no tiene productos que exportar.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            ExportDataService.ExportToExcel<OrdenDetailsMP>(Ordenes, "ordenes_mp.xlsx");
        }

        /// <summary>
        /// Arma la lista que se exporta. Se lee desde las filas del detalle y no desde las
        /// celdas del grid: product_type, msi, splice, core y las cantidades no son columnas
        /// visibles, y Cells["..."] sobre una columna inexistente lanzaba ArgumentException
        /// al pulsar Exportar.
        /// </summary>
        private List<OrdenDetailsMP> CREATE_LIST_PRODUCTS()
        {
            List<OrdenDetailsMP> Ordenes = [];
            foreach (DataGridViewRow item in GridItems.Rows)
            {
                if (item.DataBoundItem is DataRowView detalle)
                {
                    Ordenes.Add(ConstruirRenglon(detalle, txt_numeroOrden.Text));
                }
            }

            return Ordenes;
        }

        private void Btn_printDoc_Click(object sender, EventArgs e)
        {
            ReportService.Reporte_Orden_MatPrima(txt_numeroOrden.Text, this, "RptImportFormat.rdlc", "ORDEN DE RECEPCION DE MATERIA PRIMA");
        }
    }
}


