namespace Ritrama2025.Forms
{
    partial class FrmPickingDespacho
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FrmPickingDespacho));
            groupBox1 = new GroupBox();
            btn_buscar = new Button();
            label2 = new Label();
            radioButton5 = new RadioButton();
            rad_code_unique = new RadioButton();
            txt_codigo = new TextBox();
            label1 = new Label();
            groupBox2 = new GroupBox();
            radioButton4 = new RadioButton();
            radioButton3 = new RadioButton();
            radioButton2 = new RadioButton();
            RA_CORTADO = new RadioButton();
            tabControl1 = new TabControl();
            tabPage1 = new TabPage();
            grid_detallerc = new DataGridView();
            tabPage2 = new TabPage();
            grid_renglones = new DataGridView();
            BOT_DESPACHAR = new Button();
            BOT_LEER_TXT = new Button();
            BOT_ELIMINAR_RENGLON = new Button();
            BOT_CARGAR_RESERVA = new Button();
            groupBox1.SuspendLayout();
            groupBox2.SuspendLayout();
            tabControl1.SuspendLayout();
            tabPage1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid_detallerc).BeginInit();
            tabPage2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)grid_renglones).BeginInit();
            SuspendLayout();
            // 
            // groupBox1
            // 
            groupBox1.Controls.Add(btn_buscar);
            groupBox1.Controls.Add(label2);
            groupBox1.Controls.Add(radioButton5);
            groupBox1.Controls.Add(rad_code_unique);
            groupBox1.Controls.Add(txt_codigo);
            groupBox1.Controls.Add(label1);
            groupBox1.Location = new Point(12, 11);
            groupBox1.Name = "groupBox1";
            groupBox1.Size = new Size(597, 125);
            groupBox1.TabIndex = 0;
            groupBox1.TabStop = false;
            groupBox1.Text = "Informacion del Picking-List";
            // 
            // btn_buscar
            // 
            btn_buscar.Image = (Image)resources.GetObject("btn_buscar.Image");
            btn_buscar.Location = new Point(227, 55);
            btn_buscar.Name = "btn_buscar";
            btn_buscar.Size = new Size(143, 55);
            btn_buscar.TabIndex = 1;
            btn_buscar.Text = "Buscar";
            btn_buscar.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_buscar.Click += Btn_buscar_Click;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(430, 51);
            label2.Name = "label2";
            label2.Size = new Size(86, 15);
            label2.TabIndex = 5;
            label2.Text = "Busqueda Por:";
            // 
            // radioButton5
            // 
            radioButton5.AutoSize = true;
            radioButton5.Location = new Point(430, 98);
            radioButton5.Name = "radioButton5";
            radioButton5.Size = new Size(140, 19);
            radioButton5.TabIndex = 4;
            radioButton5.Text = "Codigo Personalizado";
            // 
            // rad_code_unique
            // 
            rad_code_unique.AutoSize = true;
            rad_code_unique.Checked = true;
            rad_code_unique.Location = new Point(430, 73);
            rad_code_unique.Name = "rad_code_unique";
            rad_code_unique.Size = new Size(98, 19);
            rad_code_unique.TabIndex = 3;
            rad_code_unique.TabStop = true;
            rad_code_unique.Text = "Codigo Unico";
            // 
            // txt_codigo
            // 
            txt_codigo.AcceptsReturn = true;
            txt_codigo.Location = new Point(13, 81);
            txt_codigo.Name = "txt_codigo";
            txt_codigo.Size = new Size(208, 29);
            txt_codigo.TabIndex = 0;
            txt_codigo.KeyDown += txt_codigo_KeyDown;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 56);
            label1.Name = "label1";
            label1.Size = new Size(73, 21);
            label1.TabIndex = 0;
            label1.Text = "Codigo :";
            // 
            // groupBox2
            // 
            groupBox2.Controls.Add(radioButton4);
            groupBox2.Controls.Add(radioButton3);
            groupBox2.Controls.Add(radioButton2);
            groupBox2.Controls.Add(RA_CORTADO);
            groupBox2.Location = new Point(615, 11);
            groupBox2.Name = "groupBox2";
            groupBox2.Size = new Size(159, 125);
            groupBox2.TabIndex = 1;
            groupBox2.TabStop = false;
            groupBox2.Text = "Categoria de Producto";
            // 
            // radioButton4
            // 
            radioButton4.AutoSize = true;
            radioButton4.Location = new Point(19, 97);
            radioButton4.Name = "radioButton4";
            radioButton4.Size = new Size(61, 19);
            radioButton4.TabIndex = 3;
            radioButton4.TabStop = true;
            radioButton4.Text = "Master";
            // 
            // radioButton3
            // 
            radioButton3.AutoSize = true;
            radioButton3.Location = new Point(19, 73);
            radioButton3.Name = "radioButton3";
            radioButton3.Size = new Size(71, 19);
            radioButton3.TabIndex = 2;
            radioButton3.TabStop = true;
            radioButton3.Text = "Graphics";
            // 
            // radioButton2
            // 
            radioButton2.AutoSize = true;
            radioButton2.Location = new Point(19, 49);
            radioButton2.Name = "radioButton2";
            radioButton2.Size = new Size(55, 19);
            radioButton2.TabIndex = 1;
            radioButton2.TabStop = true;
            radioButton2.Text = "Hojas";
            // 
            // RA_CORTADO
            // 
            RA_CORTADO.AutoSize = true;
            RA_CORTADO.Checked = true;
            RA_CORTADO.Location = new Point(19, 26);
            RA_CORTADO.Name = "RA_CORTADO";
            RA_CORTADO.Size = new Size(98, 19);
            RA_CORTADO.TabIndex = 0;
            RA_CORTADO.TabStop = true;
            RA_CORTADO.Text = "Rollo Cortado";
            // 
            // tabControl1
            // 
            tabControl1.Controls.Add(tabPage1);
            tabControl1.Controls.Add(tabPage2);
            tabControl1.Location = new Point(0, 141);
            tabControl1.Name = "tabControl1";
            tabControl1.SelectedIndex = 0;
            tabControl1.Size = new Size(890, 420);
            tabControl1.TabIndex = 2;
            // 
            // tabPage1
            // 
            tabPage1.Controls.Add(grid_detallerc);
            tabPage1.Location = new Point(4, 24);
            tabPage1.Name = "tabPage1";
            tabPage1.Padding = new Padding(3);
            tabPage1.Size = new Size(882, 392);
            tabPage1.TabIndex = 0;
            tabPage1.Text = "Rollos Cortados";
            // 
            // grid_detallerc
            // 
            grid_detallerc.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid_detallerc.Location = new Point(6, 8);
            grid_detallerc.Name = "grid_detallerc";
            grid_detallerc.ReadOnly = true;
            grid_detallerc.RowHeadersWidth = 38;
            grid_detallerc.Size = new Size(873, 378);
            grid_detallerc.TabIndex = 0;
            // 
            // tabPage2
            // 
            tabPage2.Controls.Add(grid_renglones);
            tabPage2.Location = new Point(4, 24);
            tabPage2.Name = "tabPage2";
            tabPage2.Padding = new Padding(3);
            tabPage2.Size = new Size(882, 392);
            tabPage2.TabIndex = 1;
            tabPage2.Text = "Items Groups";
            // 
            // grid_renglones
            // 
            grid_renglones.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            grid_renglones.Location = new Point(6, 5);
            grid_renglones.Name = "grid_renglones";
            grid_renglones.RowHeadersWidth = 45;
            grid_renglones.Size = new Size(870, 384);
            grid_renglones.TabIndex = 0;
            // 
            // BOT_DESPACHAR
            // 
            BOT_DESPACHAR.Image = Properties.Resources.DATA_DOWNLOAD48;
            BOT_DESPACHAR.Location = new Point(892, 11);
            BOT_DESPACHAR.Name = "BOT_DESPACHAR";
            BOT_DESPACHAR.Size = new Size(160, 56);
            BOT_DESPACHAR.TabIndex = 3;
            BOT_DESPACHAR.Text = "Despachar";
            BOT_DESPACHAR.TextImageRelation = TextImageRelation.ImageBeforeText;
            BOT_DESPACHAR.Click += BOT_DESPACHAR_Click;
            // 
            // BOT_LEER_TXT
            // 
            BOT_LEER_TXT.Image = Properties.Resources.DATA_READ48;
            BOT_LEER_TXT.Location = new Point(892, 71);
            BOT_LEER_TXT.Name = "BOT_LEER_TXT";
            BOT_LEER_TXT.Size = new Size(160, 56);
            BOT_LEER_TXT.TabIndex = 4;
            BOT_LEER_TXT.Text = "Leer Data TXT";
            BOT_LEER_TXT.TextImageRelation = TextImageRelation.ImageBeforeText;
            BOT_LEER_TXT.Click += Button2_Click;
            // 
            // BOT_ELIMINAR_RENGLON
            // 
            BOT_ELIMINAR_RENGLON.Image = Properties.Resources.ROWDELETE48;
            BOT_ELIMINAR_RENGLON.Location = new Point(892, 193);
            BOT_ELIMINAR_RENGLON.Name = "BOT_ELIMINAR_RENGLON";
            BOT_ELIMINAR_RENGLON.Size = new Size(160, 56);
            BOT_ELIMINAR_RENGLON.TabIndex = 5;
            BOT_ELIMINAR_RENGLON.Text = "Borrar Renglon";
            BOT_ELIMINAR_RENGLON.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // BOT_CARGAR_RESERVA
            // 
            BOT_CARGAR_RESERVA.Enabled = false;
            BOT_CARGAR_RESERVA.Image = Properties.Resources.DATA_RESERVA48;
            BOT_CARGAR_RESERVA.Location = new Point(892, 132);
            BOT_CARGAR_RESERVA.Name = "BOT_CARGAR_RESERVA";
            BOT_CARGAR_RESERVA.Size = new Size(160, 56);
            BOT_CARGAR_RESERVA.TabIndex = 6;
            BOT_CARGAR_RESERVA.Text = "Reserva";
            BOT_CARGAR_RESERVA.TextImageRelation = TextImageRelation.ImageBeforeText;
            // 
            // FrmPickingDespacho
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1066, 573);
            Controls.Add(BOT_ELIMINAR_RENGLON);
            Controls.Add(BOT_CARGAR_RESERVA);
            Controls.Add(BOT_DESPACHAR);
            Controls.Add(BOT_LEER_TXT);
            Controls.Add(tabControl1);
            Controls.Add(groupBox2);
            Controls.Add(groupBox1);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FrmPickingDespacho";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Picking Despacho";
            Load += FrmPickingDespacho_Load;
            groupBox1.ResumeLayout(false);
            groupBox1.PerformLayout();
            groupBox2.ResumeLayout(false);
            groupBox2.PerformLayout();
            tabControl1.ResumeLayout(false);
            tabPage1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid_detallerc).EndInit();
            tabPage2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)grid_renglones).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private GroupBox groupBox1;
        private TextBox txt_codigo;
        private Label label1;
        private GroupBox groupBox2;
        private RadioButton radioButton4;
        private RadioButton radioButton3;
        private RadioButton radioButton2;
        private RadioButton RA_CORTADO;
        private TabControl tabControl1;
        private TabPage tabPage1;
        private TabPage tabPage2;
        private Button BOT_DESPACHAR;
        private Button BOT_LEER_TXT;
        private Button BOT_ELIMINAR_RENGLON;
        private Button BOT_CARGAR_RESERVA;
        private DataGridView grid_detallerc;
        private DataGridView grid_renglones;
        private RadioButton radioButton5;
        private RadioButton rad_code_unique;
        private Label label2;
        private Button btn_buscar;
    }
}

