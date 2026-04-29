using UnityEngine;

namespace Mimic.Actors
{
	public sealed class PhysicsProjectileActor : ProjectileActor
	{
		[SerializeField]
		private float hitRadius = 1f;

		private MoveHint currentMoveHint;

		private MoveHint futureMoveHint;

		private Vector3 gravityVelocity = Vector3.zero;

		public bool SliceMeshWhenDestroy { get; private set; }

		public float Speed { get; private set; }

		public Vector3 Gravity { get; private set; } = Vector3.zero;

		public float Lifetime { get; private set; }

		private void Update()
		{
			switch (base.State)
			{
			case ProjectileState.Moving:
				UpdateMovingState();
				break;
			default:
				Logger.RError($"Unhandled state: {base.State}");
				break;
			case ProjectileState.Uninitialized:
			case ProjectileState.Created:
			case ProjectileState.Hit:
			case ProjectileState.Destroying:
				break;
			}
		}

		public void Initialize(int actorID, float speed, float gravity, float lifetime, bool sliceMeshWhenDestroy)
		{
			Initialize(actorID, base.transform.position, base.transform.forward);
			Speed = speed;
			Gravity = Vector3.up * gravity;
			Lifetime = lifetime;
			SliceMeshWhenDestroy = sliceMeshWhenDestroy;
			base.State = ProjectileState.Created;
		}

		public override void StartMove()
		{
			currentMoveHint = new MoveHint(base.transform.position, base.transform.rotation, Time.time);
			futureMoveHint = currentMoveHint;
			base.State = ProjectileState.Moving;
		}

		protected override void OnMoveStartSig(MoveStartSig sig)
		{
			float time = Time.time;
			currentMoveHint.UpdatePositionAndRotation(sig.basePositionCurr, time);
			float time2 = time + (float)sig.futureTime / 1000f;
			futureMoveHint.UpdatePositionAndRotation(sig.basePositionFuture, time2);
		}

		protected override void OnDestroyStarted()
		{
			if (SliceMeshWhenDestroy)
			{
				MeshSlicer componentInChildren = GetComponentInChildren<MeshSlicer>();
				if (componentInChildren != null)
				{
					componentInChildren.Switch(Vector3.up);
				}
			}
			Hub.s.residualObject.PreserveAllInChildren(base.gameObject.transform);
			base.gameObject.SetActive(value: false);
			Object.Destroy(base.gameObject);
		}

		private void UpdateMovingState()
		{
			float num = futureMoveHint.Time - currentMoveHint.Time;
			if (num <= 0f)
			{
				Logger.RWarn($"moveHintDeltaTime is less than or equal to zero: {num}", sendToLogServer: false, useConsoleOut: true, "projectile");
				return;
			}
			float t = (Time.time - currentMoveHint.Time) / (futureMoveHint.Time - currentMoveHint.Time);
			Vector3 position = Vector3.Lerp(currentMoveHint.Position, futureMoveHint.Position, t);
			Quaternion rotation = Quaternion.Lerp(currentMoveHint.Rotation, futureMoveHint.Rotation, t);
			base.transform.SetPositionAndRotation(position, rotation);
		}

		[PacketHandler(false)]
		private void OnDebugInfoSig(DebugInfoSig packet)
		{
			if (packet.actorID != base.ActorID)
			{
				Logger.RError($"[DebugInfoSig] PhysicsProjectileActor. packet.actorID != ActorID. {packet.actorID} != {base.ActorID}");
				return;
			}
			string.IsNullOrEmpty(packet.debugInfo);
			Hub.s.UpdateHitCheckVisualizations(packet.actorID, packet.hitCheckDrawInfos);
		}
	}
}
