// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.ComponentModel;

namespace MauiNUnitRunner.Controls.Views;

[DesignTimeVisible(false)]
public partial class AboutPage : ContentPage
{
    #region Private Members

    /// <summary>
    ///     Url to the MAUI website.
    /// </summary>
    private const string c_MauiUri = "https://learn.microsoft.com/en-us/dotnet/maui";

    /// <summary>
    ///     Url to the NUnit website.
    /// </summary>
    private const string c_NUnitUri = "https://nunit.org/";

    /// <summary>
    ///     Url to this project's website.
    /// </summary>
    private const string c_ProjectUri = "https://github.com/bstadick/MauiNUnitRunner";

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="AboutPage" />.
    /// </summary>
    internal AboutPage()
    {
        InitializeComponent();

        VersionText.Text = GetType().Assembly.GetName().Version?.ToString(3) ?? string.Empty;
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Opens the provided Uri using the default web browser.
    /// </summary>
    /// <remarks>This method will not be covered by unit tests as it invokes opening an external application.</remarks>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The button click event arguments.</param>
    private async void OpenUrlButton_OnClicked(object sender, EventArgs e)
    {
        Button button = sender as Button;
        string uri;
        switch (button?.StyleId)
        {
            case "AboutMauiButton":
                uri = c_MauiUri;
                break;
            case "AboutNUnitButton":
                uri = c_NUnitUri;
                break;
            case "AboutProjectButton":
                uri = c_ProjectUri;
                break;
            default:
                return;
        }

        await Browser.Default.OpenAsync(new Uri(uri), BrowserLaunchMode.SystemPreferred).ConfigureAwait(false);
    }

    #endregion
}