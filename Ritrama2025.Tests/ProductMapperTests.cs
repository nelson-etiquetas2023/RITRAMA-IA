using System.Data;
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.ProductsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Lectura del catálogo producto (<see cref="ProductMapper.FromDataRow"/>) con la
/// convención vigente: product_id = código de empresa (lo que busca el inventario y las
/// órdenes de corte) e IdConsec = consecutivo del sistema. Ya no existe la columna
/// codigo_ritrama: el mapper no debe mirarla.
/// </summary>
[Trait("Categoria", "Unit")]
public class ProductMapperTests
{
    private static DataRow FilaCatalogo()
    {
        DataTable tabla = new("producto");
        tabla.Columns.Add("product_id", typeof(string));
        tabla.Columns.Add("idconsec", typeof(int));
        tabla.Columns.Add("product_name", typeof(string));
        tabla.Columns.Add("product_descrip", typeof(string));
        tabla.Columns.Add("product_ref", typeof(string));
        tabla.Columns.Add("codebar", typeof(string));
        tabla.Columns.Add("masterRolls", typeof(int));
        tabla.Columns.Add("rollo_cortado", typeof(int));
        tabla.Columns.Add("resmas", typeof(int));
        tabla.Columns.Add("graphics", typeof(int));
        tabla.Columns.Add("anulado", typeof(int));
        tabla.Columns.Add("precio", typeof(decimal));
        tabla.Columns.Add("costo", typeof(decimal));
        tabla.Columns.Add("code_rc", typeof(string));
        tabla.Columns.Add("ratio", typeof(decimal));

        DataRow fila = tabla.NewRow();
        fila["product_id"] = "00753";
        fila["idconsec"] = 1;
        fila["product_name"] = "00753-500 (RI-705/60 PP Gloss)";
        fila["product_descrip"] = DBNull.Value;
        fila["product_ref"] = DBNull.Value;
        fila["codebar"] = DBNull.Value;
        fila["masterRolls"] = 1;
        fila["rollo_cortado"] = 0;
        fila["resmas"] = 0;
        fila["graphics"] = 0;
        fila["anulado"] = 0;
        fila["precio"] = 100.00m;
        fila["costo"] = 50.00m;
        fila["code_rc"] = DBNull.Value;
        fila["ratio"] = 0m;
        tabla.Rows.Add(fila);
        return tabla.Rows[0];
    }

    [Fact]
    public void FromDataRow_ElCodigoEsElProduct_IDYElConsecutivoVaEnIdConsec()
    {
        Product producto = ProductMapper.FromDataRow(FilaCatalogo());

        producto.Product_id.Should().Be("00753",
            "el product_id es el codigo de empresa, que es lo que cruzan inventario y OC");
        producto.IdConsec.Should().Be(1, "el consecutivo del sistema vive en su propia columna");
        producto.Product_Name.Should().Be("00753-500 (RI-705/60 PP Gloss)");
        producto.Master.Should().BeTrue();
        producto.Anulado.Should().BeFalse();
        producto.Precio.Should().Be(100.00m);
    }

    [Fact]
    public void FromDataRow_SinIdConsec_NoTiraExcepcion()
    {
        // Una fila antigua (o una consulta que no pida la columna) no debe romper la lectura.
        DataRow fila = FilaCatalogo();
        fila["idconsec"] = DBNull.Value;

        ProductMapper.FromDataRow(fila).IdConsec.Should().Be(0);
    }
}
