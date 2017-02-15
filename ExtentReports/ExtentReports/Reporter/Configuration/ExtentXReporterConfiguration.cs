namespace AventStack.ExtentReports.Reporter.Configuration
{
    public class ExtentXReporterConfiguration : BasicConfiguration, IReporterConfiguration
    {
        public string ProjectName
        {
            get
            {
                return _projectName;
            }
            set
            {
                _projectName = value;
                _userConfiguration.Add("projectName", value);
            }
        }

        private string _projectName = "Default";
    }
}
