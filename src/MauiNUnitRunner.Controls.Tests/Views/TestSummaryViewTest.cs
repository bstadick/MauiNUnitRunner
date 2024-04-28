// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Services;
using MauiNUnitRunner.Controls.Views;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

// Ignore Spelling: Bindable
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace MauiNUnitRunner.Controls.Tests.Views;

[TestFixture]
public class TestSummaryViewTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructor()
    {
        TestSummaryView page = new TestSummaryViewForTest();

        Assert.That(page.Test, Is.Null);
        Assert.That(page.ShowTestButtons, Is.True);
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestTestProperty()
    {
        TestSummaryView page = new TestSummaryViewForTest();

        Assert.That(TestSummaryView.TestProperty.PropertyName, Is.EqualTo("Test"));
        Assert.That(TestSummaryView.TestProperty.DeclaringType, Is.EqualTo(typeof(TestSummaryView)));
        Assert.That(TestSummaryView.TestProperty.ReturnType, Is.EqualTo(typeof(INUnitTest)));
        Assert.That(TestSummaryView.TestProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
        Assert.That(TestSummaryView.TestProperty.DefaultValue, Is.Null);
        Assert.That(TestSummaryView.TestProperty.IsReadOnly, Is.False);

        Assert.That(page.Test, Is.Null);

        ITest testInstance = new TestStub { Id = "123" };
        INUnitTest test = new NUnitTest(testInstance);

        page.Test = test;

        Assert.That(page.Test, Is.SameAs(test));
    }

    #endregion

    #region Tests for TestRunState Property

    [Test]
    public void TestTestRunStateProperty()
    {
        TestSummaryView page = new TestSummaryViewForTest();

        Assert.That(TestSummaryView.TestRunStateProperty.PropertyName, Is.EqualTo("TestRunState"));
        Assert.That(TestSummaryView.TestRunStateProperty.DeclaringType, Is.EqualTo(typeof(TestSummaryView)));
        Assert.That(TestSummaryView.TestRunStateProperty.ReturnType, Is.EqualTo(typeof(INUnitTestRunState)));
        Assert.That(TestSummaryView.TestRunStateProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
        Assert.That(TestSummaryView.TestRunStateProperty.DefaultValue, Is.Null);
        Assert.That(TestSummaryView.TestRunStateProperty.IsReadOnly, Is.False);

        Assert.That(page.TestRunState, Is.Null);

        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        page.TestRunState = state;

        Assert.That(page.TestRunState, Is.SameAs(state));
    }

    #endregion

    #region Tests for ShowTestButtons Property

    [Test]
    public void TestShowTestButtonsProperty()
    {
        TestSummaryView page = new TestSummaryViewForTest();

        Assert.That(TestSummaryView.ShowTestButtonsProperty.PropertyName, Is.EqualTo("ShowTestButtons"));
        Assert.That(TestSummaryView.ShowTestButtonsProperty.DeclaringType, Is.EqualTo(typeof(TestSummaryView)));
        Assert.That(TestSummaryView.ShowTestButtonsProperty.ReturnType, Is.EqualTo(typeof(bool)));
        Assert.That(TestSummaryView.ShowTestButtonsProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
        Assert.That(TestSummaryView.ShowTestButtonsProperty.DefaultValue, Is.True);
        Assert.That(TestSummaryView.ShowTestButtonsProperty.IsReadOnly, Is.False);

        Assert.That(page.ShowTestButtons, Is.True);

        page.ShowTestButtons = true;

        Assert.That(page.ShowTestButtons, Is.True);

        page.ShowTestButtons = false;

        Assert.That(page.ShowTestButtons, Is.False);

        page.ShowTestButtons = false;

        Assert.That(page.ShowTestButtons, Is.False);

        page.ShowTestButtons = true;

        Assert.That(page.ShowTestButtons, Is.True);
    }

    #endregion

    #region Tests for RunTestsButton_OnClicked

    [Test]
    public void TestRunTestsButtonOnClickedWhenEventSetInvokesEvent([Values] bool isTestNull)
    {
        TestSummaryViewForTest page = new TestSummaryViewForTest();

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

        page.InvokeRunTestsButtonOnClicked(this, EventArgs.Empty);

        Assert.That(eventSender, Is.SameAs(this));
        Assert.That(eventArgs, Is.Not.Null);
        Assert.That(eventArgs.Test, Is.SameAs(test));
    }

    [Test]
    public void TestRunTestsButtonOnClickedWhenEventNotSetDoesNothing([Values] bool isTestNull)
    {
        TestSummaryViewForTest page = new TestSummaryViewForTest();

        ITest testInstance = new TestStub { Id = "123" };
        INUnitTest test = isTestNull ? null : new NUnitTest(testInstance);
        page.Test = test;

        Assert.DoesNotThrow(() =>
        {
            page.InvokeRunTestsButtonOnClicked(this, EventArgs.Empty);
        });
    }

    #endregion

    #region Tests for SaveResultsButton_OnClicked

    [Test]
    public void TestSaveResultsButtonOnClickedWhenEventSetInvokesEvent([Values] bool isTestNull, [Values] bool isResultNull)
    {
        TestSummaryViewForTest page = new TestSummaryViewForTest();

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

        page.InvokeSaveResultsButtonOnClicked(this, EventArgs.Empty);

        Assert.That(eventSender, Is.SameAs(this));
        Assert.That(eventArgs, Is.Not.Null);
        Assert.That(eventArgs.Result, Is.SameAs(result));
    }

    [Test]
    public void TestSaveResultsButtonOnClickedWhenEventNotSetDoesNothing([Values] bool isTestNull, [Values] bool isResultNull)
    {
        TestSummaryViewForTest page = new TestSummaryViewForTest();

        ITest testInstance = new TestStub { Id = "123" };
        INUnitTestResult result = isResultNull ? null : new NUnitTestResult(new TestResultStub { Name = "test" });
        INUnitTest test = isTestNull ? null : new NUnitTest(testInstance, result);
        page.Test = test;

        Assert.DoesNotThrow(() =>
        {
            page.InvokeSaveResultsButtonOnClicked(this, EventArgs.Empty);
        });
    }

    #endregion

    #region Nested Class: TestSummaryViewForTest

    /// <summary>
    ///     Extends TestSummaryView for use with tests.
    /// </summary>
    private class TestSummaryViewForTest : TestSummaryView
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
        ///     Initializes a new TestSummaryViewForTest.
        /// </summary>
        public TestSummaryViewForTest() : base(false)
        {
        }

        #endregion

        #region Methods for Test

        /// <summary>
        ///     Invokes the <see cref="TestSummaryView.RunTestsButton_OnClicked"/> method.
        /// </summary>
        /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
        /// <param name="e">The button click event arguments.</param>
        public void InvokeRunTestsButtonOnClicked(object sender, EventArgs e)
        {
            RunTestsButton_OnClicked(sender, e);
        }

        /// <summary>
        ///     Invokes the <see cref="TestSummaryView.SaveResultsButton_OnClicked"/> method.
        /// </summary>
        /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
        /// <param name="e">The button click event arguments.</param>
        public void InvokeSaveResultsButtonOnClicked(object sender, EventArgs e)
        {
            SaveResultsButton_OnClicked(sender, e);
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