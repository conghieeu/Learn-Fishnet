public interface ITramUpgradeLevelObject
{
	bool IsUpgradeActive { get; }

	int TramUpgradeID { get; }

	void PrepareUpgradeEffect();

	void PlayUpgradeEffect();
}
