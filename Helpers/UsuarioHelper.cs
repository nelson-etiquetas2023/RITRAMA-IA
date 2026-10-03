using Ritrama2025.Models;

namespace Ritrama2025.Helpers
{
    /// <summary>
    /// Texto que se muestra de un usuario en la interfaz (barra lateral, etiquetas,
    /// tooltip, avatar). Centraliza los fallbacks para que el mismo usuario se vea
    /// igual en todas partes: primero <see cref="Usuario.NombreCompleto"/>, luego
    /// <see cref="Usuario.Username"/> y, si no hay nada, un texto por defecto.
    /// Las filas anteriores a la migracion pueden traer nombre_completo NULL o en
    /// blanco, y un label vacio en el sidebar se ve roto.
    /// </summary>
    public static class UsuarioHelper
    {
        /// <summary>Texto por defecto cuando no hay sesion activa.</summary>
        public const string SinSesion = "Sin sesión";

        /// <summary>Texto por defecto cuando hay usuario pero sin nombres utilizables.</summary>
        public const string SinNombre = "Usuario";

        /// <summary>Inicial de respaldo cuando no hay ni nombre ni usuario.</summary>
        public const string InicialPorDefecto = "?";

        /// <summary>
        /// Nombre a mostrar: nombre completo si tiene contenido, si no el usuario,
        /// y si tampoco hay sesión el texto por defecto. Nunca devuelve cadena vacia.
        /// </summary>
        /// <param name="usuario">Usuario de la sesión; puede ser null.</param>
        public static string NombreMostrar(Usuario? usuario)
        {
            if (usuario is null)
            {
                return SinSesion;
            }

            string nombre = usuario.NombreCompleto?.Trim() ?? string.Empty;
            if (nombre.Length > 0)
            {
                return nombre;
            }

            string username = usuario.Username?.Trim() ?? string.Empty;
            return username.Length > 0 ? username : SinNombre;
        }

        /// <summary>
        /// Letra del avatar, en mayuscula: la del nombre completo, o la del usuario
        /// si el nombre esta vacio. Nunca lanza: un Substring sobre cadena vacia o
        /// de un solo caracter es el fallo tipico de este calculo.
        /// </summary>
        /// <param name="usuario">Usuario de la sesión; puede ser null.</param>
        public static string Inicial(Usuario? usuario)
        {
            string candidata = NombreMostrar(usuario);
            if (candidata.Length == 0 || candidata == SinSesion)
            {
                return InicialPorDefecto;
            }

            return candidata[..1].ToUpperInvariant();
        }

        /// <summary>
        /// El login (username) tal cual, recortado. Cadena vacia si no hay sesión o
        /// si el usuario quedo sin nombre de acceso.
        /// </summary>
        /// <param name="usuario">Usuario de la sesión; puede ser null.</param>
        public static string Login(Usuario? usuario) => usuario?.Username?.Trim() ?? string.Empty;

        /// <summary>
        /// El correo recortado, o cadena vacia si no tiene. La base lo guarda
        /// nullable: NULL significa "no capturado", no un correo vacio de verdad.
        /// </summary>
        /// <param name="usuario">Usuario de la sesión; puede ser null.</param>
        public static string Correo(Usuario? usuario) => usuario?.Email?.Trim() ?? string.Empty;

        /// <summary>
        /// Texto del tooltip de la barra lateral: nombre, login y correo, cada uno
        /// solo si aporta. Con el sidebar colapsado el tooltip es lo unico que deja
        /// ver estos datos, por eso lleva los tres.
        /// </summary>
        /// <param name="usuario">Usuario de la sesión; puede ser null.</param>
        public static string Tooltip(Usuario? usuario)
        {
            string nombre = NombreMostrar(usuario);
            string login = Login(usuario);
            string correo = Correo(usuario);

            // El login va entre parentesis detras del nombre, como en un @mencion;
            // solo se anade si no es el mismo dato que ya esta en el nombre.
            string texto = nombre;
            if (login.Length > 0 && !string.Equals(login, nombre, StringComparison.OrdinalIgnoreCase))
            {
                texto = $"{nombre} (@{login})";
            }

            if (correo.Length > 0)
            {
                texto = $"{texto} · {correo}";
            }

            return texto;
        }
    }
}
