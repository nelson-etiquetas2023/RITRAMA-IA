namespace Ritrama2025.Forms
{
    partial class FrmCodeBarLabel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmCodeBarLabel));
            btn_buscar_printer = new Button();
            textBox1 = new TextBox();
            textBox2 = new TextBox();
            btn_send_printer_usb = new Button();
            label1 = new Label();
            numero_copias = new NumericUpDown();
            rad_label1 = new RadioButton();
            rad_label2 = new RadioButton();
            rad_label3 = new RadioButton();
            label2 = new Label();
            label3 = new Label();
            groupBox1 = new GroupBox();
            grp_marca_printer = new GroupBox();
            rad_zebra = new RadioButton();
            rad_TSC = new RadioButton();
            previewLabels = new PictureBox();
            label4 = new Label();
            btn_PreviewLabels = new Button();
            cbo_printer = new ComboBox();
            btn_ServicePrintThermal = new Button();
            btn_raw_printer = new Button();
            ((System.ComponentModel.ISupportInitialize)numero_copias).BeginInit();
            groupBox1.SuspendLayout();
            grp_marca_printer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)previewLabels).BeginInit();
            SuspendLayout();
            // 
            // btn_buscar_printer
            // 
            btn_buscar_printer.Image = Properties.Resources.job_seeker_50px;
            btn_buscar_printer.ImageAlign = ContentAlignment.TopCenter;
            btn_buscar_printer.Location = new Point(12, 12);
            btn_buscar_printer.Name = "btn_buscar_printer";
            btn_buscar_printer.Size = new Size(302, 66);
            btn_buscar_printer.TabIndex = 1;
            btn_buscar_printer.Text = "Buscar Printer Instaladas local";
            btn_buscar_printer.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_buscar_printer.Click += Btn_buscar_printer_Click;
            // 
            // textBox1
            // 
            textBox1.Location = new Point(486, 37);
            textBox1.Name = "textBox1";
            textBox1.Size = new Size(302, 25);
            textBox1.TabIndex = 2;
            // 
            // textBox2
            // 
            textBox2.Location = new Point(486, 87);
            textBox2.Name = "textBox2";
            textBox2.Size = new Size(302, 24);
            textBox2.TabIndex = 3;
            // 
            // btn_send_printer_usb
            // 
            btn_send_printer_usb.Image = Properties.Resources.print_48px1;
            btn_send_printer_usb.Location = new Point(12, 84);
            btn_send_printer_usb.Name = "btn_send_printer_usb";
            btn_send_printer_usb.Size = new Size(302, 66);
            btn_send_printer_usb.TabIndex = 4;
            btn_send_printer_usb.Text = "Enviar ZPL impresora USB local";
            btn_send_printer_usb.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_send_printer_usb.Click += Button1_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 321);
            label1.Name = "label1";
            label1.Size = new Size(112, 15);
            label1.TabIndex = 5;
            label1.Text = "Numero de Copias :";
            // 
            // numero_copias
            // 
            numero_copias.Location = new Point(18, 339);
            numero_copias.Name = "numero_copias";
            numero_copias.Size = new Size(120, 23);
            numero_copias.TabIndex = 6;
            numero_copias.Value = new decimal(new int[] { 1, 0, 0, 0 });
            // 
            // rad_label1
            // 
            rad_label1.AutoSize = true;
            rad_label1.Location = new Point(6, 24);
            rad_label1.Name = "rad_label1";
            rad_label1.Size = new Size(79, 19);
            rad_label1.TabIndex = 7;
            rad_label1.TabStop = true;
            rad_label1.Text = "Formato 1";
            // 
            // rad_label2
            // 
            rad_label2.AutoSize = true;
            rad_label2.Location = new Point(6, 49);
            rad_label2.Name = "rad_label2";
            rad_label2.Size = new Size(79, 19);
            rad_label2.TabIndex = 8;
            rad_label2.TabStop = true;
            rad_label2.Text = "Formato 2";
            // 
            // rad_label3
            // 
            rad_label3.AutoSize = true;
            rad_label3.Location = new Point(6, 74);
            rad_label3.Name = "rad_label3";
            rad_label3.Size = new Size(79, 19);
            rad_label3.TabIndex = 9;
            rad_label3.TabStop = true;
            rad_label3.Text = "Formato 3";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(486, 19);
            label2.Name = "label2";
            label2.Size = new Size(235, 18);
            label2.TabIndex = 10;
            label2.Text = "Nombre de la Impresora Selecionadea";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(486, 66);
            label3.Name = "label3";
            label3.Size = new Size(131, 18);
            label3.TabIndex = 11;
            label3.Text = "Cadena de Conexion";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(rad_label3);
            groupBox1.Controls.Add(rad_label1);
            groupBox1.Controls.Add(rad_label2);
            groupBox1.Location = new Point(12, 396);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(129, 119);
            groupBox1.TabIndex = 12;
            groupBox1.TabStop = false;
            groupBox1.Text = "Formatos";
            // 
            // grp_marca_printer
            // 
            grp_marca_printer.Controls.Add(rad_zebra);
            grp_marca_printer.Controls.Add(rad_TSC);
            grp_marca_printer.Location = new Point(147, 396);
            grp_marca_printer.Name = "grp_marca_printer";
            grp_marca_printer.Size = new Size(129, 119);
            grp_marca_printer.TabIndex = 13;
            grp_marca_printer.TabStop = false;
            grp_marca_printer.Text = "Marca Printer";
            // 
            // rad_zebra
            // 
            rad_zebra.AutoSize = true;
            rad_zebra.Location = new Point(6, 24);
            rad_zebra.Name = "rad_zebra";
            rad_zebra.Size = new Size(55, 19);
            rad_zebra.TabIndex = 7;
            rad_zebra.TabStop = true;
            rad_zebra.Text = "Zebra";
            // 
            // rad_TSC
            // 
            rad_TSC.AutoSize = true;
            rad_TSC.Location = new Point(6, 49);
            rad_TSC.Name = "rad_TSC";
            rad_TSC.Size = new Size(41, 19);
            rad_TSC.TabIndex = 8;
            rad_TSC.TabStop = true;
            rad_TSC.Text = "Tsc";
            // 
            // previewLabels
            // 
            previewLabels.Location = new Point(486, 244);
            previewLabels.Name = "previewLabels";
            previewLabels.Size = new Size(302, 194);
            previewLabels.TabIndex = 14;
            previewLabels.TabStop = false;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(486, 221);
            label4.Name = "label4";
            label4.Size = new Size(139, 18);
            label4.TabIndex = 15;
            label4.Text = "Preview de la Etiqueta";
            // 
            // btn_PreviewLabels
            // 
            btn_PreviewLabels.Location = new Point(486, 151);
            btn_PreviewLabels.Name = "btn_PreviewLabels";
            btn_PreviewLabels.Size = new Size(189, 67);
            btn_PreviewLabels.TabIndex = 16;
            btn_PreviewLabels.Text = "Preview";
            btn_PreviewLabels.Click += Btn_PreviewLabels_Click;
            // 
            // cbo_printer
            // 
            cbo_printer.FormattingEnabled = true;
            cbo_printer.Location = new Point(18, 368);
            cbo_printer.Name = "cbo_printer";
            cbo_printer.Size = new Size(302, 23);
            cbo_printer.TabIndex = 17;
            // 
            // btn_ServicePrintThermal
            // 
            btn_ServicePrintThermal.Image = (Image)resources.GetObject("btn_ServicePrintThermal.Image");
            btn_ServicePrintThermal.Location = new Point(12, 156);
            btn_ServicePrintThermal.Name = "btn_ServicePrintThermal";
            btn_ServicePrintThermal.Size = new Size(302, 66);
            btn_ServicePrintThermal.TabIndex = 18;
            btn_ServicePrintThermal.Text = "Probar Servicio de Impresion";
            btn_ServicePrintThermal.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_ServicePrintThermal.Click += Btn_ServicePrintThermal_Click;
            // 
            // btn_raw_printer
            // 
            btn_raw_printer.Image = Properties.Resources.print_48px1;
            btn_raw_printer.Location = new Point(12, 228);
            btn_raw_printer.Name = "btn_raw_printer";
            btn_raw_printer.Size = new Size(302, 66);
            btn_raw_printer.TabIndex = 19;
            btn_raw_printer.Text = "Enviar ZPL Raw a Controlador";
            btn_raw_printer.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_raw_printer.Click += btn_raw_printer_Click;
            // 
            // FrmCodeBarLabel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 525);
            Controls.Add(btn_raw_printer);
            Controls.Add(btn_ServicePrintThermal);
            Controls.Add(cbo_printer);
            Controls.Add(btn_PreviewLabels);
            Controls.Add(label4);
            Controls.Add(previewLabels);
            Controls.Add(grp_marca_printer);
            Controls.Add(groupBox1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(numero_copias);
            Controls.Add(label1);
            Controls.Add(btn_send_printer_usb);
            Controls.Add(textBox2);
            Controls.Add(textBox1);
            Controls.Add(btn_buscar_printer);
            Name = "FrmCodeBarLabel";
            Text = "Configuracion de Etiquetas de Codigo de Barras";
            Load += FrmCodeBarLabel_Load;
            ((System.ComponentModel.ISupportInitialize)numero_copias).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            grp_marca_printer.ResumeLayout(false);
            grp_marca_printer.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)previewLabels).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion
        private Button btn_buscar_printer;
        private TextBox textBox1;
        private TextBox textBox2;
        private Button btn_send_printer_usb;
        private Label label1;
        private NumericUpDown numero_copias;
        private RadioButton rad_label1;
        private RadioButton rad_label2;
        private RadioButton rad_label3;
        private Label label2;
        private Label label3;
        private GroupBox groupBox1;
        private GroupBox grp_marca_printer;
        private RadioButton rad_zebra;
        private RadioButton rad_TSC;
        private PictureBox previewLabels;
        private Label label4;
        private Button btn_PreviewLabels;
        private ComboBox cbo_printer;
        private Button btn_ServicePrintThermal;
        private Button btn_raw_printer;
    }
}

