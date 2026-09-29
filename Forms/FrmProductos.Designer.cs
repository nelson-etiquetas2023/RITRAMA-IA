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
            DataGridViewCellStyle estiloEncabezado = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloFilasPares = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloFilasImpares = new DataGridViewCellStyle();
            DataGridViewCellStyle estiloCuerpo = new DataGridViewCellStyle();
            tlpRoot = new TableLayoutPanel();
            panelIzq = new Panel();
            gridProductos = new Sunny.UI.UIDataGridView();
            colProductId = new DataGridViewTextBoxColumn();
            colProductName = new DataGridViewTextBoxColumn();
            colTipo = new DataGridViewTextBoxColumn();
            pnlBuscador = new Panel();
            lblTitulo = new Sunny.UI.UILabel();
            txtSearch = new Sunny.UI.UITextBox();
            pnlContador = new Panel();
            lblContador = new Sunny.UI.UILabel();
            lblContadorDetalle = new Sunny.UI.UILabel();
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
            txtDetTipo = new Sunny.UI.UITextBox();
            lblDetReferencia = new Sunny.UI.UILabel();
            txtDetReferencia = new Sunny.UI.UITextBox();
            lblDetCodebar = new Sunny.UI.UILabel();
            txtDetCodebar = new Sunny.UI.UITextBox();
            lblDetPrecio = new Sunny.UI.UILabel();
            txtDetPrecio = new Sunny.UI.UITextBox();
            lblDetRatio = new Sunny.UI.UILabel();
            txtDetRatio = new Sunny.UI.UITextBox();
            lblDetEstado = new Sunny.UI.UILabel();
            txtDetEstado = new Sunny.UI.UITextBox();
            lblDetDescripcion = new Sunny.UI.UILabel();
            txtDetDescripcion = new Sunny.UI.UITextBox();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridProductos).BeginInit();
            pnlBuscador.SuspendLayout();
            pnlContador.SuspendLayout();
            panelDer.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabDetalleProducto.SuspendLayout();
            tlpDetalle.SuspendLayout();
            SuspendLayout();
            // 
            // tlpRoot: divide el ancho en 30% (catalogo) y 70% (detalle)
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
            // panelIzq (30%): buscador arriba, grid en el medio, contador abajo
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(gridProductos);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Controls.Add(pnlContador);
            panelIzq.Dock = DockStyle.Fill;
            panelIzq.Location = new Point(1, 1);
            panelIzq.Margin = new Padding(0);
            panelIzq.MinimumSize = new Size(260, 0);
            panelIzq.Name = "panelIzq";
            panelIzq.Size = new Size(345, 648);
            panelIzq.TabIndex = 0;
            // 
            // gridProductos: product_id | product_name | tipo
            // 
            gridProductos.AllowUserToAddRows = false;
            gridProductos.AllowUserToDeleteRows = false;
            gridProductos.AllowUserToResizeRows = false;
            gridProductos.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridProductos.BackgroundColor = Color.White;
            gridProductos.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloEncabezado.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            estiloEncabezado.ForeColor = Color.White;
            estiloEncabezado.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.SelectionForeColor = Color.White;
            estiloEncabezado.WrapMode = DataGridViewTriState.True;
            gridProductos.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            gridProductos.ColumnHeadersHeight = 32;
            gridProductos.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridProductos.Columns.AddRange(new DataGridViewColumn[] { colProductId, colProductName, colTipo });
            estiloCuerpo.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloCuerpo.BackColor = Color.White;
            estiloCuerpo.Font = new Font("JetBrains Mono", 9F);
            estiloCuerpo.ForeColor = Color.FromArgb(48, 48, 48);
            estiloCuerpo.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloCuerpo.SelectionForeColor = Color.White;
            estiloCuerpo.WrapMode = DataGridViewTriState.False;
            gridProductos.DefaultCellStyle = estiloCuerpo;
            gridProductos.Dock = DockStyle.Fill;
            gridProductos.EnableHeadersVisualStyles = false;
            gridProductos.Font = new Font("JetBrains Mono", 9F);
            gridProductos.GridColor = Color.FromArgb(180, 210, 180);
            gridProductos.Location = new Point(1, 65);
            gridProductos.MultiSelect = false;
            gridProductos.Name = "gridProductos";
            gridProductos.ReadOnly = true;
            gridProductos.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloFilasPares.BackColor = Color.White;
            estiloFilasPares.Font = new Font("JetBrains Mono", 9F);
            gridProductos.RowsDefaultCellStyle = estiloFilasPares;
            estiloFilasImpares.BackColor = Color.FromArgb(245, 250, 240);
            estiloFilasImpares.Font = new Font("JetBrains Mono", 9F);
            gridProductos.AlternatingRowsDefaultCellStyle = estiloFilasImpares;
            gridProductos.RowHeadersVisible = false;
            gridProductos.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridProductos.Size = new Size(341, 520);
            gridProductos.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridProductos.TabIndex = 2;
            // 
            // colProductId
            // 
            colProductId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colProductId.DataPropertyName = "ProductId";
            colProductId.FillWeight = 28;
            colProductId.HeaderText = "Product ID";
            colProductId.Name = "colProductId";
            colProductId.ReadOnly = true;
            // 
            // colProductName
            // 
            colProductName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colProductName.DataPropertyName = "ProductName";
            colProductName.FillWeight = 52;
            colProductName.HeaderText = "Product Name";
            colProductName.Name = "colProductName";
            colProductName.ReadOnly = true;
            // 
            // colTipo
            // 
            colTipo.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colTipo.DataPropertyName = "Tipo";
            colTipo.FillWeight = 20;
            colTipo.HeaderText = "Tipo";
            colTipo.Name = "colTipo";
            colTipo.ReadOnly = true;
            // 
            // pnlBuscador
            // 
            pnlBuscador.BackColor = Color.White;
            pnlBuscador.Controls.Add(txtSearch);
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
            lblTitulo.Text = "CATALOGO DE PRODUCTOS";
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // txtSearch
            // 
            txtSearch.Dock = DockStyle.Bottom;
            txtSearch.FillColor = Color.White;
            txtSearch.Font = new Font("JetBrains Mono", 9F);
            txtSearch.Name = "txtSearch";
            txtSearch.Padding = new Padding(6, 0, 6, 0);
            txtSearch.RectColor = Color.FromArgb(110, 190, 40);
            txtSearch.Size = new Size(325, 29);
            txtSearch.TabIndex = 1;
            txtSearch.TextAlignment = ContentAlignment.MiddleLeft;
            txtSearch.Watermark = "Buscar por codigo o nombre...";
            // 
            // pnlContador (franja verde con el conteo de productos)
            // 
            pnlContador.BackColor = Color.FromArgb(110, 190, 40);
            pnlContador.Controls.Add(lblContadorDetalle);
            pnlContador.Controls.Add(lblContador);
            pnlContador.Dock = DockStyle.Bottom;
            pnlContador.Location = new Point(1, 585);
            pnlContador.Name = "pnlContador";
            pnlContador.Padding = new Padding(10, 4, 10, 4);
            pnlContador.Size = new Size(341, 60);
            pnlContador.TabIndex = 1;
            // 
            // lblContador
            // 
            lblContador.AutoSize = false;
            lblContador.Dock = DockStyle.Top;
            lblContador.Font = new Font("JetBrains Mono", 11F, FontStyle.Bold);
            lblContador.ForeColor = Color.White;
            lblContador.Name = "lblContador";
            lblContador.Size = new Size(321, 26);
            lblContador.TabIndex = 0;
            lblContador.Text = "Productos existentes: 0";
            lblContador.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblContadorDetalle
            // 
            lblContadorDetalle.AutoSize = false;
            lblContadorDetalle.Dock = DockStyle.Fill;
            lblContadorDetalle.Font = new Font("JetBrains Mono", 8F);
            lblContadorDetalle.ForeColor = Color.White;
            lblContadorDetalle.Name = "lblContadorDetalle";
            lblContadorDetalle.Size = new Size(321, 26);
            lblContadorDetalle.TabIndex = 1;
            lblContadorDetalle.Text = "Mostrando 0  -  Anulados 0  -  Total 0";
            lblContadorDetalle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelDer (70%): pestaas de detalle
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
            tabDetalle.Controls.Add(tabDetalleProducto);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabDetalle.Font = new Font("JetBrains Mono", 9F);
            tabDetalle.ItemSize = new Size(160, 32);
            tabDetalle.Location = new Point(8, 8);
            tabDetalle.MainPage = "";
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(787, 632);
            tabDetalle.SizeMode = TabSizeMode.Fixed;
            tabDetalle.TabIndex = 0;
            tabDetalle.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            tabDetalle.TipsFont = new Font("JetBrains Mono", 9F);
            // 
            // tabDetalleProducto
            // 
            tabDetalleProducto.BackColor = Color.White;
            tabDetalleProducto.Controls.Add(tlpDetalle);
            tabDetalleProducto.Location = new Point(0, 32);
            tabDetalleProducto.Name = "tabDetalleProducto";
            tabDetalleProducto.Size = new Size(787, 600);
            tabDetalleProducto.TabIndex = 0;
            tabDetalleProducto.Text = "Detalle";
            // 
            // tlpDetalle (etiqueta | valor)
            // 
            tlpDetalle.ColumnCount = 2;
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 160F));
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpDetalle.Controls.Add(lblDetalleTitulo, 0, 0);
            tlpDetalle.SetColumnSpan(lblDetalleTitulo, 2);
            tlpDetalle.Controls.Add(lblDetId, 0, 1);
            tlpDetalle.Controls.Add(txtDetId, 1, 1);
            tlpDetalle.Controls.Add(lblDetNombre, 0, 2);
            tlpDetalle.Controls.Add(txtDetNombre, 1, 2);
            tlpDetalle.Controls.Add(lblDetTipo, 0, 3);
            tlpDetalle.Controls.Add(txtDetTipo, 1, 3);
            tlpDetalle.Controls.Add(lblDetReferencia, 0, 4);
            tlpDetalle.Controls.Add(txtDetReferencia, 1, 4);
            tlpDetalle.Controls.Add(lblDetCodebar, 0, 5);
            tlpDetalle.Controls.Add(txtDetCodebar, 1, 5);
            tlpDetalle.Controls.Add(lblDetPrecio, 0, 6);
            tlpDetalle.Controls.Add(txtDetPrecio, 1, 6);
            tlpDetalle.Controls.Add(lblDetRatio, 0, 7);
            tlpDetalle.Controls.Add(txtDetRatio, 1, 7);
            tlpDetalle.Controls.Add(lblDetEstado, 0, 8);
            tlpDetalle.Controls.Add(txtDetEstado, 1, 8);
            tlpDetalle.Controls.Add(lblDetDescripcion, 0, 9);
            tlpDetalle.Controls.Add(txtDetDescripcion, 1, 9);
            tlpDetalle.Dock = DockStyle.Fill;
            tlpDetalle.Location = new Point(0, 0);
            tlpDetalle.Name = "tlpDetalle";
            tlpDetalle.Padding = new Padding(12, 10, 12, 10);
            tlpDetalle.RowCount = 11;
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 34F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpDetalle.Size = new Size(787, 600);
            tlpDetalle.TabIndex = 0;
            // 
            // lblDetalleTitulo
            // 
            lblDetalleTitulo.AutoSize = false;
            lblDetalleTitulo.Dock = DockStyle.Fill;
            lblDetalleTitulo.Name = "lblDetalleTitulo";
            lblDetalleTitulo.Size = new Size(763, 34);
            lblDetalleTitulo.TabIndex = 0;
            lblDetalleTitulo.Text = "DETALLE DEL PRODUCTO";
            lblDetalleTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblDetId / txtDetId
            // 
            lblDetId.AutoSize = false;
            lblDetId.Dock = DockStyle.Fill;
            lblDetId.Name = "lblDetId";
            lblDetId.Padding = new Padding(2, 0, 0, 0);
            lblDetId.TabIndex = 1;
            lblDetId.Text = "Codigo";
            lblDetId.TextAlign = ContentAlignment.MiddleLeft;
            txtDetId.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetId.Name = "txtDetId";
            txtDetId.Size = new Size(590, 29);
            txtDetId.TabIndex = 2;
            // 
            // lblDetNombre / txtDetNombre
            // 
            lblDetNombre.AutoSize = false;
            lblDetNombre.Dock = DockStyle.Fill;
            lblDetNombre.Name = "lblDetNombre";
            lblDetNombre.Padding = new Padding(2, 0, 0, 0);
            lblDetNombre.TabIndex = 3;
            lblDetNombre.Text = "Nombre";
            lblDetNombre.TextAlign = ContentAlignment.MiddleLeft;
            txtDetNombre.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetNombre.Name = "txtDetNombre";
            txtDetNombre.Size = new Size(590, 29);
            txtDetNombre.TabIndex = 4;
            // 
            // lblDetTipo / txtDetTipo
            // 
            lblDetTipo.AutoSize = false;
            lblDetTipo.Dock = DockStyle.Fill;
            lblDetTipo.Name = "lblDetTipo";
            lblDetTipo.Padding = new Padding(2, 0, 0, 0);
            lblDetTipo.TabIndex = 5;
            lblDetTipo.Text = "Tipo";
            lblDetTipo.TextAlign = ContentAlignment.MiddleLeft;
            txtDetTipo.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetTipo.Name = "txtDetTipo";
            txtDetTipo.Size = new Size(590, 29);
            txtDetTipo.TabIndex = 6;
            // 
            // lblDetReferencia / txtDetReferencia
            // 
            lblDetReferencia.AutoSize = false;
            lblDetReferencia.Dock = DockStyle.Fill;
            lblDetReferencia.Name = "lblDetReferencia";
            lblDetReferencia.Padding = new Padding(2, 0, 0, 0);
            lblDetReferencia.TabIndex = 7;
            lblDetReferencia.Text = "Referencia";
            lblDetReferencia.TextAlign = ContentAlignment.MiddleLeft;
            txtDetReferencia.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetReferencia.Name = "txtDetReferencia";
            txtDetReferencia.Size = new Size(590, 29);
            txtDetReferencia.TabIndex = 8;
            // 
            // lblDetCodebar / txtDetCodebar
            // 
            lblDetCodebar.AutoSize = false;
            lblDetCodebar.Dock = DockStyle.Fill;
            lblDetCodebar.Name = "lblDetCodebar";
            lblDetCodebar.Padding = new Padding(2, 0, 0, 0);
            lblDetCodebar.TabIndex = 9;
            lblDetCodebar.Text = "Codigo de barra";
            lblDetCodebar.TextAlign = ContentAlignment.MiddleLeft;
            txtDetCodebar.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetCodebar.Name = "txtDetCodebar";
            txtDetCodebar.Size = new Size(590, 29);
            txtDetCodebar.TabIndex = 10;
            // 
            // lblDetPrecio / txtDetPrecio
            // 
            lblDetPrecio.AutoSize = false;
            lblDetPrecio.Dock = DockStyle.Fill;
            lblDetPrecio.Name = "lblDetPrecio";
            lblDetPrecio.Padding = new Padding(2, 0, 0, 0);
            lblDetPrecio.TabIndex = 11;
            lblDetPrecio.Text = "Precio";
            lblDetPrecio.TextAlign = ContentAlignment.MiddleLeft;
            txtDetPrecio.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetPrecio.Name = "txtDetPrecio";
            txtDetPrecio.Size = new Size(590, 29);
            txtDetPrecio.TabIndex = 12;
            // 
            // lblDetRatio / txtDetRatio
            // 
            lblDetRatio.AutoSize = false;
            lblDetRatio.Dock = DockStyle.Fill;
            lblDetRatio.Name = "lblDetRatio";
            lblDetRatio.Padding = new Padding(2, 0, 0, 0);
            lblDetRatio.TabIndex = 13;
            lblDetRatio.Text = "Ratio";
            lblDetRatio.TextAlign = ContentAlignment.MiddleLeft;
            txtDetRatio.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetRatio.Name = "txtDetRatio";
            txtDetRatio.Size = new Size(590, 29);
            txtDetRatio.TabIndex = 14;
            // 
            // lblDetEstado / txtDetEstado
            // 
            lblDetEstado.AutoSize = false;
            lblDetEstado.Dock = DockStyle.Fill;
            lblDetEstado.Name = "lblDetEstado";
            lblDetEstado.Padding = new Padding(2, 0, 0, 0);
            lblDetEstado.TabIndex = 15;
            lblDetEstado.Text = "Estado";
            lblDetEstado.TextAlign = ContentAlignment.MiddleLeft;
            txtDetEstado.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetEstado.Name = "txtDetEstado";
            txtDetEstado.Size = new Size(590, 29);
            txtDetEstado.TabIndex = 16;
            // 
            // lblDetDescripcion / txtDetDescripcion
            // 
            lblDetDescripcion.AutoSize = false;
            lblDetDescripcion.Dock = DockStyle.Fill;
            lblDetDescripcion.Name = "lblDetDescripcion";
            lblDetDescripcion.Padding = new Padding(2, 0, 0, 0);
            lblDetDescripcion.TabIndex = 17;
            lblDetDescripcion.Text = "Descripcion";
            lblDetDescripcion.TextAlign = ContentAlignment.MiddleLeft;
            txtDetDescripcion.Anchor = AnchorStyles.Top | AnchorStyles.Left | AnchorStyles.Right;
            txtDetDescripcion.Name = "txtDetDescripcion";
            txtDetDescripcion.Size = new Size(590, 29);
            txtDetDescripcion.TabIndex = 18;
            // 
            // FrmProductos
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1150, 650);
            Controls.Add(tlpRoot);
            MinimumSize = new Size(980, 560);
            Name = "FrmProductos";
            Text = "Productos";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 650);
            tlpRoot.ResumeLayout(false);
            panelIzq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridProductos).EndInit();
            pnlBuscador.ResumeLayout(false);
            pnlContador.ResumeLayout(false);
            panelDer.ResumeLayout(false);
            tabDetalle.ResumeLayout(false);
            tabDetalleProducto.ResumeLayout(false);
            tlpDetalle.ResumeLayout(false);
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
        private Sunny.UI.UITextBox txtDetTipo;
        private Sunny.UI.UILabel lblDetReferencia;
        private Sunny.UI.UITextBox txtDetReferencia;
        private Sunny.UI.UILabel lblDetCodebar;
        private Sunny.UI.UITextBox txtDetCodebar;
        private Sunny.UI.UILabel lblDetPrecio;
        private Sunny.UI.UITextBox txtDetPrecio;
        private Sunny.UI.UILabel lblDetRatio;
        private Sunny.UI.UITextBox txtDetRatio;
        private Sunny.UI.UILabel lblDetEstado;
        private Sunny.UI.UITextBox txtDetEstado;
        private Sunny.UI.UILabel lblDetDescripcion;
        private Sunny.UI.UITextBox txtDetDescripcion;
    }
}
