using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wowhead
{
	public class Faction
	{
		public string Name { get; set; }

		public int Id { get; set; }

		public Faction() { }

		public Faction(int id, string name)
		{
			Id = id;
			Name = name;
		}

		public static Faction GetById(int id)
		{
			return null;
		}
	}
}
