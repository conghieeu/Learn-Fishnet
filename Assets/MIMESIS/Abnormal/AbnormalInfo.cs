using System.Collections.Immutable;
using Bifrost.AbnormalData;

public class AbnormalInfo
{
	public readonly int MasterID;

	public readonly string Name = "";

	public readonly bool Overlap;

	public readonly bool Dispelable;

	public readonly ImmutableDictionary<int, IAbnormalElementInfo> ElementList = ImmutableDictionary<int, IAbnormalElementInfo>.Empty;

	public readonly int Duration;

	public readonly int ChainingAbnormalID;

	public readonly int SkillTargetEffectId;

	public AbnormalInfo(AbnormalData_MasterData masterData)
	{
		MasterID = masterData.id;
		Name = masterData.name;
		Overlap = masterData.overlap;
		Dispelable = masterData.dispelable;
		Duration = masterData.duration;
		ChainingAbnormalID = masterData.chaining_abnormal_id;
		SkillTargetEffectId = masterData.link_skill_target_effect_data_id;
		ImmutableDictionary<int, IAbnormalElementInfo>.Builder builder = ImmutableDictionary.CreateBuilder<int, IAbnormalElementInfo>();
		foreach (AbnormalData_element item in masterData.AbnormalData_elementval)
		{
			switch (item.category)
			{
			case 1:
				builder.Add(item.index, new CCElementInfo(item));
				break;
			case 2:
				builder.Add(item.index, new AbnormalStatsElementInfo(item));
				break;
			case 3:
				builder.Add(item.index, new DispelElementInfo(item));
				break;
			case 4:
				builder.Add(item.index, new ImmuneElementInfo(item));
				break;
			default:
				Logger.RError("Invalid AbnormalCategory");
				break;
			}
		}
		ElementList = builder.ToImmutable();
	}
}
