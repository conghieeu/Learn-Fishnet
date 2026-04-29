using System;
using System.Collections.Generic;
using System.Linq;
using DLAgent;
using Mimic.Actors;
using Mimic.Voice.SpeechSystem;
using Unity.MLAgents;
using UnityEngine;

public class DLAcademyManager : MonoBehaviour
{
	public int CurrentFrameIndex;

	private float timer;

	private int _activatedAgentCount;

	private List<int> _activeDLAgents = new List<int>();

	private BoxCollider[] _outdoor;

	private BoxCollider[] _mainDoorInside;

	private BoxCollider[] _backDoorInside;

	private BoxCollider[] _stairs;

	private BoxCollider[] _tram;

	private BoxCollider[] _tramInside;

	private BoxCollider[] _trapCorridors;

	private BoxCollider[] _trapWeight;

	private BoxCollider[] _8way;

	private BoxCollider[] _mainDoorOutside;

	private BoxCollider[] _backDoorOutside;

	private BoxCollider[] _mainStreet;

	private BoxCollider[] _mainStreetFromBackdoor;

	private BoxCollider[] _jackpotRoom;

	private Transform _tramTransform;

	private Transform _toTramHelperTransform;

	private Transform _exitTramHelperTransform;

	private Transform _toEntranceHelperTransform;

	private Transform _enterTramHelperTransform;

	private BoxCollider _enterTramHelperCollider;

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] DLAcademyManager.Awake ->");
		Academy.Instance.AutomaticSteppingEnabled = false;
		CurrentFrameIndex = 0;
		Logger.RLog("[AwakeLogs] DLAcademyManager.Awake <-");
	}

	public int Step()
	{
		timer = Time.realtimeSinceStartup;
		StepActiveAgents();
		Academy.Instance.EnvironmentStep();
		int currentFrameIndex = CurrentFrameIndex;
		CurrentFrameIndex = (CurrentFrameIndex + 1) % 3;
		return currentFrameIndex;
	}

	public void Reset()
	{
		_activeDLAgents.Clear();
	}

	public bool IsAcademyInitialized()
	{
		return Academy.IsInitialized;
	}

	private ProtoActor? GetActorByActorID(int actorID)
	{
		if (Hub.s == null)
		{
			return null;
		}
		if (Hub.s.pdata == null)
		{
			return null;
		}
		if (Hub.s.pdata.main == null)
		{
			return null;
		}
		return Hub.s.pdata.main.GetActorByActorID(actorID);
	}

	public bool ExistActivatedAgent()
	{
		return _activatedAgentCount > 0;
	}

	public DLMovementAgent? GetDLMovementAgentByActorID(int actorID)
	{
		ProtoActor actorByActorID = GetActorByActorID(actorID);
		if (actorByActorID == null)
		{
			return null;
		}
		if (actorByActorID.dlMovementAgent == null)
		{
			return null;
		}
		return actorByActorID.dlMovementAgent;
	}

	public DLDecisionAgent? GetDLDecisionAgentByActorID(int actorID)
	{
		ProtoActor actorByActorID = GetActorByActorID(actorID);
		if (actorByActorID == null)
		{
			return null;
		}
		if (actorByActorID.dlDecisionAgent == null)
		{
			return null;
		}
		return actorByActorID.dlDecisionAgent;
	}

	public bool RegisterTargetActor(int actorID, VCreature targerCreature)
	{
		if (targerCreature == null)
		{
			return false;
		}
		DLMovementAgent dLMovementAgentByActorID = GetDLMovementAgentByActorID(actorID);
		if (dLMovementAgentByActorID == null)
		{
			return false;
		}
		dLMovementAgentByActorID.RegisterTargetActor(targerCreature);
		return true;
	}

	public bool ActivateDLAgentByActorID(VCreature creature, float frequency)
	{
		int objectID = creature.ObjectID;
		DLMovementAgent dLMovementAgentByActorID = GetDLMovementAgentByActorID(objectID);
		if (dLMovementAgentByActorID == null)
		{
			return false;
		}
		dLMovementAgentByActorID.SetFrequency(frequency);
		dLMovementAgentByActorID.SetCreature(creature);
		dLMovementAgentByActorID.enabled = true;
		DLDecisionAgent dLDecisionAgentByActorID = GetDLDecisionAgentByActorID(objectID);
		if (dLDecisionAgentByActorID == null)
		{
			return false;
		}
		dLDecisionAgentByActorID.SetCreature(creature);
		dLDecisionAgentByActorID.enabled = true;
		_activatedAgentCount++;
		_activeDLAgents.Add(objectID);
		return true;
	}

	public bool DeactivateDLAgentByActorID(int actorID)
	{
		DLMovementAgent dLMovementAgentByActorID = GetDLMovementAgentByActorID(actorID);
		if (dLMovementAgentByActorID == null)
		{
			return false;
		}
		dLMovementAgentByActorID.RegisterTargetActor(null);
		dLMovementAgentByActorID.enabled = false;
		DLDecisionAgent dLDecisionAgentByActorID = GetDLDecisionAgentByActorID(actorID);
		if (dLDecisionAgentByActorID == null)
		{
			return false;
		}
		dLDecisionAgentByActorID.enabled = false;
		_activatedAgentCount--;
		_activeDLAgents.Remove(actorID);
		return true;
	}

	public void StepActiveAgents()
	{
		foreach (int activeDLAgent in _activeDLAgents)
		{
			DLMovementAgent dLMovementAgentByActorID = GetDLMovementAgentByActorID(activeDLAgent);
			if (dLMovementAgentByActorID != null)
			{
				dLMovementAgentByActorID.Step();
			}
		}
	}

	private bool BoundsContainsSafe(Collider c, Vector3 p)
	{
		if (!c)
		{
			return false;
		}
		try
		{
			return c.bounds.Contains(p);
		}
		catch (NullReferenceException)
		{
			return false;
		}
		catch (MissingReferenceException)
		{
			return false;
		}
	}

	public int GetAreaForDL(Vector3 pos, out SpeechType_Area areaType, bool forceReset = false)
	{
		if (forceReset)
		{
			_outdoor = null;
			_mainDoorInside = null;
			_backDoorInside = null;
			_stairs = null;
			_tram = null;
			_tramInside = null;
			_trapCorridors = null;
			_trapWeight = null;
			_8way = null;
			_mainStreet = null;
			_mainStreetFromBackdoor = null;
			_mainDoorOutside = null;
			_backDoorOutside = null;
			_tramTransform = null;
			_toTramHelperTransform = null;
			_exitTramHelperTransform = null;
			_toEntranceHelperTransform = null;
			_enterTramHelperTransform = null;
			_jackpotRoom = null;
			GameObject gameObject = GameObject.FindGameObjectWithTag("Area_Outdoor");
			if (gameObject != null)
			{
				_outdoor = gameObject.GetComponents<BoxCollider>();
				if (gameObject.transform.childCount > 0)
				{
					_mainStreet = gameObject.transform.GetChild(0).gameObject.GetComponents<BoxCollider>();
				}
				if (gameObject.transform.childCount > 1)
				{
					_mainStreetFromBackdoor = gameObject.transform.GetChild(1).gameObject.GetComponents<BoxCollider>();
				}
				_toEntranceHelperTransform = gameObject.transform.Find("ToEntranceHelper");
				_enterTramHelperTransform = gameObject.transform.Find("EnterTramHelper");
				if (_enterTramHelperTransform != null)
				{
					_enterTramHelperCollider = _enterTramHelperTransform.GetComponent<BoxCollider>();
				}
				else
				{
					_enterTramHelperCollider = null;
				}
			}
			_outdoor = GameObject.FindGameObjectsWithTag("Area_Outdoor").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			_mainDoorInside = GameObject.FindGameObjectsWithTag("Area_MainDoor_Inside").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			_backDoorInside = GameObject.FindGameObjectsWithTag("Area_BackDoor_Inside").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			_stairs = GameObject.FindGameObjectsWithTag("Area_Stairs").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			_trapCorridors = GameObject.FindGameObjectsWithTag("Area_TrapCorridor").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			_trapWeight = GameObject.FindGameObjectsWithTag("Area_TrapWeight").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			_8way = GameObject.FindGameObjectsWithTag("Area_8way").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			_jackpotRoom = GameObject.FindGameObjectsWithTag("Area_Jackpot").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
			GameObject gameObject2 = GameObject.FindGameObjectWithTag("Area_Tram");
			if (gameObject2 != null)
			{
				_tramTransform = gameObject2.transform;
				_tram = gameObject2.GetComponents<BoxCollider>();
				Transform transform = gameObject2.transform.Find("TramInside");
				if (transform != null)
				{
					_tramInside = transform.GetComponents<BoxCollider>();
				}
				else
				{
					_tramInside = null;
				}
				_exitTramHelperTransform = gameObject2.transform.Find("ExitTramHelper");
			}
			GameObject gameObject3 = GameObject.FindGameObjectWithTag("Area_MainDoor_Outside");
			if (gameObject3 != null)
			{
				_mainDoorOutside = gameObject3.GetComponents<BoxCollider>();
				if (gameObject3.transform.childCount > 0)
				{
					_toTramHelperTransform = gameObject3.transform.GetChild(0).transform;
				}
				else
				{
					_toTramHelperTransform = null;
				}
			}
			_backDoorOutside = GameObject.FindGameObjectsWithTag("Area_BackDoor_Outside").SelectMany((GameObject go) => go.GetComponents<BoxCollider>()).ToArray();
		}
		if (Hub.s.pdata.main != null && Hub.s.pdata.main as DeathMatchScene != null)
		{
			areaType = SpeechType_Area.DeathMatch;
			return 0;
		}
		if (_tramInside != null)
		{
			for (int num = 0; num < _tramInside.Length; num++)
			{
				if (BoundsContainsSafe(_tramInside[num], pos))
				{
					areaType = SpeechType_Area.Tram;
					return 1;
				}
			}
		}
		if (_tram != null)
		{
			for (int num2 = 0; num2 < _tram.Length; num2++)
			{
				if (BoundsContainsSafe(_tram[num2], pos))
				{
					areaType = SpeechType_Area.Tram;
					return 1;
				}
			}
		}
		if (_outdoor != null)
		{
			for (int num3 = 0; num3 < _outdoor.Length; num3++)
			{
				if (BoundsContainsSafe(_outdoor[num3], pos))
				{
					areaType = SpeechType_Area.Outdoor;
					return 1;
				}
			}
		}
		if (_mainDoorInside != null)
		{
			for (int num4 = 0; num4 < _mainDoorInside.Length; num4++)
			{
				if (BoundsContainsSafe(_mainDoorInside[num4], pos))
				{
					areaType = SpeechType_Area.MainDoor_Indoor;
					return 0;
				}
			}
		}
		if (_backDoorInside != null)
		{
			for (int num5 = 0; num5 < _backDoorInside.Length; num5++)
			{
				if (BoundsContainsSafe(_backDoorInside[num5], pos))
				{
					areaType = SpeechType_Area.BackDoor_Indoor;
					return 0;
				}
			}
		}
		if (_stairs != null)
		{
			for (int num6 = 0; num6 < _stairs.Length; num6++)
			{
				if (BoundsContainsSafe(_stairs[num6], pos))
				{
					areaType = SpeechType_Area.Stairs;
					return 0;
				}
			}
		}
		if (_trapWeight != null)
		{
			for (int num7 = 0; num7 < _trapWeight.Length; num7++)
			{
				if (BoundsContainsSafe(_trapWeight[num7], pos))
				{
					areaType = SpeechType_Area.TrapWeight;
					return 0;
				}
			}
		}
		if (_trapCorridors != null)
		{
			for (int num8 = 0; num8 < _trapCorridors.Length; num8++)
			{
				if (BoundsContainsSafe(_trapCorridors[num8], pos))
				{
					areaType = SpeechType_Area.TrapCorridor;
					return 0;
				}
			}
		}
		if (_8way != null)
		{
			for (int num9 = 0; num9 < _8way.Length; num9++)
			{
				if (BoundsContainsSafe(_8way[num9], pos))
				{
					areaType = SpeechType_Area.Indoor;
					return 0;
				}
			}
		}
		if (_jackpotRoom != null)
		{
			for (int num10 = 0; num10 < _jackpotRoom.Length; num10++)
			{
				if (BoundsContainsSafe(_jackpotRoom[num10], pos))
				{
					areaType = SpeechType_Area.JackpotRoom;
					return 0;
				}
			}
		}
		areaType = SpeechType_Area.Indoor;
		return 0;
	}

	public int GetAreaForDLTraining(Vector3 pos)
	{
		int dungeonMasterID = Hub.s.pdata.dungeonMasterID;
		DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonMasterID);
		if (dungeonInfo == null)
		{
			return 0;
		}
		if (Hub.s.pdata.main != null && Hub.s.pdata.main as DeathMatchScene != null)
		{
			return 0;
		}
		if (_tramInside != null)
		{
			for (int i = 0; i < _tramInside.Length; i++)
			{
				if (BoundsContainsSafe(_tramInside[i], pos))
				{
					if (dungeonInfo.MapID == 15)
					{
						return 14;
					}
					if (dungeonInfo.MapID == 17)
					{
						return 15;
					}
					if (dungeonInfo.MapID == 18)
					{
						return 16;
					}
					return 0;
				}
			}
		}
		if (_tram != null)
		{
			for (int j = 0; j < _tram.Length; j++)
			{
				if (BoundsContainsSafe(_tram[j], pos))
				{
					if (dungeonInfo.MapID == 15)
					{
						return 4;
					}
					if (dungeonInfo.MapID == 17)
					{
						return 5;
					}
					if (dungeonInfo.MapID == 18)
					{
						return 6;
					}
					return 0;
				}
			}
		}
		if (_mainStreet != null)
		{
			for (int k = 0; k < _mainStreet.Length; k++)
			{
				if (BoundsContainsSafe(_mainStreet[k], pos))
				{
					return 1;
				}
			}
		}
		if (_mainStreetFromBackdoor != null)
		{
			for (int l = 0; l < _mainStreetFromBackdoor.Length; l++)
			{
				if (BoundsContainsSafe(_mainStreetFromBackdoor[l], pos))
				{
					return 1;
				}
			}
		}
		if (_mainDoorOutside != null)
		{
			for (int m = 0; m < _mainDoorOutside.Length; m++)
			{
				if (BoundsContainsSafe(_mainDoorOutside[m], pos))
				{
					return 1;
				}
			}
		}
		if (_backDoorOutside != null)
		{
			for (int n = 0; n < _backDoorOutside.Length; n++)
			{
				if (BoundsContainsSafe(_backDoorOutside[n], pos))
				{
					return 1;
				}
			}
		}
		if (_outdoor != null)
		{
			for (int num = 0; num < _outdoor.Length; num++)
			{
				if (BoundsContainsSafe(_outdoor[num], pos))
				{
					return 0;
				}
			}
		}
		if (_mainDoorInside != null)
		{
			for (int num2 = 0; num2 < _mainDoorInside.Length; num2++)
			{
				if (BoundsContainsSafe(_mainDoorInside[num2], pos))
				{
					if (dungeonInfo.MapID == 15)
					{
						return 7;
					}
					if (dungeonInfo.MapID == 17)
					{
						return 8;
					}
					if (dungeonInfo.MapID == 18)
					{
						return 9;
					}
					return 0;
				}
			}
		}
		if (_backDoorInside != null)
		{
			for (int num3 = 0; num3 < _backDoorInside.Length; num3++)
			{
				if (BoundsContainsSafe(_backDoorInside[num3], pos))
				{
					if (dungeonInfo.MapID == 15)
					{
						return 10;
					}
					if (dungeonInfo.MapID == 17)
					{
						return 11;
					}
					if (dungeonInfo.MapID == 18)
					{
						return 12;
					}
					return 0;
				}
			}
		}
		if (_stairs != null)
		{
			for (int num4 = 0; num4 < _stairs.Length; num4++)
			{
				if (BoundsContainsSafe(_stairs[num4], pos))
				{
					return 0;
				}
			}
		}
		if (_trapWeight != null)
		{
			for (int num5 = 0; num5 < _trapWeight.Length; num5++)
			{
				if (BoundsContainsSafe(_trapWeight[num5], pos))
				{
					return 0;
				}
			}
		}
		if (_trapCorridors != null)
		{
			for (int num6 = 0; num6 < _trapCorridors.Length; num6++)
			{
				if (BoundsContainsSafe(_trapCorridors[num6], pos))
				{
					return 0;
				}
			}
		}
		if (_8way != null)
		{
			for (int num7 = 0; num7 < _8way.Length; num7++)
			{
				if (BoundsContainsSafe(_8way[num7], pos))
				{
					return 0;
				}
			}
		}
		return 0;
	}

	public DLDecisionAgent.OutdoorArea GetOutdoorArea(Vector3 pos)
	{
		if (_mainDoorOutside != null)
		{
			for (int i = 0; i < _mainDoorOutside.Length; i++)
			{
				if (BoundsContainsSafe(_mainDoorOutside[i], pos))
				{
					return DLDecisionAgent.OutdoorArea.MainDoorOutside;
				}
			}
		}
		if (_backDoorOutside != null)
		{
			for (int j = 0; j < _backDoorOutside.Length; j++)
			{
				if (BoundsContainsSafe(_backDoorOutside[j], pos))
				{
					return DLDecisionAgent.OutdoorArea.BackDoorOutside;
				}
			}
		}
		if (_tramInside != null)
		{
			for (int k = 0; k < _tramInside.Length; k++)
			{
				if (BoundsContainsSafe(_tramInside[k], pos))
				{
					return DLDecisionAgent.OutdoorArea.TramInside;
				}
			}
		}
		if (_tram != null)
		{
			for (int l = 0; l < _tram.Length; l++)
			{
				if (BoundsContainsSafe(_tram[l], pos))
				{
					return DLDecisionAgent.OutdoorArea.Tram;
				}
			}
		}
		if (_mainStreet != null)
		{
			for (int m = 0; m < _mainStreet.Length; m++)
			{
				if (BoundsContainsSafe(_mainStreet[m], pos))
				{
					return DLDecisionAgent.OutdoorArea.MainStreet;
				}
			}
		}
		if (_mainStreetFromBackdoor != null)
		{
			for (int n = 0; n < _mainStreetFromBackdoor.Length; n++)
			{
				if (BoundsContainsSafe(_mainStreetFromBackdoor[n], pos))
				{
					return DLDecisionAgent.OutdoorArea.MainStreetFromBackdoor;
				}
			}
		}
		return DLDecisionAgent.OutdoorArea.Other;
	}

	public Vector3 GetToTramHelperPosition()
	{
		if (_toTramHelperTransform != null)
		{
			return _toTramHelperTransform.position;
		}
		return Vector3.zero;
	}

	public Vector3 GetExitTramHelperPosition()
	{
		if (_exitTramHelperTransform != null)
		{
			return _exitTramHelperTransform.position;
		}
		return Vector3.zero;
	}

	public Vector3 GetToEntranceHelperPosition()
	{
		if (_toEntranceHelperTransform != null)
		{
			return _toEntranceHelperTransform.position;
		}
		return Vector3.zero;
	}

	public Vector3 GetEnterTramHelperPosition()
	{
		if (_enterTramHelperCollider != null)
		{
			Bounds bounds = _enterTramHelperCollider.bounds;
			return new Vector3(UnityEngine.Random.Range(bounds.min.x, bounds.max.x), UnityEngine.Random.Range(bounds.min.y, bounds.max.y), UnityEngine.Random.Range(bounds.min.z, bounds.max.z));
		}
		return Vector3.zero;
	}

	public bool IsInsideInsideTram(Vector3 pos)
	{
		if (_enterTramHelperCollider != null)
		{
			Bounds bounds = _enterTramHelperCollider.bounds;
			if (pos.x >= bounds.min.x && pos.x <= bounds.max.x && pos.z >= bounds.min.z)
			{
				return pos.z <= bounds.max.z;
			}
			return false;
		}
		return false;
	}

	public Vector3 GetInsideTramHelperDirection()
	{
		if (_enterTramHelperTransform != null)
		{
			return _enterTramHelperTransform.forward;
		}
		return Vector3.zero;
	}
}
