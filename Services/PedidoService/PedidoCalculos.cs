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
        /// Cantidad total de articulos del pedido: la suma de la cantidad de cada linea, sin
        /// importar el precio. Es lo que va en el campo "Total Cantidad" de la pantalla, que se
        /// recalcula cada vez que las lineas cambian.
        ///
        /// No se redondea, y no por descuido. La columna pedido_detalle.cant es decimal(18,2)
        /// NOT NULL, asi que cada cantidad tiene a lo sumo dos decimales y la suma de decimales
        /// es exacta en aritmetica decimal. Redondear la suma solo podria perder precision: tres
        /// lineas de 0.33 darían 0.99, y un redondeo a dos decimales lo devolveria como 0.99
        /// tambien, pero cualquier redondeo intermedioaria. Con cantidades enteras el problema
        /// no aparece y por eso es facil no verlo.
        /// </summary>
        public static decimal TotalCantidad(IEnumerable<PedidoDetalle> lineas)
        {
            if (lineas == null)
            {
                return 0m;
            }

            decimal acumulado = 0m;
            foreach (PedidoDetalle linea in lineas)
            {
                acumulado += linea.Cant;
            }

            return acumulado;
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
