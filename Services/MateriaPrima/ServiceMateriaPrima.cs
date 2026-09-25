using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.MateriaPrima;

public class ServiceMateriaPrima : IServiceMateriaPrima
{
    public IConfiguration Config { get; }
    private readonly IServiceCommonData ServiceData;
    public string StringConnex { get; set; } = null!;
    public DataSet Ds = new();
    public SqlDataAdapter DaMateria = new();
    public DataTable DtMateria = new();
    public SqlDataAdapter DaDetalle = new();
    public DataTable DtDetalle = new();
    public SqlDataAdapter DaProvider = new();
    public DataTable DtProvider = new();
    public SqlDataAdapter DaTransport = new();
    public DataTable DtTransport = new();
    public SqlDataAdapter DaProducts = new();
    public DataTable DtProducts = new();
    public SqlDataAdapter DaPerson = new();
    public DataTable DtPerson = new();

    public readonly Dictionary<string, (string query, string message, SqlDataAdapter adapter, string dataTableName)> mapTables;

    public ServiceMateriaPrima(IConfiguration Config, IServiceCommonData ServiceData)
    {
        this.Config = Config;
        //Carga el string de Connexion de la aplicacion.
        if (Config != null)
        {
            string ambiente = Config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
            StringConnex = Config.GetSection("ConnectionStringsEnvironment")[ambiente]!;
        }
        //Injecta el servicio de datos.
        this.ServiceData = ServiceData;

        mapTables = new Dictionary<string, (string, string, SqlDataAdapter, string)>
        {
            ["master"] = (R.SQL_STRING_QUERY.SELECT_QUERY_MP_MASTER, R.ERROR_MESSAGE_SYSTEM.ERROR_LOAD_MP_MASTER, DaMateria, "DtMateria"),
            ["details"] = (R.SQL_STRING_QUERY.SELECT_QUERY_MP_DETAILS, R.ERROR_MESSAGE_SYSTEM.ERROR_MP_DETAILS, DaDetalle, "DtDetalle"),
            ["products"] = (R.SQL_STRING_QUERY.SELECT_QUERY_PRODUCTS, R.ERROR_MESSAGE_SYSTEM.ERROR_LOAD_PRODUCTS, DaProducts, "DtProducts"),
            ["prov"] = (R.SQL_STRING_QUERY.SELECT_QUERY_PROVEEDORES, R.ERROR_MESSAGE_SYSTEM.ERROR_MP_PROVEEDORES, DaProvider, "DtProvider"),
            ["transport"] = (R.SQL_STRING_QUERY.SELECT_QUERY_TRANSPORTISTA, R.ERROR_MESSAGE_SYSTEM.ERROR_MP_TRANSPORT, DaTransport, "DtTransport"),
            ["person"] = (R.SQL_STRING_QUERY.SELECT_QUERY_PERSON, R.ERROR_MESSAGE_SYSTEM.ERROR_LOAD_PERSON, DaPerson, "DtPerson")
        };
    }
    public async Task LoadTableByName(string tableName)
    {
        await ServiceData.LoadTable(QUERY_COMMANDS(tableName));
    }
    public async Task<DataSet> LoadData()
    {
        LimpiarDataSet();
        await LoadTableHeaderMateriaPrima();
        await LoadTableDetailsMateriaPrima();
        await LoadTableProveedores();
        await LoadTableTransportista();
        await LoadProducts();
        await LoadPerson();
        await SetRelationsTables();
        return Ds;
    }
    public async Task LoadTableHeaderMateriaPrima() => await LoadTableByName("master");
    public async Task LoadTableDetailsMateriaPrima() => await LoadTableByName("details");
    public async Task LoadProducts() => await LoadTableByName("products");
    public async Task LoadTableProveedores() => await LoadTableByName("prov");
    public async Task LoadTableTransportista() => await LoadTableByName("transport");
    public async Task LoadPerson() => await LoadTableByName("person");
    private ObjectQuery QUERY_COMMANDS(string table)
    {
        if (!mapTables.TryGetValue(table, out (string query, string message, SqlDataAdapter adapter, string dataTableName) props))
        {
            throw new ArgumentException($"Invalid table name at create object query: {table}");
        }

        return new ObjectQuery
        {
            Query = props.query,
            Message = props.message,
            Adapter = props.adapter,
            DataTableName = props.dataTableName,
            DataSet = Ds
        };
    }
    public async Task SetRelationsTables()
    {
        await Task.Run(() =>
        {
            CreateRelation();
        });
    }
    private void CreateRelation()
    {
        // Tablas validadas contra mapTables: DtMateria, DtDetalle, DtProducts, DtProvider, DtTransport, DtPerson
        if (!Ds.Tables.Contains("DtMateria") || !Ds.Tables.Contains("DtDetalle") || !Ds.Tables.Contains("DtProducts")
            || !Ds.Tables.Contains("DtProvider") || !Ds.Tables.Contains("DtTransport") || !Ds.Tables.Contains("DtPerson"))
        {
            throw new InvalidOperationException("No se pudieron crear relaciones: faltan tablas en el DataSet. Verifique mapTables y LoadData.");
        }

        // relacion details-products.
        DataColumn ParentCol1 = Ds.Tables["DtProducts"]!.Columns["product_id"]!;
        DataColumn ChildCol1 = Ds.Tables["DtDetalle"]!.Columns["product_id"]!;
        DataRelation details_products = new("DETAILS_PRODUCTS", ParentCol1, ChildCol1, false);
        Ds.Relations.Add(details_products);
        Ds.Tables["DtDetalle"]!.Columns.Add("product_name", Type.GetType("System.String")!, "parent(DETAILS_PRODUCTS).Product_Name");
        // relacion proveedores-master.
        DataColumn ParentCol2 = Ds.Tables["DtProvider"]!.Columns["proveedor_id"]!;
        DataColumn ChildCol2 = Ds.Tables["DtMateria"]!.Columns["proveedor_id"]!;
        DataRelation master_provider = new("MASTER_PROVIDER", ParentCol2, ChildCol2, false);
        Ds.Relations.Add(master_provider);
        Ds.Tables["DtMateria"]!.Columns.Add("proveedor_name", Type.GetType("System.String")!, "parent(MASTER_PROVIDER).Proveedor_Name");
        // relacion transportista-master.
        DataColumn ParentCol3 = Ds.Tables["DtTransport"]!.Columns["transport_id"]!;
        DataColumn ChildCol3 = Ds.Tables["DtMateria"]!.Columns["transport_id"]!;
        DataRelation master_transport = new("MASTER_TRANSPORT", ParentCol3, ChildCol3, false);
        Ds.Relations.Add(master_transport);
        Ds.Tables["DtMateria"]!.Columns.Add("transport_name", Type.GetType("System.String")!, "parent(MASTER_TRANSPORT).Transport_Name");
        // relacion persona-master.
        DataColumn ParentCol4 = Ds.Tables["DtPerson"]!.Columns["person_id"]!;
        DataColumn ChildCol4 = Ds.Tables["DtMateria"]!.Columns["person_id"]!;
        DataRelation master_person = new("MASTER_PERSON", ParentCol4, ChildCol4, false);
        Ds.Relations.Add(master_person);
        Ds.Tables["DtMateria"]!.Columns.Add("person_name", Type.GetType("System.String")!, "parent(MASTER_PERSON).Person_Name");
        // relacion master-details.
        DataColumn ParentCol0 = Ds.Tables["DtMateria"]!.Columns["numero"]!;
        DataColumn ChildCol0 = Ds.Tables["DtDetalle"]!.Columns["numero"]!;
        DataRelation master_details = new(R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS, ParentCol0, ChildCol0, false);
        Ds.Relations.Add(master_details);
    }
    private void LimpiarDataSet()
    {
        //limpiar el dataset cualdo se carga el form varias veces.
        if (Ds.Tables.Count > 0)
        {
            DataTable tabla = Ds.Tables["DtMateria"]!;

            // Eliminar todas las restricciones del master
            List<Constraint> tempConstraints = tabla.Constraints.Cast<Constraint>().ToList();
            foreach (Constraint constraint in tempConstraints)
            {
                tabla.Constraints.Remove(constraint);
            }
            //eliminar las relaciones
            Ds.Relations.Clear();
            Ds.Tables.Clear();
            Ds.Clear();
            Ds.AcceptChanges();
        }
    }
    public bool GuardarOrden(OrdenMP orden)
    {
        using SqlConnection conn = new(StringConnex);
        conn.Open();
        using SqlTransaction transaction = conn.BeginTransaction();
        try
        {
            //Guardo el header de la Orden
            using SqlCommand comando = new()
            {
                Connection = conn,
                Transaction = transaction,
                CommandType = CommandType.Text,
                CommandText = "INSERT INTO OrdenMateria (numero,fecha_pro,fecha_recepcion,orden_compra,proveedor_id,persona_respons,notas,transport_id,guia_import,lote,doc_embarque,anulado,CloseDocument,total_cantidad,person_id,estado,fecha_hora_close) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17)"
            };
            comando.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar, 30) { Value = orden.Numero });
            comando.Parameters.Add(new SqlParameter("@p2", SqlDbType.DateTime) { Value = orden.Fecha_Produccion });
            comando.Parameters.Add(new SqlParameter("@p3", SqlDbType.DateTime) { Value = orden.Fecha_Recepcion });
            comando.Parameters.Add(new SqlParameter("@p4", SqlDbType.NVarChar, 30) { Value = orden.Orden_Compra });
            comando.Parameters.Add(new SqlParameter("@p5", SqlDbType.UniqueIdentifier) { Value = orden.Proveedor_id });
            comando.Parameters.Add(new SqlParameter("@p6", SqlDbType.NVarChar, 35) { Value = orden.Person_Name });
            comando.Parameters.Add(new SqlParameter("@p7", SqlDbType.Text) { Value = orden.Notas });
            comando.Parameters.Add(new SqlParameter("@p8", SqlDbType.UniqueIdentifier) { Value = orden.Transport_id });
            comando.Parameters.Add(new SqlParameter("@p9", SqlDbType.NVarChar, 30) { Value = orden.Guia });
            comando.Parameters.Add(new SqlParameter("@p10", SqlDbType.NVarChar, 30) { Value = orden.Lote });
            comando.Parameters.Add(new SqlParameter("@p11", SqlDbType.NVarChar, 30) { Value = orden.Numero_Embarque });
            comando.Parameters.Add(new SqlParameter("@p12", SqlDbType.Bit) { Value = false });
            comando.Parameters.Add(new SqlParameter("@p13", SqlDbType.Bit) { Value = orden.CloseDocument });
            comando.Parameters.Add(new SqlParameter("@p14", SqlDbType.Int) { Value = orden.Renglones });
            comando.Parameters.Add(new SqlParameter("@p15", SqlDbType.UniqueIdentifier) { Value = orden.Person_Id });
            comando.Parameters.Add(new SqlParameter("@p16", SqlDbType.NChar, 20) { Value = "open" });
            comando.Parameters.Add(new SqlParameter("@p17", SqlDbType.DateTime) { Value = DateTime.Now });

            comando.ExecuteNonQuery();

            //guardar el detalle de la orden.
            foreach (OrdenDetailsMP item in orden.Items)
            {
                SqlCommand comandoItems = new()
                {
                    Connection = conn,
                    Transaction = transaction,
                    CommandText = "INSERT INTO ItemsMateria (numero,product_id,cant_pedido,cant_real,width,length,msi,rollid,splice,ubicacion,core,largo_restante,estado,empalme,num_paleta,fecha_produccion,fecha_llegada,factura) VALUES (@p1,@p2,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@restante,@estado,@num_empalme,@num_paleta,@fecpro,@fecIngr,@fac)"
                };
                comandoItems.Parameters.Add(new SqlParameter("@p1", SqlDbType.NVarChar, 25) { Value = orden.Numero });
                comandoItems.Parameters.Add(new SqlParameter("@p2", SqlDbType.NVarChar, 25) { Value = item.Product_Id });
                comandoItems.Parameters.Add(new SqlParameter("@p3", SqlDbType.NChar, 10) { Value = item.Product_Type });
                comandoItems.Parameters.Add(new SqlParameter("@p4", SqlDbType.Decimal) { Value = item.Cantidad_Pedido });
                comandoItems.Parameters.Add(new SqlParameter("@p5", SqlDbType.Decimal) { Value = item.Cantidad_Real });
                comandoItems.Parameters.Add(new SqlParameter("@p6", SqlDbType.Decimal) { Value = item.Width });
                comandoItems.Parameters.Add(new SqlParameter("@p7", SqlDbType.Decimal) { Value = item.Length });
                comandoItems.Parameters.Add(new SqlParameter("@p8", SqlDbType.Decimal) { Value = item.Msi });
                comandoItems.Parameters.Add(new SqlParameter("@p9", SqlDbType.NVarChar, 20) { Value = item.RollId });
                comandoItems.Parameters.Add(new SqlParameter("@p10", SqlDbType.Int) { Value = item.Splice });
                comandoItems.Parameters.Add(new SqlParameter("@p11", SqlDbType.NChar, 15) { Value = item.Ubicacion });
                comandoItems.Parameters.Add(new SqlParameter("@p12", SqlDbType.Decimal) { Value = item.Core });
                comandoItems.Parameters.Add(new SqlParameter("@restante", SqlDbType.Decimal) { Value = item.Length });
                comandoItems.Parameters.Add(new SqlParameter("@estado", SqlDbType.NVarChar, 30) { Value = item.Estado });

                comandoItems.Parameters.Add(new SqlParameter("@num_empalme", SqlDbType.Int) { Value = item.Num_empalme });
                comandoItems.Parameters.Add(new SqlParameter("@num_paleta", SqlDbType.NVarChar, 20) { Value = item.Num_Paleta });
                comandoItems.Parameters.Add(new SqlParameter("@fecpro", SqlDbType.DateTime) { Value = item.Fecha_produccion });
                comandoItems.Parameters.Add(new SqlParameter("@fecIngr", SqlDbType.DateTime) { Value = item.Fecha_Ingreso });
                comandoItems.Parameters.Add(new SqlParameter("@fac", SqlDbType.NChar, 20) { Value = item.Factura });











                comandoItems.ExecuteNonQuery();
            }
            transaction.Commit();
            return true;
        }
        catch (Exception ex)
        {
            transaction.Rollback();
            ServiceLogger.LogError("GuardarOrden", ex);
            ServiceErrors.Report("error al tratar de grabar la orden..." + ex);
            return false;
        }
    }
    public int LoadConsecOrden(string filtro)
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
    public bool UpdateConsecOrden(string NumConsec)
    {
        try
        {
            using SqlConnection conn = new(StringConnex);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update control set par1=@p1 where filter='CMP'",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@p1", NumConsec);
            comando.Parameters.Add(p1);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (SqlException ex)
        {
            ServiceLogger.LogError("UpdateConsecOrden", ex);
            ServiceErrors.Report("Error al actualizar el consecutivo de la orden MATERIA PRIMA. Codigo de Error : " + ex.Message);
            return false;
        }
    }
    public bool AnularOrden(string orden)
    {
        try
        {
            using SqlConnection conn = new(StringConnex);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update OrdenMateria SET Anulado=1 where numero=@p1",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@p1", orden);
            comando.Parameters.Add(p1);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (Exception ex)
        {
            ServiceLogger.LogError("AnularOrden", ex);
            ServiceErrors.Report("Error al tratar de Anular el documento...[Codigo de Error:]" + ex.Message);
            return false;
        }
    }
    public bool CloseOrder(string orden)
    {
        try
        {
            using SqlConnection conn = new(StringConnex);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update OrdenMateria SET CloseDocument=1 where numero=@p1",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@p1", orden);
            comando.Parameters.Add(p1);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (SqlException ex)
        {
            ServiceLogger.LogError("CloseOrder", ex);
            ServiceErrors.Report("Error al tratar de cerrar el documento...[Codigo de Error:]" + ex.Message);
            return false;
        }
    }
    public bool UpDateLogsNotes(string orden, string logText)
    {
        try
        {
            using SqlConnection conn = new(StringConnex);
            conn.Open();
            using SqlCommand comando = new()
            {
                Connection = conn,
                CommandText = "update OrdenMateria SET notas = CONCAT(ISNULL(notas,''),CHAR(13),CHAR(10),@p2) where numero=@p1",
                CommandType = CommandType.Text
            };
            SqlParameter p1 = new("@p1", orden);
            SqlParameter p2 = new("@p2", logText);
            comando.Parameters.Add(p1);
            comando.Parameters.Add(p2);
            comando.ExecuteNonQuery();
            return true;
        }
        catch (SqlException ex)
        {
            ServiceLogger.LogError("UpDateLogsNotes", ex);
            ServiceErrors.Report("Error al tratar de actualizar los logs del documentos...[Codigo de Error:]" + ex.Message);
            return false;
        }
    }
}
