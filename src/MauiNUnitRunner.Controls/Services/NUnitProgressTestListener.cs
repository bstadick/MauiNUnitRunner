// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Services;

/// <summary>
///     Listens to and reports the progress of tests for updating the test run state.
/// </summary>
public class NUnitProgressTestListener : ITestListener
{
    #region Public Members

    /// <summary>
    ///     Gets the test run state of the test progress listener.
    /// </summary>
    public INUnitTestRunState TestRunState { get; }

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="NUnitProgressTestListener"/> to track the progress of tests.
    /// </summary>
    /// <param name="testRunState">The test run state to update with the tracked progress.</param>
    /// <exception cref="ArgumentNullException">Thrown if the <paramref name="testRunState"/> is null.</exception>
    public NUnitProgressTestListener(INUnitTestRunState testRunState)
    {
        TestRunState = testRunState ?? throw ExceptionHelper.ThrowArgumentNullException(nameof(testRunState));
    }

    #endregion

    #region Implementation of ITestListener

    /// <inheritdoc />
    public void TestStarted(ITest test)
    {
        // ReSharper disable once ConstantNullCoalescingCondition
        // ReSharper disable once ConstantConditionalAccessQualifier
        if (!test?.HasChildren ?? false)
        {
            TestRunState.TestRunStartedCount++;
        }
    }

    /// <inheritdoc />
    public void TestFinished(ITestResult result)
    {
        // ReSharper disable once ConstantNullCoalescingCondition
        // ReSharper disable once ConstantConditionalAccessQualifier
        if (!result?.HasChildren ?? false)
        {
            TestRunState.TestRunFinishedCount++;
        }
    }

    /// <inheritdoc />
    public void TestOutput(TestOutput output)
    {
        // Do nothing
    }

    /// <inheritdoc />
    public void SendMessage(TestMessage message)
    {
        // Do nothing
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///     Resets the test state.
    /// </summary>
    public void Reset()
    {
        TestRunState.Reset();
    }

    #endregion
}