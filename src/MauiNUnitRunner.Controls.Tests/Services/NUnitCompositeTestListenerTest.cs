// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Services;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Tests.Services;

[TestFixture]
public class NUnitCompositeTestListenerTest
{
    #region Tests for TestListeners Property

    [Test]
    public void TestTestListenersProperty()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        Assert.That(listener.TestListeners, Is.Not.Null.And.Empty);

        ITestListener child1 = new TestListenerForTest();
        ITestListener child2 = new TestListenerForTest();

        listener.AddTestListener(child1);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child1));

        listener.AddTestListener(null);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child1));

        listener.AddTestListener(child2);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(2));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));
    }

    #endregion

    #region Tests for TestStarted

    [Test]
    public void TestTestStarted()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(2));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));

        ITest test1 = new TestStub();
        ITest test2 = new TestStub();

        listener.TestStarted(test1);

        Assert.That(child1.TestStartedValue, Is.SameAs(test1));
        Assert.That(child2.TestStartedValue, Is.SameAs(test1));

        listener.TestStarted(test2);

        Assert.That(child1.TestStartedValue, Is.SameAs(test2));
        Assert.That(child2.TestStartedValue, Is.SameAs(test2));
    }

    [Test]
    public void TestTestStartedWithNoTestListenersAddedDoesNothing()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        // ReSharper disable once AssignNullToNotNullAttribute
        Assert.DoesNotThrow(() => listener.TestStarted(null));
        Assert.DoesNotThrow(() => listener.TestStarted(new TestStub()));
    }

    [Test]
    public void TestTestStartedWhenTestListenerThrowsExceptionIgnoresException()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();
        TestListenerForTest child3 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);
        listener.AddTestListener(child3);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(3));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));
        Assert.That(listener.TestListeners, Does.Contain(child3));

        ITest test1 = new TestStub();
        ITest test2 = new TestStub();

        child2.ThrowException = true;
        listener.TestStarted(test1);

        Assert.That(child1.TestStartedValue, Is.SameAs(test1));
        Assert.That(child2.TestStartedValue, Is.Null);
        Assert.That(child3.TestStartedValue, Is.SameAs(test1));

        child2.ThrowException = false;
        listener.TestStarted(test2);

        Assert.That(child1.TestStartedValue, Is.SameAs(test2));
        Assert.That(child2.TestStartedValue, Is.SameAs(test2));
        Assert.That(child3.TestStartedValue, Is.SameAs(test2));
    }

    #endregion

    #region Tests for TestFinished

    [Test]
    public void TestTestFinished()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(2));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));

        ITestResult test1 = new TestResultStub();
        ITestResult test2 = new TestResultStub();

        listener.TestFinished(test1);

        Assert.That(child1.TestFinishedValue, Is.SameAs(test1));
        Assert.That(child2.TestFinishedValue, Is.SameAs(test1));

        listener.TestFinished(test2);

        Assert.That(child1.TestFinishedValue, Is.SameAs(test2));
        Assert.That(child2.TestFinishedValue, Is.SameAs(test2));
    }

    [Test]
    public void TestTestFinishedWithNoTestListenersAddedDoesNothing()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        // ReSharper disable once AssignNullToNotNullAttribute
        Assert.DoesNotThrow(() => listener.TestFinished(null));
        Assert.DoesNotThrow(() => listener.TestFinished(new TestResultStub()));
    }

    [Test]
    public void TestTestFinishedWhenTestListenerThrowsExceptionIgnoresException()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();
        TestListenerForTest child3 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);
        listener.AddTestListener(child3);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(3));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));
        Assert.That(listener.TestListeners, Does.Contain(child3));

        ITestResult test1 = new TestResultStub();
        ITestResult test2 = new TestResultStub();

        child2.ThrowException = true;
        listener.TestFinished(test1);

        Assert.That(child1.TestFinishedValue, Is.SameAs(test1));
        Assert.That(child2.TestFinishedValue, Is.Null);
        Assert.That(child3.TestFinishedValue, Is.SameAs(test1));

        child2.ThrowException = false;
        listener.TestFinished(test2);

        Assert.That(child1.TestFinishedValue, Is.SameAs(test2));
        Assert.That(child2.TestFinishedValue, Is.SameAs(test2));
        Assert.That(child3.TestFinishedValue, Is.SameAs(test2));
    }

    #endregion

    #region Tests for TestOutput

    [Test]
    public void TestTestOutput()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(2));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));

        TestOutput test1 = new TestOutput("", "", "", "");
        TestOutput test2 = new TestOutput("", "", "", "");

        listener.TestOutput(test1);

        Assert.That(child1.TestOutputValue, Is.SameAs(test1));
        Assert.That(child2.TestOutputValue, Is.SameAs(test1));

        listener.TestOutput(test2);

        Assert.That(child1.TestOutputValue, Is.SameAs(test2));
        Assert.That(child2.TestOutputValue, Is.SameAs(test2));
    }

    [Test]
    public void TestTestOutputWithNoTestListenersAddedDoesNothing()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        // ReSharper disable once AssignNullToNotNullAttribute
        Assert.DoesNotThrow(() => listener.TestOutput(null));
        Assert.DoesNotThrow(() => listener.TestOutput(new TestOutput("", "", "", "")));
    }

    [Test]
    public void TestTestOutputWhenTestListenerThrowsExceptionIgnoresException()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();
        TestListenerForTest child3 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);
        listener.AddTestListener(child3);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(3));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));
        Assert.That(listener.TestListeners, Does.Contain(child3));

        TestOutput test1 = new TestOutput("", "", "", "");
        TestOutput test2 = new TestOutput("", "", "", "");

        child2.ThrowException = true;
        listener.TestOutput(test1);

        Assert.That(child1.TestOutputValue, Is.SameAs(test1));
        Assert.That(child2.TestOutputValue, Is.Null);
        Assert.That(child3.TestOutputValue, Is.SameAs(test1));

        child2.ThrowException = false;
        listener.TestOutput(test2);

        Assert.That(child1.TestOutputValue, Is.SameAs(test2));
        Assert.That(child2.TestOutputValue, Is.SameAs(test2));
        Assert.That(child3.TestOutputValue, Is.SameAs(test2));
    }

    #endregion

    #region Tests for SendMessage

    [Test]
    public void TestSendMessage()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(2));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));

        TestMessage test1 = new TestMessage("", "", "");
        TestMessage test2 = new TestMessage("", "", "");

        listener.SendMessage(test1);

        Assert.That(child1.SendMessageValue, Is.SameAs(test1));
        Assert.That(child2.SendMessageValue, Is.SameAs(test1));

        listener.SendMessage(test2);

        Assert.That(child1.SendMessageValue, Is.SameAs(test2));
        Assert.That(child2.SendMessageValue, Is.SameAs(test2));
    }

    [Test]
    public void TestSendMessageWithNoTestListenersAddedDoesNothing()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        // ReSharper disable once AssignNullToNotNullAttribute
        Assert.DoesNotThrow(() => listener.SendMessage(null));
        Assert.DoesNotThrow(() => listener.SendMessage(new TestMessage("", "", "")));
    }

    [Test]
    public void TestSendMessageWhenTestListenerThrowsExceptionIgnoresException()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        TestListenerForTest child1 = new TestListenerForTest();
        TestListenerForTest child2 = new TestListenerForTest();
        TestListenerForTest child3 = new TestListenerForTest();

        listener.AddTestListener(child1);
        listener.AddTestListener(child2);
        listener.AddTestListener(child3);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(3));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));
        Assert.That(listener.TestListeners, Does.Contain(child3));

        TestMessage test1 = new TestMessage("", "", "");
        TestMessage test2 = new TestMessage("", "", "");

        child2.ThrowException = true;
        listener.SendMessage(test1);

        Assert.That(child1.SendMessageValue, Is.SameAs(test1));
        Assert.That(child2.SendMessageValue, Is.Null);
        Assert.That(child3.SendMessageValue, Is.SameAs(test1));

        child2.ThrowException = false;
        listener.SendMessage(test2);

        Assert.That(child1.SendMessageValue, Is.SameAs(test2));
        Assert.That(child2.SendMessageValue, Is.SameAs(test2));
        Assert.That(child3.SendMessageValue, Is.SameAs(test2));
    }

    #endregion

    #region Tests for AddTestListener

    [Test]
    public void TestAddTestListener()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        Assert.That(listener.TestListeners, Is.Not.Null.And.Empty);

        ITestListener child1 = new TestListenerForTest();
        ITestListener child2 = new TestListenerForTest();

        listener.AddTestListener(child1);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child1));

        listener.AddTestListener(null);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child1));

        listener.AddTestListener(child2);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(2));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));
    }

    #endregion

    #region Tests for RemoveTestListener

    [Test]
    public void TestRemoveTestListener()
    {
        NUnitCompositeTestListener listener = new NUnitCompositeTestListener();

        Assert.That(listener.TestListeners, Is.Not.Null.And.Empty);

        ITestListener child1 = new TestListenerForTest();
        ITestListener child2 = new TestListenerForTest();

        listener.AddTestListener(child1);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child1));

        listener.AddTestListener(null);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child1));

        listener.AddTestListener(child2);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(2));
        Assert.That(listener.TestListeners, Does.Contain(child1));
        Assert.That(listener.TestListeners, Does.Contain(child2));

        listener.RemoveTestListener(child1);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child2));

        listener.RemoveTestListener(null);

        Assert.That(listener.TestListeners, Has.Count.EqualTo(1));
        Assert.That(listener.TestListeners, Does.Contain(child2));

        listener.RemoveTestListener(child2);

        Assert.That(listener.TestListeners, Is.Empty);
    }

    #endregion

    #region Nested Class: TestListenerForTest

    /// <summary>
    ///     Implements ITestListener for use with tests.
    /// </summary>
    private class TestListenerForTest : ITestListener
    {
        #region Members for Test

        /// <summary>
        ///     Gets or sets if to throw an exception.
        /// </summary>
        public bool ThrowException { get; set; }

        /// <summary>
        ///     Gets the TestStarted value.
        /// </summary>
        public ITest TestStartedValue { get; private set; }

        /// <summary>
        ///     Gets the TestFinished value.
        /// </summary>
        public ITestResult TestFinishedValue { get; private set; }

        /// <summary>
        ///     Gets the TestOutput value.
        /// </summary>
        public TestOutput TestOutputValue { get; private set; }

        /// <summary>
        ///     Gets the SendMessage value.
        /// </summary>
        public TestMessage SendMessageValue { get; private set; }

        #endregion

        #region Implementation of ITestListener

        /// <inheritdoc />
        public void TestStarted(ITest test)
        {
            if (ThrowException)
            {
                throw new InvalidOperationException("TestStarted exception.");
            }

            TestStartedValue = test;
        }

        /// <inheritdoc />
        public void TestFinished(ITestResult result)
        {
            if (ThrowException)
            {
                throw new InvalidOperationException("TestFinished exception.");
            }

            TestFinishedValue = result;
        }

        /// <inheritdoc />
        public void TestOutput(TestOutput output)
        {
            if (ThrowException)
            {
                throw new InvalidOperationException("TestOutput exception.");
            }

            TestOutputValue = output;
        }

        /// <inheritdoc />
        public void SendMessage(TestMessage message)
        {
            if (ThrowException)
            {
                throw new InvalidOperationException("SendMessage exception.");
            }

            SendMessageValue = message;
        }

        #endregion
    }

    #endregion
}