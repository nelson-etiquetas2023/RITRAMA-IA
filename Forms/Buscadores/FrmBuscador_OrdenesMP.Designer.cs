namespace Ritrama2025.Forms.Buscadores
{
    partial class FrmBuscador_OrdenesMP
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmBuscador_OrdenesMP));
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label1 = new Label();
            txt_buscar = new TextBox();
            btn_buscar = new Button();
            Grid_Items = new DataGridView();
            label2 = new Label();
            lbl_registros_encontrados = new Label();
            sqlCommand1 = new Microsoft.Data.SqlClient.SqlCommand();
            txt_fecha_desde = new DateTimePicker();
            label4 = new Label();
            label5 = new Label();
            txt_fecha_hasta = new DateTimePicker();
            rad_numeroOrden = new RadioButton();
            rad_proveedor = new RadioButton();
            rad_transporte = new RadioButton();
            rad_fecha_emision = new RadioButton();
            rad_fecha_produccion = new RadioButton();
            label6 = new Label();
            rad_recepcionista = new RadioButton();
            rad_embarque = new RadioButton();
            rad_Orden_Compra = new RadioButton();
            rad_guia = new RadioButton();
            label7 = new Label();
            pictureBox2 = new PictureBox();
            pictureBox3 = new PictureBox();
            pictureBox4 = new PictureBox();
            label3 = new Label();
            label8 = new Label();
            btn_reload = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).BeginInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).BeginInit();
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
            panel1.Size = new Size(914, 120);
            panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(208, 27);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(65, 66);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(279, 36);
            label1.Name = "label1";
            label1.Size = new Size(401, 47);
            label1.TabIndex = 0;
            label1.Text = "Buscador de Ordenes ";
            // 
            // txt_buscar
            // 
            txt_buscar.Location = new Point(14, 151);
            txt_buscar.Margin = new Padding(3, 4, 3, 4);
            txt_buscar.Name = "txt_buscar";
            txt_buscar.Size = new Size(775, 25);
            txt_buscar.TabIndex = 1;
            // 
            // btn_buscar
            // 
            btn_buscar.Image = (Image)resources.GetObject("btn_buscar.Image");
            btn_buscar.Location = new Point(797, 137);
            btn_buscar.Margin = new Padding(3, 4, 3, 4);
            btn_buscar.Name = "btn_buscar";
            btn_buscar.Size = new Size(104, 42);
            btn_buscar.TabIndex = 2;
            btn_buscar.Text = "Buscar";
            btn_buscar.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_buscar.Click += btn_buscar_Click;
            // 
            // Grid_Items
            // 
            Grid_Items.AllowUserToAddRows = false;
            Grid_Items.AllowUserToDeleteRows = false;
            Grid_Items.AllowUserToResizeRows = false;
            Grid_Items.AutoSizeRowsMode = DataGridViewAutoSizeRowsMode.AllCells;
            Grid_Items.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Grid_Items.Location = new Point(16, 187);
            Grid_Items.Margin = new Padding(3, 4, 3, 4);
            Grid_Items.Name = "Grid_Items";
            Grid_Items.ReadOnly = true;
            Grid_Items.ScrollBars = ScrollBars.Vertical;
            Grid_Items.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid_Items.Size = new Size(887, 431);
            Grid_Items.TabIndex = 3;
            Grid_Items.CellMouseDoubleClick += Grid_Items_CellMouseDoubleClick;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(719, 637);
            label2.Name = "label2";
            label2.Size = new Size(91, 20);
            label2.TabIndex = 4;
            label2.Text = "Filtrar Por:";
            // 
            // lbl_registros_encontrados
            // 
            lbl_registros_encontrados.AutoSize = true;
            lbl_registros_encontrados.Location = new Point(54, 637);
            lbl_registros_encontrados.Name = "lbl_registros_encontrados";
            lbl_registros_encontrados.Size = new Size(193, 20);
            lbl_registros_encontrados.TabIndex = 5;
            lbl_registros_encontrados.Text = "0 Registros Encontrados:";
            // 
            // sqlCommand1
            // 
            sqlCommand1.CommandTimeout = 30;
            sqlCommand1.EnableOptimizedParameterBinding = false;
            // 
            // txt_fecha_desde
            // 
            txt_fecha_desde.Location = new Point(77, 698);
            txt_fecha_desde.Margin = new Padding(3, 4, 3, 4);
            txt_fecha_desde.Name = "txt_fecha_desde";
            txt_fecha_desde.Size = new Size(262, 25);
            txt_fecha_desde.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(14, 698);
            label4.Name = "label4";
            label4.Size = new Size(49, 18);
            label4.TabIndex = 7;
            label4.Text = "Desde:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(16, 733);
            label5.Name = "label5";
            label5.Size = new Size(47, 18);
            label5.TabIndex = 8;
            label5.Text = "Hasta:";
            // 
            // txt_fecha_hasta
            // 
            txt_fecha_hasta.Location = new Point(77, 733);
            txt_fecha_hasta.Margin = new Padding(3, 4, 3, 4);
            txt_fecha_hasta.Name = "txt_fecha_hasta";
            txt_fecha_hasta.Size = new Size(262, 25);
            txt_fecha_hasta.TabIndex = 9;
            // 
            // rad_numeroOrden
            // 
            rad_numeroOrden.AutoSize = true;
            rad_numeroOrden.Checked = true;
            rad_numeroOrden.Location = new Point(695, 656);
            rad_numeroOrden.Margin = new Padding(3, 4, 3, 4);
            rad_numeroOrden.Name = "rad_numeroOrden";
            rad_numeroOrden.Size = new Size(135, 22);
            rad_numeroOrden.TabIndex = 10;
            rad_numeroOrden.TabStop = true;
            rad_numeroOrden.Text = "Numero de Orden";
            // 
            // rad_proveedor
            // 
            rad_proveedor.AutoSize = true;
            rad_proveedor.Location = new Point(695, 677);
            rad_proveedor.Margin = new Padding(3, 4, 3, 4);
            rad_proveedor.Name = "rad_proveedor";
            rad_proveedor.Size = new Size(89, 22);
            rad_proveedor.TabIndex = 11;
            rad_proveedor.Text = "Proveedor";
            // 
            // rad_transporte
            // 
            rad_transporte.AutoSize = true;
            rad_transporte.Location = new Point(695, 699);
            rad_transporte.Margin = new Padding(3, 4, 3, 4);
            rad_transporte.Name = "rad_transporte";
            rad_transporte.Size = new Size(92, 22);
            rad_transporte.TabIndex = 12;
            rad_transporte.Text = "Transporte";
            // 
            // rad_fecha_emision
            // 
            rad_fecha_emision.AutoSize = true;
            rad_fecha_emision.Location = new Point(361, 721);
            rad_fecha_emision.Margin = new Padding(3, 4, 3, 4);
            rad_fecha_emision.Name = "rad_fecha_emision";
            rad_fecha_emision.Size = new Size(129, 22);
            rad_fecha_emision.TabIndex = 13;
            rad_fecha_emision.Text = "Fecha de Emisión";
            // 
            // rad_fecha_produccion
            // 
            rad_fecha_produccion.AutoSize = true;
            rad_fecha_produccion.Location = new Point(361, 742);
            rad_fecha_produccion.Margin = new Padding(3, 4, 3, 4);
            rad_fecha_produccion.Name = "rad_fecha_produccion";
            rad_fecha_produccion.Size = new Size(150, 22);
            rad_fecha_produccion.TabIndex = 14;
            rad_fecha_produccion.Text = "Fecha de Producción";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(14, 126);
            label6.Name = "label6";
            label6.Size = new Size(346, 18);
            label6.TabIndex = 15;
            label6.Text = "Incluya aqui las palabras claves por donde quiere buscar:";
            // 
            // rad_recepcionista
            // 
            rad_recepcionista.AutoSize = true;
            rad_recepcionista.Location = new Point(695, 720);
            rad_recepcionista.Margin = new Padding(3, 4, 3, 4);
            rad_recepcionista.Name = "rad_recepcionista";
            rad_recepcionista.Size = new Size(108, 22);
            rad_recepcionista.TabIndex = 16;
            rad_recepcionista.Text = "Recepcionista";
            // 
            // rad_embarque
            // 
            rad_embarque.AutoSize = true;
            rad_embarque.Location = new Point(695, 741);
            rad_embarque.Margin = new Padding(3, 4, 3, 4);
            rad_embarque.Name = "rad_embarque";
            rad_embarque.Size = new Size(88, 22);
            rad_embarque.TabIndex = 17;
            rad_embarque.Text = "Embarque";
            // 
            // rad_Orden_Compra
            // 
            rad_Orden_Compra.AutoSize = true;
            rad_Orden_Compra.Location = new Point(695, 762);
            rad_Orden_Compra.Margin = new Padding(3, 4, 3, 4);
            rad_Orden_Compra.Name = "rad_Orden_Compra";
            rad_Orden_Compra.Size = new Size(115, 22);
            rad_Orden_Compra.TabIndex = 18;
            rad_Orden_Compra.Text = "Orden Compra";
            // 
            // rad_guia
            // 
            rad_guia.AutoSize = true;
            rad_guia.Location = new Point(695, 783);
            rad_guia.Margin = new Padding(3, 4, 3, 4);
            rad_guia.Name = "rad_guia";
            rad_guia.Size = new Size(53, 22);
            rad_guia.TabIndex = 19;
            rad_guia.Text = "Guia";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(399, 694);
            label7.Name = "label7";
            label7.Size = new Size(164, 20);
            label7.TabIndex = 20;
            label7.Text = "Busqueda por Fecha:";
            // 
            // pictureBox2
            // 
            pictureBox2.Image = (Image)resources.GetObject("pictureBox2.Image");
            pictureBox2.Location = new Point(361, 682);
            pictureBox2.Name = "pictureBox2";
            pictureBox2.Size = new Size(32, 32);
            pictureBox2.TabIndex = 21;
            pictureBox2.TabStop = false;
            // 
            // pictureBox3
            // 
            pictureBox3.Image = (Image)resources.GetObject("pictureBox3.Image");
            pictureBox3.Location = new Point(686, 624);
            pictureBox3.Name = "pictureBox3";
            pictureBox3.Size = new Size(32, 32);
            pictureBox3.TabIndex = 22;
            pictureBox3.TabStop = false;
            // 
            // pictureBox4
            // 
            pictureBox4.Image = (Image)resources.GetObject("pictureBox4.Image");
            pictureBox4.Location = new Point(16, 625);
            pictureBox4.Name = "pictureBox4";
            pictureBox4.Size = new Size(32, 32);
            pictureBox4.TabIndex = 23;
            pictureBox4.TabStop = false;
            // 
            // label3
            // 
            label3.Location = new Point(14, 801);
            label3.Name = "label3";
            label3.Size = new Size(575, 65);
            label3.TabIndex = 24;
            label3.Text = resources.GetString("label3.Text");
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(16, 781);
            label8.Name = "label8";
            label8.Size = new Size(61, 20);
            label8.TabIndex = 25;
            label8.Text = "Ayuda:";
            // 
            // btn_reload
            // 
            btn_reload.Image = (Image)resources.GetObject("btn_reload.Image");
            btn_reload.Location = new Point(695, 813);
            btn_reload.Margin = new Padding(3, 4, 3, 4);
            btn_reload.Name = "btn_reload";
            btn_reload.Size = new Size(206, 42);
            btn_reload.TabIndex = 26;
            btn_reload.Text = "Recargar Data";
            btn_reload.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_reload.Click += btn_reload_Click;
            // 
            // FrmBuscador_OrdenesMP
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(914, 875);
            Controls.Add(btn_reload);
            Controls.Add(label8);
            Controls.Add(label3);
            Controls.Add(pictureBox4);
            Controls.Add(pictureBox3);
            Controls.Add(pictureBox2);
            Controls.Add(label7);
            Controls.Add(rad_guia);
            Controls.Add(rad_Orden_Compra);
            Controls.Add(rad_embarque);
            Controls.Add(rad_recepcionista);
            Controls.Add(label6);
            Controls.Add(rad_fecha_produccion);
            Controls.Add(rad_fecha_emision);
            Controls.Add(rad_transporte);
            Controls.Add(rad_proveedor);
            Controls.Add(rad_numeroOrden);
            Controls.Add(txt_fecha_hasta);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txt_fecha_desde);
            Controls.Add(lbl_registros_encontrados);
            Controls.Add(label2);
            Controls.Add(Grid_Items);
            Controls.Add(btn_buscar);
            Controls.Add(txt_buscar);
            Controls.Add(panel1);
            Font = new Font("Noto Sans", 9.75F, FontStyle.Regular, GraphicsUnit.Point, 0);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmBuscador_OrdenesMP";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Buscador de Ordenes:";
            Load += FrmBuscador_OrdenesMP_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox2).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox3).EndInit();
            ((System.ComponentModel.ISupportInitialize)pictureBox4).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label1;
        private TextBox txt_buscar;
        private Button btn_buscar;
        private DataGridView Grid_Items;
        private Label label2;
        private Label lbl_registros_encontrados;
        private Microsoft.Data.SqlClient.SqlCommand sqlCommand1;
        private DateTimePicker txt_fecha_desde;
        private Label label4;
        private Label label5;
        private DateTimePicker txt_fecha_hasta;
        private RadioButton rad_numeroOrden;
        private RadioButton rad_proveedor;
        private RadioButton rad_transporte;
        private RadioButton rad_fecha_emision;
        private RadioButton rad_fecha_produccion;
        private Label label6;
        private RadioButton rad_recepcionista;
        private RadioButton rad_embarque;
        private RadioButton rad_Orden_Compra;
        private RadioButton rad_guia;
        private Label label7;
        private PictureBox pictureBox2;
        private PictureBox pictureBox3;
        private PictureBox pictureBox4;
        private Label label3;
        private Label label8;
        private Button btn_reload;
    }
}


