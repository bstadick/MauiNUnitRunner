// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Views;
using NUnit.Framework;
using NUnit.Framework.Interfaces;

// Ignore Spelling: Bindable
// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace MauiNUnitRunner.Controls.Tests.Views;

[TestFixture]
public class TestDetailViewTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructor()
    {
        TestDetailView page = new TestDetailViewForTest();

        Assert.That(page.Test, Is.Null);
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestTestProperty()
    {
        TestDetailView page = new TestDetailViewForTest();

        Assert.That(TestDetailView.TestProperty.PropertyName, Is.EqualTo("Test"));
        Assert.That(TestDetailView.TestProperty.DeclaringType, Is.EqualTo(typeof(TestDetailView)));
        Assert.That(TestDetailView.TestProperty.ReturnType, Is.EqualTo(typeof(INUnitTest)));
        Assert.That(TestDetailView.TestProperty.DefaultBindingMode, Is.EqualTo(BindingMode.OneWay));
        Assert.That(TestDetailView.TestProperty.DefaultValue, Is.Null);
        Assert.That(TestDetailView.TestProperty.IsReadOnly, Is.False);

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
        TestDetailView page = new TestDetailViewForTest();

        Assert.That(TestDetailView.IsTestRunningProperty.PropertyName, Is.EqualTo("IsTestRunning"));
        Assert.That(TestDetailView.IsTestRunningProperty.DeclaringType, Is.EqualTo(typeof(TestDetailView)));
        Assert.That(TestDetailView.IsTestRunningProperty.ReturnType, Is.EqualTo(typeof(bool)));
        Assert.That(TestDetailView.IsTestRunningProperty.DefaultBindingMode, Is.EqualTo(BindingMode.TwoWay));
        Assert.That(TestDetailView.IsTestRunningProperty.DefaultValue, Is.False);
        Assert.That(TestDetailView.IsTestRunningProperty.IsReadOnly, Is.False);

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

    #region Tests for RunTestsButton_OnClicked

    [Test]
    public void TestRunTestsButtonOnClickedWhenEventSetInvokesEvent([Values] bool isTestNull)
    {
        TestDetailViewForTest page = new TestDetailViewForTest();

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
        TestDetailViewForTest page = new TestDetailViewForTest();

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
        TestDetailViewForTest page = new TestDetailViewForTest();

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
        TestDetailViewForTest page = new TestDetailViewForTest();

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

    #region Nested Class: TestDetailViewForTest

    /// <summary>
    ///     Extends TestDetailView for use with tests.
    /// </summary>
    private class TestDetailViewForTest : TestDetailView
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
        ///     Initializes a new TestDetailViewForTest.
        /// </summary>
        public TestDetailViewForTest() : base(false)
        {
        }

        #endregion

        #region Methods for Test

        /// <summary>
        ///     Invokes the <see cref="TestDetailView.RunTestsButton_OnClicked"/> method.
        /// </summary>
        /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
        /// <param name="e">The button click event arguments.</param>
        public void InvokeRunTestsButtonOnClicked(object sender, EventArgs e)
        {
            RunTestsButton_OnClicked(sender, e);
        }

        /// <summary>
        ///     Invokes the <see cref="TestDetailView.SaveResultsButton_OnClicked"/> method.
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