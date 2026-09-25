using System.Data;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using Sunny.UI;

namespace Ritrama2025.Forms;

public partial class FrmAuditoriaInconsistencias : UIForm
{
    private readonly IAuditoriaStore _store;
    private readonly IReconciliacionService _reconciliacion;
    private List<HallazgoAuditoria> _vista = [];
    private string _mensajeEstado = string.Empty;

    private static readonly string[] OrigenesFiltro =
    [
        "Todas", "Reconciliación", "Validación documento", "Generar rollos",
        "Montar master", "Etiquetado", "Cierre", "Aprobación", "Guardado"
    ];

    public FrmAuditoriaInconsistencias(IAuditoriaStore store, IReconciliacionService reconciliacion)
    {
        InitializeComponent();
        _store = store;
        _reconciliacion = reconciliacion;
        cmbOrigen.Items.AddRange(OrigenesFiltro);
        cmbOrigen.SelectedIndex = 0;
        _store.Changed += StoreChanged;
        FormClosed += (_, _) => _store.Changed -= StoreChanged;
    }

    private void StoreChanged(object? sender, EventArgs e)
    {
        if (IsDisposed)
        {
            return;
        }

        if (InvokeRequired)
        {
            BeginInvoke(RefrescarGrid);
        }
        else
        {
            RefrescarGrid();
        }
    }

    private void FrmAuditoriaInconsistencias_Load(object sender, EventArgs e)
    {
        RefrescarGrid();
    }

    private void FiltroChanged(object? sender, EventArgs e) => RefrescarGrid();

    private static string NombreOrigen(OrigenHallazgo o) => o switch
    {
        OrigenHallazgo.Reconciliacion => "Reconciliación",
        OrigenHallazgo.ValidacionDocumento => "Validación documento",
        OrigenHallazgo.GenerarRollos => "Generar rollos",
        OrigenHallazgo.MontarMaster => "Montar master",
        OrigenHallazgo.Etiquetado => "Etiquetado",
        OrigenHallazgo.Cierre => "Cierre",
        OrigenHallazgo.Aprobacion => "Aprobación",
        OrigenHallazgo.Guardado => "Guardado",
        _ => o.ToString()
    };

    private bool PasaFiltro(HallazgoAuditoria h)
    {
        if (chkPendientes.Checked && h.Estado != EstadoHallazgo.Pendiente)
        {
            return false;
        }

        return cmbOrigen.SelectedIndex switch
        {
            1 => h.Origen == OrigenHallazgo.Reconciliacion,
            2 => h.Origen == OrigenHallazgo.ValidacionDocumento,
            3 => h.Origen == OrigenHallazgo.GenerarRollos,
            4 => h.Origen == OrigenHallazgo.MontarMaster,
            5 => h.Origen == OrigenHallazgo.Etiquetado,
            6 => h.Origen == OrigenHallazgo.Cierre,
            7 => h.Origen == OrigenHallazgo.Aprobacion,
            8 => h.Origen == OrigenHallazgo.Guardado,
            _ => true
        };
    }

    private void RefrescarGrid()
    {
        if (IsDisposed)
        {
            return;
        }

        _vista = _store.Hallazgos
            .Where(PasaFiltro)
            .OrderByDescending(h => h.Fecha)
            .ToList();

        gridHallazgos.DataSource = _vista.Select(h => new FilaHallazgo
        {
            Id = h.Id,
            Fecha = h.Fecha,
            Origen = NombreOrigen(h.Origen),
            OC = h.OC,
            Codigo = h.Codigo,
            Descripcion = h.Descripcion,
            Severidad = h.Severidad.ToString(),
            Estado = h.Estado.ToString(),
            Manual = h.RequiereAccionManual
        }).ToList();

        int pendientes = _store.PendientesCount;
        lblContador.Text = string.IsNullOrEmpty(_mensajeEstado)
            ? $"{pendientes} pendiente(s)"
            : $"{pendientes} pendiente(s) · {_mensajeEstado}";
        MostrarDetalle();
    }

    private HallazgoAuditoria? HallazgoSeleccionado()
    {
        if (gridHallazgos.CurrentRow?.DataBoundItem is FilaHallazgo fila)
        {
            return _vista.FirstOrDefault(h => h.Id == fila.Id);
        }

        return null;
    }

    private void gridHallazgos_SelectionChanged(object? sender, EventArgs e) => MostrarDetalle();

    private void gridHallazgos_CellDoubleClick(object? sender, DataGridViewCellEventArgs e) => MostrarDetalle();

    private void MostrarDetalle()
    {
        HallazgoAuditoria? h = HallazgoSeleccionado();
        if (h == null)
        {
            HallazgoAuditoria? primero = _vista.FirstOrDefault();
            txtDetalle.Text = primero == null
                ? "Sin hallazgos. Use Re-detectar para buscar inconsistencias en la base de datos."
                : TextoDetalle(primero);
            return;
        }
        txtDetalle.Text = TextoDetalle(h);
    }

    private static string TextoDetalle(HallazgoAuditoria h)
    {
        return $"Fecha: {h.Fecha:yyyy-MM-dd HH:mm}" + Environment.NewLine +
               $"Origen: {NombreOrigen(h.Origen)}" + Environment.NewLine +
               $"OC: {h.OC}" +
               (string.IsNullOrWhiteSpace(h.Rollid) ? "" : $"   Master: {h.Rollid}") + Environment.NewLine +
               $"Código: {h.Codigo}" + Environment.NewLine +
               $"Severidad: {h.Severidad}   Estado: {h.Estado}" +
               (h.RequiereAccionManual ? "   (requiere acción manual)" : "") + Environment.NewLine +
               (h.FechaRevision.HasValue ? $"Revisado: {h.FechaRevision:yyyy-MM-dd HH:mm} por {h.RevisadoPor}" + Environment.NewLine : "") +
               Environment.NewLine + "Descripción:" + Environment.NewLine + h.Descripcion +
               (string.IsNullOrWhiteSpace(h.AccionSugerida) ? "" : Environment.NewLine + Environment.NewLine +
                "Acción sugerida:" + Environment.NewLine + h.AccionSugerida);
    }

    private async void btnRedetectar_Click(object sender, EventArgs e)
    {
        btnRedetectar.Enabled = false;
        try
        {
            List<InconsistenciaOC> incs = await _reconciliacion.DetectarInconsistenciasAsync();
            _store.SincronizarReconciliacion(incs);
            _mensajeEstado = $"re-detectado {DateTime.Now:HH:mm} ({incs.Count} en BD)";
        }
        catch (Exception ex)
        {
            _mensajeEstado = "error al re-detectar: " + ex.Message;
        }
        finally
        {
            btnRedetectar.Enabled = true;
            RefrescarGrid();
        }
    }

    private async void btnCorregir_Click(object sender, EventArgs e)
    {
        List<InconsistenciaOC> automaticas = _store.TomarAutomaticasPendientes();
        if (automaticas.Count == 0)
        {
            _mensajeEstado = "no hay automáticas pendientes";
            RefrescarGrid();
            return;
        }
        btnCorregir.Enabled = false;
        try
        {
            int corregidas = await _reconciliacion.CorregirInconsistenciasAsync(automaticas);
            List<InconsistenciaOC> incs = await _reconciliacion.DetectarInconsistenciasAsync();
            _store.SincronizarReconciliacion(incs);
            _mensajeEstado = $"corregidas {corregidas} de {automaticas.Count}";
        }
        catch (Exception ex)
        {
            _mensajeEstado = "error al corregir: " + ex.Message;
        }
        finally
        {
            btnCorregir.Enabled = true;
            RefrescarGrid();
        }
    }

    private void btnMarcarRevisado_Click(object sender, EventArgs e)
    {
        List<Guid> ids = gridHallazgos.SelectedRows
            .OfType<DataGridViewRow>()
            .Select(r => (r.DataBoundItem as FilaHallazgo)?.Id)
            .Where(id => id.HasValue)
            .Select(id => id!.Value)
            .ToList();
        if (ids.Count == 0)
        {
            _mensajeEstado = "seleccione filas para marcar";
            RefrescarGrid();
            return;
        }
        string usuario = SesionActual.Usuario?.Username ?? Environment.UserName;
        _store.MarcarRevisados(ids, usuario);
        _mensajeEstado = $"{ids.Count} marcada(s) por {usuario}";
        RefrescarGrid();
    }

    private void btnExportar_Click(object sender, EventArgs e)
    {
        if (_vista.Count == 0)
        {
            _mensajeEstado = "nada para exportar";
            RefrescarGrid();
            return;
        }
        using SaveFileDialog sfd = new();
        sfd.Filter = "CSV files (*.csv)|*.csv";
        sfd.FileName = $"auditoria_inconsistencias_{DateTime.Now:yyyyMMdd_HHmmss}.csv";
        if (sfd.ShowDialog() != DialogResult.OK)
        {
            return;
        }

        using StreamWriter sw = new(sfd.FileName, false, System.Text.Encoding.UTF8);
        sw.WriteLine("Fecha,Origen,OC,Master,Codigo,Descripcion,AccionSugerida,Severidad,Estado,Manual,RevisadoPor,FechaRevision");
        foreach (HallazgoAuditoria h in _vista)
        {
            string Celda(string? v)
            {
                v ??= "";
                return (v.Contains(',') || v.Contains('"') || v.Contains('\n'))
                    ? "\"" + v.Replace("\"", "\"\"") + "\"" : v;
            }
            sw.WriteLine(string.Join(",", [
                Celda(h.Fecha.ToString("yyyy-MM-dd HH:mm")),
                Celda(NombreOrigen(h.Origen)),
                Celda(h.OC), Celda(h.Rollid), Celda(h.Codigo),
                Celda(h.Descripcion), Celda(h.AccionSugerida),
                Celda(h.Severidad.ToString()), Celda(h.Estado.ToString()),
                h.RequiereAccionManual ? "SI" : "NO",
                Celda(h.RevisadoPor), Celda(h.FechaRevision?.ToString("yyyy-MM-dd HH:mm"))
            ]));
        }
        _mensajeEstado = "exportado a " + sfd.FileName;
        RefrescarGrid();
    }

    private sealed class FilaHallazgo
    {
        public Guid Id { get; set; }
        public DateTime Fecha { get; set; }
        public string Origen { get; set; } = string.Empty;
        public string OC { get; set; } = string.Empty;
        public string Codigo { get; set; } = string.Empty;
        public string Descripcion { get; set; } = string.Empty;
        public string Severidad { get; set; } = string.Empty;
        public string Estado { get; set; } = string.Empty;
        public bool Manual { get; set; }
    }
}
