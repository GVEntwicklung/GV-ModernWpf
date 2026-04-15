using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls
{
    public enum InfoBarSeverity
    {
        Informational,
        Success,
        Warning,
        Error,
    }

    [TemplatePart(Name = CloseButtonName, Type = typeof(Button))]
    public class InfoBar : Control
    {
        private const string CloseButtonName = "PART_CloseButton";

        static InfoBar()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(InfoBar), new FrameworkPropertyMetadata(typeof(InfoBar)));
        }

        public static readonly RoutedEvent ClosedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(Closed),
                RoutingStrategy.Bubble,
                typeof(RoutedEventHandler),
                typeof(InfoBar));

        public event RoutedEventHandler Closed
        {
            add => AddHandler(ClosedEvent, value);
            remove => RemoveHandler(ClosedEvent, value);
        }

        #region Title

        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register(
                nameof(Title),
                typeof(string),
                typeof(InfoBar),
                new PropertyMetadata(string.Empty));

        public string Title
        {
            get => (string)GetValue(TitleProperty);
            set => SetValue(TitleProperty, value);
        }

        #endregion

        #region Message

        public static readonly DependencyProperty MessageProperty =
            DependencyProperty.Register(
                nameof(Message),
                typeof(string),
                typeof(InfoBar),
                new PropertyMetadata(string.Empty));

        public string Message
        {
            get => (string)GetValue(MessageProperty);
            set => SetValue(MessageProperty, value);
        }

        #endregion

        #region Severity

        public static readonly DependencyProperty SeverityProperty =
            DependencyProperty.Register(
                nameof(Severity),
                typeof(InfoBarSeverity),
                typeof(InfoBar),
                new PropertyMetadata(InfoBarSeverity.Informational));

        public InfoBarSeverity Severity
        {
            get => (InfoBarSeverity)GetValue(SeverityProperty);
            set => SetValue(SeverityProperty, value);
        }

        #endregion

        #region IsOpen

        public static readonly DependencyProperty IsOpenProperty =
            DependencyProperty.Register(
                nameof(IsOpen),
                typeof(bool),
                typeof(InfoBar),
                new PropertyMetadata(true, OnIsOpenChanged));

        public bool IsOpen
        {
            get => (bool)GetValue(IsOpenProperty);
            set => SetValue(IsOpenProperty, value);
        }

        private static void OnIsOpenChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
            => ((InfoBar)d).UpdateVisibility();

        #endregion

        #region IsClosable

        public static readonly DependencyProperty IsClosableProperty =
            DependencyProperty.Register(
                nameof(IsClosable),
                typeof(bool),
                typeof(InfoBar),
                new PropertyMetadata(true));

        public bool IsClosable
        {
            get => (bool)GetValue(IsClosableProperty);
            set => SetValue(IsClosableProperty, value);
        }

        #endregion

        #region CornerRadius

        public static readonly DependencyProperty CornerRadiusProperty =
            ControlHelper.CornerRadiusProperty.AddOwner(typeof(InfoBar));

        public CornerRadius CornerRadius
        {
            get => (CornerRadius)GetValue(CornerRadiusProperty);
            set => SetValue(CornerRadiusProperty, value);
        }

        #endregion

        private Button _closeButton;

        public override void OnApplyTemplate()
        {
            if (_closeButton != null)
                _closeButton.Click -= OnCloseButtonClick;

            base.OnApplyTemplate();

            _closeButton = GetTemplateChild(CloseButtonName) as Button;
            if (_closeButton != null)
                _closeButton.Click += OnCloseButtonClick;

            UpdateVisibility();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
            => new InfoBarAutomationPeer(this);

        private void OnCloseButtonClick(object sender, RoutedEventArgs e)
        {
            SetCurrentValue(IsOpenProperty, false);
            RaiseEvent(new RoutedEventArgs(ClosedEvent));
        }

        private void UpdateVisibility()
            => Visibility = IsOpen ? Visibility.Visible : Visibility.Collapsed;
    }
}
