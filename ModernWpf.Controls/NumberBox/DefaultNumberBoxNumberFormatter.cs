namespace ModernWpf.Controls
{
    internal class DefaultNumberBoxNumberFormatter : INumberBoxNumberFormatter
    {
        public string FormatDouble(double value)
        {
            return value.ToString();
        }

        public double? ParseDouble(string text)
        {
            // Delegiert an den kultur-unabhängigen Parser, damit wissenschaftliche Notation
            // (z.B. "1,5e-9") und beide Dezimaltrenner immer akzeptiert werden (VisionQM CAQ-352).
            return NumberBoxTextParser.Parse(text);
        }
    }
}
