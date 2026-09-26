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
    [InlineData(1, "SO-00001")]
    [InlineData(27, "SO-00027")]
    [InlineData(100, "SO-00100")]
    [InlineData(999, "SO-00999")]
    [InlineData(9999, "SO-09999")]
    [InlineData(12345, "SO-12345")]
    [InlineData(99999, "SO-99999")]
    public void Formatear_NumeroValido_AgregaPrefijoYRelenoACincoDigitos(int numero, string esperado)
    {
        PedidoNumero.Formatear(numero).Should().Be(esperado);
    }

    [Theory]
    [InlineData(0)]
    [InlineData(-1)]
    [InlineData(100000)]
    public void Formatear_NumeroFueraDeRango_Throws(int numero)
    {
        Action accion = () => PedidoNumero.Formatear(numero);

        accion.Should().Throw<ArgumentOutOfRangeException>();
    }

    /// <summary>
    /// El mensaje dice el formato que produce el metodo, asi que su ancho tiene que salir de
    /// LargoNumero y no de un literal. Si alguien sube el largo y deja un "####" fijo, el
    /// mensaje pasa a describir un formato que Formatear ya no devuelve, y esta prueba es la
    /// que se entera. El ancho se arma con el mismo LargoNumero que usa el codigo de
    /// produccion, para que la prueba no tautologica: si el codigo y la pruebaaran a mirar
    /// constantes distintas, la asercion compararia el mensaje consigo mismo.
    /// </summary>
    [Fact]
    public void Formatear_Excepcion_ElMensajeAnunciaElAnchoReal()
    {
        string formatoEsperado = PedidoNumero.Prefijo + new string('#', PedidoNumero.LargoNumero);

        Action accion = () => PedidoNumero.Formatear(PedidoNumero.MaximoNumero + 1);

        accion.Should().Throw<ArgumentOutOfRangeException>()
            .WithMessage($"*{formatoEsperado}*");
    }

    /// <summary>
    /// Los casos de rechazo se mantienen con el largo correcto a proposito: si uno tuviera el
    /// largo equivocado pasaria por el chequeo de longitud y no probaria la razon que dice
    /// probar. "so-00001" tiene 8 caracteres, asi que solo puede fallar por la minuscula.
    /// </summary>
    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("SO-")]
    [InlineData("SO-abc")]
    [InlineData("SO-12")]
    [InlineData("SO-0001")]
    [InlineData("SO-000001")]
    [InlineData("so-00001")]
    [InlineData("  SO-00001")]
    [InlineData("PED-00001")]
    public void EsValido_FormatoInvalido_Rechazado(string? numero)
    {
        PedidoNumero.EsValido(numero).Should().BeFalse();
    }

    [Fact]
    public void EsValido_DigitoUnicode_NoAceptado()
    {
        // char.IsDigit acepta digitos Unicode, pero int.TryParse con InvariantCulture los
        // rechaza. Si EsValido los aceptara, EsValido y ParteNumerica se contradirian.
        string conDigitoUnicode = PedidoNumero.Prefijo + "\u0664\u0664\u0664\u0664\u0664";

        PedidoNumero.EsValido(conDigitoUnicode).Should().BeFalse();
        PedidoNumero.ParteNumerica(conDigitoUnicode).Should().Be(0);
    }

    [Theory]
    [InlineData("SO-00001")]
    [InlineData("SO-00027")]
    [InlineData("SO-09999")]
    [InlineData("SO-99999")]
    public void EsValido_FormatoValido_Aceptado(string numero)
    {
        PedidoNumero.EsValido(numero).Should().BeTrue();
    }

    [Theory]
    [InlineData("SO-00027", 27)]
    [InlineData("SO-00001", 1)]
    [InlineData("SO-09999", 9999)]
    [InlineData("SO-00000", 0)]
    [InlineData("SO-99999", 99999)]
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
        for (int numero = 1; numero <= 99999; numero += 3331)
        {
            string texto = PedidoNumero.Formatear(numero);

            PedidoNumero.EsValido(texto).Should().BeTrue(
                "el numero {0} debe producir un texto que el propio validador acepte", texto);
            PedidoNumero.ParteNumerica(texto).Should().Be(numero);
        }
    }
}
