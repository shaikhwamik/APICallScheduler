using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Web;



namespace APICallScheduler.Common
{
    public enum HttpVerb
    {
        GET,
        POST,
        PUT,
        DELETE
    }
    public class WebService
    {
        string ServerUsername = string.Empty;
        string ServerPassword = string.Empty;
        public WebService()
        {
            ServerUsername = Convert.ToString(ConfigurationManager.AppSettings["ServerUsername"]);
            ServerPassword = Convert.ToString(ConfigurationManager.AppSettings["ServerPassword"]);
        }

        public class RestClient
        {
            public string EndPoint { get; set; }
            public HttpVerb Method { get; set; }
            public string ContentType { get; set; }
            public string PostData { get; set; }
            public string userName { get; set; }
            public string userPassword { get; set; }
            public List<KeyValuePair<string, string>> Header { get; set; }

            public RestClient(string contenttype)
            {
                EndPoint = "";
                Method = HttpVerb.GET;
                ContentType = contenttype;
                PostData = "";
            }

            public RestClient(string contenttype, string endpoint)
            {
                EndPoint = endpoint;
                Method = HttpVerb.GET;
                ContentType = contenttype;
                PostData = "";
            }

            public RestClient(string contenttype, string endpoint, HttpVerb method)
            {
                EndPoint = endpoint;
                Method = method;
                ContentType = contenttype;
                PostData = "";
            }

            public RestClient(string contenttype, string endpoint, HttpVerb method, string postData)
            {
                EndPoint = endpoint;
                Method = method;
                ContentType = contenttype;
                PostData = postData;
            }

            public RestClient(string contenttype, string endpoint, HttpVerb method, string postData, List<KeyValuePair<string, string>> header)
            {
                EndPoint = endpoint;
                Method = method;
                ContentType = contenttype;
                PostData = postData;
                Header = header;
            }

            public string MakeRequest()
            {
                return MakeRequest("");
            }

            public string MakeRequest(string parameters)
            {
                try
                {
                    ServicePointManager.ServerCertificateValidationCallback = delegate { return true; };

                    var request = (HttpWebRequest)WebRequest.Create(EndPoint + parameters);
                    ServicePointManager.Expect100Continue = true;
                    ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
                    request.KeepAlive = false;
                    request.Method = Method.ToString();
                    request.ContentLength = 0;
                    request.ContentType = ContentType;

                    if (Header != null && Header.Count > 0)
                    {
                        foreach (var item in Header)
                        {
                            request.Headers.Add(item.Key, item.Value);
                        }
                    }

                    if (!string.IsNullOrEmpty(PostData) && Method == HttpVerb.POST)
                    {
                        var encoding = new UTF8Encoding();
                        var bytes = Encoding.GetEncoding("iso-8859-1").GetBytes(PostData);
                        request.ContentLength = bytes.Length;
                        using (var writeStream = request.GetRequestStream())
                        {
                            writeStream.Write(bytes, 0, bytes.Length);
                        }
                    }
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        var responseValue = string.Empty;
                        if (response.StatusCode != HttpStatusCode.OK && response.StatusCode != HttpStatusCode.Created)
                        {
                            var message = String.Format("Request failed. Received HTTP {0}", response.StatusCode);
                            throw new ApplicationException(message);
                        }
                        using (var responseStream = response.GetResponseStream())
                        {
                            if (responseStream != null)
                                using (var reader = new StreamReader(responseStream))
                                {
                                    responseValue = reader.ReadToEnd();
                                }
                        }
                        return responseValue;
                    }
                }
                catch (WebException webex)
                {
                    WebResponse errResp = webex.Response;
                    using (Stream respStream = errResp.GetResponseStream())
                    {
                        StreamReader reader = new StreamReader(respStream);
                        string text = reader.ReadToEnd();
                        if (string.IsNullOrEmpty(text))
                        {
                            text = webex.Message;
                        }
                        return text;
                    }
                }
            }

            public static bool ValidateServerCertificate(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
            {
                if (sslPolicyErrors == SslPolicyErrors.None)
                    return true;
                return false;
            }
        }
    }
}