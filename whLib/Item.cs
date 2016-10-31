using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;

namespace Wowhead
{
    public class Item : Entity
	{
		public Item(int id, string name)
            : base(id, name, EntityType.Item)
		{ }

        public Item(int id)
            : base(id, EntityType.Item)
        { }

        protected override void ParseFoundProperties(CaptureCollection capKeys, CaptureCollection capVals, string sid)
        { }

        protected override void ProcessQIKey(string key, int idx, string l, bool force = false)
        {

        }
        protected override void ParseDescription(HtmlDocument htmlDoc)
        {

        }
	}
}
