public struct MutableStatChangeInfo
{
	public readonly MutableStatType MutableStatType;

	public readonly MutableStatChangeMethodType MethodType;

	public readonly long OldImmutableValue;

	public readonly long NewImmutableValue;

	public MutableStatChangeInfo(MutableStatType mutableStatType, MutableStatChangeMethodType methodType, long oldImmutableValue, long newImmutableValue)
	{
		MutableStatType = mutableStatType;
		MethodType = methodType;
		OldImmutableValue = oldImmutableValue;
		NewImmutableValue = newImmutableValue;
	}
}
