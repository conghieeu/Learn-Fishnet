using System.Collections.Immutable;

public class ActorAnimInfo
{
	public readonly string PrefabName;

	public readonly ImmutableDictionary<string, SkillAnimInfo> SkillAnims;

	public ActorAnimInfo(string prefabName, ImmutableDictionary<string, SkillAnimInfo> skillAnims)
	{
		PrefabName = prefabName;
		SkillAnims = skillAnims;
	}
}
