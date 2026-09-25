using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.InventarioService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de integracion de las APIs batch de inventario (evitan el N+1 de validar
/// roll-ids / product-ids / borrados fila por fila en las importaciones de Excel).
/// </summary>
[Trait("Categoria", "Integracion")]
public class InventarioServiceBatchTests : IClassFixture<DatabaseFixture>
{
    private readonly DatabaseFixture _fixture;

    public InventarioServiceBatchTests(DatabaseFixture fixture)
    {
        _fixture = fixture;
    }

    private IInventarioService CrearServicio()
    {
        IConfiguration options = TestConfiguration.BuildServiceOptions();
        return new InventarioService(options);
    }

    [SkippableFact]
    public void GetExistingProductIds_DevuelveSoloLosQueExisten()
    {
        IInventarioService service = CrearServicio();
        object? obj = _fixture.ExecuteScalar("SELECT TOP 1 product_id FROM producto");
        Skip.If(obj == null, "no hay productos; validado en prueba manual");

        string existente = obj.ToString()!;
        string inexistente = "NO_EXISTE_" + Guid.NewGuid().ToString("N")[..10];

        List<string> resultado = service.GetExistingProductIds([existente, inexistente]);

        resultado.Should().Contain(existente);
        resultado.Should().NotContain(inexistente);
    }

    [SkippableFact]
    public void GetExistingRollIds_DevuelveSoloLosQueExisten()
    {
        IInventarioService service = CrearServicio();
        object? obj = _fixture.ExecuteScalar("SELECT TOP 1 roll_id FROM MasterInic");
        Skip.If(obj == null, "no hay masters; validado en prueba manual");

        string existente = obj.ToString()!;
        string inexistente = "NO_EXISTE_" + Guid.NewGuid().ToString("N")[..10];

        List<string> resultado = service.GetExistingRollIds([existente, inexistente]);

        resultado.Should().Contain(existente);
        resultado.Should().NotContain(inexistente);
    }

    [SkippableFact]
    public void GetExistingRollIds_MilesDeIds_NoLanzaYDevuelveExistentes()
    {
        IInventarioService service = CrearServicio();
        object? obj = _fixture.ExecuteScalar("SELECT TOP 1 roll_id FROM MasterInic");
        Skip.If(obj == null, "no hay masters; validado en prueba manual");

        string existente = obj.ToString()!;
        List<string> ids = new List<string> { existente };
        for (int i = 0; i < 1100; i++)
        {
            ids.Add("FAKE_" + i.ToString("D5"));
        }

        List<string> resultado = service.GetExistingRollIds(ids);

        resultado.Should().Contain(existente);
        resultado.Should().HaveCount(1);
    }

    [SkippableFact]
    public void BorrarMastersDB_EliminaVariosEnUnSoloLote()
    {
        IInventarioService service = CrearServicio();
        string rollA = "TEST_BATCH_A_" + Guid.NewGuid().ToString("N")[..10];
        string rollB = "TEST_BATCH_B_" + Guid.NewGuid().ToString("N")[..10];
        string product = "TEST_BATCH_PROD_" + Guid.NewGuid().ToString("N")[..10];

        try
        {
            service.SaveMasterInitialDB(new ProductMAP
            {
                Product_Id = product,
                Product_Name = "Test Batch",
                Rollid = rollA,
                Ubic = "TST",
                Factura = "",
                Fecha_Produccion = DateTime.Today,
                Fecha_Llegada = DateTime.Today,
                Paleta = "",
            });
            service.SaveMasterInitialDB(new ProductMAP
            {
                Product_Id = product,
                Product_Name = "Test Batch",
                Rollid = rollB,
                Ubic = "TST",
                Factura = "",
                Fecha_Produccion = DateTime.Today,
                Fecha_Llegada = DateTime.Today,
                Paleta = "",
            });

            bool resultado = service.BorrarMastersDB([rollA, rollB]);

            resultado.Should().BeTrue();
            Convert.ToInt32(_fixture.ExecuteScalar(
                "SELECT COUNT(*) FROM MasterInic WHERE roll_id IN (@p1,@p2)",
                ("@p1", rollA), ("@p2", rollB))!).Should().Be(0);
        }
        finally
        {
            _fixture.ExecuteNonQuery(
                "DELETE FROM MasterInic WHERE roll_id IN (@p1,@p2)",
                ("@p1", rollA), ("@p2", rollB));
            _fixture.ExecuteNonQuery("DELETE FROM producto WHERE product_id = @p1", ("@p1", product));
        }
    }
}
