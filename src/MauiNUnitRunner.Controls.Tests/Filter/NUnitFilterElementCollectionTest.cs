// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using MauiNUnitRunner.Controls.Filter;

namespace MauiNUnitRunner.Controls.Tests.Filter;

[TestFixture]
public class NUnitFilterElementCollectionTest
{
    #region Tests for Constructor

    [Test]
    public void
        TestConstructorThrowsArgumentExceptionWhenParentNotNullAndElementTypeRootFilter()
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message.EqualTo(
                "The parent element cannot be non-null if the element type is RootFilter. (Parameter 'parent')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterElementCollection(parent, NUnitElementType.RootFilter));
    }

    [Test]
    public void
        TestConstructorThrowsArgumentNullExceptionWhenParentNullAndElementTypeNotRootFilter()
    {
        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The parent cannot be null. (Parameter 'parent')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterElementCollection(null, NUnitElementType.And));
    }

    [Test]
    public void
        TestConstructorThrowsArgumentOutOfRangeExceptionWhenElementTypeNotSupported(
            [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElementsPlusNot))]
            NUnitElementType elementType)
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        Assert.Throws(
            Is.TypeOf<ArgumentOutOfRangeException>().And.Message
                .EqualTo(
                    $"The given element type is not supported. (Parameter 'elementType'){Environment.NewLine}" +
                    $"Actual value was {elementType}."),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterElementCollection(parent, elementType));
    }

    [Test]
    [TestCase(NUnitElementType.And)]
    [TestCase(NUnitElementType.Or)]
    public void TestConstructorWithParentNotNullAndElementTypeSupportedButNotRootFilter(
        NUnitElementType elementType)
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        string expectedXmlTag = null;
        switch (elementType)
        {
            // NUnitElementType.RootFilter covered in a dedicated test case
            case NUnitElementType.And:
                expectedXmlTag = NUnitFilterTestHelper.XmlAndTag;
                break;
            case NUnitElementType.Or:
                expectedXmlTag = NUnitFilterTestHelper.XmlOrTag;
                break;
            default:
                Assert.Fail($"The type {elementType} is not supported for this test.");
                break;
        }

        INUnitFilterElementCollectionInternal element = new NUnitFilterElementCollection(parent, elementType);

        Assert.That(element.Parent, Is.SameAs(parent));
        Assert.That(element.Child, Is.Null);
        Assert.That(element.ElementType, Is.EqualTo(elementType));
        Assert.That(element.XmlTag, Is.EqualTo(expectedXmlTag));
    }

    [Test]
    public void TestConstructorWithParentNullAndElementTypeRootFilter()
    {
        INUnitFilterElementCollectionInternal element =
            new NUnitFilterElementCollection(null, NUnitElementType.RootFilter);

        Assert.That(element.Parent, Is.Null);
        Assert.That(element.Child, Is.Null);
        Assert.That(element.ElementType, Is.EqualTo(NUnitElementType.RootFilter));
        Assert.That(element.XmlTag, Is.EqualTo(NUnitFilterTestHelper.XmlFilterTag));
    }

    #endregion

    #region Tests for Not Property

    [Test]
    public void TestNotPropertySetsChildAndReturnsNewAndElementWithParent()
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterElementCollectionInternal element =
            new NUnitFilterElementCollection(parent, NUnitElementType.And);

        INUnitFilterContainerElementInternal not = (INUnitFilterContainerElementInternal) element.Not;

        Assert.That(not, Is.Not.Null);
        Assert.That(not.ElementType, Is.EqualTo(NUnitElementType.Not));
        Assert.That(not.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(not));
    }

    [Test]
    public void TestNotPropertyThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        const string value = "Value_";
        const string xmlTag = "name_";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

        NUnitFilterElementCollectionForTest element =
            new NUnitFilterElementCollectionForTest(parent, NUnitElementType.And);
        element.SetChild(child);

        Assert.Throws(
            Is.TypeOf<InvalidOperationException>().And.Message
                .EqualTo("The child element has already been set for this instance."),
            // ReSharper disable once UnusedVariable
            () =>
            {
                INUnitFilterContainerElement not = element.Not;
            });
    }

    #endregion

    // Rest of the NUnitFilterElementCollection class is inherited directly from the NUnitFilterContainerElement class,
    // therefore no further testing is needed as testing is covered by NUnitFilterContainerElement tests.
}