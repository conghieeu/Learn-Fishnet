namespace Bifrost.Cooked
{
	public class SkillPair
	{
		public readonly int SkillMasterIDNoGague;

		public readonly int SkillMasterIDWithGauge;

		public SkillPair(int skillMasterIDNoGauge, int skillMasterIDWithGauge)
		{
			SkillMasterIDNoGague = skillMasterIDNoGauge;
			SkillMasterIDWithGauge = skillMasterIDWithGauge;
		}
	}
}
