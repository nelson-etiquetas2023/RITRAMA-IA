using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Models;
using Ritrama2025.Services.ProduccionService;

namespace Ritrama2025.Services.OrdenesCompras
{
    /// <summary>
    /// Servicio de órdenes de compra. Reúne la lógica de acceso a datos que FrmOrdenesCompra
    /// necesita, siguiendo el mismo patrón que PedidoService: resuelve la cadena de conexión
    /// del ambiente activo, valida antes de tocar la base, y reserva el número con UPDLOCK
    /// dentro de la transacción para que un fallo devuelva el número al contador.
    /// </summary>
    public class OrdenesComprasService : IOrdenesComprasService
    {
        private readonly string _conn;

        public OrdenesComprasService(IConfiguration config)
        {
            _conn = ConexionResolver.Resolver(config);
        }

        public string? ErrorMsg { get; set; }

        public async Task<DataTable> LoadDataOrdenesCompra(CancellationToken ct = default)
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
                    CommandText = R.QUERY.PURCHASE.SQL_SELECT_OC
                };
                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al cargar las órdenes de compra: " + ex.Message);
            }
            return dt;
        }

        public async Task<DataTable> LoadDataProveedores(CancellationToken ct = default)
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
                    CommandText = R.QUERY.PURCHASE.SQL_SELECT_LOAD_PROVEEDOR_COMBO
                };
                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al cargar los proveedores: " + ex.Message);
            }
            return dt;
        }

        public async Task<ProveedorDatos?> BuscarProveedorAsync(string? proveedorId, CancellationToken ct = default)
        {
            if (string.IsNullOrWhiteSpace(proveedorId))
            {
                return null;
            }

            try
            {
                ct.ThrowIfCancellationRequested();
                using SqlConnection conn = new(_conn);
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.PURCHASE.SQL_SELECT_PROVEEDOR_POR_ID
                };
                cmd.Parameters.Add(new SqlParameter("@id", proveedorId));

                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataReader reader = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);

                if (!await reader.ReadAsync(ct).ConfigureAwait(false))
                {
                    return null;
                }

                string direccion = Texto(reader, "direccion");
                return new ProveedorDatos
                {
                    DireccionEntrega = direccion,
                    DireccionFacturacion = direccion
                };
            }
            catch (OperationCanceledException)
            {
                throw;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al buscar el proveedor: " + ex.Message);
                return null;
            }

            static string Texto(SqlDataReader reader, string columna)
            {
                int ordinal = reader.GetOrdinal(columna);
                return reader.IsDBNull(ordinal) ? string.Empty : reader.GetString(ordinal);
            }
        }

        public async Task<DataTable> LoadDataOrdenCompraDetalle(string numero, CancellationToken ct = default)
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
                    CommandText = R.QUERY.PURCHASE.SQL_SELECT_OC_DETALLE
                };
                cmd.Parameters.Add(new SqlParameter("@p1", numero));
                await conn.OpenAsync(ct).ConfigureAwait(false);
                using SqlDataAdapter da = new(cmd);
                da.Fill(dt);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al cargar el detalle de la orden de compra: " + ex.Message);
            }
            return dt;
        }

        /// <summary>
        /// Previsualiza el número que tendrá la próxima OC, SIN tocar el contador.
        /// </summary>
        public async Task<string> GetProximoNumeroOrdenCompra(CancellationToken ct = default)
        {
            try
            {
                ct.ThrowIfCancellationRequested();
                using SqlConnection conn = new(_conn);
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.PURCHASE.SQL_SELECT_OC_PROXIMO
                };
                await conn.OpenAsync(ct).ConfigureAwait(false);
                object? valor = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
                return FormatearConsecutivo(valor);
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al previsualizar el número de orden de compra: " + ex.Message);
                throw;
            }
        }

        private static string FormatearConsecutivo(object? valor)
        {
            if (valor == null || valor == DBNull.Value || !int.TryParse(valor.ToString(), out int consecutivo))
            {
                return string.Empty;
            }

            return consecutivo < 1 || consecutivo > OrdenesCompraNumero.MaximoNumero
                ? string.Empty
                : OrdenesCompraNumero.Formatear(consecutivo);
        }

        /// <summary>
        /// Reserva el número de la OC dentro de la transacción abierta.
        /// </summary>
        private static bool ReservarNumeroOrdenCompra(SqlConnection conn, SqlTransaction tran, out string numero, out string error)
        {
            numero = string.Empty;
            error = string.Empty;

            try
            {
                using SqlCommand cmd = new(
                    R.QUERY.PURCHASE.SQL_QUERY_CONSUMO_OC_CONSECUTIVO,
                    conn, tran);
                object? valor = cmd.ExecuteScalar();

                if (valor == null || valor == DBNull.Value || !int.TryParse(valor.ToString(), out int consecutivo))
                {
                    error = "No se pudo leer el consecutivo de órdenes de compra.";
                    return false;
                }

                if (consecutivo > OrdenesCompraNumero.MaximoNumero)
                {
                    error = "Se alcanzó el máximo de " + OrdenesCompraNumero.MaximoNumero
                        + " órdenes de compra del consecutivo. Reinicie el contador para seguir.";
                    return false;
                }

                if (consecutivo < 1)
                {
                    error = "El consecutivo de órdenes de compra está en " + consecutivo
                        + ". Revise el registro control de la base de datos.";
                    return false;
                }

                numero = OrdenesCompraNumero.Formatear(consecutivo);
                return true;
            }
            catch (Exception ex)
            {
                error = "Error al reservar el número de orden de compra: " + ex.Message;
                return false;
            }
        }

        public bool SaveOrdenCompraCompleto(OrdenCompra orden)
        {
            if (!OrdenesCompraValidador.EsValido(orden, out string error))
            {
                ErrorMsg = error;
                return false;
            }

            using SqlConnection conn = new SqlConnection(_conn);
            SqlTransaction? tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                if (!ReservarNumeroOrdenCompra(conn, tran, out string numeroReservado, out string errorReserva))
                {
                    tran.Rollback();
                    tran.Dispose();
                    tran = null;
                    ErrorMsg = errorReserva;
                    ServiceErrors.Report(errorReserva);
                    return false;
                }

                orden.Numero = numeroReservado;

                if (!OrdenesCompraValidador.EsValido(orden, out error))
                {
                    tran.Rollback();
                    tran.Dispose();
                    tran = null;
                    ErrorMsg = error;
                    return false;
                }

                using (SqlCommand cmd = new SqlCommand(
                    R.QUERY.PURCHASE.SQL_INSERT_OC,
                    conn, tran))
                {
                    AgregarParametrosEncabezado(cmd, orden);
                    cmd.ExecuteNonQuery();
                }

                InsertarDetalle(conn, tran, orden);

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
                        ServiceLogger.Log("No se pudo revertir la transacción de la OC " + orden.Numero + ": " + rollbackEx.Message);
                    }
                }

                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al grabar la orden de compra: " + ex.Message);
                return false;
            }
            finally
            {
                tran?.Dispose();
            }
        }

        /// <summary>
        /// Inserta las líneas de la OC en orden_compra_detalle dentro de la transacción.
        /// </summary>
        private static void InsertarDetalle(SqlConnection conn, SqlTransaction tran, OrdenCompra orden)
        {
            foreach (OrdenCompraDetalle det in orden.Detalle)
            {
                using SqlCommand cmd = new SqlCommand(
                    R.QUERY.PURCHASE.SQL_INSERT_OC_DETALLE,
                    conn, tran);
                cmd.Parameters.Add(new SqlParameter("@p1", orden.Numero));
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
        }

        /// <summary>
        /// Carga los 17 parámetros del encabezado de la OC. Los mismos nombres, orden y tipos
        /// para el INSERT de una OC nueva y para el UPDATE de la edición.
        /// </summary>
        private static void AgregarParametrosEncabezado(SqlCommand cmd, OrdenCompra orden)
        {
            cmd.Parameters.Add(new SqlParameter("@p1", orden.Numero));
            cmd.Parameters.Add(new SqlParameter("@p2", orden.Fecha));
            cmd.Parameters.Add(new SqlParameter("@p3", (object?)orden.Proveedor_Id ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p4", (object?)orden.Proveedor_Name ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p5", (object?)orden.Persona_Contacto ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p6", orden.Fecha_entrega.HasValue ? orden.Fecha_entrega.Value : (object)DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p7", (object?)orden.Direccion_entrega ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p8", (object?)orden.Direccion_facturacion ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p9", (object?)orden.Condiciones_pago ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p10", (object?)orden.Prioridad ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p11", string.IsNullOrEmpty(orden.Estado) ? OrdenCompraEstado.Creado : orden.Estado));
            cmd.Parameters.Add(new SqlParameter("@p12", (object?)orden.Notas ?? DBNull.Value));
            cmd.Parameters.Add(new SqlParameter("@p13", orden.Anulado));
            cmd.Parameters.Add(new SqlParameter("@p14", orden.SubTotal));
            cmd.Parameters.Add(new SqlParameter("@p15", orden.Porc_Itbis));
            cmd.Parameters.Add(new SqlParameter("@p16", orden.Monto_Itbis));
            cmd.Parameters.Add(new SqlParameter("@p17", orden.Total));
        }

        public bool ActualizarOrdenCompraCompleto(OrdenCompra orden)
        {
            if (!OrdenesCompraValidador.EsValido(orden, out string error))
            {
                ErrorMsg = error;
                return false;
            }

            using SqlConnection conn = new SqlConnection(_conn);
            SqlTransaction? tran = null;
            try
            {
                conn.Open();
                tran = conn.BeginTransaction();

                int rowsAffected;
                using (SqlCommand cmd = new SqlCommand(
                    R.QUERY.PURCHASE.SQL_UPDATE_OC,
                    conn, tran))
                {
                    AgregarParametrosEncabezado(cmd, orden);
                    rowsAffected = cmd.ExecuteNonQuery();
                }

                if (rowsAffected != 1)
                {
                    tran.Rollback();
                    tran.Dispose();
                    tran = null;
                    ErrorMsg = "La orden de compra " + orden.Numero + " ya no existe.";
                    return false;
                }

                using (SqlCommand cmd = new SqlCommand(
                    R.QUERY.PURCHASE.SQL_DELETE_OC_DETALLE,
                    conn, tran))
                {
                    cmd.Parameters.Add(new SqlParameter("@p1", orden.Numero));
                    cmd.ExecuteNonQuery();
                }

                InsertarDetalle(conn, tran, orden);

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
                        ServiceLogger.Log("No se pudo revertir la transacción de la OC " + orden?.Numero + ": " + rollbackEx.Message);
                    }
                }

                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al actualizar la orden de compra " + orden?.Numero + ": " + ex.Message);
                return false;
            }
            finally
            {
                tran?.Dispose();
            }
        }

        public bool AnularOrdenCompra(string numero)
        {
            try
            {
                using SqlConnection conn = new(_conn);
                conn.Open();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.PURCHASE.SQL_ANULAR_OC
                };
                cmd.Parameters.Add(new SqlParameter("@p1", numero));
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al anular la orden de compra: " + ex.Message);
                return false;
            }
        }

        public bool RestaurarOrdenCompra(string numero)
        {
            try
            {
                using SqlConnection conn = new(_conn);
                conn.Open();
                using SqlCommand cmd = new()
                {
                    Connection = conn,
                    CommandType = CommandType.Text,
                    CommandText = R.QUERY.PURCHASE.SQL_RESTAURAR_OC
                };
                cmd.Parameters.Add(new SqlParameter("@p1", numero));
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al restaurar la orden de compra: " + ex.Message);
                return false;
            }
        }

        public bool ActualizarEstadoOrdenCompra(string numero, string estado)
        {
            string estadoNormalizado = estado.ToLowerInvariant().Trim();
            if (estadoNormalizado != OrdenCompraEstado.Creado
                && estadoNormalizado != OrdenCompraEstado.Ordenado
                && estadoNormalizado != OrdenCompraEstado.Parcial
                && estadoNormalizado != OrdenCompraEstado.Recibido
                && estadoNormalizado != OrdenCompraEstado.Cancelado)
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
                    CommandText = R.QUERY.PURCHASE.SQL_UPDATE_OC_ESTADO
                };
                cmd.Parameters.Add(new SqlParameter("@p1", numero));
                cmd.Parameters.Add(new SqlParameter("@p2", estadoNormalizado));
                int rowsAffected = cmd.ExecuteNonQuery();
                return rowsAffected > 0;
            }
            catch (Exception ex)
            {
                ErrorMsg = ex.Message;
                ServiceErrors.Report("Error al actualizar el estado de la orden de compra: " + ex.Message);
                return false;
            }
        }
    }
}
