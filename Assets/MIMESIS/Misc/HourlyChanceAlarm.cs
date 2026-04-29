using System;
using System.Collections.Generic;
using UnityEngine;

public class HourlyChanceAlarm : SocketAttachable, ITimeSyncable
{
	[SerializeField]
	[Tooltip("알림이 울렸을 때 Animator에 트리거")]
	private string ringAnimTrigger = "Ring";

	[SerializeField]
	[Tooltip("알림이 울릴 확률 (0.0 ~ 1.0)")]
	[Range(0f, 1f)]
	private float ringChance = 1f;

	private GameMainBase.SyncRandom? syncRandom;

	private HashSet<int> ringHours = new HashSet<int>();

	public override void OnAttachToSocket()
	{
		RandomizeRingHours(ref ringHours);
		if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null)
		{
			Hub.s.pdata.main.AddTimeSyncable(this);
		}
	}

	public override void OnDetachFromSocket()
	{
		ringHours.Clear();
		if (Hub.s != null)
		{
			if (Hub.s.pdata != null && Hub.s.pdata.main != null)
			{
				Hub.s.pdata.main.RemoveTimeSyncable(this);
			}
			if (Hub.s.audioman != null)
			{
				Hub.s.audioman.StopSfxTransform(base.transform);
			}
		}
	}

	public void OnTimeSync(TimeSpan now)
	{
		if (ringHours.Contains(now.Hours))
		{
			Ring();
		}
	}

	private void Ring()
	{
		if (animator != null)
		{
			animator.SetTrigger(ringAnimTrigger);
		}
	}

	private void InitRandomState()
	{
		if (base.owner == null || Hub.s == null || Hub.s.pdata == null)
		{
			Logger.RError($"Failed to initialize random state: itemMasterID={base.item.ItemMasterID}");
			return;
		}
		int seed = Hub.s.pdata.randDungeonSeed + base.owner.ActorID;
		syncRandom = new GameMainBase.SyncRandom(seed);
	}

	private void RandomizeRingHours(ref HashSet<int> ringHours)
	{
		InitRandomState();
		ringHours.Clear();
		for (int i = 0; i < 24; i++)
		{
			if (syncRandom.Next(0, 1000) < (int)(ringChance * 1000f))
			{
				ringHours.Add(i);
			}
		}
	}
}
