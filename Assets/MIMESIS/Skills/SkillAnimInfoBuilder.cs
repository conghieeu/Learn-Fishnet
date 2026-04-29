using System;
using System.Collections.Generic;

public class SkillAnimInfoBuilder
{
	public readonly int ID;

	public readonly double Length;

	public readonly List<AnimNotifyHitCheck> HitChecks = new List<AnimNotifyHitCheck>();

	public readonly List<AnimNotifyProjectile> Projectiles = new List<AnimNotifyProjectile>();

	public readonly List<AnimNotifyFieldSkill> FieldSkills = new List<AnimNotifyFieldSkill>();

	public readonly List<AnimNotifyDestroyWeapon> WeaponDestroyes = new List<AnimNotifyDestroyWeapon>();

	public readonly List<AnimNotifyImmuneApplier> ImmuneAppliers = new List<AnimNotifyImmuneApplier>();

	public readonly List<AnimNotifyActivateAura> ActivateAuras = new List<AnimNotifyActivateAura>();

	public readonly List<AnimNotifyReloadWeapon> ReloadWeapons = new List<AnimNotifyReloadWeapon>();

	public SkillAnimInfoBuilder(int id, double length)
	{
		ID = id;
		Length = length;
	}

	public void Sort()
	{
		Comparison<AnimNotifyInfo> comparison = (AnimNotifyInfo a, AnimNotifyInfo b) => a.Start.CompareTo(b.Start);
		HitChecks.Sort(comparison);
		Projectiles.Sort(comparison);
		FieldSkills.Sort(comparison);
		WeaponDestroyes.Sort(comparison);
		ImmuneAppliers.Sort(comparison);
		ActivateAuras.Sort(comparison);
		ReloadWeapons.Sort(comparison);
	}
}
