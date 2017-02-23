using System;
using System.Collections.Generic;
using System.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net;
using System.IO;
using System.Globalization;

using AventStack.ExtentReports.Model;

using Newtonsoft.Json;

namespace AventStack.ExtentReports.MediaStorageNS
{
    public class HttpMediaManager : MediaStorage
    {
        private const string _route = "upload";
        private const string _csrfRoute = "csrfToken";

        private string _csrf;
        private string _host;
        private string _cookie;

        public void Init(string host)
        {
            _host = host;

            if (_host.LastIndexOf('/') != host.Length - 1)
                _host = _host + "/";

            StoreCsrfTokenAndCookie();
        }

        private void StoreCsrfTokenAndCookie()
        {
            var uri = new Uri(_host + _csrfRoute);

            var request = WebRequest.Create(uri);
            
            using (HttpWebResponse res = (HttpWebResponse) request.GetResponse())
            {
                if (res.StatusCode != HttpStatusCode.OK)
                {
                    // CRITICAL!
                    // unable to connect to the server or bad response
                    // throw exception
                    throw new WebException("Bad response from the server at " + uri);
                }

                string responseText;
                var encoding = ASCIIEncoding.ASCII;
                using (var reader = new System.IO.StreamReader(res.GetResponseStream(), encoding))
                {
                    responseText = reader.ReadToEnd();
                }

                dynamic result = JsonConvert.DeserializeObject(responseText);
                _csrf = result._csrf.Value;

                _cookie = res.Headers["Set-Cookie"];
            }
        }

        public void StoreMedia(Media m)
        {
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
                    client.DefaultRequestHeaders.Add("X-CSRF-TOKEN", _csrf);
                    client.DefaultRequestHeaders.Add("Cookie", _cookie);
                    
                    using (var content = new MultipartFormDataContent(DateTime.Now.ToString(CultureInfo.InvariantCulture)))
                    {
                        var values = new[]
                        {
                            new KeyValuePair<string, string>("name", m.Sequence + Path.GetExtension(m.Path)),
                            new KeyValuePair<string, string>("id", m.ObjectId.ToString()),
                            new KeyValuePair<string, string>("reportId", m.ReportObjectId.ToString()),
                            new KeyValuePair<string, string>("testId", m.TestObjectId.ToString()),
                            new KeyValuePair<string, string>("mediaType", m.MediaType.ToString().ToLower())
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
