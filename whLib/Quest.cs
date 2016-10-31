using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;


namespace Wowhead
{
    public enum QuestType
    {
        Normal = 0,
        Group = 1,
        Class = 21,
        Raid = 62,
        Dungeon = 81
        /*
         <option value="0">Normal</option>
<option value="1">Group</option>
<option value="21">Class</option>
<option value="41">PvP</option>
<option value="62">Raid</option>
<option value="81">Dungeon</option>
<option value="82">World Event</option>
<option value="83">Legendary</option>
<option value="84">Escort</option>
<option value="85">Heroic</option>
<option value="88">10 Player</option>
<option value="89">25 Player</option>
<option value="98">Scenario</option>
<option value="102">Account</option>
         */
    }

	public class Quest
	{
		#region [ Properties ]
		/*
		 * 27066
		 * $.extend(g_quests[27066], {"category":-262,"category2":4,"classs":16,"id":27066,"level":3,"name":"Learning the Word","race":160,"reprewards":[[81,75]],"reqclass":16,"reqlevel":2,"reqrace":160,"side":2,"type":21,"xp":125});
		 Level: 3
Requires level 2
Type: Class
Side: Horde
Races:  Tauren,  Troll
Class:  Priest
Start: Seer Ravenfeather
End: Seer Ravenfeather
Sharable
Difficulty: 2  6  8
Added in patch 4.0.3
		 
		 */

		private bool IsValidId
		{
			get { return Id > 0; }
		}

		/// <summary>
        /// Where quest arrived from (quest list, zone list - just for GUI )
        /// </summary>
        public string SourceType { get; set; }

		/// <summary>
		/// not clear property
		/// </summary>
		public string IssueList { get; set; }
		/// <summary>
		/// not clear property
		/// </summary>
		public string Rewards { get; set; }
		/// <summary>
		/// not clear property
		/// </summary>
		public int wflags { get; set; }

		public string Name { get; set; }
		public int Id { get; set; }
		public string ShortDescription { get; set; }
		public string Description { get; set; }
		public int RequiredLevel { get; set; }
		public int Level { get; set; }
		/// <summary>
		/// default 1
		/// </summary>
		public int RecomendedGroupOf { get; set; } 

		public InteractEntity QuestGiver { get; set; }
		public InteractEntity QuestTurnIn { get; set; }

		public Side Side { get; set; }

		public bool Sharable { get; set; }

		/// <summary>
		/// Category:
		/// </summary>
		public Zone WZone { get; set; }

		/// <summary>
		/// Category2: 0, 1, 8, 10, 11, 12 
		/// </summary>
		public Map WMap { get; set; }

		private List<wClass> _classes = null;
		/// <summary>
		/// Category2: 4
		/// Category: -61, -81, -82, -141, -161, -162, -261, -262, -263, -372, -395
		/// </summary>
        public List<wClass> Classes 
        {
			get { return _classes; }
		}

		private List<Race> _races = null;
		public List<Race> Races
		{
			get { return _races; }
		}

        /// <summary>
		/// Difficulty: 16  17  22  26
		/// Very Hard, Hard, Normal, Easy
		/// </summary>
		public int[] Difficulty { get; set; }

		/// <summary>
		/// Do account to Loremaster
		/// </summary>
		public bool Loremaster { get; set; }
		
		/// <summary>
		/// Added in patch X.X.X
		/// </summary>
		public string AddedIn { get; set; }

        public QuestType Type { get; set; }
		private string _type { get; set; }

		// Menu keys
		// Zones: ...
		// Classes: -61, -81, -82, -141, -161, -162, -261, -262, -263, -372, -395
		// Battlegrounds: 2597 3277 3358
		private int _categoryId;
		// Continents (Maps) : 0, 1, 8, 10, 11, 12
		// Uncategorized: -2
		// Dungeons: 2
		// Raids: 3
		// Classes: 4
		// Professions: 5
		// Battlegrounds: 6
		// Mescellaneous: 7
		// WorldEvents: 9
		private int _category2Id; 


		#endregion

		/*public Quest(string html)
		{
			HtmlDocument htmlDoc = new HtmlDocument();
			htmlDoc.LoadHtml(html);
			Initialize(htmlDoc);
		}*/

        public Quest(string json)
        {
            RecomendedGroupOf = 1;
            Sharable = true;
            Difficulty = new int[4];

            ParseJSON(json);
        }

		public Quest(HtmlDocument htmlDoc)
		{
			Initialize(htmlDoc);
		}

		public Quest(int id)
		{
			HtmlDocument htmlDoc = Entity.LoadWowHeadEntity("quest", id);
			Initialize(htmlDoc);
		}

		private void Initialize(HtmlDocument htmlDoc)
		{
			RecomendedGroupOf = 1;
            Sharable = true;
            Difficulty = new int[4];

            HtmlNode n = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='main-contents']/script[contains(text(), '(g_quests[')]");

			ParseId(n.InnerHtml);

			if (!IsValidId) return;

            ParseJSON(n.InnerHtml.Substring(n.InnerHtml.IndexOf("(g_quests[")));

            // Load Quick Info
            n = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='main-contents']//table[@class='infobox']/tr/td/script/text()");

            // http://wow.gamepedia.com/Badlands_storyline --- null reference

            ParseQuickInfo(n.InnerHtml, true);
            // Load Descriptions
            ParseDescription(htmlDoc);  

            bool debug = true;
		}

		private void ParseId(string json)
		{
			string strRegex = @"g_quests\[(?<id>[\d]+)]";

			Regex myRegex = new Regex(strRegex, RegexOptions.Compiled);
			MatchCollection mc = myRegex.Matches(json);

			if (mc.Count < 1 || mc[0].Groups.Count == 0)
			{
				Helper.LogDebug("Quest. Can't parse Id:" + json);
				return;
			}

			Id = int.Parse(mc[0].Groups["id"].Value);
		}

		private void ParseJSON(string json)
        {
            string strRegex = @"\{(""(?<key>\w+)"":(?<val>-?\d+|\""([\w ':!\-,\.\?]|\\\\|\\""|\\)+\""|\[(\[-?\d+,-?\d+],?)*]),?)+}";
            //\{("(?<key>[\w]+)"\s*:\s*((?<val>(\-?[\d]*))|((?<val>"([\w \:\'\,!]|\\\\|\\"|\\)*"))|(?<val>(\[(\[-?\d+,-?\d+],?)*])))?,?)*},?

			string sid = IsValidId ? Id.ToString() : json;

            Regex myRegex = new Regex(strRegex, RegexOptions.Compiled);
            MatchCollection mc = myRegex.Matches(json);

            if (mc.Count == 0)
            {
                Helper.LogDebug("Can't parse Quest JSON:" + sid);
                return ;
            }
            if (mc.Count > 1)
            {
                Helper.LogDebug("Quest JSON: Invalid matches count:" + sid);
            }
            Match m = mc[0];
            if (!m.Success)
            {
				Helper.LogDebug("Can't parse Quest JSON, match is failed:" + sid);
                return;
            }

			ParseJSONMatch(m, sid);
        }

		/// <summary>
		/// 
		/// </summary>
		/// <param name="m"></param>
		/// <param name="sid">Just for logger</param>
		private void ParseJSONMatch(Match m, string sid = null)
		{
			sid = sid ?? Id.ToString();

			if (m.Groups.Count > 0)
			{
				CaptureCollection ccKeys = m.Groups["key"].Captures;

				if (ccKeys.Count < 1)
				{
					Helper.LogDebug("Can't parse JSON block:" + sid);
				}
				else ParseFoundProperties(ccKeys, m.Groups["val"].Captures, sid);
			}
			else Helper.LogDebug("Can't find any match group in JSON block:" + sid);
		}

		/// <summary>
		/// 
		/// </summary>
		/// <param name="capKeys"></param>
		/// <param name="capVals"></param>
		/// <param name="sid">Just for Logger</param>
		private void ParseFoundProperties(CaptureCollection capKeys, CaptureCollection capVals, string sid)
		{
			for (int i = 0; i < capKeys.Count; i++)
			{
				Capture capKey = capKeys[i];
				Capture capVal = capVals[i];

				switch (capKey.Value)
				{
					case "category":
						_categoryId = Helper.ParseInt(capVal.Value);
						break;
					case "category2":
						_category2Id = Helper.ParseInt(capVal.Value);
						break;
					case "level":
						Level = Helper.ParseInt(capVal.Value);
						break;
					case "name":
						Name = Helper.ParseString(capVal.Value);
						break;
					case "reqlevel":
						RequiredLevel = Helper.ParseInt(capVal.Value);
						break;
					case "side":
						Side = (Side)Helper.ParseInt(capVal.Value);
						break;
					case "wflags":
						wflags = Helper.ParseInt(capVal.Value);
						break;
					case "reqrace":
						_races = Race.ParseFlags(Helper.ParseInt(capVal.Value));
						break;
					case "reqclass":
						_classes = wClass.ParseFlags(Helper.ParseInt(capVal.Value));
						break;
					case "type":
						int t = Helper.ParseInt(capVal.Value);
						if (!Enum.IsDefined(typeof(QuestType), t))
						{
							Helper.LogDebug("Unknown Type [" + (Id == 0 ? sid : Id.ToString()) + "]:" + capVal.Value);
						}
						else Type = (QuestType)t;

						break;
					case "id":
						if (Id == 0)
							Id = Helper.ParseInt(capVal.Value);
						break;
					case "money":
					case "xp":
					case "reprewards":
					case "itemrewards":
					case "itemchoices":
                    case "currencyrewards":
					case "race": // same as reqrace
					case "classs": // same as reqclass
						break;
					default:
						Helper.LogDebug("Unknown property[" + (Id == 0 ? sid : Id.ToString()) + "]:" + capKey.Value);
						break;
				}
			}
		}

        private void ParseDescription(HtmlDocument htmlDoc)
        {
            //Short Description
            // //div[@id="main-contents"]/div/div[@id="touch-quest-medrec"]:after/sript:after/#text, parse
            HtmlNode n = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='main-contents']/div/script[contains(text(),'#quest-medrec-container')]/following-sibling::text()");

            ShortDescription = n.InnerHtml;
            // //div[@id="main-contents"]/div/h2[value='Description']:after/#text, parse

            //Long Description
            HtmlNodeCollection hnc = htmlDoc.DocumentNode.SelectNodes("//div[@id='main-contents']/div/h2[contains(text(),'Description')]/following-sibling::node()");

            Description = "";
            foreach (HtmlNode hn in hnc)
            {
                if (hn.NodeType == HtmlNodeType.Text)
                    Description += hn.InnerHtml.Replace("\n", "").Replace("\r", "");
                else if (hn.NodeType == HtmlNodeType.Element && hn.Name == "br")
                {
                    Description += Environment.NewLine;
                }
                else break;
            }
        }

        private void ParseQuickInfo(string html, bool force = false)
        {
            string opnQI = "[ul][li]";
            string clsQI = "[/li][/ul]";
            string sepQI = "[/li][li]";

            Regex myRegex = new Regex(@"printHtml\('(?<html>[\w\\\.,]*)'", RegexOptions.Compiled);
            MatchCollection mc = myRegex.Matches(html);

            Match m = mc.Count > 0 ? mc[0] : null;
            if (m == null || !m.Success)
            {
                Helper.LogDebug("Can't parse Quick Info block:" + Id);
                return;
            }

            string phtml = Regex.Unescape(m.Groups["html"].Value);
            if (!phtml.StartsWith(opnQI) || !phtml.EndsWith(clsQI))
            {
                Helper.LogDebug("Invalid format of Quick Info block[" + Id + "]:" + phtml);
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

        private void ProcessQIKey(string key, int pos, string line, bool force = false)
        {
            Regex myRegex = null;
            MatchCollection mc = null;
            Match m = null;
            int j, k;

            switch (key)
            {
                case "level":
                    if (force || Level < 1)
                    {
                        k = line.Length - 1;
                        while (Char.IsDigit(line[k])) k--;
                        Level = Helper.ParseInt(line.Substring(k + 1));
                    }
                    break;
                case "requires":
                    if (force || RequiredLevel < 1)
                    {
                        k = line.Length - 1;
                        while (Char.IsDigit(line[k])) k--;
                        Level = Helper.ParseInt(line.Substring(k + 1));
                    }
                    break;
                case "loremaster":
                    Loremaster = true;
                    break;
                case "side":
                    if (force || (int)Side == -1)
                    {
                        myRegex = new Regex(@"](?<side>\w+)\[", RegexOptions.Compiled);
                        mc = myRegex.Matches(line);

                        m = mc.Count > 0 ? mc[0] : null;
                        if (m == null || !m.Success)
                        {
                            line = line.Substring(pos + 1).Trim();
                        }
                        else line = m.Groups["side"].Value;
                        Side _dside = (Wowhead.Side)(-1);

                        if (!Enum.TryParse(line, true, out _dside))
                        {
                            Helper.LogDebug("Can't parse QI Side[" + Id + "]:" + line);
                        }
                        else
                            Side = _dside;
                    }
                    break;
                case "[icon":
                    myRegex = new Regex(@"](?<key>\w+):\s*\[(url=/)?(?<type>\w*)=(?<id>\d+)]", RegexOptions.Compiled);
                    mc = myRegex.Matches(line);

                    m = mc.Count > 0 ? mc[0] : null;
                    if (m == null || !m.Success)
                        Helper.LogDebug("Can't parse QI Icon[" + Id + "]:" + line);

                    Group gKey = m.Groups["key"];
                    CaptureCollection gTypeCap = m.Groups["type"].Captures;
                    CaptureCollection gIdCap = m.Groups["id"].Captures;
                    for (j = 0; j < gKey.Captures.Count; j++)
                    {
                        Capture c = gKey.Captures[j];

                        switch (c.Value.ToLower())
                        {
                            case "start":
                                QuestGiver = Helper.GetInteractEntity(Helper.ParseInt(gIdCap[j].Value), gTypeCap[j].Value);
                                break;
                            case "end":
                                QuestTurnIn = Helper.GetInteractEntity(Helper.ParseInt(gIdCap[j].Value), gTypeCap[j].Value);
                                break;
                            default:
                                Helper.LogDebug("Unknown QI line[" + Id + "]:" + line);
                                break;
                        }
                    }

                    break;
                case "not":
                    line = line.Substring(3).Trim().ToLower();
                    if (line == "sharable")
                        Sharable = false;
                    else
                        Helper.LogDebug("Unknown sub 'not' key[" + Id + "]:" + line);
                    break;
                case "difficulty":
                    myRegex = new Regex(@"r(?<d>\d)](?<level>\d+)\[", RegexOptions.Compiled);
                    mc = myRegex.Matches(line);

                    for (j = 0; j < mc.Count; j++)
                    {
                        if (!mc[j].Success)
                            Helper.LogDebug("Can't parse QI difficulty[" + Id + "]:" + line);

                        Group gIdx = mc[j].Groups["d"];
                        CaptureCollection gLevelCap = mc[j].Groups["level"].Captures;

                        pos = Helper.ParseInt(gIdx.Captures[0].Value) - 1;
                        Difficulty[pos] = Helper.ParseInt(gLevelCap[0].Value);
                    }
                    break;
                case "added":
                    k = line.Length - 1;
                    while (Char.IsDigit(line[k]) || line[k] == '.') k--;
                    AddedIn = line.Substring(k + 1);
                    break;
                case "type":
                    string lt = line.Substring("Type:".Length).Trim();
                    QuestType qt = (QuestType)(-1);
                    if (!Enum.TryParse(lt, out qt))
                    {
                        Helper.LogDebug("Unknown QI Type [" + Id + "]:" + lt);
                    }
                    else
                        if (force || Type == QuestType.Normal)
                            Type = qt;
                    break;
                case "race":
                case "races":
                    if (force || _races == null)
                    {
                        myRegex = new Regex(@"=(?<id>\d+)", RegexOptions.Compiled);
                        mc = myRegex.Matches(line);
                        _races = new List<Race>();

                        for (j = 0; j < mc.Count; j++)
                        {
                            if (!mc[j].Success)
                                Helper.LogDebug("Can't parse QI Races[" + Id + "]:" + line);

                            _races.Add(Race.GetById(Helper.ParseInt(mc[j].Groups["id"].Captures[0].Value)));
                        }
                    }
                    break;
                case "class":
                    if (force || _classes == null)
                    {
                        myRegex = new Regex(@"=(?<id>\d+)", RegexOptions.Compiled);
                        mc = myRegex.Matches(line);
                        _classes = new List<wClass>();

                        for (j = 0; j < mc.Count; j++)
                        {
                            if (!mc[j].Success)
                                Helper.LogDebug("Can't parse QI Races[" + Id + "]:" + line);

                            _classes.Add(wClass.GetById(Helper.ParseInt(mc[j].Groups["id"].Captures[0].Value)));
                        }
                    }
                    break;
                case "sharable": // by default - sharable
                    break;
                default:
                    Helper.LogDebug("Unknown key[" + Id + "]:" + key);
                    break;
            }
        }

        public static List<Quest> LoadQuestsByFilter(string filter)
        {
            string xpQuestsBlock = "//div[@id='main-contents']/script[contains(text(), 'Listview({template:')]";

            HtmlDocument htmlDoc = null;

            string whFilter = string.Format("http://{0}/quests?filter={1}", Entity.WowHeadHost, filter);

            htmlDoc = (HtmlDocument)Entity.LoadWoWHeadUrl(whFilter);
            HtmlNode n = htmlDoc.DocumentNode.SelectSingleNode(xpQuestsBlock);

            return ParseJSONquests(n.InnerHtml, "f");
        }

        public static List<Quest> LoadQuestsByCategory(string category2, string category = null)
        {
            string xpQuestsBlock = "//div[@id='main-contents']/script[contains(text(), 'Listview({template:')]";

            HtmlDocument htmlDoc = null;
            String qurl = string.Format("http://{0}/quests={1}{2}{3}",
               Entity.WowHeadHost,
               category2,
               string.IsNullOrEmpty(category) ? string.Empty : ".",
               string.IsNullOrEmpty(category) ? string.Empty : category
               );

            htmlDoc = (HtmlDocument)Entity.LoadWoWHeadUrl(qurl);
            HtmlNode n = htmlDoc.DocumentNode.SelectSingleNode(xpQuestsBlock);
            string sn = n.InnerHtml;

            var quests = ParseJSONquests(sn, "q");

			if (!string.IsNullOrEmpty(category))
			{
				int cat = Helper.ParseInt(category, 0);
				htmlDoc = null;

				if (cat > 0)
				{
					htmlDoc = Entity.LoadWowHeadEntity("zone", cat);
					n = htmlDoc.DocumentNode.SelectSingleNode("//div[@id='main-contents']/script[contains(text(), \"template: 'quest\")]");
					sn = n.InnerHtml;
				}

				if (htmlDoc != null)
				{
					sn = sn.Substring(sn.IndexOf("template: 'quest"));
					ParseJSONquests(sn, "z", quests);
				}
			}

			return quests;
        }

        private static List<Quest> ParseJSONquests(string json, string sourceType = null, List<Quest> quests = null)
		{
			string strRegex = @"data:\s*\[(?<quest>\{(""\w+"":(-?\d+|\""([\w ':!-]|\\\\|\\""|\\)+\""|\[(\[-?\d+,-?\d+],?)*]),?)+},?)*]}\);";

			Regex myRegex = new Regex(strRegex, RegexOptions.Compiled);
			MatchCollection mc = myRegex.Matches(json);

			if (mc.Count == 0)
			{
				Helper.LogDebug("Can't parse Quest block:" + json);
				return null;
			}
			if (mc.Count > 1)
			{
				Helper.LogDebug("Invalid matches count:" + json);
				return null;
			}
			Match m = mc[0];
			if (!m.Success)
			{
				Helper.LogDebug("Can't parse Quest block, match is failed:" + json);
				return null;
			}

			Group gQuests = m.Groups["quest"];

			if (quests == null)
				quests = new List<Quest>(gQuests.Captures.Count);

			foreach (Capture ctr in gQuests.Captures)
			{
				Quest q = new Quest(ctr.Value);
				if (quests.Find(f => f.Id == q.Id) == null)
				{
					q.SourceType = sourceType;
					quests.Add(q);
				}
			}

			return quests;
		}
	}
}

/****************************************
 * Information about wowhead structures
 ****************************************/
/* Quest QuickInfo template
 * ************************
 *  [ul][li]Level: 1
 * [/li][li]Requires level 1
 * [/li][li]Loremaster: Yes
 * [/li][li]Side: [span class=icon-horde]Horde[/span]
 * [/li][li][icon name=quest_start]Start: [url=/npc=2981]Chief Hawkwind[/url][/icon]
 * [/li][li][icon name=quest_end]End: [url=/npc=2980]Grull Hawkwind[/url][/icon]
 * [/li][li]Not sharable
 * [/li][li]Difficulty: [color=r2]1[/color][small] &nbsp;[/small][color=r3]4[/color][small] &nbsp;[/small][color=r4]6[/color]
 * [/li][li]Added in patch 4.0.3
 */
/* Quest JSON templates
 * **********************
 * $.extend(g_quests[3094], {"category":-263,"category2":4,"classs":1024,"id":3094,"level":3,"name":"Verdant Note","race":160,"reprewards":[[81,75]],"reqclass":1024,"reqlevel":2,"reqrace":160,"side":2,"type":21,"xp":125});
 * $.extend(g_quests[14449], {"category":215,"category2":1,"id":14449,"level":1,"money":8,"name":"The First Step","reqlevel":1,"side":2,"wflags":160,"xp":40});
 */

/*
ShortDescription	//div[@id="main-contents"]/div/div[@id="touch-quest-medrec"]:after/sript:after/#text, parse		
			
Description	//div[@id="main-contents"]/div/h2[value='Description']:after/#text, parse		
			
IssueList	//div[@id="main-contents"]/div/table[@class='iconlist'], parse		
	//div[@id="main-contents"]/script, parseVar:g_items		
 * 
QuestGiver	//div[@id="main-contents"]/div/table[@class='infobox']/tr/td/script, parse Markup.printHtml
QuestTurnIn	//div[@id="main-contents"]/div/table[@class='infobox']/tr/td/script, parse Markup.printHtml
	
Side	//div[@id="main-contents"]/div/table[@class='infobox'']/tr/td/script, parse Markup.printHtml
	//div[@id="main-contents"]/script, parseVar:g_quests
	
Type	//div[@id="main-contents"]/div/table[@class='infobox'']/tr/td/script, parse Markup.printHtml
	//div[@id="main-contents"]/script, parseVar:g_quests
	
Sharable	//div[@id="main-contents"]/div/table[@class='infobox'']/tr/td/script, parse Markup.printHtml
Loremaster	//div[@id="main-contents"]/div/table[@class='infobox'']/tr/td/script, parse Markup.printHtml
Difficulty	//div[@id="main-contents"]/div/table[@class='infobox'']/tr/td/script, parse Markup.printHtml
Added in	//div[@id="main-contents"]/div/table[@class='infobox'']/tr/td/script, parse Markup.printHtml
 
 */


/*
 
 
// Races  http://ru.wowhead.com/races -> #lv-races->script
      {"classes":443,"faction":72,"id":1,"leader":29611,"name":"Человек","side":1,"zone":12},
      {"classes":365,"faction":76,"id":2,"leader":4949,"name":"Орк","side":2,"zone":14},
      {"classes":63,"faction":47,"id":3,"leader":2784,"name":"Дворф","side":1,"zone":1},
      {"classes":1085,"faction":69,"id":4,"leader":7999,"name":"Ночной эльф","side":1,"zone":141},
      {"classes":441,"faction":68,"id":5,"leader":10181,"name":"Нежить","side":2,"zone":85},
      {"classes":1125,"faction":81,"id":6,"leader":3057,"name":"Таурен","side":2,"zone":215},
      {"classes":425,"faction":54,"id":7,"leader":7937,"name":"Гном","side":1,"zone":1},
      {"classes":253,"faction":530,"id":8,"leader":10540,"name":"Тролль","side":2,"zone":14},
      {"classes":446,"expansion":1,"faction":911,"id":10,"leader":16802,"name":"Эльф крови","side":2,"zone":3430},
      {"classes":247,"expansion":1,"faction":930,"id":11,"leader":17468,"name":"Дреней","side":1,"zone":3524}

// classes flags
    

// Classes  http://ru.wowhead.com/classes -> #lv-classes->script
      {"armor":94,  "id":1, "name":"Воин",         "races":1279,"roles":10,"weapon":370175,"power":1},
      {"armor":222, "id":2, "name":"Паладин",      "races":1541,"roles":11,"weapon":499},
      {"armor":14,  "id":3, "name":"Охотник",      "races":1710,"roles":4, "weapon":370127},
      {"armor":6,   "id":4, "name":"Разбойник",    "races":735, "roles":2, "weapon":368797,"power":3},
      {"armor":2,   "id":5, "name":"Жрец",         "races":1693,"roles":5, "weapon":558096},
      {"armor":542, "id":6, "name":"Рыцарь смерти","races":1791,"roles":10,"weapon":499,   "power":6,"expansion":2,"hero":55},
      {"armor":1102,"id":7, "name":"Шаман",        "races":1186,"roles":7, "weapon":42035},
      {"armor":2,   "id":8, "name":"Маг",          "races":1745,"roles":4, "weapon":558208},
      {"armor":2,   "id":9, "name":"Чернокнижник", "races":595, "roles":4, "weapon":558208},
      {"armor":262, "id":11,"name":"Друид",        "races":40,  "roles":15,"weapon":42096}

 */


/* roles flags
   0010 - melee-DD
   0100 - range-DD
   0001 - Healer
   1000 - Tank
   
   armor flags
   2    - 000 00000010 - Ткань*
   4    - 000 00000100 - Кожа*
   8    - 000 00001000 - Кольчуга*
   16   - 000 00010000 - Латы*
   64   - 000 01000000 - Щит*
   128  - 000 10000000 - Манускрипт*
   256  - 001 00000000 - Реликвия*
   512  - 010 00000000 - Печать*
   1024 - 100 00000000 - Тотем*
   
   weapon flags
            #00001     - Экс
            #00002     - Двур.Экс
            #00010     - Мейс
            #00020     - Двур.Мейс
            #00040     - Копье
            #00080     - Меч
            #00100     - Двур.Меч
            #00200     - ?
            #00400     - Посох
            #00800     - ?
            #01000     - ?
            #02000     - Фист
            #08000     - Кинжал
            #0A000     - Кинжал
            #5000С(4)  - Ган, Лук, Арбалет, XXX (Бросок, Выстрел)
            #20000     - ?
            #80000     - Ванд
   
   races flags
var g_chr_races =
{
    1: "Человек", 2: "Орк", 3: "Дворф", 4: "Ночной эльф", 5: "Нежить", 6: "Таурен", 7: "Гном",
    8: "Тролль", 10: "Эльф крови", 11: "Дреней"
}; 
   //(d & 1 << b - 1)
   // 40 & 1 << 4-1 == #28 & #20 - 1
   // 40 & 1 << 6-1 == #28 & #40 - 1     
   40   - 
   595  -
   735  -
   1186 -
   1279 -
   1541 -
   1693 -
   1710 -
   1745 -
   1791 -
   
   roles flags
*/
// http://ru.wowhead.com/data=quests&partial&callback=$WowheadProfiler.loadOnDemand&1287045509980
/*
_[<questID>]={
  "category":<zoneID>,
  "category2":<zonesID>,
  "side":<sideFlag[2=Horde,1=Alliance,3=Both]>,
  "reqclass":<classFlag[1024=Druid(11)]>,
  "reqrace":<raceFlag[32=Tauren(6),8=NE(4)]>
  };
  
  
var _ = g_quests;
_[2]={"category":331,"category2":1,"side":2};
_[5]={"category":10,"category2":0,"side":1};
_[6]={"category":12,"category2":0,"side":1};
_[7]={"category":12,"category2":0,"side":1};
_[8]={"category":85,"category2":0,"side":2};
_[9]={"category":40,"category2":0,"side":1};
_[10]={"category":440,"category2":1,"side":3};
_[11]={"category":12,"category2":0,"side":1};
_[12]={"category":40,"category2":0,"side":1};
_[13]={"category":40,"category2":0,"side":1};
_[14]={"category":40,"category2":0,"side":1};
_[15]={"category":12,"category2":0,"side":1};
_[17]={"category":1337,"category2":2,"side":1};
_[18]={"category":12,"category2":0,"side":1};
_[19]={"category":44,"category2":0,"side":1};
_[20]={"category":44,"category2":0,"side":1};
_[21]={"category":12,"category2":0,"side":1};
_[22]={"category":40,"category2":0,"side":1};
_[23]={"category":331,"category2":1,"side":2};
_[24]={"category":331,"category2":1,"side":2};
_[25]={"category":331,"category2":1,"side":2};
_[26]={"category2":4,"reqclass":1024,"side":2};
_[27]={"category2":4,"reqclass":1024,"side":1};
_[28]={"category2":4,"reqclass":1024,"reqrace":32,"side":2};
_[29]={"category2":4,"reqclass":1024,"reqrace":8,"side":1};
_[30]={"category2":4,"reqclass":1024,"reqrace":32,"side":2};
_[31]={"category2":4,"reqclass":1024,"reqrace":32,"side":2};
_[32]={"category":440,"category2":1,"side":2};
_[33]={"category":12,"category2":0,"side":1};
_[34]={"category":44,"category2":0,"side":1};
_[35]={"category":12,"category2":0,"side":1};
_[36]={"category":40,"category2":0,"side":1};
_[37]={"category":12,"category2":0,"side":1};
_[38]={"category":40,"category2":0,"side":1};
_[39]={"category":12,"category2":0,"side":1};
_[40]={"category":12,"category2":0,"side":1};
_[45]={"category":12,"category2":0,"side":1};
_[46]={"category":12,"category2":0,"side":1};
_[47]={"category":12,"category2":0,"side":1};
_[48]={"category":40,"category2":0,"side":1};
_[49]={"category":40,"category2":0,"side":1};
_[50]={"category":40,"category2":0,"side":1};
_[51]={"category":40,"category2":0,"side":1};
_[52]={"category":12,"category2":0,"side":1};
_[53]={"category":40,"category2":0,"side":1};
_[54]={"category":12,"category2":0,"side":1};
_[55]={"category":10,"category2":0,"side":1};
_[56]={"category":10,"category2":0,"side":1};

g_quest_catorder=[0,1,8,10,2,3,4,5,6,9,7,-2];

$WowheadProfiler.loadOnDemand('quests',null);

 
 
 */

/*
 var mn_quests=[[,"Continents"],[0,"Eastern Kingdoms",,[[5145,"Abyssal Depths"],[36,"Alterac Mountains"],[45,"Arathi Highlands"],[3,"Badlands"],[4,"Blasted Lands"],[46,"Burning Steppes"],[41,"Deadwind Pass"],[2257,"Deeprun Tram"],[1,"Dun Morogh"],[10,"Duskwood"],[139,"Eastern Plaguelands"],[12,"Elwynn Forest"],[3430,"Eversong Woods"],[3433,"Ghostlands"],[4714,"Gilneas"],[4755,"Gilneas City"],[267,"Hillsbrad Foothills"],[1537,"Ironforge"],[4080,"Isle of Quel'Danas"],[4815,"Kelp'thar Forest"],[38,"Loch Modan"],[33,"Northern Stranglethorn"],[95,"Redridge Canyons"],[44,"Redridge Mountains"],[4706,"Ruins of Gilneas"],[51,"Searing Gorge"],[5144,"Shimmering Expanse"],[3487,"Silvermoon City"],[130,"Silverpine Forest"],[1519,"Stormwind City"],[4411,"Stormwind Harbor"],[8,"Swamp of Sorrows"],[5287,"The Cape of Stranglethorn"],[47,"The Hinterlands"],[85,"Tirisfal Glades"],[4922,"Twilight Highlands"],[1497,"Undercity"],[28,"Western Plaguelands"],[40,"Westfall"],[11,"Wetlands"]]],[1,"Kalimdor",,[[331,"Ashenvale"],[16,"Azshara"],[3524,"Azuremyst Isle"],[3525,"Bloodmyst Isle"],[148,"Darkshore"],[1657,"Darnassus"],[405,"Desolace"],[14,"Durotar"],[15,"Dustwallow Marsh"],[368,"Echo Isles"],[361,"Felwood"],[357,"Feralas"],[5733,"Molten Front"],[493,"Moonglade"],[616,"Mount Hyjal"],[215,"Mulgore"],[17,"Northern Barrens"],[1637,"Orgrimmar"],[989,"Ruins of Uldum"],[1377,"Silithus"],[4709,"Southern Barrens"],[406,"Stonetalon Mountains"],[440,"Tanaris"],[141,"Teldrassil"],[4707,"The Lost Isles"],[400,"Thousand Needles"],[1638,"Thunder Bluff"],[5034,"Uldum"],[490,"Un'Goro Crater"],[618,"Winterspring"]]],[11,"The Maelstrom",,[[5042,"Deepholm"],[4737,"Kezan"],[4720,"The Lost Isles"],[5736,"The Wandering Isle"],[5095,"Tol Barad"],[5389,"Tol Barad Peninsula"]]],[8,"Outland",,[[3522,"Blade's Edge Mountains"],[3483,"Hellfire Peninsula"],[3518,"Nagrand"],[3523,"Netherstorm"],[3520,"Shadowmoon Valley"],[3703,"Shattrath City"],[3679,"Skettis"],[3519,"Terokkar Forest"],[3521,"Zangarmarsh"]]],[10,"Northrend",,[[3537,"Borean Tundra"],[4395,"Dalaran"],[65,"Dragonblight"],[394,"Grizzly Hills"],[495,"Howling Fjord"],[210,"Icecrown"],[3711,"Sholazar Basin"],[67,"The Storm Peaks"],[4197,"Wintergrasp"],[66,"Zul'Drak"]]],[12,"Pandaria",,[[6426,"Brewmoon Festival"],[6138,"Dread Wastes"],[6507,"Isle of Thunder"],[5974,"Jade Temple Grounds"],[6134,"Krasarang Wilds"],[5841,"Kun-Lai Summit"],[6081,"Peak of Serenity"],[6173,"Shado-Pan Monastery"],[5931,"The Arboretum"],[5981,"The Halfhill Market"],[5785,"The Jade Forest"],[6006,"The Veiled Stair"],[6040,"Theramore's Fall (H)"],[6757,"Timeless Isle"],[5842,"Townlong Steppes"],[5840,"Vale of Eternal Blossoms"],[5805,"Valley of the Four Winds"]]],[,"Other"],[6,"Battlegrounds",,[[2597,"Alterac Valley"],[3358,"Arathi Basin"],[3277,"Warsong Gulch"]]],[4,"Classes",,[[-372,"Death Knight"],[-263,"Druid"],[-261,"Hunter"],[-161,"Mage"],[-395,"Monk"],[-141,"Paladin"],[-262,"Priest"],[-162,"Rogue"],[-82,"Shaman"],[-61,"Warlock"],[-81,"Warrior"]]],[2,"Dungeons",,[[4494,"Ahn'kahet: The Old Kingdom"],[3790,"Auchenai Crypts"],[4277,"Azjol-Nerub"],[719,"Blackfathom Deeps"],[4926,"Blackrock Caverns"],[1584,"Blackrock Depths"],[1583,"Blackrock Spire"],[1941,"Caverns of Time"],[2557,"Dire Maul"],[4196,"Drak'Tharon Keep"],[5789,"End Time"],[5976,"Gate of the Setting Sun"],[721,"Gnomeregan"],[4950,"Grim Batol"],[4416,"Gundrak"],[4272,"Halls of Lightning"],[4945,"Halls of Origination"],[4820,"Halls of Reflection"],[4264,"Halls of Stone"],[3562,"Hellfire Ramparts"],[5844,"Hour of Twilight"],[5396,"Lost City of the Tol'vir"],[4131,"Magisters' Terrace"],[3792,"Mana-Tombs"],[2100,"Maraudon"],[6182,"Mogu'shan Palace"],[2367,"Old Hillsbrad Foothills"],[4813,"Pit of Saron"],[2437,"Ragefire Chasm"],[722,"Razorfen Downs"],[491,"Razorfen Kraul"],[6052,"Scarlet Halls"],[6109,"Scarlet Monastery"],[6066,"Scholomance"],[3791,"Sethekk Halls"],[5918,"Shado-Pan Monastery"],[3789,"Shadow Labyrinth"],[209,"Shadowfang Keep"],[6214,"Siege of Niuzao Temple"],[5963,"Stormstout Brewery"],[2017,"Stratholme"],[1477,"Sunken Temple"],[5956,"Temple of the Jade Serpent"],[3848,"The Arcatraz"],[2366,"The Black Morass"],[3713,"The Blood Furnace"],[3847,"The Botanica"],[4100,"The Culling of Stratholme"],[1581,"The Deadmines"],[3845,"The Eye"],[4809,"The Forge of Souls"],[3849,"The Mechanar"],[4120,"The Nexus"],[4228,"The Oculus"],[3714,"The Shattered Halls"],[3717,"The Slave Pens"],[3715,"The Steamvault"],[717,"The Stockade"],[5088,"The Stonecore"],[3716,"The Underbog"],[4415,"The Violet Hold"],[5035,"The Vortex Pinnacle"],[5004,"Throne of the Tides"],[4723,"Trial of the Champion"],[1337,"Uldaman"],[206,"Utgarde Keep"],[1196,"Utgarde Pinnacle"],[718,"Wailing Caverns"],[5788,"Well of Eternity"],[1176,"Zul'Farrak"]]],[5,"Professions",,[[-181,"Alchemy"],[-377,"Archaeology"],[-121,"Blacksmithing"],[-304,"Cooking"],[-201,"Engineering"],[-324,"First Aid"],[-101,"Fishing"],[-24,"Herbalism"],[-371,"Inscription"],[-373,"Jewelcrafting"],[-182,"Leatherworking"],[-264,"Tailoring"]]],[3,"Raids",,[[3959,"Black Temple"],[3606,"Hyjal Summit"],[4812,"Icecrown Citadel"],[3457,"Karazhan"],[3836,"Magtheridon's Lair"],[2717,"Molten Core"],[3456,"Naxxramas"],[3429,"Ruins of Ahn'Qiraj"],[6738,"Siege of Orgrimmar"],[4075,"Sunwell Plateau"],[3842,"Tempest Keep"],[3428,"Temple of Ahn'Qiraj"],[4500,"The Eye of Eternity"],[4493,"The Obsidian Sanctum"],[4987,"The Ruby Sanctum"],[6622,"Throne of Thunder"],[4722,"Trial of the Crusader"],[4273,"Ulduar"],[3805,"Zul'Aman"],[1977,"Zul'Gurub"]]],[9,"World Events",,[[-370,"Brewfest"],[-1002,"Children's Week"],[-364,"Darkmoon Faire"],[-1007,"Day of the Dead"],[-1003,"Hallow's End"],[-1005,"Harvest Festival"],[-1004,"Love is in the Air"],[-366,"Lunar Festival"],[-369,"Midsummer"],[-1006,"New Year's Eve"],[-1008,"Pilgrim's Bounty"],[-374,"Noblegarden"],[-1001,"Winter Veil"]]],[7,"Miscellaneous",,[[-394,"Battle Pets"],[-399,"Brawler's Guild"],[-1010,"Dungeon Finder"],[-381,"Elemental Bonds"],[-1,"Epic"],[-379,"Firelands Invasion"],[-396,"Landfall"],[-344,"Legendary"],[-400,"Proving Grounds"],[-391,"Pandaren Brewmasters"],[-392,"Scenario"],[-380,"The Zandalari"],[-241,"Tournament"]]],[-2,"Uncategorized"]];
 mn_quests=\[(\[\d*,"[\w ']+"(,,\[(\[\d+,"[\w ']+"],?)*])?],?)*];
 */