using System.Data;
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas unitarias del mapeo del detalle. No abren conexiones: la tabla se arma en memoria.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoDetalleMapperTests
{
    private static DataTable TablaDetalle()
    {
        DataTable tabla = new();
        tabla.Columns.Add("numero", typeof(string));
        tabla.Columns.Add("product_id", typeof(string));
        tabla.Columns.Add("product_name", typeof(string));
        tabla.Columns.Add("cant", typeof(decimal));
        tabla.Columns.Add("unidad", typeof(string));
        tabla.Columns.Add("width", typeof(decimal));
        tabla.Columns.Add("lenght", typeof(decimal));
        tabla.Columns.Add("msi", typeof(decimal));
        tabla.Columns.Add("precio", typeof(decimal));
        tabla.Columns.Add("total_renglon", typeof(decimal));
        tabla.Columns.Add("notas", typeof(string));
        return tabla;
    }

    [Fact]
    public void Mapear_ConservaElProductId()
    {
        DataTable tabla = TablaDetalle();
        tabla.Rows.Add("SO-01001", "P-0001", "Rollo 60", 3m, "Rollo", 60m, 1000m, 1m, 25.50m, 76.50m, "corte especial");

        List<PedidoDetalle> lineas = PedidoDetalleMapper.Mapear(tabla);

        lineas.Should().HaveCount(1);
        lineas[0].Product_id.Should().Be("P-0001");
        lineas[0].Product_name.Should().Be("Rollo 60");
        lineas[0].Cant.Should().Be(3m);
        lineas[0].Unidad.Should().Be("Rollo");
        lineas[0].Precio.Should().Be(25.50m);
        lineas[0].Total_Renglon.Should().Be(76.50m);
        lineas[0].Notas.Should().Be("corte especial");
    }

    [Fact]
    public void Mapear_ValoresDBNull_LosConvierteEnCeroONulo()
    {
        DataTable tabla = TablaDetalle();
        tabla.Rows.Add("SO-01001", "P-0001", "Rollo 60", 1m, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value, DBNull.Value);

        List<PedidoDetalle> lineas = PedidoDetalleMapper.Mapear(tabla);

        lineas[0].Unidad.Should().BeNull();
        lineas[0].Width.Should().Be(0m);
        lineas[0].Precio.Should().BeNull();
        lineas[0].Total_Renglon.Should().BeNull();
    }

    [Fact]
    public void Mapear_TablaVacia_DevuelveListaVacia()
    {
        PedidoDetalleMapper.Mapear(TablaDetalle()).Should().BeEmpty();
    }

    [Fact]
    public void Mapear_TablaNula_DevuelveListaVacia()
    {
        PedidoDetalleMapper.Mapear(null!).Should().BeEmpty();
    }
}
