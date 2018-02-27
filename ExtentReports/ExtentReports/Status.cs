using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports
{
    /// <summary>
    /// List of allowed status for <see cref="Log"/>
    /// </summary>
    public enum Status
    {
        Pass,
        Fail,
        Fatal,
        Error,
        Warning,
        Info,
        Skip,
        Debug
    }
}
