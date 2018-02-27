using System.Collections.Generic;

namespace AventStack.ExtentReports.Reporter.Configuration
{
    public abstract class BasicConfiguration
    {
        public string ReportName
        {
            get
            {
                return _reportName;
            }
            set
            {
                _reportName = value;
                UserConfiguration.Add("reportName", _reportName);
            }
        }

        internal Dictionary<string, string> UserConfiguration;

        private string _reportName;

        public BasicConfiguration()
        {
            UserConfiguration = new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetConfiguration()
        {
            return UserConfiguration;
        }
    }
}
