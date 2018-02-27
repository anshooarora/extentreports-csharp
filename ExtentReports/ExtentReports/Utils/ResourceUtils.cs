using System.IO;

namespace AventStack.ExtentReports.Utils
{
    internal class ResourceUtils
    {
        public string GetResource(string folder, string fileName)
        {
            string result = string.Empty;

            using (Stream stream = GetType().Assembly.GetManifestResourceStream(folder + "." + fileName))
            {
                using (StreamReader sr = new StreamReader(stream))
                {
                    result = sr.ReadToEnd();
                }
            }

            return result;
        }
    }
}
