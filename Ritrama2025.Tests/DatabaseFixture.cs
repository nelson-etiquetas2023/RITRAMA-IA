using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Services.ProduccionService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Fixture compartido que construye el servicio de produccion apuntando al SQL Server de pruebas/dev
/// y expone helpers de SQL para sembrar/limpiar datos de forma aislada.
/// </summary>
public class DatabaseFixture : IDisposable
{
    public string ConnectionString { get; }
    public ProduccionService Service { get; }

    public DatabaseFixture()
    {
        ConnectionString = TestConfiguration.ConnectionString;
        IConfiguration config = TestConfiguration.BuildServiceConfiguration();
        Service = new ProduccionService(
            new OrdenCorteService(config, new ConsecutivosService(config)),
            new ConsecutivosService(config),
            new ConsumoMasterService(config));
        // En entorno de pruebas (headless) el MessageBox bloquea; anulamos el reporte de errores.
        ServiceErrors.Report = _ => { };
    }

    public object? ExecuteScalar(string sql, params (string name, object value)[] parameters)
    {
        using SqlConnection conn = new SqlConnection(ConnectionString);
        using SqlCommand cmd = new SqlCommand(sql, conn);
        foreach ((string name, object value) p in parameters)
        {
            cmd.Parameters.AddWithValue(p.name, p.value);
        }

        conn.Open();
        return cmd.ExecuteScalar();
    }

    public int ExecuteNonQuery(string sql, params (string name, object value)[] parameters)
    {
        using SqlConnection conn = new SqlConnection(ConnectionString);
        using SqlCommand cmd = new SqlCommand(sql, conn);
        foreach ((string name, object value) p in parameters)
        {
            cmd.Parameters.AddWithValue(p.name, p.value);
        }

        conn.Open();
        return cmd.ExecuteNonQuery();
    }

    public void Dispose()
    {
    }
}
