using System;
using System.Collections.Generic;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports.Reporter
{
    public abstract class AbstractReporter : IExtentReporter
    {
        public abstract TestAttributeTestContextProvider<Category> CategoryContext { get; set; }
        public abstract ExceptionTestContextProvider ExceptionContext { get; set; }
        public abstract SessionStatusStats SessionStatusStats { get; set; }
        public abstract DateTime StartTime { get; set; }
        public abstract SystemAttributeContext SystemAttributeContext { get; set; }
        public abstract List<Test> TestList { get; set; }
        public abstract List<string> TestRunnerLogs { get; set; }

        public abstract void Flush();
        public abstract void LoadConfig(string filePath);
        public abstract void OnAuthorAssigned(Test test, Author author);
        public abstract void OnCategoryAssigned(Test test, Category category);
        public abstract void OnLogAdded(Test test, Log log);
        public abstract void OnNodeStarted(Test node);
        public abstract void OnScreenCaptureAdded(Test test, ScreenCapture screenCapture);
        public abstract void OnTestStarted(Test test);
        public abstract void Start();
        public abstract void Stop();
    }
}
