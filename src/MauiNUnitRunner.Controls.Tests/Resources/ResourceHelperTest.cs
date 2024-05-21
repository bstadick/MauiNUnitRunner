// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework;

namespace MauiNUnitRunner.Controls.Tests.Resources;

[TestFixture]
public class ResourceHelperTest
{
    #region Private Members

    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    private readonly ResourceDictionary v_TestResourceDictionary = new ResourceDictionary();

    private const string c_StringForTestKey = "StringForTest";
    private const string c_StringForTestValue = "This is a test string";
    private const string c_NullForTestKey = "NullForTest";

    private const string c_StyleForTestKey = "StyleForTest";
    private readonly Color v_StyleColorForTestValue = Colors.Blue;
    // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
    private readonly Style v_StyleForTestValue = new Style(typeof(Label));
    private const string c_EmptyStyleSetterForTestKey = "EmptyStyleSetterForTest";
    private const string c_NullStyleSetterForTestKey = "NullStyleSetterForTest";
    private const string c_NullStyleSetterPropertyForTestKey = "NullStyleSetterPropertyForTestKey";
    private const string c_WrongStyleSetterPropertyNameForTestKey = "WrongStyleSetterPropertyNameForTestKey";
    private const string c_NullStyleSetterPropertyValueForTestKey = "NullStyleSetterPropertyValueForTestKey";

    #endregion

    #region Test SetUp/TearDown

    [OneTimeSetUp]
    public void TestFixtureSetUp()
    {
        v_TestResourceDictionary.Add(c_StringForTestKey, c_StringForTestValue);
        v_TestResourceDictionary.Add(c_NullForTestKey, null);

        // Add default style test case
        Setter setter = new Setter
        {
            Property = BindableProperty.Create("TextColor", typeof(Color), typeof(Label), v_StyleColorForTestValue),
            Value = v_StyleColorForTestValue
        };
        v_StyleForTestValue.Setters.Add(setter);
        v_TestResourceDictionary.Add(c_StyleForTestKey, v_StyleForTestValue);

        // Add style test case for empty setter list
        v_TestResourceDictionary.Add(c_EmptyStyleSetterForTestKey, new Style(typeof(Label)));

        // Add style test case for null setter
        Style nullSetterStyle = new Style(typeof(Label));
        nullSetterStyle.Setters.Add(null);
        v_TestResourceDictionary.Add(c_NullStyleSetterForTestKey, nullSetterStyle);

        // Add style test case for null property
        Style nullPropertyStyle = new Style(typeof(Label));
        nullPropertyStyle.Setters.Add(new Setter { Property = null, Value = v_StyleColorForTestValue });
        v_TestResourceDictionary.Add(c_NullStyleSetterPropertyForTestKey, nullPropertyStyle);

        // Add style test case for wrong property name
        Style wrongPropertyStyle = new Style(typeof(Label));
        Setter wrongPropertySetter = new Setter
        {
            Property = BindableProperty.Create("FontSize", typeof(Color), typeof(Label), v_StyleColorForTestValue),
            Value = v_StyleColorForTestValue
        };
        wrongPropertyStyle.Setters.Add(wrongPropertySetter);
        v_TestResourceDictionary.Add(c_WrongStyleSetterPropertyNameForTestKey, wrongPropertyStyle);

        // Add style test case for null property value
        Style nullValueStyle = new Style(typeof(Label));
        Setter nullValueSetter = new Setter
        {
            Property = BindableProperty.Create("TextColor", typeof(Color), typeof(Label), v_StyleColorForTestValue),
            Value = null
        };
        nullValueStyle.Setters.Add(nullValueSetter);
        v_TestResourceDictionary.Add(c_NullStyleSetterPropertyValueForTestKey, nullValueStyle);
    }

    [SetUp]
    public void TestSetUp()
    {
        ResourceHelper.UseOverriddenCurrentApplication = true;
        ResourceHelper.UseOverriddenResourceDictionary = true;

        ResourceHelper.CurrentApplication = new Application();
        ResourceHelper.ResourceDictionary = v_TestResourceDictionary;
    }

    [OneTimeTearDown]
    public void TestFixtureTearDown()
    {
        ResourceHelper.UseOverriddenCurrentApplication = false;
        ResourceHelper.UseOverriddenResourceDictionary = false;

        ResourceHelper.CurrentApplication = Application.Current;
        Application.Current?.Resources.MergedDictionaries.Add(TestControlResources.GetInstance());
    }

    #endregion

    #region Tests for GetResourceString

    [Test]
    public void TestGetResourceStringWhenCurrentApplicationOrResourcesNullReturnsNull([Values] bool isApplicationNull)
    {
        if (isApplicationNull)
        {
            ResourceHelper.UseOverriddenCurrentApplication = true;
            ResourceHelper.UseOverriddenResourceDictionary = false;
            ResourceHelper.CurrentApplication = null;

            Assert.That(ResourceHelper.CurrentApplication, Is.Null);
            Assert.That(ResourceHelper.ResourceDictionary, Is.Null);
        }
        else
        {
            ResourceHelper.UseOverriddenCurrentApplication = false;
            ResourceHelper.UseOverriddenResourceDictionary = true;
            ResourceHelper.ResourceDictionary = null;

            Assert.That(ResourceHelper.CurrentApplication, Is.Not.Null);
            Assert.That(ResourceHelper.ResourceDictionary, Is.Null);
        }

        string resource = ResourceHelper.GetResourceString(c_StringForTestKey);

        Assert.That(v_TestResourceDictionary, Does.ContainKey(c_StringForTestKey));
        Assert.That(v_TestResourceDictionary[c_StringForTestKey], Is.EqualTo(c_StringForTestValue));

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceStringWhenCurrentApplicationAndResourcesNotNullAndKeyPresentReturnsValue()
    {
        string resource = ResourceHelper.GetResourceString(c_StringForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_StringForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_StringForTestKey], Is.EqualTo(c_StringForTestValue));

        Assert.That(resource, Is.EqualTo(c_StringForTestValue));
    }

    [Test]
    public void TestGetResourceStringWhenCurrentApplicationAndResourcesNotNullAndKeyNotPresentReturnsNull()
    {
        const string key = "NotA" + c_StringForTestKey;
        string resource = ResourceHelper.GetResourceString(key);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.Not.ContainKey(key));

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceStringWhenCurrentApplicationAndResourcesNotNullAndValueNullReturnsNull()
    {
        string resource = ResourceHelper.GetResourceString(c_NullForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_NullForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_NullForTestKey], Is.Null);

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceStringWhenCurrentApplicationAndResourcesNotNullAndValueNotStringReturnsValueAsString()
    {
        string resource = ResourceHelper.GetResourceString(c_StyleForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_StyleForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_StyleForTestKey], Is.EqualTo(v_StyleForTestValue));

        Assert.That(resource, Is.Not.Null.And.Not.Empty.And.EqualTo(v_StyleForTestValue.ToString()));
    }

    #endregion

    #region Tests for GetResourceStyle

    [Test]
    public void TestGetResourceStyleWhenCurrentApplicationOrResourcesNullReturnsNull([Values] bool isApplicationNull)
    {
        if (isApplicationNull)
        {
            ResourceHelper.UseOverriddenCurrentApplication = true;
            ResourceHelper.UseOverriddenResourceDictionary = false;
            ResourceHelper.CurrentApplication = null;

            Assert.That(ResourceHelper.CurrentApplication, Is.Null);
            Assert.That(ResourceHelper.ResourceDictionary, Is.Null);
        }
        else
        {
            ResourceHelper.UseOverriddenCurrentApplication = false;
            ResourceHelper.UseOverriddenResourceDictionary = true;
            ResourceHelper.ResourceDictionary = null;

            Assert.That(ResourceHelper.CurrentApplication, Is.Not.Null);
            Assert.That(ResourceHelper.ResourceDictionary, Is.Null);
        }

        Style resource = ResourceHelper.GetResourceStyle(c_StyleForTestKey);

        Assert.That(v_TestResourceDictionary, Does.ContainKey(c_StyleForTestKey));
        Assert.That(v_TestResourceDictionary[c_StyleForTestKey], Is.EqualTo(v_StyleForTestValue));

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceStyleWhenCurrentApplicationAndResourcesNotNullAndKeyPresentReturnsValue()
    {
        Style resource = ResourceHelper.GetResourceStyle(c_StyleForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_StyleForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_StyleForTestKey], Is.EqualTo(v_StyleForTestValue));

        Assert.That(resource, Is.EqualTo(v_StyleForTestValue));
    }

    [Test]
    public void TestGetResourceStyleWhenCurrentApplicationAndResourcesNotNullAndKeyNotPresentReturnsNull()
    {
        const string key = "NotA" + c_StyleForTestKey;
        Style resource = ResourceHelper.GetResourceStyle(key);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.Not.ContainKey(key));

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceStyleWhenCurrentApplicationAndResourcesNotNullAndValueNullReturnsNull()
    {
        Style resource = ResourceHelper.GetResourceStyle(c_NullForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_NullForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_NullForTestKey], Is.Null);

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceStyleWhenCurrentApplicationAndResourcesNotNullAndValueNotStyleReturnsNull()
    {
        Style resource = ResourceHelper.GetResourceStyle(c_StringForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_StringForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_StringForTestKey], Is.EqualTo(c_StringForTestValue));

        Assert.That(resource, Is.Null);
    }

    #endregion

    #region Tests for GetResourceTextColor

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationOrResourcesNullReturnsNull([Values] bool isApplicationNull)
    {
        if (isApplicationNull)
        {
            ResourceHelper.UseOverriddenCurrentApplication = true;
            ResourceHelper.UseOverriddenResourceDictionary = false;
            ResourceHelper.CurrentApplication = null;

            Assert.That(ResourceHelper.CurrentApplication, Is.Null);
            Assert.That(ResourceHelper.ResourceDictionary, Is.Null);
        }
        else
        {
            ResourceHelper.UseOverriddenCurrentApplication = false;
            ResourceHelper.UseOverriddenResourceDictionary = true;
            ResourceHelper.ResourceDictionary = null;

            Assert.That(ResourceHelper.CurrentApplication, Is.Not.Null);
            Assert.That(ResourceHelper.ResourceDictionary, Is.Null);
        }

        Color resource = ResourceHelper.GetResourceTextColor(c_StyleForTestKey);

        Assert.That(v_TestResourceDictionary, Does.ContainKey(c_StyleForTestKey));
        Assert.That(v_TestResourceDictionary[c_StyleForTestKey], Is.EqualTo(v_StyleForTestValue));

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndKeyPresentReturnsValue()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_StyleForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_StyleForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_StyleForTestKey], Is.EqualTo(v_StyleForTestValue));

        Assert.That(resource, Is.EqualTo(v_StyleColorForTestValue));
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndKeyNotPresentReturnsNull()
    {
        const string key = "NotA" + c_StyleForTestKey;
        Color resource = ResourceHelper.GetResourceTextColor(key);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.Not.ContainKey(key));

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndValueNullReturnsNull()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_NullForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_NullForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_NullForTestKey], Is.Null);

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndValueNotStyleReturnsValueAsString()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_StringForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_StringForTestKey));
        Assert.That(ResourceHelper.ResourceDictionary[c_StringForTestKey], Is.EqualTo(c_StringForTestValue));

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndSetterListEmptyReturnsNull()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_EmptyStyleSetterForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_EmptyStyleSetterForTestKey));
        Style style = ResourceHelper.ResourceDictionary[c_EmptyStyleSetterForTestKey] as Style;
        Assert.That(style, Is.Not.Null);
        Assert.That(style.Setters, Is.Not.Null.And.Empty);

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndSetterNullReturnsNull()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_NullStyleSetterForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_NullStyleSetterForTestKey));
        Style style = ResourceHelper.ResourceDictionary[c_NullStyleSetterForTestKey] as Style;
        Assert.That(style, Is.Not.Null);
        Assert.That(style.Setters, Has.Count.EqualTo(1));
        Setter setter = style.Setters.First();
        Assert.That(setter, Is.Null);

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndSetterPropertyNullReturnsNull()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_NullStyleSetterPropertyForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_NullStyleSetterPropertyForTestKey));
        Style style = ResourceHelper.ResourceDictionary[c_NullStyleSetterPropertyForTestKey] as Style;
        Assert.That(style, Is.Not.Null);
        Assert.That(style.Setters, Has.Count.EqualTo(1));
        Setter setter = style.Setters.First();
        Assert.That(setter, Is.Not.Null);
        Assert.That(setter.Property, Is.Null);

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndSetterPropertyNameMismatchReturnsNull()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_WrongStyleSetterPropertyNameForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_WrongStyleSetterPropertyNameForTestKey));
        Style style = ResourceHelper.ResourceDictionary[c_WrongStyleSetterPropertyNameForTestKey] as Style;
        Assert.That(style, Is.Not.Null);
        Assert.That(style.Setters, Has.Count.EqualTo(1));
        Setter setter = style.Setters.First();
        Assert.That(setter, Is.Not.Null);
        BindableProperty property = setter.Property;
        Assert.That(property, Is.Not.Null);
        Assert.That(property.PropertyName, Is.Not.EqualTo("TextColor"));
        Assert.That(setter.Value, Is.Not.Null);

        Assert.That(resource, Is.Null);
    }

    [Test]
    public void TestGetResourceTextColorWhenCurrentApplicationAndResourcesNotNullAndSetterPropertyValueNullReturnsNull()
    {
        Color resource = ResourceHelper.GetResourceTextColor(c_NullStyleSetterPropertyValueForTestKey);

        Assert.That(ResourceHelper.ResourceDictionary, Is.Not.Null);
        Assert.That(ResourceHelper.ResourceDictionary, Does.ContainKey(c_NullStyleSetterPropertyValueForTestKey));
        Style style = ResourceHelper.ResourceDictionary[c_NullStyleSetterPropertyValueForTestKey] as Style;
        Assert.That(style, Is.Not.Null);
        Assert.That(style.Setters, Has.Count.EqualTo(1));
        Setter setter = style.Setters.First();
        Assert.That(setter, Is.Not.Null);
        BindableProperty property = setter.Property;
        Assert.That(property, Is.Not.Null);
        Assert.That(property.PropertyName, Is.EqualTo("TextColor"));
        Assert.That(setter.Value, Is.Null);

        Assert.That(resource, Is.Null);
    }

    #endregion

    #region Tests for GetCurrentThemeForegroundColor

    [Test]
    public void TestGetCurrentThemeForegroundColorWhenCurrentApplicationNullReturnsBlack()
    {
        ResourceHelper.CurrentApplication = null;

        Color resource = ResourceHelper.GetCurrentThemeForegroundColor();

        Assert.That(resource, Is.EqualTo(Colors.Black));
    }

    [Test]
    [TestCase(AppTheme.Unspecified, true)]
    [TestCase(AppTheme.Light, true)]
    [TestCase(AppTheme.Dark, false)]
    [TestCase((AppTheme)(-1), true)]
    public void TestGetCurrentThemeForegroundColorReturnsCorrespondingColor(AppTheme theme, bool expectedDark)
    {
        Color expected = expectedDark ? Colors.Black : Colors.White;

        // ReSharper disable once UseObjectOrCollectionInitializer
        ResourceHelper.CurrentApplication = new Application();
        ResourceHelper.CurrentApplication.UserAppTheme = theme;

        Color resource = ResourceHelper.GetCurrentThemeForegroundColor();

        Assert.That(resource, Is.EqualTo(expected));
    }

    #endregion
}