using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AventStack.ExtentReports.MediaStorageNS
{
    public class MediaStorageManagerFactory
    {
        public MediaStorage GetManager(string manager)
        {
            switch (manager.Trim().ToLower())
            {
                case "http":
                    return new HttpMediaManager();
                default:
                    return null;
            }
        }
    }
}
