// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Services;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Tests.Services;

[TestFixture]
public class NUnitTestRunnerTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructor()
    {
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.That(runner.TestRunner, Is.Not.Null);
        Assert.That(runner.IsTestRunning, Is.False);
        Assert.That(runner.TestListener, Is.Null);
    }

    [Test]
    public void TestConstructorThrowsArgumentNullExceptionWhenRunnerIsNull()
    {
        Assert.Throws(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("The runner cannot be null. (Parameter 'runner')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitTestRunnerForTest(null));
    }

    #endregion

    #region Tests for TestRunner Property

    [Test]
    public void TestTestRunnerProperty()
    {
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.That(runner.TestRunner, Is.Not.Null);
    }

    #endregion

    #region Tests for IsTestRunning Property

    [Test]
    public void TestIsTestRunningPropertyWhenTestIsRunningReturnsTrue()
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        runner.TestRunnerStub.IsTestRunning = true;

        Assert.That(runner.IsTestRunning, Is.True);
    }

    [Test]
    public void TestIsTestRunningPropertyWhenTestIsNotRunningReturnsFalse()
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        runner.TestRunnerStub.IsTestRunning = false;

        Assert.That(runner.IsTestRunning, Is.False);
    }

    #endregion

    #region Tests for TestListener Property

    [Test]
    public void TestTestListenerProperty([Values] bool isNull)
    {
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.That(runner.TestListener, Is.Null);

        NUnitTestListener listener = null;
        try
        {
            listener = isNull ? null : new NUnitTestListener();

            runner.TestListener = listener;

            Assert.That(runner.TestListener, Is.SameAs(listener));
        }
        finally
        {
            listener?.Dispose();
        }
    }

    #endregion

    #region Tests for AddTestAssembly

    [Test]
    public void TestAddTestAssembly([Values] bool isNull)
    {
        Assembly testAssembly = isNull ? null : GetType().Assembly;
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.That(runner.TestRunner.LoadedTest, Is.Null);

        runner.AddTestAssembly(testAssembly);

        if (isNull)
        {
            Assert.That(runner.TestRunner.LoadedTest, Is.Null);
        }
        else
        {
            Assert.That(runner.TestRunner.LoadedTest, Is.Not.Null);
        }
    }

    [Test]
    public void TestAddTestAssemblyWithSettings([Values] bool isNull, [Values] bool hasSettings)
    {
        Assembly testAssembly = isNull ? null : GetType().Assembly;
        IDictionary<string, object> settings = hasSettings ? new Dictionary<string, object>() : null;
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.That(runner.TestRunner.LoadedTest, Is.Null);

        runner.AddTestAssembly(testAssembly, settings);

        if (isNull)
        {
            Assert.That(runner.TestRunner.LoadedTest, Is.Null);
        }
        else
        {
            Assert.That(runner.TestRunner.LoadedTest, Is.Not.Null);
        }
    }

    [Test]
    public void TestAddMultipleTestAssemblies([Values] bool isNull, [Values] bool hasSettings)
    {
        Assembly testAssembly = isNull ? null : GetType().Assembly;
        IDictionary<string, object> settings = hasSettings ? new Dictionary<string, object>() : null;
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.That(runner.TestRunner.LoadedTest, Is.Null);

        runner.AddTestAssembly(testAssembly, settings);
        runner.AddTestAssembly(testAssembly);

        if (isNull)
        {
            Assert.That(runner.TestRunner.LoadedTest, Is.Null);
        }
        else
        {
            Assert.That(runner.TestRunner.LoadedTest, Is.Not.Null);
        }
    }

    #endregion

    #region Tests for ExploreTests

    [Test]
    public void TestExploreTestsWhenNoTestsAddedThrowsInvalidOperationException()
    {
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.Throws(
            Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Tests must be loaded before exploring them."),
            () => runner.ExploreTests());
    }

    [Test]
    public void TestExploreTestsWhenTestsAddedReturnsTests()
    {
        Assembly testAssembly = GetType().Assembly;
        NUnitTestRunner runner = new NUnitTestRunner();
        string expected = new Uri(testAssembly.Location).LocalPath;

        runner.AddTestAssembly(testAssembly);

        INUnitTest tests = runner.ExploreTests();

        Assert.That(tests, Is.Not.Null);
        Assert.That(tests.Test, Is.Not.Null);
        Assert.That(tests.Test.FullName, Is.EqualTo(expected));
    }


    [Test]
    public void TestExploreTestsWhenNoTestsAddedAndWithFilterThrowsInvalidOperationException([Values] bool isFilterNull)
    {
        NUnitTestRunner runner = new NUnitTestRunner();
        ITestFilter filter = isFilterNull ? null : NUnitFilter.Where.Class(".*" + nameof(TestFixtureForNUnitRunnerTest), true).Build().Filter;

        Assert.Throws(
            Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Tests must be loaded before exploring them."),
            () => runner.ExploreTests(filter));
    }

    [Test]
    public void TestExploreTestsWhenTestsAddedAndWithFilterReturnsFilteredTests([Values] bool isFilterNull)
    {
        Assembly testAssembly = GetType().Assembly;
        NUnitTestRunner runner = new NUnitTestRunner();
        ITestFilter filter =
            isFilterNull ? null : NUnitFilter.Where.Class(".*" + nameof(TestFixtureForNUnitRunnerTest), true).Build().Filter;
        string expected = new Uri(testAssembly.Location).LocalPath;

        runner.AddTestAssembly(testAssembly);

        INUnitTest tests = runner.ExploreTests(filter);

        // Find test right before the setup fixture.
        ITest leafTest = tests.Test.Tests.FirstOrDefault();
        while (leafTest != null && !leafTest.FullName.EndsWith("NUnitTestSetupFixture"))
        {
            leafTest = leafTest.Tests.FirstOrDefault();
        }

        Assert.That(tests, Is.Not.Null);
        Assert.That(tests.Test, Is.Not.Null);
        Assert.That(tests.Test.FullName, Is.EqualTo(expected));
        Assert.That(leafTest, Is.Not.Null);

        // Compare number of tests under setup fixture. Filtered tests will only have one match while no filter will have multiple matches.
        if (isFilterNull)
        {
            Assert.That(leafTest.Tests.Count, Is.GreaterThan(1));
        }
        else
        {
            Assert.That(leafTest.Tests.Count, Is.EqualTo(1));
        }
    }

    #endregion

    #region Tests for Run

    [Test]
    public void TestRunWhenTestsAreNotRunningRunsTestsAndReturnsTestsResults()
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        AutoResetEvent waitToEndRun = new AutoResetEvent(false);
        AutoResetEvent waitForRunStart = new AutoResetEvent(false);
        ITestResult result = new TestResultStub();
        runner.TestRunnerStub.OnRun = (_, _) =>
        {
            // ReSharper disable once AccessToDisposedClosure
            waitForRunStart.Set();
            // ReSharper disable once AccessToDisposedClosure
            waitToEndRun.WaitOne();
            return result;
        };

        Assert.That(runner.IsTestRunning, Is.False);

        Task<INUnitTestResult> testRun = runner.Run();
        waitForRunStart.WaitOne();

        Assert.That(runner.IsTestRunning, Is.True);
        waitToEndRun.Set();

        testRun.Wait();

        Assert.That(testRun, Is.Not.Null);
        Assert.That(testRun.IsCompleted, Is.True);
        Assert.That(runner.IsTestRunning, Is.False);
        Assert.That(testRun.Result, Is.Not.Null);
        Assert.That(testRun.Result.Result, Is.SameAs(result));

        waitForRunStart.Dispose();
        waitToEndRun.Dispose();
    }

    [Test]
    public void TestRunWhenTestsAreNotRunningAndWithFilterRunsTestsAndReturnsTestsResults([Values] bool withListener,
        [Values] bool isFilterNull)
    {
        NUnitTestListener listener = null;
        try
        {
            NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
            listener = withListener ? new NUnitTestListener() : null;
            runner.TestListener = listener;

            ITestFilter filter =
                isFilterNull ? null : NUnitFilter.Where.Class(".*" + nameof(TestFixtureForNUnitRunnerTest), true).Build().Filter;
            ITestFilter expectedFilter = isFilterNull ? NUnitFilter.Empty : filter;
            AutoResetEvent waitToEndRun = new AutoResetEvent(false);
            AutoResetEvent waitForRunStart = new AutoResetEvent(false);
            ITestResult result = new TestResultStub();
            ITestListener runListener = null;
            ITestFilter runFilter = null;
            runner.TestRunnerStub.OnRun = (l, f) =>
            {
                runListener = l;
                runFilter = f;
                // ReSharper disable once AccessToDisposedClosure
                waitForRunStart.Set();
                // ReSharper disable once AccessToDisposedClosure
                waitToEndRun.WaitOne();
                return result;
            };

            Assert.That(runner.IsTestRunning, Is.False);

            Task<INUnitTestResult> testRun = runner.Run(filter);
            waitForRunStart.WaitOne();

            Assert.That(runner.IsTestRunning, Is.True);
            waitToEndRun.Set();

            testRun.Wait();

            Assert.That(testRun, Is.Not.Null);
            Assert.That(testRun.IsCompleted, Is.True);
            Assert.That(runner.IsTestRunning, Is.False);
            Assert.That(testRun.Result, Is.Not.Null);
            Assert.That(testRun.Result.Result, Is.SameAs(result));
            Assert.That(runListener, Is.SameAs(listener));
            Assert.That(runFilter, Is.SameAs(expectedFilter));

            waitForRunStart.Dispose();
            waitToEndRun.Dispose();

        }
        finally
        {
            listener?.Dispose();
        }
    }

    [Test]
    public void TestRunWhenTestsAreAlreadyRunningDoesNotRunTestsAndReturnsNullResult()
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        runner.TestRunnerStub.IsTestRunning = true;

        Assert.That(runner.IsTestRunning, Is.True);

        Task<INUnitTestResult> testRun = runner.Run();

        Assert.That(runner.IsTestRunning, Is.True);
        Assert.That(testRun, Is.Not.Null);
        Assert.That(testRun.IsCompleted, Is.True);
        Assert.That(testRun.Result, Is.Null);
    }

    #endregion

    #region Tests for StopRun

    [Test]
    public void TestStopRunWhenTestsAreRunningResultsInStoppedTests([Values] bool force)
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        AutoResetEvent waitToEndRun = new AutoResetEvent(false);
        AutoResetEvent waitForRunStart = new AutoResetEvent(false);
        runner.TestRunnerStub.OnRun = (_, _) =>
        {
            // ReSharper disable once AccessToDisposedClosure
            waitForRunStart.Set();
            // ReSharper disable once AccessToDisposedClosure
            waitToEndRun.WaitOne();
            return null;
        };
        bool forceValue = !force;
        runner.TestRunnerStub.OnStopRun = b =>
        {
            forceValue = b;
            // ReSharper disable once AccessToDisposedClosure
            waitToEndRun.Set();
        };

        Task<INUnitTestResult> testRun = runner.Run();
        waitForRunStart.WaitOne();

        Assert.That(runner.IsTestRunning, Is.True);

        runner.StopRun(force);

        testRun.Wait();

        Assert.That(testRun.IsCompleted, Is.True);
        Assert.That(runner.IsTestRunning, Is.False);
        Assert.That(forceValue, Is.EqualTo(force));

        waitForRunStart.Dispose();
        waitToEndRun.Dispose();
    }

    [Test]
    public void TestStopRunWhenTestsAreNotRunningResultsInNoChange([Values] bool force)
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();

        Assert.That(runner.IsTestRunning, Is.False);

        runner.StopRun(force);

        Assert.That(runner.IsTestRunning, Is.False);
    }

    #endregion

    #region Tests for WaitForCompletion

    [Test]
    public void TestWaitForCompletionWhenTestsAreRunningWaitsForTestCompletionAndReturnsTrue()
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        AutoResetEvent waitToEndRun = new AutoResetEvent(false);
        AutoResetEvent waitForRunStart = new AutoResetEvent(false);
        runner.TestRunnerStub.OnRun = (_, _) =>
        {
            // ReSharper disable once AccessToDisposedClosure
            waitForRunStart.Set();
            // ReSharper disable once AccessToDisposedClosure
            waitToEndRun.WaitOne();
            return null;
        };
        runner.TestRunnerStub.OnWaitForCompletion = _ =>
        {
            // ReSharper disable once AccessToDisposedClosure
            waitToEndRun.Set();
            return true;
        };

        Task<INUnitTestResult> testRun = runner.Run();
        waitForRunStart.WaitOne();

        Assert.That(runner.IsTestRunning, Is.True);

        bool complete = runner.WaitForCompletion(100);

        testRun.Wait();

        Assert.That(complete, Is.True);
        Assert.That(testRun.IsCompleted, Is.True);
        Assert.That(runner.IsTestRunning, Is.False);

        waitForRunStart.Dispose();
        waitToEndRun.Dispose();
    }

    [Test]
    public void TestWaitForCompletionWhenTestsAreRunningAndWaitTimeoutsReturnsFalse()
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        AutoResetEvent waitToEndRun = new AutoResetEvent(false);
        AutoResetEvent waitForRunStart = new AutoResetEvent(false);
        runner.TestRunnerStub.OnRun = (_, _) =>
        {
            // ReSharper disable once AccessToDisposedClosure
            waitForRunStart.Set();
            // ReSharper disable once AccessToDisposedClosure
            waitToEndRun.WaitOne();
            return null;
        };
        int timeout = 0;
        runner.TestRunnerStub.OnWaitForCompletion = i =>
        {
            timeout = i;
            return false;
        };

        Task<INUnitTestResult> testRun = runner.Run();
        waitForRunStart.WaitOne();

        Assert.That(runner.IsTestRunning, Is.True);

        bool complete = runner.WaitForCompletion(100);

        Assert.That(timeout, Is.EqualTo(100));
        Assert.That(complete, Is.False);
        Assert.That(testRun.IsCompleted, Is.False);
        Assert.That(runner.IsTestRunning, Is.True);

        waitToEndRun.Set();
        testRun.Wait();

        waitForRunStart.Dispose();
        waitToEndRun.Dispose();
    }

    [Test]
    public void TestWaitForCompletionWhenTestsAreNotRunningReturnsImmediatelyAndTrue()
    {
        NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
        runner.TestRunnerStub.OnWaitForCompletion = _ => true;

        Assert.That(runner.IsTestRunning, Is.False);

        bool complete = runner.WaitForCompletion(100);

        Assert.That(complete, Is.True);
        Assert.That(runner.IsTestRunning, Is.False);
    }

    #endregion

    #region Tests for GetTestResultsAsXmlStream

    [Test]
    public void TestGetTestResultsAsXmlStreamWithNullOrNoResultReturnsNull([Values] bool isNull)
    {
        INUnitTestResult result = isNull ? null : new NUnitTestResult(null);
        NUnitTestRunner runner = new NUnitTestRunner();

        Stream xmlStream = runner.GetTestResultsAsXmlStream(result, out string fileName);

        Assert.That(xmlStream, Is.Null);
        Assert.That(fileName, Is.Empty);
    }

    [Test]
    [TestCase("testcase", "123", "testcase-123.xml")]
    [TestCase("testcase('a', 'b', 'c')", "123", "testcase-123.xml")]
    [TestCase("testcase with space", "123", "testcase-with-space-123.xml")]
    [TestCase(" (testcase) ", "123", "test-123.xml")]
    [TestCase(null, "123", "test-123.xml")]
    [TestCase("", "123", "test-123.xml")]
    public void TestGetTestResultsAsXmlStreamReturnsXmlStream(string testName, string testId, string expectedName)
    {
        TestResultStub resultInstance = new TestResultStub();
        resultInstance.Test = new TestStub { Id = testId, FullName = testName };
        const string xml = "<result><test>foo</test></result>";
        resultInstance.TestResultXml = TNode.FromXml(xml);
        INUnitTestResult result = new NUnitTestResult(resultInstance);
        NUnitTestRunner runner = new NUnitTestRunner();
        string expectedXml = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}<result>{Environment.NewLine}  <test>foo</test>{Environment.NewLine}</result>";

        using Stream xmlStream = runner.GetTestResultsAsXmlStream(result, out string fileName);

        Assert.That(xmlStream, Is.Not.Null);
        using StreamReader sr = new StreamReader(xmlStream);
        Assert.That(sr.ReadToEnd(), Is.EqualTo(expectedXml));
        Assert.That(fileName, Is.EqualTo(expectedName));
    }

    [Test]
    public void TestGetTestResultsAsXmlStreamWhenExceptionOccursClosesStreamAndThrowsException()
    {
        TestStubForXmlTest resultInstance = new TestStubForXmlTest();
        resultInstance.Test = new TestStub { Id = "123", FullName = "testcase" };
        const string xml = "<result><test>foo</test></result>";
        resultInstance.TestResultXml = TNode.FromXml(xml);
        resultInstance.ToXmlThrowsException = true;
        INUnitTestResult result = new NUnitTestResult(resultInstance);
        NUnitTestRunner runner = new NUnitTestRunner();

        Assert.Throws(Is.TypeOf<InvalidOperationException>().And.Message.EqualTo("Xml node is invalid"),
            () => runner.GetTestResultsAsXmlStream(result, out _));
    }

    #endregion

    #region Nested Class: NUnitTestRunnerForTest

    /// <summary>
    ///     Extends NUnitTestRunner for use with tests.
    /// </summary>
    private class NUnitTestRunnerForTest : NUnitTestRunner
    {
        #region Members for Test

        /// <summary>
        ///     Gets the underlying test runner stub.
        /// </summary>
        public NUnitTestAssemblyRunnerStub TestRunnerStub => v_TestRunner as NUnitTestAssemblyRunnerStub;

        #endregion

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="NUnitTestRunner"/> with overloads for testing purposes.
        /// </summary>
        public NUnitTestRunnerForTest() : base(new NUnitTestAssemblyRunnerStub())
        {
        }

        /// <summary>
        ///     Initializes a new <see cref="NUnitTestRunner"/> with overloads for testing purposes.
        /// </summary>
        /// <param name="runner">The <see cref="NUnitTestAssemblyRunnerStub"/> stub for test.</param>
        public NUnitTestRunnerForTest(NUnitTestAssemblyRunnerStub runner) : base(runner)
        {
        }

        #endregion
    }

    #endregion

    #region Nested Class: TestResultStubForXmlTest

    /// <summary>
    ///     Extends TestResultStub for use with specific tests.
    /// </summary>
    private class TestStubForXmlTest : TestResultStub
    {

        #region Members for Test

        /// <summary>
        ///     Gets or sets if <see cref="ToXml"/> throws an exception.
        /// </summary>
        public bool ToXmlThrowsException { get; set; }

        #endregion

        #region Overridden Methods

        /// <inheritdoc />
        public override TNode ToXml(bool recursive)
        {
            if (ToXmlThrowsException)
            {
                throw new InvalidOperationException("Xml node is invalid");
            }

            return base.ToXml(recursive);
        }

        #endregion
    }

    #endregion
}