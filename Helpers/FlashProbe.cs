using System;
using System.IO;

// TEMPORAL-DEBUG: sonda para diagnosticar el flash naranja al abrir la OC.
// Se elimina cuando se confirme la causa. No cambia comportamiento: solo anexa
// lineas con marca de tiempo a %TEMP%\oc_flash.log.
namespace Ritrama2025.Helpers;

internal static class FlashProbe
{
    private static readonly object _lock = new();

    public static string RutaLog => Path.Combine(Path.GetTempPath(), "oc_flash.log");

    public static void Reset()
    {
        try { File.Delete(RutaLog); }
        catch { }
    }

    public static void Log(string evento)
    {
        try
        {
            lock (_lock)
            {
                File.AppendAllText(RutaLog,
                    $"{DateTime.Now:HH:mm:ss.fff} [hilo:{Environment.CurrentManagedThreadId}] {evento}{Environment.NewLine}");
            }
        }
        catch { }
    }
}
