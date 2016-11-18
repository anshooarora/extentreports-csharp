using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class NodeSingleLogsStatusTests : Base
    {
        [Test]
        public void verifyIfTestHasStatusPass()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child").Pass("Pass");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(node.GetModel().LogContext().Count, 1);
            Assert.AreEqual(node.Status, Status.Pass);
            Assert.AreEqual(test.Status, Status.Pass);
        }

        [Test]
        public void verifyIfTestHasStatusSkip()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child").Skip("Skip");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(node.GetModel().LogContext().Count, 1);
            Assert.AreEqual(node.Status, Status.Skip);
            Assert.AreEqual(test.Status, Status.Skip);
        }

        [Test]
        public void verifyIfTestHasStatusWarning()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child").Warning("Warning");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(node.GetModel().LogContext().Count, 1);
            Assert.AreEqual(node.Status, Status.Warning);
            Assert.AreEqual(test.Status, Status.Warning);
        }

        [Test]
        public void verifyIfTestHasStatusError()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child").Error("Error");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(node.GetModel().LogContext().Count, 1);
            Assert.AreEqual(node.Status, Status.Error);
            Assert.AreEqual(test.Status, Status.Error);
        }

        [Test]
        public void verifyIfTestHasStatusFail()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child").Fail("Fail");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(node.GetModel().LogContext().Count, 1);
            Assert.AreEqual(node.Status, Status.Fail);
            Assert.AreEqual(test.Status, Status.Fail);
        }

        [Test]
        public void verifyIfTestHasStatusFatal()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child").Fatal("Fatal");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(node.GetModel().LogContext().Count, 1);
            Assert.AreEqual(node.Status, Status.Fatal);
            Assert.AreEqual(test.Status, Status.Fatal);
        }

        [Test]
        public void verifyIfTestHasStatusPassWithOnlyInfoSingle()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child").Info("Info");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(node.GetModel().LogContext().Count, 1);
            Assert.AreEqual(node.Status, Status.Pass);
            Assert.AreEqual(test.Status, Status.Pass);
        }
    }
}
