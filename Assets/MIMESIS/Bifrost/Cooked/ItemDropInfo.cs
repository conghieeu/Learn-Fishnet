using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using Bifrost.ItemDrop;

namespace Bifrost.Cooked
{
	public class ItemDropInfo
	{
		public readonly int MasterID;

		public readonly int DropCountMin;

		public readonly int DropCountMax;

		public readonly ImmutableArray<ItemDropCandidateInfo> ItemDropCandidates;

		public ItemDropInfo(ItemDrop_MasterData data)
		{
			MasterID = data.id;
			DropCountMin = data.drop_count_min;
			DropCountMax = data.drop_count_max;
			ImmutableArray<ItemDropCandidateInfo>.Builder builder = ImmutableArray.CreateBuilder<ItemDropCandidateInfo>();
			foreach (ItemDrop_candidate item in data.ItemDrop_candidateval)
			{
				builder.Add(new ItemDropCandidateInfo(item));
			}
			ItemDropCandidates = builder.ToImmutable();
		}

		public List<int> GetDropItemList()
		{
			int num = SimpleRandUtil.Next(DropCountMin, DropCountMax + 1);
			List<int> list = new List<int>();
			for (int i = 0; i < num; i++)
			{
				int num2 = ItemDropCandidates.Sum((ItemDropCandidateInfo candidate) => candidate.Rate);
				int num3 = SimpleRandUtil.Next(0, num2 + 1);
				int num4 = 0;
				foreach (ItemDropCandidateInfo item in ItemDropCandidates.OrderByDescending((ItemDropCandidateInfo c) => c.Rate))
				{
					num4 += item.Rate;
					if (num3 <= num4)
					{
						list.Add(item.ItemMasterID);
						break;
					}
				}
			}
			return list;
		}
	}
}
