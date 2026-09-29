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

                internal static string SQL_QUERY_SELECT_LOAD_ROLL_ID = "WITH detalle_consumo AS (SELECT (x.consumo + CASE WHEN x.desperdicio = 1 THEN (x.leng - x.consumo) ELSE 0 END) AS ConsumoTotal, x.rollid FROM orden_corte oc CROSS APPLY (VALUES (oc.util1_real_lenght, oc.rollid_1, desperdicio, lenght_1, anulada, numero, TwoMasters), (oc.util2_real_lenght, oc.rollid_2, desperdicio2, lenght_2, anulada, numero, TwoMasters)) AS x(consumo, rollid, desperdicio, leng, anulada, numero, TwoMaster) WHERE x.consumo > 0 AND x.anulada = 0), consumo_total AS (SELECT rollid, SUM(ConsumoTotal) AS largo_consumido FROM detalle_consumo GROUP BY rollid), inventario_unificado AS (SELECT a.Roll_Id, a.Part_Number, b.Product_Name, a.Width, a.Lenght, ISNULL(a.msi,0) AS msi, a.fecha_pro, a.fecha_reg, a.splice, a.core, a.Ubicacion, 'Inic.' AS tipo_mov, a.documento, (SELECT STUFF((SELECT ',' + CONVERT(varchar(12), ocx.numero) FROM orden_corte ocx WHERE ocx.anulada = 0 AND (ocx.rollid_1 = a.Roll_Id OR ocx.rollid_2 = a.Roll_Id) ORDER BY ocx.numero FOR XML PATH('')), 1, 1, '')) AS documento_oc, ISNULL(ct.largo_consumido,0) AS largo_consumido, (a.Lenght - ISNULL(ct.largo_consumido,0)) AS largo_restante, CASE WHEN ISNULL(ct.largo_consumido,0) = 0 THEN 'Completo' WHEN (a.Lenght - ISNULL(ct.largo_consumido,0)) <= 0 THEN 'Agotado' ELSE 'Parcialmente Consumido' END AS estado FROM MasterInic a INNER JOIN producto b ON a.Part_Number = b.product_id LEFT JOIN consumo_total ct ON a.Roll_Id = ct.rollid WHERE b.MasterRolls = 1 UNION ALL SELECT a.rollid, a.product_id, b.Product_Name, a.Width, a.length, ISNULL(a.msi,0) AS msi, a.fecha_produccion, a.fecha_llegada, a.splice, a.core, a.Ubicacion, 'Por Compras' AS tipo_mov, a.documento, (SELECT STUFF((SELECT ',' + CONVERT(varchar(12), ocx.numero) FROM orden_corte ocx WHERE ocx.anulada = 0 AND (ocx.rollid_1 = a.rollid OR ocx.rollid_2 = a.rollid) ORDER BY ocx.numero FOR XML PATH('')), 1, 1, '')) AS documento_oc, ISNULL(ct.largo_consumido,0) AS largo_consumido, (a.length - ISNULL(ct.largo_consumido,0)) AS largo_restante, CASE WHEN ISNULL(ct.largo_consumido,0) = 0 THEN 'Completo' WHEN (a.length - ISNULL(ct.largo_consumido,0)) <= 0 THEN 'Agotado' ELSE 'Parcialmente Consumido' END AS estado FROM ItemsMateria a INNER JOIN producto b ON a.product_id = b.product_id LEFT JOIN consumo_total ct ON a.rollid = ct.rollid WHERE b.MasterRolls = 1), deduplicado AS (SELECT *, ROW_NUMBER() OVER (PARTITION BY Roll_Id ORDER BY CASE WHEN tipo_mov = 'Inic.' THEN 0 ELSE 1 END) AS rn FROM inventario_unificado) SELECT Roll_Id, Part_Number, Product_Name, Width, Lenght, msi, fecha_pro, fecha_reg, splice, core, Ubicacion, tipo_mov, documento, documento_oc, largo_consumido, largo_restante, estado FROM deduplicado WHERE rn = 1 ORDER BY Roll_Id";

                // Variante SOLO del modulo Inventario: un master con restante <= 100 pies
                // se marca como 'Desperdicio' (se pinta en rojo). El OC picker, reportes y
                // demas siguen usando la version base con 'Agotado'.
                internal static string SQL_QUERY_SELECT_LOAD_ROLL_ID_INVENTARIO = SQL_QUERY_SELECT_LOAD_ROLL_ID
                    .Replace("<= 0 THEN 'Agotado'", "<= 100 THEN 'Desperdicio'");

                internal static string SQL_QUERY_SELECT_LOAD_OPERATOR = "SELECT operador_id,nombre,status FROM operadores";
                internal static string SQL_QUERY_SELECT_LOAD_CUSTOMER = "SELECT customer_id,customer_name FROM customer";

                internal static string SQL_QUERY_ACTUALIZAR_INVENTARIO_INICIALES = "UPDATE MasterInic SET largo_consumido=largo_consumido+@consumo WHERE roll_id=@rollid";
                internal static string SQL_QUERY_ACTUALIZAR_INVENTARIO_MATERIA = "UPDATE ItemsMateria SET largo_consumido=largo_consumido+@consumo WHERE rollid=@rollid";

                internal static string UPDATE_QUERY_ACTUALIZAR_INVENTARIO_DETAILS_INICIALES = "INSERT INTO MasterDetailsInic (rollid,orden,consumo,fecha_reg,desperdicio) VALUES(@rollid,@orden,@consumo,@fecha,@desperdicio)";

                internal static string SQL_SELECT_QUERY_LOAD_DETAILS_MASTER_INICIALES_END_TRANSACTIONS = "SELECT rollid,orden,consumo,fecha_reg,a.desperdicio,case when a.desperdicio=1 then 0 else b.cant_rollos end as cant_rollos,b.customer_id,c.Customer_Name FROM MasterDetailsInic a left join orden_corte b on a.orden=b.numero left join Customer c on b.customer_id=c.customer_id WHERE rollid=@rollid";

                internal static string SQL_SELECT_QUERY_LOAD_DETAILS_MASTER_INICIALES_START_TRANSACTIONS = "select a.rollid_1 as rollid,a.numero as orden, a.util1_real_lenght as consumo,fecha as fecha_reg,a.desperdicio,case when desperdicio=1 then (a.lenght_1 - a.util1_real_lenght) else 0 end as monto_desperdicio,a.cant_rollos,a.customer_id,b.Customer_Name from orden_corte a left join Customer b on a.customer_id = b.Customer_Id where rollid_1=@rollid";

                internal static string SQL_QUERY_CONSUMO_OC_DETALLE_EXISTE = "SELECT 1 FROM MasterDetailsInic WHERE rollid=@rollid AND orden=@orden AND desperdicio=@desperdicio";


            }
            public class COMMERCIAL
            {
                // Sin filtro de anulado: los pedidos anulados siguen en el listado para que la
                // pantalla los muestre con la fila en rojo (fue un requisito explicito).
                internal static string SQL_SELECT_PEDIDOS = "SELECT numero,fecha,customer_id,customer_name,vendor_id,persona_contacto,tipo_venta,fecha_entrega,condiciones_pago,prioridad,direccion_entrega,direccion_facturacion,estado,notas,anulado,subtotal,porc_itbis,itbis,total$ FROM pedido ORDER BY numero DESC";
                internal static string SQL_SELECT_PEDIDO_DETALLE = "SELECT numero,product_id,product_name,cant,unidad,width,lenght,msi,precio,total_renglon,notas FROM pedido_detalle WHERE numero = @p1 ORDER BY id";
                internal static string SQL_INSERT_PEDIDO = "INSERT INTO pedido (numero,fecha,customer_id,customer_name,vendor_id,persona_contacto,tipo_venta,fecha_entrega,condiciones_pago,prioridad,direccion_entrega,direccion_facturacion,estado,notas,anulado,subtotal,porc_itbis,itbis,total$) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19)";
                internal static string SQL_INSERT_PEDIDO_DETALLE = "INSERT INTO pedido_detalle (numero,product_id,product_name,cant,unidad,width,lenght,msi,precio,total_renglon,notas) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11)";
                internal static string SQL_UPDATE_PEDIDO_ESTADO = "UPDATE pedido SET estado = @p2 WHERE numero = @p1";
                internal static string SQL_ANULAR_PEDIDO = "UPDATE pedido SET anulado = 1 WHERE numero = @p1";
                internal static string SQL_RESTAURAR_PEDIDO = "UPDATE pedido SET anulado = 0 WHERE numero = @p1";
                // El hint va despues del nombre de la tabla y antes del SET: al final de la
                // sentencia SQL Server responde "sintaxis incorrecta junto a la palabra clave
                // 'with'". Con UPDLOCK la fila queda bloqueada hasta el COMMIT, asi que el
                // segundo taker en espera y lee el valor ya incrementado en vez del mismo.
                internal static string SQL_QUERY_CONSUMO_PEDIDO_CONSECUTIVO = "UPDATE control WITH (UPDLOCK, HOLDLOCK) SET par1 = par1 + 1 OUTPUT DELETED.par1 WHERE filter='PED'";

                // Previsualizacion: lee el mismo valor que la reserva devolveria, sin tocar el
                // contador. control.par1 guarda el ultimo numero entregado, asi que la siguiente
                // reserva (OUTPUT DELETED.par1) devuelve justamente este valor.
                internal static string SQL_SELECT_PEDIDO_PROXIMO = "SELECT par1 FROM control WHERE filter='PED'";
                // Trae las dos direcciones del maestro. direccion_cliente se conserva como
                // respaldo para los clientes que todavia no tienen ninguna de las dos cargada.
                internal static string SQL_SELECT_LOAD_CUSTOMER_COMBO = "SELECT customer_id,customer_name,consecutivo,COALESCE(direccion_facturacion, Customer_Dir, customer_address, Customer_Dir, customer_zone, 'Sin especificar') AS direccion_cliente, COALESCE(direccion_facturacion, Customer_Dir, customer_address, Customer_Dir, customer_zone, 'Sin especificar') AS facturacion_cliente, COALESCE(direccion_entrega, direccion_facturacion, Customer_Dir, customer_address, customer_zone, 'Sin especificar') AS entrega_cliente FROM customer WHERE anulado = 0 ORDER BY customer_name";
                internal static string SQL_SELECT_LOAD_VENDOR_COMBO = "SELECT vendor_id,vendor_name,ROW_NUMBER() OVER (ORDER BY vendor_name) AS consecutivo FROM vendedor WHERE anulado = 0 ORDER BY vendor_name";

                // Consulta de un solo cliente, la que se dispara al elegirlo en el combo. Trae lo
                // mismo que la del combo pero de la base, para que las direcciones que se vean
                // sean las que el maestro tiene en ese momento y no las que se cargaron al abrir
                // el formulario.
                internal static string SQL_SELECT_CLIENTE_POR_ID = "SELECT consecutivo,COALESCE(direccion_facturacion, Customer_Dir, customer_address, customer_zone, 'Sin especificar') AS facturacion_cliente, COALESCE(direccion_entrega, direccion_facturacion, Customer_Dir, customer_address, customer_zone, 'Sin especificar') AS entrega_cliente FROM customer WHERE customer_id = @id AND anulado = 0";
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

            internal readonly static string SELECT_QUERY_PRODUCTS = "SELECT product_id,product_name,case when masterRolls=1 then 'Master' when rollo_cortado=1 then 'Rollo Cortado' when resmas=1 then 'Resma' when graphics=1 then 'Graphics' end as tipo,product_descrip,product_ref,codebar,category_id,masterRolls,rollo_cortado,resmas,graphics,anulado,precio,code_rc,ratio FROM producto";

            // Productos - queries parametrizadas (usar siempre con SqlParameter, nunca concatenar)
            internal const string INSERT_PRODUCT = "INSERT INTO producto (Product_ID,Product_Name,Product_Descrip,Product_Ref,Codebar,MasterRolls,rollo_cortado,Resmas,Graphics,anulado,precio,ratio) VALUES (@product_id,@product_name,@product_description,@reference,@codebar,@master,@rollo,@resma,@graphics,@anulado,@precio,@ratio)";
            internal const string UPDATE_PRODUCT = "UPDATE dbo.producto SET Product_Name=@name,Product_Descrip=@descrip,Product_Ref=@reference,codebar=@barra,precio=@precio,ratio=@ratio,masterRolls=@master,graphics=@graphics,resmas=@hoja,rollo_cortado=@rollo WHERE product_id=@id";
            internal const string SELECT_PRODUCT_EXISTS = "SELECT COUNT(*) FROM producto WHERE product_id=@id";
            internal const string SELECT_PRODUCT_ANULADO = "SELECT anulado FROM producto WHERE product_id=@id";
            internal const string UPDATE_PRODUCT_ANULAR = "UPDATE producto SET anulado=1 WHERE Product_ID=@id AND anulado=0";
            internal const string SELECT_PRODUCT_BY_ID = "SELECT product_id,product_name,product_descrip,product_ref,codebar,masterRolls,rollo_cortado,resmas,graphics,anulado,precio,ratio FROM producto WHERE product_id=@id";

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
