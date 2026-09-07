namespace Ritrama2025.Forms.Otros
{
    partial class Frm_descriptionPalet
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
            txt_ContentText = new TextBox();
            btn_aceptar = new Button();
            label1 = new Label();
            SuspendLayout();
            // 
            // txt_ContentText
            // 
            txt_ContentText.Location = new Point(12, 68);
            txt_ContentText.Multiline = true;
            txt_ContentText.Name = "txt_ContentText";
            txt_ContentText.Size = new Size(367, 228);
            txt_ContentText.TabIndex = 0;
            // 
            // btn_aceptar
            // 
            btn_aceptar.Location = new Point(12, 302);
            btn_aceptar.Name = "btn_aceptar";
            btn_aceptar.Size = new Size(367, 43);
            btn_aceptar.TabIndex = 1;
            btn_aceptar.Text = "Aceptar";
            btn_aceptar.Click += Btn_aceptar_Click;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(76, 28);
            label1.Name = "label1";
            label1.Size = new Size(232, 17);
            label1.TabIndex = 2;
            label1.Text = "Descripción del Contenido de Paleta";
            // 
            // Frm_descriptionPalet
            // 
            AutoScaleDimensions = new SizeF(7F, 17F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(391, 357);
            Controls.Add(label1);
            Controls.Add(btn_aceptar);
            Controls.Add(txt_ContentText);
            FormBorderStyle = FormBorderStyle.FixedSingle;
            Name = "Frm_descriptionPalet";
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Descripcion de Paleta";
            Load += Frm_descriptionPalet_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txt_ContentText;
        private Button btn_aceptar;
        private Label label1;
    }
}

