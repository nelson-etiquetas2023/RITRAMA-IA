using System.Data;
using System.Globalization;

namespace Ritrama2025.Services.PedidoService
{
    /// <summary>
    /// Regla de los campos de solo lectura que muestran el consecutivo del cliente y del
    /// vendedor. Sin dependencias de interfaz: la pantalla la usa y las pruebas la ejercitan
    /// sobre tablas en memoria, que es lo que permite fijar el comportamiento.
    ///
    /// Vive separada del formulario a proposito. Estas reglas ya se havian partido una vez: la
    /// ruta que corre al elegir en pantalla paso a mostrar el consecutivo y la que corre al
    /// abrir un pedido guardado se quedo mostrando el identificador, sin que ninguna prueba
    /// lo notara. Estando aqui, cliente y vendedor pasan por la misma funcion y las dos rutas
    /// por el mismo formateo, asi que no pueden volver a divergir sin romper una prueba.
    /// </summary>
    public static class ConsecutivoCliente
    {
        /// <summary>Columna que trae el consecutivo en la consulta de clientes y de vendedores.</summary>
        public const string ColumnaConsecutivo = "consecutivo";

        private const int Largo = 4;

        /// <summary>
        /// Devuelve el consecutivo de la fila con el relleno de <see cref="Largo"/> digitos, o
        /// cadena vacia si la fila no existe, si la columna no esta o si el valor no es un entero.
        /// Nunca lanza: un maestro con un dato raro no debe voltear la pantalla.
        /// </summary>
        public static string Formatear(DataRow? fila)
        {
            if (fila is null || !fila.Table.Columns.Contains(ColumnaConsecutivo))
            {
                return string.Empty;
            }

            object? valor = fila[ColumnaConsecutivo];

            if (valor is null || valor == DBNull.Value)
            {
                return string.Empty;
            }

            return int.TryParse(valor.ToString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out int numero)
                ? numero.ToString("D" + Largo.ToString(CultureInfo.InvariantCulture), CultureInfo.InvariantCulture)
                : string.Empty;
        }

        /// <summary>
        /// Devuelve el consecutivo de la fila cuya columna <paramref name="columnaLlave"/> vale
        /// <paramref name="valor"/>, sin mirar que fila este seleccionada. Es lo que se usa al
        /// abrir un pedido: la fila correcta se deduce del identificador del propio pedido, no
        /// de lo que quede elegido en pantalla, que en consulta depende de quien abrio el form.
        /// Devuelve cadena vacia si no hay coincidencia.
        /// </summary>
        public static string FormatearPorLlave(DataTable? tabla, string columnaLlave, object? valor)
        {
            return Formatear(BuscarFila(tabla, columnaLlave, valor));
        }

        /// <summary>
        /// Busca la fila cuya columna llave coincide con el valor. Compara con Equals sobre el
        /// valor crudo de cada celda, asi que funciona con el tipo con que la columna entregue el
        /// dato: un Guid contra un Guid, no un Guid contra su texto.
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
