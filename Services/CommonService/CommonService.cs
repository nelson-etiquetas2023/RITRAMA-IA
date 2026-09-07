using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.CommonService;

public class CommonService : ICommonService
{
    public string StringConnex { get; set; } = null!;
    public string ErrorMsg { get; set; } = null!;
    public string Ambiente { get; } = "";

    public IConfiguration Config { get; set; }
    public CommonService(IConfiguration Config)
    {
        this.Config = Config;
        if (Config != null)
        {
            Ambiente = Config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            var ambiente = Ambiente;
                StringConnex = Config.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]!;
            }
        }

        private static string ObtenerServidor(string cs)
        {
            string Valor(string clave)
            {
                var m = System.Text.RegularExpressions.Regex.Match(cs, $@"(?i){clave}\s*=\s*([^;]+)");
                return m.Success ? m.Groups[1].Value.Trim() : "";
            }
            return $"{Valor("Data Source")}/{Valor("Initial Catalog")}";
        }
public async Task<List<RolloCortado>> GetDataRolloCortado(List<RolloCortado> lista)
    {
        if (lista.Count == 0) return lista;

        var codigos = lista
            .Select(i => i.UniqueCode)
            .Where(c => !string.IsNullOrWhiteSpace(c))
            .Distinct()
            .ToList();
        if (codigos.Count == 0) return lista;

        try
        {
            using SqlConnection conn = new(StringConnex);
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandType = CommandType.Text
            };

            var placeholders = codigos.Select((_, idx) => $"@uc{idx}").ToArray();
            for (int idx = 0; idx < codigos.Count; idx++)
            {
                comando.Parameters.Add(new SqlParameter(placeholders[idx], SqlDbType.NVarChar) { Value = codigos[idx] });
            }

            comando.CommandText =
                $"SELECT numero, product_id, product_name, roll_number, width, large, msi, splice, roll_id, code_person, status, unique_code, 'M' AS tipo_mov FROM rolls_details WHERE unique_code IN ({string.Join(",", placeholders)}) AND disponible = 1 " +
                $"UNION SELECT numero, product_id, product_name, roll_number, width, large, msi, splice, roll_id, code_person, status, unique_code, 'M' AS tipo_mov FROM RollsInic WHERE unique_code IN ({string.Join(",", placeholders)}) AND disponible = 1";

            await conn.OpenAsync();
            var mapa = lista.GroupBy(i => i.UniqueCode).ToDictionary(g => g.Key, g => g.First());
            using SqlDataReader reader = await comando.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                if (!mapa.TryGetValue(reader.GetString(reader.GetOrdinal("unique_code")), out var item)) continue;
                item.Product_Id = reader.GetString("product_id");
                item.Product_Name = reader.GetString("product_name");
                item.RollNumber = reader.GetInt32("roll_number");
                item.Width = (double)reader.GetDecimal("width");
                item.Length = (double)reader.GetDecimal("large");
                item.Msi = (double)reader.GetDecimal("msi");
                item.Splice = reader.GetInt32("splice");
                item.Roll_Id = reader.GetString("roll_id");
                item.Cantidad_despacho = 0;
                item.Cantidad = 0;
                item.Tipo = reader.GetString("tipo_mov");
                item.Code_Person = reader.GetString("code_person");
            }
        }
        catch (SqlException ex)
        {
            ErrorMsg = $"Entorno={Ambiente} -> servidor {ObtenerServidor(StringConnex)}: {ex.Message}";
            ServiceErrors.Report("error al cargar los datos del rc en el picking: " + ErrorMsg);
        }
        return lista;
    }
    public void SaveTransportEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO transporte (transport_id, transport_name) VALUES (@p1, @p2)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });

        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de transporte: " + ErrorMsg);
        }
    }
    public void DeleteTransportEntity(string Id)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "DELETE FROM transporte WHERE transport_id=@p1"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al eliminar la entidad de transporte: " + ErrorMsg);
        }
    }
    public void SaveChoferEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO chofer (chofer_id, chofer_name) VALUES (@p1, @p2)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de chofer: " + ErrorMsg);
        }
    }
    public void DeleteChoferEntity(string Id)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "DELETE FROM chofer WHERE chofer_id=@p1"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al eliminar la entidad de chofer: " + ErrorMsg);
        }
    }
    public void SaveCamionEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO camion (placas_id, camion_name) VALUES (@p1, @p2)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de camion: " + ErrorMsg);
        }
    }
    public void DeleteCamionEntity(string Id)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "DELETE FROM camion WHERE placas_id=@p1"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al eliminar la entidad de camion: " + ErrorMsg);
        }
    }
    public void SaveProvaiderEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO provider (proveedor_id, proveedor_name,unidad_master_1,unidad_master_2,phone,direccion,email,anulado) VALUES (@p1, @p2, 0, 0, 'sn','','',0)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de proveedor: " + ErrorMsg);
        }
    }
    public void DeleteProvaiderEntity(string Id)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "DELETE FROM provider WHERE proveedor_id=@p1"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al eliminar la entidad de proveedor: " + ErrorMsg);
        }
    }
    public void SavePersonEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO person (person_id, person_name) VALUES (@p1, @p2)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de persona: " + ErrorMsg);
        }
    }
    public void DeletePersonEntity(string Id)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "DELETE FROM person WHERE person_id=@p1"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al eliminar la entidad de persona: " + ErrorMsg);
        }
    }
    public bool DocumentCheckWriteOC(DocumentCheckOC doc)
    {
        try
        {
            using SqlConnection conn = new(StringConnex);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = "update orden_corte set PersonCheck=@p2,Orden_Servicio=@p3,Orden_Trabajo=@p4,notes=@p5,Fecha_Autorize=@p6 where numero=@p1"
            };
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = Convert.ToInt32(doc.OrdenCorte) });
            comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NChar, 25) { Value = doc.PersonCheck });
            comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.NChar, 20) { Value = doc.Orden_Servicio });
            comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.NChar, 20) { Value = doc.Orden_Trabajo });
            comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.Text) { Value = doc.Observaciones });
            comando.Parameters.Add(new SqlParameter("@p6", SqlDbType.DateTime) { Value = doc.FechaCheck });
            comando.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al tratar de aprobar el documento de orden de corte...: codigo error :" + ex.Message);
            return false;
        }
    }
    public DocumentCheckOC DocumentCheckReadOC(string oc)
    {
        DocumentCheckOC document = new();

        try
        {
            using SqlConnection conn = new(StringConnex);
            conn.Open();
            SqlCommand comando = new()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = "select PersonCheck,Orden_Servicio,Orden_Trabajo,notes,Fecha_Autorize from orden_corte where numero=@p1"
            };
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.Int) { Value = Convert.ToInt32(oc) });
            SqlDataReader dr = comando.ExecuteReader();
            while (dr.Read())
            {
                document = new()
                {
                    PersonCheck = dr.GetString(dr.GetOrdinal("PersonCheck")),
                    Orden_Servicio = dr.GetString(dr.GetOrdinal("Orden_Servicio")),
                    Orden_Trabajo = dr.GetString(dr.GetOrdinal("Orden_Trabajo")),
                    Observaciones = dr.GetString(dr.GetOrdinal("notes")),
                    FechaCheck = dr.GetDateTime(dr.GetOrdinal("Fecha_Autorize"))
                };
            }
            return document;
        }
        catch (Exception ex)
        {
            ServiceErrors.Report("Error al tratar de leer el documento aprobado de orden de corte...: codigo error :" + ex.Message);
            return document;
        }

    }
    public void SaveOperatorEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO operadores (operador_id, nombre) VALUES (@p1, @p2)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de operadores: " + ErrorMsg);
        }
    }
    public void DeleteOperatorEntity(string Id)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "DELETE FROM operadores WHERE operador_id=@p1"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al eliminar la entidad de operadores: " + ErrorMsg);
        }
    }


    public void SaveVendedorEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO vendedor (vendor_id, vendor_name, correo, phone, anulado) VALUES (@p1, @p2,'nt','nt',0)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });
        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de vendedores: " + ErrorMsg);
        }
    }

    public void SaveCustomerEntity(string Id, string Name)
    {
        using SqlConnection conn = new(StringConnex);
        SqlCommand comando = new()
        {
            Connection = conn,
            CommandType = CommandType.Text,
            CommandText = "INSERT INTO customer (customer_id, customer_name,customer_category,customer_email,anulado) VALUES (@p1, @p2, @p3, @p4, @p5)"
        };
        comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar) { Value = Id });
        comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar) { Value = Name });
        comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.NVarChar) { Value = "general" });
        comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar) { Value = "nt" });
        comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.NVarChar) { Value = false });

        try
        {
            conn.Open();
            comando.ExecuteNonQuery();
        }
        catch (SqlException ex)
        {
            ErrorMsg = ex.Message;
            ServiceErrors.Report("Error al guardar la entidad de clientes: " + ErrorMsg);
        }
    }
    public static void ADD_COLUMN_GRID(string name, int size, string title, string field_bd, DataGridView grid)
    {
        DataGridViewTextBoxColumn col = new()
        {
            Name = name,
            Width = size,
            HeaderText = title,
            DataPropertyName = field_bd,
        };
        grid.Columns.Add(col);
    }
    public RolloCortado SearchCodigoUnico(string id)
    {
        var rollo = new RolloCortado();
        try
        {
            using SqlConnection conn = new(StringConnex);
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandType = CommandType.Text,
                CommandText = "select numero,product_id,product_name,unique_code,roll_id,code_person,width,large,msi,ubic,roll_number,status,disponible from rolls_details where unique_code=@p1"
            };
            conn.Open();
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar, 50) { Value = id });
            SqlDataReader reader = comando.ExecuteReader();
            while (reader.Read())
            {
                rollo.Product_Id = reader.GetString(reader.GetOrdinal("product_id"));
                rollo.Product_Name = reader.GetString(reader.GetOrdinal("product_name"));
                rollo.Roll_Id = reader.GetString(reader.GetOrdinal("roll_id"));
                rollo.UniqueCode = reader.GetString(reader.GetOrdinal("unique_code"));
                rollo.Code_Person = reader.GetString(reader.GetOrdinal("code_person"));
                rollo.Width = (double)reader.GetDecimal(reader.GetOrdinal("width"));
                rollo.Length = (double)reader.GetDecimal(reader.GetOrdinal("large"));
                rollo.Msi = (double)reader.GetDecimal(reader.GetOrdinal("msi"));
                rollo.Ubicacion = reader.GetString(reader.GetOrdinal("ubic"));
                rollo.RollNumber = reader.GetInt32(reader.GetOrdinal("roll_number"));
                rollo.Numero = Convert.ToString(reader.GetInt32(reader.GetOrdinal("numero")));
                rollo.Status = reader.GetString(reader.GetOrdinal("status"));
                rollo.Paleta = "0";
                rollo.Tipo = "rollo cortado";
                rollo.Tipo_mov = "salida";
                rollo.Disponible = reader.GetBoolean(reader.GetOrdinal("disponible"));
            }
        }
        catch (SqlException ex)
        {
            ServiceErrors.Report("error al buscar por codigo unico" + ex);
        }
        return rollo;
    }


}
