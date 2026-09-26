using System.Collections.Generic;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Tests;

/// <summary>
/// Resuelve la cadena de conexion del entorno de pruebas/dev.
/// Prioridad: variable de entorno RITRAMA_TEST_CONNECTION, luego appsettings.Test.json.
/// </summary>
public static class TestConfiguration
{
    /// <summary>
    /// Bases que se consideran de pruebas. Cualquier otra descarta la ejecucion: las pruebas
    /// hacen INSERT, UPDATE y DELETE sin rollback, asi que apuntarlas a produccion escribe
    /// datos de prueba en los datos reales.
    /// </summary>
    private static readonly string[] BasesPermitidas = { "RITRAMA2025-TEST", "RITRAMA2025_TEST" };

    public static string ConnectionString
    {
        get
        {
            string? env = Environment.GetEnvironmentVariable("RITRAMA_TEST_CONNECTION");
            if (!string.IsNullOrWhiteSpace(env))
            {
                VerificarQueNoSeaProduccion(env);
                return env;
            }

            IConfigurationRoot config = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.Test.json", optional: true)
                .Build();

            // Se lee la clave de pruebas, nunca la de desarrollo. Antes caia en Desarrollo, que
            // en este proyecto apunta a la MISMA base que Produccion, y por eso las pruebas
            // escribian en los datos reales.
            string? cs = config["ConnectionStringsEnvironment:Testing"]
                ?? Environment.GetEnvironmentVariable("RITRAMA_TEST_CONNECTION");

            if (string.IsNullOrWhiteSpace(cs))
            {
                throw new InvalidOperationException(
                    "No se encontro la cadena de conexion de pruebas. Define la variable de entorno "
                    + "RITRAMA_TEST_CONNECTION, o crea Ritrama2025.Tests/appsettings.Test.json con la "
                    + "clave ConnectionStringsEnvironment:Testing apuntando a una base de pruebas "
                    + "(por ejemplo RITRAMA2025-TEST). A proposito no se usa la de desarrollo: en este "
                    + "proyecto esa es la misma base que produccion.");
            }

            VerificarQueNoSeaProduccion(cs);
            return cs;
        }
    }

    /// <summary>
    /// Corta la ejecucion si la base no es una de pruebas. Las pruebas de servicio siembran,
    /// modifican y borran datos reales, y la fixture no abre transaccion: cada sentencia
    /// committea sola. Sin este control, un appsettings.Test.json mal puesto deja basura en
    /// produccion y nadie se entera hasta que aparece un pedido de prueba en la consulta.
    /// </summary>
    private static void VerificarQueNoSeaProduccion(string connectionString)
    {
        string baseDeDatos = ExtraerBaseDeDatos(connectionString);
        bool esDePruebas = BasesPermitidas.Any(b => baseDeDatos.Contains(b, StringComparison.OrdinalIgnoreCase));

        if (!esDePruebas)
        {
            throw new InvalidOperationException(
                "Las pruebas se detienen: la cadena apunta a la base '" + baseDeDatos + "', que no es de pruebas. "
                + "Se permiten: " + string.Join(", ", BasesPermitidas) + ". "
                + "Las pruebas hacen INSERT, UPDATE y DELETE sin rollback, asi que apuntarlas a otra base "
                + "escribe datos de prueba en los datos reales. Define RITRAMA_TEST_CONNECTION con una base de pruebas.");
        }
    }

    private static string ExtraerBaseDeDatos(string connectionString)
    {
        foreach (string parte in connectionString.Split(';', StringSplitOptions.RemoveEmptyEntries))
        {
            string[] par = parte.Split('=', 2);
            if (par.Length == 2 && par[0].Trim().Equals("Initial Catalog", StringComparison.OrdinalIgnoreCase))
            {
                return par[1].Trim();
            }
        }

        return "(no se pudo leer)";
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









































































































































