using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Reporter;
using AventStack.ExtentReports.Reporter.Configuration;

namespace AventStack.ExtentReports
{
    class Program
    {
        static void Main(string[] args)
        {
            ExtentHtmlReporter r = new ExtentHtmlReporter("Extent.html");
            //r.LoadConfig("extent-config.xml");
            //r.Configuration().ChartLocation = ChartLocation.Top;
            
            var extent = new ExtentReports();
            extent.AttachReporter(r);
            extent.AddSystemInfo("os", "win10x64");

            Exception e;
            try
            {
                throw new InvalidOperationException("An InvalidOperationException message");
            }
            catch (Exception ex)
            {
                e = ex;
            }

            var extentTest = extent.CreateTest("ParentTest").AssignCategory("Extent");
            var node = extentTest.CreateNode("Node1").Pass("Pass");
            extentTest.CreateNode("Node2", "description").Fail(e);
            extentTest.CreateNode("Node3", "description").Error("Error");

            // bdd
            /*
            var extentTest = extent.CreateTest<Feature>("ParentTest").AssignCategory("Extent");
            var node = extentTest.CreateNode<Scenario>("Node1");
            node.CreateNode<Given>("Node2", "description").Pass("pass");
            node.CreateNode<When>("Node3", "description").Pass("pass");
            node.CreateNode<Then>("Node4").Fail(e);*/

            extent.Flush();
        }
    }
}
