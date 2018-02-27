using System;

namespace AventStack.ExtentReports.Model
{
    public class ExceptionInfo
    {
        public Exception Exception { get; private set; }
        public string StackTrace { get; private set; }
        public string Name { get; private set; }

        private const string _lhs = "<textarea class='code-block'>";
        private const string _rhs = "</textarea>";

        public ExceptionInfo(Exception ex)
        {
            Exception = ex;

            var msg = ex.StackTrace == null ? ex.Message : ex.StackTrace;
            StackTrace = _lhs + msg + _rhs;
            Name = ex.GetType().FullName;
        }
    }
}
