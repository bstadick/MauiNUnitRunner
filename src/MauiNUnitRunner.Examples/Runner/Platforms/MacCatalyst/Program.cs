// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using UIKit;

namespace MauiNUnitRunner.Examples.Runner
{
    public class Program
    {
        // This is the main entry point of the application.
        static void Main(string[] args)
        {
            // if you want to use a different Application Delegate class from "AppDelegate"
            // you can specify it here.
            UIApplication.Main(args, null, typeof(AppDelegate));
        }
    }
}