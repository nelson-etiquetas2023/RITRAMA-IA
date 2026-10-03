namespace Ritrama2025.Forms
{
    partial class FrmUsuarios
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
        /// InitializeComponent del rediseño 30/70 del modulo de Usuarios, sobre el
        /// esquema de FrmProductos: panel izquierdo con buscador + listado + resumen,
        /// panel derecho con el tab de detalle.
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
            gridUsuarios = new Sunny.UI.UIDataGridView();
            colUsuarioId = new DataGridViewTextBoxColumn();
            colUsuario = new DataGridViewTextBoxColumn();
            colNombre = new DataGridViewTextBoxColumn();
            colRol = new DataGridViewTextBoxColumn();
            colEstado = new DataGridViewTextBoxColumn();
            pnlBuscador = new Panel();
            txtBuscar = new Sunny.UI.UITextBox();
            pnlResumen = new Panel();
            lblResumenDetalle = new Sunny.UI.UILabel();
            lblResumen = new Sunny.UI.UILabel();
            panelDer = new Panel();
            tabDetalle = new Sunny.UI.UITabControl();
            tabDetalleUsuario = new TabPage();
            tlpDetalle = new TableLayoutPanel();
            lblCapId = new Sunny.UI.UILabel();
            txtDetId = new Sunny.UI.UITextBox();
            lblCapUsuario = new Sunny.UI.UILabel();
            txtDetUsuario = new Sunny.UI.UITextBox();
            lblCapPassword = new Sunny.UI.UILabel();
            pnlPassword = new Panel();
            txtPassword = new Sunny.UI.UITextBox();
            btnOjoPassword = new Sunny.UI.UIButton();
            lblCapNombre = new Sunny.UI.UILabel();
            txtDetNombre = new Sunny.UI.UITextBox();
            lblCapEmail = new Sunny.UI.UILabel();
            txtDetEmail = new Sunny.UI.UITextBox();
            lblCapTituloCargo = new Sunny.UI.UILabel();
            txtDetTituloCargo = new Sunny.UI.UITextBox();
            lblCapDepartamento = new Sunny.UI.UILabel();
            txtDetDepartamento = new Sunny.UI.UITextBox();
            lblCapRoles = new Sunny.UI.UILabel();
            txtDetRoles = new Sunny.UI.UITextBox();
            cboRoles = new Sunny.UI.UIComboBox();
            lblCapFechaCreacion = new Sunny.UI.UILabel();
            txtDetFechaCreacion = new Sunny.UI.UITextBox();
            lblCapActivo = new Sunny.UI.UILabel();
            swDetActivo = new Sunny.UI.UISwitch();
            lblDetalleTitulo = new Sunny.UI.UILabel();
            barraHerramientas = new ToolStrip();
            btnNuevoUsuario = new ToolStripButton();
            btnEditarUsuario = new ToolStripButton();
            btnExportarUsuario = new ToolStripButton();
            btnReporteUsuario = new ToolStripButton();
            btnGuardarUsuario = new ToolStripButton();
            btnCancelarUsuario = new ToolStripButton();
            panelTitulo = new Panel();
            lblTitulo = new Sunny.UI.UILabel();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUsuarios).BeginInit();
            pnlBuscador.SuspendLayout();
            pnlResumen.SuspendLayout();
            panelDer.SuspendLayout();
            tabDetalle.SuspendLayout();
            tabDetalleUsuario.SuspendLayout();
            tlpDetalle.SuspendLayout();
            pnlPassword.SuspendLayout();
            barraHerramientas.SuspendLayout();
            panelTitulo.SuspendLayout();
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
            tlpRoot.Location = new Point(0, 95);
            tlpRoot.Margin = new Padding(0);
            tlpRoot.Name = "tlpRoot";
            tlpRoot.RowCount = 1;
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpRoot.Size = new Size(1150, 617);
            tlpRoot.TabIndex = 1;
            // 
            // panelIzq
            // 
            panelIzq.BackColor = Color.White;
            panelIzq.BorderStyle = BorderStyle.FixedSingle;
            panelIzq.Controls.Add(gridUsuarios);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Controls.Add(pnlResumen);
            panelIzq.Dock = DockStyle.Fill;
            panelIzq.Location = new Point(0, 0);
            panelIzq.Margin = new Padding(0);
            panelIzq.MinimumSize = new Size(260, 0);
            panelIzq.Name = "panelIzq";
            panelIzq.Size = new Size(345, 617);
            panelIzq.TabIndex = 0;
            // 
            // gridUsuarios
            // 
            gridUsuarios.AllowUserToAddRows = false;
            gridUsuarios.AllowUserToDeleteRows = false;
            gridUsuarios.AllowUserToResizeRows = false;
            dataGridViewCellStyle1.BackColor = Color.FromArgb(245, 250, 240);
            dataGridViewCellStyle1.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle1.SelectionBackColor = Color.Black;
            dataGridViewCellStyle1.SelectionForeColor = Color.White;
            gridUsuarios.AlternatingRowsDefaultCellStyle = dataGridViewCellStyle1;
            gridUsuarios.AutoSizeColumnsMode = DataGridViewAutoSizeColumnsMode.Fill;
            gridUsuarios.BackgroundColor = Color.White;
            gridUsuarios.ColumnHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle2.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle2.BackColor = Color.FromArgb(110, 190, 40);
            dataGridViewCellStyle2.Font = new Font("Microsoft Sans Serif", 9F, FontStyle.Bold);
            dataGridViewCellStyle2.ForeColor = Color.White;
            dataGridViewCellStyle2.SelectionBackColor = Color.FromArgb(110, 190, 40);
            dataGridViewCellStyle2.SelectionForeColor = Color.White;
            dataGridViewCellStyle2.WrapMode = DataGridViewTriState.True;
            gridUsuarios.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle2;
            gridUsuarios.ColumnHeadersHeight = 32;
            gridUsuarios.ColumnHeadersHeightSizeMode = DataGridViewColumnHeadersHeightSizeMode.DisableResizing;
            gridUsuarios.Columns.AddRange(new DataGridViewColumn[] { colUsuarioId, colUsuario, colNombre, colRol, colEstado });
            dataGridViewCellStyle3.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle3.BackColor = Color.White;
            dataGridViewCellStyle3.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle3.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle3.SelectionBackColor = Color.Black;
            dataGridViewCellStyle3.SelectionForeColor = Color.White;
            dataGridViewCellStyle3.WrapMode = DataGridViewTriState.False;
            gridUsuarios.DefaultCellStyle = dataGridViewCellStyle3;
            gridUsuarios.Dock = DockStyle.Fill;
            gridUsuarios.EnableHeadersVisualStyles = false;
            gridUsuarios.Font = new Font("Microsoft Sans Serif", 9F);
            gridUsuarios.GridColor = Color.FromArgb(180, 210, 180);
            gridUsuarios.Location = new Point(0, 44);
            gridUsuarios.MultiSelect = false;
            gridUsuarios.Name = "gridUsuarios";
            gridUsuarios.ReadOnly = true;
            gridUsuarios.RowHeadersBorderStyle = DataGridViewHeaderBorderStyle.Single;
            dataGridViewCellStyle4.Alignment = DataGridViewContentAlignment.MiddleLeft;
            dataGridViewCellStyle4.BackColor = Color.FromArgb(235, 243, 255);
            dataGridViewCellStyle4.Font = new Font("Microsoft Sans Serif", 12F);
            dataGridViewCellStyle4.ForeColor = Color.FromArgb(48, 48, 48);
            dataGridViewCellStyle4.SelectionBackColor = Color.FromArgb(80, 160, 255);
            dataGridViewCellStyle4.SelectionForeColor = Color.White;
            dataGridViewCellStyle4.WrapMode = DataGridViewTriState.True;
            gridUsuarios.RowHeadersDefaultCellStyle = dataGridViewCellStyle4;
            gridUsuarios.RowHeadersVisible = false;
            dataGridViewCellStyle5.BackColor = Color.White;
            dataGridViewCellStyle5.Font = new Font("Microsoft Sans Serif", 9F);
            dataGridViewCellStyle5.SelectionBackColor = Color.Black;
            dataGridViewCellStyle5.SelectionForeColor = Color.White;
            gridUsuarios.RowsDefaultCellStyle = dataGridViewCellStyle5;
            gridUsuarios.SelectedIndex = -1;
            gridUsuarios.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            gridUsuarios.Size = new Size(343, 511);
            gridUsuarios.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridUsuarios.TabIndex = 2;
            // 
            // colUsuarioId
            // 
            colUsuarioId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colUsuarioId.DataPropertyName = "UserId";
            colUsuarioId.FillWeight = 10F;
            colUsuarioId.HeaderText = "ID";
            colUsuarioId.Name = "colUsuarioId";
            colUsuarioId.ReadOnly = true;
            // 
            // colUsuario
            // 
            colUsuario.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colUsuario.DataPropertyName = "Username";
            colUsuario.FillWeight = 25F;
            colUsuario.HeaderText = "Usuario";
            colUsuario.Name = "colUsuario";
            colUsuario.ReadOnly = true;
            // 
            // colNombre
            // 
            colNombre.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colNombre.DataPropertyName = "Nombre";
            colNombre.FillWeight = 25F;
            colNombre.HeaderText = "Nombre";
            colNombre.Name = "colNombre";
            colNombre.ReadOnly = true;
            // 
            // colRol
            // 
            colRol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colRol.DataPropertyName = "Rol";
            colRol.FillWeight = 25F;
            colRol.HeaderText = "Rol";
            colRol.Name = "colRol";
            colRol.ReadOnly = true;
            // 
            // colEstado
            // 
            colEstado.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colEstado.DataPropertyName = "Estado";
            colEstado.FillWeight = 15F;
            colEstado.HeaderText = "Estado";
            colEstado.Name = "colEstado";
            colEstado.ReadOnly = true;
            // 
            // pnlBuscador
            // 
            pnlBuscador.BackColor = Color.White;
            pnlBuscador.Controls.Add(txtBuscar);
            pnlBuscador.Dock = DockStyle.Top;
            pnlBuscador.Location = new Point(0, 0);
            pnlBuscador.Name = "pnlBuscador";
            pnlBuscador.Padding = new Padding(8, 6, 8, 6);
            pnlBuscador.Size = new Size(343, 44);
            pnlBuscador.TabIndex = 0;
            // 
            // txtBuscar
            // 
            txtBuscar.Dock = DockStyle.Fill;
            txtBuscar.Font = new Font("Microsoft Sans Serif", 9F);
            txtBuscar.Location = new Point(8, 6);
            txtBuscar.Margin = new Padding(4, 5, 4, 5);
            txtBuscar.MinimumSize = new Size(1, 16);
            txtBuscar.Name = "txtBuscar";
            txtBuscar.Padding = new Padding(6, 0, 6, 0);
            txtBuscar.RectColor = Color.FromArgb(110, 190, 40);
            txtBuscar.ShowText = false;
            txtBuscar.Size = new Size(327, 32);
            txtBuscar.TabIndex = 0;
            txtBuscar.TextAlignment = ContentAlignment.MiddleLeft;
            txtBuscar.Watermark = "Buscar por usuario, nombre o correo...";
            // 
            // pnlResumen
            // 
            pnlResumen.BackColor = Color.FromArgb(110, 190, 40);
            pnlResumen.Controls.Add(lblResumenDetalle);
            pnlResumen.Controls.Add(lblResumen);
            pnlResumen.Dock = DockStyle.Bottom;
            pnlResumen.Location = new Point(0, 555);
            pnlResumen.Name = "pnlResumen";
            pnlResumen.Padding = new Padding(10, 4, 10, 4);
            pnlResumen.Size = new Size(343, 60);
            pnlResumen.TabIndex = 1;
            // 
            // lblResumenDetalle
            // 
            lblResumenDetalle.Dock = DockStyle.Fill;
            lblResumenDetalle.Font = new Font("Microsoft Sans Serif", 8F);
            lblResumenDetalle.ForeColor = Color.White;
            lblResumenDetalle.Location = new Point(10, 30);
            lblResumenDetalle.Name = "lblResumenDetalle";
            lblResumenDetalle.Size = new Size(323, 26);
            lblResumenDetalle.TabIndex = 1;
            lblResumenDetalle.Text = "Mostrando 0  -  Activos 0  -  Inactivos 0";
            lblResumenDetalle.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // lblResumen
            // 
            lblResumen.Dock = DockStyle.Top;
            lblResumen.Font = new Font("Microsoft Sans Serif", 11F, FontStyle.Bold);
            lblResumen.ForeColor = Color.White;
            lblResumen.Location = new Point(10, 4);
            lblResumen.Name = "lblResumen";
            lblResumen.Size = new Size(323, 26);
            lblResumen.TabIndex = 0;
            lblResumen.Text = "Usuarios registrados: 0";
            lblResumen.TextAlign = ContentAlignment.MiddleLeft;
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
            panelDer.Size = new Size(799, 611);
            panelDer.TabIndex = 1;
            // 
            // tabDetalle
            // 
            tabDetalle.Controls.Add(tabDetalleUsuario);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabDetalle.Font = new Font("Microsoft Sans Serif", 9F);
            tabDetalle.ItemSize = new Size(160, 32);
            tabDetalle.Location = new Point(8, 48);
            tabDetalle.MainPage = "";
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(783, 555);
            tabDetalle.SizeMode = TabSizeMode.Fixed;
            tabDetalle.TabIndex = 0;
            tabDetalle.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            tabDetalle.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // tabDetalleUsuario
            // 
            tabDetalleUsuario.BackColor = Color.White;
            tabDetalleUsuario.Controls.Add(tlpDetalle);
            tabDetalleUsuario.Controls.Add(lblDetalleTitulo);
            tabDetalleUsuario.Location = new Point(0, 32);
            tabDetalleUsuario.Name = "tabDetalleUsuario";
            tabDetalleUsuario.Size = new Size(783, 523);
            tabDetalleUsuario.TabIndex = 0;
            tabDetalleUsuario.Text = "Detalle";
            // 
            // tlpDetalle
            // 
            tlpDetalle.ColumnCount = 2;
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 170F));
            tlpDetalle.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100F));
            tlpDetalle.Controls.Add(lblCapId, 0, 0);
            tlpDetalle.Controls.Add(txtDetId, 1, 0);
            tlpDetalle.Controls.Add(lblCapUsuario, 0, 1);
            tlpDetalle.Controls.Add(txtDetUsuario, 1, 1);
            tlpDetalle.Controls.Add(lblCapPassword, 0, 2);
            tlpDetalle.Controls.Add(pnlPassword, 1, 2);
            tlpDetalle.Controls.Add(lblCapNombre, 0, 3);
            tlpDetalle.Controls.Add(txtDetNombre, 1, 3);
            tlpDetalle.Controls.Add(lblCapEmail, 0, 4);
            tlpDetalle.Controls.Add(txtDetEmail, 1, 4);
            tlpDetalle.Controls.Add(lblCapTituloCargo, 0, 5);
            tlpDetalle.Controls.Add(txtDetTituloCargo, 1, 5);
            tlpDetalle.Controls.Add(lblCapDepartamento, 0, 6);
            tlpDetalle.Controls.Add(txtDetDepartamento, 1, 6);
            tlpDetalle.Controls.Add(lblCapRoles, 0, 7);
            tlpDetalle.Controls.Add(txtDetRoles, 1, 7);
            tlpDetalle.Controls.Add(cboRoles, 1, 7);
            tlpDetalle.Controls.Add(lblCapFechaCreacion, 0, 8);
            tlpDetalle.Controls.Add(txtDetFechaCreacion, 1, 8);
            tlpDetalle.Controls.Add(lblCapActivo, 0, 9);
            tlpDetalle.Controls.Add(swDetActivo, 1, 9);
            tlpDetalle.Dock = DockStyle.Fill;
            tlpDetalle.Location = new Point(0, 26);
            tlpDetalle.Name = "tlpDetalle";
            tlpDetalle.Padding = new Padding(12, 10, 12, 10);
            tlpDetalle.RowCount = 10;
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            tlpDetalle.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
            // Diez filas de 38 y ni una mas: sobraba un RowStyle de 20 px de un layout
            // anterior. RowCount = 10 hacia que el TableLayoutPanel lo ignorara, pero
            // dejaba el Designer con 11 estilos para 10 filas y cualquier lectura de las
            // alturas daba un numero que no era el de la fila.
            tlpDetalle.Size = new Size(783, 497);
            tlpDetalle.TabIndex = 1;
            // 
            // lblCapId
            // 
            lblCapId.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapId.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapId.Location = new Point(15, 10);
            lblCapId.Name = "lblCapId";
            lblCapId.Size = new Size(100, 23);
            lblCapId.TabIndex = 0;
            lblCapId.Text = "Código";
            // 
            // txtDetId
            // 
            txtDetId.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetId.Location = new Point(186, 15);
            txtDetId.Margin = new Padding(4, 5, 4, 5);
            txtDetId.MinimumSize = new Size(1, 16);
            txtDetId.Name = "txtDetId";
            txtDetId.Padding = new Padding(5);
            txtDetId.ShowText = false;
            txtDetId.Size = new Size(150, 28);
            txtDetId.TabIndex = 1;
            txtDetId.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetId.Watermark = "";
            // 
            // lblCapUsuario
            // 
            lblCapUsuario.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapUsuario.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapUsuario.Location = new Point(15, 48);
            lblCapUsuario.Name = "lblCapUsuario";
            lblCapUsuario.Size = new Size(100, 23);
            lblCapUsuario.TabIndex = 2;
            lblCapUsuario.Text = "Usuario";
            // 
            // txtDetUsuario
            // 
            txtDetUsuario.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetUsuario.Location = new Point(186, 53);
            txtDetUsuario.Margin = new Padding(4, 5, 4, 5);
            txtDetUsuario.MinimumSize = new Size(1, 16);
            txtDetUsuario.Name = "txtDetUsuario";
            txtDetUsuario.Padding = new Padding(5);
            txtDetUsuario.ShowText = false;
            txtDetUsuario.Size = new Size(150, 28);
            txtDetUsuario.TabIndex = 3;
            txtDetUsuario.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetUsuario.Watermark = "";
            // 
            // lblCapPassword
            // 
            lblCapPassword.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapPassword.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapPassword.Location = new Point(15, 86);
            lblCapPassword.Name = "lblCapPassword";
            lblCapPassword.Size = new Size(100, 23);
            lblCapPassword.TabIndex = 4;
            lblCapPassword.Text = "Contraseña";
            // 
            // pnlPassword
            // 
            pnlPassword.BackColor = Color.White;
            pnlPassword.Controls.Add(txtPassword);
            pnlPassword.Controls.Add(btnOjoPassword);
            pnlPassword.Dock = DockStyle.Fill;
            pnlPassword.Location = new Point(185, 92);
            pnlPassword.Margin = new Padding(3, 6, 3, 6);
            pnlPassword.Name = "pnlPassword";
            pnlPassword.Size = new Size(583, 26);
            pnlPassword.TabIndex = 5;
            // 
            // txtPassword
            // 
            txtPassword.Dock = DockStyle.Fill;
            txtPassword.Font = new Font("Microsoft Sans Serif", 9F);
            txtPassword.Location = new Point(0, 0);
            txtPassword.Margin = new Padding(4, 5, 4, 5);
            txtPassword.MinimumSize = new Size(1, 16);
            txtPassword.Name = "txtPassword";
            txtPassword.Padding = new Padding(6, 0, 6, 0);
            txtPassword.ReadOnly = true;
            txtPassword.RectColor = Color.FromArgb(110, 190, 40);
            txtPassword.ShowText = false;
            txtPassword.Size = new Size(557, 26);
            txtPassword.TabIndex = 0;
            txtPassword.TextAlignment = ContentAlignment.MiddleLeft;
            txtPassword.Watermark = "";
            // 
            // btnOjoPassword
            // 
            btnOjoPassword.Cursor = Cursors.Hand;
            btnOjoPassword.Dock = DockStyle.Right;
            btnOjoPassword.FillColor = Color.FromArgb(255, 255, 255);
            btnOjoPassword.FillColor2 = Color.FromArgb(255, 255, 255);
            btnOjoPassword.Font = new Font("Segoe UI Emoji", 10F);
            btnOjoPassword.ForeColor = Color.FromArgb(90, 90, 90);
            btnOjoPassword.Location = new Point(557, 0);
            btnOjoPassword.Margin = new Padding(0, 0, 2, 0);
            btnOjoPassword.MinimumSize = new Size(1, 1);
            btnOjoPassword.Name = "btnOjoPassword";
            btnOjoPassword.RectColor = Color.FromArgb(110, 190, 40);
            btnOjoPassword.Size = new Size(26, 26);
            btnOjoPassword.TabIndex = 1;
            btnOjoPassword.TabStop = false;
            btnOjoPassword.Text = "👁";
            btnOjoPassword.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // lblCapNombre
            // 
            lblCapNombre.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapNombre.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapNombre.Location = new Point(15, 124);
            lblCapNombre.Name = "lblCapNombre";
            lblCapNombre.Size = new Size(100, 23);
            lblCapNombre.TabIndex = 6;
            lblCapNombre.Text = "Nombre";
            // 
            // txtDetNombre
            // 
            txtDetNombre.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetNombre.Location = new Point(186, 129);
            txtDetNombre.Margin = new Padding(4, 5, 4, 5);
            txtDetNombre.MinimumSize = new Size(1, 16);
            txtDetNombre.Name = "txtDetNombre";
            txtDetNombre.Padding = new Padding(5);
            txtDetNombre.ShowText = false;
            txtDetNombre.Size = new Size(150, 28);
            txtDetNombre.TabIndex = 7;
            txtDetNombre.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetNombre.Watermark = "";
            // 
            // lblCapEmail
            // 
            lblCapEmail.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapEmail.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapEmail.Location = new Point(15, 162);
            lblCapEmail.Name = "lblCapEmail";
            lblCapEmail.Size = new Size(100, 23);
            lblCapEmail.TabIndex = 8;
            lblCapEmail.Text = "Email";
            // 
            // txtDetEmail
            // 
            txtDetEmail.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetEmail.Location = new Point(186, 167);
            txtDetEmail.Margin = new Padding(4, 5, 4, 5);
            txtDetEmail.MinimumSize = new Size(1, 16);
            txtDetEmail.Name = "txtDetEmail";
            txtDetEmail.Padding = new Padding(5);
            txtDetEmail.ShowText = false;
            txtDetEmail.Size = new Size(150, 28);
            txtDetEmail.TabIndex = 9;
            txtDetEmail.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetEmail.Watermark = "";
            // 
            // lblCapTituloCargo
            // 
            lblCapTituloCargo.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapTituloCargo.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapTituloCargo.Location = new Point(15, 200);
            lblCapTituloCargo.Name = "lblCapTituloCargo";
            lblCapTituloCargo.Size = new Size(100, 23);
            lblCapTituloCargo.TabIndex = 10;
            lblCapTituloCargo.Text = "Cargo";
            // 
            // txtDetTituloCargo
            // 
            txtDetTituloCargo.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetTituloCargo.Location = new Point(186, 205);
            txtDetTituloCargo.Margin = new Padding(4, 5, 4, 5);
            txtDetTituloCargo.MinimumSize = new Size(1, 16);
            txtDetTituloCargo.Name = "txtDetTituloCargo";
            txtDetTituloCargo.Padding = new Padding(5);
            txtDetTituloCargo.ShowText = false;
            txtDetTituloCargo.Size = new Size(150, 28);
            txtDetTituloCargo.TabIndex = 11;
            txtDetTituloCargo.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetTituloCargo.Watermark = "";
            // 
            // lblCapDepartamento
            // 
            lblCapDepartamento.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapDepartamento.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapDepartamento.Location = new Point(15, 238);
            lblCapDepartamento.Name = "lblCapDepartamento";
            lblCapDepartamento.Size = new Size(100, 23);
            lblCapDepartamento.TabIndex = 12;
            lblCapDepartamento.Text = "Departamento";
            // 
            // txtDetDepartamento
            // 
            txtDetDepartamento.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetDepartamento.Location = new Point(186, 243);
            txtDetDepartamento.Margin = new Padding(4, 5, 4, 5);
            txtDetDepartamento.MinimumSize = new Size(1, 16);
            txtDetDepartamento.Name = "txtDetDepartamento";
            txtDetDepartamento.Padding = new Padding(5);
            txtDetDepartamento.ShowText = false;
            txtDetDepartamento.Size = new Size(150, 28);
            txtDetDepartamento.TabIndex = 13;
            txtDetDepartamento.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetDepartamento.Watermark = "";
            // 
            // lblCapRoles
            // 
            lblCapRoles.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapRoles.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapRoles.Location = new Point(15, 276);
            lblCapRoles.Name = "lblCapRoles";
            lblCapRoles.Size = new Size(100, 23);
            lblCapRoles.TabIndex = 14;
            lblCapRoles.Text = "Roles";
            // 
            // txtDetRoles
            // 
            txtDetRoles.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetRoles.Location = new Point(16, 319);
            txtDetRoles.Margin = new Padding(4, 5, 4, 5);
            txtDetRoles.MinimumSize = new Size(1, 16);
            txtDetRoles.Name = "txtDetRoles";
            txtDetRoles.Padding = new Padding(5);
            txtDetRoles.ShowText = false;
            txtDetRoles.Size = new Size(150, 28);
            txtDetRoles.TabIndex = 15;
            txtDetRoles.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetRoles.Watermark = "";
            // 
            // cboRoles
            // 
            cboRoles.DataSource = null;
            cboRoles.Dock = DockStyle.Fill;
            cboRoles.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
            cboRoles.FillColor = Color.White;
            cboRoles.Font = new Font("Microsoft Sans Serif", 9F);
            cboRoles.ItemHoverColor = Color.FromArgb(155, 200, 255);
            cboRoles.ItemSelectForeColor = Color.FromArgb(235, 243, 255);
            cboRoles.Location = new Point(185, 282);
            cboRoles.Margin = new Padding(3, 6, 3, 6);
            cboRoles.MinimumSize = new Size(63, 0);
            cboRoles.Name = "cboRoles";
            cboRoles.Padding = new Padding(0, 0, 30, 2);
            cboRoles.RectColor = Color.FromArgb(110, 190, 40);
            cboRoles.Size = new Size(583, 26);
            cboRoles.SymbolSize = 24;
            cboRoles.TabIndex = 16;
            cboRoles.TextAlignment = ContentAlignment.MiddleLeft;
            cboRoles.Visible = false;
            cboRoles.Watermark = "";
            // 
            // lblCapFechaCreacion
            // 
            lblCapFechaCreacion.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapFechaCreacion.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapFechaCreacion.Location = new Point(185, 314);
            lblCapFechaCreacion.Name = "lblCapFechaCreacion";
            lblCapFechaCreacion.Size = new Size(100, 23);
            lblCapFechaCreacion.TabIndex = 17;
            lblCapFechaCreacion.Text = "Fecha de creación";
            // 
            // txtDetFechaCreacion
            // 
            txtDetFechaCreacion.Font = new Font("Microsoft Sans Serif", 12F);
            txtDetFechaCreacion.Location = new Point(16, 357);
            txtDetFechaCreacion.Margin = new Padding(4, 5, 4, 5);
            txtDetFechaCreacion.MinimumSize = new Size(1, 16);
            txtDetFechaCreacion.Name = "txtDetFechaCreacion";
            txtDetFechaCreacion.Padding = new Padding(5);
            txtDetFechaCreacion.ShowText = false;
            txtDetFechaCreacion.Size = new Size(150, 28);
            txtDetFechaCreacion.TabIndex = 18;
            txtDetFechaCreacion.TextAlignment = ContentAlignment.MiddleLeft;
            txtDetFechaCreacion.Watermark = "";
            // 
            // lblCapActivo
            // 
            lblCapActivo.Font = new Font("Microsoft Sans Serif", 12F);
            lblCapActivo.ForeColor = Color.FromArgb(48, 48, 48);
            lblCapActivo.Location = new Point(185, 352);
            lblCapActivo.Name = "lblCapActivo";
            lblCapActivo.Size = new Size(100, 23);
            lblCapActivo.TabIndex = 19;
            lblCapActivo.Text = "Estado";
            // 
            // swDetActivo
            // 
            swDetActivo.ActiveColor = Color.FromArgb(110, 190, 40);
            swDetActivo.Font = new Font("Microsoft Sans Serif", 12F);
            swDetActivo.Location = new Point(17, 394);
            // El texto del rótulo arranca en Padding.Left = 2 y el rótulo pegado al borde de la
            // celda, así que el switch arranca en 3: si no, el botón queda a la izquierda
            // del texto y las dos líneas no leen como una. El 4 vertical centra los 29 px
            // del switch en la fila de 38.
            swDetActivo.Margin = new Padding(3, 4, 3, 3);
            swDetActivo.MinimumSize = new Size(1, 1);
            swDetActivo.Name = "swDetActivo";
            swDetActivo.Size = new Size(75, 29);
            swDetActivo.TabIndex = 20;
            swDetActivo.Visible = false;
            // 
            // lblDetalleTitulo
            // 
            lblDetalleTitulo.Dock = DockStyle.Top;
            lblDetalleTitulo.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            lblDetalleTitulo.ForeColor = Color.FromArgb(60, 110, 20);
            lblDetalleTitulo.Location = new Point(0, 0);
            lblDetalleTitulo.Name = "lblDetalleTitulo";
            lblDetalleTitulo.Padding = new Padding(8, 0, 0, 0);
            lblDetalleTitulo.Size = new Size(783, 26);
            lblDetalleTitulo.TabIndex = 0;
            lblDetalleTitulo.Text = "DETALLE DEL USUARIO";
            lblDetalleTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // barraHerramientas
            // 
            barraHerramientas.AutoSize = false;
            barraHerramientas.BackColor = Color.FromArgb(225, 225, 225);
            barraHerramientas.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            barraHerramientas.ForeColor = Color.FromArgb(80, 80, 80);
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevoUsuario, btnEditarUsuario, btnExportarUsuario, btnReporteUsuario, btnGuardarUsuario, btnCancelarUsuario });
            barraHerramientas.Location = new Point(8, 8);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 2, 0, 2);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(783, 40);
            barraHerramientas.TabIndex = 2;
            barraHerramientas.Text = "barraHerramientas";
            // 
            // btnNuevoUsuario
            // 
            btnNuevoUsuario.AutoSize = false;
            btnNuevoUsuario.BackColor = Color.Transparent;
            btnNuevoUsuario.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnNuevoUsuario.ForeColor = Color.FromArgb(80, 80, 80);
            btnNuevoUsuario.Image = Properties.Resources.add_file_32px;
            btnNuevoUsuario.ImageAlign = ContentAlignment.MiddleLeft;
            btnNuevoUsuario.ImageScaling = ToolStripItemImageScaling.None;
            btnNuevoUsuario.Margin = new Padding(4, 1, 0, 2);
            btnNuevoUsuario.Name = "btnNuevoUsuario";
            btnNuevoUsuario.Size = new Size(92, 36);
            btnNuevoUsuario.Text = "Nuevo";
            btnNuevoUsuario.ToolTipText = "Nuevo usuario";
            // 
            // btnEditarUsuario
            // 
            btnEditarUsuario.AutoSize = false;
            btnEditarUsuario.BackColor = Color.Transparent;
            btnEditarUsuario.Enabled = false;
            btnEditarUsuario.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnEditarUsuario.ForeColor = Color.FromArgb(80, 80, 80);
            btnEditarUsuario.Image = Properties.Resources.edit_24px;
            btnEditarUsuario.ImageAlign = ContentAlignment.MiddleLeft;
            btnEditarUsuario.ImageScaling = ToolStripItemImageScaling.None;
            btnEditarUsuario.Margin = new Padding(4, 1, 0, 2);
            btnEditarUsuario.Name = "btnEditarUsuario";
            btnEditarUsuario.Size = new Size(92, 36);
            btnEditarUsuario.Text = "Editar";
            btnEditarUsuario.ToolTipText = "Editar el usuario seleccionado";
            // 
            // btnExportarUsuario
            // 
            // Icono de Excel de 16 px (excel_16px, el PNG microsoft_excel_2019_16.png),
            // el mismo que el boton equivalente de Productos: los otros de Excel del
            // proyecto son de 48 px y desbordarian la barra. ToolStripTheme.Ajustar le
            // pone despues icono 24x24 y AutoSize, igual que al resto.
            btnExportarUsuario.AutoSize = false;
            btnExportarUsuario.BackColor = Color.Transparent;
            btnExportarUsuario.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnExportarUsuario.ForeColor = Color.FromArgb(80, 80, 80);
            btnExportarUsuario.Image = Properties.Resources.excel_16px;
            btnExportarUsuario.ImageAlign = ContentAlignment.MiddleLeft;
            btnExportarUsuario.ImageScaling = ToolStripItemImageScaling.None;
            btnExportarUsuario.Margin = new Padding(4, 1, 0, 2);
            btnExportarUsuario.Name = "btnExportarUsuario";
            btnExportarUsuario.Size = new Size(92, 36);
            btnExportarUsuario.Text = "Exportar";
            btnExportarUsuario.ToolTipText = "Crear una hoja de Excel con todos los usuarios";
            // 
            // btnReporteUsuario
            // 
            // Icono de informe de 32 px (reports_32px). Los otros de informe del proyecto
            // (report_48, print_48px, report48_48) son de 48 px y con ImageScaling = None
            // desbordarian este boton, que es de 36 px de alto. ToolStripTheme.Ajustar le
            // pone despues icono 24x24 y AutoSize, igual que al resto de la barra.
            btnReporteUsuario.AutoSize = false;
            btnReporteUsuario.BackColor = Color.Transparent;
            btnReporteUsuario.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnReporteUsuario.ForeColor = Color.FromArgb(80, 80, 80);
            btnReporteUsuario.Image = Properties.Resources.reports_32px;
            btnReporteUsuario.ImageAlign = ContentAlignment.MiddleLeft;
            btnReporteUsuario.ImageScaling = ToolStripItemImageScaling.None;
            btnReporteUsuario.Margin = new Padding(4, 1, 0, 2);
            btnReporteUsuario.Name = "btnReporteUsuario";
            btnReporteUsuario.Size = new Size(92, 36);
            btnReporteUsuario.Text = "Reporte";
            btnReporteUsuario.ToolTipText = "Ver el catalogo de usuarios en el visor de reportes";
            btnGuardarUsuario.AutoSize = false;
            btnGuardarUsuario.BackColor = Color.Transparent;
            btnGuardarUsuario.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnGuardarUsuario.ForeColor = Color.FromArgb(80, 80, 80);
            btnGuardarUsuario.ImageAlign = ContentAlignment.MiddleLeft;
            btnGuardarUsuario.ImageScaling = ToolStripItemImageScaling.None;
            btnGuardarUsuario.Margin = new Padding(16, 1, 0, 2);
            btnGuardarUsuario.Name = "btnGuardarUsuario";
            btnGuardarUsuario.Size = new Size(92, 36);
            btnGuardarUsuario.Text = "Guardar";
            btnGuardarUsuario.ToolTipText = "Guardar el usuario";
            btnGuardarUsuario.Visible = false;
            // 
            // btnCancelarUsuario
            // 
            btnCancelarUsuario.AutoSize = false;
            btnCancelarUsuario.BackColor = Color.Transparent;
            btnCancelarUsuario.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            btnCancelarUsuario.ForeColor = Color.FromArgb(80, 80, 80);
            btnCancelarUsuario.Image = Properties.Resources.cancel_24px;
            btnCancelarUsuario.ImageAlign = ContentAlignment.MiddleLeft;
            btnCancelarUsuario.ImageScaling = ToolStripItemImageScaling.None;
            btnCancelarUsuario.Margin = new Padding(4, 1, 0, 2);
            btnCancelarUsuario.Name = "btnCancelarUsuario";
            btnCancelarUsuario.Size = new Size(92, 36);
            btnCancelarUsuario.Text = "Cancelar";
            btnCancelarUsuario.ToolTipText = "Descartar lo escrito";
            btnCancelarUsuario.Visible = false;
            // 
            // panelTitulo
            // 
            panelTitulo.BackColor = Color.FromArgb(190, 225, 150);
            panelTitulo.Controls.Add(lblTitulo);
            panelTitulo.Dock = DockStyle.Top;
            panelTitulo.Location = new Point(0, 35);
            panelTitulo.Name = "panelTitulo";
            panelTitulo.Padding = new Padding(0);
            panelTitulo.Size = new Size(1150, 60);
            panelTitulo.TabIndex = 0;
            // 
            // lblTitulo
            // 
            lblTitulo.Dock = DockStyle.Fill;
            lblTitulo.Font = new Font("JetBrains Mono", 14F, FontStyle.Bold);
            lblTitulo.ForeColor = Color.FromArgb(70, 140, 25);
            lblTitulo.Location = new Point(18, 0);
            lblTitulo.Name = "lblTitulo";
            lblTitulo.Size = new Size(1132, 60);
            lblTitulo.TabIndex = 0;
            lblTitulo.Text = "USUARIOS";
            lblTitulo.TextAlign = ContentAlignment.MiddleCenter;
            // 
            // FrmUsuarios
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(240, 250, 235);
            ClientSize = new Size(1150, 712);
            Controls.Add(tlpRoot);
            Controls.Add(panelTitulo);
            MinimumSize = new Size(980, 622);
            Name = "FrmUsuarios";
            Text = "Usuarios";
            ZoomScaleRect = new Rectangle(15, 15, 1150, 712);
            tlpRoot.ResumeLayout(false);
            panelIzq.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)gridUsuarios).EndInit();
            pnlBuscador.ResumeLayout(false);
            pnlResumen.ResumeLayout(false);
            panelDer.ResumeLayout(false);
            tabDetalle.ResumeLayout(false);
            tabDetalleUsuario.ResumeLayout(false);
            tlpDetalle.ResumeLayout(false);
            pnlPassword.ResumeLayout(false);
            barraHerramientas.ResumeLayout(false);
            barraHerramientas.PerformLayout();
            panelTitulo.ResumeLayout(false);
            ResumeLayout(false);
        }

        #endregion

        /// <summary>Rotulo de un campo del detalle. Estilo igual que FrmProductos.</summary>
        private void ConfigurarCaption(Sunny.UI.UILabel caption, string nombre, string texto)
        {
            caption.AutoSize = false;
            caption.Dock = DockStyle.Fill;
            caption.Font = new Font("Microsoft Sans Serif", 12F);
            caption.ForeColor = Color.FromArgb(48, 48, 48);
            caption.Name = nombre;
            caption.Padding = new Padding(2, 0, 0, 0);
            caption.Text = texto;
            caption.TextAlign = ContentAlignment.MiddleLeft;
        }

        /// <summary>
        /// Valor de solo lectura del detalle. ReadOnly no se fija aca a proposito: lo
        /// gobierna AplicarModo, que es el unico sitio que decide que se puede escribir.
        /// Nace en guion, que es lo que se ve sin fila elegida.
        /// </summary>
        private void ConfigurarValorConsulta(Sunny.UI.UITextBox valor, string nombre)
        {
            valor.Dock = DockStyle.Fill;
            valor.FillColor = Color.White;
            valor.Font = new Font("JetBrains Mono", 9F);
            valor.Margin = new Padding(3, 6, 3, 6);
            valor.Name = nombre;
            valor.Padding = new Padding(6, 0, 6, 0);
            valor.RectColor = Color.FromArgb(110, 190, 40);
            valor.Text = "—";
            valor.TextAlignment = ContentAlignment.MiddleLeft;
        }

        private TableLayoutPanel tlpRoot;
        private Panel panelIzq;
        private Sunny.UI.UIDataGridView gridUsuarios;
        private DataGridViewTextBoxColumn colUsuarioId;
        private DataGridViewTextBoxColumn colUsuario;
        private DataGridViewTextBoxColumn colNombre;
        private DataGridViewTextBoxColumn colRol;
        private DataGridViewTextBoxColumn colEstado;
        private Panel pnlBuscador;
        private Sunny.UI.UITextBox txtBuscar;
        private Panel pnlResumen;
        private Sunny.UI.UILabel lblResumen;
        private Sunny.UI.UILabel lblResumenDetalle;
        private Panel panelDer;
        private Sunny.UI.UITabControl tabDetalle;
        private TabPage tabDetalleUsuario;
        private TableLayoutPanel tlpDetalle;
        private Sunny.UI.UILabel lblDetalleTitulo;
        private Sunny.UI.UILabel lblCapId;
        private Sunny.UI.UITextBox txtDetId;
        private Sunny.UI.UILabel lblCapUsuario;
        private Sunny.UI.UITextBox txtDetUsuario;
        private Sunny.UI.UILabel lblCapNombre;
        private Sunny.UI.UITextBox txtDetNombre;
        private Sunny.UI.UILabel lblCapEmail;
            private Sunny.UI.UITextBox txtDetEmail;
            private Sunny.UI.UILabel lblCapTituloCargo;
            private Sunny.UI.UITextBox txtDetTituloCargo;
            private Sunny.UI.UILabel lblCapDepartamento;
            private Sunny.UI.UITextBox txtDetDepartamento;
            private Sunny.UI.UILabel lblCapPassword;
        private Sunny.UI.UITextBox txtPassword;
            private Panel pnlPassword;
            private Sunny.UI.UIButton btnOjoPassword;
            private Sunny.UI.UISwitch swDetActivo;
            private Sunny.UI.UIComboBox cboRoles;
            private Sunny.UI.UILabel lblCapRoles;
        private Sunny.UI.UITextBox txtDetRoles;
private Sunny.UI.UILabel lblCapActivo;
            private Sunny.UI.UILabel lblCapFechaCreacion;
            private Sunny.UI.UITextBox txtDetFechaCreacion;
            private System.Windows.Forms.Panel panelTitulo;
        private Sunny.UI.UILabel lblTitulo;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevoUsuario;
        private ToolStripButton btnEditarUsuario;
        private ToolStripButton btnExportarUsuario;
        private ToolStripButton btnReporteUsuario;
        private ToolStripButton btnGuardarUsuario;
        private ToolStripButton btnCancelarUsuario;
    }
}
