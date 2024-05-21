// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Globalization;
using MauiNUnitRunner.Controls.Resources;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Binding value converter to select the disabled button style when true is passed.
/// </summary>
public class ButtonDisabledStyleConverter : IValueConverter
{
    #region Implementation of IValueConverter

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return SelectStyle(value);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        Style valueStyle = value as Style;
        return valueStyle != null && valueStyle.BasedOn == ResourceHelper.GetResourceStyle("ButtonStyle") &&
               valueStyle.Setters.Any(x => x.Property.PropertyName == "IsEnabled" && x.Value is false);
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Gets the style if a button is disabled (true is passed).
    /// </summary>
    /// <param name="value">true if to use the disabled style, otherwise use the base button style.</param>
    /// <returns>The button style to use.</returns>
    private static Style SelectStyle(object value)
    {
        if (TryCastToBool(value, out bool boolValue) && boolValue)
        {
            return ResourceHelper.GetResourceStyle("ButtonDisabledStyle");
        }

        return ResourceHelper.GetResourceStyle("ButtonStyle");
    }

    /// <summary>
    ///     Tries and casts the value to a boolean or parses a string as a boolean.
    /// </summary>
    /// <param name="value">The value to cast or parse.</param>
    /// <param name="boolValue">Outputs the boolean value.</param>
    /// <returns>true if the value was successfully cast or parsed, otherwise false.</returns>
    private static bool TryCastToBool(object value, out bool boolValue)
    {
        if (value is bool)
        {
            boolValue = (bool)value;
            return true;
        }

        if (value is string && bool.TryParse((string)value, out boolValue))
        {
            return true;
        }

        boolValue = false;
        return false;
    }

    #endregion
}