using System.Data;
using Ritrama2025.Models;

namespace Ritrama2025.Services.OrdenesCompras
{
    /// <summary>
    /// Convierte el resultado de SQL_SELECT_OC_DETALLE en lineas de negocio.
    /// Conserva el product_id, que es NOT NULL en la base.
    /// </summary>
    public static class OrdenesCompraDetalleMapper
    {
        public static List<OrdenCompraDetalle> Mapear(DataTable tabla)
        {
            List<OrdenCompraDetalle> lineas = new();
            if (tabla == null)
            {
                return lineas;
            }

            foreach (DataRow row in tabla.Rows)
            {
                lineas.Add(new OrdenCompraDetalle
                {
                    Product_id = Texto(row, "product_id"),
                    Product_name = Texto(row, "product_name"),
                    Cant = Decimal(row, "cant"),
                    Unidad = Texto(row, "unidad"),
                    Width = Decimal(row, "width"),
                    Lenght = Decimal(row, "lenght"),
                    Msi = Decimal(row, "msi"),
                    Precio = DecimalNulo(row, "precio"),
                    Total_Renglon = DecimalNulo(row, "total_renglon"),
                    Notas = Texto(row, "notas")
                });
            }

            return lineas;
        }

        private static string? Texto(DataRow row, string columna)
        {
            if (!TieneColumna(row, columna))
            {
                return null;
            }

            object? valor = row[columna];
            return valor == null || valor == DBNull.Value ? null : valor.ToString();
        }

        private static decimal Decimal(DataRow row, string columna)
        {
            return DecimalNulo(row, columna) ?? 0m;
        }

        private static decimal? DecimalNulo(DataRow row, string columna)
        {
            if (!TieneColumna(row, columna))
            {
                return null;
            }

            object? valor = row[columna];
            if (valor == null || valor == DBNull.Value)
            {
                return null;
            }

            return decimal.TryParse(valor.ToString(), out decimal monto) ? monto : null;
        }

        private static bool TieneColumna(DataRow row, string columna)
        {
            return row.Table.Columns.Contains(columna);
        }
    }
}
