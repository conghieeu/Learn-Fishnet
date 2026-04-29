using System.Collections.Generic;
using System.Collections.Immutable;

public class SkillAnimInfo
{
	public readonly int ID;

	public readonly double Length;

	public readonly ImmutableArray<AnimNotifyHitCheck> HitChecks;

	public readonly ImmutableArray<AnimNotifyProjectile> Projectiles;

	public readonly ImmutableArray<AnimNotifyFieldSkill> FieldSkills;

	public readonly ImmutableArray<AnimNotifyDestroyWeapon> WeaponDestroyes;

	public readonly ImmutableArray<AnimNotifyImmuneApplier> ImmuneAppliers;

	public readonly ImmutableArray<AnimNotifyActivateAura> ActivateAuras;

	public readonly ImmutableArray<AnimNotifyReloadWeapon> ReloadWeapons;

	public SkillAnimInfo(SkillAnimInfoBuilder builder)
	{
		ID = builder.ID;
		Length = builder.Length;
		HitChecks = builder.HitChecks.ToImmutableArray();
		Projectiles = builder.Projectiles.ToImmutableArray();
		FieldSkills = builder.FieldSkills.ToImmutableArray();
		WeaponDestroyes = builder.WeaponDestroyes.ToImmutableArray();
		ImmuneAppliers = builder.ImmuneAppliers.ToImmutableArray();
		ActivateAuras = builder.ActivateAuras.ToImmutableArray();
		ReloadWeapons = builder.ReloadWeapons.ToImmutableArray();
	}

	public SortedDictionary<double, AnimNotifyInfo> GetAllData()
	{
		SortedDictionary<double, AnimNotifyInfo> sortedDictionary = new SortedDictionary<double, AnimNotifyInfo>();
		ImmutableArray<AnimNotifyHitCheck>.Enumerator enumerator = HitChecks.GetEnumerator();
		while (enumerator.MoveNext())
		{
			AnimNotifyHitCheck current = enumerator.Current;
			sortedDictionary.TryAdd(current.Start, current);
		}
		return sortedDictionary;
	}
}
