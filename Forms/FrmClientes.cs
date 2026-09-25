using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Forms.Otros;
using Ritrama2025.Helpers;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ProduccionService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Clientes: listado, alta, edición, búsqueda y anulación de clientes.
    /// </summary>
    public partial class FrmClientes : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IConfiguration _configuration;
        private readonly string _conexion;

        private DataTable _dtClientes = new();
        private Guid? _customerEnEdicion;
        private string _errorMsg = string.Empty;

        /// <summary>
        /// Crea el formulario de clientes con su configuración.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación (cadena de conexión).</param>
        public FrmClientes(IConfiguration configuration)
        {
            InitializeComponent();
            _configuration = configuration ?? throw new ArgumentNullException(nameof(configuration));

            string ambiente = _configuration["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            _conexion = _configuration.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]
                        ?? throw new InvalidOperationException("No se pudo resolver la cadena de conexión.");

            Text = "Clientes";

            // Este módulo usa el estilo VERDE de SunnyUI (igual que Pedidos/Producción/Despacho).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();
        }

        /// <summary>
        /// Reaplica el tema verde para pisar el UIStyleManager global del Main.
        /// </summary>
        public void ReaplicarTema() => AplicarTemaVerde();

        private void AplicarTemaVerde()
        {
            Color verde = Color.FromArgb(110, 190, 40);
            BackColor = Color.White;
            Style = UIStyle.Green;
            TitleColor = verde;
            TitleForeColor = Color.White;
        }

        /// <summary>
        /// Carga asíncrona del listado de clientes antes de mostrar el form.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                await CargarListadoAsync();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Clientes: " + ex.Message);
            }
            FinalizarConfiguracionUI();
            DeshabilitarCaptura();
        }

        private void FinalizarConfiguracionUI()
        {
            gridClientes.Columns.Clear();
            gridClientes.AutoGenerateColumns = false;
            CommonService.ADD_COLUMN_GRID("customer_id", 140, "Código", "customer_id", gridClientes);
            CommonService.ADD_COLUMN_GRID("customer_name", 320, "Cliente", "customer_name", gridClientes);
            CommonService.ADD_COLUMN_GRID("customer_category", 150, "Categoría", "customer_category", gridClientes);
            CommonService.ADD_COLUMN_GRID("customer_email", 220, "Email", "customer_email", gridClientes);
            gridClientes.DataSource = _dtClientes;
        }

        private async Task<DataTable> CargarTablaAsync(string sql)
        {
            DataTable dt = new DataTable();
            using SqlConnection conn = new SqlConnection(_conexion);
            using SqlCommand cmd = new SqlCommand(sql, conn);
            await conn.OpenAsync();
            using SqlDataAdapter da = new SqlDataAdapter(cmd);
            da.Fill(dt);
            return dt;
        }

        private async Task CargarListadoAsync()
        {
            _dtClientes = await CargarTablaAsync(
                "SELECT customer_id, customer_name, customer_category, customer_email FROM customer WHERE anulado = 0 ORDER BY customer_name");
            if (gridClientes != null)
            {
                gridClientes.DataSource = _dtClientes;
            }
        }

        private void DeshabilitarCaptura()
        {
            _customerEnEdicion = null;
            LimpiarCampos();
            panelCaptura.Enabled = false;
            tsbGuardar.Enabled = false;
            tsbAnular.Enabled = false;
        }

        private void HabilitarCaptura()
        {
            panelCaptura.Enabled = true;
            tsbGuardar.Enabled = true;
            tsbAnular.Enabled = false;
        }

        private void LimpiarCampos()
        {
            txtNombre.Clear();
            txtCategoria.Clear();
            txtEmail.Clear();
        }

        private void LimpiarBusqueda()
        {
            txtBuscar.Text = string.Empty;
            AplicarFiltroBusqueda();
        }

        // NUEVO: limpia los campos y habilita la captura de un cliente nuevo.
        private void TsbNuevo_Click(object? sender, EventArgs e)
        {
            _customerEnEdicion = null;
            LimpiarCampos();
            gridClientes.ClearSelection();
            HabilitarCaptura();
            txtNombre.Focus();
        }

        // Al seleccionar una fila del listado, carga sus valores en la captura (edición).
        private void GridClientes_SelectionChanged(object? sender, EventArgs e)
        {
            if (gridClientes.CurrentRow == null || gridClientes.CurrentRow.Index < 0)
            {
                tsbAnular.Enabled = false;
                return;
            }

            // Si se está capturando un cliente nuevo, no pisar los campos.
            if (_customerEnEdicion == null && panelCaptura.Enabled)
            {
                return;
            }

            DataGridViewRow row = gridClientes.CurrentRow;
            if (row.Cells["customer_id"].Value == DBNull.Value || row.Cells["customer_id"].Value == null)
            {
                tsbAnular.Enabled = false;
                return;
            }

            if (Guid.TryParse(row.Cells["customer_id"].Value?.ToString(), out Guid guid))
            {
                _customerEnEdicion = guid;
                txtNombre.Text = row.Cells["customer_name"].Value?.ToString() ?? string.Empty;
                txtCategoria.Text = row.Cells["customer_category"].Value?.ToString() ?? string.Empty;
                txtEmail.Text = row.Cells["customer_email"].Value?.ToString() ?? string.Empty;
                panelCaptura.Enabled = true;
                tsbGuardar.Enabled = true;
                tsbAnular.Enabled = true;
            }
            else
            {
                tsbAnular.Enabled = false;
            }
        }

        // GUARDAR: inserta un cliente nuevo o actualiza uno existente mostrando un loading.
        private async void TsbGuardar_Click(object? sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtNombre.Text))
            {
                MessageBox.Show("Debe ingresar el nombre del cliente.", "Validación",
                    MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            tsbGuardar.Enabled = false;
            using FrmLoading loading = new FrmLoading("Guardando cliente...");
            loading.Show(this);
            loading.BringToFront();
            try
            {
                bool guardado = await Task.Run(() => _customerEnEdicion == null ? InsertarCliente() : ActualizarCliente());
                if (guardado)
                {
                    MessageBox.Show("Cliente guardado.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarListadoAsync();
                    LimpiarBusqueda();
                    DeshabilitarCaptura();
                }
                else
                {
                    MessageBox.Show("No se pudo guardar el cliente: " + ObtenerErrorServicio(), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tsbGuardar.Enabled = true;
                }
            }
            finally
            {
                if (!loading.IsDisposed)
                {
                    loading.Close();
                }
            }
        }

        private bool InsertarCliente()
        {
            Guid id = Guid.NewGuid();
            try
            {
                using SqlConnection conn = new SqlConnection(_conexion);
                using SqlCommand cmd = new SqlCommand(
                    "INSERT INTO customer (customer_id, customer_name, customer_category, customer_email, anulado) VALUES (@p1, @p2, @p3, @p4, 0)", conn);
                cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.UniqueIdentifier) { Value = id });
                cmd.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = txtNombre.Text.Trim() });
                cmd.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = (object?)NuloSiVacio(txtCategoria.Text) ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = (object?)NuloSiVacio(txtEmail.Text) ?? DBNull.Value });
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                return false;
            }
        }

        private bool ActualizarCliente()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conexion);
                using SqlCommand cmd = new SqlCommand(
                    "UPDATE customer SET customer_name = @p2, customer_category = @p3, customer_email = @p4 WHERE customer_id = @p1", conn);
                cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.UniqueIdentifier) { Value = _customerEnEdicion!.Value });
                cmd.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = txtNombre.Text.Trim() });
                cmd.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = (object?)NuloSiVacio(txtCategoria.Text) ?? DBNull.Value });
                cmd.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = (object?)NuloSiVacio(txtEmail.Text) ?? DBNull.Value });
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                return false;
            }
        }

        private string? NuloSiVacio(string value)
        {
            string trim = value?.Trim() ?? string.Empty;
            return trim.Length == 0 ? null : trim;
        }

        // ANULAR: confirma y marca el cliente seleccionado como anulado.
        private async void TsbAnular_Click(object? sender, EventArgs e)
        {
            if (_customerEnEdicion == null)
            {
                MessageBox.Show("Seleccione un cliente para anular.", "Aviso",
                    MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            string nombre = txtNombre.Text.Trim();
            DialogResult confirm = MessageBox.Show($"¿Anular el cliente {nombre}?", "Confirmar anulación",
                MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (confirm != DialogResult.Yes)
            {
                return;
            }

            tsbAnular.Enabled = false;
            using FrmLoading loading = new FrmLoading("Anulando cliente...");
            loading.Show(this);
            loading.BringToFront();
            try
            {
                bool anulado = await Task.Run(AnularCliente);
                if (anulado)
                {
                    MessageBox.Show("Cliente anulado.", "Éxito",
                        MessageBoxButtons.OK, MessageBoxIcon.Information);
                    await CargarListadoAsync();
                    LimpiarBusqueda();
                    DeshabilitarCaptura();
                }
                else
                {
                    MessageBox.Show("No se pudo anular el cliente: " + ObtenerErrorServicio(), "Error",
                        MessageBoxButtons.OK, MessageBoxIcon.Error);
                    tsbAnular.Enabled = true;
                }
            }
            finally
            {
                if (!loading.IsDisposed)
                {
                    loading.Close();
                }
            }
        }

        private bool AnularCliente()
        {
            try
            {
                using SqlConnection conn = new SqlConnection(_conexion);
                using SqlCommand cmd = new SqlCommand("UPDATE customer SET anulado = 1 WHERE customer_id = @p1", conn);
                cmd.Parameters.Add(new SqlParameter("@p1", SqlDbType.UniqueIdentifier) { Value = _customerEnEdicion!.Value });
                conn.Open();
                cmd.ExecuteNonQuery();
                return true;
            }
            catch (Exception ex)
            {
                _errorMsg = ex.Message;
                return false;
            }
        }

        // BUSCAR: filtra el DataTable cargado por nombre (sin volver a la BD).
        private void TsbBuscar_Click(object? sender, EventArgs e)
        {
            AplicarFiltroBusqueda();
        }

        private void TxtBuscar_KeyDown(object? sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.SuppressKeyPress = true;
                AplicarFiltroBusqueda();
            }
        }

        private void AplicarFiltroBusqueda()
        {
            if (_dtClientes == null)
            {
                return;
            }

            string filtro = txtBuscar.Text?.Trim() ?? string.Empty;
            _dtClientes.DefaultView.RowFilter = filtro.Length == 0
                ? string.Empty
                : $"customer_name LIKE '%{EscapeLike(filtro)}%'";
            gridClientes.DataSource = _dtClientes;
        }

        private static string EscapeLike(string value)
        {
            return value.Replace("'", "''");
        }

        /// <summary>
        /// Obtiene el mensaje del último error de acceso a datos.
        /// </summary>
        private string ObtenerErrorServicio()
        {
            return string.IsNullOrEmpty(_errorMsg) ? "Error desconocido." : _errorMsg;
        }
    }
}
