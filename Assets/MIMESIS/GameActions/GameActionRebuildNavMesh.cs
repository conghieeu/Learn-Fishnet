using Bifrost.ConstEnum;

public class GameActionRebuildNavMesh : GameAction
{
	public GameActionRebuildNavMesh()
		: base(DefAction.REBUILD_NAVMESH)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionRebuildNavMesh();
	}

	public override IGameAction Clone()
	{
		return new GameActionRebuildNavMesh();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionRebuildNavMesh;
	}
}
