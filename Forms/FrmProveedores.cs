using System.Data;
using Ritrama2025.Helpers;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ProveedorService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Proveedores (rediseño 30/70): panel izquierdo con buscador, cuadro
    /// de resumen (total/filtrado) y grid de proveedores; panel derecho con la página de
    /// detalle, que se refresca al seleccionar una fila del grid.
    /// </summary>
    public partial class FrmProveedores : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly IProveedorService _proveedoresService;
        private DataTable _dtProveedores = new();

        /// <summary>
        /// Crea el formulario de proveedores con su servicio de listado.
        /// </summary>
        /// <param name="proveedoresService">Servicio que trae el listado de la base.</param>
        public FrmProveedores(IProveedorService proveedoresService)
        {
            InitializeComponent();
            ArgumentNullException.ThrowIfNull(proveedoresService);
            _proveedoresService = proveedoresService;

            Text = "Proveedores";

            // Este módulo usa el estilo VERDE de SunnyUI (igual que Pedidos/Producción/Despacho).
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();

            // Filtrado en vivo por nombre sobre el listado ya cargado: el RowFilter del
            // DataTable se refleja solo en el grid enlazado, sin volver a la base.
            txtBuscar.TextChanged += (_, _) => AplicarFiltroBusqueda();

            // RefrescarDetalle con los dos eventos: el SelectionChanged se difiere al
            // siguiente ciclo de mensajes (y dispara con la fila anterior) mientras que
            // el CurrentCellChanged dispara síncrono — mismo criterio que FrmClientes.
            gridProveedores.SelectionChanged += (_, _) => RefrescarDetalle();
            gridProveedores.CurrentCellChanged += (_, _) => RefrescarDetalle();
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
        /// Carga el listado de proveedores antes de mostrar la pestaña. Si la base falla,
        /// se reporta el error y el formulario queda visible con el grid vacío.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _dtProveedores = await _proveedoresService.LoadListadoAsync();
                gridProveedores.DataSource = _dtProveedores;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar Proveedores: " + ex.Message);
            }
            finally
            {
                ActualizaResumen();
            }
        }

        /// <summary>
        /// Filtra el listado por nombre (comodín SQL, escapando las comillas)
        /// y actualiza el cuadro de resumen.
        /// </summary>
        private void AplicarFiltroBusqueda()
        {
            string filtro = txtBuscar.Text?.Trim() ?? string.Empty;
            _dtProveedores.DefaultView.RowFilter = filtro.Length == 0
                ? string.Empty
                : $"Proveedor_Name LIKE '%{filtro.Replace("'", "''")}%'";
            ActualizaResumen();
        }

        /// <summary>
        /// Cuadro de resumen bajo el buscador: total de proveedores sin filtro, o
        /// cuántos se muestran del total cuando el filtro está activo.
        /// </summary>
        private void ActualizaResumen()
        {
            int total = _dtProveedores.Rows.Count;
            int visibles = _dtProveedores.DefaultView.Count;
            bool filtrando = (txtBuscar.Text?.Trim().Length ?? 0) > 0;

            lblResumen.Text = !filtrando
                ? $"Total: {total} {(total == 1 ? "proveedor" : "proveedores")}"
                : $"Mostrando {visibles} de {total} {(total == 1 ? "proveedor" : "proveedores")}";
        }

        /// <summary>
        /// Refresca los campos de la página de detalle con la fila seleccionada
        /// del grid; sin selección deja los valores en "—".
        /// </summary>
        private void RefrescarDetalle()
        {
            if (gridProveedores.CurrentRow?.DataBoundItem is not DataRowView fila)
            {
                LimpiarDetalle();
                return;
            }

            txtValorId.Text = Texto(fila, "Proveedor_Id");
            txtValorNombre.Text = Texto(fila, "Proveedor_Name");
            txtValorTelefono.Text = Texto(fila, "phone");
            txtValorDireccion.Text = Texto(fila, "direccion");
            txtValorEmail.Text = Texto(fila, "email");
            txtValorUnity1.Text = SiNo(fila, "unidad_master_1");
            txtValorUnity2.Text = SiNo(fila, "unidad_master_2");
            txtValorEstado.Text = Texto(fila, "status");
            txtValorEstado.ForeColor = txtValorEstado.Text == "activo"
                ? Color.FromArgb(60, 110, 20)
                : Color.FromArgb(180, 60, 60);
        }

        /// <summary>
        /// Deja el detalle vacío ("—") cuando no hay fila seleccionada.
        /// </summary>
        private void LimpiarDetalle()
        {
            txtValorId.Text = "—";
            txtValorNombre.Text = "—";
            txtValorTelefono.Text = "—";
            txtValorDireccion.Text = "—";
            txtValorEmail.Text = "—";
            txtValorUnity1.Text = "—";
            txtValorUnity2.Text = "—";
            txtValorEstado.Text = "—";
            txtValorEstado.ForeColor = Color.FromArgb(48, 48, 48);
        }

        /// <summary>Lee un campo del detalle; "—" si la columna no existe o está vacía.</summary>
        private static string Texto(DataRowView fila, string columna)
        {
            if (!fila.Row.Table.Columns.Contains(columna))
            {
                return "—";
            }

            object valor = fila[columna];
            if (valor is null or DBNull)
            {
                return "—";
            }

            string texto = valor.ToString() ?? string.Empty;
            return texto.Length == 0 ? "—" : texto;
        }

        /// <summary>Traduce un bit a Sí/No ("—" si no hay valor).</summary>
        private static string SiNo(DataRowView fila, string columna)
        {
            if (!fila.Row.Table.Columns.Contains(columna) || fila[columna] is not bool valor)
            {
                return "—";
            }

            return valor ? "Sí" : "No";
        }
    }
}
