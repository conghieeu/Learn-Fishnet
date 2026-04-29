using Mimic;
using Mimic.Actors;
using UnityEngine;

public class CrowShopLevelObject : SwitchLevelObject
{
	[SerializeField]
	public int crowShopId;

	[SerializeField]
	public string avaliableText;

	[SerializeField]
	public string unavaliableText;

	[SerializeField]
	public string emptyHandText;

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		InventoryItem selectedInventoryItem = protoActor.GetSelectedInventoryItem();
		if (selectedInventoryItem != null && selectedInventoryItem.MasterInfo.IsVendingMachineExchange)
		{
			return Time.time - lastTransitionTime >= minTransitionInterval;
		}
		return false;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		protoActor.BarterItem(OnBarterItemSuccess);
	}

	private void OnBarterItemSuccess(bool success)
	{
		if (success)
		{
			PlayTriggerSound(base.State, base.State);
			AnimateObject(base.State, base.State);
		}
		else
		{
			ctsForTriggerAction?.Cancel();
			ctsForTriggerAction?.Dispose();
			ctsForTriggerAction = null;
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		InventoryItem selectedInventoryItem = protoActor.GetSelectedInventoryItem();
		if (selectedInventoryItem != null)
		{
			if (selectedInventoryItem.MasterInfo.IsVendingMachineExchange)
			{
				return Hub.GetL10NText(avaliableText);
			}
			return Hub.GetL10NText(unavaliableText);
		}
		return Hub.GetL10NText(emptyHandText);
	}
}
