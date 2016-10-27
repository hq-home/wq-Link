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

		/// <summary>
		/// [ Deprecated ]
		/// </summary>
		/// <param name="fileUrl"></param>
		/// <param name="stype"></param>
		/// <param name="saveTo"></param>
		/// <returns></returns>
		public static HtmlDocument LoadWoWHeadUrl(string fileUrl, string stype = null, string saveTo = null)
		{
			string sID = GetIDByUrl(fileUrl, ref stype);

			string filepath = sID;

			int id = -1;
			if (int.TryParse(sID, out id))
			{
				if (id > 0)
				{
					int hp = (int)Math.Floor((double)id / 1000.0);
					int lp = id - hp * 1000;
					filepath = String.Format("{2}\\{0:000}\\{1:000}.html", hp, lp, stype);
				}
			}

			if(saveTo != null)
			{
				filepath = Path.Combine(saveTo, filepath);
				string justPath = Path.GetDirectoryName(filepath);
				if (!Directory.Exists(justPath))
					Directory.CreateDirectory(justPath);
			}

			HtmlWeb hw = new HtmlWeb();
			HtmlDocument htmlDoc = null;

			hw.PreRequest += Web_PreRequest;

			if (saveTo == null || !File.Exists(filepath))
			{
				htmlDoc = hw.Load(fileUrl);
				if (saveTo != null) htmlDoc.Save(filepath, Encoding.UTF8);
			}
			else
			{
				htmlDoc = new HtmlDocument();
				htmlDoc.Load(filepath);
			}
			return htmlDoc;
		}

		public static HtmlDocument LoadWowHeadEntity(string stype, int id, bool forceRequest = false)
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
		}

	    public static string GetIDByUrl(string url, ref string stype)
		{
			string regexp = String.Format(stype == null ? @"{0}/(?<stype>\w+)=(?<id>[0-9]+)" : @"{0}/{1}=(?<id>[0-9]+)", WowHeadHost, stype);

			Match match = Regex.Match(url, regexp, RegexOptions.IgnoreCase | RegexOptions.Compiled);

			if (match.Groups.Count > 0)
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

	    public static string WowHeadHost
	    {
			get { return ConfigurationManager.AppSettings["hq.wh.baseURL"]; }
	    }

		public static string WowPediaHost
		{
			get { return ConfigurationManager.AppSettings["hq.wpedia.baseURL"]; }
		}

		public static string WowHeadDataPath
		{
			get { return Path.Combine(AppDataPath, ConfigurationManager.AppSettings["hq.wh.dataPath"]); }
		}

		public static string WowHeadLocalDBPath
		{
			get { return Path.Combine(AppDataPath, ConfigurationManager.AppSettings["hq.wh.localDBPath"]); }
		}
		public static string WowPediaDataPath
		{
			get { return Path.Combine(AppDataPath, ConfigurationManager.AppSettings["hq.wpedia.dataPath"]); }
		}

    }
}
