using System.Data;
using FluentAssertions;
using Ritrama2025.Services.ClienteService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba de integración del listado de clientes contra la base de pruebas
/// (RITRAMA2025-TEST vía RITRAMA_TEST_CONNECTION): valida el esquema que espera
/// el grid y que el status traduzca a activo/desactivado.
/// </summary>
[Trait("Categoria", "Integracion")]
public class ClienteServiceTests
{
    [Fact]
    public async Task LoadListadoAsync_RetornaColumnasEsperadasYStatusValidos()
    {
        IClienteService servicio = new ClienteService(TestConfiguration.BuildServiceConfiguration());

        DataTable listado = await servicio.LoadListadoAsync();

        listado.Columns.Contains("customer_id").Should().BeTrue();
        listado.Columns.Contains("customer_name").Should().BeTrue();
        listado.Columns.Contains("status").Should().BeTrue();
        foreach (DataRow fila in listado.Rows)
        {
            fila["status"].Should().BeOneOf("activo", "desactivado");
        }
    }
}
