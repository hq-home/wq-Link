using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wowhead
{
	public class Item
	{
		public string Name { get; set; }

		public int Id { get; set; }

		public Item(int id, string name)
		{
			Id = id;
			Name = name;
		}
        public Item(int id)
        {
            Id = id;

            // TO DO: Load from site/storage
        }
	}
}
