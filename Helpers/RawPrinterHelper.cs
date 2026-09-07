using System.Runtime.InteropServices;

namespace Ritrama2025.Helpers
{
    public static partial class RawPrinterHelper
    {
        [StructLayout(LayoutKind.Sequential, CharSet = CharSet.Ansi)]
        public struct DOCINFOA
        {
            [MarshalAs(UnmanagedType.LPStr)] public string? pDocName;
            [MarshalAs(UnmanagedType.LPStr)] public string? pOutputFile;
            [MarshalAs(UnmanagedType.LPStr)] public string? pDataType;
        }

        [LibraryImport("winspool.drv", EntryPoint = "OpenPrinterA", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool OpenPrinter(
            [MarshalAs(UnmanagedType.LPStr)] string szPrinter,
            out IntPtr hPrinter,
            IntPtr pd);

        [LibraryImport("winspool.drv", EntryPoint = "ClosePrinter", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool ClosePrinter(IntPtr hPrinter);

        // Sustituir LibraryImport por DllImport para StartDocPrinter
        [DllImport("winspool.drv", EntryPoint = "StartDocPrinterA", SetLastError = true, CharSet = CharSet.Ansi)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool StartDocPrinter(IntPtr hPrinter, int level, ref DOCINFOA di);

        [LibraryImport("winspool.drv", EntryPoint = "EndDocPrinter", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool EndDocPrinter(IntPtr hPrinter);

        [LibraryImport("winspool.drv", EntryPoint = "StartPagePrinter", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool StartPagePrinter(IntPtr hPrinter);

        [LibraryImport("winspool.drv", EntryPoint = "EndPagePrinter", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool EndPagePrinter(IntPtr hPrinter);

        [LibraryImport("winspool.drv", EntryPoint = "WritePrinter", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static partial bool WritePrinter(IntPtr hPrinter, IntPtr pBytes, int dwCount, out int dwWritten);

        public static bool SendStringToPrinter(string printerName, string zpl)
        {
            if (!OpenPrinter(printerName, out IntPtr hPrinter, IntPtr.Zero))
                return false;

            var di = new DOCINFOA
            {
                pDocName = "ZPL Label",
                pDataType = "RAW"
            };

            if (!StartDocPrinter(hPrinter, 1, ref di))
            {
                ClosePrinter(hPrinter);
                return false;
            }

            StartPagePrinter(hPrinter);

            IntPtr pBytes = Marshal.StringToCoTaskMemAnsi(zpl);
            WritePrinter(hPrinter, pBytes, zpl.Length, out int dwWritten);
            Marshal.FreeCoTaskMem(pBytes);

            EndPagePrinter(hPrinter);
            EndDocPrinter(hPrinter);
            ClosePrinter(hPrinter);

            return dwWritten == zpl.Length;
        }
    }
}
