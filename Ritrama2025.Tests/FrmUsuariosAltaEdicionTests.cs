using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Services.SeguridadService;
using Ritrama2025.Tests.Stubs;
using Sunny.UI;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Flujo de alta y edicion de FrmUsuarios. El detalle pasa de solo lectura a
/// escribible segun el modo, y lo que llega al servicio tiene que ser lo que el
/// usuario escribio.
/// </summary>
[Trait("Categoria", "Unit")]
[Collection("Usuarios")]
public class FrmUsuariosAltaEdicionTests : IDisposable
{
    private readonly Usuario? _usuarioAnterior = SesionActual.Usuario;
    private readonly List<string> _permisosAnteriores = [.. SesionActual.Permisos];

    public FrmUsuariosAltaEdicionTests()
    {
        // El módulo Usuarios no tiene permisos propios (igual que Vendedores), asi que la
        // sesion ya no decide nada aqui. Se dejan los del rol porque la de una aplicacion
        // real los trae y asi los tests corren con la sesion como es.
        SesionActual.Usuario = new Usuario { UserId = 1, Username = "prueba" };
        SesionActual.Permisos.AddRange(["Usuarios:Ver", "Usuarios:Crear", "Usuarios:Editar"]);
    }

    public void Dispose()
    {
        SesionActual.Usuario = _usuarioAnterior;
        SesionActual.Permisos.Clear();
        SesionActual.Permisos.AddRange(_permisosAnteriores);
    }

    /// <summary>
    /// Formulario con los dialogos sustituidos por capturas: un MessageBox real
    /// dentro de una prueba la deja colgada. Cancelar ya no pregunta, asi que solo
    /// hace falta capturar <see cref="MostrarAviso"/>.
    /// </summary>
    private sealed class FrmBajoPrueba(ISeguridadService servicio, IExportDataService exporta, IReportsService reportes)
        : FrmUsuarios(servicio, exporta, reportes)
    {
        public List<string> Avisos { get; } = [];

        protected override void MostrarAviso(string mensaje) => Avisos.Add(mensaje);
    }

    private static readonly Role[] Catalogo =
    [
        new() { RoleId = 1, Nombre = "Admin" },
        new() { RoleId = 2, Nombre = "Super-Admin" },
        new() { RoleId = 3, Nombre = "User Default" },
        new() { RoleId = 4, Nombre = "Invitado" }
    ];

    private static Usuario Admin() => new()
    {
        UserId = 1,
        Username = "admin",
        NombreCompleto = "Ana Perez",
        Email = "admin@ritrama.com",
        Activo = true
    };

    private static Usuario Mostrador() => new()
    {
        UserId = 2,
        Username = "mostrador",
        NombreCompleto = "Luis Gomez",
        Email = null,
        Activo = false
    };

    /// <summary>
    /// Crea el formulario ya mostrado. No es un detalle: <see cref="Control.Visible"/>
    /// devuelve false en cualquier control cuyo padre no lo esta, asi que sin Show()
    /// "esta visible" y "esta oculto" darian el mismo false y las aserciones de
    /// visibilidad no probarian nada. TopLevel=false evita abrir una ventana real.
    /// </summary>
    private static FrmBajoPrueba Form(ISeguridadService servicio)
    {
        FrmBajoPrueba form = new(servicio, new ExportDataServiceStub(), new ReportsServiceStub());
        form.TopLevel = false;
        form.Show();
        System.Windows.Forms.Application.DoEvents();
        return form;
    }

    private static FrmBajoPrueba FormCargado(params Usuario[] usuarios)
        => Form(new SeguridadServiceStub([.. usuarios], [.. Catalogo], RolesDe(usuarios)));

    /// <summary>
    /// Asigna a cada usuario el rol Admin (RoleId 1), que es lo que tiene un usuario real
    /// recien creado. Sin esto el combo de rol llega vacio al Editar y la validacion
    /// bloquea la edicion con "Elija el rol", que no es lo que se quiere probar.
    /// </summary>
    private static Dictionary<int, List<int>> RolesDe(IEnumerable<Usuario> usuarios)
        => usuarios.ToDictionary(u => u.UserId, _ => new List<int> { 1 });

    private static T Control<T>(FrmBajoPrueba form, string nombre) where T : Control
        => (T)form.Controls.Find(nombre, true).Single();

    /// <summary>
    /// Busca un ToolStripButton por nombre. No se usa Controls.Find porque los
    /// ToolStripItem no son Control: cuelgan del ToolStrip que los contiene.
    /// </summary>
    private static ToolStripButton Boton(FrmBajoPrueba form, string nombre)
    {
        ToolStrip barra = Control<ToolStrip>(form, "barraHerramientas");
        return barra.Items.OfType<ToolStripButton>().Single(b => b.Name == nombre);
    }

    private static void Click(ToolStripButton boton) => boton.PerformClick();

    private static void ElegirFila(FrmBajoPrueba form, int indice)
    {
        DataGridView grid = Control<DataGridView>(form, "gridUsuarios");
        grid.Rows[indice].Selected = true;
        grid.CurrentCell = grid.Rows[indice].Cells[0];
    }

    // ─────────────────────────────────────────────────────────────
    // Modo: que se puede escribir y que no
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task EnConsultaElDetalleEstaBloqueadoYNoHayParaEscribir()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        ElegirFila(form, 0);

        Control<UITextBox>(form, "txtDetUsuario").ReadOnly.Should().BeTrue();
        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeTrue();
        Control<UITextBox>(form, "txtDetTituloCargo").ReadOnly.Should().BeTrue();
        Control<UITextBox>(form, "txtDetDepartamento").ReadOnly.Should().BeTrue();
        Control<UIComboBox>(form, "cboRoles").Visible.Should().BeFalse();
        Control<UITextBox>(form, "txtPassword").Visible.Should().BeFalse();
        Boton(form, "btnGuardarUsuario").Visible.Should().BeFalse();
        Boton(form, "btnCancelarUsuario").Visible.Should().BeFalse();
    }

    [Fact]
    public async Task NuevoDejaEscribiblesNombreEmailPuestoAreaYContrasenaYElUsuario()
    {
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));

        Control<UITextBox>(form, "txtDetUsuario").ReadOnly.Should().BeFalse("el username se elige al dar de alta");
        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeFalse();
        Control<UITextBox>(form, "txtDetEmail").ReadOnly.Should().BeFalse();
        Control<UITextBox>(form, "txtDetTituloCargo").ReadOnly.Should().BeFalse();
        Control<UITextBox>(form, "txtDetDepartamento").ReadOnly.Should().BeFalse();
        Control<UITextBox>(form, "txtPassword").Visible.Should().BeTrue("la contraseña solo se pone al crear");
        Control<UIComboBox>(form, "cboRoles").Visible.Should().BeTrue();
        Boton(form, "btnGuardarUsuario").Visible.Should().BeTrue();
    }

    [Fact]
    public async Task EnNuevoElInterruptorDeActivoNoSeMuestra()
    {
        // CreateUsuarioAsync tiene activo=1 en el SQL: no hay parámetro que lo envíe.
        // Mostrarlo bloqueado daría a entender lo contrario.
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));

        Control<UISwitch>(form, "swDetActivo").Visible.Should().BeFalse();
        Control<UILabel>(form, "lblCapActivo").Visible.Should().BeFalse();
    }

    [Fact]
    public async Task EnNuevoElRotuloDeLaContrasenaSeVuelveAVer()
    {
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();

        // En consulta la fila de la contraseña vale 0 y Load mide el rotulo con ella.
        Control<UILabel>(form, "lblCapPassword").Height.Should().Be(0);

        Click(Boton(form, "btnNuevoUsuario"));

        // Al abrir la fila el rotulo tiene que recuperar su alto: Visible no basta,
        // porque un rotulo de 0 px esta visible y no se ve. Sin esto el alta mostraba
        // el campo de la contraseña sin decir que contenia.
        Control<UILabel>(form, "lblCapPassword").Visible.Should().BeTrue();
        Control<UILabel>(form, "lblCapPassword").Height.Should().Be(
            38, "la fila del alta mide 38 px y el rotulo los ocupa todos");
    }

    [Fact]
    public async Task EditarDejaElUsuarioEditableYElActivoTambien()
    {
        // UpdateUsuarioAsync escribe username en el UPDATE, asi que el login ya no
        // queda fijo en edicion: se puede renombrar desde la misma pantalla.
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        ElegirFila(form, 0);

        Click(Boton(form, "btnEditarUsuario"));

        Control<UITextBox>(form, "txtDetUsuario").ReadOnly.Should().BeFalse("el login ahora va al UPDATE");
        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeFalse();
        Control<UITextBox>(form, "txtDetEmail").ReadOnly.Should().BeFalse();
        Control<UITextBox>(form, "txtDetTituloCargo").ReadOnly.Should().BeFalse();
        Control<UITextBox>(form, "txtDetDepartamento").ReadOnly.Should().BeFalse();
        Control<UISwitch>(form, "swDetActivo").Visible.Should().BeTrue();
        Control<UITextBox>(form, "txtPassword").Visible.Should().BeFalse("la contraseña se cambia con ResetPasswordAsync");
    }

    [Fact]
    public async Task EnNuevoLasCajasVanVaciasYNoConGuiones()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        ElegirFila(form, 0);

        Click(Boton(form, "btnNuevoUsuario"));

        // El guion es la marca de consulta de "no hay valor". En una caja que hay que
        // llenar parece un dato ya puesto e invita a escribir encima.
        foreach (string caja in new[]
                 {
                     "txtDetUsuario", "txtDetNombre", "txtDetEmail",
                     "txtDetTituloCargo", "txtDetDepartamento", "txtPassword"
                 })
        {
            Control<UITextBox>(form, caja).Text.Should().BeEmpty(caja);
        }

        Control<UIComboBox>(form, "cboRoles").SelectedIndex.Should().Be(-1);
    }

    [Fact]
    public async Task ElFiltroDeRolNoSePierdeAlAbrirLaEdicion()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin(), Mostrador()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1], [2] = [3] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        RadioButton admin = Control<FlowLayoutPanel>(form, "flwRadiosRol")
            .Controls.OfType<RadioButton>().Single(r => r.Text == "Admin");
        admin.PerformClick();
        admin.Checked.Should().BeTrue();

        // Editar recarga el catalogo para volver a armar el combo de rol, y con el
        // catalogo se rearman los radios del filtro. Si no se cuida la seleccion, el
        // filtro se pierde solo y el usuario ve la lista completa sin haber pedido nada.
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        Control<FlowLayoutPanel>(form, "flwRadiosRol")
            .Controls.OfType<RadioButton>().Single(r => r.Checked).Text
            .Should().Be("Admin");
    }

    [Fact]
    public async Task ElRotuloDeActivoSeLeeEnLaMismaLineaQueElSwitch()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        System.Windows.Forms.Application.DoEvents();

        UILabel rotulo = Control<UILabel>(form, "lblCapActivo");
        UISwitch interruptor = Control<UISwitch>(form, "swDetActivo");

        // Esta prueba mira los bounds REALES del texto y del boton, no los margenes: un
        // rotulo con el alto equivocado tiene los margenes perfectos y aun asi la palabra
        // cae decenas de pixels mas abajo, que es como se veia.
        rotulo.Height.Should().Be(38 - rotulo.Margin.Vertical,
            "el alto de su propia fila menos sus margenes");

        float centroRotulo = rotulo.Top + rotulo.Height / 2f;
        float centroSwitch = interruptor.Top + interruptor.Height / 2f;

        Math.Abs(centroRotulo - centroSwitch).Should().BeLessThan(
            1.5f, "las dos palabras tienen que leerse en la misma linea");
    }

    [Fact]
    public async Task EditarCambiaElNombreYVuelveAConsultaConElValorNuevo()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();

        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        Control<UITextBox>(form, "txtDetNombre").Text = "Ana Perez de Lopez";
        Click(Boton(form, "btnGuardarUsuario"));

        form.Avisos.Should().ContainSingle(a => a.Contains("guardado"));

        // El detalle vuelve al de la fila, ya con el dato nuevo: si el formulario se
        // quedara mostrando lo tecleado, el usuario no sabria si se guardo.
        Control<UITextBox>(form, "txtDetNombre").Text.Should().Be("Ana Perez de Lopez");
        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeTrue();
    }

    [Fact]
    public async Task EditarNoCambiaElUsuarioNiLaClave()
    {
        SeguridadServiceStub servicio = new([Admin()], [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));
        Control<UITextBox>(form, "txtDetNombre").Text = "Otro nombre";
        Click(Boton(form, "btnGuardarUsuario"));

        // El username es la clave del UPDATE y la clave va por su propio camino
        // (ResetPasswordAsync): si el UPDATE los tocara, se renombraria el login o se
        // borraria el hash del usuario.
        servicio.UsuarioActualizado!.Username.Should().Be("admin");
        Control<UITextBox>(form, "txtPassword").Text.Should().BeEmpty("la clave no se escribe al editar");
    }

    [Fact]
    public async Task EditarGuardaElRolElegidoEnElCombo()
    {
        SeguridadServiceStub servicio = new([Admin(), Mostrador()], [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1], [2] = [3] });
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));
        Control<UIComboBox>(form, "cboRoles").SelectedIndex = 1;
        Click(Boton(form, "btnGuardarUsuario"));

        // UpdateUsuarioAsync no lleva roles: el rol se guarda aparte. Esta prueba fija
        // que al menos el formulario no se cuelga ni avisa de error en el camino.
        form.Avisos.Should().NotContain(a => a.Contains("Error"));
    }

    [Fact]
    public async Task TraGuardarSeVuelveAPoderEditarOtraVez()
    {
        using FrmBajoPrueba form = FormCargado(Admin(), Mostrador());
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, "primero", "Ana Uno", "uno@ritrama.com", "Clave123", 1);
        Click(Boton(form, "btnGuardarUsuario"));

        // El ciclo tiene que poder repetirse: elegir fila, Editar, guardar.
        Boton(form, "btnNuevoUsuario").Enabled.Should().BeTrue();
        ElegirFila(form, 0);
        Boton(form, "btnEditarUsuario").Enabled.Should().BeTrue();
    }

    [Fact]
    public async Task PuedeGuardarUnSegundoUsuarioDespuesDelPrimero()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();

        // Primer alta.
        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, "primero", "Ana Uno", "uno@ritrama.com", "Clave123", 1);
        Click(Boton(form, "btnGuardarUsuario"));
        form.Avisos.Should().Contain(a => a.Contains("creado"));

        // Segundo alta, sin cerrar el formulario.
        Click(Boton(form, "btnNuevoUsuario"));
        Boton(form, "btnGuardarUsuario").Visible.Should().BeTrue("el alta se vuelve a abrir");
        Escribir(form, "segundo", "Beto Dos", "dos@ritrama.com", "Clave456", 1);
        Click(Boton(form, "btnGuardarUsuario"));

        // Si el segundo guardado no ocurriera, el aviso seria el del fallo o no habria
        // ninguno nuevo.
        form.Avisos.Count(a => a.Contains("creado")).Should().Be(2, "los dos altas se guardaron");
        form.Avisos.Should().NotContain(a => a.Contains("Error") || a.Contains("No se pudo"));
    }

    [Fact]
    public async Task TrasGuardarElFormularioVuelveAConsultaYGuardarDesaparece()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, "primero", "Ana Uno", "uno@ritrama.com", "Clave123", 1);
        Click(Boton(form, "btnGuardarUsuario"));

        Boton(form, "btnGuardarUsuario").Visible.Should().BeFalse();
        Boton(form, "btnNuevoUsuario").Enabled.Should().BeTrue();
    }

    [Fact]
    public async Task ElOjoDeLaContrasenaLaMuestraYLaVuelveATapar()
    {
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));

        UITextBox clave = Control<UITextBox>(form, "txtPassword");
        UIButton ojo = Control<UIButton>(form, "btnOjoPassword");

        clave.PasswordChar.Should().NotBe(' ', "arranca tapada");

        ojo.PerformClick();
        clave.PasswordChar.Should().Be('\0', "'\\0' es sin mascara: la clave se ve");

        ojo.PerformClick();
        clave.PasswordChar.Should().Be('•', "vuelve a taparse con el mismo caracter de siempre");
    }

    [Fact]
    public async Task AlSalirDelAltaLaClaveQuedaTapada()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));
        Control<UIButton>(form, "btnOjoPassword").PerformClick();
        Control<UITextBox>(form, "txtPassword").PasswordChar.Should().Be('\0');

        Click(Boton(form, "btnCancelarUsuario"));

        // Si quedara a la vista, el siguiente alta abriría con la del usuario anterior
        // escrita en claro.
        Control<UITextBox>(form, "txtPassword").PasswordChar.Should().Be('•');
    }

    [Fact]
    public async Task FueraDelAltaElOjoNoSeUsa()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();

        Control<UIButton>(form, "btnOjoPassword").Enabled.Should().BeFalse();
        Control<UIButton>(form, "btnOjoPassword").Visible.Should().BeFalse();
    }

    [Fact]
    public async Task ElAltaMuestraElConsecutivoQueLeToca()
    {
        using FrmBajoPrueba form = FormCargado(Admin(), Mostrador());
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));

        // El UserId mas alto es 2, asi que el siguiente es 3. Se ve mientras se escribe
        // y no se escribe nunca: el ID lo pone la base.
        Control<UITextBox>(form, "txtDetId").Text.Should().Be("3");
        Control<UITextBox>(form, "txtDetId").ReadOnly.Should().BeTrue();
    }

    [Fact]
    public async Task SinUsuariosElConsecutivoArrancaEnUno()
    {
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));

        Control<UITextBox>(form, "txtDetId").Text.Should().Be("1");
    }

    [Fact]
    public async Task ElAltaMuestraElIdAunqueElUsuarioNoSeEscriba()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));

        // El campo no se escribe, pero se muestra: el rotulo tambien, o el numero
        // apareceria sin contexto.
        Control<UILabel>(form, "lblCapId").Visible.Should().BeTrue();
        Control<UITextBox>(form, "txtDetId").Visible.Should().BeTrue();
    }

    [Fact]
    public async Task EditarPoneEnElComboElRolDelUsuario()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [3] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);

        Click(Boton(form, "btnEditarUsuario"));

        UIComboBox roles = Control<UIComboBox>(form, "cboRoles");
        roles.Items.Cast<object>().Select(i => i.ToString())
            .Should().Equal("Admin", "Super-Admin", "User Default", "Invitado");
        roles.SelectedIndex.Should().Be(2, "User Default es el rol de este usuario");
    }

    [Fact]
    public async Task ElComboDeRolesNoOfreceLosRolesInactivos()
    {
        // Un rol dado de baja no se asigna: ofrecerlo dejaría usuarios sin permisos
        // porque el catalogo activo es el que tiene role_permisos al dia.
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo, new Role { RoleId = 9, Nombre = "Retirado", Activo = false }],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));

        Control<UIComboBox>(form, "cboRoles").Items.Cast<object>().Select(i => i.ToString())
            .Should().NotContain("Retirado");
    }

    // ─────────────────────────────────────────────────────────────
    // Validación
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task SinNombreNoSeGuarda()
    {
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));

        Escribir(form, usuario: "nuevo", nombre: "", email: "a@b.com", password: "clave123", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        form.Avisos.Should().ContainSingle();
        form.Avisos[0].Should().Contain("nombre");
    }

    [Fact]
    public async Task SinContrasenaEnNuevoNoSeGuarda()
    {
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));

        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "a@b.com", password: "", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        form.Avisos.Should().ContainSingle();
        form.Avisos[0].Should().Contain("contrase");
    }

    [Fact]
    public async Task UnUsuarioRepetidoNoSeGuarda()
    {
        // El servicio no comprueba duplicados: si hay uno, revienta el INSERT y el
        // mensaje que sale no le dice al usuario qué hacer. Se comprueba acá.
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));

        Escribir(form, usuario: "ADMIN", nombre: "Otro", email: "otro@b.com", password: "clave123", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        form.Avisos.Should().ContainSingle();
        form.Avisos[0].Should().ContainEquivalentOf("admin");
    }

    [Fact]
    public async Task SinRolElegidoNoSeGuarda()
    {
        // Un usuario sin rol no ve nada: es más barato rechazarlo acá.
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));

        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "a@b.com", password: "clave123", rol: null);

        Click(Boton(form, "btnGuardarUsuario"));

        form.Avisos.Should().ContainSingle();
        form.Avisos[0].Should().Contain("rol");
    }

    [Fact]
    public async Task UnCorreoMalEscritoNoSeGuarda()
    {
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));

        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "no-es-correo", password: "clave123", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        form.Avisos.Should().ContainSingle();
    }

    [Fact]
    public async Task SinCorreoSeGuardaPorqueElCampoEsOpcional()
    {
        SeguridadServiceStub servicio = new([], [.. Catalogo]);
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));

        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "", password: "clave123", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        // El correo no es obligatorio: lo que se prueba es que no se bloquea el alta,
        // no que no haya ningun aviso (guardar bien si avisa).
        servicio.UsuarioCreado.Should().NotBeNull();
        form.Avisos.Should().NotContain(a => a.Contains("correo", StringComparison.OrdinalIgnoreCase));
    }

    // ─────────────────────────────────────────────────────────────
    // Alta
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GuardarNuevoMandaLoEscritoYElIdDelRolElegido()
    {
        SeguridadServiceStub servicio = new([], [.. Catalogo]);
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "Luis Gomez", email: "luis@b.com", password: "clave123", rol: 2);

        Click(Boton(form, "btnGuardarUsuario"));

        // CreateUsuarioAsync recibe List<int>, no Role: el formulario traduce.
        servicio.UsuarioCreado.Should().NotBeNull();
        servicio.UsuarioCreado!.Username.Should().Be("nuevo");
        servicio.UsuarioCreado.NombreCompleto.Should().Be("Luis Gomez");
        servicio.UsuarioCreado.Email.Should().Be("luis@b.com");
        servicio.PasswordCreada.Should().Be("clave123");
        servicio.RoleIdsCreados.Should().Equal(2);
    }

    [Fact]
    public async Task GuardarNuevoMandaElPuestoYElDepartamento()
    {
        SeguridadServiceStub servicio = new([], [.. Catalogo]);
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "l@b.com", password: "clave123", rol: 1);
        Control<UITextBox>(form, "txtDetTituloCargo").Text = "Coordinador de ventas";
        Control<UITextBox>(form, "txtDetDepartamento").Text = "Ventas";

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioCreado!.TituloCargo.Should().Be("Coordinador de ventas");
        servicio.UsuarioCreado.Departamento.Should().Be("Ventas");
    }

    [Fact]
    public async Task PuestoYAreaVaciosSeGuardanComoNuloYNoComoGuion()
    {
        // El guion es solo visual de "sin valor": si se guardara literal, la base
        // tendria usuarios con un "—" de título.
        SeguridadServiceStub servicio = new([], [.. Catalogo]);
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "l@b.com", password: "clave123", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioCreado!.TituloCargo.Should().BeNull();
        servicio.UsuarioCreado.Departamento.Should().BeNull();
    }

    [Fact]
    public async Task GuardarNuevoVuelveAConsultaYTraeElUsuarioAlListado()
    {
        SeguridadServiceStub servicio = new([], [.. Catalogo]);
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "l@b.com", password: "clave123", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeTrue("tras guardar vuelve a consulta");
        Boton(form, "btnGuardarUsuario").Visible.Should().BeFalse();
        Control<DataGridView>(form, "gridUsuarios").Rows.Count.Should().Be(1, "el alta recarga el listado");
    }

    [Fact]
    public async Task SiElAltaFallaSeQuedaEnEdicionConLoEscrito()
    {
        // Perder lo escrito obliga a teclearlo de nuevo: el usuario lo corrige, no lo rehace.
        SeguridadServiceStub servicio = new([], [.. Catalogo]) { FallarAlCrear = true };
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "Luis Gomez", email: "l@b.com", password: "clave123", rol: 1);

        Click(Boton(form, "btnGuardarUsuario"));

        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeFalse("no se sale del modo escritura");
        Control<UITextBox>(form, "txtDetNombre").Text.Should().Be("Luis Gomez", "lo escrito se queda en pantalla");
        Boton(form, "btnGuardarUsuario").Visible.Should().BeTrue("se puede reintentar");
    }

    // ─────────────────────────────────────────────────────────────
    // Edición
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task GuardarEdicionMandaElUsuarioConSuIdYElRolComoObjeto()
    {
        // UpdateUsuarioAsync lee usuario.Roles (List<Role>), no roleIds: es la otra
        // traducción que la del alta. Aunque sea de un elemento, es una lista.
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        Control<UITextBox>(form, "txtDetNombre").Text = "Ana Gomez";
        Control<UIComboBox>(form, "cboRoles").SelectedIndex = 2;

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado.Should().NotBeNull();
        servicio.UsuarioActualizado!.UserId.Should().Be(1, "sin el id el UPDATE no encuentra la fila");
        servicio.UsuarioActualizado.NombreCompleto.Should().Be("Ana Gomez");
        servicio.UsuarioActualizado.Roles.Select(r => r.RoleId).Should().Equal(3);
    }

    [Fact]
    public async Task GuardarEdicionPuedeCambiarElLogin()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        // Con espacios de sobra: el login se recorta antes de mandarlo.
        Control<UITextBox>(form, "txtDetUsuario").Text = "  ana.perez  ";

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado!.Username.Should().Be("ana.perez");
        form.Avisos.Should().NotContain(a => a.Contains("ya existe"));

        // El listado se vuelve a pedir tras guardar: el detalle tiene que quedar
        // con el login nuevo y otra vez bloqueado, ya en modo consulta.
        Control<UITextBox>(form, "txtDetUsuario").ReadOnly.Should().BeTrue("vuelve a consulta");
        Control<UITextBox>(form, "txtDetUsuario").Text.Should().Be("ana.perez");
    }

    [Fact]
    public async Task EditarSinLoginNoGuardaYAvisa()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        Control<UITextBox>(form, "txtDetUsuario").Text = "   ";

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado.Should().BeNull("sin login no hay nada que escribir en la base");
        form.Avisos.Should().Contain(a => a.Contains("nombre de usuario"));
        Control<UITextBox>(form, "txtDetUsuario").ReadOnly.Should().BeFalse("se queda en edicion para corregir");
    }

    [Fact]
    public async Task EditarConElLoginDeOtroUsuarioNoGuarda()
    {
        Usuario[] padron = [Admin(), Mostrador()];
        SeguridadServiceStub servicio = new(
            usuarios: [.. padron],
            roles: [.. Catalogo],
            rolesPorUsuario: RolesDe(padron));

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        // La columna es UNIQUE: sin esta comprobacion el UPDATE revienta en la base
        // con un mensaje que no le dice a nadie qué hacer.
        Control<UITextBox>(form, "txtDetUsuario").Text = "MOSTRADOR";

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado.Should().BeNull();
        form.Avisos.Should().Contain(a => a.Contains("ya existe"));
        Control<UITextBox>(form, "txtDetUsuario").ReadOnly.Should().BeFalse("se queda en edicion para corregir");
    }

    [Fact]
    public async Task EditarConservandoSuPropioLoginNoEsDuplicado()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        // El usuario que se edita se excluye del calculo: si no, cualquier guardado
        // diria que su propio login ya existe.
        Control<UITextBox>(form, "txtDetNombre").Text = "Ana Perez Gomez";

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado.Should().NotBeNull();
        form.Avisos.Should().NotContain(a => a.Contains("ya existe"));
    }

    [Fact]
    public async Task GuardarEdicionTomaElInterruptorDeActivo()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        Control<UISwitch>(form, "swDetActivo").Active = false;

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado!.Activo.Should().BeFalse();
    }

    [Fact]
    public async Task EditarConElGridMovidoGuardaSobreElUsuarioOriginal()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin(), Mostrador()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1], [2] = [2] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        // El usuario se mueve por el listado mientras escribe.
        ElegirFila(form, 1);

        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado!.UserId.Should().Be(1,
            "el id se captura al entrar a editar, no se lee de la fila activa");
    }

    [Fact]
    public async Task SinFilaElegidaEditarNoEstaHabilitado()
    {
        // Con un usuario cargado el grid elige la primera fila solo, asi que Editar si
        // queda habilitado. El caso sin ninguna fila es el que no tiene a quien editar.
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();

        Boton(form, "btnEditarUsuario").Enabled.Should().BeFalse();
    }

    // ─────────────────────────────────────────────────────────────
    // Permisos: el módulo no los tiene
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task SinPermisoDeCrearElAltaNoSeCorta()
    {
        // Usuarios no tiene permisos propios (mismo criterio que Vendedores): no se le
        // pide Usuarios:Crear, asi que quitarlo de la sesion no corta el alta.
        SeguridadServiceStub servicio = new([], [.. Catalogo]);
        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();

        SesionActual.Permisos.Remove("Usuarios:Crear");

        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "Luis", email: "l@b.com", password: "clave123", rol: 1);
        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioCreado.Should().NotBeNull("el alta llega al servicio sin comprobar permisos");
        form.Avisos.Should().NotContain(a => a.Contains("permiso"));
    }

    [Fact]
    public async Task SinPermisoDeEditarElGuardadoNoSeCorta()
    {
        SeguridadServiceStub servicio = new(
            usuarios: [Admin()],
            roles: [.. Catalogo],
            rolesPorUsuario: new Dictionary<int, List<int>> { [1] = [1] });

        using FrmBajoPrueba form = Form(servicio);
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));

        SesionActual.Permisos.Remove("Usuarios:Editar");
        Click(Boton(form, "btnGuardarUsuario"));

        servicio.UsuarioActualizado.Should().NotBeNull("el modulo no pide permisos para editar");
        form.Avisos.Should().NotContain(a => a.Contains("permiso"));
    }

    // ─────────────────────────────────────────────────────────────
    // Cancelar
    // ─────────────────────────────────────────────────────────────

    [Fact]
    public async Task CancelarDescartaSinPreguntar()
    {
        // Cancelar ya no abre ninguna confirmacion: vuelve a consulta y tira lo
        // escrito sin ningun dialogo en medio.
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "A medias", email: "", password: "", rol: null);

        Click(Boton(form, "btnCancelarUsuario"));

        form.Avisos.Should().BeEmpty("no debe abrirse ningun dialogo al cancelar");
        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeTrue("vuelve a consulta");
        Control<UITextBox>(form, "txtDetUsuario").Text.Should().Be("—", "lo escrito se tiró");
    }

    [Fact]
    public async Task CancelarDescartaLoEscritoYVuelveAConsulta()
    {
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnEditarUsuario"));
        Control<UITextBox>(form, "txtDetNombre").Text = "Tipeado a medias";

        Click(Boton(form, "btnCancelarUsuario"));

        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeTrue();
        Control<UITextBox>(form, "txtDetNombre").Text.Should().Be("Ana Perez", "vuelve el valor de la fila");
    }

    [Fact]
    public async Task CancelarEnNuevoDejaElDetalleVacio()
    {
        // Sin filas de fondo, descartar el alta tiene que dejar el detalle vacio.
        using FrmBajoPrueba form = FormCargado();
        await form.InitializeAsync();
        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "A medias", email: "", password: "", rol: null);

        Click(Boton(form, "btnCancelarUsuario"));

        Control<UITextBox>(form, "txtDetUsuario").Text.Should().Be("—", "el alta se descartó entera");
    }

    [Fact]
    public async Task CancelarEnNuevoVuelveALaFilaQueEstabaElegida()
    {
        // Descartar el alta no tiene que dejar la pantalla vacia si ya habia alguien
        // elegido antes: se vuelve a esa persona, que es lo que el usuario estaba viendo.
        using FrmBajoPrueba form = FormCargado(Admin());
        await form.InitializeAsync();
        ElegirFila(form, 0);
        Click(Boton(form, "btnNuevoUsuario"));
        Escribir(form, usuario: "nuevo", nombre: "A medias", email: "", password: "", rol: null);

        Click(Boton(form, "btnCancelarUsuario"));

        Control<UITextBox>(form, "txtDetUsuario").Text.Should().Be("admin");
        Control<UITextBox>(form, "txtDetNombre").ReadOnly.Should().BeTrue();
    }

    // ─────────────────────────────────────────────────────────────
    // Ayudas
    // ─────────────────────────────────────────────────────────────

    private static void Escribir(
        FrmBajoPrueba form,
        string usuario,
        string nombre,
        string email,
        string password,
        int? rol)
    {
        Control<UITextBox>(form, "txtDetUsuario").Text = usuario;
        Control<UITextBox>(form, "txtDetNombre").Text = nombre;
        Control<UITextBox>(form, "txtDetEmail").Text = email;
        Control<UITextBox>(form, "txtPassword").Text = password;

        // El combo guarda el NOMBRE del rol; el id se busca por posicion, asi que se
        // elige el item que corresponde al id pedido. Si el combo esta vacio —el
        // formulario no llego a modo escritura porque faltava un permiso— no se elige
        // nada: esta ayuda no debe ser la que hace fallar al test por otra causa.
        UIComboBox combo = Control<UIComboBox>(form, "cboRoles");
        combo.SelectedIndex = -1;

        if (rol is null || combo.Items.Count == 0)
        {
            return;
        }

        int? indice = combo.Items.Cast<object>()
            .Select((nombre, i) => (Nombre: nombre.ToString(), Indice: i))
            .Where(x => x.Nombre == NombreDelRol(rol.Value))
            .Select(x => (int?)x.Indice)
            .FirstOrDefault();

        if (indice is not null)
        {
            combo.SelectedIndex = indice.Value;
        }
    }

    /// <summary>Nombre del rol del catalogo de prueba que tiene ese RoleId.</summary>
    private static string NombreDelRol(int roleId) => Catalogo.First(r => r.RoleId == roleId).Nombre;
}