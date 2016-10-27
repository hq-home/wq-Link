using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wowhead
{
	public class Map
	{
		public string Name { get; set; }

		public int Id { get; set; }

		public Map(int id, string name)
		{
			Id = id;
			Name = name;
		}
	}
}