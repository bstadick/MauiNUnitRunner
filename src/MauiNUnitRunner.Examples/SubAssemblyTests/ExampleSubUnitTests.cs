// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;

namespace MauiNUnitRunner.Examples.SubAssemblyTests;

/// <summary>
///     A suite of example test cases in a sub-assembly. This class is a direct copy of the ExampleUnitTests class.
/// </summary>
/// <remarks>
///     Tests will toggle between passing and failing (besides the ignored test), with the first test run passing.
/// </remarks>
[TestFixture]
public class ExampleSubUnitTests
{
    /// <summary>
    ///     Holds a toggled value, start as false so that the first test run will toggle it true.
    /// </summary>
    private static bool v_Toggle;

    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        // Toggle the value between true and false
        v_Toggle = !v_Toggle;

        Console.Write("This is a console message in the test fixture");
    }

    [SetUp]
    public void TestSetup()
    {
        // Throw an exception during setup only for the specified test
        if (TestContext.CurrentContext.Test.Name.Equals("TestThrowsExceptionDuringSetup"))
        {
            if (!v_Toggle)
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
        Assert.That(v_Toggle, "Toggles between pass and fail");
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
        if (!v_Toggle)
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
        if (!v_Toggle)
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
        if (!v_Toggle)
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
}