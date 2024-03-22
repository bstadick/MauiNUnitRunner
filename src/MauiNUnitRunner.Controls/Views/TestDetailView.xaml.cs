// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;

namespace MauiNUnitRunner.Controls.Views;

/// <summary>
///     Implements a <see cref="ContentView"/> that contains a single test's details.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TestDetailView : ContentView
{
    #region Public Members

    /// <summary>
    ///     The bindable <see cref="Test"/> property.
    /// </summary>
    public static readonly BindableProperty TestProperty =
        BindableProperty.Create(nameof(Test), typeof(INUnitTest), typeof(TestDetailView));

    /// <summary>
    ///     Gets or sets the <see cref="INUnitTest"/> to bind to the view.
    /// </summary>
    public INUnitTest Test
    {
        get => (INUnitTest)GetValue(TestProperty);
        set => SetValue(TestProperty, value);
    }

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
    public TestDetailView()
    {
        InitializeComponent();
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> test run is requested.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The test run event arguments.</param>
    private void RunTestsButton_OnClicked(object sender, EventArgs e)
    {
        RunTestsClicked?.Invoke(sender, new NUnitTestEventArgs(Test));
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTestResult"/> export results is requested.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The export results event arguments.</param>
    private void SaveResultsButton_OnClicked(object sender, EventArgs e)
    {
        SaveResultsClicked?.Invoke(sender, new NUnitTestResultEventArgs(Test.Result));
    }

    #endregion
}