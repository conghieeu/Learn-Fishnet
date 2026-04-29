public class TouchLevelObjectInfo : ILevelObjectInfo
{
	public TouchLevelObjectInfo(LevelObject origin)
		: base(LevelObjectType.Touch, origin)
	{
	}
}
