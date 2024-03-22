// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using MauiNUnitRunner.Controls.Models;

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class NUnitTestResultTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructorWithTestResult([Values] bool isNull)
    {
        ITestResult result = isNull ? null : new TestResultForTest();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.SameAs(result));
    }

    #endregion

    #region Tests for Result Property

    [Test]
    public void TestResultProperty([Values] bool isNull)
    {
        ITestResult result = !isNull ? null : new TestResultForTest();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.SameAs(result));
    }

    #endregion

    #region Tests for ResultStateStatus Property

    [Test]
    public void TestResultStateStatusPropertyWithResultNullReturnsDefaultString([Values] bool hasResult)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.ResultState = null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.EqualTo(result));
        Assert.That(test.ResultStateStatus, Is.EqualTo("Test not executed."));
    }

    [Test]
    public void TestResultStateStatusPropertyWithResultReturnsResultStateStatusString()
    {
        ResultState state = ResultState.Success;
        TestResultForTest result = new TestResultForTest();
        result.ResultState = state;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.ResultStateStatus, Is.EqualTo(state.Status.ToString()));
    }

    #endregion

    #region Tests for TextColor Property

    [Test]
    public void TestTextColorPropertyWithNullResultReturnsColorBlack([Values] bool hasResult)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.ResultState = null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.TextColor, Is.EqualTo(Colors.Black));
    }

    [Test]
    public void TestTextColorPropertyWithNotSupportedTestStatusReturnsColorBlack()
    {
        TestResultForTest result = new TestResultForTest();
        result.ResultState = new ResultState((TestStatus) (-1));

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.TextColor, Is.EqualTo(Colors.Black));
    }

    [Test]
    public void TestTextColorPropertyReturnsColorForResultState([Values] TestStatus status)
    {
        TestResultForTest result = new TestResultForTest();
        result.ResultState = new ResultState((TestStatus) (-1));

        Color expected = Colors.Black;
        ResultState state = ResultState.NotRunnable;
        switch (status)
        {
            case TestStatus.Inconclusive:
                state = ResultState.Inconclusive;
                expected = Colors.Purple;
                break;
            case TestStatus.Skipped:
                state = ResultState.Ignored;
                expected = Colors.Blue;
                break;
            case TestStatus.Passed:
                state = ResultState.Success;
                expected = Colors.Green;
                break;
            case TestStatus.Warning:
                state = ResultState.Warning;
                expected = Colors.Orange;
                break;
            case TestStatus.Failed:
                state = ResultState.Failure;
                expected = Colors.Red;
                break;
            default:
                Assert.Fail($"This status {status} is not supported.");
                break;
        }

        result.ResultState = state;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.TextColor, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for DurationString Property

    [Test]
    public void TestDurationStringPropertyReturnsFormattedDurationString([Values] bool hasResult,
        [Values] bool inMilliseconds)
    {
        double count = inMilliseconds ? 0.005123456 : 5.123456;
        string unit = inMilliseconds ? "ms" : "sec";
        string expected = hasResult ? "5.123 " + unit : "0 sec";
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Duration = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.DurationString, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for HasInconclusive Property

    [Test]
    public void TestHasInconclusivePropertyWithNotInconclusiveReturnsFalse([Values] bool hasResult)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.InconclusiveCount = 0;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.InconclusiveCount, Is.EqualTo(0));
        Assert.That(test.HasInconclusive, Is.False);
    }

    [Test]
    public void TestHasInconclusivePropertyWithInconclusiveReturnsTrue()
    {
        const int count = 5;
        TestResultForTest result = new TestResultForTest();
        result.InconclusiveCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.InconclusiveCount, Is.EqualTo(count));
        Assert.That(test.HasInconclusive, Is.True);
    }

    #endregion

    #region Tests for HasWarning Property

    [Test]
    public void TestHasWarningPropertyWithNoWarningReturnsFalse([Values] bool hasResult)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.InconclusiveCount = 0;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.WarningCount, Is.EqualTo(0));
        Assert.That(test.HasWarning, Is.False);
    }

    [Test]
    public void TestHasWarningPropertyWithWarningReturnsTrue()
    {
        const int count = 5;
        TestResultForTest result = new TestResultForTest();
        result.WarningCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.WarningCount, Is.EqualTo(count));
        Assert.That(test.HasWarning, Is.True);
    }

    #endregion

    #region Tests for HasSkip Property

    [Test]
    public void TestHasSkipPropertyWithNoSkipReturnsFalse([Values] bool hasResult)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.SkipCount = 0;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.SkipCount, Is.EqualTo(0));
        Assert.That(test.HasSkip, Is.False);
    }

    [Test]
    public void TestHasSkipPropertyWithSkipReturnsTrue()
    {
        const int count = 5;
        TestResultForTest result = new TestResultForTest();
        result.SkipCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.SkipCount, Is.EqualTo(count));
        Assert.That(test.HasSkip, Is.True);
    }

    #endregion

    #region Tests for HasOutput Property

    [Test]
    public void TestHasOutputPropertyWithNoOutputReturnsFalse([Values] bool hasResult, [Values] bool isNull)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Output = isNull ? null : string.Empty;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Output, Is.EqualTo(string.Empty));
        Assert.That(test.HasOutput, Is.False);
    }

    [Test]
    public void TestHasOutputPropertyWithOutputReturnsTrue()
    {
        const string msg = "This is a test message.";
        TestResultForTest result = new TestResultForTest();
        result.Output = msg;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Output, Is.EqualTo(msg));
        Assert.That(test.HasOutput, Is.True);
    }

    #endregion

    #region Tests for HasMessage Property

    [Test]
    public void TestHasMessagePropertyWithNoMessageReturnsFalse([Values] bool hasResult, [Values] bool isNull)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Message = isNull ? null : string.Empty;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Message, Is.EqualTo(string.Empty));
        Assert.That(test.HasMessage, Is.False);
    }

    [Test]
    public void TestHasMessagePropertyWithMessageAndFailedAssertionsReturnsFalse([Values] bool hasResult)
    {
        const string msg = "This is a test message.";
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Failed, "message", "trace")
        };

        TestResultForTest result = new TestResultForTest();
        result.Message = msg;
        result.AssertionResults = assertions;
        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Message, Is.EqualTo(msg));
        Assert.That(test.HasFailedAssertions, Is.True);
        Assert.That(test.HasMessage, Is.False);
    }

    [Test]
    public void TestHasMessagePropertyWithMessageReturnsTrue()
    {
        const string msg = "This is a test message.";
        TestResultForTest result = new TestResultForTest();
        result.Message = msg;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Message, Is.EqualTo(msg));
        Assert.That(test.HasMessage, Is.True);
    }

    #endregion

    #region Tests for HasStackTrace Property

    [Test]
    public void TestHasStackTracePropertyWithNoStackTraceReturnsFalse([Values] bool hasResult,
        [Values] bool isNull)
    {
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.StackTrace = isNull ? null : string.Empty;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.StackTrace, Is.EqualTo(string.Empty));
        Assert.That(test.HasStackTrace, Is.False);
    }

    [Test]
    public void TestHasStackTracePropertyWithStackTraceAndFailedAssertionsReturnsFalse()
    {
        const string msg = "This is a test message.";
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Failed, "message", "trace")
        };

        TestResultForTest result = new TestResultForTest();
        result.StackTrace = msg;
        result.AssertionResults = assertions;
        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.StackTrace, Is.EqualTo(msg));
        Assert.That(test.HasFailedAssertions, Is.True);
        Assert.That(test.HasStackTrace, Is.False);
    }

    [Test]
    public void TestHasStackTracePropertyWithStackTraceReturnsTrue()
    {
        const string msg = "This is a test message.";
        TestResultForTest result = new TestResultForTest();
        result.StackTrace = msg;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.StackTrace, Is.EqualTo(msg));
        Assert.That(test.HasStackTrace, Is.True);
    }

    #endregion

    #region Tests for HasFailedAssertions Property

    [Test]
    public void TestHasFailedAssertionsPropertyReturnsIfTestsHasFailedAssertion([Values] bool hasResult,
        [Values] bool hasAssertions, [Values] AssertionStatus status)
    {
        bool expected = hasResult && hasAssertions && status != AssertionStatus.Passed;
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Passed, "message", "trace"),
            null,
            new AssertionResult(status, "message", "trace"),
            new AssertionResult(AssertionStatus.Passed, "message", "trace")
        };

        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.AssertionResults = hasAssertions ? assertions : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasFailedAssertions, Is.EqualTo(expected));
        if (hasResult && hasAssertions)
        {
            Assert.That(test.Result.AssertionResults, Is.EqualTo(assertions));
        }
        else
        {
            Assert.That(test.Result.AssertionResults, Is.Empty);
        }
    }

    #endregion

    #region Tests for FailedAssertionsString Property

    [Test]
    public void TestFailedAssertionsStringPropertyReturnsIfFormattedFailedAssertionString([Values] bool hasResult,
        [Values] bool hasAssertions, [Values] bool missingIsNull, [Values] AssertionStatus status)
    {
        string nl = Environment.NewLine;
        string missing = missingIsNull ? null : string.Empty;
        bool expected = hasResult && hasAssertions && status != AssertionStatus.Passed;
        string expectedMsg = expected
            ? $"Assertion Status: {status}{nl}message 2{nl}StackTrace:{nl}trace 2{nl}" +
              $"Assertion Status: {status}{nl}StackTrace:{nl}trace 3{nl}" +
              $"Assertion Status: {status}{nl}message 4"
            : string.Empty;
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Passed, "message 1", "trace 1"),
            null,
            new AssertionResult(status, "message 2", "trace 2"),
            new AssertionResult(status, missing, "trace 3"),
            new AssertionResult(status, "message 4", missing),
        };

        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.AssertionResults = hasAssertions ? assertions : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasFailedAssertions, Is.EqualTo(expected));
        Assert.That(test.FailedAssertionsString, Is.EqualTo(expectedMsg));
        if (hasResult && hasAssertions)
        {
            Assert.That(test.Result.AssertionResults, Is.EqualTo(assertions));
        }
        else
        {
            Assert.That(test.Result.AssertionResults, Is.Empty);
        }
    }

    #endregion

    #region Tests for ToXml

    [Test]
    public void TestToXml([Values] bool hasResult, [Values] bool recursive)
    {
        ITestResult resultInstance = new TestSuiteResult(new TestSuite("suite-name"));
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        TNode node = test.Result.ToXml(recursive);

        if (hasResult)
        {
            Assert.That(node, Is.Not.Null);
        }
        else
        {
            Assert.That(node, Is.Null);
        }
    }

    #endregion

    #region Tests for AddToXml

    [Test]
    public void TestAddToXml([Values] bool hasResult, [Values] bool isParentNull, [Values] bool recursive)
    {
        ITestResult resultInstance = new TestSuiteResult(new TestSuite("suite-name"));
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        TNode parent = isParentNull ? null : new TNode("parent-node");

        // Parent node null is not handled by NUnit implementation of ITestResult nor the thin wrapper being tested here
        if (hasResult && isParentNull)
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws(Is.TypeOf<NullReferenceException>(), () => test.Result.AddToXml(parent, recursive));
            return;
        }

        TNode node = test.Result.AddToXml(parent, recursive);

        if (hasResult)
        {
            Assert.That(node, Is.Not.Null);
        }
        else
        {
            Assert.That(node, Is.Null);
        }
    }

    #endregion

    #region Tests for ResultState Property

    [Test]
    public void TestResultStatePropertyReturnsResultState([Values] bool hasResult, [Values] bool hasState)
    {
        ResultState state = ResultState.Success;
        ResultState expected = hasResult && hasState ? state : ResultState.Inconclusive;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.ResultState = hasState ? state : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.ResultState, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Name Property

    [Test]
    public void TestNamePropertyReturnsResultName([Values] bool hasResult, [Values] bool hasName)
    {
        const string name = "result-name";
        string expected = hasResult && hasName ? name : string.Empty;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Name = hasName ? name : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Name, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for FullName Property

    [Test]
    public void TestFullNamePropertyReturnsResultFullName([Values] bool hasResult, [Values] bool hasName)
    {
        const string name = "result-name";
        string expected = hasResult && hasName ? name : string.Empty;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.FullName = hasName ? name : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.FullName, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Duration Property

    [Test]
    public void TestDurationPropertyReturnsTestDuration([Values] bool hasResult)
    {
        const double duration = 5.123456;
        double expected = hasResult ? duration : 0;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Duration = duration;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Duration, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for StartTime Property

    [Test]
    public void TestStartTimePropertyReturnsTestStartTime([Values] bool hasResult)
    {
        DateTime time = DateTime.Now;
        DateTime expected = hasResult ? time : DateTime.MinValue;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.StartTime = time;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.StartTime, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for EndTime Property

    [Test]
    public void TestEndTimePropertyReturnsTestEndTime([Values] bool hasResult)
    {
        DateTime time = DateTime.Now;
        DateTime expected = hasResult ? time : DateTime.MaxValue;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.EndTime = time;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.EndTime, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Message Property

    [Test]
    public void TestMessagePropertyReturnsResultExceptionMessage([Values] bool hasResult, [Values] bool hasMsg)
    {
        const string msg = "This is a test message.";
        string expected = hasResult && hasMsg ? msg : string.Empty;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Message = hasMsg ? msg : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Message, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for StackTrace Property

    [Test]
    public void TestStackTracePropertyReturnsResultExceptionStackTrace([Values] bool hasResult,
        [Values] bool hasTrace)
    {
        const string trace = "This is a test message.";
        string expected = hasResult && hasTrace ? trace : string.Empty;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.StackTrace = hasTrace ? trace : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.StackTrace, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for AssertCount Property

    [Test]
    public void TestAssertCountPropertyReturnsAssertCount([Values] bool hasResult)
    {
        const int count = 5;
        int expected = hasResult ? count : 0;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.AssertCount = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.AssertCount, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for FailCount Property

    [Test]
    public void TestFailCountPropertyReturnsFailCount([Values] bool hasResult)
    {
        const int count = 5;
        int expected = hasResult ? count : 0;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.FailCount = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.FailCount, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for WarningCount Property

    [Test]
    public void TestWarningCountPropertyReturnsWarningCount([Values] bool hasResult)
    {
        const int count = 5;
        int expected = hasResult ? count : 0;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.WarningCount = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.WarningCount, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for PassCount Property

    [Test]
    public void TestPassCountPropertyReturnsPassCount([Values] bool hasResult)
    {
        const int count = 5;
        int expected = hasResult ? count : 0;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.PassCount = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.PassCount, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for SkipCount Property

    [Test]
    public void TestSkipCountPropertyReturnsSkipCount([Values] bool hasResult)
    {
        const int count = 5;
        int expected = hasResult ? count : 0;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.SkipCount = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.SkipCount, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for InconclusiveCount Property

    [Test]
    public void TestInconclusiveCountPropertyReturnsInconclusiveCount([Values] bool hasResult)
    {
        const int count = 5;
        int expected = hasResult ? count : 0;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.InconclusiveCount = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.InconclusiveCount, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for HasChildren Property

    [Test]
    public void TestHasChildrenPropertyReturnsIfTestHasChildren([Values] bool hasResult, [Values] bool hasChildren)
    {
        bool expected = hasResult && hasChildren;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.HasChildren = hasChildren;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasChildren, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Children Property

    [Test]
    public void TestChildrenPropertyReturnsChildren([Values] bool hasResult, [Values] bool hasChildren)
    {
        IEnumerable<ITestResult> children = new List<ITestResult>();
        IEnumerable<ITestResult> expected = hasResult && hasChildren ? children : null;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Children = expected;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Children, Is.SameAs(expected));
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestTestPropertyReturnsTest([Values] bool hasResult, [Values] bool hasTest)
    {
        ITest testInstance = new TestSuite("suite-name");
        ITest expected = hasResult && hasTest ? testInstance : null;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Test = expected;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Test, Is.SameAs(expected));
    }

    #endregion

    #region Tests for Output Property

    [Test]
    public void TestOutputPropertyReturnsResultOutputMessages([Values] bool hasResult, [Values] bool hasOutput)
    {
        const string output = "This is a test message.";
        string expected = hasResult && hasOutput ? output : string.Empty;
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.Output = hasOutput ? output : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result.Output, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for AssertionResults Property

    [Test]
    public void TestAssertionResultsPropertyReturnsTestAssertionResults([Values] bool hasResult,
        [Values] bool hasAssertions)
    {
        IList<AssertionResult> assertions = new List<AssertionResult>
            {new AssertionResult(AssertionStatus.Passed, "message", "trace")};
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.AssertionResults = hasAssertions ? assertions : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        if (hasResult && hasAssertions)
        {
            Assert.That(test.Result.AssertionResults, Is.EqualTo(assertions));
        }
        else
        {
            Assert.That(test.Result.AssertionResults, Is.Empty);
        }
    }

    #endregion

    #region Tests for TestAttachments Property

    [Test]
    public void TestTestAttachmentsPropertyReturnsTestAttachments([Values] bool hasResult,
        [Values] bool hasAttachments)
    {
        IList<TestAttachment> attachments = new List<TestAttachment> {new TestAttachment("file.txt", "item")};
        TestResultForTest resultInstance = new TestResultForTest();
        resultInstance.TestAttachments = hasAttachments ? attachments : null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        if (hasResult && hasAttachments)
        {
            Assert.That(test.Result.TestAttachments, Is.EqualTo(attachments));
        }
        else
        {
            Assert.That(test.Result.TestAttachments, Is.Empty);
        }
    }

    #endregion

    #region Tests for Equals

    [Test]
    public void TestEqualsWithSameResultReturnsTrue([Values] bool isNull)
    {
        TestResultForTest resultInstanceOne = new TestResultForTest();
        ITestResult resultOne = isNull ? null : resultInstanceOne;

        INUnitTestResult testOne = new NUnitTestResult(resultOne);

        Assert.That(testOne.Equals(resultOne), Is.True);
        // ReSharper disable once EqualExpressionComparison
        Assert.That(testOne.Equals(testOne), Is.True);
    }

    [Test]
    public void TestEqualsWithNotSameResultReturnsFalse([Values] bool isNull)
    {
        TestResultForTest resultInstanceOne = new TestResultForTest();
        TestResultForTest resultInstanceTwo = new TestResultForTest();
        resultInstanceTwo.Name = "result-name";
        ITestResult resultOne = isNull ? null : resultInstanceOne;
        ITestResult resultTwo = isNull ? resultInstanceTwo : null;
        object resultWrong = "string";

        INUnitTestResult testOne = new NUnitTestResult(resultOne);
        INUnitTestResult testTwo = new NUnitTestResult(resultTwo);

        Assert.That(testOne.Equals(resultTwo), Is.False);
        Assert.That(testOne.Equals(testTwo), Is.False);
        Assert.That(testOne.Equals(resultWrong), Is.False);
    }

    #endregion
}