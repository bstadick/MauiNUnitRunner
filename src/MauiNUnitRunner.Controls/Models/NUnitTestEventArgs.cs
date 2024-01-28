// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     EventArgs for actions that involve <see cref="INUnitTest"/>.
/// </summary>
public class NUnitTestEventArgs : EventArgs
{
    /// <summary>
    ///     Gets the <see cref="INUnitTest"/> associated with the event.
    /// </summary>
    public INUnitTest Test { get; }

    /// <summary>
    ///     Instantiates a new <see cref="NUnitTestEventArgs"/> instance.
    /// </summary>
    /// <param name="test">The <see cref="INUnitTest"/> associated with the event.</param>
    public NUnitTestEventArgs(INUnitTest test)
    {
        Test = test;
    }
}

/// <summary>
///     EventArgs for actions that involve <see cref="INUnitTestResult"/>.
/// </summary>
public class NUnitTestResultEventArgs : EventArgs
{
    /// <summary>
    ///     Gets the <see cref="INUnitTestResult"/> associated with the event.
    /// </summary>
    public INUnitTestResult Result { get; }

    /// <summary>
    ///     Instantiates a new <see cref="NUnitTestResultEventArgs"/> instance.
    /// </summary>
    /// <param name="result">The <see cref="INUnitTestResult"/> associated with the event.</param>
    public NUnitTestResultEventArgs(INUnitTestResult result)
    {
        Result = result;
    }
}