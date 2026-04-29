using ReluProtocol.Enum;
using UnityEngine;

namespace Mimic.Actors
{
	public abstract class ProjectileActor : MonoBehaviour, IActor
	{
		public ActorType ActorType { get; } = ActorType.Projectile;

		public int ActorID { get; private set; }

		public Vector3 InitialPosition { get; private set; } = Vector3.zero;

		public Vector3 InitialForward { get; private set; } = Vector3.forward;

		public ProjectileState State { get; protected set; }

		public void StartDestroy()
		{
			State = ProjectileState.Destroying;
			OnDestroyStarted();
		}

		public abstract void StartMove();

		[PacketHandler(false)]
		protected abstract void OnMoveStartSig(MoveStartSig sig);

		protected abstract void OnDestroyStarted();

		protected void Initialize(int actorID, Vector3 initialPosition, Vector3 initialForward)
		{
			ActorID = actorID;
			InitialPosition = initialPosition;
			InitialForward = initialForward;
			base.transform.SetPositionAndRotation(initialPosition, Quaternion.LookRotation(initialForward));
		}
	}
}
