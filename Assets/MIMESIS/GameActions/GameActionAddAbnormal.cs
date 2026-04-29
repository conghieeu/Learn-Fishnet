using Bifrost.ConstEnum;

public class GameActionAddAbnormal : GameAction
{
	public int AbnormalMasterID { get; private set; }

	public GameActionAddAbnormal(int abnormalMasterID)
		: base(DefAction.ADD_ABNORMAL)
	{
		AbnormalMasterID = abnormalMasterID;
	}

	public override void Clone(ref IGameAction? action)
	{
		action = new GameActionAddAbnormal(AbnormalMasterID);
	}

	public override IGameAction Clone()
	{
		return new GameActionAddAbnormal(AbnormalMasterID);
	}

	public override bool Correct(IGameAction action)
	{
		if (action is GameActionAddAbnormal gameActionAddAbnormal)
		{
			return gameActionAddAbnormal.AbnormalMasterID == AbnormalMasterID;
		}
		return false;
	}
}
