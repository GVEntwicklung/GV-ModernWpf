// Copyright (c) Microsoft Corporation. All rights reserved.
// Licensed under the MIT License. See LICENSE in the project root for license information.

using System.Windows;
using System.Windows.Controls;

namespace ModernWpf.Controls.Primitives
{
    public sealed class CheckBoxHelper
    {
        internal CheckBoxHelper()
        {
        }

        /// <summary>
        /// Identifies the ShowGlyphs dependency property.
        /// </summary>
        public static readonly DependencyProperty ShowGlyphsProperty =
            DependencyProperty.RegisterAttached(
                "ShowGlyphs",
                typeof(bool),
                typeof(CheckBoxHelper),
                new PropertyMetadata(true));

        /// <summary>
        /// Gets whether the CheckBox should display glyphs or not.
        /// </summary>
        /// <param name="checkBox">The element from which to read the property value.</param>
        /// <returns>Whether the CheckBox should display glyphs or not.</returns>
        public static bool GetShowGlyphs(CheckBox checkBox)
        {
            return (bool)checkBox.GetValue(ShowGlyphsProperty);
        }

        /// <summary>
        /// Sets whether the CheckBox should display glyphs or not.
        /// </summary>
        /// <param name="checkBox">The element on which to set the attached property.</param>
        /// <param name="value">The property value to set.</param>
        public static void SetShowGlyphs(CheckBox checkBox, bool value)
        {
            checkBox.SetValue(ShowGlyphsProperty, value);
        }
    }
}
