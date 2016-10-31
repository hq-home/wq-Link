using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Web;

namespace Wowhead
{
    public static class Helper
    {
        private static EventHandler _loggerEH;
        public static InteractEntity GetInteractEntity(int id, string stype)
        {
            InteractEntityType type = (InteractEntityType)Enum.Parse(typeof(InteractEntityType), stype, true);
            
            object entity = null;

            switch(type)
            {
                case InteractEntityType.Npc:
                    entity = new Npc(id);
                    break;
                case InteractEntityType.Item:
                    entity = new Item(id);
                    break;
                case InteractEntityType.Object:
                    entity = new wObject(id);
                    break;
            }
            return new InteractEntity(type, entity);
        }

        public static void RegisterLogger(EventHandler loggerEH)
        {
            _loggerEH += loggerEH;
        }
        public static void LogDebug(string s)
        {
            if (_loggerEH != null)
            {
                _loggerEH(s, EventArgs.Empty);
            }
        }

        public static bool Web_PreRequest(HttpWebRequest webRequest)
        {
            webRequest.UserAgent = "Mozilla/5.0 (Windows NT 6.1; WOW64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/37.0.2062.120 Safari/537.36";
            webRequest.ContentType = "text/html,application/xhtml+xml,application/xml;q=0.9,image/webp,*/*;q=0.8";
            webRequest.Accept = "text/html,application/xhtml+xml,application/xml;q=0.9,*/*;q=0.8";
            webRequest.Headers[HttpRequestHeader.AcceptLanguage] = "en-US,en;q=0.8,ru;q=0.6,uk;q=0.4";

            return true;
        }


		/*public static object LoadWoWHeadUrl(string url, bool forceRequest = false)
		{
            if (string.IsNullOrEmpty(url)) return null;

            string filepath = ReplaceRestrictedChars(url);

            if (filepath != null)
			{
				filepath = Path.Combine(WowHeadDataPath, filepath);
	
                if (!Directory.Exists(WowHeadDataPath))
                    Directory.CreateDirectory(WowHeadDataPath);
			}

            string utype = GetRequestedUrlType(url);

			HtmlDocument htmlDoc = null;

            if (forceRequest || !File.Exists(filepath))
			{
                if (0 <= "html php aspx".IndexOf(utype))
                {
                    HtmlWeb hw = new HtmlWeb();
                    hw.PreRequest += Web_PreRequest;
                    htmlDoc = hw.Load(url);
                    htmlDoc.Save(filepath, Encoding.UTF8);
                    return htmlDoc;
                }
                else
                {
                    PortalClient pc = new PortalClient();
                    pc.PreRequest += Web_PreRequest;
                    byte[] myDataBuffer = pc.DownloadData(url);
                    string sfile = Encoding.UTF8.GetString(myDataBuffer);
                    File.WriteAllBytes(filepath, myDataBuffer);
                    return sfile;
                    
                }
			}
			else
			{
                if (0 <= "html php aspx".IndexOf(utype))
                {
                    htmlDoc = new HtmlDocument();
                    htmlDoc.Load(filepath);
                    return htmlDoc;
                }
                else
                {
                    if (0 <= "js css".IndexOf(utype))
                        return File.ReadAllText(filepath, Encoding.UTF8);
                    else
                        return File.ReadAllBytes(filepath);

                }
			}
		}*/

        public static string GetRequestedUrlType(string url)
        {
            int idx = url.IndexOf('?');
            if (idx >= 0)
                url = url.Substring(0, idx);

            idx = url.LastIndexOf('/');
            if (idx >= 0)
                url = url.Substring(idx + 1);

            idx = url.LastIndexOf('.');
            if (idx >= 0 && -1 == url.LastIndexOf('='))
                return url.Substring(idx + 1).ToLower();
            return "html";
        }

        public static string ReplaceRestrictedChars(string s)
        { 
            return s
				.Replace(':', '-')
				.Replace('#', '-')
				.Replace('=', '-')
				.Replace('&', '-')
				.Replace(';', '-')
				.Replace('%', '-')
				.Replace('+', '-')
				.Replace('/', '-')
                .Replace('?', '-')
				;
        }

		/*public static HtmlDocument LoadWowHeadEntity(string stype, int id, bool forceRequest = false)
		{
			string filepath = null;
			if (!forceRequest && id > 0)
			{
				int hp = (int)Math.Floor((double)id / 1000.0);
				int lp = id - hp * 1000;

				filepath = Path.Combine(WowHeadDataPath, String.Format("{0}\\{1:000}", stype, hp));

				if (!Directory.Exists(filepath))
					Directory.CreateDirectory(filepath);

				filepath += String.Format("\\{0:000}.html", lp);
			}
			
			HtmlWeb hw = new HtmlWeb();
			HtmlDocument htmlDoc = null;

			hw.PreRequest += Web_PreRequest;

			if (filepath == null || !File.Exists(filepath))
			{
				htmlDoc = hw.Load(String.Format("http://{0}/{1}={2}", WowHeadHost, stype, id));
				if (filepath != null) htmlDoc.Save(filepath, Encoding.UTF8);
			}
			else
			{
				htmlDoc = new HtmlDocument();
				htmlDoc.Load(filepath);
			}
			return htmlDoc;
		}*/

	    public static string GetIDByUrl(string url, ref string stype)
		{
			string regexp = String.Format(stype == null ? @"{0}/(?<stype>\w+)=(?<id>[0-9]+)" : @"{0}/{1}=(?<id>[0-9]+)", Entity.WowHeadHost, stype);

			Match match = Regex.Match(url, regexp, RegexOptions.IgnoreCase | RegexOptions.Compiled);

            if (match.Groups.Count > 0 &&  match.Groups["id"].Captures.Count > 0)
			{
				if (stype == null) stype = match.Groups["stype"].Captures[0].Value;
				return match.Groups["id"].Captures[0].Value;
			}
			else
			{
				LogDebug("Can not get valid ID from: " + url);
				return "unk-1";
			}
		}

		public static int ParseInt(string p, int defval = -1)
		{
			int i = defval;
			if (!string.IsNullOrEmpty(p))
			{
				int.TryParse(p, out i);
			}
			return i;
		}

		public static string ParseString(string p)
		{
			if (p != null && p.Length > 1)
				// TO DO: maybe add replacing of \\
				// JavaScriptSerializer
				return p.Substring(1, p.Length - 2);
			return p;
		}

		public static HttpContext Context
		{
			get { return HttpContext.Current; }
		}
		public static HttpServerUtility Server
		{
			get { return Context.Server; }
		}

		private static string _appDataPath = null;
		public static string AppDataPath
		{
			get
			{
				if(string.IsNullOrEmpty(_appDataPath))
				{
					_appDataPath = ConfigurationManager.AppSettings["hq.app.workPath"];
					if (_appDataPath[0] == '~')
						_appDataPath = Server.MapPath(_appDataPath);
				}
				return _appDataPath;
			}
		}
		public static string WowPediaHost
		{
			get { return ConfigurationManager.AppSettings["hq.wpedia.baseURL"]; }
		}
		public static string WowPediaDataPath
		{
			get { return Path.Combine(AppDataPath, ConfigurationManager.AppSettings["hq.wpedia.dataPath"]); }
		}

    }
}
