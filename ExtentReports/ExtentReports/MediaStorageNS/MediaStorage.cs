using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports.MediaStorageNS
{
    public interface MediaStorage
    {
        void Init(string host);
        void StoreMedia(Media m);
    }
}
