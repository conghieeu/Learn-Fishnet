using Bifrost.ConstEnum;

namespace Bifrost.Cooked
{
	public class AggroInfo
	{
		public readonly AggroType AggroType;

		public readonly double Weight;

		public readonly double Range;

		public readonly float DistanceForScore;

		public AggroInfo(AggroType aggroType, double weight, double range, float distanceForScore)
		{
			AggroType = aggroType;
			Weight = weight * 0.0001;
			Range = range;
			DistanceForScore = distanceForScore;
		}
	}
}
