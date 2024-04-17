// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Views;
using NUnit.Framework;

// ReSharper disable ArrangeObjectCreationWhenTypeEvident

namespace MauiNUnitRunner.Controls.Tests.Views
{
    [TestFixture]
    public class AboutPageTest
    {
        #region Tests for Constructor

        [Test]
        public void TestConstructor()
        {
            AboutPage page = new AboutPageForTest();

            Assert.That(page.AssemblyVersion, Is.EqualTo("0.2.0"));
        }

        #endregion

        #region Tests for OpenUrlButton_OnClicked

        [Test]
        [TestCase("AboutMauiButton", "https://learn.microsoft.com/en-us/dotnet/maui")]
        [TestCase("AboutNUnitButton", "https://nunit.org/")]
        [TestCase("AboutProjectButton", "https://github.com/bstadick/MauiNUnitRunner")]
        public void OpenUrlButtonOnClickedOpensSelectedUrl(string name, string expectedUrl)
        {
            AboutPageForTest page = new AboutPageForTest();

            Button button = new Button { StyleId = name };

            page.InvokeOpenUrlButtonOnClicked(button, EventArgs.Empty);

            Assert.That(page.Browser.OpenAsyncInvoked, Is.True);
            Assert.That(page.Browser.OpenedUrl, Is.EqualTo(new Uri(expectedUrl)));
            Assert.That(page.Browser.LaunchOptions.LaunchMode, Is.EqualTo(BrowserLaunchMode.SystemPreferred));
        }

        [Test]
        public void OpenUrlButtonOnClickedOpensSelectedUrlWhenNotAButtonDoesNotOpenUrl([Values] bool isNull)
        {
            AboutPageForTest page = new AboutPageForTest();

            object button = isNull ? null : "button";

            page.InvokeOpenUrlButtonOnClicked(button, EventArgs.Empty);

            Assert.That(page.Browser.OpenAsyncInvoked, Is.False);
            Assert.That(page.Browser.OpenedUrl, Is.Null);
            Assert.That(page.Browser.LaunchOptions, Is.Null);
        }

        [Test]
        [TestCase(null)]
        [TestCase("")]
        [TestCase("NotAnAboutButton")]
        public void OpenUrlButtonOnClickedWhenButtonIdNotMappedDoesNotOpenUrl(string name)
        {
            AboutPageForTest page = new AboutPageForTest();

            Button button = new Button { StyleId = name };

            page.InvokeOpenUrlButtonOnClicked(button, EventArgs.Empty);

            Assert.That(page.Browser.OpenAsyncInvoked, Is.False);
            Assert.That(page.Browser.OpenedUrl, Is.Null);
            Assert.That(page.Browser.LaunchOptions, Is.Null);
        }

        #endregion

        #region Tests for GetDefaultBrowser

        [Test]
        public void TestGetDefaultBrowserReturnsDefaultBrowser()
        {
            AboutPageForTest page = new AboutPageForTest();

            IBrowser browser = page.InvokeGetDefaultBrowser();

            Assert.That(browser, Is.Not.Null.And.Not.TypeOf<BrowserForTest>());
        }

        #endregion

        #region Nested Class: AboutPageForTest

        /// <summary>
        ///     Extends AboutPage for use with tests.
        /// </summary>
        private class AboutPageForTest : AboutPage
        {
            #region Members for Test

            /// <summary>
            ///     Gets the browser instance for test.
            /// </summary>
            public BrowserForTest Browser { get; } = new BrowserForTest();

            #endregion

            #region Constructors

            /// <summary>
            ///     Initializes a new AboutPageForTest.
            /// </summary>
            public AboutPageForTest() : base(false)
            {
            }

            #endregion

            #region Methods for Test

            /// <summary>
            ///     Invokes the <see cref="AboutPage.OpenUrlButton_OnClicked"/> method.
            /// </summary>
            /// <param name="sender">The <see cref="Button"/> that was clicked.</param>
            /// <param name="e">The button click event arguments.</param>
            public void InvokeOpenUrlButtonOnClicked(object sender, EventArgs e)
            {
                OpenUrlButton_OnClicked(sender, e);
            }

            /// <summary>
            ///     Invokes the <see cref="AboutPage.GetDefaultBrowser"/> method.
            /// </summary>
            /// <returns>The <see cref="IBrowser"/> instance returned by <see cref="AboutPage.GetDefaultBrowser"/>.</returns>
            public IBrowser InvokeGetDefaultBrowser()
            {
                return base.GetDefaultBrowser();
            }

            #endregion

            #region Overridden Methods

            /// <inheritdoc />
            protected override IBrowser GetDefaultBrowser()
            {
                return Browser;
            }

            #endregion
        }

        #endregion

        #region Nested Class: BrowserForTest

        /// <summary>
        ///     Implements an IBrowser for use with tests.
        /// </summary>
        private class BrowserForTest : IBrowser
        {
            #region Members for Test

            /// <summary>
            ///     Gets the last url that was requested to be opened.
            /// </summary>
            public Uri OpenedUrl { get; private set; }

            /// <summary>
            ///     Gets the last browser launch options that was requested to be opened.
            /// </summary>
            public BrowserLaunchOptions LaunchOptions { get; private set; }

            /// <summary>
            ///     Gets if the OpenAsync method was invoked.
            /// </summary>
            public bool OpenAsyncInvoked { get; private set; }

            #endregion

            #region Implementation of IBrowser

            /// <inheritdoc />
            public Task<bool> OpenAsync(Uri uri, BrowserLaunchOptions options)
            {
                OpenAsyncInvoked = true;
                OpenedUrl = uri;
                LaunchOptions = options;
                return Task.FromResult(true);
            }

            #endregion
        }

        #endregion
    }
}
