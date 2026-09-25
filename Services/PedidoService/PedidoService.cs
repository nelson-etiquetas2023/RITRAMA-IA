using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.PedidoService
{
    public class PedidoService : IPedidoService
    {
        private readonly string _conn;
        public string? ErrorMsg { get; set; }

        public PedidoService(IConfiguration config)
        {
            _conn = ConexionResolver.Resolver(config);
        }

        public async Task<DataTable> LoadDataPedidos(CancellationToken ct = default)
        {
            DataTable dt = new();
            try
            {
                ct.ThrowIfCancellationRequested();
                using SqlConnection conn = new(_conn);
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_SELECT_PEDIDOS
                };
                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al cargar los pedidos: " + ex.Message);
            }
            return dt;
        }

        public async Task<DataTable> LoadDataCustomers(CancellationToken ct = default)
        {
            DataTable dt = new();
            try
            {
                ct.ThrowIfCancellationRequested();
                using SqlConnection conn = new(_conn);
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_SELECT_LOAD_CUSTOMER_COMBO
                };
                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al cargar los clientes: " + ex.Message);
            }
            return dt;
        }

        public async Task<DataTable> LoadDataVendors(CancellationToken ct = default)
        {
            DataTable dt = new();
            try
            {
                ct.ThrowIfCancellationRequested();
                using SqlConnection conn = new(_conn);
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_SELECT_LOAD_VENDOR_COMBO
                };
                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al cargar los vendedores: " + ex.Message);
            }
            return dt;
        }

        public async Task<DataTable> LoadDataPedidoDetalle(string numero, CancellationToken ct = default)
        {
            DataTable dt = new();
            try
            {
                ct.ThrowIfCancellationRequested();
                using SqlConnection conn = new(_conn);
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_SELECT_PEDIDO_DETALLE
                };
                cmd.Parameters.Add(new SqlParameter("@p1", numero));
                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al cargar el detalle del pedido: " + ex.Message);
            }
            return dt;
        }

        public Task<string> GetNewNumeroPedido(CancellationToken ct = default)
        {
            try
            {
                using SqlConnection conn = new(_conn);
                conn.Open();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_QUERY_CONSUMO_PEDIDO_CONSECUTIVO
                };
                int consecutivo = Convert.ToInt32(cmd.ExecuteScalar()!);
                return Task.FromResult(PedidoNumero.Formatear(consecutivo));
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al obtener el consecutivo del pedido: " + ex.Message);
                throw;
            }
        }

        public bool SavePedidoCompleto(Pedido pedido)
        {
            // Validar antes de abrir la conexion: un pedido invalido se rechaza sin tocar la
            // base de datos y con un mensaje de negocio en vez de una excepcion de SQL.
            if (!PedidoValidador.EsValido(pedido, out string error))
            {
                // No se reporta por ServiceErrors: es un resultado esperado y la pantalla
                // ya muestra el motivo al usuario.
                ErrorMsg = error;
                return false;
            }

            using SqlConnection conn = new SqlConnection(_conn);
            SqlTransaction? tran = null;
            try
            {
                // Abrir primero y recien despues iniciar la transaccion: al reves, SqlConnection
                // lanza InvalidOperationException porque no hay conexion abierta.
                conn.Open();
                tran = conn.BeginTransaction();

                using (SqlCommand cmd = new SqlCommand(
                    R.QUERY.COMMERCIAL.SQL_INSERT_PEDIDO,
                        conn, tran))
                {
                    cmd.Parameters.Add(new SqlParameter("@p1", pedido.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", pedido.Fecha));
                    cmd.Parameters.Add(new SqlParameter("@p3", pedido.Customer_Id) { SqlDbType = SqlDbType.UniqueIdentifier });
                    cmd.Parameters.Add(new SqlParameter("@p4", (object?)pedido.Customer_Name ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p5", pedido.Vendor_Id.HasValue ? pedido.Vendor_Id.Value : (object)DBNull.Value) { SqlDbType = SqlDbType.UniqueIdentifier });
                    cmd.Parameters.Add(new SqlParameter("@p6", (object?)pedido.Persona_Contacto ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p7", (object?)pedido.Tipo_venta ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p8", pedido.Fecha_entrega.HasValue ? pedido.Fecha_entrega.Value : (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p9", (object?)pedido.Condiciones_pago ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p10", (object?)pedido.Prioridad ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p11", (object?)pedido.Direccion_entrega ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p12", string.IsNullOrEmpty(pedido.Estado) ? PedidoEstado.Creado : pedido.Estado));
                    cmd.Parameters.Add(new SqlParameter("@p13", (object?)pedido.Notas ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p14", pedido.Anulado));
                    cmd.Parameters.Add(new SqlParameter("@p15", pedido.SubTotal));
                    cmd.Parameters.Add(new SqlParameter("@p16", pedido.Porc_Itbis));
                    cmd.Parameters.Add(new SqlParameter("@p17", pedido.Monto_Itbis));
                    cmd.Parameters.Add(new SqlParameter("@p18", pedido.Total));
                    cmd.ExecuteNonQuery();
                }

                foreach (PedidoDetalle det in pedido.Detalle)
                {
                    using SqlCommand cmd = new SqlCommand(
                        R.QUERY.COMMERCIAL.SQL_INSERT_PEDIDO_DETALLE,
                        conn, tran);
                    cmd.Parameters.Add(new SqlParameter("@p1", pedido.Numero));
                    cmd.Parameters.Add(new SqlParameter("@p2", (object?)det.Product_id ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p3", (object?)det.Product_name ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p4", det.Cant));
                    cmd.Parameters.Add(new SqlParameter("@p5", (object?)det.Unidad ?? DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p6", det.Width));
                    cmd.Parameters.Add(new SqlParameter("@p7", det.Lenght));
                    cmd.Parameters.Add(new SqlParameter("@p8", det.Msi));
                    cmd.Parameters.Add(new SqlParameter("@p9", det.Precio.HasValue ? det.Precio.Value : (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p10", det.Total_Renglon.HasValue ? det.Total_Renglon.Value : (object)DBNull.Value));
                    cmd.Parameters.Add(new SqlParameter("@p11", (object?)det.Notas ?? DBNull.Value));
                    cmd.ExecuteNonQuery();
                }

                tran.Commit();
                return true;
            }
            catch (Exception ex)
            {
                if (tran != null)
                {
                    try
                    {
                        tran.Rollback();
                    }
                    catch (Exception rollbackEx)
                    {
                        ServiceLogger.Log("No se pudo revertir la transaccion del pedido " + pedido.Numero + ": " + rollbackEx.Message);
                    }
                }

                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al grabar el pedido: " + ex.Message);
                return false;
            }
            finally
            {
                tran?.Dispose();
            }
        }

        public bool AnularPedido(string numero)
        {
            try
            {
                using SqlConnection conn = new(_conn);
                conn.Open();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_ANULAR_PEDIDO
                };
                cmd.Parameters.Add(new SqlParameter("@p1", numero));
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al anular el pedido: " + ex.Message);
                return false;
            }
        }

        public bool ActualizarEstadoPedido(string numero, string estado)
        {
            string estadoNormalizado = estado.ToLowerInvariant().Trim();
            if (estadoNormalizado != PedidoEstado.Creado
                && estadoNormalizado != PedidoEstado.EnProduccion
                && estadoNormalizado != PedidoEstado.Pickeado
                && estadoNormalizado != PedidoEstado.Despachado
                && estadoNormalizado != PedidoEstado.Devuelto)
            {
                return false;
            }

            try
            {
                using SqlConnection conn = new(_conn);
                conn.Open();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.COMMERCIAL.SQL_UPDATE_PEDIDO_ESTADO
                };
                cmd.Parameters.Add(new SqlParameter("@p1", numero));
                cmd.Parameters.Add(new SqlParameter("@p2", estadoNormalizado));
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al actualizar el estado del pedido: " + ex.Message);
                return false;
            }
        }
    }
}
