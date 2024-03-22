// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using MauiNUnitRunner.Controls.Filter;

namespace MauiNUnitRunner.Controls.Tests.Filter;

[TestFixture]
public class NUnitFilterElementTest
{
    #region Tests for Constructor

    [Test]
    public void
        TestConstructorThrowsArgumentExceptionWhenElementTypeIsPropertyAndValueIsNullOrEmpty(
            [Values] bool isNull)
    {
        const string name = "name_1";
        string value = isNull ? null : string.Empty;
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The value cannot be null or empty. (Parameter 'value')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterElement(parent, NUnitElementType.Property, name, false, value));
    }

    [Test]
    public void TestConstructorThrowsArgumentExceptionWhenNameIsNullOrEmpty([Values] bool isNull)
    {
        string name = isNull ? null : string.Empty;
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The name cannot be null or empty. (Parameter 'name')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterElement(parent, NUnitElementType.Test, name, false));
    }

    [Test]
    public void TestConstructorThrowsArgumentNullExceptionWhenParentIsNull()
    {
        const string name = "name_1";

        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The parent cannot be null. (Parameter 'parent')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterElement(null, NUnitElementType.Test, name, false));
    }


    [Test]
    [TestCase(NUnitElementType.RootFilter)]
    [TestCase(NUnitElementType.And)]
    [TestCase(NUnitElementType.Or)]
    [TestCase(NUnitElementType.Not)]
    public void
        TestConstructorThrowsArgumentOutOfRangeExceptionWhenElementTypeNotSupported(
            NUnitElementType elementType)
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        Assert.Throws(
            Is.TypeOf<ArgumentOutOfRangeException>().And.Message
                .EqualTo(
                    $"The given element type is not supported. (Parameter 'elementType'){Environment.NewLine}" +
                    $"Actual value was {elementType}."),
            // ReSharper disable once ObjectCreationAsStatement
            () => new NUnitFilterElement(parent, elementType, name, false, value));
    }

    [Test]
    public void TestConstructorWithValidArgumentsAndElementTypeProperty(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, isRegularExpression, value);

        Assert.That(element.Parent, Is.SameAs(parent));
        Assert.That(element.Child, Is.Null);
        Assert.That(element.ElementName, Is.EqualTo(name));
        Assert.That(element.ElementValue, Is.EqualTo(value));
        Assert.That(element.XmlTag, Is.SameAs("prop"));
        Assert.That(element.ElementType, Is.EqualTo(NUnitElementType.Property));
        Assert.That(element.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    [Test]
    public void TestConstructorWithValidArgumentsAndElementTypeSupportedButNotProperty(
        [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElementsExceptProperty))]
        NUnitElementType elementType, [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        string expectedXmlTag = null;
        switch (elementType)
        {
            case NUnitElementType.Id:
                expectedXmlTag = "id";
                break;
            case NUnitElementType.Test:
                expectedXmlTag = "test";
                break;
            case NUnitElementType.Category:
                expectedXmlTag = "cat";
                break;
            case NUnitElementType.Class:
                expectedXmlTag = "class";
                break;
            case NUnitElementType.Method:
                expectedXmlTag = "method";
                break;
            case NUnitElementType.Namespace:
                expectedXmlTag = "namespace";
                break;
            // NUnitElementType.Property covered in a dedicated test case
            case NUnitElementType.NUnitName:
                expectedXmlTag = "name";
                break;
            default:
                Assert.Fail($"The type {elementType} is not supported for this test.");
                break;
        }

        INUnitFilterElementInternal
            element = new NUnitFilterElement(parent, elementType, name, isRegularExpression);

        Assert.That(element.Parent, Is.SameAs(parent));
        Assert.That(element.Child, Is.Null);
        Assert.That(element.ElementName, Is.EqualTo(name));
        Assert.That(element.ElementValue, Is.Null);
        Assert.That(element.XmlTag, Is.SameAs(expectedXmlTag));
        Assert.That(element.ElementType, Is.EqualTo(elementType));
        Assert.That(element.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    #endregion

    #region Tests for ToString

    [Test]
    public void TestToStringReturnsStringRepresentation([Values] bool isChildNull,
        [Values] bool withValue, [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_";
        const string xmlTag = "child_";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Id);

        const string parentString = "{NUnitFilterContainerElement: {Type: RootFilter}}";
        string childString = isChildNull ? "Null" : "{XmlSerializableElementForTest: {Type: Id}}";
        string elementValue = withValue ? "Value_1" : null;
        string expected =
            $"{{NUnitFilterElementForTest: {{Type: Test, Name: \"name_1\", Value: \"{elementValue}\"," +
            $" IsRegularExpression: {isRegularExpression}, Parent: {parentString}, Child: {childString}}}}}";

        NUnitFilterElementForTest element = new NUnitFilterElementForTest(parent, NUnitElementType.Test, name,
            isRegularExpression, elementValue);
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
    public void TestParentPropertyReturnsParentProvidedWithConstructor()
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, false, value);

        Assert.That(element.Parent, Is.SameAs(parent));
    }

    #endregion

    #region Tests for Child Property

    [Test]
    public void TestChildPropertyReturnsSetChild()
    {
        const string name = "name_1";
        const string value = "Value_";
        const string xmlTag = "child_";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

        NUnitFilterElementForTest element =
            new NUnitFilterElementForTest(parent, NUnitElementType.Property, name, false, value);

        Assert.That(element.Child, Is.Null);

        element.SetChild(child);

        Assert.That(element.Child, Is.SameAs(child));
    }

    [Test]
    public void TestChildPropertyThrowsArgumentNullExceptionWhenChildSetToNull()
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        NUnitFilterElementForTest element =
            new NUnitFilterElementForTest(parent, NUnitElementType.Property, name, false, value);

        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("The value cannot be null. (Parameter 'value')"), () => element.SetChild(null));
    }

    #endregion

    #region Tests for ElementType Property

    [Test]
    public void TestElementTypePropertyReturnsElementTypeProvidedWithConstructor()
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, false, value);

        Assert.That(element.ElementType, Is.EqualTo(NUnitElementType.Property));
    }

    #endregion

    #region Tests for XmlTag Property

    [Test]
    public void TestXmlTagPropertyReturnsXmlTagProvidedWithConstructor()
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, false, value);

        Assert.That(element.XmlTag, Is.EqualTo("prop"));
    }

    #endregion

    #region Tests for ToXmlString

    [Test]
    public void TestToXmlString(
        [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElements))]
        NUnitElementType elementType, [Values] bool isRegularExpression, [Values] bool withXmlTag)
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        string expectedXmlTag = null;
        switch (elementType)
        {
            case NUnitElementType.Id:
                expectedXmlTag = "id";
                break;
            case NUnitElementType.Test:
                expectedXmlTag = "test";
                break;
            case NUnitElementType.Category:
                expectedXmlTag = "cat";
                break;
            case NUnitElementType.Class:
                expectedXmlTag = "class";
                break;
            case NUnitElementType.Method:
                expectedXmlTag = "method";
                break;
            case NUnitElementType.Namespace:
                expectedXmlTag = "namespace";
                break;
            case NUnitElementType.Property:
                expectedXmlTag = "prop";
                break;
            case NUnitElementType.NUnitName:
                expectedXmlTag = "name";
                break;
            default:
                Assert.Fail($"The type {elementType} is not supported for this test.");
                break;
        }

        string expected;
        if (elementType == NUnitElementType.Property)
        {
            expected = withXmlTag
                ? NUnitFilterTestHelper.CreateXmlNode(expectedXmlTag, value, isRegularExpression,
                    new Dictionary<string, string> {{"name", name}})
                : value;
        }
        else
        {
            expected = withXmlTag
                ? NUnitFilterTestHelper.CreateXmlNode(expectedXmlTag, name, isRegularExpression)
                : name;
        }

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, elementType, name, isRegularExpression, value);

        string actual = element.ToXmlString(withXmlTag);

        Assert.That(actual, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for ElementName Property

    [Test]
    public void TestElementNamePropertyReturnsElementNameProvidedWithConstructor()
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, false, value);

        Assert.That(element.ElementName, Is.EqualTo(name));
    }

    #endregion

    #region Tests for ElementValue Property

    [Test]
    public void TestElementValuePropertyReturnsElementValueProvidedWithConstructor(
        [Values] bool isNull)
    {
        const string name = "name_1";
        string value = isNull ? null : "Value_1";
        NUnitElementType elementType = isNull ? NUnitElementType.Test : NUnitElementType.Property;
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element = new NUnitFilterElement(parent, elementType, name, false, value);

        Assert.That(element.ElementValue, Is.EqualTo(value));
    }

    #endregion

    #region Tests for IsRegularExpression Property

    [Test]
    public void TestIsRegularExpressionPropertyReturnsIsRegularExpressionProvidedWithConstructor(
        [Values] bool isRegularExpression)
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, isRegularExpression, value);

        Assert.That(element.IsRegularExpression, Is.EqualTo(isRegularExpression));
    }

    #endregion

    #region Tests for And Property

    [Test]
    public void TestAndPropertySetsChildAndReturnsNewAndElementWithParent()
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, false, value);

        INUnitFilterElementCollectionInternal and = (INUnitFilterElementCollectionInternal) element.And;

        Assert.That(and, Is.Not.Null);
        Assert.That(and.ElementType, Is.EqualTo(NUnitElementType.And));
        Assert.That(and.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(and));
    }

    [Test]
    public void TestAndPropertyThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        const string name = "name_1";
        const string value = "Value_";
        const string xmlTag = "child_";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

        NUnitFilterElementForTest element =
            new NUnitFilterElementForTest(parent, NUnitElementType.Property, name, false, value);
        element.SetChild(child);

        Assert.Throws(
            Is.TypeOf<InvalidOperationException>().And.Message
                .EqualTo("The child element has already been set for this instance."),
            () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElementCollection and = element.And;
            });
    }

    #endregion

    #region Tests for Or Property

    [Test]
    public void TestOrPropertySetsChildAndReturnsNewOrElementWithParent()
    {
        const string name = "name_1";
        const string value = "Value_1";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);

        INUnitFilterElementInternal element =
            new NUnitFilterElement(parent, NUnitElementType.Property, name, false, value);

        INUnitFilterElementCollectionInternal or = (INUnitFilterElementCollectionInternal) element.Or;

        Assert.That(or, Is.Not.Null);
        Assert.That(or.ElementType, Is.EqualTo(NUnitElementType.Or));
        Assert.That(or.Parent, Is.SameAs(element));
        Assert.That(element.Child, Is.SameAs(or));
    }

    [Test]
    public void TestOrPropertyThrowsInvalidOperationExceptionWhenChildIsAlreadySet()
    {
        const string name = "name_1";
        const string value = "Value_";
        const string xmlTag = "child_";
        NUnitFilterContainerElement parent =
            new NUnitFilterContainerElement(null, NUnitElementType.RootFilter);
        XmlSerializableElementForTest child =
            new XmlSerializableElementForTest(xmlTag + 2, value + 2, NUnitElementType.Test);

        NUnitFilterElementForTest element =
            new NUnitFilterElementForTest(parent, NUnitElementType.Property, name, false, value);
        element.SetChild(child);

        Assert.Throws(
            Is.TypeOf<InvalidOperationException>().And.Message
                .EqualTo("The child element has already been set for this instance."), () =>
            {
                // ReSharper disable once UnusedVariable
                INUnitFilterElementCollection or = element.Or;
            });
    }

    #endregion

    #region Tests for Build

    [Test]
    public void TestBuildWithBothAndPlusOrPlusFilterElements()
    {
        NUnitFilter filter2 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).Or.Not
            .Id("id3", "id4", "id5").Build();
        const string expected2 =
            "<filter><or><and><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop></and><not><id>id3,id4,id5</id></not></or></filter>";

        Assert.That(filter2, Is.Not.Null);
        Assert.That(filter2.FilterXmlString, Is.EqualTo(expected2));

        NUnitFilter filter3 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).Or.Not
            .Id("id3", "id4", "id5").And.Id("id6").Build();
        const string expected3 =
            "<filter><or><and><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop></and><and><not><id>id3,id4,id5</id></not><id>id6</id></and></or></filter>";

        Assert.That(filter3, Is.Not.Null);
        Assert.That(filter3.FilterXmlString, Is.EqualTo(expected3));

        NUnitFilter filter4 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).Or.Not
            .Id("id3", "id4", "id5").And.Id("id6").And.Not.Class("class7", true).Build();
        const string expected4 =
            "<filter><or><and><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop></and><and><not><id>id3,id4,id5</id></not><id>id6</id><not><class re=\"1\">class7</class></not></and></or></filter>";

        Assert.That(filter4, Is.Not.Null);
        Assert.That(filter4.FilterXmlString, Is.EqualTo(expected4));

        NUnitFilter filter5 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).Or.Not
            .Id("id3", "id4", "id5").And.Id("id6").And.Not.Class("class7", true).Or.Method("method8").Build();
        const string expected5 =
            "<filter><or><and><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop></and><and><not><id>id3,id4,id5</id></not><id>id6</id><not><class re=\"1\">class7</class></not></and><method>method8</method></or></filter>";

        Assert.That(filter5, Is.Not.Null);
        Assert.That(filter5.FilterXmlString, Is.EqualTo(expected5));

        NUnitFilter filter6 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).Or.Not
            .Id("id3", "id4", "id5").And.Id("id6").And.Not.Class("class7", true).Or.Method("method8").Or
            .Name("name9").Build();
        const string expected6 =
            "<filter><or><and><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop></and><and><not><id>id3,id4,id5</id></not><id>id6</id><not><class re=\"1\">class7</class></not></and><method>method8</method><name>name9</name></or></filter>";

        Assert.That(filter6, Is.Not.Null);
        Assert.That(filter6.FilterXmlString, Is.EqualTo(expected6));
    }

    [Test]
    public void TestBuildWithOnlyAndPlusFilterElements()
    {
        NUnitFilter filter1 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).Build();
        const string expected1 = "<filter><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop></filter>";

        Assert.That(filter1, Is.Not.Null);
        Assert.That(filter1.FilterXmlString, Is.EqualTo(expected1));

        NUnitFilter filter2 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).And.Not
            .Id("id3", "id4", "id5").Build();
        const string expected2 =
            "<filter><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop><not><id>id3,id4,id5</id></not></filter>";

        Assert.That(filter2, Is.Not.Null);
        Assert.That(filter2.FilterXmlString, Is.EqualTo(expected2));

        NUnitFilter filter3 = NUnitFilter.Where.Category("cat1").And.Property("prop2", "value2", true).And.Not
            .Id("id3", "id4", "id5").And.Id("id6").Build();
        const string expected3 =
            "<filter><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop><not><id>id3,id4,id5</id></not><id>id6</id></filter>";

        Assert.That(filter3, Is.Not.Null);
        Assert.That(filter3.FilterXmlString, Is.EqualTo(expected3));
    }

    [Test]
    public void TestBuildWithOnlyANotPlusFilterElement(
        [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElements))]
        NUnitElementType elementType, [Values] bool isRegularExpression)
    {
        NUnitFilter filter = null;
        string expected = null;
        string regexp = isRegularExpression ? " re=\"1\"" : string.Empty;
        switch (elementType)
        {
            case NUnitElementType.Id:
                filter = NUnitFilter.Where.Not.Id("id1", "id2", "id3").Build();
                expected = "<id>id1,id2,id3</id>";
                break;
            case NUnitElementType.Test:
                filter = NUnitFilter.Where.Not.Test("test1", isRegularExpression).Build();
                expected = $"<test{regexp}>test1</test>";
                break;
            case NUnitElementType.Category:
                filter = NUnitFilter.Where.Not.Category("cat1", isRegularExpression).Build();
                expected = $"<cat{regexp}>cat1</cat>";
                break;
            case NUnitElementType.Class:
                filter = NUnitFilter.Where.Not.Class("class1", isRegularExpression).Build();
                expected = $"<class{regexp}>class1</class>";
                break;
            case NUnitElementType.Method:
                filter = NUnitFilter.Where.Not.Method("method1", isRegularExpression).Build();
                expected = $"<method{regexp}>method1</method>";
                break;
            case NUnitElementType.Namespace:
                filter = NUnitFilter.Where.Not.Namespace("ns1", isRegularExpression).Build();
                expected = $"<namespace{regexp}>ns1</namespace>";
                break;
            case NUnitElementType.Property:
                filter = NUnitFilter.Where.Not.Property("prop1", "value1", isRegularExpression).Build();
                expected = $"<prop{regexp} name=\"prop1\">value1</prop>";
                break;
            case NUnitElementType.NUnitName:
                filter = NUnitFilter.Where.Not.Name("name1", isRegularExpression).Build();
                expected = $"<name{regexp}>name1</name>";
                break;
            default:
                Assert.Fail($"The type {elementType} is not supported for this test.");
                break;
        }

        expected = $"<filter><not>{expected}</not></filter>";

        Assert.That(filter, Is.Not.Null);
        Assert.That(filter.FilterXmlString, Is.EqualTo(expected));
    }

    [Test]
    public void TestBuildWithOnlyOneFilterElement(
        [ValueSource(typeof(NUnitFilterTestHelper), nameof(NUnitFilterTestHelper.GetFilterElements))]
        NUnitElementType elementType, [Values] bool isRegularExpression)
    {
        NUnitFilter filter = null;
        string expected = null;
        string regexp = isRegularExpression ? " re=\"1\"" : string.Empty;
        switch (elementType)
        {
            case NUnitElementType.Id:
                filter = NUnitFilter.Where.Id("id1", "id2", "id3").Build();
                expected = "<id>id1,id2,id3</id>";
                break;
            case NUnitElementType.Test:
                filter = NUnitFilter.Where.Test("test1", isRegularExpression).Build();
                expected = $"<test{regexp}>test1</test>";
                break;
            case NUnitElementType.Category:
                filter = NUnitFilter.Where.Category("cat1", isRegularExpression).Build();
                expected = $"<cat{regexp}>cat1</cat>";
                break;
            case NUnitElementType.Class:
                filter = NUnitFilter.Where.Class("class1", isRegularExpression).Build();
                expected = $"<class{regexp}>class1</class>";
                break;
            case NUnitElementType.Method:
                filter = NUnitFilter.Where.Method("method1", isRegularExpression).Build();
                expected = $"<method{regexp}>method1</method>";
                break;
            case NUnitElementType.Namespace:
                filter = NUnitFilter.Where.Namespace("ns1", isRegularExpression).Build();
                expected = $"<namespace{regexp}>ns1</namespace>";
                break;
            case NUnitElementType.Property:
                filter = NUnitFilter.Where.Property("prop1", "value1", isRegularExpression).Build();
                expected = $"<prop{regexp} name=\"prop1\">value1</prop>";
                break;
            case NUnitElementType.NUnitName:
                filter = NUnitFilter.Where.Name("name1", isRegularExpression).Build();
                expected = $"<name{regexp}>name1</name>";
                break;
            default:
                Assert.Fail($"The type {elementType} is not supported for this test.");
                break;
        }

        expected = $"<filter>{expected}</filter>";

        Assert.That(filter, Is.Not.Null);
        Assert.That(filter.FilterXmlString, Is.EqualTo(expected));
    }

    [Test]
    public void TestBuildWithOnlyOrPlusFilterElements()
    {
        NUnitFilter filter1 = NUnitFilter.Where.Category("cat1").Or.Property("prop2", "value2", true).Build();
        const string expected1 =
            "<filter><or><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop></or></filter>";

        Assert.That(filter1, Is.Not.Null);
        Assert.That(filter1.FilterXmlString, Is.EqualTo(expected1));

        NUnitFilter filter2 = NUnitFilter.Where.Category("cat1").Or.Property("prop2", "value2", true).Or.Not
            .Id("id3", "id4", "id5").Build();
        const string expected2 =
            "<filter><or><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop><not><id>id3,id4,id5</id></not></or></filter>";

        Assert.That(filter2, Is.Not.Null);
        Assert.That(filter2.FilterXmlString, Is.EqualTo(expected2));

        NUnitFilter filter3 = NUnitFilter.Where.Category("cat1").Or.Property("prop2", "value2", true).Or.Not
            .Id("id3", "id4", "id5").Or.Id("id6").Build();
        const string expected3 =
            "<filter><or><cat>cat1</cat><prop re=\"1\" name=\"prop2\">value2</prop><not><id>id3,id4,id5</id></not><id>id6</id></or></filter>";

        Assert.That(filter3, Is.Not.Null);
        Assert.That(filter3.FilterXmlString, Is.EqualTo(expected3));
    }

    #endregion
}