using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Gherkin
{
    internal class GherkinKeywords
    {
        public List<string> and { get; set; }
        public List<string> background { get; set; }
        public List<string> but { get; set; }
        public List<string> examples { get; set; }
        public List<string> feature { get; set; }
        public List<string> given { get; set; }
        public string name { get; set; }
        public string native { get; set; }
        public List<string> scenario { get; set; }
        public List<string> scenarioOutline { get; set; }
        public List<string> then { get; set; }
        public List<string> when { get; set; }
    }
}
