public abstract class IGameEventLog
{
	public readonly GELType GelType;

	public IGameEventLog(GELType gelType)
	{
		GelType = gelType;
	}
}
