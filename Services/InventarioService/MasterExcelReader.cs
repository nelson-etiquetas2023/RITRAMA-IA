using System.Globalization;
using System.Text;
using ClosedXML.Excel;
using Ritrama2025.Models;

namespace Ritrama2025.Services.InventarioService
{
    /// <summary>
    /// Resultado de leer la hoja de la pestaña "Cargar Inventario" (Master):
    /// las filas ya convertidas en <see cref="ProductMAP"/> más los mensajes de lectura.
    /// </summary>
    public sealed class LecturaMasterResultado
    {
        /// <summary>Filas leídas de la hoja, listas para mostrar en la grilla.</summary>
        public List<ProductMAP> Filas { get; } = [];

        /// <summary>Mensajes de error/advertencia de lectura, uno por línea.</summary>
        public StringBuilder Errores { get; } = new();
    }

    /// <summary>
    /// Lee la hoja de Excel de carga inicial de inventario Master y la convierte en filas
    /// <see cref="ProductMAP"/>. Las columnas se localizan por su encabezado (se aceptan los
    /// alias típicos: wid/width/ancho, length/lenght/largo, ubic/ubicacion...); si la hoja no
    /// trae encabezados reconocibles se usa el orden antiguo de 11 columnas, de modo que las
    /// hojas existentes siguen cargando igual que siempre. Los valores en blanco toman el
    /// valor por defecto: splice 0, fechas de hoy y textos vacíos.
    /// </summary>
    public static class MasterExcelReader
    {
        /// <summary>Cabeceras de la plantilla descargable, en el orden en que se descargan.</summary>
        public static readonly string[] CabecerasPlantilla =
        [
            "product_id", "product_name", "rollid", "wid", "length", "msi",
            "splice", "fecha_produccion", "factura", "ubic", "fecha_llegada", "paleta"
        ];

        /// <summary>Columnas que identifican una hoja escrita con la plantilla.</summary>
        private static readonly string[] ColumnasNecesarias =
            ["product_id", "product_name", "rollid", "wid", "length"];

        /// <summary>
        /// Columna de la plantilla -> encabezados aceptados, ya normalizados
        /// (sin acentos, sin espacios ni guiones y en minúsculas).
        /// </summary>
        private static readonly Dictionary<string, string[]> AliasColumnas = new(StringComparer.Ordinal)
        {
            ["product_id"] = ["productid", "partnumber", "codigoproducto"],
            ["product_name"] = ["productname", "nombreproducto", "nombre"],
            ["rollid"] = ["rollid", "roll"],
            ["wid"] = ["wid", "width", "ancho"],
            ["length"] = ["length", "lenght", "largo"],
            ["msi"] = ["msi"],
            ["splice"] = ["splice"],
            ["fecha_produccion"] = ["fechaproduccion"],
            ["factura"] = ["factura", "numerofactura"],
            ["ubic"] = ["ubic", "ubicacion"],
            ["fecha_llegada"] = ["fechallegada"],
            ["paleta"] = ["paleta", "palet", "paletanum", "paletnum"],
        };

        /// <summary>Orden antiguo de las columnas (formato posicional que ya usaba el formulario).</summary>
        private static readonly Dictionary<string, int> ColumnasAntiguas = new(StringComparer.Ordinal)
        {
            ["product_id"] = 1,
            ["product_name"] = 2,
            ["rollid"] = 3,
            ["wid"] = 4,
            ["length"] = 5,
            ["splice"] = 6,
            ["fecha_produccion"] = 7,
            ["factura"] = 8,
            ["ubic"] = 9,
            ["fecha_llegada"] = 10,
            ["paleta"] = 11,
            ["msi"] = -1, // el formato antiguo no traía msi
        };

        /// <summary>Lee la primera hoja del archivo y mapea sus filas.</summary>
        /// <param name="pathFileName">Ruta del archivo .xlsx.</param>
        public static LecturaMasterResultado Leer(string pathFileName)
        {
            LecturaMasterResultado resultado = new();

            using XLWorkbook workbook = new(pathFileName);
            IXLWorksheet hoja = workbook.Worksheet(1);
            IXLRow? primeraFila = hoja.FirstRowUsed();
            if (primeraFila is null)
            {
                return resultado;
            }

            Dictionary<string, int> columnas = MapearColumnas(primeraFila);
            if (ColumnasNecesarias.Count(c => columnas.ContainsKey(c)) >= 3)
            {
                foreach (string requerida in ColumnasNecesarias)
                {
                    if (!columnas.ContainsKey(requerida))
                    {
                        resultado.Errores.AppendLine($"Falta la columna '{requerida}' en la hoja.");
                    }
                }

                if (!columnas.ContainsKey("product_id"))
                {
                    return resultado;
                }
            }
            else
            {
                // La hoja no trae encabezados reconocibles: se lee por posición, como siempre.
                columnas = new Dictionary<string, int>(ColumnasAntiguas, StringComparer.Ordinal);
            }

            int itemno = 1;
            foreach (IXLRow fila in hoja.RowsUsed())
            {
                if (fila.RowNumber() <= 1)
                {
                    continue; // la fila 1 son los encabezados
                }

                string productId = Texto(fila, columnas, "product_id");
                if (productId.Length == 0)
                {
                    continue; // fila en blanco
                }

                resultado.Filas.Add(new ProductMAP
                {
                    ItemNo = itemno++,
                    Product_Id = productId,
                    Product_Name = Texto(fila, columnas, "product_name"),
                    Rollid = Texto(fila, columnas, "rollid"),
                    Width = Numero(fila, columnas, "wid", resultado.Errores),
                    Length = Numero(fila, columnas, "length", resultado.Errores),
                    Msi = Numero(fila, columnas, "msi", resultado.Errores),
                    Splice = (int)Math.Round(Numero(fila, columnas, "splice", resultado.Errores)),
                    Fecha_Produccion = Fecha(fila, columnas, "fecha_produccion", resultado.Errores),
                    Factura = Texto(fila, columnas, "factura"),
                    Ubic = Texto(fila, columnas, "ubic"),
                    Fecha_Llegada = Fecha(fila, columnas, "fecha_llegada", resultado.Errores),
                    Paleta = Texto(fila, columnas, "paleta"),
                });
            }

            return resultado;
        }

        /// <summary>Localiza cada columna de la plantilla en la fila de encabezados.</summary>
        private static Dictionary<string, int> MapearColumnas(IXLRow filaEncabezado)
        {
            Dictionary<string, int> columnas = new(StringComparer.Ordinal);
            HashSet<int> usadas = [];

            foreach (KeyValuePair<string, string[]> campo in AliasColumnas)
            {
                foreach (IXLCell celda in filaEncabezado.CellsUsed())
                {
                    string encabezado = Normalizar(celda.GetString());
                    if (encabezado.Length == 0 ||
                        !campo.Value.Contains(encabezado, StringComparer.Ordinal) ||
                        usadas.Contains(celda.Address.ColumnNumber))
                    {
                        continue;
                    }

                    columnas[campo.Key] = celda.Address.ColumnNumber;
                    usadas.Add(celda.Address.ColumnNumber);
                    break;
                }
            }

            return columnas;
        }

        /// <summary>Normaliza un encabezado: quita acentos, espacios y guiones y pasa a minúsculas.</summary>
        private static string Normalizar(string? valor)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return string.Empty;
            }

            string desacentuado = valor.Normalize(NormalizationForm.FormD);
            StringBuilder letras = new(desacentuado.Length);
            foreach (char c in desacentuado)
            {
                if (CharUnicodeInfo.GetUnicodeCategory(c) != UnicodeCategory.NonSpacingMark)
                {
                    letras.Append(c);
                }
            }

            StringBuilder normalizado = new(letras.Length);
            foreach (char c in letras.ToString().Normalize(NormalizationForm.FormC))
            {
                if (char.IsLetterOrDigit(c))
                {
                    normalizado.Append(char.ToLowerInvariant(c));
                }
            }

            return normalizado.ToString();
        }

        private static string Texto(IXLRow fila, Dictionary<string, int> columnas, string campo)
        {
            if (!columnas.TryGetValue(campo, out int columna) || columna <= 0)
            {
                return string.Empty;
            }

            return fila.Cell(columna).Value.ToString().Trim();
        }

        private static double Numero(IXLRow fila, Dictionary<string, int> columnas, string campo, StringBuilder errores)
        {
            string valor = Texto(fila, columnas, campo);
            if (valor.Length == 0)
            {
                return 0d;
            }

            if (double.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out double numero))
            {
                return numero;
            }

            if (double.TryParse(valor.Replace(',', '.'), NumberStyles.Float, CultureInfo.InvariantCulture, out numero))
            {
                return numero;
            }

            errores.AppendLine($"Fila {fila.RowNumber()}: la columna '{campo}' tiene el valor '{valor}' que no es un número.");
            return 0d;
        }

        private static DateTime Fecha(IXLRow fila, Dictionary<string, int> columnas, string campo, StringBuilder errores)
        {
            if (!columnas.TryGetValue(campo, out int columna) || columna <= 0)
            {
                return DateTime.Today;
            }

            IXLCell celda = fila.Cell(columna);
            if (celda.DataType == XLDataType.DateTime)
            {
                return celda.GetDateTime();
            }

            string valor = celda.Value.ToString().Trim();
            if (valor.Length == 0)
            {
                return DateTime.Today;
            }

            if (DateTime.TryParse(valor, CultureInfo.CurrentCulture, DateTimeStyles.None, out DateTime fecha))
            {
                return fecha;
            }

            if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.None, out fecha))
            {
                return fecha;
            }

            // Excel también guarda las fechas como número (serial).
            if (double.TryParse(valor, NumberStyles.Float, CultureInfo.InvariantCulture, out double serial) &&
                serial > 0 && serial <= 2958465)
            {
                return DateTime.FromOADate(serial);
            }

            errores.AppendLine($"Fila {fila.RowNumber()}: la columna '{campo}' tiene la fecha '{valor}' que no se puede leer.");
            return DateTime.Today;
        }
    }
}
