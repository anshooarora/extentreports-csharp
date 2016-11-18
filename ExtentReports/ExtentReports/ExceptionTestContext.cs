using System.Collections.Generic;
using System.Linq;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public class ExceptionTestContext
    {
        public List<Test> TestCollection { get; private set; }
        public string ExceptionName { get; private set; }

        private ExceptionInfo _exceptionInfo;

        public ExceptionTestContext(ExceptionInfo exceptionInfo)
        {
            if (TestCollection == null)
                TestCollection = new List<Test>();

            _exceptionInfo = exceptionInfo;
            ExceptionName = _exceptionInfo.Name;
        }

        public void AddTest(Test test)
        {
            // prevent adding duplicate tests
            if (TestCollection.Where(x => x.TestId == test.TestId).Count() > 0)
                return;

            TestCollection.Add(test);
        }
    }
}
