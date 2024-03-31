// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;
using System.Reflection;
using MauiNUnitRunner.Controls.Resources;

namespace MauiNUnitRunner.Controls.Services
{
    /// <summary>
    ///     Interface to wrap the <see cref="NUnitTestAssemblyRunner"/> for stubbing purposes.
    /// </summary>
    public interface INUnitTestAssemblyRunner
    {
        #region Public Members

        /// <summary>
        ///     Gets the underlying unit test assembly runner.
        /// </summary>
        public NUnitTestAssemblyRunner TestRunner { get; }

        /// <inheritdoc cref="NUnitTestAssemblyRunner.IsTestRunning"/>
        bool IsTestRunning { get; }

        #endregion

        #region Public Methods

        /// <inheritdoc cref="NUnitTestAssemblyRunner.Load(Assembly, IDictionary&lt;string, object&gt;)"/>
        void Load(Assembly assembly, IDictionary<string, object> settings);

        /// <inheritdoc cref="NUnitTestAssemblyRunner.ExploreTests"/>
        ITest ExploreTests(ITestFilter filter);

        /// <inheritdoc cref="NUnitTestAssemblyRunner.Run"/>
        ITestResult Run(ITestListener listener, ITestFilter filter);

        /// <inheritdoc cref="NUnitTestAssemblyRunner.StopRun"/>
        void StopRun(bool force);

        /// <inheritdoc cref="NUnitTestAssemblyRunner.WaitForCompletion"/>
        bool WaitForCompletion(int timeout);

        #endregion
    }

    /// <summary>
    ///     Implementation of <see cref="INUnitTestAssemblyRunner"/> to wrap a <see cref="NUnitTestAssemblyRunner"/> instance.
    /// </summary>
    internal class NUnitTestAssemblyRunnerWrapper : INUnitTestAssemblyRunner
    {

        #region Constructors

        /// <summary>
        ///     Initializes a new <see cref="NUnitTestAssemblyRunnerWrapper"/> instance with the given <see cref="NUnitTestAssemblyRunner"/>.
        /// </summary>
        /// <param name="runner">The underlying <see cref="NUnitTestAssemblyRunner"/>.</param>
        /// <exception cref="ArgumentNullException">Thrown if the <paramref name="runner"/> is null.</exception>
        public NUnitTestAssemblyRunnerWrapper(NUnitTestAssemblyRunner runner)
        {
            TestRunner = runner ?? throw ExceptionHelper.ThrowArgumentNullException(nameof(runner));
        }

        #endregion

        #region Implementation of INUnitTestAssemblyRunner

        /// <inheritdoc />
        public NUnitTestAssemblyRunner TestRunner { get; }

        /// <inheritdoc />
        public bool IsTestRunning => TestRunner.IsTestRunning;

        /// <inheritdoc />
        public void Load(Assembly assembly, IDictionary<string, object> settings)
        {
            TestRunner.Load(assembly, settings);
        }

        /// <inheritdoc />
        public ITest ExploreTests(ITestFilter filter)
        {
            return TestRunner.ExploreTests(filter);
        }

        /// <inheritdoc />
        public ITestResult Run(ITestListener listener, ITestFilter filter)
        {
            return TestRunner.Run(listener, filter);
        }

        /// <inheritdoc />
        public void StopRun(bool force)
        {
            TestRunner.StopRun(force);
        }

        /// <inheritdoc />
        public bool WaitForCompletion(int timeout)
        {
            return TestRunner.WaitForCompletion(timeout);
        }

        #endregion
    }
}