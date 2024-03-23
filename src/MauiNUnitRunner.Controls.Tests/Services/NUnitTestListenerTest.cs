// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Services;

namespace MauiNUnitRunner.Controls.Tests.Services;

[TestFixture]
public class NUnitTestListenerTest
{
    #region Private Members

    /// <summary>
    ///     The wait timeout (in milliseconds) to wait for asynchronous events to complete.
    /// </summary>
    private const int c_WaitTimeout = 500;

    /// <summary>
    ///     Holds the logger instance to dispose at the end of the test.
    /// </summary>
    private NUnitTestListener v_ListenerInstance;

    #endregion

    #region Test Setup/Teardown

    [TearDown]
    public void TestTearDown()
    {
        if (v_ListenerInstance != null)
        {
            v_ListenerInstance.Dispose();
            v_ListenerInstance = null;
        }
    }

    #endregion

    #region Tests for Constructors

    [Test]
    public void TestConstructorStartsLogListener()
    {
        NUnitTestListenerForTest listener = new NUnitTestListenerForTest();
        v_ListenerInstance = listener;

        Thread thread = listener.GetLoggingThreadTest();
        Assert.That(thread, Is.Not.Null);
        Assert.That(thread.Name, Is.EqualTo("NUnitTestListenerLogListener"));
        Assert.That(thread.IsAlive, Is.True);
    }

    #endregion

    #region Tests for Tests Property

    [Test]
    public void TestTestsPropertyReturnsTestsList()
    {
        ITest test1 = new TestSuite("suite-name1");
        ITest test2 = new TestSuite("suite-name2");
        IDictionary<string, NUnitTestArtifact> expectedMessages = new Dictionary<string, NUnitTestArtifact>
            {{"id1", new NUnitTestArtifact(test1)}, {"id2", new NUnitTestArtifact(test2)}};

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IDictionary<string, NUnitTestArtifact> tests = listener.Tests;

        Assert.That(tests, Is.Not.Null);
        Assert.That(tests, Is.Empty);

        tests.Add(expectedMessages.Single(p => p.Key == "id1"));
        tests.Add(expectedMessages.Single(p => p.Key == "id2"));

        Assert.That(listener.Tests, Is.EqualTo(expectedMessages));
    }

    #endregion

    #region Tests for WriteOutput Event

    [Test]
    public void TestWriteOutputEventInvokedOnWriteMessageWhenEventNullReturnsNoMessages()
    {
        NUnitTestListenerForTest listener = new NUnitTestListenerForTest();
        v_ListenerInstance = listener;

        listener.InvokeWriteMessage("First message arg: {0}", "arg1");
        listener.InvokeWriteMessage("Second message arg");
        listener.InvokeWriteMessage("Third message arg: {0} {1}", "arg3", "arg3.5");

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);
    }

    [Test]
    public void TestWriteOutputEventInvokedOnWriteMessageReturnsMessage()
    {
        NUnitTestListenerForTest listener = new NUnitTestListenerForTest();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        listener.InvokeWriteMessage("First message arg: {0}", "arg1");
        listener.InvokeWriteMessage("Second message arg");
        listener.InvokeWriteMessage("Third message arg: {0} {1}", "arg3", "arg3.5");

        SpinWait.SpinUntil(() => messages.Count >= 3, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(3));
        Assert.That(messages, Does.Contain("First message arg: arg1"));
        Assert.That(messages, Does.Contain("Second message arg"));
        Assert.That(messages, Does.Contain("Third message arg: arg3 arg3.5"));
    }

    [Test]
    public void TestWriteOutputEventInvokedOnWriteMessageWithExceptionReturnsMessage()
    {
        NUnitTestListenerForTest listener = new NUnitTestListenerForTest();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += message =>
        {
            if (message == "Error msg")
            {
                throw new Exception("Event exception");
            }

            messages.Add(message);
        };

        listener.InvokeWriteMessage("First message arg: {0}", "arg1");
        listener.InvokeWriteMessage("Second message arg");
        listener.InvokeWriteMessage("Error msg");
        listener.InvokeWriteMessage(null);
        listener.InvokeWriteMessage("null", null);
        listener.InvokeWriteMessage("invalid format {1}", "arg");
        listener.InvokeWriteMessage("Third message arg: {0} {1}", "arg3", "arg3.5");

        SpinWait.SpinUntil(() => messages.Count >= 3, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(3));
        Assert.That(messages, Does.Contain("First message arg: arg1"));
        Assert.That(messages, Does.Contain("Second message arg"));
        Assert.That(messages, Does.Contain("Third message arg: arg3 arg3.5"));
    }

    #endregion

    #region Tests for Dispose

    [Test]
    public void TestDisposeStopsListener()
    {
        NUnitTestListenerForTest listener = new NUnitTestListenerForTest();
        v_ListenerInstance = listener;

        Thread thread = listener.GetLoggingThreadTest();
        Assert.That(thread, Is.Not.Null);
        Assert.That(thread.Name, Is.EqualTo("NUnitTestListenerLogListener"));
        Assert.That(thread.IsAlive, Is.True);
        Assert.That(thread.ThreadState, Is.EqualTo(ThreadState.Running));

        listener.Dispose();
        v_ListenerInstance = null;

        thread = listener.GetLoggingThreadTest();
        Assert.That(thread, Is.Not.Null);
        Assert.That(thread.IsAlive, Is.False);
        Assert.That(thread.ThreadState, Is.EqualTo(ThreadState.Stopped));

        // Calling dispose a second time does not throw exception
        listener.Dispose();

        thread = listener.GetLoggingThreadTest();
        Assert.That(thread, Is.Not.Null);
        Assert.That(thread.IsAlive, Is.False);
        Assert.That(thread.ThreadState, Is.EqualTo(ThreadState.Stopped));
    }

    #endregion

    #region Tests for TestStarted

    [Test]
    public void TestTestStartedWithTestNullOrTestIdNullOrEmptyWritesMessageAndDoesNotLogTest([Values] bool isTestNull, [Values] bool isTestIdNull)
    {
        ITest test1 = isTestNull ? null : new TestStub();
        // ReSharper disable once AssignNullToNotNullAttribute
        ITest test2 = new TestStub { FullName = "suite-name2", Id = isTestIdNull ? null : string.Empty };
        int expectedMsgCount = isTestNull ? 1 : 2;

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        // ReSharper disable once AssignNullToNotNullAttribute
        listener.TestStarted(test1);
        listener.TestStarted(test2);

        SpinWait.SpinUntil(() => messages.Count >= expectedMsgCount, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(expectedMsgCount));
        if (expectedMsgCount >= 2)
        {
            Assert.That(messages, Does.Contain("Started "));
        }

        Assert.That(messages, Does.Contain("Started suite-name2"));
    }

    [Test]
    public void TestTestStartedWithTestWritesMessageAndLogsTest()
    {
        ITest test1 = new TestSuite("suite-name1");
        ITest test2 = new TestSuite("suite-name2");
        ITest test3 = new TestStub { Id = "id3"};

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += message => messages.Add(message);

        listener.TestStarted(test1);
        listener.TestStarted(test2);
        listener.TestStarted(test3);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));
        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal1 = listener.Tests[test1.Id];
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal2 = listener.Tests[test2.Id];

        // Test adding copy of same test isn't added to list but message logged
        listener.TestStarted(test2);
        listener.TestStarted(test1);

        SpinWait.SpinUntil(() => messages.Count >= 5, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifact1 = listener.Tests[test1.Id];
        Assert.That(testArtifact1, Is.SameAs(testArtifactOriginal1));
        Assert.That(testArtifact1, Is.Not.Null);
        Assert.That(testArtifact1.Test, Is.Not.Null);
        Assert.That(testArtifact1.Test.Result, Is.Null);
        Assert.That(testArtifact1.Test.Test, Is.SameAs(test1));
        Assert.That(testArtifact1.Outputs, Is.Not.Null);
        Assert.That(testArtifact1.Outputs, Is.Empty);
        Assert.That(testArtifact1.Messages, Is.Not.Null);
        Assert.That(testArtifact1.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifact2 = listener.Tests[test2.Id];
        Assert.That(testArtifact2, Is.SameAs(testArtifactOriginal2));
        Assert.That(testArtifact2, Is.Not.Null);
        Assert.That(testArtifact2.Test, Is.Not.Null);
        Assert.That(testArtifact2.Test.Result, Is.Null);
        Assert.That(testArtifact2.Test.Test, Is.SameAs(test2));
        Assert.That(testArtifact2.Outputs, Is.Not.Null);
        Assert.That(testArtifact2.Outputs, Is.Empty);
        Assert.That(testArtifact2.Messages, Is.Not.Null);
        Assert.That(testArtifact2.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);
        NUnitTestArtifact testArtifact3 = listener.Tests[test3.Id];
        Assert.That(testArtifact3, Is.Not.Null);
        Assert.That(testArtifact3.Test, Is.Not.Null);
        Assert.That(testArtifact3.Test.Result, Is.Null);
        Assert.That(testArtifact3.Test.Test, Is.SameAs(test3));
        Assert.That(testArtifact3.Outputs, Is.Not.Null);
        Assert.That(testArtifact3.Outputs, Is.Empty);
        Assert.That(testArtifact3.Messages, Is.Not.Null);
        Assert.That(testArtifact3.Messages, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(5));
        Assert.That(messages.Count(m => m == "Started suite-name1"), Is.EqualTo(2));
        Assert.That(messages.Count(m => m == "Started suite-name2"), Is.EqualTo(2));
        Assert.That(messages.Count(m => m == "Started "), Is.EqualTo(1));
    }

    #endregion

    #region Tests for TestFinished

    [Test]
    public void TestTestFinishedWithTestOrTestResultNullOrTestIdNullOrEmptyWritesMessageAndDoesNotLogTest([Values] bool isTestResultNull,
        [Values] bool isTestNull, [Values] bool isTestIdNull)
    {
        ITest test1 = isTestNull ? null : new TestStub();
        // ReSharper disable once AssignNullToNotNullAttribute
        ITestResult testResult1 = isTestResultNull ? null : new TestResultStub { Test = test1 };
        // ReSharper disable once AssignNullToNotNullAttribute
        ITest test2 = new TestStub { FullName = "suite-name2", Id = isTestIdNull ? null : string.Empty };
        ITestResult testResult2 = new TestResultStub { Test = test2 };
        int expectedMsgCount = isTestNull || isTestResultNull ? 1 : 2;

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        // ReSharper disable once AssignNullToNotNullAttribute
        listener.TestFinished(testResult1);
        listener.TestFinished(testResult2);

        SpinWait.SpinUntil(() => messages.Count >= expectedMsgCount, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(expectedMsgCount));
        if (expectedMsgCount >= 2)
        {
            Assert.That(messages, Does.Contain("Finished : "));
        }

        Assert.That(messages, Does.Contain("Finished suite-name2: "));
    }

    [Test]
    public void TestTestFinishedWithTestNotStartedWritesMessageAndLogsTest()
    {
        ITest test1 = new TestStub { FullName = "suite-name1", Id = "id1" };
        ITestResult testResult1 = new TestResultStub { Test = test1 };
        ITest test2 = new TestStub { FullName = "suite-name2", Id = "id2" };
        ITestResult testResult2 = new TestResultStub { Test = test2 };

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        listener.Tests.Add(test1.Id, null);

        // ReSharper disable once AssignNullToNotNullAttribute
        listener.TestFinished(testResult1);
        listener.TestFinished(testResult2);

        SpinWait.SpinUntil(() => messages.Count >= 2, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(2));
        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        Assert.That(listener.Tests[test1.Id], Is.Not.Null);
        Assert.That(listener.Tests[test1.Id].Test, Is.Not.Null);
        Assert.That(listener.Tests[test1.Id].Test.Test, Is.EqualTo(test1));
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        Assert.That(listener.Tests[test2.Id], Is.Not.Null);
        Assert.That(listener.Tests[test2.Id].Test, Is.Not.Null);
        Assert.That(listener.Tests[test2.Id].Test.Test, Is.EqualTo(test2));

        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages, Does.Contain("Finished suite-name1: "));
        Assert.That(messages, Does.Contain("Finished suite-name2: "));
    }

    [Test]
    public void TestTestFinishedWithTestWritesMessageAndLogsTest()
    {
        ITest test1 = new TestSuite("suite-name1");
        ITestResult testResult1 = new TestResultStub { Test = test1, ResultState = ResultState.Success };
        ITest test2 = new TestSuite("suite-name2");
        ITestResult testResult2 = new TestResultStub { Test = test2 };
        ITest test3 = new TestStub { Id = "id3" };
        ITestResult testResult3 = new TestResultStub { Test = test3, ResultState = ResultState.Failure };

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += message => messages.Add(message);

        listener.Tests.Add(test1.Id, new NUnitTestArtifact(test1));
        listener.Tests.Add(test2.Id, new NUnitTestArtifact(test2));
        listener.Tests.Add(test3.Id, new NUnitTestArtifact(test3));

        listener.TestFinished(testResult1);
        listener.TestFinished(testResult2);
        listener.TestFinished(testResult3);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));
        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal1 = listener.Tests[test1.Id];
        INUnitTestResult testResultOriginal1 = testArtifactOriginal1.Test.Result;
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal2 = listener.Tests[test2.Id];
        INUnitTestResult testResultOriginal2 = testArtifactOriginal2.Test.Result;

        // Test adding copy of same test result overwrites existing result
        listener.TestFinished(testResult2);
        listener.TestFinished(testResult1);

        SpinWait.SpinUntil(() => messages.Count >= 5, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifact1 = listener.Tests[test1.Id];
        Assert.That(testArtifact1, Is.SameAs(testArtifactOriginal1));
        Assert.That(testArtifact1, Is.Not.Null);
        Assert.That(testArtifact1.Test, Is.Not.Null);
        Assert.That(testArtifact1.Test.Result, Is.Not.Null);
        Assert.That(testArtifact1.Test.Result, Is.Not.SameAs(testResultOriginal1));
        Assert.That(testArtifact1.Test.Result, Is.EqualTo(testResultOriginal1));
        Assert.That(testArtifact1.Test.Test, Is.SameAs(test1));
        Assert.That(testArtifact1.Outputs, Is.Not.Null);
        Assert.That(testArtifact1.Outputs, Is.Empty);
        Assert.That(testArtifact1.Messages, Is.Not.Null);
        Assert.That(testArtifact1.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifact2 = listener.Tests[test2.Id];
        Assert.That(testArtifact2, Is.SameAs(testArtifactOriginal2));
        Assert.That(testArtifact2, Is.Not.Null);
        Assert.That(testArtifact2.Test, Is.Not.Null);
        Assert.That(testArtifact2.Test.Result, Is.Not.Null);
        Assert.That(testArtifact2.Test.Result, Is.Not.SameAs(testResultOriginal2));
        Assert.That(testArtifact2.Test.Result, Is.EqualTo(testResultOriginal2));
        Assert.That(testArtifact2.Test.Test, Is.SameAs(test2));
        Assert.That(testArtifact2.Outputs, Is.Not.Null);
        Assert.That(testArtifact2.Outputs, Is.Empty);
        Assert.That(testArtifact2.Messages, Is.Not.Null);
        Assert.That(testArtifact2.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);
        NUnitTestArtifact testArtifact3 = listener.Tests[test3.Id];
        Assert.That(testArtifact3, Is.Not.Null);
        Assert.That(testArtifact3.Test, Is.Not.Null);
        Assert.That(testArtifact3.Test.Result, Is.Not.Null);
        Assert.That(testArtifact3.Test.Result.Result, Is.Not.Null);
        Assert.That(testArtifact3.Test.Result.Result, Is.SameAs(testResult3));
        Assert.That(testArtifact3.Test.Test, Is.SameAs(test3));
        Assert.That(testArtifact3.Outputs, Is.Not.Null);
        Assert.That(testArtifact3.Outputs, Is.Empty);
        Assert.That(testArtifact3.Messages, Is.Not.Null);
        Assert.That(testArtifact3.Messages, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(5));
        Assert.That(messages.Count(m => m == "Finished suite-name1: Passed"), Is.EqualTo(2));
        Assert.That(messages.Count(m => m == "Finished suite-name2: "), Is.EqualTo(2));
        Assert.That(messages.Count(m => m == "Finished : Failed"), Is.EqualTo(1));
    }

    #endregion

    #region Tests for TestOutput

    [Test]
    public void TestTestOutputWithTestOutputNullOrTestIdNullOrEmptyWritesMessageAndDoesNotLogTest(
        [Values] bool isTestOutputNull, [Values] bool isTestIdNull, [Values] bool isTestNameNull)
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        ITest test1 = new TestStub { FullName = "suite-name1", Id = isTestIdNull ? null : string.Empty };
        TestOutput testMessage1 =
            isTestOutputNull ? null : new TestOutput("test 1", string.Empty, test1.Id, test1.FullName);
        // ReSharper disable twice AssignNullToNotNullAttribute
        ITest test2 = new TestStub { FullName = isTestNameNull ? null : string.Empty, Id = isTestIdNull ? null : string.Empty };
        TestOutput testMessage2 = new TestOutput("test 2", string.Empty, test2.Id, test2.FullName);

        int expectedMsgCount = isTestOutputNull ? 1 : 2;

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        // ReSharper disable once AssignNullToNotNullAttribute
        listener.TestOutput(testMessage1);
        listener.TestOutput(testMessage2);

        SpinWait.SpinUntil(() => messages.Count >= 1, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(expectedMsgCount));
        if (expectedMsgCount >= 2)
        {
            Assert.That(messages, Does.Contain("Output suite-name1: test 1"));
        }

        Assert.That(messages, Does.Contain("Output : test 2"));
    }

    [Test]
    public void TestTestOutputWithTestNotStartedWritesMessageAndLogsTest()
    {
        ITest test1 = new TestStub { FullName = "suite-name1", Id = "id1" };
        TestOutput testMessage1 = new TestOutput("test 1", string.Empty, test1.Id, test1.FullName);
        ITest test2 = new TestStub { FullName = "suite-name2", Id = "id2" };
        TestOutput testMessage2 = new TestOutput("test 2", string.Empty, test2.Id, test2.FullName);

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        listener.Tests.Add(test1.Id, null);

        // ReSharper disable once AssignNullToNotNullAttribute
        listener.TestOutput(testMessage1);
        listener.TestOutput(testMessage2);

        SpinWait.SpinUntil(() => messages.Count >= 2, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(1));
        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        Assert.That(listener.Tests[test1.Id], Is.Null);

        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages, Does.Contain("Output suite-name1: test 1"));
        Assert.That(messages, Does.Contain("Output suite-name2: test 2"));
    }

    [Test]
    public void TestTestOutputWithTestWritesMessageAndLogsTest([Values] bool isTestNameNull)
    {
        ITest test1 = new TestSuite("suite-name1");
        TestOutput testMessage1 = new TestOutput("test 1", string.Empty, test1.Id, test1.FullName);
        TestOutput testMessageDup1 = new TestOutput("test 1 duplicate", string.Empty, test1.Id, test1.FullName);
        ITest test2 = new TestSuite("suite-name2");
        TestOutput testMessage2 = new TestOutput("test 2", string.Empty, test2.Id, test2.FullName);
        TestOutput testMessageDup2 = new TestOutput("test 2 duplicate", string.Empty, test2.Id, test2.FullName);
        ITest test3 = new TestStub { Id = "id3" };
        TestOutput testMessage3 = new TestOutput("test 3", string.Empty, test3.Id, isTestNameNull ? null : string.Empty);

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += message => messages.Add(message);

        listener.Tests.Add(test1.Id, new NUnitTestArtifact(test1));
        listener.Tests.Add(test2.Id, new NUnitTestArtifact(test2));
        listener.Tests.Add(test3.Id, new NUnitTestArtifact(test3));

        listener.TestOutput(testMessage1);
        listener.TestOutput(testMessage2);
        listener.TestOutput(testMessage3);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));
        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal1 = listener.Tests[test1.Id];
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal2 = listener.Tests[test2.Id];

        // Test adding copy of same test output appends additional outputs
        listener.TestOutput(testMessageDup2);
        listener.TestOutput(testMessageDup1);

        SpinWait.SpinUntil(() => messages.Count >= 5, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifact1 = listener.Tests[test1.Id];
        Assert.That(testArtifact1, Is.SameAs(testArtifactOriginal1));
        Assert.That(testArtifact1, Is.Not.Null);
        Assert.That(testArtifact1.Test, Is.Not.Null);
        Assert.That(testArtifact1.Test.Result, Is.Null);
        Assert.That(testArtifact1.Test.Test, Is.SameAs(test1));
        Assert.That(testArtifact1.Outputs, Is.Not.Null);
        Assert.That(testArtifact1.Outputs.Count, Is.EqualTo(2));
        Assert.That(testArtifact1.Outputs, Is.EqualTo(new List<TestOutput> { testMessage1, testMessageDup1 }));
        Assert.That(testArtifact1.Messages, Is.Not.Null);
        Assert.That(testArtifact1.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifact2 = listener.Tests[test2.Id];
        Assert.That(testArtifact2, Is.SameAs(testArtifactOriginal2));
        Assert.That(testArtifact2, Is.Not.Null);
        Assert.That(testArtifact2.Test, Is.Not.Null);
        Assert.That(testArtifact2.Test.Result, Is.Null);
        Assert.That(testArtifact2.Test.Test, Is.SameAs(test2));
        Assert.That(testArtifact2.Outputs, Is.Not.Null);
        Assert.That(testArtifact2.Outputs.Count, Is.EqualTo(2));
        Assert.That(testArtifact2.Outputs, Is.EqualTo(new List<TestOutput> { testMessage2, testMessageDup2 }));
        Assert.That(testArtifact2.Messages, Is.Not.Null);
        Assert.That(testArtifact2.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);
        NUnitTestArtifact testArtifact3 = listener.Tests[test3.Id];
        Assert.That(testArtifact3, Is.Not.Null);
        Assert.That(testArtifact3.Test, Is.Not.Null);
        Assert.That(testArtifact3.Test.Result, Is.Null);
        Assert.That(testArtifact3.Test.Test, Is.SameAs(test3));
        Assert.That(testArtifact3.Outputs, Is.Not.Null);
        Assert.That(testArtifact3.Outputs.Count, Is.EqualTo(1));
        Assert.That(testArtifact3.Outputs, Is.EqualTo(new List<TestOutput> { testMessage3 }));
        Assert.That(testArtifact3.Messages, Is.Not.Null);
        Assert.That(testArtifact3.Messages, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(5));
        Assert.That(messages, Does.Contain("Output suite-name1: test 1"));
        Assert.That(messages, Does.Contain("Output suite-name2: test 2"));
        Assert.That(messages, Does.Contain("Output id3: test 3"));
        Assert.That(messages, Does.Contain("Output suite-name1: test 1 duplicate"));
        Assert.That(messages, Does.Contain("Output suite-name2: test 2 duplicate"));
    }

    #endregion

    #region Tests for SendMessage

    [Test]
    public void TestSendMessageWithTestMessageNullOrTestIdNullOrEmptyWritesMessageAndDoesNotLogTest(
        [Values] bool isTestMessageNull, [Values] bool isTestIdNull)
    {
        // ReSharper disable once AssignNullToNotNullAttribute
        ITest test1 = new TestStub { FullName = "suite-name1", Id = isTestIdNull ? null : string.Empty };
        TestMessage testOutput1 =
            isTestMessageNull ? null : new TestMessage("dest 1", "test 1", test1.Id);
        int expectedMsgCount = isTestMessageNull ? 0 : 1;

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        // ReSharper disable once AssignNullToNotNullAttribute
        listener.SendMessage(testOutput1);

        SpinWait.SpinUntil(() => messages.Count >= 1, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(expectedMsgCount));
        if (expectedMsgCount >= 1)
        {
            Assert.That(messages, Does.Contain("Message [dest 1]: test 1"));
        }
    }

    [Test]
    public void TestSendMessageWithTestNotStartedWritesMessageAndLogsTest()
    {
        ITest test1 = new TestStub { FullName = "suite-name1", Id = "id1" };
        TestMessage testOutput1 = new TestMessage("dest 1", "test 1", test1.Id);
        ITest test2 = new TestStub { FullName = "suite-name2", Id = "id2" };
        TestMessage testOutput2 = new TestMessage("dest 2", "test 2", test2.Id);

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += messages.Add;

        listener.Tests.Add(test1.Id, null);

        // ReSharper disable once AssignNullToNotNullAttribute
        listener.SendMessage(testOutput1);
        listener.SendMessage(testOutput2);

        SpinWait.SpinUntil(() => messages.Count >= 2, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(1));
        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        Assert.That(listener.Tests[test1.Id], Is.Null);

        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages, Does.Contain("Message id1[dest 1]: test 1"));
        Assert.That(messages, Does.Contain("Message id2[dest 2]: test 2"));
    }

    [Test]
    public void TestSendMessageWithTestWritesMessageAndLogsTest([Values] bool isTestNull, [Values] bool isTestNameNull)
    {
        ITest test1 = new TestSuite("suite-name1");
        TestMessage testOutput1 = new TestMessage("dest 1", "test 1", test1.Id);
        TestMessage testOutputDup1 = new TestMessage("dest 1", "test 1 duplicate", test1.Id);
        ITest test2 = new TestSuite("suite-name2");
        TestMessage testOutput2 = new TestMessage("dest 2", "test 2", test2.Id);
        TestMessage testOutputDup2 = new TestMessage("dest 2", "test 2 duplicate", test2.Id);
        // ReSharper disable once AssignNullToNotNullAttribute
        ITest test3 = new TestStub { Id = "id3", FullName = isTestNameNull ? null : string.Empty };
        ITest expectedTest3 = isTestNull ? null : test3;
        // ReSharper disable once AssignNullToNotNullAttribute
        TestMessage testOutput3 = new TestMessage(string.Empty, "test 3", test3.Id);

        NUnitTestListener listener = new NUnitTestListener();
        v_ListenerInstance = listener;

        IList<string> messages = new List<string>();
        listener.WriteOutput += message => messages.Add(message);

        listener.Tests.Add(test1.Id, new NUnitTestArtifact(test1));
        listener.Tests.Add(test2.Id, new NUnitTestArtifact(test2));
        listener.Tests.Add(test3.Id, new NUnitTestArtifact(expectedTest3));

        listener.SendMessage(testOutput1);
        listener.SendMessage(testOutput2);
        listener.SendMessage(testOutput3);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));
        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal1 = listener.Tests[test1.Id];
        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifactOriginal2 = listener.Tests[test2.Id];

        // Test adding copy of same test message appends additional messages
        listener.SendMessage(testOutputDup2);
        listener.SendMessage(testOutputDup1);

        SpinWait.SpinUntil(() => messages.Count >= 5, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifact1 = listener.Tests[test1.Id];
        Assert.That(testArtifact1, Is.SameAs(testArtifactOriginal1));
        Assert.That(testArtifact1, Is.Not.Null);
        Assert.That(testArtifact1.Test, Is.Not.Null);
        Assert.That(testArtifact1.Test.Result, Is.Null);
        Assert.That(testArtifact1.Test.Test, Is.SameAs(test1));
        Assert.That(testArtifact1.Outputs, Is.Not.Null);
        Assert.That(testArtifact1.Outputs, Is.Empty);
        Assert.That(testArtifact1.Messages, Is.Not.Null);
        Assert.That(testArtifact1.Messages.Count, Is.EqualTo(2));
        Assert.That(testArtifact1.Messages, Is.EqualTo(new List<TestMessage> { testOutput1, testOutputDup1 }));

        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifact2 = listener.Tests[test2.Id];
        Assert.That(testArtifact2, Is.SameAs(testArtifactOriginal2));
        Assert.That(testArtifact2, Is.Not.Null);
        Assert.That(testArtifact2.Test, Is.Not.Null);
        Assert.That(testArtifact2.Test.Result, Is.Null);
        Assert.That(testArtifact2.Test.Test, Is.SameAs(test2));
        Assert.That(testArtifact2.Outputs, Is.Not.Null);
        Assert.That(testArtifact2.Outputs, Is.Empty);
        Assert.That(testArtifact2.Messages, Is.Not.Null);
        Assert.That(testArtifact2.Messages.Count, Is.EqualTo(2));
        Assert.That(testArtifact2.Messages, Is.EqualTo(new List<TestMessage> { testOutput2, testOutputDup2 }));

        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);
        NUnitTestArtifact testArtifact3 = listener.Tests[test3.Id];
        Assert.That(testArtifact3, Is.Not.Null);
        Assert.That(testArtifact3.Test, Is.Not.Null);
        Assert.That(testArtifact3.Test.Result, Is.Null);
        Assert.That(testArtifact3.Test.Test, Is.SameAs(expectedTest3));
        Assert.That(testArtifact3.Outputs, Is.Not.Null);
        Assert.That(testArtifact3.Outputs, Is.Empty);
        Assert.That(testArtifact3.Messages, Is.Not.Null);
        Assert.That(testArtifact3.Messages.Count, Is.EqualTo(1));
        Assert.That(testArtifact3.Messages, Is.EqualTo(new List<TestMessage> { testOutput3 }));

        Assert.That(messages.Count, Is.EqualTo(5));
        Assert.That(messages, Does.Contain("Message suite-name1[dest 1]: test 1"));
        Assert.That(messages, Does.Contain("Message suite-name2[dest 2]: test 2"));
        Assert.That(messages, Does.Contain("Message id3[]: test 3"));
        Assert.That(messages, Does.Contain("Message suite-name1[dest 1]: test 1 duplicate"));
        Assert.That(messages, Does.Contain("Message suite-name2[dest 2]: test 2 duplicate"));
    }

    #endregion

    #region Nested Class: NUnitTestListenerForTest

    /// <summary>
    ///     Extends NUnitTestListener for use with tests.
    /// </summary>
    private class NUnitTestListenerForTest : NUnitTestListener
    {
        /// <summary>
        ///     Write message to <see cref="NUnitTestListener.WriteOutput" /> event handler.
        /// </summary>
        /// <param name="msg">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public void InvokeWriteMessage(string msg, params object[] args)
        {
            WriteMessage(msg, args);
        }

        /// <summary>
        ///     Gets the underlying logging thread.
        /// </summary>
        /// <returns>The underlying logging thread.</returns>
        public Thread GetLoggingThreadTest()
        {
            return GetLoggingThread();
        }
    }

    #endregion
}