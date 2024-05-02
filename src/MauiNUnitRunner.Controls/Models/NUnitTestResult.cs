// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework.Interfaces;
using System.Collections.ObjectModel;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Implementation of <see cref="INUnitTestResult" />.
/// </summary>
public class NUnitTestResult : INUnitTestResult
{
    #region Private Members

    /// <summary>
    ///     Holds a cached version of the failed assertion string.
    /// </summary>
    private string v_FailedAssertionString = string.Empty;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="NUnitTestResult" /> with the given <see cref="ITestResult" />.
    /// </summary>
    /// <param name="result">The test result to initialize with.</param>
    public NUnitTestResult(ITestResult result)
    {
        Result = result;
        // ReSharper disable once ConstantConditionalAccessQualifier
        List<INUnitTestResult> children = Result?.Children?.Select(x => (INUnitTestResult)(new NUnitTestResult(x))).ToList();
        Children = children != null ? new ObservableCollection<INUnitTestResult>(children) : new ObservableCollection<INUnitTestResult>();
    }

    #endregion

    #region Implementation of INUnitTestResult

    /// <inheritdoc />
    // ReSharper disable once ConstantConditionalAccessQualifier
    public string Id => Result?.Test?.Id ?? string.Empty;

    /// <inheritdoc />
    public ITestResult Result { get; }

    /// <inheritdoc />
    public IList<INUnitTestResult> Children { get; }

    /// <inheritdoc />
    public bool HasChildren => Children != null && Children.Any();

    /// <inheritdoc />
    public bool HasNoChildren => !HasChildren;

    /// <inheritdoc />
    public string ResultStateStatus =>
        Result?.ResultState == null
            ? ResourceHelper.GetResourceString("TestResultNotExecuted") ?? string.Empty
            : Result.ResultState.Status.ToString();

    /// <inheritdoc />
    public Color TextColor
    {
        get
        {
            if (Result?.ResultState == null)
            {
                return ResourceHelper.GetCurrentThemeForegroundColor();
            }

            Color textColor = ResourceHelper.GetCurrentThemeForegroundColor();
            switch (Result.ResultState.Status)
            {
                case TestStatus.Inconclusive:
                    textColor = ResourceHelper.GetResourceTextColor("LabelPurpleStyle");
                    break;
                case TestStatus.Skipped:
                    textColor = ResourceHelper.GetResourceTextColor("LabelBlueStyle");
                    break;
                case TestStatus.Passed:
                    textColor = ResourceHelper.GetResourceTextColor("LabelGreenStyle");
                    break;
                case TestStatus.Warning:
                    textColor = ResourceHelper.GetResourceTextColor("LabelOrangeStyle");
                    break;
                case TestStatus.Failed:
                    textColor = ResourceHelper.GetResourceTextColor("LabelRedStyle");
                    break;
            }

            return textColor ?? ResourceHelper.GetCurrentThemeForegroundColor();
        }
    }

    /// <inheritdoc />
    public string DurationString
    {
        get
        {
            // Display duration in seconds
            string unit = ResourceHelper.GetResourceString("TestsPageUnitSecond") ?? string.Empty;
            if (Result == null)
            {
                return $"0 {unit}";
            }

            // Duration is less than a second, so display in milliseconds
            double duration = Result.Duration;
            if (Result.Duration < 1)
            {
                duration *= 1000;
                unit = ResourceHelper.GetResourceString("TestsPageUnitMillisecond") ?? string.Empty;
            }

            return $"{duration:F3} {unit}";
        }
    }

    /// <inheritdoc />
    public bool HasTestResult => Result != null;

    /// <inheritdoc />
    public bool HasFailed => Result?.FailCount > 0;

    /// <inheritdoc />
    public bool HasInconclusive => Result?.InconclusiveCount > 0;

    /// <inheritdoc />
    public bool HasWarning => Result?.WarningCount > 0;

    /// <inheritdoc />
    public bool HasSkip => Result?.SkipCount > 0;

    /// <inheritdoc />
    public bool HasOutput => !string.IsNullOrEmpty(Result?.Output);

    /// <inheritdoc />
    public bool HasMessage => !string.IsNullOrEmpty(Result?.Message) && !FailedAssertionsString.Contains(Result.Message);

    /// <inheritdoc />
    public bool HasStackTrace => !string.IsNullOrEmpty(Result?.StackTrace) && !FailedAssertionsString.Contains(Result.StackTrace);

    /// <inheritdoc />
    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
    // ReSharper disable once ConstantConditionalAccessQualifier
    public bool HasFailedAssertions => Result?.AssertionResults?.Any(x => x != null && x.Status != AssertionStatus.Passed) ?? false;

    /// <inheritdoc />
    /// <remarks>
    ///     A failed assertion string is in the format of: "Status + \n + optional Message + \n + optional StackTrace".
    /// </remarks>
    public string FailedAssertionsString
    {
        get
        {
            // Set cached value if failed assertions present and value not already set
            if (Result != null && string.IsNullOrEmpty(v_FailedAssertionString) && HasFailedAssertions)
            {
                // Create string from failed assertions
                v_FailedAssertionString = string.Join(Environment.NewLine,
                    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
                    // ReSharper disable once ConstantConditionalAccessQualifier
                    // ReSharper disable once ConstantNullCoalescingCondition
                    Result.AssertionResults?.Where(x => x != null && x.Status != AssertionStatus.Passed).Select(x =>
                        FormatAssertionResult(x.Status.ToString(), x.Message, x.StackTrace)) ?? new List<string>());

                // ReSharper disable once ConstantConditionalAccessQualifier
                // ReSharper disable once ConstantNullCoalescingCondition
                if (!(Result.Test?.IsSuite ?? true))
                {
                    // Fallback to creating string from xml results in cases where a test case's xml result is more detailed than the assertion list
                    TNode resultNode = Result.ToXml(true);
                    TNode messageNode = resultNode.SelectSingleNode("failure/message");
                    TNode stackTraceNode = resultNode.SelectSingleNode("failure/stack-trace");
                    string errorString = AssertionStatus.Error.ToString();
                    string resultState =
                        Result.ResultState.Status == TestStatus.Failed && Result.ResultState.Label == errorString
                            ? errorString
                            : ResultStateStatus;
                    string fromXml = FormatAssertionResult(resultState, messageNode?.Value, stackTraceNode?.Value);
                    if (fromXml.Length > v_FailedAssertionString.Length)
                    {
                        v_FailedAssertionString = fromXml;
                    }
                }
            }

            // Return cached value
            return v_FailedAssertionString;
        }
    }

    #endregion

    #region Overridden Methods

    /// <inheritdoc />
    public override bool Equals(object obj)
    {
        if (ReferenceEquals(null, obj)) return false;
        if (ReferenceEquals(this, obj)) return true;
        if (obj is ITestResult result) return Equals(result);
        if (obj.GetType() != this.GetType()) return false;
        return Equals((obj as NUnitTestResult)?.Result);
    }

    /// <summary>
    ///     Compares the <see cref="NUnitTestResult" /> with the underlying <see cref="ITestResult"/>.
    /// </summary>
    /// <param name="other">The <see cref="ITestResult"/> to compare against.</param>
    /// <returns>true if the instance is equivalent to the provided <see cref="ITestResult"/>, otherwise false.</returns>
    protected bool Equals(ITestResult other)
    {
        if (other == null) return false;
        return Equals(Result, other);
    }

    /// <inheritdoc />
    public override int GetHashCode()
    {
        return Result?.GetHashCode() ?? 0;
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Formats an <see cref="AssertionResult"/> to a string.
    /// </summary>
    /// <param name="status">The <see cref="AssertionResult"/> status.</param>
    /// <param name="message">The <see cref="AssertionResult"/> message.</param>
    /// <param name="stackTrace">The <see cref="AssertionResult"/> stackTrace.</param>
    /// <returns>The formatted assertion result string.</returns>
    private static string FormatAssertionResult(string status, string message, string stackTrace)
    {
        string statusHeader = ResourceHelper.GetResourceString("TestsPageAssertionStatus") ?? string.Empty;
        string msg = $"{statusHeader} {status}";

        if (!string.IsNullOrEmpty(message))
        {
            msg += $"{Environment.NewLine}{message}";
        }

        if (!string.IsNullOrEmpty(stackTrace))
        {
            string stackTraceHeader = ResourceHelper.GetResourceString("TestsPageTestStackTrace") ?? string.Empty;
            msg += $"{Environment.NewLine}{stackTraceHeader}{Environment.NewLine}{stackTrace}";
        }

        return msg;
    }

    #endregion
}