namespace Ritrama2025.Forms.Buscadores
{
    partial class Frm_ProductSeach
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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ProductSeach));
            Grid_Products = new DataGridView();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label2 = new Label();
            txt_buscar = new TextBox();
            btn_search = new Button();
            groupBox1 = new GroupBox();
            rad_rolloCortado = new RadioButton();
            rad_hojas = new RadioButton();
            rad_graphics = new RadioButton();
            rad_master = new RadioButton();
            rad_productName = new RadioButton();
            rad_productid = new RadioButton();
            COUNTER_ROWS = new Label();
            groupBox2 = new GroupBox();
            label3 = new Label();
            ((System.ComponentModel.ISupportInitialize)Grid_Products).BeginInit();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            SuspendLayout();
            // 
            // Grid_Products
            // 
            Grid_Products.AllowUserToAddRows = false;
            Grid_Products.AllowUserToDeleteRows = false;
            Grid_Products.AllowUserToResizeRows = false;
            Grid_Products.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            dataGridViewCellStyle1.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle1.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = DataGridViewTriState.False;
            Grid_Products.DefaultCellStyle = dataGridViewCellStyle1;
            Grid_Products.Location = new Point(18, 147);
            Grid_Products.Name = "Grid_Products";
            Grid_Products.ReadOnly = true;
            Grid_Products.Size = new Size(665, 330);
            Grid_Products.TabIndex = 2;
            Grid_Products.CellMouseDoubleClick += Grid_Products_CellMouseDoubleClick;
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label2);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(695, 100);
            panel1.TabIndex = 0;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(186, 26);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(52, 50);
            pictureBox1.TabIndex = 7;
            pictureBox1.TabStop = false;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(239, 34);
            label2.Name = "label2";
            label2.Size = new Size(162, 32);
            label2.TabIndex = 6;
            label2.Text = "PRODUCTOS";
            // 
            // txt_buscar
            // 
            txt_buscar.Location = new Point(18, 112);
            txt_buscar.Name = "txt_buscar";
            txt_buscar.Size = new Size(558, 29);
            txt_buscar.TabIndex = 0;
            // 
            // btn_search
            // 
            btn_search.Location = new Point(582, 111);
            btn_search.Name = "btn_search";
            btn_search.Size = new Size(101, 29);
            btn_search.TabIndex = 1;
            btn_search.Text = "Buscar";
            btn_search.Click += btn_search_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rad_rolloCortado);
            groupBox1.Controls.Add(rad_hojas);
            groupBox1.Controls.Add(rad_graphics);
            groupBox1.Controls.Add(rad_master);
            groupBox1.Controls.Add(rad_productName);
            groupBox1.Controls.Add(rad_productid);
            groupBox1.Location = new Point(502, 483);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(181, 202);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtrar Por:";
            // 
            // rad_rolloCortado
            // 
            rad_rolloCortado.AutoSize = true;
            rad_rolloCortado.Location = new Point(6, 157);
            rad_rolloCortado.Name = "rad_rolloCortado";
            rad_rolloCortado.Size = new Size(165, 20);
            rad_rolloCortado.TabIndex = 5;
            rad_rolloCortado.Text = "Todos Rollo Cortados";
            rad_rolloCortado.CheckedChanged += rad_rolloCortado_CheckedChanged;
            // 
            // rad_hojas
            // 
            rad_hojas.AutoSize = true;
            rad_hojas.Location = new Point(6, 132);
            rad_hojas.Name = "rad_hojas";
            rad_hojas.Size = new Size(130, 20);
            rad_hojas.TabIndex = 4;
            rad_hojas.Text = "Todos las Hojas";
            rad_hojas.CheckedChanged += rad_hojas_CheckedChanged;
            // 
            // rad_graphics
            // 
            rad_graphics.AutoSize = true;
            rad_graphics.Location = new Point(6, 107);
            rad_graphics.Name = "rad_graphics";
            rad_graphics.Size = new Size(151, 20);
            rad_graphics.TabIndex = 3;
            rad_graphics.Text = "Todos los Graphics";
            rad_graphics.CheckedChanged += rad_graphics_CheckedChanged;
            // 
            // rad_master
            // 
            rad_master.AutoSize = true;
            rad_master.Location = new Point(6, 82);
            rad_master.Name = "rad_master";
            rad_master.Size = new Size(137, 20);
            rad_master.TabIndex = 2;
            rad_master.Text = "Todos los Master";
            rad_master.CheckedChanged += rad_master_CheckedChanged;
            // 
            // rad_productName
            // 
            rad_productName.AutoSize = true;
            rad_productName.Location = new Point(6, 57);
            rad_productName.Name = "rad_productName";
            rad_productName.Size = new Size(158, 20);
            rad_productName.TabIndex = 1;
            rad_productName.Text = "Nombre del Producto";
            // 
            // rad_productid
            // 
            rad_productid.AutoSize = true;
            rad_productid.Checked = true;
            rad_productid.Location = new Point(6, 32);
            rad_productid.Name = "rad_productid";
            rad_productid.Size = new Size(102, 20);
            rad_productid.TabIndex = 0;
            rad_productid.TabStop = true;
            rad_productid.Text = "Product Id.";
            // 
            // COUNTER_ROWS
            // 
            COUNTER_ROWS.AutoSize = true;
            COUNTER_ROWS.BorderStyle = BorderStyle.FixedSingle;
            COUNTER_ROWS.Location = new Point(12, 499);
            COUNTER_ROWS.Name = "COUNTER_ROWS";
            COUNTER_ROWS.Size = new Size(282, 23);
            COUNTER_ROWS.TabIndex = 3;
            COUNTER_ROWS.Text = "NUMERO DE REGISTROS: 0 DE 0";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(label3);
            groupBox2.Location = new Point(12, 559);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(431, 126);
            groupBox2.TabIndex = 6;
            groupBox2.TabStop = false;
            groupBox2.Text = "Instrucciones";
            // 
            // label3
            // 
            label3.Location = new Point(6, 24);
            label3.Name = "label3";
            label3.Size = new Size(425, 65);
            label3.TabIndex = 25;
            label3.Text = resources.GetString("label3.Text");
            // 
            // Frm_ProductSeach
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(695, 697);
            Controls.Add(groupBox2);
            Controls.Add(COUNTER_ROWS);
            Controls.Add(groupBox1);
            Controls.Add(btn_search);
            Controls.Add(txt_buscar);
            Controls.Add(panel1);
            Controls.Add(Grid_Products);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "Frm_ProductSeach";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Buscador de Productos :";
            Load += Frm_ProductSeach_Load;
            ((System.ComponentModel.ISupportInitialize)Grid_Products).EndInit();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView Grid_Products;
        private Panel panel1;
        private TextBox txt_buscar;
        private Button btn_search;
        private GroupBox groupBox1;
        private RadioButton rad_rolloCortado;
        private RadioButton rad_hojas;
        private RadioButton rad_graphics;
        private RadioButton rad_master;
        private RadioButton rad_productName;
        private RadioButton rad_productid;
        private Label COUNTER_ROWS;
        private PictureBox pictureBox1;
        private Label label2;
        private GroupBox groupBox2;
        private Label label3;
    }
}


