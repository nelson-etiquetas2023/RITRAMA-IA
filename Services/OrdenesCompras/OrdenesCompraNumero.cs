using System.Globalization;

namespace Ritrama2025.Services.OrdenesCompras
{
    /// <summary>
    /// Regla unica del numero de orden de compra: formato OC-####, validacion y lectura de la
    /// parte numerica. Logica pura, sin base de datos ni interfaz.
    /// </summary>
    public static class OrdenesCompraNumero
    {
        /// <summary>Prefijo que precede a la parte numerica.</summary>
        public const string Prefijo = "OC-";

        /// <summary>Cantidad de digitos de la parte numerica, con relleno de ceros a la izquierda.</summary>
        public const int LargoNumero = 5;

        /// <summary>
        /// Valor maximo representable con el relleno de <see cref="LargoNumero"/> digitos.
        /// </summary>
        public const int MaximoNumero = 99999;

        private const int NumeroMinimo = 1;

        /// <summary>
        /// Compone el numero completo. Por ejemplo, 27 produce "OC-00027".
        /// </summary>
        /// <exception cref="ArgumentOutOfRangeException">
        /// Si el numero no cabe en <see cref="LargoNumero"/> digitos o no es positivo.
        /// </exception>
        public static string Formatear(int numero)
        {
            if (numero < NumeroMinimo || numero > MaximoNumero)
            {
                throw new ArgumentOutOfRangeException(
                    nameof(numero),
                    numero,
                    "El numero de orden de compra debe estar entre 1 y " + MaximoNumero + " para usar el formato "
                        + Prefijo + new string('#', LargoNumero) + ".");
            }

            return Prefijo + numero.ToString(
                "D" + LargoNumero.ToString(CultureInfo.InvariantCulture),
                CultureInfo.InvariantCulture);
        }

        /// <summary>
        /// Indica si el texto tiene exactamente el formato OC-####, con digitos en la parte
        /// numerica. Rechaza nulo, vacio, otros prefijos y cualquier otra longitud.
        /// </summary>
        public static bool EsValido(string? numero)
        {
            if (numero is null || numero.Length != Prefijo.Length + LargoNumero)
            {
                return false;
            }

            if (!numero.StartsWith(Prefijo, StringComparison.Ordinal))
            {
                return false;
            }

            for (int i = Prefijo.Length; i < numero.Length; i++)
            {
                if (numero[i] is < '0' or > '9')
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Devuelve la parte numerica del numero, o cero si el texto no tiene el formato valido.
        /// Nunca lanza, para que un dato corrupto en la base no tumbe la pantalla.
        /// </summary>
        public static int ParteNumerica(string? numero)
        {
            if (!EsValido(numero))
            {
                return 0;
            }

            string parte = numero!.Substring(Prefijo.Length, LargoNumero);

            return int.TryParse(parte, NumberStyles.None, CultureInfo.InvariantCulture, out int valor)
                ? valor
                : 0;
        }
    }
}
