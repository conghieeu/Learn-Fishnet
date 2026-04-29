using ReluProtocol;
using ReluProtocol.Enum;

namespace WorldServer.Controller.Cooltime.Info
{
	public class SkillCooltimeElement : ICooltimeElement
	{
		public readonly int SkillMasterID;

		public SkillCooltimeElement(int skillMasterID, long syncID, long duration, bool global, bool sync)
			: base(syncID, duration, global, sync)
		{
			SkillMasterID = skillMasterID;
		}

		public override void FillCooltimeSig(ref CooltimeSig sig, CooltimeChangeType type)
		{
			sig.skillCooltimeInfos.Add(new SkillCooltimeInfo
			{
				cooltimeType = CooltimeType.Skill,
				skillMasterID = SkillMasterID,
				changeType = type,
				endTime = base.EndTimestamp,
				global = base.Global,
				remainTime = ((type == CooltimeChangeType.Remove) ? 0f : Hub.s.timeutil.ChangeTimeMilli2Sec(base.RemainDuration))
			});
		}
	}
}
