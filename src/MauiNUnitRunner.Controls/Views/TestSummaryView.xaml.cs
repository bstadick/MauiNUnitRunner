// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;

// Ignore Spelling: Bindable

namespace MauiNUnitRunner.Controls.Views;

/// <summary>
///     Implements a <see cref="ContentView"/> that contains a summary of a test.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TestSummaryView : ContentView
{
    #region Public Members

    /// <summary>
    ///     The bindable <see cref="Test"/> property.
    /// </summary>
    public static readonly BindableProperty TestProperty =
        BindableProperty.Create(nameof(Test), typeof(INUnitTest), typeof(TestSummaryView));

    /// <summary>
    ///     Gets or sets the <see cref="INUnitTest"/> to bind to the view.
    /// </summary>
    public INUnitTest Test
    {
        get => (INUnitTest)GetBindableValue(TestProperty, TestProperty.DefaultValue);
        set => SetBindableValue(TestProperty, value);
    }

    /// <summary>
    ///     The bindable <see cref="TestRunState"/> property.
    /// </summary>
    public static readonly BindableProperty TestRunStateProperty =
        BindableProperty.Create(nameof(TestRunState), typeof(INUnitTestRunState), typeof(TestSummaryView));

    /// <summary>
    ///     Gets or sets the test run state.
    /// </summary>
    public INUnitTestRunState TestRunState
    {
        get => (INUnitTestRunState)GetBindableValue(TestRunStateProperty, TestRunStateProperty.DefaultValue);
        set => SetBindableValue(TestRunStateProperty, value);
    }

    /// <summary>
    ///     The bindable <see cref="ShowTestButtons"/> property.
    /// </summary>
    public static readonly BindableProperty ShowTestButtonsProperty =
        BindableProperty.Create(nameof(ShowTestButtons), typeof(bool), typeof(TestSummaryView), true);

    /// <summary>
    ///     Gets or sets if to show the run tests and save results <see cref="Button"/>.
    /// </summary>
    public bool ShowTestButtons
    {
        get => (bool)GetBindableValue(ShowTestButtonsProperty, ShowTestButtonsProperty.DefaultValue);
        set => SetBindableValue(ShowTestButtonsProperty, value);
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
    public TestSummaryView() : this(true)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="TestSummaryView"/> with the option to skip initializing the components.
    /// </summary>
    /// <param name="initializeComponent">true if to initialize the component, otherwise false to skip initialize component.</param>
    protected TestSummaryView(bool initializeComponent = true)
    {
        if (initializeComponent)
        {
            InitializeComponent();
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> test run is requested.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The test run event arguments.</param>
    protected void RunTestsButton_OnClicked(object sender, EventArgs e)
    {
        RunTestsClicked?.Invoke(sender, new NUnitTestEventArgs(Test));
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTestResult"/> export results is requested.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The export results event arguments.</param>
    protected void SaveResultsButton_OnClicked(object sender, EventArgs e)
    {
        SaveResultsClicked?.Invoke(sender, new NUnitTestResultEventArgs(Test?.Result));
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

    #endregion
}