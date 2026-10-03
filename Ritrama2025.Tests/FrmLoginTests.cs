using FluentAssertions;
using Ritrama2025.Forms;
using Ritrama2025.Models;
using Ritrama2025.Services.SeguridadService;
using Ritrama2025.Tests.Stubs;
using Sunny.UI;
using Xunit;

namespace Ritrama2025.Tests;

/// <summary>
/// Login de FrmLogin contra un servicio en memoria: valida el usuario y la clave con
/// lo que devolveria la base, no deja entrar a los desactivados, consume los intentos
/// y deja la sesion lista (usuario + permisos) para que el Main pinte la barra lateral.
/// Toca la UI, asi que los dialogos van sustituidos por grabaciones: un MessageBox de
/// verdad dentro de una prueba lo dejaria colgado.
/// </summary>
[Trait("Categoria", "Unit")]
public class FrmLoginTests
{
    private const string Clave = "ClaveDePrueba.1";

    /// <summary>Formulario con los avisos grabados en vez de abrir dialogos.</summary>
    private sealed class FrmLoginSinDialogos(ISeguridadService servicio) : FrmLogin(servicio)
    {
        public List<string> Avisos { get; } = [];

        protected override void MostrarAviso(string mensaje, MessageBoxIcon icono = MessageBoxIcon.Warning)
            => Avisos.Add(mensaje);
    }

    private static Usuario UsuarioActivo(int id = 1, string username = "nelson") => new()
    {
        UserId = id,
        Username = username,
        PasswordHash = BCrypt.Net.BCrypt.HashPassword(Clave),
        NombreCompleto = "Nelson Pino",
        Activo = true,
        PrimerLogin = false,
        FechaCreacion = new DateTime(2026, 3, 14, 9, 30, 0)
    };

    private static FrmLoginSinDialogos Formulario(SeguridadLoginStub servicio)
        => new(servicio);

    private static void Escribir(FrmLogin form, string usuario, string clave)
    {
        ((UITextBox)form.Controls.Find("txt_username", true).Single()).Text = usuario;
        ((UITextBox)form.Controls.Find("txt_password", true).Single()).Text = clave;
    }

    private static string IntentosEnPantalla(FrmLogin form)
        => ((UILabel)form.Controls.Find("lbl_intentos", true).Single()).Text;

    [Fact]
    public async Task UsuarioYClaveCorrectosAbrenLaSesion()
    {
        Usuario usuario = UsuarioActivo();
        SeguridadLoginStub servicio = new(usuario) { Permisos = ["Usuarios:Ver"] };
        FrmLoginSinDialogos form = Formulario(servicio);

        Escribir(form, "  NELSON  ", Clave);
        await form.RealizarLogin();

        // La sesion queda montada: es lo que Program.cs copia a SesionActual y lo que
        // el Main pinta en la barra lateral del sidebar.
        form.UsuarioAutenticado.Should().BeSameAs(usuario);
        form.Permisos.Should().Equal("Usuarios:Ver");
        form.DialogResult.Should().Be(DialogResult.OK, "el formulario se acepta");
        form.Avisos.Should().BeEmpty("un login bueno no avisa");

        // El usuario llega recortado a la base: los espacios alrededor no deberian
        // convertir un credito en un usuario inexistente. La caja (NELSON vs nelson)
        // la normaliza la consulta en SQL, no el formulario.
        servicio.UsernameConsultado.Should().Be("NELSON");
        servicio.UsuariosAuditados.Should().Equal(usuario.UserId);
        servicio.ResultadosAuditados.Should().Equal(true);
        servicio.LlamadasPermisos.Should().Be(1);
    }

    [Fact]
    public async Task ClaveIncorrectaNoEntraYRestaUnIntento()
    {
        SeguridadLoginStub servicio = new(UsuarioActivo());
        FrmLoginSinDialogos form = Formulario(servicio);

        Escribir(form, "nelson", "clave-equivocada");
        await form.RealizarLogin();

        form.UsuarioAutenticado.Should().BeNull("la clave no coincide con la de la base");
        form.DialogResult.Should().Be(DialogResult.None, "no se acepta el formulario");
        form.Avisos.Should().ContainSingle().Which.Should().Contain("Intentos restantes: 4");
        IntentosEnPantalla(form).Should().Be("Intentos restantes: 4");

        // Auditoria de fallo con user_id NULL: la columna es nullable con FK a usuarios.
        servicio.UsuariosAuditados.Should().Equal((int?)null);
        servicio.ResultadosAuditados.Should().Equal(false);
        servicio.LlamadasPermisos.Should().Be(0, "sin sesion no se piden permisos");
    }

    [Fact]
    public async Task UsuarioDesactivadoNoPuedeEntrar()
    {
        // Clave CORRECTA a proposito: lo unico que tiene que frenarlo es el estado.
        Usuario desactivado = UsuarioActivo();
        desactivado.Activo = false;
        SeguridadLoginStub servicio = new(desactivado);
        FrmLoginSinDialogos form = Formulario(servicio);

        Escribir(form, "nelson", Clave);
        await form.RealizarLogin();

        form.UsuarioAutenticado.Should().BeNull("el usuario esta desactivado");
        form.DialogResult.Should().Be(DialogResult.None);
        form.Avisos.Should().ContainSingle();

        // El aviso es el mismo que el de una clave mala: no se revela que la cuenta
        // existe y esta suspendida.
        string avisoInactivo = form.Avisos[0];

        SeguridadLoginStub otro = new(UsuarioActivo());
        FrmLoginSinDialogos otroForm = Formulario(otro);
        Escribir(otroForm, "nelson", "clave-equivocada");
        await otroForm.RealizarLogin();

        avisoInactivo.Should().Be(otroForm.Avisos[0],
            "desactivado y clave mala deben decir exactamente lo mismo");
        servicio.UsuariosAuditados.Should().Equal((int?)null);
        servicio.ResultadosAuditados.Should().Equal(false);
    }

    [Fact]
    public async Task UsuarioInexistenteNoPuedeEntrar()
    {
        SeguridadLoginStub servicio = new(UsuarioActivo());
        FrmLoginSinDialogos form = Formulario(servicio);

        Escribir(form, "nadie", Clave);
        await form.RealizarLogin();

        form.UsuarioAutenticado.Should().BeNull();
        form.Avisos.Should().ContainSingle().Which.Should().Contain("erróneos");
        servicio.UsuariosAuditados.Should().Equal((int?)null);
        servicio.LlamadasPermisos.Should().Be(0);
    }

    [Fact]
    public async Task CamposVaciosAvisanSinPreguntarALaBase()
    {
        SeguridadLoginStub servicio = new(UsuarioActivo());
        FrmLoginSinDialogos form = Formulario(servicio);

        Escribir(form, "   ", "");
        await form.RealizarLogin();

        form.Avisos.Should().ContainSingle().Which.Should().Contain("usuario y contraseña");
        servicio.LlamadasLogin.Should().Be(0, "no se consulta la base con campos vacios");
        servicio.UsuariosAuditados.Should().BeEmpty();
    }

    [Fact]
    public async Task AlQuintoFalloSeCierraLaAplicacion()
    {
        SeguridadLoginStub servicio = new(UsuarioActivo());
        FrmLoginSinDialogos form = Formulario(servicio);

        for (int intento = 1; intento <= 5; intento++)
        {
            Escribir(form, "nelson", "clave-equivocada");
            await form.RealizarLogin();
        }

        form.Avisos.Should().HaveCount(5, "cuatro avisos de intento y uno del cierre");
        form.Avisos[^1].Should().Contain("Agotó todos los intentos");
        form.DialogResult.Should().Be(DialogResult.Cancel, "al agotar intentos se cierra");
        form.UsuarioAutenticado.Should().BeNull();
        servicio.UsuariosAuditados.Should().HaveCount(5);
        servicio.UsuariosAuditados.Should().OnlyContain(id => !id.HasValue);
    }

    [Fact]
    public void OjoDeLaClaveAlternaElEnmascarado()
    {
        SeguridadLoginStub servicio = new(UsuarioActivo());
        FrmLoginSinDialogos form = Formulario(servicio);

        // El clic solo responde si el formulario esta visible (mismo criterio que
        // CrearFormularioVisible de FrmUsuarios): sin Show() se pierde en silencio.
        form.TopLevel = false;
        form.Show();
        Application.DoEvents();

        UITextBox password = (UITextBox)form.Controls.Find("txt_password", true).Single();
        UIButton ojo = (UIButton)form.Controls.Find("btn_ojo", true).Single();

        password.PasswordChar.Should().Be('*');

        ojo.PerformClick();
        password.PasswordChar.Should().Be('\0', "al mostrar la clave el caracter se vacia");

        ojo.PerformClick();
        password.PasswordChar.Should().Be('*');
    }
}
