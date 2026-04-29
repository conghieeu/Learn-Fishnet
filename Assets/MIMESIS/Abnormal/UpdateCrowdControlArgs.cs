using ReluProtocol;

public sealed class UpdateCrowdControlArgs : VActorEventArgs
{
	public readonly PosWithRot TargetPosition;

	public readonly PosWithRot RegisterPosition;

	public readonly long PushTime;

	public readonly long ElapsedTimeTotal;

	public UpdateCrowdControlArgs(PosWithRot targetPosition, PosWithRot registerPosition, long pushTime, long elapsedTimeTotal)
		: base(VActorEventType.UpdateCrowdControl)
	{
		TargetPosition = targetPosition.Clone();
		RegisterPosition = registerPosition.Clone();
		PushTime = pushTime;
		ElapsedTimeTotal = elapsedTimeTotal;
	}
}
