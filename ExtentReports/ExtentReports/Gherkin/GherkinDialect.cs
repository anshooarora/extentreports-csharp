using AventStack.ExtentReports.Utils;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Gherkin
{
    internal class GherkinDialect
    {
        public GherkinKeywords Keywords
        {
            get; private set;
        }

        public string Language
        {
            get; private set;
        }

        public GherkinDialect(string language, GherkinKeywords keywords)
        {
            Language = language;
            Keywords = keywords;
        }

        public string Match(string keyword)
        {
            if (Keywords.and.Contains(keyword, StringComparison.OrdinalIgnoreCase))
                return "And";

            if (Keywords.background.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "Background";

            if (Keywords.but.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "But";

            if (Keywords.examples.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "Examples";

            if (Keywords.feature.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "Feature";

            if (Keywords.given.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "Given";

            if (Keywords.scenario.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "Scenario";

            if (Keywords.scenarioOutline.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "ScenarioOutline";

            if (Keywords.then.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "Then";

            if (Keywords.when.Contains(keyword, StringComparer.OrdinalIgnoreCase))
                return "When";

            return null;
        }
    }
}
