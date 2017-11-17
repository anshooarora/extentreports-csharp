using System;
using System.Linq;

using AventStack.ExtentReports.Gherkin.Model;
using AventStack.ExtentReports.Gherkin;
using System.Collections.Generic;

namespace AventStack.ExtentReports
{
    /// <summary>
    /// Allows <see cref="IGherkinFormatterModel"/> to be returned by using a name, from the below gherkin model classes:
    /// 
    /// <list type="bullet">
    /// <item><see cref="Feature"/></item>
    /// <item><see cref="Background"/></item>
    /// <item><see cref="Scenario"/></item>
    /// <item><see cref="Given"/></item>
    /// <item><see cref="When"/></item>
    /// <item><see cref="Then"/></item>
    /// <item><see cref="And"/></item>
    /// </list>
    /// </summary>
    public class GherkinKeyword
    {
        private IGherkinFormatterModel _model;

        public GherkinKeyword(string keyword)
        {
            var type = typeof(IGherkinFormatterModel);
            var language = GherkinDialectProvider.Language;
            var dialect = GherkinDialectProvider.Dialect;

            try
            {
                if (!language.ToLower().Equals(GherkinDialectProvider.DefaultLanguage))
                {
                    keyword = dialect.Match(keyword);
                }

                var gherkinType = AppDomain.CurrentDomain.GetAssemblies()
                    .SelectMany(s => s.GetTypes())
                    .Where(p => p.Name.Equals(keyword, StringComparison.CurrentCultureIgnoreCase))
                    .First();

                var obj = Activator.CreateInstance(gherkinType);
                _model = (IGherkinFormatterModel)obj;
            }
            catch (InvalidOperationException e)
            {
                throw new InvalidOperationException("Invalid keyword specified: " + keyword, e);
            }
        }

        internal IGherkinFormatterModel GetModel()
        {
            return _model;
        }
    }
}
