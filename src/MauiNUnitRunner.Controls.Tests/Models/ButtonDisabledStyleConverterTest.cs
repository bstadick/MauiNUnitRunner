// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using NUnit.Framework;
using System.Globalization;
using MauiNUnitRunner.Controls.Resources;

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class ButtonDisabledStyleConverterTest
{
    #region Tests for Convert

    [Test]
    public void TestConvertWithNullValueReturnsButtonStyle()
    {
        ButtonDisabledStyleConverter converter = new ButtonDisabledStyleConverter();

        object result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<Style>());
        Style expectedStyle = ResourceHelper.GetResourceStyle("ButtonStyle");
        Assert.That(expectedStyle, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedStyle));
    }

    [Test]
    public void TestConvertWithNotBoolValueReturnsButtonStyle()
    {
        ButtonDisabledStyleConverter converter = new ButtonDisabledStyleConverter();

        object result = converter.Convert("not bool", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<Style>());
        Style expectedStyle = ResourceHelper.GetResourceStyle("ButtonStyle");
        Assert.That(expectedStyle, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedStyle));
    }

    [Test]
    [TestCase(true, "ButtonDisabledStyle")]
    [TestCase(false, "ButtonStyle")]
    [TestCase("true", "ButtonDisabledStyle")]
    [TestCase("True", "ButtonDisabledStyle")]
    [TestCase("false", "ButtonStyle")]
    [TestCase("False", "ButtonStyle")]
    public void TestConvertWithBoolValueReturnsButtonStyleStateValue(object value, string expected)
    {
        ButtonDisabledStyleConverter converter = new ButtonDisabledStyleConverter();

        object result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<Style>());
        Style expectedStyle = ResourceHelper.GetResourceStyle(expected);
        Assert.That(expectedStyle, Is.Not.Null);
        Assert.That(result, Is.EqualTo(expectedStyle));
    }

    #endregion

    #region Tests for ConvertBack

    [Test]
    public void TestConvertBackWithNullValueReturnsFalse()
    {
        ButtonDisabledStyleConverter converter = new ButtonDisabledStyleConverter();

        object result = converter.ConvertBack(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.False);
    }

    [Test]
    public void TestConvertBackWithNotStyleValueReturnsFalse()
    {
        ButtonDisabledStyleConverter converter = new ButtonDisabledStyleConverter();

        object result = converter.ConvertBack("not bool", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase("ButtonDisabledStyle", true)]
    [TestCase("ButtonStyle", false)]
    [TestCase("LabelTextStyle", false)]
    public void TestConvertBackWithButtonStyleValueReturnsBoolValue(string value, bool expected)
    {
        ButtonDisabledStyleConverter converter = new ButtonDisabledStyleConverter();

        Style style = ResourceHelper.GetResourceStyle(value);
        Assert.IsNotNull(style);

        object result = converter.ConvertBack(style, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion
}