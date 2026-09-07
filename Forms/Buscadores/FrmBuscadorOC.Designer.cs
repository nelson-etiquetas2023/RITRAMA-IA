namespace Ritrama2025.Forms.Buscadores
{
    partial class FrmBuscadorOC
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBuscadorOC));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            label6 = new Label();
            btn_buscar = new Button();
            txt_buscar = new TextBox();
            Grid_Items = new DataGridView();
            pictureBox4 = new PictureBox();
            lbl_registros_encontrados = new Label();
            pictureBox2 = new PictureBox();
            label7 = new Label();
            rad_fecha_produccion = new RadioButton();
            rad_fecha_emision = new RadioButton();
            txt_fecha_hasta = new DateTimePicker();
            label5 = new Label();
            label4 = new Label();
            txt_fecha_desde = new DateTimePicker();
            btn_reload = new Button();
            pictureBox3 = new PictureBox();
            rad_RollId = new RadioButton();
            rad_Sell_Order = new RadioButton();
            rad_ProductName = new RadioButton();
            rad_Product_id = new RadioButton();
            rad_Opetator = new RadioButton();
            rad_Customer = new RadioButton();
            rad_numeroOrden = new RadioButton();
            label2 = new Label();
            label8 = new Label();
            label3 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label1);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(905, 120);
            panel1.TabIndex = 1;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(160, 26);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(65, 66);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(231, 34);
            label1.Name = "label1";
            label1.Size = new Size(426, 47);
            label1.TabIndex = 0;
            label1.Text = "Ordenes de Producción";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 130);
            label6.Name = "label6";
            label6.Size = new Size(346, 18);
            label6.TabIndex = 18;
            label6.Text = "Incluya aqui las palabras claves por donde quiere buscar:";
            // 
            // btn_buscar
            // 
            btn_buscar.Image = (Image)resources.GetObject("btn_buscar.Image");
            btn_buscar.Location = new Point(795, 141);
            btn_buscar.Margin = new Padding(3, 4, 3, 4);
            btn_buscar.Name = "btn_buscar";
            btn_buscar.Size = new Size(104, 42);
            btn_buscar.TabIndex = 17;
            btn_buscar.Text = "Buscar";
            btn_buscar.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_buscar.Click += btn_buscar_Click;
            // 
            // txt_buscar
            // 
            txt_buscar.Location = new Point(12, 155);
            txt_buscar.Margin = new Padding(3, 4, 3, 4);
            txt_buscar.Name = "txt_buscar";
            txt_buscar.Size = new Size(775, 23);
            txt_buscar.TabIndex = 16;
            // 
            // Grid_Items
            // 
            Grid_Items.AllowUserToAddRows = false;
            Grid_Items.AllowUserToDeleteRows = false;
            Grid_Items.AllowUserToResizeRows = false;
            Grid_Items.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Grid_Items.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Grid_Items.Location = new Point(9, 191);
            Grid_Items.Margin = new Padding(3, 4, 3, 4);
            Grid_Items.Name = "Grid_Items";
            Grid_Items.ReadOnly = true;
            Grid_Items.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid_Items.Size = new Size(887, 250);
            Grid_Items.TabIndex = 19;
            Grid_Items.CellMouseDoubleClick += Grid_Items_CellMouseDoubleClick;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(11, 448);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(32, 32);
            pictureBox4.TabIndex = 25;
            pictureBox4.TabStop = false;
            // 
            // lbl_registros_encontrados
            // 
            lbl_registros_encontrados.AutoSize = true;
            lbl_registros_encontrados.Location = new Point(49, 460);
            lbl_registros_encontrados.Name = "lbl_registros_encontrados";
            lbl_registros_encontrados.Size = new Size(193, 20);
            lbl_registros_encontrados.TabIndex = 24;
            lbl_registros_encontrados.Text = "0 Registros Encontrados:";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(359, 529);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(32, 32);
            pictureBox2.TabIndex = 33;
            pictureBox2.TabStop = false;
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(397, 541);
            label7.Name = "label7";
            label7.Size = new Size(164, 20);
            label7.TabIndex = 32;
            label7.Text = "Busqueda por Fecha:";
            // 
            // rad_fecha_produccion
            // 
            rad_fecha_produccion.AutoSize = true;
            rad_fecha_produccion.Location = new Point(359, 589);
            rad_fecha_produccion.Margin = new Padding(3, 4, 3, 4);
            rad_fecha_produccion.Name = "rad_fecha_produccion";
            rad_fecha_produccion.Size = new Size(136, 19);
            rad_fecha_produccion.TabIndex = 31;
            rad_fecha_produccion.Text = "Fecha de Producción";
            // 
            // rad_fecha_emision
            // 
            rad_fecha_emision.AutoSize = true;
            rad_fecha_emision.Location = new Point(359, 568);
            rad_fecha_emision.Margin = new Padding(3, 4, 3, 4);
            rad_fecha_emision.Name = "rad_fecha_emision";
            rad_fecha_emision.Size = new Size(117, 19);
            rad_fecha_emision.TabIndex = 30;
            rad_fecha_emision.Text = "Fecha de Emisión";
            // 
            // txt_fecha_hasta
            // 
            txt_fecha_hasta.Location = new Point(75, 580);
            txt_fecha_hasta.Margin = new Padding(3, 4, 3, 4);
            txt_fecha_hasta.Name = "txt_fecha_hasta";
            txt_fecha_hasta.Size = new Size(262, 23);
            txt_fecha_hasta.TabIndex = 29;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(14, 580);
            label5.Name = "label5";
            label5.Size = new Size(47, 18);
            label5.TabIndex = 28;
            label5.Text = "Hasta:";
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 545);
            label4.Name = "label4";
            label4.Size = new Size(49, 18);
            label4.TabIndex = 27;
            label4.Text = "Desde:";
            // 
            // txt_fecha_desde
            // 
            txt_fecha_desde.Location = new Point(75, 545);
            txt_fecha_desde.Margin = new Padding(3, 4, 3, 4);
            txt_fecha_desde.Name = "txt_fecha_desde";
            txt_fecha_desde.Size = new Size(262, 23);
            txt_fecha_desde.TabIndex = 26;
            // 
            // btn_reload
            // 
            btn_reload.Image = (Image)resources.GetObject("btn_reload.Image");
            btn_reload.Location = new Point(688, 649);
            btn_reload.Margin = new Padding(3, 4, 3, 4);
            btn_reload.Name = "btn_reload";
            btn_reload.Size = new Size(206, 42);
            btn_reload.TabIndex = 43;
            btn_reload.Text = "Recargar Data";
            btn_reload.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_reload.Click += btn_reload_Click;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(679, 460);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(32, 32);
            pictureBox3.TabIndex = 42;
            pictureBox3.TabStop = false;
            // 
            // rad_RollId
            // 
            rad_RollId.AutoSize = true;
            rad_RollId.Location = new Point(688, 619);
            rad_RollId.Margin = new Padding(3, 4, 3, 4);
            rad_RollId.Name = "rad_RollId";
            rad_RollId.Size = new Size(60, 19);
            rad_RollId.TabIndex = 41;
            rad_RollId.Text = "Roll-Id";
            // 
            // rad_Sell_Order
            // 
            rad_Sell_Order.AutoSize = true;
            rad_Sell_Order.Location = new Point(688, 598);
            rad_Sell_Order.Margin = new Padding(3, 4, 3, 4);
            rad_Sell_Order.Name = "rad_Sell_Order";
            rad_Sell_Order.Size = new Size(76, 19);
            rad_Sell_Order.TabIndex = 40;
            rad_Sell_Order.Text = "Sell Order";
            // 
            // rad_ProductName
            // 
            rad_ProductName.AutoSize = true;
            rad_ProductName.Location = new Point(688, 577);
            rad_ProductName.Margin = new Padding(3, 4, 3, 4);
            rad_ProductName.Name = "rad_ProductName";
            rad_ProductName.Size = new Size(104, 19);
            rad_ProductName.TabIndex = 39;
            rad_ProductName.Text = "Product_Name";
            // 
            // rad_Product_id
            // 
            rad_Product_id.AutoSize = true;
            rad_Product_id.Location = new Point(688, 556);
            rad_Product_id.Margin = new Padding(3, 4, 3, 4);
            rad_Product_id.Name = "rad_Product_id";
            rad_Product_id.Size = new Size(82, 19);
            rad_Product_id.TabIndex = 38;
            rad_Product_id.Text = "Product_Id";
            // 
            // rad_Opetator
            // 
            rad_Opetator.AutoSize = true;
            rad_Opetator.Location = new Point(688, 535);
            rad_Opetator.Margin = new Padding(3, 4, 3, 4);
            rad_Opetator.Name = "rad_Opetator";
            rad_Opetator.Size = new Size(75, 19);
            rad_Opetator.TabIndex = 37;
            rad_Opetator.Text = "Operador";
            // 
            // rad_Customer
            // 
            rad_Customer.AutoSize = true;
            rad_Customer.Location = new Point(688, 513);
            rad_Customer.Margin = new Padding(3, 4, 3, 4);
            rad_Customer.Name = "rad_Customer";
            rad_Customer.Size = new Size(77, 19);
            rad_Customer.TabIndex = 36;
            rad_Customer.Text = "Customer";
            // 
            // rad_numeroOrden
            // 
            rad_numeroOrden.AutoSize = true;
            rad_numeroOrden.Checked = true;
            rad_numeroOrden.Location = new Point(688, 492);
            rad_numeroOrden.Margin = new Padding(3, 4, 3, 4);
            rad_numeroOrden.Name = "rad_numeroOrden";
            rad_numeroOrden.Size = new Size(121, 19);
            rad_numeroOrden.TabIndex = 35;
            rad_numeroOrden.TabStop = true;
            rad_numeroOrden.Text = "Numero de Orden";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(712, 473);
            label2.Name = "label2";
            label2.Size = new Size(91, 20);
            label2.TabIndex = 34;
            label2.Text = "Filtrar Por:";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(11, 629);
            label8.Name = "label8";
            label8.Size = new Size(61, 20);
            label8.TabIndex = 45;
            label8.Text = "Ayuda:";
            // 
            // label3
            // 
            label3.Location = new Point(9, 649);
            label3.Name = "label3";
            label3.Size = new Size(575, 65);
            label3.TabIndex = 44;
            label3.Text = resources.GetString("label3.Text");
            // 
            // FrmBuscadorOC
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(905, 737);
            Controls.Add(label8);
            Controls.Add(label3);
            Controls.Add(btn_reload);
            Controls.Add(pictureBox3);
            Controls.Add(rad_RollId);
            Controls.Add(rad_Sell_Order);
            Controls.Add(rad_ProductName);
            Controls.Add(rad_Product_id);
            Controls.Add(rad_Opetator);
            Controls.Add(rad_Customer);
            Controls.Add(rad_numeroOrden);
            Controls.Add(label2);
            Controls.Add(pictureBox2);
            Controls.Add(label7);
            Controls.Add(rad_fecha_produccion);
            Controls.Add(rad_fecha_emision);
            Controls.Add(txt_fecha_hasta);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txt_fecha_desde);
            Controls.Add(pictureBox4);
            Controls.Add(lbl_registros_encontrados);
            Controls.Add(Grid_Items);
            Controls.Add(label6);
            Controls.Add(btn_buscar);
            Controls.Add(txt_buscar);
            Controls.Add(panel1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmBuscadorOC";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Buscador Ordenes de Corte";
            Load += FrmBuscadorOC_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label1;
        private Label label6;
        private Button btn_buscar;
        private TextBox txt_buscar;
        private DataGridView Grid_Items;
        private PictureBox pictureBox4;
        private Label lbl_registros_encontrados;
        private PictureBox pictureBox2;
        private Label label7;
        private RadioButton rad_fecha_produccion;
        private RadioButton rad_fecha_emision;
        private DateTimePicker txt_fecha_hasta;
        private Label label5;
        private Label label4;
        private DateTimePicker txt_fecha_desde;
        private Button btn_reload;
        private PictureBox pictureBox3;
        private RadioButton rad_RollId;
        private RadioButton rad_Sell_Order;
        private RadioButton rad_ProductName;
        private RadioButton rad_Product_id;
        private RadioButton rad_Opetator;
        private RadioButton rad_Customer;
        private RadioButton rad_numeroOrden;
        private Label label2;
        private Label label8;
        private Label label3;
    }
}


