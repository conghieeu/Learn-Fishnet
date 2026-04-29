using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Mimic.Actors;
using ReluProtocol.Enum;
using TMPro;
using UnityEngine;

public class ScrapScanLevelObject : SimpleSwitchLevelObject, ITramUpgradeLevelObject
{
	[Serializable]
	private class HighlightInfo
	{
		public Animator animator;

		public string animatorStateName = "First";
	}

	[Header("Scrap Scan Level Object")]
	[SerializeField]
	private string textKeyScanAvailable = "STRING_TRAMUPGRADE_SCRAPSCANNER_MONITOR_TEXT";

	[SerializeField]
	private string textKeyScanning = "STRING_TRAMUPGRADE_SCRAPSCANNER_MONITOR_TEXT_SCANNING";

	[SerializeField]
	private string textKeyScanNotAvailable = "STRING_TRAMUPGRADE_SCRAPSCANNER_MONITOR_TEXT_NOTAVAILABLE";

	[SerializeField]
	private List<HighlightInfo> highlightInfos;

	[SerializeField]
	private float scanMonitorAnimationDuration = 2f;

	[SerializeField]
	private float scanTextOutDuration = 2f;

	private bool isUpgradeActive;

	[SerializeField]
	private Animator scanMonitorAnimator;

	[SerializeField]
	private string scanMonitorNormalAnim = "";

	[SerializeField]
	private string scanMonitorScanningAnim = "";

	[SerializeField]
	private TMP_Text scanMonitorText;

	[SerializeField]
	private Animator scanAntennaAnimator;

	[SerializeField]
	private string scanAntennaNormalAnim = "";

	[SerializeField]
	private GameObject scanningEffect;

	[SerializeField]
	private string scanningEffectSound = "scanning_effect_sound";

	[SerializeField]
	private int tramUpgradeID = -1;

	public override LevelObjectClientType LevelObjectType => LevelObjectClientType.ScrapScan;

	public bool IsUpgradeActive => isUpgradeActive;

	public int TramUpgradeID => tramUpgradeID;

	public void OnEnable()
	{
		isUpgradeActive = true;
		if (scanMonitorText != null)
		{
			scanMonitorText.enabled = false;
		}
	}

	public void PrepareUpgradeEffect()
	{
	}

	public void PlayUpgradeEffect()
	{
		foreach (HighlightInfo highlightInfo in highlightInfos)
		{
			if (highlightInfo.animator != null)
			{
				highlightInfo.animator.Play(highlightInfo.animatorStateName);
			}
		}
	}

	protected override void OnSwitchInitialized()
	{
	}

	protected override void OnSwitchStateChanged(bool isOn)
	{
		if (!isOn)
		{
			return;
		}
		Hub.s.pdata.main?.SendPacketWithCallback(new GetRemainScrapValueReq(), delegate(GetRemainScrapValueRes res)
		{
			if (res == null)
			{
				Logger.RError("GetRemainScrapValueRes is null");
			}
			else if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RError($"GetRemainScrapValueRes: errorCode={res.errorCode}");
			}
			else
			{
				StartCoroutine(CorResetState(scanMonitorAnimationDuration + scanTextOutDuration));
			}
		}, base.destroyCancellationToken);
	}

	private IEnumerator CorResetState(float delaySec)
	{
		yield return new WaitForSeconds(delaySec);
		ChangeLevelObjectState(levelObjectID, 0, occupy: false, CancellationToken.None, delegate(int newState, UseLevelObjectRes? res)
		{
			if (res == null)
			{
				Logger.RLog("ScrapScanLevelObject::CorResetState response is null");
			}
			else if (res.errorCode != MsgErrorCode.Success)
			{
				Logger.RLog($"ScrapScanLevelObject::CorResetState failed: {res.errorCode}");
			}
		});
	}

	public void OnScrapScanning(int remainValue)
	{
		StartCoroutine(CorOnScrapScanning(remainValue));
	}

	private IEnumerator CorOnScrapScanning(int remainValue)
	{
		scanMonitorAnimator.Play(scanMonitorScanningAnim);
		scanAntennaAnimator.SetTrigger("ScanStart");
		yield return new WaitForSeconds(scanMonitorAnimationDuration);
		if (Hub.s != null && Hub.s.audioman != null && scanningEffect != null)
		{
			Hub.s.audioman.PlaySfx(scanningEffectSound, scanningEffect.transform);
		}
		if (scanningEffect != null)
		{
			scanningEffect.SetActive(value: true);
		}
		if (scanMonitorText != null)
		{
			scanMonitorText.enabled = true;
			scanMonitorText.text = $"$ {remainValue}";
		}
		yield return new WaitForSeconds(scanTextOutDuration);
		if (scanningEffect != null)
		{
			scanningEffect.SetActive(value: false);
		}
		if (scanMonitorText != null)
		{
			scanMonitorText.enabled = false;
		}
		scanAntennaAnimator.Play(scanAntennaNormalAnim);
		scanMonitorAnimator.Play(scanMonitorNormalAnim);
	}

	protected override bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (!(Hub.s.pdata.main is GamePlayScene))
		{
			return false;
		}
		if (base.State == 1)
		{
			return false;
		}
		return true;
	}

	public override bool TryInteract(ProtoActor protoActor)
	{
		int newState = ((!base.IsOn) ? 1 : 0);
		if (IsTriggerable(protoActor, newState))
		{
			Trigger(protoActor, newState);
			return true;
		}
		return false;
	}

	public override string GetSimpleText(ProtoActor protoActor)
	{
		if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null && !(Hub.s.pdata.main is GamePlayScene))
		{
			return Hub.GetL10NText(textKeyScanNotAvailable);
		}
		if (base.IsOn)
		{
			return Hub.GetL10NText(textKeyScanning);
		}
		return Hub.GetL10NText(textKeyScanAvailable);
	}
}
