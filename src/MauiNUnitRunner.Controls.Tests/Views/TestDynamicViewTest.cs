// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Views;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

// Ignore Spelling: Bindable
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace MauiNUnitRunner.Controls.Tests.Views;

[TestFixture]
public class TestDynamicViewTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructor()
    {
        TestDynamicView page = new TestDynamicViewForTest();

        Assert.That(page.Test, Is.Null);
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestTestProperty()
    {
        TestDynamicView page = new TestDynamicViewForTest();

        Assert.That(TestDynamicView.TestProperty.PropertyName, Is.EqualTo("Test"));
        Assert.That(TestDynamicView.TestProperty.DeclaringType, Is.EqualTo(typeof(TestDynamicView)));
        Assert.That(TestDynamicView.TestProperty.ReturnType, Is.EqualTo(typeof(INUnitTest)));
        Assert.That(TestDynamicView.TestProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
        Assert.That(TestDynamicView.TestProperty.DefaultValue, Is.Null);
        Assert.That(TestDynamicView.TestProperty.IsReadOnly, Is.False);

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
        TestDynamicView page = new TestDynamicViewForTest();

        Assert.That(TestDynamicView.IsTestRunningProperty.PropertyName, Is.EqualTo("IsTestRunning"));
        Assert.That(TestDynamicView.IsTestRunningProperty.DeclaringType, Is.EqualTo(typeof(TestDynamicView)));
        Assert.That(TestDynamicView.IsTestRunningProperty.ReturnType, Is.EqualTo(typeof(bool)));
        Assert.That(TestDynamicView.IsTestRunningProperty.DefaultBindingMode, Is.EqualTo(BindingMode.TwoWay));
        Assert.That(TestDynamicView.IsTestRunningProperty.DefaultValue, Is.False);
        Assert.That(TestDynamicView.IsTestRunningProperty.IsReadOnly, Is.False);

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

    #region Tests for TestDynamicView_OnTestItemSelected

    [Test]
    public void TestOnTestItemSelectedWhenEventSetInvokesEvent([Values] bool isTestNull)
    {
        TestDynamicViewForTest page = new TestDynamicViewForTest();

        ITest testInstance = new TestStub { Id = "123" };
        INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
        page.Test = test;

        object eventSender = null;
        NUnitTestEventArgs eventArgs = null;
        page.TestItemSelected += (sender, args) =>
        {
            eventSender = sender;
            eventArgs = args;
        };

        page.InvokeOnTestItemSelected(this, new NUnitTestEventArgs(test));

        Assert.That(eventSender, Is.SameAs(this));
        Assert.That(eventArgs, Is.Not.Null);
        Assert.That(eventArgs.Test, Is.SameAs(test));
    }

    [Test]
    public void TestOnTestItemSelectedWhenEventNotSetDoesNothing([Values] bool isTestNull)
    {
        TestDynamicViewForTest page = new TestDynamicViewForTest();

        ITest testInstance = new TestStub { Id = "123" };
        INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
        page.Test = test;

        Assert.DoesNotThrow(() =>
        {
            page.InvokeOnTestItemSelected(this, new NUnitTestEventArgs(test));
        });
    }

    #endregion

    #region Tests for TestDynamicView_OnRunTestsClicked

    [Test]
    public void TestOnRunTestsClickedWhenEventSetInvokesEvent([Values] bool isTestNull)
    {
        TestDynamicViewForTest page = new TestDynamicViewForTest();

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
        TestDynamicViewForTest page = new TestDynamicViewForTest();

        ITest testInstance = new TestStub { Id = "123" };
        INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
        page.Test = test;

        Assert.DoesNotThrow(() =>
        {
            page.InvokeOnRunTestsClicked(this, new NUnitTestEventArgs(test));
        });
    }

    #endregion

    #region Tests for TestDynamicView_OnSaveResultsClicked

    [Test]
    public void TestOnSaveResultsClickedWhenEventSetInvokesEvent([Values] bool isTestNull, [Values] bool isResultNull)
    {
        TestDynamicViewForTest page = new TestDynamicViewForTest();

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
        TestDynamicViewForTest page = new TestDynamicViewForTest();

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

    #region Nested Class: TestDynamicViewForTest

    /// <summary>
    ///     Extends TestDynamicView for use with tests.
    /// </summary>
    private class TestDynamicViewForTest : TestDynamicView
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
        ///     Initializes a new TestDynamicViewForTest.
        /// </summary>
        public TestDynamicViewForTest() : base(false)
        {
        }

        #endregion

        #region Methods for Test

        /// <summary>
        ///     Invokes the <see cref="TestDynamicView.TestDynamicView_OnTestItemSelected"/> method.
        /// </summary>
        /// <param name="sender">The <see cref="ListView"/> that contains the item.</param>
        /// <param name="e">The test selected event arguments.</param>
        public void InvokeOnTestItemSelected(object sender, NUnitTestEventArgs e)
        {
            TestDynamicView_OnTestItemSelected(sender, e);
        }

        /// <summary>
        ///     Invokes the <see cref="TestDynamicView.TestDynamicView_OnRunTestsClicked"/> method.
        /// </summary>
        /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
        /// <param name="e">The test run event arguments.</param>
        public void InvokeOnRunTestsClicked(object sender, NUnitTestEventArgs e)
        {
            TestDynamicView_OnRunTestsClicked(sender, e);
        }

        /// <summary>
        ///     Invokes the <see cref="TestDynamicView.TestDynamicView_OnSaveResultsClicked"/> method.
        /// </summary>
        /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
        /// <param name="e">The export results event arguments.</param>
        public void InvokeOnSaveResultsClicked(object sender, EventArgs e)
        {
            TestDynamicView_OnSaveResultsClicked(sender, e);
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

        #endregion
    }

    #endregion
}