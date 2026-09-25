using Ritrama2025.Models;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Reglas de negocio del encabezado y las lineas de un pedido. Logica pura: el mismo
    /// validador corre en la pantalla antes de guardar y en el servicio como red de seguridad.
    /// </summary>
    public static class PedidoValidador
    {
        /// <summary>
        /// Indica si el pedido puede guardarse y, cuando no, devuelve el motivo en español.
        /// </summary>
        public static bool EsValido(Pedido? pedido, out string error)
        {
            error = string.Empty;

            if (pedido == null)
            {
                error = "El pedido es nulo.";
                return false;
            }

            if (!PedidoNumero.EsValido(pedido.Numero))
            {
                error = "El numero de pedido debe tener el formato SO-#### (por ejemplo, SO-0027).";
                return false;
            }

            if (pedido.Customer_Id == Guid.Empty)
            {
                error = "Debe seleccionar un cliente.";
                return false;
            }

            if (pedido.Porc_Itbis < 0m)
            {
                error = "El porcentaje de ITBIS no puede ser negativo.";
                return false;
            }

            if (pedido.SubTotal < 0m || pedido.Monto_Itbis < 0m || pedido.Total < 0m)
            {
                error = "Los importes del pedido no pueden ser negativos.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(pedido.Estado) && !EsEstadoValido(pedido.Estado))
            {
                error = $"El estado '{pedido.Estado}' no es valido.";
                return false;
            }

            if (pedido.Detalle == null || pedido.Detalle.Count == 0)
            {
                error = "Debe agregar al menos una linea de producto.";
                return false;
            }

            for (int i = 0; i < pedido.Detalle.Count; i++)
            {
                PedidoDetalle linea = pedido.Detalle[i];
                int numeroLinea = i + 1;

                if (string.IsNullOrWhiteSpace(linea.Product_id))
                {
                    error = $"La linea {numeroLinea} no tiene producto asignado.";
                    return false;
                }

                if (linea.Cant <= 0m)
                {
                    error = $"La cantidad de la linea {numeroLinea} debe ser mayor que cero.";
                    return false;
                }

                if (linea.Precio.HasValue && linea.Precio.Value < 0m)
                {
                    error = $"El precio de la linea {numeroLinea} no puede ser negativo.";
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Indica si el estado corresponde a uno de los estados validos del pedido.
        /// </summary>
        public static bool EsEstadoValido(string? estado)
        {
            string normalizado = (estado ?? string.Empty).Trim().ToLowerInvariant();
            return normalizado == PedidoEstado.Creado
                || normalizado == PedidoEstado.EnProduccion
                || normalizado == PedidoEstado.Pickeado
                || normalizado == PedidoEstado.Despachado
                || normalizado == PedidoEstado.Devuelto;
        }
    }
}
