namespace Ritrama2025.Forms
{
    partial class FrmCambiarContrasena
    {
        private System.ComponentModel.IContainer components = null;

        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            lbl_title = new Sunny.UI.UILabel();
            lbl_nueva = new Sunny.UI.UILabel();
            txt_nueva = new Sunny.UI.UITextBox();
            lbl_confirmar = new Sunny.UI.UILabel();
            txt_confirmar = new Sunny.UI.UITextBox();
            btn_cambiar = new Sunny.UI.UIButton();
            btn_cancelar = new Sunny.UI.UIButton();
            lbl_info = new Sunny.UI.UILabel();
            SuspendLayout();
            // 
            // lbl_title
            // 
            lbl_title.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            lbl_title.ForeColor = Color.FromArgb(38, 38, 44);
            lbl_title.Location = new Point(0, 15);
            lbl_title.Name = "lbl_title";
            lbl_title.Size = new Size(380, 30);
            lbl_title.TabIndex = 0;
            lbl_title.Text = "CAMBIAR CONTRASEÑA";
            lbl_title.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_info
            // 
            lbl_info.Font = new Font("Segoe UI", 9F);
            lbl_info.ForeColor = Color.FromArgb(108, 117, 125);
            lbl_info.Location = new Point(30, 50);
            lbl_info.Name = "lbl_info";
            lbl_info.Size = new Size(320, 20);
            lbl_info.TabIndex = 1;
            lbl_info.Text = "Debe cambiar su contraseña para continuar.";
            lbl_info.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // lbl_nueva
            // 
            lbl_nueva.AutoSize = true;
            lbl_nueva.Font = new Font("Segoe UI", 10F);
            lbl_nueva.ForeColor = Color.FromArgb(80, 80, 90);
            lbl_nueva.Location = new Point(30, 80);
            lbl_nueva.Name = "lbl_nueva";
            lbl_nueva.Size = new Size(113, 19);
            lbl_nueva.TabIndex = 2;
            lbl_nueva.Text = "Nueva contraseña:";
            // 
            // txt_nueva
            // 
            txt_nueva.Font = new Font("Segoe UI", 11F);
            txt_nueva.Location = new Point(30, 105);
            txt_nueva.Margin = new Padding(4, 5, 4, 5);
            txt_nueva.MinimumSize = new Size(1, 16);
            txt_nueva.Name = "txt_nueva";
            txt_nueva.Padding = new Padding(7);
            txt_nueva.PasswordChar = '*';
            txt_nueva.Size = new Size(320, 36);
            txt_nueva.TabIndex = 3;
            txt_nueva.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // lbl_confirmar
            // 
            lbl_confirmar.AutoSize = true;
            lbl_confirmar.Font = new Font("Segoe UI", 10F);
            lbl_confirmar.ForeColor = Color.FromArgb(80, 80, 90);
            lbl_confirmar.Location = new Point(30, 150);
            lbl_confirmar.Name = "lbl_confirmar";
            lbl_confirmar.Size = new Size(140, 19);
            lbl_confirmar.TabIndex = 4;
            lbl_confirmar.Text = "Confirmar contraseña:";
            // 
            // txt_confirmar
            // 
            txt_confirmar.Font = new Font("Segoe UI", 11F);
            txt_confirmar.Location = new Point(30, 175);
            txt_confirmar.Margin = new Padding(4, 5, 4, 5);
            txt_confirmar.MinimumSize = new Size(1, 16);
            txt_confirmar.Name = "txt_confirmar";
            txt_confirmar.Padding = new Padding(7);
            txt_confirmar.PasswordChar = '*';
            txt_confirmar.Size = new Size(320, 36);
            txt_confirmar.TabIndex = 5;
            txt_confirmar.TextAlignment = ContentAlignment.MiddleLeft;
            // 
            // btn_cambiar
            // 
            btn_cambiar.Cursor = Cursors.Hand;
            btn_cambiar.FillColor = Color.FromArgb(40, 167, 69);
            btn_cambiar.FillColor2 = Color.FromArgb(40, 167, 69);
            btn_cambiar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn_cambiar.ForeColor = Color.White;
            btn_cambiar.Location = new Point(30, 230);
            btn_cambiar.MinimumSize = new Size(1, 1);
            btn_cambiar.Name = "btn_cambiar";
            btn_cambiar.Size = new Size(155, 40);
            btn_cambiar.TabIndex = 6;
            btn_cambiar.Text = "Cambiar Contraseña";
            btn_cambiar.Click += btn_cambiar_Click;
            // 
            // btn_cancelar
            // 
            btn_cancelar.Cursor = Cursors.Hand;
            btn_cancelar.FillColor = Color.FromArgb(108, 117, 125);
            btn_cancelar.FillColor2 = Color.FromArgb(108, 117, 125);
            btn_cancelar.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn_cancelar.ForeColor = Color.White;
            btn_cancelar.Location = new Point(195, 230);
            btn_cancelar.MinimumSize = new Size(1, 1);
            btn_cancelar.Name = "btn_cancelar";
            btn_cancelar.Size = new Size(155, 40);
            btn_cancelar.TabIndex = 7;
            btn_cancelar.Text = "Cancelar";
            btn_cancelar.Click += btn_cancelar_Click;
            // 
            // FrmCambiarContrasena
            // 
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(380, 290);
            Controls.Add(lbl_title);
            Controls.Add(lbl_info);
            Controls.Add(lbl_nueva);
            Controls.Add(txt_nueva);
            Controls.Add(lbl_confirmar);
            Controls.Add(txt_confirmar);
            Controls.Add(btn_cambiar);
            Controls.Add(btn_cancelar);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmCambiarContrasena";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Text = "Ritrama - Cambiar Contraseña";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Sunny.UI.UILabel lbl_title;
        private Sunny.UI.UILabel lbl_info;
        private Sunny.UI.UILabel lbl_nueva;
        private Sunny.UI.UITextBox txt_nueva;
        private Sunny.UI.UILabel lbl_confirmar;
        private Sunny.UI.UITextBox txt_confirmar;
        private Sunny.UI.UIButton btn_cambiar;
        private Sunny.UI.UIButton btn_cancelar;
    }
}
