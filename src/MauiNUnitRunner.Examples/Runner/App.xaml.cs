// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
//using MauiNUnitRunner.Controls.Services;
using MauiNUnitRunner.Controls.Views;
using MauiNUnitRunner.Examples.SubAssemblyTests;

namespace MauiNUnitRunner.Examples.Runner;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set app theme
        UserAppTheme = AppTheme.Dark;

        // Get assemblies with unit tests
        IList<Assembly> testAssemblies = new List<Assembly>();
        testAssemblies.Add(GetType().Assembly);
        testAssemblies.Add(typeof(ExampleSubUnitTests).Assembly);

        // Specify any test settings
        Dictionary<string, object> settings = new Dictionary<string, object>();
        settings.Add("MySetting", "value");

        // Create initial test page
        TestDynamicPage page = new TestDynamicPage(testAssemblies, settings);

        // Add an optional test listener to get test output and progress
        //NUnitTestListener listener = new NUnitTestListener();
        //listener.WriteOutput += Console.WriteLine;
        //page.TestListener = listener;

        // Set test page as main page
        MainPage = new NavigationPage(page);
    }

    /// <inheritdoc cref="CreateWindow"/>
    protected override Window CreateWindow(IActivationState? activationState)
    {
        Window window = base.CreateWindow(activationState);

        // Set app's starting width and height
        window.Width = 1600;
        window.Height = 900;

        return window;
    }
}