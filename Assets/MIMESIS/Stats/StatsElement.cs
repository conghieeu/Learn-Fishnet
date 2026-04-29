public class StatsElement
{
	public bool IsDirty { get; private set; }

	public long Value { get; private set; }

	public StatsElement(long value, bool dirty = false)
	{
		Value = value;
		IsDirty = dirty;
	}

	public void Set(long value)
	{
		Value = value;
		IsDirty = true;
	}

	public void Add(long value)
	{
		Value += value;
		IsDirty = true;
	}

	public void Add(StatsElement element)
	{
		Value += element.Value;
		IsDirty = true;
	}

	public void Sync(bool flag = false)
	{
		IsDirty = flag;
	}

	public void Clear()
	{
		Value = 0L;
		IsDirty = true;
	}
}
