using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wowhead
{
	public class Npc
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

		public string Name { get; set; }

		public int Id { get; set; }

		public Npc(int id, string name)
		{
			Id = id;
			Name = name;
		}

        public Npc(int id)
        {
            Id = id;

            // TO DO: Load from site/storage
        }


	}
}
