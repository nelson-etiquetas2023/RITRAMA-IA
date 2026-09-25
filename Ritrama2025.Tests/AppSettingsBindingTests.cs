using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Ritrama2025.Core.Settings;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba de regresion del binding: AppSettings.ConnectionStrings debe enlazarse
/// desde la seccion "ConnectionStringsEnvironment" (igual que la estructura de appsettings.json),
/// no desde la seccion "ConnectionStrings" (que solo tiene DefaultConnection/RitramaConnection).
/// Sin esto, el resolver devolvia cadena vacia y el login fallaba con error de connection string.
/// </summary>
public class AppSettingsBindingTests
{
    [Fact]
    public void BindConfiguration_Root_EnlazaConnectionStringsDesdeConnectionStringsEnvironment()
    {
        IConfigurationRoot config = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ambiente"] = "Desarrollo",
                ["ConnectionStrings"] = "no_usar",
                ["ConnectionStrings:DefaultConnection"] = "data source=default",
                ["ConnectionStrings:RitramaConnection"] = "data source=ritrama",
                ["ConnectionStringsEnvironment:Desarrollo"] = "data source=dev",
                ["ConnectionStringsEnvironment:Produccion"] = "data source=prod",
                ["ConnectionStringsEnvironment:Testing"] = "data source=test",
                ["Consecutivos:OrdenCorte"] = "125368",
                ["Compilacion:Version"] = "1.01",
                ["Compilacion:Fecha"] = "2024-06-15 - 12:00",
            })
            .Build();

        ServiceCollection services = new ServiceCollection();
        services.AddSingleton<IConfiguration>(config);
        services.AddOptions<AppSettings>().BindConfiguration("").ValidateDataAnnotations();
        using ServiceProvider sp = services.BuildServiceProvider();

        IOptions<AppSettings> options = sp.GetRequiredService<IOptions<AppSettings>>();

        options.Value.Ambiente.Should().Be("Desarrollo");
        options.Value.ConnectionStrings.Desarrollo.Should().Be("data source=dev");
        options.Value.ConnectionStrings.Produccion.Should().Be("data source=prod");
        options.Value.ConnectionStrings.Testing.Should().Be("data source=test");
        options.Value.ConnectionStrings.GetConnectionString("Desarrollo").Should().Be("data source=dev");
        options.Value.Consecutivos.OrdenCorte.Should().Be(125368);
    }
}
