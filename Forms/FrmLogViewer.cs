using System.Data;
using Ritrama2025.Services.ProduccionService;
using Sunny.UI;

namespace Ritrama2025.Forms;

public partial class FrmLogViewer : UIForm
{
    private readonly ILogViewerService _logService;

    public FrmLogViewer(ILogViewerService logService)
    {
        InitializeComponent();
        _logService = logService;
    }

    private async void FrmLogViewer_Load(object sender, EventArgs e)
    {
        dtpFechaInicio.Value = DateTime.Today.AddDays(-1);
        dtpFechaFin.Value = DateTime.Now;
        await CargarResumenAsync();
    }

    private async void btnBuscar_Click(object sender, EventArgs e)
    {
        await CargarLogsAsync();
    }

    private async void btnResumen_Click(object sender, EventArgs e)
    {
        await CargarResumenAsync();
    }

    private async void btnPorOC_Click(object sender, EventArgs e)
    {
        string numeroOC = txtNumeroOC.Text.Trim();
        if (string.IsNullOrEmpty(numeroOC))
        {
            MessageBox.Show("Ingrese el número de OC", "Error", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            return;
        }

        DataTable dt = await _logService.ObtenerLogsPorOCAsync(numeroOC);
        gridLogs.DataSource = dt;
        AjustarColumnas();
    }

    private async void gridLogs_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
    {
        if (e.RowIndex < 0)
        {
            return;
        }

        if (gridLogs.DataSource is DataTable dt && dt.Rows.Count > e.RowIndex)
        {
            long operacionId = Convert.ToInt64(dt.Rows[e.RowIndex]["id"]);
            await MostrarDetalleAsync(operacionId);
        }
    }

    private async void btnExportar_Click(object sender, EventArgs e)
    {
        if (gridLogs.DataSource is DataTable dt && dt.Rows.Count > 0)
        {
            using SaveFileDialog sfd = new();
            sfd.Filter = "CSV files (*.csv)|*.csv|Text files (*.txt)|*.txt";
            sfd.FileName = $"log_operaciones_{DateTime.Now:yyyyMMdd_HHmmss}.csv";

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                ExportarACsv(dt, sfd.FileName);
                MessageBox.Show($"Log exportado a:\n{sfd.FileName}", "Exportar", MessageBoxButtons.OK, MessageBoxIcon.Information);
            }
        }
    }

    private async Task CargarLogsAsync()
    {
        DataTable dt = await _logService.ObtenerLogsAsync(dtpFechaInicio.Value, dtpFechaFin.Value, null);
        gridLogs.DataSource = dt;
        AjustarColumnas();
    }

    private async Task CargarResumenAsync()
    {
        DataTable dt = await _logService.ObtenerResumenDiarioAsync();
        gridResumen.DataSource = dt;
    }

    private async Task MostrarDetalleAsync(long operacionId)
    {
        string reporte = await _logService.GenerarReporteTextoAsync(operacionId);
        txtDetalle.Text = reporte;

        DataTable dtDetalles = await _logService.ObtenerDetallesLogAsync(operacionId);
        gridDetalles.DataSource = dtDetalles;
    }

    private void AjustarColumnas()
    {
        foreach (DataGridViewColumn col in gridLogs.Columns)
        {
            col.AutoSizeMode = DataGridViewAutoSizeColumnMode.AllCells;
        }
    }

    private void ExportarACsv(DataTable dt, string ruta)
    {
        using StreamWriter sw = new(ruta, false, System.Text.Encoding.UTF8);

        // Encabezados
        IEnumerable<string> columnas = dt.Columns.Cast<DataColumn>().Select(c => c.ColumnName);
        sw.WriteLine(string.Join(",", columnas));

        // Datos
        foreach (DataRow row in dt.Rows)
        {
            IEnumerable<string> valores = row.ItemArray.Select(v =>
            {
                string val = v?.ToString() ?? "";
                if (val.Contains(",") || val.Contains("\"") || val.Contains("\n"))
                {
                    val = "\"" + val.Replace("\"", "\"\"") + "\"";
                }

                return val;
            });
            sw.WriteLine(string.Join(",", valores));
        }
    }
}
