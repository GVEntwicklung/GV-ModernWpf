using System.Globalization;

namespace ModernWpf.Controls
{
    /// <summary>
    /// Kultur-unabhängiges Parsen von Zahleneingaben für die <see cref="NumberBox"/> (VisionQM CAQ-352).
    /// Akzeptiert wissenschaftliche Notation (z.B. "1e-5", "1,5e-9", "2E+3") und sowohl '.' als auch ','
    /// als Dezimaltrenner. Tausendertrenner werden bewusst NICHT unterstützt, damit "1.5e-9" nicht als
    /// "15e-9" fehlinterpretiert wird.
    /// </summary>
    public static class NumberBoxTextParser
    {
        private const NumberStyles Styles =
            NumberStyles.AllowLeadingWhite | NumberStyles.AllowTrailingWhite |
            NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint |
            NumberStyles.AllowExponent;

        public static double? Parse(string text)
        {
            if (string.IsNullOrWhiteSpace(text))
            {
                return null;
            }

            string normalized = text.Trim().Replace(',', '.');
            return double.TryParse(normalized, Styles, CultureInfo.InvariantCulture, out double result)
                ? result
                : null;
        }
    }
}
