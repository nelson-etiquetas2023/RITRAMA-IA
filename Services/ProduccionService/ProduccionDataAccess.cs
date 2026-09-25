using System.Data;
using Microsoft.Data.SqlClient;

namespace Ritrama2025.Services.ProduccionService;

internal static class ProduccionDataAccess
{
    public static async Task<DataTable?> CargarTablaAsync(
        string connectionString,
        string sqlQuery,
        bool loadDataset,
        DataSet? dataset,
        SqlParameter[]? parametros,
        string? nombreTabla,
        bool returnDataTable)
    {
        using SqlConnection conn = new(connectionString);
        await conn.OpenAsync();

        using SqlCommand comando = new()
        {
            Connection = conn,
            CommandText = sqlQuery,
            CommandType = CommandType.Text
        };

        if (parametros != null)
        {
            comando.Parameters.AddRange(parametros);
        }

        if (loadDataset)
        {
            using SqlDataAdapter adapter = new() { SelectCommand = comando };
            adapter.Fill(dataset!, nombreTabla!);
            return null;
        }

        if (returnDataTable)
        {
            DataTable dt = new();
            using SqlDataReader reader = await comando.ExecuteReaderAsync(CommandBehavior.Default);
            dt.Load(reader);
            return dt;
        }

        await comando.ExecuteNonQueryAsync();
        return null;
    }
}
