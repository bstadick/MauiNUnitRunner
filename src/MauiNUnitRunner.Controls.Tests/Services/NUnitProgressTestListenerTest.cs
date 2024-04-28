// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Services;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Tests.Services;

[TestFixture]
public class NUnitProgressTestListenerTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructor()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        NUnitProgressTestListener listener = new NUnitProgressTestListener(state);

        Assert.That(listener.TestRunState, Is.SameAs(state));
    }

    [Test]
    public void TestConstructorThrowsArgumentNullExceptionWhenTestRunStateIsNull()
    {
        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The testRunState cannot be null. (Parameter 'testRunState')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitProgressTestListener(null));
    }

    #endregion

    #region Tests for TestRunState Property

    [Test]
    public void TestTestRunStateProperty()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        NUnitProgressTestListener listener = new NUnitProgressTestListener(state);

        Assert.That(listener.TestRunState, Is.SameAs(state));
    }

    #endregion

    #region Tests for TestStarted

    [Test]
    public void TestTestStartedIncrementsCountIfTestIsLeafNode([Values] bool isTestNull, [Values] bool isLeafNode)
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        NUnitProgressTestListener listener = new NUnitProgressTestListener(state);

        state.TestRunStartedCount = 5;

        Assert.That(state.IsTestRunning, Is.False);
        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(5));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));

        TestStub testInstance = new TestStub();
        // ReSharper disable once AssignNullToNotNullAttribute
        testInstance.Tests = isLeafNode ? null : new List<ITest> { new TestStub() };

        int expectedCount = !isTestNull && isLeafNode ? state.TestRunStartedCount + 1 : state.TestRunStartedCount;

        TestStub test = isTestNull ? null : testInstance;

        listener.TestStarted(test);

        Assert.That(state.IsTestRunning, Is.False);
        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(expectedCount));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));
    }

    #endregion

    #region Tests for TestFinished

    [Test]
    public void TestTestFinishedIncrementsCountIfTestIsLeafNode([Values] bool isResultNull, [Values] bool isLeafNode)
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        NUnitProgressTestListener listener = new NUnitProgressTestListener(state);

        state.TestRunFinishedCount = 5;

        Assert.That(state.IsTestRunning, Is.False);
        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(5));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));

        TestResultStub resultInstance = new TestResultStub();
        // ReSharper disable once AssignNullToNotNullAttribute
        resultInstance.Children = isLeafNode ? null : new List<ITestResult> { new TestResultStub() };

        int expectedCount = !isResultNull && isLeafNode ? state.TestRunFinishedCount + 1 : state.TestRunFinishedCount;

        TestResultStub result = isResultNull ? null : resultInstance;

        listener.TestFinished(result);

        Assert.That(state.IsTestRunning, Is.False);
        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(expectedCount));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));
    }

    #endregion

    #region Tests for TestOutput

    [Test]
    public void TestTestOutputDoesNothing([Values] bool isOutputNull)
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        NUnitProgressTestListener listener = new NUnitProgressTestListener(state);

        state.IsTestRunning = true;
        state.TestRunCount = 10;
        state.TestRunStartedCount = 7;
        state.TestRunFinishedCount = 5;

        Assert.That(state.IsTestRunning, Is.True);
        Assert.That(state.TestRunCount, Is.EqualTo(10));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(7));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(5));
        Assert.That(state.TestRunProgress, Is.EqualTo(0.5));

        TestOutput output = isOutputNull ? null : new TestOutput("", "", "", "");

        listener.TestOutput(output);

        Assert.That(state.IsTestRunning, Is.True);
        Assert.That(state.TestRunCount, Is.EqualTo(10));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(7));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(5));
        Assert.That(state.TestRunProgress, Is.EqualTo(0.5));
    }

    #endregion

    #region Tests for SendMessage

    [Test]
    public void TestSendMessageDoesNothing([Values] bool isMessageNull)
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        NUnitProgressTestListener listener = new NUnitProgressTestListener(state);

        state.IsTestRunning = true;
        state.TestRunCount = 10;
        state.TestRunStartedCount = 7;
        state.TestRunFinishedCount = 5;

        Assert.That(state.IsTestRunning, Is.True);
        Assert.That(state.TestRunCount, Is.EqualTo(10));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(7));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(5));
        Assert.That(state.TestRunProgress, Is.EqualTo(0.5));

        TestMessage msg = isMessageNull ? null : new TestMessage("", "", "");

        listener.SendMessage(msg);

        Assert.That(state.IsTestRunning, Is.True);
        Assert.That(state.TestRunCount, Is.EqualTo(10));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(7));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(5));
        Assert.That(state.TestRunProgress, Is.EqualTo(0.5));
    }

    #endregion
}