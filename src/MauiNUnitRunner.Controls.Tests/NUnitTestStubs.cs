// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using MauiNUnitRunner.Controls.Services;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

// ReSharper disable NotNullOrRequiredMemberIsNotInitialized
// ReSharper disable UnusedAutoPropertyAccessor.Global
// ReSharper disable PropertyCanBeMadeInitOnly.Global
// ReSharper disable ClassWithVirtualMembersNeverInherited.Global

namespace MauiNUnitRunner.Controls.Tests;

/// <summary>
///     Stub that implements a <see cref="ITestResult"/>.
/// </summary>
public class TestResultStub : ITestResult
{
    #region Members for Test

    /// <summary>
    ///     Gets or sets the test result xml value.
    /// </summary>
    public TNode TestResultXml { get; set; }

    #endregion

    #region Implementation of ITestResult

    /// <inheritdoc />
    public virtual TNode ToXml(bool recursive)
    {
        return TestResultXml;
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

    /// <summary>
    ///     Holds if the duration has been set.
    /// </summary>
    private bool v_DurationSet;

    /// <summary>
    ///     Holds the set duration.
    /// </summary>
    private double v_Duration;

    /// <inheritdoc />
    public double Duration
    {
        get => v_DurationSet ? v_Duration : (EndTime - StartTime).TotalSeconds;
        set
        {
            v_DurationSet = true;
            v_Duration = value;
        }
    }

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
    #region Members for Test

    /// <summary>
    ///     Gets or sets the test xml value.
    /// </summary>
    public TNode TestXml { get; set; }

    #endregion

    #region Implementation of ITest

    /// <inheritdoc />
    public virtual TNode ToXml(bool recursive)
    {
        return TestXml;
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

/// <summary>
///     Stub that implements a <see cref="INUnitTestAssemblyRunner"/>.
/// </summary>
public class NUnitTestAssemblyRunnerStub : INUnitTestAssemblyRunner
{
    #region Members for Test

    /// <summary>
    ///     Gets or sets the <see cref="Load"/> action for test.
    /// </summary>
    public Action<Assembly, IDictionary<string, object>> OnLoad { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="ExploreTests"/> function for test.
    /// </summary>
    public Func<ITestFilter, ITest> OnExploreTests { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="Run"/> function for test.
    /// </summary>
    public Func<ITestListener, ITestFilter, ITestResult> OnRun { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="StopRun"/> action for test.
    /// </summary>
    public Action<bool> OnStopRun { get; set; }

    /// <summary>
    ///     Gets or sets the <see cref="WaitForCompletion"/> function for test.
    /// </summary>
    public Func<int, bool> OnWaitForCompletion { get; set; }

    #endregion

    #region Implementation of INUnitTestAssemblyRunner

    /// <inheritdoc />
    public NUnitTestAssemblyRunner TestRunner { get; set; }

    /// <inheritdoc />
    public bool IsTestRunning { get; set; }

    /// <inheritdoc />
    public void Load(Assembly assembly, IDictionary<string, object> settings)
    {
        OnLoad?.Invoke(assembly, settings);
    }

    /// <inheritdoc />
    public ITest ExploreTests(ITestFilter filter)
    {
        return OnExploreTests?.Invoke(filter);
    }

    /// <inheritdoc />
    public ITestResult Run(ITestListener listener, ITestFilter filter)
    {
        IsTestRunning = true;
        ITestResult result = OnRun?.Invoke(listener, filter);
        IsTestRunning = false;
        return result;
    }

    /// <inheritdoc />
    public void StopRun(bool force)
    {
        OnStopRun?.Invoke(force);
        IsTestRunning = false;
    }

    /// <inheritdoc />
    public bool WaitForCompletion(int timeout)
    {
        return OnWaitForCompletion?.Invoke(timeout) ?? false;
    }

    #endregion
}

/// <summary>
///     Implements a stub ListView for use with tests.
/// </summary>
public class ListViewStub
{
    #region Public Members

    /// <summary>
    ///     Gets or sets the selected item.
    /// </summary>
    public object SelectedItem { get; set; }

    #endregion
}

/// <summary>
///     A Test fixture class to run as part of test runner tests. Not meant to be run as part of the project's unit tests.
/// </summary>
[TestFixture, Explicit]
public class TestFixtureForNUnitRunnerTest
{
    #region Members for Test

    /// <summary>
    ///     Gets or sets the delay in the running test.
    /// </summary>
    public static int TestDelay { get; set; }

    #endregion

    #region Tests

    [Test]
    public void Test()
    {
        if (TestDelay > 0)
        {
            Thread.Sleep(TestDelay);
        }

#pragma warning disable NUnit2007 // The actual value should not be a constant
        Assert.That(true, Is.True);
#pragma warning restore NUnit2007 // The actual value should not be a constant
    }

    #endregion
}