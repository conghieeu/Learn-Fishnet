using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using TMPro;
using UnityEngine;

public class TeleporterLevelObject : StaticLevelObject
{
	[SerializeField]
	private TeleporterState initialState = TeleporterState.On;

	[Header("Delay Relative")]
	[SerializeField]
	public float delayForChargingSec;

	[SerializeField]
	private Transform test_destination;

	[SerializeField]
	private TMP_Text objectIDDisplay;

	[SerializeField]
	private string startCallSign;

	[SerializeField]
	private bool destinationIsToInDoor;

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<TeleporterState>> stateActions = new List<StateAction<TeleporterState>>();

	private CancellationTokenSource? ctsForServerRequest;

	[SerializeField]
	private string textInCrossHair;

	[SerializeField]
	protected string teleportMasterAudioKey = "";

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Teleporter;

	public string StartCallSign => startCallSign;

	public bool DestinationIsToInDoor => destinationIsToInDoor;

	public override bool ForServer => true;

	private void Awake()
	{
		base.State = (int)initialState;
		base.InitialState = base.State;
		LoadStateActionsToMap(stateActions);
		ChangeClientState(base.State);
		objectIDDisplay?.gameObject.SetActive(value: false);
	}

	private void Start()
	{
		base.crossHairType = CrosshairType.Teleport;
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

	private void Update()
	{
		if (objectIDDisplay.gameObject.activeInHierarchy && objectIDDisplay.enabled)
		{
			objectIDDisplay.text = $"Teleporter to\n{startCallSign}\n({(TeleporterState)base.State})";
		}
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	public override bool NeedToShowCrossHair(ProtoActor protoActor)
	{
		return true;
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		Trigger(protoActor, 1);
		return true;
	}

	protected override void TriggerAction(ProtoActor protoActor, int newState)
	{
		ChangeLevelObjectState(levelObjectID, base.State, occupy: false, CancellationToken.None, delegate(int num, UseLevelObjectRes? res)
		{
			if (res != null)
			{
				if (res.errorCode == MsgErrorCode.Success)
				{
					if (Hub.s.audioman != null && !string.IsNullOrEmpty(teleportMasterAudioKey))
					{
						Hub.s.audioman.PlaySfx(teleportMasterAudioKey);
					}
					GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
					if (gamePlayScene != null)
					{
						gamePlayScene.MoveToIndoorOrOutdoor(destinationIsToInDoor);
						currentTransition = null;
					}
				}
				else
				{
					Logger.RWarn($"Teleport failed: {res.errorCode}");
					currentTransition = null;
				}
			}
		});
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return Hub.GetL10NText(textInCrossHair);
	}
}
