using Ritrama2025.Models;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Calculo de los importes de un pedido. Logica pura: no toca la base de datos ni la
    /// interfaz, de modo que la pantalla y las pruebas comparten una sola regla de redondeo.
    /// </summary>
    public static class PedidoCalculos
    {
        /// <summary>
        /// Importe de una linea del detalle. Un precio nulo equivale a cero.
        /// </summary>
        public static decimal TotalRenglon(decimal cant, decimal? precio)
        {
            return Math.Round(cant * (precio ?? 0m), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Subtotal, ITBIS y total del pedido a partir de sus lineas y el porcentaje de ITBIS.
        /// </summary>
        public static (decimal SubTotal, decimal MontoItbis, decimal Total) Calcular(
            IEnumerable<PedidoDetalle> lineas,
            decimal porcItbis)
        {
            decimal acumulado = 0m;
            if (lineas != null)
            {
                foreach (PedidoDetalle linea in lineas)
                {
                    acumulado += linea.Cant * (linea.Precio ?? 0m);
                }
            }

            decimal subTotal = Math.Round(acumulado, 2, MidpointRounding.AwayFromZero);
            decimal montoItbis = Math.Round(subTotal * porcItbis / 100m, 2, MidpointRounding.AwayFromZero);
            return (subTotal, montoItbis, subTotal + montoItbis);
        }
    }
}
