using FluentAssertions;
using Ritrama2025.Services.ProduccionService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de integración del contador interno de productos contra el SQL Server de pruebas/dev.
///
/// Cubren el bug del "contador que siempre daba 99999": el método usaba OUTPUT DELETED.par1
/// (el valor ANTES de sumar) y el semillero hacía INSERT con 99999 devolviendo 99999, de modo
/// que la siguiente llamada volvía a entregar 99999 y dos altas seguidas se quedaban con el
/// mismo código. Con INSERTED el valor guardado es siempre el último entregado.
/// </summary>
[Trait("Categoria", "Integracion")]
public class ConsecutivosServiceTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;
    private readonly ConsecutivosService _service;

    public ConsecutivosServiceTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
        _service = new ConsecutivosService(TestConfiguration.BuildServiceConfiguration());
    }

    [Fact]
    public void ConsecutivoProducto_EntregaElValorYaIncrementadoYNuncaRepite()
    {
        // Par1 es nvarchar en la base: se lee tal cual y se convierte.
        object? previo = _fixture.ExecuteScalar("SELECT par1 FROM control WHERE filter = 'PROD'");

        // Sin semillero, el propio servicio lo crea y el primer codigo entregado es 1;
        // con semillero, lo que toca es el valor guardado mas uno.
        int esperado = previo is null ? 1 : Convert.ToInt32(previo) + 1;

        int primero = _service.GetAndIncrementConsecProducto();
        primero.Should().Be(
            esperado,
            "el contador debe entregar el valor YA incrementado (INSERTED); con DELETED devolvia "
                + "el valor anterior y repetia el primer codigo");

        int segundo = _service.GetAndIncrementConsecProducto();
        segundo.Should().Be(
            primero + 1,
            "dos altas de producto seguidas no pueden repetir numero ni saltarse ninguno");
    }
}
