namespace Ritrama2025.Forms;

partial class FrmAuditoriaInconsistencias
{
    private System.ComponentModel.IContainer components = null;

    protected override void Dispose(bool disposing)
    {
        if (disposing && (components != null))
        {
            components.Dispose();
        }
        base.Dispose(disposing);
    }

    private void InitializeComponent()
    {
        this.btnRedetectar = new Sunny.UI.UIButton();
        this.btnCorregir = new Sunny.UI.UIButton();
        this.btnMarcarRevisado = new Sunny.UI.UIButton();
        this.btnExportar = new Sunny.UI.UIButton();
        this.cmbOrigen = new Sunny.UI.UIComboBox();
        this.chkPendientes = new Sunny.UI.UICheckBox();
        this.lblContador = new Sunny.UI.UILabel();
        this.lblOrigen = new Sunny.UI.UILabel();
        this.gridHallazgos = new Sunny.UI.UIDataGridView();
        this.colId = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colFecha = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colOrigen = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colOC = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colCodigo = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colDescripcion = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colSeveridad = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colEstado = new System.Windows.Forms.DataGridViewTextBoxColumn();
        this.colManual = new System.Windows.Forms.DataGridViewCheckBoxColumn();
        this.txtDetalle = new Sunny.UI.UITextBox();
        this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        this.splitContainer2 = new System.Windows.Forms.SplitContainer();

        ((System.ComponentModel.ISupportInitialize)(this.gridHallazgos)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
        this.splitContainer1.Panel1.SuspendLayout();
        this.splitContainer1.Panel2.SuspendLayout();
        this.splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
        this.splitContainer2.Panel1.SuspendLayout();
        this.splitContainer2.Panel2.SuspendLayout();
        this.splitContainer2.SuspendLayout();
        this.SuspendLayout();

        //
        // btnRedetectar
        //
        this.btnRedetectar.Text = "Re-detectar";
        this.btnRedetectar.Location = new System.Drawing.Point(12, 10);
        this.btnRedetectar.Size = new System.Drawing.Size(130, 30);
        this.btnRedetectar.Click += new System.EventHandler(this.btnRedetectar_Click);

        //
        // btnCorregir
        //
        this.btnCorregir.Text = "Corregir automáticas";
        this.btnCorregir.Location = new System.Drawing.Point(152, 10);
        this.btnCorregir.Size = new System.Drawing.Size(170, 30);
        this.btnCorregir.Click += new System.EventHandler(this.btnCorregir_Click);

        //
        // btnMarcarRevisado
        //
        this.btnMarcarRevisado.Text = "Marcar revisado";
        this.btnMarcarRevisado.Location = new System.Drawing.Point(332, 10);
        this.btnMarcarRevisado.Size = new System.Drawing.Size(150, 30);
        this.btnMarcarRevisado.Click += new System.EventHandler(this.btnMarcarRevisado_Click);

        //
        // btnExportar
        //
        this.btnExportar.Text = "Exportar CSV";
        this.btnExportar.Location = new System.Drawing.Point(492, 10);
        this.btnExportar.Size = new System.Drawing.Size(130, 30);
        this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);

        //
        // lblOrigen
        //
        this.lblOrigen.AutoSize = true;
        this.lblOrigen.Location = new System.Drawing.Point(12, 55);
        this.lblOrigen.Text = "Origen:";

        //
        // cmbOrigen
        //
        this.cmbOrigen.DropDownStyle = Sunny.UI.UIDropDownStyle.DropDownList;
        this.cmbOrigen.Location = new System.Drawing.Point(70, 50);
        this.cmbOrigen.MinimumSize = new System.Drawing.Size(63, 0);
        this.cmbOrigen.Name = "cmbOrigen";
        this.cmbOrigen.Size = new System.Drawing.Size(200, 30);
        this.cmbOrigen.SelectedIndexChanged += new System.EventHandler(this.FiltroChanged);

        //
        // chkPendientes
        //
        this.chkPendientes.Checked = true;
        this.chkPendientes.Location = new System.Drawing.Point(290, 52);
        this.chkPendientes.Name = "chkPendientes";
        this.chkPendientes.Size = new System.Drawing.Size(150, 28);
        this.chkPendientes.Text = "Solo pendientes";
        this.chkPendientes.CheckedChanged += new System.EventHandler(this.FiltroChanged);

        //
        // lblContador
        //
        this.lblContador.AutoSize = true;
        this.lblContador.Location = new System.Drawing.Point(440, 56);
        this.lblContador.Text = "0 pendientes";

        //
        // splitContainer1 (arriba: controles, abajo: grid + detalle)
        //
        this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        this.splitContainer1.Location = new System.Drawing.Point(0, 35);
        this.splitContainer1.SplitterDistance = 90;
        this.splitContainer1.Panel1MinSize = 90;

        this.splitContainer1.Panel1.Controls.Add(this.btnRedetectar);
        this.splitContainer1.Panel1.Controls.Add(this.btnCorregir);
        this.splitContainer1.Panel1.Controls.Add(this.btnMarcarRevisado);
        this.splitContainer1.Panel1.Controls.Add(this.btnExportar);
        this.splitContainer1.Panel1.Controls.Add(this.lblOrigen);
        this.splitContainer1.Panel1.Controls.Add(this.cmbOrigen);
        this.splitContainer1.Panel1.Controls.Add(this.chkPendientes);
        this.splitContainer1.Panel1.Controls.Add(this.lblContador);

        this.splitContainer1.Panel2.Controls.Add(this.splitContainer2);

        //
        // splitContainer2 (arriba: grid, abajo: detalle)
        //
        this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer2.SplitterDistance = 380;
        this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;

        this.splitContainer2.Panel1.Controls.Add(this.gridHallazgos);
        this.splitContainer2.Panel2.Controls.Add(this.txtDetalle);

        //
        // gridHallazgos
        //
        this.gridHallazgos.Dock = System.Windows.Forms.DockStyle.Fill;
        this.gridHallazgos.ReadOnly = true;
        this.gridHallazgos.AllowUserToAddRows = false;
        this.gridHallazgos.AllowUserToDeleteRows = false;
        this.gridHallazgos.AutoGenerateColumns = false;
        this.gridHallazgos.SelectionMode = System.Windows.Forms.DataGridViewSelectionMode.FullRowSelect;
        this.gridHallazgos.MultiSelect = true;
        this.gridHallazgos.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        this.gridHallazgos.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.colId,
            this.colFecha,
            this.colOrigen,
            this.colOC,
            this.colCodigo,
            this.colDescripcion,
            this.colSeveridad,
            this.colEstado,
            this.colManual });
        this.gridHallazgos.SelectionChanged += new System.EventHandler(this.gridHallazgos_SelectionChanged);
        this.gridHallazgos.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridHallazgos_CellDoubleClick);

        //
        // colId
        //
        this.colId.DataPropertyName = "Id";
        this.colId.HeaderText = "Id";
        this.colId.Name = "colId";
        this.colId.Visible = false;

        //
        // colFecha
        //
        this.colFecha.DataPropertyName = "Fecha";
        this.colFecha.HeaderText = "Fecha";
        this.colFecha.Name = "colFecha";

        //
        // colOrigen
        //
        this.colOrigen.DataPropertyName = "Origen";
        this.colOrigen.HeaderText = "Origen";
        this.colOrigen.Name = "colOrigen";

        //
        // colOC
        //
        this.colOC.DataPropertyName = "OC";
        this.colOC.HeaderText = "OC";
        this.colOC.Name = "colOC";

        //
        // colCodigo
        //
        this.colCodigo.DataPropertyName = "Codigo";
        this.colCodigo.HeaderText = "Código";
        this.colCodigo.Name = "colCodigo";

        //
        // colDescripcion
        //
        this.colDescripcion.DataPropertyName = "Descripcion";
        this.colDescripcion.HeaderText = "Descripción";
        this.colDescripcion.Name = "colDescripcion";
        this.colDescripcion.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;

        //
        // colSeveridad
        //
        this.colSeveridad.DataPropertyName = "Severidad";
        this.colSeveridad.HeaderText = "Severidad";
        this.colSeveridad.Name = "colSeveridad";

        //
        // colEstado
        //
        this.colEstado.DataPropertyName = "Estado";
        this.colEstado.HeaderText = "Estado";
        this.colEstado.Name = "colEstado";

        //
        // colManual
        //
        this.colManual.DataPropertyName = "Manual";
        this.colManual.HeaderText = "Manual";
        this.colManual.Name = "colManual";
        this.colManual.ReadOnly = true;

        //
        // txtDetalle
        //
        this.txtDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtDetalle.Font = new System.Drawing.Font("Consolas", 10F);
        this.txtDetalle.Multiline = true;
        this.txtDetalle.ReadOnly = true;
        this.txtDetalle.ShowScrollBar = true;

        //
        // FrmAuditoriaInconsistencias
        //
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1100, 650);
        this.Controls.Add(this.splitContainer1);
        this.Name = "FrmAuditoriaInconsistencias";
        this.Text = "Auditoría de inconsistencias y validaciones";
        this.Load += new System.EventHandler(this.FrmAuditoriaInconsistencias_Load);

        ((System.ComponentModel.ISupportInitialize)(this.gridHallazgos)).EndInit();
        this.splitContainer1.Panel1.ResumeLayout(false);
        this.splitContainer1.Panel1.PerformLayout();
        this.splitContainer1.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
        this.splitContainer1.ResumeLayout(false);
        this.splitContainer2.Panel1.ResumeLayout(false);
        this.splitContainer2.Panel2.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
        this.splitContainer2.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private Sunny.UI.UIButton btnRedetectar;
    private Sunny.UI.UIButton btnCorregir;
    private Sunny.UI.UIButton btnMarcarRevisado;
    private Sunny.UI.UIButton btnExportar;
    private Sunny.UI.UIComboBox cmbOrigen;
    private Sunny.UI.UICheckBox chkPendientes;
    private Sunny.UI.UILabel lblContador;
    private Sunny.UI.UILabel lblOrigen;
    private Sunny.UI.UIDataGridView gridHallazgos;
    private System.Windows.Forms.DataGridViewTextBoxColumn colId;
    private System.Windows.Forms.DataGridViewTextBoxColumn colFecha;
    private System.Windows.Forms.DataGridViewTextBoxColumn colOrigen;
    private System.Windows.Forms.DataGridViewTextBoxColumn colOC;
    private System.Windows.Forms.DataGridViewTextBoxColumn colCodigo;
    private System.Windows.Forms.DataGridViewTextBoxColumn colDescripcion;
    private System.Windows.Forms.DataGridViewTextBoxColumn colSeveridad;
    private System.Windows.Forms.DataGridViewTextBoxColumn colEstado;
    private System.Windows.Forms.DataGridViewCheckBoxColumn colManual;
    private Sunny.UI.UITextBox txtDetalle;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
}
