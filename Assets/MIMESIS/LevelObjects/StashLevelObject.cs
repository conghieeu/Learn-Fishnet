using Mimic;
using Mimic.Actors;
using UnityEngine;

public class StashLevelObject : StaticLevelObject
{
	public string audiokey = "";

	[SerializeField]
	private string putAvailableText;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Lever;

	private void Start()
	{
		base.crossHairType = CrosshairType.Switch;
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (CanStashCurrentInventoryItem(protoActor))
		{
			Hub.s.legacyAudio.Play(audiokey, triggerAudioSource);
			protoActor.PutIntoStash();
			return true;
		}
		return false;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		InventoryItem selectedInventoryItem = protoActor.GetSelectedInventoryItem();
		if (CanStashCurrentInventoryItem(protoActor))
		{
			int realPrice = Hub.s.pdata.main.GetRealPrice(selectedInventoryItem.ItemMasterID, selectedInventoryItem.Price);
			if (realPrice == 0)
			{
				return Hub.GetL10NText("TRAM_STASH_LEVEL_OBJECT_DO_NOT_PUT");
			}
			string text = $"${realPrice,5}";
			return Hub.GetL10NText(putAvailableText).Replace("[itemname:]", Hub.GetL10NText(selectedInventoryItem.MasterInfo.Name)) + text;
		}
		return Hub.GetL10NText("STASH_CAN_PUT_ITEM");
	}

	private bool CanStashCurrentInventoryItem(ProtoActor protoActor)
	{
		InventoryItem selectedInventoryItem = protoActor.GetSelectedInventoryItem();
		if (selectedInventoryItem != null)
		{
			return selectedInventoryItem.Price > -1;
		}
		return false;
	}
}
