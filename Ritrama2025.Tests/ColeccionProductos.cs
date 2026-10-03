using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Todas las pruebas de Productos van en la MISMA coleccion, porque xUnit ejecuta de forma
/// secuencial lo que esta en una coleccion. Sin esto se pisan entre si: <c>SesionActual</c> es
/// un estatico compartido, asi que la prueba "sin permiso" puede encontrarse con la sesion que
/// acaba de abrir otra prueba y fallar sin motivo. Solo afecta a Productos; el resto del
/// proyecto sigue ejecutandose en paralelo.
/// </summary>
[CollectionDefinition("Productos", DisableParallelization = true)]
public class ColeccionProductos
{
}
