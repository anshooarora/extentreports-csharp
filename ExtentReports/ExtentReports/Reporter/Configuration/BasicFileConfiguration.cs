using System;

namespace AventStack.ExtentReports.Reporter.Configuration
{
    public abstract class BasicFileConfiguration : BasicConfiguration
    {
        public string FilePath
        {
            get
            {
                return _filePath;
            }
            set
            {
                _filePath = value;
                UserConfiguration["filePath"] = _filePath;
            }
        }

        public Theme Theme
        {
            get
            {
                return _theme;
            }
            set
            {
                _theme = value;
                UserConfiguration["theme"] = Enum.GetName(typeof(Theme), _theme).ToLower();
            }
        }

        public string Encoding
        {
            get
            {
                return _encoding;
            }
            set
            {
                _encoding = value;
                UserConfiguration["encoding"] = _encoding;
            }
        }

        public string DocumentTitle
        {
            get
            {
                return _documentTitle;
            }
            set
            {
                _documentTitle = value;
                UserConfiguration["documentTitle"] = _documentTitle;
            }
        }

        public string CSS
        {
            get
            {
                return _css;
            }
            set
            {
                _css = value;
                UserConfiguration["css"] = _css;
            }
        }

        public string JS
        {
            get
            {
                return _js;
            }
            set
            {
                _js = value;
                UserConfiguration["js"] = _js;
            }
        }

        private string _filePath;
        private Theme _theme;
        private string _encoding;
        private string _documentTitle;
        private string _css;
        private string _js;
    }
}
