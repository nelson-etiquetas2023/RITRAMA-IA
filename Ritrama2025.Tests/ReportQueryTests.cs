using FluentAssertions;
using Microsoft.Data.SqlClient;
using Ritrama2025;
using System.Data;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Valida que las consultas SQL de los reportes (incluidas las que se modificaron para
/// rendimiento: filtros de ordenes activas y browser de master) se ejecuten sin errores
/// contra el esquema real. No requiere datos: un resultado vacio tambien ejercita el SQL.
/// </summary>
[Trait("Categoria", "Integracion")]
public class ReportQueryTests
{
    private static DataTable RunQuery(string sql)
    {
        var dt = new DataTable();
        using var conn = new SqlConnection(TestConfiguration.ConnectionString);
        using var cmd = new SqlCommand(sql, conn);
        conn.Open();
        using var da = new SqlDataAdapter(cmd);
        da.Fill(dt);
        return dt;
    }

    [Fact]
    public void ReporteInventarioMaster_EjecutaSinError()
    {
        var dt = RunQuery(R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_ROLL_ID);
        dt.Should().NotBeNull();
    }

    [Fact]
    public void ReporteRollosCortados_EjecutaSinError()
    {
        var dt = RunQuery(R.QUERY.PRODUCTION.SQL_QUERY_LOAD_INVENTARIO_ROLLO_CORTADO);
        dt.Should().NotBeNull();
    }

    [Fact]
    public void CargaOrdenCorteHeader_EjecutaSinError()
    {
        var dt = RunQuery(R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_OC_HEADER);
        dt.Should().NotBeNull();
    }

    [Fact]
    public void CargaOrdenCorteRollos_EjecutaSinError()
    {
        var dt = RunQuery(R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_OC_ROLLO_CORTADO);
        dt.Should().NotBeNull();
    }

    [Fact]
    public void CargaOrdenCorteCortes_EjecutaSinError()
    {
        var dt = RunQuery(R.QUERY.PRODUCTION.SQL_QUERY_SELECT_LOAD_OC_CORTES);
        dt.Should().NotBeNull();
    }
}
