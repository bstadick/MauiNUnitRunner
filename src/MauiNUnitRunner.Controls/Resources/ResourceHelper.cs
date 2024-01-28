// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

namespace MauiNUnitRunner.Controls.Resources;

/// <summary>
///     Helper class for accessing resources.
/// </summary>
internal static class ResourceHelper
{
    /// <summary>
    ///     Gets the resource string for the given key, or null if not found or not a string.
    /// </summary>
    /// <param name="key">The resource string key to get</param>
    /// <returns>The resource string for the given key, or null if not found or not a string.</returns>
    public static string GetResourceString(string key)
    {
        if (Application.Current != null &&
            Application.Current.Resources.TryGetValue(key, out object valueStr))
        {
            return valueStr?.ToString();
        }

        return null;
    }

    /// <summary>
    ///     Gets the resource text color for the given key, or null if not found or not a color.
    /// </summary>
    /// <param name="key">The resource text color key to get</param>
    /// <returns>The resource text color for the given key, or null if not found or not a color.</returns>
    public static Color GetResourceTextColor(string key)
    {
        if (Application.Current != null &&
            Application.Current.Resources.TryGetValue(key, out object valueStr))
        {
            return ((Style)valueStr).Setters.FirstOrDefault(x => x.Property.PropertyName == "TextColor")?.Value as Color;
        }

        return null;
    }

    /// <summary>
    ///     Gets the foreground color associated with the current app theme (light or dark).
    /// </summary>
    /// <returns>The foreground color associated with the current app theme.</returns>
    public static Color GetCurrentThemeForegroundColor()
    {
        switch (Application.Current?.UserAppTheme)
        {
            case AppTheme.Unspecified:
                return Colors.Black;
            case AppTheme.Light:
                return Colors.Black;
            case AppTheme.Dark:
                return Colors.White;
            case null:
                return Colors.Black;
            default:
                return Colors.Black;
        }
    }
}