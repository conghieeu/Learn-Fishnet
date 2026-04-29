using System;
using System.Collections;
using System.Collections.Generic;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;
using UnityEngine.Serialization;

[HelpURL("https://wiki.krafton.com/pages/viewpage.action?pageId=5084410072")]
public class MapTrigger : MonoBehaviour
{
	public enum eShapeType
	{
		None = 0,
		AABB = 1,
		Sphere = 2
	}

	public enum eUsageType
	{
		None = 0,
		Client_SendEvent = 1,
		Client_DirectLink = 2,
		Server_TriggerVolume = 3,
		Server_CollectingVolume = 4,
		ClientOnly_PlaySound = 5,
		ClientOnly_PlayRecordedVoice = 6,
		ClientOnly_PlayVFX = 7,
		ClientOnly_PlayUIUnderlayEffect = 8,
		Server_StartingVolume = 9,
		Server_CanopyVolume = 10,
		ClientOnly_InsideTramVolume = 11,
		Server_VerticalTrackVolume = 12
	}

	[Flags]
	public enum eCheckTypeFlag
	{
		None = 0,
		Enter = 1,
		Inside = 2
	}

	public enum eUIOverlayEffectType
	{
		None = 0,
		BlackOut = 1,
		WhiteOut = 2
	}

	public enum eSoundActorFilter
	{
		Avatar = 0,
		Player = 1,
		Monster = 2,
		ALL = 3
	}

	public delegate void DirectLinkCallback(MapTrigger trigger);

	private DirectLinkCallback clientOnlyDirectLink;

	[SerializeField]
	private eShapeType _shapeType = eShapeType.AABB;

	[SerializeField]
	private eUsageType _usageType;

	[SerializeField]
	private eCheckTypeFlag _checkTypeFlag = eCheckTypeFlag.Enter;

	[SerializeField]
	private Color _debug_color = new Color(0f, 1f, 0f, 0.5f);

	[Header("Server data")]
	[Tooltip("맵 생성시 트리거가 서버에서 활성화될 확률(0~10000)")]
	public int activateRate = 10000;

	public string serverData_conditionString;

	public string serverData_actionString;

	[Tooltip("트리거 엔터시 액션이 실행될 확률(0~10000)")]
	public int actionRate = 10000;

	public int actionDelay;

	[Tooltip("트리거 발동 확률 체크만 해도 쿨타임이 시작하는지 여부. 체크하지 않으면 발동시 쿨타임 시작.")]
	public bool startActionCooltimeOnRateCheck;

	[Tooltip("트리거 발동 후 다음 발동 가능까지 쿨타임. millisec")]
	public int actionCooltime;

	[Tooltip("트리거 발동 제한 횟수. -1 은 무한")]
	public int serverData_repeatCount;

	[Header("ClientOnly 공통")]
	public float clientOnly_delay;

	public float clientOnly_coolTime = 1f;

	[Header("ClientOnly_PlayVFX")]
	public string vfxKey;

	[Header("ClientOnly_PlayUIUnderlayEffect")]
	[FormerlySerializedAs("UIOverlayEffectType")]
	public eUIOverlayEffectType UIUnderlayEffectType;

	[FormerlySerializedAs("UIOverlayEffectStartDuration")]
	public float UIUnderlayEffectStartDuration = 1f;

	[FormerlySerializedAs("UIOverlayEffectSustain")]
	public float UIUnderlayEffectSustain = 1f;

	[FormerlySerializedAs("UIOverlayEffectEndDuration")]
	public float UIUnderlayEffectEndDuration = 1f;

	[Header("ClientOnly_PlaySound")]
	public eSoundActorFilter soundActorFilter = eSoundActorFilter.Player;

	public string soundKey;

	public string masterAudioKey;

	public AudioSource audioSource;

	[Header("ClientOnly_PlayRecordedVoice")]
	[Tooltip("이 값이 지정된 경우 해당 위치를 기준으로 스폰한다. 아닌 경우 기존처럼 MapTrigger의 위치를 사용한다.")]
	public Transform recordedVoiceSpawnTarget;

	public float recordedVoiceRadius = 10f;

	[SerializeField]
	public List<MapMarker_HolePoint> holePoints = new List<MapMarker_HolePoint>();

	private float? clientOnly_coolTimeUntil;

	public eShapeType shapeType => _shapeType;

	public eUsageType usageType => _usageType;

	public eCheckTypeFlag checkTypeFlag => _checkTypeFlag;

	public void RegisterDirectLink(DirectLinkCallback callback)
	{
		clientOnlyDirectLink = (DirectLinkCallback)Delegate.Combine(clientOnlyDirectLink, callback);
	}

	public Bounds? GetBounds()
	{
		switch (_shapeType)
		{
		case eShapeType.AABB:
		{
			Bounds bounds = GetComponent<BoxCollider>().bounds;
			bounds.center = base.transform.TransformPoint(bounds.center);
			bounds.extents = base.transform.TransformVector(bounds.extents);
			return bounds;
		}
		case eShapeType.Sphere:
			return new Bounds(base.transform.position, Vector3.one * GetComponent<SphereCollider>().radius);
		default:
			return null;
		}
	}

	private void OnTriggerEnter(Collider other)
	{
		Hub.s?.dynamicDataMan?.ReportTriggerEvent(this, other.name, isEnter: true);
		clientOnlyDirectLink?.Invoke(this);
		if (usageType == eUsageType.ClientOnly_PlaySound || usageType == eUsageType.ClientOnly_PlayRecordedVoice || usageType == eUsageType.ClientOnly_PlayVFX || usageType == eUsageType.ClientOnly_PlayUIUnderlayEffect)
		{
			StartCoroutine(OnTriggerClientOnly(other));
		}
	}

	private IEnumerator OnTriggerClientOnly(Collider other)
	{
		if (clientOnly_coolTimeUntil.HasValue && Time.time < clientOnly_coolTimeUntil.Value)
		{
			yield break;
		}
		if (clientOnly_delay > 0f)
		{
			yield return new WaitForSeconds(clientOnly_delay);
		}
		if (other == null)
		{
			yield break;
		}
		switch (usageType)
		{
		case eUsageType.ClientOnly_PlayVFX:
			Hub.s.vfxman.InstantiateVfx(vfxKey, base.transform.position);
			break;
		case eUsageType.ClientOnly_PlaySound:
		{
			bool flag = false;
			switch (soundActorFilter)
			{
			case eSoundActorFilter.Avatar:
			{
				ProtoActor component5 = other.GetComponent<ProtoActor>();
				if (component5 != null && component5.AmIAvatar())
				{
					flag = true;
				}
				break;
			}
			case eSoundActorFilter.Player:
			{
				ProtoActor component4 = other.GetComponent<ProtoActor>();
				if (component4 != null && component4.ActorType == ActorType.Player)
				{
					flag = true;
				}
				break;
			}
			case eSoundActorFilter.Monster:
			{
				ProtoActor component3 = other.GetComponent<ProtoActor>();
				if (component3 != null && component3.ActorType == ActorType.Monster)
				{
					flag = true;
				}
				break;
			}
			default:
				flag = true;
				break;
			}
			if (flag)
			{
				if (masterAudioKey != string.Empty)
				{
					Hub.s.audioman.PlaySfx(masterAudioKey, base.transform);
				}
				else
				{
					Hub.s.legacyAudio.Play(soundKey, audioSource);
				}
			}
			break;
		}
		case eUsageType.ClientOnly_PlayUIUnderlayEffect:
		{
			ProtoActor component2 = other.GetComponent<ProtoActor>();
			if (!(component2 == null) && component2.AmIAvatar())
			{
				switch (UIUnderlayEffectType)
				{
				case eUIOverlayEffectType.BlackOut:
					Hub.s.uiman.TriggerUnderlayUIEffect(UIManager.eUIEffectType.PingPong, Color.black, UIUnderlayEffectStartDuration, UIUnderlayEffectEndDuration, UIUnderlayEffectSustain);
					break;
				case eUIOverlayEffectType.WhiteOut:
					Hub.s.uiman.TriggerUnderlayUIEffect(UIManager.eUIEffectType.PingPong, Color.white, UIUnderlayEffectStartDuration, UIUnderlayEffectEndDuration, UIUnderlayEffectSustain);
					break;
				}
			}
			break;
		}
		case eUsageType.ClientOnly_PlayRecordedVoice:
		{
			ProtoActor component = other.GetComponent<ProtoActor>();
			if (!(component == null) && component.AmIAvatar())
			{
				Vector3 center = ((recordedVoiceSpawnTarget != null) ? recordedVoiceSpawnTarget.position : base.transform.position);
				Hub.s.voiceman.TryPlayHalucinationInsideCircle(center, recordedVoiceRadius);
			}
			break;
		}
		}
		clientOnly_coolTimeUntil = Time.time + clientOnly_coolTime + clientOnly_delay;
	}

	private void OnTriggerExit(Collider other)
	{
		Hub.s?.dynamicDataMan?.ReportTriggerEvent(this, other.name, isEnter: false);
	}

	public void OnDrawGizmos()
	{
		if (_shapeType == eShapeType.AABB)
		{
			BoxCollider component = GetComponent<BoxCollider>();
			if (component != null)
			{
				Matrix4x4 matrix = Gizmos.matrix;
				Color color = Gizmos.color;
				Gizmos.color = _debug_color;
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawCube(component.center, component.size);
				Gizmos.color = color;
				Gizmos.matrix = matrix;
			}
		}
		else if (_shapeType == eShapeType.Sphere)
		{
			SphereCollider component2 = GetComponent<SphereCollider>();
			if (component2 != null)
			{
				Matrix4x4 matrix2 = Gizmos.matrix;
				Color color2 = Gizmos.color;
				Gizmos.color = _debug_color;
				Gizmos.matrix = base.transform.localToWorldMatrix;
				Gizmos.DrawSphere(component2.center, component2.radius);
				Gizmos.color = color2;
				Gizmos.matrix = matrix2;
			}
		}
	}
}
