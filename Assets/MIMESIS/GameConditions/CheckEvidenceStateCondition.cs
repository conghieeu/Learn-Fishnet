using Bifrost.ConstEnum;

public class CheckEvidenceStateCondition : BaseGameCondition
{
	public int EvidenceMasterID { get; private set; }

	public int State { get; private set; }

	public bool Accomplished { get; private set; }

	public CheckEvidenceStateCondition(int evidenceMasterID, int state)
		: base(DefCondition.CHECK_EVIDENCE_STATE)
	{
		EvidenceMasterID = evidenceMasterID;
		State = state;
		Accomplished = false;
	}

	public override bool Correct(IGameCondition info)
	{
		if (info is CheckEvidenceStateCondition checkEvidenceStateCondition)
		{
			if (checkEvidenceStateCondition.EvidenceMasterID == EvidenceMasterID)
			{
				return checkEvidenceStateCondition.State == State;
			}
			return false;
		}
		return false;
	}

	public override bool IsComplete()
	{
		return Accomplished;
	}

	public override bool IsFailed()
	{
		return false;
	}

	public override bool Progress(int accumulateCount)
	{
		if (Accomplished)
		{
			return false;
		}
		Accomplished = true;
		return true;
	}

	public override int GetCurrentCount()
	{
		if (!Accomplished)
		{
			return 0;
		}
		return 1;
	}

	public override void Clone(ref IGameCondition? info)
	{
		info = new CheckEvidenceStateCondition(EvidenceMasterID, State);
		info.SetIndex(base.ConditionIndex);
	}

	public override IGameCondition Clone()
	{
		CheckEvidenceStateCondition checkEvidenceStateCondition = new CheckEvidenceStateCondition(EvidenceMasterID, State);
		checkEvidenceStateCondition.SetIndex(base.ConditionIndex);
		return checkEvidenceStateCondition;
	}

	public override void ApplyCurrentCondition(int conditionValue)
	{
		if (conditionValue == 1)
		{
			Accomplished = true;
		}
	}

	public override void ForceComplete()
	{
		Accomplished = true;
	}

	public override GameConditionParamType GetLinkedParamType()
	{
		return GameConditionParamType.None;
	}
}
