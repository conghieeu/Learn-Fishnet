using System;
using Bifrost.AbnormalData;
using Bifrost.ConstEnum;
using Bifrost.Cooked;

public class CCElementInfo : IAbnormalElementInfo
{
	public readonly CCType CCType;

	public readonly BattleActionInfo BattleActionInfo;

	public readonly BattleActionDistanceType DistanceType;

	public CCElementInfo(AbnormalData_element element)
		: base(element)
	{
		if (element.type == "BATTLE_ACTION")
		{
			BattleActionInfo battleActionData = Hub.s.dataman.ExcelDataManager.GetBattleActionData(element.sub_type);
			if (battleActionData == null)
			{
				Logger.RError($"CCElementInfo: Invalid BattleActionData for type '{element.sub_type}' in AbnormalData_element with index {element.index}.");
				CCType = CCType.None;
			}
			else
			{
				CCType = battleActionData.CCType;
				BattleActionInfo = battleActionData;
				DistanceType = (BattleActionDistanceType)element.move_origin;
			}
		}
		else if (!Enum.TryParse<CCType>(element.type, ignoreCase: true, out CCType))
		{
			Logger.RError($"BattleActionInfo: Invalid CCType '{element.type}' for AbnormalData_element with index {element.index}. Defaulting to None.");
		}
	}
}
