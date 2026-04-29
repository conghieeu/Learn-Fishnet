using System;
using System.IO;
using Bifrost;
using ReluProtocol;

public class MsgTypePair<T> : IMsgTypePair where T : IMsg, new()
{
	public override IMsg Deserialize(MemoryStream stream)
	{
		return Serializer.Deserialize<T>(stream);
	}

	public override IMsg Deserialize(string jsonBody)
	{
		return Serializer.Deserialize<T>(jsonBody);
	}

	public override IMsg? Deserialize(in ArraySegment<byte> data)
	{
		return Serializer.Deserialize<T>(in data);
	}

	public override byte[] Serialize(IMsg msg)
	{
		return SerializeInternal(msg as T);
	}

	private byte[] SerializeInternal(T msg)
	{
		return Serializer.Serialize(msg);
	}
}
