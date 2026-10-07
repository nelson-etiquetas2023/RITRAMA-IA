using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProductsService;
using Sunny.UI;

namespace Ritrama2025.Forms.Otros
{
    /// <summary>
    /// Importación masiva de productos desde Excel. Plantilla mínima (una fila = un producto):
    /// product_id (el código Ritrama único que teclea el usuario), product_name y categoria
    /// (Master / Rollo Cortado / Resma / Graphics); se aceptan también las cabeceras
    /// CodigoRitrama, Nombre y Tipo. El resto de datos nace por defecto y el consecutivo
    /// del sistema lo guarda el importador en IdConsec. La hoja se lee y se valida sin insertar nada;
    /// solo entran las filas válidas y las inválidas se listan con su motivo.
    /// </summary>
    public partial class Frm_ImportProductos : UIForm
    {
        private readonly IProductsImportService _importService;

        /// <summary>Filas leídas de la hoja (válidas y con error), fuente del grid.</summary>
        private List<ProductoImportFila> _filas = [];

        public Frm_ImportProductos(IProductsImportService importService)
        {
            ArgumentNullException.ThrowIfNull(importService);
            InitializeComponent();
            _importService = importService;
        }

        private void BtnBuscar_Click(object? sender, EventArgs e)
        {
            OpenFileDialog dialog = new()
            {
                Filter = "Excel Files|*.xls;*.xlsx;*.xlsm",
                Title = "Seleccione la hoja de productos"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            txtPathFile.Text = dialog.FileName;

            Result<List<ProductoImportFila>> lectura = _importService.LeerYValidarExcel(dialog.FileName);
            if (!lectura.IsSuccess)
            {
                _filas = [];
                gridPreview.DataSource = null;
                lblContador.Text = lectura.Error ?? "No se pudo leer la hoja.";
                return;
            }

            _filas = lectura.Value!;
            gridPreview.DataSource = _filas;
            int validas = _filas.Count(f => f.EsValida);
            lblContador.Text = $"{validas} de {_filas.Count} filas listas para importar.";
            btnImportar.Enabled = validas > 0;
        }

        private async void BtnImportar_Click(object? sender, EventArgs e)
        {
            List<ProductoImportFila> validas = _filas.Where(f => f.EsValida).ToList();
            if (validas.Count == 0)
            {
                return;
            }

            btnImportar.Enabled = false;
            btnBuscar.Enabled = false;

            Result<ProductImportResumen> resultado = await _importService.ImportarFilasAsync(validas);

            btnImportar.Enabled = true;
            btnBuscar.Enabled = true;

            if (!resultado.IsSuccess)
            {
                MessageBox.Show(this, resultado.Error ?? "No se pudo ejecutar la importación.",
                    "Importar productos", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            ProductImportResumen resumen = resultado.Value!;
            string mensaje = $"Importadas: {resumen.Insertadas} de {validas.Count} filas.";
            if (resumen.Fallos.Count > 0)
            {
                mensaje += Environment.NewLine + Environment.NewLine
                    + "No se insertaron:" + Environment.NewLine
                    + string.Join(Environment.NewLine, resumen.Fallos.Take(20))
                    + (resumen.Fallos.Count > 20 ? Environment.NewLine + $"... y {resumen.Fallos.Count - 20} más." : string.Empty);
            }

            MessageBox.Show(this, mensaje, "Importar productos",
                MessageBoxButtons.OK, resumen.Insertadas > 0 ? MessageBoxIcon.Information : MessageBoxIcon.Warning);

            if (resumen.Insertadas > 0)
            {
                DialogResult = DialogResult.OK;
                Close();
            }
        }

        private void BtnPlantilla_Click(object? sender, EventArgs e)
        {
            SaveFileDialog dialog = new()
            {
                FileName = "Plantilla_Productos.xlsx",
                Filter = "Excel Files|*.xlsx",
                Title = "Guardar la plantilla de productos"
            };

            if (dialog.ShowDialog(this) != DialogResult.OK)
            {
                return;
            }

            Result guardado = _importService.CrearPlantilla(dialog.FileName);
            if (!guardado.IsSuccess)
            {
                MessageBox.Show(this, guardado.Error ?? "No se pudo guardar la plantilla.",
                    "Descargar plantilla", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            MessageBox.Show(this,
                "Plantilla guardada en:" + Environment.NewLine + dialog.FileName
                + Environment.NewLine + Environment.NewLine
                + "Rellene product_id (el código Ritrama), product_name y categoria "
                + "(Master, Rollo Cortado, Resma o Graphics), y vuelva a pulsar "
                + "«Buscar...» para revisarla antes de importar.",
                "Descargar plantilla", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void BtnCancelar_Click(object? sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
