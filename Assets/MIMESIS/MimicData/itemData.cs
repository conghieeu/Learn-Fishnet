namespace MimicData
{
	public class itemData
	{
		public string name = "N/A";

		public string iconAssetAddress = "N/A";

		public string prefabAssetAddress = "N/A";

		public eItemType type = eItemType.Scrap;

		public eItemCarryType carryType = eItemCarryType.TwoHand;

		public eItemUseType useType;

		public eWeaponUsageType weaponUsageType;

		public float buyPrice = 100f;

		public float sellPrice = 100f;

		public float weight = 10f;

		public float durability = 100f;

		public float durabilityDecreaseRate;

		public float maxBattery;

		public float batteryUsage;

		public float minChargeTime;

		public float maxChargeTime;

		public float overChargeTime;
	}
}
