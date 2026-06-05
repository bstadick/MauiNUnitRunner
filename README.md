# MauiNUnitRunner

[![.NET Build/Publish](https://github.com/bstadick/MauiNUnitRunner/actions/workflows/dotnetcorerelease.yml/badge.svg?event=release)](https://github.com/bstadick/MauiNUnitRunner/actions/workflows/dotnetcorerelease.yml)
[![NuGet Version](https://img.shields.io/nuget/v/BStaSoft.MauiNUnitRunner.Controls)](https://www.nuget.org/packages/BStaSoft.MauiNUnitRunner.Controls)

NUnit test runner for .NET MAUI based projects.

This project looks to revive the [NUnit](https://github.com/nunit/nunit.xamarin) and [Xamarin NUnitLite](https://github.com/xamarin) unit test runner projects and combine the granular interface provided by the Xamarin runner with the full functionality provided by the NUnit framework and built for .NET MAUI.

Features include:

- Exploring loaded tests by namespace, class, and method.
- Running tests and test cases individually or by namespace, class, or method.
- Viewing overall results and individual results with details.
- Saving test results to a file.
- Ability to write the output of test results to the console as tests are ran.
- Running tests on a background thread, leaving the UI thread available to perform other work.
  - This can also help avoid deadlock situations when a test requires the UI thread.
- Supports all major platforms supported by .NET MAUI.
- Extensible design allows for customization of existing or creation of new behaviors and views.

> **Note:** The controls are optimized for landscape orientation on larger devices (e.g. tablets). While the application will function in portrait orientation and on smaller devices, some text may be clipped.

## Usage

1. Include a reference to the `BStaSoft.MauiNUnitRunner.Controls` Nuget package in a .NET MAUI app project for your target platform(s).
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

4. In the .NET MAUI app project's `App.xaml.cs` override of `CreateWindow`, create and return an instance of the `MauiNUnitRunner.Controls.TestDynamicPage` **ContentPage** which is wrapped in a **NavigationPage** which is returned as a **Window**.
5. The `TestDynamicPage` takes in an `INUnitTestRunner` to which test assemblies and test settings are added. This should include the current app's assembly if it contains tests to run.

    ```csharp
    using MauiNUnitRunner.Controls;

    public partial class App : Application
    {
        public App()
        {
            InitializeComponent();
        }

        /// <inheritdoc cref="CreateWindow" />
        protected override Window CreateWindow(IActivationState? activationState)
        {
            // It is recommended to set the app theme
            UserAppTheme = Current?.RequestedTheme ?? AppTheme.Unspecified;

            // Specify any test settings
            Dictionary<string, object> settings = new Dictionary<string, object>();
            settings.Add("MySetting", "value");

            // Add assemblies with unit tests to test runner
            NUnitTestRunner runner = new NUnitTestRunner();
            runner.AddTestAssembly(GetType().Assembly, settings);

            // Create initial test page
            TestDynamicPage page = new TestDynamicPage(runner);

            // Set test page as main page
            return new Window(new NavigationPage(page));
        }
    }
    ```

6. It is recommended to build the .NET MAUI app project in **Debug**. NUnit will not be able to work correctly on some platforms in **Release** builds.
7. To add more or change tests that are loaded or change test settings, create and set a new `TestDynamicPage` using `Application.Current.OpenWindow(Window)` or directly access the underlying `INUnitTestRunner` from the `TestDynamicPage.TestRunner` property.

### Test Listener

Add an `NUnit.Framework.Interfaces.ITestListener` such as the `MauiNUnitRunner.Controls.NUnitTestListener` using the `INUnitTestRunner.AddTestListener` method to output tests messages and results as tests are ran. Multiple and custom test listeners can be added.

```csharp
// Add an optional test listener to get test output and progress
NUnitTestListener listener = new NUnitTestListener();
listener.WriteOutput += Console.WriteLine;
runner.AddTestListener(listener);
```

### Extensibility

The `MauiNUnitRunner.Controls` namespace exposes the individual **ContentViews** used to populate the `TestDynamicPage` as well as marking a number of key methods virtual. This allows for the construction of custom views and pages or modification of the existing page's behavior.

## Build

Use the provided `./src/MauiNUnitRunner.sln` solution (to build everything) or the `./src/MauiNUnitRunner.Controls/MauiNUnitRunner.Controls.csproj` project directly and Visual Studio with .NET SDK and the .NET MAUI workloads to build the project.

Build the project or solution from Visual Studio or with the command `dotnet build <Solution|Project path> -c <Config>`.

The MauiNUnitRunner project can be built and then referenced in a .NET MAUI app using the produced `BStaSoft.MauiNUnitRunner.Controls.*.nupkg` Nuget package.

### Tools List

| Tool | Tool Version | Purpose | Notes |
| --- | --- | --- | --- |
| .NET SDK | Match `global.json` and `Directory.Build.props` for previous versions (recommended latest build versions) | Build, test, and run the solution (`dotnet build`, `dotnet test`, `dotnet run`) | Install from https://dotnet.microsoft.com/en-us/download and ensure `dotnet` is on PATH |
| .NET MAUI workloads | Latest matching SDK in `global.json` | Provide platform SDKs and build targets for MAUI | Run `dotnet workload restore ./src/MauiNUnitRunner.sln` |
| Visual Studio / IDE (optional) | Visual Studio 2026 or latest | Develop, debug, and platform-specific packaging (Windows/macOS) | Install MAUI workloads and workload-specific components |
| Java JDK | 11+ | Required for Android builds | Set `JAVA_HOME` to JDK install path |
| Android SDK / NDK | Platform-specific (SDK/NDK versions via Visual Studio) | Build and deploy Android apps | Install via Visual Studio or Android Studio SDK manager |
| Xcode (macOS only) | Latest compatible with target macOS/iOS | Build and deploy macOS/iOS apps | Required on macOS for macOS/iOS builds |
| Windows SDK (Windows only) | Windows 10/11 SDK matching target | Required for Windows / WinUI targets | Install via Visual Studio installer |

## Examples

Example test runner projects can be found in the `./src/MauiNUnitRunner.Examples` project folders. The examples consist of an example test runner app and a separate test sub-assembly to include in the test runner app. The examples can be built from the project's solution or `./src/MauiNUnitRunner.Examples.slnf` solution filter using the **Debug** or **Release** configuration.

See the **Build** section for instructions on building the projects. Refer to the `./.vscode/launch.json` file for `dotnet run` command arguments to deploy and run the example projects for various targets.

> **Note:** While the `MauiNUnitRunner.Controls` library may target multiple .NET versions, the examples will only target the latest .NET version.

> **Note:** Android examples built for **Release** are currently disabled due to a .NET SDK build tools issue. To test with Android, build the examples using the **Debug** configuration.

## Unit Tests

The project includes near complete code coverage of the `MauiNUnitRunner.Controls` namespace found in the `./src/MauiNUnitRunner.Controls.Tests` project folder. Build and run the unit tests using the project's solution or `./src/MauiNUnitRunner.Tests.slnf` solution filter using the **Debug** or **Release** configuration.

Run the tests from Visual Studio or with the command `dotnet test ./src/MauiNUnitRunner.sln -c <Config>`.

## Future Enhancements

The following are planned future enhancements in the general order of priority. Contributions towards any of these objectives is welcome per the Contributing guidelines in the section below.

- User interface beautification
- Updating overall results when child tests are ran individually.
  - Current behavior is the same as the older NUnit Lite runner, but is not the desired behavior.
  - This is limited currently by NUnit returning results for the entire test tree and not updating and recalculating the overall results when a subset of tests are ran. This could be handled in the MAUI test runner, but would be simpler and more efficient if handled within the underlying NUnit classes. NUnit will need to update such that overall parent results are calculated and cached from their child results and updated when a child result changes.
  - Will impact generation of test result report.
- Live UI updates (console output, results) as tests are progressing.
  - See ReSharper's Unit Test Sessions window as an example.
    - Reset results of tests to be ran.
    - Update individual test results as they are completed.
    - Update overall test result counts and progress bar color as tests are completed.
  - Currently limited by above point on updating overall results when child tests are ran.
- Export results to TRX and other common formats
- Performance improvements
- Loading multiple test assemblies
- Canceling active test run
- Running tests by category
- Searching and filtering tests
- Configuring test settings from UI
- Remote test management
- Portrait orientation and small screen optimization

## Contributing

Pull requests for new features or fixes and reporting issues are welcome. Before submitting a new issue or request, search the existing issues and requests to see if it has already been reported. Provide as much detail as possible when reporting a new issue or request.

When developing code, follow the patterns and conventions in the existing code, such as naming, indentation, casing, and grouping code elements within classes. Standard code formatting and cleanup is defined in the included .editorconfig file as well as with the ReSharper DotSettings file, when ReSharper is available. When including additional resources they should be added as MAUI Resources and strings included in a xaml dictionary file. Consider testability and use TDD to develop new code and make changes. Unit tests should always all pass and maintain at least 95% code coverage.

## License

See the License file for licensing details.
