using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mimic.Actors;
using UnityEngine;

public class UIPrefab_Spectator_PlayerListView : UIPrefabScript
{
	[SerializeField]
	private Color liveColor;

	[SerializeField]
	private Color deadColor;

	private UIPrefab_Spectator_PlayerListViewItem[] playerListViewItems;

	private Dictionary<string, string> steamIDToNameCache = new Dictionary<string, string>();

	private void OnEnable()
	{
		steamIDToNameCache.Clear();
	}

	private void Start()
	{
		playerListViewItems = GetComponentsInChildren<UIPrefab_Spectator_PlayerListViewItem>(includeInactive: true);
		if (playerListViewItems == null)
		{
			Logger.RError("PlayerListViewItem not found");
			return;
		}
		for (int i = 0; i < playerListViewItems.Length; i++)
		{
			UIPrefab_Spectator_PlayerListViewItem obj = playerListViewItems[i];
			obj.gameObject.SetActive(value: true);
			obj.SetColor(liveColor);
			obj.SpriteChangeAnimation.TurnOff();
		}
	}

	public void UpdatePlayerListView(List<Tuple<int, bool, bool>> actorsInfo, CancellationToken cancellationToken)
	{
		if (playerListViewItems == null || actorsInfo.Count == 0 || Hub.s.pdata?.main == null)
		{
			return;
		}
		for (int i = 0; i < playerListViewItems.Length; i++)
		{
			UIPrefab_Spectator_PlayerListViewItem uIPrefab_Spectator_PlayerListViewItem = playerListViewItems[i];
			if (i >= actorsInfo.Count)
			{
				uIPrefab_Spectator_PlayerListViewItem.UE_Name_Text.SetText(string.Empty);
				uIPrefab_Spectator_PlayerListViewItem.SpriteChangeAnimation.TurnOff();
				continue;
			}
			Tuple<int, bool, bool> tuple = actorsInfo[i];
			int item = tuple.Item1;
			bool item2 = tuple.Item2;
			bool item3 = tuple.Item3;
			ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(item);
			if (actorByActorID == null)
			{
				uIPrefab_Spectator_PlayerListViewItem.UE_Name_Text.SetText(string.Empty);
				uIPrefab_Spectator_PlayerListViewItem.SpriteChangeAnimation.TurnOff();
				continue;
			}
			string text = Hub.s.pdata.main.ResolveNickName(actorByActorID, actorByActorID.netSyncActorData.actorName);
			uIPrefab_Spectator_PlayerListViewItem.UE_Name_Text.SetText(text);
			uIPrefab_Spectator_PlayerListViewItem.SetColor(item2 ? deadColor : liveColor);
			if (item3)
			{
				if (uIPrefab_Spectator_PlayerListViewItem.SpriteChangeAnimation.CanPlay)
				{
					uIPrefab_Spectator_PlayerListViewItem.SpriteChangeAnimation.Play(cancellationToken).Forget();
				}
			}
			else if (!uIPrefab_Spectator_PlayerListViewItem.SpriteChangeAnimation.IsPlaying)
			{
				uIPrefab_Spectator_PlayerListViewItem.SpriteChangeAnimation.TurnOff();
			}
		}
	}
}
