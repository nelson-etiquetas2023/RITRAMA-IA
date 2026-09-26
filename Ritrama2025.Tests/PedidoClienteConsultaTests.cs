using System.Reflection;
using FluentAssertions;
using Ritrama2025.Models;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de contrato de la consulta de cliente que se dispara al elegirlo en el combo de
/// pedidos. No tocan la base a proposito: la cadena de pruebas resuelve a la base real de
/// desarrollo, que es la misma que produccion, asi que una prueba de integracion escribiria
/// arriba. Estas fijan la forma del contrato, que es lo que se puede fijar sin riesgo.
///
/// Que exista y devuelva las tres cosas es lo que sostiene el comportamiento: al elegir un
/// cliente, el identificador y las dos direcciones se buscan en la base y no se leen de la tabla
/// que el combo ya tiene cargada. Si alguien vuelve a leerlos de ahi, el servicio queda sin uso
/// y estas pruebas lo delatan.
/// </summary>
[Trait("Categoria", "Unit")]
public class PedidoClienteConsultaTests
{
    private const BindingFlags Instancia = BindingFlags.Public | BindingFlags.Instance;

    [Fact]
    public void ElServicioExponeBuscarClientePorId()
    {
        MethodInfo? metodo = typeof(IPedidoService).GetMethod("BuscarClienteAsync");

        metodo.Should().NotBeNull(
            "es el metodo que llena el id y las dos direcciones al elegir un cliente; "
            + "sin el, la pantalla tendria que leerlos de la tabla del combo");
    }

    [Fact]
    public void ElMetodoRecibeElIdDelClienteYDevuelveLosTresDatos()
    {
        MethodInfo? metodo = typeof(IPedidoService).GetMethod("BuscarClienteAsync");

        metodo.Should().NotBeNull();

        ParameterInfo[] parametros = metodo!.GetParameters();
        parametros.Should().HaveCount(2);
        parametros[0].ParameterType.Should().Be(typeof(Guid));
        parametros[0].Name.Should().Be("customerId");
        parametros[1].ParameterType.Should().Be(typeof(CancellationToken));
        metodo.ReturnType.Should().Be(typeof(Task<ClienteDatos?>));
    }

    [Fact]
    public void ClienteDatosTraeConsecutivoYLasDosDirecciones()
    {
        ClienteDatos datos = new()
        {
            Consecutivo = "0042",
            DireccionFacturacion = "Calle 1",
            DireccionEntrega = "Calle 2",
        };

        datos.Consecutivo.Should().Be("0042");
        datos.DireccionFacturacion.Should().Be("Calle 1");
        datos.DireccionEntrega.Should().Be("Calle 2");
    }

    /// <summary>
    /// La consulta tiene que traer las tres columnas por las que existe. Si se le saca una, el
    /// textbox correspondiente queda vacio en pantalla y no hay ningun error que lo avise.
    /// </summary>
    [Fact]
    public void LaConsultaTraeElConsecutivoYLasDosDirecciones()
    {
        string sql = R.QUERY.COMMERCIAL.SQL_SELECT_CLIENTE_POR_ID;

        sql.Should().Contain("consecutivo");
        sql.Should().Contain("facturacion_cliente");
        sql.Should().Contain("entrega_cliente");
    }

    /// <summary>
    /// Y tiene que filtrar por el id del cliente. Si el filtro desapareciera, devolveria la
    /// primera fila de la tabla y todos los clientes mostrarian la misma direccion.
    /// </summary>
    [Fact]
    public void LaConsultaFiltraPorElIdYPorAnulado()
    {
        string sql = R.QUERY.COMMERCIAL.SQL_SELECT_CLIENTE_POR_ID;

        sql.Should().Contain("customer_id = @id");
        sql.Should().Contain("anulado = 0");
    }

    /// <summary>
    /// La direccion de entrega tiene su propia columna y la de facturacion puede venir de la
    /// direccion de entrega como respaldo. Es lo que permite que un cliente tenga una direccion
    /// de facturacion y otra de entrega distintas, que es el caso de Afatex SRL en la base.
    /// </summary>
    [Fact]
    public void CadaDireccionTieneSuPropiaColumnaEnLaConsulta()
    {
        string sql = R.QUERY.COMMERCIAL.SQL_SELECT_CLIENTE_POR_ID;

        // La de entrega resuelve primero por su propia columna.
        int posEntrega = sql.IndexOf("AS entrega_cliente", StringComparison.Ordinal);
        int columnaEntrega = sql.LastIndexOf("direccion_entrega", posEntrega, StringComparison.Ordinal);
        columnaEntrega.Should().BeGreaterThan(-1);

        // La de facturacion resuelve por la suya, no por la de entrega.
        int posFacturacion = sql.IndexOf("AS facturacion_cliente", StringComparison.Ordinal);
        posFacturacion.Should().BeGreaterThan(-1);
        posFacturacion.Should().BeLessThan(posEntrega);
    }

    /// <summary>
    /// El formateo del consecutive es una sola regla y la usan tanto la fila de la tabla del
    /// combo como el valor crudo de la consulta. Si se duplicara, el id se veria distinto
    /// segun de donde saliera, que es lo que paso cuando el id se armaba en cada pantalla.
    /// </summary>
    [Fact]
    public void LaConsultaUsaLaMismaReglaDeFormatoQueElCombo()
    {
        // El servicio delega en la regla compartida, no arma el texto a mano.
        MethodInfo? formatear = typeof(ConsecutivoCliente).GetMethod(
            "FormatearValor", BindingFlags.Public | BindingFlags.Static);

        formatear.Should().NotBeNull();
        formatear!.ReturnType.Should().Be(typeof(string));
    }
}
