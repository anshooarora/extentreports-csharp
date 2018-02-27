using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class NodesStatusHierarchyTests : Base
    {
        [Test]
        public void verifyPassHasHigherPriorityThanInfoLevelsShallow()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            child.Info("Info");
            child.Pass("Pass");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(parent.Status, Status.Pass);
            Assert.AreEqual(child.Status, Status.Pass);
        }

        [Test]
        public void verifyPassHasHigherPriorityThanInfoLevelsDeep()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            ExtentTest grandchild = child.CreateNode("GrandChild");
            grandchild.Info("Info");
            grandchild.Pass("Pass");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(grandchild.GetModel().Level, 2);
            Assert.AreEqual(parent.Status, Status.Pass);
            Assert.AreEqual(child.Status, Status.Pass);
            Assert.AreEqual(grandchild.Status, Status.Pass);
        }

        [Test]
        public void verifySkipHasHigherPriorityThanPassLevelsShallow()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            child.Pass("Pass");
            child.Skip("Skip");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(parent.Status, Status.Skip);
            Assert.AreEqual(child.Status, Status.Skip);
        }

        [Test]
        public void verifySkipHasHigherPriorityThanPassLevelsDeep()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            ExtentTest grandchild = child.CreateNode("GrandChild");
            grandchild.Pass("Pass");
            grandchild.Skip("Skip");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(grandchild.GetModel().Level, 2);
            Assert.AreEqual(parent.Status, Status.Skip);
            Assert.AreEqual(child.Status, Status.Skip);
            Assert.AreEqual(grandchild.Status, Status.Skip);
        }

        [Test]
        public void verifyWarningHasHigherPriorityThanSkipLevelsShallow()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            child.Skip("Skip");
            child.Warning("Warning");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(parent.Status, Status.Warning);
            Assert.AreEqual(child.Status, Status.Warning);
        }

        [Test]
        public void verifyWarningHasHigherPriorityThanSkipLevelsDeep()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            ExtentTest grandchild = child.CreateNode("GrandChild");
            grandchild.Skip("Skip");
            grandchild.Warning("Warning");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(grandchild.GetModel().Level, 2);
            Assert.AreEqual(parent.Status, Status.Warning);
            Assert.AreEqual(child.Status, Status.Warning);
            Assert.AreEqual(grandchild.Status, Status.Warning);
        }

        [Test]
        public void verifyErrorHasHigherPriorityThanWarningLevelsShallow()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            child.Warning("Warning");
            child.Error("Error");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(parent.Status, Status.Error);
            Assert.AreEqual(child.Status, Status.Error);
        }

        [Test]
        public void verifyErrorHasHigherPriorityThanWarningLevelsDeep()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            ExtentTest grandchild = child.CreateNode("GrandChild");
            grandchild.Warning("Warning");
            grandchild.Error("Error");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(grandchild.GetModel().Level, 2);
            Assert.AreEqual(parent.Status, Status.Error);
            Assert.AreEqual(child.Status, Status.Error);
            Assert.AreEqual(grandchild.Status, Status.Error);
        }

        [Test]
        public void verifFailHasHigherPriorityThanErrorLevelsShallow()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            child.Error("Error");
            child.Fail("Fail");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(parent.Status, Status.Fail);
            Assert.AreEqual(child.Status, Status.Fail);
        }

        [Test]
        public void verifFailHasHigherPriorityThanErrorLevelsDeep()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            ExtentTest grandchild = child.CreateNode("GrandChild");
            grandchild.Error("Error");
            grandchild.Fail("Fail");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(grandchild.GetModel().Level, 2);
            Assert.AreEqual(parent.Status, Status.Fail);
            Assert.AreEqual(child.Status, Status.Fail);
            Assert.AreEqual(grandchild.Status, Status.Fail);
        }

        [Test]
        public void verifFatalHasHigherPriorityThanFailLevelsShallow()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            child.Fail("Fail");
            child.Fatal("Fatal");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(parent.Status, Status.Fatal);
            Assert.AreEqual(child.Status, Status.Fatal);
        }

        [Test]
        public void verifFatalHasHigherPriorityThanFailLevelsDeep()
        {
            ExtentTest parent = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest child = parent.CreateNode("Child");
            ExtentTest grandchild = child.CreateNode("GrandChild");
            grandchild.Fail("Fail");
            grandchild.Fatal("Fatal");

            Assert.AreEqual(child.GetModel().Level, 1);
            Assert.AreEqual(grandchild.GetModel().Level, 2);
            Assert.AreEqual(parent.Status, Status.Fatal);
            Assert.AreEqual(child.Status, Status.Fatal);
            Assert.AreEqual(grandchild.Status, Status.Fatal);
        }
    }
}
