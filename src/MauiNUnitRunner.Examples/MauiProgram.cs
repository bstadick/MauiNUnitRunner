// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using MauiNUnitRunner.Controls;
#if DEBUG
using Microsoft.Extensions.Logging;
#endif

namespace MauiNUnitRunner.Examples;

public static class MauiProgram
{
    public static MauiApp CreateMauiApp()
    {
        MauiAppBuilder builder = MauiApp.CreateBuilder();
        builder
            .UseMauiApp<App>()
            // Initialize using the MauiNUnitRunner
            .UseMauiNUnitRunner()
            .ConfigureFonts(fonts =>
            {
                fonts.AddFont("OpenSans-Regular.ttf", "OpenSansRegular");
                fonts.AddFont("OpenSans-Semibold.ttf", "OpenSansSemibold");
            });

#if DEBUG
        builder.Logging.AddDebug();
#endif

        return builder.Build();
    }
}