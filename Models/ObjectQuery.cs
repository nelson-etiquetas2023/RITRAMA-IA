using System.Data;
using Microsoft.Data.SqlClient;

namespace Ritrama2025.Models
{
    public class ObjectQuery
    {
        public string Query { get; set; } = null!;
        public string Message { get; set; } = null!;
        public SqlDataAdapter Adapter { get; set; } = new();
        public string DataTableName { get; set; } = null!;
        public string StringConnex { get; set; } = null!;
        public DataSet DataSet { get; set; } = new();

    }
}

