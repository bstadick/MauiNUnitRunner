// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework.Interfaces;
using System.ComponentModel;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Interface to hold the <see cref="ITest"/> and associated <see cref="INUnitTestResult"/>.
/// </summary>
public interface INUnitTest : INotifyPropertyChanged
{
    /// <summary>
    ///     Gets the underlying <see cref="ITest"/> id.
    /// </summary>
    string Id { get; }

    /// <summary>
    ///     Gets the <see cref="ITest"/>.
    /// </summary>
    ITest Test { get; }

    /// <summary>
    ///     Gets or sets the <see cref="INUnitTestResult"/> of the test, or null if test not ran.
    /// </summary>
    INUnitTestResult Result { get; set; }

    /// <summary>
    ///     Gets if there is a test result.
    /// </summary>
    bool HasTestResult { get; }

    /// <summary>
    ///     Gets the children <see cref="INUnitTest"/> to the current test.
    /// </summary>
    IList<INUnitTest> Children { get; }

    /// <summary>
    ///     Gets if the test has child tests.
    /// </summary>
    bool HasChildren { get; }

    /// <summary>
    ///     Inverse of <see cref="HasChildren"/>.
    /// </summary>
    bool HasNoChildren { get; }

    /// <summary>
    ///     Gets the color of the test result string.
    /// </summary>
    Color TextColor { get; }

    /// <summary>
    ///     Gets the display name for the test.
    /// </summary>
    string DisplayName { get; }

    /// <summary>
    ///     Gets the full display name for the test.
    /// </summary>
    string FullDisplayName { get; }

    /// <summary>
    ///     Gets the <see cref="FullDisplayName" /> for the test if it is a suite and not a class nor a method, otherwise gets
    ///     the <see cref="DisplayName" />.
    /// </summary>
    string ConditionalDisplayName { get; }

    /// <summary>
    ///     Gets either the current or next child <see cref="INUnitTest"/> that has more than one child test or is a leaf test.
    /// </summary>
    /// <returns>The current or next child <see cref="INUnitTest"/> that has more than one child test or is a leaf test.</returns>
    INUnitTest SkipSingleTestSuites();
}