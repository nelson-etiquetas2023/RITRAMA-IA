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
            txtValorEstado = new Sunny.UI.UITextBox();
            panelDer = new Panel();
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
            // pnlResumen (bajo el buscador: total de vendedores y cuántos muestra el filtro)
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
            lblResumen.Text = "Total: 0 vendedores";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelDer (70%): página de detalle del vendedor seleccionado
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
            tabDetalle.Controls.Add(tabDetalleVendedor);
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
            // tabDetalleVendedor
            // 
            tabDetalleVendedor.BackColor = Color.White;
            tabDetalleVendedor.Controls.Add(tlpDetalle);
            tabDetalleVendedor.Controls.Add(lblDetalleTitulo);
            tabDetalleVendedor.Location = new Point(0, 32);
            tabDetalleVendedor.Name = "tabDetalleVendedor";
            tabDetalleVendedor.Size = new Size(787, 600);
            tabDetalleVendedor.TabIndex = 0;
            tabDetalleVendedor.Text = "Detalle";
            // 
            // lblDetalleTitulo
            // 
            lblDetalleTitulo.AutoSize = false;
            lblDetalleTitulo.Dock = DockStyle.Top;
            lblDetalleTitulo.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            lblDetalleTitulo.ForeColor = Color.FromArgb(60, 110, 20);
            lblDetalleTitulo.Name = "lblDetalleTitulo";
            lblDetalleTitulo.Padding = new Padding(8, 0, 0, 0);
            lblDetalleTitulo.Size = new Size(787, 26);
            lblDetalleTitulo.TabIndex = 0;
            lblDetalleTitulo.Text = "DETALLE DEL VENDEDOR";
            lblDetalleTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tlpDetalle (campos de vendedor: caption | valor, 5 filas)
            // 
            tlpDetalle.ColumnCount = 2;
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tlpDetalle.Dock = DockStyle.Fill;
            tlpDetalle.Name = "tlpDetalle";
            tlpDetalle.Padding = new Padding(12, 8, 12, 8);
            tlpDetalle.RowCount = 5;
            tlpDetalle.TabIndex = 1;
            // 
            // lblCapId / txtValorId
            // 
            lblCapId.Dock = DockStyle.Fill;
            lblCapId.Font = fuenteCaption;
            lblCapId.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapId.Name = "lblCapId";
            lblCapId.Text = "Código:";
            lblCapId.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapId, 0, 0);
            txtValorId.Dock = DockStyle.Fill;
            txtValorId.FillColor = Color.White;
            txtValorId.Font = fuenteValor;
            txtValorId.ReadOnly = true;

            txtValorId.Name = "txtValorId";
            txtValorId.Padding = new Padding(6, 0, 6, 0);
            txtValorId.RectColor = Color.FromArgb(110, 190, 40);
            txtValorId.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorId.Text = "—";
            tlpDetalle.Controls.Add(txtValorId, 1, 0);
            // 
            // lblCapNombre / txtValorNombre
            // 
            lblCapNombre.Dock = DockStyle.Fill;
            lblCapNombre.Font = fuenteCaption;
            lblCapNombre.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapNombre.Name = "lblCapNombre";
            lblCapNombre.Text = "Nombre:";
            lblCapNombre.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapNombre, 0, 1);
            txtValorNombre.Dock = DockStyle.Fill;
            txtValorNombre.FillColor = Color.White;
            txtValorNombre.Font = fuenteValor;

            txtValorNombre.Name = "txtValorNombre";
            txtValorNombre.Padding = new Padding(6, 0, 6, 0);
            txtValorNombre.RectColor = Color.FromArgb(110, 190, 40);
            txtValorNombre.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorNombre.Text = "—";
            tlpDetalle.Controls.Add(txtValorNombre, 1, 1);
            // 
            // lblCapCorreo / txtValorCorreo
            // 
            lblCapCorreo.Dock = DockStyle.Fill;
            lblCapCorreo.Font = fuenteCaption;
            lblCapCorreo.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapCorreo.Name = "lblCapCorreo";
            lblCapCorreo.Text = "Correo:";
            lblCapCorreo.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapCorreo, 0, 2);
            txtValorCorreo.Dock = DockStyle.Fill;
            txtValorCorreo.FillColor = Color.White;
            txtValorCorreo.Font = fuenteValor;

            txtValorCorreo.Name = "txtValorCorreo";
            txtValorCorreo.Padding = new Padding(6, 0, 6, 0);
            txtValorCorreo.RectColor = Color.FromArgb(110, 190, 40);
            txtValorCorreo.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorCorreo.Text = "—";
            tlpDetalle.Controls.Add(txtValorCorreo, 1, 2);
            // 
            // lblCapTelefono / txtValorTelefono
            // 
            lblCapTelefono.Dock = DockStyle.Fill;
            lblCapTelefono.Font = fuenteCaption;
            lblCapTelefono.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapTelefono.Name = "lblCapTelefono";
            lblCapTelefono.Text = "Teléfono:";
            lblCapTelefono.TextAlign = ContentAlignment.MiddleRight;
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
            // lblCapEstado / txtValorEstado
            // 
            lblCapEstado.Dock = DockStyle.Fill;
            lblCapEstado.Font = fuenteCaption;
            lblCapEstado.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapEstado.Name = "lblCapEstado";
            lblCapEstado.Text = "Estado:";
            lblCapEstado.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapEstado, 0, 4);
            txtValorEstado.Dock = DockStyle.Fill;
            txtValorEstado.FillColor = Color.White;
            txtValorEstado.Font = fuenteValor;
            txtValorEstado.ReadOnly = true;

            txtValorEstado.Name = "txtValorEstado";
            txtValorEstado.Padding = new Padding(6, 0, 6, 0);
            txtValorEstado.RectColor = Color.FromArgb(110, 190, 40);
            txtValorEstado.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorEstado.Text = "—";
            tlpDetalle.Controls.Add(txtValorEstado, 1, 4);
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
        private Sunny.UI.UITextBox txtValorEstado;
        private Panel panelDer;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabDetalleVendedor;
    }
}
