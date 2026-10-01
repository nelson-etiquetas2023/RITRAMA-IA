namespace Ritrama2025.Forms
{
    partial class FrmProductos
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
        /// InitializeComponent del rediseño 30/70 del modulo de Productos.
        /// </summary>
        private void InitializeComponent()
        {
            DataGridViewCellStyle dataGridViewCellStyle1 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle2 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle3 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle4 = new DataGridViewCellStyle();
            DataGridViewCellStyle dataGridViewCellStyle5 = new DataGridViewCellStyle();
            tlpRoot = new TableLayoutPanel();
            panelIzq = new Panel();
            gridProductos = new Sunny.UI.UIDataGridView();
            colProductId = new DataGridViewTextBoxColumn();
            colProductName = new DataGridViewTextBoxColumn();
            colTipo = new DataGridViewTextBoxColumn();
            pnlBuscador = new Panel();
            pnlFiltroTipo = new FlowLayoutPanel();
            rbFiltroTodos = new Sunny.UI.UIRadioButton();
            rbFiltroMaster = new Sunny.UI.UIRadioButton();
            rbFiltroRolloCortado = new Sunny.UI.UIRadioButton();
            rbFiltroHojas = new Sunny.UI.UIRadioButton();
            rbFiltroGraphics = new Sunny.UI.UIRadioButton();
            txtSearch = new Sunny.UI.UITextBox();
            lblTitulo = new Sunny.UI.UILabel();
            pnlContador = new Panel();
            lblContadorDetalle = new Sunny.UI.UILabel();
            lblContador = new Sunny.UI.UILabel();
            panelDer = new Panel();
            tabDetalle = new Sunny.UI.UITabControl();
            tabDetalleProducto = new TabPage();
            tlpDetalle = new TableLayoutPanel();
            lblDetalleTitulo = new Sunny.UI.UILabel();
            lblDetId = new Sunny.UI.UILabel();
            txtDetId = new Sunny.UI.UITextBox();
            lblDetNombre = new Sunny.UI.UILabel();
            txtDetNombre = new Sunny.UI.UITextBox();
            lblDetTipo = new Sunny.UI.UILabel();
            grpTipo = new Sunny.UI.UIGroupBox();
            rbTipoMaster = new Sunny.UI.UIRadioButton();
            rbTipoRolloCortado = new Sunny.UI.UIRadioButton();
            rbTipoHojas = new Sunny.UI.UIRadioButton();
            rbTipoGraphics = new Sunny.UI.UIRadioButton();
            lblDetReferencia = new Sunny.UI.UILabel();
            txtDetReferencia = new Sunny.UI.UITextBox();
            lblDetCodebar = new Sunny.UI.UILabel();
            txtDetCodebar = new Sunny.UI.UITextBox();
            lblDetPrecio = new Sunny.UI.UILabel();
            txtDetPrecio = new Sunny.UI.UITextBox();
            lblDetCosto = new Sunny.UI.UILabel();
            txtDetCosto = new Sunny.UI.UITextBox();
            lblDetRatio = new Sunny.UI.UILabel();
            txtDetRatio = new Sunny.UI.UITextBox();
            lblDetEstado = new Sunny.UI.UILabel();
            swDetEstado = new Sunny.UI.UISwitch();
            lblDetDescripcion = new Sunny.UI.UILabel();
            txtDetDescripcion = new Sunny.UI.UITextBox();
            barraHerramientas = new ToolStrip();
            btnNuevoProducto = new ToolStripButton();
            btnEditarProducto = new ToolStripButton();
            btnImportarProducto = new ToolStripButton();
            btnGuardarProducto = new ToolStripButton();
            btnCancelarProducto = new ToolStripButton();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridProductos).BeginInit();
            pnlBuscador.SuspendLayout();
            pnlFiltroTipo.SuspendLayout();
            pnlContador.SuspendLayout();
            panelDer.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabDetalleProducto.SuspendLayout();
            tlpDetalle.SuspendLayout();
            grpTipo.SuspendLayout();
            barraHerramientas.SuspendLayout();
            SuspendLayout();
            // 
            // tlpRoot
            // 
            tlpRoot.ColumnCount = 2;
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tlpRoot.Controls.Add(panelIzq, 0, 0);
            tlpRoot.Controls.Add(panelDer, 1, 0);
            tlpRoot.Dock = DockStyle.Fill;
            tlpRoot.Location = new Point(0, 35);
            tlpRoot.Name = "tlpRoot";
            tlpRoot.RowCount = 1;
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpRoot.Size = new Size(1150, 677);
            tlpRoot.TabIndex = 0;
            // 
            // panelIzq
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(gridProductos);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Controls.Add(pnlContador);
            panelIzq.Dock = DockStyle.Fill;
            panelIzq.Location = new Point(0, 0);
            panelIzq.Margin = new Padding(0);
            panelIzq.MinimumSize = new Size(260, 0);
            panelIzq.Name = "panelIzq";
            panelIzq.Size = new Size(345, 677);
            panelIzq.TabIndex = 0;
            // 
            // gridProductos
            // 
            gridProductos.AllowUserToAddRows = false;
            gridProductos.AllowUserToDeleteRows = false;
            gridProductos.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 250, 240);
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle1.SelectionBackColor = Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            gridProductos.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            gridProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridProductos.BackgroundColor = Color.White;
            gridProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(110, 190, 40);
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(110, 190, 40);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            gridProductos.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            gridProductos.ColumnHeadersHeight = 32;
            gridProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridProductos.Columns.AddRange(new DataGridViewColumn[] { colProductId, colProductName, colTipo });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle3.SelectionBackColor = Color.Black;
            dataGridViewCellStyle3.SelectionForeColor = Color.White;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            gridProductos.DefaultCellStyle = dataGridViewCellStyle3;
            gridProductos.Dock = DockStyle.Fill;
            gridProductos.EnableHeadersVisualStyles = false;
            gridProductos.Font = new Font("Microsoft Sans Serif", 9F);
            gridProductos.GridColor = Color.FromArgb(180, 210, 180);
            gridProductos.Location = new Point(0, 64);
            gridProductos.MultiSelect = false;
            gridProductos.Name = "gridProductos";
            gridProductos.ReadOnly = true;
            gridProductos.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle4.SelectionForeColor = Color.White;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            gridProductos.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            gridProductos.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle5.SelectionBackColor = Color.Black;
            dataGridViewCellStyle5.SelectionForeColor = Color.White;
            gridProductos.RowsDefaultCellStyle = dataGridViewCellStyle5;
            gridProductos.SelectedIndex = -1;
            gridProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridProductos.Size = new Size(343, 551);
            gridProductos.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridProductos.TabIndex = 2;
            // 
            // colProductId
            // 
            colProductId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colProductId.DataPropertyName = "ProductId";
            colProductId.FillWeight = 28F;
            colProductId.HeaderText = "Product ID";
            colProductId.Name = "colProductId";
            colProductId.ReadOnly = true;
            // 
            // colProductName
            // 
            colProductName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colProductName.DataPropertyName = "ProductName";
            colProductName.FillWeight = 52F;
            colProductName.HeaderText = "Product Name";
            colProductName.Name = "colProductName";
            colProductName.ReadOnly = true;
            // 
            // colTipo
            // 
            colTipo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTipo.DataPropertyName = "Tipo";
            colTipo.FillWeight = 20F;
            colTipo.HeaderText = "Tipo";
            colTipo.Name = "colTipo";
            colTipo.ReadOnly = true;
            // 
            // pnlBuscador
            // 
            pnlBuscador.BackColor = Color.White;
            // El orden importa con Dock: WinForms acopla en orden INVERSO al de alta, asi que
            // este orden deja el titulo arriba, el buscador debajo y la tira de filtro al
            // final, que es donde tiene que estar.
            pnlBuscador.Controls.Add(txtSearch);
            pnlBuscador.Controls.Add(pnlFiltroTipo);
            pnlBuscador.Controls.Add(lblTitulo);
            pnlBuscador.Dock = DockStyle.Top;
            pnlBuscador.Location = new Point(0, 0);
            pnlBuscador.Name = "pnlBuscador";
            pnlBuscador.Padding = new Padding(4, 6, 4, 6);
            pnlBuscador.Size = new Size(343, 92);
            pnlBuscador.TabIndex = 0;
            //
            // pnlFiltroTipo
            //
            pnlFiltroTipo.Controls.Add(rbFiltroTodos);
            pnlFiltroTipo.Controls.Add(rbFiltroMaster);
            pnlFiltroTipo.Controls.Add(rbFiltroRolloCortado);
            pnlFiltroTipo.Controls.Add(rbFiltroHojas);
            pnlFiltroTipo.Controls.Add(rbFiltroGraphics);
            pnlFiltroTipo.Dock = DockStyle.Bottom;
            pnlFiltroTipo.Location = new Point(8, 29);
            pnlFiltroTipo.Name = "pnlFiltroTipo";
            pnlFiltroTipo.Size = new Size(335, 30);
            pnlFiltroTipo.TabIndex = 2;
            // WrapContents a true como red de seguridad: si con otra fuente o DPI los cinco
            // radios no caben en una fila, pasan a la de abajo en vez de recortarse.
            pnlFiltroTipo.WrapContents = true;
            //
            // rbFiltroTodos
            //
            // El que deja el catalogo entero. Va marcado de serie: al abrir el modulo se ve
            // todo, y para volver a verlo tras filtrar se pulsa este, sin trucos de doble clic.
            rbFiltroTodos.AutoSize = true;
            rbFiltroTodos.Checked = true;
            rbFiltroTodos.Font = new Font("Microsoft Sans Serif", 8F);
            rbFiltroTodos.ForeColor = Color.FromArgb(64, 64, 64);
            rbFiltroTodos.Location = new Point(2, 6);
            rbFiltroTodos.Margin = new Padding(2, 3, 2, 3);
            rbFiltroTodos.Name = "rbFiltroTodos";
            rbFiltroTodos.Size = new Size(48, 19);
            rbFiltroTodos.TabIndex = 0;
            rbFiltroTodos.TabStop = true;
            rbFiltroTodos.Text = "Todos";
            //
            // rbFiltroMaster
            //
            rbFiltroMaster.AutoSize = true;
            rbFiltroMaster.Checked = false;
            rbFiltroMaster.Font = new Font("Microsoft Sans Serif", 8F);
            rbFiltroMaster.ForeColor = Color.FromArgb(64, 64, 64);
            rbFiltroMaster.Location = new Point(54, 6);
            rbFiltroMaster.Margin = new Padding(2, 3, 2, 3);
            rbFiltroMaster.Name = "rbFiltroMaster";
            rbFiltroMaster.Size = new Size(57, 19);
            rbFiltroMaster.TabIndex = 1;
            rbFiltroMaster.TabStop = true;
            rbFiltroMaster.Text = "Master";
            //
            // rbFiltroRolloCortado
            //
            rbFiltroRolloCortado.AutoSize = true;
            rbFiltroRolloCortado.Checked = false;
            rbFiltroRolloCortado.Font = new Font("Microsoft Sans Serif", 8F);
            rbFiltroRolloCortado.ForeColor = Color.FromArgb(64, 64, 64);
            rbFiltroRolloCortado.Location = new Point(115, 6);
            rbFiltroRolloCortado.Margin = new Padding(2, 3, 2, 3);
            rbFiltroRolloCortado.Name = "rbFiltroRolloCortado";
            rbFiltroRolloCortado.Size = new Size(92, 19);
            rbFiltroRolloCortado.TabIndex = 2;
            rbFiltroRolloCortado.TabStop = true;
            rbFiltroRolloCortado.Text = "Rollos Cortados";
            //
            // rbFiltroHojas
            //
            rbFiltroHojas.AutoSize = true;
            rbFiltroHojas.Checked = false;
            rbFiltroHojas.Font = new Font("Microsoft Sans Serif", 8F);
            rbFiltroHojas.ForeColor = Color.FromArgb(64, 64, 64);
            rbFiltroHojas.Location = new Point(211, 6);
            rbFiltroHojas.Margin = new Padding(2, 3, 2, 3);
            rbFiltroHojas.Name = "rbFiltroHojas";
            rbFiltroHojas.Size = new Size(50, 19);
            rbFiltroHojas.TabIndex = 3;
            rbFiltroHojas.TabStop = true;
            rbFiltroHojas.Text = "Hojas";
            //
            // rbFiltroGraphics
            //
            rbFiltroGraphics.AutoSize = true;
            rbFiltroGraphics.Checked = false;
            rbFiltroGraphics.Font = new Font("Microsoft Sans Serif", 8F);
            rbFiltroGraphics.ForeColor = Color.FromArgb(64, 64, 64);
            rbFiltroGraphics.Location = new Point(265, 6);
            rbFiltroGraphics.Margin = new Padding(2, 3, 2, 3);
            rbFiltroGraphics.Name = "rbFiltroGraphics";
            rbFiltroGraphics.Size = new Size(63, 19);
            rbFiltroGraphics.TabIndex = 4;
            rbFiltroGraphics.TabStop = true;
            rbFiltroGraphics.Text = "Graphics";
            // 
            // txtSearch
            // 
            txtSearch.Dock = DockStyle.Bottom;
            txtSearch.Font = new Font("Microsoft Sans Serif", 9F);
            txtSearch.Location = new Point(8, 29);
            txtSearch.Margin = new Padding(4, 5, 4, 5);
            txtSearch.MinimumSize = new Size(1, 16);
            txtSearch.Name = "txtSearch";
            txtSearch.Padding = new Padding(6, 0, 6, 0);
            txtSearch.RectColor = Color.FromArgb(110, 190, 40);
            txtSearch.ShowText = false;
            txtSearch.Size = new Size(327, 29);
            txtSearch.TabIndex = 1;
            txtSearch.TextAlignment = ContentAlignment.MiddleLeft;
            txtSearch.Watermark = "Buscar por codigo, nombre o tipo...";
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(60, 110, 20);
            lblTitulo.Location = new Point(8, 6);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(327, 20);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "CATALOGO DE PRODUCTOS";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlContador
            // 
            pnlContador.BackColor = Color.FromArgb(110, 190, 40);
            pnlContador.Controls.Add(lblContadorDetalle);
            pnlContador.Controls.Add(lblContador);
            pnlContador.Dock = DockStyle.Bottom;
            pnlContador.Location = new Point(0, 615);
            pnlContador.Name = "pnlContador";
            pnlContador.Padding = new Padding(10, 4, 10, 4);
            pnlContador.Size = new Size(343, 60);
            pnlContador.TabIndex = 1;
            // 
            // lblContadorDetalle
            // 
            lblContadorDetalle.Dock = DockStyle.Fill;
            lblContadorDetalle.Font = new Font("Microsoft Sans Serif", 8F);
            lblContadorDetalle.ForeColor = Color.White;
            lblContadorDetalle.Location = new Point(10, 30);
            lblContadorDetalle.Name = "lblContadorDetalle";
            lblContadorDetalle.Size = new Size(323, 26);
            lblContadorDetalle.TabIndex = 1;
            lblContadorDetalle.Text = "Mostrando 0  -  Anulados 0  -  Total 0";
            lblContadorDetalle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblContador
            // 
            lblContador.Dock = DockStyle.Top;
            lblContador.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            lblContador.ForeColor = Color.White;
            lblContador.Location = new Point(10, 4);
            lblContador.Name = "lblContador";
            lblContador.Size = new Size(323, 26);
            lblContador.TabIndex = 0;
            lblContador.Text = "Productos existentes: 0";
            lblContador.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelDer
            // 
            panelDer.BackColor = Color.White;
            panelDer.Controls.Add(tabDetalle);
            panelDer.Controls.Add(barraHerramientas);
            panelDer.Dock = DockStyle.Fill;
            panelDer.Location = new Point(348, 3);
            panelDer.Name = "panelDer";
            panelDer.Padding = new Padding(8);
            panelDer.Size = new Size(799, 671);
            panelDer.TabIndex = 1;
            // 
            // tabDetalle
            // 
            tabDetalle.Controls.Add(tabDetalleProducto);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabDetalle.Font = new Font("Microsoft Sans Serif", 9F);
            tabDetalle.ItemSize = new Size(160, 32);
            tabDetalle.Location = new Point(8, 48);
            tabDetalle.MainPage = "";
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(783, 615);
            tabDetalle.SizeMode = TabSizeMode.Fixed;
            tabDetalle.TabIndex = 0;
            tabDetalle.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            tabDetalle.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // tabDetalleProducto
            // 
            tabDetalleProducto.BackColor = Color.White;
            tabDetalleProducto.Controls.Add(tlpDetalle);
            tabDetalleProducto.Location = new Point(0, 32);
            tabDetalleProducto.Name = "tabDetalleProducto";
            tabDetalleProducto.Size = new Size(783, 583);
            tabDetalleProducto.TabIndex = 0;
            tabDetalleProducto.Text = "Detalle";
            // 
            // tlpDetalle
            // 
            tlpDetalle.ColumnCount = 2;
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpDetalle.Controls.Add(lblDetalleTitulo, 0, 0);
            tlpDetalle.Controls.Add(lblDetId, 0, 1);
            tlpDetalle.Controls.Add(txtDetId, 1, 1);
            tlpDetalle.Controls.Add(lblDetNombre, 0, 2);
            tlpDetalle.Controls.Add(txtDetNombre, 1, 2);
            tlpDetalle.Controls.Add(lblDetTipo, 0, 9);
            tlpDetalle.SetColumnSpan(lblDetTipo, 2);
            tlpDetalle.Controls.Add(grpTipo, 0, 10);
            tlpDetalle.SetColumnSpan(grpTipo, 2);
            tlpDetalle.Controls.Add(lblDetReferencia, 0, 4);
            tlpDetalle.Controls.Add(txtDetReferencia, 1, 4);
            tlpDetalle.Controls.Add(lblDetCodebar, 0, 5);
            tlpDetalle.Controls.Add(txtDetCodebar, 1, 5);
            tlpDetalle.Controls.Add(lblDetPrecio, 0, 6);
            tlpDetalle.Controls.Add(txtDetPrecio, 1, 6);
            tlpDetalle.Controls.Add(lblDetCosto, 0, 7);
            tlpDetalle.Controls.Add(txtDetCosto, 1, 7);
            tlpDetalle.Controls.Add(lblDetRatio, 0, 8);
            tlpDetalle.Controls.Add(txtDetRatio, 1, 8);
            tlpDetalle.Controls.Add(lblDetEstado, 0, 11);
            tlpDetalle.Controls.Add(swDetEstado, 1, 11);
            tlpDetalle.Controls.Add(lblDetDescripcion, 0, 3);
            tlpDetalle.Controls.Add(txtDetDescripcion, 1, 3);
            tlpDetalle.AutoSize = true;
            tlpDetalle.AutoSizeMode = AutoSizeMode.GrowAndShrink;
            tlpDetalle.Dock = DockStyle.Top;
            tlpDetalle.Location = new Point(0, 0);
            tlpDetalle.Name = "tlpDetalle";
            tlpDetalle.Padding = new Padding(12, 10, 12, 10);
            tlpDetalle.RowCount = 12;
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 24F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 138F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpDetalle.Size = new Size(783, 544);
            tlpDetalle.TabIndex = 0;
            // 
            // lblDetalleTitulo
            // 
            tlpDetalle.SetColumnSpan(lblDetalleTitulo, 2);
            lblDetalleTitulo.Dock = DockStyle.Fill;
            lblDetalleTitulo.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetalleTitulo.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetalleTitulo.Location = new Point(15, 10);
            lblDetalleTitulo.Name = "lblDetalleTitulo";
            lblDetalleTitulo.Size = new Size(753, 34);
            lblDetalleTitulo.TabIndex = 0;
            lblDetalleTitulo.Text = "DETALLE DEL PRODUCTO";
            lblDetalleTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblDetId
            // 
            lblDetId.Dock = DockStyle.Fill;
            lblDetId.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetId.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetId.Location = new Point(15, 44);
            lblDetId.Name = "lblDetId";
            lblDetId.Padding = new Padding(2, 0, 0, 0);
            lblDetId.Size = new Size(154, 38);
            lblDetId.TabIndex = 1;
            lblDetId.Text = "Codigo";
            lblDetId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetId
            // 
            txtDetId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetId.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetId.Location = new Point(176, 49);
            txtDetId.Margin = new Padding(4, 5, 4, 5);
            txtDetId.MinimumSize = new Size(1, 16);
            txtDetId.Name = "txtDetId";
            txtDetId.Padding = new Padding(5);
            txtDetId.ShowText = false;
            txtDetId.Size = new Size(591, 28);
            txtDetId.TabIndex = 2;
            txtDetId.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetId.Watermark = "";
            // 
            // lblDetNombre
            // 
            lblDetNombre.Dock = DockStyle.Fill;
            lblDetNombre.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetNombre.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetNombre.Location = new Point(15, 82);
            lblDetNombre.Name = "lblDetNombre";
            lblDetNombre.Padding = new Padding(2, 0, 0, 0);
            lblDetNombre.Size = new Size(154, 38);
            lblDetNombre.TabIndex = 3;
            lblDetNombre.Text = "Nombre";
            lblDetNombre.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetNombre
            // 
            txtDetNombre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetNombre.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetNombre.Location = new Point(176, 87);
            txtDetNombre.Margin = new Padding(4, 5, 4, 5);
            txtDetNombre.MinimumSize = new Size(1, 16);
            txtDetNombre.Name = "txtDetNombre";
            txtDetNombre.Padding = new Padding(5);
            txtDetNombre.ShowText = false;
            txtDetNombre.Size = new Size(591, 28);
            txtDetNombre.TabIndex = 4;
            txtDetNombre.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetNombre.Watermark = "";
            // 
            // lblDetTipo
            // 
            lblDetTipo.Dock = DockStyle.Fill;
            lblDetTipo.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            lblDetTipo.ForeColor = Color.FromArgb(60, 110, 20);
            lblDetTipo.Location = new Point(15, 348);
            lblDetTipo.Name = "lblDetTipo";
            lblDetTipo.Padding = new Padding(2, 0, 0, 2);
            lblDetTipo.Size = new Size(753, 24);
            lblDetTipo.TabIndex = 17;
            lblDetTipo.Text = "Tipo de producto";
            lblDetTipo.TextAlign = ContentAlignment.BottomLeft;
            // 
            // grpTipo: caja de grupo con los cuatro radio de categoria en vertical
            // 
            grpTipo.Controls.Add(rbTipoMaster);
            grpTipo.Controls.Add(rbTipoRolloCortado);
            grpTipo.Controls.Add(rbTipoHojas);
            grpTipo.Controls.Add(rbTipoGraphics);
            grpTipo.Dock = DockStyle.Fill;
            grpTipo.FillColor = Color.White;
            grpTipo.Font = new Font("Microsoft Sans Serif", 12F);
            grpTipo.ForeColor = Color.FromArgb(64, 64, 64);
            grpTipo.Location = new Point(15, 372);
            grpTipo.Margin = new Padding(3, 0, 3, 3);
            grpTipo.Name = "grpTipo";
            grpTipo.Padding = new Padding(10, 2, 10, 8);
            grpTipo.RectColor = Verde;
            grpTipo.Size = new Size(753, 135);
            grpTipo.Style = Sunny.UI.UIStyle.Green;
            grpTipo.TabIndex = 18;
            grpTipo.Text = string.Empty;
            // 
            // rbTipoMaster
            // 
            rbTipoMaster.AutoSize = false;
            rbTipoMaster.Font = new Font("Microsoft Sans Serif", 12F);
            rbTipoMaster.Location = new Point(18, 20);
            rbTipoMaster.MinimumSize = new Size(1, 1);
            rbTipoMaster.Name = "rbTipoMaster";
            rbTipoMaster.RadioButtonColor = Verde;
            rbTipoMaster.ReadOnly = true;
            rbTipoMaster.Size = new Size(240, 26);
            rbTipoMaster.Style = Sunny.UI.UIStyle.Green;
            rbTipoMaster.TabIndex = 19;
            rbTipoMaster.Text = "Master";
            rbTipoMaster.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // rbTipoRolloCortado
            // 
            rbTipoRolloCortado.AutoSize = false;
            rbTipoRolloCortado.Font = new Font("Microsoft Sans Serif", 12F);
            rbTipoRolloCortado.Location = new Point(18, 48);
            rbTipoRolloCortado.MinimumSize = new Size(1, 1);
            rbTipoRolloCortado.Name = "rbTipoRolloCortado";
            rbTipoRolloCortado.RadioButtonColor = Verde;
            rbTipoRolloCortado.ReadOnly = true;
            rbTipoRolloCortado.Size = new Size(240, 26);
            rbTipoRolloCortado.Style = Sunny.UI.UIStyle.Green;
            rbTipoRolloCortado.TabIndex = 20;
            rbTipoRolloCortado.Text = "Rollos Cortados";
            rbTipoRolloCortado.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // rbTipoHojas
            // 
            rbTipoHojas.AutoSize = false;
            rbTipoHojas.Font = new Font("Microsoft Sans Serif", 12F);
            rbTipoHojas.Location = new Point(18, 76);
            rbTipoHojas.MinimumSize = new Size(1, 1);
            rbTipoHojas.Name = "rbTipoHojas";
            rbTipoHojas.RadioButtonColor = Verde;
            rbTipoHojas.ReadOnly = true;
            rbTipoHojas.Size = new Size(240, 26);
            rbTipoHojas.Style = Sunny.UI.UIStyle.Green;
            rbTipoHojas.TabIndex = 21;
            rbTipoHojas.Text = "Hojas";
            rbTipoHojas.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // rbTipoGraphics
            // 
            rbTipoGraphics.AutoSize = false;
            rbTipoGraphics.Font = new Font("Microsoft Sans Serif", 12F);
            rbTipoGraphics.Location = new Point(18, 104);
            rbTipoGraphics.MinimumSize = new Size(1, 1);
            rbTipoGraphics.Name = "rbTipoGraphics";
            rbTipoGraphics.RadioButtonColor = Verde;
            rbTipoGraphics.ReadOnly = true;
            rbTipoGraphics.Size = new Size(240, 26);
            rbTipoGraphics.Style = Sunny.UI.UIStyle.Green;
            rbTipoGraphics.TabIndex = 22;
            rbTipoGraphics.Text = "Graphics";
            rbTipoGraphics.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblDetReferencia
            // 
            lblDetReferencia.Dock = DockStyle.Fill;
            lblDetReferencia.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetReferencia.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetReferencia.Location = new Point(15, 158);
            lblDetReferencia.Name = "lblDetReferencia";
            lblDetReferencia.Padding = new Padding(2, 0, 0, 0);
            lblDetReferencia.Size = new Size(154, 38);
            lblDetReferencia.TabIndex = 7;
            lblDetReferencia.Text = "Referencia";
            lblDetReferencia.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetReferencia
            // 
            txtDetReferencia.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetReferencia.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetReferencia.Location = new Point(176, 163);
            txtDetReferencia.Margin = new Padding(4, 5, 4, 5);
            txtDetReferencia.MinimumSize = new Size(1, 16);
            txtDetReferencia.Name = "txtDetReferencia";
            txtDetReferencia.Padding = new Padding(5);
            txtDetReferencia.ShowText = false;
            txtDetReferencia.Size = new Size(591, 28);
            txtDetReferencia.TabIndex = 8;
            txtDetReferencia.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetReferencia.Watermark = "";
            // 
            // lblDetCodebar
            // 
            lblDetCodebar.Dock = DockStyle.Fill;
            lblDetCodebar.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetCodebar.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetCodebar.Location = new Point(15, 196);
            lblDetCodebar.Name = "lblDetCodebar";
            lblDetCodebar.Padding = new Padding(2, 0, 0, 0);
            lblDetCodebar.Size = new Size(154, 38);
            lblDetCodebar.TabIndex = 9;
            lblDetCodebar.Text = "Codigo de barra";
            lblDetCodebar.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetCodebar
            // 
            txtDetCodebar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetCodebar.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetCodebar.Location = new Point(176, 201);
            txtDetCodebar.Margin = new Padding(4, 5, 4, 5);
            txtDetCodebar.MinimumSize = new Size(1, 16);
            txtDetCodebar.Name = "txtDetCodebar";
            txtDetCodebar.Padding = new Padding(5);
            txtDetCodebar.ShowText = false;
            txtDetCodebar.Size = new Size(591, 28);
            txtDetCodebar.TabIndex = 10;
            txtDetCodebar.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetCodebar.Watermark = "";
            // 
            // lblDetPrecio
            // 
            lblDetPrecio.Dock = DockStyle.Fill;
            lblDetPrecio.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetPrecio.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetPrecio.Location = new Point(15, 234);
            lblDetPrecio.Name = "lblDetPrecio";
            lblDetPrecio.Padding = new Padding(2, 0, 0, 0);
            lblDetPrecio.Size = new Size(154, 38);
            lblDetPrecio.TabIndex = 11;
            lblDetPrecio.Text = "Precio";
            lblDetPrecio.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetPrecio
            // 
            txtDetPrecio.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetPrecio.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetPrecio.Location = new Point(176, 239);
            txtDetPrecio.Margin = new Padding(4, 5, 4, 5);
            txtDetPrecio.MinimumSize = new Size(1, 16);
            txtDetPrecio.Name = "txtDetPrecio";
            txtDetPrecio.Padding = new Padding(5);
            txtDetPrecio.ShowText = false;
            txtDetPrecio.Size = new Size(591, 28);
            txtDetPrecio.TabIndex = 12;
            txtDetPrecio.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetPrecio.Watermark = "";
            //
            // lblDetCosto
            //
            lblDetCosto.Dock = DockStyle.Fill;
            lblDetCosto.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetCosto.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetCosto.Location = new Point(15, 272);
            lblDetCosto.Name = "lblDetCosto";
            lblDetCosto.Padding = new Padding(2, 0, 0, 0);
            lblDetCosto.Size = new Size(154, 38);
            lblDetCosto.TabIndex = 13;
            lblDetCosto.Text = "Costo";
            lblDetCosto.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txtDetCosto
            //
            txtDetCosto.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetCosto.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetCosto.Location = new Point(176, 277);
            txtDetCosto.Margin = new Padding(4, 5, 4, 5);
            txtDetCosto.MinimumSize = new Size(1, 16);
            txtDetCosto.Name = "txtDetCosto";
            txtDetCosto.Padding = new Padding(5);
            txtDetCosto.ShowText = false;
            txtDetCosto.Size = new Size(591, 28);
            txtDetCosto.TabIndex = 14;
            txtDetCosto.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetCosto.Watermark = "";
            //
            // lblDetRatio
            //
            lblDetRatio.Dock = DockStyle.Fill;
            lblDetRatio.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetRatio.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetRatio.Location = new Point(15, 310);
            lblDetRatio.Name = "lblDetRatio";
            lblDetRatio.Padding = new Padding(2, 0, 0, 0);
            lblDetRatio.Size = new Size(154, 38);
            lblDetRatio.TabIndex = 15;
            lblDetRatio.Text = "Ratio";
            lblDetRatio.TextAlign = ContentAlignment.MiddleLeft;
            //
            // txtDetRatio
            //
            txtDetRatio.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetRatio.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetRatio.Location = new Point(176, 315);
            txtDetRatio.Margin = new Padding(4, 5, 4, 5);
            txtDetRatio.MinimumSize = new Size(1, 16);
            txtDetRatio.Name = "txtDetRatio";
            txtDetRatio.Padding = new Padding(5);
            txtDetRatio.ShowText = false;
            txtDetRatio.Size = new Size(591, 28);
            txtDetRatio.TabIndex = 16;
            txtDetRatio.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetRatio.Watermark = "";
            //
            // lblDetEstado: va de ultimo en el detalle, debajo de la caja de tipo
            //
            lblDetEstado.Dock = DockStyle.Fill;
            lblDetEstado.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetEstado.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetEstado.Location = new Point(15, 522);
            lblDetEstado.Name = "lblDetEstado";
            lblDetEstado.Padding = new Padding(2, 0, 0, 0);
            lblDetEstado.Size = new Size(154, 38);
            lblDetEstado.TabIndex = 17;
            lblDetEstado.Text = "Estado";
            lblDetEstado.TextAlign = ContentAlignment.MiddleLeft;
            //
            // swDetEstado: UISwitch con los dos estados del producto (Activo / Desactivado)
            // Dock.Left y no Dock.Fill: el switch ya trae su ancho del texto que muestra
            // ("Desactivado" es el mas largo) y con Fill se estiraba a los 591 px de la
            // columna, descuadrado frente a las cajas de arriba.
            //
            swDetEstado.Dock = DockStyle.Left;
            swDetEstado.Font = new Font("Microsoft Sans Serif", 12F);
            swDetEstado.Location = new Point(180, 527);
            swDetEstado.Margin = new Padding(4, 5, 4, 5);
            swDetEstado.MinimumSize = new Size(1, 16);
            swDetEstado.Name = "swDetEstado";
            swDetEstado.Size = new Size(120, 28);
            swDetEstado.TabIndex = 18;
            // 
            // lblDetDescripcion
            // 
            lblDetDescripcion.Dock = DockStyle.Fill;
            lblDetDescripcion.Font = new Font("Microsoft Sans Serif", 12F);
            lblDetDescripcion.ForeColor = Color.FromArgb(48, 48, 48);
            lblDetDescripcion.Location = new Point(15, 120);
            lblDetDescripcion.Name = "lblDetDescripcion";
            lblDetDescripcion.Padding = new Padding(2, 0, 0, 0);
            lblDetDescripcion.Size = new Size(154, 38);
            lblDetDescripcion.TabIndex = 5;
            lblDetDescripcion.Text = "Descripcion";
            lblDetDescripcion.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetDescripcion
            // 
            txtDetDescripcion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetDescripcion.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetDescripcion.Location = new Point(176, 125);
            txtDetDescripcion.Margin = new Padding(4, 5, 4, 5);
            txtDetDescripcion.MinimumSize = new Size(1, 16);
            txtDetDescripcion.Name = "txtDetDescripcion";
            txtDetDescripcion.Padding = new Padding(5);
            txtDetDescripcion.ShowText = false;
            txtDetDescripcion.Size = new Size(591, 28);
            txtDetDescripcion.TabIndex = 6;
            txtDetDescripcion.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetDescripcion.Watermark = "";
            // 
            // barraHerramientas
            // 
            barraHerramientas.AutoSize = false;
            barraHerramientas.BackColor = Color.FromArgb(225, 225, 225);
            barraHerramientas.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            barraHerramientas.ForeColor = Color.FromArgb(80, 80, 80);
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevoProducto, btnEditarProducto, btnImportarProducto, btnGuardarProducto, btnCancelarProducto });
            barraHerramientas.Location = new Point(8, 8);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 2, 0, 2);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(783, 40);
            barraHerramientas.TabIndex = 1;
            barraHerramientas.Text = "barraHerramientas";
            // 
            // btnNuevoProducto
            // 
            btnNuevoProducto.AutoSize = false;
            btnNuevoProducto.BackColor = Color.Transparent;
            btnNuevoProducto.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnNuevoProducto.ForeColor = Color.FromArgb(80, 80, 80);
            btnNuevoProducto.Image = Properties.Resources.add_file_32px;
            btnNuevoProducto.ImageAlign = ContentAlignment.MiddleLeft;
            btnNuevoProducto.ImageScaling = ToolStripItemImageScaling.None;
            btnNuevoProducto.Margin = new Padding(4, 1, 0, 2);
            btnNuevoProducto.Name = "btnNuevoProducto";
            btnNuevoProducto.Size = new Size(92, 36);
            btnNuevoProducto.Text = "Nuevo";
            btnNuevoProducto.ToolTipText = "Nuevo producto";
            // 
            // btnEditarProducto
            // 
            btnEditarProducto.AutoSize = false;
            btnEditarProducto.BackColor = Color.Transparent;
            btnEditarProducto.Enabled = false;
            btnEditarProducto.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnEditarProducto.ForeColor = Color.FromArgb(80, 80, 80);
            btnEditarProducto.Image = Properties.Resources.edit_24px;
            btnEditarProducto.ImageAlign = ContentAlignment.MiddleLeft;
            btnEditarProducto.ImageScaling = ToolStripItemImageScaling.None;
            btnEditarProducto.Margin = new Padding(4, 1, 0, 2);
            btnEditarProducto.Name = "btnEditarProducto";
            btnEditarProducto.Size = new Size(92, 36);
            btnEditarProducto.Text = "Editar";
            btnEditarProducto.ToolTipText = "Editar el producto seleccionado";
            //
            // btnImportarProducto
            //
            // Sin icono: no hay ninguno de Excel de 24 px, y los de 48 px (excel_48,
            // excel48_48) no caben en un boton de 36 px de alto con ImageScaling = None.
            btnImportarProducto.AutoSize = false;
            btnImportarProducto.BackColor = Color.Transparent;
            btnImportarProducto.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnImportarProducto.ForeColor = Color.FromArgb(80, 80, 80);
            btnImportarProducto.ImageAlign = ContentAlignment.MiddleLeft;
            btnImportarProducto.ImageScaling = ToolStripItemImageScaling.None;
            btnImportarProducto.Margin = new Padding(4, 1, 0, 2);
            btnImportarProducto.Name = "btnImportarProducto";
            btnImportarProducto.Size = new Size(92, 36);
            btnImportarProducto.Text = "Importar";
            btnImportarProducto.ToolTipText = "Crear una hoja de Excel con todos los productos";
            //
            // btnGuardarProducto
            //
            // Sin icono: Properties.Resources no tiene ninguno de guardado. El texto basta y
            // evita crear un recurso nuevo solo para esta pantalla.
            btnGuardarProducto.AutoSize = false;
            btnGuardarProducto.BackColor = Color.Transparent;
            btnGuardarProducto.Enabled = false;
            btnGuardarProducto.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnGuardarProducto.ForeColor = Color.FromArgb(80, 80, 80);
            btnGuardarProducto.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardarProducto.ImageScaling = ToolStripItemImageScaling.None;
            btnGuardarProducto.Margin = new Padding(4, 1, 0, 2);
            btnGuardarProducto.Name = "btnGuardarProducto";
            btnGuardarProducto.Size = new Size(92, 36);
            btnGuardarProducto.Text = "Guardar";
            btnGuardarProducto.ToolTipText = "Guardar el producto";
            btnGuardarProducto.Visible = false;
            //
            // btnCancelarProducto
            //
            // Sin icono a proposito: cancel_24px es una X ROJA (rgb 235,53,66) y junto a un
            // Guardar se lee como borrar o como error, no como descartar. Se deja igual que
            // Guardar, solo con el texto.
            btnCancelarProducto.AutoSize = false;
            btnCancelarProducto.BackColor = Color.Transparent;
            btnCancelarProducto.Enabled = false;
            btnCancelarProducto.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnCancelarProducto.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancelarProducto.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelarProducto.ImageScaling = ToolStripItemImageScaling.None;
            btnCancelarProducto.Margin = new Padding(4, 1, 0, 2);
            btnCancelarProducto.Name = "btnCancelarProducto";
            btnCancelarProducto.Size = new Size(92, 36);
            btnCancelarProducto.Text = "Cancelar";
            btnCancelarProducto.ToolTipText = "Descartar los cambios";
            btnCancelarProducto.Visible = false;
            // 
            // FrmProductos
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1150, 712);
            Controls.Add(tlpRoot);
            MinimumSize = new Size(980, 622);
            Name = "FrmProductos";
            Text = "Productos";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 712);
            tlpRoot.ResumeLayout(false);
            panelIzq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridProductos).EndInit();
            pnlBuscador.ResumeLayout(false);
            pnlFiltroTipo.ResumeLayout(false);
            pnlContador.ResumeLayout(false);
            panelDer.ResumeLayout(false);
            tabDetalle.ResumeLayout(false);
            tabDetalleProducto.ResumeLayout(false);
            tlpDetalle.ResumeLayout(false);
            grpTipo.ResumeLayout(false);
            barraHerramientas.ResumeLayout(false);
            barraHerramientas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpRoot;
        private Panel panelIzq;
        private Sunny.UI.UIDataGridView gridProductos;
        private DataGridViewTextBoxColumn colProductId;
        private DataGridViewTextBoxColumn colProductName;
        private DataGridViewTextBoxColumn colTipo;
        private Panel pnlBuscador;
        private FlowLayoutPanel pnlFiltroTipo;
        private Sunny.UI.UIRadioButton rbFiltroTodos;
        private Sunny.UI.UIRadioButton rbFiltroMaster;
        private Sunny.UI.UIRadioButton rbFiltroRolloCortado;
        private Sunny.UI.UIRadioButton rbFiltroHojas;
        private Sunny.UI.UIRadioButton rbFiltroGraphics;
        private Sunny.UI.UILabel lblTitulo;
        private Sunny.UI.UITextBox txtSearch;
        private Panel pnlContador;
        private Sunny.UI.UILabel lblContador;
        private Sunny.UI.UILabel lblContadorDetalle;
        private Panel panelDer;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabDetalleProducto;
        private TableLayoutPanel tlpDetalle;
        private Sunny.UI.UILabel lblDetalleTitulo;
        private Sunny.UI.UILabel lblDetId;
        private Sunny.UI.UITextBox txtDetId;
        private Sunny.UI.UILabel lblDetNombre;
        private Sunny.UI.UITextBox txtDetNombre;
        private Sunny.UI.UILabel lblDetTipo;
        private Sunny.UI.UIGroupBox grpTipo;
        private Sunny.UI.UIRadioButton rbTipoMaster;
        private Sunny.UI.UIRadioButton rbTipoRolloCortado;
        private Sunny.UI.UIRadioButton rbTipoHojas;
        private Sunny.UI.UIRadioButton rbTipoGraphics;
        private Sunny.UI.UILabel lblDetReferencia;
        private Sunny.UI.UITextBox txtDetReferencia;
        private Sunny.UI.UILabel lblDetCodebar;
        private Sunny.UI.UITextBox txtDetCodebar;
        private Sunny.UI.UILabel lblDetPrecio;
        private Sunny.UI.UITextBox txtDetPrecio;
        private Sunny.UI.UILabel lblDetCosto;
        private Sunny.UI.UITextBox txtDetCosto;
        private Sunny.UI.UILabel lblDetRatio;
        private Sunny.UI.UITextBox txtDetRatio;
        private Sunny.UI.UILabel lblDetEstado;
        private Sunny.UI.UISwitch swDetEstado;
        private Sunny.UI.UILabel lblDetDescripcion;
        private Sunny.UI.UITextBox txtDetDescripcion;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevoProducto;
        private ToolStripButton btnEditarProducto;
        private ToolStripButton btnImportarProducto;
        private ToolStripButton btnGuardarProducto;
        private ToolStripButton btnCancelarProducto;
    }
}
