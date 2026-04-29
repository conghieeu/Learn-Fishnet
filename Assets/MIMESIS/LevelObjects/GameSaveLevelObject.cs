using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluNetwork.ConstEnum;
using ReluProtocol.Enum;
using UnityEngine;

public class GameSaveLevelObject : StaticLevelObject
{
	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<GameSaveState>> stateActions = new List<StateAction<GameSaveState>>();

	private CancellationToken destroyToken;

	private ProtoActor? saveProtoActor;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.GameSaver;

	private GameSaveState gameSaveState => (GameSaveState)base.State;

	public override bool ForServer => true;

	private void Awake()
	{
		base.State = 0;
		base.InitialState = base.State;
		LoadStateActionsToMap(stateActions);
	}

	private void Start()
	{
		base.crossHairType = CrosshairType.Saver;
		destroyToken = base.destroyCancellationToken;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (Time.time - lastTransitionTime >= minTransitionInterval && gameSaveState == GameSaveState.None)
		{
			return Hub.s.pdata.ClientMode == NetworkClientMode.Host;
		}
		return false;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		if (newState == 1)
		{
			saveProtoActor = protoActor;
		}
		ChangeLevelObjectState(levelObjectID, newState, occupy: false, CancellationToken.None, delegate
		{
		});
	}

	private IEnumerator CorOnSaving(int prevState, int newState)
	{
		if (Hub.s.pdata == null)
		{
			yield break;
		}
		base.crossHairType = CrosshairType.Saving;
		if (Hub.s.pdata.ClientMode != NetworkClientMode.Host)
		{
			yield break;
		}
		Hub.s.pdata.main?.OnSaving(saveProtoActor, auto: false);
		if (HasStateActionTransition(prevState, newState, out StateActionInfo stateActionInfo))
		{
			PlayTriggerSound(prevState, newState);
			AnimateObject(prevState, newState);
			yield return new WaitForSeconds(stateActionInfo.transitionDurtaion);
		}
		List<ProtoActor> obj = Hub.s.pdata.main?.GetAllPlayers();
		List<string> list = new List<string>();
		foreach (ProtoActor item2 in obj)
		{
			string item = ((item2 != null) ? Hub.s.pdata.main?.ResolveNickName(item2, item2.nickName) : string.Empty);
			list.Add(item);
		}
		Hub.s.pdata.main?.SendPacketWithCallback<SaveGameDataRes>(new SaveGameDataReq
		{
			SlotID = Hub.s.pdata.SaveSlotID,
			PlayerNames = list
		}, OnSavingCallback, destroyToken);
	}

	private IEnumerator CorOnEndSaving(ProtoActor protoActor, int prevState, int newState)
	{
		if (Hub.s.pdata != null && Hub.s.pdata.ClientMode == NetworkClientMode.Host)
		{
			ChangeLevelObjectState(levelObjectID, newState, occupy: false, CancellationToken.None, delegate
			{
			});
		}
		yield break;
	}

	private void OnSavingCallback(SaveGameDataRes _res)
	{
		if (_res != null)
		{
			int state = base.State;
			int newState = 2;
			if (_res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"[SaveGameDataRes] SaveGameDataRes : {_res.errorCode}");
				List<string> list = new List<string> { Hub.GetL10NText("STRING_SAVE_OBJECT_SAVE_FAILED") };
				MMUIPrefabTable uiprefabs = Hub.s.tableman.uiprefabs;
				object[] contentsArgs = list.ToArray();
				uiprefabs.ShowTimerDialog("ToastSimple", 0f, contentsArgs);
				newState = 0;
			}
			StartCoroutine(CorOnEndSaving(saveProtoActor, state, newState));
		}
	}

	private IEnumerator CorOnSaved(int prevState, int newState)
	{
		base.crossHairType = CrosshairType.Saver;
		if (HasStateActionTransition(prevState, newState, out StateActionInfo stateActionInfo))
		{
			PlayTriggerSound(prevState, newState);
			AnimateObject(prevState, newState);
			yield return new WaitForSeconds(stateActionInfo.transitionDurtaion);
		}
		if (Hub.s.pdata.ClientMode == NetworkClientMode.Host)
		{
			ChangeLevelObjectState(levelObjectID, 0, occupy: false, CancellationToken.None, delegate
			{
			});
			saveProtoActor = null;
		}
	}

	private IEnumerator CorOnEndSaved(int prevState, int newState)
	{
		base.crossHairType = CrosshairType.Saver;
		if (HasStateActionTransition(prevState, newState, out StateActionInfo stateActionInfo))
		{
			PlayTriggerSound(prevState, newState);
			AnimateObject(prevState, newState);
			yield return new WaitForSeconds(stateActionInfo.transitionDurtaion);
		}
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int CurrentState)
	{
		Logger.RLog($"GameSaveLevelObject OnChangeLevelObjectStateSig: {prevState} -> {CurrentState}");
		base.OnChangeLevelObjectStateSig(actorId, occupiedActorID, prevState, CurrentState);
		switch (CurrentState)
		{
		case 1:
			StartCoroutine(CorOnSaving(prevState, CurrentState));
			break;
		case 2:
			StartCoroutine(CorOnSaved(prevState, CurrentState));
			break;
		default:
			StartCoroutine(CorOnEndSaved(prevState, CurrentState));
			break;
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (gameSaveState == GameSaveState.Saving)
		{
			return Hub.GetL10NText("STRING_SAVING_OBJECT_HOST");
		}
		if (gameSaveState == GameSaveState.Saved)
		{
			return Hub.GetL10NText("STRING_SAVE_OBJECT_HOST_SAVED");
		}
		if (Hub.s.pdata.ClientMode == NetworkClientMode.Host)
		{
			return Hub.GetL10NText("STRING_SAVE_OBJECT_HOST");
		}
		return Hub.GetL10NText("STRING_SAVE_OBJECT_GUEST");
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (IsTriggerable(protoActor, 1))
		{
			TriggerAction(protoActor, 1);
		}
		return true;
	}
}
