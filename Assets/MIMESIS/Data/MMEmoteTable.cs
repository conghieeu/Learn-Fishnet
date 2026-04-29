using System;
using System.Collections.Generic;
using System.Linq;
using Mimic.InputSystem;
using UnityEngine;

[CreateAssetMenu(fileName = "MMEmoteTable", menuName = "_Mimic/MMEmoteTable", order = 0)]
public class MMEmoteTable : ScriptableObject
{
	[Serializable]
	public class EmoteGroup
	{
		public InputAction action = InputAction.Emote01;

		public Emote[] emotes = Array.Empty<Emote>();
	}

	[Serializable]
	public class Emote
	{
		public int emoteMasterID;

		public string motionName = string.Empty;

		public int prob = 1;
	}

	[Header("만약 Prob값이 1보다 작을 경우 이 값은 무시되며 1로 간주합니다.")]
	[SerializeField]
	private EmoteGroup[] emoteGroups = Array.Empty<EmoteGroup>();

	public bool TryGetEmote(InputAction action, out Emote? emote)
	{
		EmoteGroup[] array = emoteGroups;
		foreach (EmoteGroup emoteGroup in array)
		{
			if (emoteGroup.action == action)
			{
				return emoteGroup.emotes.TryPickRandom<Emote>((Emote e) => e.prob, out emote);
			}
		}
		emote = null;
		return false;
	}

	public int GetRandomEmoteMasterID()
	{
		if (emoteGroups.Length == 0)
		{
			return 0;
		}
		EmoteGroup emoteGroup = emoteGroups.OrderBy((EmoteGroup _) => Guid.NewGuid()).FirstOrDefault();
		if (emoteGroup == null || emoteGroup.emotes.Length == 0)
		{
			return 0;
		}
		if (emoteGroup.emotes.TryPickRandom((Emote e) => e.prob, out var pick) && pick != null)
		{
			return pick.emoteMasterID;
		}
		return 0;
	}

	public bool TryGetEmote(int emoteMasterID, out Emote? emote)
	{
		EmoteGroup[] array = emoteGroups;
		for (int i = 0; i < array.Length; i++)
		{
			Emote[] emotes = array[i].emotes;
			foreach (Emote emote2 in emotes)
			{
				if (emote2.emoteMasterID == emoteMasterID)
				{
					emote = emote2;
					return true;
				}
			}
		}
		emote = null;
		return false;
	}

	public List<InputAction> CollectInputActionList()
	{
		List<InputAction> list = new List<InputAction>();
		EmoteGroup[] array = emoteGroups;
		foreach (EmoteGroup emoteGroup in array)
		{
			list.Add(emoteGroup.action);
		}
		return list;
	}

	public bool TryGetRandomEmoteInSameGroup(int emoteMasterID, out Emote? emote)
	{
		EmoteGroup[] array = emoteGroups;
		foreach (EmoteGroup emoteGroup in array)
		{
			Emote[] emotes = emoteGroup.emotes;
			for (int j = 0; j < emotes.Length; j++)
			{
				if (emotes[j].emoteMasterID == emoteMasterID)
				{
					return emoteGroup.emotes.TryPickRandom<Emote>((Emote x) => x.prob, out emote);
				}
			}
		}
		emote = null;
		return false;
	}

	public bool AreEmotesInSameGroup(int emoteID1, int emoteID2)
	{
		if (emoteID1 == emoteID2)
		{
			return true;
		}
		EmoteGroup[] array = emoteGroups;
		foreach (EmoteGroup obj in array)
		{
			bool flag = false;
			bool flag2 = false;
			Emote[] emotes = obj.emotes;
			foreach (Emote obj2 in emotes)
			{
				if (obj2.emoteMasterID == emoteID1)
				{
					flag = true;
				}
				if (obj2.emoteMasterID == emoteID2)
				{
					flag2 = true;
				}
			}
			if (flag && flag2)
			{
				return true;
			}
		}
		return false;
	}
}
