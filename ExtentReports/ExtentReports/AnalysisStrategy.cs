namespace AventStack.ExtentReports
{
    /// <summary>
    /// Type of AnalysisStrategy for the reporter.Not all reporters support this setting.
    /// 
    /// There are 2 types of strategies available:
    /// 
    /// <list type="bullet">
    ///     <item>Class: Shows analysis in 3 different charts: Class, Test and Step (log)</item>
    ///     <item>Test: Shows analysis in 2 different charts: Test and Step(log)</item>
    /// </list>
    /// </summary>
    public enum AnalysisStrategy
    {
        Class,
        Test
    }
}
