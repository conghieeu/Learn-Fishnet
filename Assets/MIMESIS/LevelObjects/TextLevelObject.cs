using Mimic.Actors;
using UnityEngine;

public class TextLevelObject : LevelObject
{
	[SerializeField]
	[Multiline(3)]
	private string crossHairText;

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.Text;

	protected void Awake()
	{
		base.crossHairType = CrosshairType.Scrap;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return Hub.GetL10NText(crossHairText);
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		return false;
	}
}
