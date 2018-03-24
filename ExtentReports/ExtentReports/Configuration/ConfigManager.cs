using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.Configuration
{
    internal class ConfigManager
    {
        private List<Config> _configList;
        
        public ConfigManager()
        {
            _configList = new List<Config>();
        }

        public List<Config> Configuration { get { return _configList; } }

        public string GetValue(string k)
        {
            var c = _configList.Where(x => x.Key.Equals(k));

            if (c.Count() > 0)
                return c.First().Value;

            return null;
        }

        public void AddConfig(Config c)
        {
            if (ContainsConfig(c.Key))
                RemoveConfig(c.Key);

            _configList.Add(c);
        }

        private bool ContainsConfig(string k)
        {
            return _configList.Where(x => x.Key.Equals(k)).Count() == 1;
        }

        private void RemoveConfig(string k)
        {
            var c = _configList.Where(x => x.Key.Equals(k)).First();
            _configList.Remove(c);
        }
    }
}
