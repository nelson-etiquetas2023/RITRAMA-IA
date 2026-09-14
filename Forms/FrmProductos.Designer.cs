namespace Ritrama2025.Forms
{
    partial class FrmProductos
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
            tsbCancelar = new ToolStripButton();
            tsbAnular = new ToolStripButton();
            panelFranja = new Panel();
            lblBuscar = new Label();
            txtBuscar = new TextBox();
            btnBuscar = new Button();
            lblProductId = new Label();
            txtProductId = new TextBox();
            lblNombre = new Label();
            txtNombre = new TextBox();
            lblUnidad = new Label();
            txtUnidad = new TextBox();
            lblCantidad = new Label();
            txtCantidad = new TextBox();
            lblWidth = new Label();
            txtWidth = new TextBox();
            lblLenght = new Label();
            txtLenght = new TextBox();
            lblMsi = new Label();
            txtMsi = new TextBox();
            lblPrecio = new Label();
            txtPrecio = new TextBox();
            lblTotal = new Label();
            txtTotal = new TextBox();
            lblCategoria = new Label();
            cboCategoria = new ComboBox();
            panelDetalle = new Panel();
            btnAdd = new Button();
            gridDetalle = new DataGridView();
            toolStripAcciones.SuspendLayout();
            panelFranja.SuspendLayout();
            panelDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridDetalle).BeginInit();
            SuspendLayout();
            // 
            // toolStripAcciones
            // 
            toolStripAcciones.AutoSize = false;
            toolStripAcciones.ImageScalingSize = new Size(18, 18);
            toolStripAcciones.Items.AddRange(new ToolStripItem[] { tsbNuevo, tsbGuardar, tsbCancelar, tsbAnular });
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
            tsbNuevo.Click += BtnNuevo_Click;
            // 
            // tsbGuardar
            // 
            tsbGuardar.AutoSize = false;
            tsbGuardar.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbGuardar.Enabled = false;
            tsbGuardar.Name = "tsbGuardar";
            tsbGuardar.Size = new Size(70, 32);
            tsbGuardar.Text = "Guardar";
            tsbGuardar.Click += BtnGuardar_Click;
            // 
            // tsbCancelar
            // 
            tsbCancelar.AutoSize = false;
            tsbCancelar.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbCancelar.Enabled = false;
            tsbCancelar.Name = "tsbCancelar";
            tsbCancelar.Size = new Size(90, 32);
            tsbCancelar.Text = "Cancelar";
            tsbCancelar.Click += BtnCancelar_Click;
            // 
            // tsbAnular
            // 
            tsbAnular.AutoSize = false;
            tsbAnular.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbAnular.Enabled = false;
            tsbAnular.Name = "tsbAnular";
            tsbAnular.Size = new Size(70, 32);
            tsbAnular.Text = "Anular";
            tsbAnular.Click += BtnAnular_Click;
            // 
            // panelFranja
            // 
            panelFranja.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelFranja.BackColor = Color.FromArgb(110, 190, 40);
            panelFranja.Controls.Add(lblBuscar);
            panelFranja.Controls.Add(txtBuscar);
            panelFranja.Controls.Add(btnBuscar);
            panelFranja.Controls.Add(lblProductId);
            panelFranja.Controls.Add(txtProductId);
            panelFranja.Controls.Add(lblNombre);
            panelFranja.Controls.Add(txtNombre);
            panelFranja.Controls.Add(lblUnidad);
            panelFranja.Controls.Add(txtUnidad);
            panelFranja.Controls.Add(lblCantidad);
            panelFranja.Controls.Add(txtCantidad);
            panelFranja.Controls.Add(lblWidth);
            panelFranja.Controls.Add(txtWidth);
            panelFranja.Controls.Add(lblLenght);
            panelFranja.Controls.Add(txtLenght);
            panelFranja.Controls.Add(lblMsi);
            panelFranja.Controls.Add(txtMsi);
            panelFranja.Controls.Add(lblPrecio);
            panelFranja.Controls.Add(txtPrecio);
            panelFranja.Controls.Add(lblTotal);
            panelFranja.Controls.Add(txtTotal);
            panelFranja.Controls.Add(lblCategoria);
            panelFranja.Controls.Add(cboCategoria);
            panelFranja.Location = new Point(10, 50);
            panelFranja.Name = "panelFranja";
            panelFranja.Size = new Size(1130, 118);
            panelFranja.TabIndex = 1;
            // 
            // lblBuscar
            // 
            lblBuscar.AutoSize = true;
            lblBuscar.ForeColor = Color.White;
            lblBuscar.Location = new Point(15, 14);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Size = new Size(92, 16);
            lblBuscar.TabIndex = 0;
            lblBuscar.Text = "Buscar Producto";
            // 
            // txtBuscar
            // 
            txtBuscar.Location = new Point(15, 33);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.ReadOnly = true;
            txtBuscar.Size = new Size(300, 24);
            txtBuscar.TabIndex = 1;
            // 
            // btnBuscar
            // 
            btnBuscar.FlatAppearance.BorderSize = 0;
            btnBuscar.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 140, 25);
            btnBuscar.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 210, 80);
            btnBuscar.FlatStyle = FlatStyle.Flat;
            btnBuscar.Location = new Point(318, 33);
            btnBuscar.Name = "btnBuscar";
            btnBuscar.Size = new Size(30, 24);
            btnBuscar.TabIndex = 2;
            btnBuscar.Text = "...";
            btnBuscar.UseVisualStyleBackColor = true;
            btnBuscar.Click += BtnBuscar_Click;
            // 
            // lblProductId
            // 
            lblProductId.AutoSize = true;
            lblProductId.ForeColor = Color.White;
            lblProductId.Location = new Point(360, 14);
            lblProductId.Name = "lblProductId";
            lblProductId.Size = new Size(63, 16);
            lblProductId.TabIndex = 3;
            lblProductId.Text = "Product ID";
            // 
            // txtProductId
            // 
            txtProductId.Location = new Point(360, 33);
            txtProductId.Name = "txtProductId";
            txtProductId.Size = new Size(120, 24);
            txtProductId.TabIndex = 4;
            // 
            // lblNombre
            // 
            lblNombre.AutoSize = true;
            lblNombre.ForeColor = Color.White;
            lblNombre.Location = new Point(495, 14);
            lblNombre.Name = "lblNombre";
            lblNombre.Size = new Size(50, 16);
            lblNombre.TabIndex = 5;
            lblNombre.Text = "Nombre";
            // 
            // txtNombre
            // 
            txtNombre.Location = new Point(495, 33);
            txtNombre.Name = "txtNombre";
            txtNombre.Size = new Size(280, 24);
            txtNombre.TabIndex = 6;
            // 
            // lblUnidad
            // 
            lblUnidad.AutoSize = true;
            lblUnidad.ForeColor = Color.White;
            lblUnidad.Location = new Point(15, 70);
            lblUnidad.Name = "lblUnidad";
            lblUnidad.Size = new Size(46, 16);
            lblUnidad.TabIndex = 7;
            lblUnidad.Text = "Unidad";
            // 
            // txtUnidad
            // 
            txtUnidad.Location = new Point(15, 90);
            txtUnidad.Name = "txtUnidad";
            txtUnidad.Size = new Size(80, 24);
            txtUnidad.TabIndex = 8;
            txtUnidad.Text = "ROLLO";
            // 
            // lblCantidad
            // 
            lblCantidad.AutoSize = true;
            lblCantidad.ForeColor = Color.White;
            lblCantidad.Location = new Point(105, 70);
            lblCantidad.Name = "lblCantidad";
            lblCantidad.Size = new Size(56, 16);
            lblCantidad.TabIndex = 9;
            lblCantidad.Text = "Cantidad";
            // 
            // txtCantidad
            // 
            txtCantidad.Location = new Point(105, 90);
            txtCantidad.Name = "txtCantidad";
            txtCantidad.Size = new Size(80, 24);
            txtCantidad.TabIndex = 10;
            txtCantidad.TextChanged += TxtMedida_TextChanged;
            // 
            // lblWidth
            // 
            lblWidth.AutoSize = true;
            lblWidth.ForeColor = Color.White;
            lblWidth.Location = new Point(195, 70);
            lblWidth.Name = "lblWidth";
            lblWidth.Size = new Size(42, 16);
            lblWidth.TabIndex = 11;
            lblWidth.Text = "Ancho";
            // 
            // txtWidth
            // 
            txtWidth.Location = new Point(195, 90);
            txtWidth.Name = "txtWidth";
            txtWidth.Size = new Size(80, 24);
            txtWidth.TabIndex = 12;
            txtWidth.TextChanged += TxtMedida_TextChanged;
            // 
            // lblLenght
            // 
            lblLenght.AutoSize = true;
            lblLenght.ForeColor = Color.White;
            lblLenght.Location = new Point(285, 70);
            lblLenght.Name = "lblLenght";
            lblLenght.Size = new Size(35, 16);
            lblLenght.TabIndex = 13;
            lblLenght.Text = "Largo";
            // 
            // txtLenght
            // 
            txtLenght.Location = new Point(285, 90);
            txtLenght.Name = "txtLenght";
            txtLenght.Size = new Size(90, 24);
            txtLenght.TabIndex = 14;
            txtLenght.TextChanged += TxtMedida_TextChanged;
            // 
            // lblMsi
            // 
            lblMsi.AutoSize = true;
            lblMsi.ForeColor = Color.White;
            lblMsi.Location = new Point(385, 70);
            lblMsi.Name = "lblMsi";
            lblMsi.Size = new Size(31, 16);
            lblMsi.TabIndex = 15;
            lblMsi.Text = "MSI";
            // 
            // txtMsi
            // 
            txtMsi.Location = new Point(385, 90);
            txtMsi.Name = "txtMsi";
            txtMsi.ReadOnly = true;
            txtMsi.Size = new Size(100, 24);
            txtMsi.TabIndex = 16;
            txtMsi.TextAlign = HorizontalAlignment.Right;
            // 
            // lblPrecio
            // 
            lblPrecio.AutoSize = true;
            lblPrecio.ForeColor = Color.White;
            lblPrecio.Location = new Point(495, 70);
            lblPrecio.Name = "lblPrecio";
            lblPrecio.Size = new Size(43, 16);
            lblPrecio.TabIndex = 17;
            lblPrecio.Text = "Precio";
            // 
            // txtPrecio
            // 
            txtPrecio.Location = new Point(495, 90);
            txtPrecio.Name = "txtPrecio";
            txtPrecio.Size = new Size(90, 24);
            txtPrecio.TabIndex = 18;
            txtPrecio.TextChanged += TxtMedida_TextChanged;
            // 
            // lblTotal
            // 
            lblTotal.AutoSize = true;
            lblTotal.ForeColor = Color.White;
            lblTotal.Location = new Point(595, 70);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(35, 16);
            lblTotal.TabIndex = 19;
            lblTotal.Text = "Total";
            // 
            // txtTotal
            // 
            txtTotal.Location = new Point(595, 90);
            txtTotal.Name = "txtTotal";
            txtTotal.ReadOnly = true;
            txtTotal.Size = new Size(110, 24);
            txtTotal.TabIndex = 20;
            txtTotal.TextAlign = HorizontalAlignment.Right;
            // 
            // lblCategoria
            // 
            lblCategoria.AutoSize = true;
            lblCategoria.ForeColor = Color.White;
            lblCategoria.Location = new Point(715, 70);
            lblCategoria.Name = "lblCategoria";
            lblCategoria.Size = new Size(60, 16);
            lblCategoria.TabIndex = 21;
            lblCategoria.Text = "Categoría";
            // 
            // cboCategoria
            // 
            cboCategoria.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCategoria.FormattingEnabled = true;
            cboCategoria.Items.AddRange(new object[] { "Master", "Rollo Cortado", "Resma", "Graphics" });
            cboCategoria.Location = new Point(715, 90);
            cboCategoria.Name = "cboCategoria";
            cboCategoria.Size = new Size(130, 24);
            cboCategoria.TabIndex = 22;
            // 
            // panelDetalle
            // 
            panelDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelDetalle.Controls.Add(btnAdd);
            panelDetalle.Controls.Add(gridDetalle);
            panelDetalle.Location = new Point(10, 180);
            panelDetalle.Name = "panelDetalle";
            panelDetalle.Size = new Size(1130, 460);
            panelDetalle.TabIndex = 2;
            // 
            // btnAdd
            // 
            btnAdd.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnAdd.BackColor = Color.FromArgb(110, 190, 40);
            btnAdd.FlatAppearance.BorderSize = 0;
            btnAdd.FlatStyle = FlatStyle.Flat;
            btnAdd.ForeColor = Color.White;
            btnAdd.Location = new Point(1020, 5);
            btnAdd.Name = "btnAdd";
            btnAdd.Size = new Size(100, 30);
            btnAdd.TabIndex = 1;
            btnAdd.Text = "Add";
            btnAdd.UseVisualStyleBackColor = false;
            btnAdd.Click += BtnAdd_Click;
            // 
            // gridDetalle
            // 
            gridDetalle.AllowUserToAddRows = false;
            gridDetalle.AllowUserToDeleteRows = false;
            gridDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridDetalle.AutoGenerateColumns = false;
            gridDetalle.BackgroundColor = Color.White;
            gridDetalle.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridDetalle.Location = new Point(10, 40);
            gridDetalle.MultiSelect = false;
            gridDetalle.Name = "gridDetalle";
            gridDetalle.ReadOnly = true;
            gridDetalle.RowHeadersVisible = false;
            gridDetalle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDetalle.Size = new Size(1110, 410);
            gridDetalle.TabIndex = 0;
            gridDetalle.SelectionChanged += GridDetalle_SelectionChanged;
            // 
            // FrmProductos
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1150, 650);
            Controls.Add(panelDetalle);
            Controls.Add(panelFranja);
            Controls.Add(toolStripAcciones);
            MinimumSize = new Size(1150, 650);
            Name = "FrmProductos";
            Text = "Productos";
            toolStripAcciones.ResumeLayout(false);
            toolStripAcciones.PerformLayout();
            panelFranja.ResumeLayout(false);
            panelFranja.PerformLayout();
            panelDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridDetalle).EndInit();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStripAcciones;
        private ToolStripButton tsbNuevo;
        private ToolStripButton tsbGuardar;
        private ToolStripButton tsbCancelar;
        private ToolStripButton tsbAnular;
        private Panel panelFranja;
        private Label lblBuscar;
        private TextBox txtBuscar;
        private Button btnBuscar;
        private Label lblProductId;
        private TextBox txtProductId;
        private Label lblNombre;
        private TextBox txtNombre;
        private Label lblUnidad;
        private TextBox txtUnidad;
        private Label lblCantidad;
        private TextBox txtCantidad;
        private Label lblWidth;
        private TextBox txtWidth;
        private Label lblLenght;
        private TextBox txtLenght;
        private Label lblMsi;
        private TextBox txtMsi;
        private Label lblPrecio;
        private TextBox txtPrecio;
        private Label lblTotal;
        private TextBox txtTotal;
        private Label lblCategoria;
        private ComboBox cboCategoria;
        private Panel panelDetalle;
        private Button btnAdd;
        private DataGridView gridDetalle;
    }
}