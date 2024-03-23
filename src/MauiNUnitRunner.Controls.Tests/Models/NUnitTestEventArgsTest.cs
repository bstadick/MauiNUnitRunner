// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using NUnit.Framework;

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class NUnitTestEventArgsTest
{
    #region Tests for Constructors

    [Test]
    public void TestConstructorWithTest([Values] bool isNull)
    {
        INUnitTest test = isNull ? null : new NUnitTest(new TestStub());

        NUnitTestEventArgs eventArgs = new NUnitTestEventArgs(test);

        Assert.That(eventArgs.Test, Is.SameAs(test));
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestGetTestProperty([Values] bool isNull)
    {
        INUnitTest test = isNull ? null : new NUnitTest(new TestStub());

        NUnitTestEventArgs eventArgs = new NUnitTestEventArgs(test);

        Assert.That(eventArgs.Test, Is.SameAs(test));
    }

    #endregion
}

[TestFixture]
public class NUnitTestResultEventArgsTest
{
    #region Tests for Constructors

    [Test]
    public void TestConstructorWithTestResult([Values] bool isNull)
    {
        INUnitTestResult result = isNull ? null : new NUnitTestResult(new TestResultStub());

        NUnitTestResultEventArgs eventArgs = new NUnitTestResultEventArgs(result);

        Assert.That(eventArgs.Result, Is.SameAs(result));
    }

    #endregion

    #region Tests for Result Property

    [Test]
    public void TestGetResultProperty([Values] bool isNull)
    {
        INUnitTestResult result = isNull ? null : new NUnitTestResult(new TestResultStub());

        NUnitTestResultEventArgs eventArgs = new NUnitTestResultEventArgs(result);

        Assert.That(eventArgs.Result, Is.SameAs(result));
    }

    #endregion
}