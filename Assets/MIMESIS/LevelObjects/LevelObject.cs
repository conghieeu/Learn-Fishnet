using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using Mimic.Actors;
using Mimic.Animation;
using ReluProtocol;
using UnityEngine;

public abstract class LevelObject : MonoBehaviour
{
	[Serializable]
	protected class StateAction<T> where T : Enum
	{
		public T FromState;

		public T ToState;

		public StateActionInfo stateActionInfo = new StateActionInfo();

		public StateAction(T initialState)
		{
			ToState = initialState;
		}
	}

	[Serializable]
	public class AnimationInfo
	{
		public Animator animator;

		public string animatorStateName = string.Empty;

		public string animatorStateNameForCancel = string.Empty;
	}

	[Serializable]
	public class StateActionInfo
	{
		[Tooltip("상태 실행 남은 횟수. -1:무한, 0이상이면 실행될 때마다 1 감소하고 0이 되면 더 이상 실행되지 않는다. 서버내부값이 실제 적용되고 여기값은 설정용으로 사용한다")]
		public int actionRemainingCount = -1;

		[EditStringList(',')]
		public string condition = string.Empty;

		[EditStringList(',')]
		public string action = string.Empty;

		[Tooltip("액션 실행 딜레이. 상태 전환이 적용된 다음 지정된 시간을 기다려서 액션들을 실행한다. ms")]
		public int delay;

		[Header("↓↓↓ CLIENT ONLY ↓↓↓")]
		public bool triggerableByClient = true;

		public string animatorStateName = string.Empty;

		public string animatorStateNameForCancel = string.Empty;

		public string audiokey = string.Empty;

		public string masterAudioKey = string.Empty;

		[Tooltip("클라이언트에서 action 수행 딜레이를 위한 값. 정확하겐 연출을 위해 delay시키는 값, delay 필드는 서버에서 작동하는 값")]
		public float transitionDurtaion;

		[Tooltip("true인 경우 TransitionDuration 후 action을 수행, false인 경우 Action을 바로 수행.")]
		public bool triggerWhenTransitionComplete;

		[Tooltip("false인 경우 트랜지션 종료시 animation 한다.")]
		public bool animateWhenTransitionStarted;

		[Tooltip("transitionDuration 안에 표시할 crosshairType")]
		public CrosshairType crosshairTypeWhenTransition;

		public List<AnimationInfo> animationInfos;
	}

	[Tooltip("트램 업그레이드 파츠처럼 비활성화된 레벨오브젝트지만 LevelObjectID를 수집하도록 하려면 true")]
	public bool assignIDWhenInactive;

	[Tooltip("이제 자동생성됨. 수정 불가")]
	[InspectorReadOnly]
	public int levelObjectID;

	public string levelObjectName;

	public float maxInteractionDistance = 1f;

	public LevelObjectBoundElement[] bounds;

	public Color iconColor = Color.white;

	[SerializeField]
	private MapMarker_AIHandle? mapMarkerAIHandle;

	private ImmutableList<LevelObjectBoundElement> _boundElementImmutableList;

	private PosWithRot pos = new PosWithRot();

	public CrosshairType crossHairType { get; protected set; }

	public abstract LevelObjectClientType LevelObjectType { get; }

	public int InitialState { get; protected set; }

	public int State { get; protected set; }

	public bool DisableReverseState { get; protected set; }

	public virtual bool ForServer => false;

	public Dictionary<int, Dictionary<int, StateActionInfo>> StateActionsMap { get; private set; } = new Dictionary<int, Dictionary<int, StateActionInfo>>();

	public ImmutableList<LevelObjectBoundElement> BoundElementImmutableList
	{
		get
		{
			if (_boundElementImmutableList == null)
			{
				_boundElementImmutableList = bounds.ToImmutableList();
			}
			return _boundElementImmutableList;
		}
	}

	public bool IsIndoor
	{
		get
		{
			if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main is GamePlayScene gamePlayScene)
			{
				return gamePlayScene.CheckPosIsIndoor(base.transform.position);
			}
			return false;
		}
	}

	public PosWithRot Pos
	{
		get
		{
			pos.x = base.transform.position.x;
			pos.y = base.transform.position.y;
			pos.z = base.transform.position.z;
			pos.yaw = base.transform.rotation.eulerAngles.y;
			return pos;
		}
	}

	private void Start()
	{
		crossHairType = CrosshairType.None;
	}

	public virtual bool NeedToShowCrossHair(ProtoActor protoActor)
	{
		return base.gameObject.activeInHierarchy;
	}

	public virtual CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		return crossHairType;
	}

	public virtual float GetCrossHairAnimDuration()
	{
		return 0f;
	}

	public abstract bool TryInteract(ProtoActor protoActor);

	public virtual bool TryInteractEnd(ProtoActor protoActor)
	{
		return true;
	}

	public virtual string GetSimpleText(ProtoActor protoActor)
	{
		return "";
	}

	public virtual string GetAddtionalSimpleText(ProtoActor protoActor)
	{
		return "";
	}

	public virtual void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int currentState)
	{
	}

	public void OnAnimationEvent(string statement)
	{
		AnimationEventHandler.Execute(base.gameObject, statement, base.gameObject.transform);
	}

	public Vector3? GetAIHandlePos()
	{
		if (mapMarkerAIHandle == null)
		{
			return null;
		}
		return mapMarkerAIHandle.transform.position;
	}

	public void internalOnly_AdjustAIHandler()
	{
		if (!(mapMarkerAIHandle == null) && !(Hub.s == null) && !(Hub.s.navman == null))
		{
			Vector3 nearestPointOnNavMesh = Hub.s.navman.GetNearestPointOnNavMesh(mapMarkerAIHandle.transform.position, 0.5f);
			if (nearestPointOnNavMesh != Vector3.zero)
			{
				mapMarkerAIHandle.transform.position = nearestPointOnNavMesh;
			}
		}
	}

	public virtual void OnDrawGizmos()
	{
		if (bounds == null)
		{
			return;
		}
		LevelObjectBoundElement[] array = bounds;
		foreach (LevelObjectBoundElement levelObjectBoundElement in array)
		{
			if (!(levelObjectBoundElement == null))
			{
				levelObjectBoundElement.DrawGizmos(iconColor);
			}
		}
	}
}
