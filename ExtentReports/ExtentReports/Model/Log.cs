using System;

using AventStack.ExtentReports.MarkupUtils;

namespace AventStack.ExtentReports.Model
{
    [Serializable]
    public class Log : IAddsMedia<Log>, IRunResult
    {
        public DateTime Timestamp { get; set; }
        public Status Status { get; set; }

        public int Sequence = 0;
        public IMarkup Markup;

        private Test _parentModel;
        private ExtentTest _parent;
        private ScreenCapture _screenCapture;
        private string _details;

        private Log()
        {
            Timestamp = DateTime.Now;
        }

        public Log(Test test) : this()
        {
            _parentModel = test;
        }

        public Log(ExtentTest extentTest) : this()
        {
            _parent = extentTest;
        }

        public string Details
        {
            get
            {
                if (_screenCapture != null)
                {
                    _details = _details + _screenCapture.Source;
                    _screenCapture = null;
                }
                return _details;
            }
            set
            {
                _details = value;
            }
        }

        public ScreenCapture ScreenCapture
        {
            get
            {
                return (ScreenCapture)_screenCapture;
            }
            set
            {
                _screenCapture = value;
            }
        }

        public Test ParentModel
        {
            get; private set;
        }

        public ExtentTest Parent
        {
            get
            {
                return _parent;
            }
            private set
            {
                _parent = value;
                _parentModel = value.GetModel();
            }
        }
    }
}