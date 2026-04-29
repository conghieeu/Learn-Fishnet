using System.Collections.Immutable;
using Bifrost.FieldSkillData;

namespace Bifrost.Cooked
{
	public class FieldSkillInfo
	{
		public readonly int MasterID;

		public ImmutableList<int> Factions = ImmutableList<int>.Empty;

		public readonly ImmutableDictionary<int, FieldSkillMemberInfo> FieldSkillMemberInfos;

		public FieldSkillInfo(FieldSkillData_MasterData masterData)
		{
			MasterID = masterData.id;
			ImmutableDictionary<int, FieldSkillMemberInfo>.Builder builder = ImmutableDictionary.CreateBuilder<int, FieldSkillMemberInfo>();
			foreach (FieldSkillData_field_info item in masterData.FieldSkillData_field_infoval)
			{
				FieldSkillMemberInfo fieldSkillMemberInfo = new FieldSkillMemberInfo(item);
				builder.Add(fieldSkillMemberInfo.Index, fieldSkillMemberInfo);
			}
			FieldSkillMemberInfos = builder.ToImmutable();
			ImmutableList<int>.Builder builder2 = ImmutableList.CreateBuilder<int>();
			foreach (int faction in masterData.factions)
			{
				builder2.Add(faction);
			}
			Factions = builder2.ToImmutable();
		}
	}
}
