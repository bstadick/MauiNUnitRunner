// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Implementation of <see cref="INUnitTest" />.
/// </summary>
public class NUnitTest : INUnitTest
{
    #region Private Members

    /// <summary>
    ///     Constant for a dll file extension.
    /// </summary>
    private const string c_DllExtension = ".dll";

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="NUnitTest" /> with the given <see cref="ITest" /> and optional <see cref="INUnitTestResult"/>.
    /// </summary>
    /// <param name="test">The test to initialize with.</param>
    /// <param name="result">The test result to initialize with.</param>
    public NUnitTest(ITest test, INUnitTestResult result = null)
    {
        Test = test;
        // ReSharper disable once ConstantConditionalAccessQualifier
        IList<INUnitTest> children = test?.Tests?.Select(childTest => (INUnitTest)(new NUnitTest(childTest))).ToList();
        Children = children != null ? new ObservableCollection<INUnitTest>(children) : new ObservableCollection<INUnitTest>();
        Result = result;
    }

    #endregion

    #region Implementation of INUnitTest

    /// <inheritdoc />
    public string Id => Test?.Id ?? string.Empty;

    /// <inheritdoc />
    public ITest Test { get; }

    /// <summary>
    ///     Holds the underlying value of <see cref="Result"/>.
    /// </summary>

    private INUnitTestResult v_Result;

    /// <inheritdoc />
    public INUnitTestResult Result
    {
        get => v_Result;
        set
        {
            // Walk tree until matching result is found since tree starts at top even if ran from a test lower down.
            // e.g. Run test with ID 1004 but get results like: ID 1005 > ID 1004 > ID 1003, 1002, 1001
            v_Result = MatchChildResult(Id, value);

            // Recursively set results of children if both test and results have children
            // ReSharper disable once MergeIntoPattern
            if (v_Result != null && v_Result.HasChildren && HasChildren)
            {
                // Set result of each of the current test's children
                foreach (INUnitTest childTest in Children)
                {
                    // Match the child test to the child test result
                    // After initially walking the results tree above,
                    // it is guaranteed that the child test results will be at the same level as the current test
                    INUnitTestResult matchingResult = v_Result.Children.FirstOrDefault(childResult => childResult.Id == childTest.Test.Id);
                    if (matchingResult != null)
                    {
                        // By setting the child INUnitTest.Result property, this property setter will be called recursively
                        childTest.Result = matchingResult;
                    }
                }
            }

            // Raise event that the property and its dependent properties have changed
            OnPropertyChanged();
            OnPropertyChanged(nameof(TextColor));
        }
    }

    /// <inheritdoc />
    public bool HasTestResult => Result != null;

    /// <inheritdoc />
    public IList<INUnitTest> Children { get; }

    /// <inheritdoc />
    public bool HasChildren => Children != null && Children.Any();

    /// <inheritdoc />
    public bool HasNoChildren => !HasChildren;

    /// <inheritdoc />
    public Color TextColor => Result?.TextColor ?? ResourceHelper.GetCurrentThemeForegroundColor();

    /// <inheritdoc />
    public string DisplayName
    {
        get
        {
            if (Test?.Name == null)
            {
                return string.Empty;
            }

            // Remove extension from file name if present
            return Test.Name.EndsWith(c_DllExtension, StringComparison.OrdinalIgnoreCase)
                ? $"{Path.GetFileNameWithoutExtension(Test.Name)} {ResourceHelper.GetResourceString("TestAssemblyTitleHeader")}"
                : Test.Name;
        }
    }

    /// <inheritdoc />
    public string FullDisplayName
    {
        get
        {
            if (Test?.FullName == null)
            {
                return string.Empty;
            }

            // Remove extension from file name if present
            return Test.FullName.EndsWith(c_DllExtension, StringComparison.OrdinalIgnoreCase)
                ? $"{Path.GetFileNameWithoutExtension(Test.FullName)} {ResourceHelper.GetResourceString("TestAssemblyTitleHeader")}"
                : Test.FullName;
        }
    }

    /// <inheritdoc />
    public string ConditionalDisplayName
    {
        get
        {
            if (Test?.Name == null)
            {
                return string.Empty;
            }

            return Test.IsSuite && Test.ClassName == null && Test.Method == null ? FullDisplayName : DisplayName;
        }
    }

    /// <inheritdoc />
    public INUnitTest SkipSingleTestSuites()
    {
        INUnitTest test = this;

        // Skip past tests that only have one child test as displaying them isn't very useful
        // ReSharper disable once MergeIntoPattern
        while (test != null && test.HasChildren && test.Children.Count == 1)
        {
            test = test.Children.FirstOrDefault();
        }

        return test;
    }

    #endregion

    #region Implementation of INotifyPropertyChanged

    /// <inheritdoc />
    public event PropertyChangedEventHandler PropertyChanged;

    /// <summary>
    ///     Raises the event handler indicating that a property has changed.
    /// </summary>
    /// <param name="propertyName">The name of the property that has changed, or default to use the name of the calling property.</param>
    protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    #endregion

    #region Private Methods

    /// <summary>
    ///     Gets the matching child <see cref="INUnitTestResult"/> the matches the given <paramref name="id"/>.
    /// </summary>
    /// <remarks>
    ///     Performs a breadth first search for the matching test.
    ///     This is generally going to be the most performant for the way the test results are structured.
    /// </remarks>
    /// <param name="id">The <see cref="ITestResult"/> id to match.</param>
    /// <param name="result">The starting <see cref="INUnitTestResult"/> to search from.</param>
    /// <returns>The matching <see cref="INUnitTestResult"/> or the given result if no match found.</returns>
    private static INUnitTestResult MatchChildResult(string id, INUnitTestResult result)
    {
        if (result == null || string.IsNullOrEmpty(id))
        {
            return result;
        }

        // Base case of the initial node is the matching node
        if (result.Id == id)
        {
            return result;
        }

        Queue<INUnitTestResult> toExplore = new Queue<INUnitTestResult>();
        HashSet<string> exploredIds = new HashSet<string>();

        // Add initial node
        toExplore.Enqueue(result);
        exploredIds.Add(result.Id);

        // Explore each node in a breadth first search, starting with the root node
        while (toExplore.TryDequeue(out INUnitTestResult node))
        {
            // Result found
            if (node.Id == id)
            {
                return node;
            }

            // Iterate through children if present
            if (node.HasChildren)
            {
                foreach (INUnitTestResult child in node.Children)
                {
                    // Add the child result to be explored if not already explored
                    if (exploredIds.Add(child.Id))
                    {
                        toExplore.Enqueue(child);
                    }
                }
            }
        }

        // Result not found
        return result;
    }

    #endregion
}