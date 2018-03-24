using System;

using AventStack.ExtentReports.MarkupUtils;

using MongoDB.Bson;

namespace AventStack.ExtentReports.Model
{
    [Serializable]
    public class Log : IAddsMedia<Log>, IRunResult
    {
        public DateTime Timestamp { get; set; }
        public Status Status { get; set; }
        public ObjectId ObjectId { get; set; }
        public int Sequence = 0;
        public IMarkup Markup;

        private ExtentTest _parent;
        private ScreenCapture _screenCapture;
        private string _details;

        private Log()
        {
            Timestamp = DateTime.Now;
        }

        public Log(Test test) : this()
        {
            ParentModel = test;
        }

        public Log(ExtentTest extentTest) : this()
        {
            _parent = extentTest;
            ParentModel = _parent.GetModel();
        }

        public string Details
        {
            get
            {
                if (_screenCapture != null)
                    return _details + _screenCapture.Source;

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
                _screenCapture.TestObjectId = ParentModel.ObjectId;
            }
        }

        public Boolean HasScreenCapture()
        {
            return _screenCapture != null;
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
                ParentModel = value.GetModel();
            }
        }
    }
}