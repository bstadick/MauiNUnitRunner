// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework;

namespace MauiNUnitRunner.Controls.Tests;

/// <summary>
///     Fixture that is run once before all tests are ran and once again after all tests complete.
/// </summary>
[SetUpFixture]
public class NUnitTestSetupFixture
{
    [OneTimeSetUp]
    public void RunBeforeAnyTests()
    {
        // ReSharper disable once UseObjectOrCollectionInitializer
        Application.Current = new Application();
        Application.Current.Resources = new ResourceDictionary();
        // Just create a new TestControlResources as it will add itself to the merged resource dictionary
        // ReSharper disable once ObjectCreationAsStatement
        new TestControlResources();
    }
}