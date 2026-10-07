using System.Data;
using System.Globalization;
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

    /// <summary>
    /// Obtiene y avanza el código interno de Clientes (1, 2, 3...) de forma atomica.
    /// </summary>
    public int GetAndIncrementConsecCliente() => GetAndIncrementInterno("CLI", "Clientes", 1);

    /// <summary>
    /// Obtiene y avanza el código interno de Proveedores (1, 2, 3...) de forma atomica.
    /// </summary>
    public int GetAndIncrementConsecProveedor() => GetAndIncrementInterno("PROV", "Proveedores", 1);

    /// <summary>
    /// Obtiene y avanza el código interno de Vendedores (1, 2, 3...) de forma atomica.
    /// </summary>
    public int GetAndIncrementConsecVendedor() => GetAndIncrementInterno("VEND", "Vendedores", 1);

    /// <summary>
    /// Obtiene y avanza el consecutivo interno de Productos (columna IdConsec). Arranca
    /// en 1: es referencial y el usuario no lo toca. El codigo Ritrama que teclea el
    /// usuario vive en <see cref="Product.Product_id"/>, que es la clave del producto.
    /// </summary>
    public int GetAndIncrementConsecProducto() => GetAndIncrementInterno("PROD", "Productos", 1);

    /// <summary>
    /// Núcleo común de los consecutivos internos: UPDATE atómico que devuelve el valor
    /// ya incrementado (INSERTED, no DELETED: así el primero es el semillero y nunca se
    /// repite). Si el filtro no existe, lo crea con <paramref name="semilla"/>, que es
    /// también el primer valor entregado: asi el valor guardado es siempre el ultimo
    /// entregado y la proxima llamada devuelve guardado + 1.
    ///
    /// Usar DELETED (el valor ANTES de sumar) era el bug del contador de productos: el
    /// INSERT del semillero devolvia 99999 y lo dejaba guardado, de modo que la primera
    /// llamada siguiente volvia a devolver 99999 y dos altas seguidas se quedaban con el
    /// mismo código.
    ///
    /// El filtro va parametrizado (nunca concatenado), aunque los llamadores usen constantes.
    /// </summary>
    private int GetAndIncrementInterno(string filter, string modulo, int semilla)
    {
        try
        {
            using SqlConnection conn = new(_conn);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "UPDATE control SET par1 = par1 + 1 OUTPUT INSERTED.par1 WHERE filter = @filter",
                CommandType = CommandType.Text
            };
            comando.Parameters.Add(new SqlParameter("@filter", SqlDbType.NVarChar, 10) { Value = filter });
            object? result = comando.ExecuteScalar();
            if (result == null || result == DBNull.Value)
            {
                using SqlCommand initCmd = new()
                {
                    Connection = conn,
                    CommandText = "INSERT INTO control (filter, par1) VALUES (@filter, @par1)",
                    CommandType = CommandType.Text
                };
                initCmd.Parameters.Add(new SqlParameter("@filter", SqlDbType.NVarChar, 10) { Value = filter });
                initCmd.Parameters.Add(new SqlParameter("@par1", SqlDbType.NVarChar, 20) { Value = semilla.ToString(CultureInfo.InvariantCulture) });
                initCmd.ExecuteNonQuery();
                return semilla;
            }
            return Convert.ToInt32(result);
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al obtener el código interno de " + modulo + ": " + ex.Message);
            throw;
        }
    }
}
