using System;
using ReluProtocol.Enum;

public interface IVActorController : IDisposable
{
	VActorControllerType type { get; }

	void Initialize();

	void Update(long delta);

	MsgErrorCode CanAction(VActorActionType actionType, int masterID = 0);

	void WaitInitDone();

	string GetDebugString();

	void CollectDebugInfo(ref DebugInfoSig sig);
}
