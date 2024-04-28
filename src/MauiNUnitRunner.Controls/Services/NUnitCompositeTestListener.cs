// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Services;

/// <summary>
///     Listens to and reports results of running tests to multiple child test listeners.
/// </summary>
public class NUnitCompositeTestListener : ITestListener
{
    #region Private Members

    /// <summary>
    ///     Holds the list of test listeners to report to.
    /// </summary>
    private readonly HashSet<ITestListener> v_TestListeners = new HashSet<ITestListener>();

    #endregion

    #region Public Members

    /// <summary>
    ///     Gets the test listeners to report to.
    /// </summary>
    public IReadOnlySet<ITestListener> TestListeners => v_TestListeners;

    #endregion

    #region Implementation of ITestListener

    /// <inheritdoc />
    public void TestStarted(ITest test)
    {
        foreach (ITestListener listener in v_TestListeners)
        {
            try
            {
                listener.TestStarted(test);
            }
            catch
            {
                // Ignore
            }
        }
    }

    /// <inheritdoc />
    public void TestFinished(ITestResult result)
    {
        foreach (ITestListener listener in v_TestListeners)
        {
            try
            {
                listener.TestFinished(result);
            }
            catch
            {
                // Ignore
            }
        }
    }

    /// <inheritdoc />
    public void TestOutput(TestOutput output)
    {
        foreach (ITestListener listener in v_TestListeners)
        {
            try
            {
                listener.TestOutput(output);
            }
            catch
            {
                // Ignore
            }
        }
    }

    /// <inheritdoc />
    public void SendMessage(TestMessage message)
    {
        foreach (ITestListener listener in v_TestListeners)
        {
            try
            {
                listener.SendMessage(message);
            }
            catch
            {
                // Ignore
            }
        }
    }

    #endregion

    #region Public Methods

    /// <summary>
    ///     Adds a <see cref="ITestListener"/>.
    /// </summary>
    /// <param name="listener">The test listener to add.</param>
    public void AddTestListener(ITestListener listener)
    {
        if (listener == null)
        {
            return;
        }

        v_TestListeners.Add(listener);
    }

    /// <summary>
    ///     Removes a <see cref="ITestListener"/>.
    /// </summary>
    /// <param name="listener">The test listener to remove.</param>
    public void RemoveTestListener(ITestListener listener)
    {
        if (listener == null)
        {
            return;
        }

        v_TestListeners.Remove(listener);
    }

    #endregion
}