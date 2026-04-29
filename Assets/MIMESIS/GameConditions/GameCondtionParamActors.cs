using System.Collections.Generic;

public class GameCondtionParamActors : IGameCondtionParam
{
	public List<int> ActorIDs { get; private set; }

	public GameCondtionParamActors(List<int> actorIDs)
	{
		ActorIDs = actorIDs.Clone();
	}
}
