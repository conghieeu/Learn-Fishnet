using System.Collections.Generic;
using Mimic.Actors;
using UnityEngine;
using UnityEngine.Playables;

[DefaultExecutionOrder(-10)]
public class TramCutSceneHelper : MonoBehaviour
{
	private GameObject _tramObject;

	private BoxCollider _tramBoxCollider;

	private float _tramStartPosition = -1f;

	private bool _startedTramRepairCutScene;

	private bool _startedTramRepairBackCutScene;

	private PlayableDirector _cutSceneDirector;

	private Vector3 _tramPrevPosition = Vector3.zero;

	private void Awake()
	{
		_tramObject = base.gameObject;
		MapTrigger componentInChildren = _tramObject.GetComponentInChildren<MapTrigger>();
		if (componentInChildren != null)
		{
			_tramBoxCollider = componentInChildren.GetComponent<BoxCollider>();
		}
	}

	private bool IsActorInTram(ProtoActor actor)
	{
		if (actor == null)
		{
			return false;
		}
		Vector3 position = actor.transform.position;
		return _tramBoxCollider.bounds.Contains(position);
	}

	private void Update()
	{
		UpdateTramRepairCutScene();
		UpdateTramRepairBackCutScene();
	}

	public void OnPrePlayTramRepairCutScene(CutScenePlayer.CutScene cutSceneInfo)
	{
		_cutSceneDirector = cutSceneInfo.direction;
	}

	public void OnPostPlayTramRepairCutScene(CutScenePlayer.CutScene cutSceneInfo)
	{
		_startedTramRepairCutScene = true;
	}

	public void OnPrePlayTramRepairBackCutScene(CutScenePlayer.CutScene cutSceneInfo)
	{
		_cutSceneDirector = cutSceneInfo.direction;
	}

	public void OnPostPlayTramRepairBackCutScene(CutScenePlayer.CutScene cutSceneInfo)
	{
		_startedTramRepairCutScene = false;
		_startedTramRepairBackCutScene = true;
	}

	private void AttachLootingObjectToTram()
	{
		LootingLevelObject[] array = Hub.s.pdata.main?.GetBGRoot().GetComponentsInChildren<LootingLevelObject>();
		if (array == null)
		{
			return;
		}
		LootingLevelObject[] array2 = array;
		foreach (LootingLevelObject lootingLevelObject in array2)
		{
			Vector3 position = lootingLevelObject.transform.position;
			if (_tramBoxCollider.bounds.Contains(position) && lootingLevelObject.transform.parent != _tramObject.transform)
			{
				lootingLevelObject.transform.SetParent(_tramObject.transform);
				lootingLevelObject.enabled = false;
			}
		}
	}

	private void DetachLootingObjectFromTram()
	{
		LootingLevelObject[] array = Hub.s.pdata.main?.GetBGRoot().GetComponentsInChildren<LootingLevelObject>();
		if (array == null)
		{
			return;
		}
		LootingLevelObject[] array2 = array;
		foreach (LootingLevelObject lootingLevelObject in array2)
		{
			if (lootingLevelObject.transform.parent == _tramObject.transform)
			{
				lootingLevelObject.transform.SetParent(Hub.s.pdata.main.GetBGRoot());
				lootingLevelObject.enabled = true;
			}
		}
	}

	private void UpdateTramRepairBackCutScene()
	{
		if (_startedTramRepairBackCutScene && !(_cutSceneDirector.time < _cutSceneDirector.duration))
		{
			_startedTramRepairBackCutScene = false;
		}
	}

	private void UpdateTramRepairCutScene()
	{
		if (!_startedTramRepairCutScene)
		{
			return;
		}
		if (_cutSceneDirector.time >= _cutSceneDirector.duration)
		{
			_startedTramRepairCutScene = false;
			return;
		}
		_ = _tramObject.transform.position - _tramPrevPosition;
		Dictionary<int, ProtoActor> dictionary = Hub.s.pdata.main?.GetProtoActorMap();
		if (dictionary == null)
		{
			return;
		}
		foreach (ProtoActor value in dictionary.Values)
		{
			if (value == null)
			{
				continue;
			}
			if (_cutSceneDirector.time > 4.5)
			{
				if (value.transform.parent == _tramObject.transform)
				{
					value.LastDamagedForceDirection = new Vector3(0f, -1f, 0f);
					if (value.AmIAvatar())
					{
						value.CancelDontMove();
					}
					else
					{
						value.StopPauseTransformSync();
					}
					value.transform.SetParent(Hub.s.pdata.main.GetBGRoot());
				}
			}
			else if (IsActorInTram(value) && value.transform.parent != _tramObject.transform)
			{
				if (value.AmIAvatar())
				{
					value.DontMove();
				}
				else
				{
					value.StartPauseTransformSync();
				}
				value.transform.SetParent(_tramObject.transform);
			}
		}
		_tramPrevPosition = _tramObject.transform.position;
	}
}
