using System.Data;
using Ritrama2025.Core;
using Ritrama2025.Helpers;
using Ritrama2025.Models;
using Ritrama2025.Services.ExportData;
using Ritrama2025.Services.ProduccionService;
using Ritrama2025.Services.ReportsService.ReportsService;
using Ritrama2025.Services.SeguridadService;
using Sunny.UI;

namespace Ritrama2025.Forms
{
    /// <summary>
    /// Gestion de Usuarios. Rediseño 30/70 sobre el esquema de FrmProductos: barra
    /// de acciones a nivel de pagina (Nuevo / Editar), panel izquierdo con buscador,
    /// grid de cuatro columnas y cuadro de resumen, panel derecho con la pagina de
    /// detalle. El detalle y la carga de datos se definen en los pasos siguientes.
    ///
    /// La version previa (listado en grid, captura con usuario/nombre/email/activo
    /// y check list de roles) se recupera con:
    ///   git checkout -- Forms/FrmUsuarios.cs Forms/FrmUsuarios.Designer.cs
    /// </summary>
    public partial class FrmUsuarios : UIForm, IAsyncFormLoad, IFormTemaClaro
    {
        private readonly ISeguridadService _seguridadService;

        /// <summary>Servicio de exportacion a Excel, para la hoja de usuarios.</summary>
        private readonly IExportDataService _exportDataService;

        /// <summary>Servicio de reportes, para abrir el catalogo en el visor (ReportsViewer).</summary>
        private readonly IReportsService _reportsService;

        /// <summary>Listado completo, sin filtrar. Es la fuente del buscador.</summary>
        private List<Usuario> _usuarios = [];

        /// <summary>
        /// RoleId de cada usuario, ya resueltos. Sin este cache, cada tecla del buscador
        /// volveria a preguntar los roles de cada usuario.
        ///
        /// Se guardan los IDS y no los nombres a proposito: el filtro de rol tiene que
        /// comparar el mismo dato que asigna el combo del alta. Con los nombres el filtro
        /// compararia texto contra texto y un rol renombrado dejaria de encontrar a sus
        /// usuarios sin que se note.
        /// </summary>
        private Dictionary<int, List<int>> _idsRolPorUsuario = [];

        /// <summary>
        /// Catalogo de roles, tal cual lo devuelve la tabla roles. Se usa para resolver
        /// el nombre que se muestra en el listado y en la consulta, y para armar los
        /// radios del filtro de rol.
        /// </summary>
        private List<Role> _catalogoRoles = [];

        /// <summary>
        /// Roles que se ofrecen en el combo, en el MISMO orden que sus items. El indice
        /// del combo es el indice de esta lista, asi que el RoleId no se puede leer del
        /// item (que es un string). Solo los activos: un rol dado de baja no se asigna.
        /// </summary>
        private List<Role> _rolesDisponibles = [];

        /// <summary>
        /// Filtro de rol bajo el buscador: un panel con los radios. Se arma por codigo
        /// (mismo criterio que CrearFiltroCategoria en Clientes) y no en el disenador
        /// porque los radios SON los roles activos de la base: si se da de alta o de baja
        /// un rol, el filtro tiene que cambiar solo, sin volver a compilar.
        /// </summary>
        private Panel pnlFiltroRol = null!;

        /// <summary>Fila donde viven los radios, dentro de <see cref="pnlFiltroRol"/>.</summary>
        private FlowLayoutPanel _filaRadiosRol = null!;

        /// <summary>
        /// Radios del filtro, en orden. El primero es "Todos". El Tag de cada uno es el
        /// RoleId en texto, o null en "Todos" — es lo que compara el filtro contra
        /// <see cref="_idsRolPorUsuario"/>.
        /// </summary>
        private readonly List<RadioButton> _radiosRol = [];

        /// <summary>Modos del detalle. Consulta es el estado de partida y de reposo.</summary>
        private enum ModoFormulario
        {
            Consulta,
            Nuevo,
            Editar
        }

        private ModoFormulario _modo = ModoFormulario.Consulta;

        /// <summary>
        /// Usuario que se esta editando. Vive en su propia propiedad y no se lee de la
        /// fila activa porque durante la edicion el usuario puede moverse por el grid:
        /// sin esto se guardaria sobre otro.
        /// </summary>
        private int? _idEnEdicion;

        /// <summary>
        /// Evita que un doble clic en Guardar dispare dos escrituras: el segundo llega
        /// mientras el primero sigue esperando al servicio.
        /// </summary>
        private bool _guardando;

        private bool EsEditable => _modo is ModoFormulario.Nuevo or ModoFormulario.Editar;

        /// <summary>Verde de las barras y de los estados activos. El de Productos.</summary>
        private static readonly Color Verde = Color.FromArgb(110, 190, 40);

        /// <summary>Verde oscuro de los rotulos de titulo. El de Productos.</summary>
        private static readonly Color VerdeTitulo = Color.FromArgb(60, 110, 20);

        /// <summary>Gris de la letra de rotulos y campos. El de Productos.</summary>
        private static readonly Color GrisTexto = Color.FromArgb(64, 64, 64);

        /// <summary>
        /// Borde suave de los campos. El estilo verde global deja el verde puro
        /// (110,190,40), que compite con el texto: en Productos el borde es este.
        /// </summary>
        private static readonly Color ColorBordeCampo = Color.FromArgb(180, 210, 180);

        /// <summary>Verde del rotulo "USUARIOS" sobre la banda verde de cabecera.</summary>
        private static readonly Color VerdeCabecera = Color.FromArgb(70, 140, 25);

        /// <summary>
        /// Fondo de las filas de usuarios desactivados: rojo vivo #D02020, el mismo que
        /// usa Productos, para que el estado se resalte de un vistazo en el listado sin
        /// tener que leer la columna ni abrir el detalle.
        /// </summary>
        private static readonly Color FondoDesactivado = Color.FromArgb(208, 32, 32);

        /// <summary>
        /// Letra de una fila desactivada: rosa claro, mas claro que el texto normal, para
        /// que se lea sobre <see cref="FondoDesactivado"/> y no se funda con el rojo.
        /// </summary>
        private static readonly Color TextoDesactivado = Color.FromArgb(255, 214, 214);

        /// <summary>
        /// Fondo de la fila desactivada cuando esta seleccionada. La seleccion del modulo
        /// es negra y ese negro taparia justo el rojo de un usuario de baja: como para
        /// ver el estado hay que seleccionarlo, aqui la seleccion va en rojo oscuro con la
        /// misma letra clara, de modo que el fondo rojo sigue notandose.
        /// </summary>
        private static readonly Color SeleccionDesactivado = Color.DarkRed;

        /// <summary>
        /// Fila del detalle que ocupa la contraseña, la segunda: va justo debajo del
        /// usuario. Se colapsa a alto 0 cuando el campo no se ve: en un TableLayoutPanel
        /// ocultar un control NO colapsa su fila, y sin esto quedaba un hueco de 38 px
        /// entre el usuario y el nombre.
        /// </summary>
        private const int FilaPassword = 2;

        /// <summary>Alto de una fila del detalle, el que deja el disenador.</summary>
        private const int AltoFilaDetalle = 38;

        /// <summary>
        /// Caracter con el que se tapa la contraseña. Vive aca y no en el disenador
        /// porque lo alterna el botón del ojo: si estuviera en el .Designer.cs el
        /// enmascarado dependeria de dos lugares y se desincronizarían.
        /// </summary>
        private const char CaracterClave = '•';

        /// <summary>
        /// Modulo al que pertenecen los permisos de esta pantalla: el mismo prefijo que
        /// usan las entradas de la tabla de permisos ("Usuarios:Ver", "Usuarios:Crear").
        /// </summary>
        private const string ConstanteModuloUsuarios = "Usuarios";

        /// <summary>
        /// Si la clave se está escribiendo a la vista. Al salir del alta vuelve a false:
        /// dejarla mostrada en el siguiente alta abriría la del usuario anterior.
        /// </summary>
        private bool _mostrarClave;

        public FrmUsuarios(ISeguridadService seguridadService, IExportDataService exportDataService, IReportsService reportsService)
        {
            ArgumentNullException.ThrowIfNull(seguridadService);
            ArgumentNullException.ThrowIfNull(exportDataService);
            ArgumentNullException.ThrowIfNull(reportsService);
            _seguridadService = seguridadService;
            _exportDataService = exportDataService;
            _reportsService = reportsService;

            InitializeComponent();
            Text = "Usuarios";

            // Estilo VERDE de SunnyUI, igual que Clientes / Proveedores / Pedidos.
            // Debe ir antes de que la UI exista para que UIStyleManager tome el mando.
            components ??= new System.ComponentModel.Container();
            _ = new UIStyleManager(components)
            {
                Style = UIStyle.Green,
                GlobalFont = true,
                GlobalFontName = "JetBrains Mono"
            };

            AplicarTemaVerde();

            // El aspecto propio del detalle, en el mismo orden que FrmProductos: el
            // UIStyleManager del constructor y el Style=Green del tema acaban de pisar
            // los colores del disenador, y esto va despues para quedar por encima. Va en
            // el constructor y no en AplicarTemaVerde porque ReaplicarTema se llama al
            // embeber el modulo en el Main y, alli, Productos tampoco lo repite: el
            // estilo global vuelve a mandar sobre el detalle en los dos modulos.
            EstilarCamposDetalle();

            // SunnyUI deja los rótulos del detalle con un alto propio (173 px medidos en
            // una celda de 38) y, como el texto va centrado, la palabra "Activo" cae muy
            // por debajo del interruptor de 29 px de al lado. Se corrige SOLO el alto.
            //
            // Va en Load porque el alto lo pone el estilo al cargar, ya después de este
            // constructor. Y NO se vuelve a llamar AplicarTemaVerde desde aqui a
            // proposito: reaplicar el tema entero en Load cambia el aspecto del
            // formulario completo (fondos y colores de todos los controles).
            Load += (_, _) => AjustarAltoDeRotulos();

            // Editar se habilita y el detalle se refresca con los dos eventos: el
            // SelectionChanged se difiere al primer pintado, el CurrentCellChanged
            // dispara sincrono — mismo criterio que FrmClientes y FrmProductos.
            gridUsuarios.SelectionChanged += (_, _) => AlCambiarLaFilaElegida();
            gridUsuarios.CurrentCellChanged += (_, _) => AlCambiarLaFilaElegida();

            // Filas de los usuarios de baja en rojo, al estilo de Productos. Va por
            // CellFormatting y no al pintar el DataTable porque el grid se reordena al
            // ordenar una columna y ahi el pintado por filas se quedaba detras.
            gridUsuarios.CellFormatting += GridUsuarios_CellFormatting;

            // Filtrado en vivo sobre el listado ya cargado: no se vuelve a la base.
            txtBuscar.TextChanged += (_, _) => AplicarFiltroBusqueda();

            // Filtro de rol debajo del buscador. Se crea aca y no en el disenador porque
            // sus radios salen del catalogo de roles, que todavia no esta cargado.
            CrearFiltroRol();

            // La barra del disenador deja los botones en tamano fijo (92x36) con
            // ImageScaling = None, y los iconos de Resources vienen en tamanos dispares
            // (el de Nuevo es de 32 px y el de Cancelar de 24). Con eso el icono se sale
            // del alto del boton. Se normaliza con el helper que ya usan MateriaPrima y
            // OrdenesCompra: icono a 24x24, SizeToFit y AutoSize por boton.
            Helpers.ToolStripTheme.Ajustar(barraHerramientas);
            Helpers.ToolStripTheme.AjustarAlto(barraHerramientas);

            // La barra se cablea aqui y no en el disenador: los handlers son privados
            // del formulario y el Visual Studio no puede engancharlos. Mismo criterio
            // que FrmProductos.
            btnNuevoUsuario.Click += BtnNuevoUsuario_Click;
            btnEditarUsuario.Click += BtnEditarUsuario_Click;
            btnExportarUsuario.Click += BtnExportarUsuario_Click;
            btnReporteUsuario.Click += BtnReporteUsuario_Click;
            btnGuardarUsuario.Click += BtnGuardarUsuario_Click;
            btnCancelarUsuario.Click += BtnCancelarUsuario_Click;

            // El ojo de la contraseña. El Click se cablea aca y no en el disenador por lo mismo
            // que la barra: el handler es privado del formulario.
            btnOjoPassword.Click += (_, _) => AlternarVisibilidadClave();

            // El detalle arranca bloqueado. Sin esto, los textbox serian escribibles
            // antes de que FormManager llame a InitializeAsync.
            AplicarModo(ModoFormulario.Consulta);
        }

        /// <summary>Reaplica el tema verde para pisar el UIStyleManager global de Main.</summary>
        public void ReaplicarTema() => AplicarTemaVerde();

        /// <summary>Fila elegida del listado: habilita Editar y refresca el detalle.</summary>
        private void AlCambiarLaFilaElegida()
        {
            ActualizarBotonesBarra();
            RefrescarDetalle();
        }

        // ─────────────────────────────────────────────────────────────
        // Modo del formulario
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Unico metodo que decide que se puede escribir. ReadOnly, Visible y Enabled se
        /// tocan SOLO aqui: con la regla duplicada, VolcarDetalle o PintarEstado se
        /// ejecutan despues y dejan el control editable otra vez. Mismo criterio que
        /// FrmProductos.
        /// </summary>
        private void AplicarModo(ModoFormulario modo)
        {
            _modo = modo;
            bool editable = EsEditable;
            bool esNuevo = modo is ModoFormulario.Nuevo;

            // El ID no se escribe en ningun modo: en el alta todavia no existe, y en
            // edicion lo decide la base. Se declara aqui para que la regla de que
            // ReadOnly se toca solo en este metodo siga siendo cierta.
            txtDetId.ReadOnly = true;

            // El username se puede escribir en Alta y en Edicion: ahora esta en el
            // UPDATE, y el filtro por user_id de la clausula WHERE sigue mandando. La
            // comprobacion de que no pise el de otro esta en ErrorDeLogin, igual que en
            // el alta, para que no salga el error crudo del UNIQUE de la base.
            txtDetUsuario.ReadOnly = !editable;
            txtDetNombre.ReadOnly = !editable;
            txtDetEmail.ReadOnly = !editable;
            txtDetTituloCargo.ReadOnly = !editable;
            txtDetDepartamento.ReadOnly = !editable;

            // Guardar y Cancelar solo existen escribiendo: en consulta no hay nada que
            // guardar, asi que se ocultan en vez de mostrarse deshabilitados.
            btnGuardarUsuario.Visible = editable;
            btnCancelarUsuario.Visible = editable;

            // Exportar se oculta al escribir por el mismo motivo que Nuevo y Editar en
            // Productos: un clic mientras se captura tiraria el borrador a un fichero
            // que nadie pidio. En consulta vuelve a estar.
            btnExportarUsuario.Visible = !editable;

            // El reporte se oculta al escribir por el MISMO criterio que Exportar: mientras
            // se captura un usuario el padron de la base es el de antes del guardado, y abrir
            // el visor a mitad de una edicion muestra datos que estan a punto de cambiar.
            btnReporteUsuario.Visible = !editable;

            // La contraseña solo se pone al dar de alta: UpdateUsuarioAsync no la toca,
            // y el cambio de clave va por ResetPasswordAsync.
            // Se oculta el PANEL, no el textbox: asi el botón del ojo se van con él.
            pnlPassword.Visible = esNuevo;
            txtPassword.ReadOnly = !editable;
            lblCapPassword.Visible = esNuevo;
            btnOjoPassword.Enabled = esNuevo;
            AplicarEnmascarado();

            // Y la fila se colapsa con ella. Ocultar los dos controles NO alcanza: el alto
            // lo manda el RowStyle, no la visibilidad, y por eso la fila se quedaba
            // tapiando el espacio entre los datos de arriba y los de abajo.
            tlpDetalle.RowStyles[FilaPassword].Height = modo is ModoFormulario.Nuevo ? AltoFilaDetalle : 0;

            // El alto del rotulo lo manda su fila, no su propio Height: al abrir o
            // colapsar la de la contraseña hay que volver a medir. Load dejo el rotulo
            // en 0 px mientras la fila valia 0, y sin esta llamada seguia en 0 pese a
            // que la fila se abria: el campo se veia pero el titulo "Contraseña", no.
            AjustarAltoDeRotulos();

            // Rol: el texto se ve en consulta y el combo al escribir. lblCapRoles se queda
            // en los dos modos: hay rotulo siempre. El texto nunca se escribe.
            txtDetRoles.ReadOnly = true;
            txtDetRoles.Visible = !editable;
            cboRoles.Visible = editable;

            // Combo: ReadOnly solo impide escribir, el desplegable se sigue abriendo con
            // el raton (ver FrmClientes), asi que en consulta además se deshabilita.
            cboRoles.ReadOnly = !editable;
            cboRoles.Enabled = editable;

            // Activo: el switch de SunnyUI es el unico control del estado, en consulta y
            // en edicion. Antes habia un textbox con "Sí"/"No" al consultar y el
            // switch al escribir, y depender de cuál se ve segun el modo hacia que
            // en un modo se viera un estado y en el otro el mismo estado.
            // En el alta NO se muestra porque CreateUsuarioAsync tiene activo=1 en
            // el SQL y no hay parametro que lo envie: mostrarlo daria a entender lo
            // contrario.
            // ReadOnly y no Enabled=false: permite pintar Active por codigo.
            lblCapActivo.Visible = !esNuevo;
            swDetActivo.Visible = !esNuevo;
            swDetActivo.ReadOnly = !editable;

            // En el alta no se muestra la fecha de creacion: no hay nada que el
            // usuario pueda decidir ahi, y enseñarla invita a pensar que si. Es de
            // solo lectura en cualquier modo: la escribe el servidor.
            lblCapFechaCreacion.Visible = !esNuevo;
            txtDetFechaCreacion.ReadOnly = true;
            txtDetFechaCreacion.Visible = !esNuevo;

            // El ID se ve siempre, tambien en el alta: ahi no se escribe pero se muestra el
            // consecutivo que le va a tocar. Nunca es editable en ningun modo —la base
            // lo decide— asi que ReadOnly se declara aca para que la regla de que
            // ReadOnly se toca solo en este metodo siga siendo cierta.
            lblCapId.Visible = true;
            txtDetId.Visible = true;

            // Y la barra se recalcula SIEMPRE al final de este metodo. Sin esta llamada
            // Nuevo y Editar se quedaban deshabilitados para siempre despues del primer
            // guardado: ActualizarBotonesBarra se habia ejecutado mientras el formulario
            // aun estaba en modo escritura, y al volver a consulta ya nadie los
            // volvia a habilitar. El sintoma era "el boton de guardar no funciona
            // despues de grabar un usuario": no se podia ni abrir el siguiente alta.
            ActualizarBotonesBarra();
        }

        /// <summary>
        /// Consecutivo que tendra el siguiente usuario: el UserId mas alto que ya existe
        /// mas uno. Sin usuarios arranca en 1.
        ///
        /// No reserva el numero: CreateUsuarioAsync lo toma de un IDENTITY y lo que se
        /// calcula aca es el que se veria si nadie mas crea un usuario en el medio. Por
        /// eso el alta termina repintando con el UserId real. Si dos administradores dan
        /// de alta al mismo tiempo, el segundo ve el numero con el que se guardo, no el
        /// que se le habia anunciado.
        /// </summary>
        private int SiguienteConsecutivo()
            => _usuarios.Count == 0 ? 1 : _usuarios.Max(u => u.UserId) + 1;

        /// <summary>
        /// Vuelca un usuario en el detalle. Se usa al entrar a Editar, no al elegir una
        /// fila: RefrescarDetalle pinta el valor de la fila activa, que mientras se
        /// edita puede ser otro usuario.
        /// </summary>
        private void VolcarDetalle(Usuario usuario)
        {
            txtDetId.Text = usuario.UserId.ToString();
            txtDetUsuario.Text = usuario.Username;
            txtDetNombre.Text = Texto(usuario.NombreCompleto);
            txtDetEmail.Text = Texto(usuario.Email);
            txtDetTituloCargo.Text = Texto(usuario.TituloCargo);
            txtDetDepartamento.Text = Texto(usuario.Departamento);
            PintarEstado(usuario.Activo);

            txtDetRoles.Text = TextoRol(usuario.UserId);

            txtDetFechaCreacion.Text = Fecha(usuario.FechaCreacion);
        }

        /// <summary>
        /// Pinta el estado en el interruptor. SunnyUI hace que UISwitch.Active ignore el
        /// setter cuando el control es ReadOnly, asi que se levanta ReadOnly solo mientras
        /// se asigna. Mismo truco que swEstado en Clientes / Proveedores / Vendedores.
        /// </summary>
        private void PintarEstado(bool activo)
        {
            swDetActivo.ReadOnly = false;
            swDetActivo.Active = activo;
            swDetActivo.ReadOnly = !EsEditable;
        }

        /// <summary>
        /// Llena el combo con los roles activos. Sin seleccion: es lo que se necesita para
        /// dar de alta, y en Editar la pone <see cref="SeleccionarRolDelUsuario"/>.
        ///
        /// El combo guarda el NOMBRE, no el RoleId, porque es lo que se muestra. El id se
        /// recupera por posicion con <see cref="RolElegido"/>, y por eso esta lista tiene
        /// que guardar el mismo orden que los items.
        /// </summary>
        private void CargarCatalogoDeRoles()
        {
            cboRoles.Items.Clear();

            _rolesDisponibles = [.. _catalogoRoles.Where(r => r.Activo)];

            foreach (Role rol in _rolesDisponibles)
            {
                cboRoles.Items.Add(rol.Nombre);
            }

            cboRoles.SelectedIndex = -1;

            // El catalogo recien cargado es el mismo que arma los radios del filtro. Va
            // aca y no suelto porque CargarCatalogoDeRoles es el unico punto por donde
            // pasa el catalogo, en los tres caminos que lo traen.
            if (CrearRadiosDeRol())
            {
                // El rol que estaba marcado ya no existe (se dio de baja): la lista en
                // pantalla todavia responde a un filtro que ya no existe.
                AplicarFiltroBusqueda();
            }
        }

        /// <summary>El rol elegido en el combo, o null si no hay ninguno.</summary>
        private Role? RolElegido()
            => cboRoles.SelectedIndex >= 0 ? _rolesDisponibles[cboRoles.SelectedIndex] : null;

        /// <summary>
        /// Pone en el combo el rol del usuario que se esta editando. Necesita el id, y el
        /// cache _idsRolPorUsuario guarda el rol de cada usuario del LISTADO: si el que se
        /// edita todavia no esta en la lista (o el cache se limpio por un fallo de carga),
        /// no hay de donde sacarlo y hay que preguntar. Es una llamada por sesion de
        /// edicion, no por tecla.
        ///
        /// Va async a proposito: con GetAwaiter().GetResult() el hilo de UI queda bloqueado
        /// esperando, y la continuacion del await del servicio se postea a ese mismo hilo
        /// bloqueado — deadlock. El resto del repo ya tiene la regla escrita (ver
        /// FrmDespacho / FrmPickingDespacho).
        ///
        /// El usuario tiene un solo rol. Si por una fila legacy tuviera mas, entra el
        /// primero: en el combo no hay forma de marcar dos.
        /// </summary>
        private async Task SeleccionarRolDelUsuario(int userId)
        {
            CargarCatalogoDeRoles();

            List<int> ids;

            try
            {
                ids = await _seguridadService.GetUsuarioRoleIdsAsync(userId);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al cargar los roles del usuario: " + ex.Message);
                return;
            }

            int indice = _rolesDisponibles.FindIndex(r => ids.Contains(r.RoleId));

            // El combo solo trae los roles ACTIVOS, pero un usuario puede tener asignado
            // uno que se dio de baja despues. Sin anadirlo aqui el combo queda en -1 y la
            // edicion se bloquea con "Elija el rol" sobre un campo que el usuario no ha
            // tocado: ese usuario no se podria editar nunca. Se anade solo para esta
            // sesion; CargarCatalogoDeRoles lo vuelve a armar desde el catalogo.
            if (indice < 0 && ids.Count > 0)
            {
                Role? asignado = _catalogoRoles.FirstOrDefault(r => ids.Contains(r.RoleId));

                if (asignado is not null)
                {
                    cboRoles.Items.Add(asignado.Nombre);
                    _rolesDisponibles.Add(asignado);
                    indice = _rolesDisponibles.Count - 1;
                }
            }

            cboRoles.SelectedIndex = indice;
        }

        /// <summary>
        /// Pasa el usuario de la fila elegida a los nueve textbox del detalle. Sin
        /// fila, todo queda en guion.
        ///
        /// El grid solo muestra cuatro columnas, asi que las propiedades que no estan
        /// en el (nombre, correo, primer login, fechas) se buscan en _usuarios por el
        /// UserId de la fila en vez de ampliar el DataTable: el listado se mantiene
        /// acotado a lo que se ve.
        /// </summary>
        private void RefrescarDetalle()
        {
            // Mientras se escribe NO se repinta: mover el listado pondria los datos de
            // otra persona encima de lo que se esta escribiendo, y lo que se guardara
            // seria una mezcla de las dos.
            if (EsEditable)
            {
                return;
            }

            Usuario? usuario = UsuarioDeLaFilaElegida();

            if (usuario is null)
            {
                LimpiarDetalle();
                return;
            }

            txtDetId.Text = usuario.UserId.ToString();
            txtDetUsuario.Text = Texto(usuario.Username);
            txtDetNombre.Text = Texto(usuario.NombreCompleto);
            txtDetEmail.Text = Texto(usuario.Email);
            txtDetTituloCargo.Text = Texto(usuario.TituloCargo);
            txtDetDepartamento.Text = Texto(usuario.Departamento);
            txtDetRoles.Text = TextoRol(usuario.UserId);

            // El switch se repinta tambien en consulta: ahora es el unico control del
            // estado y se ve siempre, asi que tiene que seguir a la fila elegida.
            PintarEstado(usuario.Activo);

            txtDetFechaCreacion.Text = Fecha(usuario.FechaCreacion);
        }

        /// <summary>El Usuario de la fila actual, o null si no hay fila o no se reconoce.</summary>
        private Usuario? UsuarioDeLaFilaElegida()
        {
            if (gridUsuarios.CurrentRow?.DataBoundItem is not DataRowView fila)
            {
                return null;
            }

            int userId = Convert.ToInt32(fila["UserId"]);

            return _usuarios.FirstOrDefault(u => u.UserId == userId);
        }

        /// <summary>
        /// Vacio o con espacios se muestra como guion, no como cadena en blanco: un
        /// textbox en blanco parece un campo que se puede llenar.
        /// </summary>
        private static string Texto(string? valor)
            => string.IsNullOrWhiteSpace(valor) ? "—" : valor.Trim();

/// <summary>
        /// La "—" es solo visual de "sin valor": al guardar equivale a vacío, y vacío en
        /// la base es NULL. Sin esta traducción el textbox guardaría el guion literal.
        /// </summary>
        private static string? SinGuion(string? valor)
        {
            string limpio = valor?.Trim() ?? string.Empty;
            return limpio == "—" ? null : limpio.Length == 0 ? null : limpio;
        }

        /// <summary>Fecha legible; sin valor, guion.</summary>
        private static string Fecha(DateTime? valor)
            => valor is null ? "—" : valor.Value.ToString("dd/MM/yyyy HH:mm");

        /// <summary>
        /// Sin fila elegida el detalle queda todo en guion. En el alta, en cambio, las
        /// cajas van vacías: el guion es la marca de consulta de "no hay valor", y en una
        /// caja que hay que llenar parece un dato puesto. Un guion en un campo vacío
        /// invita a escribir encima en vez de a llenarlo.
        /// </summary>
        /// <param name="paraAlta">True vacía las cajas; false las deja en guion.</param>
        private void LimpiarDetalle(bool paraAlta = false)
        {
            string sinValor = paraAlta ? string.Empty : "—";

            txtDetId.Text = sinValor;
            txtDetUsuario.Text = sinValor;
            txtDetNombre.Text = sinValor;
            txtDetEmail.Text = sinValor;
            txtDetTituloCargo.Text = sinValor;
            txtDetDepartamento.Text = sinValor;
            txtDetRoles.Text = sinValor;
            txtDetFechaCreacion.Text = sinValor;
            txtPassword.Text = string.Empty;
            cboRoles.SelectedIndex = -1;

            // El switch no tiene estado "sin valor" como lo tenia el guion del textbox:
            // se deja en Activo, que es lo que hace LimpiarDetalle en Clientes y
            // Proveedores. No puede dar a un guardado equivocado: sin fila no hay Editar
            // habilitada, y al entrar a Editar VolcarDetalle lo repinta desde la fila.
            PintarEstado(true);
        }

        /// <summary>
        /// Editar solo tiene sentido con una fila elegida en el listado; Nuevo siempre
        /// esta disponible. Mismo criterio que ActualizarBotonesBarra de FrmProductos.
        /// </summary>
        private void ActualizarBotonesBarra()
        {
            // Escribir y elegir fila son cosas distintas: mientras se esta escribiendo,
            // Editar se apaga para no abrir un segundo formulario encima del primero.
            btnEditarUsuario.Enabled = !EsEditable && gridUsuarios.CurrentRow is not null;
            btnNuevoUsuario.Enabled = !EsEditable;
        }

        /// <summary>Nuevo usuario: limpia el detalle y lo abre en modo escritura.</summary>
        private void BtnNuevoUsuario_Click(object? sender, EventArgs e)
        {
            // Sin comprobación de permisos: Usuarios no tiene permisos propios. Pedir
            // Usuarios:Crear sacaba "No tiene permiso para crear usuarios" a cualquiera
            // que no fuera admin y el alta no arrancaba.
            _idEnEdicion = null;

            // Vacío y no con guiones: en el alta las cajas están para escribirlas.
            LimpiarDetalle(paraAlta: true);
            AplicarModo(ModoFormulario.Nuevo);

            // El alta siempre arranca con la clave tapada.
            TaparClave();

            // Un usuario nuevo nace activo: CreateUsuarioAsync tiene activo=1 en el SQL,
            // asi que el interruptor ni siquiera se muestra (ver AplicarModo).
            swDetActivo.Active = true;

            // El consecutivo que le va a tocar. Se calcula al abrir el alta, no al
            // guardar: el usuario lo ve mientras escribe y no se lleva la sorpresa al
            // final de que el numero es otro. Es un aviso, no una reserva —
            // CreateUsuarioAsync lo decide la base — asi que se vuelve a pintar con el
            // UserId real al terminar el alta.
            txtDetId.Text = SiguienteConsecutivo().ToString();
            CargarCatalogoDeRoles();

            ActualizarBotonesBarra();
            txtDetUsuario.Focus();
        }

        /// <summary>Editar usuario: vuelca el de la fila activa y abre el modo escritura.</summary>
        private async void BtnEditarUsuario_Click(object? sender, EventArgs e)
        {
            // Igual que en el alta: el módulo no pide permisos para editar.
            Usuario? usuario = UsuarioDeLaFilaElegida();
            if (usuario is null)
            {
                MostrarAviso("Seleccione primero el usuario que quiere editar.");
                return;
            }

            // El id se captura ACA y no se vuelve a leer de la fila: mientras edita se
            // puede seguir moviendose por el listado, y sin esto se guardaria sobre
            // otro usuario.
            _idEnEdicion = usuario.UserId;

            AplicarModo(ModoFormulario.Editar);
            VolcarDetalle(usuario);
            await SeleccionarRolDelUsuario(usuario.UserId);

            ActualizarBotonesBarra();
            txtDetNombre.Focus();
        }

        /// <summary>Descarta lo escrito y vuelve a consulta.</summary>
        private void BtnCancelarUsuario_Click(object? sender, EventArgs e)
        {
            // Cancelar descarta sin preguntar: decision del usuario, evita dialogos
            // modales bloqueantes (un MessageBox de verdad deja colgada cualquier
            // prueba que haga clic en este boton).
            VolverAConsulta();
        }

        /// <summary>
        /// Guarda el usuario del formulario, sea alta o edición, y refresca el listado.
        /// Ante un fallo NO se sale del modo escritura: lo escrito se queda en pantalla
        /// para que el usuario lo corrija en vez de teclearlo de nuevo.
        /// </summary>
        private async void BtnGuardarUsuario_Click(object? sender, EventArgs e)
        {
            if (_guardando)
            {
                return;
            }

            bool esNuevo = _modo == ModoFormulario.Nuevo;

            if (esNuevo)
            {
                Result<(Usuario Usuario, string Password, List<int> RoleIds)> alta = ConstruirAltaDesdeFormulario();
                if (!alta.IsSuccess)
                {
                    MostrarAviso(alta.Error ?? "Revise los datos del usuario.");
                    return;
                }

                await GuardarAlta(alta.Value!);
                return;
            }

            // El id se resuelve aca y no antes de validar: es la fila del UPDATE, y en el
            // alta no existe todavia.
            int? idEnEdicion = _idEnEdicion;

            if (idEnEdicion is null)
            {
                // Solo si se perdio el id sin pasar por Editar: sin el, el UPDATE
                // actualizaria cero filas y el formulario diria que guardo bien.
                MostrarAviso("No se sabe qué usuario está editando. Vuelva a elegirlo del listado.");
                VolverAConsulta();
                return;
            }

            Result<Usuario> edicion = ConstruirEdicionDesdeFormulario(idEnEdicion.Value);
            if (!edicion.IsSuccess)
            {
                MostrarAviso(edicion.Error ?? "Revise los datos del usuario.");
                return;
            }

            await GuardarEdicion(edicion.Value!, idEnEdicion.Value);
        }

        /// <summary>
        /// Vuelve al estado de consulta dejando el detalle como estaba antes de empezar
        /// a escribir: el usuario editado, o vacio si era un alta que se descartó.
        /// </summary>
        private void VolverAConsulta()
        {
            int? mostrar = _idEnEdicion;
            _idEnEdicion = null;
            _guardando = false;

            // La clave vuelve a taparse al salir del alta, se haya guardado o no.
            TaparClave();

            AplicarModo(ModoFormulario.Consulta);

            Usuario? usuario = mostrar is null ? null : BuscarEnCatalogo(mostrar.Value);
            if (mostrar is null || usuario is null)
            {
                RefrescarDetalle();
                return;
            }

            SeleccionarFilaEnGrid(mostrar.Value);
            RefrescarDetalle();
        }

        // ─────────────────────────────────────────────────────────────────
        // Hoja de Excel con todos los usuarios
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Crea un Excel con TODOS los usuarios registrados. Se exporta el listado
        /// entero, no lo que haya salido en pantalla: el buscador o el filtro de rol no
        /// pueden cambiar lo que el fichero contiene, que es el padron completo.
        /// </summary>
        private void BtnExportarUsuario_Click(object? sender, EventArgs e)
        {
            // Sin comprobación de permisos: el módulo no los tiene. Exportar el padrón
            // completo no depende de Usuarios:Ver, que era el unico que se pedia.
            //
            // ExportToExcel lanza si la coleccion va vacia: se comprueba antes para
            // salir en silencio. No haber nada que exportar no es un error y en el
            // listado ya se ve, asi que el unico aviso que queda es el del fallo.
            if (_usuarios.Count == 0)
            {
                return;
            }

            List<UsuarioExportado> filas = _usuarios.Select(ParaExcel).ToList();

            try
            {
                // Sin aviso de exito al exportar: el propio ExportToExcel abre el
                // fichero, que ya es la confirmacion. Los fallos siguen avisando.
                _exportDataService.ExportToExcel(filas, "Usuarios.xlsx");
            }
            catch (Exception ex)
            {
                // El servicio ya avisa por ServiceErrors de sus fallos propios; aqui se
                // cubre lo que se le escape (permisos de carpeta, disco lleno, Excel
                // abierto...).
                ServiceErrors.Report("Error al crear la hoja de usuarios: " + ex.Message);
                MostrarAviso("No se pudo crear la hoja de Excel: " + ex.Message);
            }
        }

        /// <summary>
        /// Proyecta un usuario a la fila del Excel. Va por ParaExcel y no se exporta la
        /// entidad <see cref="Usuario"/> tal cual porque PasswordHash saldria en claro
        /// en la hoja, que es un listado de personas y no un volcado de la tabla.
        ///
        /// El rol sale por <see cref="NombresDeRol(int)"/>, el mismo sitio que pinta el
        /// grid: en el fichero se ve exactamente lo que se ve en pantalla, y el texto
        /// del estado es el "Activo" / "Inactivo" de la ultima columna.
        /// </summary>
        private UsuarioExportado ParaExcel(Usuario usuario)
        {
            ArgumentNullException.ThrowIfNull(usuario);

            return new UsuarioExportado
            {
                UserId = usuario.UserId,
                Usuario = usuario.Username ?? string.Empty,
                Nombre = usuario.NombreCompleto ?? string.Empty,
                Email = usuario.Email ?? string.Empty,
                Cargo = usuario.TituloCargo ?? string.Empty,
                Departamento = usuario.Departamento ?? string.Empty,
                Roles = NombresDeRol(usuario.UserId),
                Estado = usuario.Activo ? "Activo" : "Inactivo",
                FechaCreacion = usuario.FechaCreacion,
                UltimoLogin = usuario.UltimoLogin
            };
        }

        // ─────────────────────────────────────────────────────────────────
        // Reporte del catalogo
        // ─────────────────────────────────────────────────────────────────

        /// <summary>
        /// Abre el reporte del catalogo en el visor (ReportsViewer). Va contra la base con la
        /// consulta de R.QUERY.USERS y trae el padron entero, no lo que este filtrado.
        /// </summary>
        private void BtnReporteUsuario_Click(object? sender, EventArgs e)
        {
            if (!PermisoHelper.PuedeVer(ConstanteModuloUsuarios))
            {
                MostrarAviso("No tiene permiso para ver los usuarios.");
                return;
            }

            try
            {
                _reportsService.Reporte_Usuarios(this, "Catalogo de Usuarios", "Report_Usuarios.rdlc");
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("No se pudo abrir el reporte de usuarios: " + ex.Message);
                MostrarAviso("No se pudo abrir el reporte: " + ex.Message);
            }
        }

        private void AplicarTemaVerde()
        {
            // Fondo verde pastel y barra de titulo en el verde saturado de la app.
            BackColor = Color.FromArgb(240, 250, 235);
            Style = UIStyle.Green;
            TitleColor = Color.FromArgb(110, 190, 40);
            TitleForeColor = Color.White;
            ForeColor = Color.FromArgb(48, 48, 48);

            // Ese ForeColor del form se lo pasa SunnyUI a los hijos (y el estilo verde
            // trae el suyo propio), asi que el rotulo del modulo se quedaba en gris en
            // vez del verde de la banda. Se repone aqui, ya con Style aplicado: en el
            // diseniador no sobrevive al estilo.
            lblTitulo.ForeColor = VerdeCabecera;

            // El interruptor de estado. Solo van colores y textos: el TAMAÑO y el Dock se
            // dejan como los trae SunnyUI por defecto (75x29, Dock.None), asi que van
            // en el disenador y no aqui.
            // ActiveText e InActiveText si hay que fijarlos siempre: los de SunnyUI
            // son "开" y "关".
            swDetActivo.Enabled = true;
            swDetActivo.ForeColor = Color.FromArgb(64, 64, 64);
            swDetActivo.ActiveColor = Color.FromArgb(110, 190, 40);
            swDetActivo.ActiveText = "Activo";
            swDetActivo.InActiveText = "Inactivo";

            // UIStyleManager con GlobalFont le deja 12pt al switch y 9pt al rotulo: el
            // mismo dato escrito con dos tamanos. Se le baja al mismo 9pt para que las
            // dos palabras midan lo mismo y se lean como una sola linea.
            swDetActivo.Font = lblCapActivo.Font;

            // Y los rotulos del detalle se les fija el alto de su fila. SunnyUI los deja
            // con un alto propio (medido: 173 px en una celda de 38), y como el texto va
            // centrado, la palabra "Activo" caia muy por debajo del interruptor de 29 px
            // que tiene al lado. El alto de la celda es el del disenio menos los margenes.
            AjustarAltoDeRotulos();
        }

        /// <summary>
        /// Aplica fuente, color y aspecto a los controles de la pestaña de detalle con el
        /// mismo criterio que <c>EstilarCamposDetalle</c> de FrmProductos: rotulos JetBrains
        /// Mono 9 en gris ocupando toda la columna, cajas blancas a todo el ancho de su
        /// celda, titulo en verde oscuro e interruptor con la misma letra que el rotulo.
        ///
        /// Se llama del constructor —y no se fija en el diseniador— porque el UIStyleManager
        /// re-estiliza el form despues de InitializeComponent y el Style=Green del tema
        /// vuelve a pisarlo: lo del diseniador no sobrevive a ninguno de los dos. Es el
        /// mismo motivo que documenta el metodo homonimo de FrmProductos.
        /// </summary>
        private void EstilarCamposDetalle()
        {
            // Ancho de la primera columna menos los márgenes: es lo que ocupa un rotulo
            // con Dock=Fill en Productos. Aqui no se doka: en la ultima fila del
            // TableLayoutPanel un control anclado a Fill se lleva el espacio que sobra
            // en la tabla y el rotulo crece de 38 a 173 px, que es exactamente el fallo
            // que este metodo viene a arreglar.
            int anchoColumnaRotulos =
                tlpDetalle.GetColumnWidths()[0];

            UILabel[] etiquetas =
            [
                lblCapId, lblCapUsuario, lblCapPassword, lblCapNombre, lblCapEmail,
                lblCapTituloCargo, lblCapDepartamento, lblCapRoles, lblCapFechaCreacion,
                lblCapActivo
            ];

            foreach (UILabel etiqueta in etiquetas)
            {
                // MiddleLeft centra la letra en los 38 px de la fila: con el TopLeft del
                // disenador la palabra quedaba arriba del todo y a media altura respecto
                // del campo y del interruptor, que van centrados en la misma fila.
                etiqueta.AutoSize = false;
                etiqueta.Width = anchoColumnaRotulos - etiqueta.Margin.Horizontal;
                etiqueta.Font = new Font("JetBrains Mono", 9F);
                etiqueta.ForeColor = GrisTexto;
                etiqueta.TextAlign = ContentAlignment.MiddleLeft;
            }

            lblDetalleTitulo.Font = new Font("JetBrains Mono", 10F, FontStyle.Bold);
            lblDetalleTitulo.ForeColor = VerdeTitulo;

            UITextBox[] campos =
            [
                txtDetId, txtDetUsuario, txtDetNombre, txtDetEmail, txtDetTituloCargo,
                txtDetDepartamento, txtDetRoles, txtDetFechaCreacion, txtPassword
            ];

            foreach (UITextBox campo in campos)
            {
                // ReadOnly no se fija aqui: lo gobierna AplicarModo, que es el unico sitio
                // que decide si el formulario admite escritura. Este metodo solo el aspecto.
                campo.Dock = DockStyle.Fill;
                campo.FillColor = Color.White;
                campo.RectColor = ColorBordeCampo;
                campo.Font = new Font("JetBrains Mono", 9F);
                campo.TextAlignment = ContentAlignment.MiddleLeft;
            }

            // El combo comparte celda con la caja de solo lectura de los roles: mismo
            // aspecto que ella, o al pasar de consulta a edicion cambiaria el campo de sitio.
            cboRoles.Dock = DockStyle.Fill;
            cboRoles.FillColor = Color.White;
            cboRoles.RectColor = ColorBordeCampo;
            cboRoles.Font = new Font("JetBrains Mono", 9F);
            cboRoles.TextAlignment = ContentAlignment.MiddleLeft;

            // La letra del interruptor con la del rotulo de al lado: con los 12 pt del
            // estilo global, "Activo" media mas que el rotulo y no se leian juntas.
            swDetActivo.Font = new Font("JetBrains Mono", 9F);
            swDetActivo.ForeColor = GrisTexto;
        }

        /// <summary>
        /// Fija en el alto de SU fila los rótulos de la primera columna del detalle. Un
        /// UILabel de SunnyUI conserva un alto propio mayor que la fila y, con el texto
        /// centrado, la palabra se va hacia abajo: al lado de un interruptor de 29 px se
        /// leen como dos lineas distintas.
        ///
        /// Se usa el alto de cada fila y no el del disenio porque en el detalle no todas
        /// miden lo mismo: hay filas de 20 px. Ponerles a todas el alto de 38 las
        /// desbordaba y el TableLayoutPanel terminaba creando una fila de mas para
        /// acomodarlas.
        /// </summary>
        private void AjustarAltoDeRotulos()
        {
            foreach (UILabel rotulo in tlpDetalle.Controls.OfType<UILabel>())
            {
                int fila = tlpDetalle.GetRow(rotulo);

                if (fila < 0 || fila >= tlpDetalle.RowStyles.Count)
                {
                    continue;
                }

                rotulo.AutoSize = false;
                rotulo.Height = (int)tlpDetalle.RowStyles[fila].Height - rotulo.Margin.Vertical;
            }
        }

        /// <summary>
        /// Muestra u oculta la contraseña mientras se escribe. Es el mismo gesto que el
        /// ojo de FrmLogin: el caracter de enmascarado pasa a '\0', que en WinForms es
        /// "sin máscara".
        /// </summary>
        private void AlternarVisibilidadClave()
        {
            _mostrarClave = !_mostrarClave;
            AplicarEnmascarado();
        }

        /// <summary>
        /// Deja la clave tapada. Se llama al salir del alta: si quedara a la vista, el
        /// siguiente alta abriría con la del usuario anterior en claro.
        /// </summary>
        private void TaparClave()
        {
            _mostrarClave = false;
            AplicarEnmascarado();
        }

        /// <summary>
        /// Traduce <see cref="_mostrarClave"/> a lo que se ve en la caja y al color del
        /// botón. Unico lugar que toca el PasswordChar: si el valor por defecto de
        /// SunnyUI seQneta en los tres (' ' = sin máscara), la clave se lee en claro.
        /// Verde cuando está a la vista, gris cuando no: el estado no se adivina, se ve.
        /// </summary>
        private void AplicarEnmascarado()
        {
            txtPassword.PasswordChar = _mostrarClave ? '\0' : CaracterClave;
            btnOjoPassword.Style = _mostrarClave ? UIStyle.Green : UIStyle.Gray;
            btnOjoPassword.StyleCustomMode = false;
        }

        /// <summary>
        /// Carga el listado antes de mostrar el formulario (la invoca FormManager).
        /// Si la base falla se reporta el error y el formulario queda utilizable con
        /// el listado vacio, en vez de reventar al abrir.
        /// </summary>
        public async Task InitializeAsync()
        {
            try
            {
                _usuarios = await _seguridadService.GetUsuariosAsync();
                _catalogoRoles = await _seguridadService.GetRolesAsync();
                _idsRolPorUsuario = await ResolverRolesPorUsuarioAsync(_usuarios, _catalogoRoles);
            }
            catch (Exception ex)
            {
                _usuarios = [];
                _catalogoRoles = [];
                _idsRolPorUsuario = [];
                ServiceErrors.Report("Error al cargar Usuarios: " + ex.Message);
            }

            AplicarFiltroBusqueda();

            // El catalogo recien descargado arma el combo de rol y los radios del filtro.
            // Sin esta llamada el combo se llenaba recien al abrir Editar o Nuevo, y el
            // filtro de rol se quedaba con el unico radio de "Todos".
            CargarCatalogoDeRoles();

            ActualizarBotonesBarra();
        }

        /// <summary>
        /// GetUsuariosAsync no hace JOIN de roles (Usuario.Roles viene vacio), asi que
        /// hay que preguntar los roles de cada usuario. Los ids se piden en paralelo
        /// para no encadenar N viajes de ida y vuelta.
        /// </summary>
        /// <param name="usuarios">Usuarios del listado.</param>
        /// <param name="catalogoRoles">Catalogo ya descargado, para no pedirlo dos veces.</param>
        private async Task<Dictionary<int, List<int>>> ResolverRolesPorUsuarioAsync(
            List<Usuario> usuarios,
            List<Role> catalogoRoles)
        {
            Dictionary<int, List<int>> idsPorUsuario = [];

            if (usuarios.Count == 0)
            {
                return idsPorUsuario;
            }

            // List<int>[] y no List<List<int>>: WhenAll devuelve el array que le pide el tipo.
            List<int>[] idsPorFila = await Task.WhenAll(
                usuarios.Select(u => _seguridadService.GetUsuarioRoleIdsAsync(u.UserId)));

            for (int i = 0; i < usuarios.Count; i++)
            {
                idsPorUsuario[usuarios[i].UserId] = idsPorFila[i];
            }

            return idsPorUsuario;
        }

/// <summary>
        /// Nombres de rol de un usuario, unidos con ", ". Sale de los ids cacheados y del
        /// catalogo de roles ACTIVOS: hay un solo sitio donde un RoleId se traduce a texto,
        /// y ese sitio es el mismo que lee el filtro de rol.
        /// </summary>
        private string NombresDeRol(int userId)
        {
            if (!_idsRolPorUsuario.TryGetValue(userId, out List<int>? ids) || ids.Count == 0)
            {
                return string.Empty;
            }

return string.Join(", ", _rolesDisponibles.Where(r => ids.Contains(r.RoleId)).Select(r => r.Nombre));
        }

        /// <summary>El rol como se muestra en el listado y en la consulta.</summary>
        private string TextoRol(int userId)
        {
            string nombres = NombresDeRol(userId);
            return string.IsNullOrWhiteSpace(nombres) ? "—" : nombres;
        }

        /// <summary>
        /// Pinta en rojo con letra clara la fila de un usuario desactivado, con el mismo
        /// criterio que Clientes, Proveedores, Vendedores y Productos: el estado se ve de
        /// un vistazo en el listado sin tener que abrir el detalle. El DataTable trae el
        /// estado ya traducido a "Inactivo", que es lo unico que hay que leer.
        /// </summary>
        private void GridUsuarios_CellFormatting(object? sender, DataGridViewCellFormattingEventArgs e)
        {
            if (e.RowIndex < 0
                || gridUsuarios.Rows[e.RowIndex].DataBoundItem is not DataRowView fila
                || !Equals(fila["Estado"], "Inactivo"))
            {
                return;
            }

            e.CellStyle ??= new DataGridViewCellStyle();
            e.CellStyle.BackColor = FondoDesactivado;
            e.CellStyle.ForeColor = TextoDesactivado;
            e.CellStyle.SelectionBackColor = SeleccionDesactivado;
            e.CellStyle.SelectionForeColor = TextoDesactivado;
        }

        /// <summary>
        /// Proyecta los usuarios a un DataTable con las cinco columnas del grid.
        /// Se proyecta a proposito y no se enlaza la lista: <see cref="Usuario.Roles"/> es
        /// una lista de Role y <c>Activo</c> es un bool, y el grid no sabe pintar ni uno
        /// ni otro. El guion en el rol o el nombre vacios mantiene el ancho de columna
        /// estable y distingue "no tiene" de un dato que todavia no se ve.
        /// </summary>
        private DataTable ConstruirDataTable(IEnumerable<Usuario> usuarios)
        {
            DataTable tabla = new();
            tabla.Columns.Add("UserId", typeof(int));
            tabla.Columns.Add("Username", typeof(string));
            tabla.Columns.Add("Nombre", typeof(string));
            tabla.Columns.Add("Rol", typeof(string));
            tabla.Columns.Add("Estado", typeof(string));

foreach (Usuario usuario in usuarios)
            {
                tabla.Rows.Add(
                    usuario.UserId,
                    usuario.Username,
                    string.IsNullOrWhiteSpace(usuario.NombreCompleto) ? "—" : usuario.NombreCompleto,
                    TextoRol(usuario.UserId),
                    usuario.Activo ? "Activo" : "Inactivo");
            }

            return tabla;
        }

        /// <summary>
        /// Filtra el listado por usuario, nombre o correo — las tres columnas que el
        /// buscador tiene a la vista — y por rol. Filtra en memoria contra la lista ya
        /// cargada: el RowFilter del DataTable exigiria escapar comillas y repetir el
        /// criterio en cada tecla. Las dos condiciones se combinan con AND.
        /// </summary>
        private void AplicarFiltroBusqueda()
        {
            string texto = txtBuscar.Text?.Trim() ?? string.Empty;
            int? roleId = RolFiltrado();

            IEnumerable<Usuario> visibles = _usuarios;

            if (texto.Length > 0)
            {
                visibles = visibles.Where(u =>
                    u.Username.Contains(texto, StringComparison.OrdinalIgnoreCase)
                    || (u.NombreCompleto?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false)
                    || (u.Email?.Contains(texto, StringComparison.OrdinalIgnoreCase) ?? false));
            }

            if (roleId is not null)
            {
                visibles = visibles.Where(u =>
                    _idsRolPorUsuario.TryGetValue(u.UserId, out List<int>? ids)
                    && ids.Contains(roleId.Value));
            }

            gridUsuarios.DataSource = ConstruirDataTable(visibles);

            ActualizarResumen();
            ActualizarBotonesBarra();
            // Cambiar el DataSource puede dejar el detalle apuntando a la fila que
            // estaba antes del filtro.
            RefrescarDetalle();
        }

        // ─────────────────────────────────────────────────────────────
        // Filtro de rol (radios bajo el buscador)
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Arma el panel de radios y lo cuelga de panelIzq entre el buscador y el grid.
        /// Los radios se crean despues, cuando llega el catalogo: ver
        /// <see cref="CrearRadiosDeRol"/>.
        /// </summary>
        private void CrearFiltroRol()
        {
            pnlFiltroRol = new Panel
            {
                Name = "pnlFiltroRol",
                Dock = DockStyle.Top,
                BackColor = Color.White,
                Padding = new Padding(8, 4, 8, 4),
                // AutoSize y no alto fijo: los radios dependen de cuantos roles activos
                // haya. Con cinco no entran en una fila de 343 px y con WrapContents el
                // panel tiene que crecer hacia abajo en vez de cortar el ultimo.
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            _filaRadiosRol = new FlowLayoutPanel
            {
                Name = "flwRadiosRol",
                // Dock.Top y no Fill: con Fill dentro de un panel AutoSize el ancho se
                // colapsa a 0 y el FlowLayoutPanel no tiene contra que envolver.
                Dock = DockStyle.Top,
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true,
                AutoSize = true,
                AutoSizeMode = AutoSizeMode.GrowAndShrink
            };

            pnlFiltroRol.Controls.Add(_filaRadiosRol);

            // Ojo con el orden: WinForms procesa el dockeo en orden INVERSO al de la
            // coleccion, asi que el ultimo control agregado es el que queda mas ARRIBA.
            // El disenador dejo [grid(Fill), pnlBuscador(Top), pnlResumen(Bottom)]; para
            // meter los radios hay que sacarlos de la cola y volver a agregarlos al reves.
            panelIzq.Controls.Remove(pnlBuscador);
            panelIzq.Controls.Remove(pnlResumen);
            panelIzq.Controls.Add(pnlFiltroRol);
            panelIzq.Controls.Add(pnlBuscador);
            panelIzq.Controls.Add(pnlResumen);

            CrearRadiosDeRol();
        }

        /// <summary>
        /// (Re)arma los radios con el catalogo de roles actual: "Todos" mas uno por rol
        /// activo. Conserva el rol que estaba marcado, porque el catalogo se recarga al
        /// abrir Editar y al guardar, y sin esto el filtro se borraba solo.
        /// </summary>
        /// <returns>
        /// True si el filtro marcado cambio — o sea, el rol marcado ya no existe. El
        /// listado en pantalla responde al filtro viejo y hay que volver a proyectarlo.
        /// </returns>
        private bool CrearRadiosDeRol()
        {
            string? marcado = RolMarcadoEnElFiltro();

            _filaRadiosRol.SuspendLayout();
            _filaRadiosRol.Controls.Clear();
            _radiosRol.Clear();

            RadioButton todos = CrearRadioRol("rbRolTodos", "Todos", roleId: null);
            _radiosRol.Add(todos);

            foreach (Role rol in _rolesDisponibles)
            {
                _radiosRol.Add(CrearRadioRol($"rbRol{rol.RoleId}", rol.Nombre, rol.RoleId));
            }

            _filaRadiosRol.ResumeLayout(true);

            RadioButton? aMarcar = _radiosRol.FirstOrDefault(r => Equals(r.Tag, marcado)) ?? todos;
            bool cambio = !ReferenceEquals(aMarcar, _radiosRol.FirstOrDefault(r => r.Checked));
            aMarcar.Checked = true;

            return cambio;
        }

        /// <summary>
        /// Un radio del filtro. El Tag lleva el RoleId en texto —o null en "Todos"—, que
        /// es contra lo que <see cref="RolFiltrado"/> decide.
        /// </summary>
        private RadioButton CrearRadioRol(string nombre, string texto, int? roleId)
        {
            RadioButton radio = new()
            {
                Name = nombre,
                Text = texto,
                AutoSize = true,
                Appearance = Appearance.Normal,
                Font = new Font("JetBrains Mono", 9F),
                ForeColor = Color.FromArgb(80, 80, 80),
                Margin = new Padding(0, 0, 12, 0),
                Tag = roleId?.ToString()
            };

            // Solo reacciona el que queda marcado: al desmarcar el anterior se dispara el
            // evento tambien, y filtrar dos veces por clic no hace nada util.
            radio.CheckedChanged += (_, _) =>
            {
                if (radio.Checked)
                {
                    AplicarFiltroBusqueda();
                }
            };

            _filaRadiosRol.Controls.Add(radio);
            return radio;
        }

        /// <summary>El RoleId del radio marcado, o null si esta "Todos".</summary>
        private int? RolFiltrado()
            => _radiosRol.FirstOrDefault(r => r.Checked)?.Tag is string tag && int.TryParse(tag, out int roleId)
                ? roleId
                : null;

        /// <summary>El Tag del radio marcado. Null significa "Todos".</summary>
        private string? RolMarcadoEnElFiltro()
            => _radiosRol.FirstOrDefault(r => r.Checked)?.Tag as string;

        /// <summary>True si el usuario eligio un rol concreto y no "Todos".</summary>
        private bool HayFiltroRol() => RolFiltrado() is not null;

        // ─────────────────────────────────────────────────────────────
        // Exportación a Excel
        // ─────────────────────────────────────────────────────────────

        // Validación y guardado
        // ─────────────────────────────────────────────────────────────

        /// <summary>
        /// Valida el formulario de alta y devuelve lo que hay que mandar a
        /// CreateUsuarioAsync: el Usuario y el id del rol ya guardado.
        /// </summary>
        private Result<(Usuario Usuario, string Password, List<int> RoleIds)> ConstruirAltaDesdeFormulario()
        {
            string username = txtDetUsuario.Text.Trim();
            string nombre = txtDetNombre.Text.Trim();
            string email = txtDetEmail.Text.Trim();
            string password = txtPassword.Text;

            // Misma regla que en edicion (ErrorDeLogin): aqui no hay id que excluir.
            if (ErrorDeLogin(username, userIdExcluido: null) is string errorLogin)
            {
                return Result<(Usuario, string, List<int>)>.Failure(errorLogin);
            }

            if (nombre.Length == 0)
            {
                return Result<(Usuario, string, List<int>)>.Failure("Escriba el nombre completo.");
            }

            if (email.Length > 0 && !CorreoTieneForma(email))
            {
                return Result<(Usuario, string, List<int>)>.Failure("El correo no tiene un formato válido.");
            }

            if (password.Length == 0)
            {
                return Result<(Usuario, string, List<int>)>.Failure("Escriba la contraseña del usuario nuevo.");
            }

            List<int> roleIds = RolElegido() is Role rolElegido ? [rolElegido.RoleId] : [];

            if (roleIds.Count == 0)
            {
                return Result<(Usuario, string, List<int>)>.Failure(
                    "Elija el rol del usuario: sin rol no puede ver nada.");
            }

            Usuario usuario = new()
            {
                Username = username,
                NombreCompleto = nombre,
                Email = email.Length == 0 ? null : email,

                // Puesto y area son texto libre y opcionales: vacio es NULL, no "".
                TituloCargo = SinGuion(txtDetTituloCargo.Text),
                Departamento = SinGuion(txtDetDepartamento.Text),

                // CreateUsuarioAsync pone activo=1 y primer_login=1 en el SQL, asi que
                // esto no se guarda: se deja explicito para que no parezca un olvido.
                Activo = true,
                PrimerLogin = true
            };

            return Result<(Usuario, string, List<int>)>.Success((usuario, password, roleIds));
        }

        /// <summary>Valida el formulario de edicion y devuelve el Usuario del UPDATE.</summary>
        private Result<Usuario> ConstruirEdicionDesdeFormulario(int userId)
        {
            string username = txtDetUsuario.Text.Trim();
            string nombre = txtDetNombre.Text.Trim();
            string email = txtDetEmail.Text.Trim();

            if (ErrorDeLogin(username, userId) is string errorLogin)
            {
                return Result<Usuario>.Failure(errorLogin);
            }

            if (nombre.Length == 0)
            {
                return Result<Usuario>.Failure("Escriba el nombre completo.");
            }

            if (email.Length > 0 && !CorreoTieneForma(email))
            {
                return Result<Usuario>.Failure("El correo no tiene un formato válido.");
            }

            List<Role> roles = RolElegido() is Role rolElegido ? [rolElegido] : [];

            if (roles.Count == 0)
            {
                return Result<Usuario>.Failure("Elija el rol del usuario: sin rol no puede ver nada.");
            }

            return Result<Usuario>.Success(new Usuario
            {
                UserId = userId,
                Username = username,
                NombreCompleto = nombre,
                Email = email.Length == 0 ? null : email,
                TituloCargo = SinGuion(txtDetTituloCargo.Text),
                Departamento = SinGuion(txtDetDepartamento.Text),
                Activo = swDetActivo.Active,

                // UpdateUsuarioAsync lee usuario.Roles (List<Role>), no roleIds como el
                // alta: es la otra traduccion de la misma idea.
                Roles = roles
            });
        }

        /// <summary>
        /// Comprobacion simple de correo: un @, algo antes, algo despues y un punto en
        /// el dominio. No es RFC 5322 y no pretende serlo: alcanza para avisar de un
        /// tecleo mal antes de ir a la base.
        /// </summary>
        private static bool CorreoTieneForma(string email)
        {
            int arroba = email.IndexOf('@', StringComparison.Ordinal);

            if (arroba <= 0 || arroba == email.Length - 1)
            {
                return false;
            }

            string dominio = email[(arroba + 1)..];

            return dominio.Contains('.', StringComparison.Ordinal)
                && !dominio.StartsWith('.')
                && !dominio.EndsWith('.');
        }

        /// <summary>Largo de usuarios.username en la base: NVARCHAR(50).</summary>
        private const int LargoMaximoLogin = 50;

        /// <summary>
        /// Valida el login escrito en la caja de usuario y devuelve el error, o null
        /// si se puede guardar. Es la misma regla para alta y para edicion: no vacio,
        /// hasta <see cref="LargoMaximoLogin"/> caracteres y sin pisar el de otro. El
        /// servicio no comprueba duplicados —si hay uno revienta el INSERT o el UPDATE
        /// con un mensaje que no le dice a nadie qué hacer— y la comparacion es
        /// insensible a mayusculas porque asi trabaja la base.
        /// </summary>
        /// <param name="username">Login ya recortado.</param>
        /// <param name="userIdExcluido">
        /// Quien se esta editando; null en el alta. Se excluye para que su propio
        /// login no se le presente como duplicado.
        /// </param>
        private string? ErrorDeLogin(string username, int? userIdExcluido)
        {
            if (username.Length == 0)
            {
                return "Escriba el nombre de usuario.";
            }

            if (username.Length > LargoMaximoLogin)
            {
                return $"El usuario no puede pasar de {LargoMaximoLogin} caracteres.";
            }

            bool enUso = _usuarios.Any(u => u.UserId != userIdExcluido
                && string.Equals(u.Username, username, StringComparison.OrdinalIgnoreCase));

            return enUso
                ? $"El usuario \"{username}\" ya existe. Elija otro nombre."
                : null;
        }

        private async Task GuardarAlta((Usuario Usuario, string Password, List<int> RoleIds) alta)
        {
            _guardando = true;

            try
            {
                int nuevoId = await _seguridadService.CreateUsuarioAsync(
                    alta.Usuario, alta.Password, alta.RoleIds);

                if (nuevoId <= 0)
                {
                    // El servicio ya aviso por ServiceErrors. No se sale de edicion:
                    // lo escrito se queda para que el usuario lo corrija.
                    MostrarAviso("No se pudo guardar el usuario nuevo.");
                    return;
                }

                await RecargarListado(nuevoId);
                VolverAConsulta();
                MostrarAviso($"Usuario \"{alta.Usuario.Username}\" creado.");
            }
            catch (Exception ex)
            {
                MostrarAviso("Error al crear el usuario: " + ex.Message);
            }
            finally
            {
                _guardando = false;
            }
        }

        private async Task GuardarEdicion(Usuario usuario, int userId)
        {
            _guardando = true;

            try
            {
                bool ok = await _seguridadService.UpdateUsuarioAsync(usuario);

                if (!ok)
                {
                    MostrarAviso("No se pudo guardar el usuario.");
                    return;
                }

                await RecargarListado(userId);
                VolverAConsulta();
                MostrarAviso("Usuario guardado.");
            }
            catch (Exception ex)
            {
                MostrarAviso("Error al actualizar el usuario: " + ex.Message);
            }
            finally
            {
                _guardando = false;
            }
        }

        /// <summary>
        /// Vuelve a pedir el listado y los roles para que el guardado se vea sin
        /// reiniciar el formulario. Se recarga SIEMPRE, tambien al fallar: si no, el
        /// grid quedaria mostrando datos viejos al lado del error.
        /// </summary>
        private async Task RecargarListado(int? seleccionarUserId = null)
        {
            try
            {
                _usuarios = await _seguridadService.GetUsuariosAsync();
                _catalogoRoles = await _seguridadService.GetRolesAsync();
                _idsRolPorUsuario = await ResolverRolesPorUsuarioAsync(_usuarios, _catalogoRoles);
            }
            catch (Exception ex)
            {
                ServiceErrors.Report("Error al recargar Usuarios: " + ex.Message);
                return;
            }

            AplicarFiltroBusqueda();

            // El combo se recarga tambien: si mientras el formulario estaba abierto se
            // dio de baja o se creo un rol, la lista de arriba tiene que ser la misma.
            // Va despues del filtro para no repintar dos veces.
            CargarCatalogoDeRoles();

            if (seleccionarUserId is not null)
            {
                SeleccionarFilaEnGrid(seleccionarUserId.Value);
            }
        }

        /// <summary>El usuario del catalogo cargado, o null si ya no existe.</summary>
        private Usuario? BuscarEnCatalogo(int userId)
            => _usuarios.FirstOrDefault(u => u.UserId == userId);

        /// <summary>
        /// Deja seleccionada la fila del usuario indicado. No hace falta que el filtro
        /// lo muestre: si esta filtrando otra cosa, se limpia para que aparezca.
        /// </summary>
        private void SeleccionarFilaEnGrid(int userId)
        {
            if (txtBuscar.Text.Trim().Length > 0)
            {
                txtBuscar.Text = string.Empty;
                return;
            }

            foreach (DataGridViewRow fila in gridUsuarios.Rows)
            {
                if (fila.DataBoundItem is DataRowView datos
                    && Convert.ToInt32(datos["UserId"]) == userId)
                {
                    fila.Selected = true;
                    gridUsuarios.CurrentCell = fila.Cells[0];
                    return;
                }
            }
        }

        /// <summary>
        /// Aviso de validacion o de fallo del servicio. Vive en un metodo aparte y no en
        /// un MessageBox en linea para que las pruebas puedan sustituir el dialogo modal
        /// por una llamada recorded: un MessageBox de verdad dentro de un test lo dejaria
        /// colgado.
        /// </summary>
        /// <param name="mensaje">Texto a mostrar al usuario.</param>
        protected virtual void MostrarAviso(string mensaje)
            => MessageBox.Show(mensaje, "Usuarios", MessageBoxButtons.OK, MessageBoxIcon.Warning);

        /// <summary>
        /// Cuadro de resumen: arriba el total de usuarios registrados, abajo lo que hay
        /// en pantalla con el corte de activos. Los dos numeros cuenta cosas distintas
        /// —el primero no cambia con el filtro— asi que van en lineas separadas.
        /// </summary>
        private void ActualizarResumen()
        {
            DataTable? tabla = gridUsuarios.DataSource as DataTable;
            int visibles = tabla?.Rows.Count ?? 0;
            int activos = 0;

            if (tabla is not null)
            {
                foreach (DataRow fila in tabla.Rows)
                {
                    if (Equals(fila["Estado"], "Activo"))
                    {
                        activos++;
                    }
                }
            }

            lblResumen.Text = $"Usuarios registrados: {_usuarios.Count}";
            lblResumenDetalle.Text = $"Mostrando {visibles} - Activos {activos} - Inactivos {visibles - activos}";
        }
    }
}
