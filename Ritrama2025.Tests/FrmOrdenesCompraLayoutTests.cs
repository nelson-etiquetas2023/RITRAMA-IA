using System.Data;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.OrdenesCompras;
using Ritrama2025.Services.ProductsService;
using FluentAssertions;
using Xunit;

namespace Ritrama2025.Tests
{
    /// <summary>
    /// Layout de <see cref="FrmOrdenesCompra"/>. El modulo se quejaba de que la barra de
    /// herramientas salia cortada: venia del disenador con botones fijos de 92x24 e
    /// <c>ImageScaling = None</c>, y el icono de 32px de "Nuevo" no cabia con el texto.
    /// El ajuste se hace en codigo (Helpers/ToolStripTheme), asi que se verifica aqui
    /// que el formulario realmente lo aplique.
    /// </summary>
    public class FrmOrdenesCompraLayoutTests
    {
        private static FrmOrdenesCompra CrearFormulario()
        {
            IConfiguration configuracion = new ConfigurationBuilder().Build();
            return new FrmOrdenesCompra(new OrdenesComprasStub(), new ProductsServiceStub(), configuracion);
        }

        [Fact]
        public void Barra_HaSidoNormalizadaPorElFormulario()
        {
            using FrmOrdenesCompra form = CrearFormulario();
            ToolStrip barra = form.Controls.Find("barraHerramientas", true).Cast<ToolStrip>().Single();

            barra.ImageScalingSize.Should().Be(Helpers.ToolStripTheme.DefaultIconSize);

            foreach (ToolStripItem item in barra.Items)
            {
                ToolStripButton boton = Assert.IsType<ToolStripButton>(item);
                boton.AutoSize.Should().BeTrue(boton.Name + " debe auto-ajustarse al icono y al texto");
                boton.ImageScaling.Should().Be(ToolStripItemImageScaling.SizeToFit);
            }
        }

        [Fact]
        public void Barra_OcupaTodoElAnchoDeLaPestanaParaNoCortarse()
        {
            using FrmOrdenesCompra form = CrearFormulario();
            form.CreateControl();
            form.PerformLayout();

            ToolStrip barra = form.Controls.Find("barraHerramientas", true).Cast<ToolStrip>().Single();

            // Docked o anclada a los dos lados: con ancho fijo (773 px del disenador) el
            // borde derecho se salia de la pestana al redimensionar el formulario.
            bool sigueElAncho = barra.Dock == DockStyle.Top
                || (barra.Anchor.HasFlag(AnchorStyles.Left) && barra.Anchor.HasFlag(AnchorStyles.Right));
            sigueElAncho.Should().BeTrue("la barra debe seguir el ancho de la pestana");

            Control padre = barra.Parent!;
            int anchoUtil = padre.ClientSize.Width - padre.Padding.Horizontal;
            barra.Width.Should().BeGreaterThanOrEqualTo(anchoUtil);
        }

        [Fact]
        public void Barra_MideAlMenosLoQueMidenSusBotonesVisibles()
        {
            using FrmOrdenesCompra form = CrearFormulario();
            form.CreateControl();
            form.PerformLayout();

            ToolStrip barra = form.Controls.Find("barraHerramientas", true).Cast<ToolStrip>().Single();

            // Ni el alto de la barra ni el ancho de cada boton pueden quedar por debajo
            // de lo que necesitan: eso es exactamente lo que se veía recortado.
            barra.Height.Should().BeGreaterThanOrEqualTo(barra.PreferredSize.Height);
            foreach (ToolStripButton boton in barra.Items.OfType<ToolStripButton>())
            {
                if (!boton.Visible)
                {
                    continue;
                }

                int anchoTexto = TextRenderer.MeasureText(boton.Text ?? string.Empty, boton.Font).Width;
                boton.Width.Should().BeGreaterThanOrEqualTo(
                    anchoTexto + barra.ImageScalingSize.Width,
                    "el texto de " + boton.Name + " no debe quedar cortado");
            }
        }

        private sealed class OrdenesComprasStub : IOrdenesComprasService
        {
            public string? ErrorMsg => null;

            public Task<DataTable> LoadDataOrdenesCompra(CancellationToken ct = default) => Task.FromResult(new DataTable());

            public Task<DataTable> LoadDataProveedores(CancellationToken ct = default) => Task.FromResult(new DataTable());

            public Task<ProveedorDatos?> BuscarProveedorAsync(string? proveedorId, CancellationToken ct = default) => Task.FromResult<ProveedorDatos?>(null);

            public Task<DataTable> LoadDataOrdenCompraDetalle(string numero, CancellationToken ct = default) => Task.FromResult(new DataTable());

            public Task<string> GetProximoNumeroOrdenCompra(CancellationToken ct = default) => Task.FromResult("OC-00001");

            public bool SaveOrdenCompraCompleto(OrdenCompra orden) => true;

            public bool ActualizarOrdenCompraCompleto(OrdenCompra orden) => true;

            public bool AnularOrdenCompra(string numero) => true;

            public bool RestaurarOrdenCompra(string numero) => true;

            public bool ActualizarEstadoOrdenCompra(string numero, string estado) => true;
        }

        private sealed class ProductsServiceStub : IProductsService
        {
            public Task<DataSet> Load(CancellationToken cancellationToken = default) => Task.FromResult(new DataSet());

            public Task<bool> Add(Product producto) => Task.FromResult(true);

            public Task<bool> Update(Product producto) => Task.FromResult(true);

            public bool Anular(string IdProduct) => true;

            public bool ValidProductid(string id) => true;

            public Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default) => Task.FromResult(Result<bool>.Success(true));

            public Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default) => Task.FromResult(Result<bool>.Success(true));

            public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default) => Task.FromResult(Result<bool>.Success(true));

            public Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default) => Task.FromResult(Result<bool>.Success(true));

            public Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default) => Task.FromResult(Result<IReadOnlyList<Product>>.Success([]));

            public Result ValidateProduct(Product producto) => Result.Success();
        }
    }
}