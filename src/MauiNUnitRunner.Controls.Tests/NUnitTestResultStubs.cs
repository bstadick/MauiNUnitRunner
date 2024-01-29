// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework.Interfaces;

// ReSharper disable NotNullOrRequiredMemberIsNotInitialized

namespace MauiNUnitRunner.Controls.Tests;

/// <summary>
///     Stub that implements a <see cref="ITestResult"/>.
/// </summary>
public class TestResultStub : ITestResult
{
    #region Implementation of ITestResult

    /// <inheritdoc />
    public virtual TNode ToXml(bool recursive)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual TNode AddToXml(TNode parentNode, bool recursive)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public ResultState ResultState { get; set; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string FullName { get; set; }

    /// <inheritdoc />
    public double Duration => (EndTime - StartTime).TotalSeconds;

    /// <inheritdoc />
    public DateTime StartTime { get; set; }

    /// <inheritdoc />
    public DateTime EndTime { get; set; }

    /// <inheritdoc />
    public string Message { get; set; }

    /// <inheritdoc />
    public string StackTrace { get; set; }

    /// <inheritdoc />
    public int TotalCount => FailCount + WarningCount + PassCount + SkipCount + InconclusiveCount;

    /// <inheritdoc />
    public int AssertCount { get; set; }

    /// <inheritdoc />
    public int FailCount { get; set; }

    /// <inheritdoc />
    public int WarningCount { get; set; }

    /// <inheritdoc />
    public int PassCount { get; set; }

    /// <inheritdoc />
    public int SkipCount { get; set; }

    /// <inheritdoc />
    public int InconclusiveCount { get; set; }

    /// <inheritdoc />
    // ReSharper disable once ConstantConditionalAccessQualifier
    public bool HasChildren => Children?.Count() > 0;

    /// <inheritdoc />
    public IEnumerable<ITestResult> Children { get; set; }

    /// <inheritdoc />
    public ITest Test { get; set; }

    /// <inheritdoc />
    public string Output { get; set; }

    /// <inheritdoc />
    public IList<AssertionResult> AssertionResults { get; set; }

    /// <inheritdoc />
    public ICollection<TestAttachment> TestAttachments { get; set; }

    #endregion
}

/// <summary>
///     Stub that implements a <see cref="ITest"/>.
/// </summary>
public class TestStub : ITest
{
    #region Implementation of ITest

    /// <inheritdoc />
    public virtual TNode ToXml(bool recursive)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public virtual TNode AddToXml(TNode parentNode, bool recursive)
    {
        throw new NotImplementedException();
    }

    /// <inheritdoc />
    public string Id { get; set; }

    /// <inheritdoc />
    public string Name { get; set; }

    /// <inheritdoc />
    public string TestType { get; set; }

    /// <inheritdoc />
    public string FullName { get; set; }

    /// <inheritdoc />
    public string ClassName { get; set; }

    /// <inheritdoc />
    public string MethodName { get; set; }

    /// <inheritdoc />
    public ITypeInfo TypeInfo { get; set; }

    /// <inheritdoc />
    public IMethodInfo Method { get; set; }

    /// <inheritdoc />
    public RunState RunState { get; set; }

    /// <inheritdoc />
    // ReSharper disable once ConstantConditionalAccessQualifier
    // ReSharper disable once ConstantNullCoalescingCondition
    public int TestCaseCount => Tests?.Count ?? 0;

    /// <inheritdoc />
    public IPropertyBag Properties { get; set; }

    /// <inheritdoc />
    public ITest Parent { get; set; }

    /// <inheritdoc />
    public bool IsSuite { get; set; }

    /// <inheritdoc />
    // ReSharper disable once ConstantConditionalAccessQualifier
    public bool HasChildren => Tests?.Count > 0;

    /// <inheritdoc />
    public IList<ITest> Tests { get; set; }

    /// <inheritdoc />
    public object Fixture { get; set; }

    /// <inheritdoc />
    public object[] Arguments { get; set; }

    #endregion
}
