using Bifrost.ConstEnum;

public class GameActionTeleportToSpawnPoint : GameAction
{
	public GameActionTeleportToSpawnPoint()
		: base(DefAction.TELEPORT_TO_SPAWNPOINT)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionTeleportToSpawnPoint();
	}

	public override IGameAction Clone()
	{
		return new GameActionTeleportToSpawnPoint();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionTeleportToSpawnPoint;
	}

	public override string ToString()
	{
		return $"{base.ActionType}";
	}
}
