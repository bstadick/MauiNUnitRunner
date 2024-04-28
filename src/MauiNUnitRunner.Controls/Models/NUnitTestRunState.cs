// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Resources;
using MauiNUnitRunner.Controls.Services;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Implements a <see cref="INUnitTestRunState"/> to hold the NUnit test run state.
/// </summary>
public class NUnitTestRunState : INUnitTestRunState
{
    #region Private Members

    /// <summary>
    ///     Holds the underlying IsTestRunning value.
    /// </summary>
    private bool v_IsTestRunning;

    /// <summary>
    ///     Holds the underlying TestRunCount value.
    /// </summary>
    private int v_TestRunCount;

    /// <summary>
    ///     Holds the underlying TestRunStartedCount value.
    /// </summary>
    private int v_TestRunStartedCount;

    /// <summary>
    ///     Holds the underlying TestRunFinishedCount value.
    /// </summary>
    private int v_TestRunFinishedCount;

    /// <summary>
    ///     Holds the list of additional properties to update when a progress value has changed.
    /// </summary>
    private static readonly IReadOnlyList<string> v_DependentProgressProperties =
        new List<string> { nameof(TestRunProgress) };

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="NUnitTestRunState"/> to hold the current test run state.
    /// </summary>
    /// <param name="testRunner">The <see cref="INUnitTestRunner"/> associated with the test run state.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="testRunner"/> is null.</exception>
    public NUnitTestRunState(INUnitTestRunner testRunner)
    {
        TestRunner = testRunner ?? throw ExceptionHelper.ThrowArgumentNullException(nameof(testRunner));
    }

    #endregion

    #region Implementation of INUnitTestRunState

    /// <inheritdoc />
    public INUnitTestRunner TestRunner { get; }

    /// <inheritdoc />
    public bool IsTestRunning
    {
        get => v_IsTestRunning;
        set => SetField(ref v_IsTestRunning, value);
    }

    /// <inheritdoc />
    public int TestRunCount
    {
        get => v_TestRunCount;
        set => SetField(ref v_TestRunCount, value, v_DependentProgressProperties);
    }

    /// <inheritdoc />
    public int TestRunStartedCount
    {
        get => v_TestRunStartedCount;
        set => SetField(ref v_TestRunStartedCount, value);
    }

    /// <inheritdoc />
    public int TestRunFinishedCount
    {
        get => v_TestRunFinishedCount;
        set => SetField(ref v_TestRunFinishedCount, value, v_DependentProgressProperties);
    }

    /// <inheritdoc />
    public double TestRunProgress => TestRunCount == 0 ? 0 : (double)TestRunFinishedCount / TestRunCount;

    /// <inheritdoc />
    public void Reset()
    {
        IsTestRunning = false;
        TestRunCount = 0;
        TestRunStartedCount = 0;
        TestRunFinishedCount = 0;
    }

    #endregion

    #region Implementation of INotifyPropertyChanged

    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///     Raises the event handler indicating that a property has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed, or default to use the name of the calling property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    /// <summary>
    ///     Sets the property field and invokes the <see cref="PropertyChanged"/> event if the value has changed.
    /// </summary>
    /// <typeparam name="T">The type of the property being set.</typeparam>
    /// <param name="field">The property field to set.</param>
    /// <param name="value">The value to set the property field to.</param>
    /// <param name="dependentProperties">A list of any dependent properties that should also have the <see cref="PropertyChanged"/> event invoked on.</param>
    /// <param name="propertyName">The name of the property being changed. Default is the current calling property.</param>
    /// <returns>true if the value has changed, otherwise false.</returns>
    // ReSharper disable once UnusedMethodReturnValue.Local
    private bool SetField<T>(ref T field, T value, IEnumerable<string> dependentProperties = null,
        [CallerMemberName] string propertyName = null)
    {
        if (EqualityComparer<T>.Default.Equals(field, value))
        {
            return false;
        }

        field = value;

        OnPropertyChanged(propertyName);
        if (dependentProperties != null)
        {
            foreach (string dependentProperty in dependentProperties)
            {
                OnPropertyChanged(dependentProperty);
            }
        }

        return true;
    }

    #endregion
}