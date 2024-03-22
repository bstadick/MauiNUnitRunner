// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using MauiNUnitRunner.Controls.Filter;

namespace MauiNUnitRunner.Controls.Tests.Filter;

[TestFixture]
public class ExpressionCollectionTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructorThrowsArgumentExceptionWhenXmlTagIsNullOrEmpty(
        [Values] bool isNull)
    {
        string xmlTag = isNull ? null : string.Empty;

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message
                .EqualTo("The xmlTag cannot be null or empty. (Parameter 'xmlTag')"),
            // ReSharper disable once ObjectCreationAsStatement
            () => new ExpressionCollection<INUnitFilterBaseElement>(xmlTag));
    }

    [Test]
    public void TestConstructorWithXmlTag()
    {
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        Assert.That(collection.XmlTag, Is.EqualTo(NUnitFilterTestHelper.XmlAndTag));
        Assert.That(collection.Count, Is.EqualTo(0));
    }

    #endregion

    #region Tests for XmlTag Property

    [Test]
    public void TestXmlTagPropertyReturnsXmlTagProvidedWithConstructor()
    {
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        Assert.That(collection.XmlTag, Is.EqualTo(NUnitFilterTestHelper.XmlAndTag));
    }

    #endregion

    #region Tests for ToXmlString

    [Test]
    public void TestToXmlStringWithCollectionEmptyReturnsEmptyString([Values] bool withXmlTag)
    {
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        Assert.That(collection.Count, Is.EqualTo(0));
        Assert.That(collection.ToXmlString(withXmlTag), Is.EqualTo(string.Empty));
    }

    [Test]
    public void TestToXmlStringWithCollectionOfMultipleItemsReturnsCollectionXmlString(
        [Values] bool withXmlTag)
    {
        // Create collection and expected string of xml nodes
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out string innerXml);
        // With tag includes parent xml tag, without is just value
        string expected = withXmlTag
            ? NUnitFilterTestHelper.CreateXmlNode(NUnitFilterTestHelper.XmlAndTag, innerXml)
            : innerXml;

        Assert.That(collection.Count, Is.EqualTo(count));
        Assert.That(collection.ToXmlString(withXmlTag), Is.EqualTo(expected));
    }

    [Test]
    public void TestToXmlStringWithCollectionOfOneItemReturnsItemXmlString(
        [Values] bool withXmlTag)
    {
        // Create expected string of xml nodes
        const string value = "Value_1";
        const string xmlTag = "name_1";
        // With tag includes parent xml tag, without is just value
        string expected = withXmlTag ? NUnitFilterTestHelper.CreateXmlNode(xmlTag, value) : value;

        ExpressionCollection<INUnitFilterBaseElement> collection =
            // ReSharper disable once UseObjectOrCollectionInitializer
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        // Add expression to collection
        collection.Add(new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And));

        Assert.That(collection.Count, Is.EqualTo(1));
        Assert.That(collection.ToXmlString(withXmlTag), Is.EqualTo(expected));
    }

    #endregion

    #region Tests for GetEnumerator

    [Test]
    public void TestGetEnumeratorWithItemsInCollection()
    {
        // Create collection
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        IEnumerator<INUnitFilterBaseElement> enumerator = collection.GetEnumerator();

        Assert.That(collection.Count, Is.EqualTo(count));
        Assert.That(enumerator, Is.Not.Null);

        // Copy enumerator to list
        IList<INUnitFilterBaseElement> copy = new List<INUnitFilterBaseElement>();
        while (enumerator.MoveNext())
        {
            copy.Add(enumerator.Current);
        }

        Assert.That(copy, Is.EqualTo(collection));

        enumerator.Dispose();
    }

    [Test]
    public void TestGetEnumeratorWithNoItemsInCollection()
    {
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        IEnumerator<INUnitFilterBaseElement> enumerator = collection.GetEnumerator();

        Assert.That(collection.Count, Is.EqualTo(0));
        Assert.That(enumerator, Is.Not.Null);

        // Copy enumerator to list
        IList<INUnitFilterBaseElement> copy = new List<INUnitFilterBaseElement>();
        while (enumerator.MoveNext())
        {
            copy.Add(enumerator.Current);
        }

        Assert.That(copy, Is.EqualTo(collection));

        enumerator.Dispose();
    }

    #endregion

    #region Tests for Add

    [Test]
    public void TestAddWithNonNullItemAddsItem()
    {
        // Create collection and item to add
        const int count = 3;
        const string value = "Value_new";
        const string xmlTag = "name_new";
        XmlSerializableElementForTest item = new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And);
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        collection.Add(item);

        Assert.That(collection.Count, Is.EqualTo(count + 1));
        Assert.That(collection.Last(), Is.SameAs(item));
    }

    [Test]
    public void TestAddWithNullItemDoesNotAddItem()
    {
        // Create collection
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        collection.Add(null);

        Assert.That(collection.Count, Is.EqualTo(count));
    }

    #endregion

    #region Tests for Clear

    [Test]
    public void TestClearWithItemsInCollection()
    {
        // Create collection
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        Assert.That(collection.Count, Is.EqualTo(count));

        collection.Clear();

        Assert.That(collection.Count, Is.EqualTo(0));
    }

    [Test]
    public void TestClearWithoutItemsInCollection()
    {
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        Assert.That(collection.Count, Is.EqualTo(0));

        collection.Clear();

        Assert.That(collection.Count, Is.EqualTo(0));
    }

    #endregion

    #region Tests for Contains

    [Test]
    public void TestContainsWhenItemIsNotPresentReturnsFalse([Values] bool isNull)
    {
        // Create collection and item to search
        const int count = 3;
        const string value = "Value_new";
        const string xmlTag = "name_new";
        XmlSerializableElementForTest item =
            isNull ? null : new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And);
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        Assert.That(collection.Count, Is.EqualTo(count));

        bool contains = collection.Contains(item);

        Assert.That(contains, Is.False);
    }

    [Test]
    public void TestContainsWhenItemIsPresentReturnsTrue()
    {
        // Create collection
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        Assert.That(collection.Count, Is.EqualTo(count));

        bool contains = collection.Contains(collection.First());

        Assert.That(contains, Is.True);
    }

    #endregion

    #region Test for CopyTo

    [Test]
    public void TestCopyToThrowsArgumentExceptionWhenCollectionDoesNotFitInArrayLength(
        [Values] bool indexOutOfRange)
    {
        // Create collection
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        // If indexOutOfRange then the array index plus collection length is longer than the array,
        // otherwise the array is just not long enough to hold collection
        int arrayLength = indexOutOfRange ? count : 0;
        int arrayIndex = indexOutOfRange ? 1 : 0;
        INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[arrayLength];

        Assert.Throws(
            Is.TypeOf<ArgumentException>().And.Message.EqualTo(
                "Destination array was not long enough." +
                " Check the destination index, length, and the array's lower bounds." +
                " (Parameter 'destinationArray')"),
            () => collection.CopyTo(array, arrayIndex));
    }

    [Test]
    public void TestCopyToThrowsArgumentNullExceptionWhenArrayIsNull()
    {
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        Assert.Throws(
            Is.TypeOf<ArgumentNullException>().And.Message
                .EqualTo("Value cannot be null. (Parameter 'destinationArray')"),
            // ReSharper disable once AssignNullToNotNullAttribute
            () => collection.CopyTo(null, 0));
    }

    [Test]
    public void TestCopyToThrowsArgumentOutOfRangeExceptionWhenArrayIndexLessThanZero()
    {
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);
        INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[1];

        Assert.Throws(
            Is.TypeOf<ArgumentOutOfRangeException>().And.Message.EqualTo(
                "Number was less than the array's lower bound in the first dimension. (Parameter 'destinationIndex')"),
            () => collection.CopyTo(array, -1));
    }

    [Test]
    public void TestCopyToWithItemsInCollectionCopiesToArray([Range(0, 2)] int arrayIndex)
    {
        // Create collection and expected
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);
        IList<INUnitFilterBaseElement> collectionList = collection.ToList();
        INUnitFilterBaseElement[] expected = new INUnitFilterBaseElement[count + arrayIndex];
        for (int i = arrayIndex, j = 0; i < expected.Length; i++, j++)
        {
            expected[i] = collectionList[j];
        }

        INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[count + arrayIndex];

        Assert.That(collection.Count, Is.EqualTo(count));

        collection.CopyTo(array, arrayIndex);

        Assert.That(array, Is.EqualTo(expected));
    }

    [Test]
    public void TestCopyToWithoutItemsInCollectionCopiesNoneToArray(
        [Range(0, 2)] int arrayIndex)
    {
        // Create collection and expected
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);
        INUnitFilterBaseElement[] expected = new INUnitFilterBaseElement[arrayIndex];
        INUnitFilterBaseElement[] array = new INUnitFilterBaseElement[arrayIndex];

        Assert.That(collection.Count, Is.EqualTo(0));

        collection.CopyTo(array, arrayIndex);

        Assert.That(array, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Remove

    [Test]
    public void TestRemoveWhenItemIsNotPresentReturnsFalse([Values] bool isNull)
    {
        // Create collection and item to search
        const int count = 3;
        const string value = "Value_new";
        const string xmlTag = "name_new";
        XmlSerializableElementForTest item =
            isNull ? null : new XmlSerializableElementForTest(xmlTag, value, NUnitElementType.And);
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);
        IList<INUnitFilterBaseElement> expected = new List<INUnitFilterBaseElement>(collection);

        Assert.That(collection.Count, Is.EqualTo(count));

        bool removed = collection.Remove(item);

        Assert.That(removed, Is.False);
        Assert.That(collection.Count, Is.EqualTo(count));
        Assert.That(collection, Is.EqualTo(expected));
    }

    [Test]
    public void TestRemoveWhenItemIsPresentRemovesItemAndReturnsTrue()
    {
        // Create collection
        const int count = 3;
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);
        IList<INUnitFilterBaseElement> expected = new List<INUnitFilterBaseElement>(collection);
        expected.RemoveAt(0);

        Assert.That(collection.Count, Is.EqualTo(count));

        bool removed = collection.Remove(collection.First());

        Assert.That(removed, Is.True);
        Assert.That(collection.Count, Is.EqualTo(count - 1));
        Assert.That(collection, Is.EqualTo(expected));
    }

    #endregion

    #region Tests for Count Property

    [Test]
    public void TestCountReturnsNumberOfItems([Range(0, 3)] int count)
    {
        // Create collection
        ExpressionCollection<INUnitFilterBaseElement> collection =
            NUnitFilterTestHelper.CreateCollection(count, out _);

        Assert.That(collection.Count, Is.EqualTo(count));
    }

    #endregion

    #region Tests for IsReadOnly Property

    [Test]
    public void TestIsReadOnlyReturnsFalse()
    {
        // ReSharper disable once CollectionNeverQueried.Local
        ExpressionCollection<INUnitFilterBaseElement> collection =
            new ExpressionCollection<INUnitFilterBaseElement>(NUnitFilterTestHelper.XmlAndTag);

        Assert.That(collection.IsReadOnly, Is.False);
    }

    #endregion
}