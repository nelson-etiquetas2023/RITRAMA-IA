using System;
using System.ComponentModel.DataAnnotations;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Core.Settings
{
    public sealed class AppSettings
    {
        public required string Ambiente { get; init; }

        [ConfigurationKeyName(ConnectionStringsSettings.SectionName)]
        public required ConnectionStringsSettings ConnectionStrings { get; init; }

        public required ConsecutivosSettings Consecutivos { get; init; }

        public required CompilacionSettings Compilacion { get; init; }
    }

    public sealed class ConnectionStringsSettings
    {
        public const string SectionName = "ConnectionStringsEnvironment";

        [Required]
        public required string Desarrollo { get; init; }

        [Required]
        public required string Produccion { get; init; }

        [Required]
        public required string Testing { get; init; }

        public string GetConnectionString(string ambiente)
        {
            return ambiente switch
            {
                "Desarrollo" => Desarrollo,
                "Produccion" => Produccion,
                "Testing" => Testing,
                _ => throw new ArgumentException($"Ambiente desconocido: {ambiente}. Valores válidos: Desarrollo, Produccion, Testing")
            };
        }
    }

    public sealed class ConsecutivosSettings
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int OrdenCorte { get; init; }
    }

    public sealed class CompilacionSettings
    {
        public required string Version { get; init; }
        public required string Fecha { get; init; }
    }
}
