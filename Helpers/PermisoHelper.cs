namespace Ritrama2025.Helpers
{
    public static class PermisoHelper
    {
        public static bool TienePermiso(string modulo, string accion)
        {
            if (!SesionActual.IsAuthenticated)
            {
                return false;
            }

            return SesionActual.Permisos.Contains($"{modulo}:{accion}");
        }

        public static bool PuedeVer(string modulo) => TienePermiso(modulo, "Ver");
        public static bool PuedeCrear(string modulo) => TienePermiso(modulo, "Crear");
        public static bool PuedeEditar(string modulo) => TienePermiso(modulo, "Editar");
        public static bool PuedeEliminar(string modulo) => TienePermiso(modulo, "Eliminar");
    }
}
