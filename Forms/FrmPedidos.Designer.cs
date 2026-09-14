namespace Ritrama2025.Forms
{
    partial class FrmPedidos
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
            tsbBuscarCliente = new ToolStripButton();
            tsbBuscarVendedor = new ToolStripButton();
            panelHeader = new Panel();
            lblNotas = new Label();
            txtNotas = new TextBox();
            lblContacto = new Label();
            txtContacto = new TextBox();
            lblDireccion = new Label();
            txtDireccion = new TextBox();
            lblCondPago = new Label();
            cboCondPago = new ComboBox();
            cboTipoVenta = new ComboBox();
            lblTipoVenta = new Label();
            btnCliente = new Button();
            txtClienteNombre = new TextBox();
            txtClienteID = new TextBox();
            lblCliente = new Label();
            dtpFechaEntrega = new DateTimePicker();
            lblFechaEntrega = new Label();
            dtpFecha = new DateTimePicker();
            lblFecha = new Label();
            txtNumero = new TextBox();
            lblNumero = new Label();
            lblProducto = new Label();
            txtProductoID = new TextBox();
            txtProductoNombre = new TextBox();
            btnBuscarProducto = new Button();
            lblVendedor = new Label();
            txtVendedorID = new TextBox();
            txtVendedorNombre = new TextBox();
            btnBuscarVendedor = new Button();
            panelDetalle = new Panel();
            btnQuitarLinea = new Button();
            btnAgregarLinea = new Button();
            gridDetalle = new DataGridView();
            panelTotales = new Panel();
            txtTotal = new TextBox();
            lblTotal = new Label();
            txtItbis = new TextBox();
            lblItbis = new Label();
            txtPorcItbis = new TextBox();
            lblPorcItbis = new Label();
            txtSubtotal = new TextBox();
            lblSubtotal = new Label();
            toolStripAcciones.SuspendLayout();
            panelHeader.SuspendLayout();
            panelDetalle.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridDetalle).BeginInit();
            panelTotales.SuspendLayout();
            SuspendLayout();
            // 
            // toolStripAcciones
            // 
            toolStripAcciones.AutoSize = false;
            toolStripAcciones.ImageScalingSize = new Size(18, 18);
            toolStripAcciones.Items.AddRange(new ToolStripItem[] { tsbNuevo, tsbGuardar, tsbBuscarCliente, tsbBuscarVendedor });
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
            tsbGuardar.Name = "tsbGuardar";
            tsbGuardar.Size = new Size(70, 32);
            tsbGuardar.Text = "Guardar";
            tsbGuardar.Click += BtnGuardar_Click;
            // 
            // tsbBuscarCliente
            // 
            tsbBuscarCliente.AutoSize = false;
            tsbBuscarCliente.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbBuscarCliente.Name = "tsbBuscarCliente";
            tsbBuscarCliente.Size = new Size(90, 32);
            tsbBuscarCliente.Text = "Buscar Cliente";
            tsbBuscarCliente.Click += BtnCliente_Click;
            // 
            // tsbBuscarVendedor
            // 
            tsbBuscarVendedor.AutoSize = false;
            tsbBuscarVendedor.DisplayStyle = ToolStripItemDisplayStyle.Text;
            tsbBuscarVendedor.Name = "tsbBuscarVendedor";
            tsbBuscarVendedor.Size = new Size(100, 32);
            tsbBuscarVendedor.Text = "Buscar Vendedor";
            tsbBuscarVendedor.Click += BtnBuscarVendedor_Click;
            // 
            // panelHeader
            // 
            panelHeader.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            panelHeader.Controls.Add(lblNotas);
            panelHeader.Controls.Add(txtNotas);
            panelHeader.Controls.Add(lblContacto);
            panelHeader.Controls.Add(txtContacto);
            panelHeader.Controls.Add(lblDireccion);
            panelHeader.Controls.Add(txtDireccion);
            panelHeader.Controls.Add(lblCondPago);
            panelHeader.Controls.Add(cboCondPago);
            panelHeader.Controls.Add(cboTipoVenta);
            panelHeader.Controls.Add(lblTipoVenta);
            panelHeader.Controls.Add(dtpFechaEntrega);
            panelHeader.Controls.Add(lblFechaEntrega);
            panelHeader.Controls.Add(dtpFecha);
            panelHeader.Controls.Add(lblFecha);
            panelHeader.Controls.Add(btnCliente);
            panelHeader.Controls.Add(txtClienteNombre);
            panelHeader.Controls.Add(txtClienteID);
            panelHeader.Controls.Add(lblCliente);
            panelHeader.Controls.Add(txtNumero);
            panelHeader.Controls.Add(lblNumero);
            panelHeader.Controls.Add(lblProducto);
            panelHeader.Controls.Add(txtProductoID);
            panelHeader.Controls.Add(txtProductoNombre);
            panelHeader.Controls.Add(btnBuscarProducto);
            panelHeader.Controls.Add(lblVendedor);
            panelHeader.Controls.Add(txtVendedorID);
            panelHeader.Controls.Add(txtVendedorNombre);
            panelHeader.Controls.Add(btnBuscarVendedor);
            panelHeader.Location = new Point(10, 154);
            panelHeader.Name = "panelHeader";
            panelHeader.Size = new Size(1130, 190);
            panelHeader.TabIndex = 2;
            // 
            // lblNumero
            // 
            lblNumero.AutoSize = true;
            lblNumero.Location = new Point(15, 18);
            lblNumero.Name = "lblNumero";
            lblNumero.Size = new Size(50, 16);
            lblNumero.TabIndex = 0;
            lblNumero.Text = "Numero";
            // 
            // txtNumero
            // 
            txtNumero.Location = new Point(15, 38);
            txtNumero.Name = "txtNumero";
            txtNumero.ReadOnly = true;
            txtNumero.Size = new Size(120, 24);
            txtNumero.TabIndex = 1;
            // 
            // lblFecha
            // 
            lblFecha.AutoSize = true;
            lblFecha.Location = new Point(150, 18);
            lblFecha.Name = "lblFecha";
            lblFecha.Size = new Size(40, 16);
            lblFecha.TabIndex = 2;
            lblFecha.Text = "Fecha";
            // 
            // dtpFecha
            // 
            dtpFecha.Format = DateTimePickerFormat.Short;
            dtpFecha.Location = new Point(150, 38);
            dtpFecha.Name = "dtpFecha";
            dtpFecha.Size = new Size(120, 24);
            dtpFecha.TabIndex = 3;
            // 
            // lblFechaEntrega
            // 
            lblFechaEntrega.AutoSize = true;
            lblFechaEntrega.Location = new Point(285, 18);
            lblFechaEntrega.Name = "lblFechaEntrega";
            lblFechaEntrega.Size = new Size(88, 16);
            lblFechaEntrega.TabIndex = 4;
            lblFechaEntrega.Text = "Fecha Entrega";
            // 
            // dtpFechaEntrega
            // 
            dtpFechaEntrega.Format = DateTimePickerFormat.Short;
            dtpFechaEntrega.Location = new Point(285, 38);
            dtpFechaEntrega.Name = "dtpFechaEntrega";
            dtpFechaEntrega.Size = new Size(120, 24);
            dtpFechaEntrega.TabIndex = 5;
            // 
            // lblCliente
            // 
            lblCliente.AutoSize = true;
            lblCliente.Location = new Point(560, 18);
            lblCliente.Name = "lblCliente";
            lblCliente.Size = new Size(45, 16);
            lblCliente.TabIndex = 7;
            lblCliente.Text = "Cliente";
            // 
            // txtClienteID
            // 
            txtClienteID.Location = new Point(560, 38);
            txtClienteID.Name = "txtClienteID";
            txtClienteID.ReadOnly = true;
            txtClienteID.Size = new Size(200, 24);
            txtClienteID.TabIndex = 8;
            // 
            // txtClienteNombre
            // 
            txtClienteNombre.Location = new Point(765, 38);
            txtClienteNombre.Name = "txtClienteNombre";
            txtClienteNombre.ReadOnly = true;
            txtClienteNombre.Size = new Size(280, 24);
            txtClienteNombre.TabIndex = 9;
            // 
            // btnCliente
            // 
            btnCliente.FlatAppearance.BorderSize = 0;
            btnCliente.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 140, 25);
            btnCliente.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 210, 80);
            btnCliente.FlatStyle = FlatStyle.Flat;
            btnCliente.Location = new Point(1050, 38);
            btnCliente.Name = "btnCliente";
            btnCliente.Size = new Size(30, 24);
            btnCliente.TabIndex = 10;
            btnCliente.Text = "...";
            btnCliente.UseVisualStyleBackColor = true;
            btnCliente.Click += BtnCliente_Click;
            // 
            // lblProducto
            // 
            lblProducto.AutoSize = true;
            lblProducto.Location = new Point(15, 135);
            lblProducto.Name = "lblProducto";
            lblProducto.Size = new Size(60, 16);
            lblProducto.TabIndex = 22;
            lblProducto.Text = "Producto";
            // 
            // txtProductoID
            // 
            txtProductoID.Location = new Point(15, 155);
            txtProductoID.Name = "txtProductoID";
            txtProductoID.ReadOnly = true;
            txtProductoID.Size = new Size(120, 24);
            txtProductoID.TabIndex = 23;
            // 
            // txtProductoNombre
            // 
            txtProductoNombre.Location = new Point(140, 155);
            txtProductoNombre.Name = "txtProductoNombre";
            txtProductoNombre.ReadOnly = true;
            txtProductoNombre.Size = new Size(400, 24);
            txtProductoNombre.TabIndex = 24;
            // 
            // btnBuscarProducto
            // 
            btnBuscarProducto.FlatAppearance.BorderSize = 0;
            btnBuscarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 140, 25);
            btnBuscarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 210, 80);
            btnBuscarProducto.FlatStyle = FlatStyle.Flat;
            btnBuscarProducto.Location = new Point(545, 155);
            btnBuscarProducto.Name = "btnBuscarProducto";
            btnBuscarProducto.Size = new Size(30, 24);
            btnBuscarProducto.TabIndex = 25;
            btnBuscarProducto.Text = "...";
            btnBuscarProducto.UseVisualStyleBackColor = true;
            btnBuscarProducto.Click += BtnBuscarProducto_Click;
            // 
            // lblVendedor
            // 
            lblVendedor.AutoSize = true;
            lblVendedor.Location = new Point(600, 135);
            lblVendedor.Name = "lblVendedor";
            lblVendedor.Size = new Size(60, 16);
            lblVendedor.TabIndex = 26;
            lblVendedor.Text = "Vendedor";
            // 
            // txtVendedorID
            // 
            txtVendedorID.Location = new Point(600, 155);
            txtVendedorID.Name = "txtVendedorID";
            txtVendedorID.ReadOnly = true;
            txtVendedorID.Size = new Size(120, 24);
            txtVendedorID.TabIndex = 27;
            // 
            // txtVendedorNombre
            // 
            txtVendedorNombre.Location = new Point(725, 155);
            txtVendedorNombre.Name = "txtVendedorNombre";
            txtVendedorNombre.ReadOnly = true;
            txtVendedorNombre.Size = new Size(350, 24);
            txtVendedorNombre.TabIndex = 28;
            // 
            // btnBuscarVendedor
            // 
            btnBuscarVendedor.FlatAppearance.BorderSize = 0;
            btnBuscarVendedor.FlatAppearance.MouseDownBackColor = Color.FromArgb(70, 140, 25);
            btnBuscarVendedor.FlatAppearance.MouseOverBackColor = Color.FromArgb(150, 210, 80);
            btnBuscarVendedor.FlatStyle = FlatStyle.Flat;
            btnBuscarVendedor.Location = new Point(1080, 155);
            btnBuscarVendedor.Name = "btnBuscarVendedor";
            btnBuscarVendedor.Size = new Size(30, 24);
            btnBuscarVendedor.TabIndex = 29;
            btnBuscarVendedor.Text = "...";
            btnBuscarVendedor.UseVisualStyleBackColor = true;
            btnBuscarVendedor.Click += BtnBuscarVendedor_Click;
            // 
            // lblTipoVenta
            // 
            lblTipoVenta.AutoSize = true;
            lblTipoVenta.Location = new Point(15, 78);
            lblTipoVenta.Name = "lblTipoVenta";
            lblTipoVenta.Size = new Size(77, 16);
            lblTipoVenta.TabIndex = 12;
            lblTipoVenta.Text = "Tipo de Venta";
            // 
            // cboTipoVenta
            // 
            cboTipoVenta.FormattingEnabled = true;
            cboTipoVenta.Items.AddRange(new object[] { "Nacional", "Importacion" });
            cboTipoVenta.Location = new Point(15, 98);
            cboTipoVenta.Name = "cboTipoVenta";
            cboTipoVenta.Size = new Size(180, 24);
            cboTipoVenta.TabIndex = 13;
            // 
            // lblCondPago
            // 
            lblCondPago.AutoSize = true;
            lblCondPago.Location = new Point(210, 78);
            lblCondPago.Name = "lblCondPago";
            lblCondPago.Size = new Size(100, 16);
            lblCondPago.TabIndex = 14;
            lblCondPago.Text = "Condiciones Pago";
            // 
            // txtCondPago
            // 
            // cboCondPago
            // 
            cboCondPago.DropDownStyle = ComboBoxStyle.DropDownList;
            cboCondPago.FormattingEnabled = true;
            cboCondPago.Items.AddRange(new object[] { "Contado", "Credito" });
            cboCondPago.Location = new Point(210, 98);
            cboCondPago.Name = "cboCondPago";
            cboCondPago.Size = new Size(240, 24);
            cboCondPago.TabIndex = 15;
            // 
            // lblDireccion
            // 
            lblDireccion.AutoSize = true;
            lblDireccion.Location = new Point(465, 78);
            lblDireccion.Name = "lblDireccion";
            lblDireccion.Size = new Size(110, 16);
            lblDireccion.TabIndex = 16;
            lblDireccion.Text = "Dirección Entrega";
            // 
            // txtDireccion
            // 
            txtDireccion.Location = new Point(465, 98);
            txtDireccion.Name = "txtDireccion";
            txtDireccion.Size = new Size(250, 24);
            txtDireccion.TabIndex = 17;
            // 
            // lblContacto
            // 
            lblContacto.AutoSize = true;
            lblContacto.Location = new Point(730, 78);
            lblContacto.Name = "lblContacto";
            lblContacto.Size = new Size(100, 16);
            lblContacto.TabIndex = 18;
            lblContacto.Text = "Persona Contacto";
            // 
            // txtContacto
            // 
            txtContacto.Location = new Point(730, 98);
            txtContacto.Name = "txtContacto";
            txtContacto.Size = new Size(200, 24);
            txtContacto.TabIndex = 19;
            // 
            // lblNotas
            // 
            lblNotas.AutoSize = true;
            lblNotas.Location = new Point(950, 78);
            lblNotas.Name = "lblNotas";
            lblNotas.Size = new Size(38, 16);
            lblNotas.TabIndex = 20;
            lblNotas.Text = "Notas";
            // 
            // txtNotas
            // 
            txtNotas.Location = new Point(950, 98);
            txtNotas.Multiline = true;
            txtNotas.Name = "txtNotas";
            txtNotas.Size = new Size(170, 50);
            txtNotas.TabIndex = 21;
            // 
            // panelDetalle
            // 
            panelDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelDetalle.Controls.Add(btnQuitarLinea);
            panelDetalle.Controls.Add(btnAgregarLinea);
            panelDetalle.Controls.Add(gridDetalle);
            panelDetalle.Location = new Point(10, 354);
            panelDetalle.Name = "panelDetalle";
            panelDetalle.Size = new Size(1130, 185);
            panelDetalle.TabIndex = 3;
            // 
            // btnQuitarLinea
            // 
            btnQuitarLinea.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            btnQuitarLinea.FlatAppearance.BorderSize = 0;
            btnQuitarLinea.FlatStyle = FlatStyle.Flat;
            btnQuitarLinea.Location = new Point(1070, 5);
            btnQuitarLinea.Name = "btnQuitarLinea";
            btnQuitarLinea.Size = new Size(60, 26);
            btnQuitarLinea.TabIndex = 2;
            btnQuitarLinea.Text = "Quitar";
            btnQuitarLinea.UseVisualStyleBackColor = true;
            btnQuitarLinea.Click += BtnQuitarLinea_Click;
            // 
            // btnAgregarLinea
            // 
            btnAgregarLinea.FlatAppearance.BorderSize = 0;
            btnAgregarLinea.FlatStyle = FlatStyle.Flat;
            btnAgregarLinea.Location = new Point(10, 5);
            btnAgregarLinea.Name = "btnAgregarLinea";
            btnAgregarLinea.Size = new Size(130, 26);
            btnAgregarLinea.TabIndex = 1;
            btnAgregarLinea.Text = "Agregar Rollo";
            btnAgregarLinea.UseVisualStyleBackColor = true;
            btnAgregarLinea.Click += BtnAgregarLinea_Click;
            // 
            // gridDetalle
            // 
            gridDetalle.AllowUserToAddRows = false;
            gridDetalle.AllowUserToDeleteRows = false;
            gridDetalle.Anchor = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            gridDetalle.AutoGenerateColumns = false;
            gridDetalle.BackgroundColor = Color.White;
            gridDetalle.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridDetalle.Location = new Point(10, 36);
            gridDetalle.MultiSelect = false;
            gridDetalle.Name = "gridDetalle";
            gridDetalle.RowHeadersVisible = false;
            gridDetalle.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridDetalle.Size = new Size(1110, 145);
            gridDetalle.TabIndex = 0;
            gridDetalle.CellEndEdit += GridDetalle_CellEndEdit;
            // 
            // panelTotales
            // 
            panelTotales.Anchor = AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            panelTotales.Controls.Add(txtTotal);
            panelTotales.Controls.Add(lblTotal);
            panelTotales.Controls.Add(txtItbis);
            panelTotales.Controls.Add(lblItbis);
            panelTotales.Controls.Add(txtPorcItbis);
            panelTotales.Controls.Add(lblPorcItbis);
            panelTotales.Controls.Add(txtSubtotal);
            panelTotales.Controls.Add(lblSubtotal);
            panelTotales.Location = new Point(10, 547);
            panelTotales.Name = "panelTotales";
            panelTotales.Size = new Size(1130, 62);
            panelTotales.TabIndex = 4;
            // 
            // txtTotal
            // 
            txtTotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtTotal.Location = new Point(955, 20);
            txtTotal.Name = "txtTotal";
            txtTotal.ReadOnly = true;
            txtTotal.Size = new Size(170, 24);
            txtTotal.TabIndex = 7;
            txtTotal.TextAlign = HorizontalAlignment.Right;
            // 
            // lblTotal
            // 
            lblTotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblTotal.AutoSize = true;
            lblTotal.Location = new Point(910, 24);
            lblTotal.Name = "lblTotal";
            lblTotal.Size = new Size(35, 16);
            lblTotal.TabIndex = 6;
            lblTotal.Text = "Total";
            // 
            // txtItbis
            // 
            txtItbis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtItbis.Location = new Point(685, 20);
            txtItbis.Name = "txtItbis";
            txtItbis.ReadOnly = true;
            txtItbis.Size = new Size(170, 24);
            txtItbis.TabIndex = 5;
            txtItbis.TextAlign = HorizontalAlignment.Right;
            // 
            // lblItbis
            // 
            lblItbis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblItbis.AutoSize = true;
            lblItbis.Location = new Point(645, 24);
            lblItbis.Name = "lblItbis";
            lblItbis.Size = new Size(35, 16);
            lblItbis.TabIndex = 4;
            lblItbis.Text = "ITBIS";
            // 
            // txtPorcItbis
            // 
            txtPorcItbis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtPorcItbis.Location = new Point(440, 20);
            txtPorcItbis.Name = "txtPorcItbis";
            txtPorcItbis.Size = new Size(100, 24);
            txtPorcItbis.TabIndex = 3;
            txtPorcItbis.Text = "18";
            txtPorcItbis.TextAlign = HorizontalAlignment.Right;
            // 
            // lblPorcItbis
            // 
            lblPorcItbis.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblPorcItbis.AutoSize = true;
            lblPorcItbis.Location = new Point(350, 24);
            lblPorcItbis.Name = "lblPorcItbis";
            lblPorcItbis.Size = new Size(75, 16);
            lblPorcItbis.TabIndex = 2;
            lblPorcItbis.Text = "% ITBIS";
            // 
            // txtSubtotal
            // 
            txtSubtotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            txtSubtotal.Location = new Point(140, 20);
            txtSubtotal.Name = "txtSubtotal";
            txtSubtotal.ReadOnly = true;
            txtSubtotal.Size = new Size(170, 24);
            txtSubtotal.TabIndex = 1;
            txtSubtotal.TextAlign = HorizontalAlignment.Right;
            // 
            // lblSubtotal
            // 
            lblSubtotal.Anchor = AnchorStyles.Top | AnchorStyles.Right;
            lblSubtotal.AutoSize = true;
            lblSubtotal.Location = new Point(70, 24);
            lblSubtotal.Name = "lblSubtotal";
            lblSubtotal.Size = new Size(56, 16);
            lblSubtotal.TabIndex = 0;
            lblSubtotal.Text = "Subtotal";
            // 
            // FrmPedidos
            // 
            AutoScaleDimensions = new SizeF(7F, 16F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = Color.White;
            ClientSize = new Size(1150, 622);
            MinimumSize = new Size(1150, 622);
            Controls.Add(panelTotales);
            Controls.Add(panelDetalle);
            Controls.Add(panelHeader);
            Controls.Add(toolStripAcciones);
            Name = "FrmPedidos";
            Text = "Pedidos de Cliente";
            toolStripAcciones.ResumeLayout(false);
            toolStripAcciones.PerformLayout();
            panelHeader.ResumeLayout(false);
            panelHeader.PerformLayout();
            panelDetalle.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridDetalle).EndInit();
            panelTotales.ResumeLayout(false);
            panelTotales.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private ToolStrip toolStripAcciones;
        private ToolStripButton tsbNuevo;
        private ToolStripButton tsbGuardar;
        private ToolStripButton tsbBuscarCliente;
        private ToolStripButton tsbBuscarVendedor;
        private Panel panelHeader;
        private Label lblNumero;
        private TextBox txtNumero;
        private Label lblFecha;
        private DateTimePicker dtpFecha;
        private Label lblFechaEntrega;
        private DateTimePicker dtpFechaEntrega;
        private Label lblCliente;
        private TextBox txtClienteID;
        private TextBox txtClienteNombre;
        private Button btnCliente;
        private Label lblProducto;
        private TextBox txtProductoID;
        private TextBox txtProductoNombre;
        private Button btnBuscarProducto;
        private Label lblVendedor;
        private TextBox txtVendedorID;
        private TextBox txtVendedorNombre;
        private Button btnBuscarVendedor;
        private Label lblTipoVenta;
        private ComboBox cboTipoVenta;
        private Label lblCondPago;
        private ComboBox cboCondPago;
        private Label lblDireccion;
        private TextBox txtDireccion;
        private Label lblContacto;
        private TextBox txtContacto;
        private Label lblNotas;
        private TextBox txtNotas;
        private Panel panelDetalle;
        private Button btnAgregarLinea;
        private Button btnQuitarLinea;
        private DataGridView gridDetalle;
        private Panel panelTotales;
        private Label lblSubtotal;
        private TextBox txtSubtotal;
        private Label lblPorcItbis;
        private TextBox txtPorcItbis;
        private Label lblItbis;
        private TextBox txtItbis;
        private Label lblTotal;
        private TextBox txtTotal;
    }
}
