using FluentAssertions;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de la regla de formato del numero de pedido. Logica pura: no abren la base de datos.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoNumeroTests
{
    [Theory]
    [InlineData(1, "SO-0001")]
    [InlineData(27, "SO-0027")]
    [InlineData(100, "SO-0100")]
    [InlineData(999, "SO-0999")]
    [InlineData(9999, "SO-9999")]
    public void Formatear_NumeroValido_AgregaPrefijoYRelenoACuatroDigitos(int numero, string esperado)
    {
        PedidoNumero.Formatear(numero).Should().Be(esperado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(10000)]
    public void Formatear_NumeroFueraDeRango_Throws(int numero)
    {
        Action accion = () => PedidoNumero.Formatear(numero);

        accion.Should().Throw<ArgumentOutOfRangeException>();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("SO-")]
    [InlineData("SO-abc")]
    [InlineData("SO-12")]
    [InlineData("SO-00001")]
    [InlineData("so-0001")]
    [InlineData("  SO-0001")]
    [InlineData("PED-0001")]
    public void EsValido_FormatoInvalido_Rechazado(string? numero)
    {
        PedidoNumero.EsValido(numero).Should().BeFalse();
    }

    [Fact]
    public void EsValido_DigitoUnicode_NoAceptado()
    {
        // char.IsDigit acepta digitos Unicode, pero int.TryParse con InvariantCulture los
        // rechaza. Si EsValido los aceptara, EsValido y ParteNumerica se contradirian.
        string conDigitoUnicode = PedidoNumero.Prefijo + "\u0664\u0664\u0664\u0664";

        PedidoNumero.EsValido(conDigitoUnicode).Should().BeFalse();
        PedidoNumero.ParteNumerica(conDigitoUnicode).Should().Be(0);
    }

    [Theory]
    [InlineData("SO-0001")]
    [InlineData("SO-0027")]
    [InlineData("SO-9999")]
    public void EsValido_FormatoValido_Aceptado(string numero)
    {
        PedidoNumero.EsValido(numero).Should().BeTrue();
    }

    [Theory]
    [InlineData("SO-0027", 27)]
    [InlineData("SO-0001", 1)]
    [InlineData("SO-9999", 9999)]
    [InlineData("SO-0000", 0)]
    public void ParteNumerica_FormatoValido_DevuelveLaParteNumerica(string numero, int esperado)
    {
        PedidoNumero.ParteNumerica(numero).Should().Be(esperado);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("basura")]
    [InlineData("SO-abc")]
    public void ParteNumerica_FormatoInvalido_DevuelveCeroSinLanzar(string? numero)
    {
        PedidoNumero.ParteNumerica(numero).Should().Be(0);
    }

    [Fact]
    public void EsValidoYParteNumerica_CuadranParaTodosLosDigitosValidos()
    {
        // Los tres miembros tienen que ser coherentes entre si: si Formatear produce algo que
        // EsValido rechaza, la pantalla dejaria de poder editar el pedido recien creado.
        for (int numero = 1; numero <= 9999; numero += 337)
        {
            string texto = PedidoNumero.Formatear(numero);

            PedidoNumero.EsValido(texto).Should().BeTrue(
                "el numero {0} debe producir un texto que el propio validador acepte", texto);
            PedidoNumero.ParteNumerica(texto).Should().Be(numero);
        }
    }
}
