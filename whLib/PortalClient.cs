using System;
using System.Data;
using System.Configuration;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Web.UI.HtmlControls;
using System.Net;
using System.IO;
using System.Xml;
using System.Collections.Specialized;

namespace Wowhead
{
    /// <summary>
    /// Summary description for MiniBrowser
    /// </summary>
    public class PortalClient : WebClient
    {
        public delegate bool PreRequestHandler(HttpWebRequest request);

        private CookieCollection _cookies;
        private CookieContainer _cookContainer;
        public CookieCollection Cookies
        {
            get
            {
                if (_cookies == null)
                {
                    _cookies = new CookieCollection();
                }
                return _cookies;
            }
        }

        private String _referer;
        private String Referer
        {
            get
            {
                return _referer;
            }
        }

        /// <summary>
        /// Occurs before an HTTP request is executed.
        /// </summary>
        public PreRequestHandler PreRequest;

        public PortalClient()
        {
        }

        public XmlDocument GetPage(String url)
        {

            //webClient.Headers.Add("Referer", Request.Headers["Referer"]);

            //Host:www.etorrent.ru
            //User-Agent:Mozilla/5.0 (Windows; U; Windows NT 5.1; en-US; rv:1.8.1.20) Gecko/20081217 Firefox/2.0.0.20
            //Accept:text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5
            //Accept-Language:en-us,en;q=0.5
            //Accept-Encoding:gzip,deflate
            //Accept-Charset:ISO-8859-1,utf-8;q=0.7,*;q=0.7
            //Keep-Alive:300
            //Proxy-Connection:keep-alive

            TextReader tr = new StreamReader(OpenRead(url));
            String str = tr.ReadToEnd();
            tr.Close();
            //return QuisterLib.HTML.Utils.CreateXmlFromHtml(str);
            throw new NotImplementedException();
        }

        public XmlDocument PostToPage(String url, Object paramArray)
        {
            Pair[] pars = paramArray as Pair[];

            NameValueCollection nvc = new NameValueCollection();

            for (int i = 0; i < pars.Length; i++)
            {
                nvc.Add(pars[i].First as String, pars[i].Second as String);
            }
            Byte[] response = UploadValues(url, "POST", nvc);

            String str = System.Text.Encoding.UTF8.GetString(response);
            //return QuisterLib.HTML.Utils.CreateXmlFromHtml(str);
            throw new NotImplementedException();
        }

        private void AssignHeaders(HttpWebRequest request)
        {
            //Headers.Add("Host", "www.etorrent.ru");
            // CONST
            Headers.Add("User-Agent", "Mozilla/4.0 (Windows; U; Windows NT 5.1; en-US;)");
            // CONST
            Headers.Add("Accept", "text/xml,application/xml,application/xhtml+xml,text/html;q=0.9,text/plain;q=0.8,image/png,*/*;q=0.5");
            // CONST/CONFIG
            Headers.Add("Accept-Language", "en-us,en;q=0.5");
            // CONST
            Headers.Add("Accept-Encoding", "gzip,deflate");
            // CONST
            //Headers.Add("Accept-Charset", "ISO-8859-1,utf-8;q=0.7,*;q=0.7");
            // CONST
            Headers.Add("Keep-Alive", "300");
            //Headers.Add("Proxy-Connection", "keep-alive"); HttpWebRequest.KeepAlive
            //request.KeepAlive = true;

            if (_referer != null)
            {
                Headers.Add("Referer", Referer);
            }
            _referer = request.RequestUri.AbsoluteUri;

            if (_cookContainer == null)
            {
                _cookContainer = new CookieContainer();

                _cookContainer.Add(Cookies);
            }

            request.CookieContainer = _cookContainer;
        }

        protected override WebRequest GetWebRequest(Uri address)
        {
            HttpWebRequest request = (HttpWebRequest)base.GetWebRequest(address);

            AssignHeaders(request);

            request.KeepAlive = true;

            request.PreAuthenticate = true;

            if (PreRequest != null)
            {
                PreRequest(request);
            }

            return request;
        }

        protected override WebResponse GetWebResponse(WebRequest request)
        {
            HttpWebResponse response = (HttpWebResponse)base.GetWebResponse(request);

            foreach (Cookie cook in response.Cookies)
            {
                Cookies.Add(cook);
            }

            // Perform any custom actions with the response ...
            return response;
        }




    }
}