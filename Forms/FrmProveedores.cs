using Microsoft.Extensions.Configuration;
using Ritrama2025.Helpers;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Formulario de Proveedores (rediseño 30/70): panel izquierdo con buscador y grid
    /// de proveedores, panel derecho con la página de detalle. Por ahora es solo layout:
    /// la carga y captura de datos volverán en los próximos pasos del módulo.
    /// </summary>
    public partial class FrmProveedores : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        /// <summary>
        /// Crea el formulario de proveedores con su configuración.
        /// </summary>
        /// <param name="configuration">Configuración de la aplicación; se guardará como
        /// punto de partida cuando el módulo vuelva a tocar la base de datos.</param>
        public FrmProveedores(IConfiguration configuration)
        {
            InitializeComponent();
            ArgumentNullException.ThrowIfNull(configuration);

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
        /// Carga inicial invocada por el FormManager antes de mostrar la pestaña.
        /// El módulo es solo layout: no hay listado que traer todavía.
        /// </summary>
        public Task InitializeAsync() => Task.CompletedTask;
    }
}
