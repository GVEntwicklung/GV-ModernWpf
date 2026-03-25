using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives
{
    public static class TextBoxHelper
    {
        private const string ButtonStatesGroup = "ButtonStates";
        private const string DeleteButtonVisibleState = "DeleteButtonVisible";
        private const string CopyButtonVisibleState = "CopyButtonVisible";
        private const string ButtonCollapsedState = "ButtonCollapsed";

        #region IsEnabled

        public static bool GetIsEnabled(TextBox textBox)
        {
            return (bool)textBox.GetValue(IsEnabledProperty);
        }

        public static void SetIsEnabled(TextBox textBox, bool value)
        {
            textBox.SetValue(IsEnabledProperty, value);
        }

        public static readonly DependencyProperty IsEnabledProperty =
            DependencyProperty.RegisterAttached(
                "IsEnabled",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(OnIsEnabledChanged));

        private static void OnIsEnabledChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            TextBox textBox = (TextBox)d;

            if ((bool)e.NewValue)
            {
                textBox.Loaded += OnLoaded;
                textBox.TextChanged += OnTextChanged;
                UpdateHasText(textBox);

            }
            else
            {
                textBox.Loaded -= OnLoaded;
                textBox.TextChanged -= OnTextChanged;
                textBox.ClearValue(HasTextPropertyKey);
            }
        }

        #endregion

        #region HasText

        private static readonly DependencyPropertyKey HasTextPropertyKey =
            DependencyProperty.RegisterAttachedReadOnly(
                "HasText",
                typeof(bool),
                typeof(TextBoxHelper),
                null);

        public static readonly DependencyProperty HasTextProperty =
            HasTextPropertyKey.DependencyProperty;

        public static bool GetHasText(TextBox textBox)
        {
            return (bool)textBox.GetValue(HasTextProperty);
        }

        private static void SetHasText(TextBox textBox, bool value)
        {
            textBox.SetValue(HasTextPropertyKey, value);
        }

        private static void UpdateHasText(TextBox textBox)
        {
            SetHasText(textBox, !string.IsNullOrEmpty(textBox.Text));
        }

        #endregion

        #region IsDeleteButton

        public static bool GetIsDeleteButton(Button button)
        {
            return (bool)button.GetValue(IsDeleteButtonProperty);
        }

        public static void SetIsDeleteButton(Button button, bool value)
        {
            button.SetValue(IsDeleteButtonProperty, value);
        }

        public static readonly DependencyProperty IsDeleteButtonProperty =
            DependencyProperty.RegisterAttached(
                "IsDeleteButton",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(OnIsDeleteButtonChanged));

        private static void OnIsDeleteButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = (Button)d;

            if ((bool)e.OldValue)
            {
                button.Click -= OnDeleteButtonClick;
            }

            if ((bool)e.NewValue)
            {
                button.Click += OnDeleteButtonClick;
            }
        }

        #endregion

        #region IsDeleteButtonVisible

        public static readonly DependencyProperty IsDeleteButtonVisibleProperty =
            DependencyProperty.RegisterAttached(
                "IsDeleteButtonVisible",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(OnIsDeleteButtonVisibleChanged));

        public static bool GetIsDeleteButtonVisible(TextBox textBox)
        {
            return (bool)textBox.GetValue(IsDeleteButtonVisibleProperty);
        }

        public static void SetIsDeleteButtonVisible(TextBox textBox, bool value)
        {
            textBox.SetValue(IsDeleteButtonVisibleProperty, value);
        }

        private static void OnIsDeleteButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            UpdateVisualStates((TextBox)d);
        }

        #endregion

        #region IsCopyButton

        public static bool GetIsCopyButton(Button button)
        {
            return (bool)button.GetValue(IsCopyButtonProperty);
        }

        public static void SetIsCopyButton(Button button, bool value)
        {
            button.SetValue(IsCopyButtonProperty, value);
        }

        public static readonly DependencyProperty IsCopyButtonProperty =
            DependencyProperty.RegisterAttached(
                "IsCopyButton",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(OnIsCopyButtonChanged));

        private static void OnIsCopyButtonChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Button button = (Button)d;

            if ((bool)e.OldValue)
            {
                button.Click -= OnCopyButtonClick;
            }

            if ((bool)e.NewValue)
            {
                button.Click += OnCopyButtonClick;
            }
        }

        #endregion

        #region IsCopyButtonVisible

        public static readonly DependencyProperty IsCopyButtonVisibleProperty =
            DependencyProperty.RegisterAttached(
                "IsCopyButtonVisible",
                typeof(bool),
                typeof(TextBoxHelper),
                new PropertyMetadata(OnIsCopyButtonVisibleChanged));

        public static bool GetIsCopyButtonVisible(TextBox textBox)
        {
            return (bool)textBox.GetValue(IsCopyButtonVisibleProperty);
        }

        public static void SetIsCopyButtonVisible(TextBox textBox, bool value)
        {
            textBox.SetValue(IsCopyButtonVisibleProperty, value);
        }

        private static void OnIsCopyButtonVisibleChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            // Templates/control styles decide how to present the copy button.
            // Keep parity with delete button visible change handler so consumers can use this property in triggers.
            UpdateVisualStates((TextBox)d);
        }

        #endregion

        private static void OnLoaded(object sender, RoutedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            UpdateVisualStates(textBox);
        }

        private static void OnTextChanged(object sender, TextChangedEventArgs e)
        {
            TextBox textBox = (TextBox)sender;
            UpdateHasText(textBox);
        }

        private static void OnDeleteButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.TemplatedParent is TextBox textBox)
            {
                textBox.SetCurrentValue(TextBox.TextProperty, null);
            }
        }

        private static void OnCopyButtonClick(object sender, RoutedEventArgs e)
        {
            Button button = (Button)sender;
            if (button.TemplatedParent is TextBox textBox)
            {
                Clipboard.SetText(textBox.Text ?? string.Empty);
            }
        }

        private static void UpdateVisualStates(TextBox textBox)
        {
            VisualStateManager.GoToState(textBox,
                                         GetIsDeleteButtonVisible(textBox)
                                             ? DeleteButtonVisibleState
                                             : GetIsCopyButtonVisible(textBox)
                                                 ? CopyButtonVisibleState
                                                 : ButtonCollapsedState,
                                         true);
        }
    }
}
