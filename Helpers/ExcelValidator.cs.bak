using System.Globalization;
using System.Text;
using ClosedXML.Excel;

namespace Ritrama2025.Helpers;

public class ExcelValidator
{
    public StringBuilder Errores { get; private set; } = new();

    public void NotificarProductoNoexiste(string message)
    {
        Errores.AppendLine(message);
    }

    public double TryGetDouble(IXLCell cell, IXLWorksheet hoja)
    {
        string valor = cell.GetString().Trim();
        string celdaRef = cell.Address.ToString()!;
        int fila = cell.Address.RowNumber;
        int columna = cell.Address.ColumnNumber;
        string nombreColumnma = hoja.Cell(1, columna).GetString().Trim();

        if (double.TryParse(valor, NumberStyles.Any, CultureInfo.InvariantCulture, out var resultado))
            return resultado;

        Errores.AppendLine($"Error en la columna: {nombreColumnma}, un valor tipo DOUBLE -> Celda: {celdaRef}, (Fila {fila}, Columna {columna}): '{valor}' no es un numero decimal -> valor por defecto 0.0");

        return 0.0;
    }

    public int TryGetInt(IXLCell cell, IXLWorksheet hoja)
    {
        string valor = cell.GetString().Trim();
        string celdaRef = cell.Address.ToString()!;
        int fila = cell.Address.RowNumber;
        int columna = cell.Address.ColumnNumber;
        string nombreColumnma = hoja.Cell(1, columna).GetString().Trim();

        if (int.TryParse(valor, out var resultado))
            return resultado;

        Errores.AppendLine($"Error en la columna: {nombreColumnma}, un valor tipo INT -> Celda: {celdaRef}, (Fila {fila}, Columna {columna}): '{valor}' no es un numero entero -> valor por defecto 0");

        return 0;

    }

    public DateTime TryGetDateTime(IXLCell cell, IXLWorksheet hoja)
    {
        string valor = cell.GetString().Trim();
        string celdaRef = cell.Address.ToString()!;
        int fila = cell.Address.RowNumber;
        int columna = cell.Address.ColumnNumber;
        string nombreColumnma = hoja.Cell(1, columna).GetString().Trim();

        if (DateTime.TryParse(valor, out var resultado))
            return resultado;

        Errores.AppendLine($"Error en la columna: {nombreColumnma}, un valor tipo DATETIME -> Celda: {celdaRef}, (Fila {fila}, Columna {columna}): '{valor}' no es una fecha valida -> valor por defecto DateTime.MinValue");

        return DateTime.MinValue;
    }
}