using System.Collections.Generic;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    public interface IReportAggregatesListener
    {
        List<Test> TestList { get; set; }
        TestAttributeTestContextProvider<Category> CategoryContext { get; set; }
        List<string> TestRunnerLogs { get; set; }
        SessionStatusStats SessionStatusStats { get; set; }
        SystemAttributeContext SystemAttributeContext { get; set; }
        ExceptionTestContextProvider ExceptionContext { get; set; }
    }
}
