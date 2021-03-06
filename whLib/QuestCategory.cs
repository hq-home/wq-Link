﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wowhead
{
    public class QuestCategory
    {
        public int? Id { get; set; }
        public string Name { get; set; }
        public QuestCategoryList Categories { get; set; }
    }

    public class QuestCategoryList : List<QuestCategory>
    {
        public QuestCategoryList()
            : base() { }
        public QuestCategoryList(int capacity)
            : base(capacity) { }
        public QuestCategoryList(IEnumerable<QuestCategory> collection)
            : base(collection) { }

        public void Load(bool forceRequest = false)
        {
            string mn_quests = (string)Entity.LoadWoWHeadUrl("http://wowjs.zamimg.com/js/locale_enus.js");

            var sss = jsParseTopLvlArray("mn_quests", mn_quests);

                //"var mn_quests=[[,\"Continents\"],[0,\"Eastern Kingdoms\",,[[5145,\"Abyssal Depths\"],[36,\"Alterac Mountains\"],[45,\"Arathi Highlands\"],[3,\"Badlands\"],[4,\"Blasted Lands\"],[46,\"Burning Steppes\"],[41,\"Deadwind Pass\"],[2257,\"Deeprun Tram\"],[1,\"Dun Morogh\"],[10,\"Duskwood\"],[139,\"Eastern Plaguelands\"],[12,\"Elwynn Forest\"],[3430,\"Eversong Woods\"],[3433,\"Ghostlands\"],[4714,\"Gilneas\"],[4755,\"Gilneas City\"],[267,\"Hillsbrad Foothills\"],[1537,\"Ironforge\"],[4080,\"Isle of Quel\'Danas\"],[4815,\"Kelp\'thar Forest\"],[38,\"Loch Modan\"],[33,\"Northern Stranglethorn\"],[95,\"Redridge Canyons\"],[44,\"Redridge Mountains\"],[4706,\"Ruins of Gilneas\"],[51,\"Searing Gorge\"],[5144,\"Shimmering Expanse\"],[3487,\"Silvermoon City\"],[130,\"Silverpine Forest\"],[1519,\"Stormwind City\"],[4411,\"Stormwind Harbor\"],[8,\"Swamp of Sorrows\"],[5287,\"The Cape of Stranglethorn\"],[47,\"The Hinterlands\"],[85,\"Tirisfal Glades\"],[4922,\"Twilight Highlands\"],[1497,\"Undercity\"],[28,\"Western Plaguelands\"],[40,\"Westfall\"],[11,\"Wetlands\"]]],[1,\"Kalimdor\",,[[331,\"Ashenvale\"],[16,\"Azshara\"],[3524,\"Azuremyst Isle\"],[3525,\"Bloodmyst Isle\"],[148,\"Darkshore\"],[1657,\"Darnassus\"],[405,\"Desolace\"],[14,\"Durotar\"],[15,\"Dustwallow Marsh\"],[368,\"Echo Isles\"],[361,\"Felwood\"],[357,\"Feralas\"],[5733,\"Molten Front\"],[493,\"Moonglade\"],[616,\"Mount Hyjal\"],[215,\"Mulgore\"],[17,\"Northern Barrens\"],[1637,\"Orgrimmar\"],[989,\"Ruins of Uldum\"],[1377,\"Silithus\"],[4709,\"Southern Barrens\"],[406,\"Stonetalon Mountains\"],[440,\"Tanaris\"],[141,\"Teldrassil\"],[4707,\"The Lost Isles\"],[400,\"Thousand Needles\"],[1638,\"Thunder Bluff\"],[5034,\"Uldum\"],[490,\"Un\'Goro Crater\"],[618,\"Winterspring\"]]],[11,\"The Maelstrom\",,[[5042,\"Deepholm\"],[4737,\"Kezan\"],[4720,\"The Lost Isles\"],[5736,\"The Wandering Isle\"],[5095,\"Tol Barad\"],[5389,\"Tol Barad Peninsula\"]]],[8,\"Outland\",,[[3522,\"Blade\'s Edge Mountains\"],[3483,\"Hellfire Peninsula\"],[3518,\"Nagrand\"],[3523,\"Netherstorm\"],[3520,\"Shadowmoon Valley\"],[3703,\"Shattrath City\"],[3679,\"Skettis\"],[3519,\"Terokkar Forest\"],[3521,\"Zangarmarsh\"]]],[10,\"Northrend\",,[[3537,\"Borean Tundra\"],[4395,\"Dalaran\"],[65,\"Dragonblight\"],[394,\"Grizzly Hills\"],[495,\"Howling Fjord\"],[210,\"Icecrown\"],[3711,\"Sholazar Basin\"],[67,\"The Storm Peaks\"],[4197,\"Wintergrasp\"],[66,\"Zul\'Drak\"]]],[12,\"Pandaria\",,[[6426,\"Brewmoon Festival\"],[6138,\"Dread Wastes\"],[6507,\"Isle of Thunder\"],[5974,\"Jade Temple Grounds\"],[6134,\"Krasarang Wilds\"],[5841,\"Kun-Lai Summit\"],[6081,\"Peak of Serenity\"],[6173,\"Shado-Pan Monastery\"],[5931,\"The Arboretum\"],[5981,\"The Halfhill Market\"],[5785,\"The Jade Forest\"],[6006,\"The Veiled Stair\"],[6040,\"Theramore\'s Fall (H)\"],[6757,\"Timeless Isle\"],[5842,\"Townlong Steppes\"],[5840,\"Vale of Eternal Blossoms\"],[5805,\"Valley of the Four Winds\"]]],[,\"Other\"],[6,\"Battlegrounds\",,[[2597,\"Alterac Valley\"],[3358,\"Arathi Basin\"],[3277,\"Warsong Gulch\"]]],[4,\"Classes\",,[[-372,\"Death Knight\"],[-263,\"Druid\"],[-261,\"Hunter\"],[-161,\"Mage\"],[-395,\"Monk\"],[-141,\"Paladin\"],[-262,\"Priest\"],[-162,\"Rogue\"],[-82,\"Shaman\"],[-61,\"Warlock\"],[-81,\"Warrior\"]]],[2,\"Dungeons\",,[[4494,\"Ahn\'kahet: The Old Kingdom\"],[3790,\"Auchenai Crypts\"],[4277,\"Azjol-Nerub\"],[719,\"Blackfathom Deeps\"],[4926,\"Blackrock Caverns\"],[1584,\"Blackrock Depths\"],[1583,\"Blackrock Spire\"],[1941,\"Caverns of Time\"],[2557,\"Dire Maul\"],[4196,\"Drak\'Tharon Keep\"],[5789,\"End Time\"],[5976,\"Gate of the Setting Sun\"],[721,\"Gnomeregan\"],[4950,\"Grim Batol\"],[4416,\"Gundrak\"],[4272,\"Halls of Lightning\"],[4945,\"Halls of Origination\"],[4820,\"Halls of Reflection\"],[4264,\"Halls of Stone\"],[3562,\"Hellfire Ramparts\"],[5844,\"Hour of Twilight\"],[5396,\"Lost City of the Tol\'vir\"],[4131,\"Magisters\' Terrace\"],[3792,\"Mana-Tombs\"],[2100,\"Maraudon\"],[6182,\"Mogu\'shan Palace\"],[2367,\"Old Hillsbrad Foothills\"],[4813,\"Pit of Saron\"],[2437,\"Ragefire Chasm\"],[722,\"Razorfen Downs\"],[491,\"Razorfen Kraul\"],[6052,\"Scarlet Halls\"],[6109,\"Scarlet Monastery\"],[6066,\"Scholomance\"],[3791,\"Sethekk Halls\"],[5918,\"Shado-Pan Monastery\"],[3789,\"Shadow Labyrinth\"],[209,\"Shadowfang Keep\"],[6214,\"Siege of Niuzao Temple\"],[5963,\"Stormstout Brewery\"],[2017,\"Stratholme\"],[1477,\"Sunken Temple\"],[5956,\"Temple of the Jade Serpent\"],[3848,\"The Arcatraz\"],[2366,\"The Black Morass\"],[3713,\"The Blood Furnace\"],[3847,\"The Botanica\"],[4100,\"The Culling of Stratholme\"],[1581,\"The Deadmines\"],[3845,\"The Eye\"],[4809,\"The Forge of Souls\"],[3849,\"The Mechanar\"],[4120,\"The Nexus\"],[4228,\"The Oculus\"],[3714,\"The Shattered Halls\"],[3717,\"The Slave Pens\"],[3715,\"The Steamvault\"],[717,\"The Stockade\"],[5088,\"The Stonecore\"],[3716,\"The Underbog\"],[4415,\"The Violet Hold\"],[5035,\"The Vortex Pinnacle\"],[5004,\"Throne of the Tides\"],[4723,\"Trial of the Champion\"],[1337,\"Uldaman\"],[206,\"Utgarde Keep\"],[1196,\"Utgarde Pinnacle\"],[718,\"Wailing Caverns\"],[5788,\"Well of Eternity\"],[1176,\"Zul\'Farrak\"]]],[5,\"Professions\",,[[-181,\"Alchemy\"],[-377,\"Archaeology\"],[-121,\"Blacksmithing\"],[-304,\"Cooking\"],[-201,\"Engineering\"],[-324,\"First Aid\"],[-101,\"Fishing\"],[-24,\"Herbalism\"],[-371,\"Inscription\"],[-373,\"Jewelcrafting\"],[-182,\"Leatherworking\"],[-264,\"Tailoring\"]]],[3,\"Raids\",,[[3959,\"Black Temple\"],[3606,\"Hyjal Summit\"],[4812,\"Icecrown Citadel\"],[3457,\"Karazhan\"],[3836,\"Magtheridon\'s Lair\"],[2717,\"Molten Core\"],[3456,\"Naxxramas\"],[3429,\"Ruins of Ahn\'Qiraj\"],[6738,\"Siege of Orgrimmar\"],[4075,\"Sunwell Plateau\"],[3842,\"Tempest Keep\"],[3428,\"Temple of Ahn\'Qiraj\"],[4500,\"The Eye of Eternity\"],[4493,\"The Obsidian Sanctum\"],[4987,\"The Ruby Sanctum\"],[6622,\"Throne of Thunder\"],[4722,\"Trial of the Crusader\"],[4273,\"Ulduar\"],[3805,\"Zul\'Aman\"],[1977,\"Zul\'Gurub\"]]],[9,\"World Events\",,[[-370,\"Brewfest\"],[-1002,\"Children\'s Week\"],[-364,\"Darkmoon Faire\"],[-1007,\"Day of the Dead\"],[-1003,\"Hallow\'s End\"],[-1005,\"Harvest Festival\"],[-1004,\"Love is in the Air\"],[-366,\"Lunar Festival\"],[-369,\"Midsummer\"],[-1006,\"New Year\'s Eve\"],[-1008,\"Pilgrim\'s Bounty\"],[-374,\"Noblegarden\"],[-1001,\"Winter Veil\"]]],[7,\"Miscellaneous\",,[[-394,\"Battle Pets\"],[-399,\"Brawler\'s Guild\"],[-1010,\"Dungeon Finder\"],[-381,\"Elemental Bonds\"],[-1,\"Epic\"],[-379,\"Firelands Invasion\"],[-396,\"Landfall\"],[-344,\"Legendary\"],[-400,\"Proving Grounds\"],[-391,\"Pandaren Brewmasters\"],[-392,\"Scenario\"],[-380,\"The Zandalari\"],[-241,\"Tournament\"]]],[-2,\"Uncategorized\"]];";
			Regex myRegex = new Regex(@"mn_quests=\[(\[(?<id>\-?\d*),""(?<name>[\w '\-\():]+)"",(?<link>""[^""]*"")?,?(\[(?:\[(?<id2>\-?\d*),""(?<name2>[\w '\-\():]+)"",(?<link2>""[^""]*"")?],?)+])?(,\{(?:[\w]+\:\-?\d,?})*)?],?)*]", RegexOptions.Compiled);
			// \[(?<id>\-?\d*),""(?<name>[\w '\-\():]+)"",(?<link>""[^""]*"")?,?(\[(?:\[(?<id2>\-?\d*),""(?<name2>[\w '\-\():]+)"",(?<link2>""[^""]*"")?],?)+])?(,\{(?:[\w]+\:1,?})*)?]
			// \[(\[(?<id>\-?\d*),""(?<name>[\w '\-\():]+)"",(?<link>""[^""]*"")?,?(\[(?:\[(?<id2>\-?\d*),""(?<name2>[\w '\-\():]+)"",(?<link2>""[^""]*"")?],?)+])?(,\{(?:[\w]+\:\-?\d,?})*)?],?)*]
            MatchCollection mc = myRegex.Matches(mn_quests);

            if (mc == null || mc.Count == 0 || !mc[0].Success)
                Helper.LogDebug("Can't find valid mn_quests in locale_enus.js");

            ParseItems(mc[0].Value);
        }

        private void ParseItems(string items)
        {
			//Regex myRegex = new Regex(@"\[(\[(?<id>\-?\d*),""(?<name>[\w '\-\():]+)"",(?<link>""[^""]*"")?,?(?<sub>\[(?:\[(?<id2>\-?\d*),""(?<name2>[\w '\-\():]+)"",(?<link2>""[^""]*"")?],?)+])?(,\{(?:[\w]+\:\-?\d,?})*)?],?)*]", 
			Regex myRegex = new Regex(@"\[(?<id>\-?\d*),""(?<name>[\w '\-\():]+)"",(?<link>""[^""]*"")?,?(?<sub>\[(?:\[(?<id2>\-?\d*),""(?<name2>[\w '\-\():]+)"",(?<link2>""[^""]*"")?],?)+])?(,\{(?:[\w]+\:\-?\d,?})*)?]", 
				RegexOptions.Compiled | RegexOptions.ExplicitCapture);
            MatchCollection mc = myRegex.Matches(items);
            //MatchCollection mc2 = null;

            if (mc == null || mc.Count == 0 || !mc[0].Success)
                Helper.LogDebug("Can't parse locale_enus.js: " + items);
            else
            {
				for (int i = 0; i < mc.Count; i++)
				{
					Group gId = mc[i].Groups["id"];
					Group gName = mc[i].Groups["name"];
					Group gSub = mc[i].Groups["sub"];

					//for (int j = 0; j < gId.Captures.Count; j++)
					//{
					QuestCategory qcat = new QuestCategory() {Name = gName.Captures[0].Value};
					this.Add(qcat);

					if (!string.IsNullOrEmpty(gId.Captures[0].Value))
						qcat.Id = int.Parse(gId.Captures[0].Value);

					if (gSub.Captures.Count > 0 && !string.IsNullOrEmpty(gSub.Captures[0].Value))
					{
						qcat.Categories = new QuestCategoryList();

						qcat.Categories.ParseItems(gSub.Captures[0].Value);
					}
					//}
				}
            }
        }

        private List<string> jsParseTopLvlArray(string name, string jsText)
        {
            List<string> list = new List<string>();
            string strArr = "[";
            char b = (char)0;
            bool esc = false;
            int bCount = 1, pos;
            int bLen = 2 + name.Length; // name.Length + "=[".Length
            int idxSt = jsText.IndexOf(name + "=");

            //"var mn_quests=[[,\"Continents\"],[0,\"Eastern Kingdoms\",,[[5145,\"Abyssal Depths\"],[36,\"Alterac Mountains\"],[45,\"Arathi Highlands\"],[3,\"Badlands\"],[4,\"Blasted Lands\"],[46,\"Burning Steppes\"],[41,\"Deadwind Pass\"],[2257,\"Deeprun Tram\"],[1,\"Dun Morogh\"],[10,\"Duskwood\"],[139,\"Eastern Plaguelands\"],[12,\"Elwynn Forest\"],[3430,\"Eversong Woods\"],[3433,\"Ghostlands\"],[4714,\"Gilneas\"],[4755,\"Gilneas City\"],[267,\"Hillsbrad Foothills\"],[1537,\"Ironforge\"],[4080,\"Isle of Quel\'Danas\"],[4815,\"Kelp\'thar Forest\"],[38,\"Loch Modan\"],[33,\"Northern Stranglethorn\"],[95,\"Redridge Canyons\"],[44,\"Redridge Mountains\"],[4706,\"Ruins of Gilneas\"],[51,\"Searing Gorge\"],[5144,\"Shimmering Expanse\"],[3487,\"Silvermoon City\"],[130,\"Silverpine Forest\"],[1519,\"Stormwind City\"],[4411,\"Stormwind Harbor\"],[8,\"Swamp of Sorrows\"],[5287,\"The Cape of Stranglethorn\"],[47,\"The Hinterlands\"],[85,\"Tirisfal Glades\"],[4922,\"Twilight Highlands\"],[1497,\"Undercity\"],[28,\"Western Plaguelands\"],[40,\"Westfall\"],[11,\"Wetlands\"]]],[1,\"Kalimdor\",,[[331,\"Ashenvale\"],[16,\"Azshara\"],[3524,\"Azuremyst Isle\"],[3525,\"Bloodmyst Isle\"],[148,\"Darkshore\"],[1657,\"Darnassus\"],[405,\"Desolace\"],[14,\"Durotar\"],[15,\"Dustwallow Marsh\"],[368,\"Echo Isles\"],[361,\"Felwood\"],[357,\"Feralas\"],[5733,\"Molten Front\"],[493,\"Moonglade\"],[616,\"Mount Hyjal\"],[215,\"Mulgore\"],[17,\"Northern Barrens\"],[1637,\"Orgrimmar\"],[989,\"Ruins of Uldum\"],[1377,\"Silithus\"],[4709,\"Southern Barrens\"],[406,\"Stonetalon Mountains\"],[440,\"Tanaris\"],[141,\"Teldrassil\"],[4707,\"The Lost Isles\"],[400,\"Thousand Needles\"],[1638,\"Thunder Bluff\"],[5034,\"Uldum\"],[490,\"Un\'Goro Crater\"],[618,\"Winterspring\"]]],[11,\"The Maelstrom\",,[[5042,\"Deepholm\"],[4737,\"Kezan\"],[4720,\"The Lost Isles\"],[5736,\"The Wandering Isle\"],[5095,\"Tol Barad\"],[5389,\"Tol Barad Peninsula\"]]],[8,\"Outland\",,[[3522,\"Blade\'s Edge Mountains\"],[3483,\"Hellfire Peninsula\"],[3518,\"Nagrand\"],[3523,\"Netherstorm\"],[3520,\"Shadowmoon Valley\"],[3703,\"Shattrath City\"],[3679,\"Skettis\"],[3519,\"Terokkar Forest\"],[3521,\"Zangarmarsh\"]]],[10,\"Northrend\",,[[3537,\"Borean Tundra\"],[4395,\"Dalaran\"],[65,\"Dragonblight\"],[394,\"Grizzly Hills\"],[495,\"Howling Fjord\"],[210,\"Icecrown\"],[3711,\"Sholazar Basin\"],[67,\"The Storm Peaks\"],[4197,\"Wintergrasp\"],[66,\"Zul\'Drak\"]]],[12,\"Pandaria\",,[[6426,\"Brewmoon Festival\"],[6138,\"Dread Wastes\"],[6507,\"Isle of Thunder\"],[5974,\"Jade Temple Grounds\"],[6134,\"Krasarang Wilds\"],[5841,\"Kun-Lai Summit\"],[6081,\"Peak of Serenity\"],[6173,\"Shado-Pan Monastery\"],[5931,\"The Arboretum\"],[5981,\"The Halfhill Market\"],[5785,\"The Jade Forest\"],[6006,\"The Veiled Stair\"],[6040,\"Theramore\'s Fall (H)\"],[6757,\"Timeless Isle\"],[5842,\"Townlong Steppes\"],[5840,\"Vale of Eternal Blossoms\"],[5805,\"Valley of the Four Winds\"]]],[,\"Other\"],[6,\"Battlegrounds\",,[[2597,\"Alterac Valley\"],[3358,\"Arathi Basin\"],[3277,\"Warsong Gulch\"]]],[4,\"Classes\",,[[-372,\"Death Knight\"],[-263,\"Druid\"],[-261,\"Hunter\"],[-161,\"Mage\"],[-395,\"Monk\"],[-141,\"Paladin\"],[-262,\"Priest\"],[-162,\"Rogue\"],[-82,\"Shaman\"],[-61,\"Warlock\"],[-81,\"Warrior\"]]],[2,\"Dungeons\",,[[4494,\"Ahn\'kahet: The Old Kingdom\"],[3790,\"Auchenai Crypts\"],[4277,\"Azjol-Nerub\"],[719,\"Blackfathom Deeps\"],[4926,\"Blackrock Caverns\"],[1584,\"Blackrock Depths\"],[1583,\"Blackrock Spire\"],[1941,\"Caverns of Time\"],[2557,\"Dire Maul\"],[4196,\"Drak\'Tharon Keep\"],[5789,\"End Time\"],[5976,\"Gate of the Setting Sun\"],[721,\"Gnomeregan\"],[4950,\"Grim Batol\"],[4416,\"Gundrak\"],[4272,\"Halls of Lightning\"],[4945,\"Halls of Origination\"],[4820,\"Halls of Reflection\"],[4264,\"Halls of Stone\"],[3562,\"Hellfire Ramparts\"],[5844,\"Hour of Twilight\"],[5396,\"Lost City of the Tol\'vir\"],[4131,\"Magisters\' Terrace\"],[3792,\"Mana-Tombs\"],[2100,\"Maraudon\"],[6182,\"Mogu\'shan Palace\"],[2367,\"Old Hillsbrad Foothills\"],[4813,\"Pit of Saron\"],[2437,\"Ragefire Chasm\"],[722,\"Razorfen Downs\"],[491,\"Razorfen Kraul\"],[6052,\"Scarlet Halls\"],[6109,\"Scarlet Monastery\"],[6066,\"Scholomance\"],[3791,\"Sethekk Halls\"],[5918,\"Shado-Pan Monastery\"],[3789,\"Shadow Labyrinth\"],[209,\"Shadowfang Keep\"],[6214,\"Siege of Niuzao Temple\"],[5963,\"Stormstout Brewery\"],[2017,\"Stratholme\"],[1477,\"Sunken Temple\"],[5956,\"Temple of the Jade Serpent\"],[3848,\"The Arcatraz\"],[2366,\"The Black Morass\"],[3713,\"The Blood Furnace\"],[3847,\"The Botanica\"],[4100,\"The Culling of Stratholme\"],[1581,\"The Deadmines\"],[3845,\"The Eye\"],[4809,\"The Forge of Souls\"],[3849,\"The Mechanar\"],[4120,\"The Nexus\"],[4228,\"The Oculus\"],[3714,\"The Shattered Halls\"],[3717,\"The Slave Pens\"],[3715,\"The Steamvault\"],[717,\"The Stockade\"],[5088,\"The Stonecore\"],[3716,\"The Underbog\"],[4415,\"The Violet Hold\"],[5035,\"The Vortex Pinnacle\"],[5004,\"Throne of the Tides\"],[4723,\"Trial of the Champion\"],[1337,\"Uldaman\"],[206,\"Utgarde Keep\"],[1196,\"Utgarde Pinnacle\"],[718,\"Wailing Caverns\"],[5788,\"Well of Eternity\"],[1176,\"Zul\'Farrak\"]]],[5,\"Professions\",,[[-181,\"Alchemy\"],[-377,\"Archaeology\"],[-121,\"Blacksmithing\"],[-304,\"Cooking\"],[-201,\"Engineering\"],[-324,\"First Aid\"],[-101,\"Fishing\"],[-24,\"Herbalism\"],[-371,\"Inscription\"],[-373,\"Jewelcrafting\"],[-182,\"Leatherworking\"],[-264,\"Tailoring\"]]],[3,\"Raids\",,[[3959,\"Black Temple\"],[3606,\"Hyjal Summit\"],[4812,\"Icecrown Citadel\"],[3457,\"Karazhan\"],[3836,\"Magtheridon\'s Lair\"],[2717,\"Molten Core\"],[3456,\"Naxxramas\"],[3429,\"Ruins of Ahn\'Qiraj\"],[6738,\"Siege of Orgrimmar\"],[4075,\"Sunwell Plateau\"],[3842,\"Tempest Keep\"],[3428,\"Temple of Ahn\'Qiraj\"],[4500,\"The Eye of Eternity\"],[4493,\"The Obsidian Sanctum\"],[4987,\"The Ruby Sanctum\"],[6622,\"Throne of Thunder\"],[4722,\"Trial of the Crusader\"],[4273,\"Ulduar\"],[3805,\"Zul\'Aman\"],[1977,\"Zul\'Gurub\"]]],[9,\"World Events\",,[[-370,\"Brewfest\"],[-1002,\"Children\'s Week\"],[-364,\"Darkmoon Faire\"],[-1007,\"Day of the Dead\"],[-1003,\"Hallow\'s End\"],[-1005,\"Harvest Festival\"],[-1004,\"Love is in the Air\"],[-366,\"Lunar Festival\"],[-369,\"Midsummer\"],[-1006,\"New Year\'s Eve\"],[-1008,\"Pilgrim\'s Bounty\"],[-374,\"Noblegarden\"],[-1001,\"Winter Veil\"]]],[7,\"Miscellaneous\",,[[-394,\"Battle Pets\"],[-399,\"Brawler\'s Guild\"],[-1010,\"Dungeon Finder\"],[-381,\"Elemental Bonds\"],[-1,\"Epic\"],[-379,\"Firelands Invasion\"],[-396,\"Landfall\"],[-344,\"Legendary\"],[-400,\"Proving Grounds\"],[-391,\"Pandaren Brewmasters\"],[-392,\"Scenario\"],[-380,\"The Zandalari\"],[-241,\"Tournament\"]]],[-2,\"Uncategorized\"]];";

            if (idxSt > -1) {
                idxSt += bLen; pos = idxSt;
                char c;

                while (bCount > 0 && pos < jsText.Length) {
                    c = jsText[pos++];
                    switch (c) {
                        case '\'':
                        case '"':
                            if (esc) { esc = false; }
                            else if (b==c) {
                                b = (char)0;
                            }
                            else if (b == 0) {
                                b = c;
                            }
                            break;
                        case '\\':
                            if (b != 0) {
                                esc = true;
                            }
                            break;
                        case '[':
                            if (b == 0) {
                                bCount++;
                            }
                            break;
                        case ']':
                            if (b == 0)
                            {
                                bCount--;

                                if(bCount == 1){
                                    var s = jsText.Substring(idxSt, pos - idxSt);
                                    list.Add(s);
                                    idxSt = pos;
                                }
                            }
                            break;
                        case ',':
                            if (bCount == 1 && b == 0)
                            {
                                idxSt = pos;
                            }
                            break;
                    }
                }
            }

            strArr+="]";
            return list;
        }

    }
    /*
     var mn_quests=[[,"Continents"],[0,"Eastern Kingdoms",,[[5145,"Abyssal Depths"],[36,"Alterac Mountains"],[45,"Arathi Highlands"],[3,"Badlands"],[4,"Blasted Lands"],[46,"Burning Steppes"],[41,"Deadwind Pass"],[2257,"Deeprun Tram"],[1,"Dun Morogh"],[10,"Duskwood"],[139,"Eastern Plaguelands"],[12,"Elwynn Forest"],[3430,"Eversong Woods"],[3433,"Ghostlands"],[4714,"Gilneas"],[4755,"Gilneas City"],[267,"Hillsbrad Foothills"],[1537,"Ironforge"],[4080,"Isle of Quel'Danas"],[4815,"Kelp'thar Forest"],[38,"Loch Modan"],[33,"Northern Stranglethorn"],[95,"Redridge Canyons"],[44,"Redridge Mountains"],[4706,"Ruins of Gilneas"],[51,"Searing Gorge"],[5144,"Shimmering Expanse"],[3487,"Silvermoon City"],[130,"Silverpine Forest"],[1519,"Stormwind City"],[4411,"Stormwind Harbor"],[8,"Swamp of Sorrows"],[5287,"The Cape of Stranglethorn"],[47,"The Hinterlands"],[85,"Tirisfal Glades"],[4922,"Twilight Highlands"],[1497,"Undercity"],[28,"Western Plaguelands"],[40,"Westfall"],[11,"Wetlands"]]],[1,"Kalimdor",,[[331,"Ashenvale"],[16,"Azshara"],[3524,"Azuremyst Isle"],[3525,"Bloodmyst Isle"],[148,"Darkshore"],[1657,"Darnassus"],[405,"Desolace"],[14,"Durotar"],[15,"Dustwallow Marsh"],[368,"Echo Isles"],[361,"Felwood"],[357,"Feralas"],[5733,"Molten Front"],[493,"Moonglade"],[616,"Mount Hyjal"],[215,"Mulgore"],[17,"Northern Barrens"],[1637,"Orgrimmar"],[989,"Ruins of Uldum"],[1377,"Silithus"],[4709,"Southern Barrens"],[406,"Stonetalon Mountains"],[440,"Tanaris"],[141,"Teldrassil"],[4707,"The Lost Isles"],[400,"Thousand Needles"],[1638,"Thunder Bluff"],[5034,"Uldum"],[490,"Un'Goro Crater"],[618,"Winterspring"]]],[11,"The Maelstrom",,[[5042,"Deepholm"],[4737,"Kezan"],[4720,"The Lost Isles"],[5736,"The Wandering Isle"],[5095,"Tol Barad"],[5389,"Tol Barad Peninsula"]]],[8,"Outland",,[[3522,"Blade's Edge Mountains"],[3483,"Hellfire Peninsula"],[3518,"Nagrand"],[3523,"Netherstorm"],[3520,"Shadowmoon Valley"],[3703,"Shattrath City"],[3679,"Skettis"],[3519,"Terokkar Forest"],[3521,"Zangarmarsh"]]],[10,"Northrend",,[[3537,"Borean Tundra"],[4395,"Dalaran"],[65,"Dragonblight"],[394,"Grizzly Hills"],[495,"Howling Fjord"],[210,"Icecrown"],[3711,"Sholazar Basin"],[67,"The Storm Peaks"],[4197,"Wintergrasp"],[66,"Zul'Drak"]]],[12,"Pandaria",,[[6426,"Brewmoon Festival"],[6138,"Dread Wastes"],[6507,"Isle of Thunder"],[5974,"Jade Temple Grounds"],[6134,"Krasarang Wilds"],[5841,"Kun-Lai Summit"],[6081,"Peak of Serenity"],[6173,"Shado-Pan Monastery"],[5931,"The Arboretum"],[5981,"The Halfhill Market"],[5785,"The Jade Forest"],[6006,"The Veiled Stair"],[6040,"Theramore's Fall (H)"],[6757,"Timeless Isle"],[5842,"Townlong Steppes"],[5840,"Vale of Eternal Blossoms"],[5805,"Valley of the Four Winds"]]],[,"Other"],[6,"Battlegrounds",,[[2597,"Alterac Valley"],[3358,"Arathi Basin"],[3277,"Warsong Gulch"]]],[4,"Classes",,[[-372,"Death Knight"],[-263,"Druid"],[-261,"Hunter"],[-161,"Mage"],[-395,"Monk"],[-141,"Paladin"],[-262,"Priest"],[-162,"Rogue"],[-82,"Shaman"],[-61,"Warlock"],[-81,"Warrior"]]],[2,"Dungeons",,[[4494,"Ahn'kahet: The Old Kingdom"],[3790,"Auchenai Crypts"],[4277,"Azjol-Nerub"],[719,"Blackfathom Deeps"],[4926,"Blackrock Caverns"],[1584,"Blackrock Depths"],[1583,"Blackrock Spire"],[1941,"Caverns of Time"],[2557,"Dire Maul"],[4196,"Drak'Tharon Keep"],[5789,"End Time"],[5976,"Gate of the Setting Sun"],[721,"Gnomeregan"],[4950,"Grim Batol"],[4416,"Gundrak"],[4272,"Halls of Lightning"],[4945,"Halls of Origination"],[4820,"Halls of Reflection"],[4264,"Halls of Stone"],[3562,"Hellfire Ramparts"],[5844,"Hour of Twilight"],[5396,"Lost City of the Tol'vir"],[4131,"Magisters' Terrace"],[3792,"Mana-Tombs"],[2100,"Maraudon"],[6182,"Mogu'shan Palace"],[2367,"Old Hillsbrad Foothills"],[4813,"Pit of Saron"],[2437,"Ragefire Chasm"],[722,"Razorfen Downs"],[491,"Razorfen Kraul"],[6052,"Scarlet Halls"],[6109,"Scarlet Monastery"],[6066,"Scholomance"],[3791,"Sethekk Halls"],[5918,"Shado-Pan Monastery"],[3789,"Shadow Labyrinth"],[209,"Shadowfang Keep"],[6214,"Siege of Niuzao Temple"],[5963,"Stormstout Brewery"],[2017,"Stratholme"],[1477,"Sunken Temple"],[5956,"Temple of the Jade Serpent"],[3848,"The Arcatraz"],[2366,"The Black Morass"],[3713,"The Blood Furnace"],[3847,"The Botanica"],[4100,"The Culling of Stratholme"],[1581,"The Deadmines"],[3845,"The Eye"],[4809,"The Forge of Souls"],[3849,"The Mechanar"],[4120,"The Nexus"],[4228,"The Oculus"],[3714,"The Shattered Halls"],[3717,"The Slave Pens"],[3715,"The Steamvault"],[717,"The Stockade"],[5088,"The Stonecore"],[3716,"The Underbog"],[4415,"The Violet Hold"],[5035,"The Vortex Pinnacle"],[5004,"Throne of the Tides"],[4723,"Trial of the Champion"],[1337,"Uldaman"],[206,"Utgarde Keep"],[1196,"Utgarde Pinnacle"],[718,"Wailing Caverns"],[5788,"Well of Eternity"],[1176,"Zul'Farrak"]]],[5,"Professions",,[[-181,"Alchemy"],[-377,"Archaeology"],[-121,"Blacksmithing"],[-304,"Cooking"],[-201,"Engineering"],[-324,"First Aid"],[-101,"Fishing"],[-24,"Herbalism"],[-371,"Inscription"],[-373,"Jewelcrafting"],[-182,"Leatherworking"],[-264,"Tailoring"]]],[3,"Raids",,[[3959,"Black Temple"],[3606,"Hyjal Summit"],[4812,"Icecrown Citadel"],[3457,"Karazhan"],[3836,"Magtheridon's Lair"],[2717,"Molten Core"],[3456,"Naxxramas"],[3429,"Ruins of Ahn'Qiraj"],[6738,"Siege of Orgrimmar"],[4075,"Sunwell Plateau"],[3842,"Tempest Keep"],[3428,"Temple of Ahn'Qiraj"],[4500,"The Eye of Eternity"],[4493,"The Obsidian Sanctum"],[4987,"The Ruby Sanctum"],[6622,"Throne of Thunder"],[4722,"Trial of the Crusader"],[4273,"Ulduar"],[3805,"Zul'Aman"],[1977,"Zul'Gurub"]]],[9,"World Events",,[[-370,"Brewfest"],[-1002,"Children's Week"],[-364,"Darkmoon Faire"],[-1007,"Day of the Dead"],[-1003,"Hallow's End"],[-1005,"Harvest Festival"],[-1004,"Love is in the Air"],[-366,"Lunar Festival"],[-369,"Midsummer"],[-1006,"New Year's Eve"],[-1008,"Pilgrim's Bounty"],[-374,"Noblegarden"],[-1001,"Winter Veil"]]],[7,"Miscellaneous",,[[-394,"Battle Pets"],[-399,"Brawler's Guild"],[-1010,"Dungeon Finder"],[-381,"Elemental Bonds"],[-1,"Epic"],[-379,"Firelands Invasion"],[-396,"Landfall"],[-344,"Legendary"],[-400,"Proving Grounds"],[-391,"Pandaren Brewmasters"],[-392,"Scenario"],[-380,"The Zandalari"],[-241,"Tournament"]]],[-2,"Uncategorized"]];
     mn_quests=\[(\[\d*,"[\w ']+"(,,\[(\[\d+,"[\w ']+"],?)*])?],?)*];
     mn_quests=\[(\[\-?\d*,"[\w '\-\():]+"(,,\[(\[\-?\d+,"[\w '\-\():]+"],?)*])?],?)*];
     */
}
