using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class TestSingleLogsStatusTests : Base
    {
        [Test]
        public void verifyIfTestHasStatusPass()
        {
            Console.WriteLine("in " + TestContext.CurrentContext.Test.Name);
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).Pass("pass");

            Assert.AreEqual(test.GetModel().LogContext().Count, 1);
            Assert.AreEqual(test.Status, Status.Pass);
            Console.WriteLine("out " + TestContext.CurrentContext.Test.Name);
        }

        [Test]
        public void verifyIfTestHasStatusSkip()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).Skip("skip");

            Assert.AreEqual(test.GetModel().LogContext().Count, 1);
            Assert.AreEqual(test.Status, Status.Skip);
        }

        [Test]
        public void verifyIfTestHasStatusWarning()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).Warning("warning");

            Assert.AreEqual(test.GetModel().LogContext().Count, 1);
            Assert.AreEqual(test.Status, Status.Warning);
        }

        [Test]
        public void verifyIfTestHasStatusError()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).Error("error");

            Assert.AreEqual(test.GetModel().LogContext().Count, 1);
            Assert.AreEqual(test.Status, Status.Error);
        }

        [Test]
        public void verifyIfTestHasStatusFail()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).Fail("fail");

            Assert.AreEqual(test.GetModel().LogContext().Count, 1);
            Assert.AreEqual(test.Status, Status.Fail);
        }

        [Test]
        public void verifyIfTestHasStatusFatal()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).Fatal("fatal");

            Assert.AreEqual(test.GetModel().LogContext().Count, 1);
            Assert.AreEqual(test.Status, Status.Fatal);
        }

        [Test]
        public void verifyIfTestHasStatusPassWithOnlyInfoSingle()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name).Info("info");

            Assert.AreEqual(test.GetModel().LogContext().Count, 1);
            Assert.AreEqual(test.Status, Status.Pass);
        }
    }
}
