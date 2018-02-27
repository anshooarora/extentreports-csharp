using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Tests.APITests
{
    [TestFixture]
    public class TestHasUniqueIdsTests : Base
    {
        private int _times = 100;

        [Test]
        public void verifyAllStartedTestsHaveUniqueIds()
        {
            var idCollection = new List<int>();

            // create [times] tests to ensure test-id is not duplicate 
            for (int ix = 0; ix < _times; ix++)
            {
                int testId = _extent.CreateTest(TestContext.CurrentContext.Test.Name + "." + ix).Info("test # " + ix).GetModel().TestId;

                Assert.False(idCollection.Contains(testId));

                idCollection.Add(testId);
            }
        }
    }
}
