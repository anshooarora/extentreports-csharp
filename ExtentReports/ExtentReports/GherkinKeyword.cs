using System;
using System.Linq;

using AventStack.ExtentReports.Gherkin.Model;

namespace AventStack.ExtentReports
{
    public class GherkinKeyword
    {
        private IGherkinFormatterModel _model;

        public GherkinKeyword(string keyword)
        {
            var type = typeof(IGherkinFormatterModel);

            try
            {
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
