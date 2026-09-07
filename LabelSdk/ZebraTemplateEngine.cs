using Ritrama2025.Helpers;

namespace Ritrama2025.LabelSdk
{
    public static class ZebraTemplateEngine
    {
        public static int MmToDots(double mm, int dpi) => (int)Math.Round(mm * dpi / 25.4);
        public static int InchesToDots(double inches, int dpi) => (int)Math.Round(inches * dpi);

        public static string BuildLabelHeader(double width, double height, int dpi, bool useInches = false)
        {
            int widthDots = useInches ? InchesToDots(width, dpi) : MmToDots(width, dpi);
            int heightDots = useInches ? InchesToDots(height, dpi) : MmToDots(height, dpi);
            return $"^XA^PW{widthDots}^LL{heightDots}^LH0,0^FS";
        }

        public static string Render(string template, Dictionary<string, string> values)
        {
            string result = template;
            foreach (var kv in values)
            {
                result = result.Replace("{" + kv.Key + "}", kv.Value ?? string.Empty);
            }
            return result;
        }

        public static string BuildLabel(LabelSize size, string template, Dictionary<string, string> values)
        {
            string header = BuildLabelHeader(size.WidthInches, size.HeightInches, size.dpi, useInches: true);
            string body = Render(template, values);
            return header + body + "\n^XZ";
        }

        public static bool Print(string printerName, string template, Dictionary<string, string> values, LabelSize size)
        {
            string zpl = BuildLabel(size, template, values);
            return RawPrinterHelper.SendStringToPrinter(printerName, zpl);

        }

    }
}
