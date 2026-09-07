using System;

namespace Ritrama2025.Services.ProduccionService
{
    public static class ServiceErrors
    {
        public static Action<string> Report { get; set; } = DefaultReport;

        /// <summary>
        /// Comportamiento por defecto: registra el mensaje (ServiceLogger) y notifica al usuario.
        /// </summary>
        internal static void DefaultReport(string msg)
        {
            // Registro centralizado + notificacion al usuario (sin bloquear la app si el log falla).
            ServiceLogger.Log(msg);
            Notify(msg);
        }

        /// <summary>
        /// Canal de notificacion al usuario. Por defecto usa MessageBox; se puede redirigir (tests, UI).
        /// </summary>
        public static Action<string> Notify { get; set; } = msg => System.Windows.Forms.MessageBox.Show(msg);
    }
}
