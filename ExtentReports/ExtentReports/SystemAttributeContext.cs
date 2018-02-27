using AventStack.ExtentReports.Model;
using System.Collections.Generic;

namespace AventStack.ExtentReports
{
    public class SystemAttributeContext
    {
        public List<SystemAttribute> SystemAttributeCollection { get; private set; }

        public SystemAttributeContext()
        {
            SystemAttributeCollection = new List<SystemAttribute>();
        }

        public void Add(SystemAttribute sa)
        {
            SystemAttributeCollection.Add(sa);
        }

        public void clear()
        {
            SystemAttributeCollection.Clear();
        }
    }
}
