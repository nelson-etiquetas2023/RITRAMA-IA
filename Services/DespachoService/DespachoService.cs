using System.Data;
using System.Text.RegularExpressions;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.DespachoService.DespachoService
{
    public class DespachoService : IDespachoService
    {
        public DataSet Ds = new();
        public string StringConnex { get; set; } = null!;
        public string ErrorMsg { get; set; } = null!;
        public string Ambiente { get; } = "";
        public IConfiguration Config { get; }

        private readonly List<Despacho> lista = [];

        public DataTable DtMasterDespachos = new();
        public SqlDataAdapter DaMasterDespachos = new();
        public DataTable DtClientes = new();
        public SqlDataAdapter DaClientes = new();
        public DataTable DtVendors = new();
        public SqlDataAdapter DaVendors = new();
        public DataTable DtDetalleRC = new();
        public SqlDataAdapter DaDetalleRC = new();
        public DataTable DtItems = new();
        public SqlDataAdapter DaItems = new();
        public DataTable DtPalet = new();
        public SqlDataAdapter DaPalet = new();
        public DataTable DtTransport = new();
        public SqlDataAdapter DaTransport = new();
        public DataTable DtChofer = new();
        public SqlDataAdapter DaChofer = new();
        public DataTable DtCamion = new();
        public SqlDataAdapter DaCamion = new();

        public DespachoService(IConfiguration Config)
        {
            this.Config = Config ?? throw new ArgumentNullException(nameof(Config));
            if (Config != null)
            {
                Ambiente = Config["Ambiente"] ?? R.ENVIRONMET.DESARROLLO;
                string ambiente = Ambiente;
                StringConnex = Config.GetSection(R.ENVIRONMET.NAME_KEY_CONNECTION)[ambiente]!;
            }
        }

        private static string ObtenerServidor(string cs)
        {
            string Valor(string clave)
            {
                Match m = System.Text.RegularExpressions.Regex.Match(cs, $@"(?i){clave}\s*=\s*([^;]+)");
                return m.Success ? m.Groups[1].Value.Trim() : "";
            }
            return $"{Valor("Data Source")}/{Valor("Initial Catalog")}";
        }

        public void UpDateInventoryRC(List<RolloCortado> items, string orden, DateTime fecha)
        {
            try
            {
                foreach (RolloCortado item in items)
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "update rolls_details set disponible=0,despacho=@p2,fecha_despacho=@p3 where unique_code=@p1"
                    };
                    conn.Open();
                    SqlParameter p1 = new("@p1", item.UniqueCode);
                    SqlParameter p2 = new("@p2", orden);
                    SqlParameter p3 = new("@p3", fecha);



                    comando.Parameters.Add(p1);
                    comando.Parameters.Add(p2);
                    comando.Parameters.Add(p3);


                    comando.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    comando.Dispose();
                }
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("Error al actualizar los Invenatrios...: " + ex.Message);
                ErrorMsg = ex.Message;

            }
        }

        public void AddPaletDetailsDespacho(List<Paleta> paleta)
        {
            try
            {
                foreach (Paleta item in paleta)
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand Comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO paleta (numero,number_palet,medida,contenido,kilo_neto,kilo_bruto) VALUES (@p1,@p2,@p3,@p4,@p5,@p6)"
                    };
                    conn.Open();
                    SqlParameter p1 = new("@p1", item.Numero);
                    SqlParameter p2 = new("@p2", item.Number_Palet);
                    SqlParameter p3 = new("@p3", item.Medida);
                    SqlParameter p4 = new("@p4", item.Contenido);
                    SqlParameter p5 = new("@p5", item.Kilo_Neto);
                    SqlParameter p6 = new("@p6", item.Kilo_Bruto);
                    Comando.Parameters.Add(p1);
                    Comando.Parameters.Add(p2);
                    Comando.Parameters.Add(p3);
                    Comando.Parameters.Add(p4);
                    Comando.Parameters.Add(p5);
                    Comando.Parameters.Add(p6);
                    Comando.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    Comando.Dispose();
                }
            }
            catch (SqlException ex)
            {
                ServiceErrors.Report("Error al grabar los items del despacho: " + ex.Message);
                ErrorMsg = ex.Message;
            }
        }
        public void AddItemsDespacho(List<ItemsDespacho> items)
        {
            try
            {
                foreach (ItemsDespacho item in items)
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand Comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO item_despacho (numero,product_id,cant,unid_id,width,lenght,code_person,msi,total_pie_lin,ratio,kilo_rollo,kilo_total,precio,total_renglon,m2) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)"
                    };
                    conn.Open();
                    SqlParameter p1 = new("@p1", item.Numero);
                    SqlParameter p2 = new("@p2", item.Product_id);
                    SqlParameter p3 = new("@p3", item.Cantidad);
                    SqlParameter p4 = new("@p4", item.Unid_id);
                    SqlParameter p5 = new("@p5", item.Width);
                    SqlParameter p6 = new("@p6", item.Lenght);
                    SqlParameter p7 = new("@p7", item.Code_Person);
                    SqlParameter p8 = new("@p8", item.Msi);
                    SqlParameter p9 = new("@p9", item.Total_PieLineal);
                    SqlParameter p10 = new("@p10", item.Ratio);
                    SqlParameter p11 = new("@p11", item.Kilo_Rollo);
                    SqlParameter p12 = new("@p12", item.Kilo_Total);
                    SqlParameter p13 = new("@p13", item.Precio);
                    SqlParameter p14 = new("@p14", item.Total_Renglon);
                    SqlParameter p15 = new("@p15", item.M2);
                    Comando.Parameters.Add(p1);
                    Comando.Parameters.Add(p2);
                    Comando.Parameters.Add(p3);
                    Comando.Parameters.Add(p4);
                    Comando.Parameters.Add(p5);
                    Comando.Parameters.Add(p6);
                    Comando.Parameters.Add(p7);
                    Comando.Parameters.Add(p8);
                    Comando.Parameters.Add(p9);
                    Comando.Parameters.Add(p10);
                    Comando.Parameters.Add(p11);
                    Comando.Parameters.Add(p12);
                    Comando.Parameters.Add(p13);
                    Comando.Parameters.Add(p14);
                    Comando.Parameters.Add(p15);
                    Comando.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    Comando.Dispose();
                }
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al grabar los items del despacho: " + ex.Message);
                ErrorMsg = ex.Message;
            }
        }
        public void AddPickingListDespacho(List<RolloCortado> rollos)
        {
            try
            {
                foreach (RolloCortado item in rollos)
                {
                    using SqlConnection conn = new(StringConnex);
                    SqlCommand Comando = new()
                    {
                        Connection = conn,
                        CommandType = CommandType.Text,
                        CommandText = "INSERT INTO rcdespacho (conduce,unique_code,product_id,roll_number,width,lenght,msi,splice,roll_id,cant_despacho,tipo,no_paleta) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12)"
                    };
                    conn.Open();
                    SqlParameter p1 = new("@p1", item.Numero);
                    SqlParameter p2 = new("@p2", item.UniqueCode);
                    SqlParameter p3 = new("@p3", item.Product_Id);
                    SqlParameter p4 = new("@p4", item.RollNumber);
                    SqlParameter p5 = new("@p5", item.Width);
                    SqlParameter p6 = new("@p6", item.Length);
                    SqlParameter p7 = new("@p7", item.Msi);
                    SqlParameter p8 = new("@p8", item.Splice);
                    SqlParameter p9 = new("@p9", item.Roll_Id);
                    SqlParameter p10 = new("@p10", item.Cantidad_despacho);
                    SqlParameter p11 = new("@p11", item.Tipo);
                    SqlParameter p12 = new("@p12", item.Paleta);
                    Comando.Parameters.Add(p1);
                    Comando.Parameters.Add(p2);
                    Comando.Parameters.Add(p3);
                    Comando.Parameters.Add(p4);
                    Comando.Parameters.Add(p5);
                    Comando.Parameters.Add(p6);
                    Comando.Parameters.Add(p7);
                    Comando.Parameters.Add(p8);
                    Comando.Parameters.Add(p9);
                    Comando.Parameters.Add(p10);
                    Comando.Parameters.Add(p11);
                    Comando.Parameters.Add(p12);
                    Comando.ExecuteNonQuery();
                    conn.Close();
                    conn.Dispose();
                    Comando.Dispose();
                }
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceLogger.LogError("AddPickingListDespacho", ex);
                ServiceErrors.Report("error al grabar los rollos cortados...");
            }
        }
        public void AddDocumentDespacho(Despacho document)
        {
            //Grabar el encabezado del despacho.
            try
            {
                using SqlConnection conn = new(StringConnex);
                SqlCommand Comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "INSERT INTO despacho (numero,fecha,person_contact,vendor_id,packing,orden_trabajo,orden_compra,subtotal,itbis,total$rd,transporte,chofer,camion,customer_id,tipo_venta,transport_id,chofer_id,placas_id,total_cantidad,total_msi,total_pie,total_kilos,porc_itbis,total_kilos_netos_palet,total_kilos_brutos_palet) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19,@p20,@p21,@p22,@p23,@p24,@p25)"
                };
                conn.Open();
                SqlParameter p1 = new("@p1", document.Numero);
                SqlParameter p2 = new("@p2", document.Fecha_despacho);
                SqlParameter p3 = new("@p3", document.Persona_Contact);
                SqlParameter p4 = new("@p4", document.Vendor_Id);
                SqlParameter p5 = new("@p5", document.Tipo_Embalaje);
                SqlParameter p6 = new("@p6", document.Orden_Trabajo);
                SqlParameter p7 = new("@p7", document.Orden_Compra);
                SqlParameter p8 = new("@p8", document.SubTotal);
                SqlParameter p9 = new("@p9", document.Monto_Itbis);
                SqlParameter p10 = new("@p10", document.Total_Despacho);
                SqlParameter p11 = new("@p11", document.Transport_Name);
                SqlParameter p12 = new("@p12", document.Chofer_Name);
                SqlParameter p13 = new("@p13", document.Camion_Name);
                SqlParameter p14 = new("@p14", document.Customer_Id);
                SqlParameter p15 = new("@p15", document.Tipo_venta);
                SqlParameter p16 = new("@p16", document.Transport_Id);
                SqlParameter p17 = new("@p17", document.Chofer_Id);
                SqlParameter p18 = new("@p18", document.Camion_Id);
                SqlParameter p19 = new("@p19", document.Total_Cantidad);
                SqlParameter p20 = new("@p20", document.Total_Msi);
                SqlParameter p21 = new("@p21", document.Total_Pie);
                SqlParameter p22 = new("@p22", document.Total_Kilos);
                SqlParameter p23 = new("@p23", document.Porc_Itbis);
                SqlParameter p24 = new("@p24", document.Total_kilos_netos_palet);
                SqlParameter p25 = new("@p25", document.Total_kilos_brutos_palet);
                Comando.Parameters.Add(p1);
                Comando.Parameters.Add(p2);
                Comando.Parameters.Add(p3);
                Comando.Parameters.Add(p4);
                Comando.Parameters.Add(p5);
                Comando.Parameters.Add(p6);
                Comando.Parameters.Add(p7);
                Comando.Parameters.Add(p8);
                Comando.Parameters.Add(p9);
                Comando.Parameters.Add(p10);
                Comando.Parameters.Add(p11);
                Comando.Parameters.Add(p12);
                Comando.Parameters.Add(p13);
                Comando.Parameters.Add(p14);
                Comando.Parameters.Add(p15);
                Comando.Parameters.Add(p16);
                Comando.Parameters.Add(p17);
                Comando.Parameters.Add(p18);
                Comando.Parameters.Add(p19);
                Comando.Parameters.Add(p20);
                Comando.Parameters.Add(p21);
                Comando.Parameters.Add(p22);
                Comando.Parameters.Add(p23);
                Comando.Parameters.Add(p24);
                Comando.Parameters.Add(p25);
                Comando.ExecuteNonQuery();
                conn.Close();
                conn.Dispose();
                Comando.Dispose();
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al grabar el encabezado del despacho...");
                ErrorMsg = ex.Message;
            }
        }
        public bool SaveDespachoCompleto(Despacho document, DateTime fecha)
        {
            using SqlConnection conn = new SqlConnection(StringConnex);
            conn.Open();
            using SqlTransaction tran = conn.BeginTransaction();
            try
            {
                // Validacion previa de integridad numerica: detecta y reporta el campo
                // cuyo valor excede el rango de numeric(18,2), evitando el error generico
                // "desbordamiento aritmetico al convertir numeric al tipo de datos numeric".
                string? errorNumerico = ValidarRangoNumerico(document);
                if (errorNumerico != null)
                {
                    ErrorMsg = errorNumerico;
                    try { tran.Rollback(); } catch { }
                    ServiceErrors.Report("Error al grabar el despacho: " + errorNumerico);
                    return false;
                }

                // 1. Encabezado del despacho
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO despacho (numero,fecha,person_contact,vendor_id,packing,orden_trabajo,orden_compra,subtotal,itbis,total$rd,transporte,chofer,camion,customer_id,tipo_venta,transport_id,chofer_id,placas_id,total_cantidad,total_msi,total_pie,total_kilos,porc_itbis,total_kilos_netos_palet,total_kilos_brutos_palet) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15,@p16,@p17,@p18,@p19,@p20,@p21,@p22,@p23,@p24,@p25)",
                    conn, tran))
                {
                    cmd.Parameters.Add(new SqlParameter("@p1", document.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", document.Fecha_despacho));
                    cmd.Parameters.Add(new SqlParameter("@p3", document.Persona_Contact));
                    cmd.Parameters.Add(new SqlParameter("@p4", document.Vendor_Id));
                    cmd.Parameters.Add(new SqlParameter("@p5", document.Tipo_Embalaje));
                    cmd.Parameters.Add(new SqlParameter("@p6", document.Orden_Trabajo));
                    cmd.Parameters.Add(new SqlParameter("@p7", document.Orden_Compra));
                    cmd.Parameters.Add(new SqlParameter("@p8", document.SubTotal));
                    cmd.Parameters.Add(new SqlParameter("@p9", document.Monto_Itbis));
                    cmd.Parameters.Add(new SqlParameter("@p10", document.Total_Despacho));
                    cmd.Parameters.Add(new SqlParameter("@p11", document.Transport_Name));
                    cmd.Parameters.Add(new SqlParameter("@p12", document.Chofer_Name));
                    cmd.Parameters.Add(new SqlParameter("@p13", document.Camion_Name));
                    cmd.Parameters.Add(new SqlParameter("@p14", document.Customer_Id));
                    cmd.Parameters.Add(new SqlParameter("@p15", document.Tipo_venta));
                    cmd.Parameters.Add(new SqlParameter("@p16", document.Transport_Id));
                    cmd.Parameters.Add(new SqlParameter("@p17", document.Chofer_Id));
                    cmd.Parameters.Add(new SqlParameter("@p18", document.Camion_Id));
                    cmd.Parameters.Add(new SqlParameter("@p19", document.Total_Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@p20", document.Total_Msi));
                    cmd.Parameters.Add(new SqlParameter("@p21", document.Total_Pie));
                    cmd.Parameters.Add(new SqlParameter("@p22", document.Total_Kilos));
                    cmd.Parameters.Add(new SqlParameter("@p23", document.Porc_Itbis));
                    cmd.Parameters.Add(new SqlParameter("@p24", document.Total_kilos_netos_palet));
                    cmd.Parameters.Add(new SqlParameter("@p25", document.Total_kilos_brutos_palet));
                    cmd.ExecuteNonQuery();
                }

                // 2. Picking list (rollos cortados)
                foreach (RolloCortado item in document.Detalle_RC)
                {
                    using SqlCommand cmd = new SqlCommand(
                        "INSERT INTO rcdespacho (conduce,unique_code,product_id,roll_number,width,lenght,msi,splice,roll_id,cant_despacho,tipo,no_paleta) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12)",
                        conn, tran);
                    cmd.Parameters.Add(new SqlParameter("@p1", item.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", item.UniqueCode));
                    cmd.Parameters.Add(new SqlParameter("@p3", item.Product_Id));
                    cmd.Parameters.Add(new SqlParameter("@p4", item.RollNumber));
                    cmd.Parameters.Add(new SqlParameter("@p5", (decimal)item.Width));
                    cmd.Parameters.Add(new SqlParameter("@p6", (decimal)item.Length));
                    cmd.Parameters.Add(new SqlParameter("@p7", (decimal)item.Msi));
                    cmd.Parameters.Add(new SqlParameter("@p8", item.Splice));
                    cmd.Parameters.Add(new SqlParameter("@p9", item.Roll_Id));
                    cmd.Parameters.Add(new SqlParameter("@p10", item.Cantidad_despacho));
                    cmd.Parameters.Add(new SqlParameter("@p11", item.Tipo));
                    cmd.Parameters.Add(new SqlParameter("@p12", item.Paleta));
                    cmd.ExecuteNonQuery();
                }

                // 3. Items de despacho
                foreach (ItemsDespacho item in document.Items_Despacho)
                {
                    using SqlCommand cmd = new SqlCommand(
                        "INSERT INTO item_despacho (numero,product_id,cant,unid_id,width,lenght,code_person,msi,total_pie_lin,ratio,kilo_rollo,kilo_total,precio,total_renglon,m2) VALUES (@p1,@p2,@p3,@p4,@p5,@p6,@p7,@p8,@p9,@p10,@p11,@p12,@p13,@p14,@p15)",
                        conn, tran);
                    cmd.Parameters.Add(new SqlParameter("@p1", item.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", item.Product_id));
                    cmd.Parameters.Add(new SqlParameter("@p3", item.Cantidad));
                    cmd.Parameters.Add(new SqlParameter("@p4", item.Unid_id));
                    cmd.Parameters.Add(new SqlParameter("@p5", item.Width));
                    cmd.Parameters.Add(new SqlParameter("@p6", item.Lenght));
                    cmd.Parameters.Add(new SqlParameter("@p7", item.Code_Person));
                    cmd.Parameters.Add(new SqlParameter("@p8", item.Msi));
                    cmd.Parameters.Add(new SqlParameter("@p9", item.Total_PieLineal));
                    cmd.Parameters.Add(new SqlParameter("@p10", item.Ratio));
                    cmd.Parameters.Add(new SqlParameter("@p11", item.Kilo_Rollo));
                    cmd.Parameters.Add(new SqlParameter("@p12", item.Kilo_Total));
                    cmd.Parameters.Add(new SqlParameter("@p13", item.Precio));
                    cmd.Parameters.Add(new SqlParameter("@p14", item.Total_Renglon));
                    cmd.Parameters.Add(new SqlParameter("@p15", item.M2));
                    cmd.ExecuteNonQuery();
                }

                // 4. Detalle de paleta
                foreach (Paleta item in document.Detalle_Paleta)
                {
                    using SqlCommand cmd = new SqlCommand(
                        "INSERT INTO paleta (numero,number_palet,medida,contenido,kilo_neto,kilo_bruto) VALUES (@p1,@p2,@p3,@p4,@p5,@p6)",
                        conn, tran);
                    cmd.Parameters.Add(new SqlParameter("@p1", item.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", item.Number_Palet));
                    cmd.Parameters.Add(new SqlParameter("@p3", item.Medida));
                    cmd.Parameters.Add(new SqlParameter("@p4", item.Contenido));
                    cmd.Parameters.Add(new SqlParameter("@p5", item.Kilo_Neto));
                    cmd.Parameters.Add(new SqlParameter("@p6", item.Kilo_Bruto));
                    cmd.ExecuteNonQuery();
                }

                // 5. Actualizar inventario de rollos cortados
                foreach (RolloCortado item in document.Detalle_RC)
                {
                    using SqlCommand cmd = new SqlCommand(
                        "UPDATE rolls_details SET disponible=0,despacho=@p2,fecha_despacho=@p3 WHERE unique_code=@p1",
                        conn, tran);
                    cmd.Parameters.Add(new SqlParameter("@p1", item.UniqueCode));
                    cmd.Parameters.Add(new SqlParameter("@p2", document.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p3", fecha));
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                try { tran.Rollback(); } catch { }
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al grabar el despacho: " + ex.Message);
                return false;
            }
        }

        /// <summary>
        /// Detecta que campos exceden el rango de numeric(18,2) (magnitud &gt; 9999999999999999.99)
        /// antes de enviarlos a SQL Server, para reemplazar el error generico de
        /// "desbordamiento aritmetico" con un mensaje que identifica el campo y su valor.
        /// </summary>
        private static string? ValidarRangoNumerico(Despacho document)
        {
            List<(string Campo, decimal Valor)> valores =
            [
                ("subtotal", document.SubTotal),
                ("itbis", document.Monto_Itbis),
                ("total$rd", document.Total_Despacho),
                ("porc_itbis", document.Porc_Itbis),
                ("total_cantidad", document.Total_Cantidad),
                ("total_msi", document.Total_Msi),
                ("total_pie", document.Total_Pie),
                ("total_kilos", document.Total_Kilos),
                ("total_kilos_netos_palet", document.Total_kilos_netos_palet),
                ("total_kilos_brutos_palet", document.Total_kilos_brutos_palet),
            ];

            if (document.Items_Despacho != null)
            {
                for (int i = 0; i < document.Items_Despacho.Count; i++)
                {
                    ItemsDespacho it = document.Items_Despacho[i];
                    valores.AddRange(new (string, decimal)[]
                    {
                        ($"item[{i}].cantidad", it.Cantidad),
                        ($"item[{i}].width", it.Width),
                        ($"item[{i}].lenght", it.Lenght),
                        ($"item[{i}].msi", it.Msi),
                        ($"item[{i}].total_pie_lin", it.Total_PieLineal),
                        ($"item[{i}].ratio", it.Ratio),
                        ($"item[{i}].kilo_rollo", it.Kilo_Rollo),
                        ($"item[{i}].kilo_total", it.Kilo_Total),
                        ($"item[{i}].precio", it.Precio),
                        ($"item[{i}].m2", it.M2),
                        ($"item[{i}].total_renglon", it.Total_Renglon),
                    });
                }
            }

            if (document.Detalle_RC != null)
            {
                for (int i = 0; i < document.Detalle_RC.Count; i++)
                {
                    RolloCortado rc = document.Detalle_RC[i];
                    valores.AddRange(new (string, decimal)[]
                    {
                        ($"rollo[{i}].width", Convert.ToDecimal(rc.Width)),
                        ($"rollo[{i}].lenght", Convert.ToDecimal(rc.Length)),
                        ($"rollo[{i}].msi", Convert.ToDecimal(rc.Msi)),
                    });
                }
            }

            if (document.Detalle_Paleta != null)
            {
                for (int i = 0; i < document.Detalle_Paleta.Count; i++)
                {
                    Paleta p = document.Detalle_Paleta[i];
                    valores.AddRange(new (string, decimal)[]
                    {
                        ($"paleta[{i}].kilo_neto", p.Kilo_Neto),
                        ($"paleta[{i}].kilo_bruto", p.Kilo_Bruto),
                    });
                }
            }

            return RangoNumericoValidator.PrimeraFueraDeRango(valores);
        }

        public string GetNumberConsec()
        {
            string consec = string.Empty;
            try
            {
                using SqlConnection conn = new(StringConnex);
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT ISNULL(MAX(TRY_CAST(numero AS INT)) + 1, 1) AS numero FROM despacho"
                };
                conn.Open();
                object result = comando.ExecuteScalar();
                consec = Convert.ToString(result)!;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
            }
            return consec;
        }

        public async Task<string> GetNumberConsecAsync(CancellationToken ct = default)
        {
            try
            {
                using SqlConnection conn = new(StringConnex);
                using SqlCommand comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT ISNULL(MAX(TRY_CAST(numero AS INT)) + 1, 1) AS numero FROM despacho"
                };
                await conn.OpenAsync(ct).ConfigureAwait(false);
                object result = await comando.ExecuteScalarAsync(ct).ConfigureAwait(false);
                return Convert.ToString(result)!;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                return string.Empty;
            }
        }
        public async Task<DataSet> LoadDataDespachos(CancellationToken cancellationToken = default)
        {
            try
            {
                cancellationToken.ThrowIfCancellationRequested();

                //limpiar el dataset cualdo se carga el form varias veces.
                if (Ds.Tables.Count > 0)
                {
                    DataTable tabla = Ds.Tables["DtMasterDespachos"]!;

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

                using SqlConnection conn = new(StringConnex);
                //1.- Carga del Encabezado de Despacho
                using SqlCommand ComandoMaster = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT numero,fecha,customer_id,person_contact,transporte,chofer,camion,vendor_id,packing,orden_trabajo,orden_compra,tipo_venta,subtotal,porc_itbis,itbis,total$rd,transport_id,chofer_id,placas_id,total_cantidad,total_msi,total_pie,total_kilos,total_kilos_netos_palet,total_kilos_brutos_palet FROM despacho"
                };
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                DaMasterDespachos.SelectCommand = ComandoMaster;
                DaMasterDespachos.Fill(Ds, "DtMasterDespachos");

                //2.- Carga de Clientes.
                SqlCommand ComandoClientes = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT customer_id,customer_name FROM customer"
                };

                DaClientes.SelectCommand = ComandoClientes;
                DaClientes.Fill(Ds, "DtClientes");
                //3.- Carga de Vendedores
                SqlCommand ComandoVendor = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT vendor_id,vendor_name FROM vendedor"
                };

                DaVendors.SelectCommand = ComandoVendor;
                DaVendors.Fill(Ds, "DtVendors");
                //4.- Carga de Detalle de Rollo Cortado
                SqlCommand ComandoRC = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT conduce,unique_code,a.product_id,b.product_Name,roll_number,a.width,a.lenght,a.msi,splice,cant_despacho,tipo,no_paleta,roll_id FROM rcdespacho a LEFT JOIN producto b on a.product_id=b.product_ID"
                };

                DaDetalleRC.SelectCommand = ComandoRC;
                DaDetalleRC.Fill(Ds, "DtDetalleRC");
                //5.- Carga de Reglones del Despacho.
                SqlCommand ComandoItems = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT numero,a.product_id,cant,b.product_name,unid_id,a.unidad, a.width,a.lenght,a.msi,total_pie_lin,a.ratio,kilo_rollo,kilo_total,a.precio,total_renglon,code_person,m2 FROM item_despacho a LEFT JOIN producto b ON a.product_id=b.product_id "
                };

                DaItems.SelectCommand = ComandoItems;
                DaItems.Fill(Ds, "DtItems");
                //6.- Carga de Detalle de Paleta.
                SqlCommand ComandoPalet = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT numero,number_palet,medida,contenido,kilo_neto,kilo_bruto FROM paleta"
                };

                DaPalet.SelectCommand = ComandoPalet;
                DaPalet.Fill(Ds, "DtPalet");
                //7.- Carga de la tabla de transportista
                SqlCommand ComandoTransport = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT transport_id,transport_name FROM transporte"
                };

                DaTransport.SelectCommand = ComandoTransport;
                DaTransport.Fill(Ds, "DtTransport");
                //7.- Carga de la tabla de chofer.
                SqlCommand ComandoChofer = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT chofer_id,chofer_name FROM chofer"
                };

                DaChofer.SelectCommand = ComandoChofer;
                DaChofer.Fill(Ds, "DtChofer");
                //8.- Carga de la tabla de Camion.
                SqlCommand ComandoCamion = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT placas_id,camion_name FROM camion"
                };

                DaCamion.SelectCommand = ComandoCamion;
                DaCamion.Fill(Ds, "DtCamion");
                Ds.EnforceConstraints = true;



                RelationDataset();
            }
            catch (SqlException ex)
            {
                ErrorMsg = $"Entorno={Ambiente} -> servidor {ObtenerServidor(StringConnex)}: {ex.Message}";
                throw new InvalidOperationException(ErrorMsg, ex);
            }
            return Ds;
        }
        public bool RelationDataset()
        {
            try
            {
                //Relacion entre master y clientes
                DataColumn ParentCol0 = Ds.Tables["DtClientes"]!.Columns["customer_id"]!;
                DataColumn ChildCol0 = Ds.Tables["DtMasterDespachos"]!.Columns["customer_id"]!;
                DataRelation Despacho_Clientes = new("FK_DESPACHOS_CLIENTES", ParentCol0, ChildCol0, false);
                Ds.Relations.Add(Despacho_Clientes);
                Ds.Tables["DtMasterDespachos"]!.Columns.Add("customer_name", Type.GetType("System.String")!, "parent(FK_DESPACHOS_CLIENTES).customer_name");
                //Relacion entre master y vendedores
                DataColumn ParentCol1 = Ds.Tables["DtVendors"]!.Columns["vendor_id"]!;
                DataColumn ChildCol1 = Ds.Tables["DtMasterDespachos"]!.Columns["vendor_id"]!;
                DataRelation Despacho_Vendors = new("FK_DESPACHOS_VENDOR", ParentCol1, ChildCol1, false);
                Ds.Relations.Add(Despacho_Vendors);
                Ds.Tables["DtMasterDespachos"]!.Columns.Add("vendor_name", Type.GetType("System.String")!, "parent(FK_DESPACHOS_VENDOR).vendor_name");
                //Relacion entre master y Detalle RC
                DataColumn ParentCol2 = Ds.Tables["DtMasterDespachos"]!.Columns["numero"]!;
                DataColumn ChildCol2 = Ds.Tables["DtDetalleRC"]!.Columns["conduce"]!;
                DataRelation Despacho_DetalleRC = new("FK_DESPACHOS_DETALLERC", ParentCol2, ChildCol2, false);
                Ds.Relations.Add(Despacho_DetalleRC);
                //Relacion entre master y Renglones de Despacho.
                DataColumn ParentCol3 = Ds.Tables["DtMasterDespachos"]!.Columns["numero"]!;
                DataColumn ChildCol3 = Ds.Tables["DtItems"]!.Columns["numero"]!;
                DataRelation Despacho_Items = new("FK_DESPACHOS_ITEMS",
                    ParentCol3, ChildCol3, false);
                Ds.Relations.Add(Despacho_Items);
                //Relacion entre master y detalle de palet.
                DataColumn ParentCol4 = Ds.Tables["DtMasterDespachos"]!.Columns["numero"]!;
                DataColumn ChildCol4 = Ds.Tables["DtPalet"]!.Columns["numero"]!;
                DataRelation Despacho_Palet = new("FK_DESPACHOS_PALET",
                   ParentCol4, ChildCol4, false);
                Ds.Relations.Add(Despacho_Palet);
                // Relacion entre Despachos y transportista
                DataColumn ParentCol5 = Ds.Tables["DtTransport"]!.Columns["transport_id"]!;
                DataColumn ChildCol5 = Ds.Tables["DtMasterDespachos"]!.Columns["transport_id"]!;
                DataRelation Despacho_Transport = new("FK_DESPACHOS_TRANSPORT", ParentCol5, ChildCol5, false);
                Ds.Relations.Add(Despacho_Transport);
                Ds.Tables["DtMasterDespachos"]!.Columns.Add("transport_name", Type.GetType("System.String")!, "parent(FK_DESPACHOS_TRANSPORT).transport_name");


                return true;
            }
            catch (Exception ex)
            {
                ServiceLogger.LogError("SetRelationsTables", ex);
                ServiceErrors.Report("Error al tratar de establecer las relaciones entre tablas. Error Code:" + ex);
                return false;
            }


        }
        public decimal GetRatioProductById(string product_id)
        {
            decimal ratio = 0;
            try
            {
                using SqlConnection conn = new(StringConnex);
                SqlCommand Comando = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = "SELECT ratio FROM producto WHERE product_id = @p1"
                };
                SqlParameter p1 = new("@p1", product_id);
                Comando.Parameters.Add(p1);
                conn.Open();
                ratio = Convert.ToDecimal(Comando.ExecuteScalar())!;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
            }
            return ratio;
        }


    }
}
