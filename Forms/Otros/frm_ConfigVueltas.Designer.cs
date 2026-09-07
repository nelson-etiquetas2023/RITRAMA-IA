namespace Ritrama2025.Forms.Otros
{
    partial class Frm_ConfigVueltas
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(Frm_ConfigVueltas));
            Grid_ConfigVueltas = new DataGridView();
            btn_saveChanges = new Button();
            txt_Total_Utilizado = new TextBox();
            label1 = new Label();
            ((System.ComponentModel.ISupportInitialize)Grid_ConfigVueltas).BeginInit();
            SuspendLayout();
            // 
            // Grid_ConfigVueltas
            // 
            Grid_ConfigVueltas.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            Grid_ConfigVueltas.Location = new Point(12, 12);
            Grid_ConfigVueltas.Name = "Grid_ConfigVueltas";
            Grid_ConfigVueltas.Size = new Size(346, 429);
            Grid_ConfigVueltas.TabIndex = 0;
            Grid_ConfigVueltas.CellEndEdit += Grid_ConfigVueltas_CellEndEdit;
            Grid_ConfigVueltas.CellValueChanged += Grid_ConfigVueltas_CellValueChanged;
            // 
            // btn_saveChanges
            // 
            btn_saveChanges.Image = (Image)resources.GetObject("btn_saveChanges.Image");
            btn_saveChanges.Location = new Point(104, 507);
            btn_saveChanges.Name = "btn_saveChanges";
            btn_saveChanges.Size = new Size(147, 44);
            btn_saveChanges.TabIndex = 1;
            btn_saveChanges.Text = "Guardar Cambios";
            btn_saveChanges.TextImageRelation = TextImageRelation.ImageBeforeText;
            btn_saveChanges.Click += Btn_saveChanges_Click;
            // 
            // txt_Total_Utilizado
            // 
            txt_Total_Utilizado.Location = new Point(121, 447);
            txt_Total_Utilizado.Name = "txt_Total_Utilizado";
            txt_Total_Utilizado.ReadOnly = true;
            txt_Total_Utilizado.Size = new Size(130, 23);
            txt_Total_Utilizado.TabIndex = 2;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 450);
            label1.Name = "label1";
            label1.Size = new Size(103, 15);
            label1.TabIndex = 3;
            label1.Text = "Total Consumido :";
            // 
            // Frm_ConfigVueltas
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(369, 574);
            Controls.Add(label1);
            Controls.Add(txt_Total_Utilizado);
            Controls.Add(btn_saveChanges);
            Controls.Add(Grid_ConfigVueltas);
            Name = "Frm_ConfigVueltas";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Configurar Vueltas";
            Load += Frm_ConfigVueltas_Load;
            ((System.ComponentModel.ISupportInitialize)Grid_ConfigVueltas).EndInit();
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private DataGridView Grid_ConfigVueltas;
        private Button btn_saveChanges;
        private TextBox txt_Total_Utilizado;
        private Label label1;
    }
}

