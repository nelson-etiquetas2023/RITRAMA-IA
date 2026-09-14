namespace Ritrama2025.Forms
{
    partial class FrmUsuarios
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
            toolStripAcciones = new ToolStrip();
            tsbNuevo = new ToolStripButton();
            tsbGuardar = new ToolStripButton();
            tsbEliminar = new ToolStripButton();
            tsbResetPassword = new ToolStripButton();
            txtBuscar = new ToolStripTextBox();
            tsbBuscar = new ToolStripButton();
            panelListado = new Panel();
            gridUsuarios = new DataGridView();
            panelCaptura = new Panel();
            lblUsername = new Label();
            txtUsername = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            chkActivo = new CheckBox();
            lblRoles = new Label();
            clbRoles = new CheckedListBox();
            toolStripAcciones.SuspendLayout();
            panelListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUsuarios).BeginInit();
            panelCaptura.SuspendLayout();
            SuspendLayout();
            //
            // toolStripAcciones
            //
            toolStripAcciones.AutoSize = false;
            toolStripAcciones.ImageScalingSize = new Size(18, 18);
            toolStripAcciones.Items.AddRange(new ToolStripItem[] { tsbNuevo, tsbGuardar, tsbEliminar, tsbResetPassword, txtBuscar, tsbBuscar });
            toolStripAcciones.Location = new Point(0, 0);
            toolStripAcciones.Name = "toolStripAcciones";
            toolStripAcciones.Size = new Size(1150, 40);
            toolStripAcciones.TabIndex = 0;
            //
            // tsbNuevo
            //
            tsbNuevo.AutoSize = false;
            tsbNuevo.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbNuevo.Name = "tsbNuevo";
            tsbNuevo.Size = new Size(70, 32);
            tsbNuevo.Text = "Nuevo";
            tsbNuevo.Click += TsbNuevo_Click;
            //
            // tsbGuardar
            //
            tsbGuardar.AutoSize = false;
            tsbGuardar.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbGuardar.Enabled = false;
            tsbGuardar.Name = "tsbGuardar";
            tsbGuardar.Size = new Size(70, 32);
            tsbGuardar.Text = "Guardar";
            tsbGuardar.Click += TsbGuardar_Click;
            //
            // tsbEliminar
            //
            tsbEliminar.AutoSize = false;
            tsbEliminar.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbEliminar.Enabled = false;
            tsbEliminar.Name = "tsbEliminar";
            tsbEliminar.Size = new Size(70, 32);
            tsbEliminar.Text = "Eliminar";
            tsbEliminar.Click += TsbEliminar_Click;
            //
            // tsbResetPassword
            //
            tsbResetPassword.AutoSize = false;
            tsbResetPassword.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbResetPassword.Enabled = false;
            tsbResetPassword.Name = "tsbResetPassword";
            tsbResetPassword.Size = new Size(120, 32);
            tsbResetPassword.Text = "Reset Password";
            tsbResetPassword.Click += TsbResetPassword_Click;
            //
            // txtBuscar
            //
            txtBuscar.AutoSize = false;
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(220, 30);
            txtBuscar.ToolTipText = "Buscar por nombre o usuario";
            txtBuscar.KeyDown += TxtBuscar_KeyDown;
            //
            // tsbBuscar
            //
            tsbBuscar.AutoSize = false;
            tsbBuscar.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbBuscar.Name = "tsbBuscar";
            tsbBuscar.Size = new Size(70, 32);
            tsbBuscar.Text = "Buscar";
            tsbBuscar.Click += TsbBuscar_Click;
            //
            // panelListado
            //
            panelListado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelListado.Controls.Add(gridUsuarios);
            panelListado.Location = new Point(10, 50);
            panelListado.Name = "panelListado";
            panelListado.Size = new Size(1130, 300);
            panelListado.TabIndex = 1;
            //
            // gridUsuarios
            //
            gridUsuarios.AllowUserToAddRows = false;
            gridUsuarios.AllowUserToDeleteRows = false;
            gridUsuarios.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridUsuarios.AutoGenerateColumns = false;
            gridUsuarios.BackgroundColor = Color.White;
            gridUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridUsuarios.Location = new Point(0, 0);
            gridUsuarios.MultiSelect = false;
            gridUsuarios.Name = "gridUsuarios";
            gridUsuarios.ReadOnly = true;
            gridUsuarios.RowHeadersVisible = false;
            gridUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridUsuarios.Size = new Size(1130, 300);
            gridUsuarios.TabIndex = 0;
            gridUsuarios.SelectionChanged += GridUsuarios_SelectionChanged;
            //
            // panelCaptura
            //
            panelCaptura.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelCaptura.Controls.Add(lblUsername);
            panelCaptura.Controls.Add(txtUsername);
            panelCaptura.Controls.Add(lblNombre);
            panelCaptura.Controls.Add(txtNombre);
            panelCaptura.Controls.Add(lblEmail);
            panelCaptura.Controls.Add(txtEmail);
            panelCaptura.Controls.Add(chkActivo);
            panelCaptura.Controls.Add(lblRoles);
            panelCaptura.Controls.Add(clbRoles);
            panelCaptura.Location = new Point(10, 360);
            panelCaptura.Name = "panelCaptura";
            panelCaptura.Size = new Size(1130, 180);
            panelCaptura.TabIndex = 2;
            //
            // lblUsername
            //
            lblUsername.AutoSize = true;
            lblUsername.Location = new Point(15, 12);
            lblUsername.Name = "lblUsername";
            lblUsername.Size = new Size(55, 16);
            lblUsername.TabIndex = 0;
            lblUsername.Text = "Usuario";
            //
            // txtUsername
            //
            txtUsername.Location = new Point(15, 32);
            txtUsername.Name = "txtUsername";
            txtUsername.Size = new Size(200, 24);
            txtUsername.TabIndex = 1;
            //
            // lblNombre
            //
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(240, 12);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(97, 16);
            lblNombre.TabIndex = 2;
            lblNombre.Text = "Nombre Completo";
            //
            // txtNombre
            //
            txtNombre.Location = new Point(240, 32);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(300, 24);
            txtNombre.TabIndex = 3;
            //
            // lblEmail
            //
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(565, 12);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(36, 16);
            lblEmail.TabIndex = 4;
            lblEmail.Text = "Email";
            //
            // txtEmail
            //
            txtEmail.Location = new Point(565, 32);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(250, 24);
            txtEmail.TabIndex = 5;
            //
            // chkActivo
            //
            chkActivo.AutoSize = true;
            chkActivo.Checked = true;
            chkActivo.CheckState = CheckState.Checked;
            chkActivo.Location = new Point(840, 34);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(59, 20);
            chkActivo.TabIndex = 6;
            chkActivo.Text = "Activo";
            //
            // lblRoles
            //
            lblRoles.AutoSize = true;
            lblRoles.Location = new Point(15, 68);
            lblRoles.Name = "lblRoles";
            lblRoles.Size = new Size(38, 16);
            lblRoles.TabIndex = 7;
            lblRoles.Text = "Roles";
            //
            // clbRoles
            //
            clbRoles.FormattingEnabled = true;
            clbRoles.Location = new Point(15, 88);
            clbRoles.Name = "clbRoles";
            clbRoles.Size = new Size(300, 84);
            clbRoles.TabIndex = 8;
            //
            // FrmUsuarios
            //
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1150, 552);
            MinimumSize = new Size(1150, 552);
            Controls.Add(panelCaptura);
            Controls.Add(panelListado);
            Controls.Add(toolStripAcciones);
            Name = "FrmUsuarios";
            Text = "Gestión de Usuarios";
            toolStripAcciones.ResumeLayout(false);
            toolStripAcciones.PerformLayout();
            panelListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridUsuarios).EndInit();
            panelCaptura.ResumeLayout(false);
            panelCaptura.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStripAcciones;
        private ToolStripButton tsbNuevo;
        private ToolStripButton tsbGuardar;
        private ToolStripButton tsbEliminar;
        private ToolStripButton tsbResetPassword;
        private ToolStripTextBox txtBuscar;
        private ToolStripButton tsbBuscar;
        private Panel panelListado;
        private DataGridView gridUsuarios;
        private Panel panelCaptura;
        private Label lblUsername;
        private TextBox txtUsername;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblEmail;
        private TextBox txtEmail;
        private CheckBox chkActivo;
        private Label lblRoles;
        private CheckedListBox clbRoles;
    }
}
