namespace Ritrama2025.Forms
{
    partial class FrmClientes
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
            toolStripAcciones = new ToolStrip();
            tsbNuevo = new ToolStripButton();
            tsbGuardar = new ToolStripButton();
            tsbAnular = new ToolStripButton();
            txtBuscar = new ToolStripTextBox();
            tsbBuscar = new ToolStripButton();
            panelListado = new Panel();
            gridClientes = new DataGridView();
            panelCaptura = new Panel();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblCategoria = new Label();
            txtCategoria = new TextBox();
            lblEmail = new Label();
            txtEmail = new TextBox();
            toolStripAcciones.SuspendLayout();
            panelListado.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridClientes).BeginInit();
            panelCaptura.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripAcciones
            // 
            toolStripAcciones.AutoSize = false;
            toolStripAcciones.ImageScalingSize = new Size(18, 18);
            toolStripAcciones.Items.AddRange(new ToolStripItem[] { tsbNuevo, tsbGuardar, tsbAnular, txtBuscar, tsbBuscar });
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
            // tsbAnular
            // 
            tsbAnular.AutoSize = false;
            tsbAnular.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbAnular.Enabled = false;
            tsbAnular.Name = "tsbAnular";
            tsbAnular.Size = new Size(70, 32);
            tsbAnular.Text = "Anular";
            tsbAnular.Click += TsbAnular_Click;
            // 
            // txtBuscar
            // 
            txtBuscar.AutoSize = false;
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Size = new Size(220, 30);
            txtBuscar.ToolTipText = "Buscar por cliente";
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
            panelListado.Controls.Add(gridClientes);
            panelListado.Location = new Point(10, 50);
            panelListado.Name = "panelListado";
            panelListado.Size = new Size(1130, 380);
            panelListado.TabIndex = 1;
            // 
            // gridClientes
            // 
            gridClientes.AllowUserToAddRows = false;
            gridClientes.AllowUserToDeleteRows = false;
            gridClientes.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridClientes.AutoGenerateColumns = false;
            gridClientes.BackgroundColor = Color.White;
            gridClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridClientes.Location = new Point(0, 0);
            gridClientes.MultiSelect = false;
            gridClientes.Name = "gridClientes";
            gridClientes.ReadOnly = true;
            gridClientes.RowHeadersVisible = false;
            gridClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridClientes.Size = new Size(1130, 380);
            gridClientes.TabIndex = 0;
            gridClientes.SelectionChanged += GridClientes_SelectionChanged;
            // 
            // panelCaptura
            // 
            panelCaptura.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelCaptura.Controls.Add(lblNombre);
            panelCaptura.Controls.Add(txtNombre);
            panelCaptura.Controls.Add(lblCategoria);
            panelCaptura.Controls.Add(txtCategoria);
            panelCaptura.Controls.Add(lblEmail);
            panelCaptura.Controls.Add(txtEmail);
            panelCaptura.Location = new Point(10, 440);
            panelCaptura.Name = "panelCaptura";
            panelCaptura.Size = new Size(1130, 100);
            panelCaptura.TabIndex = 2;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.Location = new Point(15, 18);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(52, 16);
            lblNombre.TabIndex = 0;
            lblNombre.Text = "Nombre";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(15, 38);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(320, 24);
            txtNombre.TabIndex = 1;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.Location = new Point(360, 18);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(60, 16);
            lblCategoria.TabIndex = 2;
            lblCategoria.Text = "Categoría";
            // 
            // txtCategoria
            // 
            txtCategoria.Location = new Point(360, 38);
            txtCategoria.Name = "txtCategoria";
            txtCategoria.Size = new Size(220, 24);
            txtCategoria.TabIndex = 3;
            // 
            // lblEmail
            // 
            lblEmail.AutoSize = true;
            lblEmail.Location = new Point(610, 18);
            lblEmail.Name = "lblEmail";
            lblEmail.Size = new Size(36, 16);
            lblEmail.TabIndex = 4;
            lblEmail.Text = "Email";
            // 
            // txtEmail
            // 
            txtEmail.Location = new Point(610, 38);
            txtEmail.Name = "txtEmail";
            txtEmail.Size = new Size(320, 24);
            txtEmail.TabIndex = 5;
            // 
            // FrmClientes
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1150, 552);
            MinimumSize = new Size(1150, 552);
            Controls.Add(panelCaptura);
            Controls.Add(panelListado);
            Controls.Add(toolStripAcciones);
            Name = "FrmClientes";
            Text = "Clientes";
            toolStripAcciones.ResumeLayout(false);
            toolStripAcciones.PerformLayout();
            panelListado.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridClientes).EndInit();
            panelCaptura.ResumeLayout(false);
            panelCaptura.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStripAcciones;
        private ToolStripButton tsbNuevo;
        private ToolStripButton tsbGuardar;
        private ToolStripButton tsbAnular;
        private ToolStripTextBox txtBuscar;
        private ToolStripButton tsbBuscar;
        private Panel panelListado;
        private DataGridView gridClientes;
        private Panel panelCaptura;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblCategoria;
        private TextBox txtCategoria;
        private Label lblEmail;
        private TextBox txtEmail;
    }
}
