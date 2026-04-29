using System;
using DunGen;
using Mimic.Actors;
using Mimic.Voice.SpeechSystem;
using ReluProtocol;
using ReluProtocol.C2S;
using ReluReplay.Data;
using ReluReplay.Recorder;
using ReluReplay.Shared;

namespace ReluReplay
{
	public class ReplayManager
	{
		private ReplayRecorder _recorder;

		private bool _bStartSpectatorCamera;

		private bool _toggleDrawMimicSelectedColor;

		private int _emotionDataCount;

		public int NextDungeonMasterID => -1;

		public int RandDungeonSeed => -1;

		public bool IsReplayPlayMode => false;

		public bool UseRecordMode => _recorder?.UseRecord ?? false;

		public int EmotionDataCount => _emotionDataCount;

		public ReplayManager()
		{
			Init_MappingPacketEvent();
			ReplaySharedData.RegisterBaseSaveReplayFilePath();
			_recorder = new ReplayRecorder(this);
		}

		private void Init_MappingPacketEvent()
		{
			RegisterHandler<LevelLoadCompleteRes>();
			RegisterHandler<SightInSig>();
			RegisterHandler<SightOutSig>();
			RegisterHandler<ChangeFactionSig>();
			RegisterHandler<SyncImmutableStatSig>();
			RegisterHandler<AnnounceImmutableStatSig>();
			RegisterHandler<ActorChangePhaseNoti>();
			RegisterHandler<ChangeAITargetSig>();
			RegisterHandler<SyncSkillMoveSig>();
			RegisterHandler<MoveStartSig>();
			RegisterHandler<MoveStopSig>();
			RegisterHandler<ChangeViewPointSig>();
			RegisterHandler<ChangeSprintModeSig>();
			RegisterHandler<TeleportSig>();
			RegisterHandler<AnnounceMutableStatSig>();
			RegisterHandler<ChangeItemLooksSig>();
			RegisterHandler<ActorDyingSig>();
			RegisterHandler<HitTargetSig>();
			RegisterHandler<AbnormalSig>();
			RegisterHandler<CooltimeSig>();
			RegisterHandler<UseLevelObjectSig>();
			RegisterHandler<AttachActorSig>();
			RegisterHandler<CancelSkillSig>();
			RegisterHandler<ChangeEquipStatusSig>();
			RegisterHandler<ReloadWeaponSig>();
			RegisterHandler<UseSkillSig>();
			RegisterHandler<PlaySoundSig>();
			RegisterHandler<GroggyStateSig>();
			RegisterHandler<EndDungeonSig>();
			RegisterHandler<FieldHitTargetSig>();
			RegisterHandler<DestroyActorSig>();
			RegisterHandler<EmotionSig>();
			RegisterHandler<CancelEmotionSig>();
			RegisterHandler<TimeSyncSig>();
			RegisterHandler<TramStatusSig>();
			RegisterHandler<CutSceneCompleteSig>();
			RegisterHandler<PlayCutSceneSig>();
			RegisterHandler<StartRepairTramSig>();
			RegisterHandler<PlayAnimationSig>();
			RegisterHandler<ChangeCurrencySig>();
			RegisterHandler<DestroyItemSig>();
			RegisterHandler<UpdateInvenSig>();
			RegisterHandler<AllMemberEnterRoomSig>();
			RegisterHandler<BlackoutSig>();
			RegisterHandler<ItemSpawnFieldSkillWaitSig>();
			RegisterHandler<JumpSig>();
			RegisterHandler<CancelJumpSig>();
			RegisterHandler<StartScrapMotionSig>();
			RegisterHandler<EndScrapMotionSig>();
			RegisterHandler<CancelScrapMotionSig>();
			RegisterHandler<EnableDeathMatchRoomSig>();
			RegisterHandler<DeathMatchRoomScoreBoardSig>();
			RegisterHandler<EndDeathMatchSig>();
			RegisterHandler<GetRemainScrapValueSig>();
			RegisterHandler<DebugBTStateInfoSig>();
			RegisterHandler<DebugDLAgentInfoSig>();
			RegisterHandler<DebugMimicVoiceInfoSig>();
			RegisterHandler<DebugSpeechEventDeltaSig>();
			RegisterHandler<DebugMimicVoiceEmotionSig>();
		}

		private void RegisterHandler<T>() where T : IMsg
		{
			ReplayRecorder.AddCapturedPacket(typeof(T));
		}

		public ReplaySaveInfo GetReplaySaveInfo()
		{
			return null;
		}

		public void OnReplaySceneStart(string filePath, Action onDataLoadCompleteCallBack = null)
		{
		}

		public void OnGamePlaySceneLoadedComplete()
		{
		}

		public void OnRandomDungeonGenerationComplete(DungeonGenerator generator)
		{
		}

		public void OnReplayGameStart()
		{
		}

		public bool OnTryDequeueMsg(out IMsg msg)
		{
			msg = null;
			return false;
		}

		public void OnPlayerSpawn(ProtoActor playerActor)
		{
		}

		public void OnDestroyGamePlayScene()
		{
		}

		public void ToggleDrawMimicSelectedColor()
		{
		}

		public bool IsRecordMode()
		{
			return _recorder.UseRecord;
		}

		public void OnReadyToDeathMatchPktRecording()
		{
			_recorder?.ReadyRecordingForDeathMatch();
		}

		public void OnReadyToGamePktRecording(int nextDungeonMasterID, int randomDungeonSeed)
		{
			DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(nextDungeonMasterID);
			if (dungeonInfo == null)
			{
				_recorder?.ReadyRecording(nextDungeonMasterID, randomDungeonSeed, 0, 0, 0);
			}
			else
			{
				_recorder?.ReadyRecording(nextDungeonMasterID, randomDungeonSeed, dungeonInfo.HazardLevel, dungeonInfo.PollutionLevel, dungeonInfo.RewardLevel);
			}
		}

		public void OnSetSessionCtxEvent(SessionContext sessionContext)
		{
			_recorder?.SetSessionCtxEvent(sessionContext);
		}

		public void OnStopRecording()
		{
			_recorder?.StopRecording();
			_emotionDataCount = 0;
		}

		public void OnCancelRecording()
		{
			_recorder?.OnCancelRecording();
			_emotionDataCount = 0;
		}

		public bool OnPreDispatcherHookEvent(IMsg msg, IContext ctx)
		{
			return _recorder?.OnPreDispatcherHookEvent(msg, ctx) ?? true;
		}

		public void OnRecordVoiceFile(ProtoActor actor, SpeechEvent speechEvent)
		{
			OnRecordVoiceFile(actor.ActorID, speechEvent);
		}

		public void OnRecordVoiceFile(int actorID, SpeechEvent speechEvent)
		{
			_recorder.OnRecordVoiceFile(actorID, speechEvent);
		}

		public bool OnCapturePacketOnlyReplay(IMsg msg)
		{
			if (UseRecordMode)
			{
				return _recorder.CapturePacket(msg);
			}
			return false;
		}

		public void AddEmotionDataCount()
		{
			_emotionDataCount++;
		}
	}
}
