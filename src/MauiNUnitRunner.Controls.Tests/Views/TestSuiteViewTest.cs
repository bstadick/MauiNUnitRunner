// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Views;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

// Ignore Spelling: Bindable
// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace MauiNUnitRunner.Controls.Tests.Views
{
    [TestFixture]
    public class TestSuiteViewTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructor()
        {
            TestSuiteView page = new TestSuiteViewForTest();

            Assert.That(page.Test, Is.Null);
        }

        #endregion

        #region Tests for Test Property

        [Test]
        public void TestTestProperty()
        {
            TestSuiteView page = new TestSuiteViewForTest();

            Assert.That(TestSuiteView.TestProperty.PropertyName, Is.EqualTo("Test"));
            Assert.That(TestSuiteView.TestProperty.DeclaringType, Is.EqualTo(typeof(TestSuiteView)));
            Assert.That(TestSuiteView.TestProperty.ReturnType, Is.EqualTo(typeof(INUnitTest)));
            Assert.That(TestSuiteView.TestProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
            Assert.That(TestSuiteView.TestProperty.DefaultValue, Is.Null);
            Assert.That(TestSuiteView.TestProperty.IsReadOnly, Is.False);

            Assert.That(page.Test, Is.Null);

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTest test = new NUnitTest(testInstance);

            page.Test = test;

            Assert.That(page.Test, Is.SameAs(test));
        }

        #endregion

        #region Tests for IsTestRunning Property

        [Test]
        public void TestIsTestRunningProperty()
        {
            TestSuiteView page = new TestSuiteViewForTest();

            Assert.That(TestSuiteView.IsTestRunningProperty.PropertyName, Is.EqualTo("IsTestRunning"));
            Assert.That(TestSuiteView.IsTestRunningProperty.DeclaringType, Is.EqualTo(typeof(TestSuiteView)));
            Assert.That(TestSuiteView.IsTestRunningProperty.ReturnType, Is.EqualTo(typeof(bool)));
            Assert.That(TestSuiteView.IsTestRunningProperty.DefaultBindingMode, Is.EqualTo(BindingMode.TwoWay));
            Assert.That(TestSuiteView.IsTestRunningProperty.DefaultValue, Is.False);
            Assert.That(TestSuiteView.IsTestRunningProperty.IsReadOnly, Is.False);

            Assert.That(page.IsTestRunning, Is.False);

            page.IsTestRunning = false;

            Assert.That(page.IsTestRunning, Is.False);

            page.IsTestRunning = true;

            Assert.That(page.IsTestRunning, Is.True);

            page.IsTestRunning = true;

            Assert.That(page.IsTestRunning, Is.True);

            page.IsTestRunning = false;

            Assert.That(page.IsTestRunning, Is.False);
        }

        #endregion

        #region Tests for TestSuiteView_OnTestItemSelected

        [Test]
        public void TestOnTestItemSelectedWhenEventSetInvokesEventAndResetsSelectedItem([Values] bool isTestNull)
        {
            TestSuiteViewForTest page = new TestSuiteViewForTest();

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
            page.Test = test;

            ListViewStub list = new ListViewStub();
            list.SelectedItem = test;

            object eventSender = null;
            NUnitTestEventArgs eventArgs = null;
            bool eventInvoked = false;
            page.TestItemSelected += (sender, args) =>
            {
                eventInvoked = true;
                eventSender = sender;
                eventArgs = args;
            };

            page.InvokeOnTestItemSelected(list, new SelectedItemChangedEventArgs(test, 1));

            if (isTestNull)
            {
                Assert.That(eventInvoked, Is.False);
            }
            else
            {
                Assert.That(eventInvoked, Is.True);
                Assert.That(eventSender, Is.SameAs(list));
                Assert.That(eventArgs, Is.Not.Null);
                Assert.That(eventArgs.Test, Is.SameAs(test));
            }

            Assert.That(list.SelectedItem, Is.Null);
        }

        [Test]
        public void TestOnTestItemSelectedWhenEventNotSetOnlyResetsSelectedItem([Values] bool isTestNull)
        {
            TestSuiteViewForTest page = new TestSuiteViewForTest();

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
            page.Test = test;

            ListViewStub list = new ListViewStub();
            list.SelectedItem = test;

            Assert.DoesNotThrow(() =>
            {
                page.InvokeOnTestItemSelected(list, new SelectedItemChangedEventArgs(test, 1));
            });

            Assert.That(list.SelectedItem, Is.Null);
        }

        [Test]
        public void TestOnTestItemSelectedWhenEventSetAndItemNotATestOnlyResetsSelectedItem([Values] bool isTestNull)
        {
            TestSuiteViewForTest page = new TestSuiteViewForTest();

            object test = isTestNull ? null : "test";

            ListViewStub list = new ListViewStub();
            list.SelectedItem = test;

            bool eventInvoked = false;
            page.TestItemSelected += (_, _) =>
            {
                eventInvoked = true;
            };

            page.InvokeOnTestItemSelected(list, new SelectedItemChangedEventArgs(test, 1));

            Assert.That(eventInvoked, Is.False);
            Assert.That(list.SelectedItem, Is.Null);
        }

        #endregion

        #region Tests for TestSuiteView_OnRunTestsClicked

        [Test]
        public void TestOnRunTestsClickedWhenEventSetInvokesEvent([Values] bool isTestNull)
        {
            TestSuiteViewForTest page = new TestSuiteViewForTest();

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
            page.Test = test;

            object eventSender = null;
            NUnitTestEventArgs eventArgs = null;
            page.RunTestsClicked += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            page.InvokeOnRunTestsClicked(this, new NUnitTestEventArgs(test));

            Assert.That(eventSender, Is.SameAs(this));
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Test, Is.SameAs(test));
        }

        [Test]
        public void TestOnRunTestsClickedWhenEventNotSetDoesNothing([Values] bool isTestNull)
        {
            TestSuiteViewForTest page = new TestSuiteViewForTest();

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
            page.Test = test;

            Assert.DoesNotThrow(() =>
            {
                page.InvokeOnRunTestsClicked(this, new NUnitTestEventArgs(test));
            });
        }

        #endregion

        #region Tests for TestSuiteView_OnSaveResultsClicked

        [Test]
        public void TestOnSaveResultsClickedWhenEventSetInvokesEvent([Values] bool isTestNull, [Values] bool isResultNull)
        {
            TestSuiteViewForTest page = new TestSuiteViewForTest();

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTestResult result = isTestNull || isResultNull ? null : new NUnitTestResult(new TestResultStub { Name = "test" });
            INUnitTest test = isTestNull ? null : new NUnitTest(testInstance, result);
            page.Test = test;

            object eventSender = null;
            NUnitTestResultEventArgs eventArgs = null;
            page.SaveResultsClicked += (sender, args) =>
            {
                eventSender = sender;
                eventArgs = args;
            };

            page.InvokeOnSaveResultsClicked(this, EventArgs.Empty);

            Assert.That(eventSender, Is.SameAs(this));
            Assert.That(eventArgs, Is.Not.Null);
            Assert.That(eventArgs.Result, Is.SameAs(result));
        }

        [Test]
        public void TestOnSaveResultsClickedWhenEventNotSetDoesNothing([Values] bool isTestNull, [Values] bool isResultNull)
        {
            TestSuiteViewForTest page = new TestSuiteViewForTest();

            ITest testInstance = new TestStub { Id = "123" };
            INUnitTestResult result = isResultNull ? null : new NUnitTestResult(new TestResultStub { Name = "test" });
            INUnitTest test = isTestNull ? null : new NUnitTest(testInstance, result);
            page.Test = test;

            Assert.DoesNotThrow(() =>
            {
                page.InvokeOnSaveResultsClicked(this, EventArgs.Empty);
            });
        }

        #endregion

        #region Nested Class: TestSuiteViewForTest

        /// <summary>
        ///     Extends TestSuiteView for use with tests.
        /// </summary>
        private class TestSuiteViewForTest : TestSuiteView
        {
            #region Members for Test

            /// <summary>
            ///     Holds the bindable property values of the class instance.
            /// </summary>
            private readonly Dictionary<BindableProperty, object> v_BindableProperties =
                new Dictionary<BindableProperty, object>();

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new TestSuiteViewForTest.
            /// </summary>
            public TestSuiteViewForTest() : base(false)
            {
            }

            #endregion

            #region Methods for Test

            /// <summary>
            ///     Invokes the <see cref="TestSuiteView.TestSuiteView_OnTestItemSelected"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="ListView"/> that contains the item.</param>
            /// <param name="e">The test selected event arguments.</param>
            public void InvokeOnTestItemSelected(object sender, SelectedItemChangedEventArgs e)
            {
                TestSuiteView_OnTestItemSelected(sender, e);
            }

            /// <summary>
            ///     Invokes the <see cref="TestSuiteView.TestSuiteView_OnRunTestsClicked"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
            /// <param name="e">The test run event arguments.</param>
            public void InvokeOnRunTestsClicked(object sender, NUnitTestEventArgs e)
            {
                TestSuiteView_OnRunTestsClicked(sender, e);
            }

            /// <summary>
            ///     Invokes the <see cref="TestSuiteView.TestSuiteView_OnSaveResultsClicked"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
            /// <param name="e">The export results event arguments.</param>
            public void InvokeOnSaveResultsClicked(object sender, EventArgs e)
            {
                TestSuiteView_OnSaveResultsClicked(sender, e);
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
            protected override void SetSelectedItem(object sender, object value)
            {
                ((ListViewStub)sender).SelectedItem = value;
            }

            #endregion
        }

        #endregion
    }
}