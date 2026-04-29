using Mimic.Actors;
using UnityEngine;

public class SwitchLevelObject : StaticLevelObject
{
	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Switch;

	private void Start()
	{
		base.crossHairType = CrosshairType.Switch;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (IsTriggerable(protoActor, 0))
		{
			Trigger(protoActor, 0);
		}
		return true;
	}
}
