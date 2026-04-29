using System;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MMAudioClipTable", menuName = "_Mimic/MMAudioClipTable", order = 0)]
[HelpURL("https://krafton.atlassian.net/wiki/x/TmE6D")]
internal class MMAudioClipTable : ScriptableObject
{
	public enum eCategory
	{
		BGM = 0,
		SFX = 1
	}

	[Serializable]
	public class Row
	{
		public string id = string.Empty;

		public eCategory category;

		public string category2;

		public AudioClip[] clips = new AudioClip[0];

		public bool loop;

		public float volume = 1f;

		public bool _3DSound = true;

		public float minDistance = 1f;

		public float maxDistance = 10f;

		public float minVolume = 1f;

		public float cooltime = 0.1f;

		[InspectorReadOnly]
		public AnimationCurve volumeCurve = new AnimationCurve();

		internal void PrepareToPlay(AudioSource? source)
		{
			if (source == null)
			{
				Logger.RError("AudioSource is null");
				return;
			}
			AudioClip clip = ((clips != null && clips.Length != 0) ? clips[UnityEngine.Random.Range(0, clips.Length)] : null);
			source.clip = clip;
			source.loop = loop;
			source.volume = volume;
			source.rolloffMode = AudioRolloffMode.Custom;
			source.minDistance = minDistance;
			source.maxDistance = maxDistance;
			source.SetCustomCurve(AudioSourceCurveType.CustomRolloff, volumeCurve);
			source.spatialBlend = (_3DSound ? 1 : 0);
		}
	}

	[Serializable]
	public class CategorizedRow
	{
		public string category;

		public List<Row> rows = new List<Row>();
	}

	[Tooltip("Y축 추가 감쇄 커브")]
	public AnimationCurve yAxisRolloffCurve = AnimationCurve.Linear(0f, 1f, 1f, 0f);

	public string[] sourceFolders = new string[1] { "Assets/_mimic/art/AudioClip" };

	[SerializeField]
	private List<CategorizedRow> rows2 = new List<CategorizedRow>();

	public Row? FindRow2(string categoriedId)
	{
		string[] array = categoriedId.Split('.');
		string text = "SFX";
		string text2 = "";
		if (array.Length == 0)
		{
			return null;
		}
		if (array.Length == 1)
		{
			text2 = array[0];
		}
		else
		{
			if (array.Length != 2)
			{
				return null;
			}
			text = array[0];
			text2 = array[1];
		}
		foreach (CategorizedRow item in rows2)
		{
			if (!(item.category == text))
			{
				continue;
			}
			foreach (Row row in item.rows)
			{
				if (row.id == text2)
				{
					return row;
				}
			}
		}
		return null;
	}

	public Row? FindRow2WithoutCategory(string bareId)
	{
		foreach (CategorizedRow item in rows2)
		{
			foreach (Row row in item.rows)
			{
				if (row.id == bareId)
				{
					return row;
				}
			}
		}
		return null;
	}
}
