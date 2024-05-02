// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using MauiNUnitRunner.Controls.Models;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable AssignNullToNotNullAttribute

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class NUnitTestResultTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructorWithTestResult([Values] bool isNull)
    {
        ITestResult result = isNull ? null : new TestResultStub();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.SameAs(result));
    }

    #endregion

    #region Tests for Id Property

    [Test]
    public void TestIdProperty([Values] bool isResultNull, [Values] bool isTestNull, [Values] bool isIdNull)
    {
        string expected = isResultNull || isTestNull || isIdNull ? string.Empty : "test-123";
        TestResultStub result = isResultNull ? null : new TestResultStub();
        if (result != null)
        {
            result.Test = isTestNull ? null : new TestStub { Id = isIdNull ? null : "test-123" };
        }

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Id, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Result Property

    [Test]
    public void TestResultProperty([Values] bool isNull)
    {
        ITestResult result = isNull ? null : new TestResultStub();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.SameAs(result));
    }

    #endregion

    #region Tests for Children Property

    [Test]
    public void TestChildrenPropertyReturnsChildren([Values] bool hasChildren, [Values] bool isNull)
    {
        ITestResult resultInstance = new TestResultStub();
        IEnumerable<ITestResult> children = hasChildren ? [resultInstance] : isNull ? null : Array.Empty<ITestResult>();
        TestResultStub result = new TestResultStub();
        result.Children = children;

        IEnumerable<INUnitTestResult> expected =
            hasChildren ? [new NUnitTestResult(resultInstance)] : Array.Empty<INUnitTestResult>();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Children, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for HasChildren Property

    [Test]
    public void TestHasChildrenPropertyReturnsIfTestHasChildren([Values] bool hasChildren, [Values] bool isNull)
    {
        IList<ITestResult> children = [new TestResultStub()];
        IEnumerable<ITestResult> expected = hasChildren ? children : isNull ? null : Array.Empty<ITestResult>();
        TestResultStub result = new TestResultStub();
        result.Children = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasChildren, Is.EqualTo(hasChildren));
    }

    #endregion

    #region Tests for HasNoChildren Property

    [Test]
    public void TestHasNoChildrenPropertyReturnsIfTestHasNoChildren([Values] bool hasChildren, [Values] bool isNull)
    {
        IList<ITestResult> children = [new TestResultStub()];
        IEnumerable<ITestResult> expected = hasChildren ? children : isNull ? null : Array.Empty<ITestResult>();
        TestResultStub result = new TestResultStub();
        result.Children = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasNoChildren, Is.Not.EqualTo(hasChildren));
    }

    #endregion

    #region Tests for ResultStateStatus Property

    [Test]
    public void TestResultStateStatusPropertyWithResultNullReturnsDefaultString()
    {
        TestResultStub result = new TestResultStub();
        result.ResultState = null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result, Is.EqualTo(result));
        Assert.That(test.ResultStateStatus, Is.EqualTo("Test not executed."));
    }

    [Test]
    public void TestResultStateStatusPropertyWithResultReturnsResultStateStatusString()
    {
        ResultState state = ResultState.Success;
        TestResultStub result = new TestResultStub();
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
        TestResultStub resultInstance = new TestResultStub();
        resultInstance.ResultState = null;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.TextColor, Is.EqualTo(Colors.Black));
    }

    [Test]
    public void TestTextColorPropertyWithNotSupportedTestStatusReturnsColorBlack()
    {
        TestResultStub result = new TestResultStub();
        result.ResultState = new ResultState((TestStatus) (-1));

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.TextColor, Is.EqualTo(Colors.Black));
    }

    [Test]
    public void TestTextColorPropertyReturnsColorForResultState([Values] TestStatus status)
    {
        TestResultStub result = new TestResultStub();
        result.ResultState = new ResultState((TestStatus) (-1));

        Color expected = Colors.Black;
        ResultState state = ResultState.NotRunnable;
        switch (status)
        {
            case TestStatus.Inconclusive:
                state = ResultState.Inconclusive;
                expected = Colors.MediumPurple;
                break;
            case TestStatus.Skipped:
                state = ResultState.Ignored;
                expected = Colors.DodgerBlue;
                break;
            case TestStatus.Passed:
                state = ResultState.Success;
                expected = Colors.LimeGreen;
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
        TestResultStub resultInstance = new TestResultStub();
        resultInstance.Duration = count;
        ITestResult result = hasResult ? resultInstance : null;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.DurationString, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for HasTestResult Property

    [Test]
    public void TestHasTestResultPropertyWithNoResultReturnsFalse()
    {
        INUnitTestResult test = new NUnitTestResult(null);

        Assert.That(test.Result, Is.Null);
        Assert.That(test.HasTestResult, Is.False);
    }

    [Test]
    public void TestHasTestResultPropertyWithResultReturnsTrue()
    {
        TestResultStub result = new TestResultStub();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.HasTestResult, Is.True);
    }

    #endregion

    #region Tests for HasFailed Property

    [Test]
    public void TestHasFailedPropertyWithNoFailedReturnsFalse()
    {
        TestResultStub result = new TestResultStub();
        result.FailCount = 0;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.FailCount, Is.EqualTo(0));
        Assert.That(test.HasFailed, Is.False);
    }

    [Test]
    public void TestHasFailedPropertyWithFailedReturnsTrue()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.FailCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.FailCount, Is.EqualTo(count));
        Assert.That(test.HasFailed, Is.True);
    }

    #endregion

    #region Tests for HasInconclusive Property

    [Test]
    public void TestHasInconclusivePropertyWithNotInconclusiveReturnsFalse()
    {
        TestResultStub result = new TestResultStub();
        result.InconclusiveCount = 0;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.InconclusiveCount, Is.EqualTo(0));
        Assert.That(test.HasInconclusive, Is.False);
    }

    [Test]
    public void TestHasInconclusivePropertyWithInconclusiveReturnsTrue()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.InconclusiveCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.InconclusiveCount, Is.EqualTo(count));
        Assert.That(test.HasInconclusive, Is.True);
    }

    #endregion

    #region Tests for HasWarning Property

    [Test]
    public void TestHasWarningPropertyWithNoWarningReturnsFalse()
    {
        TestResultStub result = new TestResultStub();
        result.InconclusiveCount = 0;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.WarningCount, Is.EqualTo(0));
        Assert.That(test.HasWarning, Is.False);
    }

    [Test]
    public void TestHasWarningPropertyWithWarningReturnsTrue()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.WarningCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.WarningCount, Is.EqualTo(count));
        Assert.That(test.HasWarning, Is.True);
    }

    #endregion

    #region Tests for HasSkip Property

    [Test]
    public void TestHasSkipPropertyWithNoSkipReturnsFalse()
    {
        TestResultStub result = new TestResultStub();
        result.SkipCount = 0;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.SkipCount, Is.EqualTo(0));
        Assert.That(test.HasSkip, Is.False);
    }

    [Test]
    public void TestHasSkipPropertyWithSkipReturnsTrue()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.SkipCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.SkipCount, Is.EqualTo(count));
        Assert.That(test.HasSkip, Is.True);
    }

    #endregion

    #region Tests for HasOutput Property

    [Test]
    public void TestHasOutputPropertyWithNoOutputReturnsFalse([Values] bool isNull)
    {
        string expected = isNull ? null : string.Empty;
        TestResultStub result = new TestResultStub();
        result.Output = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Output, Is.EqualTo(expected));
        Assert.That(test.HasOutput, Is.False);
    }

    [Test]
    public void TestHasOutputPropertyWithOutputReturnsTrue()
    {
        const string msg = "This is a test message.";
        TestResultStub result = new TestResultStub();
        result.Output = msg;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Output, Is.EqualTo(msg));
        Assert.That(test.HasOutput, Is.True);
    }

    #endregion

    #region Tests for HasMessage Property

    [Test]
    public void TestHasMessagePropertyWithNoMessageReturnsFalse([Values] bool isNull)
    {
        string expected = isNull ? null : string.Empty;
        TestResultStub result = new TestResultStub();
        result.Message = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Message, Is.EqualTo(expected));
        Assert.That(test.HasMessage, Is.False);
    }

    [Test]
    public void TestHasMessagePropertyWithMessageAndFailedAssertionsReturnsFalse()
    {
        const string msg = "This is a test message.";
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Failed, msg, "trace")
        };

        TestResultStub result = new TestResultStub();
        result.Message = msg;
        result.AssertionResults = assertions;
        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Message, Is.EqualTo(msg));
        Assert.That(test.HasFailedAssertions, Is.True);
        Assert.That(test.HasMessage, Is.False);
    }

    [Test]
    public void TestHasMessagePropertyWithMessageAndPassedOrNoAssertionsReturnsTrue([Values] bool hasAssertions)
    {
        const string msg = "This is a test message.";
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Passed, "message", "trace")
        };

        TestResultStub result = new TestResultStub();
        result.Message = msg;
        result.AssertionResults = hasAssertions ? assertions : Array.Empty<AssertionResult>();
        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Message, Is.EqualTo(msg));
        Assert.That(test.HasFailedAssertions, Is.False);
        Assert.That(test.HasMessage, Is.True);
    }

    [Test]
    public void TestHasMessagePropertyWithMessageReturnsTrue()
    {
        const string msg = "This is a test message.";
        TestResultStub result = new TestResultStub();
        result.Message = msg;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Message, Is.EqualTo(msg));
        Assert.That(test.HasMessage, Is.True);
    }

    #endregion

    #region Tests for HasStackTrace Property

    [Test]
    public void TestHasStackTracePropertyWithNoStackTraceReturnsFalse([Values] bool isNull)
    {
        string expected = isNull ? null : string.Empty;
        TestResultStub result = new TestResultStub();
        result.StackTrace = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.StackTrace, Is.EqualTo(expected));
        Assert.That(test.HasStackTrace, Is.False);
    }

    [Test]
    public void TestHasStackTracePropertyWithStackTraceAndFailedAssertionsReturnsFalse()
    {
        const string msg = "This is a test message.";
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Failed, "message", msg)
        };

        TestResultStub result = new TestResultStub();
        result.StackTrace = msg;
        result.AssertionResults = assertions;
        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.StackTrace, Is.EqualTo(msg));
        Assert.That(test.HasFailedAssertions, Is.True);
        Assert.That(test.HasStackTrace, Is.False);
    }

    [Test]
    public void TestHasStackTracePropertyWithStackTraceAndPassedOrNoAssertionsReturnsTrue([Values] bool hasAssertions)
    {
        const string msg = "This is a test message.";
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Passed, "message", "trace")
        };

        TestResultStub result = new TestResultStub();
        result.StackTrace = msg;
        result.AssertionResults = hasAssertions ? assertions : Array.Empty<AssertionResult>();
        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.StackTrace, Is.EqualTo(msg));
        Assert.That(test.HasFailedAssertions, Is.False);
        Assert.That(test.HasStackTrace, Is.True);
    }

    [Test]
    public void TestHasStackTracePropertyWithStackTraceReturnsTrue()
    {
        const string msg = "This is a test message.";
        TestResultStub result = new TestResultStub();
        result.StackTrace = msg;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.StackTrace, Is.EqualTo(msg));
        Assert.That(test.HasStackTrace, Is.True);
    }

    #endregion

    #region Tests for HasFailedAssertions Property

    [Test]
    public void TestHasFailedAssertionsPropertyReturnsIfTestsHasFailedAssertion([Values] bool hasAssertions,
        [Values] AssertionStatus status, [Values] bool isNull)
    {
        bool expected = hasAssertions && status != AssertionStatus.Passed;
        IList<AssertionResult> assertions = new List<AssertionResult>
        {
            new AssertionResult(AssertionStatus.Passed, "message", "trace"),
            null,
            new AssertionResult(status, "message", "trace"),
            new AssertionResult(AssertionStatus.Passed, "message", "trace")
        };

        TestResultStub result = new TestResultStub();
        result.AssertionResults = hasAssertions ? assertions : isNull ? null : Array.Empty<AssertionResult>();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasFailedAssertions, Is.EqualTo(expected));
        Assert.That(test.Result, Is.Not.Null);
        if (hasAssertions)
        {
            Assert.That(test.Result.AssertionResults, Is.EqualTo(assertions));
        }
        else
        {
            Assert.That(test.Result.AssertionResults, Is.Null.Or.Empty);
        }
    }

    #endregion

    #region Tests for FailedAssertionsString Property

    [Test]
    public void TestFailedAssertionsStringPropertyWithNullResultReturnsEmptyString()
    {
        INUnitTestResult test = new NUnitTestResult(null);

        Assert.That(test.HasFailedAssertions, Is.False);
        Assert.That(test.FailedAssertionsString, Is.Empty);
        Assert.That(test.Result, Is.Null);
    }

    [Test]
    public void TestFailedAssertionsStringPropertyWithNoAssertionsReturnsEmptyString([Values] bool isNull)
    {
        TestResultStub result = new TestResultStub();
        result.ResultState = AssertionStatusToResultState(AssertionStatus.Passed);
        result.AssertionResults = isNull ? null : Array.Empty<AssertionResult>();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasFailedAssertions, Is.False);
        Assert.That(test.FailedAssertionsString, Is.Empty);
        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.AssertionResults, Is.Null.Or.Empty);
    }

    [Test]
    public void TestFailedAssertionsStringPropertyWithAssertionsAndIsTestSuiteReturnsFormattedFailedAssertionString(
        [Values] AssertionStatus status, [Values] bool missingIsNull, [Values] bool isSuite)
    {
        string nl = Environment.NewLine;
        string missing = missingIsNull ? null : string.Empty;
        bool expected = status != AssertionStatus.Passed;
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

        TestResultStub result = new TestResultStub();
        result.ResultState = AssertionStatusToResultState(status);
        result.TestResultXml =
            TNode.FromXml(
                $"<test-case><failure><message><![CDATA[message {new string('a', 1000)}]]></message><stack-trace><![CDATA[trace 5]]></stack-trace></failure></test-case>");
        result.Test = isSuite ? new TestStub() { IsSuite = true } : null;
        result.AssertionResults = assertions;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasFailedAssertions, Is.EqualTo(expected));
        Assert.That(test.FailedAssertionsString, Is.EqualTo(expectedMsg));
        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.AssertionResults, Is.EqualTo(assertions));
    }

    [Test]
    public void TestFailedAssertionsStringPropertyWithAssertionsAndIsNotTestSuiteAndFromXmlShorterReturnsFormattedFailedAssertionString(
        [Values] AssertionStatus status, [Values] bool missingIsNull)
    {
        string nl = Environment.NewLine;
        string missing = missingIsNull ? null : string.Empty;
        bool expected = status != AssertionStatus.Passed;
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

        TestResultStub result = new TestResultStub();
        result.ResultState = AssertionStatusToResultState(status);
        result.TestResultXml =
            TNode.FromXml(
                "<test-case><failure><message><![CDATA[message 5]]></message><stack-trace><![CDATA[trace 5]]></stack-trace></failure></test-case>");
        result.Test = new TestStub() { IsSuite = false };
        result.AssertionResults = assertions;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.HasFailedAssertions, Is.EqualTo(expected));
        Assert.That(test.FailedAssertionsString, Is.EqualTo(expectedMsg));
        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.AssertionResults, Is.EqualTo(assertions));
    }

    #endregion

    #region Tests for ToXml

    [Test]
    public void TestToXml([Values] bool recursive)
    {
        ITestResult result = new TestSuiteResult(new TestSuite("suite-name"));

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);

        TNode node = test.Result.ToXml(recursive);

        Assert.That(node, Is.Not.Null);
    }

    #endregion

    #region Tests for AddToXml

    [Test]
    public void TestAddToXml([Values] bool isParentNull, [Values] bool recursive)
    {
        ITestResult result = new TestSuiteResult(new TestSuite("suite-name"));

        INUnitTestResult test = new NUnitTestResult(result);

        TNode parent = isParentNull ? null : new TNode("parent-node");

        Assert.That(test.Result, Is.Not.Null);

        // Parent node null is not handled by NUnit implementation of ITestResult nor the thin wrapper being tested here
        if (isParentNull)
        {
            // ReSharper disable once ExpressionIsAlwaysNull
            Assert.Throws(Is.TypeOf<NullReferenceException>(), () => test.Result.AddToXml(parent, recursive));
            return;
        }

        TNode node = test.Result.AddToXml(parent, recursive);

        Assert.That(node, Is.Not.Null);
    }

    #endregion

    #region Tests for ResultState Property

    [Test]
    public void TestResultStatePropertyReturnsResultState([Values] bool hasState)
    {
        ResultState state = ResultState.Success;
        ResultState expected = hasState ? state : ResultState.Inconclusive;
        TestResultStub result = new TestResultStub();
        result.ResultState = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.ResultState, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Name Property

    [Test]
    public void TestNamePropertyReturnsResultName([Values] bool hasName)
    {
        const string name = "result-name";
        string expected = hasName ? name : string.Empty;
        TestResultStub result = new TestResultStub();
        result.Name = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Name, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for FullName Property

    [Test]
    public void TestFullNamePropertyReturnsResultFullName([Values] bool hasName)
    {
        const string name = "result-name";
        string expected = hasName ? name : string.Empty;
        TestResultStub result = new TestResultStub();
        result.FullName = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.FullName, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Duration Property

    [Test]
    public void TestDurationPropertyReturnsTestDuration()
    {
        const double duration = 5.123456;
        TestResultStub result = new TestResultStub();
        result.Duration = duration;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Duration, Is.EqualTo(duration));
    }

    #endregion

    #region Tests for StartTime Property

    [Test]
    public void TestStartTimePropertyReturnsTestStartTime()
    {
        DateTime time = DateTime.Now;
        TestResultStub result = new TestResultStub();
        result.StartTime = time;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.StartTime, Is.EqualTo(time));
    }

    #endregion

    #region Tests for EndTime Property

    [Test]
    public void TestEndTimePropertyReturnsTestEndTime()
    {
        DateTime time = DateTime.Now;
        TestResultStub result = new TestResultStub();
        result.EndTime = time;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.EndTime, Is.EqualTo(time));
    }

    #endregion

    #region Tests for Message Property

    [Test]
    public void TestMessagePropertyReturnsResultExceptionMessage([Values] bool hasMsg)
    {
        const string msg = "This is a test message.";
        string expected = hasMsg ? msg : string.Empty;
        TestResultStub result = new TestResultStub();
        result.Message = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Message, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for StackTrace Property

    [Test]
    public void TestStackTracePropertyReturnsResultExceptionStackTrace([Values] bool hasTrace)
    {
        const string trace = "This is a test message.";
        string expected = hasTrace ? trace : string.Empty;
        TestResultStub result = new TestResultStub();
        result.StackTrace = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.StackTrace, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for AssertCount Property

    [Test]
    public void TestAssertCountPropertyReturnsAssertCount()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.AssertCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.AssertCount, Is.EqualTo(count));
    }

    #endregion

    #region Tests for FailCount Property

    [Test]
    public void TestFailCountPropertyReturnsFailCount()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.FailCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.FailCount, Is.EqualTo(count));
    }

    #endregion

    #region Tests for WarningCount Property

    [Test]
    public void TestWarningCountPropertyReturnsWarningCount()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.WarningCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.WarningCount, Is.EqualTo(count));
    }

    #endregion

    #region Tests for PassCount Property

    [Test]
    public void TestPassCountPropertyReturnsPassCount()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.PassCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.PassCount, Is.EqualTo(count));
    }

    #endregion

    #region Tests for SkipCount Property

    [Test]
    public void TestSkipCountPropertyReturnsSkipCount()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.SkipCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.SkipCount, Is.EqualTo(count));
    }

    #endregion

    #region Tests for InconclusiveCount Property

    [Test]
    public void TestInconclusiveCountPropertyReturnsInconclusiveCount()
    {
        const int count = 5;
        TestResultStub result = new TestResultStub();
        result.InconclusiveCount = count;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.InconclusiveCount, Is.EqualTo(count));
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestTestPropertyReturnsTest([Values] bool hasTest)
    {
        ITest testInstance = new TestSuite("suite-name");
        ITest expected = hasTest ? testInstance : null;
        TestResultStub result = new TestResultStub();
        result.Test = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Test, Is.SameAs(expected));
    }

    #endregion

    #region Tests for Output Property

    [Test]
    public void TestOutputPropertyReturnsResultOutputMessages([Values] bool hasOutput)
    {
        const string output = "This is a test message.";
        string expected = hasOutput ? output : string.Empty;
        TestResultStub result = new TestResultStub();
        result.Output = expected;

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.Result.Output, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for AssertionResults Property

    [Test]
    public void TestAssertionResultsPropertyReturnsTestAssertionResults([Values] bool hasAssertions, [Values] bool isNull)
    {
        IList<AssertionResult> assertions = new List<AssertionResult>
            {new AssertionResult(AssertionStatus.Passed, "message", "trace")};
        TestResultStub result = new TestResultStub();
        result.AssertionResults = hasAssertions ? assertions : isNull ? null : Array.Empty<AssertionResult>();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        if (hasAssertions)
        {
            Assert.That(test.Result.AssertionResults, Is.EqualTo(assertions));
        }
        else
        {
            Assert.That(test.Result.AssertionResults, Is.Null.Or.Empty);
        }
    }

    #endregion

    #region Tests for TestAttachments Property

    [Test]
    public void TestTestAttachmentsPropertyReturnsTestAttachments([Values] bool hasAttachments, [Values] bool isNull)
    {
        IList<TestAttachment> attachments = new List<TestAttachment> {new TestAttachment("file.txt", "item")};
        TestResultStub result = new TestResultStub();
        result.TestAttachments = hasAttachments ? attachments : isNull ? null : Array.Empty<TestAttachment>();

        INUnitTestResult test = new NUnitTestResult(result);

        Assert.That(test.Result, Is.Not.Null);
        if (hasAttachments)
        {
            Assert.That(test.Result.TestAttachments, Is.EqualTo(attachments));
        }
        else
        {
            Assert.That(test.Result.TestAttachments, Is.Null.Or.Empty);
        }
    }

    #endregion

    #region Tests for Equals

    [Test]
    public void TestEqualsWithSameResultReturnsTrue()
    {
        TestResultStub resultOne = new TestResultStub();

        INUnitTestResult testOne = new NUnitTestResult(resultOne);
        INUnitTestResult testNull = new NUnitTestResult(null);

        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.That(testOne.Equals(resultOne), Is.True);
        // ReSharper disable once EqualExpressionComparison
        Assert.That(testOne.Equals(testOne), Is.True);
        // ReSharper disable once EqualExpressionComparison
        Assert.That(testNull.Equals(testNull), Is.True);
    }

    [Test]
    public void TestEqualsWithNotSameResultReturnsFalse([Values] bool isNull)
    {
        TestResultStub resultInstanceOne = new TestResultStub();
        TestResultStub resultInstanceTwo = new TestResultStub();
        resultInstanceTwo.Name = "result-name";
        ITestResult resultOne = isNull ? null : resultInstanceOne;
        ITestResult resultTwo = isNull ? resultInstanceTwo : null;
        object resultWrong = "string";

        INUnitTestResult testOne = new NUnitTestResult(resultOne);
        INUnitTestResult testTwo = new NUnitTestResult(resultTwo);
        INUnitTestResult testNull = new NUnitTestResult(null);

        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.That(testOne.Equals(resultTwo), Is.False);
        Assert.That(testOne.Equals(testTwo), Is.False);
        Assert.That(testOne.Equals(testNull), Is.False);
        Assert.That(testOne.Equals(resultWrong), Is.False);
        Assert.That(testOne.Equals(null), Is.False);
        Assert.That(testNull.Equals(null), Is.False);
    }

    #endregion

    #region Tests for GetHashCode

    [Test]
    public void TestGetHashCode([Values] bool isNull)
    {
        TestResultStub resultInstanceOne = new TestResultStub();
        int hashCode = isNull ? 0 : resultInstanceOne.GetHashCode();
        ITestResult resultOne = isNull ? null : resultInstanceOne;

        INUnitTestResult testOne = new NUnitTestResult(resultOne);

        Assert.That(testOne.GetHashCode(), Is.EqualTo(hashCode));
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Helper method to convert an AssertionStatus to a ResultState.
    /// </summary>
    /// <param name="status">The AssertionStatus to convert.</param>
    /// <returns>The converted ResultState.</returns>
    private static ResultState AssertionStatusToResultState(AssertionStatus status)
    {

        switch (status)
        {
            case AssertionStatus.Inconclusive:
                return ResultState.Inconclusive;
            case AssertionStatus.Passed:
                return ResultState.Success;
            case AssertionStatus.Warning:
                return ResultState.Warning;
            case AssertionStatus.Failed:
                return ResultState.Failure;
            default:
                return ResultState.Error;
        }
    }

    #endregion
}
