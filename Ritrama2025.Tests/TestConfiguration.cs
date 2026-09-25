using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Tests;

/// <summary>
/// Resuelve la cadena de conexion del entorno de pruebas/dev.
/// Prioridad: variable de entorno RITRAMA_TEST_CONNECTION, luego appsettings.Test.json.
/// </summary>
public static class TestConfiguration
{
    public static string ConnectionString
    {
        get
        {
            string? env = Environment.GetEnvironmentVariable("RITRAMA_TEST_CONNECTION");
            if (!string.IsNullOrWhiteSpace(env))
            {
                return env;
            }

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json", optional: true)
                .Build();

            string? cs = config["ConnectionStringsEnvironment:Desarrollo"];
            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException(
                    "No se encontro la cadena de conexion. Define la variable de entorno RITRAMA_TEST_CONNECTION " +
                    "o crea Ritrama2025.Tests/appsettings.Test.json con la cadena de conexion de desarrollo.");
            }

            return cs;
        }
    }

    public static IConfiguration BuildServiceConfiguration()
    {
        return new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["Ambiente"] = "Desarrollo",
                ["ConnectionStringsEnvironment:Desarrollo"] = ConnectionString,
                ["ConnectionStringsEnvironment:Produccion"] = ConnectionString,
                ["ConnectionStringsEnvironment:Testing"] = ConnectionString
            })
            .Build();
    }

    public static IConfiguration BuildServiceOptions()
    {
        return BuildServiceConfiguration();
    }
}









































































































































