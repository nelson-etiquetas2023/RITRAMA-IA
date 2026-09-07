using System.Data;
using System.Globalization;
using FluentAssertions;
using Ritrama2025.Core;
using Xunit;

namespace Ritrama2025.Tests;

public class DataTableHelperTests
{
    private sealed class ItemPrueba
    {
        public string Id { get; set; } = "";
        public decimal Precio { get; set; }
        public int? Cantidad { get; set; }
    }

    [Fact]
    public void ToDataTable_ListaConItems_CreaColumnasYFilas()
    {
        var dt = DataTableHelper.ToDataTable(new List<ItemPrueba>
        {
            new() { Id = "A", Precio = 1.5m, Cantidad = 3 },
            new() { Id = "B", Precio = 2.25m, Cantidad = null },
        });

        dt.Columns.Cast<DataColumn>().Should().HaveCount(3);
        dt.Rows.Cast<DataRow>().Should().HaveCount(2);
        DataColumn colId = dt.Columns["Id"]!;
        DataColumn colPrecio = dt.Columns["Precio"]!;
        DataColumn colCantidad = dt.Columns["Cantidad"]!;
        colId.DataType.Should().Be(typeof(string));
        colPrecio.DataType.Should().Be(typeof(decimal));
        colCantidad.DataType.Should().Be(typeof(int));
        dt.Rows[0]["Id"].Should().Be("A");
        dt.Rows[1]["Precio"].Should().Be(2.25m);
        dt.Rows[1]["Cantidad"].Should().Be(DBNull.Value);
    }

    [Fact]
    public void ToDataTable_ListaVacia_DevuelveTablaSinColumnas()
    {
        var dt = DataTableHelper.ToDataTable(new List<ItemPrueba>());

        dt.Rows.Cast<DataRow>().Should().BeEmpty();
        dt.Columns.Cast<DataColumn>().Should().BeEmpty();
    }

    [Fact]
    public void ToDataTable_ListaNula_DevuelveTablaVacia()
    {
        var dt = DataTableHelper.ToDataTable<ItemPrueba>(null!);

        dt.Should().NotBeNull();
        dt.Rows.Cast<DataRow>().Should().BeEmpty();
    }
}

public class RangoNumericoValidatorTests
{
    [Theory]
    [InlineData("subtotal", "9999999999999999.99", true)]
    [InlineData("kilo_total", "10000000000000000.00", false)]
    [InlineData("precio", "-10000000000000000.00", false)]
    public void Verificar_ValidaValoresDentroYFueraDeRango(string campo, string valorTexto, bool dentroDeRango)
    {
        decimal valor = decimal.Parse(valorTexto, CultureInfo.InvariantCulture);
        string? resultado = RangoNumericoValidator.Verificar(campo, valor);

        if (dentroDeRango)
        {
            resultado.Should().BeNull();
        }
        else
        {
            resultado.Should().NotBeNull();
            resultado.Should().Contain(campo);
            resultado.Should().Contain("fuera de rango");
        }
    }

    [Fact]
    public void PrimeraFueraDeRango_DevuelveLaPrimeraCoincidencia()
    {
        var valores = new (string Campo, decimal Valor)[]
        {
            ("subtotal", 100m),
            ("total$rd", 999999999999999999m),
            ("itbis", 200m),
        };

        string? r = RangoNumericoValidator.PrimeraFueraDeRango(valores);

        r.Should().NotBeNull();
        r.Should().Contain("total$rd");
        r.Should().NotContain("itbis");
    }

    [Fact]
    public void PrimeraFueraDeRango_SinValoresFuera_DevuelveNull()
    {
        var valores = new (string Campo, decimal Valor)[]
        {
            ("subtotal", 100m),
            ("itbis", 18m),
        };

        RangoNumericoValidator.PrimeraFueraDeRango(valores).Should().BeNull();
    }

    [Fact]
    public void PrimeraFueraDeRango_ListaVacia_DevuelveNull()
    {
        RangoNumericoValidator.PrimeraFueraDeRango([]).Should().BeNull();
    }
}