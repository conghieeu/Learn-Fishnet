using System.Collections.Generic;
using Bifrost.Cooked;
using ReluProtocol.Enum;

public class MappedStats : CommonStats
{
	public MappedStats()
		: base(StatCategory.Mapped)
	{
	}

	public bool LoadBaseStats(ActorType type, int masterDataID, int charLevel = 1)
	{
		GameActorStats gameActorStats = null;
		switch (type)
		{
		case ActorType.Player:
		{
			PlayerMasterInfo playerInfo = Hub.s.dataman.ExcelDataManager.GetPlayerInfo(charLevel);
			if (playerInfo == null)
			{
				return false;
			}
			gameActorStats = new GameActorStats(playerInfo);
			break;
		}
		case ActorType.Monster:
		{
			MonsterInfo monsterInfo = Hub.s.dataman.ExcelDataManager.GetMonsterInfo(masterDataID);
			if (monsterInfo == null)
			{
				return false;
			}
			gameActorStats = new GameActorStats(monsterInfo);
			break;
		}
		}
		if (gameActorStats == null)
		{
			return false;
		}
		elements[StatType.HP].Set(gameActorStats.hp);
		elements[StatType.Attack].Set(gameActorStats.attack);
		elements[StatType.MoveSpeedWalk].Set(gameActorStats.moveSpeedWalk);
		elements[StatType.MoveSpeedRun].Set(gameActorStats.moveSpeedRun);
		elements[StatType.AbnormalTriggerGauge].Set(gameActorStats.maxGroggyGauge);
		elements[StatType.Stamina].Set(Hub.s.dataman.ExcelDataManager.Consts.C_MaxStaminaValue);
		return true;
	}

	public bool LoadEquipStats(List<EquipmentItemElement> equips)
	{
		return true;
	}
}
