using System.Threading;

public class AtomicFlag
{
	private int atomicValue;

	public bool IsOn => Volatile.Read(ref atomicValue) == 1;

	public AtomicFlag(bool value)
	{
		atomicValue = (value ? 1 : 0);
	}

	public bool On()
	{
		return Interlocked.CompareExchange(ref atomicValue, 1, 0) == 0;
	}

	public bool Off()
	{
		return Interlocked.CompareExchange(ref atomicValue, 0, 1) == 1;
	}
}
