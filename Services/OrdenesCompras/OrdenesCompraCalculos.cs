using Ritrama2025.Models;

namespace Ritrama2025.Services.OrdenesCompras
{
    /// <summary>
    /// Calculo de los importes de una orden de compra. Logica pura: no toca la base de datos ni
    /// la interfaz, de modo que la pantalla y las pruebas comparten una sola regla de redondeo.
    /// </summary>
    public static class OrdenesCompraCalculos
    {
        /// <summary>
        /// Importe de una linea de detalle. Un precio nulo equivale a cero.
        /// </summary>
        public static decimal TotalRenglon(decimal cant, decimal? precio)
        {
            return Math.Round(cant * (precio ?? 0m), 2, MidpointRounding.AwayFromZero);
        }

        /// <summary>
        /// Cantidad total de articulos: suma de la cantidad de cada linea. No se redondea:
        /// la columna cant es decimal(18,2) NOT NULL, asi que cada cantidad tiene a lo sumo dos
        /// decimales y la suma es exacta en aritmetica decimal.
        /// </summary>
        public static decimal TotalCantidad(IEnumerable<OrdenCompraDetalle> lineas)
        {
            if (lineas == null)
            {
                return 0m;
            }

            decimal acumulado = 0m;
            foreach (OrdenCompraDetalle linea in lineas)
            {
                acumulado += linea.Cant;
            }

            return acumulado;
        }

        /// <summary>
        /// Subtotal, ITBIS y total de la orden a partir de sus lineas y el porcentaje de ITBIS.
        /// </summary>
        public static (decimal SubTotal, decimal MontoItbis, decimal Total) Calcular(
            IEnumerable<OrdenCompraDetalle> lineas,
            decimal porcItbis)
        {
            decimal acumulado = 0m;
            if (lineas != null)
            {
                foreach (OrdenCompraDetalle linea in lineas)
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
