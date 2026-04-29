using System.Collections;
using System.Threading;
using Mimic.Actors;
using UnityEngine;

public class ChargerLevelObject : SwitchLevelObject
{
	[Header("Delay Relative")]
	[SerializeField]
	public float delayForChargingSec;

	[SerializeField]
	private string NeedToChargeText = "NeedToCharge";

	[SerializeField]
	private string ChargingText = "Charging";

	[SerializeField]
	private string FullChargedText = "FullCharged";

	private CancellationTokenSource ctsForChargingStart;

	public override LevelObjectClientType LevelObjectType { get; } = LevelObjectClientType.Switch;

	public override bool ForServer => true;

	public void Start()
	{
		base.crossHairType = CrosshairType.BatteryChargeable;
	}

	public override void OnDrawGizmos()
	{
		base.OnDrawGizmos();
		Gizmos.DrawIcon(base.transform.position, "icon_button", allowScaling: true, iconColor);
	}

	public override bool NeedToShowCrossHair(ProtoActor protoActor)
	{
		if (base.gameObject.activeSelf)
		{
			return protoActor.IsCurrentChargeableItem();
		}
		return false;
	}

	public override CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		UpdateCrosshairType(protoActor);
		return base.crossHairType;
	}

	private IEnumerator TryStartCharging(ProtoActor protoActor, CancellationTokenSource cts)
	{
		yield return new WaitForSeconds(delayForChargingSec);
		if (cts.IsCancellationRequested || !(protoActor != null))
		{
			yield break;
		}
		protoActor.TryStartCharge(delegate
		{
			if (triggerMasterAudioKey.Length > 0)
			{
				Hub.s.audioman.PlaySfxAtTransform(triggerMasterAudioKey, base.transform);
			}
			else if (triggerAudioKey.Length > 0)
			{
				Hub.s.legacyAudio.Play(triggerAudioKey, triggerAudioSource);
			}
			base.crossHairType = CrosshairType.BatteryFullCharged;
			ctsForChargingStart.Dispose();
			ctsForChargingStart = null;
			UpdateCrosshairType(protoActor);
		});
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		if (protoActor.IsCurrentChargeableItem() && protoActor.IsCurrentNeedToCharge())
		{
			base.crossHairType = CrosshairType.BatteryCharging;
			ctsForChargingStart = new CancellationTokenSource();
			StartCoroutine(TryStartCharging(protoActor, ctsForChargingStart));
		}
		return true;
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (ctsForChargingStart != null && !ctsForChargingStart.IsCancellationRequested)
		{
			ctsForChargingStart.Cancel();
			ctsForChargingStart.Dispose();
			ctsForChargingStart = null;
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
			else
			{
				base.crossHairType = CrosshairType.BatteryFullCharged;
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
			_ => "", 
		};
	}
}
