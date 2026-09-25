using System.Globalization;
using System.Text;
using ClosedXML.Excel;

namespace Ritrama2025.Helpers;

public class ExcelValidator
{
    public const string ProductId = "ProductId";
    public const string Width = "Width";
    public const string Ubicacion = "Ubicacion";
    public const string RollId = "RollId";
    public const string Splice = "Splice";
    public const string FechaProduccion = "FechaProduccion";

    private readonly Dictionary<string, int> _columnMap = new(StringComparer.OrdinalIgnoreCase);

    public StringBuilder Errores { get; private set; } = new();

    public void NotificarProductoNoexiste(string message)
    {
        Errores.AppendLine(message);
    }

    public void MapHeaders(IXLWorksheet hoja, params string[] columnas)
    {
        if (hoja is null)
        {
            throw new ArgumentNullException(nameof(hoja));
        }

        IXLRow headerRow = hoja.Row(1);
        foreach (string nombre in columnas)
        {
            IXLCell? columna = headerRow.Cells()
                .FirstOrDefault(c => HeaderMatches(nombre, c.GetString()));

            int indice = columna is null ? -1 : columna.Address.ColumnNumber;
            _columnMap[NormalizeHeader(nombre)] = indice;

            if (indice < 0)
            {
                Errores.AppendLine($"Falta la columna '{nombre}' en la cabecera del archivo");
            }
        }
    }

    public int GetColumn(string nombre)
    {
        if (string.IsNullOrWhiteSpace(nombre))
        {
            return -1;
        }

        string normalized = NormalizeHeader(nombre);
        if (_columnMap.TryGetValue(normalized, out int columna))
        {
            return columna;
        }

        return -1;
    }

    public bool HasColumn(string nombre) => GetColumn(nombre) >= 0;

    public string GetString(IXLRow row, string nombre)
    {
        int indice = GetColumn(nombre);
        if (indice <= 0 || row is null)
        {
            return string.Empty;
        }

        return row.Cell(indice).GetString().Trim();
    }

    public bool IsBlankRow(IXLRow row)
    {
        return row is not null && !row.CellsUsed().Any();
    }

    public double TryGetDouble(IXLCell cell, IXLWorksheet hoja)
    {
        IXLRow row = hoja.Row(cell.Address.RowNumber);
        string header = hoja.Cell(1, cell.Address.ColumnNumber).GetString();
        return TryGetDouble(row, header);
    }

    public double TryGetDouble(IXLRow row, string nombre)
    {
        string valor = GetString(row, nombre);
        if (string.IsNullOrWhiteSpace(valor))
        {
            return 0d;
        }

        string normalizado = valor.Replace(',', '.');
        if (double.TryParse(normalizado, NumberStyles.Float, CultureInfo.InvariantCulture, out double resultado))
        {
            return resultado;
        }

        Errores.AppendLine($"La columna '{nombre}' tiene un valor no numérico: '{valor}'");
        return 0d;
    }

    public int TryGetInt(IXLCell cell, IXLWorksheet hoja)
    {
        IXLRow row = hoja.Row(cell.Address.RowNumber);
        string header = hoja.Cell(1, cell.Address.ColumnNumber).GetString();
        return TryGetInt(row, header);
    }

    public int TryGetInt(IXLRow row, string nombre)
    {
        string valor = GetString(row, nombre);
        if (string.IsNullOrWhiteSpace(valor))
        {
            return 0;
        }

        if (int.TryParse(valor, NumberStyles.Integer, CultureInfo.InvariantCulture, out int resultado))
        {
            return resultado;
        }

        Errores.AppendLine($"La columna '{nombre}' tiene un valor no entero: '{valor}'");
        return 0;
    }

    public DateTime TryGetDateTime(IXLCell cell, IXLWorksheet hoja)
    {
        IXLRow row = hoja.Row(cell.Address.RowNumber);
        string header = hoja.Cell(1, cell.Address.ColumnNumber).GetString();
        return TryGetDateTime(row, header);
    }

    public DateTime TryGetDateTime(IXLRow row, string nombre)
    {
        string valor = GetString(row, nombre);
        if (string.IsNullOrWhiteSpace(valor))
        {
            return DateTime.MinValue;
        }

        string[] formatos = new[]
        {
            "yyyy-MM-dd",
            "yyyy/MM/dd",
            "yyyy.MM.dd",
            "yyyyMMdd",
            "dd/MM/yyyy",
            "MM/dd/yyyy",
            "dd-MM-yyyy",
            "dd.MM.yyyy",
            "MM-dd-yyyy",
            "M/d/yyyy",
            "d/M/yyyy",
            "dd/M/yyyy",
            "d/MM/yyyy",
            "yyyy-MM-dd HH:mm:ss",
            "dd/MM/yyyy HH:mm:ss"
        };

        foreach (string? formato in formatos)
        {
            if (DateTime.TryParseExact(valor, formato, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out DateTime fecha))
            {
                return fecha;
            }
        }

        if (DateTime.TryParse(valor, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out DateTime fechaParse))
        {
            return fechaParse;
        }

        Errores.AppendLine($"La columna '{nombre}' tiene una fecha no válida: '{valor}'");
        return DateTime.MinValue;
    }

    private static bool HeaderMatches(string nombreEsperado, string nombreReal)
    {
        if (string.IsNullOrWhiteSpace(nombreEsperado) || string.IsNullOrWhiteSpace(nombreReal))
        {
            return false;
        }

        string expected = NormalizeHeader(nombreEsperado);
        string actual = NormalizeHeader(nombreReal);

        if (string.IsNullOrEmpty(expected) || string.IsNullOrEmpty(actual))
        {
            return false;
        }

        if (expected == actual)
        {
            return true;
        }

        return nombreEsperado switch
        {
            ProductId => actual.Contains("product") && actual.Contains("id"),
            Width => actual.StartsWith("width", StringComparison.OrdinalIgnoreCase),
            Ubicacion => actual.Contains("ubic"),
            RollId => actual.Contains("roll") && actual.Contains("id"),
            Splice => actual.Contains("splice"),
            FechaProduccion => actual.Contains("fecha") && actual.Contains("produccion"),
            _ => false
        };
    }

    private static string NormalizeHeader(string? valor)
    {
        if (string.IsNullOrWhiteSpace(valor))
        {
            return string.Empty;
        }

        StringBuilder sb = new StringBuilder(valor.Length);
        foreach (char ch in valor.Trim())
        {
            if (char.IsLetterOrDigit(ch))
            {
                sb.Append(char.ToLowerInvariant(ch));
            }
        }

        return sb.ToString();
    }
}







































































































