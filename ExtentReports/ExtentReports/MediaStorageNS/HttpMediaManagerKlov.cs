using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;

using AventStack.ExtentReports.Model;

namespace AventStack.ExtentReports.MediaStorageNS
{
    public class HttpMediaManagerKlov : MediaStorage
    {
        private const string _route = "files/upload";

        private string _host;

        public void Init(string host)
        {
            if (!host.Substring(host.Length - 1).Equals("/"))
                host += "/";

            this._host = host;
        }

        public void StoreMedia(Media m)
        {
            if (m.Path == null || m.Base64String != null)
                return;

            if (!File.Exists(m.Path))
                throw new IOException("The system cannot find the file specified " + m.Path);

            var uri = new Uri(_host + _route);
            var file = File.ReadAllBytes(m.Path);
            var fileName = m.Sequence + Path.GetExtension(m.Path);

            using (var reader = new StreamReader(m.Path))
            {
                using (var handler = new HttpClientHandler() { UseCookies = false })
                using (var client = new HttpClient(handler))
                {
                    client.BaseAddress = uri;

                    client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
                    client.DefaultRequestHeaders.Add("Accept", "application/json");
                    client.DefaultRequestHeaders.Add("Connection", "keep-alive");
                    client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0");
                    //client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", _csrf);
                    //client.DefaultRequestHeaders.Add("Cookie", _cookie);

                    using (var content = new MultipartFormDataContent(DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        string logId = m.LogObjectId == null ? "" : m.LogObjectId.ToString();

                        var values = new[]
                        {
                            new KeyValuePair<string, string>("name", m.Sequence + Path.GetExtension(m.Path)),
                            new KeyValuePair<string, string>("id", m.ObjectId.ToString()),
                            new KeyValuePair<string, string>("reportId", m.ReportObjectId.ToString()),
                            new KeyValuePair<string, string>("testId", m.TestObjectId.ToString()),
                            new KeyValuePair<string, string>("mediaType", m.MediaType.ToString().ToLower()),
                            new KeyValuePair<string, string>("logId", logId)
                        };

                        foreach (var keyValuePair in values)
                        {
                            content.Add(new StringContent(keyValuePair.Value), String.Format("\"{0}\"", keyValuePair.Key));
                        }

                        var imageContent = new ByteArrayContent(file);
                        content.Add(imageContent,
                            '"' + "f" + '"',
                            '"' + fileName + '"');

                        var result = client.PostAsync("/" + _route, content).Result;

                        if (result.StatusCode != HttpStatusCode.OK)
                        {
                            throw new IOException("Unable to upload file to server: " + m.Path);
                        }
                    }
                }
            }
        }
    }
}
