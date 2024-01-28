// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using Android.App;
using Android.Content.PM;

namespace MauiNUnitRunner.Examples
{
    [Activity(Theme = "@style/Maui.SplashTheme", MainLauncher = true, ConfigurationChanges = ConfigChanges.ScreenSize | ConfigChanges.Orientation | ConfigChanges.UiMode | ConfigChanges.ScreenLayout | ConfigChanges.SmallestScreenSize | ConfigChanges.Density)]
    public class MainActivity : MauiAppCompatActivity
    // ReSharper disable once RedundantTypeDeclarationBody
    {
    }
}