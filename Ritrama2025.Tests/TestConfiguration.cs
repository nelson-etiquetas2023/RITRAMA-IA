using Microsoft.Extensions.Configuration;
using System.Collections.Generic;

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
            var env = Environment.GetEnvironmentVariable("RITRAMA_TEST_CONNECTION");
            if (!string.IsNullOrWhiteSpace(env))
            {
                return env;
            }

            var config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json", optional: true)
                .Build();

            var cs = config["ConnectionStringsEnvironment:Desarrollo"];
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
                ["ConnectionStringsEnvironment:Desarrollo"] = ConnectionString
            })
            .Build();
    }
}
