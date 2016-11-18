using System;

using AventStack.ExtentReports.MarkupUtils;

namespace AventStack.ExtentReports.Model
{
    [Serializable]
    public class Log : IAddsMedia<Log>, IRunResult
    {
        public DateTime Timestamp;
        public string Details;        
        public int Sequence = 0;

        public Test _parentModel;
        public ExtentTest _parent;
        public Status Status { get; set; }
        public IMarkup Markup;

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
