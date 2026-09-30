namespace Ritrama2025.Forms
{
    partial class FrmClientes
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
        /// InitializeComponent del rediseño 30/70 del módulo de Clientes.
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
            gridClientes = new Sunny.UI.UIDataGridView();
            colCustomerId = new DataGridViewTextBoxColumn();
            colCustomerName = new DataGridViewTextBoxColumn();
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
            lblCapIdentificacion = new Sunny.UI.UILabel();
            txtValorIdentificacion = new Sunny.UI.UITextBox();
            lblCapEmpresa = new Sunny.UI.UILabel();
            txtValorEmpresa = new Sunny.UI.UITextBox();
            lblCapCategoria = new Sunny.UI.UILabel();
            txtValorCategoria = new Sunny.UI.UITextBox();
            lblCapTelefono = new Sunny.UI.UILabel();
            txtValorTelefono = new Sunny.UI.UITextBox();
            lblCapContacto = new Sunny.UI.UILabel();
            txtValorContacto = new Sunny.UI.UITextBox();
            lblCapEmail = new Sunny.UI.UILabel();
            txtValorEmail = new Sunny.UI.UITextBox();
            lblCapCondicion = new Sunny.UI.UILabel();
            txtValorCondicion = new Sunny.UI.UITextBox();
            lblCapImpuesto = new Sunny.UI.UILabel();
            txtValorImpuesto = new Sunny.UI.UITextBox();
            lblCapDireccion = new Sunny.UI.UILabel();
            txtValorDireccion = new Sunny.UI.UITextBox();
            lblCapUnity1 = new Sunny.UI.UILabel();
            txtValorUnity1 = new Sunny.UI.UITextBox();
            lblCapUnity2 = new Sunny.UI.UILabel();
            txtValorUnity2 = new Sunny.UI.UITextBox();
            lblCapEstado = new Sunny.UI.UILabel();
            txtValorEstado = new Sunny.UI.UITextBox();
            panelDer = new Panel();
            tabDetalle = new Sunny.UI.UITabControl();
            tabDetalleCliente = new TabPage();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridClientes).BeginInit();
            pnlBuscador.SuspendLayout();
            pnlResumen.SuspendLayout();
            tlpDetalle.SuspendLayout();
            panelDer.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabDetalleCliente.SuspendLayout();
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
            // panelIzq (30%): buscador arriba, cuadro de resumen debajo, grid de clientes
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(gridClientes);
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
            // gridClientes: customer_id | customer_name | status (activo/desactivado)
            // 
            gridClientes.AllowUserToAddRows = false;
            gridClientes.AllowUserToDeleteRows = false;
            gridClientes.AllowUserToResizeRows = false;
            gridClientes.AutoGenerateColumns = false;
            gridClientes.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridClientes.BackgroundColor = Color.White;
            gridClientes.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloEncabezado.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloEncabezado.BackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.Font = new Font("JetBrains Mono", 9F, FontStyle.Bold);
            estiloEncabezado.ForeColor = Color.White;
            estiloEncabezado.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloEncabezado.SelectionForeColor = Color.White;
            estiloEncabezado.WrapMode = DataGridViewTriState.True;
            gridClientes.ColumnHeadersDefaultCellStyle = estiloEncabezado;
            gridClientes.ColumnHeadersHeight = 32;
            gridClientes.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridClientes.Columns.AddRange(new DataGridViewColumn[] { colCustomerId, colCustomerName, colStatus });
            estiloCuerpo.Alignment = DataGridViewContentAlignment.MiddleLeft;
            estiloCuerpo.BackColor = Color.White;
            estiloCuerpo.Font = new Font("JetBrains Mono", 9F);
            estiloCuerpo.ForeColor = Color.FromArgb(48, 48, 48);
            estiloCuerpo.SelectionBackColor = Color.FromArgb(110, 190, 40);
            estiloCuerpo.SelectionForeColor = Color.White;
            estiloCuerpo.WrapMode = DataGridViewTriState.False;
            gridClientes.DefaultCellStyle = estiloCuerpo;
            gridClientes.Dock = DockStyle.Fill;
            gridClientes.EnableHeadersVisualStyles = false;
            gridClientes.Font = new Font("JetBrains Mono", 9F);
            gridClientes.GridColor = Color.FromArgb(180, 210, 180);
            gridClientes.Location = new Point(1, 93);
            gridClientes.MultiSelect = false;
            gridClientes.Name = "gridClientes";
            gridClientes.ReadOnly = true;
            gridClientes.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            estiloFilasPares.BackColor = Color.White;
            estiloFilasPares.Font = new Font("JetBrains Mono", 9F);
            gridClientes.RowsDefaultCellStyle = estiloFilasPares;
            estiloFilasImpares.BackColor = Color.FromArgb(245, 250, 240);
            estiloFilasImpares.Font = new Font("JetBrains Mono", 9F);
            gridClientes.AlternatingRowsDefaultCellStyle = estiloFilasImpares;
            gridClientes.RowHeadersVisible = false;
            gridClientes.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridClientes.Size = new Size(341, 553);
            gridClientes.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridClientes.TabIndex = 1;
            // 
            // colCustomerId
            // 
            colCustomerId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colCustomerId.DataPropertyName = "customer_id";
            colCustomerId.FillWeight = 30F;
            colCustomerId.HeaderText = "customer_id";
            colCustomerId.Name = "colCustomerId";
            colCustomerId.ReadOnly = true;
            // 
            // colCustomerName
            // 
            colCustomerName.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colCustomerName.DataPropertyName = "customer_name";
            colCustomerName.FillWeight = 45F;
            colCustomerName.HeaderText = "customer_name";
            colCustomerName.Name = "colCustomerName";
            colCustomerName.ReadOnly = true;
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
            lblTitulo.Text = "CATALOGO DE CLIENTES";
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
            // pnlResumen (bajo el buscador: total de clientes y cuántos muestra el filtro)
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
            lblResumen.Text = "Total: 0 clientes";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // panelDer (70%): página de detalle del cliente seleccionado
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
            tabDetalle.Controls.Add(tabDetalleCliente);
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
            // tabDetalleCliente
            // 
            tabDetalleCliente.BackColor = Color.White;
            tabDetalleCliente.Controls.Add(tlpDetalle);
            tabDetalleCliente.Controls.Add(lblDetalleTitulo);
            tabDetalleCliente.Location = new Point(0, 32);
            tabDetalleCliente.Name = "tabDetalleCliente";
            tabDetalleCliente.Size = new Size(787, 600);
            tabDetalleCliente.TabIndex = 0;
            tabDetalleCliente.Text = "Detalle";
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
            lblDetalleTitulo.Text = "DETALLE DEL CLIENTE";
            lblDetalleTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // tlpDetalle (campos de customer: caption | valor, 14 filas)
            // 
            tlpDetalle.ColumnCount = 2;
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
            tlpDetalle.Dock = DockStyle.Fill;
            tlpDetalle.Name = "tlpDetalle";
            tlpDetalle.Padding = new Padding(12, 8, 12, 8);
            tlpDetalle.RowCount = 14;
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
            // lblCapIdentificacion / txtValorIdentificacion
            // 
            lblCapIdentificacion.Dock = DockStyle.Fill;
            lblCapIdentificacion.Font = fuenteCaption;
            lblCapIdentificacion.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapIdentificacion.Name = "lblCapIdentificacion";
            lblCapIdentificacion.Text = "Identificación:";
            lblCapIdentificacion.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapIdentificacion, 0, 2);
            txtValorIdentificacion.Dock = DockStyle.Fill;
            txtValorIdentificacion.FillColor = Color.White;
            txtValorIdentificacion.Font = fuenteValor;

            txtValorIdentificacion.Name = "txtValorIdentificacion";
            txtValorIdentificacion.Padding = new Padding(6, 0, 6, 0);
            txtValorIdentificacion.RectColor = Color.FromArgb(110, 190, 40);
            txtValorIdentificacion.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorIdentificacion.Text = "—";
            tlpDetalle.Controls.Add(txtValorIdentificacion, 1, 2);
            // 
            // lblCapEmpresa / txtValorEmpresa
            // 
            lblCapEmpresa.Dock = DockStyle.Fill;
            lblCapEmpresa.Font = fuenteCaption;
            lblCapEmpresa.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapEmpresa.Name = "lblCapEmpresa";
            lblCapEmpresa.Text = "Empresa:";
            lblCapEmpresa.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapEmpresa, 0, 3);
            txtValorEmpresa.Dock = DockStyle.Fill;
            txtValorEmpresa.FillColor = Color.White;
            txtValorEmpresa.Font = fuenteValor;

            txtValorEmpresa.Name = "txtValorEmpresa";
            txtValorEmpresa.Padding = new Padding(6, 0, 6, 0);
            txtValorEmpresa.RectColor = Color.FromArgb(110, 190, 40);
            txtValorEmpresa.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorEmpresa.Text = "—";
            tlpDetalle.Controls.Add(txtValorEmpresa, 1, 3);
            // 
            // lblCapCategoria / txtValorCategoria
            // 
            lblCapCategoria.Dock = DockStyle.Fill;
            lblCapCategoria.Font = fuenteCaption;
            lblCapCategoria.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapCategoria.Name = "lblCapCategoria";
            lblCapCategoria.Text = "Categoría:";
            lblCapCategoria.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapCategoria, 0, 4);
            txtValorCategoria.Dock = DockStyle.Fill;
            txtValorCategoria.FillColor = Color.White;
            txtValorCategoria.Font = fuenteValor;

            txtValorCategoria.Name = "txtValorCategoria";
            txtValorCategoria.Padding = new Padding(6, 0, 6, 0);
            txtValorCategoria.RectColor = Color.FromArgb(110, 190, 40);
            txtValorCategoria.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorCategoria.Text = "—";
            tlpDetalle.Controls.Add(txtValorCategoria, 1, 4);
            // 
            // lblCapTelefono / txtValorTelefono
            // 
            lblCapTelefono.Dock = DockStyle.Fill;
            lblCapTelefono.Font = fuenteCaption;
            lblCapTelefono.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapTelefono.Name = "lblCapTelefono";
            lblCapTelefono.Text = "Teléfono:";
            lblCapTelefono.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapTelefono, 0, 5);
            txtValorTelefono.Dock = DockStyle.Fill;
            txtValorTelefono.FillColor = Color.White;
            txtValorTelefono.Font = fuenteValor;

            txtValorTelefono.Name = "txtValorTelefono";
            txtValorTelefono.Padding = new Padding(6, 0, 6, 0);
            txtValorTelefono.RectColor = Color.FromArgb(110, 190, 40);
            txtValorTelefono.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorTelefono.Text = "—";
            tlpDetalle.Controls.Add(txtValorTelefono, 1, 5);
            // 
            // lblCapContacto / txtValorContacto
            // 
            lblCapContacto.Dock = DockStyle.Fill;
            lblCapContacto.Font = fuenteCaption;
            lblCapContacto.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapContacto.Name = "lblCapContacto";
            lblCapContacto.Text = "Contacto:";
            lblCapContacto.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapContacto, 0, 6);
            txtValorContacto.Dock = DockStyle.Fill;
            txtValorContacto.FillColor = Color.White;
            txtValorContacto.Font = fuenteValor;

            txtValorContacto.Name = "txtValorContacto";
            txtValorContacto.Padding = new Padding(6, 0, 6, 0);
            txtValorContacto.RectColor = Color.FromArgb(110, 190, 40);
            txtValorContacto.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorContacto.Text = "—";
            tlpDetalle.Controls.Add(txtValorContacto, 1, 6);
            // 
            // lblCapEmail / txtValorEmail
            // 
            lblCapEmail.Dock = DockStyle.Fill;
            lblCapEmail.Font = fuenteCaption;
            lblCapEmail.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapEmail.Name = "lblCapEmail";
            lblCapEmail.Text = "Email:";
            lblCapEmail.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapEmail, 0, 7);
            txtValorEmail.Dock = DockStyle.Fill;
            txtValorEmail.FillColor = Color.White;
            txtValorEmail.Font = fuenteValor;

            txtValorEmail.Name = "txtValorEmail";
            txtValorEmail.Padding = new Padding(6, 0, 6, 0);
            txtValorEmail.RectColor = Color.FromArgb(110, 190, 40);
            txtValorEmail.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorEmail.Text = "—";
            tlpDetalle.Controls.Add(txtValorEmail, 1, 7);
            // 
            // lblCapCondicion / txtValorCondicion
            // 
            lblCapCondicion.Dock = DockStyle.Fill;
            lblCapCondicion.Font = fuenteCaption;
            lblCapCondicion.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapCondicion.Name = "lblCapCondicion";
            lblCapCondicion.Text = "Condición pago:";
            lblCapCondicion.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapCondicion, 0, 8);
            txtValorCondicion.Dock = DockStyle.Fill;
            txtValorCondicion.FillColor = Color.White;
            txtValorCondicion.Font = fuenteValor;

            txtValorCondicion.Name = "txtValorCondicion";
            txtValorCondicion.Padding = new Padding(6, 0, 6, 0);
            txtValorCondicion.RectColor = Color.FromArgb(110, 190, 40);
            txtValorCondicion.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorCondicion.Text = "—";
            tlpDetalle.Controls.Add(txtValorCondicion, 1, 8);
            // 
            // lblCapImpuesto / txtValorImpuesto
            // 
            lblCapImpuesto.Dock = DockStyle.Fill;
            lblCapImpuesto.Font = fuenteCaption;
            lblCapImpuesto.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapImpuesto.Name = "lblCapImpuesto";
            lblCapImpuesto.Text = "Impuesto:";
            lblCapImpuesto.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapImpuesto, 0, 9);
            txtValorImpuesto.Dock = DockStyle.Fill;
            txtValorImpuesto.FillColor = Color.White;
            txtValorImpuesto.Font = fuenteValor;

            txtValorImpuesto.Name = "txtValorImpuesto";
            txtValorImpuesto.Padding = new Padding(6, 0, 6, 0);
            txtValorImpuesto.RectColor = Color.FromArgb(110, 190, 40);
            txtValorImpuesto.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorImpuesto.Text = "—";
            tlpDetalle.Controls.Add(txtValorImpuesto, 1, 9);
            // 
            // lblCapDireccion / txtValorDireccion
            // 
            lblCapDireccion.Dock = DockStyle.Fill;
            lblCapDireccion.Font = fuenteCaption;
            lblCapDireccion.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapDireccion.Name = "lblCapDireccion";
            lblCapDireccion.Text = "Dirección:";
            lblCapDireccion.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapDireccion, 0, 10);
            txtValorDireccion.Dock = DockStyle.Fill;
            txtValorDireccion.FillColor = Color.White;
            txtValorDireccion.Font = fuenteValor;

            txtValorDireccion.Name = "txtValorDireccion";
            txtValorDireccion.Padding = new Padding(6, 0, 6, 0);
            txtValorDireccion.RectColor = Color.FromArgb(110, 190, 40);
            txtValorDireccion.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorDireccion.Text = "—";
            tlpDetalle.Controls.Add(txtValorDireccion, 1, 10);
            // 
            // lblCapUnity1 / txtValorUnity1
            // 
            lblCapUnity1.Dock = DockStyle.Fill;
            lblCapUnity1.Font = fuenteCaption;
            lblCapUnity1.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapUnity1.Name = "lblCapUnity1";
            lblCapUnity1.Text = "Unity 1:";
            lblCapUnity1.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapUnity1, 0, 11);
            txtValorUnity1.Dock = DockStyle.Fill;
            txtValorUnity1.FillColor = Color.White;
            txtValorUnity1.Font = fuenteValor;

            txtValorUnity1.Name = "txtValorUnity1";
            txtValorUnity1.Padding = new Padding(6, 0, 6, 0);
            txtValorUnity1.RectColor = Color.FromArgb(110, 190, 40);
            txtValorUnity1.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorUnity1.Text = "—";
            tlpDetalle.Controls.Add(txtValorUnity1, 1, 11);
            // 
            // lblCapUnity2 / txtValorUnity2
            // 
            lblCapUnity2.Dock = DockStyle.Fill;
            lblCapUnity2.Font = fuenteCaption;
            lblCapUnity2.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapUnity2.Name = "lblCapUnity2";
            lblCapUnity2.Text = "Unity 2:";
            lblCapUnity2.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapUnity2, 0, 12);
            txtValorUnity2.Dock = DockStyle.Fill;
            txtValorUnity2.FillColor = Color.White;
            txtValorUnity2.Font = fuenteValor;

            txtValorUnity2.Name = "txtValorUnity2";
            txtValorUnity2.Padding = new Padding(6, 0, 6, 0);
            txtValorUnity2.RectColor = Color.FromArgb(110, 190, 40);
            txtValorUnity2.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorUnity2.Text = "—";
            tlpDetalle.Controls.Add(txtValorUnity2, 1, 12);
            // 
            // lblCapEstado / txtValorEstado
            // 
            lblCapEstado.Dock = DockStyle.Fill;
            lblCapEstado.Font = fuenteCaption;
            lblCapEstado.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapEstado.Name = "lblCapEstado";
            lblCapEstado.Text = "Estado:";
            lblCapEstado.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapEstado, 0, 13);
            txtValorEstado.Dock = DockStyle.Fill;
            txtValorEstado.FillColor = Color.White;
            txtValorEstado.Font = fuenteValor;
            txtValorEstado.ReadOnly = true;

            txtValorEstado.Name = "txtValorEstado";
            txtValorEstado.Padding = new Padding(6, 0, 6, 0);
            txtValorEstado.RectColor = Color.FromArgb(110, 190, 40);
            txtValorEstado.TextAlignment = ContentAlignment.MiddleLeft;
            txtValorEstado.Text = "—";
            tlpDetalle.Controls.Add(txtValorEstado, 1, 13);
            // 
            // FrmClientes
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.White;
            ClientSize = new Size(1150, 650);
            Controls.Add(tlpRoot);
            MinimumSize = new Size(980, 560);
            Name = "FrmClientes";
            Text = "Clientes";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 650);
            tlpRoot.ResumeLayout(false);
            panelIzq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridClientes).EndInit();
            pnlBuscador.ResumeLayout(false);
            pnlResumen.ResumeLayout(false);
            tlpDetalle.ResumeLayout(false);
            panelDer.ResumeLayout(false);
            tabDetalle.ResumeLayout(false);
            tabDetalleCliente.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpRoot;
        private Panel panelIzq;
        private Sunny.UI.UIDataGridView gridClientes;
        private DataGridViewTextBoxColumn colCustomerId;
        private DataGridViewTextBoxColumn colCustomerName;
        private DataGridViewTextBoxColumn colStatus;
        private Panel pnlBuscador;
        private Sunny.UI.UILabel lblTitulo;
        private Sunny.UI.UITextBox txtBuscar;
        private Panel panelDer;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabDetalleCliente;
        private Panel pnlResumen;
        private Sunny.UI.UILabel lblResumen;
        private Sunny.UI.UILabel lblDetalleTitulo;
        private TableLayoutPanel tlpDetalle;
        private Sunny.UI.UILabel lblCapId;
        private Sunny.UI.UITextBox txtValorId;
        private Sunny.UI.UILabel lblCapNombre;
        private Sunny.UI.UITextBox txtValorNombre;
        private Sunny.UI.UILabel lblCapIdentificacion;
        private Sunny.UI.UITextBox txtValorIdentificacion;
        private Sunny.UI.UILabel lblCapEmpresa;
        private Sunny.UI.UITextBox txtValorEmpresa;
        private Sunny.UI.UILabel lblCapCategoria;
        private Sunny.UI.UITextBox txtValorCategoria;
        private Sunny.UI.UILabel lblCapTelefono;
        private Sunny.UI.UITextBox txtValorTelefono;
        private Sunny.UI.UILabel lblCapContacto;
        private Sunny.UI.UITextBox txtValorContacto;
        private Sunny.UI.UILabel lblCapEmail;
        private Sunny.UI.UITextBox txtValorEmail;
        private Sunny.UI.UILabel lblCapCondicion;
        private Sunny.UI.UITextBox txtValorCondicion;
        private Sunny.UI.UILabel lblCapImpuesto;
        private Sunny.UI.UITextBox txtValorImpuesto;
        private Sunny.UI.UILabel lblCapDireccion;
        private Sunny.UI.UITextBox txtValorDireccion;
        private Sunny.UI.UILabel lblCapUnity1;
        private Sunny.UI.UITextBox txtValorUnity1;
        private Sunny.UI.UILabel lblCapUnity2;
        private Sunny.UI.UITextBox txtValorUnity2;
        private Sunny.UI.UILabel lblCapEstado;
        private Sunny.UI.UITextBox txtValorEstado;
    }
}
