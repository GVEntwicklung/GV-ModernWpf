using System.Windows;

namespace ModernWpf.Controls
{
    /// <summary>
    /// Represents an icon that uses a vector path as its content.
    /// </summary>
    public class SvgIcon : IconElement
    {
        static SvgIcon()
        {
            ForegroundProperty.OverrideMetadata(typeof(SvgIcon), new FrameworkPropertyMetadata(OnForegroundChanged));
        }

        /// <summary>
        /// Initializes a new instance of the SvgIcon class.
        /// </summary>
        public SvgIcon()
        {
        }

        #region Data

        /// <summary>
        /// Identifies the Data dependency property.
        /// </summary>
        public static readonly DependencyProperty SvgSourceProperty =
            SharpVectors.Converters.SvgIcon.SvgSourceProperty.AddOwner(typeof(SvgIcon),
                new FrameworkPropertyMetadata(OnSvgSourceChanged));

        /// <summary>
        /// Gets or sets a Geometry that specifies the shape to be drawn. In XAML. this can
        /// also be set using a string that describes Move and draw commands syntax.
        /// </summary>
        /// <returns>A description of the shape to be drawn.</returns>
        public string SvgSource
        {
            get => (string)GetValue(SvgSourceProperty);
            set => SetValue(SvgSourceProperty, value);
        }

        private static void OnSvgSourceChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SvgIcon)d).ApplyData();
        }

        #endregion

        private protected override void InitializeChildren()
        {
            _svgIcon = new SharpVectors.Converters.SvgIcon
            {
                HorizontalAlignment = HorizontalAlignment.Stretch,
                VerticalAlignment = VerticalAlignment.Stretch,
            };

            ApplyForeground();
            ApplyData();

            Children.Add(_svgIcon);
        }

        private protected override void OnShouldInheritForegroundFromVisualParentChanged()
        {
            ApplyForeground();
        }

        private protected override void OnVisualParentForegroundPropertyChanged(DependencyPropertyChangedEventArgs args)
        {
            if (ShouldInheritForegroundFromVisualParent)
            {
                ApplyForeground();
            }
        }

        private static void OnForegroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((SvgIcon)d).ApplyForeground();
        }

        private void ApplyForeground()
        {
            if (_svgIcon != null)
            {
                _svgIcon.Fill = ShouldInheritForegroundFromVisualParent ? VisualParentForeground : Foreground;
            }
        }

        private void ApplyData()
        {
            if (_svgIcon != null)
            {
                _svgIcon.SvgSource = SvgSource;
            }
        }

        private SharpVectors.Converters.SvgIcon _svgIcon;
    }
}
