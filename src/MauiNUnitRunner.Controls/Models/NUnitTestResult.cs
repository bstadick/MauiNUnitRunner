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
    public string Id => Result?.Test.Id ?? string.Empty;

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
    public bool HasMessage => !string.IsNullOrEmpty(Result?.Message) && !HasFailedAssertions;

    /// <inheritdoc />
    public bool HasStackTrace => !string.IsNullOrEmpty(Result?.StackTrace) && !HasFailedAssertions;

    /// <inheritdoc />
    // ReSharper disable once ConditionIsAlwaysTrueOrFalse
    // ReSharper disable once ConstantConditionalAccessQualifier
    public bool HasFailedAssertions => Result?.AssertionResults?.Any(x => x != null && x.Status != AssertionStatus.Passed) ?? false;

    /// <inheritdoc />
    /// <remarks>
    ///     A failed assertion string is in the format of: "Status + \n + optional Message + \n + optional StackTrace".
    /// </remarks>
    public string FailedAssertionsString => string.Join(Environment.NewLine,
        // ReSharper disable once ConditionIsAlwaysTrueOrFalse
        // ReSharper disable once ConstantConditionalAccessQualifier
        Result?.AssertionResults?.Where(x => x != null && x.Status != AssertionStatus.Passed).Select(x =>
            $"{ResourceHelper.GetResourceString("TestsPageAssertionStatus") ?? string.Empty}{x.Status}" + (string.IsNullOrEmpty(x.Message)
                ? string.Empty
                : $"{Environment.NewLine}{x.Message}") +
            (string.IsNullOrEmpty(x.StackTrace)
                ? string.Empty
                : $"{Environment.NewLine}{ResourceHelper.GetResourceString("TestsPageTestStackTrace") ?? string.Empty}{Environment.NewLine}{x.StackTrace}")
        ) ?? new List<string>());

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
}