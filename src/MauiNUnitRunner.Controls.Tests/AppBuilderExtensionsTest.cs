// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using CommunityToolkit.Maui.Core;
using NUnit.Framework;

namespace MauiNUnitRunner.Controls.Tests;

[TestFixture]
public class AppBuilderExtensionsTest
{
    #region Tests for UseMauiNUnitRunner

    [Test]
    public void TestUseMauiNUnitRunnerInitializesServices()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder().UseMauiNUnitRunner();
        MauiApp app = builder.Build();

        // Check that popup service is registered meaning that the MauiCommunityToolkit has been initialized
        Assert.That(app.Services.GetService(typeof(IPopupService)), Is.Not.Null);
    }

    #endregion
}