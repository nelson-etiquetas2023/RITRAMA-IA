namespace Ritrama2025.Forms.Otros
{
    partial class Frm_oneparameter
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
            txt_buscar = new TextBox();
            label1 = new Label();
            btn_aceptar = new Button();
            SuspendLayout();
            // 
            // txt_buscar
            // 
            txt_buscar.Location = new Point(12, 36);
            txt_buscar.Name = "txt_buscar";
            txt_buscar.Size = new Size(100, 25);
            txt_buscar.TabIndex = 0;
            txt_buscar.KeyPress += txt_buscar_KeyPress;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 14);
            label1.Name = "label1";
            label1.Size = new Size(96, 18);
            label1.TabIndex = 1;
            label1.Text = "Valor a Buscar:";
            // 
            // btn_aceptar
            // 
            btn_aceptar.Location = new Point(12, 65);
            btn_aceptar.Name = "btn_aceptar";
            btn_aceptar.Size = new Size(100, 26);
            btn_aceptar.TabIndex = 2;
            btn_aceptar.Text = "Aceptar";
            btn_aceptar.Click += Btn_aceptar_Click;
            // 
            // Frm_oneparameter
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(125, 100);
            Controls.Add(btn_aceptar);
            Controls.Add(label1);
            Controls.Add(txt_buscar);
            FormBorderStyle = FormBorderStyle.FixedToolWindow;
            Name = "Frm_oneparameter";
            Text = "Buscar:";
            Load += Frm_oneparameter_Load;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txt_buscar;
        private Label label1;
        private Button btn_aceptar;
    }
}

