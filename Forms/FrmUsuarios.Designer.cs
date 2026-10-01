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
            panelTitulo = new System.Windows.Forms.Panel();
            lblTitulo = new Sunny.UI.UILabel();
            barraHerramientas = new ToolStrip();
            btnNuevoUsuario = new ToolStripButton();
            btnEditarUsuario = new ToolStripButton();
            tlpRoot.SuspendLayout();
            panelIzq.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)gridUsuarios).BeginInit();
            pnlBuscador.SuspendLayout();
            pnlResumen.SuspendLayout();
            panelDer.SuspendLayout();
            tabDetalle.SuspendLayout();
            panelTitulo.SuspendLayout();
            barraHerramientas.SuspendLayout();
            SuspendLayout();
            // 
            // tlpRoot: 30% listado, 70% detalle.
            // 
            tlpRoot.ColumnCount = 2;
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 30F));
            tlpRoot.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 70F));
            tlpRoot.Controls.Add(panelIzq, 0, 0);
            tlpRoot.Controls.Add(panelDer, 1, 0);
            tlpRoot.Dock = DockStyle.Fill;
            tlpRoot.Location = new Point(0, 0);
            tlpRoot.Margin = new Padding(0);
            tlpRoot.Name = "tlpRoot";
            tlpRoot.RowCount = 1;
            tlpRoot.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
            tlpRoot.Size = new Size(1150, 612);
            tlpRoot.TabIndex = 1;
            // 
            // panelIzq: buscador arriba, grid al centro, resumen abajo.
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
            panelIzq.Size = new Size(345, 652);
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
            gridUsuarios.Columns.AddRange(new DataGridViewColumn[] { colUsuarioId, colUsuario, colRol, colEstado });
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
            gridUsuarios.Size = new Size(343, 547);
            gridUsuarios.StripeOddColor = Color.FromArgb(245, 250, 240);
            gridUsuarios.TabIndex = 2;
            // 
            // colUsuarioId
            // 
            colUsuarioId.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colUsuarioId.DataPropertyName = "UserId";
            colUsuarioId.FillWeight = 15F;
            colUsuarioId.HeaderText = "ID";
            colUsuarioId.Name = "colUsuarioId";
            colUsuarioId.ReadOnly = true;
            // 
            // colUsuario
            // 
            colUsuario.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colUsuario.DataPropertyName = "Username";
            colUsuario.FillWeight = 35F;
            colUsuario.HeaderText = "Usuario";
            colUsuario.Name = "colUsuario";
            colUsuario.ReadOnly = true;
            // 
            // colRol: Usuario.Roles es List<Role>, que una celda no puede pintar.
            // El binding es contra la columna "Rol" del DataTable de presentacion que
            // arma ConstruirDataTable, no contra Usuario.Roles.
            // 
            colRol.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colRol.DataPropertyName = "Rol";
            colRol.FillWeight = 30F;
            colRol.HeaderText = "Rol";
            colRol.Name = "colRol";
            colRol.ReadOnly = true;
            // 
            // colEstado: se llena en codigo con Activo / Inactivo desde el bool Activo,
            // para que la celda no muestre True / False.
            // 
            colEstado.AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill;
            colEstado.DataPropertyName = "Estado";
            colEstado.FillWeight = 20F;
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
            txtBuscar.Watermark = "Buscar por usuario o correo...";
            // 
            // pnlResumen
            // 
            pnlResumen.BackColor = Color.FromArgb(110, 190, 40);
            pnlResumen.Controls.Add(lblResumenDetalle);
            pnlResumen.Controls.Add(lblResumen);
            pnlResumen.Dock = DockStyle.Bottom;
            pnlResumen.Location = new Point(0, 591);
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
            // La barra se agrega despues que el tab a proposito: el docking se resuelve
            // en orden inverso, asi queda Dock=Top arriba del detalle y no superpuesta.
            panelDer.Controls.Add(barraHerramientas);
            panelDer.Dock = DockStyle.Fill;
            panelDer.Location = new Point(348, 3);
            panelDer.Name = "panelDer";
            panelDer.Padding = new Padding(8);
            panelDer.Size = new Size(799, 646);
            panelDer.TabIndex = 1;
            // 
            // tabDetalle
            // 
            tabDetalle.Controls.Add(tabDetalleUsuario);
            tabDetalle.Dock = DockStyle.Fill;
            tabDetalle.DrawMode = TabDrawMode.OwnerDrawFixed;
            tabDetalle.Font = new Font("Microsoft Sans Serif", 9F);
            tabDetalle.ItemSize = new Size(160, 32);
            tabDetalle.Location = new Point(8, 8);
            tabDetalle.MainPage = "";
            tabDetalle.Name = "tabDetalle";
            tabDetalle.SelectedIndex = 0;
            tabDetalle.Size = new Size(783, 630);
            tabDetalle.SizeMode = TabSizeMode.Fixed;
            tabDetalle.TabIndex = 0;
            tabDetalle.TabUnSelectedForeColor = Color.FromArgb(240, 240, 240);
            tabDetalle.TipsFont = new Font("Microsoft Sans Serif", 9F);
            // 
            // tabDetalleUsuario: la pagina de detalle se define en el proximo paso.
            // 
            tabDetalleUsuario.BackColor = Color.White;
            tabDetalleUsuario.Location = new Point(0, 32);
            tabDetalleUsuario.Name = "tabDetalleUsuario";
            tabDetalleUsuario.Size = new Size(783, 598);
            tabDetalleUsuario.TabIndex = 0;
            tabDetalleUsuario.Text = "Detalle";
            // 
            // panelTitulo: banner de titulo a lo ancho del form.
            // 
            panelTitulo.BackColor = Color.FromArgb(190, 225, 150);
            panelTitulo.Controls.Add(lblTitulo);
            panelTitulo.Dock = DockStyle.Top;
            panelTitulo.Location = new Point(0, 0);
            panelTitulo.Name = "panelTitulo";
            panelTitulo.Padding = new Padding(18, 0, 0, 0);
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
            lblTitulo.TextAlign = ContentAlignment.MiddleLeft;
            // 
            // barraHerramientas: acciones de Nuevo y Editar. Va a nivel de la pagina de
            // detalle: Dock=Top dentro de panelDer, arriba del tab. Mismo estilo y
            // mismo criterio que barraHerramientas de FrmProductos.
            // 
            barraHerramientas.AutoSize = false;
            barraHerramientas.BackColor = Color.FromArgb(225, 225, 225);
            barraHerramientas.Font = new Font("Microsoft Sans Serif", 10F, FontStyle.Bold);
            barraHerramientas.ForeColor = Color.FromArgb(80, 80, 80);
            barraHerramientas.GripStyle = ToolStripGripStyle.Hidden;
            barraHerramientas.Items.AddRange(new ToolStripItem[] { btnNuevoUsuario, btnEditarUsuario });
            barraHerramientas.Location = new Point(0, 0);
            barraHerramientas.Name = "barraHerramientas";
            barraHerramientas.Padding = new Padding(4, 2, 0, 2);
            barraHerramientas.RenderMode = ToolStripRenderMode.Professional;
            barraHerramientas.Size = new Size(1150, 40);
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
            // btnEditarUsuario: deshabilitado hasta que haya una fila seleccionada.
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
            // FrmUsuarios
            // 
            AutoScaleMode = AutoScaleMode.None;
            BackColor = Color.FromArgb(240, 250, 235);
            ClientSize = new Size(1150, 712);
            // El orden de Controls.Add fija el orden de las bandas de arriba hacia abajo:
            // el docking se resuelve en orden inverso de agregacion, asi que el ultimo
            // agregado es el que queda mas arriba. Banner, y despues el 30/70. La barra
            // de herramientas NO va aqui: cuelga de panelDer, a nivel de la pagina de
            // detalle.
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
            panelTitulo.ResumeLayout(false);
            barraHerramientas.ResumeLayout(false);
            barraHerramientas.PerformLayout();
            ResumeLayout(false);
        }

        #endregion

        private TableLayoutPanel tlpRoot;
        private Panel panelIzq;
        private Sunny.UI.UIDataGridView gridUsuarios;
        private DataGridViewTextBoxColumn colUsuarioId;
        private DataGridViewTextBoxColumn colUsuario;
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
        private System.Windows.Forms.Panel panelTitulo;
        private Sunny.UI.UILabel lblTitulo;
        private ToolStrip barraHerramientas;
        private ToolStripButton btnNuevoUsuario;
        private ToolStripButton btnEditarUsuario;
    }
}
