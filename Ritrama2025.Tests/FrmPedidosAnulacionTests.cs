using System.Data;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Ritrama2025.Services.ProductsService;
using Sunny.UI;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de la anulacion de pedidos en FrmPedidos, enfocadas en el switch
/// sw_anular_pedido. No toca base de datos: construye el formulario con stubs de
/// servicios para comprobar que el switch conserva el fondo rojo (InActiveColor)
/// cuando el pedido esta anulado, sin importar cuantas veces SunnyUI re-aplique
/// el tema global.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmPedidosAnulacionTests
{
    private sealed class PedidoServiceStub : IPedidoService
    {
        public Task<DataTable> LoadDataPedidos(CancellationToken cancellationToken = default)
            => Task.FromResult(new DataTable());

        public Task<DataTable> LoadDataCustomers(CancellationToken cancellationToken = default)
            => Task.FromResult(new DataTable());

        public Task<ClienteDatos?> BuscarClienteAsync(Guid customerId, CancellationToken cancellationToken = default)
            => Task.FromResult<ClienteDatos?>(null);

        public Task<DataTable> LoadDataVendors(CancellationToken cancellationToken = default)
            => Task.FromResult(new DataTable());

        public Task<DataTable> LoadDataPedidoDetalle(string numero, CancellationToken cancellationToken = default)
            => Task.FromResult(new DataTable());

        public Task<string> GetProximoNumeroPedido(CancellationToken cancellationToken = default)
            => Task.FromResult("SO-0001");

        public bool SavePedidoCompleto(Pedido pedido) => true;

        public bool ActualizarPedidoCompleto(Pedido pedido) => true;

        public string? ErrorMsg => null;

        public bool AnularPedido(string numero) => true;

        public bool RestaurarPedido(string numero) => true;

        public bool ActualizarEstadoPedido(string numero, string estado) => true;
    }

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

    private static FrmPedidos CrearFormulario() => new(
        new PedidoServiceStub(),
        new ProductsServiceStub(),
        new ConfigurationBuilder().Build());

    private static UISwitch SwitchDe(FrmPedidos form)
        => (UISwitch)form.Controls.Find("sw_anular_pedido", true).Single();

    [Fact]
    public void TrasReaplicarTema_ElSwitchMantieneElFondoRojo()
    {
        using FrmPedidos form = CrearFormulario();

        form.ReaplicarTema();
        UISwitch sw = SwitchDe(form);

        sw.InActiveColor.Should().Be(Color.Firebrick,
            "AplicarTemaVerde debe fijar el rojo DESPUES de Style = UIStyle.Green, "
            + "porque UISwitch.SetStyleColor() pisa InActiveColor con el gris del tema");
        sw.ActiveColor.Should().Be(LightGreenTheme.PrimaryDark);
    }

    [Fact]
    public void RenderDeSunnyUI_NoPisaLosColoresDelSwitch()
    {
        using FrmPedidos form = CrearFormulario();
        form.ReaplicarTema();

        // UIBaseForm.OnShown llama Render(), que recorre los controles con
        // Style == Inherited y les re-aplica SetStyleColor() del tema global.
        // El switch se marca como Custom en el constructor para quedar fuera.
        form.Render();

        UISwitch sw = SwitchDe(form);
        sw.InActiveColor.Should().Be(Color.Firebrick,
            "Render() de SunnyUI no debe poder pisar el rojo: si falla, el switch volvio "
            + "a quedar con Style = Inherited y SetChildUIStyle le borra los colores");
        sw.ActiveColor.Should().Be(LightGreenTheme.PrimaryDark);
    }

    [Fact]
    public void FlujoEmbebido_TrasShowReaplicarYPintado_ElSwitchSigueRojo()
    {
        using FrmPedidos form = CrearFormulario();

        // Secuencia identica a FormManager.CargarYMostrarAsync:
        // TopLevel=false, Show(), ReaplicarTema(), Refresh().
        form.TopLevel = false;
        form.Show();
        form.ReaplicarTema();
        form.Refresh();

        // Deja correr los mensajes pendientes (Shown u otros) antes de mirar.
        System.Windows.Forms.Application.DoEvents();
        System.Windows.Forms.Application.DoEvents();

        UISwitch sw = SwitchDe(form);
        sw.InActiveColor.Should().Be(Color.Firebrick,
            "Al usuario no se le pone el fondo rojo: algo re-aplica el tema DESPUES de "
            + "ReaplicarTema() y se queda con el gris del tema");
    }

    [Fact]
    public void SwitchAnulado_SePintaConFondoRojo()
    {
        using FrmPedidos form = CrearFormulario();
        form.ReaplicarTema();

        // El switch necesita handle y estar visible para que OnPaintFill corra;
        // sin eso DrawToBitmap devuelve el fondo en blanco.
        form.TopLevel = false;
        form.Show();
        System.Windows.Forms.Application.DoEvents();

        UISwitch sw = SwitchDe(form);
        sw.Enabled = true;
        sw.Active = false;
        sw.CreateControl();
        sw.Refresh();

        using Bitmap pintura = new(sw.Width, sw.Height);
        sw.DrawToBitmap(pintura, new Rectangle(Point.Empty, sw.Size));

        // OnPaintFill rellena todo el rectangulo con InActiveColor y solo dibuja encima
        // el knob y el texto (blancos): el rojo debe dominar la imagen.
        int total = pintura.Width * pintura.Height;
        int rojos = 0;
        Dictionary<Color, int> histograma = new();
        for (int x = 0; x < pintura.Width; x++)
        {
            for (int y = 0; y < pintura.Height; y++)
            {
                Color c = pintura.GetPixel(x, y);
                histograma[c] = histograma.TryGetValue(c, out int n) ? n + 1 : 1;
                if (c.R > 90 && c.R - c.G > 30 && c.R - c.B > 30)
                {
                    rojos++;
                }
            }
        }

        string top = string.Join(", ",
            histograma.OrderByDescending(kv => kv.Value).Take(5)
                .Select(kv => $"{kv.Key} x{kv.Value}"));

        rojos.Should().BeGreaterThan((int)(total * 0.4),
            $"el fondo del switch anulado debe pintarse rojo (Firebrick), no gris del tema. "
            + $"Colores: {top}");
    }
}
