using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Wowhead
{
    public class Zone : Entity
	{
		public Zone(int id, string name)
            : base(id, name, EntityType.Zone)
		{ }

        public Zone(int id)
            : base(id, EntityType.Zone)
        { }

		public static Zone GetById(int id)
		{
			return null;
		}
	}
}