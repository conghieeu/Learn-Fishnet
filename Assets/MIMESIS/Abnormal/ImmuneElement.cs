using System.Collections.Generic;
using Bifrost.ConstEnum;
using ReluProtocol;

public class ImmuneElement : IAbnormalElement
{
	private readonly AbnormalReason _immuneReason;

	public readonly ImmuneGrade Grade;

	public readonly ImmuneType Type;

	public ImmuneCategory ImmuneCategory { get; private set; }

	public CCType CCType { get; private set; }

	public StatType ImmutableStatType { get; private set; }

	public MutableStatType MutableStatType { get; private set; }

	public List<int> TargetAbnormalMasterIDs { get; private set; }

	public ImmuneElement(AbnormalReason reason, ImmuneType type, float duration = 0f)
		: base(AbnormalCategory.Immune)
	{
		_immuneReason = reason;
		base.Duration = Hub.s.timeutil.ChangeTimeSec2Milli(duration);
		ImmuneCategory = ImmuneCategory.None;
		Type = type;
		Grade = DefAbnormalUtil.ChangeImmuneType2Grade(type);
		CCType = CCType.None;
		ImmutableStatType = StatType.Invalid;
		MutableStatType = MutableStatType.Invalid;
	}

	public override bool Initialize(long syncID, long abnormalObjectID, int casterObjectID, AbnormalInfo info, int index, int initialStack, long duration, PosWithRot? pos = null)
	{
		if (!base.Initialize(syncID, abnormalObjectID, casterObjectID, info, index, initialStack, duration, pos))
		{
			return false;
		}
		if (!(info.ElementList[index] is ImmuneElementInfo immuneElementInfo))
		{
			return false;
		}
		if (Grade == ImmuneGrade.Invalid)
		{
			switch (Type)
			{
			case ImmuneType.TargetImmutableStat:
				SetImmutableStatType(immuneElementInfo.ImmutableStatType);
				break;
			case ImmuneType.TargetCC:
				SetCCType(immuneElementInfo.CCType);
				break;
			case ImmuneType.TargetAbnormalID:
				SetTargetAbnormalMasterIDs(immuneElementInfo.TargetAbnormalMasterIDs);
				break;
			case ImmuneType.TargetMutableStat:
				SetMutableStatType(immuneElementInfo.MutableStatType);
				break;
			}
		}
		Reset(0L);
		Apply(0L);
		return true;
	}

	public override bool SetValue(AbnormalCommonInputArgs args)
	{
		if (!base.SetValue(args))
		{
			return false;
		}
		if (!(args is ImmuneInputArgs immuneInputArgs))
		{
			return false;
		}
		if (Grade == ImmuneGrade.Invalid)
		{
			switch (immuneInputArgs.ImmuneType)
			{
			case ImmuneType.TargetImmutableStat:
				SetImmutableStatType(immuneInputArgs.ImmutableStatType);
				break;
			case ImmuneType.TargetCC:
				SetCCType(immuneInputArgs.CCType);
				break;
			case ImmuneType.TargetMutableStat:
				SetMutableStatType(immuneInputArgs.MutableStatType);
				break;
			}
		}
		Reset(0L);
		return true;
	}

	public bool SetCCType(CCType ccType)
	{
		if (Grade != ImmuneGrade.Invalid)
		{
			return false;
		}
		ImmuneCategory = ImmuneCategory.CC;
		CCType = ccType;
		return true;
	}

	public bool SetImmutableStatType(StatType statType)
	{
		if (Grade != ImmuneGrade.Invalid)
		{
			return false;
		}
		ImmuneCategory = ImmuneCategory.ImmutableStats;
		ImmutableStatType = statType;
		return true;
	}

	public bool SetTargetAbnormalMasterIDs(IEnumerable<int> abnormalMasterIDs)
	{
		if (Grade != ImmuneGrade.Invalid)
		{
			return false;
		}
		ImmuneCategory = ImmuneCategory.AbnormalID;
		TargetAbnormalMasterIDs.AddRange(abnormalMasterIDs);
		return true;
	}

	public bool SetMutableStatType(MutableStatType mutableStatType)
	{
		if (Grade != ImmuneGrade.Invalid)
		{
			return false;
		}
		ImmuneCategory = ImmuneCategory.MutableStats;
		MutableStatType = mutableStatType;
		return true;
	}

	public override void Update(long delta)
	{
		Apply(delta);
	}

	public override void Dispose()
	{
	}
}
