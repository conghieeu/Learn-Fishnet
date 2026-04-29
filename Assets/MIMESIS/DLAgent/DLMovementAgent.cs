using System;
using System.Collections.Generic;
using DunGen;
using Mimic.Actors;
using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;
using UnityEngine;

namespace DLAgent
{
	public class DLMovementAgent : Agent
	{
		[Serializable]
		public class BehaviorContext
		{
			public List<ModelProfile> modelProfiles = new List<ModelProfile>();

			public int ProfileCount => modelProfiles?.Count ?? 0;

			public ModelProfile GetRandomProfile()
			{
				if (modelProfiles == null || modelProfiles.Count == 0)
				{
					return null;
				}
				return modelProfiles[UnityEngine.Random.Range(0, modelProfiles.Count)];
			}

			public ModelProfile GetProfile(int index)
			{
				if (modelProfiles == null || index < 0 || index >= modelProfiles.Count)
				{
					return null;
				}
				return modelProfiles[index];
			}
		}

		private enum ModelKey
		{
			Stay = 0,
			Explore = 1,
			Follower = 2,
			Runaway = 3,
			ToTram = 4,
			ToEntrance = 5,
			EnterTram = 6,
			ExitTram = 7,
			InsideTram = 8,
			OutsideOther = 9,
			ToTramFromBackdoor = 10,
			ToTramViaHelper = 11,
			LookAtStashHanger = 12,
			LookAtScrapScanner = 13
		}

		public class PredictedAction
		{
			public Vector3 Movement;

			public float Angle;
		}

		[SerializeField]
		private long _lastDLTick;

		private long _initialTickOffset;

		private long _stepInterval = 166L;

		public bool ShowMovement = true;

		public bool ShowMovementFromModel = true;

		public bool UseWorldCoord;

		[SerializeField]
		private bool _showRayGizmo = true;

		[SerializeField]
		private bool _drawPathGizmos;

		[SerializeField]
		private float _pathNodeRadius = 0.15f;

		[SerializeField]
		private Color _pathColor = new Color(0.2f, 0.7f, 1f, 0.9f);

		[SerializeField]
		private Color _startToFirstColor = new Color(0.3f, 1f, 0.6f, 0.9f);

		private readonly Color[] debugColors = new Color[7]
		{
			Color.white,
			Color.red,
			Color.green,
			Color.blue,
			Color.yellow,
			Color.cyan,
			Color.magenta
		};

		public int ObservationStackSize = 10;

		public int ObservationSize = 1410;

		public int PredActionSeqCount = 3;

		public int PredActionSize = 3;

		public float ActionPosScaleFactor = 3f;

		public float ActionAngleScaleFactor = 360f;

		public float ObservationPosScaleFactor = 1f;

		public float ObservationAngleScaleFactor = 360f;

		private float _lambdaForModel = 0.6f;

		private float _storedTargetPitch;

		private bool _pitchTargetInitialized;

		private bool _isPitchPaused;

		private float _pitchPauseTimer;

		[SerializeField]
		private float _pitchStep = 0.5f;

		[SerializeField]
		private Vector2 _pitchRange = new Vector2(-20f, 45f);

		[SerializeField]
		private Vector2 _forceRotationRange = new Vector2(30f, 45f);

		[SerializeField]
		private float _forceRotationAngleBoost = 3f;

		private float _deltaTime;

		private float _deltaTimer;

		private ObservationStacker _observationStack;

		[SerializeField]
		private ProtoActor _me;

		[SerializeField]
		private VCreature _creature;

		[InspectorReadOnly]
		public VCreature TargetCreature;

		[InspectorReadOnly]
		public ProtoActor TargetActor;

		[SerializeField]
		private Vector3 _targetPos = Vector3.zero;

		[SerializeField]
		private float _followerDistanceToTargetTooFar = 1f;

		[SerializeField]
		private float _followerDistanceToTargetTooNear;

		[SerializeField]
		private bool _enableInferenceLog;

		[SerializeField]
		private int _inferenceLogStepCount;

		private int _inferenceLogCurrentFrameIndex;

		private InferenceLogger _inferenceLogger = new InferenceLogger();

		private bool _inRunawayFromMonsters;

		private float _jumpPacketSendTimer;

		private float _jumpPacketSendCooltime = 0.5f;

		private bool _inStationaryMove;

		private bool _inTooFarAwayFromTarget;

		private bool _inLookAtScrapScanner;

		private bool _inLookAtStashHanger;

		[Header("Models — Base")]
		[SerializeField]
		private BehaviorContext _stay;

		[SerializeField]
		private BehaviorContext _explorerLeftHanded;

		[SerializeField]
		private BehaviorContext _explorerRightHanded;

		[SerializeField]
		private BehaviorContext _follower;

		[SerializeField]
		private BehaviorContext _runaway;

		[SerializeField]
		private BehaviorContext _lookAtScrapScanner;

		[SerializeField]
		private BehaviorContext _lookAtStashHanger;

		[SerializeField]
		private BehaviorContext _enterTram;

		[SerializeField]
		private BehaviorContext _stayInTram;

		[SerializeField]
		private BehaviorContext _exitTram;

		[Header("Models — Outdoor A (Map A)")]
		[SerializeField]
		private BehaviorContext _outdoorAToTram;

		[SerializeField]
		private BehaviorContext _outdoorAToEntrance;

		[SerializeField]
		private BehaviorContext _outdoorAOther;

		[SerializeField]
		private BehaviorContext _outdoorAToTramFromBackdoor;

		[SerializeField]
		private BehaviorContext _outdoorAToTramViaHelper;

		[Header("Models — Outdoor B (Map B)")]
		[SerializeField]
		private BehaviorContext _outdoorBToTram;

		[SerializeField]
		private BehaviorContext _outdoorBToEntrance;

		[SerializeField]
		private BehaviorContext _outdoorBOther;

		[Header("Models — Outdoor C (Map C)")]
		[SerializeField]
		private BehaviorContext _outdoorCToTram;

		[SerializeField]
		private BehaviorContext _outdoorCToEntrance;

		[SerializeField]
		private BehaviorContext _outdoorCOther;

		[SerializeField]
		private string _behaviorName = "mimic";

		[SerializeField]
		private bool _pathFinding = true;

		[SerializeField]
		private bool _alignRotationToMovement;

		[SerializeField]
		private bool _movementExtension = true;

		private List<Vector3> _pathList = new List<Vector3>();

		private ModelKey _currentModelKey;

		private DLDecisionType _lastNonOverlayMoveDecision;

		[SerializeField]
		private float _explorerAngleBoostScaleFactor = 2f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _explorerAngleBoostThresholdFrac = 0.7f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _explorerAngleBoostStartFrac = 0.3f;

		[SerializeField]
		[Range(0f, 1f)]
		private float _explorerMoveRotateRatio = 0.5f;

		[SerializeField]
		private bool _useAngleBoost = true;

		[SerializeField]
		private bool _useExplorerMoveRotate = true;

		[SerializeField]
		private bool _rightHanded = true;

		private List<PredictedAction> _actionArray = new List<PredictedAction>();

		private int _zDim = 4;

		private float[] _zFromPrevOutput;

		private List<DLAgentObservationFrameData>? _allFrames;

		private int _currentFrameIndex;

		private DLAgentMovementOutput _dLAgentMovementOutput = new DLAgentMovementOutput();

		public DLAgentMovementOutput _dLAgentMovementOutputRaw = new DLAgentMovementOutput();

		private LineRenderer _lineRenderer;

		[SerializeField]
		private bool _deadzoneStopEnabled = true;

		[SerializeField]
		private float _movementDeadzoneMin = 0.1f;

		[SerializeField]
		private Vector2 _deadzoneStopDurationRange = new Vector2(0.5f, 1.5f);

		private float _deadzoneStopTimer;

		private bool _deadzoneStopActive;

		[SerializeField]
		private float _deadzoneActivateCooltime = 5f;

		private float _deadzoneActivateCooltimeTimer;

		private ProtoActor.NetSyncActorData _netSyncActorData = new ProtoActor.NetSyncActorData();

		[SerializeField]
		private DLDecisionAgent _decisionAgent;

		[SerializeField]
		private float _turnSpeed = 90f;

		[SerializeField]
		private float _angle_deadzone_max = 10f;

		private float _agentStepFrequency = 5f;

		private int _mapIDCached;

		private static readonly Dictionary<DLDecisionType, ModelKey> s_BaseDecisionToModel = new Dictionary<DLDecisionType, ModelKey>
		{
			{
				DLDecisionType.MoveToNextRoom,
				ModelKey.Explore
			},
			{
				DLDecisionType.Stay,
				ModelKey.Stay
			},
			{
				DLDecisionType.MoveToTram,
				ModelKey.ToTram
			},
			{
				DLDecisionType.MoveToEntrance,
				ModelKey.ToEntrance
			},
			{
				DLDecisionType.EnterTram,
				ModelKey.EnterTram
			},
			{
				DLDecisionType.ExitTram,
				ModelKey.ExitTram
			},
			{
				DLDecisionType.OutsideOther,
				ModelKey.OutsideOther
			},
			{
				DLDecisionType.StayInTram,
				ModelKey.InsideTram
			},
			{
				DLDecisionType.MoveToTramFromBackdoor,
				ModelKey.ToTramFromBackdoor
			},
			{
				DLDecisionType.MoveToTramViaHelper,
				ModelKey.ToTramViaHelper
			},
			{
				DLDecisionType.LookAtStashHanger,
				ModelKey.LookAtStashHanger
			}
		};

		private float MinAngle => (_pitchRange.x + 360f) % 360f;

		private float MaxAngle => _pitchRange.y;

		private float PitchSpan => (MaxAngle + 360f - MinAngle) % 360f;

		public RealWorldRaySensor? RaySensor
		{
			get
			{
				if (_me == null)
				{
					return null;
				}
				if (_me.raySensor == null)
				{
					return null;
				}
				return _me.raySensor;
			}
		}

		public DLAgentMovementOutput DLAgentMovementOutput => _dLAgentMovementOutput;

		public bool DeadzoneStopActive => _deadzoneStopActive;

		private float _walkSpeed => (float)_netSyncActorData.MoveSpeedWalk / 100f;

		private float _runSpeed => (float)_netSyncActorData.MoveSpeedRun / 100f;

		public override void Initialize()
		{
			if (_me == null)
			{
				Logger.RError("DLAgent : me is null");
			}
		}

		protected override void OnEnable()
		{
			base.OnEnable();
			_pitchTargetInitialized = false;
			_isPitchPaused = true;
			_dLAgentMovementOutput.Reset();
			_deltaTime = 0f;
			_deltaTimer = 0f;
			_rightHanded = UnityEngine.Random.Range(0, 2) == 0;
			_observationStack = new ObservationStacker(ObservationStackSize, ObservationSize, ObservationAngleScaleFactor, UseWorldCoord);
			_inStationaryMove = false;
			_inTooFarAwayFromTarget = false;
			_inRunawayFromMonsters = false;
			_inLookAtStashHanger = false;
			_inLookAtScrapScanner = false;
			_zFromPrevOutput = new float[_zDim];
			for (int i = 0; i < _zDim; i++)
			{
				_zFromPrevOutput[i] = 0f;
			}
			_lastDLTick = 0L;
			_stepInterval = 166 + UnityEngine.Random.Range(-5, 6);
			_initialTickOffset = UnityEngine.Random.Range(0, (int)_stepInterval);
			_jumpPacketSendCooltime = 0.85f;
			_currentModelKey = ModelKey.Stay;
			SetStayModel();
			_deadzoneStopActive = false;
			_deadzoneStopTimer = 0f;
			_deadzoneActivateCooltimeTimer = 0f;
		}

		public override void CollectObservations(VectorSensor sensor)
		{
			_observationStack.GetStackedObservations(sensor);
			List<float> observation = new List<float>(sensor.GetObservations());
			if (_enableInferenceLog)
			{
				_inferenceLogger.RecordObservation(observation);
			}
		}

		public override void OnEpisodeBegin()
		{
		}

		public void Step()
		{
			if (Hub.s == null || Hub.s.timeutil == null)
			{
				return;
			}
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			if (_lastDLTick == 0L)
			{
				_lastDLTick = currentTickMilliSec - _initialTickOffset;
			}
			long num = currentTickMilliSec - _lastDLTick;
			if (num < _stepInterval)
			{
				return;
			}
			_lastDLTick = currentTickMilliSec - num % _stepInterval;
			if (_observationStack == null || TargetCreature == null)
			{
				return;
			}
			if (_me == null)
			{
				Logger.RError("DLAgent : me is null");
				return;
			}
			if (_me != null && TargetCreature != null)
			{
				if (_enableInferenceLog)
				{
					if (_currentFrameIndex < _inferenceLogStepCount)
					{
						_currentFrameIndex++;
					}
					else
					{
						_inferenceLogger.SaveToFile();
						_enableInferenceLog = false;
					}
				}
				DLAgentObservationFrameData dLAgentObservationFrameData = null;
				dLAgentObservationFrameData = ((_currentModelKey == ModelKey.ToTramFromBackdoor || _currentModelKey == ModelKey.ToTramViaHelper || _currentModelKey == ModelKey.ExitTram || _currentModelKey == ModelKey.InsideTram || _currentModelKey == ModelKey.EnterTram || _currentModelKey == ModelKey.LookAtStashHanger || _currentModelKey == ModelKey.LookAtScrapScanner) ? DLAgentObservationFrameData.MakeFrameData(_me, _creature, _targetPos, _followerDistanceToTargetTooFar, _followerDistanceToTargetTooNear, Vector3.zero, _zFromPrevOutput, _lambdaForModel) : ((_currentModelKey != ModelKey.ToEntrance) ? DLAgentObservationFrameData.MakeFrameData(_me, _creature, TargetCreature, null, _followerDistanceToTargetTooFar, _followerDistanceToTargetTooNear, Vector3.zero, _zFromPrevOutput, _lambdaForModel) : ((_mapIDCached != 15) ? DLAgentObservationFrameData.MakeFrameData(_me, _creature, _targetPos, _followerDistanceToTargetTooFar, _followerDistanceToTargetTooNear, Vector3.zero, _zFromPrevOutput, _lambdaForModel) : DLAgentObservationFrameData.MakeFrameData(_me, _creature, TargetCreature, null, _followerDistanceToTargetTooFar, _followerDistanceToTargetTooNear, Vector3.zero, _zFromPrevOutput, _lambdaForModel))));
				_observationStack.AddObservation(dLAgentObservationFrameData);
			}
			RequestDecision();
		}

		public void StepFromClient()
		{
			if (_observationStack != null && !(TargetActor == null) && !(_me == null))
			{
				_observationStack.AddObservation(DLAgentObservationFrameData.MakeFrameData(_me, null, null, TargetActor, _followerDistanceToTargetTooFar, _followerDistanceToTargetTooNear, TargetActor.transform.rotation.eulerAngles, _zFromPrevOutput, _lambdaForModel));
				RequestDecision();
			}
		}

		public override void OnActionReceived(ActionBuffers actions)
		{
			if (!_dLAgentMovementOutput.isConsumed)
			{
				return;
			}
			_dLAgentMovementOutput.isConsumed = false;
			_dLAgentMovementOutput.calcAngle = false;
			_actionArray.Clear();
			if (_deltaTime == 0f)
			{
				_deltaTime = 1f / _agentStepFrequency;
				_deltaTimer = Time.realtimeSinceStartup;
			}
			else
			{
				_deltaTime = Time.realtimeSinceStartup - _deltaTimer;
				_deltaTimer = Time.realtimeSinceStartup;
			}
			if (_decisionAgent.DLAgentDecisionForMovement.Decision != DLDecisionType.MoveToTarget && _decisionAgent.DLAgentDecisionForMovement.Decision != DLDecisionType.LookTarget && _decisionAgent.DLAgentDecisionForMovement.Decision != DLDecisionType.RunawayFromMonster && _decisionAgent.DLAgentDecisionForMovement.Decision != DLDecisionType.TooFarAwayFromTarget && _decisionAgent.DLAgentDecisionForMovement.Decision != DLDecisionType.LookAtScrapScanner && _decisionAgent.DLAgentDecisionForMovement.Decision != DLDecisionType.LookAtStashHanger)
			{
				_lastNonOverlayMoveDecision = _decisionAgent.DLAgentDecisionForMovement.Decision;
				SetModelIfChanged(MapBaseModel(_lastNonOverlayMoveDecision));
			}
			float actionAngleScaleFactor = ActionAngleScaleFactor;
			float num = Mathf.Max(1E-05f, _deltaTime * _turnSpeed);
			for (int i = 0; i < PredActionSeqCount; i++)
			{
				PredictedAction predictedAction = new PredictedAction();
				Vector3 vector = new Vector3(actions.ContinuousActions[i * 3] * ActionPosScaleFactor, 0f, actions.ContinuousActions[i * 3 + 1] * ActionPosScaleFactor);
				float num2 = actions.ContinuousActions[i * 3 + 2];
				float f = num2 * actionAngleScaleFactor;
				float num3 = 1f;
				if (_useAngleBoost)
				{
					float value = Mathf.Abs(f) / num;
					float num4 = Mathf.Clamp01(_explorerAngleBoostStartFrac);
					float num5 = Mathf.Clamp01(_explorerAngleBoostThresholdFrac);
					if (num5 < num4)
					{
						float num6 = num5;
						num5 = num4;
						num4 = num6;
					}
					if (Mathf.Approximately(num5, num4))
					{
						num5 = Mathf.Min(1f, num4 + 0.001f);
					}
					float t = Mathf.InverseLerp(num4, num5, value);
					num3 = Mathf.Lerp(1f, _explorerAngleBoostScaleFactor, t);
				}
				float num7 = num3 * actionAngleScaleFactor;
				predictedAction.Angle = num2 * num7;
				if (_decisionAgent.DLAgentDecisionForMovement.Decision == DLDecisionType.MoveToNextRoom && _useExplorerMoveRotate)
				{
					float y = Mathf.Clamp(predictedAction.Angle, 0f - num, num) * _explorerMoveRotateRatio;
					vector = Quaternion.Euler(0f, y, 0f) * vector;
				}
				predictedAction.Movement = vector;
				_actionArray.Add(predictedAction);
			}
			for (int j = 0; j < _zDim; j++)
			{
				_zFromPrevOutput[j] = actions.ContinuousActions[PredActionSeqCount * 3 + j];
			}
			if (_creature == null)
			{
				return;
			}
			Vector3 movement = _actionArray[0].Movement;
			float yaw = _creature.Position.yaw;
			Quaternion quaternion = Quaternion.Euler(0f, yaw, 0f);
			Vector3 vector2 = quaternion * movement;
			ObservationStacker.CoordMode coordMode = ObservationStacker.CoordMode.LocalRelativeToCurrent;
			if (UseWorldCoord)
			{
				coordMode = _observationStack.GetCoordMode(_creature.PositionVector);
			}
			vector2 = ((coordMode != ObservationStacker.CoordMode.LocalRelativeToCurrent) ? movement : (quaternion * movement));
			Vector3 vector3 = vector2;
			Vector3 vector4 = _creature.PositionVector + vector2;
			if (_currentModelKey == ModelKey.ToTramFromBackdoor || _currentModelKey == ModelKey.ToTramViaHelper || _currentModelKey == ModelKey.ExitTram)
			{
				vector4.y = _targetPos.y;
			}
			else if (_currentModelKey == ModelKey.ToEntrance && _mapIDCached != 15)
			{
				vector4.y = _targetPos.y;
			}
			if (_decisionAgent.DLAgentDecisionForMovement.Decision == DLDecisionType.TooFarAwayFromTarget)
			{
				vector4.y = TargetCreature.PositionVector.y;
			}
			_pathList.Clear();
			if (_pathFinding)
			{
				if (Vector3.Distance(_creature.PositionVector, vector4) < 0.1f)
				{
					vector2 = Vector3.zero;
				}
				else
				{
					float maxDistance = 1.5f;
					vector2 = ((!_creature.VRoom.FindNearestPoly(vector4, out var nearestPos, maxDistance)) ? Vector3.zero : ((!_creature.MovementControlUnit.FindPath(nearestPos, ref _pathList) || _pathList.Count <= 1) ? Vector3.zero : (_pathList[1] - _creature.PositionVector)));
				}
			}
			float magnitude = vector2.magnitude;
			UpdateRunState();
			float num8 = ((_dLAgentMovementOutput.isRunning == true) ? _runSpeed : _walkSpeed);
			Vector3 vector5;
			if (!_pathFinding && magnitude < _movementDeadzoneMin)
			{
				vector5 = Vector3.zero;
			}
			else
			{
				vector5 = ((!_movementExtension) ? (vector2.normalized * num8 * _deltaTime * 0.7f) : (vector2.normalized * num8 * _deltaTime * 1.5f));
				GamePlayScene gamePlayScene = Hub.s.pdata.main as GamePlayScene;
				if (gamePlayScene != null)
				{
					Tile tile = gamePlayScene.FindCurrentTile(_creature.PositionVector);
					if (tile != null && tile.gameObject.CompareTag("Area_TrapHole"))
					{
						vector5 = vector2.normalized * 3f;
					}
				}
			}
			Vector3 vector6 = vector5;
			float angle = _actionArray[0].Angle;
			float num9 = ((coordMode != ObservationStacker.CoordMode.World) ? Mathf.DeltaAngle(0f, angle) : Mathf.DeltaAngle(yaw, angle));
			if (Mathf.Abs(num9) > num)
			{
				num9 = Mathf.Sign(num9) * num;
			}
			_dLAgentMovementOutput.yawDiff = num9;
			if (_decisionAgent.DLAgentDecisionForMovement.Decision == DLDecisionType.LookTarget)
			{
				ExecuteLookAction(vector6);
			}
			else if (_decisionAgent.DLAgentDecisionForForcedRotation.Decision == DLDecisionType.ForceRotation)
			{
				ExecuteForcedRotation();
			}
			DLDecisionType decision = _decisionAgent.DLAgentDecisionForMovement.Decision;
			_dLAgentMovementOutput.worldPosDiff = vector6;
			bool active = decision == DLDecisionType.LookAtScrapScanner;
			bool active2 = decision == DLDecisionType.MoveToTarget;
			bool active3 = decision == DLDecisionType.RunawayFromMonster;
			bool active4 = decision == DLDecisionType.TooFarAwayFromTarget;
			bool active5 = decision == DLDecisionType.LookAtStashHanger;
			ExecuteForcedMoveWhenStationary(vector6, active2);
			ExecuteRunawayFromMonsters(vector6, active3);
			ExecuteTooFarAwayFromTarget(vector6, active4);
			ExecuteLookAtScrapScanner(active);
			ExecuteLookAtStashHanger(active5);
			UpdateJumpState(_deltaTime);
			_dLAgentMovementOutput.calcAngle = _alignRotationToMovement;
			if (ShowMovement)
			{
				float yaw2 = _creature.Position.yaw;
				float num10 = (_dLAgentMovementOutput.yawDiff.HasValue ? _dLAgentMovementOutput.yawDiff.Value : 0f);
				if (float.IsNaN(num10) || float.IsInfinity(num10))
				{
					Logger.RWarn($"Invalid yawDiff detected: {num10}, resetting to 0.", sendToLogServer: false, useConsoleOut: true, "dl");
					num10 = 0f;
				}
				float num11 = yaw2 + num10;
				if (float.IsNaN(num11) || float.IsInfinity(num11))
				{
					Logger.RWarn($"Invalid totalYaw detected: {num11}, resetting to 0.", sendToLogServer: false, useConsoleOut: true, "dl");
					num11 = 0f;
				}
				Vector3 vector7 = Quaternion.Euler(0f, num11, 0f) * Vector3.forward;
				Hub.DebugDraw_line_long(_me.transform.position, _me.transform.position + vector7, 10f, Color.magenta, 0.3f);
				if (_pathFinding)
				{
					Hub.DebugDraw_line_long(_me.transform.position, vector4, Vector3.Distance(_me.transform.position, vector4), Color.white, 0.3f);
				}
				Vector3 end = _me.transform.position + _dLAgentMovementOutput.worldPosDiff.Value;
				Hub.DebugDraw_Arrow(_me.transform.position, end, Color.yellow, 0.3f);
			}
			else if (ShowMovementFromModel)
			{
				float yaw3 = _creature.Position.yaw;
				float num12 = (_dLAgentMovementOutput.yawDiff.HasValue ? _dLAgentMovementOutput.yawDiff.Value : 0f);
				if (float.IsNaN(num12) || float.IsInfinity(num12))
				{
					Logger.RWarn($"Invalid yawDiff detected: {num12}, resetting to 0.", sendToLogServer: false, useConsoleOut: true, "dl");
					num12 = 0f;
				}
				float num13 = yaw3 + num12;
				if (float.IsNaN(num13) || float.IsInfinity(num13))
				{
					Logger.RWarn($"Invalid totalYaw detected: {num13}, resetting to 0.", sendToLogServer: false, useConsoleOut: true, "dl");
					num13 = 0f;
				}
				Vector3 vector8 = Quaternion.Euler(0f, num13, 0f) * Vector3.forward;
				Hub.DebugDraw_line_long(_me.transform.position, _me.transform.position + vector8, 10f, Color.magenta, 0.3f);
				if (_pathFinding)
				{
					Hub.DebugDraw_line_long(_me.transform.position, vector4, Vector3.Distance(_me.transform.position, vector4), Color.white, 0.3f);
				}
				Vector3 end2 = _me.transform.position + vector3.normalized * num8 * _deltaTime * 1.5f;
				Hub.DebugDraw_Arrow(_me.transform.position, end2, Color.green, 0.3f);
			}
			_ = _dLAgentMovementOutput.isRunning == true;
			if (_enableInferenceLog)
			{
				List<float> list = new List<float>();
				list.Add(actions.ContinuousActions[0]);
				list.Add(actions.ContinuousActions[1]);
				list.Add(actions.ContinuousActions[2]);
				_inferenceLogger.RecordAction(list);
			}
		}

		private void ExecuteForcedRotation()
		{
			_decisionAgent.DLAgentDecisionForForcedRotation.Reset();
			float num = _dLAgentMovementOutput.yawDiff.Value * _forceRotationAngleBoost;
			float num2 = Mathf.Max(1E-05f, _deltaTime * _turnSpeed * 0.7f);
			if (Mathf.Abs(num) > num2)
			{
				num = Mathf.Sign(num) * num2;
			}
			_dLAgentMovementOutput.yawDiff = num;
		}

		private void SetStayModel()
		{
			ApplyBehavior(_stay, 0);
		}

		private void SetExploreModel()
		{
			if (UnityEngine.Random.Range(0, 2) == 0)
			{
				ApplyBehavior(_explorerLeftHanded, 1);
			}
			else
			{
				ApplyBehavior(_explorerRightHanded, 2);
			}
		}

		private void SetFollowerModel()
		{
			ApplyBehavior(_follower, 3);
		}

		private void SetRunawayModel()
		{
			ApplyBehavior(_runaway, 4);
		}

		private void SetToTramModel()
		{
			_followerDistanceToTargetTooFar = 0.7f;
			_followerDistanceToTargetTooNear = 0f;
			int dungeonMasterID = Hub.s.pdata.dungeonMasterID;
			DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonMasterID);
			if (dungeonInfo != null)
			{
				_mapIDCached = dungeonInfo.MapID;
				switch (_mapIDCached)
				{
				case 15:
					ApplyBehavior(_outdoorAToTram, 5);
					break;
				case 17:
					ApplyBehavior(_outdoorBToTram, 5);
					break;
				case 18:
					ApplyBehavior(_outdoorCToTram, 5);
					break;
				default:
					ApplyBehavior(_outdoorAToTram, 5);
					break;
				}
			}
		}

		private void SetToEntranceModel()
		{
			_targetPos = Hub.s.dLAcademyManager.GetToEntranceHelperPosition();
			int dungeonMasterID = Hub.s.pdata.dungeonMasterID;
			DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonMasterID);
			if (dungeonInfo != null)
			{
				_mapIDCached = dungeonInfo.MapID;
				switch (_mapIDCached)
				{
				case 15:
					ApplyBehavior(_outdoorAToEntrance, 6);
					break;
				case 17:
					ApplyBehavior(_outdoorBToEntrance, 6);
					break;
				case 18:
					ApplyBehavior(_outdoorCToEntrance, 6);
					break;
				default:
					ApplyBehavior(_outdoorAToEntrance, 6);
					break;
				}
			}
		}

		private void SetInsideTramModel()
		{
			_targetPos = _creature.PositionVector + Hub.s.dLAcademyManager.GetInsideTramHelperDirection() * 10f;
			int dungeonMasterID = Hub.s.pdata.dungeonMasterID;
			if (Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonMasterID) != null)
			{
				ApplyBehavior(_stayInTram, 10);
			}
		}

		private void SetEnterTramModel()
		{
			_targetPos = Hub.s.dLAcademyManager.GetEnterTramHelperPosition();
			int dungeonMasterID = Hub.s.pdata.dungeonMasterID;
			if (Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonMasterID) != null)
			{
				ApplyBehavior(_enterTram, 7);
			}
		}

		private void SetExitTramModel()
		{
			int dungeonMasterID = Hub.s.pdata.dungeonMasterID;
			if (Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonMasterID) != null)
			{
				_targetPos = Hub.s.dLAcademyManager.GetExitTramHelperPosition();
				ApplyBehavior(_exitTram, 8);
			}
		}

		private void SetOutsideOtherModel()
		{
			int dungeonMasterID = Hub.s.pdata.dungeonMasterID;
			DungeonMasterInfo dungeonInfo = Hub.s.dataman.ExcelDataManager.GetDungeonInfo(dungeonMasterID);
			if (dungeonInfo != null)
			{
				_mapIDCached = dungeonInfo.MapID;
				switch (_mapIDCached)
				{
				case 15:
					ApplyBehavior(_outdoorAOther, 9);
					break;
				case 17:
					ApplyBehavior(_outdoorBOther, 9);
					break;
				case 18:
					ApplyBehavior(_outdoorCOther, 9);
					break;
				default:
					ApplyBehavior(_outdoorAOther, 9);
					break;
				}
			}
		}

		private void SetToTramFromBackdoorModel()
		{
			_targetPos = Hub.s.dLAcademyManager.GetEnterTramHelperPosition();
			ApplyBehavior(_outdoorAToTramFromBackdoor, 11);
		}

		private void SetToTramViaHelperModel()
		{
			_targetPos = Hub.s.dLAcademyManager.GetToTramHelperPosition();
			ApplyBehavior(_outdoorAToTramViaHelper, 12);
		}

		private void SetToLookAtStashHangerModel()
		{
			_targetPos = _decisionAgent.DLAgentDecisionForMovement.TargetPos;
			ApplyBehavior(_lookAtStashHanger, 13);
		}

		private void SetToLookAtScrapScannerModel()
		{
			Vector3 targetPos = _decisionAgent.DLAgentDecisionForMovement.TargetPos;
			Vector3 positionVector = _creature.PositionVector;
			Vector3 vector = new Vector3(targetPos.x - positionVector.x, 0f, targetPos.z - positionVector.z);
			if (vector.sqrMagnitude > 0.0001f)
			{
				vector.Normalize();
			}
			_targetPos = positionVector + vector * 10f;
			ApplyBehavior(_lookAtScrapScanner, 14);
		}

		public void ExecuteForcedMoveWhenStationary(Vector3 originaDiff, bool active)
		{
			_dLAgentMovementOutput.worldPosDiff = originaDiff;
			if (active)
			{
				_followerDistanceToTargetTooFar = 0.7f;
				_followerDistanceToTargetTooNear = 0f;
				SetModelIfChanged(ModelKey.Follower);
				if (!_inStationaryMove)
				{
					_inStationaryMove = true;
				}
			}
			else if (_inStationaryMove)
			{
				_inStationaryMove = false;
				RevertToBaseModel();
			}
		}

		public void ExecuteRunawayFromMonsters(Vector3 original_diff, bool active)
		{
			_dLAgentMovementOutput.worldPosDiff = original_diff;
			if (active)
			{
				SetModelIfChanged(ModelKey.Runaway);
				if (!_inRunawayFromMonsters)
				{
					_inRunawayFromMonsters = true;
				}
			}
			else if (_inRunawayFromMonsters)
			{
				_inRunawayFromMonsters = false;
				RevertToBaseModel();
			}
		}

		public void SetFrequency(float newFrequency)
		{
			_agentStepFrequency = newFrequency;
		}

		public void SetCreature(VCreature creature)
		{
			_creature = creature;
		}

		public void RegisterTargetActor(VCreature target)
		{
			if (TargetCreature != target)
			{
				_observationStack.Reset();
			}
			TargetCreature = target;
		}

		public void RegisterTargetActorFromClient(ProtoActor target)
		{
			if (TargetActor != target)
			{
				_observationStack.Reset();
			}
			TargetActor = target;
		}

		public void Update()
		{
			UpdatePitchDiff();
		}

		private float GetClosestAngle(float diff, float[] snapAngles)
		{
			float result = 0f;
			float num = float.MaxValue;
			foreach (float num2 in snapAngles)
			{
				float num3 = Mathf.Abs(Mathf.DeltaAngle(diff, num2));
				if (num3 < num)
				{
					num = num3;
					result = num2;
				}
			}
			return result;
		}

		public void OnDrawGizmos()
		{
			if (_drawPathGizmos)
			{
				DrawPathGizmos();
			}
			if (!_showRayGizmo)
			{
				return;
			}
			RaySensorHitResult[] array = ((_creature == null) ? _me.raySensor.Shoot(base.transform.position, base.transform.rotation.eulerAngles.y) : _me.raySensor.Shoot(_creature.PositionVector, _creature.Position.yaw));
			Vector3 vector = base.transform.position + Vector3.up;
			if (_creature != null)
			{
				vector = _creature.PositionVector + Vector3.up;
			}
			for (int i = 0; i < array.Length; i++)
			{
				for (int j = 0; j < array[i].hitPoints.Count; j++)
				{
					Gizmos.color = debugColors[array[i].hitTypes[j]];
					Gizmos.DrawWireSphere(array[i].hitPoints[j], _me.raySensor.forwardHeadRadius);
					if (j + 1 == array[i].hitPoints.Count)
					{
						Vector3 direction = array[i].hitPoints[j] - vector;
						Gizmos.DrawRay(vector, direction);
					}
				}
			}
		}

		private void DrawPathGizmos()
		{
			if (_pathList != null && _pathList.Count != 0)
			{
				Vector3 obj = ((_creature != null) ? _creature.PositionVector : base.transform.position);
				Gizmos.color = _startToFirstColor;
				Gizmos.DrawSphere(_targetPos, 0.3f);
				Gizmos.color = _startToFirstColor;
				Gizmos.DrawLine(obj, _pathList[0]);
				Gizmos.color = _pathColor;
				Gizmos.DrawSphere(_pathList[0], _pathNodeRadius);
				for (int i = 1; i < _pathList.Count; i++)
				{
					Vector3 vector = _pathList[i - 1];
					Vector3 vector2 = _pathList[i];
					Gizmos.DrawLine(vector, vector2);
					Gizmos.DrawSphere(vector2, _pathNodeRadius);
				}
			}
		}

		private void UpdatePitchDiff()
		{
			if (_creature == null)
			{
				return;
			}
			float pitch = _creature.Position.pitch;
			float? overridePitchForDecision = GetOverridePitchForDecision();
			if (overridePitchForDecision.HasValue)
			{
				float value = overridePitchForDecision.Value;
				_pitchStep = 200f;
				_storedTargetPitch = value;
				_pitchTargetInitialized = true;
				_dLAgentMovementOutput.pitchDiff = _storedTargetPitch - pitch;
				return;
			}
			if (!_pitchTargetInitialized)
			{
				_storedTargetPitch = GenerateRandomTargetPitch();
				_pitchTargetInitialized = true;
				_dLAgentMovementOutput.pitchDiff = _storedTargetPitch - pitch;
			}
			if (_isPitchPaused)
			{
				_pitchPauseTimer -= Time.deltaTime;
				if (!(_pitchPauseTimer <= 0f))
				{
					_dLAgentMovementOutput.pitchDiff = 0f;
					return;
				}
				_isPitchPaused = false;
				_storedTargetPitch = GenerateRandomTargetPitch();
			}
			float num = Mathf.DeltaAngle(pitch, _storedTargetPitch);
			float num2 = Mathf.Clamp01(Mathf.Abs(num) / 45f);
			float num3 = _pitchStep * num2 * Time.deltaTime;
			float value2 = Mathf.Clamp(num, 0f - num3, num3);
			_dLAgentMovementOutput.pitchDiff = value2;
			if (!InAllowedPitchRange(pitch) || !InAllowedPitchRange(_storedTargetPitch))
			{
				if (!InAllowedPitchRange(pitch))
				{
					_isPitchPaused = false;
					_storedTargetPitch = GenerateRandomTargetPitch();
				}
				else
				{
					_dLAgentMovementOutput.pitchDiff = 0f;
					_isPitchPaused = true;
					_pitchPauseTimer = UnityEngine.Random.Range(0f, 3f);
				}
			}
			if (Mathf.Abs(Mathf.DeltaAngle(pitch, _storedTargetPitch)) < 0.1f)
			{
				_isPitchPaused = true;
				_pitchPauseTimer = GetPauseTimeForPitchTarget(_storedTargetPitch);
			}
		}

		private float GetPauseTimeForPitchTarget(float target)
		{
			if (IsTargetInInnerMargin(target))
			{
				return UnityEngine.Random.Range(0f, 1f);
			}
			return UnityEngine.Random.Range(0f, 3f);
		}

		private bool IsTargetInInnerMargin(float target)
		{
			bool num = target >= 330f && target <= 350f;
			bool flag = target >= 60f && target <= 70f;
			return num || flag;
		}

		private float GenerateRandomTargetPitch()
		{
			float num = UnityEngine.Random.Range(0f, PitchSpan);
			float result = (MinAngle + num) % 360f;
			_pitchStep = UnityEngine.Random.Range(100f, 150f);
			return result;
		}

		private bool InAllowedPitchRange(float angle)
		{
			if (!(angle >= MinAngle))
			{
				return angle <= MaxAngle;
			}
			return true;
		}

		private void UpdateRunState()
		{
			if (_decisionAgent.DLAgentDecisionForRunning.Decision == DLDecisionType.Run)
			{
				_dLAgentMovementOutput.isRunning = true;
			}
			else if (_inRunawayFromMonsters)
			{
				_dLAgentMovementOutput.isRunning = true;
			}
			else
			{
				_dLAgentMovementOutput.isRunning = false;
			}
		}

		private void UpdateJumpState(float deltaTime)
		{
			if (_decisionAgent.DLAgentDecisionForJumping.Decision == DLDecisionType.Jump)
			{
				_jumpPacketSendTimer -= deltaTime;
				if (_jumpPacketSendTimer <= 0f)
				{
					_creature.SendInSight(new JumpSig
					{
						actorID = _creature.ObjectID
					});
					_jumpPacketSendTimer = _jumpPacketSendCooltime;
				}
			}
		}

		private void ExecuteLookAction(Vector3 worldPosDiff_temp)
		{
			Vector3 vector = TargetCreature.PositionVector - _creature.PositionVector;
			float target = Mathf.Atan2(vector.x, vector.z) * 57.29578f;
			float value = Mathf.DeltaAngle(_creature.Position.yaw, target);
			_dLAgentMovementOutput.yawDiff = value;
		}

		private void ExecuteTooFarAwayFromTarget(Vector3 originalDiff, bool active)
		{
			_dLAgentMovementOutput.worldPosDiff = originalDiff;
			if (active)
			{
				_followerDistanceToTargetTooFar = 0.7f;
				_followerDistanceToTargetTooNear = 0f;
				SetModelIfChanged(ModelKey.Follower);
				if (!_inTooFarAwayFromTarget)
				{
					_inTooFarAwayFromTarget = true;
				}
			}
			else if (_inTooFarAwayFromTarget)
			{
				_followerDistanceToTargetTooFar = 0.7f;
				_followerDistanceToTargetTooNear = 0f;
				_inTooFarAwayFromTarget = false;
				RevertToBaseModel();
			}
		}

		public DLAgentMovementOutput GetDLAgentMovementOutputRaw()
		{
			if (_actionArray.Count == 0)
			{
				return null;
			}
			_dLAgentMovementOutputRaw.worldPosDiff = _actionArray[0].Movement;
			_dLAgentMovementOutputRaw.yawDiff = _actionArray[0].Angle;
			_dLAgentMovementOutputRaw.isRunning = _dLAgentMovementOutput.isRunning;
			return _dLAgentMovementOutputRaw;
		}

		private ModelKey MapBaseModel(DLDecisionType d)
		{
			if (!s_BaseDecisionToModel.TryGetValue(d, out var value))
			{
				return ModelKey.Stay;
			}
			return value;
		}

		private void SetModelIfChanged(ModelKey wanted)
		{
			if (_currentModelKey != wanted)
			{
				_currentModelKey = wanted;
				switch (wanted)
				{
				case ModelKey.Explore:
					SetExploreModel();
					break;
				case ModelKey.Follower:
					SetFollowerModel();
					break;
				case ModelKey.Runaway:
					SetRunawayModel();
					break;
				case ModelKey.Stay:
					SetStayModel();
					break;
				case ModelKey.ToTram:
					SetToTramModel();
					break;
				case ModelKey.ToEntrance:
					SetToEntranceModel();
					break;
				case ModelKey.InsideTram:
					SetInsideTramModel();
					break;
				case ModelKey.OutsideOther:
					SetOutsideOtherModel();
					break;
				case ModelKey.EnterTram:
					SetEnterTramModel();
					break;
				case ModelKey.ExitTram:
					SetExitTramModel();
					break;
				case ModelKey.ToTramFromBackdoor:
					SetToTramFromBackdoorModel();
					break;
				case ModelKey.ToTramViaHelper:
					SetToTramViaHelperModel();
					break;
				case ModelKey.LookAtStashHanger:
					SetToLookAtStashHangerModel();
					break;
				case ModelKey.LookAtScrapScanner:
					SetToLookAtScrapScannerModel();
					break;
				default:
					SetStayModel();
					break;
				}
			}
		}

		private void ApplyBehavior(BehaviorContext context, int debugChangedInfoCode)
		{
			if (context == null || context.ProfileCount == 0)
			{
				Logger.RError("ApplyProfile: BehaviorContext is null or empty");
				return;
			}
			ModelProfile randomProfile = context.GetRandomProfile();
			if (randomProfile == null)
			{
				Logger.RError("ApplyProfile: ModelProfileSO is null");
				return;
			}
			_lambdaForModel = randomProfile.lambda;
			_pathFinding = randomProfile.pathFinding;
			_useAngleBoost = randomProfile.useAngleBoost;
			_alignRotationToMovement = randomProfile.alignRotationToMovement;
			_movementExtension = randomProfile.movementExtension;
			SetModel(_behaviorName, randomProfile.asset);
			ResetDeadzoneStop();
			_creature.SendInSight(new DebugDLAgentInfoSig
			{
				actorID = _creature.ObjectID,
				changedInfoType = 1,
				changedInfo = debugChangedInfoCode
			});
		}

		private void ApplyBaseModel(DLDecisionType baseDecision)
		{
			ModelKey modelIfChanged = MapBaseModel(baseDecision);
			SetModelIfChanged(modelIfChanged);
		}

		private void RevertToBaseModel()
		{
			ApplyBaseModel(_lastNonOverlayMoveDecision);
		}

		private void ActivateDeadzoneStop()
		{
			float minInclusive = Mathf.Min(_deadzoneStopDurationRange.x, _deadzoneStopDurationRange.y);
			float maxInclusive = Mathf.Max(_deadzoneStopDurationRange.x, _deadzoneStopDurationRange.y);
			_deadzoneStopTimer = UnityEngine.Random.Range(minInclusive, maxInclusive);
			_deadzoneStopActive = true;
		}

		private void ResetDeadzoneStop()
		{
			_deadzoneStopActive = false;
			_deadzoneStopTimer = 0f;
			_deadzoneActivateCooltimeTimer = _deadzoneActivateCooltime;
		}

		private void ExecuteLookAtScrapScanner(bool active)
		{
			if (active)
			{
				SetModelIfChanged(ModelKey.LookAtScrapScanner);
				if (!_inLookAtScrapScanner)
				{
					_inLookAtScrapScanner = true;
				}
			}
			else if (_inLookAtScrapScanner)
			{
				_inLookAtScrapScanner = false;
				RevertToBaseModel();
			}
		}

		private void ExecuteLookAtStashHanger(bool active)
		{
			if (active)
			{
				_followerDistanceToTargetTooFar = 3f;
				_followerDistanceToTargetTooNear = 1.5f;
				SetModelIfChanged(ModelKey.LookAtStashHanger);
				if (!_inLookAtStashHanger)
				{
					_inLookAtStashHanger = true;
				}
			}
			else if (_inLookAtStashHanger)
			{
				_followerDistanceToTargetTooFar = 0.7f;
				_followerDistanceToTargetTooNear = 0f;
				_inLookAtStashHanger = false;
				RevertToBaseModel();
			}
		}

		private float? GetOverridePitchForDecision()
		{
			if (_decisionAgent == null)
			{
				return null;
			}
			if (_decisionAgent.DLAgentDecisionForMovement.Decision == DLDecisionType.LookAtStashHanger)
			{
				return -10f;
			}
			return null;
		}
	}
}
