using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Gherkin
{
    internal static class GherkinDialectProvider
    {
        private static string _defaultLanguage = "en";

        private static GherkinDialect _currentDialect;
        private static Dictionary<string, GherkinKeywords> _dialects;
        private static GherkinKeywords _keywords;
        private static string _language;

        static GherkinDialectProvider()
        {
            string json = Resources.GherkinLanguages.Languages;
            _dialects = JsonConvert.DeserializeObject<Dictionary<string, GherkinKeywords>>(json);
        }
        
        public static string DefaultLanguage
        {
            get
            {
                return _defaultLanguage;
            }
        }

        public static GherkinDialect Dialect
        {
            get
            {
                return _currentDialect;
            }
        }

        public static string Language
        {
            set
            {
                _language = value;

                if (!_dialects.ContainsKey(_language))
                    throw new InvalidGherkinLanguageException(_language + " is not a valid Gherkin dialect");

                _keywords = _dialects[_language];
                _currentDialect = new GherkinDialect(_language, _keywords);
            }
            get
            {
                if (string.IsNullOrEmpty(_language))
                    Language = _defaultLanguage;

                return _language;
            }
        }
    }
}
