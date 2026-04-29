using ReluProtocol;

namespace ReluReplay.Data
{
	public class MsgWithTime
	{
		public IMsg msg { get; set; }

		public long time { get; set; }
	}
}
