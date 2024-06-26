// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using CommunityToolkit.Maui.Storage;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Resources;
using MauiNUnitRunner.Controls.Services;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

// Ignore Spelling: Bindable

namespace MauiNUnitRunner.Controls.Views;

/// <summary>
///     Implements a basic test runner page to display a <see cref="TestDynamicView"/> that contains either a test suite in a <see cref="TestSummaryView"/> or a single test details in a <see cref="TestDetailView"/>.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TestDynamicPage : ContentPage
{
    #region Private Members

    /// <summary>
    ///     The file saver instance.
    /// </summary>
    private readonly IFileSaver v_FileSaver = FileSaver.Default;

    /// <summary>
    ///     The parent page.
    /// </summary>
    private readonly TestDynamicPage v_ParentPage;

    /// <summary>
    ///     The NUnit test runner.
    /// </summary>
    private readonly INUnitTestRunner v_TestRunner;

    /// <summary>
    ///     The test progress listener.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    private readonly NUnitProgressTestListener v_ProgressTestListener;

    #endregion

    #region Public Members

    /// <summary>
    ///     Event called when the test run throws an exception.
    /// </summary>
    public event EventHandler<Exception> TestRunException;

    /// <summary>
    ///     Gets the NUnit test runner.
    /// </summary>
    public INUnitTestRunner TestRunner => v_ParentPage != null ? v_ParentPage.TestRunner : v_TestRunner;

    /// <summary>
    ///     Gets the test progress listener.
    /// </summary>
    public NUnitProgressTestListener ProgressTestListener => v_ParentPage != null ? v_ParentPage.ProgressTestListener : v_ProgressTestListener;

    /// <summary>
    ///     The bindable <see cref="Test"/> property.
    /// </summary>
    public static readonly BindableProperty TestProperty =
        BindableProperty.Create(nameof(Test), typeof(INUnitTest), typeof(TestDynamicPage));

    /// <summary>
    ///     Gets the <see cref="INUnitTest"/> to bind to the page.
    /// </summary>
    public INUnitTest Test
    {
        get => (INUnitTest)GetBindableValue(TestProperty, TestProperty.DefaultValue);
        private set => SetBindableValue(TestProperty, value);
    }

    /// <summary>
    ///     The bindable <see cref="TestRunState"/> property.
    /// </summary>
    public static readonly BindableProperty TestRunStateProperty =
        BindableProperty.Create(nameof(TestRunState), typeof(INUnitTestRunState), typeof(TestDynamicPage));

    /// <summary>
    ///     Gets the test run state.
    /// </summary>
    public INUnitTestRunState TestRunState
    {
        get => (INUnitTestRunState)GetBindableValue(TestRunStateProperty, TestRunStateProperty.DefaultValue);
        private set => SetBindableValue(TestRunStateProperty, value);
    }

    /// <summary>
    ///     The bindable <see cref="ShowFooterLinks"/> property.
    /// </summary>
    public static readonly BindableProperty ShowFooterLinksProperty =
        BindableProperty.Create(nameof(ShowFooterLinks), typeof(bool), typeof(TestDynamicPage), true);

    /// <summary>
    ///     Gets if the page footer links should be shown.
    /// </summary>
    public bool ShowFooterLinks
    {
        get => (bool)GetBindableValue(ShowFooterLinksProperty, ShowFooterLinksProperty.DefaultValue);
        private set => SetBindableValue(ShowFooterLinksProperty, value);
    }

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="TestDynamicPage"/> loading and running the tests from the given <paramref name="testRunner"/>.
    /// </summary>
    /// <param name="testRunner">The <see cref="INUnitTestRunner"/> with the loaded tests to run.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="testRunner"/> is null.</exception>
    public TestDynamicPage(INUnitTestRunner testRunner) : this(testRunner, null)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="TestDynamicPage"/> with the given <see cref="INUnitTest"/> and <see cref="NUnitTestAssemblyRunner"/>.
    /// </summary>
    /// <param name="testRunner">The NUnit test runner the test was loaded from.</param>
    /// <param name="test">The test to bind to the page, or null to explore all currently loaded tests.</param>
    /// <param name="parent">The parent <see cref="TestDynamicPage"/> or null if no parent.</param>
    /// <param name="initializeComponent">true if to initialize the component, otherwise false to skip initialize component.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="testRunner"/> and <paramref name="parent"/> is null.</exception>
    protected TestDynamicPage(INUnitTestRunner testRunner, INUnitTest test, TestDynamicPage parent = null,
        bool initializeComponent = true)
    {
        // Need either parent page or test runner
        if (parent == null && testRunner == null)
        {
            throw ExceptionHelper.ThrowArgumentNullException(nameof(testRunner));
        }

        if (initializeComponent)
        {
            InitializeComponent();
        }

        // Set test runner and state instance only to the root parent page
        // All child pages will inherit them from the root parent page.
        v_ParentPage = parent;
        if (v_ParentPage == null)
        {
            v_TestRunner = testRunner;
            TestRunState = new NUnitTestRunState(v_TestRunner);
            v_ProgressTestListener = new NUnitProgressTestListener(TestRunState);
            v_TestRunner.AddTestListener(v_ProgressTestListener);
        }
        else
        {
            TestRunState = v_ParentPage.TestRunState;
        }

        Test = test ?? TestRunner.ExploreTests(NUnitFilter.Empty);
        Title = Test?.DisplayName ?? string.Empty;
        v_ParentPage = parent;
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
        if (test?.Test == null)
        {
            return;
        }

        // Navigate to child test page
        TestDynamicPage page = CreateTestDynamicPage(test.SkipSingleTestSuites());
        page.ShowFooterLinks = false;
        await NavigationPushAsync(page);
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
        if (test?.Test == null)
        {
            return;
        }

        // Reset test run state
        ProgressTestListener.Reset();
        TestRunState.IsTestRunning = true;

        try
        {
            // Create filter to run the selected test
            ITestFilter filter = NUnitFilter.Where.Id(test.Test.Id).Build().Filter;

            // Count number of tests to be run
            TestRunState.TestRunCount = CountTests(TestRunner.ExploreTests(filter), 0);

            INUnitTestResult result = null;
            try
            {
                // Run and wait for test to complete
                result = await TestRunner.Run(filter);
            }
            catch (Exception ex)
            {
                TestRunException?.Invoke(this, ex);
            }

            if (result == null)
            {
                return;
            }

            // Set test results
            e.Test.Result = result;
        }
        finally
        {
            TestRunState.IsTestRunning = false;
        }
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
            await SaveAsync(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), fileName, resultStream);
        }
        catch
        {
            // Alert user if save failed
            // Don't alert on the SaveAsync returned result since it doesn't differentiate between a user Cancel action and an exception
            await DisplayAlertMessage(ResourceHelper.GetResourceString("SaveResultsButton"),
                ResourceHelper.GetResourceString("SaveResultsFailedAlert"),
                ResourceHelper.GetResourceString("SaveResultsFailedConfirmAlert"));
        }
    }

    /// <summary>
    ///     Event callback when the About <see cref="Button"/> is clicked to open the <see cref="AboutPage"/>.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The clicked button event arguments.</param>
    protected async void AboutButton_OnClicked(object sender, EventArgs e)
    {
        await NavigationPushAsync(new AboutPage());
    }

    /// <summary>
    ///     Calls <see cref="BindableObject.GetValue(BindableProperty)"/>.
    /// </summary>
    /// <param name="property">The <see cref="BindableProperty"/> to get.</param>
    /// <param name="defaultValue">The default value to return if no value is set.</param>
    /// <returns>The value of the property to get.</returns>
    protected virtual object GetBindableValue(BindableProperty property, object defaultValue)
    {
        return GetValue(property);
    }

    /// <summary>
    ///     Calls <see cref="BindableObject.SetValue(BindableProperty, object)"/>.
    /// </summary>
    /// <param name="property">The <see cref="BindableProperty"/> to set.</param>
    /// <param name="value">The value of the property to set.</param>
    protected virtual void SetBindableValue(BindableProperty property, object value)
    {
        SetValue(property, value);
    }

    /// <summary>
    ///     Creates a new <see cref="TestDynamicPage"/> instance.
    /// </summary>
    /// <param name="test">The <see cref="INUnitTest"/> of the test page.</param>
    /// <returns>The newly created <see cref="TestDynamicPage"/>.</returns>
    protected virtual TestDynamicPage CreateTestDynamicPage(INUnitTest test)
    {
        return new TestDynamicPage(null, test, this);
    }

    /// <summary>
    ///     Navigates to the given <see cref="Page"/>.
    /// </summary>
    /// <param name="page">The page to navigate to.</param>
    /// <returns>A <see cref="Task"/> to await.</returns>
    protected virtual async Task NavigationPushAsync(Page page)
    {
        await Navigation.PushAsync(page);
    }

    /// <summary>
    ///     Opens a dialog to save the file.
    /// </summary>
    /// <param name="folderPath">The starting path of the folder to save to.</param>
    /// <param name="fileName">The name of the file to save.</param>
    /// <param name="resultStream">The file stream to save.</param>
    /// <returns>A <see cref="Task"/> to await.</returns>
    protected virtual async Task SaveAsync(string folderPath, string fileName, Stream resultStream)
    {
#pragma warning disable CA1416
        await v_FileSaver.SaveAsync(folderPath, fileName, resultStream);
#pragma warning restore CA1416
    }

    /// <summary>
    ///     Displays an alert message.
    /// </summary>
    /// <param name="title">The title of the message.</param>
    /// <param name="message">The message to display.</param>
    /// <param name="cancel">The cancel/confirmation button text.</param>
    /// <returns>A <see cref="Task"/> to await.</returns>
    protected virtual async Task DisplayAlertMessage(string title, string message, string cancel)
    {
        await DisplayAlert(title, message, cancel);
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Counts the number of test cases for a given test.
    /// </summary>
    /// <param name="test">The test to count the number of test cases of</param>
    /// <param name="previousCount">The previous test count to increment.</param>
    /// <returns>The number of test cases.</returns>
    private int CountTests(INUnitTest test, int previousCount)
    {
        if (test == null)
        {
            return previousCount;
        }

        // Only count tests that are leaf nodes
        if (!test.HasChildren)
        {
            return previousCount + 1;
        }

        // Iterate through tests recursively
        foreach (INUnitTest child in test.Children)
        {
            previousCount = CountTests(child, previousCount);
        }

        return previousCount;

    }

    #endregion
}