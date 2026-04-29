using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using UnityEngine;

public class CommonChargerLevelObject : StaticLevelObject
{
	[Header("Delay Relative")]
	[SerializeField]
	public float delayForChargingSec;

	[Header("Charge State Relative")]
	[SerializeField]
	private string NeedToChargeText = "NeedToCharge";

	[SerializeField]
	private string ChargingText = "Charging";

	[SerializeField]
	private string FullChargedText = "FullCharged";

	[SerializeField]
	private string GuideText = "CHARGER_LEVEL_OBJECT_GUIDE";

	[SerializeField]
	private string ChargingAnimationName = "Charging";

	[SerializeField]
	private string UnchargingAnimationName = "Uncharging";

	[SerializeField]
	[Tooltip("같은 state를 키로 넣으면 항목이 실행될 지 보장하지 않습니다.")]
	private List<StateAction<ChargingState>> stateActions = new List<StateAction<ChargingState>>();

	private CancellationTokenSource ctsForChargingStart;

	[SerializeField]
	private GameObject chargingEffect;

	[SerializeField]
	private GameObject fullChargedEffect;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Electronics_05;

	public ChargingState BatteryChargingState => (ChargingState)base.State;

	public override bool ForServer => true;

	private void Awake()
	{
		LoadStateActionsToMap(stateActions);
	}

	public void Start()
	{
		base.crossHairType = CrosshairType.BatteryChargeable;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	public override CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		UpdateCrosshairType(protoActor);
		return base.crossHairType;
	}

	private IEnumerator TryStartCharging(ProtoActor protoActor, CancellationTokenSource cts)
	{
		if (animator != null)
		{
			animator.CrossFade(ChargingAnimationName, 0.3f);
		}
		DateTime waitTime = Hub.s.timeutil.GetTimestamp() + TimeSpan.FromSeconds(delayForChargingSec);
		yield return new WaitUntil(() => Hub.s.timeutil.IsAfter(waitTime) || cts.IsCancellationRequested);
		if (cts.IsCancellationRequested)
		{
			yield break;
		}
		ChangeLevelObjectState(levelObjectID, base.State, occupy: false, CancellationToken.None, delegate(int newState, UseLevelObjectRes? res)
		{
			if (!(this == null))
			{
				if (res != null && res.errorCode == MsgErrorCode.Success)
				{
					if (animator != null)
					{
						animator.CrossFade(UnchargingAnimationName, 0.3f);
					}
					if (Hub.s != null)
					{
						if (triggerMasterAudioKey.Length > 0)
						{
							if (Hub.s.audioman != null)
							{
								Hub.s.audioman.PlaySfx(triggerMasterAudioKey, base.transform);
							}
						}
						else if (triggerAudioKey.Length > 0 && Hub.s.legacyAudio != null)
						{
							Hub.s.legacyAudio.Play(triggerAudioKey, triggerAudioSource);
						}
						base.crossHairType = CrosshairType.BatteryFullCharged;
						if (chargingEffect != null)
						{
							chargingEffect.SetActive(value: false);
						}
						if (fullChargedEffect != null)
						{
							fullChargedEffect.SetActive(value: true);
							fullChargedEffect.GetComponent<ParticleSystem>().Play();
						}
						if (ctsForChargingStart != null)
						{
							ctsForChargingStart.Dispose();
							ctsForChargingStart = null;
						}
						UpdateCrosshairType(protoActor);
					}
				}
				else
				{
					Logger.RError($"Charge failed: {res?.errorCode}");
				}
			}
		});
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (protoActor.IsCurrentChargeableItem() && protoActor.IsCurrentNeedToCharge())
		{
			base.crossHairType = CrosshairType.BatteryCharging;
			ctsForChargingStart = new CancellationTokenSource();
			StartCoroutine(TryStartCharging(protoActor, ctsForChargingStart));
			if (chargingEffect != null)
			{
				chargingEffect.SetActive(value: true);
			}
		}
		else if (protoActor.IsCurrentChargeableItem() && !protoActor.IsCurrentNeedToCharge() && fullChargedEffect != null)
		{
			fullChargedEffect.SetActive(value: true);
			fullChargedEffect.GetComponent<ParticleSystem>().Play();
		}
		return true;
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (ctsForChargingStart != null && !ctsForChargingStart.IsCancellationRequested)
		{
			if (animator != null)
			{
				animator.CrossFade(UnchargingAnimationName, 0.3f);
			}
			ctsForChargingStart.Cancel();
			ctsForChargingStart.Dispose();
			ctsForChargingStart = null;
		}
		if (chargingEffect != null)
		{
			chargingEffect.SetActive(value: false);
		}
		UpdateCrosshairType(protoActor);
		return true;
	}

	private void UpdateCrosshairType(ProtoActor protoActor)
	{
		if (ctsForChargingStart != null && !ctsForChargingStart.IsCancellationRequested)
		{
			base.crossHairType = CrosshairType.BatteryCharging;
		}
		else if (protoActor != null)
		{
			if (protoActor.IsCurrentNeedToCharge())
			{
				base.crossHairType = CrosshairType.BatteryChargeable;
			}
			else if (protoActor.IsCurrentChargeableItem())
			{
				base.crossHairType = CrosshairType.BatteryFullCharged;
			}
			else
			{
				base.crossHairType = CrosshairType.BatteryNonChargeable;
			}
		}
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		return base.crossHairType switch
		{
			CrosshairType.BatteryChargeable => Hub.GetL10NText(NeedToChargeText), 
			CrosshairType.BatteryCharging => Hub.GetL10NText(ChargingText), 
			CrosshairType.BatteryFullCharged => Hub.GetL10NText(FullChargedText), 
			CrosshairType.BatteryNonChargeable => Hub.GetL10NText(GuideText), 
			_ => string.Empty, 
		};
	}
}
