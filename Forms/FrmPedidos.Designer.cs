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
            panelBuscar.Size = new Size(345, 56);
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
            // uiRichTextBox3
            // 
            uiRichTextBox3.FillColor = Color.White;
            uiRichTextBox3.Font = new Font("Microsoft Sans Serif", 12F);
            uiRichTextBox3.Location = new Point(8, 466);
            uiRichTextBox3.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox3.MinimumSize = new Size(1, 1);
            uiRichTextBox3.Name = "uiRichTextBox3";
            uiRichTextBox3.Padding = new Padding(2);
            uiRichTextBox3.ShowText = false;
            uiRichTextBox3.Size = new Size(343, 113);
            uiRichTextBox3.TabIndex = 11;
            uiRichTextBox3.Text = "Comentario :";
            uiRichTextBox3.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiLabel12
            // 
            uiLabel12.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel12.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel12.Location = new Point(383, 550);
            uiLabel12.Name = "uiLabel12";
            uiLabel12.Size = new Size(114, 23);
            uiLabel12.TabIndex = 22;
            uiLabel12.Text = "Total :";
            // 
            // uiLabel11
            // 
            uiLabel11.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel11.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel11.Location = new Point(383, 511);
            uiLabel11.Name = "uiLabel11";
            uiLabel11.Size = new Size(114, 23);
            uiLabel11.TabIndex = 21;
            uiLabel11.Text = "Itbis :";
            // 
            // uiLabel10
            // 
            uiLabel10.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel10.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel10.Location = new Point(383, 472);
            uiLabel10.Name = "uiLabel10";
            uiLabel10.Size = new Size(114, 23);
            uiLabel10.TabIndex = 20;
            uiLabel10.Text = "Sub-Total :";
            // 
            // uiTextBox6
            // 
            uiTextBox6.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox6.Location = new Point(504, 544);
            uiTextBox6.Margin = new Padding(4, 5, 4, 5);
            uiTextBox6.MinimumSize = new Size(1, 16);
            uiTextBox6.Name = "uiTextBox6";
            uiTextBox6.Padding = new Padding(5);
            uiTextBox6.ShowText = false;
            uiTextBox6.Size = new Size(248, 29);
            uiTextBox6.TabIndex = 11;
            uiTextBox6.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox6.Watermark = "";
            // 
            // uiTextBox5
            // 
            uiTextBox5.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox5.Location = new Point(504, 505);
            uiTextBox5.Margin = new Padding(4, 5, 4, 5);
            uiTextBox5.MinimumSize = new Size(1, 16);
            uiTextBox5.Name = "uiTextBox5";
            uiTextBox5.Padding = new Padding(5);
            uiTextBox5.ShowText = false;
            uiTextBox5.Size = new Size(248, 29);
            uiTextBox5.TabIndex = 10;
            uiTextBox5.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox5.Watermark = "";
            // 
            // uiTextBox4
            // 
            uiTextBox4.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox4.Location = new Point(504, 466);
            uiTextBox4.Margin = new Padding(4, 5, 4, 5);
            uiTextBox4.MinimumSize = new Size(1, 16);
            uiTextBox4.Name = "uiTextBox4";
            uiTextBox4.Padding = new Padding(5);
            uiTextBox4.ShowText = false;
            uiTextBox4.Size = new Size(248, 29);
            uiTextBox4.TabIndex = 9;
            uiTextBox4.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox4.Watermark = "";
            // 
            // btnBuscarProducto
            // 
            btnBuscarProducto.BackColor = Color.Transparent;
            btnBuscarProducto.FlatAppearance.BorderSize = 0;
            btnBuscarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnBuscarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnBuscarProducto.FlatStyle = FlatStyle.Flat;
            btnBuscarProducto.Image = Properties.Resources.search_property_24px;
            btnBuscarProducto.Location = new Point(605, 264);
            btnBuscarProducto.Margin = new Padding(3, 4, 3, 4);
            btnBuscarProducto.Name = "btnBuscarProducto";
            btnBuscarProducto.Size = new Size(42, 35);
            btnBuscarProducto.TabIndex = 17;
            btnBuscarProducto.UseVisualStyleBackColor = false;
            // 
            // btnAddProducto
            // 
            btnAddProducto.BackColor = Color.Transparent;
            btnAddProducto.FlatAppearance.BorderSize = 0;
            btnAddProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnAddProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnAddProducto.FlatStyle = FlatStyle.Flat;
            btnAddProducto.Image = Properties.Resources.plus_24px;
            btnAddProducto.Location = new Point(571, 264);
            btnAddProducto.Margin = new Padding(3, 4, 3, 4);
            btnAddProducto.Name = "btnAddProducto";
            btnAddProducto.Size = new Size(42, 35);
            btnAddProducto.TabIndex = 16;
            btnAddProducto.UseVisualStyleBackColor = false;
            // 
            // btnEliminarProducto
            // 
            btnEliminarProducto.BackColor = Color.Transparent;
            btnEliminarProducto.FlatAppearance.BorderSize = 0;
            btnEliminarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnEliminarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnEliminarProducto.FlatStyle = FlatStyle.Flat;
            btnEliminarProducto.Image = Properties.Resources.delete_row_24px;
            btnEliminarProducto.Location = new Point(867, 351);
            btnEliminarProducto.Margin = new Padding(3, 4, 3, 4);
            btnEliminarProducto.Name = "btnEliminarProducto";
            btnEliminarProducto.Size = new Size(42, 35);
            btnEliminarProducto.TabIndex = 19;
            btnEliminarProducto.UseVisualStyleBackColor = false;
            // 
            // btnEditarProducto
            // 
            btnEditarProducto.BackColor = Color.Transparent;
            btnEditarProducto.FlatAppearance.BorderSize = 0;
            btnEditarProducto.FlatAppearance.MouseDownBackColor = Color.FromArgb(200, 200, 200);
            btnEditarProducto.FlatAppearance.MouseOverBackColor = Color.FromArgb(235, 235, 235);
            btnEditarProducto.FlatStyle = FlatStyle.Flat;
            btnEditarProducto.Image = Properties.Resources.edit_24px;
            btnEditarProducto.Location = new Point(868, 308);
            btnEditarProducto.Margin = new Padding(3, 4, 3, 4);
            btnEditarProducto.Name = "btnEditarProducto";
            btnEditarProducto.Size = new Size(42, 35);
            btnEditarProducto.TabIndex = 18;
            btnEditarProducto.UseVisualStyleBackColor = false;
            // 
            // uiDataGridView1
            // 
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
            uiDataGridView1.Location = new Point(8, 308);
            uiDataGridView1.Name = "uiDataGridView1";
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
            uiDataGridView1.Size = new Size(805, 150);
            uiDataGridView1.StripeOddColor = Color.FromArgb(235, 243, 255);
            uiDataGridView1.TabIndex = 15;
            // 
            // renglon
            // 
            renglon.HeaderText = "Id. Pro.";
            renglon.Name = "renglon";
            renglon.Width = 80;
            // 
            // Description
            // 
            Description.HeaderText = "Product Name";
            Description.Name = "Description";
            Description.Width = 280;
            // 
            // Unit
            // 
            Unit.HeaderText = "Unit";
            Unit.Name = "Unit";
            Unit.Width = 50;
            // 
            // Qty
            // 
            Qty.HeaderText = "Qty";
            Qty.Name = "Qty";
            Qty.Width = 70;
            // 
            // Notes
            // 
            Notes.HeaderText = "Notes";
            Notes.Name = "Notes";
            Notes.Width = 30;
            // 
            // Price
            // 
            Price.HeaderText = "Price";
            Price.Name = "Price";
            Price.Width = 80;
            // 
            // subtotal
            // 
            subtotal.HeaderText = "total";
            subtotal.Name = "subtotal";
            subtotal.Width = 70;
            // 
            // uiComboBox2
            // 
            uiComboBox2.DataSource = null;
            uiComboBox2.FillColor = Color.White;
            uiComboBox2.Font = new Font("Microsoft Sans Serif", 12F);
            uiComboBox2.ItemHoverColor = Color.FromArgb(155, 200, 255);
            uiComboBox2.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            uiComboBox2.Location = new Point(309, 270);
            uiComboBox2.Margin = new Padding(4, 5, 4, 5);
            uiComboBox2.MinimumSize = new Size(63, 0);
            uiComboBox2.Name = "uiComboBox2";
            uiComboBox2.Padding = new Padding(0, 0, 30, 2);
            uiComboBox2.Size = new Size(255, 29);
            uiComboBox2.SymbolSize = 24;
            uiComboBox2.TabIndex = 2;
            uiComboBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox2.Watermark = "";
            // 
            // uiLabel9
            // 
            uiLabel9.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel9.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel9.Location = new Point(233, 276);
            uiLabel9.Name = "uiLabel9";
            uiLabel9.Size = new Size(81, 23);
            uiLabel9.TabIndex = 14;
            uiLabel9.Text = "Product :";
            // 
            // uiTextBox3
            // 
            uiTextBox3.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox3.Location = new Point(151, 270);
            uiTextBox3.Margin = new Padding(4, 5, 4, 5);
            uiTextBox3.MinimumSize = new Size(1, 16);
            uiTextBox3.Name = "uiTextBox3";
            uiTextBox3.Padding = new Padding(5);
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
            uiLabel8.Location = new Point(19, 277);
            uiLabel8.Name = "uiLabel8";
            uiLabel8.Size = new Size(81, 23);
            uiLabel8.TabIndex = 13;
            uiLabel8.Text = "Quantity :";
            // 
            // uiLabel7
            // 
            uiLabel7.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel7.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel7.Location = new Point(19, 147);
            uiLabel7.Name = "uiLabel7";
            uiLabel7.Size = new Size(114, 23);
            uiLabel7.TabIndex = 12;
            uiLabel7.Text = "Direccion :";
            // 
            // uiRichTextBox2
            // 
            uiRichTextBox2.FillColor = Color.White;
            uiRichTextBox2.Font = new Font("Microsoft Sans Serif", 12F);
            uiRichTextBox2.Location = new Point(414, 147);
            uiRichTextBox2.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox2.MinimumSize = new Size(1, 1);
            uiRichTextBox2.Name = "uiRichTextBox2";
            uiRichTextBox2.Padding = new Padding(2);
            uiRichTextBox2.ShowText = false;
            uiRichTextBox2.Size = new Size(255, 113);
            uiRichTextBox2.TabIndex = 11;
            uiRichTextBox2.Text = "Ship To:";
            uiRichTextBox2.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiRichTextBox1
            // 
            uiRichTextBox1.FillColor = Color.White;
            uiRichTextBox1.Font = new Font("Microsoft Sans Serif", 12F);
            uiRichTextBox1.Location = new Point(151, 147);
            uiRichTextBox1.Margin = new Padding(4, 5, 4, 5);
            uiRichTextBox1.MinimumSize = new Size(1, 1);
            uiRichTextBox1.Name = "uiRichTextBox1";
            uiRichTextBox1.Padding = new Padding(2);
            uiRichTextBox1.ShowText = false;
            uiRichTextBox1.Size = new Size(255, 113);
            uiRichTextBox1.TabIndex = 10;
            uiRichTextBox1.Text = "Bill To:";
            uiRichTextBox1.TextAlignment = ContentAlignment.MiddleCenter;
            // 
            // uiLabel6
            // 
            uiLabel6.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel6.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel6.Location = new Point(422, 108);
            uiLabel6.Name = "uiLabel6";
            uiLabel6.Size = new Size(122, 23);
            uiLabel6.TabIndex = 9;
            uiLabel6.Text = "Status :";
            // 
            // uiTextBox2
            // 
            uiTextBox2.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox2.Location = new Point(544, 102);
            uiTextBox2.Margin = new Padding(4, 5, 4, 5);
            uiTextBox2.MinimumSize = new Size(1, 16);
            uiTextBox2.Name = "uiTextBox2";
            uiTextBox2.Padding = new Padding(5);
            uiTextBox2.ShowText = false;
            uiTextBox2.Size = new Size(248, 29);
            uiTextBox2.TabIndex = 8;
            uiTextBox2.TextAlignment = ContentAlignment.MiddleLeft;
            uiTextBox2.Watermark = "";
            // 
            // uiLabel5
            // 
            uiLabel5.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel5.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel5.Location = new Point(422, 69);
            uiLabel5.Name = "uiLabel5";
            uiLabel5.Size = new Size(100, 23);
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
            uiComboBox1.Location = new Point(544, 63);
            uiComboBox1.Margin = new Padding(4, 5, 4, 5);
            uiComboBox1.MinimumSize = new Size(63, 0);
            uiComboBox1.Name = "uiComboBox1";
            uiComboBox1.Padding = new Padding(0, 0, 30, 2);
            uiComboBox1.Size = new Size(248, 29);
            uiComboBox1.SymbolSize = 24;
            uiComboBox1.TabIndex = 2;
            uiComboBox1.TextAlignment = ContentAlignment.MiddleLeft;
            uiComboBox1.Watermark = "";
            // 
            // uiTextBox1
            // 
            uiTextBox1.Font = new Font("Microsoft Sans Serif", 12F);
            uiTextBox1.Location = new Point(544, 24);
            uiTextBox1.Margin = new Padding(4, 5, 4, 5);
            uiTextBox1.MinimumSize = new Size(1, 16);
            uiTextBox1.Name = "uiTextBox1";
            uiTextBox1.Padding = new Padding(5);
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
            uiLabel4.Location = new Point(422, 30);
            uiLabel4.Name = "uiLabel4";
            uiLabel4.Size = new Size(122, 23);
            uiLabel4.TabIndex = 6;
            uiLabel4.Text = "Numero Orden :";
            // 
            // uiLabel3
            // 
            uiLabel3.Font = new Font("Microsoft Sans Serif", 12F);
            uiLabel3.ForeColor = Color.FromArgb(48, 48, 48);
            uiLabel3.Location = new Point(19, 114);
            uiLabel3.Name = "uiLabel3";
            uiLabel3.Size = new Size(131, 23);
            uiLabel3.TabIndex = 5;
            uiLabel3.Text = "Fecha Ent Est :";
            // 
            // uiDatetimePicker2
            // 
            uiDatetimePicker2.DateCultureInfo = new System.Globalization.CultureInfo("es-DO");
            uiDatetimePicker2.FillColor = Color.White;
            uiDatetimePicker2.Font = new Font("Microsoft Sans Serif", 12F);
            uiDatetimePicker2.Location = new Point(151, 108);
            uiDatetimePicker2.Margin = new Padding(4, 5, 4, 5);
            uiDatetimePicker2.MaxLength = 19;
            uiDatetimePicker2.MinimumSize = new Size(63, 0);
            uiDatetimePicker2.Name = "uiDatetimePicker2";
            uiDatetimePicker2.Padding = new Padding(0, 0, 30, 2);
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
            uiDatetimePicker1.Location = new Point(151, 69);
            uiDatetimePicker1.Margin = new Padding(4, 5, 4, 5);
            uiDatetimePicker1.MaxLength = 19;
            uiDatetimePicker1.MinimumSize = new Size(63, 0);
            uiDatetimePicker1.Name = "uiDatetimePicker1";
            uiDatetimePicker1.Padding = new Padding(0, 0, 30, 2);
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
            uiLabel2.Location = new Point(19, 72);
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
            cbo_customers.Location = new Point(151, 30);
            cbo_customers.Margin = new Padding(4, 5, 4, 5);
            cbo_customers.MinimumSize = new Size(63, 0);
            cbo_customers.Name = "cbo_customers";
            cbo_customers.Padding = new Padding(0, 0, 30, 2);
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
            uiLabel1.Location = new Point(19, 30);
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
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar });
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
        private Sunny.UI.UITextBox uiTextBox5;
        private Sunny.UI.UITextBox uiTextBox4;
        private DataGridViewTextBoxColumn renglon;
        private DataGridViewTextBoxColumn Description;
        private DataGridViewTextBoxColumn Unit;
        private DataGridViewTextBoxColumn Qty;
        private DataGridViewTextBoxColumn Notes;
        private DataGridViewTextBoxColumn Price;
        private DataGridViewTextBoxColumn subtotal;
    }
}