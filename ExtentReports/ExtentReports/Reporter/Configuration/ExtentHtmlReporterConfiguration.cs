using System;

namespace AventStack.ExtentReports.Reporter.Configuration
{
    public class ExtentHtmlReporterConfiguration : BasicFileConfiguration, IReporterConfiguration
    {
        public Protocol Protocol
        {
            get
            {
                return _protocol;
            }
            set
            {
                _protocol = value;
                _userConfiguration.Add("protocol", Enum.GetName(typeof(Protocol), _protocol).ToLower());
            }
        }

        public ChartLocation ChartLocation
        {
            get
            {
                return _chartLocation;
            }
            set
            {
                _chartLocation = value;
                _userConfiguration.Add("chartLocation", Enum.GetName(typeof(ChartLocation), _chartLocation).ToLower());
            }
        }

        public bool ChartVisibilityOnOpen
        {
            get
            {
                return _chartVisibilityOnOpen;
            }
            set
            {
                _chartVisibilityOnOpen = value;
                _userConfiguration.Add("chartVisibilityOnOpen", _chartVisibilityOnOpen.ToString().ToLower());
            }
        }

        private Protocol _protocol = Protocol.HTTPS;
        private ChartLocation _chartLocation = ChartLocation.Top;
        private bool _chartVisibilityOnOpen = true;
    }
}
