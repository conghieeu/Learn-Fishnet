public class GameCondtionParamActor : IGameCondtionParam
{
	public int ActorID { get; private set; }

	public GameCondtionParamActor(int actorID)
	{
		ActorID = actorID;
	}
}
