using System;

namespace AventStack.ExtentReports
{
    public interface IExtentReporter : ITestListener, IReportAggregatesListener, IConfigurable
    {
        void Start();
        void Stop();
        void Flush();
        DateTime StartTime { get; set; }
    }
}
