[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("Ritrama2025.Tests")]

namespace Ritrama2025
{
    public static class R
    {
        public class PARAMETERS
        {
            internal static string NAME_RELATION_OC_MASTER_DETAILS = "FK_MASTER_DETAILS";
        }
        public class QUERY
        {
            public class PRODUCTION
            {
                internal static string SQL_QUERY_LOAD_INVENTARIO_ROLLO_CORTADO = "SELECT product_id,product_name,roll_number,unique_code,width,large,msi,splice,numero,roll_id,code_person,status,disponible,ubic,fecha,despacho,fecha_despacho FROM rolls_details";
                internal static string SQL_QUERY_SELECT_LOAD_OC_HEADER = "SELECT numero,fecha,fecha_produccion,a.product_id,b.product_Name,rollid_1,width_1,lenght_1,rollid_2,width_2,lenght_2,util1_real_width,util1_real_lenght,util2_real_width,util2_real_lenght,rest1_width,rest1_lenght,rest2_width,rest2_lenght,a.operador_id,c.nombre,a.customer_id,d.customer_name,tot_inch_ancho,lenght_entrada,resta_entrada,total_salida,plus1_pies,plus2_pies,longitud_cortar,cortes_ancho,cortes_largo,cant_rollos,cant_rollos2,step,sellOrder,desperdicio,master_tipo,ubicacion,configvueltas,TwoMasters,longitud_cortar2,vueltas2,cantidad_rollos2,desperdicio2,anulada FROM orden_corte a LEFT JOIN producto b ON a.product_id = b.product_id LEFT JOIN operadores c ON a.operador_id = c.operador_id LEFT JOIN customer d ON a.customer_id = d.customer_id WHERE a.anulada = 0 AND a.CloseDocument = 0 ORDER BY numero";
                internal static string SQL_QUERY_SELECT_LOAD_OC_CORTES = "select num,width,lenght,msi,orden,code_person from cortes WHERE orden IN (SELECT numero FROM orden_corte WHERE anulada = 0 AND CloseDocument = 0)";
                internal static string SQL_QUERY_SELECT_LOAD_OC_ROLLO_CORTADO = "SELECT numero,product_id,product_name,roll_number,unique_code,splice,width,large,msi,roll_id,code_person,status,disponible,width_c,lenght_c,ubic,ratio,fecha,rollid_oculto,vuelta FROM rolls_details WHERE numero IN (SELECT numero FROM orden_corte WHERE anulada = 0 AND CloseDocument = 0)";

                internal static string SQL_QUERY_SELECT_LOAD_ROLL_ID = "WITH detalle_consumo AS (SELECT (x.consumo + CASE WHEN x.desperdicio = 1 THEN (x.leng - x.consumo) ELSE 0 END) AS ConsumoTotal, x.rollid FROM orden_corte oc CROSS APPLY (VALUES (oc.util1_real_lenght, oc.rollid_1, desperdicio, lenght_1, anulada, numero, TwoMasters), (oc.util2_real_lenght, oc.rollid_2, desperdicio2, lenght_2, anulada, numero, TwoMasters)) AS x(consumo, rollid, desperdicio, leng, anulada, numero, TwoMaster) WHERE x.consumo > 0 AND x.anulada = 0), consumo_total AS (SELECT rollid, SUM(ConsumoTotal) AS largo_consumido FROM detalle_consumo GROUP BY rollid) SELECT a.Roll_Id, a.Part_Number, b.Product_Name, a.Width, a.Lenght, ISNULL(a.msi,0) AS msi, a.fecha_pro, a.fecha_reg, a.splice, a.core, a.Ubicacion, 'Inic.' AS tipo_mov, ISNULL(ct.largo_consumido,0) AS largo_consumido, (a.Lenght - ISNULL(ct.largo_consumido,0)) AS largo_restante, CASE WHEN ISNULL(ct.largo_consumido,0) = 0 THEN 'Completo' WHEN (a.Lenght - ISNULL(ct.largo_consumido,0)) <= 0 THEN 'Agotado' ELSE 'Parcialmente Consumido' END AS estado FROM MasterInic a LEFT JOIN producto b ON a.Part_Number = b.product_id LEFT JOIN consumo_total ct ON a.Roll_Id = ct.rollid WHERE b.MasterRolls = 1 UNION ALL SELECT a.rollid, a.product_id, b.Product_Name, a.Width, a.length, ISNULL(a.msi,0) AS msi, a.fecha_produccion, a.fecha_llegada, a.splice, a.core, a.Ubicacion, 'Por Compras' AS tipo_mov, ISNULL(ct.largo_consumido,0) AS largo_consumido, (a.length - ISNULL(ct.largo_consumido,0)) AS largo_restante, CASE WHEN ISNULL(ct.largo_consumido,0) = 0 THEN 'Completo' WHEN (a.length - ISNULL(ct.largo_consumido,0)) <= 0 THEN 'Agotado' ELSE 'Parcialmente Consumido' END AS estado FROM ItemsMateria a LEFT JOIN producto b ON a.product_id = b.product_id LEFT JOIN consumo_total ct ON a.rollid = ct.rollid WHERE b.MasterRolls = 1 ORDER BY Roll_Id";

                internal static string SQL_QUERY_SELECT_LOAD_OPERATOR = "SELECT operador_id,nombre,status FROM operadores";
                internal static string SQL_QUERY_SELECT_LOAD_CUSTOMER = "SELECT customer_id,customer_name FROM customer";

                internal static string SQL_QUERY_ACTUALIZAR_INVENTARIO_INICIALES = "UPDATE MasterInic SET largo_consumido=largo_consumido+@consumo WHERE roll_id=@rollid";
                internal static string SQL_QUERY_ACTUALIZAR_INVENTARIO_MATERIA = "UPDATE ItemsMateria SET largo_consumido=largo_consumido+@consumo WHERE rollid=@rollid";

                internal static string UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES = "INSERT INTO MasterDetailsInic (rollid,orden,consumo,fecha_reg,desperdicio) VALUES(@rollid,@orden,@consumo,@fecha,@desperdicio)";

                internal static string SQL_SELECT_QUERY_LOAD_DETAILS_MASTER_INICIALES_END_TRANSACTIONS = "SELECT rollid,orden,consumo,fecha_reg,a.desperdicio,case when a.desperdicio=1 then 0 else b.cant_rollos end as cant_rollos,b.customer_id,c.Customer_Name FROM MasterDetailsInic a left join orden_corte b on a.orden=b.numero left join Customer c on b.customer_id=c.customer_id WHERE rollid=@rollid";

                internal static string SQL_SELECT_QUERY_LOAD_DETAILS_MASTER_INICIALES_START_TRANSACTIONS = "select a.rollid_1 as rollid,a.numero as orden, a.util1_real_lenght as consumo,fecha as fecha_reg,a.desperdicio,case when desperdicio=1 then (a.lenght_1 - a.util1_real_lenght) else 0 end as monto_desperdicio,a.cant_rollos,a.customer_id,b.Customer_Name from orden_corte a left join Customer b on a.customer_id = b.Customer_Id where rollid_1=@rollid";


            }
        }
        [Obsolete("Usar IConfiguration[\"ConnectionStringsEnvironment\"] + User Secrets. Eliminado en Sprint 1.")]
        public static class CONNECTIONSTRINGS
        {
            public static readonly string DESARROLLO = string.Empty;
            public static readonly string PRODUCCION = string.Empty;
        }
        public static class ENVIRONMET
        {
            public static readonly string DESARROLLO = "Desarrollo";
            public static readonly string PRODUCCION = "Produccion";
            public static readonly string NAME_KEY_CONNECTION = "ConnectionStringsEnvironment";
        }
        public static class CONSTANTES
        {
            public const double FACTOR_METROS_PULDADAS = 39.3701;

            public const double FACTOR_METROS_PIES = 3.28084;

            public const double FACTOR_PULGADAS_METROS = 0.0254;

            public const double FACTOR_PIES_METROS = 0.3048;

            public const double FACTOR_MM_PULGADAS = 0.0393701;
        }
        public class PATH_REPORTS
        {
            public const string REPORTS_DESPACHO = @"Reports";
            public const string REPORTS_PRODUCTION = @"Reports\Production\";
            public const string REPORTS_INVENTARIOS = @"Reports\Inventario\";

        }
        public class REPORT_NAME
        {
            public const string REPORT_OC = @"RptOC.rdlc";
        }
        public class REPORT_TITLE
        {
            public const string REPORT_OC = @"REPORTE DE ORDEN DE CORTE.";
        }
        public static class SQL_STRING_QUERY
        {
            internal readonly static string SELECT_QUERY_PROVEEDORES = "SELECT Proveedor_ID,Proveedor_Name,phone,direccion,email,anulado,unidad_master_1,unidad_master_2  FROM provider";

            internal readonly static string SELECT_QUERY_TRANSPORTISTA = "SELECT transport_id,transport_name FROM transporte";

            internal readonly static string SELECT_QUERY_PRODUCTS = "SELECT product_id,product_name,case when MasterRolls=1 then 'Master' when rollo_cortado=1 then 'Rollo Cortado' when resmas=1 then 'Resma' when Graphics=1 then 'Graphics' end as tipo,product_descrip,product_ref,codebar,category_id,masterRolls,rollo_cortado,resmas,graphics,anulado,precio,code_rc,ratio FROM producto";

            internal readonly static string SELECT_QUERY_MP_MASTER = "select numero,fecha_recepcion,fecha_pro,proveedor_id,orden_compra,persona_respons,notas,CloseDocument,Anulado,transport_id,guia_import,lote,doc_embarque,estado,total_cantidad,fecha_hora_close,anulado,person_id from OrdenMateria";

            internal readonly static string SELECT_QUERY_MP_DETAILS = "select numero,product_id,type,cant_pedido,cant_real,width,length,msi,rollid,splice,ubicacion,core,empalme,fecha_produccion,factura,num_paleta,fecha_llegada from ItemsMateria";

            internal readonly static string SELECT_QUERY_PERSON = "SELECT person_id, person_name FROM person";
        }
        public static class ERROR_MESSAGE_SYSTEM
        {
            internal static readonly string ERROR_LOAD_PRODUCTS = "error al cargar los productos en el modulo de materia prima. error code: ";

            internal static readonly string ERROR_LOAD_MP_MASTER = "error al cargar la tabla de encabezado de recepciones de materia prima. error code: ";
            internal static readonly string ERROR_MP_DETAILS = "error al cargar la tabla de detalle de recepciones de materia prima. error code: ";
            internal static readonly string ERROR_MP_PROVEEDORES = "error al cargar la tabla de proveedores en el modulo de la materia prima. error code: ";

            internal static readonly string ERROR_MP_TRANSPORT = "error al cargar la tabla de proveedores. error code: ";
            internal static readonly string ERROR_LOAD_PERSON = "error al cargar la tabla de PERSON. error code: ";
        }
        public static class COMMAND
        {
            internal static readonly string CREATE_QUERY_PRODUCTS = "";
        }
    }
}
