using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.ProductsService;

/// <summary>
/// Mapeos entre <see cref="Product"/> y <see cref="DataRow"/>.
/// Centralizado para evitar duplicación entre services y Forms.
/// Nota: vive en Services (no en Core) para evitar dependencia circular Core → Models.
/// Si Product se mueve a Core, este mapper puede migrarse a Core sin cambios.
/// </summary>
internal static class ProductMapper
{
    /// <summary>
    /// Crea un <see cref="Product"/> desde un <see cref="DataRow"/> del catálogo (SELECT_QUERY_PRODUCTS).
    /// Maneja DBNull y nombres de columna case-insensitive.
    /// </summary>
    public static Product FromDataRow(DataRow row)
    {
        ArgumentNullException.ThrowIfNull(row);

        return new Product
        {
            Product_id = GetString(row, "product_id"),
            Product_Name = GetString(row, "product_name"),
            Product_Description = GetString(row, "product_descrip"),
            Referencia = GetString(row, "product_ref"),
            Codigo_Barra = GetString(row, "codebar"),
            Precio = GetDecimal(row, "precio"),
            Costo = GetDecimal(row, "costo"),
            Ratio = GetDecimal(row, "ratio"),
            Anulado = GetBool(row, "anulado"),
            Master = GetBool(row, "masterRolls") || GetBool(row, "MasterRolls") || GetBool(row, "Master"),
            Hoja = GetBool(row, "resmas") || GetBool(row, "Resmas"),
            Graphics = GetBool(row, "graphics") || GetBool(row, "Graphics"),
            RolloCortado = GetBool(row, "rollo_cortado") || GetBool(row, "rolloCortado")
        };
    }

    /// <summary>Mapea una colección de filas a lista tipada.</summary>
    public static IReadOnlyList<Product> FromDataTable(DataTable table)
    {
        ArgumentNullException.ThrowIfNull(table);
        List<Product> list = new(table.Rows.Count);
        foreach (DataRow row in table.Rows)
        {
            list.Add(FromDataRow(row));
        }

        return list;
    }

    private static string GetString(DataRow row, string column)
    {
        if (!row.Table.Columns.Contains(column))
        {
            return string.Empty;
        }

        object? v = row[column];
        if (v == null || v == DBNull.Value)
        {
            return string.Empty;
        }

        return v.ToString() ?? string.Empty;
    }

    private static decimal GetDecimal(DataRow row, string column)
    {
        if (!row.Table.Columns.Contains(column))
        {
            return 0m;
        }

        object? v = row[column];
        if (v == null || v == DBNull.Value)
        {
            return 0m;
        }

        if (v is decimal d)
        {
            return d;
        }

        if (v is double db)
        {
            return (decimal)db;
        }

        if (v is float f)
        {
            return (decimal)f;
        }

        if (decimal.TryParse(v.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out decimal parsed))
        {
            return parsed;
        }

        return 0m;
    }

    private static bool GetBool(DataRow row, string column)
    {
        if (!row.Table.Columns.Contains(column))
        {
            return false;
        }

        object? v = row[column];
        if (v == null || v == DBNull.Value)
        {
            return false;
        }

        if (v is bool b)
        {
            return b;
        }

        if (v is byte by)
        {
            return by != 0;
        }

        if (v is short sh)
        {
            return sh != 0;
        }

        if (v is int i)
        {
            return i != 0;
        }

        if (v is long l)
        {
            return l != 0;
        }

        string s = v.ToString() ?? string.Empty;
        if (bool.TryParse(s, out bool bb))
        {
            return bb;
        }

        if (int.TryParse(s, out int ii))
        {
            return ii != 0;
        }

        return false;
    }
}
