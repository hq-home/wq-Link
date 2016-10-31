using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wowhead
{
	public class Map : Entity
	{
		public Map(int id, string name)
            : base(id, name, EntityType.Map)
		{ }

        public Map(int id)
            : base(id, EntityType.Map)
        { }
	}
}