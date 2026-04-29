using Bifrost.ConstEnum;

public struct ImmuneCheckResult
{
	public readonly ImmuneType ImmuneType;

	public readonly bool Immuned;

	public ImmuneCheckResult(ImmuneType immuneType = ImmuneType.None, bool immuned = false)
	{
		ImmuneType = immuneType;
		Immuned = immuned;
	}
}
