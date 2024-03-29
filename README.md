# MauiNUnitRunner

NUnit test runner for .NET MAUI based projects.

This project looks to revive the [NUnit](https://github.com/nunit/nunit.xamarin) and [Xamarin NUnitLite](https://github.com/xamarin) unit test runner projects and combine the granular interface provided by the Xamarin runner with the full functionality provided by the NUnit framework and built for .NET MAUI.

Features include:

- Exploring loaded tests by namespace, class, and method.
- Running tests and test cases individually or by namespace, class, or method.
- Viewing overall results and individual results with details.
- Writes output of test results to the console as tests are ran.
- Running tests on a background thread, leaving the UI thread available to perform other work.
  - This can also help avoid deadlock situations when a test requires the UI thread.
- Supports all major platforms supported by .NET MAUI.
- Extensible design allows for customization of existing or creation of new behaviors and views.

## Usage

1. Include a reference to the `MauiNUnitRunner.Controls` Nuget package in a .NET MAUI app project for your target platform(s).
2. Include references to the project(s) with your NUnit tests or the test code itself in the .NET MAUI app project.
3. On the .NET MAUI app project's `MauiAppBuilder` method, call the `.UseMauiNUnitRunner()` method to initialize the MauiNUnitRunner controls.

```csharp
using MauiNUnitRunner.Controls;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize using the MauiNUnitRunner
            .UseMauiNUnitRunner();

        return builder.Build();
    }
}
```

4. In the .NET MAUI app project's `App.xaml.cs` constructor, create and load an instance of the `MauiNUnitRunner.Controls.TestDynamicPage` **ContentPage** and set as the `App.MainPage`.
5. The `TestDynamicPage` takes in a list of the assemblies to load the tests from. This should include the current app's assembly if it contains tests to run.

```csharp
using MauiNUnitRunner.Controls;

public partial class App : Application
{
    public App()
    {
        InitializeComponent();

        // Get assemblies with unit tests
        IList<Assembly> testAssemblies = new List<Assembly>();
        testAssemblies.Add(GetType().Assembly);
        testAssemblies.Add(typeof(MyUnitTestClass).Assembly);

        // Specify any test settings
        Dictionary<string, object> settings = new Dictionary<string, object>();
        settings.Add("MySetting", "value");

        // Create initial test page
        TestDynamicPage page = new TestDynamicPage(testAssemblies, settings);

        // Set test page as main page
        MainPage = new NavigationPage(page);
    }
}
```

6. Build the .NET MAUI app project in **Debug**. NUnit will not be able to work correctly in **Release** builds.
7. To add more or change tests that are loaded or change test settings, create and set a new `TestDynamicPage` as the `App.MainPage` or directly access the underlying `NUnit.Framework.Api.NUnitTestAssemblyRunner` from the `TestDynamicPage.TestRunner` property.

### Test Listener

Add an `NUnit.Framework.Interfaces.ITestListener` such as the `MauiNUnitRunner.Controls.NUnitTestListener` to the `TestDynamicPage.TestRunner` property to output tests messages and results as tests are ran.

```csharp
// Add an optional test listener to get test output and progress
NUnitTestListener listener = new NUnitTestListener();
listener.WriteOutput += Console.WriteLine;
page.TestListener = listener;
```

### Extensibility

The `MauiNUnitRunner.Controls` namespace exposes the individual **ContentViews** used to populate the `TestDynamicPage` as well as marking a number of key methods virtual. This allows for the construction of custom view and pages or modification of existing page's behavior.

## Build

Use the provided `./src/MauiNUnitRunner.sln` solution and Visual Studio 2022 with .NET 8 to build the project.

The MauiNUnitRunner project can be built and referenced using the pre-built Nuget package.

## Examples

Example test runner projects can be found in the `./src/MauiNUnitRunner.Examples` project folders. The examples consist of an example test runner app and an separate test sub-assembly to include in the test runner app. The examples can be built from the project's solution using the **Example** configuration.

## Future Enhancements

- User interface beatification
- Performance improvements
- Running tests by category
- Searching and filtering tests
- Configuring test settings from UI
- Live UI updates as tests are progressing
- Remote test management

## Contributing

Pull requests for new features or fixes and reporting issues are welcome. When developing, follow patterns in existing code when grouping code elements within classes. Additional resources should be included as MAUI Resources and strings included in a resx file where applicable. Consider testability and use TDD to develop new code and make changes. Unit tests should maintain at least 95% code coverage.

## License

See the License file for licensing details.