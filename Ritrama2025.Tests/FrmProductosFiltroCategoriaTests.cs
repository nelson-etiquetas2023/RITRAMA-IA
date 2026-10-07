using System.Data;
using System.Reflection;
using System.Xml.Linq;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.ProductsService;
using Ritrama2025.Services.ExportData;
using Microsoft.Reporting.WinForms;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.InventarioService;
using Sunny.UI;
using Xunit;

namespace Ritrama2025.Tests;
/// Pruebas del filtro por categoria de la lista de productos: los cuatro radios que viven
/// debajo del buscador, y su combinacion con el texto de busqueda.
/// No toca base de datos: monta el formulario con un stub de IProductsService.
/// Vive en un fichero propio, y no en FrmProductosLayoutTests, para no mezclarse con las
/// pruebas de maquetacion del formulario.
/// </summary>

[Collection("Productos")]
public class FrmProductosFiltroCategoriaTests
{
    /// <summary>Servicio minimo: devuelve el catalogo que se le pase y nada mas.</summary>
    private sealed class ProductsServiceStub(IReadOnlyList<Product> catalogo) : IProductsService
    {
        public Task<DataSet> Load(CancellationToken cancellationToken = default)
            => Task.FromResult(new DataSet());

        public Task<bool> Add(Product producto) => Task.FromResult(true);

        public Task<bool> Update(Product producto) => Task.FromResult(true);

        public bool Anular(string IdProduct) => true;

        public bool ValidProductid(string id) => false;

        public Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(false));

        public Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyList<Product>>.Success(catalogo));

        public Result ValidateProduct(Product producto) => Result.Success();
    }

    /// <summary>
    /// Stub de la exportacion: graba lo que se le pasa, para poder comprobar el contenido de la
    /// hoja sin escribir un fichero de verdad. Devuelve false (fallo) por defecto, que es el
    /// caso mas proximo a la realidad; da igual para los avisos, porque el formulario no
    /// muestra mensaje de exito en ninguno de los dos casos: solo avisa si el servicio lanza.
    /// </summary>
    private sealed class ExportDataServiceStub : IExportDataService
    {
        /// <summary>Filas de la ultima exportacion (vacias si no se exporto nada).</summary>
        public List<ProductoExportado> Exportado { get; } = new();

        /// <summary>Numero de veces que se pidio exportar.</summary>
        public int Llamadas { get; private set; }

        /// <summary>Nombre de fichero de la ultima exportacion.</summary>
        public string? Fichero { get; private set; }

        /// <summary>Lo que devuelve la exportacion.</summary>
        public bool Resultado { get; set; }

        /// <summary>Si se pone, el servicio lanza al exportar, para probar el camino de error.</summary>
        public Exception? ErrorAlExportar { get; set; }

        public bool ExportToExcel<T>(List<T> data, string FileName)
        {
            Llamadas++;
            Fichero = FileName;
            Exportado.Clear();
            Exportado.AddRange(data.OfType<ProductoExportado>());

            if (ErrorAlExportar is not null)
            {
                throw ErrorAlExportar;
            }

            return Resultado;
        }

        public bool ExportToExcelProducts<T>(List<T> data, string FileName)
            => ExportToExcel(data, FileName);

        public bool ExportTxtFormatRollosCortados(DataRow[] rollos, bool solo_rc, string? fecha_produccion, string? fecha_registro, bool openNotePad) => true;

        public bool ExportTxtFormatMasterRePrintLabel(ProductMAP master, bool openNotePad) => true;
    }

    /// <summary>
    /// Formulario con los dialogos sustituidos por capturas: un MessageBox real en una prueba
    /// la dejaria colgada a la espera de que alguien pulse un boton.
    /// </summary>
    private sealed class FrmProductosSinDialogos(IProductsService productos, IExportDataService exporta, IReportsService reportes, IConfiguration config, IConsecutivosService consecutivos, IProductsImportService importa, IInventarioService inventario)
        : FrmProductos(productos, exporta, reportes, config, consecutivos, importa, inventario)
    {
        public List<string> Avisos { get; } = new();

        protected override void MostrarAviso(string mensaje) => Avisos.Add(mensaje);

        protected override bool ConfirmarDescarte(string pregunta) => true;
    }

    private static FrmProductosSinDialogos CrearFormulario(IReadOnlyList<Product> catalogo)
        => CrearFormulario(catalogo, out _);

    /// <summary>Stub del visor de reportes: graba la llamada sin abrir ninguna ventana.</summary>
    private sealed class ReportsServiceStub : IReportsService
    {
        /// <summary>Numero de veces que se pidio el reporte.</summary>
        public int Llamadas { get; private set; }

        /// <summary>Titulo y nombre de fichero de la ultima peticion.</summary>
        public string? Titulo { get; private set; }

        public string? Fichero { get; private set; }

        public void Reporte_Productos(Form form, string Report_Title, string Report_Name)
        {
            Llamadas++;
            Titulo = Report_Title;
            Fichero = Report_Name;
        }

        public void Reporte_Orden_Corte(string orden, Form form, string ReportName, string TitleReport) { }

        public void Reporte_Desperdicios(string orden, Form form, string ReportName, string TitleReport) { }

        public void Reporte_Orden_MatPrima(string orden, Form form, string ReportName, string TitleReport) { }

        public void ReporteConduce_conPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

        public void ReporteCondece_sinPrecio(string conduce, Form form, string ReportName, string TitleReport) { }

        public void Reporte_PackingList(string conduce, Form form) { }

        public void Reporte_DetallePaleta(string conduce, Form form) { }

        public void Reporte_InventarioRollosCortados(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_InventarioMaster(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Clientes(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Proveedores(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Vendedores(Form form, string Report_Title, string Report_Name) { }

        public void Reporte_Usuarios(Form form, string Report_Title, string Report_Name) { }
    }

    /// <summary>Stub para el servicio de consecutivos de productos (inicia en 1).</summary>
    private sealed class ConsecutivosServiceStub : IConsecutivosService
    {
        private int _valor; // Se incrementa a 1 en la primera llamada

        public int GetAndIncrementConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecOCTransactional(Microsoft.Data.SqlClient.SqlConnection conn, Microsoft.Data.SqlClient.SqlTransaction transaction)
            => throw new NotImplementedException();

        public int BuscarUniqueCodeConsec() => throw new NotImplementedException();

        public int BuscarConsecOC() => throw new NotImplementedException();

        public int GetAndIncrementConsecProducto() => ++_valor;

        public int GetAndIncrementConsecCliente() => throw new NotImplementedException();

        public int GetAndIncrementConsecProveedor() => throw new NotImplementedException();

        public int GetAndIncrementConsecVendedor() => throw new NotImplementedException();

        public bool UpdateConsecOC(string consec) => throw new NotImplementedException();

        public bool UpdateUniqueCodeBD(string consec) => throw new NotImplementedException();
    }

    private static FrmProductosSinDialogos CrearFormulario(
        IReadOnlyList<Product> catalogo,
        out ExportDataServiceStub exporta)
    {
        exporta = new ExportDataServiceStub();
        return new FrmProductosSinDialogos(new ProductsServiceStub(catalogo), exporta, new ReportsServiceStub(), new ConfigurationBuilder().Build(), new ConsecutivosServiceStub(), new Stubs.ProductsImportServiceStub(), new Stubs.InventarioServiceStub());
    }

    /// <summary>Los radios del filtro, en el orden en que se pintan. El primero es "Todos".</summary>
    private static UIRadioButton[] RadiosFiltro(FrmProductos form) =>
    [
        (UIRadioButton)form.Controls.Find("rbFiltroTodos", true).Single(),
        (UIRadioButton)form.Controls.Find("rbFiltroMaster", true).Single(),
        (UIRadioButton)form.Controls.Find("rbFiltroRolloCortado", true).Single(),
        (UIRadioButton)form.Controls.Find("rbFiltroHojas", true).Single(),
        (UIRadioButton)form.Controls.Find("rbFiltroGraphics", true).Single()
    ];

    /// <summary>Producto de un solo tipo, para el filtro de categoria.</summary>
    private static Product ProductoDeCategoria(string id, bool master = false, bool rolloCortado = false,
                                               bool hoja = false, bool graphics = false) => new()
    {
        Product_id = id,
        Product_Name = "Producto " + id,
        Product_Description = string.Empty,
        Referencia = string.Empty,
        Codigo_Barra = string.Empty,
        Master = master,
        RolloCortado = rolloCortado,
        Hoja = hoja,
        Graphics = graphics
    };

    /// <summary>Catalogo con un producto de cada tipo, para comprobar que el filtro discrimina.</summary>
    private static IReadOnlyList<Product> CatalogoPorTipos() =>
    [
        ProductoDeCategoria("M-001", master: true),
        ProductoDeCategoria("R-001", rolloCortado: true),
        ProductoDeCategoria("H-001", hoja: true),
        ProductoDeCategoria("G-001", graphics: true)
    ];

    private static DataGridView Grid(FrmProductos form)
        => (DataGridView)form.Controls.Find("gridProductos", true).Single();

    /// <summary>
    /// Simula un clic en uno de los radios. El UIRadioButton de SunnyUI no trae PerformClick
    /// (solo el evento Click), asi que se marca el radio como haria el clic y se invoca el
    /// manejador, que es quien aplica el filtro.
    /// </summary>
    private static void ClicEnFiltro(FrmProductos form, UIRadioButton radio)
    {
        radio.Checked = true;

        MethodInfo manejador = typeof(FrmProductos).GetMethod(
            "FiltroCategoria_Click", BindingFlags.Instance | BindingFlags.NonPublic)!;

        manejador.Invoke(form, [radio, EventArgs.Empty]);
    }

    [Fact]
    public void Filtro_HayCincoRadiosEnLaCajaDelBuscadorYTodosVieneMarcado()
    {
        using FrmProductos form = CrearFormulario(CatalogoPorTipos());

        Panel buscador = (Panel)form.Controls.Find("pnlBuscador", true).Single();
        UIRadioButton[] radios = RadiosFiltro(form);

        radios.Select(r => r.Text)
            .Should().Equal("Todos", "Master", "Rollos Cortados", "Hojas", "Graphics");
        radios.Should().OnlyContain(r => r.Parent!.Parent == buscador,
            "los cinco viven dentro de la caja del buscador");
        radios[0].Checked.Should().BeTrue("al abrir el modulo se ve el catalogo entero");
        radios.Skip(1).Should().OnlyContain(r => !r.Checked, "solo uno puede estar marcado");
    }

    [Fact]
    public void Filtro_AlAbrirElModuloLosRadiosEstanHabilitados()
    {
        using FrmProductos form = CrearFormulario(CatalogoPorTipos());

        // Enabled si es observable sin mostrar el formulario (a diferencia de Visible), asi que
        // esta comprobacion si se puede hacer. Es la que hacia falta: con la condicion
        // invertida, el filtro nacia deshabilitado y el clic no hacia nada.
        RadiosFiltro(form).Should().OnlyContain(r => r.Enabled,
            "el filtro se usa en consulta, asi que al abrir el modulo tiene que estar disponible");
    }

    [Fact]
    public async Task Filtro_AlEscribirUnProductoLosRadiosSeBloquean()
    {
        // Con permiso de escritura: el formulario solo deja entrar a Nuevo/Editar si
        // PermisoHelper lo da, y hace falta para comprobar el bloqueo del filtro.
        using SesionDeEscritura sesion = new();

        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();
            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

            barra.Items["btnNuevoProducto"]!.PerformClick();

            RadiosFiltro(form).Should().OnlyContain(r => !r.Enabled,
                "mientras se esta escribiendo el filtro se bloquea, para que el listado no cambie debajo");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Theory]
    [InlineData("resma")]  // el nombre canonico de la columna Tipo del grid
    [InlineData("Resma")]
    [InlineData("RESMAS")] // en mayusculas y en plural
    [InlineData("resmas")]
    [InlineData("hojas")]  // como lo llama el filtro de radios
    [InlineData("Hoja")]
    public async Task Buscador_EncuentraPorElTipoDeProductoSinImportarComoSeEscriba(string texto)
    {
        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();

            DataGridView grid = Grid(form);
            UITextBox cuadro = (UITextBox)form.Controls.Find("txtSearch", true).Single();

            cuadro.Text = texto;

            grid.Rows.Count.Should().Be(1, $"buscando '{texto}' tiene que salir la hoja");
            grid.Rows[0].Cells[0].Value.Should().Be("H-001");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Buscador_ElTipoSeEncadenaConElFiltroDeCategoria()
    {
        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();

            DataGridView grid = Grid(form);
            UITextBox cuadro = (UITextBox)form.Controls.Find("txtSearch", true).Single();

            cuadro.Text = "master";
            grid.Rows.Count.Should().Be(1, "solo hay un master en el catalogo");
            grid.Rows[0].Cells[0].Value.Should().Be("M-001");

            // Se limpia el texto para comprobar el filtro de categoria por separado.
            cuadro.Text = string.Empty;
            grid.Rows.Count.Should().Be(4);

            ClicEnFiltro(form, RadiosFiltro(form)[4]);
            grid.Rows.Count.Should().Be(1, "el filtro deja solo el Graphics");

            // El tipo tambien se busca, pero los dos filtros van encadenados: con el filtro en
            // Graphics, buscar una hoja no puede devolver ninguna.
            cuadro.Text = "resma";
            grid.Rows.Count.Should().Be(0,
                "y buscar una hoja dentro de Graphics no encuentra nada");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Guardar_ConElBuscadorPuestoEnElTipo_NoSeLimpiaElFiltro()
    {
        // Al guardar, el formulario comprueba si lo guardado sigue entrando en el filtro y, si
        // no, limpia el buscador para que la fila nueva se vea. Esa comprobacion tiene que usar
        // la MISMA logica que el filtro: si no, buscar "resma" y guardar una hoja borraria el
        // texto y pareceria que no se habia guardado nada.
        using SesionDeEscritura sesion = new();

        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();

            UITextBox cuadro = (UITextBox)form.Controls.Find("txtSearch", true).Single();
            cuadro.Text = "resma";

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            barra.Items["btnNuevoProducto"]!.PerformClick();

            // Alta de un producto que es una hoja, que es justo lo que pide el buscador.
            ((Control)form.Controls.Find("txtDetCodigoRitrama", true).Single()).Text = "H-002";
            ((Control)form.Controls.Find("txtDetNombre", true).Single()).Text = "Otra hoja";
            ((UIRadioButton)form.Controls.Find("rbTipoHojas", true).Single()).Checked = true;

            barra.Items["btnGuardarProducto"]!.PerformClick();

            cuadro.Text.Should().Be("resma",
                "la hoja recien guardada entra en el filtro, asi que el buscador no se toca");
        }
        finally
        {
            form.Dispose();
        }
    }

    /// <summary>
    /// Sesion con permiso de escritura para Productos. PermisoHelper devuelve false sin usuario,
    /// y sin el permiso los botones Nuevo/Editar no abren nada. Se restaura al terminar.
    /// </summary>
    private sealed class SesionDeEscritura : IDisposable
    {
        private readonly Usuario? _usuarioAnterior = Ritrama2025.Helpers.SesionActual.Usuario;
        private readonly List<string> _permisosAnteriores = [.. Ritrama2025.Helpers.SesionActual.Permisos];

        public SesionDeEscritura()
        {
            Ritrama2025.Helpers.SesionActual.Usuario =
                new Usuario { UserId = 1, Username = "prueba" };
            Ritrama2025.Helpers.SesionActual.Permisos.AddRange(["Productos:Ver", "Productos:Crear"]);
        }

        public void Dispose()
        {
            Ritrama2025.Helpers.SesionActual.Usuario = _usuarioAnterior;
            Ritrama2025.Helpers.SesionActual.Permisos.Clear();
            Ritrama2025.Helpers.SesionActual.Permisos.AddRange(_permisosAnteriores);
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Hoja de Excel con todos los productos
    // ─────────────────────────────────────────────────────────────────

    /// <summary>Producto con todos los datos puestos, para comprobar el mapeo al Excel.</summary>
    private static Product ProductoParaExcel(string id, bool anulado = false, bool hoja = false) => new()
    {
        Product_id = id,
        Product_Name = "Producto " + id,
        Product_Description = "Descripcion de " + id,
        Referencia = "REF-" + id,
        Codigo_Barra = "750" + id,
        Precio = 125.5m,
        Costo = 98.25m,
        Ratio = 1.25m,
        Hoja = hoja,
        Anulado = anulado
    };

    /// <summary>Pulsa el boton Exportar de la barra.</summary>
    private static void ClicEnExportar(FrmProductos form)
        => ((ToolStripButton)((ToolStrip)form.Controls.Find("barraHerramientas", true).Single())
            .Items["btnExportarProducto"]!).PerformClick();

    [Fact]
    public async Task Exportar_CreaLaHojaConTodosLosProductosDelCatalogo()
    {
        using SesionDeEscritura sesion = new();
        FrmProductosSinDialogos form = CrearFormulario(
            [ProductoParaExcel("M-001"), ProductoParaExcel("H-001", hoja: true), ProductoParaExcel("R-001")],
            out ExportDataServiceStub exporta);
        try
        {
            await form.InitializeAsync();
            exporta.Resultado = true;

            ClicEnExportar(form);

            exporta.Llamadas.Should().Be(1);
            exporta.Fichero.Should().Be("Productos.xlsx");

            // La lista va entre corchetes a proposito: Equal(...) con params string[] se traga el
            // texto del "porque" como un elemento mas de la lista esperada.
            exporta.Exportado.Select(p => p.Codigo)
                .Should().Equal(["M-001", "H-001", "R-001"],
                    "van TODOS los productos del catalogo, no solo los que se ven filtrados");

            // Al exportar no salen avisos de exito: el propio ExportToExcel abre el fichero,
            // que ya es la confirmacion. Solo se avisa cuando algo falla.
            form.Avisos.Should().BeEmpty();
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_ElFiltroDePantallaNoCambiaLoQueVaAlFichero()
    {
        using SesionDeEscritura sesion = new();
        FrmProductosSinDialogos form = CrearFormulario(
            [ProductoParaExcel("M-001"), ProductoParaExcel("H-001", hoja: true), ProductoParaExcel("R-001")],
            out ExportDataServiceStub exporta);
        try
        {
            await form.InitializeAsync();
            exporta.Resultado = true;

            ClicEnFiltro(form, RadiosFiltro(form)[3]);
            Grid(form).Rows.Count.Should().Be(1, "en pantalla solo sale la hoja");

            ClicEnExportar(form);

            exporta.Exportado.Should().HaveCount(3,
                "el fichero es el catalogo completo: filtrar en pantalla no lo recorta");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_ElTipoVaResueltoYElEstadoEnTexto()
    {
        using SesionDeEscritura sesion = new();
        FrmProductosSinDialogos form = CrearFormulario(
            [ProductoParaExcel("H-001", hoja: true), ProductoParaExcel("X-001", anulado: true)],
            out ExportDataServiceStub exporta);
        try
        {
            await form.InitializeAsync();
            exporta.Resultado = true;

            ClicEnExportar(form);

            ProductoExportado hoja = exporta.Exportado.Single(p => p.Codigo == "H-001");
            hoja.Tipo.Should().Be("Resma", "el tipo sale ya resuelto, no como los cuatro bits sueltos");
            hoja.Estado.Should().Be("Activo");
            hoja.Precio.Should().Be(125.5m);
            hoja.Costo.Should().Be(98.25m);
            hoja.Descripcion.Should().Be("Descripcion de H-001");

            ProductoExportado apagado = exporta.Exportado.Single(p => p.Codigo == "X-001");
            apagado.Estado.Should().Be("Desactivado", "en vez de un anulado = 1 que hay que traducir");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_SinProductosNoIntentaExportarNiDiceNada()
    {
        using SesionDeEscritura sesion = new();
        FrmProductosSinDialogos form = CrearFormulario([], out ExportDataServiceStub exporta);
        try
        {
            await form.InitializeAsync();

            ClicEnExportar(form);

            exporta.Llamadas.Should().Be(0, "ExportToExcel lanza si la lista va vacia, asi que no se llama");
            form.Avisos.Should().BeEmpty(
                "no hay filas que exportar: no es un error y en el listado ya se ve, asi que no se avisa");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_SinPermisoParaVer_NoExporta()
    {
        // Sin sesion abierta, PermisoHelper puede ver = false.
        FrmProductosSinDialogos form = CrearFormulario(
            [ProductoParaExcel("M-001")], out ExportDataServiceStub exporta);
        try
        {
            await form.InitializeAsync();

            ClicEnExportar(form);

            exporta.Llamadas.Should().Be(0, "sin permiso no se saca nada del catalogo");
            form.Avisos.Should().ContainSingle();
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Exportar_SiElServicioFallaSeAvisaYNoSeDaPorHecho()
    {
        using SesionDeEscritura sesion = new();
        FrmProductosSinDialogos form = CrearFormulario(
            [ProductoParaExcel("M-001")], out ExportDataServiceStub exporta);
        try
        {
            await form.InitializeAsync();
            exporta.ErrorAlExportar = new IOException("el disco esta lleno");

            ClicEnExportar(form);

            form.Avisos.Should().ContainSingle();
            form.Avisos[0].Should().Contain("No se pudo crear la hoja de Excel");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public void ReporteProductos_ElRdlcLoAceptaElVisorYTraeLasSieteColumnasPedidas()
    {
        // Esta es la comprobacion que de verdad importa: la carga la hace el ReportViewer, no la
        // aplicacion. Si el .rdlc no cuadra, LoadReportDefinition lanza y el reporte no abriria
        // nunca, aunque el XML estuviese bien formado.
        string ruta = Path.Combine(AppContext.BaseDirectory, "Reports", "Products", "Report_Productos.rdlc");
        File.Exists(ruta).Should().BeTrue($"el .rdlc tiene que copiarse a la salida: {ruta}");

        using LocalReport informe = new();
        using FileStream flujo = File.OpenRead(ruta);
        informe.LoadReportDefinition(flujo);

        // El nombre del DataSource es el contrato con ReportsService.Reporte_Productos: si uno de
        // los dos cambia, el visor abre el reporte vacio sin dar ningun error.
        informe.GetDataSourceNames().Should().Contain("DsProductos");

        // Campos y columnas se sacan del XML: la API del visor no los expone.
        XDocument xml = XDocument.Load(ruta);
        XNamespace r = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

        xml.Descendants(r + "Field").Select(f => f.Attribute("Name")!.Value)
            .Should().Equal(["Codigo", "Nombre", "Tipo", "Precio", "Costo", "Ratio", "Estado"]);

        // Una celda de cabecera y una de detalle por campo: si no cuadran, el Tablix se rompe al
        // procesar y el error sale en pantalla, no en el codigo.
        xml.Descendants(r + "TablixColumn").Count().Should().Be(7);
        xml.Descendants(r + "TablixCell").Count().Should().Be(14, "7 celdas de cabecera + 7 de detalle");
    }

    [Fact]
    public void LosBotonesDeLaBarraTienenIconoYSeVen()
    {
        using FrmProductos form = CrearFormulario(CatalogoPorTipos());
        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

        // btnExportarProducto es el boton que exporta a Excel y btnImportarCatalogo el que
        // importa productos desde una hoja: los dos llevan icono de Excel.
        string[] botones = ["btnNuevoProducto", "btnEditarProducto", "btnExportarProducto", "btnImportarCatalogo", "btnReporteProducto"];

        foreach (string nombre in botones)
        {
            ToolStripButton boton = (ToolStripButton)barra.Items[nombre]!;
            boton.Should().NotBeNull("el boton {0} deberia estar en la barra", nombre);

            // Que el recurso exista no basta: un PNG transparente o en blanco se ve como si no
            // hubiera icono, que es justo lo que paso con excel_16px. Se cuentan los pixeles
            // con contenido para que un icono vacio no pase por bueno.
            Image icono = boton.Image;
            icono.Should().NotBeNull("el boton {0} necesita un icono", nombre);

            int conContenido = 0;
            using Bitmap bmp = new(icono!);
            for (int y = 0; y < bmp.Height; y++)
            {
                for (int x = 0; x < bmp.Width; x++)
                {
                    if (bmp.GetPixel(x, y).A > 20)
                    {
                        conContenido++;
                    }
                }
            }

            conContenido.Should().BeGreaterThan(50,
                "el icono del boton {0} esta en blanco o transparente, se veria como si faltara", nombre);
        }

        // Los iconos van a tamano fijo: si escalan, 16 px se ven como un borron dentro de un boton de 36.
        barra.Items["btnReporteProducto"]!.ImageScaling.Should().Be(ToolStripItemImageScaling.None);
    }

    [Fact]
    public void ReporteProductos_EsTabularConBordesYTitulos()
    {
        // El formato se comprueba sobre el XML porque la API del visor no lo expone. Si alguien
        // regenera el .rdlc desde Visual Studio y pierde los bordes, esto avisa.
        string ruta = Path.Combine(AppContext.BaseDirectory, "Reports", "Products", "Report_Productos.rdlc");
        XDocument xml = XDocument.Load(ruta);
        XNamespace r = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

        // Una celda de cabecera y una de detalle por cada uno de los 7 campos, y todas con borde.
        XElement[] celdas = xml.Descendants(r + "TablixCell").ToArray();
        celdas.Should().HaveCount(14, "7 celdas de cabecera + 7 de detalle");

        foreach (XElement celda in celdas)
        {
            celda.Descendants(r + "Border").Should().ContainSingle(
                "cada campo de la tabla necesita su borde, si no sale una lista sin cuadrar");
        }

        // Los dos titulos, cada uno en su linea, y por encima de la tabla.
        string[] valores = xml.Descendants(r + "Value").Select(v => v.Value).ToArray();
        valores.Should().Contain("SISTEMA DE RITRAMA");
        valores.Should().Contain("CATALOGO DE PRODUCTOS");
        valores.IndexOf("SISTEMA DE RITRAMA").Should().BeLessThan(valores.IndexOf("CATALOGO DE PRODUCTOS"),
            "SISTEMA DE RITRAMA va en la linea de arriba");
        valores.IndexOf("CATALOGO DE PRODUCTOS").Should().BeLessThan(valores.IndexOf("Codigo"),
            "los titulos van antes de la tabla, no despues");
    }

    [Fact]
    public void ReporteProductos_LosElementosTienenPosicionFijaYCabenEnLaPagina()
    {
        // Sin <Top> el visor reparte el cuerpo por su cuenta y la tabla se va hacia abajo,
        // dejando un hueco entre el titulo y los datos. Por eso los tres llevan posicion fija.
        string ruta = Path.Combine(AppContext.BaseDirectory, "Reports", "Products", "Report_Productos.rdlc");
        XDocument xml = XDocument.Load(ruta);
        XNamespace r = "http://schemas.microsoft.com/sqlserver/reporting/2016/01/reportdefinition";

        double Pulgadas(XElement item, string nombre) =>
            double.Parse(item.Elements(r + nombre).Single().Value.TrimEnd("in".ToCharArray()),
                System.Globalization.CultureInfo.InvariantCulture);

        XElement titulo1 = xml.Descendants(r + "Textbox").Single(t => (string)t.Attribute("Name") == "TituloSistema");
        XElement titulo2 = xml.Descendants(r + "Textbox").Single(t => (string)t.Attribute("Name") == "TituloCatalogo");
        XElement tabla = xml.Descendants(r + "Tablix").Single();

        Pulgadas(titulo1, "Top").Should().Be(0, "el titulo arranca pegado al margen superior, sin espacio vacio");
        Pulgadas(titulo1, "Left").Should().Be(0);

        // El subtitulo va justo debajo del titulo, y la tabla justo debajo del subtitulo.
        Pulgadas(titulo2, "Top").Should().Be(Pulgadas(titulo1, "Top") + Pulgadas(titulo1, "Height"));
        Pulgadas(tabla, "Top").Should().Be(Pulgadas(titulo2, "Top") + Pulgadas(titulo2, "Height"),
            "la tabla continua despues del subtitulo, sin hueco en medio");

        // La tabla no puede ser mas ancha que el papel: 8.5in menos los margenes de 0.5in.
        double anchoPagina = 8.5;
        double margenes = 0.5;
        double anchoUtil = anchoPagina - (margenes * 2);

        double anchoTabla = tabla.Descendants(r + "TablixColumn")
            .Sum(c => double.Parse(c.Element(r + "Width")!.Value.TrimEnd("in".ToCharArray()),
                System.Globalization.CultureInfo.InvariantCulture));

        anchoTabla.Should().BeLessThanOrEqualTo(anchoUtil,
            "si las columnas suman mas que el ancho util, la ultima se sale de la pagina");
    }

    [Fact]
    public void ReporteProductos_LaConsultaTraeLasColumnasQuePideElRdlc()
    {
        // Los alias de la consulta y los Fields del .rdlc tienen que casar uno a uno; si no, el
        // visor abre la hoja en blanco sin avisar.
        string sql = R.QUERY.PRODUCTS.SQL_QUERY_REPORTE_PRODUCTOS;
        string[] campos = ["Codigo", "Nombre", "Tipo", "Precio", "Costo", "Ratio", "Estado"];

        foreach (string campo in campos)
        {
            sql.Should().Contain($"AS {campo}", $"el .rdlc espera el campo {campo}");
        }

        // Sin filtro de anulado: el reporte es el catalogo completo, con los desactivados
        // marcados en la columna Estado.
        sql.Should().NotContain("WHERE");
        sql.Should().Contain("Desactivado").And.Contain("Activo");
    }

    [Fact]
    public void BarraDeAcciones_TieneNuevoEditarExportarEnEseOrden()
    {
        using FrmProductos form = CrearFormulario([ProductoParaExcel("M-001")]);

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

        ToolStripButton exportar = (ToolStripButton)barra.Items["btnExportarProducto"]!;
        exportar.Text.Should().Be("Exportar");
        exportar.ToolTipText.Should().Contain("Excel");
        exportar.Enabled.Should().BeTrue("exportar no depende de que haya algo seleccionado");
        exportar.Image.Should().NotBeNull("el boton lleva el icono de Excel");

        // El icono tiene que caber en el boton: con ImageScaling = None se dibuja a su tamano
        // natural, y si fuera mas alto que el boton se saldría de la barra.
        exportar.Image!.Height.Should().BeLessThanOrEqualTo(exportar.Size.Height,
            "un icono mas alto que el boton se desborda al pintarse");

        // Es la tercera opcion de la barra, detras de Nuevo y Editar; la cuarta es Importar.
        barra.Items.OfType<ToolStripButton>().Select(b => b.Text)
            .Take(4).Should().Equal(["Nuevo", "Editar", "Exportar", "Importar"]);

        // El reporte es la cuarta y se oculta mientras se escribe un producto.
        ToolStripButton reporte = (ToolStripButton)barra.Items["btnReporteProducto"]!;
        reporte.Text.Should().Be("Reporte");
        reporte.Image.Should().NotBeNull("el boton lleva su icono de informe");

        // El icono tiene que caber en el boton: con ImageScaling = None se dibuja a su tamano
        // natural, y si fuera mas alto que el boton se saldria de la barra.
        reporte.Image!.Height.Should().BeLessThanOrEqualTo(reporte.Size.Height,
            "un icono mas alto que el boton se desborda al pintarse");
    }

    [Fact]
    public void Filtro_LosCincoRadiosCabenEnUnaSolaFila()
    {
        using FrmProductos form = CrearFormulario(CatalogoPorTipos());

        FlowLayoutPanel tira = (FlowLayoutPanel)form.Controls.Find("pnlFiltroTipo", true).Single();
        UIRadioButton[] radios = RadiosFiltro(form);

        // Con AutoSize manda el ancho del texto, asi que se mide de verdad en vez de fiarse de
        // los Size del disenador: si no caben, la tira envuelve y queda en dos filas.
        int necesario = radios.Sum(r => r.GetPreferredSize(Size.Empty).Width + r.Margin.Horizontal);

        necesario.Should().BeLessThanOrEqualTo(tira.ClientSize.Width,
            $"los cinco radios necesitan {necesario}px y la tira tiene {tira.ClientSize.Width}px");
        radios.Select(r => r.Top).Should().OnlyContain(y => y == radios[0].Top,
            "y tienen que quedar todos en la misma fila");
    }

    [Fact]
    public async Task Filtro_MarcarUnTipoDejaSoloLosProductosDeEseTipo()
    {
        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();
            Grid(form).Rows.Count.Should().Be(4, "con Todos marcado se ve el catalogo entero");

            ClicEnFiltro(form, RadiosFiltro(form)[3]);

            Grid(form).Rows.Count.Should().Be(1);
            Grid(form).Rows[0].Cells[0].Value.Should().Be("H-001", "solo queda la hoja");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Filtro_LaBusquedaSeAplicaEncimaDelFiltroDeCategoria()
    {
        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();

            DataGridView grid = Grid(form);
            UITextBox cuadro = (UITextBox)form.Controls.Find("txtSearch", true).Single();

            ClicEnFiltro(form, RadiosFiltro(form)[1]);
            grid.Rows.Count.Should().Be(1, "queda solo el master");

            // El texto se filtra sobre lo que ya paso el filtro de categoria, no sobre todo el
            // catalogo: por eso buscar una hoja con el filtro en Master no la encuentra.
            cuadro.Text = "H-001";
            grid.Rows.Count.Should().Be(0, "la hoja no es un Master, asi que la busqueda no la encuentra");

            ClicEnFiltro(form, RadiosFiltro(form)[3]);
            grid.Rows.Count.Should().Be(1, "al cambiar a Hojas, el mismo texto si encuentra");
            grid.Rows[0].Cells[0].Value.Should().Be("H-001");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Filtro_PonerTodosDevuelveLaListaCompleta()
    {
        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();

            DataGridView grid = Grid(form);
            UIRadioButton[] radios = RadiosFiltro(form);
            UIRadioButton todos = (UIRadioButton)radios[0];

            ClicEnFiltro(form, radios[2]);
            todos.Checked.Should().BeFalse("al marcar un tipo, Todos se desmarca solo");
            grid.Rows.Count.Should().Be(1, "solo queda el rollo cortado");

            ClicEnFiltro(form, todos);
            todos.Checked.Should().BeTrue();
            radios[2].Checked.Should().BeFalse("y el tipo anterior se desmarca");
            grid.Rows.Count.Should().Be(4, "vuelve a verse el catalogo entero");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Filtro_ElContadorDiceCuantoMuestraYQueFiltroHayPuesto()
    {
        FrmProductos form = CrearFormulario(CatalogoPorTipos());
        try
        {
            await form.InitializeAsync();

            UILabel contador = (UILabel)form.Controls.Find("lblContadorDetalle", true).Single();
            contador.Text.Should().Contain("Mostrando 4", "sin filtro se ven los cuatro");
            contador.Text.Should().NotContain("Master:");

            ClicEnFiltro(form, RadiosFiltro(form)[4]);

            contador.Text.Should().Contain("Graphics: Mostrando 1",
                "el contador nombra el filtro activo y el total de lo que se ve");
            contador.Text.Should().Contain("Total 4", "y recuerda cuanto hay en el catalogo entero");
        }
        finally
        {
            form.Dispose();
        }
    }
}
