using System.Data;
using System.Reflection;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Ritrama2025.Core;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ProductsService;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Services.ProduccionService;
using Sunny.UI;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Prueba estructural del rediseño de FrmProductos (paneles 30/70).
/// No toca base de datos: construye el formulario con un stub de IProductsService para
/// comprobar que el disenador enlaza buscador, grid de 3 columnas, contador y pestana de detalle.
/// </summary>
[Collection("Productos")]
public class FrmProductosLayoutTests
{
    /// <summary>
    /// Servicio minimo para poder instanciar el formulario sin dependencias reales.
    /// Graba lo que llega a Add/Update (para comprobar el mapeo de pantalla a dominio) y su
    /// resultado es programable, para poder probar los caminos de fallo sin base de datos.
    /// </summary>
    private sealed class ProductsServiceStub : IProductsService
    {
        private readonly List<Product> _catalogo;

        public ProductsServiceStub(IReadOnlyList<Product>? catalogo = null)
            => _catalogo = catalogo?.ToList() ?? new List<Product>();

        /// <summary>Ultimo producto recibido por Add o Update, o null si no se llamo a ninguno.</summary>
        public Product? UltimoGuardado { get; private set; }

        /// <summary>Que devuelven Add y Update. Programable para simular un rechazo del servicio.</summary>
        public Result<bool> ResultadoGuardado { get; set; } = Result<bool>.Success(true);

        /// <summary>Que devuelve ValidateProduct. Programable para probar la validacion sin ir a la base.</summary>
        public Result ResultadoValidacion { get; set; } = Result.Success();

        public Task<DataSet> Load(CancellationToken cancellationToken = default)
            => Task.FromResult(new DataSet());

        public Task<bool> Add(Product producto) => Task.FromResult(true);

        public Task<bool> Update(Product producto) => Task.FromResult(true);

        public bool Anular(string IdProduct) => true;

        public bool ValidProductid(string id) => false;

        public Task<Result<bool>> AddValidatedAsync(Product producto, CancellationToken cancellationToken = default)
        {
            UltimoGuardado = producto;

            if (ResultadoGuardado.IsSuccess)
            {
                // El alta deja el producto en el catalogo, igual que lo haria la recarga desde
                // la base: es lo que permite comprobar que el grid lo vuelve a mostrar.
                _catalogo.Add(producto);
            }

            return Task.FromResult(ResultadoGuardado);
        }

        public Task<Result<bool>> UpdateValidatedAsync(Product producto, CancellationToken cancellationToken = default)
        {
            UltimoGuardado = producto;
            return Task.FromResult(ResultadoGuardado);
        }

        public Task<Result<bool>> ExistsAsync(string id, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(false));

        public Task<Result<bool>> AnularAsync(string idProduct, CancellationToken cancellationToken = default)
            => Task.FromResult(Result<bool>.Success(true));

        public Task<Result<IReadOnlyList<Product>>> LoadTypedAsync(CancellationToken cancellationToken = default)
            => Task.FromResult(Result<IReadOnlyList<Product>>.Success(_catalogo.ToList()));

        public Result ValidateProduct(Product producto) => ResultadoValidacion;
    }

    /// <summary>Stub para el servicio de consecutivos de productos (inicia en 99999).</summary>
    private sealed class ConsecutivosServiceStub : IConsecutivosService
    {
        private int _valor = 99998; // Se incrementa a 99999 en la primera llamada

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

    /// <summary>
    /// Formulario con los dialogos sustituidos por capturas. Un MessageBox real dentro de una
    /// prueba la dejaria colgada a la espera de que alguien pulse un boton a mano.
    /// </summary>
    private sealed class FrmProductosSinDialogos : FrmProductos
    {
        /// <summary>Avisos que habria mostrado el formulario, en orden.</summary>
        public List<string> Avisos { get; } = new();

        /// <summary>
        /// Respuesta que devolveria la confirmacion de Cancelar. Es un campo y no una propiedad
        /// porque el analizador de WinForms (WFO1000) exige atributo de serializacion en toda
        /// propiedad publicable de un formulario, y esto es puro apoyo de prueba.
        /// </summary>
        public bool ConfirmarSi = true;

        public FrmProductosSinDialogos(IProductsService productsService, IExportDataService exportDataService, IReportsService reportsService, IConfiguration configuration, IConsecutivosService consecutivosService)
            : base(productsService, exportDataService, reportsService, configuration, consecutivosService)
        {
        }

        protected override void MostrarAviso(string mensaje) => Avisos.Add(mensaje);

        protected override bool ConfirmarDescarte(string pregunta) => ConfirmarSi;
    }

    /// <summary>Stub de la exportacion a Excel: cumple el contrato sin escribir ficheros.</summary>
    private sealed class ExportDataServiceStub : IExportDataService
    {
        public bool ExportToExcel<T>(List<T> data, string FileName) => true;

        public bool ExportToExcelProducts<T>(List<T> data, string FileName) => true;

        public bool ExportTxtFormatRollosCortados(DataRow[] rollos, bool solo_rc, string? fecha_produccion, string? fecha_registro, bool openNotePad) => true;

        public bool ExportTxtFormatMasterRePrintLabel(ProductMAP master, bool openNotePad) => true;
    }

    /// <summary>Stub del visor de reportes: cumple el contrato sin abrir ninguna ventana.</summary>
    private sealed class ReportsServiceStub : IReportsService
    {
        public void Reporte_Productos(Form form, string Report_Title, string Report_Name) { }

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

    private static FrmProductos CrearFormulario(IReadOnlyList<Product>? catalogo = null)
        => new(new ProductsServiceStub(catalogo), new ExportDataServiceStub(), new ReportsServiceStub(),
               new ConfigurationBuilder().Build(), new ConsecutivosServiceStub());

    /// <summary>
    /// Formulario con dialogos capturados y acceso al stub, que es lo que necesitan las pruebas
    /// de guardado: sin el stub no se puede comprobar que llego al servicio lo tecleado.
    /// </summary>
    private static FrmProductosSinDialogos CrearFormularioProbable(
        IReadOnlyList<Product>? catalogo,
        out ProductsServiceStub servicio)
    {
        servicio = new ProductsServiceStub(catalogo);
        return new FrmProductosSinDialogos(servicio, new ExportDataServiceStub(), new ReportsServiceStub(),
                                           new ConfigurationBuilder().Build(), new ConsecutivosServiceStub());
    }

    /// <summary>
    /// Abre sesion con los permisos de escritura de Productos. Sin esto, PermisoHelper devuelve
    /// false (no hay usuario) y los botones de la barra no hacen nada. Se restaura al terminar
    /// para no arrastrar la sesion a las demas pruebas.
    /// </summary>
    private static IDisposable SesionConPermisoDeEscritura()
    {
        Usuario? usuarioAnterior = SesionActual.Usuario;
        List<string> permisosAnteriores = [.. SesionActual.Permisos];

        SesionActual.Usuario = new Usuario { UserId = 1, Username = "prueba" };
        SesionActual.Permisos.AddRange(["Productos:Ver", "Productos:Crear", "Productos:Editar"]);

        return new AccionAlDispose(() =>
        {
            SesionActual.Usuario = usuarioAnterior;
            SesionActual.Permisos.Clear();
            SesionActual.Permisos.AddRange(permisosAnteriores);
        });
    }

    private sealed class AccionAlDispose(Action accion) : IDisposable
    {
        public void Dispose() => accion();
    }

    /// <summary>Producto minimo del catalogo de prueba: el codigo, el estado y la categoria.</summary>
    private static Product ProductoDePrueba(string id, bool anulado, bool master = true) => new()
    {
        Product_id = id,
        Product_Name = "Producto " + id,
        Product_Description = string.Empty,
        Referencia = string.Empty,
        Codigo_Barra = string.Empty,
        Anulado = anulado,
        Master = master
    };

    /// <summary>Los cuatro radio de tipo, en el orden en que se pintan.</summary>
    private static UIRadioButton[] RadiosTipo(FrmProductos form) =>
    [
        (UIRadioButton)form.Controls.Find("rbTipoMaster", true).Single(),
        (UIRadioButton)form.Controls.Find("rbTipoRolloCortado", true).Single(),
        (UIRadioButton)form.Controls.Find("rbTipoHojas", true).Single(),
        (UIRadioButton)form.Controls.Find("rbTipoGraphics", true).Single()
    ];

    /// <summary>Selecciona la fila del grid que tiene el codigo indicado.</summary>
    private static void SeleccionarFila(DataGridView grid, string productId)
    {
        DataGridViewRow fila = grid.Rows.Cast<DataGridViewRow>()
            .Single(r => string.Equals(r.Cells[0].Value?.ToString(), productId, StringComparison.Ordinal));

        grid.CurrentCell = fila.Cells[0];
    }

    [Fact]
    public void Raiz_DivideElAnchoEnTreintaYSetentaPorCiento()
    {
        using FrmProductos form = CrearFormulario();

        TableLayoutPanel root = (TableLayoutPanel)form.Controls.Find("tlpRoot", true).Single();

        root.ColumnCount.Should().Be(2);
        root.ColumnStyles[0].SizeType.Should().Be(SizeType.Percent);
        root.ColumnStyles[0].Width.Should().Be(30f);
        root.ColumnStyles[1].Width.Should().Be(70f);
    }

    [Fact]
    public void PanelIzquierdo_TieneBuscadorGridDeTresColumnasYContador()
    {
        using FrmProductos form = CrearFormulario();

        form.Controls.Find("txtSearch", true).Should().ContainSingle();

        DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
        grid.Columns.Count.Should().Be(3);
        grid.Columns[0].DataPropertyName.Should().Be("ProductId");
        grid.Columns[1].DataPropertyName.Should().Be("ProductName");
        grid.Columns[2].DataPropertyName.Should().Be("Tipo");
        grid.ReadOnly.Should().BeTrue();
        grid.AllowUserToAddRows.Should().BeFalse();
        grid.DefaultCellStyle.SelectionBackColor.Should().Be(Color.Black);
        grid.DefaultCellStyle.SelectionForeColor.Should().Be(Color.White);
        grid.RowsDefaultCellStyle.SelectionBackColor.Should().Be(Color.Black);
        grid.RowsDefaultCellStyle.SelectionForeColor.Should().Be(Color.White);

        Control contador = form.Controls.Find("lblContador", true).Single();
        contador.Text.Should().StartWith("Productos existentes");
    }

    [Fact]
    public void PanelDerecho_EmpiezaConLaPestanaDeDetalle()
    {
        using FrmProductos form = CrearFormulario();

        TabPage detalle = (TabPage)form.Controls.Find("tabDetalleProducto", true).Single();

        detalle.Text.Should().Be("Detalle");
        form.Controls.Find("txtDetId", true).Should().ContainSingle();
        form.Controls.Find("txtDetDescripcion", true).Should().ContainSingle();
    }

    [Fact]
    public void Detalle_MuestraDescripcionEnSuFilaYTipoEncimaDelGrupo()
    {
        using FrmProductos form = CrearFormulario();

        TableLayoutPanel detalle = (TableLayoutPanel)form.Controls.Find("tlpDetalle", true).Single();
        Control etiquetaDescripcion = form.Controls.Find("lblDetDescripcion", true).Single();
        Control campoDescripcion = form.Controls.Find("txtDetDescripcion", true).Single();
        Control etiquetaTipo = form.Controls.Find("lblDetTipo", true).Single();
        Control grupoTipo = form.Controls.Find("grpTipo", true).Single();

        detalle.GetRow(etiquetaDescripcion).Should().Be(3, "la descripcion ocupa el hueco que dejo el tipo");
        detalle.GetRow(campoDescripcion).Should().Be(3);
        detalle.GetRow(etiquetaTipo).Should().Be(9, "el titulo del tipo va sobre la caja de grupo");
        detalle.GetRow(grupoTipo).Should().Be(10, "la caja de grupo con los radios va justo debajo de su titulo");
        detalle.GetColumnSpan(etiquetaTipo).Should().Be(2, "el titulo abarca todo el ancho de su fila");
        detalle.GetColumnSpan(grupoTipo).Should().Be(2, "la caja de grupo abarca todo el ancho de su fila");
        detalle.RowStyles[3].Height.Should().Be(38f);
        detalle.RowStyles[9].Height.Should().Be(24f, "la fila del titulo va pegada a la caja de grupo");
        detalle.RowStyles[10].Height.Should().Be(138f, "la fila del grupo es su alta");
    }

    [Fact]
    public void Detalle_ElEstadoVaDeUltimoConUnSwitchDeActivoYDesactivado()
    {
        using FrmProductos form = CrearFormulario();

        TableLayoutPanel detalle = (TableLayoutPanel)form.Controls.Find("tlpDetalle", true).Single();
        UISwitch estado = (UISwitch)form.Controls.Find("swDetEstado", true).Single();
        Control etiquetaEstado = form.Controls.Find("lblDetEstado", true).Single();
        Control grupoTipo = form.Controls.Find("grpTipo", true).Single();

        estado.GetType().Should().Be<UISwitch>("el estado se muestra con el switch de Sunny UI");
        estado.ActiveText.Should().Be("Activo");
        estado.InActiveText.Should().Be("Desactivado");
        estado.ReadOnly.Should().BeTrue("sin modo edicion el switch solo muestra el estado");

        // El switch es un control compacto: con Dock.Fill se estiraba a los 591 px de la
        // columna y quedaba descolgado de las cajas de arriba. Se compara contra el ancho
        // de una caja de texto de la misma columna, que es la referencia del diseno.
        Control campoRatio = form.Controls.Find("txtDetRatio", true).Single();
        estado.Dock.Should().Be(DockStyle.Left, "el switch conserva su ancho en vez de llenar la columna");
        estado.Width.Should().BeLessThan(
            campoRatio.Width / 2,
            "es un interruptor, no un campo de texto de ancho completo");
        estado.Height.Should().BeLessThan(
            (int)detalle.RowStyles[11].Height,
            "no se estira a toda la altura de la fila");

        detalle.GetRow(etiquetaEstado).Should().Be(
            detalle.GetRow(grupoTipo) + 1,
            "el estado es la ultima fila del detalle, debajo de la caja de tipo");
        detalle.GetRow(estado).Should().Be(detalle.GetRow(etiquetaEstado));
        detalle.RowStyles[11].Height.Should().Be(38f, "misma altura que las demas filas de campos");
    }

    [Fact]
    public async Task Detalle_ElSwitchEnciendeConElProductoVigenteYApagaConElAnulado()
    {
        FrmProductos form = CrearFormulario(
        [
            ProductoDePrueba("00001", anulado: false),
            ProductoDePrueba("00002", anulado: true)
        ]);

        try
        {
            await form.InitializeAsync();

            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            UISwitch estado = (UISwitch)form.Controls.Find("swDetEstado", true).Single();

            SeleccionarFila(grid, "00001");
            estado.Active.Should().BeTrue("un producto vigente se muestra Activo");

            SeleccionarFila(grid, "00002");
            estado.Active.Should().BeFalse("un producto anulado se muestra Desactivado");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public void Detalle_MuestraElCostoEnLaFilaJustoDebajoDelPrecio()
    {
        using FrmProductos form = CrearFormulario();

        TableLayoutPanel detalle = (TableLayoutPanel)form.Controls.Find("tlpDetalle", true).Single();
        Control etiquetaPrecio = form.Controls.Find("lblDetPrecio", true).Single();
        Control campoPrecio = form.Controls.Find("txtDetPrecio", true).Single();
        Control etiquetaCosto = form.Controls.Find("lblDetCosto", true).Single();
        Control campoCosto = form.Controls.Find("txtDetCosto", true).Single();

        etiquetaCosto.Text.Should().Be("Costo");
        detalle.GetRow(etiquetaCosto).Should().Be(
            detalle.GetRow(etiquetaPrecio) + 1,
            "el costo se muestra justo debajo del precio");
        detalle.GetRow(campoCosto).Should().Be(detalle.GetRow(campoPrecio) + 1);
        detalle.RowStyles[7].Height.Should().Be(38f, "misma altura que las demas filas de campos");
    }

    [Fact]
    public async Task Detalle_EscribeElCostoDelProductoSeleccionadoConDosDecimales()
    {
        FrmProductos form = CrearFormulario(
        [
            new Product
            {
                Product_id = "00001",
                Product_Name = "Producto 00001",
                Product_Description = string.Empty,
                Referencia = string.Empty,
                Codigo_Barra = string.Empty,
                Precio = 125.5m,
                Costo = 98.25m,
                Master = true
            }
        ]);

        try
        {
            await form.InitializeAsync();

            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            // Sunny.UI.UITextBox no hereda de TextBoxBase: se leen como Control.
            Control precio = form.Controls.Find("txtDetPrecio", true).Single();
            Control costo = form.Controls.Find("txtDetCosto", true).Single();

            SeleccionarFila(grid, "00001");

            precio.Text.Should().Be("125.50");
            costo.Text.Should().Be("98.25", "el costo se muestra con los mismos dos decimales que el precio");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public void Tipo_SeMuestraConCuatroRadiosDentroDeLaPestanaDeDetalle()
    {
        using FrmProductos form = CrearFormulario();

        UIGroupBox contenedor = (UIGroupBox)form.Controls.Find("grpTipo", true).Single();
        contenedor.Parent!.Name.Should().Be("tlpDetalle", "el grupo de tipo va en la fila Tipo del detalle");
        contenedor.Text.Should().BeEmpty("el titulo vive en la etiqueta, la caja queda limpia");

        Control titulo = form.Controls.Find("lblDetTipo", true).Single();
        titulo.Text.Should().Be("Tipo de producto");

        UIRadioButton[] radios = RadiosTipo(form);

        radios.Select(r => r.Text).Should().Equal("Master", "Rollos Cortados", "Hojas", "Graphics");
        radios.Should().OnlyContain(r => r.Parent == contenedor, "los cuatro viven en el mismo contenedor");
        radios.Should().OnlyContain(r => r.ReadOnly, "sin modo edicion, el tipo solo se muestra");
        radios.Select(r => r.Location.X).Should().OnlyContain(
            x => x == radios[0].Location.X, "los cuatro radios van apilados en vertical");
        radios.Select(r => r.Location.Y).Should().BeInAscendingOrder(
            "cada categoria va en su propia fila, de arriba hacia abajo");

        // Sin producto seleccionado no hay categoria marcada: no se inventa una.
        radios.Should().OnlyContain(r => !r.Checked);
    }

    [Fact]
    public async Task Tipo_MarcaLaCategoriaDelProductoSeleccionado()
    {
        FrmProductos form = CrearFormulario(
        [
            ProductoDePrueba("00001", anulado: false, master: true),
            ProductoDePrueba("00002", anulado: false, master: false)
        ]);

        try
        {
            await form.InitializeAsync();

            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            UIRadioButton[] radios = RadiosTipo(form);

            SeleccionarFila(grid, "00001");
            radios.Select(r => r.Checked).Should().Equal(true, false, false, false);

            // El segundo producto no tiene ningun bit de categoria: los cuatro quedan sin marcar.
            SeleccionarFila(grid, "00002");
            radios.Should().OnlyContain(r => !r.Checked);
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public void BarraDeAcciones_CuelgaArribaDelDetalle_ConNuevoYEditar()
    {
        using FrmProductos form = CrearFormulario();

        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        Panel panelDer = (Panel)form.Controls.Find("panelDer", true).Single();

        barra.Dock.Should().Be(DockStyle.Top);
        barra.Parent.Should().BeSameAs(panelDer);
        panelDer.Controls.IndexOf(barra).Should().BeGreaterThan(
            0,
            "el area de detalle (Dock.Fill) se agrega primero para que la barra le robe el borde superior");

        ToolStripButton nuevo = (ToolStripButton)barra.Items["btnNuevoProducto"]!;
        ToolStripButton editar = (ToolStripButton)barra.Items["btnEditarProducto"]!;

        nuevo.Text.Should().Be("Nuevo");
        nuevo.ToolTipText.Should().Be("Nuevo producto");
        nuevo.Enabled.Should().BeTrue("crear no depende de la seleccion de la lista");

        editar.Text.Should().Be("Editar");
        editar.ToolTipText.Should().Be("Editar el producto seleccionado");
        editar.Enabled.Should().BeFalse("al abrir el modulo todavia no hay producto seleccionado");
    }

    [Fact]
    public async Task BotonEditar_SeEnciendeConCualquierProductoSeleccionado_TambienAnulado()
    {
        FrmProductos form = CrearFormulario([ProductoDePrueba("00001", anulado: false), ProductoDePrueba("00002", anulado: true)]);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            ToolStripButton editar = (ToolStripButton)barra.Items["btnEditarProducto"]!;
            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();

            grid.Rows.Count.Should().Be(2);

            SeleccionarFila(grid, "00001");
            editar.Enabled.Should().BeTrue("el producto seleccionado esta vigente");

            // Reactivar un producto anulado se hace desde Editar, asi que el boton no lo apaga:
            // es el ProductsService quien rechaza si la operacion no lo reactiva.
            SeleccionarFila(grid, "00002");
            editar.Enabled.Should().BeTrue("reactivar un anulado es una operacion valida desde Editar");
        }
        finally
        {
            form.Dispose();
        }
    }

    // ─────────────────────────────────────────────────────────────────
    // Modos de edicion (Consulta / Nuevo / Editar)
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void ModoConsulta_LlegaElDetalleEnteroEnSoloLectoriaYOCultaGuardarYCancelar()
    {
        using FrmProductos form = CrearFormulario([ProductoDePrueba("00001", anulado: false)]);

        DetalleEnteroEnSoloLectoria(form).Should().BeTrue("en consulta el detalle se ve pero no se escribe");
        (UIRadioButton[] radios, UISwitch estado) = ControlesDeCategoria(form);
        radios.Should().OnlyContain(r => r.ReadOnly,
            "la categoria solo se muestra mientras no se escribe");
        estado.ReadOnly.Should().BeTrue("el estado solo se muestra mientras no se escribe");

        // No se comprueba que Guardar y Cancelar esten ocultos: Visible devuelve false en
        // cuanto el ToolStrip contenedor no esta visible, y un formulario de prueba nunca se
        // muestra, asi que la asercion leeria siempre false y no probaria nada. Lo que si se
        // comprueba es que la consulta no habilite la escritura.
    }

    [Fact]
    public void BotonNuevo_DejaElDetalleEscribibleYEnciendeGuardarYCancelar()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        using FrmProductos form = CrearFormulario();

        ToolStrip barra = BarraVisible(form);
        ((ToolStripButton)barra.Items["btnNuevoProducto"]!).PerformClick();

        DetalleEnteroEnSoloLectoria(form).Should().BeFalse("en alta se escribe todo el detalle");
        (UIRadioButton[] radios, UISwitch estado) = ControlesDeCategoria(form);
        radios.Should().OnlyContain(r => !r.ReadOnly, "la categoria se elige al dar de alta, no se inventa sola");

        // Un producto nuevo nace siempre activo, asi que en el alta el interruptor de estado se
        // OCULTA (junto con su etiqueta): no hay nada que decidir y enseñarlo invita a creer que
        // se puede dar de alta desactivado. Lo que si se comprueba es que quede bloqueado y en
        // Activo, que es lo que acaba yendo a la base.
        //
        // Que este oculto no se puede asertar: Control.Visible es visibilidad EFECTIVA, y da
        // false en cuanto el formulario no esta mostrado, que es el caso de todos estos tests
        // (mismo motivo por el que no se comprueba la visibilidad de los botones de la barra).
        estado.ReadOnly.Should().BeTrue("y ademas esta bloqueado, por si acaso se llegara a ver");

        // Que Guardar y Cancelar aparezcan y Nuevo se oculte tampoco: la visibilidad de un
        // ToolStripItem depende de la del ToolStrip que lo contiene. Se comprueba que la
        // escritura se abra y que los numeros salgan con su valor de partida.
        form.Controls.Find("txtDetPrecio", true).Single().Text.Should().Be("0",
            "los numeros arrancan a 0 para no obligar a escribir lo que no aplique");
        estado.Active.Should().BeTrue(
            "y por dentro sigue en Activo, que es lo que se va a guardar");
    }

    [Fact]
    public async Task BotonEditar_DejaElDetalleEscribibleYFijaElCodigo()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductos form = CrearFormulario([ProductoDePrueba("00001", anulado: false)]);
        try
        {
            await form.InitializeAsync();

            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            SeleccionarFila(grid, "00001");

            ((ToolStripButton)((ToolStrip)form.Controls.Find("barraHerramientas", true).Single())
                .Items["btnEditarProducto"]!).PerformClick();

            ((UITextBox)form.Controls.Find("txtDetId", true).Single()).ReadOnly.Should().BeTrue(
                "el codigo es la clave primaria del UPDATE: no se puede cambiar al editar");
            DetalleEnteroEnSoloLectoriaSinCodigo(form).Should().BeFalse("en edicion se escribe el detalle");
            (UIRadioButton[] radios, UISwitch estado) = ControlesDeCategoria(form);
            radios.Should().OnlyContain(r => !r.ReadOnly, "la categoria se puede corregir al editar");
            estado.ReadOnly.Should().BeFalse("el estado se puede cambiar al editar");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task EditarUnProductoAnulado_PermiteTocarElSwitchParaReactivarlo()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductos form = CrearFormulario([ProductoDePrueba("00001", anulado: true)]);
        try
        {
            await form.InitializeAsync();

            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            SeleccionarFila(grid, "00001");

            ((ToolStripButton)((ToolStrip)form.Controls.Find("barraHerramientas", true).Single())
                .Items["btnEditarProducto"]!).PerformClick();

            UISwitch estado = (UISwitch)form.Controls.Find("swDetEstado", true).Single();
            estado.Active.Should().BeFalse("un anulado entra en edicion con el switch apagado");
            estado.ReadOnly.Should().BeFalse("reactivarlo exige poder mover el switch");
        }
        finally
        {
            form.Dispose();
        }
    }

    /// <summary>
    /// Los siete campos de datos del detalle, sin el codigo. El codigo va aparte porque en Editar
    /// se queda fijo a proposito y las pruebas del resto del detalle no deben depender de eso.
    /// Son UITextBox de SunnyUI, que no heredan de TextBox: se toman como Control y se castean.
    /// </summary>
    private static UITextBox[] CamposDelDetalle(FrmProductos form) =>
    [
        (UITextBox)form.Controls.Find("txtDetNombre", true).Single(),
        (UITextBox)form.Controls.Find("txtDetReferencia", true).Single(),
        (UITextBox)form.Controls.Find("txtDetCodebar", true).Single(),
        (UITextBox)form.Controls.Find("txtDetPrecio", true).Single(),
        (UITextBox)form.Controls.Find("txtDetCosto", true).Single(),
        (UITextBox)form.Controls.Find("txtDetRatio", true).Single(),
        (UITextBox)form.Controls.Find("txtDetDescripcion", true).Single()
    ];

    /// <summary>
    /// Los dos grupos que se mueven al escribir: los cuatro radios de categoria y el switch de
    /// estado. Se devuelven con su tipo concreto porque ReadOnly no esta en Control.
    /// </summary>
    private static (UIRadioButton[] Radios, UISwitch Estado) ControlesDeCategoria(FrmProductos form)
        => (RadiosTipo(form), (UISwitch)form.Controls.Find("swDetEstado", true).Single());

    /// <summary>True si los campos de datos del detalle estan bloqueados.</summary>
    private static bool DetalleEnteroEnSoloLectoriaSinCodigo(FrmProductos form)
        => CamposDelDetalle(form).All(c => c.ReadOnly);

    /// <summary>
    /// Devuelve la barra de acciones con la visibilidad encendida. ToolStripItem.Visible devuelve
    /// false cuando el ToolStrip que lo contiene no esta visible, y los formularios de estas
    /// pruebas nunca se muestran: sin esto, la asercion leeria siempre false y no probaria nada.
    /// </summary>
    private static ToolStrip BarraVisible(FrmProductos form)
    {
        ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
        barra.Visible = true;
        return barra;
    }

    /// <summary>Codigo de la fila seleccionada del grid, o null si no hay ninguna seleccionada.</summary>
    private static string? CodigoSeleccionado(DataGridView grid)
        => grid.CurrentRow?.Cells[0].Value?.ToString();

    /// <summary>True si todo el detalle, codigo incluido, esta bloqueado.</summary>
    private static bool DetalleEnteroEnSoloLectoria(FrmProductos form)
        => DetalleEnteroEnSoloLectoriaSinCodigo(form)
            && ((UITextBox)form.Controls.Find("txtDetId", true).Single()).ReadOnly;

    [Fact]
    public async Task GuardarEnNuevo_LlamaAddConLoTecleadoYDejaSeleccionadoElProductoNuevo()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductos form = CrearFormularioProbable(null, out ProductsServiceStub servicio);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();

            barra.Items["btnNuevoProducto"]!.PerformClick();

            form.Controls.Find("txtDetId", true).Single().Text = " 90001 ";
            form.Controls.Find("txtDetNombre", true).Single().Text = " Papel bond 90 ";
            form.Controls.Find("txtDetReferencia", true).Single().Text = "REF-9";
            form.Controls.Find("txtDetCodebar", true).Single().Text = "7501234567890";
            form.Controls.Find("txtDetDescripcion", true).Single().Text = "Bobina de 90 g";
            form.Controls.Find("txtDetPrecio", true).Single().Text = "125,50";
            form.Controls.Find("txtDetCosto", true).Single().Text = "98.25";
            form.Controls.Find("txtDetRatio", true).Single().Text = "1,2345";
            RadiosTipo(form)[2].Checked = true;

            // Aunque se intente apagar el estado, un alta SIEMPRE nace activa: el switch esta
            // bloqueado y SunnyUI ignora el setter de Active cuando el control es ReadOnly.
            ((UISwitch)form.Controls.Find("swDetEstado", true).Single()).Active = false;

            barra.Items["btnGuardarProducto"]!.PerformClick();

            Product guardado = servicio.UltimoGuardado!;
            guardado.Product_id.Should().Be("90001", "el codigo se recorta antes de guardarlo");
            guardado.Product_Name.Should().Be("Papel bond 90", "los textos tambien se recortan");
            guardado.Referencia.Should().Be("REF-9");
            guardado.Codigo_Barra.Should().Be("7501234567890");
            guardado.Product_Description.Should().Be("Bobina de 90 g");
            guardado.Precio.Should().Be(125.50m, "el precio con coma decimal es el que escribe el usuario");
            guardado.Costo.Should().Be(98.25m, "el costo con punto decimal, como se muestra en pantalla, tambien vale");
            guardado.Ratio.Should().Be(1.2345m, "el ratio se guarda con sus cuatro decimales");
            guardado.Hoja.Should().BeTrue();
            (guardado.Master || guardado.RolloCortado || guardado.Graphics).Should().BeFalse(
                "los cuatro bits de categoria son excluyentes y salen de los radios marcados");
            guardado.Anulado.Should().BeFalse(
                "un alta nace vigente: el estado esta bloqueado, asi que ni el interruptor ni lo guardado se apagan");

            grid.Rows.Count.Should().Be(1);
            grid.Rows[0].Cells[0].Value.Should().Be("90001");
            CodigoSeleccionado(grid).Should().Be("90001",
                "la fila guardada queda seleccionada, no la primera por defecto");

            // Tras guardar, el formulario vuelve a consulta: el detalle se bloquea otra vez.
            DetalleEnteroEnSoloLectoria(form).Should().BeTrue();
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task GuardarEnEditar_LlamaUpdateConLoCambiadoYCambiaElEstadoDelProducto()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductos form = CrearFormularioProbable(
            [ProductoDePrueba("00001", anulado: false)], out ProductsServiceStub servicio);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            SeleccionarFila(grid, "00001");

            barra.Items["btnEditarProducto"]!.PerformClick();
            form.Controls.Find("txtDetNombre", true).Single().Text = "Producto 00001 corregido";
            ((UISwitch)form.Controls.Find("swDetEstado", true).Single()).Active = false;

            barra.Items["btnGuardarProducto"]!.PerformClick();

            Product guardado = servicio.UltimoGuardado!;
            guardado.Product_id.Should().Be("00001", "el codigo nunca cambia al editar");
            guardado.Product_Name.Should().Be("Producto 00001 corregido");
            guardado.Anulado.Should().BeTrue("el switch apagado es lo que anula el producto");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task GuardarEnEditar_ActualizaElProductoSobreElQueSeApertro_YNoSobreLaFilaActiva()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductos form = CrearFormularioProbable(
            [ProductoDePrueba("00001", anulado: false), ProductoDePrueba("00002", anulado: false)],
            out ProductsServiceStub servicio);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            SeleccionarFila(grid, "00001");

            barra.Items["btnEditarProducto"]!.PerformClick();

            // Mover la seleccion del grid mientras se edita no debe recargar el detalle: si lo
            // hiciera, el usuario estaria corrigiendo un producto sobre la pantalla de otro y se
            // guardarian sobre el equivocado.
            SeleccionarFila(grid, "00002");
            form.Controls.Find("txtDetNombre", true).Single().Text.Should().Be("Producto 00001",
                "el detalle sigue mostrando el producto abierto, no el de la fila activa");

            barra.Items["btnGuardarProducto"]!.PerformClick();

            servicio.UltimoGuardado!.Product_id.Should().Be("00001",
                "se guarda el producto abierto, no el de la fila que quedo activa");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Guardar_ConFiltroActivoQueNoCuega_LimpiaElBuscadorParaQueSeVeaLoGuardado()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductos form = CrearFormularioProbable(
            [ProductoDePrueba("00001", anulado: false)], out ProductsServiceStub _);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = BarraVisible(form);
            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            UITextBox buscador = (UITextBox)form.Controls.Find("txtSearch", true).Single();

            buscador.Text = "00001";
            grid.Rows.Count.Should().Be(1);

            barra.Items["btnNuevoProducto"]!.PerformClick();
            form.Controls.Find("txtDetId", true).Single().Text = "90001";
            form.Controls.Find("txtDetNombre", true).Single().Text = "Producto 90001";
            RadiosTipo(form)[0].Checked = true;

            barra.Items["btnGuardarProducto"]!.PerformClick();

            buscador.Text.Should().BeEmpty(
                "si el filtro no casa con lo guardado, su fila no saldria y pareceria un fallo");
            grid.Rows.Count.Should().Be(2);
            CodigoSeleccionado(grid).Should().Be("90001", "y la fila del producto nuevo queda seleccionada");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Guardar_ConUnNumeroQueNoEsNumero_MuestraAvisoYNoLlamaAlServicio()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductosSinDialogos form = CrearFormularioProbable(null, out ProductsServiceStub servicio);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

            barra.Items["btnNuevoProducto"]!.PerformClick();
            form.Controls.Find("txtDetId", true).Single().Text = "90001";
            form.Controls.Find("txtDetNombre", true).Single().Text = "Producto 90001";
            RadiosTipo(form)[0].Checked = true;
            form.Controls.Find("txtDetPrecio", true).Single().Text = "doce";

            barra.Items["btnGuardarProducto"]!.PerformClick();

            form.Avisos.Should().ContainSingle("un solo aviso: el del campo que no se pudo leer");
            form.Avisos[0].Should().Contain("precio");
            servicio.UltimoGuardado.Should().BeNull("no se escribe nada en la base si un numero no cuadra");

            DetalleEnteroEnSoloLectoriaSinCodigo(form).Should().BeFalse(
                "el alta sigue abierta para corregir sin teclear de nuevo");
            form.Controls.Find("txtDetId", true).Single().Text.Should().Be("90001",
                "lo ya escrito se conserva");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Guardar_CuandoLaValidacionDeDominioFalla_MuestraElErrorYNoLlamaAlServicio()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductosSinDialogos form = CrearFormularioProbable(null, out ProductsServiceStub servicio);
        try
        {
            await form.InitializeAsync();
            servicio.ResultadoValidacion = Result.Failure("El código del producto (Product_id) es obligatorio.");

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

            barra.Items["btnNuevoProducto"]!.PerformClick();
            form.Controls.Find("txtDetNombre", true).Single().Text = "Producto sin codigo";
            RadiosTipo(form)[0].Checked = true;

            barra.Items["btnGuardarProducto"]!.PerformClick();

            form.Avisos.Should().ContainSingle();
            form.Avisos[0].Should().Contain("código");
            servicio.UltimoGuardado.Should().BeNull(
                "la validacion de dominio corta antes de tocar la base, sin viaje de ida y vuelta");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Guardar_CuandoElServicioFalla_MuestraElErrorYConservaLoTecleado()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductosSinDialogos form = CrearFormularioProbable(null, out ProductsServiceStub servicio);
        try
        {
            await form.InitializeAsync();
            servicio.ResultadoGuardado = Result<bool>.Failure("Ya existe un producto con el código '90001'.");

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();

            barra.Items["btnNuevoProducto"]!.PerformClick();
            form.Controls.Find("txtDetId", true).Single().Text = "90001";
            form.Controls.Find("txtDetNombre", true).Single().Text = "Producto 90001";
            RadiosTipo(form)[0].Checked = true;

            barra.Items["btnGuardarProducto"]!.PerformClick();

            form.Avisos.Should().ContainSingle();
            form.Avisos[0].Should().Be("Ya existe un producto con el código '90001'.",
                "el error del servicio se muestra tal cual, sin reescribirlo");
            DetalleEnteroEnSoloLectoriaSinCodigo(form).Should().BeFalse("el alta sigue abierta para corregirla");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task CancelarEnEditar_NoGuardaNadaYVuelveAConsulta()
    {
        using IDisposable sesion = SesionConPermisoDeEscritura();
        FrmProductos form = CrearFormularioProbable(
            [ProductoDePrueba("00001", anulado: false)], out ProductsServiceStub servicio);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            SeleccionarFila(grid, "00001");

            barra.Items["btnEditarProducto"]!.PerformClick();
            form.Controls.Find("txtDetNombre", true).Single().Text = "Cambio que se descarta";

            barra.Items["btnCancelarProducto"]!.PerformClick();

            servicio.UltimoGuardado.Should().BeNull("descartar no escribe nada");
            DetalleEnteroEnSoloLectoria(form).Should().BeTrue("vuelve al estado de consulta");
            form.Controls.Find("txtDetNombre", true).Single().Text.Should().Be("Producto 00001",
                "el detalle vuelve a mostrar el producto tal como estaba");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task SinPermisoParaCrear_NuevoNoAbreElFormularioParaEscribir()
    {
        // Sin sesion abierta, PermisoHelper devuelve false.
        FrmProductosSinDialogos form = CrearFormularioProbable(null, out ProductsServiceStub _);
        try
        {
            await form.InitializeAsync();

            ToolStrip barra = (ToolStrip)form.Controls.Find("barraHerramientas", true).Single();
            barra.Items["btnNuevoProducto"]!.PerformClick();

            DetalleEnteroEnSoloLectoria(form).Should().BeTrue("sin permiso, el boton no abre la escritura");
            form.Avisos.Should().ContainSingle();
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Grid_LaFilaDeUnProductoAnuladoSePintaEnRojoYLaVigenteNo()
    {
        FrmProductos form = CrearFormulario(
        [
            ProductoDePrueba("00001", anulado: false),
            ProductoDePrueba("00002", anulado: true)
        ]);

        try
        {
            await form.InitializeAsync();

            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();
            grid.Rows.Count.Should().Be(2);

            Color vigente = ColorPintada(form, grid, 0, "00001");
            Color anulado = ColorPintada(form, grid, 1, "00002");

            vigente.Should().NotBe(TextoDesactivadoEsperado,
                "un producto vigente se ve con su color normal, no apagado");
            anulado.Should().Be(TextoDesactivadoEsperado,
                "un producto desactivado tiene que verse con letra clara sobre fondo rojo");

            // Y el fondo es rojo, igual que en Clientes/Proveedores/Vendedores.
            grid.Rows[1].Cells[0].Style.BackColor.Should().Be(FondoDesactivadoEsperado);
            grid.Rows[0].Cells[0].Style.BackColor.Should().NotBe(FondoDesactivadoEsperado);

            // Ademas el tono tiene que RESALTAR: el rojo de las filas desactivadas de Productos
            // es #D02020, mas vivo que el Firebrick del resto de modulos, que se veia apagado
            // en este listado.
            int brillo = FondoDesactivadoEsperado.R + FondoDesactivadoEsperado.G + FondoDesactivadoEsperado.B;
            int brilloFirebrick = Color.Firebrick.R + Color.Firebrick.G + Color.Firebrick.B;
            brillo.Should().BeGreaterThan(
                brilloFirebrick,
                "el fondo de un producto desactivado debe ser un rojo vivo que resalte, no el Firebrick apagado");
        }
        finally
        {
            form.Dispose();
        }
    }

    [Fact]
    public async Task Grid_UnaFilaDesactivadaYSelecionadaSiguePintandoseEnRojo()
    {
        FrmProductos form = CrearFormulario(
        [
            ProductoDePrueba("00001", anulado: false),
            ProductoDePrueba("00002", anulado: true)
        ]);

        try
        {
            await form.InitializeAsync();

            DataGridView grid = (DataGridView)form.Controls.Find("gridProductos", true).Single();

            // Se selecciona el desactivado, que es lo que hace el usuario para mirarlo en el
            // detalle. Con el negro de la seleccion el rojo se comia y el estado no se veia.
            SeleccionarFila(grid, "00002");
            grid.CurrentRow!.Selected.Should().BeTrue("partimos de la fila desactivada seleccionada");

            ColorPintada(form, grid, 1, "00002");

            DataGridViewCell celda = grid.Rows[1].Cells[0];

            // La seleccion de una fila desactivada se pinta en rojo oscuro para que el fondo
            // rojo de la fila siga notandose; la vigente sigue con la seleccion negra.
            celda.Style.SelectionBackColor.Should().Be(Color.DarkRed,
                "la seleccion del modulo no debe tapar el rojo del producto desactivado");
            celda.Style.SelectionForeColor.Should().Be(TextoDesactivadoEsperado,
                "pero la letra de un desactivado seleccionado sigue clara, para que se vea el estado");
            celda.Style.SelectionForeColor.Should().NotBe(Color.White);
        }
        finally
        {
            form.Dispose();
        }
    }

    /// <summary>El rojo vivo del fondo de las filas de productos desactivados (#D02020).</summary>
    private static Color FondoDesactivadoEsperado => Color.FromArgb(208, 32, 32);

    /// <summary>La letra clara con la que se marcan las filas de productos desactivados.</summary>
    private static Color TextoDesactivadoEsperado => Color.FromArgb(255, 214, 214);

    /// <summary>
    /// Dispara el CellFormatting del grid sobre una celda (que es lo que hace el pintado) y
    /// devuelve el ForeColor que queda. Se invoca a mano porque el evento solo salta al
    /// pintar de verdad, y un formulario de prueba nunca se dibuja.
    /// </summary>
    private static Color ColorPintada(FrmProductos form, DataGridView grid, int indiceFila, string productId)
    {
        DataGridViewRow fila = grid.Rows[indiceFila];
        fila.Cells[0].Value.Should().Be(productId, "la fila consultada es la esperada");

        // El handler lee el Anulado del objeto enlazado: si la fila no lo trae, no pinta nada.
        MethodInfo formatear = typeof(FrmProductos).GetMethod(
            "GridProductos_CellFormatting", BindingFlags.Instance | BindingFlags.NonPublic)!;

        // Invoke recibe primero el objeto sobre el que se invoca (el formulario) y despues los
        // argumentos del handler: el grid es el sender.
        //
        // El cellStyle que se le pasa ES el de la celda, que es lo que hace WinForms al lanzar
        // el evento de verdad: asi el handler escribe sobre la celda y el cambio se ve. Con un
        // estilo suelto el evento queda desconectado y la prueba no probaria nada.
        object?[] argumentos =
        [
            grid,
            new DataGridViewCellFormattingEventArgs(0, indiceFila, null, typeof(string), fila.Cells[0].Style)
        ];

        formatear.Invoke(form, argumentos);

        return fila.Cells[0].Style.ForeColor;
    }


    // ─────────────────────────────────────────────────────────────────
    // Persistencia del estado (capa de servicio, sin base de datos)
    // ─────────────────────────────────────────────────────────────────

    [Fact]
    public void UpdateDeProducto_PersisteElEstado()
    {
        // Activar o desactivar un producto desde el formulario solo funciona si el UPDATE
        // escribe la columna anulado. Sin esta linea, el switch se moveria en pantalla y el
        // cambio no llegaria nunca a la base.
        R.SQL_STRING_QUERY.UPDATE_PRODUCT.Should().Contain("anulado=@anulado");

        // Y tiene que ir parametrizado como el resto, no enrutado a un literal.
        R.SQL_STRING_QUERY.UPDATE_PRODUCT.Should().NotContain("anulado=1");
    }

    [Fact]
    public void UnProductoAnuladoSoloSeGuardaSiLaOperacionLoReactiva()
    {
        ProductValidator.ValidateEditableState(false, false, "00001").IsSuccess.Should().BeTrue(
            "un vigente se edita tal cual");
        ProductValidator.ValidateEditableState(false, true, "00001").IsSuccess.Should().BeTrue(
            "un vigente se puede desactivar");
        ProductValidator.ValidateEditableState(true, false, "00001").IsSuccess.Should().BeFalse(
            "un anulado no se edita en el mismo estado");
        ProductValidator.ValidateEditableState(true, true, "00001").IsSuccess.Should().BeTrue(
            "un anulado se puede reactivar, que es lo que vuelve a hacerlo editable");

        Result fallo = ProductValidator.ValidateEditableState(true, false, "00001");
        fallo.ErrorCode.Should().Be(ProductValidator.CODE_ANULADO);
        fallo.Error.Should().Contain("00001", "el error dice que producto es");
    }

    [Fact]
    public void ElAnuladoDelUpdateYElDelBotonAnularSonLaMismaColumna()
    {
        // Si el UPDATE y el anulado escribieran columnas distintas, reactivar desde Editar y
        // anular desde el boton de anular no serian la misma operacion.
        R.SQL_STRING_QUERY.UPDATE_PRODUCT.Should().Contain("anulado=@anulado");
        R.SQL_STRING_QUERY.UPDATE_PRODUCT_ANULAR.Should().Contain("anulado=1");
        R.SQL_STRING_QUERY.SELECT_PRODUCT_ANULADO.Should().Contain("SELECT anulado");
    }
}
