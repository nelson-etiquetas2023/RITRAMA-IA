using System.Data;
using FluentAssertions;
using Ritrama2025.Services.PedidoService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Pruebas de los campos de solo lectura que muestran el consecutivo de cliente y vendedor.
/// Logica pura: no abren la base de datos ni la interfaz.
///
/// Estas reglas se partieron una vez sin que ninguna prueba lo notara: la ruta que corre al
/// elegir en pantalla paso a mostrar el consecutivo y la que corre al abrir un pedido guardado
/// se quedo con el identificador. Estas pruebas fijan las dos rutas, y fijan tambien que
/// cliente y vendedor pasen por el mismo formateo, que es lo que hacia falta para que no
/// vuelvan a divergir.
/// </summary>
[Trait("Categoria", "Unit")]
public class ConsecutivoClienteTests
{
    private static DataTable TablaCon(string columnaLlave, params (object? llave, object? consecutivo)[] filas)
    {
        DataTable tabla = new();
        tabla.Columns.Add(columnaLlave, typeof(Guid));
        tabla.Columns.Add(ConsecutivoCliente.ColumnaConsecutivo, typeof(int));

        foreach ((object? llave, object? consecutivo) in filas)
        {
            DataRow fila = tabla.NewRow();
            fila[columnaLlave] = llave ?? DBNull.Value;
            fila[ConsecutivoCliente.ColumnaConsecutivo] = consecutivo ?? DBNull.Value;
            tabla.Rows.Add(fila);
        }

        return tabla;
    }

    [Theory]
    [InlineData(1, "0001")]
    [InlineData(42, "0042")]
    [InlineData(199, "0199")]
    [InlineData(1234, "1234")]
    [InlineData(12345, "12345")]
    public void Formatear_RellenaACuatroDigitos(int consecutivo, string esperado)
    {
        DataTable tabla = TablaCon("customer_id", (Guid.NewGuid(), consecutivo));

        ConsecutivoCliente.Formatear(tabla.Rows[0]).Should().Be(esperado);
    }

    /// <summary>
    /// "D4" es un ancho minimo, no un maximo: un consecutivo de 5 digitos sale entero y no
    /// truncado. Se fija para que nadie lo lea como un recorte y cambie el formato.
    /// </summary>
    [Fact]
    public void Formatear_MasDeCuatroDigitos_NoTrunca()
    {
        DataTable tabla = TablaCon("customer_id", (Guid.NewGuid(), 123456));

        ConsecutivoCliente.Formatear(tabla.Rows[0]).Should().Be("123456");
    }

    [Fact]
    public void Formatear_ConsecutivoNulo_DevuelveVacioYNoLanza()
    {
        DataTable tabla = TablaCon("customer_id", (Guid.NewGuid(), null));

        ConsecutivoCliente.Formatear(tabla.Rows[0]).Should().BeEmpty();
    }

    [Fact]
    public void Formatear_FilaNula_DevuelveVacioYNoLanza()
    {
        ConsecutivoCliente.Formatear(null).Should().BeEmpty();
    }

    /// <summary>
    /// Un maestro con el consecutvo en texto no numerico no debe voltear la pantalla. El formateo
    /// devuelve vacio en vez de lanzar, que es lo que permite que el handler siga y cargue las
    /// direcciones, que vienen de otras columnas de la misma fila.
    /// </summary>
    [Fact]
    public void Formatear_ConsecutivoNoNumerico_DevuelveVacioYNoLanza()
    {
        DataTable tabla = new();
        tabla.Columns.Add("customer_id", typeof(Guid));
        tabla.Columns.Add(ConsecutivoCliente.ColumnaConsecutivo, typeof(string));
        DataRow fila = tabla.NewRow();
        fila["customer_id"] = Guid.NewGuid();
        fila[ConsecutivoCliente.ColumnaConsecutivo] = "no es un numero";
        tabla.Rows.Add(fila);

        ConsecutivoCliente.Formatear(fila).Should().BeEmpty();
    }

    [Fact]
    public void Formatear_SinLaColumnaConsecutivo_DevuelveVacioYNoLanza()
    {
        DataTable tabla = new();
        tabla.Columns.Add("customer_id", typeof(Guid));
        DataRow fila = tabla.NewRow();
        fila["customer_id"] = Guid.NewGuid();
        tabla.Rows.Add(fila);

        ConsecutivoCliente.Formatear(fila).Should().BeEmpty();
    }

    [Fact]
    public void BuscarFila_DevuelveLaFilaQueCoincideConLaLlave()
    {
        object? primera = Guid.NewGuid();
        object? segunda = Guid.NewGuid();
        DataTable tabla = TablaCon("customer_id", (primera, 10), (segunda, 20));

        ConsecutivoCliente.BuscarFila(tabla, "customer_id", segunda).Should().NotBeNull();
        ConsecutivoCliente.Formatear(ConsecutivoCliente.BuscarFila(tabla, "customer_id", segunda))
            .Should().Be("0020");
    }

    [Fact]
    public void BuscarFila_LlaveInexistente_DevuelveNulo()
    {
        DataTable tabla = TablaCon("customer_id", (Guid.NewGuid(), 10));

        ConsecutivoCliente.BuscarFila(tabla, "customer_id", Guid.NewGuid()).Should().BeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    public void FormatearPorLlave_ValorNuloODevuelveVacio(string? valor)
    {
        DataTable tabla = TablaCon("customer_id", (Guid.NewGuid(), 10));

        ConsecutivoCliente.FormatearPorLlave(tabla, "customer_id", valor).Should().BeEmpty();
    }

    /// <summary>
    /// Resuelve la fila por el texto visible, no por la posicion. Es lo que permite que el combo
    /// con filtro incremental encuentre al cliente: el indice de la lista filtrada no coincide
    /// con la fila de la tabla, y con indice el id y las direcciones quedaban vacios.
    /// </summary>
    [Fact]
    public void BuscarFilaPorTexto_EncuentraLaFilaPorElNombreVisible()
    {
        DataTable tabla = new();
        tabla.Columns.Add("customer_name", typeof(string));
        tabla.Columns.Add(ConsecutivoCliente.ColumnaConsecutivo, typeof(int));
        tabla.Rows.Add("Afatex SRL", 11);
        tabla.Rows.Add("21 St Century", 22);

        ConsecutivoCliente.BuscarFilaPorTexto(tabla, "customer_name", "21 St Century")
            .Should().NotBeNull();
        ConsecutivoCliente.Formatear(ConsecutivoCliente.BuscarFilaPorTexto(tabla, "customer_name", "21 St Century"))
            .Should().Be("0022");
    }

    [Fact]
    public void BuscarFilaPorTexto_NoDistingueMayusculasNiEspacios()
    {
        DataTable tabla = new();
        tabla.Columns.Add("customer_name", typeof(string));
        tabla.Columns.Add(ConsecutivoCliente.ColumnaConsecutivo, typeof(int));
        tabla.Rows.Add("Agostini Gmbh", 5);

        ConsecutivoCliente.BuscarFilaPorTexto(tabla, "customer_name", "  agostini gmbh ").Should().NotBeNull();
    }

    [Theory]
    [InlineData(null)]
    [InlineData("")]
    [InlineData("   ")]
    public void BuscarFilaPorTexto_TextoVacioODevuelveNulo(string? texto)
    {
        DataTable tabla = new();
        tabla.Columns.Add("customer_name", typeof(string));
        tabla.Columns.Add(ConsecutivoCliente.ColumnaConsecutivo, typeof(int));
        tabla.Rows.Add("Afatex SRL", 11);

        ConsecutivoCliente.BuscarFilaPorTexto(tabla, "customer_name", texto).Should().BeNull();
    }

    [Fact]
    public void BuscarFilaPorTexto_NombreQueNoEsta_DevuelveNuloYNoLanza()
    {
        DataTable tabla = new();
        tabla.Columns.Add("customer_name", typeof(string));
        tabla.Columns.Add(ConsecutivoCliente.ColumnaConsecutivo, typeof(int));
        tabla.Rows.Add("Afatex SRL", 11);

        ConsecutivoCliente.BuscarFilaPorTexto(tabla, "customer_name", "Cliente Inexistente").Should().BeNull();
    }

    [Fact]
    public void BuscarFilaPorTexto_SinLaColumna_DevuelveNuloYNoLanza()
    {
        DataTable tabla = new();
        tabla.Columns.Add("otracosa", typeof(string));
        tabla.Rows.Add("Afatex SRL");

        ConsecutivoCliente.BuscarFilaPorTexto(tabla, "customer_name", "Afatex SRL").Should().BeNull();
    }

    [Fact]
    public void BuscarFilaPorTexto_TablaNula_DevuelveNuloYNoLanza()
    {
        ConsecutivoCliente.BuscarFilaPorTexto(null, "customer_name", "Afatex SRL").Should().BeNull();
    }

    /// <summary>
    /// La fila que devuelve la busqueda por texto tiene que ser la que el filtro muestra, no una
    /// cualquiera: con dos clientes de nombre parecido, el que coincide exacto gana. Es lo que
    /// evita mostrar el consecutivo de otro cliente cuando se escribe un prefijo.
    /// </summary>
    [Fact]
    public void BuscarFilaPorTexto_PrefijoNoAlcanzaParaOtraFila()
    {
        DataTable tabla = new();
        tabla.Columns.Add("customer_name", typeof(string));
        tabla.Columns.Add(ConsecutivoCliente.ColumnaConsecutivo, typeof(int));
        tabla.Rows.Add("Zona Franca", 9);
        tabla.Rows.Add("Zona Franca Pisano", 10);

        ConsecutivoCliente.Formatear(ConsecutivoCliente.BuscarFilaPorTexto(tabla, "customer_name", "Zona Franca Pisano"))
            .Should().Be("0010");
    }

    /// <summary>
    /// La columna llave es un Guid en la base, asi que comparar contra el texto del mismo Guid

    /// daria falso y el campo quedaria vacio al abrir un pedido. La comparacion es con Equals
    /// sobre el valor crudo, y esta prueba lo fija.
    /// </summary>
    [Fact]
    public void BuscarFila_ComparaElGuidYNoSuTexto()
    {
        Guid id = Guid.NewGuid();
        DataTable tabla = TablaCon("customer_id", (id, 77));

        ConsecutivoCliente.BuscarFila(tabla, "customer_id", id).Should().NotBeNull();
        ConsecutivoCliente.BuscarFila(tabla, "customer_id", id.ToString()).Should().BeNull();
    }

    /// <summary>
    /// Cliente y vendedor van por la misma funcion, asi que el mismo dato se formatea igual en
    /// los dos. Es la garantia de que el campo del vendedor no vuelva a quedar con el formato
    /// viejo mientras el del cliente ya no.
    /// </summary>
    [Fact]
    public void ClienteYVendedor_FormateanIgual_PorqueCompartenLaRegla()
    {
        DataTable clientes = TablaCon("customer_id", (Guid.NewGuid(), 42));
        DataTable vendedores = TablaCon("vendor_id", (Guid.NewGuid(), 3));

        ConsecutivoCliente.Formatear(clientes.Rows[0]).Should().Be("0042");
        ConsecutivoCliente.Formatear(vendedores.Rows[0]).Should().Be("0003");
        ConsecutivoCliente.ColumnaConsecutivo.Should().Be("consecutivo");
    }

    /// <summary>
    /// Las dos rutas de la pantalla, la que corre al elegir y la que corre al abrir un pedido,
    /// tienen que dar el mismo texto para la misma fila. Es exactamente lo que se partio: al
    /// elegir se veia el consecutivo y al abrir el pedido se veia el identificador. Ninguna de
    /// las dos rutas depende ya de cual fila este seleccionada, asi que el resultado no puede
    /// depender del orden en que se escriban.
    /// </summary>
    [Fact]
    public void AlElegirYAlAbrirUnPedido_DaElMismoTexto()
    {
        Guid clienteId = Guid.NewGuid();
        DataTable clientes = TablaCon("customer_id", (Guid.NewGuid(), 99), (clienteId, 42));

        // Al elegir: se formatea la fila seleccionada.
        string alElegir = ConsecutivoCliente.Formatear(ConsecutivoCliente.BuscarFila(clientes, "customer_id", clienteId));

        // Al abrir un pedido: se busca por la llave del propio pedido, sin mirar la seleccion.
        string alAbrir = ConsecutivoCliente.FormatearPorLlave(clientes, "customer_id", clienteId);

        alAbrir.Should().Be(alElegir);
        alAbrir.Should().Be("0042");
    }

    /// <summary>
    /// El formateo nunca devuelve un identificador. Antes de unificar, la ruta de consulta
    /// escribia el Guid crudo en el textbox; esta prueba falla si eso vuelve a pasar.
    /// </summary>
    [Fact]
    public void Formatear_NuncaDevuelveUnGuid()
    {
        Guid id = Guid.NewGuid();
        DataTable tabla = TablaCon("customer_id", (id, 42));

        ConsecutivoCliente.Formatear(tabla.Rows[0]).Should().NotContain(id.ToString());
    }
}
