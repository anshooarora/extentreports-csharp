using System;

namespace AventStack.ExtentReports.Model
{
    public class ExceptionInfo
    {
        public Exception Exception { get; private set; }
        public string StackTrace { get; private set; }
        public string Name { get; private set; }

        private const string _lhs = "<textarea>";
        private const string _rhs = "</textarea>";

        public ExceptionInfo(Exception ex)
        {
            Exception = ex;
            StackTrace = _lhs + ex.StackTrace + _rhs;
            Name = ex.GetType().FullName;
        }
    }
}
