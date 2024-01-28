// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Extension of the <see cref="ITestResult" /> interface.
/// </summary>
public interface INUnitTestResult
{
    /// <summary>
    ///     Gets the underlying <see cref="ITest"/> id.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///     Gets or sets the underlying <see cref="ITestResult" />.
    /// </summary>
    ITestResult Result { get; }

    /// <summary>
    ///     Gets the children <see cref="INUnitTestResult"/> to the current test.
    /// </summary>
    IList<INUnitTestResult> Children { get; }

    /// <summary>
    ///     Gets if the result has child results.
    /// </summary>
    bool HasChildren { get; }

    /// <summary>
    ///     Inverse of <see cref="HasChildren"/>.
    /// </summary>
    bool HasNoChildren { get; }

    /// <summary>
    ///     Gets the result state status string of the test.
    /// </summary>
    string ResultStateStatus { get; }

    /// <summary>
    ///     Gets the color of the test result string.
    /// </summary>
    Color TextColor { get; }

    /// <summary>
    ///     Gets the test formatted duration string.
    /// </summary>
    string DurationString { get; }

    /// <summary>
    ///     Gets if there is a test result.
    /// </summary>
    bool HasTestResult { get; }

    /// <summary>
    ///     Gets if there are failed child tests.
    /// </summary>
    bool HasFailed { get; }

    /// <summary>
    ///     Gets if there are inconclusive child tests.
    /// </summary>
    bool HasInconclusive { get; }

    /// <summary>
    ///     Gets if there are warning child tests.
    /// </summary>
    bool HasWarning { get; }

    /// <summary>
    ///     Gets if there are skipped child tests.
    /// </summary>
    bool HasSkip { get; }

    /// <summary>
    ///     Gets if there are output messages of child tests.
    /// </summary>
    /// <remarks>An output message such as from <see cref="Console.WriteLine()" />.</remarks>
    bool HasOutput { get; }

    /// <summary>
    ///     Gets if there are exception messages of child tests.
    /// </summary>
    /// <remarks>An exception message due to an uncaught exception.</remarks>
    bool HasMessage { get; }

    /// <summary>
    ///     Gets if there are stack traces of child tests.
    /// </summary>
    bool HasStackTrace { get; }

    /// <summary>
    ///     Gets if an assertion has failed.
    /// </summary>
    bool HasFailedAssertions { get; }

    /// <summary>
    ///     Gets a string of failed assertions.
    /// </summary>
    string FailedAssertionsString { get; }
}