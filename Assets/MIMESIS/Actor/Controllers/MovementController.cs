using System;
using System.Collections.Generic;
using ReluProtocol;
using ReluProtocol.Enum;
using UnityEngine;

public class MovementController : IVActorController, IDisposable
{
	private class DirectMoveContext : IMoveContext
	{
		private float m_MoveCredit;

		private long m_MoveCreditRegenTime;

		private PosWithRot m_ReceivedFuturePosition = new PosWithRot();

		private long m_ReceivedTime;

		private long m_ReceivedFutureTime;

		private float m_PeakMoveSpeed;

		public DirectMoveContext(MovementController parent, VCreature owner)
			: base(parent, owner, MoveContextType.Direct)
		{
			m_MoveCredit = 0f;
			m_MoveCreditRegenTime = Hub.s.timeutil.GetCurrentTickMilliSec();
			Reset();
		}

		public override void Reset()
		{
			base.IsMoving = false;
			m_ReceivedTime = 0L;
			m_ReceivedFutureTime = 0L;
			m_ReceivedFuturePosition.Clean();
		}

		private void RegenDirectMoveCredit()
		{
			float num = (base.IsMoving ? m_PeakMoveSpeed : (m_Owner.StatControlUnit?.GetMoveSpeed() ?? 0f));
			long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
			long timeMilliSec = currentTickMilliSec - m_MoveCreditRegenTime;
			float val = num * 5f;
			float num2 = Hub.s.timeutil.ChangeTimeMilli2Sec(timeMilliSec) * num * 1.1f;
			if (!(num2 < 0f))
			{
				m_MoveCreditRegenTime = currentTickMilliSec;
				m_MoveCredit = Math.Min(val, m_MoveCredit + num2);
			}
		}

		private bool ValidateMoveSpped(PosWithRot endPos)
		{
			RegenDirectMoveCredit();
			(bool, float) tuple = Misc.ValidateMoveSpped(base.IsMoving ? m_Owner.SavedPosition : m_Owner.Position, endPos, m_MoveCredit);
			if (tuple.Item1)
			{
				m_MoveCredit = tuple.Item2;
				return true;
			}
			return false;
		}

		public override void SyncMoveStopSig()
		{
			MoveStopSig msg = new MoveStopSig
			{
				actorMoveType = m_Parent.GetMoveType(),
				actorID = m_Owner.ObjectID,
				currentPos = m_Owner.Position
			};
			m_Owner.SendInSight(msg, includeSelf: true);
		}

		public MsgErrorCode OnMoveStopReq(PosWithRot prevPos, PosWithRot currPos)
		{
			float val = Misc.Distance(currPos.toVector3(), prevPos.toVector3(), ignoreHeight: true);
			float val2 = Misc.Distance(m_Owner.SavedPosition.toVector3(), currPos.toVector3(), ignoreHeight: true);
			if (Math.Min(val, val2) > 10f || !ValidateMoveSpped(currPos))
			{
				return MsgErrorCode.WillBeTeleported;
			}
			float num = m_Parent.CheckFallDamage(prevPos.toVector3(), currPos.toVector3(), stopped: true);
			if (num > 0f)
			{
				long num2 = (long)((double?)((float?)m_Owner.StatControlUnit?.GetSpecificStatValue(StatType.HP) * num) * 0.01).GetValueOrDefault();
				if (num2 > 0)
				{
					m_Owner.StatControlUnit?.ApplyDamage(new ApplyDamageArgs(null, m_Owner, MutableStatChangeCause.Fall, num2, 0L));
				}
			}
			m_Owner.SetPosition(currPos, ActorMoveCause.Move, toSave: true);
			UpdateLastPosOnNav();
			if (base.IsMoving)
			{
				StopMove(sync: true);
			}
			else
			{
				SyncMoveStopSig();
			}
			return MsgErrorCode.Success;
		}

		public MsgErrorCode StartMove(PosWithRot prevPos, PosWithRot currPos, PosWithRot futurePos, long futureTime, float pitch, long receivedTick, int targetActorID, int transID, ActorMoveType prevMoveType, ActorMoveType currentMoveType)
		{
			if (!base.IsMoving)
			{
				m_PeakMoveSpeed = (m_Owner.StatControlUnit ?? throw new Exception("StatControlUnit is null in DirectMoveContext")).GetMoveSpeed();
			}
			float num = Misc.Distance(currPos.toVector3(), prevPos.toVector3(), ignoreHeight: true);
			if (10f < num || !ValidateMoveSpped(currPos))
			{
				Logger.RError($"[DirectMoveContext] StartMove failed. distance:{num} moveCredit:{m_MoveCredit}");
				return MsgErrorCode.WillBeTeleported;
			}
			if (prevMoveType == ActorMoveType.FallDown || currentMoveType == ActorMoveType.FallDown)
			{
				float num2 = m_Parent.CheckFallDamage(prevPos.toVector3(), currPos.toVector3(), prevMoveType == ActorMoveType.FallDown && currentMoveType != ActorMoveType.FallDown);
				if (num2 > 0f)
				{
					long num3 = (long)((double?)((float?)m_Owner.StatControlUnit?.GetSpecificStatValue(StatType.HP) * num2) * 0.01).GetValueOrDefault();
					if (num3 > 0)
					{
						m_Owner.StatControlUnit?.ApplyDamage(new ApplyDamageArgs(null, m_Owner, MutableStatChangeCause.Fall, num3, 0L));
					}
				}
			}
			if (futureTime < 0 || (float)futureTime > 1000f)
			{
				futureTime = 0L;
			}
			if (futureTime > 0)
			{
				float num4 = m_PeakMoveSpeed * (float)Hub.s.timeutil.ChangeTimeSec2Milli(futureTime) * 1.5f;
				float num5 = Misc.Distance(currPos.toVector3(), futurePos.toVector3(), ignoreHeight: true);
				if (num4 < num5)
				{
					Logger.RWarn($"[DirectMoveContext] StartMove failed. expectedDistance:{num4} actualDistance:{num5}, m_peakMoveSpeed:{m_PeakMoveSpeed}");
					futureTime = 0L;
					return MsgErrorCode.WillBeTeleported;
				}
			}
			base.IsMoving = true;
			m_Owner.SetPosition(currPos, ActorMoveCause.Move, toSave: true);
			UpdateLastPosOnNav();
			if (futureTime > 0)
			{
				m_ReceivedFuturePosition = futurePos.Clone();
				m_Parent.DebugDestination = m_ReceivedFuturePosition.Clone();
			}
			else
			{
				m_ReceivedFuturePosition.Clean();
				m_Parent.SyncDebugPosition();
			}
			m_Parent.AccumulateMoveDistance(num);
			m_ReceivedTime = ((receivedTick == 0L) ? Hub.s.timeutil.GetCurrentTickMilliSec() : receivedTick);
			m_ReceivedFutureTime = futureTime;
			MoveStartSig msg = new MoveStartSig
			{
				actorID = m_Owner.ObjectID,
				basePositionCurr = currPos,
				basePositionFuture = futurePos,
				futureTime = futureTime,
				pitch = pitch
			};
			m_Owner.SendInSight(msg, includeSelf: true);
			OnUpdate(0L);
			m_Owner.AttachControlUnit.OnMove();
			return MsgErrorCode.Success;
		}

		public void UpdateLastPosOnNav()
		{
			if (m_Owner.VRoom.FindNearestPoly(m_Owner.PositionVector, out var nearestPos))
			{
				m_Parent.LastPosOnNavi = nearestPos;
			}
		}

		public override void OnUpdate(long delta)
		{
			if (base.IsMoving && m_ReceivedTime != 0L && m_ReceivedFutureTime != 0L)
			{
				float num = Hub.s.timeutil.ChangeTimeMilli2Sec(m_ReceivedFutureTime);
				float num2 = Hub.s.timeutil.ChangeTimeMilli2Sec(Math.Min(Hub.s.timeutil.GetCurrentTickMilliSec() - m_ReceivedTime, m_ReceivedFutureTime));
				Vector3 a = m_Owner.SavedPosition.toVector3();
				Vector3 b = m_ReceivedFuturePosition.toVector3();
				Vector3 vector = Vector3.Lerp(a, b, num2 / num);
				if (Vector3.Distance(a, vector) > float.Epsilon)
				{
					PosWithRot pos = vector.toPosWithRot(m_Owner.SavedPosition.yaw, m_Owner.SavedPosition.pitch);
					m_Owner.SetPosition(pos, ActorMoveCause.Move);
				}
			}
		}

		public override void OnChangedPosition()
		{
			if (base.IsMoving)
			{
				m_ReceivedFutureTime = 0L;
			}
		}

		public override void OnForceSyncPosition(TeleportReason teleportReason)
		{
			if (base.IsMoving)
			{
				m_Owner.OverwritePosition();
			}
			if (teleportReason != TeleportReason.ForceMoveSync)
			{
				m_MoveCredit = 0f;
				m_MoveCreditRegenTime = Hub.s.timeutil.GetCurrentTickMilliSec();
			}
		}

		public override void OnSetMoveType(ActorMoveType moveType)
		{
			UpdatePeakMoveSpeed();
		}

		private void UpdatePeakMoveSpeed()
		{
			if (base.IsMoving)
			{
				float moveSpeed = (m_Owner.StatControlUnit ?? throw new Exception("StatControlUnit is null in DirectMoveContext")).GetMoveSpeed();
				m_PeakMoveSpeed = Math.Max(m_PeakMoveSpeed, moveSpeed);
			}
		}

		public override void OnChangeImmutableStats()
		{
			UpdatePeakMoveSpeed();
		}

		public override void StopMove(bool sync)
		{
			UpdateLastPosOnNav();
			if (base.IsMoving)
			{
				Reset();
				if (sync)
				{
					SyncMoveStopSig();
				}
				m_Parent.OnStopMove();
			}
		}

		public override PosWithRot GetPrevPos()
		{
			return m_Owner.SavedPosition;
		}

		public override void SyncMoveForce(PosWithRot pos, ActorMoveType moveType)
		{
			m_ReceivedFuturePosition.Clean();
			m_ReceivedTime = 0L;
			m_ReceivedFutureTime = 0L;
			m_MoveCredit = 0f;
			m_MoveCreditRegenTime = Hub.s.timeutil.GetCurrentTickMilliSec();
			m_Owner.SetPosition(pos, ActorMoveCause.Attach, toSave: true);
			UpdateLastPosOnNav();
			m_Parent.SyncDebugPosition();
			m_Owner.SendInSight(new MoveStartSig
			{
				actorID = m_Owner.ObjectID,
				basePositionPrev = m_Owner.SavedPosition,
				basePositionCurr = pos,
				basePositionFuture = pos,
				futureTime = 0L,
				pitch = m_Owner.SavedPosition.pitch,
				actorMoveType = moveType
			}, includeSelf: true);
			OnUpdate(0L);
		}
	}

	private abstract class IMoveContext
	{
		protected readonly MovementController m_Parent;

		protected readonly VCreature m_Owner;

		public readonly MoveContextType MoveContextType;

		public bool IsMoving { get; protected set; }

		public IMoveContext(MovementController parent, VCreature owner, MoveContextType moveContextType)
		{
			m_Parent = parent;
			m_Owner = owner;
			MoveContextType = moveContextType;
		}

		public abstract void Reset();

		public abstract void OnUpdate(long delta);

		public abstract void OnChangedPosition();

		public abstract void OnForceSyncPosition(TeleportReason teleportReason);

		public abstract void OnSetMoveType(ActorMoveType moveType);

		public abstract void OnChangeImmutableStats();

		public abstract void StopMove(bool sync);

		public abstract PosWithRot GetPrevPos();

		public abstract void SyncMoveStopSig();

		public abstract void SyncMoveForce(PosWithRot pos, ActorMoveType moveType);
	}

	private class PathMoveContext : IMoveContext
	{
		private Vector3 m_TargetPos;

		private bool m_FixLookAtTarget;

		private bool m_CalcAngle = true;

		private PosWithRot m_DestPos = new PosWithRot();

		private List<Vector3> m_PathList = new List<Vector3>();

		private int m_CurrentPathIndex;

		private int m_TraceActorID;

		private PosWithRot m_LastSyncPosition = new PosWithRot();

		private Vector3 _fallBottomPos = Vector3.zero;

		public PathMoveContext(MovementController parent, VCreature owner)
			: base(parent, owner, MoveContextType.Path)
		{
			Reset();
		}

		public override void Reset()
		{
			base.IsMoving = false;
			m_TargetPos = Vector3.zero;
			m_DestPos.Clean();
			m_LastSyncPosition.Clean();
			m_PathList.Clear();
			m_CurrentPathIndex = 0;
		}

		private void TurnToTarget()
		{
			if (m_TraceActorID != 0)
			{
				VActor vActor = m_Owner.VRoom.FindActorByObjectID(m_TraceActorID);
				if (vActor != null)
				{
					float directionAngle = Misc.GetDirectionAngle(m_Owner.PositionVector, vActor.PositionVector);
					m_Owner.SetAngle(directionAngle);
				}
			}
		}

		private void SendMoveStartPacket(PosWithRot futurePos)
		{
			MoveStartSig moveStartSig = new MoveStartSig
			{
				actorID = m_Owner.ObjectID,
				targetID = m_TraceActorID,
				actorMoveType = m_Parent.GetMoveType(),
				basePositionPrev = m_LastSyncPosition.Clone(),
				basePositionCurr = m_Owner.Position.Clone(),
				basePositionFuture = futurePos.Clone(),
				pitch = futurePos.pitch
			};
			float num = Misc.Distance(m_Owner.PositionVector, futurePos.toVector3());
			float moveSpeed = (m_Owner.StatControlUnit ?? throw new Exception("StatControlUnit is null in PathMoveContext")).GetMoveSpeed();
			if (moveSpeed == 0f)
			{
				moveStartSig.futureTime = 0L;
				moveStartSig.basePositionFuture.Clean();
			}
			else
			{
				moveStartSig.futureTime = (int)Hub.s.timeutil.ChangeTimeSec2Milli(num / moveSpeed);
			}
			m_Owner.SendInSight(moveStartSig, includeSelf: true);
		}

		public bool FindPath(Vector3 dest, ref List<Vector3> outPath)
		{
			outPath.Clear();
			if (Vector3.Distance(m_Owner.PositionVector, dest) < 0.1f)
			{
				return true;
			}
			if (!m_Owner.VRoom.FindPath(m_Owner.PositionVector, dest, out List<Vector3> path))
			{
				if (!m_Owner.VRoom.HasNearestPoly(dest))
				{
					Logger.RError($"[PathMoveContext] FindPath failed. Invalid destination. ActorID: {m_Owner.ObjectID}, Dest: {dest}");
				}
				return false;
			}
			if (path.Count > 100)
			{
				int num = path.Count - 1;
				int num2 = 0;
				int num3 = 1;
				int num4 = 2;
				for (int i = 3; i < path.Count; i++)
				{
					if (path[num2] == path[num4] && path[num3] == path[i])
					{
						num = num2;
						break;
					}
					num2++;
					num3++;
					num4++;
				}
				path.RemoveRange(num, path.Count - num);
			}
			if (path.Count == 0)
			{
				return false;
			}
			outPath.AddRange(path);
			return true;
		}

		private PathMoveResult StartMoveInternal(Vector3 targetPos, bool calcAngle = true, bool resetPath = false)
		{
			if ((!resetPath && m_TargetPos.Equals(targetPos) && m_PathList.Count != 0) || !_fallBottomPos.Equals(Vector3.zero))
			{
				return PathMoveResult.Duplicate;
			}
			m_TargetPos = targetPos;
			if (m_TargetPos.Equals(m_Owner.PositionVector))
			{
				StopMove(sync: true);
				return PathMoveResult.AlreadyArrived;
			}
			m_PathList.Clear();
			if (m_Owner.IsEnableFallPath && Misc.IsFallingDistance(m_Owner.PositionVector, targetPos, 1f))
			{
				Vector3 vector = targetPos;
				Vector3 nearestPoint = default(Vector3);
				List<Vector3> holePointsOnTrackVolume = m_Owner.VRoom.GetHolePointsOnTrackVolume(m_Owner.PositionVector);
				if (holePointsOnTrackVolume == null || holePointsOnTrackVolume.Count == 0)
				{
					m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
					StopMove(sync: true);
					return PathMoveResult.Fail;
				}
				float num = Misc.Distance(m_Owner.PositionVector, targetPos);
				Vector3 vector2 = default(Vector3);
				foreach (Vector3 item in holePointsOnTrackVolume)
				{
					if (m_Owner.VRoom.FindNearestPoly(item, out var nearestPos, 2f))
					{
						float num2 = Misc.Distance(m_Owner.PositionVector, nearestPos);
						if (num2 < num)
						{
							num = num2;
							vector = nearestPos;
							vector2 = item;
						}
					}
				}
				if (!FindPath(vector, ref m_PathList))
				{
					m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
					StopMove(sync: true);
					return PathMoveResult.Fail;
				}
				if (m_PathList.Count == 0)
				{
					m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
					StopMove(sync: true);
					return PathMoveResult.Success;
				}
				List<Vector3> linearMovementPath = Misc.GetLinearMovementPath(vector, vector2, m_Owner.StatControlUnit.GetMoveSpeed());
				m_PathList.AddRange(linearMovementPath);
				if (!m_Owner.VRoom.VerticalRaycast(vector2, (vector2.y - targetPos.y) * 1.2f, upDir: false, out nearestPoint))
				{
					m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
					StopMove(sync: true);
					return PathMoveResult.Fail;
				}
				List<Vector3> gravityFallPath = Misc.GetGravityFallPath(vector2, nearestPoint);
				if (gravityFallPath.Count > 0)
				{
					m_PathList.AddRange(gravityFallPath);
				}
				if (!m_Owner.VRoom.FindPath(nearestPoint, targetPos, out List<Vector3> path) || path.Count == 0)
				{
					m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
					StopMove(sync: true);
					return PathMoveResult.Fail;
				}
				_fallBottomPos = nearestPoint;
				foreach (Vector3 item2 in path)
				{
					m_PathList.Add(item2);
				}
			}
			else
			{
				if (!FindPath(m_TargetPos, ref m_PathList))
				{
					m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
					StopMove(sync: true);
					return PathMoveResult.Fail;
				}
				if (m_PathList.Count == 0)
				{
					m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
					StopMove(sync: true);
					return PathMoveResult.Success;
				}
			}
			m_CurrentPathIndex = 0;
			while (m_CurrentPathIndex < m_PathList.Count && !(m_PathList[m_CurrentPathIndex] != m_Owner.PositionVector))
			{
				m_CurrentPathIndex++;
			}
			if (m_CurrentPathIndex >= m_PathList.Count)
			{
				return PathMoveResult.Fail;
			}
			m_Parent.DebugDestination = m_TargetPos.toPosWithRot(m_Owner.Position.yaw, m_Owner.Position.pitch);
			m_CalcAngle = calcAngle;
			bool flag = Misc.IsInFall(m_Owner.PositionVector, m_PathList[m_CurrentPathIndex]);
			if (m_CalcAngle && !flag)
			{
				(m_Owner.StatControlUnit ?? throw new Exception("StatControlUnit is null in PathMoveContext")).GetMoveSpeed();
				m_Owner.SetAngle(Misc.GetDirectionAngle(m_Owner.PositionVector, m_PathList[m_CurrentPathIndex]));
			}
			if (m_FixLookAtTarget && !flag)
			{
				TurnToTarget();
			}
			if (!base.IsMoving)
			{
				m_LastSyncPosition = m_Owner.Position.Clone();
			}
			base.IsMoving = true;
			PosWithRot posWithRot = new PosWithRot();
			posWithRot.x = m_PathList[m_CurrentPathIndex].x;
			posWithRot.y = m_PathList[m_CurrentPathIndex].y;
			posWithRot.z = m_PathList[m_CurrentPathIndex].z;
			posWithRot.yaw = m_Owner.Position.yaw;
			posWithRot.pitch = m_Owner.Position.pitch;
			PosWithRot futurePos = posWithRot;
			SendMoveStartPacket(futurePos);
			return PathMoveResult.Success;
		}

		public void SetFixLookAtTarget(bool fixLookAtTarget)
		{
			m_FixLookAtTarget = fixLookAtTarget;
		}

		public PathMoveResult StartMove(Vector3 targetPos, bool calcAngle = true, bool resetPath = false)
		{
			return StartMoveInternal(targetPos, calcAngle, resetPath);
		}

		public override void OnUpdate(long deltaTime)
		{
			OnSimulate(deltaTime);
			if (m_TraceActorID != 0 && m_Owner.VRoom.FindActorByObjectID(m_TraceActorID) == null)
			{
				m_TraceActorID = 0;
			}
		}

		public void SetTraceActorID(int actorID)
		{
			m_TraceActorID = actorID;
		}

		private void OnSimulate(long delta)
		{
			float num = Hub.s.timeutil.ChangeTimeMilli2Sec(delta);
			if (m_PathList.Count == 0 || m_CurrentPathIndex >= m_PathList.Count)
			{
				return;
			}
			float moveSpeed = (m_Owner.StatControlUnit ?? throw new Exception("StatControlUnit is null in PathMoveContext")).GetMoveSpeed();
			if (moveSpeed == 0f)
			{
				return;
			}
			Vector3 vector = m_Owner.PositionVector;
			float num2 = moveSpeed * num;
			if (num2 <= 0f)
			{
				return;
			}
			int i;
			for (i = m_CurrentPathIndex; i < m_PathList.Count; i++)
			{
				float num3 = Misc.Distance(vector, m_PathList[i]);
				if (num3 > num2)
				{
					vector = Misc.GetPosWithVectorDistance(vector, m_PathList[i] - vector, num2);
					break;
				}
				num2 -= num3;
				vector = m_PathList[i];
				m_Parent.LastPosOnNavi = m_PathList[i];
			}
			PosWithRot posWithRot = new PosWithRot();
			posWithRot.x = vector.x;
			posWithRot.y = vector.y;
			posWithRot.z = vector.z;
			posWithRot.pitch = m_Owner.Position.pitch;
			PosWithRot posWithRot2 = posWithRot;
			bool flag = Misc.IsInFall(m_Owner.PositionVector, vector);
			if (!m_FixLookAtTarget && m_Owner.PositionVector != vector)
			{
				if (m_CalcAngle && !flag)
				{
					posWithRot2.yaw = Misc.GetDirectionAngle(m_Owner.PositionVector, vector);
				}
				else
				{
					posWithRot2.yaw = m_Owner.Position.yaw;
				}
			}
			posWithRot2.pitch = m_Owner.Position.pitch;
			m_Owner.SetPosition(posWithRot2, ActorMoveCause.Move);
			float num4 = Misc.Distance(m_Owner.PositionVector, posWithRot2.toVector3());
			if (num4 > 0f)
			{
				m_Parent.AccumulateMoveDistance(num4);
			}
			if (!_fallBottomPos.Equals(Vector3.zero) && MathF.Abs(posWithRot2.y - _fallBottomPos.y) < 0.2f)
			{
				_fallBottomPos = Vector3.zero;
			}
			if (m_FixLookAtTarget && !flag)
			{
				TurnToTarget();
			}
			if (i == m_PathList.Count)
			{
				StopMove(sync: true);
				return;
			}
			if (i != m_CurrentPathIndex)
			{
				m_CurrentPathIndex = i;
			}
			PosWithRot posWithRot3 = new PosWithRot();
			posWithRot3.x = m_PathList[m_CurrentPathIndex].x;
			posWithRot3.y = m_PathList[m_CurrentPathIndex].y;
			posWithRot3.z = m_PathList[m_CurrentPathIndex].z;
			posWithRot3.yaw = m_Owner.Position.yaw;
			posWithRot3.pitch = m_Owner.Position.pitch;
			PosWithRot futurePos = posWithRot3;
			SendMoveStartPacket(futurePos);
			m_Owner.AttachControlUnit.OnMove();
			m_LastSyncPosition = m_Owner.Position.Clone();
		}

		public float GetPathDistanceToPosition(Vector3 pos)
		{
			if (base.IsMoving && m_TargetPos.Equals(pos) && m_PathList.Count != 0)
			{
				return GetPathDistance(m_PathList, m_CurrentPathIndex);
			}
			List<Vector3> outPath = new List<Vector3>();
			if (!FindPath(pos, ref outPath) || outPath.Count == 0)
			{
				return 0f;
			}
			return GetPathDistance(outPath, 0);
		}

		private float GetPathDistance(List<Vector3> path, int pathIndex)
		{
			if (pathIndex >= path.Count || path.Count == 0)
			{
				return 0f;
			}
			float num = Vector3.Distance(m_Owner.PositionVector, path[pathIndex]);
			for (int i = pathIndex; i < path.Count - 1; i++)
			{
				num += Vector3.Distance(path[i], path[i + 1]);
			}
			return num;
		}

		public override void OnChangedPosition()
		{
			if (base.IsMoving && m_PathList.Count != 0 && m_Owner.CanAction(VActorActionType.Move) == MsgErrorCode.Success)
			{
				StartMove(m_TargetPos, calcAngle: true, resetPath: true);
			}
		}

		public void CopyDestPath(ref List<PosWithRot> path)
		{
			path.Clear();
			foreach (Vector3 path2 in m_PathList)
			{
				path.Add(path2.toPosWithRot(0f));
			}
		}

		public override void SyncMoveStopSig()
		{
			MoveStopSig msg = new MoveStopSig
			{
				actorID = m_Owner.ObjectID,
				actorMoveType = m_Parent.GetMoveType(),
				targetID = m_TraceActorID,
				currentPos = m_Owner.Position.Clone()
			};
			m_Owner.SendInSight(msg, includeSelf: true);
		}

		public override void OnForceSyncPosition(TeleportReason teleportReason)
		{
		}

		public override void OnSetMoveType(ActorMoveType moveType)
		{
		}

		public override void OnChangeImmutableStats()
		{
		}

		public override void StopMove(bool sync)
		{
			if (base.IsMoving)
			{
				Reset();
				if (sync)
				{
					SyncMoveStopSig();
				}
				m_Parent.OnStopMove();
			}
		}

		public override PosWithRot GetPrevPos()
		{
			return m_LastSyncPosition.Clone();
		}

		public override void SyncMoveForce(PosWithRot pos, ActorMoveType moveType)
		{
			if (m_PathList.Count == 0)
			{
				Reset();
			}
			m_Owner.SetPosition(pos, ActorMoveCause.Attach);
			SendMoveStartPacket(pos);
			m_LastSyncPosition = m_Owner.Position.Clone();
		}
	}

	private VCreature _creature;

	private ActorMoveType _moveType;

	private DirectMoveContext m_DirectMoveContext;

	private PathMoveContext m_PathMoveContext;

	private IMoveContext? m_CurrentMoveContext;

	private long _sprintElapsed;

	private (bool flag, long startTime) _jumpState = (flag: false, startTime: 0L);

	private FallChecker m_FallChecker;

	private long _tickAccumulate;

	public VActorControllerType type { get; }

	public ActorMotionType ActorMotionType { get; private set; }

	public PosWithRot DebugDestination { get; private set; } = new PosWithRot();

	public Vector3 LastPosOnNavi { get; private set; }

	public bool IsMoving => m_CurrentMoveContext?.IsMoving ?? false;

	public bool IsSprint { get; private set; }

	public float TotalMoveDistance { get; private set; }

	public float LastAggroDistance { get; private set; }

	public long LastAggroBroadcastTime { get; private set; }

	public MovementController(VActor actor)
	{
		if (!(actor is VCreature creature))
		{
			throw new Exception("actor is not VCreature");
		}
		_creature = creature;
		m_DirectMoveContext = new DirectMoveContext(this, _creature);
		m_PathMoveContext = new PathMoveContext(this, _creature);
		SyncDebugPosition();
	}

	public void Initialize()
	{
		_moveType = ActorMoveType.None;
		LastPosOnNavi = _creature.Position.toVector3();
		IsSprint = false;
		_sprintElapsed = 0L;
	}

	private void UpdateJump()
	{
		if (_jumpState.flag && Hub.s.timeutil.GetCurrentTickMilliSec() - _jumpState.startTime >= 800)
		{
			_jumpState = (flag: false, startTime: 0L);
		}
	}

	public void Update(long delta)
	{
		bool flag = true;
		try
		{
			_tickAccumulate += delta;
			if (_tickAccumulate < 100)
			{
				return;
			}
			flag = false;
			BroadcastAggro();
			UpdateJump();
			IMoveContext? currentMoveContext = m_CurrentMoveContext;
			if (currentMoveContext != null && currentMoveContext.IsMoving && _creature.CanAction(VActorActionType.Move) != MsgErrorCode.Success)
			{
				m_CurrentMoveContext?.StopMove(sync: true);
				return;
			}
			UpdateSprint(_tickAccumulate);
			float num = m_FallChecker.CheckLanding();
			if (num > 0f)
			{
				long num2 = (long)((double?)((float?)_creature.StatControlUnit?.GetSpecificStatValue(StatType.HP) * num) * 0.01).GetValueOrDefault();
				_creature.StatControlUnit?.AdjustHP(-num2);
			}
			if (_creature.CanAction(VActorActionType.Move) == MsgErrorCode.Success)
			{
				m_CurrentMoveContext?.OnUpdate(_tickAccumulate);
			}
		}
		finally
		{
			if (!flag)
			{
				_tickAccumulate = 0L;
			}
		}
	}

	public void Dispose()
	{
		m_CurrentMoveContext?.StopMove(sync: false);
	}

	private void SyncDebugPosition()
	{
		DebugDestination = _creature.Position;
	}

	public void WaitInitDone()
	{
	}

	public void StopMove(bool sync = true, bool needToCancel = false)
	{
		if (_jumpState.flag && needToCancel)
		{
			_jumpState = (flag: false, startTime: 0L);
			_creature.SendInSight(new CancelJumpSig
			{
				actorID = _creature.ObjectID
			});
		}
		m_CurrentMoveContext?.StopMove(sync);
	}

	public void OnStopMove()
	{
		SetMoveType(ActorMoveType.None);
		SyncDebugPosition();
	}

	public void SetTargetID(int targetID)
	{
		m_PathMoveContext.SetTraceActorID(targetID);
	}

	public bool CanForceMoveSync(PosWithRot endPos)
	{
		float num = Misc.Distance(IsMoving ? _creature.Position.toVector3() : endPos.toVector3(), endPos.toVector3(), ignoreHeight: true);
		float moveSpeed = (_creature.StatControlUnit ?? throw new Exception("StatControlUnit is null in MovementController")).GetMoveSpeed();
		return num <= moveSpeed * 2f;
	}

	public void TurnToTargetPos(Vector3 targetPos)
	{
		if (!(_creature.PositionVector == targetPos))
		{
			float directionAngle = Misc.GetDirectionAngle(_creature.PositionVector, targetPos);
			TurnAngle(directionAngle, _creature.Position.pitch);
		}
	}

	public void TurnAngle(float angle, float pitch)
	{
		if (_creature.Position.yaw != angle || _creature.Position.pitch != pitch)
		{
			_creature.SetAngle(angle);
			_creature.SetPitch(pitch);
			_creature.SendInSight(new ChangeViewPointSig
			{
				actorID = _creature.ObjectID,
				angle = angle,
				pitch = pitch
			});
		}
	}

	public void SetMoveType(ActorMoveType moveType)
	{
		if (_moveType != moveType)
		{
			_moveType = moveType;
			m_CurrentMoveContext?.OnSetMoveType(moveType);
		}
	}

	public void ForceSyncPosition(TeleportReason reason)
	{
		m_CurrentMoveContext?.OnForceSyncPosition(reason);
		Logger.RWarn($"[ForceSyncPosition] TeleportSig : {reason}, PositionVector : {_creature.Position.toVector3()}");
		m_FallChecker.Pause();
		_creature.SendInSight(new TeleportSig
		{
			actorID = _creature.ObjectID,
			pos = _creature.Position.Clone(),
			reason = reason
		}, includeSelf: true);
	}

	public void OnChangePosition()
	{
		m_CurrentMoveContext?.OnChangedPosition();
	}

	public void OnChangeImmutableStats()
	{
		m_CurrentMoveContext?.OnChangeImmutableStats();
	}

	private void ChangeContext(MoveContextType type)
	{
		if (m_CurrentMoveContext == null || m_CurrentMoveContext.MoveContextType != type)
		{
			m_CurrentMoveContext?.StopMove(sync: false);
			m_CurrentMoveContext = type switch
			{
				MoveContextType.Direct => m_DirectMoveContext, 
				MoveContextType.Path => m_PathMoveContext, 
				_ => throw new NotImplementedException(), 
			};
		}
	}

	public PathMoveResult PathMoveStart(Vector3 targetPos, ActorMoveType moveType = ActorMoveType.Walk, bool calcAngle = true, bool fixLookAt = false)
	{
		if (_creature.CanAction(VActorActionType.Move) != MsgErrorCode.Success)
		{
			return PathMoveResult.Fail;
		}
		SetMoveType(moveType);
		if (m_CurrentMoveContext == m_DirectMoveContext)
		{
			m_DirectMoveContext.StopMove(sync: true);
		}
		m_CurrentMoveContext = m_PathMoveContext;
		m_PathMoveContext.SetFixLookAtTarget(fixLookAt);
		return m_PathMoveContext.StartMove(targetPos, calcAngle);
	}

	public float CheckFallDamage(Vector3 before, Vector3 after, bool stopped = false)
	{
		float num = m_FallChecker.UpdateMove(before, after, stopped);
		if (num > 0f)
		{
			if (stopped)
			{
				m_FallChecker.Reset();
			}
			return num;
		}
		float totalFallDistance = m_FallChecker.TotalFallDistance;
		float num2 = Hub.s.dataman.ExcelDataManager.Consts.C_FallInstanceDeathDistance;
		if (totalFallDistance > num2)
		{
			Logger.RWarn($"[CheckFallDamage] Fatal Fall Damage : {totalFallDistance}");
			m_FallChecker.Reset();
			return 100f;
		}
		return 0f;
	}

	public MsgErrorCode DirectMoveStart(MoveStartReq msg)
	{
		bool flag = false;
		try
		{
			MsgErrorCode msgErrorCode = _creature.CanAction(VActorActionType.Move);
			if (msgErrorCode != MsgErrorCode.Success)
			{
				return msgErrorCode;
			}
			PosWithRot posWithRot = msg.basePositionPrev;
			if (m_CurrentMoveContext != null)
			{
				posWithRot = m_CurrentMoveContext.GetPrevPos();
			}
			if (!_creature.VRoom.ValidPosition(posWithRot.toVector3()) || !_creature.VRoom.ValidPosition(msg.basePositionCurr.toVector3()) || (msg.futureTime != 0L && !_creature.VRoom.ValidPosition(msg.basePositionFuture.toVector3())))
			{
				Logger.RError($"[DirectMoveStart] Invalid Position. prevPos:{posWithRot} currentPos:{msg.basePositionCurr} futurePos:{msg.basePositionFuture}");
				flag = true;
				return MsgErrorCode.InvalidPosition;
			}
			ActorMoveType moveType = GetMoveType();
			if (msg.actorMoveType == ActorMoveType.Walk && IsSprint)
			{
				SetMoveType(ActorMoveType.Walk);
				if (!StopSprint())
				{
					return MsgErrorCode.InvalidMoveType;
				}
			}
			else if (msg.actorMoveType == ActorMoveType.Run && !IsSprint)
			{
				SetMoveType(ActorMoveType.Run);
				if (!StartSprint())
				{
					return MsgErrorCode.InvalidMoveType;
				}
			}
			else
			{
				if (msg.actorMoveType == ActorMoveType.Attached)
				{
					AttachController? attachControlUnit = _creature.AttachControlUnit;
					if (attachControlUnit != null && !attachControlUnit.IsAttached())
					{
						goto IL_016b;
					}
				}
				if (msg.actorMoveType != ActorMoveType.Attached)
				{
					AttachController? attachControlUnit2 = _creature.AttachControlUnit;
					if (attachControlUnit2 != null && attachControlUnit2.IsAttached())
					{
						goto IL_016b;
					}
				}
			}
			if (_creature.OccupiedLevelObjectInfo != null)
			{
				return MsgErrorCode.CantMove;
			}
			if (m_CurrentMoveContext != m_DirectMoveContext)
			{
				m_PathMoveContext.StopMove(sync: true);
				m_CurrentMoveContext = m_DirectMoveContext;
			}
			SetMoveType(msg.actorMoveType);
			msgErrorCode = m_DirectMoveContext.StartMove(posWithRot, msg.basePositionCurr, msg.basePositionFuture, msg.futureTime, msg.pitch, 0L, msg.targetActorID, msg.transID, moveType, msg.actorMoveType);
			if (msgErrorCode != MsgErrorCode.Success)
			{
				flag = true;
				return msgErrorCode;
			}
			_creature.SendToMe(new MoveStartRes(msg.hashCode)
			{
				errorCode = msgErrorCode,
				transID = msg.transID
			});
			return MsgErrorCode.Success;
			IL_016b:
			return MsgErrorCode.InvalidMoveType;
		}
		finally
		{
			if (flag)
			{
				ForceSyncPosition(TeleportReason.ForceMoveSync);
			}
		}
	}

	public void DirectMoveStop(MoveStopReq msg)
	{
		MsgErrorCode msgErrorCode = MsgErrorCode.Success;
		bool flag = false;
		try
		{
			if (m_CurrentMoveContext != m_DirectMoveContext)
			{
				msgErrorCode = MsgErrorCode.InvalidMoveType;
				return;
			}
			if (_creature.CanAction(VActorActionType.Move) != MsgErrorCode.Success)
			{
				msgErrorCode = MsgErrorCode.CantAction;
				return;
			}
			if (!_creature.VRoom.ValidPosition(msg.currentPos.toVector3()))
			{
				Logger.RError($"[DirectMoveStop] Invalid Position. currentPos:{msg.currentPos}");
				flag = true;
				msgErrorCode = MsgErrorCode.InvalidPosition;
				return;
			}
			msgErrorCode = m_DirectMoveContext.OnMoveStopReq(_creature.SavedPosition, msg.currentPos);
			if (msgErrorCode != MsgErrorCode.Success)
			{
				flag = true;
			}
		}
		finally
		{
			_creature.SendToMe(new MoveStopRes(msg.hashCode)
			{
				errorCode = msgErrorCode
			});
			if (msgErrorCode != MsgErrorCode.Success && flag)
			{
				ForceSyncPosition(TeleportReason.ForceMoveSync);
			}
		}
	}

	public void SetTraceTarget(int objectID)
	{
		m_PathMoveContext.SetTraceActorID(objectID);
	}

	public void CopyDestPath(ref List<PosWithRot> path)
	{
		m_PathMoveContext.CopyDestPath(ref path);
	}

	public float GetPathDistanceToPosition(Vector3 position)
	{
		return m_PathMoveContext.GetPathDistanceToPosition(position);
	}

	public bool ValidateMoveDistance(PosWithRot endPos)
	{
		float num = Misc.Distance(_creature.Position.toVector3(), endPos.toVector3(), ignoreHeight: true);
		float moveSpeed = (_creature.StatControlUnit ?? throw new Exception("StatControlUnit is null in MovementController")).GetMoveSpeed();
		return num <= moveSpeed * 2f;
	}

	public void PostUpdate(long deltaTime)
	{
	}

	public MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0)
	{
		switch (actionType)
		{
		case VActorActionType.MoveSkill:
		case VActorActionType.Skill:
		case VActorActionType.Looting:
		case VActorActionType.Emotion:
		case VActorActionType.ScrapMotion:
		case VActorActionType.UseLevelObject:
			if (!_jumpState.flag)
			{
				return MsgErrorCode.Success;
			}
			return MsgErrorCode.CantAction;
		default:
			return MsgErrorCode.Success;
		}
	}

	public MsgErrorCode SetSprint(bool flag, int hashCode)
	{
		bool flag2 = false;
		if (!((!flag) ? StopSprint() : StartSprint()))
		{
			return MsgErrorCode.InvalidMoveType;
		}
		_creature.SendToMe(new ToggleSprintRes(hashCode));
		return MsgErrorCode.Success;
	}

	private bool CanSprint()
	{
		ActorMoveType moveType = _moveType;
		if ((uint)moveType <= 2u)
		{
			return true;
		}
		return false;
	}

	private bool StartSprint()
	{
		if (!CanSprint())
		{
			return false;
		}
		if (IsSprint)
		{
			return true;
		}
		StatController? statControlUnit = _creature.StatControlUnit;
		if (statControlUnit != null && statControlUnit.GetCurrentConta() < 0)
		{
			return false;
		}
		if (_creature.StatControlUnit?.GetCurrentStamina() < Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumeValue)
		{
			return false;
		}
		IsSprint = true;
		_sprintElapsed = 0L;
		_creature.StatControlUnit?.SetSprintMode(flag: true);
		_creature.SendInSight(new ChangeSprintModeSig
		{
			actorID = _creature.ObjectID,
			isSprint = true
		}, includeSelf: true);
		if (IsMoving)
		{
			_creature.StatControlUnit?.ConsumeStamina(Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumeValue);
		}
		return true;
	}

	private bool StopSprint()
	{
		if (!IsSprint)
		{
			return true;
		}
		IsSprint = false;
		_sprintElapsed = 0L;
		_creature.StatControlUnit?.SetSprintMode(flag: false);
		_creature.SendInSight(new ChangeSprintModeSig
		{
			actorID = _creature.ObjectID,
			isSprint = false
		}, includeSelf: true);
		return true;
	}

	private void UpdateSprint(long delta)
	{
		if (!IsSprint || !IsMoving)
		{
			return;
		}
		if (!CanSprint())
		{
			StopSprint();
			return;
		}
		_sprintElapsed += delta;
		if (_sprintElapsed >= Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumePeriod)
		{
			_sprintElapsed -= Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumePeriod;
			if (_creature.StatControlUnit?.GetCurrentStamina() < Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumeValue)
			{
				StopSprint();
			}
			else
			{
				_creature.StatControlUnit?.ConsumeStamina(Hub.s.dataman.ExcelDataManager.Consts.C_RunStaminaConsumeValue);
			}
		}
	}

	public void HandleJump()
	{
		MsgErrorCode msgErrorCode = _creature.CanAction(VActorActionType.Jump);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			_creature.SendToMe(new JumpRes
			{
				errorCode = msgErrorCode
			});
			return;
		}
		if (_jumpState.flag)
		{
			_creature.SendToMe(new JumpRes
			{
				errorCode = MsgErrorCode.DuplicateJumpAction
			});
			return;
		}
		_jumpState = (flag: true, startTime: Hub.s.timeutil.GetCurrentTickMilliSec());
		_creature.EmotionControlUnit?.OnMove();
		_creature.SendToMe(new JumpRes
		{
			errorCode = MsgErrorCode.Success
		});
		_creature.SendInSight(new JumpSig
		{
			actorID = _creature.ObjectID
		});
	}

	public void HandleChangeViewPoint(ChangeViewPointReq msg)
	{
		MsgErrorCode msgErrorCode = _creature.CanAction(VActorActionType.Move);
		if (msgErrorCode != MsgErrorCode.Success)
		{
			_creature.SendToMe(new ChangeViewPointRes(msg.hashCode)
			{
				errorCode = msgErrorCode
			});
			return;
		}
		_creature.SetPitch(msg.pitch);
		_creature.SetAngle(msg.angle);
		_creature.SendToMe(new ChangeViewPointRes(msg.hashCode));
		_creature.SendInSight(new ChangeViewPointSig
		{
			actorID = _creature.ObjectID,
			pitch = msg.pitch,
			angle = msg.angle
		});
	}

	public string GetDebugString()
	{
		return string.Empty;
	}

	public void SyncMoveForce(PosWithRot pos, ActorMoveType type)
	{
		m_CurrentMoveContext?.SyncMoveForce(pos, type);
	}

	public void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}

	public ActorMoveType GetMoveType()
	{
		AttachController? attachControlUnit = _creature.AttachControlUnit;
		if (attachControlUnit != null && attachControlUnit.IsAttached())
		{
			return ActorMoveType.Attached;
		}
		return _moveType;
	}

	public void AccumulateMoveDistance(float distance)
	{
		TotalMoveDistance += distance;
		LastAggroDistance += distance;
		_creature.InventoryControlUnit?.OnMoveDistance(distance);
	}

	public void BroadcastAggro()
	{
		long currentTickMilliSec = Hub.s.timeutil.GetCurrentTickMilliSec();
		if (currentTickMilliSec - LastAggroBroadcastTime < Hub.s.dataman.ExcelDataManager.Consts.C_MovementAggroCapturePeriod)
		{
			return;
		}
		LastAggroBroadcastTime = currentTickMilliSec;
		if (LastAggroDistance > Hub.s.dataman.ExcelDataManager.Consts.C_MovementAggroMinDistanceThreshold)
		{
			_creature.VRoom.IterateAllMonsterInRange(_creature.PositionVector, 2000f, delegate(VMonster monster)
			{
				monster.AIControlUnit?.AddMovementAggroPoint(_creature, LastAggroDistance);
			});
		}
		LastAggroDistance = 0f;
	}

	public bool FindPath(Vector3 targetPos, ref List<Vector3> path)
	{
		return m_PathMoveContext.FindPath(targetPos, ref path);
	}
}
