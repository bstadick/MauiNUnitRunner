// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.ComponentModel;
using MauiNUnitRunner.Controls.Services;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Interface to hold the state of a test run.
/// </summary>
public interface INUnitTestRunState : INotifyPropertyChanged
{
    /// <summary>
    ///     Gets the NUnit test runner.
    /// </summary>
    INUnitTestRunner TestRunner { get; }

    /// <summary>
    ///     Gets or sets if a test is currently running.
    /// </summary>
    bool IsTestRunning { get; set; }

    /// <summary>
    ///     Gets or sets the number of tests in the current test run.
    /// </summary>
    int TestRunCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of tests that have been started in the test run.
    /// </summary>
    int TestRunStartedCount { get; set; }

    /// <summary>
    ///     Gets or sets the number of tests that have been finished in the test run.
    /// </summary>
    int TestRunFinishedCount { get; set; }

    /// <summary>
    ///     Gets the progress of the test run as a percentage between 0 and 1.
    /// </summary>
    double TestRunProgress { get; }

    /// <summary>
    ///     Resets the state.
    /// </summary>
    void Reset();
}