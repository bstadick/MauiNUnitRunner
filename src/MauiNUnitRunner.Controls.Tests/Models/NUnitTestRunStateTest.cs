// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using NUnit.Framework;
using System.ComponentModel;
using MauiNUnitRunner.Controls.Services;

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class NUnitTestRunStateTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructor()
    {
        INUnitTestRunner runner = new NUnitTestRunner();

        INUnitTestRunState state = new NUnitTestRunState(runner);

        Assert.That(state.TestRunner, Is.SameAs(runner));
    }

    [Test]
    public void TestConstructorThrowsArgumentNullExceptionWhenTestRunnerIsNull()
    {
        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The testRunner cannot be null. (Parameter 'testRunner')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitTestRunState(null));
    }

    #endregion

    #region Tests for TestRunner Property

    [Test]
    public void TestTestRunnerProperty()
    {
        INUnitTestRunner runner = new NUnitTestRunner();

        INUnitTestRunState state = new NUnitTestRunState(runner);

        Assert.That(state.TestRunner, Is.SameAs(runner));
    }

    #endregion

    #region Tests for IsTestRunning Property

    [Test]
    public void TestIsTestRunningProperty()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        Assert.That(state.IsTestRunning, Is.False);

        state.IsTestRunning = true;

        Assert.That(state.IsTestRunning, Is.True);

        state.IsTestRunning = true;

        Assert.That(state.IsTestRunning, Is.True);

        state.IsTestRunning = false;

        Assert.That(state.IsTestRunning, Is.False);
    }

    #endregion

    #region Tests for TestRunCount Property

    [Test]
    public void TestTestRunCountProperty()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        Assert.That(state.TestRunCount, Is.EqualTo(0));

        state.TestRunCount = 10;

        Assert.That(state.TestRunCount, Is.EqualTo(10));

        state.TestRunCount = 10;

        Assert.That(state.TestRunCount, Is.EqualTo(10));

        state.TestRunCount = 0;

        Assert.That(state.TestRunCount, Is.EqualTo(0));
    }

    #endregion

    #region Tests for TestRunStartedCount Property

    [Test]
    public void TestTestRunStartedCountProperty()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        Assert.That(state.TestRunStartedCount, Is.EqualTo(0));

        state.TestRunStartedCount = 10;

        Assert.That(state.TestRunStartedCount, Is.EqualTo(10));

        state.TestRunStartedCount = 10;

        Assert.That(state.TestRunStartedCount, Is.EqualTo(10));

        state.TestRunStartedCount = 0;

        Assert.That(state.TestRunStartedCount, Is.EqualTo(0));
    }

    #endregion

    #region Tests for TestRunFinishedCount Property

    [Test]
    public void TestTestRunFinishedCountProperty()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));

        state.TestRunFinishedCount = 10;

        Assert.That(state.TestRunFinishedCount, Is.EqualTo(10));

        state.TestRunFinishedCount = 10;

        Assert.That(state.TestRunFinishedCount, Is.EqualTo(10));

        state.TestRunFinishedCount = 0;

        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
    }

    #endregion

    #region Tests for TestRunProgress Property

    [Test]
    public void TestTestRunProgressProperty()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));

        state.TestRunCount = 10;

        Assert.That(state.TestRunCount, Is.EqualTo(10));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));

        state.TestRunCount = 0;
        state.TestRunFinishedCount = 10;

        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(10));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));

        state.TestRunCount = 10;
        state.TestRunFinishedCount = 5;

        Assert.That(state.TestRunCount, Is.EqualTo(10));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(5));
        Assert.That(state.TestRunProgress, Is.EqualTo(0.5));

        state.TestRunCount = 0;
        state.TestRunFinishedCount = 0;

        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));
    }

    #endregion

    #region Tests for Reset

    [Test]
    public void TestReset()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        Assert.That(state.IsTestRunning, Is.False);
        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));

        state.Reset();

        Assert.That(state.IsTestRunning, Is.False);
        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));

        state.IsTestRunning = true;
        state.TestRunCount = 10;
        state.TestRunStartedCount = 7;
        state.TestRunFinishedCount = 5;

        Assert.That(state.IsTestRunning, Is.True);
        Assert.That(state.TestRunCount, Is.EqualTo(10));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(7));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(5));
        Assert.That(state.TestRunProgress, Is.EqualTo(0.5));

        state.Reset();

        Assert.That(state.IsTestRunning, Is.False);
        Assert.That(state.TestRunCount, Is.EqualTo(0));
        Assert.That(state.TestRunStartedCount, Is.EqualTo(0));
        Assert.That(state.TestRunFinishedCount, Is.EqualTo(0));
        Assert.That(state.TestRunProgress, Is.EqualTo(0));
    }

    #endregion

    #region Tests for INotifyPropertyChanged

    [Test]
    public void TestPropertyChangedEventIsInvokedWhenIsTestRunningPropertyChanges()
    {
        TestPropertyChangedEventIsInvokedWhenPropertyChanges(true, false, (state, value) => state.IsTestRunning = value,
            "IsTestRunning");
    }

    [Test]
    public void TestPropertyChangedEventIsInvokedWhenTestRunCountPropertyChanges()
    {
        TestPropertyChangedEventIsInvokedWhenPropertyChanges(10, 0, (state, value) => state.TestRunCount = value,
            "TestRunCount", "TestRunProgress");
    }

    [Test]
    public void TestPropertyChangedEventIsInvokedWhenTestRunStartedCountPropertyChanges()
    {
        TestPropertyChangedEventIsInvokedWhenPropertyChanges(10, 0, (state, value) => state.TestRunStartedCount = value,
            "TestRunStartedCount");
    }

    [Test]
    public void TestPropertyChangedEventIsInvokedWhenTestRunFinishedCountPropertyChanges()
    {
        TestPropertyChangedEventIsInvokedWhenPropertyChanges(10, 0, (state, value) => state.TestRunFinishedCount = value,
            "TestRunFinishedCount", "TestRunProgress");
    }

    private static void TestPropertyChangedEventIsInvokedWhenPropertyChanges<T>(T firstValue, T secondValue,
        Action<INUnitTestRunState, T> changePropertyValue, params string[] propertyNames)
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        List<Tuple<INUnitTestRunState, PropertyChangedEventArgs>> eventArgs =
            new List<Tuple<INUnitTestRunState, PropertyChangedEventArgs>>();
        state.PropertyChanged += (sender, args) =>
            eventArgs.Add(new Tuple<INUnitTestRunState, PropertyChangedEventArgs>(sender as INUnitTestRunState, args));

        changePropertyValue.Invoke(state, firstValue);

        Assert.That(eventArgs, Has.Count.EqualTo(propertyNames.Length));
        for (int i = 0; i < propertyNames.Length; i++)
        {
            Assert.That(eventArgs[i].Item1, Is.SameAs(state));
            Assert.That(eventArgs[i].Item2, Is.Not.Null);
            Assert.That(eventArgs[i].Item2.PropertyName, Is.EqualTo(propertyNames[i]));
        }

        eventArgs.Clear();

        changePropertyValue.Invoke(state, secondValue);

        Assert.That(eventArgs, Has.Count.EqualTo(propertyNames.Length));
        for (int i = 0; i < propertyNames.Length; i++)
        {
            Assert.That(eventArgs[i].Item1, Is.SameAs(state));
            Assert.That(eventArgs[i].Item2, Is.Not.Null);
            Assert.That(eventArgs[i].Item2.PropertyName, Is.EqualTo(propertyNames[i]));
        }

        eventArgs.Clear();

        // Setting to same value as current value results in no event raised
        changePropertyValue.Invoke(state, secondValue);

        Assert.That(eventArgs, Is.Empty);
    }

    [Test]
    public void TestPropertyChangedEventWhenEventNotSetAndPropertyChangedDoesNotThrowException()
    {
        INUnitTestRunState state = new NUnitTestRunState(new NUnitTestRunner());

        Assert.DoesNotThrow(() => { state.IsTestRunning = true; });
    }

    #endregion
}