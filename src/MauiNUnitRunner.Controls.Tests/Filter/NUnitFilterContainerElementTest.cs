// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using MauiNUnitRunner.Controls.Filter;

namespace MauiNUnitRunner.Controls.Tests.Filter;

[TestFixture]
public class NUnitFilterContainerElementTest
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
            () => new NUnitFilterContainerElement(parent, NUnitElementType.RootFilter));
    }

    [Test]
    public void
        TestConstructorThrowsArgumentNullExceptionWhenParentNullAndElementTypeNotRootFilter()
    {
        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The parent cannot be null. (Parameter 'parent')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterContainerElement(null, NUnitElementType.And));
    }

    [Test]
    public void
        TestConstructorThrowsArgumentOutOfRangeExceptionWhenElementTypeNotSupported(
            [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElements))]
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
            () => new NUnitFilterContainerElement(parent, elementType));
    }

    [Test]
    [TestCase(NUnitElementType.And)]
    [TestCase(NUnitElementType.Or)]
    [TestCase(NUnitElementType.Not)]
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
            case NUnitElementType.Not:
                expectedXmlTag = NUnitFilterTestHelper.XmlNotTag;
                break;
            default:
                Assert.Fail($"The type {elementType} is not supported for this test.");
                break;
        }

        INUnitFilterContainerElementInternal element = new NUnitFilterContainerElement(parent, elementType);

        Assert.That(element.Parent, Is.SameAs(parent));
        Assert.That(element.Child, Is.Null);
        Assert.That(element.ElementType, Is.EqualTo(elementType));
        Assert.That(element.XmlTag, Is.EqualTo(expectedXmlTag));
    }

    [Test]
    public void TestConstructorWithParentNullAndElementTypeRootFilter()
    {
        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        Assert.That(element.Parent, Is.Null);
        Assert.That(element.Child, Is.Null);
        Assert.That(element.ElementType, Is.EqualTo(NUnitElementType.RootFilter));
        Assert.That(element.XmlTag, Is.EqualTo(NUnitFilterTestHelper.XmlFilterTag));
    }

    #endregion

    #region Tests for ToString

    [Test]
    public void TestToStringReturnsStringRepresentation([Values] bool isParentNull,
        [Values] bool isChildNull)
    {
        // Create expected string of xml nodes
        const string valueParent = "Value_1";
        const string xmlTagParent = "name_1";
        XmlSerializableElementForTest parent = isParentNull
            ? null
            : new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);

        const string valueChild = "Value_2";
        const string xmlTagChild = "name_2";
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Id);

        NUnitElementType elementType = isParentNull ? NUnitElementType.RootFilter : NUnitElementType.And;
        string parentString = isParentNull ? "Null" : "{XmlSerializableElementForTest: {Type: Test}}";
        string childString = isChildNull ? "Null" : "{XmlSerializableElementForTest: {Type: Id}}";
        string expected =
            $"{{NUnitFilterContainerElementForTest: {{Type: {elementType}, Parent: {parentString}, Child: {childString}}}}}";

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(parent, elementType);
        if (!isChildNull)
        {
            element.SetChild(child);
        }

        string actual = element.ToString();

        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Parent Property

    [Test]
    public void TestParentPropertyReturnsParentProvidedWithConstructor(
        [Values] bool isNull)
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            isNull ? null : new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);
        NUnitElementType elementType = isNull ? NUnitElementType.RootFilter : NUnitElementType.And;

        INUnitFilterContainerElementInternal element = new NUnitFilterContainerElement(parent, elementType);

        if (isNull)
        {
            Assert.That(element.Parent, Is.Null);
        }
        else
        {
            Assert.That(element.Parent, Is.SameAs(parent));
        }
    }

    #endregion

    #region Tests for Child Property

    [Test]
    public void TestChildPropertyReturnsSetChild()
    {
        const string value = "Value_";
        const string xmlTag = "name_";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);

        Assert.That(element.Child, Is.Null);

        element.SetChild(child);

        Assert.That(element.Child, Is.SameAs(child));
    }

    [Test]
    public void TestChildPropertyThrowsArgumentNullExceptionWhenChildSetToNull()
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The value cannot be null. (Parameter 'value')"), () => element.SetChild(null));
    }

    #endregion

    #region Tests for ElementType Property

    [Test]
    public void TestElementTypePropertyReturnsElementTypeProvidedWithConstructor()
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.That(element.ElementType, Is.EqualTo(NUnitElementType.And));
    }

    #endregion

    #region Tests for XmlTag Property

    [Test]
    public void TestXmlTagPropertyReturnsXmlTagProvidedWithConstructor()
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.That(element.XmlTag, Is.EqualTo(NUnitFilterTestHelper.XmlAndTag));
    }

    #endregion

    #region Tests for ToXmlString

    [Test]
    public void TestToXmlStringThrowsNullArgumentExceptionWhenChildNull(
        [Values] bool withXmlTag)
    {
        const string valueParent = "Value_1";
        const string xmlTagParent = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);

        Assert.That(element.Child, Is.Null);

        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The Child cannot be null. (Parameter 'Child')"),
            () => element.ToXmlString(withXmlTag));
    }

    [Test]
    public void
        TestToXmlStringWithParentNotNullAndElementTypeNotNotReturnsParentAndChildXmlStrings(
            [Values] bool withXmlTag)
    {
        // Create expected string of xml nodes
        const string valueParent = "Value_1";
        const string xmlTagParent = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);
        string expectedValueParent = NUnitFilterTestHelper.CreateXmlNode(xmlTagParent, valueParent);

        const string valueChild = "Value_2";
        const string xmlTagChild = "name_2";
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Test);
        string expectedValueChild = NUnitFilterTestHelper.CreateXmlNode(xmlTagChild, valueChild);

        // With tag includes parent xml tag, without is just value
        string valueParentAndChild = expectedValueParent + expectedValueChild;
        string expected = withXmlTag
            ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlAndTag, valueParentAndChild)
            : valueParentAndChild;

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);
        element.SetChild(child);

        string actual = element.ToXmlString(withXmlTag);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestToXmlStringWithParentNotNullAndElementTypeNotReturnsChildXmlStrings(
        [Values] bool withXmlTag)
    {
        const string valueParent = "Value_1";
        const string xmlTagParent = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTagParent, valueParent, NUnitElementType.Test);

        const string valueChild = "Value_2";
        const string xmlTagChild = "name_2";
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Test);
        string expectedValueChild = NUnitFilterTestHelper.CreateXmlNode(xmlTagChild, valueChild);

        // With tag includes parent xml tag, without is just value
        string expected = withXmlTag
            ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlNotTag, expectedValueChild)
            : expectedValueChild;

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(parent, NUnitElementType.Not);
        element.SetChild(child);

        string actual = element.ToXmlString(withXmlTag);

        Assert.That(actual, Is.EqualTo(expected));
    }

    [Test]
    public void TestToXmlStringWithParentNullAndElementTypeNotNotReturnsChildXmlString(
        [Values] bool withXmlTag)
    {
        // Create expected string of xml nodes
        const string valueChild = "Value_1";
        const string xmlTagChild = "name_1";
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTagChild, valueChild, NUnitElementType.Test);
        string expectedValueChild = NUnitFilterTestHelper.CreateXmlNode(xmlTagChild, valueChild);

        // With tag includes parent xml tag, without is just value
        string expected = withXmlTag
            ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlFilterTag, expectedValueChild)
            : expectedValueChild;

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(null, NUnitElementType.RootFilter);
        element.SetChild(child);

        string actual = element.ToXmlString(withXmlTag);

        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Id

    [Test]
    public void
        TestIdThrowsArgumentExceptionWhenMultipleTestIdsContainsOnlyNullOrEmptyValues()
    {
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement id = element.Id(null, string.Empty);
            });
    }

    [Test]
    public void TestIdThrowsArgumentExceptionWhenSingleTestIdNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement id = element.Id(name);
            });
    }

    [Test]
    public void
        TestIdThrowsArgumentExceptionWhenTestIdArrayContainsOnlyNullOrEmptyValues()
    {
        string[] names = [null, string.Empty];
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement id = element.Id(names);
            });
    }

    [Test]
    public void TestIdThrowsArgumentExceptionWhenTestIdArrayNullOrEmpty(
        [Values] bool isNull)
    {
        string[] names = isNull ? null : Array.Empty<string>();
        const string value = "Value_1";
        const string xmlTag = "name_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The testIds cannot be null or empty. (Parameter 'testIds')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement id = element.Id(names);
            });
    }

    [Test]
    public void TestIdThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, _) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement id = e.Id(s);
        });
    }

    [Test]
    public void TestIdWithMultipleTestIdsSetsChildAndReturnsNewAndElementWithParent()
    {
        const string name1 = "name_1";
        const string name2 = "name_2";
        const string name3 = "name_3";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);
        const string expectedName = name1 + "," + name2 + "," + name3;

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal id =
            (INUnitFilterElementInternal) element.Id(name1, null, name2, string.Empty, name3);

        Assert.That(id, Is.Not.Null);
        Assert.That(id.ElementType, Is.EqualTo(NUnitElementType.Id));
        Assert.That(id.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(id));
        Assert.That(id.ElementName, Is.EqualTo(expectedName));
        Assert.That(id.ElementValue, Is.Null);
        Assert.That(id.IsRegularExpression, Is.False);
    }

    [Test]
    public void TestIdWithSingleTestIdSetsChildAndReturnsNewAndElementWithParent()
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal id = (INUnitFilterElementInternal) element.Id(name);

        Assert.That(id, Is.Not.Null);
        Assert.That(id.ElementType, Is.EqualTo(NUnitElementType.Id));
        Assert.That(id.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(id));
        Assert.That(id.ElementName, Is.EqualTo(name));
        Assert.That(id.ElementValue, Is.Null);
        Assert.That(id.IsRegularExpression, Is.False);
    }

    [Test]
    public void TestIdWithTestIdArraySetsChildAndReturnsNewAndElementWithParent()
    {
        const string name1 = "name_1";
        const string name2 = "name_2";
        const string name3 = "name_3";
        string[] names = [name1, null, name2, string.Empty, name3];
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);
        const string expectedName = name1 + "," + name2 + "," + name3;

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal id = (INUnitFilterElementInternal) element.Id(names);

        Assert.That(id, Is.Not.Null);
        Assert.That(id.ElementType, Is.EqualTo(NUnitElementType.Id));
        Assert.That(id.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(id));
        Assert.That(id.ElementName, Is.EqualTo(expectedName));
        Assert.That(id.ElementValue, Is.Null);
        Assert.That(id.IsRegularExpression, Is.False);
    }

    #endregion

    #region Tests for Test

    [Test]
    public void TestTestSetsChildAndReturnsNewAndElementWithParent(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal test = (INUnitFilterElementInternal) element.Test(name, isRegularExpression);

        Assert.That(test, Is.Not.Null);
        Assert.That(test.ElementType, Is.EqualTo(NUnitElementType.Test));
        Assert.That(test.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(test));
        Assert.That(test.ElementName, Is.EqualTo(name));
        Assert.That(test.ElementValue, Is.Null);
        Assert.That(test.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    [Test]
    public void TestTestThrowsArgumentExceptionWhenTestNameIsNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement test = element.Test(name);
            });
    }

    [Test]
    public void TestTestThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement test = e.Test(s, b);
        });
    }

    #endregion

    #region Tests for Category

    [Test]
    public void TestCategorySetsChildAndReturnsNewAndElementWithParent(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal category =
            (INUnitFilterElementInternal) element.Category(name, isRegularExpression);

        Assert.That(category, Is.Not.Null);
        Assert.That(category.ElementType, Is.EqualTo(NUnitElementType.Category));
        Assert.That(category.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(category));
        Assert.That(category.ElementName, Is.EqualTo(name));
        Assert.That(category.ElementValue, Is.Null);
        Assert.That(category.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    [Test]
    public void TestCategoryThrowsArgumentExceptionWhenCategoryNameIsNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement category = element.Category(name);
            });
    }

    [Test]
    public void TestCategoryThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement category = e.Category(s, b);
        });
    }

    #endregion

    #region Tests for Class

    [Test]
    public void TestClassSetsChildAndReturnsNewAndElementWithParent(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal classElement =
            (INUnitFilterElementInternal) element.Class(name, isRegularExpression);

        Assert.That(classElement, Is.Not.Null);
        Assert.That(classElement.ElementType, Is.EqualTo(NUnitElementType.Class));
        Assert.That(classElement.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(classElement));
        Assert.That(classElement.ElementName, Is.EqualTo(name));
        Assert.That(classElement.ElementValue, Is.Null);
        Assert.That(classElement.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    [Test]
    public void TestClassThrowsArgumentExceptionWhenClassNameIsNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement classElement = element.Class(name);
            });
    }

    [Test]
    public void TestClassThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement classElement = e.Class(s, b);
        });
    }

    #endregion

    #region Tests for Method

    [Test]
    public void TestMethodSetsChildAndReturnsNewAndElementWithParent(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal
            method = (INUnitFilterElementInternal) element.Method(name, isRegularExpression);

        Assert.That(method, Is.Not.Null);
        Assert.That(method.ElementType, Is.EqualTo(NUnitElementType.Method));
        Assert.That(method.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(method));
        Assert.That(method.ElementName, Is.EqualTo(name));
        Assert.That(method.ElementValue, Is.Null);
        Assert.That(method.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    [Test]
    public void TestMethodThrowsArgumentExceptionWhenMethodNameIsNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement method = element.Method(name);
            });
    }

    [Test]
    public void TestMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement method = e.Method(s, b);
        });
    }

    [Test]
    public void TestNameSetsChildAndReturnsNewAndElementWithParent(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal nameElement =
            (INUnitFilterElementInternal) element.Name(name, isRegularExpression);

        Assert.That(nameElement, Is.Not.Null);
        Assert.That(nameElement.ElementType, Is.EqualTo(NUnitElementType.NUnitName));
        Assert.That(nameElement.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(nameElement));
        Assert.That(nameElement.ElementName, Is.EqualTo(name));
        Assert.That(nameElement.ElementValue, Is.Null);
        Assert.That(nameElement.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    #endregion

    #region Tests for Namespace

    [Test]
    public void TestNamespaceSetsChildAndReturnsNewAndElementWithParent(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal namespaceElement =
            (INUnitFilterElementInternal) element.Namespace(name, isRegularExpression);

        Assert.That(namespaceElement, Is.Not.Null);
        Assert.That(namespaceElement.ElementType, Is.EqualTo(NUnitElementType.Namespace));
        Assert.That(namespaceElement.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(namespaceElement));
        Assert.That(namespaceElement.ElementName, Is.EqualTo(name));
        Assert.That(namespaceElement.ElementValue, Is.Null);
        Assert.That(namespaceElement.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    [Test]
    public void TestNamespaceThrowsArgumentExceptionWhenNamespaceNameIsNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement namespaceElement = element.Namespace(name);
            });
    }

    [Test]
    public void TestNamespaceThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement namespaceElement = e.Namespace(s, b);
        });
    }

    #endregion

    #region Tests for Property

    [Test]
    public void TestPropertySetsChildAndReturnsNewAndElementWithParent(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        INUnitFilterElementInternal property =
            (INUnitFilterElementInternal) element.Property(name, value, isRegularExpression);

        Assert.That(property, Is.Not.Null);
        Assert.That(property.ElementType, Is.EqualTo(NUnitElementType.Property));
        Assert.That(property.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(property));
        Assert.That(property.ElementName, Is.EqualTo(name));
        Assert.That(property.ElementValue, Is.EqualTo(value));
        Assert.That(property.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    [Test]
    public void TestPropertyThrowsArgumentExceptionWhenPropertyNameIsNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement property = element.Property(name, value);
            });
    }

    [Test]
    public void TestPropertyThrowsArgumentExceptionWhenPropertyValueIsNullOrEmpty(
        [Values] bool isNull)
    {
        const string name = "name_1";
        string value = isNull ? null : string.Empty;
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The value cannot be null or empty. (Parameter 'value')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement property = element.Property(name, value);
            });
    }

    [Test]
    public void TestPropertyThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement property = e.Property(s, s, b);
        });
    }

    #endregion

    #region Tests for Name

    [Test]
    public void TestNameThrowsArgumentExceptionWhenNameElementNameIsNullOrEmpty(
        [Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        const string value = "Value_1";
        const string xmlTag = "parent_1";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.Test);

        INUnitFilterContainerElementInternal element =
            new NUnitFilterContainerElement(parent, NUnitElementType.And);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElement nameElement = element.Name(name);
            });
    }

    [Test]
    public void TestNameThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet((e, s, b) =>
        {
            // ReSharper disable once UnusedVariable
            INUnitFilterElement nameElement = e.Name(s, b);
        });
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Generic test for Test(Method)ThrowsInvalidOperationExceptionWhenChildIsAlreadySet.
    /// </summary>
    /// <param name="testFunction">
    ///     The function under test provided with the <see cref="NUnitFilterContainerElement" />,
    ///     <see cref="INUnitFilterElementInternal.ElementName" />, and
    ///     <see cref="INUnitFilterElementInternal.IsRegularExpression" />
    /// </param>
    private static void TestElementMethodThrowsInvalidOperationExceptionWhenChildIsAlreadySet(
        Action<NUnitFilterContainerElement, string, bool> testFunction)
    {
        const string name = "element_1";
        const string value = "Value_";
        const string xmlTag = "name_";
        XmlSerializableElementForTest parent =
            new XmlSerializableElementForTest(xmlTag + 1, value + 1, NUnitElementType.Test);
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

        NUnitFilterContainerElementForTest element =
            new NUnitFilterContainerElementForTest(parent, NUnitElementType.And);
        element.SetChild(child);

        Assert.Throws(
            Is.TypeOf<InvalidOperationException>().And.Message
                .EqualTo("The child element has already been set for this instance."),
            () => { testFunction(element, name, false); });
    }

    #endregion
}