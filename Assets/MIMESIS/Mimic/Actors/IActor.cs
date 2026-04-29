using ReluProtocol.Enum;

namespace Mimic.Actors
{
	public interface IActor
	{
		ActorType ActorType { get; }

		int ActorID { get; }
	}
}
