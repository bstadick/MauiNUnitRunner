// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.ComponentModel;

namespace MauiNUnitRunner.Controls.Views;

/// <summary>
///     Implements a page to show information about the MauiNUnitRunner project.
/// </summary>
[DesignTimeVisible(false)]
// ReSharper disable once RedundantExtendsListEntry
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

    #region Public Members

    /// <summary>
    ///     Gets the version of the <see cref="AboutPage"/> assembly.
    /// </summary>
    public string AssemblyVersion { get; }

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="AboutPage" />.
    /// </summary>
    public AboutPage() : this(true)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="AboutPage"/> with the option to skip initializing the components.
    /// </summary>
    /// <param name="initializeComponent">true if to initialize the component, otherwise false to skip initialize component.</param>
    protected AboutPage(bool initializeComponent)
    {
        // Assign before initializing so binding has value without needing to be notified
        AssemblyVersion = typeof(AboutPage).Assembly.GetName().Version?.ToString(3) ?? string.Empty;

        if (initializeComponent)
        {
            InitializeComponent();
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///     Opens the provided Uri using the default web browser.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The button click event arguments.</param>
    protected async void OpenUrlButton_OnClicked(object sender, EventArgs e)
    {
        try
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

            await GetDefaultBrowser().OpenAsync(new Uri(uri), BrowserLaunchMode.SystemPreferred).ConfigureAwait(false);
        }
        catch
        {
            // Ignore exceptions here as navigation to the About page's links is not significant
        }
    }

    /// <summary>
    ///     Gets the default <see cref="IBrowser"/>.
    /// </summary>
    /// <returns>The default <see cref="IBrowser"/>.</returns>
    protected virtual IBrowser GetDefaultBrowser()
    {
        return Browser.Default;
    }

    #endregion
}
