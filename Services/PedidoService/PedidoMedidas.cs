using System;
using System.Globalization;
using Ritrama2025.Models;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Regla de medidas del detalle de un pedido. Logica pura: no toca la base de datos ni la
    /// interfaz, de modo que la pantalla y las pruebas compartan una sola definicion de cuando
    /// un producto obliga a capturar ancho y largo.
    ///
    /// Solo la categoria Rollo Cortado los exige: un rollo se vende por sus medidas, asi que sin
    /// ancho y largo la linea no describe el producto. Las demas categorias (Master, Resma y
    /// Graphics) se venden por unidades, y ahi las medidas no aplican y quedan en cero.
    ///
    /// El nombre de la categoria llega como texto desde el combo de productos, que lo arma el
    /// CASE WHEN de SELECT_QUERY_PRODUCTS sobre los cuatro bit de producto. No se vuelve a leer
    /// el producto de la base, para que la regla no dependa de una segunda consulta.
    /// </summary>
    public static class PedidoMedidas
    {
        /// <summary>
        /// Categoria que exige medidas. Es el mismo texto que produce el CASE WHEN de
        /// R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS.
        /// </summary>
        public const string TipoRolloCortado = "Rollo Cortado";

        /// <summary>Texto de la etiqueta que nombra el ancho, para los mensajes al usuario.</summary>
        private const string NombreAncho = "ancho";

        /// <summary>Texto de la etiqueta que nombra el largo, para los mensajes al usuario.</summary>
        private const string NombreLargo = "largo";

        /// <summary>
        /// Indica si la categoria del producto obliga a capturar ancho y largo.
        ///
        /// La comparacion no distingue mayusculas ni recorta espacios porque el dato viene de una
        /// columna con un CASE WHEN y no todas las filas se escribieron con la misma
        /// capitalizacion, igual que ocurre con PedidoCatalogos.EsContado.
        /// </summary>
        public static bool PideMedidas(string? tipoProducto)
        {
            return string.Equals(
                tipoProducto?.Trim(),
                TipoRolloCortado,
                StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// Convierte el texto capturado en el editor en una medida. Devuelve false y deja el valor
        /// en cero cuando no hay un numero utilizable, que es el caso que la pantalla translate
        /// en "falta capturar".
        ///
        /// Se intenta primero con es-ES, que es la cultura que impone Program.cs, y despues con la
        /// invariante. Se fijan las dos en vez de usar la cultura vigente para que el resultado no
        /// dependa de la maquina que corre las pruebas: con la cultura del sistema, "60,5" se
        /// interpretaria como 605 en un equipo en ingles.
        /// </summary>
        public static bool LeerMedida(string? texto, out decimal valor)
        {
            valor = 0m;
            string? contenido = texto?.Trim();
            if (string.IsNullOrEmpty(contenido))
            {
                return false;
            }

            if (decimal.TryParse(contenido, NumberStyles.Number, CultureInfo.GetCultureInfo("es-ES"), out valor))
            {
                return true;
            }

            return decimal.TryParse(contenido, NumberStyles.Number, CultureInfo.InvariantCulture, out valor);
        }

        /// <summary>
        /// Valida las medidas de una linea contra la categoria del producto. Devuelve true y un
        /// error nulo cuando la categoria no exige medidas, aunque venga en cero, y cuando la que
        /// las exige trae las dos.
        /// </summary>
        public static bool Validar(string? tipoProducto, decimal ancho, decimal largo, out string? error)
        {
            if (!PideMedidas(tipoProducto))
            {
                error = null;
                return true;
            }

            if (ancho <= 0m)
            {
                error = "El " + NombreAncho + " es obligatorio para un producto de tipo " + TipoRolloCortado + ".";
                return false;
            }

            if (largo <= 0m)
            {
                error = "El " + NombreLargo + " es obligatorio para un producto de tipo " + TipoRolloCortado + ".";
                return false;
            }

            error = null;
            return true;
        }
    }
}
