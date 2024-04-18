// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Models;
using NUnit.Framework;
using System.Globalization;

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class InverseBoolConverterTest
{
    #region Tests for Convert

    [Test]
    public void TestConvertWithNullValueReturnsFalse()
    {
        InverseBoolConverter converter = new InverseBoolConverter();

        object result = converter.Convert(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.False);
    }

    [Test]
    public void TestConvertWithNotBoolValueReturnsFalse()
    {
        InverseBoolConverter converter = new InverseBoolConverter();

        object result = converter.Convert("not bool", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase("true", false)]
    [TestCase("True", false)]
    [TestCase("false", true)]
    [TestCase("False", true)]
    public void TestConvertWithBoolValueReturnsInvertedBoolValue(object value, bool expected)
    {
        InverseBoolConverter converter = new InverseBoolConverter();

        object result = converter.Convert(value, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for ConvertBack

    [Test]
    public void TestConvertBackWithNullValueReturnsFalse()
    {
        InverseBoolConverter converter = new InverseBoolConverter();

        object result = converter.ConvertBack(null, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.False);
    }

    [Test]
    public void TestConvertBackWithNotBoolValueReturnsFalse()
    {
        InverseBoolConverter converter = new InverseBoolConverter();

        object result = converter.ConvertBack("not bool", typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.False);
    }

    [Test]
    [TestCase(true, false)]
    [TestCase(false, true)]
    [TestCase("true", false)]
    [TestCase("True", false)]
    [TestCase("false", true)]
    [TestCase("False", true)]
    public void TestConvertBackWithBoolValueReturnsInvertedBoolValue(object value, bool expected)
    {
        InverseBoolConverter converter = new InverseBoolConverter();

        object result = converter.ConvertBack(value, typeof(bool), null, CultureInfo.InvariantCulture);

        Assert.That(result, Is.TypeOf<bool>());
        Assert.That(result, Is.EqualTo(expected));
    }

    #endregion
}