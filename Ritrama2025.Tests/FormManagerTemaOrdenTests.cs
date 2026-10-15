using FluentAssertions;
using Xunit;

namespace Ritrama2025.Tests;

// Bug: flash naranja al abrir Producción (FrmOrdenCorte).
// El primer frame visible se pintaba con el default naranja de SunnyUI
// porque FormManager mostraba el form ANTES de aplicar el tema verde.
// Regression lock: el tema debe aplicarse ANTES del primer Show() visible.
public sealed class FormManagerTemaOrdenTests
{
    private static string LeerFormManager()
    {
        string dir = AppDomain.CurrentDomain.BaseDirectory;
        for (int i = 0; i < 8; i++)
        {
            string candidato = Path.Combine(dir, "Helpers", "FormManager.cs");
            if (File.Exists(candidato))
            {
                return File.ReadAllText(candidato);
            }
            string? padre = Path.GetDirectoryName(dir.TrimEnd(Path.DirectorySeparatorChar));
            if (string.IsNullOrEmpty(padre))
            {
                break;
            }
            dir = padre;
        }
        // Fallback: ruta relativa desde el output de tests.
        string fallback = Path.GetFullPath(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "..", "..", "..", "..", "Helpers", "FormManager.cs"));
        return File.ReadAllText(fallback);
    }

    [Fact]
    public void CargarYMostrarAsync_AplicaTemaAntesDelPrimerShow()
    {
        string src = LeerFormManager();
        int metodo = src.IndexOf("async Task CargarYMostrarAsync", StringComparison.Ordinal);
        metodo.Should().BeGreaterThanOrEqualTo(0, "debe existir CargarYMostrarAsync en FormManager");

        string cuerpo = src.Substring(metodo);
        int show = cuerpo.IndexOf("form.Show()", StringComparison.Ordinal);
        int tema = cuerpo.IndexOf("ReaplicarTema()", StringComparison.Ordinal);
        if (tema < 0)
        {
            tema = cuerpo.IndexOf("TemaOscuroHelper.Aplicar", StringComparison.Ordinal);
        }

        show.Should().BeGreaterThanOrEqualTo(0, "CargarYMostrarAsync debe mostrar el form");
        tema.Should().BeGreaterThanOrEqualTo(0, "CargarYMostrarAsync debe aplicar el tema");
        tema.Should().BeLessThan(show,
            "el tema (ReaplicarTema/TemaOscuroHelper) debe aplicarse ANTES del primer form.Show(), "
            + "si no el primer frame se pinta naranja y luego repinta (flash)");
    }
}
