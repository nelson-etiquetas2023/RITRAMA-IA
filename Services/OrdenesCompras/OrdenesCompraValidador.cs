using Ritrama2025.Models;

namespace Ritrama2025.Services.OrdenesCompras
{
    /// <summary>
    /// Reglas de negocio del encabezado y las lineas de una orden de compra. Logica pura: el
    /// validador corre en la pantalla antes de guardar y en el servicio como red de seguridad.
    /// </summary>
    public static class OrdenesCompraValidador
    {
        /// <summary>
        /// Indica si la orden puede guardarse y, cuando no, devuelve el motivo en español.
        /// </summary>
        public static bool EsValido(OrdenCompra? orden, out string error)
        {
            error = string.Empty;

            if (orden == null)
            {
                error = "La orden de compra es nula.";
                return false;
            }

            // El numero NO se valida aca: lo asigna el servicio dentro de la transaccion.
            if (string.IsNullOrWhiteSpace(orden.Proveedor_Id))
            {
                error = "Debe seleccionar un proveedor.";
                return false;
            }

            if (orden.Porc_Itbis < 0m)
            {
                error = "El porcentaje de ITBIS no puede ser negativo.";
                return false;
            }

            if (orden.SubTotal < 0m || orden.Monto_Itbis < 0m || orden.Total < 0m)
            {
                error = "Los importes de la orden no pueden ser negativos.";
                return false;
            }

            if (!string.IsNullOrWhiteSpace(orden.Estado) && !EsEstadoValido(orden.Estado))
            {
                error = $"El estado '{orden.Estado}' no es valido.";
                return false;
            }

            if (orden.Detalle == null || orden.Detalle.Count == 0)
            {
                error = "Debe agregar al menos una linea de producto.";
                return false;
            }

            for (int i = 0; i < orden.Detalle.Count; i++)
            {
                OrdenCompraDetalle linea = orden.Detalle[i];
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
        /// Indica si el estado corresponde a uno de los estados validos de la orden.
        /// </summary>
        public static bool EsEstadoValido(string? estado)
        {
            string normalizado = (estado ?? string.Empty).Trim().ToLowerInvariant();
            return normalizado == OrdenCompraEstado.Creado
                || normalizado == OrdenCompraEstado.Ordenado
                || normalizado == OrdenCompraEstado.Parcial
                || normalizado == OrdenCompraEstado.Recibido
                || normalizado == OrdenCompraEstado.Cancelado;
        }
    }
}
