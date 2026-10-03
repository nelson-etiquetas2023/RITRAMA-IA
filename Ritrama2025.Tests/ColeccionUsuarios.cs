using Ritrama2025.Services.ProduccionService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Todas las pruebas de Usuarios van en la MISMA coleccion, porque xUnit ejecuta de
/// forma secuencial lo que esta en una coleccion. Sin esto se pisarian entre si:
/// <c>SesionActual</c> es un estatico compartido, asi que la prueba "sin permiso" puede
/// encontrarse con la sesion que acaba de abrir otra prueba y fallar sin motivo. Mismo
/// criterio que ColeccionProductos; el resto del proyecto sigue ejecutandose en
/// paralelo.
///
/// La coleccion ademas monta <see cref="SilencioDialogosDeServicio"/>: con esto el unico
/// hilo que puede estar ejecutando pruebas de Usuarios es el de la coleccion, que corre
/// sola al final, y ServiceErrors no puede enseñar un MessageBox a medias.
/// </summary>
[CollectionDefinition("Usuarios", DisableParallelization = true)]
public class ColeccionUsuarios : ICollectionFixture<SilencioDialogosDeServicio>
{
}

/// <summary>
/// Apaga el canal de notificacion de <c>ServiceErrors</c> mientras dura la coleccion.
/// Ese canal acaba en <c>MessageBox.Show</c> por defecto, y un dialogo modal dentro de
/// una prueba deja el proceso de pruebas colgado hasta que vstest lo corta por tiempo de
/// inactividad (le pasaba a "Exportar_SiElServicioFallaSeAvisaYNoSeDaPorHecho" y a
/// "ElFiltroDeRolSobreviveAUnaRecargaDelListado"). El canal se repone al terminar para
/// no dejarlo apagado al resto del ensamblado.
/// </summary>
public sealed class SilencioDialogosDeServicio : IDisposable
{
    private readonly Action<string> _notifyOriginal = ServiceErrors.Notify;

    public SilencioDialogosDeServicio() => ServiceErrors.Notify = _ => { };

    public void Dispose() => ServiceErrors.Notify = _notifyOriginal;
}
