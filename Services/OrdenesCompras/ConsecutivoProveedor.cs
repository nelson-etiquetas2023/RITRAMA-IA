using System.Data;
using System.Globalization;

namespace Ritrama2025.Services.OrdenesCompras
{
    /// <summary>
    /// Regla de formateo del consecutivo del proveedor: 4 digitos con relleno de ceros.
    /// Sin dependencias de interfaz: la pantalla la usa y las pruebas la ejercitan sobre
    /// tablas en memoria.
    /// </summary>
    public static class ConsecutivoProveedor
    {
        public const string ColumnaConsecutivo = "consecutivo";

        private const int Largo = 4;

        /// <summary>
        /// Devuelve el consecutivo de la fila con el relleno de <see cref="Largo"/> digitos,
        /// o cadena vacia si la fila no existe, si la columna no esta o si el valor no es entero.
        /// </summary>
        public static string Formatear(DataRow? fila)
        {
            if (fila is null || !fila.Table.Columns.Contains(ColumnaConsecutivo))
            {
                return string.Empty;
            }

            return FormatearValor(fila[ColumnaConsecutivo]);
        }

        /// <summary>
        /// La regla de formateo del consecutivo, sobre el valor crudo.
        /// </summary>
        public static string FormatearValor(object? valor)
        {
            if (valor is null || valor == DBNull.Value)
            {
                return string.Empty;
            }

            return int.TryParse(valor.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int numero)
                ? numero.ToString("D" + Largo.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)
                : string.Empty;
        }

        /// <summary>
        /// Devuelve el consecutivo de la fila cuya columna llave coincide con el valor.
        /// Devuelve cadena vacia si no hay coincidencia.
        /// </summary>
        public static string FormatearPorLlave(DataTable? tabla, string columnaLlave, object? valor)
        {
            return Formatear(BuscarFila(tabla, columnaLlave, valor));
        }

        /// <summary>
        /// Busca la fila cuya columna llave coincide con el valor. Compara con Equals sobre
        /// el valor crudo, asi que funciona con string contra string.
        /// </summary>
        public static DataRow? BuscarFila(DataTable? tabla, string columnaLlave, object? valor)
        {
            if (tabla is null
                || valor is null
                || valor == DBNull.Value
                || !tabla.Columns.Contains(columnaLlave))
            {
                return null;
            }

            foreach (DataRow fila in tabla.Rows)
            {
                if (Equals(fila[columnaLlave], valor))
                {
                    return fila;
                }
            }

            return null;
        }
    }
}
