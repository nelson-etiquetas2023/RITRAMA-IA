using System.Data;
using DocumentFormat.OpenXml.Office.CoverPageProps;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;
using static Ritrama2025.R.QUERY;

namespace Ritrama2025.Services.InventarioService
{
    public class InventarioService : IInventarioService
    {
        public IConfiguration Config { get; }
        public string StringConnex { get; set; } = null!;

        public InventarioService(IConfiguration Config)
        {
            this.Config = Config;
            //Carga el string de Connexion de la aplicacion.
            if (Config != null)
            {
                string ambiente = Config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
                StringConnex = Config.GetSection("ConnectionStringsEnvironment")[ambiente]!;
            }
        }

        public List<string> GetExistingProductIds(IEnumerable<string> productIds)
        {
            List<string> ids = productIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ids.Count == 0)
            {
                return new List<string>();
            }

            try
            {
                SqlParameter[] parameters = ids
                    .Select((id, index) => new SqlParameter($"@p{index}", SqlDbType.NVarChar, 50) { Value = id })
                    .ToArray();

                string sql = $"SELECT DISTINCT product_id FROM producto WHERE product_id IN ({string.Join(", ", parameters.Select((_, index) => $"@p{index}"))})";

                using SqlConnection conn = new(StringConnex);
                conn.Open();

                using SqlCommand comando = new(sql, conn)
                {
                    CommandType = CommandType.Text
                };
                comando.Parameters.AddRange(parameters);

                using SqlDataReader reader = comando.ExecuteReader();
                List<string> resultado = new List<string>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        resultado.Add(reader.GetString(0));
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al consultar IDs de producto existentes. Error code: " + ex.Message);
                return new List<string>();
            }
        }

        public List<string> GetExistingRollIds(IEnumerable<string> rollIds)
        {
            List<string> ids = rollIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ids.Count == 0)
            {
                return new List<string>();
            }

            try
            {
                SqlParameter[] parameters = ids
                    .Select((id, index) => new SqlParameter($"@p{index}", SqlDbType.NVarChar, 50) { Value = id })
                    .ToArray();

                string sql = $"SELECT DISTINCT roll_id FROM MasterInic WHERE roll_id IN ({string.Join(", ", parameters.Select((_, index) => $"@p{index}"))})";

                using SqlConnection conn = new(StringConnex);
                conn.Open();

                using SqlCommand comando = new(sql, conn)
                {
                    CommandType = CommandType.Text
                };
                comando.Parameters.AddRange(parameters);

                using SqlDataReader reader = comando.ExecuteReader();
                List<string> resultado = new List<string>();
                while (reader.Read())
                {
                    if (!reader.IsDBNull(0))
                    {
                        resultado.Add(reader.GetString(0));
                    }
                }

                return resultado;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al consultar IDs de rollos existentes. Error code: " + ex.Message);
                return new List<string>();
            }
        }

        public bool BorrarMasterDB(string rollid)
        {
            return BorrarMastersDB(new[] { rollid });
        }

        public bool BorrarMastersDB(IEnumerable<string> rollIds)
        {
            List<string> ids = rollIds
                .Where(id => !string.IsNullOrWhiteSpace(id))
                .Select(id => id.Trim())
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();

            if (ids.Count == 0)
            {
                return true;
            }

            try
            {
                using SqlConnection conn = new(StringConnex);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();

                string sqlIn = string.Join(", ", ids.Select((_, index) => $"@p{index}"));

                SqlParameter[] parameters1 = ids
                    .Select((id, index) => new SqlParameter($"@p{index}", SqlDbType.NVarChar, 50) { Value = id })
                    .ToArray();

                using (SqlCommand comando1 = new($"DELETE FROM masterInic WHERE roll_id IN ({sqlIn})", conn, transaction))
                {
                    comando1.Parameters.AddRange(parameters1);
                    comando1.ExecuteNonQuery();
                }

                SqlParameter[] parameters2 = ids
                    .Select((id, index) => new SqlParameter($"@p{index}", SqlDbType.NVarChar, 50) { Value = id })
                    .ToArray();

                using (SqlCommand comando2 = new($"DELETE FROM ItemsMateria WHERE rollid IN ({sqlIn})", conn, transaction))
                {
                    comando2.Parameters.AddRange(parameters2);
                    comando2.ExecuteNonQuery();
                }

                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al eliminar varios masters del inventario. Error code: " + ex.Message);
                return false;
            }
        }

        public async Task<DataTable?> BuscarRollosCortadosInventario(string? rollid, string? productId, string? productName, string? ubicacion, string? uniqueCode, string? codePerson, string? numeroOC)
        {
            try
            {
                string sql = R.QUERY.PRODUCTION.SQL_QUERY_LOAD_INVENTARIO_ROLLO_CORTADO;
                List<string> filtros = new List<string>();
                List<SqlParameter> parametros = new List<SqlParameter>();
                AgregarFiltroLike(filtros, parametros, "roll_id", rollid, "rollid");
                AgregarFiltroLike(filtros, parametros, "product_id", productId, "productId");
                AgregarFiltroLike(filtros, parametros, "product_name", productName, "productName");
                AgregarFiltroLike(filtros, parametros, "ubic", ubicacion, "ubicacion");
                AgregarFiltroLike(filtros, parametros, "unique_code", uniqueCode, "uniqueCode");
                AgregarFiltroLike(filtros, parametros, "code_person", codePerson, "codePerson");
                if (!string.IsNullOrWhiteSpace(numeroOC))
                {
                    filtros.Add("CAST(numero AS NVARCHAR(20)) LIKE @numeroOC");
                    parametros.Add(new SqlParameter("@numeroOC", SqlDbType.NVarChar, 20) { Value = "%" + numeroOC.Trim() + "%" });
                }

                if (filtros.Count > 0)
                {
                    sql += " WHERE " + string.Join(" AND ", filtros);
                }

                DataTable? dt = await CargarTablaAsync(sql, false, parametros.ToArray(), "rolls_details", true);
                return dt ?? throw new InvalidOperationException("La busqueda de rollos cortados no devolvio datos.");
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("error al buscar los rollos cortados [error code: ] " + ex.Message);
                return null;
            }
        }

        public async Task<DataTable?> BuscarMasterInventario(string? rollid, string? productId, string? productName, string? ubicacion, string? estado)
        {
            try
            {
                const string orderBy = "ORDER BY Roll_Id";
                string baseSql = R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID_INVENTARIO.TrimEnd();
                if (baseSql.EndsWith(orderBy, StringComparison.OrdinalIgnoreCase))
                {
                    baseSql = baseSql[..^orderBy.Length].TrimEnd();
                }

                List<string> filtros = new List<string>();
                List<SqlParameter> parametros = new List<SqlParameter>();
                AgregarFiltroLike(filtros, parametros, "Roll_Id", rollid, "rollid");
                AgregarFiltroLike(filtros, parametros, "Part_Number", productId, "productId");
                AgregarFiltroLike(filtros, parametros, "Product_Name", productName, "productName");
                AgregarFiltroLike(filtros, parametros, "Ubicacion", ubicacion, "ubicacion");
                AgregarFiltroLike(filtros, parametros, "estado", estado, "estado");

                string sql = baseSql;
                if (filtros.Count > 0)
                {
                    sql += " AND " + string.Join(" AND ", filtros);
                }

                sql += " " + orderBy;

                DataTable? dt = await CargarTablaAsync(sql, false, (parametros.Count > 0) ? parametros.ToArray() : null, "MasterInics", true);
                return dt ?? throw new InvalidOperationException("La busqueda de masters no devolvio datos.");
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("error al buscar los masters [error code: ] " + ex.Message);
                return null;
            }
        }

        private static void AgregarFiltroLike(List<string> filtros, List<SqlParameter> parametros, string columna, string? valor, string nombreParametro)
        {
            if (string.IsNullOrWhiteSpace(valor))
            {
                return;
            }

            filtros.Add($"{columna} LIKE @{nombreParametro}");
            parametros.Add(new SqlParameter($"@{nombreParametro}", SqlDbType.NVarChar, 100) { Value = "%" + valor.Trim() + "%" });
        }

        public bool SaveMasterInitialDB(ProductMAP producto)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandType = CommandType.Text,
                    CommandText = "INSERT INTO MasterInic (part_number,disponible,OrderPurchase,width,lenght,roll_id,splice,ubicacion,core,anulado,master,resma,graphics,embarque,fecha_pro,fecha_reg,width_c,lenght_c,palet_num) VALUES (@product_id,@dispo,@order,@wid,@len,@rollid,@splice,@ubic,@core,@anulado,@master,@resma,@graphics,@embarque,@fecha_pro,@fecha_reg,@wid_c,@len_c,@palet)"
                };
                comando.Parameters.Add(new SqlParameter("@product_id", SqlDbType.NVarChar, 25) { Value = producto.Product_Id });
                comando.Parameters.Add(new SqlParameter("@dispo", SqlDbType.Bit) { Value = true });
                comando.Parameters.Add(new SqlParameter("@order", SqlDbType.Int) { Value = 1 });
                comando.Parameters.Add(new SqlParameter("@wid", SqlDbType.Decimal) { Value = producto.Width });
                comando.Parameters.Add(new SqlParameter("@len", SqlDbType.Decimal) { Value = producto.Length });
                comando.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar, 25) { Value = producto.Rollid });
                comando.Parameters.Add(new SqlParameter("@splice", SqlDbType.Int) { Value = producto.Splice });
                comando.Parameters.Add(new SqlParameter("@ubic", SqlDbType.NVarChar, 15) { Value = producto.Ubic });
                comando.Parameters.Add(new SqlParameter("@core", SqlDbType.Decimal) { Value = 0 });
                comando.Parameters.Add(new SqlParameter("@anulado", SqlDbType.Bit) { Value = false });
                comando.Parameters.Add(new SqlParameter("@master", SqlDbType.Bit) { Value = true });
                comando.Parameters.Add(new SqlParameter("@resma", SqlDbType.Bit) { Value = false });
                comando.Parameters.Add(new SqlParameter("@graphics", SqlDbType.Bit) { Value = false });
                comando.Parameters.Add(new SqlParameter("@embarque", SqlDbType.VarChar, 25) { Value = producto.Factura });
                comando.Parameters.Add(new SqlParameter("@fecha_pro", SqlDbType.DateTime) { Value = producto.Fecha_Produccion });
                comando.Parameters.Add(new SqlParameter("@fecha_reg", SqlDbType.DateTime) { Value = producto.Fecha_Llegada });
                comando.Parameters.Add(new SqlParameter("@wid_c", SqlDbType.Decimal) { Value = 0 });
                comando.Parameters.Add(new SqlParameter("@len_c", SqlDbType.Decimal) { Value = 0 });
                comando.Parameters.Add(new SqlParameter("@palet", SqlDbType.VarChar, 25) { Value = producto.Paleta });
                comando.ExecuteNonQuery();
                transaction.Commit();
                return true;
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al guardar los datos en la base de datos. Error code: " + ex.Message);
                return false;
            }
        }

        public bool ValidProductid(string id)
        {
            try
            {
                using SqlConnection Conn = new(StringConnex);
                Conn.Open();

                using SqlCommand comando = new()
                {
                    Connection = Conn,
                    CommandType = CommandType.Text,
                    CommandText = "select COUNT(*) from producto where product_id = @id"
                };

                SqlParameter p1 = new("@id", id);
                comando.Parameters.Add(p1);

                int result = (int)comando.ExecuteScalar();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al validar el product ID. Error code: " + ex.Message);
                return false;
            }
        }

        public bool InsertProduct(Product producto)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandType = CommandType.Text,
                    CommandText = "INSERT INTO producto (product_id,product_name,product_descrip,anulado,masterRolls,graphics,resmas,rollo_cortado) VALUES (@product_id,@name,@descrip,@anulado,@master,@graphics,@hojas,@rollo)"
                };
                comando.Parameters.Add(new SqlParameter("@product_id", SqlDbType.NVarChar, 25) { Value = producto.Product_id });
                comando.Parameters.Add(new SqlParameter("@name", SqlDbType.NVarChar, 200) { Value = producto.Product_Name });
                comando.Parameters.Add(new SqlParameter("@descrip", SqlDbType.NVarChar, 200) { Value = producto.Product_Description });
                comando.Parameters.Add(new SqlParameter("@anulado", SqlDbType.Bit) { Value = producto.Anulado });
                comando.Parameters.Add(new SqlParameter("@master", SqlDbType.Bit) { Value = producto.Master });
                comando.Parameters.Add(new SqlParameter("@graphics", SqlDbType.Bit) { Value = producto.Graphics });
                comando.Parameters.Add(new SqlParameter("@hojas", SqlDbType.Bit) { Value = producto.Hoja });
                comando.Parameters.Add(new SqlParameter("@rollo", SqlDbType.Bit) { Value = producto.RolloCortado });
                comando.ExecuteNonQuery();
                transaction.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("Error al tratar de registrar los productos, en el modulo de inventario...[error code: ] " + ex.Message);
                return false;

            }
        }

        private async Task<DataTable?> CargarTablaAsync(
            string sqlQuery,
            bool loadDataset = false,
            SqlParameter[]? parametros = null,
            string? nombreTabla = null,
            bool returnDataTable = false)
        {

            using SqlConnection conn = new(StringConnex);
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
                DataSet dsTemp = new DataSet();
                adapter.Fill(dsTemp, nombreTabla!);
                return null;
            }

            if (returnDataTable)
            {
                // P0: I/O realmente asincrona (ExecuteReaderAsync) en lugar de adapter.Fill
                // sincronico, que bloqueaba el hilo de UI al hacer await desde el form.
                DataTable dt = new();
                using SqlDataReader reader = await comando.ExecuteReaderAsync(CommandBehavior.Default);
                dt.Load(reader);
                return dt;
            }

            await comando.ExecuteNonQueryAsync();
            return null;

        }

        public bool DropTableInit(int indexTable)
        {
            //limpia toda la tabla de Inventario de Masters.
            string sqlQuery = "";
            try
            {
                switch (indexTable)
                {
                    case 0:
                        sqlQuery = "DELETE FROM MasterInic";
                        break;
                    case 1:
                        sqlQuery = "DELETE FROM Rolls_Details";
                        break;
                    default:
                        // No se ha seleccionado una tabla valida para limpiar.
                        break;
                }
                using SqlConnection conn = new(StringConnex);
                conn.Open();
                using SqlTransaction transac = conn.BeginTransaction();
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transac,
                    CommandType = CommandType.Text,
                    CommandText = sqlQuery

                };
                comando.ExecuteNonQuery();
                transac.Commit();
                return true;
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("Error  al  tratar de limpiar la tabla de inventario " +
                    "inicial de masters: => codigo de error: => " + ex.Message);
                return false;

            }
        }

        public bool ValidRollId(string rollid)
        {
            //busca en la base de datos si existe un registro con un rollid especifico.
            try
            {
                using SqlConnection conn = new(StringConnex);
                conn.Open();
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "select COUNT(*) from MasterInic WHERE roll_id=@par1"
                };
                comando.Parameters.Add(new SqlParameter("@par1", SqlDbType.NVarChar, 25) { Value = rollid });
                int rows = (int)comando.ExecuteScalar();
                if (rows > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("Error en la consulta por roll-id...error code => " + ex.Message);
                return false;
            }
        }

        public bool SaveRolloCortado(RolloCortado rollo)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                conn.Open();
                using SqlTransaction transaction = conn.BeginTransaction();
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandType = CommandType.Text,
                    CommandText = "INSERT INTO rolls_details (numero,product_id,product_name,roll_number,unique_code,splice," +
                    "width,large,msi,code_person,ubic,roll_id,disponible,fecha) VALUES " +
                    "(@numero,@product_id,@product_name,@roll_number,@uniquecode," +
                    "@splice,@wid,@len,@msi,@code_per,@ubic,@rollid,@dispo,@fechacrea)"
                };
                comando.Parameters.Add(new SqlParameter("@numero", SqlDbType.Int) { Value = int.TryParse(rollo.Numero, out int numero) ? numero : 1000000 });
                comando.Parameters.Add(new SqlParameter("@product_id", SqlDbType.NChar, 50) { Value = rollo.Product_Id });
                comando.Parameters.Add(new SqlParameter("@product_name", SqlDbType.NVarChar, 250) { Value = rollo.Product_Name });
                comando.Parameters.Add(new SqlParameter("@roll_number", SqlDbType.Int) { Value = rollo.RollNumber });
                comando.Parameters.Add(new SqlParameter("@uniquecode", SqlDbType.NVarChar, 50) { Value = rollo.UniqueCode });
                comando.Parameters.Add(new SqlParameter("@splice", SqlDbType.Int) { Value = rollo.Splice });
                comando.Parameters.Add(new SqlParameter("@wid", SqlDbType.Decimal) { Value = rollo.Width });
                comando.Parameters.Add(new SqlParameter("@len", SqlDbType.Decimal) { Value = rollo.Length });
                comando.Parameters.Add(new SqlParameter("@msi", SqlDbType.Decimal) { Value = rollo.Msi });
                comando.Parameters.Add(new SqlParameter("@code_per", SqlDbType.NVarChar, 60) { Value = rollo.Code_Person });
                comando.Parameters.Add(new SqlParameter("@ubic", SqlDbType.NChar, 50) { Value = rollo.Ubicacion });
                comando.Parameters.Add(new SqlParameter("@rollid", SqlDbType.NVarChar, 50) { Value = string.IsNullOrEmpty(rollo.Roll_Id) ? "1" : rollo.Roll_Id });
                comando.Parameters.Add(new SqlParameter("@dispo", SqlDbType.Bit) { Value = true });
                comando.Parameters.Add(new SqlParameter("@fechacrea", SqlDbType.DateTime) { Value = DateTime.Today });




                comando.ExecuteNonQuery();
                transaction.Commit();

                return true;
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("Error al guardar los datos en la base de datos de " +
                    "inicial de rollos cortados. Error code: " + ex.Message);
                return false;

            }
        }
    }
}

