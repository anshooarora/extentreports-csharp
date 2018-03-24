using System;
using System.Linq;

using AventStack.ExtentReports.Model;
using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.MarkupUtils;

namespace AventStack.ExtentReports
{
    /// <summary>
    /// Defines a test. You can add logs, snapshots, assign author and categories to a test and its children.
    /// </summary>
    [Serializable]
    public class ExtentTest : IAddsMedia<ExtentTest>, IRunResult
    {
        /// <summary>
        /// Provides the current run status of the test or node
        /// </summary>
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

        /// <summary>
        /// Creates a node with description
        /// </summary>
        /// <param name="name">Node name</param>
        /// <param name="description">A short description</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest CreateNode(string name, string description = null)
        {
            var node = new ExtentTest(_extent, name, description);
            ApplyCommonNodeSettings(node);
            AddNodeToReport(node);
            return node;
        }

        /// <summary>
        /// Creates a BDD-style node with description representing one of the <see cref="IGherkinFormatterModel"/> 
        /// </summary>
        /// <typeparam name="T">A <see cref="IGherkinFormatterModel"/> type</typeparam>
        /// <param name="name">Node name</param>
        /// <param name="description">A short description</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest CreateNode<T>(string name, string description = null) where T : IGherkinFormatterModel
        {
            Type type = typeof(T);
            var obj = (IGherkinFormatterModel)Activator.CreateInstance(type);

            var node = createNode(name, description);
            node.GetModel().BehaviorDrivenType = obj;
            AddNodeToReport(node);

            return node;
        }

        private ExtentTest createNode(string name, string description = null)
        {
            var node = new ExtentTest(_extent, name, description);
            ApplyCommonNodeSettings(node);
            return node;
        }

        /// <summary>
        /// Creates a BDD-style node with description using name of the Gherkin model such as:
        /// 
        /// <list type="bullet">
        /// <item><see cref="Feature"/></item>
        /// <item><see cref="Background"/></item>
        /// <item><see cref="Scenario"/></item>
        /// <item><see cref="Given"/></item>
        /// <item><see cref="When"/></item>
        /// <item><see cref="Then"/></item>
        /// <item><see cref="And"/></item>
        /// </list>
        /// 
        /// <code>
        /// test.CreateNode(new GherkinKeyword("Feature"), "Name", "Description");
        /// </code>
        /// </summary>
        /// <param name="gherkinKeyword">Name of the <see cref="GherkinKeyword"/></param>
        /// <param name="name">Node name</param>
        /// <param name="description">A short description</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest CreateNode(GherkinKeyword gherkinKeyword, string name, string description = null)
        {
            var node = createNode(name, description);
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

        /// <summary>
        /// Logs an event with <see cref="Status"/>, details and a media object: <see cref="ScreenCapture"/>
        /// 
        /// <code>
        /// test.Log(Status.Fail, "details", MediaEntityBuilder.CreateScreenCaptureFromPath("screen.png").Build());
        /// </code>
        /// </summary>
        /// <param name="status"><see cref="Status"/></param>
        /// <param name="details">Log details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Log(Status status, string details, MediaEntityModelProvider provider = null)
        {
            Log evt = CreateLog(status, details);

            if (provider != null)
            {
                evt.ScreenCapture = (ScreenCapture)provider.Media;
            }

            return AddLog(evt);
        }

        /// <summary>
        /// Logs an event with <see cref="Status"/>, an exception and a media object: <see cref="ScreenCapture"/>
        /// 
        /// <code>
        /// test.Log(Status.Fail, exception, MediaEntityBuilder.CreateScreenCaptureFromPath("screen.png").Build());
        /// </code>
        /// </summary>
        /// <param name="status"><see cref="Status"/></param>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Log(Status status, Exception ex, MediaEntityModelProvider provider = null)
        {
            ExceptionInfo exInfo = new ExceptionInfo(ex);
            GetModel().ExceptionInfo = exInfo;
            
            return Log(status, exInfo.StackTrace, provider);
        }

        /// <summary>
        /// Logs an event with <see cref="Status"/> and custom <see cref="IMarkup"/> such as:
        /// 
        /// <list type="bullet">
        /// <item>CodeBlock</item>
        /// <item>Label</item>
        /// <item>Table</item>
        /// </list>
        /// </summary>
        /// <param name="status"></param>
        /// <param name="markup"></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
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

        /// <summary>
        /// Logs an <see cref="Status.Info"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Info(string details, MediaEntityModelProvider provider = null)
        {
            Log(Status.Info, details, provider);
            return this;
        }

        /// <summary>
        /// Logs an <see cref="Status.Info"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Info(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Info, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Info"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Info(IMarkup m)
        {
            return Log(Status.Info, m);
        }

        /// <summary>
        /// Logs an <see cref="Status.Pass"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Pass(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Pass, details, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Pass"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Pass(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Pass, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Pass"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Pass(IMarkup m)
        {
            return Log(Status.Pass, m);
        }

        /// <summary>
        /// Logs an <see cref="Status.Fail"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Fail(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fail, details, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Fail"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Fail(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fail, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Fail"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Fail(IMarkup m)
        {
            return Log(Status.Fail, m);
        }

        /// <summary>
        /// Logs an <see cref="Status.Fatal"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Fatal(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fatal, details, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Fatal"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Fatal(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Fatal, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Fatal"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Fatal(IMarkup m)
        {
            return Log(Status.Fatal, m);
        }

        /// <summary>
        /// Logs an <see cref="Status.Warning"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Warning(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Warning, details, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Warning"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Warning(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Warning, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Warning"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Warning(IMarkup m)
        {
            return Log(Status.Warning, m);
        }

        /// <summary>
        /// Logs an <see cref="Status.Error"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Error(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Error, details, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Error"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Error(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Error, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Error"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Error(IMarkup m)
        {
            return Log(Status.Error, m);
        }

        /// <summary>
        /// Logs an <see cref="Status.Skip"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Skip(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Skip, details, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Skip"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Skip(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Skip, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Skip"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Skip(IMarkup m)
        {
            return Log(Status.Skip, m);
        }

        /// <summary>
        /// Logs an <see cref="Status.Debug"/> event with details and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="details">Details</param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Debug(string details, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Debug, details, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Debug"/> event with an exception and a media object <see cref="ScreenCapture"/>
        /// </summary>
        /// <param name="ex"><see cref="Exception"/></param>
        /// <param name="provider">A <see cref="MediaEntityModelProvider"/> object</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Debug(Exception ex, MediaEntityModelProvider provider = null)
        {
            return Log(Status.Debug, ex, provider);
        }

        /// <summary>
        /// Logs an <see cref="Status.Debug"/> event with <see cref="IMarkup"/>
        /// </summary>
        /// <param name="m"><see cref="IMarkup"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest Debug(IMarkup m)
        {
            return Log(Status.Debug, m);
        }

        /// <summary>
        /// Assigns a category or group
        /// </summary>
        /// <param name="category"><see cref="Category"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
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

        /// <summary>
        /// Assigns an author
        /// </summary>
        /// <param name="author"><see cref="Author"/></param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
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

        /// <summary>
        /// Adds a snapshot to the test or log with title
        /// </summary>
        /// <param name="path">Image path</param>
        /// <param name="title">Image title</param>
        /// <returns>A <see cref="ExtentTest"/> object</returns>
        public ExtentTest AddScreenCaptureFromPath(string path, string title = null)
        {
            ScreenCapture sc = new ScreenCapture();
            sc.Path = path;
            sc.Title = title;

            _test.ScreenCaptureContext().Add(sc);

            if (_test.ObjectId != null)
            {
                int seq = _test.ScreenCaptureContext().Count;
                sc.TestObjectId = _test.ObjectId;
            }

            _extent.AddScreenCapture(_test, sc);

            return this;
        }

        /// <summary>
        /// Returns the underlying test which controls the internal model
        /// </summary>
        /// <returns><see cref="Test"/></returns>
        public Test GetModel()
        {
            return _test;
        }

    }
}
