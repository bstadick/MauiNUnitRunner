// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using Foundation;

namespace MauiNUnitRunner.Examples.Runner
{
    [Register("AppDelegate")]
    public class AppDelegate : MauiUIApplicationDelegate
    {
        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}