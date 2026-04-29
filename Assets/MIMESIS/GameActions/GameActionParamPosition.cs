using ReluProtocol;

public class GameActionParamPosition : IGameActionParam
{
	public PosWithRot Position { get; private set; }

	public bool IsIndoor { get; private set; }

	public GameActionParamPosition(PosWithRot position, bool isIndoor)
	{
		Position = position;
		IsIndoor = isIndoor;
	}
}
