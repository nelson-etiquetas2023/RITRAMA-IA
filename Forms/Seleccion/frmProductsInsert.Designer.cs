namespace Ritrama2025.Forms.Seleccion
{
    partial class FrmProductsInsert
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmProductsInsert));
            panel1 = new Panel();
            label14 = new Label();
            pictureBox6 = new PictureBox();
            label13 = new Label();
            label1 = new Label();
            txt_productid = new TextBox();
            txt_productname = new TextBox();
            label2 = new Label();
            btn_buscar = new Button();
            backgroundWorker1 = new System.ComponentModel.BackgroundWorker();
            groupBox1 = new GroupBox();
            rad_hojas = new RadioButton();
            rad_graphics = new RadioButton();
            rad_rolloCortado = new RadioButton();
            rad_master = new RadioButton();
            txt_width = new TextBox();
            label3 = new Label();
            txt_lenght = new TextBox();
            txt_msi = new TextBox();
            label4 = new Label();
            label5 = new Label();
            txt_core = new TextBox();
            txt_splice = new TextBox();
            label6 = new Label();
            label7 = new Label();
            label8 = new Label();
            txt_rollid = new TextBox();
            label9 = new Label();
            txt_ubic = new TextBox();
            label10 = new Label();
            txt_cant = new TextBox();
            btn_guardar = new Button();
            btn_cancel = new Button();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).BeginInit();
            groupBox1.SuspendLayout();
            SuspendLayout();
            // 
            // panel1
            // 
            panel1.Controls.Add(label14);
            panel1.Controls.Add(pictureBox6);
            panel1.Controls.Add(label13);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Margin = new Padding(3, 5, 3, 5);
            panel1.Name = "panel1";
            panel1.Size = new Size(627, 98);
            panel1.TabIndex = 37;
            // 
            // label14
            // 
            label14.AutoSize = true;
            label14.Location = new Point(1114, 31);
            label14.Name = "label14";
            label14.Size = new Size(161, 25);
            label14.TabIndex = 97;
            label14.Text = "Registros : 1/100";
            // 
            // pictureBox6
            // 
            pictureBox6.Image = (Image)resources.GetObject("pictureBox6.Image");
            pictureBox6.Location = new Point(34, 16);
            pictureBox6.Margin = new Padding(3, 5, 3, 5);
            pictureBox6.Name = "pictureBox6";
            pictureBox6.Size = new Size(69, 67);
            pictureBox6.TabIndex = 97;
            pictureBox6.TabStop = false;
            // 
            // label13
            // 
            label13.AutoSize = true;
            label13.Location = new Point(110, 28);
            label13.Name = "label13";
            label13.Size = new Size(331, 32);
            label13.TabIndex = 0;
            label13.Text = "INSERCION DE PRODUCTOS";
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(34, 146);
            label1.Name = "label1";
            label1.Size = new Size(74, 18);
            label1.TabIndex = 38;
            label1.Text = "Product Id.";
            // 
            // txt_productid
            // 
            txt_productid.Location = new Point(129, 145);
            txt_productid.Margin = new Padding(3, 4, 3, 4);
            txt_productid.Name = "txt_productid";
            txt_productid.Size = new Size(215, 25);
            txt_productid.TabIndex = 39;
            // 
            // txt_productname
            // 
            txt_productname.Location = new Point(129, 180);
            txt_productname.Margin = new Padding(3, 4, 3, 4);
            txt_productname.Name = "txt_productname";
            txt_productname.Size = new Size(468, 25);
            txt_productname.TabIndex = 41;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(10, 181);
            label2.Name = "label2";
            label2.Size = new Size(98, 18);
            label2.TabIndex = 40;
            label2.Text = "Product Name.";
            // 
            // btn_buscar
            // 
            btn_buscar.Location = new Point(352, 144);
            btn_buscar.Margin = new Padding(3, 4, 3, 4);
            btn_buscar.Name = "btn_buscar";
            btn_buscar.Size = new Size(34, 30);
            btn_buscar.TabIndex = 42;
            btn_buscar.Text = "...";
            btn_buscar.Click += Btn_buscar_Click;
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rad_hojas);
            groupBox1.Controls.Add(rad_graphics);
            groupBox1.Controls.Add(rad_rolloCortado);
            groupBox1.Controls.Add(rad_master);
            groupBox1.Enabled = false;
            groupBox1.Location = new Point(369, 227);
            groupBox1.Margin = new Padding(3, 4, 3, 4);
            groupBox1.Name = "groupBox1";
            groupBox1.Padding = new Padding(3, 4, 3, 4);
            groupBox1.Size = new Size(229, 129);
            groupBox1.TabIndex = 43;
            groupBox1.TabStop = false;
            groupBox1.Text = "Tipo Product";
            // 
            // rad_hojas
            // 
            rad_hojas.AutoSize = true;
            rad_hojas.Location = new Point(25, 82);
            rad_hojas.Margin = new Padding(3, 4, 3, 4);
            rad_hojas.Name = "rad_hojas";
            rad_hojas.Size = new Size(66, 22);
            rad_hojas.TabIndex = 3;
            rad_hojas.TabStop = true;
            rad_hojas.Text = "Resma";
            // 
            // rad_graphics
            // 
            rad_graphics.AutoSize = true;
            rad_graphics.Location = new Point(25, 62);
            rad_graphics.Margin = new Padding(3, 4, 3, 4);
            rad_graphics.Name = "rad_graphics";
            rad_graphics.Size = new Size(78, 22);
            rad_graphics.TabIndex = 2;
            rad_graphics.TabStop = true;
            rad_graphics.Text = "Graphics";
            // 
            // rad_rolloCortado
            // 
            rad_rolloCortado.AutoSize = true;
            rad_rolloCortado.Location = new Point(25, 44);
            rad_rolloCortado.Margin = new Padding(3, 4, 3, 4);
            rad_rolloCortado.Name = "rad_rolloCortado";
            rad_rolloCortado.Size = new Size(108, 22);
            rad_rolloCortado.TabIndex = 1;
            rad_rolloCortado.TabStop = true;
            rad_rolloCortado.Text = "Rollo Cortado";
            // 
            // rad_master
            // 
            rad_master.AutoSize = true;
            rad_master.Location = new Point(25, 26);
            rad_master.Margin = new Padding(3, 4, 3, 4);
            rad_master.Name = "rad_master";
            rad_master.Size = new Size(68, 22);
            rad_master.TabIndex = 0;
            rad_master.TabStop = true;
            rad_master.Text = "Master";
            // 
            // txt_width
            // 
            txt_width.Location = new Point(129, 215);
            txt_width.Margin = new Padding(3, 4, 3, 4);
            txt_width.Name = "txt_width";
            txt_width.Size = new Size(215, 25);
            txt_width.TabIndex = 44;
            txt_width.Enter += Txt_width_Enter;
            txt_width.KeyPress += Txt_width_KeyPress;
            txt_width.KeyUp += Txt_width_KeyUp;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(21, 216);
            label3.Name = "label3";
            label3.Size = new Size(87, 18);
            label3.TabIndex = 45;
            label3.Text = "Width [Inch]:";
            // 
            // txt_lenght
            // 
            txt_lenght.Location = new Point(129, 250);
            txt_lenght.Margin = new Padding(3, 4, 3, 4);
            txt_lenght.Name = "txt_lenght";
            txt_lenght.Size = new Size(215, 25);
            txt_lenght.TabIndex = 46;
            txt_lenght.Enter += Txt_lenght_Enter;
            txt_lenght.KeyPress += Txt_lenght_KeyPress;
            txt_lenght.KeyUp += Txt_lenght_KeyUp;
            // 
            // txt_msi
            // 
            txt_msi.Location = new Point(129, 284);
            txt_msi.Margin = new Padding(3, 4, 3, 4);
            txt_msi.Name = "txt_msi";
            txt_msi.ReadOnly = true;
            txt_msi.Size = new Size(215, 25);
            txt_msi.TabIndex = 47;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(21, 251);
            label4.Name = "label4";
            label4.Size = new Size(87, 18);
            label4.TabIndex = 48;
            label4.Text = "Lengh [Pies]:";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(73, 286);
            label5.Name = "label5";
            label5.Size = new Size(35, 18);
            label5.TabIndex = 49;
            label5.Text = "MSI:";
            // 
            // txt_core
            // 
            txt_core.Location = new Point(129, 319);
            txt_core.Margin = new Padding(3, 4, 3, 4);
            txt_core.Name = "txt_core";
            txt_core.Size = new Size(215, 25);
            txt_core.TabIndex = 50;
            txt_core.Enter += Txt_core_Enter;
            txt_core.KeyPress += Txt_core_KeyPress;
            // 
            // txt_splice
            // 
            txt_splice.Location = new Point(129, 354);
            txt_splice.Margin = new Padding(3, 4, 3, 4);
            txt_splice.Name = "txt_splice";
            txt_splice.Size = new Size(215, 25);
            txt_splice.TabIndex = 51;
            txt_splice.Enter += Txt_splice_Enter;
            txt_splice.KeyPress += Txt_splice_KeyPress;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(65, 320);
            label6.Name = "label6";
            label6.Size = new Size(43, 18);
            label6.TabIndex = 52;
            label6.Text = "Core :";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.Location = new Point(59, 355);
            label7.Name = "label7";
            label7.Size = new Size(49, 18);
            label7.TabIndex = 53;
            label7.Text = "Splice :";
            // 
            // label8
            // 
            label8.AutoSize = true;
            label8.Location = new Point(55, 390);
            label8.Name = "label8";
            label8.Size = new Size(53, 18);
            label8.TabIndex = 55;
            label8.Text = "Roll-Id :";
            // 
            // txt_rollid
            // 
            txt_rollid.Location = new Point(129, 389);
            txt_rollid.Margin = new Padding(3, 4, 3, 4);
            txt_rollid.Name = "txt_rollid";
            txt_rollid.Size = new Size(215, 25);
            txt_rollid.TabIndex = 54;
            txt_rollid.Enter += Txt_rollid_Enter;
            // 
            // label9
            // 
            label9.AutoSize = true;
            label9.Location = new Point(34, 425);
            label9.Name = "label9";
            label9.Size = new Size(74, 18);
            label9.TabIndex = 57;
            label9.Text = "Ubicacion :";
            // 
            // txt_ubic
            // 
            txt_ubic.Location = new Point(129, 424);
            txt_ubic.Margin = new Padding(3, 4, 3, 4);
            txt_ubic.Name = "txt_ubic";
            txt_ubic.Size = new Size(215, 25);
            txt_ubic.TabIndex = 56;
            txt_ubic.Enter += Txt_ubic_Enter;
            // 
            // label10
            // 
            label10.AutoSize = true;
            label10.Location = new Point(39, 460);
            label10.Name = "label10";
            label10.Size = new Size(69, 18);
            label10.TabIndex = 59;
            label10.Text = "Cantidad :";
            // 
            // txt_cant
            // 
            txt_cant.Location = new Point(129, 458);
            txt_cant.Margin = new Padding(3, 4, 3, 4);
            txt_cant.Name = "txt_cant";
            txt_cant.Size = new Size(215, 25);
            txt_cant.TabIndex = 58;
            txt_cant.Enter += Txt_cant_Enter;
            txt_cant.KeyPress += Txt_cant_KeyPress;
            // 
            // btn_guardar
            // 
            btn_guardar.Image = (Image)resources.GetObject("btn_guardar.Image");
            btn_guardar.Location = new Point(40, 590);
            btn_guardar.Margin = new Padding(3, 4, 3, 4);
            btn_guardar.Name = "btn_guardar";
            btn_guardar.Size = new Size(138, 43);
            btn_guardar.TabIndex = 60;
            btn_guardar.Text = "Guardar";
            btn_guardar.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_guardar.Click += Btn_guardar_Click;
            // 
            // btn_cancel
            // 
            btn_cancel.Image = (Image)resources.GetObject("btn_cancel.Image");
            btn_cancel.Location = new Point(185, 590);
            btn_cancel.Margin = new Padding(3, 4, 3, 4);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(135, 43);
            btn_cancel.TabIndex = 61;
            btn_cancel.Text = "Cancelar";
            btn_cancel.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_cancel.Click += Btn_cancel_Click;
            // 
            // FrmProductsInsert
            // 
            AutoScaleDimensions = new SizeF(8F, 18F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(627, 673);
            Controls.Add(btn_cancel);
            Controls.Add(btn_guardar);
            Controls.Add(label10);
            Controls.Add(txt_cant);
            Controls.Add(label9);
            Controls.Add(txt_ubic);
            Controls.Add(label8);
            Controls.Add(txt_rollid);
            Controls.Add(label7);
            Controls.Add(label6);
            Controls.Add(txt_splice);
            Controls.Add(txt_core);
            Controls.Add(label5);
            Controls.Add(label4);
            Controls.Add(txt_msi);
            Controls.Add(txt_lenght);
            Controls.Add(label3);
            Controls.Add(txt_width);
            Controls.Add(groupBox1);
            Controls.Add(btn_buscar);
            Controls.Add(txt_productname);
            Controls.Add(label2);
            Controls.Add(txt_productid);
            Controls.Add(label1);
            Controls.Add(panel1);
            Font = new Font("Noto Sans", 9.75F);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Margin = new Padding(3, 4, 3, 4);
            Name = "FrmProductsInsert";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Insercion de Productos";
            Load += FrmProductsInsert_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox6).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Panel panel1;
        private Label label14;
        private PictureBox pictureBox6;
        private Label label13;
        private Label label1;
        private TextBox txt_productid;
        private TextBox txt_productname;
        private Label label2;
        private Button btn_buscar;
        private System.ComponentModel.BackgroundWorker backgroundWorker1;
        private GroupBox groupBox1;
        private RadioButton rad_rolloCortado;
        private RadioButton rad_master;
        private RadioButton rad_hojas;
        private RadioButton rad_graphics;
        private TextBox txt_width;
        private Label label3;
        private TextBox txt_lenght;
        private TextBox txt_msi;
        private Label label4;
        private Label label5;
        private TextBox txt_core;
        private TextBox txt_splice;
        private Label label6;
        private Label label7;
        private Label label8;
        private TextBox txt_rollid;
        private Label label9;
        private TextBox txt_ubic;
        private Label label10;
        private TextBox txt_cant;
        private Button btn_guardar;
        private Button btn_cancel;
    }
}


