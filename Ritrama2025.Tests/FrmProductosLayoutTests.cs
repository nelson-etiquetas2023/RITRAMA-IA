using System.Data;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.ProductsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño de FrmProductos (paneles 30/70).
/// No toca base de datos: construye el formulario con un stub de IProductsService para
/// comprobar que el disenador enlaza buscador, grid de 3 columnas, contador y pestana de detalle.
/// </summary>
public class FrmProductosLayoutTests
{
    /// <summary>Servicio minimo para poder instanciar el formulario sin dependencias reales.</summary>
    private sealed class ProductsServiceStub : IProductsService
    {
        public Task<DataSet> Load(CancellationToken cancellationToken = default)
            => Task.FromResult(new DataSet());

        public Task<bool> Add(Product producto) => Task.FromResult(true);

        public Task<bool> Update(Product producto) => Task.FromResult(true);

        public bool Anular(string IdProduct) => true;

        public bool ValidProductid(string id) => false;

        public Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(false));

        public Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyList<Product>>.Success(Array.Empty<Product>()));

        public Result ValidateProduct(Product producto) => Result.Success();
    }

    private static FrmProductos CrearFormulario() => new(new ProductsServiceStub(), new ConfigurationBuilder().Build());

    [Fact]
    public void Raiz_DivideElAnchoEnTreintaYSetentaPorCiento()
    {
        using FrmProductos form = CrearFormulario();

        TableLayoutPanel root = (TableLayoutPanel)form.Controls.Find("tlpRoot", true).Single();

        root.ColumnCount.Should().Be(2);
        root.ColumnStyles[0].SizeType.Should().Be(SizeType.Percent);
        root.ColumnStyles[0].Width.Should().Be(30f);
        root.ColumnStyles[1].Width.Should().Be(70f);
    }

    [Fact]
    public void PanelIzquierdo_TieneBuscadorGridDeTresColumnasYContador()
    {
        using FrmProductos form = CrearFormulario();

        form.Controls.Find("txtSearch", true).Should().ContainSingle();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
        grid.Columns.Count.Should().Be(3);
        grid.Columns[0].DataPropertyName.Should().Be("ProductId");
        grid.Columns[1].DataPropertyName.Should().Be("ProductName");
        grid.Columns[2].DataPropertyName.Should().Be("Tipo");
        grid.ReadOnly.Should().BeTrue();
        grid.AllowUserToAddRows.Should().BeFalse();

        Control contador = form.Controls.Find("lblContador", true).Single();
        contador.Text.Should().StartWith("Productos existentes");
    }

    [Fact]
    public void PanelDerecho_EmpiezaConLaPestanaDeDetalle()
    {
        using FrmProductos form = CrearFormulario();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleProducto", true).Single();

        detalle.Text.Should().Be("Detalle");
        form.Controls.Find("txtDetId", true).Should().ContainSingle();
        form.Controls.Find("txtDetDescripcion", true).Should().ContainSingle();
    }
}
