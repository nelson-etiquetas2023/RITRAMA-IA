using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.ProductsService
{

    public class ProductsService : IProductsService
    {
        public IConfiguration Configuration { get; set; } = null!;
        IServiceCommonData CommondData;
        DataSet Ds = new();
        DataTable DtProducts = new();
        SqlDataAdapter DaProducts = new();

        public string StringConnex { get; set; } = null!;
        public ProductsService(IServiceCommonData commonData, IConfiguration configuration)
        {
            CommondData = commonData;
            Configuration = configuration;
            if (Configuration != null)
            {
                var ambiente = Configuration["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
                StringConnex = Configuration.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]!;
            }
        }

        public async Task<DataSet> Load(CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();
            var sqlQuery = R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS;

            DtProducts = await DataAccess.ExecuteQuery<Product>(StringConnex, sqlQuery, null, false).ConfigureAwait(false);
            Ds.Tables.Add(DtProducts);
            return Ds;

        }
        public async Task<bool> Add(Product producto)
        {
            try
            {
                var sqlQuery = "INSERT INTO producto (Product_ID,Product_Name,Product_Descrip,Product_Ref,Codebar,MasterRolls,rollo_cortado,Resmas,Graphics,anulado,precio,ratio) VALUES (@product_id,@product_name,@product_description,@reference,@codebar,@master,@rollo,@resma,@graphics,@anulado,@precio,@ratio)";

                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@product_id", SqlDbType.NVarChar) { Value = producto.Product_id },
                    new SqlParameter("@product_name", SqlDbType.NVarChar) { Value = producto.Product_Name},
                    new SqlParameter("@product_description", SqlDbType.NVarChar) { Value = producto.Product_Description},
                    new SqlParameter("@reference", SqlDbType.NVarChar) { Value = producto.Referencia},
                    new SqlParameter("@codebar", SqlDbType.NVarChar) { Value = producto.Codigo_Barra},
                    new SqlParameter("@master", SqlDbType.Bit) { Value = producto.Master},
                    new SqlParameter("@rollo", SqlDbType.Bit) { Value = producto.RolloCortado},
                    new SqlParameter("@resma", SqlDbType.Bit) { Value = producto.Hoja},
                    new SqlParameter("@graphics", SqlDbType.Bit) { Value = producto.Graphics},
                    new SqlParameter("@anulado", SqlDbType.Bit) { Value = producto.Anulado },
                    new SqlParameter("@precio", SqlDbType.Decimal) { Value = producto.Precio },
                    new SqlParameter("@ratio", SqlDbType.Decimal) { Value = producto.Ratio },
                };

                var data = await DataAccess.ExecuteQueryWrite(StringConnex, sqlQuery, parametros, true);
                return data;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.Add", ex);
                ServiceErrors.Report("error al agregar un producto nuevo...");
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

                var result = (int)comando.ExecuteScalar();
                if (result > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception)
            {
                return false;
            }
        }
        public async Task<bool> Update(Product producto)
        {

            try
            {
                // FIX P0: sintaxis inválida @param = columna. Debe ser columna = @param
                string sqlQuery = "UPDATE dbo.producto SET Product_Name = @name,Product_Descrip = @descrip,Product_Ref = @reference,codebar=@barra,precio=@precio,ratio=@ratio,masterRolls=@master,graphics=@graphics,resmas=@hoja,rollo_cortado=@rollo WHERE product_id = @id";

                var parametros = new List<SqlParameter>
                {
                    new SqlParameter("@id", SqlDbType.NVarChar) { Value = producto.Product_id },
                    new SqlParameter("@name", SqlDbType.NVarChar) { Value = producto.Product_Name},
                    new SqlParameter("@descrip", SqlDbType.NVarChar) { Value = producto.Product_Description},
                    new SqlParameter("@reference", SqlDbType.NVarChar) { Value = producto.Referencia},
                    new SqlParameter("@barra", SqlDbType.NVarChar) { Value = producto.Codigo_Barra},
                    new SqlParameter("@precio", SqlDbType.Decimal) { Value = producto.Precio},
                    new SqlParameter("@ratio", SqlDbType.Decimal) { Value = producto.Ratio},
                    new SqlParameter("@master", SqlDbType.Bit) { Value = producto.Master },
                    new SqlParameter("@graphics", SqlDbType.Bit) { Value = producto.Graphics },
                    new SqlParameter("@hoja", SqlDbType.Bit) { Value = producto.Hoja },
                    new SqlParameter("@rollo", SqlDbType.Bit) { Value = producto.RolloCortado },
                };

                var data = await DataAccess.ExecuteQueryWrite(StringConnex, sqlQuery, parametros, true);
                return data;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("ProductsService.Update", ex);
                ServiceErrors.Report("Error al modificar los datos del producto [Codigo Error:] " + ex.Message);
                return false;
            }
        }
        public bool Anular(string IdProduct)
        {
            return false;
        }





    }
}
