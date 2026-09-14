namespace Ritrama2025.Forms
{
    partial class FrmLogin
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
            txt_username = new Sunny.UI.UITextBox();
            txt_password = new Sunny.UI.UITextBox();
            btn_ojo = new Sunny.UI.UIButton();
            btn_login = new Sunny.UI.UIButton();
            btn_salir = new Sunny.UI.UIButton();
            lbl_intentos = new Sunny.UI.UILabel();
            lbl_empresa = new Sunny.UI.UILabel();
            lbl_title = new Sunny.UI.UILabel();
            lbl_usuario = new Sunny.UI.UILabel();
            lbl_contrasena = new Sunny.UI.UILabel();
            SuspendLayout();
            //
            // lbl_empresa
            //
            lbl_empresa.Font = new Font("Segoe UI", 28F, FontStyle.Bold);
            lbl_empresa.ForeColor = Color.FromArgb(70, 140, 25);
            lbl_empresa.Location = new Point(0, 12);
            lbl_empresa.Name = "lbl_empresa";
            lbl_empresa.Size = new Size(380, 52);
            lbl_empresa.TabIndex = 0;
            lbl_empresa.Text = "RITRAMA";
            lbl_empresa.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lbl_title
            //
            lbl_title.Font = new Font("Segoe UI", 12F, FontStyle.Bold);
            lbl_title.ForeColor = Color.FromArgb(70, 140, 25);
            lbl_title.Location = new Point(0, 64);
            lbl_title.Name = "lbl_title";
            lbl_title.Size = new Size(380, 26);
            lbl_title.TabIndex = 1;
            lbl_title.Text = "INICIO DE SESIÓN";
            lbl_title.TextAlign = ContentAlignment.MiddleCenter;
            //
            // lbl_usuario
            //
            lbl_usuario.AutoSize = true;
            lbl_usuario.Font = new Font("Segoe UI", 10F);
            lbl_usuario.ForeColor = Color.FromArgb(80, 80, 90);
            lbl_usuario.Location = new Point(40, 106);
            lbl_usuario.Name = "lbl_usuario";
            lbl_usuario.Size = new Size(60, 19);
            lbl_usuario.TabIndex = 2;
            lbl_usuario.Text = "Usuario:";
            //
            // txt_username
            //
            txt_username.Font = new Font("Segoe UI", 11F);
            txt_username.Location = new Point(40, 131);
            txt_username.Margin = new Padding(4, 5, 4, 5);
            txt_username.MinimumSize = new Size(1, 16);
            txt_username.Name = "txt_username";
            txt_username.Padding = new Padding(7);
            txt_username.Size = new Size(300, 36);
            txt_username.Style = Sunny.UI.UIStyle.Green;
            txt_username.TabIndex = 3;
            txt_username.TextAlignment = ContentAlignment.MiddleLeft;
            txt_username.KeyPress += txt_username_KeyPress;
            //
            // lbl_contrasena
            //
            lbl_contrasena.AutoSize = true;
            lbl_contrasena.Font = new Font("Segoe UI", 10F);
            lbl_contrasena.ForeColor = Color.FromArgb(80, 80, 90);
            lbl_contrasena.Location = new Point(40, 181);
            lbl_contrasena.Name = "lbl_contrasena";
            lbl_contrasena.Size = new Size(87, 19);
            lbl_contrasena.TabIndex = 4;
            lbl_contrasena.Text = "Contraseña:";
            //
            // txt_password
            //
            txt_password.Font = new Font("Segoe UI", 11F);
            txt_password.Location = new Point(40, 206);
            txt_password.Margin = new Padding(4, 5, 4, 5);
            txt_password.MinimumSize = new Size(1, 16);
            txt_password.Name = "txt_password";
            txt_password.Padding = new Padding(7);
            txt_password.PasswordChar = '*';
            txt_password.Size = new Size(258, 36);
            txt_password.Style = Sunny.UI.UIStyle.Green;
            txt_password.TabIndex = 5;
            txt_password.TextAlignment = ContentAlignment.MiddleLeft;
            txt_password.KeyPress += txt_password_KeyPress;
            //
            // btn_ojo
            //
            btn_ojo.Cursor = Cursors.Hand;
            btn_ojo.FillColor = Color.FromArgb(108, 117, 125);
            btn_ojo.FillColor2 = Color.FromArgb(108, 117, 125);
            btn_ojo.Font = new Font("Segoe UI Emoji", 12F);
            btn_ojo.ForeColor = Color.White;
            btn_ojo.Location = new Point(304, 206);
            btn_ojo.MinimumSize = new Size(1, 1);
            btn_ojo.Name = "btn_ojo";
            btn_ojo.Size = new Size(36, 36);
            btn_ojo.Style = Sunny.UI.UIStyle.Gray;
            btn_ojo.TabIndex = 6;
            btn_ojo.TabStop = false;
            btn_ojo.Text = "👁";
            btn_ojo.Click += btn_ojo_Click;
            //
            // lbl_intentos
            //
            lbl_intentos.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lbl_intentos.ForeColor = Color.FromArgb(110, 190, 40);
            lbl_intentos.Location = new Point(40, 251);
            lbl_intentos.Name = "lbl_intentos";
            lbl_intentos.Size = new Size(300, 23);
            lbl_intentos.TabIndex = 7;
            lbl_intentos.Text = "Intentos restantes: 5";
            lbl_intentos.TextAlign = ContentAlignment.MiddleCenter;
            //
            // btn_login
            //
            btn_login.Cursor = Cursors.Hand;
            btn_login.FillColor = Color.FromArgb(110, 190, 40);
            btn_login.FillColor2 = Color.FromArgb(110, 190, 40);
            btn_login.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn_login.ForeColor = Color.White;
            btn_login.Location = new Point(40, 286);
            btn_login.MinimumSize = new Size(1, 1);
            btn_login.Name = "btn_login";
            btn_login.Size = new Size(145, 40);
            btn_login.Style = Sunny.UI.UIStyle.Green;
            btn_login.TabIndex = 8;
            btn_login.Text = "Iniciar Sesión";
            btn_login.Click += btn_login_Click;
            //
            // btn_salir
            //
            btn_salir.Cursor = Cursors.Hand;
            btn_salir.FillColor = Color.FromArgb(108, 117, 125);
            btn_salir.FillColor2 = Color.FromArgb(108, 117, 125);
            btn_salir.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            btn_salir.ForeColor = Color.White;
            btn_salir.Location = new Point(195, 286);
            btn_salir.MinimumSize = new Size(1, 1);
            btn_salir.Name = "btn_salir";
            btn_salir.Size = new Size(145, 40);
            btn_salir.Style = Sunny.UI.UIStyle.Gray;
            btn_salir.TabIndex = 9;
            btn_salir.Text = "Salir";
            btn_salir.Click += btn_salir_Click;
            //
            // FrmLogin
            //
            AutoScaleMode = AutoScaleMode.None;
            ClientSize = new Size(380, 348);
            Controls.Add(lbl_empresa);
            Controls.Add(lbl_title);
            Controls.Add(lbl_usuario);
            Controls.Add(txt_username);
            Controls.Add(lbl_contrasena);
            Controls.Add(txt_password);
            Controls.Add(btn_ojo);
            Controls.Add(lbl_intentos);
            Controls.Add(btn_login);
            Controls.Add(btn_salir);
            FormBorderStyle = FormBorderStyle.FixedDialog;
            MaximizeBox = false;
            MinimizeBox = false;
            Name = "FrmLogin";
            ShowInTaskbar = false;
            StartPosition = FormStartPosition.CenterScreen;
            Style = Sunny.UI.UIStyle.Green;
            Text = "Ritrama - Login";
            TitleColor = Color.FromArgb(110, 190, 40);
            TitleForeColor = Color.White;
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Sunny.UI.UITextBox txt_username;
        private Sunny.UI.UITextBox txt_password;
        private Sunny.UI.UIButton btn_ojo;
        private Sunny.UI.UIButton btn_login;
        private Sunny.UI.UIButton btn_salir;
        private Sunny.UI.UILabel lbl_intentos;
        private Sunny.UI.UILabel lbl_empresa;
        private Sunny.UI.UILabel lbl_title;
        private Sunny.UI.UILabel lbl_usuario;
        private Sunny.UI.UILabel lbl_contrasena;
    }
}
