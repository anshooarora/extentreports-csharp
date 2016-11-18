using System;
using System.Linq;
using System.Runtime.CompilerServices;

using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public class ExtentReports : Report
    {
        public void AttachReporter(params IExtentReporter[] reporter)
        {
            reporter.ToList().ForEach(x => Attach(x));
        }

        public void DetachReporter(params IExtentReporter[] reporter)
        {
            reporter.ToList().ForEach(x => Detach(x));
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ExtentTest CreateTest<T>(string name, string description = null) where T : IGherkinFormatterModel
        {
            Type type = typeof(T);
            var obj = (IGherkinFormatterModel)Activator.CreateInstance(type);

            var extentTest = new ExtentTest(this, obj, name, description);
            CreateTest(extentTest.GetModel());

            return extentTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ExtentTest CreateTest(string name, string description = null)
        {
            var extentTest = new ExtentTest(this, name, description);
            CreateTest(extentTest.GetModel());

            return extentTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public ExtentTest CreateTest(GherkinKeyword gherkinKeyword, string name, string description = null)
        {
            var extentTest = CreateTest(name, description);
            extentTest.GetModel().BehaviorDrivenType = gherkinKeyword.GetModel();
            return extentTest;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public new void Flush()
        {
            base.Flush();
        }

        public void AddSystemInfo(string name, string value)
        {
            SystemAttribute sa = new SystemAttribute(name, value);
            AddSystemAttribute(sa);
        }

        public void AddTestRunnerLogs(string log)
        {
            AddTestRunnerLog(log);
        }

        public void AddTestRunnerLogs(string[] log)
        {
            log.ToList().ForEach(x => AddTestRunnerLog(x));
        }
    }
}
