using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Wowhead
{
	public enum QuestCategoryEntityType
	{
		None,
		Zone,
		Class,
		Profession,
		Event
	}

	public class QuestCategoryEntity
	{
		public QuestCategoryEntityType Type { get; set; }
		public object Entity { get; set; }

		public wObject Object { get { return Entity as wObject; } }
		public Item Item { get { return Entity as Item; } }
		public Npc Npc { get { return Entity as Npc; } }

		public QuestCategoryEntity(QuestCategoryEntityType type, object entity)
		{
			Type = type;
			Entity = entity;
		}
	}
}
