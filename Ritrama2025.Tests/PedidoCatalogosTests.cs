using FluentAssertions;
using Ritrama2025.Models;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de los vocabularios cerrados del pedido y de la regla que gobierna si la condicion de
/// pago se puede editar. Logica pura: no abren la base de datos ni la interfaz.
///
/// La regla importa porque sin ella un pedido al contado guardaba "30 dias" sin que nadie pudiera
/// corregirlo. Se fija aca y no en el formulario para que se pueda ejercitar sin levantar la
/// pantalla, que es lo que impidio detectarla cuando estaba escrita sobre el combo.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoCatalogosTests
{
    [Fact]
    public void Contado_EsElMismoValorEnLosDosVocababularios()
    {
        PedidoCatalogos.TipoVenta.Should().Contain(PedidoCatalogos.Contado);
        PedidoCatalogos.CondicionesPago.Should().Contain(PedidoCatalogos.Contado);
    }

    [Theory]
    [InlineData("contado", true)]
    [InlineData("CONTADO", true)]
    [InlineData("Contado", true)]
    [InlineData("credito", false)]
    [InlineData("", false)]
    [InlineData(null, false)]
    [InlineData(" contado", false)]
    public void EsContado_ComparaSinDistinguirMayusculas(string? tipoVenta, bool esperado)
    {
        PedidoCatalogos.EsContado(tipoVenta).Should().Be(esperado);
    }

    /// <summary>
    /// La capitalizacion viene de una columna varchar y no todas las filas se escribieron igual.
    /// "Contado" tiene que contar como contado o el combo se habilitaria con un pedido al
    /// contado guardado con otra capitalizacion.
    /// </summary>
    [Fact]
    public void CondicionesPagoEditable_ContadoConOtraCapitalizacion_TampocoSeEdita()
    {
        PedidoCatalogos.CondicionesPagoEditable("CONTADO", esNuevo: true).Should().BeFalse();
    }

    [Theory]
    [InlineData("credito", true, true)]
    [InlineData("", true, true)]
    [InlineData(null, true, true)]
    [InlineData("contado", true, false)]
    [InlineData("contado", false, false)]
    [InlineData("credito", false, false)]
    public void CondicionesPagoEditable_SoloEsEditableEnNuevoYSiNoEsContado(
        string? tipoVenta, bool esNuevo, bool esperado)
    {
        PedidoCatalogos.CondicionesPagoEditable(tipoVenta, esNuevo).Should().Be(esperado);
    }

    /// <summary>
    /// El modo manda por encima del tipo de venta: al consultar un pedido guardado el combo no
    /// se edita, este al contado o no. Sin esta precedencia, un pedido al contado en consulta
    /// dejaria el combo editable y se podrian cambiar datos de un pedido ya guardado.
    /// </summary>
    [Fact]
    public void CondicionesPagoEditable_EnConsultaNoEditaNiAlContadoNiAlCredito()
    {
        PedidoCatalogos.CondicionesPagoEditable("credito", esNuevo: false).Should().BeFalse();
        PedidoCatalogos.CondicionesPagoEditable("contado", esNuevo: false).Should().BeFalse();
    }

    /// <summary>
    /// El tipo de venta solo tiene los dos valores medidos sobre los pedidos existentes. Si
    /// alguno se agrega, la regla de la condicion de pago hay que revisarla: con un valor nuevo
    /// el combo se habilitaria sin que nadie haya decidido si corresponde.
    /// </summary>
    [Fact]
    public void TipoVenta_SoloContieneLosDosValoresConocidos()
    {
        PedidoCatalogos.TipoVenta.Should().BeEquivalentTo(
            new[] { PedidoCatalogos.Contado, "credito" });
    }
}
