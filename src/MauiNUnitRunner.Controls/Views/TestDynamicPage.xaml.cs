// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using CommunityToolkit.Maui.Storage;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Resources;
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
    public NUnitTestAssemblyRunner TestRunner { get; }

    /// <summary>
    ///     Gets or sets the test listener.
    /// </summary>
    public ITestListener TestListener { get; set; }

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="TestDynamicPage"/> loading the tests from the given <paramref name="assemblies"/>.
    /// </summary>
    /// <param name="assemblies">The list of assemblies to load the tests from.</param>
    /// <param name="settings">The dictionary of test settings to pass to the test runner.</param>
    /// <exception cref="ArgumentNullException">Thrown if <paramref name="assemblies"/> is null.</exception>
    public TestDynamicPage(ICollection<Assembly> assemblies, IDictionary<string, object> settings = null)
    {
        if (assemblies == null)
        {
            throw ExceptionHelper.ThrowArgumentNullException(nameof(assemblies));
        }

        InitializeComponent();

        TestRunner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

        // Load test assemblies
        settings ??= new Dictionary<string, object>();
        foreach (Assembly assembly in assemblies)
        {
            if (assembly != null)
            {
                TestRunner.Load(assembly, settings);
            }
        }

        // Explore and set tests
        ITest tests = TestRunner.ExploreTests(NUnitFilter.Empty);
        Test = new NUnitTest(tests);
        Title = Test.DisplayName;
    }

    /// <summary>
    ///     Initializes a new <see cref="TestDynamicPage"/> with the given <see cref="INUnitTest"/> and <see cref="NUnitTestAssemblyRunner"/>.
    /// </summary>
    /// <param name="test">The test to bind to the page.</param>
    /// <param name="runner">The NUnit test runner the test was loaded from.</param>
    /// <param name="showFooterLinks">If to show the page footer links</param>
    private TestDynamicPage(INUnitTest test, NUnitTestAssemblyRunner runner, bool showFooterLinks)
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
        // Navigate to child test page
        if (e.Test != null)
        {
            await Navigation.PushAsync(new TestDynamicPage(e.Test, TestRunner, false));
        }
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> test run is requested from the <see cref="TestSummaryView"/> or <see cref="TestDetailView"/>.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The test run event arguments.</param>
    protected virtual async void TestDynamicPage_OnRunTestsClicked(object sender, NUnitTestEventArgs e)
    {
        // Only allow one test run to run at a time
        if (TestRunner.IsTestRunning)
        {
            return;
        }

        INUnitTest test = e.Test;
        if (test == null || test.Test == null)
        {
            return;
        }

        // Run tests
        Task<INUnitTestResult> runTask = Task.Run(() =>
        {
            ITestResult result = TestRunner.Run(TestListener, NUnitFilter.Where.Id(test.Test.Id).Build().Filter);
            return (INUnitTestResult)(new NUnitTestResult(result));
        });

        // Wait for test to complete
        INUnitTestResult result = await runTask;

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
        INUnitTestResult result = e.Result;
        if (result == null || !result.HasTestResult)
        {
            return;
        }

        // Get result as xml
        TNode resultXml = result.Result.ToXml(true);

        try
        {
            // Convert result string to stream, use an XmlTextWrite and XDocument to apply xml formatting
            await using MemoryStream resultStream = new MemoryStream();
            await using XmlTextWriter writer = new XmlTextWriter(resultStream, Encoding.UTF8);
            writer.Formatting = Formatting.Indented;
            XDocument doc = XDocument.Parse(resultXml.OuterXml);
            doc.WriteTo(writer);
            writer.Flush();
            resultStream.Seek(0, SeekOrigin.Begin);

            // Format full test name for initial file, remove any part of a test case string after a '(' and replacing spaces with '-'
            INUnitTest test = new NUnitTest(result.Result.Test);
            string testName = test.FullDisplayName;
            int ind = testName.IndexOf('(', StringComparison.Ordinal);
            if (ind > 0)
            {
                testName = testName.Substring(0, ind);
            }

            testName = testName.Replace(" ", "-");

            // Open save result dialog
            await v_FileSaver.SaveAsync(
                Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), $"{testName}-{test.Id}.xml",
                resultStream);
        }
        catch (Exception)
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