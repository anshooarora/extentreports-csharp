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
                UserConfiguration.Add("projectName", value);
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
                UserConfiguration.Add("serverUrl", value);
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
                UserConfiguration.Add("reportId", value.ToString());
            }
        }

        private string _projectName = "Default";
        private string _serverUrl = null;
        private ObjectId _reportObjectId;
    }
}
