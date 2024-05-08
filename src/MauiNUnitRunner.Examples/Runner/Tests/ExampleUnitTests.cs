// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;

namespace MauiNUnitRunner.Examples.Runner.Tests;

/// <summary>
///     A suite of example test cases.
/// </summary>
/// <remarks>
///     Tests will toggle between passing and failing (besides the ignored test), with the first test run passing.
/// </remarks>
[TestFixture]
public class ExampleUnitTests
{
    /// <summary>
    ///     Gets a toggled value, start as false so that the first test run will toggle it true.
    /// </summary>
    public static bool Toggle { get; private set; }

    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        // Toggle the value between true and false
        Toggle = !Toggle;

        Console.Write("This is a console message in the test fixture");
    }

    [SetUp]
    public void TestSetup()
    {
        // Throw an exception during setup only for the specified test
        if (TestContext.CurrentContext.Test.Name.Equals("TestThrowsExceptionDuringSetup"))
        {
            if (!Toggle)
            {
                throw new InvalidOperationException("This test setup threw an exception");
            }
        }
    }

    [Test]
    public void TestPass()
    {
        Assert.That(true);
    }

    [Test]
    public void TestPassWithConsoleMessage()
    {
        Assert.That(true);

        Console.Write("This is a console message");
    }

    [Test]
    public void TestTogglePassFail()
    {
        Assert.That(Toggle, "Toggles between pass and fail");
    }

    [Test]
    [TestCase("Case 1", true)]
    [TestCase("Case 2", false)]
    [TestCase("Case 3", true)]
    public void TestWithCases(string param1, bool param2)
    {
        Assert.That(param1, Is.Not.Null);
        Assert.That(param2, Is.True.Or.False);
    }

    [Test, Combinatorial]
    public void TestWithManyCases([Range(1, 10)] int param1, [Values] bool param2)
    {
        Assert.That(param1, Is.GreaterThan(0));
        Assert.That(param2, Is.True.Or.False);
    }

    [Test]
    public void TestInconclusive()
    {
        if (!Toggle)
        {
            Assume.That(false, "This test is inconclusive");
        }
        else
        {
            Assume.That(true);
            Console.Write("This test is sometimes inconclusive");
        }
    }

    [Test]
    public void TestWarning()
    {
        if (!Toggle)
        {
            Assert.Warn("This test raised a warning");
        }
        else
        {
            Assert.That(true);
            Console.Write("This test sometimes raises a warning");
        }
    }

    [Test]
    public void TestThrowsException()
    {
        if (!Toggle)
        {
            throw new InvalidOperationException("This test threw an exception");
        }
        else
        {
            Assert.That(true);
            Console.Write("This test sometimes throws an exception");
        }
    }

    [Test]
    [Ignore("This is an ignored test")]
    public void TestIgnored()
    {
        Assert.That(true);
    }

    [Test]
    public void TestThrowsExceptionDuringSetup()
    {
        // Will throw an exception during test setup

        Assert.That(true);
        Console.Write("This test sometimes throws an exception during setup");
    }

    [Test]
    public void TestWithDelay([Range(0, 4)] int count)
    {
        const int totalDelay = 5000;
        const int steps = 5;

        Thread.Sleep(totalDelay / steps);

        Assert.That(true, Is.True, "Count: {0}", count);
    }

    [Test]
    public void
        TestWithAVeryVeryVeryVeryLongNameSinceWeWantToTestThatItProperlyScrollsAndAvoidsTestNameClippingInTheUserInterface(
            [Values] bool param1, [Range(0, 4)] int param2)
    {
        Assert.That(param1, Is.True.Or.False);
        Assert.That(param2, Is.GreaterThanOrEqualTo(0));
    }
}

/// <summary>
///     A suite of an example test case with a single test case.
/// </summary>
/// <remarks>This test suite should not be skipped over when navigated to.</remarks>
[TestFixture]
public class ExampleUnitTestsWithOneTestCase
{
    [Test]
    public void TestWithSingleTest()
    {
        Assert.That(true);
    }
}

/// <summary>
///     A suite of example test cases with a single test with multiple test cases.
/// </summary>
/// <remarks>This test suite should not be skipped over when navigated to.</remarks>
[TestFixture]
public class ExampleUnitTestsWithOneTestAndMultipleCases
{
    [Test]
    public void TestWithSingleTestButMultipleCases([Range(0, 4)] int count)
    {
        Assert.That(true, "Count: {0}", count);
    }
}