// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using CommunityToolkit.Maui;
using NUnit.Framework;

namespace MauiNUnitRunner.Controls.Tests;

[TestFixture]
public class AppBuilderExtensionsTest
{
    #region Tests for UseMauiNUnitRunner

    [Test]
    public void TestUseMauiNUnitRunnerInitializesServices()
    {
#pragma warning disable CA1416 // Validate platform compatibility
        MauiAppBuilder builder = MauiApp.CreateBuilder().UseMauiNUnitRunner();
#pragma warning restore CA1416 // Validate platform compatibility
        MauiApp app = builder.Build();

        // Check that popup service is registered meaning that the MauiCommunityToolkit has been initialized
        Assert.That(app.Services.GetService(typeof(IPopupService)), Is.Not.Null);
    }

    #endregion
}
