using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Bifrost.Cooked;
using Mimic.Actors;
using Mimic.Voice.SpeechSystem;
using ReluProtocol.Enum;
using ReluReplay.Recorder;
using UnityEngine;

namespace DLAgent
{
	public class DLDecisionAgent : MonoBehaviour
	{
		public enum BaseBehavior
		{
			Stay = 0,
			MoveToNextRoom = 1,
			ToEntrance = 2,
			ToTram = 3,
			EnterTram = 4,
			ExitTram = 5,
			OutsideOther = 6,
			StayInTram = 7,
			ToTramFromBackdoor = 8,
			ToTramViaHelper = 9
		}

		public enum OutdoorArea
		{
			None = 0,
			Tram = 1,
			MainDoorOutside = 2,
			BackDoorOutside = 3,
			TramInside = 4,
			MainStreet = 5,
			MainStreetFromBackdoor = 6,
			Other = 7
		}

		[Serializable]
		public enum IndoorAreaPolicy
		{
			DefaultToggle = 0,
			ForceMoveOnly = 1,
			ForceStayOnly = 2
		}

		private const float FIELD_OF_VIEW_THRESHOLD = 60f;

		private const float EMOTE_RANGE_MULTIPLIER = 1.5f;

		private const long DECISION_INTERVAL_MS = 500L;

		private const long RECENT_DECISION_THRESHOLD_MS = 3000L;

		private const float SPHERE_CAST_RADIUS = 0.2f;

		private const float SWITCH_INTERACTION_RANGE = 7f;

		private const float SHUTTER_SWITCH_INTERACTION_RANGE = 7f;

		private const float CHARGER_INTERACTION_RANGE = 7f;

		private const float TRANSMITTER_INTERACTION_RANGE = 10f;

		private const float SPRINKLER_INTERACTION_RANGE = 3f;

		private const long MOVEMENT_DECISION_INTERVAL_MS = 100L;

		private List<VCreature> _cachedCreatureList = new List<VCreature>();

		private float _lastCreatureListUpdateTime;

		private const float CREATURE_LIST_UPDATE_INTERVAL = 0.5f;

		[SerializeField]
		private float _emoteRespondMaxWindow = 5f;

		private readonly Dictionary<DLDecisionType, long> _lastEvalTimeMs = new Dictionary<DLDecisionType, long>();

		private long _lastMovementDecisionTime;

		private Vector3 _tempVector1;

		private Vector3 _tempVector2;

		private Vector3 _tempVector3;

		private DLAgentDecisionOutput _dLAgentDecisionOutput = new DLAgentDecisionOutput();

		private DLAgentDecisionOutputForMovement _dLAgentDecisionForMovement = new DLAgentDecisionOutputForMovement();

		private DLAgentDecisionOutputForMovement _dLAgentDecisionForForcedRotation = new DLAgentDecisionOutputForMovement();

		private DLAgentDecisionOutputForMovement _dLAgentDecisionForRunning = new DLAgentDecisionOutputForMovement();

		private DLAgentDecisionOutputForMovement _dLAgentDecisionForJumping = new DLAgentDecisionOutputForMovement();

		[SerializeField]
		private VCreature _creature;

		[SerializeField]
		private float _emoteRespondProb = 1f;

		[SerializeField]
		private float _emoteSuggestProb = 0.3f;

		[SerializeField]
		private float _reactToSprinklerProb = 1f;

		[SerializeField]
		private float _useTrapSwitchProb = 1f;

		[SerializeField]
		private float _useChargerProb = 1f;

		[SerializeField]
		private float _useTransmitterProb = 1f;

		[SerializeField]
		private float _useShutterSwitchProb = 1f;

		[SerializeField]
		private float _reactToSprinklerCooltime = 3f;

		[SerializeField]
		private float _useTrapSwitchCooltime = 5f;

		[SerializeField]
		private float _useChargerCooltime = 5f;

		[SerializeField]
		private float _useTransmitterCooltime = 5f;

		[SerializeField]
		private float _emoteSuggestCooltime = 5f;

		[SerializeField]
		private float _useShutterSwitchCooltime = 5f;

		[SerializeField]
		private float _emoteCheckRange = 10f;

		[SerializeField]
		private Vector2 _emoteRespondDelayRange = new Vector2(0.7f, 1f);

		[SerializeField]
		private Vector2 _bewareExpireTimeRange = new Vector2(10f, 30f);

		[SerializeField]
		private float _bewareTargetNavDistance = 15f;

		private float _timeToAttackTimer;

		[SerializeField]
		private bool _checkForLookTarget = true;

		[SerializeField]
		[Range(0f, 100f)]
		private float _lookProbability = 5f;

		[SerializeField]
		private float _lookCooldownTime = 5f;

		private float _lookExecutionTime;

		[SerializeField]
		private Vector2 _lookDurationRange = new Vector2(0.5f, 2f);

		[SerializeField]
		private bool _checkForStationaryState = true;

		private Vector3 _prevPosition = Vector3.zero;

		private float _distanceMoved;

		private float _timeStationary;

		[SerializeField]
		private float _timeStationaryLimit = 2f;

		private const float _stationaryEpsilon = 0.01f;

		[SerializeField]
		private float _forceMoveWhenStationaryExecutionTime = 2f;

		[SerializeField]
		private float _forceMoveWhenStationaryCooldownTime = 1f;

		[SerializeField]
		private bool _checkForRunawayFromMonsters = true;

		[SerializeField]
		private float _monsterDetectRange = 5f;

		[SerializeField]
		private float _runawayFromMonstersExecutionTime = 5f;

		[SerializeField]
		private bool _checkForLookAtScanner = true;

		[SerializeField]
		private float _lookAtScannerCooltime = 5f;

		[SerializeField]
		private float _lookAtScannerProb = 0.1f;

		[SerializeField]
		private float _useLookAtScannerProb = 0.1f;

		private float _lookAtScannerExecutionTime;

		[SerializeField]
		private Vector2 _lookAtScannerDurationRange = new Vector2(0.5f, 2f);

		[SerializeField]
		private Vector2 _useLookAtScannerDurationRange = new Vector2(0.5f, 2f);

		private bool _justUsedScanner;

		private float _stayInTramTimer;

		[SerializeField]
		private float _stayInTramBeforeLookAtScannerDuration = 1f;

		[SerializeField]
		private float _stayInTramBeforeUseScannerDuration = 3f;

		[SerializeField]
		private bool _checkForUseScrapScanner = true;

		[SerializeField]
		private float _useScrapScannerCooltime = 10f;

		[SerializeField]
		private float _useScrapScannerProb = 0.5f;

		[SerializeField]
		private bool _checkForLookAtStashHanger = true;

		[SerializeField]
		private float _lookAtStashHangerCooltime = 5f;

		[SerializeField]
		private float _lookAtStashHangerProb = 0.3f;

		private float _lookAtStashHangerExecutionTime;

		[SerializeField]
		private Vector2 _lookAtStashHangerDurationRange = new Vector2(0.5f, 2f);

		private const float STASH_HANGER_INTERACTION_RANGE = 2.5f;

		[SerializeField]
		private float _dropFakeItemProb = 0.5f;

		[SerializeField]
		private float _dropFakeItemCooltime = 10f;

		[SerializeField]
		private bool _useBaseBehavior = true;

		[SerializeField]
		private Vector2 _baseStayDurationRange = new Vector2(3f, 6f);

		[SerializeField]
		private Vector2 _baseMoveDurationRange = new Vector2(4f, 7f);

		private BaseBehavior _baseBehavior;

		private long _baseStartedAtMs = -1L;

		private float _baseDurationSec;

		[SerializeField]
		private float _outdoorForceToggleSec = 45f;

		[SerializeField]
		private float _exitTramProb = 0.9f;

		[SerializeField]
		private float _enterTramProb = 0.9f;

		[SerializeField]
		private Vector2 _toTramDurationRange = new Vector2(10f, 45f);

		[SerializeField]
		private Vector2 _toTramViaHelperDurationRange = new Vector2(5f, 6f);

		[SerializeField]
		private Vector2 _toEntranceDurationRange = new Vector2(3f, 30f);

		[SerializeField]
		private Vector2 _enterTramDurationRange = new Vector2(8f, 10f);

		[SerializeField]
		private Vector2 _exitTramDurationRange = new Vector2(8f, 10f);

		[SerializeField]
		private Vector2 _outsideOtherDurationRange = new Vector2(8f, 45f);

		[SerializeField]
		private Vector2 _insideTramDurationRange = new Vector2(2f, 3f);

		private long _earliestSwitchAtMs;

		private long _forceSwitchAtMs;

		private BaseBehavior? _queuedBehavior;

		private bool _isOutdoorSnapshot;

		private OutdoorArea _outdoorAreaSnapshot;

		private SpeechType_Area _indoorAreaSnapshot = SpeechType_Area.Indoor;

		private IndoorAreaPolicy _indoorAreaPolicy;

		[SerializeField]
		private bool _checkForForceRotation = true;

		[SerializeField]
		private float _wallDetectRange = 3f;

		[SerializeField]
		private float _forceRotationCooldownTime = 0.5f;

		[SerializeField]
		private float _forceRotationHoldDuration = 0.8f;

		private long _forceRotationHoldTime;

		[SerializeField]
		private bool _checkForRun = true;

		private float _runCooldownTime = 5f;

		[SerializeField]
		private Vector2 _runCooldownTimeRange = new Vector2(2f, 5f);

		[SerializeField]
		[Range(0f, 100f)]
		private float _runProbability = 30f;

		private float _runExecutionTime;

		[SerializeField]
		private Vector2 _runDurationRange = new Vector2(1f, 3f);

		[SerializeField]
		private bool _checkForJump = true;

		[SerializeField]
		private float _jumpCooldownTime = 5f;

		[SerializeField]
		[Range(0f, 100f)]
		private float _jumpProbability = 10f;

		private float _jumpExecutionTime;

		[SerializeField]
		private Vector2 _jumpDurationRange = new Vector2(0.5f, 2f);

		[SerializeField]
		private bool _checkForTooFarAwayFromTarget = true;

		[SerializeField]
		private float _tooFarAwayFromTargetDistance = 10f;

		[SerializeField]
		[Range(0f, 100f)]
		private float _tooFarAwayFromTargetProbability = 50f;

		private float _tooFarAwayFromTargetExecutionTime = 1f;

		[SerializeField]
		private Vector2 _tooFarAwayFromTargetDurationRange = new Vector2(0.5f, 0.8f);

		[SerializeField]
		private bool _checkForCircularPattern = true;

		[SerializeField]
		private float _circleYawDeltaHistorySize = 30f;

		[SerializeField]
		private float _circularMovementEpsilon = 2f;

		[SerializeField]
		private float _circularMovementThreshold = 0.8f;

		private float _lastYawForCircularPattern = float.NaN;

		private Queue<float> _yawDeltaHistory = new Queue<float>();

		[SerializeField]
		[Range(0f, 1f)]
		private float _moveToTramViaHelperProb = 0.5f;

		private long _lastTick;

		private Queue<DLAgentDecisionOutput> _dLAgentDecisionOutputHistory = new Queue<DLAgentDecisionOutput>();

		private Queue<DLAgentDecisionOutputForMovement> _dLAgentDecisionForMovementOutputHistory = new Queue<DLAgentDecisionOutputForMovement>();

		[SerializeField]
		private int _historyCapacity = 5;

		[SerializeField]
		private int _historyCapacityForMovement = 50;

		[SerializeField]
		private BTVoiceRule _btVoiceRule;

		private ReplayRecorder _replayRecorder;

		private List<VCreature> _creatureList = new List<VCreature>();

		private VCreature _pendingEmoteRespondTarget;

		private long _pendingEmoteRespondTargetEmoteStartTime;

		private long _pendingEmoteRespondReadyTime;

		[SerializeField]
		private DLMovementAgent _movementAgent;

		[SerializeField]
		private RealWorldRaySensor _realWorldRaySensor;

		[SerializeField]
		private bool _checkFriendly = true;

		[SerializeField]
		private Vector2 _checkFriendlyDurationRange = new Vector2(2f, 5f);

		[SerializeField]
		private float _checkFriendlyExecutionTime;

		public DLAgentDecisionOutput DLAgentDecisionOutput => _dLAgentDecisionOutput;

		public DLAgentDecisionOutputForMovement DLAgentDecisionForMovement => _dLAgentDecisionForMovement;

		public DLAgentDecisionOutputForMovement DLAgentDecisionForForcedRotation => _dLAgentDecisionForForcedRotation;

		public DLAgentDecisionOutputForMovement DLAgentDecisionForRunning => _dLAgentDecisionForRunning;

		public DLAgentDecisionOutputForMovement DLAgentDecisionForJumping => _dLAgentDecisionForJumping;

		public BTVoiceRule VoiceRule => _btVoiceRule;

		private bool CheckMutualLineOfSight(VCreature actorA, VCreature actorB, float threshold = 60f)
		{
			_tempVector1 = actorA.PositionVector;
			_tempVector2 = actorB.PositionVector;
			float f = actorA.Position.yaw * (MathF.PI / 180f);
			float f2 = actorB.Position.yaw * (MathF.PI / 180f);
			_tempVector3.Set(Mathf.Sin(f), 0f, Mathf.Cos(f));
			Vector3 tempVector = _tempVector3;
			_tempVector3.Set(Mathf.Sin(f2), 0f, Mathf.Cos(f2));
			Vector3 tempVector2 = _tempVector3;
			Vector3 normalized = (_tempVector2 - _tempVector1).normalized;
			Vector3 normalized2 = (_tempVector1 - _tempVector2).normalized;
			float num = Vector3.Angle(tempVector, normalized);
			float num2 = Vector3.Angle(tempVector2, normalized2);
			if (num <= threshold)
			{
				return num2 <= threshold;
			}
			return false;
		}

		private bool CheckLineOfSight(VCreature actor, Vector3 targetDirection, float threshold = 60f)
		{
			float f = actor.Position.yaw * (MathF.PI / 180f);
			_tempVector1.Set(Mathf.Sin(f), 0f, Mathf.Cos(f));
			return Vector3.Angle(_tempVector1, targetDirection.normalized) <= threshold;
		}

		private List<VCreature> GetCachedCreatureList()
		{
			float time = Time.time;
			if (time - _lastCreatureListUpdateTime > 0.5f)
			{
				_lastCreatureListUpdateTime = time;
				_cachedCreatureList.Clear();
				List<VCreature> creaturesInRange = _creature.VRoom.GetCreaturesInRange(_creature, _emoteCheckRange);
				float num = Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold;
				Vector3 myPos = _creature.PositionVector;
				foreach (VCreature item in creaturesInRange)
				{
					if (Mathf.Abs(myPos.y - item.PositionVector.y) <= num)
					{
						_cachedCreatureList.Add(item);
					}
				}
				_cachedCreatureList.Sort((VCreature a, VCreature b) => Vector3.SqrMagnitude(myPos - a.PositionVector).CompareTo(Vector3.SqrMagnitude(myPos - b.PositionVector)));
			}
			return _cachedCreatureList;
		}

		private bool IsWithinNavigationRange(Vector3 targetPos, float maxRange)
		{
			return Hub.s.vworld.GetNavDistance(_creature.PositionVector, targetPos) < maxRange;
		}

		public void SetCreature(VCreature creature)
		{
			_creature = creature;
		}

		private void OnEnable()
		{
			_dLAgentDecisionOutput.Reset();
			GetBlackboardParam();
			_prevPosition = Vector3.zero;
			_timeStationary = 0f;
			_pendingEmoteRespondTarget = null;
			_pendingEmoteRespondTargetEmoteStartTime = 0L;
			_pendingEmoteRespondReadyTime = 0L;
			InitBaseBehaviorAccordingToEnvironment();
			_lastYawForCircularPattern = float.NaN;
			_yawDeltaHistory.Clear();
		}

		private void OnDisable()
		{
			_dLAgentDecisionOutput.Reset();
			_pendingEmoteRespondTarget = null;
			_pendingEmoteRespondTargetEmoteStartTime = 0L;
			_pendingEmoteRespondReadyTime = 0L;
		}

		private static bool CooldownPassedMs(long lastMs, float cooldownSec, long nowMs)
		{
			return nowMs - lastMs >= (long)(cooldownSec * 1000f);
		}

		private static float ElapsedSecFromMs(long lastMs, long nowMs)
		{
			return (float)(nowMs - lastMs) * 0.001f;
		}

		public void Update()
		{
			if (!IsValidForUpdate())
			{
				return;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (_lastTick == 0L)
			{
				_lastTick = currentTickMilliSec;
			}
			long num = currentTickMilliSec - _lastTick;
			_timeToAttackTimer += Time.deltaTime;
			if (currentTickMilliSec - _lastMovementDecisionTime >= 100)
			{
				float deltaTime = ((_lastMovementDecisionTime == 0L) ? 0.1f : ElapsedSecFromMs(_lastMovementDecisionTime, currentTickMilliSec));
				_lastMovementDecisionTime = currentTickMilliSec;
				_tempVector1 = _creature.PositionVector;
				_distanceMoved = Vector3.Distance(_tempVector1, _prevPosition);
				_prevPosition = _tempVector1;
				UpdateDecisionForMovement(deltaTime);
			}
			if (num >= 500)
			{
				_lastTick = currentTickMilliSec;
				_dLAgentDecisionOutput.Reset();
				_creatureList = GetCachedCreatureList();
				ProcessDecisions();
				AddDecisionOutput();
				if (CheckConsecutiveTargetLookingAway())
				{
					_dLAgentDecisionOutput.Decision = DLDecisionType.TimeToAttack;
					AddDecisionOutput();
				}
				_ = _dLAgentDecisionOutput.Decision;
			}
		}

		private bool IsValidForUpdate()
		{
			if (Hub.s?.timeutil != null)
			{
				VCreature creature = _creature;
				if (creature != null && creature.IsAliveStatus())
				{
					return _creature.VRoom != null;
				}
			}
			return false;
		}

		private bool HasRecentEmoteRespondDecision()
		{
			if (_dLAgentDecisionOutputHistory.Count == 0)
			{
				return false;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			long num = 3000L;
			DLAgentDecisionOutput dLAgentDecisionOutput = _dLAgentDecisionOutputHistory.Last();
			if (dLAgentDecisionOutput != null && dLAgentDecisionOutput.Decision != DLDecisionType.None && dLAgentDecisionOutput.Decision != DLDecisionType.EmoteRespond && dLAgentDecisionOutput.Decision != DLDecisionType.EmoteRespondPending)
			{
				return dLAgentDecisionOutput.DecisionTime + num > currentTickMilliSec;
			}
			return false;
		}

		private void ProcessDecisions()
		{
			if (!CheckAttackTime() && !CheckEmoteRespond() && !CheckUseScrapScanner() && !CheckDropFakeItems() && !CheckSprinkler() && !CheckCharger() && !CheckTransmitter() && !CheckTrapSwitch() && !CheckUseShutterSwitch() && !CheckEmoteSuggest())
			{
				_dLAgentDecisionOutput.Decision = DLDecisionType.None;
			}
		}

		private void UpdateDecisionForMovement(float deltaTime)
		{
			_stayInTramTimer += deltaTime;
			SampleYawForCircularPattern();
			if (_dLAgentDecisionOutput.Decision != DLDecisionType.None)
			{
				return;
			}
			DLDecisionType baseMovementDecision = GetBaseMovementDecision();
			bool flag = false;
			switch (baseMovementDecision)
			{
			case DLDecisionType.MoveToTram:
			case DLDecisionType.MoveToTramViaHelper:
			case DLDecisionType.MoveToEntrance:
			case DLDecisionType.EnterTram:
				if (_outdoorAreaSnapshot == OutdoorArea.MainDoorOutside && baseMovementDecision == DLDecisionType.MoveToTram && CheckTooFarAwayFromTarget())
				{
					flag = true;
				}
				break;
			case DLDecisionType.StayInTram:
			case DLDecisionType.ExitTram:
				if (CheckLookAtScrapScanner() || CheckStashHanger())
				{
					flag = true;
				}
				break;
			case DLDecisionType.MoveToTramFromBackdoor:
				if (CheckTimeToLookTarget())
				{
					flag = true;
				}
				break;
			case DLDecisionType.OutsideOther:
				if (CheckTooFarAwayFromTarget())
				{
					flag = true;
				}
				break;
			default:
				if (CheckRunawayFromMonsters())
				{
					flag = true;
				}
				else if (CheckStationaryState(deltaTime))
				{
					flag = true;
				}
				else if (CheckTimeToLookTarget())
				{
					flag = true;
				}
				else if (baseMovementDecision == DLDecisionType.MoveToNextRoom && CheckTooFarAwayFromTarget())
				{
					flag = true;
				}
				break;
			}
			if (!flag)
			{
				_dLAgentDecisionForMovement.Decision = baseMovementDecision;
				AddDecisionOutput(forMovement: true);
			}
			if (baseMovementDecision != DLDecisionType.MoveToNextRoom && baseMovementDecision != DLDecisionType.MoveToTram && baseMovementDecision != DLDecisionType.MoveToEntrance && baseMovementDecision != DLDecisionType.StayInTram && baseMovementDecision != DLDecisionType.ExitTram && baseMovementDecision != DLDecisionType.EnterTram && baseMovementDecision != DLDecisionType.MoveToTramFromBackdoor && baseMovementDecision != DLDecisionType.MoveToTramViaHelper)
			{
				CheckForcedRotation();
			}
			else if (_outdoorAreaSnapshot == OutdoorArea.MainDoorOutside && baseMovementDecision == DLDecisionType.MoveToTram)
			{
				CheckForcedRotation();
			}
			else if (baseMovementDecision == DLDecisionType.MoveToNextRoom)
			{
				Hub.s.dLAcademyManager.GetAreaForDL(_creature.PositionVector, out var areaType);
				if (areaType == SpeechType_Area.MainDoor_Indoor)
				{
					CheckForcedRotation();
				}
			}
			CheckRunning();
			CheckJumping();
		}

		public void SetVoiceRule(BTVoiceRule rule)
		{
			if (_btVoiceRule != rule)
			{
				_creature.SendInSight(new DebugDLAgentInfoSig
				{
					actorID = _creature.ObjectID,
					changedInfoType = 0,
					changedInfo = (int)rule
				});
			}
			_btVoiceRule = rule;
		}

		public void AddDecisionOutput(bool forMovement = false)
		{
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (forMovement)
			{
				DLAgentDecisionOutputForMovement dLAgentDecisionOutputForMovement = _dLAgentDecisionForMovement.Clone();
				dLAgentDecisionOutputForMovement.DecisionTime = currentTickMilliSec;
				_dLAgentDecisionForMovementOutputHistory.Enqueue(dLAgentDecisionOutputForMovement);
				if (_dLAgentDecisionForMovementOutputHistory.Count > _historyCapacityForMovement)
				{
					_dLAgentDecisionForMovementOutputHistory.Dequeue();
				}
			}
			else
			{
				DLAgentDecisionOutput dLAgentDecisionOutput = _dLAgentDecisionOutput.Clone();
				dLAgentDecisionOutput.DecisionTime = currentTickMilliSec;
				_dLAgentDecisionOutputHistory.Enqueue(dLAgentDecisionOutput);
				if (_dLAgentDecisionOutputHistory.Count > _historyCapacity)
				{
					_dLAgentDecisionOutputHistory.Dequeue();
				}
			}
		}

		public bool CheckEmoteRespond()
		{
			if (_pendingEmoteRespondTarget == null && !Chance01(_emoteRespondProb))
			{
				return false;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			DLAgentDecisionOutput objA = _dLAgentDecisionOutputHistory.LastOrDefault();
			VCreature vCreature = null;
			foreach (VCreature creature in _creatureList)
			{
				if (creature.ActorType == ActorType.Player)
				{
					EmotionController? emotionControlUnit = creature.EmotionControlUnit;
					if ((emotionControlUnit == null || emotionControlUnit.PlayedEmotionMasterID != 0) && CheckMutualLineOfSight(_creature, creature) && IsWithinNavigationRange(creature.PositionVector, _emoteCheckRange * 1.5f))
					{
						vCreature = creature;
						break;
					}
				}
			}
			if (vCreature == null || vCreature.EmotionControlUnit == null)
			{
				_pendingEmoteRespondTarget = null;
				_pendingEmoteRespondTargetEmoteStartTime = 0L;
				_pendingEmoteRespondReadyTime = 0L;
				return false;
			}
			long emotionStartTime = vCreature.EmotionControlUnit.EmotionStartTime;
			if (_pendingEmoteRespondTarget != vCreature || _pendingEmoteRespondTargetEmoteStartTime != emotionStartTime)
			{
				_pendingEmoteRespondTarget = vCreature;
				_pendingEmoteRespondTargetEmoteStartTime = emotionStartTime;
				float num = UnityEngine.Random.Range(_emoteRespondDelayRange.x, _emoteRespondDelayRange.y);
				long pendingEmoteRespondReadyTime = emotionStartTime + (long)(num * 1000f);
				_pendingEmoteRespondReadyTime = pendingEmoteRespondReadyTime;
			}
			if (currentTickMilliSec < _pendingEmoteRespondReadyTime)
			{
				_dLAgentDecisionOutput.Decision = DLDecisionType.EmoteRespondPending;
				_dLAgentDecisionOutput.TargetActor = vCreature;
				return true;
			}
			_dLAgentDecisionOutput.Decision = DLDecisionType.EmoteRespond;
			_dLAgentDecisionOutput.TargetActor = vCreature;
			_dLAgentDecisionOutput.EmoteMasterID = vCreature.EmotionControlUnit.PlayedEmotionMasterID;
			_dLAgentDecisionOutput.EmoteTime = emotionStartTime;
			_dLAgentDecisionOutput.ResetEmoteInfoChanged();
			if (!object.Equals(objA, _dLAgentDecisionOutput))
			{
				_dLAgentDecisionOutput.SetEmoteInfoChanged();
			}
			return true;
		}

		public bool CheckEmoteSuggest()
		{
			if (_creatureList == null || _creatureList.Count == 0)
			{
				return false;
			}
			if (!TryEnterEvaluationWindow(DLDecisionType.EmoteSuggest, _emoteSuggestCooltime))
			{
				return false;
			}
			if (!Chance01(_emoteSuggestProb))
			{
				return false;
			}
			foreach (VCreature creature in _creatureList)
			{
				if (creature.ActorType == ActorType.Player && CheckMutualLineOfSight(_creature, creature) && IsWithinNavigationRange(creature.PositionVector, _emoteCheckRange * 1.5f))
				{
					_dLAgentDecisionOutput.Decision = DLDecisionType.EmoteSuggest;
					_dLAgentDecisionOutput.TargetActor = creature;
					return true;
				}
			}
			return false;
		}

		public bool CheckTransmitter()
		{
			if (!TryEnterEvaluationWindow(DLDecisionType.UseTransmitter, _useTransmitterCooltime))
			{
				return false;
			}
			if (!Chance01(_useTransmitterProb))
			{
				return false;
			}
			List<ILevelObjectInfo> list = _creature.VRoom.FindLevelObjectsByType(_creature.Position.toVector3(), LevelObjectClientType.Transmitter, checkHeight: true, AIRangeType.Absolute);
			if (list.Count == 0)
			{
				return false;
			}
			Vector3 posB = _creature.Position.toVector3();
			foreach (ILevelObjectInfo item in list)
			{
				if (Misc.Distance(item.Pos, posB) <= 10f)
				{
					_dLAgentDecisionOutput.Decision = DLDecisionType.UseTransmitter;
					return true;
				}
			}
			return false;
		}

		public bool CheckSprinkler()
		{
			if (!TryEnterEvaluationWindow(DLDecisionType.ReactToSprinkler, _reactToSprinklerCooltime))
			{
				return false;
			}
			if (!Chance01(_reactToSprinklerProb))
			{
				return false;
			}
			List<FieldSkillObject> fieldSkillObjects = _creature.VRoom.GetFieldSkillObjects();
			Vector3 vector = _creature.Position.toVector3();
			float num = Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold;
			foreach (FieldSkillObject item in fieldSkillObjects)
			{
				if (item.GetFieldSkillMasterID() != 70007)
				{
					continue;
				}
				Vector3 vector2 = item.Position.toVector3();
				if (!(Mathf.Abs(vector2.y - vector.y) >= num))
				{
					float num2 = (vector2.x - vector.x) * (vector2.x - vector.x);
					float num3 = (vector2.z - vector.z) * (vector2.z - vector.z);
					if (num2 + num3 < 9f)
					{
						_dLAgentDecisionOutput.Decision = DLDecisionType.ReactToSprinkler;
						return true;
					}
				}
			}
			return false;
		}

		public bool CheckTrapSwitch()
		{
			if (!TryEnterEvaluationWindow(DLDecisionType.UseTrapSwitch, _useTrapSwitchCooltime))
			{
				return false;
			}
			if (!Chance01(_useTrapSwitchProb))
			{
				return false;
			}
			List<ILevelObjectInfo> list = _creature.VRoom.FindLevelObjectsByType(_creature.Position.toVector3(), LevelObjectClientType.MomentarySwitch, checkHeight: true, AIRangeType.Absolute);
			if (list.Count == 0)
			{
				return false;
			}
			Vector3 posB = _creature.Position.toVector3();
			foreach (ILevelObjectInfo item in list)
			{
				if (Misc.Distance(item.Pos, posB) <= 7f)
				{
					_dLAgentDecisionOutput.Decision = DLDecisionType.UseTrapSwitch;
					return true;
				}
			}
			return false;
		}

		public bool CheckCharger()
		{
			if (_creature == null || _creature.VRoom == null)
			{
				return false;
			}
			if (!TryEnterEvaluationWindow(DLDecisionType.UseCharger, _useChargerCooltime))
			{
				return false;
			}
			if (_creature.InventoryControlUnit == null || !_creature.InventoryControlUnit.HasRechargableItem())
			{
				return false;
			}
			if (!Chance01(_useChargerProb))
			{
				return false;
			}
			if (Hub.s.dLAcademyManager.GetAreaForDL(_creature.PositionVector, out var _) == 1)
			{
				return false;
			}
			Vector3 vector = _creature.Position.toVector3();
			List<ILevelObjectInfo> list = _creature.VRoom.FindLevelObjectsByType(vector, LevelObjectClientType.Electronics_05, checkHeight: true, AIRangeType.Absolute);
			if (list == null || list.Count == 0)
			{
				return false;
			}
			float num = Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold;
			foreach (ILevelObjectInfo item in list)
			{
				if (item != null && Misc.Distance(item.Pos, vector) <= 7f && Mathf.Abs(item.Pos.y - vector.y) <= num)
				{
					_dLAgentDecisionOutput.Decision = DLDecisionType.UseCharger;
					return true;
				}
			}
			return false;
		}

		public bool CheckOutputHistory(DLDecisionType decisionType, float cooltime, bool forMovement = false)
		{
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			long? num = null;
			if (forMovement)
			{
				foreach (DLAgentDecisionOutputForMovement item in _dLAgentDecisionForMovementOutputHistory)
				{
					if (item.Decision == decisionType && (!num.HasValue || item.DecisionTime > num.Value))
					{
						num = item.DecisionTime;
					}
				}
			}
			else
			{
				foreach (DLAgentDecisionOutput item2 in _dLAgentDecisionOutputHistory)
				{
					if (item2.Decision == decisionType && (!num.HasValue || item2.DecisionTime > num.Value))
					{
						num = item2.DecisionTime;
					}
				}
			}
			if (!num.HasValue)
			{
				return true;
			}
			return CooldownPassedMs(num.Value, cooltime, currentTickMilliSec);
		}

		public bool CheckAttackTime()
		{
			VCreature creature = _creature;
			AIController aIController = creature?.AIControlUnit;
			if (creature == null || aIController == null)
			{
				return false;
			}
			if (_timeToAttackTimer < _bewareExpireTimeRange.x)
			{
				return false;
			}
			if (_timeToAttackTimer > _bewareExpireTimeRange.y)
			{
				_dLAgentDecisionOutput.Decision = DLDecisionType.TimeToAttack;
				return true;
			}
			VCreature target = aIController.GetTarget();
			if (target == null || !target.IsAliveStatus())
			{
				return false;
			}
			Vector3 positionVector = target.PositionVector;
			Vector3 positionVector2 = creature.PositionVector;
			if (!IsWithinNavigationRange(positionVector, _bewareTargetNavDistance))
			{
				return false;
			}
			_tempVector1 = positionVector - positionVector2;
			if (CheckLineOfSight(target, _tempVector1))
			{
				return false;
			}
			_dLAgentDecisionOutput.Decision = DLDecisionType.TargetLookingAway;
			return true;
		}

		public void GetBlackboardParam()
		{
			(BlackBoardDataType, string)? value = _creature.AIControlUnit.BlackBoard.GetValue("EmoteRespondProb");
			if (value.HasValue && value.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value.Value.Item2, out var result))
			{
				_emoteRespondProb = (float)result / 10000f;
			}
			(BlackBoardDataType, string)? value2 = _creature.AIControlUnit.BlackBoard.GetValue("EmoteSuggestProb");
			if (value2.HasValue && value2.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value2.Value.Item2, out var result2))
			{
				_emoteSuggestProb = (float)result2 / 10000f;
			}
			(BlackBoardDataType, string)? value3 = _creature.AIControlUnit.BlackBoard.GetValue("ReactToSprinklerProb");
			if (value3.HasValue && value3.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value3.Value.Item2, out var result3))
			{
				_reactToSprinklerProb = (float)result3 / 10000f;
			}
			(BlackBoardDataType, string)? value4 = _creature.AIControlUnit.BlackBoard.GetValue("UseTrapSwitchProb");
			if (value4.HasValue && value4.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value4.Value.Item2, out var result4))
			{
				_useTrapSwitchProb = (float)result4 / 10000f;
			}
			(BlackBoardDataType, string)? value5 = _creature.AIControlUnit.BlackBoard.GetValue("UseChargerProb");
			if (value5.HasValue && value5.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value5.Value.Item2, out var result5))
			{
				_useChargerProb = (float)result5 / 10000f;
			}
			(BlackBoardDataType, string)? value6 = _creature.AIControlUnit.BlackBoard.GetValue("LookAtScrapScannerProb");
			if (value6.HasValue && value6.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value6.Value.Item2, out var result6))
			{
				_lookAtScannerProb = (float)result6 / 10000f;
			}
			(BlackBoardDataType, string)? value7 = _creature.AIControlUnit.BlackBoard.GetValue("LookAtScrapScannerCooltime");
			if (value7.HasValue && value7.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value7.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result7))
			{
				_lookAtScannerCooltime = result7;
			}
			(BlackBoardDataType, string)? value8 = _creature.AIControlUnit.BlackBoard.GetValue("ReactToSprinklerCooltime");
			if (value8.HasValue && value8.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value8.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result8))
			{
				_reactToSprinklerCooltime = result8;
			}
			(BlackBoardDataType, string)? value9 = _creature.AIControlUnit.BlackBoard.GetValue("UseTrapSwitchCooltime");
			if (value9.HasValue && value9.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value9.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result9))
			{
				_useTrapSwitchCooltime = result9;
			}
			(BlackBoardDataType, string)? value10 = _creature.AIControlUnit.BlackBoard.GetValue("UseChargerCooltime");
			if (value10.HasValue && value10.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value10.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result10))
			{
				_useChargerCooltime = result10;
			}
			(BlackBoardDataType, string)? value11 = _creature.AIControlUnit.BlackBoard.GetValue("EmoteCheckRange");
			if (value11.HasValue && value11.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value11.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result11))
			{
				_emoteCheckRange = result11;
			}
			(BlackBoardDataType, string)? value12 = _creature.AIControlUnit.BlackBoard.GetValue("UseTransmitterProb");
			if (value12.HasValue && value12.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value12.Value.Item2, out var result12))
			{
				_useTransmitterProb = (float)result12 / 10000f;
			}
			(BlackBoardDataType, string)? value13 = _creature.AIControlUnit.BlackBoard.GetValue("UseTransmitterCooltime");
			if (value13.HasValue && value13.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value13.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result13))
			{
				_useTransmitterCooltime = result13;
			}
			(BlackBoardDataType, string)? value14 = _creature.AIControlUnit.BlackBoard.GetValue("EmoteSuggestCooltime");
			if (value14.HasValue && value14.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value14.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result14))
			{
				_emoteSuggestCooltime = result14;
			}
			(BlackBoardDataType, string)? value15 = _creature.AIControlUnit.BlackBoard.GetValue("EmoteRespondDelayMin");
			if (value15.HasValue && value15.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value15.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result15))
			{
				_emoteRespondDelayRange.x = result15;
			}
			(BlackBoardDataType, string)? value16 = _creature.AIControlUnit.BlackBoard.GetValue("EmoteRespondDelayMax");
			if (value16.HasValue && value16.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value16.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result16))
			{
				_emoteRespondDelayRange.y = result16;
			}
			if (_emoteRespondDelayRange.y < _emoteRespondDelayRange.x)
			{
				float x = _emoteRespondDelayRange.x;
				_emoteRespondDelayRange.x = _emoteRespondDelayRange.y;
				_emoteRespondDelayRange.y = x;
			}
			(BlackBoardDataType, string)? value17 = _creature.AIControlUnit.BlackBoard.GetValue("BewareBTTimeMin");
			if (value17.HasValue && value17.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value17.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result17))
			{
				_bewareExpireTimeRange.x = result17;
			}
			(BlackBoardDataType, string)? value18 = _creature.AIControlUnit.BlackBoard.GetValue("BewareBTTimeMax");
			if (value18.HasValue && value18.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value18.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result18))
			{
				_bewareExpireTimeRange.y = result18;
			}
			(BlackBoardDataType, string)? value19 = _creature.AIControlUnit.BlackBoard.GetValue("UseShutterSwitchCooltime");
			if (value19.HasValue && value19.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value19.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result19))
			{
				_useShutterSwitchCooltime = result19;
			}
			(BlackBoardDataType, string)? value20 = _creature.AIControlUnit.BlackBoard.GetValue("UseShutterSwitchProb");
			if (value20.HasValue && value20.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value20.Value.Item2, out var result20))
			{
				_useShutterSwitchProb = (float)result20 / 10000f;
			}
			(BlackBoardDataType, string)? value21 = _creature.AIControlUnit.BlackBoard.GetValue("BewareTargetNavDistance");
			if (value21.HasValue && value21.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value21.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result21))
			{
				_bewareTargetNavDistance = result21;
			}
			(BlackBoardDataType, string)? value22 = _creature.AIControlUnit.BlackBoard.GetValue("UseScrapScannerCooltime");
			if (value22.HasValue && value22.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value22.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result22))
			{
				_useScrapScannerCooltime = result22;
			}
			(BlackBoardDataType, string)? value23 = _creature.AIControlUnit.BlackBoard.GetValue("UseScrapScannerProb");
			if (value23.HasValue && value23.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value23.Value.Item2, out var result23))
			{
				_useScrapScannerProb = (float)result23 / 10000f;
			}
			(BlackBoardDataType, string)? value24 = _creature.AIControlUnit.BlackBoard.GetValue("StayInTramBeforeLookAtScannerDuration");
			if (value24.HasValue && value24.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value24.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result24))
			{
				_stayInTramBeforeLookAtScannerDuration = result24;
			}
			(BlackBoardDataType, string)? value25 = _creature.AIControlUnit.BlackBoard.GetValue("StayInTramBeforeUseScannerDuration");
			if (value25.HasValue && value25.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value25.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result25))
			{
				_stayInTramBeforeUseScannerDuration = result25;
			}
			(BlackBoardDataType, string)? value26 = _creature.AIControlUnit.BlackBoard.GetValue("DropFakeItemProb");
			if (value26.HasValue && value26.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value26.Value.Item2, out var result26))
			{
				_dropFakeItemProb = (float)result26 / 10000f;
			}
			(BlackBoardDataType, string)? value27 = _creature.AIControlUnit.BlackBoard.GetValue("DropFakeItemCooltime");
			if (value27.HasValue && value27.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value27.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result27))
			{
				_dropFakeItemCooltime = result27;
			}
			(BlackBoardDataType, string)? value28 = _creature.AIControlUnit.BlackBoard.GetValue("LookAtScannerDurationMin");
			if (value28.HasValue && value28.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value28.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result28))
			{
				_lookAtScannerDurationRange.x = result28;
			}
			(BlackBoardDataType, string)? value29 = _creature.AIControlUnit.BlackBoard.GetValue("LookAtScannerDurationMax");
			if (value29.HasValue && value29.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value29.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result29))
			{
				_lookAtScannerDurationRange.y = result29;
			}
			if (_lookAtScannerDurationRange.y < _lookAtScannerDurationRange.x)
			{
				float x2 = _lookAtScannerDurationRange.x;
				_lookAtScannerDurationRange.x = _lookAtScannerDurationRange.y;
				_lookAtScannerDurationRange.y = x2;
			}
			(BlackBoardDataType, string)? value30 = _creature.AIControlUnit.BlackBoard.GetValue("UseLookAtScrapScannerProb");
			if (value30.HasValue && value30.Value.Item1 == BlackBoardDataType.Int && int.TryParse(value30.Value.Item2, out var result30))
			{
				_useLookAtScannerProb = (float)result30 / 10000f;
			}
			(BlackBoardDataType, string)? value31 = _creature.AIControlUnit.BlackBoard.GetValue("UseLookAtScrapScannerDurationMin");
			if (value31.HasValue && value31.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value31.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result31))
			{
				_useLookAtScannerDurationRange.x = result31;
			}
			(BlackBoardDataType, string)? value32 = _creature.AIControlUnit.BlackBoard.GetValue("UseLookAtScrapScannerDurationMax");
			if (value32.HasValue && value32.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value32.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result32))
			{
				_useLookAtScannerDurationRange.y = result32;
			}
			if (_useLookAtScannerDurationRange.y < _useLookAtScannerDurationRange.x)
			{
				float x3 = _useLookAtScannerDurationRange.x;
				_useLookAtScannerDurationRange.x = _useLookAtScannerDurationRange.y;
				_useLookAtScannerDurationRange.y = x3;
			}
			(BlackBoardDataType, string)? value33 = _creature.AIControlUnit.BlackBoard.GetValue("LookAtStashHangerDurationMin");
			if (value33.HasValue && value33.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value33.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result33))
			{
				_lookAtStashHangerDurationRange.x = result33;
			}
			(BlackBoardDataType, string)? value34 = _creature.AIControlUnit.BlackBoard.GetValue("LookAtStashHangerDurationMax");
			if (value34.HasValue && value34.Value.Item1 == BlackBoardDataType.Float && float.TryParse(value34.Value.Item2, NumberStyles.Any, CultureInfo.InvariantCulture, out var result34))
			{
				_lookAtStashHangerDurationRange.y = result34;
			}
			if (_lookAtStashHangerDurationRange.y < _lookAtStashHangerDurationRange.x)
			{
				float x4 = _lookAtStashHangerDurationRange.x;
				_lookAtStashHangerDurationRange.x = _lookAtStashHangerDurationRange.y;
				_lookAtStashHangerDurationRange.y = x4;
			}
		}

		private bool CheckConsecutiveTargetLookingAway()
		{
			if (_dLAgentDecisionOutputHistory.Count < 4)
			{
				return false;
			}
			return _dLAgentDecisionOutputHistory.TakeLast(4).ToList().All((DLAgentDecisionOutput decision) => decision.Decision == DLDecisionType.TargetLookingAway);
		}

		private bool CheckTimeToLookTarget()
		{
			if (!_checkForLookTarget)
			{
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.LookTarget, _lookExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookTarget;
				return true;
			}
			if (!CheckOutputHistory(DLDecisionType.LookTarget, _lookCooldownTime + _lookExecutionTime, forMovement: true))
			{
				return false;
			}
			if (UnityEngine.Random.Range(0f, 100f) < _lookProbability)
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookTarget;
				_lookExecutionTime = UnityEngine.Random.Range(_lookDurationRange.x, _lookDurationRange.y);
				AddDecisionOutput(forMovement: true);
				return true;
			}
			_dLAgentDecisionForMovement.Decision = DLDecisionType.LookTarget;
			_lookExecutionTime = 0f;
			AddDecisionOutput(forMovement: true);
			_dLAgentDecisionForMovement.Decision = DLDecisionType.None;
			return false;
		}

		private bool CheckStationaryState(float deltaTime)
		{
			if (!_checkForStationaryState)
			{
				return false;
			}
			if (_movementAgent.DeadzoneStopActive)
			{
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.MoveToTarget, _forceMoveWhenStationaryExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.MoveToTarget;
				return true;
			}
			if (!CheckOutputHistory(DLDecisionType.TooFarAwayFromTarget, _tooFarAwayFromTargetExecutionTime, forMovement: true))
			{
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.MoveToTarget, _forceMoveWhenStationaryExecutionTime + _forceMoveWhenStationaryCooldownTime, forMovement: true))
			{
				return false;
			}
			if (_dLAgentDecisionOutput.Decision == DLDecisionType.EmoteRespond || _dLAgentDecisionOutput.Decision == DLDecisionType.EmoteRespondPending || _dLAgentDecisionOutput.Decision == DLDecisionType.EmoteSuggest)
			{
				return false;
			}
			if (_distanceMoved < 0.01f)
			{
				_timeStationary += deltaTime;
			}
			else
			{
				_timeStationary = 0f;
			}
			if (_timeStationary >= _timeStationaryLimit)
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.MoveToTarget;
				AddDecisionOutput(forMovement: true);
				return true;
			}
			return false;
		}

		private bool CheckRunawayFromMonsters()
		{
			if (!_checkForRunawayFromMonsters)
			{
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.RunawayFromMonster, _runawayFromMonstersExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.RunawayFromMonster;
				return true;
			}
			if (!CheckOutputHistory(DLDecisionType.LookTarget, _lookExecutionTime, forMovement: true))
			{
				return false;
			}
			if (CheckMonsterInSight())
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.RunawayFromMonster;
				AddDecisionOutput(forMovement: true);
				return true;
			}
			return false;
		}

		public bool CheckMonsterInSight()
		{
			List<VCreature> creaturesInRange = _creature.VRoom.GetCreaturesInRange(_creature, _monsterDetectRange);
			float num = Hub.s.dataman.ExcelDataManager.Consts.C_NavYThreshold;
			float f = _creature.Position.yaw * (MathF.PI / 180f);
			_tempVector1.Set(Mathf.Sin(f), 0f, Mathf.Cos(f));
			Vector3 tempVector = _tempVector1;
			Vector3 positionVector = _creature.PositionVector;
			foreach (VCreature item in creaturesInRange)
			{
				if (item.ActorType != ActorType.Monster || !item.IsAliveStatus() || Mathf.Abs(positionVector.y - item.PositionVector.y) > num)
				{
					continue;
				}
				MonsterInfo? monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(item.MasterID);
				if (monsterInfo != null && !monsterInfo.IsMimic())
				{
					_tempVector2 = item.PositionVector - positionVector;
					_tempVector3.Set(_tempVector2.x, 0f, _tempVector2.z);
					_tempVector3.Normalize();
					if (Vector3.Angle(tempVector, _tempVector3) <= 30f)
					{
						return true;
					}
				}
			}
			return false;
		}

		private void TickBaseBehavior()
		{
			if (!_useBaseBehavior)
			{
				return;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			RefreshEnvironmentSnapshot();
			if (_isOutdoorSnapshot)
			{
				if (_outdoorAreaSnapshot != OutdoorArea.TramInside)
				{
					_stayInTramTimer = 0f;
				}
				if (_outdoorAreaSnapshot == OutdoorArea.TramInside)
				{
					BaseBehavior wanted = ((UnityEngine.Random.value < _exitTramProb) ? BaseBehavior.ExitTram : BaseBehavior.StayInTram);
					TryScheduleOrSwitch(wanted, currentTickMilliSec);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.MainDoorOutside && _baseBehavior != BaseBehavior.ToTram && _baseBehavior != BaseBehavior.ToTramViaHelper)
				{
					BaseBehavior wanted2 = ((UnityEngine.Random.value < _moveToTramViaHelperProb) ? BaseBehavior.ToTramViaHelper : BaseBehavior.ToTram);
					TryScheduleOrSwitch(wanted2, currentTickMilliSec);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.BackDoorOutside && _baseBehavior != BaseBehavior.ToTramFromBackdoor)
				{
					TryScheduleOrSwitch(BaseBehavior.ToTramFromBackdoor, currentTickMilliSec);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.Tram)
				{
					BaseBehavior wanted3 = ((UnityEngine.Random.value < _enterTramProb) ? BaseBehavior.EnterTram : BaseBehavior.ToEntrance);
					TryScheduleOrSwitch(wanted3, currentTickMilliSec);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.MainStreet)
				{
					BaseBehavior wanted4 = ((UnityEngine.Random.value < 0.5f) ? BaseBehavior.ToEntrance : BaseBehavior.ToTram);
					TryScheduleOrSwitch(wanted4, currentTickMilliSec);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.MainStreetFromBackdoor)
				{
					TryScheduleOrSwitch(BaseBehavior.ToTramFromBackdoor, currentTickMilliSec);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.Other)
				{
					TryScheduleOrSwitch(BaseBehavior.OutsideOther, currentTickMilliSec);
				}
				if (currentTickMilliSec >= _forceSwitchAtMs)
				{
					if (_queuedBehavior.HasValue)
					{
						BeginBehavior(_queuedBehavior.Value, currentTickMilliSec);
					}
					else if (_outdoorAreaSnapshot == OutdoorArea.Tram)
					{
						BaseBehavior b = ((_baseBehavior == BaseBehavior.EnterTram) ? BaseBehavior.ToEntrance : BaseBehavior.EnterTram);
						BeginBehavior(b, currentTickMilliSec);
					}
					else if (_outdoorAreaSnapshot == OutdoorArea.TramInside)
					{
						BaseBehavior b2 = ((_baseBehavior == BaseBehavior.StayInTram) ? BaseBehavior.ExitTram : BaseBehavior.StayInTram);
						BeginBehavior(b2, currentTickMilliSec);
					}
					else
					{
						BeginBehavior(_baseBehavior, currentTickMilliSec);
					}
				}
			}
			else if (_baseStartedAtMs < 0)
			{
				BaseBehavior b3 = ((_indoorAreaPolicy == IndoorAreaPolicy.ForceMoveOnly) ? BaseBehavior.MoveToNextRoom : BaseBehavior.Stay);
				BeginBehavior(b3, currentTickMilliSec);
				_baseDurationSec = ((_baseBehavior == BaseBehavior.Stay) ? UnityEngine.Random.Range(_baseStayDurationRange.x, _baseStayDurationRange.y) : UnityEngine.Random.Range(_baseMoveDurationRange.x, _baseMoveDurationRange.y));
			}
			else if ((float)(currentTickMilliSec - _baseStartedAtMs) * 0.001f >= _baseDurationSec)
			{
				switch (_indoorAreaPolicy)
				{
				case IndoorAreaPolicy.ForceMoveOnly:
					BeginBehavior(BaseBehavior.MoveToNextRoom, currentTickMilliSec);
					break;
				case IndoorAreaPolicy.ForceStayOnly:
					BeginBehavior(BaseBehavior.Stay, currentTickMilliSec);
					break;
				default:
					_baseBehavior = ((_baseBehavior == BaseBehavior.Stay) ? BaseBehavior.MoveToNextRoom : BaseBehavior.Stay);
					_baseStartedAtMs = currentTickMilliSec;
					break;
				}
				_baseDurationSec = ((_baseBehavior == BaseBehavior.Stay) ? UnityEngine.Random.Range(_baseStayDurationRange.x, _baseStayDurationRange.y) : UnityEngine.Random.Range(_baseMoveDurationRange.x, _baseMoveDurationRange.y));
			}
		}

		private DLDecisionType GetBaseMovementDecision()
		{
			if (!_useBaseBehavior)
			{
				return DLDecisionType.Stay;
			}
			TickBaseBehavior();
			BaseBehavior baseBehavior = _baseBehavior;
			switch (baseBehavior)
			{
			case BaseBehavior.Stay:
				return DLDecisionType.Stay;
			case BaseBehavior.MoveToNextRoom:
				return DLDecisionType.MoveToNextRoom;
			case BaseBehavior.ToTram:
				return DLDecisionType.MoveToTram;
			case BaseBehavior.ToTramFromBackdoor:
				return DLDecisionType.MoveToTramFromBackdoor;
			case BaseBehavior.ToEntrance:
				return DLDecisionType.MoveToEntrance;
			case BaseBehavior.StayInTram:
				return DLDecisionType.StayInTram;
			case BaseBehavior.OutsideOther:
				return DLDecisionType.OutsideOther;
			case BaseBehavior.EnterTram:
				return DLDecisionType.EnterTram;
			case BaseBehavior.ExitTram:
				return DLDecisionType.ExitTram;
			case BaseBehavior.ToTramViaHelper:
				return DLDecisionType.MoveToTramViaHelper;
			default:
			{
				global::_003CPrivateImplementationDetails_003E.ThrowSwitchExpressionException(baseBehavior);
				DLDecisionType result = default(DLDecisionType);
				return result;
			}
			}
		}

		private bool CheckForcedRotation()
		{
			if (!_checkForForceRotation)
			{
				return false;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			bool flag = CheckWallsInFront();
			if (!CooldownPassedMs(_dLAgentDecisionForForcedRotation.DecisionTime, _forceRotationCooldownTime, currentTickMilliSec))
			{
				return false;
			}
			if (flag)
			{
				_dLAgentDecisionForForcedRotation.Decision = DLDecisionType.ForceRotation;
				_dLAgentDecisionForForcedRotation.DecisionTime = currentTickMilliSec;
				return true;
			}
			return false;
		}

		private bool CheckWallsInFront()
		{
			_tempVector1 = _creature.PositionVector;
			_tempVector1.y += 1f;
			_tempVector2 = Quaternion.Euler(0f, _creature.Position.yaw, 0f) * Vector3.forward;
			bool flag = false;
			if (Physics.SphereCast(_tempVector1, 0.2f, _tempVector2, out var hitInfo, _wallDetectRange, Hub.AllWallsLayerMaskForMimic, QueryTriggerInteraction.Ignore) && hitInfo.distance < _wallDetectRange && !IsClosedDoorHit(hitInfo.collider))
			{
				flag = true;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (flag)
			{
				if ((float)_forceRotationHoldTime == 0f)
				{
					_forceRotationHoldTime = currentTickMilliSec;
				}
				if (currentTickMilliSec - _forceRotationHoldTime >= (long)(_forceRotationHoldDuration * 1000f))
				{
					return true;
				}
				return false;
			}
			_forceRotationHoldTime = 0L;
			return false;
		}

		private bool IsClosedDoorHit(Collider hitCollider)
		{
			if (_realWorldRaySensor == null)
			{
				return false;
			}
			_realWorldRaySensor.CollectDoorColliders(_creature.PositionVector);
			return _realWorldRaySensor.IsDoorHit(hitCollider);
		}

		private bool CheckRunning()
		{
			if (!_checkForRun)
			{
				return false;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			float num = ElapsedSecFromMs(_dLAgentDecisionForRunning.DecisionTime, currentTickMilliSec);
			if (num < _runExecutionTime)
			{
				return true;
			}
			if (_indoorAreaSnapshot == SpeechType_Area.TrapCorridor)
			{
				_dLAgentDecisionForRunning.Decision = DLDecisionType.Run;
				_dLAgentDecisionForRunning.DecisionTime = currentTickMilliSec;
				_runExecutionTime = UnityEngine.Random.Range(_runDurationRange.x, _runDurationRange.y);
				_runCooldownTime = UnityEngine.Random.Range(_runCooldownTimeRange.x, _runCooldownTimeRange.y);
				return true;
			}
			if (num < _runExecutionTime + _runCooldownTime)
			{
				_dLAgentDecisionForRunning.Decision = DLDecisionType.None;
				return false;
			}
			if (_dLAgentDecisionForMovement.Decision == DLDecisionType.MoveToNextRoom && CheckForCircularPattern())
			{
				_dLAgentDecisionForRunning.Decision = DLDecisionType.Run;
				_dLAgentDecisionForRunning.DecisionTime = currentTickMilliSec;
				_runExecutionTime = UnityEngine.Random.Range(_runDurationRange.x, _runDurationRange.y);
				_runCooldownTime = UnityEngine.Random.Range(_runCooldownTimeRange.x, _runCooldownTimeRange.y);
				_yawDeltaHistory.Clear();
				return true;
			}
			if (UnityEngine.Random.Range(0f, 100f) < _runProbability)
			{
				_dLAgentDecisionForRunning.Decision = DLDecisionType.Run;
				_dLAgentDecisionForRunning.DecisionTime = currentTickMilliSec;
				_runExecutionTime = UnityEngine.Random.Range(_runDurationRange.x, _runDurationRange.y);
				_runCooldownTime = UnityEngine.Random.Range(_runCooldownTimeRange.x, _runCooldownTimeRange.y);
				return true;
			}
			_dLAgentDecisionForRunning.Decision = DLDecisionType.None;
			_dLAgentDecisionForRunning.DecisionTime = currentTickMilliSec;
			_runExecutionTime = 0f;
			_runCooldownTime = UnityEngine.Random.Range(_runCooldownTimeRange.x, _runCooldownTimeRange.y);
			return false;
		}

		private bool CheckJumping()
		{
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (!_checkForJump)
			{
				_dLAgentDecisionForJumping.Decision = DLDecisionType.None;
				_dLAgentDecisionForJumping.DecisionTime = currentTickMilliSec;
				_jumpExecutionTime = 0f;
				return false;
			}
			float num = ElapsedSecFromMs(_dLAgentDecisionForJumping.DecisionTime, currentTickMilliSec);
			if (num < _jumpExecutionTime)
			{
				return true;
			}
			if (num < _jumpExecutionTime + _jumpCooldownTime)
			{
				_dLAgentDecisionForJumping.Decision = DLDecisionType.None;
				return false;
			}
			VCreature creature = _creature;
			AttachController attachController = creature?.AttachControlUnit;
			if (creature == null || attachController == null)
			{
				return false;
			}
			if (attachController.IsAttaching())
			{
				_dLAgentDecisionForJumping.Decision = DLDecisionType.None;
				_dLAgentDecisionForJumping.DecisionTime = currentTickMilliSec;
				_jumpExecutionTime = 0f;
				return false;
			}
			if (UnityEngine.Random.Range(0f, 100f) < _jumpProbability)
			{
				_dLAgentDecisionForJumping.Decision = DLDecisionType.Jump;
				_dLAgentDecisionForJumping.DecisionTime = currentTickMilliSec;
				_jumpExecutionTime = UnityEngine.Random.Range(_jumpDurationRange.x, _jumpDurationRange.y);
				return true;
			}
			_dLAgentDecisionForJumping.Decision = DLDecisionType.None;
			_dLAgentDecisionForJumping.DecisionTime = currentTickMilliSec;
			_jumpExecutionTime = 0f;
			return false;
		}

		private bool CheckTooFarAwayFromTarget()
		{
			if (!_checkForTooFarAwayFromTarget)
			{
				return false;
			}
			VCreature creature = _creature;
			AIController aIController = creature?.AIControlUnit;
			if (creature == null || aIController == null)
			{
				return false;
			}
			VCreature target = aIController.GetTarget();
			if (target == null)
			{
				return false;
			}
			if (_indoorAreaSnapshot == SpeechType_Area.TrapCorridor || _indoorAreaSnapshot == SpeechType_Area.TrapWeight)
			{
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.TooFarAwayFromTarget, _tooFarAwayFromTargetExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.TooFarAwayFromTarget;
				return true;
			}
			if (UnityEngine.Random.Range(0f, 100f) < _tooFarAwayFromTargetProbability && !IsWithinNavigationRange(target.PositionVector, _tooFarAwayFromTargetDistance))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.TooFarAwayFromTarget;
				_tooFarAwayFromTargetExecutionTime = UnityEngine.Random.Range(_tooFarAwayFromTargetDurationRange.x, _tooFarAwayFromTargetDurationRange.y);
				AddDecisionOutput(forMovement: true);
				return true;
			}
			return false;
		}

		public void SetChargingCompleted()
		{
			ProtoActor component = base.transform.parent.GetComponent<ProtoActor>();
			if (!(component == null))
			{
				component.AddIncomingEvent(SpeechEvent_IncomingType.ChargeCompleted, Time.realtimeSinceStartup + component.GetIncomingEventExpireTime(SpeechEvent_IncomingType.ChargeCompleted));
			}
		}

		private bool CheckUseShutterSwitch()
		{
			if (!TryEnterEvaluationWindow(DLDecisionType.UseShutterSwitch, _useShutterSwitchCooltime))
			{
				return false;
			}
			if (!Chance01(_useShutterSwitchProb))
			{
				return false;
			}
			List<ILevelObjectInfo> list = _creature.VRoom.FindLevelObjectsByType(_creature.Position.toVector3(), LevelObjectClientType.ShutterSwitch, checkHeight: true, AIRangeType.Absolute);
			if (list.Count == 0)
			{
				return false;
			}
			Vector3 posB = _creature.Position.toVector3();
			foreach (ILevelObjectInfo item in list)
			{
				if (Misc.Distance(item.Pos, posB) <= 7f)
				{
					_dLAgentDecisionOutput.Decision = DLDecisionType.UseShutterSwitch;
					return true;
				}
			}
			return false;
		}

		public void ResetDLDecision()
		{
			_dLAgentDecisionOutput.Reset();
			_prevPosition = Vector3.zero;
			_timeStationary = 0f;
		}

		public void SetJustUsedScrapScanner()
		{
			_justUsedScanner = true;
		}

		private bool TryEnterEvaluationWindow(DLDecisionType type, float intervalSec, bool immediateFirst = true, bool jitterFirst = false)
		{
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			long num = (long)(intervalSec * 1000f);
			if (num <= 0)
			{
				return true;
			}
			if (!_lastEvalTimeMs.TryGetValue(type, out var value))
			{
				if (immediateFirst)
				{
					_lastEvalTimeMs[type] = currentTickMilliSec;
					return true;
				}
				if (jitterFirst && num > 0)
				{
					long num2 = UnityEngine.Random.Range(0, (int)num);
					_lastEvalTimeMs[type] = currentTickMilliSec - num2;
				}
				else
				{
					_lastEvalTimeMs[type] = currentTickMilliSec;
				}
				return false;
			}
			if (currentTickMilliSec - value >= num)
			{
				_lastEvalTimeMs[type] = currentTickMilliSec;
				return true;
			}
			return false;
		}

		private static bool Chance01(float p01)
		{
			p01 = Mathf.Clamp01(p01);
			if (p01 <= 0f)
			{
				return false;
			}
			if (p01 >= 1f)
			{
				return true;
			}
			return UnityEngine.Random.value < p01;
		}

		private void RefreshEnvironmentSnapshot()
		{
			if (Hub.s.dLAcademyManager.GetAreaForDL(_creature.PositionVector, out var areaType) == 1)
			{
				_isOutdoorSnapshot = true;
				_outdoorAreaSnapshot = Hub.s.dLAcademyManager.GetOutdoorArea(_creature.PositionVector);
				_indoorAreaSnapshot = SpeechType_Area.Indoor;
				return;
			}
			_isOutdoorSnapshot = false;
			_outdoorAreaSnapshot = OutdoorArea.None;
			_indoorAreaSnapshot = areaType;
			if (_indoorAreaSnapshot == SpeechType_Area.TrapCorridor || _indoorAreaSnapshot == SpeechType_Area.TrapWeight)
			{
				_indoorAreaPolicy = IndoorAreaPolicy.ForceMoveOnly;
			}
			else
			{
				_indoorAreaPolicy = IndoorAreaPolicy.DefaultToggle;
			}
		}

		private void InitBaseBehaviorAccordingToEnvironment()
		{
			long valueOrDefault = (Hub.s?.timeutil?.GetCurrentTickMilliSec()).GetValueOrDefault();
			RefreshEnvironmentSnapshot();
			if (_isOutdoorSnapshot)
			{
				if (_outdoorAreaSnapshot == OutdoorArea.TramInside)
				{
					BeginBehavior((UnityEngine.Random.value < _exitTramProb) ? BaseBehavior.ExitTram : BaseBehavior.StayInTram, valueOrDefault);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.MainDoorOutside)
				{
					BeginBehavior((UnityEngine.Random.value < _moveToTramViaHelperProb) ? BaseBehavior.ToTramViaHelper : BaseBehavior.ToTram, valueOrDefault);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.Tram)
				{
					BeginBehavior((UnityEngine.Random.value < _enterTramProb) ? BaseBehavior.EnterTram : BaseBehavior.ToEntrance, valueOrDefault);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.MainStreet)
				{
					BeginBehavior((UnityEngine.Random.value < 0.5f) ? BaseBehavior.ToEntrance : BaseBehavior.ToTram, valueOrDefault);
				}
				else if (_outdoorAreaSnapshot == OutdoorArea.MainStreetFromBackdoor || _outdoorAreaSnapshot == OutdoorArea.BackDoorOutside)
				{
					BeginBehavior(BaseBehavior.ToTramFromBackdoor, valueOrDefault);
				}
				else
				{
					BeginBehavior(BaseBehavior.OutsideOther, valueOrDefault);
				}
				if (_outdoorAreaSnapshot != OutdoorArea.TramInside)
				{
					_stayInTramTimer = 0f;
				}
			}
			else
			{
				BaseBehavior b = ((_indoorAreaPolicy == IndoorAreaPolicy.ForceMoveOnly) ? BaseBehavior.MoveToNextRoom : BaseBehavior.Stay);
				BeginBehavior(b, valueOrDefault);
				_baseStartedAtMs = valueOrDefault;
				_baseDurationSec = ((_baseBehavior == BaseBehavior.Stay) ? UnityEngine.Random.Range(_baseStayDurationRange.x, _baseStayDurationRange.y) : UnityEngine.Random.Range(_baseMoveDurationRange.x, _baseMoveDurationRange.y));
			}
		}

		private (float min, float max) GetRange(BaseBehavior b)
		{
			return b switch
			{
				BaseBehavior.ToTram => (min: _toTramDurationRange.x, max: _toTramDurationRange.y), 
				BaseBehavior.ToTramFromBackdoor => (min: _toTramDurationRange.x, max: _toTramDurationRange.y), 
				BaseBehavior.ToEntrance => (min: _toEntranceDurationRange.x, max: _toEntranceDurationRange.y), 
				BaseBehavior.EnterTram => (min: _enterTramDurationRange.x, max: _enterTramDurationRange.y), 
				BaseBehavior.ExitTram => (min: _exitTramDurationRange.x, max: _exitTramDurationRange.y), 
				BaseBehavior.OutsideOther => (min: _outsideOtherDurationRange.x, max: _outsideOtherDurationRange.y), 
				BaseBehavior.StayInTram => (min: _insideTramDurationRange.x, max: _insideTramDurationRange.y), 
				BaseBehavior.Stay => (min: _baseStayDurationRange.x, max: _baseStayDurationRange.y), 
				BaseBehavior.MoveToNextRoom => (min: _baseMoveDurationRange.x, max: _baseMoveDurationRange.y), 
				BaseBehavior.ToTramViaHelper => (min: _toTramViaHelperDurationRange.x, max: _toTramViaHelperDurationRange.y), 
				_ => (min: _outsideOtherDurationRange.x, max: _outsideOtherDurationRange.y), 
			};
		}

		private void BeginBehavior(BaseBehavior b, long nowMs)
		{
			_baseBehavior = b;
			_baseStartedAtMs = nowMs;
			(float min, float max) range = GetRange(b);
			float item = range.min;
			float item2 = range.max;
			_earliestSwitchAtMs = nowMs + (long)(item * 1000f);
			_forceSwitchAtMs = nowMs + (long)(item2 * 1000f);
			_queuedBehavior = null;
		}

		private bool TryScheduleOrSwitch(BaseBehavior wanted, long nowMs, bool urgent = false)
		{
			if (_baseBehavior == wanted)
			{
				return false;
			}
			if (urgent || nowMs >= _earliestSwitchAtMs)
			{
				BeginBehavior(wanted, nowMs);
				return true;
			}
			_queuedBehavior = wanted;
			return false;
		}

		private void SampleYawForCircularPattern()
		{
			if (_creature == null)
			{
				return;
			}
			float yaw = _creature.Position.yaw;
			if (float.IsNaN(_lastYawForCircularPattern))
			{
				_lastYawForCircularPattern = yaw;
				return;
			}
			float num = Mathf.DeltaAngle(_lastYawForCircularPattern, yaw);
			_lastYawForCircularPattern = yaw;
			if (!(Mathf.Abs(num) < 0.1f))
			{
				_yawDeltaHistory.Enqueue(num);
				while ((float)_yawDeltaHistory.Count > _circleYawDeltaHistorySize)
				{
					_yawDeltaHistory.Dequeue();
				}
			}
		}

		private bool CheckForCircularPattern()
		{
			if (!_checkForCircularPattern)
			{
				return false;
			}
			int num = Mathf.Max(5, (int)(_circleYawDeltaHistorySize * 0.5f));
			if (_yawDeltaHistory.Count < num)
			{
				return false;
			}
			int num2 = 0;
			float[] array = _yawDeltaHistory.ToArray();
			for (int i = 0; i < array.Length; i++)
			{
				float num3 = Mathf.Abs(array[i]);
				int num4 = 0;
				for (int j = 0; j < array.Length; j++)
				{
					float num5 = Mathf.Abs(array[j]);
					if (Mathf.Abs(num3 - num5) <= _circularMovementEpsilon)
					{
						num4++;
					}
				}
				if ((float)num4 >= (float)array.Length * _circularMovementThreshold)
				{
					num2 = num4;
					break;
				}
			}
			return (float)num2 >= (float)array.Length * _circularMovementThreshold;
		}

		private DLInterestedBTType GetCurrentBTType()
		{
			AIController aIController = _creature?.AIControlUnit;
			if (aIController == null)
			{
				return DLInterestedBTType.None;
			}
			string currentBTTemplateName = aIController.CurrentBTTemplateName;
			if (currentBTTemplateName.Contains("beware"))
			{
				return DLInterestedBTType.Beware;
			}
			if (currentBTTemplateName.Contains("Friendly"))
			{
				return DLInterestedBTType.CheckFriendly;
			}
			return DLInterestedBTType.None;
		}

		public void ResetDLTimeToAttackTimer()
		{
			_timeToAttackTimer = 0f;
		}

		private bool CheckLookAtScrapScanner()
		{
			if (!_checkForLookAtScanner)
			{
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.LookAtStashHanger, _lookAtStashHangerExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtStashHanger;
				return true;
			}
			if (!CheckOutputHistory(DLDecisionType.LookAtScrapScanner, _lookAtScannerExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtScrapScanner;
				return true;
			}
			if (!_creature.VRoom.IsTramUpgradeActive(2))
			{
				return false;
			}
			List<ILevelObjectInfo> list = _creature.VRoom.FindLevelObjectsByType(_creature.Position.toVector3(), LevelObjectClientType.ScrapScan, checkHeight: true, AIRangeType.Absolute);
			if (list.Count == 0)
			{
				return false;
			}
			Vector3 targetPos = _creature.PositionVector;
			bool flag = false;
			foreach (ILevelObjectInfo item in list)
			{
				if (item is StateLevelObjectInfo { CurrentState: 1 })
				{
					flag = true;
					targetPos = item.Pos;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			if (_justUsedScanner)
			{
				_justUsedScanner = false;
				if (Chance01(_useLookAtScannerProb))
				{
					_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtScrapScanner;
					_dLAgentDecisionForMovement.TargetPos = targetPos;
					_lookAtScannerExecutionTime = UnityEngine.Random.Range(_useLookAtScannerDurationRange.x, _useLookAtScannerDurationRange.y);
					AddDecisionOutput(forMovement: true);
					ProtoActor component = base.transform.parent.GetComponent<ProtoActor>();
					if (component != null)
					{
						component.AddIncomingEvent(SpeechEvent_IncomingType.LookAtScrapScanner, Time.realtimeSinceStartup + component.GetIncomingEventExpireTime(SpeechEvent_IncomingType.LookAtScrapScanner));
					}
					return true;
				}
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtScrapScanner;
				_lookAtScannerExecutionTime = 0f;
				AddDecisionOutput(forMovement: true);
				_dLAgentDecisionForMovement.Decision = DLDecisionType.None;
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.LookAtScrapScanner, _lookAtScannerCooltime + _lookAtScannerExecutionTime, forMovement: true))
			{
				return false;
			}
			if (_stayInTramTimer < _stayInTramBeforeLookAtScannerDuration)
			{
				return false;
			}
			if (Chance01(_lookAtScannerProb))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtScrapScanner;
				_dLAgentDecisionForMovement.TargetPos = targetPos;
				_lookAtScannerExecutionTime = UnityEngine.Random.Range(_lookAtScannerDurationRange.x, _lookAtScannerDurationRange.y);
				AddDecisionOutput(forMovement: true);
				ProtoActor component2 = base.transform.parent.GetComponent<ProtoActor>();
				if (component2 != null)
				{
					component2.AddIncomingEvent(SpeechEvent_IncomingType.LookAtScrapScanner, Time.realtimeSinceStartup + component2.GetIncomingEventExpireTime(SpeechEvent_IncomingType.LookAtScrapScanner));
				}
				return true;
			}
			_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtScrapScanner;
			_lookAtScannerExecutionTime = 0f;
			AddDecisionOutput(forMovement: true);
			_dLAgentDecisionForMovement.Decision = DLDecisionType.None;
			return false;
		}

		private bool CheckUseScrapScanner()
		{
			if (_stayInTramTimer < _stayInTramBeforeUseScannerDuration)
			{
				return false;
			}
			if (!_creature.VRoom.IsTramUpgradeActive(2))
			{
				return false;
			}
			List<ILevelObjectInfo> list = _creature.VRoom.FindLevelObjectsByType(_creature.Position.toVector3(), LevelObjectClientType.ScrapScan, checkHeight: true, AIRangeType.Absolute);
			if (list.Count == 0)
			{
				return false;
			}
			bool flag = false;
			foreach (ILevelObjectInfo item in list)
			{
				if (item is StateLevelObjectInfo { CurrentState: 0 })
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			if (!TryEnterEvaluationWindow(DLDecisionType.UseScrapScanner, _useScrapScannerCooltime))
			{
				return false;
			}
			if (!Chance01(_useScrapScannerProb))
			{
				return false;
			}
			_dLAgentDecisionOutput.Decision = DLDecisionType.UseScrapScanner;
			return true;
		}

		private bool CheckStashHanger()
		{
			if (!_checkForLookAtStashHanger)
			{
				return false;
			}
			if (!CheckOutputHistory(DLDecisionType.LookAtScrapScanner, _lookAtScannerExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtScrapScanner;
				return true;
			}
			if (!CheckOutputHistory(DLDecisionType.LookAtStashHanger, _lookAtStashHangerExecutionTime, forMovement: true))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtStashHanger;
				return true;
			}
			if (!CheckOutputHistory(DLDecisionType.LookAtStashHanger, _lookAtStashHangerExecutionTime + _lookAtStashHangerCooltime, forMovement: true))
			{
				return false;
			}
			if (!_creature.VRoom.IsTramUpgradeActive(4))
			{
				return false;
			}
			List<ILevelObjectInfo> list = _creature.VRoom.FindLevelObjectsByType(_creature.Position.toVector3(), LevelObjectClientType.StashHanger, checkHeight: true, AIRangeType.Absolute);
			if (list.Count == 0)
			{
				return false;
			}
			Vector3 vector = _creature.Position.toVector3();
			List<Vector3> list2 = new List<Vector3>();
			foreach (ILevelObjectInfo item in list)
			{
				float num = item.Pos.x - vector.x;
				float num2 = item.Pos.z - vector.z;
				if (!(Mathf.Sqrt(num * num + num2 * num2) > 2.5f))
				{
					StashHangerLevelObject stashHangerLevelObject = item.DataOrigin as StashHangerLevelObject;
					if (stashHangerLevelObject != null && stashHangerLevelObject.IsScrapHanging())
					{
						list2.Add(item.Pos);
					}
				}
			}
			if (list2.Count == 0)
			{
				return false;
			}
			Vector3 targetPos = list2[UnityEngine.Random.Range(0, list2.Count)];
			if (Chance01(_lookAtStashHangerProb))
			{
				_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtStashHanger;
				_dLAgentDecisionForMovement.TargetPos = targetPos;
				_lookAtStashHangerExecutionTime = UnityEngine.Random.Range(_lookAtStashHangerDurationRange.x, _lookAtStashHangerDurationRange.y);
				AddDecisionOutput(forMovement: true);
				return true;
			}
			_dLAgentDecisionForMovement.Decision = DLDecisionType.LookAtStashHanger;
			_lookAtStashHangerExecutionTime = 0f;
			AddDecisionOutput(forMovement: true);
			_dLAgentDecisionForMovement.Decision = DLDecisionType.None;
			return false;
		}

		private bool CheckDropFakeItems()
		{
			if (!Hub.s.dLAcademyManager.IsInsideInsideTram(_creature.PositionVector))
			{
				return false;
			}
			if (!TryEnterEvaluationWindow(DLDecisionType.DropFakeItem, _dropFakeItemCooltime))
			{
				return false;
			}
			if (!Chance01(_dropFakeItemProb))
			{
				return false;
			}
			bool flag = false;
			foreach (VCreature item in _creature.VRoom.GetCreaturesInRange(_creature, 50f))
			{
				if (item.ActorType == ActorType.Player && Hub.s.dLAcademyManager.GetOutdoorArea(item.PositionVector) == OutdoorArea.TramInside)
				{
					flag = true;
					break;
				}
			}
			if (!flag)
			{
				return false;
			}
			bool hasFakeItemInTram = false;
			_creature.VRoom.IterateAllActor(delegate(VActor actor)
			{
				if (actor is VLootingObject vLootingObject && vLootingObject.IsFake() && Hub.s.dLAcademyManager.GetOutdoorArea(vLootingObject.PositionVector) == OutdoorArea.TramInside)
				{
					hasFakeItemInTram = true;
				}
			});
			if (hasFakeItemInTram)
			{
				return false;
			}
			_dLAgentDecisionOutput.Decision = DLDecisionType.DropFakeItem;
			return true;
		}

		private void CancelCheckFriendly()
		{
			_checkFriendlyExecutionTime = 0f;
		}
	}
}
