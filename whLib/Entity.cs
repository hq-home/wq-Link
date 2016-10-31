using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wowhead
{
    public enum EntityType
    { 
        Object,
        Item,
        Npc,
        Quest,
        Zone,
        Map,
        Class,
        Race,
        Faction
    }

    public abstract class Entity
    {
        #region [ Properties ]
        private EntityType _eType;
        public EntityType EType { get { return _eType; } }

        public static string WowHeadHost
        {
            get { return ConfigurationManager.AppSettings["hq.wh.baseURL"]; }
        }

        public static string WowHeadDataPath
        {
            get { return Path.Combine(Helper.AppDataPath, ConfigurationManager.AppSettings["hq.wh.dataPath"]); }
        }

        public static string WowHeadLocalDBPath
        {
            get { return Path.Combine(Helper.AppDataPath, ConfigurationManager.AppSettings["hq.wh.localDBPath"]); }
        }

        private string StrType
        {
            get { return Enum.GetName(typeof(EntityType), _eType); }
        }
        public string Name { get; set; }

        public int Id { get; set; }

        protected bool IsValidId
        {
            get { return Id > 0; }
        }

        public string Description { get; set; }

        public virtual string XPathJSON
        {
            get { return "//div[@id='main-contents']/script[contains(text(), '" + SearchTemplate + "')]"; }
        }

        public virtual string XPathQuickInfo
        {
            get { return "//div[@id='main-contents']//table[@class='infobox']/tr/td/script/text()"; }
        }
        public virtual string SearchTemplate
        {
            get { return "(g_" + StrType.ToLower() + "s["; }
        }
        public virtual string StrRegExJSON
        {
            get { return @"\{(""(?<key>\w+)"":(?<val>-?\d+|\""([\w ':!\-,\.\?]|\\\\|\\""|\\)+\""|\[\d+]|\[(\-?\d+|\w+),(\-?\d+|\w+)]),?)+"; }
        }
        public virtual string StrRegExId
        {
            get { return  @"g_" + StrType.ToLower() + @"s\[(?<id>[\d]+)]"; }
        }
        public virtual string StrRegExQuickInfo
        {
            get { return @"printHtml\('(?<html>[\w\\\.,]*)'"; }
        }

        #endregion

        private Entity()
        {}

        protected Entity(EntityType etype)
        {
            _eType = etype;    
        }

        protected Entity(int id, string name, EntityType etype)
            :this(etype)
		{
			Id = id;
			Name = name;
		}

        public Entity(int id, EntityType etype)
            :this(etype)
        {
            Id = id;

            HtmlDocument htmlDoc = LoadWowHeadEntity(StrType.ToLower(), id);
            Initialize(htmlDoc);
        }

        protected void Initialize(HtmlDocument htmlDoc)
        {
            HtmlNode n = htmlDoc.DocumentNode.SelectSingleNode(XPathJSON);

            if (!IsValidId) ParseId(n.InnerHtml);

			if (!IsValidId) return;

            ParseJSON(n.InnerHtml.Substring(n.InnerHtml.IndexOf(SearchTemplate)));

            // Load Quick Info
            n = htmlDoc.DocumentNode.SelectSingleNode(XPathQuickInfo);

            if (n == null)
            {
                Helper.LogDebug(StrType + ". Can't find Quick Info block:" + Id);
                return;
            }

            ParseQuickInfo(n.InnerHtml, true);

            // Load Descriptions
            ParseDescription(htmlDoc); 

        }

        protected virtual void ParseDescription(HtmlDocument htmlDoc)
        {
            throw new NotImplementedException();
        }

        protected void ParseQuickInfo(string html, bool force)
        {
            string opnQI = "[ul][li]";
            string clsQI = "[/li][/ul]";
            string sepQI = "[/li][li]";

            Regex myRegex = new Regex(StrRegExQuickInfo, RegexOptions.Compiled);
            MatchCollection mc = myRegex.Matches(html);

            Match m = mc.Count > 0 ? mc[0] : null;
            if (m == null || !m.Success)
            {
                Helper.LogDebug(StrType + ". Can't parse Quick Info block:" + Id);
                return;
            }

            string phtml = Regex.Unescape(m.Groups["html"].Value);
            if (!phtml.StartsWith(opnQI) || !phtml.EndsWith(clsQI))
            {
                Helper.LogDebug(StrType + ". Invalid format of Quick Info block[" + Id + "]:" + phtml);
                return;
            }

            string[] lines = phtml.Substring(opnQI.Length, phtml.Length - opnQI.Length - clsQI.Length)
                    .Split(new string[] { sepQI }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < lines.Length; i++)
            {
                string l = lines[i].TrimEnd();
                int idx = 0, k, j;
                while (idx < l.Length && (Char.IsLetterOrDigit(l[idx])
                    || idx == 0 && l[idx] == '[')) idx++;

                string key = l.Substring(0, idx).ToLower();

                ProcessQIKey(key, idx, l, force);
            }
        }

        protected virtual void ProcessQIKey(string key, int idx, string l, bool force=false)
        {
            throw new NotImplementedException();
        }

        protected void ParseJSON(string json)
        {
            string sid = IsValidId ? Id.ToString() : json;

            Regex myRegex = new Regex(StrRegExJSON, RegexOptions.Compiled);
            MatchCollection mc = myRegex.Matches(json);

            if (mc.Count == 0)
            {
                Helper.LogDebug("Can't parse " + StrType + " JSON:" + sid);
                return;
            }
            if (mc.Count > 1)
            {
                Helper.LogDebug(StrType + " JSON: Invalid matches count:" + sid);
            }
            Match m = mc[0];
            if (!m.Success)
            {
                Helper.LogDebug("Can't parse " + StrType + " JSON, match is failed:" + sid);
                return;
            }

            ParseJSONMatch(m, sid);
        }

        protected void ParseJSONMatch(Match m, string sid)
        {
            sid = sid ?? Id.ToString();

            if (m.Groups.Count > 0)
            {
                CaptureCollection ccKeys = m.Groups["key"].Captures;

                if (ccKeys.Count < 1)
                {
                    Helper.LogDebug("Can't parse " + StrType + " JSON block:" + sid);
                }
                else ParseFoundProperties(ccKeys, m.Groups["val"].Captures, sid);
            }
            else Helper.LogDebug("Can't find any match group in " + StrType + " JSON block:" + sid);
        }

        protected virtual void ParseFoundProperties(CaptureCollection capKeys, CaptureCollection capVals, string sid)
        {
            throw new NotImplementedException();
        }

        protected void ParseId(string json)
        {
            Regex myRegex = new Regex(StrRegExId, RegexOptions.Compiled);
            MatchCollection mc = myRegex.Matches(json);

            if (mc.Count < 1 || mc[0].Groups.Count == 0)
            {
                Helper.LogDebug(StrType + ". Can't parse Id:" + json);
                return;
            }

            Id = int.Parse(mc[0].Groups["id"].Value);
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

            hw.PreRequest += Helper.Web_PreRequest;

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
        
        public static object LoadWoWHeadUrl(string url, bool forceRequest = false)
        {
            if (string.IsNullOrEmpty(url)) return null;

            string filepath = Helper.ReplaceRestrictedChars(url);

            if (filepath != null)
            {
                filepath = Path.Combine(WowHeadDataPath, filepath);

                if (!Directory.Exists(WowHeadDataPath))
                    Directory.CreateDirectory(WowHeadDataPath);
            }

            string utype = Helper.GetRequestedUrlType(url);

            HtmlDocument htmlDoc = null;

            if (forceRequest || !File.Exists(filepath))
            {
                if (0 <= "html php aspx".IndexOf(utype))
                {
                    HtmlWeb hw = new HtmlWeb();
                    hw.PreRequest += Helper.Web_PreRequest;
                    htmlDoc = hw.Load(url);
                    htmlDoc.Save(filepath, Encoding.UTF8);
                    return htmlDoc;
                }
                else
                {
                    PortalClient pc = new PortalClient();
                    pc.PreRequest += Helper.Web_PreRequest;
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
        }

        protected virtual void GetById(int id)
        {
            throw new NotImplementedException();
        }
    }
}
