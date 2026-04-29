using System.Collections;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using Mimic.Actors;
using Mimic.InputSystem;
using Unity.Cinemachine;
using UnityEngine;
using UnityEngine.CrashReportHandler;
using UnityEngine.Events;

public class CameraManager : MonoBehaviour
{
	[SerializeField]
	private CinemachineCamera? playerCameraPrefab;

	[SerializeField]
	private CinemachineCamera? spectatorCameraPrefab;

	[SerializeField]
	private CinemachineBlendDefinition.Styles blendStyle;

	private CinemachineCamera? playerCamera;

	private float originAmplitudeGain = 5f;

	private CinemachineCamera? spectatorCamera;

	private int? spectatorTargetActorID;

	private CancellationTokenSource? blendToCts;

	private CinemachineBlendDefinition? brainDefaultBlend;

	public UnityAction<float, float> OnElapsedTimeCheckForChangeSpectatorTarget;

	public bool IsSpectatorMode => spectatorTargetActorID.HasValue;

	public int? SpectatorTargetActorID => spectatorTargetActorID;

	private void Update()
	{
		UpdateSpectatorCameraInput();
	}

	public void BlendTo(CinemachineCamera? targetCamera, float duration, float inBlendTime, float outBlendTime)
	{
		BlendToAsync(targetCamera, duration, inBlendTime, outBlendTime).Forget();
	}

	private async UniTaskVoid BlendToAsync(CinemachineCamera? targetCamera, float duration, float inBlendTime, float outBlendTime, CancellationToken cancellationToken = default(CancellationToken))
	{
		CancelBlendTo();
		blendToCts = CancellationTokenSource.CreateLinkedTokenSource(cancellationToken, base.destroyCancellationToken);
		if (targetCamera == null)
		{
			Logger.RError("Target camera is not set.");
			return;
		}
		if (playerCamera == null)
		{
			Logger.RError("Player camera is not set.");
			return;
		}
		if (Camera.main.TryGetComponent<CinemachineBrain>(out var brain))
		{
			brainDefaultBlend = brain.DefaultBlend;
			brain.DefaultBlend = new CinemachineBlendDefinition(blendStyle, inBlendTime);
		}
		targetCamera.enabled = true;
		if (blendToCts != null && !blendToCts.IsCancellationRequested)
		{
			if (!float.IsPositiveInfinity(duration))
			{
				await UniTask.WaitForSeconds(duration, ignoreTimeScale: false, PlayerLoopTiming.Update, blendToCts.Token, cancelImmediately: true).SuppressCancellationThrow();
			}
			else
			{
				await UniTask.WaitUntilCanceled(blendToCts.Token, PlayerLoopTiming.Update, completeImmediately: true).SuppressCancellationThrow();
			}
		}
		if (brain != null)
		{
			brain.DefaultBlend = new CinemachineBlendDefinition(blendStyle, outBlendTime);
		}
		if (targetCamera != null)
		{
			targetCamera.enabled = false;
		}
		if (outBlendTime > 0f && blendToCts != null && !blendToCts.IsCancellationRequested)
		{
			await UniTask.WaitForSeconds(outBlendTime, ignoreTimeScale: false, PlayerLoopTiming.Update, blendToCts.Token, cancelImmediately: true).SuppressCancellationThrow();
		}
		if (brain != null && brainDefaultBlend.HasValue)
		{
			brain.DefaultBlend = brainDefaultBlend.Value;
		}
	}

	public void CancelBlendTo()
	{
		if (blendToCts != null)
		{
			blendToCts.Cancel();
			blendToCts.Dispose();
			blendToCts = null;
		}
	}

	private IEnumerator DelayedChangeSpectatorTargetToNextPlayer(float delaySeconds, int targetActorID)
	{
		float eventTimePeriod = 1f;
		float elapsedTime = 0f;
		while (elapsedTime < delaySeconds)
		{
			OnElapsedTimeCheckForChangeSpectatorTarget?.Invoke(elapsedTime, delaySeconds);
			yield return new WaitForSeconds(eventTimePeriod);
			elapsedTime += eventTimePeriod;
			if (!IsCurrentSpectatorTarget(targetActorID))
			{
				OnElapsedTimeCheckForChangeSpectatorTarget?.Invoke(-1f, delaySeconds);
				yield break;
			}
		}
		ChangeSpectatorCameraTarget(1);
		OnElapsedTimeCheckForChangeSpectatorTarget?.Invoke(-1f, delaySeconds);
	}

	public void OnPlayerDespawn(int actorID, bool isGameSessionEnded)
	{
		if (IsSpectatorMode && !isGameSessionEnded)
		{
			ChangeSpectatorTargetToNextPlayerAfterDelay(actorID);
		}
	}

	public void OnPlayerDeath(ProtoActor actor)
	{
		if (!(actor == null))
		{
			if (IsSpectatorMode)
			{
				ChangeSpectatorTargetToNextPlayerAfterDelay(actor.ActorID);
			}
			else if (actor.AmIAvatar())
			{
				ChangeCameraPlayerToSpectator();
				CrashReportHandler.SetUserMetadata("player_dead_reason", actor.ReasonOfDeath.ToString());
			}
		}
	}

	public bool ChangeCameraPlayerToSpectator()
	{
		List<ProtoActor> alivePlayers = GetAlivePlayers();
		if (alivePlayers.Count > 0)
		{
			SetupSpectatorCamera(alivePlayers[0]);
			Hub.s.inputman.SetCapturing(on: true);
			if (playerCamera != null)
			{
				Object.Destroy(playerCamera.gameObject);
				playerCamera = null;
			}
			return true;
		}
		return false;
	}

	public void OnEndDungeon(bool isSuccess)
	{
		if (playerCamera != null)
		{
			Object.Destroy(playerCamera.gameObject);
			playerCamera = null;
		}
		if (spectatorCamera != null)
		{
			Object.Destroy(spectatorCamera.gameObject);
			spectatorCamera = null;
		}
	}

	public void SetupPlayerCamera(ProtoActor actor, string jumpCameraSocketName)
	{
		if (!(playerCameraPrefab == null))
		{
			if (playerCamera == null)
			{
				playerCamera = Object.Instantiate(playerCameraPrefab);
			}
			if (playerCamera.TryGetComponent<CinemachineBasicMultiChannelPerlin>(out var component))
			{
				originAmplitudeGain = component.AmplitudeGain;
				component.AmplitudeGain = 0f;
			}
			playerCamera.Follow = actor.FpvCameraRoot;
			playerCamera.LookAt = actor.FpvCameraRoot;
			Transform transform = SocketNodeMarker.FindFirstInHierarchy(actor.transform, jumpCameraSocketName);
			if (transform != null && transform.TryGetComponent<CinemachineCamera>(out var component2))
			{
				component2.Follow = actor.FpvCameraRoot;
				component2.LookAt = actor.FpvCameraRoot;
			}
			spectatorTargetActorID = null;
			Camera.main?.gameObject.SetActive(value: true);
		}
	}

	private void SetupSpectatorCamera(ProtoActor actor)
	{
		if (!(spectatorCameraPrefab == null))
		{
			if (spectatorCamera == null)
			{
				spectatorCamera = Object.Instantiate(spectatorCameraPrefab);
			}
			spectatorCamera.Follow = actor.FpvCameraRoot;
			spectatorCamera.LookAt = actor.FpvCameraRoot;
			spectatorTargetActorID = actor.ActorID;
			Hub.s.uiman.OnSetupSpectatorCamera?.Invoke(actor);
			if (Hub.s.pdata.main is GamePlayScene gamePlayScene && !gamePlayScene.CheckActorIsIndoor(actor))
			{
				Hub.s.voiceman.StopAllMimicVoiceIndoor();
			}
		}
	}

	private void ChangeSpectatorCameraTarget(int targetIndexDelta)
	{
		if (targetIndexDelta == 0)
		{
			return;
		}
		List<ProtoActor> alivePlayers = GetAlivePlayers();
		if (alivePlayers.Count > 0)
		{
			int num = 0;
			int num2 = alivePlayers.FindIndex((ProtoActor a) => a.ActorID == spectatorTargetActorID);
			if (num2 > -1)
			{
				num = (num2 + targetIndexDelta + alivePlayers.Count) % alivePlayers.Count;
			}
			if (num2 != num)
			{
				SetupSpectatorCamera(alivePlayers[num]);
			}
		}
	}

	public bool IsCurrentSpectatorTarget(ProtoActor actor)
	{
		return IsCurrentSpectatorTarget(actor.ActorID);
	}

	public bool IsCurrentSpectatorTarget(int actorID)
	{
		if (spectatorTargetActorID.HasValue)
		{
			return spectatorTargetActorID.Value == actorID;
		}
		return false;
	}

	public bool TryGetCurrentSpectatorTarget(out ProtoActor? target)
	{
		target = null;
		if (Hub.s == null || Hub.s.pdata.main == null)
		{
			return false;
		}
		if (!spectatorTargetActorID.HasValue)
		{
			return false;
		}
		target = Hub.s.pdata.main.GetActorByActorID(spectatorTargetActorID.Value);
		if (target == null)
		{
			return false;
		}
		return true;
	}

	private List<ProtoActor> GetAlivePlayers()
	{
		List<ProtoActor> list = new List<ProtoActor>();
		if (Hub.s != null && Hub.s.pdata != null && Hub.s.pdata.main != null)
		{
			foreach (ProtoActor allPlayer in Hub.s.pdata.main.GetAllPlayers())
			{
				if (!allPlayer.dead)
				{
					list.Add(allPlayer);
				}
			}
		}
		if (list.Count > 0)
		{
			list.Sort((ProtoActor lhs, ProtoActor rhs) => lhs.ActorID.CompareTo(rhs.ActorID));
		}
		return list;
	}

	private void UpdateSpectatorCameraInput()
	{
		if (!spectatorTargetActorID.HasValue || !(spectatorCamera != null) || !(Hub.s != null) || !(Hub.s.inputman != null))
		{
			return;
		}
		if (Hub.s.inputman.wasPressedThisFrame(InputAction.NextSpectatorTarget))
		{
			ChangeSpectatorCameraTarget(1);
		}
		else if (Hub.s.inputman.wasPressedThisFrame(InputAction.PreviousSpectatorTarget))
		{
			ChangeSpectatorCameraTarget(-1);
		}
		else if (Hub.s.inputman.wasPressedThisFrame(InputAction.UI_ZOOM_OUT))
		{
			CinemachineOrbitalFollow component = spectatorCamera.GetComponent<CinemachineOrbitalFollow>();
			if (component != null && component.Radius > 1f)
			{
				component.Radius -= 1f;
			}
		}
		else if (Hub.s.inputman.wasPressedThisFrame(InputAction.UI_ZOOM_IN))
		{
			CinemachineOrbitalFollow component2 = spectatorCamera.GetComponent<CinemachineOrbitalFollow>();
			if (component2 != null && component2.Radius < 5f)
			{
				component2.Radius += 1f;
			}
		}
	}

	private void ChangeSpectatorTargetToNextPlayerAfterDelay(int actorID)
	{
		if (!IsSpectatorMode)
		{
			Logger.RError("ChangeSpectatorTargetToNextPlayerAfterDelay : Not in spectator mode or actor is null.");
		}
		else if (IsCurrentSpectatorTarget(actorID) && GetAlivePlayers().Count > 0)
		{
			float delaySeconds = (float)Hub.s.dataman.ExcelDataManager.Consts.C_AutoObservingTargetChangeTime * 0.001f;
			StartCoroutine(DelayedChangeSpectatorTargetToNextPlayer(delaySeconds, spectatorTargetActorID.Value));
		}
	}

	public void ChangeSpectatorCameraTarget(string actorName)
	{
		if (string.IsNullOrEmpty(actorName))
		{
			return;
		}
		List<ProtoActor> list = Hub.s?.pdata?.main?.GetAllPlayers();
		if (list != null)
		{
			ProtoActor protoActor = list.Find((ProtoActor a) => a.nickName == actorName);
			if (!(protoActor == null))
			{
				SetupSpectatorCamera(protoActor);
			}
		}
	}
}
