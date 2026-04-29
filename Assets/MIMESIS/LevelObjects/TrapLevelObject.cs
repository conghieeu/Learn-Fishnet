using System;
using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using TMPro;
using UnityEngine;

public class TrapLevelObject : StaticLevelObject
{
	public TrapState initialState;

	[SerializeField]
	private TrapType _trapType;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<TrapState>> stateActions = new List<StateAction<TrapState>>();

	[SerializeField]
	private TextMeshPro debugText;

	[SerializeField]
	private List<TrapStateText> stateTexts;

	private CancellationTokenSource? ctsForServerRequest;

	private List<Action<TrapState>> onTrapStateChanged = new List<Action<TrapState>>();

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Trap;

	public TrapType TrapType => _trapType;

	public TrapState TrapState => (TrapState)base.State;

	public string TrapName => levelObjectName;

	public override bool ForServer => true;

	private void Awake()
	{
		base.State = (int)initialState;
		base.InitialState = base.State;
		LoadStateActionsToMap(stateActions);
		ChangeClientState(base.State);
		OnUpdateTrapState();
		debugText?.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		base.crossHairType = CrosshairType.None;
	}

	private void OnDestroy()
	{
		if (ctsForServerRequest != null)
		{
			if (!ctsForServerRequest.IsCancellationRequested)
			{
				ctsForServerRequest.Cancel();
			}
			ctsForServerRequest.Dispose();
			ctsForServerRequest = null;
		}
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_trap", allowScaling: true, iconColor);
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		return false;
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		return false;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return "";
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int OccupiedActorID, int prevState, int currentState)
	{
		base.OnChangeLevelObjectStateSig(actorId, OccupiedActorID, prevState, currentState);
		PlayTriggerSound(prevState, currentState);
		AnimateObject(prevState, currentState);
		OnUpdateTrapState();
		onTrapStateChanged.ForEach(delegate(Action<TrapState> e)
		{
			e(TrapState);
		});
	}

	public void AddOnTrapStateChanged(Action<TrapState> action)
	{
		onTrapStateChanged.Add(action);
	}

	private void OnUpdateTrapState()
	{
		if (debugText != null)
		{
			debugText.text = $"TestTrap\n( {TrapName} )\nInitialState: {base.InitialState}\nCurrentState: {TrapState}";
		}
	}
}
