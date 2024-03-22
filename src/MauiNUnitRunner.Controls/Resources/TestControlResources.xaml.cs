// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

namespace MauiNUnitRunner.Controls.Resources;

/// <summary>
///     Defines the resources for use in Views and Code.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TestControlResources : ResourceDictionary
{
    #region Constructors

    /// <summary>
    ///     Static constructor.
    /// </summary>
    static TestControlResources()
    {
        // Add resources to current application's resource dictionary
        Application.Current?.Resources.MergedDictionaries.Add(new TestControlResources());
    }

    /// <summary>
    ///     Default constructor.
    /// </summary>
	public TestControlResources()
	{
		InitializeComponent();
	}

    #endregion
}