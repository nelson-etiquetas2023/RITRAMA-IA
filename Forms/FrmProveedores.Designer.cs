namespace Ritrama2025.Forms
{
    partial class FrmProveedores
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null!;

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
        /// InitializeComponent del rediseño 30/70 del módulo de Proveedores.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle estiloEncabezado = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloFilasPares = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloFilasImpares = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloCuerpo = new DataGridViewCellStyle();
            Font fuenteCaption = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            Font fuenteValor = new Font("JetBrains Mono", 9F);
            tlpRoot = new TableLayoutPanel();
            panelIzq = new Panel();
            gridProveedores = new Sunny.UI.UIDataGridView();
            colProveedorId = new DataGridViewTextBoxColumn();
            colProveedorName = new DataGridViewTextBoxColumn();
            colStatus = new DataGridViewTextBoxColumn();
            pnlBuscador = new Panel();
            lblTitulo = new Sunny.UI.UILabel();
            txtBuscar = new Sunny.UI.UITextBox();
            pnlResumen = new Panel();
            lblResumen = new Sunny.UI.UILabel();
            lblDetalleTitulo = new Sunny.UI.UILabel();
            tlpDetalle = new TableLayoutPanel();
            lblCapId = new Sunny.UI.UILabel();
            txtValorId = new Sunny.UI.UITextBox();
            lblCapNombre = new Sunny.UI.UILabel();
            txtValorNombre = new Sunny.UI.UITextBox();
            lblCapTelefono = new Sunny.UI.UILabel();
            txtValorTelefono = new Sunny.UI.UITextBox();
            lblCapDireccion = new Sunny.UI.UILabel();
            txtValorDireccion = new Sunny.UI.UITextBox();
            lblCapEmail = new Sunny.UI.UILabel();
            txtValorEmail = new Sunny.UI.UITextBox();
            lblCapEstado = new Sunny.UI.UILabel();
            swEstado = new Sunny.UI.UISwitch();
            panelDer = new Panel();
            barraHerramientas = new ToolStrip();
            btnNuevoProveedor = new ToolStripButton();
            btnEditarProveedor = new ToolStripButton();
            tabDetalle = new Sunny.UI.UITabControl();
            tabDetalleProveedor = new TabPage();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridProveedores).BeginInit();
            pnlBuscador.SuspendLayout();
            pnlResumen.SuspendLayout();
            tlpDetalle.SuspendLayout();
            panelDer.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabDetalleProveedor.SuspendLayout();
            barraHerramientas.SuspendLayout();
            SuspendLayout();
            // 
            // tlpRoot: divide el ancho en 30% (listado) y 70% (detalle)
            // 
            tlpRoot.ColumnCount = 2;
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tlpRoot.Controls.Add(panelIzq, 0, 0);
            tlpRoot.Controls.Add(panelDer, 1, 0);
            tlpRoot.Dock = DockStyle.Fill;
            tlpRoot.Location = new Point(0, 0);
            tlpRoot.Name = "tlpRoot";
            tlpRoot.RowCount = 1;
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpRoot.Size = new Size(1150, 650);
            tlpRoot.TabIndex = 0;
            // 
            // panelIzq (30%): buscador arriba, cuadro de resumen debajo, grid de proveedores
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(gridProveedores);
            panelIzq.Controls.Add(pnlResumen);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Dock = DockStyle.Fill;
            panelIzq.Location = new Point(1, 1);
            panelIzq.Margin = new Padding(0);
            panelIzq.MinimumSize = new Size(260, 0);
            panelIzq.Name = "panelIzq";
            panelIzq.Size = new Size(345, 648);
            panelIzq.TabIndex = 0;
            // 
            // gridProveedores: Proveedor_Id | Proveedor_Name | status (activo/desactivado)
            // 
            gridProveedores.AllowUserToAddRows = false;
            gridProveedores.AllowUserToDeleteRows = false;
            gridProveedores.AllowUserToResizeRows = false;
            gridProveedores.AutoGenerateColumns = false;
            gridProveedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridProveedores.BackgroundColor = Color.White;
            gridProveedores.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloEncabezado.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            estiloEncabezado.ForeColor = Color.White;
            estiloEncabezado.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.SelectionForeColor = Color.White;
            estiloEncabezado.WrapMode = DataGridViewTriState.True;
            gridProveedores.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            gridProveedores.ColumnHeadersHeight = 32;
            gridProveedores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridProveedores.Columns.AddRange(new DataGridViewColumn[] { colProveedorId, colProveedorName, colStatus });
            estiloCuerpo.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloCuerpo.BackColor = Color.White;
            estiloCuerpo.Font = new Font("JetBrains Mono", 9F);
            estiloCuerpo.ForeColor = Color.FromArgb(48, 48, 48);
            estiloCuerpo.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloCuerpo.SelectionForeColor = Color.White;
            estiloCuerpo.WrapMode = DataGridViewTriState.False;
            gridProveedores.DefaultCellStyle = estiloCuerpo;
            gridProveedores.Dock = DockStyle.Fill;
            gridProveedores.EnableHeadersVisualStyles = false;
            gridProveedores.Font = new Font("JetBrains Mono", 9F);
            gridProveedores.GridColor = Color.FromArgb(180, 210, 180);
            gridProveedores.Location = new Point(1, 93);
            gridProveedores.MultiSelect = false;
            gridProveedores.Name = "gridProveedores";
            gridProveedores.ReadOnly = true;
            gridProveedores.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloFilasPares.BackColor = Color.White;
            estiloFilasPares.Font = new Font("JetBrains Mono", 9F);
            gridProveedores.RowsDefaultCellStyle = estiloFilasPares;
            estiloFilasImpares.BackColor = Color.FromArgb(245, 250, 240);
            estiloFilasImpares.Font = new Font("JetBrains Mono", 9F);
            gridProveedores.AlternatingRowsDefaultCellStyle = estiloFilasImpares;
            gridProveedores.RowHeadersVisible = false;
            gridProveedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridProveedores.Size = new Size(341, 553);
            gridProveedores.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridProveedores.TabIndex = 1;
            // 
            // colProveedorId
            // 
            colProveedorId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colProveedorId.DataPropertyName = "Proveedor_Id";
            colProveedorId.FillWeight = 30F;
            colProveedorId.HeaderText = "Proveedor_Id";
            colProveedorId.Name = "colProveedorId";
            colProveedorId.ReadOnly = true;
            // 
            // colProveedorName
            // 
            colProveedorName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colProveedorName.DataPropertyName = "Proveedor_Name";
            colProveedorName.FillWeight = 45F;
            colProveedorName.HeaderText = "Proveedor_Name";
            colProveedorName.Name = "colProveedorName";
            colProveedorName.ReadOnly = true;
            // 
            // colStatus (se muestra activo / desactivado al venir el dato)
            // 
            colStatus.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colStatus.DataPropertyName = "status";
            colStatus.FillWeight = 25F;
            colStatus.HeaderText = "status";
            colStatus.Name = "colStatus";
            colStatus.ReadOnly = true;
            // 
            // pnlBuscador
            // 
            pnlBuscador.BackColor = Color.White;
            pnlBuscador.Controls.Add(txtBuscar);
            pnlBuscador.Controls.Add(lblTitulo);
            pnlBuscador.Dock = DockStyle.Top;
            pnlBuscador.Location = new Point(1, 1);
            pnlBuscador.Name = "pnlBuscador";
            pnlBuscador.Padding = new Padding(8, 6, 8, 6);
            pnlBuscador.Size = new Size(341, 64);
            pnlBuscador.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = false;
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(60, 110, 20);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(325, 20);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "CATALOGO DE PROVEEDORES";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtBuscar
            // 
            txtBuscar.Dock = DockStyle.Bottom;
            txtBuscar.FillColor = Color.White;
            txtBuscar.Font = new Font("JetBrains Mono", 9F);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Padding = new Padding(6, 0, 6, 0);
            txtBuscar.RectColor = Color.FromArgb(110, 190, 40);
            txtBuscar.Size = new Size(325, 29);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextAlignment = ContentAlignment.MiddleLeft;
            txtBuscar.Watermark = "Buscar por nombre...";
            // 
            // pnlResumen (barra de totales al pie, como la de Productos: verde, texto blanco)
            // 
            pnlResumen.BackColor = Color.FromArgb(110, 190, 40);
            pnlResumen.Controls.Add(lblResumen);
            pnlResumen.Dock = DockStyle.Bottom;
            pnlResumen.Location = new Point(1, 592);
            pnlResumen.Name = "pnlResumen";
            pnlResumen.Padding = new Padding(10, 4, 10, 4);
            pnlResumen.Size = new Size(341, 56);
            pnlResumen.TabIndex = 2;
            // 
            // lblResumen
            // 
            lblResumen.AutoSize = false;
            lblResumen.Dock = DockStyle.Fill;
            lblResumen.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            lblResumen.ForeColor = Color.White;
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(321, 48);
            lblResumen.TabIndex = 0;
            lblResumen.Text = "Total: 0 proveedores";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelDer (70%): página de detalle del proveedor seleccionado
            // 
            panelDer.BackColor = Color.White;
            panelDer.Controls.Add(tabDetalle);
            panelDer.Controls.Add(barraHerramientas);
            panelDer.Dock = DockStyle.Fill;
            panelDer.Location = new Point(346, 1);
            panelDer.Name = "panelDer";
            panelDer.Padding = new Padding(8);
            panelDer.Size = new Size(803, 648);
            panelDer.TabIndex = 1;
            // 
            // tabDetalle
            // 
            tabDetalle.Controls.Add(tabDetalleProveedor);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabDetalle.Font = new Font("JetBrains Mono", 9F);
            tabDetalle.ItemSize = new Size(160, 32);
            tabDetalle.Location = new Point(8, 48);
            tabDetalle.MainPage = "";
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(787, 592);
            tabDetalle.SizeMode = TabSizeMode.Fixed;
            tabDetalle.TabIndex = 0;
            tabDetalle.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            tabDetalle.TipsFont = new Font("JetBrains Mono", 9F);
            // 
            // barraHerramientas
            // 
            barraHerramientas.AutoSize = false;
            barraHerramientas.BackColor = Color.FromArgb(100, 100, 110);
            barraHerramientas.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            barraHerramientas.ForeColor = Color.FromArgb(80, 80, 80);
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            btnGuardarProveedor = new ToolStripButton();
            btnCancelarProveedor = new ToolStripButton();
            btnImportarProveedor = new ToolStripButton();
            btnReporteProveedor = new ToolStripButton();
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevoProveedor, btnEditarProveedor, btnImportarProveedor, btnReporteProveedor, btnGuardarProveedor, btnCancelarProveedor });
            barraHerramientas.Location = new Point(8, 8);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 2, 0, 2);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(787, 40);
            barraHerramientas.TabIndex = 1;
            barraHerramientas.Text = "barraHerramientas";
            // 
            // btnNuevoProveedor
            // 
            btnNuevoProveedor.AutoSize = false;
            btnNuevoProveedor.BackColor = Color.Transparent;
            btnNuevoProveedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnNuevoProveedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnNuevoProveedor.Image = Properties.Resources.add_file_32px;
            btnNuevoProveedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnNuevoProveedor.ImageScaling = ToolStripItemImageScaling.None;
            btnNuevoProveedor.Margin = new Padding(4, 1, 0, 2);
            btnNuevoProveedor.Name = "btnNuevoProveedor";
            btnNuevoProveedor.Size = new Size(92, 36);
            btnNuevoProveedor.Text = "Nuevo";
            btnNuevoProveedor.ToolTipText = "Nuevo proveedor";
            // 
            // btnEditarProveedor
            // 
            btnEditarProveedor.AutoSize = false;
            btnEditarProveedor.BackColor = Color.Transparent;
            btnEditarProveedor.Enabled = false;
            btnEditarProveedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnEditarProveedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnEditarProveedor.Image = Properties.Resources.edit_24px;
            btnEditarProveedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnEditarProveedor.ImageScaling = ToolStripItemImageScaling.None;
            btnEditarProveedor.Margin = new Padding(8, 1, 0, 2);
            btnEditarProveedor.Name = "btnEditarProveedor";
            btnEditarProveedor.Size = new Size(92, 36);
            btnEditarProveedor.Text = "Editar";
            btnEditarProveedor.ToolTipText = "Editar el proveedor seleccionado";
            // 
            // btnImportarProveedor
            // 
            btnImportarProveedor.AutoSize = false;
            btnImportarProveedor.BackColor = Color.Transparent;
            btnImportarProveedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnImportarProveedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnImportarProveedor.Image = Properties.Resources.excel_16px;
            btnImportarProveedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnImportarProveedor.ImageScaling = ToolStripItemImageScaling.None;
            btnImportarProveedor.Margin = new Padding(8, 1, 0, 2);
            btnImportarProveedor.Name = "btnImportarProveedor";
            btnImportarProveedor.Size = new Size(92, 36);
            btnImportarProveedor.Text = "Importar";
            btnImportarProveedor.ToolTipText = "Crear una hoja de Excel con todos los proveedores";
            // 
            // btnReporteProveedor
            // 
            // Icono de informe de 32 px (reports_32px), mismo criterio que Productos: los
            // demas iconos de informe del proyecto son de 48 px y con ImageScaling = None
            // desbordarian este boton, que es de 36 px de alto.
            btnReporteProveedor.AutoSize = false;
            btnReporteProveedor.BackColor = Color.Transparent;
            btnReporteProveedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnReporteProveedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnReporteProveedor.Image = Properties.Resources.reports_32px;
            btnReporteProveedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnReporteProveedor.ImageScaling = ToolStripItemImageScaling.None;
            btnReporteProveedor.Margin = new Padding(4, 1, 0, 2);
            btnReporteProveedor.Name = "btnReporteProveedor";
            btnReporteProveedor.Size = new Size(92, 36);
            btnReporteProveedor.Text = "Reporte";
            btnReporteProveedor.ToolTipText = "Ver el catalogo de proveedores en el visor de reportes";
            // 
            // btnGuardarProveedor
            // 
            btnGuardarProveedor.AutoSize = false;
            btnGuardarProveedor.BackColor = Color.Transparent;
            btnGuardarProveedor.Enabled = false;
            btnGuardarProveedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnGuardarProveedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnGuardarProveedor.Image = Properties.Resources.check_file_16;
            btnGuardarProveedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardarProveedor.ImageScaling = ToolStripItemImageScaling.None;
            btnGuardarProveedor.Margin = new Padding(4, 1, 0, 2);
            btnGuardarProveedor.Name = "btnGuardarProveedor";
            btnGuardarProveedor.Size = new Size(92, 36);
            btnGuardarProveedor.Text = "Guardar";
            btnGuardarProveedor.ToolTipText = "Guardar los cambios";
            btnGuardarProveedor.Visible = false;
            // 
            // btnCancelarProveedor
            // 
            btnCancelarProveedor.AutoSize = false;
            btnCancelarProveedor.BackColor = Color.Transparent;
            btnCancelarProveedor.Enabled = false;
            btnCancelarProveedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnCancelarProveedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancelarProveedor.Image = Properties.Resources.cancel_24px;
            btnCancelarProveedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelarProveedor.ImageScaling = ToolStripItemImageScaling.None;
            btnCancelarProveedor.Margin = new Padding(4, 1, 0, 2);
            btnCancelarProveedor.Name = "btnCancelarProveedor";
            btnCancelarProveedor.Size = new Size(92, 36);
            btnCancelarProveedor.Text = "Cancelar";
            btnCancelarProveedor.ToolTipText = "Cancelar la edición";
            btnCancelarProveedor.Visible = false;
            // 
            // tabDetalleProveedor
            // 
            tabDetalleProveedor.BackColor = Color.White;
            tabDetalleProveedor.Controls.Add(tlpDetalle);
            tabDetalleProveedor.Location = new Point(0, 32);
            tabDetalleProveedor.Name = "tabDetalleProveedor";
            tabDetalleProveedor.Size = new Size(787, 600);
            tabDetalleProveedor.TabIndex = 0;
            tabDetalleProveedor.Text = "Detalle";
            // 
            // lblDetalleTitulo (título que ocupa ambas columnas)
            // 
            lblDetalleTitulo.AutoSize = false;
            lblDetalleTitulo.Dock = DockStyle.Fill;
            lblDetalleTitulo.Name = "lblDetalleTitulo";
            lblDetalleTitulo.Text = "DETALLE DEL PROVEEDOR";
            lblDetalleTitulo.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblDetalleTitulo, 0, 0);
            tlpDetalle.SetColumnSpan(lblDetalleTitulo, 2);
            // 
            // tlpDetalle (estilo FrmProductos: etiqueta | valor, 9 filas + título)
            // 
            tlpDetalle.ColumnCount = 2;
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpDetalle.Dock = DockStyle.Fill;
            tlpDetalle.Name = "tlpDetalle";
            tlpDetalle.Padding = new Padding(12, 10, 12, 10);
            tlpDetalle.RowCount = 8;
            tlpDetalle.TabIndex = 1;
            // 
            // RowStyles: título (34), 6 filas de datos (38 cada una) y relleno
            // 
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            // 
            // lblCapId / txtValorId
            // 
            lblCapId.AutoSize = false;
            lblCapId.Dock = DockStyle.Fill;
            lblCapId.Name = "lblCapId";
            lblCapId.Padding = new Padding(2, 0, 0, 0);
            lblCapId.Text = "Código";
            lblCapId.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapId, 0, 1);
            txtValorId.Dock = DockStyle.Fill;
            txtValorId.FillColor = Color.White;
            txtValorId.Font = fuenteValor;
            txtValorId.ReadOnly = true;

            txtValorId.Name = "txtValorId";
            txtValorId.Padding = new Padding(6, 0, 6, 0);
            txtValorId.RectColor = Color.FromArgb(110, 190, 40);
            txtValorId.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorId.Text = "—";
            tlpDetalle.Controls.Add(txtValorId, 1, 1);
            // 
            // lblCapNombre / txtValorNombre
            // 
            lblCapNombre.AutoSize = false;
            lblCapNombre.Dock = DockStyle.Fill;
            lblCapNombre.Name = "lblCapNombre";
            lblCapNombre.Padding = new Padding(2, 0, 0, 0);
            lblCapNombre.Text = "Nombre";
            lblCapNombre.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapNombre, 0, 2);
            txtValorNombre.Dock = DockStyle.Fill;
            txtValorNombre.FillColor = Color.White;
            txtValorNombre.Font = fuenteValor;

            txtValorNombre.Name = "txtValorNombre";
            txtValorNombre.Padding = new Padding(6, 0, 6, 0);
            txtValorNombre.RectColor = Color.FromArgb(110, 190, 40);
            txtValorNombre.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorNombre.Text = "—";
            tlpDetalle.Controls.Add(txtValorNombre, 1, 2);
            // 
            // lblCapTelefono / txtValorTelefono
            // 
            lblCapTelefono.AutoSize = false;
            lblCapTelefono.Dock = DockStyle.Fill;
            lblCapTelefono.Name = "lblCapTelefono";
            lblCapTelefono.Padding = new Padding(2, 0, 0, 0);
            lblCapTelefono.Text = "Teléfono";
            lblCapTelefono.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapTelefono, 0, 3);
            txtValorTelefono.Dock = DockStyle.Fill;
            txtValorTelefono.FillColor = Color.White;
            txtValorTelefono.Font = fuenteValor;

            txtValorTelefono.Name = "txtValorTelefono";
            txtValorTelefono.Padding = new Padding(6, 0, 6, 0);
            txtValorTelefono.RectColor = Color.FromArgb(110, 190, 40);
            txtValorTelefono.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorTelefono.Text = "—";
            tlpDetalle.Controls.Add(txtValorTelefono, 1, 3);
            // 
            // lblCapDireccion / txtValorDireccion
            // 
            lblCapDireccion.AutoSize = false;
            lblCapDireccion.Dock = DockStyle.Fill;
            lblCapDireccion.Name = "lblCapDireccion";
            lblCapDireccion.Padding = new Padding(2, 0, 0, 0);
            lblCapDireccion.Text = "Dirección";
            lblCapDireccion.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapDireccion, 0, 4);
            txtValorDireccion.Dock = DockStyle.Fill;
            txtValorDireccion.FillColor = Color.White;
            txtValorDireccion.Font = fuenteValor;

            txtValorDireccion.Name = "txtValorDireccion";
            txtValorDireccion.Padding = new Padding(6, 0, 6, 0);
            txtValorDireccion.RectColor = Color.FromArgb(110, 190, 40);
            txtValorDireccion.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorDireccion.Text = "—";
            tlpDetalle.Controls.Add(txtValorDireccion, 1, 4);
            // 
            // lblCapEmail / txtValorEmail
            // 
            lblCapEmail.AutoSize = false;
            lblCapEmail.Dock = DockStyle.Fill;
            lblCapEmail.Name = "lblCapEmail";
            lblCapEmail.Padding = new Padding(2, 0, 0, 0);
            lblCapEmail.Text = "Email";
            lblCapEmail.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapEmail, 0, 5);
            txtValorEmail.Dock = DockStyle.Fill;
            txtValorEmail.FillColor = Color.White;
            txtValorEmail.Font = fuenteValor;

            txtValorEmail.Name = "txtValorEmail";
            txtValorEmail.Padding = new Padding(6, 0, 6, 0);
            txtValorEmail.RectColor = Color.FromArgb(110, 190, 40);
            txtValorEmail.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorEmail.Text = "—";
            tlpDetalle.Controls.Add(txtValorEmail, 1, 5);
            // 
            // lblCapEstado / swEstado
            // 
            lblCapEstado.AutoSize = false;
            lblCapEstado.Dock = DockStyle.Fill;
            lblCapEstado.Name = "lblCapEstado";
            lblCapEstado.Padding = new Padding(2, 0, 0, 0);
            lblCapEstado.Text = "Estado";
            lblCapEstado.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapEstado, 0, 6);
            swEstado.Active = true;
            swEstado.Dock = DockStyle.Fill;
            swEstado.Enabled = false;
            swEstado.Font = fuenteValor;
            swEstado.Name = "swEstado";
            swEstado.Size = new Size(60, 26);
            swEstado.TabIndex = 0;
            swEstado.Text = "UISwitch";
            tlpDetalle.Controls.Add(swEstado, 1, 6);
            // 
            // FrmProveedores
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1150, 650);
            Controls.Add(tlpRoot);
            MinimumSize = new Size(980, 560);
            Name = "FrmProveedores";
            Text = "Proveedores";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 650);
            tlpRoot.ResumeLayout(false);
            panelIzq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridProveedores).EndInit();
            pnlBuscador.ResumeLayout(false);
            pnlResumen.ResumeLayout(false);
            tlpDetalle.ResumeLayout(false);
            panelDer.ResumeLayout(false);
            tabDetalle.ResumeLayout(false);
            tabDetalleProveedor.ResumeLayout(false);
            barraHerramientas.ResumeLayout(false);
            barraHerramientas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpRoot;
        private Panel panelIzq;
        private Sunny.UI.UIDataGridView gridProveedores;
        private DataGridViewTextBoxColumn colProveedorId;
        private DataGridViewTextBoxColumn colProveedorName;
        private DataGridViewTextBoxColumn colStatus;
        private Panel pnlBuscador;
        private Sunny.UI.UILabel lblTitulo;
        private Sunny.UI.UITextBox txtBuscar;
        private Panel pnlResumen;
        private Sunny.UI.UILabel lblResumen;
        private Sunny.UI.UILabel lblDetalleTitulo;
        private TableLayoutPanel tlpDetalle;
        private Sunny.UI.UILabel lblCapId;
        private Sunny.UI.UITextBox txtValorId;
        private Sunny.UI.UILabel lblCapNombre;
        private Sunny.UI.UITextBox txtValorNombre;
        private Sunny.UI.UILabel lblCapTelefono;
        private Sunny.UI.UITextBox txtValorTelefono;
        private Sunny.UI.UILabel lblCapDireccion;
        private Sunny.UI.UITextBox txtValorDireccion;
        private Sunny.UI.UILabel lblCapEmail;
        private Sunny.UI.UITextBox txtValorEmail;
        private Sunny.UI.UILabel lblCapEstado;
        private Sunny.UI.UISwitch swEstado;
        private Panel panelDer;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabDetalleProveedor;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevoProveedor;
        private ToolStripButton btnEditarProveedor;
        private ToolStripButton btnImportarProveedor;
        private ToolStripButton btnReporteProveedor;
        private ToolStripButton btnGuardarProveedor;
        private ToolStripButton btnCancelarProveedor;
    }
}
