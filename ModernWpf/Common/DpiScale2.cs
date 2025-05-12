using System.Windows;

namespace ModernWpf
{
    internal readonly record struct DpiScale2
    {
        public DpiScale2(double dpiScaleX, double dpiScaleY)
        {
            DpiScaleX = dpiScaleX;
            DpiScaleY = dpiScaleY;
        }

        public DpiScale2(DpiScale dpiScale)
            : this(dpiScale.DpiScaleX, dpiScale.DpiScaleY)
        {
        }

        public double DpiScaleX { get; }
        public double DpiScaleY { get; }
    }
}
