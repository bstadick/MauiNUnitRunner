// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework;

namespace MauiNUnitRunner.Controls.Tests.Resources;

[TestFixture]
public class TestControlResourcesTest
{
    #region Tests for Static Constructor

    [Test]
    public void TestStaticConstructorAddsResourceDictionaryToCurrentApplication()
    {
        TestControlResources resource = TestControlResources.GetInstance();

        var dictionary = Application.Current?.Resources.MergedDictionaries;
        Assert.That(dictionary, Is.Not.Null);
        Assert.That(dictionary.Count, Is.EqualTo(1));
        Assert.That(resource, Is.Not.Null);
        Assert.That(dictionary.Contains(resource));
    }

    #endregion

    #region Tests for GetInstance

    [Test]
    public void TestGetInstanceReturnsANonNullSingletonInstance()
    {
        TestControlResources resource = TestControlResources.GetInstance();

        Assert.That(resource, Is.Not.Null);
        Assert.That(TestControlResources.GetInstance(), Is.SameAs(resource));
    }

    #endregion
}