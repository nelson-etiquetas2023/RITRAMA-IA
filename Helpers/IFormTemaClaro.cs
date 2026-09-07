namespace Ritrama2025.Helpers
{
    /// <summary>
    /// Marca los formularios cuyo tema NO debe sobrescribirse con el tema oscuro
    /// global (por ejemplo los que usan un estilo propio de SunnyUI como el verde).
    /// </summary>
    public interface IFormTemaClaro
    {
        /// <summary>
        /// Reaplica el tema propio del formulario (por ejemplo el verde de SunnyUI)
        /// para pisar cualquier re-estilizado hecho por el UIStyleManager global del
        /// formulario contenedor (Main) al embeberse o al repintar.
        /// </summary>
        void ReaplicarTema();
    }
}
