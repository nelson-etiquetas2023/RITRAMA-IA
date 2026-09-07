namespace Ritrama2025.Forms.Otros
{
    partial class Frm_Imports
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_Imports));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label5 = new Label();
            label1 = new Label();
            txt_buscar = new TextBox();
            btn_search = new Button();
            Grid_Items = new DataGridView();
            txt_log_notifications = new RichTextBox();
            label2 = new Label();
            groupBox1 = new GroupBox();
            rad_rollid = new RadioButton();
            rad_product_name = new RadioButton();
            rad_productid = new RadioButton();
            label3 = new Label();
            label4 = new Label();
            btn_load_data = new Button();
            chk_valid_products = new CheckBox();
            txt_number_rows = new TextBox();
            txt_warning = new TextBox();
            txt_fileName = new TextBox();
            label6 = new Label();
            txt_filePath = new TextBox();
            label7 = new Label();
            btn_saveDatabase = new Button();
            checkBox1 = new CheckBox();
            button1 = new Button();
            NUMBERS_NOTIFICATIONS = new Label();
            txt_errors = new TextBox();
            groupBox2 = new GroupBox();
            groupBox3 = new GroupBox();
            btn_accion = new Button();
            chk_saveproductsnotfound = new CheckBox();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label5);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(985, 100);
            panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(146, 19);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(57, 51);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(209, 20);
            label5.Name = "label5";
            label5.Size = new Size(401, 50);
            label5.TabIndex = 0;
            label5.Text = "Importacion de Datos";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 121);
            label1.Name = "label1";
            label1.Size = new Size(118, 15);
            label1.TabIndex = 1;
            label1.Text = "Buscar Producto Por:";
            // 
            // txt_buscar
            // 
            txt_buscar.Location = new Point(136, 118);
            txt_buscar.Name = "txt_buscar";
            txt_buscar.Size = new Size(566, 23);
            txt_buscar.TabIndex = 2;
            // 
            // btn_search
            // 
            btn_search.Image = (Image)resources.GetObject("btn_search.Image");
            btn_search.Location = new Point(708, 118);
            btn_search.Name = "btn_search";
            btn_search.Size = new Size(115, 23);
            btn_search.TabIndex = 3;
            btn_search.Text = "Buscar";
            btn_search.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // Grid_Items
            // 
            Grid_Items.AllowUserToAddRows = false;
            Grid_Items.AllowUserToDeleteRows = false;
            Grid_Items.AllowUserToOrderColumns = true;
            Grid_Items.AllowUserToResizeRows = false;
            Grid_Items.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Grid_Items.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Grid_Items.Location = new Point(12, 147);
            Grid_Items.MultiSelect = false;
            Grid_Items.Name = "Grid_Items";
            Grid_Items.ReadOnly = true;
            Grid_Items.RowHeadersWidth = 32;
            Grid_Items.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid_Items.Size = new Size(811, 288);
            Grid_Items.TabIndex = 4;
            Grid_Items.CellContentClick += Grid_Items_CellContentClick;
            // 
            // txt_log_notifications
            // 
            txt_log_notifications.Location = new Point(12, 604);
            txt_log_notifications.Name = "txt_log_notifications";
            txt_log_notifications.ReadOnly = true;
            txt_log_notifications.Size = new Size(961, 217);
            txt_log_notifications.TabIndex = 5;
            txt_log_notifications.Text = "";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 586);
            label2.Name = "label2";
            label2.Size = new Size(92, 15);
            label2.TabIndex = 6;
            label2.Text = "Notificaciones :";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rad_rollid);
            groupBox1.Controls.Add(rad_product_name);
            groupBox1.Controls.Add(rad_productid);
            groupBox1.Location = new Point(12, 441);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(129, 100);
            groupBox1.TabIndex = 7;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtrar por: ";
            // 
            // rad_rollid
            // 
            rad_rollid.AutoSize = true;
            rad_rollid.Location = new Point(6, 54);
            rad_rollid.Name = "rad_rollid";
            rad_rollid.Size = new Size(60, 19);
            rad_rollid.TabIndex = 10;
            rad_rollid.TabStop = true;
            rad_rollid.Text = "Roll-Id";
            // 
            // rad_product_name
            // 
            rad_product_name.AutoSize = true;
            rad_product_name.Location = new Point(6, 38);
            rad_product_name.Name = "rad_product_name";
            rad_product_name.Size = new Size(121, 19);
            rad_product_name.TabIndex = 9;
            rad_product_name.TabStop = true;
            rad_product_name.Text = "Nombre Producto";
            // 
            // rad_productid
            // 
            rad_productid.AutoSize = true;
            rad_productid.Location = new Point(6, 22);
            rad_productid.Name = "rad_productid";
            rad_productid.Size = new Size(83, 19);
            rad_productid.TabIndex = 8;
            rad_productid.TabStop = true;
            rad_productid.Text = "Product Id.";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(160, 454);
            label3.Name = "label3";
            label3.Size = new Size(99, 15);
            label3.TabIndex = 8;
            label3.Text = "Numero de Filas :";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(178, 483);
            label4.Name = "label4";
            label4.Size = new Size(81, 15);
            label4.TabIndex = 9;
            label4.Text = "Advertencias :";
            // 
            // btn_load_data
            // 
            btn_load_data.Image = (Image)resources.GetObject("btn_load_data.Image");
            btn_load_data.Location = new Point(829, 147);
            btn_load_data.Name = "btn_load_data";
            btn_load_data.Size = new Size(145, 73);
            btn_load_data.TabIndex = 10;
            btn_load_data.Text = "Cargar Datos";
            btn_load_data.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_load_data.Click += Btn_load_data_Click;
            // 
            // chk_valid_products
            // 
            chk_valid_products.AutoSize = true;
            chk_valid_products.Location = new Point(6, 36);
            chk_valid_products.Name = "chk_valid_products";
            chk_valid_products.Size = new Size(171, 19);
            chk_valid_products.TabIndex = 11;
            chk_valid_products.Text = "Validacion de los Productos";
            // 
            // txt_number_rows
            // 
            txt_number_rows.Location = new Point(265, 451);
            txt_number_rows.Name = "txt_number_rows";
            txt_number_rows.ReadOnly = true;
            txt_number_rows.Size = new Size(255, 23);
            txt_number_rows.TabIndex = 12;
            // 
            // txt_warning
            // 
            txt_warning.Location = new Point(265, 480);
            txt_warning.Name = "txt_warning";
            txt_warning.ReadOnly = true;
            txt_warning.Size = new Size(255, 23);
            txt_warning.TabIndex = 13;
            // 
            // txt_fileName
            // 
            txt_fileName.Location = new Point(265, 509);
            txt_fileName.Name = "txt_fileName";
            txt_fileName.ReadOnly = true;
            txt_fileName.Size = new Size(255, 23);
            txt_fileName.TabIndex = 15;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(158, 512);
            label6.Name = "label6";
            label6.Size = new Size(101, 15);
            label6.TabIndex = 14;
            label6.Text = "Nombre Archivo :";
            // 
            // txt_filePath
            // 
            txt_filePath.Location = new Point(265, 538);
            txt_filePath.Name = "txt_filePath";
            txt_filePath.ReadOnly = true;
            txt_filePath.Size = new Size(255, 23);
            txt_filePath.TabIndex = 17;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(222, 541);
            label7.Name = "label7";
            label7.Size = new Size(37, 15);
            label7.TabIndex = 16;
            label7.Text = "Ruta :";
            // 
            // btn_saveDatabase
            // 
            btn_saveDatabase.Image = (Image)resources.GetObject("btn_saveDatabase.Image");
            btn_saveDatabase.Location = new Point(828, 226);
            btn_saveDatabase.Name = "btn_saveDatabase";
            btn_saveDatabase.Size = new Size(145, 73);
            btn_saveDatabase.TabIndex = 18;
            btn_saveDatabase.Text = "Guardar BD";
            btn_saveDatabase.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_saveDatabase.Click += Btn_saveDatabase_Click;
            // 
            // checkBox1
            // 
            checkBox1.AutoSize = true;
            checkBox1.Location = new Point(6, 60);
            checkBox1.Name = "checkBox1";
            checkBox1.Size = new Size(161, 19);
            checkBox1.TabIndex = 19;
            checkBox1.Text = "Validar Repeticion Roll-Id ";
            // 
            // button1
            // 
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(828, 305);
            button1.Name = "button1";
            button1.Size = new Size(145, 73);
            button1.TabIndex = 20;
            button1.Text = "Reporte Data";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // NUMBERS_NOTIFICATIONS
            // 
            NUMBERS_NOTIFICATIONS.AutoSize = true;
            NUMBERS_NOTIFICATIONS.Location = new Point(206, 570);
            NUMBERS_NOTIFICATIONS.Name = "NUMBERS_NOTIFICATIONS";
            NUMBERS_NOTIFICATIONS.Size = new Size(53, 15);
            NUMBERS_NOTIFICATIONS.TabIndex = 24;
            NUMBERS_NOTIFICATIONS.Text = "Errores :";
            // 
            // txt_errors
            // 
            txt_errors.Location = new Point(265, 567);
            txt_errors.Name = "txt_errors";
            txt_errors.ReadOnly = true;
            txt_errors.Size = new Size(255, 23);
            txt_errors.TabIndex = 25;
            txt_errors.Text = "0";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(chk_valid_products);
            groupBox2.Controls.Add(checkBox1);
            groupBox2.Location = new Point(535, 448);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(200, 150);
            groupBox2.TabIndex = 26;
            groupBox2.TabStop = false;
            groupBox2.Text = "Validaciones";
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(btn_accion);
            groupBox3.Controls.Add(chk_saveproductsnotfound);
            groupBox3.Location = new Point(741, 448);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(200, 150);
            groupBox3.TabIndex = 27;
            groupBox3.TabStop = false;
            groupBox3.Text = "Acciones";
            // 
            // btn_accion
            // 
            btn_accion.Enabled = false;
            btn_accion.FlatStyle = FlatStyle.Flat;
            btn_accion.Image = Properties.Resources.DATA_RESERVA48;
            btn_accion.Location = new Point(23, 69);
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
            chk_saveproductsnotfound.Location = new Point(6, 36);
            chk_saveproductsnotfound.Name = "chk_saveproductsnotfound";
            chk_saveproductsnotfound.Size = new Size(179, 19);
            chk_saveproductsnotfound.TabIndex = 11;
            chk_saveproductsnotfound.Text = "Crear Productos si no Existen";
            // 
            // Frm_Imports
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(985, 833);
            Controls.Add(groupBox3);
            Controls.Add(groupBox2);
            Controls.Add(txt_errors);
            Controls.Add(NUMBERS_NOTIFICATIONS);
            Controls.Add(button1);
            Controls.Add(btn_saveDatabase);
            Controls.Add(txt_filePath);
            Controls.Add(label7);
            Controls.Add(txt_fileName);
            Controls.Add(label6);
            Controls.Add(txt_warning);
            Controls.Add(txt_number_rows);
            Controls.Add(btn_load_data);
            Controls.Add(label4);
            Controls.Add(label3);
            Controls.Add(groupBox1);
            Controls.Add(label2);
            Controls.Add(txt_log_notifications);
            Controls.Add(Grid_Items);
            Controls.Add(btn_search);
            Controls.Add(txt_buscar);
            Controls.Add(label1);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Frm_Imports";
            StartPosition = FormStartPosition.CenterScreen;
            Text = " ";
            Load += Frm_Imports_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            groupBox3.ResumeLayout(false);
            groupBox3.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label1;
        private TextBox txt_buscar;
        private Button btn_search;
        private DataGridView Grid_Items;
        private RichTextBox txt_log_notifications;
        private Label label2;
        private Label label5;
        private GroupBox groupBox1;
        private RadioButton rad_rollid;
        private RadioButton rad_product_name;
        private RadioButton rad_productid;
        private Label label3;
        private Label label4;
        private PictureBox pictureBox1;
        private Button btn_load_data;
        private CheckBox chk_valid_products;
        private TextBox txt_number_rows;
        private TextBox txt_warning;
        private TextBox txt_fileName;
        private Label label6;
        private TextBox txt_filePath;
        private Label label7;
        private Button btn_saveDatabase;
        private CheckBox checkBox1;
        private Button button1;
        private Label NUMBERS_NOTIFICATIONS;
        private TextBox txt_errors;
        private GroupBox groupBox2;
        private GroupBox groupBox3;
        private CheckBox chk_saveproductsnotfound;
        private Button btn_accion;
    }
}


