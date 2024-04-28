// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Models;
using MauiNUnitRunner.Controls.Resources;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Services;

/// <summary>
///     Implements a <see cref="INUnitTestRunner"/> to run NUnit tests.
/// </summary>
public class NUnitTestRunner : INUnitTestRunner
{
    #region Private Members

    /// <summary>
    ///     Holds the underlying unit test assembly runner.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected readonly INUnitTestAssemblyRunner v_TestRunner;

    /// <summary>
    ///     Holds the underlying test listener.
    /// </summary>
    // ReSharper disable once InconsistentNaming
    protected readonly NUnitCompositeTestListener v_TestListener;

    #endregion

    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="NUnitTestRunner"/> instance.
    /// </summary>
    public NUnitTestRunner() : this(
        new NUnitTestAssemblyRunnerWrapper(new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder())))
    {
    }

    /// <summary>
    ///     Initializes a new <see cref="NUnitTestRunner"/> instance with the given <see cref="INUnitTestAssemblyRunner"/>.
    /// </summary>
    /// <param name="runner">The underlying <see cref="INUnitTestAssemblyRunner"/>.</param>
    protected NUnitTestRunner(INUnitTestAssemblyRunner runner)
    {
        v_TestRunner = runner ?? throw ExceptionHelper.ThrowArgumentNullException(nameof(runner));
        v_TestListener = new NUnitCompositeTestListener();
    }

    #endregion

    #region Implementation of INUnitTestRunner

    /// <inheritdoc />
    public NUnitTestAssemblyRunner TestRunner => v_TestRunner.TestRunner;

    /// <inheritdoc />
    public bool IsTestRunning => v_TestRunner.IsTestRunning;

    /// <inheritdoc />
    public void AddTestAssembly(Assembly assembly, IDictionary<string, object> settings = null)
    {
        if (assembly == null)
        {
            return;
        }

        settings ??= new Dictionary<string, object>();
        v_TestRunner.Load(assembly, settings);
    }

    /// <inheritdoc />
    public INUnitTest ExploreTests(ITestFilter filter = null)
    {
        filter ??= NUnitFilter.Empty;
        ITest tests = v_TestRunner.ExploreTests(filter);
        return new NUnitTest(tests);
    }

    /// <inheritdoc />
    public async Task<INUnitTestResult> Run(ITestFilter filter = null)
    {
        // Only allow one test run to run at a time
        if (v_TestRunner.IsTestRunning)
        {
            return await Task.FromResult((INUnitTestResult)null);
        }

        // Run tests
        filter ??= NUnitFilter.Empty;
        Task<INUnitTestResult> runTask = Task.Run(() =>
        {
            ITestResult result = v_TestRunner.Run(v_TestListener, filter);
            return (INUnitTestResult)(new NUnitTestResult(result));
        });

        // Wait for test to complete
        return await runTask;
    }

    /// <inheritdoc />
    public void StopRun(bool force)
    {
        v_TestRunner.StopRun(force);
    }

    /// <inheritdoc />
    public bool WaitForCompletion(int timeout)
    {
        return v_TestRunner.WaitForCompletion(timeout);
    }

    /// <inheritdoc />
    public Stream GetTestResultsAsXmlStream(INUnitTestResult result, out string fileName)
    {
        fileName = string.Empty;
        if (result == null || !result.HasTestResult)
        {
            return null;
        }

        // Convert result string to stream, use an XmlTextWrite and XDocument to apply xml formatting
        MemoryStream resultStream = new MemoryStream();
        try
        {
            // Get result as xml
            TNode resultXml = result.Result.ToXml(true);

            // Write xml to stream with formatting
            XmlWriterSettings settings = new XmlWriterSettings();
            settings.CloseOutput = false;
            settings.OmitXmlDeclaration = false;
            settings.Indent = true;
            settings.Encoding = Encoding.UTF8;
            using (XmlWriter writer = XmlWriter.Create(resultStream, settings))
            {
                XDocument doc = XDocument.Parse(resultXml.OuterXml);
                doc.WriteTo(writer);
                writer.Flush();
            }

            // Set stream back to beginning
            resultStream.Seek(0, SeekOrigin.Begin);

            // Format full test name for initial file, remove any part of a test case string after a '(' and replacing spaces with '-'
            INUnitTest test = new NUnitTest(result.Result.Test);
            string testName = test.FullDisplayName;
            int ind = testName.IndexOf('(', StringComparison.Ordinal);
            if (ind > 0)
            {
                testName = testName.Substring(0, ind);
            }

            testName = testName.Trim().Replace(" ", "-");
            testName = string.IsNullOrEmpty(testName) ? "test" : testName;
            fileName = $"{testName}-{test.Id}.xml";
        }
        catch
        {
            resultStream.Dispose();
            throw;
        }

        return resultStream;
    }

    /// <inheritdoc />
    public void AddTestListener(ITestListener listener)
    {
        v_TestListener.AddTestListener(listener);
    }

    /// <inheritdoc />
    public void RemoveTestListener(ITestListener listener)
    {
        v_TestListener.RemoveTestListener(listener);
    }

    #endregion
}