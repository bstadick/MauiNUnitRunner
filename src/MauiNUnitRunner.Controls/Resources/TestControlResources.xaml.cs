// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

namespace MauiNUnitRunner.Controls.Resources;

/// <summary>
///     Defines the resources for use in Views and Code.
/// </summary>
// ReSharper disable once RedundantExtendsListEntry
public partial class TestControlResources : ResourceDictionary
{
    #region Private Members

    /// <summary>
    ///     Holds the <see cref="TestControlResources"/> instance singleton.
    /// </summary>
    private static readonly Lazy<TestControlResources> v_Resources =
        // ReSharper disable once ArrangeObjectCreationWhenTypeEvident
        new Lazy<TestControlResources>(() => new TestControlResources(), LazyThreadSafetyMode.PublicationOnly);

    #endregion

    #region Constructors

    /// <summary>
    ///     Static constructor.
    /// </summary>
    static TestControlResources()
    {
        // Add resources to current application's resource dictionary
        Application.Current?.Resources.MergedDictionaries.Add(GetInstance());
    }

    /// <summary>
    ///     Default constructor.
    /// </summary>
	public TestControlResources()
	{
		InitializeComponent();
	}

    #endregion

    #region Public Methods

    /// <summary>
    ///     Gets the <see cref="TestControlResources"/> instance singleton. 
    /// </summary>
    /// <returns>The <see cref="TestControlResources"/> instance singleton. </returns>
    public static TestControlResources GetInstance()
    {
        return v_Resources.Value;
    }

    #endregion
}