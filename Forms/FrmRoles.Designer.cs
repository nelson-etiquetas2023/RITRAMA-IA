namespace Ritrama2025.Forms
{
    partial class FrmRoles
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
            panelListado = new Panel();
            gridRoles = new DataGridView();
            panelCaptura = new Panel();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblDescripcion = new Label();
            txtDescripcion = new TextBox();
            chkActivo = new CheckBox();
            lblPermisos = new Label();
            clbPermisos = new CheckedListBox();
            toolStripAcciones.SuspendLayout();
            panelListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridRoles).BeginInit();
            panelCaptura.SuspendLayout();
            SuspendLayout();
            //
            // toolStripAcciones
            //
            toolStripAcciones.AutoSize = false;
            toolStripAcciones.ImageScalingSize = new Size(18, 18);
            toolStripAcciones.Items.AddRange(new ToolStripItem[] { tsbNuevo, tsbGuardar, tsbEliminar });
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
            // panelListado
            //
            panelListado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelListado.Controls.Add(gridRoles);
            panelListado.Location = new Point(10, 50);
            panelListado.Name = "panelListado";
            panelListado.Size = new Size(400, 300);
            panelListado.TabIndex = 1;
            //
            // gridRoles
            //
            gridRoles.AllowUserToAddRows = false;
            gridRoles.AllowUserToDeleteRows = false;
            gridRoles.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridRoles.AutoGenerateColumns = false;
            gridRoles.BackgroundColor = Color.White;
            gridRoles.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridRoles.Location = new Point(0, 0);
            gridRoles.MultiSelect = false;
            gridRoles.Name = "gridRoles";
            gridRoles.ReadOnly = true;
            gridRoles.RowHeadersVisible = false;
            gridRoles.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridRoles.Size = new Size(400, 300);
            gridRoles.TabIndex = 0;
            gridRoles.SelectionChanged += GridRoles_SelectionChanged;
            //
            // panelCaptura
            //
            panelCaptura.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelCaptura.Controls.Add(lblNombre);
            panelCaptura.Controls.Add(txtNombre);
            panelCaptura.Controls.Add(lblDescripcion);
            panelCaptura.Controls.Add(txtDescripcion);
            panelCaptura.Controls.Add(chkActivo);
            panelCaptura.Controls.Add(lblPermisos);
            panelCaptura.Controls.Add(clbPermisos);
            panelCaptura.Location = new Point(430, 50);
            panelCaptura.Name = "panelCaptura";
            panelCaptura.Size = new Size(710, 480);
            panelCaptura.TabIndex = 2;
            //
            // lblNombre
            //
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(15, 12);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(46, 16);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre";
            //
            // txtNombre
            //
            txtNombre.Location = new Point(15, 32);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(250, 24);
            txtNombre.TabIndex = 1;
            //
            // lblDescripcion
            //
            lblDescripcion.AutoSize = true;
            lblDescripcion.Location = new Point(290, 12);
            lblDescripcion.Name = "lblDescripcion";
            lblDescripcion.Size = new Size(69, 16);
            lblDescripcion.TabIndex = 2;
            lblDescripcion.Text = "Descripción";
            //
            // txtDescripcion
            //
            txtDescripcion.Location = new Point(290, 32);
            txtDescripcion.Name = "txtDescripcion";
            txtDescripcion.Size = new Size(300, 24);
            txtDescripcion.TabIndex = 3;
            //
            // chkActivo
            //
            chkActivo.AutoSize = true;
            chkActivo.Checked = true;
            chkActivo.CheckState = CheckState.Checked;
            chkActivo.Location = new Point(610, 34);
            chkActivo.Name = "chkActivo";
            chkActivo.Size = new Size(59, 20);
            chkActivo.TabIndex = 4;
            chkActivo.Text = "Activo";
            //
            // lblPermisos
            //
            lblPermisos.AutoSize = true;
            lblPermisos.Location = new Point(15, 68);
            lblPermisos.Name = "lblPermisos";
            lblPermisos.Size = new Size(56, 16);
            lblPermisos.TabIndex = 5;
            lblPermisos.Text = "Permisos";
            //
            // clbPermisos
            //
            clbPermisos.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            clbPermisos.FormattingEnabled = true;
            clbPermisos.Location = new Point(15, 88);
            clbPermisos.Name = "clbPermisos";
            clbPermisos.Size = new Size(680, 382);
            clbPermisos.TabIndex = 6;
            //
            // FrmRoles
            //
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1150, 552);
            MinimumSize = new Size(1150, 552);
            Controls.Add(panelCaptura);
            Controls.Add(panelListado);
            Controls.Add(toolStripAcciones);
            Name = "FrmRoles";
            Text = "Gestión de Roles";
            toolStripAcciones.ResumeLayout(false);
            toolStripAcciones.PerformLayout();
            panelListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridRoles).EndInit();
            panelCaptura.ResumeLayout(false);
            panelCaptura.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStripAcciones;
        private ToolStripButton tsbNuevo;
        private ToolStripButton tsbGuardar;
        private ToolStripButton tsbEliminar;
        private Panel panelListado;
        private DataGridView gridRoles;
        private Panel panelCaptura;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblDescripcion;
        private TextBox txtDescripcion;
        private CheckBox chkActivo;
        private Label lblPermisos;
        private CheckedListBox clbPermisos;
    }
}
