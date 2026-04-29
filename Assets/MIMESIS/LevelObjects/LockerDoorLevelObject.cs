using UnityEngine;

[RequireComponent(typeof(Animator))]
public class LockerDoorLevelObject : DoorLevelObject
{
	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.LockerDoor;
}
