using System;
using Bifrost.BattleActionData;
using Bifrost.ConstEnum;

namespace Bifrost.Cooked
{
	public class BattleActionInfo
	{
		public readonly int MasterID;

		public readonly string Key;

		public readonly CCType CCType;

		public readonly float Distance;

		public readonly long MoveTime;

		public readonly long DownTime;

		public readonly bool TurnToAttacker;

		public BattleActionInfo(BattleActionData_MasterData masterData)
		{
			MasterID = masterData.id;
			Key = masterData.key;
			if (!Enum.TryParse<CCType>(masterData.type, ignoreCase: true, out CCType))
			{
				Logger.RError($"BattleActionInfo: Invalid CCType '{masterData.type}' for MasterID {MasterID}. Defaulting to None.");
			}
			Distance = (float)masterData.distance * 0.01f;
			MoveTime = masterData.move_time;
			DownTime = masterData.down_time;
			TurnToAttacker = masterData.turn_to_attacker;
		}
	}
}
