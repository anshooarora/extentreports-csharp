using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class NodeWithoutLogs : Base
    {
        [Test]
        public void VerifyNodeAndParentHasPassStatusIfNoLogsAdded()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);
            ExtentTest node = test.CreateNode("Child");

            Assert.AreEqual(node.GetModel().Level, 1);
            Assert.AreEqual(test.GetModel().LogContext().Count, 0);
            Assert.AreEqual(test.Status, Status.Pass);
            Assert.AreEqual(node.GetModel().LogContext().Count, 0);
            Assert.AreEqual(node.Status, Status.Pass);
        }
    }
}
