using System.Collections.Generic;

namespace Wowhead
{
	public class Race
	{

		/*
		 Race flag = 2^(RaceId - 1) 
		 
		{"classes":959,"faction":72,"id":1,"leader":29611,"name":"Human","side":1,"zone":12},
		 * {"classes":1005,"faction":76,"id":2,"leader":39605,"name":"Orc","side":2,"zone":14},
		 * {"classes":1023,"faction":47,"id":3,"leader":2784,"name":"Dwarf","side":1,"zone":1},
		 * {"classes":1725,"faction":69,"id":4,"leader":7999,"name":"Night Elf","side":1,"zone":141},
		 * {"classes":957,"faction":68,"id":5,"leader":10181,"name":"Undead","side":2,"zone":85},
		 * {"classes":1655,"faction":81,"id":6,"leader":36648,"name":"Tauren","side":2,"zone":215},
		 * {"classes":953,"faction":54,"id":7,"leader":7937,"name":"Gnome","side":1,"zone":1},
		 * {"classes":2045,"faction":530,"id":8,"leader":10540,"name":"Troll","side":2,"zone":14},
		 * {"classes":509,"expansion":3,"faction":1133,"id":9,"leader":35222,"name":"Goblin","side":2,"zone":4737},
		 * {"classes":959,"expansion":1,"faction":911,"id":10,"leader":16802,"name":"Blood Elf","side":2,"zone":3430},
		 * {"classes":759,"expansion":1,"faction":930,"id":11,"leader":17468,"name":"Draenei","side":1,"zone":3524},
		 * {"classes":1469,"expansion":3,"faction":1134,"id":22,"leader":45253,"name":"Worgen","side":1,"zone":4714},
		 * {"classes":733,"expansion":4,"faction":1216,"id":24,"name":"Pandaren","zone":5736},
		 * {"classes":733,"expansion":4,"faction":1216,"id":25,"leader":62419,"name":"Pandaren","side":1,"zone":5736},
		 * {"classes":733,"expansion":4,"faction":1216,"id":26,"leader":62445,"name":"Pandaren","side":2,"zone":5736}]
		 */

		public static List<Race> Races = new List<Race>()
			{
				new Race(1, "Human", Npc.VarianWrynn, 12, 72, 959, Side.Alliance)
				,new Race(2, "Orc", Npc.GarroshHellscream, 14, 76, 1005, Side.Horde)
				,new Race(3, "Dwarf", Npc.MagniBronzebeard, 1, 47, 1023, Side.Alliance)
				,new Race(4, "Night Elf", Npc.TyrandeWhisperwind, 141, 69, 1725, Side.Alliance)
				,new Race(5, "Undead", Npc.SylvanasWindrunner, 85, 68, 957, Side.Horde)
				,new Race(6, "Tauren", Npc.BaineBloodhoof, 215, 81, 1655, Side.Horde)
				,new Race(7, "Gnome", Npc.Mekkatorque, 1, 54, 953, Side.Alliance)
				,new Race(8, "Troll", Npc.Voljin, 14, 530, 2045, Side.Horde)
				,new Race(9, "Goblin", Npc.Gallywix, 4737, 1133, 509, Side.Horde, ExpansionType.Cataclysm)
				,new Race(10, "Blood Elf", Npc.LorthemarTheron, 3430, 911, 959, Side.Horde, ExpansionType.BurningCrusade)
				,new Race(11, "Draenei", Npc.Velen, 3524, 930, 759, Side.Alliance, ExpansionType.BurningCrusade)
				,new Race(22, "Worgen", Npc.GennGreymane, 4714, 1134, 1469, Side.Alliance, ExpansionType.Cataclysm)
				,new Race(24, "Pandaren", Npc.NoOne, 5736, 1216, 733, Side.Neutral, ExpansionType.MistsOfPandaria)
				,new Race(25, "Pandaren", Npc.AysaCloudsinger, 5736, 1216, 733, Side.Alliance, ExpansionType.MistsOfPandaria)
				,new Race(26, "Pandaren", Npc.JiFirepaw, 5736, 1216, 733, Side.Horde, ExpansionType.MistsOfPandaria)
			};

		public string Name { get; set; }

		public int Id { get; set; }

		public int Flag { get; set; }

		public Side Side { get; set; }

		public Faction Faction { get; set; }

		public Zone StartZone { get; set; }

		public Npc Leader { get; set; }

		public ExpansionType Expansion { get; set; }

		public List<wClass> AvaClasses;

		public Race(int id, string name, Npc leader, int zoneId, int factionId, int classFlags, Side side = Side.Neutral, ExpansionType expansion = ExpansionType.Classic)
		{
			Id = id;
			Name = name;
			Flag = 1 << (id - 1);
			Side = side;
			Leader = leader;
			StartZone = Zone.GetById(zoneId);
			Faction = Faction.GetById(factionId);
			Expansion = expansion;

            AvaClasses = wClass.ParseFlags(classFlags);
		}

		public static Race GetById(int id)
		{
			return Races.Find(r => r.Id == id);
		}

        public static List<Race> ParseFlags(int flags)
        {
            List<Race> lr = new List<Race>();
            foreach(Race r in Races)
			{
                if (r.Flag == (r.Flag & flags))
				{
					lr.Add(r);
				}
			}
            return lr;
        }
	}
}
