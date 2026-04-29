using ReluNetwork.ConstEnum;
using ReluProtocol;
using ReluProtocol.Enum;

public class VInteractor : VActor
{
	public VInteractor(int actorID, int masterID, string actorName, PosWithRot position, bool isIndoor, IVroom room, ReasonOfSpawn reasonOfSpawn)
		: base(ActorType.Interactor, actorID, masterID, actorName, position, isIndoor, room, 0L, reasonOfSpawn)
	{
	}

	public override void CollectDebugInfo(ref DebugInfoSig sig)
	{
	}

	public override void FillSightInSig(ref SightInSig sig)
	{
	}

	public override bool IsAliveStatus()
	{
		return true;
	}

	public override SendResult SendToMe(IMsg msg)
	{
		return SendResult.Success;
	}
}
