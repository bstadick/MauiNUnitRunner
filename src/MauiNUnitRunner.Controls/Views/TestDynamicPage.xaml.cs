// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using CommunityToolkit.Maui.Storage;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Resources;
using MauiNUnitRunner.Controls.Services;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using ExceptionHelper = MauiNUnitRunner.Controls.Resources.ExceptionHelper;

namespace MauiNUnitRunner.Controls.Views;

/// <summary>
///     Implements a basic test runner page to display a <see cref="TestDynamicView"/> that contains either a test suite in a <see cref="TestSummaryView"/> or a single test details in a <see cref="TestDetailView"/>.
/// </summary>
public partial class TestDynamicPage : ContentPage
{
    #region Private Members

    /// <summary>
    ///     The file saver instance.
    /// </summary>
    private readonly IFileSaver v_FileSaver = FileSaver.Default;

    #endregion

    #region Public Members

    /// <summary>
    ///     The bindable <see cref="Test"/> property.
    /// </summary>
    public static readonly BindableProperty TestProperty =
        BindableProperty.Create(nameof(Test), typeof(INUnitTest), typeof(TestDynamicPage));

    /// <summary>
    ///     Gets or sets the <see cref="INUnitTest"/> to bind to the page.
    /// </summary>
    public INUnitTest Test
    {
        get => (INUnitTest)GetValue(TestProperty);
        set => SetValue(TestProperty, value);
    }

    /// <summary>
    ///     The bindable <see cref="ShowFooterLinks"/> property.
    /// </summary>
    public static readonly BindableProperty ShowFooterLinksProperty =
        BindableProperty.Create(nameof(ShowFooterLinks), typeof(bool), typeof(TestDynamicPage), defaultValue:true);

    /// <summary>
    ///     Gets or sets if the page footer links should be shown.
    /// </summary>
    public bool ShowFooterLinks
    {
        get => (bool)GetValue(ShowFooterLinksProperty);
        set => SetValue(ShowFooterLinksProperty, value);
    }

    /// <summary>
    ///     Gets the NUnit test runner.
    /// </summary>
    public INUnitTestRunner TestRunner { get; }

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="TestDynamicPage"/> loading and running the tests from the given <paramref name="testRunner"/>.
    /// </summary>
    /// <param name="testRunner">The <see cref="INUnitTestRunner"/> with the loaded tests to run.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="testRunner"/> is null.</exception>
    public TestDynamicPage(INUnitTestRunner testRunner)
    {
        if (testRunner == null)
        {
            throw ExceptionHelper.ThrowArgumentNullException(nameof(testRunner));
        }

        InitializeComponent();

        // Explore and set tests
        TestRunner = testRunner;
        Test = TestRunner.ExploreTests(NUnitFilter.Empty);
        Title = Test.DisplayName;
    }

    /// <summary>
    ///     Initializes a new <see cref="TestDynamicPage"/> with the given <see cref="INUnitTest"/> and <see cref="NUnitTestAssemblyRunner"/>.
    /// </summary>
    /// <param name="test">The test to bind to the page.</param>
    /// <param name="runner">The NUnit test runner the test was loaded from.</param>
    /// <param name="showFooterLinks">If to show the page footer links</param>
    private TestDynamicPage(INUnitTest test, INUnitTestRunner runner, bool showFooterLinks)
    {
        InitializeComponent();

        TestRunner = runner;
        Test = test;
        Title = Test.DisplayName;
        ShowFooterLinks = showFooterLinks;
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> item is selected from the list of tests in the <see cref="TestSuiteView"/>.
    /// </summary>
    /// <param name="sender">The <see cref="ListView"/> that contains the item.</param>
    /// <param name="e">The test selected event arguments.</param>
    protected virtual async void TestDynamicPage_OnItemSelected(object sender, NUnitTestEventArgs e)
    {
        // Get test from event arg
        INUnitTest test = e?.Test;
        if (test == null || test.Test == null)
        {
            return;
        }

        // Navigate to child test page
        await Navigation.PushAsync(new TestDynamicPage(test, TestRunner, false));
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> test run is requested from the <see cref="TestSummaryView"/> or <see cref="TestDetailView"/>.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The test run event arguments.</param>
    protected virtual async void TestDynamicPage_OnRunTestsClicked(object sender, NUnitTestEventArgs e)
    {
        // Get test from event arg
        INUnitTest test = e?.Test;
        if (test == null || test.Test == null)
        {
            return;
        }

        // Create filter to run the selected test
        ITestFilter filter = NUnitFilter.Where.Id(test.Test.Id).Build().Filter;

        // Wait for test to complete
        INUnitTestResult result = await TestRunner.Run(filter);
        if (result == null)
        {
            return;
        }

        // Set test results
        e.Test.Result = result;
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTestResult"/> export results is requested from the <see cref="TestSummaryView"/> or <see cref="TestDetailView"/>.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The export results event arguments.</param>
    protected virtual async void TestDynamicView_OnSaveResultsClicked(object sender, NUnitTestResultEventArgs e)
    {
        try
        {
            // Get test result as xml
            await using Stream resultStream = TestRunner.GetTestResultsAsXmlStream(e?.Result, out string fileName);
            if (resultStream == null)
            {
                return;
            }

            // Open save result dialog
            await v_FileSaver.SaveAsync(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName,
                resultStream);
        }
        catch
        {
            // Alert user if save failed
            // Don't alert on the SaveAsync returned result since it doesn't differentiate between a user Cancel action and an exception
            await DisplayAlert(ResourceHelper.GetResourceString("SaveResultsButton"),
                ResourceHelper.GetResourceString("SaveResultsFailedAlert"),
                ResourceHelper.GetResourceString("SaveResultsFailedConfirmAlert"));
        }
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Event callback when the About <see cref="Button"/> is clicked to open the <see cref="AboutPage"/>.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The clicked button event arguments.</param>
    private async void AboutButton_OnClicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new AboutPage());
    }

    #endregion
}