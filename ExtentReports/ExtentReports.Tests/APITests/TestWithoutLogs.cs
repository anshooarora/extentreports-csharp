using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class TestWithoutLogs : Base
    {
        [Test]
        public void VerifyTestHasPassStatusIfNoLogsAdded()
        {
            ExtentTest test = _extent.CreateTest(TestContext.CurrentContext.Test.Name);

            Assert.AreEqual(test.GetModel().LogContext().Count, 0);
            Assert.AreEqual(test.Status, Status.Pass);
        }
    }
}
