// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls.Services;
using MauiNUnitRunner.Controls.Views;

namespace MauiNUnitRunner.Examples.Runner;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Set app theme
        UserAppTheme = Current?.RequestedTheme ?? AppTheme.Unspecified;

        // Specify any test settings
        Dictionary<string, object> settings = new Dictionary<string, object>();
        settings.Add("MySetting", "value");

        // Add assemblies with unit tests to test runner
        NUnitTestRunner runner = new NUnitTestRunner();
        runner.AddTestAssembly(GetType().Assembly);

        // Create initial test page
        TestDynamicPage page = new TestDynamicPage(runner);

        // Add an optional test listener to get test output and progress
        //NUnitTestListener listener = new NUnitTestListener();
        //listener.WriteOutput += Console.WriteLine;
        //runner.AddTestListener(listener);

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