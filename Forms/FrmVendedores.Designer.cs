namespace Ritrama2025.Forms
{
    partial class FrmVendedores
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
        /// InitializeComponent del rediseño 30/70 del módulo de Vendedores.
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
            gridVendedores = new Sunny.UI.UIDataGridView();
            colVendorId = new DataGridViewTextBoxColumn();
            colVendorName = new DataGridViewTextBoxColumn();
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
            lblCapCorreo = new Sunny.UI.UILabel();
            txtValorCorreo = new Sunny.UI.UITextBox();
            lblCapTelefono = new Sunny.UI.UILabel();
            txtValorTelefono = new Sunny.UI.UITextBox();
            lblCapEstado = new Sunny.UI.UILabel();
            swEstado = new Sunny.UI.UISwitch();
            panelDer = new Panel();
            barraHerramientas = new ToolStrip();
            btnNuevoVendedor = new ToolStripButton();
            btnEditarVendedor = new ToolStripButton();
            tabDetalle = new Sunny.UI.UITabControl();
            tabDetalleVendedor = new TabPage();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridVendedores).BeginInit();
            pnlBuscador.SuspendLayout();
            pnlResumen.SuspendLayout();
            tlpDetalle.SuspendLayout();
            panelDer.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabDetalleVendedor.SuspendLayout();
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
            // panelIzq (30%): buscador arriba, cuadro de resumen debajo, grid de vendedores
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(gridVendedores);
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
            // gridVendedores: vendor_id | vendor_name | status (activo/desactivado)
            // 
            gridVendedores.AllowUserToAddRows = false;
            gridVendedores.AllowUserToDeleteRows = false;
            gridVendedores.AllowUserToResizeRows = false;
            gridVendedores.AutoGenerateColumns = false;
            gridVendedores.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridVendedores.BackgroundColor = Color.White;
            gridVendedores.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloEncabezado.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            estiloEncabezado.ForeColor = Color.White;
            estiloEncabezado.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.SelectionForeColor = Color.White;
            estiloEncabezado.WrapMode = DataGridViewTriState.True;
            gridVendedores.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            gridVendedores.ColumnHeadersHeight = 32;
            gridVendedores.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridVendedores.Columns.AddRange(new DataGridViewColumn[] { colVendorId, colVendorName, colStatus });
            estiloCuerpo.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloCuerpo.BackColor = Color.White;
            estiloCuerpo.Font = new Font("JetBrains Mono", 9F);
            estiloCuerpo.ForeColor = Color.FromArgb(48, 48, 48);
            estiloCuerpo.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloCuerpo.SelectionForeColor = Color.White;
            estiloCuerpo.WrapMode = DataGridViewTriState.False;
            gridVendedores.DefaultCellStyle = estiloCuerpo;
            gridVendedores.Dock = DockStyle.Fill;
            gridVendedores.EnableHeadersVisualStyles = false;
            gridVendedores.Font = new Font("JetBrains Mono", 9F);
            gridVendedores.GridColor = Color.FromArgb(180, 210, 180);
            gridVendedores.Location = new Point(1, 93);
            gridVendedores.MultiSelect = false;
            gridVendedores.Name = "gridVendedores";
            gridVendedores.ReadOnly = true;
            gridVendedores.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloFilasPares.BackColor = Color.White;
            estiloFilasPares.Font = new Font("JetBrains Mono", 9F);
            gridVendedores.RowsDefaultCellStyle = estiloFilasPares;
            estiloFilasImpares.BackColor = Color.FromArgb(245, 250, 240);
            estiloFilasImpares.Font = new Font("JetBrains Mono", 9F);
            gridVendedores.AlternatingRowsDefaultCellStyle = estiloFilasImpares;
            gridVendedores.RowHeadersVisible = false;
            gridVendedores.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridVendedores.Size = new Size(341, 553);
            gridVendedores.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridVendedores.TabIndex = 1;
            // 
            // colVendorId
            // 
            colVendorId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colVendorId.DataPropertyName = "vendor_id";
            colVendorId.FillWeight = 30F;
            colVendorId.HeaderText = "vendor_id";
            colVendorId.Name = "colVendorId";
            colVendorId.ReadOnly = true;
            // 
            // colVendorName
            // 
            colVendorName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colVendorName.DataPropertyName = "vendor_name";
            colVendorName.FillWeight = 45F;
            colVendorName.HeaderText = "vendor_name";
            colVendorName.Name = "colVendorName";
            colVendorName.ReadOnly = true;
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
            lblTitulo.Text = "CATALOGO DE VENDEDORES";
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
            lblResumen.Text = "Total: 0 vendedores";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelDer (70%): página de detalle del vendedor seleccionado
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
            tabDetalle.Controls.Add(tabDetalleVendedor);
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
            btnGuardarVendedor = new ToolStripButton();
            btnCancelarVendedor = new ToolStripButton();
            btnImportarVendedor = new ToolStripButton();
            btnReporteVendedor = new ToolStripButton();
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevoVendedor, btnEditarVendedor, btnImportarVendedor, btnReporteVendedor, btnGuardarVendedor, btnCancelarVendedor });
            barraHerramientas.Location = new Point(8, 8);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 2, 0, 2);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(787, 40);
            barraHerramientas.TabIndex = 1;
            barraHerramientas.Text = "barraHerramientas";
            // 
            // btnNuevoVendedor
            // 
            btnNuevoVendedor.AutoSize = false;
            btnNuevoVendedor.BackColor = Color.Transparent;
            btnNuevoVendedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnNuevoVendedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnNuevoVendedor.Image = Properties.Resources.add_file_32px;
            btnNuevoVendedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnNuevoVendedor.ImageScaling = ToolStripItemImageScaling.None;
            btnNuevoVendedor.Margin = new Padding(4, 1, 0, 2);
            btnNuevoVendedor.Name = "btnNuevoVendedor";
            btnNuevoVendedor.Size = new Size(92, 36);
            btnNuevoVendedor.Text = "Nuevo";
            btnNuevoVendedor.ToolTipText = "Nuevo vendedor";
            // 
            // btnEditarVendedor
            // 
            btnEditarVendedor.AutoSize = false;
            btnEditarVendedor.BackColor = Color.Transparent;
            btnEditarVendedor.Enabled = false;
            btnEditarVendedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnEditarVendedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnEditarVendedor.Image = Properties.Resources.edit_24px;
            btnEditarVendedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnEditarVendedor.ImageScaling = ToolStripItemImageScaling.None;
            btnEditarVendedor.Margin = new Padding(8, 1, 0, 2);
            btnEditarVendedor.Name = "btnEditarVendedor";
            btnEditarVendedor.Size = new Size(92, 36);
            btnEditarVendedor.Text = "Editar";
            btnEditarVendedor.ToolTipText = "Editar el vendedor seleccionado";
            // 
            // btnImportarVendedor
            // 
            btnImportarVendedor.AutoSize = false;
            btnImportarVendedor.BackColor = Color.Transparent;
            btnImportarVendedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnImportarVendedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnImportarVendedor.Image = Properties.Resources.excel_16px;
            btnImportarVendedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnImportarVendedor.ImageScaling = ToolStripItemImageScaling.None;
            btnImportarVendedor.Margin = new Padding(8, 1, 0, 2);
            btnImportarVendedor.Name = "btnImportarVendedor";
            btnImportarVendedor.Size = new Size(92, 36);
            btnImportarVendedor.Text = "Importar";
            btnImportarVendedor.ToolTipText = "Crear una hoja de Excel con todos los vendedores";
            // 
            // btnReporteVendedor
            // 
            // Icono de informe de 32 px (reports_32px), mismo criterio que Productos: los
            // demas iconos de informe del proyecto son de 48 px y con ImageScaling = None
            // desbordarian este boton, que es de 36 px de alto.
            btnReporteVendedor.AutoSize = false;
            btnReporteVendedor.BackColor = Color.Transparent;
            btnReporteVendedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnReporteVendedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnReporteVendedor.Image = Properties.Resources.reports_32px;
            btnReporteVendedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnReporteVendedor.ImageScaling = ToolStripItemImageScaling.None;
            btnReporteVendedor.Margin = new Padding(4, 1, 0, 2);
            btnReporteVendedor.Name = "btnReporteVendedor";
            btnReporteVendedor.Size = new Size(92, 36);
            btnReporteVendedor.Text = "Reporte";
            btnReporteVendedor.ToolTipText = "Ver el catalogo de vendedores en el visor de reportes";
            // 
            // btnGuardarVendedor
            // 
            btnGuardarVendedor.AutoSize = false;
            btnGuardarVendedor.BackColor = Color.Transparent;
            btnGuardarVendedor.Enabled = false;
            btnGuardarVendedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnGuardarVendedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnGuardarVendedor.Image = Properties.Resources.check_file_16;
            btnGuardarVendedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardarVendedor.ImageScaling = ToolStripItemImageScaling.None;
            btnGuardarVendedor.Margin = new Padding(4, 1, 0, 2);
            btnGuardarVendedor.Name = "btnGuardarVendedor";
            btnGuardarVendedor.Size = new Size(92, 36);
            btnGuardarVendedor.Text = "Guardar";
            btnGuardarVendedor.ToolTipText = "Guardar los cambios";
            btnGuardarVendedor.Visible = false;
            // 
            // btnCancelarVendedor
            // 
            btnCancelarVendedor.AutoSize = false;
            btnCancelarVendedor.BackColor = Color.Transparent;
            btnCancelarVendedor.Enabled = false;
            btnCancelarVendedor.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            btnCancelarVendedor.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancelarVendedor.Image = Properties.Resources.cancel_24px;
            btnCancelarVendedor.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelarVendedor.ImageScaling = ToolStripItemImageScaling.None;
            btnCancelarVendedor.Margin = new Padding(4, 1, 0, 2);
            btnCancelarVendedor.Name = "btnCancelarVendedor";
            btnCancelarVendedor.Size = new Size(92, 36);
            btnCancelarVendedor.Text = "Cancelar";
            btnCancelarVendedor.ToolTipText = "Cancelar la edición";
            btnCancelarVendedor.Visible = false;
            // 
            // tabDetalleVendedor
            // 
            tabDetalleVendedor.BackColor = Color.White;
            tabDetalleVendedor.Controls.Add(tlpDetalle);
            tabDetalleVendedor.Location = new Point(0, 32);
            tabDetalleVendedor.Name = "tabDetalleVendedor";
            tabDetalleVendedor.Size = new Size(787, 600);
            tabDetalleVendedor.TabIndex = 0;
            tabDetalleVendedor.Text = "Detalle";
            // 
            // tlpDetalle (estilo FrmProductos/FrmClientes: etiqueta | valor, 9 filas + título)
            // 
            tlpDetalle.ColumnCount = 2;
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpDetalle.Dock = DockStyle.Fill;
            tlpDetalle.Name = "tlpDetalle";
            tlpDetalle.Padding = new Padding(12, 10, 12, 10);
            tlpDetalle.RowCount = 10;
            tlpDetalle.TabIndex = 1;
            // 
            // RowStyles: título (34), 8 filas de datos (38 cada una), última de relleno
            // 
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            // 
            // lblDetalleTitulo (título que ocupa ambas columnas)
            // 
            lblDetalleTitulo.AutoSize = false;
            lblDetalleTitulo.Dock = DockStyle.Fill;
            lblDetalleTitulo.Name = "lblDetalleTitulo";
            lblDetalleTitulo.Text = "DETALLE DEL VENDEDOR";
            lblDetalleTitulo.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblDetalleTitulo, 0, 0);
            tlpDetalle.SetColumnSpan(lblDetalleTitulo, 2);
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
            // lblCapCorreo / txtValorCorreo
            // 
            lblCapCorreo.AutoSize = false;
            lblCapCorreo.Dock = DockStyle.Fill;
            lblCapCorreo.Name = "lblCapCorreo";
            lblCapCorreo.Padding = new Padding(2, 0, 0, 0);
            lblCapCorreo.Text = "Correo";
            lblCapCorreo.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapCorreo, 0, 3);
            txtValorCorreo.Dock = DockStyle.Fill;
            txtValorCorreo.FillColor = Color.White;
            txtValorCorreo.Font = fuenteValor;

            txtValorCorreo.Name = "txtValorCorreo";
            txtValorCorreo.Padding = new Padding(6, 0, 6, 0);
            txtValorCorreo.RectColor = Color.FromArgb(110, 190, 40);
            txtValorCorreo.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorCorreo.Text = "—";
            tlpDetalle.Controls.Add(txtValorCorreo, 1, 3);
            // 
            // lblCapTelefono / txtValorTelefono
            // 
            lblCapTelefono.AutoSize = false;
            lblCapTelefono.Dock = DockStyle.Fill;
            lblCapTelefono.Name = "lblCapTelefono";
            lblCapTelefono.Padding = new Padding(2, 0, 0, 0);
            lblCapTelefono.Text = "Teléfono";
            lblCapTelefono.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapTelefono, 0, 4);
            txtValorTelefono.Dock = DockStyle.Fill;
            txtValorTelefono.FillColor = Color.White;
            txtValorTelefono.Font = fuenteValor;

            txtValorTelefono.Name = "txtValorTelefono";
            txtValorTelefono.Padding = new Padding(6, 0, 6, 0);
            txtValorTelefono.RectColor = Color.FromArgb(110, 190, 40);
            txtValorTelefono.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorTelefono.Text = "—";
            tlpDetalle.Controls.Add(txtValorTelefono, 1, 4);
            // 
            // lblCapEstado / swEstado
            // 
            lblCapEstado.AutoSize = false;
            lblCapEstado.Dock = DockStyle.Fill;
            lblCapEstado.Name = "lblCapEstado";
            lblCapEstado.Padding = new Padding(2, 0, 0, 0);
            lblCapEstado.Text = "Estado";
            lblCapEstado.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblCapEstado, 0, 5);
            swEstado.Active = true;
            swEstado.Dock = DockStyle.Fill;
            swEstado.Enabled = false;
            swEstado.Font = fuenteValor;
            swEstado.Name = "swEstado";
            swEstado.Size = new Size(60, 26);
            swEstado.TabIndex = 0;
            swEstado.Text = "UISwitch";
            tlpDetalle.Controls.Add(swEstado, 1, 5);
            // 
            // FrmVendedores
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1150, 650);
            Controls.Add(tlpRoot);
            MinimumSize = new Size(980, 560);
            Name = "FrmVendedores";
            Text = "Vendedores";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 650);
            tlpRoot.ResumeLayout(false);
            panelIzq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridVendedores).EndInit();
            pnlBuscador.ResumeLayout(false);
            pnlResumen.ResumeLayout(false);
            tlpDetalle.ResumeLayout(false);
            panelDer.ResumeLayout(false);
            tabDetalle.ResumeLayout(false);
            tabDetalleVendedor.ResumeLayout(false);
            barraHerramientas.ResumeLayout(false);
            barraHerramientas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpRoot;
        private Panel panelIzq;
        private Sunny.UI.UIDataGridView gridVendedores;
        private DataGridViewTextBoxColumn colVendorId;
        private DataGridViewTextBoxColumn colVendorName;
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
        private Sunny.UI.UILabel lblCapCorreo;
        private Sunny.UI.UITextBox txtValorCorreo;
        private Sunny.UI.UILabel lblCapTelefono;
        private Sunny.UI.UITextBox txtValorTelefono;
        private Sunny.UI.UILabel lblCapEstado;
        private Sunny.UI.UISwitch swEstado;
        private Panel panelDer;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabDetalleVendedor;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevoVendedor;
        private ToolStripButton btnEditarVendedor;
        private ToolStripButton btnImportarVendedor;
        private ToolStripButton btnReporteVendedor;
        private ToolStripButton btnGuardarVendedor;
        private ToolStripButton btnCancelarVendedor;
    }
}