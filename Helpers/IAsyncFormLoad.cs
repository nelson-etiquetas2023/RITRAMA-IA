namespace Ritrama2025.Helpers
{
    /// <summary>
    /// Interfaz para formularios que cargan datos de forma asincrónica.
    /// El FormManager llama a InitializeAsync() ANTES de mostrar el form, por lo que
    /// la carga y el pintado de controles ocurren mientras se muestra un FrmLoading,
    /// evitando la pantalla en blanco y el efecto de "minimizado a maximizado".
    /// </summary>
    public interface IAsyncFormLoad
    {
        /// <summary>
        /// Carga los datos y deja el formulario listo para mostrarse (bindings, grids,
        /// estilos, etc.). Se invoca antes de agregar el form a la pestana central.
        /// </summary>
        Task InitializeAsync();
    }
}
