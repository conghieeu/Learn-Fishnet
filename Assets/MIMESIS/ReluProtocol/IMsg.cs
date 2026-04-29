using System.Threading;
using MemoryPack;
using ReluProtocol.Enum;

namespace ReluProtocol
{
	public class IMsg
	{
		[MemoryPackIgnore]
		public static int pktHashCodeGenerator;

		public MsgType msgType { get; protected set; }

		public int hashCode { get; set; }

		[MemoryPackIgnore]
		public ParsingType parsingType { get; set; }

		[MemoryPackIgnore]
		public bool reliable { get; set; }

		public IMsg(MsgType msgType, int hashcode = 0)
		{
			this.msgType = msgType;
			if (hashcode == 0)
			{
				hashCode = GenerateHashCode();
			}
			else
			{
				hashCode = hashcode;
			}
		}

		private int GenerateHashCode()
		{
			return Interlocked.Increment(ref pktHashCodeGenerator);
		}
	}
}
