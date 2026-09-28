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
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            panelTitulo = new Panel();
            lblTitulo = new Label();
            sales_orders_search = new Panel();
            panelContador = new Panel();
            lblContador = new Label();
            gridPedidos = new DataGridView();
            panelBuscar = new Panel();
            txtBuscarPedido = new TextBox();
            picLupa = new PictureBox();
            btnLimpiarBusqueda = new Button();
            lblBuscar = new Label();
            sales_orders_tabs = new TabControl();
            tabGeneral = new TabPage();
            uiLabel21 = new Sunny.UI.UILabel();
            txt_total_cantidad = new Sunny.UI.UITextBox();
            uiLabel20 = new Sunny.UI.UILabel();
            uiLabel19 = new Sunny.UI.UILabel();
            uiLabel18 = new Sunny.UI.UILabel();
            uiLabel17 = new Sunny.UI.UILabel();
            uiLabel13 = new Sunny.UI.UILabel();
            uiTextBox7 = new Sunny.UI.UITextBox();
            uiLabel14 = new Sunny.UI.UILabel();
            cbo_prioridad = new Sunny.UI.UIComboBox();
            uiLabel16 = new Sunny.UI.UILabel();
            txt_id_vendor = new Sunny.UI.UITextBox();
            uiLabel15 = new Sunny.UI.UILabel();
            txt_id_cust = new Sunny.UI.UITextBox();
            txtPersonaContacto = new Sunny.UI.UITextBox();
            cboCondicionesPago = new Sunny.UI.UIComboBox();
            cboTipoVenta = new Sunny.UI.UIComboBox();
            uiRichTextBox3 = new Sunny.UI.UIRichTextBox();
            uiLabel12 = new Sunny.UI.UILabel();
            uiLabel11 = new Sunny.UI.UILabel();
            uiLabel10 = new Sunny.UI.UILabel();
            uiTextBox6 = new Sunny.UI.UITextBox();
            uiTextBox5 = new Sunny.UI.UITextBox();
            uiTextBox4 = new Sunny.UI.UITextBox();
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
            subtotal = new DataGridViewTextBoxColumn();
            uiComboBox2 = new Sunny.UI.UIComboBox();
            uiTextBox9 = new Sunny.UI.UITextBox();
            uiTextBox8 = new Sunny.UI.UITextBox();
            uiLabel9 = new Sunny.UI.UILabel();
            uiTextBox3 = new Sunny.UI.UITextBox();
            uiLabel8 = new Sunny.UI.UILabel();
            uiLabel7 = new Sunny.UI.UILabel();
            uiRichTextBox2 = new Sunny.UI.UIRichTextBox();
            uiRichTextBox1 = new Sunny.UI.UIRichTextBox();
            uiLabel6 = new Sunny.UI.UILabel();
            uiTextBox2 = new Sunny.UI.UITextBox();
            uiLabel5 = new Sunny.UI.UILabel();
            uiComboBox1 = new Sunny.UI.UIComboBox();
            uiTextBox1 = new Sunny.UI.UITextBox();
            uiLabel4 = new Sunny.UI.UILabel();
            uiLabel3 = new Sunny.UI.UILabel();
            uiDatetimePicker2 = new Sunny.UI.UIDatetimePicker();
            uiDatetimePicker1 = new Sunny.UI.UIDatetimePicker();
            uiLabel2 = new Sunny.UI.UILabel();
            cbo_customers = new Sunny.UI.UIComboBox();
            uiLabel1 = new Sunny.UI.UILabel();
            barraHerramientas = new ToolStrip();
            btnNuevo = new ToolStripButton();
            btnEditar = new ToolStripButton();
            btnGuardar = new ToolStripButton();
            btnCancelar = new ToolStripButton();
            panelTitulo.SuspendLayout();
            sales_orders_search.SuspendLayout();
            panelContador.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridPedidos).BeginInit();
            panelBuscar.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)picLupa).BeginInit();
            sales_orders_tabs.SuspendLayout();
            tabGeneral.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).BeginInit();
            barraHerramientas.SuspendLayout();
            SuspendLayout();
            // 
            // panelTitulo
            // 
            panelTitulo.BackColor = Color.FromArgb(190, 225, 150);
            panelTitulo.Controls.Add(lblTitulo);
            panelTitulo.Dock = DockStyle.Top;
            panelTitulo.Location = new Point(0, 35);
            panelTitulo.Name = "panelTitulo";
            panelTitulo.Size = new Size(1264, 60);
            panelTitulo.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("Segoe UI", 18F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(70, 140, 25);
            lblTitulo.Location = new Point(0, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(1264, 60);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Pedidos de Cliente";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // sales_orders_search
            // 
            sales_orders_search.BackColor = Color.FromArgb(240, 250, 230);
            sales_orders_search.Controls.Add(panelContador);
            sales_orders_search.Controls.Add(gridPedidos);
            sales_orders_search.Controls.Add(panelBuscar);
            sales_orders_search.Dock = DockStyle.Left;
            sales_orders_search.Location = new Point(0, 135);
            sales_orders_search.Name = "sales_orders_search";
            sales_orders_search.Size = new Size(345, 623);
            sales_orders_search.TabIndex = 1;
            // 
            // panelContador
            // 
            panelContador.BackColor = Color.FromArgb(100, 170, 80);
            panelContador.Controls.Add(lblContador);
            panelContador.Dock = DockStyle.Bottom;
            panelContador.Location = new Point(0, 583);
            panelContador.Name = "panelContador";
            panelContador.Size = new Size(345, 40);
            panelContador.TabIndex = 2;
            // 
            // lblContador
            // 
            lblContador.BackColor = Color.FromArgb(100, 170, 80);
            lblContador.Dock = DockStyle.Fill;
            lblContador.Font = new Font("Segoe UI", 10F, FontStyle.Bold);
            lblContador.ForeColor = Color.White;
            lblContador.Location = new Point(0, 0);
            lblContador.Name = "lblContador";
            lblContador.Padding = new Padding(8, 0, 0, 0);
            lblContador.Size = new Size(345, 40);
            lblContador.TabIndex = 0;
            lblContador.Text = "0 pedidos";
            lblContador.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // gridPedidos
            // 
            gridPedidos.AllowUserToAddRows = false;
            gridPedidos.AllowUserToDeleteRows = false;
            gridPedidos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridPedidos.BackgroundColor = Color.White;
            gridPedidos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.AutoSize;
            gridPedidos.Dock = DockStyle.Fill;
            gridPedidos.GridColor = Color.FromArgb(200, 220, 180);
            gridPedidos.Location = new Point(0, 56);
            gridPedidos.MultiSelect = false;
            gridPedidos.Name = "gridPedidos";
            gridPedidos.ReadOnly = true;
            gridPedidos.RowHeadersVisible = false;
            gridPedidos.Size = new Size(345, 567);
            gridPedidos.TabIndex = 1;
            // 
            // panelBuscar
            // 
            panelBuscar.BackColor = Color.FromArgb(240, 250, 230);
            panelBuscar.Controls.Add(txtBuscarPedido);
            panelBuscar.Controls.Add(picLupa);
            panelBuscar.Controls.Add(btnLimpiarBusqueda);
            panelBuscar.Controls.Add(lblBuscar);
            panelBuscar.Dock = DockStyle.Top;
            panelBuscar.Location = new Point(0, 0);
            panelBuscar.Name = "panelBuscar";
            panelBuscar.Size = new Size(345, 86);
            panelBuscar.TabIndex = 3;
            // 
            // txtBuscarPedido
            // 
            txtBuscarPedido.Dock = DockStyle.Fill;
            txtBuscarPedido.Location = new Point(26, 23);
            txtBuscarPedido.Name = "txtBuscarPedido";
            txtBuscarPedido.PlaceholderText = "Buscar por numero, cliente o estado...";
            txtBuscarPedido.Size = new Size(293, 26);
            txtBuscarPedido.TabIndex = 0;
            // 
            // picLupa
            // 
            picLupa.BackColor = Color.Transparent;
            picLupa.Dock = DockStyle.Left;
            picLupa.Image = Properties.Resources.search_property_24px;
            picLupa.Location = new Point(0, 23);
            picLupa.Name = "picLupa";
            picLupa.Size = new Size(26, 33);
            picLupa.SizeMode = PictureBoxSizeMode.CenterImage;
            picLupa.TabIndex = 4;
            picLupa.TabStop = false;
            // 
            // btnLimpiarBusqueda
            // 
            btnLimpiarBusqueda.BackColor = Color.Transparent;
            btnLimpiarBusqueda.Dock = DockStyle.Right;
            btnLimpiarBusqueda.FlatAppearance.BorderSize = 0;
            btnLimpiarBusqueda.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnLimpiarBusqueda.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnLimpiarBusqueda.FlatStyle = FlatStyle.Flat;
            btnLimpiarBusqueda.Image = Properties.Resources.cancel_24px;
            btnLimpiarBusqueda.Location = new Point(319, 23);
            btnLimpiarBusqueda.Margin = new Padding(3, 4, 3, 4);
            btnLimpiarBusqueda.Name = "btnLimpiarBusqueda";
            btnLimpiarBusqueda.Size = new Size(26, 33);
            btnLimpiarBusqueda.TabIndex = 5;
            btnLimpiarBusqueda.UseVisualStyleBackColor = false;
            // 
            // lblBuscar
            // 
            lblBuscar.Dock = DockStyle.Top;
            lblBuscar.Font = new Font("Segoe UI", 9F, FontStyle.Bold);
            lblBuscar.ForeColor = Color.FromArgb(100, 170, 80);
            lblBuscar.Location = new Point(0, 0);
            lblBuscar.Name = "lblBuscar";
            lblBuscar.Padding = new Padding(4, 0, 0, 0);
            lblBuscar.Size = new Size(345, 23);
            lblBuscar.TabIndex = 1;
            lblBuscar.Text = "Buscar por:";
            lblBuscar.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // sales_orders_tabs
            // 
            sales_orders_tabs.Controls.Add(tabGeneral);
            sales_orders_tabs.Dock = DockStyle.Fill;
            sales_orders_tabs.Location = new Point(345, 135);
            sales_orders_tabs.Name = "sales_orders_tabs";
            sales_orders_tabs.SelectedIndex = 0;
            sales_orders_tabs.Size = new Size(919, 623);
            sales_orders_tabs.TabIndex = 2;
            // 
            // tabGeneral
            // 
            tabGeneral.BackColor = Color.White;
            tabGeneral.Controls.Add(uiLabel21);
            tabGeneral.Controls.Add(txt_total_cantidad);
            tabGeneral.Controls.Add(uiLabel20);
            tabGeneral.Controls.Add(uiLabel19);
            tabGeneral.Controls.Add(uiLabel18);
            tabGeneral.Controls.Add(uiLabel17);
            tabGeneral.Controls.Add(uiLabel13);
            tabGeneral.Controls.Add(uiTextBox7);
            tabGeneral.Controls.Add(uiLabel14);
            tabGeneral.Controls.Add(cbo_prioridad);
            tabGeneral.Controls.Add(uiLabel16);
            tabGeneral.Controls.Add(txt_id_vendor);
            tabGeneral.Controls.Add(uiLabel15);
            tabGeneral.Controls.Add(txt_id_cust);
            tabGeneral.Controls.Add(txtPersonaContacto);
            tabGeneral.Controls.Add(cboCondicionesPago);
            tabGeneral.Controls.Add(cboTipoVenta);
            tabGeneral.Controls.Add(uiRichTextBox3);
            tabGeneral.Controls.Add(uiLabel12);
            tabGeneral.Controls.Add(uiLabel11);
            tabGeneral.Controls.Add(uiLabel10);
            tabGeneral.Controls.Add(uiTextBox6);
            tabGeneral.Controls.Add(uiTextBox5);
            tabGeneral.Controls.Add(uiTextBox4);
            tabGeneral.Controls.Add(btnBuscarProducto);
            tabGeneral.Controls.Add(btnAddProducto);
            tabGeneral.Controls.Add(btnEliminarProducto);
            tabGeneral.Controls.Add(btnEditarProducto);
            tabGeneral.Controls.Add(uiDataGridView1);
            tabGeneral.Controls.Add(uiComboBox2);
            tabGeneral.Controls.Add(uiTextBox9);
            tabGeneral.Controls.Add(uiTextBox8);
            tabGeneral.Controls.Add(uiLabel9);
            tabGeneral.Controls.Add(uiTextBox3);
            tabGeneral.Controls.Add(uiLabel8);
            tabGeneral.Controls.Add(uiLabel7);
            tabGeneral.Controls.Add(uiRichTextBox2);
            tabGeneral.Controls.Add(uiRichTextBox1);
            tabGeneral.Controls.Add(uiLabel6);
            tabGeneral.Controls.Add(uiTextBox2);
            tabGeneral.Controls.Add(uiLabel5);
            tabGeneral.Controls.Add(uiComboBox1);
            tabGeneral.Controls.Add(uiTextBox1);
            tabGeneral.Controls.Add(uiLabel4);
            tabGeneral.Controls.Add(uiLabel3);
            tabGeneral.Controls.Add(uiDatetimePicker2);
            tabGeneral.Controls.Add(uiDatetimePicker1);
            tabGeneral.Controls.Add(uiLabel2);
            tabGeneral.Controls.Add(cbo_customers);
            tabGeneral.Controls.Add(uiLabel1);
            tabGeneral.Location = new Point(4, 26);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Padding = new Padding(3);
            tabGeneral.Size = new Size(911, 593);
            tabGeneral.TabIndex = 0;
            tabGeneral.Text = "General";
            tabGeneral.UseVisualStyleBackColor = true;
            // 
            // uiLabel21
            // 
            uiLabel21.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel21.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel21.Location = new Point(288, 460);
            uiLabel21.Name = "uiLabel21";
            uiLabel21.Size = new Size(129, 23);
            uiLabel21.TabIndex = 36;
            uiLabel21.Text = "Total Cantidad :";
            // 
            // txt_total_cantidad
            // 
            txt_total_cantidad.Font = new Font("Microsoft Sans Serif", 12F);
            txt_total_cantidad.Location = new Point(424, 460);
            txt_total_cantidad.Margin = new Padding(4, 5, 4, 5);
            txt_total_cantidad.MinimumSize = new Size(1, 16);
            txt_total_cantidad.Name = "txt_total_cantidad";
            txt_total_cantidad.Padding = new Padding(5);
            txt_total_cantidad.ReadOnly = true;
            txt_total_cantidad.ShowText = false;
            txt_total_cantidad.Size = new Size(131, 29);
            txt_total_cantidad.TabIndex = 26;
            txt_total_cantidad.TextAlignment = ContentAlignment.MiddleLeft;
            txt_total_cantidad.Watermark = "";
            // 
            // uiLabel20
            // 
            uiLabel20.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel20.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel20.Location = new Point(8, 471);
            uiLabel20.Name = "uiLabel20";
            uiLabel20.Size = new Size(180, 23);
            uiLabel20.TabIndex = 35;
            uiLabel20.Text = "Notas y Comentarios :";
            // 
            // uiLabel19
            // 
            uiLabel19.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel19.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel19.Location = new Point(329, 118);
            uiLabel19.Name = "uiLabel19";
            uiLabel19.Size = new Size(88, 23);
            uiLabel19.TabIndex = 34;
            uiLabel19.Text = "Ship To :";
            // 
            // uiLabel18
            // 
            uiLabel18.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel18.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel18.Location = new Point(549, 210);
            uiLabel18.Name = "uiLabel18";
            uiLabel18.Size = new Size(92, 23);
            uiLabel18.TabIndex = 33;
            uiLabel18.Text = "Cond. Pago :";
            // 
            // uiLabel17
            // 
            uiLabel17.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel17.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel17.Location = new Point(566, 171);
            uiLabel17.Name = "uiLabel17";
            uiLabel17.Size = new Size(76, 23);
            uiLabel17.TabIndex = 32;
            uiLabel17.Text = "Tipo Vta :";
            // 
            // uiLabel13
            // 
            uiLabel13.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel13.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel13.Location = new Point(482, 511);
            uiLabel13.Name = "uiLabel13";
            uiLabel13.Size = new Size(73, 23);
            uiLabel13.TabIndex = 23;
            uiLabel13.Text = "Itbis % :";
            // 
            // uiTextBox7
            // 
            uiTextBox7.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox7.Location = new Point(562, 505);
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
            // uiLabel14
            // 
            uiLabel14.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel14.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel14.Location = new Point(562, 132);
            uiLabel14.Name = "uiLabel14";
            uiLabel14.Size = new Size(78, 23);
            uiLabel14.TabIndex = 31;
            uiLabel14.Text = "prioridad :";
            // 
            // cbo_prioridad
            // 
            cbo_prioridad.DataSource = null;
            cbo_prioridad.FillColor = Color.White;
            cbo_prioridad.Font = new Font("Microsoft Sans Serif", 12F);
            cbo_prioridad.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cbo_prioridad.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cbo_prioridad.Location = new Point(644, 130);
            cbo_prioridad.Margin = new Padding(4, 5, 4, 5);
            cbo_prioridad.MinimumSize = new Size(63, 0);
            cbo_prioridad.Name = "cbo_prioridad";
            cbo_prioridad.Padding = new Padding(0, 0, 30, 2);
            cbo_prioridad.ReadOnly = true;
            cbo_prioridad.Size = new Size(251, 29);
            cbo_prioridad.SymbolSize = 24;
            cbo_prioridad.TabIndex = 28;
            cbo_prioridad.TextAlignment = ContentAlignment.MiddleLeft;
            cbo_prioridad.Watermark = "";
            // 
            // uiLabel16
            // 
            uiLabel16.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel16.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel16.Location = new Point(704, 53);
            uiLabel16.Name = "uiLabel16";
            uiLabel16.Size = new Size(70, 23);
            uiLabel16.TabIndex = 30;
            uiLabel16.Text = "Id Ven :";
            // 
            // txt_id_vendor
            // 
            txt_id_vendor.Font = new Font("Microsoft Sans Serif", 12F);
            txt_id_vendor.Location = new Point(781, 47);
            txt_id_vendor.Margin = new Padding(4, 5, 4, 5);
            txt_id_vendor.MinimumSize = new Size(1, 16);
            txt_id_vendor.Name = "txt_id_vendor";
            txt_id_vendor.Padding = new Padding(5);
            txt_id_vendor.ReadOnly = true;
            txt_id_vendor.ShowText = false;
            txt_id_vendor.Size = new Size(114, 29);
            txt_id_vendor.TabIndex = 29;
            txt_id_vendor.TextAlignment = ContentAlignment.MiddleLeft;
            txt_id_vendor.Watermark = "";
            // 
            // uiLabel15
            // 
            uiLabel15.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel15.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel15.Location = new Point(372, 14);
            uiLabel15.Name = "uiLabel15";
            uiLabel15.Size = new Size(70, 23);
            uiLabel15.TabIndex = 28;
            uiLabel15.Text = "Id Cust :";
            uiLabel15.Click += uiLabel15_Click;
            // 
            // txt_id_cust
            // 
            txt_id_cust.Font = new Font("Microsoft Sans Serif", 12F);
            txt_id_cust.Location = new Point(449, 8);
            txt_id_cust.Margin = new Padding(4, 5, 4, 5);
            txt_id_cust.MinimumSize = new Size(1, 16);
            txt_id_cust.Name = "txt_id_cust";
            txt_id_cust.Padding = new Padding(5);
            txt_id_cust.ReadOnly = true;
            txt_id_cust.ShowText = false;
            txt_id_cust.Size = new Size(80, 29);
            txt_id_cust.TabIndex = 8;
            txt_id_cust.TextAlignment = ContentAlignment.MiddleLeft;
            txt_id_cust.Watermark = "";
            // 
            // txtPersonaContacto
            // 
            txtPersonaContacto.Font = new Font("Microsoft Sans Serif", 12F);
            txtPersonaContacto.Location = new Point(676, 86);
            txtPersonaContacto.Margin = new Padding(4, 5, 4, 5);
            txtPersonaContacto.MinimumSize = new Size(1, 16);
            txtPersonaContacto.Name = "txtPersonaContacto";
            txtPersonaContacto.Padding = new Padding(5);
            txtPersonaContacto.ReadOnly = true;
            txtPersonaContacto.ShowText = false;
            txtPersonaContacto.Size = new Size(219, 29);
            txtPersonaContacto.TabIndex = 29;
            txtPersonaContacto.TextAlignment = ContentAlignment.MiddleLeft;
            txtPersonaContacto.Watermark = "Persona Contacto";
            // 
            // cboCondicionesPago
            // 
            cboCondicionesPago.DataSource = null;
            cboCondicionesPago.FillColor = Color.White;
            cboCondicionesPago.Font = new Font("Microsoft Sans Serif", 12F);
            cboCondicionesPago.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cboCondicionesPago.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cboCondicionesPago.Location = new Point(644, 208);
            cboCondicionesPago.Margin = new Padding(4, 5, 4, 5);
            cboCondicionesPago.MinimumSize = new Size(63, 0);
            cboCondicionesPago.Name = "cboCondicionesPago";
            cboCondicionesPago.Padding = new Padding(0, 0, 30, 2);
            cboCondicionesPago.ReadOnly = true;
            cboCondicionesPago.Size = new Size(251, 29);
            cboCondicionesPago.SymbolSize = 24;
            cboCondicionesPago.TabIndex = 28;
            cboCondicionesPago.TextAlignment = ContentAlignment.MiddleLeft;
            cboCondicionesPago.Watermark = "";
            // 
            // cboTipoVenta
            // 
            cboTipoVenta.DataSource = null;
            cboTipoVenta.FillColor = Color.White;
            cboTipoVenta.Font = new Font("Microsoft Sans Serif", 12F);
            cboTipoVenta.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cboTipoVenta.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cboTipoVenta.Location = new Point(644, 169);
            cboTipoVenta.Margin = new Padding(4, 5, 4, 5);
            cboTipoVenta.MinimumSize = new Size(63, 0);
            cboTipoVenta.Name = "cboTipoVenta";
            cboTipoVenta.Padding = new Padding(0, 0, 30, 2);
            cboTipoVenta.ReadOnly = true;
            cboTipoVenta.Size = new Size(251, 29);
            cboTipoVenta.SymbolSize = 24;
            cboTipoVenta.TabIndex = 27;
            cboTipoVenta.TextAlignment = ContentAlignment.MiddleLeft;
            cboTipoVenta.Watermark = "";
            // 
            // uiRichTextBox3
            // 
            uiRichTextBox3.FillColor = Color.White;
            uiRichTextBox3.Font = new Font("Microsoft Sans Serif", 12F);
            uiRichTextBox3.Location = new Point(8, 499);
            uiRichTextBox3.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox3.MinimumSize = new Size(1, 1);
            uiRichTextBox3.Name = "uiRichTextBox3";
            uiRichTextBox3.Padding = new Padding(2);
            uiRichTextBox3.ReadOnly = true;
            uiRichTextBox3.ShowText = false;
            uiRichTextBox3.Size = new Size(450, 98);
            uiRichTextBox3.TabIndex = 11;
            uiRichTextBox3.Text = "Comentario :";
            uiRichTextBox3.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiLabel12
            // 
            uiLabel12.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel12.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel12.Location = new Point(618, 550);
            uiLabel12.Name = "uiLabel12";
            uiLabel12.Size = new Size(92, 23);
            uiLabel12.TabIndex = 22;
            uiLabel12.Text = "Total :";
            // 
            // uiLabel11
            // 
            uiLabel11.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel11.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel11.Location = new Point(618, 511);
            uiLabel11.Name = "uiLabel11";
            uiLabel11.Size = new Size(92, 23);
            uiLabel11.TabIndex = 21;
            uiLabel11.Text = "Itbis :";
            // 
            // uiLabel10
            // 
            uiLabel10.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel10.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel10.Location = new Point(618, 472);
            uiLabel10.Name = "uiLabel10";
            uiLabel10.Size = new Size(92, 23);
            uiLabel10.TabIndex = 20;
            uiLabel10.Text = "Sub-Total :";
            // 
            // uiTextBox6
            // 
            uiTextBox6.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox6.Location = new Point(716, 544);
            uiTextBox6.Margin = new Padding(4, 5, 4, 5);
            uiTextBox6.MinimumSize = new Size(1, 16);
            uiTextBox6.Name = "uiTextBox6";
            uiTextBox6.Padding = new Padding(5);
            uiTextBox6.ReadOnly = true;
            uiTextBox6.ShowText = false;
            uiTextBox6.Size = new Size(156, 29);
            uiTextBox6.TabIndex = 11;
            uiTextBox6.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox6.Watermark = "";
            // 
            // uiTextBox5
            // 
            uiTextBox5.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox5.Location = new Point(716, 505);
            uiTextBox5.Margin = new Padding(4, 5, 4, 5);
            uiTextBox5.MinimumSize = new Size(1, 16);
            uiTextBox5.Name = "uiTextBox5";
            uiTextBox5.Padding = new Padding(5);
            uiTextBox5.ReadOnly = true;
            uiTextBox5.ShowText = false;
            uiTextBox5.Size = new Size(156, 29);
            uiTextBox5.TabIndex = 10;
            uiTextBox5.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox5.Watermark = "";
            // 
            // uiTextBox4
            // 
            uiTextBox4.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox4.Location = new Point(716, 466);
            uiTextBox4.Margin = new Padding(4, 5, 4, 5);
            uiTextBox4.MinimumSize = new Size(1, 16);
            uiTextBox4.Name = "uiTextBox4";
            uiTextBox4.Padding = new Padding(5);
            uiTextBox4.ReadOnly = true;
            uiTextBox4.ShowText = false;
            uiTextBox4.Size = new Size(156, 29);
            uiTextBox4.TabIndex = 9;
            uiTextBox4.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox4.Watermark = "";
            // 
            // btnBuscarProducto
            // 
            btnBuscarProducto.BackColor = Color.Transparent;
            btnBuscarProducto.Enabled = false;
            btnBuscarProducto.FlatAppearance.BorderSize = 0;
            btnBuscarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnBuscarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnBuscarProducto.FlatStyle = FlatStyle.Flat;
            btnBuscarProducto.Image = Properties.Resources.search_property_24px;
            btnBuscarProducto.Location = new Point(737, 247);
            btnBuscarProducto.Margin = new Padding(3, 4, 3, 4);
            btnBuscarProducto.Name = "btnBuscarProducto";
            btnBuscarProducto.Size = new Size(27, 35);
            btnBuscarProducto.TabIndex = 17;
            btnBuscarProducto.UseVisualStyleBackColor = false;
            // 
            // btnAddProducto
            // 
            btnAddProducto.BackColor = Color.Transparent;
            btnAddProducto.Enabled = false;
            btnAddProducto.FlatAppearance.BorderSize = 0;
            btnAddProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnAddProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnAddProducto.FlatStyle = FlatStyle.Flat;
            btnAddProducto.Image = Properties.Resources.plus_24px;
            btnAddProducto.Location = new Point(707, 246);
            btnAddProducto.Margin = new Padding(3, 4, 3, 4);
            btnAddProducto.Name = "btnAddProducto";
            btnAddProducto.Size = new Size(28, 35);
            btnAddProducto.TabIndex = 16;
            btnAddProducto.UseVisualStyleBackColor = false;
            // 
            // btnEliminarProducto
            // 
            btnEliminarProducto.BackColor = Color.Transparent;
            btnEliminarProducto.Enabled = false;
            btnEliminarProducto.FlatAppearance.BorderSize = 0;
            btnEliminarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnEliminarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnEliminarProducto.FlatStyle = FlatStyle.Flat;
            btnEliminarProducto.Image = Properties.Resources.delete_row_24px;
            btnEliminarProducto.Location = new Point(878, 336);
            btnEliminarProducto.Margin = new Padding(3, 4, 3, 4);
            btnEliminarProducto.Name = "btnEliminarProducto";
            btnEliminarProducto.Size = new Size(30, 27);
            btnEliminarProducto.TabIndex = 19;
            btnEliminarProducto.UseVisualStyleBackColor = false;
            // 
            // btnEditarProducto
            // 
            btnEditarProducto.BackColor = Color.Transparent;
            btnEditarProducto.Enabled = false;
            btnEditarProducto.FlatAppearance.BorderSize = 0;
            btnEditarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnEditarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnEditarProducto.FlatStyle = FlatStyle.Flat;
            btnEditarProducto.Image = Properties.Resources.edit_24px;
            btnEditarProducto.Location = new Point(878, 308);
            btnEditarProducto.Margin = new Padding(3, 4, 3, 4);
            btnEditarProducto.Name = "btnEditarProducto";
            btnEditarProducto.Size = new Size(27, 35);
            btnEditarProducto.TabIndex = 18;
            btnEditarProducto.UseVisualStyleBackColor = false;
            btnEditarProducto.Click += btnEditarProducto_Click;
            // 
            // uiDataGridView1
            // 
            uiDataGridView1.AllowUserToAddRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(235, 243, 255);
            uiDataGridView1.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            uiDataGridView1.BackgroundColor = Color.White;
            uiDataGridView1.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle2.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            uiDataGridView1.ColumnHeadersHeight = 32;
            uiDataGridView1.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            uiDataGridView1.Columns.AddRange(new DataGridViewColumn[] { renglon, Description, Unit, Qty, Notes, Price, subtotal });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = SystemColors.Window;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle3.SelectionBackColor = SystemColors.Highlight;
            dataGridViewCellStyle3.SelectionForeColor = SystemColors.HighlightText;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            uiDataGridView1.DefaultCellStyle = dataGridViewCellStyle3;
            uiDataGridView1.EnableHeadersVisualStyles = false;
            uiDataGridView1.Font = new Font("Microsoft Sans Serif", 12F);
            uiDataGridView1.GridColor = Color.FromArgb(80, 160, 255);
            uiDataGridView1.Location = new Point(8, 285);
            uiDataGridView1.Name = "uiDataGridView1";
            uiDataGridView1.ReadOnly = true;
            uiDataGridView1.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle4.SelectionForeColor = Color.White;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            uiDataGridView1.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 12F);
            uiDataGridView1.RowsDefaultCellStyle = dataGridViewCellStyle5;
            uiDataGridView1.SelectedIndex = -1;
            uiDataGridView1.Size = new Size(864, 173);
            uiDataGridView1.StripeOddColor = Color.FromArgb(235, 243, 255);
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
            // subtotal
            // 
            subtotal.HeaderText = "total";
            subtotal.Name = "subtotal";
            subtotal.ReadOnly = true;
            subtotal.Width = 150;
            // 
            // uiComboBox2
            // 
            uiComboBox2.DataSource = null;
            uiComboBox2.FillColor = Color.White;
            uiComboBox2.Font = new Font("Microsoft Sans Serif", 12F);
            uiComboBox2.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox2.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox2.Location = new Point(263, 248);
            uiComboBox2.Margin = new Padding(4, 5, 4, 5);
            uiComboBox2.MinimumSize = new Size(63, 0);
            uiComboBox2.Name = "uiComboBox2";
            uiComboBox2.Padding = new Padding(0, 0, 30, 2);
            uiComboBox2.ReadOnly = true;
            uiComboBox2.Size = new Size(255, 29);
            uiComboBox2.SymbolSize = 24;
            uiComboBox2.TabIndex = 2;
            uiComboBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox2.Watermark = "";
            // 
            // uiTextBox9
            // 
            uiTextBox9.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox9.Location = new Point(618, 250);
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
            // uiTextBox8
            // 
            uiTextBox8.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox8.Location = new Point(525, 248);
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
            // uiLabel9
            // 
            uiLabel9.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel9.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel9.Location = new Point(195, 254);
            uiLabel9.Name = "uiLabel9";
            uiLabel9.Size = new Size(81, 23);
            uiLabel9.TabIndex = 14;
            uiLabel9.Text = "Product :";
            // 
            // uiTextBox3
            // 
            uiTextBox3.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox3.Location = new Point(113, 248);
            uiTextBox3.Margin = new Padding(4, 5, 4, 5);
            uiTextBox3.MinimumSize = new Size(1, 16);
            uiTextBox3.Name = "uiTextBox3";
            uiTextBox3.Padding = new Padding(5);
            uiTextBox3.ReadOnly = true;
            uiTextBox3.ShowText = false;
            uiTextBox3.Size = new Size(75, 29);
            uiTextBox3.TabIndex = 8;
            uiTextBox3.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox3.Watermark = "";
            // 
            // uiLabel8
            // 
            uiLabel8.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel8.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel8.Location = new Point(25, 248);
            uiLabel8.Name = "uiLabel8";
            uiLabel8.Size = new Size(81, 23);
            uiLabel8.TabIndex = 13;
            uiLabel8.Text = "Quantity :";
            // 
            // uiLabel7
            // 
            uiLabel7.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel7.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel7.Location = new Point(113, 118);
            uiLabel7.Name = "uiLabel7";
            uiLabel7.Size = new Size(88, 23);
            uiLabel7.TabIndex = 12;
            uiLabel7.Text = "Bill To :";
            // 
            // uiRichTextBox2
            // 
            uiRichTextBox2.FillColor = Color.White;
            uiRichTextBox2.Font = new Font("Microsoft Sans Serif", 12F);
            uiRichTextBox2.Location = new Point(329, 146);
            uiRichTextBox2.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox2.MinimumSize = new Size(1, 1);
            uiRichTextBox2.Name = "uiRichTextBox2";
            uiRichTextBox2.Padding = new Padding(2);
            uiRichTextBox2.ReadOnly = true;
            uiRichTextBox2.ShowText = false;
            uiRichTextBox2.Size = new Size(208, 91);
            uiRichTextBox2.TabIndex = 11;
            uiRichTextBox2.Text = "Ship To:";
            uiRichTextBox2.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiRichTextBox1
            // 
            uiRichTextBox1.FillColor = Color.White;
            uiRichTextBox1.Font = new Font("Microsoft Sans Serif", 12F);
            uiRichTextBox1.Location = new Point(113, 146);
            uiRichTextBox1.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox1.MinimumSize = new Size(1, 1);
            uiRichTextBox1.Name = "uiRichTextBox1";
            uiRichTextBox1.Padding = new Padding(2);
            uiRichTextBox1.ReadOnly = true;
            uiRichTextBox1.ShowText = false;
            uiRichTextBox1.Size = new Size(208, 92);
            uiRichTextBox1.TabIndex = 10;
            uiRichTextBox1.Text = "Bill To:";
            uiRichTextBox1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiLabel6
            // 
            uiLabel6.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel6.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel6.Location = new Point(375, 92);
            uiLabel6.Name = "uiLabel6";
            uiLabel6.Size = new Size(67, 23);
            uiLabel6.TabIndex = 9;
            uiLabel6.Text = "Status :";
            // 
            // uiTextBox2
            // 
            uiTextBox2.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox2.Location = new Point(449, 86);
            uiTextBox2.Margin = new Padding(4, 5, 4, 5);
            uiTextBox2.MinimumSize = new Size(1, 16);
            uiTextBox2.Name = "uiTextBox2";
            uiTextBox2.Padding = new Padding(5);
            uiTextBox2.ReadOnly = true;
            uiTextBox2.ShowText = false;
            uiTextBox2.Size = new Size(146, 29);
            uiTextBox2.TabIndex = 8;
            uiTextBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox2.Watermark = "";
            // 
            // uiLabel5
            // 
            uiLabel5.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel5.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel5.Location = new Point(371, 53);
            uiLabel5.Name = "uiLabel5";
            uiLabel5.Size = new Size(71, 23);
            uiLabel5.TabIndex = 8;
            uiLabel5.Text = "Vendor :";
            // 
            // uiComboBox1
            // 
            uiComboBox1.DataSource = null;
            uiComboBox1.FillColor = Color.White;
            uiComboBox1.Font = new Font("Microsoft Sans Serif", 12F);
            uiComboBox1.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox1.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox1.Location = new Point(449, 47);
            uiComboBox1.Margin = new Padding(4, 5, 4, 5);
            uiComboBox1.MinimumSize = new Size(63, 0);
            uiComboBox1.Name = "uiComboBox1";
            uiComboBox1.Padding = new Padding(0, 0, 30, 2);
            uiComboBox1.ReadOnly = true;
            uiComboBox1.Size = new Size(248, 29);
            uiComboBox1.SymbolSize = 24;
            uiComboBox1.TabIndex = 2;
            uiComboBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox1.Watermark = "";
            // 
            // uiTextBox1
            // 
            uiTextBox1.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox1.Location = new Point(647, 8);
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
            // uiLabel4
            // 
            uiLabel4.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel4.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel4.Location = new Point(525, 14);
            uiLabel4.Name = "uiLabel4";
            uiLabel4.Size = new Size(122, 23);
            uiLabel4.TabIndex = 6;
            uiLabel4.Text = "Numero Orden :";
            // 
            // uiLabel3
            // 
            uiLabel3.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(8, 92);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(100, 23);
            uiLabel3.TabIndex = 5;
            uiLabel3.Text = "Fecha Est :";
            // 
            // uiDatetimePicker2
            // 
            uiDatetimePicker2.DateCultureInfo = new System.Globalization.CultureInfo("es-DO");
            uiDatetimePicker2.FillColor = Color.White;
            uiDatetimePicker2.Font = new Font("Microsoft Sans Serif", 12F);
            uiDatetimePicker2.Location = new Point(113, 86);
            uiDatetimePicker2.Margin = new Padding(4, 5, 4, 5);
            uiDatetimePicker2.MaxLength = 19;
            uiDatetimePicker2.MinimumSize = new Size(63, 0);
            uiDatetimePicker2.Name = "uiDatetimePicker2";
            uiDatetimePicker2.Padding = new Padding(0, 0, 30, 2);
            uiDatetimePicker2.ReadOnly = true;
            uiDatetimePicker2.Size = new Size(255, 29);
            uiDatetimePicker2.SymbolDropDown = 61555;
            uiDatetimePicker2.SymbolNormal = 61555;
            uiDatetimePicker2.SymbolSize = 24;
            uiDatetimePicker2.TabIndex = 4;
            uiDatetimePicker2.Text = "2026-09-23 16:42:29";
            uiDatetimePicker2.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatetimePicker2.Value = new DateTime(2026, 9, 23, 16, 42, 29, 543);
            uiDatetimePicker2.Watermark = "";
            uiDatetimePicker2.WatermarkActiveColor = SystemColors.GrayText;
            // 
            // uiDatetimePicker1
            // 
            uiDatetimePicker1.DateCultureInfo = new System.Globalization.CultureInfo("es-DO");
            uiDatetimePicker1.FillColor = Color.White;
            uiDatetimePicker1.Font = new Font("Microsoft Sans Serif", 12F);
            uiDatetimePicker1.Location = new Point(113, 47);
            uiDatetimePicker1.Margin = new Padding(4, 5, 4, 5);
            uiDatetimePicker1.MaxLength = 19;
            uiDatetimePicker1.MinimumSize = new Size(63, 0);
            uiDatetimePicker1.Name = "uiDatetimePicker1";
            uiDatetimePicker1.Padding = new Padding(0, 0, 30, 2);
            uiDatetimePicker1.ReadOnly = true;
            uiDatetimePicker1.Size = new Size(255, 29);
            uiDatetimePicker1.SymbolDropDown = 61555;
            uiDatetimePicker1.SymbolNormal = 61555;
            uiDatetimePicker1.SymbolSize = 24;
            uiDatetimePicker1.TabIndex = 3;
            uiDatetimePicker1.Text = "2026-09-23 16:42:29";
            uiDatetimePicker1.TextAlignment = ContentAlignment.MiddleLeft;
            uiDatetimePicker1.Value = new DateTime(2026, 9, 23, 16, 42, 29, 0);
            uiDatetimePicker1.Watermark = "";
            // 
            // uiLabel2
            // 
            uiLabel2.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel2.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel2.Location = new Point(8, 53);
            uiLabel2.Name = "uiLabel2";
            uiLabel2.Size = new Size(100, 23);
            uiLabel2.TabIndex = 2;
            uiLabel2.Text = "Fecha Reg :";
            // 
            // cbo_customers
            // 
            cbo_customers.DataSource = null;
            cbo_customers.FillColor = Color.White;
            cbo_customers.Font = new Font("Microsoft Sans Serif", 12F);
            cbo_customers.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cbo_customers.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cbo_customers.Location = new Point(113, 8);
            cbo_customers.Margin = new Padding(4, 5, 4, 5);
            cbo_customers.MinimumSize = new Size(63, 0);
            cbo_customers.Name = "cbo_customers";
            cbo_customers.Padding = new Padding(0, 0, 30, 2);
            cbo_customers.ReadOnly = true;
            cbo_customers.Size = new Size(255, 29);
            cbo_customers.SymbolSize = 24;
            cbo_customers.TabIndex = 1;
            cbo_customers.TextAlignment = ContentAlignment.MiddleLeft;
            cbo_customers.Watermark = "";
            // 
            // uiLabel1
            // 
            uiLabel1.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel1.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel1.Location = new Point(6, 14);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(100, 23);
            uiLabel1.TabIndex = 0;
            uiLabel1.Text = "Customer :";
            // 
            // barraHerramientas
            // 
            barraHerramientas.AutoSize = false;
            barraHerramientas.BackColor = Color.FromArgb(225, 225, 225);
            barraHerramientas.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            barraHerramientas.ForeColor = Color.FromArgb(80, 80, 80);
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar, btnGuardar, btnCancelar });
            barraHerramientas.Location = new Point(0, 95);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 0, 0, 0);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(1264, 40);
            barraHerramientas.TabIndex = 3;
            barraHerramientas.Text = "barraHerramientas";
            // 
            // btnNuevo
            // 
            btnNuevo.AutoSize = false;
            btnNuevo.BackColor = Color.Transparent;
            btnNuevo.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnNuevo.ForeColor = Color.FromArgb(80, 80, 80);
            btnNuevo.Image = Properties.Resources.add_file_32px;
            btnNuevo.ImageAlign = ContentAlignment.MiddleLeft;
            btnNuevo.ImageScaling = ToolStripItemImageScaling.None;
            btnNuevo.Margin = new Padding(4, 1, 0, 2);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(92, 36);
            btnNuevo.Text = "Nuevo";
            btnNuevo.ToolTipText = "Nuevo pedido";
            // 
            // btnEditar
            // 
            btnEditar.AutoSize = false;
            btnEditar.BackColor = Color.Transparent;
            btnEditar.Enabled = false;
            btnEditar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnEditar.ForeColor = Color.FromArgb(80, 80, 80);
            btnEditar.Image = Properties.Resources.edit_24px;
            btnEditar.ImageAlign = ContentAlignment.MiddleLeft;
            btnEditar.ImageScaling = ToolStripItemImageScaling.None;
            btnEditar.Margin = new Padding(4, 1, 0, 2);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(92, 36);
            btnEditar.Text = "Editar";
            btnEditar.ToolTipText = "Editar pedido seleccionado";
            // 
            // btnGuardar
            // 
            btnGuardar.AutoSize = false;
            btnGuardar.BackColor = Color.Transparent;
            btnGuardar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnGuardar.ForeColor = Color.FromArgb(80, 80, 80);
            btnGuardar.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardar.ImageScaling = ToolStripItemImageScaling.None;
            btnGuardar.Margin = new Padding(4, 1, 0, 2);
            btnGuardar.Name = "btnGuardar";
            btnGuardar.Size = new Size(92, 36);
            btnGuardar.Text = "Guardar";
            btnGuardar.ToolTipText = "Guardar el pedido nuevo";
            btnGuardar.Visible = false;
            // 
            // btnCancelar
            // 
            btnCancelar.AutoSize = false;
            btnCancelar.BackColor = Color.Transparent;
            btnCancelar.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnCancelar.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancelar.Image = Properties.Resources.cancel_24px;
            btnCancelar.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelar.ImageScaling = ToolStripItemImageScaling.None;
            btnCancelar.Margin = new Padding(4, 1, 0, 2);
            btnCancelar.Name = "btnCancelar";
            btnCancelar.Size = new Size(92, 36);
            btnCancelar.Text = "Cancelar";
            btnCancelar.ToolTipText = "Descartar el pedido nuevo";
            btnCancelar.Visible = false;
            // 
            // FrmPedidos
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1264, 758);
            Controls.Add(sales_orders_tabs);
            Controls.Add(sales_orders_search);
            Controls.Add(barraHerramientas);
            Controls.Add(panelTitulo);
            MinimumSize = new Size(1150, 622);
            Name = "FrmPedidos";
            Text = "Pedidos de Cliente";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 622);
            panelTitulo.ResumeLayout(false);
            sales_orders_search.ResumeLayout(false);
            panelContador.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridPedidos).EndInit();
            panelBuscar.ResumeLayout(false);
            panelBuscar.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)picLupa).EndInit();
            sales_orders_tabs.ResumeLayout(false);
            tabGeneral.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)uiDataGridView1).EndInit();
            barraHerramientas.ResumeLayout(false);
            barraHerramientas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelTitulo;
        private Label lblTitulo;
        private Panel sales_orders_search;
        private Panel panelBuscar;
        private Label lblBuscar;
        private Panel panelContador;
        private Label lblContador;
        private TextBox txtBuscarPedido;
        private PictureBox picLupa;
        private Button btnLimpiarBusqueda;
        private DataGridView gridPedidos;
        private TabControl sales_orders_tabs;
        private TabPage tabGeneral;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevo;
        private ToolStripButton btnEditar;
        private ToolStripButton btnGuardar;
        private ToolStripButton btnCancelar;
        private Button btnEditarProducto;
        private Button btnEliminarProducto;
        private Sunny.UI.UILabel uiLabel1;
        private Sunny.UI.UIDatetimePicker uiDatetimePicker1;
        private Sunny.UI.UILabel uiLabel2;
        private Sunny.UI.UIComboBox cbo_customers;
        private Sunny.UI.UILabel uiLabel3;
        private Sunny.UI.UIDatetimePicker uiDatetimePicker2;
        private Sunny.UI.UILabel uiLabel4;
        private Sunny.UI.UITextBox uiTextBox1;
        private Sunny.UI.UILabel uiLabel5;
        private Sunny.UI.UIComboBox uiComboBox1;
        private Sunny.UI.UILabel uiLabel6;
        private Sunny.UI.UITextBox uiTextBox2;
        private Sunny.UI.UIRichTextBox uiRichTextBox2;
        private Sunny.UI.UIRichTextBox uiRichTextBox1;
        private Sunny.UI.UILabel uiLabel7;
        private Sunny.UI.UILabel uiLabel9;
        private Sunny.UI.UITextBox uiTextBox3;
        private Sunny.UI.UILabel uiLabel8;
        private Sunny.UI.UIComboBox uiComboBox2;
        private Sunny.UI.UIDataGridView uiDataGridView1;
        private Button btnAddProducto;
        private Button btnBuscarProducto;
        private Sunny.UI.UIRichTextBox uiRichTextBox3;
        private Sunny.UI.UILabel uiLabel12;
        private Sunny.UI.UILabel uiLabel11;
        private Sunny.UI.UILabel uiLabel10;
        private Sunny.UI.UITextBox uiTextBox6;
        private Sunny.UI.UITextBox uiTextBox7;
        private Sunny.UI.UITextBox uiTextBox8;
        private Sunny.UI.UITextBox uiTextBox9;
        private Sunny.UI.UILabel uiLabel13;
        private Sunny.UI.UILabel uiLabel15;
        private Sunny.UI.UILabel uiLabel16;
        private Sunny.UI.UITextBox txt_id_cust;
        private Sunny.UI.UITextBox uiTextBox5;
        private Sunny.UI.UITextBox uiTextBox4;
        private Sunny.UI.UITextBox txt_id_vendor;
        private Sunny.UI.UIComboBox cboTipoVenta;
        private Sunny.UI.UIComboBox cboCondicionesPago;
        private Sunny.UI.UITextBox txtPersonaContacto;
        private Sunny.UI.UIComboBox cbo_prioridad;
        private Sunny.UI.UILabel uiLabel18;
        private Sunny.UI.UILabel uiLabel17;
        private Sunny.UI.UILabel uiLabel14;
        private Sunny.UI.UILabel uiLabel20;
        private Sunny.UI.UILabel uiLabel19;
        private DataGridViewTextBoxColumn renglon;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn Unit;
        private DataGridViewTextBoxColumn Qty;
        private DataGridViewTextBoxColumn Notes;
        private DataGridViewTextBoxColumn Price;
        private DataGridViewTextBoxColumn subtotal;
        private Sunny.UI.UILabel uiLabel21;
        private Sunny.UI.UITextBox txt_total_cantidad;
    }
}