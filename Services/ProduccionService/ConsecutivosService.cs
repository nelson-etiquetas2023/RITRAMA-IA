using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace Ritrama2025.Services.ProduccionService;

public class ConsecutivosService : IConsecutivosService
{
    private readonly string _conn;

    public ConsecutivosService(IConfiguration config)
    {
        _conn = ConexionResolver.Resolver(config);
    }

    /// <summary>
    /// Obtiene y avanza el consecutivo de la Orden de Corte de forma atomica (UPDATE + OUTPUT),
    /// eliminando la ventana de carrera entre leer y escribir el contador.
    /// </summary>
    public int GetAndIncrementConsecOC()
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "UPDATE control SET par1 = par1 + 1 OUTPUT DELETED.par1 WHERE filter='COC'",
                CommandType = CommandType.Text
            };
            int numero = Convert.ToInt32(comando.ExecuteScalar()!);
            return numero;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al obtener el consecutivo de la Orden Corte" + ex.Message);
            throw;
        }
    }

    public int GetAndIncrementConsecOCTransactional(SqlConnection conn, SqlTransaction transaction)
    {
        using SqlCommand comando = new()
        {
            Connection = conn,
            Transaction = transaction,
            CommandText = "UPDATE control SET par1 = par1 + 1 OUTPUT DELETED.par1 WHERE filter='COC'",
            CommandType = CommandType.Text
        };
        return Convert.ToInt32(comando.ExecuteScalar()!);
    }

    public int BuscarUniqueCodeConsec()
    {
        int Consec;
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "select par1 from control where filter='UC'",
                CommandType = CommandType.Text
            };
            Consec = Convert.ToInt32(comando.ExecuteScalar());
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al buscar el Unique-Code de la Orden Corte" + ex.Message);
            throw;
        }
        return Consec;
    }

    public int BuscarConsecOC()
    {
        int Consec;
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "select par1 from control where filter='COC'",
                CommandType = CommandType.Text
            };
            Consec = Convert.ToInt32(comando.ExecuteScalar());
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al buscar el consecutivo de la Orden Corte" + ex.Message);
            throw;
        }
        return Consec;
    }

    public bool UpdateConsecOC(string consec)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update control set par1=@p1 where filter='COC'",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@p1", consec);
            comando.Parameters.Add(p1);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al actualizar el consecutivo de la orden de corte. Codigo de Error : " + ex.Message);
            return false;
        }
    }

    public bool UpdateUniqueCodeBD(string consec)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update control set par1=@p1 where filter='UC'",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@p1", consec);
            comando.Parameters.Add(p1);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al actualizar el UNIQUE CODE de los rollos cortados. Codigo de Error : " + ex.Message);
            return false;
        }
    }
}