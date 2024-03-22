// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;

namespace MauiNUnitRunner.Controls.Views;

/// <summary>
///     Implements a <see cref="ContentView"/> that contains either a test suite in a <see cref="TestSummaryView"/> or a single test details in a <see cref="TestDetailView"/>.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TestDynamicView : ContentView
{
    #region Public Members

    /// <summary>
    ///     The bindable <see cref="Test"/> property.
    /// </summary>
    public static readonly BindableProperty TestProperty =
        BindableProperty.Create(nameof(Test), typeof(INUnitTest), typeof(TestSuiteView));

    /// <summary>
    ///     Gets or sets the <see cref="INUnitTest"/> to bind to the view.
    /// </summary>
    public INUnitTest Test
    {
        get => (INUnitTest)GetValue(TestProperty);
        set => SetValue(TestProperty, value);
    }

    /// <summary>
    ///     Event raised when a <see cref="INUnitTest"/> item is selected.
    /// </summary>
    public event EventHandler<NUnitTestEventArgs> TestItemSelected;

    /// <summary>
    ///     Event raised when a <see cref="INUnitTest"/> test run is requested.
    /// </summary>
    public event EventHandler<NUnitTestEventArgs> RunTestsClicked;

    /// <summary>
    ///     Event raised when a <see cref="INUnitTestResult"/> export is requested.
    /// </summary>

    public event EventHandler<NUnitTestResultEventArgs> SaveResultsClicked;

    #endregion

    #region Constructors

    /// <summary>
    ///     Default constructor.
    /// </summary>
    public TestDynamicView()
    {
        InitializeComponent();
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> item is selected from the list of tests in the <see cref="TestSuiteView"/>.
    /// </summary>
    /// <param name="sender">The <see cref="ListView"/> that contains the item.</param>
    /// <param name="e">The test selected event arguments.</param>
    private void TestDynamicView_OnTestItemSelected(object sender, NUnitTestEventArgs e)
    {
        TestItemSelected?.Invoke(sender, e);
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> test run is requested from the <see cref="TestSummaryView"/> or <see cref="TestDetailView"/>.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The test run event arguments.</param>
    private void TestDynamicView_OnRunTestsClicked(object sender, NUnitTestEventArgs e)
    {
        RunTestsClicked?.Invoke(sender, e);
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTestResult"/> export results is requested.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The export results event arguments.</param>
    private void TestDynamicView_OnSaveResultsClicked(object sender, EventArgs e)
    {
        SaveResultsClicked?.Invoke(sender, new NUnitTestResultEventArgs(Test.Result));
    }

    #endregion
}