using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wowhead
{
	public class wObject
	{
		public string Name { get; set; }

		public int Id { get; set; }

		public wObject(int id, string name)
		{
			Id = id;
			Name = name;
		}
        public wObject(int id)
        {
            Id = id;

            // TO DO: Load from site/storage
        }
	}
}
