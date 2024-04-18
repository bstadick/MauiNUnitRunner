// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Globalization;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Binding value converter to invert a boolean.
/// </summary>
public class InverseBoolConverter : IValueConverter
{
    #region Implementation of IValueConverter

    /// <inheritdoc />
    public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return InvertBool(value);
    }

    /// <inheritdoc />
    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
    {
        return InvertBool(value);
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Inverts the boolean value.
    /// </summary>
    /// <param name="value">The value to invert.</param>
    /// <returns>The inverted boolean value, or false if not a boolean or parseable string boolean.</returns>
    private static bool InvertBool(object value)
    {
        if (TryCastToBool(value, out bool boolValue))
        {
            return !boolValue;
        }

        return false;
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