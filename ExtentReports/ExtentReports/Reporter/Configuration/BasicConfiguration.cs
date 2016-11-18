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
                _userConfiguration.Add("reportName", _reportName);
            }
        }

        protected Dictionary<string, string> _userConfiguration;

        private string _reportName;

        public BasicConfiguration()
        {
            _userConfiguration = new Dictionary<string, string>();
        }

        public Dictionary<string, string> GetConfiguration()
        {
            return _userConfiguration;
        }
    }
}
