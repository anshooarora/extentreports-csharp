using System.Collections.Generic;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports.Reporter
{
    public abstract class BasicFileReporter : AbstractReporter
    {
        protected string _filePath;
        protected List<Status> _logStatusCollection;

        public override void OnTestStarted(Test test) { }

        public override void OnNodeStarted(Test node) { }

        public override void OnLogAdded(Test test, Log log)
        {
            if (_logStatusCollection == null)
                _logStatusCollection = new List<Status>();

            var status = log.Status == Status.Info ? Status.Pass : log.Status;

            if (!_logStatusCollection.Contains(status))
                _logStatusCollection.Add(status);
        }

        public override void OnAuthorAssigned(Test test, Author author) { }

        public override void OnCategoryAssigned(Test test, Category category) { }

        public override void OnScreenCaptureAdded(Test test, ScreenCapture screenCapture) { }
    }
}
