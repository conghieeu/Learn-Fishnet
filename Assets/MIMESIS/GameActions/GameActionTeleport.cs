using Bifrost.ConstEnum;

public class GameActionTeleport : GameAction
{
	public string TeleportStartPointCallSign { get; private set; }

	public GameActionTeleport(string teleportStartPointCallSign)
		: base(DefAction.TELEPORT)
	{
		TeleportStartPointCallSign = teleportStartPointCallSign;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionTeleport(TeleportStartPointCallSign);
	}

	public override IGameAction Clone()
	{
		return new GameActionTeleport(TeleportStartPointCallSign);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionTeleport gameActionTeleport)
		{
			return gameActionTeleport.TeleportStartPointCallSign == TeleportStartPointCallSign;
		}
		return false;
	}

	public override string ToString()
	{
		return $"{base.ActionType}:{TeleportStartPointCallSign}";
	}
}
