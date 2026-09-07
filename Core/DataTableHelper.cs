using System.Data;
using System.Reflection;

namespace Ritrama2025.Core;

public static class DataTableHelper
{
    public static DataTable ToDataTable<T>(IEnumerable<T> lista)
    {
        DataTable table = new();
        if (lista == null)
        {
            return table;
        }

        List<T> items = lista.ToList();
        if (items.Count == 0)
        {
            return table;
        }

        PropertyInfo[] properties = typeof(T).GetProperties();
        foreach (PropertyInfo prop in properties)
        {
            table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
        }

        foreach (T item in items)
        {
            DataRow row = table.NewRow();
            foreach (PropertyInfo prop in properties)
            {
                row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
            }
            table.Rows.Add(row);
        }

        return table;
    }
}