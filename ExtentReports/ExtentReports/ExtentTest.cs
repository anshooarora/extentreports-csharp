using System;
using System.Linq;

using AventStack.ExtentReports.Model;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.MarkupUtils;

namespace AventStack.ExtentReports
{
    [Serializable]
    public class ExtentTest : IAddsMedia<ExtentTest>, IRunResult
    {
        public Status Status
        {
            get
            {
                return _test.Status;
            }
        }

        private ExtentReports _extent;
        private Test _test;

        internal ExtentTest(ExtentReports extent, IGherkinFormatterModel bddType, string testName, string description = null)
        {
            if (string.IsNullOrEmpty(testName))
                throw new ArgumentException("TestName cannot be null or empty");

            _extent = extent;

            _test = new Test();
            _test.Name = testName.Trim();
            _test.Description = description;

            if (bddType != null)
                _test.BehaviorDrivenType = bddType;
        }

        internal ExtentTest(ExtentReports extent, string testName, string description = null)
            : this(extent, null, testName, description)
        { }

        public ExtentTest CreateNode(string name, string description = null)
        {
            var node = new ExtentTest(_extent, name, description);
            ApplyCommonNodeSettings(node);
            AddNodeToReport(node);
            return node;
        }

        public ExtentTest CreateNode<T>(string name, string description = null) where T : IGherkinFormatterModel
        {
            Type type = typeof(T);
            var obj = (IGherkinFormatterModel)Activator.CreateInstance(type);

            var node = CreateNode(name, description);
            node.GetModel().BehaviorDrivenType = obj;
            AddNodeToReport(node);

            return node;
        }

        public ExtentTest CreateNode(GherkinKeyword gherkinKeyword, string name, string description = null)
        {
            var node = CreateNode(name, description);
            node.GetModel().BehaviorDrivenType = gherkinKeyword.GetModel();
            return node;
        }

        private void ApplyCommonNodeSettings(ExtentTest node)
        {
            node.GetModel().Parent = _test;
            node.GetModel().Level = node.GetModel().Parent.Level + 1;
            _test.NodeContext().Add(node.GetModel());
        }

        private void AddNodeToReport(ExtentTest extentNode)
        {
            _extent.AddNode(extentNode.GetModel());
        }

        public ExtentTest Log(Status status, string details, MediaEntityModelProvider provider = null)
        {
            Log evt = CreateLog(status, details);

            if (provider != null)
            {
                evt.ScreenCapture = (ScreenCapture)provider.Media;
            }

            return AddLog(evt);
        }

        public ExtentTest Log(Status status, Exception ex, MediaEntityModelProvider provider = null)
        {
            ExceptionInfo exInfo = new ExceptionInfo(ex);
            GetModel().ExceptionInfo = exInfo;
            
            return Log(status, exInfo.StackTrace, provider);
        }

        public ExtentTest Log(Status status, IMarkup markup)
        {
            string details = markup.GetMarkup();
            return Log(status, details);
        }

        private ExtentTest AddLog(Log evt)
        {
            _test.LogContext().Add(evt);
            _test.End();

            _extent.AddLog(_test, evt);

            return this;
        }

        private Log CreateLog(Status status, string details = null)
        {
            details = details == null ? "" : details.Trim();

            Log evt = new Log(this);
            evt.Status = status;
            evt.Details = details;
            evt.Sequence = _test.LogContext().GetAllItems().Count + 1;

            return evt;
        }

        public ExtentTest Info(string details, MediaEntityModelProvider provider = null)
        {
            Log(Status.Info, details, provider);
            return this;
        }

        public ExtentTest Info(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Info, ex, provider);
        }

        public ExtentTest Info(IMarkup m)
        {
            return Log(Status.Info, m);
        }

        public ExtentTest Pass(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Pass, details, provider);
        }

        public ExtentTest Pass(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Pass, ex, provider);
        }

        public ExtentTest Pass(IMarkup m)
        {
            return Log(Status.Pass, m);
        }

        public ExtentTest Fail(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fail, details, provider);
        }

        public ExtentTest Fail(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fail, ex, provider);
        }

        public ExtentTest Fail(IMarkup m)
        {
            return Log(Status.Fail, m);
        }

        public ExtentTest Fatal(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fatal, details, provider);
        }

        public ExtentTest Fatal(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fatal, ex, provider);
        }

        public ExtentTest Fatal(IMarkup m)
        {
            return Log(Status.Fatal, m);
        }

        public ExtentTest Warning(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Warning, details, provider);
        }

        public ExtentTest Warning(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Warning, ex, provider);
        }

        public ExtentTest Warning(IMarkup m)
        {
            return Log(Status.Warning, m);
        }

        public ExtentTest Error(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Error, details, provider);
        }

        public ExtentTest Error(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Error, ex, provider);
        }

        public ExtentTest Error(IMarkup m)
        {
            return Log(Status.Error, m);
        }

        public ExtentTest Skip(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Skip, details, provider);
        }

        public ExtentTest Skip(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Skip, ex, provider);
        }

        public ExtentTest Skip(IMarkup m)
        {
            return Log(Status.Skip, m);
        }

        public ExtentTest debug(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Debug, details, provider);
        }

        public ExtentTest debug(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Debug, ex, provider);
        }

        public ExtentTest debug(IMarkup m)
        {
            return Log(Status.Debug, m);
        }

        public ExtentTest AssignCategory(params string[] category)
        {
            if (category == null || category.Length == 0)
                return this;

            category.ToList().Where(c => !string.IsNullOrEmpty(c)).ToList().ForEach(c =>
            {
                var objCategory = new Category(c.Replace(" ", ""));
                _test.CategoryContext().Add(objCategory);

                _extent.AssignCategory(_test, objCategory);
            });

            return this;
        }

        public ExtentTest AssignAuthor(params string[] author)
        {
            if (author == null || author.Length == 0)
                return this;

            author.ToList().Where(c => !string.IsNullOrEmpty(c)).ToList().ForEach(a =>
            {
                var objAuthor = new Author(a.Replace(" ", ""));
                _test.AuthorContext().Add(objAuthor);

                _extent.AssignAuthor(_test, objAuthor);
            });

            return this;
        }

        public ExtentTest AddScreenCaptureFromPath(string path, string title = null)
        {
            ScreenCapture sc = new ScreenCapture();
            sc.Path = path;
            sc.Title = title;

            _test.ScreenCaptureContext().Add(sc);

            if (_test.ObjectId != null)
            {
                int seq = _test.ScreenCaptureContext().Count;
                sc.Sequence = seq;
                sc.TestObjectId = _test.ObjectId;
            }

            _extent.AddScreenCapture(_test, sc);

            return this;
        }

        public Test GetModel()
        {
            return _test;
        }

    }
}
