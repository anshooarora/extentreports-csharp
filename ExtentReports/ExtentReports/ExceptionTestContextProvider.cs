using System.Collections.Generic;
using System.Linq;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public class ExceptionTestContextProvider
    {
        public List<ExceptionTestContext> ExceptionTestContextCollection { get; private set; }

        public ExceptionTestContextProvider()
        {
            ExceptionTestContextCollection = new List<ExceptionTestContext>();
        }

        public void AddExceptionInfoContext(ExceptionInfo exceptionInfo, Test test)
        {
            var context = ExceptionTestContextCollection.Where(x => x.ExceptionName.Equals(exceptionInfo.Name));
            
            if (context.Count() > 0)
            {
                context.First().AddTest(test);
            }
            else
            {
                var exceptionContext = new ExceptionTestContext(exceptionInfo);
                exceptionContext.AddTest(test);
                ExceptionTestContextCollection.Add(exceptionContext);
            }
        }
    }
}
