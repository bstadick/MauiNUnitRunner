// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Models;

/// <summary>
///     Class that holds the artifacts of a test run.
/// </summary>
public class NUnitTestArtifact
{
    #region Public Members

    /// <summary>
    ///     The <see cref="INUnitTest" /> that the artifact belongs to.
    /// </summary>
    public INUnitTest Test { get; }

    /// <summary>
    ///     The outputs of the test.
    /// </summary>
    public IList<TestOutput> Outputs { get; } = new List<TestOutput>();

    /// <summary>
    ///     The messages produced by the test.
    /// </summary>
    public IList<TestMessage> Messages { get; } = new List<TestMessage>();

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="NUnitTestArtifact" /> with the given <see cref="ITest" />.
    /// </summary>
    /// <param name="test">The test to initialize with.</param>
    public NUnitTestArtifact(ITest test)
    {
        Test = new NUnitTest(test);
    }

    #endregion

    #region Public Methods

    /// <inheritdoc />
    public override string ToString()
    {
        return Test?.Test?.FullName ?? "NUnitTestArtifacts: ITest null";
    }

    #endregion
}