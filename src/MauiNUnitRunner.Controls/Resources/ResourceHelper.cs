// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

namespace MauiNUnitRunner.Controls.Resources;

/// <summary>
///     Helper class for accessing resources.
/// </summary>
internal static class ResourceHelper
{
    #region Private Members

    /// <summary>
    ///     Holds the overridden <see cref="CurrentApplication"/> value.
    /// </summary>
    private static Application v_CurrentApplication;

    /// <summary>
    ///     Holds the overridden <see cref="ResourceDictionary"/> value.
    /// </summary>
    private static ResourceDictionary v_ResourceDictionary;

    #endregion

    #region Internal Members

    /// <summary>
    ///     Gets or sets if to use overridden static values.
    /// </summary>
    /// <remarks>This property is intended for use with unit tests.</remarks>
    internal static bool UseOverriddenCurrentApplication { get; set; }

    /// <summary>
    ///     Gets or sets the current <see cref="Application"/> to access.
    /// </summary>
    internal static Application CurrentApplication
    {
        get => UseOverriddenCurrentApplication ? v_CurrentApplication : Application.Current;
        set => v_CurrentApplication = value;
    }

    /// <summary>
    ///     Gets or sets if to use overridden static values.
    /// </summary>
    /// <remarks>This property is intended for use with unit tests.</remarks>
    internal static bool UseOverriddenResourceDictionary { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="Microsoft.Maui.Controls.ResourceDictionary"/> to access.
    /// </summary>
    internal static ResourceDictionary ResourceDictionary
    {
        get => UseOverriddenResourceDictionary ? v_ResourceDictionary : CurrentApplication?.Resources;
        set => v_ResourceDictionary = value;
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///     Gets the resource string for the given key, or null if not found or not a string.
    /// </summary>
    /// <param name="key">The resource string key to get</param>
    /// <returns>The resource string for the given key, or null if not found or not a string.</returns>
    public static string GetResourceString(string key)
    {
        if (ResourceDictionary != null && ResourceDictionary.TryGetValue(key, out object valueStr))
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
        if (ResourceDictionary != null && ResourceDictionary.TryGetValue(key, out object style))
        {
            return (style as Style)?.Setters?.FirstOrDefault(x => x?.Property?.PropertyName == "TextColor")?.Value as Color;
        }

        return null;
    }

    /// <summary>
    ///     Gets the foreground color associated with the current app theme (light or dark).
    /// </summary>
    /// <returns>The foreground color associated with the current app theme.</returns>
    public static Color GetCurrentThemeForegroundColor()
    {
        switch (CurrentApplication?.UserAppTheme)
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

    #endregion
}