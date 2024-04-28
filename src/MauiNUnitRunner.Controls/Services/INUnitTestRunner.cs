// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using MauiNUnitRunner.Controls.Models;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Services;

/// <summary>
///     Interface for a test runner to run NUnit tests.
/// </summary>
public interface INUnitTestRunner
{
    #region Public Members

    /// <summary>
    ///     Gets the underlying NUnit test runner.
    /// </summary>
    NUnitTestAssemblyRunner TestRunner { get; }

    /// <summary>
    ///     Gets if a test run is currently running.
    /// </summary>
    bool IsTestRunning { get; }

    #endregion

    #region Public Methods

    /// <summary>
    ///     Add a test assembly to the runner.
    /// </summary>
    /// <param name="assembly">The assembly to load the tests from.</param>
    /// <param name="settings">The dictionary of test settings to pass to the test runner for the loaded assembly.</param>
    void AddTestAssembly(Assembly assembly, IDictionary<string, object> settings = null);

    /// <inheritdoc cref="NUnitTestAssemblyRunner.ExploreTests"/>
    INUnitTest ExploreTests(ITestFilter filter = null);

    /// <inheritdoc cref="NUnitTestAssemblyRunner.Run"/>
    /// <remarks>Returns null if a test is already running.</remarks>
    Task<INUnitTestResult> Run(ITestFilter filter = null);

    /// <inheritdoc cref="NUnitTestAssemblyRunner.StopRun"/>
    void StopRun(bool force);

    /// <inheritdoc cref="NUnitTestAssemblyRunner.WaitForCompletion"/>
    bool WaitForCompletion(int timeout);

    /// <summary>
    ///     Gets the given <see cref="INUnitTestResult"/> as a formatted xml document <see cref="Stream"/>.
    /// </summary>
    /// <param name="result">The <see cref="INUnitTestResult"/> to get as a xml stream.</param>
    /// <param name="fileName">Outputs an appropriate name for the test result file.</param>
    /// <returns>The results as a xml stream, or null if results were null.</returns>
    Stream GetTestResultsAsXmlStream(INUnitTestResult result, out string fileName);

    /// <summary>
    ///     Adds a <see cref="ITestListener"/>.
    /// </summary>
    /// <param name="listener">The test listener to add.</param>
    void AddTestListener(ITestListener listener);

    /// <summary>
    ///     Removes a <see cref="ITestListener"/>.
    /// </summary>
    /// <param name="listener">The test listener to remove.</param>
    void RemoveTestListener(ITestListener listener);

    #endregion
}