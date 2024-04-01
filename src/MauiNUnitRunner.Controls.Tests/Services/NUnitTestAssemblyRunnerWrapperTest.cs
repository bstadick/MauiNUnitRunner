// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Resources;
using MauiNUnitRunner.Controls.Services;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

namespace MauiNUnitRunner.Controls.Tests.Services
{
    [TestFixture]
    public class NUnitTestAssemblyRunnerWrapperTest
    {
        #region Test SetUp/TearDown

        [TearDown]
        public void TestTearDown()
        {
            TestFixtureStubForNUnitRunnerTest.TestDelay = 0;
        }

        [OneTimeTearDown]
        public void TestFixtureTearDown()
        {
            // Since this test fixture re-invokes the test suite setup fixture, have to reapply the resource dictionary
            ResourceHelper.UseOverriddenCurrentApplication = false;
            ResourceHelper.UseOverriddenResourceDictionary = false;

            ResourceHelper.CurrentApplication = Application.Current;
            Application.Current?.Resources.MergedDictionaries.Add(TestControlResources.GetInstance());
        }

        #endregion

        #region Tests for Constructor

        [Test]
        public void TestConstructor()
        {
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            Assert.That(wrapper.TestRunner, Is.Not.Null.And.SameAs(runner));
        }

        [Test]
        public void TestConstructorThrowsArgumentNullExceptionWhenRunnerIsNull()
        {
            Assert.Throws(Is.TypeOf<ArgumentNullException>().And.Message.EqualTo("The runner cannot be null. (Parameter 'runner')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => new NUnitTestAssemblyRunnerWrapper(null));
        }

        #endregion

        #region Tests for TestRunner Property

        [Test]
        public void TestTestRunnerProperty()
        {
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            Assert.That(wrapper.TestRunner, Is.Not.Null.And.SameAs(runner));
        }

        #endregion

        #region Tests for IsTestRunning Property

        [Test]
        public void TestIsTestRunningProperty()
        {
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            Assert.That(wrapper.IsTestRunning, Is.False);
        }

        #endregion

        #region Tests for Load

        [Test]
        public void TestLoad()
        {
            Assembly testAssembly = GetType().Assembly;
            IDictionary<string, object> settings = new Dictionary<string, object>();
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            Assert.That(wrapper.TestRunner.LoadedTest, Is.Null);

            wrapper.Load(testAssembly, settings);

            Assert.That(wrapper.TestRunner.LoadedTest, Is.Not.Null);
        }

        #endregion

        #region Tests for ExploreTests

        [Test]
        public void TestExploreTests([Values] bool withFilter)
        {
            Assembly testAssembly = GetType().Assembly;
            IDictionary<string, object> settings = new Dictionary<string, object>();
            ITestFilter filter = withFilter
                ? NUnitFilter.Where.Class(".*" + nameof(TestFixtureStubForNUnitRunnerTest), true).Build().Filter
                : TestFilter.Empty;
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            string expected = new Uri(testAssembly.Location).LocalPath;

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            wrapper.Load(testAssembly, settings);

            ITest tests = wrapper.ExploreTests(filter);

            // Find test right before the setup fixture.
            ITest leafTest = tests.Tests.FirstOrDefault();
            while (leafTest != null && !leafTest.FullName.EndsWith("NUnitTestSetupFixture"))
            {
                leafTest = leafTest.Tests.FirstOrDefault();
            }

            Assert.That(tests, Is.Not.Null);
            Assert.That(tests.FullName, Is.EqualTo(expected));
            Assert.That(leafTest, Is.Not.Null);

            // Compare number of tests under setup fixture. Filtered tests will only have one match while no filter will have multiple matches.
            if (withFilter)
            {
                Assert.That(leafTest.Tests.Count, Is.EqualTo(1));
            }
            else
            {
                Assert.That(leafTest.Tests.Count, Is.GreaterThan(1));
            }
        }

        #endregion

        #region Tests for Run

        [Test]
        public void TestRun()
        {
            Assembly testAssembly = GetType().Assembly;
            IDictionary<string, object> settings = new Dictionary<string, object>();
            TestFixtureStubForNUnitRunnerTest.TestDelay = 100;
            ITestFilter filter = NUnitFilter.Where.Class(".*" + nameof(TestFixtureStubForNUnitRunnerTest), true).Build().Filter;
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            string expected = new Uri(testAssembly.Location).LocalPath;

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            wrapper.Load(testAssembly, settings);

            ITestResult results = wrapper.Run(null, filter);

            Assert.That(results, Is.Not.Null);
            Assert.That(results.FullName, Is.EqualTo(expected));
            Assert.That(results.PassCount, Is.EqualTo(1));
            Assert.That(results.AssertCount, Is.EqualTo(1));
            Assert.That(results.TotalCount, Is.EqualTo(1));
            Assert.That(results.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(wrapper.IsTestRunning, Is.False);
        }

        #endregion

        #region Tests for StopRun

        [Test]
        public void TestStopRun([Values] bool force)
        {
            Assembly testAssembly = GetType().Assembly;
            IDictionary<string, object> settings = new Dictionary<string, object>();
            TestFixtureStubForNUnitRunnerTest.TestDelay = 200;
            ITestFilter filter = NUnitFilter.Where.Class(".*" + nameof(TestFixtureStubForNUnitRunnerTest), true).Build().Filter;
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            string expected = new Uri(testAssembly.Location).LocalPath;

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            wrapper.Load(testAssembly, settings);

            ITestResult results = null;
            Task testRun = Task.Run(() =>
            {
                results = wrapper.Run(null, filter);
            });

            wrapper.StopRun(force);
            testRun.Wait();

            Assert.That(results, Is.Not.Null);
            Assert.That(results.FullName, Is.EqualTo(expected));
            Assert.That(results.PassCount, Is.EqualTo(1));
            Assert.That(results.AssertCount, Is.EqualTo(1));
            Assert.That(results.TotalCount, Is.EqualTo(1));
            Assert.That(results.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(wrapper.IsTestRunning, Is.False);
        }

        #endregion

        #region Tests for WaitForCompletion

        [Test]
        public void TestWaitForCompletion()
        {
            Assembly testAssembly = GetType().Assembly;
            IDictionary<string, object> settings = new Dictionary<string, object>();
            TestFixtureStubForNUnitRunnerTest.TestDelay = 100;
            ITestFilter filter = NUnitFilter.Where.Class(".*" + nameof(TestFixtureStubForNUnitRunnerTest), true).Build().Filter;
            NUnitTestAssemblyRunner runner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
            string expected = new Uri(testAssembly.Location).LocalPath;

            NUnitTestAssemblyRunnerWrapper wrapper = new NUnitTestAssemblyRunnerWrapper(runner);

            wrapper.Load(testAssembly, settings);

            ITestResult results = null;
            Task testRun = Task.Run(() =>
            {
                results = wrapper.Run(null, filter);
            });

            bool complete = wrapper.WaitForCompletion(200);
            testRun.Wait();

            Assert.That(complete, Is.True);
            Assert.That(results, Is.Not.Null);
            Assert.That(results.FullName, Is.EqualTo(expected));
            Assert.That(results.PassCount, Is.EqualTo(1));
            Assert.That(results.AssertCount, Is.EqualTo(1));
            Assert.That(results.TotalCount, Is.EqualTo(1));
            Assert.That(results.ResultState, Is.EqualTo(ResultState.Success));
            Assert.That(wrapper.IsTestRunning, Is.False);
        }

        #endregion
    }
}