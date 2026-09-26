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
            panelIzq = new Panel();
            flpProductos = new FlowLayoutPanel();
            pnlBuscador = new Panel();
            cboFiltroCategoria = new Sunny.UI.UIComboBox();
            txtSearch = new Sunny.UI.UITextBox();
            lblTitulo = new Sunny.UI.UILabel();
            pnlSummary = new Panel();
            lblTotal = new Sunny.UI.UILabel();
            panelDerecho = new Panel();
            tabDetalle = new Sunny.UI.UITabControl();
            tabGeneral = new TabPage();
            pnlGeneral = new Panel();
            grpDetCategoria = new GroupBox();
            rdoDetHojas = new RadioButton();
            rdoDetGraphics = new RadioButton();
            rdoDetRollo = new RadioButton();
            rdoDetMaster = new RadioButton();
            chkDetAnulado = new Sunny.UI.UICheckBox();
            txtDetRatio = new Sunny.UI.UITextBox();
            lblDetRatio = new Sunny.UI.UILabel();
            txtDetPrecio = new Sunny.UI.UITextBox();
            lblDetPrecio = new Sunny.UI.UILabel();
            txtDetCategoria = new Sunny.UI.UITextBox();
            lblDetCategoria = new Sunny.UI.UILabel();
            txtDetCodebar = new Sunny.UI.UITextBox();
            lblDetCodebar = new Sunny.UI.UILabel();
            txtDetRef = new Sunny.UI.UITextBox();
            lblDetRef = new Sunny.UI.UILabel();
            txtDetDescrip = new Sunny.UI.UITextBox();
            lblDetDescrip = new Sunny.UI.UILabel();
            txtDetNombre = new Sunny.UI.UITextBox();
            lblDetNombre = new Sunny.UI.UILabel();
            txtDetId = new Sunny.UI.UITextBox();
            lblDetId = new Sunny.UI.UILabel();
            tabNuevoProducto = new TabPage();
            pnlNuevo = new Panel();
            btnGuardarNuevo = new Sunny.UI.UIButton();
            txtNuevoRatio = new Sunny.UI.UITextBox();
            lblNuevoRatio = new Sunny.UI.UILabel();
            txtNuevoPrecio = new Sunny.UI.UITextBox();
            lblNuevoPrecio = new Sunny.UI.UILabel();
            cboNuevoCategoria = new Sunny.UI.UIComboBox();
            lblNuevoCategoria = new Sunny.UI.UILabel();
            txtNuevoCodebar = new Sunny.UI.UITextBox();
            lblNuevoCodebar = new Sunny.UI.UILabel();
            txtNuevoRef = new Sunny.UI.UITextBox();
            lblNuevoRef = new Sunny.UI.UILabel();
            txtNuevoDescrip = new Sunny.UI.UITextBox();
            lblNuevoDescrip = new Sunny.UI.UILabel();
            txtNuevoNombre = new Sunny.UI.UITextBox();
            lblNuevoNombre = new Sunny.UI.UILabel();
            txtNuevoId = new Sunny.UI.UITextBox();
            lblNuevoId = new Sunny.UI.UILabel();
            barraHerramientas = new ToolStrip();
            btnNuevo = new ToolStripButton();
            btnEditar = new ToolStripButton();
            btnImportar = new ToolStripButton();
            txt_Costo = new Sunny.UI.UITextBox();
            uiLabel1 = new Sunny.UI.UILabel();
            panelIzq.SuspendLayout();
            pnlBuscador.SuspendLayout();
            pnlSummary.SuspendLayout();
            panelDerecho.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabGeneral.SuspendLayout();
            pnlGeneral.SuspendLayout();
            grpDetCategoria.SuspendLayout();
            tabNuevoProducto.SuspendLayout();
            pnlNuevo.SuspendLayout();
            barraHerramientas.SuspendLayout();
            SuspendLayout();
            // 
            // panelIzq
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(flpProductos);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Controls.Add(pnlSummary);
            panelIzq.Dock = DockStyle.Left;
            panelIzq.Location = new Point(0, 103);
            panelIzq.Name = "panelIzq";
            panelIzq.Size = new Size(370, 547);
            panelIzq.TabIndex = 0;
            // 
            // flpProductos
            // 
            flpProductos.AutoScroll = true;
            flpProductos.BackColor = Color.FromArgb(245, 245, 245);
            flpProductos.Dock = DockStyle.Fill;
            flpProductos.FlowDirection = FlowDirection.TopDown;
            flpProductos.Location = new Point(0, 42);
            flpProductos.Margin = new Padding(0);
            flpProductos.Name = "flpProductos";
            flpProductos.Padding = new Padding(4);
            flpProductos.Size = new Size(368, 459);
            flpProductos.TabIndex = 2;
            flpProductos.WrapContents = false;
            // 
            // pnlBuscador
            // 
            pnlBuscador.BackColor = Color.White;
            pnlBuscador.Controls.Add(cboFiltroCategoria);
            pnlBuscador.Controls.Add(txtSearch);
            pnlBuscador.Controls.Add(lblTitulo);
            pnlBuscador.Dock = DockStyle.Top;
            pnlBuscador.Location = new Point(0, 0);
            pnlBuscador.Name = "pnlBuscador";
            pnlBuscador.Padding = new Padding(10);
            pnlBuscador.Size = new Size(368, 42);
            pnlBuscador.TabIndex = 0;
            // 
            // cboFiltroCategoria
            // 
            cboFiltroCategoria.DataSource = null;
            cboFiltroCategoria.Dock = DockStyle.Top;
            cboFiltroCategoria.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            cboFiltroCategoria.FillColor = Color.White;
            cboFiltroCategoria.Font = new Font("Microsoft Sans Serif", 9F);
            cboFiltroCategoria.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cboFiltroCategoria.Items.AddRange(new object[] { "Todos", "Master", "Rollo Cortado", "Resma", "Graphics" });
            cboFiltroCategoria.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cboFiltroCategoria.Location = new Point(10, 59);
            cboFiltroCategoria.Margin = new Padding(0, 4, 0, 0);
            cboFiltroCategoria.MinimumSize = new Size(63, 0);
            cboFiltroCategoria.Name = "cboFiltroCategoria";
            cboFiltroCategoria.Padding = new Padding(0, 0, 30, 2);
            cboFiltroCategoria.RectColor = Color.FromArgb(110, 190, 40);
            cboFiltroCategoria.Size = new Size(348, 29);
            cboFiltroCategoria.SymbolSize = 24;
            cboFiltroCategoria.TabIndex = 2;
            cboFiltroCategoria.TextAlignment = ContentAlignment.MiddleLeft;
            cboFiltroCategoria.Visible = false;
            cboFiltroCategoria.Watermark = "";
            // 
            // txtSearch
            // 
            txtSearch.Dock = DockStyle.Top;
            txtSearch.Font = new Font("Microsoft Sans Serif", 9F);
            txtSearch.Location = new Point(10, 30);
            txtSearch.Margin = new Padding(0, 2, 0, 2);
            txtSearch.MinimumSize = new Size(1, 16);
            txtSearch.Name = "txtSearch";
            txtSearch.Padding = new Padding(5);
            txtSearch.RectColor = Color.FromArgb(110, 190, 40);
            txtSearch.ShowText = false;
            txtSearch.Size = new Size(348, 29);
            txtSearch.TabIndex = 1;
            txtSearch.TextAlignment = ContentAlignment.MiddleLeft;
            txtSearch.Visible = false;
            txtSearch.Watermark = "Buscar por ID / Nombre / Categoría…";
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Top;
            lblTitulo.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(40, 40, 40);
            lblTitulo.Location = new Point(10, 10);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(348, 20);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "Productos";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // pnlSummary
            // 
            pnlSummary.BackColor = Color.FromArgb(110, 190, 40);
            pnlSummary.Controls.Add(lblTotal);
            pnlSummary.Dock = DockStyle.Bottom;
            pnlSummary.Location = new Point(0, 501);
            pnlSummary.Name = "pnlSummary";
            pnlSummary.Size = new Size(368, 44);
            pnlSummary.TabIndex = 1;
            // 
            // lblTotal
            // 
            lblTotal.Dock = DockStyle.Fill;
            lblTotal.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            lblTotal.ForeColor = Color.White;
            lblTotal.Location = new Point(0, 0);
            lblTotal.Name = "lblTotal";
            lblTotal.Padding = new Padding(10, 0, 0, 0);
            lblTotal.Size = new Size(368, 44);
            lblTotal.TabIndex = 0;
            lblTotal.Text = "Total: 0 productos";
            lblTotal.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelDerecho
            // 
            panelDerecho.BackColor = Color.White;
            panelDerecho.Controls.Add(tabDetalle);
            panelDerecho.Dock = DockStyle.Fill;
            panelDerecho.Location = new Point(370, 103);
            panelDerecho.Name = "panelDerecho";
            panelDerecho.Padding = new Padding(10);
            panelDerecho.Size = new Size(780, 547);
            panelDerecho.TabIndex = 1;
            // 
            // tabDetalle
            // 
            tabDetalle.Controls.Add(tabGeneral);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabDetalle.Font = new Font("Microsoft Sans Serif", 9F);
            tabDetalle.ItemSize = new Size(120, 32);
            tabDetalle.Location = new Point(10, 10);
            tabDetalle.MainPage = "";
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(760, 527);
            tabDetalle.SizeMode = TabSizeMode.Fixed;
            tabDetalle.TabIndex = 0;
            tabDetalle.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            tabDetalle.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // tabGeneral
            // 
            tabGeneral.BackColor = Color.White;
            tabGeneral.Controls.Add(pnlGeneral);
            tabGeneral.Location = new Point(0, 32);
            tabGeneral.Name = "tabGeneral";
            tabGeneral.Size = new Size(760, 495);
            tabGeneral.TabIndex = 0;
            tabGeneral.Text = "General";
            // 
            // pnlGeneral
            // 
            pnlGeneral.AutoScroll = true;
            pnlGeneral.BackColor = Color.White;
            pnlGeneral.Controls.Add(txt_Costo);
            pnlGeneral.Controls.Add(uiLabel1);
            pnlGeneral.Controls.Add(grpDetCategoria);
            pnlGeneral.Controls.Add(chkDetAnulado);
            pnlGeneral.Controls.Add(txtDetRatio);
            pnlGeneral.Controls.Add(lblDetRatio);
            pnlGeneral.Controls.Add(txtDetPrecio);
            pnlGeneral.Controls.Add(lblDetPrecio);
            pnlGeneral.Controls.Add(txtDetCategoria);
            pnlGeneral.Controls.Add(lblDetCategoria);
            pnlGeneral.Controls.Add(txtDetCodebar);
            pnlGeneral.Controls.Add(lblDetCodebar);
            pnlGeneral.Controls.Add(txtDetRef);
            pnlGeneral.Controls.Add(lblDetRef);
            pnlGeneral.Controls.Add(txtDetDescrip);
            pnlGeneral.Controls.Add(lblDetDescrip);
            pnlGeneral.Controls.Add(txtDetNombre);
            pnlGeneral.Controls.Add(lblDetNombre);
            pnlGeneral.Controls.Add(txtDetId);
            pnlGeneral.Controls.Add(lblDetId);
            pnlGeneral.Dock = DockStyle.Fill;
            pnlGeneral.Location = new Point(0, 0);
            pnlGeneral.Name = "pnlGeneral";
            pnlGeneral.Padding = new Padding(15);
            pnlGeneral.Size = new Size(760, 495);
            pnlGeneral.TabIndex = 0;
            // 
            // grpDetCategoria
            // 
            grpDetCategoria.BackColor = Color.White;
            grpDetCategoria.Controls.Add(rdoDetHojas);
            grpDetCategoria.Controls.Add(rdoDetGraphics);
            grpDetCategoria.Controls.Add(rdoDetRollo);
            grpDetCategoria.Controls.Add(rdoDetMaster);
            grpDetCategoria.Enabled = false;
            grpDetCategoria.Font = new Font("Microsoft Sans Serif", 8.5F, FontStyle.Bold);
            grpDetCategoria.ForeColor = Color.FromArgb(80, 80, 80);
            grpDetCategoria.Location = new Point(18, 320);
            grpDetCategoria.Name = "grpDetCategoria";
            grpDetCategoria.Padding = new Padding(10, 6, 10, 6);
            grpDetCategoria.Size = new Size(153, 157);
            grpDetCategoria.TabIndex = 17;
            grpDetCategoria.TabStop = false;
            grpDetCategoria.Text = "Categoría";
            // 
            // rdoDetHojas
            // 
            rdoDetHojas.AutoSize = true;
            rdoDetHojas.Font = new Font("Microsoft Sans Serif", 9F);
            rdoDetHojas.ForeColor = Color.FromArgb(40, 40, 40);
            rdoDetHojas.Location = new Point(13, 103);
            rdoDetHojas.Name = "rdoDetHojas";
            rdoDetHojas.Size = new Size(57, 19);
            rdoDetHojas.TabIndex = 3;
            rdoDetHojas.TabStop = true;
            rdoDetHojas.Text = "Hojas";
            rdoDetHojas.UseVisualStyleBackColor = true;
            // 
            // rdoDetGraphics
            // 
            rdoDetGraphics.AutoSize = true;
            rdoDetGraphics.Font = new Font("Microsoft Sans Serif", 9F);
            rdoDetGraphics.ForeColor = Color.FromArgb(40, 40, 40);
            rdoDetGraphics.Location = new Point(14, 78);
            rdoDetGraphics.Name = "rdoDetGraphics";
            rdoDetGraphics.Size = new Size(74, 19);
            rdoDetGraphics.TabIndex = 2;
            rdoDetGraphics.TabStop = true;
            rdoDetGraphics.Text = "Graphics";
            rdoDetGraphics.UseVisualStyleBackColor = true;
            // 
            // rdoDetRollo
            // 
            rdoDetRollo.AutoSize = true;
            rdoDetRollo.Font = new Font("Microsoft Sans Serif", 9F);
            rdoDetRollo.ForeColor = Color.FromArgb(40, 40, 40);
            rdoDetRollo.Location = new Point(14, 53);
            rdoDetRollo.Name = "rdoDetRollo";
            rdoDetRollo.Size = new Size(100, 19);
            rdoDetRollo.TabIndex = 1;
            rdoDetRollo.TabStop = true;
            rdoDetRollo.Text = "Rollo Cortado";
            rdoDetRollo.UseVisualStyleBackColor = true;
            // 
            // rdoDetMaster
            // 
            rdoDetMaster.AutoSize = true;
            rdoDetMaster.Font = new Font("Microsoft Sans Serif", 9F);
            rdoDetMaster.ForeColor = Color.FromArgb(40, 40, 40);
            rdoDetMaster.Location = new Point(14, 28);
            rdoDetMaster.Name = "rdoDetMaster";
            rdoDetMaster.Size = new Size(63, 19);
            rdoDetMaster.TabIndex = 0;
            rdoDetMaster.TabStop = true;
            rdoDetMaster.Text = "Master";
            rdoDetMaster.UseVisualStyleBackColor = true;
            // 
            // chkDetAnulado
            // 
            chkDetAnulado.CheckBoxColor = Color.FromArgb(110, 190, 40);
            chkDetAnulado.Cursor = Cursors.Hand;
            chkDetAnulado.Enabled = false;
            chkDetAnulado.Font = new Font("Microsoft Sans Serif", 9F);
            chkDetAnulado.ForeColor = Color.FromArgb(220, 80, 60);
            chkDetAnulado.Location = new Point(632, 431);
            chkDetAnulado.MinimumSize = new Size(1, 1);
            chkDetAnulado.Name = "chkDetAnulado";
            chkDetAnulado.Size = new Size(110, 29);
            chkDetAnulado.TabIndex = 16;
            chkDetAnulado.Text = "Anulado";
            // 
            // txtDetRatio
            // 
            txtDetRatio.FillColor = Color.WhiteSmoke;
            txtDetRatio.Font = new Font("Microsoft Sans Serif", 9F);
            txtDetRatio.Location = new Point(500, 273);
            txtDetRatio.Margin = new Padding(4, 5, 4, 5);
            txtDetRatio.MinimumSize = new Size(1, 16);
            txtDetRatio.Name = "txtDetRatio";
            txtDetRatio.Padding = new Padding(5);
            txtDetRatio.ReadOnly = true;
            txtDetRatio.RectColor = Color.FromArgb(220, 220, 220);
            txtDetRatio.ShowText = false;
            txtDetRatio.Size = new Size(120, 29);
            txtDetRatio.TabIndex = 15;
            txtDetRatio.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetRatio.Watermark = "";
            // 
            // lblDetRatio
            // 
            lblDetRatio.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetRatio.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetRatio.Location = new Point(500, 250);
            lblDetRatio.Name = "lblDetRatio";
            lblDetRatio.Size = new Size(110, 23);
            lblDetRatio.TabIndex = 14;
            lblDetRatio.Text = "Ratio";
            lblDetRatio.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetPrecio
            // 
            txtDetPrecio.FillColor = Color.WhiteSmoke;
            txtDetPrecio.Font = new Font("Microsoft Sans Serif", 9F);
            txtDetPrecio.Location = new Point(18, 273);
            txtDetPrecio.Margin = new Padding(4, 5, 4, 5);
            txtDetPrecio.MinimumSize = new Size(1, 16);
            txtDetPrecio.Name = "txtDetPrecio";
            txtDetPrecio.Padding = new Padding(5);
            txtDetPrecio.ReadOnly = true;
            txtDetPrecio.RectColor = Color.FromArgb(220, 220, 220);
            txtDetPrecio.ShowText = false;
            txtDetPrecio.Size = new Size(140, 29);
            txtDetPrecio.TabIndex = 13;
            txtDetPrecio.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetPrecio.Watermark = "";
            // 
            // lblDetPrecio
            // 
            lblDetPrecio.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetPrecio.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetPrecio.Location = new Point(18, 250);
            lblDetPrecio.Name = "lblDetPrecio";
            lblDetPrecio.Size = new Size(110, 23);
            lblDetPrecio.TabIndex = 12;
            lblDetPrecio.Text = "Precio";
            lblDetPrecio.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetCategoria
            // 
            txtDetCategoria.FillColor = Color.WhiteSmoke;
            txtDetCategoria.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            txtDetCategoria.Location = new Point(500, 205);
            txtDetCategoria.Margin = new Padding(4, 5, 4, 5);
            txtDetCategoria.MinimumSize = new Size(1, 16);
            txtDetCategoria.Name = "txtDetCategoria";
            txtDetCategoria.Padding = new Padding(5);
            txtDetCategoria.ReadOnly = true;
            txtDetCategoria.RectColor = Color.FromArgb(110, 190, 40);
            txtDetCategoria.ShowText = false;
            txtDetCategoria.Size = new Size(230, 29);
            txtDetCategoria.TabIndex = 11;
            txtDetCategoria.TextAlignment = ContentAlignment.MiddleCenter;
            txtDetCategoria.Watermark = "";
            // 
            // lblDetCategoria
            // 
            lblDetCategoria.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetCategoria.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetCategoria.Location = new Point(500, 182);
            lblDetCategoria.Name = "lblDetCategoria";
            lblDetCategoria.Size = new Size(110, 23);
            lblDetCategoria.TabIndex = 10;
            lblDetCategoria.Text = "Categoría";
            lblDetCategoria.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetCodebar
            // 
            txtDetCodebar.FillColor = Color.WhiteSmoke;
            txtDetCodebar.Font = new Font("Microsoft Sans Serif", 9F);
            txtDetCodebar.Location = new Point(260, 205);
            txtDetCodebar.Margin = new Padding(4, 5, 4, 5);
            txtDetCodebar.MinimumSize = new Size(1, 16);
            txtDetCodebar.Name = "txtDetCodebar";
            txtDetCodebar.Padding = new Padding(5);
            txtDetCodebar.ReadOnly = true;
            txtDetCodebar.RectColor = Color.FromArgb(220, 220, 220);
            txtDetCodebar.ShowText = false;
            txtDetCodebar.Size = new Size(220, 29);
            txtDetCodebar.TabIndex = 9;
            txtDetCodebar.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetCodebar.Watermark = "";
            // 
            // lblDetCodebar
            // 
            lblDetCodebar.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetCodebar.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetCodebar.Location = new Point(260, 182);
            lblDetCodebar.Name = "lblDetCodebar";
            lblDetCodebar.Size = new Size(110, 23);
            lblDetCodebar.TabIndex = 8;
            lblDetCodebar.Text = "Código Barra";
            lblDetCodebar.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetRef
            // 
            txtDetRef.FillColor = Color.WhiteSmoke;
            txtDetRef.Font = new Font("Microsoft Sans Serif", 9F);
            txtDetRef.Location = new Point(18, 205);
            txtDetRef.Margin = new Padding(4, 5, 4, 5);
            txtDetRef.MinimumSize = new Size(1, 16);
            txtDetRef.Name = "txtDetRef";
            txtDetRef.Padding = new Padding(5);
            txtDetRef.ReadOnly = true;
            txtDetRef.RectColor = Color.FromArgb(220, 220, 220);
            txtDetRef.ShowText = false;
            txtDetRef.Size = new Size(220, 29);
            txtDetRef.TabIndex = 7;
            txtDetRef.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetRef.Watermark = "";
            // 
            // lblDetRef
            // 
            lblDetRef.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetRef.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetRef.Location = new Point(18, 182);
            lblDetRef.Name = "lblDetRef";
            lblDetRef.Size = new Size(110, 23);
            lblDetRef.TabIndex = 6;
            lblDetRef.Text = "Referencia";
            lblDetRef.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetDescrip
            // 
            txtDetDescrip.FillColor = Color.WhiteSmoke;
            txtDetDescrip.Font = new Font("Microsoft Sans Serif", 9F);
            txtDetDescrip.Location = new Point(18, 108);
            txtDetDescrip.Margin = new Padding(4, 5, 4, 5);
            txtDetDescrip.MinimumSize = new Size(1, 16);
            txtDetDescrip.Multiline = true;
            txtDetDescrip.Name = "txtDetDescrip";
            txtDetDescrip.Padding = new Padding(5);
            txtDetDescrip.ReadOnly = true;
            txtDetDescrip.RectColor = Color.FromArgb(220, 220, 220);
            txtDetDescrip.ShowText = false;
            txtDetDescrip.Size = new Size(712, 60);
            txtDetDescrip.TabIndex = 5;
            txtDetDescrip.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetDescrip.Watermark = "";
            // 
            // lblDetDescrip
            // 
            lblDetDescrip.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetDescrip.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetDescrip.Location = new Point(18, 85);
            lblDetDescrip.Name = "lblDetDescrip";
            lblDetDescrip.Size = new Size(110, 23);
            lblDetDescrip.TabIndex = 4;
            lblDetDescrip.Text = "Descripción";
            lblDetDescrip.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetNombre
            // 
            txtDetNombre.FillColor = Color.WhiteSmoke;
            txtDetNombre.Font = new Font("Microsoft Sans Serif", 9F);
            txtDetNombre.Location = new Point(260, 41);
            txtDetNombre.Margin = new Padding(4, 5, 4, 5);
            txtDetNombre.MinimumSize = new Size(1, 16);
            txtDetNombre.Name = "txtDetNombre";
            txtDetNombre.Padding = new Padding(5);
            txtDetNombre.ReadOnly = true;
            txtDetNombre.RectColor = Color.FromArgb(220, 220, 220);
            txtDetNombre.ShowText = false;
            txtDetNombre.Size = new Size(470, 29);
            txtDetNombre.TabIndex = 3;
            txtDetNombre.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetNombre.Watermark = "";
            // 
            // lblDetNombre
            // 
            lblDetNombre.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetNombre.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetNombre.Location = new Point(260, 18);
            lblDetNombre.Name = "lblDetNombre";
            lblDetNombre.Size = new Size(110, 23);
            lblDetNombre.TabIndex = 2;
            lblDetNombre.Text = "Nombre";
            lblDetNombre.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtDetId
            // 
            txtDetId.FillColor = Color.WhiteSmoke;
            txtDetId.Font = new Font("Microsoft Sans Serif", 9F);
            txtDetId.Location = new Point(18, 41);
            txtDetId.Margin = new Padding(4, 5, 4, 5);
            txtDetId.MinimumSize = new Size(1, 16);
            txtDetId.Name = "txtDetId";
            txtDetId.Padding = new Padding(5);
            txtDetId.ReadOnly = true;
            txtDetId.RectColor = Color.FromArgb(220, 220, 220);
            txtDetId.ShowText = false;
            txtDetId.Size = new Size(220, 29);
            txtDetId.TabIndex = 1;
            txtDetId.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetId.Watermark = "";
            // 
            // lblDetId
            // 
            lblDetId.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblDetId.ForeColor = Color.FromArgb(80, 80, 80);
            lblDetId.Location = new Point(18, 18);
            lblDetId.Name = "lblDetId";
            lblDetId.Size = new Size(110, 23);
            lblDetId.TabIndex = 0;
            lblDetId.Text = "Product ID";
            lblDetId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tabNuevoProducto
            // 
            tabNuevoProducto.BackColor = Color.White;
            tabNuevoProducto.Controls.Add(pnlNuevo);
            tabNuevoProducto.Location = new Point(0, 32);
            tabNuevoProducto.Name = "tabNuevoProducto";
            tabNuevoProducto.Size = new Size(760, 563);
            tabNuevoProducto.TabIndex = 1;
            tabNuevoProducto.Text = "Producto Nuevo";
            // 
            // pnlNuevo
            // 
            pnlNuevo.AutoScroll = true;
            pnlNuevo.BackColor = Color.White;
            pnlNuevo.Controls.Add(btnGuardarNuevo);
            pnlNuevo.Controls.Add(txtNuevoRatio);
            pnlNuevo.Controls.Add(lblNuevoRatio);
            pnlNuevo.Controls.Add(txtNuevoPrecio);
            pnlNuevo.Controls.Add(lblNuevoPrecio);
            pnlNuevo.Controls.Add(cboNuevoCategoria);
            pnlNuevo.Controls.Add(lblNuevoCategoria);
            pnlNuevo.Controls.Add(txtNuevoCodebar);
            pnlNuevo.Controls.Add(lblNuevoCodebar);
            pnlNuevo.Controls.Add(txtNuevoRef);
            pnlNuevo.Controls.Add(lblNuevoRef);
            pnlNuevo.Controls.Add(txtNuevoDescrip);
            pnlNuevo.Controls.Add(lblNuevoDescrip);
            pnlNuevo.Controls.Add(txtNuevoNombre);
            pnlNuevo.Controls.Add(lblNuevoNombre);
            pnlNuevo.Controls.Add(txtNuevoId);
            pnlNuevo.Controls.Add(lblNuevoId);
            pnlNuevo.Dock = DockStyle.Fill;
            pnlNuevo.Location = new Point(0, 0);
            pnlNuevo.Name = "pnlNuevo";
            pnlNuevo.Padding = new Padding(15);
            pnlNuevo.Size = new Size(760, 563);
            pnlNuevo.TabIndex = 0;
            // 
            // btnGuardarNuevo
            // 
            btnGuardarNuevo.Cursor = Cursors.Hand;
            btnGuardarNuevo.FillColor = Color.FromArgb(110, 190, 40);
            btnGuardarNuevo.FillColor2 = Color.FromArgb(110, 190, 40);
            btnGuardarNuevo.FillHoverColor = Color.FromArgb(139, 203, 83);
            btnGuardarNuevo.FillPressColor = Color.FromArgb(88, 152, 32);
            btnGuardarNuevo.FillSelectedColor = Color.FromArgb(88, 152, 32);
            btnGuardarNuevo.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnGuardarNuevo.LightColor = Color.FromArgb(245, 251, 241);
            btnGuardarNuevo.Location = new Point(500, 320);
            btnGuardarNuevo.MinimumSize = new Size(1, 1);
            btnGuardarNuevo.Name = "btnGuardarNuevo";
            btnGuardarNuevo.RectColor = Color.FromArgb(110, 190, 40);
            btnGuardarNuevo.RectHoverColor = Color.FromArgb(139, 203, 83);
            btnGuardarNuevo.RectPressColor = Color.FromArgb(88, 152, 32);
            btnGuardarNuevo.RectSelectedColor = Color.FromArgb(88, 152, 32);
            btnGuardarNuevo.Size = new Size(230, 36);
            btnGuardarNuevo.Style = Sunny.UI.UIStyle.Custom;
            btnGuardarNuevo.TabIndex = 16;
            btnGuardarNuevo.Text = "Guardar";
            btnGuardarNuevo.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // txtNuevoRatio
            // 
            txtNuevoRatio.Font = new Font("Microsoft Sans Serif", 9F);
            txtNuevoRatio.Location = new Point(180, 273);
            txtNuevoRatio.Margin = new Padding(4, 5, 4, 5);
            txtNuevoRatio.MinimumSize = new Size(1, 16);
            txtNuevoRatio.Name = "txtNuevoRatio";
            txtNuevoRatio.Padding = new Padding(5);
            txtNuevoRatio.RectColor = Color.FromArgb(110, 190, 40);
            txtNuevoRatio.ShowText = false;
            txtNuevoRatio.Size = new Size(120, 29);
            txtNuevoRatio.TabIndex = 15;
            txtNuevoRatio.TextAlignment = ContentAlignment.MiddleLeft;
            txtNuevoRatio.Watermark = "1.0";
            // 
            // lblNuevoRatio
            // 
            lblNuevoRatio.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoRatio.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoRatio.Location = new Point(180, 250);
            lblNuevoRatio.Name = "lblNuevoRatio";
            lblNuevoRatio.Size = new Size(110, 23);
            lblNuevoRatio.TabIndex = 14;
            lblNuevoRatio.Text = "Ratio";
            lblNuevoRatio.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtNuevoPrecio
            // 
            txtNuevoPrecio.Font = new Font("Microsoft Sans Serif", 9F);
            txtNuevoPrecio.Location = new Point(18, 273);
            txtNuevoPrecio.Margin = new Padding(4, 5, 4, 5);
            txtNuevoPrecio.MinimumSize = new Size(1, 16);
            txtNuevoPrecio.Name = "txtNuevoPrecio";
            txtNuevoPrecio.Padding = new Padding(5);
            txtNuevoPrecio.RectColor = Color.FromArgb(110, 190, 40);
            txtNuevoPrecio.ShowText = false;
            txtNuevoPrecio.Size = new Size(140, 29);
            txtNuevoPrecio.TabIndex = 13;
            txtNuevoPrecio.TextAlignment = ContentAlignment.MiddleLeft;
            txtNuevoPrecio.Watermark = "0.00";
            // 
            // lblNuevoPrecio
            // 
            lblNuevoPrecio.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoPrecio.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoPrecio.Location = new Point(18, 250);
            lblNuevoPrecio.Name = "lblNuevoPrecio";
            lblNuevoPrecio.Size = new Size(110, 23);
            lblNuevoPrecio.TabIndex = 12;
            lblNuevoPrecio.Text = "Precio";
            lblNuevoPrecio.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // cboNuevoCategoria
            // 
            cboNuevoCategoria.DataSource = null;
            cboNuevoCategoria.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            cboNuevoCategoria.FillColor = Color.White;
            cboNuevoCategoria.Font = new Font("Microsoft Sans Serif", 9F);
            cboNuevoCategoria.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cboNuevoCategoria.Items.AddRange(new object[] { "Master", "Rollo Cortado", "Resma", "Graphics" });
            cboNuevoCategoria.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cboNuevoCategoria.Location = new Point(500, 205);
            cboNuevoCategoria.Margin = new Padding(4, 5, 4, 5);
            cboNuevoCategoria.MinimumSize = new Size(63, 0);
            cboNuevoCategoria.Name = "cboNuevoCategoria";
            cboNuevoCategoria.Padding = new Padding(0, 0, 30, 2);
            cboNuevoCategoria.RectColor = Color.FromArgb(110, 190, 40);
            cboNuevoCategoria.Size = new Size(230, 29);
            cboNuevoCategoria.SymbolSize = 24;
            cboNuevoCategoria.TabIndex = 11;
            cboNuevoCategoria.TextAlignment = ContentAlignment.MiddleLeft;
            cboNuevoCategoria.Watermark = "";
            // 
            // lblNuevoCategoria
            // 
            lblNuevoCategoria.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoCategoria.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoCategoria.Location = new Point(500, 182);
            lblNuevoCategoria.Name = "lblNuevoCategoria";
            lblNuevoCategoria.Size = new Size(110, 23);
            lblNuevoCategoria.TabIndex = 10;
            lblNuevoCategoria.Text = "Categoría *";
            lblNuevoCategoria.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtNuevoCodebar
            // 
            txtNuevoCodebar.Font = new Font("Microsoft Sans Serif", 9F);
            txtNuevoCodebar.Location = new Point(260, 205);
            txtNuevoCodebar.Margin = new Padding(4, 5, 4, 5);
            txtNuevoCodebar.MinimumSize = new Size(1, 16);
            txtNuevoCodebar.Name = "txtNuevoCodebar";
            txtNuevoCodebar.Padding = new Padding(5);
            txtNuevoCodebar.RectColor = Color.FromArgb(110, 190, 40);
            txtNuevoCodebar.ShowText = false;
            txtNuevoCodebar.Size = new Size(220, 29);
            txtNuevoCodebar.TabIndex = 9;
            txtNuevoCodebar.TextAlignment = ContentAlignment.MiddleLeft;
            txtNuevoCodebar.Watermark = "";
            // 
            // lblNuevoCodebar
            // 
            lblNuevoCodebar.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoCodebar.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoCodebar.Location = new Point(260, 182);
            lblNuevoCodebar.Name = "lblNuevoCodebar";
            lblNuevoCodebar.Size = new Size(110, 23);
            lblNuevoCodebar.TabIndex = 8;
            lblNuevoCodebar.Text = "Código Barra";
            lblNuevoCodebar.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtNuevoRef
            // 
            txtNuevoRef.Font = new Font("Microsoft Sans Serif", 9F);
            txtNuevoRef.Location = new Point(18, 205);
            txtNuevoRef.Margin = new Padding(4, 5, 4, 5);
            txtNuevoRef.MinimumSize = new Size(1, 16);
            txtNuevoRef.Name = "txtNuevoRef";
            txtNuevoRef.Padding = new Padding(5);
            txtNuevoRef.RectColor = Color.FromArgb(110, 190, 40);
            txtNuevoRef.ShowText = false;
            txtNuevoRef.Size = new Size(220, 29);
            txtNuevoRef.TabIndex = 7;
            txtNuevoRef.TextAlignment = ContentAlignment.MiddleLeft;
            txtNuevoRef.Watermark = "";
            // 
            // lblNuevoRef
            // 
            lblNuevoRef.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoRef.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoRef.Location = new Point(18, 182);
            lblNuevoRef.Name = "lblNuevoRef";
            lblNuevoRef.Size = new Size(110, 23);
            lblNuevoRef.TabIndex = 6;
            lblNuevoRef.Text = "Referencia";
            lblNuevoRef.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtNuevoDescrip
            // 
            txtNuevoDescrip.Font = new Font("Microsoft Sans Serif", 9F);
            txtNuevoDescrip.Location = new Point(18, 108);
            txtNuevoDescrip.Margin = new Padding(4, 5, 4, 5);
            txtNuevoDescrip.MinimumSize = new Size(1, 16);
            txtNuevoDescrip.Multiline = true;
            txtNuevoDescrip.Name = "txtNuevoDescrip";
            txtNuevoDescrip.Padding = new Padding(5);
            txtNuevoDescrip.RectColor = Color.FromArgb(110, 190, 40);
            txtNuevoDescrip.ShowText = false;
            txtNuevoDescrip.Size = new Size(712, 60);
            txtNuevoDescrip.TabIndex = 5;
            txtNuevoDescrip.TextAlignment = ContentAlignment.MiddleLeft;
            txtNuevoDescrip.Watermark = "";
            // 
            // lblNuevoDescrip
            // 
            lblNuevoDescrip.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoDescrip.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoDescrip.Location = new Point(18, 85);
            lblNuevoDescrip.Name = "lblNuevoDescrip";
            lblNuevoDescrip.Size = new Size(110, 23);
            lblNuevoDescrip.TabIndex = 4;
            lblNuevoDescrip.Text = "Descripción";
            lblNuevoDescrip.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtNuevoNombre
            // 
            txtNuevoNombre.Font = new Font("Microsoft Sans Serif", 9F);
            txtNuevoNombre.Location = new Point(260, 41);
            txtNuevoNombre.Margin = new Padding(4, 5, 4, 5);
            txtNuevoNombre.MinimumSize = new Size(1, 16);
            txtNuevoNombre.Name = "txtNuevoNombre";
            txtNuevoNombre.Padding = new Padding(5);
            txtNuevoNombre.RectColor = Color.FromArgb(110, 190, 40);
            txtNuevoNombre.ShowText = false;
            txtNuevoNombre.Size = new Size(470, 29);
            txtNuevoNombre.TabIndex = 3;
            txtNuevoNombre.TextAlignment = ContentAlignment.MiddleLeft;
            txtNuevoNombre.Watermark = "";
            // 
            // lblNuevoNombre
            // 
            lblNuevoNombre.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoNombre.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoNombre.Location = new Point(260, 18);
            lblNuevoNombre.Name = "lblNuevoNombre";
            lblNuevoNombre.Size = new Size(110, 23);
            lblNuevoNombre.TabIndex = 2;
            lblNuevoNombre.Text = "Nombre *";
            lblNuevoNombre.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtNuevoId
            // 
            txtNuevoId.Font = new Font("Microsoft Sans Serif", 9F);
            txtNuevoId.Location = new Point(18, 41);
            txtNuevoId.Margin = new Padding(4, 5, 4, 5);
            txtNuevoId.MinimumSize = new Size(1, 16);
            txtNuevoId.Name = "txtNuevoId";
            txtNuevoId.Padding = new Padding(5);
            txtNuevoId.RectColor = Color.FromArgb(110, 190, 40);
            txtNuevoId.ShowText = false;
            txtNuevoId.Size = new Size(220, 29);
            txtNuevoId.TabIndex = 1;
            txtNuevoId.TextAlignment = ContentAlignment.MiddleLeft;
            txtNuevoId.Watermark = "";
            // 
            // lblNuevoId
            // 
            lblNuevoId.Font = new Font("Microsoft Sans Serif", 8.5F);
            lblNuevoId.ForeColor = Color.FromArgb(80, 80, 80);
            lblNuevoId.Location = new Point(18, 18);
            lblNuevoId.Name = "lblNuevoId";
            lblNuevoId.Size = new Size(110, 23);
            lblNuevoId.TabIndex = 0;
            lblNuevoId.Text = "Product ID *";
            lblNuevoId.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // barraHerramientas
            // 
            barraHerramientas.AutoSize = false;
            barraHerramientas.BackColor = Color.FromArgb(110, 190, 40);
            barraHerramientas.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            barraHerramientas.ForeColor = Color.White;
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevo, btnEditar, btnImportar });
            barraHerramientas.Location = new Point(0, 35);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 0, 0, 0);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(1150, 68);
            barraHerramientas.TabIndex = 2;
            barraHerramientas.Text = "barraHerramientas";
            // 
            // btnNuevo
            // 
            btnNuevo.AutoSize = false;
            btnNuevo.BackColor = Color.Transparent;
            btnNuevo.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnNuevo.ForeColor = Color.White;
            btnNuevo.Image = Properties.Resources.add_file_32px;
            btnNuevo.ImageAlign = ContentAlignment.TopCenter;
            btnNuevo.ImageScaling = ToolStripItemImageScaling.None;
            btnNuevo.Margin = new Padding(4, 1, 0, 2);
            btnNuevo.Name = "btnNuevo";
            btnNuevo.Size = new Size(80, 64);
            btnNuevo.Text = "Nuevo";
            btnNuevo.TextImageRelation = TextImageRelation.ImageAboveText;
            btnNuevo.ToolTipText = "Nuevo producto";
            // 
            // btnEditar
            // 
            btnEditar.AutoSize = false;
            btnEditar.BackColor = Color.Transparent;
            btnEditar.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnEditar.ForeColor = Color.White;
            btnEditar.Image = Properties.Resources.edit_24px;
            btnEditar.ImageAlign = ContentAlignment.TopCenter;
            btnEditar.ImageScaling = ToolStripItemImageScaling.None;
            btnEditar.Margin = new Padding(4, 1, 0, 2);
            btnEditar.Name = "btnEditar";
            btnEditar.Size = new Size(80, 64);
            btnEditar.Text = "Editar";
            btnEditar.TextImageRelation = TextImageRelation.ImageAboveText;
            btnEditar.ToolTipText = "Editar producto seleccionado";
            // 
            // btnImportar
            // 
            btnImportar.AutoSize = false;
            btnImportar.BackColor = Color.Transparent;
            btnImportar.Font = new Font("Microsoft Sans Serif", 12F, FontStyle.Bold);
            btnImportar.ForeColor = Color.White;
            btnImportar.Image = Properties.Resources.DATA_DOWNLOAD48;
            btnImportar.ImageAlign = ContentAlignment.TopCenter;
            btnImportar.ImageScaling = ToolStripItemImageScaling.None;
            btnImportar.Margin = new Padding(4, 1, 0, 2);
            btnImportar.Name = "btnImportar";
            btnImportar.Size = new Size(90, 64);
            btnImportar.Text = "Importar";
            btnImportar.TextImageRelation = TextImageRelation.ImageAboveText;
            btnImportar.ToolTipText = "Importar productos desde Excel";
            // 
            // txt_Costo
            // 
            txt_Costo.FillColor = Color.WhiteSmoke;
            txt_Costo.Font = new Font("Microsoft Sans Serif", 9F);
            txt_Costo.Location = new Point(260, 273);
            txt_Costo.Margin = new Padding(4, 5, 4, 5);
            txt_Costo.MinimumSize = new Size(1, 16);
            txt_Costo.Name = "txt_Costo";
            txt_Costo.Padding = new Padding(5);
            txt_Costo.ReadOnly = true;
            txt_Costo.RectColor = Color.FromArgb(220, 220, 220);
            txt_Costo.ShowText = false;
            txt_Costo.Size = new Size(140, 29);
            txt_Costo.TabIndex = 15;
            txt_Costo.TextAlignment = ContentAlignment.MiddleLeft;
            txt_Costo.Watermark = "";
            // 
            // uiLabel1
            // 
            uiLabel1.Font = new Font("Microsoft Sans Serif", 8.5F);
            uiLabel1.ForeColor = Color.FromArgb(80, 80, 80);
            uiLabel1.Location = new Point(260, 250);
            uiLabel1.Name = "uiLabel1";
            uiLabel1.Size = new Size(110, 23);
            uiLabel1.TabIndex = 14;
            uiLabel1.Text = "Costo";
            uiLabel1.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // FrmProductos
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1150, 650);
            Controls.Add(panelDerecho);
            Controls.Add(panelIzq);
            Controls.Add(barraHerramientas);
            MinimumSize = new Size(1150, 650);
            Name = "FrmProductos";
            Text = "Productos";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 650);
            Load += FrmProductos_Load;
            panelIzq.ResumeLayout(false);
            pnlBuscador.ResumeLayout(false);
            pnlSummary.ResumeLayout(false);
            panelDerecho.ResumeLayout(false);
            tabDetalle.ResumeLayout(false);
            tabGeneral.ResumeLayout(false);
            pnlGeneral.ResumeLayout(false);
            grpDetCategoria.ResumeLayout(false);
            grpDetCategoria.PerformLayout();
            tabNuevoProducto.ResumeLayout(false);
            pnlNuevo.ResumeLayout(false);
            barraHerramientas.ResumeLayout(false);
            barraHerramientas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private Panel panelIzq;
        private Panel pnlBuscador;
        private Panel pnlSummary;
        private FlowLayoutPanel flpProductos;
        private Sunny.UI.UILabel lblTitulo;
        private Sunny.UI.UITextBox txtSearch;
        private Sunny.UI.UIComboBox cboFiltroCategoria;
        private Sunny.UI.UILabel lblTotal;
        private Panel panelDerecho;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabGeneral;
        private Panel pnlGeneral;
        private Sunny.UI.UILabel lblDetId;
        private Sunny.UI.UITextBox txtDetId;
        private Sunny.UI.UILabel lblDetNombre;
        private Sunny.UI.UITextBox txtDetNombre;
        private Sunny.UI.UILabel lblDetDescrip;
        private Sunny.UI.UITextBox txtDetDescrip;
        private Sunny.UI.UILabel lblDetRef;
        private Sunny.UI.UITextBox txtDetRef;
        private Sunny.UI.UILabel lblDetCodebar;
        private Sunny.UI.UITextBox txtDetCodebar;
        private Sunny.UI.UILabel lblDetCategoria;
        private Sunny.UI.UITextBox txtDetCategoria;
        private Sunny.UI.UILabel lblDetPrecio;
        private Sunny.UI.UITextBox txtDetPrecio;
        private Sunny.UI.UILabel lblDetRatio;
        private Sunny.UI.UITextBox txtDetRatio;
        private Sunny.UI.UICheckBox chkDetAnulado;
        private GroupBox grpDetCategoria;
        private RadioButton rdoDetMaster;
        private RadioButton rdoDetRollo;
        private RadioButton rdoDetGraphics;
        private RadioButton rdoDetHojas;
        private TabPage tabNuevoProducto;
        private Panel pnlNuevo;
        private Sunny.UI.UILabel lblNuevoId;
        private Sunny.UI.UILabel lblNuevoNombre;
        private Sunny.UI.UILabel lblNuevoDescrip;
        private Sunny.UI.UILabel lblNuevoRef;
        private Sunny.UI.UILabel lblNuevoCodebar;
        private Sunny.UI.UILabel lblNuevoCategoria;
        private Sunny.UI.UILabel lblNuevoPrecio;
        private Sunny.UI.UILabel lblNuevoRatio;
        private Sunny.UI.UITextBox txtNuevoId;
        private Sunny.UI.UITextBox txtNuevoNombre;
        private Sunny.UI.UITextBox txtNuevoDescrip;
        private Sunny.UI.UITextBox txtNuevoRef;
        private Sunny.UI.UITextBox txtNuevoCodebar;
        private Sunny.UI.UITextBox txtNuevoPrecio;
        private Sunny.UI.UITextBox txtNuevoRatio;
        private Sunny.UI.UIComboBox cboNuevoCategoria;
        private Sunny.UI.UIButton btnGuardarNuevo;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevo;
        private ToolStripButton btnEditar;
        private ToolStripButton btnImportar;
        private Sunny.UI.UITextBox txt_Costo;
        private Sunny.UI.UILabel uiLabel1;
    }
}
