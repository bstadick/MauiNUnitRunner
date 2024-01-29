// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using MauiNUnitRunner.Controls.Filter;
using MauiNUnitRunner.Controls.Models;
using NUnit.Framework.Api;
using NUnit.Framework.Interfaces;

namespace MauiNUnitRunner.Controls.Services;

/// <summary>
///     Implements a <see cref="INUnitTestRunner"/> to run NUnit tests.
/// </summary>
public class NUnitTestRunner : INUnitTestRunner
{
    #region Constructors

    /// <summary>
    ///     Initializes a new <see cref="NUnitTestRunner"/> instance.
    /// </summary>
    public NUnitTestRunner()
    {
        TestRunner = new NUnitTestAssemblyRunner(new DefaultTestAssemblyBuilder());
    }

    #endregion

    #region Implementation of ITestListener

    /// <inheritdoc />
    public NUnitTestAssemblyRunner TestRunner { get; }

    /// <inheritdoc />
    public bool IsTestRunning => TestRunner.IsTestRunning;

    /// <inheritdoc />
    public ITestListener TestListener { get; set; }

    /// <inheritdoc />
    public void AddTestAssembly(Assembly assembly, IDictionary<string, object> settings = null)
    {
        if (assembly == null)
        {
            return;
        }

        settings ??= new Dictionary<string, object>();
        TestRunner.Load(assembly, settings);
    }

    /// <inheritdoc />
    public INUnitTest ExploreTests(ITestFilter filter = null)
    {
        filter ??= NUnitFilter.Empty;
        ITest tests = TestRunner.ExploreTests(filter);
        return new NUnitTest(tests);
    }

    /// <inheritdoc />
    public async Task<INUnitTestResult> Run(ITestFilter filter = null)
    {
        // Only allow one test run to run at a time
        if (TestRunner.IsTestRunning)
        {
            return await Task.FromResult((INUnitTestResult)null);
        }

        // Run tests
        filter ??= NUnitFilter.Empty;
        Task<INUnitTestResult> runTask = Task.Run(() =>
        {
            ITestResult result = TestRunner.Run(TestListener, filter);
            return (INUnitTestResult)(new NUnitTestResult(result));
        });

        // Wait for test to complete
        return await runTask;
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

    /// <inheritdoc />
    public Stream GetTestResultsAsXmlStream(INUnitTestResult result, out string fileName)
    {
        fileName = string.Empty;
        if (result == null || !result.HasTestResult)
        {
            return null;
        }

        // Get result as xml
        TNode resultXml = result.Result.ToXml(true);

        // Convert result string to stream, use an XmlTextWrite and XDocument to apply xml formatting
        MemoryStream resultStream = new MemoryStream();
        try
        {
            using (XmlTextWriter writer = new XmlTextWriter(resultStream, Encoding.UTF8))
            {
                writer.Formatting = Formatting.Indented;
                XDocument doc = XDocument.Parse(resultXml.OuterXml);
                doc.WriteTo(writer);
                writer.Flush();
            }

            resultStream.Seek(0, SeekOrigin.Begin);

            // Format full test name for initial file, remove any part of a test case string after a '(' and replacing spaces with '-'
            INUnitTest test = new NUnitTest(result.Result.Test);
            string testName = test.FullDisplayName;
            int ind = testName.IndexOf('(', StringComparison.Ordinal);
            if (ind > 0)
            {
                testName = testName.Substring(0, ind);
            }

            testName = testName.Replace(" ", "-");
            fileName = $"{testName}-{test.Id}.xml";
        }
        catch
        {
            resultStream.Dispose();
            throw;
        }

        return resultStream;
    }

    #endregion
}