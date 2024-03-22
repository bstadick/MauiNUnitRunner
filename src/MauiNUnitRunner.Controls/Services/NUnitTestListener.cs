// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Collections.Concurrent;
using System.Globalization;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Services;

/// <summary>
///     Listens to and reports results of running tests.
/// </summary>
public class NUnitTestListener : ITestListener, IDisposable
{
    #region Private Members

    /// <summary>
    ///     If the listener should be listening for test messages.
    /// </summary>
    private bool v_Listen;

    /// <summary>
    ///     The thread the log listener writes on.
    /// </summary>
    private readonly Thread v_LoggingThread;

    /// <summary>
    ///     The queue of log messages.
    /// </summary>
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    private readonly ConcurrentQueue<string> v_LogQueue = new ConcurrentQueue<string>();

    #endregion

    #region Public Members

    /// <summary>
    ///     The dictionary of test ID's and their artifacts produced by the listener.
    /// </summary>
    public IDictionary<string, NUnitTestArtifact> Tests { get; } = new Dictionary<string, NUnitTestArtifact>();

    /// <summary>
    ///     Delegate to write messages to an output stream.
    /// </summary>
    /// <param name="message">The message to write.</param>
    public delegate void WriteOutputDelegate(string message);

    /// <summary>
    ///     Event called to write messages.
    /// </summary>
    public event WriteOutputDelegate WriteOutput;

    #endregion

    /// <summary>
    ///     Instantiates a new <see cref="NUnitTestListener"/>.
    /// </summary>
    public NUnitTestListener()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        v_LoggingThread = new Thread(LogListener);
        v_LoggingThread.Name = "NUnitTestListenerLogListener";
        v_Listen = true;
        v_LoggingThread.Start();
    }

    /// <summary>
    ///     Disposes of the <see cref="NUnitTestListener"/>.
    /// </summary>
    public void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    #region Protected Methods

    /// <summary>
    ///     Method ran on the log listener thread to write to the log output.
    /// </summary>
    protected void LogListener()
    {
        while (v_Listen)
        {
            // Skip if nothing to write or nothing to write to
            if (v_LogQueue.Count <= 0 || WriteOutput == null || !v_LogQueue.TryDequeue(out string message) || string.IsNullOrEmpty(message))
            {
                continue;
            }

            try
            {
                WriteOutput.Invoke(message);
            }
            catch
            {
                // Ignore
            }
        }
    }

    /// <summary>
    ///     Write message to <see cref="WriteOutput" /> event handler.
    /// </summary>
    /// <param name="msg">A composite format string.</param>
    /// <param name="args">An object array that contains zero or more objects to format.</param>
    protected void WriteMessage(string msg, params object[] args)
    {
        try
        {
            // Only queue messages for logging if output is attached
            if (WriteOutput != null)
            {
                v_LogQueue.Enqueue(string.Format(CultureInfo.InvariantCulture, msg, args));
            }
        }
        catch
        {
            // Ignore
        }
    }

    /// <summary>
    ///     Disposes of the <see cref="NUnitTestListener"/>.
    /// </summary>
    /// <param name="disposing">If the instances is being disposed.</param>
    protected virtual void Dispose(bool disposing)
    {
        if (v_Listen)
        {
            v_Listen = false;
            v_LoggingThread.Join();
        }
    }

    #endregion

    #region Implementation of ITestListener

    /// <inheritdoc />
    public virtual void TestStarted(ITest test)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable HeuristicUnreachableCode
        if (test == null)
        {
            return;
        }
        // ReSharper enable HeuristicUnreachableCode

        if (!string.IsNullOrEmpty(test.Id) && !Tests.ContainsKey(test.Id))
        {
            Tests.Add(test.Id, new NUnitTestArtifact(test));
        }

        WriteMessage("{0} {1}", ResourceHelper.GetResourceString("TestListenerStarted"), test.FullName);
    }

    /// <inheritdoc />
    public virtual void TestFinished(ITestResult result)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable HeuristicUnreachableCode
        if (result == null)
        {
            return;
        }
        // ReSharper enable HeuristicUnreachableCode


        ITest test = result.Test;
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable HeuristicUnreachableCode
        if (test == null)
        {
            return;
        }
        // ReSharper enable HeuristicUnreachableCode


        if (!string.IsNullOrEmpty(test.Id) && Tests.TryGetValue(test.Id, out NUnitTestArtifact testArtifact) && testArtifact != null)
        {
            testArtifact.Test.Result = new NUnitTestResult(result);
        }

        WriteMessage("{0} {1}: {2}", ResourceHelper.GetResourceString("TestListenerFinished"), test.FullName,
            result.ResultState);
    }

    /// <inheritdoc />
    public virtual void TestOutput(TestOutput output)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (output == null)
        {
            return;
        }

        string fullName = string.Empty;
        if (!string.IsNullOrEmpty(output.TestId) && Tests.TryGetValue(output.TestId, out NUnitTestArtifact test) && test != null)
        {
            test.Outputs.Add(output);
            fullName = test.Test.Test.FullName;
        }

        WriteMessage("{0} {1}:{2}", ResourceHelper.GetResourceString("TestListenerOutput"), fullName, output.Text);
    }

    /// <inheritdoc />
    public virtual void SendMessage(TestMessage message)
    {
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        if (message == null)
        {
            return;
        }

        string fullName = string.Empty;
        if (!string.IsNullOrEmpty(message.TestId) && Tests.TryGetValue(message.TestId, out NUnitTestArtifact testArtifact) &&
            testArtifact != null)
        {
            testArtifact.Messages.Add(message);
            fullName = testArtifact.Test.Test.FullName;
        }

        WriteMessage("{0} {1}[{2}]: {3}", ResourceHelper.GetResourceString("TestListenerMessage"), fullName,
            message.Destination, message.Message);
    }

    #endregion
}