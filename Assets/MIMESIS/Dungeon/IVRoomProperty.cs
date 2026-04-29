using UnityEngine;

public abstract class IVRoomProperty
{
	public readonly long SessionID;

	public readonly VRoomType vRoomType;

	public readonly Vector2 Min;

	public readonly Vector2 Max;

	public readonly int SectorSize;

	public int TargetCurrency { get; private set; }

	public IVRoomProperty(long sessionID, VRoomType vRoomType, Vector2 min, Vector2 max, int sectorSize, int targetCurrency)
	{
		SessionID = sessionID;
		this.vRoomType = vRoomType;
		Min = min;
		Max = max;
		SectorSize = sectorSize;
		TargetCurrency = targetCurrency;
	}

	public void OverwriteTargetCurrency(int targetCurrency)
	{
		TargetCurrency = targetCurrency;
	}
}
