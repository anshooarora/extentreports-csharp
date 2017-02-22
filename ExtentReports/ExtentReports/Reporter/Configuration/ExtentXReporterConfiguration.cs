using MongoDB.Bson;

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

        public string ServerURL
        {
            get
            {
                return _serverUrl;
            }
            set
            {
                _serverUrl = value;
                _userConfiguration.Add("serverUrl", value);
            }
        }

        public ObjectId ReportObjectId
        {
            get
            {
                return _reportObjectId;
            }
            set
            {
                _reportObjectId = value;
                _userConfiguration.Add("reportId", value.ToString());
            }
        }

        private string _projectName = "Default";
        private string _serverUrl = null;
        private ObjectId _reportObjectId;
    }
}
