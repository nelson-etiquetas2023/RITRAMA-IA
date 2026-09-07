namespace Ritrama2025.LabelSdk
{
    public record LabelSize(double WidthInches, double HeightInches, int dpi);

    public static class StandardLabelSizes
    {
        public static LabelSize Size_2x1_203dpi => new LabelSize(2.0, 1.0, 203);
        public static LabelSize Size_3x2_203dpi => new LabelSize(3.0, 2.0, 203);
        public static LabelSize Size_4x3_203dpi => new LabelSize(4.0, 3.0, 203);
        public static LabelSize Size_4x6_203dpi => new LabelSize(4.0, 6.0, 203);
        public static LabelSize Size_2x1_300dpi => new LabelSize(2.0, 1.0, 300);
        public static LabelSize Size_3x2_300dpi => new LabelSize(3.0, 2.0, 300);
        public static LabelSize Size_4x3_300dpi => new LabelSize(4.0, 3.0, 300);
        public static LabelSize Size_4x6_300dpi => new LabelSize(4.0, 6.0, 300);
    }
}
