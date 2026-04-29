using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using DarkTonic.MasterAudio;
using Mimic.Actors;
using UnityEngine;

public abstract class StaticLevelObject : LevelObject
{
	[SerializeField]
	[Tooltip("상태의 전환을 반복적으로 수행하려할 때 전환이 막히는 최소한의 지연 시간")]
	protected float minTransitionInterval = 1f;

	[SerializeField]
	protected Animator? animator;

	[SerializeField]
	protected string animationName = string.Empty;

	[SerializeField]
	private bool triggerWhenTransitionComplete;

	[SerializeField]
	private bool animateWhenTransitionComplete;

	[SerializeField]
	protected AudioSource? triggerAudioSource;

	[SerializeField]
	protected string triggerAudioKey = "";

	[SerializeField]
	protected string triggerMasterAudioKey = "";

	protected StateActionInfo? currentTransition;

	protected CancellationTokenSource? ctsForTriggerAction;

	protected PlaySoundResult? currentPlaySoundResult;

	protected float lastTransitionTime;

	protected Action<int, int, string> OnTransitionStarted;

	protected Action<int, int, string> OnTransitionCanceled;

	public int OccupiedActorID { get; protected set; }

	private void OnDestroy()
	{
		StopCurrentSound();
	}

	protected void LoadStateActionsToMap<T>(List<StateAction<T>> stateActions) where T : Enum
	{
		base.StateActionsMap.Clear();
		foreach (StateAction<T> stateAction in stateActions)
		{
			int num = Convert.ToInt32(stateAction.FromState);
			int num2 = Convert.ToInt32(stateAction.ToState);
			if (!base.StateActionsMap.TryGetValue(num, out var value))
			{
				value = new Dictionary<int, StateActionInfo>();
				base.StateActionsMap[num] = value;
			}
			if (value.ContainsKey(num2))
			{
				Logger.RError($"[StaticLevelObject] Duplicate state action: {num} -> {num2}");
			}
			value[num2] = stateAction.stateActionInfo;
		}
	}

	public bool HasStateActionTransition(int fromState, int toState, out StateActionInfo stateActionInfo)
	{
		if (base.StateActionsMap.TryGetValue(fromState, out var value) && value.TryGetValue(toState, out stateActionInfo))
		{
			return true;
		}
		stateActionInfo = null;
		return false;
	}

	public string FindAnyAnimatorStateNameForState(int toState)
	{
		foreach (int key in base.StateActionsMap.Keys)
		{
			if (base.StateActionsMap[key].ContainsKey(toState) && !string.IsNullOrEmpty(base.StateActionsMap[key][toState].animatorStateName))
			{
				return base.StateActionsMap[key][toState].animatorStateName;
			}
		}
		return string.Empty;
	}

	public override CrosshairType GetCrossHairType(ProtoActor protoActor)
	{
		if (currentTransition != null && currentTransition.transitionDurtaion > 0f)
		{
			return currentTransition.crosshairTypeWhenTransition;
		}
		return base.crossHairType;
	}

	public override float GetCrossHairAnimDuration()
	{
		return GetTransitionDuration();
	}

	public virtual float GetTransitionDuration()
	{
		if (currentTransition != null)
		{
			return currentTransition.transitionDurtaion;
		}
		return 0f;
	}

	protected virtual bool IsTriggerable(ProtoActor protoActor, int newState)
	{
		if (base.State != newState && Time.time - lastTransitionTime >= minTransitionInterval)
		{
			return currentTransition == null;
		}
		return false;
	}

	protected virtual void Trigger(ProtoActor protoActor, int newState)
	{
		lastTransitionTime = Time.time;
		TriggerAsync(protoActor, newState).Forget();
	}

	private async UniTask TriggerAsync(ProtoActor protoActor, int newState)
	{
		ctsForTriggerAction?.Cancel();
		ctsForTriggerAction?.Dispose();
		ctsForTriggerAction = CancellationTokenSource.CreateLinkedTokenSource(base.destroyCancellationToken);
		int prevState = base.State;
		if (base.StateActionsMap.Count == 0)
		{
			OnTransitionStarted?.Invoke(prevState, newState, "");
			if (triggerWhenTransitionComplete)
			{
				PlayTriggerSound(prevState, newState);
				AnimateObject(prevState, newState);
				try
				{
					await UniTask.WaitForSeconds(minTransitionInterval, ignoreTimeScale: false, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
				}
				catch (OperationCanceledException)
				{
					OnTransitionCanceled?.Invoke(prevState, newState, "");
					CancelAnimateObject(prevState, newState);
					return;
				}
				TriggerAction(protoActor, newState);
				try
				{
					await UniTask.WaitUntil(() => base.State == newState, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
				}
				catch (OperationCanceledException)
				{
					OnTransitionCanceled?.Invoke(prevState, newState, "");
					CancelAnimateObject(prevState, newState);
					return;
				}
			}
			else if (animateWhenTransitionComplete)
			{
				TriggerAction(protoActor, newState);
			}
			else
			{
				PlayTriggerSound(prevState, newState);
				AnimateObject(prevState, newState);
				TriggerAction(protoActor, newState);
				try
				{
					await UniTask.WaitForSeconds(minTransitionInterval, ignoreTimeScale: false, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
				}
				catch (OperationCanceledException)
				{
					OnTransitionCanceled?.Invoke(prevState, newState, "");
					CancelAnimateObject(prevState, newState);
					return;
				}
			}
		}
		if (!HasStateActionTransition(prevState, newState, out StateActionInfo stateActionInfo))
		{
			return;
		}
		OnTransitionStarted?.Invoke(prevState, newState, stateActionInfo.action);
		currentTransition = stateActionInfo;
		float transitionDuration = GetTransitionDuration();
		if (currentTransition.animateWhenTransitionStarted && currentTransition.triggerWhenTransitionComplete)
		{
			PlayTriggerSound(prevState, newState);
			AnimateObject(prevState, newState);
			if (transitionDuration > 0f)
			{
				try
				{
					await UniTask.WaitForSeconds(transitionDuration, ignoreTimeScale: false, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
				}
				catch (OperationCanceledException)
				{
					OnTransitionCanceled?.Invoke(prevState, newState, stateActionInfo.action);
					CancelAnimateObject(prevState, newState);
					currentTransition = null;
					return;
				}
			}
			TriggerAction(protoActor, newState);
			try
			{
				await UniTask.WaitUntil(() => base.State == newState, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
				return;
			}
			catch (OperationCanceledException)
			{
				OnTransitionCanceled?.Invoke(prevState, newState, stateActionInfo.action);
				CancelAnimateObject(prevState, newState);
				currentTransition = null;
				return;
			}
		}
		if (currentTransition.animateWhenTransitionStarted && !currentTransition.triggerWhenTransitionComplete)
		{
			PlayTriggerSound(prevState, newState);
			AnimateObject(prevState, newState);
			TriggerAction(protoActor, newState);
			try
			{
				await UniTask.WaitUntil(() => base.State == newState, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
				return;
			}
			catch (OperationCanceledException)
			{
				if (base.State != newState)
				{
					OnTransitionCanceled?.Invoke(prevState, newState, stateActionInfo.action);
					CancelAnimateObject(prevState, newState);
				}
				currentTransition = null;
				return;
			}
		}
		if (!currentTransition.animateWhenTransitionStarted && !currentTransition.triggerWhenTransitionComplete)
		{
			TriggerAction(protoActor, newState);
			if (transitionDuration > 0f)
			{
				try
				{
					await UniTask.WaitForSeconds(transitionDuration, ignoreTimeScale: false, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
				}
				catch (OperationCanceledException)
				{
					if (base.State == newState)
					{
						PlayTriggerSound(prevState, newState);
						AnimateObject(prevState, newState);
					}
					currentTransition = null;
					return;
				}
			}
			PlayTriggerSound(prevState, newState);
			AnimateObject(prevState, newState);
			return;
		}
		try
		{
			if (transitionDuration > 0f)
			{
				await UniTask.WaitForSeconds(transitionDuration, ignoreTimeScale: false, PlayerLoopTiming.Update, ctsForTriggerAction.Token, cancelImmediately: true);
			}
			PlayTriggerSound(prevState, newState);
			AnimateObject(prevState, newState);
			TriggerAction(protoActor, newState);
		}
		catch (OperationCanceledException)
		{
			currentTransition = null;
		}
	}

	protected virtual void AnimateObject(int fromState, int toState)
	{
		if (HasStateActionTransition(fromState, toState, out StateActionInfo stateActionInfo))
		{
			string animatorStateName = stateActionInfo.animatorStateName;
			if (animator != null && animatorStateName != "")
			{
				animator.CrossFade(animatorStateName, 0.3f);
				animator.Update(0f);
			}
			if (stateActionInfo.animationInfos == null)
			{
				return;
			}
			{
				foreach (AnimationInfo animationInfo in stateActionInfo.animationInfos)
				{
					animationInfo.animator.CrossFade(animationInfo.animatorStateName, 0.3f);
					animationInfo.animator.Update(0f);
				}
				return;
			}
		}
		if (animator != null && animationName != string.Empty)
		{
			animator.CrossFade(animationName, 0.3f);
			animator.Update(0f);
		}
	}

	protected virtual void CancelAnimateObject(int fromState, int toState)
	{
		if (HasStateActionTransition(fromState, toState, out StateActionInfo stateActionInfo))
		{
			string animatorStateNameForCancel = stateActionInfo.animatorStateNameForCancel;
			if (animator != null && animatorStateNameForCancel != "")
			{
				animator.CrossFadeInFixedTime(animatorStateNameForCancel, 0.3f, -1, 1f);
			}
			if (stateActionInfo.animationInfos == null)
			{
				return;
			}
			{
				foreach (AnimationInfo animationInfo in stateActionInfo.animationInfos)
				{
					animationInfo.animator.CrossFade(animationInfo.animatorStateNameForCancel, 0.3f);
				}
				return;
			}
		}
		if (animator != null && animationName != string.Empty)
		{
			animator.CrossFadeInFixedTime(animationName, 0.3f, -1, 1f);
		}
	}

	protected virtual void TriggerAction(ProtoActor protoActor, int newState)
	{
	}

	public override bool TryInteractEnd(ProtoActor protoActor)
	{
		if (currentTransition != null)
		{
			ctsForTriggerAction?.Cancel();
			ctsForTriggerAction?.Dispose();
			ctsForTriggerAction = null;
		}
		return true;
	}

	protected void ChangeClientState(int newState)
	{
		int state = base.State;
		base.State = newState;
		lastTransitionTime = Time.time;
		if (!(animator != null))
		{
			return;
		}
		if (HasStateActionTransition(state, base.State, out StateActionInfo stateActionInfo))
		{
			string animatorStateName = stateActionInfo.animatorStateName;
			animator.CrossFade(animatorStateName, 1f);
			if (stateActionInfo.animationInfos == null)
			{
				return;
			}
			{
				foreach (AnimationInfo animationInfo in stateActionInfo.animationInfos)
				{
					animationInfo.animator.CrossFade(animationInfo.animatorStateName, 1f);
				}
				return;
			}
		}
		if (state == base.State)
		{
			string stateName = FindAnyAnimatorStateNameForState(base.State);
			animator.CrossFade(stateName, 1f);
		}
	}

	protected void StopCurrentSound()
	{
		if (currentPlaySoundResult?.ActingVariation != null)
		{
			if (currentPlaySoundResult.ActingVariation.AudioLoops)
			{
				currentPlaySoundResult.ActingVariation.Stop();
			}
			currentPlaySoundResult = null;
		}
	}

	protected void PlayTriggerSound(int fromState, int toState)
	{
		if (Hub.s == null)
		{
			return;
		}
		StopCurrentSound();
		if (HasStateActionTransition(fromState, toState, out StateActionInfo stateActionInfo))
		{
			if (stateActionInfo == null)
			{
				return;
			}
			if (stateActionInfo.masterAudioKey != string.Empty)
			{
				if (Hub.s.audioman != null)
				{
					currentPlaySoundResult = Hub.s.audioman.PlaySfxTransform(stateActionInfo.masterAudioKey, base.transform);
				}
			}
			else if (stateActionInfo.audiokey != string.Empty && Hub.s.legacyAudio != null && triggerAudioSource != null)
			{
				Hub.s.legacyAudio.Play(stateActionInfo.audiokey, triggerAudioSource);
			}
		}
		else
		{
			if (base.StateActionsMap.Count != 0)
			{
				return;
			}
			if (triggerMasterAudioKey != string.Empty)
			{
				if (Hub.s.audioman != null)
				{
					currentPlaySoundResult = Hub.s.audioman.PlaySfxTransform(triggerMasterAudioKey, base.transform);
				}
			}
			else if (triggerAudioKey.Length > 0 && Hub.s.legacyAudio != null && triggerAudioSource != null)
			{
				Hub.s.legacyAudio.Play(triggerAudioKey, triggerAudioSource);
			}
		}
	}

	public void ChangeLevelObjectState(int levelObjectID, int newState, bool occupy, CancellationToken cancellationToken, Action<int, UseLevelObjectRes?> callback)
	{
		if (Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.main == null)
		{
			return;
		}
		Hub.s.pdata.main.SendPacketWithCallback(new UseLevelObjectReq
		{
			levelObjectID = levelObjectID,
			state = newState,
			occupy = occupy
		}, delegate(UseLevelObjectRes res)
		{
			if (!cancellationToken.IsCancellationRequested)
			{
				callback?.Invoke(newState, res);
			}
		}, base.destroyCancellationToken);
	}

	public override void OnChangeLevelObjectStateSig(int actorId, int occupiedActorID, int prevState, int CurrentState)
	{
		if (base.State != prevState)
		{
			Logger.RLog($"[StaticLevelObject] OnChangeLevelObjectStateSig: clientState : {base.State} -> {CurrentState}, serverState : {prevState} -> {CurrentState}, actorId: {actorId}, OccupiedActorID: {occupiedActorID}");
		}
		base.State = CurrentState;
		OccupiedActorID = occupiedActorID;
		currentTransition = null;
	}
}
