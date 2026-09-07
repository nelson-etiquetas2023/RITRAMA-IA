namespace Ritrama2025.Forms.Otros
{
    partial class PrintLabelsRolls
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(PrintLabelsRolls));
            button1 = new Button();
            txt_hasta = new TextBox();
            txt_desde = new TextBox();
            label1 = new Label();
            label2 = new Label();
            label3 = new Label();
            panel1 = new Panel();
            label4 = new Label();
            txt_numero_etiq = new TextBox();
            label5 = new Label();
            label6 = new Label();
            CboSelectPrinters = new ComboBox();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // button1
            // 
            button1.Image = (Image)resources.GetObject("button1.Image");
            button1.Location = new Point(100, 321);
            button1.Name = "button1";
            button1.Size = new Size(150, 66);
            button1.TabIndex = 0;
            button1.Text = "Print Label";
            button1.TextImageRelation = TextImageRelation.ImageBeforeText;
            button1.Click += button1_Click;
            // 
            // txt_hasta
            // 
            txt_hasta.Location = new Point(71, 278);
            txt_hasta.Name = "txt_hasta";
            txt_hasta.Size = new Size(59, 23);
            txt_hasta.TabIndex = 1;
            // 
            // txt_desde
            // 
            txt_desde.Location = new Point(71, 249);
            txt_desde.Name = "txt_desde";
            txt_desde.Size = new Size(59, 23);
            txt_desde.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 249);
            label1.Name = "label1";
            label1.Size = new Size(42, 15);
            label1.TabIndex = 3;
            label1.Text = "Desde:";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(12, 281);
            label2.Name = "label2";
            label2.Size = new Size(40, 15);
            label2.TabIndex = 4;
            label2.Text = "Hasta:";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(12, 214);
            label3.Name = "label3";
            label3.Size = new Size(117, 15);
            label3.TabIndex = 5;
            label3.Text = "Imprimir etiquetas :";
            // 
            // panel1
            // 
            panel1.Controls.Add(label4);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(355, 100);
            panel1.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(31, 31);
            label4.Name = "label4";
            label4.Size = new Size(298, 28);
            label4.TabIndex = 7;
            label4.Text = "Etiquetas de Codigo de Barra :";
            // 
            // txt_numero_etiq
            // 
            txt_numero_etiq.Location = new Point(100, 106);
            txt_numero_etiq.Name = "txt_numero_etiq";
            txt_numero_etiq.Size = new Size(89, 23);
            txt_numero_etiq.TabIndex = 7;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 109);
            label5.Name = "label5";
            label5.Size = new Size(85, 15);
            label5.TabIndex = 8;
            label5.Text = "Total Etiqueas :";
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 138);
            label6.Name = "label6";
            label6.Size = new Size(66, 15);
            label6.TabIndex = 9;
            label6.Text = "Impresora :";
            // 
            // CboSelectPrinters
            // 
            CboSelectPrinters.FormattingEnabled = true;
            CboSelectPrinters.Location = new Point(100, 135);
            CboSelectPrinters.Name = "CboSelectPrinters";
            CboSelectPrinters.Size = new Size(246, 23);
            CboSelectPrinters.TabIndex = 10;
            // 
            // PrintLabelsRolls
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(355, 399);
            Controls.Add(CboSelectPrinters);
            Controls.Add(label6);
            Controls.Add(label5);
            Controls.Add(txt_numero_etiq);
            Controls.Add(panel1);
            Controls.Add(label3);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txt_desde);
            Controls.Add(txt_hasta);
            Controls.Add(button1);
            Name = "PrintLabelsRolls";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "PrintLabelsRolls";
            Load += PrintLabelsRolls_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button button1;
        private TextBox txt_hasta;
        private TextBox txt_desde;
        private Label label1;
        private Label label2;
        private Label label3;
        private Panel panel1;
        private Label label4;
        private TextBox txt_numero_etiq;
        private Label label5;
        private Label label6;
        private ComboBox CboSelectPrinters;
    }
}


