// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;

// Ignore Spelling: Bindable

namespace MauiNUnitRunner.Controls.Views;

/// <summary>
///     Implements a <see cref="ContentView"/> that contains a suite of tests.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TestSuiteView : ContentView
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
        get => (INUnitTest)GetBindableValue(TestProperty, TestProperty.DefaultValue);
        set => SetBindableValue(TestProperty, value);
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
    public TestSuiteView() : this(true)
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="TestSuiteView"/> with the option to skip initializing the components.
    /// </summary>
    /// <param name="initializeComponent">true if to initialize the component, otherwise false to skip initialize component.</param>
    protected TestSuiteView(bool initializeComponent = true)
    {
        if (initializeComponent)
        {
            InitializeComponent();
        }
    }

    #endregion

    #region Protected Methods

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> item is selected from the test suite list.
    /// </summary>
    /// <param name="sender">The <see cref="ListView"/> that contains the item.</param>
    /// <param name="e">The test selected event arguments.</param>
    protected void TestSuiteView_OnTestItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        if (e.SelectedItem is INUnitTest test)
        {
            TestItemSelected?.Invoke(sender, new NUnitTestEventArgs(test));
        }

        // Reset selected item to allow for re-entry
        SetSelectedItem(sender, null);
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTest"/> test run is requested.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The test run event arguments.</param>
    protected void TestSuiteView_OnRunTestsClicked(object sender, EventArgs e)
    {
        RunTestsClicked?.Invoke(sender, new NUnitTestEventArgs(Test));
    }

    /// <summary>
    ///     Event callback when a <see cref="INUnitTestResult"/> export results is requested.
    /// </summary>
    /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
    /// <param name="e">The export results event arguments.</param>
    protected void TestSuiteView_OnSaveResultsClicked(object sender, EventArgs e)
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

    /// <summary>
    ///     Sets the selected item of the <see cref="ListView"/>.
    /// </summary>
    /// <param name="sender">The <see cref="ListView"/> to set the selected item of.</param>
    /// <param name="value">The value to set the selected item.</param>
    protected virtual void SetSelectedItem(object sender, object value)
    {
        if (sender is ListView view)
        {
            view.SelectedItem = value;
        }
    }

    #endregion
}