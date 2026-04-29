using System;
using System.Collections.Generic;
using DarkTonic.MasterAudio;
using Mimic.Voice.SpeechSystem;
using UnityEngine;

public class Compass : SocketAttachable, IToggleableItem
{
	public enum CompassMode
	{
		Exit = 0,
		Treasure = 1
	}

	[SerializeField]
	private GameObject needle;

	[Header("Audio Settings")]
	[SerializeField]
	[Tooltip("모드 변경 시 재생할 사운드")]
	private string modeChangeSoundKey = "compass_mode_change";

	[SerializeField]
	[Tooltip("목표를 찾을 수 없어 빙글빙글 돌 때 재생할 사운드 (루프)")]
	private string spinSoundKey = "compass_spin";

	[SerializeField]
	[Tooltip("처음 잡을때 소리")]
	private string grabSoundKey = "compass_grab";

	[SerializeField]
	[Tooltip("needle 회전 보간 속도 (도/초)")]
	private float needleRotationSpeed = 180f;

	private PlaySoundResult currentSpinSound;

	private bool wasSpinning;

	private float targetNeedleAngle;

	private float currentNeedleAngle;

	private float yAngle;

	private bool isTakeOut;

	private List<Vector3> teleporterPositions = new List<Vector3>();

	private Vector3? currentTargetPosition;

	private Vector3? debugTargetPosition;

	private Vector3? debugNearestExit;

	private Vector3? debugNavMeshWaypoint;

	private string debugTargetType = "";

	private bool isAvatarIndoor;

	[SerializeField]
	private CompassMode currentMode;

	private List<LootingLevelObject> indoorScraps = new List<LootingLevelObject>();

	private float spinSpeed = 360f;

	private float scrapRefreshTimer;

	private LootingLevelObject cachedMostExpensiveScrap;

	private List<Vector3> cachedNavMeshPath;

	private float navPathRefreshTimer;

	private const float SCRAP_REFRESH_INTERVAL = 2f;

	private const float NAV_PATH_REFRESH_INTERVAL = 5f;

	private const float WAYPOINT_REACH_DISTANCE = 3f;

	public void ToggleAnimatorMode(CompassMode mode)
	{
		if (mode == CompassMode.Exit)
		{
			if (animator != null)
			{
				animator.SetBool("IsExit", value: true);
			}
		}
		else if (animator != null)
		{
			animator.SetBool("IsExit", value: false);
		}
	}

	public override void OnAttachToSocket()
	{
		TakeOut();
	}

	public override void OnDetachFromSocket()
	{
		PutIn();
	}

	private void TakeOut()
	{
		GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
		if (gamePlayScene != null)
		{
			gamePlayScene.OnIndoorOutdoorChanged = (Action<bool>)Delegate.Combine(gamePlayScene.OnIndoorOutdoorChanged, new Action<bool>(OnIndoorOutdoorChanged));
			RefreshPositions(gamePlayScene.IsAvatarIndoor);
		}
		cachedMostExpensiveScrap = null;
		cachedNavMeshPath = null;
		scrapRefreshTimer = 0f;
		navPathRefreshTimer = 0f;
		isTakeOut = true;
	}

	private void PutIn()
	{
		GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
		if (gamePlayScene != null)
		{
			gamePlayScene.OnIndoorOutdoorChanged = (Action<bool>)Delegate.Remove(gamePlayScene.OnIndoorOutdoorChanged, new Action<bool>(OnIndoorOutdoorChanged));
		}
		cachedMostExpensiveScrap = null;
		cachedNavMeshPath = null;
		scrapRefreshTimer = 0f;
		navPathRefreshTimer = 0f;
		StopSpinSound();
		wasSpinning = false;
		isTakeOut = false;
		if (animator != null)
		{
			animator.SetBool("IsGrab", value: false);
		}
	}

	public void OnIndoorOutdoorChanged(bool isIndoor)
	{
		RefreshPositions(isIndoor);
	}

	private void RefreshPositions(bool isIndoor)
	{
		isAvatarIndoor = isIndoor;
		teleporterPositions = Hub.s.pdata.main.GetTeleporterPositions(isIndoor);
		if (currentMode == CompassMode.Treasure)
		{
			cachedMostExpensiveScrap = null;
			cachedNavMeshPath = null;
			scrapRefreshTimer = 0f;
			navPathRefreshTimer = 0f;
		}
		UpdateCompass();
	}

	public void OnToggled(long itemID, bool toggleOn)
	{
		if (base.owner.IsMimic())
		{
			ToggleMode(CompassMode.Exit);
		}
		else if (toggleOn)
		{
			ToggleMode(CompassMode.Treasure);
		}
		else
		{
			ToggleMode(CompassMode.Exit);
		}
	}

	public void OnEnable()
	{
		if (isTakeOut)
		{
			ToggleAnimatorMode(currentMode);
		}
	}

	public void ToggleMode(CompassMode mode)
	{
		if (currentMode == mode)
		{
			if (animator != null)
			{
				animator.SetTrigger("IsFirstGrab");
				animator.SetBool("IsGrab", value: true);
			}
			if (!string.IsNullOrEmpty(grabSoundKey) && Hub.s?.audioman != null)
			{
				Hub.s.audioman.PlaySfx(grabSoundKey, base.transform);
			}
			return;
		}
		if (animator != null)
		{
			animator.SetBool("IsGrab", value: true);
		}
		PlayModeChangeSound();
		ToggleAnimatorMode(mode);
		if (mode == CompassMode.Treasure)
		{
			currentMode = CompassMode.Treasure;
			cachedMostExpensiveScrap = null;
			cachedNavMeshPath = null;
			scrapRefreshTimer = 0f;
			navPathRefreshTimer = 0f;
			RefreshTreasureTarget();
		}
		else
		{
			currentMode = CompassMode.Exit;
			cachedMostExpensiveScrap = null;
			cachedNavMeshPath = null;
			navPathRefreshTimer = 0f;
		}
		UpdateCompass();
	}

	private void RefreshIndoorScraps()
	{
		indoorScraps.Clear();
		GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
		if (gamePlayScene == null)
		{
			return;
		}
		foreach (LevelObject item in gamePlayScene.CollectLevelObjects())
		{
			if (item is LootingLevelObject lootingLevelObject && lootingLevelObject.gameObject.activeSelf && !lootingLevelObject.isFake && lootingLevelObject.IsVendingMachineExchange() && IsPositionIndoor(lootingLevelObject.transform.position))
			{
				indoorScraps.Add(lootingLevelObject);
			}
		}
	}

	private bool IsPositionIndoor(Vector3 position)
	{
		SpeechType_Area areaType;
		return Hub.s.dLAcademyManager.GetAreaForDL(position, out areaType) != 1;
	}

	private LootingLevelObject FindMostExpensiveIndoorScrap()
	{
		if (indoorScraps.Count == 0)
		{
			return null;
		}
		LootingLevelObject result = null;
		int num = 0;
		foreach (LootingLevelObject indoorScrap in indoorScraps)
		{
			if (!(indoorScrap == null) && indoorScrap.gameObject.activeSelf && indoorScrap.marketPrice > num)
			{
				num = indoorScrap.marketPrice;
				result = indoorScrap;
			}
		}
		return result;
	}

	private void UpdateCompass()
	{
		Vector3 position = base.owner.transform.position;
		Vector3? vector = null;
		if (currentMode == CompassMode.Treasure)
		{
			if (!isAvatarIndoor)
			{
				debugTargetType = "보물 찾기 실패 (실외)";
				cachedMostExpensiveScrap = null;
				cachedNavMeshPath = null;
				navPathRefreshTimer = 0f;
			}
			else
			{
				LootingLevelObject lootingLevelObject = cachedMostExpensiveScrap;
				if (lootingLevelObject != null && (lootingLevelObject.gameObject == null || !lootingLevelObject.gameObject.activeSelf))
				{
					cachedMostExpensiveScrap = null;
					cachedNavMeshPath = null;
					navPathRefreshTimer = 0f;
					lootingLevelObject = null;
				}
				if (lootingLevelObject != null)
				{
					vector = lootingLevelObject.transform.position;
					debugTargetType = $"보물 (${lootingLevelObject.marketPrice})";
				}
				else
				{
					debugTargetType = "보물 찾기 실패 (스크랩 없음)";
				}
			}
		}
		else
		{
			Vector3? vector2 = null;
			float num = float.MaxValue;
			foreach (Vector3 teleporterPosition in teleporterPositions)
			{
				float sqrMagnitude = (position - teleporterPosition).sqrMagnitude;
				if (sqrMagnitude < num)
				{
					num = sqrMagnitude;
					vector2 = teleporterPosition;
				}
			}
			if (vector2.HasValue)
			{
				vector = vector2.Value;
				float num2 = Mathf.Sqrt(num);
				debugTargetType = $"출구 ({num2:F1}m)";
				debugNearestExit = vector2;
			}
			else
			{
				vector = null;
				debugTargetType = "출구 없음";
				debugNearestExit = null;
			}
		}
		currentTargetPosition = vector;
		debugTargetPosition = vector;
		if (vector.HasValue)
		{
			Vector3 vector3 = vector.Value;
			if (currentMode == CompassMode.Treasure)
			{
				debugNavMeshWaypoint = null;
				if (cachedNavMeshPath != null && cachedNavMeshPath.Count > 1)
				{
					if (Vector3.Distance(position, cachedNavMeshPath[1]) < 3f)
					{
						cachedNavMeshPath.RemoveAt(0);
					}
					if (cachedNavMeshPath != null && cachedNavMeshPath.Count > 1)
					{
						vector3 = cachedNavMeshPath[1];
						debugNavMeshWaypoint = cachedNavMeshPath[1];
					}
				}
			}
			Vector3 vector4 = vector3 - position;
			vector4.y = 0f;
			Vector3 forward = base.owner.transform.forward;
			float num3 = Vector3.Angle(vector4, forward);
			if (Vector3.Cross(forward, vector4).y < 0f)
			{
				num3 = 0f - num3;
			}
			float num4 = (num3 + 270f + 360f + 15f) % 360f;
			targetNeedleAngle = num4;
		}
		else
		{
			debugNavMeshWaypoint = null;
		}
	}

	private void Update()
	{
		if (!isTakeOut)
		{
			return;
		}
		if (currentMode == CompassMode.Treasure && isAvatarIndoor)
		{
			scrapRefreshTimer += Time.deltaTime;
			if (scrapRefreshTimer >= 2f)
			{
				scrapRefreshTimer = 0f;
				RefreshTreasureTarget();
			}
			if (cachedMostExpensiveScrap != null)
			{
				navPathRefreshTimer += Time.deltaTime;
				if (navPathRefreshTimer >= 5f)
				{
					navPathRefreshTimer = 0f;
					RefreshNavMeshPath();
					UpdateCompass();
				}
			}
		}
		if (!currentTargetPosition.HasValue)
		{
			yAngle += spinSpeed * Time.deltaTime;
			yAngle %= 360f;
			needle.transform.localRotation = Quaternion.Euler(0f, yAngle, 0f);
			currentNeedleAngle = yAngle;
			if (!wasSpinning)
			{
				StartSpinSound();
				wasSpinning = true;
			}
			return;
		}
		if (wasSpinning)
		{
			StopSpinSound();
			wasSpinning = false;
		}
		if (base.owner.isRotated || base.owner.isMoved)
		{
			UpdateCompass();
		}
		currentNeedleAngle = Mathf.MoveTowardsAngle(currentNeedleAngle, targetNeedleAngle, needleRotationSpeed * Time.deltaTime);
		needle.transform.localRotation = Quaternion.Euler(0f, currentNeedleAngle, 0f);
	}

	private void RefreshTreasureTarget()
	{
		RefreshIndoorScraps();
		LootingLevelObject lootingLevelObject = FindMostExpensiveIndoorScrap();
		if (lootingLevelObject != cachedMostExpensiveScrap)
		{
			cachedMostExpensiveScrap = lootingLevelObject;
			cachedNavMeshPath = null;
			navPathRefreshTimer = 0f;
			if (lootingLevelObject != null)
			{
				RefreshNavMeshPath();
			}
			UpdateCompass();
		}
	}

	private void RefreshNavMeshPath()
	{
		if (cachedMostExpensiveScrap == null)
		{
			return;
		}
		if (base.owner == null || cachedMostExpensiveScrap.transform == null)
		{
			cachedNavMeshPath = null;
			return;
		}
		Vector3 position = base.owner.transform.position;
		Vector3 position2 = cachedMostExpensiveScrap.transform.position;
		Vector3 nearestPointOnNavMesh = Hub.s.navman.GetNearestPointOnNavMesh(position2, 2f);
		position2 = nearestPointOnNavMesh;
		PathFindResult route = Hub.s.navman.GetRoute(position, position2);
		if (route.Success)
		{
			cachedNavMeshPath = route.PathPoints;
		}
		else
		{
			cachedNavMeshPath = null;
		}
	}

	private void PlayModeChangeSound()
	{
		if (!string.IsNullOrEmpty(modeChangeSoundKey) && Hub.s?.audioman != null)
		{
			Hub.s.audioman.PlaySfx(modeChangeSoundKey, base.transform);
		}
	}

	private void StartSpinSound()
	{
		if (!string.IsNullOrEmpty(spinSoundKey) && Hub.s?.audioman != null)
		{
			currentSpinSound = Hub.s.audioman.PlaySfxTransform(spinSoundKey, base.transform);
		}
	}

	private void StopSpinSound()
	{
		if (currentSpinSound?.ActingVariation != null)
		{
			currentSpinSound.ActingVariation.Stop();
			currentSpinSound = null;
		}
	}
}
