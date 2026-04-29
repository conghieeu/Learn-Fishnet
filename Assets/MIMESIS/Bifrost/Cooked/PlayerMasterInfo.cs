using System.Collections.Immutable;
using Bifrost.PlayerData;

namespace Bifrost.Cooked
{
	public class PlayerMasterInfo
	{
		public readonly int MasterID;

		public readonly string Name;

		public readonly long HP;

		public readonly long AttackPower;

		public readonly long Defense;

		public readonly long MoveSpeedWalk;

		public readonly long MoveSpeedRun;

		public readonly ImmutableArray<int> Factions = ImmutableArray<int>.Empty;

		public readonly string FppPuppetName;

		public readonly string TppPuppetName;

		public readonly float MoveCollisionRadius;

		public readonly float HitCollisionRadius;

		public PlayerMasterInfo(PlayerData_MasterData data)
		{
			MasterID = data.id;
			Name = data.name;
			HP = data.hp;
			AttackPower = data.attack_power;
			Defense = data.defense;
			MoveSpeedWalk = data.move_speed_walk;
			MoveSpeedRun = data.move_speed_run;
			ImmutableArray<int>.Builder builder = ImmutableArray.CreateBuilder<int>();
			foreach (int faction in data.factions)
			{
				builder.Add(faction);
			}
			Factions = builder.ToImmutable();
			FppPuppetName = data.fpp_model_path;
			TppPuppetName = data.tpp_model_path;
			MoveCollisionRadius = data.move_radius;
			HitCollisionRadius = data.hit_radius;
		}
	}
}
