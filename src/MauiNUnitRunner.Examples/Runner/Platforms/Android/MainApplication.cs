// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using Android.App;
using Android.Runtime;

namespace MauiNUnitRunner.Examples.Runner
{
    [Application]
    public class MainApplication : MauiApplication
    {
        public MainApplication(IntPtr handle, JniHandleOwnership ownership)
            : base(handle, ownership)
        {
        }

        protected override MauiApp CreateMauiApp() => MauiProgram.CreateMauiApp();
    }
}