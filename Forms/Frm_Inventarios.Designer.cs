namespace Ritrama2025.Forms
{
    partial class Frm_Inventarios
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Inventarios));
            TabPages_Inventario = new TabControl();
            tabPage1 = new TabPage();
            groupBox6 = new GroupBox();
            rad_MasterConsumido = new RadioButton();
            rad_MasterParcial = new RadioButton();
            rad_MasterCompleto = new RadioButton();
            btn_delete_master = new Button();
            bot_printLabel = new Button();
            pictureBox3 = new PictureBox();
            label11 = new Label();
            btn_limpiar_filtros = new Button();
            COUNT_ROWS = new Label();
            btn_DetailsConsumos = new Button();
            groupBox3 = new GroupBox();
            rad_rollid = new RadioButton();
            rad_ubication = new RadioButton();
            rad_productid = new RadioButton();
            rad_product_name = new RadioButton();
            GridMaster = new DataGridView();
            btn_buscar = new Button();
            label8 = new Label();
            txt_buscar = new TextBox();
            tabPage2 = new TabPage();
            tabPage3 = new TabPage();
            Page_RolloCortado = new TabPage();
            pictureBox4 = new PictureBox();
            label12 = new Label();
            COUNTER_ROLLOS = new Label();
            groupBox4 = new GroupBox();
            rad_ordencorte_cor = new RadioButton();
            rad_codeperson_cor = new RadioButton();
            rad_codeunique_cor = new RadioButton();
            rad_rollid_cor = new RadioButton();
            rad_ubic_cor = new RadioButton();
            rad_productid_cor = new RadioButton();
            rad_productname_cor = new RadioButton();
            GridRollosCortados = new DataGridView();
            bto_limpiar_cor = new Button();
            bot_buscar_cor = new Button();
            label10 = new Label();
            txt_buscar_cor = new TextBox();
            tabPage5 = new TabPage();
            groupBox1 = new GroupBox();
            btn_clearGrid = new Button();
            label15 = new Label();
            txt_log_notifications = new RichTextBox();
            groupBox9 = new GroupBox();
            btn_accion = new Button();
            chk_saveproductsnotfound = new CheckBox();
            groupBox8 = new GroupBox();
            chk_valid_products = new CheckBox();
            chk_repeat_rollid = new CheckBox();
            txt_errors = new TextBox();
            NUMBERS_NOTIFICATIONS = new Label();
            txt_filePath = new TextBox();
            label7 = new Label();
            txt_fileName = new TextBox();
            label4 = new Label();
            txt_warning = new TextBox();
            txt_number_rows = new TextBox();
            label9 = new Label();
            label14 = new Label();
            groupBox7 = new GroupBox();
            radioButton1 = new RadioButton();
            radioButton2 = new RadioButton();
            radioButton3 = new RadioButton();
            btn_saveDatabase = new Button();
            btn_load_data = new Button();
            Grid_Items = new DataGridView();
            btn_search = new Button();
            textBox1 = new TextBox();
            label6 = new Label();
            groupBox5 = new GroupBox();
            label16 = new Label();
            cbo_tabla = new ComboBox();
            label22 = new Label();
            btn_dropmaster = new Button();
            label5 = new Label();
            groupBox2 = new GroupBox();
            rad_rollos = new RadioButton();
            rad_hojas = new RadioButton();
            rad_graphics = new RadioButton();
            rad_master = new RadioButton();
            btn_load_sheet = new Button();
            label3 = new Label();
            txt_file_path = new TextBox();
            label2 = new Label();
            txt_file_name = new TextBox();
            tabPage6 = new TabPage();
            imageList1 = new ImageList(components);
            PANEL_TITULO = new Panel();
            label13 = new Label();
            ComboPrinters = new ComboBox();
            pictureBox5 = new PictureBox();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            toolStrip1 = new ToolStrip();
            Btn_reload = new ToolStripButton();
            Bot_Reports = new ToolStripButton();
            Bot_Excel = new ToolStripButton();
            Bot_Txt = new ToolStripButton();
            panel_loading = new Panel();
            text_loadingindicator = new Label();
            pictureBox2 = new PictureBox();
            TabPages_Inventario.SuspendLayout();
            tabPage1.SuspendLayout();
            groupBox6.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            groupBox3.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridMaster).BeginInit();
            Page_RolloCortado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            groupBox4.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)GridRollosCortados).BeginInit();
            tabPage5.SuspendLayout();
            groupBox1.SuspendLayout();
            groupBox9.SuspendLayout();
            groupBox8.SuspendLayout();
            groupBox7.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).BeginInit();
            groupBox5.SuspendLayout();
            groupBox2.SuspendLayout();
            PANEL_TITULO.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            toolStrip1.SuspendLayout();
            panel_loading.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            SuspendLayout();
            // 
            // TabPages_Inventario
            // 
            TabPages_Inventario.Controls.Add(tabPage1);
            TabPages_Inventario.Controls.Add(tabPage2);
            TabPages_Inventario.Controls.Add(tabPage3);
            TabPages_Inventario.Controls.Add(Page_RolloCortado);
            TabPages_Inventario.Controls.Add(tabPage5);
            TabPages_Inventario.Controls.Add(tabPage6);
            TabPages_Inventario.ImageList = imageList1;
            TabPages_Inventario.Location = new Point(8, 164);
            TabPages_Inventario.Name = "TabPages_Inventario";
            TabPages_Inventario.SelectedIndex = 0;
            TabPages_Inventario.Size = new Size(1018, 792);
            TabPages_Inventario.TabIndex = 0;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(groupBox6);
            tabPage1.Controls.Add(btn_delete_master);
            tabPage1.Controls.Add(bot_printLabel);
            tabPage1.Controls.Add(pictureBox3);
            tabPage1.Controls.Add(label11);
            tabPage1.Controls.Add(btn_limpiar_filtros);
            tabPage1.Controls.Add(COUNT_ROWS);
            tabPage1.Controls.Add(btn_DetailsConsumos);
            tabPage1.Controls.Add(groupBox3);
            tabPage1.Controls.Add(GridMaster);
            tabPage1.Controls.Add(btn_buscar);
            tabPage1.Controls.Add(label8);
            tabPage1.Controls.Add(txt_buscar);
            tabPage1.ImageIndex = 0;
            tabPage1.Location = new Point(4, 23);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(1010, 765);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Master";
            // 
            // groupBox6
            // 
            groupBox6.Controls.Add(rad_MasterConsumido);
            groupBox6.Controls.Add(rad_MasterParcial);
            groupBox6.Controls.Add(rad_MasterCompleto);
            groupBox6.Location = new Point(183, 589);
            groupBox6.Name = "groupBox6";
            groupBox6.Size = new Size(167, 115);
            groupBox6.TabIndex = 8;
            groupBox6.TabStop = false;
            groupBox6.Text = "Filtro según su Uso: ";
            // 
            // rad_MasterConsumido
            // 
            rad_MasterConsumido.AutoSize = true;
            rad_MasterConsumido.Location = new Point(6, 60);
            rad_MasterConsumido.Name = "rad_MasterConsumido";
            rad_MasterConsumido.Size = new Size(88, 18);
            rad_MasterConsumido.TabIndex = 8;
            rad_MasterConsumido.Text = "Consumido";
            rad_MasterConsumido.CheckedChanged += Rad_MasterConsumido_CheckedChanged;
            // 
            // rad_MasterParcial
            // 
            rad_MasterParcial.AutoSize = true;
            rad_MasterParcial.Location = new Point(6, 44);
            rad_MasterParcial.Name = "rad_MasterParcial";
            rad_MasterParcial.Size = new Size(121, 18);
            rad_MasterParcial.TabIndex = 7;
            rad_MasterParcial.Text = "Parcialmente Util.";
            rad_MasterParcial.CheckedChanged += Rad_MasterParcial_CheckedChanged;
            // 
            // rad_MasterCompleto
            // 
            rad_MasterCompleto.AutoSize = true;
            rad_MasterCompleto.Location = new Point(6, 28);
            rad_MasterCompleto.Name = "rad_MasterCompleto";
            rad_MasterCompleto.Size = new Size(78, 18);
            rad_MasterCompleto.TabIndex = 6;
            rad_MasterCompleto.Text = "Completo";
            rad_MasterCompleto.CheckedChanged += Rad_MasterCompleto_CheckedChanged;
            // 
            // btn_delete_master
            // 
            btn_delete_master.Image = (Image)resources.GetObject("btn_delete_master.Image");
            btn_delete_master.Location = new Point(880, 8);
            btn_delete_master.Name = "btn_delete_master";
            btn_delete_master.Size = new Size(120, 56);
            btn_delete_master.TabIndex = 17;
            btn_delete_master.Text = "Eliminar Master";
            btn_delete_master.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_delete_master.Click += Btn_delete_master_Click;
            // 
            // bot_printLabel
            // 
            bot_printLabel.Image = (Image)resources.GetObject("bot_printLabel.Image");
            bot_printLabel.Location = new Point(754, 6);
            bot_printLabel.Name = "bot_printLabel";
            bot_printLabel.Size = new Size(120, 56);
            bot_printLabel.TabIndex = 16;
            bot_printLabel.Text = "Imprimir Etiqueta";
            bot_printLabel.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_printLabel.Click += Bot_printLabel_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(736, 670);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(37, 34);
            pictureBox3.TabIndex = 15;
            pictureBox3.TabStop = false;
            pictureBox3.Click += PictureBox3_Click;
            // 
            // label11
            // 
            label11.AutoSize = true;
            label11.Location = new Point(94, 3);
            label11.Name = "label11";
            label11.Size = new Size(193, 14);
            label11.TabIndex = 14;
            label11.Text = "LISTADO INVENTARIO DE MASTER";
            // 
            // btn_limpiar_filtros
            // 
            btn_limpiar_filtros.Image = (Image)resources.GetObject("btn_limpiar_filtros.Image");
            btn_limpiar_filtros.Location = new Point(539, 8);
            btn_limpiar_filtros.Name = "btn_limpiar_filtros";
            btn_limpiar_filtros.Size = new Size(83, 53);
            btn_limpiar_filtros.TabIndex = 13;
            btn_limpiar_filtros.Text = "Limpiar";
            btn_limpiar_filtros.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_limpiar_filtros.Click += Btn_limpiar_filtros_Click;
            // 
            // COUNT_ROWS
            // 
            COUNT_ROWS.AutoSize = true;
            COUNT_ROWS.Location = new Point(779, 685);
            COUNT_ROWS.Name = "COUNT_ROWS";
            COUNT_ROWS.Size = new Size(139, 14);
            COUNT_ROWS.TabIndex = 12;
            COUNT_ROWS.Text = "0 Registros Encontrados";
            // 
            // btn_DetailsConsumos
            // 
            btn_DetailsConsumos.Image = (Image)resources.GetObject("btn_DetailsConsumos.Image");
            btn_DetailsConsumos.Location = new Point(628, 6);
            btn_DetailsConsumos.Name = "btn_DetailsConsumos";
            btn_DetailsConsumos.Size = new Size(120, 56);
            btn_DetailsConsumos.TabIndex = 11;
            btn_DetailsConsumos.Text = "Detalle Cosumos";
            btn_DetailsConsumos.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_DetailsConsumos.Click += Btn_DetailsConsumos_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(rad_rollid);
            groupBox3.Controls.Add(rad_ubication);
            groupBox3.Controls.Add(rad_productid);
            groupBox3.Controls.Add(rad_product_name);
            groupBox3.Location = new Point(10, 589);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(167, 115);
            groupBox3.TabIndex = 7;
            groupBox3.TabStop = false;
            groupBox3.Text = "Filtrar Por: ";
            // 
            // rad_rollid
            // 
            rad_rollid.AutoSize = true;
            rad_rollid.Checked = true;
            rad_rollid.Location = new Point(6, 28);
            rad_rollid.Name = "rad_rollid";
            rad_rollid.Size = new Size(58, 18);
            rad_rollid.TabIndex = 7;
            rad_rollid.TabStop = true;
            rad_rollid.Text = "Roll-Id";
            // 
            // rad_ubication
            // 
            rad_ubication.AutoSize = true;
            rad_ubication.Location = new Point(6, 84);
            rad_ubication.Name = "rad_ubication";
            rad_ubication.Size = new Size(101, 18);
            rad_ubication.TabIndex = 7;
            rad_ubication.Text = "Por Ubicación";
            // 
            // rad_productid
            // 
            rad_productid.AutoSize = true;
            rad_productid.Location = new Point(6, 47);
            rad_productid.Name = "rad_productid";
            rad_productid.Size = new Size(81, 18);
            rad_productid.TabIndex = 5;
            rad_productid.Text = "Product Id";
            // 
            // rad_product_name
            // 
            rad_product_name.AutoSize = true;
            rad_product_name.Location = new Point(6, 65);
            rad_product_name.Name = "rad_product_name";
            rad_product_name.Size = new Size(141, 18);
            rad_product_name.TabIndex = 6;
            rad_product_name.Text = "Nombre del Producto";
            // 
            // GridMaster
            // 
            GridMaster.AllowUserToAddRows = false;
            GridMaster.AllowUserToDeleteRows = false;
            GridMaster.AllowUserToResizeRows = false;
            GridMaster.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            GridMaster.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridMaster.Location = new Point(7, 70);
            GridMaster.MultiSelect = false;
            GridMaster.Name = "GridMaster";
            GridMaster.ReadOnly = true;
            GridMaster.RowHeadersWidth = 33;
            GridMaster.SelectionMode = DataGridViewSelectionMode.CellSelect;
            GridMaster.Size = new Size(996, 513);
            GridMaster.TabIndex = 3;
            GridMaster.CellContentClick += GridMaster_CellContentClick;
            GridMaster.CellFormatting += GridMaster_CellFormatting;
            // 
            // btn_buscar
            // 
            btn_buscar.Image = (Image)resources.GetObject("btn_buscar.Image");
            btn_buscar.Location = new Point(457, 8);
            btn_buscar.Name = "btn_buscar";
            btn_buscar.Size = new Size(76, 53);
            btn_buscar.TabIndex = 2;
            btn_buscar.Text = "Buscar";
            btn_buscar.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_buscar.Click += Btn_buscar_Click;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(7, 21);
            label8.Name = "label8";
            label8.Size = new Size(68, 14);
            label8.TabIndex = 1;
            label8.Text = "Buscar por:";
            // 
            // txt_buscar
            // 
            txt_buscar.Location = new Point(7, 37);
            txt_buscar.Name = "txt_buscar";
            txt_buscar.Size = new Size(444, 22);
            txt_buscar.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.ImageIndex = 1;
            tabPage2.Location = new Point(4, 26);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(1010, 762);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Graphics";
            // 
            // tabPage3
            // 
            tabPage3.ImageIndex = 3;
            tabPage3.Location = new Point(4, 26);
            tabPage3.Name = "tabPage3";
            tabPage3.Padding = new Padding(3);
            tabPage3.Size = new Size(1010, 762);
            tabPage3.TabIndex = 2;
            tabPage3.Text = "Hojas";
            // 
            // Page_RolloCortado
            // 
            Page_RolloCortado.Controls.Add(pictureBox4);
            Page_RolloCortado.Controls.Add(label12);
            Page_RolloCortado.Controls.Add(COUNTER_ROLLOS);
            Page_RolloCortado.Controls.Add(groupBox4);
            Page_RolloCortado.Controls.Add(GridRollosCortados);
            Page_RolloCortado.Controls.Add(bto_limpiar_cor);
            Page_RolloCortado.Controls.Add(bot_buscar_cor);
            Page_RolloCortado.Controls.Add(label10);
            Page_RolloCortado.Controls.Add(txt_buscar_cor);
            Page_RolloCortado.ImageIndex = 2;
            Page_RolloCortado.Location = new Point(4, 26);
            Page_RolloCortado.Name = "Page_RolloCortado";
            Page_RolloCortado.Padding = new Padding(3);
            Page_RolloCortado.Size = new Size(1010, 762);
            Page_RolloCortado.TabIndex = 3;
            Page_RolloCortado.Text = "Rollos Cortados";
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(745, 466);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(37, 34);
            pictureBox4.TabIndex = 24;
            pictureBox4.TabStop = false;
            // 
            // label12
            // 
            label12.AutoSize = true;
            label12.Location = new Point(92, 6);
            label12.Name = "label12";
            label12.Size = new Size(271, 14);
            label12.TabIndex = 23;
            label12.Text = "LISTADO DE INVENTARIO DE ROLLOS CORTADOS";
            // 
            // COUNTER_ROLLOS
            // 
            COUNTER_ROLLOS.AutoSize = true;
            COUNTER_ROLLOS.Location = new Point(788, 481);
            COUNTER_ROLLOS.Name = "COUNTER_ROLLOS";
            COUNTER_ROLLOS.Size = new Size(160, 14);
            COUNTER_ROLLOS.TabIndex = 22;
            COUNTER_ROLLOS.Text = "0000 Registros Encontrados";
            // 
            // groupBox4
            // 
            groupBox4.Controls.Add(rad_ordencorte_cor);
            groupBox4.Controls.Add(rad_codeperson_cor);
            groupBox4.Controls.Add(rad_codeunique_cor);
            groupBox4.Controls.Add(rad_rollid_cor);
            groupBox4.Controls.Add(rad_ubic_cor);
            groupBox4.Controls.Add(rad_productid_cor);
            groupBox4.Controls.Add(rad_productname_cor);
            groupBox4.Location = new Point(6, 466);
            groupBox4.Name = "groupBox4";
            groupBox4.Size = new Size(245, 174);
            groupBox4.TabIndex = 21;
            groupBox4.TabStop = false;
            groupBox4.Text = "Filtrar Por: ";
            // 
            // rad_ordencorte_cor
            // 
            rad_ordencorte_cor.AutoSize = true;
            rad_ordencorte_cor.Location = new Point(6, 138);
            rad_ordencorte_cor.Name = "rad_ordencorte_cor";
            rad_ordencorte_cor.Size = new Size(89, 18);
            rad_ordencorte_cor.TabIndex = 10;
            rad_ordencorte_cor.Text = "Orden Corte";
            // 
            // rad_codeperson_cor
            // 
            rad_codeperson_cor.AutoSize = true;
            rad_codeperson_cor.Location = new Point(6, 121);
            rad_codeperson_cor.Name = "rad_codeperson_cor";
            rad_codeperson_cor.Size = new Size(167, 18);
            rad_codeperson_cor.TabIndex = 9;
            rad_codeperson_cor.Text = "Por Codigo Personalizado";
            // 
            // rad_codeunique_cor
            // 
            rad_codeunique_cor.AutoSize = true;
            rad_codeunique_cor.Location = new Point(6, 103);
            rad_codeunique_cor.Name = "rad_codeunique_cor";
            rad_codeunique_cor.Size = new Size(120, 18);
            rad_codeunique_cor.TabIndex = 8;
            rad_codeunique_cor.Text = "Por Codigo Unico";
            // 
            // rad_rollid_cor
            // 
            rad_rollid_cor.AutoSize = true;
            rad_rollid_cor.Checked = true;
            rad_rollid_cor.Location = new Point(6, 28);
            rad_rollid_cor.Name = "rad_rollid_cor";
            rad_rollid_cor.Size = new Size(58, 18);
            rad_rollid_cor.TabIndex = 7;
            rad_rollid_cor.TabStop = true;
            rad_rollid_cor.Text = "Roll-Id";
            // 
            // rad_ubic_cor
            // 
            rad_ubic_cor.AutoSize = true;
            rad_ubic_cor.Location = new Point(6, 84);
            rad_ubic_cor.Name = "rad_ubic_cor";
            rad_ubic_cor.Size = new Size(101, 18);
            rad_ubic_cor.TabIndex = 7;
            rad_ubic_cor.Text = "Por Ubicación";
            // 
            // rad_productid_cor
            // 
            rad_productid_cor.AutoSize = true;
            rad_productid_cor.Location = new Point(6, 47);
            rad_productid_cor.Name = "rad_productid_cor";
            rad_productid_cor.Size = new Size(81, 18);
            rad_productid_cor.TabIndex = 5;
            rad_productid_cor.Text = "Product Id";
            // 
            // rad_productname_cor
            // 
            rad_productname_cor.AutoSize = true;
            rad_productname_cor.Location = new Point(6, 65);
            rad_productname_cor.Name = "rad_productname_cor";
            rad_productname_cor.Size = new Size(141, 18);
            rad_productname_cor.TabIndex = 6;
            rad_productname_cor.Text = "Nombre del Producto";
            // 
            // GridRollosCortados
            // 
            GridRollosCortados.AllowUserToAddRows = false;
            GridRollosCortados.AllowUserToDeleteRows = false;
            GridRollosCortados.AllowUserToResizeColumns = false;
            GridRollosCortados.AllowUserToResizeRows = false;
            GridRollosCortados.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            GridRollosCortados.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            GridRollosCortados.Location = new Point(6, 67);
            GridRollosCortados.MultiSelect = false;
            GridRollosCortados.Name = "GridRollosCortados";
            GridRollosCortados.ReadOnly = true;
            GridRollosCortados.RowHeadersWidth = 34;
            GridRollosCortados.SelectionMode = DataGridViewSelectionMode.CellSelect;
            GridRollosCortados.Size = new Size(996, 393);
            GridRollosCortados.TabIndex = 18;
            GridRollosCortados.CellContentClick += GridRollosCortados_CellContentClick;
            GridRollosCortados.CellFormatting += GridRollosCortados_CellFormatting;
            // 
            // bto_limpiar_cor
            // 
            bto_limpiar_cor.Image = (Image)resources.GetObject("bto_limpiar_cor.Image");
            bto_limpiar_cor.Location = new Point(1062, 21);
            bto_limpiar_cor.Name = "bto_limpiar_cor";
            bto_limpiar_cor.Size = new Size(95, 40);
            bto_limpiar_cor.TabIndex = 17;
            bto_limpiar_cor.Text = "Limpiar";
            bto_limpiar_cor.TextImageRelation = TextImageRelation.ImageBeforeText;
            bto_limpiar_cor.Click += Bto_limpiar_cor_Click;
            // 
            // bot_buscar_cor
            // 
            bot_buscar_cor.Image = (Image)resources.GetObject("bot_buscar_cor.Image");
            bot_buscar_cor.Location = new Point(774, 21);
            bot_buscar_cor.Name = "bot_buscar_cor";
            bot_buscar_cor.Size = new Size(75, 40);
            bot_buscar_cor.TabIndex = 16;
            bot_buscar_cor.Text = "Buscar";
            bot_buscar_cor.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_buscar_cor.Click += Bot_buscar_cor_Click;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(6, 23);
            label10.Name = "label10";
            label10.Size = new Size(68, 14);
            label10.TabIndex = 15;
            label10.Text = "Buscar por:";
            // 
            // txt_buscar_cor
            // 
            txt_buscar_cor.Location = new Point(6, 39);
            txt_buscar_cor.Name = "txt_buscar_cor";
            txt_buscar_cor.Size = new Size(762, 22);
            txt_buscar_cor.TabIndex = 14;
            // 
            // tabPage5
            // 
            tabPage5.Controls.Add(groupBox1);
            tabPage5.ImageIndex = 5;
            tabPage5.Location = new Point(4, 26);
            tabPage5.Name = "tabPage5";
            tabPage5.Padding = new Padding(3);
            tabPage5.Size = new Size(1010, 762);
            tabPage5.TabIndex = 4;
            tabPage5.Text = "Cargar Inventario";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btn_clearGrid);
            groupBox1.Controls.Add(label15);
            groupBox1.Controls.Add(txt_log_notifications);
            groupBox1.Controls.Add(groupBox9);
            groupBox1.Controls.Add(groupBox8);
            groupBox1.Controls.Add(txt_errors);
            groupBox1.Controls.Add(NUMBERS_NOTIFICATIONS);
            groupBox1.Controls.Add(txt_filePath);
            groupBox1.Controls.Add(label7);
            groupBox1.Controls.Add(txt_fileName);
            groupBox1.Controls.Add(label4);
            groupBox1.Controls.Add(txt_warning);
            groupBox1.Controls.Add(txt_number_rows);
            groupBox1.Controls.Add(label9);
            groupBox1.Controls.Add(label14);
            groupBox1.Controls.Add(groupBox7);
            groupBox1.Controls.Add(btn_saveDatabase);
            groupBox1.Controls.Add(btn_load_data);
            groupBox1.Controls.Add(Grid_Items);
            groupBox1.Controls.Add(btn_search);
            groupBox1.Controls.Add(textBox1);
            groupBox1.Controls.Add(label6);
            groupBox1.Controls.Add(groupBox5);
            groupBox1.Controls.Add(label5);
            groupBox1.Controls.Add(groupBox2);
            groupBox1.Controls.Add(btn_load_sheet);
            groupBox1.Controls.Add(label3);
            groupBox1.Controls.Add(txt_file_path);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(txt_file_name);
            groupBox1.Location = new Point(12, 15);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(990, 743);
            groupBox1.TabIndex = 2;
            groupBox1.TabStop = false;
            groupBox1.Text = "Importar Data de Excel";
            groupBox1.Enter += GroupBox1_Enter;
            // 
            // btn_clearGrid
            // 
            btn_clearGrid.Image = Properties.Resources.update_doc;
            btn_clearGrid.Location = new Point(827, 348);
            btn_clearGrid.Name = "btn_clearGrid";
            btn_clearGrid.Size = new Size(145, 73);
            btn_clearGrid.TabIndex = 48;
            btn_clearGrid.Text = "Limpiar Data";
            btn_clearGrid.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_clearGrid.Click += Btn_clearGrid_Click;
            // 
            // label15
            // 
            label15.Location = new Point(17, 585);
            label15.Name = "label15";
            label15.Size = new Size(169, 26);
            label15.TabIndex = 39;
            label15.Text = "NOTIFICACIONES :";
            // 
            // txt_log_notifications
            // 
            txt_log_notifications.Location = new Point(17, 614);
            txt_log_notifications.Name = "txt_log_notifications";
            txt_log_notifications.ReadOnly = true;
            txt_log_notifications.Size = new Size(961, 123);
            txt_log_notifications.TabIndex = 47;
            txt_log_notifications.Text = "";
            // 
            // groupBox9
            // 
            groupBox9.Controls.Add(btn_accion);
            groupBox9.Controls.Add(chk_saveproductsnotfound);
            groupBox9.Location = new Point(718, 487);
            groupBox9.Name = "groupBox9";
            groupBox9.Size = new Size(200, 121);
            groupBox9.TabIndex = 46;
            groupBox9.TabStop = false;
            groupBox9.Text = "Acciones";
            // 
            // btn_accion
            // 
            btn_accion.Enabled = false;
            btn_accion.FlatStyle = FlatStyle.Flat;
            btn_accion.Image = Properties.Resources.DATA_RESERVA48;
            btn_accion.Location = new Point(23, 45);
            btn_accion.Name = "btn_accion";
            btn_accion.Size = new Size(145, 62);
            btn_accion.TabIndex = 28;
            btn_accion.Text = "Ejecutar";
            btn_accion.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_accion.Click += Btn_accion_Click;
            // 
            // chk_saveproductsnotfound
            // 
            chk_saveproductsnotfound.AutoSize = true;
            chk_saveproductsnotfound.Enabled = false;
            chk_saveproductsnotfound.Location = new Point(5, 20);
            chk_saveproductsnotfound.Name = "chk_saveproductsnotfound";
            chk_saveproductsnotfound.Size = new Size(185, 18);
            chk_saveproductsnotfound.TabIndex = 11;
            chk_saveproductsnotfound.Text = "Crear Productos si no Existen";
            // 
            // groupBox8
            // 
            groupBox8.Controls.Add(chk_valid_products);
            groupBox8.Controls.Add(chk_repeat_rollid);
            groupBox8.Location = new Point(512, 484);
            groupBox8.Name = "groupBox8";
            groupBox8.Size = new Size(200, 124);
            groupBox8.TabIndex = 45;
            groupBox8.TabStop = false;
            groupBox8.Text = "Validaciones";
            // 
            // chk_valid_products
            // 
            chk_valid_products.AutoSize = true;
            chk_valid_products.Checked = true;
            chk_valid_products.CheckState = CheckState.Checked;
            chk_valid_products.Location = new Point(6, 36);
            chk_valid_products.Name = "chk_valid_products";
            chk_valid_products.Size = new Size(178, 18);
            chk_valid_products.TabIndex = 11;
            chk_valid_products.Text = "Validacion de los Productos";
            // 
            // chk_repeat_rollid
            // 
            chk_repeat_rollid.AutoSize = true;
            chk_repeat_rollid.Checked = true;
            chk_repeat_rollid.CheckState = CheckState.Checked;
            chk_repeat_rollid.Location = new Point(6, 60);
            chk_repeat_rollid.Name = "chk_repeat_rollid";
            chk_repeat_rollid.Size = new Size(163, 18);
            chk_repeat_rollid.TabIndex = 19;
            chk_repeat_rollid.Text = "Validar Repeticion Roll-Id ";
            // 
            // txt_errors
            // 
            txt_errors.Location = new Point(251, 585);
            txt_errors.Name = "txt_errors";
            txt_errors.ReadOnly = true;
            txt_errors.Size = new Size(255, 22);
            txt_errors.TabIndex = 44;
            txt_errors.Text = "0";
            // 
            // NUMBERS_NOTIFICATIONS
            // 
            NUMBERS_NOTIFICATIONS.AutoSize = true;
            NUMBERS_NOTIFICATIONS.Location = new Point(192, 588);
            NUMBERS_NOTIFICATIONS.Name = "NUMBERS_NOTIFICATIONS";
            NUMBERS_NOTIFICATIONS.Size = new Size(51, 14);
            NUMBERS_NOTIFICATIONS.TabIndex = 43;
            NUMBERS_NOTIFICATIONS.Text = "Errores :";
            // 
            // txt_filePath
            // 
            txt_filePath.Location = new Point(251, 560);
            txt_filePath.Name = "txt_filePath";
            txt_filePath.ReadOnly = true;
            txt_filePath.Size = new Size(255, 22);
            txt_filePath.TabIndex = 42;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(208, 563);
            label7.Name = "label7";
            label7.Size = new Size(38, 14);
            label7.TabIndex = 41;
            label7.Text = "Ruta :";
            // 
            // txt_fileName
            // 
            txt_fileName.Location = new Point(251, 534);
            txt_fileName.Name = "txt_fileName";
            txt_fileName.ReadOnly = true;
            txt_fileName.Size = new Size(255, 22);
            txt_fileName.TabIndex = 40;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(144, 537);
            label4.Name = "label4";
            label4.Size = new Size(101, 14);
            label4.TabIndex = 39;
            label4.Text = "Nombre Archivo :";
            // 
            // txt_warning
            // 
            txt_warning.Location = new Point(251, 508);
            txt_warning.Name = "txt_warning";
            txt_warning.ReadOnly = true;
            txt_warning.Size = new Size(255, 22);
            txt_warning.TabIndex = 38;
            // 
            // txt_number_rows
            // 
            txt_number_rows.Location = new Point(251, 484);
            txt_number_rows.Name = "txt_number_rows";
            txt_number_rows.ReadOnly = true;
            txt_number_rows.Size = new Size(255, 22);
            txt_number_rows.TabIndex = 37;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(164, 511);
            label9.Name = "label9";
            label9.Size = new Size(83, 14);
            label9.TabIndex = 36;
            label9.Text = "Advertencias :";
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(144, 487);
            label14.Name = "label14";
            label14.Size = new Size(102, 14);
            label14.TabIndex = 35;
            label14.Text = "Numero de Filas :";
            // 
            // groupBox7
            // 
            groupBox7.Controls.Add(radioButton1);
            groupBox7.Controls.Add(radioButton2);
            groupBox7.Controls.Add(radioButton3);
            groupBox7.Location = new Point(11, 484);
            groupBox7.Name = "groupBox7";
            groupBox7.Size = new Size(129, 87);
            groupBox7.TabIndex = 34;
            groupBox7.TabStop = false;
            groupBox7.Text = "Filtrar por: ";
            // 
            // radioButton1
            // 
            radioButton1.AutoSize = true;
            radioButton1.Location = new Point(6, 54);
            radioButton1.Name = "radioButton1";
            radioButton1.Size = new Size(58, 18);
            radioButton1.TabIndex = 10;
            radioButton1.TabStop = true;
            radioButton1.Text = "Roll-Id";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(6, 38);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(122, 18);
            radioButton2.TabIndex = 9;
            radioButton2.TabStop = true;
            radioButton2.Text = "Nombre Producto";
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(6, 22);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(84, 18);
            radioButton3.TabIndex = 8;
            radioButton3.TabStop = true;
            radioButton3.Text = "Product Id.";
            // 
            // btn_saveDatabase
            // 
            btn_saveDatabase.Image = (Image)resources.GetObject("btn_saveDatabase.Image");
            btn_saveDatabase.Location = new Point(827, 269);
            btn_saveDatabase.Name = "btn_saveDatabase";
            btn_saveDatabase.Size = new Size(145, 73);
            btn_saveDatabase.TabIndex = 33;
            btn_saveDatabase.Text = "Guardar BD";
            btn_saveDatabase.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_saveDatabase.Click += Btn_saveDatabase_Click;
            // 
            // btn_load_data
            // 
            btn_load_data.Image = (Image)resources.GetObject("btn_load_data.Image");
            btn_load_data.Location = new Point(828, 190);
            btn_load_data.Name = "btn_load_data";
            btn_load_data.Size = new Size(145, 73);
            btn_load_data.TabIndex = 32;
            btn_load_data.Text = "Cargar Datos";
            btn_load_data.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_load_data.Click += Btn_load_data_Click;
            // 
            // Grid_Items
            // 
            Grid_Items.AllowUserToAddRows = false;
            Grid_Items.AllowUserToDeleteRows = false;
            Grid_Items.AllowUserToOrderColumns = true;
            Grid_Items.AllowUserToResizeRows = false;
            Grid_Items.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Grid_Items.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Grid_Items.Location = new Point(11, 190);
            Grid_Items.MultiSelect = false;
            Grid_Items.Name = "Grid_Items";
            Grid_Items.ReadOnly = true;
            Grid_Items.RowHeadersWidth = 32;
            Grid_Items.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid_Items.Size = new Size(811, 288);
            Grid_Items.TabIndex = 30;
            // 
            // btn_search
            // 
            btn_search.Image = (Image)resources.GetObject("btn_search.Image");
            btn_search.Location = new Point(707, 161);
            btn_search.Name = "btn_search";
            btn_search.Size = new Size(115, 23);
            btn_search.TabIndex = 29;
            btn_search.Text = "Buscar";
            btn_search.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(135, 161);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(566, 22);
            textBox1.TabIndex = 28;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(3, 165);
            label6.Name = "label6";
            label6.Size = new Size(122, 14);
            label6.TabIndex = 27;
            label6.Text = "Buscar Producto Por:";
            // 
            // groupBox5
            // 
            groupBox5.Controls.Add(label16);
            groupBox5.Controls.Add(cbo_tabla);
            groupBox5.Controls.Add(label22);
            groupBox5.Controls.Add(btn_dropmaster);
            groupBox5.Location = new Point(509, 20);
            groupBox5.Name = "groupBox5";
            groupBox5.Size = new Size(313, 141);
            groupBox5.TabIndex = 7;
            groupBox5.TabStop = false;
            groupBox5.Text = "Inicializar Tabla";
            // 
            // label16
            // 
            label16.AutoSize = true;
            label16.Location = new Point(9, 88);
            label16.Name = "label16";
            label16.Size = new Size(106, 14);
            label16.TabIndex = 49;
            label16.Text = "Nombre de Tabla :";
            // 
            // cbo_tabla
            // 
            cbo_tabla.FormattingEnabled = true;
            cbo_tabla.Items.AddRange(new object[] { "Master", "Rollo Cortados" });
            cbo_tabla.Location = new Point(9, 106);
            cbo_tabla.Name = "cbo_tabla";
            cbo_tabla.Size = new Size(156, 22);
            cbo_tabla.TabIndex = 49;
            // 
            // label22
            // 
            label22.Location = new Point(6, 19);
            label22.Name = "label22";
            label22.Size = new Size(301, 51);
            label22.TabIndex = 38;
            label22.Text = "Este proceso es delicado porque borra todos los datos de los inventarios de master, debe estar seguro porque es un proceso irreversible.\r\n\r\n";
            // 
            // btn_dropmaster
            // 
            btn_dropmaster.Image = Properties.Resources.multiply_32px;
            btn_dropmaster.Location = new Point(171, 78);
            btn_dropmaster.Name = "btn_dropmaster";
            btn_dropmaster.Size = new Size(136, 51);
            btn_dropmaster.TabIndex = 0;
            btn_dropmaster.Text = "Inicializar Tabla";
            btn_dropmaster.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_dropmaster.Click += Btn_dropmaster_Click;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(1608, 469);
            label5.Name = "label5";
            label5.Size = new Size(119, 14);
            label5.TabIndex = 26;
            label5.Text = "Ruta de Localizacion";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(rad_rollos);
            groupBox2.Controls.Add(rad_hojas);
            groupBox2.Controls.Add(rad_graphics);
            groupBox2.Controls.Add(rad_master);
            groupBox2.Location = new Point(6, 110);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(349, 51);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Tipo Producto";
            // 
            // rad_rollos
            // 
            rad_rollos.AutoSize = true;
            rad_rollos.Location = new Point(242, 20);
            rad_rollos.Name = "rad_rollos";
            rad_rollos.Size = new Size(99, 18);
            rad_rollos.TabIndex = 3;
            rad_rollos.TabStop = true;
            rad_rollos.Text = "Rollo Cortado";
            rad_rollos.CheckedChanged += Rad_rollos_CheckedChanged_1;
            // 
            // rad_hojas
            // 
            rad_hojas.AutoSize = true;
            rad_hojas.Location = new Point(178, 20);
            rad_hojas.Name = "rad_hojas";
            rad_hojas.Size = new Size(57, 18);
            rad_hojas.TabIndex = 2;
            rad_hojas.TabStop = true;
            rad_hojas.Text = "Hojas";
            // 
            // rad_graphics
            // 
            rad_graphics.AutoSize = true;
            rad_graphics.Location = new Point(96, 20);
            rad_graphics.Name = "rad_graphics";
            rad_graphics.Size = new Size(73, 18);
            rad_graphics.TabIndex = 1;
            rad_graphics.TabStop = true;
            rad_graphics.Text = "Graphics";
            // 
            // rad_master
            // 
            rad_master.AutoSize = true;
            rad_master.Location = new Point(25, 20);
            rad_master.Name = "rad_master";
            rad_master.Size = new Size(62, 18);
            rad_master.TabIndex = 0;
            rad_master.TabStop = true;
            rad_master.Text = "Master";
            rad_master.CheckedChanged += Rad_master_CheckedChanged;
            // 
            // btn_load_sheet
            // 
            btn_load_sheet.Image = (Image)resources.GetObject("btn_load_sheet.Image");
            btn_load_sheet.Location = new Point(361, 34);
            btn_load_sheet.Name = "btn_load_sheet";
            btn_load_sheet.Size = new Size(142, 64);
            btn_load_sheet.TabIndex = 4;
            btn_load_sheet.Text = "Buscar Hoja";
            btn_load_sheet.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_load_sheet.Click += Btn_load_sheet_Click;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(11, 63);
            label3.Name = "label3";
            label3.Size = new Size(119, 14);
            label3.TabIndex = 3;
            label3.Text = "Ruta de Localizacion";
            // 
            // txt_file_path
            // 
            txt_file_path.Location = new Point(6, 81);
            txt_file_path.Name = "txt_file_path";
            txt_file_path.ReadOnly = true;
            txt_file_path.Size = new Size(349, 22);
            txt_file_path.TabIndex = 2;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(11, 20);
            label2.Name = "label2";
            label2.Size = new Size(114, 14);
            label2.TabIndex = 1;
            label2.Text = "Nombre del Archivo";
            // 
            // txt_file_name
            // 
            txt_file_name.Location = new Point(6, 38);
            txt_file_name.Name = "txt_file_name";
            txt_file_name.ReadOnly = true;
            txt_file_name.Size = new Size(349, 22);
            txt_file_name.TabIndex = 0;
            // 
            // tabPage6
            // 
            tabPage6.ImageIndex = 4;
            tabPage6.Location = new Point(4, 26);
            tabPage6.Name = "tabPage6";
            tabPage6.Padding = new Padding(3);
            tabPage6.Size = new Size(1010, 762);
            tabPage6.TabIndex = 5;
            tabPage6.Text = "Notificaciones";
            // 
            // imageList1
            // 
            imageList1.ColorDepth = ColorDepth.Depth32Bit;
            imageList1.ImageStream = (ImageListStreamer)resources.GetObject("imageList1.ImageStream");
            imageList1.TransparentColor = Color.Transparent;
            imageList1.Images.SetKeyName(0, "product16.ico");
            imageList1.Images.SetKeyName(1, "check_file_bw_16.png");
            imageList1.Images.SetKeyName(2, "FilterDocument.ico");
            imageList1.Images.SetKeyName(3, "insert_rows.ico");
            imageList1.Images.SetKeyName(4, "package_search.ico");
            imageList1.Images.SetKeyName(5, "microsoft_excel_2019_16.png");
            imageList1.Images.SetKeyName(6, "unpacking.ico");
            imageList1.Images.SetKeyName(7, "user_rights.ico");
            imageList1.Images.SetKeyName(8, "Remove_16.png");
            imageList1.Images.SetKeyName(9, "ordenDocument_16.png");
            imageList1.Images.SetKeyName(10, "CloseDoc.ico");
            // 
            // PANEL_TITULO
            // 
            PANEL_TITULO.Controls.Add(label13);
            PANEL_TITULO.Controls.Add(ComboPrinters);
            PANEL_TITULO.Controls.Add(pictureBox5);
            PANEL_TITULO.Controls.Add(pictureBox1);
            PANEL_TITULO.Controls.Add(label1);
            PANEL_TITULO.Dock = DockStyle.Top;
            PANEL_TITULO.Location = new Point(0, 35);
            PANEL_TITULO.Name = "PANEL_TITULO";
            PANEL_TITULO.Size = new Size(1030, 93);
            PANEL_TITULO.TabIndex = 1;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(780, 30);
            label13.Name = "label13";
            label13.Size = new Size(148, 14);
            label13.TabIndex = 27;
            label13.Text = "Seleccione la Imporesora :";
            // 
            // ComboPrinters
            // 
            ComboPrinters.FormattingEnabled = true;
            ComboPrinters.Location = new Point(780, 52);
            ComboPrinters.Name = "ComboPrinters";
            ComboPrinters.Size = new Size(246, 22);
            ComboPrinters.TabIndex = 26;
            // 
            // pictureBox5
            // 
            pictureBox5.Image = (Image)resources.GetObject("pictureBox5.Image");
            pictureBox5.Location = new Point(12, 24);
            pictureBox5.Name = "pictureBox5";
            pictureBox5.Size = new Size(245, 50);
            pictureBox5.TabIndex = 2;
            pictureBox5.TabStop = false;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(279, 24);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(50, 47);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(335, 21);
            label1.Name = "label1";
            label1.Size = new Size(126, 14);
            label1.TabIndex = 0;
            label1.Text = "Control de Inventarios";
            // 
            // toolStrip1
            // 
            toolStrip1.Items.AddRange(new ToolStripItem[] { Btn_reload, Bot_Reports, Bot_Excel, Bot_Txt });
            toolStrip1.Location = new Point(0, 128);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.RenderMode = ToolStripRenderMode.Professional;
            toolStrip1.Size = new Size(1030, 33);
            toolStrip1.TabIndex = 2;
            toolStrip1.Text = "toolStrip1";
            toolStrip1.ItemClicked += ToolStrip1_ItemClicked;
            // 
            // Btn_reload
            // 
            Btn_reload.AutoSize = false;
            Btn_reload.Image = (Image)resources.GetObject("Btn_reload.Image");
            Btn_reload.ImageTransparentColor = Color.Magenta;
            Btn_reload.Name = "Btn_reload";
            Btn_reload.Size = new Size(100, 30);
            Btn_reload.Text = "Cargar";
            Btn_reload.Click += Btn_reload_Click;
            // 
            // Bot_Reports
            // 
            Bot_Reports.AutoSize = false;
            Bot_Reports.Image = (Image)resources.GetObject("Bot_Reports.Image");
            Bot_Reports.ImageTransparentColor = Color.Magenta;
            Bot_Reports.Name = "Bot_Reports";
            Bot_Reports.Size = new Size(100, 30);
            Bot_Reports.Text = "Reportes";
            Bot_Reports.Click += Bot_Reports_Click;
            // 
            // Bot_Excel
            // 
            Bot_Excel.AutoSize = false;
            Bot_Excel.Image = (Image)resources.GetObject("Bot_Excel.Image");
            Bot_Excel.ImageTransparentColor = Color.Magenta;
            Bot_Excel.Name = "Bot_Excel";
            Bot_Excel.Size = new Size(100, 30);
            Bot_Excel.Text = "Excel";
            Bot_Excel.Click += Bot_Excel_Click;
            // 
            // Bot_Txt
            // 
            Bot_Txt.AutoSize = false;
            Bot_Txt.Image = (Image)resources.GetObject("Bot_Txt.Image");
            Bot_Txt.ImageTransparentColor = Color.Magenta;
            Bot_Txt.Name = "Bot_Txt";
            Bot_Txt.Size = new Size(100, 30);
            Bot_Txt.Text = "Texto";
            Bot_Txt.Click += Bot_Txt_Click;
            // 
            // panel_loading
            // 
            panel_loading.Controls.Add(text_loadingindicator);
            panel_loading.Controls.Add(pictureBox2);
            panel_loading.Location = new Point(415, 421);
            panel_loading.Name = "panel_loading";
            panel_loading.Size = new Size(200, 83);
            panel_loading.TabIndex = 27;
            panel_loading.Visible = false;
            // 
            // text_loadingindicator
            // 
            text_loadingindicator.AutoSize = true;
            text_loadingindicator.Location = new Point(90, 37);
            text_loadingindicator.Name = "text_loadingindicator";
            text_loadingindicator.Size = new Size(60, 14);
            text_loadingindicator.TabIndex = 1;
            text_loadingindicator.Text = "Loading...";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(13, 12);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(64, 64);
            pictureBox2.SizeMode = PictureBoxSizeMode.AutoSize;
            pictureBox2.TabIndex = 0;
            pictureBox2.TabStop = false;
            pictureBox2.WaitOnLoad = true;
            // 
            // Frm_Inventarios
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(1030, 924);
            Controls.Add(panel_loading);
            Controls.Add(toolStrip1);
            Controls.Add(PANEL_TITULO);
            Controls.Add(TabPages_Inventario);
            Font = new Font("Roboto", 9F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Frm_Inventarios";
            Text = "Control de Inventarios:";
            ZoomScaleRect = new Rectangle(15, 15, 1030, 924);
            Load += Frm_Inventarios_Load;
            TabPages_Inventario.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            tabPage1.PerformLayout();
            groupBox6.ResumeLayout(false);
            groupBox6.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)GridMaster).EndInit();
            Page_RolloCortado.ResumeLayout(false);
            Page_RolloCortado.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            groupBox4.ResumeLayout(false);
            groupBox4.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)GridRollosCortados).EndInit();
            tabPage5.ResumeLayout(false);
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox9.ResumeLayout(false);
            groupBox9.PerformLayout();
            groupBox8.ResumeLayout(false);
            groupBox8.PerformLayout();
            groupBox7.ResumeLayout(false);
            groupBox7.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).EndInit();
            groupBox5.ResumeLayout(false);
            groupBox5.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            PANEL_TITULO.ResumeLayout(false);
            PANEL_TITULO.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox5).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            panel_loading.ResumeLayout(false);
            panel_loading.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TabControl TabPages_Inventario;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private TabPage tabPage3;
        private TabPage Page_RolloCortado;
        private Label label1;
        private GroupBox groupBox1;
        private Button btn_load_sheet;
        private Label label3;
        private TextBox txt_file_path;
        private Label label2;
        private TextBox txt_file_name;
        private PictureBox pictureBox1;
        private TabPage tabPage5;
        private GroupBox groupBox2;
        private RadioButton rad_rollos;
        private RadioButton rad_hojas;
        private RadioButton rad_graphics;
        private RadioButton rad_master;
        private TabPage tabPage6;
        private ToolStrip toolStrip1;
        private ToolStripButton Btn_reload;
        private GroupBox groupBox3;
        private RadioButton rad_product_name;
        private RadioButton rad_productid;
        private DataGridView GridMaster;
        private Button btn_buscar;
        private Label label8;
        private TextBox txt_buscar;
        private RadioButton rad_rollid;
        private Button btn_DetailsConsumos;
        private RadioButton rad_ubication;
        private Label COUNT_ROWS;
        private ToolStripButton Bot_Reports;
        private ToolStripButton Bot_Txt;
        private Button btn_limpiar_filtros;
        private ToolStripButton Bot_Excel;
        private Button bto_limpiar_cor;
        private Label label10;
        private TextBox txt_buscar_cor;
        private GroupBox groupBox4;
        private RadioButton rad_rollid_cor;
        private RadioButton rad_ubic_cor;
        private RadioButton rad_productid_cor;
        private RadioButton rad_productname_cor;
        private DataGridView GridRollosCortados;
        private Label COUNTER_ROLLOS;
        private RadioButton rad_codeperson_cor;
        private RadioButton rad_codeunique_cor;
        private Button bot_buscar_cor;
        private RadioButton rad_ordencorte_cor;
        private Label label11;
        private Label label12;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private PictureBox pictureBox5;
        private ImageList imageList1;
        private Button bot_printLabel;
        private Button btn_delete_master;
        private ComboBox ComboPrinters;
        private Label label13;
        private GroupBox groupBox6;
        private RadioButton rad_MasterCompleto;
        private RadioButton rad_MasterParcial;
        private RadioButton rad_MasterConsumido;
        private Label label5;
        private GroupBox groupBox5;
        private Label label22;
        private Button btn_dropmaster;
        private Button btn_search;
        private TextBox textBox1;
        private Label label6;
        private DataGridView Grid_Items;
        private Button btn_saveDatabase;
        private Button btn_load_data;
        private GroupBox groupBox7;
        private RadioButton radioButton1;
        private RadioButton radioButton2;
        private RadioButton radioButton3;
        private TextBox txt_errors;
        private Label NUMBERS_NOTIFICATIONS;
        private TextBox txt_filePath;
        private Label label7;
        private TextBox txt_fileName;
        private Label label4;
        private TextBox txt_warning;
        private TextBox txt_number_rows;
        private Label label9;
        private Label label14;
        private GroupBox groupBox8;
        private CheckBox chk_valid_products;
        private CheckBox chk_repeat_rollid;
        private GroupBox groupBox9;
        private Button btn_accion;
        private CheckBox chk_saveproductsnotfound;
        private RichTextBox txt_log_notifications;
        private Label label15;
        private Panel panel_loading;
        private Label text_loadingindicator;
        private PictureBox pictureBox2;
        private Button btn_clearGrid;
        private ComboBox cbo_tabla;
        private Label label16;
        private Panel PANEL_TITULO;
    }
}


