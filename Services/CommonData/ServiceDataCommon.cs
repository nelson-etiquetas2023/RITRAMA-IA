using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;


namespace Ritrama2025.Services.CommonData;

public class ServiceDataCommon : IServiceCommonData
{
    public string StringConnex { get; set; } = null!;
    private readonly IConfiguration _config;

    public ServiceDataCommon(IConfiguration config)
    {
        _config = config ?? throw new ArgumentNullException(nameof(config));
        string ambiente = _config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
        StringConnex = _config.GetSection("ConnectionStringsEnvironment")[ambiente]!;
    }

    public ObjectQuery CreateObjectQuery(ObjectQuery objectquery, DataSet dataset)
    {
        return new ObjectQuery()
        {
            Query = objectquery.Query,
            Message = objectquery.Message,
            Adapter = objectquery.Adapter,
            DataTableName = objectquery.DataTableName,
            DataSet = dataset
        };
    }

    public ObjectQuery CreateObjectProduct(SqlDataAdapter da)
    {
        return new ObjectQuery()
        {
            Query = R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS,
            Message = R.ERROR_MESSAGE_SYSTEM.ERROR_LOAD_PRODUCTS,
            Adapter = da,
            DataTableName = "DtProducts"
        };
    }
    public async Task LoadTable(ObjectQuery objectQuery)
    {
        try
        {
            using SqlConnection connection = new(StringConnex);
            await connection.OpenAsync();
            using SqlCommand comando = new()
            {
                Connection = connection,
                CommandText = objectQuery.Query,
                CommandType = CommandType.Text
            };
            objectQuery.Adapter.SelectCommand = comando;
            objectQuery.Adapter.Fill(objectQuery.DataSet, objectQuery.DataTableName);
            await connection.CloseAsync();
        }
        catch (SqlException ex)
        {
            ServiceErrors.Report(objectQuery.Message + ex.Message);
        }
    }
    public int GetConsecutive(string filtro)
    {
        int Consec = 0;
        try
        {
            using SqlConnection conn = new(StringConnex);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "select par1 from control where filter=@p1",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar, 10) { Value = filtro });
            Consec = Convert.ToInt32(comando.ExecuteScalar());
            conn.Close();
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al calcular el consecutivo..." + ex.Message);
        }
        return Consec;
    }
    public bool VerificarRollIdNoRepeat(string rollid)
    {
        try
        {
            using SqlConnection conn = new(StringConnex);
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = "SELECT count(*) from itemsMateria WHERE rollid=@p1"
            };
            SqlParameter p1 = new("@p1", rollid);
            comando.Parameters.Add(p1);
            conn.Open();

            using SqlDataReader reader = comando.ExecuteReader();
            {
                if (reader.Read())
                {
                    int count = reader.GetInt32(0); // Primera columna
                    if (count > 0)
                    {
                        ServiceErrors.Report("El numero del Rollid esta repetido.[Rollid: ] " + rollid);
                        return false;
                    }
                    else
                    {
                        return true;
                    }
                }
                else
                {
                    return true;
                }
            }
        }
        catch (Exception ex)
        {
            ServiceLogger.LogError("VerificarRollIdNoRepeat", ex);
            ServiceErrors.Report("No se pudo verificar el rollid: " + ex.Message);
            return false;
        }
    }







}
public static class DataAccess
{
    public static async Task<bool> ExecuteQueryWrite(string connectionString, string sqlQuery, List<SqlParameter>? parameters, bool useTransaction)
    {
        using SqlConnection conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        SqlTransaction? transaction = null;
        if (useTransaction)
        {
            transaction = conn.BeginTransaction();
        }

        try
        {
            using SqlCommand comando = new SqlCommand()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = sqlQuery,
                Transaction = transaction
            };

            if (parameters != null)
            {
                comando.Parameters.AddRange(parameters.ToArray());
            }

            await comando.ExecuteNonQueryAsync();

            transaction?.Commit();
            return true;

        }
        catch
        {
            transaction?.Rollback();
            return false;
        }
    }

    public static async Task<DataTable> ExecuteQuery<T>(string connectionString, string sqlQuery, List<SqlParameter>? parameters, bool useTransaction)
    {
        //var result = new List<T>();

        using SqlConnection conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        SqlTransaction? transaction = null;
        if (useTransaction)
        {
            transaction = conn.BeginTransaction();
        }

        try
        {
            using SqlCommand comando = new SqlCommand()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = sqlQuery,
            };

            if (parameters != null)
            {
                comando.Parameters.AddRange(parameters.ToArray());
            }

            using SqlDataReader reader = await comando.ExecuteReaderAsync();

            DataTable table = new DataTable();
            table.Load(reader);
            table.TableName = "Dtproducts";

            transaction?.Commit();
            return table ?? new DataTable();

        }
        catch
        {
            transaction?.Rollback();
            throw;
        }
    }
}
