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
            lblValorId = new Sunny.UI.UILabel();
            lblCapNombre = new Sunny.UI.UILabel();
            lblValorNombre = new Sunny.UI.UILabel();
            lblCapIdentificacion = new Sunny.UI.UILabel();
            lblValorIdentificacion = new Sunny.UI.UILabel();
            lblCapEmpresa = new Sunny.UI.UILabel();
            lblValorEmpresa = new Sunny.UI.UILabel();
            lblCapCategoria = new Sunny.UI.UILabel();
            lblValorCategoria = new Sunny.UI.UILabel();
            lblCapTelefono = new Sunny.UI.UILabel();
            lblValorTelefono = new Sunny.UI.UILabel();
            lblCapContacto = new Sunny.UI.UILabel();
            lblValorContacto = new Sunny.UI.UILabel();
            lblCapEmail = new Sunny.UI.UILabel();
            lblValorEmail = new Sunny.UI.UILabel();
            lblCapCondicion = new Sunny.UI.UILabel();
            lblValorCondicion = new Sunny.UI.UILabel();
            lblCapImpuesto = new Sunny.UI.UILabel();
            lblValorImpuesto = new Sunny.UI.UILabel();
            lblCapDireccion = new Sunny.UI.UILabel();
            lblValorDireccion = new Sunny.UI.UILabel();
            lblCapUnity1 = new Sunny.UI.UILabel();
            lblValorUnity1 = new Sunny.UI.UILabel();
            lblCapUnity2 = new Sunny.UI.UILabel();
            lblValorUnity2 = new Sunny.UI.UILabel();
            lblCapEstado = new Sunny.UI.UILabel();
            lblValorEstado = new Sunny.UI.UILabel();
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
            // lblCapId / lblValorId
            // 
            lblCapId.Dock = DockStyle.Fill;
            lblCapId.Font = fuenteCaption;
            lblCapId.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapId.Name = "lblCapId";
            lblCapId.Text = "Código:";
            lblCapId.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapId, 0, 0);
            lblValorId.AutoSize = false;
            lblValorId.Dock = DockStyle.Fill;
            lblValorId.Font = fuenteValor;
            lblValorId.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorId.Name = "lblValorId";
            lblValorId.Text = "—";
            lblValorId.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorId, 1, 0);
            // 
            // lblCapNombre / lblValorNombre
            // 
            lblCapNombre.Dock = DockStyle.Fill;
            lblCapNombre.Font = fuenteCaption;
            lblCapNombre.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapNombre.Name = "lblCapNombre";
            lblCapNombre.Text = "Nombre:";
            lblCapNombre.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapNombre, 0, 1);
            lblValorNombre.AutoSize = false;
            lblValorNombre.Dock = DockStyle.Fill;
            lblValorNombre.Font = fuenteValor;
            lblValorNombre.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorNombre.Name = "lblValorNombre";
            lblValorNombre.Text = "—";
            lblValorNombre.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorNombre, 1, 1);
            // 
            // lblCapIdentificacion / lblValorIdentificacion
            // 
            lblCapIdentificacion.Dock = DockStyle.Fill;
            lblCapIdentificacion.Font = fuenteCaption;
            lblCapIdentificacion.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapIdentificacion.Name = "lblCapIdentificacion";
            lblCapIdentificacion.Text = "Identificación:";
            lblCapIdentificacion.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapIdentificacion, 0, 2);
            lblValorIdentificacion.AutoSize = false;
            lblValorIdentificacion.Dock = DockStyle.Fill;
            lblValorIdentificacion.Font = fuenteValor;
            lblValorIdentificacion.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorIdentificacion.Name = "lblValorIdentificacion";
            lblValorIdentificacion.Text = "—";
            lblValorIdentificacion.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorIdentificacion, 1, 2);
            // 
            // lblCapEmpresa / lblValorEmpresa
            // 
            lblCapEmpresa.Dock = DockStyle.Fill;
            lblCapEmpresa.Font = fuenteCaption;
            lblCapEmpresa.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapEmpresa.Name = "lblCapEmpresa";
            lblCapEmpresa.Text = "Empresa:";
            lblCapEmpresa.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapEmpresa, 0, 3);
            lblValorEmpresa.AutoSize = false;
            lblValorEmpresa.Dock = DockStyle.Fill;
            lblValorEmpresa.Font = fuenteValor;
            lblValorEmpresa.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorEmpresa.Name = "lblValorEmpresa";
            lblValorEmpresa.Text = "—";
            lblValorEmpresa.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorEmpresa, 1, 3);
            // 
            // lblCapCategoria / lblValorCategoria
            // 
            lblCapCategoria.Dock = DockStyle.Fill;
            lblCapCategoria.Font = fuenteCaption;
            lblCapCategoria.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapCategoria.Name = "lblCapCategoria";
            lblCapCategoria.Text = "Categoría:";
            lblCapCategoria.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapCategoria, 0, 4);
            lblValorCategoria.AutoSize = false;
            lblValorCategoria.Dock = DockStyle.Fill;
            lblValorCategoria.Font = fuenteValor;
            lblValorCategoria.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorCategoria.Name = "lblValorCategoria";
            lblValorCategoria.Text = "—";
            lblValorCategoria.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorCategoria, 1, 4);
            // 
            // lblCapTelefono / lblValorTelefono
            // 
            lblCapTelefono.Dock = DockStyle.Fill;
            lblCapTelefono.Font = fuenteCaption;
            lblCapTelefono.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapTelefono.Name = "lblCapTelefono";
            lblCapTelefono.Text = "Teléfono:";
            lblCapTelefono.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapTelefono, 0, 5);
            lblValorTelefono.AutoSize = false;
            lblValorTelefono.Dock = DockStyle.Fill;
            lblValorTelefono.Font = fuenteValor;
            lblValorTelefono.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorTelefono.Name = "lblValorTelefono";
            lblValorTelefono.Text = "—";
            lblValorTelefono.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorTelefono, 1, 5);
            // 
            // lblCapContacto / lblValorContacto
            // 
            lblCapContacto.Dock = DockStyle.Fill;
            lblCapContacto.Font = fuenteCaption;
            lblCapContacto.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapContacto.Name = "lblCapContacto";
            lblCapContacto.Text = "Contacto:";
            lblCapContacto.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapContacto, 0, 6);
            lblValorContacto.AutoSize = false;
            lblValorContacto.Dock = DockStyle.Fill;
            lblValorContacto.Font = fuenteValor;
            lblValorContacto.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorContacto.Name = "lblValorContacto";
            lblValorContacto.Text = "—";
            lblValorContacto.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorContacto, 1, 6);
            // 
            // lblCapEmail / lblValorEmail
            // 
            lblCapEmail.Dock = DockStyle.Fill;
            lblCapEmail.Font = fuenteCaption;
            lblCapEmail.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapEmail.Name = "lblCapEmail";
            lblCapEmail.Text = "Email:";
            lblCapEmail.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapEmail, 0, 7);
            lblValorEmail.AutoSize = false;
            lblValorEmail.Dock = DockStyle.Fill;
            lblValorEmail.Font = fuenteValor;
            lblValorEmail.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorEmail.Name = "lblValorEmail";
            lblValorEmail.Text = "—";
            lblValorEmail.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorEmail, 1, 7);
            // 
            // lblCapCondicion / lblValorCondicion
            // 
            lblCapCondicion.Dock = DockStyle.Fill;
            lblCapCondicion.Font = fuenteCaption;
            lblCapCondicion.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapCondicion.Name = "lblCapCondicion";
            lblCapCondicion.Text = "Condición pago:";
            lblCapCondicion.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapCondicion, 0, 8);
            lblValorCondicion.AutoSize = false;
            lblValorCondicion.Dock = DockStyle.Fill;
            lblValorCondicion.Font = fuenteValor;
            lblValorCondicion.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorCondicion.Name = "lblValorCondicion";
            lblValorCondicion.Text = "—";
            lblValorCondicion.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorCondicion, 1, 8);
            // 
            // lblCapImpuesto / lblValorImpuesto
            // 
            lblCapImpuesto.Dock = DockStyle.Fill;
            lblCapImpuesto.Font = fuenteCaption;
            lblCapImpuesto.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapImpuesto.Name = "lblCapImpuesto";
            lblCapImpuesto.Text = "Impuesto:";
            lblCapImpuesto.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapImpuesto, 0, 9);
            lblValorImpuesto.AutoSize = false;
            lblValorImpuesto.Dock = DockStyle.Fill;
            lblValorImpuesto.Font = fuenteValor;
            lblValorImpuesto.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorImpuesto.Name = "lblValorImpuesto";
            lblValorImpuesto.Text = "—";
            lblValorImpuesto.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorImpuesto, 1, 9);
            // 
            // lblCapDireccion / lblValorDireccion
            // 
            lblCapDireccion.Dock = DockStyle.Fill;
            lblCapDireccion.Font = fuenteCaption;
            lblCapDireccion.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapDireccion.Name = "lblCapDireccion";
            lblCapDireccion.Text = "Dirección:";
            lblCapDireccion.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapDireccion, 0, 10);
            lblValorDireccion.AutoSize = false;
            lblValorDireccion.Dock = DockStyle.Fill;
            lblValorDireccion.Font = fuenteValor;
            lblValorDireccion.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorDireccion.Name = "lblValorDireccion";
            lblValorDireccion.Text = "—";
            lblValorDireccion.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorDireccion, 1, 10);
            // 
            // lblCapUnity1 / lblValorUnity1
            // 
            lblCapUnity1.Dock = DockStyle.Fill;
            lblCapUnity1.Font = fuenteCaption;
            lblCapUnity1.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapUnity1.Name = "lblCapUnity1";
            lblCapUnity1.Text = "Unity 1:";
            lblCapUnity1.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapUnity1, 0, 11);
            lblValorUnity1.AutoSize = false;
            lblValorUnity1.Dock = DockStyle.Fill;
            lblValorUnity1.Font = fuenteValor;
            lblValorUnity1.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorUnity1.Name = "lblValorUnity1";
            lblValorUnity1.Text = "—";
            lblValorUnity1.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorUnity1, 1, 11);
            // 
            // lblCapUnity2 / lblValorUnity2
            // 
            lblCapUnity2.Dock = DockStyle.Fill;
            lblCapUnity2.Font = fuenteCaption;
            lblCapUnity2.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapUnity2.Name = "lblCapUnity2";
            lblCapUnity2.Text = "Unity 2:";
            lblCapUnity2.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapUnity2, 0, 12);
            lblValorUnity2.AutoSize = false;
            lblValorUnity2.Dock = DockStyle.Fill;
            lblValorUnity2.Font = fuenteValor;
            lblValorUnity2.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorUnity2.Name = "lblValorUnity2";
            lblValorUnity2.Text = "—";
            lblValorUnity2.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorUnity2, 1, 12);
            // 
            // lblCapEstado / lblValorEstado
            // 
            lblCapEstado.Dock = DockStyle.Fill;
            lblCapEstado.Font = fuenteCaption;
            lblCapEstado.ForeColor = Color.FromArgb(60, 110, 20);
            lblCapEstado.Name = "lblCapEstado";
            lblCapEstado.Text = "Estado:";
            lblCapEstado.TextAlign = ContentAlignment.MiddleRight;
            tlpDetalle.Controls.Add(lblCapEstado, 0, 13);
            lblValorEstado.AutoSize = false;
            lblValorEstado.Dock = DockStyle.Fill;
            lblValorEstado.Font = fuenteValor;
            lblValorEstado.ForeColor = Color.FromArgb(48, 48, 48);
            lblValorEstado.Name = "lblValorEstado";
            lblValorEstado.Text = "—";
            lblValorEstado.TextAlign = ContentAlignment.MiddleLeft;
            tlpDetalle.Controls.Add(lblValorEstado, 1, 13);
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
        private Sunny.UI.UILabel lblValorId;
        private Sunny.UI.UILabel lblCapNombre;
        private Sunny.UI.UILabel lblValorNombre;
        private Sunny.UI.UILabel lblCapIdentificacion;
        private Sunny.UI.UILabel lblValorIdentificacion;
        private Sunny.UI.UILabel lblCapEmpresa;
        private Sunny.UI.UILabel lblValorEmpresa;
        private Sunny.UI.UILabel lblCapCategoria;
        private Sunny.UI.UILabel lblValorCategoria;
        private Sunny.UI.UILabel lblCapTelefono;
        private Sunny.UI.UILabel lblValorTelefono;
        private Sunny.UI.UILabel lblCapContacto;
        private Sunny.UI.UILabel lblValorContacto;
        private Sunny.UI.UILabel lblCapEmail;
        private Sunny.UI.UILabel lblValorEmail;
        private Sunny.UI.UILabel lblCapCondicion;
        private Sunny.UI.UILabel lblValorCondicion;
        private Sunny.UI.UILabel lblCapImpuesto;
        private Sunny.UI.UILabel lblValorImpuesto;
        private Sunny.UI.UILabel lblCapDireccion;
        private Sunny.UI.UILabel lblValorDireccion;
        private Sunny.UI.UILabel lblCapUnity1;
        private Sunny.UI.UILabel lblValorUnity1;
        private Sunny.UI.UILabel lblCapUnity2;
        private Sunny.UI.UILabel lblValorUnity2;
        private Sunny.UI.UILabel lblCapEstado;
        private Sunny.UI.UILabel lblValorEstado;
    }
}
