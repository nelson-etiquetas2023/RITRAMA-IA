namespace Ritrama2025.Forms.Otros
{
    partial class Frm_ImportacionExcel
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ImportacionExcel));
            label4 = new Label();
            ConsoleResults = new TextBox();
            label2 = new Label();
            panel1 = new Panel();
            pictureBox1 = new PictureBox();
            label3 = new Label();
            label5 = new Label();
            txt_path_file = new TextBox();
            bot_guardar = new Button();
            bot_validar = new Button();
            counts_rows = new Label();
            grid_items = new DataGridView();
            label1 = new Label();
            bot_cargar = new Button();
            bot_buscar = new Button();
            txt_name_file = new TextBox();
            label6 = new Label();
            panel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).BeginInit();
            ((System.ComponentModel.ISupportInitialize)grid_items).BeginInit();
            SuspendLayout();
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(12, 634);
            label4.Name = "label4";
            label4.Size = new Size(142, 17);
            label4.TabIndex = 30;
            label4.Text = "Consola de Resultados";
            // 
            // ConsoleResults
            // 
            ConsoleResults.Location = new Point(12, 654);
            ConsoleResults.Multiline = true;
            ConsoleResults.Name = "ConsoleResults";
            ConsoleResults.ReadOnly = true;
            ConsoleResults.Size = new Size(468, 108);
            ConsoleResults.TabIndex = 29;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(286, 237);
            label2.Name = "label2";
            label2.Size = new Size(119, 17);
            label2.TabIndex = 28;
            label2.Text = "Datos Importados";
            // 
            // panel1
            // 
            panel1.Controls.Add(pictureBox1);
            panel1.Controls.Add(label3);
            panel1.Dock = DockStyle.Top;
            panel1.Location = new Point(0, 0);
            panel1.Name = "panel1";
            panel1.Size = new Size(732, 87);
            panel1.TabIndex = 27;
            // 
            // pictureBox1
            // 
            pictureBox1.Image = (Image)resources.GetObject("pictureBox1.Image");
            pictureBox1.Location = new Point(100, 24);
            pictureBox1.Name = "pictureBox1";
            pictureBox1.Size = new Size(35, 33);
            pictureBox1.TabIndex = 9;
            pictureBox1.TabStop = false;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(141, 24);
            label3.Name = "label3";
            label3.Size = new Size(447, 32);
            label3.TabIndex = 8;
            label3.Text = "Importación de Datos Materia Prima ";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(12, 165);
            label5.Name = "label5";
            label5.Size = new Size(150, 17);
            label5.TabIndex = 26;
            label5.Text = "Ruta de la Hoja de Excel";
            // 
            // txt_path_file
            // 
            txt_path_file.Location = new Point(12, 132);
            txt_path_file.Name = "txt_path_file";
            txt_path_file.ReadOnly = true;
            txt_path_file.Size = new Size(540, 25);
            txt_path_file.TabIndex = 25;
            // 
            // bot_guardar
            // 
            bot_guardar.Image = (Image)resources.GetObject("bot_guardar.Image");
            bot_guardar.Location = new Point(611, 175);
            bot_guardar.Name = "bot_guardar";
            bot_guardar.Size = new Size(109, 35);
            bot_guardar.TabIndex = 24;
            bot_guardar.Text = "Guardar";
            bot_guardar.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_guardar.Click += bot_guardar_Click;
            // 
            // bot_validar
            // 
            bot_validar.Enabled = false;
            bot_validar.Image = (Image)resources.GetObject("bot_validar.Image");
            bot_validar.Location = new Point(610, 216);
            bot_validar.Name = "bot_validar";
            bot_validar.Size = new Size(109, 35);
            bot_validar.TabIndex = 23;
            bot_validar.Text = "Validar";
            bot_validar.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // counts_rows
            // 
            counts_rows.AutoSize = true;
            counts_rows.Location = new Point(531, 668);
            counts_rows.Name = "counts_rows";
            counts_rows.Size = new Size(136, 17);
            counts_rows.TabIndex = 22;
            counts_rows.Text = "Numero de Fila 0 e 0";
            // 
            // grid_items
            // 
            grid_items.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid_items.Location = new Point(12, 257);
            grid_items.Name = "grid_items";
            grid_items.Size = new Size(707, 364);
            grid_items.TabIndex = 21;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(15, -30);
            label1.Name = "label1";
            label1.Size = new Size(179, 17);
            label1.TabIndex = 20;
            label1.Text = "Nombre de la Hoja de Excel";
            // 
            // bot_cargar
            // 
            bot_cargar.Image = (Image)resources.GetObject("bot_cargar.Image");
            bot_cargar.Location = new Point(611, 134);
            bot_cargar.Name = "bot_cargar";
            bot_cargar.Size = new Size(109, 35);
            bot_cargar.TabIndex = 19;
            bot_cargar.Text = "Cargar";
            bot_cargar.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_cargar.Click += bot_cargar_Click;
            // 
            // bot_buscar
            // 
            bot_buscar.Image = (Image)resources.GetObject("bot_buscar.Image");
            bot_buscar.Location = new Point(611, 93);
            bot_buscar.Name = "bot_buscar";
            bot_buscar.Size = new Size(109, 35);
            bot_buscar.TabIndex = 18;
            bot_buscar.Text = "Buscar";
            bot_buscar.TextImageRelation = TextImageRelation.ImageBeforeText;
            bot_buscar.Click += Bot_buscar_Click;
            // 
            // txt_name_file
            // 
            txt_name_file.Location = new Point(12, 185);
            txt_name_file.Name = "txt_name_file";
            txt_name_file.ReadOnly = true;
            txt_name_file.Size = new Size(540, 25);
            txt_name_file.TabIndex = 17;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(12, 112);
            label6.Name = "label6";
            label6.Size = new Size(173, 17);
            label6.TabIndex = 31;
            label6.Text = "Nombre de la Hoja de Excel";
            // 
            // Frm_ImportacionExcel
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(732, 774);
            Controls.Add(label6);
            Controls.Add(label4);
            Controls.Add(ConsoleResults);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(label5);
            Controls.Add(txt_path_file);
            Controls.Add(bot_guardar);
            Controls.Add(bot_validar);
            Controls.Add(counts_rows);
            Controls.Add(grid_items);
            Controls.Add(label1);
            Controls.Add(bot_cargar);
            Controls.Add(bot_buscar);
            Controls.Add(txt_name_file);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            Name = "Frm_ImportacionExcel";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Frm_ImportacionExcel";
            Load += Frm_ImportacionExcel_Load;
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)pictureBox1).EndInit();
            ((System.ComponentModel.ISupportInitialize)grid_items).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label4;
        private TextBox ConsoleResults;
        private Label label2;
        private Panel panel1;
        private PictureBox pictureBox1;
        private Label label3;
        private Label label5;
        private TextBox txt_path_file;
        private Button bot_guardar;
        private Button bot_validar;
        private Label counts_rows;
        private DataGridView grid_items;
        private Label label1;
        private Button bot_cargar;
        private Button bot_buscar;
        private TextBox txt_name_file;
        private Label label6;
    }
}


