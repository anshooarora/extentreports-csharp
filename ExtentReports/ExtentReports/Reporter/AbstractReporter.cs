using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports.Reporter
{
    [ComVisible(true)]
    public abstract class AbstractReporter : IExtentReporter
    {
        public AbstractReporter()
        {
            StartTime = DateTime.Now;
        }

        public virtual TestAttributeTestContextProvider<Category> CategoryContext { get; set; }
        public virtual ExceptionTestContextProvider ExceptionContext { get; set; }
        public virtual SessionStatusStats SessionStatusStats { get; set; }
        public virtual DateTime StartTime { get; set; }
        public virtual SystemAttributeContext SystemAttributeContext { get; set; }
        public virtual List<Test> TestList { get; set; }
        public virtual List<string> TestRunnerLogs { get; set; }

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
