using System.Collections.Generic;

public static class EnumeratorHelper
{
	public static BehaviorResult Next(this IEnumerator<BehaviorResult> enumerator)
	{
		if (!enumerator.MoveNext())
		{
			return BehaviorResult.FAILURE;
		}
		return enumerator.Current;
	}
}
