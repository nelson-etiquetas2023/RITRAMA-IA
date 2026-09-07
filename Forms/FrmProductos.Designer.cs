namespace Ritrama2025.Forms
{
    partial class FrmProductos
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProductos));
            label1 = new Label();
            txt_partid = new TextBox();
            txt_productname = new TextBox();
            label2 = new Label();
            txt_productdescription = new TextBox();
            label3 = new Label();
            txt_referencia = new TextBox();
            label4 = new Label();
            txt_codebar = new TextBox();
            label5 = new Label();
            txt_precio = new TextBox();
            label6 = new Label();
            txt_ratio = new TextBox();
            label8 = new Label();
            groupBox1 = new GroupBox();
            lbl_contador = new Label();
            groupBox2 = new GroupBox();
            rad_rollocortado = new RadioButton();
            rad_graphics = new RadioButton();
            rad_hoja = new RadioButton();
            rad_master = new RadioButton();
            chk_product_anulado = new CheckBox();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label10 = new Label();
            toolStrip1 = new ToolStrip();
            bot_primero = new ToolStripButton();
            bot_anterior = new ToolStripButton();
            bot_siguiente = new ToolStripButton();
            bot_ultimo = new ToolStripButton();
            bot_nuevo = new ToolStripButton();
            bot_guardar = new ToolStripButton();
            bot_cancelar = new ToolStripButton();
            btn_update = new ToolStripButton();
            bot_buscar = new ToolStripButton();
            bot_print = new ToolStripButton();
            bot_excel = new ToolStripButton();
            groupBox3 = new GroupBox();
            label7 = new Label();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            toolStrip1.SuspendLayout();
            groupBox3.SuspendLayout();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(14, 182);
            label1.Name = "label1";
            label1.Size = new Size(80, 17);
            label1.TabIndex = 0;
            label1.Text = "Part Id.:";
            // 
            // txt_partid
            // 
            txt_partid.Location = new Point(14, 205);
            txt_partid.Margin = new Padding(3, 4, 3, 4);
            txt_partid.Name = "txt_partid";
            txt_partid.ReadOnly = true;
            txt_partid.Size = new Size(293, 25);
            txt_partid.TabIndex = 1;
            // 
            // txt_productname
            // 
            txt_productname.Location = new Point(14, 268);
            txt_productname.Margin = new Padding(3, 4, 3, 4);
            txt_productname.Name = "txt_productname";
            txt_productname.ReadOnly = true;
            txt_productname.Size = new Size(639, 25);
            txt_productname.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(14, 249);
            label2.Name = "label2";
            label2.Size = new Size(120, 17);
            label2.TabIndex = 2;
            label2.Text = "Product Name :";
            // 
            // txt_productdescription
            // 
            txt_productdescription.Location = new Point(14, 328);
            txt_productdescription.Margin = new Padding(3, 4, 3, 4);
            txt_productdescription.Name = "txt_productdescription";
            txt_productdescription.ReadOnly = true;
            txt_productdescription.Size = new Size(639, 25);
            txt_productdescription.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(14, 307);
            label3.Name = "label3";
            label3.Size = new Size(216, 17);
            label3.TabIndex = 4;
            label3.Text = "Descripcion del Producto :";
            // 
            // txt_referencia
            // 
            txt_referencia.Location = new Point(14, 389);
            txt_referencia.Margin = new Padding(3, 4, 3, 4);
            txt_referencia.Name = "txt_referencia";
            txt_referencia.ReadOnly = true;
            txt_referencia.Size = new Size(293, 25);
            txt_referencia.TabIndex = 7;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(14, 369);
            label4.Name = "label4";
            label4.Size = new Size(104, 17);
            label4.TabIndex = 6;
            label4.Text = "Referencia :";
            // 
            // txt_codebar
            // 
            txt_codebar.Location = new Point(14, 454);
            txt_codebar.Margin = new Padding(3, 4, 3, 4);
            txt_codebar.Name = "txt_codebar";
            txt_codebar.ReadOnly = true;
            txt_codebar.Size = new Size(293, 25);
            txt_codebar.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(14, 432);
            label5.Name = "label5";
            label5.Size = new Size(144, 17);
            label5.TabIndex = 8;
            label5.Text = "Codigo de Barra :";
            // 
            // txt_precio
            // 
            txt_precio.Location = new Point(14, 525);
            txt_precio.Margin = new Padding(3, 4, 3, 4);
            txt_precio.Name = "txt_precio";
            txt_precio.ReadOnly = true;
            txt_precio.Size = new Size(293, 25);
            txt_precio.TabIndex = 11;
            txt_precio.TextChanged += txt_precio_TextChanged;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(14, 498);
            label6.Name = "label6";
            label6.Size = new Size(72, 17);
            label6.TabIndex = 10;
            label6.Text = "Precio :";
            // 
            // txt_ratio
            // 
            txt_ratio.Location = new Point(14, 589);
            txt_ratio.Margin = new Padding(3, 4, 3, 4);
            txt_ratio.Name = "txt_ratio";
            txt_ratio.ReadOnly = true;
            txt_ratio.Size = new Size(293, 25);
            txt_ratio.TabIndex = 15;
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(14, 562);
            label8.Name = "label8";
            label8.Size = new Size(64, 17);
            label8.TabIndex = 14;
            label8.Text = "Ratio :";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(lbl_contador);
            groupBox1.Location = new Point(695, 465);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(206, 114);
            groupBox1.TabIndex = 16;
            groupBox1.TabStop = false;
            groupBox1.Text = "Contador de Registro";
            // 
            // lbl_contador
            // 
            lbl_contador.AutoSize = true;
            lbl_contador.Location = new Point(29, 55);
            lbl_contador.Name = "lbl_contador";
            lbl_contador.Size = new Size(152, 17);
            lbl_contador.TabIndex = 17;
            lbl_contador.Text = "1 de 381 Registros";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(rad_rollocortado);
            groupBox2.Controls.Add(rad_graphics);
            groupBox2.Controls.Add(rad_hoja);
            groupBox2.Controls.Add(rad_master);
            groupBox2.Location = new Point(695, 277);
            groupBox2.Margin = new Padding(3, 4, 3, 4);
            groupBox2.Name = "groupBox2";
            groupBox2.Padding = new Padding(3, 4, 3, 4);
            groupBox2.Size = new Size(206, 172);
            groupBox2.TabIndex = 17;
            groupBox2.TabStop = false;
            groupBox2.Text = "Tipo de Producto";
            // 
            // rad_rollocortado
            // 
            rad_rollocortado.AutoSize = true;
            rad_rollocortado.Enabled = false;
            rad_rollocortado.Location = new Point(29, 117);
            rad_rollocortado.Margin = new Padding(3, 4, 3, 4);
            rad_rollocortado.Name = "rad_rollocortado";
            rad_rollocortado.Size = new Size(130, 21);
            rad_rollocortado.TabIndex = 3;
            rad_rollocortado.TabStop = true;
            rad_rollocortado.Text = "Rollo Cortado";
            // 
            // rad_graphics
            // 
            rad_graphics.AutoSize = true;
            rad_graphics.Enabled = false;
            rad_graphics.Location = new Point(29, 89);
            rad_graphics.Margin = new Padding(3, 4, 3, 4);
            rad_graphics.Name = "rad_graphics";
            rad_graphics.Size = new Size(90, 21);
            rad_graphics.TabIndex = 2;
            rad_graphics.TabStop = true;
            rad_graphics.Text = "Graphics";
            // 
            // rad_hoja
            // 
            rad_hoja.AutoSize = true;
            rad_hoja.Enabled = false;
            rad_hoja.Location = new Point(29, 61);
            rad_hoja.Margin = new Padding(3, 4, 3, 4);
            rad_hoja.Name = "rad_hoja";
            rad_hoja.Size = new Size(58, 21);
            rad_hoja.TabIndex = 1;
            rad_hoja.TabStop = true;
            rad_hoja.Text = "Hoja";
            // 
            // rad_master
            // 
            rad_master.AutoSize = true;
            rad_master.Enabled = false;
            rad_master.Location = new Point(29, 32);
            rad_master.Margin = new Padding(3, 4, 3, 4);
            rad_master.Name = "rad_master";
            rad_master.Size = new Size(122, 21);
            rad_master.TabIndex = 0;
            rad_master.TabStop = true;
            rad_master.Text = "Master Rolls";
            // 
            // chk_product_anulado
            // 
            chk_product_anulado.AutoSize = true;
            chk_product_anulado.Enabled = false;
            chk_product_anulado.Location = new Point(501, 398);
            chk_product_anulado.Margin = new Padding(3, 4, 3, 4);
            chk_product_anulado.Name = "chk_product_anulado";
            chk_product_anulado.Size = new Size(155, 21);
            chk_product_anulado.TabIndex = 18;
            chk_product_anulado.Text = "Producto Anulado";
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label10);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 4, 3, 4);
            panel1.Name = "panel1";
            panel1.Size = new Size(984, 114);
            panel1.TabIndex = 19;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = Properties.Resources.products64px;
            pictureBox1.Location = new Point(303, 26);
            pictureBox1.Margin = new Padding(3, 4, 3, 4);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(73, 67);
            pictureBox1.TabIndex = 1;
            pictureBox1.TabStop = false;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(382, 35);
            label10.Name = "label10";
            label10.Size = new Size(209, 47);
            label10.TabIndex = 0;
            label10.Text = "Productos";
            // 
            // toolStrip1
            // 
            toolStrip1.AutoSize = false;
            toolStrip1.Items.AddRange(new ToolStripItem[] { bot_primero, bot_anterior, bot_siguiente, bot_ultimo, bot_nuevo, bot_guardar, bot_cancelar, btn_update, bot_buscar, bot_print, bot_excel });
            toolStrip1.Location = new Point(0, 114);
            toolStrip1.Name = "toolStrip1";
            toolStrip1.Size = new Size(984, 39);
            toolStrip1.TabIndex = 20;
            toolStrip1.Text = "toolStrip1";
            // 
            // bot_primero
            // 
            bot_primero.AutoSize = false;
            bot_primero.Image = (Image)resources.GetObject("bot_primero.Image");
            bot_primero.ImageTransparentColor = Color.Magenta;
            bot_primero.Name = "bot_primero";
            bot_primero.Size = new Size(85, 30);
            bot_primero.Text = "Primero";
            bot_primero.Click += bot_primero_Click;
            // 
            // bot_anterior
            // 
            bot_anterior.AutoSize = false;
            bot_anterior.Image = (Image)resources.GetObject("bot_anterior.Image");
            bot_anterior.ImageTransparentColor = Color.Magenta;
            bot_anterior.Name = "bot_anterior";
            bot_anterior.Size = new Size(85, 30);
            bot_anterior.Text = "Anterior";
            bot_anterior.Click += bot_anterior_Click;
            // 
            // bot_siguiente
            // 
            bot_siguiente.AutoSize = false;
            bot_siguiente.Image = (Image)resources.GetObject("bot_siguiente.Image");
            bot_siguiente.ImageTransparentColor = Color.Magenta;
            bot_siguiente.Name = "bot_siguiente";
            bot_siguiente.Size = new Size(85, 30);
            bot_siguiente.Text = "Siguien";
            bot_siguiente.ToolTipText = "Sigui";
            bot_siguiente.Click += bot_siguiente_Click;
            // 
            // bot_ultimo
            // 
            bot_ultimo.AutoSize = false;
            bot_ultimo.Image = (Image)resources.GetObject("bot_ultimo.Image");
            bot_ultimo.ImageTransparentColor = Color.Magenta;
            bot_ultimo.Name = "bot_ultimo";
            bot_ultimo.Size = new Size(85, 30);
            bot_ultimo.Text = "Ultimo";
            bot_ultimo.ToolTipText = "Ultimo";
            bot_ultimo.Click += bot_ultimo_Click;
            // 
            // bot_nuevo
            // 
            bot_nuevo.AutoSize = false;
            bot_nuevo.Image = (Image)resources.GetObject("bot_nuevo.Image");
            bot_nuevo.ImageTransparentColor = Color.Magenta;
            bot_nuevo.Name = "bot_nuevo";
            bot_nuevo.Size = new Size(85, 30);
            bot_nuevo.Text = "Nuevo";
            bot_nuevo.ToolTipText = "Crear productos nuevos";
            bot_nuevo.Click += bot_nuevo_Click;
            // 
            // bot_guardar
            // 
            bot_guardar.AutoSize = false;
            bot_guardar.Enabled = false;
            bot_guardar.Image = (Image)resources.GetObject("bot_guardar.Image");
            bot_guardar.ImageTransparentColor = Color.Magenta;
            bot_guardar.Name = "bot_guardar";
            bot_guardar.Size = new Size(85, 30);
            bot_guardar.Text = "Guardar";
            bot_guardar.ToolTipText = "Guardar Documento";
            bot_guardar.Click += bot_guardar_Click;
            // 
            // bot_cancelar
            // 
            bot_cancelar.AutoSize = false;
            bot_cancelar.Enabled = false;
            bot_cancelar.Image = (Image)resources.GetObject("bot_cancelar.Image");
            bot_cancelar.ImageTransparentColor = Color.Magenta;
            bot_cancelar.Name = "bot_cancelar";
            bot_cancelar.Size = new Size(85, 30);
            bot_cancelar.Text = "Cancelar";
            bot_cancelar.ToolTipText = "Cancelar Documento";
            bot_cancelar.Click += bot_cancelar_Click;
            // 
            // btn_update
            // 
            btn_update.AutoSize = false;
            btn_update.Image = (Image)resources.GetObject("btn_update.Image");
            btn_update.ImageTransparentColor = Color.Magenta;
            btn_update.Name = "btn_update";
            btn_update.Size = new Size(85, 30);
            btn_update.Text = "Update ";
            btn_update.Click += btn_update_Click;
            // 
            // bot_buscar
            // 
            bot_buscar.AutoSize = false;
            bot_buscar.Image = (Image)resources.GetObject("bot_buscar.Image");
            bot_buscar.ImageScaling = ToolStripItemImageScaling.None;
            bot_buscar.ImageTransparentColor = Color.Magenta;
            bot_buscar.Name = "bot_buscar";
            bot_buscar.Size = new Size(85, 30);
            bot_buscar.Text = "Buscar";
            bot_buscar.ToolTipText = "Formulario de Busqueda";
            bot_buscar.Click += bot_buscar_Click;
            // 
            // bot_print
            // 
            bot_print.AutoSize = false;
            bot_print.Image = (Image)resources.GetObject("bot_print.Image");
            bot_print.ImageTransparentColor = Color.Magenta;
            bot_print.Name = "bot_print";
            bot_print.Size = new Size(85, 30);
            bot_print.Text = "Print";
            bot_print.ToolTipText = "Imprimir Documento";
            // 
            // bot_excel
            // 
            bot_excel.AutoSize = false;
            bot_excel.Image = (Image)resources.GetObject("bot_excel.Image");
            bot_excel.ImageTransparentColor = Color.Magenta;
            bot_excel.Name = "bot_excel";
            bot_excel.Size = new Size(85, 30);
            bot_excel.Text = "Excel";
            bot_excel.ToolTipText = "Importar Data Excel";
            bot_excel.Click += bot_excel_Click;
            // 
            // groupBox3
            // 
            groupBox3.Controls.Add(label7);
            groupBox3.Location = new Point(391, 586);
            groupBox3.Name = "groupBox3";
            groupBox3.Size = new Size(510, 132);
            groupBox3.TabIndex = 21;
            groupBox3.TabStop = false;
            groupBox3.Text = "Informacion: ";
            // 
            // label7
            // 
            label7.Location = new Point(9, 27);
            label7.Name = "label7";
            label7.Size = new Size(495, 88);
            label7.TabIndex = 0;
            label7.Text = "Puede anular un producto en el boton de \"Update\",\r\n y dar click en el check\r\n\"Producto Anulado\" y guardar los cambios, asi se \r\nanular� el producto.\r\n";
            // 
            // FrmProductos
            // 
            AutoScaleDimensions = new SizeF(8F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(984, 730);
            Controls.Add(groupBox3);
            Controls.Add(toolStrip1);
            Controls.Add(panel1);
            Controls.Add(chk_product_anulado);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            Controls.Add(txt_ratio);
            Controls.Add(label8);
            Controls.Add(txt_precio);
            Controls.Add(label6);
            Controls.Add(txt_codebar);
            Controls.Add(label5);
            Controls.Add(txt_referencia);
            Controls.Add(label4);
            Controls.Add(txt_productdescription);
            Controls.Add(label3);
            Controls.Add(txt_productname);
            Controls.Add(label2);
            Controls.Add(txt_partid);
            Controls.Add(label1);
            Font = new Font("JetBrains Mono", 9.75F, FontStyle.Bold);
            ForeColor = SystemColors.ActiveCaptionText;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmProductos";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Administrar Productos del Sistemas";
            FormClosing += FrmProductos_FormClosing;
            Load += FrmProductos_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            toolStrip1.ResumeLayout(false);
            toolStrip1.PerformLayout();
            groupBox3.ResumeLayout(false);
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txt_partid;
        private TextBox txt_productname;
        private Label label2;
        private TextBox txt_productdescription;
        private Label label3;
        private TextBox txt_referencia;
        private Label label4;
        private TextBox txt_codebar;
        private Label label5;
        private TextBox txt_precio;
        private Label label6;
        private TextBox txt_ratio;
        private Label label8;
        private GroupBox groupBox1;
        private Label lbl_contador;
        private GroupBox groupBox2;
        private RadioButton rad_graphics;
        private RadioButton rad_hoja;
        private RadioButton rad_master;
        private CheckBox chk_product_anulado;
        private Panel panel1;
        private Label label10;
        private PictureBox pictureBox1;
        private ToolStrip toolStrip1;
        private ToolStripButton bot_primero;
        private ToolStripButton bot_siguiente;
        private ToolStripButton bot_anterior;
        private ToolStripButton bot_excel;
        private ToolStripButton bot_ultimo;
        private ToolStripButton bot_nuevo;
        private ToolStripButton bot_cancelar;
        private ToolStripButton bot_guardar;
        private ToolStripButton bot_buscar;
        private ToolStripButton bot_print;
        private RadioButton rad_rollocortado;
        private GroupBox groupBox3;
        private Label label7;
        private ToolStripButton btn_update;
    }
}


