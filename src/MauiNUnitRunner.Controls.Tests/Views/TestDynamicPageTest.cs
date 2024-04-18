// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using System.Text;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Services;
using MauiNUnitRunner.Controls.Views;
using NUnit.Framework;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

// Ignore Spelling: Bindable
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace MauiNUnitRunner.Controls.Tests.Views
{
    [TestFixture]
    public class TestDynamicPageTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructor([Values] bool isTestNull, [Values] bool isExploreTestNull)
        {
            NUnitTestAssemblyRunnerStub testAssemblyRunner = new NUnitTestAssemblyRunnerStub();
            NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest(testAssemblyRunner);
            runner.ExploreTestsReturnNull = isExploreTestNull;

            ITest testInstance = new TestStub { Name = "testcase" };
            INUnitTest expectedTest = new NUnitTest(testInstance);
            INUnitTest test = isTestNull ? null : expectedTest;

            ITestFilter exploreFilter = null;
            bool exploreInvoke = false;
            testAssemblyRunner.OnExploreTests = filter =>
            {
                exploreInvoke = true;
                exploreFilter = filter;
                return testInstance;
            };

            TestDynamicPage page = new TestDynamicPageForTest(runner, test);

            if (isTestNull && isExploreTestNull)
            {
                Assert.That(page.Test, Is.Null);
                Assert.That(page.Title, Is.Empty);
            }
            else
            {
                Assert.That(page.Test, Is.EqualTo(expectedTest));
                Assert.That(page.Title, Is.EqualTo("testcase"));
            }
            
            Assert.That(page.ShowFooterLinks, Is.True);
            Assert.That(page.TestRunner, Is.SameAs(runner));
            if (isTestNull)
            {
                Assert.That(exploreInvoke, Is.True);
                Assert.That(exploreFilter, Is.EqualTo(NUnitFilter.Empty));
            }
            else
            {
                Assert.That(exploreInvoke, Is.False);
            }
        }

        [Test]
        public void TestConstructorThrowsArgumentNullExceptionWhenTestRunnerIsNull()
        {
            Assert.Throws(
                Is.TypeOf<ArgumentNullException>().And.Message
                    .EqualTo("The testRunner cannot be null. (Parameter 'testRunner')"),
                // ReSharper disable once ObjectCreationAsStatement
                () => new TestDynamicPageForTest(null));
        }

        #endregion

        #region Tests for Test Property

        [Test]
        public void TestTestProperty([Values] bool isTestNull)
        {
            NUnitTestAssemblyRunnerStub testAssemblyRunner = new NUnitTestAssemblyRunnerStub();
            NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest(testAssemblyRunner);

            ITest testInstanceInitial = new TestStub { Name = "testcase" };
            INUnitTest expectedTest = new NUnitTest(testInstanceInitial);
            INUnitTest testInitial = isTestNull ? null : expectedTest;

            testAssemblyRunner.OnExploreTests = _ => testInstanceInitial;

            TestDynamicPage page = new TestDynamicPageForTest(runner, testInitial);

            Assert.That(TestDynamicPage.TestProperty.PropertyName, Is.EqualTo("Test"));
            Assert.That(TestDynamicPage.TestProperty.DeclaringType, Is.EqualTo(typeof(TestDynamicPage)));
            Assert.That(TestDynamicPage.TestProperty.ReturnType, Is.EqualTo(typeof(INUnitTest)));
            Assert.That(TestDynamicPage.TestProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
            Assert.That(TestDynamicPage.TestProperty.DefaultValue, Is.Null);
            Assert.That(TestDynamicPage.TestProperty.IsReadOnly, Is.False);

            Assert.That(page.Test, Is.EqualTo(expectedTest));

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTest test = new NUnitTest(testInstance);

            page.Test = test;

            Assert.That(page.Test, Is.SameAs(test));
        }

        #endregion

        #region Tests for IsTestRunning Property

        [Test]
        public void TestIsTestRunningProperty([Values] bool withParentPage)
        {
            INUnitTestRunner runner = new NUnitTestRunnerForTest();

            TestDynamicPage parentPage = new TestDynamicPageForTest(runner);
            TestDynamicPage page = new TestDynamicPageForTest(runner, null, withParentPage ? parentPage : null);

            Assert.That(TestDynamicPage.IsTestRunningProperty.PropertyName, Is.EqualTo("IsTestRunning"));
            Assert.That(TestDynamicPage.IsTestRunningProperty.DeclaringType, Is.EqualTo(typeof(TestDynamicPage)));
            Assert.That(TestDynamicPage.IsTestRunningProperty.ReturnType, Is.EqualTo(typeof(bool)));
            Assert.That(TestDynamicPage.IsTestRunningProperty.DefaultBindingMode, Is.EqualTo(BindingMode.TwoWay));
            Assert.That(TestDynamicPage.IsTestRunningProperty.DefaultValue, Is.False);
            Assert.That(TestDynamicPage.IsTestRunningProperty.IsReadOnly, Is.False);

            Assert.That(page.IsTestRunning, Is.False);
            Assert.That(parentPage.IsTestRunning, Is.False);

            page.IsTestRunning = false;

            Assert.That(page.IsTestRunning, Is.False);
            Assert.That(parentPage.IsTestRunning, Is.False);

            page.IsTestRunning = true;

            Assert.That(page.IsTestRunning, Is.True);
            Assert.That(parentPage.IsTestRunning, Is.EqualTo(withParentPage));

            page.IsTestRunning = true;

            Assert.That(page.IsTestRunning, Is.True);
            Assert.That(parentPage.IsTestRunning, Is.EqualTo(withParentPage));

            page.IsTestRunning = false;

            Assert.That(page.IsTestRunning, Is.False);
            Assert.That(parentPage.IsTestRunning, Is.False);
        }

        #endregion

        #region Tests for ShowFooterLinks Property

        [Test]
        public void TestShowFooterLinksProperty()
        {
            INUnitTestRunner runner = new NUnitTestRunnerForTest();

            TestDynamicPage page = new TestDynamicPageForTest(runner);

            Assert.That(TestDynamicPage.ShowFooterLinksProperty.PropertyName, Is.EqualTo("ShowFooterLinks"));
            Assert.That(TestDynamicPage.ShowFooterLinksProperty.DeclaringType, Is.EqualTo(typeof(TestDynamicPage)));
            Assert.That(TestDynamicPage.ShowFooterLinksProperty.ReturnType, Is.EqualTo(typeof(bool)));
            Assert.That(TestDynamicPage.ShowFooterLinksProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
            Assert.That(TestDynamicPage.ShowFooterLinksProperty.DefaultValue, Is.True);
            Assert.That(TestDynamicPage.ShowFooterLinksProperty.IsReadOnly, Is.False);

            Assert.That(page.ShowFooterLinks, Is.True);

            page.ShowFooterLinks = true;

            Assert.That(page.ShowFooterLinks, Is.True);

            page.ShowFooterLinks = false;

            Assert.That(page.ShowFooterLinks, Is.False);

            page.ShowFooterLinks = false;

            Assert.That(page.ShowFooterLinks, Is.False);

            page.ShowFooterLinks = true;

            Assert.That(page.ShowFooterLinks, Is.True);
        }

        #endregion

        #region Tests for TestRunner Property

        [Test]
        public void TestTestRunnerProperty()
        {
            INUnitTestRunner runner = new NUnitTestRunnerForTest();

            TestDynamicPage page = new TestDynamicPageForTest(runner);

            Assert.That(page.TestRunner, Is.SameAs(runner));
        }

        #endregion

        #region Tests for TestDynamicPage_OnItemSelected

        [Test]
        public void TestOnTestItemSelected()
        {
            INUnitTestRunner runner = new NUnitTestRunnerForTest();

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTest test = new NUnitTest(testInstance);

            page.InvokeOnTestItemSelected(this, new NUnitTestEventArgs(test));

            Assert.That(page.NavigationPushAsyncInvoked, Is.True);
            TestDynamicPage navigatedPage = page.NavigatedPage as TestDynamicPage;
            Assert.That(navigatedPage, Is.Not.Null);
            Assert.That(navigatedPage.TestRunner, Is.SameAs(runner));
            Assert.That(navigatedPage.Test, Is.SameAs(test));
            Assert.That(navigatedPage.ShowFooterLinks, Is.False);
        }

        [Test]
        public void TestOnTestItemSelectedWhenTestNullDoesNothing([Values] bool isEventNull, [Values] bool isTestNull)
        {
            INUnitTestRunner runner = new NUnitTestRunnerForTest();

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            INUnitTest test = isTestNull ? null : new NUnitTest(null);
            NUnitTestEventArgs eventArgs = isEventNull ? null : new NUnitTestEventArgs(test);

            page.InvokeOnTestItemSelected(this, eventArgs);

            Assert.That(page.NavigationPushAsyncInvoked, Is.False);
        }

        #endregion

        #region Tests for TestDynamicPage_OnRunTestsClicked

        [Test]
        public void TestOnRunTestsClicked([Values] bool isResultNull)
        {
            NUnitTestAssemblyRunnerStub testAssemblyRunner = new NUnitTestAssemblyRunnerStub();
            NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest(testAssemblyRunner);
            runner.RunReturnNull = isResultNull;

            ITest testInstance = new TestStub { Id = "123", Name = "testcase" };
            INUnitTest test = new NUnitTest(testInstance);

            ITestResult result = new TestResultStub { Name = "testResult", Test = testInstance};
            INUnitTestResult expectedResult = new NUnitTestResult(result);
            if (isResultNull)
            {
                expectedResult = new NUnitTestResult(new TestResultStub { Name = "notResult" });
                test.Result = expectedResult;
            }

            ITestFilter runFilter = null;
            bool runInvoke = false;
            testAssemblyRunner.OnRun = (_, filter) =>
            {
                runInvoke = true;
                runFilter = filter;
                return result;
            };

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            page.InvokeOnRunTestsClicked(this, new NUnitTestEventArgs(test));

            Assert.That(runInvoke, Is.True);
            Assert.That(runFilter, Is.Not.Null);
            Assert.That(runFilter.ToXml(true).OuterXml, Is.EqualTo("<id>123</id>"));
            Assert.That(test.Result, Is.EqualTo(expectedResult));
        }

        [Test]
        public void TestOnRunTestsClickedWhenTestNullDoesNothing([Values] bool isEventNull, [Values] bool isTestNull)
        {
            NUnitTestAssemblyRunnerStub testAssemblyRunner = new NUnitTestAssemblyRunnerStub();
            NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest(testAssemblyRunner);

            bool runInvoke = false;
            testAssemblyRunner.OnRun = (_, _) =>
            {
                runInvoke = true;
                return null;
            };

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            INUnitTest test = isTestNull ? null : new NUnitTest(null);
            NUnitTestEventArgs eventArgs = isEventNull ? null : new NUnitTestEventArgs(test);

            page.InvokeOnRunTestsClicked(this, eventArgs);

            Assert.That(runInvoke, Is.False);
        }

        #endregion

        #region Tests for TestDynamicView_OnSaveResultsClicked

        [Test]
        public void TestOnSaveResultsClicked()
        {
            INUnitTestRunner runner = new NUnitTestRunnerForTest();

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            ITest testInstance = new TestStub { FullName = "testcase", Id = "123" };
            ITestResult resultInstance = new TestResultStub
                { Test = testInstance, TestResultXml = TNode.FromXml("<test-run id=\"0\"><test-suite id=\"1\"><test-case id=\"2\" /></test-suite></test-run>") };
            string expectedXml = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}<test-run id=\"0\">{Environment.NewLine}  <test-suite id=\"1\">{Environment.NewLine}    <test-case id=\"2\"></test-case>{Environment.NewLine}  </test-suite>{Environment.NewLine}</test-run>";
            INUnitTestResult result = new NUnitTestResult(resultInstance);
            NUnitTestResultEventArgs eventArgs = new NUnitTestResultEventArgs(result);

            page.InvokeOnSaveResultsClicked(this, eventArgs);

            Assert.That(page.SaveAsyncInvoked, Is.True);
            Assert.That(page.SaveAsyncFolderPath, Is.EqualTo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            Assert.That(page.SaveAsyncFileName, Is.EqualTo("testcase-123.xml"));
            Assert.That(page.SaveAsyncResultStream, Is.EqualTo(expectedXml));
        }

        [Test]
        public void TestOnSaveResultsClickedWhenResultNullDoesNothing([Values] bool isEventNull, [Values] bool isStreamNull)
        {
            NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();
            runner.GetTestResultsAsXmlStreamReturnNull = isStreamNull;

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            NUnitTestResultEventArgs eventArgs = isEventNull ? null : new NUnitTestResultEventArgs(null);

            page.InvokeOnSaveResultsClicked(this, eventArgs);

            Assert.That(page.SaveAsyncInvoked, Is.False);
        }

        [Test]
        public void TestOnSaveResultsClickedDisplaysAlertWhenExceptionThrown()
        {
            NUnitTestRunnerForTest runner = new NUnitTestRunnerForTest();

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            ITest testInstance = new TestStub { FullName = "testcase", Id = "123" };
            ITestResult resultInstance = new TestResultStub
                { Test = testInstance, TestResultXml = TNode.FromXml("<test-run id=\"0\"><test-suite id=\"1\"><test-case id=\"2\" /></test-suite></test-run>") };
            string expectedXml = $"<?xml version=\"1.0\" encoding=\"utf-8\"?>{Environment.NewLine}<test-run id=\"0\">{Environment.NewLine}  <test-suite id=\"1\">{Environment.NewLine}    <test-case id=\"2\"></test-case>{Environment.NewLine}  </test-suite>{Environment.NewLine}</test-run>";
            INUnitTestResult result = new NUnitTestResult(resultInstance);
            NUnitTestResultEventArgs eventArgs = new NUnitTestResultEventArgs(result);

            page.SaveAsyncThrowsException = true;

            page.InvokeOnSaveResultsClicked(this, eventArgs);

            Assert.That(page.SaveAsyncInvoked, Is.True);
            Assert.That(page.SaveAsyncFolderPath, Is.EqualTo(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments)));
            Assert.That(page.SaveAsyncFileName, Is.EqualTo("testcase-123.xml"));
            Assert.That(page.SaveAsyncResultStream, Is.EqualTo(expectedXml));
            Assert.That(page.DisplayAlertMessageInvoked, Is.True);
            Assert.That(page.DisplayAlertMessageTitle, Is.EqualTo("Save Results"));
            Assert.That(page.DisplayAlertMessageMessage, Is.EqualTo("Failed to save the test result to a file."));
            Assert.That(page.DisplayAlertMessageCancel, Is.EqualTo("OK"));
        }

        #endregion

        #region Tests for AboutButton_OnClicked

        [Test]
        public void TestOnAboutButtonClicked()
        {
            INUnitTestRunner runner = new NUnitTestRunnerForTest();

            TestDynamicPageForTest page = new TestDynamicPageForTest(runner);

            page.InvokeOnAboutButtonClicked(this, EventArgs.Empty);

            Assert.That(page.NavigationPushAsyncInvoked, Is.True);
            Assert.That(page.NavigatedPage, Is.Not.Null.And.TypeOf<AboutPage>());
        }

        #endregion

        #region Nested Class: TestDynamicPageForTest

        /// <summary>
        ///     Extends TestDynamicPage for use with tests.
        /// </summary>
        private class TestDynamicPageForTest : TestDynamicPage
        {
            #region Members for Test

            /// <summary>
            ///     Holds the bindable property values of the class instance.
            /// </summary>
            private readonly Dictionary<BindableProperty, object> v_BindableProperties =
                new Dictionary<BindableProperty, object>();

            /// <summary>
            ///     Gets if the NavigationPushAsync method was invoked.
            /// </summary>
            public bool NavigationPushAsyncInvoked { get; private set; }

            /// <summary>
            ///     Gets the last page navigated to.
            /// </summary>
            public Page NavigatedPage { get; private set; }

            /// <summary>
            ///     Gets if the SaveAsync method was invoked.
            /// </summary>
            public bool SaveAsyncInvoked { get; private set; }

            /// <summary>
            ///     Gets the last saved folder path.
            /// </summary>
            public string SaveAsyncFolderPath { get; private set; }

            /// <summary>
            ///     Gets the last saved file name
            /// </summary>
            public string SaveAsyncFileName { get; private set; }

            /// <summary>
            ///     Gets the last saved file contents.
            /// </summary>
            public string SaveAsyncResultStream { get; private set; }

            /// <summary>
            ///     Gets or sets if the SaveAsync throws an exception.
            /// </summary>
            public bool SaveAsyncThrowsException { get; set; }

            /// <summary>
            ///     Gets if the DisplayAlertMessage method was invoked.
            /// </summary>
            public bool DisplayAlertMessageInvoked { get; private set; }

            /// <summary>
            ///     Gets the last alert message title.
            /// </summary>
            public string DisplayAlertMessageTitle { get; private set; }

            /// <summary>
            ///     Gets the last alert message.
            /// </summary>
            public string DisplayAlertMessageMessage { get; private set; }

            /// <summary>
            ///     Gets the last alert message cancel button text.
            /// </summary>
            public string DisplayAlertMessageCancel { get; private set; }

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new TestDynamicPageForTest.
            /// </summary>
            public TestDynamicPageForTest(INUnitTestRunner testRunner, INUnitTest test = null,
                TestDynamicPage parentPage = null) : base(testRunner, test, parentPage, false)
            {
            }

            #endregion

            #region Methods for Test

            /// <summary>
            ///     Invokes the <see cref="TestDynamicPage.TestDynamicPage_OnItemSelected"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="ListView"/> that contains the item.</param>
            /// <param name="e">The test selected event arguments.</param>
            public void InvokeOnTestItemSelected(object sender, NUnitTestEventArgs e)
            {
                TestDynamicPage_OnItemSelected(sender, e);
            }

            /// <summary>
            ///     Invokes the <see cref="TestDynamicPage.TestDynamicPage_OnRunTestsClicked"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
            /// <param name="e">The button click event arguments.</param>
            public void InvokeOnRunTestsClicked(object sender, NUnitTestEventArgs e)
            {
                TestDynamicPage_OnRunTestsClicked(sender, e);
            }

            /// <summary>
            ///     Invokes the <see cref="TestDynamicPage.TestDynamicView_OnSaveResultsClicked"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
            /// <param name="e">The button click event arguments.</param>
            public void InvokeOnSaveResultsClicked(object sender, NUnitTestResultEventArgs e)
            {
                TestDynamicView_OnSaveResultsClicked(sender, e);
            }

            /// <summary>
            ///     Invokes the <see cref="TestDynamicPage.AboutButton_OnClicked"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
            /// <param name="e">The button click event arguments.</param>
            public void InvokeOnAboutButtonClicked(object sender, EventArgs e)
            {
                AboutButton_OnClicked(sender, e);
            }

            #endregion

            #region Overridden Methods

            /// <inheritdoc />
            protected override object GetBindableValue(BindableProperty property, object defaultValue)
            {
                if (!v_BindableProperties.ContainsKey(property))
                {
                    return defaultValue;
                }

                return v_BindableProperties.GetValueOrDefault(property);
            }

            /// <inheritdoc />
            protected override void SetBindableValue(BindableProperty property, object value)
            {
                v_BindableProperties[property] = value;
            }

            /// <inheritdoc />
            protected override TestDynamicPage CreateTestDynamicPage(INUnitTest test)
            {
                return new TestDynamicPageForTest(TestRunner, test);
            }

            /// <inheritdoc />
            protected override Task NavigationPushAsync(Page page)
            {
                NavigationPushAsyncInvoked = true;
                NavigatedPage = page;
                return Task.CompletedTask;
            }

            /// <inheritdoc />
            protected override Task SaveAsync(string folderPath, string fileName, Stream resultStream)
            {
                SaveAsyncInvoked = true;
                SaveAsyncFolderPath = folderPath;
                SaveAsyncFileName = fileName;
                if (resultStream != null)
                {
                    using StreamReader sr = new StreamReader(resultStream, Encoding.UTF8);
                    SaveAsyncResultStream = sr.ReadToEnd();
                }

                if (SaveAsyncThrowsException)
                {
                    throw new InvalidOperationException("SaveAsync threw an exception.");
                }

                return Task.CompletedTask;
            }

            /// <inheritdoc />
            protected override Task DisplayAlertMessage(string title, string message, string cancel)
            {
                DisplayAlertMessageInvoked = true;
                DisplayAlertMessageTitle = title;
                DisplayAlertMessageMessage = message;
                DisplayAlertMessageCancel = cancel;
                return Task.CompletedTask;
            }

            #endregion
        }

        #endregion

        #region Nested Class: NUnitTestRunnerForTest

        /// <summary>
        ///     Extends NUnitTestRunnerForTest for use with tests.
        /// </summary>
        private class NUnitTestRunnerForTest : INUnitTestRunner
        {
            #region Members for Test
            /// <summary>
            ///     Holds the underlying INUnitTestAssemblyRunner.
            /// </summary>
            private readonly INUnitTestAssemblyRunner v_TestRunner;

            /// <summary>
            ///     Gets or sets if the ExploreTests call returns null.
            /// </summary>
            public bool ExploreTestsReturnNull { get; set; }

            /// <summary>
            ///     Gets or sets if the Run call returns null.
            /// </summary>
            public bool RunReturnNull { get; set; }

            /// <summary>
            ///     Gets or sets if the GetTestResultsAsXmlStream call returns null.
            /// </summary>
            public bool GetTestResultsAsXmlStreamReturnNull { get; set; }

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new NUnitTestRunnerForTest.
            /// </summary>
            /// <param name="runner">The underlying INUnitTestAssemblyRunner, or null to use the default stub runner.</param>
            public NUnitTestRunnerForTest(INUnitTestAssemblyRunner runner = null)
            {
                v_TestRunner = runner ?? new NUnitTestAssemblyRunnerStub();
            }

            #endregion

            #region Implementation of INUnitTestRunner

            /// <inheritdoc />
            public NUnitTestAssemblyRunner TestRunner => null;

            /// <inheritdoc />
            public bool IsTestRunning => false;

            /// <inheritdoc />
            public ITestListener TestListener { get; set; }

            /// <inheritdoc />
            public void AddTestAssembly(Assembly assembly, IDictionary<string, object> settings = null)
            {
                v_TestRunner.Load(assembly, settings);
            }

            /// <inheritdoc />
            public INUnitTest ExploreTests(ITestFilter filter = null)
            {
                ITest test = v_TestRunner.ExploreTests(filter);

                if (ExploreTestsReturnNull)
                {
                    return null;
                }

                return new NUnitTest(test);
            }

            /// <inheritdoc />
            public Task<INUnitTestResult> Run(ITestFilter filter = null)
            {
                ITestResult result = v_TestRunner.Run(null, filter);

                if (RunReturnNull)
                {
                    return Task.FromResult((INUnitTestResult)null);
                }

                return Task.FromResult((INUnitTestResult)new NUnitTestResult(result));
            }

            /// <inheritdoc />
            public void StopRun(bool force)
            {
                v_TestRunner.StopRun(force);
            }

            /// <inheritdoc />
            public bool WaitForCompletion(int timeout)
            {
                return v_TestRunner.WaitForCompletion(timeout);
            }

            /// <inheritdoc />
            public Stream GetTestResultsAsXmlStream(INUnitTestResult result, out string fileName)
            {
                if (GetTestResultsAsXmlStreamReturnNull)
                {
                    fileName = null;
                    return null;
                }

                return new NUnitTestRunner().GetTestResultsAsXmlStream(result, out fileName);
            }

            #endregion
        }

        #endregion
    }
}