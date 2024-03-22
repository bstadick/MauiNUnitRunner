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
    // TODO - NUnitTestListenerTest tests
    // TODO - Fix test concurrency issues

    [Test]
    public void Foo()
    {
        Assert.Fail();
    }

    #region Private Members

    private const int c_WaitTimeout = 500;

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

        IList<string> messages = new List<string>();
        listener.WriteOutput += message => messages.Add(message);

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

    #region Tests for TestStarted

    [Test]
    public void TestTestStartedWithTestOrTestIdNullWritesMessageAndDoesNotLogTest([Values] bool isTestNull)
    {
        ITest test1 = isTestNull ? null : new TestStub();
        ITest test2 = new TestStub { FullName = "suite-name2"};

        NUnitTestListener listener = new NUnitTestListener();

        IList<string> messages = new List<string>();
        listener.WriteOutput += message => messages.Add(message);

        listener.TestStarted(test1);
        listener.TestStarted(test2);

        SpinWait.SpinUntil(() => messages.Count >= 2, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests, Is.Empty);

        Assert.That(messages.Count, Is.EqualTo(2));
        Assert.That(messages, Does.Contain("Started "));
        Assert.That(messages, Does.Contain("Started suite-name2"));
    }

    [Test]
    public void TestTestStartedWithTestWritesMessageAndLogsTest()
    {
        ITest test1 = new TestSuite("suite-name1");
        ITest test2 = new TestSuite("suite-name2");
        ITest test3 = new TestStub { Id = "id3"};

        NUnitTestListener listener = new NUnitTestListener();

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

        // Test adding copy of same test isn't added to list but message logged
        listener.TestStarted(test2);
        listener.TestStarted(test1);

        SpinWait.SpinUntil(() => messages.Count >= 5, c_WaitTimeout);

        Assert.That(listener.Tests, Is.Not.Null);
        Assert.That(listener.Tests.Count, Is.EqualTo(3));

        Assert.That(listener.Tests.ContainsKey(test1.Id), Is.True);
        NUnitTestArtifact testArtifact1 = listener.Tests[test1.Id];
        Assert.That(testArtifact1, Is.Not.Null);
        Assert.That(testArtifact1.Test, Is.SameAs(test1));
        Assert.That(testArtifact1.Outputs, Is.Not.Null);
        Assert.That(testArtifact1.Outputs, Is.Empty);
        Assert.That(testArtifact1.Messages, Is.Not.Null);
        Assert.That(testArtifact1.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test2.Id), Is.True);
        NUnitTestArtifact testArtifact2 = listener.Tests[test2.Id];
        Assert.That(testArtifact2, Is.Not.Null);
        Assert.That(testArtifact2.Test, Is.SameAs(test2));
        Assert.That(testArtifact2.Outputs, Is.Not.Null);
        Assert.That(testArtifact2.Outputs, Is.Empty);
        Assert.That(testArtifact2.Messages, Is.Not.Null);
        Assert.That(testArtifact2.Messages, Is.Empty);

        Assert.That(listener.Tests.ContainsKey(test3.Id), Is.True);
        NUnitTestArtifact testArtifact3 = listener.Tests[test3.Id];
        Assert.That(testArtifact3, Is.Not.Null);
        Assert.That(testArtifact3.Test, Is.SameAs(test3));
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

    #region Nested Class: NUnitTestListenerForTest

    /// <summary>
    ///     Extends NUnitTestListener for use with tests.
    /// </summary>
    public class NUnitTestListenerForTest : NUnitTestListener
    {
        /// <summary>
        ///     Write message to <see cref="NUnitTestListener.WriteOutput" /> event handler.
        /// </summary>
        /// <param name="msg">A composite format string.</param>
        /// <param name="args">An object array that contains zero or more objects to format.</param>
        public async void InvokeWriteMessage(string msg, params object[] args)
        {
            //await WriteMessage(msg, args).ConfigureAwait(false);
        }
    }

    #endregion
}