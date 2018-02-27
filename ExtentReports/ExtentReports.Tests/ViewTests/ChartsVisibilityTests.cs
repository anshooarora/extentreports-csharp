using System;

using NUnit.Framework;
using AventStack.ExtentReports.Reporter;

namespace AventStack.ExtentReports.Tests.ViewTests
{
    // todo complete parsing of file
    [TestFixture]
    public class ChartsVisibilityTests
    {
        private static string EXT = ".html";

        [Test]
        public void testAndLogsExpectsParentAndGrandChildCharts()
        {
            var fileName = TestContext.CurrentContext.Test.Name + EXT;

            var htmlReporter = new ExtentHtmlReporter(fileName);
            var extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
            extent.CreateTest(TestContext.CurrentContext.Test.Name).Pass("Pass");
            extent.Flush();


        }

        [Test]
        public void classAndTestAndLogsExpectsAllCharts()
        {
            var fileName = TestContext.CurrentContext.Test.Name + EXT;

            ExtentHtmlReporter htmlReporter = new ExtentHtmlReporter(fileName);
            ExtentReports extent = new ExtentReports();
            extent.AttachReporter(htmlReporter);
            extent.CreateTest(TestContext.CurrentContext.Test.Name).CreateNode("Child").Pass("Pass");
            extent.Flush();

        }
    }
}
