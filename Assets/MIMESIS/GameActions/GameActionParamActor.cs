public class GameActionParamActor : IGameActionParam
{
	public int ActorID { get; private set; }

	public GameActionParamActor(int actorID)
	{
		ActorID = actorID;
	}
}
