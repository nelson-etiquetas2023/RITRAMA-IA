using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Services.ProduccionService;

internal static class ConexionResolver
{
    public static string Resolver(IConfiguration config)
    {
        string ambiente = config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
        return config.GetSection("ConnectionStringsEnvironment")[ambiente]!;
    }
}
