using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Bifrost.ConstEnum;
using Bifrost.Cooked;
using Bifrost.DefAbnormal;
using Bifrost.GrabData;
using Cysharp.Threading.Tasks;
using DLAgent;
using DarkTonic.MasterAudio;
using Dissonance;
using DunGen;
using EZhex1991.EZSoftBone;
using GameKit.Dependencies.Utilities;
using Mimic.Animation;
using Mimic.Audio;
using Mimic.Character;
using Mimic.Character.HitSystem;
using Mimic.InputSystem;
using Mimic.Voice.SpeechSystem;
using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;
using ReluReplay.Shared;
using TMPro;
using UnityEngine;
using UnityEngine.Audio;
using UnityEngine.Rendering;

namespace Mimic.Actors
{
	[HelpURL("https://wiki.krafton.com/x/XoCtNgE")]
	public class ProtoActor : MonoBehaviour, IActor
	{
		private struct MoveInterpolationData
		{
			public enum eCommand
			{
				StartMove = 0,
				StopMove = 1
			}

			public eCommand command;

			public Vector3 pos;

			public float rotY;

			public float camRotX;

			public float velocity;
		}

		public struct ActorDeathInfo
		{
			public int DeadActorID;

			public int AttackerActorID;

			public int LinkedMasterID;

			public ReasonOfDeath ReasonOfDeath;
		}

		public enum ControlMode
		{
			Manual = 0,
			Remote = 1
		}

		public class NetSyncActorData
		{
			public long hp = 100L;

			public long maxHP = 100L;

			public long stamina = 100L;

			public long maxStamina = 100L;

			public long conta = 1000L;

			public long maxConta = 1000L;

			public long voicePitch;

			public string actorName = "unknown";

			public long MoveSpeedWalk = 350L;

			public long MoveSpeedRun = 700L;
		}

		private struct DebugBTStateInfo
		{
			public string btDataName;

			public string btTemplateName;
		}

		public class AbnormalHelper
		{
			public class AbnormalCCInfoWrapper
			{
				public readonly DefAbnormal_MasterData masterData;

				public readonly AbnormalCCInfo ccinfo;

				public bool IsUnableInput => masterData.unable_input;

				public bool IsUnableInputMove => masterData.unable_input_move;

				public bool IsUnableMove => masterData.unable_move;

				public bool IsSilence => masterData.unable_sound;

				public bool IsBlind => ccinfo.ccType == CCType.Blind;

				public bool IsVoiceChanged => ccinfo.ccType == CCType.ChangeVoice;

				public bool IsRinging => ccinfo.ccType == CCType.Ringing;

				public AbnormalCCInfoWrapper(AbnormalCCInfo ccinfo)
				{
					this.ccinfo = ccinfo;
					masterData = GetDefAbnormalMasterDataWithCCType(ccinfo.ccType);
				}
			}

			private ProtoActor owner;

			private Dictionary<long, AbnormalObjectInfo> abnormalinfos = new Dictionary<long, AbnormalObjectInfo>();

			private Dictionary<long, AbnormalCCInfoWrapper> ccinfowrappers = new Dictionary<long, AbnormalCCInfoWrapper>();

			private Dictionary<long, AbnormalStatsInfo> statinfos = new Dictionary<long, AbnormalStatsInfo>();

			private Dictionary<long, AbnormalImmuneInfo> immuneinfos = new Dictionary<long, AbnormalImmuneInfo>();

			private DataManager dataman => Hub.s.dataman;

			public Dictionary<long, AbnormalObjectInfo> AbnormalInfos => abnormalinfos;

			public bool IsUnableSound { get; private set; }

			public string ringingAudioKey { get; private set; } = string.Empty;

			public bool IsSilence { get; private set; }

			public bool IsChangeVoice { get; private set; }

			public bool IsBlind { get; private set; }

			public AbnormalHelper(ProtoActor owner)
			{
				this.owner = owner;
			}

			public void OnNetAbnormalSig(AbnormalSig sig)
			{
				UpdateAbnormal(sig);
				UpdateAnimationByAbnormal(sig);
				UpdateImmobileByAbnormal(sig);
				bool isSilence = IsSilence;
				bool isUnableSound = IsUnableSound;
				UpdateCCStatus(sig);
				if (isSilence && !IsSilence)
				{
					if (owner.AmIAvatar())
					{
						owner.SetVoiceEffect(VoiceEffecter.VoiceEffectType.Silence, isEnabled: false);
					}
				}
				else if (!isSilence && IsSilence && owner.AmIAvatar())
				{
					owner.SetVoiceEffect(VoiceEffecter.VoiceEffectType.Silence, isEnabled: true);
				}
				if (isUnableSound && !IsUnableSound)
				{
					owner.AmIAvatar();
				}
				else if (!isUnableSound && IsUnableSound)
				{
					owner.AmIAvatar();
				}
			}

			private void UpdateAbnormal(AbnormalSig sig)
			{
				sig.abnormalIcons.FindAll((AbnormalObjectInfo a) => a.syncType == AbnormalDataSyncType.Add);
				sig.abnormalIcons.FindAll((AbnormalObjectInfo a) => a.syncType == AbnormalDataSyncType.Remove);
				sig.abnormalIcons.FindAll((AbnormalObjectInfo a) => a.syncType == AbnormalDataSyncType.Change);
				foreach (AbnormalObjectInfo abnormalIcon in sig.abnormalIcons)
				{
					if (abnormalIcon.syncType == AbnormalDataSyncType.Add)
					{
						if (abnormalinfos.TryAdd(abnormalIcon.abnormalSyncID, abnormalIcon))
						{
							owner.PlayAbnormalEffect(abnormalIcon.abnormalSyncID, abnormalIcon.abnormalMasterID);
						}
						else
						{
							Logger.RWarn("[abnormal] add AbnormalSig normal failed. syncID is already used");
						}
					}
					else if (abnormalIcon.syncType == AbnormalDataSyncType.Remove)
					{
						if (abnormalinfos.ContainsKey(abnormalIcon.abnormalSyncID))
						{
							owner.StopAbnormalEffect(abnormalIcon.abnormalSyncID, abnormalIcon.abnormalMasterID);
							abnormalinfos.Remove(abnormalIcon.abnormalSyncID);
						}
						else
						{
							Logger.RWarn("[abnormal] remove AbnormalSig normal failed. syncID is not exist");
						}
					}
					else if (abnormalIcon.syncType == AbnormalDataSyncType.Change)
					{
						if (!abnormalinfos.ContainsKey(abnormalIcon.abnormalSyncID))
						{
							Logger.RWarn("[abnormal] change AbnormalSig normal failed. syncID is not exist");
							continue;
						}
						abnormalinfos[abnormalIcon.abnormalSyncID] = abnormalIcon;
						owner.UpdateAbnormalEffect(abnormalIcon.abnormalSyncID, abnormalIcon.abnormalMasterID);
					}
				}
			}

			private void UpdateAnimationByAbnormal(AbnormalSig sig)
			{
				List<AbnormalObjectInfo> added = sig.abnormalIcons.FindAll((AbnormalObjectInfo ai) => ai.syncType == AbnormalDataSyncType.Add);
				List<AbnormalObjectInfo> removed = sig.abnormalIcons.FindAll((AbnormalObjectInfo ai) => ai.syncType == AbnormalDataSyncType.Remove);
				List<AbnormalObjectInfo> changed = sig.abnormalIcons.FindAll((AbnormalObjectInfo ai) => ai.syncType == AbnormalDataSyncType.Change);
				owner.RefreshAnimationByAbnormal(added, removed, changed);
			}

			private bool IsActionAbnormal(AbnormalCCInfo ccinfo)
			{
				if ((ccinfo == null || ccinfo.ccType != CCType.NormalPush) && (ccinfo == null || ccinfo.ccType != CCType.Airborne) && (ccinfo == null || ccinfo.ccType != CCType.Knockback))
				{
					if (ccinfo == null)
					{
						return false;
					}
					return ccinfo.ccType == CCType.Knockdown;
				}
				return true;
			}

			private void UpdateImmobileByAbnormal(AbnormalSig sig)
			{
				List<AbnormalCCInfo> added = sig.ccList.FindAll((AbnormalCCInfo cc) => cc.changeType == AbnormalDataSyncType.Add && IsActionAbnormal(cc));
				List<AbnormalCCInfo> removed = sig.ccList.FindAll((AbnormalCCInfo cc) => cc.changeType == AbnormalDataSyncType.Remove && IsActionAbnormal(cc));
				List<AbnormalCCInfo> changed = sig.ccList.FindAll((AbnormalCCInfo cc) => cc.changeType == AbnormalDataSyncType.Change && IsActionAbnormal(cc));
				owner.RefreshImmobileByAbnormalCC(added, removed, changed);
			}

			public void AttachingResolveOtherInfo(OtherCreatureInfo otherInfo)
			{
			}

			protected AbnormalSig Clone(AbnormalSig sig)
			{
				return new AbnormalSig
				{
					abnormalIcons = sig.abnormalIcons.Clone(),
					ccList = sig.ccList.Clone(),
					statsList = sig.statsList.Clone(),
					immuneList = sig.immuneList.Clone()
				};
			}

			public static DefAbnormal_MasterData GetDefAbnormalMasterDataWithCCType(CCType cctype)
			{
				DefAbnormal_MasterData value = null;
				Hub.s.dataman.ExcelDataManager.CCAbnormalDict.TryGetValue(cctype, out value);
				return value;
			}

			public static AbnormalInfo GetAbnormalInfo(int abnormalMasterId)
			{
				AbnormalInfo value = null;
				Hub.s.dataman.ExcelDataManager.AbnormalDict.TryGetValue(abnormalMasterId, out value);
				return value;
			}

			public void UpdateCCStatus(AbnormalSig sig)
			{
				sig.ccList.FindAll((AbnormalCCInfo cc) => cc.changeType == AbnormalDataSyncType.Add);
				sig.ccList.FindAll((AbnormalCCInfo cc) => cc.changeType == AbnormalDataSyncType.Remove);
				sig.ccList.FindAll((AbnormalCCInfo cc) => cc.changeType == AbnormalDataSyncType.Add);
				foreach (AbnormalCCInfo cc in sig.ccList)
				{
					if (cc.changeType == AbnormalDataSyncType.Add)
					{
						if (!ccinfowrappers.TryAdd(cc.syncID, new AbnormalCCInfoWrapper(cc)))
						{
							Logger.RWarn("[abnormal] add CC failed. syncID is already used");
						}
					}
					else if (cc.changeType == AbnormalDataSyncType.Remove)
					{
						if (!ccinfowrappers.Remove(cc.syncID))
						{
							Logger.RWarn("[abnormal] remove CC failed. syncID is not exist");
						}
					}
					else if (cc.changeType == AbnormalDataSyncType.Change)
					{
						if (!ccinfowrappers.ContainsKey(cc.syncID))
						{
							Logger.RWarn("[abnormal] change CC failed. syncID is not exist");
						}
						else
						{
							ccinfowrappers[cc.syncID] = new AbnormalCCInfoWrapper(cc);
						}
					}
				}
				owner.ClearInputDisableReason(EInputDisableReason.Abnormal);
				owner.ClearInputMoveDisableReason(EInputMoveDisableReason.Abnormal);
				IsUnableSound = false;
				IsSilence = false;
				IsChangeVoice = false;
				IsBlind = false;
				foreach (AbnormalCCInfoWrapper value in ccinfowrappers.Values)
				{
					if (value.IsUnableInput)
					{
						owner.SetInputDisableReason(EInputDisableReason.Abnormal);
					}
					if (value.IsUnableInputMove)
					{
						owner.SetInputMoveDisableReason(EInputMoveDisableReason.Abnormal);
					}
					if (value.IsSilence)
					{
						IsSilence = true;
					}
					if (value.IsVoiceChanged)
					{
						IsChangeVoice = true;
					}
					if (value.IsRinging)
					{
						IsUnableSound = true;
					}
					if (value.IsBlind)
					{
						IsBlind = true;
					}
				}
			}
		}

		public class AuraHelper
		{
			private ProtoActor owner;

			private Dictionary<AuraInfo, AuraSkillActor> currentAuras = new Dictionary<AuraInfo, AuraSkillActor>();

			private DataManager dataman => Hub.s.dataman;

			public AuraHelper(ProtoActor owner)
			{
				this.owner = owner;
			}

			public void OnNetAuraSig(AuraSig sig)
			{
				if (sig == null)
				{
					return;
				}
				Logger.RLog($"OnNetAuraSig called for ActorID: {sig.actorID}", sendToLogServer: false, useConsoleOut: true, "aura");
				foreach (int removedAuraMasterID in sig.removedAuraMasterIDs)
				{
					Logger.RLog($"Removing aura with MasterID: {removedAuraMasterID}", sendToLogServer: false, useConsoleOut: true, "aura");
					AuraInfo auraInfo = dataman.ExcelDataManager.GetAuraInfo(removedAuraMasterID);
					if (auraInfo != null && currentAuras.TryGetValue(auraInfo, out var value))
					{
						_ = value != null;
						currentAuras.Remove(auraInfo);
					}
				}
				foreach (int addedAuraMasterID in sig.addedAuraMasterIDs)
				{
					AddAura(addedAuraMasterID);
				}
			}

			private void AddAura(int auraMasterId)
			{
				AuraInfo auraInfo = dataman.ExcelDataManager.GetAuraInfo(auraMasterId);
				if (auraInfo != null)
				{
					AuraSkillActor auraSkillActor = SpawnAuraActor(owner.ActorID, auraInfo);
					if (auraSkillActor != null)
					{
						Logger.RLog($"Spawned aura actor for MasterID: {auraMasterId}", sendToLogServer: false, useConsoleOut: true, "aura");
						currentAuras.Add(auraInfo, auraSkillActor);
					}
					else
					{
						Logger.RError($"Failed to spawn aura actor for MasterID: {auraMasterId}", sendToLogServer: false, "aura");
					}
				}
			}

			private AuraSkillActor? SpawnAuraActor(int ownerActorId, AuraInfo auraInfo)
			{
				GameObject gameObject = UnityEngine.Object.Instantiate(Hub.s.tableman.actor.Get("AuraSkillActor"));
				if (gameObject != null)
				{
					AuraSkillActor component = gameObject.GetComponent<AuraSkillActor>();
					if (component != null)
					{
						ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(ownerActorId);
						component.Spawn(auraInfo, actorByActorID);
						return component;
					}
					Logger.RError("no AuraSkillActor component.", sendToLogServer: false, "aura");
				}
				else
				{
					Logger.RError($"Failed to instantiate AuraSkillActor prefab for MasterID: {auraInfo.MasterID}", sendToLogServer: false, "aura");
				}
				return null;
			}

			public void ResolveOtherAuraInfo(OtherCreatureInfo otherCreatureInfo)
			{
				if (otherCreatureInfo == null)
				{
					return;
				}
				foreach (int activatedAuraMasterID in otherCreatureInfo.activatedAuraMasterIDs)
				{
					Hub.s.pdata.main.GetActorByActorID(otherCreatureInfo.actorID)?.auraHelper.AddAura(activatedAuraMasterID);
				}
			}
		}

		public class EffectPlayer
		{
			public class ParticleEffectData
			{
				private GameObject playedParticle;

				private bool isPlayingEndParticle;

				private bool isCancelRequested;

				public void Reset()
				{
					playedParticle = null;
					isPlayingEndParticle = false;
					isCancelRequested = false;
				}

				public IEnumerator PlayEffect(ProtoActor actor, int skillTargetEffectId)
				{
					SkillTargetEffectDataInfo skillEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillEffectDataInfo == null || actor == null)
					{
						yield break;
					}
					isCancelRequested = false;
					if (skillEffectDataInfo.StartParticle.Length > 0)
					{
						string eventData = skillEffectDataInfo.StartParticle;
						Transform parent = actor.puppet.PickSocketFromEffectEventData(ref eventData);
						playedParticle = Hub.s.vfxman.InstantiateVfx(eventData, parent);
						if (playedParticle == null)
						{
							yield break;
						}
						playedParticle.SetActive(value: true);
						float duration = 0f;
						ParticleSystem[] components = playedParticle.GetComponents<ParticleSystem>();
						for (int i = 0; i < components.Count(); i++)
						{
							if (components[i] != null)
							{
								ParticleSystem.MainModule main = components[i].main;
								if (duration < main.duration)
								{
									duration = main.duration;
								}
							}
						}
						float elapsed = 0f;
						while (elapsed < duration)
						{
							if (isCancelRequested)
							{
								yield break;
							}
							elapsed += Time.deltaTime;
							yield return null;
						}
						if (isCancelRequested)
						{
							yield break;
						}
					}
					if (playedParticle != null)
					{
						playedParticle.SetActive(value: false);
						UnityEngine.Object.Destroy(playedParticle);
						playedParticle = null;
					}
					if (skillEffectDataInfo.LoopParticle.Length > 0)
					{
						string eventData = skillEffectDataInfo.LoopParticle;
						Transform parent = actor.puppet.PickSocketFromEffectEventData(ref eventData);
						playedParticle = Hub.s.vfxman.InstantiateVfx(eventData, parent);
						if (playedParticle == null)
						{
							yield break;
						}
						ParticleSystem[] components2 = playedParticle.GetComponents<ParticleSystem>();
						for (int j = 0; j < components2.Count(); j++)
						{
							if (components2[j] != null)
							{
								ParticleSystem.MainModule main2 = components2[j].main;
								main2.loop = true;
							}
						}
						float elapsed = (float)skillEffectDataInfo.LoopParticleDurationMSec / 1000f;
						playedParticle.SetActive(value: true);
						float duration = 0f;
						while (duration < elapsed)
						{
							if (isCancelRequested)
							{
								yield break;
							}
							duration += Time.deltaTime;
							yield return null;
						}
						if (isCancelRequested)
						{
							yield break;
						}
					}
					if (playedParticle != null)
					{
						playedParticle.SetActive(value: false);
						UnityEngine.Object.Destroy(playedParticle);
						playedParticle = null;
					}
					if (skillEffectDataInfo.EndParticle.Length > 0)
					{
						string eventData = skillEffectDataInfo.EndParticle;
						Transform parent = actor.puppet.PickSocketFromEffectEventData(ref eventData);
						playedParticle = Hub.s.vfxman.InstantiateVfx(eventData, parent);
						if (playedParticle == null)
						{
							yield break;
						}
						playedParticle.SetActive(value: true);
						isPlayingEndParticle = true;
						float duration = 0f;
						ParticleSystem[] components3 = playedParticle.GetComponents<ParticleSystem>();
						for (int k = 0; k < components3.Count(); k++)
						{
							if (components3[k] != null)
							{
								ParticleSystem.MainModule main3 = components3[k].main;
								if (duration < main3.duration)
								{
									duration = main3.duration;
								}
							}
						}
						duration += 0.5f;
						float elapsed = 0f;
						while (elapsed < duration)
						{
							if (isCancelRequested)
							{
								yield break;
							}
							elapsed += Time.deltaTime;
							yield return null;
						}
						isPlayingEndParticle = false;
					}
					if (playedParticle != null)
					{
						playedParticle.SetActive(value: false);
						UnityEngine.Object.Destroy(playedParticle);
						playedParticle = null;
					}
				}

				public void CancelEffect()
				{
					isCancelRequested = true;
					if (!isPlayingEndParticle && playedParticle != null)
					{
						playedParticle.SetActive(value: false);
						UnityEngine.Object.Destroy(playedParticle);
						playedParticle = null;
					}
				}
			}

			public class SoundEffectData
			{
				public static AudioSource PlayRinging(ProtoActor actor, int skillTargetEffectId)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillTargetEffectDataInfo == null)
					{
						return null;
					}
					if (skillTargetEffectDataInfo.PlaySounds.Count > 0 && skillTargetEffectDataInfo.PlaySounds[0].Length > 0)
					{
						string eventData = skillTargetEffectDataInfo.PlaySounds[0];
						Transform parent = actor.puppet.PickSocketFromEffectEventData(ref eventData);
						return Hub.s.legacyAudio.PlayRingingOnParent(eventData, parent);
					}
					return null;
				}

				public static PlaySoundResult? PlayLooping(ProtoActor actor, int skillTargetEffectId)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillTargetEffectDataInfo == null)
					{
						return null;
					}
					if (skillTargetEffectDataInfo.PlaySounds.Count > 0 && skillTargetEffectDataInfo.PlaySounds[0].Length > 0)
					{
						string eventData = skillTargetEffectDataInfo.PlaySounds[0];
						Transform parent = actor.puppet.PickSocketFromEffectEventData(ref eventData);
						return Hub.s.audioman.PlaySfxTransform(eventData, parent);
					}
					return null;
				}

				public static void PlayOneShot(ProtoActor actor, int skillTargetEffectId)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.PlaySounds.Count > 0 && skillTargetEffectDataInfo.PlaySounds[0].Length > 0)
					{
						string eventData = skillTargetEffectDataInfo.PlaySounds[0];
						Transform parent = actor.puppet.PickSocketFromEffectEventData(ref eventData);
						Hub.s.audioman.PlaySfx(eventData, parent);
					}
				}
			}

			public class ScreenEffectData
			{
				private string currentEffectId;

				private bool isCancelRequested;

				private int skillTargetEffectId;

				public void Reset()
				{
					currentEffectId = null;
					skillTargetEffectId = 0;
					isCancelRequested = false;
				}

				public void PlayLoopingEffectByTableId(string id)
				{
					MMScreenEffectTable.Row row = Hub.s.tableman.screenEffect.rows.FirstOrDefault((MMScreenEffectTable.Row x) => x.id == id);
					if (row != null)
					{
						currentEffectId = Hub.s.uiman.PlayLoopingScreenEffect(row.prefab);
					}
				}

				public void CancelLoopingEffect()
				{
					Hub.s.uiman.StopScreenEffect(currentEffectId);
				}

				public IEnumerator PlayEffect(ProtoActor actor, int skillTargetEffectId)
				{
					SkillTargetEffectDataInfo skillEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillEffectDataInfo == null)
					{
						yield break;
					}
					this.skillTargetEffectId = skillTargetEffectId;
					isCancelRequested = false;
					if (skillEffectDataInfo.StartScreenEffect.Length > 0 && isCancelRequested)
					{
						yield break;
					}
					if (skillEffectDataInfo.LoopScreenEffect.Length > 0)
					{
						if (skillEffectDataInfo.LoopScreenEffectDurationMSec > 0)
						{
							float loopSec = (float)skillEffectDataInfo.LoopScreenEffectDurationMSec / 1000f;
							MMScreenEffectTable.Row row = Hub.s.tableman.screenEffect.rows.FirstOrDefault((MMScreenEffectTable.Row x) => x.id == skillEffectDataInfo.LoopScreenEffect);
							currentEffectId = Hub.s.uiman.PlayScreenEffect(row.prefab, loopSec, skillEffectDataInfo.ScreenEffectLayerOrder);
							float elapsed = 0f;
							while (elapsed < loopSec)
							{
								if (isCancelRequested)
								{
									Hub.s.uiman.StopScreenEffect(currentEffectId);
									yield break;
								}
								elapsed += Time.deltaTime;
								yield return null;
							}
							Hub.s.uiman.StopScreenEffect(currentEffectId);
							if (skillEffectDataInfo.EndScreenEffect.Length > 0)
							{
								_ = isCancelRequested;
							}
						}
						else
						{
							PlayLoopingEffectByTableId(skillEffectDataInfo.LoopScreenEffect);
						}
					}
					else if (skillEffectDataInfo.EndScreenEffect.Length > 0)
					{
						_ = isCancelRequested;
					}
				}

				public void CancelEffect()
				{
					isCancelRequested = true;
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.LoopScreenEffect.Length > 0 && skillTargetEffectDataInfo.LoopScreenEffectDurationMSec >= 0)
					{
						CancelLoopingEffect();
						currentEffectId = null;
					}
				}
			}

			public class DecalEffectData
			{
				private int decalEffectInstanceId;

				public void Reset()
				{
					decalEffectInstanceId = 0;
				}

				public void PlayEffect(ProtoActor actor, int skillTargetEffectId)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.DecalPathWithSocket.Length > 0)
					{
						DecalManager.DecalData decalData = DecalManager.DecalData.CreateDecalData(skillTargetEffectDataInfo.DecalPathWithSocket, skillTargetEffectDataInfo.DecalColorId, skillTargetEffectDataInfo.DecalPrintIntervalDist, skillTargetEffectDataInfo.IsDecalRotateRandomly, skillTargetEffectDataInfo.DecalLifetimeMSec, skillTargetEffectDataInfo.DecalfadeoutMSec, skillTargetEffectDataInfo.DistanceFromSpawnPosition, skillTargetEffectDataInfo.DecalRoatation, -skillTargetEffectDataInfo.DecalRoatation, actor.puppet.transform);
						decalEffectInstanceId = Hub.s.decalManamger.ActivateDecalPeriodically(decalData);
					}
				}

				public void CancelEffect()
				{
					if (decalEffectInstanceId != 0)
					{
						Hub.s.decalManamger.DeactivateDecal(decalEffectInstanceId);
						decalEffectInstanceId = 0;
					}
				}
			}

			public class CCBehaivorBase
			{
				public long syncId;

				public CCType cctype;

				public DefAbnormal_MasterData? defAbnormalMasterData;

				protected Coroutine animationCoroutine;

				public CCBehaivorBase(long syncId, CCType cctype)
				{
					this.syncId = syncId;
					this.cctype = cctype;
				}

				public CCBehaivorBase(long syncId)
				{
					this.syncId = syncId;
				}

				public virtual void PlayImmobilize(ProtoActor actor)
				{
				}

				public virtual void CancelImmobilize(ProtoActor actor)
				{
				}
			}

			public class CCAnimData : CCBehaivorBase
			{
				public int abnormalMasterId;

				public int HighestMotionPriority = -1;

				public bool MotionCancel;

				private long animationEndTime;

				public static (bool, int) GetHighestMotionPriority(int abnormalMasterId)
				{
					AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
					if (abnormalInfo == null)
					{
						return (false, -1);
					}
					int num = -1;
					bool item = false;
					foreach (KeyValuePair<int, IAbnormalElementInfo> element in abnormalInfo.ElementList)
					{
						DefAbnormal_MasterData defAbnormalMasterDataWith = GetDefAbnormalMasterDataWith((int)element.Value.Category, element.Value.Type);
						if (defAbnormalMasterDataWith != null && defAbnormalMasterDataWith.motion_priority > num)
						{
							num = defAbnormalMasterDataWith.motion_priority;
							if (defAbnormalMasterDataWith.motion_cancel)
							{
								item = defAbnormalMasterDataWith.motion_cancel;
							}
						}
					}
					return (item, num);
				}

				public CCAnimData(long syncId, int abnormalMasterId)
					: base(syncId)
				{
					this.abnormalMasterId = abnormalMasterId;
					(MotionCancel, HighestMotionPriority) = GetHighestMotionPriority(abnormalMasterId);
				}

				public override void PlayImmobilize(ProtoActor actor)
				{
					animationCoroutine = actor.StartCoroutine(PlayAnimation(actor));
				}

				private IEnumerator PlayAnimation(ProtoActor actor)
				{
					AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
					if (abnormalInfo != null)
					{
						SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(abnormalInfo.SkillTargetEffectId);
						if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.animation.Length > 0)
						{
							animationEndTime = Hub.s.timeutil.GetCurrentTickMilliSec() + abnormalInfo.Duration;
							actor.PlayAnimByAbnormal(skillTargetEffectDataInfo.animation, skillTargetEffectDataInfo.loopAnimation);
							yield return new WaitForSeconds((float)abnormalInfo.Duration / 1000f);
						}
					}
				}

				public override void CancelImmobilize(ProtoActor actor)
				{
					actor.StopAnimByAbnormal();
					animationCoroutine = null;
				}
			}

			public class CCImmobilizeData : CCBehaivorBase
			{
				public AbnormalCCInfo ccinfo;

				public TargetHitInfo skillTargetHitInfo;

				public CCImmobilizeData(long syncId, TargetHitInfo hitInfo)
					: base(syncId, hitInfo.actionAbnormalHitType)
				{
					skillTargetHitInfo = hitInfo;
					defAbnormalMasterData = GetDefAbnormalMasterDataWithCCType(hitInfo.actionAbnormalHitType);
				}

				public CCImmobilizeData(long syncId, AbnormalCCInfo ccinfo)
					: base(syncId, ccinfo.ccType)
				{
					this.ccinfo = ccinfo;
					defAbnormalMasterData = GetDefAbnormalMasterDataWithCCType(ccinfo.ccType);
				}

				private PosWithRot GetBasePosition()
				{
					if (ccinfo != null)
					{
						return ccinfo.basePosition;
					}
					return skillTargetHitInfo.basePosition;
				}

				private Quaternion GetBaseRotation()
				{
					if (ccinfo != null)
					{
						return ccinfo.basePosition.toRotation();
					}
					return skillTargetHitInfo.basePosition.toRotation();
				}

				private PosWithRot GetDestination()
				{
					if (ccinfo != null)
					{
						return ccinfo.hitPosition;
					}
					return skillTargetHitInfo.hitPosition;
				}

				private long GetDurationMSec()
				{
					if (ccinfo != null)
					{
						if (!IsActionAbnormal())
						{
							return ccinfo.duration;
						}
						return ccinfo.pushTime;
					}
					return skillTargetHitInfo.pushTime;
				}

				public override void PlayImmobilize(ProtoActor actor)
				{
					actor.PlayImmobilizeByAbnormal();
					actor.InvalidateMoveSyncTarget();
					Hub.s.timeutil.GetCurrentTickMilliSec();
					animationCoroutine = actor.StartCoroutine(CorPlayImmobilize(actor));
				}

				private IEnumerator CorPlayImmobilize(ProtoActor actor)
				{
					PosWithRot basePosition = GetBasePosition();
					PosWithRot destination = GetDestination();
					float durationSec = (float)GetDurationMSec() / 1000f;
					Rotator rotationForce = destination.toRotation();
					rotationForce.Pitch = 0f;
					actor.SetRotationForce(rotationForce);
					Vector3 start = basePosition.toVector3();
					Vector3 dest = destination.toVector3();
					yield return actor.CorMove(start, dest, durationSec);
					actor.StopImmobilizeByAbnormal();
				}

				public override void CancelImmobilize(ProtoActor actor)
				{
					actor.StopImmobilizeByAbnormal();
					if (animationCoroutine != null)
					{
						actor.StopCoroutine(animationCoroutine);
						animationCoroutine = null;
						Hub.s.timeutil.GetCurrentTickMilliSec();
					}
				}

				private bool IsActionAbnormal()
				{
					AbnormalCCInfo abnormalCCInfo = ccinfo;
					if (abnormalCCInfo == null || abnormalCCInfo.ccType != CCType.NormalPush)
					{
						AbnormalCCInfo abnormalCCInfo2 = ccinfo;
						if (abnormalCCInfo2 == null || abnormalCCInfo2.ccType != CCType.Airborne)
						{
							AbnormalCCInfo abnormalCCInfo3 = ccinfo;
							if (abnormalCCInfo3 == null || abnormalCCInfo3.ccType != CCType.Knockback)
							{
								AbnormalCCInfo abnormalCCInfo4 = ccinfo;
								if (abnormalCCInfo4 == null)
								{
									return false;
								}
								return abnormalCCInfo4.ccType == CCType.Knockdown;
							}
						}
					}
					return true;
				}
			}

			private ProtoActor owner;

			private readonly Stack<ParticleEffectData> particleEffectPool = new Stack<ParticleEffectData>();

			private readonly Stack<ScreenEffectData> screenEffectPool = new Stack<ScreenEffectData>();

			private readonly Stack<DecalEffectData> decalEffectPool = new Stack<DecalEffectData>();

			private Dictionary<long, ParticleEffectData> currentPlayedParticleEffects = new Dictionary<long, ParticleEffectData>();

			private Dictionary<long, ScreenEffectData> currentPlayedScreenEffects = new Dictionary<long, ScreenEffectData>();

			private Dictionary<long, PlaySoundResult> currentPlayedLoopingResults = new Dictionary<long, PlaySoundResult>();

			private Dictionary<long, DecalEffectData> currentPlayedDecaling = new Dictionary<long, DecalEffectData>();

			private int currentAppliedMaterialSkillTargetEffectId;

			private CCImmobilizeData currentCCImmobile;

			private CCAnimData currentCCAnimation;

			private DataManager dataman => Hub.s.dataman;

			private ParticleEffectData GetParticleEffectData()
			{
				if (particleEffectPool.Count <= 0)
				{
					return new ParticleEffectData();
				}
				return particleEffectPool.Pop();
			}

			private void ReturnParticleEffectData(ParticleEffectData data)
			{
				data.Reset();
				particleEffectPool.Push(data);
			}

			private ScreenEffectData GetScreenEffectData()
			{
				if (screenEffectPool.Count <= 0)
				{
					return new ScreenEffectData();
				}
				return screenEffectPool.Pop();
			}

			private void ReturnScreenEffectData(ScreenEffectData data)
			{
				data.Reset();
				screenEffectPool.Push(data);
			}

			private DecalEffectData GetDecalEffectData()
			{
				if (decalEffectPool.Count <= 0)
				{
					return new DecalEffectData();
				}
				return decalEffectPool.Pop();
			}

			private void ReturnDecalEffectData(DecalEffectData data)
			{
				data.Reset();
				decalEffectPool.Push(data);
			}

			public EffectPlayer(ProtoActor owner)
			{
				this.owner = owner;
			}

			public void Clear()
			{
				foreach (long key in currentPlayedScreenEffects.Keys)
				{
					if (currentPlayedScreenEffects.TryGetValue(key, out var value))
					{
						value.CancelEffect();
						ReturnScreenEffectData(value);
					}
				}
				currentPlayedScreenEffects.Clear();
			}

			public void PlayParticle(int skillTargetEffectId)
			{
				if (HasParticle(skillTargetEffectId))
				{
					ParticleEffectData particleEffectData = GetParticleEffectData();
					owner.StartCoroutine(PlayAndReturnParticle(particleEffectData, skillTargetEffectId));
				}
			}

			private IEnumerator PlayAndReturnParticle(ParticleEffectData particleEffect, int skillTargetEffectId)
			{
				yield return particleEffect.PlayEffect(owner, skillTargetEffectId);
				ReturnParticleEffectData(particleEffect);
			}

			public void PlayScreenEffect(int skillTargetEffectId)
			{
				if (HasScreenEffect(skillTargetEffectId))
				{
					ScreenEffectData screenEffectData = GetScreenEffectData();
					owner.StartCoroutine(PlayAndReturnScreenEffect(screenEffectData, skillTargetEffectId));
				}
			}

			private IEnumerator PlayAndReturnScreenEffect(ScreenEffectData screenEffect, int skillTargetEffectId)
			{
				yield return screenEffect.PlayEffect(owner, skillTargetEffectId);
				ReturnScreenEffectData(screenEffect);
			}

			private IEnumerator PlayParticleEffect(ParticleEffectData particleEffect, int skillTargetEffectId)
			{
				if (HasParticle(skillTargetEffectId))
				{
					yield return particleEffect.PlayEffect(owner, skillTargetEffectId);
				}
			}

			public void PlayParticle(long syncId, int skillTargetEffectId)
			{
				if (HasParticle(skillTargetEffectId))
				{
					ParticleEffectData particleEffectData = GetParticleEffectData();
					owner.StartCoroutine(PlayParticleEffect(particleEffectData, skillTargetEffectId));
					currentPlayedParticleEffects.Add(syncId, particleEffectData);
				}
			}

			private IEnumerator PlayScreenEffect(ScreenEffectData screenEffect, int skillTargetEffectId)
			{
				if (HasScreenEffect(skillTargetEffectId))
				{
					yield return screenEffect.PlayEffect(owner, skillTargetEffectId);
				}
			}

			public void PlayScreenEffect(long syncId, int skillTargetEffectId)
			{
				if (HasScreenEffect(skillTargetEffectId))
				{
					ScreenEffectData screenEffectData = GetScreenEffectData();
					owner.StartCoroutine(PlayScreenEffect(screenEffectData, skillTargetEffectId));
					currentPlayedScreenEffects.Add(syncId, screenEffectData);
				}
			}

			public void PlaySoundLooping(long syncId, int skillTargetEffectId)
			{
				if (HasSoundEffect(skillTargetEffectId))
				{
					PlaySoundResult playSoundResult = SoundEffectData.PlayLooping(owner, skillTargetEffectId);
					if (playSoundResult != null)
					{
						currentPlayedLoopingResults.Add(syncId, playSoundResult);
					}
				}
			}

			public void PlaySoundRinging(long syncId, int skillTargetEffectId)
			{
				if (HasSoundEffect(skillTargetEffectId))
				{
					PlaySoundResult playSoundResult = SoundEffectData.PlayLooping(owner, skillTargetEffectId);
					if (playSoundResult != null)
					{
						currentPlayedLoopingResults.Add(syncId, playSoundResult);
					}
				}
			}

			public void PlaySoundOneShot(int skillTargetEffectId)
			{
				if (HasSoundEffect(skillTargetEffectId))
				{
					SoundEffectData.PlayOneShot(owner, skillTargetEffectId);
				}
			}

			private bool HasParticle(int SkillTargetEffectId)
			{
				SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(SkillTargetEffectId);
				if (skillTargetEffectDataInfo == null)
				{
					return false;
				}
				if (skillTargetEffectDataInfo.StartParticle.Length > 0 || skillTargetEffectDataInfo.LoopParticle.Length > 0 || skillTargetEffectDataInfo.EndParticle.Length > 0)
				{
					return true;
				}
				return false;
			}

			public void PlayAbnormalParticle(long syncAbnormalId, int abnormalMasterId)
			{
				AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
				if (abnormalInfo != null && HasParticle(abnormalInfo.SkillTargetEffectId))
				{
					ParticleEffectData particleEffectData = GetParticleEffectData();
					owner.StartCoroutine(PlayParticleEffect(particleEffectData, abnormalInfo.SkillTargetEffectId));
					currentPlayedParticleEffects.Add(syncAbnormalId, particleEffectData);
				}
			}

			public void StopParticle(long syncId)
			{
				if (currentPlayedParticleEffects.TryGetValue(syncId, out var value))
				{
					currentPlayedParticleEffects.Remove(syncId);
					value.CancelEffect();
					ReturnParticleEffectData(value);
				}
			}

			private bool HasScreenEffect(int skillTargetEffectId)
			{
				SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
				if (skillTargetEffectDataInfo == null)
				{
					return false;
				}
				if (skillTargetEffectDataInfo.StartScreenEffect.Length > 0 || skillTargetEffectDataInfo.LoopScreenEffect.Length > 0 || skillTargetEffectDataInfo.EndScreenEffect.Length > 0)
				{
					return true;
				}
				return false;
			}

			public void PlayAbnormalScreenEffect(long syncAbnormalId, int abnormalMasterId)
			{
				AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
				if (abnormalInfo != null && HasScreenEffect(abnormalInfo.SkillTargetEffectId))
				{
					ScreenEffectData screenEffectData = GetScreenEffectData();
					owner.StartCoroutine(PlayScreenEffect(screenEffectData, abnormalInfo.SkillTargetEffectId));
					currentPlayedScreenEffects.Add(syncAbnormalId, screenEffectData);
				}
			}

			public void StopScreenEffect(long syncId)
			{
				if (currentPlayedScreenEffects.TryGetValue(syncId, out var value))
				{
					currentPlayedScreenEffects.Remove(syncId);
					value.CancelEffect();
					ReturnScreenEffectData(value);
				}
			}

			private bool HasSoundEffect(int skillTargetEffectId)
			{
				SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
				if (skillTargetEffectDataInfo == null)
				{
					return false;
				}
				if (skillTargetEffectDataInfo.PlaySounds.Count > 0 && skillTargetEffectDataInfo.PlaySounds[0].Length > 0)
				{
					return true;
				}
				return false;
			}

			public void PlayAbnormalSoundEffect(long syncId, int abnormalMasterId)
			{
				AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
				if (abnormalInfo == null)
				{
					return;
				}
				SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(abnormalInfo.SkillTargetEffectId);
				if (HasSoundEffect(abnormalInfo.SkillTargetEffectId))
				{
					if (abnormalInfo.ElementList.Count((KeyValuePair<int, IAbnormalElementInfo> e) => e.Value is CCElementInfo cCElementInfo && cCElementInfo.CCType == CCType.Ringing) > 0)
					{
						PlaySoundRinging(syncId, abnormalInfo.SkillTargetEffectId);
					}
					else if (skillTargetEffectDataInfo.StopSoundOnAbnormalEnd)
					{
						PlaySoundLooping(syncId, abnormalInfo.SkillTargetEffectId);
					}
					else
					{
						PlaySoundOneShot(abnormalInfo.SkillTargetEffectId);
					}
				}
			}

			public void TryStopSoundEffect(long syncId)
			{
				if (currentPlayedLoopingResults.TryGetValue(syncId, out var value))
				{
					if (value.ActingVariation != null)
					{
						value.ActingVariation.Stop();
					}
					currentPlayedLoopingResults.Remove(syncId);
				}
			}

			public void TryStopAbnormalSoundEffect(long syncAbnormalId, int abnormalMasterId)
			{
				AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
				if (abnormalInfo != null)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(abnormalInfo.SkillTargetEffectId);
					if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.StopSoundOnAbnormalEnd)
					{
						TryStopSoundEffect(syncAbnormalId);
					}
				}
			}

			private bool HasDecalEffect(int skillTargetEffectId)
			{
				SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
				if (skillTargetEffectDataInfo == null)
				{
					return false;
				}
				if (skillTargetEffectDataInfo.DecalPathWithSocket.Length > 0)
				{
					return true;
				}
				return false;
			}

			public void PlayAbnormalDecaling(long syncId, int abnormalMasterID)
			{
				AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterID);
				if (abnormalInfo != null && HasDecalEffect(abnormalInfo.SkillTargetEffectId))
				{
					DecalEffectData decalEffectData = GetDecalEffectData();
					decalEffectData.PlayEffect(owner, abnormalInfo.SkillTargetEffectId);
					currentPlayedDecaling.Add(syncId, decalEffectData);
				}
			}

			public void StopAbnormalDecaling(long syncId)
			{
				if (currentPlayedDecaling.TryGetValue(syncId, out var value))
				{
					currentPlayedDecaling.Remove(syncId);
					value.CancelEffect();
					ReturnDecalEffectData(value);
				}
			}

			public void ApplyAbnormalMaterial(long syncId, int abnormalMasterId)
			{
				AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
				if (abnormalInfo == null)
				{
					Logger.RError($"ApplyAbnormalMaterial: {owner.ActorID}, {syncId}, {abnormalMasterId}, abnormalInfo is null");
					return;
				}
				int skillTargetEffectId = abnormalInfo.SkillTargetEffectId;
				if (skillTargetEffectId != 0)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.ActorDecalColorId.Length != 0)
					{
						Color color = Hub.s.tableman.color.GetColor(skillTargetEffectDataInfo.ActorDecalColorId);
						owner.TurnOnMaterialPaint(color);
						currentAppliedMaterialSkillTargetEffectId = skillTargetEffectId;
					}
				}
			}

			public void RestoreAbnormalMaterial(long syncId, int abnormalMasterId)
			{
				AbnormalInfo abnormalInfo = AbnormalHelper.GetAbnormalInfo(abnormalMasterId);
				if (abnormalInfo == null)
				{
					Logger.RError($"RestoreAbnormalMaterial: {owner.ActorID}, {syncId}, {abnormalMasterId}, abnormalInfo is null");
				}
				else if (currentAppliedMaterialSkillTargetEffectId == abnormalInfo.SkillTargetEffectId)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(currentAppliedMaterialSkillTargetEffectId);
					if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.ActorDecalColorId.Length != 0)
					{
						owner.TurnOffMaterialPaint((float)skillTargetEffectDataInfo.ActorDecalFadeoutMSec / 1000f);
						currentAppliedMaterialSkillTargetEffectId = 0;
					}
				}
			}

			public static DefAbnormal_MasterData GetDefAbnormalMasterDataWith(int category, string type)
			{
				KeyValuePair<int, DefAbnormal_MasterData> keyValuePair = Hub.s.dataman.ExcelDataManager.DefAbnormalDict.FirstOrDefault<KeyValuePair<int, DefAbnormal_MasterData>>((KeyValuePair<int, DefAbnormal_MasterData> d) => d.Value.category == category && d.Value.key == type);
				if (keyValuePair.Value != null)
				{
					return keyValuePair.Value;
				}
				return null;
			}

			public static DefAbnormal_MasterData GetDefAbnormalMasterDataWithCCType(CCType cctype)
			{
				DefAbnormal_MasterData value = null;
				Hub.s.dataman.ExcelDataManager.CCAbnormalDict.TryGetValue(cctype, out value);
				return value;
			}

			public static AbnormalInfo GetAbnormalInfo(int abnormalMasterId)
			{
				AbnormalInfo value = null;
				Hub.s.dataman.ExcelDataManager.AbnormalDict.TryGetValue(abnormalMasterId, out value);
				return value;
			}

			public void UpdateTargetEffectAnimByAbnomal(List<AbnormalObjectInfo> added, List<AbnormalObjectInfo> removed, List<AbnormalObjectInfo> changed)
			{
			}

			public void UpdateAnimationByAbnormal(List<AbnormalObjectInfo> added, List<AbnormalObjectInfo> removed, List<AbnormalObjectInfo> changed)
			{
				bool flag = false;
				CCBehaivorBase cCBehaivorBase = null;
				if (currentCCAnimation != null && removed.Find((AbnormalObjectInfo d) => d.abnormalSyncID == currentCCAnimation.syncId) != null)
				{
					flag = true;
					cCBehaivorBase = currentCCAnimation;
					currentCCAnimation = null;
				}
				CCBehaivorBase cCBehaivorBase2 = null;
				if (added.Count > 0)
				{
					List<AbnormalObjectInfo> list = added.FindAll(delegate(AbnormalObjectInfo d)
					{
						AbnormalInfo abnormalInfo = GetAbnormalInfo(d.abnormalMasterID);
						if (abnormalInfo != null)
						{
							SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(abnormalInfo.SkillTargetEffectId);
							if (skillTargetEffectDataInfo != null)
							{
								return skillTargetEffectDataInfo.animation.Length > 0;
							}
						}
						return false;
					});
					list.Sort(delegate(AbnormalObjectInfo c1, AbnormalObjectInfo c2)
					{
						var (flag3, num2) = CCAnimData.GetHighestMotionPriority(c1.abnormalMasterID);
						var (flag4, num3) = CCAnimData.GetHighestMotionPriority(c2.abnormalMasterID);
						if (flag3 && !flag4)
						{
							return 1;
						}
						if (!flag3 && flag4)
						{
							return -1;
						}
						if (num2 > num3)
						{
							return 1;
						}
						return (num2 < num3) ? (-1) : 0;
					});
					if (list.Count > 0)
					{
						if (currentCCAnimation != null)
						{
							var (flag2, num) = CCAnimData.GetHighestMotionPriority(list[0].abnormalMasterID);
							if (flag2 && num >= currentCCAnimation.HighestMotionPriority)
							{
								cCBehaivorBase = currentCCAnimation;
								currentCCAnimation = new CCAnimData(list[0].abnormalSyncID, list[0].abnormalMasterID);
								cCBehaivorBase2 = currentCCAnimation;
							}
						}
						else
						{
							currentCCAnimation = new CCAnimData(list[0].abnormalSyncID, list[0].abnormalMasterID);
							cCBehaivorBase2 = currentCCAnimation;
						}
					}
				}
				if (cCBehaivorBase2 != null)
				{
					cCBehaivorBase2.PlayImmobilize(owner);
				}
				else if (flag)
				{
					cCBehaivorBase.CancelImmobilize(owner);
				}
				cCBehaivorBase = null;
			}

			public void UpdateImmobilizeByAbnormalCC(List<AbnormalCCInfo> added, List<AbnormalCCInfo> removed, List<AbnormalCCInfo> changed)
			{
				bool flag = false;
				CCBehaivorBase cCBehaivorBase = null;
				if (currentCCImmobile != null && removed.Find((AbnormalCCInfo d) => d.syncID == currentCCImmobile.syncId) != null)
				{
					flag = true;
					cCBehaivorBase = currentCCImmobile;
					currentCCImmobile = null;
				}
				CCBehaivorBase cCBehaivorBase2 = null;
				if (added.Count > 0)
				{
					added.Sort(delegate(AbnormalCCInfo c1, AbnormalCCInfo c2)
					{
						DefAbnormal_MasterData defAbnormalMasterDataWithCCType2 = GetDefAbnormalMasterDataWithCCType(c1.ccType);
						DefAbnormal_MasterData defAbnormalMasterDataWithCCType3 = GetDefAbnormalMasterDataWithCCType(c2.ccType);
						if (defAbnormalMasterDataWithCCType2.motion_cancel && !defAbnormalMasterDataWithCCType3.motion_cancel)
						{
							return 1;
						}
						if (!defAbnormalMasterDataWithCCType2.motion_cancel && defAbnormalMasterDataWithCCType3.motion_cancel)
						{
							return -1;
						}
						if (defAbnormalMasterDataWithCCType2.motion_priority > defAbnormalMasterDataWithCCType3.motion_priority)
						{
							return 1;
						}
						return (defAbnormalMasterDataWithCCType2.motion_priority < defAbnormalMasterDataWithCCType3.motion_priority) ? (-1) : 0;
					});
					if (added.Count > 0)
					{
						if (currentCCImmobile != null)
						{
							DefAbnormal_MasterData defAbnormalMasterDataWithCCType = GetDefAbnormalMasterDataWithCCType(added[0].ccType);
							if (defAbnormalMasterDataWithCCType.motion_cancel && defAbnormalMasterDataWithCCType.motion_priority >= currentCCImmobile.defAbnormalMasterData.motion_priority)
							{
								cCBehaivorBase = currentCCImmobile;
								currentCCImmobile = new CCImmobilizeData(added[0].syncID, added[0]);
								cCBehaivorBase2 = currentCCImmobile;
							}
						}
						else
						{
							currentCCImmobile = new CCImmobilizeData(added[0].syncID, added[0]);
							cCBehaivorBase2 = currentCCImmobile;
						}
					}
				}
				if (flag)
				{
					cCBehaivorBase.CancelImmobilize(owner);
				}
				cCBehaivorBase2?.PlayImmobilize(owner);
				cCBehaivorBase = null;
			}

			public void UpdateImmobilizeAnimByHit(TargetHitInfo skillTargetHitInfo)
			{
				DefAbnormal_MasterData defAbnormalMasterDataWithCCType = GetDefAbnormalMasterDataWithCCType(skillTargetHitInfo.actionAbnormalHitType);
				if (currentCCImmobile == null || (defAbnormalMasterDataWithCCType.motion_cancel && defAbnormalMasterDataWithCCType.motion_priority >= currentCCImmobile.defAbnormalMasterData.motion_priority))
				{
					if (currentCCImmobile != null)
					{
						currentCCImmobile.CancelImmobilize(owner);
					}
					currentCCImmobile = new CCImmobilizeData(0L, skillTargetHitInfo);
					currentCCImmobile.PlayImmobilize(owner);
				}
			}

			public void PlaySkillHitAnim(int skillTargetEffectId)
			{
				if (currentCCImmobile == null)
				{
					SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
					if (skillTargetEffectDataInfo != null && skillTargetEffectDataInfo.animation != string.Empty)
					{
						owner.puppet.PlayHitAnim(skillTargetEffectDataInfo.animation, skillTargetEffectDataInfo.loopAnimation);
					}
				}
			}
		}

		public class EmotePlayer
		{
			private static readonly InputAction[] stopEmoteInputActions = new InputAction[4]
			{
				InputAction.MoveForward,
				InputAction.MoveBackward,
				InputAction.MoveLeft,
				InputAction.MoveRight
			};

			private ProtoActor owner;

			private CancellationToken destroyToken;

			private bool isSendingStart;

			private bool hasBeenCanceledWhileSendingStart;

			private Transform? hiddenHandheldParent;

			private PuppetScript? puppet
			{
				get
				{
					if (!(owner != null))
					{
						return null;
					}
					return owner.puppet;
				}
			}

			private GameMainBase? main
			{
				get
				{
					if (!(owner != null))
					{
						return null;
					}
					return owner.main;
				}
			}

			public EmotePlayer(ProtoActor owner)
			{
				this.owner = owner;
				destroyToken = this.owner.destroyCancellationToken;
			}

			public bool IsEmotePlaying()
			{
				if (puppet != null)
				{
					return puppet.IsEmotePlaying;
				}
				return false;
			}

			public bool TryStartEmoteByInput()
			{
				if (owner == null || owner.hasMovementInput || Hub.s == null || Hub.s.tableman == null || Hub.s.inputman == null)
				{
					return false;
				}
				foreach (InputAction item in Hub.s.tableman.emote.CollectInputActionList())
				{
					if (Hub.s.inputman.wasPressedThisFrame(item) && Hub.s.tableman.emote.TryGetEmote(item, out MMEmoteTable.Emote emote) && emote != null)
					{
						SendStartEmote(emote.emoteMasterID);
						return true;
					}
				}
				return false;
			}

			public bool TryStopEmoteByInput()
			{
				if (puppet == null || !puppet.IsEmotePlaying || Hub.s == null || Hub.s.inputman == null)
				{
					return false;
				}
				InputAction[] array = stopEmoteInputActions;
				foreach (InputAction action in array)
				{
					if (Hub.s.inputman.wasPressedThisFrame(action))
					{
						puppet.StopEmote();
						return true;
					}
				}
				return false;
			}

			public void SendStartEmote(int emoteMasterID)
			{
				if (isSendingStart)
				{
					return;
				}
				isSendingStart = true;
				main?.SendPacketWithCallback(new EmotionReq
				{
					emotionMasterID = emoteMasterID,
					basePosition = owner.transform.toPosWithRot()
				}, delegate(EmotionRes _res)
				{
					isSendingStart = false;
					if (_res == null)
					{
						Logger.RError("EmotionRes is null.");
					}
					else if (_res.errorCode != MsgErrorCode.CantAction && _res.errorCode != MsgErrorCode.CantActionByUsingSkill && _res.errorCode != MsgErrorCode.CantEmotion)
					{
						if (_res.errorCode != MsgErrorCode.Success)
						{
							Logger.RError($"Failed to start emote: {_res.errorCode}");
						}
						else if (hasBeenCanceledWhileSendingStart)
						{
							hasBeenCanceledWhileSendingStart = false;
						}
						else
						{
							StartEmote(_res.emotionMasterID);
						}
					}
				}, destroyToken);
			}

			public void OnCancelEmoteSig(CancelEmotionSig sig)
			{
				if (isSendingStart)
				{
					hasBeenCanceledWhileSendingStart = true;
				}
				else
				{
					StopEmote();
				}
			}

			public void StartEmote(int emoteMasterID)
			{
				if (!(puppet == null) && !(Hub.s == null) && !(Hub.s.tableman == null) && Hub.s.tableman.emote.TryGetEmote(emoteMasterID, out MMEmoteTable.Emote emote) && emote != null)
				{
					puppet.StartEmote(emote.motionName);
					SetHandheldVisibility(visible: false);
				}
			}

			public void StopEmote()
			{
				if (!(puppet == null))
				{
					puppet.StopEmote();
					SetHandheldVisibility(visible: true);
				}
			}

			private void SetHandheldVisibility(bool visible)
			{
				if (visible)
				{
					if (!(hiddenHandheldParent != null))
					{
						return;
					}
					hiddenHandheldParent.gameObject.SetActive(value: true);
					hiddenHandheldParent = null;
					if (owner != null)
					{
						InventoryItem handheldItem = owner.GetHandheldItem();
						if (handheldItem != null && handheldItem.TryGetComponent<SocketAttachable>(out SocketAttachable component))
						{
							component.OnAttachToSocket();
						}
					}
					return;
				}
				InventoryItem handheldItem2 = owner.GetHandheldItem();
				if (handheldItem2 != null && handheldItem2.MasterInfo.HideItemByEmote && handheldItem2.Transform != null)
				{
					if (handheldItem2.TryGetComponent<SocketAttachable>(out SocketAttachable component2))
					{
						component2.OnDetachFromSocket();
					}
					hiddenHandheldParent = handheldItem2.Transform.parent;
					hiddenHandheldParent.gameObject.SetActive(value: false);
				}
			}
		}

		public class FakeJumper
		{
			private const InputAction jumpInputAction = InputAction.Jump;

			private readonly float jumpCooldown;

			private readonly ProtoActor owner;

			private readonly CancellationToken destroyCancellationToken;

			private float lastJumpTime;

			private CancellationTokenSource? jumpCts;

			private PuppetScript? puppet
			{
				get
				{
					if (!(owner != null))
					{
						return null;
					}
					return owner.puppet;
				}
			}

			public FakeJumper(ProtoActor owner)
			{
				this.owner = owner;
				destroyCancellationToken = ((owner != null) ? owner.destroyCancellationToken : CancellationToken.None);
				jumpCooldown = 0.85f;
			}

			public bool IsJumping()
			{
				if (!(puppet != null))
				{
					return false;
				}
				return puppet.IsJumping;
			}

			public bool TryStartJumpByInput()
			{
				if (CanJump() && Hub.s != null && Hub.s.inputman != null && Hub.s.inputman.isPressed(InputAction.Jump))
				{
					SendStartJump();
					return true;
				}
				return false;
			}

			private bool CanJump()
			{
				if (Time.time < lastJumpTime + jumpCooldown)
				{
					return false;
				}
				if (owner.isGrabbing)
				{
					return false;
				}
				return true;
			}

			private void StartJump()
			{
				StopJump();
				PuppetScript puppetScript = owner.puppet;
				if (!(puppetScript == null))
				{
					puppetScript.StartJump();
					jumpCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
					UniTask.Void(async delegate(CancellationToken cancellationToken)
					{
						await UniTask.WaitForSeconds(jumpCooldown, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken, cancelImmediately: true).SuppressCancellationThrow();
						StopJump();
					}, jumpCts.Token);
				}
			}

			private void StopJump()
			{
				if (jumpCts != null)
				{
					jumpCts?.Cancel();
					jumpCts?.Dispose();
					jumpCts = null;
				}
				PuppetScript puppetScript = owner.puppet;
				if (!(puppetScript == null))
				{
					puppetScript.StopJump();
				}
			}

			private void SendStartJump()
			{
				GameMainBase main = owner.main;
				if (!(main == null))
				{
					main.SendPacket(new JumpReq());
					StartJump();
					lastJumpTime = Time.time;
				}
			}

			public void OnJumpSig(JumpSig sig)
			{
				StartJump();
			}

			public void OnCancelJumpSig(CancelJumpSig sig)
			{
				StopJump();
			}
		}

		private class GrabHelper
		{
			private readonly Vector3 defaultTargetRotation = new Vector3(0f, 180f, 0f);

			private ProtoActor grabber;

			public ProtoActor? target;

			public int grabMasterID;

			private CancellationTokenSource? grabCts;

			public GrabHelper(ProtoActor grabber)
			{
				this.grabber = grabber;
			}

			~GrabHelper()
			{
				CancelGrab();
			}

			public void OnAttachActorSig(AttachActorSig sig)
			{
				if (Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.main == null || Hub.s.dataman == null || Hub.s.dataman.ExcelDataManager == null)
				{
					return;
				}
				ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(sig.targetID);
				if (actorByActorID == null)
				{
					Logger.RWarn($"AttachActorSig: target actor not found (actorID={sig.targetID})");
					return;
				}
				AttachMasterInfo attachInfo = Hub.s.dataman.ExcelDataManager.GetAttachInfo(sig.grabMasterID);
				if (attachInfo == null)
				{
					Logger.RWarn($"AttachActorSig: attachInfo not found (grabMasterID={sig.grabMasterID})");
				}
				else if (sig.state == AttachState.Attached)
				{
					GrabData_socket_info grabData_socket_info = attachInfo.SocketInfoList.Find((GrabData_socket_info s) => s.index == sig.socketIndex);
					if (grabData_socket_info != null)
					{
						Vector3 targetPositionOffset = new Vector3(grabData_socket_info.grab_target_attach_object_position_offset[0], grabData_socket_info.grab_target_attach_object_position_offset[1], grabData_socket_info.grab_target_attach_object_position_offset[2]);
						Vector3 targetRotationOffset = new Vector3(grabData_socket_info.grab_target_attach_object_rotation_offset[0], grabData_socket_info.grab_target_attach_object_rotation_offset[1], grabData_socket_info.grab_target_attach_object_rotation_offset[2]);
						if (grabber.AmIAvatar())
						{
							GrabAsync(actorByActorID, grabData_socket_info.grab_socket_name, grabData_socket_info.grab_target_attach_object_name, targetPositionOffset, targetRotationOffset, (float)attachInfo.GrabAttachDuration / 1000f).Forget();
						}
						else
						{
							GrabAsync(actorByActorID, grabData_socket_info.grab_socket_name_third_person, grabData_socket_info.grab_target_attach_object_name_third_person, targetPositionOffset, targetRotationOffset, (float)attachInfo.GrabAttachDuration / 1000f).Forget();
						}
						grabMasterID = sig.grabMasterID;
						if (grabber.IsMimic() && actorByActorID.AmIAvatar())
						{
							actorByActorID.AddIncomingEvent(SpeechEvent_IncomingType.OnDamaged, Time.realtimeSinceStartup + actorByActorID.GetIncomingEventExpireTime(SpeechEvent_IncomingType.OnDamaged));
							Hub.s.voiceman?.TrySendToServerVoiceEmotion(actorByActorID.ActorID, ReplaySharedData.E_EVENT.DEAD_FROM_MIMIC);
						}
					}
				}
				else if (sig.state == AttachState.Detached)
				{
					CancelGrab();
					grabMasterID = 0;
				}
			}

			private async UniTaskVoid GrabAsync(ProtoActor? target, string grabSocketName, string targetSocketName, Vector3 targetPositionOffset, Vector3 targetRotationOffset, float blendTime)
			{
				CancelGrab();
				this.target = target;
				if (target == null)
				{
					Logger.RWarn("target is null in GrabAsync");
					return;
				}
				Transform grabSocket = SocketNodeMarker.FindFirstInHierarchy(grabber.transform, grabSocketName);
				Transform targetSocket = SocketNodeMarker.FindFirstInHierarchy(target.transform, targetSocketName);
				if (!(grabSocket == null) && !(targetSocket == null))
				{
					grabCts = CancellationTokenSource.CreateLinkedTokenSource(grabber.destroyCancellationToken, target.destroyCancellationToken);
					target.SetGrabbedBy(grabSocket);
					grabber.SetGrabber(value: true);
					Quaternion targetRotation = Quaternion.Euler(defaultTargetRotation + targetRotationOffset);
					await BlendTargetTransformAsync(grabSocket, target, targetSocket, targetPositionOffset, targetRotation, blendTime, grabCts.Token);
					await SyncTargetTransformAsync(grabSocket, target, targetSocket, targetPositionOffset, targetRotation, grabCts.Token);
					target.SetGrabbedBy(null);
					grabber.SetGrabber(value: false);
					this.target = null;
				}
			}

			private async UniTask BlendTargetTransformAsync(Transform grabSocket, ProtoActor target, Transform targetSocket, Vector3 targetPositionOffset, Quaternion targetRotation, float blendTime, CancellationToken cancellationToken)
			{
				Vector3 initialTargetPosition = targetSocket.position;
				Quaternion initialTargetRotation = target.transform.localRotation;
				for (float blendingTime = 0f; blendingTime < blendTime; blendingTime += Time.deltaTime)
				{
					if (cancellationToken.IsCancellationRequested)
					{
						break;
					}
					target.transform.position = Vector3.Lerp(initialTargetPosition, grabSocket.position + targetPositionOffset, blendingTime / blendTime);
					target.transform.localRotation = Quaternion.Lerp(initialTargetRotation, targetRotation, blendingTime / blendTime);
					await UniTask.Yield(cancellationToken, cancelImmediately: true).SuppressCancellationThrow();
					if (target == null || grabSocket == null || targetSocket == null)
					{
						break;
					}
				}
				target.transform.position = grabSocket.position + targetPositionOffset;
				target.transform.localRotation = targetRotation;
			}

			private async UniTask SyncTargetTransformAsync(Transform grabSocket, ProtoActor target, Transform targetSocket, Vector3 targetPositionOffset, Quaternion targetRotation, CancellationToken cancellationToken)
			{
				while (!cancellationToken.IsCancellationRequested)
				{
					Vector3 vector = grabSocket.position + targetPositionOffset;
					Vector3 position = targetSocket.position;
					Vector3 vector2 = vector - position;
					target.transform.position += vector2;
					target.transform.localRotation = targetRotation;
					await UniTask.Yield(cancellationToken, cancelImmediately: true).SuppressCancellationThrow();
					if (target == null || grabSocket == null || targetSocket == null)
					{
						break;
					}
				}
			}

			public void CancelGrab()
			{
				if (grabCts != null)
				{
					grabCts.Cancel();
					grabCts.Dispose();
					grabCts = null;
				}
			}
		}

		public enum EInputDisableReason
		{
			None = 0,
			GameNotStarted = 1,
			Abnormal = 2,
			SkillCast = 4,
			GrabbedByOther = 8
		}

		public enum EInputMoveDisableReason
		{
			None = 0,
			Abnormal = 1,
			OccupyingLevelObject = 2
		}

		public class Inventory
		{
			private readonly ProtoActor owner;

			private readonly CancellationToken destroyCancellationToken;

			private readonly GameMainBase? main;

			private readonly int slotSize;

			private List<InventoryItem?> slotItems;

			private int selectedSlotIndex;

			private bool isGrabLootingPacketRequested;

			private bool isEndScrapMotionPacketRequested;

			private CancellationTokenSource? scrapMotionCts;

			private CancellationTokenSource? cycleRandomEventCts;

			public List<InventoryItem?> SlotItems => slotItems;

			public int SelectedSlotIndex => selectedSlotIndex;

			public InventoryItem? SelectedItem => GetSelectedItem();

			private PuppetScript? puppet
			{
				get
				{
					if (!(owner != null))
					{
						return null;
					}
					return owner.puppet;
				}
			}

			public Inventory(ProtoActor owner)
			{
				this.owner = owner;
				destroyCancellationToken = ((owner != null) ? owner.destroyCancellationToken : CancellationToken.None);
				main = ((owner != null) ? owner.main : null);
				if (Hub.s != null && Hub.s.gameConfig != null && Hub.s.gameConfig.playerActor != null)
				{
					slotSize = Hub.s.gameConfig.playerActor.maxGenericInventorySlot;
				}
				else
				{
					slotSize = 4;
				}
				selectedSlotIndex = 0;
				slotItems = new List<InventoryItem>(slotSize);
				for (int i = 0; i < slotSize; i++)
				{
					slotItems.Add(null);
				}
				scrapMotionCts = null;
			}

			public static ItemMasterInfo? GetItemMasterInfo(int itemMasterID)
			{
				return Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
			}

			public bool IsScrapMotionPlaying()
			{
				if (puppet != null)
				{
					return puppet.IsScrapMotionPlaying;
				}
				return false;
			}

			public void StartScrapMotion()
			{
				StopScrapMotion();
				InventoryItem selectedItem = SelectedItem;
				if (selectedItem == null || !selectedItem.TryCastMasterInfo<ItemMiscellanyInfo>(out ItemMiscellanyInfo masterInfo))
				{
					return;
				}
				string scrapAnimationStateName = masterInfo.ScrapAnimationStateName;
				if (string.IsNullOrWhiteSpace(scrapAnimationStateName))
				{
					return;
				}
				owner.StartScrapMotion(scrapAnimationStateName, out var stateInfo);
				if (!masterInfo.IsLoopScrapAnimation)
				{
					float animationDuration = stateInfo.length;
					scrapMotionCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
					UniTask.Void(async delegate(CancellationToken scrapMotionCancellationToken)
					{
						await UniTask.WaitForSeconds(animationDuration, ignoreTimeScale: false, PlayerLoopTiming.Update, scrapMotionCancellationToken, cancelImmediately: true).SuppressCancellationThrow();
						SendEndScrapMotion();
					}, scrapMotionCts.Token);
				}
				Transform transform = selectedItem.Transform;
				if (transform != null && transform.TryGetComponent<IMotionableItem>(out var component))
				{
					component.OnMotionStarted();
				}
			}

			public void StopScrapMotion()
			{
				if (scrapMotionCts != null)
				{
					scrapMotionCts.Cancel();
					scrapMotionCts.Dispose();
					scrapMotionCts = null;
				}
				InventoryItem selectedItem = SelectedItem;
				if (selectedItem != null)
				{
					Transform transform = selectedItem.Transform;
					if (transform != null && transform.TryGetComponent<IMotionableItem>(out var component))
					{
						component.OnMotionStopped();
					}
				}
				owner.StopScrapMotion();
			}

			public void SelectNextSlot()
			{
				int slotIndex = (selectedSlotIndex + 1) % slotSize;
				SelectSlot(slotIndex);
			}

			public void SelectPreviousSlot()
			{
				int slotIndex = (selectedSlotIndex - 1 + slotSize) % slotSize;
				SelectSlot(slotIndex);
			}

			private void SelectSlot(int slotIndex)
			{
				if (!isGrabLootingPacketRequested)
				{
					if (SelectedItem != null && SelectedItem.MasterInfo.ForbidChange)
					{
						Hub.s.tableman.uiprefabs.ShowTimerDialog("ToastSimple", 0f, Hub.GetL10NText("STRING_DO_NOT_WHEEL"));
					}
					else
					{
						SendChangeActiveInvenSlot(slotIndex);
					}
				}
			}

			private void SelectForbidChangeSlot()
			{
				for (int i = 0; i < slotItems.Count; i++)
				{
					InventoryItem inventoryItem = slotItems[i];
					if (inventoryItem != null && inventoryItem.MasterInfo.ForbidChange && selectedSlotIndex != i)
					{
						SelectSlot(i);
						break;
					}
				}
			}

			private InventoryItem? GetSelectedItem()
			{
				if (slotItems != null && selectedSlotIndex >= 0 && selectedSlotIndex < slotItems.Count)
				{
					return slotItems[selectedSlotIndex];
				}
				return null;
			}

			private void SendChangeActiveInvenSlot(int slotIndex)
			{
				if (main == null)
				{
					return;
				}
				main.SendPacketWithCallback(new ChangeActiveInvenSlotReq
				{
					slotIndex = slotIndex + 1
				}, delegate(ChangeActiveInvenSlotRes res)
				{
					if (res.errorCode != MsgErrorCode.CantActionByUsingSkill && res.errorCode != MsgErrorCode.CantAction && res.errorCode != MsgErrorCode.CannotHandleItem && res.errorCode != MsgErrorCode.Success)
					{
						Logger.RError($"ChangeActiveInvenSlotRes: errorCode={res.errorCode}");
					}
				}, destroyCancellationToken);
			}

			public void SendGrapLootingObject(int lootingObjectActorID)
			{
				if (main == null)
				{
					return;
				}
				isGrabLootingPacketRequested = true;
				main.SendPacketWithCallback(new GrapLootingObjectReq
				{
					lootingObjectID = lootingObjectActorID
				}, delegate(GrapLootingObjectRes res)
				{
					isGrabLootingPacketRequested = false;
					if (res.errorCode != MsgErrorCode.ActorNotFound && res.errorCode != MsgErrorCode.CantAction && res.errorCode != MsgErrorCode.CantActionByUsingSkill && res.errorCode != MsgErrorCode.CannotHandleItem && res.errorCode != MsgErrorCode.FakeItemRemoved && res.errorCode != MsgErrorCode.InventoryFull && res.errorCode != MsgErrorCode.LootingObjectAlreadyAssigned && res.errorCode != MsgErrorCode.Success)
					{
						Logger.RError($"GrapLootingObjectRes: errorCode={res.errorCode}");
					}
				}, destroyCancellationToken);
			}

			public void SendReleaseItem()
			{
				if (!(main == null))
				{
					main.SendPacketWithCallback(new ReleaseItemReq(), delegate(ReleaseItemRes res)
					{
						_ = res.errorCode;
					}, destroyCancellationToken);
				}
			}

			public void SendStartScrapMotion()
			{
				if (main == null)
				{
					return;
				}
				main.SendPacketWithCallback(new StartScrapMotionReq
				{
					basePosition = owner.transform.toPosWithRot()
				}, delegate(StartScrapMotionRes res)
				{
					if (res.errorCode == MsgErrorCode.Success)
					{
						ResolveInventoryItemInfo(res.onHandItem);
						StartScrapMotion();
					}
				}, destroyCancellationToken);
			}

			public void OnStartScrapMotionSig(StartScrapMotionSig sig)
			{
				ResolveInventoryItemInfo(sig.onHandItem);
				StartScrapMotion();
			}

			public void SendEndScrapMotion()
			{
				if (main == null || isEndScrapMotionPacketRequested)
				{
					return;
				}
				isEndScrapMotionPacketRequested = true;
				main.SendPacketWithCallback(new EndScrapMotionReq
				{
					basePosition = owner.transform.toPosWithRot()
				}, delegate(EndScrapMotionRes res)
				{
					isEndScrapMotionPacketRequested = false;
					if (res.errorCode != MsgErrorCode.CantAction)
					{
						if (res.errorCode != MsgErrorCode.Success)
						{
							Logger.RError($"EndScrapMotionRes: errorCode={res.errorCode}");
						}
						else
						{
							StopScrapMotion();
						}
					}
				}, destroyCancellationToken);
			}

			public void OnEndScrapMotionSig(EndScrapMotionSig sig)
			{
				StopScrapMotion();
			}

			public void OnCancelScrapMotionSig(CancelScrapMotionSig sig)
			{
				StopScrapMotion();
			}

			public void OnUpdateInvenSig(UpdateInvenSig sig)
			{
				ResolveInventoryInfos(sig.inventoryInfos, null);
			}

			private void StartCycleRandomEventUpdate(int randomSeed, in ItemInfo itemInfo)
			{
				if (cycleRandomEventCts != null)
				{
					cycleRandomEventCts.Cancel();
					cycleRandomEventCts.Dispose();
					cycleRandomEventCts = null;
				}
				if (CheckValidateCycleRandomEventUpdate(itemInfo.itemID, itemInfo.itemMasterID))
				{
					cycleRandomEventCts = CancellationTokenSource.CreateLinkedTokenSource(destroyCancellationToken);
					UpdateCycleRandomEvent(randomSeed, itemInfo.itemID, itemInfo.itemMasterID, cycleRandomEventCts.Token).Forget();
				}
			}

			private bool CheckValidateCycleRandomEventUpdate(long itemID, int itemMasterID)
			{
				if (owner == null || SelectedItem == null || SelectedItem.Transform == null || SelectedItem.ItemMasterID != itemMasterID || SelectedItem.ItemID != itemID)
				{
					return false;
				}
				return true;
			}

			private async UniTask UpdateCycleRandomEvent(int randomSeed, long itemID, int itemMasterID, CancellationToken cancellationToken)
			{
				ItemMasterInfo itemInfo = Hub.s.dataman.ExcelDataManager.GetItemInfo(itemMasterID);
				if (itemInfo == null || !(itemInfo is ItemEquipmentInfo itemEquipmentInfo))
				{
					return;
				}
				SocketAttachable component = null;
				if (!(SelectedItem?.TryGetComponent<SocketAttachable>(out component) ?? false) || !(component is ICycleRandomableItem cycleRandomableItem))
				{
					return;
				}
				int scrapRandomPlayCycle = itemEquipmentInfo.ScrapRandomPlayCycle;
				int scrapRandomPlayRate = itemEquipmentInfo.ScrapRandomPlayRate;
				await UniTask.Delay(itemEquipmentInfo.ScrapRandomPlayStartDelay, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken, cancelImmediately: true).SuppressCancellationThrow();
				GameMainBase.SyncRandom syncRand = new GameMainBase.SyncRandom(randomSeed);
				while (CheckValidateCycleRandomEventUpdate(itemID, itemMasterID) && !cancellationToken.IsCancellationRequested)
				{
					if (syncRand.Next(0, 10000) < scrapRandomPlayRate)
					{
						cycleRandomableItem?.OnCycleRandomEvent(randomSeed);
					}
					await UniTask.Delay(scrapRandomPlayCycle, ignoreTimeScale: false, PlayerLoopTiming.Update, cancellationToken, cancelImmediately: true).SuppressCancellationThrow();
				}
			}

			public void OnChangeItemLooksSig(ChangeItemLooksSig sig)
			{
				if (!(owner == null) && sig != null && sig.onHandItem != null)
				{
					selectedSlotIndex = sig.slotIndex - 1;
					ResolveInventoryItemInfo(sig.onHandItem, selectedSlotIndex);
					owner.EquipHandheld(SelectedItem);
					StartCycleRandomEventUpdate((int)sig.activateTime, sig.onHandItem);
					UpdateSelectedItemComponents();
					owner.UpdateMountedLight();
					if (main != null)
					{
						main.UpdateInventoryUI(owner);
					}
				}
			}

			public void OnItemSpawnFieldSkillWaitSig(long itemID, bool waitEvent)
			{
				foreach (InventoryItem slotItem in slotItems)
				{
					if (slotItem != null && slotItem.ItemID == itemID)
					{
						slotItem.UpdateWaitEvent(waitEvent);
						break;
					}
				}
				if (main != null)
				{
					main.UpdateInventoryUI(owner);
				}
			}

			public void ResolveInventoryItemInfo(ItemInfo itemInfo)
			{
				for (int i = 0; i < slotItems.Count; i++)
				{
					InventoryItem inventoryItem = slotItems[i];
					if (inventoryItem != null && inventoryItem.ItemID == itemInfo.itemID && inventoryItem.ItemMasterID == itemInfo.itemMasterID)
					{
						inventoryItem.UpdateInfo(itemInfo);
						break;
					}
				}
			}

			public void ResolveInventoryItemInfo(ItemInfo itemInfo, int slotIndex)
			{
				if (slotIndex < 0 || slotIndex >= slotItems.Count)
				{
					Logger.RError($"ResolveInventoryItemInfo: Invalid slot index {slotIndex}");
					return;
				}
				InventoryItem inventoryItem = slotItems[slotIndex];
				if (inventoryItem != null && inventoryItem.ItemID == itemInfo.itemID && inventoryItem.ItemMasterID == itemInfo.itemMasterID)
				{
					inventoryItem.UpdateInfo(itemInfo);
					return;
				}
				InventoryItem inventoryItem2 = InventoryItem.Create(itemInfo);
				slotItems[slotIndex] = inventoryItem2;
				if ((inventoryItem != null && inventoryItem.IsAccessory) || (inventoryItem2 != null && inventoryItem2.IsAccessory))
				{
					owner.EquipAccessories(slotItems);
				}
			}

			public void ResolveInventoryInfos(Dictionary<int, ItemInfo> serverItemInfos, int? currentServerSlotIndex)
			{
				slotItems.Clone();
				if (currentServerSlotIndex.HasValue)
				{
					selectedSlotIndex = currentServerSlotIndex.Value - 1;
				}
				bool flag = false;
				for (int i = 0; i < slotSize; i++)
				{
					int key = i + 1;
					if (serverItemInfos.ContainsKey(key))
					{
						InventoryItem inventoryItem = slotItems[i];
						ItemInfo itemInfo = serverItemInfos[key];
						if (inventoryItem != null && inventoryItem.ItemID == itemInfo.itemID && inventoryItem.ItemMasterID == itemInfo.itemMasterID)
						{
							inventoryItem.UpdateInfo(itemInfo);
							continue;
						}
						InventoryItem inventoryItem2 = InventoryItem.Create(itemInfo);
						slotItems[i] = inventoryItem2;
						if (!flag && ((inventoryItem != null && inventoryItem.IsAccessory) || (inventoryItem2 != null && inventoryItem2.IsAccessory)))
						{
							flag = true;
						}
					}
					else
					{
						slotItems[i] = null;
					}
				}
				if (flag)
				{
					owner.EquipAccessories(slotItems);
				}
				owner.EquipHandheld(SelectedItem);
				UpdateSelectedItemComponents();
				owner.UpdateMountedLight();
				if (main != null)
				{
					main.UpdateInventoryUI(owner);
				}
			}

			public bool IsCurrentChargeableItem()
			{
				if (SelectedItem != null && SelectedItem.TryCastMasterInfo<ItemEquipmentInfo>(out ItemEquipmentInfo masterInfo))
				{
					return masterInfo.UseCharge;
				}
				return false;
			}

			public bool IsCurrentNeedToCharge()
			{
				if (SelectedItem != null && SelectedItem.TryCastMasterInfo<ItemEquipmentInfo>(out ItemEquipmentInfo masterInfo))
				{
					return SelectedItem.RemainGauge < masterInfo.InitialGauge;
				}
				return false;
			}

			public InventoryItem? FindItemByID(long itemID)
			{
				return slotItems.FirstOrDefault((InventoryItem item) => item != null && item.ItemID == itemID);
			}

			public bool IsTurnedOnMountedLight()
			{
				List<int> flashLightItemMasterIDs = new List<int> { 2000, 2500 };
				if (SelectedItem != null && flashLightItemMasterIDs.Contains(SelectedItem.ItemMasterID) && SelectedItem.IsTurnOn)
				{
					return false;
				}
				return slotItems.Any((InventoryItem item) => item != null && !item.Equals(SelectedItem) && flashLightItemMasterIDs.Contains(item.ItemMasterID) && item.IsTurnOn);
			}

			public void UpdateSelectedItemComponents()
			{
				if (SelectedItem != null && SelectedItem.TryGetComponent<SocketAttachable>(out SocketAttachable component))
				{
					InventoryItem.ItemChangedValueFlags changedValueFlags = SelectedItem.ChangedValueFlags;
					if (changedValueFlags.HasFlag(InventoryItem.ItemChangedValueFlags.IsTurnOn) && component is IToggleableItem toggleableItem)
					{
						toggleableItem.OnToggled(SelectedItem.ItemID, SelectedItem.IsTurnOn);
					}
					if (changedValueFlags.HasFlag(InventoryItem.ItemChangedValueFlags.RemainGauge) && component is IGaugeableItem gaugeableItem)
					{
						gaugeableItem.OnGaugeChanged(SelectedItem.ItemID, SelectedItem.RemainGauge);
					}
					changedValueFlags.HasFlag(InventoryItem.ItemChangedValueFlags.StackCount);
					changedValueFlags.HasFlag(InventoryItem.ItemChangedValueFlags.Durability);
					SelectedItem.ResetChangedValueFlags();
				}
			}
		}

		public class SkillHelper
		{
			public enum eSkillOwner
			{
				None = 0,
				Me = 1,
				OtherPlayer = 2,
				Monster = 3
			}

			private ProtoActor owner;

			private List<int> coolWaitingSkillMasterIds = new List<int>();

			public eSkillOwner currentCastingSkillOwner;

			public UseSkillSig currentCastingUseSkillSigCached;

			public long currentCastingSkillSyncIDCached;

			public int currentCastingSkillMasterIdCached;

			public int currentCastingSkillSequenceMasterIdCached;

			public int currentCastingSkillTargetActorIdCached;

			private GameMainBase? main
			{
				get
				{
					if (!(owner != null))
					{
						return null;
					}
					return owner.main;
				}
			}

			private CancellationToken destroyCancellationToken
			{
				get
				{
					if (!(owner != null))
					{
						return CancellationToken.None;
					}
					return owner.destroyCancellationToken;
				}
			}

			public SkillHelper(ProtoActor owner)
			{
				this.owner = owner;
			}

			public bool HasAnyCooltime()
			{
				return coolWaitingSkillMasterIds.Count > 0;
			}

			public void UseSkill(int skillMasterID, ProtoActor? target)
			{
				if (!coolWaitingSkillMasterIds.Contains(skillMasterID))
				{
					if (!owner.AmIAvatar())
					{
						Logger.RError("only avatar can use skill");
					}
					else if (!Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterID).CastableWhenMoving)
					{
						owner.SetInputDisableReason(EInputDisableReason.SkillCast);
						owner.TrySendMoveStop();
						SendUseSkill(skillMasterID, target);
					}
					else
					{
						SendUseSkill(skillMasterID, target);
					}
				}
			}

			private void SendUseSkill(int skillMasterID, ProtoActor? target)
			{
				main?.SendPacketWithCallback<UseSkillRes>(new UseSkillReq
				{
					skillMasterID = skillMasterID,
					targetID = ((!(target == null)) ? target.ActorID : 0),
					actorID = owner.ActorID,
					startBasePosition = owner.MyPosWithAngle(),
					endBasePosition = owner.MyPosWithAngle()
				}, UseSkillResCallback, destroyCancellationToken);
			}

			private void UseSkillResCallback(UseSkillRes res)
			{
				if (res == null || owner == null || owner.main == null)
				{
					return;
				}
				if (!owner.AmIAvatar())
				{
					Logger.RError("only avatar can process UseSkillRes");
					return;
				}
				if (res.errorCode != MsgErrorCode.CantAction && res.errorCode != MsgErrorCode.SkillCooltime)
				{
					if (res.errorCode == MsgErrorCode.ItemNotFound || res.errorCode == MsgErrorCode.CanUseSkillByGauge)
					{
						if (Hub.s != null && Hub.s.tableman != null)
						{
							Hub.s.tableman.uiprefabs.ShowTimerDialog("ToastSimple", 0f, Hub.GetL10NText("STRING_NO_BULLET"));
						}
					}
					else if (res.errorCode != MsgErrorCode.Success)
					{
						Logger.RError($"UseSkillRes failed : {res.errorCode}");
					}
				}
				SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(res.skillMasterID);
				if (skillInfo != null)
				{
					owner.StartCoroutine(ReserveEndSkillCoolTime(res.skillMasterID, (float)skillInfo.Cooltime * 0.001f));
				}
			}

			private IEnumerator ReserveEndSkillCoolTime(int skillMasterID, float durationSec)
			{
				yield return new WaitForSeconds(durationSec);
				OnEndSkillCoolTime(skillMasterID);
			}

			internal void OnNetUseSkillSig(UseSkillSig sig)
			{
				currentCastingUseSkillSigCached = sig;
				SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(sig.skillMasterID);
				if (skillInfo == null)
				{
					Logger.RError($"Can't find skilldata = {sig.skillMasterID}");
					return;
				}
				if (owner.ActorType == ActorType.Monster)
				{
					currentCastingSkillOwner = eSkillOwner.Monster;
					if (skillInfo.HideHandItemWhenSkillCast && owner.handheldItem != null)
					{
						owner.handheldItem.Transform.gameObject.SetActive(value: false);
						owner.ReserveShowHandItem(4.266667f);
					}
				}
				else if (owner.ActorType == ActorType.Player)
				{
					if (owner.controlMode == ControlMode.Manual)
					{
						currentCastingSkillOwner = eSkillOwner.Me;
					}
					else
					{
						currentCastingSkillOwner = eSkillOwner.OtherPlayer;
					}
				}
				currentCastingSkillSyncIDCached = sig.skillSyncID;
				currentCastingSkillMasterIdCached = sig.skillMasterID;
				currentCastingSkillTargetActorIdCached = sig.targetID;
			}

			internal void OnNetCancelSkillSig(CancelSkillSig sig)
			{
				SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(currentCastingSkillMasterIdCached);
				if (skillInfo != null)
				{
					owner.CancelSkillAnim(skillInfo.SkillCancelAnimationState);
				}
				currentCastingSkillSyncIDCached = 0L;
				currentCastingSkillMasterIdCached = 0;
				currentCastingSkillTargetActorIdCached = 0;
				currentCastingSkillOwner = eSkillOwner.None;
			}

			internal void OnNetSyncSkillMoveSig(SyncSkillMoveSig packet)
			{
			}

			internal void OnStartSkillCoolTime(int skillMasterId)
			{
				if (coolWaitingSkillMasterIds.Contains(skillMasterId))
				{
					Logger.RError($"Can't cast skill[{skillMasterId}] because of cooltime");
				}
				coolWaitingSkillMasterIds.AddUnique(skillMasterId);
			}

			internal void OnEndSkillCoolTime(int skillMasterId)
			{
				if (coolWaitingSkillMasterIds.Contains(skillMasterId))
				{
					coolWaitingSkillMasterIds.Remove(skillMasterId);
				}
			}
		}

		public class VoiceEffecter
		{
			public enum VoiceEffectType
			{
				None = 0,
				Vibration = 1,
				Amplification = 2,
				Range = 3,
				Transmitter = 4,
				Silence = 5
			}

			private enum VoiceType
			{
				None = 0,
				DissonanceLocal = 1,
				DissonanceRemote = 2,
				VoiceAudioSource = 3
			}

			private class VoiceEffectData
			{
				public bool IsEnabled;

				public VoiceEffectPreset Preset;

				public VoiceEffectData(VoiceEffectPreset preset)
				{
					IsEnabled = false;
					Preset = preset;
					if (preset == null)
					{
						Logger.RError("[VoiceEffecter] VoiceEffectData() - preset is null");
					}
				}
			}

			private AudioSourceFilterController? _audioSourceFilterController;

			private readonly ProtoActor _owner;

			private VoiceType _voiceType;

			private bool _isInitialized;

			private readonly Dictionary<VoiceEffectType, VoiceEffectData> _voiceEffectData = new Dictionary<VoiceEffectType, VoiceEffectData>();

			private readonly Dictionary<VoiceEffectType, bool> _voiceEffectInitializeData = new Dictionary<VoiceEffectType, bool>();

			private float _voiceVibrationStartTime;

			private bool _isVoiceVibrationUpdateRunning;

			private Coroutine? _voiceInitializeCoroutine;

			public VoiceEffecter(ProtoActor owner)
			{
				_owner = owner;
				MMVoiceEffectTable mMVoiceEffectTable = Hub.s.tableman?.voiceEffect;
				if (mMVoiceEffectTable != null)
				{
					_voiceEffectData.Add(VoiceEffectType.Vibration, new VoiceEffectData(mMVoiceEffectTable.GetPresetByName("VoiceVibration")));
					_voiceEffectData.Add(VoiceEffectType.Amplification, new VoiceEffectData(mMVoiceEffectTable.GetPresetByName("VoiceAmplification")));
					_voiceEffectData.Add(VoiceEffectType.Range, new VoiceEffectData(mMVoiceEffectTable.GetPresetByName("VoiceRange")));
					_voiceEffectData.Add(VoiceEffectType.Transmitter, new VoiceEffectData(mMVoiceEffectTable.GetPresetByName("VoiceTransmitter")));
					_voiceEffectData.Add(VoiceEffectType.Silence, new VoiceEffectData(mMVoiceEffectTable.GetPresetByName("VoiceSilence")));
				}
				_voiceEffectInitializeData.Clear();
				_voiceEffectInitializeData[VoiceEffectType.Amplification] = false;
				_voiceEffectInitializeData[VoiceEffectType.Range] = false;
				_voiceEffectInitializeData[VoiceEffectType.Vibration] = false;
				_voiceEffectInitializeData[VoiceEffectType.Silence] = false;
			}

			~VoiceEffecter()
			{
			}

			public void Initialize(AudioSource? inVoiceAudioSource)
			{
				if (_isInitialized)
				{
					Logger.RError($"Voice pitch shifter is already initialized: {_owner.ActorID}");
					return;
				}
				bool num = _owner.ActorType == ActorType.Player;
				bool flag = _owner.controlMode == ControlMode.Remote;
				bool flag2 = _owner.IsMimic();
				bool flag3 = IsReplayPlayMode();
				AudioSource audioSource = null;
				if (num)
				{
					if (flag3)
					{
						_voiceType = VoiceType.VoiceAudioSource;
						audioSource = inVoiceAudioSource;
					}
					else if (flag)
					{
						_voiceType = VoiceType.DissonanceRemote;
						audioSource = Hub.s.voiceman?.GetVoicePlaybackAudioSource(_owner.UID);
					}
					else
					{
						_voiceType = VoiceType.DissonanceLocal;
						audioSource = Hub.s.voiceman?.GetLocalVoiceAudioSources();
					}
				}
				else if (flag2)
				{
					_voiceType = VoiceType.VoiceAudioSource;
					audioSource = inVoiceAudioSource;
				}
				if (audioSource != null)
				{
					SetAudioFilterReference(audioSource);
					_isInitialized = true;
					InitializeVoiceStatData();
					Logger.RLog($"<color=green>VoiceEffecter initialized: ActorId({_owner.ActorID})</color>", sendToLogServer: false, useConsoleOut: true, "voiceeffect");
				}
			}

			public void Uninitialize()
			{
				if (_isInitialized)
				{
					if (_voiceInitializeCoroutine != null)
					{
						_owner.StopCoroutine(_voiceInitializeCoroutine);
						_voiceInitializeCoroutine = null;
					}
					_isVoiceVibrationUpdateRunning = false;
					if (_audioSourceFilterController != null)
					{
						_audioSourceFilterController.Reset(inResetPitch: true);
					}
					_audioSourceFilterController = null;
					_voiceType = VoiceType.None;
					_isInitialized = false;
				}
			}

			public void Update(float deltaTime)
			{
				if (_isInitialized && NeedUpdateVoiceVibration())
				{
					UpdateVoiceVibration();
				}
			}

			public bool IsVoicePlaying()
			{
				if (_audioSourceFilterController == null)
				{
					return false;
				}
				return _audioSourceFilterController.IsPlaying();
			}

			public void StopVoice()
			{
				if (!(_audioSourceFilterController == null))
				{
					_audioSourceFilterController.Stop();
				}
			}

			public void SetVoiceEffect(VoiceEffectType inType, bool isEnabled)
			{
				VoiceEffectData value;
				if (!_isInitialized)
				{
					_voiceEffectInitializeData[inType] = isEnabled;
				}
				else if (!_voiceEffectData.TryGetValue(inType, out value) || value == null)
				{
					Logger.RError($"[VoiceEffecter] SetVoiceEffect() - VoiceEffectType({inType}) is not found, ActorId({_owner.ActorID})");
				}
				else if (isEnabled)
				{
					ApplyEffectPreset(inType, value);
					value.IsEnabled = isEnabled;
				}
				else
				{
					value.IsEnabled = isEnabled;
					RemoveEffectPreset(inType);
				}
			}

			public void Set3DEffect(bool isEnabled)
			{
				if (_audioSourceFilterController != null)
				{
					_audioSourceFilterController.Set3DEffect(isEnabled);
				}
			}

			public void PlayAudioClip(AudioClip clip, bool isLoop = false)
			{
				if (_audioSourceFilterController != null)
				{
					_audioSourceFilterController.PlayAudioClip(clip, isLoop);
				}
			}

			public void SetMimicVoiceEcho(bool isEnabled)
			{
				if (!(_audioSourceFilterController == null))
				{
					_audioSourceFilterController.SetMimicVoiceEcho(isEnabled);
				}
			}

			private bool NeedUpdateVoiceVibration()
			{
				return _isVoiceVibrationUpdateRunning;
			}

			private void UpdateVoiceVibration()
			{
				if (!_isInitialized || !_isVoiceVibrationUpdateRunning)
				{
					return;
				}
				if (_audioSourceFilterController == null)
				{
					_isVoiceVibrationUpdateRunning = false;
					return;
				}
				float num = Time.time - _voiceVibrationStartTime;
				float num2 = CalculateCombinedModulation(num);
				if (_audioSourceFilterController != null)
				{
					_audioSourceFilterController.LowPass.cutoffFrequency = Mathf.Clamp(1500f + num2 * 60f, 800f, 3000f);
					_audioSourceFilterController.HighPass.cutoffFrequency = Mathf.Clamp(400f + num2 * 30f, 200f, 800f);
					_audioSourceFilterController.Chorus.rate = 0.8f + Mathf.Sin(num * 10f) * 0.7f;
					_audioSourceFilterController.Chorus.depth = 0.1f + Mathf.Abs(Mathf.Sin(num * 20f)) * 0.7f;
					_audioSourceFilterController.Reverb.decayTime = 0.5f + Mathf.Abs(Mathf.Sin(num * 4f)) * 5f;
					_audioSourceFilterController.Reverb.dryLevel = -400f + Mathf.Abs(num2) * 0.05f;
				}
			}

			public void SetPitchStat(long statValue)
			{
				if (!_isInitialized)
				{
					return;
				}
				if (_audioSourceFilterController == null)
				{
					Logger.RError($"<color=yellow>[VoiceEffecter] SetPitchStat() - _audioSource is null, ActorId({_owner.ActorID})</color>");
					return;
				}
				(AudioMixerGroup, float)? tuple = Hub.s.tableman?.voicePitchShift?.GetMixerParameter(statValue);
				if (tuple.HasValue && !(tuple.Value.Item1 == null))
				{
					_audioSourceFilterController.SetOutputMixer(tuple.Value.Item1);
				}
			}

			private void SetAudioFilterReference(AudioSource inAudioSource)
			{
				_audioSourceFilterController = inAudioSource.GetComponent<AudioSourceFilterController>();
				if (_audioSourceFilterController == null)
				{
					Logger.RError($"[VoiceEffecter] SetAudioFilterReference() - AudioSourceFilterController is null, ActorId({_owner.ActorID})");
				}
				else
				{
					_audioSourceFilterController.Reset(inResetPitch: true);
				}
			}

			private float CalculateCombinedModulation(float inTimeSinceStart)
			{
				if (!_voiceEffectData.TryGetValue(VoiceEffectType.Vibration, out VoiceEffectData value) || value == null || value.Preset == null)
				{
					return 1f;
				}
				float modulationSpeed = value.Preset.modulationSpeed;
				float modulationIntensity = value.Preset.modulationIntensity;
				float wobbleSpeed = value.Preset.wobbleSpeed;
				float superWobbleSpeed = value.Preset.superWobbleSpeed;
				float num = Mathf.Sin(inTimeSinceStart * modulationSpeed) * modulationIntensity;
				float num2 = Mathf.Sin(inTimeSinceStart * wobbleSpeed) * (modulationIntensity * 0.3f);
				float num3 = Mathf.Sin(inTimeSinceStart * superWobbleSpeed) * (modulationIntensity * 0.15f);
				return num + num2 + num3;
			}

			private static bool IsReplayPlayMode()
			{
				return Hub.s?.replayManager?.IsReplayPlayMode == true;
			}

			private void ApplyEffectPreset(VoiceEffectType inType, VoiceEffectData inEffectData)
			{
				if (_voiceEffectData[VoiceEffectType.Silence].IsEnabled)
				{
					inEffectData.IsEnabled = true;
					return;
				}
				switch (inType)
				{
				case VoiceEffectType.Silence:
					if (_owner.AmIAvatar())
					{
						Hub.s.voiceman.EnableSelfVoice(inSpeak: false, inHear: true, inTransmitter: false);
					}
					SetAudioFilter(inType, inEffectData.Preset);
					break;
				case VoiceEffectType.Transmitter:
					if (_owner.AmIAvatar())
					{
						Hub.s.voiceman.EnableSelfVoice(inSpeak: true, inHear: true, inTransmitter: true);
					}
					SetAudioFilter(inType, inEffectData.Preset);
					break;
				case VoiceEffectType.Amplification:
					if (!_voiceEffectData[VoiceEffectType.Transmitter].IsEnabled)
					{
						if (_owner.AmIAvatar())
						{
							Hub.s.voiceman.EnableSelfVoice(inSpeak: true, inHear: true, inTransmitter: false);
						}
						SetAudioFilter(inType, inEffectData.Preset);
					}
					break;
				case VoiceEffectType.Vibration:
					if (!_voiceEffectData[VoiceEffectType.Transmitter].IsEnabled)
					{
						if (_owner.AmIAvatar())
						{
							Hub.s.voiceman.EnableSelfVoice(inSpeak: true, inHear: true, inTransmitter: false);
						}
						_isVoiceVibrationUpdateRunning = true;
						_voiceVibrationStartTime = Time.time;
						SetAudioFilter(inType, inEffectData.Preset);
					}
					break;
				case VoiceEffectType.Range:
					if (_audioSourceFilterController != null)
					{
						_audioSourceFilterController.SetDistance(inEffectData.Preset.distance);
					}
					break;
				}
			}

			public void RemoveEffectPreset(VoiceEffectType inType)
			{
				bool inSpeak = true;
				bool inHear = false;
				bool inTransmitter = false;
				switch (inType)
				{
				case VoiceEffectType.Vibration:
					_isVoiceVibrationUpdateRunning = false;
					break;
				case VoiceEffectType.Range:
					if (_audioSourceFilterController != null)
					{
						_audioSourceFilterController.SetDistance(20f);
					}
					return;
				}
				VoiceEffectData value2;
				VoiceEffectData value3;
				if (_voiceEffectData.TryGetValue(VoiceEffectType.Transmitter, out VoiceEffectData value) && value.IsEnabled)
				{
					inHear = true;
					inTransmitter = true;
					SetAudioFilter(VoiceEffectType.Transmitter, value.Preset);
				}
				else if (_voiceEffectData.TryGetValue(VoiceEffectType.Amplification, out value2) && value2.IsEnabled)
				{
					inHear = true;
					SetAudioFilter(VoiceEffectType.Amplification, value2.Preset);
				}
				else if (_voiceEffectData.TryGetValue(VoiceEffectType.Vibration, out value3) && value3.IsEnabled)
				{
					inHear = true;
					SetAudioFilter(VoiceEffectType.Vibration, value3.Preset);
				}
				else
				{
					ResetAudioFilter();
				}
				if (_voiceEffectData.TryGetValue(VoiceEffectType.Silence, out VoiceEffectData value4) && value4.IsEnabled)
				{
					inSpeak = false;
				}
				if (_owner.AmIAvatar())
				{
					Hub.s.voiceman.EnableSelfVoice(inSpeak, inHear, inTransmitter);
				}
			}

			private void SetAudioFilter(VoiceEffectType inType, VoiceEffectPreset? inPreset)
			{
				if (!(inPreset == null) && _isInitialized)
				{
					if (_audioSourceFilterController == null)
					{
						Logger.RError($"[VoiceEffecter] ApplyEffectPreset() - _audioSourceFilterController is null, ActorId({_owner.ActorID})");
					}
					else
					{
						_audioSourceFilterController.ApplyFilters(inPreset);
					}
				}
			}

			private void InitializeVoiceStatData()
			{
				SetPitchStat(_owner.netSyncActorData.voicePitch);
				SetVoiceEffect(VoiceEffectType.Amplification, _voiceEffectInitializeData[VoiceEffectType.Amplification]);
				SetVoiceEffect(VoiceEffectType.Range, _voiceEffectInitializeData[VoiceEffectType.Range]);
				SetVoiceEffect(VoiceEffectType.Vibration, _voiceEffectInitializeData[VoiceEffectType.Vibration]);
				SetVoiceEffect(VoiceEffectType.Silence, _voiceEffectInitializeData[VoiceEffectType.Silence]);
			}

			private void StartRemoteVoiceFadeIn(AudioSource? inAudioSource)
			{
				if (_voiceInitializeCoroutine != null)
				{
					_owner.StopCoroutine(_voiceInitializeCoroutine);
					_voiceInitializeCoroutine = null;
				}
				if (!(inAudioSource == null))
				{
					_voiceInitializeCoroutine = _owner.StartCoroutine(_owner.RemoteVoiceFadeInCoroutine(inAudioSource));
				}
			}

			public void OnEndVolumeFadeIn()
			{
				_voiceInitializeCoroutine = null;
			}

			public void ResetAudioFilter()
			{
				if (_isInitialized && !(_audioSourceFilterController == null))
				{
					_audioSourceFilterController.Reset(inResetPitch: false);
				}
			}
		}

		[Tooltip("1인칭 시점의 기준이 될 Transform")]
		[SerializeField]
		private Transform camRoot;

		[Tooltip("점프 연출용 카메라. (액터에서 Tracking Target에 camRoot를 넣어주는 처리를 해줌)")]
		[SerializeField]
		private string jumpCameraSocketName = "socket_jump_camera";

		[Tooltip("사운드 생성의 기준이 될 Transform")]
		[SerializeField]
		private Transform sfxRoot;

		[Tooltip("미믹 음성 또는 플레이어의 음성(리플레이 한정)을 재생하기 위한 프리팹")]
		[SerializeField]
		private AudioSource voicePrefab;

		[Tooltip("손전등을 켰지만 들고 있지 않을 때의 대체 광원")]
		[SerializeField]
		private Light mountedLight;

		[Tooltip("닉네임 텍스트")]
		[SerializeField]
		private TMP_Text nameTag;

		[Tooltip("디버깅용 텍스트")]
		[SerializeField]
		private TMP_Text debugText;

		[Tooltip("디버깅용 텍스트(오버래이)")]
		[SerializeField]
		private TMP_Text debugOverlayText;

		[Tooltip("피격 판정을 위한 Hurtbox")]
		[SerializeField]
		private Hurtbox hurtbox;

		[SerializeField]
		private GameObject debugUI;

		[SerializeField]
		private GameObject debugSkillUiPrefab;

		[SerializeField]
		private float checkGroundedDistance = 0.5f;

		[SerializeField]
		private string audioKeyWhenBlackOut = "BlackOut";

		[SerializeField]
		private string audioKeyWhenRecoverFromBlackOut = "BlackOutRecover";

		[SerializeField]
		private string audioKeyWhenBlackOutByOwnedItem = "BlackOutScream";

		[SerializeField]
		private string audioKeyWhenTeleported = "teleportEnd";

		private InventoryItem? handheldItem;

		private List<InventoryItem> accessoryItems = new List<InventoryItem>();

		private AudioSource? _voiceAudioSource;

		private PlaySoundResult? currentHummingSoundResult;

		private CancellationToken destroyToken;

		public Vector3 LastDamagedForceDirection = Vector3.zero;

		private ProtoActorInterpolationType interpolationMethod = ProtoActorInterpolationType.DebugByPass;

		[InspectorReadOnly]
		public ControlMode controlMode;

		private ReasonOfDeath reasonOfDeath;

		public DLMovementAgent dlMovementAgent;

		public DLDecisionAgent dlDecisionAgent;

		[HideInInspector]
		public ulong steamID;

		private Vector2 movementInput = Vector2.zero;

		private PuppetScript puppet;

		private float falling;

		private Vector3? debug_syncTargetPos;

		private Vector3? syncTargetPosVForAnimation;

		private float? syncTargetRotY;

		private float? syncTargetCamRotX;

		private List<MoveInterpolationData> moveInterpolationData = new List<MoveInterpolationData>();

		private List<(int transId, Vector3 pos, float rotY, float camRotX)> moveStartFallbackList = new List<(int, Vector3, float, float)>();

		private Vector3? oldPos;

		private float oldRotY;

		private float oldCamRotX;

		private ActorMotionType stance;

		private bool isGrabbedByOther;

		private bool pauseTransformSync;

		public NetSyncActorData netSyncActorData = new NetSyncActorData();

		private CharacterController? _cc;

		private bool lastGrounded;

		private SkillHelper skillHelper;

		private AbnormalHelper abnormalHelper;

		private AuraHelper auraHelper;

		private Inventory inventory;

		private EmotePlayer emotePlayer;

		private GrabHelper grabHelper;

		private VoiceEffecter voiceEffecter;

		private FakeJumper fakeJumper;

		private RealWorldRaySensor? _raySensor;

		private DebugBTStateInfo debugBTStateInfo = new DebugBTStateInfo
		{
			btDataName = string.Empty,
			btTemplateName = string.Empty
		};

		private Vector3 lastCharacterControlerHorizontalVelocity = Vector3.zero;

		private bool _debug_keepWalking;

		private List<Coroutine> TurnOffPaintRunners = new List<Coroutine>();

		private SkinnedMeshRenderer[] rdrs;

		[SerializeField]
		[InspectorReadOnly]
		[Tooltip("현재 타일 정보")]
		private Tile currentTile;

		private bool needsToSendMoveStop;

		private int sendTransformTransID;

		private EffectPlayer effectPlayer;

		private float _lastUpdate;

		private float _updateInterval = 0.5f;

		private float _incomingEventExpireTime = 3f;

		private float _incomingEventExpireTimeForMimic = 1f;

		private List<IncomingEvent> _incomingEvent = new List<IncomingEvent>();

		private HashSet<ProtoActor> _adjacentPlayers = new HashSet<ProtoActor>();

		private HashSet<ProtoActor> _visibleAdjacentPlayers = new HashSet<ProtoActor>();

		private HashSet<ProtoActor> _monsters = new HashSet<ProtoActor>();

		private HashSet<ProtoActor> _visibleMonsters = new HashSet<ProtoActor>();

		private HashSet<int> _adjacentPlayersHandHeldItems = new HashSet<int>();

		private HashSet<LootingLevelObject> _scrapObjects = new HashSet<LootingLevelObject>();

		private HashSet<CommonChargerLevelObject> _chargers = new HashSet<CommonChargerLevelObject>();

		private HashSet<TeleporterLevelObject> _teleporters = new HashSet<TeleporterLevelObject>();

		private HashSet<MomentarySwitchLevelObject> _corridorSwitches = new HashSet<MomentarySwitchLevelObject>();

		private HashSet<CrowShopLevelObject> _crowShops = new HashSet<CrowShopLevelObject>();

		private HashSet<TrapLevelObject> _invisibleMines = new HashSet<TrapLevelObject>();

		private HashSet<ScrapScanLevelObject> _scrapScanners = new HashSet<ScrapScanLevelObject>();

		private HashSet<FieldSkillObjectInfo> _sprinklers = new HashSet<FieldSkillObjectInfo>();

		private HashSet<FieldSkillObjectInfo> _paintballs = new HashSet<FieldSkillObjectInfo>();

		private HashSet<FieldSkillObjectInfo> _paintspots = new HashSet<FieldSkillObjectInfo>();

		private HashSet<FieldSkillObjectInfo> _heliumGasPumps = new HashSet<FieldSkillObjectInfo>();

		private HashSet<FieldSkillObjectInfo> _lightnings = new HashSet<FieldSkillObjectInfo>();

		private List<(int actorID, int grabMasterID)> _grabSkills = new List<(int, int)>();

		private readonly List<ProtoActor> _tempActorList = new List<ProtoActor>(32);

		private readonly List<(int, int)> _tempCurrentGrabSkills = new List<(int, int)>();

		private readonly Dictionary<SpeechEvent_IncomingType, IList> _tempLevelObjectsByType = new Dictionary<SpeechEvent_IncomingType, IList>();

		private bool _indoorEntered;

		private AlarmBounds? _alarmBounds;

		private readonly Dictionary<ProtoActor, float> _lastSeenPlayerTime = new Dictionary<ProtoActor, float>();

		[SerializeField]
		private float _incomingPlayerReacquireSeconds = 5f;

		private BTVoiceRule _debugVoiceRule;

		private int _debugMovementModelType;

		private string _voicePickReason = string.Empty;

		private Vector3? debug_oldMoveCurr;

		private bool debug_movePacketFlipFlop;

		public int InputDisableReason;

		public int InputMoveDisableReason;

		private float netintp_accelerationStartValue = 1.05f;

		private const float netintp_accelerationEndValue = 2f;

		private const int netintp_dropDataCount = 10;

		private const int netintp_accelerateDataCount = 1;

		public ActorType ActorType { get; private set; }

		public int ActorID { get; private set; }

		public string nickName { get; private set; } = "unknown";

		public Transform FpvCameraRoot => camRoot;

		public Transform SfxRoot => sfxRoot;

		public Hurtbox Hurtbox => hurtbox;

		private float rotXMin => paConfig.rotXMin;

		private float rotXMax => paConfig.rotXMax;

		private float walkSpeed => (float)netSyncActorData.MoveSpeedWalk / 100f;

		private float runSpeed => (float)netSyncActorData.MoveSpeedRun / 100f;

		private float turnSpeed => paConfig.mouseSensitivity;

		private float rotXInterpolationSpeed => paConfig.rotXInterpolationSpeed;

		private float rotYInterpolationSpeed => paConfig.rotYInterpolationSpeed;

		public bool isLightOn => IsFlashOrMountedLightOn();

		public bool dead { get; private set; }

		public float deadTime { get; private set; }

		public bool RemainAfterDeath { get; private set; }

		public ReasonOfDeath ReasonOfDeath => reasonOfDeath;

		public string puppetName { get; private set; } = string.Empty;

		private Hub.PersistentData pdata => Hub.s.pdata;

		private GameConfig config => Hub.s.gameConfig;

		public GameConfig.PlayerActor paConfig => config.playerActor;

		private InputManager? inputman
		{
			get
			{
				if (Hub.s == null)
				{
					return null;
				}
				return Hub.s.inputman;
			}
		}

		private NetworkManagerV2 netman2 => Hub.s.netman2;

		public long UID { get; private set; } = -1L;

		public int playerMasterID { get; private set; } = -1;

		public int monsterMasterID { get; private set; } = -1;

		private GameMainBase? main
		{
			get
			{
				if (Hub.s == null)
				{
					return null;
				}
				if (Hub.s.pdata == null)
				{
					return null;
				}
				return Hub.s.pdata.main;
			}
		}

		private bool hasMovementInput => movementInput.sqrMagnitude > 0f;

		public Vector3 flying { get; private set; } = Vector3.zero;

		private CharacterController cc
		{
			get
			{
				if (_cc == null)
				{
					_cc = GetComponent<CharacterController>();
				}
				return _cc;
			}
		}

		public bool grounded { get; private set; }

		public bool isSprinting { get; private set; }

		public bool dontMoveFlag { get; private set; }

		public bool isGrabbing { get; private set; }

		public bool IsAnimByAbnormal { get; private set; }

		public RealWorldRaySensor raySensor
		{
			get
			{
				if (_raySensor == null)
				{
					_raySensor = GetComponent<RealWorldRaySensor>();
				}
				return _raySensor;
			}
		}

		public bool debug_keepWalking
		{
			get
			{
				return _debug_keepWalking;
			}
			set
			{
				_debug_keepWalking = value;
			}
		}

		public bool isMoved { get; private set; }

		public bool isRotated { get; private set; }

		public Action<OtherCreatureInfo> Attached_ResolveOtherInfo => abnormalHelper.AttachingResolveOtherInfo;

		public Action<OtherCreatureInfo> Aura_ResolveOtherInfo => auraHelper.ResolveOtherAuraInfo;

		private void Awake()
		{
			skillHelper = new SkillHelper(this);
			inventory = new Inventory(this);
			abnormalHelper = new AbnormalHelper(this);
			auraHelper = new AuraHelper(this);
			effectPlayer = new EffectPlayer(this);
			emotePlayer = new EmotePlayer(this);
			grabHelper = new GrabHelper(this);
			voiceEffecter = new VoiceEffecter(this);
			fakeJumper = new FakeJumper(this);
			destroyToken = base.destroyCancellationToken;
		}

		public void SetAsMyAvatar(PlayerInfo info, bool firstEnterMap)
		{
			ActorType = ActorType.Player;
			ActorID = info.actorID;
			UID = info.UID;
			SetActorName("Avatar", info.actorName ?? "");
			SetControlMode(ControlMode.Manual);
			ChangePlayerPuppetByMasterID(info.masterID);
			_voiceAudioSource = ((voicePrefab != null) ? UnityEngine.Object.Instantiate(voicePrefab, sfxRoot) : null);
			voiceEffecter.Initialize(_voiceAudioSource);
			ResolveStatCollection(info.statInfoCollection);
			cc.enableOverlapRecovery = false;
			main.OnPlayerSpawn(this);
			Hub.s.cameraman.SetupPlayerCamera(this, jumpCameraSocketName);
			inventory.ResolveInventoryInfos(info.inventories, info.currentInventorySlot);
			rdrs = GetComponentsInChildren<SkinnedMeshRenderer>();
			if (firstEnterMap)
			{
				puppet.PlayFirstSpawnMotion();
			}
		}

		public void SetAsOtherPlayer(PlayerInfo info)
		{
			ActorType = ActorType.Player;
			ActorID = info.actorID;
			UID = info.UID;
			SetActorName("OtherPlayer", info.actorName ?? "");
			SetControlMode(ControlMode.Remote);
			ChangePlayerPuppetByMasterID(info.masterID);
			_voiceAudioSource = ((voicePrefab != null) ? UnityEngine.Object.Instantiate(voicePrefab, sfxRoot) : null);
			voiceEffecter.Initialize(_voiceAudioSource);
			ResolveStatCollection(info.statInfoCollection);
			interpolationMethod = ProtoActorInterpolationType.PathQueue;
			main.OnPlayerSpawn(this);
			inventory.ResolveInventoryInfos(info.inventories, info.currentInventorySlot);
			rdrs = GetComponentsInChildren<SkinnedMeshRenderer>();
			if (info.firstEnterMap)
			{
				puppet.PlayFirstSpawnMotion();
			}
			netintp_accelerationStartValue = 1.05f;
		}

		public void SetAsMonster(OtherCreatureInfo info)
		{
			ActorType = ActorType.Monster;
			ActorID = info.actorID;
			UID = -1L;
			SetActorName("Monster", $"{info.actorName} #{info.actorID}");
			SetControlMode(ControlMode.Remote);
			ChangeMonsterPuppetByMasterID(info.masterID);
			_voiceAudioSource = ((voicePrefab != null) ? UnityEngine.Object.Instantiate(voicePrefab, sfxRoot) : null);
			voiceEffecter.Initialize(_voiceAudioSource);
			ResolveStatCollection(info.statInfoCollection);
			interpolationMethod = ProtoActorInterpolationType.PathQueue;
			inventory.ResolveInventoryInfos(info.inventories, 1);
			rdrs = GetComponentsInChildren<SkinnedMeshRenderer>();
			netintp_accelerationStartValue = 1.02f;
		}

		public void InitializeVoiceEffecter()
		{
			voiceEffecter.Initialize(_voiceAudioSource);
		}

		public void UpdateStatus(PlayerStatusInfo info)
		{
			inventory.ResolveInventoryInfos(info.inventories, info.currentInventorySlot);
		}

		public void ActivateRagDoll(Vector3 externalForce)
		{
			if (puppet != null && puppet.TryGetComponent<easyRagDoll>(out var component) && component != null)
			{
				component.ActivateRagDoll(externalForce);
			}
		}

		public void PlayAnimByAbnormal(string stateName, bool loop = false)
		{
			if (puppet != null)
			{
				puppet.PlayHitAnim(stateName, loop);
			}
			IsAnimByAbnormal = true;
		}

		public void StopAnimByAbnormal()
		{
			if (puppet != null)
			{
				puppet.CancelHitAnim();
			}
			IsAnimByAbnormal = false;
		}

		public void PlayImmobilizeByAbnormal()
		{
			pauseTransformSync = true;
			IsAnimByAbnormal = true;
			SetInputDisableReason(EInputDisableReason.Abnormal);
		}

		public void StopImmobilizeByAbnormal()
		{
			pauseTransformSync = false;
			IsAnimByAbnormal = false;
			ClearInputDisableReason(EInputDisableReason.Abnormal);
		}

		private void OnDestroy()
		{
			if (!(this == null))
			{
				voiceEffecter?.Uninitialize();
				if (ActorType == ActorType.Player && main != null)
				{
					main.OnPlayerDespawn(this);
				}
			}
		}

		public void Teleport(Vector3 position, Vector3 rot, bool ignorePitch = false)
		{
			bool flag = cc.enabled;
			if (flag)
			{
				cc.enabled = false;
			}
			base.transform.position = position;
			base.transform.rotation = Quaternion.Euler(rot);
			if (!ignorePitch)
			{
				camRoot.localRotation = Quaternion.Euler(0f, 0f, 0f);
			}
			InvalidateMoveSyncTarget();
			if (flag)
			{
				cc.enabled = flag;
			}
			Hub.s.pdata.main.OnActorTeleported(this, position);
		}

		private IEnumerator CorCancelDontMove()
		{
			yield return new WaitForSeconds(1.7f);
			CancelDontMove();
		}

		public void OnTeleported(TeleportSig sig)
		{
			if (sig.reason == TeleportReason.RandomTeleport)
			{
				puppet.OnRandomTeleported();
				if (Hub.s != null && Hub.s.audioman != null)
				{
					Hub.s.audioman.PlaySfxTransform(audioKeyWhenTeleported, sfxRoot);
				}
				if (AmIAvatar())
				{
					DontMove();
					StartCoroutine(CorCancelDontMove());
				}
			}
		}

		public void InvalidateMoveSyncTarget()
		{
			debug_syncTargetPos = null;
			syncTargetPosVForAnimation = null;
			oldPos = null;
			moveInterpolationData.Clear();
			moveStartFallbackList.Clear();
		}

		public void UpdateHp(long hp, long maxHP)
		{
			if (!(Hub.s == null) && !(this == null) && !(main == null))
			{
				long hp2 = netSyncActorData.hp;
				netSyncActorData.hp = hp;
				main.OnHpChanged(this, hp, maxHP);
				if (hp2 > hp && AmIAvatar())
				{
					AddIncomingEvent(SpeechEvent_IncomingType.OnDamaged, Time.realtimeSinceStartup + GetIncomingEventExpireTime(SpeechEvent_IncomingType.OnDamaged));
				}
			}
		}

		public void UpdateConta(long conta, long maxConta)
		{
			if (!(Hub.s == null) && !(this == null) && !(main == null))
			{
				netSyncActorData.conta = conta;
				main.OnContaChanged(this, conta, maxConta);
			}
		}

		public void UpdateStamina(long newStamina, long maxStamina)
		{
			if (!(Hub.s == null) && !(this == null) && !(main == null))
			{
				netSyncActorData.stamina = newStamina;
				main.OnStaminaChanged(this, netSyncActorData.stamina, maxStamina);
			}
		}

		public void SetActorName(string actorUseAs, string name)
		{
			nickName = name;
			netSyncActorData.actorName = nickName;
			base.name = $"ProtoActor({actorUseAs}) #{ActorID}";
			nameTag.text = name;
		}

		private void SetControlMode(ControlMode controlMode)
		{
			switch (controlMode)
			{
			case ControlMode.Manual:
				cc.enabled = true;
				inputman.SetCapturing(on: true);
				interpolationMethod = ProtoActorInterpolationType.None;
				break;
			case ControlMode.Remote:
				cc.enabled = false;
				break;
			}
			this.controlMode = controlMode;
		}

		public void InstantiatePuppetForMyAvatar()
		{
			int num = 1;
			PlayerMasterInfo playerInfo = Hub.s.dataman.ExcelDataManager.GetPlayerInfo(num);
			if (playerInfo == null)
			{
				Logger.RError($"Cannot find any PlayerInfo: masterID={num}");
				return;
			}
			playerMasterID = num;
			puppetName = playerInfo.FppPuppetName;
			MMPuppetTable.Row row = Hub.s.tableman.monsterPuppet.FindRow(puppetName);
			if (row == null)
			{
				Logger.RError("Cannot find any MMPuppetData: puppetName=" + puppetName);
				return;
			}
			GameObject prefab = row.prefab;
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, base.transform);
			puppet = gameObject.GetComponent<PuppetScript>();
			if (puppet == null)
			{
				Logger.RError("Cannot find any PuppetScript in prefab: " + prefab.name);
				return;
			}
			EyeRaycast componentInChildren = base.transform.GetComponentInChildren<EyeRaycast>();
			if (componentInChildren != null)
			{
				componentInChildren.SetPuppet(puppet);
			}
			puppet.SetProtoActor(this);
			cc.enabled = false;
		}

		private void InstantiatePuppet(GameObject prefab)
		{
			if (puppet != null)
			{
				UnityEngine.Object.Destroy(puppet.gameObject);
				puppet = null;
			}
			GameObject gameObject = UnityEngine.Object.Instantiate(prefab, base.transform);
			puppet = gameObject.GetComponent<PuppetScript>();
			if (puppet == null)
			{
				Logger.RError("Cannot find any PuppetScript in prefab: " + prefab.name);
			}
			else
			{
				puppet.SetProtoActor(this);
			}
		}

		public void ChangeMonsterPuppetByMasterID(int masterID)
		{
			MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(masterID);
			if (monsterInfo == null)
			{
				Logger.RError($"Cannot find any MonsterInfo: masterID={masterID}");
				return;
			}
			monsterMasterID = masterID;
			puppetName = monsterInfo.PuppetName;
			RemainAfterDeath = monsterInfo.RemainAfterDeath;
			MMPuppetTable.Row row = Hub.s.tableman.monsterPuppet.FindRow(puppetName);
			if (row == null)
			{
				Logger.RError("Cannot find any MMPuppetData: puppetName=" + puppetName);
			}
			else
			{
				InstantiatePuppet(row.prefab);
			}
		}

		public void ChangePlayerPuppetByMasterID(int masterID)
		{
			if (AmIAvatar())
			{
				puppet.enabled = true;
				return;
			}
			PlayerMasterInfo playerInfo = Hub.s.dataman.ExcelDataManager.GetPlayerInfo(masterID);
			if (playerInfo == null)
			{
				Logger.RError($"Cannot find any PlayerInfo: masterID={masterID}");
				return;
			}
			playerMasterID = masterID;
			puppetName = (AmIAvatar() ? playerInfo.FppPuppetName : playerInfo.TppPuppetName);
			MMPuppetTable.Row row = Hub.s.tableman.monsterPuppet.FindRow(puppetName);
			if (row == null)
			{
				Logger.RError("Cannot find any MMPuppetData: puppetName=" + puppetName);
			}
			else
			{
				InstantiatePuppet(row.prefab);
			}
		}

		public void OnBeforeTick()
		{
			SendTransform();
		}

		public void SetRotationForce(Rotator rotator)
		{
			base.transform.rotation = rotator;
			oldRotY = rotator.Yaw;
		}

		public void SetPositionAndRotationForce(Vector3 position, Rotator rotator)
		{
			base.transform.SetPositionAndRotation(position, rotator);
			oldPos = position;
			oldRotY = rotator.Yaw;
		}

		private void SendTransform()
		{
			if (dead || Hub.s == null || Hub.s.netman2 == null)
			{
				return;
			}
			if (!oldPos.HasValue)
			{
				oldPos = base.transform.position;
				oldRotY = base.transform.rotation.eulerAngles.y;
				return;
			}
			Vector3 value = oldPos.Value;
			float num = oldRotY;
			float x = camRoot.localRotation.eulerAngles.x;
			float num2 = 1f / paConfig.sendPositionFrequency;
			Vector3 position = base.transform.position;
			float y = base.transform.rotation.eulerAngles.y;
			isMoved = Vector3.Distance(value, position) > 0.01f;
			isRotated = Mathf.Abs(num - y) > 1f || Mathf.Abs(oldCamRotX - x) > 1f;
			if (isMoved || isRotated)
			{
				if (pauseTransformSync)
				{
					if (needsToSendMoveStop)
					{
						TrySendMoveStop();
					}
					return;
				}
				if (!isMoved && isRotated)
				{
					main?.SendPacket(new ChangeViewPointReq
					{
						pitch = x,
						angle = y
					});
				}
				else
				{
					if (IsInputDisabledBy(EInputDisableReason.GrabbedByOther))
					{
						return;
					}
					main?.SendPacket(new MoveStartReq
					{
						transID = sendTransformTransID,
						actorMoveType = (IsFalling() ? ActorMoveType.FallDown : ((!isSprinting) ? ActorMoveType.Walk : ActorMoveType.Run)),
						basePositionPrev = MakePosWithAngle(value, x, num),
						basePositionCurr = MakePosWithAngle(position, x, y),
						basePositionFuture = MakePosWithAngle(position + (position - value), x, y),
						futureTime = (int)(num2 * 1000f),
						targetActorID = 0,
						pitch = x
					});
					moveStartFallbackList.Add((sendTransformTransID, value, num, x));
					sendTransformTransID++;
					needsToSendMoveStop = true;
				}
			}
			else
			{
				TrySendMoveStop();
			}
			oldPos = position;
			oldRotY = y;
			oldCamRotX = x;
		}

		[AvatarResPacketHandler(MsgType.C2S_MoveStartRes)]
		private void OnPacket(UseSkillRes res)
		{
			_ = res.errorCode;
		}

		[AvatarResPacketHandler(MsgType.C2S_MoveStartRes)]
		private void OnMoveStartRes(MoveStartRes res)
		{
			if (res != null && res.errorCode != MsgErrorCode.WillBeTeleported && res.errorCode != MsgErrorCode.Success)
			{
				float camRotX = camRoot.rotation.eulerAngles.x;
				PosWithRot transformFailFallback = null;
				moveStartFallbackList.FindAll(((int transId, Vector3 pos, float rotY, float camRotX) x) => x.transId == res.transID).ForEach(delegate((int transId, Vector3 pos, float rotY, float camRotX) x)
				{
					transformFailFallback = MakePosWithAngle(x.pos, camRoot.localRotation.eulerAngles.x, x.rotY);
					camRotX = x.camRotX;
				});
				if (transformFailFallback != null)
				{
					cc.enabled = false;
					base.transform.position = transformFailFallback.toVector3();
					base.transform.rotation = Quaternion.Euler(0f, transformFailFallback.yaw, 0f);
					camRoot.localRotation = Quaternion.Euler(camRotX, 0f, 0f);
					cc.enabled = true;
				}
			}
			if (res != null)
			{
				moveStartFallbackList.RemoveAll(((int transId, Vector3 pos, float rotY, float camRotX) x) => x.transId == res.transID);
			}
		}

		[AvatarResPacketHandler(MsgType.C2S_ChangeViewPointRes)]
		private void OnChangeViewPointRes(ChangeViewPointRes res)
		{
		}

		public void TrySendMoveStop()
		{
			if (needsToSendMoveStop && oldPos.HasValue)
			{
				main?.SendPacket(new MoveStopReq
				{
					actorMoveType = ActorMoveType.None,
					prevPos = MakePosWithAngle(oldPos.Value, camRoot.localRotation.eulerAngles.x, oldRotY),
					currentPos = MakePosWithAngle(oldPos.Value, camRoot.localRotation.eulerAngles.x, oldRotY)
				});
				needsToSendMoveStop = false;
			}
		}

		[AvatarResPacketHandler(MsgType.C2S_MoveStopRes)]
		private void OnMoveStopRes(MoveStopRes res)
		{
			if (res.errorCode != MsgErrorCode.WillBeTeleported)
			{
				_ = res.errorCode;
			}
		}

		private PosWithRot MakePosWithAngle(Vector3 pos, float pitch, float yaw)
		{
			PosWithRot posWithRot = new PosWithRot();
			posWithRot.x = pos.x;
			posWithRot.y = pos.y;
			posWithRot.z = pos.z;
			posWithRot.pitch = pitch;
			posWithRot.yaw = yaw;
			return posWithRot;
		}

		private PosWithRot MyPosWithAngle()
		{
			PosWithRot posWithRot = new PosWithRot();
			posWithRot.x = base.transform.position.x;
			posWithRot.y = base.transform.position.y;
			posWithRot.z = base.transform.position.z;
			posWithRot.pitch = camRoot.localRotation.eulerAngles.x;
			posWithRot.yaw = base.transform.rotation.eulerAngles.y;
			return posWithRot;
		}

		private bool TryProcessInteractKey()
		{
			bool result = false;
			if (!fakeJumper.IsJumping() && !skillHelper.HasAnyCooltime())
			{
				result = inputman.isPressed(InputAction.Interact);
				if (inputman.wasPressedThisFrame(InputAction.Interact))
				{
					if (inventory.IsScrapMotionPlaying())
					{
						inventory.SendEndScrapMotion();
					}
					result = main.TryPerformInteraction();
				}
				if (inputman.wasRelesedThisFrame(InputAction.Interact))
				{
					result = main.TryPerformInteractionEnd();
				}
			}
			return result;
		}

		private void ProcessPushToTalkKey()
		{
			if (Hub.s.gameSettingManager.voiceMode == CommActivationMode.PushToTalk)
			{
				if (Hub.s.inputman.isPressed(InputAction.PushToTalk))
				{
					Hub.s.voiceman.GetPlayerBroadcastTrigger().pushToTalkButtonPushed = true;
					Hub.s.voiceman.GetObserverBroadcastTrigger().pushToTalkButtonPushed = true;
					Hub.s.voiceman.GetTransmitterBroadcastTrigger().pushToTalkButtonPushed = true;
				}
				else
				{
					Hub.s.voiceman.GetPlayerBroadcastTrigger().pushToTalkButtonPushed = false;
					Hub.s.voiceman.GetObserverBroadcastTrigger().pushToTalkButtonPushed = false;
					Hub.s.voiceman.GetTransmitterBroadcastTrigger().pushToTalkButtonPushed = false;
				}
			}
		}

		private void ProcessFireKey()
		{
			if (inputman == null)
			{
				return;
			}
			InventoryItem selectedItem = inventory.SelectedItem;
			if (selectedItem == null)
			{
				return;
			}
			int num = -1;
			if (inputman.wasPressedThisFrame(InputAction.FireWeapon))
			{
				num = 0;
			}
			else if (inputman.wasPressedThisFrame(InputAction.FireWeapon2))
			{
				num = 1;
			}
			if (num < 0)
			{
				return;
			}
			if (selectedItem.TryCastMasterInfo<ItemEquipmentInfo>(out ItemEquipmentInfo masterInfo) && masterInfo != null)
			{
				if (masterInfo.EquipPartsType == EquipPartsType.SkillEquip && masterInfo.SkillList.Count() > 0)
				{
					TryUseEquipSkill(masterInfo, selectedItem, num);
				}
				if (num == 0 && masterInfo.EquipPartsType == EquipPartsType.ToggleEquip)
				{
					ChangeEquipStatus(!selectedItem.IsTurnOn);
				}
			}
			if (num == 0 && selectedItem.TryCastMasterInfo<ItemConsumableInfo>(out ItemConsumableInfo masterInfo2) && masterInfo2 != null && masterInfo2.ConsumeType == ConsumeItemType.Potion)
			{
				RequeseUseItem();
			}
			if (selectedItem.TryCastMasterInfo<ItemMiscellanyInfo>(out ItemMiscellanyInfo masterInfo3) && masterInfo3 != null)
			{
				if (inventory.IsScrapMotionPlaying())
				{
					inventory.SendEndScrapMotion();
				}
				else if (!string.IsNullOrWhiteSpace(masterInfo3.ScrapAnimationStateName))
				{
					inventory.SendStartScrapMotion();
				}
			}
		}

		private void TryUseEquipSkill(ItemEquipmentInfo equipMasterInfo, InventoryItem selectedItem, int skillIndex)
		{
			if (skillIndex >= equipMasterInfo.SkillList.Count() || (skillIndex == 1 && equipMasterInfo.SkillList[skillIndex].SkillMasterIDWithGauge == 0))
			{
				return;
			}
			SkillPair skillPair = equipMasterInfo.SkillList[skillIndex];
			int skillMasterIDNoGague = skillPair.SkillMasterIDNoGague;
			int skillMasterIDWithGauge = skillPair.SkillMasterIDWithGauge;
			if (skillMasterIDNoGague == 0 && skillMasterIDWithGauge == 0)
			{
				return;
			}
			int num = skillMasterIDNoGague;
			if (skillMasterIDWithGauge > 0)
			{
				SkillInfo skillInfo = Hub.s.dataman.ExcelDataManager.GetSkillInfo(skillMasterIDWithGauge);
				if (skillInfo == null)
				{
					Logger.RError($"Cannot find any SkillInfo: skillMasterID={skillMasterIDWithGauge}");
					return;
				}
				num = ((selectedItem.RemainGauge >= skillInfo.DecGauge) ? skillMasterIDWithGauge : skillMasterIDNoGague);
			}
			if (num > 0)
			{
				skillHelper.UseSkill(num, null);
			}
		}

		private void CancelSkillAnim(string cancelStateName)
		{
			if (puppet != null)
			{
				puppet.CancelSkill(cancelStateName);
			}
		}

		private void RequeseUseItem()
		{
			InventoryItem selectedItem = inventory.SelectedItem;
			if (selectedItem != null && selectedItem.TryCastMasterInfo<ItemConsumableInfo>(out ItemConsumableInfo masterInfo) && masterInfo.ConsumeType == ConsumeItemType.Potion)
			{
				if (masterInfo.UseEffectId != 0)
				{
					PlaySkillHitEffect(masterInfo.UseEffectId);
				}
				main.SendPacket(new UseItemReq());
			}
		}

		private void ProcessReloadKey()
		{
			if (!(inputman == null) && inputman.wasPressedThisFrame(InputAction.ReloadWeapon))
			{
				InventoryItem selectedItem = inventory.SelectedItem;
				if (selectedItem != null && selectedItem.TryCastMasterInfo<ItemEquipmentInfo>(out ItemEquipmentInfo masterInfo) && masterInfo != null && masterInfo.EquipPartsType == EquipPartsType.SkillEquip && masterInfo.MaxGauge > selectedItem.RemainGauge && masterInfo.ReloadSkillMasterID > 0)
				{
					skillHelper.UseSkill(masterInfo.ReloadSkillMasterID, null);
				}
			}
		}

		private void ProcessDropKey()
		{
			if (!(inputman == null) && inputman.wasPressedThisFrame(InputAction.DropItemOnHand))
			{
				inventory.SendReleaseItem();
			}
		}

		private void ProcessJumpKey()
		{
			fakeJumper.TryStartJumpByInput();
		}

		private void ProcessEmoteKey()
		{
			if (emotePlayer.TryStopEmoteByInput())
			{
				_ = 1;
			}
			else
				emotePlayer.TryStartEmoteByInput();
		}

		private void ProcessUISelectKey()
		{
			if (!(inputman == null) && !(main == null))
			{
				if (inputman.wasPressedThisFrame(InputAction.UI_PREV))
				{
					main.TryPerformInteractionEnd();
					inventory.SelectPreviousSlot();
				}
				if (inputman.wasPressedThisFrame(InputAction.UI_NEXT))
				{
					main.TryPerformInteractionEnd();
					inventory.SelectNextSlot();
				}
			}
		}

		private void ProcessGamePadEmoteKey()
		{
			if (inputman.wasPressedThisFrame(InputAction.EmotePanel))
			{
				Hub.s.uiman.ShowGamepadEmote();
			}
		}

		private void RotateByInput()
		{
			if (!(inputman == null))
			{
				float y = base.transform.rotation.eulerAngles.y;
				float x = camRoot.localRotation.eulerAngles.x;
				float num = 0f;
				num = inputman.mouseMovement.x * turnSpeed * inputman.mouseSensetivity;
				float num2 = 0f - inputman.mouseMovement.y * turnSpeed * inputman.mouseSensetivity * (float)inputman.invertYAxis;
				if (Hub.GetGamepad() != null && !inputman.isGamepadEmoteActive)
				{
					num += inputman.gamepadRightStick.x * turnSpeed * inputman.mouseSensetivity * 80f;
					num2 -= inputman.gamepadRightStick.y * turnSpeed * inputman.mouseSensetivity * (float)inputman.invertYAxis * 80f;
				}
				float y2 = y + num;
				float num3 = x + Mathf.Clamp(num2, -90f, 90f);
				float x2 = num3;
				if (num3 > 180f && num3 < 360f + rotXMin)
				{
					x2 = 360f + rotXMin;
				}
				if (num3 < 180f && num3 > rotXMax)
				{
					x2 = rotXMax;
				}
				base.transform.rotation = Quaternion.Euler(0f, y2, 0f);
				camRoot.localRotation = Quaternion.Euler(x2, 0f, 0f);
			}
		}

		private Vector2 GetMovementInput()
		{
			Vector2 zero = Vector2.zero;
			if (inputman.isPressed(InputAction.MoveForward) || debug_keepWalking)
			{
				zero.y += 1f;
			}
			if (inputman.isPressed(InputAction.MoveBackward))
			{
				zero.y -= 1f;
			}
			if (inputman.isPressed(InputAction.MoveLeft))
			{
				zero.x -= 1f;
			}
			if (inputman.isPressed(InputAction.MoveRight))
			{
				zero.x += 1f;
			}
			if (Hub.GetGamepad() != null)
			{
				zero += inputman.gamepadLeftStick;
			}
			if (zero.magnitude > 1f)
			{
				zero.Normalize();
			}
			return zero;
		}

		private bool ProcessSprintKey(bool isSprintingNow)
		{
			if (inputman == null)
			{
				return false;
			}
			if (main == null)
			{
				return false;
			}
			bool result = isSprintingNow;
			if (IsInputMoveAvailable())
			{
				if (inputman.wasPressedThisFrame(InputAction.Run) && !isSprintingNow && netSyncActorData.stamina >= Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumeValue)
				{
					main.SendPacket(new ToggleSprintReq
					{
						isSprint = true
					});
					result = true;
				}
			}
			else if (isSprintingNow)
			{
				main.SendPacket(new ToggleSprintReq
				{
					isSprint = false
				});
				result = false;
			}
			if ((inputman.wasRelesedThisFrame(InputAction.Run) || netSyncActorData.stamina < Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumeValue) && isSprintingNow)
			{
				main.SendPacket(new ToggleSprintReq
				{
					isSprint = false
				});
				result = false;
			}
			return result;
		}

		private float CaculateSpeed()
		{
			if (!isSprinting)
			{
				return walkSpeed;
			}
			return runSpeed;
		}

		public bool IsFalling()
		{
			return Math.Abs(falling) >= Mathf.Abs(Physics.gravity.y * 0.1f);
		}

		private void Update()
		{
			if (!(Hub.s == null) && Hub.s.pdata != null && !(Hub.s.gameConfig == null) && !(inputman == null) && !(main == null))
			{
				CheckGrounded();
				UpdateHelper();
				UpdateDL();
				ProcessPushToTalkKey();
				UpdateControl();
			}
		}

		private void UpdateHelper()
		{
			float deltaTime = Time.deltaTime;
			voiceEffecter?.Update(deltaTime);
		}

		private void UpdateControl()
		{
			float deltaTime = Time.deltaTime;
			if (controlMode == ControlMode.Manual)
			{
				if (dontMoveFlag || Hub.s.console.IsConsoleActive())
				{
					return;
				}
				if (!dead)
				{
					if (inputman.IsCapturing())
					{
						if (IsInputAvailable())
						{
							if (!TryProcessInteractKey())
							{
								ProcessFireKey();
								ProcessReloadKey();
								ProcessDropKey();
								ProcessGamePadEmoteKey();
								ProcessEmoteKey();
								ProcessJumpKey();
							}
						}
						else
						{
							main.TryPerformInteractionEnd();
						}
					}
					if (!IsInputDisabledBy(EInputDisableReason.SkillCast) && !IsInputDisabledBy(EInputDisableReason.GameNotStarted) && inputman.IsCapturing())
					{
						ProcessUISelectKey();
					}
				}
				movementInput = Vector2.zero;
				if (!dead && inputman.IsCapturing())
				{
					if (IsInputMoveAvailable())
					{
						RotateByInput();
					}
					if (IsInputMoveAvailable())
					{
						movementInput = GetMovementInput();
					}
				}
				if (!dead)
				{
					isSprinting = ProcessSprintKey(isSprinting);
				}
				float num = CaculateSpeed();
				lastCharacterControlerHorizontalVelocity = (base.transform.forward * movementInput.y + base.transform.right * movementInput.x) * num;
				Vector3 vector = lastCharacterControlerHorizontalVelocity * deltaTime;
				if (grounded)
				{
					flying = Vector3.zero;
					falling = -1f * deltaTime;
				}
				else
				{
					if (lastGrounded)
					{
						flying = vector;
					}
					if (dead)
					{
						falling = Physics.gravity.y * 0.05f * deltaTime;
					}
					else
					{
						falling += Physics.gravity.y * deltaTime;
					}
				}
				_ = falling;
				Vector3 position = base.transform.position;
				if (cc.enabled)
				{
					if (grounded)
					{
						cc.Move(vector + Vector3.up * falling * deltaTime);
					}
					else
					{
						cc.Move(flying + Vector3.up * falling * deltaTime);
					}
				}
				Vector3 targetVelocity = (base.transform.position - position) / Time.deltaTime;
				puppet.Move(targetVelocity, walkSpeed, runSpeed);
			}
			else if (controlMode == ControlMode.Remote)
			{
				if (dead || pauseTransformSync)
				{
					return;
				}
				TransformInterpolation(deltaTime);
			}
			lastGrounded = grounded;
		}

		private void LateUpdate()
		{
			UpdateNonAvatarCulling();
		}

		private void UpdateNonAvatarCulling()
		{
			if (Hub.s == null || Hub.s.pdata == null || Hub.s.pdata.main == null || AmIAvatar())
			{
				return;
			}
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene == null || gamePlayScene.dungenCuller == null)
			{
				return;
			}
			AdjacentRoomCulling dungenCuller = gamePlayScene.dungenCuller;
			if (dungenCuller == null)
			{
				return;
			}
			if (!dungenCuller.enabled)
			{
				currentTile = null;
				if (puppet.IsRenderersVisible)
				{
					puppet.TurnOnRenderes();
				}
				return;
			}
			if (gamePlayScene.IsCameraTargetTileChanged)
			{
				currentTile = null;
			}
			Tile tile = currentTile;
			if (currentTile == null)
			{
				currentTile = dungenCuller.relumod_FastFindCurrentTile(base.transform, 1f);
			}
			else if (!currentTile.Bounds.Contains(base.transform.position + Vector3.up * 1f))
			{
				currentTile = dungenCuller.relumod_FastFindCurrentTile(base.transform, 1f);
			}
			bool flag;
			bool num = (flag = puppet.IsRenderersVisible);
			if (gamePlayScene.IsCameraTargetOutdoor())
			{
				flag = ((currentTile == null) ? true : false);
			}
			else if (tile != currentTile || gamePlayScene.IsCameraTargetTileChanged || gamePlayScene.IsCameraTargetOutdoorChanged)
			{
				flag = !(currentTile == null) && dungenCuller.IsTileVisible(currentTile);
			}
			if (!num && flag)
			{
				puppet.TurnOnRenderes();
			}
			if (num && !flag)
			{
				puppet.TurnOffRenderes();
			}
		}

		private float CorrectEulerRot2(float rot, float target)
		{
			if (rot - target > 180f)
			{
				target += 360f;
			}
			else if (rot - target < -180f)
			{
				target -= 360f;
			}
			return target;
		}

		public IEnumerator CorMove(Vector3 start, Vector3 dest, float durationSec)
		{
			Transform tr = base.transform;
			Hub.s.timeutil.GetCurrentTickMilliSec();
			float elapsed = 0f;
			while (elapsed < durationSec)
			{
				float t = elapsed / durationSec;
				tr.position = Vector3.Lerp(start, dest, t);
				elapsed += Time.deltaTime;
				yield return null;
			}
			tr.position = dest;
		}

		private void EquipHandheld(InventoryItem? item = null)
		{
			if (item != null && item.ItemMasterID == 0)
			{
				item = null;
			}
			if (AmIAvatar())
			{
				string text = string.Empty;
				if (item != null)
				{
					foreach (string item2 in item.MasterInfo.ToolTip)
					{
						string text2 = item2.Replace(" ", "");
						text = text + text2 + "," + item.MasterInfo.Name + "\n";
					}
				}
				Hub.s.uiman.ShowGameTips_v2(text);
			}
			if (item != null && item.IsAccessory)
			{
				item = null;
			}
			if ((handheldItem == null && item == null) || (handheldItem != null && item != null && handheldItem.ItemID == item.ItemID))
			{
				return;
			}
			long num = handheldItem?.ItemID ?? 0;
			long num2 = handheldItem?.ItemMasterID ?? 0;
			if (handheldItem != null)
			{
				if (handheldItem.TryGetComponent<SocketAttachable>(out SocketAttachable component))
				{
					component.OnDetachFromSocket();
				}
				handheldItem.DestroyGameObject();
			}
			GameObject gameObject = null;
			if (item != null)
			{
				gameObject = item.InstantiateGameObject();
			}
			if (gameObject != null)
			{
				Transform transform = gameObject.transform;
				string attachSocketName = item.MasterInfo.AttachSocketName;
				Transform transform2 = SocketNodeMarker.FindFirstInHierarchy(base.transform, attachSocketName);
				if (transform2 != null)
				{
					string socketName = attachSocketName + "_pivot";
					Transform transform3 = SocketNodeMarker.FindFirstInHierarchy(transform, socketName);
					if (transform3 != null)
					{
						transform.localPosition = transform3.localPosition;
						transform.localRotation = transform3.localRotation;
						transform.localScale = transform3.localScale;
					}
					transform.SetParent(transform2, worldPositionStays: false);
				}
				else
				{
					Logger.RError("Socket not found: socket=" + attachSocketName + ", actor=" + base.gameObject.name + ", puppet=" + puppet.gameObject.name);
					transform.SetParent(base.transform, worldPositionStays: false);
				}
				if (gameObject.TryGetComponent<SocketAttachable>(out var component2))
				{
					component2.Initialize(this, item);
					component2.OnAttachToSocket();
				}
				handheldItem = item;
				puppet.HoldByHand(item.MasterInfo.PuppetHandheldState);
			}
			else
			{
				handheldItem = null;
				puppet.HoldByHand(PuppetHandheldState.Empty);
			}
			bool prevItemIsInInventory = false;
			if (num != 0L && num2 != 0L && inventory.FindItemByID(num) != null)
			{
				prevItemIsInInventory = true;
			}
			OnEquipChanged(num, num2, item?.ItemID ?? 0, item?.ItemMasterID ?? 0, prevItemIsInInventory);
		}

		public InventoryItem? GetHandheldItem()
		{
			return handheldItem;
		}

		private void EquipAccessories(List<InventoryItem?> newSlotItems)
		{
			foreach (InventoryItem accessoryItem in accessoryItems)
			{
				if (accessoryItem != null && accessoryItem.IsAccessory)
				{
					accessoryItem.DestroyGameObject();
				}
			}
			accessoryItems.Clear();
			List<InventoryItem> list = new List<InventoryItem>();
			foreach (InventoryItem newSlotItem in newSlotItems)
			{
				if (newSlotItem != null && newSlotItem.IsAccessory)
				{
					list.Add(newSlotItem);
				}
			}
			for (int i = 0; i < list.Count; i++)
			{
				InventoryItem inventoryItem = list[i];
				InventoryItem inventoryItem2 = null;
				for (int num = i - 1; num >= 0; num--)
				{
					InventoryItem inventoryItem3 = list[num];
					if (inventoryItem3 != null && inventoryItem3.AccessoryGroup == inventoryItem.AccessoryGroup)
					{
						inventoryItem2 = inventoryItem3;
						break;
					}
				}
				string attachSocketName = inventoryItem.MasterInfo.AttachSocketName;
				Transform transform = ((inventoryItem2 == null) ? SocketNodeMarker.FindFirstInHierarchy(base.transform, attachSocketName, includeInactive: false) : SocketNodeMarker.FindFirstInHierarchy(inventoryItem2.Transform, attachSocketName, includeInactive: false));
				if (transform == null)
				{
					continue;
				}
				GameObject gameObject = inventoryItem.InstantiateGameObject();
				if (gameObject == null)
				{
					Logger.RError("Failed to instantiate gameObject in EquipAccessories");
					continue;
				}
				if (AmIAvatar())
				{
					inventoryItem.SetPersonViewMode(PersonViewMode.First);
				}
				Transform transform2 = gameObject.transform;
				string socketName = attachSocketName + "_pivot";
				Transform transform3 = SocketNodeMarker.FindFirstInHierarchy(transform2, socketName);
				if (transform3 != null)
				{
					transform2.localPosition = transform3.localPosition;
					transform2.localRotation = transform3.localRotation;
					transform2.localScale = transform3.localScale;
				}
				transform2.SetParent(transform, worldPositionStays: false);
				accessoryItems.Add(inventoryItem);
			}
		}

		public void StartScrapMotion(string stateName, out AnimatorStateInfo stateInfo)
		{
			if (puppet != null)
			{
				puppet.StartScrapMotion(stateName, out stateInfo);
			}
			else
			{
				stateInfo = default(AnimatorStateInfo);
			}
		}

		public void StopScrapMotion()
		{
			if (puppet != null)
			{
				puppet.StopScrapMotion();
			}
		}

		public void OnPuppetMove(float forward, float strafe, float speed)
		{
			if (handheldItem != null && handheldItem.TryGetComponent<SocketAttachable>(out SocketAttachable component))
			{
				component.OnPuppetMove(forward, strafe, speed);
			}
		}

		public void OnFire()
		{
			if (handheldItem != null && handheldItem.TryGetComponent<SocketAttachable>(out SocketAttachable component))
			{
				component.OnFire();
			}
		}

		public void OnReload()
		{
			if (handheldItem != null && handheldItem.TryGetComponent<SocketAttachable>(out SocketAttachable component))
			{
				component.OnReload();
			}
		}

		public void UpdateMountedLight()
		{
			TurnMountedLight(inventory.IsTurnedOnMountedLight());
		}

		public bool IsMountedLightOn()
		{
			if (mountedLight == null)
			{
				return false;
			}
			return mountedLight.gameObject.activeSelf;
		}

		private void TurnMountedLight(bool on)
		{
			if (mountedLight != null && IsMountedLightOn() != on)
			{
				mountedLight.gameObject.SetActive(on);
			}
		}

		public void OverrideMountedLight(Light light)
		{
			if (!(mountedLight == null) && !(light == null))
			{
				mountedLight.type = light.type;
				mountedLight.renderMode = light.renderMode;
				mountedLight.spotAngle = light.spotAngle;
				mountedLight.innerSpotAngle = light.innerSpotAngle;
				mountedLight.color = light.color;
				mountedLight.intensity = light.intensity;
				mountedLight.bounceIntensity = light.bounceIntensity;
				mountedLight.range = light.range;
				mountedLight.renderingLayerMask = light.renderingLayerMask;
				mountedLight.cullingMask = light.cullingMask;
				mountedLight.renderMode = light.renderMode;
				mountedLight.shadows = light.shadows;
				mountedLight.shadowStrength = light.shadowStrength;
				mountedLight.shadowResolution = light.shadowResolution;
				mountedLight.shadowBias = light.shadowBias;
				mountedLight.shadowNormalBias = light.shadowNormalBias;
				mountedLight.shadowNearPlane = light.shadowNearPlane;
			}
		}

		private bool HasHandheldItem()
		{
			return handheldItem != null;
		}

		private bool IsHandheldItem(long itemID)
		{
			if (handheldItem == null || itemID < 0)
			{
				return false;
			}
			return handheldItem.ItemID == itemID;
		}

		private void ReserveShowHandItem(float skillDuration)
		{
			StartCoroutine(ShowHandItemCoroutine(skillDuration));
		}

		private IEnumerator ShowHandItemCoroutine(float skillDuration)
		{
			yield return new WaitForSeconds(skillDuration);
			if (handheldItem != null)
			{
				handheldItem.Transform.gameObject.SetActive(value: true);
			}
		}

		public void UpdateChangedItemInfo(ItemInfo itemInfo)
		{
			if (itemInfo == null)
			{
				Logger.RError("UpdateChangedItemInfo called with null itemInfo");
				return;
			}
			long itemID = itemInfo.itemID;
			if (HasHandheldItem() && IsHandheldItem(itemID))
			{
				inventory.UpdateSelectedItemComponents();
			}
			else if (itemInfo.remainGauge <= 0 && IsFlashLight(itemInfo.itemMasterID))
			{
				UpdateMountedLight();
			}
		}

		private void ForceTurnOffFlashLightAndMountedLight()
		{
			if (handheldItem != null && IsFlashLight(handheldItem.ItemMasterID) && handheldItem.TryGetComponent<Flashlight>(out Flashlight component))
			{
				component?.OnToggled(handheldItem.ItemID, toggleOn: false);
			}
			TurnMountedLight(on: false);
		}

		private bool IsFlashOrMountedLightOn()
		{
			if (handheldItem != null && IsFlashLightOn(handheldItem))
			{
				return true;
			}
			return IsMountedLightOn();
		}

		private bool IsFlashLightOn(InventoryItem item)
		{
			if (IsFlashLight(item.ItemMasterID))
			{
				return item.IsTurnOn;
			}
			return false;
		}

		private bool IsFlashLight(int itemMasterID)
		{
			if (itemMasterID != 2000)
			{
				return itemMasterID == 2500;
			}
			return true;
		}

		public bool IsHoldingTimeBombFastBeeping()
		{
			if (handheldItem == null)
			{
				return false;
			}
			if (handheldItem.TryGetComponent<TimeBomb>(out TimeBomb component))
			{
				return component.IsFastBeeping();
			}
			return false;
		}

		private void CheckGrounded()
		{
			float num = 0.05f;
			RaycastHit[] array = Physics.SphereCastAll(base.transform.position + cc.center, cc.radius, Vector3.down, cc.height * checkGroundedDistance - cc.radius + num + cc.skinWidth, Hub.DefaultOnlyLayerMask, QueryTriggerInteraction.Ignore);
			grounded = false;
			RaycastHit[] array2 = array;
			for (int i = 0; i < array2.Length; i++)
			{
				RaycastHit raycastHit = array2[i];
				if (!(raycastHit.collider == null) && !(raycastHit.collider.GetComponent<EZSoftBoneCollider>() != null) && !raycastHit.collider.TryGetComponent<ProtoActor>(out var _) && raycastHit.collider.gameObject != base.gameObject)
				{
					grounded = true;
				}
			}
		}

		private Vector3 DropToPhysicsCollider(Vector3 from)
		{
			Vector3 result = from;
			float num = 1f;
			float maxDistance = 2f;
			if (Physics.Raycast(from + num * Vector3.up, Vector3.down, out var hitInfo, maxDistance, Hub.DefaultOnlyLayerMask, QueryTriggerInteraction.Ignore))
			{
				result = hitInfo.point;
			}
			return result;
		}

		public void ShowDebugText(string text)
		{
			debugText.gameObject.SetActive(value: true);
			debugText.text = text;
		}

		public void HideDebugText()
		{
			debugText.gameObject.SetActive(value: false);
		}

		public void ShowDebugBTStateInfoText()
		{
			if (!(debugText == null))
			{
				debugText.gameObject.SetActive(value: true);
				UpdateDebugBTStateInfoText();
			}
		}

		public void UpdateDebugBTStateInfoText()
		{
			if (!(debugText == null))
			{
				string text = "Name: <color=yellow>" + debugBTStateInfo.btDataName + "</color>\nState: <color=red>" + debugBTStateInfo.btTemplateName + "</color>";
				if (IsMimic())
				{
					string text2 = ((_debugMovementModelType == 0) ? "Stay" : ((_debugMovementModelType == 1) ? "LeftExplorer" : ((_debugMovementModelType == 2) ? "RightExplorer" : ((_debugMovementModelType == 3) ? "Follower" : ((_debugMovementModelType == 4) ? "Runaway" : ((_debugMovementModelType == 5) ? "ToTram" : ((_debugMovementModelType == 6) ? "ToEntrance" : ((_debugMovementModelType == 7) ? "EnterTram" : ((_debugMovementModelType == 8) ? "ExitTram" : ((_debugMovementModelType == 9) ? "OutsideOther" : ((_debugMovementModelType == 10) ? "InsideTram" : ((_debugMovementModelType == 11) ? "ToTramFromBackdoor" : ((_debugMovementModelType == 12) ? "ToTramViaHelper" : ((_debugMovementModelType == 13) ? "LookAtStashHanger" : ((_debugMovementModelType == 14) ? "LookAtScrapScanner" : "Unknown")))))))))))))));
					text = text + "\nVoiceRule: <color=green>" + _debugVoiceRule.ToString() + "</color>\n";
					text = text + "ModelType: <color=lightblue>" + text2 + "</color>\n";
					text = text + "Voice: <color=yellow>" + _voicePickReason + "</color>\n";
				}
				debugText.text = text;
			}
		}

		public void HideDebugBTStateInfoText()
		{
			if (!(debugText == null))
			{
				debugText.text = "";
				debugText.gameObject.SetActive(value: false);
			}
		}

		public void ShowOverlayDebugText(string text)
		{
			debugOverlayText.gameObject.SetActive(value: true);
			debugOverlayText.text = text;
		}

		public void HideOverlayDebugText()
		{
			debugOverlayText.gameObject.SetActive(value: false);
		}

		public bool AmIAvatar()
		{
			if (ActorType == ActorType.Player)
			{
				return controlMode == ControlMode.Manual;
			}
			return false;
		}

		public void DontMove()
		{
			dontMoveFlag = true;
		}

		public void CancelDontMove()
		{
			dontMoveFlag = false;
		}

		public void GrapLootingObject(int lootingObjectActorID)
		{
			inventory.SendGrapLootingObject(lootingObjectActorID);
		}

		public void BuyItemByMasterId(int inItemMasterId, int machineIndex, Action successCallback = null)
		{
			main?.SendPacketWithCallback(new BuyItemReq
			{
				itemMasterID = inItemMasterId,
				machineIndex = machineIndex
			}, delegate(BuyItemRes _res)
			{
				if (_res != null)
				{
					main?.UpdateCurrency(_res.remainCurrency);
					if (_res.errorCode != MsgErrorCode.Success)
					{
						if (_res.errorCode != MsgErrorCode.NotEnoughCurrency)
						{
							Logger.RError($"[BuyItemRes] BuyItemRes : {inItemMasterId}, {_res.errorCode}");
						}
					}
					else
					{
						successCallback?.Invoke();
					}
				}
			}, destroyToken);
		}

		public void BarterItem(Action<bool> successCallback)
		{
			main?.SendPacketWithCallback(new BarterItemReq(), delegate(BarterItemRes _res)
			{
				if (_res == null)
				{
					successCallback?.Invoke(obj: false);
				}
				else if (_res.errorCode != MsgErrorCode.Success)
				{
					if (_res.errorCode != MsgErrorCode.NotEnoughCurrency)
					{
						Logger.RError($"[BarterItemRes] BarterItem : error {_res.errorCode}");
					}
					successCallback?.Invoke(obj: false);
				}
				else
				{
					successCallback?.Invoke(obj: true);
				}
			}, destroyToken);
		}

		public void TryStartCharge(Action successCallback)
		{
			if (!inventory.IsCurrentChargeableItem() || !inventory.IsCurrentNeedToCharge())
			{
				return;
			}
			main?.SendPacketWithCallback(new ChargeItemReq(), delegate(ChargeItemRes _res)
			{
				if (_res != null)
				{
					if (_res.errorCode != MsgErrorCode.Success)
					{
						Logger.RError($"[ChargeItemRes] ChargeItemRes : errorCode {_res.errorCode}");
					}
					else
					{
						inventory.ResolveInventoryItemInfo(_res.currentHandEquipmentInfo);
						if (main != null)
						{
							main.UpdateInventoryUI(this);
						}
						AddIncomingEvent(SpeechEvent_IncomingType.ChargeCompleted, Time.realtimeSinceStartup + GetIncomingEventExpireTime(SpeechEvent_IncomingType.ChargeCompleted));
						if (successCallback != null)
						{
							successCallback();
						}
					}
				}
			}, destroyToken);
		}

		public void TryHangItem(int index)
		{
			main?.SendPacketWithCallback(new HangItemReq
			{
				index = index
			}, delegate(HangItemRes _res)
			{
				if (_res != null && _res.errorCode != MsgErrorCode.Success)
				{
					Logger.RError($"[HangItemRes] HangItem : errorCode {_res.errorCode}");
				}
			}, destroyToken);
		}

		public void TryUnhangItem(int index)
		{
			main?.SendPacketWithCallback(new UnhangItemReq
			{
				index = index
			}, delegate(UnhangItemRes _res)
			{
				if (_res != null && _res.errorCode != MsgErrorCode.Success)
				{
					Logger.RError($"[UnhangItemRes] UnhangItem : errorCode {_res.errorCode}");
				}
			}, destroyToken);
		}

		public void ReinforceItem(Action successCallback)
		{
			main?.SendPacketWithCallback(new ReinforceItemReq(), delegate(ReinforceItemRes _res)
			{
				if (_res != null)
				{
					if (_res.errorCode != MsgErrorCode.Success)
					{
						Logger.RError($"[ReinforceItemRes] ReinforceItem : errorCode {_res.errorCode}");
					}
					else
					{
						successCallback?.Invoke();
					}
				}
			}, destroyToken);
		}

		public bool IsCurrentChargeableItem()
		{
			return inventory.IsCurrentChargeableItem();
		}

		public bool IsCurrentNeedToCharge()
		{
			if (inventory.IsCurrentChargeableItem())
			{
				return inventory.IsCurrentNeedToCharge();
			}
			return false;
		}

		public void PutIntoStash()
		{
			main?.SendPacket(new PutIntoToiletReq());
		}

		public void ResolveStatCollection(StatCollection collection)
		{
			ResolveImmutableStats(collection.ImmutableStats);
			foreach (KeyValuePair<MutableStatType, long> mutableStat in collection.mutableStats)
			{
				switch (mutableStat.Key)
				{
				case MutableStatType.HP:
					UpdateHp(mutableStat.Value, netSyncActorData.maxHP);
					break;
				case MutableStatType.Conta:
					UpdateConta(mutableStat.Value, netSyncActorData.maxConta);
					break;
				case MutableStatType.Stamina:
					UpdateStamina(mutableStat.Value, netSyncActorData.maxStamina);
					break;
				}
			}
		}

		public void ResolveImmutableStats(Dictionary<StatType, long> immutableStats)
		{
			foreach (KeyValuePair<StatType, long> immutableStat in immutableStats)
			{
				switch (immutableStat.Key)
				{
				case StatType.HP:
					netSyncActorData.maxHP = immutableStat.Value;
					break;
				case StatType.MoveSpeedWalk:
					netSyncActorData.MoveSpeedWalk = immutableStat.Value;
					break;
				case StatType.MoveSpeedRun:
					netSyncActorData.MoveSpeedRun = immutableStat.Value;
					break;
				case StatType.Conta:
					netSyncActorData.maxConta = immutableStat.Value;
					break;
				case StatType.Stamina:
					netSyncActorData.maxStamina = immutableStat.Value;
					break;
				case StatType.VoicePitch:
					netSyncActorData.voicePitch = immutableStat.Value;
					voiceEffecter.SetPitchStat(immutableStat.Value);
					break;
				case StatType.VoiceAmplification:
					voiceEffecter.SetVoiceEffect(VoiceEffecter.VoiceEffectType.Amplification, immutableStat.Value > 0);
					break;
				case StatType.VoiceRange:
					voiceEffecter.SetVoiceEffect(VoiceEffecter.VoiceEffectType.Range, 0.01f < (float)immutableStat.Value);
					break;
				case StatType.VoiceVibration:
					voiceEffecter.SetVoiceEffect(VoiceEffecter.VoiceEffectType.Vibration, immutableStat.Value > 0);
					break;
				default:
					Logger.RError($"[ProtoActor] Resolve_ImmutableStats : Unknown StatType {immutableStat.Key}");
					break;
				case StatType.Attack:
				case StatType.Defense:
				case StatType.AbnormalTriggerGauge:
				case StatType.MAX:
					break;
				}
			}
		}

		public float GetNavMeshPathDist(ProtoActor targetActor)
		{
			float result = -1f;
			if (targetActor == null)
			{
				return result;
			}
			PathFindResult route = Hub.s.navman.GetRoute(base.transform.position, targetActor.transform.position);
			if (route.Success)
			{
				result = route.Length;
			}
			return result;
		}

		public bool IsMimic()
		{
			if (ActorType != ActorType.Monster)
			{
				return false;
			}
			return Hub.s.dataman.ExcelDataManager.GetMonsterInfo(monsterMasterID)?.IsMimic() ?? false;
		}

		public bool IsPlayer()
		{
			return ActorType == ActorType.Player;
		}

		private void ChangeEquipStatus(bool isTurnOn)
		{
			main?.SendPacket(new ChangeEquipStatusReq
			{
				isTurnOn = isTurnOn
			});
		}

		[AvatarResPacketHandler(MsgType.C2S_ChangeEquipStatusRes)]
		private void OnPacket(ChangeEquipStatusRes res)
		{
			if (res == null)
			{
				Logger.RError("ChangeEquipStatusReq failed : res is null");
			}
			else
			{
				_ = res.errorCode;
			}
		}

		public void SwitchPersonViewMode(PersonViewMode mode)
		{
			if (puppet != null && puppet.TryGetComponent<PuppetPersonViewModeSwitcher>(out var component))
			{
				component.SwitchMode(mode);
			}
		}

		public void OnCollisionEnter(Collision collision)
		{
			if (collision == null || collision.gameObject == null || !AmIAvatar())
			{
				return;
			}
			ProtoActor componentInParent = collision.gameObject.GetComponentInParent<ProtoActor>();
			if (!(componentInParent != null) || !(componentInParent == base.gameObject))
			{
				Vector3 force = lastCharacterControlerHorizontalVelocity * 2f + Vector3.up * 0.5f;
				Rigidbody[] componentsInChildren = collision.gameObject.GetComponentsInChildren<Rigidbody>();
				for (int i = 0; i < componentsInChildren.Length; i++)
				{
					componentsInChildren[i].AddForce(force, ForceMode.VelocityChange);
				}
			}
		}

		public void StartPauseTransformSync()
		{
			pauseTransformSync = true;
		}

		public void StopPauseTransformSync()
		{
			pauseTransformSync = false;
			oldPos = base.transform.position;
		}

		public void TurnOnMaterialDissolve()
		{
			SkinnedMeshRenderer[] array = rdrs;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (!(skinnedMeshRenderer == null) && !(skinnedMeshRenderer.material == null) && !(skinnedMeshRenderer.material.shader == null) && skinnedMeshRenderer.material.shader.keywordSpace.FindKeyword("_DISSOLVESWITCH_ON").isValid)
				{
					skinnedMeshRenderer.material.EnableKeyword("_DISSOLVESWITCH_ON");
				}
			}
		}

		public void TurnOffMaterialDissolve()
		{
			SkinnedMeshRenderer[] array = rdrs;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (!(skinnedMeshRenderer == null) && !(skinnedMeshRenderer.material == null) && !(skinnedMeshRenderer.material.shader == null) && skinnedMeshRenderer.material.shader.keywordSpace.FindKeyword("_DISSOLVESWITCH_ON").isValid)
				{
					skinnedMeshRenderer.material.DisableKeyword("_DISSOLVESWITCH_ON");
				}
			}
		}

		public void TurnOnMaterialPaint(Color paintColor)
		{
			foreach (Coroutine turnOffPaintRunner in TurnOffPaintRunners)
			{
				StopCoroutine(turnOffPaintRunner);
			}
			TurnOffPaintRunners.Clear();
			SkinnedMeshRenderer[] array = rdrs;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (!(skinnedMeshRenderer == null) && !(skinnedMeshRenderer.material == null) && !(skinnedMeshRenderer.material.shader == null) && !(skinnedMeshRenderer.GetComponentInParent<LootingLevelObject>() != null))
				{
					if (skinnedMeshRenderer.material.shader.keywordSpace.FindKeyword("_PAINTSWITCH_ON").isValid)
					{
						skinnedMeshRenderer.material.EnableKeyword("_PAINTSWITCH_ON");
					}
					skinnedMeshRenderer.material.SetColor("_PaintColor", paintColor);
					skinnedMeshRenderer.material.SetFloat("_internal_PaintOpacityMul", 1f);
				}
			}
		}

		public void TurnOffMaterialPaint(float vanishingAnimationDuration)
		{
			SkinnedMeshRenderer[] array = rdrs;
			foreach (SkinnedMeshRenderer skinnedMeshRenderer in array)
			{
				if (!(skinnedMeshRenderer == null) && !(skinnedMeshRenderer.material == null) && !(skinnedMeshRenderer.material.shader == null))
				{
					TurnOffPaintRunners.Add(StartCoroutine(CorTurnOffMaterialPaint(skinnedMeshRenderer, vanishingAnimationDuration)));
				}
			}
		}

		private IEnumerator CorTurnOffMaterialPaint(SkinnedMeshRenderer rdr, float vanishingAnimationDuration)
		{
			if (rdr == null || rdr.material == null || rdr.material.shader == null)
			{
				yield break;
			}
			float elapsedTime = 0f;
			while (elapsedTime < vanishingAnimationDuration)
			{
				if (rdr == null || rdr.material == null || rdr.material.shader == null)
				{
					yield break;
				}
				elapsedTime += Time.deltaTime;
				float t = elapsedTime / vanishingAnimationDuration;
				float value = Mathf.Lerp(1f, 0f, t);
				rdr.material.SetFloat("_internal_PaintOpacityMul", value);
				yield return null;
			}
			if (!(rdr == null) && !(rdr.material == null) && !(rdr.material.shader == null) && rdr.material.shader.keywordSpace.FindKeyword("_PAINTSWITCH_ON").isValid)
			{
				rdr.material.DisableKeyword("_PAINTSWITCH_ON");
			}
		}

		public void SetHurtBox(Hurtbox hurtboxComp)
		{
			if (hurtboxComp == null)
			{
				hurtbox = puppet.GetHurtbox();
			}
			else
			{
				hurtbox = hurtboxComp;
			}
		}

		public void UseMimicSelectedColor(bool isOn)
		{
			if (this == null || puppet == null)
			{
				return;
			}
			SkinnedMeshRenderer componentInChildren = GetComponentInChildren<SkinnedMeshRenderer>();
			if (componentInChildren == null || componentInChildren.material == null || componentInChildren.material.shader == null)
			{
				return;
			}
			LocalKeyword localKeyword = componentInChildren.material.shader.keywordSpace.FindKeyword("_INTERNAL_USESELECTEDCOLOR_ON");
			if (localKeyword.isValid)
			{
				if (isOn)
				{
					componentInChildren.material.EnableKeyword(localKeyword.name);
				}
				else
				{
					componentInChildren.material.DisableKeyword(localKeyword.name);
				}
			}
		}

		private void TurnOffActorCollider()
		{
			CapsuleCollider component = GetComponent<CapsuleCollider>();
			if (component != null)
			{
				component.enabled = false;
			}
		}

		public void PlayFirstSpawnMotion()
		{
			if (puppet != null)
			{
				puppet.PlayFirstSpawnMotion();
			}
		}

		internal void OnBlackOut(bool blackout = true)
		{
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene != null && gamePlayScene.IsAvatarIndoor)
			{
				if (blackout)
				{
					Hub.s.audioman.PlaySfx(audioKeyWhenBlackOut, base.transform);
				}
				else
				{
					Hub.s.audioman.PlaySfx(audioKeyWhenRecoverFromBlackOut, base.transform);
				}
			}
		}

		internal void OnBlackOutByItem(int triggerActorID)
		{
			if (triggerActorID == ActorID)
			{
				Hub.s.audioman.PlaySfx(audioKeyWhenBlackOutByOwnedItem, base.transform);
				if (handheldItem != null && handheldItem.TryGetComponent<VoodooDoll>(out VoodooDoll component))
				{
					component.OnBlackOut();
				}
			}
		}

		public void OnActorDeathOnReconnect()
		{
			ActorDeathInfo actorDeathInfo = new ActorDeathInfo
			{
				DeadActorID = ActorID,
				ReasonOfDeath = ReasonOfDeath.None,
				AttackerActorID = 0,
				LinkedMasterID = 0
			};
			OnActorDeath(in actorDeathInfo);
		}

		public void OnActorDeath(in ActorDeathInfo actorDeathInfo)
		{
			if (!(this == null) && !(main == null))
			{
				if (IsMimic())
				{
					StopVoiceOnActor();
				}
				dead = true;
				reasonOfDeath = actorDeathInfo.ReasonOfDeath;
				deadTime = Time.time;
				voiceEffecter.Uninitialize();
				grabHelper.CancelGrab();
				if (ActorType == ActorType.Monster)
				{
					StopHummingSound();
				}
				cc.enabled = false;
				TurnOffActorCollider();
				puppet.Die(actorDeathInfo.ReasonOfDeath);
				ForceTurnOffFlashLightAndMountedLight();
				main.OnPlayerDeath(this, in actorDeathInfo);
				if (reasonOfDeath == ReasonOfDeath.Fall)
				{
					PlaySkillHitEffect(Hub.s.dataman.ExcelDataManager.Consts.C_FallDamageSkillTargetEffectId, playAnimation: false);
				}
			}
		}

		public void HideActor()
		{
			puppet.gameObject.SetActive(value: false);
		}

		public void ShowActor()
		{
			puppet.gameObject.SetActive(value: true);
		}

		public void TryUpdateVoicePitchToVoiceEffect()
		{
			if (netSyncActorData != null)
			{
				voiceEffecter.SetPitchStat(netSyncActorData.voicePitch);
			}
		}

		public void SetVoiceEffect(VoiceEffecter.VoiceEffectType type, bool isEnabled)
		{
			voiceEffecter.SetVoiceEffect(type, isEnabled);
		}

		public IndoorOutdoorDetector? GetIndoorOutdoorDetector()
		{
			return puppet?.GetComponent<IndoorOutdoorDetector>();
		}

		public bool IsHummingSoundPlaying()
		{
			if (currentHummingSoundResult?.ActingVariation != null && !currentHummingSoundResult.ActingVariation.IsPlaying)
			{
				currentHummingSoundResult = null;
				return false;
			}
			if (currentHummingSoundResult?.ActingVariation != null)
			{
				return currentHummingSoundResult.ActingVariation.IsPlaying;
			}
			return false;
		}

		public bool TryPlayHummingSound(string sfxId)
		{
			if (IsHummingSoundPlaying())
			{
				return false;
			}
			if (Hub.s?.audioman != null)
			{
				currentHummingSoundResult = Hub.s.audioman.PlaySfxTransform(sfxId, SfxRoot);
				return currentHummingSoundResult != null;
			}
			return false;
		}

		public void StopHummingSound()
		{
			if (IsHummingSoundPlaying())
			{
				currentHummingSoundResult?.ActingVariation?.Stop();
			}
			currentHummingSoundResult = null;
		}

		public void ClearEffectPlayer()
		{
			effectPlayer.Clear();
		}

		public void PlayAbnormalEffect(long syncID, int abnormalMasterID)
		{
			effectPlayer.PlayAbnormalParticle(syncID, abnormalMasterID);
			effectPlayer.PlayAbnormalDecaling(syncID, abnormalMasterID);
			effectPlayer.ApplyAbnormalMaterial(syncID, abnormalMasterID);
			effectPlayer.PlayAbnormalSoundEffect(syncID, abnormalMasterID);
			if (AmIAvatar())
			{
				effectPlayer.PlayAbnormalScreenEffect(syncID, abnormalMasterID);
			}
		}

		public void StopAbnormalEffect(long syncID, int abnormalMasterID)
		{
			effectPlayer.StopParticle(syncID);
			effectPlayer.StopAbnormalDecaling(syncID);
			effectPlayer.RestoreAbnormalMaterial(syncID, abnormalMasterID);
			effectPlayer.TryStopAbnormalSoundEffect(syncID, abnormalMasterID);
			if (AmIAvatar())
			{
				effectPlayer.StopScreenEffect(syncID);
			}
		}

		public void UpdateAbnormalEffect(long syncID, int abnormalMasterID)
		{
			StopAbnormalEffect(syncID, abnormalMasterID);
			PlayAbnormalEffect(syncID, abnormalMasterID);
		}

		public void RefreshAnimationByAbnormal(List<AbnormalObjectInfo> added, List<AbnormalObjectInfo> removed, List<AbnormalObjectInfo> changed)
		{
			effectPlayer.UpdateAnimationByAbnormal(added, removed, changed);
		}

		public void RefreshImmobileByAbnormalCC(List<AbnormalCCInfo> added, List<AbnormalCCInfo> removed, List<AbnormalCCInfo> changed)
		{
			effectPlayer.UpdateImmobilizeByAbnormalCC(added, removed, changed);
		}

		public void PlaySkillHitEffect(int skillTargetEffectId, bool playAnimation = true)
		{
			effectPlayer.PlayParticle(skillTargetEffectId);
			if (AmIAvatar())
			{
				effectPlayer.PlayScreenEffect(skillTargetEffectId);
			}
			if (Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId) != null)
			{
				effectPlayer.PlaySoundOneShot(skillTargetEffectId);
			}
			if (playAnimation)
			{
				TryHitAnimation(skillTargetEffectId);
			}
		}

		private void TryHitAnimation(int skillTargetEffectId)
		{
			if (skillTargetEffectId != 0)
			{
				SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
				if ((skillTargetEffectDataInfo == null || skillTargetEffectDataInfo.animation.Length > 0) && !IsAnimByAbnormal)
				{
					effectPlayer.PlaySkillHitAnim(skillTargetEffectId);
				}
			}
		}

		public void PlaySkillHitEffectSync(long syncId, int skillTargetEffectId)
		{
			effectPlayer.PlayParticle(syncId, skillTargetEffectId);
			if (AmIAvatar())
			{
				effectPlayer.PlayScreenEffect(syncId, skillTargetEffectId);
			}
			SkillTargetEffectDataInfo skillTargetEffectDataInfo = Hub.s.dataman.ExcelDataManager.GetSkillTargetEffectDataInfo(skillTargetEffectId);
			if (skillTargetEffectDataInfo != null)
			{
				if (skillTargetEffectDataInfo.StopSoundOnAbnormalEnd)
				{
					effectPlayer.PlaySoundLooping(syncId, skillTargetEffectId);
				}
				else
				{
					effectPlayer.PlaySoundOneShot(skillTargetEffectId);
				}
			}
			effectPlayer.PlaySkillHitAnim(skillTargetEffectId);
		}

		public void StopSkillHitEffect(int syncId)
		{
			effectPlayer.StopParticle(syncId);
			if (AmIAvatar())
			{
				effectPlayer.StopScreenEffect(syncId);
				effectPlayer.TryStopSoundEffect(syncId);
			}
		}

		public void ApplyImmobilize(TargetHitInfo skillTargetHitInfo)
		{
			effectPlayer.UpdateImmobilizeAnimByHit(skillTargetHitInfo);
		}

		public void OnEquipChanged(long prevItemId, long prevItemMasterId, long itemId, long itemMasterId, bool prevItemIsInInventory)
		{
			Hub.s.pdata.main.CheckFieldSkillSignEffectAtProtoActor(this, prevItemId, prevItemMasterId, itemId, itemMasterId, prevItemIsInInventory);
		}

		public void SetVoiceTypeDebugText(SpeechEvent ev)
		{
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene != null && gamePlayScene.DlHelper.ShowVoiceType)
			{
				debugOverlayText.gameObject.SetActive(value: true);
				debugOverlayText.text = ev.ToStringSimple();
			}
		}

		public float GetIncomingEventExpireTime(SpeechEvent_IncomingType etype = SpeechEvent_IncomingType.User)
		{
			if (ActorType == ActorType.Monster)
			{
				MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(monsterMasterID);
				if (monsterInfo != null && monsterInfo.IsMimic())
				{
					return _incomingEventExpireTimeForMimic;
				}
			}
			return etype switch
			{
				SpeechEvent_IncomingType.ChargeCompleted => 2f, 
				SpeechEvent_IncomingType.InvisibleMine => 5f, 
				SpeechEvent_IncomingType.User => 10f, 
				_ => _incomingEventExpireTime, 
			};
		}

		public void CheckIndoorEntered(Vector3 pos)
		{
			if (Hub.s.dLAcademyManager.GetAreaForDL(pos, out var _) == 0 && !_indoorEntered)
			{
				_indoorEntered = true;
			}
		}

		private void UpdateDL()
		{
			if (dead)
			{
				return;
			}
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			if (!(_lastUpdate + _updateInterval < realtimeSinceStartup))
			{
				return;
			}
			_lastUpdate = realtimeSinceStartup;
			if (ActorType == ActorType.Player)
			{
				if (controlMode == ControlMode.Manual)
				{
					UpdateSpeechRelatedData();
				}
			}
			else if (ActorType == ActorType.Monster && Hub.s.pdata.ClientMode == NetworkClientMode.Host)
			{
				MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(monsterMasterID);
				if (monsterInfo != null && monsterInfo.IsMimic())
				{
					UpdateSpeechRelatedData();
				}
			}
		}

		private void UpdateSpeechRelatedData()
		{
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene == null)
			{
				return;
			}
			ClearExpiredIncomingEvent();
			bool isUserIncoming = CheckIncomingPlayer(gamePlayScene, ActorType.Player, 10f);
			isUserIncoming = CheckMonster(gamePlayScene, isUserIncoming);
			GatherLevelObject(gamePlayScene);
			CheckIncomeLevelObject();
			if (gamePlayScene.IsWeatherForcastUIActive() && !_incomingEvent.Any((IncomingEvent x) => x.EventType == SpeechEvent_IncomingType.SquallWarning))
			{
				AddIncomingEvent(SpeechEvent_IncomingType.SquallWarning, Time.realtimeSinceStartup + GetIncomingEventExpireTime());
			}
			Tile? tile = gamePlayScene.FindCurrentTile(base.transform.position);
			if ((object)tile != null && tile.AllDoorways.Count == 1 && !_incomingEvent.Any((IncomingEvent x) => x.EventType == SpeechEvent_IncomingType.ClosedRoom))
			{
				AddIncomingEvent(SpeechEvent_IncomingType.ClosedRoom, Time.realtimeSinceStartup + GetIncomingEventExpireTime());
			}
			CheckFieldSkillObject(gamePlayScene);
			foreach (var item2 in _tempCurrentGrabSkills.Except<(int, int)>(_grabSkills).ToList())
			{
				int item = item2.Item2;
				AddIncomingEvent(SpeechEvent_IncomingType.GrabSkill, Time.realtimeSinceStartup + GetIncomingEventExpireTime(), item);
			}
			_grabSkills = _tempCurrentGrabSkills;
		}

		private List<T> GetLevelObjectList<T>(SpeechEvent_IncomingType inEventType) where T : LevelObject
		{
			if (!_tempLevelObjectsByType.ContainsKey(inEventType))
			{
				_tempLevelObjectsByType[inEventType] = new List<T>();
			}
			return (List<T>)_tempLevelObjectsByType[inEventType];
		}

		private void ClearExpiredIncomingEvent()
		{
			_incomingEvent.RemoveAll((IncomingEvent e) => Time.realtimeSinceStartup >= e.EventExpireTime);
		}

		private bool CheckIncomingPlayer(GamePlayScene inMainScene, ActorType inActorType, float inDistanceMeters)
		{
			bool result = false;
			List<ProtoActor> currentAdjacentPlayers = inMainScene.GetAllActorsInRange(base.transform.position, inDistanceMeters, ActorType.Player);
			currentAdjacentPlayers.Remove(this);
			List<ProtoActor> visibleActors = GetVisibleActors(currentAdjacentPlayers);
			float realtimeSinceStartup = Time.realtimeSinceStartup;
			List<ProtoActor> list = new List<ProtoActor>();
			foreach (ProtoActor item in visibleActors)
			{
				if (!(item == null))
				{
					if (!_lastSeenPlayerTime.TryGetValue(item, out var value))
					{
						list.Add(item);
					}
					else if (realtimeSinceStartup - value >= _incomingPlayerReacquireSeconds)
					{
						list.Add(item);
					}
					_lastSeenPlayerTime[item] = realtimeSinceStartup;
				}
			}
			if (list.Count > 0)
			{
				AddIncomingEvent(SpeechEvent_IncomingType.User, realtimeSinceStartup + GetIncomingEventExpireTime());
				result = true;
			}
			_visibleAdjacentPlayers.UnionWith(list);
			_visibleAdjacentPlayers.RemoveWhere((ProtoActor m) => !currentAdjacentPlayers.Contains(m));
			_adjacentPlayers.Clear();
			_adjacentPlayers.UnionWith(currentAdjacentPlayers);
			if (inActorType == ActorType.Player)
			{
				_tempCurrentGrabSkills.Clear();
				foreach (ProtoActor item2 in currentAdjacentPlayers)
				{
					if (!(item2 == null) && item2.grabHelper != null && item2.grabHelper.target != null && item2.grabHelper.grabMasterID != 0)
					{
						_tempCurrentGrabSkills.Add((item2.ActorID, item2.grabHelper.grabMasterID));
					}
				}
				HashSet<int> hashSet = new HashSet<int>();
				foreach (ProtoActor item3 in currentAdjacentPlayers)
				{
					if (!(item3 == this))
					{
						InventoryItem inventoryItem = item3.handheldItem;
						if (inventoryItem != null)
						{
							hashSet.Add(inventoryItem.ItemMasterID);
						}
					}
				}
				foreach (int item4 in hashSet.Where((int x) => !_adjacentPlayersHandHeldItems.Contains(x)))
				{
					if (item4 == 5110 || item4 == 5111 || item4 == 5124 || item4 == 5133)
					{
						AddIncomingEvent(SpeechEvent_IncomingType.HandHeldItem, Time.realtimeSinceStartup + GetIncomingEventExpireTime(), item4);
					}
				}
				_adjacentPlayersHandHeldItems.Clear();
				_adjacentPlayersHandHeldItems.UnionWith(hashSet);
			}
			if (CheckFastBeepingPlayer(currentAdjacentPlayers))
			{
				AddIncomingEvent(SpeechEvent_IncomingType.TimeBombWarning, Time.realtimeSinceStartup + GetIncomingEventExpireTime());
			}
			return result;
		}

		private bool CheckFastBeepingPlayer(List<ProtoActor> inPlayerList)
		{
			if (inPlayerList.Where((ProtoActor actor) => actor.IsHoldingTimeBombFastBeeping()).ToList().Count > 0 && _incomingEvent.Where((IncomingEvent x) => x.EventType == SpeechEvent_IncomingType.TimeBombWarning).ToList().Count() == 0)
			{
				return true;
			}
			return false;
		}

		private bool CheckMonster(GamePlayScene inMainScene, bool isUserIncoming)
		{
			List<ProtoActor> currentMonsterActors = inMainScene.GetAllActorsInRange(base.transform.position, 10f, ActorType.Monster);
			List<ProtoActor> candidates = currentMonsterActors.Where((ProtoActor x) => !_visibleMonsters.Contains(x)).ToList();
			List<ProtoActor> visibleActors = GetVisibleActors(candidates, facing: false);
			foreach (ProtoActor item in visibleActors)
			{
				if (item.IsMimic())
				{
					if (!isUserIncoming)
					{
						AddIncomingEvent(SpeechEvent_IncomingType.User, Time.realtimeSinceStartup + GetIncomingEventExpireTime());
						isUserIncoming = true;
					}
					Hub.s.voiceman?.TrySendToServerVoiceEmotion(ActorID, ReplaySharedData.E_EVENT.INCOMING_MIMIC);
				}
				else
				{
					AddIncomingEvent(SpeechEvent_IncomingType.Monster, Time.realtimeSinceStartup + GetIncomingEventExpireTime(), item.monsterMasterID);
				}
			}
			_visibleMonsters.UnionWith(visibleActors);
			_visibleMonsters.RemoveWhere((ProtoActor m) => !currentMonsterActors.Contains(m));
			_monsters.Clear();
			_monsters.UnionWith(currentMonsterActors);
			foreach (ProtoActor item2 in currentMonsterActors)
			{
				if (!(item2 == null) && item2.grabHelper != null && item2.grabHelper.target != null && item2.grabHelper.grabMasterID != 0 && item2.grabHelper.target != this)
				{
					_tempCurrentGrabSkills.Add((item2.ActorID, item2.grabHelper.grabMasterID));
				}
			}
			return isUserIncoming;
		}

		private void GatherLevelObject(GamePlayScene inMainScene)
		{
			float yThreshold = Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold;
			foreach (IList value in _tempLevelObjectsByType.Values)
			{
				value.Clear();
			}
			foreach (LevelObject item2 in inMainScene.CollectLevelObjects())
			{
				if (item2 == null)
				{
					continue;
				}
				SpeechType_Area areaType2;
				if (item2 is LootingLevelObject item && IsNearBy(item2.transform.position))
				{
					GetLevelObjectList<LootingLevelObject>(SpeechEvent_IncomingType.ScrapObject).Add(item);
				}
				else if (item2 is CommonChargerLevelObject commonChargerLevelObject && IsNearBy(commonChargerLevelObject.transform.position, yThreshold, 5f))
				{
					if (Hub.s.dLAcademyManager.GetAreaForDL(commonChargerLevelObject.transform.position, out var _) == 0)
					{
						GetLevelObjectList<CommonChargerLevelObject>(SpeechEvent_IncomingType.Charger).Add(commonChargerLevelObject);
					}
				}
				else if (item2 is TeleporterLevelObject teleporterLevelObject && IsNearBy(teleporterLevelObject.transform.position))
				{
					GetLevelObjectList<TeleporterLevelObject>(SpeechEvent_IncomingType.Teleporter).Add(teleporterLevelObject);
				}
				else if (item2 is CrowShopLevelObject crowShopLevelObject && IsNearBy(crowShopLevelObject.transform.position))
				{
					GetLevelObjectList<CrowShopLevelObject>(SpeechEvent_IncomingType.CrowShop).Add(crowShopLevelObject);
				}
				else if (item2 is MomentarySwitchLevelObject momentarySwitchLevelObject && IsNearBy(momentarySwitchLevelObject.transform.position, yThreshold, 5f))
				{
					GetLevelObjectList<MomentarySwitchLevelObject>(SpeechEvent_IncomingType.CorridorSwitches).Add(momentarySwitchLevelObject);
				}
				else if (item2 is TrapLevelObject { TrapType: TrapType.Mine_Invisible } trapLevelObject)
				{
					if (_alarmBounds == null)
					{
						_alarmBounds = trapLevelObject.transform.parent.GetComponent<AlarmBounds>();
					}
					if (_alarmBounds != null)
					{
						AlarmRadiusData alarmRadiusData = _alarmBounds.AlarmRadiusDataList.LastOrDefault();
						if (0f < alarmRadiusData.radius && IsNearBy(trapLevelObject.transform.position, yThreshold, alarmRadiusData.radius))
						{
							GetLevelObjectList<TrapLevelObject>(SpeechEvent_IncomingType.InvisibleMine).Add(trapLevelObject);
						}
					}
				}
				else if (item2 is ScrapScanLevelObject scrapScanLevelObject && IsNearBy(scrapScanLevelObject.transform.position, yThreshold, 3f) && Hub.s.dLAcademyManager.GetAreaForDL(base.transform.position, out areaType2) == 1 && Hub.s.dLAcademyManager.GetOutdoorArea(base.transform.position) == DLDecisionAgent.OutdoorArea.TramInside)
				{
					GetLevelObjectList<ScrapScanLevelObject>(SpeechEvent_IncomingType.LookAtScrapScanner).Add(scrapScanLevelObject);
				}
			}
		}

		private void CheckIncomeLevelObject()
		{
			CheckIncomingEvent_LevelObject(SpeechEvent_IncomingType.ScrapObject, _scrapObjects, GetLevelObjectList<LootingLevelObject>(SpeechEvent_IncomingType.ScrapObject));
			CheckIncomingEvent_LevelObject(SpeechEvent_IncomingType.Charger, _chargers, GetLevelObjectList<CommonChargerLevelObject>(SpeechEvent_IncomingType.Charger));
			CheckIncomingEvent_LevelObject(SpeechEvent_IncomingType.Teleporter, _teleporters, GetLevelObjectList<TeleporterLevelObject>(SpeechEvent_IncomingType.Teleporter));
			CheckIncomingEvent_LevelObject(SpeechEvent_IncomingType.CrowShop, _crowShops, GetLevelObjectList<CrowShopLevelObject>(SpeechEvent_IncomingType.CrowShop));
			CheckIncomingEvent_LevelObject(SpeechEvent_IncomingType.CorridorSwitches, _corridorSwitches, GetLevelObjectList<MomentarySwitchLevelObject>(SpeechEvent_IncomingType.CorridorSwitches));
			CheckIncomingEvent_LevelObject(SpeechEvent_IncomingType.InvisibleMine, _invisibleMines, GetLevelObjectList<TrapLevelObject>(SpeechEvent_IncomingType.InvisibleMine));
			CheckIncomingEvent_LevelObject(SpeechEvent_IncomingType.LookAtScrapScanner, _scrapScanners, GetLevelObjectList<ScrapScanLevelObject>(SpeechEvent_IncomingType.LookAtScrapScanner));
		}

		private void CheckIncomingEvent_LevelObject<T>(SpeechEvent_IncomingType eventType, HashSet<T> inHashSet, List<T> inList) where T : LevelObject
		{
			if (inHashSet == null || inList == null)
			{
				Debug.LogError("AddLevelObject : inHashSet or inList is null");
				return;
			}
			for (int i = 0; i < inList.Count; i++)
			{
				if (!(inList[i] == null) && !inHashSet.Contains(inList[i]))
				{
					AddIncomingEventForce_LevelObject(eventType, inList[i]);
				}
			}
			inHashSet.Clear();
			inHashSet.UnionWith(inList);
		}

		private void CheckFieldSkillObject(GamePlayScene mainScene)
		{
			List<FieldSkillObjectInfo> inList = (from x in mainScene.GetSprinklerFieldSkillInRange(base.transform.position, 10f)
				where x != null && !_sprinklers.Contains(x)
				select x).ToList();
			CheckIncomingEvent_FieldSkillObject(SpeechEvent_IncomingType.SprinklerActivated, _sprinklers, inList);
			List<FieldSkillObjectInfo> inList2 = (from x in mainScene.GetPaintballFieldSkillInRange(base.transform.position, 10f)
				where x != null && !_paintballs.Contains(x)
				select x).ToList();
			CheckIncomingEvent_FieldSkillObject(SpeechEvent_IncomingType.Paintball, _paintballs, inList2);
			List<FieldSkillObjectInfo> inList3 = (from x in mainScene.GetPaintspotFieldSkillInRange(base.transform.position, 20f)
				where x != null && !_paintspots.Contains(x)
				select x).ToList();
			CheckIncomingEvent_FieldSkillObject(SpeechEvent_IncomingType.Paintspot, _paintspots, inList3);
			List<FieldSkillObjectInfo> inList4 = (from x in mainScene.GetHeliumGasFieldSkillInRange(base.transform.position, 10f)
				where x != null && !_heliumGasPumps.Contains(x)
				select x).ToList();
			CheckIncomingEvent_FieldSkillObject(SpeechEvent_IncomingType.HeliumGasActivated, _heliumGasPumps, inList4);
			List<FieldSkillObjectInfo> inList5 = (from x in mainScene.GetLightningFieldSkillInRange(base.transform.position, 10f)
				where x != null && !_lightnings.Contains(x)
				select x).ToList();
			CheckIncomingEvent_FieldSkillObject(SpeechEvent_IncomingType.Lightning, _lightnings, inList5);
		}

		private void CheckIncomingEvent_FieldSkillObject(SpeechEvent_IncomingType eventType, HashSet<FieldSkillObjectInfo> inHashSet, List<FieldSkillObjectInfo> inList)
		{
			if (inHashSet == null || inList == null)
			{
				Debug.LogError("AddLevelObject : inHashSet or inList is null");
			}
			else if (0 < inList.Count)
			{
				AddIncomingEvent(eventType, Time.realtimeSinceStartup + GetIncomingEventExpireTime());
				inHashSet.Clear();
				inHashSet.UnionWith(inList);
			}
		}

		private void SwapAdjacentPlayersList(List<ProtoActor> newList)
		{
			_adjacentPlayers.Clear();
			_adjacentPlayers.UnionWith(newList);
		}

		private List<ProtoActor> GetVisibleActors(List<ProtoActor> candidates, bool facing = true)
		{
			List<ProtoActor> list = new List<ProtoActor>();
			_ = 60f / 2f;
			foreach (ProtoActor candidate in candidates)
			{
				if (IsVisibleActor(candidate, facing))
				{
					list.Add(candidate);
				}
			}
			return list;
		}

		private bool IsVisibleActor(ProtoActor inActor, bool facing = true)
		{
			float num = 60f / 2f;
			if (!facing)
			{
				num = 360f;
			}
			Vector3 position = base.transform.position;
			if (inActor == this)
			{
				return false;
			}
			Vector3 position2 = inActor.transform.position;
			Vector3 normalized = (position2 - position).normalized;
			if (Vector3.Angle(base.transform.forward, normalized) <= num)
			{
				position.y += 1.5f;
				position2.y += 1.5f;
				float maxDistance = Vector3.Distance(position, position2);
				if (!Physics.Raycast(position, normalized, out var hitInfo, maxDistance, Hub.DefaultAndActorLayerMask))
				{
					return true;
				}
				if (hitInfo.transform == inActor.transform)
				{
					return true;
				}
			}
			return false;
		}

		private bool IsNearBy(Vector3 targetPosition, float yThreshold = 5f, float xzThreshold = 10f)
		{
			Vector3 vector = base.transform.position - targetPosition;
			if (Mathf.Abs(vector.y) > yThreshold)
			{
				return false;
			}
			if (vector.x * vector.x + vector.z * vector.z > xzThreshold * xzThreshold)
			{
				return false;
			}
			return true;
		}

		public SpeechType_AdjacentPlayerCount GetAdjacentPlayerCount()
		{
			int num = _adjacentPlayers.Count();
			if (num < 1)
			{
				return SpeechType_AdjacentPlayerCount.Monologue;
			}
			if (num == 1)
			{
				return SpeechType_AdjacentPlayerCount.Dialogue;
			}
			return SpeechType_AdjacentPlayerCount.Multilogue;
		}

		public SpeechType_Area GetAreaType(Vector3 pos)
		{
			if (Hub.s == null)
			{
				return SpeechType_Area.None;
			}
			if (Hub.s.dLAcademyManager == null)
			{
				return SpeechType_Area.None;
			}
			if (Hub.s.dLAcademyManager.GetAreaForDL(pos, out var areaType) == 0 && !_indoorEntered)
			{
				_indoorEntered = true;
			}
			if (CheckForMeGrabbByMonster())
			{
				return SpeechType_Area.GrabbedByMonster;
			}
			return areaType;
		}

		public SpeechType_FacingPlayerCount GetFacingPlayerCount()
		{
			float num = 60f / 2f;
			Vector3 position = base.transform.position;
			int num2 = 0;
			foreach (ProtoActor adjacentPlayer in _adjacentPlayers)
			{
				if (adjacentPlayer == this || adjacentPlayer == null)
				{
					continue;
				}
				Vector3 position2 = adjacentPlayer.transform.position;
				Vector3 normalized = (position2 - position).normalized;
				if (!(Vector3.Angle(base.transform.forward, normalized) <= num))
				{
					continue;
				}
				Vector3 normalized2 = (position - position2).normalized;
				if (!(Vector3.Angle(adjacentPlayer.transform.forward, normalized2) <= num))
				{
					continue;
				}
				position.y = 1.5f;
				position2.y = 1.5f;
				float maxDistance = Vector3.Distance(position, position2);
				if (Physics.Raycast(position, normalized, out var hitInfo, maxDistance, Hub.DefaultAndActorLayerMask))
				{
					if (hitInfo.transform == adjacentPlayer.transform)
					{
						num2++;
					}
				}
				else
				{
					num2++;
				}
			}
			return num2 switch
			{
				0 => SpeechType_FacingPlayerCount.None, 
				1 => SpeechType_FacingPlayerCount.One, 
				2 => SpeechType_FacingPlayerCount.Two, 
				_ => SpeechType_FacingPlayerCount.MoreThanTwo, 
			};
		}

		public List<int> GetScrapObjects()
		{
			return (from scrap in _scrapObjects
				where scrap != null
				select scrap.itemMasterID).ToList() ?? new List<int>();
		}

		public List<int> GetMonsters()
		{
			return _monsters.Select((ProtoActor monster) => monster.monsterMasterID).ToList();
		}

		public SpeechType_Teleporter GetTeleporter()
		{
			if (_teleporters.Count > 0)
			{
				return SpeechType_Teleporter.Yes;
			}
			return SpeechType_Teleporter.None;
		}

		public SpeechType_IndoorEntered GetIndoorEntered()
		{
			if (_indoorEntered)
			{
				return SpeechType_IndoorEntered.Yes;
			}
			return SpeechType_IndoorEntered.None;
		}

		public SpeechType_Charger GetCharger()
		{
			if (_chargers.Count > 0)
			{
				return SpeechType_Charger.Yes;
			}
			return SpeechType_Charger.None;
		}

		public List<IncomingEvent> GetIncomingEvents()
		{
			return _incomingEvent;
		}

		public SpeechType_CrowShop GetCrowShop()
		{
			if (_crowShops.Count > 0)
			{
				return SpeechType_CrowShop.Yes;
			}
			return SpeechType_CrowShop.None;
		}

		public void AddIncomingEvent(SpeechEvent_IncomingType eventType, float expireTime, int eventID = 0)
		{
			if (ActorType == ActorType.Player)
			{
				if (controlMode != ControlMode.Manual)
				{
					return;
				}
			}
			else if (ActorType == ActorType.Monster)
			{
				if (Hub.s.pdata.ClientMode != NetworkClientMode.Host)
				{
					return;
				}
				MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(monsterMasterID);
				if (monsterInfo != null && !monsterInfo.IsMimic())
				{
					return;
				}
			}
			_incomingEvent.Add(new IncomingEvent
			{
				EventExpireTime = expireTime,
				EventType = eventType,
				EventID = eventID
			});
		}

		public void AddIncomingEventDelayed(SpeechEvent_IncomingType eventType, float expireTime, float delaySeconds, int eventID = 0)
		{
			StartCoroutine(AddIncomingEventDelayedCoroutine(eventType, expireTime, delaySeconds, eventID));
		}

		private IEnumerator AddIncomingEventDelayedCoroutine(SpeechEvent_IncomingType eventType, float expireTime, float delaySeconds, int eventID)
		{
			yield return new WaitForSeconds(delaySeconds);
			AddIncomingEvent(eventType, expireTime, eventID);
		}

		public void AddIncomingEventForce_LevelObject<T>(SpeechEvent_IncomingType eventType, T inLevelObject) where T : LevelObject
		{
			float eventExpireTime = Time.realtimeSinceStartup + GetIncomingEventExpireTime(eventType);
			int eventID = 0;
			if (eventType == SpeechEvent_IncomingType.ScrapObject && inLevelObject is LootingLevelObject lootingLevelObject)
			{
				eventID = lootingLevelObject.itemMasterID;
			}
			_incomingEvent.Add(new IncomingEvent
			{
				EventExpireTime = eventExpireTime,
				EventType = eventType,
				EventID = eventID
			});
		}

		public bool CheckForUserWithHeliumGasNearby()
		{
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene == null)
			{
				return false;
			}
			List<ProtoActor> allActorsInRange = gamePlayScene.GetAllActorsInRange(base.transform.position, 10f, ActorType.Player);
			allActorsInRange.Remove(this);
			foreach (ProtoActor item in allActorsInRange)
			{
				if (item == null || item.abnormalHelper.AbnormalInfos == null || item.abnormalHelper.AbnormalInfos.Count <= 0)
				{
					continue;
				}
				foreach (AbnormalObjectInfo value in item.abnormalHelper.AbnormalInfos.Values)
				{
					if (value != null && value.abnormalMasterID == 72001)
					{
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckForMeGrabbByMonster()
		{
			GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
			if (gamePlayScene == null)
			{
				return false;
			}
			foreach (ProtoActor item in gamePlayScene.GetAllActorsInRange(base.transform.position, 10f, ActorType.Monster))
			{
				if (!(item == null) && !item.IsMimic() && item.grabHelper != null && item.grabHelper.target == this)
				{
					return true;
				}
			}
			return false;
		}

		private void SetGrabbedBy(Transform? grabSocket)
		{
			if (base.destroyCancellationToken.IsCancellationRequested)
			{
				return;
			}
			bool flag = grabSocket != null;
			if (IsInputDisabledBy(EInputDisableReason.GrabbedByOther) != flag)
			{
				if (!dead)
				{
					cc.enabled = !flag;
				}
				else
				{
					cc.enabled = false;
				}
				pauseTransformSync = flag;
				if (puppet.TryGetComponent<FootstepAudioPlayer>(out var component))
				{
					component.enabled = !flag;
				}
				if (grabSocket != null)
				{
					base.transform.SetParent(grabSocket, worldPositionStays: true);
				}
				else
				{
					Transform parent = ((main != null) ? main.GetActorSpawnRootTransform(base.transform.position) : null);
					base.transform.SetParent(parent, worldPositionStays: true);
				}
				ClearInputDisableReason(EInputDisableReason.GrabbedByOther);
				if (flag)
				{
					SetInputDisableReason(EInputDisableReason.GrabbedByOther);
				}
			}
		}

		private void SetGrabber(bool value)
		{
			puppet.SetGrabbing(value);
			isGrabbing = value;
		}

		[PacketHandler(false)]
		private void OnMoveStartSig(MoveStartSig sig)
		{
			if (this == null || sig == null || AmIAvatar())
			{
				return;
			}
			if (ActorType != ActorType.Player)
			{
				Logger.RLog("ResolvePacket_ChangeViewPointSig called for non-player ActorType", sendToLogServer: false, useConsoleOut: true, "movebug");
			}
			if (pauseTransformSync)
			{
				return;
			}
			if (controlMode != ControlMode.Remote)
			{
				Logger.RError("ResolvePacket_MoveStartSig : controlMode != eControlMode.RemoteControl");
			}
			else if (sig.actorID != ActorID)
			{
				Logger.RError("ResolvePacket_MoveStartSig : packet.actorID != actorID");
			}
			else
			{
				if (sig.futureTime == 0L)
				{
					return;
				}
				puppet.UpdateMoveType(sig.actorMoveType);
				Vector3 vector = ((ActorType != ActorType.Monster) ? sig.basePositionCurr.toVector3() : DropToPhysicsCollider(sig.basePositionCurr.toVector3()));
				Vector3 pos = vector;
				syncTargetRotY = sig.basePositionCurr.yaw;
				syncTargetCamRotX = sig.pitch;
				float num = (float)sig.futureTime / 1000f;
				if (num < 0.09f)
				{
					num = 0.09f;
				}
				float num2 = (sig.basePositionFuture.toVector3() - sig.basePositionCurr.toVector3()).magnitude / num;
				float num3 = ((sig.actorMoveType == ActorMoveType.Run) ? runSpeed : walkSpeed);
				float num4 = 0f;
				if (ActorType == ActorType.Monster)
				{
					if (debug_oldMoveCurr.HasValue && debug_oldMoveCurr == sig.basePositionCurr.toVector3())
					{
						return;
					}
					num4 = num3;
				}
				else
				{
					num4 = num2;
				}
				if (interpolationMethod != ProtoActorInterpolationType.deprecated_CurrAndFuture && interpolationMethod == ProtoActorInterpolationType.PathQueue)
				{
					moveInterpolationData.Add(new MoveInterpolationData
					{
						command = MoveInterpolationData.eCommand.StartMove,
						pos = pos,
						rotY = sig.basePositionCurr.yaw,
						camRotX = sig.pitch,
						velocity = num4
					});
				}
				debug_oldMoveCurr = sig.basePositionCurr.toVector3();
			}
		}

		[PacketHandler(false)]
		private void OnDebugInfoSig(DebugInfoSig sig)
		{
			if (this == null || sig == null)
			{
				return;
			}
			if (sig.actorID != ActorID)
			{
				Logger.RError($"[DebugInfoSig] ProtoActor. packet.actorID != ActorID. {sig.actorID} != {ActorID}");
				return;
			}
			if (!string.IsNullOrEmpty(sig.debugInfo))
			{
				ShowDebugText(sig.debugInfo);
			}
			Hub.s.UpdateHitCheckVisualizations(sig.actorID, sig.hitCheckDrawInfos);
		}

		[PacketHandler(false)]
		private void ResolvePacket_ChangeViewPointSig(ChangeViewPointSig sig)
		{
			if (!(this == null) && sig != null && !AmIAvatar())
			{
				if (ActorType != ActorType.Player)
				{
					Logger.RLog("ResolvePacket_ChangeViewPointSig called for non-player ActorType", sendToLogServer: false, useConsoleOut: true, "movebug");
				}
				if (controlMode != ControlMode.Remote)
				{
					Logger.RError("ResolvePacket_ChangeViewPointSig : controlMode != eControlMode.RemoteControl");
					return;
				}
				if (sig.actorID != ActorID)
				{
					Logger.RError("ResolvePacket_ChangeViewPointSig : packet.actorID != actorID");
					return;
				}
				syncTargetPosVForAnimation = Vector3.zero;
				syncTargetRotY = sig.angle;
				syncTargetCamRotX = sig.pitch;
			}
		}

		[PacketHandler(false)]
		private void OnMoveStopSig(MoveStopSig sig)
		{
			if (this == null || sig == null || AmIAvatar())
			{
				return;
			}
			if (ActorType != ActorType.Player)
			{
				Logger.RLog("OnMoveStopSig called for non-player ActorType", sendToLogServer: false, useConsoleOut: true, "movebug");
			}
			if (!pauseTransformSync)
			{
				if (controlMode != ControlMode.Remote)
				{
					Logger.RError("ResolvePacket_MoveStopSig : controlMode != eControlMode.RemoteControl");
					return;
				}
				if (sig.actorID != ActorID)
				{
					Logger.RError("ResolvePacket_MoveStopSig : packet.actorID != actorID");
					return;
				}
				debug_syncTargetPos = base.transform.position;
				syncTargetPosVForAnimation = Vector3.zero;
				syncTargetRotY = base.transform.rotation.eulerAngles.y;
				syncTargetCamRotX = camRoot.rotation.eulerAngles.x;
				Vector3 vector = ((ActorType != ActorType.Monster) ? sig.currentPos.toVector3() : DropToPhysicsCollider(sig.currentPos.toVector3()));
				Vector3 vector2 = vector;
				float velocity = walkSpeed;
				moveInterpolationData.Add(new MoveInterpolationData
				{
					command = MoveInterpolationData.eCommand.StopMove,
					pos = vector2,
					rotY = sig.currentPos.yaw,
					camRotX = syncTargetCamRotX.Value,
					velocity = velocity
				});
				debug_oldMoveCurr = vector2;
				Hub.DebugDraw_Line(base.transform.position, vector2, Color.yellow, 1f);
			}
		}

		[PacketHandler(false)]
		private void OnJumpSig(JumpSig sig)
		{
			if (!(this == null) && sig != null)
			{
				fakeJumper.OnJumpSig(sig);
			}
		}

		[PacketHandler(false)]
		private void OnCancelJumpSig(CancelJumpSig sig)
		{
			if (!(this == null) && sig != null)
			{
				fakeJumper.OnCancelJumpSig(sig);
			}
		}

		[PacketHandler(false)]
		private void OnEmoteSig(EmotionSig sig)
		{
			if (!(this == null) && sig != null)
			{
				if (AmIAvatar())
				{
					Logger.RError("내 이모트에 대한 sig 는 안와야 함");
					return;
				}
				Teleport(sig.basePosition.toVector3(), new Vector3(0f, sig.basePosition.yaw, 0f), ignorePitch: true);
				emotePlayer.StartEmote(sig.emotionMasterID);
			}
		}

		[PacketHandler(false)]
		private void OnCancelEmoteSig(CancelEmotionSig sig)
		{
			if (!(this == null) && sig != null)
			{
				emotePlayer.OnCancelEmoteSig(sig);
			}
		}

		private ActorDeathInfo ConvertActorDyingSigToActorDeathInfo(ActorDyingSig actorDyingSig)
		{
			return new ActorDeathInfo
			{
				DeadActorID = actorDyingSig.actorID,
				AttackerActorID = actorDyingSig.attackerActorID,
				LinkedMasterID = actorDyingSig.linkedMasterID,
				ReasonOfDeath = actorDyingSig.reasonOfDeath
			};
		}

		[PacketHandler(false)]
		private void ResolvePacket_ActorDyingSig(ActorDyingSig sig)
		{
			if (!(this == null) && sig != null)
			{
				OnActorDeath(ConvertActorDyingSigToActorDeathInfo(sig));
			}
		}

		[PacketHandler(false)]
		private void ResolvePacket_AnnounceImmutableStatSig(AnnounceImmutableStatSig announceImmutableStatSig)
		{
			ResolveImmutableStats(announceImmutableStatSig.ImmutableStats);
		}

		[PacketHandler(false)]
		private void OnPacket(AnnounceMutableStatSig announceMutableStatSig)
		{
			switch (announceMutableStatSig.type)
			{
			case MutableStatType.HP:
				UpdateHp(announceMutableStatSig.remainValue, netSyncActorData.maxHP);
				break;
			case MutableStatType.Conta:
				UpdateConta(announceMutableStatSig.remainValue, netSyncActorData.maxConta);
				break;
			case MutableStatType.Stamina:
				UpdateStamina(announceMutableStatSig.remainValue, netSyncActorData.maxStamina);
				break;
			}
		}

		[PacketHandler(false)]
		private void ResolvePacket_UseSkillSig(UseSkillSig sig)
		{
			puppet.UseSkill(sig.skillMasterID);
			skillHelper.OnNetUseSkillSig(sig);
			InventoryItem selectedItem = inventory.SelectedItem;
			if (selectedItem != null && selectedItem.TryCastMasterInfo<ItemEquipmentInfo>(out ItemEquipmentInfo masterInfo) && masterInfo != null)
			{
				if (masterInfo.ReloadSkillMasterID == sig.skillMasterID)
				{
					OnReload();
				}
				else
				{
					OnFire();
				}
			}
		}

		[PacketHandler(false)]
		private void ResolvePacket_HitTargetSig(HitTargetSig sig)
		{
			SkillSequenceInfo skillSequenceInfo = Hub.s.dataman.ExcelDataManager.GetSkillSequenceInfo(sig.skillSequenceMasterID);
			if (skillSequenceInfo == null)
			{
				Logger.RError("Error - skillSequenceMasterData not found OnNetHitTargetSig(HitTargetSig)");
				return;
			}
			ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(sig.actorID);
			if (actorByActorID == null)
			{
				Logger.RError("Error - sig.ownerActorID not found OnNetHitTargetSig(HitTargetSig)");
				return;
			}
			actorByActorID.skillHelper.currentCastingSkillSequenceMasterIdCached = sig.skillSequenceMasterID;
			foreach (TargetHitInfo targetHitInfo in sig.targetHitInfos)
			{
				ProtoActor actorByActorID2 = main.GetActorByActorID(targetHitInfo.targetID);
				if (!(actorByActorID2 == null))
				{
					if (skillSequenceInfo.SkillTagetEffectId != 0)
					{
						actorByActorID2.PlaySkillHitEffect(skillSequenceInfo.SkillTagetEffectId);
					}
					actorByActorID2.puppet.PlayHitFeedback();
					if (skillSequenceInfo.SequenceType != SkillSeqType.Abnormal && targetHitInfo.actionAbnormalHitType != CCType.None)
					{
						actorByActorID2.ApplyImmobilize(targetHitInfo);
					}
					if (skillSequenceInfo.RagDollForceDirection != Vector3.zero)
					{
						actorByActorID2.LastDamagedForceDirection = actorByActorID.transform.rotation * skillSequenceInfo.RagDollForceDirection;
					}
				}
			}
		}

		[PacketHandler(false)]
		private void ResolvePacket_CancelSkillSig(CancelSkillSig packet)
		{
			skillHelper.OnNetCancelSkillSig(packet);
		}

		[PacketHandler(false)]
		private void ResolvePacket_GroggyStateSig(GroggyStateSig sig)
		{
			if (sig.state != GroggyState.Normal)
			{
				_ = sig.state;
				_ = 1;
			}
		}

		[PacketHandler(false)]
		private void ResolvePacket_ReloadWeaponSig(ReloadWeaponSig sig)
		{
			inventory.ResolveInventoryInfos(sig.inventoryInfos, sig.currentInvenSlotIndex);
		}

		[PacketHandler(false)]
		private void OnUpdateInvenSig(UpdateInvenSig sig)
		{
			inventory.OnUpdateInvenSig(sig);
		}

		[PacketHandler(false)]
		private void OnChangeItemLooksSig(ChangeItemLooksSig sig)
		{
			inventory.OnChangeItemLooksSig(sig);
		}

		[PacketHandler(false)]
		private void OnStartUseScrapSig(StartScrapMotionSig sig)
		{
			inventory.OnStartScrapMotionSig(sig);
		}

		[PacketHandler(false)]
		private void OnEndScrapMotionSig(EndScrapMotionSig sig)
		{
			inventory.OnEndScrapMotionSig(sig);
		}

		[PacketHandler(false)]
		private void OnCancelScrapMotionSig(CancelScrapMotionSig sig)
		{
			inventory.OnCancelScrapMotionSig(sig);
		}

		[PacketHandler(false)]
		private void OnNet_CoolTimeSig(CooltimeSig sig)
		{
			if (!AmIAvatar())
			{
				return;
			}
			sig.skillCooltimeInfos.ForEach(delegate(SkillCooltimeInfo c)
			{
				ProtoActor actorByActorID = Hub.s.pdata.main.GetActorByActorID(sig.actorID);
				if (actorByActorID != null)
				{
					if (c.changeType == CooltimeChangeType.Remove)
					{
						actorByActorID.skillHelper.OnEndSkillCoolTime(c.skillMasterID);
					}
					else if (c.changeType == CooltimeChangeType.Add)
					{
						actorByActorID.skillHelper.OnStartSkillCoolTime(c.skillMasterID);
					}
				}
			});
		}

		[PacketHandler(false)]
		private void OnChangeAITargetSig(ChangeAITargetSig packet)
		{
		}

		[PacketHandler(false)]
		protected void OnPacket(ChangeEquipStatusSig sig)
		{
			if (!(this == null) && sig != null && !(main == null))
			{
				inventory.ResolveInventoryItemInfo(sig.changedItemInfo);
				main.UpdateInventoryUI(this);
				UpdateChangedItemInfo(sig.changedItemInfo);
			}
		}

		[PacketHandler(false)]
		protected void OnPacket(AbnormalSig sig)
		{
			abnormalHelper.OnNetAbnormalSig(sig);
		}

		[PacketHandler(false)]
		protected void OnPacket(AuraSig sig)
		{
			auraHelper.OnNetAuraSig(sig);
		}

		public void OnTeleportSig(TeleportSig sig)
		{
			Teleport(sig.pos.toVector3(), new Vector3(0f, sig.pos.yaw, 0f));
			OnTeleported(sig);
			if (Hub.s.pdata.main is GamePlayScene)
			{
				CheckIndoorEntered(sig.pos.toVector3());
			}
		}

		[PacketHandler(false)]
		protected void OnPacket(ChangeSprintModeSig sig)
		{
			_ = sig.isSprint;
		}

		[PacketHandler(false)]
		protected void OnPacket(SyncSkillMoveSig sig)
		{
			skillHelper.OnNetSyncSkillMoveSig(sig);
		}

		[PacketHandler(false)]
		protected void OnPacket(AttachActorSig sig)
		{
			grabHelper.OnAttachActorSig(sig);
		}

		[PacketHandler(false)]
		protected void OnPacket(DestroyItemSig sig)
		{
			if (handheldItem != null)
			{
				main.TryDestroyItem(base.transform, handheldItem.Transform, sig.destroyedItemInfo.itemMasterID);
			}
		}

		[PacketHandler(false)]
		protected void OnPacket(DebugBTStateInfoSig sig)
		{
			if (!(this == null) && sig != null)
			{
				debugBTStateInfo.btDataName = sig.aiDataName;
				debugBTStateInfo.btTemplateName = sig.templateName;
				UpdateDebugBTStateInfoText();
			}
		}

		[PacketHandler(false)]
		protected void OnPacket(DebugDLAgentInfoSig sig)
		{
			if (!(this == null) && sig != null)
			{
				if (sig.changedInfoType == 0)
				{
					_debugVoiceRule = (BTVoiceRule)sig.changedInfo;
				}
				else if (sig.changedInfoType == 1)
				{
					_debugMovementModelType = sig.changedInfo;
				}
				UpdateDebugBTStateInfoText();
			}
		}

		[PacketHandler(false)]
		protected void OnPacket(DebugMimicVoiceInfoSig sig)
		{
			if (!(this == null) && sig != null)
			{
				_voicePickReason = sig.VoicePickReason;
				UpdateDebugBTStateInfoText();
			}
		}

		[PacketHandler(false)]
		protected void OnPacket(DebugSpeechEventDeltaSig sig)
		{
		}

		public bool IsInputAvailable()
		{
			return InputDisableReason == 0;
		}

		public bool IsInputMoveAvailable()
		{
			if (InputMoveDisableReason == 0 && (InputDisableReason & 4) == 0 && (InputDisableReason & 8) == 0)
			{
				return (InputDisableReason & 1) == 0;
			}
			return false;
		}

		public void SetInputDisableReason(EInputDisableReason reason)
		{
			InputDisableReason |= (int)reason;
		}

		public void ClearInputDisableReason(EInputDisableReason reason)
		{
			InputDisableReason &= (int)(~reason);
		}

		public bool IsInputDisabledBy(EInputDisableReason reason)
		{
			return ((uint)InputDisableReason & (uint)reason) != 0;
		}

		public void SetInputMoveDisableReason(EInputMoveDisableReason reason)
		{
			InputMoveDisableReason |= (int)reason;
		}

		public void ClearInputMoveDisableReason(EInputMoveDisableReason reason)
		{
			InputMoveDisableReason &= (int)(~reason);
		}

		public bool IsInputMoveDisabledBy(EInputMoveDisableReason reason)
		{
			return ((uint)InputMoveDisableReason & (uint)reason) != 0;
		}

		public List<InventoryItem?> GetInventoryItems()
		{
			return inventory.SlotItems;
		}

		public int GetSelectedInventorySlotIndex()
		{
			return inventory.SelectedSlotIndex;
		}

		public InventoryItem? GetSelectedInventoryItem()
		{
			return inventory.SelectedItem;
		}

		public InventoryItem? FindInventoryItemByID(long itemID)
		{
			return inventory.FindItemByID(itemID);
		}

		public void OnItemSpawnFieldSkillWaitSig(long itemID, bool waitEvent)
		{
			inventory.OnItemSpawnFieldSkillWaitSig(itemID, waitEvent);
		}

		private void TransformInterpolation(float dt)
		{
			if (interpolationMethod == ProtoActorInterpolationType.deprecated_CurrAndFuture)
			{
				return;
			}
			if (interpolationMethod == ProtoActorInterpolationType.PathQueue)
			{
				if (this.moveInterpolationData.Count > 0)
				{
					float num = 1f;
					if (this.moveInterpolationData.Count > 10)
					{
						MoveInterpolationData moveInterpolationData = this.moveInterpolationData[this.moveInterpolationData.Count - 1];
						base.transform.position = moveInterpolationData.pos;
						this.moveInterpolationData.Clear();
						puppet?.Move(Vector3.zero, walkSpeed, runSpeed);
					}
					else
					{
						bool flag = false;
						while (!flag && this.moveInterpolationData.Count > 0)
						{
							num = 1f;
							if (this.moveInterpolationData.Count > 1)
							{
								float t = (float)(this.moveInterpolationData.Count - 1) / 9f;
								num = Mathf.Lerp(netintp_accelerationStartValue, 2f, t);
							}
							MoveInterpolationData moveInterpolationData2 = this.moveInterpolationData[0];
							Vector3 position = base.transform.position;
							Vector3 vector = moveInterpolationData2.pos - position;
							float num2 = Vector3.Distance(position, moveInterpolationData2.pos);
							Vector3 vector2 = Vector3.zero;
							float velocity = moveInterpolationData2.velocity;
							float num3 = moveInterpolationData2.velocity * num;
							if (num2 > 0.001f)
							{
								vector2 = vector.normalized;
							}
							Vector3 position2 = base.transform.position;
							if (num2 < num3 * dt)
							{
								base.transform.position = moveInterpolationData2.pos;
								this.moveInterpolationData.RemoveAt(0);
							}
							else
							{
								base.transform.position = Vector3.MoveTowards(position, moveInterpolationData2.pos, num3 * dt);
								flag = true;
							}
							Vector3.Distance(base.transform.position, position2);
							Vector3.Distance(base.transform.position, moveInterpolationData2.pos);
							float num4 = Mathf.Clamp(velocity, 0f, runSpeed);
							puppet?.Move(vector2 * num4, walkSpeed, runSpeed);
						}
					}
				}
				else
				{
					puppet?.Move(Vector3.zero, walkSpeed, runSpeed);
				}
				if (syncTargetRotY.HasValue)
				{
					float y = base.transform.rotation.eulerAngles.y;
					float num5 = CorrectEulerRot2(y, syncTargetRotY.Value);
					float y2 = Mathf.Lerp(y, num5, rotYInterpolationSpeed * dt);
					base.transform.rotation = Quaternion.Euler(0f, y2, 0f);
					float num6 = 0.1f;
					if (Mathf.Abs(y - num5) < num6)
					{
						syncTargetRotY = null;
					}
				}
				if (syncTargetCamRotX.HasValue)
				{
					float x = camRoot.localRotation.eulerAngles.x;
					float num7 = CorrectEulerRot2(x, syncTargetCamRotX.Value);
					float x2 = Mathf.Lerp(x, num7, rotXInterpolationSpeed * dt);
					camRoot.localRotation = Quaternion.Euler(x2, 0f, 0f);
					float num8 = 0.1f;
					if (Mathf.Abs(x - num7) < num8)
					{
						syncTargetCamRotX = null;
					}
				}
			}
			else if (interpolationMethod == ProtoActorInterpolationType.DebugByPass)
			{
				if (cc.enabled)
				{
					cc.enabled = false;
				}
				if (debug_syncTargetPos.HasValue)
				{
					base.transform.position = debug_syncTargetPos.Value;
				}
				if (syncTargetRotY.HasValue)
				{
					base.transform.rotation = Quaternion.Euler(0f, syncTargetRotY.Value, 0f);
				}
				if (syncTargetCamRotX.HasValue)
				{
					camRoot.localRotation = Quaternion.Euler(syncTargetCamRotX.Value, 0f, 0f);
				}
			}
		}

		public bool PlayVoiceOnActor(AudioClip clip, bool isTransmitter, bool isMimicVoiceEcho)
		{
			if (voiceEffecter == null)
			{
				Logger.RError("VoiceEffecter or VoiceAudioSource is not null");
				return false;
			}
			if (!isMimicVoiceEcho)
			{
				voiceEffecter.SetMimicVoiceEcho(isEnabled: false);
			}
			voiceEffecter.SetVoiceEffect(VoiceEffecter.VoiceEffectType.Transmitter, isTransmitter);
			if (isMimicVoiceEcho)
			{
				voiceEffecter.SetMimicVoiceEcho(isEnabled: true);
			}
			voiceEffecter.Set3DEffect(!isTransmitter);
			voiceEffecter.PlayAudioClip(clip);
			return true;
		}

		public void StopVoiceOnActor()
		{
			if (voiceEffecter != null)
			{
				voiceEffecter.StopVoice();
			}
		}

		public bool IsVoicePlaying()
		{
			if (voiceEffecter != null)
			{
				return voiceEffecter.IsVoicePlaying();
			}
			return false;
		}

		public IEnumerator RemoteVoiceFadeInCoroutine(AudioSource? inAudioSource)
		{
			float elapsedTime = 0f;
			float fadeDuration = 3f;
			SetAudioSourceVolume(inAudioSource, 0f);
			while (elapsedTime < fadeDuration)
			{
				elapsedTime += Time.deltaTime;
				SetAudioSourceVolume(inAudioSource, Mathf.Clamp01(elapsedTime / fadeDuration));
				yield return null;
			}
			SetAudioSourceVolume(inAudioSource, 1f);
			if (voiceEffecter != null)
			{
				voiceEffecter.OnEndVolumeFadeIn();
			}
		}

		private void SetAudioSourceVolume(AudioSource? inAudioSource, float inVolume)
		{
			if (!(inAudioSource == null))
			{
				inAudioSource.volume = inVolume;
			}
		}
	}
}
