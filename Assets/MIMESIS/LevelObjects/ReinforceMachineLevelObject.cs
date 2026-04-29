using Mimic;
using Mimic.Actors;
using MoreMountains.Feedbacks;
using UnityEngine;

public class ReinforceMachineLevelObject : SwitchLevelObject
{
	[Header("Reinforce Machine Level Object")]
	[SerializeField]
	private MMF_Player feedbackEffect;

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		InventoryItem handheldItem = protoActor.GetHandheldItem();
		if (handheldItem != null && handheldItem.IsReinforceable && handheldItem.ReinforceCost <= Hub.s.pdata.main.CurrentCurrency)
		{
			return true;
		}
		return false;
	}

	protected override void Trigger(ProtoActor protoActor, int newState)
	{
		protoActor.ReinforceItem(delegate
		{
			OnReinforceItem();
		});
	}

	private void OnReinforceItem()
	{
		if (feedbackEffect != null)
		{
			feedbackEffect.PlayFeedbacks();
		}
		PlayTriggerSound(0, 0);
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		InventoryItem handheldItem = protoActor.GetHandheldItem();
		if (handheldItem == null)
		{
			return Hub.GetL10NText("STRING_EQUIPMENT_UPGRADE_TEXT");
		}
		if (!handheldItem.IsReinforceable)
		{
			return Hub.GetL10NText("STRING_EQUIPMENT_UPGRADE_TEXT");
		}
		if (handheldItem.ReinforceCost > Hub.s.pdata.main.CurrentCurrency)
		{
			return Hub.GetL10NText("STRING_EQUIPMENT_UPGRADE_NOT_ENOUGH_MONEY", handheldItem.ReinforceCost);
		}
		return Hub.GetL10NText("STRING_EQUIPMENT_UPGRADE_PROGRESS_TEXT", handheldItem.ReinforceCost);
	}
}
