namespace Ritrama2025.Forms
{
    using System.Globalization;

    partial class FrmOrdenesCompra
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
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle styleDetalle = new DataGridViewCellStyle();
            DataGridViewCellStyle styleEncabezado = new DataGridViewCellStyle();
            DataGridViewCellStyle styleCuerpo = new DataGridViewCellStyle();
            DataGridViewCellStyle styleAlt = new DataGridViewCellStyle();
            Font fuenteLabel = new Font("JetBrains Mono", 9F);
            Font fuenteValor = new Font("JetBrains Mono", 9F);
            tlpRoot = new TableLayoutPanel();
            panelIzq = new Panel();
            pnlBuscadorOc = new Panel();
            lblTitulo = new Sunny.UI.UILabel();
            txtBuscar = new Sunny.UI.UITextBox();
            pnlResumen = new Panel();
            lblResumen = new Sunny.UI.UILabel();
            gridOrdenes = new Sunny.UI.UIDataGridView();
            colNumero = new DataGridViewTextBoxColumn();
            colProveedor = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            panelDer = new Panel();
            tabDetalle = new Sunny.UI.UITabControl();
            tabGeneral = new TabPage();
            barraHerramientas = new ToolStrip();
            btnNuevo = new ToolStripButton();
            btnEditar = new ToolStripButton();
            btnGuardar = new ToolStripButton();
            btnCancelar = new ToolStripButton();
            sw_anular_oc = new Sunny.UI.UISwitch();
            txt_width = new Sunny.UI.UITextBox();
            txt_length = new Sunny.UI.UITextBox();
            txt_total_cantidad = new Sunny.UI.UITextBox();
            uiLabelTotalCant = new Sunny.UI.UILabel();
            uiLabelNotas = new Sunny.UI.UILabel();
            uiLabelItbisPor = new Sunny.UI.UILabel();
            uiLabelTotal = new Sunny.UI.UILabel();
            uiLabelItbis = new Sunny.UI.UILabel();
            uiLabelSubtotal = new Sunny.UI.UILabel();
            uiTextBox7 = new Sunny.UI.UITextBox();
            cbo_prioridad = new Sunny.UI.UIComboBox();
            uiLabelPrioridad = new Sunny.UI.UILabel();
            txt_id_proveedor = new Sunny.UI.UITextBox();
            uiLabelIdProv = new Sunny.UI.UILabel();
            txtPersonaContacto = new Sunny.UI.UITextBox();
            cboCondicionesPago = new Sunny.UI.UIComboBox();
            uiRichTextBox3 = new Sunny.UI.UIRichTextBox();
            uiLabelShipTo = new Sunny.UI.UILabel();
            uiLabelBillTo = new Sunny.UI.UILabel();
            uiRichTextBox2 = new Sunny.UI.UIRichTextBox();
            uiRichTextBox1 = new Sunny.UI.UIRichTextBox();
            uiTextBox2 = new Sunny.UI.UITextBox();
            uiLabelStatus = new Sunny.UI.UILabel();
            uiTextBox1 = new Sunny.UI.UITextBox();
            uiLabelNumero = new Sunny.UI.UILabel();
            uiDatetimePicker2 = new Sunny.UI.UIDatetimePicker();
            uiLabelFechaEst = new Sunny.UI.UILabel();
            uiDatetimePicker1 = new Sunny.UI.UIDatetimePicker();
            uiLabelFechaReg = new Sunny.UI.UILabel();
            cbo_proveedores = new Sunny.UI.UIComboBox();
            uiLabelProveedor = new Sunny.UI.UILabel();
            uiLabelItbisPor2 = new Sunny.UI.UILabel();
            btnBuscarProducto = new Button();
            btnAddProducto = new Button();
            btnEliminarProducto = new Button();
            btnEditarProducto = new Button();
            uiDataGridView1 = new Sunny.UI.UIDataGridView();
            renglon = new DataGridViewTextBoxColumn();
            Description = new DataGridViewTextBoxColumn();
            Unit = new DataGridViewTextBoxColumn();
            Qty = new DataGridViewTextBoxColumn();
            Notes = new DataGridViewTextBoxColumn();
            Price = new DataGridViewTextBoxColumn();
            widthCol = new DataGridViewTextBoxColumn();
            lenghtCol = new DataGridViewTextBoxColumn();
            subtotalCol = new DataGridViewTextBoxColumn();
            uiComboBox2 = new Sunny.UI.UIComboBox();
            uiTextBox9 = new Sunny.UI.UITextBox();
            uiTextBox8 = new Sunny.UI.UITextBox();
            uiLabelProduct = new Sunny.UI.UILabel();
            uiTextBox3 = new Sunny.UI.UITextBox();
            uiLabelQty = new Sunny.UI.UILabel();
            uiTextBox4 = new Sunny.UI.UITextBox();
            uiTextBox5 = new Sunny.UI.UITextBox();
            uiTextBox6 = new Sunny.UI.UITextBox();
            uiLine1 = new Sunny.UI.UILine();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            pnlBuscadorOc.SuspendLayout();
            pnlResumen.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridOrdenes).BeginInit();
            panelDer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).BeginInit();
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
            // panelIzq (30%): buscador arriba, cuadro de resumen debajo, grid de OCs
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(gridOrdenes);
            panelIzq.Controls.Add(pnlResumen);
            panelIzq.Controls.Add(pnlBuscadorOc);
            panelIzq.Dock = DockStyle.Fill;
            panelIzq.Location = new Point(1, 1);
            panelIzq.Margin = new Padding(0);
            panelIzq.MinimumSize = new Size(260, 0);
            panelIzq.Name = "panelIzq";
            panelIzq.Size = new Size(345, 648);
            panelIzq.TabIndex = 0;
            // 
            // pnlBuscadorOc
            // 
            pnlBuscadorOc.BackColor = Color.White;
            pnlBuscadorOc.Controls.Add(txtBuscar);
            pnlBuscadorOc.Controls.Add(lblTitulo);
            pnlBuscadorOc.Dock = DockStyle.Top;
            pnlBuscadorOc.Location = new Point(1, 1);
            pnlBuscadorOc.Name = "pnlBuscadorOc";
            pnlBuscadorOc.Padding = new Padding(8, 6, 8, 6);
            pnlBuscadorOc.Size = new Size(341, 64);
            pnlBuscadorOc.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.AutoSize = false;
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Font = new Font("JetBrains Mono", 9.75F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(60, 110, 20);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(325, 20);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "CATALOG O DE ORDENES DE COMPRA";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtBuscar
            // 
            txtBuscar.Dock = DockStyle.Bottom;
            txtBuscar.FillColor = Color.White;
            txtBuscar.Font = fuenteValor;
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Padding = new Padding(6, 0, 6, 0);
            txtBuscar.RectColor = Color.FromArgb(110, 190, 40);
            txtBuscar.Size = new Size(325, 29);
            txtBuscar.TabIndex = 1;
            txtBuscar.TextAlignment = ContentAlignment.MiddleLeft;
            txtBuscar.Watermark = "Buscar por número, proveedor o estado...";
            // 
            // pnlResumen
            // 
            pnlResumen.BackColor = Color.FromArgb(245, 250, 240);
            pnlResumen.Controls.Add(lblResumen);
            pnlResumen.Dock = DockStyle.Top;
            pnlResumen.Location = new Point(1, 65);
            pnlResumen.Name = "pnlResumen";
            pnlResumen.Padding = new Padding(8, 4, 8, 4);
            pnlResumen.Size = new Size(341, 28);
            pnlResumen.TabIndex = 2;
            // 
            // lblResumen
            // 
            lblResumen.AutoSize = false;
            lblResumen.Dock = DockStyle.Fill;
            lblResumen.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            lblResumen.ForeColor = Color.FromArgb(60, 110, 20);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(325, 20);
            lblResumen.TabIndex = 0;
            lblResumen.Text = "Total: 0 órdenes";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // gridOrdenes
            // 
            gridOrdenes.AllowUserToAddRows = false;
            gridOrdenes.AllowUserToDeleteRows = false;
            gridOrdenes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridOrdenes.BackgroundColor = Color.White;
            gridOrdenes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            styleEncabezado.Alignment = DataGridViewContentAlignment.MiddleCenter;
            styleEncabezado.BackColor = Color.FromArgb(110, 190, 40);
            styleEncabezado.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            styleEncabezado.ForeColor = Color.White;
            styleEncabezado.SelectionBackColor = Color.FromArgb(110, 190, 40);
            styleEncabezado.SelectionForeColor = Color.White;
            styleEncabezado.WrapMode = DataGridViewTriState.True;
            gridOrdenes.ColumnHeadersDefaultCellStyle = styleEncabezado;
            gridOrdenes.ColumnHeadersHeight = 32;
            gridOrdenes.Columns.AddRange(new DataGridViewColumn[] { colNumero, colProveedor, colEstado });
            styleCuerpo.Alignment = DataGridViewContentAlignment.MiddleLeft;
            styleCuerpo.BackColor = Color.White;
            styleCuerpo.Font = new Font("JetBrains Mono", 9F);
            styleCuerpo.ForeColor = Color.FromArgb(48, 48, 48);
            styleCuerpo.SelectionBackColor = Color.FromArgb(110, 190, 40);
            styleCuerpo.SelectionForeColor = Color.White;
            styleCuerpo.WrapMode = DataGridViewTriState.False;
            gridOrdenes.DefaultCellStyle = styleCuerpo;
            gridOrdenes.Dock = DockStyle.Fill;
            gridOrdenes.EnableHeadersVisualStyles = false;
            gridOrdenes.Font = new Font("JetBrains Mono", 9F);
            gridOrdenes.GridColor = Color.FromArgb(180, 210, 180);
            gridOrdenes.Location = new Point(1, 93);
            gridOrdenes.MultiSelect = false;
            gridOrdenes.Name = "gridOrdenes";
            gridOrdenes.ReadOnly = true;
            gridOrdenes.RowHeadersVisible = false;
            gridOrdenes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridOrdenes.Size = new Size(341, 553);
            gridOrdenes.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridOrdenes.TabIndex = 1;
            styleAlt.BackColor = Color.White;
            styleAlt.Font = new Font("JetBrains Mono", 9F);
            gridOrdenes.RowsDefaultCellStyle = styleAlt;
            styleAlt.BackColor = Color.FromArgb(245, 250, 240);
            gridOrdenes.AlternatingRowsDefaultCellStyle = styleAlt;
            // 
            // colNumero
            // 
            colNumero.DataPropertyName = "numero";
            colNumero.HeaderText = "Número";
            colNumero.Name = "colNumero";
            colNumero.ReadOnly = true;
            colNumero.Width = 100;
            // 
            // colProveedor
            // 
            colProveedor.DataPropertyName = "proveedor_name";
            colProveedor.HeaderText = "Proveedor";
            colProveedor.Name = "colProveedor";
            colProveedor.ReadOnly = true;
            colProveedor.Width = 180;
            // 
            // colEstado
            // 
            colEstado.DataPropertyName = "estado";
            colEstado.HeaderText = "Estado";
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            colEstado.Width = 65;
            // 
            // panelDer (70%): página de detalle + edición
            // 
            panelDer.BackColor = Color.White;
            panelDer.Controls.Add(tabDetalle);
            panelDer.Dock = DockStyle.Fill;
            panelDer.Location = new Point(346, 1);
            panelDer.Name = "panelDer";
            panelDer.Padding = new Padding(8);
            panelDer.Size = new Size(803, 648);
            panelDer.TabIndex = 1;
            // 
            // tabDetalle
            // 
            tabDetalle.Controls.Add(tabGeneral);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabDetalle.Font = new Font("JetBrains Mono", 9F);
            tabDetalle.ItemSize = new Size(100, 24);
            tabDetalle.Location = new Point(8, 8);
            tabDetalle.MainPage = "";
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(787, 632);
            tabDetalle.SizeMode = TabSizeMode.Fixed;
            tabDetalle.TabIndex = 0;
            // tabDetalle.TabUnselectedForeColor = Color.FromArgb(240, 240, 240);
            tabDetalle.TipsFont = new Font("JetBrains Mono", 9F);
            // 
            // tabGeneral
            // 
            tabGeneral.BackColor = Color.White;
            tabGeneral.Controls.Add(uiLine1);
            tabGeneral.Controls.Add(txt_width);
            tabGeneral.Controls.Add(txt_length);
            tabGeneral.Controls.Add(txt_total_cantidad);
            tabGeneral.Controls.Add(uiLabelTotalCant);
            tabGeneral.Controls.Add(uiLabelNotas);
            tabGeneral.Controls.Add(uiLabelItbisPor);
            tabGeneral.Controls.Add(uiLabelTotal);
            tabGeneral.Controls.Add(uiLabelItbis);
            tabGeneral.Controls.Add(uiLabelSubtotal);
            tabGeneral.Controls.Add(uiTextBox7);
            tabGeneral.Controls.Add(cbo_prioridad);
            tabGeneral.Controls.Add(uiLabelPrioridad);
            tabGeneral.Controls.Add(txt_id_proveedor);
            tabGeneral.Controls.Add(uiLabelIdProv);
            tabGeneral.Controls.Add(txtPersonaContacto);
            tabGeneral.Controls.Add(cboCondicionesPago);
            tabGeneral.Controls.Add(uiLabelItbisPor2);
            tabGeneral.Controls.Add(uiRichTextBox3);
            tabGeneral.Controls.Add(uiLabelShipTo);
            tabGeneral.Controls.Add(uiLabelBillTo);
            tabGeneral.Controls.Add(uiRichTextBox2);
            tabGeneral.Controls.Add(uiRichTextBox1);
            tabGeneral.Controls.Add(uiTextBox2);
            tabGeneral.Controls.Add(uiLabelStatus);
            tabGeneral.Controls.Add(uiTextBox1);
            tabGeneral.Controls.Add(uiLabelNumero);
            tabGeneral.Controls.Add(uiDatetimePicker2);
            tabGeneral.Controls.Add(uiLabelFechaEst);
            tabGeneral.Controls.Add(uiDatetimePicker1);
            tabGeneral.Controls.Add(uiLabelFechaReg);
            tabGeneral.Controls.Add(cbo_proveedores);
            tabGeneral.Controls.Add(uiLabelProveedor);
            tabGeneral.Controls.Add(btnBuscarProducto);
            tabGeneral.Controls.Add(btnAddProducto);
            tabGeneral.Controls.Add(btnEliminarProducto);
            tabGeneral.Controls.Add(btnEditarProducto);
            tabGeneral.Controls.Add(uiDataGridView1);
            tabGeneral.Controls.Add(uiComboBox2);
            tabGeneral.Controls.Add(uiTextBox9);
            tabGeneral.Controls.Add(uiTextBox8);
            tabGeneral.Controls.Add(uiLabelProduct);
            tabGeneral.Controls.Add(uiTextBox3);
            tabGeneral.Controls.Add(uiLabelQty);
            tabGeneral.Controls.Add(uiTextBox4);
            tabGeneral.Controls.Add(uiTextBox5);
            tabGeneral.Controls.Add(uiTextBox6);
            tabGeneral.Controls.Add(sw_anular_oc);
            tabGeneral.Controls.Add(barraHerramientas);
            tabGeneral.Location = new Point(4, 26);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Padding = new Padding(3);
            tabGeneral.Size = new Size(783, 602);
            tabGeneral.TabIndex = 0;
            tabGeneral.Text = "General";
            tabGeneral.UseVisualStyleBackColor = true;
            // 
            // barraHerramientas
            // 
            barraHerramientas.AutoSize = false;
            barraHerramientas.BackColor = Color.FromArgb(225, 225, 225);
            barraHerramientas.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            barraHerramientas.ForeColor = Color.FromArgb(80, 80, 80);
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar, btnGuardar, btnCancelar });
            barraHerramientas.Location = new Point(3, 6);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 0, 0, 0);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(773, 28);
            barraHerramientas.TabIndex = 0;
            // 
            // btnNuevo
            // 
            btnNuevo.AutoSize = false;
            btnNuevo.BackColor = Color.Transparent;
            btnNuevo.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            btnNuevo.ForeColor = Color.FromArgb(80, 80, 80);
            btnNuevo.Image = Properties.Resources.add_file_32px;
            btnNuevo.ImageAlign = ContentAlignment.MiddleLeft;
            btnNuevo.ImageScaling = ToolStripItemImageScaling.None;
            btnNuevo.Margin = new Padding(4, 1, 0, 2);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(92, 24);
            btnNuevo.Text = "Nuevo";
            btnNuevo.ToolTipText = "Nueva orden de compra";
            // 
            // btnEditar
            // 
            btnEditar.AutoSize = false;
            btnEditar.BackColor = Color.Transparent;
            btnEditar.Enabled = true;
            btnEditar.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            btnEditar.ForeColor = Color.FromArgb(80, 80, 80);
            btnEditar.Image = Properties.Resources.edit_24px;
            btnEditar.ImageAlign = ContentAlignment.MiddleLeft;
            btnEditar.ImageScaling = ToolStripItemImageScaling.None;
            btnEditar.Margin = new Padding(4, 1, 0, 2);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(92, 24);
            btnEditar.Text = "Editar";
            btnEditar.ToolTipText = "Editar orden seleccionada";
            // 
            // btnGuardar
            // 
            btnGuardar.AutoSize = false;
            btnGuardar.BackColor = Color.Transparent;
            btnGuardar.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.FromArgb(80, 80, 80);
            btnGuardar.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardar.ImageScaling = ToolStripItemImageScaling.None;
            btnGuardar.Margin = new Padding(4, 1, 0, 2);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(92, 24);
            btnGuardar.Text = "Guardar";
            btnGuardar.ToolTipText = "Guardar la orden de compra";
            btnGuardar.Visible = false;
            // 
            // btnCancelar
            // 
            btnCancelar.AutoSize = false;
            btnCancelar.BackColor = Color.Transparent;
            btnCancelar.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancelar.Image = Properties.Resources.cancel_24px;
            btnCancelar.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelar.ImageScaling = ToolStripItemImageScaling.None;
            btnCancelar.Margin = new Padding(4, 1, 0, 2);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(92, 24);
            btnCancelar.Text = "Cancelar";
            btnCancelar.ToolTipText = "Descartar la orden en borrador";
            btnCancelar.Visible = false;
            // 
            // sw_anular_oc
            // 
            sw_anular_oc.Active = true;
            sw_anular_oc.ActiveColor = Color.FromArgb(110, 190, 40);
            sw_anular_oc.ActiveText = "Orden Activa";
            sw_anular_oc.Font = new Font("Microsoft Sans Serif", 10F);
            sw_anular_oc.InActiveColor = Color.Firebrick;
            sw_anular_oc.InActiveText = "Orden Anulada";
            sw_anular_oc.Location = new Point(12, 559);
            sw_anular_oc.MinimumSize = new Size(1, 1);
            sw_anular_oc.Name = "sw_anular_oc";
            sw_anular_oc.Size = new Size(155, 26);
            sw_anular_oc.TabIndex = 38;
            sw_anular_oc.Text = "Anular Orden";
            // 
            // uiLine1
            // 
            uiLine1.BackColor = Color.Transparent;
            uiLine1.Font = new Font("Microsoft Sans Serif", 10F);
            uiLine1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLine1.Location = new Point(10, 540);
            uiLine1.MinimumSize = new Size(1, 1);
            uiLine1.Name = "uiLine1";
            uiLine1.Size = new Size(745, 18);
            uiLine1.TabIndex = 37;
            uiLine1.Text = "Acciones Orden";
            // 
            // Header Row 1: Proveedor combo, Numero, Id Prov, Status
            // 
            uiLabelProveedor.Font = fuenteLabel;
            uiLabelProveedor.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelProveedor.Location = new Point(8, 40);
            uiLabelProveedor.Name = "uiLabelProveedor";
            uiLabelProveedor.Size = new Size(100, 23);
            uiLabelProveedor.TabIndex = 0;
            uiLabelProveedor.Text = "Proveedor :";
            uiLabelProveedor.TextAlign = ContentAlignment.MiddleLeft;
            // 
            cbo_proveedores.DataSource = null;
            cbo_proveedores.FillColor = Color.White;
            cbo_proveedores.Font = fuenteValor;
            cbo_proveedores.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cbo_proveedores.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cbo_proveedores.Location = new Point(113, 34);
            cbo_proveedores.Margin = new Padding(4, 5, 4, 5);
            cbo_proveedores.MinimumSize = new Size(63, 0);
            cbo_proveedores.Name = "cbo_proveedores";
            cbo_proveedores.Padding = new Padding(0, 0, 30, 2);
            cbo_proveedores.ReadOnly = true;
            cbo_proveedores.Size = new Size(255, 29);
            cbo_proveedores.SymbolSize = 24;
            cbo_proveedores.TabIndex = 1;
            cbo_proveedores.TextAlignment = ContentAlignment.MiddleLeft;
            cbo_proveedores.Watermark = "";
            // 
            uiLabelNumero.Font = fuenteLabel;
            uiLabelNumero.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelNumero.Location = new Point(371, 40);
            uiLabelNumero.Name = "uiLabelNumero";
            uiLabelNumero.Size = new Size(122, 23);
            uiLabelNumero.TabIndex = 6;
            uiLabelNumero.Text = "Numero Orden :";
            // 
            uiTextBox1.Font = fuenteValor;
            uiTextBox1.Location = new Point(499, 34);
            uiTextBox1.Margin = new Padding(4, 5, 4, 5);
            uiTextBox1.MinimumSize = new Size(1, 16);
            uiTextBox1.Name = "uiTextBox1";
            uiTextBox1.Padding = new Padding(5);
            uiTextBox1.ReadOnly = true;
            uiTextBox1.ShowText = false;
            uiTextBox1.Size = new Size(248, 29);
            uiTextBox1.TabIndex = 7;
            uiTextBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox1.Watermark = "";
            // 
            uiLabelIdProv.Font = fuenteLabel;
            uiLabelIdProv.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelIdProv.Location = new Point(8, 80);
            uiLabelIdProv.Name = "uiLabelIdProv";
            uiLabelIdProv.Size = new Size(100, 23);
            uiLabelIdProv.TabIndex = 8;
            uiLabelIdProv.Text = "Id Proveedor :";
            // 
            txt_id_proveedor.Font = fuenteValor;
            txt_id_proveedor.Location = new Point(113, 74);
            txt_id_proveedor.Margin = new Padding(4, 5, 4, 5);
            txt_id_proveedor.MinimumSize = new Size(1, 16);
            txt_id_proveedor.Name = "txt_id_proveedor";
            txt_id_proveedor.Padding = new Padding(5);
            txt_id_proveedor.ReadOnly = true;
            txt_id_proveedor.ShowText = false;
            txt_id_proveedor.Size = new Size(120, 29);
            txt_id_proveedor.TabIndex = 9;
            txt_id_proveedor.TextAlignment = ContentAlignment.MiddleLeft;
            txt_id_proveedor.Watermark = "";
            // 
            uiLabelStatus.Font = fuenteLabel;
            uiLabelStatus.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelStatus.Location = new Point(240, 80);
            uiLabelStatus.Name = "uiLabelStatus";
            uiLabelStatus.Size = new Size(60, 23);
            uiLabelStatus.TabIndex = 10;
            uiLabelStatus.Text = "Estado :";
            // 
            uiTextBox2.Font = fuenteValor;
            uiTextBox2.Location = new Point(306, 74);
            uiTextBox2.Margin = new Padding(4, 5, 4, 5);
            uiTextBox2.MinimumSize = new Size(1, 16);
            uiTextBox2.Name = "uiTextBox2";
            uiTextBox2.Padding = new Padding(5);
            uiTextBox2.ReadOnly = true;
            uiTextBox2.ShowText = false;
            uiTextBox2.Size = new Size(120, 29);
            uiTextBox2.TabIndex = 11;
            uiTextBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox2.Watermark = "";
            // 
            // Header Row 2: Fecha Reg, Fecha Est, Prioridad
            // 
            uiLabelFechaReg.Font = fuenteLabel;
            uiLabelFechaReg.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelFechaReg.Location = new Point(8, 120);
            uiLabelFechaReg.Name = "uiLabelFechaReg";
            uiLabelFechaReg.Size = new Size(100, 23);
            uiLabelFechaReg.TabIndex = 12;
            uiLabelFechaReg.Text = "Fecha Reg :";
            // 
            uiDatetimePicker1.DateCultureInfo = new CultureInfo("es-DO");
            uiDatetimePicker1.FillColor = Color.White;
            uiDatetimePicker1.Font = fuenteValor;
            uiDatetimePicker1.Location = new Point(113, 114);
            uiDatetimePicker1.Margin = new Padding(4, 5, 4, 5);
            uiDatetimePicker1.MaxLength = 19;
            uiDatetimePicker1.MinimumSize = new Size(63, 0);
            uiDatetimePicker1.Name = "uiDatetimePicker1";
            uiDatetimePicker1.ReadOnly = true;
            uiDatetimePicker1.Size = new Size(255, 29);
            uiDatetimePicker1.SymbolDropDown = 61555;
            uiDatetimePicker1.SymbolNormal = 61555;
            uiDatetimePicker1.SymbolSize = 24;
            uiDatetimePicker1.TabIndex = 13;
            uiDatetimePicker1.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatetimePicker1.Watermark = "";
            // 
            uiLabelFechaEst.Font = fuenteLabel;
            uiLabelFechaEst.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelFechaEst.Location = new Point(371, 120);
            uiLabelFechaEst.Name = "uiLabelFechaEst";
            uiLabelFechaEst.Size = new Size(100, 23);
            uiLabelFechaEst.TabIndex = 14;
            uiLabelFechaEst.Text = "Fecha Est :";
            // 
            uiDatetimePicker2.DateCultureInfo = new CultureInfo("es-DO");
            uiDatetimePicker2.FillColor = Color.White;
            uiDatetimePicker2.Font = fuenteValor;
            uiDatetimePicker2.Location = new Point(499, 114);
            uiDatetimePicker2.Margin = new Padding(4, 5, 4, 5);
            uiDatetimePicker2.MaxLength = 19;
            uiDatetimePicker2.MinimumSize = new Size(63, 0);
            uiDatetimePicker2.Name = "uiDatetimePicker2";
            uiDatetimePicker2.ReadOnly = true;
            uiDatetimePicker2.Size = new Size(248, 29);
            uiDatetimePicker2.SymbolDropDown = 61555;
            uiDatetimePicker2.SymbolNormal = 61555;
            uiDatetimePicker2.SymbolSize = 24;
            uiDatetimePicker2.TabIndex = 15;
            uiDatetimePicker2.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatetimePicker2.Watermark = "";
            // 
            uiLabelPrioridad.Font = fuenteLabel;
            uiLabelPrioridad.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelPrioridad.Location = new Point(371, 160);
            uiLabelPrioridad.Name = "uiLabelPrioridad";
            uiLabelPrioridad.Size = new Size(92, 23);
            uiLabelPrioridad.TabIndex = 16;
            uiLabelPrioridad.Text = "prioridad :";
            // 
            cbo_prioridad.DataSource = null;
            cbo_prioridad.FillColor = Color.White;
            cbo_prioridad.Font = fuenteValor;
            cbo_prioridad.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cbo_prioridad.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cbo_prioridad.Location = new Point(469, 154);
            cbo_prioridad.Margin = new Padding(4, 5, 4, 5);
            cbo_prioridad.MinimumSize = new Size(63, 0);
            cbo_prioridad.Name = "cbo_prioridad";
            cbo_prioridad.Padding = new Padding(0, 0, 30, 2);
            cbo_prioridad.ReadOnly = true;
            cbo_prioridad.Size = new Size(278, 29);
            cbo_prioridad.SymbolSize = 24;
            cbo_prioridad.TabIndex = 17;
            cbo_prioridad.TextAlignment = ContentAlignment.MiddleLeft;
            cbo_prioridad.Watermark = "";
            // 
            // Direcciones: Bill To (facturacion) + Ship To (entrega)
            // 
            uiLabelBillTo.Font = fuenteLabel;
            uiLabelBillTo.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelBillTo.Location = new Point(8, 160);
            uiLabelBillTo.Name = "uiLabelBillTo";
            uiLabelBillTo.Size = new Size(88, 23);
            uiLabelBillTo.TabIndex = 18;
            uiLabelBillTo.Text = "Bill To :";
            // 
            uiRichTextBox1.FillColor = Color.White;
            uiRichTextBox1.Font = fuenteValor;
            uiRichTextBox1.Location = new Point(113, 154);
            uiRichTextBox1.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox1.MinimumSize = new Size(1, 1);
            uiRichTextBox1.Name = "uiRichTextBox1";
            uiRichTextBox1.Padding = new Padding(2);
            uiRichTextBox1.ReadOnly = true;
            uiRichTextBox1.ShowText = false;
            uiRichTextBox1.Size = new Size(248, 92);
            uiRichTextBox1.TabIndex = 19;
            uiRichTextBox1.Text = "Bill To:";
            uiRichTextBox1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            uiLabelShipTo.Font = fuenteLabel;
            uiLabelShipTo.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelShipTo.Location = new Point(371, 160);
            uiLabelShipTo.Name = "uiLabelShipTo";
            uiLabelShipTo.Size = new Size(88, 23);
            uiLabelShipTo.TabIndex = 20;
            uiLabelShipTo.Text = "Ship To :";
            // 
            uiRichTextBox2.FillColor = Color.White;
            uiRichTextBox2.Font = fuenteValor;
            uiRichTextBox2.Location = new Point(469, 154);
            uiRichTextBox2.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox2.MinimumSize = new Size(1, 1);
            uiRichTextBox2.Name = "uiRichTextBox2";
            uiRichTextBox2.Padding = new Padding(2);
            uiRichTextBox2.ReadOnly = true;
            uiRichTextBox2.ShowText = false;
            uiRichTextBox2.Size = new Size(278, 92);
            uiRichTextBox2.TabIndex = 21;
            uiRichTextBox2.Text = "Ship To:";
            uiRichTextBox2.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // Condiciones Pago, Persona Contacto
            // 
            uiLabelItbisPor2.Font = fuenteLabel;
            uiLabelItbisPor2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelItbisPor2.Location = new Point(8, 260);
            uiLabelItbisPor2.Name = "uiLabelItbisPor2";
            uiLabelItbisPor2.Size = new Size(100, 23);
            uiLabelItbisPor2.TabIndex = 22;
            uiLabelItbisPor2.Text = "Cond. Pago :";
            // 
            cboCondicionesPago.DataSource = null;
            cboCondicionesPago.FillColor = Color.White;
            cboCondicionesPago.Font = fuenteValor;
            cboCondicionesPago.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cboCondicionesPago.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cboCondicionesPago.Location = new Point(113, 254);
            cboCondicionesPago.Margin = new Padding(4, 5, 4, 5);
            cboCondicionesPago.MinimumSize = new Size(63, 0);
            cboCondicionesPago.Name = "cboCondicionesPago";
            cboCondicionesPago.Padding = new Padding(0, 0, 30, 2);
            cboCondicionesPago.ReadOnly = true;
            cboCondicionesPago.Size = new Size(255, 29);
            cboCondicionesPago.SymbolSize = 24;
            cboCondicionesPago.TabIndex = 23;
            cboCondicionesPago.TextAlignment = ContentAlignment.MiddleLeft;
            cboCondicionesPago.Watermark = "";
            // 
            uiLabelItbisPor2.Text = "Cond. Pago :";
            // 
            txtPersonaContacto.Font = fuenteValor;
            txtPersonaContacto.Location = new Point(371, 254);
            txtPersonaContacto.Margin = new Padding(4, 5, 4, 5);
            txtPersonaContacto.MinimumSize = new Size(1, 16);
            txtPersonaContacto.Name = "txtPersonaContacto";
            txtPersonaContacto.Padding = new Padding(5);
            txtPersonaContacto.ReadOnly = true;
            txtPersonaContacto.ShowText = false;
            txtPersonaContacto.Size = new Size(233, 29);
            txtPersonaContacto.TabIndex = 24;
            txtPersonaContacto.TextAlignment = ContentAlignment.MiddleLeft;
            txtPersonaContacto.Watermark = "Persona Contacto";
            // 
            // Notas
            // 
            uiLabelNotas.Font = fuenteLabel;
            uiLabelNotas.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelNotas.Location = new Point(8, 300);
            uiLabelNotas.Name = "uiLabelNotas";
            uiLabelNotas.Size = new Size(100, 23);
            uiLabelNotas.TabIndex = 25;
            uiLabelNotas.Text = "Notas :";
            // 
            uiRichTextBox3.FillColor = Color.White;
            uiRichTextBox3.Font = fuenteValor;
            uiRichTextBox3.Location = new Point(113, 294);
            uiRichTextBox3.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox3.MinimumSize = new Size(1, 1);
            uiRichTextBox3.Name = "uiRichTextBox3";
            uiRichTextBox3.Padding = new Padding(2);
            uiRichTextBox3.ReadOnly = true;
            uiRichTextBox3.ShowText = false;
            uiRichTextBox3.Size = new Size(634, 92);
            uiRichTextBox3.TabIndex = 26;
            uiRichTextBox3.Text = "Comentario :";
            uiRichTextBox3.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // Detalle grid + line editor (same layout as FrmPedidos)
            // 
            uiDataGridView1.AllowUserToAddRows = false;
            styleDetalle.Alignment = DataGridViewContentAlignment.MiddleLeft;
            styleDetalle.BackColor = Color.White;
            styleDetalle.Font = new Font("JetBrains Mono", 9F);
            styleDetalle.ForeColor = Color.FromArgb(48, 48, 48);
            styleDetalle.SelectionBackColor = Color.FromArgb(110, 190, 40);
            styleDetalle.SelectionForeColor = Color.White;
            styleDetalle.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.AlternatingRowsDefaultCellStyle = styleDetalle;
            uiDataGridView1.BackgroundColor = Color.White;
            uiDataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            styleEncabezado.Alignment = DataGridViewContentAlignment.MiddleCenter;
            styleEncabezado.BackColor = Color.FromArgb(110, 190, 40);
            styleEncabezado.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            styleEncabezado.ForeColor = Color.White;
            styleEncabezado.SelectionBackColor = Color.FromArgb(110, 190, 40);
            styleEncabezado.SelectionForeColor = Color.White;
            styleEncabezado.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.ColumnHeadersDefaultCellStyle = styleEncabezado;
            uiDataGridView1.ColumnHeadersHeight = 32;
            uiDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            uiDataGridView1.Columns.AddRange(new DataGridViewColumn[] { renglon, Description, Unit, Qty, Notes, Price, widthCol, lenghtCol, subtotalCol });
            uiDataGridView1.DefaultCellStyle = styleCuerpo;
            uiDataGridView1.EnableHeadersVisualStyles = false;
            uiDataGridView1.Font = new Font("JetBrains Mono", 9F);
            uiDataGridView1.GridColor = Color.FromArgb(180, 210, 180);
            uiDataGridView1.Location = new Point(8, 420);
            uiDataGridView1.Name = "uiDataGridView1";
            uiDataGridView1.ReadOnly = true;
            uiDataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            styleDetalle.BackColor = Color.FromArgb(245, 250, 240);
            styleDetalle.Font = new Font("JetBrains Mono", 9F);
            uiDataGridView1.AlternatingRowsDefaultCellStyle = styleDetalle;
            uiDataGridView1.RowHeadersVisible = false;
            uiDataGridView1.Size = new Size(745, 120);
            uiDataGridView1.StripeOddColor = Color.FromArgb(245, 250, 240);
            uiDataGridView1.TabIndex = 15;
            // 
            // renglon
            // 
            renglon.HeaderText = "Id. Pro.";
            renglon.Name = "renglon";
            renglon.ReadOnly = true;
            renglon.Width = 80;
            // 
            // Description
            // 
            Description.HeaderText = "Product Name";
            Description.Name = "Description";
            Description.ReadOnly = true;
            Description.Width = 280;
            // 
            // Unit
            // 
            Unit.HeaderText = "Unit";
            Unit.Name = "Unit";
            Unit.ReadOnly = true;
            Unit.Width = 50;
            // 
            // Qty
            // 
            Qty.HeaderText = "Qty";
            Qty.Name = "Qty";
            Qty.ReadOnly = true;
            // 
            // Notes
            // 
            Notes.HeaderText = "Notes";
            Notes.Name = "Notes";
            Notes.ReadOnly = true;
            // 
            // Price
            // 
            Price.HeaderText = "Price";
            Price.Name = "Price";
            Price.ReadOnly = true;
            // 
            // widthCol
            // 
            widthCol.HeaderText = "Width";
            widthCol.Name = "widthCol";
            widthCol.ReadOnly = true;
            widthCol.Width = 80;
            // 
            // lenghtCol
            // 
            lenghtCol.HeaderText = "Length";
            lenghtCol.Name = "lenghtCol";
            lenghtCol.ReadOnly = true;
            lenghtCol.Width = 80;
            // 
            // subtotalCol
            // 
            subtotalCol.HeaderText = "total";
            subtotalCol.Name = "subtotalCol";
            subtotalCol.ReadOnly = true;
            subtotalCol.Width = 150;
            // 
            // Line editor controls
            // 
            uiLabelProduct.Font = fuenteLabel;
            uiLabelProduct.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelProduct.Location = new Point(134, 360);
            uiLabelProduct.Name = "uiLabelProduct";
            uiLabelProduct.Size = new Size(81, 23);
            uiLabelProduct.TabIndex = 14;
            uiLabelProduct.Text = "Product :";
            // 
            uiComboBox2.DataSource = null;
            uiComboBox2.FillColor = Color.White;
            uiComboBox2.Font = fuenteValor;
            uiComboBox2.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox2.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox2.Location = new Point(220, 354);
            uiComboBox2.Margin = new Padding(4, 5, 4, 5);
            uiComboBox2.MinimumSize = new Size(63, 0);
            uiComboBox2.Name = "uiComboBox2";
            uiComboBox2.Padding = new Padding(0, 0, 30, 2);
            uiComboBox2.ReadOnly = true;
            uiComboBox2.Size = Size = new Size(255, 29);
            uiComboBox2.SymbolSize = 24;
            uiComboBox2.TabIndex = 2;
            uiComboBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox2.Watermark = "";
            // 
            uiLabelQty.Font = fuenteLabel;
            uiLabelQty.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelQty.Location = new Point(6, 400);
            uiLabelQty.Name = "uiLabelQty";
            uiLabelQty.Size = new Size(81, 23);
            uiLabelQty.TabIndex = 13;
            uiLabelQty.Text = "Quantity :";
            // 
            uiTextBox3.Font = fuenteValor;
            uiTextBox3.Location = new Point(80, 394);
            uiTextBox3.Margin = new Padding(4, 5, 4, 5);
            uiTextBox3.MinimumSize = new Size(1, 16);
            uiTextBox3.Name = "uiTextBox3";
            uiTextBox3.Padding = new Padding(5);
            uiTextBox3.ReadOnly = true;
            uiTextBox3.ShowText = false;
            uiTextBox3.Size = new Size(47, 29);
            uiTextBox3.TabIndex = 8;
            uiTextBox3.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox3.Watermark = "";
            // 
            uiLabelProduct.Location = new Point(134, 360);
            // 
            // width
            // 
            txt_width.Font = fuenteValor;
            txt_width.Location = new Point(466, 384);
            txt_width.Margin = new Padding(4, 5, 4, 5);
            txt_width.MinimumSize = new Size(1, 16);
            txt_width.Name = "txt_width";
            txt_width.Padding = new Padding(5);
            txt_width.ReadOnly = true;
            txt_width.ShowText = false;
            txt_width.Size = new Size(85, 29);
            txt_width.TabIndex = 29;
            txt_width.TextAlignment = ContentAlignment.MiddleLeft;
            txt_width.Watermark = "Width (in)";
            // 
            // length
            // 
            txt_length.Font = fuenteValor;
            txt_length.Location = new Point(559, 384);
            txt_length.Margin = new Padding(4, 5, 4, 5);
            txt_length.MinimumSize = new Size(1, 16);
            txt_length.Name = "txt_length";
            txt_length.Padding = new Padding(5);
            txt_length.ReadOnly = true;
            txt_length.ShowText = false;
            txt_length.Size = new Size(85, 29);
            txt_length.TabIndex = 28;
            txt_length.TextAlignment = ContentAlignment.MiddleLeft;
            txt_length.Watermark = "Length (ft)";
            // 
            // Price editor
            // 
            uiLabelItbisPor.Font = fuenteLabel;
            uiLabelItbisPor.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelItbisPor.Location = new Point(466, 400);
            uiLabelItbisPor.Name = "uiLabelItbisPor";
            uiLabelItbisPor.Size = new Size(73, 23);
            uiLabelItbisPor.TabIndex = 23;
            uiLabelItbisPor.Text = "Price :";
            // 
            uiTextBox8.Font = fuenteValor;
            uiTextBox8.Location = new Point(546, 394);
            uiTextBox8.Margin = new Padding(4, 5, 4, 5);
            uiTextBox8.MinimumSize = new Size(1, 16);
            uiTextBox8.Name = "uiTextBox8";
            uiTextBox8.Padding = new Padding(5);
            uiTextBox8.ReadOnly = true;
            uiTextBox8.ShowText = false;
            uiTextBox8.Size = new Size(85, 29);
            uiTextBox8.TabIndex = 26;
            uiTextBox8.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox8.Watermark = "Precio";
            // 
            // Notes editor
            // 
            uiLabelNotas.Font = fuenteLabel;
            uiLabelNotas.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelNotas.Location = new Point(638, 360);
            uiLabelNotas.Name = "uiLabelNotas";
            uiLabelNotas.Size = new Size(56, 23);
            uiLabelNotas.TabIndex = 25;
            uiLabelNotas.Text = "Notas :";
            // 
            uiTextBox9.Font = fuenteValor;
            uiTextBox9.Location = new Point(699, 354);
            uiTextBox9.Margin = new Padding(4, 5, 4, 5);
            uiTextBox9.MinimumSize = new Size(1, 16);
            uiTextBox9.Name = "uiTextBox9";
            uiTextBox9.Padding = new Padding(5);
            uiTextBox9.ReadOnly = true;
            uiTextBox9.ShowText = false;
            uiTextBox9.Size = new Size(82, 29);
            uiTextBox9.TabIndex = 27;
            uiTextBox9.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox9.Watermark = "Notas";
            // 
            // Botones de línea
            // 
            btnBuscarProducto.BackColor = Color.Transparent;
            btnBuscarProducto.Enabled = false;
            btnBuscarProducto.FlatAppearance.BorderSize = 0;
            btnBuscarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnBuscarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnBuscarProducto.FlatStyle = FlatStyle.Flat;
            btnBuscarProducto.Image = Properties.Resources.search_property_24px;
            btnBuscarProducto.Location = new Point(868, 250);
            btnBuscarProducto.Margin = new Padding(3, 4, 3, 4);
            btnBuscarProducto.Name = "btnBuscarProducto";
            btnBuscarProducto.Size = new Size(27, 35);
            btnBuscarProducto.TabIndex = 17;
            btnBuscarProducto.UseVisualStyleBackColor = false;
            // 
            btnAddProducto.BackColor = Color.Transparent;
            btnAddProducto.Enabled = false;
            btnAddProducto.FlatAppearance.BorderSize = 0;
            btnAddProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnAddProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnAddProducto.FlatStyle = FlatStyle.Flat;
            btnAddProducto.Image = Properties.Resources.plus_24px;
            btnAddProducto.Location = new Point(838, 384);
            btnAddProducto.Margin = new Padding(3, 4, 3, 4);
            btnAddProducto.Name = "btnAddProducto";
            btnAddProducto.Size = new Size(28, 35);
            btnAddProducto.TabIndex = 16;
            btnAddProducto.UseVisualStyleBackColor = false;
            // 
            btnEliminarProducto.BackColor = Color.Transparent;
            btnEliminarProducto.Enabled = false;
            btnEliminarProducto.FlatAppearance.BorderSize = 0;
            btnEliminarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnEliminarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnEliminarProducto.FlatStyle = FlatStyle.Flat;
            btnEliminarProducto.Image = Properties.Resources.delete_row_24px;
            btnEliminarProducto.Location = new Point(878, 516);
            btnEliminarProducto.Margin = new Padding(3, 4, 3, 4);
            btnEliminarProducto.Name = "btnEliminarProducto";
            btnEliminarProducto.Size = new Size(30, 27);
            btnEliminarProducto.TabIndex = 19;
            btnEliminarProducto.UseVisualStyleBackColor = false;
            // 
            btnEditarProducto.BackColor = Color.Transparent;
            btnEditarProducto.Enabled = false;
            btnEditarProducto.FlatAppearance.BorderSize = 0;
            btnEditarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnEditarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnEditarProducto.FlatStyle = FlatStyle.Flat;
            btnEditarProducto.Image = Properties.Resources.edit_24px;
            btnEditarProducto.Location = new Point(878, 488);
            btnEditarProducto.Margin = new Padding(3, 4, 3, 4);
            btnEditarProducto.Name = "btnEditarProducto";
            btnEditarProducto.Size = new Size(27, 35);
            btnEditarProducto.TabIndex = 18;
            btnEditarProducto.UseVisualStyleBackColor = false;
            // 
            // Totales section
            // 
            uiLabelSubtotal.Font = fuenteLabel;
            uiLabelSubtotal.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelSubtotal.Location = new Point(562, 560);
            uiLabelSubtotal.Name = "uiLabelSubtotal";
            uiLabelSubtotal.Size = new Size(92, 23);
            uiLabelSubtotal.TabIndex = 20;
            uiLabelSubtotal.Text = "Sub-Total :";
            // 
            uiTextBox4.Font = fuenteValor;
            uiTextBox4.Location = new Point(660, 554);
            uiTextBox4.Margin = new Padding(4, 5, 4, 5);
            uiTextBox4.MinimumSize = new Size(1, 16);
            uiTextBox4.Name = "uiTextBox4";
            uiTextBox4.Padding = new Padding(5);
            uiTextBox4.ReadOnly = true;
            uiTextBox4.ShowText = false;
            uiTextBox4.Size = new Size(120, 29);
            uiTextBox4.TabIndex = 9;
            uiTextBox4.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox4.Watermark = "";
            // 
            uiLabelItbis.Font = fuenteLabel;
            uiLabelItbis.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelItbis.Location = new Point(562, 584);
            uiLabelItbis.Name = "uiLabelItbis";
            uiLabelItbis.Size = new Size(72, 23);
            uiLabelItbis.TabIndex = 21;
            uiLabelItbis.Text = "Itbis :";
            // 
            uiTextBox5.Font = fuenteValor;
            uiTextBox5.Location = new Point(640, 584);
            uiTextBox5.Margin = new Padding(4, 5, 4, 5);
            uiTextBox5.MinimumSize = new Size(1, 16);
            uiTextBox5.Name = "uiTextBox5";
            uiTextBox5.Padding = new Padding(5);
            uiTextBox5.ReadOnly = true;
            uiTextBox5.ShowText = false;
            uiTextBox5.Size = new Size(120, 29);
            uiTextBox5.TabIndex = 10;
            uiTextBox5.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox5.Watermark = "";
            // 
            uiLabelTotal.Font = fuenteLabel;
            uiLabelTotal.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelTotal.Location = new Point(562, 610);
            uiLabelTotal.Name = "uiLabelTotal";
            uiLabelTotal.Size = new Size(60, 23);
            uiLabelTotal.TabIndex = 22;
            uiLabelTotal.Text = "Total :";
            // 
            uiTextBox6.Font = fuenteValor;
            uiTextBox6.Location = new Point(628, 610);
            uiTextBox6.Margin = new Padding(4, 5, 4, 5);
            uiTextBox6.MinimumSize = new Size(1, 16);
            uiTextBox6.Name = "uiTextBox6";
            uiTextBox6.Padding = new Padding(5);
            uiTextBox6.ReadOnly = true;
            uiTextBox6.ShowText = false;
            uiTextBox6.Size = new Size(136, 29);
            uiTextBox6.TabIndex = 11;
            uiTextBox6.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox6.Watermark = "";
            // 
            uiLabelTotalCant.Font = fuenteLabel;
            uiLabelTotalCant.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelTotalCant.Location = new Point(466, 560);
            uiLabelTotalCant.Name = "uiLabelTotalCant";
            uiLabelTotalCant.Size = new Size(110, 23);
            uiLabelTotalCant.TabIndex = 36;
            uiLabelTotalCant.Text = "Total Cantidad :";
            // 
            txt_total_cantidad.Font = fuenteValor;
            txt_total_cantidad.Location = new Point(582, 554);
            txt_total_cantidad.Margin = new Padding(4, 5, 4, 5);
            txt_total_cantidad.MinimumSize = new Size(1, 16);
            txt_total_cantidad.Name = "txt_total_cantidad";
            txt_total_cantidad.Padding = new Padding(5);
            txt_total_cantidad.ReadOnly = true;
            txt_total_cantidad.ShowText = false;
            txt_total_cantidad.Size = new Size(78, 29);
            txt_total_cantidad.TabIndex = 26;
            txt_total_cantidad.TextAlignment = ContentAlignment.MiddleLeft;
            txt_total_cantidad.Watermark = "";
            // 
            // uiTextBox7 (porc_itbis)
            // 
            uiTextBox7.Font = fuenteValor;
            uiTextBox7.Location = new Point(668, 524);
            uiTextBox7.Margin = new Padding(4, 5, 4, 5);
            uiTextBox7.MinimumSize = new Size(1, 16);
            uiTextBox7.Name = "uiTextBox7";
            uiTextBox7.Padding = new Padding(5);
            uiTextBox7.ReadOnly = true;
            uiTextBox7.ShowText = false;
            uiTextBox7.Size = new Size(49, 29);
            uiTextBox7.TabIndex = 25;
            uiTextBox7.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox7.Watermark = "";
            // 
            uiLabelItbisPor.Font = fuenteLabel;
            uiLabelItbisPor.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabelItbisPor.Location = new Point(578, 528);
            uiLabelItbisPor.Name = "uiLabelItbisPor";
            uiLabelItbisPor.Size = new Size(73, 23);
            uiLabelItbisPor.TabIndex = 23;
            uiLabelItbisPor.Text = "Itbis % :";
            // 
            // FrmOrdenesCompra
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1150, 650);
            Controls.Add(tlpRoot);
            MinimumSize = new Size(980, 560);
            Name = "FrmOrdenesCompra";
            Text = "Órdenes de Compra";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 650);
            tlpRoot.ResumeLayout(false);
            panelIzq.ResumeLayout(false);
            pnlBuscadorOc.ResumeLayout(false);
            pnlResumen.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridOrdenes).EndInit();
            panelDer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).EndInit();
            tabDetalle.ResumeLayout(false);
            tabGeneral.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpRoot;
        private Panel panelIzq;
        private Panel pnlBuscadorOc;
        private Sunny.UI.UILabel lblTitulo;
        private Sunny.UI.UITextBox txtBuscar;
        private Panel pnlResumen;
        private Sunny.UI.UILabel lblResumen;
        private Sunny.UI.UIDataGridView gridOrdenes;
        private DataGridViewTextBoxColumn colNumero;
        private DataGridViewTextBoxColumn colProveedor;
        private DataGridViewTextBoxColumn colEstado;
        private Panel panelDer;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabGeneral;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevo;
        private ToolStripButton btnEditar;
        private ToolStripButton btnGuardar;
        private ToolStripButton btnCancelar;
        private Sunny.UI.UIDatetimePicker uiDatetimePicker1;
        private Sunny.UI.UIDatetimePicker uiDatetimePicker2;
        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UIComboBox cbo_proveedores;
        private Sunny.UI.UIComboBox cboCondicionesPago;
        private Sunny.UI.UIComboBox cbo_prioridad;
        private Sunny.UI.UITextBox uiTextBox2;
        private Sunny.UI.UITextBox uiTextBox3;
        private Sunny.UI.UITextBox uiTextBox4;
        private Sunny.UI.UITextBox uiTextBox5;
        private Sunny.UI.UITextBox uiTextBox6;
        private Sunny.UI.UITextBox uiTextBox7;
        private Sunny.UI.UITextBox uiTextBox8;
        private Sunny.UI.UITextBox uiTextBox9;
        private Sunny.UI.UITextBox txt_id_proveedor;
        private Sunny.UI.UITextBox txtPersonaContacto;
        private Sunny.UI.UITextBox txt_width;
        private Sunny.UI.UITextBox txt_length;
        private Sunny.UI.UITextBox txt_total_cantidad;
        private Sunny.UI.UIRichTextBox uiRichTextBox1;
        private Sunny.UI.UIRichTextBox uiRichTextBox2;
        private Sunny.UI.UIRichTextBox uiRichTextBox3;
        private Sunny.UI.UILabel uiLabelProveedor;
        private Sunny.UI.UILabel uiLabelNumero;
        private Sunny.UI.UILabel uiLabelIdProv;
        private Sunny.UI.UILabel uiLabelStatus;
        private Sunny.UI.UILabel uiLabelFechaReg;
        private Sunny.UI.UILabel uiLabelFechaEst;
        private Sunny.UI.UILabel uiLabelBillTo;
        private Sunny.UI.UILabel uiLabelShipTo;
        private Sunny.UI.UILabel uiLabelItbisPor2;
        private Sunny.UI.UILabel uiLabelItbisPor;
        private Sunny.UI.UILabel uiLabelNotas;
        private Sunny.UI.UILabel uiLabelProduct;
        private Sunny.UI.UILabel uiLabelQty;
        private Sunny.UI.UILabel uiLabelPrioridad;
        private Sunny.UI.UILabel uiLabelSubtotal;
        private Sunny.UI.UILabel uiLabelItbis;
        private Sunny.UI.UILabel uiLabelTotal;
        private Sunny.UI.UILabel uiLabelTotalCant;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private DataGridViewTextBoxColumn renglon;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn Unit;
        private DataGridViewTextBoxColumn Qty;
        private DataGridViewTextBoxColumn Notes;
        private DataGridViewTextBoxColumn Price;
        private DataGridViewTextBoxColumn widthCol;
        private DataGridViewTextBoxColumn lenghtCol;
        private DataGridViewTextBoxColumn subtotalCol;
        private Sunny.UI.UIComboBox uiComboBox2;
        private Button btnBuscarProducto;
        private Button btnAddProducto;
        private Button btnEliminarProducto;
        private Button btnEditarProducto;
        private Sunny.UI.UISwitch sw_anular_oc;
        private Sunny.UI.UILine uiLine1;
    }
}
