using System;
using System.Collections.Generic;
using System.Threading;
using MMBehaviorTree;
using MimicData;
using Newtonsoft.Json;
using UnityEngine;
using UnityEngine.Diagnostics;

public class DataManager : MonoBehaviour
{
	private ExcelDataManager _excelDataManager;

	private ResourceDataHandler _dataHandler;

	private AIDataManager? _aiDataManager;

	private AnimNotifyDataManager _animNotiManager;

	public static string staticFilePath = Application.streamingAssetsPath ?? "";

	public readonly long c_AggroRecoveryWaitTime = 3000L;

	public readonly long c_AggroDecrementTime = 1000L;

	public readonly int c_AggroDecrementPointPerSecond = 1;

	public allData data { get; private set; }

	public ExcelDataManager ExcelDataManager => _excelDataManager;

	public AnimNotifyDataManager AnimNotiManager => _animNotiManager;

	public AIDataManager AIDataManager => _aiDataManager ?? throw new InvalidOperationException("AIDataManager is not initialized yet. Call InitAIData() first.");

	private void Awake()
	{
		Logger.RLog("[AwakeLogs] DataManager.Awake ->");
		data = new allData();
		_dataHandler = new ResourceDataHandler();
		_dataHandler.InitLocal(staticFilePath);
		_excelDataManager = new ExcelDataManager(_dataHandler);
		if (!_excelDataManager.Initialize())
		{
			CrashClient();
			return;
		}
		_animNotiManager = new AnimNotifyDataManager(_dataHandler);
		if (!_animNotiManager.LoadAnimNotify())
		{
			CrashClient();
			return;
		}
		InitAIData();
		TextAsset[] array = Resources.LoadAll<TextAsset>("data/puppetAnimationInfo");
		foreach (TextAsset textAsset in array)
		{
			try
			{
				if (textAsset != null)
				{
					animationInfoData.PuppetAnimationInfo item = JsonConvert.DeserializeObject<animationInfoData.PuppetAnimationInfo>(textAsset.text);
					data.animationInfoDatas.puppetAnimationInfos.Add(item);
				}
			}
			catch (Exception arg)
			{
				Logger.RError($"critical error at DataManager.Awake / msg: {arg}");
				CrashClient();
				return;
			}
		}
		Logger.RLog("[AwakeLogs] DataManager.Awake <-");
	}

	public void InitAIData()
	{
		AIDataManager aIDataManager = new AIDataManager(_dataHandler);
		(List<Type> btActions, List<Type> btConditions) tuple = InitBTType();
		List<Type> item = tuple.btActions;
		List<Type> item2 = tuple.btConditions;
		BehaviorTreeBuilder.BuilderConfig config = new BehaviorTreeBuilder.BuilderConfig(item, item2);
		if (!aIDataManager.LoadLocalData(config))
		{
			CrashClient();
		}
		else
		{
			Interlocked.Exchange(ref _aiDataManager, aIDataManager);
		}
	}

	private void OnDestroy()
	{
		_excelDataManager.Dispose();
		_aiDataManager.Dispose();
	}

	private (List<Type> btActions, List<Type> btConditions) InitBTType()
	{
		List<Type> item = new List<Type>
		{
			typeof(ClearBlackBoard),
			typeof(CopyInventory),
			typeof(PickInvenSlot),
			typeof(UsePickedInvenItem),
			typeof(MoveToRandomPos),
			typeof(MoveToTarget),
			typeof(PickLevelObject),
			typeof(ChangeLevelObjectState),
			typeof(PickTarget),
			typeof(RunDLMovementAgent),
			typeof(RemoveBlackBoardParam),
			typeof(SetBlackBoardParam),
			typeof(Wait),
			typeof(UseSkill),
			typeof(PickSkill),
			typeof(ChangeBT),
			typeof(Emote),
			typeof(SetTrace),
			typeof(MoveToLevelObject),
			typeof(DetachActor),
			typeof(MoveToPickedPosition),
			typeof(PickRandomTargetPoint),
			typeof(ResetTargetPosition),
			typeof(EnableDLAgent),
			typeof(ResetDLDecision),
			typeof(ResetDLTimeToAttackTimer),
			typeof(SetFakeItem),
			typeof(SetVoiceRule),
			typeof(ResetTargetLevelObject),
			typeof(ResetTargetActor),
			typeof(UseTransmitter),
			typeof(AddFaction),
			typeof(ResetFaction),
			typeof(SetChargingCompleted),
			typeof(MMBehaviorTree.Ping),
			typeof(SetDetectionRange),
			typeof(ResetDetectionRange),
			typeof(Stop),
			typeof(ReleaseItem),
			typeof(SetHandEmpty),
			typeof(PickTeleportMarker),
			typeof(UseTeleportMarker),
			typeof(SetRotation),
			typeof(ResetUseScrapScanner)
		};
		List<Type> item2 = new List<Type>
		{
			typeof(CheckBlackBoardParam),
			typeof(ExistTarget),
			typeof(IsArrived),
			typeof(CheckDistanceToTarget),
			typeof(CheckDistanceToPickedTeleporter),
			typeof(IsSkillUsableRange),
			typeof(CheckBTActivateTime),
			typeof(CheckTargetCountInRadius),
			typeof(CheckPlayerCountInRadius),
			typeof(CheckMaxAggroValue),
			typeof(IsLevelObjectInRange),
			typeof(AnyActorAttaching),
			typeof(IsAttached),
			typeof(CheckDLDecision),
			typeof(CheckDeadPlayerCount),
			typeof(CheckHP),
			typeof(CheckPickedItemType),
			typeof(AnyActorWatchingInRadius),
			typeof(CheckAIState),
			typeof(CheckPickedLevelObjectState),
			typeof(IsInDoor)
		};
		return (btActions: item, btConditions: item2);
	}

	public float? GetAnimationLength(string puppetName, string clipName)
	{
		animationInfoData.PuppetAnimationInfo puppetAnimationInfo = data.animationInfoDatas.puppetAnimationInfos.Find((animationInfoData.PuppetAnimationInfo x) => x.name == puppetName);
		if (puppetAnimationInfo != null)
		{
			animationInfoData.PuppetAnimationInfo.Clip clip = puppetAnimationInfo.clips.Find((animationInfoData.PuppetAnimationInfo.Clip x) => x.name == clipName);
			if (clip != null)
			{
				return clip.length;
			}
		}
		return null;
	}

	private void CrashClient()
	{
		Utils.ForceCrash(ForcedCrashCategory.FatalError);
	}
}
