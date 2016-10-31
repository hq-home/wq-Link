using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wowhead
{
	public class Npc : Entity
	{
		#region [ Named NPCs ]
		public static Npc NoOne = new Npc(-1, null);
		public static Npc VarianWrynn = new Npc(29611, "King Varian Wrynn <King of Stormwind>");
		public static Npc GarroshHellscream = new Npc(39605, "Garrosh Hellscream <Warchief>");
		public static Npc MagniBronzebeard = new Npc(2784, "King Magni Bronzebeard <Lord of Ironforge>");
		public static Npc TyrandeWhisperwind = new Npc(7999, "Tyrande Whisperwind <High Priestess of Elune>");
		public static Npc SylvanasWindrunner = new Npc(10181, "Lady Sylvanas Windrunner <Banshee Queen>");
		public static Npc BaineBloodhoof = new Npc(36648, "Baine Bloodhoof <High Chieftain>");
		public static Npc Mekkatorque = new Npc(7937, "High Tinker Mekkatorque <King of Gnomes>");
		public static Npc Voljin = new Npc(10540, "Vol'jin");
		public static Npc Gallywix = new Npc(35222, "Trade Prince Gallywix <Leader of the Bilgewater Cartel>");
		public static Npc LorthemarTheron = new Npc(16802, "Lor'themar Theron <Regent Lord of Quel'Thalas>");
		public static Npc Velen = new Npc(17468, "Prophet Velen");
		public static Npc GennGreymane = new Npc(45253, "Genn Greymane");
		public static Npc AysaCloudsinger = new Npc(62419, "Aysa Cloudsinger <Monk Trainer>");
		public static Npc JiFirepaw = new Npc(62445, "Ji Firepaw <Monk Trainer>");
		#endregion

        private int _hasQuests;
        private int _classification;
        private int _maxlevel;
        private int _minlevel;
        private int _type;

		public Npc(int id, string name)
            :base(id, name, EntityType.Npc)
		{ }

        public Npc(int id)
            : base(id, EntityType.Npc)
        { }

        protected override void ParseFoundProperties(CaptureCollection capKeys, CaptureCollection capVals, string sid)
        {
            for (int i = 0; i < capKeys.Count; i++)
            {
                Capture capKey = capKeys[i];
                Capture capVal = capVals[i];

                switch (capKey.Value)
                {
                    case "classification":
                        _classification = Helper.ParseInt(capVal.Value);
                        break;
                    case "hasQuests":
                        _hasQuests = Helper.ParseInt(capVal.Value);
                        break;
                    case "maxlevel":
                        _maxlevel = Helper.ParseInt(capVal.Value);
                        break;
                    case "name":
                        Name = Helper.ParseString(capVal.Value);
                        break;
                    case "minlevel":
                        _minlevel = Helper.ParseInt(capVal.Value);
                        break;
                    case "type":
                        _type = Helper.ParseInt(capVal.Value);
                        //int t = Helper.ParseInt(capVal.Value);
                        //if (!Enum.IsDefined(typeof(QuestType), t))
                        //{
                        //    Helper.LogDebug("Unknown Type [" + (Id == 0 ? sid : Id.ToString()) + "]:" + capVal.Value);
                        //}
                        //else Type = (QuestType)t;

                        break;
                    case "id":
                        if (Id == 0)
                            Id = Helper.ParseInt(capVal.Value);
                        break;

                    case "react":
                    case "location":
                        break;
                    default:
                        Helper.LogDebug("Npc. Unknown property[" + (Id == 0 ? sid : Id.ToString()) + "]:" + capKey.Value);
                        break;
                }
            }
        }

        protected override void ProcessQIKey(string key, int idx, string l, bool force = false)
        {
        
        }

        protected override void ParseDescription(HtmlDocument htmlDoc)
        {
           
        }
    }
}
