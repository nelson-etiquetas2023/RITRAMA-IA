using FluentAssertions;
using Ritrama2025.Services.SeguridadService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Comparacion de la clave contra el hash de la base. Es el paso que decide si un
/// usuario puede entrar o no, y al ser estatica se prueba sin tocar la base: un hash
/// vacio o corrupto tiene que contar como credencial invalida y nunca lanzar, porque
/// una excepcion ahi se leeria como "error del sistema" en vez de "clave mala".
/// </summary>
[Trait("Categoria", "Unit")]
public class LoginPasswordTests
{
    private const string Clave = "Ritrama.2026";

    [Fact]
    public void ClaveCorrectaContraElHashDeLaBase()
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(Clave);

        SeguridadService.VerificarPassword(Clave, hash).Should().BeTrue();
    }

    [Fact]
    public void ClaveDistintaNoPasa()
    {
        string hash = BCrypt.Net.BCrypt.HashPassword(Clave);

        SeguridadService.VerificarPassword("otra-clave", hash).Should().BeFalse();
        SeguridadService.VerificarPassword(string.Empty, hash).Should().BeFalse();
    }

    [Fact]
    public void HashVacioCuentaComoCredencialInvalida()
    {
        SeguridadService.VerificarPassword(Clave, string.Empty).Should().BeFalse();
        SeguridadService.VerificarPassword(Clave, "   ").Should().BeFalse();
    }

    [Fact]
    public void HashNuloNoLanza()
    {
        SeguridadService.VerificarPassword(Clave, null!).Should().BeFalse();
    }

    [Fact]
    public void HashCorruptoNoLanzaYSigueSiendoUnFallo()
    {
        // Un dato guardado a mano o truncado por una migracion: tiene que devolver
        // false y no explotar en el medio del login.
        SeguridadService.VerificarPassword(Clave, "no-es-un-hash-bcrypt").Should().BeFalse();
        SeguridadService.VerificarPassword(Clave, "$2a$novalido").Should().BeFalse();
        SeguridadService.VerificarPassword(Clave, "12345").Should().BeFalse();
    }

    [Fact]
    public void LosHashesNuevosSeGuardanEnFormatoBcrypt()
    {
        // Si esto falla, la fila no se podria verificar nunca y el usuario quedaria
        // sin poder entrar de forma permanente.
        string hash = BCrypt.Net.BCrypt.HashPassword(Clave);

        hash.Should().StartWith("$2");
        SeguridadService.VerificarPassword(Clave, hash).Should().BeTrue(
            "recien creado tiene que poder verificarse");
    }
}
