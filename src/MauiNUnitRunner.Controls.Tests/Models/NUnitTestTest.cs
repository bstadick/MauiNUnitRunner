// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.ComponentModel;
using MauiNUnitRunner.Controls.Models;
using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;

// ReSharper disable UseObjectOrCollectionInitializer
// ReSharper disable ArrangeObjectCreationWhenTypeEvident
// ReSharper disable AssignNullToNotNullAttribute

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class NUnitTestTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructor([Values] bool hasChildren, [Values] bool isChildrenNull,
        [Values] bool isNull, [Values] bool hasResult, [Values] bool isResultNull)
    {
        ITestResult resultInstance = new TestResultStub();
        INUnitTestResult result = isResultNull ? null : new NUnitTestResult(resultInstance);
        INUnitTestResult expectedResult = hasResult ? result : null;
        TestStub childTest = new TestStub();
        IList<ITest> children = hasChildren ? [childTest] : isChildrenNull ? null : Array.Empty<ITest>();
        TestStub testInstance = isNull ? null : new TestStub();
        if (!isNull)
        {
            testInstance.Tests = children;
        }

        IEnumerable<INUnitTest> expectedChildren = hasChildren && !isNull ? [new NUnitTest(childTest)] : Array.Empty<INUnitTest>();

        INUnitTest test = hasResult ? new NUnitTest(testInstance, result) : new NUnitTest(testInstance);

        Assert.That(test.Test, Is.EqualTo(testInstance));
        Assert.That(test.Children, Is.EqualTo(expectedChildren));
        Assert.That(test.Result, Is.SameAs(expectedResult));
    }

    #endregion

    #region Tests for Id Property

    [Test]
    public void TestIdProperty([Values] bool isTestNull, [Values] bool isIdNull)
    {
        string expected = isTestNull || isIdNull ? string.Empty : "test-123";
        TestStub testInstance = isTestNull ? null : new TestStub { Id = isIdNull ? null : "test-123" };

        NUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.Id, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestTestPropertyReturnsTest([Values] bool hasTest)
    {
        ITest testInstance = new TestSuite("suite-name");
        ITest expected = hasTest ? testInstance : null;

        INUnitTest test = new NUnitTest(expected);

        if (hasTest)
        {
            Assert.That(test.Test, Is.Not.Null);
            Assert.That(test.Test, Is.SameAs(expected));
        }
        else
        {
            Assert.That(test.Test, Is.Null);
        }
    }

    #endregion

    #region Tests for Result Property

    [Test]
    public void TestResultPropertyWhenSettingWithNullResultSetsValueNull()
    {
        TestStub testInstance = new TestStub { Id = "1" };

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.Result, Is.Null);

        test.Result = null;

        Assert.That(test.Result, Is.Null);
    }

    [Test]
    public void TestResultPropertyWhenSettingWithNullOrEmptyTestIdNullSetsValueToProvidedResult([Values] bool isIdNull, [Values] bool isResultNull)
    {
        TestStub testInstance = new TestStub { Id = isIdNull ? null : string.Empty };

        INUnitTest test = new NUnitTest(testInstance);
        INUnitTestResult result = isResultNull ? null : new NUnitTestResult(new TestResultStub { Test = testInstance });

        Assert.That(test.Result, Is.Null);

        test.Result = result;

        Assert.That(test.Result, Is.SameAs(result));
    }

    [Test]
    public void TestResultPropertyWhenSettingWithResultIdMatchingTestIdSetsValueToProvidedResult()
    {
        TestStub testInstance = new TestStub { Id = "1" };

        INUnitTest test = new NUnitTest(testInstance);
        INUnitTestResult result = new NUnitTestResult(new TestResultStub { Test = testInstance });

        Assert.That(test.Result, Is.Null);

        test.Result = result;

        Assert.That(test.Result, Is.SameAs(result));
    }

    [Test]
    public void TestResultPropertyWhenSettingWithResultIdMatchingInChildTestIdSetsValueToProvidedResult(
        [Values("1A", "1B", "1C", "2", "3")] string level)
    {
        TestStub testInstance1A = new TestStub { Id = "1A" };
        TestStub testInstance1B = new TestStub { Id = "1B" };
        TestStub testInstance1C = new TestStub { Id = "1C" };
        TestStub testInstance2 = new TestStub { Id = "2", Tests = [testInstance1A, testInstance1B, testInstance1C] };
        TestStub testInstance3 = new TestStub { Id = "3", Tests = [testInstance2] };

        INUnitTestResult result1A = new NUnitTestResult(new TestResultStub { Test = testInstance1A });
        INUnitTestResult result1B = new NUnitTestResult(new TestResultStub { Test = testInstance1B });
        INUnitTestResult result2 = new NUnitTestResult(new TestResultStub { Test = testInstance2, Children = [result1A.Result, result1B.Result] });
        INUnitTestResult result3 = new NUnitTestResult(new TestResultStub { Test = testInstance3, Children = [result2.Result] });

        INUnitTest test = null;
        INUnitTestResult expectedResult = null;
        switch (level)
        {
            case "1A":
                test = new NUnitTest(testInstance1A);
                expectedResult = result1A;
                break;
            case "1B":
                test = new NUnitTest(testInstance1B);
                expectedResult = result1B;
                break;
            case "1C":
                test = new NUnitTest(testInstance1C);
                expectedResult = result3;
                break;
            case "2":
                test = new NUnitTest(testInstance2);
                expectedResult = result2;
                break;
            case "3":
                test = new NUnitTest(testInstance3);
                expectedResult = result3;
                break;
        }

        Assert.That(test, Is.Not.Null);
        Assert.That(test.Result, Is.Null);

        test.Result = result3;

        Assert.That(expectedResult, Is.Not.Null);
        Assert.That(test.Result, Is.EqualTo(expectedResult));
        Assert.That(test.Result.Children, Is.EqualTo(expectedResult.Children));
    }

    [Test]
    public void TestResultPropertyWhenSettingWithResultIdMatchingTestIdAndChildTestButNoChildResultsSetsValueToProvidedResult()
    {
        TestStub testChild = new TestStub { Id = "2"};
        TestStub testInstance = new TestStub { Id = "1", Tests = [testChild] };

        INUnitTest test = new NUnitTest(testInstance);
        INUnitTestResult result = new NUnitTestResult(new TestResultStub { Test = testInstance });

        Assert.That(test.Result, Is.Null);

        test.Result = result;

        Assert.That(test.Result, Is.SameAs(result));
        Assert.That(test.Result.Children, Is.Empty);
    }

    [Test]
    public void TestResultPropertyWhenSettingWithResultIdMatchingTestIdAndChildResultsButNoChildTestSetsValueToProvidedResult()
    {
        TestStub testChild = new TestStub { Id = "2" };
        TestStub testInstance = new TestStub { Id = "1" };

        INUnitTest test = new NUnitTest(testInstance);
        INUnitTestResult resultChild = new NUnitTestResult(new TestResultStub { Test = testChild });
        INUnitTestResult result = new NUnitTestResult(new TestResultStub { Test = testInstance, Children = [resultChild.Result] });

        Assert.That(test.Result, Is.Null);

        test.Result = result;

        Assert.That(test.Result, Is.SameAs(result));
        Assert.That(test.Result.Children, Is.EqualTo(new List<INUnitTestResult> { resultChild }));
    }

    [Test]
    public void TestResultPropertyInvokesPropertyChangedEventWhenPropertyValueChanges()
    {
        INUnitTestResult result = new NUnitTestResult(new TestResultStub());

        TestPropertyChangedEventIsInvokedWhenPropertyChanges(result, (state, value) => state.Result = value,
            "Result", "TextColor");
    }

    #endregion

    #region Tests for HasTestResult Property

    [Test]
    public void TestHasTestResultPropertyWithNoResultReturnsFalse()
    {
        ITest testInstance = new TestSuite("suite-name");
        // ReSharper disable once RedundantArgumentDefaultValue
        INUnitTest test = new NUnitTest(testInstance, null);

        Assert.That(test.Result, Is.Null);
        Assert.That(test.HasTestResult, Is.False);
    }

    [Test]
    public void TestHasTestResultPropertyWithResultReturnsTrue()
    {
        ITest testInstance = new TestSuite("suite-name");
        ITestResult resultInstance = new TestResultStub();
        INUnitTestResult result = new NUnitTestResult(resultInstance);

        INUnitTest test = new NUnitTest(testInstance, result);

        Assert.That(test.Result, Is.Not.Null);
        Assert.That(test.HasTestResult, Is.True);
    }

    #endregion

    #region Tests for Children Property

    [Test]
    public void TestChildrenPropertyReturnsChildren([Values] bool hasChildren, [Values] bool isChildrenNull,
        [Values] bool isNull)
    {
        TestStub childTest = new TestStub();
        IList<ITest> children = hasChildren ? [childTest] : isChildrenNull ? null : Array.Empty<ITest>();
        TestStub testInstance = isNull ? null : new TestStub();
        if (!isNull)
        {
            testInstance.Tests = children;
        }

        IEnumerable<INUnitTest> expected = hasChildren && !isNull ? [new NUnitTest(childTest)] : Array.Empty<INUnitTest>();

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.Children, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for HasChildren Property

    [Test]
    public void TestHasChildrenPropertyReturnsIfTestHasChildren([Values] bool hasChildren, [Values] bool isChildrenNull,
        [Values] bool isNull)
    {
        bool expected = hasChildren && !isNull;
        TestStub childTest = new TestStub();
        IList<ITest> children = hasChildren ? [childTest] : isChildrenNull ? null : Array.Empty<ITest>();
        TestStub testInstance = isNull ? null : new TestStub();
        if (!isNull)
        {
            testInstance.Tests = children;
        }

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.HasChildren, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for HasNoChildren Property

    [Test]
    public void TestHasNoChildrenPropertyReturnsIfTestHasNoChildren([Values] bool hasChildren,
        [Values] bool isChildrenNull, [Values] bool isNull)
    {
        bool expected = hasChildren && !isNull;
        TestStub childTest = new TestStub();
        IList<ITest> children = hasChildren ? [childTest] : isChildrenNull ? null : Array.Empty<ITest>();
        TestStub testInstance = isNull ? null : new TestStub();
        if (!isNull)
        {
            testInstance.Tests = children;
        }

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.HasNoChildren, Is.Not.EqualTo(expected));
    }

    #endregion

    #region Tests for TextColor Property

    [Test]
    public void TestTextColorPropertyWithNullResultReturnsColorBlack([Values] bool hasResult)
    {
        ITest testInstance = new TestSuite("suite-name");
        TestResultStub resultInstance = new TestResultStub();
        resultInstance.ResultState = null;
        INUnitTestResult result = hasResult ? new NUnitTestResult(resultInstance) : null;

        INUnitTest test = new NUnitTest(testInstance, result);

        Assert.That(test.TextColor, Is.EqualTo(Colors.Black));
    }

    [Test]
    public void TestTextColorPropertyWithNotSupportedTestStatusReturnsColorBlack()
    {
        ITest testInstance = new TestSuite("suite-name");
        TestResultStub result = new TestResultStub();
        result.ResultState = new ResultState((TestStatus)(-1));

        INUnitTest test = new NUnitTest(testInstance, new NUnitTestResult(result));

        Assert.That(test.TextColor, Is.EqualTo(Colors.Black));
    }

    [Test]
    public void TestTextColorPropertyReturnsColorForResultState([Values] TestStatus status)
    {
        ITest testInstance = new TestSuite("suite-name");
        TestResultStub result = new TestResultStub();
        result.ResultState = new ResultState((TestStatus)(-1));

        Color expected = Colors.Black;
        ResultState state = ResultState.NotRunnable;
        switch (status)
        {
            case TestStatus.Inconclusive:
                state = ResultState.Inconclusive;
                expected = Colors.MediumPurple;
                break;
            case TestStatus.Skipped:
                state = ResultState.Ignored;
                expected = Colors.DodgerBlue;
                break;
            case TestStatus.Passed:
                state = ResultState.Success;
                expected = Colors.LimeGreen;
                break;
            case TestStatus.Warning:
                state = ResultState.Warning;
                expected = Colors.Orange;
                break;
            case TestStatus.Failed:
                state = ResultState.Failure;
                expected = Colors.Red;
                break;
            default:
                Assert.Fail($"This status {status} is not supported.");
                break;
        }

        result.ResultState = state;

        INUnitTest test = new NUnitTest(testInstance, new NUnitTestResult(result));

        Assert.That(test.TextColor, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for DisplayName Property

    [Test]
    public void TestDisplayNameProperty([Values] bool isTestNull, [Values] bool isNameNull)
    {
        string expected = isTestNull || isNameNull ? string.Empty : "test-123";
        TestStub testInstance = isTestNull ? null : new TestStub { Name = isNameNull ? null : "test-123" };

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.DisplayName, Is.EqualTo(expected));
    }

    [Test]
    public void TestDisplayNamePropertyWithDllExtensionReturnsAssemblyDisplayName()
    {
        const string expected = "test-123 Assembly";
        TestStub testInstance = new TestStub { Name = "path\\to\\dll\\test-123.dll" };

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.DisplayName, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for FullDisplayName Property

    [Test]
    public void TestFullDisplayNameProperty([Values] bool isTestNull, [Values] bool isNameNull)
    {
        string expected = isTestNull || isNameNull ? string.Empty : "test-123";
        TestStub testInstance = isTestNull ? null : new TestStub { FullName = isNameNull ? null : "test-123" };

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.FullDisplayName, Is.EqualTo(expected));
    }

    [Test]
    public void TestFullDisplayNamePropertyWithDllExtensionReturnsAssemblyDisplayName()
    {
        const string expected = "test-123 Assembly";
        TestStub testInstance = new TestStub { FullName = "path\\to\\dll\\test-123.dll" };

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.FullDisplayName, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for ConditionalDisplayName Property

    [Test]
    public void TestConditionalDisplayNameProperty([Values] bool isTestNull, [Values] bool isNameNull,
        [Values] bool isFullNameNull, [Values] bool isSuite, [Values] bool isClassNull, [Values] bool isMethodNull)
    {
        string expected = string.Empty;
        if (!isTestNull)
        {
            expected = isSuite && isClassNull && isMethodNull
                ? (isFullNameNull ? string.Empty : "test-123-full")
                : (isNameNull ? string.Empty : "test-123");
        }

        IMethodInfo methodInfo = new MethodWrapper(typeof(NUnitTestTest), nameof(TestConditionalDisplayNameProperty));
        TestStub testInstance = isTestNull
            ? null
            : new TestStub
            {
                Name = isNameNull ? null : "test-123",
                FullName = isFullNameNull ? null : "test-123-full",
                IsSuite = isSuite,
                ClassName = isClassNull ? null : "class-name",
                Method = isMethodNull ? null : methodInfo
            };

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.ConditionalDisplayName, Is.EqualTo(expected));
    }

    [Test]
    public void TestConditionalDisplayNamePropertyWithDllExtensionReturnsAssemblyDisplayName([Values] bool isSuite,
        [Values] bool isClassNull, [Values] bool isMethodNull)
    {
        string expected = isSuite && isClassNull && isMethodNull ? "test-123-full Assembly" : "test-123 Assembly";
        IMethodInfo methodInfo = new MethodWrapper(typeof(NUnitTestTest), nameof(TestConditionalDisplayNameProperty));
        TestStub testInstance = new TestStub
        {
            Name = "path\\to\\dll\\test-123.dll",
            FullName = "path\\to\\dll\\test-123-full.dll",
            IsSuite = isSuite,
            ClassName = isClassNull ? null : "class-name",
            Method = isMethodNull ? null : methodInfo
        };

        INUnitTest test = new NUnitTest(testInstance);

        Assert.That(test.ConditionalDisplayName, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for SkipSingleTestSuites

    [Test]
    public void TestSkipSingleTestSuites([Values] bool hasChildren, [Values] bool hasMultipleChildren, [Values] bool isChildrenNull)
    {
        TestStub childTest = new TestStub { Id = "child" };
        TestStub grandchildTest = new TestStub { Id = "grandchild" };
        childTest.Tests = [grandchildTest];
        IList<ITest> children = hasChildren
            ? (hasMultipleChildren ? [childTest, childTest] : (isChildrenNull ? [null] : [childTest]))
            : Array.Empty<ITest>();
        TestStub testInstance = new TestStub() { Id = "parent" };
        testInstance.Tests = children;

        INUnitTest test = new NUnitTest(testInstance);

        INUnitTest expected =
            !hasChildren || hasMultipleChildren || isChildrenNull ? test : new NUnitTest(grandchildTest);

        INUnitTest skippedTest = test.SkipSingleTestSuites();

        Assert.That(skippedTest, Is.Not.Null);
        Assert.That(skippedTest.Id, Is.Not.Null.And.Not.Empty);
        Assert.That(skippedTest.Id, Is.EqualTo(expected.Id));
    }

    #endregion

    #region Tests for INotifyPropertyChanged

    [Test]
    public void TestPropertyChangedEventIsInvokedWhenResultPropertyChanges()
    {
        INUnitTestResult result = new NUnitTestResult(new TestResultStub());

        // Use an existing property to test the event is invoked
        TestPropertyChangedEventIsInvokedWhenPropertyChanges(result, (state, value) => state.Result = value,
            "Result", "TextColor");
    }

    [Test]
    public void TestPropertyChangedEventWhenEventNotSetAndResultPropertyChangedDoesNotThrowException()
    {
        INUnitTest test = new NUnitTest(new TestStub());

        Assert.DoesNotThrow(() => { test.Result = new NUnitTestResult(new TestResultStub()); });
    }

    #endregion

    #region Tests for Equals

    [Test]
    public void TestEqualsWithSameTestReturnsTrue()
    {
        TestStub testInstanceOne = new TestStub();

        INUnitTest testOne = new NUnitTest(testInstanceOne);
        INUnitTest testNull = new NUnitTest(null);

        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.That(testOne.Equals(testInstanceOne), Is.True);
        // ReSharper disable once EqualExpressionComparison
        Assert.That(testOne.Equals(testOne), Is.True);
        // ReSharper disable once EqualExpressionComparison
        Assert.That(testNull.Equals(testNull), Is.True);
    }

    [Test]
    public void TestEqualsWithNotSameTestReturnsFalse([Values] bool isNull)
    {
        TestStub testInstanceOne = new TestStub();
        TestStub testInstanceTwo = new TestStub();
        testInstanceTwo.Name = "test-name";
        ITest testInputOne = isNull ? null : testInstanceOne;
        ITest testInputTwo = isNull ? testInstanceTwo : null;
        object testWrong = "string";

        INUnitTest testOne = new NUnitTest(testInputOne);
        INUnitTest testTwo = new NUnitTest(testInputTwo);
        INUnitTest testNull = new NUnitTest(null);

        // ReSharper disable once SuspiciousTypeConversion.Global
        Assert.That(testOne.Equals(testInputTwo), Is.False);
        Assert.That(testOne.Equals(testTwo), Is.False);
        Assert.That(testOne.Equals(testNull), Is.False);
        Assert.That(testOne.Equals(testWrong), Is.False);
        Assert.That(testOne.Equals(null), Is.False);
        Assert.That(testNull.Equals(null), Is.False);
    }

    #endregion

    #region Tests for GetHashCode

    [Test]
    public void TestGetHashCode([Values] bool isNull)
    {
        TestStub testInstanceOne = new TestStub();
        int hashCode = isNull ? 0 : testInstanceOne.GetHashCode();
        ITest testInputOne = isNull ? null : testInstanceOne;

        INUnitTest testOne = new NUnitTest(testInputOne);

        Assert.That(testOne.GetHashCode(), Is.EqualTo(hashCode));
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Tests the property changed event for generic properties.
    /// </summary>
    /// <typeparam name="T">The type of the property value being changed.</typeparam>
    /// <param name="value">The value to set, should be different from the default value.</param>
    /// <param name="changePropertyValue">The action to change the property value.</param>
    /// <param name="propertyNames">The name of the property changed events that are invoked.</param>
    private static void TestPropertyChangedEventIsInvokedWhenPropertyChanges<T>(T value,
        Action<INUnitTest, T> changePropertyValue, params string[] propertyNames)
    {
        INUnitTest test = new NUnitTest(new TestStub());

        List<Tuple<INUnitTest, PropertyChangedEventArgs>> eventArgs =
            new List<Tuple<INUnitTest, PropertyChangedEventArgs>>();
        test.PropertyChanged += (sender, args) =>
            eventArgs.Add(new Tuple<INUnitTest, PropertyChangedEventArgs>(sender as INUnitTest, args));

        changePropertyValue.Invoke(test, value);

        Assert.That(eventArgs, Has.Count.EqualTo(propertyNames.Length));
        for (int i = 0; i < propertyNames.Length; i++)
        {
            Assert.That(eventArgs[i].Item1, Is.SameAs(test));
            Assert.That(eventArgs[i].Item2, Is.Not.Null);
            Assert.That(eventArgs[i].Item2.PropertyName, Is.EqualTo(propertyNames[i]));
        }
    }

    #endregion
}