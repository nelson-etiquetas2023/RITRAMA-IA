namespace Ritrama2025.Forms.Otros
{
    partial class Frm_AddNew
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
            label1 = new Label();
            txt_name = new TextBox();
            btn_save = new Button();
            btn_cancel = new Button();
            Titulo = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 79);
            label1.Name = "label1";
            label1.Size = new Size(57, 15);
            label1.TabIndex = 0;
            label1.Text = "Nombre :";
            // 
            // txt_name
            // 
            txt_name.Location = new Point(12, 97);
            txt_name.Name = "txt_name";
            txt_name.Size = new Size(413, 23);
            txt_name.TabIndex = 1;
            // 
            // btn_save
            // 
            btn_save.Location = new Point(12, 126);
            btn_save.Name = "btn_save";
            btn_save.Size = new Size(75, 23);
            btn_save.TabIndex = 2;
            btn_save.Text = "Guardar";
            btn_save.Click += Btn_save_Click;
            // 
            // btn_cancel
            // 
            btn_cancel.Location = new Point(93, 126);
            btn_cancel.Name = "btn_cancel";
            btn_cancel.Size = new Size(75, 23);
            btn_cancel.TabIndex = 3;
            btn_cancel.Text = "Cancelar";
            btn_cancel.Click += Btn_cancel_Click;
            // 
            // Titulo
            // 
            Titulo.AutoSize = true;
            Titulo.Location = new Point(12, 24);
            Titulo.Name = "Titulo";
            Titulo.Size = new Size(105, 32);
            Titulo.TabIndex = 4;
            Titulo.Text = "no titulo";
            // 
            // Frm_AddNew
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(437, 175);
            Controls.Add(Titulo);
            Controls.Add(btn_cancel);
            Controls.Add(btn_save);
            Controls.Add(txt_name);
            Controls.Add(label1);
            Name = "Frm_AddNew";
            Text = "Nuevo Elemento";
            Load += Frm_AddNew_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txt_name;
        private Button btn_save;
        private Button btn_cancel;
        private Label Titulo;
    }
}

