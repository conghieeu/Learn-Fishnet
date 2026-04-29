using Bifrost.ItemDrop;

namespace Bifrost.Cooked
{
	public class ItemDropCandidateInfo
	{
		public readonly int ItemMasterID;

		public readonly int Rate;

		public ItemDropCandidateInfo(ItemDrop_candidate data)
		{
			ItemMasterID = data.item_id;
			Rate = data.rate;
		}
	}
}
