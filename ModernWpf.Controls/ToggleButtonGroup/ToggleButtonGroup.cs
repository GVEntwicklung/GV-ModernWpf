using System;
using System.ComponentModel;
using System.Windows;
using System.Windows.Automation.Peers;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using ModernWpf.Automation.Peers;
using ModernWpf.Controls.Primitives;

namespace ModernWpf.Controls
{
    [TemplatePart(Name = PartItemsPanel, Type = typeof(StackPanel))]
    public class ToggleButtonGroup : Control
    {
        private const string PartItemsPanel = "PART_ItemsPanel";

        private StackPanel _itemsPanel;
        private bool _isUpdatingSelection;

        static ToggleButtonGroup()
        {
            DefaultStyleKeyProperty.OverrideMetadata(
                typeof(ToggleButtonGroup),
                new FrameworkPropertyMetadata(typeof(ToggleButtonGroup)));
        }

        #region SelectionChanged event

        public static readonly RoutedEvent SelectionChangedEvent =
            EventManager.RegisterRoutedEvent(
                nameof(SelectionChanged),
                RoutingStrategy.Bubble,
                typeof(RoutedPropertyChangedEventHandler<Enum>),
                typeof(ToggleButtonGroup));

        public event RoutedPropertyChangedEventHandler<Enum> SelectionChanged
        {
            add => AddHandler(SelectionChangedEvent, value);
            remove => RemoveHandler(SelectionChangedEvent, value);
        }

        #endregion

        #region SelectedValue

        public static readonly DependencyProperty SelectedValueProperty =
            DependencyProperty.Register(
                nameof(SelectedValue),
                typeof(Enum),
                typeof(ToggleButtonGroup),
                new FrameworkPropertyMetadata(
                    null,
                    FrameworkPropertyMetadataOptions.BindsTwoWayByDefault,
                    OnSelectedValueChanged,
                    CoerceSelectedValue));

        public Enum SelectedValue
        {
            get => (Enum)GetValue(SelectedValueProperty);
            set => SetValue(SelectedValueProperty, value);
        }

        private static object CoerceSelectedValue(DependencyObject d, object baseValue)
        {
            // Once we have a known enum type, reject null to enforce "always one selected"
            if (baseValue == null)
            {
                var control = (ToggleButtonGroup)d;
                var enumType = control.EnumType;
                if (enumType != null && enumType.IsEnum)
                {
                    var values = Enum.GetValues(enumType);
                    if (values.Length > 0)
                        return (Enum)values.GetValue(0);
                }
            }
            return baseValue;
        }

        private static void OnSelectedValueChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var control = (ToggleButtonGroup)d;

            // Infer EnumType from the first non-null value
            if (e.NewValue != null && control.EnumType == null)
            {
                control.SetCurrentValue(EnumTypeProperty, e.NewValue.GetType());
            }

            control.SyncCheckedState();

            control.RaiseEvent(new RoutedPropertyChangedEventArgs<Enum>(
                (Enum)e.OldValue,
                (Enum)e.NewValue,
                SelectionChangedEvent));
        }

        #endregion

        #region EnumType

        public static readonly DependencyProperty EnumTypeProperty =
            DependencyProperty.Register(
                nameof(EnumType),
                typeof(Type),
                typeof(ToggleButtonGroup),
                new FrameworkPropertyMetadata(null, OnEnumTypeChanged));

        public Type EnumType
        {
            get => (Type)GetValue(EnumTypeProperty);
            set => SetValue(EnumTypeProperty, value);
        }

        private static void OnEnumTypeChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleButtonGroup)d).RebuildButtons();
        }

        #endregion

        #region Orientation

        public static readonly DependencyProperty OrientationProperty =
            DependencyProperty.Register(
                nameof(Orientation),
                typeof(Orientation),
                typeof(ToggleButtonGroup),
                new FrameworkPropertyMetadata(Orientation.Horizontal, OnOrientationChanged));

        public Orientation Orientation
        {
            get => (Orientation)GetValue(OrientationProperty);
            set => SetValue(OrientationProperty, value);
        }

        private static void OnOrientationChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((ToggleButtonGroup)d).RebuildButtons();
        }

        #endregion

        public override void OnApplyTemplate()
        {
            if (_itemsPanel != null)
                ClearButtons();

            base.OnApplyTemplate();
            _itemsPanel = GetTemplateChild(PartItemsPanel) as StackPanel;
            RebuildButtons();
        }

        protected override AutomationPeer OnCreateAutomationPeer()
        {
            return new ToggleButtonGroupAutomationPeer(this);
        }

        private void RebuildButtons()
        {
            if (_itemsPanel == null) return;

            ClearButtons();

            var enumType = EnumType;
            if (enumType == null || !enumType.IsEnum) return;

            var values = Enum.GetValues(enumType);
            var cornerRadius = TryFindResource("ControlCornerRadius") is CornerRadius cr ? cr.TopLeft : 4d;
            var isHorizontal = Orientation == Orientation.Horizontal;

            for (int i = 0; i < values.Length; i++)
            {
                var value = (Enum)values.GetValue(i);
                var isFirst = i == 0;
                var isLast = i == values.Length - 1;

                var button = new ToggleButton
                {
                    Content = GetLabel(enumType, value),
                    Tag = value,
                    IsChecked = value.Equals(SelectedValue),
                    Style = TryFindResource("ToggleButtonGroupItemStyle") as Style,
                };

                // Rounded corners only on the outer edges of the strip
                var radius = isHorizontal
                    ? new CornerRadius(isFirst ? cornerRadius : 0, isLast ? cornerRadius : 0,
                                       isLast ? cornerRadius : 0, isFirst ? cornerRadius : 0)
                    : new CornerRadius(isFirst ? cornerRadius : 0, isFirst ? cornerRadius : 0,
                                       isLast ? cornerRadius : 0, isLast ? cornerRadius : 0);
                button.SetValue(ControlHelper.CornerRadiusProperty, radius);

                // Collapse the shared border between adjacent buttons
                if (!isFirst)
                {
                    button.Margin = isHorizontal
                        ? new Thickness(-1, 0, 0, 0)
                        : new Thickness(0, -1, 0, 0);
                }

                button.Checked += OnButtonChecked;
                button.Unchecked += OnButtonUnchecked;

                _itemsPanel.Children.Add(button);
            }
        }

        private void ClearButtons()
        {
            foreach (ToggleButton button in _itemsPanel.Children)
            {
                button.Checked -= OnButtonChecked;
                button.Unchecked -= OnButtonUnchecked;
            }
            _itemsPanel.Children.Clear();
        }

        private static string GetLabel(Type enumType, Enum value)
        {
            var field = enumType.GetField(value.ToString());
            if (field != null)
            {
                var attr = (DescriptionAttribute)Attribute.GetCustomAttribute(
                    field, typeof(DescriptionAttribute));
                if (attr != null)
                    return attr.Description;
            }
            return value.ToString();
        }

        private void OnButtonChecked(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingSelection) return;
            var button = (ToggleButton)sender;
            SetCurrentValue(SelectedValueProperty, (Enum)button.Tag);
        }

        private void OnButtonUnchecked(object sender, RoutedEventArgs e)
        {
            if (_isUpdatingSelection) return;
            var button = (ToggleButton)sender;
            // Prevent deselecting the active value — always keep one checked
            if (button.Tag.Equals(SelectedValue))
            {
                _isUpdatingSelection = true;
                button.IsChecked = true;
                _isUpdatingSelection = false;
            }
        }

        private void SyncCheckedState()
        {
            if (_itemsPanel == null) return;

            _isUpdatingSelection = true;
            foreach (ToggleButton button in _itemsPanel.Children)
            {
                button.IsChecked = button.Tag.Equals(SelectedValue);
            }
            _isUpdatingSelection = false;
        }
    }
}
