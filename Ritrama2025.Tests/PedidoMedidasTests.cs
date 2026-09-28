using System;
using System.Collections.Generic;
using FluentAssertions;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Regla de medidas del detalle de un pedido: solo los productos de categoria Rollo Cortado
/// exigen ancho y largo, porque un rollo se vende por sus medidas. Las demas categorias se
/// venden por unidades, asi que el ancho y el largo no aplican y quedan en cero.
///
/// El nombre de la categoria llega como texto desde el combo de productos, que lo arma el
/// CASE WHEN de R.cs sobre los cuatro bit de producto. No se lee el producto de la base
/// otra vez, para que la regla no dependa de una segunda consulta.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoMedidasTests
{
    [Theory]
    [InlineData("Rollo Cortado")]
    [InlineData("rollo cortado")]
    [InlineData("  Rollo Cortado  ")]
    public void PideMedidas_CategoriaRolloCortado_DevuelveTrue(string tipo)
    {
        PedidoMedidas.PideMedidas(tipo).Should().BeTrue();
    }

    [Theory]
    [InlineData("Master")]
    [InlineData("Resma")]
    [InlineData("Graphics")]
    [InlineData("")]
    [InlineData(null)]
    public void PideMedidas_CategoriaQueNoEsRolloCortado_DevuelveFalse(string? tipo)
    {
        PedidoMedidas.PideMedidas(tipo).Should().BeFalse();
    }

    [Theory]
    [InlineData("60", 60)]
    [InlineData("  60  ", 60)]
    [InlineData("60,5", 60.5)]
    [InlineData("0", 0)]
    public void LeerMedida_TextoNumero_ConvierteElValor(string texto, decimal esperado)
    {
        PedidoMedidas.LeerMedida(texto, out decimal valor).Should().BeTrue();

        valor.Should().Be(esperado);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("abc")]
    [InlineData("12abc")]
    [InlineData(null)]
    public void LeerMedida_TextoNoNumero_DevuelveFalseYCero(string? texto)
    {
        PedidoMedidas.LeerMedida(texto, out decimal valor).Should().BeFalse();

        valor.Should().Be(0m);
    }

    [Fact]
    public void Validar_RolloCortadoConAnchoYLargo_Aceptado()
    {
        PedidoMedidas.Validar("Rollo Cortado", 24m, 1000m, out string? error).Should().BeTrue();

        error.Should().BeNull();
    }

    [Fact]
    public void Validar_RolloCortadoSinAncho_Rechazado()
    {
        PedidoMedidas.Validar("Rollo Cortado", 0m, 1000m, out string? error).Should().BeFalse();

        error.Should().Contain("ancho");
    }

    [Fact]
    public void Validar_RolloCortadoSinLargo_Rechazado()
    {
        PedidoMedidas.Validar("Rollo Cortado", 24m, 0m, out string? error).Should().BeFalse();

        error.Should().Contain("largo");
    }

    [Fact]
    public void Validar_RolloCortadoSinNingunaMedida_Rechazado()
    {
        PedidoMedidas.Validar("Rollo Cortado", 0m, 0m, out string? error).Should().BeFalse();

        error.Should().NotBeNullOrEmpty();
    }

    [Fact]
    public void Validar_RolloCortadoConMedidaNegativa_Rechazado()
    {
        PedidoMedidas.Validar("Rollo Cortado", -24m, 1000m, out string? error).Should().BeFalse();

        error.Should().Contain("ancho");
    }

    [Theory]
    [InlineData("Master")]
    [InlineData("Resma")]
    [InlineData("Graphics")]
    [InlineData("")]
    [InlineData(null)]
    public void Validar_CategoriaQueNoEsRolloCortado_SinMedidasAceptado(string? tipo)
    {
        PedidoMedidas.Validar(tipo, 0m, 0m, out string? error).Should().BeTrue();

        error.Should().BeNull();
    }

    [Fact]
    public void Validar_CategoriaQueNoEsRolloCortado_RespetaLasMedidasSiVienen()
    {
        PedidoMedidas.Validar("Resma", 10m, 20m, out string? error).Should().BeTrue();

        error.Should().BeNull();
    }
}
