namespace Ritrama2025.Forms;

partial class FrmLogViewer
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
        this.dtpFechaInicio = new Sunny.UI.UIDatePicker();
        this.dtpFechaFin = new Sunny.UI.UIDatePicker();
        this.btnBuscar = new Sunny.UI.UIButton();
        this.btnResumen = new Sunny.UI.UIButton();
        this.btnPorOC = new Sunny.UI.UIButton();
        this.txtNumeroOC = new Sunny.UI.UITextBox();
        this.btnExportar = new Sunny.UI.UIButton();
        this.gridLogs = new Sunny.UI.UIDataGridView();
        this.gridResumen = new Sunny.UI.UIDataGridView();
        this.gridDetalles = new Sunny.UI.UIDataGridView();
        this.txtDetalle = new System.Windows.Forms.TextBox();
        this.lblFechaInicio = new System.Windows.Forms.Label();
        this.lblFechaFin = new System.Windows.Forms.Label();
        this.lblNumeroOC = new System.Windows.Forms.Label();
        this.splitContainer1 = new System.Windows.Forms.SplitContainer();
        this.splitContainer2 = new System.Windows.Forms.SplitContainer();
        this.tabControl1 = new System.Windows.Forms.TabControl();
        this.tabLogs = new System.Windows.Forms.TabPage();
        this.tabResumen = new System.Windows.Forms.TabPage();
        this.tabDetalle = new System.Windows.Forms.TabPage();

        ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.gridResumen)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.gridDetalles)).BeginInit();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).BeginInit();
        this.splitContainer1.SuspendLayout();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).BeginInit();
        this.splitContainer2.SuspendLayout();
        this.tabControl1.SuspendLayout();
        this.tabLogs.SuspendLayout();
        this.tabResumen.SuspendLayout();
        this.tabDetalle.SuspendLayout();
        this.SuspendLayout();

        // 
        // Panel superior (controles)
        // 
        this.lblFechaInicio.AutoSize = true;
        this.lblFechaInicio.Location = new System.Drawing.Point(12, 15);
        this.lblFechaInicio.Text = "Fecha Inicio:";

        this.dtpFechaInicio.Location = new System.Drawing.Point(100, 10);
        this.dtpFechaInicio.Size = new System.Drawing.Size(200, 30);

        this.lblFechaFin.AutoSize = true;
        this.lblFechaFin.Location = new System.Drawing.Point(320, 15);
        this.lblFechaFin.Text = "Fecha Fin:";

        this.dtpFechaFin.Location = new System.Drawing.Point(400, 10);
        this.dtpFechaFin.Size = new System.Drawing.Size(200, 30);

        this.btnBuscar.Text = "Buscar";
        this.btnBuscar.Location = new System.Drawing.Point(620, 10);
        this.btnBuscar.Size = new System.Drawing.Size(100, 30);
        this.btnBuscar.Click += new System.EventHandler(this.btnBuscar_Click);

        this.btnResumen.Text = "Resumen";
        this.btnResumen.Location = new System.Drawing.Point(730, 10);
        this.btnResumen.Size = new System.Drawing.Size(100, 30);
        this.btnResumen.Click += new System.EventHandler(this.btnResumen_Click);

        this.btnExportar.Text = "Exportar CSV";
        this.btnExportar.Location = new System.Drawing.Point(840, 10);
        this.btnExportar.Size = new System.Drawing.Size(120, 30);
        this.btnExportar.Click += new System.EventHandler(this.btnExportar_Click);

        this.lblNumeroOC.AutoSize = true;
        this.lblNumeroOC.Location = new System.Drawing.Point(12, 55);
        this.lblNumeroOC.Text = "Nº OC:";

        this.txtNumeroOC.Location = new System.Drawing.Point(100, 50);
        this.txtNumeroOC.Size = new System.Drawing.Size(150, 30);

        this.btnPorOC.Text = "Buscar por OC";
        this.btnPorOC.Location = new System.Drawing.Point(270, 50);
        this.btnPorOC.Size = new System.Drawing.Size(130, 30);
        this.btnPorOC.Click += new System.EventHandler(this.btnPorOC_Click);

        // 
        // splitContainer1 (arriba: controles, abajo: tabs)
        // 
        this.splitContainer1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer1.FixedPanel = System.Windows.Forms.FixedPanel.Panel1;
        this.splitContainer1.Location = new System.Drawing.Point(0, 35);
        this.splitContainer1.SplitterDistance = 90;
        this.splitContainer1.Panel1MinSize = 90;

        // Panel1: controles superiores
        this.splitContainer1.Panel1.Controls.Add(this.dtpFechaInicio);
        this.splitContainer1.Panel1.Controls.Add(this.dtpFechaFin);
        this.splitContainer1.Panel1.Controls.Add(this.btnBuscar);
        this.splitContainer1.Panel1.Controls.Add(this.btnResumen);
        this.splitContainer1.Panel1.Controls.Add(this.btnExportar);
        this.splitContainer1.Panel1.Controls.Add(this.txtNumeroOC);
        this.splitContainer1.Panel1.Controls.Add(this.btnPorOC);
        this.splitContainer1.Panel1.Controls.Add(this.lblFechaInicio);
        this.splitContainer1.Panel1.Controls.Add(this.lblFechaFin);
        this.splitContainer1.Panel1.Controls.Add(this.lblNumeroOC);

        // Panel2: tabs
        this.splitContainer1.Panel2.Controls.Add(this.tabControl1);

        // 
        // tabControl1
        // 
        this.tabControl1.Dock = System.Windows.Forms.DockStyle.Fill;
        this.tabControl1.TabPages.Add(this.tabLogs);
        this.tabControl1.TabPages.Add(this.tabResumen);
        this.tabControl1.TabPages.Add(this.tabDetalle);

        // 
        // tabLogs
        // 
        this.tabLogs.Controls.Add(this.gridLogs);

        // 
        // gridLogs
        // 
        this.gridLogs.Dock = System.Windows.Forms.DockStyle.Fill;
        this.gridLogs.ReadOnly = true;
        this.gridLogs.AllowUserToAddRows = false;
        this.gridLogs.AllowUserToDeleteRows = false;
        this.gridLogs.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.AllCells;
        this.gridLogs.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.gridLogs_CellDoubleClick);

        // 
        // tabResumen
        // 
        this.tabResumen.Controls.Add(this.gridResumen);

        // 
        // gridResumen
        // 
        this.gridResumen.Dock = System.Windows.Forms.DockStyle.Fill;
        this.gridResumen.ReadOnly = true;
        this.gridResumen.AllowUserToAddRows = false;
        this.gridResumen.AllowUserToDeleteRows = false;

        // 
        // tabDetalle
        // 
        this.tabDetalle.Controls.Add(this.splitContainer2);

        // 
        // splitContainer2 (arriba: grid detalles, abajo: texto)
        // 
        this.splitContainer2.Dock = System.Windows.Forms.DockStyle.Fill;
        this.splitContainer2.SplitterDistance = 200;
        this.splitContainer2.Orientation = System.Windows.Forms.Orientation.Horizontal;

        this.splitContainer2.Panel1.Controls.Add(this.gridDetalles);
        this.splitContainer2.Panel2.Controls.Add(this.txtDetalle);

        // 
        // gridDetalles
        // 
        this.gridDetalles.Dock = System.Windows.Forms.DockStyle.Fill;
        this.gridDetalles.ReadOnly = true;
        this.gridDetalles.AllowUserToAddRows = false;
        this.gridDetalles.AllowUserToDeleteRows = false;

        // 
        // txtDetalle
        // 
        this.txtDetalle.Dock = System.Windows.Forms.DockStyle.Fill;
        this.txtDetalle.Font = new System.Drawing.Font("Consolas", 10F);
        this.txtDetalle.Multiline = true;
        this.txtDetalle.ReadOnly = true;
        this.txtDetalle.ScrollBars = System.Windows.Forms.ScrollBars.Both;

        // 
        // FrmLogViewer
        // 
        this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
        this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
        this.ClientSize = new System.Drawing.Size(1000, 600);
        this.Controls.Add(this.splitContainer1);
        this.Name = "FrmLogViewer";
        this.Text = "Historial de Operaciones - Log Viewer";
        this.Load += new System.EventHandler(this.FrmLogViewer_Load);

        ((System.ComponentModel.ISupportInitialize)(this.gridLogs)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.gridResumen)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.gridDetalles)).EndInit();
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer1)).EndInit();
        this.splitContainer1.ResumeLayout(false);
        ((System.ComponentModel.ISupportInitialize)(this.splitContainer2)).EndInit();
        this.splitContainer2.ResumeLayout(false);
        this.tabControl1.ResumeLayout(false);
        this.tabLogs.ResumeLayout(false);
        this.tabResumen.ResumeLayout(false);
        this.tabDetalle.ResumeLayout(false);
        this.ResumeLayout(false);
    }

    private Sunny.UI.UIDatePicker dtpFechaInicio;
    private Sunny.UI.UIDatePicker dtpFechaFin;
    private Sunny.UI.UIButton btnBuscar;
    private Sunny.UI.UIButton btnResumen;
    private Sunny.UI.UIButton btnPorOC;
    private Sunny.UI.UITextBox txtNumeroOC;
    private Sunny.UI.UIButton btnExportar;
    private Sunny.UI.UIDataGridView gridLogs;
    private Sunny.UI.UIDataGridView gridResumen;
    private Sunny.UI.UIDataGridView gridDetalles;
    private System.Windows.Forms.TextBox txtDetalle;
    private System.Windows.Forms.Label lblFechaInicio;
    private System.Windows.Forms.Label lblFechaFin;
    private System.Windows.Forms.Label lblNumeroOC;
    private System.Windows.Forms.SplitContainer splitContainer1;
    private System.Windows.Forms.SplitContainer splitContainer2;
    private System.Windows.Forms.TabControl tabControl1;
    private System.Windows.Forms.TabPage tabLogs;
    private System.Windows.Forms.TabPage tabResumen;
    private System.Windows.Forms.TabPage tabDetalle;
}
