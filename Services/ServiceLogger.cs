using System;
using System.IO;

namespace Ritrama2025.Services.ProduccionService
{
    /// <summary>
    /// Registro centralizado de mensajes/errores del servicio de produccion.
    /// Por defecto escribe en un archivo de log en la carpeta de datos locales de la aplicacion;
    /// se puede redirigir (Sink) para tests o para integrarlo con otro sistema de logs.
    /// El registro nunca debe interrumpir la ejecucion de la aplicacion.
    /// </summary>
    public static class ServiceLogger
    {
        private static readonly string LogPath = Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
            "Ritrama2025", "servicio.log");

        /// <summary>
        /// Redirige el destino de los mensajes. Si es null, se usa el archivo de log por defecto.
        /// </summary>
        public static Action<string>? Sink { get; set; }

        public static void Log(string message)
        {
            try
            {
                if (Sink != null)
                {
                    Sink(message);
                    return;
                }

                var directorio = Path.GetDirectoryName(LogPath);
                if (!string.IsNullOrEmpty(directorio))
                {
                    Directory.CreateDirectory(directorio!);
                }

                var linea = $"[{DateTime.Now:yyyy-MM-dd HH:mm:ss}] {message}{Environment.NewLine}";
                File.AppendAllText(LogPath, linea);
            }
            catch
            {
                // el registro nunca debe romper la aplicacion
            }
        }

        public static void LogError(string contexto, Exception ex)
        {
            Log($"ERROR [{contexto}]: {ex.Message}");
        }
    }
}
