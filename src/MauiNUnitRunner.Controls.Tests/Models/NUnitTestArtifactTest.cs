// Copyright (c) bstadick and contributors. MIT License - see LICENSE file

using NUnit.Framework;
using NUnit.Framework.Interfaces;
using NUnit.Framework.Internal;
using MauiNUnitRunner.Controls.Models;

namespace MauiNUnitRunner.Controls.Tests.Models;

[TestFixture]
public class NUnitTestArtifactTest
{
    #region Tests for Constructor

    [Test]
    public void TestConstructorWithTest([Values] bool isNull)
    {
        ITest test = isNull ? null : new TestSuite("suite-name");

        NUnitTestArtifact artifact = new NUnitTestArtifact(test);

        Assert.That(artifact.Test, Is.Not.Null);
        Assert.That(artifact.Test.Test, Is.SameAs(test));
    }

    #endregion

    #region Tests for Test Property

    [Test]
    public void TestTestPropertyReturnsTest([Values] bool isNull)
    {
        ITest test = isNull ? null : new TestSuite("suite-name");

        NUnitTestArtifact artifact = new NUnitTestArtifact(test);

        Assert.That(artifact.Test, Is.Not.Null);
        Assert.That(artifact.Test.Test, Is.SameAs(test));
    }

    #endregion

    #region Tests for Outputs Property

    [Test]
    public void TestOutputsPropertyReturnsTestOutputList()
    {
        ITest test = new TestSuite("suite-name");
        TestOutput testOutput = new TestOutput("text", "stream", "id", "name");
        IList<TestOutput> expectedOutputs = new List<TestOutput> {testOutput, null};

        NUnitTestArtifact artifact = new NUnitTestArtifact(test);

        IList<TestOutput> outputs = artifact.Outputs;

        Assert.That(outputs, Is.Not.Null);
        Assert.That(outputs, Is.Empty);

        outputs.Add(testOutput);
        outputs.Add(null);

        Assert.That(artifact.Outputs, Is.EqualTo(expectedOutputs));
    }

    #endregion

    #region Tests for Messages Property

    [Test]
    public void TestMessagesPropertyReturnsTestMessageList()
    {
        ITest test = new TestSuite("suite-name");
        TestMessage testMessage = new TestMessage("destination", "text", "id");
        IList<TestMessage> expectedMessages = new List<TestMessage> {testMessage, null};

        NUnitTestArtifact artifact = new NUnitTestArtifact(test);

        IList<TestMessage> messages = artifact.Messages;

        Assert.That(messages, Is.Not.Null);
        Assert.That(messages, Is.Empty);

        messages.Add(testMessage);
        messages.Add(null);

        Assert.That(artifact.Messages, Is.EqualTo(expectedMessages));
    }

    #endregion

    #region Tests for ToString

    [Test]
    public void TestToStringReturnsTestFullNameWhenTestNotNull()
    {
        ITest test = new TestSuite("suite-name");

        NUnitTestArtifact artifact = new NUnitTestArtifact(test);

        Assert.That(artifact.ToString(), Is.EqualTo(test.FullName));
    }

    [Test]
    public void TestToStringReturnsTestNullMessageWhenTestIsNull()
    {
        NUnitTestArtifact artifact = new NUnitTestArtifact(null);

        Assert.That(artifact.ToString(), Is.EqualTo("NUnitTestArtifacts: ITest null"));
    }

    #endregion
}