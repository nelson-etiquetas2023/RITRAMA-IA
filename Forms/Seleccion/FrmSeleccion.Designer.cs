namespace Ritrama2025.Forms.Seleccion
{
    partial class FrmSeleccion
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmSeleccion));
            Grid_Items = new DataGridView();
            txt_buscar = new TextBox();
            bot_buscar = new Button();
            label1 = new Label();
            groupBox1 = new GroupBox();
            ra_description = new RadioButton();
            ra_id = new RadioButton();
            Numero_reg = new Label();
            panel1 = new Panel();
            btn_delete_row = new Button();
            btn_add_new = new Button();
            titleform = new Label();
            label2 = new Label();
            ((System.ComponentModel.ISupportInitialize)Grid_Items).BeginInit();
            groupBox1.SuspendLayout();
            panel1.SuspendLayout();
            SuspendLayout();
            // 
            // Grid_Items
            // 
            Grid_Items.AllowUserToAddRows = false;
            Grid_Items.AllowUserToDeleteRows = false;
            Grid_Items.AllowUserToResizeRows = false;
            Grid_Items.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Grid_Items.Location = new Point(12, 124);
            Grid_Items.MultiSelect = false;
            Grid_Items.Name = "Grid_Items";
            Grid_Items.ReadOnly = true;
            Grid_Items.RowHeadersWidth = 36;
            Grid_Items.ScrollBars = ScrollBars.Vertical;
            Grid_Items.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            Grid_Items.Size = new Size(481, 258);
            Grid_Items.TabIndex = 0;
            Grid_Items.CellMouseDoubleClick += Grid_Items_CellMouseDoubleClick;
            // 
            // txt_buscar
            // 
            txt_buscar.Location = new Point(96, 96);
            txt_buscar.Name = "txt_buscar";
            txt_buscar.Size = new Size(308, 23);
            txt_buscar.TabIndex = 1;
            // 
            // bot_buscar
            // 
            bot_buscar.Location = new Point(410, 96);
            bot_buscar.Name = "bot_buscar";
            bot_buscar.Size = new Size(83, 22);
            bot_buscar.TabIndex = 2;
            bot_buscar.Text = "Buscar";
            bot_buscar.Click += Bot_buscar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 100);
            label1.Name = "label1";
            label1.Size = new Size(69, 15);
            label1.TabIndex = 3;
            label1.Text = "Buscar por:";
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(ra_description);
            groupBox1.Controls.Add(ra_id);
            groupBox1.Location = new Point(274, 386);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(221, 81);
            groupBox1.TabIndex = 4;
            groupBox1.TabStop = false;
            groupBox1.Text = "Filtrar por:";
            // 
            // ra_description
            // 
            ra_description.AutoSize = true;
            ra_description.Checked = true;
            ra_description.Location = new Point(36, 50);
            ra_description.Name = "ra_description";
            ra_description.Size = new Size(78, 19);
            ra_description.TabIndex = 1;
            ra_description.TabStop = true;
            ra_description.Text = "Por Name";
            // 
            // ra_id
            // 
            ra_id.AutoSize = true;
            ra_id.Location = new Point(36, 26);
            ra_id.Name = "ra_id";
            ra_id.Size = new Size(59, 19);
            ra_id.TabIndex = 0;
            ra_id.Text = "Por Id.";
            // 
            // Numero_reg
            // 
            Numero_reg.AutoSize = true;
            Numero_reg.Location = new Point(12, 386);
            Numero_reg.Name = "Numero_reg";
            Numero_reg.Size = new Size(108, 15);
            Numero_reg.TabIndex = 5;
            Numero_reg.Text = "Numero de Items:";
            // 
            // panel1
            // 
            panel1.Controls.Add(btn_delete_row);
            panel1.Controls.Add(btn_add_new);
            panel1.Controls.Add(titleform);
            panel1.Location = new Point(0, -1);
            panel1.Name = "panel1";
            panel1.Size = new Size(504, 74);
            panel1.TabIndex = 6;
            // 
            // btn_delete_row
            // 
            btn_delete_row.Image = (Image)resources.GetObject("btn_delete_row.Image");
            btn_delete_row.Location = new Point(368, 42);
            btn_delete_row.Name = "btn_delete_row";
            btn_delete_row.Size = new Size(119, 23);
            btn_delete_row.TabIndex = 2;
            btn_delete_row.Text = "Borrar Registro";
            btn_delete_row.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_delete_row.Click += Btn_delete_row_Click;
            // 
            // btn_add_new
            // 
            btn_add_new.Image = (Image)resources.GetObject("btn_add_new.Image");
            btn_add_new.Location = new Point(368, 13);
            btn_add_new.Name = "btn_add_new";
            btn_add_new.Size = new Size(119, 23);
            btn_add_new.TabIndex = 1;
            btn_add_new.Text = "Agregar Nuevo";
            btn_add_new.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_add_new.Click += Btn_add_new_Click;
            // 
            // titleform
            // 
            titleform.AutoSize = true;
            titleform.ImageAlign = ContentAlignment.MiddleLeft;
            titleform.Location = new Point(28, 19);
            titleform.Name = "titleform";
            titleform.Size = new Size(133, 43);
            titleform.TabIndex = 0;
            titleform.Text = "label2";
            // 
            // label2
            // 
            label2.Location = new Point(12, 415);
            label2.Name = "label2";
            label2.Size = new Size(160, 53);
            label2.TabIndex = 7;
            label2.Text = "Haga double-click en la fila para seleccionarla ";
            // 
            // FrmSeleccion
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(499, 478);
            Controls.Add(label2);
            Controls.Add(panel1);
            Controls.Add(Numero_reg);
            Controls.Add(groupBox1);
            Controls.Add(label1);
            Controls.Add(bot_buscar);
            Controls.Add(txt_buscar);
            Controls.Add(Grid_Items);
            Name = "FrmSeleccion";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Seleccion de Registros:";
            Load += Seleccion_Load;
            ((System.ComponentModel.ISupportInitialize)Grid_Items).EndInit();
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            panel1.ResumeLayout(false);
            panel1.PerformLayout();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView Grid_Items;
        private TextBox txt_buscar;
        private Button bot_buscar;
        private Label label1;
        private GroupBox groupBox1;
        private RadioButton ra_description;
        private RadioButton ra_id;
        private Label Numero_reg;
        private Panel panel1;
        private Label titleform;
        private Label label2;
        private Button btn_add_new;
        private Button btn_delete_row;
    }
}


