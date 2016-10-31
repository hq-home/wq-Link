using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wowhead
{
	

	public class wClass : Entity
	{
		public static List<wClass> Classes = new List<wClass>() {
			
			new wClass(1, "Warrior", -81)			// 1 1010 1111	
			, new wClass(2, "Paladin", -141)		// 1 0111 0011	
			, new wClass(3, "Hunter", -261)			// 0 1111 1010
			, new wClass(4, "Rogue", -162)			// 1 0101 1110
			, new wClass(5, "Priest", -262)			// 0 1111 1010
			, new wClass(6, "Death Knight", -372)	// 0 1000 1100	
			, new wClass(7, "Shaman", -82)			// 1 1010 1110	
			, new wClass(8, "Mage", -161)			// 1 0101 1111
			, new wClass(9, "Warlock", -61)			// 1 1100 0011
			, new wClass(10, "Monk", -395)			// 0 0111 0101																																																																																																																																																																																																																																																																																																																		
			, new wClass(11, "Druid", -263)			// 0 1111 1001
			//======================================== |
		};

		public int Flag { get; set; }

		public int Key { get; set; }

		public wClass(int id, string name, int key)
            :base(id, name, EntityType.Class)
		{
			Key = key;
			Flag = 1 << (id - 1);
		}

		public static wClass GetById(int id)
		{
			return Classes.Find(c => c.Id == id);
		}

		public static wClass GetByKey(int k)
		{
			return Classes.Find(c => c.Key == k);
		}

        public static List<wClass> ParseFlags(int flags)
        {
            List<wClass> lc = new List<wClass>();
            foreach(wClass c in Classes)
			{
                if (c.Flag == (c.Flag & flags))
				{
					lc.Add(c);
				}
			}
            return lc;
        }
	}
}
