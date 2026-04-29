using Bifrost.Cooked;

public class GameActorStats
{
	public readonly long hp;

	public readonly long attack;

	public readonly long moveSpeedWalk;

	public readonly long moveSpeedRun;

	public readonly long maxGroggyGauge;

	public readonly long voiceResonance;

	public readonly long stamina;

	public GameActorStats(PlayerMasterInfo masterData)
	{
		hp = masterData.HP;
		attack = masterData.AttackPower;
		moveSpeedRun = masterData.MoveSpeedRun;
		moveSpeedWalk = masterData.MoveSpeedWalk;
		maxGroggyGauge = 0L;
	}

	public GameActorStats(MonsterInfo masterData)
	{
		hp = masterData.HP;
		attack = masterData.AttackPower;
		moveSpeedRun = masterData.MoveSpeedRun;
		moveSpeedWalk = masterData.MoveSpeedWalk;
		maxGroggyGauge = masterData.AbnormalTriggerThreshold;
	}
}
