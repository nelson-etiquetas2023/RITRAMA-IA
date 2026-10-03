using FluentAssertions;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Texto de usuario de la barra lateral: como se rellena el nombre, el tooltip y la
/// inicial del avatar. Las filas anteriores a la migracion traen nombre_completo en
/// NULL o en blanco, y un label vacio en el sidebar se ve roto, de ahi los fallbacks.
/// </summary>
[Trait("Categoria", "Unit")]
public class UsuarioHelperTests
{
    [Fact]
    public void SinSesionElNombreDiceSinSesion()
    {
        UsuarioHelper.NombreMostrar(null).Should().Be(UsuarioHelper.SinSesion);
        UsuarioHelper.Inicial(null).Should().Be("?");
    }

    [Fact]
    public void ConNombreCompletoSeUsaEse()
    {
        Usuario usuario = new() { Username = "nelson", NombreCompleto = "Nelson Pino" };

        UsuarioHelper.NombreMostrar(usuario).Should().Be("Nelson Pino");
        UsuarioHelper.Inicial(usuario).Should().Be("N");
    }

    [Fact]
    public void NombreCompletoVacioCaeAlUsername()
    {
        Usuario usuario = new() { Username = "nelson", NombreCompleto = "   " };

        UsuarioHelper.NombreMostrar(usuario).Should().Be("nelson");
        UsuarioHelper.Inicial(usuario).Should().Be("N");
    }

    [Fact]
    public void NombreCompletoNuloCaeAlUsername()
    {
        Usuario usuario = new() { Username = "npinio", NombreCompleto = null! };

        UsuarioHelper.NombreMostrar(usuario).Should().Be("npinio");
        UsuarioHelper.Inicial(usuario).Should().Be("N");
    }

    [Fact]
    public void SinNombresUtilesQuedaUnTextoPorDefecto()
    {
        Usuario usuario = new() { Username = "   ", NombreCompleto = "  " };

        UsuarioHelper.NombreMostrar(usuario).Should().Be(UsuarioHelper.SinNombre);
        UsuarioHelper.Inicial(usuario).Should().Be("U");
    }

    [Theory]
    [InlineData("nelson", "N")]
    [InlineData("NELSON", "N")]
    [InlineData("a", "A")]
    [InlineData("8usuario", "8")]
    public void LaInicialEsLaPrimeraLetraEnMayuscula(string username, string esperado)
    {
        Usuario usuario = new() { Username = username, NombreCompleto = " " };

        UsuarioHelper.Inicial(usuario).Should().Be(esperado);
    }

    [Fact]
    public void LosEspaciosAlRededorNoCuentanParaLaInicial()
    {
        // Con espacios, Substring(0,1) devolveria un espacio en blanco y el avatar
        // quedaria sin letra.
        Usuario usuario = new() { Username = "nelson", NombreCompleto = "   Nelson Pino   " };

        UsuarioHelper.NombreMostrar(usuario).Should().Be("Nelson Pino");
        UsuarioHelper.Inicial(usuario).Should().Be("N");
    }

    [Fact]
    public void NingunCasoDevuelveCadenaVacia()
    {
        Usuario?[] casos =
        [
            null,
            new Usuario(),
            new Usuario { Username = "", NombreCompleto = "" },
            new Usuario { Username = "u", NombreCompleto = " " }
        ];

        foreach (Usuario? usuario in casos)
        {
            UsuarioHelper.NombreMostrar(usuario).Should().NotBeNullOrWhiteSpace();
            UsuarioHelper.Inicial(usuario).Should().NotBeNullOrWhiteSpace();
        }
    }

    // ── Login y correo ────────────────────────────────────────────────────

    [Fact]
    public void SinSesionElLoginYElCorreoEstanVacios()
    {
        // Cadena vacia, no "Sin sesion": quien pinta el label la oculta en ese caso
        // y no queda un renglon de mas en la barra.
        UsuarioHelper.Login(null).Should().BeEmpty();
        UsuarioHelper.Correo(null).Should().BeEmpty();
    }

    [Fact]
    public void LoginYCorreoSeRecortan()
    {
        Usuario usuario = new()
        {
            Username = "  npinio ",
            Email = "  nelson.pino@ritrama.cl  "
        };

        UsuarioHelper.Login(usuario).Should().Be("npinio");
        UsuarioHelper.Correo(usuario).Should().Be("nelson.pino@ritrama.cl");
    }

    [Fact]
    public void CorreoNuloYSoloEspaciosQuedanVacios()
    {
        UsuarioHelper.Correo(new Usuario { Email = null! }).Should().BeEmpty();
        UsuarioHelper.Correo(new Usuario { Email = "   " }).Should().BeEmpty();
    }

    // ── Tooltip de la barra ───────────────────────────────────────────────

    [Fact]
    public void TooltipSinSesionSoloDiceSinSesion()
    {
        UsuarioHelper.Tooltip(null).Should().Be(UsuarioHelper.SinSesion);
    }

    [Fact]
    public void TooltipLlevaNombreLoginYCorreo()
    {
        Usuario usuario = new()
        {
            Username = "npinio",
            NombreCompleto = "Nelson Pino",
            Email = "nelson.pino@ritrama.cl"
        };

        UsuarioHelper.Tooltip(usuario).Should().Be("Nelson Pino (@npinio) · nelson.pino@ritrama.cl");
    }

    [Fact]
    public void TooltipNoRepiteElLoginCuandoElNombreYaEsElUsuario()
    {
        // Sin nombre_completo la linea de arriba ya muestra "npinio": ponerlo otra
        // vez en el tooltip seria el mismo dato dos veces.
        Usuario usuario = new()
        {
            Username = "npinio",
            NombreCompleto = "  ",
            Email = "npinio@ritrama.cl"
        };

        UsuarioHelper.Tooltip(usuario).Should().Be("npinio · npinio@ritrama.cl");
    }

    [Fact]
    public void TooltipSinCorreoNoDejaElSeparadorColgando()
    {
        Usuario usuario = new() { Username = "npinio", NombreCompleto = "Nelson Pino" };

        UsuarioHelper.Tooltip(usuario).Should().Be("Nelson Pino (@npinio)");
    }
}
