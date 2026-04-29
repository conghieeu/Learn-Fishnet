using Bifrost.ConstEnum;

public class GameActionChargeItem : GameAction
{
	public GameActionChargeItem()
		: base(DefAction.CHARGE_ITEM)
	{
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionChargeItem();
	}

	public override IGameAction Clone()
	{
		return new GameActionChargeItem();
	}

	public override bool Correct(IGameAction action)
	{
		return action is GameActionChargeItem;
	}

	public override string ToString()
	{
		return $"{base.ActionType}";
	}
}
