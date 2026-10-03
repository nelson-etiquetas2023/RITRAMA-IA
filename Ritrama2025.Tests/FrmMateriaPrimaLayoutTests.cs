using System.Data;
using FluentAssertions;
using Microsoft.Data.SqlClient;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.CommonData;
using Ritrama2025.Services.CommonService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.MateriaPrima;
using Ritrama2025.Services.ReportsService.ReportsService;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño de FrmMateriaPrima (Recepción de Materia Prima):
/// todo el formulario va en paneles dockeados al estilo de FrmPedidos, el grid ocupa el
/// panel central completo y los controles no se pisan entre si al redimensionar.
/// No toca base de datos: los servicios van con stubs.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmMateriaPrimaLayoutTests
{
    private sealed class MateriaPrimaServiceStub : IServiceMateriaPrima
    {
        private readonly DataSet _datos;

        public MateriaPrimaServiceStub(DataSet? datos = null) => _datos = datos ?? DatasetVacio();

        public Task<DataSet> LoadData() => Task.FromResult(_datos);

        public Task LoadProducts() => Task.CompletedTask;

        public Task LoadTableHeaderMateriaPrima() => Task.CompletedTask;

        public Task LoadTableDetailsMateriaPrima() => Task.CompletedTask;

        public Task LoadTableProveedores() => Task.CompletedTask;

        public Task LoadTableTransportista() => Task.CompletedTask;

        public Task SetRelationsTables() => Task.CompletedTask;

        public bool GuardarOrden(OrdenMP orden) => true;

        public int LoadConsecOrden(string filtro) => 1;

        public bool UpdateConsecOrden(string NumConsec) => true;

        public bool CloseOrder(string orden) => true;

        public bool UpDateLogsNotes(string orden, string logText) => true;

        public bool AnularOrden(string orden) => true;
    }

    private sealed class ExportDataServiceStub : IExportDataService
    {
        public bool ExportToExcel<T>(List<T> data, string FileName) => true;

        public bool ExportToExcelProducts<T>(List<T> data, string FileName) => true;

        public bool ExportTxtFormatRollosCortados(DataRow[] rollos, bool solo_rc, string? fecha_produccion, string? fecha_registro, bool openNotePad) => true;

        public bool ExportTxtFormatMasterRePrintLabel(ProductMAP master, bool openNotePad) => true;
    }

    private sealed class ReportsServiceStub : IReportsService
    {
        public void Reporte_Orden_Corte(string orden, Form form, string ReportName, string TitleReport) { }

        public void Reporte_Desperdicios(string orden, Form form, string ReportName, string TitleReport) { }

        public void Reporte_Orden_MatPrima(string orden, Form form, string ReportName, string TitleReport) { }

        public void ReporteConduce_conPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

        public void ReporteCondece_sinPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

        public void Reporte_PackingList(string conduce, Form form) { }

        public void Reporte_DetallePaleta(string conduce, Form form) { }

        public void Reporte_InventarioRollosCortados(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_InventarioMaster(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Productos(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Clientes(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Proveedores(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Vendedores(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Usuarios(Form form, string Report_Title, string Report_Name) { }
    }

    private sealed class CommonDataServiceStub : IServiceCommonData
    {
        public ObjectQuery CreateObjectQuery(ObjectQuery objectquery, DataSet dataset) => objectquery;

        public ObjectQuery CreateObjectProduct(SqlDataAdapter da) => new ObjectQuery();

        public Task LoadTable(ObjectQuery objectQuery) => Task.CompletedTask;

        public int GetConsecutive(string filtro) => 0;

        public bool VerificarRollIdNoRepeat(string rollid) => true;
    }

    private sealed class CommonServiceStub : ICommonService
    {
        public Task<List<RolloCortado>> GetDataRolloCortado(List<RolloCortado> lista) => Task.FromResult(lista);

        public void SaveTransportEntity(string Id, string Name) { }

        public void DeleteTransportEntity(string Id) { }

        public void SaveChoferEntity(string Id, string Name) { }

        public void DeleteChoferEntity(string Id) { }

        public void SaveCamionEntity(string Id, string Name) { }

        public void DeleteCamionEntity(string Id) { }

        public void SaveProvaiderEntity(string Id, string Name) { }

        public void DeleteProvaiderEntity(string Id) { }

        public void SavePersonEntity(string Id, string Name) { }

        public void DeletePersonEntity(string Id) { }

        public void SaveOperatorEntity(string Id, string Name) { }

        public void DeleteOperatorEntity(string Id) { }

        public void SaveCustomerEntity(string Id, string Name) { }

        public void SaveVendedorEntity(string Id, string Name) { }

        public bool DocumentCheckWriteOC(DocumentCheckOC doc) => true;

        public DocumentCheckOC DocumentCheckReadOC(string oc) => new DocumentCheckOC();

        public RolloCortado SearchCodigoUnico(string id) => new RolloCortado();
    }

    /// <summary>
    /// DataSet con la misma forma que arma ServiceMateriaPrima: las seis tablas y la
    /// relacion maestro-detalle que el formulario espera para enlazar sus controles.
    /// </summary>
    private static DataSet DatasetDePrueba()
    {
        DataSet ds = DatasetVacio();

        DataTable materia = ds.Tables["DtMateria"];
        materia.Rows.Add("10", "OC-00010", Guid.NewGuid(), "Recepcion", DateTime.Today, DateTime.Today,
            Guid.NewGuid(), "GUIA-1", "LOTE-1", "EMB-1", 1, "notas", Guid.NewGuid(), false, false, "creado", 10);
        materia.Rows.Add("2", "OC-00002", Guid.NewGuid(), "Recepcion", DateTime.Today, DateTime.Today,
            Guid.NewGuid(), "GUIA-2", "LOTE-2", "EMB-2", 1, "notas", Guid.NewGuid(), false, false, "creado", 2);

        // Renglones por nombre de columna: el orden posicional del DataTable cambia con
        // cualquier columna nueva y mandaba el roll-id a la columna msi.
        DataTable detalle = ds.Tables["DtDetalle"];
        foreach ((string numero, string producto, string roll) in new[]
        {
            ("10", "P-001", "ROLL-10"),
            ("2", "P-002", "ROLL-2"),
        })
        {
            DataRow fila = detalle.NewRow();
            fila["numero"] = numero;
            fila["product_id"] = producto;
            fila["product_name"] = $"Producto {producto}";
            fila["type"] = "Master";
            fila["cant_pedido"] = 1m;
            fila["cant_real"] = 0m;
            fila["width"] = 10.5m;
            fila["length"] = 100m;
            fila["msi"] = 2.2m;
            fila["rollid"] = roll;
            fila["splice"] = 1;
            fila["ubicacion"] = "UBI";
            fila["core"] = 3m;
            fila["empalme"] = 1;
            fila["fecha_produccion"] = DateTime.Today;
            fila["factura"] = "FAC-1";
            fila["num_paleta"] = "P-1";
            fila["fecha_llegada"] = DateTime.Today;
            fila["estado"] = "Completo";
            detalle.Rows.Add(fila);
        }

        DataColumn padre = ds.Tables["DtMateria"].Columns["numero"];
        DataColumn hijo = ds.Tables["DtDetalle"].Columns["numero"];
        Assert.True(ds.Relations.Contains("FK_MASTER_DETAILS"));

        return ds;
    }

    private static DataSet DatasetVacio()
    {
        DataSet ds = new DataSet();

        DataTable materia = new DataTable("DtMateria");
        materia.Columns.Add("numero", typeof(string));
        materia.Columns.Add("Orden_Compra", typeof(string));
        materia.Columns.Add("proveedor_id", typeof(Guid));
        materia.Columns.Add("persona_respons", typeof(string));
        materia.Columns.Add("fecha_pro", typeof(DateTime));
        materia.Columns.Add("fecha_recepcion", typeof(DateTime));
        materia.Columns.Add("transport_id", typeof(Guid));
        materia.Columns.Add("guia_import", typeof(string));
        materia.Columns.Add("lote", typeof(string));
        materia.Columns.Add("doc_embarque", typeof(string));
        materia.Columns.Add("total_cantidad", typeof(int));
        materia.Columns.Add("notas", typeof(string));
        materia.Columns.Add("person_id", typeof(Guid));
        materia.Columns.Add("CloseDocument", typeof(bool));
        materia.Columns.Add("Anulado", typeof(bool));
        materia.Columns.Add("estado", typeof(string));
        // Columnas que el servicio agrega como expresion de la relacion con los catalogos.
        materia.Columns.Add("proveedor_name", typeof(string));
        materia.Columns.Add("transport_name", typeof(string));
        materia.Columns.Add("person_name", typeof(string));
        ds.Tables.Add(materia);

        DataTable detalle = new DataTable("DtDetalle");
        detalle.Columns.Add("numero", typeof(string));
        detalle.Columns.Add("product_id", typeof(string));
        detalle.Columns.Add("product_name", typeof(string));
        detalle.Columns.Add("type", typeof(string));
        detalle.Columns.Add("cant_pedido", typeof(decimal));
        detalle.Columns.Add("cant_real", typeof(decimal));
        detalle.Columns.Add("width", typeof(decimal));
        detalle.Columns.Add("length", typeof(decimal));
        detalle.Columns.Add("msi", typeof(decimal));
        detalle.Columns.Add("rollid", typeof(string));
        detalle.Columns.Add("splice", typeof(int));
        detalle.Columns.Add("ubicacion", typeof(string));
        detalle.Columns.Add("core", typeof(decimal));
        detalle.Columns.Add("empalme", typeof(int));
        detalle.Columns.Add("fecha_produccion", typeof(DateTime));
        detalle.Columns.Add("factura", typeof(string));
        detalle.Columns.Add("num_paleta", typeof(string));
        detalle.Columns.Add("fecha_llegada", typeof(DateTime));
        detalle.Columns.Add("estado", typeof(string));
        ds.Tables.Add(detalle);

        DataTable proveedor = new DataTable("DtProvider");
        proveedor.Columns.Add("proveedor_id", typeof(Guid));
        proveedor.Columns.Add("Proveedor_Name", typeof(string));
        ds.Tables.Add(proveedor);

        DataTable transporte = new DataTable("DtTransport");
        transporte.Columns.Add("transport_id", typeof(Guid));
        transporte.Columns.Add("transport_name", typeof(string));
        ds.Tables.Add(transporte);

        DataTable persona = new DataTable("DtPerson");
        persona.Columns.Add("person_id", typeof(Guid));
        persona.Columns.Add("person_name", typeof(string));
        ds.Tables.Add(persona);

        DataTable productos = new DataTable("DtProducts");
        productos.Columns.Add("product_id", typeof(string));
        productos.Columns.Add("product_name", typeof(string));
        ds.Tables.Add(productos);

        // El formulario navega el detalle con BindingSource/DataMember "FK_MASTER_DETAILS",
        // que es el nombre de la relacion maestro-detalle (R.PARAMETERS.NAME_RELATION_OC_MASTER_DETAILS).
        ds.Relations.Add(new DataRelation("FK_MASTER_DETAILS",
            ds.Tables["DtMateria"].Columns["numero"],
            ds.Tables["DtDetalle"].Columns["numero"],
            false));

        return ds;
    }

    private static FrmMateriaPrima CrearFormulario(DataSet? datos = null)
        => new(new MateriaPrimaServiceStub(datos), new ExportDataServiceStub(), new ReportsServiceStub(),
            new CommonDataServiceStub(), new CommonServiceStub());

    [Fact]
    public void Formulario_NoReescalaAlEmbeberseYTieneTamanoMinimo()
    {
        using FrmMateriaPrima form = CrearFormulario();

        // Con AutoScaleMode.Font y una fuente propia en el disenador, Main (fuente global
        // JetBrains Mono) reescalaba todos los controles y por eso quedaban "montados".
        form.AutoScaleMode.Should().Be(AutoScaleMode.None);
        form.MinimumSize.Width.Should().BeGreaterThan(0);
        form.MinimumSize.Height.Should().BeGreaterThan(0);
    }

    [Theory]
    [InlineData("panel1", DockStyle.Top)]
    [InlineData("panelContador", DockStyle.Top)]
    [InlineData("toolStrip1", DockStyle.Top)]
    [InlineData("panelDatos", DockStyle.Top)]
    [InlineData("panelDetalle", DockStyle.Fill)]
    [InlineData("panelNotas", DockStyle.Bottom)]
    public void LosPanelesSeDockeanEnElOrdenDeLaPantalla(string nombre, DockStyle esperado)
    {
        using FrmMateriaPrima form = CrearFormulario();

        Control panel = form.Controls.Find(nombre, true).Single();
        panel.Dock.Should().Be(esperado);
    }

    [Fact]
    public void BarraDeHerramientas_NoQuedaEncimaDelPanelDeTitulo()
    {
        using FrmMateriaPrima form = CrearFormulario();
        form.CreateControl();
        form.PerformLayout();

        Panel titulo = (Panel)form.Controls.Find("panel1", true).Single();
        ToolStrip barra = (ToolStrip)form.Controls.Find("toolStrip1", true).Single();

        // Dock garantiza que la barra va debajo del titulo y no lo tapa.
        barra.Top.Should().BeGreaterThanOrEqualTo(titulo.Bottom);
        barra.Bounds.IntersectsWith(titulo.Bounds).Should().BeFalse();
    }

    [Fact]
    public void BarraDeHerramientas_CabeEnElAnchoMinimoDelFormulario()
    {
        using FrmMateriaPrima form = CrearFormulario();
        form.CreateControl();
        form.PerformLayout();

        ToolStrip barra = (ToolStrip)form.Controls.Find("toolStrip1", true).Single();

        // Si el minimo del formulario no cubre el ancho real de la barra, el ultimo
        // boton queda cortado: por eso el constructor lo recalcula con PreferredSize.
        form.MinimumSize.Width.Should().BeGreaterThanOrEqualTo(barra.PreferredSize.Width);
    }

    [Fact]
    public void Grid_OcupaTodoElPanelCentralYSeEstira()
    {
        using FrmMateriaPrima form = CrearFormulario();
        form.CreateControl();
        form.PerformLayout();

        Panel detalle = (Panel)form.Controls.Find("panelDetalle", true).Single();
        DataGridView grid = (DataGridView)form.Controls.Find("GridItems", true).Single();

        grid.Dock.Should().Be(DockStyle.Fill);
        grid.Size.Should().Be(detalle.ClientSize);
        // Sin Fill las columnas quedaban con ancho fijo y aparecian huecos al ensanchar.
        grid.AutoSizeColumnsMode.Should().Be(DataGridViewAutoSizeColumnsMode.Fill);
    }

    [Fact]
    public void Estilos_PaletaVerdeYControlesPlanosComoEnPedidos()
    {
        using FrmMateriaPrima form = CrearFormulario();

        Panel titulo = (Panel)form.Controls.Find("panel1", true).Single();
        titulo.BackColor.Should().Be(Color.FromArgb(190, 225, 150));

        Panel contador = (Panel)form.Controls.Find("panelContador", true).Single();
        contador.BackColor.Should().Be(Color.FromArgb(100, 170, 80));

        ToolStrip barra = (ToolStrip)form.Controls.Find("toolStrip1", true).Single();
        barra.GripStyle.Should().Be(ToolStripGripStyle.Hidden);
        barra.RenderMode.Should().Be(ToolStripRenderMode.Professional);
        foreach (ToolStripItem item in barra.Items)
        {
            ToolStripButton boton = Assert.IsType<ToolStripButton>(item);
            boton.BackColor.Should().Be(Color.Transparent);

            // Los botones se auto-ajustan al icono y al texto: es lo que evita que
            // "Exportar" o "Imprimir" queden cortados dentro del boton.
            boton.AutoSize.Should().BeTrue();
            int anchoDelTexto = TextRenderer.MeasureText(boton.Text ?? string.Empty, boton.Font).Width;
            boton.Width.Should().BeGreaterThanOrEqualTo(anchoDelTexto + barra.ImageScalingSize.Width);
        }

        foreach (string nombre in new[] { "btn_OrdenBuscar", "btn_ProvBuscar", "btn_TransportBuscar", "btn_RecepBuscar" })
        {
            Button boton = (Button)form.Controls.Find(nombre, true).Single();
            boton.FlatStyle.Should().Be(FlatStyle.Flat);
        }
    }

    [Fact]
    public void Formulario_NoLlevaElTemaOscuroDeMain()
    {
        using FrmMateriaPrima form = CrearFormulario();

        form.Should().BeAssignableTo<Ritrama2025.Helpers.IFormTemaClaro>();

form.ReaplicarTema();

        // SunnyUI vuelve a pintar el fondo con el verde claro de su tema: lo que no puede
        // pasar es que Main le imponga el tema oscuro (37,37,42) encima.
        form.BackColor.Should().NotBe(Color.FromArgb(37, 37, 42));
        form.TitleForeColor.Should().Be(Color.White);
    }

    [Fact]
    public async Task InitializeAsync_CargaCabeceraYOrdenaDeMayorAMenor()
    {
        using FrmMateriaPrima form = CrearFormulario(DatasetDePrueba());

        await form.InitializeAsync();

        Label contador = (Label)form.Controls.Find("label_counter_rows", true).Single();
        contador.Text.Should().Be("Registros: 2");

        // numero es texto en la base: el formulario agrega la columna numerica para que la
        // ultima orden quede primero y no "9" antes que "10". La cabecera visible es la
        // orden 10, asi que su unico renglon es ROLL-10.
        DataGridView grid = (DataGridView)form.Controls.Find("GridItems", true).Single();
        Assert.Single(grid.Rows);
        grid.Rows[0].Cells["rollid"].Value.Should().Be("ROLL-10");
    }

    [Fact]
    public async Task InitializeAsync_CreaColumnasDelDetalleConNombreIgualAlDato()
    {
        using FrmMateriaPrima form = CrearFormulario(DatasetDePrueba());

        await form.InitializeAsync();

        DataGridView grid = (DataGridView)form.Controls.Find("GridItems", true).Single();
        Assert.NotEmpty(grid.Columns);
        // El codigo lee Cells["empalme"], Cells["num_paleta"], etc.: si el Name difiere del
        // DataPropertyName esas celdas vuelven null.
        foreach (DataGridViewColumn columna in grid.Columns)
        {
            Assert.Equal(columna.Name, columna.DataPropertyName);
        }

        List<string> nombres = grid.Columns.Cast<DataGridViewColumn>().Select(c => c.Name).ToList();
        nombres.Should().Contain(new[] { "product_id", "rollid", "empalme", "num_paleta", "factura" });
        Assert.Single(grid.Rows);
    }

    [Fact]
    public async Task InitializeAsync_SinTablaDeOrdenesNoDejaElFormularioEnBlanco()
    {
        // DataSet sin la tabla de ordenes: es lo que devuelve el servicio cuando la carga
        // falla (sin conexion, tabla inexistente). Antes el formulario seguia enlazando y
        // reventaba con "DataMember 'FK_MASTER_DETAILS' no se encontro en DataSource".
        DataSet sinOrdenes = new DataSet();
        sinOrdenes.Tables.Add(new DataTable("DtProvider"));
        using FrmMateriaPrima form = CrearFormulario(sinOrdenes);

        await form.InitializeAsync();

        Label contador = (Label)form.Controls.Find("label_counter_rows", true).Single();
        contador.Text.Should().Be("Registros: sin datos");
    }
}