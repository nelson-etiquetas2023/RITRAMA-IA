using FluentAssertions;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProductsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Reglas de dominio del catálogo de productos bajo la convención vigente: el código
/// Ritrama que teclea el usuario ES el product_id (clave primaria, nvarchar(25) en las
/// dos bases) y el consecutivo del sistema (IdConsec) es solo referencial. De ahi sale
/// que el validador exija y limite el codigo, y que el consecutivo no pueda sustituirlo.
/// </summary>
[Trait("Categoria", "Unit")]
public class ProductValidatorTests
{
    private static Product ProductoValido(string codigo = "00753") => new()
    {
        Product_id = codigo,
        IdConsec = 1,
        Product_Name = "00753-500 (RI-705/60 PP Gloss)",
        Product_Description = string.Empty,
        Referencia = string.Empty,
        Codigo_Barra = string.Empty,
        Precio = 0m,
        Costo = 0m,
        Ratio = 0m,
        Anulado = false,
        Master = true
    };

    [Fact]
    public void UnProductoConTodoElDatoValido_PasaLaValidacion()
    {
        ProductValidator.ValidateForPersistence(ProductoValido()).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UnCodigoVacio_NoPasa_PorqueElCodigoEsElProduct_ID()
    {
        Product producto = ProductoValido(codigo: "   ");

        Result resultado = ProductValidator.ValidateForPersistence(producto);

        resultado.IsSuccess.Should().BeFalse("sin codigo no hay clave primaria que guardar");
        resultado.ErrorCode.Should().Be(ProductValidator.CODE_REQUIRED);
        resultado.Error.Should().Contain("código Ritrama");
    }

    [Fact]
    public void UnConsecutivoRelleno_NoSalvaAUnProductoSinCodigo()
    {
        // El IdConsec no identifica nada por si solo: si el codigo falta, el consecutivo
        // relleno no deberia dejar pasar el producto.
        Product producto = ProductoValido(codigo: string.Empty);
        producto.IdConsec = 99;

        ProductValidator.ValidateForPersistence(producto).IsSuccess.Should().BeFalse();
    }

    [Fact]
    public void UnCodigoDe25Caracteres_PasaPorqueEsLoQueCabeEnLaColumna()
    {
        // product_id y MasterInic.part_number son nvarchar(25): a partir de ahi el INSERT
        // o el guardado del inventario inicial truncan en silencio.
        Product producto = ProductoValido(new string('C', ProductValidator.MaxCodigoRitrama));

        ProductValidator.ValidateForPersistence(producto).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UnCodigoDeMasDe25Caracteres_NoPasa()
    {
        Product producto = ProductoValido(new string('C', ProductValidator.MaxCodigoRitrama + 1));

        Result resultado = ProductValidator.ValidateForPersistence(producto);

        resultado.IsSuccess.Should().BeFalse();
        resultado.Error.Should().Contain(ProductValidator.MaxCodigoRitrama.ToString());
    }

    [Fact]
    public void UnCodigoConEspaciosALosLados_SeValidaRecortado()
    {
        // El formulario recorta antes de validar; aqui se comprueba que el propio
        // validador no rechaza un codigo bueno por los espacios de la tecleada.
        Product producto = ProductoValido(" 00753 ");

        ProductValidator.ValidateForPersistence(producto).IsSuccess.Should().BeTrue();
    }

    [Fact]
    public void UnaCategoriaNoExclusiva_NoPasa()
    {
        Product producto = ProductoValido();
        producto.RolloCortado = true; // Master ya esta activo: dos categorias a la vez.

        Result resultado = ProductValidator.ValidateForPersistence(producto);

        resultado.IsSuccess.Should().BeFalse();
        resultado.ErrorCode.Should().Be(ProductValidator.CODE_CATEGORY);
    }
}
